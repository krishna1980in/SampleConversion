

    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisLoginData", Namespace="http://tempuri.org/kartrisLoginData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisLoginData", Namespace="http://tempuri.org/kartrisLoginData.xsd")]
    public partial class KartrisLoginData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisLoginDataLogins> _logins;
        
        [System.Xml.Serialization.XmlElementAttribute("Logins", Namespace="http://tempuri.org/kartrisLoginData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisLoginDataLogins> Logins
        {
            get
            {
                return this._logins;
            }
            private set
            {
                this._logins = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LoginsSpecified
        {
            get
            {
                return (this.Logins.Count != 0);
            }
        }
        
        public KartrisLoginData()
        {
            this._logins = new System.Collections.ObjectModel.Collection<KartrisLoginDataLogins>();
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisLoginDataLogins", Namespace="http://tempuri.org/kartrisLoginData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisLoginDataLogins
    {
        
        [System.Xml.Serialization.XmlElementAttribute("LOGIN_ID", Namespace="http://tempuri.org/kartrisLoginData.xsd")]
        public short LOGIN_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("LOGIN_Username", Namespace="http://tempuri.org/kartrisLoginData.xsd")]
        public string LOGIN_Username { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("LOGIN_Password", Namespace="http://tempuri.org/kartrisLoginData.xsd")]
        public string LOGIN_Password { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LOGIN_Live", Namespace="http://tempuri.org/kartrisLoginData.xsd")]
        public bool LOGIN_LiveValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LOGIN_LiveValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> LOGIN_Live
        {
            get
            {
                if (this.LOGIN_LiveValueSpecified)
                {
                    return this.LOGIN_LiveValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LOGIN_LiveValue = value.GetValueOrDefault();
                this.LOGIN_LiveValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LOGIN_Orders", Namespace="http://tempuri.org/kartrisLoginData.xsd")]
        public bool LOGIN_OrdersValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LOGIN_OrdersValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> LOGIN_Orders
        {
            get
            {
                if (this.LOGIN_OrdersValueSpecified)
                {
                    return this.LOGIN_OrdersValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LOGIN_OrdersValue = value.GetValueOrDefault();
                this.LOGIN_OrdersValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LOGIN_Products", Namespace="http://tempuri.org/kartrisLoginData.xsd")]
        public bool LOGIN_ProductsValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LOGIN_ProductsValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> LOGIN_Products
        {
            get
            {
                if (this.LOGIN_ProductsValueSpecified)
                {
                    return this.LOGIN_ProductsValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LOGIN_ProductsValue = value.GetValueOrDefault();
                this.LOGIN_ProductsValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LOGIN_Config", Namespace="http://tempuri.org/kartrisLoginData.xsd")]
        public bool LOGIN_ConfigValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LOGIN_ConfigValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> LOGIN_Config
        {
            get
            {
                if (this.LOGIN_ConfigValueSpecified)
                {
                    return this.LOGIN_ConfigValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LOGIN_ConfigValue = value.GetValueOrDefault();
                this.LOGIN_ConfigValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LOGIN_Protected", Namespace="http://tempuri.org/kartrisLoginData.xsd")]
        public bool LOGIN_ProtectedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LOGIN_ProtectedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> LOGIN_Protected
        {
            get
            {
                if (this.LOGIN_ProtectedValueSpecified)
                {
                    return this.LOGIN_ProtectedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LOGIN_ProtectedValue = value.GetValueOrDefault();
                this.LOGIN_ProtectedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LOGIN_LanguageID", Namespace="http://tempuri.org/kartrisLoginData.xsd")]
        public byte LOGIN_LanguageIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LOGIN_LanguageIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> LOGIN_LanguageID
        {
            get
            {
                if (this.LOGIN_LanguageIDValueSpecified)
                {
                    return this.LOGIN_LanguageIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LOGIN_LanguageIDValue = value.GetValueOrDefault();
                this.LOGIN_LanguageIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("LOGIN_EmailAddress", Namespace="http://tempuri.org/kartrisLoginData.xsd")]
        public string LOGIN_EmailAddress { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("LOGIN_Tickets", Namespace="http://tempuri.org/kartrisLoginData.xsd")]
        public bool LOGIN_TicketsValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool LOGIN_TicketsValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> LOGIN_Tickets
        {
            get
            {
                if (this.LOGIN_TicketsValueSpecified)
                {
                    return this.LOGIN_TicketsValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.LOGIN_TicketsValue = value.GetValueOrDefault();
                this.LOGIN_TicketsValueSpecified = value.HasValue;
            }
        }
    }

 