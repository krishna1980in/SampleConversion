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
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using kartrisLanguageData;
using kartrisLanguageDataTableAdapters;
using CkartrisDataManipulation;
using System.Web.HttpContext;
using CkartrisFormatErrors;
using CkartrisEnumerations;
using KartSettingsManager;

public class LanguagesBLL
{

    // ' LanguageElementTypeField
    private static LanguageElementTypeFieldsTblAdptr _TypeFieldAdptr = null/* TODO Change to default(_) if this is not a reference type */;

    protected static LanguageElementTypeFieldsTblAdptr TypeFieldAdptr
    {
        get
        {
            _TypeFieldAdptr = new LanguageElementTypeFieldsTblAdptr();
            return _TypeFieldAdptr;
        }
    }

    public static DataTable _GetTypeFieldDetails()
    {
        return TypeFieldAdptr._GetData();
    }

    // ' Languages
    private static LanguagesTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;

    protected static LanguagesTblAdptr Adptr
    {
        get
        {
            _Adptr = new LanguagesTblAdptr();
            return _Adptr;
        }
    }

    /// <summary>
    ///     ''' 'Get Languages' function that handles its own caching methods...just pass true to force-refresh the cache
    ///     ''' </summary>
    ///     ''' <param name="refreshCache"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable GetLanguages(bool refreshCache = false)
    {
        if (refreshCache)
        {
            if (!HttpRuntime.Cache("KartrisFrontLanguagesCache") == null)
                HttpRuntime.Cache.Remove("KartrisFrontLanguagesCache");
            DataTable tblLanguages = Adptr.GetData;
            HttpRuntime.Cache.Add("KartrisFrontLanguagesCache", tblLanguages, null/* TODO Change to default(_) if this is not a reference type */, DateTime.MaxValue, TimeSpan.Zero, Caching.CacheItemPriority.Low, null/* TODO Change to default(_) if this is not a reference type */);
            // were only trying to refresh the cache so return nothing
            return null/* TODO Change to default(_) if this is not a reference type */;
        }
        else
        {
            if (HttpRuntime.Cache("KartrisFrontLanguagesCache") == null)
                GetLanguages(true);
            return (DataTable)HttpRuntime.Cache("KartrisFrontLanguagesCache");
        }
    }

    public static int GetLanguagesCount()
    {
        if (HttpRuntime.Cache("KartrisFrontLanguagesCache") != null)
            return (DataTable)HttpRuntime.Cache("KartrisFrontLanguagesCache").Rows.Count;
        else
            return 0;
    }

    public static DataTable _GetLanguages()
    {
        return Adptr._GetData();
    }

    public static DataRow[] _GetBackendLanguages()
    {
        return GetLanguagesFromCache().Select("LANG_LiveBack = 1");
    }

    public static DataRow[] _GetByLanguageID(byte _LanguageID)
    {
        return GetLanguagesFromCache().Select("LANG_ID=" + _LanguageID);
    }

    public static string GetLanguageFrontName_s(short _LanguageID)
    {
        foreach (DataRow row in GetLanguagesFromCache().Rows)
        {
            if (FixNullFromDB(row("LANG_ID")) == _LanguageID)
                return FixNullFromDB(row("LANG_FrontName"));
        }
        return "";
    }

    public static string GetSkinURLByCulture(string _Language_Culture)
    {
        foreach (DataRow row in GetLanguagesFromCache().Rows)
        {
            if (FixNullFromDB(row("LANG_Culture")) == _Language_Culture)
                return FixNullFromDB(row("LANG_SkinLocation"));
        }
        return "";
    }

    public static byte GetLanguageIDByCulture_s(string _Language_Culture)
    {
        foreach (DataRow row in GetLanguages().Rows)
        {
            if (UCase(FixNullFromDB(row("LANG_Culture"))) == Strings.UCase(_Language_Culture))
                return System.Convert.ToByte(row("LANG_ID"));
        }
        return 0;
    }

    public static byte GetLanguageIDByCultureUI_s(string _Language_Culture)
    {
        foreach (DataRow row in GetLanguages().Rows)
        {
            if (UCase(Left(FixNullFromDB(row("LANG_Culture")), 2)) == Strings.UCase(Strings.Left(_Language_Culture, 2)))
                return System.Convert.ToByte(row("LANG_ID"));
        }
        return 0;
    }

