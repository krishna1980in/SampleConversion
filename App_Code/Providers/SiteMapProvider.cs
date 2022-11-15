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

using System.Collections.Specialized;
using System.Web.Configuration;
using System.Web;
using System.Web.Caching;
using CkartrisDataManipulation;
using CkartrisBLL;
using System.Web.HttpContext;
using CkartrisDisplayFunctions;

/// <summary>

/// ''' uses the standard StaticSiteMapProvider in .NET framework - here we're creating a class that inherits this

/// ''' provider to build the dynamic sitemap from the Kartris database.

/// ''' </summary>
public class CategorySiteMapProvider : StaticSiteMapProvider
{
    private bool _isInitialized = false;
    private SiteMapNode _rootNode;

    private string _navigateUrl;
    private string _idFieldName;

    private Int16 numLANGID;
    private Hashtable _htRootNodes = new Hashtable();

    /// <summary>
    ///     ''' Loads configuration settings from Web configuration file
    ///     ''' </summary>
    public override void Initialize(string name, NameValueCollection attributes)
    {
        if (_isInitialized)
            return;
        base.Initialize(name, attributes);
        // Get navigateUrl from config file
        _navigateUrl = attributes["navigateUrl"];
        if ((string.IsNullOrEmpty(_navigateUrl)))
            throw new Exception("You must provide a navigateUrl attribute");

        // Get idFieldName from config file
        _idFieldName = attributes["idFieldName"];
        if (string.IsNullOrEmpty(_idFieldName))
            _idFieldName = "CategoryID";
        _isInitialized = true;
    }

    /// <summary>
    ///     ''' Retrieve the root node by building the Site Map
    ///     ''' </summary>
    protected override SiteMapNode GetRootNodeCore()
    {
        numLANGID = GetLanguageIDfromSession();
        return BuildSiteMap();
    }

    /// <summary>
    ///     ''' Resets the Category SiteMap by deleting the 
    ///     ''' root node if the current language session variable
    ///     ''' is different to the language that the current sitemap uses.
    ///     ''' </summary>
    public void ResetSiteMap()
    {
        numLANGID = GetLanguageIDfromSession();
        if (!Information.IsNothing(_rootNode))
        {
            if (_rootNode.Key != "0-" + numLANGID)
                _rootNode = null;
        }
    }

    /// <summary>
    ///     ''' Clears all the nodes in the Category SiteMap (all languages).
    ///     ''' This will cause the SiteMap to rebuild all of its nodes from stratch.
    ///     ''' *Primarily used for category updates in the backend.
    ///     ''' </summary>
    public void RefreshSiteMap()
    {
        _rootNode = null;
        StaticSiteMapProvider.Clear();
        _htRootNodes.Clear();
        HttpRuntime.Cache.Insert("CategoryMenuKey", DateTime.Now);
    }

    /// <summary>
    ///     ''' Build the Site Map by retrieving
    ///     ''' records from database table
    ///     ''' </summary>
    public override SiteMapNode BuildSiteMap()
    {
        // Only allow the Site Map to be created by a single thread
        lock (this)
        {
            _rootNode = (SiteMapNode)_htRootNodes.Item(numLANGID);
            if (_rootNode == null)
            {
                // Show trace for debugging
                HttpContext.Current.Trace.Warn("Loading category site map from database");

                // Clear current Site Map
                RefreshSiteMap();

                // Load the database data
                DataTable tblSiteMap = GetSiteMapFromDB(numLANGID);
                // Set the root node
                string strCategoryLabel = HttpContext.GetGlobalResourceObject("Kartris", "ContentText_Categories");
                _rootNode = new SiteMapNode(this, "0-" + numLANGID, "~/Category.aspx?L=" + numLANGID, strCategoryLabel, strCategoryLabel);
                SiteMapNode HomeNode = new SiteMapNode(SiteMap.Providers.Item["BreadCrumbSiteMap"], "Home", "~/Default.aspx", HttpContext.GetGlobalResourceObject("Kartris", "ContentText_Home"));

                StaticSiteMapProvider.AddNode(_rootNode, HomeNode);
                _htRootNodes.Add(numLANGID, _rootNode);

                // Build the child nodes 
                BuildSiteMapRecurse(tblSiteMap, _rootNode, "0-" + numLANGID);
            }
            return _rootNode;
        }
    }

    /// <summary>
    ///     ''' Load the contents of the database table
    ///     ''' that contains the Site Map
    ///     ''' </summary>
    private DataTable GetSiteMapFromDB(int numLANGID)
    {
        CategoriesBLL objCategoriesBLL = new CategoriesBLL();
        return objCategoriesBLL.GetHierarchyByLanguageID(numLANGID);
    }

    /// <summary>
    ///     ''' Recursively build the Site Map from the DataTable
    ///     ''' </summary>
    private void BuildSiteMapRecurse(DataTable siteMapTable, SiteMapNode parentNode, string parentkey, int parentCGroup = 0)
    {
        Array arr = Strings.Split(parentNode.Key, ",");

        string strMainParent = "";
        if (arr(Information.UBound(arr)) == "0-" + numLANGID)
            strMainParent = 0;
        else
            strMainParent = arr(Information.UBound(arr));

        DataRow[] results = siteMapTable.Select("ParentID=" + strMainParent);
        foreach (DataRow row in results)
        {
            string url;
            string strParentKey;
            if (Strings.InStr(parentkey, "0-" + numLANGID + ","))
                strParentKey = Strings.Mid(parentkey, Strings.InStr(parentkey, ",") + 1);
            else
                strParentKey = parentkey;
            if (strParentKey == "0-" + numLANGID)
                strParentKey = "";
            if (strParentKey != "")
            {
                string[] arrTempParentKeys = Strings.Split(strParentKey, ",");
                if (Information.UBound(arrTempParentKeys) > 0)
                {
                    if (arrTempParentKeys[Information.UBound(arrTempParentKeys)] != row("parentid"))
                    {
                        strParentKey = strParentKey + "," + row("parentid");
                        parentkey = parentkey + "," + row("parentid");
                    }
                }
                else if (strParentKey != row("parentid"))
                {
                    strParentKey = strParentKey + "," + row("parentid");
                    parentkey = parentkey + "," + row("parentid");
                }
            }
            else
            {
                strParentKey = row("parentid");
                if (row("parentid") == 0)
                    parentkey = "0-" + numLANGID;
                else
                    parentkey = parentkey + "," + row("parentid");
            }

            url = SiteMapHelper.CreateURL(SiteMapHelper.Page.Category, row("CAT_ID"), strParentKey, strRetrievedName: row("title").ToString());
            SiteMapNode node = new SiteMapNode(this, parentkey + "," + row("CAT_ID").ToString(), url, row("title").ToString());
            var intCategoryCustomerGroup = System.Convert.ToInt32(FixNullFromDB(row("CAT_CustomerGroupID")));
            // if the parent has customer group, override this category's customer group
            if (parentCGroup > 0)
                intCategoryCustomerGroup = parentCGroup;
            node.Item["CG_ID"] = intCategoryCustomerGroup;
            node.Item["CAT_ID"] = row("CAT_ID");
            StaticSiteMapProvider.AddNode(node, parentNode);

            BuildSiteMapRecurse(siteMapTable, node, parentNode.Key, intCategoryCustomerGroup);
        }
    }
}

public class SiteMapHelper
{
    public enum Page
    {
        Product,
        Category,
        CustomPage,
        Search,
        News,
        CanonicalProduct,
        CanonicalCategory,
        Knowledgebase,
        SupportTicket
    }

