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

using kartrisProductsDataTableAdapters;
using System.Web.HttpContext;
using CkartrisFormatErrors;
using CkartrisDisplayFunctions;
using CkartrisEnumerations;
using CkartrisDataManipulation;
using KartSettingsManager;

public class ProductsBLL
{
    private ProductsTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;
    private ProductCategoryLinkTblAdptr _ProdcutCategoryLinkAdptr = null/* TODO Change to default(_) if this is not a reference type */;
    private RelatedProductsTblAdptr _RelatedAdptr = null/* TODO Change to default(_) if this is not a reference type */;

    protected ProductsTblAdptr Adptr
    {
        get
        {
            _Adptr = new ProductsTblAdptr();
            return _Adptr;
        }
    }

    protected ProductCategoryLinkTblAdptr ProductCategoryLinkAdptr
    {
        get
        {
            _ProdcutCategoryLinkAdptr = new ProductCategoryLinkTblAdptr();
            return _ProdcutCategoryLinkAdptr;
        }
    }

    protected RelatedProductsTblAdptr RelatedAdptr
    {
        get
        {
            _RelatedAdptr = new RelatedProductsTblAdptr();
            return _RelatedAdptr;
        }
    }

    public DataTable _GetRelatedProductsByParent(int intParentID)
    {
        return RelatedAdptr._GetRelatedProductsByParent(intParentID);
    }

    public DataTable GetProductDetailsByID(int _ProductID, short _LanguageID)
    {
        return Adptr.GetByID(_ProductID, _LanguageID);
    }

    public DataTable _GetProductsBySupplier(byte numLanguageID, short numSupplierID)
    {
        return Adptr._GetBySupplier(numLanguageID, numSupplierID);
    }

    public DataTable _GetFeaturedProducts(byte numLanguageID)
    {
        return _Adptr._GetFeaturedProducts(numLanguageID);
    }

    // We can use this in the back end to check easily
    // if an options product is a combinations product.
    // We can also use it on the front end to nullify
    // the UseCombinationPrices object config setting
    // for a product that is not a combinations product.
    public int _NumberOfCombinations(int _ProductID)
    {
        ProductsTblAdptr Adptr = new ProductsTblAdptr();
        DataTable dtbData = Adptr._NumberOfCombinations(_ProductID);
        int intCombinations = 0;
        foreach (DataRow drwData in dtbData.Rows)
        {
            intCombinations = drwData("Combinations");
            break;
        }
        return intCombinations;
    }

