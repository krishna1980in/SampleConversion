using System.Collections.Generic;
namespace Kartis
{ 
    public class kartrisConfigData
    {
        
         public string CFG_Name { get; set; }
        
       
        public string CFG_Value { get; set; }
        
    
        public string CFG_DataType { get; set; }
        

        public string CFG_DisplayType { get; set; }
        

        public string CFG_DisplayInfo { get; set; }
        

        public string CFG_Description { get; set; }
        
       
        public string CFG_VersionAdded { get; set; }
        
     
        public string CFG_DefaultValue { get; set; }
         public string CactuShopName_DEPRECATED { get; set; } 
         public string CFG_Important { get; set; }
        
        public ICollection<kartrisConfigData> kartrisConfigDatas { get; set; } = new List<Book>();
    }
}