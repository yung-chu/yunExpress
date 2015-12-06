using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace LighTake.Infrastructure.Common
{
    public static class WCFExtension
    {
        public static void Using<T>(T serviceClient, Action<T> action) where T : ICommunicationObject
        {
            try
            {
                action(serviceClient);
                serviceClient.Close();
            }
            catch (CommunicationException)
            {
                serviceClient.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                serviceClient.Abort();
                throw;
            }
            catch (Exception)
            {
                serviceClient.Abort();
                throw;
            }
        }
    }
}
