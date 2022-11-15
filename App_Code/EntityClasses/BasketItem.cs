using System;

using KartSettingsManager;

namespace Kartris
{
    /// <summary>
    ///     ''' A single row in a basket
    ///     ''' </summary>
    ///     ''' <remarks>Can contain multiple quantity of an item</remarks>
    ///     '''
    [Serializable()]
    public class BasketItem
    {
        public long _VersionID;           // The version ID
        public long ComboVersionID;       // The ID of the combination version
        private float _Quantity;         // The quantity of the item within this basket row
        private double _StockQty;
        public int _AppliedPromo;     // The number of promotion already applied for this version
        private double _ExTax;            // The price of the item, before tax

        private double _TaxRate1;         // The tax rate applied to this item
        private double _TaxRate2;         // In scenarios where there is a two tier tax system (e.g. Canada) this would be the second tax rate.
        private double _ComputedTaxRate;  // A single tax rate which is calculated by taking all of the individual tax rates for the individual items and producing one single rate. This is because different items may have different tax rates. in this way we can apply tax to an entire basket in one operation which is useful with coupons etc. which are applied to the whole basket.

        public bool FreeShipping;      // Whether the item has free shipping
        public double Weight;             // Weight of a single item
        public double RRP;                // The RRP of the version

        public bool ApplyTax;          // Whether the tax is on or off
        public bool PricesIncTax;      // Whether prices are entered incl or ex of tax

        public bool HasCombinations;   // Whether the options have specific combinations
        public string VersionCode;
        public double Price;              // This is the display price, which might be inc or ex tax 

        public string TableText;
        private string _DownloadType;
        private double _QtyWarnLevel;
        private double _IncTax;           // Price for a single item (not row, which is items multiplied by quantity) including tax.

        private string _CodeNumber;       // Product SKU
        private string _VersionName;      // Version name taken from language table
        private string _ProductName;
        private string _ProductType;      // Product type as set in the product setup. Tells us if there is one version or if there are potentially options etc. At time of writting options are 'Single Version', 'Multiple Version', 'Options Product'
        private string _VersionType;      // How should the various product options be displayed (dropdown, list etc.)
        private long _ID;                 // Id of the basket item
        private long _ProductID;          // Id of the product from the product table
        private string _CategoryIDs;      // Comma delimited string of categor IDs
        private double _OptionPrice;      // Price of an opion
        private string _OptionText;       // Text associated with an option
        private string _OptionLink;
        private bool _Free;            // Item is free (without charge)
        private bool _ApplyTax;        // We should apply tax to this item
        private string _CustomText;       // Some products can have custom text assigned to them when they are added to the basket. e.g. A product that allows you to engrave a custom message
        private string _CustomType;
        private string _CustomDesc;
        private double _CustomCost;
        private double _TaxRateItem;
        private bool _AdjustedQty;
        private long _VersionBaseID;      // Where a product has multiple versions this is the base / genesis version that has all of the text, images etc. that may be missing from the child versions
        private long _PromoQty;

        private bool _ExcludeFromCustomerDiscount; // v2.9010 addition, allows items to be excluded from the % customer discount available to customer

        /// <summary>
        ///         ''' Some products can have custom text assigned to them when they are added to the basket.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>e.g. A product that allows you to engrave a custom message</remarks>
        public string CustomText
        {
            get
            {
                return _CustomText;
            }
            set
            {
                _CustomText = value;
            }
        }

        /// <summary>
        ///         ''' Product SKU
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public string CodeNumber
        {
            get
            {
                return _CodeNumber;
            }
            set
            {
                _CodeNumber = value;
            }
        }

        /// <summary>
        ///         ''' Exclude this item from customer discount
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public string ExcludeFromCustomerDiscount
        {
            get
            {
                return _ExcludeFromCustomerDiscount;
            }
            set
            {
                _ExcludeFromCustomerDiscount = value;
            }
        }

        /// <summary>
        ///         ''' Name of product version taken from language table
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public string VersionName
        {
            get
            {
                return _VersionName;
            }
            set
            {
                _VersionName = value;
            }
        }

        /// <summary>
        ///         '''  Name of the product
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public string ProductName
        {
            get
            {
                return _ProductName;
            }
            set
            {
                _ProductName = value;
            }
        }

        // Duplicate, used in basket for image control
        public string P_Name
        {
            get
            {
                return _ProductName;
            }
            set
            {
                _ProductName = value;
            }
        }

