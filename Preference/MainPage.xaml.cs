#nullable enable
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

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
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    ProcessItems.Clear();
            //    foreach (var item in newItems)
            //        ProcessItems.Add(item);
            //});
            //return;
            // めちゃくちゃ
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
                        ProcessItems.SafeAdd(item);
                }
                else
                {
                    // HACK: ComboBox UI won't refresh when there comes new items
                    var moto = ProcessItems.ToList().AsReadOnly();
                    ProcessItems.Clear();
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        foreach (var item in moto.Concat(newishItems))
                            ProcessItems.SafeAdd(item);
                    });
                }
                // the real end
            });
            ServiceCount--;
        };
    }


    public ObservableCollection<ProcessDataModel> ProcessItems { get; } = new();

    private async void InjectButtonOnClick(object sender, RoutedEventArgs e)
    {
        var selected = (ProcessDataModel)ProcessComboBox.SelectedItem;

        await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppWithArgumentsAsync($"--path \"{selected.FullPath}\"");

        InjectButton.IsEnabled = false;
    }

    private static int ServiceCount = 0;
    private async void ProcessComboBoxOnDropDownOpened(object sender, object e)
    {
        if (ServiceCount > 3)
            return;
        await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppWithArgumentsAsync($"-channel");
        ServiceCount++;
    }

    private void ProcessComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
        InjectButton.IsEnabled = ProcessComboBox.SelectedItem is not null;
}

public static class Ext
{
    // As possible as atmosphere operation
    public static void SafeAdd(this ObservableCollection<ProcessDataModel> group, ProcessDataModel item)
    {
        if (!group.Contains(item))
            group.Add(item);
    }
}

public class ProcessDataModel
{
    public int ProcessId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Describe { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;

    public byte[] IconBytes
    {
        set => Icon = ImageFromBytes(value).Result;
    }

    [JsonIgnore]
    public BitmapImage? Icon { get; set; }

    public static Task<BitmapImage> ImageFromBytes(byte[] bytes)
    {
        var tcs = new TaskCompletionSource<BitmapImage>();

        _ = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
            var image = new BitmapImage();
            using var stream = new InMemoryRandomAccessStream();
            var dataWriter = new DataWriter(stream.GetOutputStreamAt(0));
            dataWriter.WriteBytes(bytes);
            dataWriter.StoreAsync().Completed += async (sender, e) =>
            {
                dataWriter.DetachStream();
                stream.Seek(0);
                await image.SetSourceAsync(stream);
                tcs.SetResult(image);
            };
        });

        return tcs.Task;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        ProcessDataModel other = (ProcessDataModel)obj;
        return ProcessId == other.ProcessId;
    }

    public override int GetHashCode() => ProcessId.GetHashCode();
}

