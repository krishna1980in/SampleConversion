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
using CkartrisDataManipulation;

/// <summary>

/// ''' This control contains the dropdown, textbox or hidden field for quantity

/// ''' which is added to the basket. For some display types, a button can be

/// ''' displayed. For options and dropdown versions, the button is hidden, as

/// ''' a separate add-to-basket button is used on those version display methods

/// ''' (because they have add button specific code to fire)

/// ''' </summary>
partial class UserControls_General_AddToBasket : System.Web.UI.UserControl
{

    // This user control contains the version ID
    // which we need when add to basket button is
    // pushed, and also a boolean to specify
    // whether it should display with a button or
    // not'
    private long c_numVersionID;
    private long c_numProductID;
    private bool c_blnHasAddButton;
    private bool c_blnCanCustomize;
    private string c_strSelectorType;
    private float c_ItemsQuantity;
    private float c_ItemEditQty;
    private int c_ItemEditVersionID;
    private string c_UnitSize;

    public long VersionID
    {
        get
        {
            return c_numVersionID;
        }
        set
        {
            c_numVersionID = value;
            litVersionID.Text = c_numVersionID;
        }
    }

    public bool HasAddButton
    {
        get
        {
            return c_blnHasAddButton;
        }
        set
        {
            c_blnHasAddButton = value;
            btnAdd.Visible = c_blnHasAddButton;
        }
    }


    public bool CanCustomize
    {
        get
        {
            return c_blnCanCustomize;
        }
        set
        {
            c_blnCanCustomize = value;
            if (c_blnCanCustomize == true)
                btnAdd.OnClientClick = "";
        }
    }

    // Dropdown, textbox or just a button
    public string SelectorType
    {
        get
        {
            return c_strSelectorType;
        }
        set
        {
            GetSelectorType(value);
        }
    }

    public string GetSelectorType(string strSelectorType)
    {
        if (strSelectorType != "")
            c_strSelectorType = strSelectorType;
        else
        {
            // Try to set to the K:product.addtorbasketqty set at product level
            VersionsBLL objVersionsBLL = new VersionsBLL();
            ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
            c_strSelectorType = LCase(FixNullFromDB(objObjectConfigBLL.GetValue("K:product.addtobasketqty", objVersionsBLL.GetProductID_s(c_numVersionID))));
            if (c_strSelectorType == null)
                c_strSelectorType = "";

            // No per-product value set, default to global config setting frontend.basket.addtobasketdisplay
            if (c_strSelectorType == "")
                c_strSelectorType = KartSettingsManager.GetKartConfig("frontend.basket.addtobasketdisplay");
        }
        return c_strSelectorType;
    }

    // Read the item quantity from the dropdown
    public float ItemsQuantity
    {
        get
        {
            float numItemsQuantity = 1;

            // Ok, this is a cheat... we don't know type of selector
            // at this stage, so how to know whether to check the
            // dropdown or textbox for the qty? What we do is check
            // which is highest value, then use that. This gives us
            // a 1 as default, or higher number if either textbox or
            // dropdown has a number in. It also means for blank or
            // non-valid textbox input, we output 1.

            if (IsNumeric(ddlItemsQuantity.SelectedValue))
                numItemsQuantity = ddlItemsQuantity.SelectedValue;
            else
                try
                {
                    numItemsQuantity = txtItemsQuantity.Text;
                }
                catch (Exception ex)
                {
                    numItemsQuantity = 1;
                }

            return numItemsQuantity;
        }
    }

    public double ItemEditQty
    {
        get
        {
            return c_ItemEditQty;
        }
        set
        {
            c_ItemEditQty = value;
        }
    }

    public int ItemEditVersionID
    {
        get
        {
            return c_ItemEditVersionID;
        }
        set
        {
            c_ItemEditVersionID = value;
        }
    }

    public string UnitSize
    {
        get
        {
            return GetUnitSize();
        }
    }

