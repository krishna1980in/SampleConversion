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
using System.Xml;

// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
// <System.Web.Script.Services.ScriptService()> _
[WebService(Namespace = "http://developer.intuit.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
public class KartrisQBService : QBWebConnectorSvc
{

    /// <summary>
    ///     ''' WebMethod - clientVersion()
    ///     ''' To enable web service with QBWC version control
    ///     ''' Signature: public string clientVersion(string strVersion)
    ///     '''
    ///     ''' IN: 
    ///     ''' string strVersion
    ///     '''
    ///     ''' OUT: 
    ///     ''' string errorOrWarning
    ///     ''' Possible values: 
    ///     ''' string retVal
    ///     ''' - NULL or [emptyString] = QBWC will let the web service update
    ///     ''' - "E:[any text]" = popup ERROR dialog with [any text] 
    ///     ''' - abort update and force download of new QBWC.
    ///     ''' - "W:[any text]" = popup WARNING dialog with [any text]
    ///     ''' - choice to user, continue update or not.
    ///     ''' </summary>
    [WebMethod()]
    public string clientVersion(string strVersion)
    {
        string evLogTxt = "WebMethod: clientVersion() has been called " + "by QBWebconnector" + Constants.vbCrLf + Constants.vbCrLf;
        evLogTxt += "Parameters received:" + Constants.vbCrLf;
        evLogTxt += ("string strVersion = ") + strVersion + Constants.vbCrLf;
        evLogTxt += Constants.vbCrLf;

        string retVal = null;
        if (!string.IsNullOrEmpty(strVersion))
        {
            double recommendedVersion = 1.5;
            double supportedMinVersion = 1.0D;
            double suppliedVersion = Convert.ToDouble(this.parseForVersion(strVersion));
            evLogTxt += ("QBWebConnector version = ") + strVersion + Constants.vbCrLf;
            evLogTxt += ("Recommended Version = ") + recommendedVersion.ToString() + Constants.vbCrLf;
            evLogTxt += ("Supported Minimum Version = ") + supportedMinVersion.ToString() + Constants.vbCrLf;
            evLogTxt += ("SuppliedVersion = ") + suppliedVersion.ToString() + Constants.vbCrLf;
            if (suppliedVersion < recommendedVersion)
                retVal = "W:We recommend that you upgrade your QBWebConnector";
            else if (suppliedVersion < supportedMinVersion)
                retVal = "E:You need to upgrade your QBWebConnector";
            evLogTxt += Constants.vbCrLf;
            evLogTxt += "Return values: " + Constants.vbCrLf;
            evLogTxt += ("string retVal = ") + retVal;
        }
        return retVal;
    }
    [WebMethod()]
    public override string[] authenticate(string strUserName, string strPassword)
    {
        string evLogTxt = "WebMethod: authenticate() has been called by QBWebconnector" + Constants.vbCrLf + Constants.vbCrLf;
        evLogTxt += "Parameters received:" + Constants.vbCrLf;
        evLogTxt += ("string strUserName = ") + strUserName + Constants.vbCrLf;
        evLogTxt += ("string strPassword = ") + strPassword + Constants.vbCrLf;
        evLogTxt += Constants.vbCrLf;

        UsersBLL objUsersBLL = new UsersBLL();

        string[] authReturn = new string[2] { };
        // Code below uses a random GUID to use as session ticket
        // a GUID looks like this -> {85B41BEE-5CD9-427a-A61B-83964F1EB426}
        authReturn[0] = System.Guid.NewGuid().ToString();

        string pwd = KartSettingsManager.GetKartConfig("general.quickbooks.pass");
        if (strUserName.Trim().Equals("Kartris") && objUsersBLL.EncryptSHA256Managed(strPassword.Trim(), LoginsBLL._GetSaltByUserName(strUserName), true) == pwd)
            // An empty string for authReturn[1] means asking QBWebConnector 
            // to connect to the company file that is currently openned in QB
            // authReturn(1) = "c:\Program Files\Intuit\QuickBooks\sample_product-based business.qbw"
            authReturn[1] = "";
        else
            authReturn[1] = "nvu";
        // "none" to indicate there is no work to do
        OrdersBLL objOrdersBLL = new OrdersBLL();
        if (objOrdersBLL.GetQBQueue.Rows.Count == 0)
            authReturn[1] = "none";
        // or a company filename in the format C:\full\path\to\company.qbw

        evLogTxt += Constants.vbCrLf;
        evLogTxt += "Return values: " + Constants.vbCrLf;
        evLogTxt += ("string[] authReturn[0] = ") + authReturn[0].ToString() + Constants.vbCrLf;
        evLogTxt += ("string[] authReturn[1] = ") + authReturn[1].ToString();
        // CkartrisFormatErrors.LogError(evLogTxt)

        return (authReturn);
    }
    [WebMethod()]
    public override string closeConnection(string ticket)
    {
        string evLogTxt = "WebMethod: closeConnection() has been called by QBWebconnector" + Constants.vbCrLf + Constants.vbCrLf;
        evLogTxt += "Parameters received:" + Constants.vbCrLf;
        evLogTxt += ("string ticket = ") + ticket + Constants.vbCrLf;
        evLogTxt += Constants.vbCrLf;
        string retVal = null;

        retVal = "OK";

        evLogTxt += Constants.vbCrLf;
        evLogTxt += "Return values: " + Constants.vbCrLf;
        evLogTxt += ("string retVal= ") + retVal + Constants.vbCrLf;

        // CkartrisFormatErrors.LogError(evLogTxt)
        return (retVal);
    }
    [WebMethod(true)]
    public override string connectionError(string ticket, string hresult, string message)
    {
        if (Session("ce_counter") == null)
            Session("ce_counter") = 0;

        string evLogTxt = "WebMethod: connectionError() has been called by QBWebconnector" + Constants.vbCrLf + Constants.vbCrLf;
        evLogTxt += "Parameters received:" + Constants.vbCrLf;
        evLogTxt += ("string ticket = ") + ticket + Constants.vbCrLf;
        evLogTxt += ("string hresult = ") + hresult + Constants.vbCrLf;
        evLogTxt += ("string message = ") + message + Constants.vbCrLf;
        evLogTxt += Constants.vbCrLf;

        string retVal = null;
        // 0x80040400 - QuickBooks found an error when parsing the provided XML text stream. 
        const string QB_ERROR_WHEN_PARSING = "0x80040400";
        // 0x80040401 - Could not access QuickBooks. 
        const string QB_COULDNT_ACCESS_QB = "0x80040401";
        // 0x80040402 - Unexpected error. Check the qbsdklog.txt file for possible, additional information. 
        const string QB_UNEXPECTED_ERROR = "0x80040402";
        // Add more as we need...

        if (hresult.Trim().Equals(QB_ERROR_WHEN_PARSING))
        {
            evLogTxt += ("HRESULT = ") + hresult + Constants.vbCrLf;
            evLogTxt += ("Message = ") + message + Constants.vbCrLf;
            retVal = "DONE";
        }
        else if (hresult.Trim().Equals(QB_COULDNT_ACCESS_QB))
        {
            evLogTxt += ("HRESULT = ") + hresult + Constants.vbCrLf;
            evLogTxt += ("Message = ") + message + Constants.vbCrLf;
            retVal = "DONE";
        }
        else if (hresult.Trim().Equals(QB_UNEXPECTED_ERROR))
        {
            evLogTxt += ("HRESULT = ") + hresult + Constants.vbCrLf;
            evLogTxt += ("Message = ") + message + Constants.vbCrLf;
            retVal = "DONE";
        }
        else
                    // Depending on various hresults return different value 
                    if (System.Convert.ToInt32(Session("ce_counter")) == 0)
        {
            // Try again with this company file
            evLogTxt += ("HRESULT = ") + hresult + Constants.vbCrLf;
            evLogTxt += ("Message = ") + message + Constants.vbCrLf;
            evLogTxt += "Sending empty company file to try again.";
            retVal = "";
        }
        else
        {
            evLogTxt += ("HRESULT = ") + hresult + Constants.vbCrLf;
            evLogTxt += ("Message = ") + message + Constants.vbCrLf;
            evLogTxt += "Sending DONE to stop.";
            retVal = "DONE";
        }
        evLogTxt += Constants.vbCrLf;
        evLogTxt += "Return values: " + Constants.vbCrLf;
        evLogTxt += ("string retVal = ") + retVal + Constants.vbCrLf;
        // CkartrisFormatErrors.LogError(evLogTxt)
        Session("ce_counter") = System.Convert.ToInt32(Session("ce_counter")) + 1;
        return retVal;
    }
    [WebMethod(true)]
    public override string getLastError(string ticket)
    {
        string evLogTxt = "WebMethod: getLastError() has been called by QBWebconnector" + Constants.vbCrLf + Constants.vbCrLf;
        evLogTxt += "Parameters received:" + Constants.vbCrLf;
        evLogTxt += ("string ticket = ") + ticket + Constants.vbCrLf;
        evLogTxt += Constants.vbCrLf;

        // Dim errorCode As Integer = 0
        string retVal = null;

        if (Session("QBLastError") != null)
            retVal = Session("QBLastError");
        else
            retVal = "";
        evLogTxt += Constants.vbCrLf;
        evLogTxt += "Return values: " + Constants.vbCrLf;
        evLogTxt += ("string retVal= ") + retVal + Constants.vbCrLf;
        // CkartrisFormatErrors.LogError(evLogTxt)
        return (retVal);
    }
    [WebMethod(true)]
    public override int receiveResponseXML(string ticket, string response, string hresult, string message)
    {
        int retVal = 0;
        string evLogTxt = "WebMethod: receiveResponseXML() has been called by QBWebconnector" + Constants.vbCrLf + Constants.vbCrLf;
        evLogTxt += "Parameters received:" + Constants.vbCrLf;
        evLogTxt += ("string ticket = ") + ticket + Constants.vbCrLf;
        evLogTxt += ("string response = ") + response + Constants.vbCrLf;
        evLogTxt += ("string hresult = ") + hresult + Constants.vbCrLf;
        evLogTxt += ("string message = ") + message + Constants.vbCrLf;
        evLogTxt += Constants.vbCrLf;


        if (!hresult.ToString().Equals(""))
        {
            // if there is an error with response received, web service could also return a -ve int 
            evLogTxt += ("HRESULT = ") + hresult + Constants.vbCrLf;
            evLogTxt += ("Message = ") + message + Constants.vbCrLf;
            Session("QBLastError") = "QB Status Code:" + hresult + " - " + message;
            retVal = -101;
        }
        else
        {
            evLogTxt += "Length of response received = " + response.Length + Constants.vbCrLf;
            XmlDocument outputXMLDoc = new XmlDocument();
            outputXMLDoc.LoadXml(response);
            string strCurrentQBProcess = Session("QBProcess");
            switch (strCurrentQBProcess)
            {
                case "SearchKartrisItem":
                    {
                        XmlNodeList qbXMLMsgsRsNodeList = outputXMLDoc.GetElementsByTagName("ItemQueryRs");

                        if (qbXMLMsgsRsNodeList.Count == 1)
                        {
                            // it's always true, since we will only add a single 'Kartris Order Item' in Quickbooks
                            XmlAttributeCollection rsAttributes = qbXMLMsgsRsNodeList.Item(0).Attributes;
                            // get the status Code, info and Severity
                            string retStatusCode = rsAttributes.GetNamedItem("statusCode").Value;
                            string retStatusSeverity = rsAttributes.GetNamedItem("statusSeverity").Value;
                            string retStatusMessage = rsAttributes.GetNamedItem("statusMessage").Value;

                            if (retStatusCode == "0")
                            {
                                // get the ItemNonInventoryRet node for detailed info
                                // an ItemQueryRs contains max one childNode for "ItemNonInverntoryRet"
                                XmlNodeList ItemQueryRsNodeList = qbXMLMsgsRsNodeList.Item(0).ChildNodes;
                                if (ItemQueryRsNodeList.Count == 1 && ItemQueryRsNodeList.Item(0).Name.Equals("ItemNonInventoryRet"))
                                {
                                    XmlNodeList ItemNonInventoryRetNodeList = ItemQueryRsNodeList.Item(0).ChildNodes;

                                    foreach (XmlNode ItemNonInventoryRetNode in ItemNonInventoryRetNodeList)
                                    {
                                        if (ItemNonInventoryRetNode.Name.Equals("ListID"))
                                        {
                                            Application("KartrisQBItemListID") = ItemNonInventoryRetNode.InnerText;
                                            retVal = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (retStatusCode == "500")
                                    Session("QBLastError") = "Cannot find 'Kartris Order Item' in Quickbooks.";
                                else
                                    Session("QBLastError") = "QB Status Code: " + retStatusCode + " - QB message: " + retStatusMessage;

                                return -1;
                            }
                        }

                        break;
                    }

                case "AddCustomer":
                    {
                        XmlNodeList qbXMLMsgsRsNodeList = outputXMLDoc.GetElementsByTagName("CustomerAddRs");
                        UsersBLL objUsersBLL = new UsersBLL();

                        if (qbXMLMsgsRsNodeList.Count == 1)
                        {
                            // it's always true, since we added a single Customer
                            XmlAttributeCollection rsAttributes = qbXMLMsgsRsNodeList.Item(0).Attributes;
                            // get the status Code, info and Severity
                            string retStatusCode = rsAttributes.GetNamedItem("statusCode").Value;
                            string retStatusSeverity = rsAttributes.GetNamedItem("statusSeverity").Value;
                            string retStatusMessage = rsAttributes.GetNamedItem("statusMessage").Value;

                            if (retStatusCode == "0")
                            {
                                // get the CustomerRet node for detailed info
                                // a CustomerAddRs contains max one childNode for "CustomerRet"
                                XmlNodeList custAddRsNodeList = qbXMLMsgsRsNodeList.Item(0).ChildNodes;
                                if (custAddRsNodeList.Count == 1 && custAddRsNodeList.Item(0).Name.Equals("CustomerRet"))
                                {
                                    XmlNodeList custRetNodeList = custAddRsNodeList.Item(0).ChildNodes;

                                    foreach (XmlNode custRetNode in custRetNodeList)
                                    {
                                        if (custRetNode.Name.Equals("ListID"))
                                        {
                                            string intCustomerID = System.Convert.ToString(Session("QBCustomerID"));
                                            string strListId = custRetNode.InnerText;
                                            objUsersBLL.UpdateQBListID(intCustomerID, strListId);
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Session("QBLastError") = "QB Status Code: " + retStatusCode + " - " + retStatusMessage + " - Error while trying to add customer.";
                                return -1;
                            }
                        }

                        break;
                    }

                case "AddOrder":
                    {
                        XmlNodeList qbXMLMsgsRsNodeList = outputXMLDoc.GetElementsByTagName("SalesReceiptAddRs");
                        OrdersBLL objOrdersBLL = new OrdersBLL();

                        if (qbXMLMsgsRsNodeList.Count == 1)
                        {
                            XmlAttributeCollection rsAttributes = qbXMLMsgsRsNodeList.Item(0).Attributes;
                            // get the status Code, info and Severity
                            string retStatusCode = rsAttributes.GetNamedItem("statusCode").Value;
                            string retStatusSeverity = rsAttributes.GetNamedItem("statusSeverity").Value;
                            string retStatusMessage = rsAttributes.GetNamedItem("statusMessage").Value;

                            if (retStatusCode == "0")
                            {
                                // get the SalesReceiptRet node for detailed info
                                // a SalesReceiptAddRs contains max one childNode for "SalesReceiptRet"
                                XmlNodeList SalesReceiptAddRsNodeList = qbXMLMsgsRsNodeList.Item(0).ChildNodes;
                                if (SalesReceiptAddRsNodeList.Count == 1 && SalesReceiptAddRsNodeList.Item(0).Name.Equals("SalesReceiptRet"))
                                {
                                    XmlNodeList SalesReceiptRet = SalesReceiptAddRsNodeList.Item(0).ChildNodes;

                                    foreach (XmlNode SalesReceiptRetNode in SalesReceiptRet)
                                    {
                                        if (SalesReceiptRetNode.Name.Equals("RefNumber"))
                                        {
                                            int O_ID = System.Convert.ToInt32(SalesReceiptRetNode.InnerText);
                                            Session("O_ctr") += 1;
                                            objOrdersBLL.UpdateQBSent(O_ID);
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Session("QBLastError") = "QB Status Code: " + retStatusCode + " - " + retStatusMessage + " - Error while trying to add order.";
                                return -1;
                            }
                        }

                        break;
                    }
            }

            if (Session("QBProcess") == "SearchKartrisItem")
                retVal = 1;
            else
            {
                int count = System.Convert.ToInt32(Session("O_ctr"));
                int total = System.Convert.ToInt32(Session("O_TotalCount"));
                int percentage = (count * 100) / (double)total;
                // If percentage >= 100 Then
                // count = 0
                // Session("O_ctr") = 0
                // End If
                retVal = percentage;
            }
        }




        evLogTxt += Constants.vbCrLf;
        evLogTxt += "Return values: " + Constants.vbCrLf;
        evLogTxt += "int retVal= " + retVal.ToString() + Constants.vbCrLf;
        // CkartrisFormatErrors.LogError(evLogTxt)
        return retVal;
    }
    [WebMethod(true)]
    public override string sendRequestXML(string ticket, string strHCPResponse, string strCompanyFileName, string qbXMLCountry, int qbXMLMajorVers, int qbXMLMinorVers)
    {
        if (!string.IsNullOrEmpty(strHCPResponse))
            DoVersionCheck(strHCPResponse);

        // UK or US?
        string strQBCountry = Session("QBCountry");
        // Newer than version 2006?
        bool blnQBNew = Session("QBNew");

        OrdersBLL objOrdersBLL = new OrdersBLL();

        string request = "";
        if (Session("O_ctr") == null)
            Session("O_ctr") = 0;
        if (Application("KartrisQBItemListID") != null)
        {
            DataTable dtQueue = objOrdersBLL.GetQBQueue;
            if (Session("O_ctr") == 0)
                Session("O_TotalCount") = dtQueue.Rows.Count;
            if (dtQueue.Rows.Count > 0)
            {
                int intOrderID = dtQueue.Rows(0)("O_ID");
                string strCustomerListID = CkartrisDataManipulation.FixNullFromDB(dtQueue.Rows(0)("U_QBListID"));
                int intCustomerID = dtQueue.Rows(0)("O_CustomerID");
                string[] arrBillAdd = Split(dtQueue.Rows(0)("O_BillingAddress"), Constants.vbCrLf);
                string[] arrShipAdd = Split(dtQueue.Rows(0)("O_ShippingAddress"), Constants.vbCrLf);
                string strEmailAddress = dtQueue.Rows(0)("U_EmailAddress");


                string strFullName = arrBillAdd[0];
                string strFirstName;
                string strLastName;
                if (Strings.InStr(strFullName, " ") > 0)
                {
                    strFirstName = Strings.Left(strFullName, Strings.InStrRev(strFullName, " ") - 1);
                    string[] aryName = Strings.Split(Strings.Trim(strFullName), " ", -1);
                    strLastName = aryName[Information.UBound(aryName)];
                }
                else
                {
                    strFirstName = strFullName;
                    strLastName = strFullName;
                }

                if (strCustomerListID == null)
                {
                    // Customer is not in QB yet, add Customer in QB first
                    XmlDocument inputXMLDoc = null;
                    XmlElement qbXMLMsgsRq = null;

                    CreateQBReqXML(ref inputXMLDoc, ref qbXMLMsgsRq, (!blnQBNew) & strQBCountry == "UK");
                    XmlElement CustomerAddRq = inputXMLDoc.CreateElement("CustomerAddRq");
                    qbXMLMsgsRq.AppendChild(CustomerAddRq);
                    CustomerAddRq.SetAttribute("requestID", "2");

                    XmlElement custAdd = inputXMLDoc.CreateElement("CustomerAdd");
                    CustomerAddRq.AppendChild(custAdd);

                    // .FullName 
                    // .Company 
                    // .StreetAddress 
                    // .TownCity 
                    // .County
                    // .PostCode
                    // .Country.Name 
                    // .Phone

                    custAdd.AppendChild(inputXMLDoc.CreateElement("Name")).InnerText = strFullName;
                    if (!string.IsNullOrEmpty(arrBillAdd[1]))
                        custAdd.AppendChild(inputXMLDoc.CreateElement("CompanyName")).InnerText = arrBillAdd[1];

                    {
                        var withBlock = custAdd;
                        withBlock.AppendChild(inputXMLDoc.CreateElement("FirstName")).InnerText = strFirstName;
                        withBlock.AppendChild(inputXMLDoc.CreateElement("LastName")).InnerText = strLastName;
                    }

                    XmlElement BillAdd = inputXMLDoc.CreateElement("BillAddress");
                    custAdd.AppendChild(BillAdd);
                    {
                        var withBlock = BillAdd;
                        withBlock.AppendChild(inputXMLDoc.CreateElement("Addr1")).InnerText = arrBillAdd[2];
                        withBlock.AppendChild(inputXMLDoc.CreateElement("City")).InnerText = arrBillAdd[3];
                        if (strQBCountry == "US" | blnQBNew)
                            withBlock.AppendChild(inputXMLDoc.CreateElement("State")).InnerText = arrBillAdd[4];
                        else
                            withBlock.AppendChild(inputXMLDoc.CreateElement("County")).InnerText = arrBillAdd[4];

                        withBlock.AppendChild(inputXMLDoc.CreateElement("PostalCode")).InnerText = arrBillAdd[5];
                        withBlock.AppendChild(inputXMLDoc.CreateElement("Country")).InnerText = arrBillAdd[6];
                    }

                    XmlElement ShipAdd = inputXMLDoc.CreateElement("ShipAddress");
                    custAdd.AppendChild(ShipAdd);
                    {
                        var withBlock = ShipAdd;
                        withBlock.AppendChild(inputXMLDoc.CreateElement("Addr1")).InnerText = arrShipAdd[2];
                        withBlock.AppendChild(inputXMLDoc.CreateElement("City")).InnerText = arrShipAdd[3];
                        if (strQBCountry == "US" | blnQBNew)
                            withBlock.AppendChild(inputXMLDoc.CreateElement("State")).InnerText = arrShipAdd[4];
                        else
                            withBlock.AppendChild(inputXMLDoc.CreateElement("County")).InnerText = arrShipAdd[4];
                        withBlock.AppendChild(inputXMLDoc.CreateElement("PostalCode")).InnerText = arrShipAdd[5];
                        withBlock.AppendChild(inputXMLDoc.CreateElement("Country")).InnerText = arrShipAdd[6];
                    }


                    {
                        var withBlock = custAdd;
                        try
                        {
                            withBlock.AppendChild(inputXMLDoc.CreateElement("Phone")).InnerText = arrBillAdd[7];
                        }
                        catch (Exception ex)
                        {
                        }
                        withBlock.AppendChild(inputXMLDoc.CreateElement("Email")).InnerText = strEmailAddress;
                        withBlock.AppendChild(inputXMLDoc.CreateElement("Contact")).InnerText = strFullName;
                    }



                    request = inputXMLDoc.OuterXml;
                    Session("QBCustomerID") = intCustomerID;
                    Session("QBProcess") = "AddCustomer";
                }
                else
                {
                    // Customer is already in QB, just add order (sales receipt)
                    XmlDocument inputXMLDoc = null;
                    XmlElement qbXMLMsgsRq = null;
                    CreateQBReqXML(ref inputXMLDoc, ref qbXMLMsgsRq, (!blnQBNew) & strQBCountry == "UK");
                    XmlElement SalesReceiptAddRq = inputXMLDoc.CreateElement("SalesReceiptAddRq");
                    qbXMLMsgsRq.AppendChild(SalesReceiptAddRq);
                    SalesReceiptAddRq.SetAttribute("requestID", "2");

                    XmlElement SalesReceiptAdd = inputXMLDoc.CreateElement("SalesReceiptAdd");
                    SalesReceiptAddRq.AppendChild(SalesReceiptAdd);

                    XmlElement CustomerRef = inputXMLDoc.CreateElement("CustomerRef");
                    SalesReceiptAdd.AppendChild(CustomerRef);

                    CustomerRef.AppendChild(inputXMLDoc.CreateElement("ListID")).InnerText = strCustomerListID;

                    System.Data.DataTable DT;
                    Kartris.Basket objBasket = new Kartris.Basket();
                    BasketBLL objBasketBLL = new BasketBLL();
                    DT = objBasketBLL.GetCustomerOrderDetails(intOrderID);

                    DateTime dtOrderDate;
                    dtOrderDate = DT.Rows[0].Item["O_Date"];
                    string strOrderDate = dtOrderDate.ToString("yyyy-MM-dd");

                    SalesReceiptAdd.AppendChild(inputXMLDoc.CreateElement("TxnDate")).InnerText = strOrderDate;
                    SalesReceiptAdd.AppendChild(inputXMLDoc.CreateElement("RefNumber")).InnerText = intOrderID;

                    XmlElement BillAdd = inputXMLDoc.CreateElement("BillAddress");
                    SalesReceiptAdd.AppendChild(BillAdd);
                    {
                        var withBlock = BillAdd;
                        withBlock.AppendChild(inputXMLDoc.CreateElement("Addr1")).InnerText = arrBillAdd[2];
                        withBlock.AppendChild(inputXMLDoc.CreateElement("City")).InnerText = arrBillAdd[3];
                        if (strQBCountry == "US" | blnQBNew)
                            withBlock.AppendChild(inputXMLDoc.CreateElement("State")).InnerText = arrBillAdd[4];
                        else
                            withBlock.AppendChild(inputXMLDoc.CreateElement("County")).InnerText = arrBillAdd[4];
                        withBlock.AppendChild(inputXMLDoc.CreateElement("PostalCode")).InnerText = arrBillAdd[5];
                        withBlock.AppendChild(inputXMLDoc.CreateElement("Country")).InnerText = arrBillAdd[6];
                    }

                    // We need to specify the correct term here based on the QBXML number
                    string strSalesTaxRefCodeTerm = "SalesTaxCodeRef";
                    if ((!blnQBNew) & strQBCountry == "UK")
                        strSalesTaxRefCodeTerm = "TaxCodeRef";


                    XmlElement ShipAdd = inputXMLDoc.CreateElement("ShipAddress");
                    SalesReceiptAdd.AppendChild(ShipAdd);
                    {
                        var withBlock = ShipAdd;
                        withBlock.AppendChild(inputXMLDoc.CreateElement("Addr1")).InnerText = arrShipAdd[2];
                        withBlock.AppendChild(inputXMLDoc.CreateElement("City")).InnerText = arrShipAdd[3];
                        if (strQBCountry == "US" | blnQBNew)
                            withBlock.AppendChild(inputXMLDoc.CreateElement("State")).InnerText = arrShipAdd[4];
                        else
                            withBlock.AppendChild(inputXMLDoc.CreateElement("County")).InnerText = arrShipAdd[4];
                        withBlock.AppendChild(inputXMLDoc.CreateElement("PostalCode")).InnerText = arrShipAdd[5];
                        withBlock.AppendChild(inputXMLDoc.CreateElement("Country")).InnerText = arrShipAdd[6];
                    }

                    // check if the prices in the order are inctax or extax
                    bool APP_PricesIncTax = DT.Rows[0].Item["O_PricesIncTax"] == true;
                    if (strQBCountry == "UK")
                    {
                        {
                            var withBlock = SalesReceiptAdd;
                            if (blnQBNew)
                                withBlock.AppendChild(inputXMLDoc.CreateElement("IsTaxIncluded")).InnerText = APP_PricesIncTax.ToString().ToLower();
                        }
                    }


                    double numTaxDue = DT.Rows[0].Item["O_TaxDue"];


                    Dictionary<double, double> dicTaxRAtes = new Dictionary<double, double>();



                    double numDiscountPercentage = DT.Rows[0].Item["O_DiscountPercentage"];
                    if (numDiscountPercentage > 0)
                        numDiscountPercentage = numDiscountPercentage / 100;


                    foreach (DataRow drLine in DT.Rows)
                    {
                        string strVersionCode = drLine("IR_VersionCode");

                        double numPricePerItem, numTaxPerItem, numQuantity;
                        string strItemLineName;
                        // If drLine("IR_OptionsText") <> "" Then strItemLineName = strVersionCode & " - " & drLine("IR_OptionsText") Else 
                        strItemLineName = strVersionCode;
                        numPricePerItem = drLine("IR_PricePerItem");
                        numTaxPerItem = drLine("IR_TaxPerItem");
                        numQuantity = drLine("IR_Quantity");

                        double numRate = numPricePerItem;

                        if ((!blnQBNew) & APP_PricesIncTax)
                            numRate = numPricePerItem - numTaxPerItem;

                        // If APP_PricesIncTax Then
                        // numRate = numPricePerItem - numTaxPerItem
                        // Else
                        // numRate = numPricePerItem
                        // End If
                        // Else
                        // numRate = numPricePerItem
                        // 'If APP_PricesIncTax Then
                        // '    numRate = numPricePerItem
                        // 'Else
                        // '    numRate = 0.0001 + numPricePerItem + (numPricePerItem * numTaxPerItem)
                        // 'End If
                        // End If


                        if (numDiscountPercentage > 0)
                        {
                            // numRate = numRate - (numRate * numDiscountPercentage)
                            double numTaxRate;
                            double numTaxItemValue;
                            if (APP_PricesIncTax)
                                numTaxRate = TaxBLL.GetClosestRate(Math.Round((numTaxPerItem / numPricePerItem) * 100, 2));
                            else
                                numTaxRate = numTaxPerItem;

                            numTaxItemValue = (numRate * numDiscountPercentage) * numQuantity;

                            if (dicTaxRAtes.ContainsKey(numTaxRate))
                                dicTaxRAtes[numTaxPerItem] += numTaxItemValue;
                            else
                                dicTaxRAtes.Add(numTaxRate, numTaxItemValue);
                        }



                        XmlElement BasketItemLineAdd = inputXMLDoc.CreateElement("SalesReceiptLineAdd");
                        SalesReceiptAdd.AppendChild(BasketItemLineAdd);

                        XmlElement ItemRefBasketLineAdd = inputXMLDoc.CreateElement("ItemRef");
                        BasketItemLineAdd.AppendChild(ItemRefBasketLineAdd);
                        ItemRefBasketLineAdd.AppendChild(inputXMLDoc.CreateElement("ListID")).InnerText = Application("KartrisQBItemListID").ToString;
                        {
                            var withBlock = BasketItemLineAdd;
                            withBlock.AppendChild(inputXMLDoc.CreateElement("Desc")).InnerText = strItemLineName;
                            withBlock.AppendChild(inputXMLDoc.CreateElement("Quantity")).InnerText = numQuantity;
                            withBlock.AppendChild(inputXMLDoc.CreateElement("Rate")).InnerText = Math.Round(numRate, 2);
                        }

                        XmlElement SalesTaxCodeRefBasketLineAdd = inputXMLDoc.CreateElement(strSalesTaxRefCodeTerm);
                        BasketItemLineAdd.AppendChild(SalesTaxCodeRefBasketLineAdd);
                        SalesTaxCodeRefBasketLineAdd.AppendChild(inputXMLDoc.CreateElement("FullName")).InnerText = TaxBLL.GetQBTaxRefCode(strVersionCode);
                    }


                    if (numDiscountPercentage > 0)
                    {
                        foreach (var item in dicTaxRAtes)
                        {
                            XmlElement BasketItemLineAdd = inputXMLDoc.CreateElement("SalesReceiptLineAdd");
                            SalesReceiptAdd.AppendChild(BasketItemLineAdd);

                            XmlElement ItemRefBasketLineAdd = inputXMLDoc.CreateElement("ItemRef");
                            BasketItemLineAdd.AppendChild(ItemRefBasketLineAdd);
                            ItemRefBasketLineAdd.AppendChild(inputXMLDoc.CreateElement("ListID")).InnerText = Application("KartrisQBItemListID").ToString;
                            {
                                var withBlock = BasketItemLineAdd;
                                withBlock.AppendChild(inputXMLDoc.CreateElement("Desc")).InnerText = System.Convert.ToString(numDiscountPercentage * 100) + "% Discount on items with " + item.Key + "% tax";
                                withBlock.AppendChild(inputXMLDoc.CreateElement("Quantity")).InnerText = 1;
                                withBlock.AppendChild(inputXMLDoc.CreateElement("Rate")).InnerText = -Math.Round(item.Value, 2);
                            }

                            XmlElement SalesTaxCodeRefBasketLineAdd = inputXMLDoc.CreateElement(strSalesTaxRefCodeTerm);
                            BasketItemLineAdd.AppendChild(SalesTaxCodeRefBasketLineAdd);
                            SalesTaxCodeRefBasketLineAdd.AppendChild(inputXMLDoc.CreateElement("FullName")).InnerText = TaxBLL.GetQBTaxRefCode(System.Convert.ToDouble(item.Key));
                        }
                    }

                    // Add Coupon Discount Line (if there's any)
                    double numCouponDiscountTotal = DT.Rows[0].Item["O_CouponDiscountTotal"];
                    string strCouponCode = DT.Rows[0].Item["O_CouponCode"] + "";

                    if (strCouponCode != "")
                    {
                        XmlElement CouponLineAdd = inputXMLDoc.CreateElement("SalesReceiptLineAdd");
                        SalesReceiptAdd.AppendChild(CouponLineAdd);

                        XmlElement ItemRefCouponLineAdd = inputXMLDoc.CreateElement("ItemRef");
                        CouponLineAdd.AppendChild(ItemRefCouponLineAdd);
                        ItemRefCouponLineAdd.AppendChild(inputXMLDoc.CreateElement("ListID")).InnerText = Application("KartrisQBItemListID").ToString;

                        {
                            var withBlock = CouponLineAdd;
                            withBlock.AppendChild(inputXMLDoc.CreateElement("Desc")).InnerText = "Coupon Discount";
                            withBlock.AppendChild(inputXMLDoc.CreateElement("Quantity")).InnerText = "1";
                            withBlock.AppendChild(inputXMLDoc.CreateElement("Rate")).InnerText = Math.Round(numCouponDiscountTotal, 2);
                        }

                        XmlElement SalesTaxCodeRefCouponLineAdd = inputXMLDoc.CreateElement(strSalesTaxRefCodeTerm);
                        CouponLineAdd.AppendChild(SalesTaxCodeRefCouponLineAdd);
                        SalesTaxCodeRefCouponLineAdd.AppendChild(inputXMLDoc.CreateElement("FullName")).InnerText = "S";
                    }

                    // Add Promotion Discount Line
                    double numPromotionDiscountTotal = DT.Rows[0].Item["O_PromotionDiscountTotal"];

                    if (numPromotionDiscountTotal < 0)
                    {
                        XmlElement PromotionLineAdd = inputXMLDoc.CreateElement("SalesReceiptLineAdd");
                        SalesReceiptAdd.AppendChild(PromotionLineAdd);

                        XmlElement ItemRefPromotionLineAdd = inputXMLDoc.CreateElement("ItemRef");
                        PromotionLineAdd.AppendChild(ItemRefPromotionLineAdd);
                        ItemRefPromotionLineAdd.AppendChild(inputXMLDoc.CreateElement("ListID")).InnerText = Application("KartrisQBItemListID").ToString;

                        {
                            var withBlock = PromotionLineAdd;
                            withBlock.AppendChild(inputXMLDoc.CreateElement("Desc")).InnerText = "Promotion Discount";
                            withBlock.AppendChild(inputXMLDoc.CreateElement("Quantity")).InnerText = "1";
                            withBlock.AppendChild(inputXMLDoc.CreateElement("Rate")).InnerText = Math.Round(numPromotionDiscountTotal, 2);
                        }

                        XmlElement SalesTaxCodeRefPromotionLineAdd = inputXMLDoc.CreateElement(strSalesTaxRefCodeTerm);
                        PromotionLineAdd.AppendChild(SalesTaxCodeRefPromotionLineAdd);
                        SalesTaxCodeRefPromotionLineAdd.AppendChild(inputXMLDoc.CreateElement("FullName")).InnerText = TaxBLL.GetQBTaxRefCode(6);
                    }

                    // Add Shipping line
                    double numShippingPrice = DT.Rows[0].Item["O_ShippingPrice"];
                    if (blnQBNew)
                    {
                        if (APP_PricesIncTax)
                            numShippingPrice += DT.Rows[0].Item["O_ShippingTax"];
                    }

                    if (numShippingPrice > 0)
                    {
                        XmlElement ShippingLineAdd = inputXMLDoc.CreateElement("SalesReceiptLineAdd");
                        SalesReceiptAdd.AppendChild(ShippingLineAdd);

                        XmlElement ItemRefShippingLineAdd = inputXMLDoc.CreateElement("ItemRef");
                        ShippingLineAdd.AppendChild(ItemRefShippingLineAdd);
                        ItemRefShippingLineAdd.AppendChild(inputXMLDoc.CreateElement("ListID")).InnerText = Application("KartrisQBItemListID").ToString;

                        {
                            var withBlock = ShippingLineAdd;
                            withBlock.AppendChild(inputXMLDoc.CreateElement("Desc")).InnerText = "Shipping";
                            withBlock.AppendChild(inputXMLDoc.CreateElement("Quantity")).InnerText = "1";
                            withBlock.AppendChild(inputXMLDoc.CreateElement("Rate")).InnerText = Math.Round(numShippingPrice, 2);
                        }

                        XmlElement SalesTaxCodeRefShippingLineAdd = inputXMLDoc.CreateElement(strSalesTaxRefCodeTerm);
                        ShippingLineAdd.AppendChild(SalesTaxCodeRefShippingLineAdd);
                        SalesTaxCodeRefShippingLineAdd.AppendChild(inputXMLDoc.CreateElement("FullName")).InnerText = TaxBLL.GetQBTaxRefCode(System.Convert.ToInt32(KartSettingsManager.GetKartConfig("frontend.checkout.shipping.taxband")));
                    }


                    // Add Order Handling Charge line
                    double numOrderHandlingPrice = DT.Rows[0].Item["O_OrderHandlingCharge"];
                    if (blnQBNew)
                    {
                        if (APP_PricesIncTax)
                            numOrderHandlingPrice += DT.Rows[0].Item["O_OrderHandlingChargeTax"];
                    }


                    if (numOrderHandlingPrice > 0)
                    {
                        XmlElement OrderHandlingLineAdd = inputXMLDoc.CreateElement("SalesReceiptLineAdd");
                        SalesReceiptAdd.AppendChild(OrderHandlingLineAdd);

                        XmlElement ItemRefHandlingLineAdd = inputXMLDoc.CreateElement("ItemRef");
                        OrderHandlingLineAdd.AppendChild(ItemRefHandlingLineAdd);
                        ItemRefHandlingLineAdd.AppendChild(inputXMLDoc.CreateElement("ListID")).InnerText = Application("KartrisQBItemListID").ToString;

                        {
                            var withBlock = OrderHandlingLineAdd;
                            withBlock.AppendChild(inputXMLDoc.CreateElement("Desc")).InnerText = "Order Handling Charge";
                            withBlock.AppendChild(inputXMLDoc.CreateElement("Quantity")).InnerText = "1";
                            withBlock.AppendChild(inputXMLDoc.CreateElement("Rate")).InnerText = Math.Round(numOrderHandlingPrice, 2);
                        }

                        XmlElement SalesTaxCodeRefHandlingLineAdd = inputXMLDoc.CreateElement(strSalesTaxRefCodeTerm);
                        OrderHandlingLineAdd.AppendChild(SalesTaxCodeRefHandlingLineAdd);
                        SalesTaxCodeRefHandlingLineAdd.AppendChild(inputXMLDoc.CreateElement("FullName")).InnerText = TaxBLL.GetQBTaxRefCode(System.Convert.ToInt32(KartSettingsManager.GetKartConfig("frontend.checkout.orderhandlingchargetaxband")));
                    }

                    // How about the customer discount???

                    // MAYBE WE'LL SUPPORT THESE TAGS LATER
                    // .AppendChild(inputXMLDoc.CreateElement("IsPending")).InnerText = "true"
                    // .AppendChild(inputXMLDoc.CreateElement("IsToBePrinted")).InnerText = "true"
                    // .AppendChild(inputXMLDoc.CreateElement("IsToBeEmailed")).InnerText = "false"

                    if (strQBCountry == "UK")
                    {
                        {
                            var withBlock = SalesReceiptAdd;
                            if (!blnQBNew)
                                withBlock.AppendChild(inputXMLDoc.CreateElement("AmountIncludesVAT")).InnerText = APP_PricesIncTax.ToString().ToLower();
                        }
                    }


                    request = inputXMLDoc.OuterXml;
                    Session("QBProcess") = "AddOrder";
                    Session("O_ctr") += 1;
                }
            }
        }
        else
        {
            XmlDocument inputXMLDoc = null;
            XmlElement qbXMLMsgsRq = null;
            CreateQBReqXML(ref inputXMLDoc, ref qbXMLMsgsRq, (!blnQBNew) & strQBCountry == "UK");
            XmlElement ItemQueryRq = inputXMLDoc.CreateElement("ItemQueryRq");
            qbXMLMsgsRq.AppendChild(ItemQueryRq);
            ItemQueryRq.SetAttribute("requestID", "1");
            ItemQueryRq.AppendChild(inputXMLDoc.CreateElement("FullName")).InnerText = "Kartris Order Item";
            request = inputXMLDoc.OuterXml;
            Session("QBProcess") = "SearchKartrisItem";
        }

        string evLogTxt = "WebMethod: sendRequestXML() has been called by QBWebconnector" + Constants.vbCrLf + Constants.vbCrLf;
        evLogTxt += "Parameters received:" + Constants.vbCrLf;
        evLogTxt += ("string ticket = ") + ticket + Constants.vbCrLf;
        evLogTxt += ("string strHCPResponse = ") + strHCPResponse + Constants.vbCrLf;
        evLogTxt += ("string strCompanyFileName = ") + strCompanyFileName + Constants.vbCrLf;
        evLogTxt += ("string qbXMLCountry = ") + qbXMLCountry + Constants.vbCrLf;
        evLogTxt += ("int qbXMLMajorVers = ") + qbXMLMajorVers.ToString() + Constants.vbCrLf;
        evLogTxt += ("int qbXMLMinorVers = ") + qbXMLMinorVers.ToString() + Constants.vbCrLf;
        evLogTxt += Constants.vbCrLf + Constants.vbCrLf;
        evLogTxt += "Return values: " + Constants.vbCrLf;
        evLogTxt += ("string request = ") + request + Constants.vbCrLf;
        // CkartrisFormatErrors.LogError(evLogTxt)
        return request;
    }
    private void CreateQBReqXML(ref XmlDocument inputXMLDoc, ref XmlElement qbXMLMgsRq, bool blnOldUK)
    {
        inputXMLDoc = new XmlDocument();
        inputXMLDoc.AppendChild(inputXMLDoc.CreateXmlDeclaration("1.0", null, null));
        if (blnOldUK)
            inputXMLDoc.AppendChild(inputXMLDoc.CreateProcessingInstruction("qbxml", "version=\"UK2.0\""));
        else
            inputXMLDoc.AppendChild(inputXMLDoc.CreateProcessingInstruction("qbxml", "version=\"6.0\""));

        XmlElement qbXML = inputXMLDoc.CreateElement("QBXML");
        inputXMLDoc.AppendChild(qbXML);
        qbXMLMgsRq = inputXMLDoc.CreateElement("QBXMLMsgsRq");
        qbXML.AppendChild(qbXMLMgsRq);
        qbXMLMgsRq.SetAttribute("onError", "stopOnError");
    }

    private string parseForVersion(string input)
    {
        // This method is created just to parse the first two version components
        // out of the standard four component version number:
        // <Major>.<Minor>.<Release>.<Build>
        // 
        string retVal = "";
        string major = "";
        string minor = "";
        System.Text.RegularExpressions.Regex version = new System.Text.RegularExpressions.Regex(@"^(?<major>\d+)\.(?<minor>\d+)(\.\w+){0,2}$", System.Text.RegularExpressions.RegexOptions.Compiled);
        System.Text.RegularExpressions.Match versionMatch = version.Match(input);
        if (versionMatch.Success)
        {
            major = versionMatch.Result("${major}");
            minor = versionMatch.Result("${minor}");
            retVal = (major + ".") + minor;
        }
        else
            retVal = input;
        return retVal;
    }
    /// <summary>
    ///     ''' WebMethod - serverVersion()
    ///     ''' To enable web service with its version number returned back to QBWC
    ///     ''' Signature: public string serverVersion()
    ///     '''
    ///     ''' OUT: 
    ///     ''' string 
    ///     ''' Possible values: 
    ///     ''' Version string representing server version
    ///     ''' </summary>

    [WebMethod()]
    public string serverVersion()
    {
        serverVersion = "2.0.0.1";
        string evLogTxt = "WebMethod: serverVersion() has been called " + "by QBWebconnector" + Constants.vbCrLf + Constants.vbCrLf;
        evLogTxt += "No Parameters required.";
        evLogTxt += ("Returned: ") + serverVersion;
        return serverVersion;
    }
    private void DoVersionCheck(string strResponse)
    {
        XmlDocument outputXMLDoc = new XmlDocument();
        outputXMLDoc.LoadXml(strResponse);
        XmlNodeList qbXMLMsgsRsNodeList = outputXMLDoc.GetElementsByTagName("HostRet");
        XmlNodeList HostRet = qbXMLMsgsRsNodeList.Item(0).ChildNodes;
        foreach (XmlNode custRetNode in HostRet)
        {
            if (custRetNode.Name.Equals("Country"))
                Session("QBCountry") = custRetNode.InnerText;
            else if (custRetNode.Name.Equals("MajorVersion"))
            {
                int intMajorVersion = System.Convert.ToInt32(custRetNode.InnerText);
                if (intMajorVersion < 16)
                    Session("QBNew") = false;
                else
                    Session("QBNew") = true;
            }
            else if (custRetNode.Name.Equals("SupportedQBXMLVersion"))
            {
                if (custRetNode.InnerText == "6.0")
                    Session("QBNew") = true;
                else if (custRetNode.InnerText == "UK2.0")
                {
                    Session("QBCountry") = "US";
                    Session("QBNew") = false;
                    break;
                }
            }
        }
        HostRet = null;
        qbXMLMsgsRsNodeList = null;
        outputXMLDoc = null;
    }
}
