
using Microsoft.AspNetCore.Mvc.Rendering;
using RentACar.Data;
using RentACar.Models;

namespace RentACar.Repository
{
    public class CarsRepository : Repository<Cars>, ICarsRepository
    {
        private AppDbContext _appDbContext;
        public CarsRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void Update(Cars car)
        {
            _appDbContext.Update(car);
        }

        public void Save()
        {
            _appDbContext.SaveChanges();
        }
        public IEnumerable<Cars> GetAllCars()
        {
            return _appDbContext.Cars.ToList();
        }
    }
}