    public static bool _UpdateFeaturedProducts(DataTable tblFeaturedProducts, string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisProducts_UpdateAsFeatured";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.Add("@ProductID", SqlDbType.Int);
                cmd.Parameters.Add("@Featured", SqlDbType.TinyInt);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                if (!_DeleteFeaturedProducts(sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                foreach (DataRow row in tblFeaturedProducts.Rows)
                {
                    cmd.Parameters("@ProductID").Value = System.Convert.ToInt32(row("ProductID"));
                    cmd.Parameters("@Featured").Value = System.Convert.ToByte(row("Priority"));
                    cmd.ExecuteNonQuery();
                }

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

    private static bool _DeleteFeaturedProducts(SqlConnection sqlConn, SqlTransaction savePoint)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("_spKartrisProducts_DeleteFeaturedProducts", sqlConn, savePoint);
            cmd.CommandType = CommandType.StoredProcedure;
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

    // Cached Featured Products
    public static DataRow[] GetFeaturedProducts(byte numLanguageID)
    {
        return GetFeaturedProductsFromCache.Select("LANG_ID = " + numLanguageID);
    }

    public static DataTable GetFeaturedProductForCache()
    {
        ProductsTblAdptr Adptr = new ProductsTblAdptr();
        return Adptr.GetFeaturedProducts();
    }

    // Cached Newest Products
    public static DataRow[] GetNewestProducts(byte numLanguageID)
    {
        return GetNewestProductsFromCache.Select("LANG_ID = " + numLanguageID);
    }

    public static DataTable GetNewestProductsForCache()
    {
        DataTable tblNewestProducts = new DataTable();
        ProductsTblAdptr Adptr = new ProductsTblAdptr();
        foreach (DataRow rowLanguage in LanguagesBLL.GetLanguages.Rows)
            tblNewestProducts.Merge(Adptr.GetNewestProducts(rowLanguage("LANG_ID")), false);
        return tblNewestProducts;
    }

    // Cached Top List Products
    public static DataRow[] GetTopListProducts(byte numLanguageID)
    {
        return GetTopListProductsForCache.Select("LANG_ID = " + numLanguageID);
    }

    public static DataTable GetTopListProductsForCache()
    {
        DataTable tblTopListProducts = new DataTable();
        DateTime datRange = DateTime.Today.AddDays(-System.Convert.ToDouble(GetKartConfig("frontend.display.topsellers.days")));
        if (datRange == DateTime.Today)
            datRange = DateTime.Today.AddYears(-100);
        int intTopSellingCount = System.Convert.ToInt32(GetKartConfig("frontend.display.topsellers.quantity"));
        ProductsTblAdptr Adptr = new ProductsTblAdptr();
        foreach (DataRow rowLanguage in LanguagesBLL.GetLanguages.Rows)
            tblTopListProducts.Merge(Adptr.GetTopList(intTopSellingCount, rowLanguage("LANG_ID"), datRange), false);
        return tblTopListProducts;
    }

    public DataTable _SearchProductByName(string _Key, byte _LanguageID)
    {
        DataTable tbl = new DataTable();
        ProductsTblAdptr Adptr = new ProductsTblAdptr();
        tbl = Adptr._SearchByName(_Key, _LanguageID);
        if (tbl.Rows.Count == 0)
            tbl = Adptr._GetData(_LanguageID);
        return tbl;
    }

    public DataTable GetProductsPageByCategory(int _CategoryID, short _LanguageID, short _PageIndx, short _RowsPerPage, short _CGroupID, ref int _TotalNoOfProducts)
    {
        _TotalNoOfProducts = GetTotalProductsInCategory_s(_CategoryID, _LanguageID, _CGroupID);
        ProductsTblAdptr Adptr = new ProductsTblAdptr();
        return Adptr.GetProductsPageByCategoryID(_LanguageID, _CategoryID, _PageIndx, _RowsPerPage, _CGroupID);
    }
    public DataTable GetProductsPageByCategory(HttpRequest Request, int _CategoryID, short _LanguageID, short _PageIndx, short _RowsPerPage, short _CGroupID, ref int _TotalNoOfProducts)
    {
        if (Request.QueryString("f") == 1)
        {
            PowerpackBLL objPowerpackBLL = new PowerpackBLL();
            DataTable dtFilteredProducts = objPowerpackBLL.GetFilteredProductsByCategory(Request, _CategoryID, _LanguageID, _PageIndx, _RowsPerPage, _CGroupID, _TotalNoOfProducts);
            if (dtFilteredProducts != null)
                return dtFilteredProducts;
        }

        return GetProductsPageByCategory(_CategoryID, _LanguageID, _PageIndx, _RowsPerPage, _CGroupID, ref _TotalNoOfProducts);
    }

    public DataTable _GetProductsPageByCategory(int _CategoryID, short _LanguageID, short _PageIndx, short _RowsPerPage, ref int _TotalNoOfProducts)
    {
        _TotalNoOfProducts = _GetTotalProductsInCategory_s(_CategoryID, _LanguageID);
        ProductsTblAdptr Adptr = new ProductsTblAdptr();
        return Adptr._GetProductsPageByCategoryID(_LanguageID, _CategoryID, _PageIndx, _RowsPerPage);
    }

    public int _GetCustomerGroup(int numProductID)
    {
        ProductQTblAdptr qAdptr = new ProductQTblAdptr();
        int _CGroupID;
        qAdptr._GetCustomerGroup(numProductID, _CGroupID);
        return _CGroupID;
    }

    public object GetTotalProductsInCategory_s(int _CategoryID, short _LanguageID, short _CGroupID)
    {
        ProductQTblAdptr qAdptr = new ProductQTblAdptr();
        int totalProducts;
        qAdptr.GetTotalByCatID_s(_LanguageID, _CategoryID, _CGroupID, totalProducts);
        return totalProducts;
    }

    public object _GetTotalProductsInCategory_s(int _CategoryID, short _LanguageID)
    {
        ProductQTblAdptr qAdptr = new ProductQTblAdptr();
        int totalProducts;
        qAdptr._GetTotalByCatID_s(_LanguageID, _CategoryID, totalProducts);
        return totalProducts;
    }

    public double GetMinPriceByCG(int numProductID, int numCG_ID)
    {
        ProductQTblAdptr qAdptr = new ProductQTblAdptr();
        float MinPrice = 0.0F;
        qAdptr.GetMinPriceWithCG_s(numProductID, numCG_ID, MinPrice);
        return MinPrice;
    }

    public DataTable GetRelatedProducts(int _ProductID, short _LanguageId, short _CustomerGroupID)
    {
        ProductsTblAdptr Adptr = new ProductsTblAdptr();
        return Adptr.GetRelatedProducts(_ProductID, _LanguageId, _CustomerGroupID);
    }

    public DataTable GetPeopleWhoBoughtThis(int ProductID, short LanguageID, int numPeopleWhoBoughtThis)
    {
        bool intType;
        if (KartSettingsManager.GetKartConfig("frontend.crossselling.peoplewhoboughtthis.type") == "y")
            intType = true;
        else
            intType = false;
        ProductsTblAdptr Adptr = new ProductsTblAdptr();
        return Adptr.GetPeopleWhoBoughtThis(ProductID, LanguageID, numPeopleWhoBoughtThis, intType);
    }

    public DataTable GetParentCategories(int _ProductID, short _LanguageID)
    {
        ProductsTblAdptr Adptr = new ProductsTblAdptr();
        return Adptr.GetParentCategories(_LanguageID, _ProductID);
    }

    public string GetNameByProductID(int _ProductID, short _LanguageID)
    {
        LanguageElementsBLL objLanguageElementsBLL = new LanguageElementsBLL();
        return objLanguageElementsBLL.GetElementValue(_LanguageID, LANG_ELEM_TABLE_TYPE.Products, LANG_ELEM_FIELD_NAME.Name, _ProductID);
    }

    public string GetMetaDescriptionByProductID(int _ProductID, short _LanguageID)
    {
        LanguageElementsBLL objLanguageElementsBLL = new LanguageElementsBLL();
        string strMetaDescription = objLanguageElementsBLL.GetElementValue(_LanguageID, LANG_ELEM_TABLE_TYPE.Products, LANG_ELEM_FIELD_NAME.MetaDescription, _ProductID);
        if (string.IsNullOrEmpty(strMetaDescription) | strMetaDescription == "# -LE- #")
            strMetaDescription = objLanguageElementsBLL.GetElementValue(_LanguageID, LANG_ELEM_TABLE_TYPE.Products, LANG_ELEM_FIELD_NAME.Description, _ProductID);
        if (strMetaDescription == "# -LE- #")
            strMetaDescription = null;
        return Left(StripHTML(strMetaDescription), 160);
    }

    public string GetMetaKeywordsByProductID(int _ProductID, short _LanguageID)
    {
        LanguageElementsBLL objLanguageElementsBLL = new LanguageElementsBLL();
        string strMetaKeywords = objLanguageElementsBLL.GetElementValue(_LanguageID, LANG_ELEM_TABLE_TYPE.Products, LANG_ELEM_FIELD_NAME.MetaKeywords, _ProductID);
        if (strMetaKeywords == "# -LE- #")
            strMetaKeywords = null;
        return StripHTML(strMetaKeywords);
    }

    public string _GetNameByProductID(int _ProductID, short _LanguageID)
    {
        LanguageElementsBLL objLanguageElementsBLL = new LanguageElementsBLL();
        return objLanguageElementsBLL.GetElementValue(_LanguageID, LANG_ELEM_TABLE_TYPE.Products, LANG_ELEM_FIELD_NAME.Name, _ProductID);
    }

    public string GetAttributeValueByAttributeID_s(short _LanguageID, int _ProductID, int _AttributeID)
    {
        ProductQTblAdptr qAdptr = new ProductQTblAdptr();
        string _AttributeValue = "";
        qAdptr.GetAttributeValue_s(_ProductID, _AttributeID, _LanguageID, _AttributeValue);
        return _AttributeValue;
    }

    public char _GetProductType_s(int _ProductID)
    {
        ProductQTblAdptr qAdptr = new ProductQTblAdptr();
        char _ProductType = "";
        qAdptr._GetProductType_s(_ProductID, _ProductType);
        return _ProductType;
    }

    public DataTable _GetProductInfoByID(int pProductID)
    {
        ProductsTblAdptr Adptr = new ProductsTblAdptr();
        return Adptr._GetProductInfoByID(pProductID);
    }

    public DataTable _GetCategoriesByProductID(int pProductID)
    {
        ProductCategoryLinkTblAdptr ProductCategoryLinkAdptr = new ProductCategoryLinkTblAdptr();
        return ProductCategoryLinkAdptr._GetCategoriesByProductID(pProductID);
    }

    public int GetProductIDByVersionCode(string strVersionCode)
    {
        ProductsTblAdptr _Adptr = new ProductsTblAdptr();
        try
        {
            return _Adptr.GetProductIDByVersionCode(strVersionCode);
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    public void _GetProductStatus(int numProductID, ref bool blnProductLive, ref string strProductType, ref int numLiveVersions, ref int numLiveCategories, ref short numCustomerGroup)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisProducts_GetStatus";

            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@P_ID", numProductID);
                cmd.Parameters.AddWithValue("@ProductLive", false).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@ProductType", "").Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@NoOfLiveVersions", 0).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@NoOfLiveCategories", 0).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@ProductCustomerGroup", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                cmd.ExecuteNonQuery();

                blnProductLive = FixNullFromDB(cmd.Parameters("@ProductLive").Value);
                strProductType = FixNullFromDB(cmd.Parameters("@ProductType").Value);
                numLiveVersions = FixNullFromDB(cmd.Parameters("@NoOfLiveVersions").Value);
                numLiveCategories = FixNullFromDB(cmd.Parameters("@NoOfLiveCategories").Value);
                numCustomerGroup = FixNullFromDB(cmd.Parameters("@ProductCustomerGroup").Value);
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
    }

    public bool _DeleteProduct(int intProductID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdDeleteProduct = sqlConn.CreateCommand;
            cmdDeleteProduct.CommandText = "_spKartrisProducts_Delete";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdDeleteProduct.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdDeleteProduct.Parameters.AddWithValue("@ProductID", intProductID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdDeleteProduct.Transaction = savePoint;
                cmdDeleteProduct.ExecuteNonQuery();
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

    public int _AddProduct(DataTable ptblElements, string pParentsList, ref int pProductID, bool pLive, byte pFeatured, string pOrderVersionsBy, char pVersionsSortDirection, char pReviews, char pVersionDisplayType, int pSupplier, char pProductType, int pCustomerGroupID, ref string strMsg, ref bool blnIsClone = false)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisProducts_Add";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@NewP_ID", pProductID).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@P_Live", pLive);
                cmd.Parameters.AddWithValue("@P_Featured", pFeatured);
                cmd.Parameters.AddWithValue("@P_OrderVersionsBy", pOrderVersionsBy);
                cmd.Parameters.AddWithValue("@P_VersionsSortDirection", pVersionsSortDirection);
                cmd.Parameters.AddWithValue("@P_VersionDisplayType", pVersionDisplayType);
                cmd.Parameters.AddWithValue("@P_Reviews", pReviews);
                cmd.Parameters.AddWithValue("@P_SupplierID", FixNullToDB(pSupplier, "i"));
                cmd.Parameters.AddWithValue("@P_Type", pProductType);
                cmd.Parameters.AddWithValue("@P_CustomerGroupID", FixNullToDB(pCustomerGroupID, "i"));
                cmd.Parameters.AddWithValue("@NowOffset", CkartrisDisplayFunctions.NowOffset);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                // 1. Add The Main Info.
                cmd.ExecuteNonQuery();

                if (cmd.Parameters("@NewP_ID").Value == null || cmd.Parameters("@NewP_ID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                pProductID = cmd.Parameters("@NewP_ID").Value;

                // 2. Add the Language Elements
                if (!LanguageElementsBLL._AddLanguageElements(ptblElements, LANG_ELEM_TABLE_TYPE.Products, pProductID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                // 3. Add the Hierarchy
                if (!_UpdateProductCategories(pProductID, pParentsList, sqlConn, savePoint, ref strMsg))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                // 4. Add single version, only if single version type product
                // and not cloning (clones we will run a separate procedure
                // to create the version[s])
                if (pProductType == "s" & !blnIsClone)
                {
                    VersionsBLL objVersionsBLL = new VersionsBLL();
                    if (!objVersionsBLL._AddNewVersionAsSingle(_GetVersionElements(ptblElements), "SKU_" + System.Convert.ToString(pProductID), pProductID, pCustomerGroupID, sqlConn, savePoint, strMsg))
                        throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                }

                savePoint.Commit();
                sqlConn.Close();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");

                return pProductID;
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

        return -1;
    }

    // Creates records such as version(s), related product links, attribute values
    // etc. that are linked to a product
    public bool _CloneProductRecords(int pProductID_OLD, int pProductID_NEW)
    {
        string strMsg = "";
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisProducts_CloneRecords";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@P_ID_OLD", pProductID_OLD);
                cmd.Parameters.AddWithValue("@P_ID_NEW", pProductID_NEW);
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

    private DataTable _GetVersionElements(DataTable tblProductElements)
    {
        DataTable tblVersionElements = new DataTable();
        tblVersionElements.Columns.Add(new DataColumn("_LE_LanguageID"));
        tblVersionElements.Columns.Add(new DataColumn("_LE_FieldID"));
        tblVersionElements.Columns.Add(new DataColumn("_LE_Value"));

        foreach (DataRow row in tblProductElements.Rows)
        {
            int numType = System.Convert.ToInt32(FixNullFromDB(row("_LE_FieldID")));
            if (numType == LANG_ELEM_FIELD_NAME.Name || numType == LANG_ELEM_FIELD_NAME.Description)
                tblVersionElements.Rows.Add(row("_LE_LanguageID"), row("_LE_FieldID"), IIf(numType == LANG_ELEM_FIELD_NAME.Name, row("_LE_Value"), ""));
        }
        return tblVersionElements;
    }

    public bool _UpdateProduct(DataTable ptblElements, string pParentsList, int pProductID, bool pLive, byte pFeatured, string pOrderVersionsBy, char pVersionsSortDirection, char pReviews, char pVersionDisplayType, int pSupplier, char pProductType, int pCustomerGroupID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisProducts_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                cmd.Parameters.AddWithValue("@P_ID", pProductID);
                cmd.Parameters.AddWithValue("@P_Live", pLive);
                cmd.Parameters.AddWithValue("@P_Featured", pFeatured);
                cmd.Parameters.AddWithValue("@P_OrderVersionsBy", pOrderVersionsBy);
                cmd.Parameters.AddWithValue("@P_VersionsSortDirection", pVersionsSortDirection);
                cmd.Parameters.AddWithValue("@P_VersionDisplayType", pVersionDisplayType);
                cmd.Parameters.AddWithValue("@P_Reviews", pReviews);
                cmd.Parameters.AddWithValue("@P_SupplierID", FixNullToDB(pSupplier, "i"));
                cmd.Parameters.AddWithValue("@P_Type", pProductType);
                cmd.Parameters.AddWithValue("@P_CustomerGroupID", FixNullToDB(pCustomerGroupID, "i"));
                cmd.Parameters.AddWithValue("@NowOffset", CkartrisDisplayFunctions.NowOffset);

                // ' Needed to update the products' version (product' type is single version only)
                long numSingleVersionID = 0;
                VersionsBLL objVersionsBLL = new VersionsBLL();
                if (pProductType == "s")
                    numSingleVersionID = objVersionsBLL._GetSingleVersionByProduct(pProductID).Rows(0)("V_ID");

                int NoOfVersions = 0;
                if (pProductType == "o" || pProductType == "s")
                    NoOfVersions = objVersionsBLL._GetNoOfVersionsByProductID(pProductID);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                // 1. Add The Main Info.
                cmd.ExecuteNonQuery();

                // 2. Update the Language Elements
                if (!LanguageElementsBLL._UpdateLanguageElements(ptblElements, LANG_ELEM_TABLE_TYPE.Products, pProductID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                // 3. Add the Hierarchy
                if (!_UpdateProductCategories(pProductID, pParentsList, sqlConn, savePoint, ref strMsg))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                // 4. Check the product type
                if (pProductType == "o" && NoOfVersions == 1)
                {
                    if (!objVersionsBLL._SetVersionAsBaseByProductID(pProductID, sqlConn, savePoint, strMsg))
                        throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                }
                else if (pProductType == "s" && NoOfVersions == 1)
                {
                    // ' Update the versions' info, the versions will be readonly in the backend
                    if (!LanguageElementsBLL._UpdateLanguageElements(_GetVersionElements(ptblElements), LANG_ELEM_TABLE_TYPE.Versions, numSingleVersionID, sqlConn, savePoint))
                        throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));
                }

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

    private bool _UpdateProductCategories(int pProductID, string pNewParents, SqlConnection sqlConn, SqlTransaction savePoint, ref string strMsg)
    {
        try
        {
            if (pNewParents.EndsWith(","))
                pNewParents = pNewParents.TrimEnd(",");

            SqlCommand cmdAddParents = new SqlCommand("_spKartrisProductCategoryLink_AddParentList", sqlConn, savePoint);
            cmdAddParents.CommandType = CommandType.StoredProcedure;
            cmdAddParents.Parameters.AddWithValue("@ProductID", pProductID);
            cmdAddParents.Parameters.AddWithValue("@ParentList", pNewParents);

            cmdAddParents.ExecuteNonQuery();

            return true;
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
        }

        return false;
    }

    public bool _UpdateRelatedProducts(int intParentProduct, string strChildList, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdDeleteRelatedProducts = sqlConn.CreateCommand;
            cmdDeleteRelatedProducts.CommandText = "_spKartrisRelatedProducts_DeleteByParentID";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdDeleteRelatedProducts.CommandType = CommandType.StoredProcedure;

            try
            {
                cmdDeleteRelatedProducts.Parameters.AddWithValue("@ParentID", intParentProduct);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdDeleteRelatedProducts.Transaction = savePoint;

                cmdDeleteRelatedProducts.ExecuteNonQuery();

                if (strChildList.EndsWith(","))
                    strChildList = strChildList.TrimEnd(",");

                SqlCommand cmdAddRelatedProducts = new SqlCommand("_spKartrisRelatedProducts_AddChildList", sqlConn, savePoint);
                cmdAddRelatedProducts.CommandType = CommandType.StoredProcedure;
                cmdAddRelatedProducts.Parameters.AddWithValue("@ParentID", intParentProduct);
                cmdAddRelatedProducts.Parameters.AddWithValue("@ChildList", strChildList);
                cmdAddRelatedProducts.ExecuteNonQuery();

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

    public bool _DeleteRelatedProducts(int intParentProduct, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdDeleteRelatedProducts = sqlConn.CreateCommand;
            cmdDeleteRelatedProducts.CommandText = "_spKartrisRelatedProducts_DeleteByParentID";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdDeleteRelatedProducts.CommandType = CommandType.StoredProcedure;

            try
            {
                cmdDeleteRelatedProducts.Parameters.AddWithValue("@ParentID", intParentProduct);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdDeleteRelatedProducts.Transaction = savePoint;
                cmdDeleteRelatedProducts.ExecuteNonQuery();
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

    public void _ChangeSortValue(int numCategoryID, int numProductID, char chrDirection)
    {
        ProductCategoryLinkTblAdptr ProductCategoryLinkAdptr = new ProductCategoryLinkTblAdptr();
        ProductCategoryLinkAdptr._ChangeSortValue(numProductID, numCategoryID, chrDirection);
    }

    public DataTable GetRichSnippetProperties(int numProductID, byte numLanguageID)
    {
        ProductsTblAdptr Adptr = new ProductsTblAdptr();
        return Adptr.GetRichSnippetProperties(numProductID, numLanguageID);
    }

    // Set whether product live or not
    public bool _HideShowAllByCategoryID(int pCategoryID, bool pLive)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisProducts_HideShowAllByCategoryID";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                cmd.Parameters.AddWithValue("@CAT_ID", pCategoryID);
                cmd.Parameters.AddWithValue("@P_Live", pLive);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                cmd.ExecuteNonQuery();

                savePoint.Commit();
                sqlConn.Close();

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

    // v3.3000
    // For performance, we now use the new tblKartrisProductSearchIndex table
    // to store max/min prices for products, calculated for any versions,
    // options and qty discounts they have. This saves us having to do that
    // calculation in real time, since it will only change when versions, 
    // options or qty discounts are updated. So we just trigger it then on
    // that product. We also include this function, so you can manually 
    // trigger a full rebuild for the site.
    public static bool _RebuildPriceIndex()
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisProducts_RebuildPriceIndex";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                cmd.ExecuteNonQuery();

                savePoint.Commit();
                sqlConn.Close();

                return true;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
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
