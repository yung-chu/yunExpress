using System;
using System.Linq;
using LighTake.Infrastructure.Common;

namespace LMS.WinForm.InversionOfControl.Autofac
{
    public class ContainerConfigurer
    {
        public virtual void Configure(IEngine engine, ContainerManager containerManager)
        {
            containerManager.AddComponentInstance<IEngine>(engine, "WinForm.engine");
            containerManager.AddComponentInstance<ContainerConfigurer>(this, "WinForm.containerConfigurer");
            //type finder
            containerManager.AddComponent<ITypeFinder, AppTypeFinder>("WinForm.typeFinder");

            //register dependencies provided by other assemblies
            var typeFinder = containerManager.Resolve<ITypeFinder>();

            containerManager.UpdateContainer(x =>
            {
                var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
                var drInstances = drTypes.Select(drType => (IDependencyRegistrar) Activator.CreateInstance(drType)).ToList();
                //sort
                drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
                foreach (var dependencyRegistrar in drInstances)
                    dependencyRegistrar.Register(x, typeFinder);
            });
        }
    }
}
