using System.Windows;
using System.Windows.Controls;

using SimpleWpf.UI.ViewModel.TreeView;

using Xceed.Wpf.Toolkit.Core.Utilities;

namespace SimpleWpf.UI.Controls.TreeViewUI.Selectors
{
    public class SimpleTreeViewItemExpanderTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var listBoxItem = VisualTreeHelperEx.FindAncestorByType<ListBoxItem>(container);
            var viewModel = listBoxItem.DataContext as TreeViewModel;

            if (viewModel == null)
                throw new Exception("Invalid use of SimpleTreeViewItemExpanderTemplateSelector. Must use for TreeViewNodeModel derived classes");

            if (viewModel.NodeValue.IsExpanded)
                return listBoxItem.FindResource("SimpleTreeViewItemExpanderOpenTemplate") as DataTemplate;

            else
                return listBoxItem.FindResource("SimpleTreeViewItemExpanderClosedTemplate") as DataTemplate;
        }
    }
}
