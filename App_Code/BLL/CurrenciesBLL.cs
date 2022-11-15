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
using CkartrisEnumerations;
using CkartrisFormatErrors;
using kartrisCurrenciesData;
using kartrisCurrenciesDataTableAdapters;
using KartSettingsManager;

public class CurrenciesBLL
{
    private static CurrenciesTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;

    protected static CurrenciesTblAdptr Adptr
    {
        get
        {
            _Adptr = new CurrenciesTblAdptr();
            return _Adptr;
        }
    }

    public static short CurrencyID(string _CurrencyCode)
    {
        DataRow[] rowCurrencies = GetCurrenciesFromCache().Select("CUR_ISOCode = '" + _CurrencyCode + "'");
        return System.Convert.ToInt16(rowCurrencies[0]("CUR_ID"));
    }

    public static char CurrencySymbol(short _CurrencyID)
    {
        DataRow[] rowCurrencies = GetCurrenciesFromCache().Select("CUR_ID = " + _CurrencyID);
        return System.Convert.ToChar(rowCurrencies[0]("CUR_Symbol"));
    }

    public static string CurrencyCode(short _CurrencyID)
    {
        DataRow[] rowCurrencies = GetCurrenciesFromCache().Select("CUR_ID = " + _CurrencyID);
        return System.Convert.ToString(Trim(rowCurrencies[0]("CUR_ISOCode")));
    }

    public static decimal CurrencyRate(short _CurrencyID)
    {
        DataRow[] rowCurrencies = GetCurrenciesFromCache().Select("CUR_ID = " + _CurrencyID);
        if (rowCurrencies.Length > 0)
            return System.Convert.ToDecimal(rowCurrencies[0]("CUR_ExchangeRate"));
        return 0;
    }

    public static decimal ConvertCurrency(short _ToCurrencyID, decimal _FromValue, short _FromCurrencyID = 0)
    {
        try
        {
            // Have seen situations where if a server restarts and the SQL is
            // slow coming up, this can error out with an 'out of bounds' error.
            // In that case, let's try to recycle app pool.
            if (_FromCurrencyID == 0)
                _FromCurrencyID = GetDefaultCurrency();
            decimal decFromRate = 0;
            decimal decToRate = 0;
            DataRow[] rowCurrenciesFrom = GetCurrenciesFromCache().Select("CUR_ID = " + _FromCurrencyID);
            decFromRate = System.Convert.ToDecimal(FixNullFromDB(rowCurrenciesFrom[0]("CUR_ExchangeRate")));
            DataRow[] rowCurrenciesTo = GetCurrenciesFromCache().Select("CUR_ID = " + _ToCurrencyID);
            decToRate = System.Convert.ToDecimal(FixNullFromDB(rowCurrenciesTo[0]("CUR_ExchangeRate")));
            return (_FromValue / (double)decFromRate) * decToRate;
        }
        catch (Exception ex)
        {
            CkartrisBLL.RecycleAppPool();
            return 0;
        }
    }

