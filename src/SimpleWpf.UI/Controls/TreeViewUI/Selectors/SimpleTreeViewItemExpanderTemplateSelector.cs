using System.Windows;
using System.Windows.Controls;

using Xceed.Wpf.Toolkit.Core.Utilities;

namespace SimpleWpf.UI.Controls.TreeViewUI.Selectors
{
    public class SimpleTreeViewItemExpanderTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var treeViewItem = VisualTreeHelperEx.FindAncestorByType<TreeViewItem>(container);

            if (treeViewItem == null ||
                treeViewItem.DataContext == null)
                throw new Exception("Trying to select data template for a null item, or null data context");

            // Going to need some reflection here because of the template class hierarchy...
            try
            {
                var nodeValueProperty = treeViewItem.DataContext.GetType().GetProperty("NodeValue");
                var isExpandedProperty = nodeValueProperty.PropertyType.GetProperty("IsExpanded");
                var nodeValue = nodeValueProperty.GetValue(treeViewItem.DataContext);
                var isExpanded = (bool)isExpandedProperty.GetValue(nodeValue);

                if (isExpanded)
                    return treeViewItem.FindResource("SimpleTreeViewItemExpanderOpenTemplate") as DataTemplate;

                else
                    return treeViewItem.FindResource("SimpleTreeViewItemExpanderClosedTemplate") as DataTemplate;
            }
            catch (Exception ex)
            {
                throw new Exception("Error trying to get the IsExpanded property from the data context of the SimpleTreeView");
            }
        }
    }
}
