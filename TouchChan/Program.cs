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

var image = typeof(Program).Assembly.GetManifestResourceStream("TouchChan.assets.klee.png")!;
var splash = new SplashScreenGdip.SplashScreen(
    args.Contains("--small-device") ? 144 :
    RegistryModifier.IsSmallDevice() ? 144 : 96, image);

// Host thread
new Thread(() =>
{
    var game = AppLauncher.PreProcessing(args.Contains("-le"), gamePath, splash);
    if (game is not null)
        TouchLauncher.Run(game, splash);
}).Start();

splash.Run();
