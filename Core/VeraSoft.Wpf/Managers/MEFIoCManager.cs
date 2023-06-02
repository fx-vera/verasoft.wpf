using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using VeraSoft.Wpf.Events;
using VeraSoft.Wpf.Exceptions;

namespace VeraSoft.Wpf.Managers
{
    /// <summary>
    /// Allows loading modules using MEF and assigns to itself and implements the IoC class functions
    /// </summary>
    public class MEFIoCManager
    {
        private MyCompositionContainer _container;
        private static readonly MEFIoCManager _instance = new MEFIoCManager();

        public static MEFIoCManager Instance { get { return _instance; } }

        public event EventHandler<ModuleLoadedEventArgs> ModuleLoaded;

        private MEFIoCManager()
        {
            SetupIoC();
        }

        private void SetupIoC()
        {
            IoC.GetInstance = GetInstance;
            IoC.SetInstance = SetInstance;
            IoC.GetInstanceByName = GetInstanceByName;
            IoC.GetAllInstances = GetAllInstances;
            IoC.BuildUp = BuildUp;
        }

        public virtual object GetInstanceByName(string typeName)
        {
            var exports = _container.GetExportedValues<object>(typeName);

            return exports?.FirstOrDefault();
        }

        public virtual T GetInstance<T>() where T : class
        {
            return GetInstance<T>(null);
        }

        public virtual T GetInstance<T>(string key) where T : class
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(typeof(T)) : key;
            var exports = _container.GetExportedValues<T>(contract);

            return exports?.FirstOrDefault();
        }

        public virtual object GetInstance(Type serviceType)
        {
            return GetInstance(serviceType, null);
        }

        public bool SetInstance(Type serviceType, string contractName)
        {
            string contract = string.IsNullOrEmpty(contractName) ? AttributedModelServices.GetContractName(serviceType) : contractName;
            _container.ComposeExportedValue(contractName, serviceType);
            _container.ComposeParts();

            var batch = new CompositionBatch();
            var ty = serviceType.GetType();
            var r2 = serviceType.BaseType;
            var t3y = serviceType.UnderlyingSystemType;
            var part1 = batch.AddExportedValue(contract, serviceType);

            var toadd = batch.PartsToAdd;


            return true;
        }

        public void ComposeParts()
        {
            _container.ComposeParts();
        }

        public virtual object GetInstance(Type serviceType, string contractName)
        {
            string contract = string.IsNullOrEmpty(contractName) ? AttributedModelServices.GetContractName(serviceType) : contractName;
            var exports = _container.GetExportedValues<object>(contract);
            if (exports == null || exports?.ToList().Count == 0)
            {
                return null;
            }
            var myexport = exports?.FirstOrDefault();

            if (myexport is UserControl)
            {

                var modelType = (UserControl)myexport;
                return modelType;
            }

            return exports?.FirstOrDefault();
        }

        public virtual IEnumerable<object> GetAllInstances(Type serviceType)
        {
            IEnumerable<object> res = _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
            return res;
        }

        public virtual IEnumerable<object> GetAllInstances(Type serviceType, string contractName)
        {
            string contract = string.IsNullOrEmpty(contractName) ? AttributedModelServices.GetContractName(serviceType) : contractName;
            IEnumerable<object> res = _container.GetExportedValues<object>(contract);
            return res;
        }

        public virtual void BuildUp(object instance)
        {
            _container.SatisfyImportsOnce(instance);
        }

        class AssemblyNameComparer : IEqualityComparer<Assembly>
        {
            public bool Equals(Assembly x, Assembly y)
            {
                bool equal = x == y;
                return x == y || x.GetName().Name == y.GetName().Name;
            }

