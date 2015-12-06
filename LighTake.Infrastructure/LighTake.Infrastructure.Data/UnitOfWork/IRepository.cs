using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LighTake.Infrastructure.Data
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetQuery();

        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Func<T, bool> where);
        T Single(Func<T, bool> where);
        T First(Func<T, bool> where);

        void Delete(T entity);
        void Add(T entity);
        void Attach(T entity);
        void SaveChanges();
    }
}
