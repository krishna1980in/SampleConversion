
  using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisCurrenciesData", Namespace="http://tempuri.org/kartrisCurrenciesData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisCurrenciesData", Namespace="http://tempuri.org/kartrisCurrenciesData.xsd")]
    public partial class KartrisCurrenciesData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisCurrenciesDataCurrencies> _currencies;
        
        [System.Xml.Serialization.XmlElementAttribute("Currencies", Namespace="http://tempuri.org/kartrisCurrenciesData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisCurrenciesDataCurrencies> Currencies
        {
            get
            {
                return this._currencies;
            }
            private set
            {
                this._currencies = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CurrenciesSpecified
        {
            get
            {
                return (this.Currencies.Count != 0);
            }
        }
        
        public KartrisCurrenciesData()
        {
            this._currencies = new System.Collections.ObjectModel.Collection<KartrisCurrenciesDataCurrencies>();
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisCurrenciesDataCurrencies", Namespace="http://tempuri.org/kartrisCurrenciesData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisCurrenciesDataCurrencies
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CUR_ID", Namespace="http://tempuri.org/kartrisCurrenciesData.xsd")]
        public byte CUR_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CUR_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> CUR_ID
        {
            get
            {
                if (this.CUR_IDValueSpecified)
                {
                    return this.CUR_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CUR_IDValue = value.GetValueOrDefault();
                this.CUR_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(5)]
        [System.Xml.Serialization.XmlElementAttribute("CUR_Symbol", Namespace="http://tempuri.org/kartrisCurrenciesData.xsd")]
        public string CUR_Symbol { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
        [System.Xml.Serialization.XmlElementAttribute("CUR_ISOCode", Namespace="http://tempuri.org/kartrisCurrenciesData.xsd")]
        public string CUR_ISOCode { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
        [System.Xml.Serialization.XmlElementAttribute("CUR_ISOCodeNumeric", Namespace="http://tempuri.org/kartrisCurrenciesData.xsd")]
        public string CUR_ISOCodeNumeric { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CUR_ExchangeRate", Namespace="http://tempuri.org/kartrisCurrenciesData.xsd")]
        public decimal CUR_ExchangeRateValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CUR_ExchangeRateValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> CUR_ExchangeRate
        {
            get
            {
                if (this.CUR_ExchangeRateValueSpecified)
                {
                    return this.CUR_ExchangeRateValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CUR_ExchangeRateValue = value.GetValueOrDefault();
                this.CUR_ExchangeRateValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CUR_HasDecimals", Namespace="http://tempuri.org/kartrisCurrenciesData.xsd")]
        public bool CUR_HasDecimalsValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CUR_HasDecimalsValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> CUR_HasDecimals
        {
            get
            {
                if (this.CUR_HasDecimalsValueSpecified)
                {
                    return this.CUR_HasDecimalsValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CUR_HasDecimalsValue = value.GetValueOrDefault();
                this.CUR_HasDecimalsValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CUR_Live", Namespace="http://tempuri.org/kartrisCurrenciesData.xsd")]
        public bool CUR_LiveValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CUR_LiveValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> CUR_Live
        {
            get
            {
                if (this.CUR_LiveValueSpecified)
                {
                    return this.CUR_LiveValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CUR_LiveValue = value.GetValueOrDefault();
                this.CUR_LiveValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("CUR_Format", Namespace="http://tempuri.org/kartrisCurrenciesData.xsd")]
        public string CUR_Format { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("CUR_IsoFormat", Namespace="http://tempuri.org/kartrisCurrenciesData.xsd")]
        public string CUR_IsoFormat { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("CUR_DecimalPoint", Namespace="http://tempuri.org/kartrisCurrenciesData.xsd")]
        public string CUR_DecimalPoint { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CUR_RoundNumbers", Namespace="http://tempuri.org/kartrisCurrenciesData.xsd")]
        public byte CUR_RoundNumbersValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CUR_RoundNumbersValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> CUR_RoundNumbers
        {
            get
            {
                if (this.CUR_RoundNumbersValueSpecified)
                {
                    return this.CUR_RoundNumbersValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CUR_RoundNumbersValue = value.GetValueOrDefault();
                this.CUR_RoundNumbersValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CUR_OrderNo", Namespace="http://tempuri.org/kartrisCurrenciesData.xsd")]
        public byte CUR_OrderNoValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CUR_OrderNoValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> CUR_OrderNo
        {
            get
            {
                if (this.CUR_OrderNoValueSpecified)
                {
                    return this.CUR_OrderNoValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CUR_OrderNoValue = value.GetValueOrDefault();
                this.CUR_OrderNoValueSpecified = value.HasValue;
            }
        }
    }
