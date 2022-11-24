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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Threading;
using CkartrisDisplayFunctions;
using KartSettingsManager;

partial class UserControls_General_Invoice : System.Web.UI.UserControl
{
    protected bool APP_ShowTaxDisplay, blnHasCustomerDiscountExemption;
    protected float numColumns;
    private decimal numTotalExTax, numTotalTaxAmount, numTotal;
    private decimal numTotalPriceExTax, numTotalPriceIncTax, numRowTaxAmount;
    private decimal numPromoDiscountTotal, numCouponDiscountTotal;
    private bool blnTaxDue;
    private decimal CP_DiscountValue;
    private string strPromoDesc, strCouponCode, CP_CouponCode, CP_DiscountType;
    private double numDiscountPercentage;
    private string strShippingMethod;
    private decimal numShippingPriceExTax, numShippingPriceIncTax, numShippingTaxTotal;
    private decimal numOrderHandlingPriceExTax, numOrderHandlingPriceIncTax, numOrderHandlingTaxTotal;
    private decimal numFinalTotalPriceInTaxGateway;
    private short numOrderCurrency, numGatewayCurrency;
    private short numCurrencyRoundNumber;

    // v2.9010 added way to exclude items from customer discount
    // Often we have mods which require calculating a subtotal of 
    // cart items. We'll create variables here for this, but these
    // could be used for other custom mods in future requiring
    // similar.
    private decimal _SubTotalExTax, _SubTotalIncTax;

    private int _OrderLanguageID;
    private int _CustomerID;
    private int _OrderID;
    private string _FrontOrBack;


    public int CustomerID
    {
        set
        {
            _CustomerID = value;
        }
    }

    public int OrderID
    {
        set
        {
            _OrderID = value;
        }
    }

    public string FrontOrBack
    {
        set
        {
            _FrontOrBack = value;
        }
    }

