using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using LighTake.Infrastructure.Common.InversionOfControl;

namespace LighTake.LMS.Web.Framework.Engine
{
    public class GrouponDependencyResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            try
            {
                return EngineContext.Current.Resolve(serviceType);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                var type = typeof(IEnumerable<>).MakeGenericType(serviceType);
                return (IEnumerable<object>)EngineContext.Current.Resolve(type);
            }
            catch
            {
                return null;
            }

        }
    }
}