    public static string FormatCurrencyPrice(short _CurrencyID, decimal _Price, bool blnShowSymbol = true, bool blnIncludeLeftDirectionTag = true)
    {
        try
        {
            // Have seen situations where if a server restarts and the SQL is
            // slow coming up, this can error out with an 'out of bounds' error.
            // In that case, let's try to recycle app pool.
            if (KartSettingsManager.GetKartConfig("frontend.users.access") == "partial")
            {
                // We check path of page, as we only want to obscure prices for front end 
                // if 'partial'
                string strFullOriginalPath = HttpContext.Current.Request.Url.ToString();

                if (!HttpContext.Current.User.Identity.IsAuthenticated & !strFullOriginalPath.ToLower().Contains("/admin"))
                {
                    // Change text to hidden message
                    try
                    {
                        return System.Web.HttpContext.GetGlobalResourceObject("Kartris", "ContentText_HiddenPriceText");
                    }
                    catch
                    {
                        return "XXXX";
                    }
                    return;
                }
            }

            DataRow[] rowCurrencies;
            rowCurrencies = GetCurrenciesFromCache().Select("CUR_ID=" + _CurrencyID);
            string strSymbol = rowCurrencies[0]("CUR_Symbol");
            string strResult = "";
            System.Text.StringBuilder sbdZeros = new System.Text.StringBuilder("");
            _Price = decimal.Round(System.Convert.ToDecimal(_Price), rowCurrencies[0]("CUR_RoundNumbers"));

            if (FixNullFromDB(rowCurrencies[0]("CUR_RoundNumbers")) > 0)
            {
                for (byte i = 0; i <= FixNullFromDB(rowCurrencies[0]("CUR_RoundNumbers")) - 1; i++)
                {
                    if (sbdZeros.ToString() == "")
                        sbdZeros.Append(".");
                    sbdZeros.Append("0");
                }
            }

            string strFormatedPrice;
            strFormatedPrice = Strings.Format(_Price, "##0" + sbdZeros.ToString());
            strFormatedPrice = Replace(strFormatedPrice, ".", FixNullFromDB(rowCurrencies[0]("CUR_DecimalPoint")));


            if (blnShowSymbol)
            {
                // We're formatting the value as text, with a currencysymbol
                if (rowCurrencies[0]("CUR_Format").ToString.IndexOf("[symbol]") < rowCurrencies[0]("CUR_Format").ToString.IndexOf("[value]"))
                {
                    strResult = strSymbol;
                    if (rowCurrencies[0]("CUR_Format").ToString.IndexOf(" ") != -1)
                        strResult += " ";
                    strResult += strFormatedPrice;
                }
                else
                {
                    strResult = strFormatedPrice;
                    if (rowCurrencies[0]("CUR_Format").ToString.IndexOf(" ") != -1)
                        strResult += " ";
                    strResult += strSymbol;
                }

                // RtL support is to ensure that currency formatting is correct for languages like Arabic
                if (GetKartConfig("general.prices.rtlsupport") == "y" && blnIncludeLeftDirectionTag)
                    strResult = "<span dir=\"ltr\">" + strResult + "</span>";
            }
            else
                // Returns a price without any non-numeric parts
                // (spaces, currency symbol, etc.)
                strResult += strFormatedPrice;

            return strResult;
        }
        catch (Exception ex)
        {
            CkartrisBLL.RecycleAppPool();
            return 0;
        }
    }

    public static DataRow[] _GetByCurrencyID(byte CurrencyID)
    {
        return GetCurrenciesFromCache().Select("CUR_ID=" + CurrencyID);
    }

    public static byte GetDefaultCurrency()
    {
        var LowestOrderNo = (from a in GetCurrenciesFromCache()
                             where a.Field<bool>("CUR_Live") == true
                             select a.Field<byte>("CUR_OrderNo")).Min();

        DataRow[] rowCurrencies = GetCurrenciesFromCache().Select("CUR_OrderNo = " + LowestOrderNo);
        byte numMinID = System.Convert.ToByte(rowCurrencies[0]("CUR_ID"));
        for (int i = 1; i <= rowCurrencies.Length - 1; i++)
            numMinID = Math.Min(Math.Min(numMinID, System.Convert.ToByte(rowCurrencies[i - 1]("CUR_ID"))), System.Convert.ToByte(rowCurrencies[i]("CUR_ID")));
        return numMinID;
    }

    public static DataTable _GetCurrencies()
    {
        return Adptr._GetData();
    }

