using System;

namespace VeraSoft.Wpf.Mapping
{
    public class VVMMappingModel
    {
        public Type View { get; set; }
        public Type ViewModel { get; set; }

        public VVMMappingModel()
        { }

        public VVMMappingModel(Type viewModel, Type view)
        { View = view; ViewModel = viewModel; }
    }
}