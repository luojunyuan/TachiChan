#nullable enable
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UnhandledExceptionEventArgs = Windows.UI.Xaml.UnhandledExceptionEventArgs;

namespace Preference;

/// <summary>
/// 既定の Application クラスを補完するアプリケーション固有の動作を提供します。
/// </summary>
sealed partial class App : Application
{
    /// <summary>
    ///単一アプリケーション オブジェクトを初期化します。これは、実行される作成したコードの
    ///最初の行であるため、論理的には main() または WinMain() と等価です。
    /// </summary>
    public App()
    {
        UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedException;

        this.InitializeComponent();
        this.Suspending += OnSuspending;
    }

    /// <summary>
    /// アプリケーションがエンド ユーザーによって正常に起動されたときに呼び出されます。他のエントリ ポイントは、
    /// アプリケーションが特定のファイルを開くために起動されたときなどに使用されます。
    /// </summary>
    /// <param name="e">起動の要求とプロセスの詳細を表示します。</param>
    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        var rootFrame = Window.Current.Content as Frame;

        // ウィンドウに既にコンテンツが表示されている場合は、アプリケーションの初期化を繰り返さずに、
        // ウィンドウがアクティブであることだけを確認してください
        if (rootFrame == null)
        {
            // ナビゲーション コンテキストとして動作するフレームを作成し、最初のページに移動します
            rootFrame = new Frame();

            rootFrame.NavigationFailed += OnNavigationFailed;

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: 以前中断したアプリケーションから状態を読み込みます
            }

            // フレームを現在のウィンドウに配置します
            Window.Current.Content = rootFrame;
        }

        if (e.PrelaunchActivated == false)
        {
            if (rootFrame.Content == null)
            {
                // ナビゲーションの履歴スタックが復元されていない場合、最初のページに移動します。
                // このとき、必要な情報をナビゲーション パラメーターとして渡して、新しいページを
                // 作成します
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }

            ApplicationView.PreferredLaunchViewSize = new Size(500, 320);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            // 現在のウィンドウがアクティブであることを確認します
            Window.Current.Activate();
        }
    }

    /// <summary>
    /// 特定のページへの移動が失敗したときに呼び出されます
    /// </summary>
    /// <param name="sender">移動に失敗したフレーム</param>
    /// <param name="e">ナビゲーション エラーの詳細</param>
    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }

    /// <summary>
    /// アプリケーションの実行が中断されたときに呼び出されます。
    /// アプリケーションが終了されるか、メモリの内容がそのままで再開されるかに
    /// かかわらず、アプリケーションの状態が保存されます。
    /// </summary>
    /// <param name="sender">中断要求の送信元。</param>
    /// <param name="e">中断要求の詳細。</param>
    private void OnSuspending(object sender, SuspendingEventArgs e)
    {
        var deferral = e.SuspendingOperation.GetDeferral();
        //TODO: アプリケーションの状態を保存してバックグラウンドの動作があれば停止します
        deferral.Complete();
    }

    // App Service

    private AppServiceConnection? _appServiceConnection;
    private BackgroundTaskDeferral? _appServiceDeferral;

    protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
    {
        base.OnBackgroundActivated(args);

        if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails appService)
        {
            _appServiceDeferral = args.TaskInstance.GetDeferral();
            args.TaskInstance.Canceled += (_, _) => _appServiceDeferral?.Complete();
            _appServiceConnection = appService.AppServiceConnection;
            _appServiceConnection.RequestReceived += AppServiceConnection_RequestReceived;
            _appServiceConnection.ServiceClosed += (_, _) => _appServiceDeferral?.Complete();
        }
    }

    public static EventHandler<List<ProcessDataModel>>? ProcessUpdated;

    private void AppServiceConnection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
    {
        var d = args.GetDeferral();

        var message = args.Request.Message;
        if (message["result"] is not string input)
            return;

        if (JsonSerializer.Deserialize<List<ProcessDataModel>>(input) is not { } processList)
            return;

        ProcessUpdated?.Invoke(this, processList);

        d.Complete();
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        // Occurs when an exception is not handled on the UI thread.
        ;
        // TODO:
        // Thread name ApplicationView ASTA
        // e.Exception.Message -> Value does not fall within the expected range.
        // https://edi.wang/post/2016/1/3/windows-10-uwp-exception
        // if you want to suppress and handle it manually, 
        // otherwise app shuts down.
        e.Handled = true;
    }

    private static void OnUnobservedException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        // Occurs when an exception is not handled on a background thread.
        // ie. A task is fired and forgotten Task.Run(() => {...})
        ;

        // suppress and handle it manually.
        e.SetObserved();
    }
}
