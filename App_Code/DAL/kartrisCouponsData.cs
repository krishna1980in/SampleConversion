

    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisCouponsData", Namespace="http://tempuri.org/kartrisCouponsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisCouponsData", Namespace="http://tempuri.org/kartrisCouponsData.xsd")]
    public partial class KartrisCouponsData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisCouponsDataCoupons> _coupons;
        
        [System.Xml.Serialization.XmlElementAttribute("Coupons", Namespace="http://tempuri.org/kartrisCouponsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisCouponsDataCoupons> Coupons
        {
            get
            {
                return this._coupons;
            }
            private set
            {
                this._coupons = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CouponsSpecified
        {
            get
            {
                return (this.Coupons.Count != 0);
            }
        }
        
        public KartrisCouponsData()
        {
            this._coupons = new System.Collections.ObjectModel.Collection<KartrisCouponsDataCoupons>();
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisCouponsDataCoupons", Namespace="http://tempuri.org/kartrisCouponsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisCouponsDataCoupons
    {
        
        [System.Xml.Serialization.XmlElementAttribute("CP_ID", Namespace="http://tempuri.org/kartrisCouponsData.xsd")]
        public short CP_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(25)]
        [System.Xml.Serialization.XmlElementAttribute("CP_CouponCode", Namespace="http://tempuri.org/kartrisCouponsData.xsd")]
        public string CP_CouponCode { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CP_Reusable", Namespace="http://tempuri.org/kartrisCouponsData.xsd")]
        public bool CP_ReusableValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CP_ReusableValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> CP_Reusable
        {
            get
            {
                if (this.CP_ReusableValueSpecified)
                {
                    return this.CP_ReusableValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CP_ReusableValue = value.GetValueOrDefault();
                this.CP_ReusableValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CP_Used", Namespace="http://tempuri.org/kartrisCouponsData.xsd")]
        public bool CP_UsedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CP_UsedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> CP_Used
        {
            get
            {
                if (this.CP_UsedValueSpecified)
                {
                    return this.CP_UsedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CP_UsedValue = value.GetValueOrDefault();
                this.CP_UsedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CP_CreatedTime", Namespace="http://tempuri.org/kartrisCouponsData.xsd", DataType="dateTime")]
        public System.DateTime CP_CreatedTimeValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CP_CreatedTimeValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> CP_CreatedTime
        {
            get
            {
                if (this.CP_CreatedTimeValueSpecified)
                {
                    return this.CP_CreatedTimeValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CP_CreatedTimeValue = value.GetValueOrDefault();
                this.CP_CreatedTimeValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CP_StartDate", Namespace="http://tempuri.org/kartrisCouponsData.xsd", DataType="dateTime")]
        public System.DateTime CP_StartDateValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CP_StartDateValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> CP_StartDate
        {
            get
            {
                if (this.CP_StartDateValueSpecified)
                {
                    return this.CP_StartDateValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CP_StartDateValue = value.GetValueOrDefault();
                this.CP_StartDateValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CP_EndDate", Namespace="http://tempuri.org/kartrisCouponsData.xsd", DataType="dateTime")]
        public System.DateTime CP_EndDateValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CP_EndDateValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> CP_EndDate
        {
            get
            {
                if (this.CP_EndDateValueSpecified)
                {
                    return this.CP_EndDateValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CP_EndDateValue = value.GetValueOrDefault();
                this.CP_EndDateValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CP_DiscountValue", Namespace="http://tempuri.org/kartrisCouponsData.xsd")]
        public decimal CP_DiscountValueValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CP_DiscountValueValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> CP_DiscountValue
        {
            get
            {
                if (this.CP_DiscountValueValueSpecified)
                {
                    return this.CP_DiscountValueValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CP_DiscountValueValue = value.GetValueOrDefault();
                this.CP_DiscountValueValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("CP_DiscountType", Namespace="http://tempuri.org/kartrisCouponsData.xsd")]
        public string CP_DiscountType { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CP_Enabled", Namespace="http://tempuri.org/kartrisCouponsData.xsd")]
        public bool CP_EnabledValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CP_EnabledValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> CP_Enabled
        {
            get
            {
                if (this.CP_EnabledValueSpecified)
                {
                    return this.CP_EnabledValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CP_EnabledValue = value.GetValueOrDefault();
                this.CP_EnabledValueSpecified = value.HasValue;
            }
        }
    }

 