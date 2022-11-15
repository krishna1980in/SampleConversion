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

using CkartrisFormatErrors;
using CkartrisDataManipulation;
using System.Web.HttpContext;
using CkartrisEnumerations;
using kartrisConfigData;
using kartrisConfigDataTableAdapters;


public class ConfigBLL
{
    private static ConfigTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;
    private static ConfigCacheTblAdptr _CacheAdptr = null/* TODO Change to default(_) if this is not a reference type */;

    protected static ConfigTblAdptr Adptr
    {
        get
        {
            _Adptr = new ConfigTblAdptr();
            return _Adptr;
        }
    }
    protected static ConfigCacheTblAdptr CacheAdptr
    {
        get
        {
            _CacheAdptr = new ConfigCacheTblAdptr();
            return _CacheAdptr;
        }
    }

    public static DataTable GetConfigCacheData()
    {
        return CacheAdptr._GetConfigCacheData();
    }

    public static string _GetConfigDesc(string ConfigName)
    {
        return System.Convert.ToString(Adptr._GetConfigDesc(ConfigName).Rows(0)("CFG_Description"));
    }
    public static DataTable _SearchConfig(string _ConfigKey, bool _ImportantConfig)
    {
        return Adptr._SearchConfig(_ConfigKey, _ImportantConfig);
    }
    public static DataTable _GetConfigByName(string _CFG_Name)
    {
        return Adptr._GetConfigByName(_CFG_Name);
    }
    public static DataTable _GetImportantConfig()
    {
        return Adptr._GetImportantConfig();
    }

    public static bool _UpdateConfigValue(string ConfigName, string ConfigValue, bool blnAddAdminLog = true, bool blnRefreshCache = true)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateConfigValue = sqlConn.CreateCommand;
            cmdUpdateConfigValue.CommandText = "_spKartrisConfig_UpdateConfigValue";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateConfigValue.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateConfigValue.Parameters.AddWithValue("@CFG_Name", FixNullToDB(ConfigName));
                cmdUpdateConfigValue.Parameters.AddWithValue("@CFG_Value", FixNullToDB(ConfigValue));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateConfigValue.Transaction = savePoint;

                cmdUpdateConfigValue.ExecuteNonQuery();

                if (blnAddAdminLog)
                    KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Config, System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully"), CreateQuery(cmdUpdateConfigValue), ConfigName, sqlConn, savePoint);

                savePoint.Commit();
                sqlConn.Close();
                if (blnRefreshCache)
                    KartSettingsManager.RefreshCache();
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
                    sqlConn.Close();
            }
        }

        return false;
    }

    public static bool _AddConfig(string pName, string pValue, string pDataType, string pDisplayType, string pDisplayInfo, string pDesc, string pDefaultValue, float pVersionAdded, bool pImportant, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlClient.SqlConnection sqlConn = new SqlClient.SqlConnection(strConnString))
        {
            SqlCommand cmdAddConfig = sqlConn.CreateCommand;
            cmdAddConfig.CommandText = "_spKartrisConfig_Add";

            SqlClient.SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddConfig.CommandType = CommandType.StoredProcedure;

            try
            {
                cmdAddConfig.Parameters.AddWithValue("@CFG_Name", FixNullToDB(pName));
                cmdAddConfig.Parameters.AddWithValue("@CFG_Value", FixNullToDB(pValue));
                cmdAddConfig.Parameters.AddWithValue("@CFG_DataType", FixNullToDB(pDataType));
                cmdAddConfig.Parameters.AddWithValue("@CFG_DisplayType", FixNullToDB(pDisplayType));
                cmdAddConfig.Parameters.AddWithValue("@CFG_DisplayInfo", FixNullToDB(pDisplayInfo));
                cmdAddConfig.Parameters.AddWithValue("@CFG_Description", FixNullToDB(pDesc));
                cmdAddConfig.Parameters.AddWithValue("@CFG_VersionAdded", FixNullToDB(pVersionAdded, "g"));
                cmdAddConfig.Parameters.AddWithValue("@CFG_DefaultValue", FixNullToDB(pDefaultValue));
                cmdAddConfig.Parameters.AddWithValue("@CFG_Important", FixNullToDB(pImportant, "b"));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddConfig.Transaction = savePoint;
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
                cmdAddConfig.ExecuteNonQuery();
                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Config, strMsg, CreateQuery(cmdAddConfig), pName, sqlConn, savePoint);

                savePoint.Commit();
                sqlConn.Close();

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

    public static int _UpdateConfig(string pName, string pValue, string pDataType, string pDisplayType, string pDisplayInfo, string pDesc, string pDefaultValue, float pVersionAdded, bool pImportant, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlClient.SqlConnection sqlConn = new SqlClient.SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateConfig = sqlConn.CreateCommand;
            cmdUpdateConfig.CommandText = "_spKartrisConfig_Update";

            SqlClient.SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateConfig.CommandType = CommandType.StoredProcedure;

            try
            {
                cmdUpdateConfig.Parameters.AddWithValue("@CFG_Value", FixNullToDB(pValue));
                cmdUpdateConfig.Parameters.AddWithValue("@CFG_DataType", FixNullToDB(pDataType));
                cmdUpdateConfig.Parameters.AddWithValue("@CFG_DisplayType", FixNullToDB(pDisplayType));
                cmdUpdateConfig.Parameters.AddWithValue("@CFG_DisplayInfo", FixNullToDB(pDisplayInfo));
                cmdUpdateConfig.Parameters.AddWithValue("@CFG_Description", FixNullToDB(pDesc));
                cmdUpdateConfig.Parameters.AddWithValue("@CFG_VersionAdded", FixNullToDB(pVersionAdded, "g"));
                cmdUpdateConfig.Parameters.AddWithValue("@CFG_DefaultValue", FixNullToDB(pDefaultValue));
                cmdUpdateConfig.Parameters.AddWithValue("@CFG_Important", FixNullToDB(pImportant, "b"));
                cmdUpdateConfig.Parameters.AddWithValue("@Original_CFG_Name", FixNullToDB(pName));

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateConfig.Transaction = savePoint;

                cmdUpdateConfig.ExecuteNonQuery();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Config, strMsg, CreateQuery(cmdUpdateConfig), pName, sqlConn, savePoint);

                savePoint.Commit();
                sqlConn.Close();

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
                    sqlConn.Close();
            }
        }

        return false;
    }
}
