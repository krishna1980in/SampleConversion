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
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using kartrisPagesDataTableAdapters;
using System.Web.HttpContext;
using CkartrisEnumerations;
using CkartrisFormatErrors;

public class PagesBLL
{
    private static PagesTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;

    protected static PagesTblAdptr Adptr
    {
        get
        {
            _Adptr = new PagesTblAdptr();
            return _Adptr;
        }
    }

    public static DataTable _GetPages(byte pLanguageID)
    {
        return Adptr._GetData(pLanguageID);
    }

    public static DataTable _GetPageByID(byte pLanguageID, int pPageID)
    {
        return Adptr._GetByID(pLanguageID, pPageID);
    }

    public static DataTable _GetAllNames()
    {
        return Adptr._GetNames();
    }

    public static DataTable GetPageByName(short numLanguageID, string strPageName)
    {
        return Adptr.GetByName(numLanguageID, strPageName);
    }

    public static bool _AddPage(DataTable tblElements, string strPageName, short numParentID, bool blnPageLive, string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddPage = sqlConn.CreateCommand;
            cmdAddPage.CommandText = "_spKartrisPages_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddPage.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdAddPage.Parameters.AddWithValue("@Page_Name", strPageName);
                cmdAddPage.Parameters.AddWithValue("@Page_ParentID", numParentID);
                cmdAddPage.Parameters.AddWithValue("@Page_Live", blnPageLive);
                cmdAddPage.Parameters.AddWithValue("@NowOffset", CkartrisDisplayFunctions.NowOffset);
                cmdAddPage.Parameters.AddWithValue("@Page_NewID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddPage.Transaction = savePoint;

                cmdAddPage.ExecuteNonQuery();

                if (cmdAddPage.Parameters("@Page_NewID").Value == null || cmdAddPage.Parameters("@Page_NewID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                int intNewPageID = cmdAddPage.Parameters("@Page_NewID").Value;
                if (!LanguageElementsBLL._AddLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.Pages, intNewPageID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                savePoint.Commit();
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

    public static bool _UpdatePage(DataTable tblElements, int numPageID, string strPageName, short numParentID, bool blnPageLive, string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdatePage = sqlConn.CreateCommand;
            cmdUpdatePage.CommandText = "_spKartrisPages_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdatePage.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdatePage.Parameters.AddWithValue("@Page_ID", numPageID);
                cmdUpdatePage.Parameters.AddWithValue("@Page_Name", strPageName);
                cmdUpdatePage.Parameters.AddWithValue("@Page_ParentID", numParentID);
                cmdUpdatePage.Parameters.AddWithValue("@Page_Live", blnPageLive);
                cmdUpdatePage.Parameters.AddWithValue("@NowOffset", CkartrisDisplayFunctions.NowOffset);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdatePage.Transaction = savePoint;

                cmdUpdatePage.ExecuteNonQuery();

                if (!LanguageElementsBLL._UpdateLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.Pages, numPageID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                savePoint.Commit();
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

    public static bool _DeletePage(int numPageID, string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdDeletePage = sqlConn.CreateCommand;
            cmdDeletePage.CommandText = "_spKartrisPages_Delete";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdDeletePage.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdDeletePage.Parameters.AddWithValue("@Page_ID", numPageID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdDeletePage.Transaction = savePoint;

                cmdDeletePage.ExecuteNonQuery();

                savePoint.Commit();
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
}
