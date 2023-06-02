using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using VeraSoft.Wpf.Core;
using VeraSoft.Wpf.Core.Components;
using VeraSoft.Wpf.Enums;
using VeraSoft.Wpf.Events;
using VeraSoft.Wpf.Extensions;
using VeraSoft.Wpf.Managers;
using VeraSoft.Wpf.Utils;

namespace VeraSoft.Wpf.Mainframe
{
    [Export(typeof(IMainWindowViewModel))]
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class MainWindowContentViewModel : IMainWindowViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<WindowStateEventArgs> WindowStateChanged;
        private readonly IPageManager pm;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowContentViewModel"/> class.
        /// </summary>
        /// <param name="pm">The pm.</param>
        [ImportingConstructor]
        public MainWindowContentViewModel(IPageManager pm)
        {
            Title = "Media Suite";
            Icon = null;
            WindowStyle = WindowStyle.None;
            IsResizable = true;
            CurrentUserName = Environment.UserName;

            Plugins = new ObservableCollection<PluginItemBase>();
            this.pm = pm;
            EnableSnapshotsPanel = true;
            IsRightIconsBarVisible = true;
            IsMainTitleVisible = true;
            ShowMinimize = true;
            ShowClose = true;
            ShowMaximize = true;

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var showProfile = config.AppSettings.Settings["ShowProfile"];
            if (showProfile != null && bool.TryParse(showProfile.Value, out bool showProfileResult))
                ShowProfile = showProfileResult;

            State = PageWindowState.Normal;

            MinimizeCmd = new RelayCommand((o) => OnMinimizeClicked());
            ProfileCmd = new RelayCommand((o) => OnProfileClicked());
            CloseCmd = new RelayCommand((o) => OnCloseClicked());
            MaximizeCmd = new RelayCommand((o) => OnMaximizeClicked());
            MenuCmd = new RelayCommand((o) => OnMenuClicked());

            RegisterAssociatedView();

            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
            eventAggregator.Register<FavouriteEventArgs>(this, SetFavourites);
            Id = "{80A3B433-EEEC-4362-AA32-44598EEF4DFD}";
            InitialLeft = SystemParameters.PrimaryScreenWidth / 4;
            InitialTop = new Point(0, 0).Y;
            FlyoutViewModels = new ObservableCollection<FlyoutBaseViewModel>();
        }

        public string Title { get; set; }
        public string ProfileManagement { get; set; }
        public ImageSource Icon { get; set; }

        public Size InitialSize { get; set; }

        public bool EnableSnapshotsPanel { get; set; }

        public bool IsRightIconsBarVisible { get; set; }

        public string CurrentUserName { get; set; }

        public WindowStyle WindowStyle { get; set; }
        public PageWindowState State { get; set; }

        public bool IsResizable { get; set; }

        public bool ShowMaximize { get; protected set; }
        public bool ShowMinimize { get; protected set; }
        public bool ShowClose { get; protected set; }
        public bool ShowProfile { get; } //protected set; }
        public bool ReloadMenusTrick { get; set; }
        public ICommand MaximizeCmd { get; private set; }
        public ICommand MinimizeCmd { get; private set; }
        public ICommand CloseCmd { get; private set; }
        public ICommand ProfileCmd { get; private set; }
        public ICommand MenuCmd { get; private set; }
        public ICommand MinimizeAllCmd { get; private set; }

        public ObservableCollection<PluginItemBase> Plugins { get; set; }

        public INotifyPropertyChanged MainContent { get; protected set; }

        public bool IsCollapsed { get; set; }

        public bool IsMainTitleVisible { get; set; }

        public bool IsMenuVerticalOpened { get; set; }

        public object EditorSettings { get; set; }
        public string Id { get; set; }
        public double InitialTop { get; set; }
        public double InitialLeft { get; set; }
        public IViewModel ViewModel { get; set; }
        public ObservableCollection<FlyoutBaseViewModel> FlyoutViewModels { get; set; }

        protected virtual void OnMaximizeClicked()
        {
            if (State == PageWindowState.Maximized)
                State = PageWindowState.Normal;
            else
                State = PageWindowState.Maximized;
        }

        protected virtual void OnMinimizeClicked()
        {
            if (State == PageWindowState.Minimized)
                State = PageWindowState.Normal;
            else
                State = PageWindowState.Minimized;
        }

        protected virtual void OnCloseClicked()
        {
            if (MessageBox.Show($"Are you sure you want to close the application", "Close Application",
                  MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                State = PageWindowState.Closed;
            }
        }

        private void OnMenuClicked()
        {
            if (IsCollapsed)
                IsMenuVerticalOpened = !IsMenuVerticalOpened;
        }

        /// <summary>
        /// Called when [profile clicked].
        /// </summary>
        protected virtual void OnProfileClicked()
        {
            var flyout = this.FlyoutViewModels[0];
            flyout.IsOpen = !flyout.IsOpen;
            //flyout.Theme = FlyoutTheme.Accent;
        }

        /// <summary>
        /// Registers upon creation the type of the view associated to this view model.
        /// We do this instead of creating a ViewMapping to allow subclasses to change the view
        /// </summary>
        protected virtual void RegisterAssociatedView()
        {
            IoC.Get<ViewsManager>().RegisterView(typeof(MainWindowContentViewModel), typeof(MainWindow));
        }

        public virtual void OnMainWindowClosing()
        {
            //InitialSize.Height = 1;
            pm.CloseAllWindows();
        }

        public virtual void OnMainWindowClosed()
        {
        }

        protected void OnStateChanged()
        {
            OnStateChanged(State);
        }

        public virtual void OnStateChanged(PageWindowState newState)
        {
            if (WindowStateChanged != null)
                WindowStateChanged(this, new WindowStateEventArgs { State = newState });
        }

        /// <summary>
        /// Minimizes all pages.
        /// </summary>
        public void MinimizeAllPages()
        {
            // Method intentionally left empty.
        }

        private void SetFavourites(FavouriteEventArgs args)
        {
            if (string.IsNullOrEmpty(args.Item.Id))
            {
                return;
            }

            var plugin = Plugins.FirstOrDefault(x => x.Id.Equals(args.Item.Id, StringComparison.InvariantCultureIgnoreCase));
            if (plugin != null && args.IsCommand)
            {
                Plugins.Remove(plugin);
            }
            else if (plugin == null)
            {
                if (Plugins.Count > 19)
                {
                    return;
                }
                plugin = new PluginItemBase(args.Item.Id, args.Item.Name, args.Item.Icon, args.Item.Command);
                Plugins.Add(plugin);
            }
        }

        public void LoadFlyouts()
        {
            var page = (FlyoutBaseViewModel)IoC.Get<IViewModel>(DefaultPages.MainframeProfile);
            if (page != null && page.Id.Equals(DefaultId.UserProfile, StringComparison.InvariantCultureIgnoreCase))
            {
                page.Position = Position.Right;
                page.Theme = FlyoutTheme.Accent;
                FlyoutViewModels.Add(page);
            }
        }

        public void SetSelectedPlugin()
        {
            Plugins.FirstOrDefault()?.Command.Execute(null);
        }
    }
}
