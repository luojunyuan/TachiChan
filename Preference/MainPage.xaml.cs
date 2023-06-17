using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace Preference;

/// <summary>
/// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
/// </summary>
public sealed partial class MainPage : Page
{
    public static MainPage Current { get; private set; }

    public MainPage()
    {
        InitializeComponent();
        Current = this;
    }

    public async Task SetTextAsync(string text)
    {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                 Tips.Text = text;
            });
    }

    private readonly ObservableCollection<ProcessDataModel> _processes = new();

    private void InjectButtonOnClick(object sender, RoutedEventArgs e)
    {
         _ = FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppWithArgumentsAsync($"-channel");
    }

    private async void ProcessComboBoxOnDropDownOpened(object sender, object e)
    {
        _processes.Add(new ProcessDataModel()
        {
            Title = "aaa",
            Describe = "sdf"
        });
        //await Task.Run(RefreshProcesses).ConfigureAwait(false);
    }

    private void ProcessComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        InjectButton.IsEnabled = true;
    }


    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
    }
}

public class ProcessDataModel
{
    public string Title { get; set; }
    public string Describe { get; set; }
}