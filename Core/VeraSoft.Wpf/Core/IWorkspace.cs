using System.Windows;
using VeraSoft.Wpf.Defaults;

namespace VeraSoft.Wpf.Core
{
    /// <summary>
    /// Basic "Workspace": a class that groups the main components 
    /// of the application framework: the Page Manager, the Event Aggregator, etc
    /// </summary>
    public interface IWorkspace
    {
        ProfileSettings ProfileSettings { get; set; }
        /// <summary>
        /// Initializes this instance.
        /// </summary>

        void LoadProfile();

        Window MainWindow { get; set; }
    }
}
