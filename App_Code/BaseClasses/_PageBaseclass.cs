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
using System.Globalization;
using System.Diagnostics;
using System.Threading;
using KartSettingsManager;
using System.Web.UI;
public abstract class _PageBaseClass : System.Web.UI.Page {
    
    protected override void Render(HtmlTextWriter writer) {
        // Create our own mechanism to store the page output
        // This way we can alter the text of the page prior to
        // rendering.
        // If Not Page.IsPostBack Then
        StringBuilder sbdPageSource = new StringBuilder();
        StringWriter objStringWriter = new StringWriter(sbdPageSource);
        HtmlTextWriter objHtmlWriter = new HtmlTextWriter(objStringWriter);
        base.Render(objHtmlWriter);
        // Add copyright notice - NOTE, this should not be
        // removed, under the GPL this conforms to the definition
        // of a copyright notification message
        this.RunGlobalReplacements(sbdPageSource);
        // Output replacements
        writer.Write(sbdPageSource.ToString());
    }
    
    protected override void InitializeCulture() {
        if (!IsPostBack) {
            // Set the UICulture and the Culture with a value stored in a Session-object.
            string strBackUserCulture;
            if ((int.Parse(Session("_LANG")) > 0)) {
                strBackUserCulture = LanguagesBLL.GetCultureByLanguageID_s(int.Parse(Session("_LANG")));
            }
            else {
                strBackUserCulture = LanguagesBLL.GetCultureByLanguageID_s(LanguagesBLL.GetDefaultLanguageID);
                Session("_LANG") = LanguagesBLL.GetDefaultLanguageID;
            }
            
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(strBackUserCulture);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            base.InitializeCulture();
        }
        
    }
    
    private void Page_Error(object sender, System.EventArgs e) {
        CkartrisFormatErrors.LogError();
    }
    
    private void Page_PreInit(object sender, System.EventArgs e) {
        // Theme = "Admin"
        MasterPageFile = "~/Skins/Admin/Template.master";
    }
    
    private void Page_Init(object sender, System.EventArgs e) {
        // Additional security - this ensures that all of our viewstates are unique and is tied to the user's session
        ViewStateUserKey = Session.SessionID;
    }
    
    protected override void OnPreLoad(System.EventArgs e) {
        Page.Title = GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");
        // 301 Redirect Code
        // On the back end, we want this to be a little smarter than the front. If you restore
        // a local db to a live site, very often you will still have the local webshopURL set up.
        // If we simply redirect to the webshopURL setting, then it will keep sending you to your
        // local site backend instead of allowing you in to the live site in order to change this
        // setting.
        // But we do want to redirect from domain.xyz to www.domain.xyz if someone uses that in
        // order that SSL sites don't show security issues.
        // So what we ideally want is to redirect to the webshopURL only if it is the same as 
        // the entered URL but with or without www.
        if (((Request.Url.ToString.ToLower.IndexOf(CkartrisBLL.WebShopURL.ToLower) + 1) 
                    == 0)) {
            // URL doesn't match the webshopURL setting, so check if it is close (www only difference)
            if ((((Request.Url.ToString.ToLower.Replace("://", "://www.").IndexOf(CkartrisBLL.WebShopURL.ToLower) + 1) 
                        == 0) 
                        && ((Request.Url.ToString.ToLower.IndexOf(CkartrisBLL.WebShopURL.ToLower.Replace("://", "://www.")) + 1) 
                        == 0))) {
                // webshopURL very different from entered one, do nothing so user can login here
                // and access back end in order to update webshopURL
            }
            else {
                // Domain entered very similar to one in webshopURL, e.g. entered domain without www,
                // so we redirect to correct one to ensure SSL if present doesn't give security error
                string strRedirectURL = CkartrisDisplayFunctions.CleanURL(Request.RawUrl.ToLower);
                // remove the web shop folder if present - webshopurl already contains this
                strRedirectURL = strRedirectURL.Replace(("/" + CkartrisBLL.WebShopFolder.ToLower), "");
                if ((strRedirectURL.Substring(0, 1) == "/")) {
                    strRedirectURL = strRedirectURL.Substring(1);
                }
                
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", (CkartrisBLL.WebShopURL + strRedirectURL));
            }
            
        }
        
    }
    
