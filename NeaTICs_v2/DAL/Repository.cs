using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.Entity;
using System.Linq.Expressions;
using NeaTICs_v2.Models;
using System.Data;

namespace NeaTICs_v2.DAL
{
    public class Repository<T> : IRepository<T> where T : class
    {
        internal Context context;
        internal DbSet<T> dbSet;

        public Repository(Context context)
        {
            context.Configuration.LazyLoadingEnabled = true;
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        public DbSet<T> DbSet
        {
            get
            {
                return context.Set<T>();
            }
        }

        public List<T> Find(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate).ToList();
        }

        public IQueryable<T> All()
        {
            return DbSet.AsQueryable();
        }

        public T Create(T entity)
        {
            try
            {
                var result = DbSet.Add(entity);
                this.context.SaveChanges();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("No se pudo crear el Usuario debido a un problema en la Base de Datos", e);
            }
        }

        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual T GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Insert(T entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            T entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(T entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(T entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public void Dispose()
        {
            if (context != null)
                context.Dispose();
        }
    }
}