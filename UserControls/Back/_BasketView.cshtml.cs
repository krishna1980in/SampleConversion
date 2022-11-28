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
using CkartrisBLL;
using KartSettingsManager;

partial class Back_BasketView : System.Web.UI.UserControl
{
    protected static double BSKT_CustomerDiscount;

    private static ArrayList arrPromotionsDiscount = new ArrayList();
    protected CurrenciesBLL objCurrency = new CurrenciesBLL();
    protected BasketItem objItem = new BasketItem();
    protected bool blnAdjusted;
    protected int SESS_CurrencyID;
    protected bool APP_PricesIncTax, APP_ShowTaxDisplay, APP_USMultiStateTax;

    private List<Kartris.Promotion> arrPromotions = new List<Kartris.Promotion>();
    private Interfaces.objShippingDetails _objShippingDetails;

    private BasketBLL.VIEW_TYPE _ViewType = BasketBLL.VIEW_TYPE.MAIN_BASKET;
    private bool _ViewOnly;
    private int _UserID; // we send in the customer ID
    private bool blnShowPromotion = true;

    public static kartris.Basket Basket
    {
        get
        {
            if (HttpContext.Current.Session("Basket") == null)
                HttpContext.Current.Session("Basket") = new Kartris.Basket();
            return HttpContext.Current.Session("Basket");
        }
    }

    public static List<Kartris.BasketItem> BasketItems
    {
        get
        {
            if (HttpContext.Current.Session("BasketItems") == null)
                HttpContext.Current.Session("BasketItems") = new List<Kartris.BasketItem>();
            return HttpContext.Current.Session("BasketItems");
        }
        set
        {
            HttpContext.Current.Session("BasketItems") = value;
        }
    }

    public kartris.Basket GetBasket
    {
        get
        {
            return Basket;
        }
    }

    public List<Kartris.BasketItem> GetBasketItems
    {
        get
        {
            return BasketItems;
        }
    }

    public ArrayList GetPromotionsDiscount
    {
        get
        {
            return arrPromotionsDiscount;
        }
    }

    public BasketBLL.VIEW_TYPE ViewType
    {
        get
        {
            return _ViewType;
        }
        set
        {
            _ViewType = value;
        }
    }

    public bool ViewOnly
    {
        get
        {
            return _ViewOnly;
        }
        set
        {
            _ViewOnly = value;
        }
    }

    public int UserID
    {
        set
        {
            _UserID = value;
        }
    }

    public Interfaces.objShippingDetails ShippingDetails
    {
        get
        {
            _objShippingDetails = System.Web.UI.UserControl.Session["_ShippingDetails"];
            if (_objShippingDetails != null)
            {
                _objShippingDetails.ShippableItemsTotalWeight = Basket.ShippingTotalWeight;
                _objShippingDetails.ShippableItemsTotalPrice = Basket.ShippingTotalExTax;
            }
            return _objShippingDetails;
        }
        set
        {
            _objShippingDetails = value;
            _objShippingDetails.ShippableItemsTotalWeight = Basket.ShippingTotalWeight;
            _objShippingDetails.ShippableItemsTotalPrice = Basket.ShippingTotalExTax;
            System.Web.UI.UserControl.Session["_ShippingDetails"] = _objShippingDetails;
        }
    }

    public int ShippingDestinationID
    {
        get
        {
            if (System.Web.UI.UserControl.Session["_ShippingDestinationID"] == null)
                return 0;
            else
                return System.Web.UI.UserControl.Session["_ShippingDestinationID"];
        }
        set
        {
            System.Web.UI.UserControl.Session["_ShippingDestinationID"] = value;
            System.Web.UI.UserControl.Session["numShippingCountryID"] = value;
        }
    }

    public double ShippingBoundary
    {
        get
        {
            if (GetKartConfig("frontend.checkout.shipping.calcbyweight") == "y")
                return Basket.ShippingTotalWeight;
            else if (KartSettingsManager.GetKartConfig("general.tax.pricesinctax") == "y")
                return Basket.ShippingTotalExTax;
            else
                return Basket.ShippingTotalIncTax;
        }
    }

    public int SelectedShippingID
    {
        get
        {
            return UC_ShippingMethodsDropdown.SelectedShippingID;
        }
    }

