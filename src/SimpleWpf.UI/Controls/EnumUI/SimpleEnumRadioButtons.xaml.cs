using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using SimpleWpf.Extensions.Collection;
using SimpleWpf.UI.ViewModel;

namespace SimpleWpf.UI.Controls.EnumUI
{
    public partial class SimpleEnumRadioButtons : UserControl
    {
        public static readonly DependencyProperty EnumTypeProperty =
            DependencyProperty.Register("EnumType",
                                        typeof(Type),
                                        typeof(SimpleEnumRadioButtons),
                                        new PropertyMetadata(new PropertyChangedCallback(OnEnumTypeChanged)));

        public static readonly DependencyProperty EnumValueProperty =
            DependencyProperty.Register("EnumValue",
                                        typeof(object),
                                        typeof(SimpleEnumRadioButtons),
                                        new PropertyMetadata(new PropertyChangedCallback(OnEnumValueChanged)));

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

        public class EnumItem : ViewModelBase
        {
            string _enumName;
            string _displayName;
            string _description;
            bool _isChecked;

            public string EnumName
            {
                get { return _enumName; }
                set { this.RaiseAndSetIfChanged(ref _enumName, value); }
            }
            public string DisplayName
            {
                get { return _displayName; }
                set { this.RaiseAndSetIfChanged(ref _displayName, value); }
            }
            public string Description
            {
                get { return _description; }
                set { this.RaiseAndSetIfChanged(ref _description, value); }
            }
            public bool IsChecked
            {
                get { return _isChecked; }
                set { this.RaiseAndSetIfChanged(ref _isChecked, value); }
            }
        }

        public SimpleEnumRadioButtons()
        {
            InitializeComponent();

            this.Loaded += (sender, e) => Initialize();
        }

        private void Initialize()
        {
            if (this.EnumType != null)
            {
                this.EnumList
                       .ItemsSource = this.EnumType.GetMembers()
                                          .Where(x => x.GetCustomAttributes(typeof(DisplayAttribute), false).Any())
                                          .Select(x => new EnumItem()
                                          {
                                              EnumName = x.Name,
                                              Description = x.GetCustomAttribute<DisplayAttribute>().Description,
                                              DisplayName = x.GetCustomAttribute<DisplayAttribute>().Name,
                                              IsChecked = this.EnumValue != null ? Enum.GetName(this.EnumType, this.EnumValue) == x.Name
                                                                                    : false
                                          })
                                          .Actualize();
            }
        }

        private static void OnEnumTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as SimpleEnumRadioButtons;
            var type = e.NewValue as Type;

            // Update controls for the new enum type
            if (control != null &&
                type != null &&
                type.IsEnum)
            {
                control.Initialize();
            }
        }

        private static void OnEnumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SimpleEnumRadioButtons;
            var itemsSource = control.EnumList.ItemsSource as IEnumerable<EnumItem>;
            var enumValue = e.NewValue;

            if (itemsSource != null &&
                control != null)
            {
                foreach (var item in itemsSource)
                    item.IsChecked = enumValue == null ?
                                        false :
                                        item.EnumName == Enum.GetName(control.EnumType, enumValue);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var itemsSource = this.EnumList.ItemsSource as IEnumerable<EnumItem>;
            var radioButton = sender as RadioButton;

            if (itemsSource != null &&
                radioButton != null)
            {
                // Enum Name for the selected radio button
                var enumName = radioButton.Tag.ToString();

                // Enum Type Values
                var values = Enum.GetValues(this.EnumType);

                for (int i = 0; i < values.Length; i++)
                {
                    if (Enum.GetName(this.EnumType, values.GetValue(i)) == enumName)
                        this.EnumValue = values.GetValue(i);
                }
            }
        }
    }
}
