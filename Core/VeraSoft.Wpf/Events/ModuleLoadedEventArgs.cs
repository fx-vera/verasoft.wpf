using System;

namespace VeraSoft.Wpf.Events
{
    public class ModuleLoadedEventArgs : EventArgs
    {
        public string ModuleName { get; set; }
        public ModuleLoadedEventArgs(string name) { ModuleName = name; }
    }
}
