using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace VeraSoft.Wpf.Mapping
{
    /// <summary>
    /// Base class used to register in the global application dictionary a mapping between 
    /// a specific viewmodel (not the interface it implements) and a view. 
    /// </summary>
    [Export(typeof(IVVMMappingBase))]
    public class ViewViewModelMappingBase : IVVMMappingBase
    {
        //public ResourceDictionary DataTemplates {  get { return this; } }
        private List<VVMMappingModel> _mappings = new List<VVMMappingModel>();
        public List<VVMMappingModel> Mappings { get { return _mappings; } }

        public ViewViewModelMappingBase()
        { }

        //public BasicViewMapping(DataTemplate dt)
        //{
        //    this.Add(dt.DataTemplateKey, dt);
        //}
        public ViewViewModelMappingBase(VVMMappingModel vm)
        {
            _mappings.Add(vm);
        }

        public ViewViewModelMappingBase(Type viewModel, Type view)
        {
            AddMapping(viewModel, view);
        }

        public void AddMapping(Type viewModel, Type view)
        {
            _mappings.Add(new VVMMappingModel(viewModel, view));
            //DataTemplate dt = DataTemplateCreator.CreateTemplateForType(viewModel, view);
            //if (dt != null)
            //    this.Add(dt.DataTemplateKey, dt);
        }
    }

    public class ViewViewModelMappingBase<ViewModel, View> : ViewViewModelMappingBase
    {
        public ViewViewModelMappingBase() : base(typeof(ViewModel), typeof(View))
        { }
    }
}
