using PropertyChanged;
using System.Windows;
using VeraSoft.Wpf.Managers;
using VeraSoft.Wpf.Validation;

namespace VeraSoft.Wpf.Core
{
    /// <summary>
    /// ViewModel
    /// </summary>
    [AddINotifyPropertyChangedInterface]    
    public abstract class ViewModel : ValidatableBase, IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
            CanBeCloned = false;
            CanBeClosed = true;
            CanBeMaximized = true;
            CanBeMinimized = true;
            CanBePinned = true;
            IsTransparent = true;
            ResizeMode = ResizeMode.CanResizeWithGrip;
            KeepAspectRatio = false;
            IsToolWindow = false;
            CanHaveMultipleInstances = false;
            MaxNumInstances = -1;
            CanBeFavourite = true;
            InitialSize = new Size(640, 480);
            SizeToContentMode = SizeToContent.Manual;
            HasSettingsPage = false;
            IsScalablePage = false;
            IsDirty = false;
            IgnoreErrors = false;

            StatusMessage = string.Empty;
            InitialTop = double.NaN;
            InitialLeft = double.NaN;

            AllowLayout = true;
            Workspace = IoC.Get<IWorkspace>();
        }

        /// <summary>
        /// Loads the editor edition mode.
        /// </summary>
        public virtual void LoadEditorEditionMode()
        {
        }

        #region IIcaroPage Properties

        /// <summary>
        /// Name that will uniquely identify the type of this page
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// Indicates if the page can be cloned. Default: false.
        /// If changed to true, remember to provide an implementation 
        /// of the Clone method
        /// </summary>
        public bool CanBeCloned { get; protected set; }

        /// <summary>
        /// Indicates if the page can be closed. Default: true
        /// </summary>
        public bool CanBeClosed { get; protected set; }

        /// <summary>
        /// Indicates if the page can be maximized. Default: true
        /// </summary>
        public bool CanBeMaximized { get; protected set; }

        /// <summary>
        /// Indicates if the page can be minimized. Default: true
        /// </summary>
        public bool CanBeMinimized { get; protected set; }

        /// <summary>
        /// Indicates if the page position and size can be "locked" ("pinned"), 
        /// to prevent it from being moved or maximized. Default: true
        /// </summary>
        public bool CanBePinned { get; protected set; }

        /// <summary>
        /// Indicates if the page can be made transparent (change its 
        /// alpha value so that other content below is partly visible). Default: true
        /// </summary>
        public bool IsTransparent { get; set; }

        /// <summary>
        /// Indicates if the page can be opened several times or only one instance is allowed.
        /// This is different from cloning: the clone of a page is opened with the exact same content
        /// as the original. Different instances, on the contrary, can be initially opened with different
        /// contents.
        /// Default: true
        /// </summary>
        public bool CanHaveMultipleInstances { get; protected set; }

        /// <summary>
        /// In case that a page can have multiple instances, maximum number of instances that
        /// are allowed. -1 indicates there is no restriction.
        /// Default: -1
        /// </summary>
        public int MaxNumInstances { get; protected set; }

        /// <summary>
        /// Indicates if the window can be resized, and how. Default: CanResizeWithGrip
        /// </summary>
        public ResizeMode ResizeMode { get; set; }

        /// <summary>
        /// Desired initial size of the window
        /// </summary>
        public Size InitialSize { get; set; }

        /// <summary>
        /// Gets or sets the size to content mode.
        /// </summary>
        /// <value>
        /// The size to content mode.
        /// </value>
        public SizeToContent SizeToContentMode { get; set; }

        /// <summary>
        /// Gets or sets the initial left.
        /// </summary>
        /// <value>
        /// The initial left.
        /// </value>
        public double InitialLeft { get; protected set; }

        /// <summary>
        /// Gets or sets the initial top.
        /// </summary>
        /// <value>
        /// The initial top.
        /// </value>
        public double InitialTop { get; protected set; }

        /// <summary>
        /// Indicates if when resizing the page, it should always maintain its aspect ratio. Default: false.
        /// </summary>
        public bool KeepAspectRatio { get; set; }

        /// <summary>
        /// Indicates if this page should have a Tool Window-like appearance. Default: false.
        /// </summary>
        public bool IsToolWindow { get; set; }

        /// <summary>
        /// Id to distinguish this page, for common events and saving the layout.
        /// Must be the same id that menu item id associated.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Indicates if the page can be added to the favourites list. Default: true
        /// </summary>
        public bool CanBeFavourite { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has settings page.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has settings page; otherwise, <c>false</c>.
        /// </value>
        public bool HasSettingsPage { get; protected set; }

        /// <summary>
        /// Gets or sets the status message: indicating user, date, changes, etc.
        /// </summary>
        /// <value>
        /// The status message.
        /// </value>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Activate/deactivate the ViewBox mode.
        /// </summary>
        public bool IsScalablePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hosting a winform control.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is winform control; otherwise, <c>false</c>.
        /// </value>
        public bool IsWinformControl { get; set; }

        /// <summary>
        /// Flag that indicates if the user has started editing the information.
        /// When IsDirty is set to true, the "Save" and "Cancel" buttons will be activated in 
        /// the container control.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [ignore errors].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [ignore errors]; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreErrors { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether [allows to save this editor as layout package].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow layout]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowLayout { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditionMode { get; protected set; }
        public IWorkspace Workspace { get; protected set; }

        #endregion

        #region Virtual methods

        /// <summary>
        /// Called when the page is added to a parent window
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            return null;
        }

        /// <summary>
        /// Opens the page settings. To be implemented in each editor that needs settings page.
        /// </summary>
        public virtual void OpenPageSettings() { }

        /// <summary>
        /// Determines whether this instance [can execute save].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can execute create]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanExecuteSave()
        {
            return !HasErrors; // TODO FRAN: por ahora no tenemos en cuenta el IsDirty, que se activará (cuando se implemente) cuando el editor detecte cambios
            //return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            // Nothing to do
        }

        /// <summary>
        /// Tells the editor that the user wants to cancel the changes he has made
        /// </summary>
        public virtual void Cancel() { }

        /// <summary>
        /// Tells the editor to reload the information from the database
        /// </summary>
        public virtual void Refresh() { }

        /// <summary>
        /// Called when the container user control or window is closed.
        /// Before closing, check if HasErrors and if IsDirty
        /// </summary>
        public virtual void OnClose() { }

        /// <summary>
        /// Saves the versionable element.
        /// </summary>
        public virtual void SaveVersionable() { }

        /// <summary>
        /// Saves as new version.
        /// </summary>
        public virtual void SaveAsNewVersion() { }

        /// <summary>
        /// Saves as new element.
        /// </summary>
        public virtual void SaveAsNewElement() { }

        #endregion

        #region Abstract methods

        /// <summary>
        /// Initializes the control with a specific item to be edited, of 
        /// the type described in the ItemType property
        /// </summary>
        /// <param name="objectToEdit"></param>
        public abstract void Load(object objectToEdit);

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public abstract void Save();

        #endregion
    }

}
