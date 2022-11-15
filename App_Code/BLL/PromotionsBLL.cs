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

using System.Web.HttpContext;
using CkartrisFormatErrors;
using CkartrisEnumerations;
using kartrisPromotionsData;
using kartrisPromotionsDataTableAdapters;
using CkartrisDisplayFunctions;
using CkartrisDataManipulation;
using SiteMapHelper;

public class PromotionsBLL
{
    private static PromotionsTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;
    private static PromotionPartsTblAdptr _AdptrParts = null/* TODO Change to default(_) if this is not a reference type */;
    private static PromotionStringsTblAdptr _AdptrPromotionString = null/* TODO Change to default(_) if this is not a reference type */;

    protected static PromotionsTblAdptr Adptr
    {
        get
        {
            _Adptr = new PromotionsTblAdptr();
            return _Adptr;
        }
    }

    protected static PromotionPartsTblAdptr AdptrParts
    {
        get
        {
            _AdptrParts = new PromotionPartsTblAdptr();
            return _AdptrParts;
        }
    }

    protected static PromotionStringsTblAdptr AdptrStrings
    {
        get
        {
            _AdptrPromotionString = new PromotionStringsTblAdptr();
            return _AdptrPromotionString;
        }
    }

    public static DataTable _GetData()
    {
        return Adptr._GetData();
    }
    public static DataTable _GetPromotionByID(int PromotionID)
    {
        return Adptr._GetByID(PromotionID);
    }
    public static DataTable GetPromotionsByProductID(int _ProductID, short _LanguageID, int numPromotionID)
    {
        return Adptr.GetByProductID(_ProductID, _LanguageID, NowOffset, numPromotionID);
    }
    public static string GetPromotionName(int numPromotionID, short numLanguageID)
    {
        LanguageElementsBLL objLanguageElementsBLL = new LanguageElementsBLL();
        string strName = objLanguageElementsBLL.GetElementValue(numLanguageID, LANG_ELEM_TABLE_TYPE.Promotions, LANG_ELEM_FIELD_NAME.Name, numPromotionID);
        if (strName == "# -LE- #")
            return null;
        return strName;
    }

    public static DataTable GetAllPromotions(short _LanguageID, int numPromotionID)
    {
        return Adptr.GetAllPromotions(_LanguageID, NowOffset, numPromotionID);
    }

    public static DataTable _GetAllPromotions(short LanguageID)
    {
        return Adptr._GetAllPromotions(LanguageID);
    }

    public static DataTable _GetPromotionStringsByType(char charType, byte languageID)
    {
        return AdptrStrings._GetByType(charType, languageID);
    }
    public static DataTable _GetPromotionString(byte bytPromotionStringID, byte languageID)
    {
        return AdptrStrings._GetByID(bytPromotionStringID, languageID);
    }

    public static DataTable _GetByPartsAndPromotion(char chrPart, int intPromotionID, byte languageID)
    {
        return AdptrParts._GetByPartsPromotionID(chrPart, intPromotionID, languageID);
    }
    public static DataTable _GetPartsByPromotion(int intPromotionID, byte languageID)
    {
        return AdptrParts._GetByPromotionID(intPromotionID, languageID);
    }

