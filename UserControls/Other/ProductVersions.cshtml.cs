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
using CkartrisTaxes;
using CkartrisDataManipulation;
using CkartrisImages;
using KartSettingsManager;

/// <summary>

/// ''' Used in the ProductView.ascx and ProductTemplateExtended.ascx, to view the available versions

/// ''' of the current Product.

/// ''' </summary>

/// ''' <remarks></remarks>
partial class ProductVersions : System.Web.UI.UserControl
{
    private int _ProductID = -1;
    private short _LanguageID = -1;
    private char _ViewType;
    private Int64 numVersionID = -1;

    private bool c_blnHasPrices = true;
    private bool c_blnShowMediaGallery = true;

    protected long BasketItem_VersionID = 0;
    private static string BasketItemInfo = "";

    public int ProductID
    {
        get
        {
            return _ProductID;
        }
    }

    public bool HasPrice
    {
        get
        {
            return c_blnHasPrices;
        }
    }

    public bool ShowMediaGallery
    {
        get
        {
            return c_blnShowMediaGallery;
        }
        set
        {
            c_blnShowMediaGallery = value;
        }
    }

    /// <summary>
    ///     ''' Page load
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void Page_Load(object sender, System.EventArgs e)
    {
        VersionsBLL objVersionsBLL = new VersionsBLL();
        ddlVersionImages.Visible = false;
        try
        {
            phdDropdownCustomizable.Visible = objVersionsBLL.IsVersionCustomizable(System.Convert.ToInt64(ddlName_DropDown.SelectedValue));
        }
        catch
        {
        }
        if (System.Web.UI.UserControl.Request.QueryString["strOptions"] + "" == "")
            System.Web.UI.UserControl.Session["BasketItemInfo"] = "";
    }

    /// <summary>
    ///     ''' Returns boolean True if we need to hide add-to-basket button
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public bool CheckHideAddButton()
    {
        if (KartSettingsManager.GetKartConfig("frontend.users.access") == "partial" & !HttpContext.Current.User.Identity.IsAuthenticated)
            return true;
        else
            return false;
    }

    /// <summary>
    ///     ''' Loads/Creates the Display Type of the product version.
    ///     ''' </summary>
    ///     ''' <param name="pProductID"></param>
    ///     ''' <param name="pLanguageID"></param>
    ///     ''' <param name="pViewType"></param>
    ///     ''' <remarks></remarks>
    public void LoadProductVersions(int pProductID, short pLanguageID, char pViewType)
    {
        _ProductID = pProductID;
        _LanguageID = pLanguageID;

        // ' If the ProductID is -1, that means no Product was selected
        // ' the viwEmpty is activated and Exit.
        if (_ProductID == -1)
        {
            mvwVersion.SetActiveView(viwEmpty); return;
        }

        // ' If the viewType of the Versions is d(default), then will get the default
        // ' from the CONFIG Settings
        if (pViewType == "d")
            pViewType = GetKartConfig("frontend.versions.display.default");

        _ViewType = pViewType;

        short numCGroupID = 0;
        if (HttpContext.Current.User.Identity.IsAuthenticated)
            numCGroupID = System.Convert.ToInt16((PageBaseClass)System.Web.UI.Control.Page.CurrentLoggedUser.CustomerGroupID);

        DataTable tblVersions;
        VersionsBLL objVersionsBLL = new VersionsBLL();
        tblVersions = objVersionsBLL.GetByProduct(_ProductID, _LanguageID, numCGroupID);

        if (tblVersions.Rows.Count == 0)
        {
            mvwVersion.SetActiveView(viwError); return;
        }

        // Load the custom control for the product
        ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
        string strCustomControlName = objObjectConfigBLL.GetValue("K:product.customcontrolname", _ProductID);
        if (!string.IsNullOrEmpty(strCustomControlName))
        {
            mvwVersion.SetActiveView(viwCustomVersion);
            KartrisClasses.CustomProductControl UC_CustomControl = System.Web.UI.TemplateControl.LoadControl("~/UserControls/Custom/" + strCustomControlName);
            if (UC_CustomControl != null)
            {
                UC_CustomControl.ID = "UC_CustomControl";
                phdCustomControl.Controls.Add(UC_CustomControl);
            }

            if (!System.Web.UI.UserControl.IsPostBack)
            {
                ddlCustomVersionQuantity.Items.Clear();
                // Create custom control quantity dropdown values based on config setting
                for (var numCounter = 1; numCounter <= (int)KartSettingsManager.GetKartConfig("frontend.basket.addtobasketdropdown.max"); numCounter++)
                    ddlCustomVersionQuantity.Items.Add(numCounter.ToString());
                // Get the first available version for the product and set it as the custom product version ID
                litHiddenV_ID.Text = FixNullFromDB(tblVersions.Rows(0)("V_ID"));
            }
            // Exit Sub as this is a custom product and we don't want to load a different view from the code below
            return;
        }

        // ' Depending on the ViewType, the corresponding viewControl will be activated,
        // '  and the corresponding repeater as well.
        switch (pViewType)
        {
            case "l" // single version product
           :
                {
                    mvwVersion.SetActiveView(PrepareSelectView(tblVersions));
                    break;
                }

            case "r" // tabular rows
     :
                {
                    mvwVersion.SetActiveView(PrepareRowsView(tblVersions));
                    break;
                }

            case "p" // dropdown views
     :
                {
                    if (!System.Web.UI.Control.Page.IsPostBack)
                        mvwVersion.SetActiveView(PrepareDropDownView(tblVersions));
                    break;
                }

            case "o":
            case "g" // options
     :
                {
                    c_blnHasPrices = false;
                    mvwVersion.SetActiveView(PrepareOptionsView(tblVersions));
                    updOptions.Visible = ReturnCallForPrice(tblVersions.Rows(0)("V_ProductID"), tblVersions.Rows(0)("V_ID")) != 1;
                    numVersionID = tblVersions.Rows(0)("V_ID");
                    break;
                }

            default:
                {
                    c_blnHasPrices = false;
                    mvwVersion.SetActiveView(PrepareSelectView(tblVersions));
                    break;
                }
        }
    }

