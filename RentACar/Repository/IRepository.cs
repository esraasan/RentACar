using System.Linq.Expressions;

namespace RentACar.Repository
{
    public interface IRepository<T> where T : class
    {
        //T ->Araba
        IEnumerable<T> GetAll(string? includeProps = null, string includeProperties = null);
        T Get(Expression<Func<T, bool>> filtre, string? includeProps = null, string includeProperties = null);

        void Add(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);

    }
}
