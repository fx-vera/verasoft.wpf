using PropertyChanged;
using System;
using System.ComponentModel.Composition;
using VeraSoft.Wpf.Core;
using VeraSoft.Wpf.Core.Components;
using VeraSoft.Wpf.Mapping;
using VeraSoft.Wpf.Utils;

namespace VeraSoft.Plugins.NotificationManager
{
    [AddINotifyPropertyChangedInterface]
    public class NotificationManagerViewModel: ViewModel
    {
        public override void Load(object objectToEdit)
        {
        }

        public override void Save()
        {
        }
    }

    public class ExportPlugin : ExportPluginBase
    {
        public override void LoadPlugins()
        {
            Notifications = new PluginItemBase();
            Notifications.Id = "{D95D70EA-465F-4FC3-8233-5AB7431B8E31}";
            Notifications.Name = CurrentDictionary.Notifications;
            Notifications.Icon = Icons.GetIconFromResourceDictionary(IconNames.notification);
            Notifications.Command = this.MenuItemCommand(Notifications, new Func<ViewModel>(() => { return new NotificationManagerViewModel(); }), true);
        }

        [Export(typeof(IPluginItem))]
        public PluginItemBase Notifications { get; set; }
    }

    [Export(typeof(IVVMMappingBase))]
    public class OrganizerViewMapping : ViewViewModelMappingBase<NotificationManagerViewModel, UserControl1> { }
}
