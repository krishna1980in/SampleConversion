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
using System.Web.HttpContext;
using kartrisLanguageData;
using kartrisLanguageDataTableAdapters;
using CkartrisFormatErrors;
using CkartrisEnumerations;

public class LanguageStringsBLL
{
    private static LanguageStringsTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;

    protected static LanguageStringsTblAdptr Adptr
    {
        get
        {
            // If _Adptr Is Nothing Then
            _Adptr = new LanguageStringsTblAdptr();
            // End If
            return _Adptr;
        }
    }

    public static LanguageStringsDataTable GetLanguageStringsByID(short _LanguageID)
    {
        return Adptr.GetDataByLanguageID(_LanguageID);
    }

    public static LanguageStringsDataTable GetLanguageStringsByClassName(short _LanguageID, string _ClassName)
    {
        return Adptr.GetDataByClassName(_LanguageID, _ClassName);
    }

    public static LanguageStringsDataTable GetLanguageStringsByVirtualPath(short _LanguageID, string _VirtualPath)
    {
        return Adptr.GetDataByVirtualPath(_LanguageID, _VirtualPath);
    }

    public static DataTable _GetByID(byte pLanguageID, char pFrontBack, string pLSName)
    {
        return Adptr._GetByID(pFrontBack, pLanguageID, pLSName);
    }

    public static DataTable _Search(string pSearchBy, string pSearchKey, string pFrontBack, byte pLanguageID)
    {
        return Adptr._Search(pSearchBy, pSearchKey, pLanguageID, pFrontBack);
    }

    public static DataTable _SearchForUpdate(string pSearchBy, string pSearchKey, string pFrontBack, byte pDefaultLanguageID, byte pLanguageID)
    {
        return Adptr._SearchForUpdate(pSearchBy, pSearchKey, pFrontBack, pDefaultLanguageID, pLanguageID);
    }

    public static DataTable _GetTotalsPerLanguage()
    {
        return Adptr.GetTotalsPerLanguage();
    }

    public static bool _FixLanguageStrings(byte numSourceLanguageID, byte numDistinationLanguageID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisLanguageStrings_FixMissingStrings";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@SourceLanguage", FixNullToDB(numSourceLanguageID, "i"));
                cmd.Parameters.AddWithValue("@DistinationLanguage", FixNullToDB(numDistinationLanguageID, "i"));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                cmd.ExecuteNonQuery();

                savePoint.Commit();
                sqlConn.Close();
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

