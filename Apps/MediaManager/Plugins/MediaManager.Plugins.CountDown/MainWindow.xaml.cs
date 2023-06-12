using System.Windows;
using System.Windows.Controls;

namespace MediaManager.Plugins.CountDown
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Button1.Content?.ToString());
        }
    }
}
