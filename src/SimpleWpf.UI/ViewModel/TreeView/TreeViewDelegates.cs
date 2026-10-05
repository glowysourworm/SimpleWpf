using System.ComponentModel;

namespace SimpleWpf.UI.ViewModel.TreeView
{
    public class TreeViewDelegates
    {
        /// <summary>
        /// (Bubble Up Event) Delegate to handle tree events. The intended use is to hook this up at the top level of the tree to listen for tree
        ///                   events. The data will be forwarded from the tree level where the event took place.
        /// </summary>
        /// <param name="treeSender">Sender for sub-tree view model where the event was fired</param>
        /// <param name="item">The child item for the sub-tree's children</param>
        /// <param name="eventArgs">Event data for the change</param>
        public delegate void ItemPropertyChangedTreeEventHandler(TreeViewNodeModelBase treeSender, object item, PropertyChangedEventArgs eventArgs);

        /// <summary>
        /// (UI-Direct Event) This is a single-fire event which is forwarded from the UI for handling the tree's selection. This UI performance is
        ///                   much better than direct binding due to the amount of needless back-and-forth with the bindings, UI, templates, and 
        ///                   user code.
        /// </summary>
        public delegate void TreeSelectionChangedEventHandler(IEnumerable<TreeViewNodeModelBase> selectedNodes);
    }
}
