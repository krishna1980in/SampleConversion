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

using kartrisBasketDataTableAdapters;
using System.Web.HttpContext;
using KartSettingsManager;
/// <summary>

/// ''' Business logic layer for handling affiliates

/// ''' </summary>

/// ''' <remarks>Affiliates are customers that sell product on behalf of the shop and receive a commission for this. Like an Avon representative</remarks>
public class AffiliateBLL
{

    // Affiliates are people who can sell on behalf of the shop owner.
    // Examples would be an Avon representative, they sell on behalf of Avon but are not
    // direct employees neither are they on the payroll.

    protected static CustomersTblAdptr _CustomersAdptr
    {
        get
        {
            return new CustomersTblAdptr();
        }
    }

    /// <summary>
    ///     ''' Set the user as an affiliate
    ///     ''' </summary>
    ///     ''' <param name="numUserID">User to set as affiliate</param>
    ///     ''' <remarks></remarks>
    public static void UpdateCustomerAffiliateStatus(int numUserID)
    {
        _CustomersAdptr.UpdateAffiliate(1, numUserID, 0, 0);
    }

    /// <summary>
    ///     ''' Set the commission rate for the user
    ///     ''' </summary>
    ///     ''' <param name="numUserID">User to update</param>
    ///     ''' <param name="numCommission">commission rate (percentage)</param>
    ///     ''' <remarks></remarks>
    public static void UpdateCustomerAffiliateCommission(int numUserID, double numCommission)
    {
        _CustomersAdptr.UpdateAffiliate(2, numUserID, numCommission, 0);
    }

    /// <summary>
    ///     ''' Set the affiliate ID
    ///     ''' </summary>
    ///     ''' <param name="numUserID">User to update</param>
    ///     ''' <param name="numAffiliateID">Affiliate Id to set</param>
    ///     ''' <remarks>Only works if the ID is currently NULL</remarks>
    public static void UpdateCustomerAffiliateID(int numUserID, int numAffiliateID)
    {
        _CustomersAdptr.UpdateAffiliate(3, numUserID, 0, numAffiliateID);
    }

    /// <summary>
    ///     ''' Log that the session has used a reference to an affiliate, this way we know that all orders within this session should result in payment to the referenced affiliate.
    ///     ''' </summary>
    ///     ''' <param name="numAffiliateId">The affiliate reference number</param>
    ///     ''' <param name="strReferer">the page and HTTP referrer that trigged this log addition</param>
    ///     ''' <param name="strIP">Source IP</param>
    ///     ''' <param name="strRequestedItem">Requested item (taken from query string at time of trigger)</param>
    ///     ''' <remarks>used to track which purchases should credit the affiliate with commission</remarks>
    public static void UpdateCustomerAffiliateLog(int numAffiliateId, string strReferer, string strIP, string strRequestedItem)
    {
        _CustomersAdptr.UpdateAffiliateLog(numAffiliateId, strReferer, strIP, strRequestedItem, CkartrisDisplayFunctions.NowOffset);
    }

    /// <summary>
    ///     ''' Is the selected user an affiliate
    ///     ''' </summary>
    ///     ''' <param name="numUserID">User to check</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static bool IsCustomerAffiliate(long numUserID)
    {
        DataTable tblAffiliate;
        bool blnIsAffiliate = false;

        tblAffiliate = BasketBLL.GetCustomerData(numUserID);
        if (tblAffiliate.Rows.Count > 0)
            blnIsAffiliate = tblAffiliate.Rows(0).Item("U_IsAffiliate");
        tblAffiliate.Dispose();

        return blnIsAffiliate;
    }

    /// <summary>
    ///     ''' Get the affiliate ID for a given user
    ///     ''' </summary>
    ///     ''' <param name="numUserID"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static long GetCustomerAffiliateID(long numUserID)
    {
        DataTable tblAffiliate;
        long numAffiliateID;

        tblAffiliate = BasketBLL.GetCustomerData(numUserID);
        if (tblAffiliate.Rows.Count > 0)
            numAffiliateID = tblAffiliate.Rows(0).Item("U_AffiliateID");
        tblAffiliate.Dispose();

        return numAffiliateID;
    }

