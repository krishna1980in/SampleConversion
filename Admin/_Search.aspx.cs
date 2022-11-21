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
using KartSettingsManager;

partial class Admin_Search : _PageBaseClass
{
    private int numPageSize = GetKartConfig("backend.search.pagesize");

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Search", "PageTitle_SearchResults") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        if (!Request.QueryString("key") == null && !Request.QueryString("location") == null)
            _Search(Request.QueryString("location"), Request.QueryString("key"));
    }

    public void _Search(string strSearchBy, string strKey)
    {
        int numTotalResult;
        switch (strSearchBy)
        {
            case "categories":
                {
                    gvwCategories.DataSource = KartrisDBBLL._SearchBackEnd(strSearchBy, strKey, Session("_LANG"), 0, numPageSize, numTotalResult);
                    gvwCategories.DataBind();
                    litCategories.Text = numTotalResult;
                    if (numTotalResult > 0)
                        pnlCategories.Visible = true;
                    if (gvwCategories.PageCount < 2)
                        gvwCategories.AllowPaging = false;
                    break;
                }

            case "products":
                {
                    gvwProducts.DataSource = KartrisDBBLL._SearchBackEnd(strSearchBy, strKey, Session("_LANG"), 0, numPageSize, numTotalResult);
                    gvwProducts.DataBind();
                    litProducts.Text = numTotalResult;
                    if (numTotalResult > 0)
                        pnlProducts.Visible = true;
                    if (gvwProducts.PageCount < 2)
                        gvwProducts.AllowPaging = false;
                    break;
                }

            case "versions":
                {
                    gvwVersions.DataSource = KartrisDBBLL._SearchBackEnd(strSearchBy, strKey, Session("_LANG"), 0, numPageSize, numTotalResult);
                    gvwVersions.DataBind();
                    litVersions.Text = numTotalResult;
                    if (numTotalResult > 0)
                        pnlVersions.Visible = true;
                    if (gvwVersions.PageCount < 2)
                        gvwVersions.AllowPaging = false;
                    break;
                }

            case "customers":
                {
                    gvwCustomers.DataSource = KartrisDBBLL._SearchBackEnd(strSearchBy, strKey, Session("_LANG"), 0, numPageSize, numTotalResult);
                    gvwCustomers.DataBind();
                    litCustomers.Text = numTotalResult;
                    if (numTotalResult > 0)
                        pnlCustomers.Visible = true;
                    if (gvwCustomers.PageCount < 2)
                        gvwCustomers.AllowPaging = false;
                    break;
                }

            case "orders":
                {
                    gvwOrders.DataSource = KartrisDBBLL._SearchBackEnd(strSearchBy, strKey, Session("_LANG"), 0, numPageSize, numTotalResult);
                    gvwOrders.DataBind();
                    litOrders.Text = numTotalResult;
                    pnlOrders.Visible = true;
                    if (gvwOrders.PageCount < 2)
                        gvwOrders.AllowPaging = false;
                    break;
                }

            case "site" // language strings
     :
                {
                    gvwSite.DataSource = KartrisDBBLL._SearchBackEnd(strSearchBy, strKey, Session("_LANG"), 0, numPageSize, numTotalResult);
                    gvwSite.DataBind();
                    litSiteText.Text = numTotalResult;
                    if (numTotalResult > 0)
                        pnlSiteText.Visible = true;
                    if (gvwSite.PageCount < 2)
                        gvwSite.AllowPaging = false;
                    break;
                }

            case "config":
                {
                    gvwConfig.DataSource = KartrisDBBLL._SearchBackEnd(strSearchBy, strKey, Session("_LANG"), 0, numPageSize, numTotalResult);
                    gvwConfig.DataBind();
                    litConfig.Text = numTotalResult;
                    if (numTotalResult > 0)
                        pnlConfig.Visible = true;
                    if (gvwConfig.PageCount < 2)
                        gvwConfig.AllowPaging = false;
                    break;
                }

            case "pages":
                {
                    gvwPages.DataSource = KartrisDBBLL._SearchBackEnd(strSearchBy, strKey, Session("_LANG"), 0, numPageSize, numTotalResult);
                    gvwPages.DataBind();
                    litPages.Text = numTotalResult;
                    if (numTotalResult > 0)
                        pnlPages.Visible = true;
                    if (gvwPages.PageCount < 2)
                        gvwPages.AllowPaging = false;
                    break;
                }

            default:
                {
                    SearchAllDB(strKey);
                    return;
                }
        }

        // ' No results found
        if (numTotalResult == 0)
            pnlNoResults.Visible = true;
        updResults.Update();
    }

    protected void SearchAllDB(string strKey)
    {
        int numTotalResult = 0;
        bool blnResultExist = false;

        gvwCategories.DataSource = KartrisDBBLL._SearchBackEnd("categories", strKey, Session("_LANG"), 0, numPageSize, numTotalResult);
        gvwCategories.DataBind();
        litCategories.Text = numTotalResult;
        if (numTotalResult > 0)
        {
            pnlCategories.Visible = true; blnResultExist = true;
        }
        if (gvwCategories.PageCount < 2)
            gvwCategories.AllowPaging = false;

        numTotalResult = 0;
        gvwProducts.DataSource = KartrisDBBLL._SearchBackEnd("products", strKey, Session("_LANG"), 0, numPageSize, numTotalResult);
        gvwProducts.DataBind();
        litProducts.Text = numTotalResult;
        if (numTotalResult > 0)
        {
            pnlProducts.Visible = true; blnResultExist = true;
        }
        if (gvwProducts.PageCount < 2)
            gvwProducts.AllowPaging = false;

        gvwVersions.DataSource = KartrisDBBLL._SearchBackEnd("versions", strKey, Session("_LANG"), 0, numPageSize, numTotalResult);
        gvwVersions.DataBind();
        litVersions.Text = numTotalResult;
        if (numTotalResult > 0)
        {
            pnlVersions.Visible = true; blnResultExist = true;
        }
        if (gvwVersions.PageCount < 2)
            gvwVersions.AllowPaging = false;

        gvwCustomers.DataSource = KartrisDBBLL._SearchBackEnd("customers", strKey, Session("_LANG"), 0, numPageSize, numTotalResult);
        gvwCustomers.DataBind();
        litCustomers.Text = numTotalResult;
        if (numTotalResult > 0)
        {
            pnlCustomers.Visible = true; blnResultExist = true;
        }
        if (gvwCustomers.PageCount < 2)
            gvwCustomers.AllowPaging = false;

        gvwOrders.DataSource = KartrisDBBLL._SearchBackEnd("orders", strKey, Session("_LANG"), 0, numPageSize, numTotalResult);
        gvwOrders.DataBind();
        litOrders.Text = numTotalResult;
        if (numTotalResult > 0)
        {
            pnlOrders.Visible = true; blnResultExist = true;
        }
        if (gvwOrders.PageCount < 2)
            gvwOrders.AllowPaging = false;

        gvwConfig.DataSource = KartrisDBBLL._SearchBackEnd("config", strKey, Session("_LANG"), 0, numPageSize, numTotalResult);
        gvwConfig.DataBind();
        litConfig.Text = numTotalResult;
        if (numTotalResult > 0)
        {
            pnlConfig.Visible = true; blnResultExist = true;
        }
        if (gvwConfig.PageCount < 2)
            gvwConfig.AllowPaging = false;

        gvwSite.DataSource = KartrisDBBLL._SearchBackEnd("site", strKey, Session("_LANG"), 0, numPageSize, numTotalResult);
        gvwSite.DataBind();
        litSiteText.Text = numTotalResult;
        if (numTotalResult > 0)
        {
            pnlSiteText.Visible = true; blnResultExist = true;
        }
        if (gvwSite.PageCount < 2)
            gvwSite.AllowPaging = false;

        gvwPages.DataSource = KartrisDBBLL._SearchBackEnd("pages", strKey, Session("_LANG"), 0, numPageSize, numTotalResult);
        gvwPages.DataBind();
        litPages.Text = numTotalResult;
        if (numTotalResult > 0)
        {
            pnlPages.Visible = true; blnResultExist = true;
        }
        if (gvwPages.PageCount < 2)
            gvwPages.AllowPaging = false;

        if (!blnResultExist)
            pnlNoResults.Visible = true;

        updResults.Update();
    }

    protected void gridView_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        (GridView)sender.PageIndex = e.NewPageIndex;
        string strGridName = (GridView)sender.ID;
        _Search(strGridName.Replace("gvw", "").ToLower(), Request.QueryString("key"));
    }
}
