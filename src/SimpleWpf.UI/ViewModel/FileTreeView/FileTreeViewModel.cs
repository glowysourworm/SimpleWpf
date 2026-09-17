using SimpleWpf.UI.ViewModel.TreeView;
using SimpleWpf.UI.ViewModel.TreeView.Interface;

namespace SimpleWpf.UI.ViewModel.FileTreeView
{
    public class FileTreeViewModel : TreeViewModelBase
    {
        /// <summary>
        /// Returns node value casted up to the file tree view model.
        /// </summary>
        public FileTreeNodeViewModel GetNodeValue()
        {
            return this.NodeValue as FileTreeNodeViewModel;
        }

        public FileTreeViewModel(FileTreeNodeViewModel nodeValue,
                                 TreeViewModelBase parent = null)
            : base(nodeValue, parent)
        {
        }

        protected override TreeViewModelBase Construct(ITreeViewNode nodeValue)
        {
            return new FileTreeViewModel(nodeValue as FileTreeNodeViewModel, this);
        }

        public int GetSelectedFileCount()
        {
            // Selected File Count
            return RecursiveCount(node => node.IsSelected && !node.CanHaveChildren);
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