    /// <summary>
    ///     ''' Prepares the 'rows' version display
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private View PrepareRowsView(DataTable ptblVersions)
    {
        float numPrice = 0.0F;
        // ' Will move around all the resulted versions' rows and Convert the Pricing Currency.
        float numRRP = 0.0F;
        foreach (DataRow drwVersion in ptblVersions.Rows)
        {
            // '//
            drwVersion("V_Price") = GetPriceWithGroupDiscount(drwVersion("V_ID"), drwVersion("V_Price"));

            DataRow[] drwCurrencies = GetCurrenciesFromCache().Select("CUR_ID = " + System.Web.UI.UserControl.Session["CUR_ID"]);
            float numCalculatedTax = CalculateTax(Math.Round(System.Convert.ToDouble(FixNullFromDB(drwVersion("V_Price"))), drwCurrencies[0]("CUR_RoundNumbers")), System.Convert.ToDouble(FixNullFromDB(drwVersion("T_TaxRate"))));
            drwCurrencies = null;
            drwVersion("CalculatedTax") = System.Convert.ToString(CurrenciesBLL.ConvertCurrency(System.Web.UI.UserControl.Session["CUR_ID"], numCalculatedTax));

            numPrice = CurrenciesBLL.ConvertCurrency(System.Web.UI.UserControl.Session["CUR_ID"], System.Convert.ToDouble(FixNullFromDB(drwVersion("V_Price"))));
            drwVersion("V_Price") = System.Convert.ToString(numPrice);

            numRRP = CurrenciesBLL.ConvertCurrency(System.Web.UI.UserControl.Session["CUR_ID"], System.Convert.ToDouble(FixNullFromDB(drwVersion("V_RRP"))));
            drwVersion("V_RRP") = System.Convert.ToString(numRRP);
        }

        // ' Binds the DataTable to the Repeater.
        rptRows.DataSource = ptblVersions;
        rptRows.DataBind();

        // ' Returns the ID of the View Control to be activated.
        return viwVersionRows;
    }

