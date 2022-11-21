// ========================================================================
// Kartris - www.kartris.com
// Copyright 2021 CACTUSOFT

// GNU GENERAL PUBLIC LICENSE v2
// This program is free software distributed under the GPL without any
// warranty.
// www.gnu.org/licenses/gpl-2.0.html

// KARTRIS COMMERCIAL LICENSE
// If a valid license.config issued by Cactusoft is present, the KCL
// overrides the GPL v2.
// www.kartris.com/t-Kartris-Commercial-License.aspx
// ========================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using kartrisBasketDataTableAdapters;
using System.Web.HttpContext;
using KartSettingsManager;
using kartrisBasketData;
using CkartrisDataManipulation;
using KartrisClasses;
using SiteMapHelper;
using MailChimp.Net.Models;
using MailChimp.Net.Core;

/// <summary>

/// ''' Basket business logic layer

/// ''' </summary>

/// ''' <remarks></remarks>
public class BasketBLL
{

    /// <summary>
    ///     ''' Where the basket comes from. This can be a normal basket or something saved such as a wishlist etc.
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public enum BASKET_PARENTS
    {
        BASKET = 1,
        SAVED_BASKET = 2,
        WISH_LIST = 3
    }

    /// <summary>
    ///     ''' Defines the usage purpose of this entity object
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public enum VIEW_TYPE
    {
        MAIN_BASKET = 1,
        CHECKOUT_BASKET = 2,
        MINI_BASKET = 3
    }

    /// <summary>
    ///     ''' Basket Values Table Adapter
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    protected static BasketValuesTblAdptr _BasketValuesAdptr
    {
        get
        {
            return new BasketValuesTblAdptr();
        }
    }

    /// <summary>
    ///     ''' Basket Options Table Adapter
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    protected static BasketOptionValuesTblAdptr _BasketOptionsAdptr
    {
        get
        {
            return new BasketOptionValuesTblAdptr();
        }
    }

    /// <summary>
    ///     ''' Coupons Table Adapter
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    protected static CouponsTblAdptr _CouponsAdptr
    {
        get
        {
            return new CouponsTblAdptr();
        }
    }

    /// <summary>
    ///     ''' Customers Table Adapter
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    protected static CustomersTblAdptr _CustomersAdptr
    {
        get
        {
            return new CustomersTblAdptr();
        }
    }

    /// <summary>
    ///     ''' The precision to be used with rounding currency numbers. 
    ///     ''' </summary>
    ///     ''' <returns></returns>
    ///     ''' <remarks>in other words how many numbers after the decimal point (e.g. 1.123456 to a precision of 3 is 1.123)</remarks>
    public static short CurrencyRoundNumber()
    {
        try
        {
            int numCurrencyID = System.Web.HttpContext.Current.Session["CUR_ID"];
            DataRow[] rowCurrencies = GetCurrenciesFromCache().Select("CUR_ID = " + numCurrencyID);
            return System.Convert.ToInt16(rowCurrencies[0]("CUR_RoundNumbers"));
        }
        catch (Exception ex)
        {
            return 2;
        }
    }

    /// <summary>
    ///     ''' Calculate the values of the coupons
    ///     ''' </summary>
    ///     ''' <param name="strCouponCode"></param>
    ///     ''' <param name="strCouponError"></param>
    ///     ''' <param name="blnZeroTotalTaxRate"></param>
    ///     ''' <remarks>This instanciates the CustomerDiscount and CouponDiscount BasketModifier but only initialises the CouponDiscount</remarks>
    public static void CalculateCoupon(ref Kartris.Basket Basket, string strCouponCode, ref string strCouponError, bool blnZeroTotalTaxRate)
    {
        string strCouponType = "";
        double numCouponValue;
        {
            var withBlock = Basket;
            withBlock.CouponDiscount = new Kartris.BasketModifier();
            withBlock.CustomerDiscount = new Kartris.BasketModifier();       // Possibly remove?

            GetCouponDiscount(ref Basket, strCouponCode, ref strCouponError, ref strCouponType, ref numCouponValue);

            withBlock.CouponDiscount.TaxRate = withBlock.TotalDiscountPriceTaxRate;
            switch (strCouponType.ToLower())
            {
                case "p":
                    {
                        // Percentage coupons
                        withBlock.CouponDiscount.IncTax = -Math.Round((numCouponValue * withBlock.TotalDiscountPriceIncTax / (double)100), CurrencyRoundNumber);
                        withBlock.CouponDiscount.ExTax = -Math.Round((numCouponValue * withBlock.TotalDiscountPriceExTax / (double)100), CurrencyRoundNumber);
                        break;
                    }

                case "f":
                    {
                        // Fixed rate coupons
                        if (numCouponValue > withBlock.TotalDiscountPriceExTax)
                        {
                            withBlock.CouponDiscount.ExTax = -withBlock.TotalDiscountPriceExTax;
                            withBlock.CouponDiscount.IncTax = -withBlock.TotalDiscountPriceIncTax;
                        }
                        else
                        {
                            bool blnPricesExtax = false;

                            if (!blnZeroTotalTaxRate)
                            {
                                if (GetKartConfig("general.tax.pricesinctax") == "y")
                                {
                                    withBlock.CouponDiscount.ExTax = -Math.Round(numCouponValue * (1 / (double)(1 + withBlock.CouponDiscount.TaxRate)), CurrencyRoundNumber);
                                    if (withBlock.D_Tax == 1)
                                        withBlock.CouponDiscount.IncTax = -Math.Round(numCouponValue, CurrencyRoundNumber);
                                    else
                                        withBlock.CouponDiscount.IncTax = withBlock.CouponDiscount.ExTax;
                                }
                                else
                                    blnPricesExtax = true;
                            }
                            else
                                blnPricesExtax = true;

                            if (blnPricesExtax)
                            {
                                withBlock.CouponDiscount.IncTax = -Math.Round(numCouponValue, CurrencyRoundNumber);
                                withBlock.CouponDiscount.ExTax = -Math.Round(numCouponValue / (1 + withBlock.CouponDiscount.TaxRate), CurrencyRoundNumber);
                            }
                        }

                        break;
                    }

                default:
                    {
                        // Dodgy coupon - ignore
                        withBlock.CouponDiscount.IncTax = 0;
                        withBlock.CouponDiscount.ExTax = 0;
                        break;
                    }
            }
        }
    }

    /// <summary>
    ///     ''' Get information about an existing discount coupon.
    ///     ''' </summary>
    ///     ''' <param name="strCouponCode">Coupon to get the information from</param>
    ///     ''' <param name="strCouponError">Error data (return)</param>
    ///     ''' <param name="strCouponType">Type such as percentage or fixed amount (return)</param>
    ///     ''' <param name="numCouponValue">Value (return)</param>
    ///     ''' <remarks></remarks>
    public static void GetCouponDiscount(ref Kartris.Basket Basket, string strCouponCode, ref string strCouponError, ref string strCouponType, ref decimal numCouponValue)
    {
        CouponsDataTable tblCoupons;
        {
            var withBlock = Basket;
            withBlock.SetCouponName(""); withBlock.SetCouponCode("");

            tblCoupons = _CouponsAdptr.GetCouponCode(strCouponCode);
            if (tblCoupons.Rows.Count == 0)
                strCouponError = System.Web.HttpContext.GetGlobalResourceObject("Basket", "ContentText_CouponDoesntExist");
            else
            {
                DataRow drwCoupon = tblCoupons.Rows(0);
                if (drwCoupon("CP_Used") && !(drwCoupon("CP_Reusable")))
                    strCouponError = System.Web.HttpContext.GetGlobalResourceObject("Basket", "ContentText_CouponExpended");
                else if ((DateTime)drwCoupon("CP_StartDate") > CkartrisDisplayFunctions.NowOffset)
                    strCouponError = System.Web.HttpContext.GetGlobalResourceObject("Basket", "ContentText_CouponNotYetValid");
                else if ((DateTime)drwCoupon("CP_EndDate") < CkartrisDisplayFunctions.NowOffset || !(drwCoupon("CP_Enabled")))
                    strCouponError = System.Web.HttpContext.GetGlobalResourceObject("Basket", "ContentText_CouponExpired");
                else
                {
                    numCouponValue = drwCoupon("CP_DiscountValue");
                    strCouponType = drwCoupon("CP_DiscountType");
                    if (strCouponType.ToLower() == "p")
                        withBlock.SetCouponName(drwCoupon("CP_CouponCode") + " - " + numCouponValue + "%");
                    else if (strCouponType.ToLower() == "t")
                    {
                        numCouponValue = System.Convert.ToDecimal(numCouponValue);
                        withBlock.SetCouponName(drwCoupon("CP_CouponCode") + " - " + PromotionsBLL.GetPromotionText(numCouponValue, true));
                    }
                    else
                        withBlock.SetCouponName(drwCoupon("CP_CouponCode") + " - " + CurrenciesBLL.FormatCurrencyPrice(HttpContext.Current.Session("CUR_ID"), numCouponValue));
                    withBlock.SetCouponCode(drwCoupon("CP_CouponCode"));
                }
            }
        }
    }

    /// <summary>
    ///     ''' Calculate the value of the customer discount
    ///     ''' </summary>
    ///     ''' <param name="numCustomerDiscount"></param>
    ///     ''' <remarks></remarks>
    public new static void CalculateCustomerDiscount(ref Kartris.Basket Basket, double numCustomerDiscount)
    {
        {
            var withBlock = Basket;
            withBlock.CustomerDiscount = new Kartris.BasketModifier();
            withBlock.CustomerDiscount.TaxRate = withBlock.TotalDiscountPriceTaxRate;
            withBlock.CustomerDiscount.IncTax = -Math.Round((withBlock.TotalDiscountPriceIncTax - (withBlock.SubTotalExTax + withBlock.SubTotalTaxAmount)) * (numCustomerDiscount / 100), CurrencyRoundNumber);
            withBlock.CustomerDiscount.ExTax = -Math.Round((withBlock.TotalDiscountPriceExTax - withBlock.SubTotalExTax) * (numCustomerDiscount / 100), CurrencyRoundNumber);

            if (!(withBlock.ApplyTax))
                withBlock.CustomerDiscount.IncTax = withBlock.CustomerDiscount.ExTax;

            // Remember the percentage that was applied.
            withBlock.CustomerDiscountPercentage = numCustomerDiscount;
        }
    }

    /// <summary>
    ///     ''' Add a new item to a basket. This may be added to an existing row or create a new row. 
    ///     ''' </summary>
    ///     ''' <param name="pParentType">What type of basket is it (stored, wishlist etc.)</param>
    ///     ''' <param name="pParentID">The basket ID (or wishlist ID etc. if the parent is not a basket)</param>
    ///     ''' <param name="pVersionID">Product version ID to be added</param>
    ///     ''' <param name="pQuantity">Quantity of product to be added</param>
    ///     ''' <param name="pCustomText">Custom text if the product being added has custom text</param>
    ///     ''' <param name="pOptionsValues">Options value if the product being added has an options list.</param>
    ///     ''' <param name="numBasketValueID">The basket item from within the target basket that this product replaces. Only use for replacing, not for adding to existing</param>
    ///     ''' <remarks></remarks>
    public static void AddNewBasketValue(ref List<Kartris.BasketItem> BasketItems, BASKET_PARENTS pParentType, int pParentID, long pVersionID, float pQuantity, string pCustomText = null, string pOptionsValues = null, int numBasketValueID = 0)
    {
        char chrParentType = "";
        switch (pParentType)
        {
            case BASKET_PARENTS.BASKET:
                {
                    chrParentType = "b";
                    break;
                }

            case BASKET_PARENTS.SAVED_BASKET:
                {
                    chrParentType = "s";
                    break;
                }

            case BASKET_PARENTS.WISH_LIST:
                {
                    chrParentType = "w";
                    break;
                }
        }

        long numIdenticalBasketValueID = -1;

        if (numBasketValueID > 0)
            DeleteBasketItems(numBasketValueID);
        else
        {
            // ' check if item to be added already in basket
            string[] arrOptions = new[] { "" };
            if (!pOptionsValues == null)
                arrOptions = Strings.Split(pOptionsValues, ",");
            int cntOption;
            int numFoundBasketValueID = 0;
            foreach (Kartris.BasketItem oBasketItem in basketitems)
            {
                if (oBasketItem.VersionID == pVersionID)
                {
                    string strOption = Replace(oBasketItem.OptionLink, "&strOptions=", "");
                    if (strOption == "" || strOption == "0")
                    {
                        if (oBasketItem.CustomText == pCustomText)
                        {
                            numIdenticalBasketValueID = oBasketItem.ID; break;
                        }
                    }
                    else
                    {
                        cntOption = 0;
                        string[] arrBasketOptions;
                        arrBasketOptions = Strings.Split(strOption, ",");
                        if (Information.UBound(arrOptions) == Information.UBound(arrBasketOptions))
                        {
                            for (var i = Information.LBound(arrOptions); i <= Information.UBound(arrOptions); i++)
                            {
                                for (var j = Information.LBound(arrBasketOptions); j <= Information.UBound(arrBasketOptions); j++)
                                {
                                    if (arrOptions[i] == arrBasketOptions[j])
                                    {
                                        cntOption = cntOption + 1;
                                        break;
                                    }
                                }
                            }
                            if (arrOptions.Count() == cntOption)
                            {
                                numIdenticalBasketValueID = oBasketItem.ID;
                                break;
                            }
                        }
                    }
                }
            }
        }

        if (numIdenticalBasketValueID == -1)
        {
            // ' The Item is not in the basket, so it should be added.
            long numNewBasketValueID = default(Long);
            if (_BasketValuesAdptr.Insert(chrParentType, pParentID, pVersionID, pQuantity, pCustomText, CkartrisDisplayFunctions.NowOffset, null/* TODO Change to default(_) if this is not a reference type */, numNewBasketValueID) != 1)
                return;

            if (numBasketValueID > 0)
            {
                string[] aBasketItemInfo = Strings.Split(System.Web.HttpContext.Current.Session["BasketItemInfo"], ";");
                System.Web.HttpContext.Current.Session["BasketItemInfo"] = numNewBasketValueID + ";" + pVersionID + ";" + pQuantity;
            }

            // ' Adding the Item options as well (if any)
            if (!pOptionsValues == null)
            {
                string[] arrOptions = new string[] { "" };
                arrOptions = Strings.Split(pOptionsValues, ",");
                if (arrOptions.GetUpperBound(0) >= 0)
                {
                    for (int i = 0; i <= arrOptions.GetUpperBound(0); i++)
                    {
                        if (Conversion.Val(arrOptions[i]) != 0)
                            AddBasketOptionValues(numNewBasketValueID, System.Convert.ToInt32(arrOptions[i]));
                    }
                }
            }
            HttpContext.Current.Trace.Warn("^^^^^^^^^^^^^^ Version does NOT exist");
        }
        else
        {

            // ' The Item already exist in the basket, so the quantity will be increased.
            AddQuantityToMyBasket(numIdenticalBasketValueID, pQuantity);
            HttpContext.Current.Trace.Warn("^^^^^^^^^^^^^^ Version exists");
        }

        HttpContext.Current.Session("NewBasketItem") = 1;
    }

