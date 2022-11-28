
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisReviewsData", Namespace="http://tempuri.org/kartrisReviewsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisReviewsData", Namespace="http://tempuri.org/kartrisReviewsData.xsd")]
    public partial class KartrisReviewsData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisReviewsDataReviews> _reviews;
        
        [System.Xml.Serialization.XmlElementAttribute("Reviews", Namespace="http://tempuri.org/kartrisReviewsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisReviewsDataReviews> Reviews
        {
            get
            {
                return this._reviews;
            }
            private set
            {
                this._reviews = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ReviewsSpecified
        {
            get
            {
                return (this.Reviews.Count != 0);
            }
        }
        
        public KartrisReviewsData()
        {
            this._reviews = new System.Collections.ObjectModel.Collection<KartrisReviewsDataReviews>();
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisReviewsDataReviews", Namespace="http://tempuri.org/kartrisReviewsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisReviewsDataReviews
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("REV_ID", Namespace="http://tempuri.org/kartrisReviewsData.xsd")]
        public short REV_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool REV_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> REV_ID
        {
            get
            {
                if (this.REV_IDValueSpecified)
                {
                    return this.REV_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.REV_IDValue = value.GetValueOrDefault();
                this.REV_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("REV_ProductID", Namespace="http://tempuri.org/kartrisReviewsData.xsd")]
        public int REV_ProductIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool REV_ProductIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> REV_ProductID
        {
            get
            {
                if (this.REV_ProductIDValueSpecified)
                {
                    return this.REV_ProductIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.REV_ProductIDValue = value.GetValueOrDefault();
                this.REV_ProductIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("REV_LanguageID", Namespace="http://tempuri.org/kartrisReviewsData.xsd")]
        public byte REV_LanguageIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool REV_LanguageIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> REV_LanguageID
        {
            get
            {
                if (this.REV_LanguageIDValueSpecified)
                {
                    return this.REV_LanguageIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.REV_LanguageIDValue = value.GetValueOrDefault();
                this.REV_LanguageIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("REV_CustomerID", Namespace="http://tempuri.org/kartrisReviewsData.xsd")]
        public int REV_CustomerIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool REV_CustomerIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> REV_CustomerID
        {
            get
            {
                if (this.REV_CustomerIDValueSpecified)
                {
                    return this.REV_CustomerIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.REV_CustomerIDValue = value.GetValueOrDefault();
                this.REV_CustomerIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(60)]
        [System.Xml.Serialization.XmlElementAttribute("REV_Title", Namespace="http://tempuri.org/kartrisReviewsData.xsd")]
        public string REV_Title { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4000)]
        [System.Xml.Serialization.XmlElementAttribute("REV_Text", Namespace="http://tempuri.org/kartrisReviewsData.xsd")]
        public string REV_Text { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("REV_Rating", Namespace="http://tempuri.org/kartrisReviewsData.xsd")]
        public byte REV_RatingValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool REV_RatingValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> REV_Rating
        {
            get
            {
                if (this.REV_RatingValueSpecified)
                {
                    return this.REV_RatingValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.REV_RatingValue = value.GetValueOrDefault();
                this.REV_RatingValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
        [System.Xml.Serialization.XmlElementAttribute("REV_Name", Namespace="http://tempuri.org/kartrisReviewsData.xsd")]
        public string REV_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(75)]
        [System.Xml.Serialization.XmlElementAttribute("REV_Email", Namespace="http://tempuri.org/kartrisReviewsData.xsd")]
        public string REV_Email { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
        [System.Xml.Serialization.XmlElementAttribute("REV_Location", Namespace="http://tempuri.org/kartrisReviewsData.xsd")]
        public string REV_Location { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("REV_DateCreated", Namespace="http://tempuri.org/kartrisReviewsData.xsd", DataType="dateTime")]
        public System.DateTime REV_DateCreatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool REV_DateCreatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> REV_DateCreated
        {
            get
            {
                if (this.REV_DateCreatedValueSpecified)
                {
                    return this.REV_DateCreatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.REV_DateCreatedValue = value.GetValueOrDefault();
                this.REV_DateCreatedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("REV_DateLastUpdated", Namespace="http://tempuri.org/kartrisReviewsData.xsd", DataType="dateTime")]
        public System.DateTime REV_DateLastUpdatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool REV_DateLastUpdatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> REV_DateLastUpdated
        {
            get
            {
                if (this.REV_DateLastUpdatedValueSpecified)
                {
                    return this.REV_DateLastUpdatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.REV_DateLastUpdatedValue = value.GetValueOrDefault();
                this.REV_DateLastUpdatedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("REV_Live", Namespace="http://tempuri.org/kartrisReviewsData.xsd")]
        public string REV_Live { get; set; }
    }

 