using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Repositories.DataContext;
using Repositories.Repositories.Interfaces;

namespace Repositories.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private IDataContext dataContext;

        public RepositoryBase(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return this.DataSource();
        }

        public virtual T GetById(int id)
        {
            return this.dataContext.Set<T>().Find(id);
        }

        public virtual void InsertAndSubmit(T entity)
        {
            this.dataContext.Set<T>().Add(entity);
            this.SaveChanges();
        }

        public virtual void UpdateAndSubmit(T entity)
        {
            this.SaveChanges();
        }

        public virtual void DeleteAndSubmit(T entity)
        {
            this.dataContext.Set<T>().Remove(entity);
            this.SaveChanges();
        }

        public virtual void SoftDeleteAndSubmit(T entity)
        {
            if (typeof(T).GetProperty("Deleted") != null)
            {
                entity.GetType().GetProperty("Deleted").SetValue(entity, DateTime.Now, null);
                this.UpdateAndSubmit(entity);
            }
            else
            {
                throw new InvalidOperationException("This entity type does not support soft deletion. Please add a DateTime? property called Deleted and try again.");
            }

        }

        public void ExecuteCommand(string sql, params object[] parameters)
        {
            this.dataContext.ExecuteCommand(sql, parameters);
        }

        #region Private Helpers
        /// <summary>
        /// Returns expression to use in expression trees, like where statements. For example query.Where(GetExpression("IsDeleted", typeof(boolean), false));
        /// </summary>
        /// <param name="propertyName">The name of the property. Either boolean or a nulleable typ</param>
        private Expression<Func<T, bool>> GetExpression(string propertyName, object value)
        {
            var param = Expression.Parameter(typeof(T));
            var actualValueExpression = Expression.Property(param, propertyName);

            var lambda = Expression.Lambda<Func<T, bool>>(
                Expression.Equal(actualValueExpression,
                    Expression.Constant(value)),
                param);

            return lambda;
        }

        private IQueryable<T> DataSource()
        {
            var query = dataContext.Set<T>().AsQueryable<T>();
            var property = typeof(T).GetProperty("Deleted");

            if (property != null)
            {
                query = query.Where(GetExpression("Deleted", null));
            }

            return query;
        }

        public IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate)
        {
            return dataContext.Set<T>().Where(predicate);
        }


        protected virtual void SaveChanges()
        {
            this.dataContext.SaveChanges();
        }
        #endregion
    }
}
