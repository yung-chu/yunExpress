using System;
using System.Collections.Generic;

namespace LighTake.Infrastructure.Common.InversionOfControl.Castle
{
    /// <summary>
    /// 通用依赖注入解析器
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年7月20日
    /// 修改历史 : 无
    /// </remarks>
    public interface IDependencyResolver : IDisposable
    {
        /// <summary>
        /// 向容器中注册一个实例
        /// </summary>
        /// <param name="instance">类型T的实例</param>
        void Register<T>(T instance);

        /// <summary>
        /// 通过容器注入一个已经存在的对象
        /// </summary>
        /// <typeparam name="T">类型T</typeparam>
        /// <param name="existing">类型T的实例</param>
        void Inject<T>(T existing);

        /// <summary>
        /// 解析并返回容器中指定类型的实例
        /// </summary>
        /// <param name="type">指定类型</param>
        /// <returns>返回容器中指定类型的实例</returns>
        T Resolve<T>(Type type);

        /// <summary>
        /// 解析并返回容器中指定类型的实例
        /// </summary>
        /// <param name="type">指定类型</param>
        /// <param name="name">指定名称</param>
        /// <returns>返回容器中指定类型的实例</returns>
        T Resolve<T>(Type type, string name);

        /// <summary>
        /// 解析并返回容器中范型T的默认实例
        /// </summary>
        /// <returns>返回容器中范型T的默认实例</returns>
        T Resolve<T>();

        /// <summary>
        /// 解析并返回容器中范型T的默认实例
        /// </summary>
        /// <param name="name">指定名称</param>
        /// <returns>返回容器中范型T的默认实例</returns>
        T Resolve<T>(string name);

        /// <summary>
        /// 解析并返回容器中注册的所有类型为T的实例
        /// </summary>
        /// <returns>返回容器中注册的所有类型为T的实例</returns>
        IEnumerable<T> ResolveAll<T>();
    }
}
