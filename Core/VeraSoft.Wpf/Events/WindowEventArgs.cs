using System;
using VeraSoft.Wpf.Core;

namespace VeraSoft.Wpf.Events
{
    public class WindowEventArgs : EventArgs
    {
        public IWindowViewModel Window { get; set; }
    }
}
