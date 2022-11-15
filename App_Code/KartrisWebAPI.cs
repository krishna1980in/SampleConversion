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

using Kartris;
using System.ServiceModel.Activation;

[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
public class KartrisWebAPI : IKartrisWebAPI
{

    /// <summary>
    ///     ''' Execute Kartris method
    ///     ''' </summary>
    ///     ''' <param name="strMethod">Name of the method that you want to execute (fully qualified name, case-sensitive) e.g. CKartrisBLL.WebShopURL </param>
    ///     ''' <param name="strParametersXML">XML parameter array for the specified method</param>
    ///     ''' <returns>XML Serialized Object</returns>
    ///     ''' <remarks></remarks>
    public string Execute(string strMethod, string strParametersXML)
    {
        string strOutput = "";
        bool blnKeyValidated = false;
        bool blnAllowIP = true;

        string strAuthorizationKey = HttpContext.Current.Request.Headers("Authorization");
        blnKeyValidated = ValidateKey(strAuthorizationKey);

        if (!blnKeyValidated)
            strOutput = "Sorry. Cant authenticate request!";

        // Get user's IP address
        string strClientIP = CkartrisEnvironment.GetClientIPAddress();

        // Check matches specified IPs in web.config, if not blank
        var strBackEndIPLock = ConfigurationManager.AppSettings("KartrisWebAPIIPLock").ToString();
        if (strBackEndIPLock != "")
        {
            try
            {
                string[] arrBackendIPs = Split(strBackEndIPLock, ",");
                bool blnFullIP;
                for (int x = 0; x <= arrBackendIPs.Count() - 1; x++)
                {
                    // check if the IP is a range or a full IP, if its a full ip then it must be matched exactly
                    if (Strings.Split(arrBackendIPs[x], ".").Count() == 4)
                        blnFullIP = true;
                    else
                        blnFullIP = false;
                    if (Interaction.IIf(blnFullIP, arrBackendIPs[x] == strClientIP, Strings.Left(strClientIP, arrBackendIPs[x].Length) == arrBackendIPs[x]))
                    {
                        // ok, let 'em in
                        blnAllowIP = true;
                        break;
                    }
                }
                if (!blnAllowIP)
                    strOutput = "Sorry. Cant authenticate request!";
            }
            catch (Exception ex)
            {
                blnAllowIP = false;
                strOutput = "Invalid Web API IP lock settings!";
            }
        }

        // HttpContext.Current.Request.UserHostAddress
        if (blnKeyValidated & blnAllowIP)
        {
            try
            {
                object obj = new object();

                string strClassName = Strings.Left(strMethod, Strings.InStr(strMethod, ".") - 1);
                string strMethodName = Strings.Mid(strMethod, Strings.InStr(strMethod, ".") + 1);

                Type t = System.Type.GetType(strClassName);

                MethodInfo m;

                object[] Params = null;

                if (t != null)
                {
                    // method was found, try to load it up
                    if (strParametersXML == null || string.IsNullOrEmpty(strParametersXML))
                        m = t.GetMethod(strMethodName, Type.EmptyTypes);
                    else
                    {
                        // <?xml version="1.0" encoding="utf-8" ?>
                        // <KartrisWebAPI>
                        // <Parameters>
                        // <Parameter Name="Param1" Type="String">
                        // <Value>Value1</Value>
                        // </Parameter>
                        // <Parameter Name="Param2" Type="Integer">
                        // <Value>Value2</Value>
                        // </Parameter>
                        // ...
                        // </Parameters>
                        // <KartrisWebAPI>

                        // parse the parameter array XML
                        XmlDocument docXML = new XmlDocument();
                        // Load the xml 
                        docXML.LoadXml(strParametersXML);

                        XmlNodeList lstNodes;
                        XmlNode ndeParameter;

                        string strParametersNodePath = "/KartrisWebAPI/Parameters/Parameter";
                        lstNodes = docXML.SelectNodes(strParametersNodePath);

                        Array.Resize(ref Params, lstNodes.Count);

                        int intIndex = 0;
                        foreach (var ndeParameter in lstNodes)
                        {
                            object objParam = new object();

                            // Dim blnisByRefType As Boolean = False

                            // Try
                            // Dim strIsByRefTypeValue As String = ndeParameter.Attributes.GetNamedItem("isByRef").Value
                            // If Not String.IsNullOrEmpty(strIsByRefTypeValue) Then blnisByRefType = CBool(strIsByRefTypeValue)
                            // Catch ex As Exception

                            // End Try

                            string strType = ndeParameter.Attributes.GetNamedItem("Type").Value;
                            string strValue = ndeParameter.FirstChild.InnerText;

                            // remove trailing spaces if parameter is not string
                            if (strType.ToLower() != "string" & !string.IsNullOrEmpty(strValue))
                                strValue = strValue.Trim();

                            switch (strType.ToLower())
                            {
                                case "string":
                                    {
                                        objParam = strValue;
                                        break;
                                    }

                                case "integer":
                                    {
                                        if (string.IsNullOrEmpty(strValue))
                                        {
                                            objParam = (int)objParam;
                                            objParam = null;
                                        }
                                        else
                                            objParam = (int)strValue;
                                        break;
                                    }

                                case "boolean":
                                    {
                                        if (string.IsNullOrEmpty(strValue))
                                        {
                                            objParam = (bool)objParam;
                                            objParam = null;
                                        }
                                        else if (strValue.ToLower() == "true")
                                            objParam = true;
                                        else
                                            objParam = false;
                                        break;
                                    }

                                case "address":
                                    {
                                        KartrisClasses.Address objAddress = Payment.Deserialize(strValue, typeof(KartrisClasses.Address));
                                        objParam = objAddress;
                                        break;
                                    }

                                case "long":
                                    {
                                        objParam = (long)strValue;
                                        break;
                                    }

                                case "short":
                                    {
                                        objParam = (short)strValue;
                                        break;
                                    }

                                case "arraylist":
                                    {
                                        ArrayList arrObject = Payment.Deserialize(strValue, typeof(ArrayList));
                                        objParam = arrObject;
                                        break;
                                    }

                                case "basket":
                                    {
                                        Kartris.Basket objBasket = Payment.Deserialize(strValue, typeof(Kartris.Basket));
                                        objParam = objBasket;
                                        break;
                                    }

                                case "datatable":
                                    {
                                        DataTable tblObject = Payment.Deserialize(strValue, typeof(DataTable));
                                        objParam = tblObject;
                                        break;
                                    }

                                case "datarow":
                                    {
                                        DataRow tblObject = Payment.Deserialize(strValue, typeof(DataRow));
                                        objParam = tblObject;
                                        break;
                                    }

                                case "double":
                                    {
                                        objParam = (double)strValue;
                                        break;
                                    }

                                case "byte":
                                    {
                                        objParam = (byte)strValue;
                                        break;
                                    }

                                case "single":
                                    {
                                        if (string.IsNullOrEmpty(strValue))
                                        {
                                            objParam = (float)objParam;
                                            objParam = null;
                                        }
                                        else
                                            objParam = (float)strValue;
                                        break;
                                    }

                                case "date":
                                    {
                                        if (string.IsNullOrEmpty(strValue))
                                        {
                                            objParam = (DateTime)objParam;
                                            objParam = null;
                                        }
                                        else
                                            objParam = (DateTime)strValue;
                                        break;
                                    }

                                case "orders_list_callmode":
                                    {
                                        OrdersBLL.ORDERS_LIST_CALLMODE callmode;
                                        callmode = strValue;
                                        objParam = callmode;
                                        break;
                                    }

                                case "char":
                                    {
                                        objParam = (char)strValue;
                                        break;
                                    }
                            }

                            // If blnisByRefType Then
                            // objParam = objParam.GetType.MakeByRefType
                            // End If

                            Params[intIndex] = objParam;
                            intIndex = intIndex + 1;
                        }

                        Type[] pTypes = new Type[Params.Length - 1 + 1];

                        for (int i = 0; i <= Params.GetUpperBound(0); i++)
                            pTypes[i] = Params[i].GetType();

                        m = t.GetMethod(strMethodName);
                    }

                    // If m IsNot Nothing Then
                    // strOutput = Payment.Serialize(m.Invoke(obj, Params))
                    // Else
                    // Throw New Exception("Can't find method """ & strMethodName & """ in Class """ & strClassName & """")
                    // End If

                    // Modded by POLYCHROME, improved serialization
                    if (m != null)
                        // Call the method and serialize the results ''
                        // 'Changed by POLYCHROME from payments.Serialize
                        strOutput = KartrisWebAPIHelperBLL.Serialize(m.Invoke(obj, Params));
                    else
                        throw new Exception("Can't find method \"" + strMethodName + "\" in Class \"" + strClassName + "\"");
                }
                else
                    // method wasn't found
                    strOutput = Payment.Serialize("Can't find specified Class \"" + strClassName + "\"");
            }
            catch (Exception ex)
            {
                // an exception occured while try to parse strMethod
                strOutput = Payment.Serialize("Error parsing method string! Exception Details -> " + ex.Message);
            }
        }

        return strOutput;
    }
    /// <summary>
    ///     ''' Validate the key in the request authorization header against the KartrisWebAPISecretKey appsetting
    ///     ''' </summary>
    ///     ''' <param name="strKartrisAPIKey"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    private bool ValidateKey(string strKartrisAPIKey)
    {
        string strConfigAPIKey = ConfigurationManager.AppSettings("KartrisWebAPISecretKey");
        // always invalidate request if API key is not set in the web.config (its safer that way)
        if (string.IsNullOrEmpty(strConfigAPIKey))
            return false;
        if ((strKartrisAPIKey == strConfigAPIKey))
            return true;
        else
            return false;
    }
}

public class UserNamePassValidator : System.IdentityModel.Selectors.UserNamePasswordValidator
{
    /// <summary>
    ///     ''' This method is only used when wsHTTPBinding is enabled
    ///     ''' Uses backend login details to authenticate API user in addition to the WebAPISecretKey validation
    ///     ''' </summary>
    ///     ''' <param name="userName"></param>
    ///     ''' <param name="password"></param>
    ///     ''' <remarks></remarks>
    public override void Validate(string userName, string password)
    {
        if (userName == null || password == null)
            throw new ArgumentNullException();

        if (!LoginsBLL.Validate(userName, password))
            throw new System.ServiceModel.FaultException("Incorrect Username or Password");
    }
}
