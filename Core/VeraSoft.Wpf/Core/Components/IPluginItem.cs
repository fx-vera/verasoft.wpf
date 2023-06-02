using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace VeraSoft.Wpf.Core.Components
{
    /// <summary>
    /// interface IPluginItem
    /// </summary>
    public interface IPluginItem : INotifyPropertyChanged
    {
        string Id { get; set; }

        string Name { get; set; }

        ImageSource Icon { get; set; }

        ICommand Command { get; set; }
    }
}
