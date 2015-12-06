using Autofac;
using LighTake.Infrastructure.Common;

namespace LMS.WinForm.InversionOfControl.Autofac
{
    public interface IDependencyRegistrar
    {
        void Register(ContainerBuilder builder, ITypeFinder typeFinder);

        int Order { get; }
    }
}
