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
using System.Data;

using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using System.Web.HttpContext;
using kartrisOptionsDataTableAdapters;
using CkartrisEnumerations;
using CkartrisFormatErrors;

public class OptionsBLL
{
    private static OptionGroupsTblAdptr _OptionGroupAdptr = null/* TODO Change to default(_) if this is not a reference type */;
    private static OptionsTblAdptr _OptionAdptr = null/* TODO Change to default(_) if this is not a reference type */;
    private static ProductOptionGroupLinkTblAdptr _ProductOptionGroupAdptr = null/* TODO Change to default(_) if this is not a reference type */;
    private static ProductOptionLinkTblAdptr _ProductOptionsAdptr = null/* TODO Change to default(_) if this is not a reference type */;


    protected static OptionGroupsTblAdptr OptionGroupAdptr
    {
        get
        {
            _OptionGroupAdptr = new OptionGroupsTblAdptr();
            return _OptionGroupAdptr;
        }
    }

    protected static OptionsTblAdptr OptionAdptr
    {
        get
        {
            _OptionAdptr = new OptionsTblAdptr();
            return _OptionAdptr;
        }
    }

    protected static ProductOptionGroupLinkTblAdptr ProductOptionGroupAdptr
    {
        get
        {
            _ProductOptionGroupAdptr = new ProductOptionGroupLinkTblAdptr();
            return _ProductOptionGroupAdptr;
        }
    }

    protected static ProductOptionLinkTblAdptr ProductOptionAdptr
    {
        get
        {
            _ProductOptionsAdptr = new ProductOptionLinkTblAdptr();
            return _ProductOptionsAdptr;
        }
    }

    public static DataTable _GetOptionSchema()
    {
        DataTable tblTemp = OptionAdptr._GetData();
        tblTemp.Clear();
        return tblTemp;
    }

    public static DataTable _GetOptionGroupSchema()
    {
        DataTable tblTemp = OptionGroupAdptr._GetData();
        tblTemp.Clear();
        return tblTemp;
    }

    public static DataTable _GetProductOptionSchema()
    {
        DataTable tblTemp = ProductOptionAdptr.GetData();
        tblTemp.Clear();
        return tblTemp;
    }

    public static DataTable _GetProductGroupSchema()
    {
        DataTable tblTemp = ProductOptionGroupAdptr.GetData();
        tblTemp.Clear();
        return tblTemp;
    }

    public static DataTable _GetOptionGroups()
    {
        return OptionGroupAdptr._GetData();
    }

    public static DataTable _GetOptionGroupPage(byte pPageIndx, int pRowsPerPage, ref int pTotalGroups)
    {
        return OptionGroupAdptr._GetGroupsPage(pPageIndx, pRowsPerPage, pTotalGroups);
    }

    public static DataTable _GetOptionsByGroupID(int pGrpID, byte pLanguageID)
    {
        return OptionAdptr._GetByGroupID(pLanguageID, pGrpID);
    }

    public static DataTable _GetOptionGroupsByProductID(int pProductID)
    {
        return ProductOptionGroupAdptr._GetByProductID(pProductID);
    }

    public static DataTable _GetOptionsByProductID(int pProductID)
    {
        return ProductOptionAdptr._GetByProductID(pProductID);
    }

    public static DataTable _GetOptionsByProductAndGroup(int pProductID, int pGroupID, byte pLanguageID)
    {
        return ProductOptionAdptr._GetOptionsByProductAndGroup(pProductID, pGroupID, pLanguageID);
    }

