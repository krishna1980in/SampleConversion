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

partial class UserControls_Skin_CategoryMenuDropDownSelect : System.Web.UI.UserControl
{
    private long _lngCategoryID = 0;
    private string _strInitialDropdownText = "-";

    /// <summary>
    ///     ''' Get or set the root category for this control - must be one of the TOP level categories
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
    ///     ''' Set the initial text on the dropdown/select menu
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public string DropdownText
    {
        get
        {
            return _strInitialDropdownText;
        }
        set
        {
            _strInitialDropdownText = value;
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        ddlCategories.Attributes.Add("onChange", "javascript:" + ddlCategories.ClientID + "_CategorySelected();");
        ddlCategories.Items(0).Text = _strInitialDropdownText;

        // Set starting node of sitemap
        // This allows the possibility of having two
        // or more menus, each starting from a top
        // level category.
        if (_lngCategoryID > 0)
        {
            SiteMapNode nodeCategory = SiteMap.Providers("CategorySiteMapProvider").FindSiteMapNodeFromKey("0-" + CkartrisBLL.GetLanguageIDfromSession + "," + _lngCategoryID);
            if (nodeCategory != null)
                srcSiteMap.StartingNodeUrl = nodeCategory.Url;
        }

        // Handles caching
        String[] dependencyKey = new String[1] { };
        dependencyKey[0] = "CategoryMenuKey";
        BasePartialCachingControl pcc = System.Web.UI.Control.Parent as BasePartialCachingControl;
        pcc.Dependency = new CacheDependency(null, dependencyKey);
    }
}
