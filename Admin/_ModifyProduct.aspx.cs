// ========================================================================
// Kartris - www.kartris.com
// Copyright 2021 CACTUSOFT

// Mods for multiple file upload - August 2014:
// Craig Moore
// Deadline Automation Limited
// www.deadline-automation.com

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
using CkartrisImages;

partial class Admin_ModifyProduct : _PageBaseClass
{
    // Dim sw As Stopwatch = New Stopwatch

    private bool _ReviewsLoaded = false;
    private bool _VersionsLoaded = false;
    private bool _AttributesLoaded = false;
    private bool _OptionsLoaded = false;
    private bool _RelatedProductsLoaded = false;
    private bool _ProductLoaded = false;
    private bool _ConfigLoaded = false;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Kartris", "BackMenu_Products") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");
        _UC_ObjectConfig.ItemID = _GetProductID();
        if (!Page.IsPostBack)
        {
            litProductID.Text = _GetProductID();

            if (_GetProductID() == 0)
                PrepareNewProduct();
            else
                PrepareExistingProduct();

            // Highlights the first tab when page first loads
            string strTab = Request.QueryString("strTab");
            if (strTab != null)
                strTab = strTab.ToLower();

            switch (strTab)
            {
                case "images":
                    {
                        if (_GetProductID() == 0)
                            break;
                        lnkImages_Click();
                        _UC_Uploader.ImageType = IMAGE_TYPE.enum_ProductImage;
                        _UC_Uploader.ItemID = _GetProductID();
                        _UC_Uploader.LoadImages();
                        break;
                    }

                case "media":
                    {
                        if (_GetProductID() == 0)
                            break;
                        lnkMedia_Click();
                        _UC_EditMedia.ParentType = "p";
                        _UC_EditMedia.ParentID = _GetProductID();
                        _UC_EditMedia.LoadMedia();
                        break;
                    }

                case "attributes":
                    {
                        if (_GetProductID() == 0)
                            break;
                        lnkAttributes_Click();
                        break;
                    }

                case "reviews":
                    {
                        if (_GetProductID() == 0)
                            break;
                        lnkReviews_Click();
                        break;
                    }

                case "relatedproducts":
                    {
                        if (_GetProductID() == 0)
                            break;
                        lnkRelatedProducts_Click();
                        break;
                    }

                case "versions":
                    {
                        if (_GetProductID() == 0)
                            break;
                        lnkProductVersions_Click();
                        break;
                    }

                case "options":
                    {
                        if (_GetProductID() == 0)
                            break;
                        lnkOptions_Click();
                        break;
                    }

                case "config":
                    {
                        if (_GetProductID() == 0)
                            break;
                        lnkConfig_Click();
                        break;
                    }

                default:
                    {
                        Session("_tab") = "products";

                        mvwEditProduct.ActiveViewIndex = 0;
                        HighLightTab();
                        break;
                    }
            }
        }
        else if (_GetProductID() != 0 && Request.QueryString("strTab") == "images")
        {
            _UC_Uploader.ImageType = IMAGE_TYPE.enum_ProductImage;
            _UC_Uploader.ItemID = _GetProductID();
            _UC_Uploader.LoadImages();
        }

        // Need this for all tabs in order to decide whether to show
        // options tab or not
        _UC_EditProduct.ReloadProduct();

        // Does this need an options link?
        CheckToShowOptionLink();

        // Prepare the hyperlinks on tabs
        PrepareTabHyperlinks();

        // Hide breadcrumbtrail if no hierarchy info
        CheckToHideBreadCrumbTrail();
    }

    public void CheckToHideBreadCrumbTrail()
    {
        if (Request.QueryString("strParent") == "")
            phdBreadCrumbTrail.Visible = false;
        if (Request.QueryString("CategoryID") == "0")
            phdBreadCrumbTrail.Visible = false;
    }

    public void PrepareTabHyperlinks()
    {
        long intP_ID = 0;
        try
        {
            intP_ID = litProductID.Text;
        }
        catch (Exception ex)
        {
        }
        long intCAT_ID = 0;
        try
        {
            intCAT_ID = Request.QueryString("CategoryID");
        }
        catch (Exception ex)
        {
        }
        int numSiteID = 0;
        try
        {
            numSiteID = Request.QueryString("SiteID");
        }
        catch (Exception ex)
        {
        }
        string strParent = Request.QueryString("strParent");

        lnkMainInfo.NavigateUrl = "_ModifyProduct.aspx?ProductID=" + intP_ID + "&SiteID=" + numSiteID + "&CategoryID=" + intCAT_ID + "&strParent=" + strParent;
        lnkImages.NavigateUrl = "_ModifyProduct.aspx?ProductID=" + intP_ID + "&SiteID=" + numSiteID + "&CategoryID=" + intCAT_ID + "&strParent=" + strParent + "&strTab=images";
        lnkMedia.NavigateUrl = "_ModifyProduct.aspx?ProductID=" + intP_ID + "&SiteID=" + numSiteID + "&CategoryID=" + intCAT_ID + "&strParent=" + strParent + "&strTab=media";
        lnkAttributes.NavigateUrl = "_ModifyProduct.aspx?ProductID=" + intP_ID + "&SiteID=" + numSiteID + "&CategoryID=" + intCAT_ID + "&strParent=" + strParent + "&strTab=attributes";
        lnkReviews.NavigateUrl = "_ModifyProduct.aspx?ProductID=" + intP_ID + "&SiteID=" + numSiteID + "&CategoryID=" + intCAT_ID + "&strParent=" + strParent + "&strTab=reviews";
        lnkRelatedProducts.NavigateUrl = "_ModifyProduct.aspx?ProductID=" + intP_ID + "&SiteID=" + numSiteID + "&CategoryID=" + intCAT_ID + "&strParent=" + strParent + "&strTab=relatedproducts";
        lnkProductVersions.NavigateUrl = "_ModifyProduct.aspx?ProductID=" + intP_ID + "&SiteID=" + numSiteID + "&CategoryID=" + intCAT_ID + "&strParent=" + strParent + "&strTab=versions";
        lnkOptions.NavigateUrl = "_ModifyProduct.aspx?ProductID=" + intP_ID + "&SiteID=" + numSiteID + "&CategoryID=" + intCAT_ID + "&strParent=" + strParent + "&strTab=options";
        lnkObjectConfig.NavigateUrl = "_ModifyProduct.aspx?ProductID=" + intP_ID + "&SiteID=" + numSiteID + "&CategoryID=" + intCAT_ID + "&strParent=" + strParent + "&strTab=config";
    }

    public void PrepareNewProduct()
    {
        litProductName.Text = GetGlobalResourceObject("_Product", "PageTitle_NewProduct");
        pnlTabStrip.Visible = false;
        _UC_ProductIndicator.Visible = false;

        Page.Title = GetGlobalResourceObject("_Product", "PageTitle_NewProduct") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");
    }

    public void PrepareExistingProduct()
    {
        ProductsBLL objProductsBLL = new ProductsBLL();
        litProductName.Text = objProductsBLL._GetNameByProductID(_GetProductID(), Session("_LANG"));
        pnlTabStrip.Visible = true;
        _UC_ProductIndicator.CheckProductStatus();

        Page.Title = litProductName.Text + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");
    }

    public void CheckToShowOptionLink()
    {
        char chrProductType = default(Char);
        ProductsBLL objProductsBLL = new ProductsBLL();
        chrProductType = objProductsBLL._GetProductType_s(_GetProductID());
        if (chrProductType != "o")
            lnkOptions.Visible = false;
        else
            lnkOptions.Visible = true;
    }

    protected void _UC_ProductOptionGroups_VersionChanged()
    {
        CheckProductOptions();
        ShowMasterUpdateMessage();
    }

    protected void _UC_ProductOptionGroups_AllOptionsDeleted()
    {
        RefreshProductInformation();
        ShowMasterUpdateMessage();
    }

    protected void _UC_EditProduct_ProductSaved()
    {
        ProductsBLL objProductsBLL = new ProductsBLL();
        litProductName.Text = objProductsBLL._GetNameByProductID(_GetProductID(), Session("_LANG"));
        _UC_EditProduct.LoadProductInfo();
        ShowMasterUpdateMessage();
    }

    protected void _UC_VersionView_VersionsChanged()
    {
        _UC_EditProduct.CheckProductType();
        ShowMasterUpdateMessage();
    }

    public void RefreshProductInformation()
    {
        PrepareExistingProduct();
        _UC_EditProduct.LoadProductInfo();
    }

    protected void _UC_EditProduct_CategoryNotExist()
    {
        RedirectToNewProduct();
    }

    public void RedirectToNewProduct()
    {
        Response.Redirect("~/Admin/_ModifyProduct.aspx?ProductID=0");
    }

    public void CheckProductOptions()
    {
        ProductsBLL objProductsBLL = new ProductsBLL();
        if (objProductsBLL._GetProductType_s(_GetProductID()) == "o")
        {
            _UC_ProductOptionGroups.CreateProductOptionGroups();
            VersionsBLL objVersionsBLL = new VersionsBLL();
            if (objVersionsBLL._GetNoOfVersionsByProductID(_GetProductID()) == 0)
            {
                if (mvwEditProduct.ActiveViewIndex == 5)
                    mvwEditProduct.ActiveViewIndex = 0;
            }
            lnkOptions.Visible = true;
            updMain.Update();
        }
        else
        {
            lnkOptions.Visible = false;
            if (!Page.IsPostBack)
            {
                if (Request.Url.AbsoluteUri.ToLower.Contains("&strtab=options"))
                    Response.Redirect(Request.Url.AbsoluteUri.ToLower.Replace("&strtab=options", ""));
                else if (Request.Url.AbsoluteUri.ToLower.Contains("strtab=options"))
                    Response.Redirect(Request.Url.AbsoluteUri.ToLower.Replace("strtab=options", ""));
            }
        }
    }

    public void HighLightTab()
    {
        lnkMainInfo.CssClass = "";
        lnkOptions.CssClass = "";
        lnkImages.CssClass = "";
        lnkMedia.CssClass = "";
        lnkAttributes.CssClass = "";
        lnkReviews.CssClass = "";
        lnkRelatedProducts.CssClass = "";
        lnkProductVersions.CssClass = "";
        lnkObjectConfig.CssClass = "";

        // rewritten the multiple 'if' statements to a single select case block
        switch (mvwEditProduct.ActiveViewIndex)
        {
            case 0:
                {
                    lnkMainInfo.CssClass = "active";
                    break;
                }

            case 1:
                {
                    lnkImages.CssClass = "active";
                    break;
                }

            case 2:
                {
                    lnkMedia.CssClass = "active";
                    break;
                }

            case 3:
                {
                    lnkAttributes.CssClass = "active";
                    break;
                }

            case 4:
                {
                    lnkReviews.CssClass = "active";
                    break;
                }

            case 5:
                {
                    lnkRelatedProducts.CssClass = "active";
                    break;
                }

            case 6:
                {
                    lnkProductVersions.CssClass = "active";
                    break;
                }

            case 7:
                {
                    lnkOptions.CssClass = "active";
                    break;
                }

            case 8:
                {
                    lnkObjectConfig.CssClass = "active";
                    break;
                }
        }
    }

    public void lnkMainInfo_Click()
    {
        Session("_tab") = "products";
        if (!_ProductLoaded)
        {
            _ProductLoaded = true; _UC_EditProduct.ReloadProduct();
        }
        mvwEditProduct.ActiveViewIndex = 0;
        HighLightTab();
    }

    public void lnkImages_Click()
    {
        Session("_tab") = "images";
        _UC_Uploader.ImageType = IMAGE_TYPE.enum_ProductImage;
        _UC_Uploader.ItemID = _GetProductID();
        _UC_Uploader.LoadImages();
        mvwEditProduct.ActiveViewIndex = 1;
        HighLightTab();
    }

    public void lnkMedia_Click()
    {
        Session("_tab") = "media";
        _UC_EditMedia.ParentType = "p";
        _UC_EditMedia.ParentID = _GetProductID();
        _UC_EditMedia.LoadMedia();
        mvwEditProduct.ActiveViewIndex = 2;
        HighLightTab();
    }

    public void lnkAttributes_Click()
    {
        Session("_tab") = "attributes";
        if (!_AttributesLoaded)
        {
            _AttributesLoaded = true; _UC_ProductAttributes.ShowProductAttributes();
        }
        mvwEditProduct.ActiveViewIndex = 3;
        HighLightTab();
    }

    public void lnkReviews_Click()
    {
        Session("_tab") = "reviews";
        if (!_ReviewsLoaded)
        {
            _ReviewsLoaded = true; _UC_ProductReview.LoadProductReviews();
        }
        mvwEditProduct.ActiveViewIndex = 4;
        HighLightTab();
    }

    public void lnkRelatedProducts_Click()
    {
        Session("_tab") = "relatedproducts";
        if (!_RelatedProductsLoaded)
        {
            _RelatedProductsLoaded = true; _UC_RelatedProducts.LoadRelatedProducts();
        }
        mvwEditProduct.ActiveViewIndex = 5;
        HighLightTab();
    }

    public void lnkProductVersions_Click()
    {
        Session("_tab") = "versions";
        if (!_VersionsLoaded)
        {
            _VersionsLoaded = true; _UC_VersionView.ShowProductVersions();
        }
        mvwEditProduct.ActiveViewIndex = 6;
        HighLightTab();
    }

    public void lnkOptions_Click()
    {
        Session("_tab") = "options";
        if (!_OptionsLoaded)
        {
            _OptionsLoaded = true; CheckProductOptions();
        }
        mvwEditProduct.ActiveViewIndex = 7;
        HighLightTab();
    }

    public void lnkConfig_Click()
    {
        Session("_tab") = "config";
        if (!_ConfigLoaded)
        {
            _ConfigLoaded = true; _UC_ObjectConfig.LoadObjectConfig();
        }
        mvwEditProduct.ActiveViewIndex = 8;
        HighLightTab();
    }

    protected void ctrl_NeedCategoryRefresh()
    {
        (Skins_Admin_Template)this.Master.LoadCategoryMenu();
    }

    protected void ShowMasterUpdateMessage()
    {
        (Skins_Admin_Template)this.Master.DataUpdated();
        _UC_ProductIndicator.CheckProductStatus();
        updMain.Update();
    }

    protected void ProductUpdated(string strNewProductName)
    {
        // ' It will redirect only if the name of the product is changed.
        if (litProductName.Text != strNewProductName)
            Response.Redirect(Request.Url.AbsoluteUri);
        _UC_EditProduct_ProductSaved();
    }
}
