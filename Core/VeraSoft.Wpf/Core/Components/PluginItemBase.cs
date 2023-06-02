using PropertyChanged;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace VeraSoft.Wpf.Core.Components
{
    [AddINotifyPropertyChangedInterface]
    public class PluginItemBase : IPluginItem
    {
        public PluginItemBase()
        {
        }

        public PluginItemBase(string id, string name, ImageSource icon, ICommand command)
        {
            Id = id;
            Name = name;
            Icon = icon;
            Command = command;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public ImageSource Icon { get; set; }

        public ICommand Command { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

}