    public string GetUnitSize()
    {
        // For some items, using ProductID works. Some like multiple versions seem not
        // to pass this well, so better to use the VersionID and then lookup parent product from that
        long numProductID;
        numProductID = c_numProductID;
        VersionsBLL objVersionsBLL = new VersionsBLL();
        if (numProductID == 0)
        {
            try
            {
                numProductID = objVersionsBLL.GetProductID_s(litVersionID.Text);
            }
            catch (Exception ex)
            {
            }
        }

        // Unit size should be checked if the quantity control is "textbox" => qty entered by user
        ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
        c_UnitSize = FixNullFromDB(objObjectConfigBLL.GetValue("K:product.unitsize", numProductID));
        if (c_UnitSize == null)
            c_UnitSize = "";
        c_UnitSize = Strings.Replace(c_UnitSize, ",", "."); // Will use the "." instead of "," (just in case wrongly typed)
        if (!Information.IsNumeric(c_UnitSize))
            c_UnitSize = "1"; // Unit size default value is 1
        return c_UnitSize;
    }

    // Handles the 'add' button being clicked.
    // Remember this does not happen for options
    // or version dropdowns, as these have their
    // own add buttons and click handling in
    // ProductVersions.aspx
    protected void btnAdd_Click(object sender, System.EventArgs e)
    {
        string strUnitSize = GetUnitSize();
        float numQuantity = this.ItemsQuantity;
        object objMiniBasket = System.Web.UI.Control.Page.Master.FindControl("UC_MiniBasket");
        decimal numMod = SafeModulus(numQuantity, strUnitSize);
        VersionsBLL objVersionsBLL = new VersionsBLL();

        if (numMod != 0M)
            // ' wrong quantity - quantity should be a multiplies of unit size
            objMiniBasket.ShowPopupMini(System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_CorrectErrors"), Strings.Replace(System.Web.UI.TemplateControl.GetGlobalResourceObject("ObjectConfig", "ContentText_OrderMultiplesOfUnitsize"), "[unitsize]", strUnitSize));
        else
        {
            long numBasketItemID = 0;
            // qty ok, matches unitsize allowed
            // Trace.Warn("Quantity: " & numQuantity)

            long numEditVersionID = 0;
            ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
            string strAddToBasketQtyCFG = FixNullFromDB(objObjectConfigBLL.GetValue("K:product.addtobasketqty", objVersionsBLL.GetProductID_s(c_numVersionID)));
            if (strAddToBasketQtyCFG == null)
                strAddToBasketQtyCFG = "";
            if (System.Web.UI.UserControl.Session["BasketItemInfo"] + "" != "" && Strings.LCase(strAddToBasketQtyCFG) == "dropdown")
            {
                string[] arrBasketItemInfo = Strings.Split(System.Web.UI.UserControl.Session["BasketItemInfo"] + "", ";");
                numBasketItemID = arrBasketItemInfo[0];
                numEditVersionID = arrBasketItemInfo[1];
            }

            if (System.Convert.ToInt64(litVersionID.Text) != numEditVersionID)
                numBasketItemID = 0;

            objMiniBasket.ShowCustomText((long)litVersionID.Text, numQuantity, "", numBasketItemID);

            System.Web.UI.UserControl.Session["AddItemVersionID"] = litVersionID.Text;
        }

        // v2.9010 Autosave basket
        try
        {
            BasketBLL.AutosaveBasket((PageBaseClass)System.Web.UI.Control.Page.CurrentLoggedUser.ID);
        }
        catch (Exception ex)
        {
        }
    }

    private void LoadAddItemToBasket()
    {
        string strUnitSize = GetUnitSize();
        string strMinimumAllowedQty = "1";
        VersionsBLL objVersionsBLL = new VersionsBLL();
        ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();

        // If nothing specified, set selector type based
        // on config setting.
        string strAddToBasketQtyCFG = FixNullFromDB(objObjectConfigBLL.GetValue("K:product.addtobasketqty", objVersionsBLL.GetProductID_s(c_numVersionID)));
        if (strAddToBasketQtyCFG == null)
            strAddToBasketQtyCFG = "";
        c_strSelectorType = GetSelectorType(strAddToBasketQtyCFG);

        // Create onclientclick event to launch
        // the 'add to basket' popup. This way it
        // appears instantly when the button is
        // clicked, while the adding process can
        // happen in the background.
        if (IsNumeric(KartSettingsManager.GetKartConfig("frontend.basket.behaviour")))
        {
            // If the item is customizable, we don't want to launch
            // the 'add to basket' popup, but revert to the 
            // customization popup.
            if (CanCustomize == false)
                btnAdd.OnClientClick = "ShowAddToBasketPopup(" + KartSettingsManager.GetKartConfig("frontend.basket.behaviour") * 1000 + ")";
        }

        // Need to know if the decimal quantity is allowed or not
        if (Math.Abs(System.Convert.ToSingle(strUnitSize) % 1) > 0.0F)
        {
            // UnitSize is decimal
            if (System.Convert.ToSingle(strUnitSize) > 1)
                strMinimumAllowedQty = strUnitSize;
            strMinimumAllowedQty = strUnitSize;
            // we need to exclude the "." if the unit size is decimal, so to accept both (numbers & ".")
            filQuantity.FilterType = AjaxControlToolkit.FilterTypes.Numbers | AjaxControlToolkit.FilterTypes.Custom;
            filQuantity.ValidChars = ".";
        }
        else
            // UnitSize is integer
            if (System.Convert.ToInt32(strUnitSize) > 1)
            strMinimumAllowedQty = System.Convert.ToString(strUnitSize);

        // Set quantity selector type
        switch (c_strSelectorType)
        {
            case "textbox":
                {
                    txtItemsQuantity.Text = strMinimumAllowedQty;
                    txtItemsQuantity.Visible = true;
                    ddlItemsQuantity.Visible = false;
                    break;
                }

            case "none":
                {
                    // No field to input quantity next to 'add' button
                    // (just adds a single item when button pushed, best
                    // for sites where there are only single items for sale, or large
                    // value items people are unlikely to order in quantity)
                    txtItemsQuantity.Visible = false;
                    ddlItemsQuantity.Visible = false;
                    txtItemsQuantity.Text = strMinimumAllowedQty;
                    break;
                }

            default:
                {
                    // Defaults the selector to dropdown if not specified explicitly.
                    int numCounter;
                    byte bitValue = 1;
                    try
                    {
                        bitValue = System.Convert.ToByte(ddlItemsQuantity.Text);
                    }
                    catch (Exception ex)
                    {
                    }
                    ddlItemsQuantity.Items.Clear();

                    // We want number of entries as in the config setting. However,
                    // if the item has a unitsize set, we want to reflect that. So
                    // we should multiply everything by unitsize.
                    for (numCounter = 1; numCounter <= System.Convert.ToInt32(KartSettingsManager.GetKartConfig("frontend.basket.addtobasketdropdown.max")); numCounter++)
                        ddlItemsQuantity.Items.Add((numCounter * System.Convert.ToSingle(strUnitSize)).ToString());

                    if (c_numVersionID == c_ItemEditVersionID & c_ItemEditVersionID > 0)
                    {
                        btnAdd.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "FormButton_Update");
                        if (!System.Web.UI.UserControl.IsPostBack)
                        {
                            if (numCounter < (c_ItemEditQty / (double)System.Convert.ToSingle(strUnitSize)))
                            {
                                ddlItemsQuantity.Items.Add(c_ItemEditQty);
                                ddlItemsQuantity.SelectedValue = c_ItemEditQty;
                            }
                            else
                                ddlItemsQuantity.SelectedValue = c_ItemEditQty;
                        }
                        else
                            ddlItemsQuantity.Text = bitValue;
                    }
                    else
                    {
                        btnAdd.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("Products", "FormButton_Add");
                        if (bitValue >= numCounter)
                        {
                            ddlItemsQuantity.Items.Add(bitValue);
                            ddlItemsQuantity.SelectedValue = bitValue;
                        }
                        try
                        {
                            ddlItemsQuantity.Text = bitValue;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    txtItemsQuantity.Visible = false;
                    ddlItemsQuantity.Visible = true;
                    break;
                }
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!System.Web.UI.UserControl.IsPostBack)
            System.Web.UI.UserControl.Session["AddItemVersionID"] = "";
    }

    protected void Page_PreRender(object sender, System.EventArgs e)
    {
        LoadAddItemToBasket();
    }
}
