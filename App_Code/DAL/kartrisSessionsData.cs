
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("SessionsData", Namespace="http://tempuri.org/SessionsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("SessionsData", Namespace="http://tempuri.org/SessionsData.xsd")]
    public partial class SessionsData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<SessionsDataSessions> _sessions;
        
        [System.Xml.Serialization.XmlElementAttribute("Sessions", Namespace="http://tempuri.org/SessionsData.xsd")]
        public System.Collections.ObjectModel.Collection<SessionsDataSessions> Sessions
        {
            get
            {
                return this._sessions;
            }
            private set
            {
                this._sessions = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SessionsSpecified
        {
            get
            {
                return (this.Sessions.Count != 0);
            }
        }
        
        public SessionsData()
        {
            this._sessions = new System.Collections.ObjectModel.Collection<SessionsDataSessions>();
            this._sessionValues = new System.Collections.ObjectModel.Collection<SessionsDataSessionValues>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<SessionsDataSessionValues> _sessionValues;
        
        [System.Xml.Serialization.XmlElementAttribute("SessionValues", Namespace="http://tempuri.org/SessionsData.xsd")]
        public System.Collections.ObjectModel.Collection<SessionsDataSessionValues> SessionValues
        {
            get
            {
                return this._sessionValues;
            }
            private set
            {
                this._sessionValues = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SessionValuesSpecified
        {
            get
            {
                return (this.SessionValues.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("SessionsDataSessions", Namespace="http://tempuri.org/SessionsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SessionsDataSessions
    {
        
        [System.Xml.Serialization.XmlElementAttribute("SESS_ID", Namespace="http://tempuri.org/SessionsData.xsd")]
        public int SESS_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("SESS_Code", Namespace="http://tempuri.org/SessionsData.xsd")]
        public string SESS_Code { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(16)]
        [System.Xml.Serialization.XmlElementAttribute("SESS_IP", Namespace="http://tempuri.org/SessionsData.xsd")]
        public string SESS_IP { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SESS_DateCreated", Namespace="http://tempuri.org/SessionsData.xsd", DataType="dateTime")]
        public System.DateTime SESS_DateCreatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SESS_DateCreatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> SESS_DateCreated
        {
            get
            {
                if (this.SESS_DateCreatedValueSpecified)
                {
                    return this.SESS_DateCreatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SESS_DateCreatedValue = value.GetValueOrDefault();
                this.SESS_DateCreatedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SESS_DateLastUpdated", Namespace="http://tempuri.org/SessionsData.xsd", DataType="dateTime")]
        public System.DateTime SESS_DateLastUpdatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SESS_DateLastUpdatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> SESS_DateLastUpdated
        {
            get
            {
                if (this.SESS_DateLastUpdatedValueSpecified)
                {
                    return this.SESS_DateLastUpdatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SESS_DateLastUpdatedValue = value.GetValueOrDefault();
                this.SESS_DateLastUpdatedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SESS_Expiry", Namespace="http://tempuri.org/SessionsData.xsd")]
        public int SESS_ExpiryValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SESS_ExpiryValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> SESS_Expiry
        {
            get
            {
                if (this.SESS_ExpiryValueSpecified)
                {
                    return this.SESS_ExpiryValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SESS_ExpiryValue = value.GetValueOrDefault();
                this.SESS_ExpiryValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("SessionsDataSessionValues", Namespace="http://tempuri.org/SessionsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SessionsDataSessionValues
    {
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("SESSV_Name", Namespace="http://tempuri.org/SessionsData.xsd")]
        public string SESSV_Name { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SESSV_ID", Namespace="http://tempuri.org/SessionsData.xsd")]
        public int SESSV_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SESSV_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> SESSV_ID
        {
            get
            {
                if (this.SESSV_IDValueSpecified)
                {
                    return this.SESSV_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SESSV_IDValue = value.GetValueOrDefault();
                this.SESSV_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SESSV_SessionID", Namespace="http://tempuri.org/SessionsData.xsd")]
        public int SESSV_SessionIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SESSV_SessionIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> SESSV_SessionID
        {
            get
            {
                if (this.SESSV_SessionIDValueSpecified)
                {
                    return this.SESSV_SessionIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SESSV_SessionIDValue = value.GetValueOrDefault();
                this.SESSV_SessionIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("SESSV_Value", Namespace="http://tempuri.org/SessionsData.xsd")]
        public string SESSV_Value { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SESSV_Expiry", Namespace="http://tempuri.org/SessionsData.xsd")]
        public int SESSV_ExpiryValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SESSV_ExpiryValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> SESSV_Expiry
        {
            get
            {
                if (this.SESSV_ExpiryValueSpecified)
                {
                    return this.SESSV_ExpiryValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SESSV_ExpiryValue = value.GetValueOrDefault();
                this.SESSV_ExpiryValueSpecified = value.HasValue;
            }
        }
    }

 