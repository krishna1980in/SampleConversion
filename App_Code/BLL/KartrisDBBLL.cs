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

using kartrisDBDataTableAdapters;
using CkartrisFormatErrors;
using CkartrisEnumerations;
using CkartrisDataManipulation;
using System.Web.HttpContext;
using KartSettingsManager;

public class KartrisDBBLL
{
    public static DataTable GetProductsAttributesToCompare(string pProductList, short pLanguageID, short pCustomerGroupID)
    {
        CustomProductsToCompareTblAdptr adptr = new CustomProductsToCompareTblAdptr();
        return adptr.GetAttributesToCompare(pProductList, pLanguageID, pCustomerGroupID);
    }

    public static DataTable GetSearchResult(string pSearchText, string pKeyList, short pLanguageID, short pPageIndx, short pRowsPerPage, ref int pTotalSearchResult, float pMinPrice, float pMaxPrice, string pSearchMethod, short pCustomerGroupID)
    {
        pSearchText = System.Web.HttpContext.Current.Server.UrlDecode(pSearchText);
        SearchTblAdptr adptr = new SearchTblAdptr();
        if (GetKartConfig("general.fts.enabled") == "y")
        {
            try
            {
                return GetSearchResult(true, pSearchText, pKeyList, pLanguageID, pPageIndx, pRowsPerPage, ref pTotalSearchResult, pMinPrice, pMaxPrice, pSearchMethod, pCustomerGroupID);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2812 || ex.Number == 208)
                {
                    if (!IsFTSEnabled())
                    {
                        ConfigBLL._UpdateConfigValue("general.fts.enabled", "n");
                        return GetSearchResult(false, pSearchText, pKeyList, pLanguageID, pPageIndx, pRowsPerPage, ref pTotalSearchResult, pMinPrice, pMaxPrice, pSearchMethod, pCustomerGroupID);
                    }
                }
            }
        }
        return GetSearchResult(false, pSearchText, pKeyList, pLanguageID, pPageIndx, pRowsPerPage, ref pTotalSearchResult, pMinPrice, pMaxPrice, pSearchMethod, pCustomerGroupID);
    }

    public static DataTable GetSearchResult(bool blnIsFTS, string pSearchText, string pKeyList, short pLanguageID, short pPageIndx, short pRowsPerPage, ref int pTotalSearchResult, float pMinPrice, float pMaxPrice, string pSearchMethod, short pCustomerGroupID)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdSearchFTS = sqlConn.CreateCommand;
            cmdSearchFTS.Connection = sqlConn;
            cmdSearchFTS.CommandText = "spKartrisDB_Search";
            if (blnIsFTS)
                cmdSearchFTS.CommandText += "FTS";
            cmdSearchFTS.CommandTimeout = 2700;
            cmdSearchFTS.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdSearchFTS.Parameters.AddWithValue("@SearchText", FixNullToDB(pSearchText));
                cmdSearchFTS.Parameters.AddWithValue("@keyWordsList", FixNullToDB(pKeyList));
                cmdSearchFTS.Parameters.AddWithValue("@LANG_ID", FixNullToDB(pLanguageID, "i"));
                cmdSearchFTS.Parameters.AddWithValue("@PageIndex", pPageIndx);
                cmdSearchFTS.Parameters.AddWithValue("@RowsPerPage", FixNullToDB(pRowsPerPage, "i"));
                cmdSearchFTS.Parameters.AddWithValue("@TotalResultProducts", pTotalSearchResult).Direction = ParameterDirection.Output;
                cmdSearchFTS.Parameters.AddWithValue("@MinPrice", FixNullToDB(pMinPrice, "g"));
                cmdSearchFTS.Parameters.AddWithValue("@MaxPrice", FixNullToDB(pMaxPrice, "g"));
                cmdSearchFTS.Parameters.AddWithValue("@Method", FixNullToDB(pSearchMethod));
                cmdSearchFTS.Parameters.AddWithValue("@CustomerGroupID", FixNullToDB(pCustomerGroupID, "i"));

                SqlDataAdapter da = new SqlDataAdapter(cmdSearchFTS);
                DataSet ds = new DataSet();
                da.Fill(ds, "tblSearch");
                pTotalSearchResult = cmdSearchFTS.Parameters("@TotalResultProducts").Value;
                return ds.Tables("tblSearch");
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                    sqlConn.Close();
            }
        }
        return null/* TODO Change to default(_) if this is not a reference type */;
    }
    public static DataTable _SearchBackEnd(string pSearchLocation, string pKeyList, short pLanguageID, short pPageIndx, short pRowsPerPage, ref int pTotalSearchResult)
    {
        SearchTblAdptr adptr = new SearchTblAdptr();
        if (GetKartConfig("general.fts.enabled") == "y")
        {
            try
            {
                return adptr._GetBackEndSearchFTS(pSearchLocation, pKeyList, pLanguageID, pPageIndx, pRowsPerPage, pTotalSearchResult);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2812 || ex.Number == 208)
                {
                    if (!IsFTSEnabled())
                    {
                        ConfigBLL._UpdateConfigValue("general.fts.enabled", "n");
                        return adptr._GetBackEndSearch(pSearchLocation, pKeyList, pLanguageID, pPageIndx, pRowsPerPage, pTotalSearchResult);
                    }
                }
            }
        }
        return adptr._GetBackEndSearch(pSearchLocation, pKeyList, pLanguageID, pPageIndx, pRowsPerPage, pTotalSearchResult);
    }

    public static void _LoadTaskList(ref int numOrdersToInvoice, ref int numOrdersNeedPayment, ref int numOrdersToDispatch, ref int numStockWarning, ref int numOutOfStock, ref int numWaitingReviews, ref int numWaitingAffiliates, ref int numCustomersWaitingRefunds, ref int numCustomersInArrears, ref int numCustomersToAnonymize, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdTaskListValues = sqlConn.CreateCommand;
            cmdTaskListValues.CommandText = "_spKartrisDB_GetTaskList";
            cmdTaskListValues.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdTaskListValues.Parameters.AddWithValue("@NoOrdersToInvoice", 0).Direction = ParameterDirection.Output;
                cmdTaskListValues.Parameters.AddWithValue("@NoOrdersNeedPayment", 0).Direction = ParameterDirection.Output;
                cmdTaskListValues.Parameters.AddWithValue("@NoOrdersToDispatch", 0).Direction = ParameterDirection.Output;
                cmdTaskListValues.Parameters.AddWithValue("@NoStockWarnings", 0).Direction = ParameterDirection.Output;
                cmdTaskListValues.Parameters.AddWithValue("@NoOutOfStock", 0).Direction = ParameterDirection.Output;
                cmdTaskListValues.Parameters.AddWithValue("@NoReviewsWaiting", 0).Direction = ParameterDirection.Output;
                cmdTaskListValues.Parameters.AddWithValue("@NoAffiliatesWaiting", 0).Direction = ParameterDirection.Output;
                cmdTaskListValues.Parameters.AddWithValue("@NoCustomersWaitingRefunds", 0).Direction = ParameterDirection.Output;
                cmdTaskListValues.Parameters.AddWithValue("@NoCustomersInArrears", 0).Direction = ParameterDirection.Output;
                cmdTaskListValues.Parameters.AddWithValue("@NoCustomersToAnonymize", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();

                cmdTaskListValues.ExecuteNonQuery();

                numOrdersToInvoice = cmdTaskListValues.Parameters("@NoOrdersToInvoice").Value;
                numOrdersNeedPayment = cmdTaskListValues.Parameters("@NoOrdersNeedPayment").Value;
                numOrdersToDispatch = cmdTaskListValues.Parameters("@NoOrdersToDispatch").Value;
                numStockWarning = cmdTaskListValues.Parameters("@NoStockWarnings").Value;
                numOutOfStock = cmdTaskListValues.Parameters("@NoOutOfStock").Value;
                numWaitingReviews = cmdTaskListValues.Parameters("@NoReviewsWaiting").Value;
                numWaitingAffiliates = cmdTaskListValues.Parameters("@NoAffiliatesWaiting").Value;
                numCustomersWaitingRefunds = cmdTaskListValues.Parameters("@NoCustomersWaitingRefunds").Value;
                numCustomersInArrears = cmdTaskListValues.Parameters("@NoCustomersInArrears").Value;
                numCustomersToAnonymize = cmdTaskListValues.Parameters("@NoCustomersToAnonymize").Value;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                    sqlConn.Close();
            }
        }
    }

    public static bool _CreateDBBackup(string strBackupPath, string strBackupDescription, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdBackup = sqlConn.CreateCommand;
            cmdBackup.CommandText = "_spKartrisDB_CreateBackup";
            cmdBackup.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdBackup.Parameters.AddWithValue("@BackupPath", strBackupPath);
                sqlConn.Open();
                cmdBackup.ExecuteNonQuery();
                if (!string.IsNullOrEmpty(strBackupDescription))
                {
                    string strDescPath = strBackupPath.Replace(".bak", ".txt");
                    StreamWriter sw;
                    sw = File.CreateText(strDescPath);
                    sw.Write(strBackupDescription);
                    sw.Flush();
                    sw.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                    sqlConn.Close();
            }
        }
        return false;
    }

    public static void _GetDBInformation(ref DataTable tblDBInformation, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdInfo = sqlConn.CreateCommand;
            cmdInfo.CommandText = "_spKartrisDB_GetInformation";
            cmdInfo.CommandType = CommandType.StoredProcedure;
            try
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter(cmdInfo))
                {
                    adptr.Fill(tblDBInformation);
                }
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                    sqlConn.Close();
            }
        }
    }

    public static void _GetFTSInformation(ref bool blnKartrisCatalogExist, ref bool blnKartrisFTSEnabled, ref string strKartrisFTSLanguages, ref bool blnFTSSupported, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdInfo = sqlConn.CreateCommand;
            cmdInfo.CommandText = "_spKartrisDB_GetFTSInfo";
            cmdInfo.CommandType = CommandType.StoredProcedure;
            cmdInfo.Parameters.AddWithValue("@kartrisCatalogExist", false).Direction = ParameterDirection.Output;
            cmdInfo.Parameters.AddWithValue("@kartrisFTSEnabled", false).Direction = ParameterDirection.Output;
            cmdInfo.Parameters.Add("@kartrisFTSLanguages", SqlDbType.NVarChar, 4000).Direction = ParameterDirection.Output;
            cmdInfo.Parameters("@kartrisFTSLanguages").Value = "";
            cmdInfo.Parameters.AddWithValue("@FTSSupported", false).Direction = ParameterDirection.Output;

            try
            {
                sqlConn.Open();
                cmdInfo.ExecuteNonQuery();
                blnKartrisCatalogExist = cmdInfo.Parameters("@kartrisCatalogExist").Value;
                blnKartrisFTSEnabled = cmdInfo.Parameters("@kartrisFTSEnabled").Value;
                strKartrisFTSLanguages = cmdInfo.Parameters("@kartrisFTSLanguages").Value;
                blnFTSSupported = cmdInfo.Parameters("@FTSSupported").Value;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                    sqlConn.Close();
            }
        }
    }
    public static bool IsFTSEnabled()
    {
        bool blnCatalogExist = false;
        bool blnFTSEnabled = false;

        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdInfo = sqlConn.CreateCommand;
            cmdInfo.CommandText = "_spKartrisDB_GetFTSInfo";
            cmdInfo.CommandType = CommandType.StoredProcedure;
            cmdInfo.Parameters.AddWithValue("@kartrisCatalogExist", false).Direction = ParameterDirection.Output;
            cmdInfo.Parameters.AddWithValue("@kartrisFTSEnabled", false).Direction = ParameterDirection.Output;
            cmdInfo.Parameters.AddWithValue("@kartrisFTSLanguages", "").Direction = ParameterDirection.Output;
            cmdInfo.Parameters.AddWithValue("@FTSSupported", false).Direction = ParameterDirection.Output;

            try
            {
                sqlConn.Open();
                cmdInfo.ExecuteNonQuery();
                blnCatalogExist = cmdInfo.Parameters("@kartrisCatalogExist").Value;
                blnFTSEnabled = cmdInfo.Parameters("@kartrisFTSEnabled").Value;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                    sqlConn.Close();
            }
        }

        if (blnCatalogExist && blnFTSEnabled)
            return true;

        return false;
    }



    public static void SetupFTS()
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdInfo = sqlConn.CreateCommand;
            cmdInfo.CommandText = "_spKartrisDB_SetupFTS";
            cmdInfo.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                cmdInfo.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                    sqlConn.Close();
            }
        }
    }
    public static void StopFTS()
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdInfo = sqlConn.CreateCommand;
            cmdInfo.CommandText = "_spKartrisDB_StopFTS";
            cmdInfo.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                cmdInfo.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                    sqlConn.Close();
            }
        }
    }


    public static DataTable _GetKartrisTriggers()
    {
        TriggersTblAdptr triggerAdptr = new TriggersTblAdptr();
        return triggerAdptr._GetDBTriggers();
    }
    public static void _EnableTrigger(string pTableName, string pTriggerName)
    {
        TriggersTblAdptr triggerAdptr = new TriggersTblAdptr();
        short numStatus = 0;
        triggerAdptr._EnableDBTrigger(pTriggerName, pTableName, numStatus);
        _AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Triggers, Interaction.IIf(numStatus == 1, "Succeeded.", "Failed"), "_spKartrisDB_EnableTrigger##", pTriggerName);
    }
    public static void _DisableTrigger(string pTableName, string pTriggerName)
    {
        TriggersTblAdptr triggerAdptr = new TriggersTblAdptr();
        short numStatus = 0;
        triggerAdptr._DisableDBTrigger(pTriggerName, pTableName, numStatus);
        _AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Triggers, Interaction.IIf(numStatus == 0, "Succeeded.", "Failed"), "_spKartrisDB_DisableTrigger##", pTriggerName);
    }
    public static void _EnableAllTriggers()
    {
        TriggersTblAdptr triggerAdptr = new TriggersTblAdptr();
        short numStatus = 0;
        triggerAdptr._EnableAllDBTriggers();
        _AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Triggers, null/* TODO Change to default(_) if this is not a reference type */, "_spKartrisDB_EnableAllTrigers", null/* TODO Change to default(_) if this is not a reference type */);
    }
    public static void _DisableAllTriggers()
    {
        TriggersTblAdptr triggerAdptr = new TriggersTblAdptr();
        short numStatus = 0;
        triggerAdptr._DisableAllDBTriggers();
        _AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Triggers, null/* TODO Change to default(_) if this is not a reference type */, "_spKartrisDB_DisableAllTrigers", null/* TODO Change to default(_) if this is not a reference type */);
    }
    public static void _AddAdminLog(string numLoginName, ADMIN_LOG_TABLE strType, string strDesc, string strQuery, string strRelatedID, SqlConnection sqlConn = null/* TODO Change to default(_) if this is not a reference type */, SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */)
    {
        try
        {
            if (sqlConn == null)
            {
                string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
                sqlConn = new SqlConnection(strConnString);
                sqlConn.Open();
            }

            SqlCommand cmdAddAdminLog = new SqlCommand("_spKartrisAdminLog_AddNewAdminLog", sqlConn);
            cmdAddAdminLog.CommandType = CommandType.StoredProcedure;

            if (savePoint != null)
                cmdAddAdminLog.Transaction = savePoint;

            cmdAddAdminLog.Parameters.AddWithValue("@LoginName", FixNullToDB(numLoginName));
            cmdAddAdminLog.Parameters.AddWithValue("@Type", FixNullToDB(strType.ToString()));
            cmdAddAdminLog.Parameters.AddWithValue("@Desc", FixNullToDB(strDesc));
            cmdAddAdminLog.Parameters.AddWithValue("@Query", FixNullToDB(strQuery));
            cmdAddAdminLog.Parameters.AddWithValue("@RelatedID", FixNullToDB(strRelatedID));
            cmdAddAdminLog.Parameters.AddWithValue("@IP", FixNullToDB(CkartrisEnvironment.GetClientIPAddress()));
            cmdAddAdminLog.Parameters.AddWithValue("@NowOffset", CkartrisDisplayFunctions.NowOffset);

            cmdAddAdminLog.ExecuteNonQuery();

            if (savePoint == null)
                sqlConn.Close();
        }
        catch (Exception ex)
        {
            string strMsg = "";
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
            if (savePoint != null)
                throw new ApplicationException(strMsg);
        }
        finally
        {
            if (savePoint == null)
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close(); savePoint.Dispose();
                }
            }
        }
    }

    public static bool _PurgeOldLogs(ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisAdminLog_PurgeOldData";
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                sqlConn.Open();
                cmd.Parameters.AddWithValue("@PurgeDate", DateTime.Now.AddDays(0 - System.Convert.ToInt32(GetKartConfig("backend.adminlog.purgedays"))));
                cmd.ExecuteNonQuery();
                sqlConn.Close();

                return true;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                    sqlConn.Close();
            }
            return false;
        }
    }
    public static DataTable _SearchAdminLog(string strKeyword, string strType, DateTime datFrom, DateTime datTo)
    {
        AdminLogTblAdptr adptrAdminLog = new AdminLogTblAdptr();
        return adptrAdminLog._Search(strKeyword, strType, datFrom, datTo);
    }
    public static DataRow _GetLogByID(int numLogID)
    {
        AdminLogTblAdptr adptrAdminLog = new AdminLogTblAdptr();
        try
        {
            return adptrAdminLog._GetByID(numLogID).Rows(0);
        }
        catch (Exception ex)
        {
        }
        return null/* TODO Change to default(_) if this is not a reference type */;
    }
    public static void _AdminClearRelatedData(char chrDataType, string strUser, string strPassword, ref bool blnSucceeded, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        UsersBLL objUsersBLL = new UsersBLL();

        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdClearProductsData = sqlConn.CreateCommand;
            cmdClearProductsData.CommandText = "_spKartrisAdminRelatedTables_Clear";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdClearProductsData.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdClearProductsData.Parameters.AddWithValue("@DataType", FixNullToDB(chrDataType, "c"));
                cmdClearProductsData.Parameters.AddWithValue("@UserName", FixNullToDB(strUser, "s"));
                cmdClearProductsData.Parameters.AddWithValue("@Password", objUsersBLL.EncryptSHA256Managed(FixNullToDB(strPassword), LoginsBLL._GetSaltByUserName(strUser), true));
                cmdClearProductsData.Parameters.AddWithValue("@IPAddress", FixNullToDB(CkartrisEnvironment.GetClientIPAddress()));
                cmdClearProductsData.Parameters.AddWithValue("@Succeeded", false).Direction = ParameterDirection.Output;
                cmdClearProductsData.Parameters.Add("@Output", SqlDbType.NVarChar, 4000).Direction = ParameterDirection.Output;
                cmdClearProductsData.Parameters("@Output").Value = "";

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdClearProductsData.Transaction = savePoint;

                cmdClearProductsData.ExecuteNonQuery();
                strMsg = cmdClearProductsData.Parameters("@Output").Value;
                string strRelatedRecords = "";
                if (chrDataType == "P")
                    strRelatedRecords = "Products Related Tables";
                if (chrDataType == "O")
                    strRelatedRecords = "Orders Related Tables";
                if (chrDataType == "S")
                    strRelatedRecords = "Sessions Related Tables";
                if (chrDataType == "C")
                    strRelatedRecords = "Content Related Tables";
                blnSucceeded = System.Convert.ToBoolean(cmdClearProductsData.Parameters("@Succeeded").Value);
                cmdClearProductsData.Parameters("@Password").Value = "[HIDDEN]";
                _AddAdminLog(strUser, ADMIN_LOG_TABLE.DataRecords, Interaction.IIf(blnSucceeded, "Succeeded", "Failed"), CreateQuery(cmdClearProductsData), strRelatedRecords, sqlConn, savePoint);
                savePoint.Commit();
                CkartrisBLL.RefreshKartrisCache();
            }
            catch (Exception ex)
            {
                if (!savePoint == null)
                    savePoint.Rollback();
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close(); savePoint.Dispose();
                }
            }
        }
    }
    public static DataTable _GetAdminRelatedTables(char chrDataType)
    {
        AdminRelatedTablesTblAdptr adptr = new AdminRelatedTablesTblAdptr();
        return adptr._GetByTpe(chrDataType);
    }
    public static bool _ExecuteQuery(string strQuery, ref int numAffectedRecords, ref DataTable tblReturnedRecords, string strUser, ref string strMessage)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdExecuteQuery = sqlConn.CreateCommand;
            cmdExecuteQuery.CommandText = "_spKartrisDB_ExecuteQuery";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdExecuteQuery.CommandType = CommandType.StoredProcedure;
            cmdExecuteQuery.Parameters.AddWithValue("@QueryText", FixNullToDB(strQuery, "s"));

            string strExecutionSucceeded = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
            string strExecutionFailed = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom");
            string strExecutionNotAllowed = HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgCommandNotAllowed");
            bool blnCommandAllowed = true;

            switch (Strings.Left(strQuery, 6).ToUpper())
            {
                case "SELECT":
                    {
                        try
                        {
                            using (SqlDataAdapter adptr = new SqlDataAdapter(cmdExecuteQuery))
                            {
                                adptr.Fill(tblReturnedRecords);
                                sqlConn.Open();
                                savePoint = sqlConn.BeginTransaction();
                                cmdExecuteQuery.Transaction = savePoint;
                                _AddAdminLog(strUser, ADMIN_LOG_TABLE.ExecuteQuery, strExecutionSucceeded, cmdExecuteQuery.CommandText + "##@QueryText=" + strQuery, null/* TODO Change to default(_) if this is not a reference type */, sqlConn, savePoint);
                                savePoint.Commit();
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {
                            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMessage);
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

                        break;
                    }

                default:
                    {
                        if (Strings.Left(strQuery, 6).ToUpper() == "INSERT" || Strings.Left(strQuery, 6).ToUpper() == "UPDATE" || Strings.Left(strQuery, 6).ToUpper() == "DELETE" || Strings.Left(strQuery, 15).ToUpper() == "ALTER PROCEDURE" || Strings.Left(strQuery, 16).ToUpper() == "CREATE PROCEDURE")
                        {
                            try
                            {
                                sqlConn.Open();
                                savePoint = sqlConn.BeginTransaction();
                                cmdExecuteQuery.Transaction = savePoint;
                                numAffectedRecords = cmdExecuteQuery.ExecuteNonQuery();
                                _AddAdminLog(strUser, ADMIN_LOG_TABLE.ExecuteQuery, strExecutionSucceeded, cmdExecuteQuery.CommandText + "##@QueryText=" + strQuery, null/* TODO Change to default(_) if this is not a reference type */, sqlConn, savePoint);
                                savePoint.Commit();
                                return true;
                            }
                            catch (Exception ex)
                            {
                                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMessage);
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
                        else
                        {
                            blnCommandAllowed = false;
                            strMessage = strExecutionNotAllowed;
                        }

                        break;
                    }
            }

            try
            {
                _AddAdminLog(strUser, ADMIN_LOG_TABLE.ExecuteQuery, Interaction.IIf(blnCommandAllowed, strExecutionFailed, strExecutionNotAllowed), cmdExecuteQuery.CommandText + "##@QueryText=" + strQuery, null/* TODO Change to default(_) if this is not a reference type */);
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMessage);
            }
        }
        return false;
    }
    public static void DeleteNotNeededFiles()
    {
        DataTable tblDeleted = _GetDeletedItems();
        CategoriesBLL objCategoriesBLL = new CategoriesBLL();
        ProductsBLL objProductsBLL = new ProductsBLL();
        VersionsBLL objVersionsBLL = new VersionsBLL();

        foreach (DataRow row in tblDeleted.Rows)
        {
            long numID = FixNullFromDB(row("Deleted_ID"));
            char chrType = FixNullFromDB(row("Deleted_Type"));
            if (numID != default(Long) && chrType != default(Char))
            {
                try
                {
                    switch (chrType)
                    {
                        case "c" // ' Category
                       :
                            {
                                DataTable tblRecord = objCategoriesBLL._GetByID(numID);
                                if (tblRecord.Rows.Count == 0)
                                    CkartrisImages.RemoveImages(CkartrisImages.IMAGE_TYPE.enum_CategoryImage, numID);
                                _DeleteRecord(numID, chrType);
                                break;
                            }

                        case "p" // ' Product
                 :
                            {
                                DataTable tblRecord = objProductsBLL._GetProductInfoByID(numID);
                                if (tblRecord.Rows.Count == 0)
                                    CkartrisImages.RemoveImages(CkartrisImages.IMAGE_TYPE.enum_ProductImage, numID);
                                _DeleteRecord(numID, chrType);
                                break;
                            }

                        case "v" // ' Version
                 :
                            {
                                if (System.Convert.ToInt32(FixNullFromDB(row("Deleted_VersionProduct"))) != default(Integer))
                                {
                                    DataTable tblRecord = objVersionsBLL._GetVersionByID(numID);
                                    if (tblRecord.Rows.Count == 0)
                                        CkartrisImages.RemoveImages(CkartrisImages.IMAGE_TYPE.enum_VersionImage, numID, row("Deleted_VersionProduct"));
                                }
                                _DeleteRecord(numID, chrType);
                                break;
                            }

                        case "m" // ' Media
                 :
                            {
                                DataTable tblRecord = MediaBLL._GetMediaLinkByID(numID);
                                if (tblRecord.Rows.Count == 0)
                                    CkartrisMedia.RemoveMedia(numID);
                                _DeleteRecord(numID, chrType);
                                break;
                            }

                        case "r" // ' Promotion
                 :
                            {
                                DataTable tblRecord = PromotionsBLL._GetPromotionByID(numID);
                                if (tblRecord.Rows.Count == 0)
                                    CkartrisImages.RemoveImages(CkartrisImages.IMAGE_TYPE.enum_PromotionImage, numID);
                                _DeleteRecord(numID, chrType);
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
    public static DataTable _GetDeletedItems()
    {
        DeletedItemsTblAdptr adptr = new DeletedItemsTblAdptr();
        return adptr._GetData();
    }
    public static void _DeleteRecord(long numID, string strType)
    {
        try
        {
            DeletedItemsTblAdptr adptr = new DeletedItemsTblAdptr();
            adptr._Delete(numID, strType);
        }
        catch (Exception ex)
        {
        }
    }
}
