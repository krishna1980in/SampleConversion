

    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisUserData", Namespace="http://tempuri.org/kartrisUserData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisUserData", Namespace="http://tempuri.org/kartrisUserData.xsd")]
    public partial class KartrisUserData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisUserDataUserDetails> _userDetails;
        
        [System.Xml.Serialization.XmlElementAttribute("UserDetails", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisUserDataUserDetails> UserDetails
        {
            get
            {
                return this._userDetails;
            }
            private set
            {
                this._userDetails = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool UserDetailsSpecified
        {
            get
            {
                return (this.UserDetails.Count != 0);
            }
        }
        
        public KartrisUserData()
        {
            this._userDetails = new System.Collections.ObjectModel.Collection<KartrisUserDataUserDetails>();
            this._customerGroups = new System.Collections.ObjectModel.Collection<KartrisUserDataCustomerGroups>();
            this._suppliers = new System.Collections.ObjectModel.Collection<KartrisUserDataSuppliers>();
            this._usersTicketsDetails = new System.Collections.ObjectModel.Collection<KartrisUserDataUsersTicketsDetails>();
            this._customerDetails = new System.Collections.ObjectModel.Collection<KartrisUserDataCustomerDetails>();
            this._addresses = new System.Collections.ObjectModel.Collection<KartrisUserDataAddresses>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisUserDataCustomerGroups> _customerGroups;
        
        [System.Xml.Serialization.XmlElementAttribute("CustomerGroups", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisUserDataCustomerGroups> CustomerGroups
        {
            get
            {
                return this._customerGroups;
            }
            private set
            {
                this._customerGroups = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CustomerGroupsSpecified
        {
            get
            {
                return (this.CustomerGroups.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisUserDataSuppliers> _suppliers;
        
        [System.Xml.Serialization.XmlElementAttribute("Suppliers", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisUserDataSuppliers> Suppliers
        {
            get
            {
                return this._suppliers;
            }
            private set
            {
                this._suppliers = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SuppliersSpecified
        {
            get
            {
                return (this.Suppliers.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisUserDataUsersTicketsDetails> _usersTicketsDetails;
        
        [System.Xml.Serialization.XmlElementAttribute("UsersTicketsDetails", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisUserDataUsersTicketsDetails> UsersTicketsDetails
        {
            get
            {
                return this._usersTicketsDetails;
            }
            private set
            {
                this._usersTicketsDetails = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool UsersTicketsDetailsSpecified
        {
            get
            {
                return (this.UsersTicketsDetails.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisUserDataCustomerDetails> _customerDetails;
        
        [System.Xml.Serialization.XmlElementAttribute("CustomerDetails", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisUserDataCustomerDetails> CustomerDetails
        {
            get
            {
                return this._customerDetails;
            }
            private set
            {
                this._customerDetails = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CustomerDetailsSpecified
        {
            get
            {
                return (this.CustomerDetails.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisUserDataAddresses> _addresses;
        
        [System.Xml.Serialization.XmlElementAttribute("Addresses", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisUserDataAddresses> Addresses
        {
            get
            {
                return this._addresses;
            }
            private set
            {
                this._addresses = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AddressesSpecified
        {
            get
            {
                return (this.Addresses.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisUserDataUserDetails", Namespace="http://tempuri.org/kartrisUserData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisUserDataUserDetails
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_CustomerDiscount", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public float U_CustomerDiscountValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_CustomerDiscountValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> U_CustomerDiscount
        {
            get
            {
                if (this.U_CustomerDiscountValueSpecified)
                {
                    return this.U_CustomerDiscountValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_CustomerDiscountValue = value.GetValueOrDefault();
                this.U_CustomerDiscountValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_DefBillingAddressID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int U_DefBillingAddressIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_DefBillingAddressIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> U_DefBillingAddressID
        {
            get
            {
                if (this.U_DefBillingAddressIDValueSpecified)
                {
                    return this.U_DefBillingAddressIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_DefBillingAddressIDValue = value.GetValueOrDefault();
                this.U_DefBillingAddressIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_DefShippingAddressID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int U_DefShippingAddressIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_DefShippingAddressIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> U_DefShippingAddressID
        {
            get
            {
                if (this.U_DefShippingAddressIDValueSpecified)
                {
                    return this.U_DefShippingAddressIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_DefShippingAddressIDValue = value.GetValueOrDefault();
                this.U_DefShippingAddressIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_AffiliateID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int U_AffiliateIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_AffiliateIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> U_AffiliateID
        {
            get
            {
                if (this.U_AffiliateIDValueSpecified)
                {
                    return this.U_AffiliateIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_AffiliateIDValue = value.GetValueOrDefault();
                this.U_AffiliateIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_Approved", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public bool U_ApprovedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_ApprovedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> U_Approved
        {
            get
            {
                if (this.U_ApprovedValueSpecified)
                {
                    return this.U_ApprovedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_ApprovedValue = value.GetValueOrDefault();
                this.U_ApprovedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_AffiliateCommission", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public float U_AffiliateCommissionValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_AffiliateCommissionValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> U_AffiliateCommission
        {
            get
            {
                if (this.U_AffiliateCommissionValueSpecified)
                {
                    return this.U_AffiliateCommissionValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_AffiliateCommissionValue = value.GetValueOrDefault();
                this.U_AffiliateCommissionValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_LanguageID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int U_LanguageIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_LanguageIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> U_LanguageID
        {
            get
            {
                if (this.U_LanguageIDValueSpecified)
                {
                    return this.U_LanguageIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_LanguageIDValue = value.GetValueOrDefault();
                this.U_LanguageIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_CustomerGroupID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int U_CustomerGroupIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_CustomerGroupIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> U_CustomerGroupID
        {
            get
            {
                if (this.U_CustomerGroupIDValueSpecified)
                {
                    return this.U_CustomerGroupIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_CustomerGroupIDValue = value.GetValueOrDefault();
                this.U_CustomerGroupIDValueSpecified = value.HasValue;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute("U_ID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int U_ID { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_IsAffiliate", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public bool U_IsAffiliateValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_IsAffiliateValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> U_IsAffiliate
        {
            get
            {
                if (this.U_IsAffiliateValueSpecified)
                {
                    return this.U_IsAffiliateValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_IsAffiliateValue = value.GetValueOrDefault();
                this.U_IsAffiliateValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(64)]
        [System.Xml.Serialization.XmlElementAttribute("U_TempPassword", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_TempPassword { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_TempPasswordExpiry", Namespace="http://tempuri.org/kartrisUserData.xsd", DataType="dateTime")]
        public System.DateTime U_TempPasswordExpiryValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_TempPasswordExpiryValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> U_TempPasswordExpiry
        {
            get
            {
                if (this.U_TempPasswordExpiryValueSpecified)
                {
                    return this.U_TempPasswordExpiryValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_TempPasswordExpiryValue = value.GetValueOrDefault();
                this.U_TempPasswordExpiryValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("U_AccountHolderName", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_AccountHolderName { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_SupportEndDate", Namespace="http://tempuri.org/kartrisUserData.xsd", DataType="dateTime")]
        public System.DateTime U_SupportEndDateValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_SupportEndDateValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> U_SupportEndDate
        {
            get
            {
                if (this.U_SupportEndDateValueSpecified)
                {
                    return this.U_SupportEndDateValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_SupportEndDateValue = value.GetValueOrDefault();
                this.U_SupportEndDateValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_CustomerBalance", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public decimal U_CustomerBalanceValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_CustomerBalanceValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> U_CustomerBalance
        {
            get
            {
                if (this.U_CustomerBalanceValueSpecified)
                {
                    return this.U_CustomerBalanceValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_CustomerBalanceValue = value.GetValueOrDefault();
                this.U_CustomerBalanceValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisUserDataCustomerGroups", Namespace="http://tempuri.org/kartrisUserData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisUserDataCustomerGroups
    {
        
        [System.Xml.Serialization.XmlElementAttribute("CG_ID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public short CG_ID { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LANG_ID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public byte LANG_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("CG_Name", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string CG_Name { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CG_Discount", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public float CG_DiscountValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CG_DiscountValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> CG_Discount
        {
            get
            {
                if (this.CG_DiscountValueSpecified)
                {
                    return this.CG_DiscountValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CG_DiscountValue = value.GetValueOrDefault();
                this.CG_DiscountValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("CG_Live", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public bool CG_LiveValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CG_LiveValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> CG_Live
        {
            get
            {
                if (this.CG_LiveValueSpecified)
                {
                    return this.CG_LiveValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.CG_LiveValue = value.GetValueOrDefault();
                this.CG_LiveValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisUserDataSuppliers", Namespace="http://tempuri.org/kartrisUserData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisUserDataSuppliers
    {
        
        [System.Xml.Serialization.XmlElementAttribute("SUP_ID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public short SUP_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("SUP_Name", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string SUP_Name { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SUP_Live", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public bool SUP_LiveValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SUP_LiveValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> SUP_Live
        {
            get
            {
                if (this.SUP_LiveValueSpecified)
                {
                    return this.SUP_LiveValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SUP_LiveValue = value.GetValueOrDefault();
                this.SUP_LiveValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisUserDataUsersTicketsDetails", Namespace="http://tempuri.org/kartrisUserData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisUserDataUsersTicketsDetails
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("UserID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int UserIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool UserIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> UserID
        {
            get
            {
                if (this.UserIDValueSpecified)
                {
                    return this.UserIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.UserIDValue = value.GetValueOrDefault();
                this.UserIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("U_EmailAddress", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_EmailAddress { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("UserTickets", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int UserTicketsValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool UserTicketsValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> UserTickets
        {
            get
            {
                if (this.UserTicketsValueSpecified)
                {
                    return this.UserTicketsValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.UserTicketsValue = value.GetValueOrDefault();
                this.UserTicketsValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("UserMessages", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int UserMessagesValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool UserMessagesValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> UserMessages
        {
            get
            {
                if (this.UserMessagesValueSpecified)
                {
                    return this.UserMessagesValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.UserMessagesValue = value.GetValueOrDefault();
                this.UserMessagesValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("TotalTickets", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public long TotalTicketsValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool TotalTicketsValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> TotalTickets
        {
            get
            {
                if (this.TotalTicketsValueSpecified)
                {
                    return this.TotalTicketsValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.TotalTicketsValue = value.GetValueOrDefault();
                this.TotalTicketsValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("TotalMessages", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public long TotalMessagesValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool TotalMessagesValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> TotalMessages
        {
            get
            {
                if (this.TotalMessagesValueSpecified)
                {
                    return this.TotalMessagesValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.TotalMessagesValue = value.GetValueOrDefault();
                this.TotalMessagesValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("TotalTime", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int TotalTimeValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool TotalTimeValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> TotalTime
        {
            get
            {
                if (this.TotalTimeValueSpecified)
                {
                    return this.TotalTimeValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.TotalTimeValue = value.GetValueOrDefault();
                this.TotalTimeValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisUserDataCustomerDetails", Namespace="http://tempuri.org/kartrisUserData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisUserDataCustomerDetails
    {
        
        [System.Xml.Serialization.XmlElementAttribute("U_ID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int U_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("U_EmailAddress", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_EmailAddress { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(64)]
        [System.Xml.Serialization.XmlElementAttribute("U_Telephone", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_Telephone { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("U_Password", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_Password { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_CustomerDiscount", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public float U_CustomerDiscountValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_CustomerDiscountValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> U_CustomerDiscount
        {
            get
            {
                if (this.U_CustomerDiscountValueSpecified)
                {
                    return this.U_CustomerDiscountValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_CustomerDiscountValue = value.GetValueOrDefault();
                this.U_CustomerDiscountValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_DefBillingAddressID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int U_DefBillingAddressIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_DefBillingAddressIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> U_DefBillingAddressID
        {
            get
            {
                if (this.U_DefBillingAddressIDValueSpecified)
                {
                    return this.U_DefBillingAddressIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_DefBillingAddressIDValue = value.GetValueOrDefault();
                this.U_DefBillingAddressIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_DefShippingAddressID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int U_DefShippingAddressIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_DefShippingAddressIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> U_DefShippingAddressID
        {
            get
            {
                if (this.U_DefShippingAddressIDValueSpecified)
                {
                    return this.U_DefShippingAddressIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_DefShippingAddressIDValue = value.GetValueOrDefault();
                this.U_DefShippingAddressIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(15)]
        [System.Xml.Serialization.XmlElementAttribute("U_CardholderEUVATNum", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_CardholderEUVATNum { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("U_Card_Number", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_Card_Number { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
        [System.Xml.Serialization.XmlElementAttribute("U_Card_Type", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_Card_Type { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
        [System.Xml.Serialization.XmlElementAttribute("U_Card_StartDate", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_Card_StartDate { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
        [System.Xml.Serialization.XmlElementAttribute("U_Card_Expiry", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_Card_Expiry { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
        [System.Xml.Serialization.XmlElementAttribute("U_Card_IssueNumber", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_Card_IssueNumber { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(5)]
        [System.Xml.Serialization.XmlElementAttribute("U_Card_SecurityNumber", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_Card_SecurityNumber { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_AffiliateID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int U_AffiliateIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_AffiliateIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> U_AffiliateID
        {
            get
            {
                if (this.U_AffiliateIDValueSpecified)
                {
                    return this.U_AffiliateIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_AffiliateIDValue = value.GetValueOrDefault();
                this.U_AffiliateIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_Approved", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public bool U_ApprovedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_ApprovedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> U_Approved
        {
            get
            {
                if (this.U_ApprovedValueSpecified)
                {
                    return this.U_ApprovedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_ApprovedValue = value.GetValueOrDefault();
                this.U_ApprovedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_CustomerGroupiD", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int U_CustomerGroupiDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_CustomerGroupiDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> U_CustomerGroupiD
        {
            get
            {
                if (this.U_CustomerGroupiDValueSpecified)
                {
                    return this.U_CustomerGroupiDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_CustomerGroupiDValue = value.GetValueOrDefault();
                this.U_CustomerGroupiDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_AffiliateCommission", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public float U_AffiliateCommissionValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_AffiliateCommissionValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> U_AffiliateCommission
        {
            get
            {
                if (this.U_AffiliateCommissionValueSpecified)
                {
                    return this.U_AffiliateCommissionValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_AffiliateCommissionValue = value.GetValueOrDefault();
                this.U_AffiliateCommissionValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_LanguageID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int U_LanguageIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_LanguageIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> U_LanguageID
        {
            get
            {
                if (this.U_LanguageIDValueSpecified)
                {
                    return this.U_LanguageIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_LanguageIDValue = value.GetValueOrDefault();
                this.U_LanguageIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_ML_SignupDateTime", Namespace="http://tempuri.org/kartrisUserData.xsd", DataType="dateTime")]
        public System.DateTime U_ML_SignupDateTimeValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_ML_SignupDateTimeValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> U_ML_SignupDateTime
        {
            get
            {
                if (this.U_ML_SignupDateTimeValueSpecified)
                {
                    return this.U_ML_SignupDateTimeValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_ML_SignupDateTimeValue = value.GetValueOrDefault();
                this.U_ML_SignupDateTimeValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("U_ML_SignupIP", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_ML_SignupIP { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_ML_ConfirmationDateTime", Namespace="http://tempuri.org/kartrisUserData.xsd", DataType="dateTime")]
        public System.DateTime U_ML_ConfirmationDateTimeValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_ML_ConfirmationDateTimeValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> U_ML_ConfirmationDateTime
        {
            get
            {
                if (this.U_ML_ConfirmationDateTimeValueSpecified)
                {
                    return this.U_ML_ConfirmationDateTimeValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_ML_ConfirmationDateTimeValue = value.GetValueOrDefault();
                this.U_ML_ConfirmationDateTimeValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("U_ML_ConfirmationIP", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_ML_ConfirmationIP { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("U_ML_RandomKey", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_ML_RandomKey { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("U_ML_Format", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_ML_Format { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_ML_SendMail", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public bool U_ML_SendMailValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_ML_SendMailValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> U_ML_SendMail
        {
            get
            {
                if (this.U_ML_SendMailValueSpecified)
                {
                    return this.U_ML_SendMailValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_ML_SendMailValue = value.GetValueOrDefault();
                this.U_ML_SendMailValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_IsAffiliate", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public bool U_IsAffiliateValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_IsAffiliateValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> U_IsAffiliate
        {
            get
            {
                if (this.U_IsAffiliateValueSpecified)
                {
                    return this.U_IsAffiliateValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_IsAffiliateValue = value.GetValueOrDefault();
                this.U_IsAffiliateValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(64)]
        [System.Xml.Serialization.XmlElementAttribute("U_TempPassword", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_TempPassword { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_TempPasswordExpiry", Namespace="http://tempuri.org/kartrisUserData.xsd", DataType="dateTime")]
        public System.DateTime U_TempPasswordExpiryValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_TempPasswordExpiryValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> U_TempPasswordExpiry
        {
            get
            {
                if (this.U_TempPasswordExpiryValueSpecified)
                {
                    return this.U_TempPasswordExpiryValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_TempPasswordExpiryValue = value.GetValueOrDefault();
                this.U_TempPasswordExpiryValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("U_AccountHolderName", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_AccountHolderName { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("U_QBListID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_QBListID { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_SupportEndDate", Namespace="http://tempuri.org/kartrisUserData.xsd", DataType="dateTime")]
        public System.DateTime U_SupportEndDateValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_SupportEndDateValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> U_SupportEndDate
        {
            get
            {
                if (this.U_SupportEndDateValueSpecified)
                {
                    return this.U_SupportEndDateValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_SupportEndDateValue = value.GetValueOrDefault();
                this.U_SupportEndDateValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("U_Notes", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_Notes { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_CustomerBalance", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public decimal U_CustomerBalanceValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_CustomerBalanceValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> U_CustomerBalance
        {
            get
            {
                if (this.U_CustomerBalanceValueSpecified)
                {
                    return this.U_CustomerBalanceValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_CustomerBalanceValue = value.GetValueOrDefault();
                this.U_CustomerBalanceValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(64)]
        [System.Xml.Serialization.XmlElementAttribute("U_SaltValue", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_SaltValue { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_GDPR_OptIn", Namespace="http://tempuri.org/kartrisUserData.xsd", DataType="dateTime")]
        public System.DateTime U_GDPR_OptInValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_GDPR_OptInValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> U_GDPR_OptIn
        {
            get
            {
                if (this.U_GDPR_OptInValueSpecified)
                {
                    return this.U_GDPR_OptInValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.U_GDPR_OptInValue = value.GetValueOrDefault();
                this.U_GDPR_OptInValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("U_GDPR_SignupIP", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string U_GDPR_SignupIP { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("U_GDPR_IsGuest", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public bool U_GDPR_IsGuest { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("ADR_Name", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string ADR_Name { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisUserDataAddresses", Namespace="http://tempuri.org/kartrisUserData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisUserDataAddresses
    {
        
        [System.Xml.Serialization.XmlElementAttribute("ADR_ID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int ADR_ID { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_UserID", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int ADR_UserIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool ADR_UserIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> ADR_UserID
        {
            get
            {
                if (this.ADR_UserIDValueSpecified)
                {
                    return this.ADR_UserIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.ADR_UserIDValue = value.GetValueOrDefault();
                this.ADR_UserIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_Label", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string ADR_Label { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_Name", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string ADR_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_Company", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string ADR_Company { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_StreetAddress", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string ADR_StreetAddress { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_TownCity", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string ADR_TownCity { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_County", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string ADR_County { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_PostCode", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string ADR_PostCode { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_Country", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public int ADR_CountryValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool ADR_CountryValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> ADR_Country
        {
            get
            {
                if (this.ADR_CountryValueSpecified)
                {
                    return this.ADR_CountryValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.ADR_CountryValue = value.GetValueOrDefault();
                this.ADR_CountryValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_Telephone", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string ADR_Telephone { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_Type", Namespace="http://tempuri.org/kartrisUserData.xsd")]
        public string ADR_Type { get; set; }
    }

 