    public string DashForBlankValue(string strInput)
    {
        if (Strings.Len(strInput) > 0)
            return strInput;
        else
            return ("-");
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        System.Data.DataTable tblInvoice;
        string strPONumber = "";
        string strVatNumber = "";
        DateTime datInvoiceDate;
        string strDefaultAddress, strBillingAddress, strShippingAddress;
        string strOrderComments = "";

        numCurrencyRoundNumber = BasketBLL.CurrencyRoundNumber;

        strDefaultAddress = ""; strBillingAddress = ""; strShippingAddress = "";
        BasketBLL objBasketBLL = new BasketBLL();
        tblInvoice = objBasketBLL.GetCustomerInvoice(_OrderID, _CustomerID, 0);

        if (tblInvoice.Rows.Count > 0)
        {
            strBillingAddress = tblInvoice.Rows[0].Item["O_BillingAddress"];
            strShippingAddress = tblInvoice.Rows[0].Item["O_ShippingAddress"];
            strPONumber = DashForBlankValue(tblInvoice.Rows[0].Item["O_PurchaseOrderNo"]);

            strVatNumber = DashForBlankValue(tblInvoice.Rows[0].Item["U_CardholderEUVATNum"] + "");
            datInvoiceDate = tblInvoice.Rows[0].Item["O_Date"];

            strBillingAddress = tblInvoice.Rows[0].Item["O_BillingAddress"];
            strShippingAddress = tblInvoice.Rows[0].Item["O_ShippingAddress"];

            // Let's see if can set comments
            try
            {
                strOrderComments = tblInvoice.Rows[0].Item["O_Comments"];
            }
            catch (Exception ex)
            {
            }

            _OrderLanguageID = tblInvoice.Rows[0].Item["O_LanguageID"];

            try
            {
                string strOrderCulture = System.Web.UI.UserControl.Server.HtmlEncode(LanguagesBLL.GetCultureByLanguageID_s(_OrderLanguageID));
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(strOrderCulture);
                Thread.CurrentThread.CurrentCulture = new CultureInfo(strOrderCulture);
            }
            catch (Exception ex)
            {
            }
        }

        if (tblInvoice.Rows.Count < 1)
        {
            // this error should only happen if a customer tries to
            // edit the querystring to view an invoice that does
            // not belong to them.
            System.Web.UI.UserControl.Response.Write("Unable to display this invoice to you");
            System.Web.UI.UserControl.Response.End();
        }
        for (int i = 0; i <= tblInvoice.Rows.Count - 1; i++)
        {
            numOrderCurrency = tblInvoice.Rows[i].Item["O_CurrencyID"];
            numGatewayCurrency = tblInvoice.Rows[i].Item["O_CurrencyIDGateway"];

            bool blnBitcoinGateway = (CurrenciesBLL.CurrencyCode(numGatewayCurrency).ToLower == "btc");
            if (blnBitcoinGateway)
                numFinalTotalPriceInTaxGateway = Math.Round(tblInvoice.Rows[0].Item["O_TotalPriceGateway"], 8);
            else
                numFinalTotalPriceInTaxGateway = Math.Round(tblInvoice.Rows[0].Item["O_TotalPriceGateway"], numCurrencyRoundNumber);
        }

        // Clean up addresses, put in line breaks
        strBillingAddress = Strings.Replace(strBillingAddress, Constants.vbCrLf + Constants.vbCrLf, Constants.vbCrLf);
        strBillingAddress = Strings.Replace(strBillingAddress + "", Constants.vbCrLf, "<br />" + Constants.vbCrLf);
        strShippingAddress = Strings.Replace(strShippingAddress, Constants.vbCrLf + Constants.vbCrLf, Constants.vbCrLf);
        strShippingAddress = Strings.Replace(strShippingAddress + "", Constants.vbCrLf, "<br />" + Constants.vbCrLf);

        // Remove phone number, last line of address
        Array aryShippingAddress = Strings.Split(strShippingAddress, "<br />");
        strShippingAddress = "";
        for (var numCounter = 0; numCounter <= Information.UBound(aryShippingAddress) - 1; numCounter++)
            strShippingAddress += aryShippingAddress(numCounter) + "<br />";

        strOrderComments = Strings.Replace(strOrderComments, Constants.vbCrLf + Constants.vbCrLf, Constants.vbCrLf);
        strOrderComments = Strings.Replace(strOrderComments + "", Constants.vbCrLf, "<br />" + Constants.vbCrLf);

        // Set shipping address to billing address if blank
        if (strShippingAddress == "")
            strShippingAddress = strBillingAddress;

        // Set values for controls on the page
        litBilling.Text = strBillingAddress;
        litShipping.Text = strShippingAddress;
        litOrderID.Text = _OrderID;
        litPONumber.Text = strPONumber;
        litCustomerID.Text = _CustomerID;
        litInvoiceDate.Text = FormatDate(datInvoiceDate, "d", _OrderLanguageID);
        litVatNumber.Text = strVatNumber;

        ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
        litEORINumber.Text = objObjectConfigBLL.GetValue("K:user.eori", _CustomerID);

        // MOD v3.0001
        // Add email to invoice
        UsersBLL objUsersBLL = new UsersBLL();
        litEmail.Text = objUsersBLL.GetEmailByID(_CustomerID);

        if (!string.IsNullOrWhiteSpace(strOrderComments) && KartSettingsManager.GetKartConfig("frontend.orders.showcommentsoninvoice") == "y")
        {
            (PlaceHolder)System.Web.UI.Control.FindControl("phdOrderComments").Visible = true;
            (Literal)System.Web.UI.Control.FindControl("litOrderComments").Text = strOrderComments;
        }

        // get sales receipt details
        tblInvoice.Dispose();

        tblInvoice = objBasketBLL.GetCustomerInvoice(_OrderID, _CustomerID, 1);

        rptInvoice.DataSource = tblInvoice;
        rptInvoice.DataBind();
    }

    protected void rptInvoice_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        string strVersionCode, strItemPriceTax;
        string strCustomizationOptionText;
        double numItemPriceIncTax, numItemPriceExTax, numItemPriceTax, numRowPriceExTax, numRowPriceIncTax;
        bool blnExcludeFromCustomerDiscount; // v2.9010 addition, allows items to be excluded from the % customer discount available to customer
        string strMark = "";

