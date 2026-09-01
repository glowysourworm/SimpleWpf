using System.Windows.Input;

namespace SimpleWpf.UI.Command
{
    public interface ISimpleAsyncCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
