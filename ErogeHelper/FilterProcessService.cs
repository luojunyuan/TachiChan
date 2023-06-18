using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ErogeHelper;

internal class FilterProcessService
{
    private const string WindowsPath = @"C:\Windows\";
    private const int MaxTitleLength = 80;

    public static IEnumerable<ProcessDataModel> Filter() =>
        Process.GetProcesses()
            .Where(p => p.MainWindowHandle != nint.Zero && p.MainWindowTitle != string.Empty)
            .Where(p =>
            {
                var fileName = RetrieveFilename(p);

                return fileName is not null &&
                    !fileName.Contains(WindowsPath) &&
                    !fileName.Contains(WindowsPath.ToUpper()) &&
                    p.Id != Environment.ProcessId;
            })
            .Select(p =>
            {
                var cjkCount = p.MainWindowTitle.Count(c => IsCJKCharacter(c));
                var fileName = p.MainModule?.FileName!;
                var describe = p.MainModule?.FileVersionInfo.FileDescription ?? string.Empty;
                var title = p.MainWindowTitle.Length + cjkCount > MaxTitleLength && describe != string.Empty ?
                    describe : p.MainWindowTitle;
                return new ProcessDataModel(p.Id, describe, title, fileName);
            });

    private static string? RetrieveFilename(Process p)
    {
        string? fileName = null;
        try
        {
            fileName = p.MainModule?.FileName;
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 5) // Access is denied.
        {
            // need elevated permissions
            Debug.WriteLine($"{p.MainWindowTitle} {ex.Message}");
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 299)
        {
            // 32bit -> 64bit
            Debug.WriteLine($"{p.MainWindowTitle} {ex.Message}");
        }

        return fileName;
    }

    private static bool IsCJKCharacter(char c) => 
        (c >= 0x4E00 && c <= 0x9FFF) ||
        (c >= 0x3400 && c <= 0x4DBF) ||
        (c >= 0x20000 && c <= 0x2A6DF) ||
        (c >= 0xF900 && c <= 0xFAFF);
}


public record ProcessDataModel(int ProcessId, string Describe, string Title, string FullPath);

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(IEnumerable<ProcessDataModel>))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}