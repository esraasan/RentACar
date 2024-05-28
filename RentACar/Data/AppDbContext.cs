using Microsoft.EntityFrameworkCore;
using RentACar.Models;

namespace RentACar.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cars> Cars { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<URent> URent { get; set; }

    }
}
