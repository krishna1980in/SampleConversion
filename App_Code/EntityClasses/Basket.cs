using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using System.Web.HttpContext;

namespace Kartris
{
    /// <summary>
    ///     ''' The customer basket which will contain zero or more basket items (rows)
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    [Serializable()]
    public class Basket
    {
        public int DB_C_CustomerID;           // Customer ID (User ID) that owns this basket

        // Basket modifiers. These affect the entire basket in one hit.
        public BasketModifier ShippingPrice;
        public BasketModifier OrderHandlingPrice;
        public BasketModifier CouponDiscount;
        public BasketModifier CustomerDiscount;
        public BasketModifier PromotionDiscount;

        public double CustomerDiscountPercentage { get; set; }

        /// <summary>
        ///         ''' All of the items that are in the basket
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        private List<Kartris.BasketItem> _BasketItems;

        private bool _AllFreeShipping, _HasFreeShipping, _PricesIncTax, _AllDigital, _HasCustomerDiscountExemption;
        private double _TotalWeight, _TotalExTax, _TotalTaxAmount, _TotalAmountSaved;
        private float _TotalItems;
        private double _ShippingTotalWeight, _ShippingTotalExTax, _ShippingTotalTaxAmount;

        private double _TotalDiscountPriceIncTax, _TotalDiscountPriceExTax;
        private double _TotalDiscountPriceTaxAmount, _TotalDiscountPriceTaxRate;

        // v2.9010 added way to exclude items from customer discount
        // Often we have mods which require calculating a subtotal of 
        // cart items. We'll create variables here for this, but these
        // could be used for other custom mods in future requiring
        // similar.
        private double _SubTotalExTax, _SubTotalTaxAmount;

        private double _TotalIncTax, _TotalTaxRate, _DTax, _DTax2;
        private string _DTaxExtra;

        private int _LastIndex, _HighlightLowStockRowNumber;
        private bool _AdjustedForElectronic, _ApplyTax, _AdjustedQuantities;
        private string _ShippingName, _ShippingDescription;

        private string _CouponName;
        private string _CouponCode;

        public List<Kartris.Promotion> objPromotions = new List<Kartris.Promotion>();
        public ArrayList objPromotionsDiscount = new ArrayList();

        private int numLanguageID;
        private int numCurrencyID = 0;
        private long numSessionID;

        /// <summary>
        ///         ''' The currency that is being used for the basket.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>Taken from the currency table</remarks>
        public int CurrencyID
        {
            get
            {
                return numCurrencyID;
            }
            set
            {
                numCurrencyID = value;
            }
        }

        /// <summary>
        ///         ''' Total number of items in the basket.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>Not the total number of rows in the basket as some rows have multiple items.</remarks>
        public float TotalItems
        {
            get
            {
                return _TotalItems;
            }
        }

        /// <summary>
        ///         ''' There is at least one item in the basket that has free shipping.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public bool HasFreeShipping
        {
            get
            {
                return _HasFreeShipping;
            }
        }

        /// <summary>
        ///         ''' All items in the basket have free shipping.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public bool AllFreeShipping
        {
            get
            {
                return _AllFreeShipping;
            }
        }

        /// <summary>
        ///         ''' All items in the basket are digital downloads
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns>The basket does not contains any physical stocked or shipped items.</returns>
        ///         ''' <remarks></remarks>
        public bool AllDigital
        {
            get
            {
                return _AllDigital;
            }
        }

        /// <summary>
        ///         ''' Total amount of money saved compared to RRP.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>Aggregation of RowAmountSaved() from each BasketItem</remarks>
        public double TotalAmountSaved
        {
            get
            {
                return Math.Max(_TotalAmountSaved, 0);
            }
        }

        /// <summary>
        ///         ''' Total weight of all items to be shipped
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>will be zero for items that have free shipping</remarks>
        public double ShippingTotalWeight
        {
            get
            {
                return _ShippingTotalWeight;
            }
        }

