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

using kartrisReviewsData;
using kartrisReviewsDataTableAdapters;
using CkartrisDataManipulation;
using CkartrisFormatErrors;
using System.Web.HttpContext;

public class ReviewsBLL
{
    private static ReviewsTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;
    protected static ReviewsTblAdptr Adptr
    {
        get
        {
            _Adptr = new ReviewsTblAdptr();
            return _Adptr;
        }
    }
    public static DataTable _GetReviews()
    {
        return Adptr._GetData();
    }
    public static DataTable _GetReviewsByProductID(int _ProductID, byte _LanguageID)
    {
        return Adptr._GetReviewsByProductID(_ProductID, _LanguageID);
    }
    public static DataTable GetReviewsByProductID(int _ProductID, byte _LanguageID)
    {
        return Adptr.GetReviewsByProductID(_ProductID, _LanguageID);
    }
    public static DataTable _GetReviewsByLanguage(byte _LanguageID)
    {
        return Adptr._GetByLanguage(_LanguageID);
    }
    public static DataTable _GetReviewByID(short ReviewID)
    {
        return Adptr._GetByID(ReviewID);
    }

    public static bool AddNewReview(int ProductID, byte LanguageID, string strTitle, string strText, byte bytRating, string strName, string strEmail, string strLocation, int numCustomerID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddNewReview = sqlConn.CreateCommand;
            cmdAddNewReview.CommandText = "_spKartrisReviews_Add";
            cmdAddNewReview.CommandType = CommandType.StoredProcedure;
            try
            {
                char _Live;
                _Live = IIf(KartSettingsManager.GetKartConfig("frontend.reviews.autopostreviews") == "y", "y", "a");

                cmdAddNewReview.Parameters.AddWithValue("@ProductID", ProductID);
                cmdAddNewReview.Parameters.AddWithValue("@LanguageID", LanguageID);
                cmdAddNewReview.Parameters.AddWithValue("@CustomerID", FixNullToDB(numCustomerID, "i"));
                cmdAddNewReview.Parameters.AddWithValue("@Title", FixNullToDB(strTitle));
                cmdAddNewReview.Parameters.AddWithValue("@Text", FixNullToDB(strText));
                cmdAddNewReview.Parameters.AddWithValue("@Rating", System.Convert.ToByte(FixNullToDB(bytRating, "i")));
                cmdAddNewReview.Parameters.AddWithValue("@Name", FixNullToDB(strName));
                cmdAddNewReview.Parameters.AddWithValue("@Email", FixNullToDB(strEmail));
                cmdAddNewReview.Parameters.AddWithValue("@Location", FixNullToDB(strLocation));
                cmdAddNewReview.Parameters.AddWithValue("@Live", FixNullToDB(_Live, "c"));
                cmdAddNewReview.Parameters.AddWithValue("@NowOffset", CkartrisDisplayFunctions.NowOffset);

                sqlConn.Open();

                cmdAddNewReview.ExecuteNonQuery();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
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
    public static bool _UpdateReview(short ReviewID, byte LanguageID, string strTitle, string strText, byte bytRating, string strName, string strEmail, string strLocation, char chrLive, int numCustomerID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateReview = sqlConn.CreateCommand;
            cmdUpdateReview.CommandText = "_spKartrisReviews_Update";
            cmdUpdateReview.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateReview.Parameters.AddWithValue("@LanguageID", LanguageID);
                cmdUpdateReview.Parameters.AddWithValue("@CustomerID", FixNullToDB(numCustomerID, "i"));
                cmdUpdateReview.Parameters.AddWithValue("@Title", FixNullToDB(strTitle, "s"));
                cmdUpdateReview.Parameters.AddWithValue("@Text", FixNullToDB(strText, "s"));
                cmdUpdateReview.Parameters.AddWithValue("@Rating", bytRating);
                cmdUpdateReview.Parameters.AddWithValue("@Name", FixNullToDB(strName, "s"));
                cmdUpdateReview.Parameters.AddWithValue("@Email", FixNullToDB(strEmail, "s"));
                cmdUpdateReview.Parameters.AddWithValue("@Location", FixNullToDB(strLocation, "s"));
                cmdUpdateReview.Parameters.AddWithValue("@Live", chrLive);
                cmdUpdateReview.Parameters.AddWithValue("@Original_ID", ReviewID);
                cmdUpdateReview.Parameters.AddWithValue("@NowOffset", CkartrisDisplayFunctions.NowOffset);

                sqlConn.Open();

                cmdUpdateReview.ExecuteNonQuery();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
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
    public static bool _DeleteReview(short ReviewID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdDeleteReview = sqlConn.CreateCommand;
            cmdDeleteReview.CommandText = "_spKartrisReviews_Delete";
            cmdDeleteReview.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdDeleteReview.Parameters.AddWithValue("@Original_ID", ReviewID);

                sqlConn.Open();

                cmdDeleteReview.ExecuteNonQuery();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
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
}