    /// <summary>
    ///     ''' Find a basket item within the given basket that matches the supplied product information
    ///     ''' </summary>
    ///     ''' <param name="pParentID">The basket ID (or wishlist ID etc. if the parent is not a basket)</param>
    ///     ''' <param name="pVersionID">Product version ID to be added</param>
    ///     ''' <param name="pOptionsValues">Options value if the product being added has an options list.</param>
    ///     ''' <param name="strCustomText">Custom text if the product being added has custom text</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks>Can be used to replace an existing entry of the same product in a given basket.</remarks>
    public static long GetIdenticalBasketValueID(int pParentID, long pVersionID, string pOptionsValues, string strCustomText = "")
    {
        short numLanguageID = 1;
        DataTable tblBasketItems = new DataTable();

        tblBasketItems = GetBasketItemsDatatable(pParentID, numLanguageID);

        DataRow[] drwIdenticalVersion;
        drwIdenticalVersion = tblBasketItems.Select("BV_VersionID=" + pVersionID + " AND isnull(BV_CustomText,'')='" + strCustomText + "'");

        string[] arrNewOptionValues = new string[] { };
        if (!pOptionsValues == null)
        {
            if (pOptionsValues != "")
                arrNewOptionValues = pOptionsValues.Split(",");
        }

        if (drwIdenticalVersion.Length == 0)
        {
            HttpContext.Current.Trace.Warn("^^^^^^ version does NOT exist in my basket ");
            return -1;
        }
        else
        {
            HttpContext.Current.Trace.Warn("^^^^^^ " + drwIdenticalVersion.GetUpperBound(0) + " Identical versions exist in my basket ");
            for (int i = 0; i <= drwIdenticalVersion.GetUpperBound(0); i++)
            {
                if (System.Convert.ToInt32(drwIdenticalVersion[i]("NoOfOptions")) != arrNewOptionValues.Length)
                    continue;
                else if (System.Convert.ToInt32(drwIdenticalVersion[i]("NoOfOptions")) != 0)
                {
                    // ' Read the options from database .. for the current BasketValueID
                    string strExistingOptionValues;
                    strExistingOptionValues = GetMiniBasketOptions(System.Convert.ToInt64(drwIdenticalVersion[i]("BV_ID")));

                    string[] arrOldOptionValues = new string[] { "" };
                    arrOldOptionValues = strExistingOptionValues.Split(",");

                    int numIdenticalOptionsCounter = 0;
                    for (int iNew = 0; iNew <= arrNewOptionValues.GetUpperBound(0); iNew++)
                    {
                        for (int iOld = 0; iOld <= arrOldOptionValues.GetUpperBound(0); iOld++)
                        {
                            if (arrNewOptionValues[iNew] == arrOldOptionValues[iOld])
                            {
                                numIdenticalOptionsCounter += 1;
                                break;
                            }
                        }
                    }

                    if (numIdenticalOptionsCounter == arrNewOptionValues.Length)
                        return System.Convert.ToInt64(drwIdenticalVersion[i]("BV_ID"));

                    HttpContext.Current.Trace.Warn("^^^^^^ options identical found BV_ID=" + drwIdenticalVersion[i]("BV_ID"));
                }
                else
                    return System.Convert.ToInt64(drwIdenticalVersion[i]("BV_ID"));
            }
            return -1;
        }
        return -1;
    }

    /// <summary>
    ///     ''' Add quantity to a given basket item. 
    ///     ''' </summary>
    ///     ''' <param name="pBasketValueID">Basket item ID</param>
    ///     ''' <param name="pQuantityToAdd">Quantity to add</param>
    ///     ''' <remarks>Basket item IDs are unique within the database so we do not need to know the basket as no two baskets will contain the same basket item IDs</remarks>
    private static void AddQuantityToMyBasket(long pBasketValueID, string pQuantityToAdd)
    {
        _BasketValuesAdptr.AddQuantityToItem(pQuantityToAdd, pBasketValueID);
    }

    /// <summary>
    ///     ''' Get all basket items for a given session. 
    ///     ''' </summary>
    ///     ''' <param name="pSessionID">Session ID</param>
    ///     ''' <param name="pLanguageID">Language that is being used (used for retrieving product and shipping names etc.)</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable GetBasketItemsDatatable(long pSessionID, short pLanguageID)
    {
        return _BasketValuesAdptr.GetBasketItems(pSessionID, pLanguageID);
    }

    public new static List<Kartris.BasketItem> GetBasketItems()
    {
        int numLanguageID = System.Web.HttpContext.Current.Session["LANG"];
        int numSessionID = System.Web.HttpContext.Current.Session["SessionID"];
        return GetBasketItems(numSessionID, numLanguageID);
    }

    /// <summary>
    ///     ''' Return a list of basket items retrieved from database
    ///     ''' </summary>
    ///     ''' <param name="SessionId">The session ID that the basket is associated with</param>
    ///     ''' <param name="LanguageId">Language that the text should be in.</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public new static List<Kartris.BasketItem> GetBasketItems(long SessionId, string LanguageId)
    {
        GetBasketItems = new List<Kartris.BasketItem>();        // Initial value
        Kartris.BasketItem objItem;
        DataTable tblBasketValues;
        int numCurrencyID; // , numLanguageId, numSessionId
        List<Kartris.BasketItem> BasketItems = new List<Kartris.BasketItem>();
        ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();

        short SESS_CurrencyID;
        if (numCurrencyID > 0)
            SESS_CurrencyID = numCurrencyID;
        else
            SESS_CurrencyID = System.Web.HttpContext.Current.Session["CUR_ID"];

        // numLanguageID = Current.Session("LANG")
        // numSessionID = Current.Session("SessionID")

        // Load datatable from database
        tblBasketValues = _BasketValuesAdptr.GetItems(LanguageId, SessionId);

        try
        {
            foreach (DataRow drwBasketValues in tblBasketValues.Rows)
            {
                objItem = new Kartris.BasketItem();
                objItem.ProductID = drwBasketValues("ProductID");
                objItem.ProductType = drwBasketValues("ProductType");
                objItem.TaxRate1 = FixNullFromDB(drwBasketValues("TaxRate"));
                objItem.TaxRate2 = FixNullFromDB(drwBasketValues("TaxRate2"));
                objItem.TaxRateItem = FixNullFromDB(drwBasketValues("TaxRate"));
                objItem.Weight = FixNullFromDB(drwBasketValues("Weight"));
                objItem.RRP = FixNullFromDB(drwBasketValues("RRP"));
                objItem.ProductName = drwBasketValues("ProductName") + "";
                objItem.ID = drwBasketValues("BV_ID");
                objItem.VersionBaseID = drwBasketValues("V_ID");
                objItem.VersionID = drwBasketValues("BV_VersionID");
                objItem.VersionName = drwBasketValues("VersionName") + "";
                objItem.VersionCode = drwBasketValues("CodeNumber") + "";
                objItem.CodeNumber = drwBasketValues("CodeNumber") + "";
                objItem.Price = FixNullFromDB(drwBasketValues("Price"));
                objItem.Quantity = FixNullFromDB(drwBasketValues("Quantity"));
                objItem.StockQty = FixNullFromDB(drwBasketValues("V_Quantity"));
                objItem.QtyWarnLevel = FixNullFromDB(drwBasketValues("QtyWarnLevel"));
                objItem.DownloadType = drwBasketValues("V_DownloadType") + "";
                objItem.OptionPrice = Math.Round(System.Convert.ToDouble(FixNullFromDB(drwBasketValues("OptionsPrice"))), CurrencyRoundNumber);
                objItem.CategoryIDs = GetCategoryIDs(objItem.ProductID);
                objItem.PromoQty = objItem.Quantity;
                objItem.VersionType = FixNullFromDB(drwBasketValues("VersionType"));

                // Added v2.9010 - lets us exclude particular products
                // from the customer discount 
                objItem.ExcludeFromCustomerDiscount = System.Convert.ToBoolean(objObjectConfigBLL.GetValue("K:product.excustomerdiscount", drwBasketValues("ProductID")));

                // We can tell if this is an combinations product
                if (objItem.VersionType == "c")
                {
                    objItem.HasCombinations = true;
                    objItem.DownloadType = FixNullFromDB(drwBasketValues("BaseVersion_DownloadType"));

                    // Normally, combinations products will use the price derived from the base version
                    // and any options that are selected, just like a regular options product. However,
                    // by setting the usecombination object config setting for the product to 'on', you
                    // can specify pricing individually for each combination.
                    if (objObjectConfigBLL.GetValue("K:product.usecombinationprice", objItem.ProductID) == "1")
                    {
                        objItem.Price = FixNullFromDB(drwBasketValues("CombinationPrice"));
                        objItem.Price = Math.Round(System.Convert.ToDouble(CurrenciesBLL.ConvertCurrency(SESS_CurrencyID, objItem.Price)), CurrencyRoundNumber);
                        objItem.OptionPrice = Math.Round(0, CurrencyRoundNumber);
                    }

                    // We determine whether stock tracking is on or not from the
                    // base version rather than the actual combination in the
                    // basket
                    VersionsBLL objVersionsBLL = new VersionsBLL();
                    if (objVersionsBLL.IsStockTrackingInBase(objItem.ProductID))
                    {
                    }
                    else
                        objItem.QtyWarnLevel = 0;
                }
                else if (objItem.ProductType == "o")
                    objItem.OptionLink = "";

                objItem.OptionText = GetOptionText(LanguageId, objItem.ID, ref objItem.OptionLink);
                objItem.CustomText = drwBasketValues("CustomText") + "";
                objItem.CustomType = drwBasketValues("V_CustomizationType") + "";
                objItem.CustomDesc = drwBasketValues("V_CustomizationDesc") + "";
                objItem.CustomCost = Math.Round(System.Convert.ToDouble(CurrenciesBLL.ConvertCurrency(SESS_CurrencyID, FixNullFromDB(drwBasketValues("V_CustomizationCost")))), CurrencyRoundNumber);
                objItem.Price = Math.Round(System.Convert.ToDouble(CurrenciesBLL.ConvertCurrency(SESS_CurrencyID, objItem.Price)), CurrencyRoundNumber);
                objItem.TableText = "";

                // Handle the price differently if basket item is from a custom product
                string strCustomControlName = objObjectConfigBLL.GetValue("K:product.customcontrolname", objItem.ProductID);
                if (!string.IsNullOrEmpty(strCustomControlName))
                {
                    try
                    {
                        string strParameterValues = FixNullFromDB(drwBasketValues("CustomText"));
                        // Split the custom text field
                        string[] arrParameters = Strings.Split(strParameterValues, "|||");

                        // arrParameters(0) contains the comma separated list of the custom control's parameters values
                        // we don't use this value when loading the basket, this is only needed when validating the price in the checkout

                        // arrParameters(1) contains the custom description of the item
                        if (!string.IsNullOrEmpty(arrParameters[1]))
                            objItem.VersionName = arrParameters[1];

                        // arrParameters(2) contains the custom price
                        objItem.Price = arrParameters[2];

                        // just set the option price to 0 just to be safe
                        objItem.OptionPrice = Math.Round(0, CurrencyRoundNumber);
                    }
                    catch (Exception ex)
                    {
                        // Failed to retrieve custom price, ignore this basket item
                        objItem.Quantity = 0;
                    }
                }

                // there must be something wrong if quantity is 0 so don't add this item to the basketitems array
                if (objItem.Quantity > 0)
                    BasketItems.Add(objItem);
            }
        }
        catch (Exception ex)
        {
            SqlConnection.ClearPool(_BasketValuesAdptr.Connection);
            CkartrisFormatErrors.LogError("BasketBLL.GetBasketItems - " + ex.Message);
        }

        return BasketItems;
    }

    /// <summary>
    ///     ''' Get a list of product options that are inside a basket
    ///     ''' </summary>
    ///     ''' <param name="pBasketValueID">Basket row to get options from</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static string GetMiniBasketOptions(long pBasketValueID)
    {
        DataTable tblBasketOptions = new DataTable();
        tblBasketOptions = _BasketOptionsAdptr.GetBasketOptionsByBasketValueID(pBasketValueID);

        string strBasketOptions = null;
        foreach (DataRow drwOptions in tblBasketOptions.Rows)
            strBasketOptions += System.Convert.ToString(drwOptions("BSKTOPT_OptionID")) + ",";
        if (!strBasketOptions == null)
            strBasketOptions = strBasketOptions.TrimEnd(",");

        return strBasketOptions;
    }

    /// <summary>
    ///     ''' Add a product option to a basket
    ///     ''' </summary>
    ///     ''' <param name="pBasketValueID">Basket row to add option to</param>
    ///     ''' <param name="pOptionID">Option to add</param>
    ///     ''' <remarks></remarks>
    private static void AddBasketOptionValues(long pBasketValueID, int pOptionID)
    {
        _BasketOptionsAdptr.Insert(pBasketValueID, pOptionID);
    }

    /// <summary>
    ///     ''' Delete all items in a basket 
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public new static void DeleteBasket()
    {
        DeleteBasket(System.Web.HttpContext.Current.Session["SessionID"]);
    }

    /// <summary>
    ///     ''' Delete all items in a basket
    ///     ''' </summary>
    ///     ''' <param name="numParentID">Reference to the basket parent (same for entire basket)</param>
    ///     ''' <remarks></remarks>
    public new static void DeleteBasket(long numParentID)
    {
        _BasketValuesAdptr.DeleteBasketItems(1, numParentID);
    }

    /// <summary>
    ///     ''' Delete a single basket item
    ///     ''' </summary>
    ///     ''' <param name="numBasketID">reference to the item in the basket we wish to delete</param>
    ///     ''' <remarks></remarks>
    public static void DeleteBasketItems(long numBasketID)
    {
        _BasketValuesAdptr.DeleteBasketItems(2, numBasketID);
    }

    /// <summary>
    ///     ''' Replace the quantity for a single row in the basket
    ///     ''' </summary>
    ///     ''' <param name="intBasketID">Basket row to affect (BasketValueID)</param>
    ///     ''' <param name="numQuantity">Quantity that it should be updated to.</param>
    ///     ''' <remarks></remarks>
    public static void UpdateQuantity(int intBasketID, float numQuantity)
    {
        if (numQuantity > 0)
            _BasketValuesAdptr.UpdateQuantity(intBasketID, numQuantity);
    }

    /// <summary>
    ///     ''' Get the customer percentage discount
    ///     ''' </summary>
    ///     ''' <param name="numCustomerID"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks>how much discount we give to the given customer as a percentage</remarks>
    public static double GetCustomerDiscount(int numCustomerID)
    {
        CustomersDataTable tblCustomers;
        double numDiscount = 0;

        tblCustomers = _CustomersAdptr.GetCustomerDiscount(numCustomerID);
        if (tblCustomers.Rows.Count > 0)
            numDiscount = tblCustomers.Rows(0).Item("Discount");

        return numDiscount;
    }

