using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace Preference;

internal class FilterProcessService
{
    private const string UwpAppsTag = "WindowsApps";
    private const string WindowsPath = @"C:\Windows\";
    private const string WindowsPathUpperCase = @"C:\WINDOWS\";
    private const int MaxTitleLength = 40;

    private static readonly int EnvironmentProcessId = Process.GetCurrentProcess().Id;

    public event Action<bool>? ShowAdminNeededTip;

    public IEnumerable<ProcessDataModel> Filter() =>
        Process.GetProcesses()
            .Where(p => p.MainWindowHandle != IntPtr.Zero && p.MainWindowTitle != string.Empty)
            .Where(p =>
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
                    ShowAdminNeededTip?.Invoke(true);
                    ShowAdminNeededTip = null;
                }
                catch (Win32Exception ex) when (ex.NativeErrorCode == 299)
                {
                    // 32bit -> 64bit
                    Debug.WriteLine($"{p.MainWindowTitle} {ex.Message}");
                }

                return fileName is not null &&
                    !fileName.Contains(UwpAppsTag) &&
                    !fileName.Contains(WindowsPath) &&
                    !fileName.Contains(WindowsPathUpperCase) &&
                    p.Id != EnvironmentProcessId;
            })
            .Select(p =>
            {
                var fileName = p.MainModule?.FileName!;
                var icon = PeIconToBitmapImage(fileName);
                var describe = p.MainModule?.FileVersionInfo.FileDescription ?? string.Empty;
                var title = p.MainWindowTitle.Length > MaxTitleLength && describe != string.Empty ?
                    describe : p.MainWindowTitle;
                return new ProcessDataModel(p, icon, describe, title);
            });

    private static Bitmap PeIconToBitmapImage(string fullPath)
    {
        Stream stream = new MemoryStream();

        var iconBitmap = (Icon.ExtractAssociatedIcon(fullPath) ?? throw new InvalidOperationException()).ToBitmap();
        iconBitmap.Save(stream, ImageFormat.Png);
        iconBitmap.Dispose();

        return iconBitmap;
    }
}


public class ProcessDataModel : IEquatable<ProcessDataModel>
{
    public ProcessDataModel(Process process, Bitmap icon, string describe, string title)
    {
        Proc = process;
        Icon = icon;
        Describe = describe;
        Title = title;
    }

    public Process Proc { get; }

    public Bitmap Icon { get; }

    public string Describe { get; }

    public string Title { get; }

    public bool Equals(ProcessDataModel? other) =>
        other is not null &&
        (ReferenceEquals(this, other) || Proc.Id == other.Proc.Id);

    #region Override
    public override bool Equals(object? obj) =>
        obj is not null &&
        (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals(other: (ProcessDataModel)obj));

    public override int GetHashCode() => Proc.Id.GetHashCode();
    #endregion
}

