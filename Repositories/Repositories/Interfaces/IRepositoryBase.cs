using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        void SoftDeleteAndSubmit(T entity);
        void DeleteAndSubmit(T entity);

        /// <summary>
        /// Retrieves all records that have not been deleted
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAll();

        void UpdateAndSubmit(T entity);
        void InsertAndSubmit(T entity);

        /// <summary>
        /// Returns a record, even if was soft-deleted before
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById(int id);

        IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate);
    }
}
