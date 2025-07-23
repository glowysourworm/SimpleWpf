using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace SimpleWpf.UI.ValidationRules
{
    public class StringNullOrWhiteSpaceValidationRule : ValidationRuleBase
    {
        string _valueName;

        public string ValueName
        {
            get { return _valueName; }
            set { this.RaiseAndSetIfChanged(ref _valueName, value); }
        }


        public StringNullOrWhiteSpaceValidationRule() 
        {
            ValueName = "Value";
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrWhiteSpace((value as string)))
                return new ValidationResult(false, this.ValueName + " must not be empty or white space");

            else
                return ValidationResult.ValidResult;
        }
    }
}