    public static bool _AddLanguageString(short _LanguageID, string _FrontBack, string _Name, string _Value, string _Description, string _DefaultValue, string _VirtualPath, string _ClassName, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlClient.SqlConnection sqlConn = new SqlClient.SqlConnection(strConnString)) // ', cmdAddLanguageString As New SqlClient.SqlCommand("_spKartrisLanguageStrings_Add", sqlConn)
        {
            SqlCommand cmdAddLanguageString = sqlConn.CreateCommand;
            cmdAddLanguageString.CommandText = "_spKartrisLanguageStrings_Add";

            SqlClient.SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddLanguageString.CommandType = CommandType.StoredProcedure;

            try
            {
                cmdAddLanguageString.Parameters.AddWithValue("@LS_LangID", _LanguageID);
                cmdAddLanguageString.Parameters.AddWithValue("@LS_FrontBack", FixNullToDB(_FrontBack));
                cmdAddLanguageString.Parameters.AddWithValue("@LS_Name", Trim(FixNullToDB(_Name)));
                cmdAddLanguageString.Parameters.AddWithValue("@LS_Value", FixNullToDB(_Value));
                cmdAddLanguageString.Parameters.AddWithValue("@LS_Description", FixNullToDB(_Description));
                cmdAddLanguageString.Parameters.AddWithValue("@LS_VersionAdded", KARTRIS_VERSION);
                cmdAddLanguageString.Parameters.AddWithValue("@LS_DefaultValue", FixNullToDB(_DefaultValue));
                cmdAddLanguageString.Parameters.AddWithValue("@LS_VirtualPath", FixNullToDB(_VirtualPath));
                cmdAddLanguageString.Parameters.AddWithValue("@LS_ClassName", FixNullToDB(_ClassName));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddLanguageString.Transaction = savePoint;

                cmdAddLanguageString.ExecuteNonQuery();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.SiteText, strMsg, CreateQuery(cmdAddLanguageString), _Name, sqlConn, savePoint);

                savePoint.Commit();
                sqlConn.Close();

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

    public static bool _UpdateLanguageString(short _LanguageID, string _FrontBack, string _Name, string _Value, string _Description, string _DefaultValue, string _VirtualPath, string _ClassName, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlClient.SqlConnection sqlConn = new SqlClient.SqlConnection(strConnString)) // ', cmdUpdateLanguageString As New SqlClient.SqlCommand("_spKartrisLanguageStrings_Update", sqlConn)
        {
            SqlCommand cmdUpdateLanguageString = sqlConn.CreateCommand;
            cmdUpdateLanguageString.CommandText = "_spKartrisLanguageStrings_Update";

            SqlClient.SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateLanguageString.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateLanguageString.Parameters.AddWithValue("@Original_LS_LanguageID", _LanguageID);
                cmdUpdateLanguageString.Parameters.AddWithValue("@Original_LS_FrontBack", FixNullToDB(_FrontBack));
                cmdUpdateLanguageString.Parameters.AddWithValue("@Original_LS_Name", FixNullToDB(_Name));
                cmdUpdateLanguageString.Parameters.AddWithValue("@LS_Value", FixNullToDB(_Value));
                cmdUpdateLanguageString.Parameters.AddWithValue("@LS_Description", FixNullToDB(_Description));
                cmdUpdateLanguageString.Parameters.AddWithValue("@LS_DefaultValue", FixNullToDB(_DefaultValue));
                cmdUpdateLanguageString.Parameters.AddWithValue("@LS_VirtualPath", FixNullToDB(_VirtualPath));
                cmdUpdateLanguageString.Parameters.AddWithValue("@LS_ClassName", FixNullToDB(_ClassName));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateLanguageString.Transaction = savePoint;

                cmdUpdateLanguageString.ExecuteNonQuery();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.SiteText, strMsg, CreateQuery(cmdUpdateLanguageString), _Name, sqlConn, savePoint);

                savePoint.Commit();
                sqlConn.Close();

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

    public static bool _TranslateLanguageString(char chrFrontBack, string strName, string strValue, string strDescription, byte numLanguageID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlClient.SqlConnection sqlConn = new SqlClient.SqlConnection(strConnString)) // ', cmdTranslateLS As New SqlClient.SqlCommand("_spKartrisLanguageStrings_Translate", sqlConn)
        {
            SqlCommand cmdTranslateLS = sqlConn.CreateCommand;
            cmdTranslateLS.CommandText = "_spKartrisLanguageStrings_Translate";

            SqlClient.SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdTranslateLS.CommandType = CommandType.StoredProcedure;

            try
            {
                cmdTranslateLS.Parameters.AddWithValue("@LanguageID", numLanguageID);
                cmdTranslateLS.Parameters.AddWithValue("@FrontBack", FixNullToDB(chrFrontBack, "c"));
                cmdTranslateLS.Parameters.AddWithValue("@Name", FixNullToDB(strName));
                cmdTranslateLS.Parameters.AddWithValue("@Value", FixNullToDB(strValue));
                cmdTranslateLS.Parameters.AddWithValue("@Description", FixNullToDB(strDescription));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdTranslateLS.Transaction = savePoint;

                cmdTranslateLS.ExecuteNonQuery();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.SiteText, strMsg, CreateQuery(cmdTranslateLS), strName, sqlConn, savePoint);

                savePoint.Commit();
                sqlConn.Close();

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

    /// <summary>
    ///     ''' Pull out a language string by specific language.
    ///     ''' We use this in the back end for order update mails, 
    ///     ''' for example, where the admin user might be working
    ///     ''' in English, but the order was made in German so we
    ///     ''' need to format order updates with German text.
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public static string _GetLanguageStringByNameAndLanguageID(byte pLanguageID, char pFrontBack, string pLSName)
    {
        DataTable dtbLanguageStrings = Adptr._GetByID(pFrontBack, pLanguageID, pLSName);
        try
        {
            return dtbLanguageStrings.Rows(0).Item("LS_Value");
        }
        catch (Exception ex)
        {
            return "string not found - check your code";
        }
    }
}
