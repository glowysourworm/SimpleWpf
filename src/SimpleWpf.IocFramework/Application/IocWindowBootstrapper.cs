using System.Windows;

using SimpleWpf.IocFramework.Application.IocException;

namespace SimpleWpf.IocFramework.Application
{
    public abstract class IocWindowBootstrapper : IocBootstrapper
    {
        bool _windowBeforeRun;

        public IocWindowBootstrapper(bool windowBeforeRun, bool runIsAsync) : base(runIsAsync)
        {
            _windowBeforeRun = windowBeforeRun;
        }

        /// <summary>
        /// Defines type for the shell window to be created
        /// </summary>
        public abstract Type DefineShell();

        /// <summary>
        /// Set runIsAsync to true to use RunAsync method of the ModuleBase, and the bootstrapper
        /// </summary>
        /// <param name="runIsAsync"></param>
        public IocWindowBootstrapper(bool runIsAsync) : base(runIsAsync) { }

        protected override void UserPreModuleInitialize()
        {
            // Get the type for the user shell to be created
            var shellType = DefineShell();

            if (!typeof(Window).IsAssignableFrom(shellType))
                throw new IocInitializationException("Improper Shell Type {0}. All module types must inherit from Window", shellType.FullName);

            var shell = (Window)IocContainer.Get(shellType);

            // SET SHELL THE MAIN WINDOW OF THE APPLICATION
            System.Windows.Application.Current.MainWindow = shell;
        }

        public override void Run()
        {
            // This matters during dialog related initialization, typically
            //
            if (_windowBeforeRun)
                System.Windows.Application.Current.MainWindow.Show();

            // User may need to attach dialog window to main window. Other WPF related
            // UI issues may be involved with initializing the UI and handling data binding.
            //
            base.Run();

            if (!_windowBeforeRun)
                System.Windows.Application.Current.MainWindow.Show();
        }
    }
}
