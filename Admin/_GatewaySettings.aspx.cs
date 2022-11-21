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
using CkartrisEnumerations;
using CkartrisImages;
using CkartrisFormatErrors;

partial class Admin_GatewaySettings : _PageBaseClass
{
    private string _GatewayName = "";

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Kartris", "ContentText_PaymentShippingGateways") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");
    }

    protected void Page_Init(object sender, System.EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request.QueryString("g")))
        {
            bool blnIsProtected = false;
            _GatewayName = Server.HtmlEncode(Request.QueryString("g"));
            if (_GatewayName != "")
            {
                ClientSettingsSection objGatewaySettingsSection = GetGatewaySettings(_GatewayName);
                if (objGatewaySettingsSection != null)
                {
                    foreach (var txtBox in phdSettings.Controls.OfType<TextBox>())
                        phdSettings.Controls.Remove(txtBox);
                    foreach (var lblControl in phdSettings.Controls.OfType<Label>())
                        phdSettings.Controls.Remove(lblControl);
                    lblGatewayName.Text = _GatewayName;
                    blnIsProtected = objGatewaySettingsSection.Settings.Get("IsProtected").Value.ValueXml.InnerText.ToLower == "yes";
                    if (blnIsProtected)
                        chkEncrypt.Checked = true;
                    else
                        chkEncrypt.Checked = false;

                    // Generate a friendly name textbox for each live language in the backend - LANG_LiveBack
                    DataTable dtbLanguages = LanguagesBLL.GetLanguages;
                    foreach (DataRow drwLanguage in dtbLanguages.Rows)
                    {
                        if (drwLanguage("LANG_LiveBack") == true)
                        {
                            string strSettingName = "FriendlyName(" + CkartrisDataManipulation.FixNullFromDB(drwLanguage("LANG_Culture")) + ")";
                            // Start of line
                            Literal litLineStart = new Literal();
                            litLineStart.Text = "<li><span class=\"Kartris-DetailsView-Name\">";
                            phdSettings.Controls.Add(litLineStart);

                            // Add the label control
                            Label lblSetting = new Label();
                            lblSetting.Text = strSettingName;
                            lblSetting.ID = "lbl" + _GatewayName + strSettingName;
                            phdSettings.Controls.Add(lblSetting);

                            // Middle of line
                            Literal litLineMiddle = new Literal();
                            litLineMiddle.Text = "</span><span class=\"Kartris-DetailsView-Value\">";
                            phdSettings.Controls.Add(litLineMiddle);

                            TextBox txtSetting = new TextBox();
                            txtSetting.ID = "txt" + _GatewayName + strSettingName;
                            phdSettings.Controls.Add(txtSetting);

                            // End of line
                            Literal litLineEnd = new Literal();
                            litLineEnd.Text = "</span></li>";
                            phdSettings.Controls.Add(litLineEnd);
                        }
                    }

                    bool blnAnonymousCheckout = false;

                    foreach (SettingElement keySettingElement in objGatewaySettingsSection.Settings)
                    {
                        if (keySettingElement.Name.ToLower != "isprotected")
                        {
                            if (!InStr(UCase(keySettingElement.Name), "FRIENDLYNAME(") > 0 && (keySettingElement.Name.ToLower != "anonymouscheckout"))
                            {
                                // Start of line
                                Literal litLineStart = new Literal();
                                litLineStart.Text = "<li><span class=\"Kartris-DetailsView-Name\">";
                                phdSettings.Controls.Add(litLineStart);

                                // Add the label control
                                Label lblSetting = new Label();
                                lblSetting.Text = keySettingElement.Name;
                                lblSetting.ID = "lbl" + _GatewayName + keySettingElement.Name;
                                phdSettings.Controls.Add(lblSetting);

                                // Middle of line
                                Literal litLineMiddle = new Literal();
                                litLineMiddle.Text = "</span><span class=\"Kartris-DetailsView-Value\">";
                                phdSettings.Controls.Add(litLineMiddle);

                                // Add form input control (text, checkbox, etc)
                                switch (UCase(keySettingElement.Name))
                                {
                                    case "STATUS":
                                        {
                                            DropDownList ddlStatus = new DropDownList();
                                            ddlStatus.ID = "ddlStatus";
                                            ddlStatus.Items.Add("ON");
                                            ddlStatus.Items.Add("OFF");
                                            ddlStatus.Items.Add("TEST");
                                            ddlStatus.Items.Add("FAKE");
                                            ddlStatus.SelectedValue = UCase(GetConfigValue(keySettingElement, blnIsProtected));
                                            phdSettings.Controls.Add(ddlStatus);
                                            break;
                                        }

                                    case "AUTHORIZEDONLY":
                                        {
                                            CheckBox chkAuthorizedOnly = new CheckBox();
                                            chkAuthorizedOnly.ID = "chkAuthorizedOnly";
                                            chkAuthorizedOnly.CssClass = "checkbox";
                                            if (UCase(GetConfigValue(keySettingElement, blnIsProtected)) == "TRUE")
                                                chkAuthorizedOnly.Checked = true;
                                            else
                                                chkAuthorizedOnly.Checked = false;
                                            phdSettings.Controls.Add(chkAuthorizedOnly);
                                            break;
                                        }

                                    default:
                                        {
                                            TextBox txtSetting = new TextBox();
                                            if (!IsPostBack)
                                                txtSetting.Text = GetConfigValue(keySettingElement, blnIsProtected);
                                            txtSetting.ID = "txt" + _GatewayName + keySettingElement.Name;
                                            phdSettings.Controls.Add(txtSetting);
                                            break;
                                        }
                                }

                                // End of line
                                Literal litLineEnd = new Literal();
                                litLineEnd.Text = "</span></li>";
                                phdSettings.Controls.Add(litLineEnd);
                            }
                            else if (keySettingElement.Name.ToLower != "anonymouscheckout")
                            {
                                // Populate the friendly name textboxes
                                TextBox txtSetting = (TextBox)FindControlRecursive(phdSettings, "txt" + _GatewayName + keySettingElement.Name);
                                if (txtSetting != null & !IsPostBack)
                                    txtSetting.Text = GetConfigValue(keySettingElement, false);
                            }
                            else if (UCase(GetConfigValue(keySettingElement, blnIsProtected)) == "TRUE")
                                blnAnonymousCheckout = true;
                            else
                                blnAnonymousCheckout = false;
                        }
                    }

                    // Start of line
                    Literal litAnoLineStart = new Literal();
                    litAnoLineStart.Text = "<li><span class=\"Kartris-DetailsView-Name\">";
                    phdSettings.Controls.Add(litAnoLineStart);

                    // Add the label control
                    Label lblAnoSetting = new Label();
                    lblAnoSetting.Text = "AnonymousCheckout";
                    lblAnoSetting.ID = "lbl" + _GatewayName + "AnonymousCheckout";
                    phdSettings.Controls.Add(lblAnoSetting);

                    // Middle of line
                    Literal litAnoLineMiddle = new Literal();
                    litAnoLineMiddle.Text = "</span><span class=\"Kartris-DetailsView-Value\">";
                    phdSettings.Controls.Add(litAnoLineMiddle);

                    CheckBox chkAnonymousCheckout = new CheckBox();
                    chkAnonymousCheckout.ID = "chkAnonymousCheckout";
                    chkAnonymousCheckout.CssClass = "checkbox";
                    if (blnAnonymousCheckout)
                        chkAnonymousCheckout.Checked = true;
                    else
                        chkAnonymousCheckout.Checked = false;
                    phdSettings.Controls.Add(chkAnonymousCheckout);

                    // End of line
                    Literal litAnoLineEnd = new Literal();
                    litAnoLineEnd.Text = "</span></li>";
                    phdSettings.Controls.Add(litAnoLineEnd);


                    btnUpdate.Visible = true;
                    chkEncrypt.Visible = true;

                    btnUpdate.CommandArgument = _GatewayName;
                }
            }
        }
    }

    private string GetConfigValue(SettingElement elemSetting, bool isProtected)
    {
        string strSettingValue = Interfaces.Utils.TrimWhiteSpace(elemSetting.Value.ValueXml.InnerText);
        if (isProtected)
            return Interfaces.Utils.Crypt(strSettingValue, ConfigurationManager.AppSettings("HashSalt").ToString, Interfaces.Utils.CryptType.Decrypt);
        else
            return strSettingValue;
    }

    private string SetConfigValue(string strValue, bool blnProtectSetting)
    {
        if (blnProtectSetting)
            return Interfaces.Utils.Crypt(strValue, ConfigurationManager.AppSettings("HashSalt").ToString, Interfaces.Utils.CryptType.Encrypt);
        else
            return strValue;
    }

    protected void btnUpdate_Click(object sender, System.EventArgs e)
    {
        string strGatewayName = btnUpdate.CommandArgument;
        ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();

        configFileMap.ExeConfigFilename = Path.Combine(Request.PhysicalApplicationPath, @"Plugins\" + strGatewayName + @"\" + strGatewayName + ".dll.config");
        configFileMap.MachineConfigFilename = Path.Combine(Request.PhysicalApplicationPath, @"Uploads\resources\Machine.Config");

        System.Configuration objConfiguration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        ConfigurationSectionGroup grpSection = objConfiguration.GetSectionGroup("applicationSettings");
        ClientSettingsSection objGatewaySettingsSection = (ClientSettingsSection)grpSection.Sections.Item("Kartris.My.MySettings");

        if (objGatewaySettingsSection != null)
        {
            bool blnFound = false;

            foreach (SettingElement keySettingElement in objGatewaySettingsSection.Settings)
            {
                switch (UCase(keySettingElement.Name))
                {
                    case "STATUS":
                        {
                            DropDownList ddlStatus = (DropDownList)FindControlRecursive(phdSettings, "ddlStatus");
                            if (ddlStatus != null)
                            {
                                keySettingElement.Value.ValueXml.InnerText = SetConfigValue(ddlStatus.SelectedValue, chkEncrypt.Checked);
                                blnFound = true;
                            }

                            break;
                        }

                    case "AUTHORIZEDONLY":
                        {
                            CheckBox chkAuthorizedOnly = (CheckBox)FindControlRecursive(phdSettings, "chkAuthorizedOnly");
                            if (chkAuthorizedOnly != null)
                            {
                                keySettingElement.Value.ValueXml.InnerText = SetConfigValue(UCase(chkAuthorizedOnly.Checked.ToString), chkEncrypt.Checked);
                                blnFound = true;
                            }

                            break;
                        }

                    case "ISPROTECTED":
                        {
                            if (chkEncrypt.Checked)
                                keySettingElement.Value.ValueXml.InnerText = "Yes";
                            else
                                keySettingElement.Value.ValueXml.InnerText = "No";
                            break;
                        }

                    default:
                        {
                            if (!InStr(UCase(keySettingElement.Name), "FriendlyName") > 0)
                            {
                                TextBox txtSetting = (TextBox)FindControlRecursive(phdSettings, "txt" + strGatewayName + keySettingElement.Name);
                                if (txtSetting != null)
                                {
                                    keySettingElement.Value.ValueXml.InnerText = SetConfigValue(txtSetting.Text, chkEncrypt.Checked);
                                    blnFound = true;
                                }
                            }

                            break;
                        }
                }
            }
            if (blnFound)
            {
                // Process all Friendly Name textboxes
                foreach (var txtBox in phdSettings.Controls.OfType<TextBox>())
                {
                    int intNamePos = Strings.InStr(UCase(txtBox.ID), "FRIENDLYNAME(");
                    if (intNamePos > 0)
                    {
                        string strSettingElementName = Strings.Mid(txtBox.ID, intNamePos);
                        bool blnSettingElementExists = false;
                        foreach (SettingElement keySettingElement in objGatewaySettingsSection.Settings)
                        {
                            if (UCase(keySettingElement.Name) == Strings.UCase(strSettingElementName))
                            {
                                keySettingElement.Value.ValueXml.InnerText = txtBox.Text;
                                blnSettingElementExists = true;
                            }
                        }
                        // Friendly Name doesn't exist in the config file yet - add it
                        if (!blnSettingElementExists)
                        {
                            XmlDocument docFriendlyName = new XmlDocument();
                            SettingElement seFriendlyName = new SettingElement(strSettingElementName, SettingsSerializeAs.String);
                            SettingValueElement veFriendlyName = new SettingValueElement();
                            XmlNode nodeFriendlyName = docFriendlyName.CreateNode(XmlNodeType.Element, "value", "");
                            nodeFriendlyName.InnerText = txtBox.Text;
                            veFriendlyName.ValueXml = nodeFriendlyName;
                            seFriendlyName.Value = veFriendlyName;
                            objGatewaySettingsSection.Settings.Add(seFriendlyName);
                        }
                    }
                }

                // check if anonymous checkout is already present in config file
                CheckBox chkAnonymousCheckout = (CheckBox)FindControlRecursive(phdSettings, "chkAnonymousCheckout");
                string strAnonymousCheckoutValue = SetConfigValue(UCase(chkAnonymousCheckout.Checked.ToString), chkEncrypt.Checked);
                bool blnAnonymousCheckoutExists = false;
                foreach (SettingElement keySettingElement in objGatewaySettingsSection.Settings)
                {
                    if (UCase(keySettingElement.Name) == Strings.UCase("ANONYMOUSCHECKOUT"))
                    {
                        keySettingElement.Value.ValueXml.InnerText = strAnonymousCheckoutValue;
                        blnAnonymousCheckoutExists = true;
                    }
                }

                if (!blnAnonymousCheckoutExists)
                {
                    XmlDocument docFriendlyName = new XmlDocument();
                    SettingElement seFriendlyName = new SettingElement("AnonymousCheckout", SettingsSerializeAs.String);
                    SettingValueElement veFriendlyName = new SettingValueElement();
                    XmlNode nodeFriendlyName = docFriendlyName.CreateNode(XmlNodeType.Element, "value", "");
                    nodeFriendlyName.InnerText = strAnonymousCheckoutValue;
                    veFriendlyName.ValueXml = nodeFriendlyName;
                    seFriendlyName.Value = veFriendlyName;
                    objGatewaySettingsSection.Settings.Add(seFriendlyName);
                }

                objGatewaySettingsSection.SectionInformation.ForceSave = true;

                try
                {
                    // Clean up the temporary folder first in case something went wrong in the last gateway setting update
                    foreach (string TempSettingFile in Directory.GetFiles(Path.Combine(Request.PhysicalApplicationPath, @"Uploads\"), "*.dll.config"))
                        File.Delete(TempSettingFile);

                    string strTempPath = Path.Combine(Request.PhysicalApplicationPath, @"Uploads\" + strGatewayName + ".dll.config");
                    string strConfigFilePath = objConfiguration.FilePath;
                    objConfiguration.SaveAs(strTempPath, ConfigurationSaveMode.Modified);
                    objConfiguration = null/* TODO Change to default(_) if this is not a reference type */;

                    // Reload saved config file, remove whitespace from empty settings, delete file and save again
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.Load(strTempPath);
                    StringWriter StringWriter = new StringWriter();
                    XmlTextWriter XmlTextWriter = new XmlTextWriter(StringWriter);
                    xmlDoc.WriteTo(XmlTextWriter);
                    Regex regex = new Regex(@"<value>\s*</value>");
                    string cleanedXml = regex.Replace(StringWriter.ToString(), "<value></value>");
                    xmlDoc = null/* TODO Change to default(_) if this is not a reference type */;
                    File.Delete(strTempPath);
                    StreamWriter objStreamWriter;
                    objStreamWriter = File.AppendText(strTempPath);
                    objStreamWriter.Write(cleanedXml);
                    objStreamWriter.Close();

                    // replace the config file in the gateway folder with the updated one in the uploads folder
                    File.Delete(strConfigFilePath);
                    File.Copy(strTempPath, strConfigFilePath);
                    // clean up - remove the temp config file from the uploads folder
                    File.Delete(strTempPath);

                    // refresh the payment gateways settings cache
                    Kartris.Interfaces.PaymentGateway clsPlugin = null/* TODO Change to default(_) if this is not a reference type */;
                    try
                    {
                        clsPlugin = Payment.PPLoader(strGatewayName);
                        clsPlugin.RefreshSettingsCache();
                    }
                    catch (Exception ex)
                    {
                    }

                    // refresh the shipping gateways settings cache
                    if (clsPlugin == null)
                    {
                        try
                        {
                            Kartris.Interfaces.ShippingGateway clsShippingPlugin;
                            clsShippingPlugin = Payment.SPLoader(strGatewayName);
                            clsShippingPlugin.RefreshSettingsCache();
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    clsPlugin = null/* TODO Change to default(_) if this is not a reference type */;

                    // update the gatewayslist config setting
                    string strGatewayListConfig = "";
                    string strGatewayListDisplay = "";
                    string[] files = Directory.GetFiles(Server.MapPath("~/Plugins/"), "*.dll", SearchOption.AllDirectories);
                    foreach (string File in files)
                    {
                        if (IsValidGatewayPlugin(File.ToString()) & !Strings.InStr(File.ToString(), "Kartris.Interfaces.dll"))
                        {
                            if (!string.IsNullOrEmpty(strGatewayListDisplay))
                                strGatewayListDisplay += ",";
                            string strPaymentGatewayName = Strings.Replace(Strings.Mid(File.ToString(), File.LastIndexOf(@"\") + 2), ".dll", "");
                            Kartris.Interfaces.PaymentGateway GatewayPlugin = Payment.PPLoader(strPaymentGatewayName);
                            if (GatewayPlugin.Status.ToLower != "off")
                            {
                                if (!string.IsNullOrEmpty(strGatewayListConfig))
                                    strGatewayListConfig += ",";
                                strGatewayListConfig += strPaymentGatewayName + "::" + GatewayPlugin.Status.ToLower + "::" + GatewayPlugin.AuthorizedOnly.ToLower + "::" + Payment.isAnonymousCheckoutEnabled(strPaymentGatewayName) + "::p";
                            }
                            strGatewayListDisplay += strPaymentGatewayName + "::" + GatewayPlugin.Status.ToLower + "::" + GatewayPlugin.AuthorizedOnly.ToLower + "::" + Payment.isAnonymousCheckoutEnabled(strPaymentGatewayName) + "::p";
                            GatewayPlugin = null/* TODO Change to default(_) if this is not a reference type */;
                        }
                        else if (IsValidShippingGatewayPlugin(File.ToString()) & !Strings.InStr(File.ToString(), "Kartris.Interfaces.dll"))
                        {
                            if (!string.IsNullOrEmpty(strGatewayListDisplay))
                                strGatewayListDisplay += ",";
                            string strShippingGatewayName = Strings.Replace(Strings.Mid(File.ToString(), File.LastIndexOf(@"\") + 2), ".dll", "");
                            Kartris.Interfaces.ShippingGateway GatewayPlugin = Payment.SPLoader(strShippingGatewayName);
                            if (GatewayPlugin.Status.ToLower != "off")
                            {
                                if (!string.IsNullOrEmpty(strGatewayListConfig))
                                    strGatewayListConfig += ",";
                                strGatewayListConfig += strShippingGatewayName + "::" + GatewayPlugin.Status.ToLower + "::n::false::s";
                            }
                            strGatewayListDisplay += strShippingGatewayName + "::" + GatewayPlugin.Status.ToLower + "::n::false::s";
                            GatewayPlugin = null/* TODO Change to default(_) if this is not a reference type */;
                        }
                    }
                    KartSettingsManager.SetKartConfig("frontend.payment.gatewayslist", strGatewayListConfig);

                    (Skins_Admin_Template)this.Master.DataUpdated();
                }
                catch (Exception ex)
                {
                    // Probably permissions error trying to save config
                    _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgGeneral"));
                    // Throw New Exception("Error saving a payment gateway config file, most likely due to permissions.")
                    ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
                }
            }
        }
    }
    /// <summary>
    ///     ''' this function retrieves the config file for specific gateway
    ///     ''' </summary>
    ///     ''' <param name="GatewayName"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    private ClientSettingsSection GetGatewaySettings(string GatewayName)
    {
        ;/* Cannot convert OnErrorResumeNextStatementSyntax, CONVERSION ERROR: Conversion for OnErrorResumeNextStatement not implemented, please report this issue in 'On Error Resume Next' at character 24451
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitOnErrorResumeNextStatement(OnErrorResumeNextStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorResumeNextStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 
        On Error Resume Next

 */
        ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
        configFileMap.ExeConfigFilename = Path.Combine(Request.PhysicalApplicationPath, @"Plugins\" + GatewayName + @"\" + GatewayName + ".dll.config");
        configFileMap.MachineConfigFilename = Path.Combine(Request.PhysicalApplicationPath, @"Uploads\resources\Machine.Config");
        System.Configuration objConfiguration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        ConfigurationSectionGroup grpSection = objConfiguration.GetSectionGroup("applicationSettings");
        ClientSettingsSection appSettingsSection = (ClientSettingsSection)grpSection.Sections.Item("Kartris.My.MySettings");
        ;/* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToZeroStatement not implemented, please report this issue in 'On Error GoTo 0' at character 25210
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitOnErrorGoToStatement(OnErrorGoToStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorGoToStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 
        On Error GoTo 0

 */
        if (appSettingsSection != null)
            return appSettingsSection;
        else
            return null/* TODO Change to default(_) if this is not a reference type */;
    }
    /// <summary>
    ///     ''' ''' check if the dll being loaded is a valid payment gateway plugin
    ///     ''' </summary>
    ///     ''' <param name="GateWayPath"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    private bool IsValidGatewayPlugin(string GateWayPath)
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
    /// <summary>
    ///     ''' check if the dll being loaded is a valid shipping gateway plugin
    ///     ''' </summary>
    ///     ''' <param name="GateWayPath"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
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
