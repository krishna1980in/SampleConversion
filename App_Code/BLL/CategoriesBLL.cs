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

using CkartrisEnumerations;
using CkartrisDataManipulation;
using System.Web.HttpContext;
using CkartrisFormatErrors;
using kartrisCategoriesData;
using kartrisCategoriesDataTableAdapters;
using CkartrisDisplayFunctions;

public class CategoriesBLL
{
    private CategoriesTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;
    private CategoryHierarchyTblAdptr _AdptrHierarchy = null/* TODO Change to default(_) if this is not a reference type */;

    protected CategoriesTblAdptr Adptr
    {
        get
        {
            _Adptr = new CategoriesTblAdptr();
            return _Adptr;
        }
    }

    protected CategoryHierarchyTblAdptr AdptrHierarchy
    {
        get
        {
            _AdptrHierarchy = new CategoryHierarchyTblAdptr();
            return _AdptrHierarchy;
        }
    }

    public DataTable _GetWithProducts(short _LanguageID)
    {
        return Adptr._GetWithProductsOnly(_LanguageID);
    }

    public DataTable _SearchTopLevelCategoryByName(string _Key, byte _LanguageID)
    {
        DataTable tbl = new DataTable();
        tbl = Adptr._SearchTopLevelByName(_Key, _LanguageID);
        return tbl;
    }

    public DataTable _SearchCategoryByName(string _Key, byte _LanguageID)
    {
        DataTable tbl = new DataTable();
        tbl = Adptr._SearchByName(_Key, _LanguageID);
        if (tbl.Rows.Count == 0)
            tbl = Adptr.GetData(_LanguageID);
        return tbl;
    }

    public DataTable GetCategoryByID(int _CategoryID, short _LanguageID)
    {
        return Adptr.GetByCategoryID(_CategoryID, _LanguageID);
    }

    public DataTable GetCategoriesByProductID(int _ProductID, short _LanguageID)
    {
        return Adptr.GetByProductID(_ProductID, _LanguageID);
    }

    public DataTable GetCategoriesPageByParentID(int _ParentCategoryID, short _LanguageID, short _PageIndx, short _RowsPerPage, short _CGroupID, ref int _TotalNoOfCategories)
    {
        _TotalNoOfCategories = GetTotalCategoriesByParentID_o(_LanguageID, _ParentCategoryID, _CGroupID);
        return Adptr.GetCategoriesPageByParentID(_LanguageID, _ParentCategoryID, _PageIndx, _RowsPerPage, _CGroupID);
    }

    // New in v3, new treeview function grabs main categories but also
    // appends sub site cats too
    public DataTable _Treeview(short _LanguageID)
    {
        return Adptr._Treeview(_LanguageID);
    }

    public DataTable _GetCategoriesPageByParentID(int _ParentCategoryID, short _LanguageID, short _PageIndx, short _RowsPerPage, ref int _TotalNoOfCategories)
    {
        _TotalNoOfCategories = _GetTotalCategoriesByParentID_o(_LanguageID, _ParentCategoryID);
        return Adptr._GetCategoriesPageByParentID(_LanguageID, _ParentCategoryID, _PageIndx, _RowsPerPage);
    }

    public int _GetTotalCategoriesByParentID_o(short _LanguageID, int _ParentCategoryID)
    {
        int totalCategories;
        CategoriesQTblAdptr qAdptr = new CategoriesQTblAdptr();
        qAdptr._GetTotalByParentID_o(_LanguageID, _ParentCategoryID, totalCategories);
        return totalCategories;
    }

    public int GetTotalCategoriesByParentID_o(short _LanguageID, int _ParentCategoryID, short _CGroupID)
    {
        int totalCategories;
        CategoriesQTblAdptr qAdptr = new CategoriesQTblAdptr();
        qAdptr.GetTotalByParentID_o(_LanguageID, _ParentCategoryID, _CGroupID, totalCategories);
        return totalCategories;
    }

    public DataTable GetHierarchyByLanguageID(short _LanguageID)
    {
        bool blnSortByName = false;
        if (LCase(KartSettingsManager.GetKartConfig("frontend.categories.display.sortdefault")) == "cat_name")
            blnSortByName = true;
        return AdptrHierarchy.GetHierarchyByLanguage(_LanguageID, blnSortByName);
    }

