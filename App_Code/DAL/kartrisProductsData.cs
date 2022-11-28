
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisProductsData", Namespace="http://tempuri.org/kartrisProductsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisProductsData", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
    public partial class KartrisProductsData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisProductsDataProducts> _products;
        
        [System.Xml.Serialization.XmlElementAttribute("Products", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisProductsDataProducts> Products
        {
            get
            {
                return this._products;
            }
            private set
            {
                this._products = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ProductsSpecified
        {
            get
            {
                return (this.Products.Count != 0);
            }
        }
        
        public KartrisProductsData()
        {
            this._products = new System.Collections.ObjectModel.Collection<KartrisProductsDataProducts>();
            this._productName = new System.Collections.ObjectModel.Collection<KartrisProductsDataProductName>();
            this._productCategoryLink = new System.Collections.ObjectModel.Collection<KartrisProductsDataProductCategoryLink>();
            this._relatedProducts = new System.Collections.ObjectModel.Collection<KartrisProductsDataRelatedProducts>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisProductsDataProductName> _productName;
        
        [System.Xml.Serialization.XmlElementAttribute("ProductName", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisProductsDataProductName> ProductName
        {
            get
            {
                return this._productName;
            }
            private set
            {
                this._productName = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ProductNameSpecified
        {
            get
            {
                return (this.ProductName.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisProductsDataProductCategoryLink> _productCategoryLink;
        
        [System.Xml.Serialization.XmlElementAttribute("ProductCategoryLink", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisProductsDataProductCategoryLink> ProductCategoryLink
        {
            get
            {
                return this._productCategoryLink;
            }
            private set
            {
                this._productCategoryLink = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ProductCategoryLinkSpecified
        {
            get
            {
                return (this.ProductCategoryLink.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisProductsDataRelatedProducts> _relatedProducts;
        
        [System.Xml.Serialization.XmlElementAttribute("RelatedProducts", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisProductsDataRelatedProducts> RelatedProducts
        {
            get
            {
                return this._relatedProducts;
            }
            private set
            {
                this._relatedProducts = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RelatedProductsSpecified
        {
            get
            {
                return (this.RelatedProducts.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisProductsDataProducts", Namespace="http://tempuri.org/kartrisProductsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisProductsDataProducts
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("P_ID", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public int P_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool P_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> P_ID
        {
            get
            {
                if (this.P_IDValueSpecified)
                {
                    return this.P_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.P_IDValue = value.GetValueOrDefault();
                this.P_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("P_OrderVersionsBy", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public string P_OrderVersionsBy { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("P_VersionDisplayType", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public string P_VersionDisplayType { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("P_Type", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public string P_Type { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("MinPrice", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public float MinPriceValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool MinPriceValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> MinPrice
        {
            get
            {
                if (this.MinPriceValueSpecified)
                {
                    return this.MinPriceValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.MinPriceValue = value.GetValueOrDefault();
                this.MinPriceValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("P_Name", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public string P_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("P_Desc", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public string P_Desc { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("P_StrapLine", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public string P_StrapLine { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("P_Featured", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public byte P_FeaturedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool P_FeaturedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> P_Featured
        {
            get
            {
                if (this.P_FeaturedValueSpecified)
                {
                    return this.P_FeaturedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.P_FeaturedValue = value.GetValueOrDefault();
                this.P_FeaturedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("MinTax", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public float MinTaxValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool MinTaxValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> MinTax
        {
            get
            {
                if (this.MinTaxValueSpecified)
                {
                    return this.MinTaxValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.MinTaxValue = value.GetValueOrDefault();
                this.MinTaxValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("P_VersionsSortDirection", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public string P_VersionsSortDirection { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("P_Reviews", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public string P_Reviews { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("P_PageTitle", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public string P_PageTitle { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("P_CustomerGroupID", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public short P_CustomerGroupIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool P_CustomerGroupIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> P_CustomerGroupID
        {
            get
            {
                if (this.P_CustomerGroupIDValueSpecified)
                {
                    return this.P_CustomerGroupIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.P_CustomerGroupIDValue = value.GetValueOrDefault();
                this.P_CustomerGroupIDValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisProductsDataProductName", Namespace="http://tempuri.org/kartrisProductsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisProductsDataProductName
    {
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("P_Name", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public string P_Name { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisProductsDataProductCategoryLink", Namespace="http://tempuri.org/kartrisProductsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisProductsDataProductCategoryLink
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("PCAT_ProductID", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public int PCAT_ProductIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PCAT_ProductIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> PCAT_ProductID
        {
            get
            {
                if (this.PCAT_ProductIDValueSpecified)
                {
                    return this.PCAT_ProductIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PCAT_ProductIDValue = value.GetValueOrDefault();
                this.PCAT_ProductIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("PCAT_CategoryID", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public int PCAT_CategoryIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PCAT_CategoryIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> PCAT_CategoryID
        {
            get
            {
                if (this.PCAT_CategoryIDValueSpecified)
                {
                    return this.PCAT_CategoryIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PCAT_CategoryIDValue = value.GetValueOrDefault();
                this.PCAT_CategoryIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("PCAT_OrderNo", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public short PCAT_OrderNoValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PCAT_OrderNoValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> PCAT_OrderNo
        {
            get
            {
                if (this.PCAT_OrderNoValueSpecified)
                {
                    return this.PCAT_OrderNoValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PCAT_OrderNoValue = value.GetValueOrDefault();
                this.PCAT_OrderNoValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisProductsDataRelatedProducts", Namespace="http://tempuri.org/kartrisProductsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisProductsDataRelatedProducts
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("RP_ParentID", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public int RP_ParentIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool RP_ParentIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> RP_ParentID
        {
            get
            {
                if (this.RP_ParentIDValueSpecified)
                {
                    return this.RP_ParentIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.RP_ParentIDValue = value.GetValueOrDefault();
                this.RP_ParentIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("RP_ChildID", Namespace="http://tempuri.org/kartrisProductsData.xsd")]
        public int RP_ChildIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool RP_ChildIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> RP_ChildID
        {
            get
            {
                if (this.RP_ChildIDValueSpecified)
                {
                    return this.RP_ChildIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.RP_ChildIDValue = value.GetValueOrDefault();
                this.RP_ChildIDValueSpecified = value.HasValue;
            }
        }
    }

 