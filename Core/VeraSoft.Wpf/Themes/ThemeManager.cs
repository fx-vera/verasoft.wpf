using System;
using System.ComponentModel.Composition;
using System.Windows;

namespace VeraSoft.Wpf.Themes
{
    [Export(typeof(IThemeManager))]
    public class ThemeManager : IThemeManager
    {
        private readonly ResourceDictionary themeResources;

        public ThemeManager()
        {
            this.themeResources = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/VeraSoft.Wpf;component/Themes/Themes.xaml")
            };
        }

        public ResourceDictionary GetThemeResources()
        {
            return this.themeResources;
        }
    }
}
