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

using kartrisObjectConfigTableAdapters;
using System.Web.HttpContext;
using CkartrisFormatErrors;
using CkartrisDisplayFunctions;
using CkartrisEnumerations;
using CkartrisDataManipulation;
using KartSettingsManager;

public class ObjectConfigBLL
{
    private ObjectConfigTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;
    protected ObjectConfigTblAdptr Adptr
    {
        get
        {
            _Adptr = new ObjectConfigTblAdptr();
            return _Adptr;
        }
    }

    public DataTable _GetData()
    {
        return Adptr._GetData();
    }

    public object _GetValue(string strConfigName, long numParentID)
    {
        // Dim strValue As String = CStr(Adptr._GetValue(strConfigName, numParentID))
        return FixNullFromDB(Adptr._GetValue(strConfigName, numParentID));
    }

    /// <summary>
    ///     ''' Set object config value - Back end (uses object config ID)
    ///     ''' </summary>
    ///     ''' <param name="numConfigID">OC_ID</param>
    ///     ''' <param name="numParentID">ID of the parent item, e.g. product ID</param>
    ///     ''' <param name="strValue">The value you want to set it to</param>
    ///     ''' <param name="strMsg">Tax extra info, we now set this to EU if using UK/VAT tax regime for EU countries</param>
    ///     ''' <returns>boolean</returns>
    public bool _SetConfigValue(int numConfigID, long numParentID, string strValue, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisObjectConfig_SetValue";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                cmd.Parameters.AddWithValue("@ParentID", FixNullToDB(numParentID, "l"));
                cmd.Parameters.AddWithValue("@ConfigID", FixNullToDB(numConfigID, "i"));
                cmd.Parameters.AddWithValue("@ConfigValue", FixNullToDB(strValue));

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

    /// <summary>
    ///     ''' Set object config value - Front end (uses object config name)
    ///     ''' </summary>
    ///     ''' <param name="strConfigName">Name of the object config setting, e.g. K:user.eori</param>
    ///     ''' <param name="numParentID">ID of the parent item, e.g. user ID</param>
    ///     ''' <param name="strValue">The value you want to set it to</param>
    ///     ''' <param name="strMsg">Tax extra info, we now set this to EU if using UK/VAT tax regime for EU countries</param>
    ///     ''' <returns>boolean</returns>
    public bool SetConfigValue(string strConfigName, long numParentID, string strValue, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "spKartrisObjectConfig_SetValue";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                cmd.Parameters.AddWithValue("@ParentID", FixNullToDB(numParentID, "l"));
                cmd.Parameters.AddWithValue("@ConfigName", strConfigName);
                cmd.Parameters.AddWithValue("@ConfigValue", FixNullToDB(strValue));

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

    /// <summary>
    ///     ''' Get object config value
    ///     ''' </summary>
    ///     ''' <param name="strConfigName">Name of the object config setting, e.g. K:user.eori</param>
    ///     ''' <param name="numParentID">ID of the parent item, e.g. user ID</param>
    ///     ''' <returns>object</returns>
    ///     ''' <remarks>Since v3.1002 this does similar to language elements retrieval,
    ///     ''' it will try twice before failing</remarks>
    public object GetValue(string strConfigName, long numParentID)
    {
        try
        {
            return Adptr.GetValue(strConfigName, numParentID);
        }
        catch (Exception ex)
        {

            // Wait a little, then try again
            System.Threading.Thread.Sleep(20);
            try
            {
                return Adptr.GetValue(strConfigName, numParentID);
            }
            catch (Exception ex2)
            {
                // Ok, maybe something more going on.
                CkartrisFormatErrors.LogError("ObjectConfigBLL.GetValue - " + ex.Message + Constants.vbCrLf + "strConfigName: " + strConfigName + Constants.vbCrLf + "numParentID: " + numParentID);
            }
        }
        return null;
    }
}