    /// <summary>
    ///     ''' Get the price for the given customer for a product version.
    ///     ''' </summary>
    ///     ''' <param name="intCustomerID">Customer ID</param>
    ///     ''' <param name="numVersionID">Product Version ID</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static double GetCustomerGroupPriceForVersion(int intCustomerID, long numVersionID)
    {
        CustomersDataTable tblCustomers;
        double numPrice = 0;

        tblCustomers = _CustomersAdptr.GetCustomerGroupPrice(intCustomerID, numVersionID);
        if (tblCustomers.Rows.Count > 0)
            numPrice = tblCustomers.Rows(0).Item("CGP_Price");

        return numPrice;
    }

    /// <summary>
    ///     ''' Get the quantity discount for this product version
    ///     ''' </summary>
    ///     ''' <param name="numVersionID">Product version we want to get discount for</param>
    ///     ''' <param name="numQuantity">Quantity being purchased</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks>Some versions have a discount (e.g. purchase more than 100 and price is reduced 25%). Retrieve this discount here.</remarks>
    public static double GetQuantityDiscount(long numVersionID, double numQuantity)
    {
        CustomersDataTable tblCustomers;
        double numDiscount = 0;

        tblCustomers = _CustomersAdptr.GetQtyDiscount(numVersionID, numQuantity);
        if (tblCustomers.Rows.Count > 0)
            numDiscount = tblCustomers.Rows(0).Item("QD_Price");

        return numDiscount;
    }

    /// <summary>
    ///     ''' Get customer information
    ///     ''' </summary>
    ///     ''' <param name="numUserID">Customer ID</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable GetCustomerData(long numUserID)
    {
        DataTable tblCustomers = new DataTable();
        tblCustomers = _CustomersAdptr.GetCustomer(numUserID, "", "");
        return tblCustomers;
    }

    /// <summary>
    ///     ''' Save custom text against an item in the basket. 
    ///     ''' </summary>
    ///     ''' <param name="numBasketItemID">The basket item (row) that we wish to save the text against</param>
    ///     ''' <param name="strCustomText">The custom text to be saved</param>
    ///     ''' <remarks></remarks>
    public static void SaveCustomText(long numBasketItemID, string strCustomText)
    {
        _BasketValuesAdptr.SaveCustomText(numBasketItemID, strCustomText);
    }

    /// <summary>
    ///     ''' Save the basket. This takes an active basket and stores it for retrieval at a later date. This is a customer triggered event
    ///     ''' </summary>
    ///     ''' <param name="numCustomerID">The customer that this basket is stored for</param>
    ///     ''' <param name="strBasketName">Name of basket (to help customer remember it later)</param>
    ///     ''' <param name="numBasketID">Basket identifier</param>
    ///     ''' <remarks>Can only be used for a logged in customer (or else there is no customer ID)</remarks>
    public static void SaveBasket(long numCustomerID, string strBasketName, long numBasketID)
    {
        _CustomersAdptr.SaveBasket(numCustomerID, strBasketName, numBasketID, CkartrisDisplayFunctions.NowOffset);
    }

    /// <summary>
    ///     ''' Retrieve a basket that was previously saved using OldBasketBLL.SaveBasket
    ///     ''' </summary>
    ///     ''' <param name="numUserID">User or Customer ID</param>
    ///     ''' <param name="PageIndex">Page to display (paginated output)</param>
    ///     ''' <param name="PageSize">Number of rows to show per page (paginated output)</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable GetSavedBasket(long numUserID, int PageIndex = -1, int PageSize = -1)
    {
        DataTable tblSavedBaskets = new DataTable();
        tblSavedBaskets = _CustomersAdptr.GetSavedBasket(1, numUserID, PageIndex, PageIndex + PageSize - 1);
        return tblSavedBaskets;
    }

    /// <summary>
    ///     ''' Gets the total value of all saved baskets for a single customer. 
    ///     ''' </summary>
    ///     ''' <param name="numUserID">The customer we want to retrieve the total for.</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static int GetSavedBasketTotal(long numUserID)
    {
        DataTable tblSavedBaskets = new DataTable();
        int numTotalBasket = 0;

        tblSavedBaskets = _CustomersAdptr.GetSavedBasket(0, numUserID, 0, 0);
        if (tblSavedBaskets.Rows.Count > 0)
            numTotalBasket = tblSavedBaskets.Rows(0).Item("TotalRec");

        tblSavedBaskets.Dispose();
        return numTotalBasket;
    }

    /// <summary>
    ///     ''' Delete a single saved basket from the database
    ///     ''' </summary>
    ///     ''' <param name="numBasketID">The basket to delete</param>
    ///     ''' <remarks></remarks>
    public static void DeleteSavedBasket(long numBasketID)
    {
        _CustomersAdptr.DeleteSavedBasket(numBasketID);
    }

    /// <summary>
    ///     ''' Load a saved basket into an active basket
    ///     ''' </summary>
    ///     ''' <param name="numBasketSavedID">The saved basket ID to load</param>
    ///     ''' <param name="numBasketID">The target active basket to put the items into</param>
    ///     ''' <remarks></remarks>
    public static void LoadSavedBasket(long numBasketSavedID, long numBasketID)
    {
        _CustomersAdptr.LoadSavedBasket(numBasketSavedID, numBasketID, CkartrisDisplayFunctions.NowOffset);
    }

    /// <summary>
    ///     ''' Load a saved AUTOSAVE basket
    ///     ''' </summary>
    ///     ''' <param name="numCustomerID">The saved basket ID to load</param>
    ///     ''' <param name="numBasketID">The target active basket to put the items into</param>
    ///     ''' <remarks></remarks>
    public static void LoadAutosaveBasket(long numCustomerID, long numBasketID)
    {
        _CustomersAdptr.LoadAutosaveBasket(numCustomerID, numBasketID, CkartrisDisplayFunctions.NowOffset);
    }

    /// <summary>
    ///     ''' Save a basket to a wishlist 
    ///     ''' </summary>
    ///     ''' <param name="numWishlistsID">The wishlist to be updated. Leave as zero to create a new wishlist</param>
    ///     ''' <param name="numBasketID">The basket that contains the items that should be put in the wishlist. Ignored if updating an existing wishlist</param>
    ///     ''' <param name="numUserID">Customer or User ID</param>
    ///     ''' <param name="strName">Name of wishlist (used to help customer identify wishlist)</param>
    ///     ''' <param name="strPublicPassword">Public password to allow other users to open wishlist</param>
    ///     ''' <param name="strMessage">Text message to users other than the customer that may open wishlist. For example, to purchase items from the wishlist for the customer.</param>
    ///     ''' <remarks>If the wishlist ID is zero a new wishlist will be generated. If a wishlist ID is supplied the name and comments will be replaced, but not the actual basket rows</remarks>
    public static void SaveWishLists(long numWishlistsID, long numBasketID, int numUserID, string strName, string strPublicPassword, string strMessage)
    {
        _CustomersAdptr.SaveWishList(numWishlistsID, numBasketID, numUserID, strName, strPublicPassword, strMessage, CkartrisDisplayFunctions.NowOffset);
    }

    /// <summary>
    ///     ''' Return the total value for all items in all wishlists for a given customer.
    ///     ''' </summary>
    ///     ''' <param name="numUserId">Customer that we want to calculate for.</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static int GetWishListTotal(long numUserId)
    {
        DataTable tblWishList = new DataTable();
        int numTotalWishList = 0;

        tblWishList = _CustomersAdptr.GetWishList(0, numUserId, 0, 0, 0, "", "", 0);
        if (tblWishList.Rows.Count > 0)
            numTotalWishList = tblWishList.Rows(0).Item("TotalRec");

        tblWishList.Dispose();
        return numTotalWishList;
    }

    /// <summary>
    ///     ''' Get a list of wishlists for a given customer.
    ///     ''' </summary>
    ///     ''' <param name="numUserId">The customer we want to find wishlists for</param>
    ///     ''' <param name="PageIndex">Page to display (paginated output)</param>
    ///     ''' <param name="PageSize">Number of rows to show per page (paginated output)</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable GetWishLists(long numUserId, int PageIndex = -1, int PageSize = -1)
    {
        DataTable tblWishList = new DataTable();
        tblWishList = _CustomersAdptr.GetWishList(1, numUserId, PageIndex, PageIndex + PageSize - 1, 0, "", "", 0);
        return tblWishList;
    }

    /// <summary>
    ///     ''' Get a single wishlist
    ///     ''' </summary>
    ///     ''' <param name="numWishlistID">Identifier for wishlist we wish to retrieve</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable GetWishListByID(long numWishlistID)
    {
        DataTable tblWishList;
        tblWishList = _CustomersAdptr.GetWishList(-1, 0, 0, 0, numWishlistID, "", "", 0);
        return tblWishList;
    }

    /// <summary>
    ///     ''' Get a single wishlist 
    ///     ''' </summary>
    ///     ''' <param name="numCustomerID">Identifier for customer that created wishlist</param>
    ///     ''' <param name="numWishlistID">Identifier for wishlist.</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks>Consider using GetWishListById for simplicity and speedier execution</remarks>
    public static DataTable GetCustomerWishList(long numCustomerID, long numWishlistID)
    {
        DataTable tblWishList = new DataTable();
        tblWishList = _CustomersAdptr.GetWishList(2, numCustomerID, 0, 0, numWishlistID, "", "", 0);
        return tblWishList;
    }

    /// <summary>
    ///     ''' Retrieve wishlist using login details
    ///     ''' </summary>
    ///     ''' <param name="strEmail">Email account that wishlist is attached to</param>
    ///     ''' <param name="strPassword">Password required to access wishlist.</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable GetWishListLogin(string strEmail, string strPassword)
    {
        DataTable tblWishList = new DataTable();
        tblWishList = _CustomersAdptr.GetWishList(3, 0, 0, 0, 0, strEmail, strPassword, 0);
        return tblWishList;
    }

    /// <summary>
    ///     ''' Get the remaining items from a wishlist
    ///     ''' </summary>
    ///     ''' <param name="numCustomerID">The customer that owns the wishlist</param>
    ///     ''' <param name="numWishlistID">The required wishlist</param>
    ///     ''' <param name="numLanguage">Display language</param>
    ///     ''' <returns>Returns items that have not been purchased from wishlist. Does not return items that have already been purchased</returns>
    ///     ''' <remarks></remarks>
    public static DataTable GetRequiredWishlist(long numCustomerID, long numWishlistID, short numLanguage)
    {
        DataTable tblWishList = new DataTable();
        tblWishList = _CustomersAdptr.GetWishList(4, numCustomerID, 0, 0, numWishlistID, "", "", numLanguage);
        return tblWishList;
    }

    /// <summary>
    ///     ''' Delete a wishlist
    ///     ''' </summary>
    ///     ''' <param name="numWishListsID">The wishlist to be deleted.</param>
    ///     ''' <remarks></remarks>
    public static void DeleteWishLists(long numWishListsID)
    {
        _CustomersAdptr.DeleteWishList(numWishListsID);
    }

    /// <summary>
    ///     ''' Transfer a stored wishlist into an active basket
    ///     ''' </summary>
    ///     ''' <param name="numWishlistsID">The wishlist to be loaded</param>
    ///     ''' <param name="numBasketID">The basket to transfer the wishlist into</param>
    ///     ''' <remarks></remarks>
    public static void LoadWishlists(long numWishlistsID, long numBasketID)
    {
        _CustomersAdptr.LoadWishlist(numWishlistsID, numBasketID, CkartrisDisplayFunctions.NowOffset);
    }

