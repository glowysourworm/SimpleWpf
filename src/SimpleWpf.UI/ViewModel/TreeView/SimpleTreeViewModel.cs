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
        TreeViewModelBase _root;

        public SimpleTreeViewModel(TreeViewModelBase root)
        {
            _root = root;

            _root.ItemPropertyChanged += OnItemPropertyChanged;
        }
        public bool IsNumberingSet()
        {
            return _root.IsNumbered;
        }
        public void RecursiveForEach(Action<TreeViewModelBase> action)
        {
            _root.RecurseForEach(action);
        }
        public void RecursiveForEach<T>(Action<T> action) where T : TreeViewModelBase
        {
            _root.RecurseForEach<T>(action);
        }
        public void RecursiveCount(Func<TreeViewModelBase, bool> predicate)
        {
            _root.RecursiveCount(predicate);
        }
        public void RecursiveCount<T>(Func<T, bool> predicate) where T : TreeViewModelBase
        {
            _root.RecursiveCount<T>(predicate);
        }
        public IEnumerable<TreeViewModelBase> RecursiveWhere(Func<TreeViewModelBase, bool> predicate)
        {
            return _root.RecursiveWhere(predicate);
        }
        public IEnumerable<T> RecursiveWhere<T>(Func<T, bool> predicate) where T : TreeViewModelBase
        {
            return _root.RecursiveWhere<T>(predicate);
        }
        public IEnumerator GetEnumerator()
        {
            return _root.GetEnumerator();
        }
        private void OnItemPropertyChanged(TreeViewModelBase treeSender, object item, PropertyChangedEventArgs eventArgs)
        {
            if (this.ItemPropertyChangedEvent != null)
                this.ItemPropertyChangedEvent(treeSender, item, eventArgs);
        }
        public void Dispose()
        {
            _root.Dispose();
        }
    }
}
