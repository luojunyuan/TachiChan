using TouchChan.AssistiveTouch.NativeMethods;

namespace TouchChan.AssistiveTouch.Helper;

public static class HwndTools
{
    public static void RemovePopupAddChildStyle(IntPtr handle)
    {
        var style = (uint)User32.GetWindowLong(handle, User32.WindowLongFlags.GWL_STYLE);
        style = style & ~(uint)User32.WindowStyles.WS_POPUP | (uint)User32.WindowStyles.WS_CHILD;
        User32.SetWindowLong(handle, User32.WindowLongFlags.GWL_STYLE, (int)style);
    }

    public static void WindowLostFocus(IntPtr windowHandle, bool loseFocus)
    {
        if (windowHandle == IntPtr.Zero)
            return;

        var exStyle = User32.GetWindowLong(windowHandle, User32.WindowLongFlags.GWL_EXSTYLE);
        if (loseFocus)
        {
            User32.SetWindowLong(windowHandle,
                User32.WindowLongFlags.GWL_EXSTYLE,
                exStyle | (int)User32.WindowStylesEx.WS_EX_NOACTIVATE);
        }
        else
        {
            User32.SetWindowLong(windowHandle,
                User32.WindowLongFlags.GWL_EXSTYLE,
                exStyle & ~(int)User32.WindowStylesEx.WS_EX_NOACTIVATE);
        }
    }
}