    /// <summary>
    ///     ''' Calculate the handling charge for the order inside the current basket and set which
    ///     ''' destination (shipping or billing) should be used for tax
    ///     ''' </summary>
    ///     ''' <param name="numShippingCountryID">Country that the items will be shipped to.</param>
    ///     ''' <remarks></remarks>
    public static void CalculateOrderHandlingCharge(ref Kartris.Basket Basket, int numShippingCountryID)
    {
        double numOrderHandlingPriceValue;
        double numOrderHandlingTaxBand1 = 0;
        double numOrderHandlingTaxBand2 = 0;

        // Calculate tax based on billing address
        if (KartSettingsManager.GetKartConfig("general.tax.shipping") == "billing")
            numShippingCountryID = System.Web.HttpContext.Current.Session["_ShippingDestinationID"];

        if (numShippingCountryID == 0)
            return;

        numOrderHandlingPriceValue = KartSettingsManager.GetKartConfig("frontend.checkout.orderhandlingcharge");

        short SESS_CurrencyID;

        {
            var withBlock = Basket;
            if (withBlock.CurrencyID > 0)
                SESS_CurrencyID = withBlock.CurrencyID;
            else
                SESS_CurrencyID = System.Web.HttpContext.Current.Session["CUR_ID"];

            if (SESS_CurrencyID != CurrenciesBLL.GetDefaultCurrency)
                numOrderHandlingPriceValue = CurrenciesBLL.ConvertCurrency(SESS_CurrencyID, numOrderHandlingPriceValue);

            Country DestinationCountry = Country.Get(numShippingCountryID);

            try
            {
                numOrderHandlingTaxBand1 = KartSettingsManager.GetKartConfig("frontend.checkout.orderhandlingchargetaxband");
                numOrderHandlingTaxBand2 = KartSettingsManager.GetKartConfig("frontend.checkout.orderhandlingchargetaxband2");
            }
            catch (Exception ex)
            {
            }

            if (ConfigurationManager.AppSettings("TaxRegime").ToLower == "us" | ConfigurationManager.AppSettings("TaxRegime").ToLower == "simple")
            {
                withBlock.OrderHandlingPrice.TaxRate = DestinationCountry.ComputedTaxRate;
                withBlock.D_Tax = DestinationCountry.ComputedTaxRate;
            }
            else
            {
                if (DestinationCountry.D_Tax)
                    withBlock.D_Tax = 1;
                else
                    withBlock.D_Tax = 0;

                try
                {
                    if (System.Web.HttpContext.Current.Session["blnEUVATValidated"] != null)
                    {
                        if (System.Convert.ToBoolean(System.Web.HttpContext.Current.Session["blnEUVATValidated"]))
                        {
                            DestinationCountry.D_Tax = false;
                            withBlock.D_Tax = 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                }

                withBlock.OrderHandlingPrice.TaxRate = TaxRegime.CalculateTaxRate(TaxBLL.GetTaxRate(numOrderHandlingTaxBand1), TaxBLL.GetTaxRate(numOrderHandlingTaxBand2), DestinationCountry.TaxRate1, DestinationCountry.TaxRate2, DestinationCountry.TaxExtra);
            }

            if (withBlock.PricesIncTax)
            {
                withBlock.OrderHandlingPrice.ExTax = Math.Round(numOrderHandlingPriceValue * (1 / (double)(1 + withBlock.OrderHandlingPrice.TaxRate)), CurrencyRoundNumber);

                // If tax is off, then inc tax can be set to just the ex tax
                if (DestinationCountry.D_Tax)
                    // Set the inctax order handling values
                    withBlock.OrderHandlingPrice.IncTax = Math.Round(numOrderHandlingPriceValue, CurrencyRoundNumber);
                else
                {
                    withBlock.OrderHandlingPrice.IncTax = withBlock.OrderHandlingPrice.ExTax;
                    withBlock.OrderHandlingPrice.TaxRate = 0;
                }
            }
            else
            {
                // Set the extax order handling values
                withBlock.OrderHandlingPrice.ExTax = numOrderHandlingPriceValue;

                // Tax rate for order handling
                // Modified to set order handling to zero if 
                // frontend.checkout.orderhandlingchargetaxband is 1 or 0
                // (should really be 1, as this is the likely db ID that
                // corresponds to a zero rate, but support zero too because
                // lots of people set it to that by mistake).
                if (DestinationCountry.D_Tax & (numOrderHandlingTaxBand1 > 1))
                    withBlock.OrderHandlingPrice.IncTax = Math.Round(withBlock.OrderHandlingPrice.ExTax * (1 + withBlock.OrderHandlingPrice.TaxRate), CurrencyRoundNumber);
                else
                {
                    withBlock.OrderHandlingPrice.IncTax = numOrderHandlingPriceValue;
                    withBlock.OrderHandlingPrice.TaxRate = 0;
                }
            }
        }
    }

    /// <summary>
    ///     ''' Get a list of orders for the given customer
    ///     ''' </summary>
    ///     ''' <param name="numUserId">The customer to get the orders for</param>
    ///     ''' <param name="PageIndex">The page number to get (paginated output)</param>
    ///     ''' <param name="PageSize">The size of the page to return (paginated output)</param>
    ///     ''' <returns>Complete and incomplete orders.</returns>
    ///     ''' <remarks></remarks>
    public static DataTable GetCustomerOrders(long numUserId, int PageIndex = -1, int PageSize = -1)
    {
        DataTable tblCustomerOrders = new DataTable();
        tblCustomerOrders = _CustomersAdptr.GetCustomerOrders(1, numUserId, PageIndex, PageIndex + PageSize - 1);
        return tblCustomerOrders;
    }


    /// <summary>
    ///     ''' Get total value of all orders for a given customer
    ///     ''' </summary>
    ///     ''' <param name="numUserId">The customer we want to totalise data for</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static int GetCustomerOrdersTotal(long numUserId)
    {
        DataTable tblCustomerOrders = new DataTable();
        int numTotalOrders = 0;

        tblCustomerOrders = _CustomersAdptr.GetCustomerOrders(0, numUserId, 0, 0);
        if (tblCustomerOrders.Rows.Count > 0)
            numTotalOrders = tblCustomerOrders.Rows(0).Item("TotalRec");

        tblCustomerOrders.Dispose();
        return numTotalOrders;
    }

    /// <summary>
    ///     ''' Get all of the details related to a single order.
    ///     ''' </summary>
    ///     ''' <param name="numOrderID">The order we want to get data for</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public DataTable GetCustomerOrderDetails(int numOrderID)
    {
        DataTable tblCustomerOrders = new DataTable();
        tblCustomerOrders = _CustomersAdptr.GetOrderDetails(numOrderID);
        return tblCustomerOrders;
    }

    /// <summary>
    ///     ''' Get a list of all downloadable products that a particular customer has purchased and are available for download.
    ///     ''' </summary>
    ///     ''' <param name="numUserID">The customer that we want to get the orders for</param>
    ///     ''' <returns>Does not return products where the download date has expired</returns>
    ///     ''' <remarks></remarks>
    public static DataTable GetDownloadableProducts(int numUserID)
    {
        DataTable tblDownloads = new DataTable();
        bool O_Shipped = LCase(GetKartConfig("frontend.downloads.instant")) == "n";
        int intDaysAvailable = 0;
        try
        {
            intDaysAvailable = System.Convert.ToInt32(LCase(GetKartConfig("frontend.downloads.daysavailable")));
        }
        catch (Exception ex)
        {
        }

        DateTime datAvailableUpTo;
        if (intDaysAvailable > 0)
            datAvailableUpTo = DateTime.Today.AddDays(-intDaysAvailable);
        else
            datAvailableUpTo = DateTime.Today.AddYears(-100);
        tblDownloads = _CustomersAdptr.GetDownloadableProducts(numUserID, O_Shipped, datAvailableUpTo);
        return tblDownloads;
    }

    /// <summary>
    ///     ''' Return a single invoice
    ///     ''' </summary>
    ///     ''' <param name="numOrderID">The order reference number taken from the order table (non visible)</param>
    ///     ''' <param name="numUserID">Reference to the selected customer</param>
    ///     ''' <param name="numType">Defines what data should be returned. A value of 0 returns only summary and address data; any other value returns the row level detail</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public DataTable GetCustomerInvoice(int numOrderID, int numUserID, int numType = 0)
    {
        DataTable tblInvoice = new DataTable();
        tblInvoice = _CustomersAdptr.GetInvoice(numOrderID, numUserID, numType);
        return tblInvoice;
    }

    /// <summary>
    ///     ''' Generate a random string
    ///     ''' </summary>
    ///     ''' <param name="numLength"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks>used for password generation</remarks>
    public static string GetRandomString(int numLength)
    {
        string strRandomString;
        int numRandomNumber;

        VBMath.Randomize();
        strRandomString = "";

        while (Strings.Len(strRandomString) < numLength)
        {
            numRandomNumber = Conversion.Int(VBMath.Rnd(1) * 36) + 1;
            if (numRandomNumber < 11)
                // If it's less than 11 then we'll do a number
                strRandomString = strRandomString + Strings.Chr(numRandomNumber + 47);
            else
                // Otherwise we'll do a letter; + 86 because 96 (min being 97, 'a') - 10 (the first 10 was for the number)
                strRandomString = strRandomString + Strings.Chr(numRandomNumber + 86);
        }

        // Zero and 'o' and '1' and 'I' are easily confused...
        // So we replace any of these with alternatives
        // To ensure best randomness, replace the numbers
        // with alternative letters and letters
        // with alternative numbers

        strRandomString = Strings.Replace(strRandomString, "0", "X");
        strRandomString = Strings.Replace(strRandomString, "1", "Y");
        strRandomString = Strings.Replace(strRandomString, "O", "4");
        strRandomString = Strings.Replace(strRandomString, "I", "9");

        return strRandomString;
    }

    /// <summary>
    ///     ''' Get all information related to a coupon.
    ///     ''' </summary>
    ///     ''' <param name="strCouponName">The coupon code.</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public DataTable GetCouponData(string strCouponName)
    {
        DataTable tblCoupon = new DataTable();
        tblCoupon = _CouponsAdptr.GetCouponCode(strCouponName);
        return tblCoupon;
    }

    /// <summary>
    ///     ''' Get the type for a version customization
    ///     ''' </summary>
    ///     ''' <param name="numVersionID">The version to get customisation data for</param>
    ///     ''' <returns>a single character (e.g. 't' = text)</returns>
    ///     ''' <remarks>Customisation data is applied to the product in the version table. This data is not customer / order specific; this data is product version specific</remarks>
    public static string GetVersionCustomType(long numVersionID)
    {
        string strCustomType = "";
        CustomersDataTable tblCoupon;

        tblCoupon = _CustomersAdptr.GetCustomization(numVersionID);

        if (tblCoupon.Rows.Count > 0)
            strCustomType = tblCoupon.Rows(0).Item("V_CustomizationType") + "";

        return strCustomType;
    }

    /// <summary>
    ///     ''' Get customisation data for a product version
    ///     ''' </summary>
    ///     ''' <param name="numVersionID">The version to get customisation data for</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks>Customisation data is applied to the product in the version table. This data is not customer / order specific; this data is product version specific</remarks>
    public static DataTable GetCustomization(long numVersionID)
    {
        DataTable tblCustomization;
        tblCustomization = _CustomersAdptr.GetCustomization(numVersionID);
        return tblCustomization;
    }

    /// <summary>
    ///     ''' Get all Version IDs for the current basket.
    ///     ''' </summary>
    ///     ''' <returns>a comma delimited string.</returns>
    ///     ''' <remarks></remarks>
    private static string GetBasketItemVersionIDs(ref List<Kartris.BasketItem> BasketItems)
    {
        string strIDs = "";
        foreach (Kartris.BasketItem Item in BasketItems)
            strIDs = strIDs + Item.VersionID + ",";
        if (strIDs != "")
            strIDs = Strings.Left(strIDs, Strings.Len(strIDs) - 1);
        return strIDs;
    }

    /// <summary>
    ///     ''' Get all product IDs for the current basket
    ///     ''' </summary>
    ///     ''' <returns>a comma delimited string</returns>
    ///     ''' <remarks></remarks>
    private static string GetBasketItemProductIDs(ref List<Kartris.BasketItem> BasketItems)
    {
        string strIDs = "";
        foreach (Kartris.BasketItem Item in BasketItems)
            strIDs = strIDs + Item.ProductID + ",";
        strIDs = Interaction.IIf(strIDs != "", Strings.Left(strIDs, Strings.Len(strIDs) - 1), strIDs);
        return strIDs;
    }

    /// <summary>
    ///     ''' Get all category IDs in the current basket
    ///     ''' </summary>
    ///     ''' <returns>a comma delimited string</returns>
    ///     ''' <remarks></remarks>
    private static string GetBasketItemCategoryIDs(ref List<Kartris.BasketItem> BasketItems)
    {
        string strIDs = "";
        foreach (Kartris.BasketItem Item in BasketItems)
            strIDs = strIDs + Item.CategoryIDs + ",";
        strIDs = Interaction.IIf(strIDs != "", Strings.Left(strIDs, Strings.Len(strIDs) - 1), strIDs);
        return strIDs;
    }

    /// <summary>
    ///     ''' Return the basket items that matches the given version ID
    ///     ''' </summary>
    ///     ''' <param name="numVersionID">The product version ID we are looking for</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks>acts upon the current basket</remarks>
    private static Kartris.BasketItem GetBasketItemByVersionID(ref List<Kartris.BasketItem> BasketItems, int numVersionID)
    {
        GetBasketItemByVersionID = null/* TODO Change to default(_) if this is not a reference type */;
        // Removed blnReload logic as not used.
        foreach (Kartris.BasketItem Item in BasketItems)
        {
            // Item found
            if (Item.VersionID == numVersionID)
                return Item;
        }
    }

    /// <summary>
    ///     ''' Find the highest value of a given product within the current basket. 
    ///     ''' </summary>
    ///     ''' <param name="numProductID"></param>
    ///     ''' <returns>finds highest value (including tax) from all versions of a single product in the basket</returns>
    ///     ''' <remarks></remarks>
    private static int GetItemMaxProductValue(ref List<Kartris.BasketItem> BasketItems, int numProductID)
    {
        Kartris.BasketItem objItem = new Kartris.BasketItem(), tmpItem = new Kartris.BasketItem();
        int index = -1;

        // Search through the items in the basket to find the product we are looking for
        for (int i = 0; i <= BasketItems.Count - 1; i++)
        {
            objItem = BasketItems[i];
            if (objItem.ProductID == numProductID & objItem.PromoQty > 0)
            {
                index = Interaction.IIf(index == -1, i, index);
                tmpItem = BasketItems[index];
                if (objItem.IncTax > tmpItem.IncTax)
                    index = i;
            }
        }

        return index;
    }

    /// <summary>
    ///     ''' Find the ordinal position of the lowest value of a given product within the current basket. 
    ///     ''' </summary>
    ///     ''' <param name="numProductID">Product to search for</param>
    ///     ''' <param name="strVersionIDArray">comma delimited string of versions that should be excluded from our search.</param>
    ///     ''' <returns>finds lowest value (including tax) from all versions of a single product in the basket and returns its ordinal position within the collection</returns>
    ///     ''' <remarks></remarks>
    private static int GetItemMinProductValue(ref List<Kartris.BasketItem> BasketItems, int numProductID, string strVersionIDArray = "")
    {
        Kartris.BasketItem objItem = new Kartris.BasketItem(), tmpItem = new Kartris.BasketItem();
        int index = -1;
        string[] arrVersionIDsToExclude = null;


        if (strVersionIDArray != "")
            arrVersionIDsToExclude = Strings.Split(strVersionIDArray, ",");
        bool blnSkipVersion = false;

        for (int i = 0; i <= BasketItems.Count - 1; i++)
        {
            objItem = BasketItems[i];
            if (arrVersionIDsToExclude != null)
            {
                blnSkipVersion = false;
                for (int x = 0; x <= arrVersionIDsToExclude.Count() - 1; x++)
                {
                    if (arrVersionIDsToExclude[x] == objItem.VersionID)
                    {
                        blnSkipVersion = true;
                        continue;
                    }
                }
                if (blnSkipVersion)
                    continue;
            }
            // If MinVersionID > 0 AndAlso MinVersionID = objItem.VersionID Then Continue For
            if (objItem.ProductID == numProductID & objItem.PromoQty > 0)
            {
                index = Interaction.IIf(index == -1, i, index);
                tmpItem = BasketItems[index];
                if (objItem.IncTax < tmpItem.IncTax)
                    index = i;
            }
        }

        return index;
    }

    /// <summary>
    ///     ''' Find the ordinal position of the highest value item in the basket that belongs to a given product category
    ///     ''' </summary>
    ///     ''' <param name="numProductID">Category that we are searching within (not Product ID strangely)</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    private static int GetItemMaxCategoryValue(ref List<Kartris.BasketItem> BasketItems, int numProductID)
    {
        Kartris.BasketItem objItem = new Kartris.BasketItem(), tmpItem = new Kartris.BasketItem();
        int index = -1;

        for (int i = 0; i <= BasketItems.Count - 1; i++)
        {
            objItem = BasketItems[i];
            if (InStr(objItem.CategoryIDs, numProductID.ToString()) > 0)
            {
                index = Interaction.IIf(index == -1, i, index);
                tmpItem = BasketItems[index];
                if (objItem.IncTax > tmpItem.IncTax)
                    index = i;
            }
        }

        return index;
    }

    /// <summary>
    ///     ''' Find the ordinal position of the lowest value item in the basket that belongs to a given product category
    ///     ''' </summary>
    ///     ''' <param name="numProductID">The category to search within (not the product ID)</param>
    ///     ''' <param name="strVersionIDArray">comma delimited string of product versions to exclude from this search</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    private static int GetItemMinCategoryValue(ref List<Kartris.BasketItem> BasketItems, int numProductID, string strVersionIDArray = "")
    {
        Kartris.BasketItem objItem = new Kartris.BasketItem(), tmpItem = new Kartris.BasketItem();
        int index = -1;
        string[] arrVersionIDsToExclude = null;

        if (strVersionIDArray != "")
            arrVersionIDsToExclude = Strings.Split(strVersionIDArray, ",");
        bool blnSkipVersion = false;

        for (int i = 0; i <= BasketItems.Count - 1; i++)
        {
            objItem = BasketItems[i];
            if (arrVersionIDsToExclude != null)
            {
                blnSkipVersion = false;
                for (int x = 0; x <= arrVersionIDsToExclude.Count() - 1; x++)
                {
                    if (arrVersionIDsToExclude[x] == objItem.VersionID)
                    {
                        blnSkipVersion = true;
                        continue;
                    }
                }
                if (blnSkipVersion)
                    continue;
            }
            if (InStr(objItem.CategoryIDs, numProductID.ToString()) > 0)
            {
                index = Interaction.IIf(index == -1, i, index);
                tmpItem = BasketItems[index];
                if (objItem.IncTax < tmpItem.IncTax)
                    index = i;
            }
        }

        return index;
    }

    /// <summary>
    ///     ''' Get all promotions
    ///     ''' </summary>
    ///     ''' <param name="numLanguageID">Applicable language</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable GetPromotions(int numLanguageID)
    {
        DataTable tblPromotions = new DataTable();
        tblPromotions = _CustomersAdptr.GetPromotions(numLanguageID);
        return tblPromotions;
    }

    /// <summary>
    ///     ''' Get the option text for an item in the basket
    ///     ''' </summary>
    ///     ''' <param name="numLanguageID">Applicable language</param>
    ///     ''' <param name="numBasketItemID">Basket item to search for</param>
    ///     ''' <param name="strOptionLink">return parameter which has a comma delimited string of the option numbers</param>
    ///     ''' <returns>All option texts seperated by HTML break symbols</returns>
    ///     ''' <remarks></remarks>
    public static string GetOptionText(int numLanguageID, int numBasketItemID, ref string strOptionLink)
    {
        DataTable tblOptionText = new DataTable();
        string strOptionText = "";
        string strOptions, strBreak;

        strBreak = "<br />";
        strOptions = "";

        tblOptionText = _CustomersAdptr.GetOptionText(numLanguageID, numBasketItemID);
        if (tblOptionText.Rows.Count > 0)
        {
            foreach (DataRow drwOptionText in tblOptionText.Rows)
            {
                if (LCase(drwOptionText("OPTG_OptionDisplayType") + "") == "c")
                    strOptionText = strOptionText + drwOptionText("LE_Value") + strBreak;
                else
                    strOptionText = strOptionText + drwOptionText("OPTG_BackendName") + ": " + drwOptionText("LE_Value") + strBreak;
                strOptions = strOptions + drwOptionText("BSKTOPT_OptionID") + ",";
            }
        }

        tblOptionText.Dispose();

        if (strOptions != "")
            strOptions = Strings.Left(strOptions, strOptions.Length - 1);
        strOptionLink = strOptions;

        return strOptionText;
    }

    /// <summary>
    ///     ''' Find all categories that a product appears in 
    ///     ''' </summary>
    ///     ''' <param name="numProductID">The product we want to find categories for</param>
    ///     ''' <returns>a comma delimited string</returns>
    ///     ''' <remarks></remarks>
    public static string GetCategoryIDs(long numProductID)
    {
        DataTable tblCategoryIDs = new DataTable();
        string strCategoryIDs = "";

        tblCategoryIDs = _CustomersAdptr.GetCategoryID(numProductID);

        if (tblCategoryIDs.Rows.Count > 0)
        {
            foreach (DataRow drwCategoryID in tblCategoryIDs.Rows)
                strCategoryIDs = strCategoryIDs + drwCategoryID("PCAT_CategoryID") + ",";
        }

        if (strCategoryIDs != "")
            strCategoryIDs = Strings.Left(strCategoryIDs, strCategoryIDs.Length - 1);

        return strCategoryIDs;
    }

    /// <summary>
    ///     ''' Transfer the information in the data row into the promotion object
    ///     ''' </summary>
    ///     ''' <param name="objPromotion">promotion object</param>
    ///     ''' <param name="drwBuy">data row containing data to be put into promotion object</param>
    ///     ''' <remarks></remarks>
    private static void SetPromotionData(ref Promotion objPromotion, DataRow drwBuy)
    {
        {
            var withBlock = objPromotion;
            withBlock.ID = drwBuy("PROM_ID");
            withBlock.StartDate = drwBuy("PROM_StartDate");
            withBlock.EndDate = drwBuy("PROM_EndDate");
            withBlock.Live = IIf(drwBuy("PROM_Live") + "" == "", 0, drwBuy("PROM_Live") + "");
            withBlock.OrderByValue = IIf(drwBuy("PROM_OrderByValue") + "" == "", 0, drwBuy("PROM_OrderByValue"));
            withBlock.MaxQuantity = IIf(drwBuy("PROM_MaxQuantity") + "" == "", 0, drwBuy("PROM_MaxQuantity"));
            withBlock.PartNo = drwBuy("PP_PartNo") + "";
            withBlock.Type = drwBuy("PP_Type") + "";
            withBlock.Value = drwBuy("PP_Value");
            withBlock.ItemType = drwBuy("PP_ItemType") + "";
            withBlock.ItemID = drwBuy("PP_ItemID");
            withBlock.ItemName = drwBuy("PP_ItemName") + "";
        }
    }

    /// <summary>
    ///     ''' Set the promotion quantities and values using reference parameters
    ///     ''' </summary>
    ///     ''' <param name="numMaxPromoQty">Maximum quantity permitted by promotion</param>
    ///     ''' <param name="Item">The basket item object</param>
    ///     ''' <param name="strType">promotion type (e.g. 'q' = free, 'p' = percentage off, 'v' price (or value) off.</param>
    ///     ''' <param name="numBuyQty"></param>
    ///     ''' <param name="numBuyValue"></param>
    ///     ''' <param name="numGetQty"></param>
    ///     ''' <param name="numGetValue"></param>
    ///     ''' <param name="numIncTax"></param>
    ///     ''' <param name="numExTax"></param>
    ///     ''' <param name="numQty"></param>
    ///     ''' <param name="numTaxRate"></param>
    ///     ''' <param name="intExcessGetQty"></param>
    ///     ''' <remarks></remarks>
    private static void SetPromotionValue(int numMaxPromoQty, Kartris.BasketItem Item, string strType, double numBuyQty, decimal numBuyValue, double numGetQty, decimal numGetValue, ref decimal numIncTax, ref decimal numExTax, ref double numQty, ref decimal numTaxRate, ref int intExcessGetQty = 0)
    {
        if (strType.ToLower() == "q")
        {
            numIncTax = -(Item.IncTax * numGetValue);
            numExTax = -(Item.ExTax * numGetValue);
            numQty = Math.Floor(Math.Min((numBuyQty / numBuyValue), (numGetQty / numGetValue)));
            numQty = Math.Min(numMaxPromoQty, numQty);
            numTaxRate = Item.ComputedTaxRate;
        }
        else if (strType.ToLower() == "p")
        {
            numIncTax = -(Item.IncTax * numGetValue) / (double)100;
            numExTax = -(Item.ExTax * numGetValue) / (double)100;
            numQty = Math.Floor(Math.Min((numBuyQty / numBuyValue), numGetQty));
            numQty = Math.Min(numMaxPromoQty, numQty);
        }
        else if (strType.ToLower() == "v")
        {
            numIncTax = -(Item.IncTax - numGetValue);
            numExTax = -(Item.IncTax - numGetValue);
            numQty = Math.Floor(Math.Min((numBuyQty / numBuyValue), numGetQty));
            numQty = Math.Min(numMaxPromoQty, numQty);
        }
        intExcessGetQty = Math.Floor(numBuyQty - (numBuyValue * numQty));
        if (Item.PromoQty <= 0)
            numQty = 0;
    }

    /// <summary>
    ///     ''' Get the text related to the promotion
    ///     ''' </summary>
    ///     ''' <param name="intPromotionID">The promotion we want the text for</param>
    ///     ''' <param name="blnTextOnly">return on the text and not a HTML anchor etc. (used when calling from presentation layer)</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    private static string GetPromotionText(int intPromotionID, bool blnTextOnly = false)
    {
        DataTable tblPromotionParts = new DataTable();    // '==== language_ID =====
        tblPromotionParts = PromotionsBLL._GetPartsByPromotion(intPromotionID, System.Web.HttpContext.Current.Session["LANG"]);

        string strPromotionText = "";
        int intTextCounter = 0;
        int numLanguageId;

        numLanguageId = System.Web.HttpContext.Current.Session["LANG"];
        VersionsBLL objVersionsBLL = new VersionsBLL();
        ProductsBLL objProductsBLL = new ProductsBLL();
        CategoriesBLL objCategoriesBLL = new CategoriesBLL();

        foreach (DataRow drwPromotionParts in tblPromotionParts.Rows)
        {
            string strText = FixNullFromDB(drwPromotionParts("PS_Text"));
            string strStringID = drwPromotionParts("PS_ID");
            string strValue = CkartrisDisplayFunctions.FixDecimal(FixNullFromDB(drwPromotionParts("PP_Value")));
            string strItemID = FixNullFromDB(drwPromotionParts("PP_ItemID"));
            int intProductID = objVersionsBLL.GetProductID_s(System.Convert.ToInt64(strItemID));
            string strItemName = "";
            string strItemLink = "";

            if (strText.Contains("[X]"))
            {
                if (strText.Contains("[£]"))
                    strText = strText.Replace("[X]", CurrenciesBLL.FormatCurrencyPrice(System.Web.HttpContext.Current.Session["CUR_ID"], CurrenciesBLL.ConvertCurrency(System.Web.HttpContext.Current.Session["CUR_ID"], drwPromotionParts("PP_Value"))));
                else
                    strText = strText.Replace("[X]", strValue);
            }

            if (strText.Contains("[C]") && strItemID != "")
            {
                strItemName = objCategoriesBLL.GetNameByCategoryID(System.Convert.ToInt32(strItemID), numLanguageId);
                strItemLink = " <b><a href='" + CreateURL(Page.CanonicalCategory, strItemID) + "'>" + strItemName + "</a></b>";
                strItemLink = Interaction.IIf(blnTextOnly, strItemName, strItemLink);
                strText = strText.Replace("[C]", strItemLink);
            }

            if (strText.Contains("[P]") && strItemID != "")
            {
                strItemName = objProductsBLL.GetNameByProductID(System.Convert.ToInt32(strItemID), numLanguageId);
                strItemLink = " <b><a href='" + CreateURL(Page.CanonicalProduct, strItemID) + "'>" + strItemName + "</a></b>";
                strItemLink = Interaction.IIf(blnTextOnly, strItemName, strItemLink);
                strText = strText.Replace("[P]", strItemLink);
            }

            if (strText.Contains("[V]") && strItemID != "")
            {
                strItemName = objProductsBLL.GetNameByProductID(intProductID, numLanguageId) + " (" + objVersionsBLL._GetNameByVersionID(System.Convert.ToInt32(strItemID), numLanguageId) + ")";
                strItemLink = " <b><a href='" + CreateURL(Page.CanonicalProduct, intProductID) + "'>" + strItemName + "</a></b>";
                strItemLink = Interaction.IIf(blnTextOnly, strItemName, strItemLink);
                strText = strText.Replace("[V]", strItemLink);
            }

            if (strText.Contains("[£]"))
                strText = strText.Replace("[£]", "");

            intTextCounter += 1;
            if (intTextCounter > 1)
                strPromotionText += ", ";
            strPromotionText += strText;
        }

        return strPromotionText;
    }

    /// <summary>
    ///     ''' Add promotion to the promotion collection for this basket.
    ///     ''' </summary>
    ///     ''' <param name="blnBasketPromo">Allow promotion to be added multiple times to same basket</param>
    ///     ''' <param name="strPromoDiscountIDs"></param>
    ///     ''' <param name="objPromotion">The promotion to be added</param>
    ///     ''' <param name="numPromoID"></param>
    ///     ''' <param name="numIncTax"></param>
    ///     ''' <param name="numExTax"></param>
    ///     ''' <param name="numQty"></param>
    ///     ''' <param name="numTaxRate"></param>
    ///     ''' <param name="blnIsFixedValuePromo"></param>
    ///     ''' <param name="blnForceAdd"></param>
    ///     ''' <remarks></remarks>
    private static void AddPromotion(ref Kartris.Basket Basket, bool blnBasketPromo, ref string strPromoDiscountIDs, ref Kartris.Promotion objPromotion, int numPromoID, decimal numIncTax, decimal numExTax, double numQty, decimal numTaxRate, bool blnIsFixedValuePromo = false, bool blnForceAdd = false)
    {
        int intMaxPromoOrder = 0;

        intMaxPromoOrder = Val(GetKartConfig("frontend.promotions.maximum"));

        if ((blnBasketPromo & (Basket.objPromotionsDiscount.Count < intMaxPromoOrder | intMaxPromoOrder == 0)) || blnForceAdd)
        {
            strPromoDiscountIDs = strPromoDiscountIDs + numPromoID + ";";
            PromotionBasketModifier objPromotionDiscount = new PromotionBasketModifier();
            {
                var withBlock = objPromotionDiscount;
                withBlock.PromotionID = numPromoID;
                withBlock.Name = GetPromotionText(objPromotion.ID, true);
                withBlock.ApplyTax = Basket.ApplyTax;
                withBlock.ComputedTaxRate = numTaxRate;
                withBlock.ExTax = CurrenciesBLL.ConvertCurrency(System.Web.HttpContext.Current.Session["CUR_ID"], numExTax);
                withBlock.IncTax = CurrenciesBLL.ConvertCurrency(System.Web.HttpContext.Current.Session["CUR_ID"], numIncTax);
                withBlock.Quantity = numQty;
                withBlock.TotalIncTax = withBlock.TotalIncTax + (withBlock.IncTax * withBlock.Quantity);
                withBlock.TotalExTax = withBlock.TotalExTax + (withBlock.ExTax * withBlock.Quantity);
                withBlock.isFixedValuePromo = blnIsFixedValuePromo;
            }
            Basket.objPromotionsDiscount.Add(objPromotionDiscount);
        }
        else
        {
            // ' add only to promotion if not in promotion discount yet
            bool blnFound = false;
            foreach (PromotionBasketModifier objPromo in Basket.objPromotionsDiscount)
            {
                if (objPromo.PromotionID == objPromotion.ID)
                {
                    blnFound = true; break;
                }
            }

            if (blnFound == false)
            {
                objPromotion.PromoText = GetPromotionText(objPromotion.ID);
                Basket.objPromotions.Add(objPromotion);
            }
        }
    }

    /// <summary>
    ///     ''' Reset any applied promotions and apply the promotion collection to the basket and all items contained therein.
    ///     ''' </summary>
    ///     ''' <param name="aryPromotions">List of promotions</param>
    ///     ''' <param name="aryPromotionsDiscount"></param>
    ///     ''' <param name="blnZeroTotalTaxRate">Do not calculate tax</param>
    ///     ''' <remarks></remarks>
    public static void CalculatePromotions(ref Kartris.Basket Basket, ref List<Kartris.Promotion> aryPromotions, ref ArrayList aryPromotionsDiscount, bool blnZeroTotalTaxRate)
    {
        System.Data.DataTable tblPromotions;
        System.Data.DataRow[] drwBuys;
        System.Data.DataRow[] drwGets;
        System.Data.DataRow[] drwSpends;
        string strPromoIDs, strPromoDiscountIDs, strList;
        int vPromoID, vMaxPromoQty, vItemID;
        string strItemType, strType;
        string strItemVersionIDs, strItemProductIDs, strItemCategoryIDs;
        double vIncTax, vExTax, vQuantity, vBuyQty, vValue, vTaxRate;
        decimal numTotalBasketAmount;
        if (Basket.BasketItems.Count == 0)
            return;

        // Clear AppliedPromotion to all Basket Items
        foreach (Kartris.BasketItem objBasketItem in Basket.BasketItems)
            objBasketItem.AppliedPromo = 0;

        int numLanguageID;
        Kartris.BasketItem objItem = new Kartris.BasketItem();

        numLanguageID = 1;
        strPromoIDs = ";"; strPromoDiscountIDs = ";";
        strItemVersionIDs = GetBasketItemVersionIDs(ref Basket.BasketItems);
        strItemProductIDs = GetBasketItemProductIDs(ref Basket.BasketItems);
        strItemCategoryIDs = GetBasketItemCategoryIDs(ref Basket.BasketItems);
        Basket.PromotionDiscount.IncTax = 0; Basket.PromotionDiscount.ExTax = 0;

        aryPromotions.Clear(); aryPromotionsDiscount.Clear();
        Basket.objPromotions.Clear(); Basket.objPromotionsDiscount.Clear();

        int intCouponPromotionID = 0;
        if (!string.IsNullOrEmpty(Basket.CouponCode))
        {
            string strCouponType = "";
            string strCouponError = "";
            decimal numCouponValue = 0;

            GetCouponDiscount(ref Basket, Basket.CouponCode, ref strCouponError, ref strCouponType, ref numCouponValue);
            if (strCouponType == "t")
                intCouponPromotionID = System.Convert.ToInt32(numCouponValue);
        }
        tblPromotions = PromotionsBLL.GetAllPromotions(numLanguageID, intCouponPromotionID);

        // ' get promotions from Basket version IDs (buy promotion parts)
        strList = "PP_PartNo='a' and PP_ItemType='v' and PP_Type='q' and PP_ItemID in (" + strItemVersionIDs + ")";
        drwBuys = tblPromotions.Select(strList);
        foreach (DataRow drwBuy in drwBuys)
        {
            vPromoID = drwBuy("PROM_ID");
            if (Strings.InStr(strPromoIDs, vPromoID) == 0)
            {
                strPromoIDs = strPromoIDs + vPromoID + ";";

                Promotion objPromotion = new Promotion();
                SetPromotionData(ref objPromotion, drwBuy);

                objItem = GetBasketItemByVersionID(ref Basket.BasketItems, objPromotion.ItemID);

                if (objItem.Quantity >= objPromotion.Value & objItem.AppliedPromo == 0)
                {
                    vIncTax = 0; vExTax = 0; vQuantity = 0; vTaxRate = 0;
                    vBuyQty = objItem.Quantity;

                    bool blnGetFound = false;

                    strList = "PP_PartNo='b' and PROM_ID=" + objPromotion.ID;
                    drwGets = tblPromotions.Select(strList);
                    foreach (DataRow drGet in drwGets) // loop the get items
                    {
                        strItemType = drGet("PP_ItemType") + "";
                        strType = drGet("PP_Type") + "";
                        vItemID = drGet("PP_ItemID");
                        vValue = drGet("PP_Value");
                        vMaxPromoQty = IIf(drGet("PROM_MaxQuantity") == 0, 1000000, drGet("PROM_MaxQuantity"));
                        switch (strItemType.ToLower())
                        {
                            case "v" // buy version and get item from version
                           :
                                {
                                    if (Strings.InStr("," + strItemVersionIDs + ",", "," + vItemID + ",") > 0)
                                    {
                                        // vBuyQty (qty in basket), objPromo.value (buy qty in db), objItem.Quantity(qty in basket get promo), vValue (get qty in db) 
                                        objItem = GetBasketItemByVersionID(ref Basket.BasketItems, vItemID);
                                        if (objItem.AppliedPromo == 1)
                                            break;
                                        if (objPromotion.ItemID == vItemID)
                                        {
                                            if ((vBuyQty > vValue && strType == "q") || (vBuyQty >= drwBuy("PP_Value") && strType == "p"))
                                            {
                                                blnGetFound = true;
                                                SetPromotionValue(vMaxPromoQty, objItem, strType, vBuyQty, objPromotion.Value, vValue, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate);
                                            }
                                        }
                                        else
                                        {
                                            blnGetFound = true;
                                            SetPromotionValue(vMaxPromoQty, objItem, strType, vBuyQty, objPromotion.Value, objItem.Quantity, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate);
                                        }
                                    }

                                    if (vQuantity <= 0)
                                        blnGetFound = false;
                                    AddPromotion(ref Basket, blnGetFound, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate);
                                    if (blnGetFound)
                                        objItem.AppliedPromo = 1;
                                    break;
                                }

                            case "p" // buy version and get item from product
                     :
                                {
                                    if (Strings.InStr("," + strItemProductIDs + ",", "," + vItemID + ",") > 0)
                                    {
                                        double vTotalQtyGot, vQtyBalance;
                                        int index;

                                        vTotalQtyGot = 0;
                                        blnGetFound = true;
                                        vTotalQtyGot = vTotalQtyGot + vQuantity;
                                        vQtyBalance = (vBuyQty / objPromotion.Value) - vTotalQtyGot;

                                        while (vQtyBalance > 0)
                                        {
                                            index = GetItemMinProductValue(ref Basket.BasketItems, vItemID);
                                            if (index < 0)
                                                break;
                                            objItem = Basket.BasketItems(index);
                                            if (objItem.AppliedPromo == 0)
                                            {
                                                vQtyBalance = Math.Min(vQtyBalance, objItem.Quantity);
                                                SetPromotionValue(vMaxPromoQty, objItem, strType, vBuyQty, objPromotion.Value, vQtyBalance, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate);
                                                Basket.BasketItems(index).PromoQty = Basket.BasketItems(index).Quantity - vQuantity;

                                                if (vQuantity <= 0)
                                                    blnGetFound = false;
                                                AddPromotion(ref Basket, blnGetFound, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate);
                                                if (blnGetFound)
                                                    objItem.AppliedPromo = 1;
                                                vTotalQtyGot = vTotalQtyGot + vQuantity;
                                                vQtyBalance = (vBuyQty / objPromotion.Value) - vTotalQtyGot;

                                                if (blnGetFound == false)
                                                    break;
                                            }
                                        }
                                    }

                                    blnGetFound = false;
                                    AddPromotion(ref Basket, blnGetFound, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate);
                                    if (blnGetFound)
                                        objItem.AppliedPromo = 1;
                                    break;
                                }

                            case "c" // buy version and get item from category
                     :
                                {
                                    if (Strings.InStr("," + strItemCategoryIDs + ",", "," + vItemID + ",") > 0)
                                    {
                                        int index;
                                        index = GetItemMinCategoryValue(ref Basket.BasketItems, vItemID);
                                        objItem = Basket.BasketItems(index);
                                        if (objItem.AppliedPromo == 1)
                                            break;
                                        blnGetFound = true;
                                        SetPromotionValue(vMaxPromoQty, objItem, strType, vBuyQty, objPromotion.Value, objItem.Quantity, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate);
                                    }

                                    if (vQuantity <= 0)
                                        blnGetFound = false;
                                    AddPromotion(ref Basket, blnGetFound, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate);
                                    if (blnGetFound)
                                        objItem.AppliedPromo = 1;
                                    break;
                                }

                            case "a":
                                {
                                    vQuantity = Math.Floor(Math.Min((vBuyQty / objPromotion.Value), (objItem.Quantity / (double)objPromotion.Value)));
                                    vQuantity = Math.Min(vQuantity, vMaxPromoQty); // Make sure it didn't exceed the MaxQty / promotion
                                    if (vQuantity <= 0)
                                        blnGetFound = false;
                                    else
                                        blnGetFound = true;

                                    vTaxRate = Basket.TotalDiscountPriceTaxRate;

                                    if (vValue > Basket.TotalDiscountPriceExTax)
                                    {
                                        vExTax = -Basket.TotalDiscountPriceExTax;
                                        vIncTax = -Basket.TotalDiscountPriceIncTax;
                                    }
                                    else
                                    {
                                        bool blnPricesExtax = false;

                                        if (!blnZeroTotalTaxRate)
                                        {
                                            if (GetKartConfig("general.tax.pricesinctax") == "y")
                                            {
                                                vExTax = -Math.Round(vValue * (1 / (1 + vTaxRate)), CurrencyRoundNumber);
                                                if (Basket.D_Tax == 1)
                                                    vIncTax = -Math.Round(vValue, CurrencyRoundNumber);
                                                else
                                                    vIncTax = vExTax;
                                            }
                                            else
                                                blnPricesExtax = true;
                                        }
                                        else
                                            blnPricesExtax = true;

                                        if (blnPricesExtax)
                                        {
                                            vIncTax = -Math.Round(vValue * (1 + vTaxRate), CurrencyRoundNumber);
                                            vExTax = -Math.Round(vValue, CurrencyRoundNumber);
                                        }
                                    }

                                    AddPromotion(ref Basket, blnGetFound, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate, true);
                                    objItem.AppliedPromo = 1;
                                    break;
                                }
                        }
                    }
                }
                else
                    AddPromotion(ref Basket, false, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate);
            }
        }

        // products
        // get promotions from basket product IDs
        strList = "PP_PartNo='a' and PP_ItemType='p' and PP_Type='q' and PP_ItemID in (" + strItemProductIDs + ")";
        drwBuys = tblPromotions.Select(strList);
        foreach (DataRow drwBuy in drwBuys)
        {
            vPromoID = drwBuy("PROM_ID");
            if (Strings.InStr(strPromoIDs, vPromoID) == 0)
            {
                strPromoIDs = strPromoIDs + vPromoID + ";";

                Promotion objPromotion = new Promotion();
                SetPromotionData(ref objPromotion, drwBuy);

                int cnt = 0;
                for (int i = 0; i <= Basket.BasketItems.Count - 1; i++)
                {
                    objItem = Basket.BasketItems(i);
                    if (objItem.ProductID == objPromotion.ItemID & objItem.AppliedPromo == 0)
                        cnt = cnt + objItem.Quantity;
                }

                if (cnt >= objPromotion.Value)
                {
                    vIncTax = 0; vExTax = 0; vQuantity = 0; vTaxRate = 0;
                    vBuyQty = cnt;

                    bool blnGetFound = false;

                    strList = "PP_PartNo='b' and PROM_ID=" + objPromotion.ID;
                    drwGets = tblPromotions.Select(strList);
                    bool blnIsFixedValuePromo = false;
                    bool blnForceAdd = false;
                    foreach (DataRow drGet in drwGets) // ' loop the get items
                    {
                        strItemType = drGet("PP_ItemType");
                        strType = drGet("PP_Type") + "";
                        vItemID = drGet("PP_ItemID");
                        vValue = drGet("PP_Value");
                        vMaxPromoQty = IIf(drGet("PROM_MaxQuantity") == 0, 1000000, drGet("PROM_MaxQuantity"));
                        blnIsFixedValuePromo = false;

                        switch (strItemType.ToLower())
                        {
                            case "v"                    // ' buy product and get item from version
                           :
                                {
                                    if (Strings.InStr("," + strItemVersionIDs + ",", "," + vItemID + ",") > 0)
                                    {
                                        objItem = GetBasketItemByVersionID(ref Basket.BasketItems, vItemID);
                                        if (objItem.AppliedPromo == 1)
                                            continue;
                                        blnGetFound = true;
                                        SetPromotionValue(vMaxPromoQty, objItem, strType, vBuyQty, objPromotion.Value, objItem.Quantity, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate);
                                    }

                                    break;
                                }

                            case "p"                    // ' buy product and get item from product
                     :
                                {
                                    if (Strings.InStr("," + strItemProductIDs + ",", "," + vItemID + ",") > 0)
                                    {
                                        int index;
                                        index = GetItemMinProductValue(ref Basket.BasketItems, vItemID);
                                        objItem = Basket.BasketItems(index);
                                        if (objItem.AppliedPromo == 1)
                                            continue;
                                        blnGetFound = true;
                                        int intExcessGetQty;
                                        SetPromotionValue(vMaxPromoQty, objItem, strType, vBuyQty, objPromotion.Value, objItem.Quantity, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate, ref intExcessGetQty);
                                        string strVersionIDArray = "";
                                        while (intExcessGetQty > 0)
                                        {
                                            if (strVersionIDArray != "")
                                                strVersionIDArray += ",";
                                            blnIsFixedValuePromo = true;
                                            objItem.AppliedPromo = 1;
                                            vMaxPromoQty = vMaxPromoQty - vQuantity;
                                            if (intExcessGetQty < objPromotion.Value | vMaxPromoQty < 1)
                                            {
                                                intExcessGetQty = 0;
                                                break;
                                            }
                                            AddPromotion(ref Basket, blnGetFound, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate, blnIsFixedValuePromo, blnForceAdd);
                                            if (blnGetFound)
                                                objItem.AppliedPromo = 1;
                                            strVersionIDArray = strVersionIDArray + objItem.VersionID;
                                            // Dim numExcessItemsInPromo As Double = vBuyQty - (objPromotion.Value * objItem.Quantity)
                                            index = GetItemMinProductValue(ref Basket.BasketItems, vItemID, strVersionIDArray);
                                            if (index != -1)
                                            {
                                                objItem = Basket.BasketItems(index);
                                                if (objItem.AppliedPromo == 1)
                                                    continue;
                                                SetPromotionValue(vMaxPromoQty, objItem, strType, intExcessGetQty, objPromotion.Value, objItem.Quantity, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate, ref intExcessGetQty);
                                                blnForceAdd = true;
                                            }
                                            else
                                                intExcessGetQty = 0;
                                        }
                                    }

                                    break;
                                }

                            case "c"                    // ' buy product and get item from category
                     :
                                {
                                    if (Strings.InStr("," + strItemCategoryIDs + ",", "," + vItemID + ",") > 0)
                                    {
                                        int index;
                                        index = GetItemMinCategoryValue(ref Basket.BasketItems, vItemID);
                                        objItem = Basket.BasketItems(index);
                                        // If objItem.AppliedPromo = 1 Then Continue For
                                        blnGetFound = true;
                                        int intExcessGetQty = 0;
                                        SetPromotionValue(vMaxPromoQty, objItem, strType, vBuyQty, objPromotion.Value, objItem.Quantity, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate, ref intExcessGetQty);
                                        string strVersionIDArray = "";
                                        while (intExcessGetQty > 0)
                                        {
                                            if (strVersionIDArray != "")
                                                strVersionIDArray += ",";
                                            blnIsFixedValuePromo = true;
                                            objItem.AppliedPromo = 1;
                                            vMaxPromoQty = vMaxPromoQty - vQuantity;
                                            if (intExcessGetQty < objPromotion.Value | vMaxPromoQty < 1)
                                            {
                                                intExcessGetQty = 0;
                                                break;
                                            }
                                            AddPromotion(ref Basket, blnGetFound, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate, blnIsFixedValuePromo, blnForceAdd);
                                            if (blnGetFound)
                                                objItem.AppliedPromo = 1;
                                            strVersionIDArray = strVersionIDArray + objItem.VersionID;
                                            // Dim numExcessItemsInPromo As Double = vBuyQty - (objPromotion.Value * objItem.Quantity)
                                            index = GetItemMinCategoryValue(ref Basket.BasketItems, vItemID, strVersionIDArray);
                                            if (index != -1)
                                            {
                                                objItem = Basket.BasketItems(index);
                                                // If objItem.AppliedPromo = 1 Then Continue For
                                                SetPromotionValue(vMaxPromoQty, objItem, strType, intExcessGetQty, objPromotion.Value, objItem.Quantity, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate, ref intExcessGetQty);
                                                blnForceAdd = true;
                                            }
                                            else
                                                intExcessGetQty = 0;
                                        }
                                    }

                                    break;
                                }

                            case "a":
                                {
                                    vQuantity = Math.Floor(Math.Min((vBuyQty / objPromotion.Value), (cnt / (double)objPromotion.Value)));
                                    vQuantity = Math.Min(vQuantity, vMaxPromoQty); // ' Make sure it didn't exceed the MaxQty / promotion
                                    if (vQuantity <= 0)
                                        blnGetFound = false;
                                    else
                                        blnGetFound = true;

                                    vTaxRate = Basket.TotalDiscountPriceTaxRate;

                                    if (vValue > Basket.TotalDiscountPriceExTax)
                                    {
                                        vExTax = -Basket.TotalDiscountPriceExTax;
                                        vIncTax = -Basket.TotalDiscountPriceIncTax;
                                    }
                                    else
                                    {
                                        bool blnPricesExtax = false;

                                        if (!blnZeroTotalTaxRate)
                                        {
                                            if (GetKartConfig("general.tax.pricesinctax") == "y")
                                            {
                                                vExTax = -Math.Round(vValue * (1 / (1 + vTaxRate)), CurrencyRoundNumber);
                                                if (Basket.D_Tax == 1)
                                                    vIncTax = -Math.Round(vValue, CurrencyRoundNumber);
                                                else
                                                    vIncTax = vExTax;
                                            }
                                            else
                                                blnPricesExtax = true;
                                        }
                                        else
                                            blnPricesExtax = true;

                                        if (blnPricesExtax)
                                        {
                                            vIncTax = -Math.Round(vValue * (1 + vTaxRate), CurrencyRoundNumber);
                                            vExTax = -Math.Round(vValue, CurrencyRoundNumber);
                                        }
                                    }

                                    blnIsFixedValuePromo = true;
                                    objItem.AppliedPromo = 1;
                                    break;
                                }
                        }
                    }
                    AddPromotion(ref Basket, blnGetFound, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate, blnIsFixedValuePromo, blnForceAdd);
                    if (blnGetFound)
                        objItem.AppliedPromo = 1;
                }
                else
                    AddPromotion(ref Basket, false, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate);
            }
        }

        // ' get promotions from basket category IDs
        strList = "PP_PartNo='a' and PP_ItemType='c' and PP_Type='q' and PP_ItemID in (" + strItemCategoryIDs + ")";
        drwBuys = tblPromotions.Select(strList);
        foreach (DataRow drwBuy in drwBuys)
        {
            vPromoID = drwBuy("PROM_ID");
            if (Strings.InStr(strPromoIDs, vPromoID) == 0)
            {
                strPromoIDs = strPromoIDs + vPromoID + ";";

                Promotion objPromotion = new Promotion();
                SetPromotionData(ref objPromotion, drwBuy);

                int cnt = 0;
                for (int i = 0; i <= Basket.BasketItems.Count - 1; i++)
                {
                    objItem = Basket.BasketItems(i);
                    if (InStr("," + objItem.CategoryIDs + ",", "," + objPromotion.ItemID + ",") > 0)
                        cnt = cnt + objItem.Quantity;
                }

                if (cnt >= objPromotion.Value)
                {
                    vIncTax = 0; vExTax = 0; vQuantity = 0; vTaxRate = 0;
                    vBuyQty = cnt;

                    bool blnGetFound = false;

                    strList = "PP_PartNo='b' and PROM_ID=" + objPromotion.ID;
                    drwGets = tblPromotions.Select(strList);
                    bool blnIsFixedValuePromo = false;
                    bool blnForceAdd = false;

                    foreach (DataRow drGet in drwGets) // ' loop the get items
                    {
                        strItemType = drGet("PP_ItemType") + "";
                        strType = drGet("PP_Type") + "";
                        vItemID = drGet("PP_ItemID");
                        vValue = drGet("PP_Value");
                        vMaxPromoQty = IIf(drGet("PROM_MaxQuantity") == 0, 1000000, drGet("PROM_MaxQuantity"));
                        blnIsFixedValuePromo = false;
                        switch (strItemType.ToLower())
                        {
                            case "v"                    // ' buy category and get item from version
                           :
                                {
                                    if (Strings.InStr("," + strItemVersionIDs + ",", "," + vItemID + ",") > 0)
                                    {
                                        objItem = GetBasketItemByVersionID(ref Basket.BasketItems, vItemID);
                                        if (objItem.AppliedPromo == 1)
                                            break;
                                        blnGetFound = true;
                                        SetPromotionValue(vMaxPromoQty, objItem, strType, vBuyQty, objPromotion.Value, objItem.Quantity, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate);
                                    }

                                    break;
                                }

                            case "p"                    // ' buy category and get item from product
                     :
                                {
                                    if (Strings.InStr("," + strItemProductIDs + ",", "," + vItemID + ",") > 0)
                                    {
                                        int index;
                                        index = GetItemMinProductValue(ref Basket.BasketItems, vItemID);
                                        objItem = Basket.BasketItems(index);
                                        if (objItem.AppliedPromo == 1)
                                            break;
                                        blnGetFound = true;
                                        int intExcessGetQty = 0;
                                        SetPromotionValue(vMaxPromoQty, objItem, strType, vBuyQty, objPromotion.Value, objItem.Quantity, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate, ref intExcessGetQty);
                                        string strVersionIDArray = "";
                                        while (intExcessGetQty > 0)
                                        {
                                            if (strVersionIDArray != "")
                                                strVersionIDArray += ",";
                                            blnIsFixedValuePromo = true;
                                            objItem.AppliedPromo = 1;
                                            vMaxPromoQty = vMaxPromoQty - vQuantity;
                                            if (intExcessGetQty < objPromotion.Value | vMaxPromoQty < 1)
                                            {
                                                intExcessGetQty = 0;
                                                break;
                                            }
                                            AddPromotion(ref Basket, blnGetFound, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate, blnIsFixedValuePromo, blnForceAdd);
                                            if (blnGetFound)
                                                objItem.AppliedPromo = 1;
                                            strVersionIDArray = strVersionIDArray + objItem.VersionID;
                                            // Dim numExcessItemsInPromo As Double = vBuyQty - (objPromotion.Value * objItem.Quantity)
                                            index = GetItemMinProductValue(ref Basket.BasketItems, vItemID, strVersionIDArray);
                                            if (index != -1)
                                            {
                                                objItem = Basket.BasketItems(index);
                                                if (objItem.AppliedPromo == 1)
                                                    continue;
                                                SetPromotionValue(vMaxPromoQty, objItem, strType, intExcessGetQty, objPromotion.Value, objItem.Quantity, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate, ref intExcessGetQty);
                                                blnForceAdd = true;
                                            }
                                            else
                                                intExcessGetQty = 0;
                                        }
                                    }

                                    break;
                                }

                            case "c"                    // ' buy category and get item from category
                     :
                                {
                                    if (Strings.InStr("," + strItemCategoryIDs + ",", "," + vItemID + ",") > 0)
                                    {
                                        int index;
                                        index = GetItemMinCategoryValue(ref Basket.BasketItems, vItemID);
                                        objItem = Basket.BasketItems(index);
                                        if (objItem.AppliedPromo == 1)
                                            break;
                                        blnGetFound = true;
                                        int intExcessGetQty = 0;
                                        SetPromotionValue(vMaxPromoQty, objItem, strType, vBuyQty, objPromotion.Value, objItem.Quantity, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate, ref intExcessGetQty);
                                        string strVersionIDArray = "";
                                        while (intExcessGetQty > 0)
                                        {
                                            if (strVersionIDArray != "")
                                                strVersionIDArray += ",";
                                            blnIsFixedValuePromo = true;
                                            objItem.AppliedPromo = 1;
                                            vMaxPromoQty = vMaxPromoQty - vQuantity;
                                            if (intExcessGetQty < objPromotion.Value | vMaxPromoQty < 1)
                                            {
                                                intExcessGetQty = 0;
                                                break;
                                            }
                                            AddPromotion(ref Basket, blnGetFound, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate, blnIsFixedValuePromo, blnForceAdd);
                                            if (blnGetFound)
                                                objItem.AppliedPromo = 1;
                                            strVersionIDArray = strVersionIDArray + objItem.VersionID;
                                            // Dim numExcessItemsInPromo As Double = vBuyQty - (objPromotion.Value * objItem.Quantity)
                                            index = GetItemMinCategoryValue(ref Basket.BasketItems, vItemID, strVersionIDArray);
                                            if (index != -1)
                                            {
                                                objItem = Basket.BasketItems(index);
                                                if (objItem.AppliedPromo == 1)
                                                    continue;
                                                SetPromotionValue(vMaxPromoQty, objItem, strType, intExcessGetQty, objPromotion.Value, objItem.Quantity, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate, ref intExcessGetQty);
                                                blnForceAdd = true;
                                            }
                                            else
                                                intExcessGetQty = 0;
                                        }
                                    }

                                    break;
                                }

                            case "a":
                                {
                                    vQuantity = Math.Floor(Math.Min((vBuyQty / objPromotion.Value), (cnt / (double)objPromotion.Value)));
                                    vQuantity = Math.Min(vQuantity, vMaxPromoQty); // ' Make sure it didn't exceed the MaxQty / promotion
                                    if (vQuantity <= 0)
                                        blnGetFound = false;
                                    else
                                        blnGetFound = true;

                                    vTaxRate = Basket.TotalDiscountPriceTaxRate;

                                    if (vValue > Basket.TotalDiscountPriceExTax)
                                    {
                                        vExTax = -Basket.TotalDiscountPriceExTax;
                                        vIncTax = -Basket.TotalDiscountPriceIncTax;
                                    }
                                    else
                                    {
                                        bool blnPricesExtax = false;

                                        if (!blnZeroTotalTaxRate)
                                        {
                                            if (GetKartConfig("general.tax.pricesinctax") == "y")
                                            {
                                                vExTax = -Math.Round(vValue * (1 / (1 + vTaxRate)), CurrencyRoundNumber);
                                                if (Basket.D_Tax == 1)
                                                    vIncTax = -Math.Round(vValue, CurrencyRoundNumber);
                                                else
                                                    vIncTax = vExTax;
                                            }
                                            else
                                                blnPricesExtax = true;
                                        }
                                        else
                                            blnPricesExtax = true;

                                        if (blnPricesExtax)
                                        {
                                            vIncTax = -Math.Round(vValue * (1 + vTaxRate), CurrencyRoundNumber);
                                            vExTax = -Math.Round(vValue, CurrencyRoundNumber);
                                        }
                                    }

                                    blnIsFixedValuePromo = true;
                                    objItem.AppliedPromo = 1;
                                    break;
                                }
                        }
                    }
                    AddPromotion(ref Basket, blnGetFound, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate, blnIsFixedValuePromo, blnForceAdd);
                    if (blnGetFound)
                        objItem.AppliedPromo = 1;
                }
                else
                    AddPromotion(ref Basket, false, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate);
            }
        }

        // get spend value from basket
        decimal vSpend = 0;
        strList = "PP_PartNo='a' and PP_ItemType='a'";
        drwSpends = tblPromotions.Select(strList);


        foreach (DataRow drSpend in drwSpends)
        {
            vPromoID = drSpend("PROM_ID");
            if (Strings.InStr(strPromoIDs, vPromoID) == 0)
            {
                strPromoIDs = strPromoIDs + vPromoID + ";";

                Promotion objPromotion = new Promotion();
                SetPromotionData(ref objPromotion, drSpend);

                vSpend = drSpend("PP_Value");

                vSpend = Math.Round(System.Convert.ToDecimal(CurrenciesBLL.ConvertCurrency(System.Web.HttpContext.Current.Session["CUR_ID"], drSpend("PP_Value"))), 4);

                vIncTax = 0; vExTax = 0; vQuantity = 0; vTaxRate = 0;

                bool blnGetFound = false;

                strList = "PP_PartNo='b' and PROM_ID=" + objPromotion.ID;
                drwGets = tblPromotions.Select(strList);
                bool blnIsFixedValuePromo = false;
                bool blnForceAdd = false;

                if (GetKartConfig("general.tax.pricesinctax") == "y")
                    numTotalBasketAmount = Basket.TotalIncTax;
                else
                    numTotalBasketAmount = Basket.TotalExTax;

                foreach (DataRow drGet in drwGets)     // ' loop the get items
                {
                    strItemType = drGet("PP_ItemType") + "";
                    strType = drGet("PP_Type") + "";
                    vItemID = drGet("PP_ItemID");
                    vValue = drGet("PP_Value");
                    vMaxPromoQty = IIf(drGet("PROM_MaxQuantity") == 0, 1000000, drGet("PROM_MaxQuantity"));
                    blnIsFixedValuePromo = false;
                    switch (strItemType.ToLower())
                    {
                        case "v" // spend a certain amount and get item from version
                       :
                            {
                                if (Strings.InStr("," + strItemVersionIDs + ",", "," + vItemID + ",") > 0)
                                {
                                    objItem = GetBasketItemByVersionID(ref Basket.BasketItems, vItemID);
                                    if (objItem.AppliedPromo == 1)
                                        break;
                                    blnGetFound = true;
                                    vBuyQty = vValue * (Int(Basket.TotalIncTax / (double)vSpend));
                                    objPromotion.Value = vValue;
                                    SetPromotionValue(vMaxPromoQty, objItem, strType, vBuyQty, objPromotion.Value, objItem.Quantity, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate);
                                }

                                break;
                            }

                        case "p" // spend a certain amount and get item from product
                 :
                            {
                                if (Strings.InStr("," + strItemProductIDs + ",", "," + vItemID + ",") > 0)
                                {
                                    int index;
                                    index = GetItemMinProductValue(ref Basket.BasketItems, vItemID);
                                    objItem = Basket.BasketItems(index);
                                    if (objItem.AppliedPromo == 1)
                                        break;
                                    blnGetFound = true;
                                    vBuyQty = vValue * (Int(Basket.TotalIncTax / (double)vSpend));
                                    objPromotion.Value = vValue;
                                    int intExcessGetQty = 0;
                                    SetPromotionValue(vMaxPromoQty, objItem, strType, vBuyQty, objPromotion.Value, objItem.Quantity, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate, ref intExcessGetQty);
                                    string strVersionIDArray = "";

                                    while (intExcessGetQty > 0)
                                    {
                                        if (strVersionIDArray != "")
                                            strVersionIDArray += ",";
                                        blnIsFixedValuePromo = true;

                                        vMaxPromoQty = vMaxPromoQty - vQuantity;
                                        if (intExcessGetQty < objPromotion.Value | vMaxPromoQty < 1)
                                        {
                                            intExcessGetQty = 0;
                                            break;
                                        }

                                        strVersionIDArray = strVersionIDArray + objItem.VersionID;
                                        index = GetItemMinProductValue(ref Basket.BasketItems, vItemID, strVersionIDArray);
                                        if (index != -1)
                                        {
                                            if (objItem.AppliedPromo == 0)
                                            {
                                                AddPromotion(ref Basket, blnGetFound, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate, blnIsFixedValuePromo, blnForceAdd);
                                                objItem.AppliedPromo = 1;
                                            }
                                            objItem = Basket.BasketItems(index);
                                            if (objItem.AppliedPromo == 1)
                                                continue;
                                            SetPromotionValue(vMaxPromoQty, objItem, strType, intExcessGetQty, objPromotion.Value, objItem.Quantity, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate, ref intExcessGetQty);
                                            blnForceAdd = true;
                                        }
                                        else
                                            intExcessGetQty = 0;
                                    }
                                }

                                break;
                            }

                        case "c" // spend a certain amount and get item from category
                 :
                            {
                                if (Strings.InStr("," + strItemCategoryIDs + ",", "," + vItemID + ",") > 0)
                                {
                                    int index;
                                    index = GetItemMinCategoryValue(ref Basket.BasketItems, vItemID);
                                    objItem = Basket.BasketItems(index);
                                    if (objItem.AppliedPromo == 1)
                                        break;
                                    blnGetFound = true;
                                    vBuyQty = vValue * (Int(Basket.TotalIncTax / (double)vSpend));
                                    objPromotion.Value = vValue;

                                    int intExcessGetQty = 0;
                                    SetPromotionValue(vMaxPromoQty, objItem, strType, vBuyQty, objPromotion.Value, objItem.Quantity, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate, ref intExcessGetQty);
                                    string strVersionIDArray = "";

                                    while (intExcessGetQty > 0)
                                    {
                                        if (strVersionIDArray != "")
                                            strVersionIDArray += ",";
                                        blnIsFixedValuePromo = true;
                                        vMaxPromoQty = vMaxPromoQty - vQuantity;
                                        if (intExcessGetQty < objPromotion.Value | vMaxPromoQty < 1)
                                        {
                                            intExcessGetQty = 0;
                                            break;
                                        }

                                        // comment out the string below as I think this blocks applying
                                        // the discount to item that triggers the discount which in the
                                        // case of spend/get promotions doesn't make sense

                                        strVersionIDArray = strVersionIDArray + objItem.VersionID;

                                        index = GetItemMinCategoryValue(ref Basket.BasketItems, vItemID, strVersionIDArray);
                                        if (index != -1)
                                        {
                                            if (objItem.AppliedPromo == 0)
                                            {
                                                AddPromotion(ref Basket, blnGetFound, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate, blnIsFixedValuePromo, blnForceAdd);
                                                objItem.AppliedPromo = 1;
                                            }

                                            objItem = Basket.BasketItems(index);
                                            if (objItem.AppliedPromo == 1)
                                                continue;
                                            SetPromotionValue(vMaxPromoQty, objItem, strType, intExcessGetQty, objPromotion.Value, objItem.Quantity, vValue, ref vIncTax, ref vExTax, ref vQuantity, ref vTaxRate, ref intExcessGetQty);
                                            blnForceAdd = true;
                                        }
                                        else
                                            intExcessGetQty = 0;
                                    }
                                }

                                break;
                            }

                        case "a" // Total spend promotion
                 :
                            {

                                // If total in basket (inc or ex, depending on settings)
                                // is more than the vSpend required, Qty will be above zero
                                vQuantity = Conversion.Int(numTotalBasketAmount / (double)vSpend);

                                // If Qty above zero (i.e. promotion is triggered) then set
                                // as zero so we don't apply multiple times
                                if (vQuantity > 1)
                                    vQuantity = 1;
                                if (vQuantity <= 0)
                                    blnGetFound = false;
                                else
                                    blnGetFound = true;

                                vTaxRate = Basket.TotalDiscountPriceTaxRate;

                                if (vValue > Basket.TotalDiscountPriceExTax)
                                {
                                    vExTax = -Basket.TotalDiscountPriceExTax;
                                    vIncTax = -Basket.TotalDiscountPriceIncTax;
                                }
                                else
                                {
                                    bool blnPricesExtax = false;

                                    if (!blnZeroTotalTaxRate)
                                    {
                                        if (GetKartConfig("general.tax.pricesinctax") == "y")
                                        {
                                            vExTax = -Math.Round(vValue * (1 / (1 + vTaxRate)), CurrencyRoundNumber);
                                            if (Basket.D_Tax == 1)
                                                vIncTax = -Math.Round(vValue, CurrencyRoundNumber);
                                            else
                                                vIncTax = vExTax;
                                        }
                                        else
                                            blnPricesExtax = true;
                                    }
                                    else
                                        blnPricesExtax = true;

                                    if (blnPricesExtax)
                                    {
                                        vIncTax = -Math.Round(vValue * (1 + vTaxRate), CurrencyRoundNumber);
                                        vExTax = -Math.Round(vValue, CurrencyRoundNumber);
                                    }
                                }

                                blnIsFixedValuePromo = true;
                                break;
                            }
                    }
                }

                if (blnGetFound)
                {
                    if (numTotalBasketAmount >= vSpend)
                        AddPromotion(ref Basket, blnGetFound, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate, blnIsFixedValuePromo, blnForceAdd);
                    else
                        AddPromotion(ref Basket, false, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate);
                }
            }
        }


        // get promotions from Basket version IDs (get parts)
        strList = "PP_PartNo='b' and PP_ItemType='v' and PP_Type='q' and PP_ItemID in (" + strItemVersionIDs + ")";
        drwGets = tblPromotions.Select(strList);

        foreach (DataRow drGet in drwGets)
        {
            vPromoID = drGet("PROM_ID");
            if (Strings.InStr(strPromoIDs, vPromoID) == 0)
            {
                strPromoIDs = strPromoIDs + vPromoID + ";";
                Promotion objPromotion = new Promotion();
                SetPromotionData(ref objPromotion, drGet);
                AddPromotion(ref Basket, false, ref strPromoDiscountIDs, ref objPromotion, vPromoID, vIncTax, vExTax, vQuantity, vTaxRate);
            }
        }

        for (int i = 1; i <= Basket.objPromotionsDiscount.Count; i++)
        {
            Basket.PromotionDiscount.ExTax = Basket.PromotionDiscount.ExTax + Basket.objPromotionsDiscount.Item(i - 1).TotalexTax;
            Basket.PromotionDiscount.IncTax = Basket.PromotionDiscount.IncTax + Basket.objPromotionsDiscount.Item(i - 1).TotalIncTax;
        }

        aryPromotions = Basket.objPromotions;
        aryPromotionsDiscount = Basket.objPromotionsDiscount;
    }

    /// <summary>
    ///     ''' Autosave basket
    ///     ''' We trigger this when adding items to the basket, or removing them.
    ///     ''' It creates a saved basket for the user called AUTOSAVE. We can
    ///     ''' recover this automatically when they next login.
    ///     ''' </summary>
    ///     ''' <param name="numCustomerID">The db ID of the customer</param>
    ///     ''' <remarks></remarks>
    public static void AutosaveBasket(ref Int32 numCustomerID)
    {
        if (numCustomerID > 0)
            BasketBLL.SaveBasket(numCustomerID, "AUTOSAVE", System.Web.HttpContext.Current.Session["SessionID"]);
    }

    /// <summary>
    ///     ''' Restore Autosaved basket
    ///     ''' When a user logs in, we can recover an autosaved basket. We
    ///     ''' only want to do this if the user's basket is blank, otherwise
    ///     ''' they might add items on a subsequent visit, but we'd then
    ///     ''' wipe these when they login or come to checkout.
    ///     ''' </summary>
    ///     ''' <param name="numCustomerID">The db ID of the customer</param>
    ///     ''' <remarks></remarks>
    public static void RecoverAutosaveBasket(ref Int32 numCustomerID)
    {

        // First we want to check if there are items in the basket already
        Kartris.Basket TestBasket = new Kartris.Basket();
        List<Kartris.BasketItem> TestBasketItems;

        // Load up basket from session
        TestBasket.LoadBasketItems();
        TestBasketItems = TestBasket.BasketItems;

        // Check number of items
        if (TestBasketItems.Count > 0)
            // This means the user has items in basket already.
            // In this case, we don't want to load items over the
            // top. In fact, we can save their current items as
            // an AUTOSAVE record!
            BasketBLL.AutosaveBasket(ref numCustomerID);
        else
            // No existing basket, let's recover the AUTOSAVE one
            // if it exists
            if (numCustomerID > 0)
            BasketBLL.LoadAutosaveBasket(numCustomerID, System.Web.HttpContext.Current.Session["SessionID"]);
    }

    /// <summary>
    ///     ''' Delete AUTOSAVE basket, or in fact, any basket by NAME and user ID
    ///     ''' </summary>
    ///     ''' <param name="UserID">User ID</param>
    ///     ''' <param name="SBSKT_Name">Basket name</param>
    ///     ''' <returns>True if ok, False if some error</returns>
    ///     ''' <remarks></remarks>
    public bool DeleteSavedBasketByNameAndUserID(int UserID, string SBSKT_Name)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddAttribute = sqlConn.CreateCommand;
            cmdAddAttribute.CommandText = "spKartrisBasket_DeleteSavedBasketByNameAndUserID";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddAttribute.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdAddAttribute.Parameters.AddWithValue("@UserID", UserID);
                cmdAddAttribute.Parameters.AddWithValue("@SBSKT_Name", SBSKT_Name);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddAttribute.Transaction = savePoint;

                cmdAddAttribute.ExecuteNonQuery();

                savePoint.Commit();
                sqlConn.Close();

                return true;
            }
            catch (Exception ex)
            {
                // ReportHandledError(ex, Reflection.MethodBase.GetCurrentMethod(), "Error")
                if (!savePoint == null)
                    savePoint.Rollback();
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close(); savePoint.Dispose();
                }
            }
        }
        return false;
    }



    /// <summary>
    ///     ''' Update the customer mail format
    ///     ''' </summary>
    ///     ''' <param name="numUserID">Customer ID</param>
    ///     ''' <param name="strMailFormat">Mail format (plain, html etc.)</param>
    ///     ''' <remarks></remarks>
    public static void UpdateCustomerMailFormat(int numUserID, string strMailFormat)
    {
        _CustomersAdptr.UpdateMailFormat(CkartrisBLL.GetLanguageIDfromSession, numUserID, strMailFormat);
    }

    /// <summary>
    ///     ''' Update settings for the mailing list for a given customer.
    ///     ''' </summary>
    ///     ''' <param name="strEmail"></param>
    ///     ''' <param name="strPassword"></param>
    ///     ''' <param name="strMailFormat"></param>
    ///     ''' <param name="strSignupIP"></param>
    ///     ''' <remarks></remarks>
    public static void UpdateCustomerMailingList(string strEmail, ref string strPassword, string strMailFormat = "t", string strSignupIP = "")
    {
        DateTime datSignup;
        int numPasswordLength;

        numPasswordLength = Val(KartSettingsManager.GetKartConfig("minimumcustomercodesize"));
        if (numPasswordLength == 0)
            numPasswordLength = 8;
        strPassword = GetRandomString(numPasswordLength);

        datSignup = CkartrisDisplayFunctions.NowOffset;

        _CustomersAdptr.UpdateMailingList(strEmail, datSignup, strSignupIP, strPassword, strMailFormat, CkartrisBLL.GetLanguageIDfromSession);
    }


    public static int ConfirmMail(int numUserID, string strPassword, string strIP = "")
    {
        UsersBLL objUsersBLL = new UsersBLL();
        DataTable tblConfirmMail = new DataTable();
        int UserID = 0;

        tblConfirmMail = _CustomersAdptr.ConfirmMail(numUserID, strPassword, CkartrisDisplayFunctions.NowOffset, strIP);

        if (tblConfirmMail.Rows.Count > 0)
            UserID = Val(tblConfirmMail.Rows(0).Item("UserID") + "");

        // If mailchimp is active, we want to add the user to the mailing list
        if (KartSettingsManager.GetKartConfig("general.mailchimp.enabled") == "y")
        {
            // Lookup user email
            string strEmail = objUsersBLL.GetEmailByID(UserID);
            AddListSubscriber(strEmail);
        }

        return UserID;
    }

    // Add user to MailChimp mailing list
    public static void AddListSubscriber(string strEmail)
    {
        MailChimpBLL manager = new MailChimpBLL();
        Member member = manager.AddListSubscriber(strEmail).Result();
    }

    /// <summary>
    ///     ''' Return Image Tag for a version/product to use in basket and on order email - if no version image, uses product image 
    ///     ''' </summary>
    ///     ''' <param name="numVersionID">The version we want to find images for</param>
    ///     ''' <param name="numProductID">The product we want to find images for</param>
    ///     ''' <returns>a string of html</returns>
    ///     ''' <remarks></remarks>
    public static string GetImageURL(long numVersionID, long numProductID)
    {
        string strImageURL = "";
        string strProductsFolderPath = HttpContext.Current.Server.MapPath(CkartrisImages.strProductImagesPath + "/" + numProductID + "/");
        string strVersionsFolderPath = strProductsFolderPath + "/" + numVersionID + "/";

        if (numVersionID > 0 & Directory.Exists(strVersionsFolderPath))
        {
            try
            {
                // we look for version image
                DirectoryInfo dirFolder = new DirectoryInfo(strVersionsFolderPath);

                if (dirFolder.GetFiles().Length < 1)
                    // folder found, but no images in it
                    strImageURL = "";
                else
                    try
                    {
                        foreach (var objFile in dirFolder.GetFiles())
                        {
                            strImageURL = CkartrisBLL.WebShopURL + "Image.ashx?strFileName=" + objFile.Name + "&amp;strItemType=v&amp;numMaxHeight=" + KartSettingsManager.GetKartConfig("frontend.display.images.minithumb.height") + "&amp;numMaxWidth=" + KartSettingsManager.GetKartConfig("frontend.display.images.minithumb.width") + "&amp;numItem=" + numVersionID + "&amp;strParent=" + numProductID;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        strImageURL = "";
                    }
            }
            catch (Exception ex)
            {
            }
        }

        // no version image found, so look for product images
        if (numProductID > 0 & strImageURL == "")
        {
            try
            {
                DirectoryInfo dirFolder = new DirectoryInfo(strProductsFolderPath);

                if (dirFolder.GetFiles().Length < 1)
                    // folder found, but no images in it
                    strImageURL = "";
                else
                    try
                    {
                        foreach (var objFile in dirFolder.GetFiles())
                        {
                            strImageURL = CkartrisBLL.WebShopURL + "Image.ashx?strFileName=" + objFile.Name + "&amp;strItemType=p&amp;numMaxHeight=" + KartSettingsManager.GetKartConfig("frontend.display.images.minithumb.height") + "&amp;numMaxWidth=" + KartSettingsManager.GetKartConfig("frontend.display.images.minithumb.width") + "&amp;numItem=" + numProductID + "&amp;strParent=0";
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        strImageURL = "";
                    }
            }
            catch (Exception ex)
            {
            }
        }

        return strImageURL;
    }
}