    public static string GetCultureByLanguageID_s(short _Language_ID)
    {
        foreach (DataRow row in GetLanguagesFromCache().Rows)
        {
            if (FixNullFromDB(row("LANG_ID")) == _Language_ID)
                return FixNullFromDB(row("LANG_Culture"));
        }
        return "";
    }

    public static string GetUICultureByLanguageID_s(short _Language_ID)
    {
        foreach (DataRow row in GetLanguagesFromCache().Rows)
        {
            if (FixNullFromDB(row("LANG_ID")) == _Language_ID)
                return FixNullFromDB(row("LANG_UICulture"));
        }
        return "";
    }

    public static byte GetDefaultLanguageID()
    {
        return System.Convert.ToByte(GetKartConfig("frontend.languages.default"));
    }

    public static string GetDateFormat(byte numLangID, char chrType)
    {
        foreach (DataRow row in GetLanguagesFromCache().Rows)
        {
            if (FixNullFromDB(row("LANG_ID")) == numLangID)
            {
                if (chrType == "d")
                    return FixNullFromDB(row("LANG_DateFormat"));
                return FixNullFromDB(row("LANG_DateAndTimeFormat"));
            }
        }
        return "";
    }

    public static string GetEmailToContact(byte numLangID)
    {
        DataRow[] dr = GetLanguagesFromCache.Select("LANG_ID=" + numLangID);
        if (dr.Length == 1)
            return FixNullFromDB(dr[0]("LANG_EmailToContact"));
        return "";
    }

    public static string GetEmailTo(byte numLangID)
    {
        DataRow[] dr = GetLanguagesFromCache.Select("LANG_ID=" + numLangID);
        if (dr.Length == 1)
            return FixNullFromDB(dr[0]("LANG_EmailTo"));
        return "";
    }

    public static string GetEmailFrom(byte numLangID)
    {
        DataRow[] dr = GetLanguagesFromCache.Select("LANG_ID=" + numLangID);
        if (dr.Length == 1)
            return FixNullFromDB(dr[0]("LANG_EmailFrom"));
        return "";
    }

    // We say 'theme' but we're really returned the 
    // skin folder
    public static string GetTheme(byte numLangID)
    {
        DataRow[] dr = GetLanguagesFromCache.Select("LANG_ID=" + numLangID);
        if (dr.Length == 1)
            return FixNullFromDB(dr[0]("LANG_Theme"));
        return "";
    }

    public static string GetMasterPage(byte numLangID)
    {
        DataRow[] dr = GetLanguagesFromCache.Select("LANG_ID=" + numLangID);
        if (dr.Length == 1)
            return FixNullFromDB(dr[0]("LANG_Master"));
        return "";
    }

