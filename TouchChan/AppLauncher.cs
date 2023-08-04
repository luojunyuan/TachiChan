using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TouchChan;

internal static class AppLauncher
{
    public static Process? RunGame(string gamePath, bool leEnable)
    {
        var gameAlreadyStart = GetProcessesByFriendlyName(Path.GetFileNameWithoutExtension(gamePath)).Any();
        if (gameAlreadyStart)
            return null;

        var gameFolder = Path.GetDirectoryName(gamePath);

        if (!RegistryModifier.IsDpiCompatibilitySet(gamePath))
        {
            RegistryModifier.SetDPICompatibilityAsApplication(gamePath);
        }

        if (leEnable)
        {
            var lePath = RegistryModifier.LEPathInRegistry();
            if (lePath == string.Empty)
                throw new ArgumentException(lePath);
            if (!File.Exists(lePath))
                throw new ArgumentException(lePath);
            if (Path.GetFileNameWithoutExtension(lePath).ToLower() == "leproc"
                && PEFileReader.GetPEType(gamePath) != PEType.X32)
                throw new InvalidOperationException();

            // NOTE: LE may throw AccessViolationException which can not be caught
            return Process.Start(new ProcessStartInfo
            {
                FileName = lePath,
                UseShellExecute = false,
                Arguments = File.Exists(gamePath + ".le.config")
                    ? $"-run \"{gamePath}\""
                    : $"\"{gamePath}\""
            });
        }
        else
        {
            var isUrlFile = Path.GetExtension(gamePath).Equals(".url", StringComparison.OrdinalIgnoreCase);
            Process.Start(new ProcessStartInfo
            {
                FileName = gamePath,
                UseShellExecute = isUrlFile,
                WorkingDirectory = gameFolder
            });
            return null;
        }
    }

    public const int UIMinimumResponseTime = 50;

    /// <summary>
    /// Get all processes ids of the game (till found valid window handle, timeout 20s).
    /// </summary>
    /// <param name="friendlyName">aka <see cref="Process.ProcessName"/>, the process name equal filename</param>
    public static (Process?, int[]) ProcessCollect(string friendlyName)
    {
        var spendTime = new Stopwatch();
        spendTime.Start();
        var procList = GetProcessesByFriendlyName(friendlyName);
        var mainProcess = procList.FirstOrDefault(p => p.MainWindowHandle != IntPtr.Zero);
        const int WaitGameStartTimeout = 20000;

        while (mainProcess is null && spendTime.Elapsed.TotalMilliseconds < WaitGameStartTimeout)
        {
            Thread.Sleep(UIMinimumResponseTime);
            procList = GetProcessesByFriendlyName(friendlyName);
            mainProcess = procList.FirstOrDefault(p => p.MainWindowHandle != IntPtr.Zero);
        }
        spendTime.Stop();

        if (mainProcess is null)
            return (null, Array.Empty<int>());

        var idx = procList.FindIndex(x => x.Id == mainProcess.Id);
        procList.RemoveAt(idx);
        procList.Insert(0, mainProcess);

        return (mainProcess, procList.Select(p => p.Id).ToArray());
    }

    private static List<Process> GetProcessesByFriendlyName(string friendlyName)
    {
        var processes = new List<Process>();
        processes.AddRange(Process.GetProcessesByName(friendlyName));
        processes.AddRange(Process.GetProcessesByName(friendlyName + ".log"));
        if (!friendlyName.Equals("main.bin", StringComparison.Ordinal))
            processes.AddRange(Process.GetProcessesByName("main.bin"));
        return processes;
    }

    public static IntPtr FindMainWindowHandle(Process proc)
    {
        const int WaitGameStartTimeout = 20000;
        const int UIMinimumResponseTime = 50;

        proc.WaitForInputIdle(WaitGameStartTimeout);
        proc.Refresh();
        // Might be zero at first
        var gameHwnd = proc.MainWindowHandle;

        if (User32.IsIconic(proc.MainWindowHandle))
        {
            User32.ShowWindow(proc.MainWindowHandle, User32.ShowWindowCommand.SW_RESTORE);
        }

        User32.GetClientRect(gameHwnd, out var clientRect);

        if (clientRect.bottom > GoodWindowHeight &&
            clientRect.right > GoodWindowWidth)
        {
            return gameHwnd;
        }
        else
        {
            var spendTime = new Stopwatch();
            spendTime.Start();
            while (spendTime.Elapsed.TotalMilliseconds < WaitGameStartTimeout)
            {
                if (proc.HasExited)
                    return IntPtr.Zero;

                // Process.MainGameHandle should included in handles
                var handles = GetRootWindowsOfProcess(proc.Id);
                foreach (var handle in handles)
                {
                    User32.GetClientRect(handle, out clientRect);
                    if (clientRect.bottom > GoodWindowHeight &&
                        clientRect.right > GoodWindowWidth)
                    {
                        return handle;
                    }
                }
                Thread.Sleep(UIMinimumResponseTime);
            }
            // throw new ArgumentException("Find window handle failed");
            return (IntPtr)(-1);
        }
    }

    // private const int VNRWindowWidth = 160;
    // private const int VNRWindowHeight = 120;
    // private const int MinWindowSize = 12;
    private const int GoodWindowWidth = 500;
    private const int GoodWindowHeight = 320;

    private static IEnumerable<IntPtr> GetRootWindowsOfProcess(int pid)
    {
        var rootWindows = GetChildWindows(IntPtr.Zero);
        var dsProcRootWindows = new List<IntPtr>();
        foreach (var hWnd in rootWindows)
        {
            _ = User32.GetWindowThreadProcessId(hWnd, out var lpdwProcessId);
            if (lpdwProcessId == pid)
                dsProcRootWindows.Add(hWnd);
        }
        return dsProcRootWindows;
    }

    private static IEnumerable<IntPtr> GetChildWindows(IntPtr parent)
    {
        List<IntPtr> result = new();
        var listHandle = GCHandle.Alloc(result);
        try
        {
            static bool ChildProc(IntPtr handle, IntPtr pointer)
            {
                var gch = GCHandle.FromIntPtr(pointer);
                if (gch.Target is not List<IntPtr> list)
                {
                    throw new InvalidCastException("GCHandle Target could not be cast as List<HWND>");
                }
                list.Add(handle);
                return true;
            }
            User32.EnumChildWindows(parent, ChildProc, GCHandle.ToIntPtr(listHandle));
        }
        finally
        {
            if (listHandle.IsAllocated)
                listHandle.Free();
        }
        return result;
    }

    internal class User32
    {
        private const string User32Dll = "user32.dll";

        [DllImport(User32Dll, SetLastError = false, ExactSpelling = true)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport(User32Dll, SetLastError = false, ExactSpelling = true)]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommand nCmdShow);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate bool EnumWindowsProc([In] IntPtr hwnd, [In] IntPtr lParam);

        [DllImport(User32Dll, SetLastError = false, ExactSpelling = true)]
        public static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport(User32Dll, SetLastError = false, ExactSpelling = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport(User32Dll, CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public int Width
            {
                get { return right - left; }
                set { right = value + left; }
            }

            public int Height
            {
                get { return bottom - top; }
                set { bottom = value + top; }
            }

            public Size Size
            {
                get { return new Size(Width, Height); }
                set { Width = value.Width; Height = value.Height; }
            }
        }

        public enum ShowWindowCommand
        {
            SW_RESTORE = 9,
        }
    }
}
