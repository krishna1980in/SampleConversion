

    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisObjectConfig", Namespace="http://tempuri.org/kartrisObjectConfig.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisObjectConfig", Namespace="http://tempuri.org/kartrisObjectConfig.xsd")]
    public partial class KartrisObjectConfig
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisObjectConfigObjectConfig> _objectConfig;
        
        [System.Xml.Serialization.XmlElementAttribute("ObjectConfig", Namespace="http://tempuri.org/kartrisObjectConfig.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisObjectConfigObjectConfig> ObjectConfig
        {
            get
            {
                return this._objectConfig;
            }
            private set
            {
                this._objectConfig = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ObjectConfigSpecified
        {
            get
            {
                return (this.ObjectConfig.Count != 0);
            }
        }
        
        public KartrisObjectConfig()
        {
            this._objectConfig = new System.Collections.ObjectModel.Collection<KartrisObjectConfigObjectConfig>();
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisObjectConfigObjectConfig", Namespace="http://tempuri.org/kartrisObjectConfig.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisObjectConfigObjectConfig
    {
        
        [System.Xml.Serialization.XmlElementAttribute("OC_ID", Namespace="http://tempuri.org/kartrisObjectConfig.xsd")]
        public short OC_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("OC_Name", Namespace="http://tempuri.org/kartrisObjectConfig.xsd")]
        public string OC_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("OC_ObjectType", Namespace="http://tempuri.org/kartrisObjectConfig.xsd")]
        public string OC_ObjectType { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("OC_DataType", Namespace="http://tempuri.org/kartrisObjectConfig.xsd")]
        public string OC_DataType { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("OC_Description", Namespace="http://tempuri.org/kartrisObjectConfig.xsd")]
        public string OC_Description { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("OC_VersionAdded", Namespace="http://tempuri.org/kartrisObjectConfig.xsd")]
        public float OC_VersionAddedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool OC_VersionAddedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> OC_VersionAdded
        {
            get
            {
                if (this.OC_VersionAddedValueSpecified)
                {
                    return this.OC_VersionAddedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.OC_VersionAddedValue = value.GetValueOrDefault();
                this.OC_VersionAddedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("OC_DefaultValue", Namespace="http://tempuri.org/kartrisObjectConfig.xsd")]
        public string OC_DefaultValue { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("OC_MultilineValue", Namespace="http://tempuri.org/kartrisObjectConfig.xsd")]
        public bool OC_MultilineValueValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool OC_MultilineValueValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> OC_MultilineValue
        {
            get
            {
                if (this.OC_MultilineValueValueSpecified)
                {
                    return this.OC_MultilineValueValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.OC_MultilineValueValue = value.GetValueOrDefault();
                this.OC_MultilineValueValueSpecified = value.HasValue;
            }
        }
    }

 