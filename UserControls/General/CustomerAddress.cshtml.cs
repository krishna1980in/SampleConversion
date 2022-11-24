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
using Payment;
using CkartrisDisplayFunctions;
using KartrisClasses;

/// <summary>

/// ''' Reusable Customer Address Input Control

/// ''' </summary>

/// ''' <remarks>Medz</remarks>
public partial class UserControls_Front_CustomerAddress : ValidatableUserControl
{
    public event CountryUpdatedEventHandler CountryUpdated;

    public delegate void CountryUpdatedEventHandler(object sender, System.EventArgs e);

    public string DisplayType
    {
        get
        {
            return hidDisplayType.Value;
        }
        set
        {
            hidDisplayType.Value = value;
            if (hidDisplayType.Value == "Billing")
            {
                lblName.Text = GetGlobalResourceObject("Address", "FormLabel_CardHolderName");
                lblCompany.Text = GetGlobalResourceObject("Address", "FormLabel_CardHolderCompany");
                lblStreetAddress.Text = GetGlobalResourceObject("Address", "FormLabel_CardHolderStreetAddress");
            }
            else if (hidDisplayType.Value == "Shipping")
            {
                lblName.Text = GetGlobalResourceObject("Address", "FormLabel_RecipientName");
                lblCompany.Text = GetGlobalResourceObject("Address", "FormLabel_ShippingCompany");
                lblStreetAddress.Text = GetGlobalResourceObject("Address", "FormLabel_ShippingAddress");
            }
        }
    }

    public string AddressType
    {
        get
        {
            return hidAddressType.Value;
        }
        set
        {
            hidAddressType.Value = value;
        }
    }

    public string CountryName
    {
        get
        {
            return ddlCountry.SelectedItem.Text;
        }
    }

    public bool AutoPostCountry
    {
        set
        {
            ddlCountry.AutoPostBack = value;
        }
    }

    public bool EnableValidation
    {
        set
        {
            valLastNameRequired.Enabled = value;
            valTelePhoneRequired.Enabled = value;
            valCityRequired.Enabled = value;
            valAddressRequired.Enabled = value;
        }
    }

