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
using System.Threading;
using System.Globalization;
using System.Web.Configuration;
using System.Web.HttpContext;
using CkartrisFormatErrors;
using KartSettingsManager;
using System.Web.UI;
using System.Text;
using System.Web.UI.HtmlControls;
public abstract class PageBaseClass : System.Web.UI.Page {
    
    private string _MetaKeywords;
    
    private string _MetaDescription;
    
    private string _CanonicalTag;
    
    private KartrisMemberShipUser _CurrentLoggedUser;
    
    protected override void Render(HtmlTextWriter writer) {
        // Create our own mechanism to store the page output
        // This way we can alter the text of the page prior to
        // rendering.
        // If Not Page.IsPostBack Then
        StringBuilder sbdPageSource = new StringBuilder();
        StringWriter objStringWriter = new StringWriter(sbdPageSource);
        HtmlTextWriter objHtmlWriter = new HtmlTextWriter(objStringWriter);
        base.Render(objHtmlWriter);
        // Insert Google Analytics, if necessary
        if ((KartSettingsManager.GetKartConfig("general.google.analytics.webpropertyid") != "")) {
            this.InsertGoogleAnalyticsCode(sbdPageSource, KartSettingsManager.GetKartConfig("general.google.analytics.webpropertyid"));
        }
        
        // Add copyright notice - NOTE, this should not be
        // removed, under the GPL this conforms to the definition
        // of a copyright notification message
        this.RunGlobalReplacements(sbdPageSource);
        // Move viewstate to end of page
        // Could give SEO benefit, depending on who you listen to.
        // You might think a regex is more efficient, but this seems
        // to be marginally faster
        string strPageHTML = sbdPageSource.ToString();
        int numStartPoint = strPageHTML.IndexOf("<input type=\"hidden\" name=\"__VIEWSTATE\"");
        if ((numStartPoint >= 0)) {
            int numEndPoint = (strPageHTML.IndexOf("/>", numStartPoint) + 2);
            string strViewstateInput = strPageHTML.Substring(numStartPoint, (numEndPoint - numStartPoint));
            strPageHTML = strPageHTML.Remove(numStartPoint, (numEndPoint - numStartPoint));
            int numFormEndStart = (strPageHTML.IndexOf("</form>") - 1);
            if ((numFormEndStart >= 0)) {
                strPageHTML = strPageHTML.Insert(numFormEndStart, strViewstateInput);
            }
            
        }
        
        // Output HTML with replacements and render changes
        writer.Write(strPageHTML);
    }
    
    // This creates the 'powered by kartris' tag in bottom right.
    protected void RunGlobalReplacements(StringBuilder sbdPageSource) {
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
    
    // This creates the Google Analytics javascript code
    // at the foot of front end pages if there is a value
    // in the general.google.analytics.webpropertyid config
    // setting.
    protected void InsertGoogleAnalyticsCode(StringBuilder sbdPageSource, string strGoogleWebPropertyID) {
        bool blnReplacedTag = false;
        string strReplacement = "";
        StringBuilder sbdLink = new StringBuilder();
        // Newest code as of 2019-11-28
        sbdLink.Append(("<!-- Google Analytics -->" + "\r\n"));
        sbdLink.Append(("<script Async src=\"https://www.googletagmanager.com/gtag/js?id=" 
                        + (strGoogleWebPropertyID + ("\"></script>" + "\r\n"))));
        sbdLink.Append(("<script>" + "\r\n"));
        sbdLink.Append(("  window.dataLayer = window.dataLayer || [];" + "\r\n"));
        sbdLink.Append(("  function gtag(){dataLayer.push(arguments);}" + "\r\n"));
        sbdLink.Append(("  gtag(\'js\', new Date());" + "\r\n"));
        sbdLink.Append(("  gtag(\'config\', \'" 
                        + (strGoogleWebPropertyID + ("\');" + "\r\n"))));
        sbdLink.Append(("</script>" + "\r\n"));
        sbdLink.Append(("<!-- End Google Analytics -->" + "\r\n"));
        // Google Analytics works in head tag, not close body
        try {
            sbdPageSource.Replace("<head id=\"Head1\">", ("<head id=\"Head1\">" + ("\r\n" 
                            + (sbdLink.ToString + "\r\n"))));
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
                sbdPageSource.Append(("\r\n" + sbdLink.ToString));
                blnReplacedTag = true;
            }
            catch (Exception ex) {
                // Oh dear
            }
            
        }
        
    }
    