    /// <summary>
    ///     ''' Prepares the 'dropdown' version display
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private View PrepareDropDownView(DataTable ptblVersions)
    {

        // ' Will move around all the resulted versions' rows and Convert the Pricing Currency.
        // '  and as this is the DropDownView .. every row will be added to the DropDownList.
        float numPrice = 0.0F;
        float numPrice2 = 0.0F;
        int intCounter = 0;
        int intFirstInStock = -1; // we use this so if there are out of stock items, we always pre-select an in-stock one
        string strV_Name = "";
        string strV_Price = "";

        ddlName_DropDown.Items.Clear();
        ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
        bool blnIsCallForPrice = (objObjectConfigBLL.GetValue("K:product.callforprice", _ProductID) == 1);

        if (blnIsCallForPrice)
        {
            foreach (DataRow drwVersion in ptblVersions.Rows)
            {
                // we want to format fixed length strings for:
                // - V_Name
                // - out of stock
                strV_Name = drwVersion("V_Name");
                // Need to check if out of stock
                if (drwVersion("V_Quantity") < 1.0F & drwVersion("V_QuantityWarnLevel") > 0.0F)
                {
                    // out of stock
                    ddlName_DropDown.Items.Add(new ListItem(strV_Name + " [" + System.Web.UI.TemplateControl.GetGlobalResourceObject("Versions", "ContentText_AltOutOfStock") + "]", drwVersion("V_ID")));
                    txtOutOfStockItems.Text += "," + intCounter + ",";
                }
                else
                {
                    // available
                    if (intFirstInStock == -1)
                        intFirstInStock = intCounter;
                    ddlName_DropDown.Items.Add(new ListItem(strV_Name, drwVersion("V_ID")));
                }

                intCounter += 1;
            }
            // Need to disable the AutoPostBack
            ddlName_DropDown.AutoPostBack = false;
            // Select the first in-stock item
            ddlName_DropDown.SelectedIndex = intFirstInStock;
            // Hide add button and dropdown, show call for price
            phdOutOfStock3.Visible = false;
            phdNotOutOfStock3.Visible = false;
            phdCalForPrice3.Visible = true;
        }
        else
        {
            foreach (DataRow drwVersion in ptblVersions.Rows)
            {
                // '//
                drwVersion("V_Price") = GetPriceWithGroupDiscount(drwVersion("V_ID"), drwVersion("V_Price"));
                DataRow[] drwCurrencies = GetCurrenciesFromCache().Select("CUR_ID = " + System.Web.UI.UserControl.Session["CUR_ID"]);
                float numCalculatedTax = CalculateTax(Math.Round(System.Convert.ToDouble(FixNullFromDB(drwVersion("V_Price"))), drwCurrencies[0]("CUR_RoundNumbers")), System.Convert.ToDouble(FixNullFromDB(drwVersion("T_TaxRate"))));

                numPrice = CurrenciesBLL.ConvertCurrency(System.Web.UI.UserControl.Session["CUR_ID"], System.Convert.ToDouble(drwVersion("V_Price")));
                numPrice2 = CurrenciesBLL.ConvertCurrency(System.Web.UI.UserControl.Session["CUR_ID"], System.Convert.ToDouble(numCalculatedTax));
                // we want to format fixed length strings for:
                // - V_Name
                // - V_Price
                // - out of stock

                strV_Name = drwVersion("V_Name") + " -- ";
                if (KartSettingsManager.GetKartConfig("frontend.display.showtax") == "y")
                {
                    // Show tax display
                    if (KartSettingsManager.GetKartConfig("general.tax.pricesinctax") == "y")
                        // ExTax / IncTax
                        strV_Price = " " + System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_ExTax") + " " + CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], numPrice2, null/* Conversion error: Set to default value for this argument */, false) + " -- " + System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_IncTax") + " " + CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], numPrice, null/* Conversion error: Set to default value for this argument */, false);
                    else
                        // litExTax / Tax%
                        strV_Price = " " + System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_ExTax") + " " + CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], numPrice, null/* Conversion error: Set to default value for this argument */, false) + " -- " + System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_Tax") + " " + CkartrisDisplayFunctions.FixDecimal(drwVersion("T_TaxRate")) + "%";
                }
                else
                    // Just show price
                    strV_Price = " " + CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], numPrice, null/* Conversion error: Set to default value for this argument */, false);

                // Need to check if out of stock
                if (drwVersion("V_Quantity") < 1.0F & drwVersion("V_QuantityWarnLevel") > 0.0F & KartSettingsManager.GetKartConfig("frontend.orders.allowpurchaseoutofstock") != "y")
                {
                    // out of stock
                    ddlName_DropDown.Items.Add(new ListItem(strV_Name + CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], numPrice, null/* Conversion error: Set to default value for this argument */, false) + " [" + System.Web.UI.TemplateControl.GetGlobalResourceObject("Versions", "ContentText_AltOutOfStock") + "]", drwVersion("V_ID")));
                    txtOutOfStockItems.Text += "," + intCounter + ",";
                }
                else if (ReturnCallForPrice(0, drwVersion("V_ID")) == 1)
                    ddlName_DropDown.Items.Add(new ListItem(strV_Name + System.Web.UI.TemplateControl.GetGlobalResourceObject("Versions", "ContentText_CallForPrice"), drwVersion("V_ID")));
                else
                {
                    // available
                    if (intFirstInStock == -1)
                        intFirstInStock = intCounter;
                    ddlName_DropDown.Items.Add(new ListItem(strV_Name + strV_Price, drwVersion("V_ID")));
                }
                intCounter += 1;
            }
            ddlName_DropDown.AutoPostBack = true;

            // Select the first in-stock item
            ddlName_DropDown.SelectedIndex = intFirstInStock;

            // First version might not be available to add to basket...
            CheckIfHidingButtons();
        }

        // ' Returns the ID of the View Control to be activated.
        return viwVersionDropDown;
    }

    // ''' <summary>
    // ''' Handles the change of the dropdown selection
    // ''' </summary>
    // ''' <remarks></remarks>
    protected void ddlName_DropDown_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        CheckIfHidingButtons();
    }

    // ''' <summary>
    // ''' Checks if need to hide 'add' button because item is 'call for price' or 'out of stock'
    // ''' </summary>
    // ''' <remarks></remarks>
    public void CheckIfHidingButtons()
    {

        // Handle out of stock stuff
        if (txtOutOfStockItems.Text.Contains("," + ddlName_DropDown.SelectedIndex + ","))
        {
            phdOutOfStock3.Visible = true;
            phdNotOutOfStock3.Visible = false;

            // Set the command argument for this item based
            // We use this for stock notifications popup
            string strVersionName = ddlName_DropDown.SelectedItem.Text;
            strVersionName = strVersionName.Substring(0, Math.Max(strVersionName.IndexOf(" --"), 0));
            btnNotifyMe3.CommandArgument = FormatStockNotificationDetails(ddlName_DropDown.SelectedItem.Value, _ProductID, strVersionName, System.Web.UI.UserControl.Request.RawUrl.ToString().ToLower(), System.Web.UI.UserControl.Session["LANG"]);
        }
        else
        {
            phdOutOfStock3.Visible = false;
            phdNotOutOfStock3.Visible = true;
            VersionsBLL objVersionsBLL = new VersionsBLL();
            phdDropdownCustomizable.Visible = objVersionsBLL.IsVersionCustomizable(System.Convert.ToInt64(ddlName_DropDown.SelectedValue));

            // In multi-version dropdown, if the [T] for text customization is not
            // visible, then we don't need to show the AJAX customization popup, so
            // can activate the client side 'add to basket' one.
            if (phdDropdownCustomizable.Visible == false)
                btnAddVersions3.OnClientClick = "ShowAddToBasketPopup(" + KartSettingsManager.GetKartConfig("frontend.basket.behaviour") * 1000 + ")";
            else
                btnAddVersions3.OnClientClick = null;
        }
    }

    /// <summary>
    ///     ''' Prepares the 'options' version display
    ///     ''' </summary>
    ///     ''' <remarks>by Paul</remarks>
    private View PrepareOptionsView(DataTable ptblVersions)
    {
        ProductsBLL objProductsBLL = new ProductsBLL();
        bool blnIncTax = IIf(KartSettingsManager.GetKartConfig("general.tax.pricesinctax") == "y", true, false);
        bool blnShowTax = IIf(KartSettingsManager.GetKartConfig("frontend.display.showtax") == "y", true, false);
        // Added check now for _NumberOfCombinations, so we don't
        // use this setting if it's an options product, not a combinations
        // product
        ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
        bool blnUseCombinationPrice = IIf(objObjectConfigBLL.GetValue("K:product.usecombinationprice", ProductID) == "1", true, false) & objProductsBLL._NumberOfCombinations(ProductID) > 0;
        ptblVersions.Rows(0)("V_Price") = GetPriceWithGroupDiscount(ptblVersions.Rows(0)("V_ID"), ptblVersions.Rows(0)("V_Price"));

        ptblVersions.Rows(0)("V_Price") = CurrenciesBLL.ConvertCurrency(System.Web.UI.UserControl.Session["CUR_ID"], System.Convert.ToDouble(System.Convert.ToString(FixNullFromDB(ptblVersions.Rows(0)("V_Price")))));
        // base version price and tax rate
        float pPrice = System.Convert.ToSingle(System.Convert.ToString(FixNullFromDB(ptblVersions.Rows(0)("V_Price"))));
        // store base price and tax
        litPriceHidden.Text = System.Convert.ToString(FixNullFromDB(ptblVersions.Rows(0)("V_Price")));
        if (ConfigurationManager.AppSettings("TaxRegime").ToLower != "us" & ConfigurationManager.AppSettings("TaxRegime").ToLower != "simple")
            litTaxRateHidden.Text = System.Convert.ToString(ptblVersions.Rows(0)("T_TaxRate"));

        // calculate display price
        PricePreview(pPrice, FixNullFromDB(ptblVersions.Rows(0)("V_ID")));

        lblVID_Options.Text = System.Convert.ToString(FixNullFromDB(ptblVersions.Rows(0)("V_ID")));

        // Initializes/Loads the OptionsContainer UC to view the Options that are available for the Product.
        UC_OptionsContainer.InitializeOption(_ProductID, _LanguageID, blnUseCombinationPrice);

        if (blnUseCombinationPrice)
            // Call the function to check the selected combination price
            GetCombinationPrice();
        else
            // We send in Version ID because we want to show 
            // Call For Price for both version and product
            // settings
            AddOptionsPrice(UC_OptionsContainer.GetSelectedPrice(), FixNullFromDB(ptblVersions.Rows(0)("V_ID")));

        // '// set addtobasket control's version id
        // If Not (IsPostBack) Then
        string strQuantityControl = LCase(objObjectConfigBLL.GetValue("K:product.addtobasketqty", ProductID));
        if (System.Web.UI.UserControl.Request.QueryString["strOptions"] != "" && (strQuantityControl == "dropdown" || strQuantityControl == "textbox"))
        {
            if (System.Web.UI.UserControl.Session["BasketItemInfo"] + "" != "")
            {
                string[] arrBasketItemInfo = Strings.Split(System.Web.UI.UserControl.Session["BasketItemInfo"] + "", ";");
                if (IsNumeric(lblVID_Options.Text))
                {
                    BasketItem_VersionID = System.Convert.ToInt64(lblVID_Options.Text);
                    UC_AddToBasketQty4.VersionID = BasketItem_VersionID;
                    UC_AddToBasketQty4.ItemEditVersionID = System.Convert.ToDouble(arrBasketItemInfo[1]);
                    UC_AddToBasketQty4.ItemEditQty = System.Convert.ToDouble(arrBasketItemInfo[2]);
                    btnAdd_Options.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "FormButton_Update");
                }
            }
        }
        else
        {
            if (IsNumeric(lblVID_Options.Text))
                UC_AddToBasketQty4.VersionID = System.Convert.ToInt64(lblVID_Options.Text);
            btnAdd_Options.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("Products", "FormButton_Add");
        }

        VersionsBLL objVersionsBLL = new VersionsBLL();
        phdOptionsCustomizable.Visible = objVersionsBLL.IsVersionCustomizable(System.Convert.ToInt64(FixNullFromDB(ptblVersions.Rows(0)("V_ID"))));

        if (UC_OptionsContainer.GetNoOfRows > 0)
            return viwVersionOptions;

        return viwError;
    }

    /// <summary>
    ///     ''' Prepares the 'select' version display
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private View PrepareSelectView(DataTable ptblVersions)
    {

        // ' Will move around all the resulted versions' rows and Convert the Pricing Currency.
        float numPrice = 0.0F;
        foreach (DataRow drwVersion in ptblVersions.Rows)
        {
            // '//
            drwVersion("V_Price") = GetPriceWithGroupDiscount(drwVersion("V_ID"), drwVersion("V_Price"));

            DataRow[] drwCurrencies = GetCurrenciesFromCache().Select("CUR_ID = " + System.Web.UI.UserControl.Session["CUR_ID"]);
            float numCalculatedTax = CalculateTax(Math.Round(System.Convert.ToDouble(FixNullFromDB(drwVersion("V_Price"))), drwCurrencies[0]("CUR_RoundNumbers")), System.Convert.ToDouble(FixNullFromDB(drwVersion("T_TaxRate"))));
            // Dim calculatedTax As Single = CalculateTax(CDbl(row("V_Price")), CDbl(FixNullFromDB(row("T_TaxRate"))))
            drwCurrencies = null;

            drwVersion("CalculatedTax") = System.Convert.ToString(CurrenciesBLL.ConvertCurrency(System.Web.UI.UserControl.Session["CUR_ID"], numCalculatedTax));

            numPrice = CurrenciesBLL.ConvertCurrency(System.Web.UI.UserControl.Session["CUR_ID"], System.Convert.ToDouble(FixNullFromDB(drwVersion("V_Price"))));
            drwVersion("V_Price") = System.Convert.ToString(numPrice);
        }

        // ' Binds the DataTable to the Repeater.
        fvwPrice.DataSource = ptblVersions;
        fvwPrice.DataBind();

        return viwSelect;
    }

    /// <summary>
    ///     ''' Runs for 'rows' display
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void rptRows_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
        foreach (Control ctlItem in e.Item.Controls)
        {
            switch (ctlItem.ID)
            {
                case "phdVersionImage":
                    {
                        e.Item.FindControl("UC_ImageViewer_Rows").Visible = true;
                        int numVersionID;
                        numVersionID = System.Convert.ToInt32((Label)e.Item.FindControl("lblVID_Rows").Text);

                        ImageViewer UC_ImageView = new ImageViewer();
                        UC_ImageView = (ImageViewer)e.Item.FindControl("UC_ImageViewer_Rows");

                        UC_ImageView.CreateImageViewer(IMAGE_TYPE.enum_VersionImage, numVersionID, KartSettingsManager.GetKartConfig("frontend.display.images.thumb.height"), KartSettingsManager.GetKartConfig("frontend.display.images.thumb.width"), "", _ProductID, ImageViewer.SmallImagesType.enum_ImageButton);

                        // Hide whole image and container if no image available, otherwise can end up
                        // with small square visible if there is a border and background set for the 
                        // image holder in CSS
                        if (UC_ImageView.FoundImage == false)
                            (PlaceHolder)e.Item.FindControl("phdVersionImage").Visible = false;
                        break;
                    }

                case "phdMediaGallery":
                    {
                        if (!c_blnShowMediaGallery)
                            e.Item.FindControl("UC_VersionMediaGallery").Visible = false;
                        break;
                    }

                case "litResultedPrice_Rows":
                    {
                        break;
                    }

                case "litResultedCalculatedTax_Rows":
                    {
                        break;
                    }

                case "litResultedIncTax_Rows":
                    {
                        break;
                    }

                case "litResultedExTax_Rows":
                    {
                        break;
                    }

                case "litResultedTaxRate_Rows":
                    {
                        break;
                    }

                case "updAddQty":
                    {
                        break;
                    }

                case "lblRRP_Rows":
                    {
                        // ' Replacing the RRP zero value by empty string.
                        if ((Label)e.Item.FindControl("lblRRP_Rows").Text == "0")
                            (Label)e.Item.FindControl("lblRRP_Rows").Text = "-";
                        break;
                    }

                case "lblStock_Rows":
                    {
                        // ' Replacing the Stock zero value by "-".
                        if ((Label)e.Item.FindControl("lblStock_Rows").Text == "0")
                            (Label)e.Item.FindControl("lblStock_Rows").Text = "-";
                        break;
                    }

                case "litRRP_Rows":
                    {
                        // ' Creating the Currency Format for the Version's Price.
                        float numRRP = System.Convert.ToSingle((Literal)e.Item.FindControl("litRRP_Rows").Text);
                        (Literal)e.Item.FindControl("litRRP_Rows").Text = CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], numRRP);
                        break;
                    }

                case "phdCustomizable":
                    {
                        if (System.Web.UI.UserControl.Session["BasketItemInfo"] != "" & System.Web.UI.UserControl.Request.QueryString["strOptions"] + "" != "")
                        {
                            string[] arrBasketInfo = Strings.Split(System.Web.UI.UserControl.Session["BasketItemInfo"], ";");
                            UserControls_General_AddToBasket UC_AddToBasket;
                            UC_AddToBasket = (UserControls_General_AddToBasket)e.Item.FindControl("UC_AddToBasketQty2");
                            UC_AddToBasket.ItemEditVersionID = arrBasketInfo[1];
                            UC_AddToBasket.ItemEditQty = arrBasketInfo[2];
                        }

                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }
    }

    /// <summary>
    ///     ''' Single version product display
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void fvwPrice_DataBound(object sender, System.EventArgs e)
    {
        try
        {
            // 'Show the panel with the correct price display
            if (KartSettingsManager.GetKartConfig("frontend.display.showtax") == "y")
            {
                if (KartSettingsManager.GetKartConfig("general.tax.pricesinctax") == "y")
                {

                    // 'Show inc/extax pricing
                    fvwPrice.FindControl("pnlExIncTax").Visible = true;

                    // 'Set the extax price (alongside inctax)
                    float numExTax1 = System.Convert.ToSingle((Literal)fvwPrice.FindControl("litCalculatedTax_Rows").Text);
                    (Literal)fvwPrice.FindControl("litResultedCalculatedTax_Rows").Text = CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], numExTax1);

                    // 'Set the inctax price
                    float numIncTax = System.Convert.ToSingle((Literal)fvwPrice.FindControl("litIncTax_Rows").Text);
                    (Literal)fvwPrice.FindControl("litResultedIncTax_Rows").Text = CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], numIncTax);
                }
                else
                {
                    // 'Show extax pricing with tax rate
                    fvwPrice.FindControl("pnlExTaxTax").Visible = true;

                    // 'Set the extax price (alongside tax rate)
                    float numExTax2 = System.Convert.ToSingle((Literal)fvwPrice.FindControl("litExTax_Rows").Text);
                    (Literal)fvwPrice.FindControl("litResultedExTax_Rows").Text = CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], numExTax2);

                    // 'Set the tax rate %
                    (Literal)fvwPrice.FindControl("litResultedTaxRate_Rows").Text = System.Convert.ToString((Literal)fvwPrice.FindControl("litTaxRate_Rows").Text) + " %";
                }
            }
            else
            {
                ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
                if (objObjectConfigBLL.GetValue("K:product.callforprice", _ProductID) != 1)
                {
                    // 'Show single price
                    fvwPrice.FindControl("pnlPrice").Visible = true;

                    // 'Set the single price
                    float numPrice = System.Convert.ToSingle((Literal)fvwPrice.FindControl("litPrice_Rows").Text);
                    (Literal)fvwPrice.FindControl("litResultedPrice_Rows").Text = CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], numPrice);
                }
            }
            // 'Set the RRP
            try
            {
                float numRRP = System.Convert.ToSingle((Literal)fvwPrice.FindControl("litRRP").Text);
                (Literal)fvwPrice.FindControl("litRRP").Text = CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], numRRP);
            }
            catch (Exception ex)
            {
            }
        }
        catch (Exception ex)
        {
        }
    }

    // ' Clears the saved selected images if any view was activated.
    protected void mvwVersion_ActiveViewChanged(object sender, System.EventArgs e)
    {
    }

    /// <summary>
    ///     ''' Handles add to basket button clicks for custom product versions
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void btnAddCustomVersions_Click(object sender, System.EventArgs e)
    {
        KartrisClasses.CustomProductControl UC_CustomControl = phdCustomControl.FindControl("UC_CustomControl");
        if (UC_CustomControl != null)
        {
            if (UC_CustomControl.ComputeFromSelectedOptions() == "success")
            {
                object objMiniBasket = System.Web.UI.Control.Page.Master.FindControl("UC_MiniBasket");
                Kartris.Basket objBasket = objMiniBasket.GetBasket;
                objMiniBasket.AddCustomItemToBasket(litHiddenV_ID.Text, ddlCustomVersionQuantity.SelectedValue, UC_CustomControl.ParameterValues + "|||" + UC_CustomControl.ItemDescription + "|||" + UC_CustomControl.ItemPrice);
            }
        }
    }

    /// <summary>
    ///     ''' Handles add to basket button clicks for dropdown versions
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void btnAddVersions3_Click(object sender, System.EventArgs e)
    {
        if (!txtOutOfStockItems.Text.Contains("," + ddlName_DropDown.SelectedIndex + ","))
        {
            float numQuantity = UC_AddToBasketQty3.ItemsQuantity;
            long numVersionID = ddlName_DropDown.SelectedValue;

            // ' Unit size should be checked if the quantity control is "textbox" => qty entered by user
            string strUnitSize = UC_AddToBasketQty3.UnitSize;

            // ' Check for wrong quantity
            int numNoOfDecimalPlacesForUnit = Interaction.IIf(strUnitSize.Contains("."), Strings.Mid(strUnitSize, strUnitSize.IndexOf(".") + 2).Length, 0);
            int numNoOfDecimalPlacesForQty = Interaction.IIf(System.Convert.ToString(numQuantity).Contains(".") && System.Convert.ToInt64(Strings.Mid(System.Convert.ToString(numQuantity), System.Convert.ToString(numQuantity).IndexOf(".") + 2)) != 0, Strings.Mid(System.Convert.ToString(numQuantity), System.Convert.ToString(numQuantity).IndexOf(".") + 2).Length, 0);
            int numMod = System.Convert.ToInt32(numQuantity * Math.Pow(10, numNoOfDecimalPlacesForQty)) % System.Convert.ToInt32(strUnitSize * Math.Pow(10, numNoOfDecimalPlacesForUnit));
            if (numMod != 0.0F || numNoOfDecimalPlacesForQty > numNoOfDecimalPlacesForUnit)
                // ' wrong quantity - quantity should be a multiplies of unit size
                AddWrongQuantity(System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_CorrectErrors"), Strings.Replace(System.Web.UI.TemplateControl.GetGlobalResourceObject("ObjectConfig", "ContentText_OrderMultiplesOfUnitsize"), "[unitsize]", strUnitSize));
            else
            {
                long sessionID = System.Web.UI.UserControl.Session["SessionID"];
                AddItemsToBasket(numVersionID, numQuantity);

                // v2.9010 Autosave basket
                try
                {
                    BasketBLL.AutosaveBasket((PageBaseClass)System.Web.UI.Control.Page.CurrentLoggedUser.ID);
                }
                catch (Exception ex)
                {
                }
            }
        }
        else
        {
            phdOutOfStock3.Visible = true;
            phdNotOutOfStock3.Visible = false;
        }
    }

    /// <summary>
    ///     ''' Handles click of the 'add' button for options products
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void btnAdd_Options_Click(object sender, System.EventArgs e)
    {
        string strNonSelectedOptions = UC_OptionsContainer.CheckForValidSelection();
        if (strNonSelectedOptions == null)
        {

            // ' Unit size should be checked if the quantity control is "textbox" => qty entered by user
            string strUnitSize = UC_AddToBasketQty4.UnitSize;
            float numQuantity = UC_AddToBasketQty4.ItemsQuantity;

            // ' Check for wrong quantity
            int numNoOfDecimalPlacesForUnit = Interaction.IIf(strUnitSize.Contains("."), Strings.Mid(strUnitSize, strUnitSize.IndexOf(".") + 2).Length, 0);
            int numNoOfDecimalPlacesForQty = Interaction.IIf(System.Convert.ToString(numQuantity).Contains(".") && System.Convert.ToInt64(Strings.Mid(System.Convert.ToString(numQuantity), System.Convert.ToString(numQuantity).IndexOf(".") + 2)) != 0, Strings.Mid(System.Convert.ToString(numQuantity), System.Convert.ToString(numQuantity).IndexOf(".") + 2).Length, 0);
            int numMod = System.Convert.ToInt32(numQuantity * Math.Pow(10, numNoOfDecimalPlacesForQty)) % System.Convert.ToInt32(strUnitSize * Math.Pow(10, numNoOfDecimalPlacesForUnit));
            if (numMod != 0.0F || numNoOfDecimalPlacesForQty > numNoOfDecimalPlacesForUnit)
                // ' wrong quantity - quantity should be a multiplies of unit size
                AddWrongQuantity(System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_CorrectErrors"), Strings.Replace(System.Web.UI.TemplateControl.GetGlobalResourceObject("ObjectConfig", "ContentText_OrderMultiplesOfUnitsize"), "[unitsize]", strUnitSize));
            else
            {
                // ' Reading the values of Options from the OptionsContainer in a muli-dimentional array
                string strOptionString = UC_OptionsContainer.GetSelectedOptions();

                if (String.IsNullOrEmpty(strOptionString))
                    strOptionString = "";
                VersionsBLL objVersionsBLL = new VersionsBLL();
                int numVersionID = objVersionsBLL.GetCombinationVersionID_s(_ProductID, strOptionString);
                if (numVersionID != 0)
                    lblVID_Options.Text = numVersionID;
                else
                    numVersionID = System.Convert.ToInt32(lblVID_Options.Text);

                AddItemsToBasket(numVersionID, numQuantity, strOptionString);

                // v2.9010 Autosave basket
                try
                {
                    BasketBLL.AutosaveBasket((PageBaseClass)System.Web.UI.Control.Page.CurrentLoggedUser.ID);
                }
                catch (Exception ex)
                {
                }
            }
        }
        else
        {
            string strOptionValidationMessage = null;

            while (strNonSelectedOptions.Contains(",,"))
                strNonSelectedOptions = Strings.Replace(strNonSelectedOptions, ",,", ",");
            strNonSelectedOptions = Strings.Replace(strNonSelectedOptions, ",", "<br />" + Constants.vbCrLf);

            strOptionValidationMessage = "<div>" + System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_OptionsMissingSelection") + "</div><p><strong>" + strNonSelectedOptions + "</strong></p>";

            UC_PopupMessage.SetTitle = System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_Alert");
            UC_PopupMessage.SetTextMessage = strOptionValidationMessage;
            UC_PopupMessage.SetWidthHeight(350, 200);
            UC_PopupMessage.ShowPopup();
            updOptionsContainer.Update();
        }
    }

    /// <summary>
    ///     ''' Handles options price change
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void UC_OptionsContainer_Event_OptionPriceChanged(float pOptionPrice)
    {
        // Reading the values of Options from the OptionsContainer in a muli-dimentional array
        string strOptionsList = UC_OptionsContainer.GetSelectedOptions();
        CleanOptionString(ref strOptionsList);

        if (UC_OptionsContainer.IsUsingCombinationPrices)
            GetCombinationPrice();
        else
            // We send in Version ID because we want to show 
            // Call For Price for both version and product
            // settings
            AddOptionsPrice(pOptionPrice, numVersionID);

        if (!String.IsNullOrEmpty(strOptionsList))
        {
            VersionsBLL objVersionsBLL = new VersionsBLL();
            numVersionID = objVersionsBLL.GetCombinationVersionID_s(ProductID, strOptionsList);
            if (numVersionID != -1)
                CheckCombinationsImages(numVersionID);
        }
    }

    /// <summary>
    ///     ''' Check if there is a combination image for the selected item
    ///     ''' </summary>
    public void CheckCombinationsImages(Int64 numVersionID)
    {
        List<string> extensions = new List<string>();
        extensions.Add("*.png");
        extensions.Add("*.jpg");
        extensions.Add("*.jpeg");

        // Let's find and create some objects we'll need to manipulate
        UpdatePanel updProduct;
        UpdatePanel updImages;
        PlaceHolder phdMainImages;
        PlaceHolder phdCombinationImage;
        Literal litTest;

        updProduct = this.Parent.Parent.Parent.Parent.Parent as UpdatePanel;
        updImages = updProduct.FindControl("updImages") as UpdatePanel;
        phdMainImages = updProduct.FindControl("phdMainImages") as PlaceHolder;
        phdCombinationImage = updProduct.FindControl("phdCombinationImage") as PlaceHolder;

        litTest = updProduct.FindControl("litTest") as Literal;

        // phdCombinationImage.Controls.Clear()

        try
        {
            string versionImagesPath = HttpContext.Current.Server.MapPath("~") + @"\Images\Products\" + ProductID + @"\" + numVersionID;
            if (Directory.Exists(versionImagesPath))
            {
                int fileCount;
                for (int i = 0; i <= extensions.Count - 1; i++)
                    fileCount += Directory.GetFiles(versionImagesPath, extensions[i], SearchOption.AllDirectories).Length;

                // updImages

                if (fileCount > 0)
                {
                    // Now let's set the image control to find a combination image,
                    // if it exists
                    ImageViewer UC_ImageView = new ImageViewer();


                    UC_ImageView = (ImageViewer)updImages.FindControl("UC_CombinationImage");
                    UC_ImageView.ClearImages();
                    UC_ImageView.CreateImageViewer(IMAGE_TYPE.enum_VersionImage, numVersionID, KartSettingsManager.GetKartConfig("frontend.display.images.normal.height"), KartSettingsManager.GetKartConfig("frontend.display.images.normal.width"), "", _ProductID, ImageViewer.SmallImagesType.enum_ImageButton);

                    // Hide whole image and container if no image available, otherwise can end up
                    // with small square visible if there is a border and background set for the 
                    // image holder in CSS
                    if (numVersionID > -1)
                    {
                        litTest.Text = numVersionID + " --- " + DateTime.Now();
                        phdCombinationImage.Visible = true;
                        phdMainImages.Visible = false;
                    }
                    else
                    {
                        phdCombinationImage.Visible = false;
                        phdMainImages.Visible = true;
                    }

                    updImages.Update();
                }
                else
                {
                    phdCombinationImage.Visible = false;
                    phdMainImages.Visible = true;
                }
            }
            else
            {
                phdCombinationImage.Visible = false;
                phdMainImages.Visible = true;
            }
        }
        catch (Exception ex)
        {
        }
    }

    /// <summary>
    ///     ''' Calculate options price change
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void AddOptionsPrice(float pOptionPrice, Int32 numV_ID = 0)
    {

        // We send in Version ID because we want to show 
        // Call For Price for both version and product
        // settings

        System.Web.UI.UserControl.Trace.Warn("UC_OptionsContainerEvent IN Versions");
        Event_VersionPriceChanged?.Invoke(pOptionPrice);

        Literal litPriceTemp = new Literal();
        litPriceTemp = (Literal)System.Web.UI.Control.FindControl("litPriceHidden");

        float numOldPrice;
        numOldPrice = System.Convert.ToSingle(litPriceTemp.Text);
        float numNewPrice;
        numNewPrice = numOldPrice + System.Convert.ToDouble(pOptionPrice);

        // Reading the values of Options from the OptionsContainer in a muli-dimentional array
        string strOptionString = UC_OptionsContainer.GetSelectedOptions();
        if (numV_ID != 0)
            PricePreview(numNewPrice, numV_ID);

        CheckOptionStock(strOptionString);
        updPricePanel.Update();
        updOptions.Update();
    }

    /// <summary>
    ///     ''' Get price of combination
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void GetCombinationPrice()
    {
        // Reading the values of Options from the OptionsContainer in a muli-dimentional array
        string strOptionsList = UC_OptionsContainer.GetSelectedOptions();
        CleanOptionString(ref strOptionsList);
        if (!String.IsNullOrEmpty(strOptionsList))
        {
            VersionsBLL objVersionsBLL = new VersionsBLL();
            float numPrice = objVersionsBLL.GetCombinationPrice(ProductID, strOptionsList);
            numVersionID = objVersionsBLL.GetCombinationVersionID_s(ProductID, strOptionsList);

            numPrice = CurrenciesBLL.ConvertCurrency(System.Web.UI.UserControl.Session["CUR_ID"], GetPriceWithGroupDiscount(numVersionID, numPrice));

            if (numPrice != default(Single))
            {
                phdNotOutOfStock4.Visible = true;
                UC_AddToBasketQty4.Visible = true;
                CheckOptionStock(strOptionsList);
                PricePreview(numPrice);
                phdNoValidCombinations.Visible = false; // hide the no-valid-combinations message
                updPricePanel.Update();
            }
            else
            {
                HidePriceForInvalidCombination();
                phdNotOutOfStock4.Visible = false;
                UC_AddToBasketQty4.Visible = false;
            }
        }
        else
        {
            HidePriceForInvalidCombination();
            phdNotOutOfStock4.Visible = false;
            UC_AddToBasketQty4.Visible = false;
        }
    }

    /// <summary>
    ///     ''' Hide price if option selections don't match
    ///     ''' any combination that is configured
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void HidePriceForInvalidCombination()
    {
        // 'Hide prices if option selections don't
        // correspond to a particular combination that
        // exists
        phdNoValidCombinations.Visible = true;  // show the no-valid-combinations message

        VersionsBLL objVersionsBLL = new VersionsBLL();
        numVersionID = objVersionsBLL.GetCombinationVersionID_s(_ProductID, "");

        // Call for price
        if (ReturnCallForPrice(_ProductID, numVersionID) != 1)
        {
            if (ConfigurationManager.AppSettings("TaxRegime").ToLower == "us" | ConfigurationManager.AppSettings("TaxRegime").ToLower == "simple")
            {
                phdPrice.Visible = true;
                litPriceView.Text = "-";
                return;
            }

            bool blnIncTax = IIf(KartSettingsManager.GetKartConfig("general.tax.pricesinctax") == "y", true, false);
            bool blnShowTax = IIf(KartSettingsManager.GetKartConfig("frontend.display.showtax") == "y", true, false);

            if (blnShowTax)
            {
                if (blnIncTax)
                {
                    phdIncTax.Visible = false;
                    phdExTax.Visible = false;
                }
                else
                {
                    phdExTax.Visible = false;
                    phdTax.Visible = false;
                }
            }
            else
                phdPrice.Visible = false;
            phdOutOfStock4.Visible = false;
        }
    }

    /// <summary>
    ///     ''' Calculates and displays the price of an options product, based on selections
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private void PricePreview(float pPrice, Int32 numV_ID = 0)
    {

        // 'Handle 'call for prices' - determine whether to
        // 'show the add to basket button or not

        if (ReturnCallForPrice(_ProductID, numV_ID) == 1)
        {
            // PricePreview(0)
            phdIncTax.Visible = false;
            phdExTax.Visible = false;
            phdTax.Visible = false;
            phdPrice.Visible = true;
            litPriceView.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("Versions", "ContentText_CallForPrice");
            // Hide add to basket button
            phdNotOutOfStock4.Visible = false;
            UC_AddToBasketQty4.Visible = false;
        }
        else
        {
            if (ConfigurationManager.AppSettings("TaxRegime").ToLower == "us" | ConfigurationManager.AppSettings("TaxRegime").ToLower == "simple")
            {
                phdPrice.Visible = true;
                litPriceView.Text = CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], pPrice);
                return;
            }

            bool blnIncTax = IIf(KartSettingsManager.GetKartConfig("general.tax.pricesinctax") == "y", true, false);
            bool blnShowTax = IIf(KartSettingsManager.GetKartConfig("frontend.display.showtax") == "y", true, false);

            float numTax = litTaxRateHidden.Text;
            DataRow[] drwCurrencies = GetCurrenciesFromCache().Select("CUR_ID = " + System.Web.UI.UserControl.Session["CUR_ID"]);
            float numCalculatedTax = CalculateTax(decimal.Round(System.Convert.ToDecimal(pPrice), drwCurrencies[0]("CUR_RoundNumbers")), numTax);
            drwCurrencies = null;

            if (blnShowTax)
            {
                if (blnIncTax)
                {
                    phdIncTax.Visible = true;
                    litIncTax.Text = CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], pPrice);
                    phdExTax.Visible = true;
                    litExTax.Text = CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], numCalculatedTax);
                }
                else
                {
                    phdExTax.Visible = true;
                    litExTax.Text = CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], pPrice);
                    phdTax.Visible = true;
                    litTaxRate.Text = System.Convert.ToString(numTax) + " %";
                }
            }
            else
            {
                phdPrice.Visible = true;
                litPriceView.Text = CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], pPrice);
            }
        }
    }

    /// <summary>
    ///     ''' Version price changed event
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public event Event_VersionPriceChangedEventHandler Event_VersionPriceChanged;

    public delegate void Event_VersionPriceChangedEventHandler(float pVersionPrice);

    /// <summary>
    ///     ''' Fires when items added to basket
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private void AddItemsToBasket(long pVersionID, float pQuantity, string pOptions = null)
    {
        object objMiniBasket = System.Web.UI.Control.Page.Master.FindControl("UC_MiniBasket");
        long numVersionID;

        numVersionID = System.Convert.ToInt64(pVersionID);

        string[] arrBasketInfo;
        int numBasketItemID = 0;
        ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();

        string strQuantityControl = LCase(objObjectConfigBLL.GetValue("K:product.addtobasketqty", ProductID));
        if (System.Web.UI.UserControl.Session["BasketItemInfo"] + "" != "" && (strQuantityControl == "dropdown" || strQuantityControl == "textbox"))
        {
            arrBasketInfo = Strings.Split(System.Web.UI.UserControl.Session["BasketItemInfo"], ";");
            numBasketItemID = arrBasketInfo[0];
        }

        objMiniBasket.ShowCustomText(pVersionID, pQuantity, pOptions, numBasketItemID);
    }

    /// <summary>
    ///     ''' Handles out-of-stock for options
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private void CheckOptionStock(string strOptionString)
    {
        phdOutOfStock4.Visible = false;
        phdNotOutOfStock4.Visible = true;
        UC_AddToBasketQty4.Visible = true;
        phdNoValidCombinations.Visible = false;

        // Just clean up the options a bit
        if (!String.IsNullOrEmpty(strOptionString))
            CleanOptionString(ref strOptionString);

        if (String.IsNullOrEmpty(strOptionString) || strOptionString == "0")
            return;

        VersionsBLL objVersionsBLL = new VersionsBLL();
        float numQty = objVersionsBLL.GetOptionStockQty(_ProductID, strOptionString);

        // The version is not live or does not exist,
        // probably because invalid options selections
        if (numQty == -9999.0F)
        {
            phdOutOfStock4.Visible = false;
            phdNotOutOfStock4.Visible = false;
            UC_AddToBasketQty4.Visible = false;
            HidePriceForInvalidCombination();
            return; // Stop right here
        }

        // This handles showing 'out of stock' and hiding the 'add' button
        if (objVersionsBLL.IsStockTrackingInBase(_ProductID) & numQty <= 0.0F & KartSettingsManager.GetKartConfig("frontend.orders.allowpurchaseoutofstock") != "y")
        {
            phdOutOfStock4.Visible = true;
            UC_AddToBasketQty4.Visible = false;
            phdNotOutOfStock4.Visible = false;

            // Set the command argument for this item based
            // We use this for stock notifications popup
            numVersionID = objVersionsBLL.GetCombinationVersionID_s(_ProductID, strOptionString); // This will find version ID for combination
            if (numVersionID == 0)
                numVersionID = UC_AddToBasketQty4.VersionID; // Combination ID will be zero if options product, so just grab base Version ID
            btnNotifyMe4.CommandArgument = FormatStockNotificationDetails(numVersionID, _ProductID, "", System.Web.UI.UserControl.Request.RawUrl.ToString().ToLower(), System.Web.UI.UserControl.Session["LANG"]);
        }
        updOptions.Update();
    }

    /// <summary>
    ///     ''' Cleans up the string of options, removing double
    ///     ''' commas where gaps occurred
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void CleanOptionString(ref string strOptionString)
    {
        strOptionString = "," + strOptionString + ",";

        // purge double commas
        while (strOptionString.Contains(",,"))
            strOptionString = Strings.Replace(strOptionString, ",,", ",");

        while (strOptionString.Contains(",0,"))
            strOptionString = Strings.Replace(strOptionString, ",0,", ",");

        if (!strOptionString == null)
        {
            if (strOptionString.EndsWith(","))
                strOptionString = strOptionString.TrimEnd((","));
            if (strOptionString.StartsWith(","))
                strOptionString = strOptionString.TrimStart((","));
        }

        // ' ------------------------------------------
        // ' Code to remove the duplicate options.
        string[] arrOptions = strOptionString.Split(",");
        for (int i = 0; i <= arrOptions.Length - 1; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                if (i != j)
                {
                    if (arrOptions[j] == arrOptions[i])
                        arrOptions[j] = "";
                }
            }
        }
        StringBuilder sbdOptions = new StringBuilder("");
        for (int i = 0; i <= arrOptions.Length - 1; i++)
        {
            if (arrOptions[i] != "")
            {
                sbdOptions.Append(arrOptions[i]);
                if (i != arrOptions.Length - 1)
                    sbdOptions.Append(",");
            }
        }
        strOptionString = sbdOptions.ToString();
    }

    /// <summary>
    ///     ''' Finds prices for customer group discount
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private double GetPriceWithGroupDiscount(long numVersionID, double numPrice)
    {
        Kartris.Basket objBasket = new Kartris.Basket();
        long numCustomerID = 0;
        double numNewPrice, numCustomerGroupPrice;

        if (HttpContext.Current.User.Identity.IsAuthenticated)
            numCustomerID = System.Convert.ToInt64((PageBaseClass)System.Web.UI.Control.Page.CurrentLoggedUser.ID);

        numCustomerGroupPrice = BasketBLL.GetCustomerGroupPriceForVersion(numCustomerID, numVersionID);
        if (numCustomerGroupPrice > 0)
            numNewPrice = Math.Min(numCustomerGroupPrice, numPrice);
        else
            numNewPrice = numPrice;

        return numNewPrice;
    }

    /// <summary>
    ///     ''' Launches error popup if qty entered does not fit with
    ///     ''' unitsize setting
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void AddWrongQuantity(string strTitle, string strMessage)
    {
        object objMiniBasket = System.Web.UI.Control.Page.Master.FindControl("UC_MiniBasket");
        objMiniBasket.ShowPopupMini(strTitle, strMessage);
    }

    /// <summary>
    ///     ''' Handles 'notify me' button clicks
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void StockNotificationHandler(object sender, CommandEventArgs e)
    {
        Button btnNotifyMe = (Button)sender;

        switch (btnNotifyMe.CommandName)
        {
            case "StockNotificationDetails":
                {

                    // Break command argument string into four parts
                    Array aryStockNotificationDetails = Split(btnNotifyMe.CommandArgument, "|||");
                    UC_StockNotification.VersionID = System.Convert.ToInt64(aryStockNotificationDetails(0));
                    UC_StockNotification.ProductID = System.Convert.ToInt32(aryStockNotificationDetails(1));
                    UC_StockNotification.VersionName = System.Web.UI.UserControl.Server.UrlDecode(aryStockNotificationDetails(2));
                    UC_StockNotification.PageLink = System.Web.UI.UserControl.Server.UrlDecode(aryStockNotificationDetails(3));
                    UC_StockNotification.LanguageID = System.Convert.ToByte(aryStockNotificationDetails(4));
                    UC_StockNotification.ShowStockNotificationsPopup();

                    break;
                    break;
                }
        }
    }

    /// <summary>
    ///     ''' Format the stock notification details into a triple-pipe
    ///     ''' separated string so it can be passed as single command
    ///     ''' argument
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public string FormatStockNotificationDetails(Int64 numVersionID, int numProductID, string strVersionName, string strPageLink, byte numLanguageID)
    {
        return (numVersionID.ToString() + "|||" + numProductID.ToString() + "|||" + System.Web.UI.UserControl.Server.UrlEncode(strVersionName) + "|||" + System.Web.UI.UserControl.Server.UrlEncode(strPageLink) + "|||" + numLanguageID.ToString());
    }

    /// <summary>
    ///     ''' This code tries to deal with occasional errors in the OptionsContainer.ascx.vb
    ///     ''' file, where _ProductID ends up as -1. In that case, we raise an event called 
    ///     ''' 'SomethingWentWrong()' there, and then act on it here.
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void UC_OptionsContainer_SomethingWentWrong()
    {
        UC_PopupMessage.SetTitle = System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_Alert");
        UC_PopupMessage.SetTextMessage = "Some kind of error occurred.";
        UC_PopupMessage.SetWidthHeight(350, 200);
        UC_PopupMessage.ShowPopup();
        System.Web.UI.UserControl.Response.Redirect(System.Web.UI.UserControl.Request.RawUrl);
    }

    /// <summary>
    ///     ''' Call for Price
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public Int16 ReturnCallForPrice(Int64 numP_ID, Int64 numV_ID = 0)
    {
        ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
        object objValue = objObjectConfigBLL.GetValue("K:product.callforprice", numP_ID);
        if (System.Convert.ToInt32(objValue) == 0 & numV_ID != 0)
            // Product not call for price, maybe there is a version
            objValue = objObjectConfigBLL.GetValue("K:version.callforprice", numV_ID);
        return objValue;
    }
}
