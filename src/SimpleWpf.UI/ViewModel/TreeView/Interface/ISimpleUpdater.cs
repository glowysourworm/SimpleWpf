namespace SimpleWpf.UI.ViewModel.TreeView.Interface
{
    /// <summary>
    /// Interface (concept) for Begin / End update pattern for view models, complex notifiers, and observable collections (binding performance).
    /// </summary>
    public interface ISimpleUpdater
    {
        void BeginUpdate();
        void EndUpdate();
        bool IsUpdating();
    }
}
