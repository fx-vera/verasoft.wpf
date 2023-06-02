using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using VeraSoft.Wpf.Enums;
using VeraSoft.Wpf.Events;
using VeraSoft.Wpf.Managers;
using VeraSoft.Wpf.Utils;

namespace VeraSoft.Wpf.Core.Components
{
    public class GenericViewModel : IWindowViewModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericViewModel"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public GenericViewModel(IViewModel content)
        {
            _view = GetView();
            if (content != null && content.PageName != null)
                _view.Title = content.PageName;
            Initialize(content);
            _minimizeWindow = new RelayCommand((o) => Minimize());
            _maximizeWindow = new RelayCommand((o) => Maximize());
            _pinWindow = new RelayCommand((o) => SetIsPinned(!IsPinned));
            _cloneWindow = new RelayCommand((o) => Clone());
            _closeWindow = new RelayCommand((o) => Close());
            _favouriteCommand = new RelayCommand((o) => SetFavourite());
            SettingsCmd = new RelayCommand((o) => OpenPageSettings());
            OkCommand = new RelayCommand((o) => PerformOkCommand(), canExecute => CanExecuteSave());
            CancelCommand = new RelayCommand((o) => PerformCancelCommand());
            ApplyCommand = new RelayCommand((o) => PerformApplyCommand(), canExecute => CanExecuteSave());

            SaveVersionableCommand = new RelayCommand((o) => PerformSaveVersionableCommand());
            SaveAsNewVersionCommand = new RelayCommand((o) => PerformSaveAsNewVersionCommand());
            SaveAsNewElementCommand = new RelayCommand((o) => PerformSaveAsNewElementCommand());

            MaxMinText = "Maximize";

            SizeToContentMode = SizeToContent.Manual;
        }

        #endregion

        #region IWindow events

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Occurs when [window state changed].
        /// </summary>
        public event EventHandler<WindowStateEventArgs> WindowStateChanged;
        /// <summary>
        /// Occurs when [window cloned].
        /// </summary>
        public event EventHandler<WindowEventArgs> WindowCloned;

        #endregion

        #region IWindow Properties

        /// <summary>
        /// Gets or sets the content of the page.
        /// </summary>
        /// <value>
        /// The content of the page.
        /// </value>
        public IViewModel ViewModel { get; set; }

        /// <summary>
        /// The view
        /// </summary>
        private Window _view;
        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        public Window View { get { return _view; } }

        /// <summary>
        /// The state
        /// </summary>
        private PageWindowState _state = PageWindowState.Normal;
        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public PageWindowState State
        {
            get { return _state; }
            set { _state = value; NotifyStateChanged(); }
        }

        #endregion

        #region Other Properties

        // Window state
        /// <summary>
        /// Gets a value indicating whether this instance is pinned.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is pinned; otherwise, <c>false</c>.
        /// </value>
        public bool IsPinned { get; private set; }

        /// <summary>
        /// Gets the window resize mode.
        /// </summary>
        /// <value>
        /// The window resize mode.
        /// </value>
        //[DependsOn("IsPinned")]
        public ResizeMode WindowResizeMode
        {
            get
            {
                if (IsPinned)
                    return ResizeMode.NoResize;
                else if (ViewModel != null)
                    return ViewModel.ResizeMode;
                else
                    return ResizeMode.CanResizeWithGrip;
            }
        }

        /// <summary>
        /// Gets or sets the maximize/minimize text.
        /// </summary>
        /// <value>
        /// The maximum minimum text.
        /// </value>
        public string MaxMinText { get; set; }

        public SizeToContent SizeToContentMode { get; set; }


        #endregion

