namespace SimpleWpf.UI.ViewModel.TreeView
{
    public class TreeViewNodeModel : TreeViewNodeModelBase
    {
        string _displayName;

        public string DisplayName
        {
            get { return _displayName; }
            set { this.RaiseAndSetIfChanged(ref _displayName, value); }
        }

        public TreeViewNodeModel(string displayName, int recursionDepth, TreeViewNodeModelBase? parent) : base(recursionDepth, parent)
        {
            _displayName = displayName;
        }

        public override string ToString()
        {
            return _displayName;
        }
    }
}
