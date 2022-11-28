

    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisConfigData", Namespace="http://tempuri.org/kartrisConfigData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisConfigData", Namespace="http://tempuri.org/kartrisConfigData.xsd")]
    public partial class KartrisConfigData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisConfigDataConfig> _config;
        
        [System.Xml.Serialization.XmlElementAttribute("Config", Namespace="http://tempuri.org/kartrisConfigData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisConfigDataConfig> Config
        {
            get
            {
                return this._config;
            }
            private set
            {
                this._config = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ConfigSpecified
        {
            get
            {
                return (this.Config.Count != 0);
            }
        }
        
        public KartrisConfigData()
        {
            this._config = new System.Collections.ObjectModel.Collection<KartrisConfigDataConfig>();
            this._configCache = new System.Collections.ObjectModel.Collection<KartrisConfigDataConfigCache>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisConfigDataConfigCache> _configCache;
        
        [System.Xml.Serialization.XmlElementAttribute("ConfigCache", Namespace="http://tempuri.org/kartrisConfigData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisConfigDataConfigCache> ConfigCache
        {
            get
            {
                return this._configCache;
            }
            private set
            {
                this._configCache = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ConfigCacheSpecified
        {
            get
            {
                return (this.ConfigCache.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisConfigDataConfig", Namespace="http://tempuri.org/kartrisConfigData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisConfigDataConfig
    {
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("CFG_Name", Namespace="http://tempuri.org/kartrisConfigData.xsd")]
        public string CFG_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("CFG_Value", Namespace="http://tempuri.org/kartrisConfigData.xsd")]
        public string CFG_Value { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("CFG_DataType", Namespace="http://tempuri.org/kartrisConfigData.xsd")]
        public string CFG_DataType { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
        [System.Xml.Serialization.XmlElementAttribute("CFG_DisplayType", Namespace="http://tempuri.org/kartrisConfigData.xsd")]
        public string CFG_DisplayType { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("CFG_DisplayInfo", Namespace="http://tempuri.org/kartrisConfigData.xsd")]
        public string CFG_DisplayInfo { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("CFG_Description", Namespace="http://tempuri.org/kartrisConfigData.xsd")]
        public string CFG_Description { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CFG_VersionAdded", Namespace="http://tempuri.org/kartrisConfigData.xsd")]
        public float CFG_VersionAddedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CFG_VersionAddedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> CFG_VersionAdded
        {
            get
            {
                if (this.CFG_VersionAddedValueSpecified)
                {
                    return this.CFG_VersionAddedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CFG_VersionAddedValue = value.GetValueOrDefault();
                this.CFG_VersionAddedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("CFG_DefaultValue", Namespace="http://tempuri.org/kartrisConfigData.xsd")]
        public string CFG_DefaultValue { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("CactuShopName_DEPRECATED", Namespace="http://tempuri.org/kartrisConfigData.xsd")]
        public string CactuShopName_DEPRECATED { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CFG_Important", Namespace="http://tempuri.org/kartrisConfigData.xsd")]
        public bool CFG_ImportantValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CFG_ImportantValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> CFG_Important
        {
            get
            {
                if (this.CFG_ImportantValueSpecified)
                {
                    return this.CFG_ImportantValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CFG_ImportantValue = value.GetValueOrDefault();
                this.CFG_ImportantValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisConfigDataConfigCache", Namespace="http://tempuri.org/kartrisConfigData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisConfigDataConfigCache
    {
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("CFG_Name", Namespace="http://tempuri.org/kartrisConfigData.xsd")]
        public string CFG_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("CFG_Value", Namespace="http://tempuri.org/kartrisConfigData.xsd")]
        public string CFG_Value { get; set; }
    }

 