    /// <summary>
    ///     ''' Map friendly URL to parametrized equivalent
    ///     ''' </summary>
    public static SiteMapNode ExpandPathForUnmappedPages(object sender, SiteMapResolveEventArgs e)
    {
        HttpContext context = HttpContext.Current;
        int numLangID = GetLanguageIDfromSession();
        System.Web.UI.Page PageHandler = context.Handler;
        if (!PageHandler.IsPostBack)
        {
            if (!(context.Request.RawUrl.ToLower().Contains("/javascript/") | context.Request.RawUrl.ToLower().Contains("/skins/") | context.Request.RawUrl.ToLower().Contains("/images/")))
            {
                try
                {
                    // Create a custom SiteMapNode for various pages that display records based on ID
                    if (context.Request.Path.ToLower().Contains("/product.aspx"))
                    {
                        if (!String.IsNullOrEmpty(context.Request["CategoryID"]) & !String.IsNullOrEmpty(context.Request["ProductID"]))
                        {
                            string strParentkeys = String.Empty;

                            Int64 id = Convert.ToString(context.Request["CategoryID"]);
                            Int64 pid = Convert.ToString(context.Request["ProductID"]);

                            // Create a new SiteMapNode to represent the requested page 
                            // Dim node As New SiteMapNode(SiteMap.Provider, context.Request.Path, context.Request.Path, ProductsBLL.GetNameByProductID(pid, 1))
                            SiteMapNode node = new SiteMapNode(SiteMap.Provider, context.Request.Path, context.Request.Path, GetProductName(pid, numLangID));


                            if (!String.IsNullOrEmpty(context.Request["strParent"]))
                                strParentkeys = Convert.ToString(context.Request["strParent"]) + ",";


                            if (String.IsNullOrEmpty(strParentkeys))
                                node.ParentNode = SiteMap.Provider.FindSiteMapNodeFromKey("0-" + numLangID + "," + id);
                            else
                                // Get the parent node from the site map and parent the new node to it 
                                node.ParentNode = SiteMap.Provider.FindSiteMapNodeFromKey("0-" + numLangID + "," + strParentkeys + id);

                            return node;
                        }
                        else if ((context.Request["strReferer"]) == "search" & !String.IsNullOrEmpty(context.Request["ProductID"]))
                        {
                            int pid = Convert.ToString(context.Request["ProductID"]);

                            // Create a new SiteMapNode to represent the requested page 
                            // Dim node As New SiteMapNode(SiteMap.Provider, context.Request.Path, context.Request.Path, ProductsBLL.GetNameByProductID(pid, 1))
                            SiteMapNode node = new SiteMapNode(SiteMap.Provider, context.Request.Path, context.Request.Path, GetProductName(pid, numLangID));

                            // The link came from the search results
                            SiteMapNode ParentSearchNode = new SiteMapNode(SiteMap.Providers.Item["BreadCrumbSiteMap"], "Search", "~/Search.aspx", HttpContext.GetGlobalResourceObject("Kartris", "ContentText_Search"));

                            // What page of results did user click from?
                            int intPageNumber = 0;
                            try
                            {
                                intPageNumber = context.Request["PPGR"];
                            }
                            catch (Exception ex)
                            {
                                intPageNumber = 0;
                            }

                            // Format URL for search results breadcrumb
                            string strSearchResultsLink = "~/Search.aspx?strResults=y&PPGR=" + intPageNumber;

                            SiteMapNode SearchResultsNode = new SiteMapNode(SiteMap.Providers.Item["BreadCrumbSiteMap"], "SearchResults", strSearchResultsLink, HttpContext.GetGlobalResourceObject("Search", "ContentText_SearchResults"));
                            SearchResultsNode.ParentNode = ParentSearchNode;
                            node.ParentNode = SearchResultsNode;
                            return node;
                        }
                        else if (!String.IsNullOrEmpty(context.Request["ProductID"]))
                        {
                            Int64 pid = System.Convert.ToInt64(context.Request["ProductID"]);
                            if (pid > 0)
                            {
                                SiteMapNode HomeNode = new SiteMapNode(SiteMap.Providers.Item["BreadCrumbSiteMap"], "Home", "~/Default.aspx", HttpContext.GetGlobalResourceObject("Kartris", "ContentText_Home"));
                                SiteMapNode node = new SiteMapNode(SiteMap.Providers.Item["BreadCrumbSiteMap"], "Product", context.Request.Url.ToString(), GetProductName(pid, numLangID));
                                node.ParentNode = HomeNode;
                                return node;
                            }
                        }
                    }
                    else if (context.Request.Path.ToLower().Contains("/category.aspx"))
                    {
                        // Create a custom SiteMapNode for category.aspx. 
                        if (!String.IsNullOrEmpty(context.Request["CategoryID"]))
                        {
                            int id = Convert.ToString(context.Request["CategoryID"]);
                            SiteMapNode node;
                            string strParentkeys = "";
                            if (!String.IsNullOrEmpty(context.Request["strParent"]))
                                strParentkeys = "0-" + numLangID + "," + Convert.ToString(context.Request["strParent"]);
                            else
                                strParentkeys = "0-" + numLangID;
                            node = SiteMap.Provider.FindSiteMapNodeFromKey(strParentkeys + "," + id);
                            if (node == null)
                            {
                                SiteMapNode HomeNode = new SiteMapNode(SiteMap.Providers.Item["BreadCrumbSiteMap"], "Home", "~/Default.aspx", HttpContext.GetGlobalResourceObject("Kartris", "ContentText_Home"));
                                node = new SiteMapNode(SiteMap.Providers.Item["BreadCrumbSiteMap"], "custom" + id, CreateURL(Page.CanonicalCategory, id), GetCategoryName(id, numLangID));
                                node.ParentNode = HomeNode;
                            }
                            return node;
                        }
                    }
                    else if (context.Request.Path.ToLower().Contains("compare.aspx"))
                    {
                        SiteMapNode HomeNode = new SiteMapNode(SiteMap.Providers.Item["BreadCrumbSiteMap"], "Home", "~/Default.aspx", HttpContext.GetGlobalResourceObject("Kartris", "ContentText_Home"));
                        if (!String.IsNullOrEmpty(context.Request["CategoryID"]))
                        {
                            Int64 id = Convert.ToString(context.Request["CategoryID"]);
                            SiteMapNode node;
                            string strParentkeys = "";
                            if (!String.IsNullOrEmpty(context.Request["strParent"]))
                                strParentkeys = "&strParent=" + Convert.ToString(context.Request["strParent"]);


                            if (!String.IsNullOrEmpty(context.Request["ProductID"]))
                            {
                                if (context.Request["action"] == "add")
                                {
                                    string strNodePath;
                                    strNodePath = "~/Product.aspx?ProductID=" + context.Request["ProductID"] + "&strPageHistory=compare";
                                    SiteMapNode parentNode;
                                    // parentNode = New SiteMapNode(SiteMap.Providers.Item("BreadCrumbSiteMap"), context.Request.Path, strNodePath, ProductsBLL.GetNameByProductID(context.Request("id"), 1))
                                    parentNode = new SiteMapNode(SiteMap.Providers.Item["BreadCrumbSiteMap"], context.Request.Path, strNodePath, GetProductName(context.Request["id"], numLangID));
                                    parentNode.ParentNode = SiteMap.Provider.FindSiteMapNodeFromKey(strParentkeys + "," + id);
                                    node = new SiteMapNode(SiteMap.Providers.Item["BreadCrumbSiteMap"], "Compare", context.Request.Url.ToString(), HttpContext.GetGlobalResourceObject("Products", "PageTitle_ProductComparision"));
                                    node.ParentNode = parentNode;
                                }
                                else
                                    node = null;
                            }
                            else
                            {
                                node = new SiteMapNode(SiteMap.Providers.Item["BreadCrumbSiteMap"], "Compare", context.Request.Url.ToString(), HttpContext.GetGlobalResourceObject("Products", "PageTitle_ProductComparision"));
                                node.Title += "(" + GetProductName(context.Request["id"], numLangID) + ")";
                                node.ParentNode = SiteMap.Provider.FindSiteMapNodeFromKey(strParentkeys + "," + id);
                            }

                            return node;
                        }
                        else
                        {
                            SiteMapNode node = new SiteMapNode(SiteMap.Providers.Item["BreadCrumbSiteMap"], "Compare", context.Request.Url.ToString(), HttpContext.GetGlobalResourceObject("Products", "PageTitle_ProductComparision"));
                            node.ParentNode = HomeNode;
                            return node;
                        }
                    }
                    else if (context.Request.Path.ToLower().Contains("page.aspx"))
                    {
                        string strPage = Convert.ToString(context.Request["strPage"]);
                        DataTable tblPage = PagesBLL.GetPageByName(numLangID, strPage);
                        SiteMapNode node;
                        if (tblPage.Rows.Count > 0)
                        {
                            string strPageName = tblPage.Rows(0).Item("Page_Title").ToString();
                            strPageName = Strings.Replace(strPageName, "-", " ");
                            node = new SiteMapNode(SiteMap.Providers.Item["BreadCrumbSiteMap"], "CustomPage", context.Request.Url.ToString(), strPageName);

                            SiteMapNode HomeNode = SiteMap.Providers.Item["BreadCrumbSiteMap"].RootNode;

                            Int64 intParentID = tblPage.Rows(0).Item("Page_ParentID").ToString;

                            if (intParentID > 0)
                                RecursiveNodeToHome(numLangID, node, intParentID, HomeNode);
                            else
                                node.ParentNode = HomeNode;
                        }
                        else
                            node = null;
                        return node;
                    }
                    else if (context.Request.Path.ToLower().Contains("news.aspx"))
                    {
                        int intNewsID = Convert.ToString(context.Request["NewsID"]);
                        if (intNewsID > 0)
                        {
                            string strNewsTitle = NewsBLL.GetNewsTitleByID(intNewsID, numLangID);
                            SiteMapNode node = new SiteMapNode(SiteMap.Providers.Item["BreadCrumbSiteMap"], "NewsPage", context.Request.Url.ToString(), strNewsTitle);
                            SiteMapNode NewsNode = SiteMap.Providers.Item["BreadCrumbSiteMap"].FindSiteMapNode("~/News.aspx");
                            node.ParentNode = NewsNode;
                            return node;
                        }
                        return null;
                    }
                    else if (context.Request.Path.ToLower().Contains("knowledgebase.aspx"))
                    {
                        if (!context.Request["kb"] == null)
                        {
                            int intKBID = Convert.ToString(context.Request["kb"]);
                            if (intKBID > 0)
                            {
                                string strKBTitle = GetKnowledgebaseTitle(intKBID, numLangID);
                                SiteMapNode node = new SiteMapNode(SiteMap.Providers.Item["BreadCrumbSiteMap"], "Knowledgebase", context.Request.Url.ToString(), strKBTitle);
                                SiteMapNode KBNode = SiteMap.Providers.Item["BreadCrumbSiteMap"].FindSiteMapNode("~/Knowledgebase.aspx");
                                node.ParentNode = KBNode;
                                return node;
                            }
                        }
                    }
                    else if (context.Request.Path.ToLower().Contains("customertickets.aspx"))
                    {
                        if (!context.Request["TIC_ID"] == null)
                        {
                            int intTICID = Convert.ToString(context.Request["TIC_ID"]);
                            if (intTICID > 0)
                            {
                                string strTicketTitle = "#" + intTICID.ToString();
                                SiteMapNode node = new SiteMapNode(SiteMap.Providers.Item["BreadCrumbSiteMap"], "SupportTicket", context.Request.Url.ToString(), strTicketTitle);
                                SiteMapNode TicNode = SiteMap.Providers.Item["BreadCrumbSiteMap"].FindSiteMapNode("~/CustomerTickets.aspx");
                                node.ParentNode = TicNode;
                                return node;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Give a 404 responsive by transferring
                    HttpContext.Current.Server.Transfer("~/404.aspx");
                }
            }
        }

        // Also check the backend URLs
        if (context.Request.Path.ToLower().Contains("_modifycategory.aspx"))
        {
            Int64 id = System.Convert.ToInt64(context.Request["CategoryID"]);
            SiteMapNode node;
            if (id > 0)
            {
                node = new SiteMapNode(SiteMap.Providers["_CategorySiteMapProvider"], "_category-" + id, context.Request.Path.ToString(), HttpContext.GetGlobalResourceObject("_Category", "ContentText_EditThisCategory"));
                node.ParentNode = SiteMap.Providers.Item["_CategorySiteMapProvider"].FindSiteMapNodeFromKey(Convert.ToString(context.Request["strParent"] + "," + id));
            }
            else
            {
                node = new SiteMapNode(SiteMap.Providers["_CategorySiteMapProvider"], "_category-" + id, context.Request.Path.ToString(), HttpContext.GetGlobalResourceObject("_Category", "ContentText_AddNewSubcategory"));
                string strSub = Interaction.IIf(string.IsNullOrEmpty(System.Convert.ToString(context.Request["Sub"])), "0,", System.Convert.ToString(context.Request["Sub"]) + ",");
                node.ParentNode = SiteMap.Providers.Item["_CategorySiteMapProvider"].FindSiteMapNodeFromKey(strSub + Convert.ToString(context.Request["strParent"]));
            }

            return node;
        }
        else if (context.Request.Path.ToLower().Contains("_modifyproduct.aspx"))
        {
            Int64 id = System.Convert.ToInt64(context.Request["CategoryID"]);
            Int64 ProductID = Convert.ToString(context.Request["ProductID"]);
            SiteMapNode node;
            if (ProductID > 0)
                node = new SiteMapNode(SiteMap.Providers["_CategorySiteMapProvider"], "_product-" + ProductID, context.Request.Path.ToString(), GetProductName(ProductID, numLangID));
            else
                node = new SiteMapNode(SiteMap.Providers["_CategorySiteMapProvider"], "_product-" + ProductID, context.Request.Path.ToString(), HttpContext.GetGlobalResourceObject("_Category", "ContentText_AddNewProduct"));
            node.ParentNode = SiteMap.Providers.Item["_CategorySiteMapProvider"].FindSiteMapNodeFromKey(Convert.ToString(context.Request["strParent"] + "," + id));

            return node;
        }
        else if (context.Request.Path.ToLower().Contains("_category.aspx"))
        {
            // Try
            // Dim id As Integer = CInt(context.Request("CategoryID"))
            // Dim node As SiteMapNode
            // Dim strCategoryName As String = "test" 'GetCategoryName(id, numLangID, True)
            // node = New SiteMapNode(SiteMap.Providers("_CategorySiteMapProvider"), "_category-" & id, context.Request.Path.ToString, strCategoryName)
            // Dim strSub As String = IIf(String.IsNullOrEmpty(CStr(context.Request("Sub"))), "0,", CStr(context.Request("Sub")) & ",")
            // node.ParentNode = SiteMap.Providers.Item("_CategorySiteMapProvider").FindSiteMapNodeFromKey(strSub & Convert.ToString(context.Request("strParent")))

            // Return node
            // Catch ex As Exception

            // End Try
            Int64 id = System.Convert.ToInt64(context.Request["CategoryID"]);
            Int64 numSiteID = System.Convert.ToInt64(context.Request["SiteID"]);
            SiteMapNode node;
            string strCategoryName = GetCategoryName(id, numLangID, true);
            string strParent = Convert.ToString(context.Request["strParent"]);
            node = new SiteMapNode(SiteMap.Providers["_CategorySiteMapProvider"], "_category-" + id, context.Request.Path.ToString(), strCategoryName);
            string strSub = Interaction.IIf(string.IsNullOrEmpty(System.Convert.ToString(context.Request["Sub"])), "0,", System.Convert.ToString(context.Request["Sub"]) + ",");
            // node.ParentNode = SiteMap.Providers.Item("_CategorySiteMapProvider").FindSiteMapNodeFromKey(strSub & strParent)
            node.ParentNode = SiteMap.Providers.Item["_CategorySiteMapProvider"].FindSiteMapNodeFromKey(Convert.ToString(context.Request["strParent"] + "," + id + "," + numSiteID));
        }

        // 
        // Do nothing for other pages. 
        // 

        return null;
    }

    private static SiteMapNode RecursiveNodeToHome(int intLangID, SiteMapNode node, int intParentID, SiteMapNode HomeNode)
    {
        DataTable tblPage = PagesBLL._GetPageByID(intLangID, intParentID);
        if (tblPage.Rows.Count > 0)
        {
            string strPageName = tblPage.Rows(0).Item("Page_Name").ToString();
            string intNewParent = tblPage.Rows(0).Item("Page_ParentID").ToString();
            SiteMapNode CustomParentNode = new SiteMapNode(SiteMap.Providers.Item["BreadCrumbSiteMap"], "CustomParentPage", "~/t-" + strPageName + ".aspx", Strings.Replace(strPageName, "-", " "));
            node.ParentNode = CustomParentNode;
            if (intNewParent > 0)
                RecursiveNodeToHome(intLangID, CustomParentNode, intNewParent, HomeNode);
            else
                CustomParentNode.ParentNode = HomeNode;
        }
        else
            node.ParentNode = HomeNode;
        return node;
    }

    public static string CreateURL(Page strPageType, int ID, string strParents = "", int ParentID = 0, int CPagerID = 0, int PPagerID = 0, string strActiveTab = "p", string strRetrievedName = "")
    {
        int numLangID = GetLanguageIDfromSession();

        string strUserCulture;

        if (LanguagesBLL.GetLanguagesCount > 1)
        {
            if (string.IsNullOrEmpty(HttpContext.Current.Session.Item["KartrisUserCulture"]))
                strUserCulture = LanguagesBLL.GetCultureByLanguageID_s(LanguagesBLL.GetDefaultLanguageID()) + "/";
            else
                strUserCulture = System.Convert.ToString(HttpContext.Current.Session.Item["KartrisUserCulture"]) + "/";
        }
        else
            strUserCulture = "";


        bool blnSEOLinks = KartSettingsManager.GetKartConfig("general.seofriendlyurls.enabled") == "y";
        string strWebShopFolder = FixURLText(WebShopFolder());
        string strURL;
        switch (strPageType)
        {
            case Page.Category:
                {
                    if (Strings.Left(strParents, 1) == ",")
                        strParents = Strings.Mid(strParents, 2);
                    if (blnSEOLinks)
                    {
                        string strFriendlyParent = "";
                        string strCategoryName = "";
                        if (!(strParents == "" | strParents == "0"))
                        {
                            strFriendlyParent = Strings.Replace(strParents, ",", "-");
                            string strCategoryParents = "";

                            Array arrParents = Strings.Split(strFriendlyParent, "-");
                            for (int index = 0; index <= Information.UBound(arrParents); index++)
                                strCategoryParents += Strings.Replace(GetCategoryName(arrParents(index), numLangID, true), " ", "-") + "/";

                            if (string.IsNullOrEmpty(strRetrievedName))
                                strCategoryName = GetCategoryName(ID, numLangID, true);
                            else
                                strCategoryName = CleanURL(strRetrievedName);
                            strCategoryParents = strCategoryParents + Strings.Replace(strCategoryName, " ", "-");
                            strURL = string.Format("/{0}{6}{1}__c-{5}-{4}-{2}-{3}", strWebShopFolder, FixURLText(strCategoryParents), FixURLText(strFriendlyParent), ID, CPagerID + "-" + PPagerID, strActiveTab, strUserCulture);
                        }
                        else
                            strURL = string.Format("/{2}{5}{0}__c-{4}-{3}-{1}", FixURLText(Strings.Replace(GetCategoryName(ID, numLangID, true), " ", "-")), ID, strWebShopFolder, CPagerID + "-" + PPagerID, strActiveTab, strUserCulture);
                        strURL += ".aspx";
                        strURL = CkartrisDisplayFunctions.CleanURL(strURL);

                        // Check if URL length is greater than 280
                        if ((strURL.Length + WebShopURL.Length - 1) > 280)
                        {
                            strURL = string.Format("/{0}{6}{1}__c-{5}-{4}-{2}-{3}", strWebShopFolder, FixURLText(Strings.Replace(strCategoryName, " ", "-")), FixURLText(strFriendlyParent), ID, CPagerID + "-" + PPagerID, strActiveTab, strUserCulture);
                            strURL += ".aspx";
                            strURL = CkartrisDisplayFunctions.CleanURL(strURL);
                            // If URL length is still greater than 280 post processing then just return a canonical link
                            if ((strURL.Length + WebShopURL.Length - 1) > 280)
                                strURL = CreateURL(Page.CanonicalCategory, ID);
                        }
                    }
                    else
                    {
                        string strPageName = "~/Category.aspx";
                        if (!(strParents == "" | strParents == "0"))
                            strURL = string.Format("{0}?CategoryID={1}&strParent={2}", strPageName, ID, strParents);
                        else if (ParentID == 0)
                            strURL = string.Format("{0}?CategoryID={1}", strPageName, ID);
                        else
                            strURL = string.Format("{0}?CategoryID={1}&strParent={2}", strPageName, ID, ParentID);
                        // If numLangID > 1 Then strURL += "&L=" & numLangID
                        if (CPagerID != 0)
                            strURL += "&CPGR=" + CPagerID;
                        if (PPagerID != 0)
                            strURL += "&PPGR=" + PPagerID;
                        if (strActiveTab == "s")
                            strURL += "&T=S";
                    }

                    break;
                }

            case Page.Product:
                {
                    string strPageName = "~/Product.aspx";
                    if (strParents != null)
                    {
                        if (strParents.ToLower() == "search")
                        {
                            strURL = string.Format("{0}?ProductID={1}&strParent=search", strPageName, ID);
                            break;
                        }
                    }

                    if (blnSEOLinks)
                    {
                        string strFriendlyParent = "";
                        if (strParents == "")
                        {
                            strURL = "";
                            if (ParentID > 0)
                                strURL = Strings.Replace(GetCategoryName(ParentID, numLangID, true), " ", "-") + "/";
                            strURL += Strings.Replace(GetProductName(ID, numLangID, true), " ", "-");
                            if (ParentID > 0)
                                strURL = string.Format("/{3}{4}{0}__p-{2}-{1}", FixURLText(strURL), ID, ParentID, strWebShopFolder, strUserCulture);
                            else
                                strURL = string.Format("/{2}{3}{0}__p-{1}", FixURLText(strURL), ID, strWebShopFolder, strUserCulture);
                        }
                        else
                        {
                            strFriendlyParent = Strings.Replace(strParents, ",", "-");
                            string strCategoryParents = "";
                            Array arrParents = Strings.Split(strFriendlyParent, "-");
                            for (int index = 0; index <= Information.UBound(arrParents); index++)

                                strCategoryParents += Strings.Replace(GetCategoryName(arrParents(index), numLangID, true), " ", "-") + "/";

                            strCategoryParents += Strings.Replace(GetCategoryName(ParentID, numLangID, true), " ", "-") + "/" + Strings.Replace(GetProductName(ID, numLangID, true), " ", "-");

                            strURL = string.Format("/{4}{5}{0}__p-{3}-{2}-{1}", FixURLText(strCategoryParents), ID, ParentID, FixURLText(strFriendlyParent), strWebShopFolder, strUserCulture);
                        }
                        strURL += ".aspx";
                        strURL = CkartrisDisplayFunctions.CleanURL(strURL);

                        // Check if URL length is greater than 280
                        if ((strURL.Length + WebShopURL.Length - 1) > 280)
                        {
                            strURL = string.Format("/{4}{5}{0}__p-{3}-{2}-{1}", FixURLText(Strings.Replace(GetCategoryName(ParentID, numLangID, true), " ", "-") + "/" + Strings.Replace(GetProductName(ID, numLangID, true), " ", "-")), ID, ParentID, FixURLText(strFriendlyParent), strWebShopFolder, strUserCulture);
                            strURL += ".aspx";
                            strURL = CkartrisDisplayFunctions.CleanURL(strURL);
                            // If URL length is still greater than 280 post processing then just return a canonical link
                            if ((strURL.Length + WebShopURL.Length - 1) > 280)
                                strURL = CreateURL(Page.CanonicalProduct, ID);
                        }
                    }
                    else
                    {
                        if (strParents == "")
                        {
                            if (ParentID > 0)
                                strURL = string.Format("{0}?ProductID={1}&CategoryID={2}", strPageName, ID, ParentID);
                            else
                                strURL = string.Format("{0}?ProductID={1}", strPageName, ID);
                        }
                        else
                            strURL = string.Format("{0}?ProductID={1}&CategoryID={2}&strParent={3}", strPageName, ID, ParentID, strParents);
                        if (numLangID > 1)
                            strURL += "&L=" + numLangID;
                    }

                    break;
                }

            case Page.CanonicalProduct:
                {
                    if (blnSEOLinks)
                    {
                        strURL = Strings.Replace(GetProductName(ID, numLangID, true), " ", "-");
                        strURL = string.Format("/{2}{3}{0}__p-{1}", FixURLText(strURL), ID, strWebShopFolder, strUserCulture);
                        strURL += ".aspx";
                        strURL = CkartrisDisplayFunctions.CleanURL(strURL);

                        if ((strURL.Length + WebShopURL.Length - 1) > 280)
                        {
                            strURL = string.Format("/{2}{3}{0}__p-{1}", "", ID, strWebShopFolder, strUserCulture);
                            strURL += ".aspx";
                            strURL = CkartrisDisplayFunctions.CleanURL(strURL);
                        }
                    }
                    else
                    {
                        string strPageName = WebShopURL() + "Product.aspx";
                        strURL = string.Format("{0}?ProductID={1}", strPageName, ID);
                        if (numLangID > 1)
                            strURL += "&L=" + numLangID;
                    }

                    break;
                }

            case Page.CanonicalCategory:
                {
                    if (blnSEOLinks)
                    {
                        strURL = string.Format("/{2}{4}{0}__c-{3}-0-0-{1}", FixURLText(Strings.Replace(GetCategoryName(ID, numLangID, true), " ", "-")), ID, strWebShopFolder, strActiveTab, strUserCulture);
                        strURL += ".aspx";
                        strURL = CkartrisDisplayFunctions.CleanURL(strURL);
                        if ((strURL.Length + WebShopURL.Length - 1) > 280)
                        {
                            strURL = string.Format("/{2}{4}{0}__c-{3}-0-0-{1}", "", ID, strWebShopFolder, strActiveTab, strUserCulture);
                            strURL += ".aspx";
                            strURL = CkartrisDisplayFunctions.CleanURL(strURL);
                        }
                    }
                    else
                    {
                        string strPageName = WebShopURL() + "Category.aspx";
                        strURL = string.Format("{0}?CategoryID={1}", strPageName, ID);
                        if (numLangID > 1)
                            strURL += "&L=" + numLangID;
                    }

                    break;
                }

            case Page.News:
                {
                    if (blnSEOLinks)
                    {
                        strURL = string.Format("/{2}{3}{0}__n-{1}", FixURLText(Strings.Replace(GetNewsTitle(ID, numLangID), " ", "-")), ID, strWebShopFolder, strUserCulture);
                        strURL += ".aspx";
                        strURL = CkartrisDisplayFunctions.CleanURL(strURL);
                    }
                    else
                    {
                        string strPageName = WebShopURL() + "News.aspx";
                        strURL = string.Format("{0}?NewsID={1}", strPageName, ID);
                    }

                    break;
                }

            case Page.Search:
                {
                    string strPageName = "~/Search.aspx?strResults=y";
                    strURL = strPageName;
                    if (PPagerID != 0)
                        strURL += "&PPGR=" + PPagerID;
                    if (numLangID > 1)
                        strURL += "&L=" + numLangID;
                    break;
                }

            case Page.Knowledgebase:
                {
                    if (blnSEOLinks)
                    {
                        strURL = string.Format("/{2}{3}{4}{0}__k-{1}", FixURLText(Strings.Replace(GetKnowledgebaseTitle(ID, numLangID), " ", "-")), ID, strWebShopFolder, strUserCulture, FixURLText(HttpContext.GetGlobalResourceObject("Knowledgebase", "PageTitle_Knowledgebase")) + "/");
                        strURL += ".aspx";
                        strURL = CkartrisDisplayFunctions.CleanURL(strURL);
                    }
                    else
                    {
                        string strPageName = WebShopURL() + "Knowledgebase.aspx";
                        strURL = string.Format("{0}?kb={1}", strPageName, ID);
                    }

                    break;
                }

            default:
                {
                    return null;
                }
        }

        return TrimBadChars(strURL);
    }

    private static string TrimBadChars(string strURL)
    {
        string strTrimmedURL = strURL;

        // Trim double dots
        try
        {
            while (Strings.InStr(strTrimmedURL, "..") > 0)
                strTrimmedURL = Strings.Replace(strTrimmedURL, "..", ".");
        }
        catch (Exception ex)
        {
            strTrimmedURL = strURL;
        }

        // Trim plus sign
        strTrimmedURL = Strings.Replace(strTrimmedURL, "+", "");

        // Trim dot-slash
        strTrimmedURL = Strings.Replace(strTrimmedURL, "./", "/");

        // Trim double quote sign
        strTrimmedURL = Strings.Replace(strTrimmedURL, "\"", "");

        return strTrimmedURL;
    }

    private static string FixURLText(string strText)
    {
        if (string.IsNullOrEmpty(strText))
            return strText;
        return strText.Replace("__", "_");
    }

    public static string FindItemBackEndURL(string strCurrentPath)
    {
        Array arrKeys;
        string strWebShopFolder = "~/";
        string strQueryStrings = "";
        // Extract the language and culture info from the friendly URL
        strCurrentPath = Strings.LCase(Strings.Replace(strCurrentPath, ":80", ""));
        string strWebShopURL;
        string strURLCultureInfo;
        string strOriginalPath = strCurrentPath;

        if (InStr(strCurrentPath, WebShopURL))
            strWebShopURL = LCase(WebShopURL());
        else
            strWebShopURL = LCase(WebShopFolder());
        short numLangID;
        if (LanguagesBLL.GetLanguagesCount > 1)
        {
            strURLCultureInfo = Strings.Left(Strings.Mid(strCurrentPath, Strings.InStr(strCurrentPath, strWebShopURL) + Strings.Len(strWebShopURL)), 5);
            numLangID = LanguagesBLL.GetLanguageIDByCulture_s(strURLCultureInfo);
        }
        else
            numLangID = LanguagesBLL.GetDefaultLanguageID;

        if (numLangID == 0)
            numLangID = GetLanguageIDfromSession();

        if (strCurrentPath.Contains(".aspx?"))
        {
            strQueryStrings = "&" + Strings.Mid(strCurrentPath, strCurrentPath.LastIndexOf("?") + 2);
            strCurrentPath = Strings.Left(strCurrentPath, strCurrentPath.LastIndexOf("?"));
        }

        if (Strings.Right(strCurrentPath, 5) == ".aspx")
            strCurrentPath = Strings.Left(strCurrentPath, Strings.Len(strCurrentPath) - 5);

        // What kind of page is this?
        if ((strCurrentPath.ToLower().Contains("__c-")))
        {
            // -----------------------------------
            // Category
            // -----------------------------------
            string strParent = strCurrentPath.Substring(strCurrentPath.IndexOf("__c-") + 4);
            arrKeys = Strings.Split(strParent, "-");
            int intUpper = Information.UBound(arrKeys);
            string strOutput;
            if (intUpper == 3)
                strOutput = "Admin/_ModifyCategory.aspx?CategoryID=" + arrKeys(intUpper) + "&CPGR=" + arrKeys(1) + "&PPGR=" + arrKeys(2);
            else if (intUpper == 4)
                strOutput = "Admin/_ModifyCategory.aspx?CategoryID=" + arrKeys(4) + "&strParent=" + arrKeys(3) + "&CPGR=" + arrKeys(1) + "&PPGR=" + arrKeys(2);
            else if (intUpper > 4)
            {
                strParent = arrKeys(3);
                for (int ctr = 4; ctr <= (intUpper - 1); ctr++)
                    strParent += "," + arrKeys(ctr);
                strOutput = "Admin/_ModifyCategory.aspx?CategoryID=" + arrKeys(intUpper).ToString() + "&strParent=" + strParent + "&CPGR=" + arrKeys(1) + "&PPGR=" + arrKeys(2);
            }
            else
                strOutput = "";
            if (arrKeys(0) == "s")
                strOutput += "&T=S";
            strOutput += "&L=" + numLangID;
            return strWebShopFolder + strOutput;
        }
        else if ((strOriginalPath.Contains("category.aspx?categoryid=")))
        {
            // -----------------------------------
            // Category QS Parameterized Link
            // -----------------------------------
            string strParent = strOriginalPath.Substring(strOriginalPath.IndexOf("category.aspx?categoryid=") + 25);
            return strWebShopFolder + "Admin/_ModifyCategory.aspx?CategoryID=" + strParent;
        }
        else if ((strCurrentPath.ToLower().Contains("__p-")))
        {
            // -----------------------------------
            // Product
            // -----------------------------------
            string strOptions = "";
            if (strCurrentPath.ToLower().Contains("?stroptions="))
                strOptions = strCurrentPath.Substring(strCurrentPath.ToLower().IndexOf("?stroptions=") + 12);
            string strParent;
            var intIDstartindex = strCurrentPath.IndexOf("__p-") + 4;
            if (string.IsNullOrEmpty(strOptions))
                strParent = strCurrentPath.Substring(intIDstartindex);
            else
                strParent = strCurrentPath.Substring(intIDstartindex, strCurrentPath.LastIndexOf(".aspx") - intIDstartindex);

            arrKeys = Strings.Split(strParent, "-");
            int intUpper = Information.UBound(arrKeys);
            string strOutputURL;
            if (intUpper == 0)
                strOutputURL = strWebShopFolder + "Admin/_ModifyProduct.aspx?ProductID=" + arrKeys(intUpper).ToString();
            else if (intUpper == 1)
                strOutputURL = strWebShopFolder + "Admin/_ModifyProduct.aspx?ProductID=" + arrKeys(1).ToString() + "&CategoryID=" + arrKeys(0);
            else if (intUpper > 1)
            {
                strParent = arrKeys(0);
                if (intUpper > 2)
                {
                    for (int ctr = 1; ctr <= intUpper - 2; ctr++)
                        strParent += "," + arrKeys(ctr);
                }
                strOutputURL = strWebShopFolder + "Admin/_ModifyProduct.aspx?ProductID=" + arrKeys(intUpper).ToString() + "&CategoryID=" + arrKeys(intUpper - 1) + "&strParent=" + strParent;
            }
            else
                strOutputURL = "";
            strOutputURL += "&L=" + numLangID;
            if (!string.IsNullOrEmpty(strOptions))
                strOutputURL += "&strOptions=" + strOptions;
            return strOutputURL;
        }
        else if ((strOriginalPath.Contains("product.aspx?productid=")))
        {
            // -----------------------------------
            // Product QS Parameterized Link
            // -----------------------------------
            string strParent = strOriginalPath.Substring(strOriginalPath.IndexOf("product.aspx?productid=") + 23);
            return strWebShopFolder + "Admin/_ModifyProduct.aspx?ProductID=" + strParent;
        }
        else if ((strCurrentPath.ToLower().Contains("__n-")))
        {
            // -----------------------------------
            // News
            // -----------------------------------
            string strParent = strCurrentPath.Substring(strCurrentPath.IndexOf("__n-") + 4);
            strParent = System.Convert.ToString(Strings.Replace(strParent, "_", ""));
            return strWebShopFolder + "Admin/_SiteNews.aspx?NewsID=" + strParent;
        }
        else if ((strOriginalPath.Contains("news.aspx?newsid=")))
        {
            // -----------------------------------
            // News QS Parameterized Link
            // -----------------------------------
            string strParent = strOriginalPath.Substring(strOriginalPath.IndexOf("news.aspx?newsid=") + 17);
            return strWebShopFolder + "Admin/_SiteNews.aspx?NewsID=" + strParent;
        }
        else if ((strCurrentPath.ToLower().Contains("/news")))
            // -----------------------------------
            // News Home
            // -----------------------------------
            return strWebShopFolder + "Admin/_SiteNews.aspx";
        else if ((strCurrentPath.ToLower().Contains("/t-")))
        {
            // -----------------------------------
            // CMS Page
            // -----------------------------------
            string strParent = strCurrentPath.Substring(strCurrentPath.IndexOf("/t-") + 3);
            strParent = Strings.Replace(strParent, "/", "");
            return strWebShopFolder + "Admin/_CustomPages.aspx?test=test&strPage=" + strParent;
        }
        else if ((strOriginalPath.Contains("/page.aspx?strpage=")))
        {
            // -----------------------------------
            // CMS Page QS Parameterized Link
            // -----------------------------------
            string strParent = strOriginalPath.Substring(strOriginalPath.IndexOf("/page.aspx?strpage=") + 19);
            return strWebShopFolder + "Admin/_CustomPages.aspx?strPage=" + strParent;
        }
        else if ((strCurrentPath.ToLower().Contains("/default")))
            // -----------------------------------
            // Home Page
            // -----------------------------------
            return strWebShopFolder + "Admin/Default.aspx";
        else if ((strCurrentPath.ToLower().Contains("__k-")))
        {
            // -----------------------------------
            // Knowledgebase
            // -----------------------------------
            string strParent = strCurrentPath.Substring(strCurrentPath.IndexOf("__k-") + 4);
            strParent = Strings.Replace(strParent, "_", "");
            return strWebShopFolder + "Admin/_Knowledgebase.aspx?kb=" + strParent;
        }
        else if (strOriginalPath.Contains("knowledgebase.aspx?kb="))
        {
            // -----------------------------------
            // Knowledgebase QS Parameterized Link
            // -----------------------------------
            string strParent = strOriginalPath.Substring(strOriginalPath.IndexOf("knowledgebase.aspx?kb=") + 22);
            return strWebShopFolder + "Admin/_Knowledgebase.aspx?kb=" + strParent;
        }
        else if ((strCurrentPath.ToLower().Contains("/knowledgebase")))
            // -----------------------------------
            // Knowledgebase home
            // -----------------------------------
            return strWebShopFolder + "Admin/_Knowledgebase.aspx";
        else
            return "";
    }

    internal static string SEORewrite(string strCurrentPath)
    {
        Array arrKeys;
        string strWebShopFolder = "~/";
        string strQueryStrings = "";
        // Extract the language and culture info from the friendly URL
        // Dim strURLCultureInfo As String = Left(Replace(LCase(strCurrentPath), LCase(WebShopURL), ""), 5)
        strCurrentPath = Strings.LCase(Strings.Replace(strCurrentPath, ":80", ""));
        string strWebShopURL;
        string strURLCultureInfo;
        if (InStr(strCurrentPath, WebShopURL))
            strWebShopURL = LCase(WebShopURL());
        else
            strWebShopURL = LCase(WebShopFolder());
        short numLangID;
        if (LanguagesBLL.GetLanguagesCount > 1)
        {
            if (Strings.Len(strWebShopURL) == 0)
                // ' There is no webshopurl (coming from no webshop folder), means we need to
                // '   skip the first character because the strCurrentPath will be similar to
                // '      "/nl-NL/CATEGORY_OR_PRODUCT_NAME", so to read the language do:
                strURLCultureInfo = Strings.Mid(strCurrentPath, 2, 5);
            else
                strURLCultureInfo = Strings.Left(Strings.Mid(strCurrentPath, Strings.InStr(strCurrentPath, strWebShopURL) + Strings.Len(strWebShopURL)), 5);
            numLangID = LanguagesBLL.GetLanguageIDByCulture_s(strURLCultureInfo);
        }
        else
            numLangID = LanguagesBLL.GetDefaultLanguageID;

        if (numLangID == 0)
            numLangID = GetLanguageIDfromSession();

        if (strCurrentPath.Contains(".aspx?"))
        {
            strQueryStrings = "&" + Strings.Mid(strCurrentPath, strCurrentPath.LastIndexOf("?") + 2);
            strCurrentPath = Strings.Left(strCurrentPath, strCurrentPath.LastIndexOf("?"));
        }
        if (Strings.Right(strCurrentPath, 5) == ".aspx")
            strCurrentPath = Strings.Left(strCurrentPath, Strings.Len(strCurrentPath) - 5);
        // If Right(strCurrentPath, 1) = "/" Then strCurrentPath = Left(strCurrentPath, Len(strCurrentPath) - 1)
        if ((strCurrentPath.Contains("__c-")))
        {
            string strParent = strCurrentPath.Substring(strCurrentPath.IndexOf("__c-") + 4);
            arrKeys = Strings.Split(strParent, "-");
            int intUpper = Information.UBound(arrKeys);
            string strOutput;
            if (intUpper == 3)
                strOutput = "Category.aspx?CategoryID=" + arrKeys(intUpper) + "&CPGR=" + arrKeys(1) + "&PPGR=" + arrKeys(2);
            else if (intUpper == 4)
                strOutput = "Category.aspx?CategoryID=" + arrKeys(4) + "&strParent=" + arrKeys(3) + "&CPGR=" + arrKeys(1) + "&PPGR=" + arrKeys(2);
            else if (intUpper > 4)
            {
                strParent = arrKeys(3);
                for (int ctr = 4; ctr <= (intUpper - 1); ctr++)
                    strParent += "," + arrKeys(ctr);
                strOutput = "Category.aspx?CategoryID=" + arrKeys(intUpper).ToString() + "&strParent=" + strParent + "&CPGR=" + arrKeys(1) + "&PPGR=" + arrKeys(2);
            }
            else
                strOutput = "";
            if (arrKeys(0) == "s")
                strOutput += "&T=S";
            strOutput += "&L=" + numLangID;
            return strWebShopFolder + strOutput + strQueryStrings;
        }
        else if ((strCurrentPath.Contains("__p-")))
        {
            string strOptions = "";
            if (strCurrentPath.ToLower().Contains("?stroptions="))
                strOptions = strCurrentPath.Substring(strCurrentPath.ToLower().IndexOf("?stroptions=") + 12);
            string strParent;
            var intIDstartindex = strCurrentPath.IndexOf("__p-") + 4;
            if (string.IsNullOrEmpty(strOptions))
                strParent = strCurrentPath.Substring(intIDstartindex);
            else
                strParent = strCurrentPath.Substring(intIDstartindex, strCurrentPath.LastIndexOf(".aspx") - intIDstartindex);

            arrKeys = Strings.Split(strParent, "-");
            int intUpper = Information.UBound(arrKeys);
            string strOutputURL;
            if (intUpper == 0)
                strOutputURL = strWebShopFolder + "Product.aspx?ProductID=" + arrKeys(intUpper).ToString();
            else if (intUpper == 1)
                strOutputURL = strWebShopFolder + "Product.aspx?ProductID=" + arrKeys(1).ToString() + "&CategoryID=" + arrKeys(0);
            else if (intUpper > 1)
            {
                strParent = arrKeys(0);
                if (intUpper > 2)
                {
                    for (int ctr = 1; ctr <= intUpper - 2; ctr++)
                        strParent += "," + arrKeys(ctr);
                }
                strOutputURL = strWebShopFolder + "Product.aspx?ProductID=" + arrKeys(intUpper).ToString() + "&CategoryID=" + arrKeys(intUpper - 1) + "&strParent=" + strParent;
            }
            else
                strOutputURL = "";
            strOutputURL += "&L=" + numLangID;
            if (!string.IsNullOrEmpty(strOptions))
                strOutputURL += "&strOptions=" + strOptions;
            return strOutputURL + strQueryStrings;
        }
        else if ((strCurrentPath.Contains("__n-")))
        {
            string strParent = strCurrentPath.Substring(strCurrentPath.IndexOf("__n-") + 4);
            strParent = System.Convert.ToString(Strings.Replace(strParent, "_", ""));
            return strWebShopFolder + "News.aspx?NewsID=" + strParent + strQueryStrings;
        }
        else if ((strCurrentPath.Contains("/t-")))
        {
            string strParent = strCurrentPath.Substring(strCurrentPath.IndexOf("/t-") + 3);
            strParent = Strings.Replace(strParent, "/", "");
            return strWebShopFolder + "Page.aspx?strPage=" + strParent + strQueryStrings;
        }
        else if ((strCurrentPath.Contains("__k-")))
        {
            string strParent = strCurrentPath.Substring(strCurrentPath.IndexOf("__k-") + 4);
            strParent = Strings.Replace(strParent, "_", "");
            return strWebShopFolder + "Knowledgebase.aspx?kb=" + strParent + strQueryStrings;
        }
        else
            return "";
    }

    private static string GetCategoryName(int numCategoryID, short numLanguageID, bool blnCheckURLName = false)
    {
        if (numCategoryID == 0)
            return "";
        string strCategoryNameInURL = "";
        if (blnCheckURLName)
        {
            LanguageElementsBLL objLanguageElementsBLL = new LanguageElementsBLL();
            strCategoryNameInURL = objLanguageElementsBLL.GetElementValue(numLanguageID, CkartrisEnumerations.LANG_ELEM_TABLE_TYPE.Categories, CkartrisEnumerations.LANG_ELEM_FIELD_NAME.URLName, numCategoryID);
        }

        if (strCategoryNameInURL.ToLower() == "# -le- #")
        {
            CategoriesBLL objCategoriesBLL = new CategoriesBLL();
            strCategoryNameInURL = objCategoriesBLL.GetNameByCategoryID(numCategoryID, numLanguageID);
        }

        // URL Safe
        return CleanURL(strCategoryNameInURL);
    }

    private static string GetProductName(int numProductID, short numLanguageID, bool blnCheckURLName = false)
    {
        string strProductNameInURL = "";
        if (blnCheckURLName)
        {
            LanguageElementsBLL objLanguageElementsBLL = new LanguageElementsBLL();
            strProductNameInURL = objLanguageElementsBLL.GetElementValue(numLanguageID, CkartrisEnumerations.LANG_ELEM_TABLE_TYPE.Products, CkartrisEnumerations.LANG_ELEM_FIELD_NAME.URLName, numProductID);
        }

        if (strProductNameInURL.ToLower() == "# -le- #" | strProductNameInURL == "")
        {
            ProductsBLL objProductsBLL = new ProductsBLL();
            strProductNameInURL = objProductsBLL.GetNameByProductID(numProductID, numLanguageID);
        }

        return strProductNameInURL;
    }

    private static string GetNewsTitle(int numNewsID, short numLanguageID)
    {
        return StripHTML(NewsBLL.GetNewsTitleByID(numNewsID, numLanguageID));
    }

    private static string GetKnowledgebaseTitle(int numKBID, short numLanguageID)
    {
        return KBBLL.GetKBTitleByID(numLanguageID, numKBID);
    }
}

public class _CategorySiteMapProvider : StaticSiteMapProvider
{
    public enum BackEndPage
    {
        Product,
        Category,
        Page,
        Search
    }

    private bool _isInitialized = false;
    private SiteMapNode _rootNode;

    private string _connectionString;
    private string _navigateUrl;
    private string _idFieldName;

    private const string _cacheDependencyName = "__SiteMapCacheDependency";

    // Database info for SQL Server 7/2000 cache dependency 
    private bool _2005dependency = false;
    // Database info for SQL Server 2005 cache dependency 
    private int _indexID;
    private int _indexTitle;
    private int _indexUrl;
    private int _indexDesc;
    private int _indexRoles;
    private int _indexParent;

    // Private _nodes As New Dictionary(Of Integer, SiteMapNode)(16)
    private readonly object _lock = new object();
    private SiteMapNode _root;

    /// <summary>
    ///     ''' Loads configuration settings from Web configuration file
    ///     ''' </summary>
    public override void Initialize(string name, NameValueCollection attributes)
    {
        if (_isInitialized)
            return;

        base.Initialize(name, attributes);

        // Get database connection string from config file
        string connectionStringName = attributes["connectionStringName"];
        if ((string.IsNullOrEmpty(connectionStringName)))
            throw new Exception("You must provide a connectionStringName attribute");
        ;/* Cannot convert AssignmentStatementSyntax, System.ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
Parameter name: index
   at System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.<>c__DisplayClass83_0.<ConvertArguments>b__0(ArgumentSyntax a, Int32 i)
   at System.Linq.Enumerable.<SelectIterator>d__5`2.MoveNext()
   at System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
   at Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SeparatedList[TNode](IEnumerable`1 nodes)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitAssignmentStatement(AssignmentStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.AssignmentStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 
        _connectionString = WebConfigurationManager.ConnectionStrings(connectionStringName).ConnectionString

 */
        if ((string.IsNullOrEmpty(_connectionString)))
            throw new Exception("Could not find connection String " + connectionStringName);

        // Get navigateUrl from config file
        _navigateUrl = attributes["navigateUrl"];
        if ((string.IsNullOrEmpty(_navigateUrl)))
            throw new Exception("You must provide a navigateUrl attribute");

        // Get idFieldName from config file
        _idFieldName = attributes["idFieldName"];
        if (string.IsNullOrEmpty(_idFieldName))
            _idFieldName = "CategoryID";
        _isInitialized = true;
    }

    /// <summary>
    ///     ''' Retrieve the root node by building the Site Map
    ///     ''' </summary>
    protected override SiteMapNode GetRootNodeCore()
    {
        HttpContext context = HttpContext.Current;
        return BuildSiteMap();
    }

    /// <summary>
    ///     ''' Resets the Site Map by deleting the 
    ///     ''' root node. This causes the BuildSiteMap()
    ///     ''' method to rebuild the Site Map
    ///     ''' </summary>
    public void ResetSiteMap()
    {
        _rootNode = null;
    }

    /// <summary>
    ///     ''' Build the Site Map by retrieving
    ///     ''' records from database table
    ///     ''' </summary>
    public override SiteMapNode BuildSiteMap()
    {
        // Only allow the Site Map to be created by a single thread
        lock (this)
        {
            if (_rootNode == null)
            {
                // Show trace for debugging
                HttpContext context = HttpContext.Current;
                HttpContext.Current.Trace.Warn("Loading back-end category site map from database");

                // Clear current Site Map
                StaticSiteMapProvider.Clear();

                // Load the database data
                DataTable tblSiteMap = GetSiteMapFromDB();
                // Set the root node
                string strCategoryLabel = HttpContext.GetGlobalResourceObject("Kartris", "ContentText_Categories");
                _rootNode = new SiteMapNode(this, "0", "~/Admin/_Category.aspx", strCategoryLabel, strCategoryLabel);

                SiteMapProvider.AddNode(_rootNode);

                // Build the child nodes 
                BuildSiteMapRecurse(tblSiteMap, _rootNode, 0, 0);
            }
            return _rootNode;
        }
    }

    /// <summary>
    ///     ''' Load the contents of the database table
    ///     ''' that contains the Site Map
    ///     ''' </summary>
    private DataTable GetSiteMapFromDB()
    {
        Int16 numLANGID;
        if (string.IsNullOrEmpty(HttpContext.Current.Session.Item["_LANG"]))
            numLANGID = 1;
        else
            numLANGID = System.Convert.ToInt16(HttpContext.Current.Session.Item["_LANG"]);
        CategoriesBLL objCategoriesBLL = new CategoriesBLL();
        return objCategoriesBLL._GetHierarchyByLanguageId(numLANGID);
    }

    /// <summary>
    ///     ''' Recursively build the Site Map from the DataTable
    ///     ''' </summary>
    private void BuildSiteMapRecurse(DataTable siteMapTable, SiteMapNode parentNode, string parentkey, Int64 numSiteID)
    {
        Array arr = Strings.Split(parentNode.Key, ",");
        DataRow[] results = siteMapTable.Select("ParentID=" + arr(Information.UBound(arr)));
        foreach (DataRow row in results)
        {
            parentkey = StripParents(parentkey);

            string url;
            string strParentKey;
            if (numSiteID < row("SUB_ID"))
                numSiteID = row("SUB_ID");
            // numSiteID = row("SUB_ID")  'we start top level with zero, increase as we work through to ensure all sub sites included

            if (Strings.Left(parentkey, 2) == "0,")
                strParentKey = Strings.Mid(parentkey, 3);
            else
                strParentKey = parentkey;
            if (strParentKey == "0")
                strParentKey = "";
            if (strParentKey != "")
                strParentKey = strParentKey + "," + row("parentid");
            else
                strParentKey = row("parentid");

            url = CreateURL(BackEndPage.Category, row("CAT_ID"), numSiteID, numSiteID + "::" + strParentKey);

            // We add in the numsite ID to the key, this is a unique string we want
            // to insert into the sitemap so each URL has a unique key. It's used
            // for setting the breadcrumbtrail correctly.
            SiteMapNode node = new SiteMapNode(this, numSiteID + "::" + strParentKey + "," + row("CAT_ID").ToString(), url, row("title").ToString());
            try
            {
                StaticSiteMapProvider.AddNode(node, parentNode);
            }
            catch (Exception ex)
            {
            }

            BuildSiteMapRecurse(siteMapTable, node, parentNode.Key, numSiteID);
        }
    }

    /// <summary>
    ///     ''' Create URL
    ///     ''' </summary>
    public static string CreateURL(BackEndPage strPageType, Int64 ID, Int64 numSiteID, string strParents = "", int ParentID = 0, int CPagerID = 0, int PPagerID = 0, string strActiveTab = "p")
    {
        int numLangID;
        if (string.IsNullOrEmpty(HttpContext.Current.Session.Item["_LANG"]))
            numLangID = 1;
        else
            numLangID = System.Convert.ToInt32(HttpContext.Current.Session.Item["_LANG"]);
        string strUserCulture;
        if (string.IsNullOrEmpty(HttpContext.Current.Session.Item["KartrisUserCulture"]))
            strUserCulture = "en/";
        else
            strUserCulture = System.Convert.ToString(HttpContext.Current.Session.Item["KartrisUserCulture"]) + "/";

        strParents = StripParents(strParents);

        string strWebShopFolder = KartSettingsManager.GetKartConfig("general.webshopfolder");
        string strURL;
        switch (strPageType)
        {
            case BackEndPage.Category:
                {
                    if (Strings.Left(strParents, 1) == ",")
                        strParents = Strings.Mid(strParents, 2);
                    string strPageName = "~/Admin/_Category.aspx";
                    if (!(strParents == "" | strParents == "0"))
                        strURL = string.Format("{0}?CategoryID={1}&SiteID={2}&strParent={3}", strPageName, ID, numSiteID, numSiteID + "::" + strParents);
                    else if (ParentID == 0)
                        strURL = string.Format("{0}?CategoryID={1}&SiteID={2}", strPageName, ID, numSiteID);
                    else
                        strURL = string.Format("{0}?CategoryID={1}SiteID={2}&strParent={3}", strPageName, ID, numSiteID, numSiteID + "::" + ParentID);
                    if (CPagerID != 0)
                        strURL += "&CPGR=" + CPagerID;
                    if (PPagerID != 0)
                        strURL += "&PPGR=" + PPagerID;
                    if (strActiveTab == "s")
                        strURL += "&T=S";
                    break;
                }

            case BackEndPage.Product:
                {
                    if (Strings.Left(strParents, 1) == ",")
                        strParents = Strings.Mid(strParents, 2);
                    string strPageName = "~/Admin/_ModifyProduct.aspx";
                    strURL = string.Format("{0}?ProductID={1}&SiteID={2}&CategoryID={3}&strParent={4}", strPageName, ID, numSiteID, ParentID, numSiteID + "::" + strParents);
                    if (CPagerID != 0)
                        strURL += "&CPGR=" + CPagerID;
                    if (PPagerID != 0)
                        strURL += "&PPGR=" + PPagerID;
                    if (strActiveTab == "s")
                        strURL += "&T=S";
                    break;
                }

            default:
                {
                    return null;
                }
        }
        return strURL;
    }

    /// <summary>
    ///     ''' Strip the site ID from parents string
    ///     ''' </summary>
    public static string StripParents(string strParents)
    {
        try
        {
            string[] aryParentKey = Strings.Split(strParents, "::");
            strParents = aryParentKey[1];
            if (strParents == "")
                strParents = "0";
        }
        catch (Exception ex)
        {
        }
        return strParents;
    }
}