    public DataTable _GetHierarchyByLanguageId(short _LanguageID)
    {
        return AdptrHierarchy._GetHierarchyByLanguage(_LanguageID);
    }

    public string GetNameByCategoryID(int _CategoryID, short _LanguageID)
    {
        LanguageElementsBLL objLanguageElementsBLL = new LanguageElementsBLL();
        return objLanguageElementsBLL.GetElementValue(_LanguageID, LANG_ELEM_TABLE_TYPE.Categories, LANG_ELEM_FIELD_NAME.Name, _CategoryID);
    }

    public string GetMetaDescriptionByCategoryID(int _CategoryID, short _LanguageID)
    {
        LanguageElementsBLL objLanguageElementsBLL = new LanguageElementsBLL();
        string strMetaDescription = objLanguageElementsBLL.GetElementValue(_LanguageID, LANG_ELEM_TABLE_TYPE.Categories, LANG_ELEM_FIELD_NAME.MetaDescription, _CategoryID);
        if (string.IsNullOrEmpty(strMetaDescription) | strMetaDescription == "# -LE- #")
            strMetaDescription = objLanguageElementsBLL.GetElementValue(_LanguageID, LANG_ELEM_TABLE_TYPE.Categories, LANG_ELEM_FIELD_NAME.Description, _CategoryID);
        if (strMetaDescription == "# -LE- #")
            strMetaDescription = "";
        return Left(StripHTML(strMetaDescription), 160);
    }

    public string GetMetaKeywordsByCategoryID(int _CategoryID, short _LanguageID)
    {
        LanguageElementsBLL objLanguageElementsBLL = new LanguageElementsBLL();
        string strMetaKeywords = objLanguageElementsBLL.GetElementValue(_LanguageID, LANG_ELEM_TABLE_TYPE.Categories, LANG_ELEM_FIELD_NAME.MetaKeywords, _CategoryID);
        if (strMetaKeywords == "# -LE- #")
            strMetaKeywords = "";
        return StripHTML(strMetaKeywords);
    }

    public string _GetNameByCategoryID(int _CategoryID, short _LanguageID)
    {
        LanguageElementsBLL objLanguageElementsBLL = new LanguageElementsBLL();
        return objLanguageElementsBLL.GetElementValue(_LanguageID, LANG_ELEM_TABLE_TYPE.Categories, LANG_ELEM_FIELD_NAME.Name, _CategoryID);
    }

    public int _GetTotalCategoriesByLanguageID(byte numLanguageID)
    {
        int numTotalCategories = 0;
        Adptr._GetTotalCategoriesByLanguageID(numLanguageID, numTotalCategories);
        return numTotalCategories;
    }

    public DataTable _GetParentsByID(byte pLanguageID, int pChildID)
    {
        return AdptrHierarchy._GetParentsByID(pLanguageID, pChildID);
    }

    public DataTable _GetByID(int pCategoryID)
    {
        return Adptr._GetByCategoryID(pCategoryID);
    }

    public short _GetTotalSubCategories_s(int CategoryID)
    {
        short totalSubcategories = 0;
        AdptrHierarchy._GetTotalSubcategories(CategoryID, totalSubcategories);
        return totalSubcategories;
    }

    public void _GetCategoryStatus(int numCategoryID, ref bool blnCategoryLive, ref int numLiveParents, ref short numCustomerGroup)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisCategories_GetStatus";
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                cmd.Parameters.AddWithValue("@CAT_ID", numCategoryID);
                cmd.Parameters.AddWithValue("@CategoryLive", false).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@NoOfLiveParents", 0).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@CategoryCustomerGroup", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                cmd.ExecuteNonQuery();

                blnCategoryLive = FixNullFromDB(cmd.Parameters("@CategoryLive").Value);
                numLiveParents = FixNullFromDB(cmd.Parameters("@NoOfLiveParents").Value);
                numCustomerGroup = FixNullFromDB(cmd.Parameters("@CategoryCustomerGroup").Value);
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

