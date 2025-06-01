using SimpleWpf.SimpleCollections.Collection.Interface;

namespace SimpleWpf.SimpleCollections.Collection
{
    /// <summary>
    /// Delegate to notify listener about change of hash code
    /// </summary>
    public delegate void NotifyHashCodeChanged(INotifyDictionaryKey sender, int oldHashCode, int newHashCode);
}