        // show tax if config says so
        APP_ShowTaxDisplay = (LCase(GetKartConfig("frontend.display.showtax")) == "y") | (LCase(GetKartConfig("frontend.display.showtax")) == "c");
        if (APP_ShowTaxDisplay == true)
            numColumns = 7;
        else
            numColumns = 4;

        if (e.Item.ItemType == ListItemType.Header)
            numPromoDiscountTotal = 0;
        else if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
        {
            ProductsBLL objProductsBLL = new ProductsBLL();
            VersionsBLL objVersionsBLL = new VersionsBLL();
            strVersionCode = e.Item.DataItem("IR_VersionCode");
            strCustomizationOptionText = e.Item.DataItem("IR_OptionsText");
            numTotal = e.Item.DataItem("O_TotalPrice");
            blnTaxDue = e.Item.DataItem("O_TaxDue");

            // get data for promotion discount
            numPromoDiscountTotal = e.Item.DataItem("O_PromotionDiscountTotal");
            strPromoDesc = e.Item.DataItem("O_PromotionDescription");

            // get data for coupon discount
            strCouponCode = e.Item.DataItem("O_CouponCode") + "";
            CP_DiscountValue = -Interaction.IIf(Information.IsDBNull(e.Item.DataItem("CP_DiscountValue")), 0, e.Item.DataItem("CP_DiscountValue"));
            CP_DiscountType = e.Item.DataItem("CP_DiscountType") + "";
            if (CP_DiscountType == "f")
                CP_DiscountValue = Interaction.IIf(Information.IsDBNull(e.Item.DataItem("O_CouponDiscountTotal")), 0, e.Item.DataItem("O_CouponDiscountTotal"));
            CP_CouponCode = e.Item.DataItem("CP_CouponCode") + "";
            numCouponDiscountTotal = e.Item.DataItem("O_CouponDiscountTotal");

            // get data for customer discount
            numDiscountPercentage = e.Item.DataItem("O_DiscountPercentage");

            // get data for shipping cost
            strShippingMethod = e.Item.DataItem("O_ShippingMethod") + "";
            numShippingPriceExTax = e.Item.DataItem("O_ShippingPrice");
            numShippingPriceIncTax = e.Item.DataItem("O_ShippingPrice") + e.Item.DataItem("O_ShippingTax");
            numShippingTaxTotal = e.Item.DataItem("O_ShippingTax");

            // get data for order handling
            numOrderHandlingPriceExTax = e.Item.DataItem("O_OrderHandlingCharge");
            numOrderHandlingPriceIncTax = e.Item.DataItem("O_OrderHandlingCharge") + e.Item.DataItem("O_OrderHandlingChargeTax");
            numOrderHandlingTaxTotal = e.Item.DataItem("O_OrderHandlingChargeTax");

            // set product/version name and customization text
            (Literal)e.Item.FindControl("litVersionCode").Text = strVersionCode;
            if (strCustomizationOptionText != "")
                (Literal)e.Item.FindControl("litCustomizationOptionText").Text = "<div>" + strCustomizationOptionText + "</div>";

            // Handle items that are exempt from customer discount
            blnExcludeFromCustomerDiscount = e.Item.DataItem("IR_ExcludeFromCustomerDiscount");

            // Totals
            if (e.Item.DataItem("O_PricesIncTax"))
            {
                // PRICES INCLUDING TAX
                numItemPriceExTax = Math.Round(e.Item.DataItem("IR_PricePerItem") - e.Item.DataItem("IR_TaxPerItem"), numCurrencyRoundNumber);
                numItemPriceIncTax = Math.Round(e.Item.DataItem("IR_PricePerItem"), numCurrencyRoundNumber);
                numItemPriceTax = e.Item.DataItem("IR_TaxPerItem");
                strItemPriceTax = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numItemPriceTax); // & " (" & (e.Item.DataItem("IR_TaxPerItem") / (e.Item.DataItem("IR_PricePerItem") - e.Item.DataItem("IR_TaxPerItem"))) & ")"
                numRowPriceExTax = e.Item.DataItem("IR_Quantity") * numItemPriceExTax;
                numRowPriceIncTax = Math.Round(e.Item.DataItem("IR_PricePerItem"), numCurrencyRoundNumber) * e.Item.DataItem("IR_Quantity");
            }
            else
            {
                // PRICES EXCLUDING TAX
                numItemPriceExTax = e.Item.DataItem("IR_PricePerItem");
                numItemPriceTax = e.Item.DataItem("IR_TaxPerItem");
                strItemPriceTax = (Math.Round(numItemPriceTax * 100, 4)) + "%";
                numRowPriceExTax = Math.Round(e.Item.DataItem("IR_PricePerItem") * e.Item.DataItem("IR_Quantity"), numCurrencyRoundNumber);
                // In following line, the 0.0000000001 is there to ensure tax is rounded up rather than down, if is 0.5
                numRowPriceIncTax = Math.Round(e.Item.DataItem("IR_PricePerItem") * e.Item.DataItem("IR_Quantity") * ((1 + e.Item.DataItem("IR_TaxPerItem")) + 0.0000000001), numCurrencyRoundNumber);
            }

