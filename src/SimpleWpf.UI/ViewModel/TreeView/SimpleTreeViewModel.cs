using System.Collections;
using System.ComponentModel;

namespace SimpleWpf.UI.ViewModel.TreeView
{
    public class SimpleTreeViewModel : ViewModelBase, IDisposable, IEnumerable
    {
        /// <summary>
        /// Occurs when an item in the tree changes
        /// </summary>
        public event TreeViewDelegates.ItemPropertyChangedTreeEventHandler ItemPropertyChangedEvent;

        // Root
        TreeViewModelBase? _root;

        // Begin / End Update:  The primary issue with this tree view is multi-select. The numbering scheme
        //                      was used as a simple solution of getting contiguous select to work. It just
        //                      requires that we have a Begin / End update process to show users that the
        //                      tree will need to undergo a bigger change after it gets modified.
        //
        bool _updadting;
        bool _numberingSet;

        public SimpleTreeViewModel()
        {
            _root = null;
            _updadting = false;
            _numberingSet = false;
        }
        public void BeginUpdate()
        {
            // Must be allowed to call this to add nodes

            _updadting = true;
        }
        public void EndUpdate()
        {
            if (_root == null)
                throw new NullReferenceException("Must first add nodes to the tree before calling any access methods");

            if (!_updadting)
                throw new Exception("Trying to end update before calling BeginUpdate");

            _root.SetTreeNumbering();

            _updadting = false;
            _numberingSet = _root.RecursiveAll(x => x.IsNumbered);
        }

        /// <summary>
        /// Adds the tree view node to the tree by matching its parent. Returns the node that was sent.
        /// </summary>
        public T Add<T>(T node) where T : TreeViewModelBase
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

                // This gets reset once the nodes are added
                _numberingSet = false;
            }

            return node;
        }
        public bool IsNumberingSet()
        {
            if (_root == null)
                throw new NullReferenceException("Must first add nodes to the tree before calling any access methods");

            return _numberingSet;
        }
        public bool RecursiveAny<T>(Func<T, bool> predicate) where T : TreeViewModelBase
        {
            if (_root == null)
                throw new NullReferenceException("Must first add nodes to the tree before calling any access methods");

            return _root.RecursiveAny(predicate);
        }
        public void RecursiveForEach(Action<TreeViewModelBase> action)
        {
            if (_root == null)
                throw new NullReferenceException("Must first add nodes to the tree before calling any access methods");

            _root.RecurseForEach(action);
        }
        public void RecursiveForEach<T>(Action<T> action) where T : TreeViewModelBase
        {
            if (_root == null)
                throw new NullReferenceException("Must first add nodes to the tree before calling any access methods");

            _root.RecurseForEach<T>(action);
        }
        public void RecursiveCount<T>(Func<T, bool> predicate) where T : TreeViewModelBase
        {
            if (_root == null)
                throw new NullReferenceException("Must first add nodes to the tree before calling any access methods");

            _root.RecursiveCount<T>(predicate);
        }
        public IEnumerable<T> RecursiveWhere<T>(Func<T, bool> predicate) where T : TreeViewModelBase
        {
            if (_root == null)
                throw new NullReferenceException("Must first add nodes to the tree before calling any access methods");

            return _root.RecursiveWhere(predicate);
        }
        public IEnumerator GetEnumerator()
        {
            if (_root == null)
                throw new NullReferenceException("Must first add nodes to the tree before calling any access methods");

            return _root.GetEnumerator();
        }
        private void OnItemPropertyChanged(TreeViewModelBase treeSender, object item, PropertyChangedEventArgs eventArgs)
        {
            if (this.ItemPropertyChangedEvent != null)
                this.ItemPropertyChangedEvent(treeSender, item, eventArgs);
        }
        public void Dispose()
        {
            if (_root != null)
            {
                _root.Dispose();
            }
        }
    }
}
