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

using System.Web.Security;
using System.Web.HttpContext;
using KartSettingsManager;
using System.Threading;
using System.Xml;
using CkartrisFormatErrors;
using CkartrisDisplayFunctions;

[assembly: Obfuscation(Feature = "code control flow obfuscation", Exclude = false)]


/// <summary> 

/// ''' Provides cookie cyphering services. 

/// ''' </summary>
public sealed class HttpSecureCookie
{
    private HttpSecureCookie()
    {
    }

    /// <summary>
    ///     ''' Creates a SHA256 hash
    ///     ''' </summary>
    public static string CreateHash(DataRow drwLogin, string strUserName, string strPassword, string strClientIP)
    {
        UsersBLL objUsersBLL = new UsersBLL();
        string strUserData = strUserName + "##" + drwLogin("LOGIN_Config").ToString + "##" + drwLogin("LOGIN_Products").ToString + "##" + drwLogin("LOGIN_Orders").ToString + "##" + drwLogin("LOGIN_LanguageID").ToString + "##" + NowOffset() + "##" + objUsersBLL.EncryptSHA256Managed(strPassword, LoginsBLL._GetSaltByUserName(strUserName), true) + "##" + strClientIP + "##" + drwLogin("LOGIN_Tickets").ToString;
        FormsAuthenticationTicket objTicket = new FormsAuthenticationTicket(1, strUserName, NowOffset, NowOffset.AddDays(1), true, strUserData, FormsAuthentication.FormsCookiePath);
        return FormsAuthentication.Encrypt(objTicket);
    }

    /// <summary>
    ///     ''' Decrypts cookie
    ///     ''' </summary>
    public static string[] Decrypt(string strCookieValue = "")
    {
        string[] arrAuth = null;
        string strValue;
        if (!string.IsNullOrEmpty(strCookieValue))
            strValue = strCookieValue;
        else
        {
            HttpCookie cokKartris = System.Web.HttpContext.Current.Request.Cookies[GetCookieName("BackAuth")];
            if (cokKartris != null)
                strValue = cokKartris.Value;
            else
                return null;
        }
        if (!string.IsNullOrEmpty(strValue))
        {
            try
            {
                FormsAuthenticationTicket returnValue = FormsAuthentication.Decrypt(strValue);
                arrAuth = Strings.Split(returnValue.UserData, "##");
                return arrAuth;
            }
            catch (Exception ex)
            {
                // Invalid Cookie Data
                ForceLogout(false);
            }
        }
        return null;
    }

    /// <summary>
    ///     ''' Decrypts cookie value
    ///     ''' </summary>
    public static string[] DecryptValue(string strValue, string strScript)
    {
        string[] arrAuth = null;
        if (!string.IsNullOrEmpty(strValue))
        {
            try
            {
                FormsAuthenticationTicket returnValue = FormsAuthentication.Decrypt(strValue);
                arrAuth = Strings.Split(returnValue.UserData, "##");
                return arrAuth;
            }
            catch (Exception ex)
            {
                // Invalid Cookie Data
                LogError("Cannot Decrypt in " + strScript);
                arrAuth.SetValue("", 0);
            }
        }
        return arrAuth;
    }