    /// <summary>
    ///     ''' Check to see if the affiliate needs to be logged to ensure that the affiliate gets their commission if an item is being purchased.
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public static void CheckAffiliateLink()
    {
        int numAffiliateID, sessAffiliateID;
        Kartris.Basket objBasket = new Kartris.Basket();

        numAffiliateID = Conversion.Val(System.Web.HttpContext.Current.Request.QueryString["af"]);

        if (numAffiliateID > 0 && AffiliateBLL.IsCustomerAffiliate(numAffiliateID))
        {
            if (!(System.Web.HttpContext.Current.Session["C_AffiliateID"] == null))
                sessAffiliateID = Conversion.Val(System.Web.HttpContext.Current.Session["C_AffiliateID"]);
            else
                sessAffiliateID = 0;

            if (numAffiliateID != sessAffiliateID)
            {
                string strReferer, strIP, strRequestedItem;
                strReferer = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];
                if (strReferer == null)
                    strReferer = "";
                strIP = CkartrisEnvironment.GetClientIPAddress();
                strRequestedItem = Strings.Left(System.Web.HttpContext.Current.Request.ServerVariables["PATH_INFO"] + "?" + System.Web.HttpContext.Current.Request.ServerVariables["QUERY_STRING"], 255);

                AffiliateBLL.UpdateCustomerAffiliateLog(numAffiliateID, strReferer, strIP, strRequestedItem);
                System.Web.HttpContext.Current.Session["C_AffiliateID"] = numAffiliateID;
            }
        }

