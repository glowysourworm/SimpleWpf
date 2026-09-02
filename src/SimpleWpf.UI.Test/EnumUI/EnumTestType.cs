using System.ComponentModel.DataAnnotations;

namespace SimpleWpf.UI.Test.EnumUI
{
    [Flags]
    public enum EnumTestType
    {
        [Display(Name = "Value 1", Description = "Test Value 1")]
        Value1 = 1,

        [Display(Name = "Value 2", Description = "Test Value 2")]
        Value2 = 2,

        [Display(Name = "Value 3", Description = "Test Value 3")]
        Value3 = 4,

        [Display(Name = "Value 4", Description = "Test Value 4")]
        Value4 = 8
    }
}
