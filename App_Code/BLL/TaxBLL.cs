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

using kartrisTaxDataTableAdapters;
using CkartrisFormatErrors;
using KartSettingsManager;

public class TaxBLL
{
    private static TaxRatesTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;
    protected static TaxRatesTblAdptr Adptr
    {
        get
        {
            _Adptr = new TaxRatesTblAdptr();
            return _Adptr;
        }
    }

    public static double GetTaxRate(byte numTaxID)
    {
        try
        {
            return System.Convert.ToDouble((GetTaxRateFromCache.Select("T_ID=" + numTaxID))(0)("T_Taxrate"));
        }
        catch (Exception ex)
        {
        }
        return 0;
    }
    public static double GetClosestRate(double numComputedRate)
    {
        try
        {
            return System.Convert.ToDouble(Adptr.GetClosestRate(numComputedRate));
        }
        catch (Exception ex)
        {
        }
        return default(Double);
    }
    public static string GetQBTaxRefCode(string strVersionCodeNumber)
    {
        try
        {
            return Adptr.GetQBTaxRefCodeOfVerCode(strVersionCodeNumber);
        }
        catch (Exception ex)
        {
        }
        return "";
    }
    public static string GetQBTaxRefCode(int intTaxID)
    {
        try
        {
            return System.Convert.ToString((GetTaxRateFromCache.Select("T_ID=" + intTaxID))(0)("T_QBRefCode"));
        }
        catch (Exception ex)
        {
        }
        return "";
    }
    public static string GetQBTaxRefCode(double dblTaxRate)
    {
        try
        {
            return System.Convert.ToString((GetTaxRateFromCache.Select("T_TaxRate=" + dblTaxRate))(0)("T_QBRefCode"));
        }
        catch (Exception ex)
        {
        }
        return "";
    }
    protected internal static DataTable _GetTaxRatesForCache()
    {
        return Adptr._GetTaxRates();
    }

