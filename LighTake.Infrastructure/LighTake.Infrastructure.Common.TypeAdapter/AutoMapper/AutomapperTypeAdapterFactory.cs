using System;
using System.Linq;
using AutoMapper;

namespace LighTake.Infrastructure.Common.TypeAdapter.AutoMapper
{
    public class AutomapperTypeAdapterFactory
        :ITypeAdapterFactory
    {
        #region Constructor

        /// <summary>
        /// Create a new Automapper type adapter factory
        /// </summary>
        public AutomapperTypeAdapterFactory()
        {
            //scan all assemblies finding Automapper Profile
            ITypeFinder finder = new AppDomainTypeFinder();
            
            var profiles = finder.GetAssemblies()
                                    .SelectMany(a => a.GetTypes())
                                    .Where(t => t.BaseType == typeof(Profile));

            Mapper.Initialize(cfg =>
            {
                foreach (var item in profiles)
                {
                    if (item.FullName != "AutoMapper.SelfProfiler`2")
                        cfg.AddProfile(Activator.CreateInstance(item) as Profile);
                } 
            });

           
        }

        #endregion

        #region ITypeAdapterFactory Members

        public ITypeAdapter Create()
        {
            return new AutomapperTypeAdapter();
        }

        #endregion
    }
}
