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

partial class UserControls_Skin_CategoryMenu : System.Web.UI.UserControl
{
    private long _lngCategoryID = 0;
    private string _strParent = "";
    private string _strInitialDropdownText = "-";

    /// <summary>
    ///     ''' Get or set the root category for this control 
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public long RootCategoryID
    {
        get
        {
            return _lngCategoryID;
        }
        set
        {
            _lngCategoryID = value;
        }
    }
    /// <summary>
    ///     ''' Get or set the category parent string - QS: strParent
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public string CategoryParentString
    {
        get
        {
            return _strParent;
        }
        set
        {
            _strParent = value;
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        menCategory.MaximumDynamicDisplayLevels = System.Convert.ToInt32(KartSettingsManager.GetKartConfig("frontend.display.categorymenu.levels")) - 1;

        // Set starting node of sitemap
        // This allows the possibility of having two
        // or more menus, each starting from a top
        // level category.

        // From Kartris v3.0 onwards, we need multi-site support
        // This means the category control can be doing double-duty, 
        // both for the main site skin, but also subsites which will
        // have their categories with a separate root category.

        // First, are we viewing a subsite?
        int numSubSiteID = SubSitesBLL.ViewingSubSite(System.Web.UI.UserControl.Session["SUB_ID"]); // Return 0 if default skin
        if (numSubSiteID > 0)
        {
            var tblSubSites = SubSitesBLL.GetSubSiteByID(numSubSiteID);
            _lngCategoryID = tblSubSites.Rows.Item(0).Item("SUB_BaseCategoryID");
        }

        if (_lngCategoryID > 0)
        {
            SiteMapNode nodeCategory = SiteMap.Providers("CategorySiteMapProvider").FindSiteMapNodeFromKey("0-" + CkartrisBLL.GetLanguageIDfromSession + "," + Interaction.IIf(_strParent != "", _strParent + ",", "") + _lngCategoryID);
            if (nodeCategory != null)
                srcSiteMap.StartingNodeUrl = nodeCategory.Url;
        }

        // Handles caching
        String[] dependencyKey = new String[1] { };
        dependencyKey[0] = "CategoryMenuKey";
        BasePartialCachingControl pcc = System.Web.UI.Control.Parent as BasePartialCachingControl;
        pcc.Dependency = new CacheDependency(null, dependencyKey);
    }

    /// <summary>
    ///     ''' CSS Fold-out menu - on menu item bound
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    protected void menCategory_MenuItemDataBound(object sender, System.Web.UI.WebControls.MenuEventArgs e)
    {
        int intCategory_CGID = System.Convert.ToInt32((SiteMapNode)e.Item.DataItem("CG_ID"));
        if (intCategory_CGID > 0)
        {
            bool blnRemove = false;
            if (System.Web.UI.Control.Page.User.Identity.IsAuthenticated)
            {
                try
                {
                    if ((PageBaseClass)System.Web.UI.Control.Page.CurrentLoggedUser.CustomerGroupID != intCategory_CGID)
                        blnRemove = true;
                }
                catch (Exception ex)
                {
                    blnRemove = true;
                }
            }
            else
                blnRemove = true;

            if (blnRemove)
            {
                if (e.Item.Parent != null)
                    e.Item.Parent.ChildItems.Remove(e.Item);
                else
                    menCategory.Items.Remove(e.Item);
            }
        }
    }
}