    public static bool _UpdateTaxRate(byte intTaxID, float snglTaxRate, string strQBRefCode, string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateTax = sqlConn.CreateCommand;
            cmdUpdateTax.CommandText = "_spKartrisTaxRates_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdUpdateTax.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdUpdateTax.Parameters.AddWithValue("@T_ID", intTaxID);
                cmdUpdateTax.Parameters.AddWithValue("@T_Taxrate", snglTaxRate);
                cmdUpdateTax.Parameters.AddWithValue("@T_QBRefCode", strQBRefCode);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdUpdateTax.Transaction = savePoint;

                cmdUpdateTax.ExecuteNonQuery();
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
}

public class TaxRegime
{
    private static string _Name;
    private static string _DTax_Type;
    private static string _DTax2_Type;
    private static string _VTax_Type;
    private static string _VTax2_Type;
    private static List<KartrisNameValuePair> _Formulas;
    private static List<KartrisNameValuePair> _DTaxNames;
    private static List<KartrisNameValuePair> _DTax2Names;
    private static bool _CalculatePerItem;
    private static string _FormulaNameToFind;
    /// <summary>
    ///     ''' Tax Regime Name
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static string Name
    {
        get
        {
            return _Name;
        }
    }
    public static string DTax_Type
    {
        get
        {
            return _DTax_Type;
        }
    }
    public static string DTax_Type2
    {
        get
        {
            return _DTax2_Type;
        }
    }
    public static string VTax_Type
    {
        get
        {
            return _VTax_Type;
        }
    }
    public static string VTax_Type2
    {
        get
        {
            return _VTax2_Type;
        }
    }
    /// <summary>
    ///     ''' Holds the list of Tax Rate Calculation Formulas for different D_TaxExtra values
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static List<KartrisNameValuePair> Formulas
    {
        get
        {
            return _Formulas;
        }
    }
    /// <summary>
    ///     ''' Holds the list of D_Tax Names for different languages
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static List<KartrisNameValuePair> DTaxNames
    {
        get
        {
            return _DTaxNames;
        }
    }
    /// <summary>
    ///     ''' Holds the list of D_Tax2 Names for different languages
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static List<KartrisNameValuePair> DTax2Names
    {
        get
        {
            return _DTax2Names;
        }
    }
    /// <summary>
    ///     ''' Determines if Regime calculate tax per item or against whole order value
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static bool CalculatePerItem
    {
        get
        {
            return _CalculatePerItem;
        }
    }
    public static void LoadTaxConfigXML()
    {
        try
        {
            XmlDocument docXML = new XmlDocument();
            // Load the TaxScheme Config file file from web root
            docXML.Load(HttpContext.Current.Server.MapPath("~/TaxRegime.Config"));

            XmlNodeList lstNodes;
            XmlNode ndeTaxRegime;

            _Name = ConfigurationManager.AppSettings("TaxRegime").ToUpper;
            string strRegimeNodePath = "/configuration/" + _Name + "TaxRegime";
            ndeTaxRegime = docXML.SelectSingleNode(strRegimeNodePath);

            // Get the regime's calculation mode - peritem(tax is calculated per item), perorder (tax is calculated against the whole order value)
            if (ndeTaxRegime.Attributes.GetNamedItem("mode").Value == "PerItem")
                _CalculatePerItem = true;
            else
                _CalculatePerItem = false;

            // get tax fields types *boolean or double(rate)* -> D_Tax, D_Tax2, V_Tax, V_Tax2
            ndeTaxRegime = docXML.SelectSingleNode(strRegimeNodePath + "/TaxFields/D_Tax");
            _DTax_Type = ndeTaxRegime.Attributes.GetNamedItem("type").Value;

            ndeTaxRegime = docXML.SelectSingleNode(strRegimeNodePath + "/TaxFields/D_Tax2");
            if (ndeTaxRegime != null)
                _DTax2_Type = ndeTaxRegime.Attributes.GetNamedItem("type").Value;
            else
                _DTax2_Type = "";

            ndeTaxRegime = docXML.SelectSingleNode(strRegimeNodePath + "/TaxFields/V_Tax");
            if (ndeTaxRegime != null)
                _VTax_Type = ndeTaxRegime.Attributes.GetNamedItem("type").Value;
            else
                _VTax_Type = "";

            ndeTaxRegime = docXML.SelectSingleNode(strRegimeNodePath + "/TaxFields/V_Tax2");
            if (ndeTaxRegime != null)
                _VTax2_Type = ndeTaxRegime.Attributes.GetNamedItem("type").Value;
            else
                _VTax2_Type = "";

            // loop through TaxRateCalculation nodes and add them all to the Formulas property
            lstNodes = docXML.SelectNodes(strRegimeNodePath + "/TaxRateCalculation");
            _Formulas = new List<KartrisNameValuePair>();
            foreach (var ndeTaxRegime in lstNodes)
            {
                KartrisNameValuePair taxEquation = new KartrisNameValuePair();
                // We only have a single formula (taxratecalculation node) if type attribute is empty so just name it as default
                if (ndeTaxRegime.Attributes.GetNamedItem("type") != null)
                    taxEquation.Name = ndeTaxRegime.Attributes.GetNamedItem("type").Value;
                else
                    taxEquation.Name = "default";
                taxEquation.Value = ndeTaxRegime.Attributes.GetNamedItem("value").Value;

                _Formulas.Add(taxEquation);
            }
        }
        catch (Exception ex)
        {
            // We want to write a log entry.
            CkartrisFormatErrors.LogError(ex.Message + Constants.vbCrLf + "This suggests the TaxRegime.config file on the root of the site is either corrupted, or permissions prevent it being read.");
        }
    }

