using System;
using System.Collections.Generic;

namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// IBidirectionalMapping 双向映射。即Key和Value都是唯一的，在这种情况下使用IBidirectionalMapping可提升依据Value查找Key的速度。
    /// 该接口的实现必须是线程安全的。2008.08.20
    /// </summary>    
    public interface IBidirectionalMapping<T1, T2>
    {
        int Count { get; }

        /// <summary>
        /// Add 添加映射对。如果已经有相同的key/value存在，则会覆盖。
        /// </summary>       
        void Add(T1 t1, T2 t2);

        void RemoveByT1(T1 t1);
        void RemoveByT2(T2 t2);

        T1 GetT1(T2 t2);
        T2 GetT2(T1 t1);

        bool ContainsT1(T1 t1);
        bool ContainsT2(T2 t2);

        /// <summary>
        /// GetAllT1ListCopy 返回T1类型元素列表的拷贝。
        /// </summary>       
        IList<T1> GetAllT1ListCopy();

        /// <summary>
        /// GetAllT2ListCopy 返回T2类型元素列表的拷贝。
        /// </summary>    
        IList<T2> GetAllT2ListCopy();      

        void Clear();
    }
}
