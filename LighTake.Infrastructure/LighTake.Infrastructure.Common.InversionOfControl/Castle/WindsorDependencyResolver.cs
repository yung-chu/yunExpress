using System;
using System.Collections.Generic;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace LighTake.Infrastructure.Common.InversionOfControl.Castle
{
    /// <summary>
    /// 封装Castle.Windsor容器 用于实现依赖注入操作
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年07月20日
    /// 修改历史 : 无
    /// </remarks>
    public class WindsorDependencyResolver : DisposableResource, IDependencyResolver
    {
        private readonly IWindsorContainer _container;

        public WindsorDependencyResolver()
            : this(new WindsorContainer("Configuration\\Windsor.config"))
        {
            
        }

        public WindsorDependencyResolver(string configPath)
            : this(new WindsorContainer(new XmlInterpreter(configPath)))
        {

        }

        public WindsorDependencyResolver(IWindsorContainer container)
        {
            Check.Argument.IsNotNull(container, "container");

            _container = container;
        }

        #region IDependencyResolver 成员

        /// <summary>
        /// 向容器中注册一个实例
        /// </summary>
        /// <param name="instance">类型T的实例</param>
        public void Register<T>(T instance)
        {
            _container.AddComponent<T>();
        }

        /// <summary>
        /// 通过容器注入一个已经存在的对象
        /// </summary>
        /// <typeparam name="T">类型T</typeparam>
        /// <param name="existing">类型T的实例</param>
        public void Inject<T>(T existing)
        {
            _container.AddComponent<T>();
        }

        /// <summary>
        /// 解析并返回容器中指定类型的实例
        /// </summary>
        /// <param name="type">指定类型</param>
        /// <returns>返回容器中指定类型的实例</returns>
        public T Resolve<T>(Type type)
        {
            Check.Argument.IsNotNull(type, "type");

            return _container.Resolve<T>(type);

        }

        /// <summary>
        /// 解析并返回容器中指定类型的实例
        /// </summary>
        /// <param name="type">指定类型</param>
        /// <param name="name">指定名称</param>
        /// <returns>返回容器中指定类型的实例</returns>
        public T Resolve<T>(Type type, string name)
        {
            Check.Argument.IsNotNull(type, "type");

            Check.Argument.IsNotEmpty(name, "name");

            return _container.Resolve<T>(name, type);
        }

        /// <summary>
        /// 解析并返回容器中范型T的默认实例
        /// </summary>
        /// <returns>返回容器中范型T的默认实例</returns>
        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        /// <summary>
        /// 解析并返回容器中范型T的默认实例
        /// </summary>
        /// <param name="name">指定名称</param>
        /// <returns>返回容器中范型T的默认实例</returns>
        public T Resolve<T>(string name)
        {
            Check.Argument.IsNotEmpty(name, "name");

            return _container.Resolve<T>(name);
        }

        /// <summary>
        /// 解析并返回容器中注册的所有类型为T的实例
        /// </summary>
        /// <returns>返回容器中注册的所有类型为T的实例</returns>
        public IEnumerable<T> ResolveAll<T>()
        {
            return _container.ResolveAll<T>();
        }

        #endregion

        #region IDisposable 成员

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _container.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
