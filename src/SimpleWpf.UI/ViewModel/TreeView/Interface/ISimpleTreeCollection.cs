using System.Collections;

namespace SimpleWpf.UI.ViewModel.TreeView.Interface
{
    public interface ISimpleTreeCollection : ISimpleUpdater, ISimpleValidator, IEnumerable
    {
        public int MaxDepth { get; }
        public IEnumerable<TreeViewNodeModelBase> GetBranch(TreeViewNodeModelBase node, bool includeDescendants = false);
        public void SimpleTreeAdd(TreeViewNodeModelBase node);
        public T? SimpleTreeGetNode<T>(object key) where T : TreeViewNodeModelBase;
        public T? SimpleTreeGetNode<T>(int recursionDepth, object key) where T : TreeViewNodeModelBase;
        public void SimpleTreeRemove<T>(T node) where T : TreeViewNodeModelBase;
        public bool SimpleTreeContains<T>(T node) where T : TreeViewNodeModelBase;
        public bool SimpleTreeAll<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase;
        public bool SimpleTreeAny<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase;
        public int SimpleTreeCount<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase;
        public T? SimpleTreeFirst<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase;
        public IEnumerable<T> SimpleTreeWhere<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase;
        public void SimpleTreeForEach<T>(Action<T> action) where T : TreeViewNodeModelBase;
        public void SimpleTreeClear();
    }
}