        #region Button Commands
        /// <summary>
        /// Gets the minimize window.
        /// </summary>
        /// <value>
        /// The minimize window.
        /// </value>
        public RelayCommand MinimizeWindow { get { return _minimizeWindow; } }
        /// <summary>
        /// Gets the maximize window.
        /// </summary>
        /// <value>
        /// The maximize window.
        /// </value>
        public RelayCommand MaximizeWindow { get { return _maximizeWindow; } }
        /// <summary>
        /// Gets the pin window.
        /// </summary>
        /// <value>
        /// The pin window.
        /// </value>
        public RelayCommand PinWindow { get { return _pinWindow; } }
        /// <summary>
        /// Gets the clone window.
        /// </summary>
        /// <value>
        /// The clone window.
        /// </value>
        public RelayCommand CloneWindow { get { return _cloneWindow; } }
        /// <summary>
        /// Gets the favourite command.
        /// </summary>
        /// <value>
        /// The favourite command.
        /// </value>
        public RelayCommand FavouriteCommand { get { return _favouriteCommand; } }
        /// <summary>
        /// Gets the close window.
        /// </summary>
        /// <value>
        /// The close window.
        /// </value>
        public RelayCommand CloseWindow { get { return _closeWindow; } }
        /// <summary>
        /// Gets or sets the settings command.
        /// </summary>
        /// <value>
        /// The settings command.
        /// </value>
        public RelayCommand SettingsCmd { get; set; }
        /// <summary>
        /// Gets or sets the ok command.
        /// </summary>
        /// <value>
        /// The ok command.
        /// </value>
        public RelayCommand OkCommand { get; set; }

        /// <summary>
        /// Gets or sets the cancel command.
        /// </summary>
        /// <value>
        /// The cancel command.
        /// </value>
        public RelayCommand CancelCommand { get; set; }

        /// <summary>
        /// Gets or sets the apply command.
        /// </summary>
        /// <value>
        /// The apply command.
        /// </value>
        public RelayCommand ApplyCommand { get; set; }

        /// <summary>
        /// Gets or sets the save command.
        /// </summary>
        /// <value>
        /// The save command.
        /// </value>
        public RelayCommand SaveVersionableCommand { get; set; }
        /// <summary>
        /// Gets or sets the save new version icon.
        /// </summary>
        /// <value>
        /// The save new version icon.
        /// </value>
        public RelayCommand SaveAsNewVersionCommand { get; set; }
        /// <summary>
        /// Gets or sets the save as new element command.
        /// </summary>
        /// <value>
        /// The save as new element command.
        /// </value>
        public RelayCommand SaveAsNewElementCommand { get; set; }

        private RelayCommand _minimizeWindow;
        private RelayCommand _maximizeWindow;
        private RelayCommand _pinWindow;
        private RelayCommand _cloneWindow;
        private RelayCommand _closeWindow;
        private RelayCommand _favouriteCommand;

        #endregion

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <returns></returns>
        protected virtual Window GetView()
        {
            if (_view == null)
                return new GenericView();
            else
                return _view;
        }

        #region IWindow functions

        /// <summary>
        /// Minimizes this instance.
        /// </summary>
        public void Minimize()
        {
            if (IsPinned || !ViewModel.CanBeMinimized)
                return;

            // Lanzamos el evento de "ventana minimizada"
            State = PageWindowState.Minimized;
        }

        /// <summary>
        /// Maximizes this instance.
        /// </summary>
        public void Maximize()
        {
            if (IsPinned || !ViewModel.CanBeMaximized)
                return;

            if (State != PageWindowState.Maximized)
            {
                MaxMinText = "Restore";
                State = PageWindowState.Maximized;
            }
            else
            {
                MaxMinText = "Maximize";
                State = PageWindowState.Normal;
            }
        }

        /// <summary>
        /// Restores this instance.
        /// </summary>
        public void Restore()
        {
            State = PageWindowState.Normal;
        }

        /// <summary>
        /// Sets the is pinned.
        /// </summary>
        /// <param name="isPinned">if set to <c>true</c> [is pinned].</param>
        public void SetIsPinned(bool isPinned)
        {
            IsPinned = isPinned;
        }

