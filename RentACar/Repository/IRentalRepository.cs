using RentACar.Models;

namespace RentACar.Repository
{
    public interface IRentalRepository : IRepository<Rental>
    {
        object GetAllRents();
        void Save();
        void Update(Rental rent);

    }
}
