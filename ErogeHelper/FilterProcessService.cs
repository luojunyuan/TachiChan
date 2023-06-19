using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text.Json.Serialization;

namespace ErogeHelper;

internal class FilterProcessService
{
    private const string UwpAppsTag = "WindowsApps"; // much more easier
    private const string WindowsPath = @"C:\Windows\";
    private const int MaxTitleLength = 80;

    public static IEnumerable<ProcessDataModel> Filter() =>
        Process.GetProcesses()
            .Where(p => p.MainWindowHandle != nint.Zero && p.MainWindowTitle != string.Empty)
            .Where(p =>
            {
                var fileName = RetrieveFilename(p);

                return fileName is not null &&
                    !fileName.Contains(UwpAppsTag) &&
                    !fileName.Contains(WindowsPath) &&
                    !fileName.Contains(WindowsPath.ToUpper()) &&
                    p.Id != Environment.ProcessId;
            })
            .Select(p => (p.Id, p.MainModule!.FileName, p.MainWindowTitle, p.MainModule!.FileVersionInfo.FileDescription ?? string.Empty))
            .Select(x =>
            {
                var (id, fileName, windowTitle, fileDescription) = x;
                var cjkCount = windowTitle.Count(c => IsCJKCharacter(c));
                var title = windowTitle.Length + cjkCount > MaxTitleLength && fileDescription != string.Empty ?
                    windowTitle : windowTitle;
                using var stream = new MemoryStream();
                if (Icon.ExtractAssociatedIcon(fileName) is Icon iconValid)
                    iconValid.ToBitmap().Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    
                return new ProcessDataModel(id, fileDescription, title, fileName, stream.ToArray());
            });

    private static string? RetrieveFilename(Process p)
    {
        string? fileName = null;
        try
        {
            fileName = p.MainModule?.FileName;
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 5)
        {
            // "Access is denied." need for elevating permissions
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


public record ProcessDataModel(int ProcessId, string Describe, string Title, string FullPath, byte[] IconBytes);

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(IEnumerable<ProcessDataModel>))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}