// See https://aka.ms/new-console-template for more information
using ErogeHelper;
using Microsoft.Toolkit.Uwp.Notifications;
using SplashScreenGdip;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;

if (args.Contains("-channel"))
{
    var connection = new AppServiceConnection
    {
        AppServiceName = "CommunicationService",
        PackageFamilyName = Package.Current.Id.FamilyName
    };
    await connection.OpenAsync();

    var result = FilterProcessService.Filter();
    var jsonString = JsonSerializer.Serialize(result, typeof(IEnumerable<ProcessDataModel>), SourceGenerationContext.Default);
    var valueSet = new ValueSet
    {
        { "result", jsonString }
    };
    await connection.SendMessageAsync(valueSet);
    return;
}

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

var stream = typeof(Program).Assembly.GetManifestResourceStream("ErogeHelper.assets.klee.png")!;
var splash = new SplashScreen(96, stream);
new Thread(() => PreProcessing(args.Contains("-le"), gamePath, splash)).Start();

splash.Run();


static string WinShortcutWrapper(string gamePath) =>
    WindowsShortcutFactory.WindowsShortcut.Load(gamePath).Path ?? "Resolve lnk file failed";

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

        // TODO: net8 Environment.IsPrivilegedProcess
        if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
        {
            new ToastContentBuilder()
                .AddText("ErogeHelper is running as admin")
                .Show(t =>
                {
                    t.Tag = "eh";
                    t.Dismissed += (_, _) => ToastNotificationManagerCompat.History.Remove("eh");
                    // t.ExpirationTime = DateTime.Now; // ExpirationTime seems not stable
                }); 
        }
        var touch = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "..\\ErogeHelper.AssistiveTouch\\ErogeHelper.AssistiveTouch.exe",
                Arguments = pipeServer.GetClientHandleAsString() + ' ' + gameWindowHandle,
                UseShellExecute = false,
            }
        };

        touch.Start();

        if (AppdataRoming.UseEnterKeyMapping())
        {
            // Exited with touch
            ProcessStart.GlobalKeyHook(touch.Id, gameWindowHandle);
        }

        touch.WaitForExit();
    }
}
