namespace TouchChan.AssistiveTouch.NativeMethods
{
    /// <summary>
    /// Just Warpper
    /// </summary>
    internal static class Win32
    {
        public static void MoveWindowToOrigin(IntPtr handle) => User32.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, User32.SetWindowPosFlags.SWP_NOZORDER | User32.SetWindowPosFlags.SWP_NOSIZE);
        public static void MoveWindow(IntPtr handle, int x, int y) => User32.SetWindowPos(handle, IntPtr.Zero, x, y, 0, 0, User32.SetWindowPosFlags.SWP_NOSIZE | User32.SetWindowPosFlags.SWP_NOZORDER);
        public static void SetWindowSize(IntPtr handle, int width, int height) => User32.SetWindowPos(handle, IntPtr.Zero, 0, 0, width, height, User32.SetWindowPosFlags.SWP_NOMOVE | User32.SetWindowPosFlags.SWP_NOZORDER);
    }
}
