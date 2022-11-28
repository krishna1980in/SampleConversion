
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisLabelFormatData", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisLabelFormatData", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
    public partial class KartrisLabelFormatData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisLabelFormatDataTblKartrisLabelFormats> _tblKartrisLabelFormats;
        
        [System.Xml.Serialization.XmlElementAttribute("tblKartrisLabelFormats", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisLabelFormatDataTblKartrisLabelFormats> TblKartrisLabelFormats
        {
            get
            {
                return this._tblKartrisLabelFormats;
            }
            private set
            {
                this._tblKartrisLabelFormats = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TblKartrisLabelFormatsSpecified
        {
            get
            {
                return (this.TblKartrisLabelFormats.Count != 0);
            }
        }
        
        public KartrisLabelFormatData()
        {
            this._tblKartrisLabelFormats = new System.Collections.ObjectModel.Collection<KartrisLabelFormatDataTblKartrisLabelFormats>();
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisLabelFormatDataTblKartrisLabelFormats", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisLabelFormatDataTblKartrisLabelFormats
    {
        
        [System.Xml.Serialization.XmlElementAttribute("LBF_ID", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public int LBF_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("LBF_LabelName", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public string LBF_LabelName { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1024)]
        [System.Xml.Serialization.XmlElementAttribute("LBF_LabelDescription", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public string LBF_LabelDescription { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LBF_PageWidth", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public double LBF_PageWidth { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LBF_PageHeight", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public double LBF_PageHeight { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LBF_TopMargin", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public double LBF_TopMargin { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LBF_LeftMargin", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public double LBF_LeftMargin { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LBF_LabelWidth", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public double LBF_LabelWidth { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LBF_LabelHeight", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public double LBF_LabelHeight { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LBF_LabelPaddingTop", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public double LBF_LabelPaddingTop { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LBF_LabelPaddingBottom", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public double LBF_LabelPaddingBottom { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LBF_LabelPaddingLeft", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public double LBF_LabelPaddingLeft { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LBF_LabelPaddingRight", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public double LBF_LabelPaddingRight { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LBF_VerticalPitch", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public double LBF_VerticalPitch { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LBF_HorizontalPitch", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public double LBF_HorizontalPitch { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LBF_LabelColumnCount", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public int LBF_LabelColumnCount { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LBF_LabelRowCount", Namespace="http://tempuri.org/kartrisLabelFormatData.xsd")]
        public int LBF_LabelRowCount { get; set; }
    }

 