using System.Collections.Specialized;
using System.ComponentModel;

using SimpleWpf.Extensions.Collection;

using static SimpleWpf.UI.ViewModel.TreeView.TreeViewDelegates;

namespace SimpleWpf.UI.ViewModel.TreeView
{
    public class SimpleTreeViewModel : ViewModelBase
    {
        /// <summary>
        /// (Bubble Up Event) Occurs (once) when an item in the tree changes
        /// </summary>
        public event TreeViewDelegates.ItemPropertyChangedTreeEventHandler ItemPropertyChangedEvent;

        /// <summary>
        /// Notify Collection Changed:  Fires once for a reset at the EndUpdate call
        /// </summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>
        /// Occurs when the tree view's selected item collection has changed (maintained by the SimpleTreeView)
        /// </summary>
        public event TreeSelectionChangedEventHandler TreeSelectionChangedEvent;

        // Collection
        SimpleTreeCollectionViewModel _collection;

        /// <summary>
        /// Primary binding collection for the tree - hides IEnumerable, which doesn't work for handling the
        /// recursive enumeration; but it is necessary for the ItemsSource binding.
        /// </summary>
        public SimpleTreeCollectionViewModel Collection
        {
            get { return _collection; }
            set { this.RaiseAndSetIfChanged(ref _collection, value); }
        }

        public SimpleTreeViewModel()
        {
            this.Collection = new SimpleTreeCollectionViewModel(SimpleTreeViewBranchStrategy.CommonParent);
        }
        public void BeginUpdate()
        {
            _collection.BeginUpdate();
        }
        public virtual void EndUpdate()
        {
            _collection.EndUpdate();
        }
        public bool IsUpdating()
        {
            return _collection.IsUpdating();
        }

        /// <summary>
        /// Adds the tree view node to the tree by matching its parent. Returns the node that was sent.
        /// </summary>
        public T Add<T>(T node) where T : TreeViewNodeModelBase
        {
            _collection.SimpleTreeAdd(node);

            return node;
        }

        /// <summary>
        /// Removes the tree view node from the tree. Must be done during an update!
        /// </summary>
        public void Remove<T>(T node) where T : TreeViewNodeModelBase
        {
            _collection.SimpleTreeRemove(node);
        }
        public bool Contains<T>(T node) where T : TreeViewNodeModelBase
        {
            return _collection.SimpleTreeContains(node);
        }
        public void Clear()
        {
            _collection.SimpleTreeClear();
        }
        public bool IsInvalid()
        {
            return _collection.IsInvalid();
        }
        public bool Any<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            return _collection.Any(predicate);
        }
        public void ForEach<T>(Action<T> action) where T : TreeViewNodeModelBase
        {
            _collection.SimpleTreeForEach(action);
        }
        public int Count<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            return _collection.SimpleTreeCount(predicate);
        }
        public T? First<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            return _collection.SimpleTreeFirst(predicate);
        }
        public IEnumerable<T> Where<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            return _collection.SimpleTreeWhere(predicate);
        }
        internal void SetSelection(IEnumerable<TreeViewNodeModelBase> selectedNodes)
        {
            // This is exposing the selection event forwarding for use in the UI tree (which is hidden in the base class)
            //
            if (this.TreeSelectionChangedEvent != null)
                this.TreeSelectionChangedEvent(selectedNodes);
        }
        private void OnItemPropertyChanged(TreeViewNodeModelBase treeSender, object item, PropertyChangedEventArgs eventArgs)
        {
            if (this.ItemPropertyChangedEvent != null)
                this.ItemPropertyChangedEvent(treeSender, item, eventArgs);
        }
    }
}
