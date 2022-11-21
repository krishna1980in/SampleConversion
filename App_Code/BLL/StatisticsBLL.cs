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

using System.Web.HttpContext;
using kartrisStatisticsDataTableAdapters;
using CkartrisFormatErrors;
using CkartrisDataManipulation;
using CkartrisDisplayFunctions;

public class StatisticsBLL
{
    private static StatisticsTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;

    protected static StatisticsTblAdptr Adptr
    {
        get
        {
            _Adptr = new StatisticsTblAdptr();
            return _Adptr;
        }
    }

    public static DataTable _GetCategoryYearSummary()
    {
        return Adptr._GetCategoryYearSummary(NowOffset);
    }

    public static DataTable _GetCategoriesByDate(byte numMonth, short numYear, byte numLanguage)
    {
        return Adptr._GetCategoriesByDate(numMonth, numYear, numLanguage);
    }

    public static DataTable _GetProductYearSummary()
    {
        return Adptr._GetProductYearSummary(NowOffset);
    }

    public static DataTable _GetProductsByDate(byte numMonth, short numYear, byte numLanguage)
    {
        return Adptr._GetProductsByDate(numMonth, numYear, numLanguage);
    }

    public static DataTable _GetProductStatsDetailsByDate(int numProductID, byte numMonth, short numYear, byte numLanguage)
    {
        return Adptr._GetProductStatsDetailsByDate(numProductID, numMonth, numYear, numLanguage);
    }

    public static DataTable GetRecentlyViewedProducts(byte numLanguageID)
    {
        return Adptr.GetRecentlyViewedProducts(numLanguageID);
    }

    public static void AddNewStatsRecord(char chrItemType, int numItemID, int numItemParentID = 0)
    {
        if (!IsValidUserAgent() || numItemID == 0)
            return;

        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddStats = sqlConn.CreateCommand;
            cmdAddStats.CommandText = "spKartrisStatistics_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddStats.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdAddStats.Parameters.AddWithValue("@Type", FixNullToDB(chrItemType, "c"));
                cmdAddStats.Parameters.AddWithValue("@ParentID", FixNullToDB(numItemParentID, "i"));
                cmdAddStats.Parameters.AddWithValue("@ItemID", FixNullToDB(numItemID, "i"));
                cmdAddStats.Parameters.AddWithValue("@IP", CkartrisEnvironment.GetClientIPAddress());
                cmdAddStats.Parameters.AddWithValue("@NowOffset", NowOffset);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddStats.Transaction = savePoint;

                cmdAddStats.ExecuteNonQuery();
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

    public static DataTable _GetOrdersTurnover(DateTime datFrom, DateTime datTo)
    {
        return Adptr._GetOrdersTurnover(datFrom, datTo);
    }

    public static DataTable _GetAverageVisits()
    {
        return Adptr._GetAverageVisits(NowOffset);
    }

    public static DataTable _GetAverageOrders()
    {
        return Adptr._GetAverageOrders(NowOffset);
    }

    private static SearchStatisticsTblAdptr _AdptrSearch = null/* TODO Change to default(_) if this is not a reference type */;

    protected static SearchStatisticsTblAdptr AdptrSearch
    {
        get
        {
            _AdptrSearch = new SearchStatisticsTblAdptr();
            return _AdptrSearch;
        }
    }

    public static void ReportSearchStatistics(string strKeywordList)
    {
        DateTime datCurrentDate = NowOffset();
        AdptrSearch.ReportSearchStatistics(strKeywordList, datCurrentDate.Year, datCurrentDate.Month, datCurrentDate.Day, new DateTime(datCurrentDate.Year, datCurrentDate.Month, datCurrentDate.Day));
    }

    public static DataTable _GetTopSearches(DateTime datFrom, DateTime datTo, int numNoOfRecords)
    {
        return AdptrSearch._GetTopSearches(datFrom, datTo, numNoOfRecords);
    }
}
