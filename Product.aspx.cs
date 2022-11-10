using System;
using System.Web;
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
using CkartrisDataManipulation;
using Microsoft.VisualBasic;

namespace EmployeeManagementSystem
{

    partial class Product : PageBaseClass
    {
        public Product()
        {
            this.Load += Page_Load;
            this.LoadComplete += Page_LoadComplete;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string strCurrentPath = Request.RawUrl.ToString.ToLower;

            // We set a value to keep track of any trapped
            // error handled, this way, we can avoid throwing
            // a generic error on top of the handled one.
            string strErrorThrown = "";

            if (!(Strings.InStr(strCurrentPath, "/skins/") > 0 | Strings.InStr(strCurrentPath, "/javascript/") > 0 | Strings.InStr(strCurrentPath, "/images/") > 0))
            {
                try
                {
                    int intProductID = Request.QueryString("ProductID");
                    var objProductsBLL = new ProductsBLL();

                    short numLangID = 1; // default to default language
                    try
                    {
                        numLangID = Session("LANG");
                    }
                    catch (Exception ex)
                    {
                        // numLangID = Request.QueryString("L")
                    }


                    try
                    {
                        UC_ProductView.LoadProduct(intProductID, numLangID);
                    }
                    catch (Exception ex)
                    {
                        // Response.End()
                    }

                    if (UC_ProductView.IsProductExist)
                    {

                        if (!Page.IsPostBack)
                        {
                            if (KartSettingsManager.GetKartConfig("general.products.hittracking") == "y")
                                StatisticsBLL.AddNewStatsRecord("P", GetIntegerQS("ProductID"), GetIntegerQS("strParent"));
                            Session("RecentProducts") = Session("RecentProducts") + GetIntegerQS("ProductID") + "~~~~" + Server.HtmlDecode(UC_ProductView.ProductName) + "||||";

                            // Above we use four hashes or pipes because these should not occur naturally in product names
                            if (intProductID > 0)
                            {
                                this.CanonicalTag = CkartrisBLL.WebShopURL + Strings.Mid(SiteMapHelper.CreateURL(SiteMapHelper.Page.CanonicalProduct, intProductID), 2);
                                this.MetaDescription = objProductsBLL.GetMetaDescriptionByProductID(intProductID, numLangID);
                                this.MetaKeywords = objProductsBLL.GetMetaKeywordsByProductID(intProductID, numLangID);
                            }
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
                            Server.ClearError();
                            HttpContext.Current.Server.Execute("~/404.aspx");
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                catch (Exception ex)
                {
                    // Some other error occurred - it seems the ID of the item
                    // exists, but loading or displaying the item caused some
                    // other error.
                    if (string.IsNullOrEmpty(strErrorThrown)) // 404s won't get logged
                    {
                        CkartrisFormatErrors.ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
                        HttpContext.Current.Server.Execute("~/Error.aspx");
                    }

                }
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (Session("NewBasketItem") == 1)
            {
                object MiniBasket = Master.FindControl("UC_MiniBasket");
                MiniBasket.LoadMiniBasket();
                Session("NewBasketItem") = 0;
            }
        }
    }
}