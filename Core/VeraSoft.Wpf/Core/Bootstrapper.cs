using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using VeraSoft.Wpf.Core.Components;
using VeraSoft.Wpf.Events;
using VeraSoft.Wpf.Exceptions;
using VeraSoft.Wpf.Mainframe;
using VeraSoft.Wpf.Managers;

namespace VeraSoft.Wpf.Core
{
    /// <summary>
    /// Subclass of Bootstrapper that provides a default main window, 
    /// a loading progress dialog, a workspace to find the loaded modules, 
    /// and loads some default xaml resources
    /// </summary>
    public abstract class Bootstrapper : Application
    {
        private ViewsManager _viewsManager;
        private MainWindow _mainWindow;
        private IMainWindowViewModel _mainViewModel;

        private IEventAggregator _eventAggregator;

        public Bootstrapper()
        {
            //var media = new MediaManager.ProgramLauncher();
            //media.Dispose();

            //new Result();
            //var res = new Result();
            //new Solution.Solution();
            LoadOnlyDllsInWhiteList = true;
            UnhandledExceptionHandler.Init();
        }

        /// <summary>
        /// Gets or sets the workspace.
        /// </summary>
        /// <value>
        /// The workspace.
        /// </value>
        public IWorkspace Workspace { get; protected set; }

        /// <summary>
        /// Gets the views manager.
        /// </summary>
        /// <value>
        /// The views manager.
        /// </value>
        public ViewsManager ViewsManager { get { return _viewsManager; } }

        /// <summary>
        /// List of substrings of the dll names that WILL be allowed when trying MEF composition
        /// </summary>
        public List<string> MefDllNamesWhiteList { get; set; }

        /// <summary>
        /// List of substrings of the dll names that will NOT be allowed when trying MEF composition
        /// </summary>
        public List<string> MefDllNamesBlackList { get; set; }

        //Si ponemos useWhiteList a true, se cogerán sólo las dlls cuyo nombre contenga lo que viene en "dllsToInclude"
        //Si ponemos useWhiteList a false, se cogerán todas las dlls menos aquellas cuyo nombre contenga lo que viene en "dllsToAvoid"

        /// <summary>
        /// This variable determines:
        ///  - If false: when searching for dlls to compose in MEF, look for all dlls in the bin directory that do not contain in the name the strings in the MefDllNamesBlackList
        ///  - If true: when searching for dlls to compose in MEF, get only dlls in the bin directory that contain in the name the strings in the MefDllNamesBlackList, and
        ///  that do not contain the strings in the MefDllNamesBlackList
        /// </summary>
        public bool LoadOnlyDllsInWhiteList { get; set; }

        /// <summary>
        /// Function called just after initializing the logger and before anything else is done
        /// </summary>
        public virtual void Initialize()
        {
            LoadResourceDictionaries();
        }

        /// <summary>
        /// Several steps: parse the info, creates the mainframe and the mission control bar
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Initialize();
            //Load application modules
            List<Assembly> assembliesToCompose = GetAssembliesForIoC();
            if (assembliesToCompose == null)
                assembliesToCompose = new List<Assembly>();
            assembliesToCompose.Add(Assembly.GetExecutingAssembly());

            Dictionary<Type, object> knownInstances = GetPreloadedIoCInstances();
            _eventAggregator = EventAggregator.Default;
            knownInstances.Add(typeof(IEventAggregator), _eventAggregator);
            string loadError = string.Empty;
            bool modulesLoadedOk = false;
            try
            {
                MEFIoCManager.Instance.LoadModules(knownInstances, assembliesToCompose);
                modulesLoadedOk = true;
            }
            catch (ModuleLoadErrorException ex)
            {
                loadError = ex.Message;
            }

            if (!modulesLoadedOk)
            {
                MessageBox.Show(loadError, "Error Loading Application Modules");
                Current.Shutdown();
                return;
            }

            //Let application do post-initialization
            OnModulesLoaded();

            MainWindow = CreateMainWindow();
            if (MainWindow == null)
                Current.Shutdown();
            else
            {
                MainWindow.Closing += (o, ee) => CustomShutDown();
                Workspace.LoadProfile();
                LoadPlugins();
                ((IMainWindowViewModel)MainWindow.DataContext).LoadFlyouts();
                ((IMainWindowViewModel)MainWindow.DataContext).SetSelectedPlugin();
                Workspace.MainWindow = MainWindow;
                MainWindow.Show();
            }
        }

        private void CustomShutDown()
        {
            CloseApplication();
            Current.Shutdown();
        }

        /// <summary>
        /// Closes the application.
        /// </summary>
        protected virtual void CloseApplication()
        {
            var pageWindow = GetMainWindow();
        }

        /// <summary>
        /// Gets the main window.
        /// </summary>
        /// <returns></returns>
        protected virtual IMainWindowViewModel GetMainWindow()
        {
            return IoC.Get<IMainWindowViewModel>();
        }