        /// <summary>
        /// Close is implemented by Window, we only capture the close call to change our WindowState to Closed
        /// </summary>
        public void Close()
        {
            ViewModel.OnClose();

            if (ViewModel != null && !ViewModel.IsDirty)
            {
                ViewModel.Dispose();
                State = PageWindowState.Closed;
            }
            else if (ViewModel != null && ViewModel.IgnoreErrors && ViewModel.IsDirty)
            {
                MessageBoxResult result = MessageBox.Show("There are changes without save. Do you want to continue?", "Close", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    State = PageWindowState.Closed;
                    ViewModel.Dispose();
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Check errors before to close.", "Close", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        public void Cancel()
        {
            if (ViewModel.IsDirty)
            {
                MessageBoxResult result = MessageBox.Show("Data not saved. Are you sure you want to cancel?", "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    State = PageWindowState.Closed;
                    ViewModel.Dispose();
                }
            }
        }

        /// <summary>
        /// Sets the enabled.
        /// </summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        public void SetEnabled(bool isEnabled)
        {
            if (ViewModel == null)
                return;

            Trace.TraceWarning("De momento no hay inhabilitación de páginas");
        }

        /// <summary>
        /// Sets the favourite.
        /// </summary>
        private void SetFavourite()
        {
            if (ViewModel != null)
                NotifyFavouriteChanged();
        }

        /// <summary>
        /// Should be implemented to opens the page settings: units, styles, etc.
        /// </summary>
        public virtual void OpenPageSettings()
        {
            ViewModel.OpenPageSettings();
        }

        /// <summary>
        /// Performs the ok command.
        /// </summary>
        public virtual void PerformOkCommand()
        {
            ViewModel.Save();
            Close();
        }

        /// <summary>
        /// Performs the cancel command.
        /// </summary>
        public virtual void PerformCancelCommand()
        {
            ViewModel.Cancel();
            Close();
        }

        /// <summary>
        /// Performs the save test (under construction) command.
        /// </summary>
        public virtual void PerformApplyCommand()
        {
            ViewModel.Save();
        }

        /// <summary>
        /// Determines whether this instance [can execute save].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can execute save]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteSave()
        {
            return ViewModel.CanExecuteSave();
        }

        /// <summary>
        /// Performs the save as new element command.
        /// </summary>
        public virtual void PerformSaveAsNewElementCommand()
        {
            ViewModel.SaveAsNewElement();
        }

        /// <summary>
        /// Performs the save as new version command.
        /// </summary>
        public virtual void PerformSaveAsNewVersionCommand()
        {
            ViewModel.SaveAsNewVersion();
        }

        /// <summary>
        /// Performs the save versionable command.
        /// </summary>
        public virtual void PerformSaveVersionableCommand()
        {
            ViewModel.SaveVersionable();
        }

        #endregion

        #region From ICloneable

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            IPageManager pm = IoC.Get<IPageManager>();
            if (pm == null)
                return null;

            return pm.Clone(ViewModel);
        }

        #endregion

        /// <summary>
        /// Notifies the state changed.
        /// </summary>
        protected void NotifyStateChanged()
        {
            if (WindowStateChanged != null)
                WindowStateChanged(this, new WindowStateEventArgs { State = State });
        }

        /// <summary>
        /// Notifies the favourite changed.
        /// </summary>
        protected void NotifyFavouriteChanged()
        {

        }

        /// <summary>
        /// Initializes the specified window content.
        /// </summary>
        /// <param name="windowContent">Content of the window.</param>
        private void Initialize(IViewModel windowContent)
        {
            ViewModel = windowContent;
            IsPinned = false;

            _view.DataContext = this;
            _view.SetCurrentValue(Window.SizeToContentProperty, ViewModel.SizeToContentMode);
            _view.SetCurrentValue(FrameworkElement.WidthProperty, ViewModel.InitialSize.Width);
            _view.SetCurrentValue(FrameworkElement.HeightProperty, ViewModel.InitialSize.Height);

            _view.SetCurrentValue(Window.TopProperty, ViewModel.InitialTop);
            _view.SetCurrentValue(Window.LeftProperty, ViewModel.InitialLeft);

            if (ViewModel.InitialTop.Equals(double.NaN) && ViewModel.InitialLeft.Equals(double.NaN))
            {
                // si es la primera vez que se abre y no tiene editorSettings, se coloca en el centro
                _view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }
    }
}
