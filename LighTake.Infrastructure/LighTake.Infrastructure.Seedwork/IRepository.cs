using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
//using EntityFramework.BulkInsert.Extensions;
using LighTake.Infrastructure.Common;


namespace LighTake.Infrastructure.Seedwork
{
    /// <summary>
    /// Base interface for implement a "Repository Pattern", for
    /// more information about this pattern see http://martinfowler.com/eaaCatalog/repository.html
    /// or http://blogs.msdn.com/adonet/archive/2009/06/16/using-repository-and-unit-of-work-patterns-with-entity-framework-4-0.aspx
    /// </summary>
    /// <remarks>
    /// Indeed, one might think that IDbSet already a generic repository and therefore
    /// would not need this item. Using this interface allows us to ensure PI principle
    /// within our domain model
    /// </remarks>
    /// <typeparam name="TEntity">Type of entity for this repository </typeparam>
    public interface IRepository<TEntity> : IDisposable
    {
        /// <summary>
        /// Get the unit of work in this repository
        /// </summary>
        IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Add item into repository
        /// </summary>
        /// <param name="item">Item to add to repository</param>
        void Add(TEntity item);

        void Add<T>(T item) where T : Entity;

        /// <summary>
        /// Delete item 
        /// </summary>
        /// <param name="item">Item to delete</param>
        void Remove(TEntity item);

        void Remove(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// Set item as modified
        /// </summary>
        /// <param name="item">Item to modify</param>
        void Modify(TEntity item);

        void Modify<T>(T item) where T : Entity;

        int Modify(Expression<Func<TEntity, TEntity>> updateExpression,
            Expression<Func<TEntity, bool>> filterExpression
            );

        ///// <summary>
        /////Track entity into this repository, really in UnitOfWork. 
        /////In EF this can be done with Attach and with Update in NH
        ///// </summary>
        ///// <param name="item">Item to attach</param>
        //void TrackItem(TEntity item);

        ///// <summary>
        ///// Sets modified entity into the repository. 
        ///// When calling Commit() method in UnitOfWork 
        ///// these changes will be saved into the storage
        ///// </summary>
        ///// <param name="persisted">The persisted item</param>
        ///// <param name="current">The current item</param>
        //void Merge(TEntity persisted, TEntity current);

        List<TEntity> GetList(Expression<Func<TEntity, bool>> filter);

        long Count(Expression<Func<TEntity, bool>> filter);

        bool Exists(Expression<Func<TEntity, bool>> filter);
        /// <summary>
        /// Get element by entity key
        /// </summary>
        /// <param name="keyValue">Entity key value</param>
        /// <returns></returns>
        TEntity Get(object keyValue);

        /// <summary>
        /// Get element by entity keys
        /// </summary>
        /// <param name="keyValues">Entity key values</param>
        /// <returns></returns>
        TEntity Get(params object[] keyValues);

        /// <summary>
        /// Get single element by Expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        TEntity Single(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// Get first element by Expression
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        TEntity First(Expression<Func<TEntity, bool>> expression,
                      Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByExpression = null);

        /// <summary>
        /// Get all elements of type TEntity in repository
        /// </summary>
        /// <returns>List of selected elements</returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// 获得DbSet.AsNoTracking() , 适用单纯查询不用于EF修改,有利于提升性能
        /// </summary>
        /// <returns></returns>
        //IEnumerable<TEntity> GetAsNoTracking();
        /// <summary>
        /// 根据条件表达式获取满足条件的所有类型为T的数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数据量</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="orderByExpression">排序表达式 如: o => o.CreatedOn</param>

        /// <returns>返回满足条件的所有类型为T的数据和分页信息</returns>
        IPagedList<TEntity> FindPagedList(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> expression,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByExpression);

        IPagedList<TEntity> FindPagedList(SearchParam searchParam, Expression<Func<TEntity, bool>> expression,
                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByExpression);

        /// <summary>
        /// Get all elements of type TEntity in repository
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageCount">Number of elements in each page</param>
        /// <param name="expression"> </param>
        /// <param name="orderByExpression">Order by expression for this query</param>
        /// <returns>List of selected elements</returns>
        IEnumerable<TEntity> GetPaged(int pageIndex, int pageCount, Expression<Func<TEntity, bool>> expression,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByExpression = null);

        IEnumerable<TEntity> GetPaged(SearchParam searchParam, Expression<Func<TEntity, bool>> expression,
                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByExpression = null);

        /// <summary>
        /// Get  elements of type TEntity in repository
        /// </summary>
        /// <param name="filter">Filter that each element do match</param>
        /// <param name="orderByExpression"> </param>
        /// <returns>List of selected elements</returns>
        IEnumerable<TEntity> GetFiltered(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByExpression = null);


        /// <summary>
        /// 获取IQueryable
        /// </summary>
        /// <returns>IQueryable</returns>
        IQueryable<TEntity> GetQuery();

        /// <summary>
        /// 根据条件获取列表
        /// </summary>
        /// <param name="predicate">查询条件表达式</param>
        /// <returns>实体列表</returns>
        IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 根据条件获取列表
        /// </summary>
        /// <param name="predicate">查询条件表达式</param>
        /// <param name="orderBy">排序</param>
        /// <returns>实体列表</returns>
        IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);

        /// <summary>
        /// 获取IQueryable
        /// </summary>
        /// <returns>IQueryable</returns>
        IQueryable<T> GetQuery<T>() where T : Entity;

        /// <summary>
        /// 根据条件获取列表
        /// </summary>
        /// <param name="predicate">查询条件表达式</param>
        /// <returns>实体列表</returns>
        IQueryable<T> GetQuery<T>(Expression<Func<T, bool>> predicate) where T : Entity;

        /// <summary>
        /// 根据条件获取列表
        /// </summary>
        /// <param name="predicate">查询条件表达式</param>
        /// <param name="orderBy">排序</param>
        /// <returns>实体列表</returns>
        IQueryable<T> GetQuery<T>(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy) where T : Entity;

        /// <summary>
        /// 执行sql语句（DDL/DML）
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int ExecuteCommand(string sql, params object[] parameters);

        IEnumerable<TEntity> ExecuteQuery(string sql, params object[] parameters);

        #region BulkInsert

        void BulkInsert<T>(IEnumerable<T> entities, int? batchSize = null);

        //void BulkInsert<T>(IEnumerable<T> entities, BulkInsertOptions options);

        void BulkInsert<T>(IEnumerable<T> entities, SqlBulkCopyOptions sqlBulkCopyOptions, int? batchSize = null);

        void BulkInsert<T>(IEnumerable<T> entities, IDbTransaction transaction,
                           SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default, int? batchSize = null);

        #endregion
    }
}
