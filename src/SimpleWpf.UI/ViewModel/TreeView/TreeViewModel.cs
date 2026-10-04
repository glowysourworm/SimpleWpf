namespace SimpleWpf.UI.ViewModel.TreeView
{
    public class TreeViewModel : TreeViewModelBase
    {


        bool _canHaveChildren;

        int _recursionDepth;

        string _displayName;

        public bool CanHaveChildren
        {
            get { return _canHaveChildren; }
            set { this.RaiseAndSetIfChanged(ref _canHaveChildren, value); }
        }
        public int RecursionDepth
        {
            get { return _recursionDepth; }
            set { this.RaiseAndSetIfChanged(ref _recursionDepth, value); }
        }
        public string DisplayName
        {
            get { return _displayName; }
            set { this.RaiseAndSetIfChanged(ref _displayName, value); }
        }

        public TreeViewModel(string displayName, int recursionDepth, TreeViewModel? parent) : base(parent)
        {
            this.DisplayName = displayName;
            this.RecursionDepth = recursionDepth;
            this.CanHaveChildren = true;
        }

        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}
