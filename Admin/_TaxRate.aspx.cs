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
using KartSettingsManager;

partial class Admin_Taxrate : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Kartris", "PageTitle_TaxRates") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        if (ConfigurationManager.AppSettings("TaxRegime").ToLower != "us" & ConfigurationManager.AppSettings("TaxRegime").ToLower != "simple")
        {
            if (!Page.IsPostBack)
                LoadTaxRates();
            mvwTax.SetActiveView(viwTaxRates);
        }
        else
            mvwTax.SetActiveView(viwMultistateTax);
    }

    private void LoadTaxRates()
    {
        DataTable tblTaxRates = new DataTable();
        tblTaxRates = GetTaxRateFromCache();

        rptTaxRate.DataSource = tblTaxRates;
        rptTaxRate.DataBind();
    }

    protected void btnUpdateTaxes_Click(object sender, System.EventArgs e)
    {
        UpdateTaxRates();
    }

    private void UpdateTaxRates()
    {
        foreach (RepeaterItem itm in rptTaxRate.Items)
        {
            if (itm.ItemType == ListItemType.AlternatingItem || itm.ItemType == ListItemType.Item)
            {
                byte intTaxID = System.Convert.ToByte((Literal)itm.FindControl("litTaxRateID").Text);
                float numTaxRate = HandleDecimalValues((TextBox)itm.FindControl("txtTaxRate").Text);
                string strQBRefCode = (TextBox)itm.FindControl("txtQBRefCode").Text;
                string strMessage = "";
                if (!TaxBLL._UpdateTaxRate(intTaxID, numTaxRate, strQBRefCode, strMessage))
                {
                    _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                    return;
                }
            }
        }
        RefreshTaxRateCache();
        LoadTaxRates();
        (Skins_Admin_Template)this.Master.DataUpdated();
    }

    protected void rptTaxRate_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            (TextBox)e.Item.FindControl("txtTaxRate").Text = _HandleDecimalValues((TextBox)e.Item.FindControl("txtTaxRate").Text);
    }
}
