namespace SimpleWpf.UI.ViewModel.TreeView
{
    public class TreeViewModel : TreeViewModelBase<TreeViewNodeModel>
    {
        public TreeViewModel(TreeViewNodeModel nodeValue,
                             TreeViewModel parent = null)
            : base(nodeValue, parent)
        {
        }

        protected override TreeViewModel Construct(TreeViewNodeModel nodeValue)
        {
            return new TreeViewModel(nodeValue, this);
        }

        public override string ToString()
        {
            return this.NodeValue?.ToString();
        }
    }
}
