namespace VeraSoft.Wpf.Enums
{
    /// <summary>
    /// Window state: the three first states are a copy of the WindowState enumeration. We add a "Hidden" state in case we 
    /// need to keep the window alive but closed (hidden)
    /// </summary>
    public enum PageWindowState

    {
        //     Window is open in normal state
        Normal = 0,
        //     Window is open but minimized
        Minimized = 1,
        //     Window is open and maximized
        Maximized = 2,
        //     Requests to close the window 
        Closed = 3,
        //Window is alive but not shown
        Hidden = 4
    };
}
