// See https://aka.ms/new-console-template for more information
using TouchChan;
using SplashScreenGdip;
using System.Diagnostics;
using System.IO.Pipes;

if (args.Length == 1 && int.TryParse(args[0], out var pid))
{
    Run(Process.GetProcessById(pid));
    return;
}

#region Arguments Check
if (args.Length == 0)
{
#if RELEASE
    MessageBox.Show(Strings.App_StartNoParameter);
#else
    MessageBox.Show($"{Strings.App_StartNoParameter}({(DateTime.Now - Process.GetCurrentProcess().StartTime).Milliseconds})");
#endif
    return;
}
// args[0] /InvokerPRAID
var index = Array.FindIndex<string>(args, i => i == "-p" || i == "--path");
var gamePath = args[index + 1];
if (File.Exists(gamePath) && Path.GetExtension(gamePath).Equals(".lnk", StringComparison.OrdinalIgnoreCase))
{
    gamePath = WinShortcutWrapper(gamePath);
}
if (!File.Exists(gamePath))
{
    MessageBox.Show(Strings.App_StartInvalidPath + $" \"{gamePath}\"");
    return;
}
#endregion

var stream = typeof(Program).Assembly.GetManifestResourceStream("TouchChan.assets.klee.png")!;
var splash = new SplashScreen(96, stream);
new Thread(() => PreProcessing(args.Contains("-le"), gamePath, splash)).Start();

splash.Run();


static string WinShortcutWrapper(string gamePath) =>
    WindowsShortcutFactory.WindowsShortcut.Load(gamePath).Path ?? "Resolve lnk file failed";

#pragma warning disable CS8321 // ローカル関数は宣言されていますが、一度も使用されていません
static string GetTargetPath(string lnkFilePath)
{
    byte[] lnkBytes = File.ReadAllBytes(lnkFilePath);

    int targetPathStartIndex = 0x1C;
    int targetPathLength = BitConverter.ToInt32(lnkBytes, targetPathStartIndex);
    string targetPath = System.Text.Encoding.Unicode.GetString(lnkBytes, targetPathStartIndex + 4, targetPathLength * 2);

    return targetPath;
}
#pragma warning restore CS8321 // ローカル関数は宣言されていますが、一度も使用されていません

static void PreProcessing(bool leEnable, string gamePath, SplashScreen splash)
{
    #region Start Game
    Process? leProc;
    try
    {
        leProc = AppLauncher.RunGame(gamePath, leEnable);
    }
    catch (ArgumentException ex) when (ex.Message == string.Empty)
    {
        splash.Close();
        MessageBox.Show(Strings.App_LENotSetup);
        return;
    }
    catch (ArgumentException ex) when (ex.Message != string.Empty)
    {
        splash.Close();
        MessageBox.Show(Strings.App_LENotFound + ex.Message);
        return;
    }
    catch (InvalidOperationException)
    {
        splash.Close();
        MessageBox.Show(Strings.App_LENotSupport);
        return;
    }

    var (game, pids) = AppLauncher.ProcessCollect(Path.GetFileNameWithoutExtension(gamePath));
    if (game is null)
    {
        splash.Close();
        MessageBox.Show(Strings.App_Timeout);
        return;
    }
    leProc?.Kill();

    try
    {
        _ = game.HasExited;
    }
    catch (System.ComponentModel.Win32Exception)
    {
        splash.Close();
        MessageBox.Show(Strings.App_ElevatedError);
        return;
    }
    #endregion

    Run(game, splash);

    // prevent exception when startup
    splash.Close();
}

static void Run(Process game, SplashScreen? splash = null)
{
    var pipeServer = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
    _ = new IpcMain(pipeServer);

    if (splash != null) IpcMain.Once("Loaded", () =>
    {
        splash!.Close();
        splash = null;
    });

    Environment.CurrentDirectory = AppContext.BaseDirectory;
    while (!game.HasExited)
    {
        var gameWindowHandle = AppLauncher.FindMainWindowHandle(game);
        if (gameWindowHandle == IntPtr.Zero) // process exit
        {
            if (splash != null)
            {
                splash.Close();
                MessageBox.Show(Strings.App_Unexpected);
            }
            break;
        }
        else if (gameWindowHandle.ToInt32() == -1) // FindHandleFailed
        {
            splash?.Close();
            MessageBox.Show(Strings.App_Timeout);
            break;
        }

        var touch = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "..\\TouchChan.AssistiveTouch\\TouchChan.AssistiveTouch.exe",
                Arguments = pipeServer.GetClientHandleAsString() + ' ' + gameWindowHandle,
                UseShellExecute = false,
            }
        };

        touch.Start();

        touch.WaitForExit();
    }
}
