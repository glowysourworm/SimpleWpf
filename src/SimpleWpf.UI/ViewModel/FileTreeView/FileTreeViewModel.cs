using SimpleWpf.UI.ViewModel.TreeView;

namespace SimpleWpf.UI.ViewModel.FileTreeView
{
    public class FileTreeViewModel : TreeViewModelBase<FileTreeNodeViewModel>
    {
        private readonly string _searchPattern;

        public FileTreeViewModel(string searchPattern,
                                 FileTreeNodeViewModel nodeValue,
                                 TreeViewModelBase<FileTreeNodeViewModel> parent = null)
            : base(nodeValue, parent)
        {
            _searchPattern = searchPattern;
        }

        protected override TreeViewModelBase<FileTreeNodeViewModel> Construct(FileTreeNodeViewModel nodeValue)
        {
            return new FileTreeViewModel(_searchPattern, nodeValue, this);
        }

        public override string ToString()
        {
            return this.NodeValue.ToString();
        }
    }
}
