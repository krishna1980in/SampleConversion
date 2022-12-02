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

using System.Web.HttpContext;
using System.Data;
using CkartrisBLL;
using CkartrisDataManipulation;
using System.Web.Configuration;
using System.Xml;
using System.Xml.Serialization;
using KartSettingsManager;
using System.Web.UI.TemplateControl;
using System.Web.UI;
/// <summary>

/// ''' This holds the 3 primary classes that the checkout uses - ADDRESS, COUNTRY AND SHIPPING METHOD CLASS.

/// ''' Also holds the CURRENCY and LANGUAGE classes which is used by the selection dropdowns on the Master Page.

/// ''' and the VALIDATABLE USER CONTROL CLASS used by some user controls on the front end.

/// ''' </summary>

/// '''
public class KartrisClasses
{
    [Serializable()]
    public class Country
    {
        private int m_countryID;
        private int m_shippingzoneID;
        private string m_name;
        private string m_codeISO3166;
        private string m_TaxExtra;
        private double _TaxRate1;
        private double _TaxRate2;
        private double _ComputedTaxRate;
        private bool m_tax;
        private bool _isLive;
        private static string _connectionstring;


        public int CountryId
        {
            get
            {
                return m_countryID;
            }
        }
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }
        public bool D_Tax
        {
            get
            {
                return m_tax;
            }
            set
            {
                m_tax = value;
            }
        }

