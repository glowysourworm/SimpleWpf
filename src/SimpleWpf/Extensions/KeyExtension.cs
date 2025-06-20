using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleWpf.Extensions
{
    public static class KeyExtension
    {
        public static bool IsModifier(this Key key)
        {
            switch (key)
            {
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LeftAlt:
                case Key.RightAlt:
                    return true;
                default:
                    return false;
            }
        }
    }
}
