using System;
using System.ComponentModel;
using System.Windows;
using VeraSoft.Wpf.Enums;
using VeraSoft.Wpf.Events;

namespace VeraSoft.Wpf.Core
{
    public interface IWindowViewModel : ICloneable, INotifyPropertyChanged
    {
        event EventHandler<WindowStateEventArgs> WindowStateChanged;
        event EventHandler<WindowEventArgs> WindowCloned;

        IViewModel ViewModel { get; set; }

        Window View { get; }

        PageWindowState State { get; }

        void Minimize();
        void Restore();

        void SetIsPinned(bool isPinned);

        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();

        void SetEnabled(bool isEnabled);
    }
}
