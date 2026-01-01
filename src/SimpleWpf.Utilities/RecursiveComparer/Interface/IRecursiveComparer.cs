namespace SimpleWpf.Utilities.RecursiveComparer.Interface
{
    public interface IRecursiveComparer
    {
        bool Compare<T>(T object1, T object2);
    }
}