        /// <summary>
        ///         ''' Total price of all shipping excluding the tax
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double ShippingTotalExTax
        {
            get
            {
                return Math.Round(_ShippingTotalExTax, BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' Total amount of tax that is due to the shipping price
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double ShippingTotalTaxAmount
        {
            get
            {
                return Math.Round(_ShippingTotalTaxAmount, BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' Total price of all shipping including the tax
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double ShippingTotalIncTax
        {
            get
            {
                return Math.Round(ShippingTotalExTax + ShippingTotalTaxAmount, BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' At least one item within the basket has had its quantity adjusted automatically as part of the validation process
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>Because the customer has attempted to purchase more items than are available in stock and stock tracking is set to ON</remarks>
        public bool AdjustedQuantities
        {
            get
            {
                return _AdjustedQuantities;
            }
        }

        /// <summary>
        ///         ''' Which row number (item index in basket) has been modified due to that product being low / out of stock?
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>Used to highlight the row that was stock adjusted as part of validation process. See AdjustedQuantities</remarks>
        public int HighlightLowStockRowNumber
        {
            get
            {
                return _HighlightLowStockRowNumber;
            }
        }

        /// <summary>
        ///         ''' If the item is electronic (such as a download) and only one download per electronic item is permitted (config setting) but the user has 
        ///         ''' put in more than one quantity. 
        ///         ''' If this happens we reduce the quantity to 1 and set this flag to true to indicate that the customer needs to be alerted to this fact.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public bool AdjustedForElectronic
        {
            get
            {
                return _AdjustedForElectronic;
            }
        }

        /// <summary>
        ///         ''' The name of the selected shipping method.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>e.g. 'Standard Post', 'Express Delivery' etc. taken from view vKartrisTypeShippingMethods</remarks>
        public string ShippingName
        {
            get
            {
                if (AllFreeShipping)
                {
                    ShippingPrice.IncTax = 0; ShippingPrice.ExTax = 0;
                    return System.Web.HttpContext.GetGlobalResourceObject("Kartris", "ContentText_ShippingElectronic");
                }
                else
                    return _ShippingName + "";
            }
        }

        /// <summary>
        ///         ''' a description of the selection shipping method
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>e.g. 'Overnight domestic, 2 days EU' taken from view vKartrisTypeShippingMethods</remarks>
        public string ShippingDescription
        {
            get
            {
                if (AllFreeShipping)
                {
                    ShippingPrice.IncTax = 0; ShippingPrice.ExTax = 0;
                    return HttpContext.GetGlobalResourceObject("Kartris", "ContentText_ShippingElectronicDesc");
                }
                else
                    return _ShippingDescription;
            }
        }

        /// <summary>
        ///         ''' The total weight for all items in the basket
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>ignores modifiers such as free shipping items</remarks>
        public double TotalWeight
        {
            get
            {
                return _TotalWeight;
            }
        }

        /// <summary>
        ///         ''' The total value of the items in the basket excluding the tax
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double TotalExTax
        {
            get
            {
                return Math.Round(_TotalExTax, BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' The total value of the items in the basket including the tax
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double TotalIncTax
        {
            get
            {
                return Math.Round(TotalExTax + TotalTaxAmount, BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' **Total value of items in basket within subtotal (i.e. by default items set not to be included in customer discount)
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double SubTotalExTax
        {
            get
            {
                return Math.Round(_SubTotalExTax, BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' **Total amount of tax subtotal items in basket (i.e. by default items set not to be included in customer discount)
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double SubTotalTaxAmount
        {
            get
            {
                return Math.Round(_SubTotalTaxAmount, BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' **Total value of items inc tax in basket within subtotal (i.e. by default items set not to be included in customer discount)
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double SubTotalIncTax
        {
            get
            {
                return Math.Round(SubTotalExTax + SubTotalTaxAmount, BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' **If basket has at least one item with customer discount exemption
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public bool HasCustomerDiscountExemption
        {
            get
            {
                return _HasCustomerDiscountExemption;
            }
        }

        /// <summary>
        ///         ''' The average rate of tax for all items in the basket
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>This is because different items in the basket may be using different tax rates (multiple tiers etc.) 
        ///         ''' and so if you need to apply tax to the entire basket as a whole (when using coupons for example) 
        ///         ''' you need the average tax of the basket as a whole.</remarks>
        public double TotalTaxRate
        {
            get
            {
                int numRounding = 0;
                if (ConfigurationManager.AppSettings("TaxRegime").ToLower != "us" & ConfigurationManager.AppSettings("TaxRegime").ToLower != "simple")
                    numRounding = 4;
                else
                    numRounding = 6;
                if (TotalExTax == 0)
                    return 0;
                else
                    return Math.Round(TotalTaxAmount / TotalExTax, numRounding);
            }
        }

        /// <summary>
        ///         ''' Total amount of tax for the entire basket
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double TotalTaxAmount
        {
            get
            {
                return Math.Round(_TotalTaxAmount, BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' Total price of the entire basket, after discount, including tax. 
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double TotalDiscountPriceIncTax
        {
            get
            {
                return Math.Round(TotalIncTax + CouponDiscount.IncTax + PromotionDiscount.IncTax + CustomerDiscount.IncTax, BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' Total price of the entire basket, after discount, including tax. 
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double TotalDiscountPriceExTax
        {
            get
            {
                return Math.Round(TotalExTax + CouponDiscount.ExTax + PromotionDiscount.ExTax + CustomerDiscount.ExTax, BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' Total amount of tax that is applied to the whole basket, after the discount has been applied.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double TotalDiscountPriceTaxAmount
        {
            get
            {
                return Math.Round(TotalDiscountPriceIncTax - TotalDiscountPriceExTax, BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' The rate of tax that has been applied to the basket as a whole after the discount has been applied.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double TotalDiscountPriceTaxRate
        {
            get
            {
                int numRounding = 0;
                if (ConfigurationManager.AppSettings("TaxRegime").ToLower != "us" & ConfigurationManager.AppSettings("TaxRegime").ToLower != "simple")
                    numRounding = 4;
                else
                    numRounding = 6;
                return Math.Round(Interaction.IIf(TotalDiscountPriceExTax == 0, 0, TotalDiscountPriceTaxAmount / TotalDiscountPriceExTax), numRounding);
            }
        }

        /// <summary>
        ///         ''' The total price to be charged to the customer including  tax.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>Includes the items, discounts, coupons and shipping</remarks>
        public double FinalPriceIncTax
        {
            get
            {
                double numFinalPriceIncTax;
                numFinalPriceIncTax = Math.Round(TotalDiscountPriceIncTax + ShippingPrice.IncTax + OrderHandlingPrice.IncTax, BasketBLL.CurrencyRoundNumber);
                return Interaction.IIf(numFinalPriceIncTax < 0, 0, numFinalPriceIncTax);
            }
        }

        /// <summary>
        ///         ''' The total price to the charged to the customer excluding tax.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>Includes the items, discounts, coupons and shipping</remarks>
        public double FinalPriceExTax
        {
            get
            {
                double numFinalPriceExTax;
                numFinalPriceExTax = Math.Round(TotalDiscountPriceExTax + ShippingPrice.ExTax + OrderHandlingPrice.ExTax, BasketBLL.CurrencyRoundNumber);
                return Interaction.IIf(numFinalPriceExTax < 0, 0, numFinalPriceExTax);
            }
        }

        /// <summary>
        ///         ''' The total amount that is being charged to the customer once all charges are calculated.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>Includes the items, discounts, coupons and shipping</remarks>
        public double FinalPriceTaxAmount
        {
            get
            {
                return Math.Round(FinalPriceIncTax - FinalPriceExTax, BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' Tax should be applied to the basket. 
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>If you write this variable all basket items will have their descrete properties for ApplyTax overwritten with this value</remarks>
        public bool ApplyTax
        {
            get
            {
                return _ApplyTax;
            }
            set
            {
                _ApplyTax = value;
                // Loop through all basket items and set their tax setting
                foreach (Kartris.BasketItem Item in BasketItems)
                    Item.ApplyTax = _ApplyTax;
                // Rebuild
                CalculateTotals();
            }
        }

        /// <summary>
        ///         ''' The prices given include tax
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>If you write this variable all basket items will have their descrete properties for PricesIncTax overwritten with this value</remarks>
        public bool PricesIncTax
        {
            get
            {
                return _PricesIncTax;
            }
            set
            {
                if (_PricesIncTax != value)
                {
                    _PricesIncTax = value;
                    // Loop through all basket items and set their tax setting
                    foreach (Kartris.BasketItem Item in BasketItems)
                        Item.PricesIncTax = _PricesIncTax;
                    // Rebuild
                    CalculateTotals();
                }
            }
        }

        /// <summary>
        ///         ''' The name of the applied coupon
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public string CouponName
        {
            get
            {
                return _CouponName;
            }
        }

        /// <summary>
        ///         '''  The code of the applied coupon
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public string CouponCode
        {
            get
            {
                return _CouponCode;
            }
        }

        /// <summary>
        ///         ''' Same as TotalExTax
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>Appears to be legacy</remarks>
        public double TotalValueToAffiliate
        {
            get
            {
                return Math.Round(TotalExTax / 1, BasketBLL.CurrencyRoundNumber);
            }
        }

        /// <summary>
        ///         ''' Prevent an accidental round down.
        ///         ''' </summary>
        ///         ''' <param name="numValue"></param>
        ///         ''' <param name="numDecimalPlaces"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks>TODO: move into a general utility and out of the basket entity</remarks>
        public static double SafeRound(double numValue, int numDecimalPlaces)
        {
            return Math.Round(numValue + 0.00001, numDecimalPlaces);
        }

        /// <summary>
        ///         ''' Prevent an accidental round up.
        ///         ''' </summary>
        ///         ''' <param name="numValue"></param>
        ///         ''' <param name="numDecimalPlaces"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks>TODO: move into a general utility and out of the basket entity</remarks>
        public static double NegSafeRound(double numValue, int numDecimalPlaces)
        {
            NegSafeRound = Math.Round(numValue - 0.00001, numDecimalPlaces);
        }

        /// <summary>
        ///         ''' Flexible variable. Used within TaxRegime.config
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>The purpose of this variable can be altered by changing what it is doing from within the config file.</remarks>
        public double D_Tax
        {
            get
            {
                return _DTax;
            }
            set
            {
                _DTax = value;
            }
        }

        /// <summary>
        ///         ''' Flexible variable. Used within TaxRegime.config
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>The purpose of this variable can be altered by changing what it is doing from within the config file.</remarks>
        public double D_Tax2
        {
            get
            {
                return _DTax2;
            }
            set
            {
                _DTax2 = value;
            }
        }

        /// <summary>
        ///         ''' The TaxRateCalculation name that should be applied when calculating the tax rate.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>See TaxBLL.CalculateTaxRate()</remarks>
        public string D_TaxExtra
        {
            get
            {
                return _DTaxExtra;
            }
            set
            {
                _DTaxExtra = value;
            }
        }

        /// <summary>
        ///         ''' Get all items in the basket
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public List<Kartris.BasketItem> BasketItems
        {
            get
            {
                if (Information.IsNothing(_BasketItems))
                    // Basket items have not been loaded
                    _LoadBasketItems();
                return _BasketItems;
            }
            set
            {
                _BasketItems = value;
            }
        }

        /// <summary>
        ///         ''' Load basket items from the database into memory
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public void LoadBasketItems()
        {
            // Public accessor method
            _LoadBasketItems();
        }

        /// <summary>
        ///         ''' Load basket items from the database into memory
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        private void _LoadBasketItems()
        {
            _BasketItems = BasketBLL.GetBasketItems;
            _ResetBasketModifiers();
        }

        /// <summary>
        ///         ''' Set basket modifiers to a reset state
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        private void _ResetBasketModifiers()
        {
            ShippingPrice = new BasketModifier();
            OrderHandlingPrice = new BasketModifier();
            CouponDiscount = new BasketModifier();
            CustomerDiscount = new BasketModifier();
            PromotionDiscount = new BasketModifier();
        }

        /// <summary>
        ///         ''' Calculate all of the total values for the basket such as price tax etc.
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public void CalculateTotals()
        {
            // Set intial settings
            _AllFreeShipping = true;
            _AllDigital = true;
            _HasFreeShipping = false;
            _TotalItems = 0;
            _TotalWeight = 0;
            _TotalExTax = 0;
            _TotalTaxAmount = 0;
            _TotalAmountSaved = 0;
            _SubTotalExTax = 0;
            _SubTotalTaxAmount = 0;
            _HasCustomerDiscountExemption = false;
            _ShippingTotalWeight = 0;
            _ShippingTotalExTax = 0;
            _ShippingTotalTaxAmount = 0;

            // Loop through all items in the basket
            foreach (Kartris.BasketItem Item in BasketItems)
            {
                {
                    var withBlock = Item;
                    // Are all the items free-shipping?
                    _AllFreeShipping = _AllFreeShipping && withBlock.FreeShipping;

                    // Is at least one item got free shipping?
                    _HasFreeShipping = _HasFreeShipping || withBlock.FreeShipping;

                    _AllDigital = _AllDigital && (withBlock.DownloadType == "l" | withBlock.DownloadType == "u");

                    // If we have an item in the basket that is sold in fractional
                    // quantities, such as rope sold by the metre, then we need to 
                    // think about how quantities are totalled. For example, if we
                    // place 0.05 of an item in the basket, this is really one item
                    // but would be considered as 0.05 items in the count. Similarly
                    // if we purchase 2.5m, it would be considered as 2.5 items. In
                    // both cases, these are really just one item. So, we only add
                    // quantities if the item is a round number.
                    if (Math.Round(withBlock.Quantity, 0) == withBlock.Quantity)
                        _TotalItems = _TotalItems + withBlock.Quantity;
                    else
                        _TotalItems = _TotalItems + 1;// decimal qty, so assume is one item

                    // Other totals
                    _TotalWeight = _TotalWeight + withBlock.RowWeight;
                    _TotalExTax = _TotalExTax + withBlock.RowExTax;
                    _TotalTaxAmount = _TotalTaxAmount + withBlock.RowTaxAmount;
                    _TotalAmountSaved = _TotalAmountSaved + withBlock.RowAmountSaved;

                    // Subtotals
                    if (withBlock.ExcludeFromCustomerDiscount)
                    {
                        _SubTotalExTax = _SubTotalExTax + withBlock.RowExTax;
                        if (ApplyTax)
                            _SubTotalTaxAmount = _SubTotalTaxAmount + withBlock.RowTaxAmount;
                        _HasCustomerDiscountExemption = true;
                    }

                    // Shipping Totals
                    if (!withBlock.FreeShipping)
                    {
                        _ShippingTotalWeight = _ShippingTotalWeight + withBlock.RowWeight;
                        _ShippingTotalExTax = _ShippingTotalExTax + withBlock.RowExTax;
                        if (ApplyTax)
                            _ShippingTotalTaxAmount = _ShippingTotalTaxAmount + withBlock.RowTaxAmount;
                    }
                }
            }
        }

        public void SetCouponName(string value)
        {
            // Public accessor
            _CouponName = value;
        }

        public void SetCouponCode(string value)
        {
            // Public accessor
            _CouponCode = value;
        }

        /// <summary>
        ///         ''' Check to make sure all of the rows in the basket are valid and there are no data errors.
        ///         ''' </summary>
        ///         ''' <param name="blnAllowOutOfStock">Should we accept the basket as valid if it contains out of stock product</param>
        ///         ''' <returns></returns>
        ///         ''' <remarks>While validating this function also triggers some totalisers so it is not strictly just validating data</remarks>
        public bool Validate(bool blnAllowOutOfStock)
        {
            bool blnAllowPurchaseOutOfStock, blnOnlyOneDownload;
            bool blnWarn;
            double numWeight, numPrice;
            string V_DownloadType;
            short SESS_CurrencyID;
            float numUnitSize;
            int i = 0;              // For...Each itteration counter

            if (numCurrencyID > 0)
                SESS_CurrencyID = numCurrencyID;
            else
                SESS_CurrencyID = System.Web.HttpContext.Current.Session["CUR_ID"];

            // Set variables from application
            _AdjustedForElectronic = false;
            _AdjustedQuantities = false;

            // Get configuration flags
            PricesIncTax = (LCase(KartSettingsManager.GetKartConfig("general.tax.pricesinctax")) == "y");     // Prices include tax already when stored in the database.
            blnAllowPurchaseOutOfStock = LCase(KartSettingsManager.GetKartConfig("frontend.orders.allowpurchaseoutofstock")) == "y";      // Allow people to order stuff that is already out of stock
            blnOnlyOneDownload = LCase(KartSettingsManager.GetKartConfig("onlyonedownload")) == "y";          // When purchasing digital downloads only allow a single download of each file.

            // Only proceed if we have basket items
            if (BasketItems.Count > 0)
            {

                // 'Loop through all basket records
                foreach (Kartris.BasketItem Item in BasketItems)
                {
                    {
                        var withBlock = Item;
                        // Craig Moore 201505061107. I cannot see why this needs to be here. .Weight is read into
                        // numWeight and then about 100 lines down it is read back out. There are no triggers, methods
                        // or anything else. I suspect this can be removed. along with the line later down where it is 
                        // read back out. 
                        numWeight = withBlock.Weight;

                        withBlock.ComputedTaxRate = TaxRegime.CalculateTaxRate(withBlock.TaxRate1, withBlock.TaxRate2, Interaction.IIf(D_Tax > 0, D_Tax, 1), Interaction.IIf(D_Tax2 > 0, D_Tax2, 1), D_TaxExtra);

                        if (D_Tax > 0 | D_Tax2 > 0)
                            ApplyTax = true;
                        else
                            ApplyTax = false;

                        // Set whether this item has free shipping
                        V_DownloadType = LCase(withBlock.DownloadType);
                        withBlock.FreeShipping = !(V_DownloadType == "" || V_DownloadType == "n");

                        // If the item is electronic, one download is allowed, and the user has
                        // more than one for quantity, change quantity and warn user
                        if (blnOnlyOneDownload && (V_DownloadType == "l" || V_DownloadType == "u") && withBlock.Quantity > 1)
                        {
                            withBlock.Quantity = 1;
                            _AdjustedForElectronic = true;
                        }

                        // Adjust quantities if stock level too low
                        if (!blnAllowPurchaseOutOfStock)
                        {
                            // Only warn if tracking turned on for this item (warn level not set to 0)
                            blnWarn = (withBlock.QtyWarnLevel != 0);
                            if (blnWarn)
                            {
                                if (withBlock.Quantity > withBlock.StockQty)
                                {
                                    withBlock.AdjustedQty = true;
                                    _AdjustedQuantities = true;
                                    _HighlightLowStockRowNumber = i;
                                    withBlock.Quantity = withBlock.StockQty;
                                }
                            }
                        }

                        // Here we check that the quantity for each items is ok
                        // with the unitsize for that item. For example, if items
                        // can only be bought in units of 10, we need to ensure
                        // the qty of that item is a multiple of 10, or fix it
                        // down.
                        ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
                        numUnitSize = CkartrisDataManipulation.FixNullFromDB(objObjectConfigBLL.GetValue("K:product.unitsize", withBlock.P_ID));
                        withBlock.Quantity = (withBlock.Quantity - CkartrisDataManipulation.SafeModulus(withBlock.Quantity, numUnitSize));

                        // Get basic price, modify for customer group pricing (if lower)
                        numPrice = withBlock.Price;

                        // Get basic price, modify for customer group pricing (if lower)
                        double numCustomerGroupPrice;
                        int numBaseVersionID = withBlock.VersionBaseID;
                        int numActualVersionID = withBlock.VersionID;
                        int numVersionID = 0;

                        // For options products, look up customer group discounts
                        // related to the base version. For combinations products
                        // use the actual version
                        if (withBlock.HasCombinations)
                            // Set version ID to use for price to the actual version in basket
                            numVersionID = numActualVersionID;
                        else
                            // Set version ID to use for price to the base version
                            numVersionID = numBaseVersionID;

                        numCustomerGroupPrice = BasketBLL.GetCustomerGroupPriceForVersion(DB_C_CustomerID, numVersionID);

                        if (numCustomerGroupPrice > 0)
                        {
                            // convert customer group price to current user currency
                            numCustomerGroupPrice = Math.Round(System.Convert.ToDouble(CurrenciesBLL.ConvertCurrency(SESS_CurrencyID, numCustomerGroupPrice)), BasketBLL.CurrencyRoundNumber);
                            numPrice = Math.Min(numCustomerGroupPrice, numPrice);
                        }

                        if (!string.IsNullOrEmpty(withBlock.CustomText))
                        {
                            string strCustomControlName = objObjectConfigBLL.GetValue("K:product.customcontrolname", withBlock.ProductID);
                            if (!string.IsNullOrEmpty(strCustomControlName))
                                // Split the custom text field
                                string[] arrParameters = Split(withBlock.CustomText, "|||");
                            else
                                numPrice += withBlock.CustomCost;
                        }

                        // Get price break level (if lower)
                        if (System.Convert.ToInt32(KartSettingsManager.GetKartConfig("general.quantitydiscounts.bands")) > 0)
                        {
                            double numDiscountPrice = 0;
                            numDiscountPrice = BasketBLL.GetQuantityDiscount(withBlock.VersionBaseID, withBlock.Quantity);
                            if (numDiscountPrice > 0)
                            {
                                // convert discount price to current user currency
                                numDiscountPrice = Math.Round(System.Convert.ToDouble(CurrenciesBLL.ConvertCurrency(SESS_CurrencyID, numDiscountPrice)), BasketBLL.CurrencyRoundNumber);
                                numPrice = Math.Min(numDiscountPrice, numPrice);
                            }
                        }

                        if (LCase(withBlock.ProductType) == "o")
                            numPrice = numPrice + CurrenciesBLL.ConvertCurrency(SESS_CurrencyID, withBlock.OptionPrice);

                        // Calculate the ex-tax - this differs as numPrice will hold 
                        // a different value depending on pricesinctax
                        if (PricesIncTax)
                            withBlock.ExTax = numPrice * (1 / (double)(1 + withBlock.ComputedTaxRate));
                        else
                            withBlock.ExTax = numPrice;

                        if (!ApplyTax)
                            withBlock.ComputedTaxRate = 0;

                        // Craig Moore 201505061107. I cannot see why this needs to be here. .Weight is read into
                        // numWeight 100 lines up and then here it is read back out. There are no triggers, methods
                        // or anything else. I suspect this can be removed. along with the line further up where it is 
                        // read out in the first place.
                        withBlock.Weight = numWeight;

                        // If item set as callforprice at product or version level,
                        // remove it from the basket

                        if (ReturnCallForPrice(withBlock.ProductID, withBlock.VersionID) == 1)
                            withBlock.Quantity = 0;

                        if (withBlock.Quantity == 0)
                            // BasketItems.Remove(Item)
                            // v2.9010 fix, need to delete item rather than update qty to zero,
                            // that doesn't seem to work now
                            // BasketBLL.UpdateQuantity(.ID, 0)
                            BasketBLL.DeleteBasketItems(withBlock.ID);
                        else
                            // IMPORTANT!
                            // If we remove items from the basket, we don't want to advance the 
                            // counter or we get an error at the "next"
                            i = i + 1;
                    }
                }
            }
            return true;
        }

        /// <summary>
        ///         ''' Calculate the shipping using data within an instance of Interfaces.objShippingDetails
        ///         ''' </summary>
        ///         ''' <param name="numLanguageID">Language</param>
        ///         ''' <param name="numShippingID">Selected shipping method</param>
        ///         ''' <param name="numShippingPriceValue">NOT USED. May be here for legacy or copy / paste error.</param>
        ///         ''' <param name="numDestinationID">Destination location</param>
        ///         ''' <param name="objShippingDetails">Information about the items to be shipped such as total weight and value</param>
        ///         ''' <remarks></remarks>
        public void CalculateShipping(int numLanguageID, int numShippingID, double numShippingPriceValue, int numDestinationID, Interfaces.objShippingDetails objShippingDetails)
        {
            ShippingPrice = new BasketModifier();

            if (numShippingID == 999)
            {
                _ShippingName = System.Web.HttpContext.GetGlobalResourceObject("Shipping", "ContentText_ShippingPickup");
                _ShippingDescription = System.Web.HttpContext.GetGlobalResourceObject("Shipping", "ContentText_ShippingPickupDesc");
                {
                    var withBlock = ShippingPrice;
                    withBlock.ExTax = 0;
                    withBlock.IncTax = 0;
                    withBlock.TaxRate = 0;
                }
            }
            else if (numDestinationID > 0 & numShippingID > 0)
            {
                try
                {
                    int numCurrencyID;
                    numCurrencyID = System.Web.HttpContext.Current.Session["CUR_ID"];
                    double ShippingBoundary;
                    if (KartSettingsManager.GetKartConfig("frontend.checkout.shipping.calcbyweight") == "y")
                        ShippingBoundary = _ShippingTotalWeight;
                    else if (KartSettingsManager.GetKartConfig("general.tax.pricesinctax") != "y")
                        ShippingBoundary = _ShippingTotalExTax;
                    else
                        ShippingBoundary = ShippingTotalIncTax;

                    KartrisClasses.ShippingMethod SelectedSM = KartrisClasses.ShippingMethod.GetByID(objShippingDetails, numShippingID, numDestinationID, CurrenciesBLL.ConvertCurrency(CurrenciesBLL.GetDefaultCurrency, ShippingBoundary, numCurrencyID), numLanguageID);
                    {
                        var withBlock = SelectedSM;
                        _ShippingName = withBlock.Name + "";
                        _ShippingDescription = withBlock.Description + "";

                        ShippingPrice.ExTax = CurrenciesBLL.ConvertCurrency(numCurrencyID, withBlock.ExTax);
                        ShippingPrice.IncTax = CurrenciesBLL.ConvertCurrency(numCurrencyID, withBlock.IncTax);
                        ShippingPrice.TaxRate = IIf(ApplyTax, withBlock.ComputedTaxRate, 0);
                    }
                }
                catch (Exception ex)
                {
                    CkartrisFormatErrors.LogError("BasketBLL.CalculateShipping: " + ex.Message + "\r\n" + "DestinationID: " + numDestinationID + "\r\n"
                  + "ShippingID: " + numShippingID + "\r\n"
                  + "This can happen if there is no valid shipping method for the weight/cost of this order.");

                    _ShippingName = "";
                    _ShippingDescription = "";
                }
            }
            else
            {
                _ShippingName = "";
                _ShippingDescription = "";
                if (numShippingID > 0)
                    CkartrisFormatErrors.LogError("BasketBLL.CalculateShipping Error - " + "DestinationID: " + numDestinationID + "\r\n"
                                         + "ShippingID: " + numShippingID);
            }
        }

        // ''' <summary>
        // ''' [DO NOT USE] Get all items within the basket
        // ''' </summary>
        // ''' <returns>ArrayList of BasketItem</returns>
        // ''' <remarks>Each item is a row, so if you have multiples of a product that would still be one item but with a quantity greater than 1</remarks>
        // Public Function GetItems() As List(Of Kartris.BasketItem)
        // 'Return BasketItems
        // Throw New ApplicationException("GetItems() no longer implemented. Instead just reference the BasketItems List Object")
        // End Function

        /// <summary>
        ///         ''' Create a new basket object
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public Basket()
        {
            _ApplyTax = true;
            _ResetBasketModifiers();
        }

        ~Basket()
        {
            base.Finalize();
        }

        /// <summary>
        ///         ''' Call for Price
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public Int16 ReturnCallForPrice(Int64 numP_ID, Int64 numV_ID = 0)
        {
            ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
            object objValue = objObjectConfigBLL.GetValue("K:product.callforprice", numP_ID);
            if (System.Convert.ToInt32(objValue) == 0 & numV_ID != 0)
                // Product not call for price, maybe there is a version
                objValue = objObjectConfigBLL.GetValue("K:version.callforprice", numV_ID);
            return System.Convert.ToInt16(objValue);
        }
    }
}
