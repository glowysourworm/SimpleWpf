namespace SimpleWpf.UI.ViewModel.TreeView
{
    public enum SimpleTreeViewBranchingStrategy
    {
        /// <summary>
        /// Creates new branches based on parent's key. Nodes will be placed in the tree based on 
        /// parent key matching.
        /// </summary>
        CommonParent
    }

    public enum SimpleTreeViewSelectionMode
    {
        /// <summary>
        /// Allows multiple selection; also includes CTRL + SHIFT modifiers; but does not include descendants
        /// on select.
        /// </summary>
        Normal,

        /// <summary>
        /// Includes descendants in addition to normal mode features.
        /// </summary>
        IncludeDescendants
    }
}