            // Set marker for items that are excluded from customer discount
            if (blnExcludeFromCustomerDiscount)
            {
                blnHasCustomerDiscountExemption = true;
                strMark = " **";
                _SubTotalExTax = _SubTotalExTax + numRowPriceExTax;
                _SubTotalIncTax = _SubTotalIncTax + numRowPriceIncTax;
            }
            else
                strMark = "";

            numTotalPriceExTax = numTotalPriceExTax + numRowPriceExTax;
            numTotalPriceIncTax = numTotalPriceIncTax + numRowPriceIncTax;
            numRowTaxAmount = Math.Round(numRowPriceIncTax - numRowPriceExTax, numCurrencyRoundNumber);
            numTotalTaxAmount = numTotalTaxAmount + numRowTaxAmount;

            // Set the various controls to appropriate value
            (Literal)e.Item.FindControl("litVersionName").Text = e.Item.DataItem("IR_VersionName") + strMark;
            (Literal)e.Item.FindControl("litItemPriceExTax").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numItemPriceExTax);
            (Literal)e.Item.FindControl("litTaxPerItem").Text = strItemPriceTax;
            if (e.Item.DataItem("O_PricesIncTax"))
                (Literal)e.Item.FindControl("litItemPrice").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numItemPriceIncTax);
            else
                (Literal)e.Item.FindControl("litItemPrice").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numItemPriceExTax);
            (Literal)e.Item.FindControl("litQuantity").Text = System.Convert.ToSingle(e.Item.DataItem("IR_Quantity"));
            (Literal)e.Item.FindControl("litRowPriceExTax").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numRowPriceExTax);
            (Literal)e.Item.FindControl("litTaxAmount").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numRowTaxAmount);
            (Literal)e.Item.FindControl("litRowPriceIncTax").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numRowPriceIncTax);

            // -------------------------------------------------
            // MOD v3.0001
            // Extra rows for more info - Brexit related for UK clients
            (PlaceHolder)e.Item.FindControl("phdNonUKRows").Visible = KartSettingsManager.GetKartConfig("general.orders.extendedinvoiceinfo") == "y";
            (Literal)e.Item.FindControl("litDiscountedValue").Text = "";
            (Literal)e.Item.FindControl("litWeight").Text = objVersionsBLL._GetWeightByVersionCode(strVersionCode);
            (Literal)e.Item.FindControl("litDiscountedValue").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, ((100 - numDiscountPercentage) / 100) * numRowPriceExTax);

            // Commodity code, first we lookup product ID from the SKU, then use that
            // to see if any commodity code. 
            int numProductID = objProductsBLL.GetProductIDByVersionCode(strVersionCode);
            ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
            string strCommodityCode = objObjectConfigBLL.GetValue("K:product.commoditycode", numProductID);
            if (strCommodityCode != "")
            {
                (PlaceHolder)e.Item.FindControl("phdCommodityCode").Visible = true;
                (PlaceHolder)e.Item.FindControl("phdNoCommodityCode").Visible = false;
                (Literal)e.Item.FindControl("litCommodityCode").Text = strCommodityCode;
            }
        }
        else if (e.Item.ItemType == ListItemType.Footer)
        {
            double numTotalTaxFraction;
            double numPromotionDiscountIncTax, numPromotionDiscountExTax, numPromotionDiscountTaxAmount;
            double numCouponDiscountIncTax, numCouponDiscountExTax, numCouponDiscountTaxAmount, numCouponDiscountValue;
            double numCustomerDiscountIncTax, numCustomerDiscountExTax, numCustomerDiscountTaxAmount, numCustomerDiscountValue;
            double numShippingPrice, numOrderHandlingPrice;

            numTotalPriceIncTax = Math.Round(numTotalPriceIncTax, numCurrencyRoundNumber);
            numTotalPriceExTax = Math.Round(numTotalPriceExTax, numCurrencyRoundNumber);

            // promotion 
            if (numPromoDiscountTotal != 0)
            {
                numTotalTaxFraction = Interaction.IIf(numTotalPriceExTax == 0, 0, numTotalTaxAmount / (double)numTotalPriceExTax);
                numPromotionDiscountIncTax = (numPromoDiscountTotal);
                numPromotionDiscountExTax = numPromotionDiscountIncTax * (1 - (numTotalTaxFraction / (1 + numTotalTaxFraction)));
                numPromotionDiscountTaxAmount = numPromotionDiscountIncTax - numPromotionDiscountExTax;

                (PlaceHolder)e.Item.FindControl("phdPromotionDiscount").Visible = true;
                (Literal)e.Item.FindControl("litPromoDesc").Text = Strings.Replace(strPromoDesc, Constants.vbCrLf, "<br/>");
                (Literal)e.Item.FindControl("litPromoDiscountExTax").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numPromotionDiscountExTax);
                (Literal)e.Item.FindControl("litPromoTaxPerItem").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numPromotionDiscountTaxAmount);
                (Literal)e.Item.FindControl("litPromoItemPrice").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numPromotionDiscountIncTax);
                (Literal)e.Item.FindControl("litPromoDiscountTotal1").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numPromotionDiscountExTax);
                (Literal)e.Item.FindControl("litPromoDiscountTaxAmount").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numPromotionDiscountTaxAmount);
                (Literal)e.Item.FindControl("litPromoDiscountTotal2").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numPromoDiscountTotal);
            }

            // coupon discount
            if (strCouponCode != "")
            {
                numTotalTaxFraction = Math.Round(Interaction.IIf(numTotalPriceExTax == 0, 0, numTotalTaxAmount / (double)numTotalPriceExTax), 4);
                if (CP_DiscountType == "p")
                {
                    numCouponDiscountIncTax = Math.Round(CP_DiscountValue * (numTotalPriceIncTax + numPromotionDiscountIncTax) / 100, numCurrencyRoundNumber);
                    // 'numCouponDiscountIncTax = Math.Round(numCouponDiscountTotal, 2)
                    numCouponDiscountExTax = Math.Round(CP_DiscountValue * (numTotalPriceExTax + numPromotionDiscountExTax) / 100, numCurrencyRoundNumber);
                    // 'numCouponDiscountExTax = numCouponDiscountIncTax * (1 - (numTotalTaxFraction / (1 + numTotalTaxFraction)))
                    numCouponDiscountTaxAmount = numCouponDiscountIncTax - numCouponDiscountExTax;
                }
                else if (CP_DiscountType == "f")
                {
                    numCouponDiscountIncTax = Math.Round(CP_DiscountValue, numCurrencyRoundNumber);
                    numCouponDiscountExTax = Math.Round(numCouponDiscountIncTax * (1 - (numTotalTaxFraction / (1 + numTotalTaxFraction))), numCurrencyRoundNumber);
                    numCouponDiscountTaxAmount = numCouponDiscountIncTax - numCouponDiscountExTax;
                }
                else
                {
                    numCouponDiscountIncTax = 0;
                    numCouponDiscountExTax = 0;
                    numCouponDiscountTaxAmount = 0;
                }
            }

            numCouponDiscountValue = Interaction.IIf(blnTaxDue, numCouponDiscountIncTax, numCouponDiscountExTax);
            if (numCouponDiscountValue != 0)
            {
                (PlaceHolder)e.Item.FindControl("phdCouponDiscount").Visible = true;
                (Literal)e.Item.FindControl("litCouponCode").Text = CP_CouponCode;
                (Literal)e.Item.FindControl("litCouponDiscountExTax").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numCouponDiscountExTax);
                (Literal)e.Item.FindControl("litCouponDiscountTaxPerItem").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numCouponDiscountTaxAmount);
                (Literal)e.Item.FindControl("litCouponDiscountPrice").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numCouponDiscountIncTax);
                (Literal)e.Item.FindControl("litCouponDiscountTotal1").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numCouponDiscountExTax);
                (Literal)e.Item.FindControl("litCouponDiscountTaxAmount").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numCouponDiscountTaxAmount);
                (Literal)e.Item.FindControl("litCouponDiscountTotal2").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numCouponDiscountValue);
            }

            // customer discount
            if (numDiscountPercentage != 0)
            {
                numCustomerDiscountExTax = -Math.Round(((((numTotalPriceExTax - _SubTotalExTax) + numPromotionDiscountExTax + numCouponDiscountExTax) * (numDiscountPercentage / 100))), numCurrencyRoundNumber);
                numCustomerDiscountIncTax = -Math.Round((((numTotalPriceIncTax - _SubTotalIncTax) + numPromotionDiscountIncTax + numCouponDiscountIncTax) * (numDiscountPercentage / 100)), numCurrencyRoundNumber);
                numCustomerDiscountTaxAmount = numCustomerDiscountIncTax - numCustomerDiscountExTax;
            }

            numCustomerDiscountValue = Interaction.IIf(blnTaxDue, numCustomerDiscountIncTax, numCustomerDiscountExTax);
            if (numCustomerDiscountValue != 0 | blnHasCustomerDiscountExemption == true)
            {
                (PlaceHolder)e.Item.FindControl("phdCustomerDiscount").Visible = true;
                (Literal)e.Item.FindControl("litContentTextExcludedItems").Text = " (" + System.Web.UI.TemplateControl.GetGlobalResourceObject("Basket", "ContentText_SomeItemsExcludedFromDiscount") + ")";
                (Literal)e.Item.FindControl("litDiscountPercentage").Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("Basket", "ContentText_Discount") + " = " + numDiscountPercentage + "%<br />";
                (Literal)e.Item.FindControl("litCustomerDiscountExTax").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numCustomerDiscountExTax);
                (Literal)e.Item.FindControl("litCustomerDiscountTaxPerItem").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numCustomerDiscountTaxAmount);
                (Literal)e.Item.FindControl("litCustomerDiscountPrice").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numCustomerDiscountIncTax);
                (Literal)e.Item.FindControl("litCustomerDiscountTotal1").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numCustomerDiscountExTax);
                (Literal)e.Item.FindControl("litCustomerDiscountTaxAmount").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numCustomerDiscountTaxAmount);
                (Literal)e.Item.FindControl("litCustomerDiscountTotal2").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numCustomerDiscountValue);
            }

            // shipping cost
            numShippingPrice = Interaction.IIf(blnTaxDue, numShippingPriceIncTax, numShippingPriceExTax);
            // If numShippingPrice <> 0 Then 'we should always show shipping, even if zero
            (PlaceHolder)e.Item.FindControl("phdShippingCost").Visible = true;
            (Literal)e.Item.FindControl("litShippingMethod").Text = strShippingMethod;
            (Literal)e.Item.FindControl("litShippingPriceExTax").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numShippingPriceExTax);
            (Literal)e.Item.FindControl("litShippingTaxPerItem").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numShippingTaxTotal);
            (Literal)e.Item.FindControl("litShippingPrice").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numShippingPriceIncTax);
            (Literal)e.Item.FindControl("litShippingPriceTotal1").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numShippingPriceExTax);
            (Literal)e.Item.FindControl("litShippingTaxAmount").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numShippingTaxTotal);
            (Literal)e.Item.FindControl("litShippingPriceTotal2").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numShippingPrice);
            // End If

            // order handling charge
            numOrderHandlingPrice = Interaction.IIf(blnTaxDue, numOrderHandlingPriceIncTax, numOrderHandlingPriceExTax);
            if (numOrderHandlingPrice != 0)
            {
                (PlaceHolder)e.Item.FindControl("phdOrderHandlingCharge").Visible = true;
                (Literal)e.Item.FindControl("litOrderHandlingPriceExTax").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numOrderHandlingPriceExTax);
                (Literal)e.Item.FindControl("litOrderHandlingTaxPerItem").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numOrderHandlingTaxTotal);
                (Literal)e.Item.FindControl("litOrderHandlingPrice").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numOrderHandlingPriceIncTax);
                (Literal)e.Item.FindControl("litOrderHandlingPriceTotal1").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numOrderHandlingPriceExTax);
                (Literal)e.Item.FindControl("litOrderHandlingPriceTaxTotal").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numOrderHandlingTaxTotal);
                (Literal)e.Item.FindControl("litOrderHandlingPriceTotal2").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numOrderHandlingPrice);
            }

            // total
            numTotalExTax = numTotalPriceExTax + numPromotionDiscountExTax + numCouponDiscountExTax + numCustomerDiscountExTax + numShippingPriceExTax + numOrderHandlingPriceExTax;
            numTotalTaxAmount = numTotalTaxAmount + numPromotionDiscountTaxAmount + numCouponDiscountTaxAmount + numCustomerDiscountTaxAmount + numShippingTaxTotal + numOrderHandlingTaxTotal;
            numTotal = numTotalPriceIncTax + numPromotionDiscountIncTax + numCouponDiscountIncTax + numCustomerDiscountIncTax + numShippingPriceIncTax + numOrderHandlingPriceIncTax;
            (Literal)e.Item.FindControl("litTotalExTax").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numTotalExTax);
            (Literal)e.Item.FindControl("litTotalTaxAmount").Text = CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numTotalTaxAmount);

            // Add currency description row, separate from total so can keep final column
            // width reasonable
            LanguageElementsBLL objLanguageElementsBLL = new LanguageElementsBLL();
            (Literal)e.Item.FindControl("litCurrencyDescription").Text = "(" + CurrenciesBLL.CurrencyCode(numOrderCurrency) + " - " + objLanguageElementsBLL.GetElementValue(_OrderLanguageID, CkartrisEnumerations.LANG_ELEM_TABLE_TYPE.Currencies, CkartrisEnumerations.LANG_ELEM_FIELD_NAME.Name, numOrderCurrency) + ")";

            (Literal)e.Item.FindControl("litTotal").Text += CurrenciesBLL.FormatCurrencyPrice(numOrderCurrency, numTotal);

            if (numGatewayCurrency != numOrderCurrency)
            {
                (PlaceHolder)e.Item.FindControl("phdTotalGateway").Visible = true;
                (Literal)e.Item.FindControl("litTotalGateway").Text = "(" + CurrenciesBLL.CurrencyCode(numGatewayCurrency) + " - " + objLanguageElementsBLL.GetElementValue(_OrderLanguageID, CkartrisEnumerations.LANG_ELEM_TABLE_TYPE.Currencies, CkartrisEnumerations.LANG_ELEM_FIELD_NAME.Name, numGatewayCurrency) + ") ";
                (Literal)e.Item.FindControl("litTotalGateway").Text += CurrenciesBLL.FormatCurrencyPrice(numGatewayCurrency, numFinalTotalPriceInTaxGateway);
            }
        }
    }

    protected void Page_Error(object sender, System.EventArgs e)
    {
        CkartrisFormatErrors.LogError();
    }
}
