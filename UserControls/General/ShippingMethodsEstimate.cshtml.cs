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

partial class UserControls_ShippingMethodsEstimate : System.Web.UI.UserControl
{
    private double _boundary;
    private int _destinationid;
    private Kartris.Interfaces.objShippingDetails _shippingdetails;

    /// <summary>
    ///     ''' Handle refresh
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public void Refresh()
    {

        // Hide this control if not turned on from config settings
        if (GetKartConfig("frontend.minibasket.shippingestimate") != "y")
            this.Visible = false;

        string strPickupAvailable = "";
        int CUR_ID = System.Convert.ToInt32(System.Web.UI.UserControl.Session["CUR_ID"]);

        if (GetKartConfig("frontend.checkout.shipping.pickupoption") == "y")
            strPickupAvailable = System.Web.UI.TemplateControl.GetGlobalResourceObject("Shipping", "ContentText_ShippingPickup");

        if (ddlCountry.SelectedValue == 0)
        {
            int intSessionShippingCountry = System.Web.UI.UserControl.Session["ShippingEstimateCountry"];
            if (intSessionShippingCountry > 0)
            {
                try
                {
                    ddlCountry.SelectedValue = intSessionShippingCountry;
                }
                catch (Exception ex)
                {
                }
                _destinationid = intSessionShippingCountry;
            }
            else
            {
                // ' try to get the default shipping destination if session is empty
                _destinationid = GetDefaultDestinationForUser();
                if (_destinationid == 0)
                    _destinationid = KartSettingsManager.GetKartConfig("frontend.checkout.defaultcountry");
                if (_destinationid == 0)
                    _destinationid = GetDestinationFromBrowser();
                if (_destinationid == 0)
                    ddlCountry.SelectedValue = 0;
                else
                    try
                    {
                        KartrisClasses.Country objCountry = KartrisClasses.Country.Get(_destinationid);
                        string strIso = objCountry.IsoCode;
                        List<Country> lstCountries = KartrisClasses.Country.GetAll();
                        var varC = from c in lstCountries
                                   where c.IsoCode == strIso
                                   orderby c.Name
                                   select c.CountryId;

                        ddlCountry.SelectedValue = varC.First();
                    }
                    catch (Exception ex)
                    {
                    }
            }
        }
        else
        {
            _destinationid = ddlCountry.SelectedValue;
            // remember selected shipping country
            System.Web.UI.UserControl.Session["ShippingEstimateCountry"] = ddlCountry.SelectedValue;
        }

        List<ShippingMethod> lstShippingMethods = ShippingMethod.GetAll(_shippingdetails, _destinationid, CurrenciesBLL.ConvertCurrency(CurrenciesBLL.GetDefaultCurrency, _boundary, CUR_ID));

        if (lstShippingMethods != null)
        {
            lblError.Visible = false;
            // add customer pickup if it is enabled in config
            if (!string.IsNullOrEmpty(strPickupAvailable))
            {
                ShippingMethod spPickup = new ShippingMethod(strPickupAvailable, System.Web.UI.TemplateControl.GetGlobalResourceObject("Shipping", "ContentText_ShippingPickupDesc"), 0, 0);
                lstShippingMethods.Add(spPickup);
            }


            if (GetKartConfig("frontend.display.showtax") != "y")
            {
                if (GetKartConfig("general.tax.pricesinctax") != "y")
                {
                    // Show extax
                    gvwShippingMethods.Columns(2).HeaderText = System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_Price");
                    gvwShippingMethods.Columns(3).Visible = false;
                }
                else
                {
                    // Show inctax
                    gvwShippingMethods.Columns(2).Visible = false;
                    gvwShippingMethods.Columns(3).HeaderText = System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_Price");
                }
            }
            // hide extax field field showtax config is set to 'n'
            // If GetKartConfig("frontend.display.showtax") <> "y" Then
            // gvwShippingMethods.Columns(2).Visible = False
            // gvwShippingMethods.Columns(3).HeaderText = GetGlobalResourceObject("Kartris", "ContentText_Price")
            // Else
            // gvwShippingMethods.Columns(2).Visible = True
            // End If

            gvwShippingMethods.DataSource = lstShippingMethods;
            gvwShippingMethods.DataBind();
        }
        else
            lblError.Visible = true;
    }

