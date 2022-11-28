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

partial class _CategoryMenu : System.Web.UI.UserControl
{
    public event ShowMasterUpdateEventHandler ShowMasterUpdate;

    public delegate void ShowMasterUpdateEventHandler();

    /// <summary>
    ///     ''' Page load
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!System.Web.UI.Control.Page.IsPostBack)
            LoadCategoryMenu();
        else
        {
        }
    }

    /// <summary>
    ///     ''' Loads the category menu and selects the current page 
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void LoadCategoryMenu()
    {
        BuildTopLevelMenu();
        BuildDefaultLevels();
        SelectCurrentPage();
    }

    /// <summary>
    ///     ''' Refresh and clears caches when button clicked
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void btnRefresh_Click(object sender, System.EventArgs e)
    {

        // Refresh caches
        CkartrisBLL.RefreshKartrisCache();

        // Rebuild the treeview
        _CategorySiteMapProvider _CatSiteMap = (_CategorySiteMapProvider)SiteMap.Providers("_CategorySiteMapProvider");
        _CatSiteMap.ResetSiteMap();
        CategorySiteMapProvider CatSiteMap = (CategorySiteMapProvider)SiteMap.Provider;
        CatSiteMap.RefreshSiteMap();
        BuildTopLevelMenu();
        SelectCurrentPage();

        // v3.3000
        // Rebuild new product price index
        bool blnRebuilt = ProductsBLL._RebuildPriceIndex();

        ShowMasterUpdate?.Invoke();
        updMenu.Update();
    }

    /// <summary>
    ///     ''' Build top level cats of treeview
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' New sproc for Kartris v3, now includes subsites
    ///     ''' </remarks>
    public void BuildTopLevelMenu()
    {
        CategoriesBLL objCategoriesBLL = new CategoriesBLL();
        DataTable tblChilds = objCategoriesBLL._Treeview(System.Web.UI.UserControl.Session["_LANG"]);
        TreeNode childNode = null/* TODO Change to default(_) if this is not a reference type */;
        foreach (DataRow drwChilds in tblChilds.Rows)
        {
            childNode = new TreeNode(drwChilds("CAT_Name"), drwChilds("CAT_ID") + "|" + drwChilds("SUB_ID"));
            childNode.NavigateUrl = _CategorySiteMapProvider.CreateURL(_CategorySiteMapProvider.BackEndPage.Category, drwChilds("CAT_ID"), drwChilds("SUB_ID"));
            childNode.PopulateOnDemand = true;
            if ((drwChilds("SUB_ID")) > 0)
            {
                childNode.ImageUrl = "~/Skins/Admin/Images/site-tree.gif";
                childNode.ToolTip = drwChilds("SUB_Name");
            }
            else if (System.Convert.ToBoolean(drwChilds("CAT_Live")))
                childNode.ImageUrl = "~/Skins/Admin/Images/category-tree.gif";
            else
                childNode.ImageUrl = "~/Skins/Admin/Images/offline-category-tree.gif";
            tvwCategory.Nodes.Add(childNode);
        }
    }

    /// <summary>
    ///     ''' Builds sub levels if required, depending on back end levels setting
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void BuildDefaultLevels()
    {
        int numLevels = System.Convert.ToInt32(KartSettingsManager.GetKartConfig("backend.categorymenu.levels")) - 1;
        if (numLevels == 0)
            return;
        foreach (TreeNode node in tvwCategory.Nodes)
            BuildLevelsRecursive(numLevels, node);
    }

    /// <summary>
    ///     ''' Recursive build levels
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void BuildLevelsRecursive(int numLevels, TreeNode node)
    {
        if (numLevels == 0)
            return;
        node.Expand();

        foreach (TreeNode childNode in node.ChildNodes)
            BuildLevelsRecursive(numLevels - 1, childNode);
    }

    /// <summary>
    ///     ''' Build child nodes
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void BuildChildNodes(ref TreeNode node, int numParentID, int numSiteID, char chrParentType = "c")
    {
        if (chrParentType == "c")
        {
            CategoriesBLL objCategoriesBLL = new CategoriesBLL();
            DataTable tblChilds = objCategoriesBLL._GetCategoriesPageByParentID(numParentID, System.Web.UI.UserControl.Session["_LANG"], 0, 2000, 0);
            TreeNode childNode = null/* TODO Change to default(_) if this is not a reference type */;
            foreach (DataRow drwChilds in tblChilds.Rows)
            {
                childNode = new TreeNode(drwChilds("CAT_Name"), drwChilds("CAT_ID") + "|" + numSiteID);

                string strParents = node.ValuePath.Replace("/", ",");

                // Clean up so we just have cat IDs comma-separated, not the site IDs
                if (strParents.Contains("|"))
                    strParents = CleanParentsString(strParents);

                try
                {
                    string[] aryParentKey = Strings.Split(strParents, "::");
                    // parentkey = parentkey.Replace(numSiteID & "::", "")
                    strParents = aryParentKey[1];
                }
                catch (Exception ex)
                {
                }

                childNode.NavigateUrl = _CategorySiteMapProvider.CreateURL(_CategorySiteMapProvider.BackEndPage.Category, drwChilds("CAT_ID"), GetIDFromNodeValue(node.Value, true), numSiteID + "::" + strParents);

                if (childNode.ChildNodes.Count == 0)
                    childNode.PopulateOnDemand = true;
                if (System.Convert.ToBoolean(drwChilds("CAT_Live")))
                    childNode.ImageUrl = "~/Skins/Admin/Images/category-tree.gif";
                else
                    childNode.ImageUrl = "~/Skins/Admin/Images/offline-category-tree.gif";
                node.ChildNodes.Add(childNode);
            }
        }
        else if (chrParentType == "p")
        {
            ProductsBLL objProductsBLL = new ProductsBLL();
            DataTable tblChilds = objProductsBLL._GetProductsPageByCategory(numParentID, System.Web.UI.UserControl.Session["_LANG"], 0, 2000, 0);
            TreeNode childNode = null/* TODO Change to default(_) if this is not a reference type */;
            foreach (DataRow drwChilds in tblChilds.Rows)
            {
                childNode = new TreeNode(drwChilds("P_Name"), drwChilds("P_ID") + "|" + numSiteID);
                string strParents = node.ValuePath.Replace("/", ",");

                // Clean up so we just have cat IDs comma-separated, not the site IDs
                if (strParents.Contains("|"))
                    strParents = CleanParentsString(strParents);

                try
                {
                    string[] aryParentKey = Strings.Split(strParents, "::");
                    // parentkey = parentkey.Replace(numSiteID & "::", "")
                    strParents = aryParentKey[1];
                }
                catch (Exception ex)
                {
                }

                if (strParents.EndsWith("," + GetIDFromNodeValue(node.Value)))
                    strParents = Replace(strParents, "," + GetIDFromNodeValue(node.Value), "");
                else if (strParents == GetIDFromNodeValue(node.Value).ToString)
                    strParents = "0";

                string strNavigateURL = _CategorySiteMapProvider.CreateURL(_CategorySiteMapProvider.BackEndPage.Product, drwChilds("P_ID"), GetIDFromNodeValue(node.Value, true), numSiteID + "::" + strParents, GetIDFromNodeValue(node.Value));

                childNode.NavigateUrl = strNavigateURL;
                if (System.Convert.ToBoolean(drwChilds("P_Live")))
                    childNode.ImageUrl = "~/Skins/Admin/Images/product-tree.gif";
                else
                    childNode.ImageUrl = "~/Skins/Admin/Images/offline-product-tree.gif";
                node.ChildNodes.Add(childNode);
            }
        }
    }

    /// <summary>
    ///     ''' Highlight link for the current product or category, if viewing product or category
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void SelectCurrentPage()
    {
        string strCurrentURL = System.Web.UI.UserControl.Request.Url.AbsoluteUri.ToString();

        if (strCurrentURL.Contains("_Category.aspx") || strCurrentURL.Contains("_ModifyCategory.aspx"))
        {
            string strParents = "";
            string[] aryParents = Split(_GetParentCategory(), "::");
            try
            {
                strParents = aryParents[1];
            }
            catch (Exception ex)
            {
            }
            if (!string.IsNullOrEmpty(System.Web.UI.UserControl.Request.QueryString["sub"]) && !string.IsNullOrEmpty(strParents))
                FindPageNode("c", strParents + "," + System.Web.UI.UserControl.Request.QueryString["sub"]);
            else if (!string.IsNullOrEmpty(strParents))
                FindPageNode("c", strParents);
            else
                SelectCategoryNode();
        }
        else if (strCurrentURL.Contains("_ModifyProduct.aspx"))
        {
            string strParents = "";
            string[] aryParents = Split(_GetParentCategory(), "::");
            try
            {
                strParents = aryParents[1];
                if (string.IsNullOrEmpty(_GetParentCategory()) && !string.IsNullOrEmpty(_GetCategoryID))
                    FindPageNode("p", _GetCategoryID());
                else if (!string.IsNullOrEmpty(_GetParentCategory()) && !string.IsNullOrEmpty(_GetCategoryID))
                    FindPageNode("p", strParents + "," + _GetCategoryID());
            }
            catch (Exception ex)
            {
            }
        }
        if (tvwCategory.SelectedNode != null)
        {
            tvwCategory.SelectedNode.SelectAction = TreeNodeSelectAction.None;
            if (tvwCategory.SelectedNode.ChildNodes.Count == 0)
                tvwCategory.SelectedNode.PopulateOnDemand = false;
        }
    }

    /// <summary>
    ///     ''' Select category node
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void SelectCategoryNode(TreeNode ParentNode = null/* TODO Change to default(_) if this is not a reference type */)
    {
        Int64 ValueID = _GetCategoryID();
        int numSiteID = 0;
        try
        {
            numSiteID = System.Web.UI.UserControl.Request.QueryString["SiteID"];
        }
        catch (Exception ex)
        {
        }

        if (ParentNode == null)
        {
            foreach (TreeNode node in tvwCategory.Nodes)
            {
                if (node.NavigateUrl.Contains("_Category.aspx"))
                {
                    if (GetIDFromNodeValue(node.Value) == ValueID & GetIDFromNodeValue(node.Value, true) == numSiteID)
                    {
                        node.Selected = true;
                        if (node.ChildNodes.Count == 0)
                            node.PopulateOnDemand = true;
                        node.Expand();
                        break;
                    }
                }
            }
        }
        else
            foreach (TreeNode node in ParentNode.ChildNodes)
            {
                if (node.NavigateUrl.Contains("_Category.aspx"))
                {
                    if (GetIDFromNodeValue(node.Value) == ValueID & GetIDFromNodeValue(node.Value, true) == numSiteID)
                    {
                        node.Selected = true;
                        if (node.ChildNodes.Count == 0)
                            node.PopulateOnDemand = true;
                        node.Expand();
                        break;
                    }
                }
            }
    }

    /// <summary>
    ///     ''' Select product node
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void SelectProductNode(TreeNode ParentNode)
    {
        int ValueID = _GetProductID();
        int numSiteID = 0;
        try
        {
            numSiteID = System.Web.UI.UserControl.Request.QueryString["SiteID"];
        }
        catch (Exception ex)
        {
        }

        foreach (TreeNode node in ParentNode.ChildNodes)
        {
            if (node.NavigateUrl.Contains("_ModifyProduct.aspx"))
            {
                if (GetIDFromNodeValue(node.Value) == ValueID & GetIDFromNodeValue(node.Value, true) == numSiteID)
                {
                    node.Selected = true;
                    break;
                }
            }
        }
    }

    /// <summary>
    ///     ''' Find page node
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void FindPageNode(char PageType, string Parents)
    {
        int numSiteID = 0;
        try
        {
            numSiteID = System.Web.UI.UserControl.Request.QueryString["SiteID"];
        }
        catch (Exception ex)
        {
        }
        if (PageType == "c")
        {
            Parents = _CategorySiteMapProvider.StripParents(Parents);
            string[] arrCategories = Strings.Split(Parents, ",");
            for (int i = 0; i <= arrCategories.Length - 1; i++)
            {

                // This might come in with the numsite ID for uniqueness, to 
                // distinguish between same product in different sites
                string strCatContent = arrCategories[i];

                int CatID = System.Convert.ToInt32(strCatContent);

                foreach (TreeNode node in tvwCategory.Nodes)
                {
                    if (node.NavigateUrl.Contains("_Category.aspx"))
                    {
                        if (GetIDFromNodeValue(node.Value) == CatID & GetIDFromNodeValue(node.Value, true) == numSiteID)
                        {
                            try
                            {
                                node.Expand();
                                node.Selected = true;
                                if (node.Expanded)
                                {
                                    string[] arrRest = new string[arrCategories.Length - 2 + 1];
                                    int counter = 0;
                                    for (int x = 0; x <= arrCategories.Length - 1; x++)
                                    {
                                        if (arrCategories[x] != CatID)
                                        {
                                            arrRest[counter] = arrCategories[x];
                                            counter += 1;
                                        }
                                    }
                                    SelectCategoryNode(node);
                                    FindChildNodeRecursive(node, arrRest);
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                            return;
                        }
                    }
                }
            }
        }
        else if (PageType == "p")
        {
            Parents = _CategorySiteMapProvider.StripParents(Parents);
            string[] arrCategories = Strings.Split(Parents, ",");
            for (int i = 0; i <= arrCategories.Length - 1; i++)
            {

                // This might come in with the numsite ID for uniqueness, to 
                // distinguish between same product in different sites
                string strCatContent = arrCategories[i];

                int CatID = System.Convert.ToInt32(strCatContent);
                foreach (TreeNode node in tvwCategory.Nodes)
                {
                    if (node.NavigateUrl.Contains("_Category.aspx"))
                    {
                        if (GetIDFromNodeValue(node.Value) == CatID & GetIDFromNodeValue(node.Value, true) == numSiteID)
                        {
                            try
                            {
                                node.Expand();
                                if (node.Expanded)
                                {
                                    string[] arrRest = new string[arrCategories.Length - 2 + 1];
                                    int counter = 0;
                                    for (int x = 0; x <= arrCategories.Length - 1; x++)
                                    {
                                        if (arrCategories[x] != CatID)
                                        {
                                            arrRest[counter] = arrCategories[x];
                                            counter += 1;
                                        }
                                    }
                                    FindChildNodeRecursive(node, arrRest);
                                    if (tvwCategory.SelectedNode != null)
                                        SelectProductNode(tvwCategory.SelectedNode);
                                    else
                                        SelectProductNode(node);
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                            return;
                        }
                    }
                    else
                        SelectProductNode(node);
                }
            }
        }
    }

    /// <summary>
    ///     ''' Find child node, recursively
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void FindChildNodeRecursive(TreeNode node, string[] arrCategories)
    {
        int numSiteID = 0;
        try
        {
            numSiteID = System.Web.UI.UserControl.Request.QueryString["SiteID"];
        }
        catch (Exception ex)
        {
        }

        for (int i = 0; i <= arrCategories.Length - 1; i++)
        {
            int CatID = System.Convert.ToInt32(arrCategories[i]);
            foreach (TreeNode childNode in node.ChildNodes)
            {
                if (childNode.NavigateUrl.Contains("_Category.aspx"))
                {
                    if (GetIDFromNodeValue(childNode.Value) == CatID & GetIDFromNodeValue(node.Value, true) == numSiteID)
                    {
                        try
                        {
                            childNode.Expand();
                            childNode.Selected = true;
                            if (childNode.Expanded)
                            {
                                string[] arrRest = new string[arrCategories.Length - 2 + 1];
                                int counter = 0;
                                for (int x = 0; x <= arrCategories.Length - 1; x++)
                                {
                                    if (arrCategories[x] != CatID)
                                    {
                                        arrRest[counter] = arrCategories[x];
                                        counter += 1;
                                    }
                                }
                                if (_GetCategoryID() != GetIDFromNodeValue(childNode.Value))
                                    SelectCategoryNode(childNode);
                                if (arrRest.Length > 0)
                                    FindChildNodeRecursive(childNode, arrRest);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        return;
                    }
                }
            }
        }
    }

    /// <summary>
    ///     ''' Handle populated node when expanded
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void tvwCategory_TreeNodePopulate(object sender, System.Web.UI.WebControls.TreeNodeEventArgs e)
    {
        TreeNode currentNode = e.Node;
        int numSiteID = 0;
        numSiteID = GetIDFromNodeValue(e.Node.Value, true);

        // Here we look up the sub site ID to send down to child
        // cats and products based on the site name of a sub site
        // matching the treeview text for the link (i.e. the domain)
        // Dim tblSubSites As DataTable = SubSitesBLL.GetSubSites()
        // For Each drwSubSite As DataRow In tblSubSites.Rows
        // If drwSubSite("SUB_Domain").ToLower = e.Node.Text.ToLower Then numSiteID = drwSubSite("SUB_ID")
        // Next

        BuildChildNodes(ref currentNode, GetIDFromNodeValue(currentNode.Value), numSiteID);
        BuildChildNodes(ref currentNode, GetIDFromNodeValue(currentNode.Value), numSiteID, "p");
    }

    /// <summary>
    ///     ''' Pull out the category ID from the node value
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' The value used to be just category ID, but now in v3, we
    ///     ''' also store side ID, so we need function to return
    ///     ''' the category or site from the pipe separated
    ///     ''' string</remarks>
    private Int64 GetIDFromNodeValue(string strNodeValue, bool blnSiteID = false)
    {
        string[] aryIDs = Strings.Split(strNodeValue, "|");
        Int64 numOutputID = 0;
        if (blnSiteID)
            // Site ID
            numOutputID = System.Convert.ToInt64(aryIDs[1]);
        else
            numOutputID = System.Convert.ToInt64(aryIDs[0]);
        return numOutputID;
    }

    /// <summary>
    ///     ''' Clean up parents string
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' With the new multi-site functionality, we get a parents
    ///     ''' string that includes catid|siteid values instead of just
    ///     ''' catID. This should clean it up.</remarks>
    private string CleanParentsString(string strParentsString)
    {
        string[] aryString = Strings.Split(strParentsString, ",");
        for (var i = 0; i <= Information.UBound(aryString); i++)
            aryString[i] = GetIDFromNodeValue(aryString[i]);

        string strOutput = "";
        strOutput = string.Join(",", aryString);
        return strOutput;
    }
}
