
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisMediaData", Namespace="http://tempuri.org/kartrisMediaData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisMediaData", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
    public partial class KartrisMediaData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisMediaDataMediaTypes> _mediaTypes;
        
        [System.Xml.Serialization.XmlElementAttribute("MediaTypes", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisMediaDataMediaTypes> MediaTypes
        {
            get
            {
                return this._mediaTypes;
            }
            private set
            {
                this._mediaTypes = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MediaTypesSpecified
        {
            get
            {
                return (this.MediaTypes.Count != 0);
            }
        }
        
        public KartrisMediaData()
        {
            this._mediaTypes = new System.Collections.ObjectModel.Collection<KartrisMediaDataMediaTypes>();
            this._mediaLinks = new System.Collections.ObjectModel.Collection<KartrisMediaDataMediaLinks>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisMediaDataMediaLinks> _mediaLinks;
        
        [System.Xml.Serialization.XmlElementAttribute("MediaLinks", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisMediaDataMediaLinks> MediaLinks
        {
            get
            {
                return this._mediaLinks;
            }
            private set
            {
                this._mediaLinks = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MediaLinksSpecified
        {
            get
            {
                return (this.MediaLinks.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisMediaDataMediaTypes", Namespace="http://tempuri.org/kartrisMediaData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisMediaDataMediaTypes
    {
        
        [System.Xml.Serialization.XmlElementAttribute("MT_ID", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public short MT_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("MT_Extension", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public string MT_Extension { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("MT_DefaultHeight", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public int MT_DefaultHeightValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool MT_DefaultHeightValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> MT_DefaultHeight
        {
            get
            {
                if (this.MT_DefaultHeightValueSpecified)
                {
                    return this.MT_DefaultHeightValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.MT_DefaultHeightValue = value.GetValueOrDefault();
                this.MT_DefaultHeightValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("MT_DefaultWidth", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public int MT_DefaultWidthValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool MT_DefaultWidthValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> MT_DefaultWidth
        {
            get
            {
                if (this.MT_DefaultWidthValueSpecified)
                {
                    return this.MT_DefaultWidthValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.MT_DefaultWidthValue = value.GetValueOrDefault();
                this.MT_DefaultWidthValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1000)]
        [System.Xml.Serialization.XmlElementAttribute("MT_DefaultParameters", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public string MT_DefaultParameters { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("MT_DefaultisDownloadable", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public bool MT_DefaultisDownloadableValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool MT_DefaultisDownloadableValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> MT_DefaultisDownloadable
        {
            get
            {
                if (this.MT_DefaultisDownloadableValueSpecified)
                {
                    return this.MT_DefaultisDownloadableValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.MT_DefaultisDownloadableValue = value.GetValueOrDefault();
                this.MT_DefaultisDownloadableValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("MT_Embed", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public bool MT_EmbedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool MT_EmbedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> MT_Embed
        {
            get
            {
                if (this.MT_EmbedValueSpecified)
                {
                    return this.MT_EmbedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.MT_EmbedValue = value.GetValueOrDefault();
                this.MT_EmbedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("MT_Inline", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public bool MT_InlineValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool MT_InlineValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> MT_Inline
        {
            get
            {
                if (this.MT_InlineValueSpecified)
                {
                    return this.MT_InlineValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.MT_InlineValue = value.GetValueOrDefault();
                this.MT_InlineValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisMediaDataMediaLinks", Namespace="http://tempuri.org/kartrisMediaData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisMediaDataMediaLinks
    {
        
        [System.Xml.Serialization.XmlElementAttribute("ML_ID", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public int ML_ID { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("ML_ParentID", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public long ML_ParentIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool ML_ParentIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> ML_ParentID
        {
            get
            {
                if (this.ML_ParentIDValueSpecified)
                {
                    return this.ML_ParentIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.ML_ParentIDValue = value.GetValueOrDefault();
                this.ML_ParentIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("ML_ParentType", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public string ML_ParentType { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1000)]
        [System.Xml.Serialization.XmlElementAttribute("ML_EmbedSource", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public string ML_EmbedSource { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("ML_MediaTypeID", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public short ML_MediaTypeIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool ML_MediaTypeIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> ML_MediaTypeID
        {
            get
            {
                if (this.ML_MediaTypeIDValueSpecified)
                {
                    return this.ML_MediaTypeIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.ML_MediaTypeIDValue = value.GetValueOrDefault();
                this.ML_MediaTypeIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("ML_Height", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public int ML_HeightValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool ML_HeightValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> ML_Height
        {
            get
            {
                if (this.ML_HeightValueSpecified)
                {
                    return this.ML_HeightValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.ML_HeightValue = value.GetValueOrDefault();
                this.ML_HeightValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("ML_Width", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public int ML_WidthValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool ML_WidthValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> ML_Width
        {
            get
            {
                if (this.ML_WidthValueSpecified)
                {
                    return this.ML_WidthValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.ML_WidthValue = value.GetValueOrDefault();
                this.ML_WidthValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("ML_isDownloadable", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public bool ML_IsDownloadableValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool ML_IsDownloadableValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> ML_IsDownloadable
        {
            get
            {
                if (this.ML_IsDownloadableValueSpecified)
                {
                    return this.ML_IsDownloadableValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.ML_IsDownloadableValue = value.GetValueOrDefault();
                this.ML_IsDownloadableValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1000)]
        [System.Xml.Serialization.XmlElementAttribute("ML_Parameters", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public string ML_Parameters { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("MT_DefaultHeight", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public int MT_DefaultHeightValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool MT_DefaultHeightValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> MT_DefaultHeight
        {
            get
            {
                if (this.MT_DefaultHeightValueSpecified)
                {
                    return this.MT_DefaultHeightValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.MT_DefaultHeightValue = value.GetValueOrDefault();
                this.MT_DefaultHeightValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("MT_DefaultWidth", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public int MT_DefaultWidthValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool MT_DefaultWidthValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> MT_DefaultWidth
        {
            get
            {
                if (this.MT_DefaultWidthValueSpecified)
                {
                    return this.MT_DefaultWidthValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.MT_DefaultWidthValue = value.GetValueOrDefault();
                this.MT_DefaultWidthValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1000)]
        [System.Xml.Serialization.XmlElementAttribute("MT_DefaultParameters", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public string MT_DefaultParameters { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("MT_DefaultisDownloadable", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public bool MT_DefaultisDownloadableValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool MT_DefaultisDownloadableValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> MT_DefaultisDownloadable
        {
            get
            {
                if (this.MT_DefaultisDownloadableValueSpecified)
                {
                    return this.MT_DefaultisDownloadableValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.MT_DefaultisDownloadableValue = value.GetValueOrDefault();
                this.MT_DefaultisDownloadableValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("MT_Extension", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public string MT_Extension { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("MT_Embed", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public bool MT_EmbedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool MT_EmbedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> MT_Embed
        {
            get
            {
                if (this.MT_EmbedValueSpecified)
                {
                    return this.MT_EmbedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.MT_EmbedValue = value.GetValueOrDefault();
                this.MT_EmbedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("ML_Live", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public bool ML_LiveValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool ML_LiveValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> ML_Live
        {
            get
            {
                if (this.ML_LiveValueSpecified)
                {
                    return this.ML_LiveValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.ML_LiveValue = value.GetValueOrDefault();
                this.ML_LiveValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("MT_Inline", Namespace="http://tempuri.org/kartrisMediaData.xsd")]
        public bool MT_InlineValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool MT_InlineValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> MT_Inline
        {
            get
            {
                if (this.MT_InlineValueSpecified)
                {
                    return this.MT_InlineValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.MT_InlineValue = value.GetValueOrDefault();
                this.MT_InlineValueSpecified = value.HasValue;
            }
        }
    }

 