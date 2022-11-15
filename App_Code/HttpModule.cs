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
using System.IO;

using System.Web;

public class KartrisHttpModule : IHttpModule
{
    private static bool HasAppStarted = false;
    private static readonly object _syncObject = new object();

    public void Dispose()
    {
    }

    public void Init(HttpApplication context)
    {
        context.BeginRequest += Kartris_BeginRequest;
        context.PreRequestHandlerExecute += Kartris_PreRequestExecute;

        if (!HasAppStarted)
        {
            lock (_syncObject)
            {
                if (!HasAppStarted)
                {
                    // Run application StartUp code here
                    if (context.Application["DBConnected"])
                    {
                        KartSettingsManager.RefreshCache();
                        KartSettingsManager.RefreshCurrencyCache();
                        LanguagesBLL.GetLanguages(true); // Refresh cache for the front end dropdown
                        KartSettingsManager.RefreshLanguagesCache();
                        KartSettingsManager.RefreshLETypesFieldsCache();
                        KartSettingsManager.RefreshTopListProductsCache();
                        KartSettingsManager.RefreshNewestProductsCache();
                        KartSettingsManager.RefreshFeaturedProductsCache();
                        KartSettingsManager.RefreshCustomerGroupsCache();
                        KartSettingsManager.RefreshSuppliersCache();
                        KartSettingsManager.RefreshTaxRateCache();
                        KartSettingsManager.RefreshSiteNewsCache();
                        CkartrisBLL.LoadSkinConfigToCache();
                        HttpRuntime.Cache.Insert("CategoryMenuKey", DateTime.Now);
                        HasAppStarted = true;
                    }
                }
            }
        }
    }

    public void Kartris_BeginRequest(object sender, EventArgs e)
    {
        HttpContext context = HttpContext.Current;
        string strCurrentPath = context.Request.RawUrl.ToString();
        if (!context.Application["CorrectGlobalizationTag"])
        {
            // read the web.config file and check if the globalization tags and language string providers are properly set
            // - if not then we probably need to redirect to 'admin/install.aspx'
            string webConfigFile = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "web.config");
            System.Xml.XmlTextReader webConfigReader = new System.Xml.XmlTextReader(new StreamReader(webConfigFile));

            if (!((webConfigReader.ReadToFollowing("globalization")) && (webConfigReader.GetAttribute("resourceProviderFactoryType") == "SqlResourceProviderFactory")))
            {
                // display 
                if (Strings.InStr(strCurrentPath.ToLower(), "admin/install.aspx") == 0 && Strings.InStr(strCurrentPath.ToLower(), ".css") == 0 && Strings.InStr(strCurrentPath.ToLower(), ".png") == 0 && Strings.InStr(strCurrentPath.ToLower(), ".axd") == 0)
                {
                    context.Response.Redirect("~/Admin/Install.aspx");
                    context.Response.End();
                }
            }
            else
                context.Application["CorrectGlobalizationTag"] = true;

            webConfigReader.Close();
        }

        // Rewrite URLs if SEO friendly URLs config is on
        if (context.Application["DBConnected"] && KartSettingsManager.GetKartConfig("general.seofriendlyurls.enabled") == "y")
        {
            // ' Support for the old SEO format, the code below will still handle the page     
            if ((Strings.InStr(strCurrentPath, "/c-p-") || Strings.InStr(strCurrentPath, "/c-s-") || Strings.InStr(strCurrentPath, "/p-") || Strings.InStr(strCurrentPath, "/n-") || Strings.InStr(strCurrentPath, "/k-")) && Strings.InStr(strCurrentPath.ToLower(), "webkit.js") == 0 && Strings.Right(strCurrentPath, 1) == "/")
            {
                if (strCurrentPath.EndsWith("/"))
                    strCurrentPath = strCurrentPath.TrimEnd("/");
                strCurrentPath += ".aspx";
                strCurrentPath = strCurrentPath.Replace("/c-p-", "__c-p-");
                strCurrentPath = strCurrentPath.Replace("/c-s-", "__c-s-");
                strCurrentPath = strCurrentPath.Replace("/p-", "__p-");
                strCurrentPath = strCurrentPath.Replace("/n-", "__n-");
                strCurrentPath = strCurrentPath.Replace("/k-", "__k-");
                if (strCurrentPath != "")
                {
                    context.Response.StatusCode = 301;
                    context.Response.Status = "301 Moved Permanently";
                    context.Response.AddHeader("Location", strCurrentPath);
                }
            }

            // Check if the current URL contains either "/c-p-" or "/p-" which indicates
            // it is a category or product fakelink which needs to be rewritten
            if ((Strings.InStr(strCurrentPath, "__c-p-") || Strings.InStr(strCurrentPath, "__c-s-") || Strings.InStr(strCurrentPath, "__p-") || Strings.InStr(strCurrentPath, "__n-") || Strings.InStr(strCurrentPath, "/t-") || Strings.InStr(strCurrentPath, "__k-")) && Strings.InStr(strCurrentPath.ToLower(), "webkit.js") == 0)
            {
                strCurrentPath = SiteMapHelper.SEORewrite(strCurrentPath);
                if (strCurrentPath != "")
                    context.RewritePath(strCurrentPath);
            }
        }
    }

    public void Kartris_PreRequestExecute(object sender, EventArgs e)
    {
        // this event occurs after the beginrequest event above
        // but occurs just before ASP.NET starts executing an event handler (ie. , a page , user control or a web service).

        HttpContext context = HttpContext.Current;
        string strCurrentPath = context.Request.RawUrl.ToLower().ToString();
    }
}
