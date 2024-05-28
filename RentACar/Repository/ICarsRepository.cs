using RentACar.Models;

namespace RentACar.Repository
{
    public interface ICarsRepository : IRepository<Cars>
    {
        void Update(Cars car);

        void Save();
    }
}
