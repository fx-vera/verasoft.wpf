using System;
using VeraSoft.Wpf.Enums;

namespace VeraSoft.Wpf.Events
{
    public class WindowStateEventArgs : EventArgs
    {
        public PageWindowState State { get; set; }
    }
}
