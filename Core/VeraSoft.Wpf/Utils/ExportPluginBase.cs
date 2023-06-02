using System;
using VeraSoft.Wpf.Core;
using VeraSoft.Wpf.Core.Components;
using VeraSoft.Wpf.Managers;

namespace VeraSoft.Wpf.Utils
{
    /// <summary>
    /// Class LoadMenuItemsBase
    /// </summary>
    public abstract class ExportPluginBase
    {
        protected ExportPluginBase()
        {
            LoadPlugins();
        }

        public abstract void LoadPlugins();

        public RelayCommand MenuItemCommand(PluginItemBase menuItem, Func<ViewModel> pageCreator, bool showInMainframe)
        {
            return new RelayCommand(param => OnCommand(menuItem, pageCreator(), showInMainframe), null);
        }

        /// <summary>
        /// Assign this command to your MenuItemBase.Command
        /// </summary>
        /// <param name="menuItem">The menu item.</param>
        /// <param name="page">The page.</param>
        private void OnCommand(PluginItemBase menuItem, ViewModel page, bool showInMainframe)
        {
            page.Id = menuItem.Id;
            var pm = IoC.Get<IPageManager>();
            pm.OpenPageWindow(page, null, false, false, showInMainframe);
        }
    }
}
