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

using System.Web.HttpContext;
using kartrisStockNotificationsDataTableAdapters;
using CkartrisFormatErrors;
using CkartrisDataManipulation;
using CkartrisDisplayFunctions;

public class StockNotificationsBLL
{
    private static StockNotificationsTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;
    protected static StockNotificationsTblAdptr Adptr
    {
        get
        {
            _Adptr = new StockNotificationsTblAdptr();
            return _Adptr;
        }
    }

    private static VersionsAwaitingTblAdptr _Adptr2 = null/* TODO Change to default(_) if this is not a reference type */;
    protected static VersionsAwaitingTblAdptr Adptr2
    {
        get
        {
            _Adptr2 = new VersionsAwaitingTblAdptr();
            return _Adptr2;
        }
    }

    /// <summary>
    ///     ''' This sub can be called from the
    ///     ''' ProductVersions.ascx.vb in response to
    ///     ''' button clicks on notify-me buttons to
    ///     ''' show the popup for users to enter their
    ///     ''' email address into.
    ///     ''' </summary>
    ///     ''' <param name="strUserEmail">Email of customer requesting the notification</param>
    ///     ''' <param name="numVersionID">ID of the version they wish to be notified about</param>
    ///     ''' <param name="strPageLink">The link to the page where this item is</param>
    ///     ''' <param name="strProductName">Name of product</param>
    ///     ''' <param name="numLanguageID">Language ID</param>
    public static void AddNewStockNotification(string strUserEmail, Int64 numVersionID, string strPageLink, string strProductName, byte numLanguageID)
    {

        // Couple of values not passed in
        DateTime datDateCreated = DateTime.Now();
        string strStatus = "w"; // waiting to be sent!
        string strUserIP = CkartrisEnvironment.GetClientIPAddress();

        // Connection string
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();

        // Connect to sproc and send over the info
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddStockNotification = sqlConn.CreateCommand;
            cmdAddStockNotification.CommandText = "spKartrisStockNotification_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddStockNotification.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdAddStockNotification.Parameters.AddWithValue("@UserEmail", strUserEmail);
                cmdAddStockNotification.Parameters.AddWithValue("@VersionID", numVersionID);
                cmdAddStockNotification.Parameters.AddWithValue("@PageLink", strPageLink);
                cmdAddStockNotification.Parameters.AddWithValue("@ProductName", strProductName);
                cmdAddStockNotification.Parameters.AddWithValue("@OpenedDate", datDateCreated);
                cmdAddStockNotification.Parameters.AddWithValue("@UserIP", strUserIP);
                cmdAddStockNotification.Parameters.AddWithValue("@LanguageID", numLanguageID);
                cmdAddStockNotification.Parameters.AddWithValue("@newSNR_ID", 0);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddStockNotification.Transaction = savePoint;

                cmdAddStockNotification.ExecuteNonQuery();
                savePoint.Commit();
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
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
    }

    /// <summary>
    ///     ''' Update stock notification record when it is sent
    ///     ''' with date and changed status
    ///     ''' </summary>
    ///     ''' <param name="numSNR_ID">The ID of the stock notification to update</param>
    public static void _UpdateStockNotification(Int64 numSNR_ID)
    {

        // Couple of values not passed in
        DateTime datDateSettled = DateTime.Now();
        string strStatus = "s"; // sent!

        // Connection string
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();

        // Connect to sproc and send over the info
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateStockNotification = sqlConn.CreateCommand;
            cmdUpdateStockNotification.CommandText = "_spKartrisStockNotification_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateStockNotification.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateStockNotification.Parameters.AddWithValue("@SNR_ID", numSNR_ID);
                cmdUpdateStockNotification.Parameters.AddWithValue("@DateSettled", datDateSettled);
                cmdUpdateStockNotification.Parameters.AddWithValue("@strStatus", strStatus);
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateStockNotification.Transaction = savePoint;

                cmdUpdateStockNotification.ExecuteNonQuery();
                savePoint.Commit();
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
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
    }

    /// <summary>
    ///     ''' Update version to reset bulktimestamp date. This is used
    ///     ''' when processing bulk notifications in back end, so we
    ///     ''' know which versions were updated
    ///     ''' </summary>
    ///     ''' <param name="numV_ID">The version ID to update</param>
    public static void _UpdateVersionResetTimeStamp(Int64 numV_ID)
    {

        // Connection string
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();

        // Connect to sproc and send over the info
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdate = sqlConn.CreateCommand;
            cmdUpdate.CommandText = "_spKartrisVersions_UpdateBulkTimeStamp";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdate.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdate.Parameters.AddWithValue("@V_ID", numV_ID);
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdate.Transaction = savePoint;

                cmdUpdate.ExecuteNonQuery();
                savePoint.Commit();
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
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
    }

