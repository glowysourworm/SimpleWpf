using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace SimpleWpf.UI.ViewModel.TreeView
{
    public class SimpleTreeViewModel : ViewModelBase, IEnumerable, INotifyCollectionChanged
    {
        /// <summary>
        /// (Bubble Up Event) Occurs (once) when an item in the tree changes
        /// </summary>
        public event TreeViewDelegates.ItemPropertyChangedTreeEventHandler ItemPropertyChangedEvent;

        /// <summary>
        /// Notify Collection Changed:  Fires once for a reset at the EndUpdate call
        /// </summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        // Root
        TreeViewNodeModelBase? _root;

        // Begin / End Update:  The primary issue with this tree view is multi-select. The numbering scheme
        //                      was used as a simple solution of getting contiguous select to work. It just
        //                      requires that we have a Begin / End update process to show users that the
        //                      tree will need to undergo a bigger change after it gets modified.
        //
        bool _updadting;
        bool _numberingSet;
        int _count;

        /// <summary>
        /// This may be required for binding on the UI's backend
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        public SimpleTreeViewModel()
        {
            _root = null;
            _updadting = false;
            _numberingSet = false;
            _count = 0;
        }
        public void BeginUpdate()
        {
            // Must be allowed to call this to add nodes

            _updadting = true;
        }
        public void EndUpdate()
        {
            if (!_updadting)
                throw new Exception("Trying to end update before calling BeginUpdate");

            // User has removed the root
            if (_root != null)
            {
                _root.SetTreeNumbering();

                _updadting = false;
                _numberingSet = _root.RecursiveAll(x => x.IsNumbered);
                _count = _root.RecursiveCount();
            }

            // Empty
            else
            {
                _updadting = false;
                _numberingSet = false;
                _count = 0;
            }

            if (this.CollectionChanged != null)
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            OnPropertyChanged(nameof(Count));
        }
        public bool IsUpdating()
        {
            return _updadting;
        }

        /// <summary>
        /// Gets count of nodes in the tree. This may only be called before / after an
        /// update!
        /// </summary>
        public int GetCount()
        {
            if (_updadting)
                throw new Exception("The tree node count is not set during an update");

            return _count;
        }

        /// <summary>
        /// Adds the tree view node to the tree by matching its parent. Returns the node that was sent.
        /// </summary>
        public T Add<T>(T node) where T : TreeViewNodeModelBase
        {
            if (!_updadting)
                throw new Exception("Must first call BeginUpdate before adding tree nodes");

            if (_root == null)
                _root = node;

            else if (node.Parent == null)
                throw new Exception("Trying to add node with no parent! The parent must first be set so that the tree can find which child collection to utilize");

            else
            {
                // Find Parent Node
                var parentNode = _root.RecursiveFirst(x => x == node.Parent);

                if (parentNode == null)
                    throw new Exception("Cannot find parent node for ancestor! Make sure that you've connected nodes properly before adding them to the tree");

                parentNode.Add(node);

                // These get reset once the nodes are added / removed
                _numberingSet = false;
                _count = 0;
            }

            return node;
        }

        /// <summary>
        /// Removes the tree view node from the tree. Must be done during an update!
        /// </summary>
        public void Remove<T>(T node) where T : TreeViewNodeModelBase
        {
            if (!_updadting)
                throw new Exception("Must first call BeginUpdate before removing tree nodes");

            if (_root == null)
                throw new Exception("Tree has no nodes! Must have first added nodes to the tree");

            else
            {
                // Find Parent Node
                var parentNode = _root.RecursiveFirst(x => x == node.Parent);

                if (parentNode == null)
                    throw new Exception("Cannot find parent node for ancestor! Make sure that you've connected nodes properly before adding them to the tree");

                // Root:  Just set root to null. The user has called for root to be removed; but may use the node in their code
                //
                if (parentNode == _root)
                    _root = null;
                else
                    parentNode.Remove(node);

                // This gets reset once the nodes are added
                _numberingSet = false;
                _count = 0;
            }
        }
        public void Clear()
        {
            if (_updadting)
                throw new Exception("Trying to clear the tree during an update");

            if (_root == null)
                return;

            _root.Clear();
            _numberingSet = false;
        }
        public bool IsNumberingSet()
        {
            if (_root == null)
                throw new NullReferenceException("Must first add nodes to the tree before calling any access methods");

            return _numberingSet;
        }
        public bool RecursiveAny<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            if (_root == null)
                throw new NullReferenceException("Must first add nodes to the tree before calling any access methods");

            return _root.RecursiveAny(predicate);
        }
        public void RecursiveForEach(Action<TreeViewNodeModelBase> action)
        {
            if (_root == null)
                throw new NullReferenceException("Must first add nodes to the tree before calling any access methods");

            _root.RecurseForEach(action);
        }
        public void RecursiveForEach<T>(Action<T> action) where T : TreeViewNodeModelBase
        {
            if (_root == null)
                throw new NullReferenceException("Must first add nodes to the tree before calling any access methods");

            _root.RecurseForEach<T>(action);
        }
        public int RecursiveCount<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            if (_root == null)
                throw new NullReferenceException("Must first add nodes to the tree before calling any access methods");

            return _root.RecursiveCount<T>(predicate);
        }
        public IEnumerable<T> RecursiveWhere<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            if (_root == null)
                throw new NullReferenceException("Must first add nodes to the tree before calling any access methods");

            return _root.RecursiveWhere(predicate);
        }
        public IEnumerator GetEnumerator()
        {
            if (_root == null)
                return Enumerable.Empty<TreeViewNodeModelBase>().GetEnumerator();

            return _root.GetEnumerator();
        }
        private void OnItemPropertyChanged(TreeViewNodeModelBase treeSender, object item, PropertyChangedEventArgs eventArgs)
        {
            if (this.ItemPropertyChangedEvent != null)
                this.ItemPropertyChangedEvent(treeSender, item, eventArgs);
        }
    }
}
