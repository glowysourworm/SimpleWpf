using SimpleWpf.Extensions.Interface;

namespace SimpleWpf.UI.ViewModel
{
    public abstract class ValidationViewModelBase : ViewModelBase
    {
        public abstract bool Validate(IValidationContext context);
    }
}
