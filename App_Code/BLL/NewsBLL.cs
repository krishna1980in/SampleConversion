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
using CkartrisDisplayFunctions;
using CkartrisEnumerations;
using kartrisNewsDataTableAdapters;
using CkartrisFormatErrors;
using KartSettingsManager;

public class NewsBLL
{
    private static NewsTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;

    protected static NewsTblAdptr Adptr
    {
        get
        {
            _Adptr = new NewsTblAdptr();
            return _Adptr;
        }
    }

    public static DataTable _GetNewsForCache()
    {
        return Adptr._GetDataForCache();
    }

    public static DataView _GetSummaryNews(byte numLanguageID)
    {
        DataRow[] drwNews = GetSiteNewsFromCache.Select("LANG_ID =" + numLanguageID);
        using (DataTable tblNews = new DataTable())
        {
            tblNews.Columns.Add(new DataColumn("N_ID", Type.GetType("System.Int32")));
            tblNews.Columns.Add(new DataColumn("N_Name", Type.GetType("System.String")));
            tblNews.Columns.Add(new DataColumn("N_DateCreated", Type.GetType("System.DateTime")));
            tblNews.Columns.Add(new DataColumn("N_LastUpdated", Type.GetType("System.DateTime")));

            for (int i = 0; i <= drwNews.Length - 1; i++)
                tblNews.Rows.Add(drwNews[i]("N_ID"), drwNews[i]("N_Name"), FixNullFromDB(drwNews[i]("N_DateCreated")), FixNullFromDB(drwNews[i]("N_LastUpdated")));

            DataView dvwNews;
            dvwNews = tblNews.DefaultView;
            dvwNews.Sort = "N_LastUpdated DESC, N_ID ASC";

            return dvwNews;
        }
    }

    public static DataTable GetLatestNews(byte numLanguageID, short numNoOfRecords = -1)
    {
        DataRow[] drwNews = GetSiteNewsFromCache.Select("LANG_ID =" + numLanguageID);
        using (DataTable tblLatestNews = new DataTable())
        {
            tblLatestNews.Columns.Add(new DataColumn("N_ID", Type.GetType("System.Int32")));
            tblLatestNews.Columns.Add(new DataColumn("N_Name", Type.GetType("System.String")));
            tblLatestNews.Columns.Add(new DataColumn("N_Strapline", Type.GetType("System.String")));
            tblLatestNews.Columns.Add(new DataColumn("N_Text", Type.GetType("System.String")));
            tblLatestNews.Columns.Add(new DataColumn("N_DateCreated", Type.GetType("System.DateTime")));

            for (int i = 0; i <= drwNews.Length - 1; i++)
            {
                // Note we pull out 500 chars for summary, will likely be truncated later
                // but we need to have enough to ensure still have some text after HTML
                // is stripped out
                tblLatestNews.Rows.Add(drwNews[i]("N_ID"), drwNews[i]("N_Name"), Left(FixNullFromDB(drwNews[i]("N_Strapline")), 500), Left(FixNullFromDB(drwNews[i]("N_Text")), 500), FixNullFromDB(drwNews[i]("N_DateCreated")));
                if (tblLatestNews.Rows.Count == numNoOfRecords)
                    break;
            }
            return tblLatestNews;
        }
    }

    public static string GetNewsTitleByID(int numID, byte numLanguageID)
    {
        DataRow[] drwNews = GetSiteNewsFromCache.Select("LANG_ID =" + numLanguageID + " AND N_ID=" + numID);
        if (drwNews.Length != 1)
            return null;
        return System.Convert.ToString(drwNews[0]("N_Name"));
    }

    public static DataTable GetByID(byte numLanguageID, int numID)
    {
        DataRow[] drwNews = GetSiteNewsFromCache.Select("LANG_ID =" + numLanguageID + " AND N_ID=" + numID);
        if (drwNews.Length != 1)
            return null/* TODO Change to default(_) if this is not a reference type */;
        using (DataTable tblNews = new DataTable())
        {
            tblNews.Columns.Add(new DataColumn("N_ID", Type.GetType("System.Int32")));
            tblNews.Columns.Add(new DataColumn("N_Name", Type.GetType("System.String")));
            tblNews.Columns.Add(new DataColumn("N_Text", Type.GetType("System.String")));
            tblNews.Columns.Add(new DataColumn("N_StrapLine", Type.GetType("System.String")));
            tblNews.Columns.Add(new DataColumn("N_DateCreated", Type.GetType("System.DateTime")));

            tblNews.Rows.Add(drwNews[0]("N_ID"), drwNews[0]("N_Name"), FixNullFromDB(drwNews[0]("N_Text")), FixNullFromDB(drwNews[0]("N_StrapLine")), FixNullFromDB(drwNews[0]("N_DateCreated")));

            return tblNews;
        }
    }

    public static bool _AddNews(DataTable tblElements, DateTime datCreation, string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddNews = sqlConn.CreateCommand;
            cmdAddNews.CommandText = "_spKartrisNews_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddNews.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdAddNews.Parameters.AddWithValue("@NowOffset", datCreation);
                cmdAddNews.Parameters.AddWithValue("@N_NewID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddNews.Transaction = savePoint;

                cmdAddNews.ExecuteNonQuery();

                if (cmdAddNews.Parameters("@N_NewID").Value == null || cmdAddNews.Parameters("@N_NewID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                int intNewNewsID = cmdAddNews.Parameters("@N_NewID").Value;
                if (!LanguageElementsBLL._AddLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.News, intNewNewsID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                savePoint.Commit();
                sqlConn.Close();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");

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

    public static bool _UpdateNews(DataTable tblElements, DateTime datCreation, int numNewsID, string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateNews = sqlConn.CreateCommand;
            cmdUpdateNews.CommandText = "_spKartrisNews_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateNews.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateNews.Parameters.AddWithValue("@N_ID", numNewsID);
                cmdUpdateNews.Parameters.AddWithValue("@N_DateCreated", datCreation);
                cmdUpdateNews.Parameters.AddWithValue("@NowOffset", CkartrisDisplayFunctions.NowOffset);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateNews.Transaction = savePoint;

                cmdUpdateNews.ExecuteNonQuery();

                if (!LanguageElementsBLL._UpdateLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.News, numNewsID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                savePoint.Commit();
                sqlConn.Close();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");

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

    public static bool _DeleteNews(int numNewsID, string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdDeleteNews = sqlConn.CreateCommand;
            cmdDeleteNews.CommandText = "_spKartrisNews_Delete";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdDeleteNews.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdDeleteNews.Parameters.AddWithValue("@N_ID", numNewsID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdDeleteNews.Transaction = savePoint;

                cmdDeleteNews.ExecuteNonQuery();

                savePoint.Commit();
                sqlConn.Close();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");

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
                    sqlConn.Close();
            }
        }

        return false;
    }
}
