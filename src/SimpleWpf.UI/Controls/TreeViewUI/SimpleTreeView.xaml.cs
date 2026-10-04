using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using SimpleWpf.Extensions.Event;
using SimpleWpf.UI.ViewModel.TreeView;

using Xceed.Wpf.Toolkit.Core.Utilities;

namespace SimpleWpf.UI.Controls.TreeViewUI
{
    public partial class SimpleTreeView : UserControl
    {
        #region (public) Dependency Properties
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SimpleTreeView), new PropertyMetadata(OnItemsSourcePropertyChanged));

        public static readonly DependencyProperty ItemExpanderClosedTemplateProperty =
            DependencyProperty.Register("ItemExpanderClosedTemplate", typeof(DataTemplate), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemExpanderOpenTemplateProperty =
            DependencyProperty.Register("ItemExpanderOpenTemplate", typeof(DataTemplate), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemHeaderContentTemplateProperty =
            DependencyProperty.Register("ItemHeaderContentTemplate", typeof(DataTemplate), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemIndentProperty =
            DependencyProperty.Register("ItemIndent", typeof(int), typeof(SimpleTreeView), new PropertyMetadata(0));

        public static readonly DependencyProperty ItemPaddingProperty =
            DependencyProperty.Register("ItemPadding", typeof(Thickness), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemBackgroundProperty =
            DependencyProperty.Register("ItemBackground", typeof(Brush), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemBackgroundAlternationProperty =
            DependencyProperty.Register("ItemBackgroundAlternation", typeof(Brush), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemHoverBrushProperty =
            DependencyProperty.Register("ItemHoverBrush", typeof(Brush), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemSelectionBrushProperty =
            DependencyProperty.Register("ItemSelectionBrush", typeof(Brush), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemBorderProperty =
            DependencyProperty.Register("ItemBorder", typeof(Brush), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemBorderThicknessProperty =
            DependencyProperty.Register("ItemBorderThickness", typeof(Thickness), typeof(SimpleTreeView));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public int ItemIndent
        {
            get { return (int)GetValue(ItemIndentProperty); }
            set { SetValue(ItemIndentProperty, value); }
        }
        public DataTemplate ItemExpanderClosedTemplate
        {
            get { return (DataTemplate)GetValue(ItemExpanderClosedTemplateProperty); }
            set { SetValue(ItemExpanderClosedTemplateProperty, value); }
        }
        public DataTemplate ItemExpanderOpenTemplate
        {
            get { return (DataTemplate)GetValue(ItemExpanderOpenTemplateProperty); }
            set { SetValue(ItemExpanderOpenTemplateProperty, value); }
        }
        public DataTemplate ItemHeaderContentTemplate
        {
            get { return (DataTemplate)GetValue(ItemHeaderContentTemplateProperty); }
            set { SetValue(ItemHeaderContentTemplateProperty, value); }
        }
        public Thickness ItemPadding
        {
            get { return (Thickness)GetValue(ItemPaddingProperty); }
            set { SetValue(ItemPaddingProperty, value); }
        }
        public Brush ItemBackground
        {
            get { return (Brush)GetValue(ItemBackgroundProperty); }
            set { SetValue(ItemBackgroundProperty, value); }
        }
        public Brush ItemBackgroundAlternation
        {
            get { return (Brush)GetValue(ItemBackgroundAlternationProperty); }
            set { SetValue(ItemBackgroundAlternationProperty, value); }
        }
        public Brush ItemHoverBrush
        {
            get { return (Brush)GetValue(ItemHoverBrushProperty); }
            set { SetValue(ItemHoverBrushProperty, value); }
        }
        public Brush ItemSelectionBrush
        {
            get { return (Brush)GetValue(ItemSelectionBrushProperty); }
            set { SetValue(ItemSelectionBrushProperty, value); }
        }
        public Brush ItemBorder
        {
            get { return (Brush)GetValue(ItemBorderProperty); }
            set { SetValue(ItemBorderProperty, value); }
        }
        public Thickness ItemBorderThickness
        {
            get { return (Thickness)GetValue(ItemBorderThicknessProperty); }
            set { SetValue(ItemBorderThicknessProperty, value); }
        }
        #endregion

        /// <summary>
        /// Event that occurs when a tree item is expanded or collapsed. The first argument is the sender (bound item). The
        /// second is the current expanded state.
        /// </summary>
        public event SimpleEventHandler<object, bool> ItemExpandedEvent;

        /// <summary>
        /// Event that occurs when the selection in the treeview has changed
        /// </summary>
        public event SimpleEventHandler<SimpleTreeView, IEnumerable<TreeViewModelBase>> SelectedItemsChanged;

        // Private collections
        private Dictionary<TreeViewModelBase, TreeViewModelBase> _selectedItems;

        // Node Selection
        TreeViewModel? _selectedNode;

        public SimpleTreeView()
        {
            InitializeComponent();

            _selectedItems = new Dictionary<TreeViewModelBase, TreeViewModelBase>();
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            //// Calculate scroll extent
            //var scrollAmount = Math.Clamp(this.TheScrollViewer.VerticalOffset - e.Delta, 0, this.TheScrollViewer.ScrollableHeight);

            //// Handle scroll with the viewer
            //this.TheScrollViewer.ScrollToVerticalOffset(scrollAmount);

            //e.Handled = true;
        }

        // Occurs when a property on the UI (target) side changes
        private void HandleTreeSelection(SimpleTreeViewModel tree, TreeViewModel nodeClicked)
        {
            var ctrl = Keyboard.Modifiers == ModifierKeys.Control;
            var shift = Keyboard.Modifiers == ModifierKeys.Shift;

            // Ordinary Selection:  (options) Single Select; Recursive Select (down the tree)
            if (!ctrl && !shift)
            {
                // Single Select
                //
                // 1) Tree becomes de-selected
                // 2) The selected node gets toggled
                //

                // Save Selection
                var isSelected = nodeClicked.IsSelected;

                // De-Select
                tree.RecursiveForEach(treeNode =>
                {
                    treeNode.IsSelected = false;

                    // Update Selection List
                    UpdateSelection(treeNode);
                });

                // Set Selection
                nodeClicked.IsSelected = !isSelected;

                // Set Follower
                _selectedNode = nodeClicked.IsSelected ? nodeClicked : null;

                // Update Selection List
                UpdateSelection(nodeClicked);
            }

            // Ctrl + Select:  User Single Select 
            else if (ctrl)
            {
                // User Single Select
                //
                // 1) Clicked node gets toggled
                //

                nodeClicked.IsSelected = !nodeClicked.IsSelected;

                // Set Follower
                _selectedNode = nodeClicked.IsSelected ? nodeClicked : null;

                // Update Selection List
                UpdateSelection(nodeClicked);
            }

            // Shift + Select:  Multiple Selection
            else if (shift)
            {
                // Multiple Selection:  We have a trick for handling this. The nodes are first
                //                      numbered going down the tree. Then, the item id's can
                //                      be used to do contiguous select.
                //

                // Preivious Selected Node
                if (_selectedNode != null)
                {
                    // First, check to see that tree has been numbered
                    if (!tree.IsNumberingSet())
                        throw new Exception("Must set tree numbering before using multi-selection");

                    // Get two item numbers
                    var numberLow = Math.Min(_selectedNode.ItemId, nodeClicked.ItemId);
                    var numberHigh = Math.Max(_selectedNode.ItemId, nodeClicked.ItemId);

                    tree.RecursiveForEach(treeNode =>
                    {
                        if (treeNode.ItemId >= numberLow &&
                            treeNode.ItemId <= numberHigh)
                        {
                            treeNode.IsSelected = true;
                        }
                        else
                            treeNode.IsSelected = false;

                        // Update Selection List
                        UpdateSelection(treeNode);
                    });
                }

                // Single Select
                else
                {
                    nodeClicked.IsSelected = !nodeClicked.IsSelected;

                    // Set Follower
                    _selectedNode = nodeClicked.IsSelected ? nodeClicked : null;

                    // Update Selection List
                    UpdateSelection(nodeClicked);
                }
            }


            //// Selection:  Follow a pattern similar to most tree views (can select "sequentially")
            ////
            //if (tree.RecursionDepth == sender.RecursionDepth)
            //{
            //    // Begin Update:  Prevent further events from firing until the update is finished
            //    sender.BeginUpdate();

            //    // Recurse Tree:  Set selection appropriately
            //    tree.RecurseForEach<TreeViewModel>(childItem =>
            //    {
            //        var selected = childItem.IsSelected;

            //        // Parent Items
            //        if (childItem.RecursionDepth < sender.RecursionDepth)
            //            childItem.IsSelected = false;

            //        if (childItem.RecursionDepth == sender.RecursionDepth)
            //            childItem.IsSelected = childItem.IsSelected && (sender.Parent == childItem.Parent);

            //        // Child Items
            //        else if (childItem.RecursionDepth > sender.RecursionDepth)
            //        {
            //            if (!sender.IsSelected)
            //                childItem.IsSelected = false;

            //            else
            //            {
            //                if (childItem.HasDirectAncestor(sender))
            //                    childItem.IsSelected = sender.IsSelected;

            //                else
            //                    childItem.IsSelected = false;
            //            }
            //        }

            //        // Selection Changed
            //        if (childItem.IsSelected && !_selectedItems.ContainsKey(childItem))
            //            _selectedItems.Add(childItem, childItem);

            //        if (!childItem.IsSelected && _selectedItems.ContainsKey(childItem))
            //            _selectedItems.Remove(childItem);

            //    });

            //    sender.EndUpdate();

            // Selected Items Changed
            if (this.SelectedItemsChanged != null)
                this.SelectedItemsChanged(this, _selectedItems.Values);
        }

        private void UpdateSelection(TreeViewModelBase treeNode)
        {
            // Selection Changed
            if (treeNode.IsSelected && !_selectedItems.ContainsKey(treeNode))
                _selectedItems.Add(treeNode, treeNode);

            if (!treeNode.IsSelected && _selectedItems.ContainsKey(treeNode))
                _selectedItems.Remove(treeNode);
        }

        private void UpdateItemsSource()
        {
            var viewModel = this.ItemsSource as TreeViewModelBase;

            if (viewModel != null)
            {
                //viewModel.ItemPropertyChangedTreeEvent -= OnItemSourceItemPropertyChanged;
                //viewModel.ItemPropertyChangedTreeEvent += OnItemSourceItemPropertyChanged;
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.OriginalSource is not DependencyObject)
                base.OnPreviewMouseLeftButtonDown(e);

            else
            {
                // Procedure: Prefer expander to selection first
                //
                // 1) Get TreeViewItem from the event data
                // 2) Search for the ContentSource="Header" (ContentPresenter)
                // 3) Check the header first for expander click
                //

                // Get topmost item
                var treeViewItem = VisualTreeHelperEx.FindAncestorByType<TreeViewItem>(e.OriginalSource as DependencyObject);

                if (treeViewItem == null)
                {
                    base.OnPreviewMouseLeftButtonDown(e);
                    return;
                }

                // Get our data context
                var viewModel = treeViewItem.DataContext as TreeViewModel;

                if (viewModel == null)
                {
                    base.OnPreviewMouseLeftButtonDown(e);
                    return;
                }

                // Get header
                var header = VisualTreeHelperEx.FindAncestorByType<ContentPresenter>(e.OriginalSource as DependencyObject);

                if (header != null)
                {
                    // Templated Parent will be the ContentPresenter that holds the template for the expander
                    var headerPresenter = header.TemplatedParent as ContentPresenter;

                    if (headerPresenter != null)
                    {
                        var inputElement = headerPresenter.InputHitTest(e.GetPosition(treeViewItem));

                        // Expansion
                        if (inputElement != null && inputElement.IsMouseOver)
                        {
                            viewModel.IsExpanded = !viewModel.IsExpanded;
                            e.Handled = true;
                            return;
                        }
                    }
                }

                // Selection (finally)
                HandleTreeSelection(this.ItemsSource as SimpleTreeViewModel, viewModel);

                e.Handled = true;
            }
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var treeView = d as SimpleTreeView;

            if (treeView != null)
            {
                treeView.UpdateItemsSource();
            }
        }

        private void InputFileExpanderButton_Checked(object sender, RoutedEventArgs e)
        {
            var toggleButton = sender as ToggleButton;

            // Have to force update of the template
            if (toggleButton != null)
            {
                var selector = toggleButton.ContentTemplateSelector;

                toggleButton.ContentTemplateSelector = null;
                toggleButton.ContentTemplateSelector = selector;
            }
        }

        private void InputFileExpanderButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var toggleButton = sender as ToggleButton;

            // Have to force update of the template
            if (toggleButton != null)
            {
                var selector = toggleButton.ContentTemplateSelector;

                toggleButton.ContentTemplateSelector = null;
                toggleButton.ContentTemplateSelector = selector;
            }
        }
    }
}
