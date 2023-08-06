#nullable enable
using IniParser;
using System;
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
                //string value = data["TouchChan"]["MappingKey"];
                //string value = data["TouchChan"]["ModernSleep"]; 

                Screenshot.IsChecked = bool.Parse(value);
                EnterKeyMapping.IsChecked = bool.Parse(value2);
            };
        }

        private async void Screenshot_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
            var iniFile = await roamingFolder.CreateFileAsync("Config.ini", CreationCollisionOption.OpenIfExists);

            string fileContent = await FileIO.ReadTextAsync(iniFile);

            var parser = new FileIniDataParser();
            var data = parser.Parser.Parse(fileContent);

            data["TouchChan"]["ScreenShotTradition"] = Screenshot.IsChecked.ToString();
            string newFileContent = data.ToString();
            await FileIO.WriteTextAsync(iniFile, newFileContent);
        }

        private async void EnterKeyMapping_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
            var iniFile = await roamingFolder.CreateFileAsync("Config.ini", CreationCollisionOption.OpenIfExists);

            string fileContent = await FileIO.ReadTextAsync(iniFile);

            var parser = new FileIniDataParser();
            var data = parser.Parser.Parse(fileContent);

            data["TouchChan"]["UseEnterKeyMapping"] = EnterKeyMapping.IsChecked.ToString();
            string newFileContent = data.ToString();
            await FileIO.WriteTextAsync(iniFile, newFileContent);
        }
    }
}
