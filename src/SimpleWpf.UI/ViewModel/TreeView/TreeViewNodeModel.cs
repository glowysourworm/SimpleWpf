namespace SimpleWpf.UI.ViewModel.TreeView
{
    public class TreeViewNodeModel : TreeViewNodeModelBase
    {
        public TreeViewNodeModel(int recursionDepth) : base(recursionDepth)
        {

        }

        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}
