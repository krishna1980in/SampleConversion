

    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisLanguageData", Namespace="http://tempuri.org/kartrisLanguageData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisLanguageData", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
    public partial class KartrisLanguageData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisLanguageDataLanguageElements> _languageElements;
        
        [System.Xml.Serialization.XmlElementAttribute("LanguageElements", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisLanguageDataLanguageElements> LanguageElements
        {
            get
            {
                return this._languageElements;
            }
            private set
            {
                this._languageElements = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LanguageElementsSpecified
        {
            get
            {
                return (this.LanguageElements.Count != 0);
            }
        }
        
        public KartrisLanguageData()
        {
            this._languageElements = new System.Collections.ObjectModel.Collection<KartrisLanguageDataLanguageElements>();
            this._languages = new System.Collections.ObjectModel.Collection<KartrisLanguageDataLanguages>();
            this._languageStrings = new System.Collections.ObjectModel.Collection<KartrisLanguageDataLanguageStrings>();
            this._languageElementTypeFields = new System.Collections.ObjectModel.Collection<KartrisLanguageDataLanguageElementTypeFields>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisLanguageDataLanguages> _languages;
        
        [System.Xml.Serialization.XmlElementAttribute("Languages", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisLanguageDataLanguages> Languages
        {
            get
            {
                return this._languages;
            }
            private set
            {
                this._languages = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LanguagesSpecified
        {
            get
            {
                return (this.Languages.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisLanguageDataLanguageStrings> _languageStrings;
        
        [System.Xml.Serialization.XmlElementAttribute("LanguageStrings", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisLanguageDataLanguageStrings> LanguageStrings
        {
            get
            {
                return this._languageStrings;
            }
            private set
            {
                this._languageStrings = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LanguageStringsSpecified
        {
            get
            {
                return (this.LanguageStrings.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisLanguageDataLanguageElementTypeFields> _languageElementTypeFields;
        
        [System.Xml.Serialization.XmlElementAttribute("LanguageElementTypeFields", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisLanguageDataLanguageElementTypeFields> LanguageElementTypeFields
        {
            get
            {
                return this._languageElementTypeFields;
            }
            private set
            {
                this._languageElementTypeFields = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LanguageElementTypeFieldsSpecified
        {
            get
            {
                return (this.LanguageElementTypeFields.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisLanguageDataLanguageElements", Namespace="http://tempuri.org/kartrisLanguageData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisLanguageDataLanguageElements
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LE_LanguageID", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public byte LE_LanguageIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LE_LanguageIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> LE_LanguageID
        {
            get
            {
                if (this.LE_LanguageIDValueSpecified)
                {
                    return this.LE_LanguageIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LE_LanguageIDValue = value.GetValueOrDefault();
                this.LE_LanguageIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LE_TypeID", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public byte LE_TypeIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LE_TypeIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> LE_TypeID
        {
            get
            {
                if (this.LE_TypeIDValueSpecified)
                {
                    return this.LE_TypeIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LE_TypeIDValue = value.GetValueOrDefault();
                this.LE_TypeIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LE_FieldID", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public byte LE_FieldIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LE_FieldIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> LE_FieldID
        {
            get
            {
                if (this.LE_FieldIDValueSpecified)
                {
                    return this.LE_FieldIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LE_FieldIDValue = value.GetValueOrDefault();
                this.LE_FieldIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LE_ParentID", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public long LE_ParentIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LE_ParentIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> LE_ParentID
        {
            get
            {
                if (this.LE_ParentIDValueSpecified)
                {
                    return this.LE_ParentIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LE_ParentIDValue = value.GetValueOrDefault();
                this.LE_ParentIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("LE_Value", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LE_Value { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisLanguageDataLanguages", Namespace="http://tempuri.org/kartrisLanguageData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisLanguageDataLanguages
    {
        
        [System.Xml.Serialization.XmlElementAttribute("LANG_ID", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public byte LANG_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_BackName", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LANG_BackName { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_FrontName", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LANG_FrontName { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_SkinLocation", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LANG_SkinLocation { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LANG_LiveFront", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public bool LANG_LiveFront { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LANG_LiveBack", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public bool LANG_LiveBack { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_EmailFrom", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LANG_EmailFrom { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_EmailTo", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LANG_EmailTo { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_EmailToContact", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LANG_EmailToContact { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_DateFormat", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LANG_DateFormat { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_DateAndTimeFormat", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LANG_DateAndTimeFormat { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_Culture", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LANG_Culture { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_UICulture", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LANG_UICulture { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_Master", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LANG_Master { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("LANG_Theme", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LANG_Theme { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisLanguageDataLanguageStrings", Namespace="http://tempuri.org/kartrisLanguageData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisLanguageDataLanguageStrings
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LS_ID", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public int LS_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LS_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> LS_ID
        {
            get
            {
                if (this.LS_IDValueSpecified)
                {
                    return this.LS_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LS_IDValue = value.GetValueOrDefault();
                this.LS_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("LS_FrontBack", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LS_FrontBack { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("LS_Name", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LS_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("LS_Value", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LS_Value { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("LS_Description", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LS_Description { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LS_VersionAdded", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public float LS_VersionAddedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LS_VersionAddedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> LS_VersionAdded
        {
            get
            {
                if (this.LS_VersionAddedValueSpecified)
                {
                    return this.LS_VersionAddedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LS_VersionAddedValue = value.GetValueOrDefault();
                this.LS_VersionAddedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("LS_DefaultValue", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LS_DefaultValue { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("LS_VirtualPath", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LS_VirtualPath { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("LS_ClassName", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LS_ClassName { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LS_LangID", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public byte LS_LangIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LS_LangIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> LS_LangID
        {
            get
            {
                if (this.LS_LangIDValueSpecified)
                {
                    return this.LS_LangIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LS_LangIDValue = value.GetValueOrDefault();
                this.LS_LangIDValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisLanguageDataLanguageElementTypeFields", Namespace="http://tempuri.org/kartrisLanguageData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisLanguageDataLanguageElementTypeFields
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LET_ID", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public byte LET_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LET_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> LET_ID
        {
            get
            {
                if (this.LET_IDValueSpecified)
                {
                    return this.LET_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LET_IDValue = value.GetValueOrDefault();
                this.LET_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LEFN_ID", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public byte LEFN_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LEFN_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> LEFN_ID
        {
            get
            {
                if (this.LEFN_IDValueSpecified)
                {
                    return this.LEFN_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LEFN_IDValue = value.GetValueOrDefault();
                this.LEFN_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("LEFN_Name", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LEFN_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("LEFN_DisplayText", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LEFN_DisplayText { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LEFN_IsMandatory", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public bool LEFN_IsMandatoryValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LEFN_IsMandatoryValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> LEFN_IsMandatory
        {
            get
            {
                if (this.LEFN_IsMandatoryValueSpecified)
                {
                    return this.LEFN_IsMandatoryValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LEFN_IsMandatoryValue = value.GetValueOrDefault();
                this.LEFN_IsMandatoryValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("LEFN_CssClass", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public string LEFN_CssClass { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LEFN_IsMultiLine", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public bool LEFN_IsMultiLineValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LEFN_IsMultiLineValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> LEFN_IsMultiLine
        {
            get
            {
                if (this.LEFN_IsMultiLineValueSpecified)
                {
                    return this.LEFN_IsMultiLineValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LEFN_IsMultiLineValue = value.GetValueOrDefault();
                this.LEFN_IsMultiLineValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LEFN_UseHTMLEditor", Namespace="http://tempuri.org/kartrisLanguageData.xsd")]
        public bool LEFN_UseHTMLEditorValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LEFN_UseHTMLEditorValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> LEFN_UseHTMLEditor
        {
            get
            {
                if (this.LEFN_UseHTMLEditorValueSpecified)
                {
                    return this.LEFN_UseHTMLEditorValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LEFN_UseHTMLEditorValue = value.GetValueOrDefault();
                this.LEFN_UseHTMLEditorValueSpecified = value.HasValue;
            }
        }
    }