        public double TaxRate1
        {
            get
            {
                return _TaxRate1;
            }
            set
            {
                _TaxRate1 = value;
            }
        }
        public double TaxRate2
        {
            get
            {
                return _TaxRate2;
            }
            set
            {
                _TaxRate2 = value;
            }
        }
        public double ComputedTaxRate
        {
            get
            {
                return _ComputedTaxRate;
            }
            set
            {
                _ComputedTaxRate = value;
            }
        }
        public string TaxExtra
        {
            get
            {
                return m_TaxExtra;
            }
            set
            {
                m_TaxExtra = value;
            }
        }
        public int ShippingZoneID
        {
            get
            {
                return m_shippingzoneID;
            }
        }
        public string IsoCode
        {
            get
            {
                return m_codeISO3166;
            }
            set
            {
                m_codeISO3166 = value;
            }
        }
        public bool isLive
        {
            get
            {
                return _isLive;
            }
            set
            {
                _isLive = value;
            }
        }
         static Country()
        {
       
            _connectionstring = WebConfigurationManager.ConnectionStrings("KartrisSQLConnection").ConnectionString;

         }
        public Country(SqlDataReader reader)
        {
            m_countryID = System.Convert.ToInt32(reader("D_ID"));
            m_shippingzoneID = System.Convert.ToInt32(reader("D_ShippingZoneID"));
            m_name = System.Convert.ToString(reader("D_Name"));
            m_codeISO3166 = System.Convert.ToString(reader("D_ISOCode"));
            try
            {
                m_TaxExtra = System.Convert.ToString(CkartrisDataManipulation.FixNullFromDB(reader("D_TaxExtra")));
            }
            catch (Exception ex)
            {
                m_TaxExtra = "";
            }


            _TaxRate1 = System.Convert.ToDouble(FixNullFromDB(reader("D_Tax")));
            _TaxRate2 = System.Convert.ToDouble(FixNullFromDB(reader("D_Tax2")));


            try
            {
                _isLive = FixNullFromDB(reader("D_Live"));
            }
            catch (Exception ex)
            {
                _isLive = true;
            }

            if (_TaxRate1 > 0 | _TaxRate2 > 0)
                m_tax = true;
            else
                m_tax = false;

            _ComputedTaxRate = TaxRegime.CalculateTaxRate(1, 1, _TaxRate1, _TaxRate2, "");
        }
        /// <summary>
        ///         ''' 
        ///         ''' </summary>
        ///         ''' <param name="LanguageID">Optional. Gets the default language if not specified</param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public static List<Country> GetAll(Int16 LanguageID = 0)
        {
            List<Country> results = new List<Country>();
            Int16 numLANGID;
            if (LanguageID == 0)
                numLANGID = GetLanguageIDfromSession();
            else
                numLANGID = LanguageID;

            if (HttpContext.Current.Cache("CountryList" + numLANGID) == null)
            {
                SqlConnection con = new SqlConnection(_connectionstring);
                SqlCommand cmd = new SqlCommand("spKartrisDestinations_GetAll", con);
                cmd.Parameters.Add("@LANG_ID", SqlDbType.Int).Value = numLANGID;
                cmd.CommandType = CommandType.StoredProcedure;

                using (con)
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        results.Add(new Country(reader));
                }
                con.Close();
                HttpContext.Current.Cache.Insert("CountryList" + numLANGID, results, null/* TODO Change to default(_) if this is not a reference type */, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60));
            }
            else
                results = HttpContext.Current.Cache("CountryList" + numLANGID);

            return results;
        }

        /// <param name="LanguageID">Optional. Gets the default language if not specified</param>
        public static Country Get(int countryId, Int16 LanguageID = 0)
        {
            Int16 numLANGID;
            if (LanguageID == 0)
                numLANGID = GetLanguageIDfromSession();
            else
                numLANGID = LanguageID;

            SqlConnection con = new SqlConnection(_connectionstring);
            SqlCommand cmd = new SqlCommand("spKartrisDestinations_Get", con);
            cmd.Parameters.Add("@D_ID", SqlDbType.Int).Value = countryId;
            cmd.Parameters.Add("@LANG_ID", SqlDbType.Int).Value = numLANGID;
            cmd.CommandType = CommandType.StoredProcedure;

            Country result;
            using (con)
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result = (new Country(reader));
                    return result;
                }
            }
            con.Close();
            if (countryId == 0)
                return null;
            CkartrisFormatErrors.LogError("Can't retrieve country info in the specified language -- COUNTRY_ID: " + countryId + "  LANGUAGE_ID: " + numLANGID);
            return null;
        }

        /// <param name="LanguageID">Optional. Gets the default language if not specified</param>
        public static Country GetByIsoCode(string countryIsoCode, Int16 LanguageID = 0)
        {
            Int16 numLANGID;
            if (LanguageID == 0)
                numLANGID = GetLanguageIDfromSession();
            else
                numLANGID = LanguageID;
            SqlConnection con = new SqlConnection(_connectionstring);
            SqlCommand cmd = new SqlCommand("spKartrisDestinations_GetbyIsoCode", con);
            cmd.Parameters.Add("@D_IsoCode", SqlDbType.Char).Value = countryIsoCode;
            cmd.Parameters.Add("@LANG_ID", SqlDbType.Int).Value = numLANGID;
            cmd.CommandType = CommandType.StoredProcedure;

            Country result;
            using (con)
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result = (new Country(reader));
                    return result;
                }
            }
            con.Close();
            return null;
        }
        /// <param name="LanguageID">Optional. Gets the default language if not specified</param>
        public static Country GetByName(string countryName, Int16 LanguageID = 0)
        {
            Int16 numLANGID;
            if (LanguageID == 0)
                numLANGID = GetLanguageIDfromSession();
            else
                numLANGID = LanguageID;
            SqlConnection con = new SqlConnection(_connectionstring);
            SqlCommand cmd = new SqlCommand("_spKartrisDestinations_GetbyName", con);
            cmd.Parameters.Add("@D_Name", SqlDbType.Char).Value = countryName;
            cmd.Parameters.Add("@LANG_ID", SqlDbType.Int).Value = numLANGID;
            cmd.CommandType = CommandType.StoredProcedure;

            Country result;
            using (con)
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result = (new Country(reader));
                    return result;
                }
            }
            con.Close();
            return null;
        }
    }

    [Serializable()]
    public class ShippingMethod
    {
        private int _ID;
        private string _name;
        private string _desc;
        private double _value;
        private double _exTax;
        private double _incTax;
        private double _taxrate1;
        private double _taxrate2;
        private double _computedtaxrate;
        private double _taxamount;
        private int _countryID;
        private double _boundary;
        private static string _connectionstring;
        private double numShippingTaxRate = -1;

        public int ID
        {
            get
            {
                return _ID;
            }
        }
        public int DestinationID
        {
            set
            {
                _countryID = value;
            }
        }
        public double Boundary
        {
            set
            {
                _boundary = value;
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
        }
        public string Description
        {
            get
            {
                return _desc;
            }
        }
        public double Rate
        {
            get
            {
                return _value;
            }
        }

        public double ExTax
        {
            get
            {
                return _exTax;
            }
        }
        public double IncTax
        {
            get
            {
                return _incTax;
            }
        }
        public double TaxRate1
        {
            get
            {
                return _taxrate1;
            }
        }
        public double TaxRate2
        {
            get
            {
                return _taxrate2;
            }
        }
        public double ComputedTaxRate
        {
            get
            {
                return _computedtaxrate;
            }
        }
        public double TaxAmount
        {
            get
            {
                return _taxamount;
            }
        }
        public string DropdownValue
        {
            get
            {
                return _ID + "|||||" + _value + "|||||" + _name;
            }
        }
        public string DropdownText
        {
            get
            {
                return _name + "|||||" + _exTax + "|||||" + _incTax;
            }
        }
        public ShippingMethod(DataRow reader, Country ShippingCountry, Interfaces.objShippingOption objShippingOption, int intTempID)
        {
            bool blnUStaxEnabled = ConfigurationManager.AppSettings("TaxRegime").ToLower == "us";
            bool blnSimpletaxEnabled = ConfigurationManager.AppSettings("TaxRegime").ToLower == "simple";
            bool blnNormalShippingTax = false;
            if (numShippingTaxRate == -1)

                // This US/simple 'if' selection was commented out prior to
                // v2.9007, not sure why. Keep an eye on this.
                // If blnUStaxEnabled Or blnSimpletaxEnabled Then
                // numShippingTaxRate = ShippingCountry.ComputedTaxRate
                // Else
                blnNormalShippingTax = true;

            if (blnNormalShippingTax)
            {
                byte T_ID1, T_ID2; // = CInt(GetKartConfig("frontend.checkout.shipping.taxband"))
                T_ID1 = System.Convert.ToByte(CkartrisDataManipulation.FixNullFromDB(reader("SM_Tax")));
                T_ID2 = System.Convert.ToByte(CkartrisDataManipulation.FixNullFromDB(reader("SM_Tax2")));
                numShippingTaxRate = TaxRegime.CalculateTaxRate(TaxBLL.GetTaxRate(T_ID1), TaxBLL.GetTaxRate(T_ID2), Interaction.IIf(ShippingCountry.TaxRate1 > 0, ShippingCountry.TaxRate1, 1), Interaction.IIf(ShippingCountry.TaxRate2 > 0, ShippingCountry.TaxRate2, 1), ShippingCountry.TaxExtra);
            }

            // If the store is in an EU country and the customer is
            // in another EU country, and has entered a VAT number
            // which is successfully validated, we zero rate the order
            // as the customer will declare and pay the VAT in their
            // own country instead.
            try
            {
                if (System.Web.HttpContext.Current.Session["blnEUVATValidated"] != null)
                {
                    if (System.Convert.ToBoolean(System.Web.HttpContext.Current.Session["blnEUVATValidated"]))
                    {
                        ShippingCountry.D_Tax = false;
                        ShippingCountry.TaxRate1 = 0;
                        ShippingCountry.TaxRate2 = 0;
                        ShippingCountry.ComputedTaxRate = 0;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            _ID = intTempID; // SM_ID
            _name = objShippingOption.Code; // SM_Name
            _desc = ""; // SM_Desc
            _value = objShippingOption.Cost; // S_ShippingRate

            if (LCase(objShippingOption.CurrencyISOCode) != LCase(CurrenciesBLL.CurrencyCode(CurrenciesBLL.GetDefaultCurrency)))
                _value = CurrenciesBLL.ConvertCurrency(CurrenciesBLL.GetDefaultCurrency, _value, CurrenciesBLL.CurrencyID(objShippingOption.CurrencyISOCode));

            if (Information.IsNothing(numShippingTaxRate))
            {
                Exception ex = new Exception("badshiptaxband");
                throw ex;
            }
            else
            {
                // Calculate shipping costs
                bool blnPricesIncTax;

                // In the US, and other general tax areas we show prices excluding tax, 
                // else we use the config setting to decide.
                if (blnUStaxEnabled | blnSimpletaxEnabled)
                    blnPricesIncTax = false;
                else
                    blnPricesIncTax = GetKartConfig("general.tax.pricesinctax") == "y";

                // Tax rate for shipping
                if (!ShippingCountry.D_Tax)
                {
                    numShippingTaxRate = 0;
                    _taxamount = 0;
                }

                // Set the extax and inctax shipping values
                _exTax = _value;
                _incTax = Math.Round(_exTax * (1 + numShippingTaxRate), 2);
                if (numShippingTaxRate > 0)
                    _taxamount = Math.Round(_exTax * numShippingTaxRate, 2);

                _computedtaxrate = numShippingTaxRate;
                if (ShippingCountry.ComputedTaxRate == 0)
                {
                    _taxrate1 = 0; _taxrate2 = 0;
                }
            }
        }

        public ShippingMethod(string Name, string Description, double ExTax, double IncTax)
        {
            _name = Name;
            _desc = Description;
            _exTax = ExTax;
            _incTax = IncTax;
        }

        public ShippingMethod(DataRow reader, Country ShippingCountry)
        {
            bool blnUStaxEnabled = ConfigurationManager.AppSettings("TaxRegime").ToLower == "us";
            bool blnSimpletaxEnabled = ConfigurationManager.AppSettings("TaxRegime").ToLower == "simple";
            bool blnNormalShippingTax = false;
            if (numShippingTaxRate == -1)

                // This US/simple 'if' selection was commented out prior to
                // v2.9007, not sure why. Keep an eye on this.
                // If blnUStaxEnabled Or blnSimpletaxEnabled Then
                // numShippingTaxRate = ShippingCountry.ComputedTaxRate
                // Else
                blnNormalShippingTax = true;

            if (blnNormalShippingTax)
            {
                byte T_ID1, T_ID2; // = CInt(GetKartConfig("frontend.checkout.shipping.taxband"))
                T_ID1 = System.Convert.ToByte(CkartrisDataManipulation.FixNullFromDB(reader("SM_Tax")));
                T_ID2 = System.Convert.ToByte(CkartrisDataManipulation.FixNullFromDB(reader("SM_Tax2")));
                numShippingTaxRate = TaxRegime.CalculateTaxRate(TaxBLL.GetTaxRate(T_ID1), TaxBLL.GetTaxRate(T_ID2), Interaction.IIf(ShippingCountry.TaxRate1 > 0, ShippingCountry.TaxRate1, 1), Interaction.IIf(ShippingCountry.TaxRate2 > 0, ShippingCountry.TaxRate2, 1), ShippingCountry.TaxExtra);
            }

            try
            {
                if (System.Web.HttpContext.Current.Session["blnEUVATValidated"] != null)
                {
                    if (System.Convert.ToBoolean(System.Web.HttpContext.Current.Session["blnEUVATValidated"]))
                    {
                        ShippingCountry.D_Tax = false;
                        ShippingCountry.TaxRate1 = 0;
                        ShippingCountry.TaxRate2 = 0;
                        ShippingCountry.ComputedTaxRate = 0;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            _ID = System.Convert.ToInt32(reader("SM_ID"));
            _name = System.Convert.ToString(FixNullFromDB(reader("SM_Name")));
            _desc = System.Convert.ToString(FixNullFromDB(reader("SM_Desc")));
            _value = System.Convert.ToDouble(reader("S_ShippingRate"));

            if (Information.IsNothing(numShippingTaxRate))
            {
                Exception ex = new Exception("badshiptaxband");
                throw ex;
            }
            else
            {
                // If objBasket.D_Tax <> "" Then shippingPrice.TaxRate = (objRecordSetMisc("T_Taxrate") / 100) * objBasket.D_Tax Else shippingPrice.TaxRate = (objRecordSetMisc("T_Taxrate") / 100)
                // Calculate shipping costs
                bool blnPricesIncTax; // = KartSettingsManager.GetKartConfig("general.tax.pricesinctax") = "y"

                if (blnUStaxEnabled | blnSimpletaxEnabled)
                    blnPricesIncTax = false;
                else
                    blnPricesIncTax = GetKartConfig("general.tax.pricesinctax") == "y";

                // Set shipping inc and ex tax values

                if (blnPricesIncTax)
                {
                    // Set the extax and inctax shipping values
                    _exTax = Math.Round(_value * (1 / (1 + numShippingTaxRate)), 2);

                    // If tax is off, then inc tax can be set to just the ex tax
                    if (ShippingCountry.D_Tax)
                    {
                        _incTax = _value;
                        _taxamount = _value - _exTax;
                    }
                    else
                    {
                        _incTax = _exTax;
                        _taxamount = 0;
                    }
                }
                else
                {
                    // Tax rate for shipping
                    if (!ShippingCountry.D_Tax)
                    {
                        numShippingTaxRate = 0;
                        _taxamount = 0;
                    }

                    // Set the extax and inctax shipping values
                    _exTax = _value;
                    _incTax = Math.Round(_exTax * (1 + numShippingTaxRate), 2);
                    if (numShippingTaxRate > 0)
                        _taxamount = Math.Round(ExTax * numShippingTaxRate, 2);
                }
                _computedtaxrate = numShippingTaxRate;
                if (ShippingCountry.ComputedTaxRate == 0)
                {
                    _taxrate1 = 0; _taxrate2 = 0;
                }
            }
        }


        public static List<ShippingMethod> GetAll(Interfaces.objShippingDetails objShippingDetails, int DestinationID, double Boundary, Int16 LanguageID = 0)
        {
            List<ShippingMethod> results = new List<ShippingMethod>();
            int intLastID;
            Int16 numLANGID;

            if (LanguageID == 0)
                numLANGID = GetLanguageIDfromSession();
            else
                numLANGID = LanguageID;

            Country ShippingCountry = Country.Get(DestinationID, numLANGID);

            DataTable dtShippingMethods = ShippingBLL.GetShippingMethodsRatesByLanguage(DestinationID, Boundary, numLANGID);

            bool blnShippingGatewayUsed = false;

            try
            {
                int intTempID = 1000;
                foreach (var drShippingMethods in dtShippingMethods.Rows)
                {
                    blnShippingGatewayUsed = false;
                    if (intLastID != System.Convert.ToInt32(drShippingMethods("SM_ID")))
                    {
                        if (!string.IsNullOrEmpty(System.Convert.ToString(FixNullFromDB(drShippingMethods("S_ShippingGateways")))))
                        {
                            string strGatewayName = System.Convert.ToString(FixNullFromDB(drShippingMethods("S_ShippingGateways")));
                            Interfaces.ShippingGateway clsPlugin = null/* TODO Change to default(_) if this is not a reference type */;
                            blnShippingGatewayUsed = true; // Thanks to Jcosmo for fix, see http://forum.kartris.com/Topic6296.aspx

                            clsPlugin = Payment.SPLoader(strGatewayName);
                            // Dim xmlShippingDetails As New Kartris.Interfaces.objShippingDetails
                            if (LCase(clsPlugin.Status) != "off")
                            {
                                List<Interfaces.objShippingOption> lstShippingOptions = null;

                                if (LCase(clsPlugin.Currency) != LCase(CurrenciesBLL.CurrencyCode(HttpContext.Current.Session("CUR_ID"))))
                                    objShippingDetails.ShippableItemsTotalPrice = CurrenciesBLL.ConvertCurrency(CurrenciesBLL.CurrencyID(clsPlugin.Currency), objShippingDetails.ShippableItemsTotalPrice, HttpContext.Current.Session("CUR_ID"));

                                lstShippingOptions = clsPlugin.GetRates(Payment.Serialize(objShippingDetails));
                                if (clsPlugin.CallSuccessful & lstShippingOptions != null)
                                {
                                    foreach (Interfaces.objShippingOption objShippingOption in lstShippingOptions)
                                    {
                                        objShippingOption.Code = clsPlugin.GatewayName + " - " + objShippingOption.Code;
                                        results.Add(new ShippingMethod(drShippingMethods, ShippingCountry, objShippingOption, intTempID));
                                        intTempID += 1;
                                    }
                                }
                                else
                                    CkartrisFormatErrors.LogError(clsPlugin.GatewayName + " - " + clsPlugin.CallMessage);
                            }
                            clsPlugin = null/* TODO Change to default(_) if this is not a reference type */;
                        }

                        if (!blnShippingGatewayUsed)
                            results.Add(new ShippingMethod(drShippingMethods, ShippingCountry));
                        intLastID = System.Convert.ToInt32(drShippingMethods("SM_ID"));
                    }
                }
            }
            catch (Exception e)
            {
                results = null;
                CkartrisFormatErrors.LogError(e.Message);
            }
            return results;
        }
        public static ShippingMethod GetByName(string ShippingName, int DestinationID, double Boundary, Int16 LanguageID)
        {
            ShippingMethod result;
            Int16 numLANGID;

            if (LanguageID == 0)
                numLANGID = GetLanguageIDfromSession();
            else
                numLANGID = LanguageID;

            Country ShippingCountry = Country.Get(DestinationID, numLANGID);
            DataTable dtShippingMethods = ShippingBLL.GetShippingMethodsRatesByLanguage(DestinationID, System.Convert.ToDecimal(Boundary), numLANGID);
            try
            {
                DataRow ShippingRow = null/* TODO Change to default(_) if this is not a reference type */;
                foreach (DataRow row in dtShippingMethods.Rows)
                {
                    if (row("SM_Name") == ShippingName)
                    {
                        ShippingRow = row;
                        break;
                    }
                }
                result = new ShippingMethod(ShippingRow, ShippingCountry);
            }
            catch (Exception e)
            {
                result = null;
                CkartrisFormatErrors.LogError(e.Message);
            }
            return result;
        }
        public static ShippingMethod GetByID(Interfaces.objShippingDetails objShippingDetails, int ShippingID, int DestinationID, double Boundary, Int16 LanguageID)
        {
            ShippingMethod result;
            Int16 numLANGID;

            if (LanguageID == 0)
                numLANGID = GetLanguageIDfromSession();
            else
                numLANGID = LanguageID;

            Country ShippingCountry = Country.Get(DestinationID, numLANGID);
            DataTable dtShippingMethods = ShippingBLL.GetShippingMethodsRatesByLanguage(DestinationID, System.Convert.ToDecimal(Boundary), numLANGID);
            try
            {
                int intLastID;
                int intTempID = 1000;

                DataRow drShippingRow = null/* TODO Change to default(_) if this is not a reference type */;
                foreach (DataRow row in dtShippingMethods.Rows)
                {
                    if (ShippingID >= 1000)
                    {
                        if (intLastID != System.Convert.ToInt32(row("SM_ID")))
                        {
                            if (!string.IsNullOrEmpty(System.Convert.ToString(FixNullFromDB(row("S_ShippingGateways")))))
                            {
                                string strGatewayName = System.Convert.ToString(FixNullFromDB(row("S_ShippingGateways")));
                                Interfaces.ShippingGateway clsPlugin = null/* TODO Change to default(_) if this is not a reference type */;

                                clsPlugin = Payment.SPLoader(strGatewayName);
                                // Dim xmlShippingDetails As New Kartris.Interfaces.objShippingDetails

                                List<Interfaces.objShippingOption> lstShippingOptions = clsPlugin.GetRates(Payment.Serialize(objShippingDetails));
                                foreach (Interfaces.objShippingOption objShippingOption in lstShippingOptions)
                                {
                                    objShippingOption.Code = clsPlugin.GatewayName + " - " + objShippingOption.Code;
                                    if (intTempID == ShippingID)
                                        return new ShippingMethod(row, ShippingCountry, objShippingOption, intTempID);
                                    intTempID += 1;
                                }
                                clsPlugin = null/* TODO Change to default(_) if this is not a reference type */;
                            }
                        }
                    }
                    else if (row("SM_ID") == ShippingID)
                    {
                        drShippingRow = row;
                        break;
                    }
                }
                result = new ShippingMethod(drShippingRow, ShippingCountry);
            }
            catch (Exception e)
            {
                result = null;
                CkartrisFormatErrors.LogError(e.Message);
            }
            return result;
        }
    }

    /// <summary>
    ///     ''' Currency Class
    ///     ''' </summary>
    public class Currency
    {
        private int _ID;
        private string _Name;
        private string _Symbol;
        private string _ISOCode;

        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = Value;
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = Value;
            }
        }
        public string Symbol
        {
            get
            {
                return _Symbol;
            }
            set
            {
                _Symbol = Value;
            }
        }
        public string ISOCode
        {
            get
            {
                return _ISOCode;
            }
            set
            {
                _ISOCode = Value;
            }
        }
        public string MenuDisplay
        {
            get
            {
                return _Symbol + " (" + _ISOCode + ")";
            }
        }
        /// <summary>
        ///         ''' Load the active currencies
        ///         ''' </summary>
        public static List<Currency> LoadCurrencies()
        {
            // Initialize command

            DataTable tblCurrencies = GetCurrenciesFromCache(); // CurrenciesBLL.GetCurrencies
            List<Currency> results = new List<Currency>();

            foreach (DataRow row in tblCurrencies.Rows)
                results.Add(new Currency(row));
            return results;
        }

        public Currency(DataRow reader)
        {
            _ID = System.Convert.ToInt32(reader("CUR_ID"));
            // _Name = CType(reader("CUR_Name1"), String)
            _Symbol = System.Convert.ToString(reader("CUR_Symbol"));
            _ISOCode = System.Convert.ToString(reader("CUR_ISOCode"));
        }
    }

    public class ValidatableUserControl : UserControl
    {
        public virtual string ValidationGroup
        {
            set
            {
                foreach (Control control in Control.Controls)
                {
                    if (control is System.Web.UI.WebControls.BaseValidator)
                        (System.Web.UI.WebControls.BaseValidator)control.ValidationGroup = value;
                }
            }
        }

        public virtual string ErrorMessagePrefix
        {
            set
            {
                foreach (Control control in Control.Controls)
                {
                    if (control is System.Web.UI.WebControls.BaseValidator)
                        (System.Web.UI.WebControls.BaseValidator)control.ErrorMessage = value + (System.Web.UI.WebControls.BaseValidator)control.ErrorMessage;
                }
            }
        }
    }

    public abstract class CustomProductControl : System.Web.UI.UserControl
    {

        /// <summary>
        ///         ''' Returns the comma separated list of values from the selected options in the control
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public abstract string ParameterValues { get; }

        /// <summary>
        ///         ''' Returns the item description based on the selected options in the control
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public abstract string ItemDescription { get; }

        /// <summary>
        ///         ''' Returns the computed price from the selected options in the control
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public abstract double ItemPrice { get; }

        /// <summary>
        ///         ''' Instructs the user control to compute and populate the properties with the correct values based on the selected options in the user control
        ///         ''' This function must be called before retrieving the 3 properties - ParameterValues, ItemDescription and ItemPrice
        ///         ''' </summary>
        ///         ''' <returns></returns>
        ///         ''' <remarks>should return either "success" or an error message</remarks>
        public abstract string ComputeFromSelectedOptions();

        /// <summary>
        ///         ''' Calculates the price that will result from processing the given parameter values
        ///         ''' Primarily used to check if stored price in the db/basket is correct
        ///         ''' </summary>
        ///         ''' <param name="ParameterValues">Comma separated list of parameters to be computed</param>
        ///         ''' <returns></returns>
        ///         ''' <remarks>returns -1 if parameters are invalid</remarks>
        public abstract double CalculatePrice(string ParameterValues);
    }

    [Serializable()]
    public class Address
    {
        public enum ADDRESS_TYPE
        {
            BILLING = 1,
            SHIPPING = 2,
            BOTH = 3
        }
        private int _ID;
        private string _AddressLabel;
        private string _FullName;
        private string _FirstName;
        private string _LastName;
        private string _Company;
        private string _StreetAddress;
        private string _TownCity;
        private string _County;
        private string _Postcode;
        private int _CountryID;
        private string _CountryName;
        private string _CountryISO;
        private Country _Country;
        private string _Phone;
        private string _Type;

        public Address(string fullName, string company, string streetAddress, string TownCity, string County, string Postcode, int countryId, string phone, int id = 0, string addressLabel = "default", string type = "u")
        {
            // Me.FirstName = firstName
            // Me.LastName = lastName
            this.FullName = fullName;
            this.Company = company;
            this.StreetAddress = streetAddress;
            this.TownCity = TownCity;

            this.County = County;
            this.Postcode = Postcode;
            this.CountryID = countryId;
            this.Country = Country.Get(countryId);
            this.Phone = phone;
            this.Label = addressLabel;
            this.ID = id;
            this.Type = type;
        }
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }
        public string Label
        {
            get
            {
                if (Strings.Trim(_AddressLabel) == "")
                    return _FullName + "," + Postcode + " " + Country.Name;
                else
                    return _AddressLabel;
            }
            set
            {
                _AddressLabel = value;
            }
        }
        public string FirstName
        {
            get
            {
                return _FirstName;
            }
            set
            {
                _FirstName = value;
            }
        }

        public string LastName
        {
            get
            {
                return _LastName;
            }
            set
            {
                _LastName = value;
            }
        }
        public int CountryID
        {
            get
            {
                return _CountryID;
            }
            set
            {
                _CountryID = value;
            }
        }
        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                _FullName = value;
            }
        }
        public string Company
        {
            get
            {
                return _Company;
            }
            set
            {
                _Company = value;
            }
        }
        public string StreetAddress
        {
            get
            {
                return _StreetAddress;
            }
            set
            {
                _StreetAddress = value;
            }
        }
        public string TownCity
        {
            get
            {
                return _TownCity;
            }
            set
            {
                _TownCity = value;
            }
        }
        public string County
        {
            get
            {
                County = _County;
            }
            set
            {
                _County = value;
            }
        }
        public string Postcode
        {
            get
            {
                return _Postcode;
            }
            set
            {
                _Postcode = value;
            }
        }
        public Country Country
        {
            get
            {
                return _Country;
            }
            set
            {
                _Country = value;
                if (_Country != null)
                    _CountryID = Country.CountryId;
            }
        }
        public string Phone
        {
            get
            {
                return _Phone;
            }
            set
            {
                _Phone = value;
            }
        }
        public static int Delete(int ADR_ID, int ADR_UserID)
        {
            int introwsAffected;
            try
            {
                ;/* Cannot convert LocalDeclarationStatementSyntax, System.ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
Parameter name: index
   at System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.<>c__DisplayClass83_0.<ConvertArguments>b__0(ArgumentSyntax a, Int32 i)
   at System.Linq.Enumerable.<SelectIterator>d__5`2.MoveNext()
   at System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
   at Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SeparatedList[TNode](IEnumerable`1 nodes)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertInitializer(VariableDeclaratorSyntax declarator)
   at ICSharpCode.CodeConverter.CSharp.CommonConversions.SplitVariableDeclarations(VariableDeclaratorSyntax declarator)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 
                Dim _connectionString As String = WebConfigurationManager.ConnectionStrings("KartrisSQLConnection").ConnectionString

 */
                SqlConnection con = new SqlConnection(_connectionString);
                SqlCommand cmd = new SqlCommand("spKartrisUsers_DeleteAddress", con);
                cmd.Parameters.Add("@ADR_UserID", SqlDbType.Int).Value = ADR_UserID;
                cmd.Parameters.Add("@ADR_ID", SqlDbType.Int).Value = ADR_ID;
                cmd.CommandType = CommandType.StoredProcedure;
                using (con)
                {
                    con.Open();
                    introwsAffected = cmd.ExecuteNonQuery;
                }
                con.Close();
            }
            catch (Exception ex)
            {
            }

            return introwsAffected;
        }
        public static int AddUpdate(Address NewAddress, int USR_ID, bool blnMakeDefault = false, int ADR_ID = 0)
        {
            int intID = 0;
            try
            {
                ;/* Cannot convert LocalDeclarationStatementSyntax, System.ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
Parameter name: index
   at System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.<>c__DisplayClass83_0.<ConvertArguments>b__0(ArgumentSyntax a, Int32 i)
   at System.Linq.Enumerable.<SelectIterator>d__5`2.MoveNext()
   at System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
   at Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SeparatedList[TNode](IEnumerable`1 nodes)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertInitializer(VariableDeclaratorSyntax declarator)
   at ICSharpCode.CodeConverter.CSharp.CommonConversions.SplitVariableDeclarations(VariableDeclaratorSyntax declarator)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 
                Dim _connectionString As String = WebConfigurationManager.ConnectionStrings("KartrisSQLConnection").ConnectionString

 */
                SqlConnection con = new SqlConnection(_connectionString);
                SqlCommand cmd = new SqlCommand("spKartrisUsers_AddUpdateAddress", con);
                cmd.Parameters.Add("@ADR_UserID", SqlDbType.Int).Value = USR_ID;
                cmd.Parameters.Add("@ADR_Label", SqlDbType.NVarChar).Value = NewAddress.Label;
                cmd.Parameters.Add("@ADR_Name", SqlDbType.NVarChar).Value = NewAddress.FullName;
                cmd.Parameters.Add("@ADR_Company", SqlDbType.NVarChar).Value = NewAddress.Company;
                cmd.Parameters.Add("@ADR_StreetAddress", SqlDbType.NVarChar).Value = NewAddress.StreetAddress;
                cmd.Parameters.Add("@ADR_TownCity", SqlDbType.NVarChar).Value = NewAddress.TownCity;
                cmd.Parameters.Add("@ADR_County", SqlDbType.NVarChar).Value = NewAddress.County;
                cmd.Parameters.Add("@ADR_PostCode", SqlDbType.NVarChar).Value = NewAddress.Postcode;
                cmd.Parameters.Add("@ADR_Country", SqlDbType.Int).Value = NewAddress.CountryID;
                cmd.Parameters.Add("@ADR_Telephone", SqlDbType.NVarChar).Value = NewAddress.Phone;
                cmd.Parameters.Add("@ADR_ID", SqlDbType.Int).Value = ADR_ID;
                cmd.Parameters.Add("@ADR_Type", SqlDbType.NVarChar).Value = NewAddress.Type;

                if (blnMakeDefault)
                    cmd.Parameters.Add("@ADR_MakeDefault", SqlDbType.Bit).Value = 1;
                else
                    cmd.Parameters.Add("@ADR_MakeDefault", SqlDbType.Bit).Value = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                using (con)
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader;
                    while (reader.Read)
                        intID = reader(0);
                }
                con.Close();
            }
            catch (Exception ex)
            {
            }

            return intID;
        }
        public static List<Address> GetAll(int U_ID)
        {
            ;/* Cannot convert LocalDeclarationStatementSyntax, System.ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
Parameter name: index
   at System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.<>c__DisplayClass83_0.<ConvertArguments>b__0(ArgumentSyntax a, Int32 i)
   at System.Linq.Enumerable.<SelectIterator>d__5`2.MoveNext()
   at System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
   at Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SeparatedList[TNode](IEnumerable`1 nodes)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertInitializer(VariableDeclaratorSyntax declarator)
   at ICSharpCode.CodeConverter.CSharp.CommonConversions.SplitVariableDeclarations(VariableDeclaratorSyntax declarator)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 
            Dim _connectionString As String = WebConfigurationManager.ConnectionStrings("KartrisSQLConnection").ConnectionString

 */
            SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("spKartrisUsers_GetAddressByID", con);
            cmd.Parameters.Add("@U_ID", SqlDbType.Int).Value = U_ID;
            cmd.CommandType = CommandType.StoredProcedure;

            List<Address> results = new List<Address>();
            using (con)
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    results.Add(new Address(System.Convert.ToString(FixNullFromDB(reader("ADR_Name"))), System.Convert.ToString(FixNullFromDB(reader("ADR_Company"))), System.Convert.ToString(FixNullFromDB(reader("ADR_StreetAddress"))), System.Convert.ToString(FixNullFromDB(reader("ADR_TownCity"))), System.Convert.ToString(FixNullFromDB(reader("ADR_County"))), System.Convert.ToString(reader("ADR_PostCode")), System.Convert.ToInt32(reader("ADR_Country")), System.Convert.ToString(FixNullFromDB(reader("ADR_Telephone"))), System.Convert.ToInt32(reader("ADR_ID")), System.Convert.ToString(FixNullFromDB(reader("ADR_Label"))), System.Convert.ToString(FixNullFromDB(reader("ADR_Type")))));
            }
            con.Close();
            return results;
        }
        public static Address Get(int ADR_ID)
        {
            ;/* Cannot convert LocalDeclarationStatementSyntax, System.ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
Parameter name: index
   at System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.<>c__DisplayClass83_0.<ConvertArguments>b__0(ArgumentSyntax a, Int32 i)
   at System.Linq.Enumerable.<SelectIterator>d__5`2.MoveNext()
   at System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
   at Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SeparatedList[TNode](IEnumerable`1 nodes)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertInitializer(VariableDeclaratorSyntax declarator)
   at ICSharpCode.CodeConverter.CSharp.CommonConversions.SplitVariableDeclarations(VariableDeclaratorSyntax declarator)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 
            Dim _connectionString As String = WebConfigurationManager.ConnectionStrings("KartrisSQLConnection").ConnectionString

 */
            SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("spKartrisAddresses_Get", con);
            cmd.Parameters.Add("@ADR_ID", SqlDbType.Int).Value = ADR_ID;
            cmd.CommandType = CommandType.StoredProcedure;
            Address results = null;
            using (con)
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    results = new Address(System.Convert.ToString(FixNullFromDB(reader("ADR_Name"))), System.Convert.ToString(FixNullFromDB(reader("ADR_Company"))), System.Convert.ToString(FixNullFromDB(reader("ADR_StreetAddress"))), System.Convert.ToString(FixNullFromDB(reader("ADR_TownCity"))), System.Convert.ToString(FixNullFromDB(reader("ADR_County"))), System.Convert.ToString(FixNullFromDB(reader("ADR_PostCode"))), System.Convert.ToInt32(reader("ADR_Country")), System.Convert.ToString(FixNullFromDB(reader("ADR_Telephone"))), System.Convert.ToInt32(reader("ADR_ID")), System.Convert.ToString(FixNullFromDB(reader("ADR_Label"))), System.Convert.ToString(FixNullFromDB(reader("ADR_Type"))));
            }
            con.Close();
            return results;
        }
        public static bool CompareAddress(Address Address1, Address Address2)
        {
            {
                var withBlock = Address1;
                if (!withBlock.FullName == Address2.FullName)
                    return false;
                if (!withBlock.StreetAddress == Address2.StreetAddress)
                    return false;
                if (!withBlock.Company == Address2.Company)
                    return false;
                if (!withBlock.TownCity == Address2.TownCity)
                    return false;
                if (!withBlock.County == Address2.County)
                    return false;
                if (!withBlock.CountryID == Address2.CountryID)
                    return false;
                if (!withBlock.Postcode == Address2.Postcode)
                    return false;
                if (!withBlock.Phone == Address2.Phone)
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    ///     ''' Language Class
    ///     ''' </summary>
    public class Language
    {
        private int _ID;
        private string _Name;
        private string _Culture;
        private string _BackEndName;


        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = Value;
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = Value;
            }
        }
        public string BackendName
        {
            get
            {
                return _BackEndName;
            }
            set
            {
                _BackEndName = Value;
            }
        }

        public string Culture
        {
            get
            {
                return _Culture;
            }
            set
            {
                _Culture = Value;
            }
        }
        /// <summary>
        ///         ''' Load the active Languages
        ///         ''' </summary>
        public static List<Language> LoadLanguages()
        {
            // Initialize command
            DataTable tblLanguages = LanguagesBLL.GetLanguages;

            List<Language> results = new List<Language>();

            foreach (DataRow row in tblLanguages.Rows)
                results.Add(new Language(row));
            return results;
        }

        public Language(DataRow reader)
        {
            try
            {
                _ID = System.Convert.ToInt32(reader("LANG_ID"));
                _Name = System.Convert.ToString(reader("LANG_FrontName"));
                _Culture = System.Convert.ToString(reader("LANG_Culture"));
                _BackEndName = System.Convert.ToString(reader("LANG_BackName"));
            }
            catch (Exception ex)
            {
            }
        }
    }
}
