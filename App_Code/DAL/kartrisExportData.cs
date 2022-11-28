

    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisExportData", Namespace="http://tempuri.org/kartrisExportData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisExportData", Namespace="http://tempuri.org/kartrisExportData.xsd")]
    public partial class KartrisExportData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisExportDataSavedExports> _savedExports;
        
        [System.Xml.Serialization.XmlElementAttribute("SavedExports", Namespace="http://tempuri.org/kartrisExportData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisExportDataSavedExports> SavedExports
        {
            get
            {
                return this._savedExports;
            }
            private set
            {
                this._savedExports = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SavedExportsSpecified
        {
            get
            {
                return (this.SavedExports.Count != 0);
            }
        }
        
        public KartrisExportData()
        {
            this._savedExports = new System.Collections.ObjectModel.Collection<KartrisExportDataSavedExports>();
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisExportDataSavedExports", Namespace="http://tempuri.org/kartrisExportData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisExportDataSavedExports
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("Export_ID", Namespace="http://tempuri.org/kartrisExportData.xsd")]
        public long Export_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool Export_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> Export_ID
        {
            get
            {
                if (this.Export_IDValueSpecified)
                {
                    return this.Export_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.Export_IDValue = value.GetValueOrDefault();
                this.Export_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("Export_Name", Namespace="http://tempuri.org/kartrisExportData.xsd")]
        public string Export_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("Export_Details", Namespace="http://tempuri.org/kartrisExportData.xsd")]
        public string Export_Details { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("Export_FieldDelimiter", Namespace="http://tempuri.org/kartrisExportData.xsd")]
        public int Export_FieldDelimiterValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool Export_FieldDelimiterValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> Export_FieldDelimiter
        {
            get
            {
                if (this.Export_FieldDelimiterValueSpecified)
                {
                    return this.Export_FieldDelimiterValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.Export_FieldDelimiterValue = value.GetValueOrDefault();
                this.Export_FieldDelimiterValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("Export_StringDelimiter", Namespace="http://tempuri.org/kartrisExportData.xsd")]
        public int Export_StringDelimiterValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool Export_StringDelimiterValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> Export_StringDelimiter
        {
            get
            {
                if (this.Export_StringDelimiterValueSpecified)
                {
                    return this.Export_StringDelimiterValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.Export_StringDelimiterValue = value.GetValueOrDefault();
                this.Export_StringDelimiterValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("Export_DateCreated", Namespace="http://tempuri.org/kartrisExportData.xsd", DataType="dateTime")]
        public System.DateTime Export_DateCreatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool Export_DateCreatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> Export_DateCreated
        {
            get
            {
                if (this.Export_DateCreatedValueSpecified)
                {
                    return this.Export_DateCreatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.Export_DateCreatedValue = value.GetValueOrDefault();
                this.Export_DateCreatedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("Export_LastModified", Namespace="http://tempuri.org/kartrisExportData.xsd", DataType="dateTime")]
        public System.DateTime Export_LastModifiedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool Export_LastModifiedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> Export_LastModified
        {
            get
            {
                if (this.Export_LastModifiedValueSpecified)
                {
                    return this.Export_LastModifiedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.Export_LastModifiedValue = value.GetValueOrDefault();
                this.Export_LastModifiedValueSpecified = value.HasValue;
            }
        }
    }

 