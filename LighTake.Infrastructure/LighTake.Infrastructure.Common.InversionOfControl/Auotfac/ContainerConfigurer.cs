using System;
using System.Linq; 

namespace LighTake.Infrastructure.Common.InversionOfControl.Auotfac
{
    public class ContainerConfigurer
    {
        public virtual void Configure(IEngine engine, ContainerManager containerManager, EventBroker broker)
        {
            containerManager.AddComponentInstance<IEngine>(engine, "groupon.engine");
            containerManager.AddComponentInstance<ContainerConfigurer>(this, "groupon.containerConfigurer");
            //type finder
            containerManager.AddComponent<ITypeFinder, WebAppTypeFinder>("groupon.typeFinder");

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

            //event broker
            containerManager.AddComponentInstance(broker);
        }
    }
}
