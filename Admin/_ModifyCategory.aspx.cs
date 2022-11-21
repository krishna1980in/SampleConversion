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
using CkartrisDataManipulation;

partial class Admin_ModifyCategory : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Category", "BackMenu_Categories") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        litContentTextSubCatsProducts.Text = HttpUtility.HtmlEncode(GetGlobalResourceObject("_Kartris", "ContentText_SubCatsProducts"));

        litCategoryID.Text = _GetCategoryID();
        litSiteID.Text = _GetSiteID();

        // v3 mod
        // Object config now available for categories
        _UC_ObjectConfig.ItemID = _GetCategoryID();
        if (!IsPostBack)
            _UC_ObjectConfig.LoadObjectConfig();

        if (_GetCategoryID() == 0)
            PrepareNewCategory();
        else
            PrepareExistingCategory();

        tabXmlFilters.Visible = PowerpackBLL._IsFiltersEnabled();

        _UC_Uploader.OneItemOnly = true;
        _UC_Uploader.ImageType = IMAGE_TYPE.enum_CategoryImage;
        _UC_Uploader.ItemID = _GetCategoryID();
        _UC_Uploader.LoadImages();
    }

    public void PrepareNewCategory()
    {
        litCategoryName.Text = GetGlobalResourceObject("_Category", "PageTitle_NewCategory");
        foreach (AjaxControlToolkit.TabPanel t in tabContainerProduct.Tabs)
        {
            if (t.ID != "tabMainInfo")
            {
                t.Enabled = false; t.Visible = false;
            }
        }
        Page.Title = GetGlobalResourceObject("_Category", "PageTitle_NewCategory") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");
    }

    public void PrepareExistingCategory()
    {
        CategoriesBLL objCategoriesBLL = new CategoriesBLL();
        litCategoryName.Text = objCategoriesBLL._GetNameByCategoryID(_GetCategoryID(), Session("_LANG"));
        foreach (AjaxControlToolkit.TabPanel t in tabContainerProduct.Tabs)
        {
            t.Enabled = true; t.Visible = true;
        }
        _UC_CategoryIndicator.CheckCategoryStatus();
        Page.Title = litCategoryName.Text + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");
    }

    protected void _UC_EditCategory_CategoryNotExist()
    {
        RedirectToNewCategory();
    }

    protected void ShowMasterUpdateMessage()
    {
        (Skins_Admin_Template)this.Master.DataUpdated();
    }

    protected void CategoryUpdated(string strNewCategoryName)
    {
        // ' It will redirect only if the name of the category is changed.
        if (litCategoryName.Text != strNewCategoryName)
            Response.Redirect(Request.Url.AbsoluteUri);
        ShowMasterUpdateMessage();
        _UC_CategoryView.LoadSubCategories();
        _UC_CategoryView.LoadProducts();
        updRelatedData.Update();
        // Check the category status again, it might
        // now might be visible or invisible on front
        // end due to changes made.
        _UC_CategoryIndicator.CheckCategoryStatus();
    }

    public void RedirectToNewCategory()
    {
        Response.Redirect("~/Admin/_ModifyCategory.aspx?CategoryID=0&SiteID" == litSiteID.Text);
    }

    protected void Page_LoadComplete(object sender, System.EventArgs e)
    {
        if (Session("tab") == "images")
        {
            tabContainerProduct.ActiveTab = tabImages;
            Session("tab") = "";
        }
    }

    protected void _UC_Uploader_NeedCategoryRefresh()
    {
        (Skins_Admin_Template)this.Master.LoadCategoryMenu();
    }
}
