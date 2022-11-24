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
using CkartrisImages;
using KartSettingsManager;

/// <summary>

/// ''' User Control Template for the Tabular View of the Products.

/// ''' </summary>

/// ''' <remarks>By Paul</remarks>
partial class Templates_ProductLinkTemplate : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        string strNavigateURL = SiteMapHelper.CreateURL(SiteMapHelper.Page.Product, litProductID.Text, System.Web.UI.UserControl.Request.QueryString["strParent"], System.Web.UI.UserControl.Request.QueryString["CategoryID"]);

        lnkProductName.NavigateUrl = strNavigateURL;

        UC_ImageView.CreateImageViewer(IMAGE_TYPE.enum_ProductImage, litProductID.Text, KartSettingsManager.GetKartConfig("frontend.display.images.thumb.height"), KartSettingsManager.GetKartConfig("frontend.display.images.thumb.width"), strNavigateURL, null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, lnkProductName.Text);
    }

    // Do this with function as can use try catch,
    // easier to trap errors if bad data import,
    // or apply other rules
    public string DisplayProductName()
    {
        try
        {
            return System.Web.UI.UserControl.Server.HtmlEncode(System.Web.UI.TemplateControl.Eval("P_Name"));
        }
        catch (Exception ex)
        {
            // do nowt
            return "";
        }
    }
}
