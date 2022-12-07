using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data.SqlClient;
namespace Kartis
{
    public class KartisContext : DbContext
    {
   
        public DbSet<kartrisConfigData> kartrisConfigDatas { get; set; }
        public DbSet<ConfigCache> ConfigCaches { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\;Database=KartisDB;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
        public IQueryable<kartrisCouponsData> GetConfigByName(string CFG_Name)
        {
            SqlParameter pCFG_Name = new SqlParameter("@CFG_Name", CFG_Name);
            return this.kartrisCouponsDatas.FromSql("EXECUTE spKartrisConfig_GetByName @CFG_Name", CFG_Name);
        }
         public IQueryable<kartrisCouponsData> GetConfigDesc(string CFG_Name)
        {
            SqlParameter pCFG_Name = new SqlParameter("@CFG_Name", CFG_Name);
            return this.kartrisCouponsDatas.FromSql("EXECUTE spKartrisConfig_GetDesc @CFG_Name", CFG_Name);
        }
          public IQueryable<kartrisCouponsData> GetImportantConfig()
        {
            
            return this.kartrisCouponsDatas.FromSql("EXECUTE spKartrisConfig_GetImportant");
        }
         public IQueryable<kartrisCouponsData> SearchConfig(string ConfigKey,bool ImportantConfig)
        {
            SqlParameter pConfigKey = new SqlParameter("@ConfigKey", ConfigKey);
            SqlParameter pImportantConfig = new SqlParameter("@ImportantConfig", ImportantConfig);
            return this.kartrisCouponsDatas.FromSql("EXECUTE spKartrisConfig_Search @ConfigKey,@ImportantConfig", ConfigKey,ImportantConfig);
        }
           public IQueryable<ConfigCache> GetConfigCacheData()
        {
            
            return this.ConfigCaches.FromSql("EXECUTE spKartrisConfig_GetforCache");
        }
    }
}