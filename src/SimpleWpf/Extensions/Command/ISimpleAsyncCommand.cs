using System.Windows.Input;

namespace SimpleWpf.Extensions.Command
{
    public interface ISimpleAsyncCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
