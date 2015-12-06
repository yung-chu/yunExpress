using System;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Data.Entity;

namespace LighTake.Infrastructure.Data
{
    /// <summary>
    /// 定义数据仓储公共接口
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年07月20日
    /// 修改历史 : 无
    /// </remarks>
    public interface IRepository
    {
        /// <summary>
        /// 自定义字段默认值
        /// </summary>
        Dictionary<string, object> DefaultValues { get; set; }

        /// <summary>
        /// 提供LINQ使用的可查询对象
        /// </summary>
        IQueryable<T> AsQueryable<T>() where T : class, IEntity;
        
        /// <summary>
        /// 根据主键获取类型为T的数据 如果不存在则返回null
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>返回类型为T的数据</returns>
        T Get<T>(object id) where T : class, IEntity;

        /// <summary>
        /// 根据主键加载类型为T的数据 如果不存在则返回默认实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>返回类型为T的数据</returns>
        T Load<T>(object id) where T : class, IEntity;

        /// <summary>
        /// 根据条件表达式获取满足条件的首个类型为T的数据,如果不存在则返回NULL
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns>返回满足条件的首个类型为T的数据</returns>
        T First<T>(Expression<Func<T, bool>> expression) where T : class, IEntity;

        /// <summary>
        /// 根据条件表达式获取满足条件的所有类型为T的数据
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns>返回满足条件的所有类型为T的数据</returns>
        IEnumerable<T> Find<T>(Expression<Func<T, bool>> expression) where T : class, IEntity;

        /// <summary>
        /// 根据条件表达式获取满足条件的所有类型为T的数据
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <param name="orderBy">排序表达式 如: Column1 ASC,Column2 DESC</param>
        /// <returns>返回满足条件的所有类型为T的数据</returns>
        IEnumerable<T> Find<T>(Expression<Func<T, bool>> expression, string orderBy) where T : class, IEntity;

        /// <summary>
        /// 获取指定分页类型为T的数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数据量</param>
        /// <returns>返回满足条件的所有类型为T的数据和分页信息</returns>
        IPagedList<T> FindPagedList<T>(int pageIndex, int pageSize) where T : class, IEntity;

        /// <summary>
        /// 根据条件表达式获取满足条件的所有类型为T的数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数据量</param>
        /// <param name="expression">条件表达式</param>
        /// <returns>返回满足条件的所有类型为T的数据和分页信息</returns>
        IPagedList<T> FindPagedList<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> expression) where T : class, IEntity;

        /// <summary>
        /// 根据条件表达式获取满足条件的所有类型为T的数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数据量</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="orderBy">排序表达式 如: Column1 ASC,Column2 DESC</param>
        /// <returns>返回满足条件的所有类型为T的数据和分页信息</returns>
        IPagedList<T> FindPagedList<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> expression, string orderBy) where T : class, IEntity;

        /// <summary>
        /// 根据主键 删除某个类型为T的数据
        /// </summary>
        /// <param name="item">待删除数据</param>
        int Delete<T>(object id) where T : class, IEntity;

        /// <summary>
        /// 删除某个类型为T的数据
        /// </summary>
        /// <param name="item">待删除数据</param>
        int Delete<T>(T item) where T : class, IEntity;

        /// <summary>
        /// 根据条件表达式获取满足条件的所有类型为T的数据
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns>返回受影响的行数</returns>
        int Delete<T>(Expression<Func<T, bool>> expression) where T : class, IEntity;

        /// <summary>
        /// 删除所有类型为T的数据 警告:危险!!!该操作为不可逆操作,使用前请仔细确认!
        /// </summary>
        /// <returns>返回受影响的行数</returns>
        int DeleteAll<T>() where T : class, IEntity;

        /// <summary>
        /// 保存新的类型为T的数据对象(主键ID会根据配置产生)
        /// </summary>
        /// <param name="item">待保存的对象</param>
        /// <returns>返回新对象的主键ID</returns>
        object Save<T>(T item) where T : class, IEntity;

        /// <summary>
        /// 保存或更新一个已存在的数据对象
        /// </summary>
        /// <param name="item">待保存/更新的对象</param>
        /// <returns>返回已保存的对象</returns>
        T SaveOrUpdate<T>(T item) where T : class, IEntity;

        /// <summary>
        /// 保存或更新一个已存在的数据对象
        /// </summary>
        /// <param name="item">待保存/更新的对象</param>
        /// <returns>返回已保存的对象</returns>
        T SaveOrUpdateCopy<T>(T item) where T : class, IEntity;

        /// <summary>
        /// 更新指定的类型为T的对象
        /// </summary>
        /// <param name="item">待更新的对象</param>
        void Update<T>(T item) where T : class, IEntity;

        /// <summary>
        /// 根据条件表达式统计满足条件的数据条数
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns>满足条件的记录数量</returns>
        long Count<T>(Expression<Func<T, bool>> expression) where T : class, IEntity;

        /// <summary>
        /// 根据条件表达式判断是否存在符合条件的数据
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns>如果存在满足条件的数据 返回True 否则 False</returns>
        bool Exists<T>(Expression<Func<T, bool>> expression) where T : class, IEntity;

        /// <summary>
        /// 获取存储过程对象
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <returns>返回指定的存储过程对象</returns>
        StoredProcedure GetStoredProcedure(string spName);
    }
}
