
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisStockNotificationsData", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisStockNotificationsData", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
    public partial class KartrisStockNotificationsData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisStockNotificationsDataStockNotifications> _stockNotifications;
        
        [System.Xml.Serialization.XmlElementAttribute("StockNotifications", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisStockNotificationsDataStockNotifications> StockNotifications
        {
            get
            {
                return this._stockNotifications;
            }
            private set
            {
                this._stockNotifications = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool StockNotificationsSpecified
        {
            get
            {
                return (this.StockNotifications.Count != 0);
            }
        }
        
        public KartrisStockNotificationsData()
        {
            this._stockNotifications = new System.Collections.ObjectModel.Collection<KartrisStockNotificationsDataStockNotifications>();
            this._versions = new System.Collections.ObjectModel.Collection<KartrisStockNotificationsDataVersions>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisStockNotificationsDataVersions> _versions;
        
        [System.Xml.Serialization.XmlElementAttribute("Versions", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisStockNotificationsDataVersions> Versions
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
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisStockNotificationsDataStockNotifications", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisStockNotificationsDataStockNotifications
    {
        
        [System.Xml.Serialization.XmlElementAttribute("SNR_ID", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public long SNR_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("SNR_UserEmail", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public string SNR_UserEmail { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("SNR_VersionID", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public long SNR_VersionID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("SNR_PageLink", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public string SNR_PageLink { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("SNR_ProductName", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public string SNR_ProductName { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("SNR_DateCreated", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd", DataType="dateTime")]
        public System.DateTime SNR_DateCreated { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("SNR_LanguageID", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public byte SNR_LanguageID { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisStockNotificationsDataVersions", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisStockNotificationsDataVersions
    {
        
        [System.Xml.Serialization.XmlElementAttribute("V_ID", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public long V_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(25)]
        [System.Xml.Serialization.XmlElementAttribute("V_CodeNumber", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public string V_CodeNumber { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_ProductID", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("V_Price", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("V_Tax", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
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
        
        [System.Xml.Serialization.XmlElementAttribute("V_Weight", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public float V_Weight { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("V_DeliveryTime", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public byte V_DeliveryTime { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("V_Quantity", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public float V_Quantity { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("V_QuantityWarnLevel", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public float V_QuantityWarnLevel { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("V_Live", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public bool V_Live { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("V_DownLoadInfo", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public string V_DownLoadInfo { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("V_DownloadType", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public string V_DownloadType { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("V_OrderByValue", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public short V_OrderByValue { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("V_RRP", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public decimal V_RRP { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("V_Type", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public string V_Type { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_CustomerGroupID", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("V_CustomizationType", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public string V_CustomizationType { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("V_CustomizationDesc", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public string V_CustomizationDesc { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_CustomizationCost", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("V_Tax2", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
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
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("V_TaxExtra", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd")]
        public string V_TaxExtra { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_BulkUpdateTimeStamp", Namespace="http://tempuri.org/kartrisStockNotificationsData.xsd", DataType="dateTime")]
        public System.DateTime V_BulkUpdateTimeStampValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_BulkUpdateTimeStampValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> V_BulkUpdateTimeStamp
        {
            get
            {
                if (this.V_BulkUpdateTimeStampValueSpecified)
                {
                    return this.V_BulkUpdateTimeStampValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_BulkUpdateTimeStampValue = value.GetValueOrDefault();
                this.V_BulkUpdateTimeStampValueSpecified = value.HasValue;
            }
        }
    }

 