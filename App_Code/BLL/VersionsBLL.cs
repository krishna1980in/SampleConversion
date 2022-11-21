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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using CkartrisEnumerations;
using CkartrisBLL;
using CkartrisFormatErrors;
using System.Web.HttpContext;
using kartrisVersionsData;
using kartrisVersionsDataTableAdapters;
using CkartrisDataManipulation;
using KartSettingsManager;

public class VersionsBLL
{
    private VersionsTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;
    protected VersionsTblAdptr Adptr
    {
        get
        {
            _Adptr = new VersionsTblAdptr();
            return _Adptr;
        }
    }

    private VersionOptionLinkTblAdptr _AdptrVersionOption = null/* TODO Change to default(_) if this is not a reference type */;
    protected VersionOptionLinkTblAdptr AdptrVersionOption
    {
        get
        {
            _AdptrVersionOption = new VersionOptionLinkTblAdptr();
            return _AdptrVersionOption;
        }
    }

    private QuantityDiscountsTblAdptr _AdptrQuantity = null/* TODO Change to default(_) if this is not a reference type */;
    protected QuantityDiscountsTblAdptr AdptrQuantity
    {
        get
        {
            _AdptrQuantity = new QuantityDiscountsTblAdptr();
            return _AdptrQuantity;
        }
    }

    public System.Data.DataTable GetByProduct(int prodID, short langID, short cgroup)
    {
        return Adptr.GetByProductID(prodID, langID, cgroup);
    }

    public System.Data.DataTable GetProductOptions(int _ProductID, short _LangID)
    {
        return Adptr.GetProductOptions(_ProductID, _LangID);
    }

    public DataTable GetProductOptionValues(int _ProductID, short _LangID, Int32 _OptionGroupID)
    {
        return Adptr.GetOptionValues(_ProductID, _OptionGroupID, _LangID);
    }

    public int GetProductID_s(long _VersionID)
    {
        VersionQTblAdptr qAdptr = new VersionQTblAdptr();
        int numProductID;
        qAdptr.GetProductID_s(_VersionID, numProductID);
        return numProductID;
    }

    public DataTable GetMinPriceByProductList(string pProductList, short pLanguageID, short pCGID)
    {
        return Adptr.GetMinPriceByProductList(pLanguageID, pProductList, pCGID);
    }

    public float GetOptionStockQty(int ProductID, string strOptionList)
    {
        float numQty = -9999.0F;
        Adptr.GetOptionStockQty(ProductID, strOptionList, numQty);
        return numQty;
    }

    public DataTable GetVersionCustomization(long numVersionID)
    {
        return Adptr.GetCustomization(numVersionID);
    }

