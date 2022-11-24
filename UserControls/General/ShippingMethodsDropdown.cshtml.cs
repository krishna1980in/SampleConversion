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
using KartSettingsManager;
using KartrisClasses;

partial class UserControls_ShippingMethodsDropdown : System.Web.UI.UserControl
{
    private double _boundary;
    private int _destinationid;
    private Kartris.Interfaces.objShippingDetails _shippingdetails;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        try
        {
            if (!this.IsPostBack)
            {
                if ((PageBaseClass)System.Web.UI.Control.Page.CurrentLoggedUser == null)
                {
                    // Try to get new page to clear memory of previous shipping selection
                    System.Web.UI.Control.ViewState["PreviouslySelected"] = null;
                    System.Web.UI.UserControl.Session["_selectedShippingAmount"] = 0;
                    System.Web.UI.UserControl.Session["_selectedShippingID"] = 0;
                }
            }

            if (System.Web.UI.UserControl.Session["_selectedShippingAmount"] == 0 & System.Web.UI.UserControl.Session["_selectedShippingID"] == 0)
            {
                if (ddlShippingMethods.Items.Count == 0)
                {
                    ddlShippingMethods.Visible = false;
                    litContentTextShippingAvailableAfterAddress.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {
            // error
            this.Visible = false;
        }
        // Take notice of the frontend.checkout.shipping.showmethods setting if set to neveronone
        if (ddlShippingMethods.Items.Count == 1 & KartSettingsManager.GetKartConfig("frontend.checkout.shipping.showmethods") == "neveronone")
            ddlShippingMethods.Visible = false;
    }

    public void Refresh()
    {
        int CUR_ID = System.Convert.ToInt32(System.Web.UI.UserControl.Session["CUR_ID"]);
        List<ShippingMethod> lstShippingMethods = ShippingMethod.GetAll(_shippingdetails, _destinationid, CurrenciesBLL.ConvertCurrency(CurrenciesBLL.GetDefaultCurrency, _boundary, CUR_ID));
        ddlShippingMethods.Items.Clear();
        {
            var withBlock = ddlShippingMethods;
            withBlock.DataSource = lstShippingMethods;
            withBlock.DataTextField = "DropDownText";
            withBlock.DataValueField = "DropDownValue";
            withBlock.DataBind();
        }

        ListItem liShippingMethod;
        CurrenciesBLL objCurrency = new CurrenciesBLL();

        foreach (var liShippingMethod in ddlShippingMethods.Items)
        {
            string[] arrText = Split(liShippingMethod.Text, "|||||");

            string strIncTax = CurrenciesBLL.FormatCurrencyPrice(CUR_ID, CurrenciesBLL.ConvertCurrency(CUR_ID, System.Convert.ToDouble(arrText[2])));
            string strExTax = CurrenciesBLL.FormatCurrencyPrice(CUR_ID, CurrenciesBLL.ConvertCurrency(CUR_ID, System.Convert.ToDouble(arrText[1])));

            if (GetKartConfig("frontend.display.showtax") != "y")
            {
                if (GetKartConfig("general.tax.pricesinctax") != "y")
                    // Show extax
                    liShippingMethod.Text = arrText[0] + Constants.vbTab.ToString() + " - " + System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_Price").ToString() + ": " + strExTax + Constants.vbTab.ToString();
                else
                    // Show inctax
                    liShippingMethod.Text = arrText[0] + Constants.vbTab.ToString() + " - " + System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_Price").ToString() + ": " + strIncTax + Constants.vbTab.ToString();
            }
            else
                liShippingMethod.Text = arrText[0] + Constants.vbTab.ToString() + " - " + System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_ExTax").ToString() + ": " + strExTax + Constants.vbTab.ToString() + " / " + System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_IncTax").ToString() + ": " + strIncTax;
        }

        System.Web.UI.Control.ViewState["PreviouslySelected"] = null;

        // customer pickup is effectively another option
        // so we must add it to find total number of shipping
        // methods - basically set to 1 or zero then add to
        // shipping methods count below
        bool blnPickUp = false;
        if (GetKartConfig("frontend.checkout.shipping.pickupoption") == "y")
            blnPickUp = true;

        // Let's figure out how many shipping methods
        int numShippingMethods = 0;
        if (GetKartConfig("frontend.checkout.shipping.pickupoption") == "y")
            numShippingMethods += 1; // Add one for customer pickup

        if (lstShippingMethods != null)
        {
            numShippingMethods += lstShippingMethods.Count;
            // Determine menu based on number of shipping options
            // (from shipping system) plus the pickup option which
            // is set in config setting
            // If lstShippingMethods.Count < 1 And blnPickUp Then
            // 'Customer pickup is only shipping option
            // ddlShippingMethods.Visible = True
            // litContentTextShippingAvailableAfterAddress.Visible = False
            // If GetKartConfig("frontend.checkout.shipping.pickupoption") = "y" Then
            // ddlShippingMethods.Items.Insert(0, New ListItem(GetGlobalResourceObject("Shipping", "ContentText_ShippingPickup"), "999"))
            // End If
            // ddlShippingMethods.SelectedIndex = 0

            // Dim arrSM As String() = Split(ddlShippingMethods.SelectedValue, "|||||")
            // Session("_selectedShippingID") = CInt(999)
            // Session("_selectedShippingAmount") = CDbl(0)
            // ViewState("PreviouslySelected") = 999 & "||||||" & GetGlobalResourceObject("Shipping", "ContentText_ShippingPickup")
            // Dim lstSelected As ListItem = ddlShippingMethods.SelectedItem
            // lstSelected.Text = GetGlobalResourceObject("Shipping", "ContentText_ShippingPickup")
            // RaiseEvent ShippingSelected(Nothing, Nothing)

            if (numShippingMethods > 0)
            {

                // More than one option including ship options and customer pickup
                ddlShippingMethods.Visible = true;
                litContentTextShippingAvailableAfterAddress.Visible = false;
                if (blnPickUp)
                    ddlShippingMethods.Items.Insert(0, new ListItem(System.Web.UI.TemplateControl.GetGlobalResourceObject("Shipping", "ContentText_ShippingPickup"), "999"));
                ddlShippingMethods.Items.Insert(0, new ListItem(System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_DropdownSelectDefault"), ""));
                System.Web.UI.UserControl.Session["_selectedShippingID"] = 0;
                System.Web.UI.UserControl.Session["_selectedShippingAmount"] = 0;
            }
            else if (numShippingMethods == 1)
            {
                // One shipping option other than customer pickup
                ddlShippingMethods.Visible = true;
                litContentTextShippingAvailableAfterAddress.Visible = false;
                if (blnPickUp)
                    ddlShippingMethods.Items.Insert(0, new ListItem(System.Web.UI.TemplateControl.GetGlobalResourceObject("Shipping", "ContentText_ShippingPickup"), "999"));
                ddlShippingMethods.SelectedIndex = 0;

                string[] arrSM = Split(ddlShippingMethods.SelectedValue, "|||||");
                System.Web.UI.UserControl.Session["_selectedShippingID"] = (int)arrSM[0];
                System.Web.UI.UserControl.Session["_selectedShippingAmount"] = (double)arrSM[1];
                System.Web.UI.Control.ViewState["PreviouslySelected"] = ddlShippingMethods.SelectedItem.Value + "||||||" + ddlShippingMethods.SelectedItem.Text;
                ListItem lstSelected = ddlShippingMethods.SelectedItem;
                lstSelected.Text = System.Convert.ToString(arrSM[2]);
                ShippingSelected?.Invoke(null, null);
            }
            else if (ddlShippingMethods.Items.Count == 0)
            {
                ddlShippingMethods.Visible = false;
                litContentTextShippingAvailableAfterAddress.Visible = true;
                litContentTextShippingAvailableAfterAddress.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("Shipping", "ContentText_NoValidShipping");
            }
        }
        else if (blnPickUp)
        {
            // Customer pickup is only shipping option
            ddlShippingMethods.Visible = true;
            litContentTextShippingAvailableAfterAddress.Visible = false;
            ddlShippingMethods.Items.Insert(0, new ListItem(System.Web.UI.TemplateControl.GetGlobalResourceObject("Shipping", "ContentText_ShippingPickup"), "999"));
            ddlShippingMethods.SelectedIndex = 0;

            string[] arrSM = Split(ddlShippingMethods.SelectedValue, "|||||");
            System.Web.UI.UserControl.Session["_selectedShippingID"] = (int)999;
            System.Web.UI.UserControl.Session["_selectedShippingAmount"] = (double)0;
            System.Web.UI.Control.ViewState["PreviouslySelected"] = 999 + "||||||" + System.Web.UI.TemplateControl.GetGlobalResourceObject("Shipping", "ContentText_ShippingPickup");
            ListItem lstSelected = ddlShippingMethods.SelectedItem;
            lstSelected.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("Shipping", "ContentText_ShippingPickup");
            ShippingSelected?.Invoke(null, null);
        }
        else
        {
            litContentTextShippingAvailableAfterAddress.Visible = true;
            litContentTextShippingAvailableAfterAddress.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("Shipping", "ContentText_NoValidShipping");
        }
    }

    public int DestinationID
    {
        get
        {
            return _destinationid;
        }
        set
        {
            _destinationid = value;
        }
    }

    public double Boundary
    {
        set
        {
            _boundary = value;
        }
    }

    public Interfaces.objShippingDetails ShippingDetails
    {
        set
        {
            _shippingdetails = value;
        }
    }

    public double SelectedShippingAmount
    {
        get
        {
            return System.Web.UI.UserControl.Session["_selectedShippingAmount"];
        }
    }

    public int SelectedShippingID
    {
        get
        {
            return System.Web.UI.UserControl.Session["_selectedShippingID"];
        }
    }

    protected void ddlShippingMethods_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        string[] arrSM = Split(ddlShippingMethods.SelectedValue, "|||||");
        if (Information.UBound(arrSM) == 2 | ddlShippingMethods.SelectedValue == "999")
        {
            if (Information.UBound(arrSM) == 2)
            {
                System.Web.UI.UserControl.Session["_selectedShippingID"] = (int)arrSM[0];
                System.Web.UI.UserControl.Session["_selectedShippingAmount"] = (double)arrSM[1];
            }
            else
            {
                System.Web.UI.UserControl.Session["_selectedShippingID"] = 999;
                System.Web.UI.UserControl.Session["_selectedShippingAmount"] = 0;
            }

            if (System.Web.UI.Control.ViewState["PreviouslySelected"] != null)
            {
                string[] arrPrevious = Strings.Split(System.Web.UI.Control.ViewState["PreviouslySelected"], "||||||");
                ListItem lstPreviouslySelected = ddlShippingMethods.Items.FindByValue(arrPrevious[0]);
                lstPreviouslySelected.Text = arrPrevious[1];
            }

            System.Web.UI.Control.ViewState["PreviouslySelected"] = ddlShippingMethods.SelectedItem.Value + "||||||" + ddlShippingMethods.SelectedItem.Text;

            if (ddlShippingMethods.SelectedValue != "999")
            {
                ListItem lstSelected = ddlShippingMethods.SelectedItem;
                lstSelected.Text = System.Convert.ToString(arrSM[2]);
            }

            // remove the ==SELECT== item in the shipping methods dropdown after a selection
            if (ddlShippingMethods.Items(0).Value == "")
                ddlShippingMethods.Items.RemoveAt(0);

            ShippingSelected?.Invoke(sender, e);
        }
    }

    public event ShippingSelectedEventHandler ShippingSelected;

    public delegate void ShippingSelectedEventHandler(object sender, System.EventArgs e);
}
