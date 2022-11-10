using System;
using System.Web;
using Microsoft.VisualBasic.CompilerServices;

namespace EmployeeManagementSystem
{
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
    partial class News : PageBaseClass
    {

        // We set a value to keep track of any trapped
        // error handled, this way, we can avoid throwing
        // a generic error on top of the handled one.
        private string strErrorThrown = "";

        public News()
        {
            this.Load += Page_Load;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (KartSettingsManager.GetKartConfig("frontend.navigationmenu.sitenews") == "y")
            {
                Page.Title = GetGlobalResourceObject("News", "PageTitle_SiteNews") + " | " + Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));
                if (!Page.IsPostBack)
                {
                    LoadNews();
                }
            }
            else
            {
                // An item was called with correctly formatted URL, but
                // the ID doesn't appear to pull out an item, so it's
                // likely the item is no longer available.
                strErrorThrown = "404";
                try
                {
                    HttpContext.Current.Server.Execute("~/404.aspx");
                }
                catch (Exception exError)
                {

                }
            }
        }

        private void LoadNews()
        {
            try
            {
                int numID = (int)Request.QueryString("NewsID");
                rptSiteNews.DataSource = NewsBLL.GetByID(Session("LANG"), numID);
                rptSiteNews.DataBind();

                if (numID == 0 & UC_SiteNews.TitleTagType == "h2")
                {
                    UC_SiteNews.TitleTagType = "h1";
                }

                if (numID != 0 & rptSiteNews.Items.Count == 0)
                {
                    strErrorThrown = "404";
                    try
                    {
                        HttpContext.Current.Server.Execute("~/404.aspx");
                    }
                    catch (Exception exError)
                    {

                    }
                }
                else
                {
                    // Set pagetitle for specific news story
                    foreach (var itmRow in rptSiteNews.Items)
                        Page.Title = Conversions.ToString(Operators.ConcatenateObject(itmRow.FindControl("N_Name").Text, " | ")) + Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));
                }
            }
            catch (Exception ex)
            {
                // Some other error occurred - it seems the ID of the item
                // exists, but loading or displaying the item caused some
                // other error.
                CkartrisFormatErrors.ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
                if (string.IsNullOrEmpty(strErrorThrown))
                    HttpContext.Current.Server.Execute("~/Error.aspx");
            }

        }
    }
}