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
using CkartrisEnumerations;
using CkartrisDataManipulation;

partial class Admin_Destinations : _PageBaseClass
{
    private static Configuration ModifiedConfig = null/* TODO Change to default(_) if this is not a reference type */;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_RegionalWizard", "PageTitle_RegionalSetupWizard") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        if (!Page.IsPostBack)
        {
            string strTaxRegime = TaxRegime.Name;

            try
            {
                // Try to set tax regime dropdown
                ddlTaxRegime.SelectedValue = strTaxRegime;
            }
            catch (Exception ex)
            {
            }

            // set default config settings for each region
            switch (strTaxRegime)
            {
                case "EU":
                    {
                        ddlPricesIncTaxConfig.SelectedValue = "y";
                        ddlShowTaxConfig.SelectedValue = "c";
                        break;
                    }

                default:
                    {
                        ddlPricesIncTaxConfig.SelectedValue = "n";
                        ddlShowTaxConfig.SelectedValue = "c";
                        break;
                    }
            }

            // Populate currency dropdown
            DataTable dtbCurrencies = KartSettingsManager.GetCurrenciesFromCache();
            DataRow[] drwCurrencies = dtbCurrencies.Select();
            if (drwCurrencies.Length > 0)
            {
                ddlCurrency.Items.Clear();
                ddlCurrency.Items.Add(new ListItem(GetGlobalResourceObject("_Kartris", "ContentText_DropDownSelect"), "noselection"));
                for (byte i = 0; i <= drwCurrencies.Length - 1; i++)
                    ddlCurrency.Items.Add(new ListItem(drwCurrencies[i]("CUR_Symbol") + " " + drwCurrencies[i]("CUR_ISOCode"), drwCurrencies[i]("CUR_ID")));
            }
        }
        else
            pnlSummary.Visible = false;
    }

    /// <summary>
    ///     ''' Reset tax regime
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void ddlTaxRegime_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        try
        {
            if (ModifiedConfig == null)
                ModifiedConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
        }
        catch (Exception ex)
        {
        }

        try
        {
            XmlDocument doc = new XmlDocument();
            System.Configuration.ConfigurationSection section = ModifiedConfig.GetSection("appSettings");
            KeyValueConfigurationElement element = (KeyValueConfigurationElement)ModifiedConfig.AppSettings.Settings("TaxRegime");
            element.Value = ddlTaxRegime.SelectedValue;

            if (ModifiedConfig != null)
            {
                ModifiedConfig.Save();
                Response.Redirect("");
            }
        }
        catch (Exception ex)
        {
        }
    }

    protected void ShowMasterUpdateMessage()
    {
        (Skins_Admin_Template)this.Master.DataUpdated();
    }

    /// <summary>
    ///     ''' Handle currency selection dropdown
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void ddlCurrency_SelectedIndexChanged(object sender, System.EventArgs e)
    {

        // If no currency, hide the rest of things
        if (ddlCurrency.SelectedValue != "noselection")
            // Currency selected
            mvwRegionalSetupWizard.Visible = true;
        else
        {
            // No currency, hide bits below
            mvwRegionalSetupWizard.Visible = false;
            pnlSummary.Visible = false;
        }

        string strActiveTaxRegime = TaxRegime.Name;
        switch (strActiveTaxRegime)
        {
            case "VAT":
                {
                    mvwRegionalSetupWizard.SetActiveView(viwVAT);
                    ddlCountries.Items.Clear();
                    ddlCountries.Items.Add(new ListItem(GetGlobalResourceObject("_Kartris", "ContentText_DropDownSelect"), "noselection"));

                    DataTable tblAllCountries = ShippingBLL._GetDestinationsByLanguage(Session("_LANG"));
                    foreach (DataRow drwCountry in tblAllCountries.Rows)
                        ddlCountries.Items.Add(new ListItem(drwCountry("D_Name"), drwCountry("D_ID")));
                    break;
                }

            case "EU":
                {
                    mvwRegionalSetupWizard.SetActiveView(viwEU);
                    break;
                }

            case "US":
                {
                    mvwRegionalSetupWizard.SetActiveView(viwUS);
                    ddlUSStates.Items.Clear();
                    ddlUSStates.Items.Add(new ListItem(GetGlobalResourceObject("_Kartris", "ContentText_DropDownSelect"), "noselection"));
                    foreach (var objCountry in GetCountryListFromTaxConfig())
                        ddlUSStates.Items.Add(new ListItem(objCountry.Name, objCountry.CountryId));
                    txtUSStateTaxRate.Text = "";
                    phdUSStateTaxRate.Visible = false;
                    break;
                }

            case "CANADA":
                {
                    mvwRegionalSetupWizard.SetActiveView(viwCanada);
                    ddlCanadaBaseProvince.Items.Clear();
                    ddlCanadaBaseProvince.Items.Add(new ListItem(GetGlobalResourceObject("_Kartris", "ContentText_DropDownSelect"), "noselection"));
                    foreach (var objCountry in GetCountryListFromTaxConfig())
                        ddlCanadaBaseProvince.Items.Add(new ListItem(objCountry.Name, objCountry.CountryId));
                    txtCanadaGST.Text = "";
                    txtCanadaPST.Text = "";
                    phdCanadaProvinceTax.Visible = false;
                    break;
                }

            case "SIMPLE":
                {
                    mvwRegionalSetupWizard.SetActiveView(viwOther);
                    ddlSimpleBaseCountry.Items.Clear();
                    ddlSimpleBaseCountry.Items.Add(new ListItem(GetGlobalResourceObject("_Kartris", "ContentText_DropDownSelect"), "noselection"));

                    DataTable tblAllCountries = ShippingBLL._GetDestinationsByLanguage(Session("_LANG"));
                    foreach (DataRow drwCountry in tblAllCountries.Rows)
                        ddlSimpleBaseCountry.Items.Add(new ListItem(drwCountry("D_Name"), drwCountry("D_ID")));

                    txtSimpleTaxRate.Text = "";
                    phdSimpleTaxRate.Visible = false;
                    break;
                }
        }
    }

    /// <summary>
    ///     ''' Handle 'VAT registered?' dropdown
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void ddlVATRegistered_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        if (ddlVatRegistered.SelectedValue == "y")
            // Yes
            phdBaseCountry.Visible = true;
        else if (ddlVatRegistered.SelectedValue == "n")
        {
            // No
            phdBaseCountry.Visible = false;
            phdVATRate.Visible = false;
            pnlSummary.Visible = true;
        }
        else
        {
            // No selection
            phdBaseCountry.Visible = false;
            phdVATRate.Visible = false;
            pnlSummary.Visible = false;
        }
    }

    /// <summary>
    ///     ''' Handle VAT country dropdown selection
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void ddlCountries_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        if (ddlCountries.SelectedValue == "noselection")
        {
            // No country selected, hide bits below
            phdVATRate.Visible = false;
            pnlSummary.Visible = false;
        }
        else
        {
            // Country selected, show VAT rate field
            txtVATRate.Text = "";
            phdVATRate.Visible = true;
        }
    }

    /// <summary>
    ///     ''' Handle EU 'VAT registered?' dropdown
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void ddlQVATRegistered_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        if (ddlEUVatRegistered.SelectedValue == "y")
        {
            // Yes
            ddlEUCountries.Items.Clear();
            ddlEUCountries.Items.Add(new ListItem(GetGlobalResourceObject("_Kartris", "ContentText_DropDownSelect"), "noselection"));
            foreach (var objCountry in GetCountryListFromTaxConfig())
            {
                try
                {
                    ddlEUCountries.Items.Add(new ListItem(objCountry.Name, objCountry.CountryId));
                }
                catch (Exception ex)
                {
                }
            }
            phdEUBaseCountry.Visible = true;
        }
        else if (ddlEUVatRegistered.SelectedValue == "n")
        {
            // No
            phdEUBaseCountry.Visible = false;
            phdEUVATRate.Visible = false;
            pnlSummary.Visible = true;
        }
        else
        {
            // No selection
            phdEUBaseCountry.Visible = false;
            phdEUVATRate.Visible = false;
            pnlSummary.Visible = false;
        }
    }

    /// <summary>
    ///     ''' Handle EU country dropdown selection
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void ddlEUCountries_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        if (ddlEUCountries.SelectedValue == "noselection")
        {
            // No country selected, hide bits below
            phdEUVATRate.Visible = false;
            pnlSummary.Visible = false;
        }
        else
        {
            // Country selected, show VAT rate field
            txtQVatRate.Text = "";
            phdEUVATRate.Visible = true;
        }
    }

    /// <summary>
    ///     ''' Handle US state dropdown selection
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void ddlUSStates_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        if (ddlUSStates.SelectedValue == "noselection")
        {
            // No state selected, hide bits below
            phdUSStateTaxRate.Visible = false;
            pnlSummary.Visible = false;
        }
        else
        {
            // State selected, show tax rate field
            txtUSStateTaxRate.Text = "";
            phdUSStateTaxRate.Visible = true;
        }
    }

    /// <summary>
    ///     ''' Handle Canada province/territory dropdown selection
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void ddlCanadaBaseProvince_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        if (ddlCanadaBaseProvince.SelectedValue == "noselection")
        {
            // No province/territory selected, hide bits below
            phdCanadaProvinceTax.Visible = false;
            pnlSummary.Visible = false;
        }
        else
        {
            // Province/territory selected, show tax rate fields
            lblCanadaBaseProvince.Text = ddlCanadaBaseProvince.SelectedItem.Text;
            txtCanadaGST.Text = "";
            txtCanadaPST.Text = "";
            phdCanadaProvinceTax.Visible = true;
        }
    }

    /// <summary>
    ///     ''' Handle country dropdown selection
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void ddlSimpleBaseCountry_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        if (ddlCanadaBaseProvince.SelectedValue == "noselection")
        {
            // No country selected, hide bits below
            phdSimpleTaxRate.Visible = false;
            pnlSummary.Visible = false;
        }
        else
        {
            // Country selected, show tax rate fields
            txtSimpleTaxRate.Text = "";
            phdSimpleTaxRate.Visible = true;
        }
    }

    /// <summary>
    ///     ''' Handle tax rate text box being changed
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void txtQVatRate_TextChanged(object sender, System.EventArgs e)
    {

        // If tax rate is blank (we total all boxes
        // so we can do check on one line) then we
        // hide next section
        if (txtQVatRate.Text + txtUSStateTaxRate.Text + txtSimpleTaxRate.Text + txtCanadaPST.Text + txtVATRate.Text == "")
            pnlSummary.Visible = false;
        else
            pnlSummary.Visible = true;
    }

    protected void btnConfirmSetup_Click(object sender, System.EventArgs e)
    {

        // CURRENCY
        int intSelectedCurrencyID = ddlCurrency.SelectedValue;

        // Set default currency
        string strMessage = string.Empty;
        CurrenciesBLL._SetDefault(intSelectedCurrencyID, strMessage);
        // /CURRENCY

        DataTable dtbAllCountries = ShippingBLL._GetDestinationsByLanguage(CkartrisBLL.GetLanguageIDfromSession);
        List<KartrisClasses.Country> lstRegionCountries = GetCountryListFromTaxConfig();

        // Store location ISO
        string strThisLocationISOCode = "";

        // Show link to reset currency rates
        lnkCurrencyLink.Visible = true;

        switch (TaxRegime.Name)
        {
            case "VAT":
                {
                    // VAT Registered?
                    if (ddlVatRegistered.SelectedValue == "y")
                    {
                        // Yes - turn off tax for everything else, except this country

                        foreach (DataRow drwCountry in dtbAllCountries.Rows)
                        {
                            int intCountryID = drwCountry("D_ID");

                            if (ddlCountries.SelectedValue == intCountryID)

                                // charge tax and set to live
                                ShippingBLL._UpdateDestinationForTaxWizard(intCountryID, drwCountry("D_ShippingZoneID"), 1, FixNullFromDB(drwCountry("D_Tax2")), drwCountry("D_ISOCode"), drwCountry("D_ISOCode3Letter"), drwCountry("D_ISOCodeNumeric"), FixNullFromDB(drwCountry("D_Region")), true, "", FixNullFromDB(drwCountry("D_TaxExtra")));
                            else
                                // just don't charge tax
                                ShippingBLL._UpdateDestinationForTaxWizard(intCountryID, drwCountry("D_ShippingZoneID"), 0, FixNullFromDB(drwCountry("D_Tax2")), drwCountry("D_ISOCode"), drwCountry("D_ISOCode3Letter"), drwCountry("D_ISOCodeNumeric"), FixNullFromDB(drwCountry("D_Region")), drwCountry("D_Live"), "", FixNullFromDB(drwCountry("D_TaxExtra")));

                            // Special case for UK/GB
                            // Brexit is a mess. It seems that for UK companies trying to import or export to the EU,
                            // an EU VAT number really should be collected and show on the invoice, because apparently it
                            // makes shipping paperwork easier.
                            // So we add the code below which updates the EU countries with some extra info in the D_TaxExtra
                            // field. Normally that won't be used, so we can use it to tell which countries are EU ones
                            // in order to show the EU VAT field at checkout.
                            // Loop through EU countries in XML, if match found with current country
                            // set tax off, but D_TaxExtra to "EU"
                            foreach (var objCountry in GetCountryListFromTaxConfig("EU"))
                            {
                                try
                                {
                                    if (objCountry.CountryId == intCountryID)
                                    {
                                        // country belongs to EU - charge tax and live
                                        ShippingBLL._UpdateDestinationForTaxWizard(intCountryID, drwCountry("D_ShippingZoneID"), 0, FixNullFromDB(drwCountry("D_Tax2")), drwCountry("D_ISOCode"), drwCountry("D_ISOCode3Letter"), drwCountry("D_ISOCodeNumeric"), FixNullFromDB(drwCountry("D_Region")), true, "", "EU");
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                    }
                    else
                        // No - Don't charge tax for all countries
                        foreach (DataRow drwCountry in dtbAllCountries.Rows)
                            ShippingBLL._UpdateDestinationForTaxWizard(drwCountry("D_ID"), drwCountry("D_ShippingZoneID"), 0, FixNullFromDB(drwCountry("D_Tax2")), drwCountry("D_ISOCode"), drwCountry("D_ISOCode3Letter"), drwCountry("D_ISOCodeNumeric"), FixNullFromDB(drwCountry("D_Region")), drwCountry("D_Live"), "", FixNullFromDB(drwCountry("D_TaxExtra")));

                    // Set default country
                    strThisLocationISOCode = ShippingBLL._GetISOCodeByDestinationID(ddlCountries.SelectedValue);
                    KartSettingsManager.SetKartConfig("general.tax.euvatcountry", strThisLocationISOCode, false);
                    break;
                }

            case "EU":
                {
                    // VAT Registered?
                    if (ddlEUVatRegistered.SelectedValue == "y")
                    {
                        // Yes - All EU Countries should be charged tax and set to live, everything else turn off tax

                        string strSelectedEUCountryISO = "";

                        // loop through all countries in the destination table
                        foreach (DataRow drwCountry in dtbAllCountries.Rows)
                        {
                            int intCountryID = drwCountry("D_ID");

                            // Set each country to non EU - don't charge tax
                            ShippingBLL._UpdateDestinationForTaxWizard(intCountryID, drwCountry("D_ShippingZoneID"), 0, FixNullFromDB(drwCountry("D_Tax2")), drwCountry("D_ISOCode"), drwCountry("D_ISOCode3Letter"), drwCountry("D_ISOCodeNumeric"), FixNullFromDB(drwCountry("D_Region")), drwCountry("D_Live"), "", FixNullFromDB(drwCountry("D_TaxExtra")));
                            // Loop through EU countries in XML, if match found with current country
                            // set tax on
                            foreach (var objCountry in GetCountryListFromTaxConfig())
                            {
                                try
                                {
                                    if (objCountry.CountryId == intCountryID)
                                    {
                                        // country belongs to EU - charge tax and live
                                        ShippingBLL._UpdateDestinationForTaxWizard(intCountryID, drwCountry("D_ShippingZoneID"), 1, FixNullFromDB(drwCountry("D_Tax2")), drwCountry("D_ISOCode"), drwCountry("D_ISOCode3Letter"), drwCountry("D_ISOCodeNumeric"), FixNullFromDB(drwCountry("D_Region")), true, "", FixNullFromDB(drwCountry("D_TaxExtra")));
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }

                        // Set general.tax.euvatcountry config to selected country
                        strThisLocationISOCode = ShippingBLL._GetISOCodeByDestinationID(ddlEUCountries.SelectedValue);
                        KartSettingsManager.SetKartConfig("general.tax.euvatcountry", strThisLocationISOCode, false);

                        // Set TaxRate Record 2 to entered tax rate value
                        TaxBLL._UpdateTaxRate(2, txtQVatRate.Text, "", "");
                    }
                    else
                    {
                        // No - Don't charge tax for all countries
                        foreach (DataRow drwCountry in dtbAllCountries.Rows)
                            ShippingBLL._UpdateDestinationForTaxWizard(drwCountry("D_ID"), drwCountry("D_ShippingZoneID"), 0, FixNullFromDB(drwCountry("D_Tax2")), drwCountry("D_ISOCode"), drwCountry("D_ISOCode3Letter"), drwCountry("D_ISOCodeNumeric"), FixNullFromDB(drwCountry("D_Region")), drwCountry("D_Live"), "", FixNullFromDB(drwCountry("D_TaxExtra")));
                        // Set general.tax.euvatcountry config to blank, don't refresh cache yet
                        KartSettingsManager.SetKartConfig("general.tax.euvatcountry", "", false);
                    }

                    break;
                }

            case "US":
                {
                    // loop through all countries in the destination table
                    foreach (DataRow drwCountry in dtbAllCountries.Rows)
                    {
                        int intCountryID = drwCountry("D_ID");

                        if (lstRegionCountries.FirstOrDefault(item => item.CountryId == intCountryID) != null)
                        {
                            // a US state so set to live and only charge tax if its the base state
                            float sngD_Tax;
                            if (ddlUSStates.SelectedValue == intCountryID)
                                sngD_Tax = txtUSStateTaxRate.Text / (double)100;
                            else
                                sngD_Tax = 0;
                            ShippingBLL._UpdateDestinationForTaxWizard(intCountryID, drwCountry("D_ShippingZoneID"), sngD_Tax, FixNullFromDB(drwCountry("D_Tax2")), drwCountry("D_ISOCode"), drwCountry("D_ISOCode3Letter"), drwCountry("D_ISOCodeNumeric"), FixNullFromDB(drwCountry("D_Region")), true, "", FixNullFromDB(drwCountry("D_TaxExtra")));
                        }
                        else
                            // Not US State
                            if (intCountryID == 201)
                            // Hardcode - Turn off main USA destination record - ID : 201
                            ShippingBLL._UpdateDestinationForTaxWizard(intCountryID, drwCountry("D_ShippingZoneID"), drwCountry("D_Tax"), FixNullFromDB(drwCountry("D_Tax2")), drwCountry("D_ISOCode"), drwCountry("D_ISOCode3Letter"), drwCountry("D_ISOCodeNumeric"), FixNullFromDB(drwCountry("D_Region")), false, "", FixNullFromDB(drwCountry("D_TaxExtra")));
                        else
                            // just don't charge tax
                            ShippingBLL._UpdateDestinationForTaxWizard(intCountryID, drwCountry("D_ShippingZoneID"), 0, FixNullFromDB(drwCountry("D_Tax2")), drwCountry("D_ISOCode"), drwCountry("D_ISOCode3Letter"), drwCountry("D_ISOCodeNumeric"), FixNullFromDB(drwCountry("D_Region")), drwCountry("D_Live"), "", FixNullFromDB(drwCountry("D_TaxExtra")));
                    }

                    // Set EU country to blank, so no EU VAT active
                    KartSettingsManager.SetKartConfig("general.tax.euvatcountry", "", false);
                    break;
                }

            case "CANADA":
                {
                    foreach (DataRow drwCountry in dtbAllCountries.Rows)
                    {
                        int intCountryID = drwCountry("D_ID");

                        if (lstRegionCountries.FirstOrDefault(item => item.CountryId == intCountryID) != null)
                        {
                            // a Canadian state so set to live 

                            float sngGST;
                            float sngPST;
                            string strTaxExtra;
                            if (ddlCanadaBaseProvince.SelectedValue == intCountryID)
                            {
                                sngGST = txtCanadaGST.Text;
                                sngPST = txtCanadaPST.Text;
                                if (chkCanadaPSTChargedOnPST.Checked)
                                    strTaxExtra = "compounded";
                                else
                                    strTaxExtra = "";
                            }
                            else
                            {
                                sngGST = 0;
                                sngPST = 0;
                                strTaxExtra = null;
                            }
                            ShippingBLL._UpdateDestinationForTaxWizard(intCountryID, drwCountry("D_ShippingZoneID"), IIf(sngGST != 0, sngPST, FixNullFromDB(drwCountry("D_Tax2"))), IIf(sngPST != 0, sngPST, FixNullFromDB(drwCountry("D_Tax2"))), drwCountry("D_ISOCode"), drwCountry("D_ISOCode3Letter"), drwCountry("D_ISOCodeNumeric"), FixNullFromDB(drwCountry("D_Region")), true, "", strTaxExtra);
                        }
                        else
                            // Not a Canadian State
                            if (intCountryID == 39)
                            // Hardcode - Turn off main CANADA destination record - ID : 39
                            ShippingBLL._UpdateDestinationForTaxWizard(intCountryID, drwCountry("D_ShippingZoneID"), drwCountry("D_Tax"), FixNullFromDB(drwCountry("D_Tax2")), drwCountry("D_ISOCode"), drwCountry("D_ISOCode3Letter"), drwCountry("D_ISOCodeNumeric"), FixNullFromDB(drwCountry("D_Region")), false, "", FixNullFromDB(drwCountry("D_TaxExtra")));
                        else
                            // just don't charge tax
                            ShippingBLL._UpdateDestinationForTaxWizard(intCountryID, drwCountry("D_ShippingZoneID"), 0, FixNullFromDB(drwCountry("D_Tax2")), drwCountry("D_ISOCode"), drwCountry("D_ISOCode3Letter"), drwCountry("D_ISOCodeNumeric"), FixNullFromDB(drwCountry("D_Region")), drwCountry("D_Live"), "", FixNullFromDB(drwCountry("D_TaxExtra")));
                    }

                    // Set EU country to blank, so no EU VAT active
                    KartSettingsManager.SetKartConfig("general.tax.euvatcountry", "", false);
                    break;
                }

            case "SIMPLE":
                {
                    foreach (DataRow drwCountry in dtbAllCountries.Rows)
                    {
                        int intCountryID = drwCountry("D_ID");
                        float sngD_Tax;

                        if (ddlSimpleBaseCountry.SelectedValue == intCountryID)
                        {

                            // Set tax rate
                            sngD_Tax = txtSimpleTaxRate.Text / (double)100;

                            // charge tax and set to live
                            ShippingBLL._UpdateDestinationForTaxWizard(intCountryID, drwCountry("D_ShippingZoneID"), sngD_Tax, FixNullFromDB(drwCountry("D_Tax2")), drwCountry("D_ISOCode"), drwCountry("D_ISOCode3Letter"), drwCountry("D_ISOCodeNumeric"), FixNullFromDB(drwCountry("D_Region")), true, "", FixNullFromDB(drwCountry("D_TaxExtra")));
                        }
                        else
                            // just don't charge tax
                            ShippingBLL._UpdateDestinationForTaxWizard(intCountryID, drwCountry("D_ShippingZoneID"), 0, FixNullFromDB(drwCountry("D_Tax2")), drwCountry("D_ISOCode"), drwCountry("D_ISOCode3Letter"), drwCountry("D_ISOCodeNumeric"), FixNullFromDB(drwCountry("D_Region")), drwCountry("D_Live"), "", FixNullFromDB(drwCountry("D_TaxExtra")));
                    }

                    // Set EU country to blank, so no EU VAT active
                    KartSettingsManager.SetKartConfig("general.tax.euvatcountry", "", false);
                    break;
                }
        }

        // CONFIG SETTINGS - set "general.tax.pricesinctax" and "frontend.display.showtax" values
        KartSettingsManager.SetKartConfig("general.tax.pricesinctax", ddlPricesIncTaxConfig.SelectedValue, false);
        KartSettingsManager.SetKartConfig("frontend.display.showtax", ddlShowTaxConfig.SelectedValue, false);

        _UC_LangContainer.Visible = false;
        ShowMasterUpdateMessage();
        pnlSummary.Visible = true;
        CkartrisBLL.RefreshKartrisCache();
    }

    protected List<KartrisClasses.Country> GetCountryListFromTaxConfig(string strTaxRegime = "")
    {
        List<KartrisClasses.Country> lstCountries = null;
        XmlDocument docXML = new XmlDocument();

        if (strTaxRegime == "")
            strTaxRegime = TaxRegime.Name;

        // Load the TaxScheme Config file file from web root
        docXML.Load(HttpContext.Current.Server.MapPath("~/TaxRegime.Config"));

        XmlNodeList lstNodes;
        XmlNode ndeDestination;
        XmlNode ndeDestinationFilter;


        string strRegimeNodePath = "/configuration/" + strTaxRegime + "TaxRegime/DestinationsFilter";
        string strKeyFieldName = "";
        string strKeyFieldValue = "";

        ndeDestinationFilter = docXML.SelectSingleNode(strRegimeNodePath);
        try
        {
            strKeyFieldName = ndeDestinationFilter.Attributes.GetNamedItem("KeyFieldName").Value;
            strKeyFieldValue = ndeDestinationFilter.Attributes.GetNamedItem("KeyFieldValue").Value;
        }
        catch (Exception ex)
        {
        }

        // Retrieve all countries
        DataTable tblAllCountries = ShippingBLL._GetDestinationsByLanguage(Session("_LANG"));
        lstCountries = new List<KartrisClasses.Country>();
        foreach (DataRow drwCountry in tblAllCountries.Rows)
            lstCountries.Add(KartrisClasses.Country.Get(drwCountry("D_ID")));

        // Key field value is given which means we need to filter the countries
        if (!string.IsNullOrEmpty(strKeyFieldValue))
        {
            if (!string.IsNullOrEmpty(strKeyFieldName))
            {
                List<KartrisClasses.Country> lstValidCountries = new List<KartrisClasses.Country>();
                foreach (var objCountry in lstCountries)
                {
                    // Exclude 9,39,201 - Australia, Canada, USA as we're sure that we're dealing with provinces/states when this bit of code is hit
                    if (objCountry.IsoCode.ToLower == strKeyFieldValue.ToLower() & (objCountry.CountryId != 9 & objCountry.CountryId != 39 & objCountry.CountryId != 201))
                        lstValidCountries.Add(objCountry);
                }
                return lstValidCountries;
            }
        }
        else
        {
            // No Key Field given - Retrieve valid destination list from the Tax Regime Config
            lstNodes = docXML.SelectNodes(strRegimeNodePath + "/Destination");
            lstCountries = new List<KartrisClasses.Country>();
            foreach (var ndeDestination in lstNodes)
                lstCountries.Add(KartrisClasses.Country.GetByIsoCode(ndeDestination.Attributes.GetNamedItem("Key").Value));
        }

        return lstCountries;
    }
}
