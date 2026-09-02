using System.Windows;
using System.Windows.Controls;

using SimpleWpf.UI.ViewModel.EnumUI;

namespace SimpleWpf.UI.Controls.EnumUI
{
    public partial class SimpleEnumRadioButtons : UserControl
    {
        public static readonly DependencyProperty EnumTypeProperty =
            DependencyProperty.Register("EnumType",
                                        typeof(Type),
                                        typeof(SimpleEnumRadioButtons));

        public static readonly DependencyProperty EnumValueProperty =
            DependencyProperty.Register("EnumValue",
                                        typeof(object),
                                        typeof(SimpleEnumRadioButtons),
                                        new PropertyMetadata(OnEnumValueChanged));

        public static readonly RoutedEvent EnumValueChangedEvent = EventManager.RegisterRoutedEvent(
                "EnumValueChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(SimpleEnumRadioButtons));

        public Type EnumType
        {
            get { return (Type)GetValue(EnumTypeProperty); }
            set { SetValue(EnumTypeProperty, value); }
        }

        public object EnumValue
        {
            get { return (object)GetValue(EnumValueProperty); }
            set { SetValue(EnumValueProperty, value); }
        }

        public event RoutedEventHandler EnumValueChanged
        {
            add { AddHandler(EnumValueChangedEvent, value); }
            remove { RemoveHandler(EnumValueChangedEvent, value); }
        }

        public SimpleEnumRadioButtons()
        {
            InitializeComponent();
        }

        private static void OnEnumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SimpleEnumRadioButtons;
            var collection = control.EnumList.ItemsSource as IEnumerable<EnumItemViewModel>;

            if (control != null && collection != null)
            {
                // Selected Item (loops with selected index changed, should run twice)
                //
                // Note*** .Equals is required for Enum value comparison
                //
                collection.FirstOrDefault(x => x.Value.Equals(e.NewValue))?.IsChecked = true;

                // Raise Event for listeners
                control.RaiseEvent(new RoutedEventArgs(EnumValueChangedEvent, control));
            }
        }

        private void TheComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var collection = this.EnumList.ItemsSource as IEnumerable<EnumItemViewModel>;

            if (collection != null)
            {
                this.EnumValue = collection.FirstOrDefault(x => x.IsChecked)?.Value;
            }
        }
    }
}
