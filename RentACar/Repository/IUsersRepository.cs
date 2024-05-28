using RentACar.Models;

namespace RentACar.Repository
{
    public interface IUsersRepository : IRepository<Users>
    {
        void Update(Users users);
        void Save();
    }
}
