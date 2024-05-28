using Microsoft.EntityFrameworkCore;
using RentACar.Data;
using RentACar.Models;

namespace RentACar.Repository
{
    public class RentalRepository : Repository<Rental>, IRentalRepository
    {
        private readonly AppDbContext _appDbContext;
        public RentalRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void Update(Rental rent)
        {
            _appDbContext.Update(rent);
        }

        public void Save()
        {
            _appDbContext.SaveChanges();
        }

        public object GetAllRents()
        {
            return _appDbContext.Rentals.Include(x => x.User).Include(x => x.Car).ToList();
           
        }

        public object GetAll(int value, string includeProperties)
        {
            throw new NotImplementedException();
        }

        public object GetAllRents(int value, string includeProperties)
        {
            throw new NotImplementedException();
        }
    }
}