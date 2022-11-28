
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisCategoriesData", Namespace="http://tempuri.org/kartrisCategoriesData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisCategoriesData", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
    public partial class KartrisCategoriesData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisCategoriesDataCategories> _categories;
        
        [System.Xml.Serialization.XmlElementAttribute("Categories", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisCategoriesDataCategories> Categories
        {
            get
            {
                return this._categories;
            }
            private set
            {
                this._categories = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CategoriesSpecified
        {
            get
            {
                return (this.Categories.Count != 0);
            }
        }
        
        public KartrisCategoriesData()
        {
            this._categories = new System.Collections.ObjectModel.Collection<KartrisCategoriesDataCategories>();
            this._categoryHierarchy = new System.Collections.ObjectModel.Collection<KartrisCategoriesDataCategoryHierarchy>();
            this._categoryName = new System.Collections.ObjectModel.Collection<KartrisCategoriesDataCategoryName>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisCategoriesDataCategoryHierarchy> _categoryHierarchy;
        
        [System.Xml.Serialization.XmlElementAttribute("CategoryHierarchy", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisCategoriesDataCategoryHierarchy> CategoryHierarchy
        {
            get
            {
                return this._categoryHierarchy;
            }
            private set
            {
                this._categoryHierarchy = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CategoryHierarchySpecified
        {
            get
            {
                return (this.CategoryHierarchy.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisCategoriesDataCategoryName> _categoryName;
        
        [System.Xml.Serialization.XmlElementAttribute("CategoryName", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisCategoriesDataCategoryName> CategoryName
        {
            get
            {
                return this._categoryName;
            }
            private set
            {
                this._categoryName = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CategoryNameSpecified
        {
            get
            {
                return (this.CategoryName.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisCategoriesDataCategories", Namespace="http://tempuri.org/kartrisCategoriesData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisCategoriesDataCategories
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CAT_ID", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public int CAT_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CAT_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> CAT_ID
        {
            get
            {
                if (this.CAT_IDValueSpecified)
                {
                    return this.CAT_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CAT_IDValue = value.GetValueOrDefault();
                this.CAT_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_ID", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("CAT_Name", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public string CAT_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("CAT_Desc", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public string CAT_Desc { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CAT_Live", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public bool CAT_LiveValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CAT_LiveValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> CAT_Live
        {
            get
            {
                if (this.CAT_LiveValueSpecified)
                {
                    return this.CAT_LiveValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CAT_LiveValue = value.GetValueOrDefault();
                this.CAT_LiveValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("CAT_ProductDisplayType", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public string CAT_ProductDisplayType { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("CAT_SubCatDisplayType", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public string CAT_SubCatDisplayType { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("CAT_OrderProductsBy", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public string CAT_OrderProductsBy { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CAT_CustomerGroupID", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public short CAT_CustomerGroupIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CAT_CustomerGroupIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> CAT_CustomerGroupID
        {
            get
            {
                if (this.CAT_CustomerGroupIDValueSpecified)
                {
                    return this.CAT_CustomerGroupIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CAT_CustomerGroupIDValue = value.GetValueOrDefault();
                this.CAT_CustomerGroupIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("CAT_ProductsSortDirection", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public string CAT_ProductsSortDirection { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("CAT_PageTitle", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public string CAT_PageTitle { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("CAT_OrderCategoriesBy", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public string CAT_OrderCategoriesBy { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("CAT_CategoriesSortDirection", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public string CAT_CategoriesSortDirection { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisCategoriesDataCategoryHierarchy", Namespace="http://tempuri.org/kartrisCategoriesData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisCategoriesDataCategoryHierarchy
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CH_ParentID", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public int CH_ParentIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CH_ParentIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> CH_ParentID
        {
            get
            {
                if (this.CH_ParentIDValueSpecified)
                {
                    return this.CH_ParentIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CH_ParentIDValue = value.GetValueOrDefault();
                this.CH_ParentIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CH_ChildID", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public int CH_ChildIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CH_ChildIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> CH_ChildID
        {
            get
            {
                if (this.CH_ChildIDValueSpecified)
                {
                    return this.CH_ChildIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CH_ChildIDValue = value.GetValueOrDefault();
                this.CH_ChildIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CH_OrderNo", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public short CH_OrderNoValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CH_OrderNoValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> CH_OrderNo
        {
            get
            {
                if (this.CH_OrderNoValueSpecified)
                {
                    return this.CH_OrderNoValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CH_OrderNoValue = value.GetValueOrDefault();
                this.CH_OrderNoValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisCategoriesDataCategoryName", Namespace="http://tempuri.org/kartrisCategoriesData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisCategoriesDataCategoryName
    {
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("CAT_Name", Namespace="http://tempuri.org/kartrisCategoriesData.xsd")]
        public string CAT_Name { get; set; }
    }
