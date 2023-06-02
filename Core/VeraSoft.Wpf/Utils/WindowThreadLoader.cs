using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace VeraSoft.Wpf.Utils
{
    /// <summary>
    /// Lanza una ventana en un hilo de mensajes aparte.
    /// <p/>Ver http://reedcopsey.com/2011/11/28/launching-a-wpf-window-in-a-separate-thread-part-1/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WindowThreadLoader<T> where T : Window, new()
    {
        public event EventHandler WindowCreated;

        private T _window;
        public T Window { get { return _window; } }

        private Thread _windowThread = null;

        public WindowThreadLoader()
        { }

        public void Show()
        {
            if (_window != null || _windowThread != null)
                return;

            /*Thread*/
            _windowThread = new Thread(new ThreadStart(LaunchWindow));
            _windowThread.SetApartmentState(ApartmentState.STA);
            _windowThread.IsBackground = true;
            _windowThread.Start();
        }

        public void Close()
        {
            if (_window != null)
            {
                if (_window.Dispatcher.CheckAccess())
                    _window.Close();
                else
                    _window.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(_window.Close));
            }
            else if (_windowThread != null)
            {
                _windowThread.Abort();
                _windowThread = null;
            }
        }

        private void LaunchWindow()
        {
            // Create our context, and install it:
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

            _window = new T();

            _window.Closed += (s, e) => { Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background); };
            _window.Show();
            if (WindowCreated != null)
                WindowCreated(_window, null);

            try
            {
                Dispatcher.Run();
            }
            catch (ThreadAbortException)
            {
                _window.Close();
                Dispatcher.CurrentDispatcher.InvokeShutdown();
            }

            _window = null;
            _windowThread = null;
        }

    }
}
