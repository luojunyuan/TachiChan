using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TouchChan.AssistiveTouch.NativeMethods;

namespace TouchChan.AssistiveTouch
{
    internal static class Config
    {
        private static string ConfigFilePath = string.Empty;

        public static bool UseEnterKeyMapping { get; private set; }

        public static Simulate.KeyCode MappingKey { get; private set; }

        public static bool ScreenShotTradition { get; private set; }

        public static string AssistiveTouchPosition { get; private set; } = string.Empty;

        public static bool UseEdgeTouchMask { get; private set; }

        public static bool EnableMagTouchMapping { get; private set; }

        public static bool UseModernSleep { get; private set; }

        public static bool DisableTouch { get; private set; }

        public static void Load()
        {
#if !NET472
            Windows.Storage.StorageFolder roamingFolder = Windows.Storage.ApplicationData.Current.RoamingFolder;
            var item = roamingFolder.TryGetItemAsync("Config.ini").GetResults();// recommend not await for winrt async function
            ConfigFilePath = item?.Path ?? string.Empty;
#else
            string RoamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string ConfigFolder = Path.Combine(RoamingPath, "TouchChan");
            ConfigFilePath = Path.Combine(RoamingPath, "TouchChan", "Config.ini");
#endif
            // First time start
            if (!File.Exists(ConfigFilePath))
                return;

            var myIni = new IniFile(ConfigFilePath);
            UseEnterKeyMapping = bool.Parse(myIni.Read(nameof(UseEnterKeyMapping)) ?? "false");
#if !NET472
            MappingKey = Enum.Parse<Simulate.KeyCode>(myIni.Read(nameof(MappingKey)) ?? "Z");
#else
            MappingKey = (Simulate.KeyCode)Enum.Parse(typeof(Simulate.KeyCode), myIni.Read(nameof(MappingKey)) ?? "Z");
#endif
            ScreenShotTradition = bool.Parse(myIni.Read(nameof(ScreenShotTradition)) ?? "false");
            AssistiveTouchPosition = myIni.Read(nameof(AssistiveTouchPosition)) ?? string.Empty;
            UseEdgeTouchMask = bool.Parse(myIni.Read(nameof(UseEdgeTouchMask)) ?? "false");
            EnableMagTouchMapping = bool.Parse(myIni.Read(nameof(EnableMagTouchMapping)) ?? "false");
            UseModernSleep = bool.Parse(myIni.Read(nameof(EnableMagTouchMapping)) ?? "false");
            DisableTouch = bool.Parse(myIni.Read(nameof(DisableTouch)) ?? "false");
        }

        public static void SaveAssistiveTouchPosition(string pos)
        {
            // UWP config.ini has not initialize yet;
            if (ConfigFilePath == string.Empty)
                return;

            // First time create folder and ini file
            if (!File.Exists(ConfigFilePath))
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath)!);

            var myIni = new IniFile(ConfigFilePath);
            myIni.Write(nameof(AssistiveTouchPosition), pos);
        }


        public static T? XmlDeserializer<T>(string text)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(text);

            var result = (T?)serializer.Deserialize(reader);

            return result;
        }

        public static string XmlSerializer<T>(T inst)
        {
            var serializer = new XmlSerializer(typeof(T));
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using var writer = new StringWriter();
            using var xmlWriter = XmlWriter.Create(writer, settings);
            serializer.Serialize(xmlWriter, inst, ns);

            return writer.ToString();
        }

        private class IniFile
        {
            private const string Section = "TouchChan";
            private readonly string IniPath;

            public IniFile(string iniPath)
            {
                IniPath = iniPath;
            }

            public string? Read(string key)
            {
                var retVal = new StringBuilder(255);
                Kernel32.GetPrivateProfileString(Section, key, string.Empty, retVal, 255, IniPath);
                return retVal.ToString() == string.Empty ? null : retVal.ToString();
            }

            public void Write(string key, string value) => Kernel32.WritePrivateProfileString(Section, key, value, IniPath);
        }
    }
}
