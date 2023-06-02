using System.Collections.Generic;

namespace VeraSoft.Wpf.Mapping
{
    /// <summary>
    /// Interface to be exported through MEF to register 
    /// the Views to be assigned to ViewModels
    /// </summary>
    public interface IVVMMappingBase
    {
        //ResourceDictionary DataTemplates { get; }
        List<VVMMappingModel> Mappings { get; }
    }
}
