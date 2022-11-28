
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisOptionsData", Namespace="http://tempuri.org/kartrisOptionsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisOptionsData", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
    public partial class KartrisOptionsData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisOptionsDataOptionGroups> _optionGroups;
        
        [System.Xml.Serialization.XmlElementAttribute("OptionGroups", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisOptionsDataOptionGroups> OptionGroups
        {
            get
            {
                return this._optionGroups;
            }
            private set
            {
                this._optionGroups = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool OptionGroupsSpecified
        {
            get
            {
                return (this.OptionGroups.Count != 0);
            }
        }
        
        public KartrisOptionsData()
        {
            this._optionGroups = new System.Collections.ObjectModel.Collection<KartrisOptionsDataOptionGroups>();
            this._options = new System.Collections.ObjectModel.Collection<KartrisOptionsDataOptions>();
            this._productOptionGroupLink = new System.Collections.ObjectModel.Collection<KartrisOptionsDataProductOptionGroupLink>();
            this._productOptionLink = new System.Collections.ObjectModel.Collection<KartrisOptionsDataProductOptionLink>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisOptionsDataOptions> _options;
        
        [System.Xml.Serialization.XmlElementAttribute("Options", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisOptionsDataOptions> Options
        {
            get
            {
                return this._options;
            }
            private set
            {
                this._options = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool OptionsSpecified
        {
            get
            {
                return (this.Options.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisOptionsDataProductOptionGroupLink> _productOptionGroupLink;
        
        [System.Xml.Serialization.XmlElementAttribute("ProductOptionGroupLink", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisOptionsDataProductOptionGroupLink> ProductOptionGroupLink
        {
            get
            {
                return this._productOptionGroupLink;
            }
            private set
            {
                this._productOptionGroupLink = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ProductOptionGroupLinkSpecified
        {
            get
            {
                return (this.ProductOptionGroupLink.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisOptionsDataProductOptionLink> _productOptionLink;
        
        [System.Xml.Serialization.XmlElementAttribute("ProductOptionLink", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisOptionsDataProductOptionLink> ProductOptionLink
        {
            get
            {
                return this._productOptionLink;
            }
            private set
            {
                this._productOptionLink = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ProductOptionLinkSpecified
        {
            get
            {
                return (this.ProductOptionLink.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisOptionsDataOptionGroups", Namespace="http://tempuri.org/kartrisOptionsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisOptionsDataOptionGroups
    {
        
        [System.Xml.Serialization.XmlElementAttribute("OPTG_ID", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public short OPTG_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("OPTG_BackendName", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public string OPTG_BackendName { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("OPTG_OptionDisplayType", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public string OPTG_OptionDisplayType { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("OPTG_DefOrderByValue", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public short OPTG_DefOrderByValueValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool OPTG_DefOrderByValueValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> OPTG_DefOrderByValue
        {
            get
            {
                if (this.OPTG_DefOrderByValueValueSpecified)
                {
                    return this.OPTG_DefOrderByValueValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.OPTG_DefOrderByValueValue = value.GetValueOrDefault();
                this.OPTG_DefOrderByValueValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisOptionsDataOptions", Namespace="http://tempuri.org/kartrisOptionsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisOptionsDataOptions
    {
        
        [System.Xml.Serialization.XmlElementAttribute("OPT_ID", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public int OPT_ID { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("OPT_OptionGroupID", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public short OPT_OptionGroupIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool OPT_OptionGroupIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> OPT_OptionGroupID
        {
            get
            {
                if (this.OPT_OptionGroupIDValueSpecified)
                {
                    return this.OPT_OptionGroupIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.OPT_OptionGroupIDValue = value.GetValueOrDefault();
                this.OPT_OptionGroupIDValueSpecified = value.HasValue;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute("OPT_CheckBoxValue", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public bool OPT_CheckBoxValue { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("OPT_DefPriceChange", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public decimal OPT_DefPriceChangeValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool OPT_DefPriceChangeValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> OPT_DefPriceChange
        {
            get
            {
                if (this.OPT_DefPriceChangeValueSpecified)
                {
                    return this.OPT_DefPriceChangeValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.OPT_DefPriceChangeValue = value.GetValueOrDefault();
                this.OPT_DefPriceChangeValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("OPT_DefWeightChange", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public float OPT_DefWeightChangeValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool OPT_DefWeightChangeValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> OPT_DefWeightChange
        {
            get
            {
                if (this.OPT_DefWeightChangeValueSpecified)
                {
                    return this.OPT_DefWeightChangeValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.OPT_DefWeightChangeValue = value.GetValueOrDefault();
                this.OPT_DefWeightChangeValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("OPT_DefOrderByValue", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public short OPT_DefOrderByValueValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool OPT_DefOrderByValueValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> OPT_DefOrderByValue
        {
            get
            {
                if (this.OPT_DefOrderByValueValueSpecified)
                {
                    return this.OPT_DefOrderByValueValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.OPT_DefOrderByValueValue = value.GetValueOrDefault();
                this.OPT_DefOrderByValueValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisOptionsDataProductOptionGroupLink", Namespace="http://tempuri.org/kartrisOptionsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisOptionsDataProductOptionGroupLink
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("P_OPTG_ProductID", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public int P_OPTG_ProductIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool P_OPTG_ProductIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> P_OPTG_ProductID
        {
            get
            {
                if (this.P_OPTG_ProductIDValueSpecified)
                {
                    return this.P_OPTG_ProductIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.P_OPTG_ProductIDValue = value.GetValueOrDefault();
                this.P_OPTG_ProductIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("P_OPTG_OptionGroupID", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public short P_OPTG_OptionGroupIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool P_OPTG_OptionGroupIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> P_OPTG_OptionGroupID
        {
            get
            {
                if (this.P_OPTG_OptionGroupIDValueSpecified)
                {
                    return this.P_OPTG_OptionGroupIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.P_OPTG_OptionGroupIDValue = value.GetValueOrDefault();
                this.P_OPTG_OptionGroupIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("P_OPTG_OrderByValue", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public short P_OPTG_OrderByValueValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool P_OPTG_OrderByValueValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> P_OPTG_OrderByValue
        {
            get
            {
                if (this.P_OPTG_OrderByValueValueSpecified)
                {
                    return this.P_OPTG_OrderByValueValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.P_OPTG_OrderByValueValue = value.GetValueOrDefault();
                this.P_OPTG_OrderByValueValueSpecified = value.HasValue;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute("P_OPTG_MustSelected", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public bool P_OPTG_MustSelected { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisOptionsDataProductOptionLink", Namespace="http://tempuri.org/kartrisOptionsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisOptionsDataProductOptionLink
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("P_OPT_OptionID", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public int P_OPT_OptionIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool P_OPT_OptionIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> P_OPT_OptionID
        {
            get
            {
                if (this.P_OPT_OptionIDValueSpecified)
                {
                    return this.P_OPT_OptionIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.P_OPT_OptionIDValue = value.GetValueOrDefault();
                this.P_OPT_OptionIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("P_OPT_ProductID", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public int P_OPT_ProductIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool P_OPT_ProductIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> P_OPT_ProductID
        {
            get
            {
                if (this.P_OPT_ProductIDValueSpecified)
                {
                    return this.P_OPT_ProductIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.P_OPT_ProductIDValue = value.GetValueOrDefault();
                this.P_OPT_ProductIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("P_OPT_OrderByValue", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public short P_OPT_OrderByValueValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool P_OPT_OrderByValueValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> P_OPT_OrderByValue
        {
            get
            {
                if (this.P_OPT_OrderByValueValueSpecified)
                {
                    return this.P_OPT_OrderByValueValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.P_OPT_OrderByValueValue = value.GetValueOrDefault();
                this.P_OPT_OrderByValueValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("P_OPT_PriceChange", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public decimal P_OPT_PriceChangeValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool P_OPT_PriceChangeValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> P_OPT_PriceChange
        {
            get
            {
                if (this.P_OPT_PriceChangeValueSpecified)
                {
                    return this.P_OPT_PriceChangeValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.P_OPT_PriceChangeValue = value.GetValueOrDefault();
                this.P_OPT_PriceChangeValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("P_OPT_WeightChange", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public float P_OPT_WeightChangeValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool P_OPT_WeightChangeValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> P_OPT_WeightChange
        {
            get
            {
                if (this.P_OPT_WeightChangeValueSpecified)
                {
                    return this.P_OPT_WeightChangeValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.P_OPT_WeightChangeValue = value.GetValueOrDefault();
                this.P_OPT_WeightChangeValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("P_OPT_Selected", Namespace="http://tempuri.org/kartrisOptionsData.xsd")]
        public bool P_OPT_SelectedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool P_OPT_SelectedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> P_OPT_Selected
        {
            get
            {
                if (this.P_OPT_SelectedValueSpecified)
                {
                    return this.P_OPT_SelectedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.P_OPT_SelectedValue = value.GetValueOrDefault();
                this.P_OPT_SelectedValueSpecified = value.HasValue;
            }
        }
    }

 