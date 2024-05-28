using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentACar.Data;
using RentACar.Models;

namespace RentACar.Repository
{
    public class URentRepository:Repository<URent>,IURentRepository
    {
        private readonly AppDbContext _appDbContext;
        public URentRepository(AppDbContext appDbContext) : base(appDbContext)
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


        public IEnumerable<SelectListItem> GetAllCarsAsSelectList()
        {
            return _appDbContext.Cars.Select(k => new SelectListItem
            {
                Text = k.CarName,
                Value = k.Id.ToString()
            }).ToList();
        }

    }
}