    /// <summary>
    ///     ''' This sub is called whenever a version is updated, stock
    ///     ''' tracking is enabled and the current item is in
    ///     ''' stock
    ///     ''' </summary>
    ///     ''' <param name="numVersionID">ID of the version they wish to be notified about</param>
    public static int _SearchSendStockNotifications(Int64 numVersionID)
    {

        // Look up all pending notifications for this
        // version ID
        DataTable tblStockNotifications = new DataTable();
        tblStockNotifications = _GetStockNotificationsByVersionID(numVersionID, "w");

        // How many stock notifications
        int numRecords = tblStockNotifications.Rows.Count;
        // Ok, now we have a datatable of stock notifications, we work through
        // one by one, send the notification and update the record to indicate
        // has been sent.
        foreach (var drwStockNotification in tblStockNotifications.Rows)
        {
            Int64 numID = drwStockNotification("SNR_ID");
            string strEmailTo = drwStockNotification("SNR_UserEmail");
            string strPageLink = drwStockNotification("SNR_PageLink");
            string strProductName = drwStockNotification("SNR_ProductName");
            byte numLanguageID = drwStockNotification("SNR_LanguageID");

            // Email to send from depends on language ID
            string strFromEmail = LanguagesBLL.GetEmailFrom(numLanguageID);

            // Retrieve the template for the user's language ID
            string strEmailTemplateText = RetrieveHTMLEmailTemplate("StockNotification", numLanguageID);

            // Set email body to template text
            string strEmailBody = strEmailTemplateText;

            // Put pagelink and product name into mail
            strEmailBody = strEmailBody.Replace("[pagelink]", CkartrisBLL.WebShopURLhttp + strPageLink.Substring(1, strPageLink.Length - 1));
            strEmailBody = strEmailBody.Replace("[productname]", System.Web.HttpContext.Current.Server.HtmlEncode(strProductName));

            strEmailBody = strEmailBody.Replace("[pagelink]", System.Web.HttpContext.GetGlobalResourceObject("Kartris", "Config_Webshopname"));
            strEmailBody = Strings.Replace(strEmailBody, "[productname]", System.Web.HttpContext.Current.Server.HtmlEncode(strProductName));

            // Subject line
            string strSubject = System.Web.HttpContext.GetGlobalResourceObject("_StockNotification", "ContentText_StockNotification");

            // Send email
            SendEmail(strFromEmail, strEmailTo, strSubject, strEmailBody, null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, true);

            // LogError(strEmailBody)
            // Update notification record to confirm as sent
            _UpdateStockNotification(numID);
        }

        return numRecords;
    }

    /// <summary>
    ///     ''' Returns database of stock notifications for the
    ///     ''' specified Version ID
    ///     ''' </summary>
    ///     ''' <param name="numVersionID">ID of the version they wish to be notified about</param>
    public static DataTable _GetStockNotificationsByVersionID(Int64 numVersionID, string strStatus)
    {
        return Adptr._GetStockNotificationsByVersionID(numVersionID, strStatus);
    }

    /// <summary>
    ///     ''' Returns datatable of versions which need
    ///     ''' to have notification checks run on them 
    ///     ''' (i.e. versions uploaded via data tool, etc.
    ///     ''' where stock notification check doesn't run)
    ///     ''' </summary>
    public static DataTable _GetVersions()
    {
        return Adptr2._GetVersions();
    }

    /// <summary>
    ///     ''' Produces a count of versions that need to
    ///     ''' have stock notification checks run against
    ///     ''' them.
    ///     ''' </summary>
    public static Int64 _GetCountVersionsAwaitingChecks()
    {
        DataTable dtbVersions = _GetVersions();
        return dtbVersions.Rows.Count;
    }

    /// <summary>
    ///     ''' Returns table of unfulfilled stock notification
    ///     ''' requests, including name of product/version they're
    ///     ''' waiting for and P_ID, V_ID we can use to link to
    ///     ''' those items
    ///     ''' </summary>
    public static DataTable _GetStockNotificationsDetails(string strStatus)
    {
        DataTable dtbDetails = Adptr._GetStockNotificationsDetails(strStatus);
        return dtbDetails;
    }

    /// <summary>
    ///     ''' Loop through the versions that have been bulk
    ///     ''' updated and check for notifications
    ///     ''' </summary>
    public static bool _RunBulkCheck()
    {
        // Let's get a datatable with versions that need to check
        DataTable dtbVersions = _GetVersions();
        foreach (var drwVersion in dtbVersions.Rows)
        {
            _SearchSendStockNotifications(drwVersion("V_ID"));
            _UpdateVersionResetTimeStamp(drwVersion("V_ID"));
        }
        return true;
    }
}
