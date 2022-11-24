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
using CkartrisImages;
using KartSettingsManager;

/// <summary>

/// ''' User Control Template for the Extended View of the Products.

/// ''' </summary>

/// ''' <remarks>By Mohammad</remarks>
partial class ProductTemplateExtended : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        string strNavigateURL = SiteMapHelper.CreateURL(SiteMapHelper.Page.Product, litProductID.Text, System.Web.UI.UserControl.Request.QueryString["strParent"], System.Web.UI.UserControl.Request.QueryString["CategoryID"]);

        lnkProductName.NavigateUrl = strNavigateURL;
        lnknMore.NavigateUrl = strNavigateURL;

        UC_ImageView.CreateImageViewer(IMAGE_TYPE.enum_ProductImage, litProductID.Text, KartSettingsManager.GetKartConfig("frontend.display.images.thumb.height"), KartSettingsManager.GetKartConfig("frontend.display.images.thumb.width"), strNavigateURL, null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, lnkProductName.Text);

        SetCompareURL();

        try
        {
            // ' Call to load the UC Product Versions for the Current Product.
            UC_ProductVersions.LoadProductVersions(litProductID.Text, System.Web.UI.UserControl.Session["LANG"], litVersionsViewType.Text);
        }
        catch (Exception ex)
        {
        }

        if (!UC_ProductVersions.HasPrice)
            phdMinPrice.Visible = true;
    }

    public void SetCompareURL()
    {
        // ' Setting the Compare URL ...
        string strCompareLink = System.Web.UI.UserControl.Request.Url.ToString().ToLower();
        if (System.Web.UI.UserControl.Request.Url.ToString().ToLower().Contains("category.aspx"))
            strCompareLink = strCompareLink.Replace("category.aspx", "Compare.aspx");
        else if (System.Web.UI.UserControl.Request.Url.ToString().ToLower().Contains("product.aspx"))
            strCompareLink = strCompareLink.Replace("product.aspx", "Compare.aspx");
        else
            strCompareLink = "~/Compare.aspx";
        if (strCompareLink.Contains("?"))
            strCompareLink += "&action=add&id=" + litProductID.Text;
        else
            strCompareLink += "?action=add&id=" + litProductID.Text;
    }

    // Show or hide minprice
    public bool ShowMinPrice(Int64 numP_ID)
    {
        ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
        if (objObjectConfigBLL.GetValue("K:product.callforprice", System.Web.UI.TemplateControl.Eval("P_ID")) == 1 || !string.IsNullOrEmpty(objObjectConfigBLL.GetValue("K:product.customcontrolname", System.Web.UI.TemplateControl.Eval("P_ID"))))
            return false;
        else
            return true;
    }
}
