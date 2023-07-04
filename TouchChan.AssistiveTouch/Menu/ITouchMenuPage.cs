using System.Windows;

namespace TouchChan.AssistiveTouch.Menu
{
    interface ITouchMenuPage
    {
        event EventHandler<PageEventArgs>? PageChanged;

        void Show(double distance);

        void Close();

        Visibility Visibility { set; }
    }

    public static class PreferencePageTag
    {
        public const string General = "general";
        public const string About = "about";
        public const string MeCab = "mecab";
        public const string Danmaku = "danmaku";
        public const string Trans = "trans";
        public const string TTS = "tts";
    }

    public enum TouchMenuPageTag
    {
        None,
        Game,
        GameBack,
        Device,
        DeviceBack,
        Function,
        FunctionBack,
        WinMove
    }

    public class PageEventArgs : EventArgs
    {
        public TouchMenuPageTag Tag { get; }

        public PageEventArgs(TouchMenuPageTag tag)
        {
            Tag = tag;
        }
    }
}

