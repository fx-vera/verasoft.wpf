using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using VeraSoft.Wpf.Core;
using VeraSoft.Wpf.Core.Components;
using VeraSoft.Wpf.Enums;
using VeraSoft.Wpf.Events;
using VeraSoft.Wpf.Extensions;
using VeraSoft.Wpf.Mainframe;

namespace VeraSoft.Wpf.Managers
{
    [Export(typeof(IPageManager))]
    public class PageManager : IPageManager
    {
        #region Attributes
        private ObservableCollection<IWindowViewModel> _openWindows;

        /// <summary>
        /// List of open windows
        /// </summary>
        public ObservableCollection<IWindowViewModel> OpenWindows { get { return _openWindows; } }

        [ImportMany(typeof(IPageWindowCreator))]
        private List<IPageWindowCreator> _windowCreators;

        protected Dictionary<Type, IPageWindowCreator> _windowCreatorsDic;

        private List<Assembly> m_pagesDLLs;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PageManager"/> class.
        /// </summary>
        [ImportingConstructor]
        public PageManager()
        {
            _openWindows = new ObservableCollection<IWindowViewModel>();
            if (_windowCreators != null)
                _windowCreatorsDic = _windowCreators.ToDictionary(x => x.PageType, x => x);
            else
                _windowCreatorsDic = new Dictionary<Type, IPageWindowCreator>();
        }

        /// <summary>
        /// Occurs when [window state changed].
        /// </summary>
        public event EventHandler<WindowEventArgs> WindowStateChanged;
        /// <summary>
        /// Occurs when [window created].
        /// </summary>
        public event EventHandler<WindowEventArgs> WindowCreated;
        /// <summary>
        /// Occurs when [window closed].
        /// </summary>
        public event EventHandler<WindowEventArgs> WindowClosed;

        #region IPageManager functions

        /// <summary>
        /// Creates a new page/editor for the given contract name
        /// </summary>
        /// <param name="pageContractName">Exported contract name of the content. 
        /// The content will be created by calling IoC.Get[IPage](pageType)</param>
        /// <returns>The page that has been created, or null if it couldn't be created</returns>
        public virtual IViewModel CreatePage(string pageContractName)
        {
            return IoC.Get<IViewModel>(pageContractName);
        }

        /// <summary>
        /// - For single instance windows: creates the single instance if not present, 
        /// or unminimizes and brings to front the existing one
        /// - For multiple instance windows: opens a new instance of the window if the maximum number has not been reached
        /// </summary>
        /// <param name="pageContractName">Exported contract name of the content. The content will be created by calling CreatePage</param>
        /// <param name="objectToLoad">If it is an editor, the parameter to pass to the "Load" function before opening the window</param>
        /// <param name="doNotCheckNumInstances">Create the window no matter if the editor is set to one instance or a maximum number</param>
        /// <param name="modalWindow">If the window should be opened as modal (no other window can be accessed until this new one is closed)</param>
        /// <returns>True if the window has been created and opened. False in any other case</returns>
        public virtual IWindowViewModel OpenPageWindow(string pageContractName, object objectToLoad = null, bool doNotCheckNumInstances = false, bool modalWindow = false)
        {
            return OpenPageWindow(CreatePage(pageContractName), objectToLoad, doNotCheckNumInstances, modalWindow);
        }

        /// <summary>
        /// - For single instance windows: creates the single instance if not present, 
        /// or unminimizes and brings to front the existing one
        /// - For multiple instance windows: opens a new instance of the window if the maximum number has not been reached
        /// </summary>
        /// <param name="newPage">Page Content to insert in the new window</param>
        /// <param name="objectToLoad">If it is an editor, the parameter to pass to the "Load" function before opening the window</param>
        /// <param name="doNotCheckNumInstances">Create the window no matter if the editor is set to one instance or a maximum number</param>
        /// <param name="modalWindow">If the window should be opened as modal (no other window can be accessed until this new one is closed)</param>
        /// <returns>True if the window has been created and opened. False in any other case</returns>
        public virtual IWindowViewModel OpenPageWindow(IViewModel newPage, object objectToLoad = null, bool doNotCheckNumInstances = false, bool modalWindow = false, bool showInMainframe = false)
        {
            if (newPage == null)
            {
                return null;
            }
            newPage.Initialize();

            IWindowViewModel existingAllowedInstance = null;
            IWindowViewModel openedInstance = null;
            bool canOpenNewWindow = doNotCheckNumInstances ? true : CheckIfNewInstanceIsAllowed(newPage, out existingAllowedInstance);


            //If a new window can be opened, open it
            if (canOpenNewWindow)
            {
                LoadObjectInPage(newPage, objectToLoad);
                openedInstance = OpenInNewWindow(newPage, modalWindow);
                if (showInMainframe)
                {
                    //LoadObjectInPage(newPage, objectToLoad);
                    var mainframe = IoC.Get<IMainWindowViewModel>();
                    mainframe.ViewModel = openedInstance.ViewModel;
                }
                else
                {
                    if (!modalWindow)
                    {
                        openedInstance.View.Show();
                        openedInstance.View.Activate();
                    }
                    else
                    {
                        openedInstance.View.ShowDialog();
                    }
                }
            }
            //If there is an already existing instance, make sure that it is not
            //minimized and bring it to the front
            else if (existingAllowedInstance != null)
            {
                openedInstance = existingAllowedInstance;
                LoadObjectInPage(existingAllowedInstance.ViewModel, objectToLoad);
                if (showInMainframe)
                {
                    var mainframe = IoC.Get<IMainWindowViewModel>();
                    mainframe.ViewModel = openedInstance.ViewModel;
                }
                else
                {
                    Window ownerWindow = existingAllowedInstance.View;
                    if (ownerWindow.WindowState == WindowState.Minimized)
                        ownerWindow.WindowState = WindowState.Normal;
                    else
                        ownerWindow.Show();
                    ownerWindow.Activate();
                }
            }
            return openedInstance;
        }