        /// <summary>
        ///         ''' Product type as set in the product setup. Tells us if there is one version or if there are potentially options etc.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>At time of writting options are 'Single Version', 'Multiple Version', 'Options Product'</remarks>
        public string ProductType
        {
            get
            {
                return _ProductType;
            }
            set
            {
                _ProductType = value;
            }
        }

        /// <summary>
        ///         ''' How should the various product options be displayed
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>options list = 'o', default = 'd', none = 'l', rows = 'r' etc.</remarks>
        public string VersionType
        {
            get
            {
                return _VersionType;
            }
            set
            {
                _VersionType = value;
            }
        }

        /// <summary>
        ///         ''' Basket item ID as it is in the Basket item table
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public long ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        /// <summary>
        ///         ''' Product ID from the product table
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public long ProductID
        {
            get
            {
                return _ProductID;
            }
            set
            {
                _ProductID = value;
            }
        }

        /// <summary>
        ///         ''' Duplicate, used in basket for image control
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public long P_ID
        {
            get
            {
                return _ProductID;
            }
            set
            {
                _ProductID = value;
            }
        }

        public string CategoryIDs
        {
            get
            {
                return _CategoryIDs;
            }
            set
            {
                _CategoryIDs = value;
            }
        }

        public string CustomType
        {
            get
            {
                return _CustomType;
            }
            set
            {
                _CustomType = value;
            }
        }

        public string CustomDesc
        {
            get
            {
                return _CustomDesc;
            }
            set
            {
                _CustomDesc = value;
            }
        }

        public double CustomCost
        {
            get
            {
                return Math.Round(_CustomCost, BasketBLL.CurrencyRoundNumber);
            }
            set
            {
                _CustomCost = value;
            }
        }

        public long PromoQty
        {
            get
            {
                return _PromoQty;
            }
            set
            {
                _PromoQty = value;
            }
        }

        /// <summary>
        ///         ''' Returns false
        ///         ''' </summary>
        ///         ''' <returns></returns>
        ///         ''' <remarks>Appears to be legacy, returns False</remarks>
        public bool HasOptions()
        {
            // The item has options if the first option group ID > 0
            return false;
        }

        /// <summary>
        ///         ''' The quantity of this product that appears in this basket row. 
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public float Quantity
        {
            get
            {
                return _Quantity;
            }
            set
            {
                _Quantity = value;
                if (_Quantity > 2147483647)
                    _Quantity = 2147483647;
            }
        }

        /// <summary>
        ///         ''' The quantity of this item that are currently in stock 
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>Used for validation control incase the user tries to purchase more stock than is available</remarks>
        public double StockQty
        {
            get
            {
                return _StockQty;
            }
            set
            {
                _StockQty = value;
            }
        }

        /// <summary>
        ///         ''' Indicates that the quantity was automatically adjusted by the system as a result of a validation step.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>If the customer attempts to order a higher quantity than is available and validation is set the quantity will be reduced and this flag will be set to TRUE</remarks>
        public bool AdjustedQty
        {
            get
            {
                return _AdjustedQty;
            }
            set
            {
                _AdjustedQty = value;
            }
        }

        /// <summary>
        ///         ''' Used as a boolean value to indicate if a promotion has been applied
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>At time of writting only values of 0 an 1 were possible here.</remarks>
        public double AppliedPromo
        {
            get
            {
                return _AppliedPromo;
            }
            set
            {
                _AppliedPromo = System.Convert.ToInt32(value);
                if (_AppliedPromo > 32767)
                    _AppliedPromo = 32767;
            }
        }

        /// <summary>
        ///         ''' Add a given amount to the current quantity
        ///         ''' </summary>
        ///         ''' <param name="value"></param>
        ///         ''' <remarks></remarks>
        public void IncreaseQuantity(double value)
        {
            Quantity = _Quantity + value;
        }

        /// <summary>
        ///         ''' Subtract a given amount from the current quantity
        ///         ''' </summary>
        ///         ''' <param name="value"></param>
        ///         ''' <remarks></remarks>
        public void DecreaseQuantity(double value)
        {
            Quantity = _Quantity - value;
        }

        /// <summary>
        ///         ''' No function.
        ///         ''' </summary>
        ///         ''' <returns></returns>
        ///         ''' <remarks>returns a blank string</remarks>
        public string GetLinkQS()
        {
            return "";
        }

        /// <summary>
        ///         ''' Used for downloadable items (not physical items that are stocked and shipped).
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public string DownloadType
        {
            get
            {
                return _DownloadType;
            }
            set
            {
                _DownloadType = value;
            }
        }

