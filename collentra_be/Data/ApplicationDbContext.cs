using collentra_be.Model;
using Microsoft.EntityFrameworkCore;

namespace collentra_be.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }

    }
}
