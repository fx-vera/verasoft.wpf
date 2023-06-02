using System;
using System.Windows;

namespace VeraSoft.Wpf.Core
{
    /// <summary>
    /// Interface to be followed by all controls that are the main content of an independent window
    /// It provides some common properties and functionalities so that a common window frame 
    /// can be provided as a container.
    /// </summary>
    public interface IViewModel : ICloneable, IDisposable
    {
        /// <summary>
        /// Name of the editor
        /// </summary>
        string PageName { get; }

        /// <summary>
        /// Initializes this instance. Manadatory when developer create an instance with "new" 
        /// </summary>
        void Initialize();

        /// <summary>
        /// Indicates if the page can be cloned with the exact same content
        /// </summary>
        bool CanBeCloned { get; }

        /// <summary>
        /// Indicates if the page can be closed
        /// </summary>
        bool CanBeClosed { get; }

        /// <summary>
        /// Indicates if the page can be maximized
        /// </summary>
        bool CanBeMaximized { get; }

        /// <summary>
        /// Indicates if the page can be minimized
        /// </summary>
        bool CanBeMinimized { get; }

        /// <summary>
        /// Indicates if the page position and size can be "locked" ("pinned"), 
        /// to prevent it from being moved or maximized
        /// </summary>
        bool CanBePinned { get; }

        /// <summary>
        /// Indicates if the page can be opened several times or only one instance is allowed
        /// </summary>
        bool CanHaveMultipleInstances { get; }

        /// <summary>
        /// In case that a page can have multiple instances, maximum number of instances that
        /// are allowed. -1 indicates there is no restriction.
        /// </summary>
        int MaxNumInstances { get; }

        /// <summary>
        /// Inidcates if the page can be added to the favourites list
        /// </summary>
        bool CanBeFavourite { get; }

        /// <summary>
        /// Indicates if the page can be made transparent (change its 
        /// alpha value so that other content below is partly visible)
        /// </summary>
        bool IsTransparent { get; }

        /// <summary>
        /// Indicates if when resizing the page, it should always maintain its aspect ratio
        /// </summary>
        bool KeepAspectRatio { get; }

        /// <summary>
        /// Indicates if this page should have a Tool Window-like appearance
        /// </summary>
        bool IsToolWindow { get; }

        /// <summary>
        /// Id to distinguish this page, for common events and saving the layout
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Indicates if the window can be resized, and how
        /// </summary>
        ResizeMode ResizeMode { get; set; }

        /// <summary>
        /// Gets or sets the initial size.
        /// </summary>
        /// <value>
        /// The initial size.
        /// </value>
        Size InitialSize { get; set; }

        /// <summary>
        /// Gets or sets the size to content mode.
        /// </summary>
        /// <value>
        /// The size to content mode.
        /// </value>
        SizeToContent SizeToContentMode { get; set; }

        /// <summary>
        /// Gets or sets the initial left.
        /// </summary>
        /// <value>
        /// The initial left.
        /// </value>
        double InitialLeft { get; }

        /// <summary>
        /// Gets or sets the initial top.
        /// </summary>
        /// <value>
        /// The initial top.
        /// </value>
        double InitialTop { get; }

        /// <summary>
        /// Opens the page settings.
        /// </summary>
        void OpenPageSettings();

        /// <summary>
        /// Determines whether this instance [can execute save].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can execute save]; otherwise, <c>false</c>.
        /// </returns>
        bool CanExecuteSave();

        /// <summary>
        /// Initializes the control with a specific item to be edited, of 
        /// the type described in the ItemType property
        /// </summary>
        /// <param name="objectToEdit"></param>
        void Load(object objectToEdit);

        /// <summary>
        /// Tells the editor to reload the information from the database
        /// </summary>
        void Refresh();

        /// <summary>
        /// Saves this instance.
        /// </summary>
        void Save();

        /// <summary>
        /// Tells the editor that the user wants to cancel the changes he has made
        /// </summary>
        void Cancel();

        /// <summary>
        /// Called when the container user control or window is closed.
        /// Before to close, check if HasErrors and if IsDirty
        /// </summary>
        void OnClose();

        /// <summary>
        /// Saves the versionable element.
        /// </summary>
        void SaveVersionable();

        /// <summary>
        /// Saves as new version.
        /// </summary>
        void SaveAsNewVersion();

        /// <summary>
        /// Saves as new element.
        /// </summary>
        void SaveAsNewElement();

        /// <summary>
        /// Gets or sets a value indicating whether this instance has settings page.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has settings page; otherwise, <c>false</c>.
        /// </value>
        bool HasSettingsPage { get; }

        /// <summary>
        /// Gets or sets the status message: indicating user, date, changes, etc.
        /// </summary>
        /// <value>
        /// The status message.
        /// </value>
        string StatusMessage { get; }

        /// <summary>
        /// Shows or hide a switch to activate ViewBox mode
        /// </summary>
        bool IsScalablePage { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hosting a winform control.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is winform control; otherwise, <c>false</c>.
        /// </value>
        bool IsWinformControl { get; set; }

        /// <summary>
        /// Flag that indicates if the user has started editing the information.
        /// When IsDirty is set to true, the "Save" and "Cancel" buttons will be activated in 
        /// the container control.
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [ignore errors].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [ignore errors]; otherwise, <c>false</c>.
        /// </value>
        bool IgnoreErrors { get; }

        /// <summary>
        /// Gets a value indicating whether [allows to save this editor as layout package].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow layout]; otherwise, <c>false</c>.
        /// </value>
        bool AllowLayout { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        bool IsEditionMode { get; }
        IWorkspace Workspace { get; }
    }
}
