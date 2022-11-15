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

using kartrisKBDataTableAdapters;
using System.Web.HttpContext;
using CkartrisEnumerations;
using CkartrisFormatErrors;

public class KBBLL
{
    private static KnowledgeBaseTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;

    protected static KnowledgeBaseTblAdptr Adptr
    {
        get
        {
            _Adptr = new KnowledgeBaseTblAdptr();
            return _Adptr;
        }
    }

    public static DataTable _GetKB(byte pLanguageID)
    {
        return Adptr._GetData(pLanguageID);
    }

    public static DataTable _GetKBByID(byte pLanguageID, int pKBID)
    {
        return Adptr._GetByID(pLanguageID, pKBID);
    }

    public static DataTable GetKBByID(short numLanguageID, int numKBID)
    {
        return Adptr.GetByID(numLanguageID, numKBID);
    }

    public static string GetKBTitleByID(short numLanguageID, int numKBID)
    {
        return Adptr.GetTitleByID(numLanguageID, numKBID);
    }

    public static DataTable GetKB(short numLanguageID)
    {
        return Adptr.GetData(numLanguageID);
    }

    public static DataTable Search(string strKeywordList, short numLanguageID)
    {
        return Adptr.Search(strKeywordList, numLanguageID);
    }

    public static bool _AddKB(DataTable tblElements, DateTime datCreated, DateTime datUpdated, bool blnKBLive, string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddKB = sqlConn.CreateCommand;
            cmdAddKB.CommandText = "_spKartrisKnowledgeBase_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddKB.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdAddKB.Parameters.AddWithValue("@NowOffset_Created", datCreated);
                cmdAddKB.Parameters.AddWithValue("@NowOffset_Updated", datUpdated);
                cmdAddKB.Parameters.AddWithValue("@KB_Live", blnKBLive);
                cmdAddKB.Parameters.AddWithValue("@KB_NewID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddKB.Transaction = savePoint;

                cmdAddKB.ExecuteNonQuery();

                if (cmdAddKB.Parameters("@KB_NewID").Value == null || cmdAddKB.Parameters("@KB_NewID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                int intNewKBID = cmdAddKB.Parameters("@KB_NewID").Value;
                if (!LanguageElementsBLL._AddLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.KnowledgeBase, intNewKBID, sqlConn, savePoint))
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

    public static bool _UpdateKB(DataTable tblElements, int numKBID, DateTime datCreated, DateTime datUpdated, bool blnKBLive, string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateKB = sqlConn.CreateCommand;
            cmdUpdateKB.CommandText = "_spKartrisKnowledgeBase_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateKB.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateKB.Parameters.AddWithValue("@KB_ID", numKBID);
                cmdUpdateKB.Parameters.AddWithValue("@KB_Live", blnKBLive);
                cmdUpdateKB.Parameters.AddWithValue("@NowOffset_Created", datCreated);
                cmdUpdateKB.Parameters.AddWithValue("@NowOffset_Updated", datUpdated);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateKB.Transaction = savePoint;

                cmdUpdateKB.ExecuteNonQuery();

                if (!LanguageElementsBLL._UpdateLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.KnowledgeBase, numKBID, sqlConn, savePoint))
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

    public static bool _DeleteKB(int numKBID, string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdDeleteKB = sqlConn.CreateCommand;
            cmdDeleteKB.CommandText = "_spKartrisKnowledgeBase_Delete";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdDeleteKB.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdDeleteKB.Parameters.AddWithValue("@KB_ID", numKBID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdDeleteKB.Transaction = savePoint;

                cmdDeleteKB.ExecuteNonQuery();

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
