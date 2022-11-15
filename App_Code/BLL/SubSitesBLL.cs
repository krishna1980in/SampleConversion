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

using CkartrisDataManipulation;
using CkartrisFormatErrors;
using System.Data;
using System.Data.SqlClient;
using kartrisSubSitesData;
using kartrisSubSitesDataTableAdapters;
using System.Web.HttpContext;
using CkartrisEnumerations;

public class SubSitesBLL
{
    private static SubSitesTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;


    protected static SubSitesTblAdptr Adptr
    {
        get
        {
            _Adptr = new SubSitesTblAdptr();
            return _Adptr;
        }
    }

    /// <summary>
    ///     ''' Function to return subsite ID.
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public static int ViewingSubSite(int SUB_ID)
    {
        // We will return 0 if not viewing a subsite, otherwise we'll return the site ID

        string strCurrentDomain = "";
        var strMainDomainConfigWebShopURL = "";

        if (SUB_ID > 0)
            // We are in preview mode
            return SUB_ID;
        else
        {
            // We need to look at URL and see if it matches the webshopURL config setting
            strCurrentDomain = Strings.Replace(Strings.Replace(System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToLower(), "https://", ""), "http://", "");
            strCurrentDomain = Strings.Left(strCurrentDomain, strCurrentDomain.IndexOf("/"));
            strMainDomainConfigWebShopURL = Replace(Replace(KartSettingsManager.GetKartConfig("general.webshopurl").ToLower(), "http://", ""), "/", "");

            // Now we should have two domains
            // If they match, we're looking at the main site URL and since
            // no SUB_ID was found, we're not previewing a subsite. So we can
            // basically return 0.
            if (strCurrentDomain == strMainDomainConfigWebShopURL)
                return 0;
            else
            {
                // Different URLs, let's look up which subsite this is
                DataTable tblSubSitesList = null;
                tblSubSitesList = SubSitesBLL.GetSubSites();
                foreach (var drwSubSite in tblSubSitesList.Rows)
                {
                    if (drwSubSite("SUB_Domain") == strCurrentDomain)
                        return drwSubSite("SUB_ID");
                }
            }
        }
        return 0;
    }

    /// <summary>
    ///     ''' Get all subsites
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public static DataTable GetSubSites()
    {
        return Adptr.GetData();
    }

    /// <summary>
    ///     ''' Get a particular sub site by ID
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public static DataTable GetSubSiteByID(long SUB_ID)
    {
        return Adptr._GetSubSiteByID(SUB_ID);
    }

    /// <summary>
    ///     ''' Update sub site
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public static int _Update(int SUB_ID, string SUB_Name, string SUB_Domain, int SUB_BaseCategoryID, string SUB_Skin, string SUB_Notes, bool SUB_Live)
    {
        try
        {
            Nullable<DateTime> dtNull = default(Date?);
            string strRandomSalt = Membership.GeneratePassword(20, 0);
            return Adptr._Update(SUB_ID, SUB_Name, SUB_Domain, SUB_BaseCategoryID, SUB_Skin, SUB_Notes, SUB_Live);
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
            return default(Integer);
        }
    }

    /// <summary>
    ///     ''' Insert new sub site
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public static int _Add(string SUB_Name, string SUB_Domain, int SUB_BaseCategoryID, string SUB_Skin, string SUB_Notes, bool SUB_Live)
    {
        try
        {
            Nullable<DateTime> dtNull = default(Date?);
            string strRandomSalt = Membership.GeneratePassword(20, 0);
            return Adptr._Add(SUB_Name, SUB_Domain, SUB_BaseCategoryID, SUB_Skin, SUB_Notes, SUB_Live);
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
            return default(Integer);
        }
    }
}
