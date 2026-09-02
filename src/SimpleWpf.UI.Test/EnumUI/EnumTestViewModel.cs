using SimpleWpf.UI.ViewModel;

namespace SimpleWpf.UI.Test.EnumUI
{
    public class EnumTestViewModel : ViewModelBase
    {
        EnumTestType _value1;
        EnumTestType _value2;
        EnumTestType _value3;

        public EnumTestType Value1
        {
            get { return _value1; }
            set { this.RaiseAndSetIfChanged(ref _value1, value); }
        }
        public EnumTestType Value2
        {
            get { return _value2; }
            set { this.RaiseAndSetIfChanged(ref _value2, value); }
        }
        public EnumTestType Value3
        {
            get { return _value3; }
            set { this.RaiseAndSetIfChanged(ref _value3, value); }
        }

        public EnumTestViewModel()
        {
            this.Value1 = EnumTestType.Value2;
            this.Value2 = EnumTestType.Value3 | EnumTestType.Value4;
            this.Value3 = EnumTestType.Value4;
        }
    }
}
