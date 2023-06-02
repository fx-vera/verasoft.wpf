using System.Windows;

namespace VeraSoft.Wpf.Core.Components
{
    /// <summary>
    /// Interaction logic for GenericView.xaml
    /// </summary>
    public partial class GenericView //: Window
    {
        private GenericViewModel _viewModel;
        public GenericView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            Loaded += Window_Loaded;
            Closed += GenericView_Closed;
        }

        private void GenericView_Closed(object sender, System.EventArgs e)
        {
            _viewModel.Close();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = this.DataContext as GenericViewModel;
        }
    }
}
