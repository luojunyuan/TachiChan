﻿#nullable enable
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        };
    }


    public ObservableCollection<ProcessDataModel> ProcessItems { get; } = new();

    private async void InjectButtonOnClick(object sender, RoutedEventArgs e)
    {
        var selected = (ProcessDataModel)ProcessComboBox.SelectedItem;

        await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppWithArgumentsAsync($"--path \"{selected.FullPath}\"");

        selected.Injected = false;
        InjectButton.IsEnabled = false;
    }

    private async void ProcessComboBoxOnDropDownOpened(object sender, object e) =>
        await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppWithArgumentsAsync($"--channel");

    private void ProcessComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
        InjectButton.IsEnabled = ProcessComboBox.SelectedItem is not null && !((ProcessDataModel)ProcessComboBox.SelectedItem).Injected;
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

    public BitmapImage? Icon { get; set; }

    public bool Injected { get; set; }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        ProcessDataModel other = (ProcessDataModel)obj;
        return ProcessId == other.ProcessId;
    }

    public override int GetHashCode() => ProcessId.GetHashCode();

    // Move the setter for IconBytes to the deserialization model
    public static async Task<BitmapImage?> ImageFromBytesAsync(byte[] bytes)
    {
        if (bytes.Length == 0)
            return null;

        BitmapImage? image = null;

        using var stream = new InMemoryRandomAccessStream();
        await stream.WriteAsync(bytes.AsBuffer());
        stream.Seek(0);
        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
                image = new BitmapImage();
                await image.SetSourceAsync(stream);
            });

        return image;
    }
}

public class ProcessDataModelDeserialization
{
    public int ProcessId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Describe { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;

    public byte[] IconBytes { get; set; } = Array.Empty<byte>();

    public async Task<ProcessDataModel> ToProcessDataModelAsync() =>
        new ProcessDataModel
        {
            ProcessId = ProcessId,
            Title = Title,
            Describe = Describe,
            FullPath = FullPath,
            Icon = await ProcessDataModel.ImageFromBytesAsync(IconBytes)
        };
}
