using System.Windows;
using System.Windows.Controls;

using SimpleWpf.UI.ViewModel.EnumUI;

namespace SimpleWpf.UI.Controls.EnumUI
{
    public partial class SimpleEnumComboBox : UserControl
    {
        public static readonly DependencyProperty EnumTypeProperty = DependencyProperty.Register(
            "EnumType",
            typeof(Type),
            typeof(SimpleEnumComboBox));

        public static readonly DependencyProperty EnumValueProperty = DependencyProperty.Register(
            "EnumValue",
            typeof(object),
            typeof(SimpleEnumComboBox),
            new PropertyMetadata(OnEnumValueChanged));

        public static readonly RoutedEvent EnumValueChangedEvent = EventManager.RegisterRoutedEvent(
            "EnumValueChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SimpleEnumComboBox));

        public Type EnumType
        {
            get { return (Type)GetValue(EnumTypeProperty); }
            set { SetValue(EnumTypeProperty, value); }
        }
        public object EnumValue
        {
            get { return GetValue(EnumValueProperty); }
            set { SetValue(EnumValueProperty, value); }
        }
        public event RoutedEventHandler EnumValueChanged
        {
            add { AddHandler(EnumValueChangedEvent, value); }
            remove { RemoveHandler(EnumValueChangedEvent, value); }
        }

        public SimpleEnumComboBox()
        {
            InitializeComponent();
        }

        private static void OnEnumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SimpleEnumComboBox;
            var collection = control.TheComboBox.ItemsSource as IEnumerable<EnumItemViewModel>;

            if (control != null && collection != null && e.NewValue != null)
            {
                // Selected Item (loops with selected index changed, should run twice)
                //
                // NOTE*** Enum equality requires .Equals method
                //
                var item = collection.FirstOrDefault(x => x.Value.Equals(e.NewValue));

                if (control.TheComboBox.SelectedItem != item)
                    control.TheComboBox.SelectedItem = item;

                // Raise Event for listeners
                control.RaiseEvent(new RoutedEventArgs(EnumValueChangedEvent, control));
            }
        }

        private void TheComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = this.TheComboBox.SelectedItem as EnumItemViewModel;

            if (selectedItem != null)
            {
                this.EnumValue = selectedItem.Value;
            }
        }
    }
}