    public static bool _AddNewPromotion(DataTable tblElements, ref int intPromotionID, DateTime dtStartDate, DateTime dtEndDate, byte intMaxQty, short intOrderNo, bool blnLive, DataTable tblParts, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdAddPromotion = sqlConn.CreateCommand;
            cmdAddPromotion.CommandText = "_spKartrisPromotions_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdAddPromotion.CommandType = CommandType.StoredProcedure;

            try
            {
                cmdAddPromotion.Parameters.AddWithValue("@PROM_StartDate", dtStartDate);
                cmdAddPromotion.Parameters.AddWithValue("@PROM_EndDate", dtEndDate);
                cmdAddPromotion.Parameters.AddWithValue("@PROM_Live", blnLive);
                cmdAddPromotion.Parameters.AddWithValue("@PROM_OrderByValue", intOrderNo);
                cmdAddPromotion.Parameters.AddWithValue("@PROM_MaxQuantity", intMaxQty);
                cmdAddPromotion.Parameters.AddWithValue("@NewPROM_ID", 0).Direction = ParameterDirection.Output;

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdAddPromotion.Transaction = savePoint;


                cmdAddPromotion.ExecuteNonQuery();

                if (cmdAddPromotion.Parameters("@NewPROM_ID").Value == null || cmdAddPromotion.Parameters("@NewPROM_ID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                intPromotionID = cmdAddPromotion.Parameters("@NewPROM_ID").Value;
                if (!LanguageElementsBLL._AddLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.Promotions, intPromotionID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                foreach (DataRow row in tblParts.Rows)
                    row("PROM_ID") = intPromotionID;

                if (!_AddPromotionParts(tblParts, sqlConn, savePoint, ref strMsg))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

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
    public static bool _UpdatePromotion(DataTable tblElements, int intPromotionID, DateTime dtStartDate, DateTime dtEndDate, byte intMaxQty, short intOrderNo, bool blnLive, DataTable tblParts, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdatePromotion = sqlConn.CreateCommand;
            cmdUpdatePromotion.CommandText = "_spKartrisPromotions_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdatePromotion.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdatePromotion.Parameters.AddWithValue("@PROM_ID", intPromotionID);
                cmdUpdatePromotion.Parameters.AddWithValue("@PROM_StartDate", dtStartDate);
                cmdUpdatePromotion.Parameters.AddWithValue("@PROM_EndDate", dtEndDate);
                cmdUpdatePromotion.Parameters.AddWithValue("@PROM_Live", blnLive);
                cmdUpdatePromotion.Parameters.AddWithValue("@PROM_OrderByValue", intOrderNo);
                cmdUpdatePromotion.Parameters.AddWithValue("@PROM_MaxQuantity", intMaxQty);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdatePromotion.Transaction = savePoint;

                cmdUpdatePromotion.ExecuteNonQuery();

                if (!LanguageElementsBLL._UpdateLanguageElements(tblElements, LANG_ELEM_TABLE_TYPE.Promotions, intPromotionID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                foreach (DataRow row in tblParts.Rows)
                    row("PROM_ID") = intPromotionID;

                if (!_DeletePromotionParts(intPromotionID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                if (!_AddPromotionParts(tblParts, sqlConn, savePoint, ref strMsg))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

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

    public static string GetPromotionText(int intPromotionID, bool blnTextOnly = false)
    {
        DataTable tblPromotionParts = new DataTable();    // '==== language_ID =====
        tblPromotionParts = PromotionsBLL._GetPartsByPromotion(intPromotionID, System.Web.HttpContext.Current.Session["LANG"]);

        string strPromotionText = "";
        int intTextCounter = 0;
        long numLanguageID;

        numLanguageID = System.Web.HttpContext.Current.Session["LANG"];

        CategoriesBLL objCategoriesBLL = new CategoriesBLL();
        ProductsBLL objProductsBLL = new ProductsBLL();
        VersionsBLL objVersionsBLL = new VersionsBLL();

        foreach (DataRow drwPromotionParts in tblPromotionParts.Rows)
        {
            string strText = drwPromotionParts("PS_Text");
            string strStringID = drwPromotionParts("PS_ID");
            string strValue = CkartrisDisplayFunctions.FixDecimal(FixNullFromDB(drwPromotionParts("PP_Value")));
            string strItemID = FixNullFromDB(drwPromotionParts("PP_ItemID"));
            int intProductID = objVersionsBLL.GetProductID_s(System.Convert.ToInt64(strItemID));
            string strItemName = "";
            string strItemLink = "";

            if (strText.Contains("[X]"))
            {
                if (strText.Contains("[£]"))
                    strText = strText.Replace("[X]", CurrenciesBLL.FormatCurrencyPrice(System.Web.HttpContext.Current.Session["CUR_ID"], CurrenciesBLL.ConvertCurrency(System.Web.HttpContext.Current.Session["CUR_ID"], drwPromotionParts("PP_Value"))));
                else
                    strText = strText.Replace("[X]", strValue);
            }

            if (strText.Contains("[C]") && strItemID != "")
            {
                strItemName = objCategoriesBLL.GetNameByCategoryID(System.Convert.ToInt32(strItemID), numLanguageID);
                strItemLink = " <b><a href='" + CreateURL(Page.CanonicalCategory, strItemID) + "'>" + strItemName + "</a></b>";
                strItemLink = Interaction.IIf(blnTextOnly, strItemName, strItemLink);
                strText = strText.Replace("[C]", strItemLink);
            }

            if (strText.Contains("[P]") && strItemID != "")
            {
                strItemName = objProductsBLL.GetNameByProductID(System.Convert.ToInt32(strItemID), numLanguageID);
                strItemLink = " <b><a href='" + CreateURL(Page.CanonicalProduct, strItemID) + "'>" + strItemName + "</a></b>";
                strItemLink = Interaction.IIf(blnTextOnly, strItemName, strItemLink);
                strText = strText.Replace("[P]", strItemLink);
            }

            if (strText.Contains("[V]") && strItemID != "")
            {
                strItemName = objProductsBLL.GetNameByProductID(intProductID, numLanguageID) + " (" + objVersionsBLL._GetNameByVersionID(System.Convert.ToInt32(strItemID), numLanguageID) + ")";
                strItemLink = " <b><a href='" + CreateURL(Page.CanonicalProduct, intProductID) + "'>" + strItemName + "</a></b>";
                strItemLink = Interaction.IIf(blnTextOnly, strItemName, strItemLink);
                strText = strText.Replace("[V]", strItemLink);
            }

            if (strText.Contains("[£]"))
                strText = strText.Replace("[£]", "");

            intTextCounter += 1;
            if (intTextCounter > 1)
                strPromotionText += ", ";
            strPromotionText += strText;
        }

        return strPromotionText;
    }

    private static bool _DeletePromotionParts(int intPromotionID, SqlConnection sqlConn, SqlTransaction savePoint)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("_spKartrisPromotionParts_DeleteByPromotionID", sqlConn, savePoint);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PromotionID", intPromotionID);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
        return false;
    }
    private static bool _AddPromotionParts(DataTable tblParts, SqlConnection sqlConn, SqlTransaction savePoint, ref string strMsg)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("_spKartrisPromotionParts_Add", sqlConn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = savePoint;
            foreach (DataRow row in tblParts.Rows)
            {
                cmd.Parameters.AddWithValue("@PROM_ID", System.Convert.ToInt32(row("PROM_ID")));
                cmd.Parameters.AddWithValue("@PP_PartNo", System.Convert.ToChar(row("PP_PartNo")));
                cmd.Parameters.AddWithValue("@PP_ItemType", System.Convert.ToChar(row("PP_ItemType")));
                cmd.Parameters.AddWithValue("@PP_ItemID", System.Convert.ToInt64(row("PP_ItemID")));
                cmd.Parameters.AddWithValue("@PP_Type", System.Convert.ToChar(row("PP_Type")));
                cmd.Parameters.AddWithValue("@PP_Value", System.Convert.ToDouble(row("PP_Value")));
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            return true;
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
        }

        return false;
    }
}
