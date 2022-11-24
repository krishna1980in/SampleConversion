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

partial class UserControls_Skin_LanguageDropdown : System.Web.UI.UserControl
{

    // Where to look for flag/language images
    private string strLanguageImages = "~/Images/Languages/";

    /// <summary>
    ///     ''' Page load
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (KartSettingsManager.GetKartConfig("frontend.languages.display") == "dropdown")
        {
            selLanguage.Visible = true;
            phdLanguageImages.Visible = false;
            phdLanguageTextLinks.Visible = false;
        }
        else if (KartSettingsManager.GetKartConfig("frontend.languages.display") == "image")
        {
            selLanguage.Visible = false;
            phdLanguageImages.Visible = true;
            phdLanguageTextLinks.Visible = false;
        }
        else
        {
            selLanguage.Visible = false;
            phdLanguageImages.Visible = false;
            phdLanguageTextLinks.Visible = true;
        }
        if (LanguagesBLL.GetLanguagesCount() == 1)
            this.Visible = false;
    }

    /// <summary>
    ///     ''' Language selected in the dropdown was changed
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void selLanguage_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        ChangeLanguage(selLanguage.SelectedValue.ToString);
    }

    /// <summary>
    ///     ''' The dropdown menu of languages was changed
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void selLanguage_Load(object sender, System.EventArgs e)
    {
        if (!System.Web.UI.Control.Page.IsPostBack & KartSettingsManager.GetKartConfig("frontend.languages.display") == "dropdown" & System.Web.UI.UserControl.Session["KartrisUserCulture"] != null)
        {
            if (!string.IsNullOrEmpty(System.Web.UI.UserControl.Session["KartrisUserCulture"]))
                selLanguage.SelectedValue = System.Web.UI.UserControl.Session["KartrisUserCulture"].ToString();
        }
    }

    /// <summary>
    ///     ''' Flag or text link was clicked
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void rptLanguages_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "ChangeLanguage")
            ChangeLanguage(e.CommandArgument);
    }

    /// <summary>
    ///     ''' Items bound for the flag image links
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void rptLanguages_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
        {
            string strImageName = (LinkButton)e.Item.FindControl("lnkImage").CommandArgument;
            DirectoryInfo dirLanguageImages = new DirectoryInfo(System.Web.UI.UserControl.Server.MapPath(strLanguageImages));
            foreach (FileInfo objFile in dirLanguageImages.GetFiles())
            {
                if (objFile.Name.StartsWith(strImageName + "."))
                {
                    (Image)e.Item.FindControl("imgLanguage").ImageUrl = strLanguageImages + objFile.Name + "?nocache=" + DateTime.Now.ToBinary();
                    break;
                }
            }
            if ((LinkButton)e.Item.FindControl("lnkImage").CommandArgument == System.Web.UI.UserControl.Session["KartrisUserCulture"].ToString())
                (LinkButton)e.Item.FindControl("lnkImage").Enabled = false;
        }
    }

    /// <summary>
    ///     ''' Items bound for text links
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void rptLanguages2_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
        {
            LinkButton lnkText = (LinkButton)e.Item.FindControl("lnkText");
            if (lnkText.CommandArgument == System.Web.UI.UserControl.Session["KartrisUserCulture"].ToString())
            {
                lnkText.Enabled = false;
                lnkText.CssClass += " lang-selected";
            }
        }
    }

    /// <summary>
    ///     ''' Show language as EN (short) or en-GB (long)
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public string LongShortLanguageText(string strCulture)
    {
        if (KartSettingsManager.GetKartConfig("frontend.languages.display") == "linkshort")
            return Strings.Left(strCulture, 2);
        else
            return strCulture;
    }

    /// <summary>
    ///     ''' Change the language
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void ChangeLanguage(string strCulture)
    {
        // if the session differs from the value in the ddl, the session-object is changed and the cookie is created
        CurrentQSCollection CQSC = new CurrentQSCollection();

        System.Web.UI.UserControl.Session["KartrisUserCulture"] = strCulture;
        byte numLangID = LanguagesBLL.GetLanguageIDByCulture_s(strCulture);
        System.Web.UI.UserControl.Session["LANG"] = numLangID;
        CQSC.Set("L", numLangID);
        HttpCookie aCookie = new HttpCookie(HttpSecureCookie.GetCookieName());
        aCookie.Values("KartrisUserCulture") = System.Web.UI.UserControl.Session["KartrisUserCulture"];
        aCookie.Expires = System.DateTime.Now.AddDays(21);
        System.Web.UI.UserControl.Response.Cookies.Add(aCookie);
        System.Web.UI.UserControl.Session["Skin_Location"] = LanguagesBLL.GetSkinURLByCulture(strCulture);

        // Reload the page
        System.Web.UI.UserControl.Response.Redirect(System.Web.UI.UserControl.Request.Url.GetLeftPart(UriPartial.Path) + CQSC.ToString());
    }

    /// <summary>
    ///     ''' This class allows us to get and modify any QueryString value from the URL. See usage above.
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public class CurrentQSCollection : NameValueCollection
    {
        public CurrentQSCollection()
        {
            foreach (string key in HttpContext.Current.Request.QueryString.AllKeys)
                Add(key, HttpContext.Current.Request.QueryString(key));
        }

        public new override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= Count - 1; i++)
            {
                string key = Keys(i);
                sb.Append((((i == 0) ? "?" : "&") + key + "=") + HttpContext.Current.Server.UrlEncode(this(key)));
            }
            return sb.ToString();
        }
    }
}
