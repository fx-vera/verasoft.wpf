using ControlzEx.Theming;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Windows;
using VeraSoft.Wpf.Defaults;
using VeraSoft.Wpf.Events;
using VeraSoft.Wpf.Managers;
using VeraSoft.Wpf.Utils;

namespace VeraSoft.Wpf.Core
{
    [Export(typeof(IWorkspace))]
    public class WorkspaceBase : IWorkspace
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceBase"/> class.
        /// </summary>
        /// <param name="pm">The pm.</param>
        /// <param name="ea">The ea.</param>
        [ImportingConstructor]
        public WorkspaceBase(IPageManager pm, IEventAggregator ea)
        {
            ProfileSettings = new ProfileSettings();
        }

        public ProfileSettings ProfileSettings { get; set; }
        public Window MainWindow { get; set; }

        public virtual void LoadProfile()
        {
            ProfileSettings profileSettings = new ProfileSettings();
            string winUser = string.Empty;

            var currentWindowsUser = WindowsIdentity.GetCurrent();
            if (currentWindowsUser.Name != null)
                winUser = currentWindowsUser.Name.Split('\\')[1].ToLower();

            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ProfileSettings.xml");

            if (!File.Exists(configPath))
            {
                profileSettings.OwnerName = winUser;
                profileSettings.Language = System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.CultureName;
                profileSettings.LanguageNative = System.Threading.Thread.CurrentThread.CurrentUICulture.NativeName;
                profileSettings.ThemeColorScheme = "Taupe";
                profileSettings.ThemeBaseColorScheme = "Light";
                XmlTools.Serialize(configPath, profileSettings);
            }
            else
            {
                profileSettings = XmlTools.Deserialize<ProfileSettings>(configPath);
                CultureInfo currentCulture = CultureInfo.CreateSpecificCulture(profileSettings.Language);
                System.Threading.Thread.CurrentThread.CurrentUICulture = currentCulture;
                System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
            }
            profileSettings.IsEnglish = profileSettings.Language == "en-GB";
            ProfileSettings = profileSettings;

            ThemeManager.Current.ChangeThemeColorScheme(Application.Current, profileSettings.ThemeColorScheme);
            ThemeManager.Current.ChangeThemeBaseColor(Application.Current, profileSettings.ThemeBaseColorScheme);
        }
    }
}
