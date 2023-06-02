using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VeraSoft.Wpf.Utils
{
    public class RelayCommandAsync : ICommand, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Events
        /// <summary>
        /// This implementation ensures that, if somebody hookt to this CanExecuteChanged event, the CommandManager
        /// will notify them on its RequerySuggested, which is triggered "the CommandManager detects conditions that 
        /// might change the ability of a command to execute.", which is mainly on any input event in the application (mouse click, focus change). 
        /// As another benefit, CommandManager.RequerySuggested uses weak eventing and does not need to be unregistered.
        /// In order to raise the CanExecuteChanged manually, call CommandManager.InvalidateRequerySuggested
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion

        #region Fields

        private readonly Func<object, Task> _execute;
        private readonly Func<object, bool> _canExecute;

        private bool _isExecuting = false;
        public bool IsExecuting { get { return _isExecuting; } }

        #endregion // Fields

        #region Constructors

        public RelayCommandAsync(Func<object, Task> execute) : this(execute, null) { }

        public RelayCommandAsync(Func<object, Task> execute, Func<object, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("Execute method is null");
            _canExecute = canExecute;
        }
        #endregion // Constructors

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
        }

        public async void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    _isExecuting = true;
                    await _execute(parameter);
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        #endregion // ICommand Members

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
            //CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

}
