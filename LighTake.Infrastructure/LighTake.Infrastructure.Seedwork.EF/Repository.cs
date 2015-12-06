using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Extensions;
using LighTake.Infrastructure.Common;
using EntityFramework.BulkInsert.Extensions;

namespace LighTake.Infrastructure.Seedwork.EF
{
    /// <summary>
    /// Repository base class
    /// </summary>
    /// <typeparam name="TEntity">The type of underlying entity in this repository</typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        #region Members

        private DbContext _dbContext;
        private IUnitOfWork _unitOfWork;

        #endregion

        #region Constructor

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _unitOfWork = dbContext as IUnitOfWork;
        }

        #endregion

        #region IRepository Members

        /// <summary>
        /// <see cref="IRepository{TEntity}"/>
        /// </summary>
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _unitOfWork;
            }
        }

        #region add update delete

        /// <summary>
        /// <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <param name="item"><see cref="IRepository{TEntity}"/></param>
        public virtual void Add(TEntity item)
        {

            if (item != null)
                DbSet.Add(item); // add new item in this set

        }

        /// <summary>
        /// <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <param name="item"><see cref="IRepository{TEntity}"/></param>
        public virtual void Add<T>(T item) where T : Entity
        {

            if (item != null)
                GetDbSet<T>().Add(item); // add new item in this set

        }

      


        /// <summary>
        /// <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <param name="item"><see cref="IRepository{TEntity}"/></param>
        public virtual void Remove(TEntity item)
        {
            if (_dbContext.Entry(item).State == EntityState.Detached)
            {
                DbSet.Attach(item);
            }
            DbSet.Remove(item);
        }

        public virtual void Remove(Expression<Func<TEntity, bool>> expression)
        {
            if (expression != null)
            {
                /*var enmbEntity = DbSet.Where(expression);
                foreach (var entity in enmbEntity)
                {
                    Remove(entity);
                }*/
                DbSet.Where(expression).Delete();
            }
        }


        /// <summary>
        /// <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <param name="item"><see cref="IRepository{TEntity}"/></param>
        [Obsolete("此方法已经作废")]
        public virtual void Modify(TEntity item)
        {
            //DbSet.Attach(item);
            //_dbContext.Entry(item).State = EntityState.Modified;

            return;
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}"/>
        /// </summary>
        public virtual int Modify(Expression<Func<TEntity, TEntity>> updateExpression,
            Expression<Func<TEntity, bool>> filterExpression
            )
        {
            return DbSet.Where(filterExpression).Update(updateExpression);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <param name="item"><see cref="IRepository{TEntity}"/></param>
        public virtual void Modify<T>(T item) where T : Entity
        {
            GetDbSet<T>().Attach(item);
            _dbContext.Entry(item).State = EntityState.Modified;
        }

        #endregion



        /// <summary>
        /// <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <param name="keyValue"><see cref="IRepository{TEntity}"/></param>
        /// <returns><see cref="IRepository{TEntity}"/></returns>
        public virtual TEntity Get(object keyValue)
        {
            if (keyValue != null)
                return DbSet.Find(keyValue);
            else
                return default(TEntity);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <param name="keyValues"><see cref="IRepository{TEntity}"/></param>
        /// <returns><see cref="IRepository{TEntity}"/></returns>
        public virtual TEntity Get(params object[] keyValues)
        {
            if (keyValues != null)
                return DbSet.Find(keyValues);
            else
                return default(TEntity);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <returns><see cref="IRepository{TEntity}"/></returns>
        public virtual TEntity Single(Expression<Func<TEntity, bool>> expression)
        {
            if (expression != null)
                return DbSet.SingleOrDefault(expression);
            return default(TEntity);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <returns><see cref="IRepository{TEntity}"/></returns>
        public virtual TEntity First(Expression<Func<TEntity, bool>> expression,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            if (expression == null) return default(TEntity);
            return orderBy == null ? DbSet.FirstOrDefault(expression) : orderBy(DbSet).FirstOrDefault(expression);

        }

        [Obsolete("此方法已过期，请通过其他方式查询获取结果!")]        
        public virtual IEnumerable<TEntity> GetAll()
        {
            return DbSet;
        }

        /// <summary>
        ///  DbSet.AsNoTracking();
        /// </summary>
        /// <returns></returns>
        //public virtual IEnumerable<TEntity> GetAsNoTracking()
        //{
        //    return DbSet.AsNoTracking();
        //}


        /// <summary>
        /// 根据条件表达式获取满足条件的所有类型为T的数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数据量</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="orderBy">排序表达式 如: Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = o => o.OrderBy(g => g.SKU).OrderByDescending(g => g.SKU)</param>
        /// <returns>返回满足条件的所有类型为T的数据和分页信息</returns>
        public IPagedList<TEntity> FindPagedList(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> expression,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            var query = DbSet.Where(expression);

            return orderBy != null ? orderBy(query).ToPagedList(pageIndex, pageSize) : query.ToPagedList(pageIndex, pageSize);
        }

        public IPagedList<TEntity> FindPagedList(SearchParam searchParam, Expression<Func<TEntity, bool>> expression,
                                                 Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            return FindPagedList(searchParam.Page, searchParam.PageSize, expression, orderBy);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <param name="pageIndex"><see cref="IRepository{TEntity}"/></param>
        /// <param name="pageCount"><see cref="IRepository{TEntity}"/></param>
        /// <param name="expression"> </param>
        /// <param name="orderBy"><see cref="IRepository{TEntity}"/></param>
        /// <returns><see cref="IRepository{TEntity}"/></returns>
        public virtual IEnumerable<TEntity> GetPaged(int pageIndex, int pageCount, Expression<Func<TEntity, bool>> expression,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            var set = DbSet;

            if (orderBy == null)
                return set.Skip(pageCount * (pageIndex - 1))
                          .Take(pageCount);

            return orderBy(set)
                        .Skip(pageCount * (pageIndex - 1))
                        .Take(pageCount);

        }


        public virtual IEnumerable<TEntity> GetPaged(SearchParam searchParam, Expression<Func<TEntity, bool>> expression,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            return GetPaged(searchParam.Page, searchParam.PageSize, expression, orderBy);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <param name="filter"><see cref="IRepository{TEntity}"/></param>
        /// <param name="orderBy"> </param>
        /// <returns><see cref="IRepository{TEntity}"/></returns>
        public virtual IEnumerable<TEntity> GetFiltered(Expression<Func<TEntity, bool>> filter,
                        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy= null)
        {

            return orderBy == null ? DbSet.Where(filter) : orderBy(DbSet.Where(filter));
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <param name="filter"><see cref="IRepository{TEntity}"/></param>
        /// <returns><see cref="IRepository{TEntity}"/></returns>
        public virtual List<TEntity> GetList(Expression<Func<TEntity, bool>> filter)
        {
            return DbSet.Where(filter).ToList();
        }

        public virtual long Count(Expression<Func<TEntity, bool>> filter)
        {
            return DbSet.Where(filter).Count();
        }

        public virtual bool Exists(Expression<Func<TEntity, bool>> expression)
        {
            var item = DbSet.Any(expression);

            return item;
        }
        #endregion

        #region BulkInsert

        public virtual void BulkInsert<T>(IEnumerable<T> entities, int? batchSize = null)
        {
            _dbContext.BulkInsert(entities, batchSize);
        }
        public virtual void BulkInsert<T>(IEnumerable<T> entities, BulkInsertOptions options)
        {
            _dbContext.BulkInsert(entities, options);
        }

        public virtual void BulkInsert<T>(IEnumerable<T> entities, SqlBulkCopyOptions sqlBulkCopyOptions, int? batchSize = null)
        {
            _dbContext.BulkInsert(entities, sqlBulkCopyOptions, batchSize);
        }
        public virtual void BulkInsert<T>(IEnumerable<T> entities, IDbTransaction transaction, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default, int? batchSize = null)
        {
            _dbContext.BulkInsert(entities, transaction, sqlBulkCopyOptions, batchSize);
        }

        #endregion
        
        #region IDisposable Members

        /// <summary>
        /// <see cref="M:System.IDisposable.Dispose"/>
        /// </summary>
        public void Dispose()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }

        #endregion
        
        #region GetQuery


        /// <summary>
        /// 获取IQueryable
        /// </summary>
        /// <returns>IQueryable</returns>
        public IQueryable<TEntity> GetQuery()
        {
            return GetQuery(null);
        }

        /// <summary>
        /// 根据条件获取IQueryable
        /// </summary>
        /// <param name="predicate">查询条件表达式</param>
        /// <returns>IQueryable</returns>
        public IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> predicate)
        {
            return GetQuery(predicate, null);
        }

        /// <summary>
        /// 根据条件获取列表
        /// </summary>
        /// <param name="predicate">查询条件表达式</param>
        /// <param name="orderBy">排序</param>
        /// <returns>实体列表</returns>
        public IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            if (orderBy != null)
            {
                return orderBy(predicate == null ? DbSet : DbSet.Where(predicate));
            }
            return predicate == null ? DbSet : DbSet.Where(predicate);
        }


        /// <summary>
        /// 获取IQueryable
        /// </summary>
        /// <returns>IQueryable</returns>
        public IQueryable<T> GetQuery<T>() where T : Entity
        {
            return GetQuery<T>(null);
        }

        /// <summary>
        /// 根据条件获取列表
        /// </summary>
        /// <param name="predicate">查询条件表达式</param>
        /// <returns>实体列表</returns>
        public IQueryable<T> GetQuery<T>(Expression<Func<T, bool>> predicate) where T : Entity
        {
            return GetQuery(predicate, null);
        }

        /// <summary>
        /// 根据条件获取列表
        /// </summary>
        /// <param name="predicate">查询条件表达式</param>
        /// <param name="orderBy">排序</param>
        /// <returns>实体列表</returns>
        public IQueryable<T> GetQuery<T>(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy) where T : Entity
        {
            var set = GetDbSet<T>();

            if (orderBy != null)
            {
                return orderBy(predicate == null ? set : set.Where(predicate));
            }
            return predicate == null ? set : set.Where(predicate);
        }

        #endregion

        #region DbSet

        private DbSet<TEntity> DbSet
        {
            get { return _dbContext.Set<TEntity>(); }
        }

        private DbSet<T> GetDbSet<T>() where T : Entity
        {
            return _dbContext.Set<T>();
        }

        #endregion

        #region

        /// <summary>
        /// 执行sql语句（DDL/DML）
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteCommand(string sql, params object[] parameters)
        {
            return _dbContext.Database.ExecuteSqlCommand(sql, parameters);
        }

        public IEnumerable<TEntity> ExecuteQuery(string sql, params object[] parameters)
        {

            return DbSet.SqlQuery(sql, parameters);
        }

        /// <summary>
        /// 返回dataset(更新于20140418)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet SqlQueryForDataSet(string sql, SqlParameter[] parameters)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = _dbContext.Database.Connection.ConnectionString;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;

            if (parameters.Length > 0)
            {
                foreach (var item in parameters)
                {
                    cmd.Parameters.Add(item);
                }
            }
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            return dataSet;
        }
        #endregion


    }
}
