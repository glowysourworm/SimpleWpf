using SimpleWpf.Extensions.Collection;
using SimpleWpf.IocFramework.Application.IocException;

namespace SimpleWpf.IocFramework.Application
{
    public abstract class IocBootstrapper
    {
        readonly bool _runIsAsync;

        // Modules for the application
        IEnumerable<ModuleInstance> _modules;

        // Initialized flag
        bool _initialized = false;

        /// <summary>
        /// Primary constructor for the base bootstrapper. This will choose a run method to
        /// execute as the last step:  Run, or RunAsync.
        /// </summary>
        public IocBootstrapper(bool runIsAsync)
        {
            _runIsAsync = runIsAsync;
        }

        /// <summary>
        /// Defines module types for the application. This will tell the bootstrapper where
        /// to look for loading exported types into the IIocContainer; and also boost reflection
        /// performance while loading by narrowing the searches to those assemblies.
        /// </summary>
        /// <returns>Collection of ModuleBase instances that mark modules for the application</returns>
        public abstract IEnumerable<ModuleDefinition> DefineModules();

        private void InitializeContainer()
        {
            // Call user code to define the modules
            var definitions = DefineModules();

            // VALIDATE MODULE AND SHELL TYPES
            foreach (var definition in definitions)
            {
                if (!typeof(ModuleBase).IsAssignableFrom(definition.ModuleType))
                    throw new IocInitializationException("Improper Module Type {0}. All module types must inherit from ModuleBase", definition.ModuleType.FullName);
            }

            // Get array of loaded assemblies via the modules. (BY DESIGN!)
            var assemblies = definitions.Select(definition => definition.ModuleType.Assembly).ToList();

            // ADD THIS ASSEMBLY FOR THE DEFAULT COMPONENTS OF THE IocFramework:  Region Manager, Event Aggregator
            assemblies.Add(typeof(IocBootstrapper).Assembly);

            // Load the IocContainer
            IocContainer.Initialize(assemblies);

            // CONTAINER IS READY! :)

            // (Go ahead and just do this here..) Create modules - uses exports from IocContainer
            _modules = definitions.Select(definition => new ModuleInstance((ModuleBase)IocContainer.Get(definition.ModuleType), definition))
                                  .Actualize();
        }

        private void InitializeModules()
        {
            if (_modules == null)
                throw new IocInitializationException("Trying to call InitializeModules before InitializeContainer has been called");

            // Initialize the modules
            foreach (var module in _modules)
                module.Instance.Initialize();
        }

        /// <summary>
        /// User defined initialize method. Run after container is built; and just to PRIOR to calling ModuleBase.Initialize().
        /// </summary>
        protected virtual void UserPreModuleInitialize() { }

        /// <summary>
        /// User defined initialize method. Run after container is built; and just to AFTER to calling ModuleBase.Initialize().
        /// </summary>
        protected virtual void UserPostModuleInitialize() { }

        public void Initialize()
        {
            InitializeContainer();

            UserPreModuleInitialize();

            InitializeModules();

            UserPostModuleInitialize();

            _initialized = true;
        }

        /// <summary>
        /// Calls the ModuleBase.Run(..) entry point
        /// </summary>
        public virtual void Run()
        {
            if (!_initialized)
                throw new IocInitializationException("Must call IocBootstrapper.Initialize() before IocBootstrapper.Run()");

            if (!_modules.Any(x => x.Definition.IsEntryPoint))
                throw new IocInitializationException("No entry point defined for any of the ModuleBase instances");

            var definition = _modules.FirstOrDefault(x => x.Definition.IsEntryPoint);

            // START THE APPLICATION!
            if (_runIsAsync)
            {
                definition.Instance
                          .RunAsync()
                          .ConfigureAwait(false)
                          .GetAwaiter()
                          .GetResult();
            }
            else
            {
                definition.Instance.Run();
            }
        }
    }
}