    public static bool _AddNewCurrency(DataTable tblElements, string strSymbol, string strIsoCode, string strIsoCodeNumeric, decimal numExchangeRate, bool blnHasDecimal, bool blnLive, string strFormat, string strIsoFormat, char chrDecimalPoint, byte numRoundNumbers, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddCurrency = sqlConn.CreateCommand;
            cmdAddCurrency.CommandText = "_spKartrisCurrencies_Add";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddCurrency.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdAddCurrency.Parameters.AddWithValue("@CUR_Symbol", FixNullToDB(strSymbol));
                cmdAddCurrency.Parameters.AddWithValue("@CUR_ISOCode", FixNullToDB(strIsoCode));
                cmdAddCurrency.Parameters.AddWithValue("@CUR_ISOCodeNumeric", FixNullToDB(strIsoCodeNumeric));
                cmdAddCurrency.Parameters.AddWithValue("@CUR_ExchangeRate", FixNullToDB(numExchangeRate, "z"));
                cmdAddCurrency.Parameters.AddWithValue("@CUR_HasDecimals", FixNullToDB(blnHasDecimal, "b"));
                cmdAddCurrency.Parameters.AddWithValue("@CUR_Live", FixNullToDB(blnLive, "b"));
                cmdAddCurrency.Parameters.AddWithValue("@CUR_Format", strFormat);
                cmdAddCurrency.Parameters.AddWithValue("@CUR_IsoFormat", strIsoFormat);
                cmdAddCurrency.Parameters.AddWithValue("@CUR_DecimalPoint", FixNullToDB(chrDecimalPoint, "c"));
                cmdAddCurrency.Parameters.AddWithValue("@CUR_RoundNumbers", numRoundNumbers);
                cmdAddCurrency.Parameters.AddWithValue("@CUR_NewID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddCurrency.Transaction = savePoint;

                cmdAddCurrency.ExecuteNonQuery();
                long intNewCurrencyID = cmdAddCurrency.Parameters("@CUR_NewID").Value;
                if (!LanguageElementsBLL._AddLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.Currencies, intNewCurrencyID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Currency, strMsg, CreateQuery(cmdAddCurrency), intNewCurrencyID, sqlConn, savePoint);

                savePoint.Commit();
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

    public static bool _UpdateCurrency(DataTable tblElements, byte numCurrencyID, string strSymbol, string strIsoCode, string strIsoCodeNumeric, decimal numExchangeRate, bool blnHasDecimal, bool blnLive, string strFormat, string strIsoFormat, char chrDecimalPoint, byte numRoundNumbers, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateCurrency = sqlConn.CreateCommand;
            cmdUpdateCurrency.CommandText = "_spKartrisCurrencies_Update";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateCurrency.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateCurrency.Parameters.AddWithValue("@CUR_Symbol", FixNullToDB(strSymbol));
                cmdUpdateCurrency.Parameters.AddWithValue("@CUR_ISOCode", FixNullToDB(strIsoCode));
                cmdUpdateCurrency.Parameters.AddWithValue("@CUR_ISOCodeNumeric", FixNullToDB(strIsoCodeNumeric));
                cmdUpdateCurrency.Parameters.AddWithValue("@CUR_ExchangeRate", FixNullToDB(numExchangeRate, "z"));
                cmdUpdateCurrency.Parameters.AddWithValue("@CUR_HasDecimals", FixNullToDB(blnHasDecimal, "b"));
                cmdUpdateCurrency.Parameters.AddWithValue("@CUR_Live", FixNullToDB(blnLive, "b"));
                cmdUpdateCurrency.Parameters.AddWithValue("@CUR_Format", strFormat);
                cmdUpdateCurrency.Parameters.AddWithValue("@CUR_IsoFormat", strIsoFormat);
                cmdUpdateCurrency.Parameters.AddWithValue("@CUR_DecimalPoint", FixNullToDB(chrDecimalPoint, "c"));
                cmdUpdateCurrency.Parameters.AddWithValue("@CUR_RoundNumbers", numRoundNumbers);
                cmdUpdateCurrency.Parameters.AddWithValue("@Original_CUR_ID", numCurrencyID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateCurrency.Transaction = savePoint;

                cmdUpdateCurrency.ExecuteNonQuery();
                if (!LanguageElementsBLL._UpdateLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.Currencies, numCurrencyID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Currency, strMsg, CreateQuery(cmdUpdateCurrency), numCurrencyID, sqlConn, savePoint);

                savePoint.Commit();
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

    public static bool _DeleteCurrency(byte CurrencyID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdDeleteCurrency = sqlConn.CreateCommand;
            cmdDeleteCurrency.CommandText = "_spKartrisCurrencies_Delete";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdDeleteCurrency.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdDeleteCurrency.Parameters.AddWithValue("@Original_CUR_ID", CurrencyID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdDeleteCurrency.Transaction = savePoint;

                cmdDeleteCurrency.ExecuteNonQuery();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Currency, strMsg, CreateQuery(cmdDeleteCurrency), CurrencyID, sqlConn, savePoint);
                savePoint.Commit();
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

    public static bool _UpdateCurrencyRate(byte CurrencyID, decimal numNewRate)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateCurrencyRates = sqlConn.CreateCommand;
            cmdUpdateCurrencyRates.CommandText = "_spKartrisCurrencies_UpdateRates";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateCurrencyRates.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateCurrencyRates.Parameters.AddWithValue("@CUR_ExchangeRate", numNewRate);
                cmdUpdateCurrencyRates.Parameters.AddWithValue("@Original_CUR_ID", CurrencyID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateCurrencyRates.Transaction = savePoint;

                cmdUpdateCurrencyRates.ExecuteNonQuery();
                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Currency, System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully"), CreateQuery(cmdUpdateCurrencyRates), null/* TODO Change to default(_) if this is not a reference type */, sqlConn, savePoint);

                savePoint.Commit();
                return true;
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
        return false;
    }

    public static bool _SetDefault(byte CurrencyID, ref string strMessage)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisCurrencies_SetDefault";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@CUR_ID", CurrencyID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                cmd.ExecuteNonQuery();
                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Currency, System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully"), CreateQuery(cmd), null/* TODO Change to default(_) if this is not a reference type */, sqlConn, savePoint);

                savePoint.Commit();
                return true;
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
        return false;
    }
}
