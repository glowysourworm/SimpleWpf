namespace SimpleWpf.UI.ViewModel.TreeView
{
    public class TreeViewModel : TreeViewModelBase
    {
        string _displayName;

        public string DisplayName
        {
            get { return _displayName; }
            set { this.RaiseAndSetIfChanged(ref _displayName, value); }
        }

        public TreeViewModel(string displayName, int recursionDepth, TreeViewModelBase? parent) : base(recursionDepth, parent)
        {
            _displayName = displayName;
        }

        public override string ToString()
        {
            return _displayName;
        }
    }
}
