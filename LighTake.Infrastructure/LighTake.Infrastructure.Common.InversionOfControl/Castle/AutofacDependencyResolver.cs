using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using LighTake.Infrastructure.Common.InversionOfControl.Auotfac;
 

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
    public class AutofacDependencyResolver : IDependencyResolver
    {
        #region Fields

        private readonly ContainerManager _containerManager;

        #endregion

        public AutofacDependencyResolver(string configPath)
        {
            var builder = new ContainerBuilder();

            _containerManager = new ContainerManager(builder.Build()); 

            //register dependencies provided by other assemblies
            var typeFinder = _containerManager.Resolve<ITypeFinder>();
            _containerManager.UpdateContainer(x =>
            {
                var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
                var drInstances = drTypes.Select(drType => (IDependencyRegistrar) Activator.CreateInstance(drType)).ToList();
                //sort
                drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
                foreach (var dependencyRegistrar in drInstances)
                    dependencyRegistrar.Register(x, typeFinder);
            });

           

        }

        #region IDependencyResolver 成员

        /// <summary>
        /// 向容器中注册一个实例
        /// </summary>
        /// <param name="instance">类型T的实例</param>
        public void Register<T>(T instance)
        {
            _containerManager.AddComponentInstance(instance.GetType(), instance);
        }

        /// <summary>
        /// 通过容器注入一个已经存在的对象
        /// </summary>
        /// <typeparam name="T">类型T</typeparam>
        /// <param name="existing">类型T的实例</param>
        public void Inject<T>(T existing)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 解析并返回容器中指定类型的实例
        /// </summary>
        /// <param name="type">指定类型</param>
        /// <returns>返回容器中指定类型的实例</returns>
        public T Resolve<T>(Type type)
        {
            Check.Argument.IsNotNull(type, "type");

            return (T)ContainerManager.Resolve(type);

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

            return ContainerManager.Resolve<T>(name);
        }

        /// <summary>
        /// 解析并返回容器中范型T的默认实例
        /// </summary>
        /// <returns>返回容器中范型T的默认实例</returns>
        public T Resolve<T>()
        {
            return ContainerManager.Resolve<T>();
        }

        /// <summary>
        /// 解析并返回容器中范型T的默认实例
        /// </summary>
        /// <param name="name">指定名称</param>
        /// <returns>返回容器中范型T的默认实例</returns>
        public T Resolve<T>(string name)
        {
            Check.Argument.IsNotEmpty(name, "name");

            return ContainerManager.Resolve<T>(name);
        }

        /// <summary>
        /// 解析并返回容器中注册的所有类型为T的实例
        /// </summary>
        /// <returns>返回容器中注册的所有类型为T的实例</returns>
        public IEnumerable<T> ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

        #endregion

        #region Properties

        public IContainer Container
        {
            get { return _containerManager.Container; }
        }

        public ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion

        public void Dispose()
        {
            
        }
    }
}
