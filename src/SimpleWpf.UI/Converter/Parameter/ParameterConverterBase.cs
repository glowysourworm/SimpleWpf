using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SimpleWpf.UI.Converter
{
    public enum ComparisonOperation
    {
        Equality = 0,
        InEquality,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    public class ParameterConverterBase : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty TrueValueProperty =
            DependencyProperty.Register("TrueValue", typeof(object), typeof(ParameterConverterBase));

        public static readonly DependencyProperty FalseValueProperty =
            DependencyProperty.Register("FalseValue", typeof(object), typeof(ParameterConverterBase));

        public static readonly DependencyProperty ComparisonProperty =
            DependencyProperty.Register("Comparison", typeof(ComparisonOperation), typeof(ParameterConverterBase));

        public object TrueValue
        {
            get { return (object)GetValue(TrueValueProperty); }
            set { SetValue(TrueValueProperty, value); }
        }
        public object FalseValue
        {
            get { return (object)GetValue(FalseValueProperty); }
            set { SetValue(FalseValueProperty, value); }
        }
        public ComparisonOperation Comparison
        {
            get { return (ComparisonOperation)GetValue(ComparisonProperty); }
            set { SetValue(ComparisonProperty, value); }
        }

        public ParameterConverterBase()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null ||
                parameter == null)
                return Binding.DoNothing;

            // NOTE:  Trying (double) [is assignable to / from] (int) failed. AND, there
            //        was no way to store a double resource value for the parameter.
            //

            double valueDouble = 0;
            double parameterDouble = 0;

            try
            {
                valueDouble = System.Convert.ToDouble(value);
                parameterDouble = System.Convert.ToDouble(parameter);
            }
            catch (Exception)
            {
                return Binding.DoNothing;
            }

            var comparison = false;

            switch (this.Comparison)
            {
                case ComparisonOperation.Equality:
                    comparison = valueDouble == parameterDouble;
                    break;
                case ComparisonOperation.InEquality:
                    comparison = valueDouble != parameterDouble;
                    break;
                case ComparisonOperation.GreaterThan:
                    comparison = valueDouble > parameterDouble;
                    break;
                case ComparisonOperation.GreaterThanOrEqual:
                    comparison = valueDouble >= parameterDouble;
                    break;
                case ComparisonOperation.LessThan:
                    comparison = valueDouble < parameterDouble;
                    break;
                case ComparisonOperation.LessThanOrEqual:
                    comparison = valueDouble <= parameterDouble;
                    break;
                default:
                    throw new Exception("Unhandled comparison type");
            }

            return comparison ? this.TrueValue : this.FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("No two-way binding will work for this converter");
        }
    }
}
