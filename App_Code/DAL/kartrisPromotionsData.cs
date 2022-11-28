

    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisPromotionsData", Namespace="http://tempuri.org/kartrisPromotionsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisPromotionsData", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
    public partial class KartrisPromotionsData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisPromotionsDataPromotions> _promotions;
        
        [System.Xml.Serialization.XmlElementAttribute("Promotions", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisPromotionsDataPromotions> Promotions
        {
            get
            {
                return this._promotions;
            }
            private set
            {
                this._promotions = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PromotionsSpecified
        {
            get
            {
                return (this.Promotions.Count != 0);
            }
        }
        
        public KartrisPromotionsData()
        {
            this._promotions = new System.Collections.ObjectModel.Collection<KartrisPromotionsDataPromotions>();
            this._promotionStrings = new System.Collections.ObjectModel.Collection<KartrisPromotionsDataPromotionStrings>();
            this._promotionParts = new System.Collections.ObjectModel.Collection<KartrisPromotionsDataPromotionParts>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisPromotionsDataPromotionStrings> _promotionStrings;
        
        [System.Xml.Serialization.XmlElementAttribute("PromotionStrings", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisPromotionsDataPromotionStrings> PromotionStrings
        {
            get
            {
                return this._promotionStrings;
            }
            private set
            {
                this._promotionStrings = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PromotionStringsSpecified
        {
            get
            {
                return (this.PromotionStrings.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisPromotionsDataPromotionParts> _promotionParts;
        
        [System.Xml.Serialization.XmlElementAttribute("PromotionParts", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisPromotionsDataPromotionParts> PromotionParts
        {
            get
            {
                return this._promotionParts;
            }
            private set
            {
                this._promotionParts = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PromotionPartsSpecified
        {
            get
            {
                return (this.PromotionParts.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisPromotionsDataPromotions", Namespace="http://tempuri.org/kartrisPromotionsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisPromotionsDataPromotions
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("PROM_ID", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public short PROM_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PROM_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> PROM_ID
        {
            get
            {
                if (this.PROM_IDValueSpecified)
                {
                    return this.PROM_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PROM_IDValue = value.GetValueOrDefault();
                this.PROM_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("PROM_StartDate", Namespace="http://tempuri.org/kartrisPromotionsData.xsd", DataType="dateTime")]
        public System.DateTime PROM_StartDateValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PROM_StartDateValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> PROM_StartDate
        {
            get
            {
                if (this.PROM_StartDateValueSpecified)
                {
                    return this.PROM_StartDateValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PROM_StartDateValue = value.GetValueOrDefault();
                this.PROM_StartDateValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("PROM_EndDate", Namespace="http://tempuri.org/kartrisPromotionsData.xsd", DataType="dateTime")]
        public System.DateTime PROM_EndDateValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PROM_EndDateValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> PROM_EndDate
        {
            get
            {
                if (this.PROM_EndDateValueSpecified)
                {
                    return this.PROM_EndDateValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PROM_EndDateValue = value.GetValueOrDefault();
                this.PROM_EndDateValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("PROM_Live", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public bool PROM_LiveValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PROM_LiveValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> PROM_Live
        {
            get
            {
                if (this.PROM_LiveValueSpecified)
                {
                    return this.PROM_LiveValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PROM_LiveValue = value.GetValueOrDefault();
                this.PROM_LiveValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("PROM_OrderByValue", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public short PROM_OrderByValueValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PROM_OrderByValueValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> PROM_OrderByValue
        {
            get
            {
                if (this.PROM_OrderByValueValueSpecified)
                {
                    return this.PROM_OrderByValueValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PROM_OrderByValueValue = value.GetValueOrDefault();
                this.PROM_OrderByValueValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("PROM_MaxQuantity", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public byte PROM_MaxQuantityValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PROM_MaxQuantityValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> PROM_MaxQuantity
        {
            get
            {
                if (this.PROM_MaxQuantityValueSpecified)
                {
                    return this.PROM_MaxQuantityValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PROM_MaxQuantityValue = value.GetValueOrDefault();
                this.PROM_MaxQuantityValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisPromotionsDataPromotionStrings", Namespace="http://tempuri.org/kartrisPromotionsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisPromotionsDataPromotionStrings
    {
        
        [System.Xml.Serialization.XmlElementAttribute("PS_ID", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public byte PS_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("PS_PartNo", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public string PS_PartNo { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("PS_Type", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public string PS_Type { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("PS_Item", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public string PS_Item { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("PS_Text", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public string PS_Text { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("PS_Order", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public byte PS_OrderValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PS_OrderValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> PS_Order
        {
            get
            {
                if (this.PS_OrderValueSpecified)
                {
                    return this.PS_OrderValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PS_OrderValue = value.GetValueOrDefault();
                this.PS_OrderValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisPromotionsDataPromotionParts", Namespace="http://tempuri.org/kartrisPromotionsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisPromotionsDataPromotionParts
    {
        
        [System.Xml.Serialization.XmlElementAttribute("PROM_ID", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public int PROM_ID { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("PS_ID", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public byte PS_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("PS_PartNo", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public string PS_PartNo { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("PS_Text", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public string PS_Text { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("PP_ItemID", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public int PP_ItemID { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("PP_Value", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public decimal PP_ValueValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PP_ValueValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> PP_Value
        {
            get
            {
                if (this.PP_ValueValueSpecified)
                {
                    return this.PP_ValueValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PP_ValueValue = value.GetValueOrDefault();
                this.PP_ValueValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("PS_Item", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public string PS_Item { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("PS_Type", Namespace="http://tempuri.org/kartrisPromotionsData.xsd")]
        public string PS_Type { get; set; }
    }

 