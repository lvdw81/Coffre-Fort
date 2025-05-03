using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace coffre_fort2.ViewModels
{
    public class RelayCommandAsync : ICommand
    {
        private readonly Func<object, Task> _executeAsync;
        private readonly Predicate<object> _canExecute;
        private bool _isExecuting;

        public RelayCommandAsync(Func<object, Task> executeAsync, Predicate<object> canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && (_canExecute == null || _canExecute(parameter));
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public async Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter))
                return;

            _isExecuting = true;
            RaiseCanExecuteChanged();

            try
            {
                await _executeAsync(parameter);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        private void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
