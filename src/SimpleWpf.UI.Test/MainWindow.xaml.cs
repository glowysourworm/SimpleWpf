using System.Windows;

using SimpleWpf.UI.ViewModel.TreeView;

namespace SimpleWpf.UI.Test
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var treeView = new TreeViewModel(new TreeViewNodeModel(0)
            {
                DisplayName = "Root"
            });

            for (int index = 0; index < 10; index++)
            {
                var item = treeView.Add(new TreeViewNodeModel(1)
                {
                    DisplayName = "Item " + index
                });

                for (int childIndex = 0; childIndex < 10; childIndex++)
                {
                    var child = item.Add(new TreeViewNodeModel(2)
                    {
                        DisplayName = "Child " + childIndex
                    });

                    for (int grandChildIndex = 0; grandChildIndex < 10; grandChildIndex++)
                    {
                        var grandChild = child.Add(new TreeViewNodeModel(3)
                        {
                            DisplayName = "Grand Child " + grandChildIndex
                        });
                    }
                }
            }

            this.DataContext = treeView;
        }
    }
}