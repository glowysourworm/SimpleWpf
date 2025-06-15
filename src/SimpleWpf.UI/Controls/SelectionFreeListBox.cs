using System.Windows.Controls;

namespace SimpleWpf.UI.Controls
{
    public class SelectionFreeListBox : ListBox
    {
        protected override void OnPreviewMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
