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

using kartrisTicketsDataTableAdapters;
using CkartrisDisplayFunctions;
using CkartrisFormatErrors;
using KartSettingsManager;
using CkartrisDataManipulation;
using System.Web.HttpContext;
using CkartrisEnumerations;

public class TicketsBLL
{
    private static TicketsTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;

    protected static TicketsTblAdptr Adptr
    {
        get
        {
            _Adptr = new TicketsTblAdptr();
            return _Adptr;
        }
    }

    public static int AddSupportTicket(int numUserID, int numTypeID, string strSubject, string strText, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "spKartrisSupportTickets_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;

            int intReturnID = 0;
            try
            {
                cmd.Parameters.AddWithValue("@OpenedDate", NowOffset);
                cmd.Parameters.AddWithValue("@TicketType", FixNullToDB(numTypeID, "i"));
                cmd.Parameters.AddWithValue("@Subject", FixNullToDB(strSubject));
                cmd.Parameters.AddWithValue("@Text", FixNullToDB(strText));
                cmd.Parameters.AddWithValue("@U_ID", FixNullToDB(numUserID, "i"));
                cmd.Parameters.AddWithValue("@TIC_NewID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                cmd.ExecuteNonQuery();

                if (cmd.Parameters("@TIC_NewID").Value == null || cmd.Parameters("@TIC_NewID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                intReturnID = cmd.Parameters("@TIC_NewID").Value;
                savePoint.Commit();
                return intReturnID;
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
        return 0;
    }

    public static DataTable GetSupportTicketsByUser(int numUserID)
    {
        return Adptr.GetByUserID(numUserID);
    }

    public static DataTable GetTicketDetailsByID(long numTicketID, int numUserID)
    {
        return Adptr.GetDetailsByID(numTicketID, numUserID);
    }

    public static bool AddCustomerReply(long numTicketID, string strText, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "spKartrisSupportTickets_AddCustomerReply";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@TIC_ID", FixNullToDB(numTicketID, "i"));
                cmd.Parameters.AddWithValue("@NowOffset", NowOffset);
                cmd.Parameters.AddWithValue("@STM_Text", FixNullToDB(strText));
                cmd.Parameters.AddWithValue("@STM_NewID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                cmd.ExecuteNonQuery();

                if (cmd.Parameters("@STM_NewID").Value == null || cmd.Parameters("@STM_NewID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                savePoint.Commit();
                return true;
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
        return false;
    }

    public static DataTable _GetTickets()
    {
        return Adptr._GetSummary();
    }

    public static DataTable _GetTicketDetails(long numTicketID)
    {
        return Adptr._GetDetailsByID(numTicketID);
    }

    public static void _TicketsCounterSummary(ref int numUnassigned, ref int numAwaiting, int numLoginID)
    {
        Adptr._TicketsCounterSummary(numUnassigned, numAwaiting, numLoginID);
    }

    public static DataTable _SearchTickets(string strKeyword, short numLangID, short numAssignedID, short numTypeID, int numUserID, string strUserEmail, char chrStatus)
    {
        return Adptr._SearchTickets(strKeyword, numLangID, numAssignedID, numTypeID, numUserID, strUserEmail, chrStatus);
    }

    public static bool _AddOwnerReply(long numTicketID, int numLoginID, string strText, int numTimeSpent, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisSupportTickets_AddOwnerReply";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@TIC_ID", FixNullToDB(numTicketID, "i"));
                cmd.Parameters.AddWithValue("@LOGIN_ID", numLoginID);
                cmd.Parameters.AddWithValue("@NowOffset", NowOffset);
                cmd.Parameters.AddWithValue("@STM_Text", FixNullToDB(strText));
                cmd.Parameters.AddWithValue("@TIC_TimeSpent", FixNullToDB(numTimeSpent, "i"));
                cmd.Parameters.AddWithValue("@STM_NewID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                cmd.ExecuteNonQuery();

                if (cmd.Parameters("@STM_NewID").Value == null || cmd.Parameters("@STM_NewID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                savePoint.Commit();
                return true;
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
        return false;
    }

    public static bool _UpdateTicket(long numTicketID, int numAssignedLoginID, char chrStatus, int numTimeSpent, string strTags, short numTicketTypeID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisSupportTickets_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@TIC_ID", FixNullToDB(numTicketID, "i"));
                cmd.Parameters.AddWithValue("@LOGIN_ID", numAssignedLoginID);
                cmd.Parameters.AddWithValue("@NowOffset", NowOffset);
                cmd.Parameters.AddWithValue("@TIC_Status", FixNullToDB(chrStatus, "c"));
                cmd.Parameters.AddWithValue("@TIC_TimeSpent", numTimeSpent);
                cmd.Parameters.AddWithValue("@TIC_Tags", FixNullToDB(strTags));
                cmd.Parameters.AddWithValue("@STT_ID", FixNullToDB(numTicketTypeID, "i"));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                cmd.ExecuteNonQuery();
                savePoint.Commit();
                return true;
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
        return false;
    }

    public static bool _DeleteTicket(int numTicketID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisSupportTickets_Delete";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@TIC_ID", FixNullToDB(numTicketID, "i"));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                cmd.ExecuteNonQuery();
                savePoint.Commit();
                return true;
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
        return false;
    }

    private static TicketTypesTblAdptr _AdptrTypes = null/* TODO Change to default(_) if this is not a reference type */;

    protected static TicketTypesTblAdptr AdptrTypes
    {
        get
        {
            _AdptrTypes = new TicketTypesTblAdptr();
            return _AdptrTypes;
        }
    }

    public static DataTable _GetTicketTypes()
    {
        return AdptrTypes._GetData();
    }

    public static bool _AddTicketType(string strType, char chrLevel, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisSupportTicketTypes_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@Type", FixNullToDB(strType));
                cmd.Parameters.AddWithValue("@Level", FixNullToDB(chrLevel, "c"));
                cmd.Parameters.AddWithValue("@New_ID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                cmd.ExecuteNonQuery();

                if (cmd.Parameters("@New_ID").Value == null || cmd.Parameters("@New_ID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                savePoint.Commit();
                return true;
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
        return false;
    }

    public static bool _UpdateTicketType(int numTypeID, string strType, char chrLevel, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisSupportTicketTypes_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@ID", FixNullToDB(numTypeID, "i"));
                cmd.Parameters.AddWithValue("@Type", FixNullToDB(strType));
                cmd.Parameters.AddWithValue("@Level", FixNullToDB(chrLevel, "c"));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                cmd.ExecuteNonQuery();

                savePoint.Commit();
                return true;
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
        return false;
    }

    public static bool _DeleteTicketType(int numTypeID, int numNewTypeID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisSupportTicketTypes_Delete";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@ID", FixNullToDB(numTypeID, "i"));
                cmd.Parameters.AddWithValue("@NewTypeID", FixNullToDB(numNewTypeID, "i"));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                cmd.ExecuteNonQuery();

                savePoint.Commit();
                return true;
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
        return false;
    }

    private static TicketMessagesTblAdptr _AdptrMessages = null/* TODO Change to default(_) if this is not a reference type */;

    protected static TicketMessagesTblAdptr AdptrMessages
    {
        get
        {
            _AdptrMessages = new TicketMessagesTblAdptr();
            return _AdptrMessages;
        }
    }

    public static DataTable _GetTicketMessages(int numTicketID)
    {
        return AdptrMessages._GetByTicketID(numTicketID);
    }

    public static DataTable _GetLastByCustomer(int numTicketID)
    {
        return AdptrMessages._GetLastByCustomer(numTicketID);
    }

    public static DataTable GetLastByOwner(int numTicketID)
    {
        return AdptrMessages.GetLastByOwner(numTicketID);
    }
}