    /// <summary>
    ///     ''' Apply appropriate pricing
    ///     ''' Inc/Ex tax, or just price
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    private void gvwShippingMethods_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if (e.Row.RowType != System.Web.UI.WebControls.DataControlRowType.Header & e.Row.RowType != System.Web.UI.WebControls.DataControlRowType.Footer)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Literal litShippingMethodNameExTax = (Literal)e.Row.FindControl("litShippingMethodNameExTax");
                Literal litShippingMethodNameIncTax = (Literal)e.Row.FindControl("litShippingMethodNameIncTax");

                int CUR_ID = System.Convert.ToInt32(System.Web.UI.UserControl.Session["CUR_ID"]);
                // format shipping method's extax and inctax values
                string strIncTax = CurrenciesBLL.FormatCurrencyPrice(CUR_ID, CurrenciesBLL.ConvertCurrency(CUR_ID, System.Convert.ToDouble(litShippingMethodNameIncTax.Text)));
                string strExTax = CurrenciesBLL.FormatCurrencyPrice(CUR_ID, CurrenciesBLL.ConvertCurrency(CUR_ID, System.Convert.ToDouble(litShippingMethodNameExTax.Text)));

                litShippingMethodNameExTax.Text = strExTax;
                litShippingMethodNameIncTax.Text = strIncTax;
            }
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

    /// <summary>
    ///     ''' Find user's location based on address if they
    ///     ''' are logged in
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public int GetDefaultDestinationForUser()
    {
        if ((PageBaseClass)System.Web.UI.Control.Page.CurrentLoggedUser != null)
        {
            // ' will try to find the default shipping address, if not then default billing address for the current user
            System.Collections.Generic.List<KartrisClasses.Address> lstUsrAddresses = null;
            lstUsrAddresses = KartrisClasses.Address.GetAll((PageBaseClass)System.Web.UI.Control.Page.CurrentLoggedUser.ID);
            List<Address> Addresses = lstUsrAddresses.FindAll(p => p.Type == "b" | p.Type == "u");
            if ((PageBaseClass)System.Web.UI.Control.Page.CurrentLoggedUser.DefaultShippingAddressID != 0)
            {
                Address adrs = Addresses.Find(p => p.ID == (PageBaseClass)System.Web.UI.Control.Page.CurrentLoggedUser.DefaultShippingAddressID);
                if (adrs != null)
                    return adrs.CountryID;
            }
            else if ((PageBaseClass)System.Web.UI.Control.Page.CurrentLoggedUser.DefaultBillingAddressID != 0)
            {
                Address adrs = Addresses.Find(p => p.ID == (PageBaseClass)System.Web.UI.Control.Page.CurrentLoggedUser.DefaultBillingAddressID);
                if (adrs != null)
                    return adrs.CountryID;
            }
        }
        return 0;
    }

    /// <summary>
    ///     ''' Guess the user's location from browser culture
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public int GetDestinationFromBrowser()
    {
        // Not perfect, but better than nothing. Could be
        // done by IP, but this would require a lookup and
        // browser culture is probably better guide (some
        // countries use multiple languages, or someone
        // might be using their computer overseas)
        try
        {
            string[] ClientLanguages = HttpContext.Current.Request.UserLanguages;
            if (ClientLanguages != null && ClientLanguages.Length > 0)
            {
                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CreateSpecificCulture(ClientLanguages[0].ToLowerInvariant().Trim());
                if (culture != null)
                    return KartrisClasses.Country.GetByIsoCode(new System.Globalization.RegionInfo(culture.LCID).TwoLetterISORegionName, System.Web.UI.UserControl.Session["LangID"]).CountryId;
            }
        }
        catch (Exception ex)
        {
        }
        return 0;
    }
}
