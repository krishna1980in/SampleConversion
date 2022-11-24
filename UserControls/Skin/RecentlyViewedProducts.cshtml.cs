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
using CkartrisDataManipulation;
using KartSettingsManager;

partial class UserControls_Skin_RecentlyViewedProducts : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (System.Convert.ToInt32(GetKartConfig("frontend.display.recentproducts")) == 0)
            this.Visible = false;

        if (!System.Web.UI.Control.Page.IsPostBack)
        {
            try
            {
                DataTable tblRecentlyViewedProducts = new DataTable();
                tblRecentlyViewedProducts.Columns.Add(new DataColumn("P_ID", Type.GetType("System.Int32")));
                tblRecentlyViewedProducts.Columns.Add(new DataColumn("P_Name", Type.GetType("System.String")));
                tblRecentlyViewedProducts.Columns.Add(new DataColumn("VersionID", Type.GetType("System.String")));

                string[] arrRecentProducts = System.Convert.ToString(System.Web.UI.UserControl.Session["RecentProducts"]).Split("||||");
                int numMaxProductsToDisplay = 0;
                if (IsNumeric(GetKartConfig("frontend.display.recentproducts")))
                    numMaxProductsToDisplay = System.Convert.ToInt32(GetKartConfig("frontend.display.recentproducts"));
                if (numMaxProductsToDisplay == 0)
                    numMaxProductsToDisplay = 100;
                int[] arrAddedProducts = new int[numMaxProductsToDisplay - 1 + 1];
                int numCounter = 0;
                for (var i = arrRecentProducts.Length - 1; i >= 0; i += -1)
                {
                    if (!string.IsNullOrEmpty(arrRecentProducts[i]) && arrRecentProducts[i].Contains("~~~~"))
                    {
                        string[] arrProduct = arrRecentProducts[i].Split("~~~~");
                        if (arrAddedProducts.Contains(arrProduct[0]) || arrProduct[0] == GetIntegerQS("P_ID"))
                            continue;

                        if (Information.IsNumeric(arrProduct[0]) && arrProduct[0] > 0)
                        {
                            tblRecentlyViewedProducts.Rows.Add(arrProduct[0], arrProduct[4], 0);
                            arrAddedProducts[numCounter] = arrProduct[0];
                            numCounter += 1;
                        }
                        if (tblRecentlyViewedProducts.Rows.Count == numMaxProductsToDisplay)
                            break;
                    }
                }

                if (tblRecentlyViewedProducts.Rows.Count == 0)
                {
                    this.Visible = false;
                    return;
                }
                rptRecentViewedProducts.DataSource = tblRecentlyViewedProducts.DefaultView;
                rptRecentViewedProducts.DataBind();
            }
            catch (Exception ex)
            {
                // This could happen if for example a product is deleted
                // by the store owner while is on your recent products
                // session. Just clear session in this case, it's not
                // so important that we need to keep it.
                System.Web.UI.UserControl.Session["RecentProducts"] = string.Empty;
            }
        }
    }

    protected void rptRecentViewedProducts_ItemDataBound(object Sender, RepeaterItemEventArgs e)
    {
        if ((e.Item.ItemType == ListItemType.Item) | (e.Item.ItemType == ListItemType.AlternatingItem))
        {
            try
            {
                (HyperLink)e.Item.FindControl("lnkRecentlyViewed").NavigateUrl = SiteMapHelper.CreateURL(SiteMapHelper.Page.CanonicalProduct, (DataRowView)e.Item.DataItem("P_ID"));
            }
            catch (Exception ex)
            {
            }
        }
    }
}
