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

partial class _OrderInvoice : Page
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Invoice", "PageTitle_Invoice") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        string[] numOrderIDQS, numCustomerIDQS;

        DataTable dtbInvoices = new DataTable();
        dtbInvoices.Columns.Add("OrderID", typeof(int));
        dtbInvoices.Columns.Add("CustomerID", typeof(int));

        Authenticate();

        numOrderIDQS = Request.QueryString("OrderID").Split("-");
        numCustomerIDQS = Request.QueryString("CustomerID").Split("-");

        for (int x = 0; x <= numOrderIDQS.Length - 1; x++)
            dtbInvoices.Rows.Add(Convert.ToInt32(numOrderIDQS[x]), Convert.ToInt32(numCustomerIDQS[x]));

        rptCompleteInvoice.DataSource = dtbInvoices;
        rptCompleteInvoice.DataBind();
    }

    /// <summary>
    ///     ''' This is only here to handle this page's authentication as its not handled by _PageBaseClass.
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void Authenticate()
    {
        HttpCookie cokKartris = Request.Cookies(HttpSecureCookie.GetCookieName("BackAuth"));
        string[] arrAuth = null;
        Session("Back_Auth") = "";
        if (cokKartris != null)
        {
            arrAuth = HttpSecureCookie.DecryptValue(cokKartris.Value, "Order Invoice");
            if (Information.UBound(arrAuth) == 8)
            {
                string strClientIP = CkartrisEnvironment.GetClientIPAddress();
                if (!string.IsNullOrEmpty(arrAuth[0]) & strClientIP == arrAuth[7])
                {
                    Session("Back_Auth") = cokKartris.Value;
                    Session("_LANG") = arrAuth[4];
                    Session("_USER") = arrAuth[0];
                }
                else
                {
                    Session("Back_Auth") = "";
                    cokKartris = new HttpCookie(HttpSecureCookie.GetCookieName("BackAuth"));
                    cokKartris.Expires = CkartrisDisplayFunctions.NowOffset.AddDays(-1M);
                    Response.Cookies.Add(cokKartris);
                }
            }
        }
        Response.Cache.SetCacheability(HttpCacheability.NoCache);


        string strScriptURL = Request.RawUrl.Substring(Request.Path.LastIndexOf("/") + 1);

        if (string.IsNullOrEmpty(Session("Back_Auth")))
        {
            if (Strings.Left(strScriptURL, 11) != "Default.aspx")
                Response.Redirect("~/Admin/Default.aspx?page=" + strScriptURL);
        }
        else
        {
            string strScriptName = Request.Path.Substring(Request.Path.LastIndexOf("/") + 1);
            SiteMapNode nodeCurrent = SiteMap.Providers("_KartrisSiteMap").FindSiteMapNodeFromKey("~/Admin/" + strScriptName);
            if (!nodeCurrent == null)
            {
                string strNodeValue = nodeCurrent.Item("Value");
                if (Information.UBound(arrAuth) == 8)
                {
                    switch (strNodeValue)
                    {
                        case "orders":
                            {
                                bool blnOrders = System.Convert.ToBoolean(arrAuth[3]);
                                // Session("Back_Orders")
                                if (!blnOrders)
                                {
                                    Response.Write("You are not authorized to view this page");
                                    Response.End();
                                }

                                break;
                            }

                        case "products":
                            {
                                bool blnProducts = System.Convert.ToBoolean(arrAuth[2]);
                                // Session("Back_Products")
                                if (!blnProducts)
                                {
                                    Response.Write("You are not authorized to view this page");
                                    Response.End();
                                }

                                break;
                            }

                        case "config":
                            {
                                bool blnConfig = System.Convert.ToBoolean(arrAuth[1]);
                                // Session("Back_Config")
                                if (!blnConfig)
                                {
                                    Response.Write("You are not authorized to view this page");
                                    Response.End();
                                }

                                break;
                            }

                        case "support":
                            {
                                bool blnSupport = System.Convert.ToBoolean(arrAuth[8]);
                                // Session("Back_Support")
                                if (!blnSupport)
                                {
                                    Response.Write("You are not authorized to view this page");
                                    Response.End();
                                }

                                break;
                            }
                    }
                }
                else
                {
                    Response.Write("Invalid Cookie");
                    Response.End();
                }
            }
            else
            {
                Response.Write("Unknown Backend Page. This needs to be added to the sitemap. If you don't want to show the link to the navigation menu. set its 'visible' tag to 'false' in the sitemap entry. <br/> e.g." + Server.HtmlEncode("<siteMapNode title=\"default\" url=\"~/Admin/_Default.aspx\" visible=\"false\" />"));
                Response.End();
            }
        }
    }

    protected void rptCompleteInvoice_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        int numOrderID, numCustomerID;

        numOrderID = e.Item.DataItem("OrderID");
        numCustomerID = e.Item.DataItem("CustomerID");
        UserControls_General_Invoice UC_Invoice = e.Item.FindControl("UC_Invoice");

        UC_Invoice.OrderID = numOrderID;
        UC_Invoice.CustomerID = numCustomerID;
        UC_Invoice.FrontOrBack = "back"; // tell user control is on back end
    }

    protected void Page_Error(object sender, System.EventArgs e)
    {
        LogError();
    }
}