    // This creates the Google Tag Manager javascript code
    // at the foot of front end pages if there is a value
    // in the general.google.tagmanager.webpropertyid config
    // setting.
    protected void InsertGoogleTagManagerCode(StringBuilder sbdPageSource, string strGoogleWebPropertyID) {
        bool blnReplacedTag = false;
        string strReplacement = "";
        StringBuilder sbdLink = new StringBuilder();
        // Newest code as of 2021-08-06
        sbdLink.Append(("<!-- Google Tag Manager -->" + "\r\n"));
        sbdLink.Append(("<script>(function(w,d,s,l,i){w[l]=w[l]||[];w[l].push({\'gtm.start\':" + "\r\n"));
        sbdLink.Append(("new Date().getTime(),event:\'gtm.js\'});var f=d.getElementsByTagName(s)[0]," + "\r\n"));
        sbdLink.Append(("j=d.createElement(s),dl=l!=\'dataLayer\'?\'&l=\'+l:\'\';j.async=true;j.src=" + "\r\n"));
        sbdLink.Append(("\'https://www.googletagmanager.com/gtm.js?id=\'+i+dl;f.parentNode.insertBefore(j,f);" + "\r\n"));
        sbdLink.Append(("})(window,document,\'script\',\'dataLayer\',\'" 
                        + (strGoogleWebPropertyID + ("\');</script>" + "\r\n"))));
        sbdLink.Append(("<!-- Google Tag Manager -->" + "\r\n"));
        // Google Tag Manager works in head tag, not close body
        try {
            sbdPageSource.Replace("<head id=\"Head1\">", ("<head id=\"Head1\">" + ("\r\n" 
                            + (sbdLink.ToString + "\r\n"))));
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
                sbdPageSource.Append(("\r\n" + sbdLink.ToString));
                blnReplacedTag = true;
            }
            catch (Exception ex) {
                // Oh dear
            }
            
        }
        
    }
    
    public string CanonicalTag {
        get {
            return _CanonicalTag;
        }
        set {
            _CanonicalTag = value;
        }
    }
    
    public string MetaKeywords {
        get {
            return _MetaKeywords;
        }
        set {
            _MetaKeywords = value;
        }
    }
    
    public string MetaDescription {
        get {
            return _MetaDescription;
        }
        set {
            _MetaDescription = value;
        }
    }
    
    public KartrisMemberShipUser CurrentLoggedUser {
        get {
            return _CurrentLoggedUser;
        }
        set {
            _CurrentLoggedUser = value;
        }
    }
    
    protected override void InitializeCulture() {
        if (!Page.IsPostBack) {
            CategorySiteMapProvider CatSiteMap;
            SiteMap.Provider;
            CategorySiteMapProvider;
            CatSiteMap.ResetSiteMap();
            long numLanguageID = CkartrisDataManipulation.NumSafe(Request.QueryString["L"]);
            if (!(numLanguageID == 0)) {
                Session("KartrisUserCulture") = Server.HtmlEncode(LanguagesBLL.GetCultureByLanguageID_s(numLanguageID));
                Session("LANG") = short.Parse(numLanguageID);
                Response.Cookies["Kartris"]["KartrisUserCulture"] = Session("KartrisUserCulture");
            }
            else {
                try {
                    // no language querystring passed so get the value from the cookie, set the session-object with the data from the cookie.
                    Session("KartrisUserCulture") = Server.HtmlEncode(Request.Cookies[HttpSecureCookie.GetCookieName()]["KartrisUserCulture"]);
                    Session("LANG") = short.Parse(LanguagesBLL.GetLanguageIDByCulture_s(Request.Cookies[HttpSecureCookie.GetCookieName()]["KartrisUserCulture"]));
                }
                catch (Exception ex) {
                    CreateKartrisCookie();
                    // no language querystring passed so get the value from the cookie, set the session-object with the data from the cookie.
                    Session("KartrisUserCulture") = Server.HtmlEncode(Request.Cookies[HttpSecureCookie.GetCookieName()]["KartrisUserCulture"]);
                    Session("LANG") = short.Parse(LanguagesBLL.GetLanguageIDByCulture_s(Request.Cookies[HttpSecureCookie.GetCookieName()]["KartrisUserCulture"]));
                }
                
            }
            
            // Set the UICulture and the Culture with a value stored in a Session-object.
            if (User.Identity.IsAuthenticated) {
                try {
                    if ((_CurrentLoggedUser == null)) {
                        _CurrentLoggedUser = Web.Security.Membership.GetUser();
                    }
                    
                    if ((_CurrentLoggedUser == null)) {
                        throw new Exception("Invalid User!");
                    }
                    
                }
                catch (Exception ex) {
                    _CurrentLoggedUser = null;
                    Web.Security.FormsAuthentication.SignOut();
                    Response.Redirect("~/CustomerAccount.aspx");
                }
                
            }
            else {
                _CurrentLoggedUser = null;
            }
            
            try {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session("KartrisUserCulture").ToString);
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Session("KartrisUserCulture").ToString);
            }
            catch (Exception ex) {
                Session("KartrisUserCulture") = LanguagesBLL.GetCultureByLanguageID_s(LanguagesBLL.GetDefaultLanguageID());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session("KartrisUserCulture").ToString);
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Session("KartrisUserCulture").ToString);
            }
            
            base.InitializeCulture();
        }
        
    }
    
    private void Page_Error(object sender, System.EventArgs e) {
        if (Server.GetLastError.ToString.ToUpper().Contains("SYSTEM.WEB.HTTPREQUESTVALIDATIONEXCEPTION")) {
            Server.ClearError();
            Session("Error") = "invalidrequest";
            Response.Redirect("~/Error.aspx");
        }
        else {
            LogError();
        }
        
    }
    
    // '' <summary>
    // '' Skin selection from skin.config
    // '' </summary>
    protected override void OnPreInit(EventArgs e) {
        // ==================================================
        // SKIN HANDLING (start)
        // ==================================================
        string strSkinOverride = "";
        int numSubSiteID = 0;
        try {
            numSubSiteID = SubSitesBLL.ViewingSubSite(Session("SUB_ID"));
            // Return 0 if default skin
        }
        catch (Exception ex) {
            numSubSiteID = 0;
        }
        
        if ((numSubSiteID > 0)) {
            object tblSubSites = SubSitesBLL.GetSubSiteByID(numSubSiteID);
            strSkinOverride = ("~/Skins/" 
                        + (tblSubSites.Rows.Item[0].Item["SUB_Skin"] + "/Template.master"));
        }
        else {
            // --------------------------------------------------
            // 2. Look for skin config rules
            // The skin.config file is cached by
            // LoadSkinConfigToCache in the kartris.vb file to
            // "tblSkinRules"... if this exists, then we look
            // to implement those rules here.
            // --------------------------------------------------
            if (HttpRuntime.Cache["tblSkinRules"]) {
                IsNot;
                null;
                string strScriptName = Path.GetFileName(Request.PhysicalPath).ToLower;
                int intCustomerGroupID = 0;
                int intProductID = 0;
                int intCategoryID = 0;
                int intCustomerID = 0;
                // --------------------------------------------------
                // 2a. Look for product or category ID
                // Need this so can implement product or category
                // skin.
                // --------------------------------------------------
                try {
                    string strCurrentPath = Request.RawUrl.ToString.ToLower;
                    if (!(((strCurrentPath.IndexOf("/skins/") + 1) 
                                > 0) 
                                || (((strCurrentPath.IndexOf("/javascript/") + 1) 
                                > 0) 
                                || ((strCurrentPath.IndexOf("/images/") + 1) 
                                > 0)))) {
                        switch (strScriptName) {
                            case "product.aspx":
                                intProductID = Request.QueryString["ProductID"];
                                break;
                            case "category.aspx":
                                intCategoryID = Request.QueryString["CategoryID"];
                                break;
                        }
                    }
                    
                }
                catch (Exception ex) {
                }
                
                // --------------------------------------------------
                // 2b. Look for user ID
                // Need this so can implement customer-specific
                // skin.
                // --------------------------------------------------
                if (User.Identity.IsAuthenticated) {
                    try {
                        intCustomerID = CurrentLoggedUser.ID;
                        intCustomerGroupID = CurrentLoggedUser.CustomerGroupID;
                    }
                    catch (Exception ex) {
                    }
                    
                }
                
                // --------------------------------------------------
                // 2c. Determine which skin to use
                // Use the SkinMasterConfig function in kartris.vb,
                // pass in the product ID, category IDs, customer
                // ID, customer group ID and page/script name, and
                // then this looks up which skin to use.
                // --------------------------------------------------
                strSkinOverride = CkartrisBLL.SkinMasterConfig(Page, intProductID, intCategoryID, intCustomerID, intCustomerGroupID, strScriptName);
            }
            
        }
        
        // --------------------------------------------------
        // 3. Check if skin specified for this language
        // This uses the SkinMaster function in kartris.vb.
        // We only run this if section (1) above did not
        // return any value.
        // --------------------------------------------------
        string strSkinMaster;
        if ((strSkinOverride != "")) {
            strSkinMaster = strSkinOverride;
        }
        else {
            strSkinMaster = CkartrisBLL.SkinMaster(this, Session("LANG"));
        }
        
        if ((strSkinMaster != "")) {
            MasterPageFile = strSkinMaster;
        }
        
        // --------------------------------------------------
        // 4a. Check if using HomePage master page
        // Often sites will want to use a different layout on
        // the home (default/index) page of the site. If we
        // are viewing the Default.aspx page, we check to see
        // if the skin includes a HomePage.master page. If
        // so, we use this. If not, just revert to the normal
        // Template.master instead.
        // Notice that we need to check for this HomePage
        // file in whichever skin is being used (which might
        // be the one specified in the Language, but could
        // be an overridden one, set in (1) or (2) above.
        // --------------------------------------------------
        if ((Path.GetFileName(Request.PhysicalPath).ToLower == "default.aspx")) {
            if ((this.MasterPageFile != "~/Skins/Kartris/Template.master")) {
                if ((strSkinOverride != "")) {
                    // Look for HomePage template in the overridden skin
                    if (File.Exists(Server.MapPath(("~/Skins/" 
                                        + (strSkinOverride + "/HomePage.master"))))) {
                        this.MasterPageFile = ("~/Skins/" 
                                    + (strSkinOverride + "/HomePage.master"));
                    }
                    
                }
                else {
                    // Look for HomePage template in the language-specified or default skin
                    if (File.Exists(Server.MapPath(("~/Skins/" 
                                        + (CkartrisBLL.Skin(Session("LANG")) + "/HomePage.master"))))) {
                        this.MasterPageFile = ("~/Skins/" 
                                    + (CkartrisBLL.Skin(Session("LANG")) + "/HomePage.master"));
                    }
                    
                }
                
            }
            
        }
        
        // --------------------------------------------------
        // 4b. Is this a customer invoice page (front end)?
        // If so, we look in the present skin folder for an
        // Invoice.master. If there is one, we use it, if not,
        // we default back to the one in the Admin skin.
        // --------------------------------------------------
        if ((Path.GetFileName(Request.PhysicalPath).ToLower == "customerinvoice.aspx")) {
            if ((strSkinOverride != "")) {
                if (File.Exists(Server.MapPath(("~/Skins/" 
                                    + (strSkinOverride + "/Invoice.master"))))) {
                    this.MasterPageFile = ("~/Skins/" 
                                + (strSkinOverride + "/Invoice.master"));
                }
                
            }
            else {
                // If the skin has a HomePage.master file, we use this
                if (File.Exists(Server.MapPath(("~/Skins/" 
                                    + (CkartrisBLL.Skin(Session("LANG")) + "/Invoice.master"))))) {
                    this.MasterPageFile = ("~/Skins/" 
                                + (CkartrisBLL.Skin(Session("LANG")) + "/Invoice.master"));
                }
                
            }
            
        }
        
        // --------------------------------------------------
        // 5. Is visitor on version of IE lower than IE9?
        // If so, the Foundation responsive interface won't
        // work, so we can fall back to an alternative
        // non-responsive skin, if present. This is sought
        // by name - it should be named same as other skin
        // but with 'NonResponsive' added to name.
        // --------------------------------------------------
        if (((Request.Browser.Browser == "IE") 
                    && (Request.Browser.MajorVersion <= 8))) {
            try {
                if ((strSkinOverride != "")) {
                    // Look for template in the overridden skin
                    if (File.Exists(Server.MapPath(("~/Skins/" 
                                        + (strSkinOverride + "NonResponsive/Template.master"))))) {
                        this.MasterPageFile = ("~/Skins/" 
                                    + (strSkinOverride + "NonResponsive/Template.master"));
                    }
                    
                }
                else if (File.Exists(Server.MapPath(("~/Skins/" 
                                    + (CkartrisBLL.Skin(Session("LANG")) + "NonResponsive/Template.master"))))) {
                    this.MasterPageFile = ("~/Skins/" 
                                + (CkartrisBLL.Skin(Session("LANG")) + "NonResponsive/Template.master"));
                }
                
            }
            catch (Exception ex) {
                // Do nothing
            }
            
        }
        
        // Check if store is closed, and user is not
        // logged into back end - if so, redirect to
        // closed message
        if (!HttpSecureCookie.IsBackendAuthenticated()) {
            if (!Current.Request.Url.AbsoluteUri.ToLower.Contains("callback")) {
                // We want to allow the callback to work so we can test payment
                // gateways even while the site is closed
                if ((GetKartConfig("general.storestatus") != "open")) {
                    Server.Transfer("~/Closed.aspx");
                }
                
            }
            
            // 301 Redirect Code
            // This redirects to the official webshopURL domain, if another is used to access the site.
            if ((!Current.Request.IsSecureConnection() 
                        && !(GetKartConfig("general.security.ssl") == "e"))) {
                if (((Request.Url.ToString.ToLower.IndexOf(CkartrisBLL.WebShopURL.ToLower) + 1) 
                            == 0)) {
                    string strRedirectURL = CkartrisDisplayFunctions.CleanURL(Request.RawUrl.ToLower);
                    // remove the web shop folder if present - webshopurl already contains this
                    strRedirectURL = strRedirectURL.Replace(("/" + CkartrisBLL.WebShopFolder), "");
                    if ((strRedirectURL.Substring(0, 1) == "/")) {
                        strRedirectURL = strRedirectURL.Substring(1);
                    }
                    
                    Response.Status = "301 Moved Permanently";
                    Response.AddHeader("Location", (CkartrisBLL.WebShopURL + strRedirectURL));
                }
                
            }
            
            // If the site requires users to login, redirect
            // to the login page if user is not logged in
            string strUserAccess = GetKartConfig("frontend.users.access").ToLower();
            if (((strUserAccess == "yes") 
                        && (!Request.Path.ToString.Contains("/CustomerAccount.aspx") 
                        && (!Request.Path.ToString.Contains("/PasswordReset.aspx") 
                        && (!Request.Path.ToString.ToLower.Contains("callback") 
                        && !User.Identity.IsAuthenticated))))) {
                Response.Redirect(("CustomerAccount.aspx?return=" + HttpContext.Current.Request.RawUrl));
            }
            
            base.OnPreInit(e);
        }
        
    }
    
    void Page_PreLoad(object sender, System.EventArgs e) {
        if (!Page.IsPostBack) {
            // Get site name and set default page title to this. It
            // will be overridden on most pages.
            KartSettingsManager.GetKartConfig("");
            Page.Title = Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));
        }
        
    }
    
    private void Page_LoadComplete(object sender, System.EventArgs e) {
        if (!Page.IsPostBack) {
            HtmlMeta tagHtmlMeta = new HtmlMeta();
            HtmlHead tagHtmlHead = ((HtmlHead)(Page.Header));
            tagHtmlMeta.Attributes.Add("name", "description");
            if (!string.IsNullOrEmpty(_MetaDescription)) {
                tagHtmlMeta.Attributes.Add("content", _MetaDescription);
            }
            else {
                tagHtmlMeta.Attributes.Add("content", GetGlobalResourceObject("Kartris", "ContentText_DefaultMetaDescription"));
            }
            
            tagHtmlHead.Controls.Add(tagHtmlMeta);
            tagHtmlMeta = new HtmlMeta();
            tagHtmlMeta.Attributes.Add("name", "keywords");
            if (!string.IsNullOrEmpty(_MetaKeywords)) {
                tagHtmlMeta.Attributes.Add("content", _MetaKeywords);
            }
            else {
                tagHtmlMeta.Attributes.Add("content", GetGlobalResourceObject("Kartris", "ContentText_DefaultMetaKeywords"));
            }
            
            tagHtmlHead.Controls.Add(tagHtmlMeta);
            // Copyright
            WebControls.Literal litLicenceNo = new WebControls.Literal();
            litLicenceNo.Text = ("\r\n" + ("\r\n" + "<!-- Kartris - Copyright 2020 CACTUSOFT - www.kartris.com -->"));
            Page.Controls.Add(litLicenceNo);
            // Add the Canonical Tag if set
            if (!string.IsNullOrEmpty(_CanonicalTag)) {
                HtmlLink tagCanonical = new HtmlLink();
                tagCanonical.Attributes.Add("rel", "canonical");
                tagCanonical.Href = _CanonicalTag;
                tagHtmlHead.Controls.Add(tagCanonical);
            }
            
        }
        
    }
    
    private void Page_Load(object sender, System.EventArgs e) {
        if (!Page.IsPostBack) {
            // Check if this is an affiliate link and we
            // need to log affiliate credit
            AffiliateBLL.CheckAffiliateLink();
            // Check if we are emptying the basket. Callbacks
            // from payment gateways often use this so when
            // a customer is returned, they don't still have
            // items in their basket.
            if ((Request.QueryString["strWipeBasket"] == "yes")) {
                Kartris.Basket BasketObject = new Kartris.Basket();
                BasketBLL.DeleteBasket();
                Session("Basket") = null;
            }
            
            if (Session("Error")) {
                IsNot;
                null;
                if ((Session("Error") == "invalidrequest")) {
                    string strBodyText = ("alert(\'" 
                                + (GetGlobalResourceObject("Kartris", "ContentText_HTMLNotAllowed") + "\');"));
                    ScriptManager.RegisterStartupScript(this, Page.GetType, "AlertUser", strBodyText, true);
                    Session("Error") = null;
                }
                
            }
            
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
            
            // Add links to javascript such a KartrisInterface.js
            this.RegisterScripts();
        }
        
    }
    
    private void Page_PreRender(object sender, System.EventArgs e) {
        // If Not String.IsNullOrEmpty(Current.Session("Back_Auth")) Then
        SSLHandler.CheckFrontSSL(Page.User.Identity.IsAuthenticated);
        // End If
    }
    
    private void RegisterScripts() {
        // Extender.RegisterScripts()
        string folderPath = WebConfigurationManager.AppSettings.Get("Kartris-JavaScript-Path");
        if (string.IsNullOrEmpty(folderPath)) {
            folderPath = "~/JavaScript";
        }
        
        string filePath = ( folderPath.EndsWith("/") ? (folderPath + "KartrisInterface.js") : (folderPath + "/KartrisInterface.js") );
        Page.ClientScript.RegisterClientScriptInclude(this.GetType(), this.GetType().ToString(), Page.ResolveUrl(filePath));
    }
}
