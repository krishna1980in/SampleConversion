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
using System.Net;
using CkartrisDataManipulation;
using CkartrisEnumerations;

partial class _CategoryView : System.Web.UI.UserControl
{
    private bool c_ShowPages = true;
    protected static bool sortByValueBool = false;
    const string c_PROD_PAGER_QUERY_STRING_KEY = "_PPGR";
    const string c_CAT_PAGER_QUERY_STRING_KEY = "_CPGR";

    public event ShowMasterUpdateEventHandler ShowMasterUpdate;

    public delegate void ShowMasterUpdateEventHandler();

    public bool ShowHeader
    {
        set
        {
            phdHeader.Visible = value;
        }
    }


    protected void Page_Load(object sender, System.EventArgs e)
    {
        lnkPreview.NavigateUrl = "~/Category.aspx?CategoryID=" + _GetCategoryID();
        CategoriesBLL objCategoriesBLL = new CategoriesBLL();

        if (!System.Web.UI.Control.Page.IsPostBack)
        {
            if (_GetSiteID() != 0)
            {
                phdCategoryHeader.Visible = false;
                phdEditCategory.Visible = false;
                lnkPreview.Visible = false;
                phdEditSubsite.Visible = true;
                phdSubsiteHeader.Visible = true;
                lnkPreviewSubsite.Visible = true;
                DataTable subsiteDT = SubSitesBLL.GetSubSiteByID(_GetSiteID);
                if (subsiteDT.Rows.Count == 0)
                    RedirectToMainCategory();
                else
                    litSubsiteName.Text = subsiteDT.Rows.Item(0).Item("SUB_Name") + " Categories";


                if (_GetCategoryID() != 0)
                {
                    if (objCategoriesBLL._GetByID(_GetCategoryID()).Rows.Count == 0)
                        RedirectToMainCategory();
                    LoadProducts(); // ' Only if its not the main category will show the products
                }
            }
            else if (_GetCategoryID() != 0)
            {
                if (objCategoriesBLL._GetByID(_GetCategoryID()).Rows.Count == 0)
                    RedirectToMainCategory();
                litCatName.Text = objCategoriesBLL._GetNameByCategoryID(_GetCategoryID(), System.Web.UI.UserControl.Session["_LANG"]);
                LoadProducts(); // ' Only if its not the main category will show the products
            }
            else
            {
                phdBreadCrumbTrail.Visible = false;
                phdEditCategory.Visible = true;
                phdProducts.Visible = false; // ' if categoryID = 0 then no products under it directly
            }
            LoadSubCategories();
        }

        if (_GetCategoryID() == 0)
        {
            _UC_LangContainer.CreateLanguageStrings(LANG_ELEM_TABLE_TYPE.Categories, false, _GetCategoryID());
            lnkBtnModifyPage.Visible = false;
        }
    }

    protected void btnUpdatePreference_Click(object sender, System.EventArgs e)
    {
        UpdatePreference();
    }


    protected void UpdatePreference()
    {
        if (System.Web.UI.UserControl.Request.Form["CAT_ID"] != currentPreference.Value)
        {
            int[] preferenceIds = (from p in System.Web.UI.UserControl.Request.Form["CAT_ID"].Split(",")
                                   select int.Parse(p)).ToArray();
            int preference = 1;
            foreach (int categoryId in preferenceIds)
            {
                this.UpdatePreference(categoryId, preference, _GetCategoryID());
                preference += 1;
            }
        }
    }

