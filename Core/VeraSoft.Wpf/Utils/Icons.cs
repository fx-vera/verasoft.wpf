using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace VeraSoft.Wpf.Utils
{
    public class Icons
    {
        /// <summary>
        /// Gets the icon from resource dictionary.
        /// </summary>
        /// <param name="resourceDictionaryEnum">The resource dictionary enum.</param>
        /// <param name="iconName">Name of the icon.</param>
        /// <returns></returns>
        public static ImageSource GetIconFromResourceDictionary(string iconName)
        {
            ImageSource icon = null;
            try
            {
                if ((icon = Application.Current.Resources[iconName] as ImageSource) == null)
                {
                    ResourceDictionary rd = Application.Current.Resources.MergedDictionaries
                                            .FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.Source.AbsolutePath).Equals("Icons"));
                    if (rd != null)
                        icon = rd[iconName] as ImageSource;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error getting icon " + iconName + " from resources: " + ex.ToString());
            }

            return icon;
        }
    }

    /// <summary>
    /// Icon names
    /// </summary>
    public static class IconNames
    {
        public const string camera = "camera";
        public const string folder = "folder";
        public const string notification = "notification";
    }
}
