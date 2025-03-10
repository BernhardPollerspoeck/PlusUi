using System.ComponentModel;
using System.Windows.Input;

namespace PlusUi.core;

public class AsyncCommand : ICommand
{
    readonly Func<object?, Task> _execute;
    readonly Func<object?, bool> _canExecute;

    /// <summary>
    /// Use this constructor for commands that have a command parameter.
    /// </summary>
    /// <param name="execute">The asynchronous function to execute</param>
    /// <param name="canExecute">Function determining if command can execute</param>
    /// <param name="notificationSource">Optional property change notification source</param>
    public AsyncCommand(Func<object?, Task> execute, Func<object?, bool>? canExecute = null, INotifyPropertyChanged? notificationSource = null)
    {
        _execute = execute;
        _canExecute = canExecute ?? (_ => true);
        if (notificationSource != null)
        {
            notificationSource.PropertyChanged += (s, e) => RaiseCanExecuteChanged();
        }
    }

    /// <summary>
    /// Use this constructor for commands that don't have a command parameter.
    /// </summary>
    public AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null, INotifyPropertyChanged? notificationSource = null)
        : this(_ => execute.Invoke(), _ => (canExecute ?? (() => true)).Invoke(), notificationSource)
    {
    }

    public bool CanExecute(object? param = null) => _canExecute.Invoke(param);

    public async void Execute(object? param = null)
    {
        if (!CanExecute(param))
        {
            return;
        }
        await _execute.Invoke(param);
    }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
