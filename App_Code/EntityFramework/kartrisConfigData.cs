using System.Collections.Generic;
namespace Kartis
{ 
    public class ConfigTblAdptr
    {
        KartisContext samplecontext;
        public ConfigTblAdptr()  
      { 
        samplecontext=new KartisContext();
         
         // This is the constructor method.  
      }
        
         public string CFG_Name { get; set; }
        
       
        public string CFG_Value { get; set; }
        
    
        public string CFG_DataType { get; set; }
        

        public string CFG_DisplayType { get; set; }
        

        public string CFG_DisplayInfo { get; set; }
        

        public string CFG_Description { get; set; }
        
       
        public float CFG_VersionAdded { get; set; }
        
     
        public string CFG_DefaultValue { get; set; }
         public string CactuShopName_DEPRECATED { get; set; } 
         public string CFG_Important { get; set; }
        
        public ICollection<Config> Configs { get; set; } = new List<kartrisConfigData>();

           public IQueryable<kartrisCouponsData> GetConfigByName(string CFG_Name)
        {
            SqlParameter pCFG_Name = new SqlParameter("@CFG_Name", CFG_Name);
            return this.samplecontext.FromSql("EXECUTE spKartrisConfig_GetByName @CFG_Name", CFG_Name);
        }
         public IQueryable<kartrisCouponsData> GetConfigDesc(string CFG_Name)
        {
            SqlParameter pCFG_Name = new SqlParameter("@CFG_Name", CFG_Name);
            return this.samplecontext.FromSql("EXECUTE spKartrisConfig_GetDesc @CFG_Name", CFG_Name);
        }
          public IQueryable<kartrisCouponsData> GetImportantConfig()
        {
            
            return this.samplecontext.FromSql("EXECUTE spKartrisConfig_GetImportant");
        }
         public IQueryable<kartrisCouponsData> SearchConfig(string ConfigKey,bool ImportantConfig)
        {
            SqlParameter pConfigKey = new SqlParameter("@ConfigKey", ConfigKey);
            SqlParameter pImportantConfig = new SqlParameter("@ImportantConfig", ImportantConfig);
            return this.samplecontext.FromSql("EXECUTE spKartrisConfig_Search @ConfigKey,@ImportantConfig", ConfigKey,ImportantConfig);
        }
 
        

    }
     public class ConfigCacheTblAdptr
    {
          KartisContext samplecontext;
        public ConfigCacheTblAdptr()  
      { 
        samplecontext=new KartisContext();
         
         // This is the constructor method.  
      }
         public string CFG_Name { get; set; }
        
       
        public string CFG_Value { get; set; }
        public ICollection<ConfigCache> ConfigCaches { get; set; } = new List<ConfigCache>();
        public IQueryable<ConfigCache> GetConfigCacheData()
        {
            
            return this.ConfigCaches.FromSql("EXECUTE spKartrisConfig_GetforCache");
        }

    }
}