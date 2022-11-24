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

/// ''' User Control Template for the search results.

/// ''' </summary>

/// ''' <remarks>By Paul</remarks>
partial class Templates_ProductTemplateSearchResult : System.Web.UI.UserControl
{
    public void LoadSearchResult(DataTable ptblResult)
    {
        rptSearchResult.DataSource = ptblResult;
        rptSearchResult.DataBind();
    }

    protected void rptSearchResult_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {

        // What page of results did user click from?
        int intPageNumber = 0;
        try
        {
            intPageNumber = System.Web.UI.UserControl.Request.QueryString["PPGR"];
        }
        catch (Exception ex)
        {
            intPageNumber = 0;
        }

        ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
        bool blnCallForPrice = objObjectConfigBLL.GetValue("K:product.callforprice", (Literal)e.Item.FindControl("litProductID").Text) == 1;
        foreach (Control ctlElement in e.Item.Controls)
        {
            string strNavigateURL = "~/Product.aspx?ProductID=" + (Literal)e.Item.FindControl("litProductID").Text;
            strNavigateURL += "&strReferer=search";
            strNavigateURL += "&PPGR=" + intPageNumber;

            switch (ctlElement.ID)
            {
                case "lnkProductName":
                    {
                        (HyperLink)e.Item.FindControl("lnkProductName").NavigateUrl = strNavigateURL;
                        break;
                    }

                case "litPriceView":
                    {
                        if (!blnCallForPrice)
                        {
                            float numPrice = 0.0F;
                            numPrice = System.Convert.ToSingle((Literal)e.Item.FindControl("litPriceHidden").Text);
                            numPrice = CurrenciesBLL.ConvertCurrency(System.Web.UI.UserControl.Session["CUR_ID"], numPrice);
                            (Literal)e.Item.FindControl("litPriceView").Text = CurrenciesBLL.FormatCurrencyPrice(System.Web.UI.UserControl.Session["CUR_ID"], numPrice);
                        }

                        break;
                    }

                case "UC_ImageView":
                    {
                        // Format image
                        ImageViewer UC_ImageView = (ImageViewer)e.Item.FindControl("UC_ImageView");
                        UC_ImageView.CreateImageViewer(IMAGE_TYPE.enum_ProductImage, (Literal)e.Item.FindControl("litProductID").Text, KartSettingsManager.GetKartConfig("frontend.display.images.minithumb.height"), KartSettingsManager.GetKartConfig("frontend.display.images.minithumb.width"), strNavigateURL, "");
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        // Determine what to show for 'from' price
        switch (GetKartConfig("frontend.products.display.fromprice").ToLower)
        {
            case "y" // From $X.XX
           :
                {
                    if (blnCallForPrice)
                    {
                        (Literal)e.Item.FindControl("litPriceFrom").Visible = true;
                        (Literal)e.Item.FindControl("litPriceView").Visible = false;
                        (Literal)e.Item.FindControl("litPriceFrom").Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("Versions", "ContentText_CallForPrice");
                    }
                    else
                    {
                        (Literal)e.Item.FindControl("litPriceFrom").Visible = true;
                        (Literal)e.Item.FindControl("litPriceView").Visible = true;
                    }

                    break;
                }

            case "p" // $X.XX
     :
                {
                    if (blnCallForPrice)
                    {
                        (Literal)e.Item.FindControl("litPriceFrom").Visible = true;
                        (Literal)e.Item.FindControl("litPriceView").Visible = false;
                        (Literal)e.Item.FindControl("litPriceFrom").Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("Versions", "ContentText_CallForPrice");
                    }
                    else
                    {
                        (Literal)e.Item.FindControl("litPriceFrom").Visible = false;
                        (Literal)e.Item.FindControl("litPriceView").Visible = true;
                    }

                    break;
                }

            default:
                {
                    (Literal)e.Item.FindControl("litPriceFrom").Visible = false;
                    (Literal)e.Item.FindControl("litPriceView").Visible = false;
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
            return (System.Web.UI.TemplateControl.Eval("P_Name"));
        }
        catch (Exception ex)
        {
            // do nowt
            return "";
        }
    }
}