    private void UpdatePreference(int id, int preference, int parent)
    {
        string _connectionstring = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ConnectionString;
        using (SqlConnection con = new SqlConnection(_connectionstring))
        {
            using (SqlCommand cmd = new SqlCommand("UPDATE tblKartrisCategoryHierarchy SET CH_OrderNo = @Preference WHERE CH_ChildId = @Id AND CH_ParentID = @Parent"))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Preference", preference);
                    cmd.Parameters.AddWithValue("@Parent", parent);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
    }

    protected void btnUpdatePreferenceProducts_Click(object sender, System.EventArgs e)
    {
        UpdatePreferenceProducts();
    }


    protected void UpdatePreferenceProducts()
    {
        if (System.Web.UI.UserControl.Request.Form["P_ID"] != currentPreferenceProducts.Value)
        {
            int[] preferenceIds = (from p in System.Web.UI.UserControl.Request.Form["P_ID"].Split(",")
                                   select int.Parse(p)).ToArray();
            int preference = 1;
            foreach (int productId in preferenceIds)
            {
                this.UpdatePreferenceProducts(productId, preference, _GetCategoryID());
                preference += 1;
            }
        }
    }

    private void UpdatePreferenceProducts(int id, int preference, int categoryId)
    {
        string _connectionstring = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ConnectionString;
        using (SqlConnection con = new SqlConnection(_connectionstring))
        {
            using (SqlCommand cmd = new SqlCommand("UPDATE tblKartrisProductCategoryLink SET PCAT_OrderNo = @Preference WHERE PCAT_ProductID = @Id AND PCAT_CategoryID = @CategoryId"))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Preference", preference);
                    cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
    }

    // Load subcategories
    public void LoadSubCategories()
    {
        int numCategoryPageSize = 1000;
        if (KartSettingsManager.GetKartConfig("backend.categories.paging.enabled") == "y")
            numCategoryPageSize = KartSettingsManager.GetKartConfig("backend.categories.display.pagesize");
        int numTotalNumberOfCategories = 0;

        short numPageIndx;
        try
        {
            if (System.Web.UI.UserControl.Request.QueryString[c_CAT_PAGER_QUERY_STRING_KEY] == null)
                numPageIndx = 0;
            else
                numPageIndx = System.Convert.ToInt16(System.Web.UI.UserControl.Request.QueryString[c_CAT_PAGER_QUERY_STRING_KEY]);
        }
        catch (Exception ex)
        {
            numPageIndx = 0;
        }


        DataTable tblCategories = new DataTable();
        CategoriesBLL objCategoriesBLL = new CategoriesBLL();

        if (c_ShowPages)
        {
            tblCategories = objCategoriesBLL._GetCategoriesPageByParentID(_GetCategoryID(), System.Web.UI.UserControl.Session["_LANG"], numPageIndx, numCategoryPageSize, numTotalNumberOfCategories);
            if (tblCategories.Rows.Count != 0)
            {
                if (numTotalNumberOfCategories > numCategoryPageSize)
                {
                    _UC_ItemPager_CAT_Header.LoadPager(numTotalNumberOfCategories, numCategoryPageSize, c_CAT_PAGER_QUERY_STRING_KEY);
                    _UC_ItemPager_CAT_Header.DisableLink(numPageIndx);
                    _UC_ItemPager_CAT_Header.Visible = true;
                }
                dtlSubCategories.DataSource = tblCategories;
                dtlSubCategories.DataBind();
                ShowHideUpDownArrowsSubCategories(numTotalNumberOfCategories);
            }
            else
            {
                dtlSubCategories.Visible = false;
                phdNoSubCategories.Visible = true;
            }
        }
        else
        {
            tblCategories = objCategoriesBLL._GetCategoriesPageByParentID(_GetCategoryID(), System.Web.UI.UserControl.Session["_LANG"], 0, 500, 500);
            if (tblCategories.Rows.Count != 0)
            {
                dtlSubCategories.DataSource = tblCategories;
                dtlSubCategories.DataBind();
            }
            else
            {
                dtlSubCategories.Visible = false;
                phdNoSubCategories.Visible = true;
            }
        }
        var currentPreferencesItems = tblCategories.AsEnumerable().Select(row => new { catId = row.Field<Int32>("CAT_ID") });
        StringBuilder sb = new StringBuilder();
        string currentPreferences = string.Join(",", currentPreferencesItems.ToList()).ToString().Replace("{", "").Replace("}", "").Replace("catId = ", "").Replace(" ", "").Replace("vbCrLf", "");
        currentPreference.Value = currentPreferences;
    }

    // Whether to show the up/down subcat buttons
    private void ShowHideUpDownArrowsSubCategories(int TotalRows)
    {
        try
        {
            (LinkButton)dtlSubCategories.Items(0).FindControl("lnkBtnMoveUp").Enabled = false;
            (LinkButton)dtlSubCategories.Items(TotalRows - 1).FindControl("lnkBtnMoveDown").Enabled = false;
            (LinkButton)dtlSubCategories.Items(0).FindControl("lnkBtnMoveUp").CssClass += " triggerswitch_disabled";
            (LinkButton)dtlSubCategories.Items(TotalRows - 1).FindControl("lnkBtnMoveDown").CssClass += " triggerswitch_disabled";
        }
        catch (Exception ex)
        {
        }
    }

    // Load products
    public void LoadProducts()
    {
        int numProductPageSize = 1000;
        if (KartSettingsManager.GetKartConfig("backend.products.paging.enabled") == "y")
            numProductPageSize = KartSettingsManager.GetKartConfig("backend.products.display.pagesize");
        int numTotalNumberOfProducts = 0;

        short numPageIndx;
        try
        {
            if (System.Web.UI.UserControl.Request.QueryString[c_PROD_PAGER_QUERY_STRING_KEY] == null)
                numPageIndx = 0;
            else
                numPageIndx = System.Web.UI.UserControl.Request.QueryString[c_PROD_PAGER_QUERY_STRING_KEY];
        }
        catch (Exception ex)
        {
            numPageIndx = 0;
        }

        DataTable tblProducts = new DataTable();
        ProductsBLL objProductsBLL = new ProductsBLL();
        if (c_ShowPages)
        {
            tblProducts = objProductsBLL._GetProductsPageByCategory(_GetCategoryID(), System.Web.UI.UserControl.Session["_LANG"], numPageIndx, numProductPageSize, numTotalNumberOfProducts);
            if (tblProducts.Rows.Count != 0)
            {
                if (numTotalNumberOfProducts > numProductPageSize)
                {
                    _UC_ItemPager_PROD_Header.LoadPager(numTotalNumberOfProducts, numProductPageSize, c_PROD_PAGER_QUERY_STRING_KEY);
                    _UC_ItemPager_PROD_Header.DisableLink(numPageIndx);
                    _UC_ItemPager_PROD_Header.Visible = true;
                }
                phdNoProducts.Visible = false;
                lnkTurnProductsOn.Visible = true;
                lnkTurnProductsOff.Visible = true;
            }
            else
            {
                dtlProducts.Visible = false;
                _UC_ItemPager_PROD_Header.Visible = false;
            }
        }
        else
        {
            tblProducts = objProductsBLL._GetProductsPageByCategory(_GetCategoryID(), System.Web.UI.UserControl.Session["_LANG"], 0, 1000, 1000);
            if (tblProducts.Rows.Count != 0)
                phdNoProducts.Visible = false;
        }


        if (tblProducts.Rows.Count > 0)
        {
            var sortByValue = tblProducts.Rows(0)("SortByValue");
            if (!string.IsNullOrEmpty(sortByValue))
                sortByValueBool = Convert.ToBoolean(sortByValue);
            if (sortByValueBool)
            {
                var currentPreferencesItems = tblProducts.AsEnumerable().Select(row => new { pId = row.Field<Int32>("P_ID") });
                StringBuilder sb = new StringBuilder();
                string currentPreferences = string.Join(",", currentPreferencesItems.ToList()).ToString().Replace("{", "").Replace("}", "").Replace("pId = ", "").Replace(" ", "").Replace("vbCrLf", "");
                currentPreferenceProducts.Value = currentPreferences;
            }
        }
        dtlProducts.DataSource = tblProducts;
        dtlProducts.DataBind();
        ShowHideUpDownArrowsProducts(numTotalNumberOfProducts);
    }

    // Load products in expanded mode
    public void LoadProductsExpanded()
    {
        int numProductPageSize = 1000;
        if (KartSettingsManager.GetKartConfig("backend.products.paging.enabled") == "y")
            numProductPageSize = KartSettingsManager.GetKartConfig("backend.products.display.pagesize");
        int numTotalNumberOfProducts = 0;

        short numPageIndx;
        try
        {
            if (System.Web.UI.UserControl.Request.QueryString[c_PROD_PAGER_QUERY_STRING_KEY] == null)
                numPageIndx = 0;
            else
                numPageIndx = System.Web.UI.UserControl.Request.QueryString[c_PROD_PAGER_QUERY_STRING_KEY];
        }
        catch (Exception ex)
        {
            numPageIndx = 0;
        }

        ProductsBLL objProductsBLL = new ProductsBLL();
        DataTable tblProducts = new DataTable();
        if (c_ShowPages)
        {
            tblProducts = objProductsBLL._GetProductsPageByCategory(_GetCategoryID(), System.Web.UI.UserControl.Session["_LANG"], numPageIndx, numProductPageSize, numTotalNumberOfProducts);
            if (tblProducts.Rows.Count != 0)
            {
                if (numTotalNumberOfProducts > numProductPageSize)
                {
                    _UC_ItemPager_PROD_Header.LoadPager(numTotalNumberOfProducts, numProductPageSize, c_PROD_PAGER_QUERY_STRING_KEY);
                    _UC_ItemPager_PROD_Header.DisableLink(numPageIndx);
                    _UC_ItemPager_PROD_Header.Visible = true;
                }
                phdNoProducts2.Visible = false;
            }
            else
            {
                dtlProductsExpanded.Visible = false;
                _UC_ItemPager_PROD_Header.Visible = false;
            }
        }
        else
        {
            tblProducts = objProductsBLL._GetProductsPageByCategory(_GetCategoryID(), System.Web.UI.UserControl.Session["_LANG"], 0, 1000, 1000);
            if (tblProducts.Rows.Count != 0)
                phdNoProducts2.Visible = false;
        }

        dtlProductsExpanded.DataSource = tblProducts;
        dtlProductsExpanded.DataBind();
        ShowHideUpDownArrowsProducts(numTotalNumberOfProducts);
    }

    // Each item bound to datalist in expanded view
    protected void dtlProductsExpanded_ItemDataBound(object sender, System.Web.UI.WebControls.DataListItemEventArgs e)
    {
        char chrProductType = "";
        try
        {
            chrProductType = System.Convert.ToChar((Literal)e.Item.FindControl("litProductType").Text);
        }
        catch (Exception ex)
        {
            return;
        }

        int numProductID = (Literal)e.Item.FindControl("litProductID").Text;
        DataTable tblVersions = new DataTable();
        VersionsBLL objVersionsBLL = new VersionsBLL();
        tblVersions = objVersionsBLL._GetByProduct(numProductID, System.Web.UI.UserControl.Session["_LANG"]);
        DataColumn dtcShowClone = new DataColumn("ShowClone", Type.GetType("System.Boolean"));
        dtcShowClone.DefaultValue = true;
        tblVersions.Columns.Add(dtcShowClone);

        if (chrProductType == "o")
            (PlaceHolder)e.Item.FindControl("phdOptionsLink").Visible = true;
        if (chrProductType != "m" && tblVersions.Rows.Count > 0)
        {
            (PlaceHolder)e.Item.FindControl("phdNewVersionLink").Visible = false;
            foreach (DataRow row in tblVersions.Rows)
                row("ShowClone") = false;
        }

        (Repeater)e.Item.FindControl("rptVersions").DataSource = tblVersions;
        (Repeater)e.Item.FindControl("rptVersions").DataBind();

        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.SelectedItem)
        {
            if (chrProductType == "o" || chrProductType == "b")
                (PlaceHolder)e.Item.FindControl("phdOptionsLink").Visible = true;
        }
    }

    // Whether to show the up/down
    private void ShowHideUpDownArrowsProducts(int TotalRows)
    {
        try
        {
            (LinkButton)dtlProducts.Items(0).FindControl("lnkBtnMoveUp").Enabled = false;
            (LinkButton)dtlProducts.Items(TotalRows - 1).FindControl("lnkBtnMoveDown").Enabled = false;
            (LinkButton)dtlProducts.Items(0).FindControl("lnkBtnMoveUp").CssClass += " triggerswitch_disabled";
            (LinkButton)dtlProducts.Items(TotalRows - 1).FindControl("lnkBtnMoveDown").CssClass += " triggerswitch_disabled";
        }
        catch (Exception ex)
        {
        }
    }

    // Clicks on product level elements within the datalist
    protected void dtlProducts_ItemCommand(object source, System.Web.UI.WebControls.DataListCommandEventArgs e)
    {
        string strTab = "";
        ProductsBLL objProductsBLL = new ProductsBLL();
        switch (e.CommandName)
        {
            case "select":
                {
                    dtlProducts.SelectedIndex = e.Item.ItemIndex;
                    LoadProducts();
                    break;
                }

            case "AddVersion":
                {
                    if ((Literal)dtlProducts.SelectedItem.FindControl("litProductType").Text == "o")
                        strTab = "options";
                    else
                        strTab = "versions";
                    System.Web.UI.UserControl.Response.Redirect("~/Admin/_ModifyProduct.aspx?ProductID=" + e.CommandArgument + "&strParent=" + _GetCategoryID() + "&strTab=" + strTab);
                    break;
                }

            case "MoveUp":
                {
                    // ' Will use try to avoid error in case of null values or 0 values
                    try
                    {
                        objProductsBLL._ChangeSortValue(_GetCategoryID(), e.CommandArgument, "u");
                        LoadProducts();
                    }
                    catch (Exception ex)
                    {
                    }

                    break;
                }

            case "MoveDown":
                {
                    // ' Will use try to avoid error in case of null values or 0 values
                    try
                    {
                        objProductsBLL._ChangeSortValue(_GetCategoryID(), e.CommandArgument, "d");
                        LoadProducts();
                    }
                    catch (Exception ex)
                    {
                    }

                    break;
                }

            case "Refresh":
                {
                    try
                    {
                        UpdatePreferenceProducts();
                        LoadProducts();
                    }
                    catch (Exception ex)
                    {
                    }

                    break;
                }
        }
    }

    // Format nav links
    public string FormatNavURL(string ParentCategoryID, long CategoryID, int SiteID)
    {
        string strURL = IIf(string.IsNullOrEmpty(_GetParentCategory), _GetCategoryID(), _GetParentCategory() + "," + _GetCategoryID());
        if (strURL == "0")
            return "~/Admin/_Category.aspx?CategoryID=" + System.Web.UI.TemplateControl.Eval("CAT_ID") + "&SiteID=" + SiteID;
        else
            return "~/Admin/_Category.aspx?CategoryID=" + System.Web.UI.TemplateControl.Eval("CAT_ID") + "&SiteID=" + SiteID + "&strParent=" + strURL;
    }

    // Each product being bound to datalist
    protected void dtlProducts_ItemDataBound(object sender, System.Web.UI.WebControls.DataListItemEventArgs e)
    {
        VersionsBLL objVersionsBLL = new VersionsBLL();
        char chrProductType = "";
        try
        {
            chrProductType = System.Convert.ToChar((Literal)e.Item.FindControl("litProductType").Text);
        }
        catch (Exception ex)
        {
            return;
        }
        if (e.Item.ItemIndex == dtlProducts.SelectedIndex)
        {
            int numProductID = (Literal)e.Item.FindControl("litProductID").Text;
            DataTable tblVersions = new DataTable();
            tblVersions = objVersionsBLL._GetByProduct(numProductID, System.Web.UI.UserControl.Session["_LANG"]);
            DataColumn dtcShowClone = new DataColumn("ShowClone", Type.GetType("System.Boolean"));
            dtcShowClone.DefaultValue = true;
            tblVersions.Columns.Add(dtcShowClone);

            if (chrProductType == "o")
                (PlaceHolder)e.Item.FindControl("phdOptionsLink").Visible = true;
            if (chrProductType != "m" && tblVersions.Rows.Count > 0)
            {
                (PlaceHolder)e.Item.FindControl("phdNewVersionLink").Visible = false;
                foreach (DataRow row in tblVersions.Rows)
                    row("ShowClone") = false;
            }

            (Repeater)e.Item.FindControl("rptVersions").DataSource = tblVersions;
            (Repeater)e.Item.FindControl("rptVersions").DataBind();
        }
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.SelectedItem)
        {
            if (chrProductType == "o" || chrProductType == "b")
                (PlaceHolder)e.Item.FindControl("phdOptionsLink").Visible = true;
        }
    }

