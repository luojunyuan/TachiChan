using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ErogeHelper.AssistiveTouch.Helper
{
    public partial class ToggleTouchMenuItem : UserControl
    {
        #region Symbol DependencyProperty
        /// <summary>Identifies the <see cref="Symbol"/> dependency property.</summary>
        public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(
            nameof(Symbol),
            typeof(Symbol),
            typeof(ToggleTouchMenuItem),
            new PropertyMetadata(Symbol.Emoji, (d, e) => ((ToggleTouchMenuItem)d).OnSymbolChanged(e)));

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
            typeof(ToggleTouchMenuItem),
            new PropertyMetadata(string.Empty, (d, e) => ((ToggleTouchMenuItem)d).OnTextChanged(e)));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        protected void OnTextChanged(DependencyPropertyChangedEventArgs e) =>
            ItemText.SetCurrentValue(TextBlock.TextProperty, (string)e.NewValue);
        #endregion

        #region IsOn DependencyProperty
        /// <summary>Identifies the <see cref="IsOn"/> dependency property.</summary>
        public static readonly DependencyProperty IsOnProperty = DependencyProperty.Register(
            nameof(IsOn),
            typeof(bool),
            typeof(ToggleTouchMenuItem),
            new PropertyMetadata(false, (d, e) => ((ToggleTouchMenuItem)d).OnIsOnChanged(e)));

        public bool IsOn
        {
            get => (bool)GetValue(IsOnProperty);
            set => SetValue(IsOnProperty, value);
        }

        protected void OnIsOnChanged(DependencyPropertyChangedEventArgs e)
        {
            SetItemForegroundColor((bool)e.NewValue ? ItemPressedColor : Brushes.White);
            Toggled?.Invoke(this, new());
        }
        #endregion

        public event EventHandler? Toggled;

        public ToggleTouchMenuItem()
        {
            InitializeComponent();
        }

        private static readonly Brush ItemPressedColor = new SolidColorBrush(Color.FromArgb(255, 111, 196, 241));

        private bool _buttonPressed;

        private void ItemOnPreviewMouseLeftButtonDown(object sender, InputEventArgs e)
        {
            if (!TouchMenuItem.ClickLocked)
            {
                _buttonPressed = true;
                SetItemForegroundColor(IsOn ? Brushes.DeepSkyBlue : Brushes.Gray);
            }
        }

        private void ItemOnPreviewMouseLeave(object sender, InputEventArgs e)
        {
            _buttonPressed = false;
            SetItemForegroundColor(IsOn ? ItemPressedColor : Brushes.White);
        }

        private void ItemOnPreviewMouseLeftButtonUp(object sender, InputEventArgs e)
        {
            if (_buttonPressed)
            {
                SetCurrentValue(IsOnProperty, !IsOn);
                _buttonPressed = false;
            }
        }

        private void ItemOnTouchUp(object sender, TouchEventArgs e)
        {
            if (_buttonPressed)
            {
                SetCurrentValue(IsOnProperty, !IsOn);
                _buttonPressed = false;
            }
        }

        private void SetItemForegroundColor(Brush color) => ItemIcon.Foreground = ItemText.Foreground = color;
    }
}
