namespace VeraSoft.Wpf.Core
{
    using MahApps.Metro.Controls;
    using PropertyChanged;

    [AddINotifyPropertyChangedInterface]
    public abstract class FlyoutRightViewModel : FlyoutBaseViewModel
    {
        public FlyoutRightViewModel()
        {
            this.Position = Position.Right;
        }
    }

    [AddINotifyPropertyChangedInterface]
    public abstract class FlyoutLeftViewModel : FlyoutBaseViewModel
    {
        public FlyoutLeftViewModel()
        {
            this.Position = Position.Left;
        }
    }

    [AddINotifyPropertyChangedInterface]
    public abstract class FlyoutBottomViewModel : FlyoutBaseViewModel
    {
        public FlyoutBottomViewModel()
        {
            this.Position = Position.Bottom;
        }
    }

    [AddINotifyPropertyChangedInterface]
    public abstract class FlyoutTopViewModel : FlyoutBaseViewModel
    {
        public FlyoutTopViewModel()
        {
            this.Position = Position.Top;
        }
    }
}
