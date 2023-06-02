using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using VeraSoft.Wpf.Mapping;
using VeraSoft.Wpf.Templates;

namespace VeraSoft.Wpf.Managers
{
    /// <summary>
    /// ViewsManager
    /// </summary>
    [Export(typeof(ViewsManager))]
    public class ViewsManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewsManager"/> class.
        /// </summary>
        /// <param name="views">The views.</param>
        [ImportingConstructor]
        public ViewsManager([ImportMany] IEnumerable<IVVMMappingBase> views, [Import] IPageManager pm)
        {
            _availableViews.AddRange(views);
        }

        /// <summary>
        /// The available views
        /// </summary>
        protected List<IVVMMappingBase> _availableViews = new List<IVVMMappingBase>();

        /// <summary>
        /// Gets the available views.
        /// </summary>
        /// <value>
        /// The available views.
        /// </value>
        public List<IVVMMappingBase> AvailableViews { get { return _availableViews; } }

        /// <summary>
        /// Loads the availiable views.
        /// </summary>
        public void LoadAvailiableViews()
        {
            AvailableViews.ForEach(v => RegisterView(v, false));

            // ahora me ahorro el [Export] de Pages
            foreach (var view in AvailableViews)
            {
                foreach (var mapping in view.Mappings)
                {
                    var usercontrol = typeof(UserControl);
                    var runtimetype = typeof(UserControl).GetType();
                    var contentcontrol = typeof(UserControl).BaseType;
                    var runtimeTypeHandle = typeof(UserControl).TypeHandle;
                    IoC.SetInstance(typeof(UserControl), mapping.View.Name);
                }
            }

            //var getview = IoC.Get<UserControl>("OrganizerView");
            MEFIoCManager.Instance.ComposeParts();
            //var getviewBien = IoC.Get<UserControl>("OrganizerView");
            //var getview = IoC.Get<object>("OrganizerView");
            //var getviewcasted = (IFakeExportable)getview;
            //var getviewcontrol = (UserControl)getview;
        }


        /// <summary>
        /// Registers the view.
        /// </summary>
        /// <param name="view">The view.</param>
        public void RegisterView(IVVMMappingBase view)
        {
            RegisterView(view, true);
        }

        /// <summary>
        /// Registers the view.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="view">The view.</param>
        public void RegisterView(Type viewModel, Type view)
        {
            ViewViewModelMappingBase bv = new ViewViewModelMappingBase(viewModel, view);
            RegisterView(bv);
        }

        private void RegisterView(IVVMMappingBase view, bool addToAvailable)
        {
            if (view == null)
            {
                return;
            }

            foreach (var mapping in view.Mappings)
            {
                DataTemplate dt = DataTemplateCreator.CreateTemplateForType(mapping.ViewModel, mapping.View);
                ResourceDictionary currentResources = Application.Current.Resources;
                object key = dt.DataTemplateKey;
                if (!currentResources.Contains(key))
                    currentResources.Add(key, dt);
                else
                    currentResources[key] = dt;
            }

            if (addToAvailable)
                AvailableViews.Add(view);
        }
    }
}
