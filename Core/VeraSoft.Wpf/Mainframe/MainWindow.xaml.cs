using System.Windows;
using System.Windows.Media;

namespace VeraSoft.Wpf.Mainframe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow //: MahApps.Metro.Controls.MetroWindow//: Window
    {
        IMainWindowViewModel _viewModel;

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        public IMainWindowViewModel ViewModel { get { return _viewModel; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public MainWindow(IMainWindowViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;

            //this.ResizeMode = _viewModel.IsResizable ? ResizeMode.CanResizeWithGrip : ResizeMode.NoResize;
            this.ResizeMode = _viewModel.IsResizable ? ResizeMode.CanResizeWithGrip : ResizeMode.NoResize;

            if (_viewModel.InitialSize.Width <= 0 && _viewModel.InitialSize.Height <= 0)
                this.SizeToContent = SizeToContent.WidthAndHeight;
            else if (_viewModel.InitialSize.Width <= 0)
                this.SizeToContent = SizeToContent.Width;
            else if (_viewModel.InitialSize.Height <= 0)
                this.SizeToContent = SizeToContent.Height;

            this.DataContext = _viewModel;

            this.Closing += (o, e) => _viewModel.OnMainWindowClosing();
            this.Closed += (o, e) => _viewModel.OnMainWindowClosed();
            this.GotKeyboardFocus += this.MainWindow_GotKeyboardFocus;
            this.LostKeyboardFocus += this.MainWindow_LostKeyboardFocus;
            //FlyoutViewModels.Items.AddRange(_viewModel.FlyoutViewModels);
            _viewModel.FlyoutViewModels.ForEach(x => FlyoutViewModels.Items.Add(x));
        }

        private void MainWindow_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            this.BorderBrush = Brushes.Gray;
        }

        private void MainWindow_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            this.BorderBrush = Brushes.Orange;
        }
    }
}
