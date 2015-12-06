using System; 
using Autofac;
using LMS.WinForm.InversionOfControl.Autofac;
using LighTake.Infrastructure.Common;

namespace LMS.WinForm.InversionOfControl
{
    public class Engine : IEngine
    {
        #region Fields

        private ContainerManager _containerManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates an instance of the content engine using default settings and configuration.
        /// </summary>
        public Engine()
            : this(new ContainerConfigurer())
        {
        }

        public Engine(ContainerConfigurer configurer)
        {
            InitializeContainer(configurer);
        }

        #endregion

        #region Utilities

        private void InitializeContainer(ContainerConfigurer configurer)
        {
            var builder = new ContainerBuilder();

            _containerManager = new ContainerManager(builder.Build());
            configurer.Configure(this, _containerManager);
        }

        #endregion


        #region Methods

        /// <summary>
        /// Initialize components and plugins in the nop environment.
        /// </summary>
        public void Initialize()
        {

        }

        public T Resolve<T>() where T : class
        {
            return ContainerManager.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        public Array ResolveAll(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

        #endregion

        #region Properties

        public IContainer Container
        {
            get { return _containerManager.Container; }
        }

        public ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion
    }
}
