

    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisTaxData", Namespace="http://tempuri.org/kartrisTaxData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisTaxData", Namespace="http://tempuri.org/kartrisTaxData.xsd")]
    public partial class KartrisTaxData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisTaxDataTaxRates> _taxRates;
        
        [System.Xml.Serialization.XmlElementAttribute("TaxRates", Namespace="http://tempuri.org/kartrisTaxData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisTaxDataTaxRates> TaxRates
        {
            get
            {
                return this._taxRates;
            }
            private set
            {
                this._taxRates = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TaxRatesSpecified
        {
            get
            {
                return (this.TaxRates.Count != 0);
            }
        }
        
        public KartrisTaxData()
        {
            this._taxRates = new System.Collections.ObjectModel.Collection<KartrisTaxDataTaxRates>();
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisTaxDataTaxRates", Namespace="http://tempuri.org/kartrisTaxData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisTaxDataTaxRates
    {
        
        [System.Xml.Serialization.XmlElementAttribute("T_ID", Namespace="http://tempuri.org/kartrisTaxData.xsd")]
        public byte T_ID { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("T_Taxrate", Namespace="http://tempuri.org/kartrisTaxData.xsd")]
        public decimal T_Taxrate { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(19)]
        [System.Xml.Serialization.XmlElementAttribute("T_TaxRateString", Namespace="http://tempuri.org/kartrisTaxData.xsd")]
        public string T_TaxRateString { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("T_QBRefCode", Namespace="http://tempuri.org/kartrisTaxData.xsd")]
        public string T_QBRefCode { get; set; }
    }

 