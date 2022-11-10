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

    partial class Category : PageBaseClass
    {
        public Category()
        {
            this.Load += Page_Load;
            this.LoadComplete += Page_LoadComplete;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // We set a value to keep track of any trapped
            // error handled, this way, we can avoid throwing
            // a generic error on top of the handled one.
            string strErrorThrown = "";

            if (!Page.IsPostBack)
            {
                string strCurrentPath = Request.RawUrl.ToString.ToLower;
                if (!(Strings.InStr(strCurrentPath, "/skins/") > 0 | Strings.InStr(strCurrentPath, "/javascript/") > 0 | Strings.InStr(strCurrentPath, "/images/") > 0))
                {
                    try
                    {
                        string strActiveTab = Request.QueryString("T");
                        int intCategoryID = Request.QueryString("CategoryID");

                        short numLangID = 1; // default to default language
                        try
                        {
                            numLangID = Session("LANG");
                        }
                        catch (Exception ex)
                        {
                            // numLangID = Request.QueryString("L")
                        }

                        UC_CategoryView.LoadCategory(intCategoryID, Session("LANG"));
                        if (UC_CategoryView.IsCategoryExist || intCategoryID == 0)
                        {
                            var objCategoriesBLL = new CategoriesBLL();
                            this.CanonicalTag = CkartrisBLL.WebShopURL + Strings.Mid(SiteMapHelper.CreateURL(SiteMapHelper.Page.CanonicalCategory, intCategoryID), 2);
                            this.MetaDescription = objCategoriesBLL.GetMetaDescriptionByCategoryID(intCategoryID, numLangID);
                            this.MetaKeywords = objCategoriesBLL.GetMetaKeywordsByCategoryID(intCategoryID, numLangID);

                            UC_SubCategoryView.LoadSubCategories(intCategoryID, Session("LANG"), UC_CategoryView.SubCategoryDisplayType);
                            UC_CategoryProductsView.LoadCategoryProducts(intCategoryID, Session("LANG"), UC_CategoryView.ProductsDisplayType, UC_CategoryView.ProductsDisplayOrder, UC_CategoryView.ProductsSortDirection);

                            if (UC_SubCategoryView.TotalItems > 0)
                            {
                                litSubCatHeader.Text += " <span class=\"total\">(" + UC_SubCategoryView.TotalItems + ")</span>";
                                if (strActiveTab == "S")
                                    tabContainer.ActiveTabIndex = 0;
                            }
                            else
                            {
                                tabSubCats.Enabled = false;
                                tabSubCats.Visible = false;
                            }
                            if (UC_CategoryProductsView.TotalItems > 0)
                            {
                                litProductsHeader.Text += " <span class=\"total\">(" + UC_CategoryProductsView.TotalItems + ")</span>";
                                if (strActiveTab != "S")
                                    tabContainer.ActiveTabIndex = 1;
                            }
                            else if (Request.QueryString("f") != "1")
                            {
                                tabProducts.Enabled = false;
                                tabProducts.Visible = false;
                            }

                            // If we are filtering in powerpack, always use the products tab, even if zero products/results
                            if (Request.QueryString("f") == 1)
                            {
                                tabContainer.ActiveTabIndex = 1;
                            }

                            if (!Page.IsPostBack && KartSettingsManager.GetKartConfig("general.products.hittracking") == "y")
                            {
                                StatisticsBLL.AddNewStatsRecord("C", GetIntegerQS("CategoryID"), GetIntegerQS("strParent"));
                            }
                        }
                        // If intCategoryID = 0 Then UC_BreadCrumbTrail.SiteMapProvider = "BreadCrumbSitemap"
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
                            catch (Exception exError)
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