using Autofac;

namespace LighTake.Infrastructure.Common.InversionOfControl.Auotfac
{
    public interface IDependencyRegistrar
    {
        void Register(ContainerBuilder builder, ITypeFinder typeFinder);

        int Order { get; }
    }
}