    public double _GetWeightByVersionCode(string strVersionCode)
    {
        try
        {
            return System.Convert.ToDouble(Adptr._GetWeightByVersionCode(strVersionCode));
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    public bool IsVersionCustomizable(long numVersionID)
    {
        char chrCustomizationType = "";
        try
        {
            chrCustomizationType = System.Convert.ToChar(GetVersionCustomization(numVersionID).Rows(0)("V_CustomizationType"));
            return chrCustomizationType != "n";
        }
        catch (Exception ex)
        {
        }
        return false;
    }

    private DataTable GetBasicVersionByProduct(int _ProductID)
    {
        return Adptr.GetBasicVersionByProduct(_ProductID);
    }

    public bool IsStockTrackingInBase(int _ProductID)
    {
        DataTable tblBaseVersion = Adptr.GetBasicVersionByProduct(_ProductID);
        if (tblBaseVersion.Rows.Count > 0 && tblBaseVersion.Rows(0)("V_QuantityWarnLevel") > 0.0F)
            return true;
        return false;
    }

    public DataTable _GetVersionByID(long _VersionID)
    {
        return Adptr._GetByID(_VersionID);
    }

    public string _GetNameByVersionID(int _VersionID, short _LanguageID)
    {
        LanguageElementsBLL objLanguageElementsBLL = new LanguageElementsBLL();
        return objLanguageElementsBLL.GetElementValue(_LanguageID, LANG_ELEM_TABLE_TYPE.Versions, LANG_ELEM_FIELD_NAME.Name, _VersionID);
    }

    public DataTable _GetStockLevel(byte numLanguageID)
    {
        return Adptr._GetStockLevel(numLanguageID);
    }

    public DataTable _SearchVersionByName(string _Key, byte _LanguageID)
    {
        DataTable tbl = new DataTable();
        tbl = Adptr._SearchByName(_Key, _LanguageID);
        if (tbl.Rows.Count == 0)
            tbl = Adptr._GetData(_LanguageID);
        return tbl;
    }

    public DataTable _SearchVersionByCode(string _Key)
    {
        DataTable tbl = new DataTable();
        try
        {
            tbl = Adptr._SearchByCode(_Key);
            if (tbl.Rows.Count == 0)
                tbl = Adptr._SearchByCode("");
        }
        catch (Exception ex)
        {
        }

        return tbl;
    }

    public DataTable _SearchVersionByCodeExcludeBaseCombinations(string _Key)
    {
        DataTable tbl = new DataTable();
        try
        {
            tbl = Adptr._SearchByCodeExcludeBaseCombinations(_Key);
            if (tbl.Rows.Count == 0)
                tbl = Adptr._SearchByCodeExcludeBaseCombinations("");
        }
        catch (Exception ex)
        {
        }

        return tbl;
    }

    public DataTable _GetBasicVersionByProduct(int _ProductID)
    {
        return Adptr._GetBasicVersionByProduct(_ProductID);
    }

    public DataTable _GetSingleVersionByProduct(int _ProductID)
    {
        return Adptr._GetSingleVersionByProduct(_ProductID);
    }

    public DataTable _GetVersionOptionsByProductID(int pProductID)
    {
        return AdptrVersionOption._GetByProduct(pProductID);
    }

    public DataTable _GetCombinationsByProductID(int pProductID)
    {
        return Adptr._GetCombinationsByProductID(pProductID);
    }

    public DataTable _GetSuspendedByVersionID(long pVersionID, byte pLanguageID)
    {
        return Adptr._GetSuspendedByVersionID(pVersionID, pLanguageID);
    }

    public DataTable _GetSchema()
    {
        return Adptr._GetByID(-1);
    }

    private DataTable _GetRowsByProduct(int pProductID)
    {
        return Adptr._GetRowsByProductID(pProductID);
    }

    public System.Data.DataTable _GetByProduct(int prodID, short langID)
    {
        return Adptr._GetByProductID(prodID, langID);
    }

    public bool _IsCodeNumberExist(string pCodeNumber, int pExcludedProductID = -1, long pExecludedVersionID = -1)
    {
        return Adptr._GetByCodeNumber(pCodeNumber, pExcludedProductID, pExecludedVersionID).Rows.Count > 0;
    }

    public Int64 _GetVersionIDByCodeNumber(string pCodeNumber)
    {
        return Adptr._GetVersionIDByCodeNumber(pCodeNumber);
    }

    public int _GetNoOfVersionsByProductID(int ProductID)
    {
        return System.Convert.ToInt32(Adptr._GetTotalByProductID(ProductID).Rows(0)("TotalVersions"));
    }

    public DataTable _GetQuantityDiscountsByVersion(long VersionID)
    {
        return AdptrQuantity._GetByVersion(VersionID);
    }

    public DataTable _GetQuantityDiscountsByVersionIDList(string strVersionIDList, byte LanguageID)
    {
        return AdptrQuantity._GetByVersionIDList(strVersionIDList, LanguageID);
    }

    public DataTable GetQuantityDiscountByProduct(int ProductID, byte LanguageID)
    {
        return AdptrQuantity.GetByProduct(ProductID, LanguageID);
    }

    public DataTable _GetDownloadableFiles(byte numLanguageID)
    {
        return Adptr._GetDownloadableFiles(numLanguageID);
    }

    public DataTable _GetDownloadableLinks(byte numLanguageID)
    {
        return Adptr._GetDownloadableLinks(numLanguageID);
    }

    public DataTable _GetVersionsByCategoryList(byte numLanguageID, float numFromPrice, float numToPrice, string strCategories)
    {
        return Adptr._GetDetailsByCategoryList(numLanguageID, numFromPrice, numToPrice, strCategories);
    }

    public static bool _MarkupPrices(string strVersionsPricesList, string strTargetField, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddDiscount = sqlConn.CreateCommand;
            cmdAddDiscount.CommandText = "_spKartrisVersions_MarkupPrices";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddDiscount.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddDiscount.Transaction = savePoint;
                cmdAddDiscount.Parameters.AddWithValue("@List", FixNullToDB(strVersionsPricesList));
                cmdAddDiscount.Parameters.AddWithValue("@Target", FixNullToDB(strTargetField));
                cmdAddDiscount.ExecuteNonQuery();
                savePoint.Commit();
                sqlConn.Close();
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

    public bool _UpdateQuantityDiscount(DataTable tblQtyDiscount, long VersionID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddDiscount = sqlConn.CreateCommand;
            cmdAddDiscount.CommandText = "_spKartrisQuantityDiscounts_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddDiscount.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddDiscount.Transaction = savePoint;
                if (!_DeleteQtyDiscountByVersion(VersionID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                foreach (DataRow row in tblQtyDiscount.Rows)
                {
                    cmdAddDiscount.Parameters.AddWithValue("@VersionID", VersionID);
                    cmdAddDiscount.Parameters.AddWithValue("@Quantity", System.Convert.ToSingle(row("QD_Quantity")));
                    cmdAddDiscount.Parameters.AddWithValue("@Price", System.Convert.ToDouble(row("QD_Price")));

                    cmdAddDiscount.ExecuteNonQuery();
                    cmdAddDiscount.Parameters.Clear();
                }

                savePoint.Commit();
                sqlConn.Close();
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

    private bool _DeleteQtyDiscountByVersion(long VersionID, SqlConnection sqlConn, SqlTransaction savePoint)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("_spKartrisQuantityDiscounts_DeleteByVersion", sqlConn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = savePoint;

            cmd.Parameters.AddWithValue("@VersionID", VersionID);

            cmd.ExecuteNonQuery();
            return true;
        }
        // End Using
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
        return false;
    }

    public DataTable _GetCustomerGroupPricesForVersion(int LanguageID, long VersionID)
    {
        DataTable dtCustomerGroupPrices = new DataTable();
        SqlDataAdapter daCustomerGroupPrices = new SqlDataAdapter();

        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdCustomerGroupPrices = sqlConn.CreateCommand;
            cmdCustomerGroupPrices.CommandText = "_spKartrisVersions_GetCustomerGroupPrices";
            cmdCustomerGroupPrices.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                cmdCustomerGroupPrices.Parameters.AddWithValue("@LanguageID", LanguageID);
                cmdCustomerGroupPrices.Parameters.AddWithValue("@VersionID", VersionID);
                daCustomerGroupPrices.SelectCommand = cmdCustomerGroupPrices;
                cmdCustomerGroupPrices.ExecuteNonQuery();
                daCustomerGroupPrices.Fill(dtCustomerGroupPrices);
                sqlConn.Close();
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                    sqlConn.Close();
            }
        }

        return dtCustomerGroupPrices;
    }

    public bool _UpdateCustomerGroupPrice(int CustomerGroupID, int VersionID, double Price, int CustomerGroupPriceID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        bool blnSuccess = true;

        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateCustomerGroupPrices = sqlConn.CreateCommand;
            cmdUpdateCustomerGroupPrices.CommandText = "_spKartrisVersions_UpdateCustomerGroupPrices";
            cmdUpdateCustomerGroupPrices.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateCustomerGroupPrices.Parameters.AddWithValue("@CustomerGroupID", CustomerGroupID);
                cmdUpdateCustomerGroupPrices.Parameters.AddWithValue("@VersionID", VersionID);
                cmdUpdateCustomerGroupPrices.Parameters.AddWithValue("@Price", Price);
                cmdUpdateCustomerGroupPrices.Parameters.AddWithValue("@CustomerGroupPriceID", CustomerGroupPriceID);
                sqlConn.Open();
                cmdUpdateCustomerGroupPrices.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
                blnSuccess = false;
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                    sqlConn.Close();
            }
        }

        return blnSuccess;
    }

    public long GetCombinationVersionID_s(int numProductID, string strOptions)
    {
        VersionQTblAdptr qAdptr = new VersionQTblAdptr();
        long numVersionID;
        qAdptr.GetCombinationVersionID_s(numProductID, strOptions, numVersionID);
        return numVersionID;
    }

    public bool _CreateNewCombinations(DataTable ptblNewData, int pProductID, long pBasicVersionID, ref string strMsg)
    {

        // ' From Kartris v3, a little different here. We want to keep the version IDs
        // ' the same for combinations that already exist and data gets recovered, rather
        // ' than copying it across to new versions. Therefore, the process is a little
        // ' different now.
        // ' 1. Accept two datatables of data - one for new versions, one for existing
        // ' 2. Unsuspend existing versions that are needed by updating them with the
        // '    new data, and setting type back to 'c' (from 's')
        // ' 3. Delete suspended combinations (any still left) using
        // '    [_spKartrisVersions_DeleteSuspendedVersions]
        // '    a. Delete the related records in the VersionLinkOptions
        // '    b. Delete the related records in the LanguageElements
        // '    c. Delete the records from Versions
        // ' 4. Add new combinations with new IDs
        // ' 5. Update the basic version To be of type 'b'

        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();

                if (!_DeleteSuspendedCombinations(pProductID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                if (!_InsertNewCombinations(ptblNewData, pProductID, sqlConn, savePoint))
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

    private bool _DeleteSuspendedCombinations(int productID, SqlConnection sqlConn, SqlTransaction savePoint)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("_spKartrisVersions_DeleteSuspendedVersions", sqlConn, savePoint);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@P_ID", productID);
            cmd.ExecuteNonQuery();
            return true;
        }
        // End Using
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
        return false;
    }

    private bool _InsertNewCombinations(DataTable ptblCombinations, int pProductID, SqlConnection pSqlConn, SqlTransaction pSavePoint)
    {
        try
        {
            DataTable tblLanguageElement = new DataTable();
            tblLanguageElement.Columns.Add(new DataColumn("_LE_LanguageID", Type.GetType("System.Byte")));
            tblLanguageElement.Columns.Add(new DataColumn("_LE_FieldID", Type.GetType("System.Byte")));
            tblLanguageElement.Columns.Add(new DataColumn("_LE_Value", Type.GetType("System.String")));
            DataTable tblLanguages = GetLanguagesFromCache();
            long numNewVersionID = 0;
            foreach (DataRow row in ptblCombinations.Rows)
            {
                tblLanguageElement.Rows.Clear();
                foreach (DataRow LangRow in tblLanguages.Rows)
                {
                    tblLanguageElement.Rows.Add(System.Convert.ToByte(LangRow("LANG_ID")), LANG_ELEM_FIELD_NAME.Name, System.Convert.ToString(row("V_Name")));
                    tblLanguageElement.Rows.Add(System.Convert.ToByte(LangRow("LANG_ID")), LANG_ELEM_FIELD_NAME.Description, "");
                }

                // Dim blnExists As Boolean = FixNullFromDB(row("IsExist"))
                // New Version
                if (!_AddNewVersionAsCombination(tblLanguageElement, System.Convert.ToString(row("V_CodeNumber")), System.Convert.ToInt32(row("V_ProductID")), System.Convert.ToSingle(row("V_Price")), System.Convert.ToByte(row("V_Tax")), FixNullFromDB(row("V_Tax2")), "", System.Convert.ToSingle(row("V_Weight")), System.Convert.ToInt16(row("V_Quantity")), System.Convert.ToInt32(row("V_QuantityWarnLevel")), System.Convert.ToSingle(row("V_RRP")), System.Convert.ToChar(row("V_Type")), pSqlConn, pSavePoint, ref numNewVersionID))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom") + " Check you have set a tax band for the base version.");

                CkartrisFormatErrors.LogError(System.Convert.ToString(row("V_CodeNumber")));

                // ' Options
                string strIDList = System.Convert.ToString(row("ID_List"));
                if (strIDList.EndsWith(","))
                    strIDList = strIDList.TrimEnd(",");
                if (strIDList.StartsWith(","))
                    strIDList = strIDList.TrimStart(",");

                SqlCommand cmdAddOptions = new SqlCommand("_spKartrisVersionOptionLink_AddOptionsList", pSqlConn, pSavePoint);
                cmdAddOptions.CommandType = CommandType.StoredProcedure;
                cmdAddOptions.Parameters.AddWithValue("@VersionID", numNewVersionID);
                cmdAddOptions.Parameters.AddWithValue("@OptionList", strIDList);
                cmdAddOptions.ExecuteNonQuery();
                numNewVersionID = 0;
            }

            return true;
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }

        return false;
    }

    public bool _AddNewVersionAsCombination(DataTable tblElements, string strCodeNumber, int intProductID, decimal decPrice, byte intTaxID, byte intTaxID2, string strExtraTax, float snglWeight, float sngStockQty, float sngWarnLevel, decimal decRRP, char chrType, SqlConnection sqlConn, SqlTransaction savePoint, ref long numNewVersionID)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        SqlCommand cmdAddVersion = new SqlCommand("_spKartrisVersions_AddAsCombination", sqlConn);
        cmdAddVersion.CommandType = CommandType.StoredProcedure;
        try
        {
            cmdAddVersion.Parameters.AddWithValue("@V_CodeNumber", FixNullToDB(strCodeNumber, "s"));
            cmdAddVersion.Parameters.AddWithValue("@V_ProductID", FixNullToDB(intProductID, "i"));
            cmdAddVersion.Parameters.AddWithValue("@V_Price", FixNullToDB(decPrice, "z"));
            cmdAddVersion.Parameters.AddWithValue("@V_Tax", FixNullToDB(intTaxID, "i"));
            cmdAddVersion.Parameters.AddWithValue("@V_Tax2", FixNullToDB(intTaxID2, "i"));
            cmdAddVersion.Parameters.AddWithValue("@V_TaxExtra", FixNullToDB(strExtraTax));
            cmdAddVersion.Parameters.AddWithValue("@V_Weight", snglWeight);
            cmdAddVersion.Parameters.AddWithValue("@V_Quantity", sngStockQty);
            cmdAddVersion.Parameters.AddWithValue("@V_QuantityWarnLevel", sngWarnLevel);
            cmdAddVersion.Parameters.AddWithValue("@V_RRP", FixNullToDB(decRRP, "z"));
            cmdAddVersion.Parameters.AddWithValue("@V_Type", FixNullToDB(chrType, "c"));
            cmdAddVersion.Parameters.AddWithValue("@V_NewID", 0).Direction = ParameterDirection.Output;

            cmdAddVersion.Transaction = savePoint;

            cmdAddVersion.ExecuteNonQuery();

            if (cmdAddVersion.Parameters("@V_NewID").Value == null || cmdAddVersion.Parameters("@V_NewID").Value == DBNull.Value)
                throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

            numNewVersionID = cmdAddVersion.Parameters("@V_NewID").Value;
            if (!LanguageElementsBLL._AddLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.Versions, numNewVersionID, sqlConn, savePoint))
                throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

            return true;
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
        // End Using
        return false;
    }

    public bool _DeleteExistingCombinations(int productID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdSuspendVersions = sqlConn.CreateCommand;
            cmdSuspendVersions.CommandText = "_spKartrisVersions_SuspendProductVersions";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdSuspendVersions.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();

                cmdSuspendVersions.Transaction = savePoint;
                cmdSuspendVersions.Parameters.AddWithValue("@P_ID", productID);
                cmdSuspendVersions.ExecuteNonQuery();
                if (!_DeleteSuspendedCombinations(productID, sqlConn, savePoint))
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

    public bool _UpdateCurrentCombinations(DataTable ptblCurrentCombinations, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateCombination = sqlConn.CreateCommand;
            cmdUpdateCombination.CommandText = "_spKartrisVersions_UpdateCombinationVersion";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateCombination.CommandType = CommandType.StoredProcedure;

            try
            {
                cmdUpdateCombination.Parameters.Add("@ID", SqlDbType.BigInt);
                cmdUpdateCombination.Parameters.Add("@Name", SqlDbType.NVarChar, 50);
                cmdUpdateCombination.Parameters.Add("@CodeNumber", SqlDbType.NVarChar, 50);
                cmdUpdateCombination.Parameters.Add("@Price", SqlDbType.Real);
                cmdUpdateCombination.Parameters.Add("@StockQty", SqlDbType.Float);
                cmdUpdateCombination.Parameters.Add("@QtyWarnLevel", SqlDbType.Float);
                cmdUpdateCombination.Parameters.Add("@Live", SqlDbType.Bit);

                // See the explanation below in _UpdateVersion
                // for this parameter. It's a clever way of tagging
                // versions updated by data tool with datetime stamp
                // so we can process stock notifications for them
                cmdUpdateCombination.Parameters.AddWithValue("@V_BulkUpdateTimeStamp", "1900/1/1");
                sqlConn.Open();

                savePoint = sqlConn.BeginTransaction();
                cmdUpdateCombination.Transaction = savePoint;
                foreach (DataRow row in ptblCurrentCombinations.Rows)
                {
                    cmdUpdateCombination.Parameters("@ID").Value = System.Convert.ToInt64(row("ID"));
                    cmdUpdateCombination.Parameters("@Name").Value = FixNullToDB(row("Name"), "s");
                    cmdUpdateCombination.Parameters("@CodeNumber").Value = FixNullToDB(row("CodeNumber"), "s");
                    cmdUpdateCombination.Parameters("@Price").Value = FixNullToDB(row("Price"), "g");
                    cmdUpdateCombination.Parameters("@StockQty").Value = row("StockQty");
                    cmdUpdateCombination.Parameters("@QtyWarnLevel").Value = row("QtyWarnLevel");
                    cmdUpdateCombination.Parameters("@Live").Value = FixNullToDB(row("Live"), "b");

                    cmdUpdateCombination.ExecuteNonQuery();
                }
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

    public float GetCombinationPrice(int numProductID, string strOptions)
    {
        VersionQTblAdptr qAdptr = new VersionQTblAdptr();
        float numCombinationPrice = 0.0F;
        qAdptr.GetCombinationPrice_s(numProductID, strOptions, numCombinationPrice);
        return FixNullFromDB(numCombinationPrice);
    }

    public bool _SetVersionAsBaseByProductID(long intProductID, SqlConnection sqlConn, SqlTransaction savePoint, ref string strMsg)
    {
        try
        {
            string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
            SqlCommand cmdUpdateVersion = new SqlCommand("_spKartrisVersions_SetBaseByProduct", sqlConn);
            cmdUpdateVersion.CommandType = CommandType.StoredProcedure;
            cmdUpdateVersion.Parameters.AddWithValue("@ProductID", intProductID);
            cmdUpdateVersion.Transaction = savePoint;
            cmdUpdateVersion.ExecuteNonQuery();
            return true;
        }
        // End Using
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
        }

        return false;
    }
    public bool _AddNewVersionAsSingle(DataTable tblElements, string strCodeNumber, int intProductID, short intCustomerGrp, SqlConnection sqlConn, SqlTransaction savePoint, ref string strMsg)
    {
        try
        {
            string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
            SqlCommand cmdAddVersion = new SqlCommand("_spKartrisVersions_AddAsSingle", sqlConn);

            cmdAddVersion.CommandType = CommandType.StoredProcedure;

            cmdAddVersion.Parameters.AddWithValue("@V_CodeNumber", FixNullToDB(strCodeNumber, "s"));
            cmdAddVersion.Parameters.AddWithValue("@V_ProductID", FixNullToDB(intProductID, "i"));
            cmdAddVersion.Parameters.AddWithValue("@V_CustomerGroupID", FixNullToDB(intCustomerGrp, "i")); // FixNullToDB(intCustomerGrp, "i"))
            cmdAddVersion.Parameters.AddWithValue("@V_NewID", 0).Direction = ParameterDirection.Output;

            cmdAddVersion.Transaction = savePoint;
            cmdAddVersion.ExecuteNonQuery();

            if (cmdAddVersion.Parameters("@V_NewID").Value == null || cmdAddVersion.Parameters("@V_NewID").Value == DBNull.Value)
                throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

            long intNewVersionID = cmdAddVersion.Parameters("@V_NewID").Value;
            if (!LanguageElementsBLL._AddLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.Versions, intNewVersionID, sqlConn, savePoint))
                throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

            strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
            // End Using
            return true;
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
        }

        return false;
    }

    public bool _AddNewVersion(DataTable tblElements, string strCodeNumber, int intProductID, decimal decPrice, byte intTaxID, byte intTaxID2, string strTaxExtra, float snglWeight, byte intDeliveryTime, float sngStockQty, float sngWarnLevel, bool blnLive, string strDownloadInfo, char chrDownloadType, decimal decRRP, char chrType, short intCustomerGrp, char chrCustomizationType, string strCustomizationDesc, decimal decCustomizationCost, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddVersion = sqlConn.CreateCommand;
            cmdAddVersion.CommandText = "_spKartrisVersions_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddVersion.CommandType = CommandType.StoredProcedure;
            DataTable tblClone = tblElements;
            try
            {
                cmdAddVersion.Parameters.AddWithValue("@V_CodeNumber", FixNullToDB(strCodeNumber, "s"));
                cmdAddVersion.Parameters.AddWithValue("@V_ProductID", FixNullToDB(intProductID, "i"));
                cmdAddVersion.Parameters.AddWithValue("@V_Price", decPrice);
                cmdAddVersion.Parameters.AddWithValue("@V_Tax", FixNullToDB(intTaxID, "i"));
                cmdAddVersion.Parameters.AddWithValue("@V_Tax2", FixNullToDB(intTaxID2, "i"));
                cmdAddVersion.Parameters.AddWithValue("@V_TaxExtra", FixNullToDB(strTaxExtra, "s"));
                cmdAddVersion.Parameters.AddWithValue("@V_Weight", snglWeight);
                cmdAddVersion.Parameters.AddWithValue("@V_DeliveryTime", intDeliveryTime);
                cmdAddVersion.Parameters.AddWithValue("@V_Quantity", sngStockQty);
                cmdAddVersion.Parameters.AddWithValue("@V_QuantityWarnLevel", sngWarnLevel);
                cmdAddVersion.Parameters.AddWithValue("@V_Live", blnLive);
                cmdAddVersion.Parameters.AddWithValue("@V_DownLoadInfo", FixNullToDB(strDownloadInfo, "s"));
                cmdAddVersion.Parameters.AddWithValue("@V_DownloadType", FixNullToDB(chrDownloadType, "c"));
                cmdAddVersion.Parameters.AddWithValue("@V_RRP", decRRP);
                cmdAddVersion.Parameters.AddWithValue("@V_Type", FixNullToDB(chrType, "c"));
                cmdAddVersion.Parameters.AddWithValue("@V_CustomerGroupID", FixNullToDB(intCustomerGrp, "i")); // FixNullToDB(intCustomerGrp, "i"))
                cmdAddVersion.Parameters.AddWithValue("@V_CustomizationType", chrCustomizationType);
                cmdAddVersion.Parameters.AddWithValue("@V_CustomizationDesc", FixNullToDB(strCustomizationDesc, "s"));
                cmdAddVersion.Parameters.AddWithValue("@V_CustomizationCost", FixNullToDB(decCustomizationCost, "z")); // z=decimal, double is d
                cmdAddVersion.Parameters.AddWithValue("@V_NewID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddVersion.Transaction = savePoint;

                cmdAddVersion.ExecuteNonQuery();

                long intNewVersionID = cmdAddVersion.Parameters("@V_NewID").Value;
                if (!LanguageElementsBLL._AddLanguageElements(tblClone, LANG_ELEM_TABLE_TYPE.Versions, intNewVersionID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                if (chrDownloadType == "u" && !string.IsNullOrEmpty(strDownloadInfo))
                {
                    string strUploadFolder = GetKartConfig("general.uploadfolder");
                    if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + "temp/" + strDownloadInfo)))
                    {
                        if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + strDownloadInfo)))
                            File.Replace(System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + "temp/" + strDownloadInfo), System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + strDownloadInfo), System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + "temp/backup_" + strDownloadInfo));
                        else
                            File.Move(System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + "temp/" + strDownloadInfo), System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + strDownloadInfo));
                    }
                    if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + strDownloadInfo)))
                        throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgFileUpload"));
                }

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

    public bool _UpdateVersion(DataTable tblElements, long lngVersionID, string strCodeNumber, int intProductID, decimal decPrice, byte intTaxID, byte intTaxID2, string strTaxExtra, float snglWeight, byte intDeliveryTime, float sngStockQty, float sngWarnLevel, bool blnLive, string strDownloadInfo, char chrDownloadType, decimal decRRP, char chrType, short intCustomerGrp, char chrCustomizationType, string strCustomizationDesc, decimal decCustomizationCost, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateVersion = sqlConn.CreateCommand;
            cmdUpdateVersion.CommandText = "_spKartrisVersions_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateVersion.CommandType = CommandType.StoredProcedure;
            DataTable tblClone = tblElements;

            try
            {
                cmdUpdateVersion.Parameters.AddWithValue("@V_ID", lngVersionID);
                cmdUpdateVersion.Parameters.AddWithValue("@V_CodeNumber", FixNullToDB(strCodeNumber, "s"));
                cmdUpdateVersion.Parameters.AddWithValue("@V_ProductID", FixNullToDB(intProductID, "i"));
                cmdUpdateVersion.Parameters.AddWithValue("@V_Price", decPrice);
                cmdUpdateVersion.Parameters.AddWithValue("@V_Tax", FixNullToDB(intTaxID, "i"));
                cmdUpdateVersion.Parameters.AddWithValue("@V_Tax2", FixNullToDB(intTaxID2, "i"));
                cmdUpdateVersion.Parameters.AddWithValue("@V_TaxExtra", FixNullToDB(strTaxExtra, "s"));
                cmdUpdateVersion.Parameters.AddWithValue("@V_Weight", snglWeight);
                cmdUpdateVersion.Parameters.AddWithValue("@V_DeliveryTime", intDeliveryTime);
                cmdUpdateVersion.Parameters.AddWithValue("@V_Quantity", sngStockQty);
                cmdUpdateVersion.Parameters.AddWithValue("@V_QuantityWarnLevel", sngWarnLevel);
                cmdUpdateVersion.Parameters.AddWithValue("@V_Live", blnLive);
                cmdUpdateVersion.Parameters.AddWithValue("@V_DownLoadInfo", FixNullToDB(strDownloadInfo, "s"));
                cmdUpdateVersion.Parameters.AddWithValue("@V_DownloadType", FixNullToDB(chrDownloadType, "c"));
                cmdUpdateVersion.Parameters.AddWithValue("@V_RRP", decRRP);
                cmdUpdateVersion.Parameters.AddWithValue("@V_Type", FixNullToDB(chrType, "c"));
                cmdUpdateVersion.Parameters.AddWithValue("@V_CustomerGroupID", FixNullToDB(intCustomerGrp, "i"));
                cmdUpdateVersion.Parameters.AddWithValue("@V_CustomizationType", chrCustomizationType);
                cmdUpdateVersion.Parameters.AddWithValue("@V_CustomizationDesc", FixNullToDB(strCustomizationDesc, "s"));
                cmdUpdateVersion.Parameters.AddWithValue("@V_CustomizationCost", FixNullToDB(decCustomizationCost, "z")); // z=decimal, d is for double

                // Ok, this optional parameter below is used for stock notification
                // functionality. If versions are updated through Kartris, we'll
                // check for and send stock notifications to users if the item is
                // now in stock. However, versions can also be updated by the data
                // tool. In that case, since this BLL function is not called, but
                // instead the _spKartrisVersions_Update sproc is called directly
                // we need a way to mark versions as having been bulk updated (i.e.
                // via data tool, not Kartris) so we can then provide a way in the
                // Kartris back end to manually trigger stock notification checks.
                // The way this works is that the sproc now has an optional parameter
                // for the V_BulkUpdateTimeStamp field. This defaults to NULL, which
                // is then coalesced within the the UPDATE query to be the date now.
                // (SQL doesn't allow us to default parameter to a function like
                // GetDate). So any version updated with the sproc directly will get
                // the current date in that field. However, we can pass a value in
                // to force this field to a different value. That's what we do below.
                // This means later we can query the version table and find any 
                // records with a V_BulkUpdateTimeStamp of a date which is not NULL
                // or 1900/1/1. Those ones were updated from the data tool and will
                // need stock notifications checked, which we can show in the task
                // list. When that routine is run, we can reset the V_BulkUpdateTimeStamp
                // field to 1900/1/1 or NULL. Good thing here is we also don't need to
                // update the data tool to pass extra parameters, because we have a
                // default value.
                cmdUpdateVersion.Parameters.AddWithValue("@V_BulkUpdateTimeStamp", "1900/1/1");
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateVersion.Transaction = savePoint;

                cmdUpdateVersion.ExecuteNonQuery();



                if (chrType == "b")
                {
                    ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
                    if (objObjectConfigBLL.GetValue("K:product.usecombinationprice", intProductID) != "1")
                    {
                        if (!_UpdateCombinationsFromBasicInfo(intProductID, decPrice, intTaxID, intTaxID2, strTaxExtra, snglWeight, decRRP, sqlConn, savePoint))
                            throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                    }
                }

                if (!LanguageElementsBLL._UpdateLanguageElements(tblClone, LANG_ELEM_TABLE_TYPE.Versions, lngVersionID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                if (chrDownloadType == "u" && !string.IsNullOrEmpty(strDownloadInfo))
                {
                    string strUploadFolder = GetKartConfig("general.uploadfolder");
                    if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + "temp/" + strDownloadInfo)))
                    {
                        if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + strDownloadInfo)))
                            File.Replace(System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + "temp/" + strDownloadInfo), System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + strDownloadInfo), System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + "temp/backup_" + strDownloadInfo));
                        else
                            File.Move(System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + "temp/" + strDownloadInfo), System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + strDownloadInfo));
                    }
                    if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + strDownloadInfo)))
                        throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgFileUpload"));
                }

                // Before we finish, note that a version has been
                // updated. It's possible that there might be some
                // people waiting on stock notifications. We can 
                // check first if this version is under stock
                // control, and is in stock. If so, we can run the
                // function to check for and send any stock
                // notifications.
                if (GetKartConfig("general.stocknotification.enabled") == "y" & sngStockQty > 0 & sngWarnLevel > 0)

                    // Stock notifications are enabled, this version is
                    // being stock tracked and has items in stock so we should
                    // run check to see if we should send stock notifications
                    StockNotificationsBLL._SearchSendStockNotifications(lngVersionID);

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

    public bool _UpdateVersionDownloadInfo(long lngVersionID, string strDownloadInfo, char chrDownloadType, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateVersion = sqlConn.CreateCommand;
            cmdUpdateVersion.CommandText = "_spKartrisVersions_UpdateDownloadInfo";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateVersion.CommandType = CommandType.StoredProcedure;

            try
            {
                cmdUpdateVersion.Parameters.AddWithValue("@V_ID", lngVersionID);
                cmdUpdateVersion.Parameters.AddWithValue("@V_DownLoadInfo", strDownloadInfo);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateVersion.Transaction = savePoint;

                cmdUpdateVersion.ExecuteNonQuery();

                if (chrDownloadType == "u")
                {
                    string strUploadFolder = GetKartConfig("general.uploadfolder");
                    if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + "temp/" + strDownloadInfo)))
                    {
                        if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + strDownloadInfo)))
                            File.Replace(System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + "temp/" + strDownloadInfo), System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + strDownloadInfo), System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + "temp/backup_" + strDownloadInfo));
                        else
                            File.Move(System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + "temp/" + strDownloadInfo), System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + strDownloadInfo));

                        if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + strDownloadInfo)))
                            throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgFileUpload"));
                    }
                }

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

    public bool _UpdateVersionStockLevel(DataTable tblVersionsToUpdate, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateVersionStock = sqlConn.CreateCommand;
            cmdUpdateVersionStock.CommandText = "_spKartrisVersions_UpdateStockLevel";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateVersionStock.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateVersionStock.Transaction = savePoint;
                foreach (DataRow row in tblVersionsToUpdate.Rows)
                {
                    cmdUpdateVersionStock.Parameters.AddWithValue("@V_ID", row("VersionID"));
                    cmdUpdateVersionStock.Parameters.AddWithValue("@V_Quantity", row("StockQty"));
                    cmdUpdateVersionStock.Parameters.AddWithValue("@V_QuantityWarnLevel", row("WarnLevel"));

                    // See the explanation above in _UpdateVersion
                    // for this parameter. It's a clever way of tagging
                    // versions updated by data tool with datetime stamp
                    // so we can process stock notifications for them
                    cmdUpdateVersionStock.Parameters.AddWithValue("@V_BulkUpdateTimeStamp", "1900/1/1");

                    cmdUpdateVersionStock.ExecuteNonQuery();
                    cmdUpdateVersionStock.Parameters.Clear();

                    // Before we finish, note that a version has been
                    // updated. It's possible that there might be some
                    // people waiting on stock notifications. We can 
                    // check first if this version is under stock
                    // control, and is in stock. If so, we can run the
                    // function to check for and send any stock
                    // notifications.
                    if (GetKartConfig("general.stocknotification.enabled") == "y" & row("StockQty") > 0 & row("WarnLevel") > 0)

                        // Stock notifications are enabled, this version is
                        // being stock tracked and has items in stock so we should
                        // run check to see if we should send stock notifications
                        StockNotificationsBLL._SearchSendStockNotifications(row("VersionID"));
                }
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

    public bool _UpdateVersionStockLevelByCode(DataTable tblVersionsToUpdate, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateVersionStock = sqlConn.CreateCommand;
            cmdUpdateVersionStock.CommandText = "_spKartrisVersions_UpdateStockLevelByCode";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateVersionStock.CommandType = CommandType.StoredProcedure;
            cmdUpdateVersionStock.CommandTimeout = 2700;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateVersionStock.Transaction = savePoint;
                foreach (DataRow row in tblVersionsToUpdate.Rows)
                {
                    cmdUpdateVersionStock.Parameters.AddWithValue("@V_CodeNumber", row("VersionCode"));
                    cmdUpdateVersionStock.Parameters.AddWithValue("@V_Quantity", row("StockQty"));
                    cmdUpdateVersionStock.Parameters.AddWithValue("@V_QuantityWarnLevel", row("WarnLevel"));

                    // See the explanation above in _UpdateVersion
                    // for this parameter. It's a clever way of tagging
                    // versions updated by data tool with datetime stamp
                    // so we can process stock notifications for them
                    cmdUpdateVersionStock.Parameters.AddWithValue("@V_BulkUpdateTimeStamp", "1900/1/1");

                    cmdUpdateVersionStock.ExecuteNonQuery();
                    cmdUpdateVersionStock.Parameters.Clear();

                    // Before we finish, note that a version has been
                    // updated. It's possible that there might be some
                    // people waiting on stock notifications. We can 
                    // check first if this version is under stock
                    // control, and is in stock. If so, we can run the
                    // function to check for and send any stock
                    // notifications.
                    if (GetKartConfig("general.stocknotification.enabled") == "y" & row("StockQty") > 0 & row("WarnLevel") > 0)

                        // Stock notifications are enabled, this version is
                        // being stock tracked and has items in stock so we should
                        // run check to see if we should send stock notifications
                        StockNotificationsBLL._SearchSendStockNotifications(row("VersionID"));
                }
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

    private bool _UpdateCombinationsFromBasicInfo(int intProductID, float snglPrice, byte intTax, byte intTax2, string strTaxExtra, float snglWeight, float snglRRP, SqlConnection sqlConn, SqlTransaction savePoint)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("_spKartrisVersions_UpdateCombinationsFromBasicInfo", sqlConn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = savePoint;

            cmd.Parameters.AddWithValue("@ProductID", FixNullToDB(intProductID, "i"));
            cmd.Parameters.AddWithValue("@Price", FixNullToDB(snglPrice, "g"));
            cmd.Parameters.AddWithValue("@Tax", FixNullToDB(intTax, "i"));
            cmd.Parameters.AddWithValue("@Tax2", FixNullToDB(intTax2, "i"));
            cmd.Parameters.AddWithValue("@TaxExtra", FixNullToDB(strTaxExtra, "s"));
            cmd.Parameters.AddWithValue("@Weight", snglWeight);
            cmd.Parameters.AddWithValue("@RRP", snglRRP);

            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
        return false;
    }

    public bool _DeleteVersion(long VersionID, ref string strFiles, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdDeleteVersion = sqlConn.CreateCommand;
            cmdDeleteVersion.CommandText = "_spKartrisVersions_Delete";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdDeleteVersion.CommandType = CommandType.StoredProcedure;

            try
            {
                cmdDeleteVersion.Parameters.AddWithValue("@V_ID", VersionID);
                cmdDeleteVersion.Parameters.Add("@DownloadFile", SqlDbType.NVarChar, 4000).Direction = ParameterDirection.Output;
                cmdDeleteVersion.Parameters("@DownloadFile").Value = "";

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdDeleteVersion.Transaction = savePoint;

                cmdDeleteVersion.ExecuteNonQuery();
                if (!cmdDeleteVersion.Parameters("@DownloadFile").Value == DBNull.Value)
                    strFiles = cmdDeleteVersion.Parameters("@DownloadFile").Value;
                savePoint.Commit();
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

    public bool _DeleteProductVersions(int ProductID, ref string strFiles, string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdDeleteProductVersions = sqlConn.CreateCommand;
            cmdDeleteProductVersions.CommandText = "_spKartrisVersions_DeleteByProduct";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdDeleteProductVersions.CommandType = CommandType.StoredProcedure;

            try
            {
                cmdDeleteProductVersions.Parameters.AddWithValue("@P_ID", ProductID);
                cmdDeleteProductVersions.Parameters.Add("@DownloadFiles", SqlDbType.NVarChar, 4000).Direction = ParameterDirection.Output;
                cmdDeleteProductVersions.Parameters("@DownloadFiles").Value = "";

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdDeleteProductVersions.Transaction = savePoint;

                cmdDeleteProductVersions.ExecuteNonQuery();

                if (!OptionsBLL._DeleteProductOptionGroupByProductID(ProductID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                if (!OptionsBLL._DeleteProductOptionsByProductID(ProductID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                savePoint.Commit();
                if (!cmdDeleteProductVersions.Parameters("@DownloadFiles").Value == DBNull.Value)
                    strFiles = cmdDeleteProductVersions.Parameters("@DownloadFiles").Value;
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
                    sqlConn.Close();
            }
        }
        return false;
    }

    public void _ChangeSortValue(long numVersionID, int numProductID, char chrDirection)
    {
        Adptr._ChangeSortValue(numVersionID, numProductID, chrDirection);
    }

    public bool _UpdateCustomerGroupPriceList(string strPriceList, ref int numLineNumber, ref string strMsg, ref bool blnUseVersionCode)
    {

        // We want to parse the price list row here first, identify the parameters and 
        // then send these to sproc, rather than try to do the parsing there which seems
        // to be far more complicated.
        int numCG_ID = 0;
        Int64 numCGP_VersionID = 0;
        string strVersionCode = "";
        double numCGP_Price = 0;
        Int64 numCounter = 0;

        string[] aryGroupPriceLines = strPriceList.Split(new char[] { '#' });
        VersionsBLL objVersionsBLL = new VersionsBLL();

        for (numCounter = 0; numCounter <= aryGroupPriceLines.Length - 1; numCounter++)
        {
            string[] aryThisLine = aryGroupPriceLines[numCounter].Split(new char[] { ',' });
            try
            {
                numCG_ID = System.Convert.ToInt32(aryThisLine[1]);
                if (blnUseVersionCode)
                    // We need to lookup the version ID from the code
                    numCGP_VersionID = objVersionsBLL._GetVersionIDByCodeNumber(aryThisLine[2]);
                else
                    numCGP_VersionID = System.Convert.ToInt64(aryThisLine[2]);
                numCGP_Price = Convert.ToDouble(aryThisLine[3]);

                // CG_Name','CG_ID','CGP_VersionID','CGP_Price'
                string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
                using (SqlConnection sqlConn = new SqlConnection(strConnString))
                {
                    SqlCommand cmd = sqlConn.CreateCommand;
                    cmd.CommandText = "_spKartrisVersions_UpdateCustomerGroupPriceList";
                    SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
                    cmd.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        cmd.Parameters.AddWithValue("@CG_ID", numCG_ID);
                        cmd.Parameters.AddWithValue("@CGP_VersionID", numCGP_VersionID);
                        cmd.Parameters.AddWithValue("@CGP_Price", numCGP_Price);

                        sqlConn.Open();
                        savePoint = sqlConn.BeginTransaction();
                        cmd.Transaction = savePoint;

                        cmd.ExecuteNonQuery();
                        savePoint.Commit();
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
                            sqlConn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        return true;
    }

    public bool _UpdatePriceList(string strPriceList, ref int numLineNumber, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisVersions_UpdatePriceList";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                cmd.Parameters.AddWithValue("@PriceList", strPriceList);
                cmd.Parameters.AddWithValue("@LineNumber", numLineNumber).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                cmd.ExecuteNonQuery();
                numLineNumber = cmd.Parameters("@LineNumber").Value;
                savePoint.Commit();
                return true;
            }
            catch (Exception ex)
            {
                numLineNumber = cmd.Parameters("@LineNumber").Value;
                if (!savePoint == null)
                    savePoint.Rollback();
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
