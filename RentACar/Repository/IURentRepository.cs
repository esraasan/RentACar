using Microsoft.AspNetCore.Mvc.Rendering;
using RentACar.Models;

namespace RentACar.Repository
{
    public interface IURentRepository:IRepository<URent>
    {
        void Update(URent uRent);
        void Save();
        IEnumerable<SelectListItem> GetAllCarsAsSelectList();
    }
}
