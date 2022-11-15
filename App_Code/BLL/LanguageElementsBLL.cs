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

using kartrisLanguageData;
using kartrisLanguageDataTableAdapters;
using CkartrisEnumerations;
using CkartrisDataManipulation;
using CkartrisFormatErrors;

public class LanguageElementsBLL
{
    private static LanguageElementsTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;

    protected static LanguageElementsTblAdptr Adptr
    {
        get
        {
            _Adptr = new LanguageElementsTblAdptr();
            return _Adptr;
        }
    }

    public static LanguageElementsDataTable GetLanguageElements()
    {
        return Adptr.GetData();
    }

    public static LanguageElementsDataTable GetLanguageElementsByLanguageID(Int16 _LanguageID)
    {
        return Adptr.GetDataByLanguageID(_LanguageID);
    }

    public string GetElementValue(short _LanguageID, LANG_ELEM_TABLE_TYPE _TypeID, LANG_ELEM_FIELD_NAME _FieldID, long _ParentID)
    {
        // In v2.9014 we modified this a little to make it more resilient. From time
        // to time, sites with very large numbers of products seem to get OOps messages
        // relating to the retrieval of language elements. Generally refreshing the page
        // clears the error. So what this does is if an error occurs retrieving a value, it
        // waits 20ms, then tries again. Generally this will work ok, but if not, we
        // then return blank. The idea is generally that if we encounter an error, we try
        // again after a little wait, and hopefully it works and the 20ms will be 
        // unnoticeable to users, but in the worst case, we may have a value missing on the
        // page which shouldn't be a world ending event and acceptable if we avoid a big
        // page blow-up "oops" message.
        string strValue = "";
        try
        {
            strValue = FixNullFromDB(Adptr.GetElementValue_s(_LanguageID, _TypeID, _FieldID, _ParentID));
        }
        catch (Exception ex)
        {
            // Wait a little, then try again
            System.Threading.Thread.Sleep(20);
            try
            {
                strValue = FixNullFromDB(Adptr.GetElementValue_s(_LanguageID, _TypeID, _FieldID, _ParentID));
            }
            catch (Exception ex2)
            {
            }
        }

        if (strValue == null)
            strValue = "# -LE- #"; // ' The string is not found
        return strValue;
    }

    public static DataTable _GetElementsByTypeAndParent(byte pTypeID, long pParentID)
    {
        return Adptr._GetByTypeAndParent(pTypeID, pParentID);
    }

    public static DataTable _GetLanguageElementsSchema()
    {
        return Adptr.GetDataByLanguageID(0);
    }

    public static DataTable _GetTotalsPerLanguage()
    {
        return Adptr.GetTotalsPerLanguage();
    }

    public static bool _FixLanguageElements(byte numSourceLanguageID, byte numDistinationLanguageID, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand;
            cmd.CommandText = "_spKartrisLanguageElements_FixMissingElements";

            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.AddWithValue("@SourceLanguage", FixNullToDB(numSourceLanguageID, "i"));
                cmd.Parameters.AddWithValue("@DistinationLanguage", FixNullToDB(numDistinationLanguageID, "i"));

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

    public static bool _AddLanguageElements(DataTable ptblElements, LANG_ELEM_TABLE_TYPE enumType, long ItemID, SqlClient.SqlConnection sqlConn, SqlClient.SqlTransaction savePoint)
    {
        try
        {
            SqlClient.SqlCommand cmd = new SqlClient.SqlCommand("_spKartrisLanguageElements_Add", sqlConn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = savePoint;
            foreach (DataRow row in ptblElements.Rows)
            {
                cmd.Parameters.AddWithValue("@LE_LanguageID", row("_LE_LanguageID"));
                cmd.Parameters.AddWithValue("@LE_TypeID", enumType);
                cmd.Parameters.AddWithValue("@LE_FieldID", row("_LE_FieldID"));
                cmd.Parameters.AddWithValue("@LE_ParentID", ItemID);
                cmd.Parameters.AddWithValue("@LE_Value", FixNullToDB(row("_LE_Value")));
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            // End Using
            return true;
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }

        return false;
    }
    public static bool _UpdateLanguageElements(DataTable ptblElements, LANG_ELEM_TABLE_TYPE enumType, long ItemID, SqlClient.SqlConnection sqlConn, SqlClient.SqlTransaction savePoint)
    {
        // ' No need to update the languageElements, will send nothing as parameter (like Update Basic Info.)
        if (ptblElements == null)
            return true;

        try
        {
            SqlClient.SqlCommand cmd = new SqlClient.SqlCommand("_spKartrisLanguageElements_Update", sqlConn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = savePoint;
            foreach (DataRow row in ptblElements.Rows)
            {
                cmd.Parameters.AddWithValue("@LE_LanguageID", row("_LE_LanguageID"));
                cmd.Parameters.AddWithValue("@LE_TypeID", enumType);
                cmd.Parameters.AddWithValue("@LE_FieldID", row("_LE_FieldID"));
                cmd.Parameters.AddWithValue("@LE_ParentID", ItemID);
                cmd.Parameters.AddWithValue("@LE_Value", FixNullToDB(row("_LE_Value"), "s"));
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            return true;
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
        return false;
    }
}
