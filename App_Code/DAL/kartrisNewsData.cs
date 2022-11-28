
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisNewsData", Namespace="http://tempuri.org/kartrisNewsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisNewsData", Namespace="http://tempuri.org/kartrisNewsData.xsd")]
    public partial class KartrisNewsData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisNewsDataNews> _news;
        
        [System.Xml.Serialization.XmlElementAttribute("News", Namespace="http://tempuri.org/kartrisNewsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisNewsDataNews> News
        {
            get
            {
                return this._news;
            }
            private set
            {
                this._news = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool NewsSpecified
        {
            get
            {
                return (this.News.Count != 0);
            }
        }
        
        public KartrisNewsData()
        {
            this._news = new System.Collections.ObjectModel.Collection<KartrisNewsDataNews>();
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisNewsDataNews", Namespace="http://tempuri.org/kartrisNewsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisNewsDataNews
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("N_ID", Namespace="http://tempuri.org/kartrisNewsData.xsd")]
        public int N_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool N_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> N_ID
        {
            get
            {
                if (this.N_IDValueSpecified)
                {
                    return this.N_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.N_IDValue = value.GetValueOrDefault();
                this.N_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_ID", Namespace="http://tempuri.org/kartrisNewsData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("N_Name", Namespace="http://tempuri.org/kartrisNewsData.xsd")]
        public string N_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("N_Text", Namespace="http://tempuri.org/kartrisNewsData.xsd")]
        public string N_Text { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("N_StrapLine", Namespace="http://tempuri.org/kartrisNewsData.xsd")]
        public string N_StrapLine { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("N_DateCreated", Namespace="http://tempuri.org/kartrisNewsData.xsd", DataType="dateTime")]
        public System.DateTime N_DateCreatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool N_DateCreatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> N_DateCreated
        {
            get
            {
                if (this.N_DateCreatedValueSpecified)
                {
                    return this.N_DateCreatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.N_DateCreatedValue = value.GetValueOrDefault();
                this.N_DateCreatedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("N_LastUpdated", Namespace="http://tempuri.org/kartrisNewsData.xsd", DataType="dateTime")]
        public System.DateTime N_LastUpdatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool N_LastUpdatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> N_LastUpdated
        {
            get
            {
                if (this.N_LastUpdatedValueSpecified)
                {
                    return this.N_LastUpdatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.N_LastUpdatedValue = value.GetValueOrDefault();
                this.N_LastUpdatedValueSpecified = value.HasValue;
            }
        }
    }

 