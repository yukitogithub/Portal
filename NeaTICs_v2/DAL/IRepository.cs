using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NeaTICs_v2.DAL
{
    public interface IRepository<T> : IDisposable where T : class
    {
        List<T> Find(Expression<Func<T, bool>> predicate);
        IQueryable<T> All();
        T Create(T entity);
    }
}
