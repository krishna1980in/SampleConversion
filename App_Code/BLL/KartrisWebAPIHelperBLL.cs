// ========================================================================
// Kartris - www.kartris.com
// Copyright 2021 CACTUSOFT
// Copyright 2014 POLYCHROME (additions and changes related to Web API)

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
using System.Xml.Serialization;

public class KartrisWebAPIHelperBLL
{
    /// <summary>
    ///     ''' Serialize Function
    ///     ''' </summary>
    ///     ''' <param name="strObject">Name of the object being passed in</param>
    ///     ''' <returns>XML String</returns>
    ///     ''' <remarks></remarks>
    public static string Serialize(object strObject)
    {
        string strType = strObject.GetType().ToString().ToLower();
        Debug.Print(strType);

        if (strType == "system.data.datatable")
            return SerializeTableToString(strObject);
        else
        {
            XmlSerializer oXS = new XmlSerializer(strObject.GetType());
            StringWriter oStrW = new StringWriter();

            // Serialize the object into an XML string
            oXS.Serialize(oStrW, strObject);
            Serialize = oStrW.ToString();
            oStrW.Close();
        }
    }

    /// <summary>
    ///     ''' Serialize Datatable
    ///     ''' </summary>
    ///     ''' <param name="ptable">Datatable to be serialized</param>
    ///     ''' <returns>XML String</returns>
    ///     ''' <remarks></remarks>
    public static string SerializeTableToString(DataTable ptable)
    {
        if (ptable == null)
            return null;
        else
            using (var sw = new StringWriter())
            {
                using (var tw = new XmlTextWriter(sw))
                {
                    // Must set name for serialization to succeed.
                    ptable.TableName = "results";
                    tw.Formatting = Formatting.Indented;
                    tw.WriteStartDocument();
                    tw.WriteStartElement("KartrisdataTable");

                    (Serialization.IXmlSerializable)ptable.WriteXml(tw);

                    tw.WriteEndElement();
                    tw.WriteEndDocument();
                    tw.Flush();
                    tw.Close();
                    sw.Flush();

                    return sw.ToString();
                }
            }
    }

    /// <summary>
    ///     ''' Datable of Language Elements
    ///     ''' </summary>
    ///     ''' <returns>Datatable</returns>
    ///     ''' <remarks></remarks>
    public static DataTable Create_ptblElements()
    {
        DataTable dt = new DataTable();
        dt.TableName = "tblCategoryLanguageElements";

        dt.Columns.Add(new DataColumn() { ColumnName = "_LE_LanguageID", DataType = System.Type.GetType("System.String") });
        dt.Columns.Add(new DataColumn() { ColumnName = "_LE_FieldID", DataType = System.Type.GetType("System.String") });
        dt.Columns.Add(new DataColumn() { ColumnName = "_LE_Value", DataType = System.Type.GetType("System.String") });

        return dt;
    }

    /// <summary>
    ///     ''' Add Row to Datatable of Language Elements
    ///     ''' </summary>
    ///     ''' <param name="p_ptblElements">Datatable name</param>
    ///     ''' <param name="pLE_LanguageID">ID of language element</param>
    ///     ''' <param name="p_LE_FieldID">Type of language element (ID of Type)</param>
    ///     ''' <param name="p_LE_Value">Value of language element</param>
    ///     ''' <remarks></remarks>
    public static void Add_ptblElements_Row(DataTable p_ptblElements, Int32 pLE_LanguageID, Int32 p_LE_FieldID, string p_LE_Value)
    {
        p_ptblElements.Rows.Add(pLE_LanguageID, p_LE_FieldID, p_LE_Value);
    }

    /// <summary>
    ///     ''' Get ID of country by ISO code
    ///     ''' </summary>
    ///     ''' <param name="pISOCode3Letter">Three letter ISO country code</param>
    ///     ''' <remarks></remarks>
    public static Int32 GetCountryID(string pISOCode3Letter)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        try
        {
            using (SqlClient.SqlConnection sqlConn = new SqlClient.SqlConnection(strConnString))
            {
                SqlCommand cmd = sqlConn.CreateCommand;
                cmd.CommandText = "SELECT D_ID, D_ISOCode3Letter FROM tblKartrisDestination WHERE (D_ISOCode3Letter = @Code)";
                cmd.Parameters.AddWithValue("@Code", pISOCode3Letter);

                using (SqlClient.SqlDataAdapter da = new SqlClient.SqlDataAdapter())
                {
                    DataTable dt = new DataTable();

                    da.SelectCommand = cmd;
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow drow in dt.Rows)
                            return drow("D_ID");
                    }
                    else
                        return 0;
                }
            }
        }
        catch (Exception ex)
        {
            return 0;
        }
        return 0;
    }

    /// <summary>
    ///     ''' Check if language element exists, return parent ID if it does
    ///     ''' </summary>
    ///     ''' <param name="pCode">Code</param>
    ///     ''' <param name="pLE_Type">Language element type</param>
    ///     ''' <remarks></remarks>
    public static Int32 ItemExists(string pCode, LANG_ELEM_TABLE_TYPE pLE_Type)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        try
        {
            using (SqlClient.SqlConnection sqlConn = new SqlClient.SqlConnection(strConnString))
            {
                SqlCommand cmd = sqlConn.CreateCommand;
                cmd.CommandText = string.Format("SELECT LE_LanguageID, LE_TypeID, LE_FieldID, LE_ParentID, LE_Value, LE_ID FROM tblKartrisLanguageElements WHERE (LE_TypeID = {0}) AND (LE_FieldID = 1) AND (LE_Value = @Code)", Convert.ToInt32(pLE_Type));
                cmd.Parameters.AddWithValue("@Code", pCode);
                using (SqlClient.SqlDataAdapter da = new SqlClient.SqlDataAdapter())
                {
                    DataTable dt = new DataTable();

                    da.SelectCommand = cmd;
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow drow in dt.Rows)
                            return drow("LE_ParentID");
                    }
                    else
                        return 0;
                }
            }
        }
        catch (Exception ex)
        {
            return 0;
        }
        return 0;
    }
}
