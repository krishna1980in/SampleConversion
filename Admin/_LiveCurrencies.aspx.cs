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
using CkartrisDisplayFunctions;
using CkartrisFormatErrors;
using KartSettingsManager;
using System.Xml.Linq;
using System.Xml;

partial class Admin_LiveCurrencies : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Currency", "PageTitle_Currencies") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        if (!Page.IsPostBack)
        {
            try
            {
                GetCurrenciesUpdate();
                phdCurrencies.Visible = true;
                phdCurrenciesNotAccessible.Visible = false;
            }
            catch (Exception ex)
            {
                phdCurrencies.Visible = false;
                phdCurrenciesNotAccessible.Visible = true;
            }
        }
    }

    private void GetCurrenciesUpdate()
    {
        DataTable tblTempCurrencies = new DataTable();
        tblTempCurrencies = GetCurrenciesFromCache(); // CurrenciesBLL._GetCurrencies()

        DataTable tblCurrencies = new DataTable();
        tblCurrencies.Columns.Add(new DataColumn("CurrencyID", Type.GetType("System.Byte")));
        tblCurrencies.Columns.Add(new DataColumn("CurrencyName", Type.GetType("System.String")));
        tblCurrencies.Columns.Add(new DataColumn("ISOCode", Type.GetType("System.String")));
        tblCurrencies.Columns.Add(new DataColumn("CurrentRate", Type.GetType("System.String")));
        tblCurrencies.Columns.Add(new DataColumn("NewRate", Type.GetType("System.String")));
        tblCurrencies.Columns.Add(new DataColumn("IsDefault", Type.GetType("System.Boolean")));
        StringBuilder sbdIsoList = new StringBuilder("");
        string strBaseISO = "";
        foreach (DataRow drwTempCurrency in tblTempCurrencies.Rows)
        {
            string strCurrencyName = "";
            LanguageElementsBLL objLanguageElementsBLL = new LanguageElementsBLL();
            strCurrencyName = System.Convert.ToString(objLanguageElementsBLL.GetElementValue(Session("_LANG"), LANG_ELEM_TABLE_TYPE.Currencies, LANG_ELEM_FIELD_NAME.Name, System.Convert.ToInt64(drwTempCurrency("CUR_ID"))));

            if (System.Convert.ToByte(drwTempCurrency("CUR_ID")) == CurrenciesBLL.GetDefaultCurrency())
            {
                strBaseISO = drwTempCurrency("CUR_ISOCode");
                tblCurrencies.Rows.Add(System.Convert.ToByte(drwTempCurrency("CUR_ID")), strCurrencyName, drwTempCurrency("CUR_ISOCode"), drwTempCurrency("CUR_ExchangeRate"), "", true);
            }
            else
                tblCurrencies.Rows.Add(System.Convert.ToByte(drwTempCurrency("CUR_ID")), strCurrencyName, drwTempCurrency("CUR_ISOCode"), drwTempCurrency("CUR_ExchangeRate"), "", false);

            sbdIsoList.Append(drwTempCurrency("CUR_ISOCode")); sbdIsoList.Append(",");
        }

        string strMessage = null;
        if (ReadLiveCurrencyRates(strBaseISO, sbdIsoList.ToString(), ref tblCurrencies, ref strMessage))
        {
            rptCurrencies.DataSource = tblCurrencies;
            rptCurrencies.DataBind();
            lnkUpdateCurrencies.Visible = true;
            updMain.Update();
        }
        else
        {
            if (!string.IsNullOrEmpty(strMessage))
                _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
            lnkUpdateCurrencies.Visible = false;
            throw new ApplicationException("");
        }
    }

    protected void rptCurrencies_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            string strCurrentRate = "";
            string strNewRate = "";

            strCurrentRate = (Literal)e.Item.FindControl("litCurrentRate").Text;
            strNewRate = (TextBox)e.Item.FindControl("txtNewRate").Text;

            if ((CheckBox)e.Item.FindControl("chkIsDefault").Checked)
                (TextBox)e.Item.FindControl("txtNewRate").Enabled = false;
            else
                (TextBox)e.Item.FindControl("txtNewRate").Enabled = true;

            if (strCurrentRate > strNewRate)
                (Literal)e.Item.FindControl("litCurrentRate").Text = "<span class=\"undervalue\">" + _HandleDecimalValues(strCurrentRate) + "</span>";
            else if (strCurrentRate < strNewRate)
                (Literal)e.Item.FindControl("litCurrentRate").Text = "<span class=\"overvalue\">" + _HandleDecimalValues(strCurrentRate) + "</span>";
            (TextBox)e.Item.FindControl("txtNewRate").Text = _HandleDecimalValues(strNewRate);
        }
    }

    protected void lnkUpdateCurrencies_Click(object sender, System.EventArgs e)
    {
        if (UpdateCurrencies())
        {
            RefreshCurrencyCache();
            GetCurrenciesUpdate();
            ShowMasterUpdateMessage();
        }
    }

    private bool UpdateCurrencies()
    {
        try
        {
            foreach (RepeaterItem itm in rptCurrencies.Items)
            {
                if (itm.ItemType == ListItemType.Item || itm.ItemType == ListItemType.AlternatingItem)
                {
                    string strCurrencyID = System.Convert.ToString((Literal)itm.FindControl("litCurrencyID").Text);
                    string strNewCurrency = System.Convert.ToString((TextBox)itm.FindControl("txtNewRate").Text);
                    if (Information.IsNumeric(strNewCurrency) && System.Convert.ToDecimal(strNewCurrency) > 0)
                        CurrenciesBLL._UpdateCurrencyRate(System.Convert.ToByte(strCurrencyID), HandleDecimalValues(strNewCurrency));
                }
            }
            return true;
        }
        catch (Exception ex)
        {
        }

        return false;
    }

    private decimal GetBitCoinRate(string strBaseIso)
    {
        string url = "https://bitpay.com/api/rates/" + strBaseIso;
        XDocument xmlDoc = null;
        decimal numValue = 0;

        // Put it in a try, in case a bad result is or some
        // other problem like an error
        try
        {
            // Grab Bitcoin feed with this code that prevents
            // timeouts taking the whole feed down, as at present
            // we expect Bitcoin feeds to be less reliable and
            // also not the primary currency on a site
            System.Net.HttpWebRequest reqFeed = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            reqFeed.Timeout = 1000; // milliseconds
            System.Net.WebResponse resFeed = reqFeed.GetResponse();
            Stream responseStream = resFeed.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string strResponse = reader.ReadToEnd();
            responseStream.Close();

            // Format of response as follows
            // {"code":"GBP","name":"Pound Sterling","rate":302.481272}
            Array aryResponse = Strings.Split(strResponse, "\"rate\":");
            strResponse = aryResponse(1).Replace("}", "");
            numValue = Math.Round(1 / (double)System.Convert.ToDecimal(strResponse), 12);
        }
        catch (Exception ex)
        {
            // Oh dear
            numValue = 0;
        }
        return numValue;
    }

    private bool ReadLiveCurrencyRates(string strBaseIso, string strIsoList, ref DataTable tblCurrencies, ref string strMessage)
    {
        string url = "http://www.ecb.int/stats/eurofxref/eurofxref-daily.xml";
        XDocument doc = XDocument.Load(url);

        XNamespace gesmes = "http://www.gesmes.org/xml/2002-08-01";
        XNamespace ns = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref";

        decimal decBaseISORate = 0;

        // This line retrieves the individual "cube" entries and outputs then as an object with currencyiso and rate properties
        var cubes = doc.Descendants(ns + "Cube").Where(x => x.Attribute("currency") != null).Select(x => new { Currency = System.Convert.ToString(x.Attribute("currency")), Rate = System.Convert.ToDecimal(x.Attribute("rate")) });

        if (cubes == null)
        {
            try
            {
                throw new ApplicationException(GetGlobalResourceObject("_Currency", "ContentText_LiveCurrencyReadError"));
            }
            catch (Exception ex)
            {
                CkartrisFormatErrors.ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            strMessage = GetGlobalResourceObject("_Currency", "ContentText_LiveCurrencyReadError");
            return false;
        }
        else
            // retrieve the Base ISO rate first to use as the converter value
            if (strBaseIso.ToUpper() == "EUR")
            decBaseISORate = 1;
        else
            foreach (object result in cubes)
            {
                if (result.Currency == strBaseIso)
                {
                    decBaseISORate = result.Rate; break;
                }
            }

        // loop through the results and add the new values to the currencies table
        if (cubes.Count() > 0)
        {
            foreach (DataRow row in tblCurrencies.Rows)
            {
                foreach (object result in cubes)
                {
                    if (row("ISOCode") == result.Currency)
                    {
                        // rates needs to be divided to the GBP value as they are originally computed against 1 EUR
                        row("NewRate") = Math.Round(result.Rate / (double)decBaseISORate, 8);
                        break;
                    }
                    else if (row("ISOCode") == "EUR")
                    {
                        // EUR is not in the returned XML so just always consider its value as 1 - its the XML's base currency 
                        // To get its actual rate, just divide it against the base iso rate
                        row("NewRate") = Math.Round(1 / (double)decBaseISORate, 8);
                        break;
                    }
                    else if (row("ISOCode") == "BTC")
                    {
                        row("NewRate") = GetBitCoinRate(strBaseIso);
                        break;
                    }
                }
            }
            return true;
        }
        else
        {
            try
            {
                throw new ApplicationException(GetGlobalResourceObject("_Currency", "ContentText_LiveCurrencyRequestError"));
            }
            catch (Exception ex)
            {
                CkartrisFormatErrors.ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            strMessage = GetGlobalResourceObject("_Currency", "ContentText_LiveCurrencyRequestError");
            return false;
        }
    }

    protected void ShowMasterUpdateMessage()
    {
        (Skins_Admin_Template)this.Master.DataUpdated();
    }
}
