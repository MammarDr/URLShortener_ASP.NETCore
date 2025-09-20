using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Data.Repositories.Implementation
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll(bool AsNoTracking = false);
        IQueryable<T> FindByConditon(Expression<Func<T, bool>> expression , bool AsNoTracking = false);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);  
    }

}
