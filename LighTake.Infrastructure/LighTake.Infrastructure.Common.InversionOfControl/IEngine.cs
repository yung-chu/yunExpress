using System;
using LighTake.Infrastructure.Common.InversionOfControl.Auotfac;

namespace LighTake.Infrastructure.Common.InversionOfControl
{
    public interface IEngine
    {
        ContainerManager ContainerManager { get; }

        /// <summary>
        /// Initialize components and plugins in the nop environment.
        /// </summary>
        /// <param name="config">Config</param>
        void Initialize();

        T Resolve<T>() where T : class;

        object Resolve(Type type);

        Array ResolveAll(Type serviceType);

        T[] ResolveAll<T>();
    }
}
