using Microsoft.Win32;

namespace TouchChan;

// need target to <TargetFramework>netX.0-windows</TargetFramework>
internal class RegistryModifier
{
    private const string ApplicationCompatibilityRegistryPath =
        @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers";
    public static bool IsDpiCompatibilitySetted(string exeFilePath)
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
}
