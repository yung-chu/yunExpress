using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.TypeAdapter;


namespace LighTake.Infrastructure.Seedwork
{
    public static class MapperExtensionMethods
    {
        /// <summary>
        /// (Entity)数据库实体转化为(Model)模型
        /// </summary>
        /// <typeparam name="TProjection">The dto projection</typeparam>
        /// <returns>The projected type</returns>
        public static TProjection ToModel<TProjection>(this Entity item)
            where TProjection : class,new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<TProjection>(item);
        }

        /// <summary>
        /// (Entity)模型转化为已存在的(Model)数据库实体
        /// </summary>
        /// <returns>The projected type</returns>
        public static TTarget ToModel<TSource, TTarget>(this TSource item, TTarget model)
            where TSource : class , new()
            where TTarget : class,new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt(item, model);
        }

        public static TTarget ToType<TTarget>(this object source)
            //where TSource : class, new()
           where TTarget : class ,new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<TTarget>(source);
        }

        /// <summary>
        /// (Model)模型转化为(Entity)数据库实体
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TTarget ToEntity<TTarget>(this object source)
            //where TSource : class, new()
            where TTarget : Entity, new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<TTarget>(source);
        }

        /// <summary>
        /// (Model)模型转化为已存在的(Entity)数据库实体
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TTarget ToEntity<TSource, TTarget>(this TSource source, TTarget target)
            where TSource : class, new()
            where TTarget : Entity, new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt(source, target);
        }


        /// <summary>
        /// (Entity)数据库实体集合转化为(Model)模型集合
        /// </summary>
        /// <returns>Projected collection</returns>
        public static List<TTarget> ToModelAsCollection<TSource, TTarget>(this IEnumerable<TSource> items)
            where TSource : Entity, new()
            where TTarget : class,new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<TSource, TTarget>(items).ToList();
        }

        /// <summary>
        /// (Entity)数据库实体集合转化为(Model)模型集合
        /// </summary>
        public static PagedList<TTarget> ToModelAsPageCollection<TSource, TTarget>(this IPagedList<TSource> items)
            where TTarget : class, new()
            where TSource : Entity, new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            var modelList = new PagedList<TTarget>(
                items.InnerList.ToModelAsCollection<TSource, TTarget>(),
                items.TotalCount, items.PageIndex, items.PageSize);
            return modelList;
            //return adapter.Adapt<PagedList<TSource>, PagedList<TTarget>>(items);

        }


        /// <summary>
        /// (Entity)数据库实体集合转化为(Model)模型集合
        /// </summary>
        /// <returns>Projected collection</returns>
        public static List<TTarget> ToCollection<TSource, TTarget>(this IEnumerable<TSource> items)
            where TSource : class , new()
            where TTarget : class,new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<TSource, TTarget>(items).ToList();
        }

        public static PagedList<TTarget> ToPageCollection<TSource, TTarget>(this IPagedList<TSource> items)
            where TTarget : class, new()
            where TSource : class, new()
        {
            //var adapter = TypeAdapterFactory.CreateAdapter();
            var modelList = new PagedList<TTarget>(
                items.InnerList.ToCollection<TSource, TTarget>(),
                items.TotalCount, items.PageIndex, items.PageSize);
            return modelList;
            //return adapter.Adapt<PagedList<TSource>, PagedList<TTarget>>(items);

        }

       /* public static PagedList<TTarget> ToPageCollection<TSource, TTarget>(this IApiPagedList<TSource> items)
            where TTarget : class,new()
            where TSource : class , new()
        {
            bool isNull = items == null;

            var modelList = new PagedList<TTarget>(
                isNull ? new List<TTarget>() : items.List.ToCollection<TSource, TTarget>(),
                isNull ? 0 : items.TotalCount, isNull ? 1 : items.PageIndex, isNull ? 10 : items.PageSize);

            return modelList;
        }

        public static ApiPagedList<TTarget> ToApiPageCollection<TSource, TTarget>(this IPagedList<TSource> items)
            where TTarget : class, new()
            where TSource : class, new()
        {
            //var adapter = TypeAdapterFactory.CreateAdapter();
            var modelList = new ApiPagedList<TTarget>(
                items.InnerList.ToCollection<TSource, TTarget>(),
                items.TotalCount, items.PageIndex, items.PageSize);
            return modelList;
            //return adapter.Adapt<PagedList<TSource>, PagedList<TTarget>>(items);

        }*/

        /// <summary>
        /// (Model)模型集合转化为(Entity)数据库实体集合
        /// </summary>
        public static List<TTarget> ToEntityAsCollection<TSource, TTarget>(this IEnumerable<TSource> items)
            where TSource : class, new()
            where TTarget : Entity, new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<List<TTarget>>(items);
        }
    }
}
