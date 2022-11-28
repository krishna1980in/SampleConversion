
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisDBData", Namespace="http://tempuri.org/kartrisDBData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisDBData", Namespace="http://tempuri.org/kartrisDBData.xsd")]
    public partial class KartrisDBData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisDBDataCustomProductsToCompare> _customProductsToCompare;
        
        [System.Xml.Serialization.XmlElementAttribute("CustomProductsToCompare", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisDBDataCustomProductsToCompare> CustomProductsToCompare
        {
            get
            {
                return this._customProductsToCompare;
            }
            private set
            {
                this._customProductsToCompare = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CustomProductsToCompareSpecified
        {
            get
            {
                return (this.CustomProductsToCompare.Count != 0);
            }
        }
        
        public KartrisDBData()
        {
            this._customProductsToCompare = new System.Collections.ObjectModel.Collection<KartrisDBDataCustomProductsToCompare>();
            this._search = new System.Collections.ObjectModel.Collection<KartrisDBDataSearch>();
            this.@__spKartrisDB_GetTriggers = new System.Collections.ObjectModel.Collection<KartrisDBData_SpKartrisDB_GetTriggers>();
            this.@__spKartrisAdminLog_Search = new System.Collections.ObjectModel.Collection<KartrisDBData_SpKartrisAdminLog_Search>();
            this._adminRelatedTables = new System.Collections.ObjectModel.Collection<KartrisDBDataAdminRelatedTables>();
            this._deletedItems = new System.Collections.ObjectModel.Collection<KartrisDBDataDeletedItems>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisDBDataSearch> _search;
        
        [System.Xml.Serialization.XmlElementAttribute("Search", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisDBDataSearch> Search
        {
            get
            {
                return this._search;
            }
            private set
            {
                this._search = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SearchSpecified
        {
            get
            {
                return (this.Search.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisDBData_SpKartrisDB_GetTriggers> @__spKartrisDB_GetTriggers;
        
        [System.Xml.Serialization.XmlElementAttribute("_spKartrisDB_GetTriggers", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisDBData_SpKartrisDB_GetTriggers> _spKartrisDB_GetTriggers
        {
            get
            {
                return this.__spKartrisDB_GetTriggers;
            }
            private set
            {
                this.__spKartrisDB_GetTriggers = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool _spKartrisDB_GetTriggersSpecified
        {
            get
            {
                return (this._spKartrisDB_GetTriggers.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisDBData_SpKartrisAdminLog_Search> @__spKartrisAdminLog_Search;
        
        [System.Xml.Serialization.XmlElementAttribute("_spKartrisAdminLog_Search", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisDBData_SpKartrisAdminLog_Search> _spKartrisAdminLog_Search
        {
            get
            {
                return this.__spKartrisAdminLog_Search;
            }
            private set
            {
                this.__spKartrisAdminLog_Search = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool _spKartrisAdminLog_SearchSpecified
        {
            get
            {
                return (this._spKartrisAdminLog_Search.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisDBDataAdminRelatedTables> _adminRelatedTables;
        
        [System.Xml.Serialization.XmlElementAttribute("AdminRelatedTables", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisDBDataAdminRelatedTables> AdminRelatedTables
        {
            get
            {
                return this._adminRelatedTables;
            }
            private set
            {
                this._adminRelatedTables = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AdminRelatedTablesSpecified
        {
            get
            {
                return (this.AdminRelatedTables.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisDBDataDeletedItems> _deletedItems;
        
        [System.Xml.Serialization.XmlElementAttribute("DeletedItems", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisDBDataDeletedItems> DeletedItems
        {
            get
            {
                return this._deletedItems;
            }
            private set
            {
                this._deletedItems = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DeletedItemsSpecified
        {
            get
            {
                return (this.DeletedItems.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisDBDataCustomProductsToCompare", Namespace="http://tempuri.org/kartrisDBData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisDBDataCustomProductsToCompare
    {
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisDBDataSearch", Namespace="http://tempuri.org/kartrisDBData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisDBDataSearch
    {
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisDBData_spKartrisDB_GetTriggers", Namespace="http://tempuri.org/kartrisDBData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisDBData_SpKartrisDB_GetTriggers
    {
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(128)]
        [System.Xml.Serialization.XmlElementAttribute("TableName", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public string TableName { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(128)]
        [System.Xml.Serialization.XmlElementAttribute("TriggerName", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public string TriggerName { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("IsTriggerEnabled", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public int IsTriggerEnabledValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool IsTriggerEnabledValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> IsTriggerEnabled
        {
            get
            {
                if (this.IsTriggerEnabledValueSpecified)
                {
                    return this.IsTriggerEnabledValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.IsTriggerEnabledValue = value.GetValueOrDefault();
                this.IsTriggerEnabledValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisDBData_spKartrisAdminLog_Search", Namespace="http://tempuri.org/kartrisDBData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisDBData_SpKartrisAdminLog_Search
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("AL_ID", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public int AL_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool AL_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> AL_ID
        {
            get
            {
                if (this.AL_IDValueSpecified)
                {
                    return this.AL_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.AL_IDValue = value.GetValueOrDefault();
                this.AL_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("AL_DateStamp", Namespace="http://tempuri.org/kartrisDBData.xsd", DataType="dateTime")]
        public System.DateTime AL_DateStampValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool AL_DateStampValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> AL_DateStamp
        {
            get
            {
                if (this.AL_DateStampValueSpecified)
                {
                    return this.AL_DateStampValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.AL_DateStampValue = value.GetValueOrDefault();
                this.AL_DateStampValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("AL_LoginName", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public string AL_LoginName { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("AL_Type", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public string AL_Type { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("AL_Description", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public string AL_Description { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("AL_Query", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public string AL_Query { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("AL_RelatedID", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public string AL_RelatedID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("AL_IP", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public string AL_IP { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("ShortQuery", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public string ShortQuery { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisDBDataAdminRelatedTables", Namespace="http://tempuri.org/kartrisDBData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisDBDataAdminRelatedTables
    {
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisDBDataDeletedItems", Namespace="http://tempuri.org/kartrisDBData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisDBDataDeletedItems
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("Deleted_ID", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public long Deleted_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool Deleted_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> Deleted_ID
        {
            get
            {
                if (this.Deleted_IDValueSpecified)
                {
                    return this.Deleted_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.Deleted_IDValue = value.GetValueOrDefault();
                this.Deleted_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("Deleted_Type", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public string Deleted_Type { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("Deleted_VersionProduct", Namespace="http://tempuri.org/kartrisDBData.xsd")]
        public int Deleted_VersionProductValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool Deleted_VersionProductValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> Deleted_VersionProduct
        {
            get
            {
                if (this.Deleted_VersionProductValueSpecified)
                {
                    return this.Deleted_VersionProductValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.Deleted_VersionProductValue = value.GetValueOrDefault();
                this.Deleted_VersionProductValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("Deleted_DateTime", Namespace="http://tempuri.org/kartrisDBData.xsd", DataType="dateTime")]
        public System.DateTime Deleted_DateTimeValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool Deleted_DateTimeValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> Deleted_DateTime
        {
            get
            {
                if (this.Deleted_DateTimeValueSpecified)
                {
                    return this.Deleted_DateTimeValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.Deleted_DateTimeValue = value.GetValueOrDefault();
                this.Deleted_DateTimeValueSpecified = value.HasValue;
            }
        }
    }

 