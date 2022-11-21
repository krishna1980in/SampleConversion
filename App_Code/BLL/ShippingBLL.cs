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

using kartrisConfigData;
using kartrisShippingDataTableAdapters;
using CkartrisEnumerations;
using System.Web.HttpContext;
using CkartrisFormatErrors;
using CkartrisDataManipulation;

public class ShippingBLL
{
    private static ShippingMethodsRatesTblAdptr _ShippingMethodsRatesAdptr = null/* TODO Change to default(_) if this is not a reference type */;
    protected static ShippingMethodsRatesTblAdptr ShippingMethodsRatesAdptr
    {
        get
        {
            _ShippingMethodsRatesAdptr = new ShippingMethodsRatesTblAdptr();
            return _ShippingMethodsRatesAdptr;
        }
    }

    private static ShippingMethodsTblAdptr _ShippingMethodsAdptr = null/* TODO Change to default(_) if this is not a reference type */;
    protected static ShippingMethodsTblAdptr ShippingMethodsAdptr
    {
        get
        {
            _ShippingMethodsAdptr = new ShippingMethodsTblAdptr();
            return _ShippingMethodsAdptr;
        }
    }

    private static ShippingRatesTblAdptr _ShippingRatesAdptr = null/* TODO Change to default(_) if this is not a reference type */;
    protected static ShippingRatesTblAdptr ShippingRatesAdptr
    {
        get
        {
            _ShippingRatesAdptr = new ShippingRatesTblAdptr();
            return _ShippingRatesAdptr;
        }
    }

    private static ShippingZonesTblAdptr _ShippingZonesAdptr = null/* TODO Change to default(_) if this is not a reference type */;
    protected static ShippingZonesTblAdptr ShippingZonesAdptr
    {
        get
        {
            _ShippingZonesAdptr = new ShippingZonesTblAdptr();
            return _ShippingZonesAdptr;
        }
    }

    private static DestinationsTblAdptr _DestinationsAdptr = null/* TODO Change to default(_) if this is not a reference type */;
    protected static DestinationsTblAdptr DestinationsAdptr
    {
        get
        {
            _DestinationsAdptr = new DestinationsTblAdptr();
            return _DestinationsAdptr;
        }
    }

    public static DataTable GetShippingMethodsRatesByLanguage(int _DestinationID, decimal _Boundary, int _LanguageID)
    {
        return ShippingMethodsRatesAdptr.GetData(_DestinationID, _Boundary, _LanguageID);
    }
    public static DataTable GetShippingMethodsRatesByNameAndLanguage(string _ShippingName, int _DestinationID, decimal _Boundary, int _LanguageID)
    {
        return ShippingMethodsRatesAdptr.GetDataByName(_DestinationID, _Boundary, _ShippingName, _LanguageID);
    }

    public static DataTable _GetShippingMethdsByLanguage(byte _LanguageID)
    {
        return ShippingMethodsAdptr._GetByLanguage(_LanguageID);
    }

    public static string _GetShippingMethodNameByID(byte numMethodID, byte numLanguageID)
    {
        LanguageElementsBLL objLanguageElementsBLL = new LanguageElementsBLL();
        return objLanguageElementsBLL.GetElementValue(numLanguageID, LANG_ELEM_TABLE_TYPE.ShippingMethods, LANG_ELEM_FIELD_NAME.Name, numMethodID);
    }

