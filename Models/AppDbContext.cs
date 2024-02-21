using Microsoft.EntityFrameworkCore;

namespace ConsoleAppProductsList.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=Huawei-Laptop; Database=GorbatyukProductsList; Trusted_Connection=True; MultipleActiveResultSets=true; TrustServerCertificate=True;");
            //optionsBuilder.UseSqlServer("Server=MAIN-PC; Database=GorbatyukProductsList; Trusted_Connection=True; MultipleActiveResultSets=true; TrustServerCertificate=True;");
        }
    }
}