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
using Microsoft.VisualBasic;
using CkartrisBLL;

partial class Admin_PaymentGateways : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Kartris", "ContentText_PaymentShippingGateways") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        if (!Page.IsPostBack)
        {
            RefreshGatewayDisplay();
            ArrayList objBasket = new ArrayList();
        }
    }

    protected void RefreshGatewayDisplay()
    {
        Kartris.Interfaces.PaymentGateway clsPlugin;
        Kartris.Interfaces.ShippingGateway clsShippingPlugin;
        DataSet dtsGateways = new DataSet();
        DataTable tblGateways = new DataTable();
        DataColumn colGateway;
        string strDLLPath;

        {
            var withBlock = tblGateways.Columns;
            colGateway = new DataColumn("ID", System.Type.GetType("System.Int32"));
            withBlock.Add(colGateway);
            colGateway = new DataColumn("Name", System.Type.GetType("System.String"));
            withBlock.Add(colGateway);
            colGateway = new DataColumn("DLL", System.Type.GetType("System.String"));
            withBlock.Add(colGateway);
            colGateway = new DataColumn("Type", System.Type.GetType("System.String"));
            withBlock.Add(colGateway);
            colGateway = new DataColumn("Status", System.Type.GetType("System.String"));
            withBlock.Add(colGateway);
        }

        string[] aryGateways = Strings.Split(LoadGateways, ",");

        foreach (var strGatewayName in aryGateways)
        {
            strGatewayName = Strings.Left(strGatewayName, Strings.InStr(strGatewayName, "::") - 1);
            strDLLPath = Server.MapPath("~/Plugins/" + strGatewayName + "/" + strGatewayName + ".dll");
            // Response.Write(strDLLPath)
            if (!string.IsNullOrEmpty(strGatewayName))
            {
                try
                {
                    clsPlugin = Payment.PPLoader(strGatewayName);
                    if (clsPlugin != null)
                    {
                        DataRow rowGateway = tblGateways.NewRow;
                        rowGateway("ID") = 1;
                        rowGateway("Name") = clsPlugin.GatewayName;
                        rowGateway("DLL") = FileVersionInfo.GetVersionInfo(strDLLPath).FileVersion;
                        rowGateway("Type") = GetGlobalResourceObject("_Orders", "ContentText_PaymentGateWay");
                        rowGateway("Status") = UCase(clsPlugin.Status);
                        tblGateways.Rows.Add(rowGateway);
                    }
                    else
                    {
                        clsShippingPlugin = Payment.SPLoader(strGatewayName);
                        if (clsShippingPlugin != null)
                        {
                            DataRow rowGateway = tblGateways.NewRow;
                            rowGateway("ID") = 1;
                            rowGateway("Name") = clsShippingPlugin.GatewayName;
                            rowGateway("DLL") = FileVersionInfo.GetVersionInfo(strDLLPath).FileVersion;
                            rowGateway("Type") = GetGlobalResourceObject("_Orders", "ContentText_ShippingGateway");
                            rowGateway("Status") = UCase(clsShippingPlugin.Status);
                            tblGateways.Rows.Add(rowGateway);
                        }
                    }
                    clsPlugin = null/* TODO Change to default(_) if this is not a reference type */;
                    clsShippingPlugin = null/* TODO Change to default(_) if this is not a reference type */;
                }
                catch (Exception ex)
                {
                    // Whoops, this means something didn't go right above
                    CkartrisFormatErrors.LogError("Error loading up a payment gateway - " + ex.Message);
                }
            }
        }

        if (Information.UBound(aryGateways) > -1)
        {
            rptGateways.DataSource = tblGateways;
            rptGateways.DataBind();
        }
    }

    public string ShowBlank(string strStatus)
    {
        if (strStatus == "OFF")
            return "-";
        else
            return strStatus;
    }

    protected void btnRefresh_Click(object sender, System.EventArgs e)
    {
        RefreshGatewayDisplay();
        (Skins_Admin_Template)Page.Master.LoadCategoryMenu();
    }

    private string LoadGateways()
    {
        string strGatewayListConfig = "";
        string strGatewayListDisplay = "";
        string[] files = Directory.GetFiles(Server.MapPath("~/Plugins/"), "*.dll", SearchOption.AllDirectories);
        foreach (string File in files)
        {
            if (IsValidPaymentGatewayPlugin(File.ToString()) & !Strings.InStr(File.ToString(), "Kartris.Interfaces.dll"))
            {
                if (!string.IsNullOrEmpty(strGatewayListDisplay))
                    strGatewayListDisplay += ",";
                string strGatewayName = Strings.Replace(Strings.Mid(File.ToString(), File.LastIndexOf(@"\") + 2), ".dll", "");
                Kartris.Interfaces.PaymentGateway GatewayPlugin = Payment.PPLoader(strGatewayName);
                if (GatewayPlugin.Status.ToLower != "off")
                {
                    if (!string.IsNullOrEmpty(strGatewayListConfig))
                        strGatewayListConfig += ",";
                    strGatewayListConfig += strGatewayName + "::" + GatewayPlugin.Status.ToLower + "::" + GatewayPlugin.AuthorizedOnly.ToLower + "::" + Payment.isAnonymousCheckoutEnabled(strGatewayName) + "::p";
                }
                strGatewayListDisplay += strGatewayName + "::" + GatewayPlugin.Status.ToLower + "::" + GatewayPlugin.AuthorizedOnly.ToLower + "::" + Payment.isAnonymousCheckoutEnabled(strGatewayName) + "::p";
                GatewayPlugin = null/* TODO Change to default(_) if this is not a reference type */;
            }
            else if (IsValidShippingGatewayPlugin(File.ToString()) & !Strings.InStr(File.ToString(), "Kartris.Interfaces.dll"))
            {
                if (!string.IsNullOrEmpty(strGatewayListDisplay))
                    strGatewayListDisplay += ",";
                string strGatewayName = Strings.Replace(Strings.Mid(File.ToString(), File.LastIndexOf(@"\") + 2), ".dll", "");
                Kartris.Interfaces.ShippingGateway GatewayPlugin = Payment.SPLoader(strGatewayName);
                if (GatewayPlugin.Status.ToLower != "off")
                {
                    if (!string.IsNullOrEmpty(strGatewayListConfig))
                        strGatewayListConfig += ",";
                    strGatewayListConfig += strGatewayName + "::" + GatewayPlugin.Status.ToLower + "::n::false::s";
                }
                strGatewayListDisplay += strGatewayName + "::" + GatewayPlugin.Status.ToLower + "::n::false::s";
                GatewayPlugin = null/* TODO Change to default(_) if this is not a reference type */;
            }
        }
        KartSettingsManager.SetKartConfig("frontend.payment.gatewayslist", strGatewayListConfig);
        return strGatewayListDisplay;
    }

    private bool IsValidPaymentGatewayPlugin(string GateWayPath)
    {
        bool blnResult = false;
        try
        {
            string strGatewayName = Strings.Replace(Strings.Mid(GateWayPath, GateWayPath.LastIndexOf(@"\") + 2), ".dll", "");
            Kartris.Interfaces.PaymentGateway GatewayPlugin = Payment.PPLoader(strGatewayName);
            if (GatewayPlugin != null)
                blnResult = true;
            GatewayPlugin = null/* TODO Change to default(_) if this is not a reference type */;
        }
        catch (Exception ex)
        {
        }

        return blnResult;
    }

    private bool IsValidShippingGatewayPlugin(string GateWayPath)
    {
        bool blnResult = false;
        try
        {
            string strGatewayName = Strings.Replace(Strings.Mid(GateWayPath, GateWayPath.LastIndexOf(@"\") + 2), ".dll", "");
            Kartris.Interfaces.ShippingGateway GatewayPlugin = Payment.SPLoader(strGatewayName);
            if (GatewayPlugin != null)
                blnResult = true;
            GatewayPlugin = null/* TODO Change to default(_) if this is not a reference type */;
        }
        catch (Exception ex)
        {
        }

        return blnResult;
    }
}
