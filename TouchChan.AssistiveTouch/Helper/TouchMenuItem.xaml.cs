using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TouchChan.AssistiveTouch.Helper
{
    public partial class TouchMenuItem : UserControl
    {
        #region Symbol DependencyProperty
        /// <summary>Identifies the <see cref="Symbol"/> dependency property.</summary>
        public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(
            nameof(Symbol),
            typeof(Symbol),
            typeof(TouchMenuItem),
            new PropertyMetadata(Symbol.Emoji, (d, e) => ((TouchMenuItem)d).OnSymbolChanged(e)));

        /// <summary>
        /// Gets or sets the Segoe MDL2 Assets glyph used as the icon content.
        /// </summary>
        public Symbol Symbol
        {
            get => (Symbol)GetValue(SymbolProperty);
            set => SetValue(SymbolProperty, value);
        }

        protected void OnSymbolChanged(DependencyPropertyChangedEventArgs e) =>
            ItemIcon.SetCurrentValue(TextBlock.TextProperty, char.ConvertFromUtf32((int)e.NewValue));
        #endregion

        #region Text DependencyProperty
        /// <summary>Identifies the <see cref="Text"/> dependency property.</summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(TouchMenuItem),
            new PropertyMetadata(string.Empty, (d, e) => ((TouchMenuItem)d).OnTextChanged(e)));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        protected void OnTextChanged(DependencyPropertyChangedEventArgs e) =>
            ItemText.SetCurrentValue(TextBlock.TextProperty, (string)e.NewValue);
        #endregion

        public event EventHandler? Click;

        public static bool ClickLocked { get; set; }

        public TouchMenuItem()
        {
            InitializeComponent();
        }

        public bool IsEnabledEx
        {
            get => IsEnabled;
            set
            {
                IsEnabled = value;
                SetItemForegroundColor(value ? Brushes.White : Brushes.Gray);
            }
        }

        private static readonly Brush ItemPressedColor = new SolidColorBrush(Color.FromArgb(255, 111, 196, 241));

        private void ItemOnPreviewMouseLeftButtonDown(object? sender, InputEventArgs e)
        {
            if (!ClickLocked) SetItemForegroundColor(ItemPressedColor);
        }

        private void ItemOnPreviewMouseLeave(object? sender, InputEventArgs e)
        {
            if (IsEnabled)
            {
                SetItemForegroundColor(Brushes.White);
            }
        }

        private void ItemOnPreviewMouseLeftButtonUp(object? sender, InputEventArgs e)
        {
            if (IsEnabled && ItemIcon.Foreground != Brushes.White && !ClickLocked)
            {
                SetItemForegroundColor(Brushes.White);

                Click?.Invoke(this, e);
            }
        }

        private void ItemOnTouchUp(object? sender, TouchEventArgs e)
        {
            if (IsEnabled && !ClickLocked) Click?.Invoke(this, e);
        }

        private void SetItemForegroundColor(Brush color) => ItemIcon.Foreground = ItemText.Foreground = color;
    }
}
