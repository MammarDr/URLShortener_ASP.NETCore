using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace UrlShortener.Data.Repositories.Implementation
{
    public class RepositoryBase<T> : IRepositoryBase<T>
        where T : class
    {
        protected readonly AppDbContext _context;

        public RepositoryBase(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<T> FindByConditon(Expression<Func<T, bool>> expression, bool AsNoTracking = false)
            => AsNoTracking ?
            _context.Set<T>().Where(expression).AsNoTracking() :
            _context.Set<T>().Where(expression);

        public IQueryable<T> FindAll(bool AsNoTracking = false)
            => AsNoTracking ?
            _context.Set<T>().AsNoTracking() :
            _context.Set<T>();

        public void Add(T entity)
            => _context.Set<T>().Add(entity);
        public void Update(T entity)
            => _context.Set<T>().Update(entity);
        public void Delete(T entity)
            => _context.Set<T>().Remove(entity);        
    }

}
