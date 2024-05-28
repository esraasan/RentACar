using RentACar.Data;
using RentACar.Models;

namespace RentACar.Repository
{
    public class URentalRepository:Repository<URent>,IURentalRepository
    {
        private readonly AppDbContext _appDbContext;
        public URentalRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public void Update(URent uRent)
        {
            _appDbContext.Update(uRent);
        }
        public void Save()
        {
            _appDbContext.SaveChanges();
        }
    }
}
