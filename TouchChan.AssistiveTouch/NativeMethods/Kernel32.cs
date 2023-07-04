using System.Runtime.InteropServices;
using System.Text;

namespace TouchChan.AssistiveTouch.NativeMethods
{
    internal class Kernel32
    {
        private const string Kernel32Dll = "kernel32.dll";

        [DllImport(Kernel32Dll, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle([Optional] string lpModuleName);

        [DllImport(Kernel32Dll, CharSet = CharSet.Unicode)]
        public static extern long WritePrivateProfileString(string section, string? key, string? value, string filePath);

        [DllImport(Kernel32Dll, CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileString(string section, string key, string @default, StringBuilder retVal, int size, string filePath);

        [DllImport(Kernel32Dll)]
        public static extern uint GetCurrentThreadId();
    }
}
