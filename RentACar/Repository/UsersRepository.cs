using RentACar.Data;
using RentACar.Models;

namespace RentACar.Repository
{
    public class UsersRepository : Repository<Users>, IUsersRepository
    {
        private readonly AppDbContext _appDbContext;
        public UsersRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void Save()
        {
            _appDbContext.SaveChanges();
        }

        public void Update(Users users)
        {
            _appDbContext.Update(users);
        }
    }
}
