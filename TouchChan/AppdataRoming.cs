using System.Runtime.InteropServices;
using System.Text;

namespace TouchChan;

public class AppdataRoming
{
    private static readonly string RoamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    private static readonly string ConfigFilePath = Path.Combine(RoamingPath, "TouchChan", "EHConfig.ini");

    public static bool IsDpiAppDisabled()
    {
        var valueBuilder = new StringBuilder(255);
        Kernel32.GetPrivateProfileString("TouchChan", "DpiAppDisabled", string.Empty, valueBuilder, 255, ConfigFilePath);
        if (valueBuilder.ToString() == string.Empty)
            return false;
        return bool.Parse(valueBuilder.ToString());
    }

    public static string GetLEPath()
    {
        var valueBuilder = new StringBuilder(255);
        Kernel32.GetPrivateProfileString("TouchChan", "LEPath", string.Empty, valueBuilder, 255, ConfigFilePath);
        return valueBuilder.ToString();
    }

    public static bool UseEnterKeyMapping()
    {
        var valueBuilder = new StringBuilder(255);
        Kernel32.GetPrivateProfileString("TouchChan", nameof(UseEnterKeyMapping), string.Empty, valueBuilder, 255, ConfigFilePath);
        if (valueBuilder.ToString() == string.Empty)
            return false;
        return bool.Parse(valueBuilder.ToString());
    }

    public class Kernel32
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileString(string section, string key, string @default, StringBuilder retVal, int size, string filePath);
    }
}
