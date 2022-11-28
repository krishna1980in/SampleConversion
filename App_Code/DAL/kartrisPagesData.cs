
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisPagesData", Namespace="http://tempuri.org/kartrisPagesData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisPagesData", Namespace="http://tempuri.org/kartrisPagesData.xsd")]
    public partial class KartrisPagesData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisPagesDataPages> _pages;
        
        [System.Xml.Serialization.XmlElementAttribute("Pages", Namespace="http://tempuri.org/kartrisPagesData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisPagesDataPages> Pages
        {
            get
            {
                return this._pages;
            }
            private set
            {
                this._pages = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PagesSpecified
        {
            get
            {
                return (this.Pages.Count != 0);
            }
        }
        
        public KartrisPagesData()
        {
            this._pages = new System.Collections.ObjectModel.Collection<KartrisPagesDataPages>();
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisPagesDataPages", Namespace="http://tempuri.org/kartrisPagesData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisPagesDataPages
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("PAGE_ID", Namespace="http://tempuri.org/kartrisPagesData.xsd")]
        public short PAGE_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PAGE_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> PAGE_ID
        {
            get
            {
                if (this.PAGE_IDValueSpecified)
                {
                    return this.PAGE_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PAGE_IDValue = value.GetValueOrDefault();
                this.PAGE_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("PAGE_Name", Namespace="http://tempuri.org/kartrisPagesData.xsd")]
        public string PAGE_Name { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("PAGE_ParentID", Namespace="http://tempuri.org/kartrisPagesData.xsd")]
        public short PAGE_ParentIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PAGE_ParentIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> PAGE_ParentID
        {
            get
            {
                if (this.PAGE_ParentIDValueSpecified)
                {
                    return this.PAGE_ParentIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PAGE_ParentIDValue = value.GetValueOrDefault();
                this.PAGE_ParentIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_ID", Namespace="http://tempuri.org/kartrisPagesData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("PAGE_SEOPageTitle", Namespace="http://tempuri.org/kartrisPagesData.xsd")]
        public string PAGE_SEOPageTitle { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("PAGE_MetaDescription", Namespace="http://tempuri.org/kartrisPagesData.xsd")]
        public string PAGE_MetaDescription { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("PAGE_MetaKeywords", Namespace="http://tempuri.org/kartrisPagesData.xsd")]
        public string PAGE_MetaKeywords { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("PAGE_Text", Namespace="http://tempuri.org/kartrisPagesData.xsd")]
        public string PAGE_Text { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("Page_Title", Namespace="http://tempuri.org/kartrisPagesData.xsd")]
        public string Page_Title { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("PAGE_DateCreated", Namespace="http://tempuri.org/kartrisPagesData.xsd", DataType="dateTime")]
        public System.DateTime PAGE_DateCreatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PAGE_DateCreatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> PAGE_DateCreated
        {
            get
            {
                if (this.PAGE_DateCreatedValueSpecified)
                {
                    return this.PAGE_DateCreatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PAGE_DateCreatedValue = value.GetValueOrDefault();
                this.PAGE_DateCreatedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("PAGE_LastUpdated", Namespace="http://tempuri.org/kartrisPagesData.xsd", DataType="dateTime")]
        public System.DateTime PAGE_LastUpdatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PAGE_LastUpdatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> PAGE_LastUpdated
        {
            get
            {
                if (this.PAGE_LastUpdatedValueSpecified)
                {
                    return this.PAGE_LastUpdatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PAGE_LastUpdatedValue = value.GetValueOrDefault();
                this.PAGE_LastUpdatedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("PAGE_Live", Namespace="http://tempuri.org/kartrisPagesData.xsd")]
        public bool PAGE_LiveValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PAGE_LiveValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> PAGE_Live
        {
            get
            {
                if (this.PAGE_LiveValueSpecified)
                {
                    return this.PAGE_LiveValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PAGE_LiveValue = value.GetValueOrDefault();
                this.PAGE_LiveValueSpecified = value.HasValue;
            }
        }
    }

 