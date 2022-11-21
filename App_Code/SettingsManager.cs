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
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Web.Configuration;
using System.Web.Caching;

/// <summary>

/// ''' Kartris Config Settings Manager

/// ''' </summary>
public class KartSettingsManager
{

    /// <summary>
    ///     ''' Checks if a commercial license file is present in the root. Note that this does
    ///     ''' not check the validity of such a license, only that a file is present.
    ///     ''' </summary>
    public static bool HasCommercialLicense()
    {
        if (File.Exists(HttpContext.Current.Server.MapPath("~/license.config")))
            return true;
        else
            return false;
    }

    /// <summary>
    ///     ''' Checks if a commercial license file is present in the root. Note that this does
    ///     ''' not check the validity of such a license, only that a file is present.
    ///     ''' </summary>
    public static string PoweredByLink()
    {
        StringBuilder sbdLink = new StringBuilder();

        // Build up string of the 'powered by kartris' tag
        sbdLink.Append("<a onmouseover=\"this.style.backgroundColor = '#AD004D';this.style.color = '#fff';\" onmouseout=\"this.style.backgroundColor = '#fff';this.style.color = '#AD004D';\" style=\"line-height: 13px;display:inline-block;padding:1px 2px 1px 3px;font-size:7pt;font-family:tahoma,arial,helvetica;position:fixed;bottom:0;right:30px;color:#AD004D;background-color:#fff;\"" + Constants.vbCrLf);
        sbdLink.Append(" href=\"http://www.kartris.com/\" title=\"Kartris - &copy;2020, CACTUSOFT. Distributed free and without warranty under the terms of the GNU GPL.\">Powered by <span style=\"font-weight: bold\">kartris</span></a>");

        return sbdLink.ToString();
    }

    /// <summary>
    ///     ''' Set/update a config setting value. The equivalent of CactuShop's GetAppVar function
    ///     ''' </summary>
    ///     ''' <param name="Config_Name">The name of the config setting you want to retrieve.</param>
    ///     ''' <param name="RefreshCache">Optional: Default value is false. Flag to refresh config cache. </param>
    public static string GetKartConfig(string Config_Name, bool RefreshCache = false)
    {
        if (RefreshCache | HttpRuntime.Cache("KartSettingsCache") == null)
            KartSettingsManager.RefreshCache();

        if (Config_Name != "")
        {
            DataTable tblWebSettings = (DataTable)HttpRuntime.Cache("KartSettingsCache");
            DataRow[] drwWebSettings = tblWebSettings.Select("CFG_Name = '" + Config_Name + "'");
            if (drwWebSettings.Length == 0)
                // The Config_Name was not found
                return "";
            else
                // The Config_Name was found: return row
                return drwWebSettings[0]("CFG_Value").ToString();
        }
        else
            // this condition is still here in case you call:  GetKartConfig("", True)
            // this will return nothing but still refresh the cache
            return "";
    }

    /// <summary>
    ///     ''' Set/update a config setting value. The equivalent of CactuShop's SetAppVar function
    ///     ''' </summary>
    ///     ''' <param name="Config_Name">The name of the config setting you want to update.</param>
    ///     ''' <param name="Config_Value">The new config setting value.</param>
    ///     ''' <param name="RefreshCache">Optional: Refresh config cache. Set to either true or false. </param>
    public new static void SetKartConfig(string Config_Name, string Config_Value, bool RefreshCache = true)
    {
        // Update Database            
        ConfigBLL._UpdateConfigValue(Config_Name, Config_Value);
    }

    /// <summary>
    ///     ''' Refresh config settings cache.
    ///     ''' </summary>
    public new static void RefreshCache()
    {
        if (!HttpRuntime.Cache("KartSettingsCache") == null)
            HttpRuntime.Cache.Remove("KartSettingsCache");
        DataTable tblWebSettings = new DataTable();
        tblWebSettings = ConfigBLL.GetConfigCacheData();
        HttpRuntime.Cache.Add("KartSettingsCache", tblWebSettings, null/* TODO Change to default(_) if this is not a reference type */, DateTime.MaxValue, TimeSpan.Zero, Caching.CacheItemPriority.High, null/* TODO Change to default(_) if this is not a reference type */);
    }

    // '=======  Currency Caching  =======
    public static void RefreshCurrencyCache()
    {
        if (!HttpRuntime.Cache("KartrisCurrenciesCache") == null)
            HttpRuntime.Cache.Remove("KartrisCurrenciesCache");
        DataTable tblCurrencies = CurrenciesBLL._GetCurrencies();
        HttpRuntime.Cache.Add("KartrisCurrenciesCache", tblCurrencies, null/* TODO Change to default(_) if this is not a reference type */, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.High, null/* TODO Change to default(_) if this is not a reference type */);
    }

