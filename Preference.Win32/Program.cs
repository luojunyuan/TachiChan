namespace Preference;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        Environment.CurrentDirectory = AppContext.BaseDirectory;
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        Form1? form1;
        if (args.Length == 1)
        {
            if (args[0] == "--install")
                form1 = new Form1(1);
            else if (args[0] == "--uninstall")
                form1 = new Form1(2);
            else
                form1 = new Form1();
        }
        else
        {
            form1 = new Form1();
        }

        Application.Run(form1);
    }
}

#if NET472
internal static partial class ApplicationConfiguration
{
    public static void Initialize()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        //Application.SetHighDpiMode(HighDpiMode.SystemAware);
        // Or set dpi aware in manifest instead
        SetProcessDPIAware(); // SystemAware
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();
}
#endif
