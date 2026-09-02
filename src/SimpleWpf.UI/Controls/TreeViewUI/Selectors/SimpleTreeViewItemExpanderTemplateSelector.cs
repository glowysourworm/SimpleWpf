using System.Windows;
using System.Windows.Controls;

using Xceed.Wpf.Toolkit.Core.Utilities;

namespace SimpleWpf.UI.Controls.TreeViewUI.Selectors
{
    public class SimpleTreeViewItemExpanderTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var listBoxItem = VisualTreeHelperEx.FindAncestorByType<ListBoxItem>(container);

            if (listBoxItem == null ||
                listBoxItem.DataContext == null)
                throw new Exception("Trying to select data template for a null item, or null data context");

            // Going to need some reflection here because of the template class hierarchy...
            try
            {
                var nodeValueProperty = listBoxItem.DataContext.GetType().GetProperty("NodeValue");
                var isExpandedProperty = nodeValueProperty.PropertyType.GetProperty("IsExpanded");
                var nodeValue = nodeValueProperty.GetValue(listBoxItem.DataContext);
                var isExpanded = (bool)isExpandedProperty.GetValue(nodeValue);

                if (isExpanded)
                    return listBoxItem.FindResource("SimpleTreeViewItemExpanderOpenTemplate") as DataTemplate;

                else
                    return listBoxItem.FindResource("SimpleTreeViewItemExpanderClosedTemplate") as DataTemplate;
            }
            catch (Exception ex)
            {
                throw new Exception("Error trying to get the IsExpanded property from the data context of the SimpleTreeView");
            }
        }
    }
}