    public double SelectedShippingAmount
    {
        get
        {
            return UC_ShippingMethodsDropdown.SelectedShippingAmount;
        }
    }
    public string SelectedShippingMethod
    {
        get
        {
            return Basket.ShippingName;
        }
    }

    private string CouponCode
    {
        get
        {
            return System.Web.UI.UserControl.Session["CouponCode"] + "";
        }
        set
        {
            System.Web.UI.UserControl.Session["CouponCode"] = value;
        }
    }

    public event ItemQuantityChangedEventHandler ItemQuantityChanged;

    public delegate void ItemQuantityChangedEventHandler();

    public void LoadBasket()
    {
        SESS_CurrencyID = System.Web.UI.UserControl.Session["CUR_ID"];
        bool blnIsInCheckout = true;

        Basket.DB_C_CustomerID = _UserID;

        Basket.LoadBasketItems();
        BasketItems = Basket.BasketItems;

        if (BasketItems.Count == 0)
            litBasketEmpty.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("Basket", "ContentText_BasketEmpty");
        else
        {
            litBasketEmpty.Text = "";
            phdBasket.Visible = true;
        }

        Basket.Validate(false);
        rptBasket.DataSource = BasketItems;
        rptBasket.DataBind();

        Basket.CalculateTotals();
        BasketBLL.CalculatePromotions(Basket, arrPromotions, arrPromotionsDiscount, (APP_PricesIncTax == false & APP_ShowTaxDisplay == false));

        string strCouponError = "";
        BasketBLL.CalculateCoupon(Basket, CouponCode + "", strCouponError, (APP_PricesIncTax == false & APP_ShowTaxDisplay == false));

        BSKT_CustomerDiscount = BasketBLL.GetCustomerDiscount(_UserID);
        BasketBLL.CalculateCustomerDiscount(Basket, BSKT_CustomerDiscount);
        BasketBLL.CalculateOrderHandlingCharge(Basket, System.Web.UI.UserControl.Session["numShippingCountryID"]);

        phdShipping.Visible = true;
        UC_ShippingMethodsDropdown.Visible = true;

        if (blnIsInCheckout)
            SetShipping(UC_ShippingMethodsDropdown.SelectedShippingID, UC_ShippingMethodsDropdown.SelectedShippingAmount, ShippingDestinationID);

        if (Basket.OrderHandlingPrice.IncTax == 0)
            phdOrderHandling.Visible = false;
        else
            phdOrderHandling.Visible = true;
        try
        {
            phdOutOfStockElectronic.Visible = Basket.AdjustedForElectronic;
            phdOutOfStock.Visible = Basket.AdjustedQuantities;
        }
        catch (Exception ex)
        {
        }
        System.Web.UI.UserControl.Session["Basket"] = Basket;
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        SESS_CurrencyID = System.Web.UI.UserControl.Session["CUR_ID"];

        if (ConfigurationManager.AppSettings("TaxRegime").ToLower == "us" | ConfigurationManager.AppSettings("TaxRegime").ToLower == "simple")
        {
            APP_PricesIncTax = false;
            APP_ShowTaxDisplay = false;
            APP_USMultiStateTax = true;
        }
        else
        {
            APP_PricesIncTax = LCase(GetKartConfig("general.tax.pricesinctax")) == "y";
            // For checkout, we show tax if showtax is set to 'y' or 'c'.
            // For other pages, only if it is set to 'y'.
            APP_ShowTaxDisplay = LCase(GetKartConfig("frontend.display.showtax")) == "y";
            APP_USMultiStateTax = false;
        }

        litError.Text = "";

        if (!(System.Web.UI.UserControl.IsPostBack))
            LoadBasket();
        else if (Basket.OrderHandlingPrice.IncTax == 0)
            phdOrderHandling.Visible = false;
        else
            phdOrderHandling.Visible = true;

        updPnlMainBasket.Update();
    }