    public static bool _UpdateLanguage(byte _LanguageID, string _BackName, string _FrontName, string _SkinLocation, bool _LiveFront, bool _LiveBack, string _EmailFrom, string _EmailTo, string _EmailContact, string _DateFormat, string _DateAndTimeFormat, string _Culture, string _UICulture, string _Master, string _Theme, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlClient.SqlConnection sqlConn = new SqlClient.SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateLanguage = sqlConn.CreateCommand;
            cmdUpdateLanguage.CommandText = "_spKartrisLanguages_Update";

            SqlClient.SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateLanguage.CommandType = CommandType.StoredProcedure;

            try
            {
                cmdUpdateLanguage.Parameters.AddWithValue("@LANG_BackName", FixNullToDB(_BackName));
                cmdUpdateLanguage.Parameters.AddWithValue("@LANG_FrontName", FixNullToDB(_FrontName));
                cmdUpdateLanguage.Parameters.AddWithValue("@LANG_SkinLocation", FixNullToDB(_SkinLocation));
                cmdUpdateLanguage.Parameters.AddWithValue("@LANG_LiveFront", _LiveFront);
                cmdUpdateLanguage.Parameters.AddWithValue("@LANG_LiveBack", _LiveBack);
                cmdUpdateLanguage.Parameters.AddWithValue("@LANG_EmailFrom", FixNullToDB(_EmailFrom));
                cmdUpdateLanguage.Parameters.AddWithValue("@LANG_EmailTo", FixNullToDB(_EmailTo));
                cmdUpdateLanguage.Parameters.AddWithValue("@LANG_EmailToContact", FixNullToDB(_EmailContact));
                cmdUpdateLanguage.Parameters.AddWithValue("@LANG_DateFormat", FixNullToDB(_DateFormat));
                cmdUpdateLanguage.Parameters.AddWithValue("@LANG_DateAndTimeFormat", FixNullToDB(_DateAndTimeFormat));
                cmdUpdateLanguage.Parameters.AddWithValue("@LANG_Culture", FixNullToDB(_Culture));
                cmdUpdateLanguage.Parameters.AddWithValue("@LANG_UICulture", FixNullToDB(_UICulture));
                cmdUpdateLanguage.Parameters.AddWithValue("@LANG_Master", FixNullToDB(_Master));
                cmdUpdateLanguage.Parameters.AddWithValue("@LANG_Theme", FixNullToDB(_Theme));
                cmdUpdateLanguage.Parameters.AddWithValue("@Original_LANG_ID", _LanguageID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateLanguage.Transaction = savePoint;

                cmdUpdateLanguage.ExecuteNonQuery();

                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Languages, strMsg, CreateQuery(cmdUpdateLanguage), _BackName, sqlConn, savePoint);

                savePoint.Commit();
                RefreshLanguagesCache();
                return true;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
                if (!savePoint == null)
                    savePoint.Rollback();
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close(); savePoint.Dispose();
                }
            }
        }
        return false;
    }

    public static bool _AddLanguage(string _BackName, string _FrontName, string _SkinLocation, bool _LiveFront, bool _LiveBack, string _EmailFrom, string _EmailTo, string _EmailContact, string _DateFormat, string _DateAndTimeFormat, string _Culture, string _UICulture, string _Master, string _Theme, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlClient.SqlConnection sqlConn = new SqlClient.SqlConnection(strConnString))
        {
            SqlCommand cmdAddLanguage = sqlConn.CreateCommand;
            cmdAddLanguage.CommandText = "_spKartrisLanguages_Add";

            SqlClient.SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddLanguage.CommandType = CommandType.StoredProcedure;

            try
            {
                cmdAddLanguage.Parameters.AddWithValue("@LANG_BackName", FixNullToDB(_BackName));
                cmdAddLanguage.Parameters.AddWithValue("@LANG_FrontName", FixNullToDB(_FrontName));
                cmdAddLanguage.Parameters.AddWithValue("@LANG_SkinLocation", FixNullToDB(_SkinLocation));
                cmdAddLanguage.Parameters.AddWithValue("@LANG_LiveFront", _LiveFront);
                cmdAddLanguage.Parameters.AddWithValue("@LANG_LiveBack", _LiveBack);
                cmdAddLanguage.Parameters.AddWithValue("@LANG_EmailFrom", FixNullToDB(_EmailFrom));
                cmdAddLanguage.Parameters.AddWithValue("@LANG_EmailTo", FixNullToDB(_EmailTo));
                cmdAddLanguage.Parameters.AddWithValue("@LANG_EmailToContact", FixNullToDB(_EmailContact));
                cmdAddLanguage.Parameters.AddWithValue("@LANG_DateFormat", FixNullToDB(_DateFormat));
                cmdAddLanguage.Parameters.AddWithValue("@LANG_DateAndTimeFormat", FixNullToDB(_DateAndTimeFormat));
                cmdAddLanguage.Parameters.AddWithValue("@LANG_Culture", FixNullToDB(_Culture));
                cmdAddLanguage.Parameters.AddWithValue("@LANG_UICulture", FixNullToDB(_UICulture));
                cmdAddLanguage.Parameters.AddWithValue("@LANG_Master", FixNullToDB(_Master));
                cmdAddLanguage.Parameters.AddWithValue("@LANG_Theme", FixNullToDB(_Theme));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddLanguage.Transaction = savePoint;

                cmdAddLanguage.ExecuteNonQuery();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Languages, strMsg, CreateQuery(cmdAddLanguage), _BackName, sqlConn, savePoint);

                savePoint.Commit();
                RefreshLanguagesCache();
                return true;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
                if (!savePoint == null)
                    savePoint.Rollback();
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close(); savePoint.Dispose();
                }
            }
        }
        return false;
    }
}
