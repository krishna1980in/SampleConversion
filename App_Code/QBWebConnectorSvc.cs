// ------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
// Runtime Version:2.0.50727.4200
// 
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

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

using System.ComponentModel;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

// 
// This source code was auto-generated by wsdl, Version=2.0.50727.3038.
// 

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCode("wsdl", "2.0.50727.3038")]
[System.Web.Services.WebServiceAttribute(Namespace = "http://developer.intuit.com/")]
[System.Web.Services.WebServiceBindingAttribute(Name = "QBWebConnectorSvcSoap", Namespace = "http://developer.intuit.com/")]
public abstract partial class QBWebConnectorSvc : System.Web.Services.WebService
{

    /// <remarks/>
    [System.Web.Services.WebMethodAttribute()]
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://developer.intuit.com/authenticate", RequestNamespace = "http://developer.intuit.com/", ResponseNamespace = "http://developer.intuit.com/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    public abstract string[] authenticate(string strUserName, string strPassword);

    /// <remarks/>
    [System.Web.Services.WebMethodAttribute()]
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://developer.intuit.com/sendRequestXML", RequestNamespace = "http://developer.intuit.com/", ResponseNamespace = "http://developer.intuit.com/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    public abstract string sendRequestXML(string ticket, string strHCPResponse, string strCompanyFileName, string qbXMLCountry, int qbXMLMajorVers, int qbXMLMinorVers);

    /// <remarks/>
    [System.Web.Services.WebMethodAttribute()]
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://developer.intuit.com/receiveResponseXML", RequestNamespace = "http://developer.intuit.com/", ResponseNamespace = "http://developer.intuit.com/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    public abstract int receiveResponseXML(string ticket, string response, string hresult, string message);

    /// <remarks/>
    [System.Web.Services.WebMethodAttribute()]
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://developer.intuit.com/connectionError", RequestNamespace = "http://developer.intuit.com/", ResponseNamespace = "http://developer.intuit.com/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    public abstract string connectionError(string ticket, string hresult, string message);

    /// <remarks/>
    [System.Web.Services.WebMethodAttribute()]
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://developer.intuit.com/getLastError", RequestNamespace = "http://developer.intuit.com/", ResponseNamespace = "http://developer.intuit.com/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    public abstract string getLastError(string ticket);

    /// <remarks/>
    [System.Web.Services.WebMethodAttribute()]
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://developer.intuit.com/closeConnection", RequestNamespace = "http://developer.intuit.com/", ResponseNamespace = "http://developer.intuit.com/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    public abstract string closeConnection(string ticket);
}