    public string PhoneNoticeText
    {
        get
        {
            return lblPhoneNotice.InnerText;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                phdPhoneNotice.Visible = true;
                lblPhoneNotice.InnerText = value;
            }
        }
    }

    public bool ShowValidationSummary
    {
        set
        {
        }
    }

    public new override string ValidationGroup
    {
        set
        {
            base.ValidationGroup = value;
            // FirstNameRequired.ValidationGroup = value
            valLastNameRequired.ValidationGroup = value;
            // valStateRequired.ValidationGroup = value
            valTelePhoneRequired.ValidationGroup = value;
            valCityRequired.ValidationGroup = value;
            valZipCodeRequired.ValidationGroup = value;
            valAddressRequired.ValidationGroup = value;
            // AddressInputValidationSummary.ValidationGroup = value
            DisplayType = value;
        }
    }

    public bool ShowNameFields
    {
        set
        {
            phdName.Visible = value;
        }
    }

    public bool ShowSaveAs
    {
        set
        {
            phdSaveAs.Visible = value;
        }
    }

    public string SaveAs
    {
        get
        {
            return txtSaveAs.Text.Trim();
        }
    }

    public string FullName
    {
        get
        {
            return StripHTML(txtLastName.Text.Trim());
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
                txtLastName.Text = value;
        }
    }

    public string LastName
    {
        get
        {
            return StripHTML(txtLastName.Text.Trim());
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
                txtLastName.Text = value;
        }
    }

    public string Company
    {
        get
        {
            return StripHTML(txtCompanyName.Text.Trim());
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
                txtCompanyName.Text = value;
        }
    }

    public string Address
    {
        get
        {
            return StripHTML(txtAddress.Text.Trim());
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
                txtAddress.Text = value;
        }
    }

    public string City
    {
        get
        {
            return StripHTML(txtCity.Text.Trim());
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
                txtCity.Text = value;
        }
    }

    public string State
    {
        get
        {
            if (string.IsNullOrEmpty(txtState.Text) || string.IsNullOrEmpty(ddlCountry.SelectedValue))
                return StripHTML(txtState.Text);
            return StripHTML(txtState.Text);
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
                txtState.Text = value;
        }
    }

    public int CountryId
    {
        get
        {
            if (string.IsNullOrEmpty(ddlCountry.SelectedValue))
                return 0;
            return int.Parse(ddlCountry.SelectedValue);
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
                ddlCountry.SelectedValue = value;
        }
    }

    public string Postcode
    {
        get
        {
            return StripHTML(txtZipCode.Text.Trim());
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
                txtZipCode.Text = value;
        }
    }

    public string Phone
    {
        get
        {
            return StripHTML(txtPhone.Text);
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
                txtPhone.Text = value;
        }
    }

    public int AddressID
    {
        get
        {
            return System.Convert.ToInt32(hidAddressID.Value);
        }
    }

    public Address InitialAddressToDisplay
    {
        set
        {
            txtSaveAs.Text = value.Label;
            txtLastName.Text = value.FullName;
            txtCompanyName.Text = IIf(string.IsNullOrEmpty(value.Company), string.Empty, value.Company);
            txtAddress.Text = value.StreetAddress;
            txtCity.Text = value.TownCity;
            txtState.Text = value.County;
            // Imported customer data with bad country details
            // can cause errors, so we need a try/catch for safety
            try
            {
                ddlCountry.SelectedValue = value.CountryID.ToString();
            }
            catch (Exception ex)
            {
                ddlCountry.SelectedValue = 0;
            }

            txtZipCode.Text = value.Postcode;
            txtPhone.Text = value.Phone;
            hidAddressID.Value = value.ID;
            hidAddressType.Value = value.Type;
        }
    }

    public Address EnteredAddress
    {
        get
        {
            Address a = new Address(LastName, Company, Address, City, State, Postcode, CountryId, Phone, AddressID, SaveAs, AddressType);
            a.Country = Country.Get(CountryId);
            return a;
        }
    }

    private const string AddressChangedJavascriptBlockFormat = "function AddressChanged() {{ " + "if (($get('{0}').value) && " + "($get('{1}').value) && " + "($get('{2}').value) && " + "($get('{3}').value)) " + "{4}();" + "}}";

    private string m_addressChangedJavacriptCallBack = null;

    public string AddressChangedJavacriptCallBack
    {
        set
        {
            txtAddress.Attributes.Add("onblur", "javascript:AddressChanged();");
            txtCity.Attributes.Add("onblur", "javascript:AddressChanged();");
            txtState.Attributes.Add("onblur", "javascript:AddressChanged();");
            ddlCountry.Attributes.Add("onblur", "javascript:AddressChanged();");
            txtZipCode.Attributes.Add("onblur", "javascript:AddressChanged();");
            m_addressChangedJavacriptCallBack = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (KartSettingsManager.GetKartConfig("frontend.checkout.postcoderequired") == "y")
        {
            valZipCodeRequired.Enabled = true;
            lblPostcode.CssClass = "requiredfield";
        }
        else
        {
            valZipCodeRequired.Enabled = false;
            lblPostcode.CssClass = "";
        }
        if (m_addressChangedJavacriptCallBack != null)
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "AddressChanged", string.Format(AddressChangedJavascriptBlockFormat, txtAddress.ClientID, txtCity.ClientID, txtState.ClientID, txtZipCode.ClientID, m_addressChangedJavacriptCallBack), true);
        if (!Page.IsPostBack)
        {
            try
            {
                ddlCountry.SelectedValue = KartSettingsManager.GetKartConfig("frontend.checkout.defaultcountry");
            }
            catch (Exception ex)
            {
                ddlCountry.SelectedIndex = 0;
            }
        }
    }

    public void Clear()
    {
        txtSaveAs.Text = "";
        txtLastName.Text = "";
        txtCompanyName.Text = "";
        txtAddress.Text = "";
        txtCity.Text = "";
        txtState.Text = "";
        txtZipCode.Text = "";
        txtPhone.Text = "";
        try
        {
            ddlCountry.SelectedValue = KartSettingsManager.GetKartConfig("frontend.checkout.defaultcountry");
        }
        catch (Exception ex)
        {
            ddlCountry.SelectedIndex = 0;
        }
        hidAddressID.Value = 0;
        hidAddressType.Value = "u";
    }

    protected void ddlCountry_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        if (ddlCountry.AutoPostBack)
            CountryUpdated?.Invoke(sender, e);
    }
}
