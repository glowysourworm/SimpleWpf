using SimpleWpf.IocFramework.Application.Attribute;
using SimpleWpf.SimpleCollections.Collection;

namespace SimpleWpf.IocFramework.EventAggregation
{
    [IocExport(typeof(IIocEventAggregator), InstancePolicy.ShareGlobal)]
    public class IocEventAggregator : IIocEventAggregator
    {
        readonly SimpleDictionary<Type, IocEventBase> _eventDict;

        public IocEventAggregator()
        {
            _eventDict = new SimpleDictionary<Type, IocEventBase>();
        }

        public void Exhaust<TEventType>(int timeoutMilliseconds = 1000) where TEventType : IocEventBase
        {
            var type = typeof(TEventType);
            var counter = 0;

            while (counter++ < timeoutMilliseconds && _eventDict[type].IsRunning())
            {
                Thread.Sleep(1);
            }
        }

        public TEventType GetEvent<TEventType>() where TEventType : IocEventBase
        {
            var type = typeof(TEventType);

            if (_eventDict.Keys.Any(x => x == typeof(TEventType)))
                return (TEventType)_eventDict[type];

            var newEvent = Construct<TEventType>();

            _eventDict[type] = newEvent;

            return newEvent;
        }

        private T Construct<T>()
        {
            var constructor = typeof(T).GetConstructor(new Type[] { });
            return (T)constructor.Invoke(new object[] { });
        }
    }
}
