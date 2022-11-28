

    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisSubSitesData", Namespace="http://tempuri.org/kartrisSubSitesData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisSubSitesData", Namespace="http://tempuri.org/kartrisSubSitesData.xsd")]
    public partial class KartrisSubSitesData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisSubSitesDataSubSites> _subSites;
        
        [System.Xml.Serialization.XmlElementAttribute("SubSites", Namespace="http://tempuri.org/kartrisSubSitesData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisSubSitesDataSubSites> SubSites
        {
            get
            {
                return this._subSites;
            }
            private set
            {
                this._subSites = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SubSitesSpecified
        {
            get
            {
                return (this.SubSites.Count != 0);
            }
        }
        
        public KartrisSubSitesData()
        {
            this._subSites = new System.Collections.ObjectModel.Collection<KartrisSubSitesDataSubSites>();
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisSubSitesDataSubSites", Namespace="http://tempuri.org/kartrisSubSitesData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisSubSitesDataSubSites
    {
        
        [System.Xml.Serialization.XmlElementAttribute("SUB_ID", Namespace="http://tempuri.org/kartrisSubSitesData.xsd")]
        public short SUB_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("SUB_Name", Namespace="http://tempuri.org/kartrisSubSitesData.xsd")]
        public string SUB_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("SUB_Domain", Namespace="http://tempuri.org/kartrisSubSitesData.xsd")]
        public string SUB_Domain { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SUB_BaseCategoryID", Namespace="http://tempuri.org/kartrisSubSitesData.xsd")]
        public long SUB_BaseCategoryIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SUB_BaseCategoryIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> SUB_BaseCategoryID
        {
            get
            {
                if (this.SUB_BaseCategoryIDValueSpecified)
                {
                    return this.SUB_BaseCategoryIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SUB_BaseCategoryIDValue = value.GetValueOrDefault();
                this.SUB_BaseCategoryIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("SUB_Skin", Namespace="http://tempuri.org/kartrisSubSitesData.xsd")]
        public string SUB_Skin { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("SUB_Notes", Namespace="http://tempuri.org/kartrisSubSitesData.xsd")]
        public string SUB_Notes { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SUB_Live", Namespace="http://tempuri.org/kartrisSubSitesData.xsd")]
        public bool SUB_LiveValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SUB_LiveValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> SUB_Live
        {
            get
            {
                if (this.SUB_LiveValueSpecified)
                {
                    return this.SUB_LiveValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SUB_LiveValue = value.GetValueOrDefault();
                this.SUB_LiveValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("CAT_Name", Namespace="http://tempuri.org/kartrisSubSitesData.xsd")]
        public string CAT_Name { get; set; }
    }

 