        /// <summary>
        ///         ''' At which quantity level should the shop owner be alerted about being low on stock.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double QtyWarnLevel
        {
            get
            {
                return _QtyWarnLevel;
            }
            set
            {
                _QtyWarnLevel = value;
            }
        }

        /// <summary>
        ///         ''' if it is an options product, the options can optionally have a value (plus or minus)
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double OptionPrice
        {
            get
            {
                return Math.Round(_OptionPrice, BasketBLL.CurrencyRoundNumber);
            }
            set
            {
                _OptionPrice = value;
            }
        }

        /// <summary>
        ///         ''' If this is an option product this will contain the text of that option.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public string OptionText
        {
            get
            {
                return _OptionText;
            }
            set
            {
                _OptionText = value;
            }
        }

        /// <summary>
        ///         ''' POSSIBLY NOT USED. Is read when creating a URL.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>Is read from in several places but never written to (except to set it as a ZLS)</remarks>
        public string OptionLink
        {
            get
            {
                return _OptionLink;
            }
            set
            {
                _OptionLink = value;
            }
        }

        /// <summary>
        ///         '''  Is product free (zero cost)?
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>some promotional offers will react differently to a product marked as free compared to a product of zero cost ($0.00).</remarks>
        public bool Free
        {
            get
            {
                return _Free;
            }
            set
            {
                _Free = value;
            }
        }

        /// <summary>
        ///         ''' Where a product has multiple versions this is the version ID. 
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public long VersionID
        {
            get
            {
                return _VersionID;
            }
            set
            {
                _VersionID = value;
            }
        }

        /// <summary>
        ///         ''' This is the ID of the base version of an options product
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public long VersionBaseID
        {
            get
            {
                return _VersionBaseID;
            }
            set
            {
                _VersionBaseID = value;
            }
        }

        /// <summary>
        ///         ''' Display name
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>Normally product name but it may contain a version name also</remarks>
        public string Name
        {
            get
            {
                if (ProductName == VersionName)
                    return ProductName;
                else
                    return ProductName + " - " + VersionName;
            }
        }

        /// <summary>
        ///         ''' [assumed] Price per item excluding tax - remember a row can have multiples of a single item.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double ExTax
        {
            get
            {
                return Math.Round(_ExTax, BasketBLL.CurrencyRoundNumber);
            }
            set
            {
                _ExTax = value;
            }
        }

        /// <summary>
        ///         ''' [assumed] Price per item including tax - remember a row can have multiples of a single item.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double IncTax
        {
            get
            {
                return Math.Round(_ExTax + TaxAmount, BasketBLL.CurrencyRoundNumber);
            }
            set
            {
                _IncTax = value;
            }
        }

        /// <summary>
        ///         ''' How much tax is applied to a single item within this basket row.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double TaxAmount
        {
            get
            {
                int numRounding = 0;
                if (ConfigurationManager.AppSettings("TaxRegime").ToLower != "us" & ConfigurationManager.AppSettings("TaxRegime").ToLower != "simple")
                    numRounding = 4;
                else
                    numRounding = 6;
                return Math.Round(Interaction.IIf(!(ApplyTax), 0, _ExTax * ComputedTaxRate), numRounding);
            }
        }

        /// <summary>
        ///         ''' The tax rate for the entire basket combining the various different tax rates that the individual items each have into one average tax rate.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>If you have some items in an order that have VAT, and some without, the total VAT % on the order won't be 20% as it would if all items have VAT. The computed tax rate is the total % of VAT on the order. We use this to correctly handle coupons, so they are split between the taxable and non taxable parts of an order in the right proportion. For example, let's say we have prices ex tax, and two items in the basket that are 10 GBP. One is taxable, the other is not. The </remarks>
        public double ComputedTaxRate
        {
            get
            {
                return Math.Round(_ComputedTaxRate, 6);
            }
            set
            {
                _ComputedTaxRate = value;
            }
        }

        /// <summary>
        ///         ''' This is the primary tax rate for the item, normally in the EU countries this is the VAT %
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double TaxRate1
        {
            get
            {
                return Math.Round(_TaxRate1, 6);
            }
            set
            {
                _TaxRate1 = value;
            }
        }

        /// <summary>
        ///         ''' Used in countries with a two tier tax system
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>Canada has a two tier tax regime, with both national and provincial tax rates often separate (though some provinces merge them into a single 'HST' - harmonized sales tax). In EU countries this value is therefore blank, but will be used in Canada.</remarks>
        public double TaxRate2
        {
            get
            {
                return Math.Round(_TaxRate2, 6);
            }
            set
            {
                _TaxRate2 = value;
            }
        }

