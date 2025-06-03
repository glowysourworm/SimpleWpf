using System.Windows;

namespace SimpleWpf.IocFramework.RegionManagement.Wpf
{
    public abstract class Transition
    {
        public virtual void BeginTransition(IocRegion transitionElement, FrameworkElement oldContent, FrameworkElement newContent)
        {

        }
        public virtual void EndTransition(IocRegion transitionElement, FrameworkElement oldContent, FrameworkElement newContent)
        {

        }
    }
}
