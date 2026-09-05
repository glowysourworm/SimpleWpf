namespace SimpleWpf.UI.ViewModel.TreeView
{
    public class TreeViewNodeModel : TreeViewNodeModelBase
    {
        public TreeViewNodeModel(string displayName, int recursionDepth) : base(displayName, recursionDepth)
        {

        }

        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}
