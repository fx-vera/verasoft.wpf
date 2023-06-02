using System;
using VeraSoft.Wpf.Core;
using VeraSoft.Wpf.Events;
using VeraSoft.Wpf.Extensions;

namespace VeraSoft.Wpf.Managers
{
    /// <summary>
    /// Component that creates windows for pages and keeps track of them
    /// </summary>
    public interface IPageManager
    {
        event EventHandler<WindowEventArgs> WindowStateChanged;
        event EventHandler<WindowEventArgs> WindowCreated;
        event EventHandler<WindowEventArgs> WindowClosed;

        ObservableCollection<IWindowViewModel> OpenWindows { get; }


        /// <summary>
        /// Creates a new page/editor for the given contract name
        /// </summary>
        /// <param name="pageContractName">Exported contract name of the content. 
        /// The content will be created by calling IoC.Get[IPage](pageType)</param>
        /// <returns>The page that has been created, or null if it couldn't be created</returns>
        IViewModel CreatePage(string pageContractName);

        /// <summary>
        /// Uses reflection to get the page to be launched. The class has to be pointed by the
        /// namespace and the assembly. So that, any new project shall include a new line in the
        /// assemblies list
        /// </summary>
        /// <param name="pattern">Class to be instanciated</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The page itself
        /// </returns>
        IViewModel CreatePageFromAssembly(string pattern, object[] args = null);

        /// <summary>
        /// - For single instance windows: creates the single instance if not present, 
        /// or unminimizes and brings to front the existing one
        /// - For multiple instance windows: opens a new instance of the window if the maximum number has not been reached
        /// </summary>
        /// <param name="pageContractName">Exported contract name of the content. The content will be created by calling CreatePage</param>
        /// <param name="objectToLoad">If it is an editor, the parameter to pass to the "Load" function before opening the window</param>
        /// <param name="doNotCheckNumInstances">Create the window no matter if the editor is set to one instance or a maximum number</param>
        /// <param name="modalWindow">If the window should be opened as modal (no other window can be accessed until this new one is closed)</param>
        /// <returns>The window has been created and/or opened, or null if the operation could not be performed</returns>
        IWindowViewModel OpenPageWindow(string pageContractName, object objectToLoad = null, bool doNotCheckNumInstances = false, bool modalWindow = false);

        /// <summary>
        /// - For single instance windows: creates the single instance if not present, 
        /// or unminimizes and brings to front the existing one
        /// - For multiple instance windows: opens a new instance of the window if the maximum number has not been reached
        /// </summary>
        /// <param name="newPage">Page Content to insert in the new window</param>
        /// <param name="objectToLoad">If it is an editor, the parameter to pass to the "Load" function before opening the window</param>
        /// <param name="doNotCheckNumInstances">Create the window no matter if the editor is set to one instance or a maximum number</param>
        /// <param name="modalWindow">If the window should be opened as modal (no other window can be accessed until this new one is closed)</param>
        /// <returns>The window has been created and/or opened, or null if the operation could not be performed</returns>
        IWindowViewModel OpenPageWindow(IViewModel newPage, object objectToLoad = null, bool doNotCheckNumInstances = false, bool modalWindow = false, bool showInMainframe = false);

        /// <summary>
        /// If multiple instances are allowed and the maximum has not been reached, 
        /// clones the given content and opens this clone in a new window 
        /// </summary>
        /// <param name="p">Page content to be cloned</param>
        /// <returns></returns>
        IWindowViewModel Clone(IViewModel p);

        /// <summary>
        /// Minimize all open pages
        /// </summary>
        void MinimizeAllWindows();

        /// <summary>
        /// Close all opened pages, including minimized
        /// </summary>
        /// <remarks>
        /// Pages with attribute CloseAttr = false will not be closed
        /// </remarks>
        void CloseAllWindows();

        /// <summary>
        /// Request the open or minimized pages to disable the availability of the controls
        /// </summary>
        void ChangeAvailabilityOfAllPages(bool enable);

        IWindowViewModel FindPageWindow(IViewModel p);

        void CloseWindow(IViewModel page);

        IWindowViewModel FindPageWindowById(string id);
    }
}
