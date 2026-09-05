using SimpleWpf.UI.ViewModel.TreeView;
using SimpleWpf.UI.ViewModel.TreeView.Interface;

namespace SimpleWpf.UI.ViewModel.FileTreeView
{
    public class FileTreeViewModel : TreeViewModelBase
    {
        private readonly string _searchPattern;

        public string SearchPattern
        {
            get { return _searchPattern; }
        }

        /// <summary>
        /// Returns node value casted up to the file tree view model.
        /// </summary>
        public FileTreeNodeViewModel GetNodeValue()
        {
            return this.NodeValue as FileTreeNodeViewModel;
        }

        public FileTreeViewModel(string searchPattern,
                                 FileTreeNodeViewModel nodeValue,
                                 TreeViewModelBase parent = null)
            : base(nodeValue, parent)
        {
            _searchPattern = searchPattern;
        }

        protected override TreeViewModelBase Construct(ITreeViewNode nodeValue)
        {
            return new FileTreeViewModel(_searchPattern, nodeValue as FileTreeNodeViewModel, this);
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
                    if (!subTree.NodeValue.CanHaveChildren)
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