    public bool _AddCategory(DataTable ptblElements, string pParentsList, ref int pCategoryID, bool pLive, char pProductDisplayType, char pSubCatDisplayType, string pOrderProductsBy, char pProductsSortDirection, string pOrderSubcatBy, char pSubcatSortDirection, int pCustomerGroupID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisCategories_Add";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                cmd.Parameters.AddWithValue("@CAT_Live", pLive);
                cmd.Parameters.AddWithValue("@CAT_ProductDisplayType", pProductDisplayType);
                cmd.Parameters.AddWithValue("@CAT_SubCatDisplayType", pSubCatDisplayType);
                cmd.Parameters.AddWithValue("@CAT_OrderProductsBy", pOrderProductsBy);
                cmd.Parameters.AddWithValue("@CAT_ProductsSortDirection", pProductsSortDirection);
                cmd.Parameters.AddWithValue("@CAT_CustomerGroupID", FixNullToDB(pCustomerGroupID, "i"));
                cmd.Parameters.AddWithValue("@CAT_OrderCategoriesBy", pOrderSubcatBy);
                cmd.Parameters.AddWithValue("@CAT_CategoriesSortDirection", pSubcatSortDirection);
                cmd.Parameters.AddWithValue("@NewCAT_ID", pCategoryID).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                // ' 1. Add The Main Info.
                cmd.ExecuteNonQuery();

                if (cmd.Parameters("@NewCAT_ID").Value == null || cmd.Parameters("@NewCAT_ID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                pCategoryID = FixNullFromDB(cmd.Parameters("@NewCAT_ID").Value);

                // ' 2. Add the Language Elements
                if (!LanguageElementsBLL._AddLanguageElements(ptblElements, LANG_ELEM_TABLE_TYPE.Categories, pCategoryID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                // ' 3. Add the Hierarchy
                if (!_UpdateCategoryHierarchy(pCategoryID, pParentsList, sqlConn, savePoint))
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

    public bool _UpdateCategory(DataTable ptblElements, string pParentsList, int pCategoryID, bool pLive, char pProductDisplayType, char pSubCatDisplayType, string pOrderProductsBy, char pProductsSortDirection, string pOrderSubcatBy, char pSubcatSortDirection, int pCustomerGroupID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisCategories_Update";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@CAT_ID", pCategoryID);
                cmd.Parameters.AddWithValue("@CAT_Live", pLive);
                cmd.Parameters.AddWithValue("@CAT_ProductDisplayType", pProductDisplayType);
                cmd.Parameters.AddWithValue("@CAT_SubCatDisplayType", pSubCatDisplayType);
                cmd.Parameters.AddWithValue("@CAT_OrderProductsBy", pOrderProductsBy);
                cmd.Parameters.AddWithValue("@CAT_ProductsSortDirection", pProductsSortDirection);
                cmd.Parameters.AddWithValue("@CAT_CustomerGroupID", FixNullToDB(pCustomerGroupID, "i"));
                cmd.Parameters.AddWithValue("@CAT_OrderCategoriesBy", pOrderSubcatBy);
                cmd.Parameters.AddWithValue("@CAT_CategoriesSortDirection", pSubcatSortDirection);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                if (pCategoryID == 0)
                {
                    if (!LanguageElementsBLL._UpdateLanguageElements(ptblElements, LANG_ELEM_TABLE_TYPE.Categories, pCategoryID, sqlConn, savePoint))
                        throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                    savePoint.Commit();
                    sqlConn.Close();
                    strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
                    return true;
                }

                // ' 1. Update The Main Info.
                cmd.ExecuteNonQuery();

                // ' 2. Update the Language Elements
                if (!LanguageElementsBLL._UpdateLanguageElements(ptblElements, LANG_ELEM_TABLE_TYPE.Categories, pCategoryID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                // ' 3. Add the Hierarchy
                // Need to remove the empty check, because we want to allow categories
                // to be saved without parents (to make them top level cats)
                if (!string.IsNullOrEmpty(pParentsList))
                {
                    if (!_UpdateCategoryHierarchy(pCategoryID, pParentsList, sqlConn, savePoint))
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

    private bool _UpdateCategoryHierarchy(int pChildID, string pNewParents, SqlConnection sqlConn, SqlTransaction savePoint)
    {
        SqlCommand cmd = new SqlCommand("_spKartrisCategoryHierarchy_DeleteByChild", sqlConn, savePoint);
        cmd.CommandType = CommandType.StoredProcedure;
        try
        {
            cmd.Parameters.AddWithValue("@ChildID", pChildID);
            cmd.ExecuteNonQuery();

            if (pNewParents.EndsWith(","))
                pNewParents = pNewParents.TrimEnd(",");
            if (pNewParents.Length == 0)
                pNewParents = "0";

            SqlCommand cmdAddParents = new SqlCommand("_spKartrisCategoryHierarchy_AddParentList", sqlConn, savePoint);
            cmdAddParents.CommandType = CommandType.StoredProcedure;
            cmdAddParents.Parameters.AddWithValue("@ParentList", pNewParents);
            cmdAddParents.Parameters.AddWithValue("@ChildID", pChildID);
            cmdAddParents.ExecuteNonQuery();

            return true;
        }
        catch (SqlException ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
        finally
        {
        }
        // End Using
        return false;
    }

    public bool _DeleteCategory(int intCategoryID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdDeleteCategory = sqlConn.CreateCommand;
            cmdDeleteCategory.CommandText = "_spKartrisCategories_Delete";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdDeleteCategory.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdDeleteCategory.Parameters.AddWithValue("@CAT_ID", intCategoryID);
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdDeleteCategory.Transaction = savePoint;

                cmdDeleteCategory.ExecuteNonQuery();

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

    public void _ChangeSortValue(int numParentID, int numCategoryID, char chrDirection)
    {
        AdptrHierarchy._ChangeSortValue(numParentID, numCategoryID, chrDirection);
    }

    // ' Delete Category Cascade (with subcategories)
    public bool _DeleteCategoryCascade(int intCategoryID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            try
            {
                sqlConn.Open();
                _RecursiveCategoryDelete(intCategoryID, sqlConn);
                sqlConn.Close();
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
    public bool _RecursiveCategoryDelete(int intCategoryID, SqlConnection sqlConn)
    {
        DataTable tblChildCategories = AdptrHierarchy._GetSubcategoriesIDs(intCategoryID);
        foreach (DataRow child in tblChildCategories.Rows)
        {

            // ' Check Parent Categories
            DataTable tblParentCategories = AdptrHierarchy._GetOtherParents(child("CH_ChildID"), intCategoryID);
            if (tblParentCategories.Rows.Count == 0)
                // ' If there is no other parents, then delete recursive
                _RecursiveCategoryDelete(child("CH_ChildID"), sqlConn);

            // ' Delete Hierarchy Link (Child, Parent)
            if (!_DeleteHierarchyLink(child("CH_ChildID"), intCategoryID, sqlConn))
                return false;
        }

        return _DeleteCategoryWithoutTransaction(intCategoryID, sqlConn);
    }
    public bool _DeleteCategoryWithoutTransaction(int numCategoryID, SqlConnection sqlConn)
    {
        SqlCommand cmdDeleteCategory = sqlConn.CreateCommand;
        cmdDeleteCategory.CommandText = "_spKartrisCategories_Delete";
        cmdDeleteCategory.CommandType = CommandType.StoredProcedure;
        cmdDeleteCategory.Parameters.AddWithValue("@CAT_ID", numCategoryID);

        try
        {
            cmdDeleteCategory.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
        finally
        {
        }

        return false;
    }
    public bool _DeleteHierarchyLink(int numChildID, int numParentID, SqlConnection sqlConn)
    {
        SqlCommand cmd = sqlConn.CreateCommand;
        cmd.CommandText = "_spKartrisCategoryHierarchy_DeleteLink";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ChildID", numChildID);
        cmd.Parameters.AddWithValue("@ParentID", numParentID);
        try
        {
            cmd.ExecuteNonQuery();

            return true;
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
        finally
        {
        }
        return false;
    }
}
