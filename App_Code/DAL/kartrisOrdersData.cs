
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisOrdersData", Namespace="http://tempuri.org/kartrisOrdersData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisOrdersData", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
    public partial class KartrisOrdersData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisOrdersDataOrders> _orders;
        
        [System.Xml.Serialization.XmlElementAttribute("Orders", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisOrdersDataOrders> Orders
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
        
        public KartrisOrdersData()
        {
            this._orders = new System.Collections.ObjectModel.Collection<KartrisOrdersDataOrders>();
            this._invoiceRows = new System.Collections.ObjectModel.Collection<KartrisOrdersDataInvoiceRows>();
            this._cardTypes = new System.Collections.ObjectModel.Collection<KartrisOrdersDataCardTypes>();
            this._payments = new System.Collections.ObjectModel.Collection<KartrisOrdersDataPayments>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisOrdersDataInvoiceRows> _invoiceRows;
        
        [System.Xml.Serialization.XmlElementAttribute("InvoiceRows", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisOrdersDataInvoiceRows> InvoiceRows
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
        private System.Collections.ObjectModel.Collection<KartrisOrdersDataCardTypes> _cardTypes;
        
        [System.Xml.Serialization.XmlElementAttribute("CardTypes", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisOrdersDataCardTypes> CardTypes
        {
            get
            {
                return this._cardTypes;
            }
            private set
            {
                this._cardTypes = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CardTypesSpecified
        {
            get
            {
                return (this.CardTypes.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisOrdersDataPayments> _payments;
        
        [System.Xml.Serialization.XmlElementAttribute("Payments", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisOrdersDataPayments> Payments
        {
            get
            {
                return this._payments;
            }
            private set
            {
                this._payments = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PaymentsSpecified
        {
            get
            {
                return (this.Payments.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisOrdersDataOrders", Namespace="http://tempuri.org/kartrisOrdersData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisOrdersDataOrders
    {
        
        [System.Xml.Serialization.XmlElementAttribute("O_ID", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public int O_ID { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_CustomerID", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_Details", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string O_Details { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_ShippingPrice", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_ShippingTax", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_DiscountPercentage", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_AffiliatePercentage", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_TotalPrice", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_Date", Namespace="http://tempuri.org/kartrisOrdersData.xsd", DataType="dateTime")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_PurchaseOrderNo", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string O_PurchaseOrderNo { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_SecurityID", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_Sent", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_Invoiced", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_Shipped", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_Paid", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_Status", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string O_Status { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_LastModified", Namespace="http://tempuri.org/kartrisOrdersData.xsd", DataType="dateTime")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_WishListID", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_CouponCode", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string O_CouponCode { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_CouponDiscountTotal", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_PricesIncTax", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_TaxDue", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_PaymentGateWay", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string O_PaymentGateWay { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("O_ReferenceCode", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string O_ReferenceCode { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_LanguageID", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_CurrencyID", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_TotalPriceGateway", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_CurrencyIDGateway", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_AffiliatePaymentID", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_AffiliateTotalPrice", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_SendOrderUpdateEmail", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_OrderHandlingCharge", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_OrderHandlingChargeTax", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_CurrencyRate", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_BillingAddress", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string O_BillingAddress { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("O_ShippingAddress", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string O_ShippingAddress { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_PromotionDiscountTotal", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_PromotionDescription", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string O_PromotionDescription { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_SentToQB", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("O_Notes", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string O_Notes { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(255)]
        [System.Xml.Serialization.XmlElementAttribute("O_ShippingMethod", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string O_ShippingMethod { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("O_Data", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string O_Data { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("O_Comments", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string O_Comments { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("O_Cancelled", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
    [System.Xml.Serialization.XmlTypeAttribute("KartrisOrdersDataInvoiceRows", Namespace="http://tempuri.org/kartrisOrdersData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisOrdersDataInvoiceRows
    {
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("IR_VersionCode", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string IR_VersionCode { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1000)]
        [System.Xml.Serialization.XmlElementAttribute("IR_VersionName", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string IR_VersionName { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("IR_Quantity", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public float IR_QuantityValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool IR_QuantityValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<float> IR_Quantity
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
        [System.Xml.Serialization.XmlElementAttribute("IR_PricePerItem", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("IR_TaxPerItem", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
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
        [System.Xml.Serialization.XmlElementAttribute("IR_OptionsText", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string IR_OptionsText { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_ID", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public long V_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> V_ID
        {
            get
            {
                if (this.V_IDValueSpecified)
                {
                    return this.V_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_IDValue = value.GetValueOrDefault();
                this.V_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_Live", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public bool V_LiveValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_LiveValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<bool> V_Live
        {
            get
            {
                if (this.V_LiveValueSpecified)
                {
                    return this.V_LiveValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_LiveValue = value.GetValueOrDefault();
                this.V_LiveValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("V_Quantity", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public short V_QuantityValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool V_QuantityValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> V_Quantity
        {
            get
            {
                if (this.V_QuantityValueSpecified)
                {
                    return this.V_QuantityValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.V_QuantityValue = value.GetValueOrDefault();
                this.V_QuantityValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("V_Type", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string V_Type { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisOrdersDataCardTypes", Namespace="http://tempuri.org/kartrisOrdersData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisOrdersDataCardTypes
    {
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("CR_NAME", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string CR_NAME { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisOrdersDataPayments", Namespace="http://tempuri.org/kartrisOrdersData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisOrdersDataPayments
    {
        
        [System.Xml.Serialization.XmlElementAttribute("Payment_ID", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public int Payment_ID { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("Payment_CustomerID", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public int Payment_CustomerIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool Payment_CustomerIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> Payment_CustomerID
        {
            get
            {
                if (this.Payment_CustomerIDValueSpecified)
                {
                    return this.Payment_CustomerIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.Payment_CustomerIDValue = value.GetValueOrDefault();
                this.Payment_CustomerIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("Payment_Date", Namespace="http://tempuri.org/kartrisOrdersData.xsd", DataType="dateTime")]
        public System.DateTime Payment_DateValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool Payment_DateValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> Payment_Date
        {
            get
            {
                if (this.Payment_DateValueSpecified)
                {
                    return this.Payment_DateValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.Payment_DateValue = value.GetValueOrDefault();
                this.Payment_DateValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("Payment_Amount", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public decimal Payment_AmountValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool Payment_AmountValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> Payment_Amount
        {
            get
            {
                if (this.Payment_AmountValueSpecified)
                {
                    return this.Payment_AmountValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.Payment_AmountValue = value.GetValueOrDefault();
                this.Payment_AmountValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("Payment_CurrencyID", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public byte Payment_CurrencyIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool Payment_CurrencyIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> Payment_CurrencyID
        {
            get
            {
                if (this.Payment_CurrencyIDValueSpecified)
                {
                    return this.Payment_CurrencyIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.Payment_CurrencyIDValue = value.GetValueOrDefault();
                this.Payment_CurrencyIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("Payment_ReferenceNo", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string Payment_ReferenceNo { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("Payment_Gateway", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public string Payment_Gateway { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("Payment_CurrencyRate", Namespace="http://tempuri.org/kartrisOrdersData.xsd")]
        public decimal Payment_CurrencyRateValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool Payment_CurrencyRateValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<decimal> Payment_CurrencyRate
        {
            get
            {
                if (this.Payment_CurrencyRateValueSpecified)
                {
                    return this.Payment_CurrencyRateValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.Payment_CurrencyRateValue = value.GetValueOrDefault();
                this.Payment_CurrencyRateValueSpecified = value.HasValue;
            }
        }
    }

 