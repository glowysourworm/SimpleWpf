using System.ComponentModel;

namespace SimpleWpf.UI.ViewModel.TreeView.Interface
{
    public interface ITreeViewNode : INotifyPropertyChanged
    {
        public bool IsLoaded { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }
        public bool CanHaveChildren { get; set; }
        public int RecursionDepth { get; set; }
        public string DisplayName { get; set; }
    }
}
