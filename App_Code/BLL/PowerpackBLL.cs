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

using kartrisAttributesData;
using kartrisAttributesDataTableAdapters;
using System.Web.HttpContext;
using CkartrisEnumerations;
using CkartrisDataManipulation;
using CkartrisFormatErrors;

public class PowerpackBLL
{
    public void LoadCategoryFilters(int intCategoryID, HttpRequest Request, ref XmlDocument xmlDoc, ref string[] arrSelectedValues, short numCurrencyID, ref PlaceHolder phdCategoryFilters, ref PlaceHolder phdPriceRange, ref DropDownList ddlPriceRange, ref TextBox txtFromPrice, ref TextBox txtToPrice, ref Literal litFromSymbol, ref Literal litToSymbol, ref PlaceHolder phdCustomPrice, ref TextBox txtSearch, ref DropDownList ddlOrderBy, ref PlaceHolder phdAttributes, ref Repeater rptAttributes)
    {
        phdCategoryFilters.Visible = false;
    }

    // This function is used to check if there are any filters active
    // In the default Kartris, without powerpack, this always returns false
    public bool CategoryHasFilters(int intCategoryID)
    {
        return false;
    }

    public void BoundRepeaterAttributeItem(XmlDocument xmlDoc, string[] arrSelectedValues, ref RepeaterItem itm)
    {
    }
    public DataTable GetFilteredProductsByCategory(HttpRequest Request, int _CategoryID, short _LanguageID, short _PageIndx, short _RowsPerPage, short _CGroupID, ref int _TotalNoOfProducts)
    {
        return null/* TODO Change to default(_) if this is not a reference type */;
    }
    public static void GoToFilterURL(HttpRequest Request, string strFromPrice, string strToPrice, string strKeywords, string strAttributeValues, string strOrderBy)
    {
    }
    public static bool _IsFiltersEnabled()
    {
        return false;
    }

    public static bool _IsSearchSuggestEnabled()
    {
        return false;
    }

    public string _GetFilterXMLByCategory(int numCategoryID)
    {
        return "Not Enabled";
    }
    public string _GenerateFilterXML(int numCategoryID, byte numLanguageID)
    {
        return null;
    }
    public int _GetFilterObjectID()
    {
        return 0;
    }
    private DataTable _GetAttributesAndValues(int _CategoryID, short _LanguageID)
    {
        return null/* TODO Change to default(_) if this is not a reference type */;
    }
    private int _GetMaxPriceByCategory_s(int _CategoryID, short _LanguageID)
    {
        return default(Integer);
    }
}
