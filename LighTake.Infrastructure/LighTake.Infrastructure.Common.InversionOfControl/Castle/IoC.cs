using System;
using System.Collections.Generic;

namespace LighTake.Infrastructure.Common.InversionOfControl.Castle
{
    /// <summary>
    /// 通用的依赖注入操作
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年07月20日
    /// 修改历史 : 无
    /// </remarks>
    public static class IoC
    {
        private static IDependencyResolver _resolver;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="factory">依赖解析器创建工厂</param>
        public static void InitializeWith(IDependencyResolverFactory factory)
        {
            Check.Argument.IsNotNull(factory, "factory");

            _resolver = factory.CreateInstance();
        }

        /// <summary>
        /// 向容器中注册一个实例
        /// </summary>
        /// <param name="instance">类型T的实例</param>
        public static void Register<T>(T instance)
        {
            Check.Argument.IsNotNull(instance, "instance");

            _resolver.Register(instance);
        }

        /// <summary>
        /// 通过容器注入一个已经存在的对象
        /// </summary>
        /// <typeparam name="T">类型T</typeparam>
        /// <param name="existing">类型T的实例</param>
        public static void Inject<T>(T existing)
        {
            Check.Argument.IsNotNull(existing, "existing");

            _resolver.Inject(existing);
        }

        /// <summary>
        /// 解析并返回容器中指定类型的实例
        /// </summary>
        /// <param name="type">指定类型</param>
        /// <returns>返回容器中指定类型的实例</returns>
        public static T Resolve<T>(Type type)
        {
            Check.Argument.IsNotNull(type, "type");

            return _resolver.Resolve<T>(type);
        }

        /// <summary>
        /// 解析并返回容器中指定类型的实例
        /// </summary>
        /// <param name="type">指定类型</param>
        /// <param name="name">指定名称</param>
        /// <returns>返回容器中指定类型的实例</returns>
        public static T Resolve<T>(Type type, string name)
        {
            Check.Argument.IsNotNull(type, "type");
            Check.Argument.IsNotEmpty(name, "name");

            return _resolver.Resolve<T>(type, name);
        }

        /// <summary>
        /// 解析并返回容器中范型T的默认实例
        /// </summary>
        /// <returns>返回容器中范型T的默认实例</returns>
        public static T Resolve<T>()
        {
            return _resolver.Resolve<T>();
        }

        /// <summary>
        /// 解析并返回容器中范型T的默认实例
        /// </summary>
        /// <param name="name">指定名称</param>
        /// <returns>返回容器中范型T的默认实例</returns>
        public static T Resolve<T>(string name)
        {
            Check.Argument.IsNotEmpty(name, "name");

            return _resolver.Resolve<T>(name);
        }

        /// <summary>
        /// 解析并返回容器中注册的所有类型为T的实例
        /// </summary>
        /// <returns>返回容器中注册的所有类型为T的实例</returns>
        public static IEnumerable<T> ResolveAll<T>()
        {
            return _resolver.ResolveAll<T>();
        }

        /// <summary>
        /// 重置容器
        /// </summary>
        public static void Reset()
        {
            if (_resolver != null)
            {
                _resolver.Dispose();
            }
        }
    }
}
