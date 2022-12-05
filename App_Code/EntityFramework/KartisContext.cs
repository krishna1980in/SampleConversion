using Microsoft.EntityFrameworkCore;
namespace Kartis
{
    public class KartisContext : DbContext
    {
        public DbSet<kartrisCouponsData> kartrisCouponsDatas { get; set; }
        public DbSet<kartrisConfigData> kartrisConfigDatas { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\;Database=KartisDB;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}