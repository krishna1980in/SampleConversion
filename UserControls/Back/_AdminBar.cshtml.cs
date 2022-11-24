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
using KartSettingsManager;

partial class UserControls_Back_AdminBar : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (GetKartConfig("general.storestatus") == "open")
            lnkFront.CssClass = "";
        else
            lnkFront.CssClass = "closed";

        // Set the front/back link to specific URL,
        // overrides the Default link
        SetFrontEndLink();
    }

    public void SetFrontEndLink()
    {
        // Get current URL
        string strResolvedURL = System.Web.UI.Control.Context.Request.RawUrl.ToLower();

        // Category
        long numCategoryID = 0;
        try
        {
            numCategoryID = System.Web.UI.UserControl.Request.QueryString["CategoryID"];
        }
        catch (Exception ex)
        {
        }
        if (numCategoryID > 0)
            lnkFront.NavigateUrl = "~/Category.aspx?CategoryID=" + numCategoryID.ToString();

        // Product
        long numProductID = 0;
        try
        {
            numProductID = System.Web.UI.UserControl.Request.QueryString["ProductID"];
        }
        catch (Exception ex)
        {
        }
        if (numProductID > 0)
            lnkFront.NavigateUrl = "~/Product.aspx?ProductID=" + numProductID.ToString();

        // Basket page
        if (strResolvedURL.ToLower().Contains("_createorder.aspx") | strResolvedURL.ToLower().Contains("_modifyorder.aspx"))
            lnkFront.NavigateUrl = "~/Basket.aspx";
    }
}