    /// <summary>
    ///     ''' Calculate the applicable tax rate
    ///     ''' </summary>
    ///     ''' <param name="V_Tax">Product Version Tax. The tax that is defined when creating or editting a single product version</param>
    ///     ''' <param name="V_Tax2">Product Version Tax 2. Same use as V_Tax but used in countries with a 2 tier tax system (e.g. Canada)</param>
    ///     ''' <param name="D_Tax">Destination Country Tax. The tax applicable in the destination country</param>
    ///     ''' <param name="D_Tax2">Destination Country Tax 2. Same as D_Tax but used in countries with a 2 tier tax system (e.g. Canada)</param>
    ///     ''' <param name="D_TaxExtra">The Tax Rate Calculation Type Name. View TaxRegime.Default and then for the country of interest look 
    ///     ''' at the TaxRateCalculation node. If there is no node name then there is only one calculation, if there are multiple nodes then 
    ///     ''' the name is used here to descriminate between them. From v3.0001, we also use to mark EU countries</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static double CalculateTaxRate(double V_Tax, double V_Tax2, double D_Tax, double D_Tax2, string D_TaxExtra)
    {
        DataTable tblCalculate = new DataTable();
        string formula = "({0})";
        string strCorrectFormula;

        if (!string.IsNullOrEmpty(D_TaxExtra))
        {
            if (D_TaxExtra != "EU")
            {
                // D_TaxExtra has a value so must match it to the TaxRateCalculation Name to get the correct Formula
                _FormulaNameToFind = D_TaxExtra;
                KartrisNameValuePair result = _Formulas.Find(FindName);
                if (result != null)
                    strCorrectFormula = result.Value;
                else
                    throw new Exception("Invalid D_TaxExtra Value: " + D_TaxExtra);
            }
            else
                // D_TaxExtra is empty so just get the "default" Formula
                strCorrectFormula = _Formulas.Item[0].Value;
        }
        else
            // D_TaxExtra is empty so just get the "default" Formula
            strCorrectFormula = _Formulas.Item[0].Value;


        // Boolean tax fields should only be either 1 or 0 
        if (_DTax_Type.ToLower() == "boolean" && D_Tax > 0)
            D_Tax = 1;
        if (_DTax2_Type.ToLower() == "boolean" && D_Tax2 > 0)
            D_Tax2 = 1;

        // V_Tax fields should always be converted to rate/percentage
        if (_VTax_Type.ToLower() == "boolean" && V_Tax > 0)
            V_Tax = 1;
        else
            V_Tax = V_Tax / 100;
        if (_VTax2_Type.ToLower() == "boolean" && V_Tax2 > 0)
            V_Tax2 = 1;
        else
            V_Tax2 = V_Tax2 / 100;

        // Replace the variables in the TaxRate Formula with the actual values
        strCorrectFormula = Strings.Replace(strCorrectFormula, "D_Tax2", D_Tax2);
        strCorrectFormula = Strings.Replace(strCorrectFormula, "V_Tax2", V_Tax2);
        strCorrectFormula = Strings.Replace(strCorrectFormula, "D_Tax", D_Tax);
        strCorrectFormula = Strings.Replace(strCorrectFormula, "V_Tax", V_Tax);


        string expr = string.Format(formula, strCorrectFormula);
        expr = expr.Replace(",", GetCurrenciesFromCache().Select("CUR_ID=" + HttpContext.Current.Session("CUR_ID"))(0)("CUR_DecimalPoint"));
        // this line does the actual computation
        return System.Convert.ToDouble(tblCalculate.Compute(expr, ""));
    }

    /// <summary>
    ///     ''' generic Name Value Pair class
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public class KartrisNameValuePair
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    /// <summary>
    ///     ''' Function to find a value in the Name property from a list
    ///     ''' </summary>
    ///     ''' <param name="Formula"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    private static bool FindName(KartrisNameValuePair Formula)
    {
        if (Formula.Name == _FormulaNameToFind)
            return true;
        else
            return false;
    }
}
