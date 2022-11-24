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

/// ''' User Control Template for the Shortened View of the Products.

/// ''' </summary>

/// ''' <remarks>By Mohammad</remarks>
partial class ProductTemplateShortened : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
        string strNavigateURL = SiteMapHelper.CreateURL(SiteMapHelper.Page.Product, litProductID.Text, System.Web.UI.UserControl.Request.QueryString["strParent"], System.Web.UI.UserControl.Request.QueryString["CategoryID"]);
        bool blnCallForPrice = objObjectConfigBLL.GetValue("K:product.callforprice", litProductID.Text) == 1;

        lnkProductName.NavigateUrl = strNavigateURL;

        UC_ImageView.CreateImageViewer(IMAGE_TYPE.enum_ProductImage, litProductID.Text, KartSettingsManager.GetKartConfig("frontend.display.images.thumb.height"), KartSettingsManager.GetKartConfig("frontend.display.images.thumb.width"), strNavigateURL, null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, litProductName.Text);

        // Determine what to show for 'from' price
        switch (GetKartConfig("frontend.products.display.fromprice").ToLower)
        {
            case "y" // From $X.XX
           :
                {
                    if (blnCallForPrice)
                    {
                        litPriceFrom.Visible = true;
                        litPriceFrom.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("Versions", "ContentText_CallForPrice");
                        litPriceView.Visible = false;
                        divPrice.Visible = true;
                    }
                    else
                    {
                        litPriceView.Text = CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], System.Convert.ToDouble(litPriceHidden.Text));
                        litPriceFrom.Visible = true;
                        litPriceView.Visible = true;
                    }

                    break;
                }

            case "p" // $X.XX
     :
                {
                    if (blnCallForPrice)
                    {
                        litPriceFrom.Visible = true;
                        litPriceFrom.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("Versions", "ContentText_CallForPrice");
                        litPriceView.Visible = false;
                        divPrice.Visible = true;
                    }
                    else
                    {
                        litPriceView.Text = CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], System.Convert.ToDouble(litPriceHidden.Text));
                        litPriceFrom.Visible = false;
                        litPriceView.Visible = true;
                    }

                    break;
                }

            default:
                {
                    litPriceFrom.Visible = false;
                    litPriceView.Visible = false;
                    break;
                }
        }
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
