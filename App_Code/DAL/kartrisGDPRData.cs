
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisGDPRData", Namespace="http://tempuri.org/kartrisGDPRData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisGDPRData", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
    public partial class KartrisGDPRData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisGDPRDataAddresses> _addresses;
        
        [System.Xml.Serialization.XmlElementAttribute("Addresses", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisGDPRDataAddresses> Addresses
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
        
        public KartrisGDPRData()
        {
            this._addresses = new System.Collections.ObjectModel.Collection<KartrisGDPRDataAddresses>();
            this._orders = new System.Collections.ObjectModel.Collection<KartrisGDPRDataOrders>();
            this._invoiceRows = new System.Collections.ObjectModel.Collection<KartrisGDPRDataInvoiceRows>();
            this._reviews = new System.Collections.ObjectModel.Collection<KartrisGDPRDataReviews>();
            this._user = new System.Collections.ObjectModel.Collection<KartrisGDPRDataUser>();
            this._wishLists = new System.Collections.ObjectModel.Collection<KartrisGDPRDataWishLists>();
            this._supportTickets = new System.Collections.ObjectModel.Collection<KartrisGDPRDataSupportTickets>();
            this._supportTicketMessages = new System.Collections.ObjectModel.Collection<KartrisGDPRDataSupportTicketMessages>();
            this._savedBaskets = new System.Collections.ObjectModel.Collection<KartrisGDPRDataSavedBaskets>();
            this._savedBasketValues = new System.Collections.ObjectModel.Collection<KartrisGDPRDataSavedBasketValues>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisGDPRDataOrders> _orders;
        
        [System.Xml.Serialization.XmlElementAttribute("Orders", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisGDPRDataOrders> Orders
        {
            get
            {
                return this._orders;
            }
            private set
            {
                this._orders = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool OrdersSpecified
        {
            get
            {
                return (this.Orders.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisGDPRDataInvoiceRows> _invoiceRows;
        
        [System.Xml.Serialization.XmlElementAttribute("InvoiceRows", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisGDPRDataInvoiceRows> InvoiceRows
        {
            get
            {
                return this._invoiceRows;
            }
            private set
            {
                this._invoiceRows = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool InvoiceRowsSpecified
        {
            get
            {
                return (this.InvoiceRows.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisGDPRDataReviews> _reviews;
        
        [System.Xml.Serialization.XmlElementAttribute("Reviews", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisGDPRDataReviews> Reviews
        {
            get
            {
                return this._reviews;
            }
            private set
            {
                this._reviews = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ReviewsSpecified
        {
            get
            {
                return (this.Reviews.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisGDPRDataUser> _user;
        
        [System.Xml.Serialization.XmlElementAttribute("User", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisGDPRDataUser> User
        {
            get
            {
                return this._user;
            }
            private set
            {
                this._user = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool UserSpecified
        {
            get
            {
                return (this.User.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisGDPRDataWishLists> _wishLists;
        
        [System.Xml.Serialization.XmlElementAttribute("WishLists", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisGDPRDataWishLists> WishLists
        {
            get
            {
                return this._wishLists;
            }
            private set
            {
                this._wishLists = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool WishListsSpecified
        {
            get
            {
                return (this.WishLists.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisGDPRDataSupportTickets> _supportTickets;
        
        [System.Xml.Serialization.XmlElementAttribute("SupportTickets", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisGDPRDataSupportTickets> SupportTickets
        {
            get
            {
                return this._supportTickets;
            }
            private set
            {
                this._supportTickets = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SupportTicketsSpecified
        {
            get
            {
                return (this.SupportTickets.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisGDPRDataSupportTicketMessages> _supportTicketMessages;
        
        [System.Xml.Serialization.XmlElementAttribute("SupportTicketMessages", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisGDPRDataSupportTicketMessages> SupportTicketMessages
        {
            get
            {
                return this._supportTicketMessages;
            }
            private set
            {
                this._supportTicketMessages = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SupportTicketMessagesSpecified
        {
            get
            {
                return (this.SupportTicketMessages.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisGDPRDataSavedBaskets> _savedBaskets;
        
        [System.Xml.Serialization.XmlElementAttribute("SavedBaskets", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisGDPRDataSavedBaskets> SavedBaskets
        {
            get
            {
                return this._savedBaskets;
            }
            private set
            {
                this._savedBaskets = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SavedBasketsSpecified
        {
            get
            {
                return (this.SavedBaskets.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisGDPRDataSavedBasketValues> _savedBasketValues;
        
        [System.Xml.Serialization.XmlElementAttribute("SavedBasketValues", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisGDPRDataSavedBasketValues> SavedBasketValues
        {
            get
            {
                return this._savedBasketValues;
            }
            private set
            {
                this._savedBasketValues = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SavedBasketValuesSpecified
        {
            get
            {
                return (this.SavedBasketValues.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisGDPRDataAddresses", Namespace="http://tempuri.org/kartrisGDPRData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisGDPRDataAddresses
    {
        
        [System.Xml.Serialization.XmlElementAttribute("ADR_ID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public int ADR_ID { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_UserID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("ADR_Label", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string ADR_Label { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_Name", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string ADR_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_Company", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string ADR_Company { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_StreetAddress", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string ADR_StreetAddress { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_TownCity", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string ADR_TownCity { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_County", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string ADR_County { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_PostCode", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string ADR_PostCode { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("ADR_Telephone", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string ADR_Telephone { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("LE_Value", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string LE_Value { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LE_TypeID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public byte LE_TypeID { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("LE_LanguageID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public byte LE_LanguageID { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisGDPRDataOrders", Namespace="http://tempuri.org/kartrisGDPRData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisGDPRDataOrders
    {
        
        [System.Xml.Serialization.XmlElementAttribute("O_ID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public int O_ID { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_CustomerID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public int O_CustomerIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_CustomerIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> O_CustomerID
        {
            get
            {
                if (this.O_CustomerIDValueSpecified)
                {
                    return this.O_CustomerIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_CustomerIDValue = value.GetValueOrDefault();
                this.O_CustomerIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("O_Details", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string O_Details { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_ShippingPrice", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public decimal O_ShippingPriceValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_ShippingPriceValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> O_ShippingPrice
        {
            get
            {
                if (this.O_ShippingPriceValueSpecified)
                {
                    return this.O_ShippingPriceValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_ShippingPriceValue = value.GetValueOrDefault();
                this.O_ShippingPriceValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_ShippingTax", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public decimal O_ShippingTaxValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_ShippingTaxValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> O_ShippingTax
        {
            get
            {
                if (this.O_ShippingTaxValueSpecified)
                {
                    return this.O_ShippingTaxValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_ShippingTaxValue = value.GetValueOrDefault();
                this.O_ShippingTaxValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_DiscountPercentage", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public decimal O_DiscountPercentageValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_DiscountPercentageValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> O_DiscountPercentage
        {
            get
            {
                if (this.O_DiscountPercentageValueSpecified)
                {
                    return this.O_DiscountPercentageValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_DiscountPercentageValue = value.GetValueOrDefault();
                this.O_DiscountPercentageValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_AffiliatePercentage", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public decimal O_AffiliatePercentageValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_AffiliatePercentageValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> O_AffiliatePercentage
        {
            get
            {
                if (this.O_AffiliatePercentageValueSpecified)
                {
                    return this.O_AffiliatePercentageValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_AffiliatePercentageValue = value.GetValueOrDefault();
                this.O_AffiliatePercentageValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_TotalPrice", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public decimal O_TotalPriceValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_TotalPriceValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> O_TotalPrice
        {
            get
            {
                if (this.O_TotalPriceValueSpecified)
                {
                    return this.O_TotalPriceValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_TotalPriceValue = value.GetValueOrDefault();
                this.O_TotalPriceValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_Date", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
        public System.DateTime O_DateValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_DateValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> O_Date
        {
            get
            {
                if (this.O_DateValueSpecified)
                {
                    return this.O_DateValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_DateValue = value.GetValueOrDefault();
                this.O_DateValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("O_PurchaseOrderNo", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string O_PurchaseOrderNo { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_SecurityID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public int O_SecurityIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_SecurityIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> O_SecurityID
        {
            get
            {
                if (this.O_SecurityIDValueSpecified)
                {
                    return this.O_SecurityIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_SecurityIDValue = value.GetValueOrDefault();
                this.O_SecurityIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_Sent", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public bool O_SentValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_SentValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> O_Sent
        {
            get
            {
                if (this.O_SentValueSpecified)
                {
                    return this.O_SentValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_SentValue = value.GetValueOrDefault();
                this.O_SentValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_Invoiced", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public bool O_InvoicedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_InvoicedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> O_Invoiced
        {
            get
            {
                if (this.O_InvoicedValueSpecified)
                {
                    return this.O_InvoicedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_InvoicedValue = value.GetValueOrDefault();
                this.O_InvoicedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_Shipped", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public bool O_ShippedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_ShippedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> O_Shipped
        {
            get
            {
                if (this.O_ShippedValueSpecified)
                {
                    return this.O_ShippedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_ShippedValue = value.GetValueOrDefault();
                this.O_ShippedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_Paid", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public bool O_PaidValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_PaidValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> O_Paid
        {
            get
            {
                if (this.O_PaidValueSpecified)
                {
                    return this.O_PaidValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_PaidValue = value.GetValueOrDefault();
                this.O_PaidValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("O_Status", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string O_Status { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_LastModified", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
        public System.DateTime O_LastModifiedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_LastModifiedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> O_LastModified
        {
            get
            {
                if (this.O_LastModifiedValueSpecified)
                {
                    return this.O_LastModifiedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_LastModifiedValue = value.GetValueOrDefault();
                this.O_LastModifiedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_WishListID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public int O_WishListIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_WishListIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> O_WishListID
        {
            get
            {
                if (this.O_WishListIDValueSpecified)
                {
                    return this.O_WishListIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_WishListIDValue = value.GetValueOrDefault();
                this.O_WishListIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(25)]
        [System.Xml.Serialization.XmlElementAttribute("O_CouponCode", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string O_CouponCode { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_CouponDiscountTotal", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public decimal O_CouponDiscountTotalValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_CouponDiscountTotalValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> O_CouponDiscountTotal
        {
            get
            {
                if (this.O_CouponDiscountTotalValueSpecified)
                {
                    return this.O_CouponDiscountTotalValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_CouponDiscountTotalValue = value.GetValueOrDefault();
                this.O_CouponDiscountTotalValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_PricesIncTax", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public bool O_PricesIncTaxValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_PricesIncTaxValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> O_PricesIncTax
        {
            get
            {
                if (this.O_PricesIncTaxValueSpecified)
                {
                    return this.O_PricesIncTaxValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_PricesIncTaxValue = value.GetValueOrDefault();
                this.O_PricesIncTaxValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_TaxDue", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public decimal O_TaxDueValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_TaxDueValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> O_TaxDue
        {
            get
            {
                if (this.O_TaxDueValueSpecified)
                {
                    return this.O_TaxDueValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_TaxDueValue = value.GetValueOrDefault();
                this.O_TaxDueValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("O_PaymentGateWay", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string O_PaymentGateWay { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("O_ReferenceCode", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string O_ReferenceCode { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_LanguageID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public byte O_LanguageIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_LanguageIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> O_LanguageID
        {
            get
            {
                if (this.O_LanguageIDValueSpecified)
                {
                    return this.O_LanguageIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_LanguageIDValue = value.GetValueOrDefault();
                this.O_LanguageIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_CurrencyID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public byte O_CurrencyIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_CurrencyIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> O_CurrencyID
        {
            get
            {
                if (this.O_CurrencyIDValueSpecified)
                {
                    return this.O_CurrencyIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_CurrencyIDValue = value.GetValueOrDefault();
                this.O_CurrencyIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_TotalPriceGateway", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public decimal O_TotalPriceGatewayValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_TotalPriceGatewayValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> O_TotalPriceGateway
        {
            get
            {
                if (this.O_TotalPriceGatewayValueSpecified)
                {
                    return this.O_TotalPriceGatewayValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_TotalPriceGatewayValue = value.GetValueOrDefault();
                this.O_TotalPriceGatewayValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_CurrencyIDGateway", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public byte O_CurrencyIDGatewayValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_CurrencyIDGatewayValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> O_CurrencyIDGateway
        {
            get
            {
                if (this.O_CurrencyIDGatewayValueSpecified)
                {
                    return this.O_CurrencyIDGatewayValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_CurrencyIDGatewayValue = value.GetValueOrDefault();
                this.O_CurrencyIDGatewayValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_AffiliatePaymentID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public int O_AffiliatePaymentIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_AffiliatePaymentIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> O_AffiliatePaymentID
        {
            get
            {
                if (this.O_AffiliatePaymentIDValueSpecified)
                {
                    return this.O_AffiliatePaymentIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_AffiliatePaymentIDValue = value.GetValueOrDefault();
                this.O_AffiliatePaymentIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_AffiliateTotalPrice", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public decimal O_AffiliateTotalPriceValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_AffiliateTotalPriceValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> O_AffiliateTotalPrice
        {
            get
            {
                if (this.O_AffiliateTotalPriceValueSpecified)
                {
                    return this.O_AffiliateTotalPriceValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_AffiliateTotalPriceValue = value.GetValueOrDefault();
                this.O_AffiliateTotalPriceValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_SendOrderUpdateEmail", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public bool O_SendOrderUpdateEmailValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_SendOrderUpdateEmailValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> O_SendOrderUpdateEmail
        {
            get
            {
                if (this.O_SendOrderUpdateEmailValueSpecified)
                {
                    return this.O_SendOrderUpdateEmailValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_SendOrderUpdateEmailValue = value.GetValueOrDefault();
                this.O_SendOrderUpdateEmailValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_OrderHandlingCharge", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public decimal O_OrderHandlingChargeValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_OrderHandlingChargeValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> O_OrderHandlingCharge
        {
            get
            {
                if (this.O_OrderHandlingChargeValueSpecified)
                {
                    return this.O_OrderHandlingChargeValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_OrderHandlingChargeValue = value.GetValueOrDefault();
                this.O_OrderHandlingChargeValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_OrderHandlingChargeTax", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public decimal O_OrderHandlingChargeTaxValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_OrderHandlingChargeTaxValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> O_OrderHandlingChargeTax
        {
            get
            {
                if (this.O_OrderHandlingChargeTaxValueSpecified)
                {
                    return this.O_OrderHandlingChargeTaxValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_OrderHandlingChargeTaxValue = value.GetValueOrDefault();
                this.O_OrderHandlingChargeTaxValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_CurrencyRate", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public decimal O_CurrencyRateValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_CurrencyRateValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> O_CurrencyRate
        {
            get
            {
                if (this.O_CurrencyRateValueSpecified)
                {
                    return this.O_CurrencyRateValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_CurrencyRateValue = value.GetValueOrDefault();
                this.O_CurrencyRateValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("O_ShippingMethod", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string O_ShippingMethod { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("O_BillingAddress", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string O_BillingAddress { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("O_ShippingAddress", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string O_ShippingAddress { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_PromotionDiscountTotal", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public decimal O_PromotionDiscountTotalValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_PromotionDiscountTotalValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> O_PromotionDiscountTotal
        {
            get
            {
                if (this.O_PromotionDiscountTotalValueSpecified)
                {
                    return this.O_PromotionDiscountTotalValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_PromotionDiscountTotalValue = value.GetValueOrDefault();
                this.O_PromotionDiscountTotalValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("O_PromotionDescription", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string O_PromotionDescription { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_SentToQB", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public bool O_SentToQBValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_SentToQBValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> O_SentToQB
        {
            get
            {
                if (this.O_SentToQBValueSpecified)
                {
                    return this.O_SentToQBValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_SentToQBValue = value.GetValueOrDefault();
                this.O_SentToQBValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("O_Notes", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string O_Notes { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("O_Data", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string O_Data { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("O_Comments", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string O_Comments { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_Cancelled", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public bool O_CancelledValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool O_CancelledValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> O_Cancelled
        {
            get
            {
                if (this.O_CancelledValueSpecified)
                {
                    return this.O_CancelledValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.O_CancelledValue = value.GetValueOrDefault();
                this.O_CancelledValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisGDPRDataInvoiceRows", Namespace="http://tempuri.org/kartrisGDPRData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisGDPRDataInvoiceRows
    {
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("IR_VersionCode", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string IR_VersionCode { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("IR_VersionName", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string IR_VersionName { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("IR_Quantity", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public double IR_QuantityValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool IR_QuantityValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<double> IR_Quantity
        {
            get
            {
                if (this.IR_QuantityValueSpecified)
                {
                    return this.IR_QuantityValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.IR_QuantityValue = value.GetValueOrDefault();
                this.IR_QuantityValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("IR_PricePerItem", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public decimal IR_PricePerItemValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool IR_PricePerItemValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> IR_PricePerItem
        {
            get
            {
                if (this.IR_PricePerItemValueSpecified)
                {
                    return this.IR_PricePerItemValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.IR_PricePerItemValue = value.GetValueOrDefault();
                this.IR_PricePerItemValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("IR_TaxPerItem", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public decimal IR_TaxPerItemValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool IR_TaxPerItemValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> IR_TaxPerItem
        {
            get
            {
                if (this.IR_TaxPerItemValueSpecified)
                {
                    return this.IR_TaxPerItemValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.IR_TaxPerItemValue = value.GetValueOrDefault();
                this.IR_TaxPerItemValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4000)]
        [System.Xml.Serialization.XmlElementAttribute("IR_OptionsText", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string IR_OptionsText { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("IR_ExcludeFromCustomerDiscount", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public bool IR_ExcludeFromCustomerDiscount { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisGDPRDataReviews", Namespace="http://tempuri.org/kartrisGDPRData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisGDPRDataReviews
    {
        
        [System.Xml.Serialization.XmlElementAttribute("REV_ID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public short REV_ID { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("REV_ProductID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public int REV_ProductIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool REV_ProductIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> REV_ProductID
        {
            get
            {
                if (this.REV_ProductIDValueSpecified)
                {
                    return this.REV_ProductIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.REV_ProductIDValue = value.GetValueOrDefault();
                this.REV_ProductIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("REV_LanguageID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public byte REV_LanguageIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool REV_LanguageIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> REV_LanguageID
        {
            get
            {
                if (this.REV_LanguageIDValueSpecified)
                {
                    return this.REV_LanguageIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.REV_LanguageIDValue = value.GetValueOrDefault();
                this.REV_LanguageIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("REV_CustomerID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public int REV_CustomerIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool REV_CustomerIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> REV_CustomerID
        {
            get
            {
                if (this.REV_CustomerIDValueSpecified)
                {
                    return this.REV_CustomerIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.REV_CustomerIDValue = value.GetValueOrDefault();
                this.REV_CustomerIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(60)]
        [System.Xml.Serialization.XmlElementAttribute("REV_Title", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string REV_Title { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4000)]
        [System.Xml.Serialization.XmlElementAttribute("REV_Text", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string REV_Text { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("REV_Rating", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public byte REV_RatingValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool REV_RatingValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> REV_Rating
        {
            get
            {
                if (this.REV_RatingValueSpecified)
                {
                    return this.REV_RatingValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.REV_RatingValue = value.GetValueOrDefault();
                this.REV_RatingValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
        [System.Xml.Serialization.XmlElementAttribute("REV_Name", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string REV_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(75)]
        [System.Xml.Serialization.XmlElementAttribute("REV_Email", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string REV_Email { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
        [System.Xml.Serialization.XmlElementAttribute("REV_Location", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string REV_Location { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("REV_DateCreated", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
        public System.DateTime REV_DateCreatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool REV_DateCreatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> REV_DateCreated
        {
            get
            {
                if (this.REV_DateCreatedValueSpecified)
                {
                    return this.REV_DateCreatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.REV_DateCreatedValue = value.GetValueOrDefault();
                this.REV_DateCreatedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("REV_DateLastUpdated", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
        public System.DateTime REV_DateLastUpdatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool REV_DateLastUpdatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> REV_DateLastUpdated
        {
            get
            {
                if (this.REV_DateLastUpdatedValueSpecified)
                {
                    return this.REV_DateLastUpdatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.REV_DateLastUpdatedValue = value.GetValueOrDefault();
                this.REV_DateLastUpdatedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("REV_Live", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string REV_Live { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisGDPRDataUser", Namespace="http://tempuri.org/kartrisGDPRData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisGDPRDataUser
    {
        
        [System.Xml.Serialization.XmlElementAttribute("U_ID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public int U_ID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("U_EmailAddress", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_EmailAddress { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("U_Telephone", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_Telephone { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(64)]
        [System.Xml.Serialization.XmlElementAttribute("U_Password", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_Password { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(64)]
        [System.Xml.Serialization.XmlElementAttribute("U_TempPassword", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_TempPassword { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_TempPasswordExpiry", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
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
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_CustomerDiscount", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("U_DefBillingAddressID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("U_DefShippingAddressID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
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
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("U_AccountHolderName", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_AccountHolderName { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(15)]
        [System.Xml.Serialization.XmlElementAttribute("U_CardholderEUVATNum", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_CardholderEUVATNum { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("U_Card_Number", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_Card_Number { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
        [System.Xml.Serialization.XmlElementAttribute("U_Card_Type", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_Card_Type { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
        [System.Xml.Serialization.XmlElementAttribute("U_Card_StartDate", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_Card_StartDate { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
        [System.Xml.Serialization.XmlElementAttribute("U_Card_Expiry", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_Card_Expiry { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
        [System.Xml.Serialization.XmlElementAttribute("U_Card_IssueNumber", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_Card_IssueNumber { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(5)]
        [System.Xml.Serialization.XmlElementAttribute("U_Card_SecurityNumber", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_Card_SecurityNumber { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_AffiliateID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("U_Approved", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("U_CustomerGroupiD", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("U_IsAffiliate", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
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
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_AffiliateCommission", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("U_LanguageID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("U_ML_SignupDateTime", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
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
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("U_ML_SignupIP", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_ML_SignupIP { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_ML_ConfirmationDateTime", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
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
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("U_ML_ConfirmationIP", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_ML_ConfirmationIP { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("U_ML_RandomKey", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_ML_RandomKey { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("U_ML_Format", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_ML_Format { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_ML_SendMail", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
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
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("U_QBListID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_QBListID { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_SupportEndDate", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
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
        [System.Xml.Serialization.XmlElementAttribute("U_Notes", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_Notes { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_CustomerBalance", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public float U_CustomerBalanceValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool U_CustomerBalanceValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> U_CustomerBalance
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
        [System.Xml.Serialization.XmlElementAttribute("U_SaltValue", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string U_SaltValue { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("U_GDPR_OptIn", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
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
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisGDPRDataWishLists", Namespace="http://tempuri.org/kartrisGDPRData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisGDPRDataWishLists
    {
        
        [System.Xml.Serialization.XmlElementAttribute("WL_ID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public int WL_ID { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("WL_UserID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public int WL_UserIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool WL_UserIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> WL_UserID
        {
            get
            {
                if (this.WL_UserIDValueSpecified)
                {
                    return this.WL_UserIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.WL_UserIDValue = value.GetValueOrDefault();
                this.WL_UserIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("WL_Name", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string WL_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
        [System.Xml.Serialization.XmlElementAttribute("WL_PublicPassword", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string WL_PublicPassword { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("WL_Message", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string WL_Message { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("WL_DateTimeAdded", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
        public System.DateTime WL_DateTimeAddedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool WL_DateTimeAddedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> WL_DateTimeAdded
        {
            get
            {
                if (this.WL_DateTimeAddedValueSpecified)
                {
                    return this.WL_DateTimeAddedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.WL_DateTimeAddedValue = value.GetValueOrDefault();
                this.WL_DateTimeAddedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("WL_LastUpdated", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
        public System.DateTime WL_LastUpdatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool WL_LastUpdatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> WL_LastUpdated
        {
            get
            {
                if (this.WL_LastUpdatedValueSpecified)
                {
                    return this.WL_LastUpdatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.WL_LastUpdatedValue = value.GetValueOrDefault();
                this.WL_LastUpdatedValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisGDPRDataSupportTickets", Namespace="http://tempuri.org/kartrisGDPRData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisGDPRDataSupportTickets
    {
        
        [System.Xml.Serialization.XmlElementAttribute("TIC_ID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public long TIC_ID { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("TIC_DateOpened", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
        public System.DateTime TIC_DateOpened { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("TIC_DateClosed", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
        public System.DateTime TIC_DateClosedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool TIC_DateClosedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> TIC_DateClosed
        {
            get
            {
                if (this.TIC_DateClosedValueSpecified)
                {
                    return this.TIC_DateClosedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.TIC_DateClosedValue = value.GetValueOrDefault();
                this.TIC_DateClosedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("TIC_Subject", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string TIC_Subject { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("TIC_UserID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public int TIC_UserID { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("TIC_LoginID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public short TIC_LoginIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool TIC_LoginIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> TIC_LoginID
        {
            get
            {
                if (this.TIC_LoginIDValueSpecified)
                {
                    return this.TIC_LoginIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.TIC_LoginIDValue = value.GetValueOrDefault();
                this.TIC_LoginIDValueSpecified = value.HasValue;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute("TIC_SupportTicketTypeID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public short TIC_SupportTicketTypeID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("TIC_Status", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string TIC_Status { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("TIC_TimeSpent", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public int TIC_TimeSpent { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(200)]
        [System.Xml.Serialization.XmlElementAttribute("TIC_Tags", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string TIC_Tags { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisGDPRDataSupportTicketMessages", Namespace="http://tempuri.org/kartrisGDPRData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisGDPRDataSupportTicketMessages
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("STM_TicketID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public long STM_TicketIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool STM_TicketIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> STM_TicketID
        {
            get
            {
                if (this.STM_TicketIDValueSpecified)
                {
                    return this.STM_TicketIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.STM_TicketIDValue = value.GetValueOrDefault();
                this.STM_TicketIDValueSpecified = value.HasValue;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute("STM_ID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public long STM_ID { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("AssignedID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public short AssignedIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool AssignedIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> AssignedID
        {
            get
            {
                if (this.AssignedIDValueSpecified)
                {
                    return this.AssignedIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.AssignedIDValue = value.GetValueOrDefault();
                this.AssignedIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("AssignedName", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string AssignedName { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("STM_DateCreated", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
        public System.DateTime STM_DateCreatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool STM_DateCreatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> STM_DateCreated
        {
            get
            {
                if (this.STM_DateCreatedValueSpecified)
                {
                    return this.STM_DateCreatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.STM_DateCreatedValue = value.GetValueOrDefault();
                this.STM_DateCreatedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("STM_Text", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string STM_Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisGDPRDataSavedBaskets", Namespace="http://tempuri.org/kartrisGDPRData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisGDPRDataSavedBaskets
    {
        
        [System.Xml.Serialization.XmlElementAttribute("SBSKT_ID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public int SBSKT_ID { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("SBSKT_UserID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public int SBSKT_UserID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("SBSKT_Name", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string SBSKT_Name { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SBSKT_DateTimeAdded", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
        public System.DateTime SBSKT_DateTimeAddedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SBSKT_DateTimeAddedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> SBSKT_DateTimeAdded
        {
            get
            {
                if (this.SBSKT_DateTimeAddedValueSpecified)
                {
                    return this.SBSKT_DateTimeAddedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SBSKT_DateTimeAddedValue = value.GetValueOrDefault();
                this.SBSKT_DateTimeAddedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SBSKT_LastUpdated", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
        public System.DateTime SBSKT_LastUpdatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SBSKT_LastUpdatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> SBSKT_LastUpdated
        {
            get
            {
                if (this.SBSKT_LastUpdatedValueSpecified)
                {
                    return this.SBSKT_LastUpdatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SBSKT_LastUpdatedValue = value.GetValueOrDefault();
                this.SBSKT_LastUpdatedValueSpecified = value.HasValue;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisGDPRDataSavedBasketValues", Namespace="http://tempuri.org/kartrisGDPRData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisGDPRDataSavedBasketValues
    {
        
        [System.Xml.Serialization.XmlElementAttribute("BV_VersionID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public long BV_VersionID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("V_Name", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string V_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(25)]
        [System.Xml.Serialization.XmlElementAttribute("V_CodeNumber", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string V_CodeNumber { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("BV_Quantity", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public double BV_QuantityValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool BV_QuantityValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<double> BV_Quantity
        {
            get
            {
                if (this.BV_QuantityValueSpecified)
                {
                    return this.BV_QuantityValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.BV_QuantityValue = value.GetValueOrDefault();
                this.BV_QuantityValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("BV_CustomText", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string BV_CustomText { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("BV_DateTimeAdded", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
        public System.DateTime BV_DateTimeAddedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool BV_DateTimeAddedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> BV_DateTimeAdded
        {
            get
            {
                if (this.BV_DateTimeAddedValueSpecified)
                {
                    return this.BV_DateTimeAddedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.BV_DateTimeAddedValue = value.GetValueOrDefault();
                this.BV_DateTimeAddedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("BV_LastUpdated", Namespace="http://tempuri.org/kartrisGDPRData.xsd", DataType="dateTime")]
        public System.DateTime BV_LastUpdatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool BV_LastUpdatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> BV_LastUpdated
        {
            get
            {
                if (this.BV_LastUpdatedValueSpecified)
                {
                    return this.BV_LastUpdatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.BV_LastUpdatedValue = value.GetValueOrDefault();
                this.BV_LastUpdatedValueSpecified = value.HasValue;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute("BV_ParentID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public long BV_ParentID { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("BV_ParentType", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public string BV_ParentType { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("BV_ID", Namespace="http://tempuri.org/kartrisGDPRData.xsd")]
        public long BV_ID { get; set; }
    }

 