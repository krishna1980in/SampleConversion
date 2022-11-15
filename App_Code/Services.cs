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

using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using Kartris;
using System.Data;

[System.Web.Script.Services.ScriptService()]
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
public class kartrisServices : System.Web.Services.WebService
{
    [WebMethod(EnableSession = true)]
    [Script.Services.ScriptMethod()]
    public string[] GetTopLevelCategories(string prefixText, int count)
    {
        DataTable tblCategories = new DataTable();
        CategoriesBLL objCategoriesBLL = new CategoriesBLL();
        tblCategories = objCategoriesBLL._SearchTopLevelCategoryByName(prefixText, Session("_LANG"));

        string[] items = new string[] { "" };
        int counter = 0;
        foreach (DataRow row in tblCategories.Rows)
        {
            var oldItems = items;
            items = new string[counter + 1];
            if (oldItems != null)
                Array.Copy(oldItems, items, Math.Min(counter + 1, oldItems.Length));
            items[counter] = row["CAT_Name"] + Constants.vbTab + "(" + (string)row["CAT_ID"] + ")";
            counter += 1;
        }

        return items;
    }

    [WebMethod(EnableSession = true)]
    [Script.Services.ScriptMethod()]
    public string[] GetCategories(string prefixText, int count)
    {
        DataTable tblCategories = new DataTable();
        CategoriesBLL objCategoriesBLL = new CategoriesBLL();
        tblCategories = objCategoriesBLL._SearchCategoryByName(prefixText, Session("_LANG"));

        string[] items = new string[] { "" };
        int counter = 0;
        foreach (DataRow row in tblCategories.Rows)
        {
            var oldItems = items;
            items = new string[counter + 1];
            if (oldItems != null)
                Array.Copy(oldItems, items, Math.Min(counter + 1, oldItems.Length));
            items[counter] = row["CAT_Name"] + Constants.vbTab + "(" + (string)row["CAT_ID"] + ")";
            counter += 1;
        }

        return items;
    }

    [WebMethod(EnableSession = true)]
    [Script.Services.ScriptMethod()]
    public string[] GetProducts(string prefixText, int count)
    {
        DataTable tblProducts = new DataTable();
        ProductsBLL objProductsBLL = new ProductsBLL();
        tblProducts = objProductsBLL._SearchProductByName(prefixText, Session("_LANG"));

        string[] items = new string[] { "" };
        int counter = 0;
        try
        {
            foreach (DataRow row in tblProducts.Rows)
            {
                var oldItems = items;
                items = new string[counter + 1];
                if (oldItems != null)
                    Array.Copy(oldItems, items, Math.Min(counter + 1, oldItems.Length));
                items[counter] = row["P_Name"] + Constants.vbTab + "(" + (string)row["P_ID"] + ")";
                counter += 1;
            }
        }
        catch (Exception ex)
        {
        }
        return items;
    }

    [WebMethod()]
    [Script.Services.ScriptMethod()]
    public string[] GetVersions(string prefixText, int count)
    {
        DataTable tblVersions = new DataTable();
        VersionsBLL objVersionsBLL = new VersionsBLL();
        tblVersions = objVersionsBLL._SearchVersionByCode(prefixText);

        string[] items = new string[] { "" };
        int counter = 0;
        foreach (DataRow row in tblVersions.Rows)
        {
            var oldItems = items;
            items = new string[counter + 1];
            if (oldItems != null)
                Array.Copy(oldItems, items, Math.Min(counter + 1, oldItems.Length));
            items[counter] = row["V_CodeNumber"] + Constants.vbTab + "(" + (string)row["V_ID"] + ")";
            counter += 1;
        }

        return items;
    }


    [WebMethod()]
    [Script.Services.ScriptMethod()]
    public string[] GetVersionsExcludeBaseCombinations(string prefixText, int count)
    {
        DataTable tblVersions = new DataTable();
        VersionsBLL objVersionsBLL = new VersionsBLL();
        tblVersions = objVersionsBLL._SearchVersionByCodeExcludeBaseCombinations(prefixText);

        string[] items = new string[] { "" };
        int counter = 0;
        foreach (DataRow row in tblVersions.Rows)
        {
            var oldItems = items;
            items = new string[counter + 1];
            if (oldItems != null)
                Array.Copy(oldItems, items, Math.Min(counter + 1, oldItems.Length));
            items[counter] = row["V_CodeNumber"] + Constants.vbTab + "(" + (string)row["V_ID"] + ")";
            counter += 1;
        }

        return items;
    }
}
