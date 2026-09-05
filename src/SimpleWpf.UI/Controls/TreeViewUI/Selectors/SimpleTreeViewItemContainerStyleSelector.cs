using System.Windows;
using System.Windows.Controls;

using Xceed.Wpf.Toolkit.Core.Utilities;

namespace SimpleWpf.UI.Controls.TreeViewUI.Selectors
{
    public class SimpleTreeViewItemContainerStyleSelector : StyleSelector
    {
        public SimpleTreeViewItemContainerStyleSelector()
        {
        }
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var treeViewItem = VisualTreeHelperEx.FindAncestorByType<TreeViewItem>(container);

            if (treeViewItem == null)
                throw new NullReferenceException("Improper handling of SimpleTreeViewItemContainerStyleSelector");

            // SPECIFIC VIEW MODEL
            //
            //var viewModel = listBoxItem.DataContext as TreeViewModelBase<TreeViewNodeModelBase>;

            //if (viewModel == null)
            //    throw new NullReferenceException("Improper handling of SimpleTreeViewItemContainerStyleSelector");

            // Styling:  There may be a need for style selection; but this is primarily to 
            //           use these with recursion.
            return treeViewItem.FindResource("SimpleTreeViewItemContainerStyle") as Style;
        }
    }
}
