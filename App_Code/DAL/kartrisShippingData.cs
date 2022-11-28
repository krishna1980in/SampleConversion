

    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisShippingData", Namespace="http://tempuri.org/kartrisShippingData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisShippingData", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
    public partial class KartrisShippingData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisShippingDataShippingMethods> _shippingMethods;
        
        [System.Xml.Serialization.XmlElementAttribute("ShippingMethods", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisShippingDataShippingMethods> ShippingMethods
        {
            get
            {
                return this._shippingMethods;
            }
            private set
            {
                this._shippingMethods = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ShippingMethodsSpecified
        {
            get
            {
                return (this.ShippingMethods.Count != 0);
            }
        }
        
        public KartrisShippingData()
        {
            this._shippingMethods = new System.Collections.ObjectModel.Collection<KartrisShippingDataShippingMethods>();
            this._shippingRates = new System.Collections.ObjectModel.Collection<KartrisShippingDataShippingRates>();
            this._shippingZones = new System.Collections.ObjectModel.Collection<KartrisShippingDataShippingZones>();
            this._destinations = new System.Collections.ObjectModel.Collection<KartrisShippingDataDestinations>();
            this._shippingMethodsRates = new System.Collections.ObjectModel.Collection<KartrisShippingDataShippingMethodsRates>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisShippingDataShippingRates> _shippingRates;
        
        [System.Xml.Serialization.XmlElementAttribute("ShippingRates", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisShippingDataShippingRates> ShippingRates
        {
            get
            {
                return this._shippingRates;
            }
            private set
            {
                this._shippingRates = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ShippingRatesSpecified
        {
            get
            {
                return (this.ShippingRates.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisShippingDataShippingZones> _shippingZones;
        
        [System.Xml.Serialization.XmlElementAttribute("ShippingZones", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisShippingDataShippingZones> ShippingZones
        {
            get
            {
                return this._shippingZones;
            }
            private set
            {
                this._shippingZones = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ShippingZonesSpecified
        {
            get
            {
                return (this.ShippingZones.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisShippingDataDestinations> _destinations;
        
        [System.Xml.Serialization.XmlElementAttribute("Destinations", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisShippingDataDestinations> Destinations
        {
            get
            {
                return this._destinations;
            }
            private set
            {
                this._destinations = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DestinationsSpecified
        {
            get
            {
                return (this.Destinations.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisShippingDataShippingMethodsRates> _shippingMethodsRates;
        
        [System.Xml.Serialization.XmlElementAttribute("ShippingMethodsRates", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisShippingDataShippingMethodsRates> ShippingMethodsRates
        {
            get
            {
                return this._shippingMethodsRates;
            }
            private set
            {
                this._shippingMethodsRates = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ShippingMethodsRatesSpecified
        {
            get
            {
                return (this.ShippingMethodsRates.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisShippingDataShippingMethods", Namespace="http://tempuri.org/kartrisShippingData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisShippingDataShippingMethods
    {
        
        [System.Xml.Serialization.XmlElementAttribute("SM_ID", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public byte SM_ID { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LANG_ID", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public byte LANG_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("SM_Name", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public string SM_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("SM_Desc", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public string SM_Desc { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("SM_Live", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public bool SM_Live { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SM_OrderByValue", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public byte SM_OrderByValueValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SM_OrderByValueValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> SM_OrderByValue
        {
            get
            {
                if (this.SM_OrderByValueValueSpecified)
                {
                    return this.SM_OrderByValueValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SM_OrderByValueValue = value.GetValueOrDefault();
                this.SM_OrderByValueValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SM_Tax", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public byte SM_TaxValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SM_TaxValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> SM_Tax
        {
            get
            {
                if (this.SM_TaxValueSpecified)
                {
                    return this.SM_TaxValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SM_TaxValue = value.GetValueOrDefault();
                this.SM_TaxValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisShippingDataShippingRates", Namespace="http://tempuri.org/kartrisShippingData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisShippingDataShippingRates
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("S_ID", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public short S_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool S_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> S_ID
        {
            get
            {
                if (this.S_IDValueSpecified)
                {
                    return this.S_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.S_IDValue = value.GetValueOrDefault();
                this.S_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("S_ShippingMethodID", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public byte S_ShippingMethodIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool S_ShippingMethodIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> S_ShippingMethodID
        {
            get
            {
                if (this.S_ShippingMethodIDValueSpecified)
                {
                    return this.S_ShippingMethodIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.S_ShippingMethodIDValue = value.GetValueOrDefault();
                this.S_ShippingMethodIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("S_ShippingZoneID", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public byte S_ShippingZoneIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool S_ShippingZoneIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> S_ShippingZoneID
        {
            get
            {
                if (this.S_ShippingZoneIDValueSpecified)
                {
                    return this.S_ShippingZoneIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.S_ShippingZoneIDValue = value.GetValueOrDefault();
                this.S_ShippingZoneIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("S_Boundary", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public decimal S_BoundaryValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool S_BoundaryValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> S_Boundary
        {
            get
            {
                if (this.S_BoundaryValueSpecified)
                {
                    return this.S_BoundaryValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.S_BoundaryValue = value.GetValueOrDefault();
                this.S_BoundaryValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("S_ShippingRate", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public decimal S_ShippingRateValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool S_ShippingRateValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> S_ShippingRate
        {
            get
            {
                if (this.S_ShippingRateValueSpecified)
                {
                    return this.S_ShippingRateValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.S_ShippingRateValue = value.GetValueOrDefault();
                this.S_ShippingRateValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(250)]
        [System.Xml.Serialization.XmlElementAttribute("S_ShippingGateways", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public string S_ShippingGateways { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisShippingDataShippingZones", Namespace="http://tempuri.org/kartrisShippingData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisShippingDataShippingZones
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SZ_ID", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public byte SZ_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SZ_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> SZ_ID
        {
            get
            {
                if (this.SZ_IDValueSpecified)
                {
                    return this.SZ_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SZ_IDValue = value.GetValueOrDefault();
                this.SZ_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_ID", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public byte LANG_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LANG_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> LANG_ID
        {
            get
            {
                if (this.LANG_IDValueSpecified)
                {
                    return this.LANG_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LANG_IDValue = value.GetValueOrDefault();
                this.LANG_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("SZ_Name", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public string SZ_Name { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SZ_Live", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public bool SZ_LiveValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SZ_LiveValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> SZ_Live
        {
            get
            {
                if (this.SZ_LiveValueSpecified)
                {
                    return this.SZ_LiveValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SZ_LiveValue = value.GetValueOrDefault();
                this.SZ_LiveValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SZ_OrderByValue", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public byte SZ_OrderByValueValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SZ_OrderByValueValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> SZ_OrderByValue
        {
            get
            {
                if (this.SZ_OrderByValueValueSpecified)
                {
                    return this.SZ_OrderByValueValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SZ_OrderByValueValue = value.GetValueOrDefault();
                this.SZ_OrderByValueValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisShippingDataDestinations", Namespace="http://tempuri.org/kartrisShippingData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisShippingDataDestinations
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("D_ID", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public short D_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool D_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> D_ID
        {
            get
            {
                if (this.D_IDValueSpecified)
                {
                    return this.D_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.D_IDValue = value.GetValueOrDefault();
                this.D_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_ID", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public byte LANG_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LANG_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> LANG_ID
        {
            get
            {
                if (this.LANG_IDValueSpecified)
                {
                    return this.LANG_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LANG_IDValue = value.GetValueOrDefault();
                this.LANG_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("D_Name", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public string D_Name { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("D_ShippingZoneID", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public byte D_ShippingZoneIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool D_ShippingZoneIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> D_ShippingZoneID
        {
            get
            {
                if (this.D_ShippingZoneIDValueSpecified)
                {
                    return this.D_ShippingZoneIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.D_ShippingZoneIDValue = value.GetValueOrDefault();
                this.D_ShippingZoneIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("D_Tax", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public decimal D_TaxValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool D_TaxValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> D_Tax
        {
            get
            {
                if (this.D_TaxValueSpecified)
                {
                    return this.D_TaxValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.D_TaxValue = value.GetValueOrDefault();
                this.D_TaxValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
        [System.Xml.Serialization.XmlElementAttribute("D_ISOCode", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public string D_ISOCode { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
        [System.Xml.Serialization.XmlElementAttribute("D_ISOCode3Letter", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public string D_ISOCode3Letter { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
        [System.Xml.Serialization.XmlElementAttribute("D_ISOCodeNumeric", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public string D_ISOCodeNumeric { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("D_Live", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public bool D_LiveValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool D_LiveValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> D_Live
        {
            get
            {
                if (this.D_LiveValueSpecified)
                {
                    return this.D_LiveValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.D_LiveValue = value.GetValueOrDefault();
                this.D_LiveValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
        [System.Xml.Serialization.XmlElementAttribute("D_Region", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public string D_Region { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("D_Tax2", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public decimal D_Tax2Value { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool D_Tax2ValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> D_Tax2
        {
            get
            {
                if (this.D_Tax2ValueSpecified)
                {
                    return this.D_Tax2Value;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.D_Tax2Value = value.GetValueOrDefault();
                this.D_Tax2ValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(25)]
        [System.Xml.Serialization.XmlElementAttribute("D_TaxExtra", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public string D_TaxExtra { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisShippingDataShippingMethodsRates", Namespace="http://tempuri.org/kartrisShippingData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisShippingDataShippingMethodsRates
    {
        
        [System.Xml.Serialization.XmlElementAttribute("S_ShippingRate", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public float S_ShippingRate { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("SM_ID", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public byte SM_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("SM_Name", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public string SM_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("SM_Desc", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public string SM_Desc { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(250)]
        [System.Xml.Serialization.XmlElementAttribute("S_ShippingGateways", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public string S_ShippingGateways { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SM_Tax", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public byte SM_TaxValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SM_TaxValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> SM_Tax
        {
            get
            {
                if (this.SM_TaxValueSpecified)
                {
                    return this.SM_TaxValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SM_TaxValue = value.GetValueOrDefault();
                this.SM_TaxValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SM_Tax2", Namespace="http://tempuri.org/kartrisShippingData.xsd")]
        public byte SM_Tax2Value { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SM_Tax2ValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> SM_Tax2
        {
            get
            {
                if (this.SM_Tax2ValueSpecified)
                {
                    return this.SM_Tax2Value;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SM_Tax2Value = value.GetValueOrDefault();
                this.SM_Tax2ValueSpecified = value.HasValue;
            }
        }
    }
