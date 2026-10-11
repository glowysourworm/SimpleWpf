using System.Collections.Specialized;

using static SimpleWpf.UI.ViewModel.TreeView.TreeViewDelegates;

namespace SimpleWpf.UI.ViewModel.TreeView.Interface
{
    public interface ISimpleTree : ISimpleUpdater, ISimpleValidator
    {
        /// <summary>
        /// (Bubble Up Event) Occurs (once) when an item in the tree changes
        /// </summary>
        event TreeViewDelegates.ItemPropertyChangedTreeEventHandler ItemPropertyChangedEvent;

        /// <summary>
        /// Notify Collection Changed:  Fires once for a reset at the EndUpdate call
        /// </summary>
        event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>
        /// Occurs when the tree view's selected item collection has changed (maintained by the SimpleTreeView)
        /// </summary>
        event TreeSelectionChangedEventHandler TreeSelectionChangedEvent;

        /// <summary>
        /// Primary binding collection for the tree - hides IEnumerable, which doesn't work for handling the
        /// recursive enumeration; but it is necessary for the ItemsSource binding.
        /// </summary>
        ISimpleTreeCollection Collection { get; }

        /// <summary>
        /// Adds the tree view node to the tree by matching its parent. Returns the node that was sent.
        /// </summary>
        public T Add<T>(T node) where T : TreeViewNodeModelBase;
        public T? GetNode<T>(object key) where T : TreeViewNodeModelBase;
        public T? GetNode<T>(int recursionDepth, object key) where T : TreeViewNodeModelBase;

        /// <summary>
        /// Removes the tree view node from the tree. Must be done during an update!
        /// </summary>
        public void Remove<T>(T node) where T : TreeViewNodeModelBase;
        public bool Contains<T>(T node) where T : TreeViewNodeModelBase;
        public void Clear();
        public IEnumerable<TreeViewNodeModelBase> GetBranch(TreeViewNodeModelBase node, bool includeDescendants = false);
        public bool Any<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase;
        public void ForEach<T>(Action<T> action) where T : TreeViewNodeModelBase;
        public int Count<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase;
        public T? First<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase;
        public IEnumerable<T> Where<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase;
    }
}
