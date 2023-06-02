using System;
using System.ComponentModel.Composition;
using VeraSoft.Plugins.PhotosManagement.ViewModels;
using VeraSoft.Plugins.PhotosManagement.Views;
using VeraSoft.Wpf.Core;
using VeraSoft.Wpf.Core.Components;
using VeraSoft.Wpf.Mapping;
using VeraSoft.Wpf.Utils;

namespace VeraSoft.Plugins.PhotosManagement
{
    public class ExportPlugin : ExportPluginBase
    {
        public override void LoadPlugins()
        {
            PhotosManagement = new PluginItemBase();
            PhotosManagement.Id = "{28DE60F3-4E39-4A64-9D8E-F8243ADF05E8}";
            PhotosManagement.Name = CurrentDictionary.Organizer;
            PhotosManagement.Icon = Icons.GetIconFromResourceDictionary(IconNames.camera);
            PhotosManagement.Command = this.MenuItemCommand(PhotosManagement, new Func<ViewModel>(() => { return new OrganizerViewModel(); }), true);
        }

        [Export(typeof(IPluginItem))]
        public PluginItemBase PhotosManagement { get; set; }
    }

    [Export(typeof(IVVMMappingBase))]
    public class OrganizerViewMapping : ViewViewModelMappingBase<OrganizerViewModel, OrganizerView> { }
}
