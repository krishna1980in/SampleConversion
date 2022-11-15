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

using kartrisCouponsData;
using kartrisCouponsDataTableAdapters;
using CkartrisFormatErrors;
using System.Web.HttpContext;
using CkartrisDataManipulation;


public class CouponsBLL
{
    private static CouponsTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;

    protected static CouponsTblAdptr Adptr
    {
        get
        {
            _Adptr = new CouponsTblAdptr();
            return _Adptr;
        }
    }

    public static DataTable _GetCouponGroups()
    {
        return Adptr._GetCouponGroups();
    }
    public static DataRow _GetCouponByID(short numCouponID)
    {
        DataTable tblCoupon = Adptr._GetByID(numCouponID);
        if (tblCoupon.Rows.Count == 1)
            return tblCoupon.Rows(0);
        return null/* TODO Change to default(_) if this is not a reference type */;
    }
    public static DataTable _GetCouponsByDate(DateTime datCreationDate)
    {
        return Adptr._GetByDate(DateTime.Year(datCreationDate), DateTime.Month(datCreationDate), DateTime.Day(datCreationDate));
    }
    public static string _GenerateNewCouponCode()
    {
        short numRandomElement1;
        short numRandomElement2;
        short numRandomElement3;
        short numRandomElement4;
        short numRandomElement5;
        string strCouponCode = "";

        do
        {
            VBMath.Randomize();
            numRandomElement1 = Conversion.Int(VBMath.Rnd() * 26);
            VBMath.Randomize();
            numRandomElement2 = Conversion.Int(VBMath.Rnd() * 26);
            VBMath.Randomize();
            numRandomElement3 = Conversion.Int(VBMath.Rnd() * 26);
            VBMath.Randomize();
            numRandomElement4 = Conversion.Int(VBMath.Rnd() * 26);
            VBMath.Randomize();
            numRandomElement5 = Conversion.Int(VBMath.Rnd() * 26);
            strCouponCode = Strings.Chr(numRandomElement1 + 65) + Strings.Chr(numRandomElement2 + 65) + Strings.Chr(numRandomElement3 + 65) + Strings.Chr(numRandomElement4 + 65) + Strings.Chr(numRandomElement5 + 65);
        }
        while (_IsCouponCodeExist(strCouponCode));

        return strCouponCode;
    }
    public static bool _IsCouponCodeExist(string strCouponCode)
    {
        if (strCouponCode == "")
            return true;

        return Adptr._GetByCouponCode(strCouponCode).Rows.Count > 0;
    }
    public static DataTable _SearchByCouponCode(string strCouponCode)
    {
        return Adptr._SearchByCode(strCouponCode);
    }
    public static short _GetByCouponCode(string strCouponCode)
    {
        try
        {
            return FixNullFromDB(Adptr._GetByCouponCode(strCouponCode).Rows(0)("CP_ID"));
        }
        catch (Exception ex)
        {
            return -1;
        }
    }

    public static bool _AddNewCoupons(string strCouponCode, float sngDiscountValue, char chrDiscountType, DateTime datStartDate, DateTime datEndDate, int numQty, bool blnReusable, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddCoupon = sqlConn.CreateCommand;
            cmdAddCoupon.CommandText = "_spKartrisCoupons_Add";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddCoupon.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddCoupon.Transaction = savePoint;
                string[] strCodes = new string[numQty + 1];
                if (strCouponCode == "")
                {
                    for (int i = 0; i <= numQty - 1; i++)
                        strCodes[i] = _GenerateNewCouponCode();
                }
                bool blnCouponIsFixed = false;
                for (int i = 1; i <= numQty; i++)
                {
                    if (strCouponCode == "")
                        strCouponCode = strCodes[i - 1];
                    else
                    {
                        if (_IsCouponCodeExist(strCouponCode))
                            throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Coupons", "ContentText_CouponCodeAlreadyInUse"));
                        blnCouponIsFixed = true;
                    }

                    cmdAddCoupon.Parameters.Clear();
                    cmdAddCoupon.Parameters.AddWithValue("@CouponCode", strCouponCode);
                    cmdAddCoupon.Parameters.AddWithValue("@Reusable", blnReusable);
                    cmdAddCoupon.Parameters.AddWithValue("@StartDate", datStartDate);
                    cmdAddCoupon.Parameters.AddWithValue("@EndDate", datEndDate);
                    cmdAddCoupon.Parameters.AddWithValue("@DiscountValue", sngDiscountValue);
                    cmdAddCoupon.Parameters.AddWithValue("@DiscountType", chrDiscountType);
                    cmdAddCoupon.Parameters.AddWithValue("@CouponCodeIsFixed", blnCouponIsFixed);
                    cmdAddCoupon.Parameters.AddWithValue("@NowOffset", CkartrisDisplayFunctions.NowOffset);

                    cmdAddCoupon.ExecuteNonQuery();
                    strCouponCode = "";
                }
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

    public static bool _UpdateCoupon(short numCouponID, float sngDiscountValue, char chrDiscountType, DateTime datStartDate, DateTime datEndDate, bool blnReusable, bool blnLive, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddCoupon = sqlConn.CreateCommand;
            cmdAddCoupon.CommandText = "_spKartrisCoupons_Update";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddCoupon.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddCoupon.Transaction = savePoint;

                cmdAddCoupon.Parameters.AddWithValue("@CouponID", numCouponID);
                cmdAddCoupon.Parameters.AddWithValue("@Reusable", blnReusable);
                cmdAddCoupon.Parameters.AddWithValue("@StartDate", datStartDate);
                cmdAddCoupon.Parameters.AddWithValue("@EndDate", datEndDate);
                cmdAddCoupon.Parameters.AddWithValue("@DiscountValue", sngDiscountValue);
                cmdAddCoupon.Parameters.AddWithValue("@DiscountType", chrDiscountType);
                cmdAddCoupon.Parameters.AddWithValue("@Live", blnLive);

                cmdAddCoupon.ExecuteNonQuery();
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

    public static bool _UpdateCouponStatus(short numCouponID, bool blnEnabled, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateCoupon = sqlConn.CreateCommand;
            cmdUpdateCoupon.CommandText = "_spKartrisCoupons_UpdateStatus";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateCoupon.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateCoupon.Transaction = savePoint;

                cmdUpdateCoupon.Parameters.AddWithValue("@CouponID", numCouponID);
                cmdUpdateCoupon.Parameters.AddWithValue("@Enabled", blnEnabled);

                cmdUpdateCoupon.ExecuteNonQuery();
                savePoint.Commit();
                return true;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
                if (!savePoint == null)
                    savePoint.Rollback();
                return false;
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

    public static bool _DeleteCoupon(short numCouponID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdDeleteCoupon = sqlConn.CreateCommand;
            cmdDeleteCoupon.CommandText = "_spKartrisCoupons_Delete";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdDeleteCoupon.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdDeleteCoupon.Transaction = savePoint;

                cmdDeleteCoupon.Parameters.AddWithValue("@CouponID", numCouponID);

                cmdDeleteCoupon.ExecuteNonQuery();
                savePoint.Commit();
                return true;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
                if (!savePoint == null)
                    savePoint.Rollback();
                return false;
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
}