    public static DataTable GetCurrenciesFromCache()
    {
        if (HttpRuntime.Cache("KartrisCurrenciesCache") == null)
            KartSettingsManager.RefreshCurrencyCache();
        return (DataTable)HttpRuntime.Cache("KartrisCurrenciesCache");
    }

    // '=======  Languages Caching  =======
    public static void RefreshLanguagesCache()
    {
        LanguagesBLL.GetLanguages(true);
        if (!HttpRuntime.Cache("KartrisLanguagesCache") == null)
            HttpRuntime.Cache.Remove("KartrisLanguagesCache");
        DataTable tblLanguages = LanguagesBLL._GetLanguages();
        HttpRuntime.Cache.Add("KartrisLanguagesCache", tblLanguages, null/* TODO Change to default(_) if this is not a reference type */, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.High, null/* TODO Change to default(_) if this is not a reference type */);
    }

    public static DataTable GetLanguagesFromCache()
    {
        if (HttpRuntime.Cache("KartrisLanguagesCache") == null)
            KartSettingsManager.RefreshLanguagesCache();
        return (DataTable)HttpRuntime.Cache("KartrisLanguagesCache");
    }

    // '=======  LE Types' Fields Caching  =======
    public static void RefreshLETypesFieldsCache()
    {
        if (!HttpRuntime.Cache("KartrisLETypesFieldsCache") == null)
            HttpRuntime.Cache.Remove("KartrisLETypesFieldsCache");
        DataTable tblLETypeFields = LanguagesBLL._GetTypeFieldDetails();
        HttpRuntime.Cache.Add("KartrisLETypesFieldsCache", tblLETypeFields, null/* TODO Change to default(_) if this is not a reference type */, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.High, null/* TODO Change to default(_) if this is not a reference type */);
    }

    public static DataTable GetLETypesFieldsFromCache()
    {
        if (HttpRuntime.Cache("KartrisLETypesFieldsCache") == null)
            KartSettingsManager.RefreshLETypesFieldsCache();
        return (DataTable)HttpRuntime.Cache("KartrisLETypesFieldsCache");
    }

    // '=======  Featured Products Caching  ======
    public static void RefreshFeaturedProductsCache()
    {
        if (!HttpRuntime.Cache("KartrisFeaturedProductsCache") == null)
            HttpRuntime.Cache.Remove("KartrisFeaturedProductsCache");
        DataTable tblFeaturedProducts = ProductsBLL.GetFeaturedProductForCache();
        HttpRuntime.Cache.Add("KartrisFeaturedProductsCache", tblFeaturedProducts, null/* TODO Change to default(_) if this is not a reference type */, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.Normal, null/* TODO Change to default(_) if this is not a reference type */);
    }

    public static DataTable GetFeaturedProductsFromCache()
    {
        if (HttpRuntime.Cache("KartrisFeaturedProductsCache") == null)
            KartSettingsManager.RefreshFeaturedProductsCache();
        return (DataTable)HttpRuntime.Cache("KartrisFeaturedProductsCache");
    }

    // '=======  Newest Products Caching  ======
    public static void RefreshNewestProductsCache()
    {
        if (!HttpRuntime.Cache("KartrisNewestProductsCache") == null)
            HttpRuntime.Cache.Remove("KartrisNewestProductsCache");
        DataTable tblNewestProducts = ProductsBLL.GetNewestProductsForCache();
        HttpRuntime.Cache.Add("KartrisNewestProductsCache", tblNewestProducts, null/* TODO Change to default(_) if this is not a reference type */, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.Normal, null/* TODO Change to default(_) if this is not a reference type */);
    }

    public static DataTable GetNewestProductsFromCache()
    {
        if (HttpRuntime.Cache("KartrisNewestProductsCache") == null)
            KartSettingsManager.RefreshNewestProductsCache();
        return (DataTable)HttpRuntime.Cache("KartrisNewestProductsCache");
    }

    // '=======  TopList Products Caching  ======
    public static void RefreshTopListProductsCache()
    {
        if (!HttpRuntime.Cache("KartrisTopListProductsCache") == null)
            HttpRuntime.Cache.Remove("KartrisTopListProductsCache");
        DataTable tblTopListProducts = ProductsBLL.GetTopListProductsForCache();
        HttpRuntime.Cache.Add("KartrisTopListProductsCache", tblTopListProducts, null/* TODO Change to default(_) if this is not a reference type */, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.Normal, null/* TODO Change to default(_) if this is not a reference type */);
    }

    public static DataTable GetTopListProductsFromCache()
    {
        if (HttpRuntime.Cache("KartrisTopListProductsCache") == null)
            KartSettingsManager.RefreshTopListProductsCache();
        return (DataTable)HttpRuntime.Cache("KartrisTopListProductsCache");
    }

