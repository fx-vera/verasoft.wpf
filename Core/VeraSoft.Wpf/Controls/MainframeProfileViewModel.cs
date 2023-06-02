using ControlzEx.Theming;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using VeraSoft.Wpf.Core;
using VeraSoft.Wpf.Defaults;
using VeraSoft.Wpf.Extensions;
using VeraSoft.Wpf.Mapping;
using VeraSoft.Wpf.Utils;

namespace VeraSoft.Wpf.Controls
{
    [Export(DefaultPages.MainframeProfile, typeof(IViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [AddINotifyPropertyChangedInterface]
    public class MainframeProfileViewModel : FlyoutBaseViewModel
    {
        private bool isLoading = true;
        private bool themeChanged = false;

        public MainframeProfileViewModel()
        {
            SaveCommand = new RelayCommand(o => PerformApplyCommand(o));
            Id = DefaultId.UserProfile;
            IsSpanish = !this.Workspace.ProfileSettings.IsEnglish;
            IsEnglish = this.Workspace.ProfileSettings.IsEnglish;
            Header = CurrentDictionary.Profile;
            SelectLanguageString = CurrentDictionary.SelectLanguage;
            SelectTheme = CurrentDictionary.SelectTheme; 
            CanHaveMultipleInstances = false;
            this.Themes = new ObservableCollection<ThemeNames>();
            this.Themes.AddRange(ThemeManager.Current.Themes
                                            .Select(a => new ThemeNames() { Name = a.ColorScheme, BaseName = a.BaseColorScheme })
                                            ?.ToList()?.OrderByDescending(x => x.FullName));
            isLoading = false;

            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ProfileSettings.xml");
            this.Workspace.ProfileSettings = XmlTools.Deserialize<ProfileSettings>(configPath);
            CurrentTheme = Themes.Where(x => x.Name.Equals(this.Workspace.ProfileSettings.ThemeColorScheme) && x.BaseName.Equals(this.Workspace.ProfileSettings.ThemeBaseColorScheme))?.FirstOrDefault();
            OriginalTheme = CurrentTheme;
        }

        public string SelectLanguageString { get; set; }
        
        public string SelectTheme { get; set; }
        public bool IsEnglish { get; set; }
        public bool IsSpanish { get; set; }
        public ObservableCollection<ThemeNames> Themes { get; set; }
        public ThemeNames CurrentTheme { get; set; }
        public ThemeNames OriginalTheme { get; set; }

        public ICommand SaveCommand { get; set; }
        private void PerformApplyCommand(object o)
        {
            if (IsEnglish)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
            }
            else if (IsSpanish)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
            }

            this.Workspace.ProfileSettings.IsEnglish = IsEnglish;
            this.Workspace.ProfileSettings.Language = Thread.CurrentThread.CurrentCulture.Name;
            this.Workspace.ProfileSettings.LanguageNative = Thread.CurrentThread.CurrentCulture.NativeName;

            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ProfileSettings.xml");
            XmlTools.Serialize(configPath, this.Workspace.ProfileSettings);
            themeChanged = true;
            MessageBox.Show(CurrentDictionary.ChangesSaved, CurrentDictionary.ChangesSaved, MessageBoxButton.OK);
        }

        public override void Load(object objectToEdit)
        {
        }

        public override void Save()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            ThemeNames theme = OriginalTheme;
            if (themeChanged)
            {
                theme = CurrentTheme;
            }
            ThemeManager.Current.ChangeThemeColorScheme(Application.Current, theme.Name);
            ThemeManager.Current.ChangeThemeBaseColor(Application.Current, theme.BaseName);
            this.Workspace.ProfileSettings.ThemeColorScheme = theme.Name;
            this.Workspace.ProfileSettings.ThemeBaseColorScheme = theme.BaseName;

            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ProfileSettings.xml");
            XmlTools.Serialize(configPath, this.Workspace.ProfileSettings);
        }

        private void OnCurrentThemeChanged()
        {
            if (!isLoading)
            {
                ThemeManager.Current.ChangeThemeColorScheme(Application.Current, CurrentTheme.Name);
                ThemeManager.Current.ChangeThemeBaseColor(Application.Current, CurrentTheme.BaseName);
                this.Workspace.ProfileSettings.ThemeColorScheme = CurrentTheme.Name;
                this.Workspace.ProfileSettings.ThemeBaseColorScheme = CurrentTheme.BaseName;

                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ProfileSettings.xml");
                XmlTools.Serialize(configPath, this.Workspace.ProfileSettings);
            }
        }

        private void OnIsSpanishChanged()
        {
            if (isLoading)
                return;
            if (IsEnglish)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
            }
            else if (IsSpanish)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
            }

            this.Workspace.ProfileSettings.IsEnglish = IsEnglish;
            this.Workspace.ProfileSettings.Language = Thread.CurrentThread.CurrentCulture.Name;
            this.Workspace.ProfileSettings.LanguageNative = Thread.CurrentThread.CurrentCulture.NativeName;

            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ProfileSettings.xml");
            XmlTools.Serialize(configPath, this.Workspace.ProfileSettings);
            themeChanged = true;
            Header = CurrentDictionary.Profile;
            SelectLanguageString = CurrentDictionary.SelectLanguage;
            SelectTheme = CurrentDictionary.SelectTheme;
            this.Workspace.MainWindow.UpdateLayout();
            this.Workspace.MainWindow.UpdateDefaultStyle();
        }

        private void OnIsEnglishChanged()
        {
            if (isLoading)
                return;
            if (IsEnglish)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
            }
            else if (IsSpanish)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
            }
            Resources.Properties.Resources.Culture = Thread.CurrentThread.CurrentCulture;
            this.Workspace.ProfileSettings.IsEnglish = IsEnglish;
            this.Workspace.ProfileSettings.Language = Thread.CurrentThread.CurrentCulture.Name;
            this.Workspace.ProfileSettings.LanguageNative = Thread.CurrentThread.CurrentCulture.NativeName;

            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ProfileSettings.xml");
            XmlTools.Serialize(configPath, this.Workspace.ProfileSettings);
            themeChanged = true;
            Header = CurrentDictionary.Profile;

            SelectLanguageString = CurrentDictionary.SelectLanguage;
            SelectTheme = CurrentDictionary.SelectTheme;
            this.Workspace.MainWindow.UpdateLayout();
            this.Workspace.MainWindow.UpdateDefaultStyle();
        }
    }

    [Export(typeof(IVVMMappingBase))]
    public class MainframeProfileViewMapping : ViewViewModelMappingBase<MainframeProfileViewModel, MainframeProfileView> { }

    [AddINotifyPropertyChangedInterface]
    public class ThemeNames : System.ComponentModel.INotifyPropertyChanged
    {
        public ThemeNames()
        {
            PerformChangeTheme = new RelayCommand(o => DoChangeTheme(o));
        }

        public string FullName { get { return BaseName + " " + Name; } }
        public string Name { get; set; }
        public string BaseName { get; set; }

        public ICommand PerformChangeTheme { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void DoChangeTheme(object sender)
        {
            ThemeManager.Current.ChangeThemeColorScheme(Application.Current, this.Name);
            ThemeManager.Current.ChangeThemeBaseColor(Application.Current, this.Name);

        }
    }
}
