using RentACar.Models;

namespace RentACar.Repository
{
    public interface IURentalRepository:IRepository<URent>
    {
        void Update(URent uRent);
        void Save();
    }
}
