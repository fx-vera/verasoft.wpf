using System;
using VeraSoft.Wpf.Core;

namespace VeraSoft.Wpf.Managers
{
    /// <summary>
    /// Interface to be exported if some module wants to create a specific type of
    /// window different from the usual one for a specific page type
    /// </summary>
    public interface IPageWindowCreator
    {
        Type PageType { get; }
        IWindowViewModel CreateWindow();
    }
}
