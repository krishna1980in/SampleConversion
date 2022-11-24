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

partial class UserControls_Skin_CurrencyDropdown : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!System.Web.UI.Control.Page.IsPostBack)
        {
            DataTable tblCurrencies = KartSettingsManager.GetCurrenciesFromCache(); // CurrenciesBLL.GetCurrencies()
            DataRow[] drwLiveCurrencies = tblCurrencies.Select("CUR_Live = 1");
            if (drwLiveCurrencies.Length > 0)
            {
                ddlCurrency.Items.Clear();
                for (byte i = 0; i <= drwLiveCurrencies.Length - 1; i++)
                    ddlCurrency.Items.Add(new ListItem(drwLiveCurrencies[i]("CUR_Symbol") + " " + drwLiveCurrencies[i]("CUR_ISOCode"), drwLiveCurrencies[i]("CUR_ID")));
            }
            ddlCurrency.SelectedIndex = ddlCurrency.Items.IndexOf(ddlCurrency.Items.FindByValue(System.Web.UI.UserControl.Session["CUR_ID"]));
        }
    }

    protected void ddlCurrency_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        System.Web.UI.UserControl.Session["CUR_ID"] = ddlCurrency.SelectedValue;
        System.Web.UI.UserControl.Response.Redirect(System.Web.UI.UserControl.Request.Url.ToString());
    }
}
