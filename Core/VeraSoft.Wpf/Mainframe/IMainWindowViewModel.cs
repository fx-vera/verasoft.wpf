using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using VeraSoft.Wpf.Core;
using VeraSoft.Wpf.Events;
using VeraSoft.Wpf.Extensions;

namespace VeraSoft.Wpf.Mainframe
{
    public interface IMainWindowViewModel : INotifyPropertyChanged
    {
        event EventHandler<WindowStateEventArgs> WindowStateChanged;

        string Title { get; }
        ImageSource Icon { get; }
        WindowStyle WindowStyle { get; }

        bool IsResizable { get; }

        bool IsCollapsed { get; set; }

        Size InitialSize { get; set; }

        void OnMainWindowClosing();

        void OnMainWindowClosed();

        object EditorSettings { get; set; }
        string Id { get; set; }

        double InitialTop { get; set; }
        double InitialLeft { get; set; }
        IViewModel ViewModel { get; set; }
        ObservableCollection<FlyoutBaseViewModel> FlyoutViewModels { get; set; }

        void LoadFlyouts();
        void SetSelectedPlugin();
    }
}