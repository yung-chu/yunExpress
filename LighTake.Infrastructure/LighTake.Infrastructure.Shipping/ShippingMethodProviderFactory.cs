using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LighTake.Infrastructure.Common;

namespace LighTake.Infrastructure.Shipping
{
    public class ShippingMethodProviderFactory
    {
        private static Dictionary<int, IShippingProvider> _dicShippingProviders;
        private static readonly object _obj = new object();

        public static IShippingProvider CreateShippingMethod(int shippingMethodId)
        {
            if (_dicShippingProviders == null)
            {
                lock (_obj)
                {
                    if (_dicShippingProviders == null)
                    {
                        Dictionary<int, IShippingProvider> dicShippingProviders = new Dictionary<int, IShippingProvider>();
                        IShippingProvider tmpShippingMethodProvider = null;
                        foreach (Type item in Assembly.GetExecutingAssembly().GetTypes())
                        {
                            Type[] arrInterface = item.GetInterfaces();
                            foreach (Type type in arrInterface)
                            {
                                if (type == typeof(IShippingProvider) && !item.IsAbstract && item != typeof(DefaultShippingProvider))
                                {
                                    try
                                    {
                                        tmpShippingMethodProvider = Activator.CreateInstance(item) as IShippingProvider;
                                        if (!dicShippingProviders.Keys.Contains(tmpShippingMethodProvider.ShippingMethodId))
                                        {
                                            dicShippingProviders.Add(tmpShippingMethodProvider.ShippingMethodId, tmpShippingMethodProvider);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("ShippingMethodProviderFactory.CreateShippingMethod", ex);
                                    }
                                }
                            }

                        }
                        _dicShippingProviders = dicShippingProviders;
                    }
                }
            }
            if (_dicShippingProviders.Keys.Contains(shippingMethodId))
            {
                return _dicShippingProviders[shippingMethodId];
            }

            return new DefaultShippingProvider(shippingMethodId);
        }
    }
}
