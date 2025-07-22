using SimpleWpf.Extensions.Interface;

namespace SimpleWpf.Extensions
{
    public abstract class ValidationViewModelBase : ViewModelBase
    {
        public abstract bool Validate(IValidationContext context);
    }
}
