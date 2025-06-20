using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace SimpleWpf.Extensions.BindingExtension
{
    [MarkupExtensionReturnType(typeof(Enum))]
    public class EnumExtension : MarkupExtension
    {
        public Type EnumType { get; set; }
        public Enum EnumValue { get; set; }

        public EnumExtension(Type enumType, string enumString) : base()
        {
            this.EnumType = enumType;

            object enumValue = null;
            if (Enum.TryParse(enumType, enumString, out enumValue))
                this.EnumValue = (Enum)enumValue;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this.EnumValue;
        }
    }
}
