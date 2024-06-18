switch (args.Length)
{
    case 0:
#if RELEASE
        MessageBox.Show(Strings.App_StartNoParameter);
#else
        MessageBox.Show($"{Strings.App_StartNoParameter} {Utils.StartTimeLeftBy()}");
#endif
        return;
    case 1 when int.TryParse(args[0], out var pid):
        TouchLauncher.Run(Utils.GetProcessById(pid));
        return;
}

// args[0] /InvokerPRAID --path path_to_game
var index = Array.FindIndex<string>(args, i => i is "-p" or "--path");
var gamePath = args[index + 1];
if (File.Exists(gamePath) && Path.GetExtension(gamePath).Equals(".lnk", StringComparison.OrdinalIgnoreCase))
{
    gamePath = Utils.ExtractLnkPath(gamePath);
}

if (!File.Exists(gamePath))
{
    MessageBox.Show(Strings.App_StartInvalidPath + $" \"{gamePath}\"");
    return;
}

// The small device means dpi settings did not make the item large enough for the device
var smallDevice = args.Contains("--small-device") || RegistryModifier.IsSmallDevice();
var image = typeof(Program).Assembly.GetManifestResourceStream("TouchChan.assets.klee.png")!;
var splash = new SplashScreenGdip.SplashScreen(smallDevice? 144 : 96, image);

// Host thread
new Thread(() =>
{
    System.Diagnostics.Process? leProc;
    try
    {
        leProc = AppLauncher.RunGame(gamePath, args.Contains("-le"));
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

    // Wait for MainHandle and contain .log main.bin process
    var (game, _) = AppLauncher.ProcessCollect(Path.GetFileNameWithoutExtension(gamePath));
    if (game is null)
    {
        splash.Close();
        MessageBox.Show(Strings.App_Timeout);
        return;
    }

    // Kill LE process after collect game process
    leProc?.Kill();

    AppLauncher.ForceSetForegroundWindow(game.MainWindowHandle);

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

    TouchLauncher.Run(game, splash);
}).Start();

splash.Run();
