using Microsoft.Win32;

namespace TouchChan;

// Reference to Microsoft.Win32 need target to <TargetFramework>netX.0-windows</TargetFramework>
internal class RegistryModifier
{
    private const string ApplicationCompatibilityRegistryPath =
        @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers";
    public static bool IsDpiCompatibilitySet(string exeFilePath)
    {
        using var key = Registry.CurrentUser.OpenSubKey(ApplicationCompatibilityRegistryPath)
            ?? Registry.CurrentUser.CreateSubKey(ApplicationCompatibilityRegistryPath);

        var currentValue = key.GetValue(exeFilePath) as string;
        if (currentValue is null)
            return false;

        var DpiSettings = new string[3] { "HIGHDPIAWARE", "DPIUNAWARE", "GDIDPISCALING DPIUNAWARE" };
        var currentValueList = currentValue.Split(' ');
        return DpiSettings.Any(v => currentValueList.Contains(v));
    }
    public static void SetDPICompatibilityAsApplication(string exeFilePath)
    {
        using var key = Registry.CurrentUser.OpenSubKey(ApplicationCompatibilityRegistryPath, true)
            ?? Registry.CurrentUser.CreateSubKey(ApplicationCompatibilityRegistryPath);

        var currentValue = key.GetValue(exeFilePath) as string;
        if (string.IsNullOrEmpty(currentValue))
            key.SetValue(exeFilePath, "~ HIGHDPIAWARE");
        else
            key.SetValue(exeFilePath, currentValue + " HIGHDPIAWARE");
    }

    private const string LERegistryPath = @"Software\Classes\CLSID\{C52B9871-E5E9-41FD-B84D-C5ACADBEC7AE}\InprocServer32";
    public static string LEPathInRegistry()
    {
        using var key = Registry.CurrentUser.OpenSubKey(LERegistryPath) ??
            Registry.LocalMachine.OpenSubKey(LERegistryPath);
        if (key is null)
            return string.Empty;

        var rawPath = key.GetValue("CodeBase") as string;
        if (rawPath is null)
            return string.Empty;

        var handleDllPath = rawPath.Substring(8);
        var dir = Path.GetDirectoryName(handleDllPath);
        if (dir is null)
            return string.Empty;

        return Path.Combine(dir, "LEProc.exe");
    }
}