    // '=======  Suppliers Caching  ======
    public static void RefreshSuppliersCache()
    {
        if (!HttpRuntime.Cache("KartrisSuppliersCache") == null)
            HttpRuntime.Cache.Remove("KartrisSuppliersCache");
        UsersBLL objUsersBLL = new UsersBLL();
        DataTable tblSuppliers = objUsersBLL._GetSuppliersForCache();
        HttpRuntime.Cache.Add("KartrisSuppliersCache", tblSuppliers, null/* TODO Change to default(_) if this is not a reference type */, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.Low, null/* TODO Change to default(_) if this is not a reference type */);
    }

    public static DataTable GetSuppliersFromCache()
    {
        if (HttpRuntime.Cache("KartrisSuppliersCache") == null)
            KartSettingsManager.RefreshSuppliersCache();
        return (DataTable)HttpRuntime.Cache("KartrisSuppliersCache");
    }

    // '=======  Customer Groups Caching  ======
    public static void RefreshCustomerGroupsCache()
    {
        if (!HttpRuntime.Cache("KartrisCustomerGroupsCache") == null)
            HttpRuntime.Cache.Remove("KartrisCustomerGroupsCache");
        UsersBLL objUsersBLL = new UsersBLL();
        DataTable tblCG = objUsersBLL._GetCustomerGroupsForCache();
        HttpRuntime.Cache.Add("KartrisCustomerGroupsCache", tblCG, null/* TODO Change to default(_) if this is not a reference type */, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.Low, null/* TODO Change to default(_) if this is not a reference type */);
    }

    public static DataTable GetCustomerGroupsFromCache()
    {
        if (HttpRuntime.Cache("KartrisCustomerGroupsCache") == null)
            KartSettingsManager.RefreshCustomerGroupsCache();
        return (DataTable)HttpRuntime.Cache("KartrisCustomerGroupsCache");
    }

    // '=======  TaxRates Caching  ======
    public static void RefreshTaxRateCache()
    {
        if (!HttpRuntime.Cache("KartrisTaxRatesCache") == null)
            HttpRuntime.Cache.Remove("KartrisTaxRatesCache");
        DataTable tblTaxRates = TaxBLL._GetTaxRatesForCache();
        foreach (var drwRate in tblTaxRates.Rows)
            // Get rid of trailing zeroes from decimal format
            drwRate("T_TaxRate") = CkartrisDisplayFunctions.FixDecimal(drwRate("T_TaxRate"));
        HttpRuntime.Cache.Add("KartrisTaxRatesCache", tblTaxRates, null/* TODO Change to default(_) if this is not a reference type */, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.Low, null/* TODO Change to default(_) if this is not a reference type */);
    }

    public static DataTable GetTaxRateFromCache()
    {
        if (HttpRuntime.Cache("KartrisTaxRatesCache") == null)
            KartSettingsManager.RefreshTaxRateCache();
        return (DataTable)HttpRuntime.Cache("KartrisTaxRatesCache");
    }

    // '=======  SiteNews Caching  ======
    public static void RefreshSiteNewsCache()
    {
        if (!HttpRuntime.Cache("KartrisSiteNewsCache") == null)
            HttpRuntime.Cache.Remove("KartrisSiteNewsCache");
        DataTable tblSiteNews = NewsBLL._GetNewsForCache();
        HttpRuntime.Cache.Add("KartrisSiteNewsCache", tblSiteNews, null/* TODO Change to default(_) if this is not a reference type */, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.Normal, null/* TODO Change to default(_) if this is not a reference type */);
    }

    public static DataTable GetSiteNewsFromCache()
    {
        if (HttpRuntime.Cache("KartrisSiteNewsCache") == null)
            KartSettingsManager.RefreshSiteNewsCache();
        return (DataTable)HttpRuntime.Cache("KartrisSiteNewsCache");
    }

    public static bool CreateKartrisCookie()
    {
        if (HttpContext.Current.Request.Cookies(HttpSecureCookie.GetCookieName()) == null)
        {
            HttpCookie cokKartris = new HttpCookie(HttpSecureCookie.GetCookieName());

            cokKartris.Expires = System.DateTime.Now.AddDays(21);

            // check if the language culture of the browser is one of the supported languages - [language-region]
            if (LanguagesBLL.GetLanguageIDByCulture_s(System.Threading.Thread.CurrentThread.CurrentCulture.Name) > 0)
                cokKartris.Values("KartrisUserCulture") = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            else
            {
                // check if the language culture of the browser is one of the supported languages - [language] only
                byte intLangID = LanguagesBLL.GetLanguageIDByCultureUI_s(System.Threading.Thread.CurrentThread.CurrentCulture.Name);

                if (intLangID > 0)
                    cokKartris.Values("KartrisUserCulture") = LanguagesBLL.GetCultureByLanguageID_s(intLangID);
                else
                    cokKartris.Values("KartrisUserCulture") = LanguagesBLL.GetCultureByLanguageID_s(LanguagesBLL.GetDefaultLanguageID());
            }

            HttpContext.Current.Request.Cookies.Add(cokKartris);
            return true;
        }
        else
            return false;
    }
}
