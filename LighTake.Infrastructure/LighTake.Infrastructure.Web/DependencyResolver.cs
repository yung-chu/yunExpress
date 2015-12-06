using System;
using System.Collections.Generic;
using System.Web.Mvc;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Common.Logging;


namespace LighTake.Infrastructure.Web
{
    /// <summary>
    /// MVCDependencyResolver
    /// update by daniel 2014-12-12 , throw Exception in DEBUG
    /// </summary>
    public class MVCDependencyResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            try
            {
                return EngineContext.Current.Resolve(serviceType);
            }
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
#if DEBUG
                //throw ex;
#endif
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
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
#if DEBUG
                //throw ex;         
#endif
                return null;
            } 
        }
    }
}
