using System.Windows;
using System.Windows.Controls;

using Xceed.Wpf.Toolkit.Core.Utilities;

namespace SimpleWpf.UI.Controls.TreeViewUI.Selectors
{
    public class SimpleTreeViewItemTemplateSelector : DataTemplateSelector
    {
        public SimpleTreeViewItemTemplateSelector()
        { }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var listBoxItem = VisualTreeHelperEx.FindAncestorByType<ListBoxItem>(container);

            if (listBoxItem == null)
                throw new NullReferenceException("Improper handling of SimpleTreeViewItemTemplateSelector");

            //var viewModel = listBoxItem.DataContext as TreeViewModelBase<TreeViewNodeModelBase>;

            //if (viewModel == null)
            //    throw new NullReferenceException("Improper handling of SimpleTreeViewItemTemplateSelector");

            // Templates:  There may be a need for template selection; but this is primarily to 
            //             use these with recursion. For further styling it may be easier to expose
            //             other dependency properties of the TreeView

            return listBoxItem.FindResource("SimpleTreeViewItemTemplate") as DataTemplate;
        }
    }
}
