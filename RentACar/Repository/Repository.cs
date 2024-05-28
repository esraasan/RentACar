using Microsoft.EntityFrameworkCore;
using RentACar.Data;
using System.Linq.Expressions;

namespace RentACar.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _appDbContext;
        internal DbSet<T> dbSet; // dbset=araba markalarının atanmış hali
        public Repository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            dbSet = _appDbContext.Set<T>();
            _appDbContext.Cars.Include(k => k.CarName).Include(k => k.Id);

        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public IEnumerable<T> GetAll(string? includeProps = null, string includeProperties = null)
        {
            IQueryable<T> sorgu = dbSet;
            if (!string.IsNullOrEmpty(includeProps))//inculude documansyonundan gelen bir yapıdır.
            {
                foreach (var includeProp in includeProps.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    sorgu = sorgu.Include(includeProp); //fk çekmek için sorguyu çalıştırarak çeker.
                }
            }
            return sorgu.ToList();
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities); //birden fazla için.
        }

        public T Get(Expression<Func<T, bool>> filtre, string? includeProps = null, string includeProperties = null)
        {
            IQueryable<T> sorgu = dbSet;
            sorgu = sorgu.Where(filtre);
            if (!string.IsNullOrEmpty(includeProps))//inculude documansyonundan gelen bir yapıdır.
            {
                foreach (var includeProp in includeProps.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    sorgu = sorgu.Include(includeProp); //fk çekmek için sorguyu çalıştırarak çeker.
                }
            }
#pragma warning disable CS8603 // Possible null reference return.
            return sorgu.FirstOrDefault(); //hazır bir kalıp araştır.
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