    public static bool _AddNewShippingMethod(DataTable tblElements, bool blnLive, byte numOrderBy, byte numTaxBandID, byte numTaxBandID2, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddShippingMethod = sqlConn.CreateCommand;
            cmdAddShippingMethod.CommandText = "_spKartrisShippingMethods_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddShippingMethod.CommandType = CommandType.StoredProcedure;

            try
            {
                cmdAddShippingMethod.Parameters.AddWithValue("@SM_Live", blnLive);
                cmdAddShippingMethod.Parameters.AddWithValue("@SM_OrderByValue", numOrderBy);
                cmdAddShippingMethod.Parameters.AddWithValue("@SM_Tax", numTaxBandID);
                cmdAddShippingMethod.Parameters.AddWithValue("@SM_Tax2", numTaxBandID2);
                cmdAddShippingMethod.Parameters.AddWithValue("@SM_NewID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddShippingMethod.Transaction = savePoint;

                cmdAddShippingMethod.ExecuteNonQuery();

                if (cmdAddShippingMethod.Parameters("@SM_NewID").Value == null || cmdAddShippingMethod.Parameters("@SM_NewID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                long intNewShippingMethodID = cmdAddShippingMethod.Parameters("@SM_NewID").Value;
                if (!LanguageElementsBLL._AddLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.ShippingMethods, intNewShippingMethodID, sqlConn, savePoint))
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

    public static bool _UpdateShippingMethod(DataTable tblElements, byte numShippingMethodID, bool blnLive, byte numOrderBy, byte numTaxBandID, byte numTaxBandID2, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateShippingMethod = sqlConn.CreateCommand;
            cmdUpdateShippingMethod.CommandText = "_spKartrisShippingMethods_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateShippingMethod.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateShippingMethod.Parameters.AddWithValue("@SM_ID", numShippingMethodID);
                cmdUpdateShippingMethod.Parameters.AddWithValue("@SM_Live", blnLive);
                cmdUpdateShippingMethod.Parameters.AddWithValue("@SM_Tax", numTaxBandID);
                cmdUpdateShippingMethod.Parameters.AddWithValue("@SM_Tax2", numTaxBandID2);
                cmdUpdateShippingMethod.Parameters.AddWithValue("@SM_OrderByValue", numOrderBy);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateShippingMethod.Transaction = savePoint;

                cmdUpdateShippingMethod.ExecuteNonQuery();

                if (!LanguageElementsBLL._UpdateLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.ShippingMethods, numShippingMethodID, sqlConn, savePoint))
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

    public static bool _DeleteShippingMethod(long numShippingMethodID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdDeleteShippingMethod = sqlConn.CreateCommand;
            cmdDeleteShippingMethod.CommandText = "_spKartrisShippingMethods_Delete";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdDeleteShippingMethod.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdDeleteShippingMethod.Parameters.AddWithValue("@SM_ID", numShippingMethodID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdDeleteShippingMethod.Transaction = savePoint;

                cmdDeleteShippingMethod.ExecuteNonQuery();

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



    public static DataTable _GetRatesByMethodAndZone(byte ShippingMethodID, byte ShippingZoneID)
    {
        return ShippingRatesAdptr._GetByMethodAndZone(ShippingMethodID, ShippingZoneID);
    }
    public static DataTable _GetShippingZonesByMethod(byte numShippingMethodID)
    {
        return ShippingRatesAdptr._GetZonesByMethod(numShippingMethodID);
    }

    public static bool _UpdateShippingRate(int numShippingRateID, decimal numNewRate, string strShippingGateways, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateShippingRate = sqlConn.CreateCommand;
            cmdUpdateShippingRate.CommandText = "_spKartrisShippingRates_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateShippingRate.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateShippingRate.Parameters.AddWithValue("@S_ID", numShippingRateID);
                cmdUpdateShippingRate.Parameters.AddWithValue("@NewRate", numNewRate);
                cmdUpdateShippingRate.Parameters.AddWithValue("@S_ShippingGateways", strShippingGateways);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateShippingRate.Transaction = savePoint;

                cmdUpdateShippingRate.ExecuteNonQuery();

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
    public static bool _DeleteShippingRate(int numShippingRateID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdDeleteShippingRate = sqlConn.CreateCommand;
            cmdDeleteShippingRate.CommandText = "_spKartrisShippingRates_DeleteByID";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdDeleteShippingRate.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdDeleteShippingRate.Parameters.AddWithValue("@S_ID", numShippingRateID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdDeleteShippingRate.Transaction = savePoint;

                cmdDeleteShippingRate.ExecuteNonQuery();

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
    public static bool _DeleteShippingRateByMethodAndZone(byte numShippingMethodID, byte numShippingZoneID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdDeleteShippingRate = sqlConn.CreateCommand;
            cmdDeleteShippingRate.CommandText = "_spKartrisShippingRates_DeleteByMethodAndZone";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdDeleteShippingRate.CommandType = CommandType.StoredProcedure;

            try
            {
                cmdDeleteShippingRate.Parameters.AddWithValue("@SM_ID", numShippingMethodID);
                cmdDeleteShippingRate.Parameters.AddWithValue("@SZ_ID", numShippingZoneID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdDeleteShippingRate.Transaction = savePoint;

                cmdDeleteShippingRate.ExecuteNonQuery();

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
    public static bool _AddNewShippingRate(byte numShippingMethodID, byte numShippingZoneID, decimal numBoundary, decimal numRate, string strShippingGateways, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddShippingRate = sqlConn.CreateCommand;
            cmdAddShippingRate.CommandText = "_spKartrisShippingRates_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddShippingRate.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdAddShippingRate.Parameters.AddWithValue("@SM_ID", numShippingMethodID);
                cmdAddShippingRate.Parameters.AddWithValue("@SZ_ID", numShippingZoneID);
                cmdAddShippingRate.Parameters.AddWithValue("@S_Boundary", numBoundary);
                cmdAddShippingRate.Parameters.AddWithValue("@S_Rate", numRate);
                cmdAddShippingRate.Parameters.AddWithValue("@S_ShippingGateways", strShippingGateways);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddShippingRate.Transaction = savePoint;

                cmdAddShippingRate.ExecuteNonQuery();

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
    public static bool _CopyRates(byte numShippingMethodID, byte numFromZone, byte numToZone, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdCopyShippingRate = sqlConn.CreateCommand;
            cmdCopyShippingRate.CommandText = "_spKartrisShippingRates_Copy";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdCopyShippingRate.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdCopyShippingRate.Parameters.AddWithValue("@SM_ID", numShippingMethodID);
                cmdCopyShippingRate.Parameters.AddWithValue("@FromZone", numFromZone);
                cmdCopyShippingRate.Parameters.AddWithValue("@ToZone", numToZone);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdCopyShippingRate.Transaction = savePoint;

                cmdCopyShippingRate.ExecuteNonQuery();

                savePoint.Commit();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
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



    public static string _GetShippingZoneNameByID(byte numZoneID, byte numLanguageID)
    {
        string strZoneName = "";
        try
        {
            strZoneName = System.Convert.ToString(ShippingZonesAdptr._GetNameByID(numZoneID, numLanguageID).Rows(0)("ZoneName"));
        }
        catch (Exception ex)
        {
            strZoneName = "";
        }
        return strZoneName;
    }
    public static DataTable _GetShippingZonesByLanguage(byte _LanguageID)
    {
        return ShippingZonesAdptr._GetByLanguage(_LanguageID);
    }

    public static bool _AddNewShippingZone(DataTable tblElements, bool blnLive, byte numOrderBy, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddShippingZone = sqlConn.CreateCommand;
            cmdAddShippingZone.CommandText = "_spKartrisShippingZones_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddShippingZone.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdAddShippingZone.Parameters.AddWithValue("@SZ_Live", blnLive);
                cmdAddShippingZone.Parameters.AddWithValue("@SZ_OrderByValue", numOrderBy);
                cmdAddShippingZone.Parameters.AddWithValue("@SZ_NewID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddShippingZone.Transaction = savePoint;

                cmdAddShippingZone.ExecuteNonQuery();

                if (cmdAddShippingZone.Parameters("@SZ_NewID").Value == null || cmdAddShippingZone.Parameters("@SZ_NewID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                long intNewShippingZoneID = cmdAddShippingZone.Parameters("@SZ_NewID").Value;
                if (!LanguageElementsBLL._AddLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.ShippingZones, intNewShippingZoneID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                savePoint.Commit();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
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
    public static bool _UpdateShippingZone(DataTable tblElements, byte numShippingZoneID, bool blnLive, byte numOrderBy, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateShippingZone = sqlConn.CreateCommand;
            cmdUpdateShippingZone.CommandText = "_spKartrisShippingZones_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateShippingZone.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateShippingZone.Parameters.AddWithValue("@SZ_ID", numShippingZoneID);
                cmdUpdateShippingZone.Parameters.AddWithValue("@SZ_Live", blnLive);
                cmdUpdateShippingZone.Parameters.AddWithValue("@SZ_OrderByValue", numOrderBy);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateShippingZone.Transaction = savePoint;

                cmdUpdateShippingZone.ExecuteNonQuery();

                if (!LanguageElementsBLL._UpdateLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.ShippingZones, numShippingZoneID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                savePoint.Commit();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
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
    public static bool _DeleteShippingZone(byte numZoneID, byte numAssignedZoneID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdDeleteZone = sqlConn.CreateCommand;
            cmdDeleteZone.CommandText = "_spKartrisShippingZones_Delete";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdDeleteZone.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdDeleteZone.Parameters.AddWithValue("@ZoneID", numZoneID);
                cmdDeleteZone.Parameters.AddWithValue("@AssignedZoneID", numAssignedZoneID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdDeleteZone.Transaction = savePoint;

                cmdDeleteZone.ExecuteNonQuery();

                savePoint.Commit();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
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



    public static DataTable _GetDestinationsByLanguage(byte _LanguageID)
    {
        return DestinationsAdptr._GetByLanguage(_LanguageID);
    }
    public static DataTable _GetDestinationsByZone(byte _ZoneID, byte _LanguageID)
    {
        return DestinationsAdptr._GetDestinationsByZone(_ZoneID, _LanguageID);
    }
    public static DataTable _GetISOCodesForFilter()
    {
        return DestinationsAdptr._GetISOCodesForFilter();
    }
    public static int _GetTotalDestinationsByZone(byte numZoneID)
    {
        return System.Convert.ToInt32(DestinationsAdptr._GetTotalDestinationsByZone(numZoneID).Rows(0)("TotalDestinations"));
    }

    public static bool _UpdateDestination(DataTable tblElements, short numDesinationID, byte numZoneID, decimal decTax, decimal decTax2, string strISOCode, string strISOCode3Letters, string strISONumeric, string strRegion, bool blnLive, ref string strMsg, string strTaxExtra)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateDestination = sqlConn.CreateCommand;
            cmdUpdateDestination.CommandText = "_spKartrisDestinations_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateDestination.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateDestination.Parameters.AddWithValue("@D_ID", numDesinationID);
                cmdUpdateDestination.Parameters.AddWithValue("@D_ShippingZoneID", numZoneID);
                cmdUpdateDestination.Parameters.AddWithValue("@D_Tax", decTax);
                cmdUpdateDestination.Parameters.AddWithValue("@D_Tax2", FixNullToDB(decTax2, "z")); // z=decimal because d already taken for double
                cmdUpdateDestination.Parameters.AddWithValue("@D_ISOCode", FixNullToDB(strISOCode, "s"));
                cmdUpdateDestination.Parameters.AddWithValue("@D_ISOCode3Letter", FixNullToDB(strISOCode3Letters, "s"));
                cmdUpdateDestination.Parameters.AddWithValue("@D_ISOCodeNumeric", FixNullToDB(strISONumeric, "s"));
                cmdUpdateDestination.Parameters.AddWithValue("@D_Region", FixNullToDB(strRegion));
                cmdUpdateDestination.Parameters.AddWithValue("@D_Live", blnLive);
                cmdUpdateDestination.Parameters.AddWithValue("@D_TaxExtra", FixNullToDB(strTaxExtra, "s"));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateDestination.Transaction = savePoint;

                cmdUpdateDestination.ExecuteNonQuery();

                if (!LanguageElementsBLL._UpdateLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.Destination, numDesinationID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                savePoint.Commit();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
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
    public static bool _UpdateDestinationForTaxWizard(short numDesinationID, byte numZoneID, decimal decTax, decimal decTax2, string strISOCode, string strISOCode3Letters, string strISONumeric, string strRegion, bool blnLive, ref string strMsg, string strTaxExtra)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateDestination = sqlConn.CreateCommand;
            cmdUpdateDestination.CommandText = "_spKartrisDestinations_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateDestination.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateDestination.Parameters.AddWithValue("@D_ID", numDesinationID);
                cmdUpdateDestination.Parameters.AddWithValue("@D_ShippingZoneID", numZoneID);
                cmdUpdateDestination.Parameters.AddWithValue("@D_Tax", decTax);
                cmdUpdateDestination.Parameters.AddWithValue("@D_Tax2", FixNullToDB(decTax2, "z")); // z=decimal because d already taken for double
                cmdUpdateDestination.Parameters.AddWithValue("@D_ISOCode", FixNullToDB(strISOCode, "s"));
                cmdUpdateDestination.Parameters.AddWithValue("@D_ISOCode3Letter", FixNullToDB(strISOCode3Letters, "s"));
                cmdUpdateDestination.Parameters.AddWithValue("@D_ISOCodeNumeric", FixNullToDB(strISONumeric, "s"));
                cmdUpdateDestination.Parameters.AddWithValue("@D_Region", FixNullToDB(strRegion));
                cmdUpdateDestination.Parameters.AddWithValue("@D_Live", blnLive);
                cmdUpdateDestination.Parameters.AddWithValue("@D_TaxExtra", FixNullToDB(strTaxExtra, "s"));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateDestination.Transaction = savePoint;

                cmdUpdateDestination.ExecuteNonQuery();

                savePoint.Commit();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
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

    // Update v3.0001 for brexit, we now set base country
    public static string _GetISOCodeByDestinationID(byte numDestinationID)
    {
        string strISO = "";
        try
        {
            strISO = System.Convert.ToString(ShippingBLL.DestinationsAdptr._GetISOCodeByDestinationID(numDestinationID));
        }
        catch (Exception ex)
        {
            strISO = "";
        }
        return strISO;
    }
}
