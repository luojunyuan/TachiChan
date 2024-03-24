#nullable enable
using IniParser;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace Preference
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            KeyComboBox.SelectedIndex = 0;

            Loaded += async (_, _) =>
            {
                StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
                IStorageItem item = await roamingFolder.TryGetItemAsync("Config.ini");

                if (item is not StorageFile iniFile)
                    return;

                string fileContent = await FileIO.ReadTextAsync(iniFile);

                var parser = new FileIniDataParser();
                var data = parser.Parser.Parse(fileContent);

                string value = data["TouchChan"]["ScreenShotTradition"] ?? "false";
                string value2 = data["TouchChan"]["UseEnterKeyMapping"] ?? "false";
                string value3 = data["TouchChan"]["MappingKey"] ?? "Z";
                string value4 = data["TouchChan"]["UseModernSleep"] ?? "false";
                string value5 = data["TouchChan"]["EnforceOldTouchStyle"] ?? "false";
                string value6 = data["TouchChan"]["UseEdgeTouchMask"] ?? "false";

                Screenshot.IsChecked = bool.Parse(value);
                ModernSleep.IsChecked = bool.Parse(value4);
                EnterKeyMapping.IsChecked = bool.Parse(value2);
                KeyComboBox.SelectedIndex = value3 == "Z" ? 0 : 1;
                KeyComboBox.SelectionChanged += KeyComboBoxOnSelectionChanged;
                OutterWindow.IsChecked = bool.Parse(value5);
                AntiTouch.IsChecked = bool.Parse(value6);
            };
        }

        private async static Task WritePropertyToIniFile(string section, string value)
        {
            StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
            var iniFile = await roamingFolder.CreateFileAsync("Config.ini", CreationCollisionOption.OpenIfExists);

            string fileContent = await FileIO.ReadTextAsync(iniFile);

            var parser = new FileIniDataParser();
            var data = parser.Parser.Parse(fileContent);

            data["TouchChan"][section] = value;
            string newFileContent = data.ToString();
            await FileIO.WriteTextAsync(iniFile, newFileContent);
        }

        private async void Screenshot_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) =>
            await WritePropertyToIniFile("ScreenShotTradition", Screenshot.IsChecked.ToString());

        private async void EnterKeyMapping_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) =>
            await WritePropertyToIniFile("UseEnterKeyMapping", EnterKeyMapping.IsChecked.ToString());

        private async void ModernSleep_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) =>
            await WritePropertyToIniFile("UseModernSleep", ModernSleep.IsChecked.ToString());

        private async void OutterWindow_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) =>
            await WritePropertyToIniFile("EnforceOldTouchStyle", OutterWindow.IsChecked.ToString());

        private async void AntiTouch_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) =>
            await WritePropertyToIniFile("UseEdgeTouchMask", AntiTouch.IsChecked.ToString());

        private async void KeyComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
            await WritePropertyToIniFile("MappingKey", ((ComboBoxItem)KeyComboBox.SelectedItem).Content.ToString());
    }
}