    private void Page_Load(object sender, System.EventArgs e) {
        if (!IsPostBack) {
            // Postbacks don't work on ipads and some other Apple devices
            // because they're assumed to be generic primitive devices not
            // capable of an 'uplevel' experience. This fixes it.
            string strUserAgent = Request.UserAgent;
            if (strUserAgent) {
                IsNot;
                (null 
                            && (((strUserAgent.IndexOf("iPhone", StringComparison.CurrentCultureIgnoreCase) >= 0) 
                            || ((strUserAgent.IndexOf("iPad", StringComparison.CurrentCultureIgnoreCase) >= 0) 
                            || (strUserAgent.IndexOf("iPod", StringComparison.CurrentCultureIgnoreCase) >= 0))) 
                            && (strUserAgent.IndexOf("Safari", StringComparison.CurrentCultureIgnoreCase) < 0)));
                this.ClientTarget = "uplevel";
            }
            
            // Get user's IP address
            string strClientIP = CkartrisEnvironment.GetClientIPAddress();
            // Check matches specified IPs in web.config, if not blank
            object strBackEndIPLock = ConfigurationManager.AppSettings("BackEndIpLock").ToString();
            if ((strBackEndIPLock != "")) {
                try {
                    string[] arrBackendIPs = strBackEndIPLock.Split(",");
                    bool blnFullIP;
                    bool blnAllowIP = false;
                    for (int x = 0; (x 
                                <= (arrBackendIPs.Count - 1)); x++) {
                        // check if the IP is a range or a full IP, if its a full ip then it must be matched exactly
                        if ((arrBackendIPs[x].Split(".").Count == 4)) {
                            blnFullIP = true;
                        }
                        else {
                            blnFullIP = false;
                        }
                        
                        if (( blnFullIP ? (arrBackendIPs[x] == strClientIP) : (strClientIP.Substring(0, arrBackendIPs[x].Length) == arrBackendIPs[x]) )) {
                            // ok, let 'em in
                            blnAllowIP = true;
                            break;
                        }
                        
                    }
                    
                    if (!blnAllowIP) {
                        Response.Write("You are not authorized to view this page");
                        Response.End();
                    }
                    
                }
                catch (Exception ex) {
                    Response.Write("Invalid IP Lock Config");
                    Response.End();
                }
                
            }
            
            // Check cookie security
            HttpCookie cokKartris = Request.Cookies[HttpSecureCookie.GetCookieName("BackAuth")];
            string[] arrAuth = null;
            Session("Back_Auth") = "";
            if (cokKartris) {
                IsNot;
                null;
                arrAuth = HttpSecureCookie.Decrypt(cokKartris.Value);
                if (arrAuth) {
                    IsNot;
                    null;
                    if ((UBound(arrAuth) == 8)) {
                        if ((!string.IsNullOrEmpty(arrAuth[0]) 
                                    && ((strClientIP == arrAuth[7]) 
                                    && LoginsBLL.Validate(arrAuth[0], arrAuth[6], true)))) {
                            Session("Back_Auth") = cokKartris.Value;
                            Session("_LANG") = arrAuth[4];
                            Session("_USER") = arrAuth[0];
                            Session("_UserID") = LoginsBLL._GetIDbyName(arrAuth[0]);
                        }
                        else {
                            Session("Back_Auth") = "";
                            cokKartris = new HttpCookie(HttpSecureCookie.GetCookieName("BackAuth"));
                            cokKartris.Expires = CkartrisDisplayFunctions.NowOffset.AddDays(-, 1D);
                            Response.Cookies.Add(cokKartris);
                        }
                        
                    }
                    
                }
                
            }
            
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            string strScriptURL = Request.RawUrl.Substring((Request.Path.LastIndexOf("/") + 1));
            if (string.IsNullOrEmpty(Session("Back_Auth"))) {
                if ((strScriptURL.Substring(0, 11).ToLower() != "default.aspx")) {
                    Response.Redirect(("~/Admin/Default.aspx?page=" + strScriptURL));
                }
                else {
                    string strScriptName = Request.Path.Substring((Request.Path.LastIndexOf("/") + 1));
                    SiteMapNode nodeCurrent = SiteMap.Providers("_KartrisSiteMap").FindSiteMapNodeFromKey(("~/Admin/" + strScriptName));
                    if (!(nodeCurrent == null)) {
                        string strNodeValue = nodeCurrent.Item["Value"];
                        if ((UBound(arrAuth) == 8)) {
                            // If user doesn't have product permissions then hide category menu and set splitterbar width to 0
                            if (!bool.Parse(arrAuth[2])) {
                                Page.Master.FindControl("_UC_CategoryMenu").Visible = false;
                                Page.Master.FindControl("litHiddenCatMenu").Visible = true;
                                Page.Master.FindControl("splMainPage");
                                VwdCms.SplitterBar;
                                Width = 40;
                            }
                            
                            switch (strNodeValue) {
                                case "orders":
                                    bool blnOrders = bool.Parse(arrAuth[3]);
                                    if (!blnOrders) {
                                        Response.Write("You are not authorized to view this page");
                                        Response.End();
                                    }
                                    
                                    break;
                                case "products":
                                    bool blnProducts = bool.Parse(arrAuth[2]);
                                    if (!blnProducts) {
                                        Response.Write("You are not authorized to view this page");
                                        Response.End();
                                    }
                                    
                                    break;
                                case "config":
                                    bool blnConfig = bool.Parse(arrAuth[1]);
                                    if (!blnConfig) {
                                        Response.Write("You are not authorized to view this page");
                                        Response.End();
                                    }
                                    
                                    break;
                                case "support":
                                    bool blnSupport = bool.Parse(arrAuth[8]);
                                    if (!blnSupport) {
                                        Response.Write("You are not authorized to view this page");
                                        Response.End();
                                    }
                                    
                                    break;
                            }
                        }
                        else {
                            Response.Write("Invalid Cookie");
                            Response.End();
                        }
                        
                    }
                    else {
                        Response.Write(("Unknown Backend Page. This needs to be added to the Admin/_web.sitemap. If you don\'t want to show the" +
                            " link to the navigation menu. set its \'visible\' tag to \'false\' in the sitemap entry. <br/> e.g." + Server.HtmlEncode("<siteMapNode title=\"default\" url=\"~/Admin/_Default.aspx\" visible=\"false\" />")));
                        Response.End();
                    }
                    
                }
                
            }
            
        }
        
        // This creates the 'powered by kartris' tag in bottom right.
    }
    
    void RunGlobalReplacements(StringBuilder sbdPageSource) {
        // Any license.config file in the root will disable this,
        // but the license must be valid or it's a violation of 
        // the GPL v2 terms
        if (!KartSettingsManager.HasCommercialLicense) {
            bool blnReplacedTag = false;
            object strLinkText = KartSettingsManager.PoweredByLink;
            // Try to replace closing body tag with this
            try {
                sbdPageSource.Replace("</body", (strLinkText + ("\r\n" + "</body")));
                blnReplacedTag = true;
            }
            catch (Exception ex) {
                // Oh dear
            }
            
            // If they have somehow managed to remove or
            // obscure the closing body and form tags, we
            // just tag our code to the end of the page.
            // It is not XHTML compliant, but it should 
            // ensure the tag shows in any case.
            if ((blnReplacedTag == false)) {
                try {
                    sbdPageSource.Append(("\r\n" + strLinkText));
                    blnReplacedTag = true;
                }
                catch (Exception ex) {
                    // Oh dear
                }
                
            }
            
        }
        
    }
    
    private void Page_PreRender(object sender, System.EventArgs e) {
        SSLHandler.CheckBackSSL();
    }
}
