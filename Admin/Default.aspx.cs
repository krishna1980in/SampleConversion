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
using CkartrisFormatErrors;
using KartSettingsManager;
using CkartrisDataManipulation;
using System.Web.HttpContext;
using CkartrisDisplayFunctions;

partial class Admin_Default : System.Web.UI.Page
{
    protected void Page_Error(object sender, System.EventArgs e)
    {
        LogError();
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        System.Web.UI.Control.Page.Title = System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "PageTitle_LogInToSite") + " | " + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        if (!System.Web.UI.Control.Page.IsPostBack)
        {
            HttpCookie cokKartris = System.Web.UI.Page.Request.Cookies(HttpSecureCookie.GetCookieName("BackAuth"));
            string[] arrAuth = null;
            System.Web.UI.Page.Session["Back_Auth"] = "";
            if (cokKartris != null)
            {
                arrAuth = HttpSecureCookie.Decrypt(cokKartris.Value);
                if (arrAuth != null)
                {
                    if (Information.UBound(arrAuth) == 8)
                    {
                        string strClientIP = CkartrisEnvironment.GetClientIPAddress();
                        if (!string.IsNullOrEmpty(arrAuth[0]) & strClientIP == arrAuth[7])
                        {
                            System.Web.UI.Page.Session["Back_Auth"] = cokKartris.Value;
                            System.Web.UI.Page.Session["_LANG"] = arrAuth[4];
                            System.Web.UI.Page.Session["_User"] = arrAuth[0];
                            System.Web.UI.Page.Session["_UserID"] = LoginsBLL._GetIDbyName(arrAuth[0]);
                        }
                    }
                }
            }
            divError.Visible = false;

            // Warn if wrong ASP.NET version running. With 2.0, the software may
            // partially function, so it's easy for users not to realize why
            // it's not running perfectly.
            string strASPNETVersion = Environment.Version.ToString();
            if (Strings.Left(strASPNETVersion, 1) != "4")
                phdASPNETWarning.Visible = true;
            else
                phdASPNETWarning.Visible = false;
        }
    }

    protected void Page_LoadComplete(object sender, System.EventArgs e)
    {
        if (!string.IsNullOrEmpty(System.Web.UI.Page.Session["Back_Auth"]))
        {
            RefreshSiteMap();
            string strRedirectTo = System.Web.UI.Page.Request.QueryString["page"];
            if (!string.IsNullOrEmpty(strRedirectTo))
                System.Web.UI.Page.Response.Redirect("~/Admin/" + strRedirectTo);
            else
                System.Web.UI.Page.Response.Redirect("~/Admin/_Default.aspx");
        }
    }

    protected void btnLogin_Click(object sender, System.EventArgs e)
    {
        string strUsername = txtUserName.Text;
        string strPassword = txtPass.Text;
        bool blnResult = LoginsBLL.Validate(strUsername, strPassword);
        if (blnResult)
        {
            string strClientIP = CkartrisEnvironment.GetClientIPAddress();

            DataTable tblLogInDetails = LoginsBLL.GetDetails(strUsername);

            string strHash = "";
            int numUserID = 0;

            {
                var withBlock = tblLogInDetails;
                numUserID = System.Convert.ToInt32(withBlock.Rows(0)("LOGIN_ID"));
                strHash = HttpSecureCookie.CreateHash(withBlock.Rows(0), strUsername, strPassword, strClientIP);
            }

            HttpCookie cokKartris = new HttpCookie(HttpSecureCookie.GetCookieName("BackAuth"), strHash);
            cokKartris.Expires = NowOffset.AddDays(1);
            if (SSLHandler.IsSSLEnabled)
                cokKartris.Secure = true;
            System.Web.UI.Page.Response.Cookies.Add(cokKartris);

            System.Web.UI.Page.Session["_UserID"] = numUserID;
            System.Web.UI.Page.Session["_USER"] = strUsername;
            System.Web.UI.Page.Session["Back_Auth"] = strHash;
            System.Web.UI.Page.Session["_LANG"] = tblLogInDetails.Rows(0)("LOGIN_LanguageID");
            System.Web.UI.Page.Session["LANG"] = System.Web.UI.Page.Session["_LANG"];
            divError.Visible = false;
        }
        else
            divError.Visible = true;
    }

    protected void Page_PreRender(object sender, System.EventArgs e)
    {
        if (!System.Web.UI.Page.Request.IsSecureConnection)
            SSLHandler.RedirectToSecuredPage();
    }
}