    /// <summary>
    ///     ''' Check if user is authenticated for back end (admin) access
    ///     ''' </summary>
    public static bool IsBackendAuthenticated()
    {
        string[] arrAuth = null;
        arrAuth = Decrypt();
        if (arrAuth != null)
        {
            if (Information.UBound(arrAuth) == 8)
            {
                string strClientIP = CkartrisEnvironment.GetClientIPAddress();

                if (!string.IsNullOrEmpty(arrAuth[0]) & strClientIP == arrAuth[7])
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    ///     ''' Log the user out
    ///     ''' </summary>
    public static void ForceLogout(bool blnRedirect = true)
    {
        HttpCookie cokKartris;
        cokKartris = new HttpCookie(HttpSecureCookie.GetCookieName("BackAuth"), "");
        cokKartris.Expires = NowOffset();
        System.Web.HttpContext.Current.Response.Cookies.Add(cokKartris);
        System.Web.HttpContext.Current.Session["Back_Auth"] = "";
        System.Web.HttpContext.Current.Session["_LANG"] = LanguagesBLL.GetDefaultLanguageID();
        if (blnRedirect)
            System.Web.HttpContext.Current.Response.Redirect("~/Admin/");
    }

    /// <summary>
    ///     ''' Find out the name of the cookie used
    ///     ''' </summary>
    public static string GetCookieName(string strPostfix = "")
    {
        return Trim(GetKartConfig("general.sessions.cookiename")) + strPostfix + Left(ConfigurationManager.AppSettings("HashSalt"), 5);
    }
}

/// <summary>

/// ''' Is SSL required?

/// ''' Checks config setting

/// ''' </summary>
public sealed class SSLHandler
{
    public static bool IsSSLEnabled()
    {
        bool blnIsSSLEnabled = false;

        // SSL enabled in config settings
        if (GetKartConfig("general.security.ssl") == "y" | GetKartConfig("general.security.ssl") == "a" | GetKartConfig("general.security.ssl") == "e")
            blnIsSSLEnabled = true;

        return blnIsSSLEnabled;
    }

    /// <summary>
    ///     ''' Force SSL on back end if available
    ///     ''' </summary>
    public static void CheckBackSSL()
    {
        if (IsSSLEnabled())
        {
            // Start with assumption no SSL required
            bool blnNeedSSL = false;

            // If admin logged in, then we set the requirement
            // for SSL to 'true'
            if (IsBackEndLoggedIn())
                blnNeedSSL = true;

            // Handle external SSL such as Cloudflare.com, this means we cannot detect it
            // from the website itself, as the SSL is applied by a proxy externally
            if (GetKartConfig("general.security.ssl") == "e")
            {
            }
            else
                // We need SSL, but current page doesn't have it
                if (blnNeedSSL == true & !System.Web.HttpContext.Current.Request.IsSecureConnection())
                System.Web.HttpContext.Current.Response.Redirect(System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace("http://", "https://"));
        }
        else if (System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Contains("https://"))
            System.Web.HttpContext.Current.Response.Redirect(System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace("https://", "http://"));
    }

    /// <summary>
    ///     ''' Force SSL on front end if available
    ///     ''' </summary>
    public static void CheckFrontSSL(bool blnAuthenticatedUser)
    {
        if (IsSSLEnabled())
        {
            // Start with assumption no SSL required
            bool blnNeedSSL = false;
            bool blnRedirectPermanent = false;

            // If admin logged in, front end user logged in,
            // or we're on a login page, then we set the requirement
            // for SSL to 'true'
            if (IsBackEndLoggedIn())
                blnNeedSSL = true;
            if (blnAuthenticatedUser)
                blnNeedSSL = true;
            if (System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToLower().Contains("customeraccount.aspx"))
                blnNeedSSL = true;
            if (System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToLower().Contains("checkout.aspx"))
                blnNeedSSL = true;
            if (System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToLower().Contains("customertickets.aspx"))
                blnNeedSSL = true;

            // This handles SSL always on
            if (GetKartConfig("general.security.ssl") == "a")
            {
                blnNeedSSL = true;
                blnRedirectPermanent = true;
            }

            // Added v2.6000 - don't redirect on callback.aspx
            // We get problems in some payment systems, because we cannot necessarily
            // control consistently for all whether SSL is used or not. Therefore, we
            // should accept calls to the callback with either http or https, and avoid
            // redirection.
            if (!System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToLower().Contains("callback"))
            {
                // We need SSL, but current page doesn't have it

                // Handle external SSL. In this case, something like Cloudflare.com is used
                // to add SSL to the site by relaying the site via it's own servers and applying
                // SSL
                if (GetKartConfig("general.security.ssl") == "e")
                {
                }
                else
                    // convential SSL code
                    if (blnNeedSSL == true & !System.Web.HttpContext.Current.Request.IsSecureConnection())
                {
                    if (blnRedirectPermanent)
                        System.Web.HttpContext.Current.Response.RedirectPermanent(System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace("http://", "https://"));
                    else
                        System.Web.HttpContext.Current.Response.Redirect(System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace("http://", "https://"));
                }
                else if (blnNeedSSL == false & System.Web.HttpContext.Current.Request.IsSecureConnection())
                    System.Web.HttpContext.Current.Response.Redirect(System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace("https://", "http://"));
            }
        }
        else if (System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Contains("https://"))
            System.Web.HttpContext.Current.Response.Redirect(System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace("https://", "http://"));
    }


    /// <summary>
    ///     ''' Force SSL redirect
    ///     ''' </summary>
    public static void RedirectToSecuredPage()
    {
        if (IsSSLEnabled() & !GetKartConfig("general.security.ssl") == "e")
            System.Web.HttpContext.Current.Response.Redirect(System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace("http://", "https://"));
    }

    /// <summary>
    ///     ''' Not sure what this is for, can anyone update this comment?
    ///     ''' </summary>
    public static bool IsSecuredForSEO()
    {
        if (IsSSLEnabled())
        {
            try
            {
                if (((!string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["Back_Auth"])) | HttpContext.Current.User.Identity.IsAuthenticated))
                    return true;
            }
            catch (Exception ex)
            {
                if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    ///     ''' Checks if back end (admin) user is logged in
    ///     ''' </summary>
    public static bool IsBackEndLoggedIn()
    {
        return !string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["Back_Auth"]);
    }
}
