

    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisKBData", Namespace="http://tempuri.org/kartrisKBData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisKBData", Namespace="http://tempuri.org/kartrisKBData.xsd")]
    public partial class KartrisKBData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisKBDataKnowledgeBase> _knowledgeBase;
        
        [System.Xml.Serialization.XmlElementAttribute("KnowledgeBase", Namespace="http://tempuri.org/kartrisKBData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisKBDataKnowledgeBase> KnowledgeBase
        {
            get
            {
                return this._knowledgeBase;
            }
            private set
            {
                this._knowledgeBase = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool KnowledgeBaseSpecified
        {
            get
            {
                return (this.KnowledgeBase.Count != 0);
            }
        }
        
        public KartrisKBData()
        {
            this._knowledgeBase = new System.Collections.ObjectModel.Collection<KartrisKBDataKnowledgeBase>();
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisKBDataKnowledgeBase", Namespace="http://tempuri.org/kartrisKBData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisKBDataKnowledgeBase
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("KB_ID", Namespace="http://tempuri.org/kartrisKBData.xsd")]
        public int KB_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool KB_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> KB_ID
        {
            get
            {
                if (this.KB_IDValueSpecified)
                {
                    return this.KB_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.KB_IDValue = value.GetValueOrDefault();
                this.KB_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("KB_DateCreated", Namespace="http://tempuri.org/kartrisKBData.xsd", DataType="dateTime")]
        public System.DateTime KB_DateCreatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool KB_DateCreatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> KB_DateCreated
        {
            get
            {
                if (this.KB_DateCreatedValueSpecified)
                {
                    return this.KB_DateCreatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.KB_DateCreatedValue = value.GetValueOrDefault();
                this.KB_DateCreatedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("KB_DateUpdated", Namespace="http://tempuri.org/kartrisKBData.xsd", DataType="dateTime")]
        public System.DateTime KB_DateUpdatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool KB_DateUpdatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> KB_DateUpdated
        {
            get
            {
                if (this.KB_DateUpdatedValueSpecified)
                {
                    return this.KB_DateUpdatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.KB_DateUpdatedValue = value.GetValueOrDefault();
                this.KB_DateUpdatedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("KB_Live", Namespace="http://tempuri.org/kartrisKBData.xsd")]
        public bool KB_LiveValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool KB_LiveValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> KB_Live
        {
            get
            {
                if (this.KB_LiveValueSpecified)
                {
                    return this.KB_LiveValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.KB_LiveValue = value.GetValueOrDefault();
                this.KB_LiveValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_ID", Namespace="http://tempuri.org/kartrisKBData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("KB_Title", Namespace="http://tempuri.org/kartrisKBData.xsd")]
        public string KB_Title { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("KB_MetaDescription", Namespace="http://tempuri.org/kartrisKBData.xsd")]
        public string KB_MetaDescription { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("KB_MetaKeywords", Namespace="http://tempuri.org/kartrisKBData.xsd")]
        public string KB_MetaKeywords { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("KB_Text", Namespace="http://tempuri.org/kartrisKBData.xsd")]
        public string KB_Text { get; set; }
    }
