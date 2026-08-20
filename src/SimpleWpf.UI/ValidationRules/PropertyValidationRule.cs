using System.Globalization;
using System.Windows.Controls;

namespace SimpleWpf.UI.ValidationRules
{
    public class PropertyValidationRule : ValidationRuleBase
    {
        public enum PropertyValidationType
        {
            IsNotNull = 0,
            IsNull = 1
        }

        string _propertyPath;
        string _invalidText;
        PropertyValidationType _validationType;

        public string PropertyPath
        {
            get { return _propertyPath; }
            set { this.RaiseAndSetIfChanged(ref _propertyPath, value); }
        }
        public string InvalidText
        {
            get { return _invalidText; }
            set { this.RaiseAndSetIfChanged(ref _invalidText, value); }
        }
        public PropertyValidationType ValidationType
        {
            get { return _validationType; }
            set { this.RaiseAndSetIfChanged(ref _validationType, value); }
        }


        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            switch (this.ValidationType)
            {
                case PropertyValidationType.IsNotNull:
                    return value == null ? new ValidationResult(false, this.InvalidText) : ValidationResult.ValidResult;
                case PropertyValidationType.IsNull:
                    return value != null ? new ValidationResult(false, this.InvalidText) : ValidationResult.ValidResult;
                default:
                    throw new Exception("Unhandled validation type");
            }
        }
    }
}
