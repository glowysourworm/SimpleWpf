using SimpleWpf.UI.ViewModel.TreeView;

namespace SimpleWpf.UI.ViewModel.FileTreeView
{
    public class FileTreeViewModel : TreeViewModelBase<FileTreeNodeViewModel>
    {
        private readonly string _searchPattern;

        public string SearchPattern
        {
            get { return _searchPattern; }
        }

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

        public IEnumerable<FileTreeViewModel> GetSelection(bool includeDirectories)
        {
            var result = new List<FileTreeViewModel>();

            // current sub-tree
            RecurseForEach(subTree =>
            {
                if (subTree.NodeValue.IsSelected)
                {
                    // File
                    if (!subTree.NodeValue.IsDirectory)
                        result.Add(subTree as FileTreeViewModel);

                    // Directory
                    else if (includeDirectories)
                        result.Add(subTree as FileTreeViewModel);
                }
            });

            return result;
        }

        public override string ToString()
        {
            return this.NodeValue.ToString();
        }
    }
}
