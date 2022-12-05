using System.Collections.Generic;
namespace Kartis
{ 
    public class kartrisCouponsData
    {
        
         public string CP_ID { get; set; }
        
       
        public string CP_CouponCode { get; set; }
        
    
        public string CP_Reusable { get; set; }
        

        public string CP_Used { get; set; }
        

        public string CP_CreatedTime { get; set; }
        

        public string CP_StartDate { get; set; }
        
       
        public string CP_EndDate { get; set; }
        
     
        public string CP_DiscountValue { get; set; }
         public string CP_DiscountType { get; set; } 
         public string CP_Enabled { get; set; }
        
        public ICollection<kartrisCouponsData> kartrisCouponsDatas { get; set; } = new List<Book>();
    }
}