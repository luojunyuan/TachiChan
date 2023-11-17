// See https://aka.ms/new-console-template for more information
using SplashScreenGdip;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using TouchChan;


if (args.Length == 1 && int.TryParse(args[0], out var pid))
{
    Run(Process.GetProcessById(pid, null, true));
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
    gamePath = ExtractLnkPath(gamePath);
}
if (!File.Exists(gamePath))
{
    MessageBox.Show(Strings.App_StartInvalidPath + $" \"{gamePath}\"");
    return;
}
#endregion

var stream = typeof(Program).Assembly.GetManifestResourceStream("TouchChan.assets.klee.png")!;
var splash = new SplashScreen(
    args.Contains("--small-device") ? 144 : 
    RegistryModifier.IsSmallDevice() ? 144 : 96, stream);
new Thread(() => PreProcessing(args.Contains("-le"), gamePath, splash)).Start();

splash.Run();


static string ExtractLnkPath(string file)
{
    try
    {
        FileStream fileStream = File.Open(file, FileMode.Open, FileAccess.Read);
        using (System.IO.BinaryReader fileReader = new BinaryReader(fileStream))
        {
            fileStream.Seek(0x14, SeekOrigin.Begin);     // Seek to flags
            uint flags = fileReader.ReadUInt32();        // Read flags
            if ((flags & 1) == 1)
            {                      // Bit 1 set means we have to
                                   // skip the shell item ID list
                fileStream.Seek(0x4c, SeekOrigin.Begin); // Seek to the end of the header
                uint offset = fileReader.ReadUInt16();   // Read the length of the Shell item ID list
                fileStream.Seek(offset, SeekOrigin.Current); // Seek past it (to the file locator info)
            }

            long fileInfoStartsAt = fileStream.Position; // Store the offset where the file info
            // structure begins
            uint totalStructLength = fileReader.ReadUInt32(); // read the length of the whole struct
            fileStream.Seek(0xc, SeekOrigin.Current); // seek to offset to base pathname
            uint fileOffset = fileReader.ReadUInt32(); // read offset to base pathname
            // the offset is from the beginning of the file info struct (fileInfoStartsAt)
            fileStream.Seek((fileInfoStartsAt + fileOffset), SeekOrigin.Begin); // Seek to beginning of
            // base pathname (target)
            long pathLength = (totalStructLength + fileInfoStartsAt) - fileStream.Position - 2; // read
            // the base pathname. I don't need the 2 terminating nulls.
            var linkTarget = fileReader.ReadBytes((int)pathLength); // should be unicode safe
            // error in VS but properly in context menu? "C:\Users\k1mlk\Desktop\金色ラブリッチェ.lnk"
#if !NET472
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            var link = Encoding.GetEncoding(0).GetString(linkTarget);

            int begin = link.IndexOf("\0\0");
            if (begin > -1)
            {
                int end = link.IndexOf("\\\\", begin + 2) + 2;
                end = link.IndexOf('\0', end) + 1;

                string firstPart = link.Substring(0, begin);
                string secondPart = link.Substring(end);

                return firstPart + secondPart;
            }
            else
            {
                return link;
            }
        }
    }
    catch
    {
        return "error when extract lnk.";
    }
}

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

static void Run(Process game, SplashScreen? splash = null, bool enableOldStyle = false)
{
    var pipeServer = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
    _ = new IpcMain(pipeServer);

    if (splash != null) IpcMain.Once("Loaded", () =>
    {
        splash!.Close();
        splash = null;
    });

    var smallDevice = Environment.GetCommandLineArgs().Contains("--small-device") || RegistryModifier.IsSmallDevice() ? " --small-device" : string.Empty;
    var oldStyle = Environment.GetCommandLineArgs().Contains("--no-dpi-compatible") || enableOldStyle ? " --no-dpi-compatible" : string.Empty;

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

        // Tip: Make sure AssistiveTouch exist when you debug
        var touch = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
#if !NET472
                FileName = "..\\TouchChan.AssistiveTouch\\TouchChan.AssistiveTouch.exe",
#else
                FileName = "TouchChan.AssistiveTouch.exe",
#endif
                Arguments = pipeServer.GetClientHandleAsString() + ' ' + gameWindowHandle + smallDevice + oldStyle,
                UseShellExecute = false,
            }
        };

        touch.Start();

        touch.WaitForExit();
    }
}