    protected void Page_PreRender(object sender, System.EventArgs e)
    {
        if (ViewType == BasketBLL.VIEW_TYPE.MAIN_BASKET)
        {
            phdMainBasket.Visible = true;
            phdControls.Visible = true;
            phdBasketButtons.Visible = true;
        }
        else if (ViewType == BasketBLL.VIEW_TYPE.CHECKOUT_BASKET)
        {
            phdControls.Visible = false;

            if (!_ViewOnly)
                phdShippingSelection.Visible = true;
        }

        if (ViewType == BasketBLL.VIEW_TYPE.CHECKOUT_BASKET & ViewOnly)
        {
            rptBasket.DataSource = BasketItems;
            rptBasket.DataBind();
        }

        phdMainBasket.Visible = true;
        phdControls.Visible = true;
        phdBasketButtons.Visible = true;

        blnShowPromotion = true;
        phdPromotions.Visible = true;

        rptPromotions.DataSource = arrPromotions;
        rptPromotions.DataBind();
        phdPromotions.Visible = Interaction.IIf(arrPromotions.Count > 0 & blnShowPromotion, true, false);

        rptPromotionDiscount.DataSource = arrPromotionsDiscount;
        rptPromotionDiscount.DataBind();
        phdPromotionDiscountHeader.Visible = IIf(arrPromotionsDiscount.Count > 0, true, false);

        if (BasketItems == null || BasketItems.Count == 0)
            phdBasket.Visible = false;

        if (UC_ShippingMethodsDropdown.SelectedShippingID > 0)
        {
            phdShippingTax.Visible = true;
            phdShippingTaxHide.Visible = false;
        }
        else
        {
            phdShippingTax.Visible = false;
            phdShippingTaxHide.Visible = true;
        }

        System.Web.UI.UserControl.Session["Basket"] = Basket;
    }

    public void QuantityChanged(object Sender, EventArgs Args)
    {
        TextBox objQuantity = Sender.Parent.FindControl("txtQuantity");
        HiddenField objBasketID = Sender.Parent.FindControl("hdfBasketID");

        if (!IsNumeric(objQuantity.Text))
        {
            objQuantity.Text = "0";
            objQuantity.Focus();
        }
        else
            try
            {
                BasketBLL.UpdateQuantity(System.Convert.ToInt32(objBasketID.Value), System.Convert.ToSingle(objQuantity.Text));
            }
            catch (Exception ex)
            {
            }

        LoadBasket();

        try
        {
            phdOutOfStockElectronic.Visible = Basket.AdjustedForElectronic;
            phdOutOfStock.Visible = Basket.AdjustedQuantities;
        }
        catch (Exception ex)
        {
        }

        updPnlMainBasket.Update();
    }

    public void RemoveItem_Click(object Sender, CommandEventArgs E)
    {
        long numItemID;
        string strArgument;
        int numRemoveVersionID = 0;

        strArgument = E.CommandArgument;

        string[] arrArguments = strArgument.Split(";");
        numItemID = System.Convert.ToInt64(arrArguments[0]);
        numRemoveVersionID = System.Convert.ToInt32(arrArguments[1]);

        BasketBLL.DeleteBasketItems(System.Convert.ToInt64(numItemID));

        LoadBasket();

        updPnlMainBasket.Update();
    }

    public void RemoveCoupon_Click(object Sender, CommandEventArgs E)
    {
        string strCouponError = "";

        Basket.CalculateTotals();

        BasketBLL.CalculatePromotions(Basket, arrPromotions, arrPromotionsDiscount, (APP_PricesIncTax == false & APP_ShowTaxDisplay == false));

        System.Web.UI.UserControl.Session["CouponCode"] = "";
        BasketBLL.CalculateCoupon(Basket, "", strCouponError, (APP_PricesIncTax == false & APP_ShowTaxDisplay == false));

        BSKT_CustomerDiscount = BasketBLL.GetCustomerDiscount(_UserID);
        BasketBLL.CalculateCustomerDiscount(Basket, BSKT_CustomerDiscount);

        BasketBLL.CalculateOrderHandlingCharge(Basket, System.Web.UI.UserControl.Session["numShippingCountryID"]);

        updPnlMainBasket.Update();
    }

