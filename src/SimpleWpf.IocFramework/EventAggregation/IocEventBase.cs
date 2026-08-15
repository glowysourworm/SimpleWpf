namespace SimpleWpf.IocFramework.EventAggregation
{
    /// <summary>
    /// Base (marker) class for rogue events
    /// </summary>
    public abstract class IocEventBase
    {
        /// <summary>
        /// Returns true while event actions are running
        /// </summary>
        public abstract bool IsRunning();
    }
}
