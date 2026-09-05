using System.Collections.Specialized;
using System.ComponentModel;

using SimpleWpf.UI.ViewModel.TreeView.Interface;

namespace SimpleWpf.UI.ViewModel.TreeView
{
    public class TreeViewDelegates
    {
        /// <summary>
        /// (Bubble Up Event) Delegate to handle tree events. The intended use is to hook this up at the top level of the tree to listen for tree
        ///                   events. The data will be forwarded from the tree level where the event took place.
        /// </summary>
        /// <param name="treeSender">Sender for sub-tree view model where the event was fired</param>
        /// <param name="sender">The child collection for INotifyCollectionChanged typical events</param>
        /// <param name="eventArgs">Event data for the change</param>
        public delegate void CollectionChangedTreeEventHandler(TreeViewModelBase treeSender, object sender, NotifyCollectionChangedEventArgs eventArgs);

        /// <summary>
        /// (Bubble Up Event) Delegate to handle tree events. The intended use is to hook this up at the top level of the tree to listen for tree
        ///                   events. The data will be forwarded from the tree level where the event took place.
        /// </summary>
        /// <param name="treeSender">Sender for sub-tree view model where the event was fired</param>
        /// <param name="item">The child item for the sub-tree's children</param>
        /// <param name="eventArgs">Event data for the change</param>
        public delegate void ItemPropertyChangedTreeEventHandler(TreeViewModelBase treeSender, ITreeViewNode item, PropertyChangedEventArgs eventArgs);
    }
}
