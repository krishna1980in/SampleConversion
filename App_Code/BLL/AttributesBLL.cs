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

using kartrisAttributesData;
using kartrisAttributesDataTableAdapters;
using System.Web.HttpContext;
using CkartrisEnumerations;
using CkartrisFormatErrors;

public class AttributesBLL
{
    private static AttributesTblAdpt _Adptr = null/* TODO Change to default(_) if this is not a reference type */;
    private static AttributeValuesTblAdptr _AdptrValues = null/* TODO Change to default(_) if this is not a reference type */;
    private static AttributeOptionsTblAdpt _AdptrOptions = null/* TODO Change to default(_) if this is not a reference type */;
    private static AttributeProductOptionsTblAdpt _AdptrProductOption = null/* TODO Change to default(_) if this is not a reference type */;

    protected static AttributesTblAdpt Adptr
    {
        get
        {
            _Adptr = new AttributesTblAdpt();
            return _Adptr;
        }
    }
    protected static AttributeValuesTblAdptr AdptrValues
    {
        get
        {
            _AdptrValues = new AttributeValuesTblAdptr();
            return _AdptrValues;
        }
    }

    protected static AttributeOptionsTblAdpt AdptrOptions
    {
        get
        {
            _AdptrOptions = new AttributeOptionsTblAdpt();
            return _AdptrOptions;
        }
    }

    protected static AttributeProductOptionsTblAdpt AdptrProductOptions
    {
        get
        {
            _AdptrProductOption = new AttributeProductOptionsTblAdpt();
            return _AdptrProductOption;
        }
    }

    public static DataTable GetAttributesByCategoryId(int numCategoryId)
    {
        return Adptr._GetByCategoryId(numCategoryId);
    }

    public static DataTable GetSummaryAttributesByProductID(int _ProductID, short _LanguageID)
    {
        return Adptr.GetSummaryByProductID(_ProductID, _LanguageID);
    }

    public static DataTable GetSpecialAttributesByProductID(int _ProductID, short _LanguageID)
    {
        return Adptr._GetSpecialByProductID(_ProductID, _LanguageID);
    }

    public static DataTable _GetAttributesByLanguage(byte languageID)
    {
        return Adptr._GetByLanguage(languageID);
    }

    public static DataTable _GetAttributeValuesByProduct(int intProductID)
    {
        return AdptrValues._GetByProductID(intProductID);
    }

    public static DataTable _GetByAttributeID(int numAttributeID)
    {
        return Adptr._GetByAttributeID(numAttributeID);
    }

    public static DataTable GetOptionsByAttributeID(int numAttributeID)
    {
        return AdptrOptions.GetDataBy(numAttributeID);
    }

    /// <summary>
    ///     ''' Get the Attribute Options that have been assigned to a given product
    ///     ''' </summary>
    ///     ''' <param name="ProductId">The product to check</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DataTable GetAttributeOptionsByProductID(int ProductId)
    {
        return AdptrProductOptions._GetByProductID(ProductId);
    }