        /// <summary>
        ///         ''' What is the rate of tax for a single item within the basket row.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double TaxRateItem
        {
            get
            {
                return Math.Round(_TaxRateItem, 6);
            }
            set
            {
                _TaxRateItem = value;
            }
        }

        /// <summary>
        ///         ''' Price excluding tax without any rounding applied.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>used for calculations but not generally for display</remarks>
        public double ExTaxNoRound
        {
            get
            {
                return _ExTax;
            }
        }

        /// <summary>
        ///         ''' Prince including tax without any rounding applied
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>used for calculations but not generally for display</remarks>
        public double IncTaxNoRound
        {
            get
            {
                return _ExTax * (Interaction.IIf(!(ApplyTax), 0, ComputedTaxRate) + 1);
            }
        }

        /// <summary>
        ///         ''' Price of the basket item row excluding tax
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>remember that a row can include multiples of a single product</remarks>
        public double RowExTax
        {
            get
            {
                int numRounding = 0;
                if (ConfigurationManager.AppSettings("TaxRegime").ToLower != "us" & ConfigurationManager.AppSettings("TaxRegime").ToLower != "simple")
                    numRounding = BasketBLL.CurrencyRoundNumber;
                else
                    numRounding = 4;
                return Math.Round(Interaction.IIf(PricesIncTax, ExTax * Quantity, ExTaxNoRound * Quantity), numRounding);
            }
        }

        /// <summary>
        ///         ''' Price of a row including the tax. 
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>remember that a row can include multiples of a single product</remarks>
        public double RowIncTax
        {
            get
            {
                return Math.Round(Interaction.IIf(PricesIncTax, IncTax * Quantity, IncTaxNoRound * Quantity), BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' Amount of tax for the row in the basket
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>remember that a row can include multiples of a single product</remarks>
        public double RowTaxAmount
        {
            get
            {
                return Math.Round(Interaction.IIf(!(ApplyTax), 0, Interaction.IIf(PricesIncTax, RowIncTax - RowExTax, IncTaxNoRound * Quantity - RowExTax)), BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' Weight of the basket row
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>remember that a row can include multiples of a single product</remarks>
        public double RowWeight
        {
            get
            {
                return Weight * Quantity;
            }
        }

        /// <summary>
        ///         ''' Amount of money saved compared to RRP
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>Calcualates tax within function automatically.</remarks>
        public double RowAmountSaved
        {
            get
            {
                return Math.Round(Interaction.IIf(PricesIncTax, Math.Max(Quantity * (RRP - IncTax), 0), Math.Max(Quantity * (RRP - ExTax), 0)), BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' The amount of tax for each item within a single Invoice Row. Used when more than one of a single item is purchased
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>price of each item. Depending on the tax mode you're using (inc/ex) 
        ///         ''' this field will hold either the extax or inctax price per item (unit price). </remarks>
        public double IR_PricePerItem
        {
            get
            {
                return Math.Round(Interaction.IIf(PricesIncTax, IncTax, ExTax), BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' The amount of tax for each item within a single Invoice Row. Used when more than one of a single item is purchased
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>IR= invoice rows, a row can have multiple items on it if you purchase several of the same item, 
        ///         ''' so the tax per row and tax per item can be different. Having values for each is important because where 
        ///         ''' the rounding happens depends on whether the store uses inc or ex tax pricing. 
        ///         ''' If prices ex tax, the IR_TaxPerItem field holds a % tax, if prices inc tax, it holds the tax amount in currency 
        ///         ''' (since rounding occurs per item, we can calculate and round this, 
        ///         ''' unlike for ex tax pricing where tax is calculated per row, not per item).</remarks>
        public double IR_TaxPerItem
        {
            get
            {
                if (PricesIncTax)
                    // Return currency amount
                    return Math.Round((TaxAmount), BasketBLL.CurrencyRoundNumber);
                else
                    // Return number (e.g. tax rate 17.5%, returns 0.175)
                    return ComputedTaxRate;
            }
        }

        /// <summary>
        ///         ''' Create a new instance of the BasketItem object
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public BasketItem()
        {
            PricesIncTax = (LCase(GetKartConfig("general.tax.pricesinctax")) == "y");

            HasCombinations = false;
            VersionID = 0;

            // ' added
            _VersionID = 0;
            ComboVersionID = 0;
            _Free = false;
        }
    }
}