        /// <summary>
        /// Retrieves the list of assemblies containing parts or modules 
        /// that must be composed using MEF and instantiated through IoC.
        /// In this case, it only adds the current executing assembly. 
        /// If a subclass overrides this, it should call this base implementation, 
        /// take the returned list and then add whatever it wants to it, and return it.
        /// </summary>
        /// <returns>The list of assemblies to be loaded and composed</returns>
        protected virtual List<Assembly> GetAssembliesForIoC()
        {
            List<Assembly> list = new List<Assembly>();
            list.Add(Assembly.GetExecutingAssembly());

            list.AddRange(GetAllDllsFromExeDir());

            return list;
        }
        public abstract void ConfigurePluginNames();

        /// <summary>
        /// Gets all DLLS from executable dir.
        /// </summary>
        /// <returns></returns>
        private List<Assembly> GetAllDllsFromExeDir()
        {
            ConfigurePluginNames();
            List<Assembly> possiblePlugins = new List<Assembly>();
            string exeDir = Path.GetDirectoryName(GetType().Assembly.Location);

            string[] dllsInDir = Directory.GetFiles(exeDir, "*.dll");

            for (int i = 0; i < dllsInDir.Length; i++)
            {
                string dllWithPath = dllsInDir[i];
                string dllName = Path.GetFileName(dllWithPath);
                bool tryLoadDll = true;
                if (LoadOnlyDllsInWhiteList)
                    tryLoadDll = MefDllNamesWhiteList.Any(x => dllName.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) >= 0);

                tryLoadDll = tryLoadDll & !MefDllNamesBlackList.Any(x => dllName?.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) >= 0);

                if (tryLoadDll)
                {
                    possiblePlugins.Add(Assembly.LoadFile(dllWithPath));
                }
            }

            possiblePlugins.Add(GetType().Assembly);

            return possiblePlugins;
        }

        /// <summary>
        /// Gets already created instances of objects that should be added 
        /// to the MEF parts catalog
        /// If a subclass overrides this, it should call this base implementation, 
        /// take the returned dictionary and then add whatever it wants to it, and return it.
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<Type, object> GetPreloadedIoCInstances()
        {
            var result = new Dictionary<Type, object>();
            result.Add(GetType(), this);
            return result;
        }

        /// <summary>
        /// Called after all modules have been loaded with MEF. 
        /// In this case, it loads the workspace and some resource dictionaries
        /// </summary>
        protected virtual void OnModulesLoaded()
        {
            LoadApplicationViews();
            LoadWorkspace();
        }

        /// <summary>
        /// Loads the workspace.
        /// </summary>
        protected virtual void LoadWorkspace()
        {
            Workspace = IoC.Get<IWorkspace>();
        }

        /// <summary>
        /// Function called to create the main application window with
        /// any implementation the subclasses want.
        /// </summary>
        /// <returns></returns>
        protected Window CreateMainWindow()
        {
            _mainViewModel = GetMainWindow();
            if (_mainViewModel == null)
            {
                MessageBox.Show("Could not find main window component. Closing down.", "Error");
                return null;
            }
            _mainWindow = new MainWindow(_mainViewModel);

            //LoadPlugins();
            _mainWindow.Closing += (o, e) => _mainViewModel.OnMainWindowClosing();
            _mainWindow.Closed += (o, e) => _mainViewModel.OnMainWindowClosed();

            _mainWindow.Dispatcher.BeginInvoke(new Action(() => _mainWindow.SetCurrentValue(Window.TopmostProperty, false)), System.Windows.Threading.DispatcherPriority.ApplicationIdle, null);

            return _mainWindow;
        }

        protected virtual void LoadPlugins()
        {
            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
            FavouriteEventArgs args = new FavouriteEventArgs();
            var itemsOfGivenType = IoC.GetAll<IPluginItem>();

            foreach (var item in itemsOfGivenType)
            {
                args = new FavouriteEventArgs();
                args.Id = item.Id;
                args.Item = item;
                args.IsCommand = true;
                eventAggregator.Send(args);
            }
        }

        private void OnLoadingProgressWindowClosed(object sender, CancelEventArgs e)
        {
            ((Window)sender).Closing -= OnLoadingProgressWindowClosed;
        }

        private void LoadApplicationViews()
        {
            _viewsManager = IoC.Get<ViewsManager>();
            _viewsManager.LoadAvailiableViews();
            MEFIoCManager.Instance.LoadModules(new Dictionary<Type, object>(), GetAssembliesForIoC());
        }

        protected virtual bool LoadResourceDictionaries()
        {
            var themeManager = new Themes.ThemeManager();
            Current.Resources.MergedDictionaries.Add(themeManager.GetThemeResources());

            List<ResourceDictionary> allResources = new List<ResourceDictionary>();
            ResourceDictionary myResourceDictionary = new ResourceDictionary();
            myResourceDictionary.Source = new Uri(@"pack://application:,,,/VeraSoft.Resources;component/Icons.xaml", UriKind.Absolute);
            allResources.Add(myResourceDictionary);
            allResources.ForEach(x => Current.Resources.MergedDictionaries.Add(x));

            return true;
        }
    }
}
