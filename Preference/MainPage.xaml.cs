#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.System.Diagnostics;
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
    public MainPage()
    {
        InitializeComponent();
        ProcessComboBox.ItemsSource = ProcessItems;
        var appView = ApplicationView.GetForCurrentView();
        var resourceLoader = ResourceLoader.GetForCurrentView();
        appView.Title = resourceLoader.GetString("ProcessSelector");
        App.ProcessUpdated += async (_, newItems) =>
        {
            var oldItems = ProcessItems.ToList().AsReadOnly();
            var disappearItems = oldItems.Where(oldItem => !newItems.Contains(oldItem)).ToList().AsReadOnly();
            var newishItems = newItems.Where(newItem => !oldItems.Contains(newItem)).ToList().AsReadOnly();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                  {
                      foreach (var item in disappearItems)
                          ProcessItems.Remove(item);

                      if (disappearItems.Count != 0 || newishItems.Count == 0)
                      {
                          foreach (var item in newishItems)
                              ProcessItems.Add(item);
                      }
                      else
                      {
                          // HACK: ComboBox UI won't refresh when there comes new items
                          var moto = ProcessItems.ToList().AsReadOnly();
                          ProcessItems.Clear();
                          await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                          {
                              foreach (var item in moto.Concat(newishItems))
                                  ProcessItems.Add(item);
                          });
                      }
                  });
        };
    }

    public ObservableCollection<ProcessDataModel> ProcessItems { get; } = new();

    private async void InjectButtonOnClick(object sender, RoutedEventArgs e)
    {
        var selected = (ProcessDataModel)ProcessComboBox.SelectedItem;

        await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppWithArgumentsAsync($"--path \"{selected.FullPath}\"");

        InjectButton.IsEnabled = false;
    }

    private async void ProcessComboBoxOnDropDownOpened(object sender, object e) => 
        await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppWithArgumentsAsync($"-channel");

    private void ProcessComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e) => 
        InjectButton.IsEnabled = ProcessComboBox.SelectedItem is not null;
}

public class ProcessDataModel
{
    public int ProcessId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Describe { get; set; } = string.Empty;    
    public string FullPath { get; set; } = string.Empty;

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        ProcessDataModel other = (ProcessDataModel)obj;
        return ProcessId == other.ProcessId;
    }

    public override int GetHashCode() => ProcessId.GetHashCode();
}

