namespace VeraSoft.Wpf.Core
{
    using MahApps.Metro.Controls;
    using PropertyChanged;
    using System.Windows.Media;

    [AddINotifyPropertyChangedInterface]
    public abstract class FlyoutBaseViewModel : ViewModel
    {
        public string Header { get; set; }
        public bool IsOpen { get; set; }

        public Position Position { get; set; }

        public FlyoutTheme Theme { get; set; }
        public SolidColorBrush SolidColorBrush { get; set; }
    }
}
