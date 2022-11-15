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

using kartrisExportData;
using kartrisExportDataTableAdapters;
using CkartrisDisplayFunctions;
using CkartrisFormatErrors;
using CkartrisDataManipulation;
using System.Web.HttpContext;

public class ExportBLL
{
    private static SavedExportsTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;
    protected static SavedExportsTblAdptr Adptr
    {
        get
        {
            _Adptr = new SavedExportsTblAdptr();
            return _Adptr;
        }
    }

    public static DataTable _GetSavedExports()
    {
        return Adptr._GetData();
    }

    public static DataTable _GetSavedExport(long numExportID)
    {
        return Adptr._GetByID(numExportID);
    }

    public static bool _AddSavedExport(string strName, string strDetails, int numFieldDelimiter, int numStringDelimiter, string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddExport = sqlConn.CreateCommand;
            cmdAddExport.CommandText = "_spKartrisSavedExports_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddExport.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdAddExport.Parameters.AddWithValue("@Name", strName);
                cmdAddExport.Parameters.AddWithValue("@DateCreated", NowOffset);
                cmdAddExport.Parameters.AddWithValue("@Details", strDetails);
                cmdAddExport.Parameters.AddWithValue("@FieldDelimiter", numFieldDelimiter);
                cmdAddExport.Parameters.AddWithValue("@StringDelimiter", numStringDelimiter);
                cmdAddExport.Parameters.AddWithValue("@New_ID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddExport.Transaction = savePoint;

                cmdAddExport.ExecuteNonQuery();

                if (cmdAddExport.Parameters("@New_ID").Value == null || cmdAddExport.Parameters("@New_ID").Value == DBNull.Value)
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
    public static bool _UpdateSavedExport(long numExportID, string strName, string strDetails, int numFieldDelimiter, int numStringDelimiter, string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateExport = sqlConn.CreateCommand;
            cmdUpdateExport.CommandText = "_spKartrisSavedExports_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateExport.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateExport.Parameters.AddWithValue("@Name", strName);
                cmdUpdateExport.Parameters.AddWithValue("@DateModified", NowOffset);
                cmdUpdateExport.Parameters.AddWithValue("@Details", strDetails);
                cmdUpdateExport.Parameters.AddWithValue("@FieldDelimiter", numFieldDelimiter);
                cmdUpdateExport.Parameters.AddWithValue("@StringDelimiter", numStringDelimiter);
                cmdUpdateExport.Parameters.AddWithValue("@ExportID", numExportID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateExport.Transaction = savePoint;

                cmdUpdateExport.ExecuteNonQuery();

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

    public static bool _DeleteSavedExport(long numExportID, string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdDeleteExport = sqlConn.CreateCommand;
            cmdDeleteExport.CommandText = "_spKartrisSavedExports_Delete";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdDeleteExport.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdDeleteExport.Parameters.AddWithValue("@ExportID", numExportID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdDeleteExport.Transaction = savePoint;

                cmdDeleteExport.ExecuteNonQuery();

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

    public static DataTable _ExportOrders(string strDateFrom, string strDateTo, bool blnIncludeDetails, bool blnIncompleteOrders)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdExecuteQuery = sqlConn.CreateCommand;
            cmdExecuteQuery.CommandText = "_spKartrisDB_ExportOrders";

            cmdExecuteQuery.CommandType = CommandType.StoredProcedure;
            cmdExecuteQuery.Parameters.AddWithValue("@StartDate", FixNullToDB(strDateFrom));
            cmdExecuteQuery.Parameters.AddWithValue("@EndDate", FixNullToDB(strDateTo));
            cmdExecuteQuery.Parameters.AddWithValue("@IncludeDetails", blnIncludeDetails);
            cmdExecuteQuery.Parameters.AddWithValue("@IncludeIncomplete", blnIncompleteOrders);
            cmdExecuteQuery.CommandTimeout = 3600;

            try
            {
                DataTable tblExport = new DataTable();
                using (SqlDataAdapter adptr = new SqlDataAdapter(cmdExecuteQuery))
                {
                    adptr.Fill(tblExport);
                    return tblExport;
                }
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
            }
        }
        return null/* TODO Change to default(_) if this is not a reference type */;
    }
    public static DataTable _CustomExecute(string strQuery)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdExecuteQuery = sqlConn.CreateCommand;
            cmdExecuteQuery.CommandText = "_spKartrisDB_ExecuteQuery";

            cmdExecuteQuery.CommandType = CommandType.StoredProcedure;
            cmdExecuteQuery.Parameters.AddWithValue("@QueryText", FixNullToDB(strQuery, "s"));
            cmdExecuteQuery.CommandTimeout = 3600;

            try
            {
                DataTable tblExport = new DataTable();
                using (SqlDataAdapter adptr = new SqlDataAdapter(cmdExecuteQuery))
                {
                    adptr.Fill(tblExport);
                    return tblExport;
                }
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        return null/* TODO Change to default(_) if this is not a reference type */;
    }
}