    public static bool _AddOptionGrp(string pBackendName, char pDisplayType, int pOrderByValue, DataTable ptblElements, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdOptionGrp = sqlConn.CreateCommand;
            cmdOptionGrp.CommandText = "_spKartrisOptionGroups_Add";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdOptionGrp.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdOptionGrp.Parameters.AddWithValue("@OPTG_BackendName", pBackendName);
                cmdOptionGrp.Parameters.AddWithValue("@OPTG_OptionDisplayType", pDisplayType);
                cmdOptionGrp.Parameters.AddWithValue("@OPTG_DefOrderByValue", pOrderByValue);
                cmdOptionGrp.Parameters.AddWithValue("@NewID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdOptionGrp.Transaction = savePoint;

                cmdOptionGrp.ExecuteNonQuery();

                if (cmdOptionGrp.Parameters("@NewID").Value == DBNull.Value || cmdOptionGrp.Parameters("@NewID").Value == null)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                int numNewGrpID = System.Convert.ToInt32(cmdOptionGrp.Parameters("@NewID").Value);

                numNewGrpID = System.Convert.ToInt32(cmdOptionGrp.Parameters("@NewID").Value);
                if (!LanguageElementsBLL._AddLanguageElements(ptblElements, LANG_ELEM_TABLE_TYPE.OptionGroups, numNewGrpID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                savePoint.Commit();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
                return true;
            }
            catch (Exception ex)
            {
                if (!savePoint == null)
                    savePoint.Rollback();
                strMsg = ex.Message;
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
    public static bool _UpdateOptionGrp(int pOptionGrpID, string pBackendName, char pDisplayType, int pOrderByValue, DataTable ptblElements, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdOptionGrp = sqlConn.CreateCommand;
            cmdOptionGrp.CommandText = "_spKartrisOptionGroups_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdOptionGrp.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdOptionGrp.Parameters.AddWithValue("@OPTG_BackendName", pBackendName);
                cmdOptionGrp.Parameters.AddWithValue("@OPTG_OptionDisplayType", pDisplayType);
                cmdOptionGrp.Parameters.AddWithValue("@OPTG_DefOrderByValue", pOrderByValue);
                cmdOptionGrp.Parameters.AddWithValue("@Original_OPTG_ID", pOptionGrpID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdOptionGrp.Transaction = savePoint;

                cmdOptionGrp.ExecuteNonQuery();

                if (!LanguageElementsBLL._UpdateLanguageElements(ptblElements, LANG_ELEM_TABLE_TYPE.OptionGroups, pOptionGrpID, sqlConn, savePoint))
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
                strMsg = ex.Message;
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
    public static bool _DeleteOptionGrp(int pOptionGrpID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdOptionGrp = sqlConn.CreateCommand;
            cmdOptionGrp.CommandText = "_spKartrisOptionGroups_Delete";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdOptionGrp.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdOptionGrp.Parameters.AddWithValue("@OPTG_ID", pOptionGrpID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdOptionGrp.Transaction = savePoint;

                cmdOptionGrp.ExecuteNonQuery();

                savePoint.Commit();
                sqlConn.Close();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
                return true;
            }
            catch (Exception ex)
            {
                if (!savePoint == null)
                    savePoint.Rollback();
                strMsg = ex.Message;
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

    public static bool _AddOption(int pOptionGrpID, bool pSelected, float pPriceChange, float pWeightChange, int pOrderByValue, DataTable ptblElements, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdOptionGrp = sqlConn.CreateCommand;
            cmdOptionGrp.CommandText = "_spKartrisOptions_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdOptionGrp.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdOptionGrp.Parameters.AddWithValue("@OPT_OptionGroupID", pOptionGrpID);
                cmdOptionGrp.Parameters.AddWithValue("@OPT_CheckBoxValue", pSelected);
                cmdOptionGrp.Parameters.AddWithValue("@OPT_DefPriceChange", pPriceChange);
                cmdOptionGrp.Parameters.AddWithValue("@OPT_DefWeightChange", pWeightChange);
                cmdOptionGrp.Parameters.AddWithValue("@OPT_DefOrderByValue", pOrderByValue);
                cmdOptionGrp.Parameters.AddWithValue("@NewID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdOptionGrp.Transaction = savePoint;

                cmdOptionGrp.ExecuteNonQuery();

                if (cmdOptionGrp.Parameters("@NewID").Value == DBNull.Value || cmdOptionGrp.Parameters("@NewID").Value == null)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                int numNewOptionID = System.Convert.ToInt32(cmdOptionGrp.Parameters("@NewID").Value);

                if (!LanguageElementsBLL._AddLanguageElements(ptblElements, LANG_ELEM_TABLE_TYPE.Options, numNewOptionID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

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
    public static bool _UpdateOption(int pOriginalOptionID, int pOptionGrpID, bool pSelected, float pPriceChange, float pWeightChange, int pOrderByValue, DataTable ptblElements, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdOptionGrp = sqlConn.CreateCommand;
            cmdOptionGrp.CommandText = "_spKartrisOptions_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdOptionGrp.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdOptionGrp.Parameters.AddWithValue("@OPT_OptionGroupID", pOptionGrpID);
                cmdOptionGrp.Parameters.AddWithValue("@OPT_CheckBoxValue", pSelected);
                cmdOptionGrp.Parameters.AddWithValue("@OPT_DefPriceChange", pPriceChange);
                cmdOptionGrp.Parameters.AddWithValue("@OPT_DefWeightChange", pWeightChange);
                cmdOptionGrp.Parameters.AddWithValue("@OPT_DefOrderByValue", pOrderByValue);
                cmdOptionGrp.Parameters.AddWithValue("@Original_OPT_ID", pOriginalOptionID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdOptionGrp.Transaction = savePoint;

                cmdOptionGrp.ExecuteNonQuery();

                if (!LanguageElementsBLL._UpdateLanguageElements(ptblElements, LANG_ELEM_TABLE_TYPE.Options, pOriginalOptionID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

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
    public static bool _DeleteOption(int pOptionID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdOptionGrp = sqlConn.CreateCommand;
            cmdOptionGrp.CommandText = "_spKartrisOptions_Delete";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdOptionGrp.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdOptionGrp.Parameters.AddWithValue("@OPT_ID", pOptionID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdOptionGrp.Transaction = savePoint;

                cmdOptionGrp.ExecuteNonQuery();

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

    public static bool _CreateProductOptions(int pProductID, DataTable ptblOptionGroupList, DataTable ptblOptionsList, ref string strMsg)
    {
        // ' To Create/Re-Create Options for the Product
        // '  --> 1. Delete the existing product's Options from ProductOptionLink.
        // '      2. Delete the existing product's OptionGroups from ProductOptionGroupLink.
        // '      3. Suspend the product's versions in the Versions Table
        // '      4. Create/Re-Create new records for the Product's Options & OptionGroups.
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();

                if (!_DeleteProductOptionsByProductID(pProductID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                if (!_DeleteProductOptionGroupByProductID(pProductID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                if (!_SuspendProductVersions(pProductID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                if (!_AddProductOptionGroupLink(ptblOptionGroupList, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                if (!_AddProductOptionLink(ptblOptionsList, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
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

    private static bool _AddProductOptionGroupLink(DataTable tblProductOptionGroup, SqlConnection sqlConn, SqlTransaction savePoint)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("_spKartrisProductOptionGroupLink_Add", sqlConn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = savePoint;
            foreach (DataRow row in tblProductOptionGroup.Rows)
            {
                cmd.Parameters.AddWithValue("@ProductID", System.Convert.ToInt32(row("P_OPTG_ProductID")));
                cmd.Parameters.AddWithValue("@GroupID", System.Convert.ToInt16(row("P_OPTG_OptionGroupID")));
                cmd.Parameters.AddWithValue("@OrderBy", System.Convert.ToInt32(row("P_OPTG_OrderByValue")));
                cmd.Parameters.AddWithValue("@MustSelect", System.Convert.ToBoolean(row("P_OPTG_MustSelected")));
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            return true;
        }
        // End Using
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }

        return false;
    }
    private static bool _AddProductOptionLink(DataTable tblProductOptionLink, SqlConnection sqlConn, SqlTransaction savePoint)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("_spKartrisProductOptionLink_Add", sqlConn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = savePoint;
            foreach (DataRow row in tblProductOptionLink.Rows)
            {
                cmd.Parameters.AddWithValue("@OptionID", System.Convert.ToInt32(row("P_OPT_OptionID")));
                cmd.Parameters.AddWithValue("@ProductID", System.Convert.ToInt32(row("P_OPT_ProductID")));
                cmd.Parameters.AddWithValue("@OrderBy", System.Convert.ToInt32(row("P_OPT_OrderByValue")));
                cmd.Parameters.AddWithValue("@PriceChange", System.Convert.ToDecimal(row("P_OPT_PriceChange")));
                cmd.Parameters.AddWithValue("@WeightChange", System.Convert.ToDouble(row("P_OPT_WeightChange")));
                cmd.Parameters.AddWithValue("@Selected", System.Convert.ToBoolean(row("P_OPT_Selected")));
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            return true;
        }
        // End Using
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }

        return false;
    }
    public static bool _DeleteProductOptionsByProductID(int pProductID, SqlConnection pConn, SqlTransaction savePoint)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("_spKartrisProductOptionLink_DeleteByProductID", pConn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = savePoint;
            cmd.Parameters.AddWithValue("@ProductID", pProductID);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }

        return false;
    }
    public static bool _DeleteProductOptionGroupByProductID(int pProductID, SqlConnection pConn, SqlTransaction savePoint)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("_spKartrisProductOptionGroupLink_DeleteByProductID", pConn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = savePoint;
            cmd.Parameters.AddWithValue("@ProductID", pProductID);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }

        return false;
    }
    private static bool _SuspendProductVersions(int pProductID, SqlConnection pConn, SqlTransaction savePoint)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("_spKartrisVersions_SuspendProductVersions", pConn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = savePoint;
            cmd.Parameters.AddWithValue("@P_ID", pProductID);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }

        return false;
    }

    // In v2.9006 we add a new feature, that means when updating an option,
    // you can choose to have all products that have option values for this
    // option to have their price and weight change values reset from the
    // change you make here.
    public static bool _UpdateOptionValues(int pOptionID, decimal decPriceChange, decimal decWeightChange)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdOption = sqlConn.CreateCommand;
            cmdOption.CommandText = "_spKartrisProductOptionLink_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdOption.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdOption.Parameters.AddWithValue("@P_OPT_OptionID", pOptionID);
                cmdOption.Parameters.AddWithValue("@P_OPT_PriceChange", decPriceChange);
                cmdOption.Parameters.AddWithValue("@P_OPT_WeightChange", decWeightChange);
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdOption.Transaction = savePoint;

                cmdOption.ExecuteNonQuery();


                savePoint.Commit();
                sqlConn.Close();
                return true;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), "Error updating option values in bulk for existing products");
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
