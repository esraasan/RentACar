using Microsoft.AspNetCore.Mvc.Rendering;
using RentACar.Models;

namespace RentACar.Repository
{
    public interface ICarsRepository : IRepository<Cars>
    {
        void Update(Cars car);

        void Save();
        IEnumerable<Cars> GetAllCars();
    }
}
