
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisVersionsData", Namespace="http://tempuri.org/kartrisVersionsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisVersionsData", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
    public partial class KartrisVersionsData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisVersionsDataVersions> _versions;
        
        [System.Xml.Serialization.XmlElementAttribute("Versions", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisVersionsDataVersions> Versions
        {
            get
            {
                return this._versions;
            }
            private set
            {
                this._versions = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool VersionsSpecified
        {
            get
            {
                return (this.Versions.Count != 0);
            }
        }
        
        public KartrisVersionsData()
        {
            this._versions = new System.Collections.ObjectModel.Collection<KartrisVersionsDataVersions>();
            this._versionOptionLink = new System.Collections.ObjectModel.Collection<KartrisVersionsDataVersionOptionLink>();
            this._versionName = new System.Collections.ObjectModel.Collection<KartrisVersionsDataVersionName>();
            this._quantityDiscounts = new System.Collections.ObjectModel.Collection<KartrisVersionsDataQuantityDiscounts>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisVersionsDataVersionOptionLink> _versionOptionLink;
        
        [System.Xml.Serialization.XmlElementAttribute("VersionOptionLink", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisVersionsDataVersionOptionLink> VersionOptionLink
        {
            get
            {
                return this._versionOptionLink;
            }
            private set
            {
                this._versionOptionLink = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool VersionOptionLinkSpecified
        {
            get
            {
                return (this.VersionOptionLink.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisVersionsDataVersionName> _versionName;
        
        [System.Xml.Serialization.XmlElementAttribute("VersionName", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisVersionsDataVersionName> VersionName
        {
            get
            {
                return this._versionName;
            }
            private set
            {
                this._versionName = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool VersionNameSpecified
        {
            get
            {
                return (this.VersionName.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisVersionsDataQuantityDiscounts> _quantityDiscounts;
        
        [System.Xml.Serialization.XmlElementAttribute("QuantityDiscounts", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisVersionsDataQuantityDiscounts> QuantityDiscounts
        {
            get
            {
                return this._quantityDiscounts;
            }
            private set
            {
                this._quantityDiscounts = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool QuantityDiscountsSpecified
        {
            get
            {
                return (this.QuantityDiscounts.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisVersionsDataVersions", Namespace="http://tempuri.org/kartrisVersionsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisVersionsDataVersions
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_ID", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public long V_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> V_ID
        {
            get
            {
                if (this.V_IDValueSpecified)
                {
                    return this.V_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_IDValue = value.GetValueOrDefault();
                this.V_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_ID", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("V_Name", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public string V_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("V_Desc", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public string V_Desc { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(25)]
        [System.Xml.Serialization.XmlElementAttribute("V_CodeNumber", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public string V_CodeNumber { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_ProductID", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public int V_ProductIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_ProductIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> V_ProductID
        {
            get
            {
                if (this.V_ProductIDValueSpecified)
                {
                    return this.V_ProductIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_ProductIDValue = value.GetValueOrDefault();
                this.V_ProductIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_Price", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public decimal V_PriceValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_PriceValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> V_Price
        {
            get
            {
                if (this.V_PriceValueSpecified)
                {
                    return this.V_PriceValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_PriceValue = value.GetValueOrDefault();
                this.V_PriceValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_Tax", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public byte V_TaxValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_TaxValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> V_Tax
        {
            get
            {
                if (this.V_TaxValueSpecified)
                {
                    return this.V_TaxValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_TaxValue = value.GetValueOrDefault();
                this.V_TaxValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_Weight", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public float V_WeightValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_WeightValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> V_Weight
        {
            get
            {
                if (this.V_WeightValueSpecified)
                {
                    return this.V_WeightValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_WeightValue = value.GetValueOrDefault();
                this.V_WeightValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_DeliveryTime", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public byte V_DeliveryTimeValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_DeliveryTimeValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> V_DeliveryTime
        {
            get
            {
                if (this.V_DeliveryTimeValueSpecified)
                {
                    return this.V_DeliveryTimeValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_DeliveryTimeValue = value.GetValueOrDefault();
                this.V_DeliveryTimeValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_Quantity", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public float V_QuantityValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_QuantityValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> V_Quantity
        {
            get
            {
                if (this.V_QuantityValueSpecified)
                {
                    return this.V_QuantityValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_QuantityValue = value.GetValueOrDefault();
                this.V_QuantityValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_QuantityWarnLevel", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public float V_QuantityWarnLevelValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_QuantityWarnLevelValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> V_QuantityWarnLevel
        {
            get
            {
                if (this.V_QuantityWarnLevelValueSpecified)
                {
                    return this.V_QuantityWarnLevelValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_QuantityWarnLevelValue = value.GetValueOrDefault();
                this.V_QuantityWarnLevelValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_Live", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public bool V_LiveValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_LiveValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> V_Live
        {
            get
            {
                if (this.V_LiveValueSpecified)
                {
                    return this.V_LiveValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_LiveValue = value.GetValueOrDefault();
                this.V_LiveValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("V_DownLoadInfo", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public string V_DownLoadInfo { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("V_DownloadType", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public string V_DownloadType { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_OrderByValue", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public short V_OrderByValueValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_OrderByValueValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> V_OrderByValue
        {
            get
            {
                if (this.V_OrderByValueValueSpecified)
                {
                    return this.V_OrderByValueValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_OrderByValueValue = value.GetValueOrDefault();
                this.V_OrderByValueValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_RRP", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public decimal V_RRPValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_RRPValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> V_RRP
        {
            get
            {
                if (this.V_RRPValueSpecified)
                {
                    return this.V_RRPValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_RRPValue = value.GetValueOrDefault();
                this.V_RRPValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("V_Type", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public string V_Type { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_CustomerGroupID", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public short V_CustomerGroupIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_CustomerGroupIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> V_CustomerGroupID
        {
            get
            {
                if (this.V_CustomerGroupIDValueSpecified)
                {
                    return this.V_CustomerGroupIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_CustomerGroupIDValue = value.GetValueOrDefault();
                this.V_CustomerGroupIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("V_CustomizationType", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public string V_CustomizationType { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("V_CustomizationDesc", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public string V_CustomizationDesc { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_CustomizationCost", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public decimal V_CustomizationCostValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_CustomizationCostValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> V_CustomizationCost
        {
            get
            {
                if (this.V_CustomizationCostValueSpecified)
                {
                    return this.V_CustomizationCostValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_CustomizationCostValue = value.GetValueOrDefault();
                this.V_CustomizationCostValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_Tax2", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public byte V_Tax2Value { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_Tax2ValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> V_Tax2
        {
            get
            {
                if (this.V_Tax2ValueSpecified)
                {
                    return this.V_Tax2Value;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_Tax2Value = value.GetValueOrDefault();
                this.V_Tax2ValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(25)]
        [System.Xml.Serialization.XmlElementAttribute("V_TaxExtra", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public string V_TaxExtra { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisVersionsDataVersionOptionLink", Namespace="http://tempuri.org/kartrisVersionsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisVersionsDataVersionOptionLink
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_OPT_VersionID", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public long V_OPT_VersionIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_OPT_VersionIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> V_OPT_VersionID
        {
            get
            {
                if (this.V_OPT_VersionIDValueSpecified)
                {
                    return this.V_OPT_VersionIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_OPT_VersionIDValue = value.GetValueOrDefault();
                this.V_OPT_VersionIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_OPT_OptionID", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public int V_OPT_OptionIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_OPT_OptionIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> V_OPT_OptionID
        {
            get
            {
                if (this.V_OPT_OptionIDValueSpecified)
                {
                    return this.V_OPT_OptionIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_OPT_OptionIDValue = value.GetValueOrDefault();
                this.V_OPT_OptionIDValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisVersionsDataVersionName", Namespace="http://tempuri.org/kartrisVersionsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisVersionsDataVersionName
    {
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("V_Name", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public string V_Name { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisVersionsDataQuantityDiscounts", Namespace="http://tempuri.org/kartrisVersionsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisVersionsDataQuantityDiscounts
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("QD_Quantity", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public float QD_QuantityValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool QD_QuantityValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> QD_Quantity
        {
            get
            {
                if (this.QD_QuantityValueSpecified)
                {
                    return this.QD_QuantityValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.QD_QuantityValue = value.GetValueOrDefault();
                this.QD_QuantityValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("QD_Price", Namespace="http://tempuri.org/kartrisVersionsData.xsd")]
        public float QD_PriceValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool QD_PriceValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> QD_Price
        {
            get
            {
                if (this.QD_PriceValueSpecified)
                {
                    return this.QD_PriceValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.QD_PriceValue = value.GetValueOrDefault();
                this.QD_PriceValueSpecified = value.HasValue;
            }
        }
    }

 