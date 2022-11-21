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

using kartrisMediaDataTableAdapters;
using CkartrisEnumerations;
using CkartrisBLL;
using CkartrisFormatErrors;
using System.Web.HttpContext;
using CkartrisDataManipulation;
using KartSettingsManager;

public class MediaBLL
{
    private static MediaLinksTblAdptr _MediaLinksAdptr = null/* TODO Change to default(_) if this is not a reference type */;
    protected static MediaLinksTblAdptr MediaLinksAdptr
    {
        get
        {
            _MediaLinksAdptr = new MediaLinksTblAdptr();
            return _MediaLinksAdptr;
        }
    }

    private static MediaTypesTblAdptr _MediaTypesAdptr = null/* TODO Change to default(_) if this is not a reference type */;
    protected static MediaTypesTblAdptr MediaTypesAdptr
    {
        get
        {
            _MediaTypesAdptr = new MediaTypesTblAdptr();
            return _MediaTypesAdptr;
        }
    }

    public static DataTable _GetMediaLinksByParent(int intParentID, string strParentType)
    {
        return MediaLinksAdptr._GetByParent(intParentID, strParentType);
    }
    public static DataTable GetMediaLinksByParent(int intParentID, string strParentType)
    {
        return MediaLinksAdptr.GetByParent(intParentID, strParentType);
    }
    public static DataTable _GetMediaLinkByID(int numMediaLinkID)
    {
        return MediaLinksAdptr._GetByID(numMediaLinkID);
    }

    public static DataTable _GetMediaTypes()
    {
        return MediaTypesAdptr._GetData();
    }
    public static DataTable _GetMediaTypesByID(int numMediaTypeID)
    {
        return MediaTypesAdptr._GetByID(numMediaTypeID);
    }

    public static void _ChangeMediaLinkOrder(int numMediaLinkID, int numParentID, char chrParentType, char chrDirection)
    {
        MediaLinksAdptr._ChangeSortValue(numMediaLinkID, numParentID, chrParentType, chrDirection);
    }
    public static bool _AddMediaLink(int numParentID, char chrParentType, string strEmbedSource, short numMediaTypeID, int numHeight, int numWidth, bool blnDownloadable, string strParameter, bool blnLive, ref int NewID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisMediaLinks_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@ParentID", FixNullToDB(numParentID, "i"));
                cmd.Parameters.AddWithValue("@ParentType", FixNullToDB(chrParentType, "c"));
                cmd.Parameters.AddWithValue("@EmbedSource", FixNullToDB(strEmbedSource));
                cmd.Parameters.AddWithValue("@MediaTypeID", FixNullToDB(numMediaTypeID, "i"));
                cmd.Parameters.AddWithValue("@Height", FixNullToDB(numHeight, "i"));
                cmd.Parameters.AddWithValue("@Width", FixNullToDB(numWidth, "i"));
                cmd.Parameters.AddWithValue("@Downloadable", FixNullToDB(blnDownloadable, "b"));
                cmd.Parameters.AddWithValue("@Parameters", FixNullToDB(strParameter));
                cmd.Parameters.AddWithValue("@Live", FixNullToDB(blnLive, "b"));
                cmd.Parameters.AddWithValue("@NewML_ID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;
                cmd.ExecuteNonQuery();

                if (cmd.Parameters("@NewML_ID").Value == null || cmd.Parameters("@NewML_ID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                NewID = cmd.Parameters("@NewML_ID").Value;

                savePoint.Commit();
                sqlConn.Close();
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
    public static bool _UpdateMediaLink(int MediaLinkID, string strEmbedSource, short numMediaTypeID, int numHeight, int numWidth, bool blnDownloadable, string strParameter, bool blnLive, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisMediaLinks_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@ML_ID", FixNullToDB(MediaLinkID, "i"));
                cmd.Parameters.AddWithValue("@EmbedSource", FixNullToDB(strEmbedSource));
                cmd.Parameters.AddWithValue("@MediaTypeID", FixNullToDB(numMediaTypeID, "i"));
                cmd.Parameters.AddWithValue("@Height", FixNullToDB(numHeight, "i"));
                cmd.Parameters.AddWithValue("@Width", FixNullToDB(numWidth, "i"));
                cmd.Parameters.AddWithValue("@Downloadable", FixNullToDB(blnDownloadable, "b"));
                cmd.Parameters.AddWithValue("@Parameters", FixNullToDB(strParameter));
                cmd.Parameters.AddWithValue("@Live", FixNullToDB(blnLive, "b"));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;
                cmd.ExecuteNonQuery();
                savePoint.Commit();
                sqlConn.Close();
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
    public static bool _DeleteMediaLink(int MediaLinkID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisMediaLinks_Delete";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@ML_ID", FixNullToDB(MediaLinkID, "i"));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;
                cmd.ExecuteNonQuery();
                savePoint.Commit();
                sqlConn.Close();
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

    public static bool _AddMediaType(string strMediaType, int numDefaultHeight, int numDefaultWidth, string strDefaultParameters, bool blnDownloadable, bool blnEmbed, bool blnInline, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisMediaTypes_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@Extension", FixNullToDB(strMediaType));
                cmd.Parameters.AddWithValue("@DefaultHeight", FixNullToDB(numDefaultHeight, "i"));
                cmd.Parameters.AddWithValue("@DefaultWidth", FixNullToDB(numDefaultWidth, "i"));
                cmd.Parameters.AddWithValue("@DefaultParameters", FixNullToDB(strDefaultParameters));
                cmd.Parameters.AddWithValue("@Downloadable", FixNullToDB(blnDownloadable, "b"));
                cmd.Parameters.AddWithValue("@Embed", FixNullToDB(blnEmbed, "b"));
                cmd.Parameters.AddWithValue("@Inline", FixNullToDB(blnInline, "b"));
                cmd.Parameters.AddWithValue("@NewMT_ID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;
                cmd.ExecuteNonQuery();

                if (cmd.Parameters("@NewMT_ID").Value == null || cmd.Parameters("@NewMT_ID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                savePoint.Commit();
                sqlConn.Close();
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
    public static bool _UpdateMediaType(int numDefaultHeight, int numDefaultWidth, string strDefaultParameters, bool blnDownloadable, bool blnEmbed, bool blnInline, int MediaTypeID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisMediaTypes_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@MT_ID", FixNullToDB(MediaTypeID, "i"));
                cmd.Parameters.AddWithValue("@DefaultHeight", FixNullToDB(numDefaultHeight, "i"));
                cmd.Parameters.AddWithValue("@DefaultWidth", FixNullToDB(numDefaultWidth, "i"));
                cmd.Parameters.AddWithValue("@DefaultParameters", FixNullToDB(strDefaultParameters));
                cmd.Parameters.AddWithValue("@Downloadable", FixNullToDB(blnDownloadable, "b"));
                cmd.Parameters.AddWithValue("@Embed", FixNullToDB(blnEmbed, "b"));
                cmd.Parameters.AddWithValue("@Inline", FixNullToDB(blnInline, "b"));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;
                cmd.ExecuteNonQuery();
                savePoint.Commit();
                sqlConn.Close();
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
    public static bool _DeleteMediaType(int MediaTypeID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisMediaTypes_Delete";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@MT_ID", FixNullToDB(MediaTypeID, "i"));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;
                cmd.ExecuteNonQuery();
                savePoint.Commit();
                sqlConn.Close();
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
}
