using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data.SqlClient;
namespace Kartis
{
    public class KartisContext : DbContext
    {
   
        public DbSet<ConfigTblAdptr> Configs { get; set; }
        public DbSet<ConfigCacheTblAdptr> ConfigCaches { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\;Database=KartisDB;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
        
    }
}