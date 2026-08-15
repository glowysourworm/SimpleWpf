
namespace SimpleWpf.IocFramework.EventAggregation
{
    public interface IIocEventAggregator
    {
        TEventType GetEvent<TEventType>() where TEventType : IocEventBase;

        void Exhaust<TEventType>(int timeoutMilliseconds = 1000) where TEventType : IocEventBase;
    }
}
