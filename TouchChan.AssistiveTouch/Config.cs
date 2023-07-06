using TouchChan.AssistiveTouch.NativeMethods;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using WindowsInput.Events;

namespace TouchChan.AssistiveTouch
{
    internal static class Config
    {
        private static readonly string RoamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string ConfigFolder = Path.Combine(RoamingPath, "TouchChan");
        private static readonly string ConfigFilePath = Path.Combine(RoamingPath, "TouchChan", "EHConfig.ini");

        public static bool UseEnterKeyMapping { get; private set; }

        public static KeyCode MappingKey { get; private set; }

        public static bool ScreenShotTradition { get; private set; }

        public static string AssistiveTouchPosition { get; private set; } = string.Empty;

        public static bool UseEdgeTouchMask { get; private set; }

        public static bool EnableMagTouchMapping { get; private set; }

        public static void Load()
        {
            // First time start
            if (!File.Exists(ConfigFilePath))
                return;

            var myIni = new IniFile(ConfigFilePath);
            UseEnterKeyMapping = bool.Parse(myIni.Read(nameof(UseEnterKeyMapping)) ?? "false");
            MappingKey = (KeyCode)Enum.Parse(typeof(KeyCode), myIni.Read(nameof(MappingKey)) ?? "Z"); // const int KEY_Z = 0x5A;
            ScreenShotTradition = bool.Parse(myIni.Read(nameof(ScreenShotTradition)) ?? "false");
            AssistiveTouchPosition = myIni.Read(nameof(AssistiveTouchPosition)) ?? string.Empty;
            // Touch size
            UseEdgeTouchMask = bool.Parse(myIni.Read(nameof(UseEdgeTouchMask)) ?? "false");
            EnableMagTouchMapping = bool.Parse(myIni.Read(nameof(EnableMagTouchMapping)) ?? "false");
        }

        public static void SaveAssistiveTouchPosition(string pos)
        {
            // First time create folder and ini file
            if (!Directory.Exists(ConfigFolder))
                Directory.CreateDirectory(ConfigFolder);

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
