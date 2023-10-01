using Microsoft.EntityFrameworkCore;
using Sunrise.Models;

namespace Sunrise.Contexts
{
    public class ApplicatinDbContext :DbContext
    {

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }
        public ApplicatinDbContext(DbContextOptions options) : base(options) {


        }
    }
}
