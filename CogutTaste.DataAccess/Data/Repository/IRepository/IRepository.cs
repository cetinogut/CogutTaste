using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CogutTaste.DataAccess.Data.Repository.IRepository
{
    public interface IRepository<T> where T : class// bunu generic bir class yaptık ki any object can call this
    {
        T Get(int id);
        IEnumerable<T> GetAll(

            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null,
            string includeProperties = null
            );
        T GetFirstOrDefault(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null
            );

        void Add(T entity);

        void Remove(int id);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