    public void CustomText_Click(object sender, CommandEventArgs e)
    {
        long numItemID;
        double numCustomCost;
        string strCustomType = "";

        numItemID = e.CommandArgument;
        hidBasketID.Value = numItemID;

        foreach (BasketItem objItem in BasketItems)
        {
            if (objItem.ID == numItemID)
            {
                numCustomCost = objItem.CustomCost;
                litCustomCost.Text = CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], objItem.CustomCost);
                lblCustomDesc.Text = objItem.CustomDesc;
                hidCustomVersionID.Value = objItem.VersionID;
                txtCustomText.Text = objItem.CustomText;
                strCustomType = objItem.CustomType;
            }
        }

        // Hide cost line if zero
        if (numCustomCost == 0)
            phdCustomizationPrice.Visible = false;
        else
            phdCustomizationPrice.Visible = true;

        // Text Customization (Required)
        // Hide certain fields
        if (strCustomType == "r")
        {
            valCustomText.Visible = true;
            phdCustomizationCancel.Visible = false;
        }
        else
        {
            valCustomText.Visible = false;
            phdCustomizationCancel.Visible = true;
        }

        popCustomText.Show();
        updPnlCustomText.Update();
    }

    public void ProductName_Click(object sender, CommandEventArgs e)
    {
        string strItemInfo;

        strItemInfo = e.CommandArgument;

        if (strItemInfo != "")
        {
            try
            {
                string[] arrInfo = Strings.Split(strItemInfo, ";");
                if (arrInfo[Information.UBound(arrInfo)] != "o")
                    strItemInfo = "";
            }
            catch (Exception ex)
            {
            }
        }

        System.Web.UI.UserControl.Session["BasketItemInfo"] = strItemInfo;

        string strURL = e.CommandName;
        System.Web.UI.UserControl.Response.Redirect(strURL);
    }

    public void ApplyCoupon_Click(object Sender, CommandEventArgs E)
    {
        string strCouponError = "";

        Basket.CalculateTotals();

        BasketBLL.CalculatePromotions(Basket, arrPromotions, arrPromotionsDiscount, (APP_PricesIncTax == false & APP_ShowTaxDisplay == false));

        BasketBLL.CalculateCoupon(Basket, Trim(txtCouponCode.Text), strCouponError, (APP_PricesIncTax == false & APP_ShowTaxDisplay == false));
        if (strCouponError != "")
        {
            {
                var withBlock = popMessage;
                withBlock.SetTitle = System.Web.UI.TemplateControl.GetGlobalResourceObject("Basket", "PageTitle_ShoppingBasket");
                withBlock.SetTextMessage = strCouponError;
                withBlock.SetWidthHeight(300, 50);
                withBlock.ShowPopup();
            }
        }
        else
            System.Web.UI.UserControl.Session["CouponCode"] = Trim(txtCouponCode.Text);

        BSKT_CustomerDiscount = BasketBLL.GetCustomerDiscount(_UserID);
        BasketBLL.CalculateCustomerDiscount(Basket, BSKT_CustomerDiscount);

        BasketBLL.CalculateOrderHandlingCharge(Basket, System.Web.UI.UserControl.Session["numShippingCountryID"]);

        updPnlMainBasket.Update();
    }

    public void EmptyBasket_Click(object Sender, CommandEventArgs E)
    {
        BasketBLL.DeleteBasket();
        LoadBasket();
        updPnlMainBasket.Update();
    }

    private string GetProductIDs()
    {
        string strProductIDs = "";

        if (!(BasketItems == null))
        {
            foreach (BasketItem objItem in BasketItems)
                strProductIDs = strProductIDs + objItem.ProductID + ";";
        }

        return strProductIDs;
    }

    private BasketItem GetBasketItemByProductID(int numProductID)
    {
        foreach (BasketItem Item in BasketItems)
        {
            if (Item.ProductID == numProductID)
                return Item;
        }

        return null/* TODO Change to default(_) if this is not a reference type */;
    }

    public double ShippingTotalIncTax()
    {
        return Basket.ShippingTotalIncTax;
    }

    public void SetShipping(int numShippingID, double numShippingAmount, int numDestinationID)
    {
        if (ConfigurationManager.AppSettings("TaxRegime").ToLower == "us" | ConfigurationManager.AppSettings("TaxRegime").ToLower == "simple")
        {
            APP_PricesIncTax = false;
            APP_ShowTaxDisplay = false;
            APP_USMultiStateTax = true;
        }
        else
        {
            APP_PricesIncTax = LCase(GetKartConfig("general.tax.pricesinctax")) == "y";
            APP_ShowTaxDisplay = LCase(GetKartConfig("frontend.display.showtax")) == "y";
            APP_USMultiStateTax = false;
        }

        SESS_CurrencyID = System.Web.UI.UserControl.Session["CUR_ID"];

        System.Web.UI.UserControl.Session["numShippingCountryID"] = numDestinationID;
        Basket.CalculateShipping(Val(HttpContext.Current.Session("LANG")), numShippingID, numShippingAmount, System.Web.UI.UserControl.Session["numShippingCountryID"], ShippingDetails);

        BasketBLL.CalculateOrderHandlingCharge(Basket, System.Web.UI.UserControl.Session["numShippingCountryID"]);
        if (Basket.OrderHandlingPrice.IncTax == 0)
            phdOrderHandling.Visible = false;
        else
            phdOrderHandling.Visible = true;

        UpdatePromotionDiscount();

        Basket.Validate(false);

        Basket.CalculateTotals();

        BasketBLL.CalculatePromotions(Basket, arrPromotions, arrPromotionsDiscount, (APP_PricesIncTax == false & APP_ShowTaxDisplay == false));

        string strCouponError = "";
        BasketBLL.CalculateCoupon(Basket, System.Web.UI.UserControl.Session["CouponCode"] + "", strCouponError, (APP_PricesIncTax == false & APP_ShowTaxDisplay == false));

        BSKT_CustomerDiscount = BasketBLL.GetCustomerDiscount(_UserID);
        BasketBLL.CalculateCustomerDiscount(Basket, BSKT_CustomerDiscount);


        BasketItems = Basket.BasketItems;

        rptBasket.DataSource = BasketItems;
        rptBasket.DataBind();
        System.Web.UI.UserControl.Session["Basket"] = Basket;
    }

    private void UpdatePromotionDiscount()
    {
        foreach (PromotionBasketModifier objItem in arrPromotionsDiscount)
        {
            objItem.ApplyTax = Basket.ApplyTax;
            objItem.ExTax = objItem.ExTax;
            objItem.IncTax = objItem.IncTax;
        }

        rptPromotionDiscount.DataSource = arrPromotionsDiscount;
        rptPromotionDiscount.DataBind();
    }

    public void SetOrderHandlingCharge(int numShippingCountryID)
    {
        BasketBLL.CalculateOrderHandlingCharge(Basket, numShippingCountryID);
    }

    protected void rptBasket_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            objItem = e.Item.DataItem;

            if (LCase(objItem.ProductType) == "s")
                (PlaceHolder)e.Item.FindControl("phdProductType1").Visible = true;
            else
            {
                (PlaceHolder)e.Item.FindControl("phdProductType2").Visible = true;
                if (objItem.HasCombinations)
                    (PlaceHolder)e.Item.FindControl("phdItemHasCombinations").Visible = true;
                else
                    (PlaceHolder)e.Item.FindControl("phdItemHasNoCombinations").Visible = true;
            }

            string strURL = SiteMapHelper.CreateURL(SiteMapHelper.Page.CanonicalProduct, objItem.ProductID);
            if (strURL.Contains("?"))
                strURL = strURL + objItem.OptionLink;
            else
                strURL = strURL + Replace(objItem.OptionLink, "&", "?");
            (LinkButton)e.Item.FindControl("lnkBtnProductName").CommandName = strURL;

            if (LCase(objItem.CustomType) != "n")
            {
                (LinkButton)e.Item.FindControl("lnkCustomize").ToolTip = "(" + CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], objItem.CustomCost) + ")" + Constants.vbCrLf + objItem.CustomText;
                (LinkButton)e.Item.FindControl("lnkCustomize").Visible = true;
            }
            else
            {
                (LinkButton)e.Item.FindControl("lnkCustomize").ToolTip = "";
                (LinkButton)e.Item.FindControl("lnkCustomize").Visible = false;
            }

            if (objItem.AdjustedQty)
                (TextBox)e.Item.FindControl("txtQuantity").CssClass = "quantity_changed";
        }
    }

    protected void btnSaveCustomText_Click(object sender, System.EventArgs e)
    {
        long numSessionID, numBasketID, numVersionID;
        double numQuantity;
        string strCustomText, strOptions;

        if (IsNumeric(hidBasketID.Value))
            numBasketID = System.Convert.ToInt64(hidBasketID.Value);
        else
            numBasketID = 0;
        if (numBasketID > 0)
        {
            numVersionID = System.Convert.ToInt64(hidCustomVersionID.Value);
            strCustomText = Trim(txtCustomText.Text);
            strCustomText = CkartrisDisplayFunctions.StripHTML(strCustomText);

            BasketBLL.SaveCustomText(numBasketID, strCustomText);

            foreach (BasketItem objItem in BasketItems)
            {
                if (numBasketID == objItem.ID)
                    objItem.CustomText = strCustomText;
            }

            rptBasket.DataSource = BasketItems;
            rptBasket.DataBind();

            LoadBasket();

            updPnlMainBasket.Update();
        }
        else
        {
            numSessionID = System.Web.UI.UserControl.Session["SessionID"];
            numVersionID = System.Convert.ToInt64(hidCustomVersionID.Value);
            numQuantity = System.Convert.ToDouble(hidCustomQuantity.Value);
            strCustomText = Trim(txtCustomText.Text);
            strCustomText = CkartrisDisplayFunctions.StripHTML(strCustomText);
            strOptions = hidOptions.Value;
            numBasketID = hidOptionBasketID.Value;
            BasketBLL.AddNewBasketValue(BasketItems, BasketBLL.BASKET_PARENTS.BASKET, numSessionID, numVersionID, numQuantity, strCustomText, strOptions, numBasketID);

            ShowAddItemToBasket(numVersionID, numQuantity, true);
        }
    }

    private void SaveCustomText(string strCustomText, int numItemID = 0)
    {
        if (numItemID == 0)
            numItemID = btnSaveCustomText.CommandArgument;

        BasketBLL.SaveCustomText(numItemID, strCustomText);

        foreach (BasketItem objItem in BasketItems)
        {
            if (numItemID == objItem.ID)
                objItem.CustomText = strCustomText;
        }

        rptBasket.DataSource = BasketItems;
        rptBasket.DataBind();

        updPnlMainBasket.Update();
    }

    public void ShowCustomText(long numVersionID, double numQuantity, string strOptions = "", int numBasketValueID = 0)
    {
        string strCustomType;
        DataTable tblCustomization = BasketBLL.GetCustomization(numVersionID);
        long sessionID;

        if (tblCustomization.Rows.Count > 0)
        {
            strCustomType = LCase(tblCustomization.Rows(0).Item("V_CustomizationType"));
            if (strCustomType != "n")
            {
                if (strCustomType == "t" | strCustomType == "r")
                {
                    // Text Customization
                    // (Optional or Required)
                    hidBasketID.Value = "0";
                    hidOptionBasketID.Value = numBasketValueID;
                    hidCustomType.Value = strCustomType;
                    hidCustomVersionID.Value = numVersionID;
                    hidCustomQuantity.Value = numQuantity;
                    hidOptions.Value = strOptions;
                    litCustomCost.Text = CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], CurrenciesBLL.ConvertCurrency(System.Web.UI.UserControl.Session["CUR_ID"], tblCustomization.Rows(0).Item("V_CustomizationCost")));
                    lblCustomDesc.Text = tblCustomization.Rows(0).Item("V_CustomizationDesc") + "";

                    string strCustomText = "";
                    foreach (BasketItem objBasketItem in Basket.BasketItems)
                    {
                        if (objBasketItem.ID == numBasketValueID)
                        {
                            strCustomText = objBasketItem.CustomText;
                            break;
                        }
                    }
                    txtCustomText.Text = strCustomText;

                    // Hide cost line if zero
                    if (tblCustomization.Rows(0).Item("V_CustomizationCost") == 0)
                        phdCustomizationPrice.Visible = false;
                    else
                        phdCustomizationPrice.Visible = true;

                    // Text Customization (Required)
                    // Hide certain fields
                    if (strCustomType == "r")
                    {
                        valCustomText.Visible = true;
                        phdCustomizationCancel.Visible = false;
                    }
                    else
                    {
                        valCustomText.Visible = false;
                        phdCustomizationCancel.Visible = true;
                    }
                }
                popCustomText.Show();
                updPnlCustomText.Update();
            }
            else
            {
                sessionID = System.Web.UI.UserControl.Session["SessionID"];
                BasketBLL.AddNewBasketValue(BasketItems, BasketBLL.BASKET_PARENTS.BASKET, sessionID, numVersionID, numQuantity, "", strOptions, numBasketValueID);
                string strUpdateBasket = System.Web.UI.TemplateControl.GetGlobalResourceObject("Basket", "ContentText_ItemsUpdated");
                if (strUpdateBasket == "")
                    strUpdateBasket = "The item(s) were updated to your basket.";
                if (strOptions != "")
                    ShowAddItemToBasket(numVersionID, numQuantity, true);
                else
                    ShowAddItemToBasket(numVersionID, numQuantity);
            }
        }
    }

    private void ShowAddItemToBasket(int VersionID, double Quantity, bool blnDisplayPopup = false)
    {
        VersionsBLL objVersionsBLL = new VersionsBLL();
        DataTable tblVersion;
        tblVersion = objVersionsBLL._GetVersionByID(VersionID);
        if (tblVersion.Rows.Count > 0)
        {
            // '// add basket item quantity to new item qty and check for stock
            double numBasketQty = 0;
            foreach (BasketItem itmBasket in BasketItems)
            {
                if (VersionID == itmBasket.VersionID)
                {
                    numBasketQty = itmBasket.Quantity;
                    break;
                }
            }
            if (tblVersion.Rows(0).Item("V_QuantityWarnLevel") > 0 & (numBasketQty + Quantity) > tblVersion.Rows(0).Item("V_Quantity"))
            {
            }
        }

        // This section below we now only
        // need to run for options products
        string strBasketBehavior;
        strBasketBehavior = LCase(KartSettingsManager.GetKartConfig("frontend.basket.behaviour"));
    }

    public string SendOutputWithoutHTMLTags(string strInput)
    {
        string strNewString = "";

        for (int n = 1; n <= Strings.Len(strInput); n++)
        {
            if (Strings.Mid(strInput, n, 1) == "<")
            {
                while (Strings.Mid(strInput, n, 1) != ">")
                    n = n + 1;
                n = n + 1;
            }
            strNewString = strNewString + Strings.Mid(strInput, n, 1);
        }
        return strNewString;
    }

    protected void UC_ShippingMethodsDropdown_ShippingSelected(object sender, System.EventArgs e)
    {
        SetShipping(UC_ShippingMethodsDropdown.SelectedShippingID, UC_ShippingMethodsDropdown.SelectedShippingAmount, ShippingDestinationID);
        updPnlMainBasket.Update();
    }

    public void RefreshShippingMethods()
    {
        UC_ShippingMethodsDropdown.DestinationID = ShippingDestinationID;
        UC_ShippingMethodsDropdown.Boundary = ShippingBoundary;
        UC_ShippingMethodsDropdown.ShippingDetails = ShippingDetails;
        UC_ShippingMethodsDropdown.Refresh();

        SetShipping(UC_ShippingMethodsDropdown.SelectedShippingID, UC_ShippingMethodsDropdown.SelectedShippingAmount, ShippingDestinationID);

        updPnlMainBasket.Update();
    }

    protected void btnCancelCustomText_Click(object sender, System.EventArgs e)
    {
        long numSessionID;
        long numBasketID, numVersionID;
        double numQuantity;
        string strOptions;

        if (IsNumeric(hidBasketID.Value))
            numBasketID = System.Convert.ToInt64(hidBasketID.Value);
        else
            numBasketID = 0;

        if (numBasketID == 0)
        {
            numSessionID = System.Web.UI.UserControl.Session["SessionID"];
            numVersionID = System.Convert.ToInt64(hidCustomVersionID.Value);
            numQuantity = System.Convert.ToDouble(hidCustomQuantity.Value);
            strOptions = hidOptions.Value;

            BasketBLL.AddNewBasketValue(BasketItems, BasketBLL.BASKET_PARENTS.BASKET, numSessionID, numVersionID, numQuantity, "", strOptions);

            ShowAddItemToBasket(numVersionID, numQuantity);
        }
    }
}
