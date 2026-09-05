using SimpleWpf.UI.ViewModel.TreeView.Interface;

namespace SimpleWpf.UI.ViewModel.TreeView
{
    public class TreeViewModel : TreeViewModelBase
    {
        public TreeViewModel(TreeViewNodeModel nodeValue,
                             TreeViewModel parent = null)
            : base(nodeValue, parent)
        {
        }

        protected override TreeViewModel Construct(ITreeViewNode nodeValue)
        {
            return new TreeViewModel(nodeValue as TreeViewNodeModel, this);
        }

        public override string ToString()
        {
            return this.NodeValue?.ToString();
        }
    }
}