            public int GetHashCode(Assembly obj)
            {
                return obj.GetHashCode();
            }
        }
        /// <summary>
        /// Tries to do the MEF composition of all assemblies and preloaded objects.
        /// Throws a ModuleLoadErrorException if the composition fails
        /// </summary>
        /// <param name="preloadedObjects"></param>
        /// <param name="assembliesToLoad"></param>
        public void LoadModules(Dictionary<Type, object> preloadedObjects, IEnumerable<Assembly> assembliesToLoad)
        {
            string errorDescription = string.Empty;
            List<Assembly> distinctAssemblies = assembliesToLoad.Distinct(new AssemblyNameComparer())?.ToList();

            _container = new MyCompositionContainer(new AggregateCatalog(
                            distinctAssemblies
                            .Select(x => new AssemblyCatalog(x))
                            ));


            //Resulta que el método que nos interesa (ComposeExportedValue) para
            //cargar los objetos ya creados es genérico, y no podemos llamarlo directamente 
            //a partir de nuestro diccionario de type->object (no podemos hacer ComposeExportedValue<Type>...)
            //así que tenemos que hacer una voltereta usando Reflection para crear la llamada a 
            //método genérico que nos interesa (MakeGenericMethod(nuestroTipo) devuelve ComposeExportedValue<NuestroTipo>)
            MethodInfo genericMethod = typeof(AttributedModelServices).GetMember("ComposeExportedValue")?.FirstOrDefault() as MethodInfo;
            foreach (var pair in preloadedObjects)
            {
                var composeExportedValueMethod = genericMethod.MakeGenericMethod(pair.Key);
                composeExportedValueMethod.Invoke(_container, new object[] { _container, pair.Value });
            }


            _container.OnPartLoaded += OnPartLoaded;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                _container.ComposeParts();
            }
            catch (CompositionException ce)
            {
                ModuleLoadErrorException me = ModuleLoadErrorException.CreateFromCompositionException(ce);
                throw me;
            }
            catch (Exception ex)
            {
                errorDescription = "Error loading modules with MEF: " + ex.ToString();
                throw new ModuleLoadErrorException(errorDescription, ex);
            }
            sw.Stop();
            string log = "Module composition finished. " + sw.ElapsedMilliseconds + "ms";
            log += "\r\nParts loaded: " + string.Join(", ", _container.ParsedParts);
            Debug.WriteLine(log);
        }

        private void OnPartLoaded(string name)
        {
            if (ModuleLoaded != null)
                ModuleLoaded(this, new ModuleLoadedEventArgs(name));
        }

        public IEnumerable<Assembly> FindAssembliesWithTypes(string sFolderPath, string pattern, IEnumerable<Type> tiposACargar)
        {
            string[] folderFiles = System.IO.Directory.GetFiles(sFolderPath, pattern);
            List<Assembly> listaAssemblysToReturn = new List<Assembly>();

            foreach (string sFiledll in folderFiles)
            {
                try
                {
                    Assembly ass = Assembly.LoadFrom(sFiledll);
                    if (ass.GetTypes().Any(tipoEnDll => tiposACargar.Any(tipoACargar => tipoEnDll.IsAssignableFrom(tipoACargar))))
                    {
                        listaAssemblysToReturn.Add(ass);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is ReflectionTypeLoadException)
                    {
                        string.Join("\r\n", ((ReflectionTypeLoadException)ex).LoaderExceptions.Select(x => x.ToString()));
                    }
                }
            }

            return listaAssemblysToReturn;
        }
    }

    /// <summary>
    /// CompositionContainer que lanza un evento cada vez que 
    /// carga una "parte" MEF, para tener idea del progreso de la composición
    /// </summary>
    internal class MyCompositionContainer : CompositionContainer
    {
        public delegate void PartLoadedDlg(string name);
        public event PartLoadedDlg OnPartLoaded;

        public MyCompositionContainer(ComposablePartCatalog catalog, params ExportProvider[] providers)
            : base(catalog, providers)
        {
        }


        private List<string> _parsed = new List<string>();
        public List<string> ParsedParts { get { return _parsed; } }

        protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
        {
            if (!_parsed.Contains(definition.ContractName))
            {
                _parsed.Add(definition.ContractName);
                if (OnPartLoaded != null)
                {
                    OnPartLoaded(definition.ContractName);
                }
            }

            return base.GetExportsCore(definition, atomicComposition);
        }
    }
}
