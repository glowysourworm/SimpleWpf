using System.Collections;
using System.Collections.Specialized;

using SimpleWpf.Extensions.ObservableCollection;

namespace SimpleWpf.UI.ViewModel.TreeView
{
    public class SimpleTreeCollectionViewModel : ViewModelBase, IEnumerable, INotifyCollectionChanged
    {
        // The base of the tree is a collection of root nodes - which is itself observable
        KeyedObservableCollection<object, TreeViewNodeModelBase> _rootNodes;

        /// <summary>
        /// Collection changed event for the root nodes (tree nodes are automatically bound to the tree); and
        /// there are tree-wide item changed events that bubble up to the roots.
        /// </summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public int Count
        {
            get { return _count; }
            set { this.RaiseAndSetIfChanged(ref _count, value); }
        }

        // Branch Strategy
        SimpleTreeViewBranchStrategy _branchStrategy;

        // Begin / End Update Pattern
        bool _updating;
        bool _invalid;
        int _count;

        public SimpleTreeCollectionViewModel(SimpleTreeViewBranchStrategy branchStrategy)
        {
            _branchStrategy = branchStrategy;
            _rootNodes = new KeyedObservableCollection<object, TreeViewNodeModelBase>();
            _rootNodes.CollectionChanged += OnCollectionChanged;
        }

        public void BeginUpdate()
        {
            // Must be allowed to call this to add nodes

            _updating = true;
        }
        public virtual void EndUpdate()
        {
            if (!_updating)
                throw new Exception("Trying to end update before calling BeginUpdate");

            var treeCounter = 0;

            // Set Numbering (multi-selection)
            foreach (var rootNode in _rootNodes.Values)
                rootNode.SetTreeNumbering(ref treeCounter);    // Set node numbers (ItemNumber)

            // Set Invalid Flag (this is for tree multi-selection)
            _invalid = SimpleTreeAny<TreeViewNodeModelBase>(x => !x.IsNumbered);

            // Set Count
            _count = treeCounter;

            // Complete update
            _updating = false;

            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public bool IsUpdating()
        {
            return _updating;
        }
        public bool IsInvalid()
        {
            return _invalid;
        }
        public void SimpleTreeAdd(TreeViewNodeModelBase node)
        {
            if (!_updating)
                throw new Exception("Must first call BeginUpdate before adding tree nodes");

            if (SimpleTreeContains(node))
                throw new Exception("Node already contained in the tree");

            var parent = GetCommonParent(node);

            // New Branch
            if (parent == null)
            {
                // -> CollectionChanged
                _rootNodes.Add(node.Key, node);
            }
            else
            {
                // -> ... -> Children.CollectionChanged
                parent.Add(node);
            }
        }

        /// <summary>
        /// Removes the tree view node from the tree. Must be done during an update!
        /// </summary>
        public void SimpleTreeRemove<T>(T node) where T : TreeViewNodeModelBase
        {
            if (!_updating)
                throw new Exception("Must first call BeginUpdate before removing tree nodes");

            else
            {
                // Check Root(s)
                if (_rootNodes.ContainsKey(node.Key))
                {
                    _rootNodes.Remove(node.Key);
                }
                else
                {
                    // Find Parent Node
                    var parentNode = GetCommonParent(node);

                    if (parentNode == null)
                        throw new Exception("Cannot locate node in the tree");

                    parentNode.Remove(node);
                }

                // -> Unhook events and clear children
                node.Clear();

                // This gets reset once the nodes are added
                _invalid = true;
                _count = 0;
            }
        }
        public IEnumerator GetEnumerator()
        {
            return _rootNodes.Values.GetEnumerator();
        }
        public bool SimpleTreeContains<T>(T node) where T : TreeViewNodeModelBase
        {
            foreach (var rootNode in _rootNodes.Values)
            {
                if (rootNode.RecursiveContains(node))
                    return true;
            }

            return false;
        }
        public bool SimpleTreeAll<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            foreach (var rootNode in _rootNodes.Values)
            {
                if (rootNode.RecursiveAll(predicate))
                    return true;
            }

            return false;
        }
        public bool SimpleTreeAny<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            foreach (var rootNode in _rootNodes.Values)
            {
                if (rootNode.RecursiveAny(predicate))
                    return true;
            }

            return false;
        }
        public int SimpleTreeCount<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            var result = 0;

            foreach (var rootNode in _rootNodes.Values)
            {
                result += rootNode.RecursiveCount(predicate);
            }

            return result;
        }
        public T? SimpleTreeFirst<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            T? result = null;

            foreach (var rootNode in _rootNodes.Values)
            {
                result = rootNode.RecursiveFirst<T>(predicate);

                if (result != null)
                    break;
            }

            return result;
        }
        public IEnumerable<T> SimpleTreeWhere<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            var result = new List<T>();

            foreach (var rootNode in _rootNodes.Values)
            {
                result.AddRange(rootNode.RecursiveWhere(predicate));
            }

            return result;
        }
        public void SimpleTreeForEach<T>(Action<T> action) where T : TreeViewNodeModelBase
        {
            foreach (var rootNode in _rootNodes.Values)
            {
                rootNode.RecurseForEach(action);
            }
        }
        public void SimpleTreeClear()
        {
            foreach (var rootNode in _rootNodes.Values)
            {
                // Unhook nodes and clear children
                rootNode.Clear();
            }
        }
        private TreeViewNodeModelBase? GetCommonParent(TreeViewNodeModelBase item)
        {
            if (item.Parent == null)
                return null;

            TreeViewNodeModelBase? result = null;

            foreach (var rootNode in _rootNodes.Values)
            {
                rootNode.RecurseForEach(node =>
                {
                    // Reference -> Value (comparison) (??)
                    if (node.Key == item.Parent.Key)
                    {
                        result = node;
                        return;
                    }
                });
            }

            return result;
        }
        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // Forward this event
            if (this.CollectionChanged != null)
                this.CollectionChanged(sender, e);
        }
    }
}