        objBasket = null/* TODO Change to default(_) if this is not a reference type */;
    }

    /// <summary>
    ///     ''' Get information related to an affiliate showing sales data group by month
    ///     ''' </summary>
    ///     ''' <param name="numUserID">the user ID</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable GetCustomerAffiliateActivitySales(int numUserID)
    {
        DataTable tblAffiliate = new DataTable();
        tblAffiliate = _CustomersAdptr.GetAffiliateData(1, numUserID, 0, 0);
        return tblAffiliate;
    }

    /// <summary>
    ///     ''' Return number of hits for an affiliate grouped by month
    ///     ''' </summary>
    ///     ''' <param name="numUserID">User Id to check</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks>hits are retrieved from the affiliate log table</remarks>
    public static DataTable GetCustomerAffiliateActivityHits(int numUserID)
    {
        DataTable tblAffiliate = new DataTable();
        tblAffiliate = _CustomersAdptr.GetAffiliateData(2, numUserID, 0, 0);
        return tblAffiliate;
    }

    /// <summary>
    ///     ''' Returns commission for affiliate for a specific month
    ///     ''' </summary>
    ///     ''' <param name="numUserID">User to retrieve data for</param>
    ///     ''' <param name="numMonth">Month number (e.g. May = 5, June = 6)</param>
    ///     ''' <param name="numYear">Year four digit number (e.g. '1996, 2005')</param>
    ///     ''' <returns>Order total, commission and hit count</returns>
    ///     ''' <remarks></remarks>
    public static DataTable GetCustomerAffiliateCommission(int numUserID, int numMonth, int numYear)
    {
        DataTable tblAffiliate = new DataTable();
        tblAffiliate = _CustomersAdptr.GetAffiliateData(3, numUserID, numMonth, numYear);
        return tblAffiliate;
    }

    /// <summary>
    ///     ''' Sales information for a single affiliate for a specific month. 
    ///     ''' </summary>
    ///     ''' <param name="numUserID">The user that is an affiliate</param>
    ///     ''' <param name="numMonth">Month number (e.g. 5 = May, 6 = June)</param>
    ///     ''' <param name="numYear">Year four digit number (e.g. '1996, 2005')</param>
    ///     ''' <returns>Commission, hit counts and order numbers</returns>
    ///     ''' <remarks></remarks>
    public static DataTable GetCustomerAffiliateSalesLink(int numUserID, int numMonth, int numYear)
    {
        DataTable tblAffiliate = new DataTable();
        tblAffiliate = _CustomersAdptr.GetAffiliateData(4, numUserID, numMonth, numYear);
        return tblAffiliate;
    }

    /// <summary>
    ///     ''' Get table of payments to an affiliate
    ///     ''' </summary>
    ///     ''' <param name="numUserID">User number that we want to retrieve data for</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable GetCustomerAffiliatePayments(int numUserID)
    {
        DataTable tblAffiliate = new DataTable();
        tblAffiliate = _CustomersAdptr.GetAffiliateData(5, numUserID, 0, 0);
        return tblAffiliate;
    }

    /// <summary>
    ///     ''' Get a list of sales commission information for an affiliate that has not been paid to them already
    ///     ''' </summary>
    ///     ''' <param name="numUserID">User number that we want to retrieve data for</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable GetCustomerAffiliateUnpaidSales(int numUserID)
    {
        DataTable tblAffiliate = new DataTable();
        tblAffiliate = _CustomersAdptr.GetAffiliateData(6, numUserID, 0, 0);
        return tblAffiliate;
    }

    /// <summary>
    ///     ''' Get a single line summary of paid and unpaid commission for a given user
    ///     ''' </summary>
    ///     ''' <param name="numUserId"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable _GetCustomerAffiliateCommissionSummary(long numUserId)
    {
        DataTable tblAffiliate = new DataTable();
        int intPaid = IIf(LCase(KartSettingsManager.GetKartConfig("frontend.users.affiliates.commissiononlyonpaid")) == "y", 1, 0);
        tblAffiliate = _CustomersAdptr._GetAffiliateCommission(0, numUserId, intPaid, 0, 0);
        return tblAffiliate;
    }

    /// <summary>
    ///     ''' Get unpaid commission details grouped by order
    ///     ''' </summary>
    ///     ''' <param name="numUserId">User ID for affiliate</param>
    ///     ''' <param name="PageIndex">The page number to show (paginated output)</param>
    ///     ''' <param name="PageSize">The page size (paginated output)</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable _GetCustomerAffiliateUnpaidCommission(long numUserId, int PageIndex = 0, int PageSize = 10)
    {
        DataTable tblAffiliate = new DataTable();
        int intPaid = IIf(LCase(KartSettingsManager.GetKartConfig("frontend.users.affiliates.commissiononlyonpaid")) == "y", 1, 0);
        tblAffiliate = _CustomersAdptr._GetAffiliateCommission(1, numUserId, intPaid, ((PageIndex - 1) * PageSize) + 1, (PageIndex * PageSize));
        return tblAffiliate;
    }

    /// <summary>
    ///     ''' Get paid commission details grouped by order
    ///     ''' </summary>
    ///     ''' <param name="numUserId">User ID for affiliate</param>
    ///     ''' <param name="PageIndex">The page number to show (paginated output)</param>
    ///     ''' <param name="PageSize">The page size (paginated output)</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable _GetCustomerAffiliatePaidCommission(long numUserId, int PageIndex = 0, int PageSize = 10)
    {
        DataTable tblAffiliate = new DataTable();
        int intPaid = IIf(LCase(KartSettingsManager.GetKartConfig("frontend.users.affiliates.commissiononlyonpaid")) == "y", 1, 0);
        tblAffiliate = _CustomersAdptr._GetAffiliateCommission(2, numUserId, intPaid, ((PageIndex - 1) * PageSize) + 1, (PageIndex * PageSize));
        return tblAffiliate;
    }

    /// <summary>
    ///     ''' Record payment to an affiliate
    ///     ''' </summary>
    ///     ''' <param name="intAffiliateID">The affiliate ID to add the information to</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks>only records the affiliate ID, date, and time</remarks>
    public static int _AddAffiliatePayments(int intAffiliateID)
    {
        DataTable tblAffiliate = new DataTable();

        tblAffiliate = _CustomersAdptr._AddAffiliatePayments(intAffiliateID, CkartrisDisplayFunctions.NowOffset);
        if (tblAffiliate.Rows.Count > 0)
            return tblAffiliate.Rows(0).Item("AFP_ID");
        else
            return 0;
    }

    /// <summary>
    ///     ''' Record an affiliate payment ID against an order ID
    ///     ''' </summary>
    ///     ''' <param name="intAffilliatePaymentID">The reference to the affiliate payment transaction</param>
    ///     ''' <param name="intOrderID">Order that the affiliate was paid for</param>
    ///     ''' <remarks>Used to record that the affiliate has been paid for the order</remarks>
    public static void _UpdateAffiliateCommission(int intAffilliatePaymentID, int intOrderID)
    {
        _CustomersAdptr._UpdateAffiliateOrders(1, intAffilliatePaymentID, intOrderID);
    }

    /// <summary>
    ///     ''' Delete references to the affiliate being paid
    ///     ''' </summary>
    ///     ''' <param name="intAffilliatePaymentID">The affiliate payment transation reference</param>
    ///     ''' <remarks></remarks>
    public static void _UpdateAffiliatePayment(int intAffilliatePaymentID)
    {
        _CustomersAdptr._UpdateAffiliateOrders(2, intAffilliatePaymentID, 0);
    }

    /// <summary>
    ///     ''' Show number of hits for month and year for all affiliates
    ///     ''' </summary>
    ///     ''' <param name="numMonth">Month number (e.g. 5 = May, 6 = June)</param>
    ///     ''' <param name="numYear">Year four digit number (e.g. '1996, 2005')</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable _GetAffiliateMonthlyHitsReport(int numMonth, int numYear)
    {
        return _CustomersAdptr._GetAffiliateReport(1, numMonth, numYear, CkartrisDisplayFunctions.NowOffset, 0, 0, 0, 0);
    }

    /// <summary>
    ///     ''' Generate affiliate hits report grouped by year. Grouped by affiliate and year
    ///     ''' </summary>
    ///     ''' <returns>shows number of hits each affiliate gained for each year</returns>
    ///     ''' <remarks></remarks>
    public static DataTable _GetAffiliateAnnualHitsReport()
    {
        return _CustomersAdptr._GetAffiliateReport(2, 0, 0, Format(DateAdd(DateInterval.Month, -11, CkartrisDisplayFunctions.NowOffset), "yyyy/MM/01 0:00:00"), 0, 0, 0, 0);
    }

    /// <summary>
    ///     ''' Generate report showing all affiliate sales for a given month and year. Grouped by affiliate
    ///     ''' </summary>
    ///     ''' <param name="numMonth">Month number (e.g. 5 = May, 6 = June)</param>
    ///     ''' <param name="numYear">Year four digit number (e.g. '1996, 2005')</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable _GetAffiliateMonthlySalesReport(int numMonth, int numYear)
    {
        short numPaid = IIf(Trim(LCase(GetKartConfig("frontend.users.affiliates.commissiononlyonpaid"))) == "y", 1, 0);
        return _CustomersAdptr._GetAffiliateReport(3, numMonth, numYear, CkartrisDisplayFunctions.NowOffset, numPaid, 0, 0, 0);
    }

    /// <summary>
    ///     ''' Generate report showing all affiliate sales. grouped by affiliate and year.
    ///     ''' </summary>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable _GetAffiliateAnnualSalesReport()
    {
        short numPaid = IIf(Trim(LCase(GetKartConfig("frontend.users.affiliates.commissiononlyonpaid"))) == "y", 1, 0);
        return _CustomersAdptr._GetAffiliateReport(4, 0, 0, Format(DateAdd(DateInterval.Month, -11, CkartrisDisplayFunctions.NowOffset), "yyyy/MM/01 0:00:00"), numPaid, 0, 0, 0);
    }

    /// <summary>
    ///     ''' Get a report showing sales, commission and hits for a given affiliate
    ///     ''' </summary>
    ///     ''' <param name="numMonth">Month number (e.g. 5 = May, 6 = June)</param>
    ///     ''' <param name="numYear">Year four digit number (e.g. '1996, 2005')</param>
    ///     ''' <param name="numAffiliateID">The affiliate you want to get data for</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable _GetAffiliateSummaryReport(int numMonth, int numYear, int numAffiliateID)
    {
        short numPaid = IIf(Trim(LCase(GetKartConfig("frontend.users.affiliates.commissiononlyonpaid"))) == "y", 1, 0);
        return _CustomersAdptr._GetAffiliateReport(5, numMonth, numYear, CkartrisDisplayFunctions.NowOffset, numPaid, numAffiliateID, 0, 0);
    }

    /// <summary>
    ///     ''' Get a report of hits associated with an affiliate. No grouping
    ///     ''' </summary>
    ///     ''' <param name="numMonth">Month number (e.g. 5 = May, 6 = June)</param>
    ///     ''' <param name="numYear">Year four digit number (e.g. '1996, 2005')</param>
    ///     ''' <param name="numAffiliateID">Affiliate we want to run the report for</param>
    ///     ''' <param name="PageIndex">The page we wish to view (paginated output)</param>
    ///     ''' <param name="PageSize">The size of each page (paginated output)</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks>hits are clicks etc.</remarks>
    public static DataTable _GetAffiliateRawDataHitsReport(int numMonth, int numYear, int numAffiliateID, int PageIndex = 0, int PageSize = 10)
    {
        return _CustomersAdptr._GetAffiliateReport(6, numMonth, numYear, CkartrisDisplayFunctions.NowOffset, 0, numAffiliateID, ((PageIndex - 1) * PageSize) + 1, (PageIndex * PageSize));
    }

    /// <summary>
    ///     ''' Get a report of all sales associated with an affiliate. No grouping
    ///     ''' </summary>
    ///     ''' <param name="numMonth">Month number (e.g. 5 = May, 6 = June)</param>
    ///     ''' <param name="numYear">Year four digit number (e.g. '1996, 2005')</param>
    ///     ''' <param name="numAffiliateID">Affiliate we want to run the report for</param>
    ///     ''' <param name="PageIndex">The page we wish to view (paginated output)</param>
    ///     ''' <param name="PageSize">The size of each page (paginated output)</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks>shows sales data</remarks>
    public static DataTable _GetAffiliateRawDataSalesReport(int numMonth, int numYear, int numAffiliateID, int PageIndex = 0, int PageSize = 10)
    {
        short numPaid = IIf(Trim(LCase(GetKartConfig("frontend.users.affiliates.commissiononlyonpaid"))) == "y", 1, 0);
        return _CustomersAdptr._GetAffiliateReport(7, numMonth, numYear, CkartrisDisplayFunctions.NowOffset, numPaid, numAffiliateID, ((PageIndex - 1) * PageSize) + 1, (PageIndex * PageSize));
    }
}
