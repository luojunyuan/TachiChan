// CODE FROM https://stackoverflow.com/questions/217902/reading-writing-an-ini-file
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Text;

namespace Preference
{
    class IniFile   // revision 11
    {

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

        string path;
        string EXE = "TouchChan"; // Assembly.GetExecutingAssembly().GetName().Name!;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string? Key, string? Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        
        private static readonly string RoamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string ConfigFilePath = Path.Combine(RoamingPath, "TouchChan", "Config.ini");
        
        public static readonly string ConfigFolder = Path.Combine(RoamingPath, "TouchChan");

        public IniFile()
        {
            path = ConfigFilePath;
        }

        public string? Read(string Key, string? Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, path);
            return RetVal.ToString() == string.Empty ? null : RetVal.ToString();
        }

        public void Write(string? Key, string? Value, string? Section = null)
        {
            if (!Directory.Exists(ConfigFolder))
                Directory.CreateDirectory(ConfigFolder);

            WritePrivateProfileString(Section ?? EXE, Key, Value, path);
        }

        public void DeleteKey(string Key, string? Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }

        public void DeleteSection(string? Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        public bool KeyExists(string Key, string? Section = null)
        {
            return (Read(Key, Section) ?? string.Empty).Length > 0;
        }
    }
}
