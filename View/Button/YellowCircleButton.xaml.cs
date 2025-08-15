// ------------------------------------------------------------
// File: YellowCircleButton.xaml.cs
// Purpose: Defines a custom yellow circular button for the calculator UI.
// ------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;

namespace Calculator.View.Button
{
    /// <summary>
    /// Interaction logic for YellowCircleButton.
    /// Custom UserControl representing a yellow circular button with text or image.
    /// </summary>
    public partial class YellowCircleButton : UserControl
    {
        /// <summary>
        /// Initializes the YellowCircleButton control.
        /// </summary>
        public YellowCircleButton()
        {
            InitializeComponent();
        }

        /// <summary>
        /// DependencyProperty for the button's displayed content (text or image).
        /// Allows binding in XAML.
        /// </summary>
        public static readonly DependencyProperty ButtonContentProperty =
            DependencyProperty.Register(nameof(ButtonContent), typeof(string), typeof(YellowCircleButton), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the content displayed on the button.
        /// </summary>
        public string ButtonContent
        {
            get { return (string)GetValue(ButtonContentProperty); }
            set { SetValue(ButtonContentProperty, value); }
        }

        /// <summary>
        /// Event raised when the button is clicked.
        /// Other controls can subscribe to this event.
        /// </summary>
        public event RoutedEventHandler? Click;

        /// <summary>
        /// Handles the internal button's Click event and raises the custom Click event.
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }
    }
}
