﻿namespace SimpleWpf.IocFramework.WindowManagement.Interface
{
    /// <summary>
    /// Component to manage modal / non-modal dialogs.
    /// </summary>
    public interface IIocWindowManager
    {
        bool ShowDialog<T>(object dataContext) where T : IIocDialogView;
        bool ShowDialog<T>(T dialogView) where T : IIocDialogView;
    }
}