        /// <summary>
        /// Clones the specified p.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public virtual IWindowViewModel Clone(IViewModel p)
        {
            if (p == null || !p.CanBeCloned)
                return null;

            IViewModel clone = p ?? p.Clone() as IViewModel;
            return OpenPageWindow(p);
        }

        /// <summary>
        /// Finds the page window.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public IWindowViewModel FindPageWindow(IViewModel p)
        {
            return _openWindows.FirstOrDefault(x => x.ViewModel == p);
        }

        /// <summary>
        /// Finds the page window by identifier.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public IWindowViewModel FindPageWindowById(IViewModel p)
        {
            if (string.IsNullOrEmpty(p.Id))
            {
                return null;
            }

            try
            {
                var page = _openWindows.FirstOrDefault(x => x.ViewModel.Id != null && x.ViewModel.Id.Equals(p.Id));
                return page;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Finds the page window by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public IWindowViewModel FindPageWindowById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            try
            {
                var page = _openWindows.FirstOrDefault(x => x.ViewModel.Id.Equals(id));
                return page;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Minimize all the opened pages
        /// </summary>
        public void MinimizeAllWindows()
        {
            // Minimizamos las paginas, que pasarán de la lista de abiertas a la de minimizadas
            List<IWindowViewModel> temp = new List<IWindowViewModel>(_openWindows);
            foreach (var openWindow in temp)
            {
                openWindow.Minimize();
            }
        }

        /// <summary>
        /// Close all opened pages, including minimized
        /// </summary>
        /// <remarks>
        /// Pages with attribute CloseAttr = false will not be closed
        /// </remarks>
        public void CloseAllWindows()
        {
            List<IWindowViewModel> removedWindows = new List<IWindowViewModel>();

            foreach (var window in _openWindows?.ToList())
            {
                if (window.ViewModel.CanBeClosed)
                {
                    window.Close();
                }
            }
        }

        /// <summary>
        /// Closes the window. For pages type IcaroPageBase.
        /// </summary>
        /// <param name="page">The page.</param>
        public void CloseWindow(IViewModel page)
        {
            IWindowViewModel window = null;

            if ((window = FindPageWindowById(page)) != null && (window = FindPageWindow(page)) != null)
            {
                window.Close();
            }
        }

        /// <summary>
        /// Request the open or minimized pages to disable the availability of the controls
        /// </summary>
        public void ChangeAvailabilityOfAllPages(bool enable)
        {
            foreach (var window in _openWindows)
            {
                window.SetEnabled(enable);
            }
        }

        #endregion
        /// <summary>
        /// Creates the window view model.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        protected virtual IWindowViewModel CreateWindowViewModel(IViewModel p)
        {
            IWindowViewModel w = null;
            IPageWindowCreator creator = null;
            if (_windowCreatorsDic.Count > 0 &&
                _windowCreatorsDic.TryGetValue(p.GetType(), out creator) &&
                creator != null)
            {
                w = creator.CreateWindow();
                w.ViewModel = p;
            }
            else
            {
                w = CreateDefaultWindowViewModel(p);
            }

            return w;
        }

        /// <summary>
        /// Creates the default window view model.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        protected virtual IWindowViewModel CreateDefaultWindowViewModel(IViewModel p)
        {
            return new GenericViewModel(p);
        }

        /// <summary>
        /// Creates a new window, registers it in a list and follows its state. 
        /// It doesn't check the window properties about cloning or having multiple instances.
        /// </summary>
        /// <param name="newPage">Instance that will be added to the window container</param>
        /// <param name="isModal">Indicates if the page will be opened as modal (only interaction possible in the application will be with the new window)</param>
        protected virtual IWindowViewModel OpenInNewWindow(IViewModel newPage, bool isModal = false)
        {
            IWindowViewModel newWindowViewModel = CreateWindowViewModel(newPage);
            if (newWindowViewModel != null)
            {
                Register(newWindowViewModel);
                SetDataContext(newWindowViewModel);
            }
            return newWindowViewModel;
        }

        private void LoadObjectInPage(IViewModel newPage, object objectToLoad)
        {
            if (objectToLoad == null)
                return;

            try
            {
                newPage.Load(objectToLoad);
            }
            catch (Exception ex)
            {
            }
        }

        private void SetDataContext(IWindowViewModel newWindowViewModel)
        {
            newWindowViewModel.View.DataContext = newWindowViewModel;
            //Para problemas con WindowsFormsHost que haya en el contenido de la ventana
            //https://www.codeproject.com/Questions/217236/WPF-WebBrowser-Problem-if-AllowsTransparencyequals
            // en el viewmodel poner IsWinformControl = true
            if (newWindowViewModel.ViewModel.IsWinformControl)
                BindingOperations.SetBinding(newWindowViewModel.View, Window.AllowsTransparencyProperty, new Binding("IsTransparent") { Mode = BindingMode.OneTime });
            //https://social.msdn.microsoft.com/Forums/vstudio/en-US/d77de508-74b6-49f9-bbc7-fefc7a94c904/binding-allowstransparency-invalidoperationexception?forum=wpf
            //https://blogs.msdn.microsoft.com/changov/2009/01/19/webbrowser-control-on-transparent-wpf-window/
        }

        private bool CheckIfNewInstanceIsAllowed(IViewModel newPage, out IWindowViewModel existingAllowedInstance)
        {
            bool canOpenNewWindow = false;
            existingAllowedInstance = null;
            //If multiple instances can be opened, check the limitations
            if (newPage.CanHaveMultipleInstances)
            {
                if (newPage.MaxNumInstances < 0 || _openWindows.Count(x => x.ViewModel.Id == newPage.Id) < newPage.MaxNumInstances)
                {
                    canOpenNewWindow = true;
                }
            }
            //If only one instance can be opened, find if there is one already open
            else
            {
                //Probably one method should be enough: find a window with the same Id. The second check is to finde the same viewmodel instance/pointer
                if ((existingAllowedInstance = FindPageWindowById(newPage)) == null &&
                    (existingAllowedInstance = FindPageWindow(newPage)) == null)
                {
                    canOpenNewWindow = true;
                }
            }

            return canOpenNewWindow;
        }

        /// <summary>
        /// Subscribes to the window events and adds it to the list of open windows
        /// </summary>
        /// <param name="window"></param>
        private void Register(IWindowViewModel window)
        {
            if (window == null)
                return;

            SubscribeToWindowEvents(window);
            _openWindows.Add(window);
        }

        /// <summary>
        /// Unsubscribes from the window events and removes it from the list of open windows
        /// </summary>
        /// <param name="window"></param>
        private void Unregister(IWindowViewModel window)
        {
            if (window == null)
                return;

            UnsubscribeFromWindowEvents(window);
            _openWindows.Remove(window);
        }

        private void SubscribeToWindowEvents(IWindowViewModel newWindow)
        {
            newWindow.WindowStateChanged += OnWindowStateChanged;
            newWindow.WindowCloned += OnWindowCloned;
        }

        private void UnsubscribeFromWindowEvents(IWindowViewModel newWindow)
        {
            newWindow.WindowStateChanged -= OnWindowStateChanged;
            newWindow.WindowCloned -= OnWindowCloned;
        }

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
        public IViewModel CreatePageFromAssembly(string pattern, object[] args = null)
        {
            Type pageTypeToCreate = null;
            IViewModel createdPage = null;

            if (m_pagesDLLs == null)
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                m_pagesDLLs = currentDomain.GetAssemblies()?.ToList();
            }

            for (int i = 0; i < m_pagesDLLs.Count && pageTypeToCreate == null; ++i)
            {
                pageTypeToCreate = GetTypeByName(pattern, m_pagesDLLs[i]);
            }

            if (pageTypeToCreate != null)
            {
                if (args != null)
                {
                    createdPage = Activator.CreateInstance(pageTypeToCreate, args) as IViewModel;
                }
                else
                {
                    createdPage = Activator.CreateInstance(pageTypeToCreate) as IViewModel;
                }
            }

            if (createdPage == null)
            {
            }

            return createdPage;
        }

        /// <summary>
        /// Gets the name of the type by.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="assemblyToLookInto">The assembly to look into.</param>
        /// <returns></returns>
        protected static Type GetTypeByName(string className, Assembly assemblyToLookInto)
        {
            return assemblyToLookInto.GetTypes().FirstOrDefault(x => x.Name.Equals(className, StringComparison.OrdinalIgnoreCase));
        }

        private void OnWindowStateChanged(object sender, WindowStateEventArgs e)
        {
            if (e.State == PageWindowState.Closed)
            {
                OnWindowClosed(sender as IWindowViewModel);
            }
        }

        private void OnWindowClosed(IWindowViewModel w)
        {
            if (w == null)
                return;
            Unregister(w);
        }

        private void OnWindowCloned(object sender, WindowEventArgs e)
        {
            // Cuando se clona una pagina, el mainframe se tiene que atachear
            // a los eventos de dicha página
            Register(e.Window);
        }
    }
}
