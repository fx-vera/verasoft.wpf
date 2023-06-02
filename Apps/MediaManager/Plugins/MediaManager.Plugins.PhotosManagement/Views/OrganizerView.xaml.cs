using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace VeraSoft.Plugins.PhotosManagement.Views
{
    /// <summary>
    /// Lógica de interacción para OrganizerView.xaml
    /// </summary>
    [Export("OrganizerView", typeof(UserControl))]
    public partial class OrganizerView : UserControl
    {
        public OrganizerView()
        {
            InitializeComponent();
        }
    }
}