    /// <summary>
    ///     ''' Add new attribute option to the product so that we have an option for the product
    ///     ''' </summary>
    ///     ''' <param name="AttributeOptionID"></param>
    ///     ''' <param name="ProductId"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static int _AddAttributeProductOption(int AttributeOptionID, int ProductId, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddAttribute = sqlConn.CreateCommand;
            cmdAddAttribute.CommandText = "_spKartrisAttributeProductOption_Add";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddAttribute.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdAddAttribute.Parameters.AddWithValue("@AttributeOptionId", AttributeOptionID);
                cmdAddAttribute.Parameters.AddWithValue("@ProductId", ProductId);
                cmdAddAttribute.Parameters.AddWithValue("@AttributeProductId", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddAttribute.Transaction = savePoint;

                cmdAddAttribute.ExecuteNonQuery();

                if (cmdAddAttribute.Parameters("@AttributeProductId").Value == null || cmdAddAttribute.Parameters("@AttributeProductId").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                int intNewAttributeProductOptionID = cmdAddAttribute.Parameters("@AttributeProductId").Value;


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
    ///     ''' Update an existing attribute option
    ///     ''' </summary>
    ///     ''' <param name="tblElements"></param>
    ///     ''' <param name="numAttributeID"></param>
    ///     ''' <param name="OrderBy"></param>
    ///     ''' <param name="strMsg"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks>Attribute options are the selectable check box items under an attribute (e.g. the colours under the attribute 'color')</remarks>
    public static bool _UpdateAttributeOption(DataTable tblElements, int numAttributeID, int OrderBy, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddAttribute = sqlConn.CreateCommand;
            cmdAddAttribute.CommandText = "_spKartrisAttributeOption_Update";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddAttribute.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdAddAttribute.Parameters.AddWithValue("@AttributeOptionId", numAttributeID);
                cmdAddAttribute.Parameters.AddWithValue("@OrderByValue", OrderBy);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddAttribute.Transaction = savePoint;

                cmdAddAttribute.ExecuteNonQuery();

                int intAttributeOptionID = cmdAddAttribute.Parameters("@AttributeOptionId").Value;
                if (!LanguageElementsBLL._UpdateLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.AttributeOption, intAttributeOptionID, sqlConn, savePoint))
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

    /// <summary>
    ///     ''' Add a new attribute option to the database
    ///     ''' </summary>
    ///     ''' <param name="tblElements"></param>
    ///     ''' <param name="numAttributeID"></param>
    ///     ''' <param name="OrderBy"></param>
    ///     ''' <param name="strMsg"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks>Attribute options are the selectable check box items under an attribute (e.g. the colours under the attribute 'color')</remarks>
    public static bool _AddNewAttributeOption(DataTable tblElements, int numAttributeID, int OrderBy, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddAttribute = sqlConn.CreateCommand;
            cmdAddAttribute.CommandText = "_spKartrisAttributeOption_Add";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddAttribute.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdAddAttribute.Parameters.AddWithValue("@AttributeId", numAttributeID);
                cmdAddAttribute.Parameters.AddWithValue("@OrderByValue", OrderBy);
                cmdAddAttribute.Parameters.AddWithValue("@NewAttributeOption_ID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddAttribute.Transaction = savePoint;

                cmdAddAttribute.ExecuteNonQuery();

                if (cmdAddAttribute.Parameters("@NewAttributeOption_ID").Value == null || cmdAddAttribute.Parameters("@NewAttributeOption_ID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                int intNewAttributeOptionID = cmdAddAttribute.Parameters("@NewAttributeOption_ID").Value;
                if (!LanguageElementsBLL._AddLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.AttributeOption, intNewAttributeOptionID, sqlConn, savePoint))
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

    /// <summary>
    ///     ''' Delete an option within an attribute
    ///     ''' </summary>
    ///     ''' <param name="AttributeOptionID"></param>
    ///     ''' <param name="strMsg">Pointer to error message</param>
    ///     ''' <returns>True if operation successful</returns>
    ///     ''' <remarks></remarks>
    public static bool _DeleteAttributeOption(int AttributeOptionID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateAttribute = sqlConn.CreateCommand;
            cmdUpdateAttribute.CommandText = "_spKartrisAttributeOption_Delete";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateAttribute.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateAttribute.Parameters.AddWithValue("@AttributeOptionId", AttributeOptionID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateAttribute.Transaction = savePoint;

                cmdUpdateAttribute.ExecuteNonQuery();

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
    ///     ''' Delete all product options for a given attribute and product
    ///     ''' </summary>
    ///     ''' <param name="AttributeId"></param>
    ///     ''' <param name="strMsg"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static bool _DeleteAttributeProductOptions(int AttributeId, int ProductId, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateAttribute = sqlConn.CreateCommand;
            cmdUpdateAttribute.CommandText = "_spKartrisAttributeProductOptions_DeleteByAttributeIdAndProductId";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateAttribute.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateAttribute.Parameters.AddWithValue("@AttributeId", AttributeId);
                cmdUpdateAttribute.Parameters.AddWithValue("@ProductId", ProductId);
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateAttribute.Transaction = savePoint;

                cmdUpdateAttribute.ExecuteNonQuery();

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

    public static bool _AddNewAttribute(DataTable tblElements, char chrType, bool blnLive, bool blnFastEntry, bool blnShowFront, bool blnShowSearch, byte numOrderNo, char chrCompare, bool blnSpecial, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddAttribute = sqlConn.CreateCommand;
            cmdAddAttribute.CommandText = "_spKartrisAttributes_Add";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddAttribute.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdAddAttribute.Parameters.AddWithValue("@Type", chrType);
                cmdAddAttribute.Parameters.AddWithValue("@Live", blnLive);
                cmdAddAttribute.Parameters.AddWithValue("@FastEntry", blnFastEntry);
                cmdAddAttribute.Parameters.AddWithValue("@ShowFrontend", blnShowFront);
                cmdAddAttribute.Parameters.AddWithValue("@ShowSearch", blnShowSearch);
                cmdAddAttribute.Parameters.AddWithValue("@OrderByValue", numOrderNo);
                cmdAddAttribute.Parameters.AddWithValue("@Compare", chrCompare);
                cmdAddAttribute.Parameters.AddWithValue("@Special", blnSpecial);
                cmdAddAttribute.Parameters.AddWithValue("@NewAttribute_ID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddAttribute.Transaction = savePoint;

                cmdAddAttribute.ExecuteNonQuery();

                if (cmdAddAttribute.Parameters("@NewAttribute_ID").Value == null || cmdAddAttribute.Parameters("@NewAttribute_ID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                int intNewAttributeID = cmdAddAttribute.Parameters("@NewAttribute_ID").Value;
                if (!LanguageElementsBLL._AddLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.Attributes, intNewAttributeID, sqlConn, savePoint))
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

    public static bool _UpdateAttribute(DataTable tblElements, int numAttributeID, char chrType, bool blnLive, bool blnFastEntry, bool blnShowFront, bool blnShowSearch, byte numOrderNo, char chrCompare, bool blnSpecial, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateAttribute = sqlConn.CreateCommand;
            cmdUpdateAttribute.CommandText = "_spKartrisAttributes_Update";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateAttribute.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateAttribute.Parameters.AddWithValue("@Original_AttributeID", numAttributeID);
                cmdUpdateAttribute.Parameters.AddWithValue("@Type", chrType);
                cmdUpdateAttribute.Parameters.AddWithValue("@Live", blnLive);
                cmdUpdateAttribute.Parameters.AddWithValue("@FastEntry", blnFastEntry);
                cmdUpdateAttribute.Parameters.AddWithValue("@ShowFrontend", blnShowFront);
                cmdUpdateAttribute.Parameters.AddWithValue("@ShowSearch", blnShowSearch);
                cmdUpdateAttribute.Parameters.AddWithValue("@OrderByValue", numOrderNo);
                cmdUpdateAttribute.Parameters.AddWithValue("@Compare", chrCompare);
                cmdUpdateAttribute.Parameters.AddWithValue("@Special", blnSpecial);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateAttribute.Transaction = savePoint;

                cmdUpdateAttribute.ExecuteNonQuery();

                if (!LanguageElementsBLL._UpdateLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.Attributes, numAttributeID, sqlConn, savePoint))
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

    public static bool _DeleteAttribute(int numAttributeID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateAttribute = sqlConn.CreateCommand;
            cmdUpdateAttribute.CommandText = "_spKartrisAttributes_Delete";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateAttribute.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateAttribute.Parameters.AddWithValue("@AttributeID", numAttributeID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateAttribute.Transaction = savePoint;

                cmdUpdateAttribute.ExecuteNonQuery();

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

    public static bool _UpdateAttributeValues(int intProductID, DataTable tblProductAttributes, DataTable tblLanguageElements, DataTable tblAttributeOptions, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();

                // ' 1. DELETE From The Product's Attribute Values
                if (!_DeleteAttributeValuesByProductID(intProductID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                // ' 2. CREATE New Attribute Values
                foreach (DataRow row in tblProductAttributes.Rows)
                {
                    int ProductID = System.Convert.ToInt32(row("ProductID"));
                    int AttributeID = System.Convert.ToInt32(row("AttributeID"));
                    string AttributeType = row("AttributeType");
                    int AttributeValueID = _AddAttributeValue(ProductID, AttributeID, sqlConn, savePoint);

                    if (AttributeValueID != 0)
                    {
                        if (AttributeType == "t")
                        {
                            // Text type attribute
                            DataTable tblValuesLanguageElements = new DataTable();
                            tblValuesLanguageElements = tblLanguageElements.Select("ProductID=" + ProductID + " AND AttributeID=" + AttributeID).CopyToDataTable();
                            tblValuesLanguageElements.Columns.Remove("ProductID");
                            tblValuesLanguageElements.Columns.Remove("AttributeID");

                            if (!LanguageElementsBLL._AddLanguageElements(tblValuesLanguageElements, LANG_ELEM_TABLE_TYPE.AttributeValues, AttributeValueID, sqlConn, savePoint))
                                // ' 3. CREATE The Language Element for each one.
                                throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                        }
                        else if (AttributeType == "c")
                        {
                            // checkbox or Yes/No datatype
                            // First delete all existing entries, then reinsert them (no 'update' function)
                            if (_DeleteAttributeProductOptions(AttributeID, ProductID, ref strMsg))
                            {
                                foreach (DataRow dr in tblAttributeOptions.Select("AttributeID = " + AttributeID))
                                    // Loop through all rows for this attribute.
                                    _AddAttributeProductOption(dr("AttributeOptionId"), ProductID, ref strMsg);
                            }
                            else
                                throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                        }
                    }
                    else
                        throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                }
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

    private static bool _DeleteAttributeValuesByProductID(int intProductID, SqlConnection sqlConn, SqlTransaction savePoint)
    {
        try
        {
            SqlCommand cmdDelete = new SqlCommand("_spKartrisAttributeValues_DeleteByProductID", sqlConn, savePoint);
            cmdDelete.CommandType = CommandType.StoredProcedure;
            cmdDelete.Parameters.AddWithValue("@ProductID", intProductID);
            cmdDelete.ExecuteNonQuery();
            return true;
        }
        // End Using
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
        return false;
    }

    private static int _AddAttributeValue(int intProductID, int numAttributeID, SqlConnection sqlConn, SqlTransaction savePoint)
    {
        try
        {
            SqlCommand cmdAdd = new SqlCommand("_spKartrisAttributeValues_Add", sqlConn, savePoint);
            cmdAdd.CommandType = CommandType.StoredProcedure;
            cmdAdd.Parameters.AddWithValue("@ProductID", intProductID);
            cmdAdd.Parameters.AddWithValue("@AttributeID", numAttributeID);
            cmdAdd.Parameters.AddWithValue("@NewAttributeValue_ID", 0).Direction = ParameterDirection.Output;
            cmdAdd.ExecuteNonQuery();

            if (cmdAdd.Parameters("@NewAttributeValue_ID").Value == null || cmdAdd.Parameters("@NewAttributeValue_ID").Value == DBNull.Value)
                throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

            return System.Convert.ToInt32(cmdAdd.Parameters("@NewAttributeValue_ID").Value);
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
            if (!savePoint == null)
                savePoint.Rollback();
        }
        return 0;
    }

    public static DataTable _GetPotentialAttributeOptionsByProduct(int ProductId, int AttributeId)
    {
        _GetPotentialAttributeOptionsByProduct = null/* TODO Change to default(_) if this is not a reference type */;
        string strMsg = "";

        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            string cmdStr = "_spDeadlineAttributeOptionsByOptionId";
            SqlCommand cmd = new SqlCommand(cmdStr, sqlConn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@P_ID", ProductId);
            cmd.Parameters.AddWithValue("@Attribute_ID", AttributeId);
            try
            {
                sqlConn.Open();
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
            }
            finally
            {
                sqlConn.Close();
                cmd.Dispose();
            }
        }
    }
}
