using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

using static SimpleWpf.UI.ViewModel.TreeView.TreeViewDelegates;

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

        /// <summary>
        /// Occurs when the tree view's selected item collection has changed (maintained by the SimpleTreeView)
        /// </summary>
        public event TreeSelectionChangedEventHandler TreeSelectionChangedEvent;

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
        public virtual void EndUpdate()
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
        /// Returns branch of the tree by tracing upward starting from the provided node. The
        /// nodes are ordered forwards - starting from the root.
        /// </summary>
        public IEnumerable<TreeViewNodeModelBase> GetBranch(TreeViewNodeModelBase node)
        {
            if (node == null)
                throw new ArgumentNullException("Node not set to an instance of the node class");

            var stack = new Stack<TreeViewNodeModelBase>();
            var result = new List<TreeViewNodeModelBase>();
            var currentNode = node;

            do
            {
                // Stack these up from the leaf-er node
                stack.Push(currentNode);

                // Trace upwards towards the root
                currentNode = currentNode.Parent;

            } while (currentNode != null);

            // Arrange these starting with the root
            while (stack.Any())
            {
                result.Add(stack.Pop());
            }

            return result;
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
        public bool Contains<T>(T node) where T : TreeViewNodeModelBase
        {
            return RecursiveAny<T>(item =>
            {
                return item == node;
            });
        }
        public void Clear()
        {
            if (!_updadting)
                throw new Exception("Must first call BeginUpdate before removing tree nodes");

            if (_root == null)
                return;

            _root.Clear();
            _numberingSet = false;
        }
        public bool IsNumberingSet()
        {
            if (_root == null)
                return false;

            return _numberingSet;
        }
        public bool RecursiveAny<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            if (_root == null)
                return false;

            return _root.RecursiveAny(predicate);
        }
        public void RecursiveForEach(Action<TreeViewNodeModelBase> action)
        {
            if (_root == null)
                return;

            _root.RecurseForEach(action);
        }
        public void RecursiveForEach<T>(Action<T> action) where T : TreeViewNodeModelBase
        {
            if (_root == null)
                return;

            _root.RecurseForEach<T>(action);
        }
        public int RecursiveCount<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            if (_root == null)
                return 0;

            return _root.RecursiveCount<T>(predicate);
        }
        public T? RecursiveFirst<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            if (_root == null)
                return null;

            return _root.RecursiveFirst<T>(predicate);
        }
        public IEnumerable<T> RecursiveWhere<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            if (_root == null)
                return Enumerable.Empty<T>();

            return _root.RecursiveWhere(predicate);
        }
        public IEnumerator GetEnumerator()
        {
            if (_root == null)
                return Enumerable.Empty<TreeViewNodeModelBase>().GetEnumerator();

            return _root.GetEnumerator();
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
