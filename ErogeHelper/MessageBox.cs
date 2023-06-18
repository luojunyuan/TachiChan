using SplashScreenGdip;
using System.Runtime.InteropServices;

namespace ErogeHelper;

internal class MessageBox
{
    public static void Show(string text, string title = "ErogeHelper", IntPtr parent = default) => _ = _MessageBox(parent, text, title, MB_TOPMOST | MB_SETFOREGROUND);

    [DllImport("user32.dll", EntryPoint = "MessageBoxA", ExactSpelling = true, CharSet = CharSet.Ansi)]
    public static extern int _MessageBox(IntPtr hWnd, string lpText, string lpCaption, int uType);

    // Not work if the process had win handle
    const int MB_TOPMOST = 0x00040000;
    const int MB_SETFOREGROUND = 0x00010000;
    // https://stackoverflow.com/a/48003123
}