    protected void dtlSubCategories_ItemCommand(object source, System.Web.UI.WebControls.DataListCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "select":
                {
                    int numCategoryID = (Literal)e.Item.FindControl("litCategoryID").Text;
                    string strParent;
                    strParent = _GetParentCategory();
                    if (string.IsNullOrEmpty(strParent))
                        strParent = _GetCategoryID();
                    else
                        strParent += "," + _GetCategoryID();
                    if (strParent == "0")
                        System.Web.UI.UserControl.Response.Redirect("~/Admin/_Category.aspx?CategoryID=" + numCategoryID);
                    else
                        System.Web.UI.UserControl.Response.Redirect("~/Admin/_Category.aspx?CategoryID=" + numCategoryID + "&strParent=" + strParent);
                    break;
                }

            case "MoveUp":
                {
                    // ' Will use try to avoid error in case of null values or 0 values
                    try
                    {
                        CategoriesBLL objCategoriesBLL = new CategoriesBLL();
                        objCategoriesBLL._ChangeSortValue(_GetCategoryID(), e.CommandArgument, "u");
                        LoadSubCategories();
                        RefreshSiteMap();
                        updSubCategories.Update();
                    }
                    catch (Exception ex)
                    {
                    }

                    break;
                }

            case "MoveDown":
                {
                    // ' Will use try to avoid error in case of null values or 0 values
                    try
                    {
                        CategoriesBLL objCategoriesBLL = new CategoriesBLL();
                        objCategoriesBLL._ChangeSortValue(_GetCategoryID(), e.CommandArgument, "d");
                        LoadSubCategories();
                        RefreshSiteMap();
                        updSubCategories.Update();
                    }
                    catch (Exception ex)
                    {
                    }

                    break;
                }

            case "Refresh":
                {
                    try
                    {
                        UpdatePreference();
                        LoadSubCategories();
                        RefreshSiteMap();
                        updSubCategories.Update();
                    }
                    catch (Exception ex)
                    {
                    }

                    break;
                }
        }
    }

    protected void lnkPreviewSubsite_Click(object sender, System.EventArgs e)
    {
        if (_GetCategoryID() != 0 & _GetSiteID() != 0)
        {
            CookieContainer cookies = new CookieContainer();
            string portNumber = "";
            var portNumberArray = CkartrisBLL.WebShopURL.Split(":");
            if (portNumberArray.Count > 1)
                portNumber = portNumberArray(2);

            DataTable subsiteDT = SubSitesBLL.GetSubSiteByID(_GetSiteID);
            string stringUrl = "~/Category.aspx?CategoryID=" + _GetCategoryID() + "&SiteID=" + _GetSiteID();

            System.Web.UI.UserControl.Application["subsiteId"] = _GetSiteID();

            System.Web.UI.UserControl.Response.Redirect(stringUrl);
        }
    }

    protected void lnkBtnModifyPage_Click(object sender, System.EventArgs e)
    {
        if (_GetCategoryID() == 0)
        {
            _UC_LangContainer.CreateLanguageStrings(LANG_ELEM_TABLE_TYPE.Categories, false, _GetCategoryID());
            mvwCategory.SetActiveView(viwCategoryDetails);
            updCategoryViews.Update();
        }
        else if (System.Convert.ToString(_GetParentCategory()) == "")
            System.Web.UI.UserControl.Response.Redirect("_ModifyCategory.aspx?CategoryID=" + _GetCategoryID() + "&strParent=0");
        else
            System.Web.UI.UserControl.Response.Redirect("_ModifyCategory.aspx?CategoryID=" + _GetCategoryID() + "&strParent=" + _GetParentCategory());
    }

    protected void lnkBtnModifySubsite_Click(object sender, System.EventArgs e)
    {
        if (_GetSiteID() == 0)
        {
            _UC_LangContainer.CreateLanguageStrings(LANG_ELEM_TABLE_TYPE.Categories, false, _GetCategoryID());
            mvwCategory.SetActiveView(viwCategoryDetails);
            updCategoryViews.Update();
        }
        else
            System.Web.UI.UserControl.Response.Redirect("_ModifySubSiteStatus.aspx?SubSiteID=" + _GetSiteID() + "");
    }

    protected void lnkBtnNewCategory_Click(object sender, System.EventArgs e)
    {
        System.Web.UI.UserControl.Response.Redirect("_ModifyCategory.aspx?CategoryID=0&strParent=" + _GetCategoryID() + "&Sub=" + _GetParentCategory());
    }

    public void RedirectToMainCategory()
    {
        System.Web.UI.UserControl.Response.Redirect("~/Admin/_Category.aspx?CategoryID=0");
    }

    protected void lnkBtnSave_Click(object sender, System.EventArgs e)
    {
        DataTable tblLanguageContents = new DataTable();
        tblLanguageContents = _UC_LangContainer.ReadContent();
        string strMessage = "";
        CategoriesBLL objCategoriesBLL = new CategoriesBLL();
        if (!objCategoriesBLL._UpdateCategory(tblLanguageContents, "", 0, null/* TODO Change to default(_) if this is not a reference type */, null/* TODO Change to default(_) if this is not a reference type */, null/* TODO Change to default(_) if this is not a reference type */, null/* TODO Change to default(_) if this is not a reference type */, null/* TODO Change to default(_) if this is not a reference type */, null/* TODO Change to default(_) if this is not a reference type */, null/* TODO Change to default(_) if this is not a reference type */, null/* TODO Change to default(_) if this is not a reference type */, strMessage))
        {
            _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
            return;
        }
        RefreshSiteMap();
        ShowMasterUpdate?.Invoke();
        ShowHierarchy();
    }

    protected void lnkBtnCancel_Click(object sender, System.EventArgs e)
    {
        ShowHierarchy();
    }

    public void ShowHierarchy()
    {
        mvwCategory.SetActiveView(viwCategoryHierarchy);
        updCategoryViews.Update();
    }

    // Turn all products in a category ON (live=true)
    protected void lnkTurnProductsOn_Click(object sender, System.EventArgs e)
    {
        ProductsBLL objProductsBLL = new ProductsBLL();
        if (objProductsBLL._HideShowAllByCategoryID(_GetCategoryID(), true))
        {
            ShowMasterUpdate?.Invoke();
            LoadProducts();
        }
    }

    // Turn all products in a category OFF (live=false)
    protected void lnkTurnProductsOff_Click(object sender, System.EventArgs e)
    {
        ProductsBLL objProductsBLL = new ProductsBLL();
        if (objProductsBLL._HideShowAllByCategoryID(_GetCategoryID(), false))
        {
            ShowMasterUpdate?.Invoke();
            LoadProducts();
        }
    }

    // Collapse products (switch to normal view)
    protected void lnkCollapseProducts_Click(object sender, System.EventArgs e)
    {
        phdProductsExpanded.Visible = false;
        phdProducts.Visible = true;
        LoadProducts();
    }

    // Expand products (switch to expanded view)
    protected void lnkExpandProducts_Click(object sender, System.EventArgs e)
    {
        phdProductsExpanded.Visible = true;
        phdProducts.Visible = false;
        LoadProductsExpanded();
    }
}
