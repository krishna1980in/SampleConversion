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
using System.Threading;
using System.Web.Configuration;
using Microsoft.SqlServer.Server;

partial class Admin_Install : System.Web.UI.Page
{

    // ---------------------------------------
    // SET CONSTANTS AND VARIABLES
    // ---------------------------------------
    // Private Shared strResLangs As String() = {"en", "ar", "de", "es", "fr", "ja"}
    private static string[] strResLangs = new[] { "en" };
    private static string strConnectionString = "";
    private static string strUploadsPath = "";
    private static bool blnPermissionsOK = false;
    private static bool blnConfigDownloadedOnce = false;
    private static Configuration ModifiedConfig = null/* TODO Change to default(_) if this is not a reference type */;
    private static bool blnConfigUpdatable = false;
    private static SqlConnection objSQLConnection = null/* TODO Change to default(_) if this is not a reference type */;
    private static bool blnConfigControlsCreated = false;
    private static string strCreatedDBName = "";

    // ---------------------------------------
    // CHECK LANGUAGE
    // ---------------------------------------
    protected override void InitializeCulture()
    {
        // override virtual method InitializeCulture() to check if profile contains a user language setting
        if (System.Web.UI.Page.Session["CultureName"] != null)
        {
            string UserCulture = System.Web.UI.Page.Session["CultureName"];
            if (UserCulture != "")
            {
                // there is a user language setting in the profile: switch to it
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(UserCulture);
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(UserCulture);
            }
        }
    } // InitializeCulture

    // ---------------------------------------
    // LANGUAGE CHANGED
    // ---------------------------------------
    protected void ddlLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
        string SelectedLanguage = ddlLanguage.SelectedValue.ToString();
        // Save selected user language in profile 
        System.Web.UI.Page.Session["CultureName"] = SelectedLanguage;

        // Force re-initialization of the page to fire InitializeCulture()
        System.Web.UI.Page.Response.Redirect(System.Web.UI.Page.Request.Url.LocalPath);
    }

    // Protected Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    // Response.Write("Globalization Error")
    // End Sub

    // ---------------------------------------
    // PRE-INIT
    // Looks up important config settings once
    // db exists to allow them to be changed.
    // ---------------------------------------
    protected void Page_PreInit(object sender, System.EventArgs e)
    {
        if (objSQLConnection != null)
        {
            try
            {
                SqlCommand objSQLCommand = new SqlCommand();
                {
                    var withBlock = objSQLCommand;
                    withBlock.CommandType = CommandType.Text;
                    withBlock.CommandText = "SELECT * FROM tblKartrisConfig WHERE CFG_Important = 1";
                    withBlock.Connection = objSQLConnection;
                    withBlock.Connection.Open();
                    SqlDataReader drwConfigSettings = withBlock.ExecuteReader;
                    while (drwConfigSettings.Read)
                    {
                        phdConfigSettings.Controls.Add(GetLiteral("<p>"));
                        string strConfigDisplayType = drwConfigSettings("CFG_DisplayType").ToString;
                        switch (strConfigDisplayType)
                        {
                            case "b" // Boolean
                           :
                                {
                                    phdConfigSettings.Controls.Add(GetLiteral("<span class=\"checkbox2\">"));
                                    CheckBox chkConfig = new CheckBox();
                                    chkConfig.ID = "CFG_CHK_" + drwConfigSettings("CFG_Name");
                                    chkConfig.Text = drwConfigSettings("CFG_Name");
                                    chkConfig.EnableViewState = true;
                                    chkConfig.Checked = (drwConfigSettings("CFG_Value") == "y");
                                    phdConfigSettings.Controls.Add(chkConfig);
                                    phdConfigSettings.Controls.Add(GetLiteral("</span><br />"));
                                    break;
                                }

                            case "t" // Text
                     :
                                {
                                    string strConfigName = drwConfigSettings("CFG_Name");

                                    Label lblConfigName = new Label();
                                    lblConfigName.Text = strConfigName + " ";
                                    lblConfigName.Font.Bold = true;
                                    phdConfigSettings.Controls.Add(lblConfigName);
                                    TextBox txtConfig = new TextBox();
                                    txtConfig.ID = "CFG_TXT_" + strConfigName;
                                    txtConfig.EnableViewState = true;
                                    if (string.IsNullOrEmpty(txtConfig.Text))
                                    {
                                        if (strConfigName.ToLower() == "general.webshopurl")
                                            txtConfig.Text = HttpContext.Current.Request.Url.AbsoluteUri.ToLower.Replace("admin/install.aspx", "");
                                        else if (strConfigName.ToLower() == "general.webshopfolder")
                                        {
                                            var strDetectedWebShopFolder = Strings.Trim(System.Web.UI.Page.Request.ApplicationPath);
                                            if (!string.IsNullOrEmpty(strDetectedWebShopFolder))
                                            {
                                                if (strDetectedWebShopFolder == "/")
                                                    strDetectedWebShopFolder = "";
                                                else if (Strings.Left(strDetectedWebShopFolder, 1) == "/")
                                                    strDetectedWebShopFolder = Strings.Mid(strDetectedWebShopFolder, 2);
                                                if (strDetectedWebShopFolder != "")
                                                {
                                                    if (!(strDetectedWebShopFolder.LastIndexOf("/") + 1 == strDetectedWebShopFolder.Length))
                                                        strDetectedWebShopFolder += "/";
                                                }
                                            }
                                            else
                                                strDetectedWebShopFolder = "";

                                            txtConfig.Text = strDetectedWebShopFolder;
                                        }
                                        else
                                            txtConfig.Text = drwConfigSettings("CFG_Value");
                                    }

                                    phdConfigSettings.Controls.Add(txtConfig);

                                    if (strConfigName.ToLower() != "general.webshopfolder")
                                    {
                                        RequiredFieldValidator valConfig = new RequiredFieldValidator();
                                        valConfig.ControlToValidate = txtConfig.ID;
                                        valConfig.Text = "*";
                                        valConfig.ErrorMessage = strConfigName + " is required!";
                                        phdConfigSettings.Controls.Add(valConfig);
                                    }

                                    phdConfigSettings.Controls.Add(GetLiteral("<br />"));
                                    break;
                                }
                        }

                        Label lblConfigDescription = new Label();
                        lblConfigDescription.Text = drwConfigSettings("CFG_Description");
                        phdConfigSettings.Controls.Add(lblConfigDescription);
                        phdConfigSettings.Controls.Add(GetLiteral("<br />"));
                        phdConfigSettings.Controls.Add(GetLiteral("</p>"));
                    }
                }
                blnConfigControlsCreated = true;
            }
            catch (Exception ex)
            {
                litError.Text = ex.Message;
                phdError.Visible = true;
            }
            finally
            {
                if (objSQLConnection.State == ConnectionState.Open)
                    objSQLConnection.Close();
            }
        }
    }

    // ---------------------------------------
    // PAGE LOAD
    // Checks if install routine should run or
    // not, depending on if globalization tag
    // in web.config is commented, and SQL
    // db cannot be found.
    // ---------------------------------------
    protected void Page_Load(object sender, System.EventArgs e)
    {
        phdError.Visible = false;
        litError.Text = "";

        if ((!System.Web.UI.Page.IsPostBack) & wizInstallation.ActiveStepIndex == 0)
        {
            txtHashKey.Text = Guid.NewGuid().ToString();

            // Check if this shop is already set up
            if (System.Web.UI.Page.Application["DBConnected"])
            {
                SqlConnection conKartris = new SqlConnection(ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString);
                SqlCommand comCheck = new SqlCommand("SELECT COUNT(*) as LoginCount FROM tblKartrisLogins", conKartris);
                conKartris.Open();
                if (comCheck.ExecuteScalar() > 0)
                {
                    // There's an existing Login record! This shop is already set up.
                    Page_Load_Error(System.Web.UI.TemplateControl.GetLocalResourceObject("Error_KartrisAlreadyInstalled"));
                    return;
                }
            }
            string webConfigFile = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "web.config");
            // Also on first load, check if the web.config is updatable
            try
            {
                bool result = false;
                try
                {
                    if (ModifiedConfig == null)
                    {
                        ModifiedConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(System.Web.UI.Page.Request.ApplicationPath);
                        System.Web.Configuration.GlobalizationSection configSection = (System.Web.Configuration.GlobalizationSection)ModifiedConfig.GetSection("system.web/globalization");
                        if (configSection.ResourceProviderFactoryType == "SqlResourceProviderFactory")
                            result = true;
                    }
                }
                catch (Exception ex)
                {
                    // Hmmm....Kartris must be running under medium trust
                    ModifiedConfig = null;
                    XmlTextReader webConfigReader = new XmlTextReader(new StreamReader(webConfigFile));
                    result = ((webConfigReader.ReadToFollowing("globalization")) && (webConfigReader.GetAttribute("resourceProviderFactoryType") == "SqlResourceProviderFactory"));
                    webConfigReader.Close();
                }
            }
            catch (Exception ex)
            {
                Page_Load_Error(System.Web.UI.TemplateControl.GetLocalResourceObject("Error_OpeningWebConfig") + " --- " + ex.Message);
                return;
            }

            try
            {
                File.Copy(webConfigFile, System.Web.UI.Page.Request.PhysicalApplicationPath + "test.txt");
                blnConfigUpdatable = true;
                phdNote.Visible = false;
            }
            catch (Exception ex)
            {
                blnConfigUpdatable = false;
                phdNote.Visible = true;
            }

            if (blnConfigUpdatable)
            {
                try
                {
                    File.Delete(System.Web.UI.Page.Request.PhysicalApplicationPath + "test.txt");
                }
                catch (Exception ex)
                {
                }
            }

            if (System.Convert.ToString(System.Web.UI.Page.Session["CultureName"]) != CultureInfo.CurrentCulture.Name)
            {
                // Populate the language dropdown with the list of available languages on the server
                ddlLanguage.Items.Clear(); // Clears the dropdown in case of a culture change
                string ResLanguage;
                foreach (var ResLanguage in strResLangs)
                {
                    CultureInfo TempCultureInfo = new CultureInfo(ResLanguage);
                    ListItem ResourceLanguage = new ListItem(TempCultureInfo.NativeName, TempCultureInfo.Name);
                    if (TempCultureInfo.Equals(CultureInfo.CurrentUICulture))
                        ResourceLanguage.Selected = true;
                    ddlLanguage.Items.Add(ResourceLanguage);
                }

                System.Web.UI.Page.Session["CultureName"] = CultureInfo.CurrentCulture.Name;
            }
        }
    }

    // ---------------------------------------
    // HANDLES PAGE LOAD ERRORS
    // For example, if globalization tag not
    // commented (so run install) but db
    // already exists.
    // ---------------------------------------
    private void Page_Load_Error(string strErrorText)
    {
        wizInstallation.Visible = false;
        phdError.Visible = true;
        litError.Text = strErrorText;
        phdNote.Visible = false;
    }

    // ---------------------------------------
    // 1. WELCOM TO KARTRIS
    // ---------------------------------------
    protected void ws1_Welcome_Load(object sender, System.EventArgs e)
    {
    }

    // ---------------------------------------
    // 2. HASH KEY CHECK
    // ---------------------------------------
    protected void ws2_HashandMachineKey_Load(object sender, System.EventArgs e)
    {
        string strHashKey = ConfigurationManager.AppSettings("hashsalt");
        if (!(string.IsNullOrEmpty(strHashKey) | strHashKey == "PutSomeRandomTextHere"))
        {
            litHashDesc.Text = System.Web.UI.TemplateControl.GetLocalResourceObject("Step2_YourHashKeyAlreadySetTo") + "\"<strong>" + System.Web.UI.Page.Server.HtmlEncode(strHashKey) + "</strong>\"" + System.Web.UI.TemplateControl.GetLocalResourceObject("Step2_YourHashKeyAlreadySetTo_2");
            // If String.IsNullOrEmpty(txtHashKey.Text) Then

            // End If
            txtHashKey.Text = strHashKey;
        }
    }

    // ---------------------------------------
    // 3. DB CONNECTION SETUP
    // ---------------------------------------
    protected void ws3_ConnectionString_Load(object sender, System.EventArgs e)
    {

        // check if we can connect to the database
        strConnectionString = ConfigurationManager.ConnectionStrings("kartrisSQLConnection").ToString;
        SqlConnection sqlconKartris = new SqlConnection(strConnectionString);
        try
        {
            sqlconKartris.Open();
            System.Web.UI.Page.Application["DBConnected"] = true;
        }
        catch (Exception ex)
        {
            System.Web.UI.Page.Application["DBConnected"] = false;
            strConnectionString = string.Empty;
        }
        finally
        {
            if (sqlconKartris.State == ConnectionState.Open)
                sqlconKartris.Close();
        }


        if (string.IsNullOrEmpty(strConnectionString))
        {
            System.Web.UI.Page.Application["DBConnected"] = false;
            phdRetryCheckButton.Visible = false;
            // Hide connection info
            phdConnectionChecking.Visible = false;
            litConnectionString.Text = "";
            btnUseSavedConnection.Visible = false;
        }
        else
        {
            // Show connection info
            phdConnectionChecking.Visible = true;
            litConnectionString.Text = strConnectionString;
            if (CheckConnectionDetails(strConnectionString))
            {
                litConnectionString.Text += " --> " + System.Web.UI.TemplateControl.GetLocalResourceObject("Step3_SuccessText");
                btnUseSavedConnection.Visible = true;
            }
        }

        if (System.Web.UI.Page.Application["DBConnected"])
        {
            mvwConnectionString.ActiveViewIndex = 0;
            phdRetryCheckButton.Visible = false;
        }
        else
            mvwConnectionString.ActiveViewIndex = 1;
    }

    // ---------------------------------------
    // 4. SETUP DATABASE STRUCTURE AND CONTENT
    // Only if does not already exist.
    // ---------------------------------------
    protected void ws4_SetUpDatabase_Load(object sender, System.EventArgs e)
    {
        // STEP 4 - SET UP DATABASE
        string strConnection;
        litBackendPassword.Text = "";
        if (!string.IsNullOrEmpty(strConnectionString))
            strConnection = strConnectionString;
        else
            strConnection = ConfigurationManager.ConnectionStrings("kartrisSQLConnection").ToString;
        objSQLConnection = new SqlConnection(strConnection);
        SqlCommand objSQLCommand = new SqlCommand();
        try
        {
            objSQLCommand = new SqlCommand("SELECT TOP 1 P_ID FROM tblKartrisProducts", objSQLConnection);
            objSQLConnection.Open();
            objSQLCommand.ExecuteNonQuery();
            mvwSetUpDatabase.ActiveViewIndex = 0;
        }
        catch (Exception ex)
        {
            mvwSetUpDatabase.ActiveViewIndex = 1;
            try
            {
                string strSQLPath = System.Web.UI.Page.Server.MapPath("~/Uploads/resources/kartrisSQL_MainData.sql");
                string strError = "";
                if (File.Exists(strSQLPath))
                {
                    ExecuteSQLScript(strSQLPath, objSQLConnection, ref strError);
                    if (!string.IsNullOrEmpty(strError))
                        throw new Exception(strError);
                }
            }
            catch (Exception SQLex)
            {
                litError.Text = " Database Schema Creation Failed - " + SQLex.Message;
                phdError.Visible = true;
                wizInstallation.ActiveStepIndex = 2;
            }
        }

        try
        {
            if (chkCreateSampleData.Checked)
            {
                string strSQLPath = System.Web.UI.Page.Server.MapPath("~/Uploads/resources/kartrisSQL_SampleData.sql");
                string strError = "";
                if (File.Exists(strSQLPath))
                {
                    ExecuteSQLScript(strSQLPath, objSQLConnection, ref strError);
                    if (!string.IsNullOrEmpty(strError))
                        throw new Exception(strError);
                }
            }
        }
        catch (Exception ex)
        {
        }
        // litError.Text = " Database Sample Creation Failed - " & ex.Message
        // phdError.Visible = True
        // wizInstallation.ActiveStepIndex = 2
        finally
        {
            if (objSQLConnection.State == ConnectionState.Open)
                objSQLConnection.Close();
        }

        if (!string.IsNullOrEmpty(txtHashKey.Text))
        {
            try
            {
                string strNewRandomSalt = Membership.GeneratePassword(20, 0);
                litBackendPassword.Text = Trim(Membership.GeneratePassword(8, 0));
                UnicodeEncoding uEncode = new UnicodeEncoding();
                byte[] bytClearString = uEncode.GetBytes(txtHashKey.Text + litBackendPassword.Text + strNewRandomSalt);
                System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed();
                byte[] hash = sha.ComputeHash(bytClearString);
                string strEncryptedPass = Convert.ToBase64String(hash);
                objSQLCommand = new SqlCommand("DELETE FROM tblKartrisLogins WHERE LOGIN_Username = 'Admin';" + "INSERT INTO tblKartrisLogins VALUES ('Admin','" + strEncryptedPass + "',1,1,1,1,1,1,'',1,'" + strNewRandomSalt + "',NULL);", objSQLConnection);
                objSQLConnection.Open();
                objSQLCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                wizInstallation.ActiveStepIndex = 2;
            }
            finally
            {
                if (objSQLConnection.State == ConnectionState.Open)
                    objSQLConnection.Close();
            }
        }
        else
            wizInstallation.ActiveStepIndex = 1;
    }


    // ---------------------------------------
    // 5. CHECK/CHANGE IMPORTANT CONFIG / SET DEFAULT CURRENCY
    // ---------------------------------------
    protected void ws5_ConfigSettings_Deactivate(object sender, EventArgs e)
    {
        string strConnection;
        if (!string.IsNullOrEmpty(strConnectionString))
            strConnection = strConnectionString;
        else
            strConnection = ConfigurationManager.ConnectionStrings("kartrisSQLConnection").ToString;
        objSQLConnection = new SqlConnection(strConnection);

        try
        {
            if (ddlDefaultCurrency.SelectedIndex > 0)
            {
                SqlCommand objSQLCommand = new SqlCommand("UPDATE tblKartrisCurrencies " + "SET CUR_ExchangeRate = 1, CUR_OrderNo = 1, CUR_Live = 1 " + "Where CUR_ID = " + ddlDefaultCurrency.SelectedValue + ";", objSQLConnection);
                objSQLConnection.Open();
                objSQLCommand.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            litError.Text = " Setting Default Currency Failed - " + ex.Message;
            phdError.Visible = true;
            wizInstallation.ActiveStepIndex = 4;
        }
        finally
        {
            if (objSQLConnection.State == ConnectionState.Open)
                objSQLConnection.Close();
        }
    }

    // ---------------------------------------
    // 6. FOLDER PERMISSIONS
    // For image uploads, etc.
    // ---------------------------------------
    protected void ws6_FolderPermissions_Load(object sender, System.EventArgs e)
    {
        TestFoldersPermissions();
    }

    // ---------------------------------------
    // 7. REVIEW SETTINGS
    // For image uploads, etc.
    // ---------------------------------------
    protected void ws7_ReviewSettings_Load(object sender, System.EventArgs e)
    {
        string strTaxRegime = ddlTaxRegime.SelectedValue;

        if (string.IsNullOrEmpty(strConnectionString))
            phdConnectionString.Visible = false;
        else
        {
            phdConnectionString.Visible = true;
            litReviewConnectionString.Text = strConnectionString;
        }

        if (Trim(txtHashKey.Text) != Trim(ConfigurationManager.AppSettings("hashsalt")))
        {
            phdHashSaltKey.Visible = true;
            litReviewHashSaltKey.Text = txtHashKey.Text;
        }
        else
            phdHashSaltKey.Visible = false;

        if (strTaxRegime != "")
        {
            phdTaxRegime.Visible = true;
            litReviewTaxRegime.Text = strTaxRegime;
        }

        try
        {
            XmlDocument doc = new XmlDocument();
            if (ModifiedConfig != null)
            {
                System.Configuration.ConfigurationSection section = ModifiedConfig.GetSection("appSettings");

                KeyValueConfigurationElement element = (KeyValueConfigurationElement)ModifiedConfig.AppSettings.Settings("hashsalt");
                if (element != null & !string.IsNullOrEmpty(txtHashKey.Text) & phdHashSaltKey.Visible)
                    element.Value = txtHashKey.Text;

                element = (KeyValueConfigurationElement)ModifiedConfig.AppSettings.Settings("TaxRegime");
                if (element != null & ddlTaxRegime.SelectedValue != "Select One")
                {
                    if (strTaxRegime == "European Union")
                        strTaxRegime = "EU";
                    if (strTaxRegime == "VAT (non EU)")
                        strTaxRegime = "VAT";
                    if (strTaxRegime == "Other")
                        strTaxRegime = "SIMPLE";
                    element.Value = strTaxRegime;
                }


                System.Web.Configuration.CustomErrorsSection ErrorSection = (System.Web.Configuration.CustomErrorsSection)ModifiedConfig.GetSection("system.web/customErrors");
                ErrorSection.Mode = CustomErrorsMode.RemoteOnly;
                ErrorSection.DefaultRedirect = "Error.aspx";

                System.Web.Configuration.GlobalizationSection configSection = (System.Web.Configuration.GlobalizationSection)ModifiedConfig.GetSection("system.web/globalization");
                configSection.ResourceProviderFactoryType = "SqlResourceProviderFactory";

                if (phdConnectionString.Visible)
                    ModifiedConfig.ConnectionStrings.ConnectionStrings("KartrisSQLConnection").ConnectionString = strConnectionString;
            }

            if (phdConnectionString.Visible | phdHashSaltKey.Visible | phdTaxRegime.Visible)
            {
                if (blnConfigUpdatable)
                    btnSaveCopy.Visible = false;
                else
                {
                    litReviewSettingsDesc.Text = System.Web.UI.TemplateControl.GetLocalResourceObject("Step7_CannotUpdateWebConfigText");
                    btnSaveCopy.Visible = true;
                }
            }
            else
            {
                litReviewSettingsDesc.Text = System.Web.UI.TemplateControl.GetLocalResourceObject("Step7_NoChangesToWebConfigText");
                btnSaveCopy.Visible = false;
            }
        }

        catch (Exception ex)
        {
            wizInstallation.ActiveStepIndex = 5;
            phdError.Visible = true;
            litError.Text = System.Web.UI.TemplateControl.GetLocalResourceObject("Error_OpeningWebConfig") + " --- " + ex.Message;
        }
    }

    // ---------------------------------------
    // 8. SETUP COMPLETE!
    // No code needed here, but there is some
    // on the aspx page.
    // ---------------------------------------

    protected void wizInstallation_NextButtonClick(object sender, System.Web.UI.WebControls.WizardNavigationEventArgs e)
    {
        btnSaveCopy.Visible = false;

        switch (e.NextStepIndex)
        {
            case 2:
                {
                    string strHashKey = ConfigurationManager.AppSettings("hashsalt");
                    if ((string.IsNullOrEmpty(strHashKey) | strHashKey == "PutSomeRandomTextHere"))
                    {
                        if (string.IsNullOrEmpty(txtHashKey.Text))
                        {
                            phdError.Visible = true;
                            litError.Text = System.Web.UI.TemplateControl.GetLocalResourceObject("Step2_Error_HashKeyRequiredText");
                            e.Cancel = true;
                        }
                        else
                            phdError.Visible = false;
                    }

                    break;
                }

            case 3:
                {
                    System.Web.UI.Control.Page.Validate();
                    if (System.Web.UI.Control.Page.IsValid)
                    {
                        if (!System.Web.UI.Page.Application["DBConnected"])
                        {
                            strConnectionString = "Data Source=" + txtServerName.Text;
                            if (chkUseWindowsAuthentication.Checked)
                                strConnectionString += ";Integrated Security=True";
                            else
                                strConnectionString += ";User ID=" + txtUsername.Text + ";Password=" + txtPassword.Text;

                            strConnectionString += ";Initial Catalog=" + txtDatabaseName.Text;

                            // ' ======== Added By Mohammad =========='
                            if (rbnCreateDB.Checked)
                            {
                                string strError = null;
                                if (!CreateNewDB(strConnectionString, txtDatabaseName.Text, ref strError))
                                {
                                    litError.Text = strError;
                                    phdError.Visible = true;
                                    btnRetryCheck.Visible = true;
                                    e.Cancel = true;
                                    break;
                                }
                            }
                            litError.Text = "";
                            phdError.Visible = false;
                            btnRetryCheck.Visible = false;
                            // ' ==================================='
                            if (CheckConnectionDetails(strConnectionString))
                                phdError.Visible = false;
                            else
                            {
                                e.Cancel = true;
                                btnRetryCheck.Visible = true;
                            }
                        }
                    }

                    break;
                }

            case 5:
                {
                    System.Web.UI.Control.Page.Validate();
                    if (System.Web.UI.Control.Page.IsValid)
                    {
                        SqlCommand objSQLCommand = new SqlCommand();
                        foreach (Control Ctrl in phdConfigSettings.Controls)
                        {
                            try
                            {
                                if (InStr(Ctrl.ID, "CFG_"))
                                {
                                    string[] arrConfig = Split(Ctrl.ID, "_");
                                    if (Information.UBound(arrConfig) == 2)
                                    {
                                        string strConfigName = arrConfig[2];
                                        string strConfigValue = "";
                                        switch (arrConfig[1])
                                        {
                                            case "TXT":
                                                {
                                                    strConfigValue = (TextBox)Ctrl.Text;
                                                    break;
                                                }

                                            case "CHK":
                                                {
                                                    if ((CheckBox)Ctrl.Checked)
                                                        strConfigValue = "y";
                                                    else
                                                        strConfigValue = "n";
                                                    break;
                                                }
                                        }

                                        objSQLCommand = new SqlCommand("_spKartrisConfig_UpdateConfigValue", objSQLConnection);
                                        SqlParameter paramConfig;
                                        {
                                            var withBlock = objSQLCommand;
                                            withBlock.CommandType = CommandType.StoredProcedure;
                                            paramConfig = new SqlParameter("@CFG_Name", strConfigName);
                                            withBlock.Parameters.Add(paramConfig);
                                            if (string.IsNullOrEmpty(strConfigValue))
                                                strConfigValue = "";
                                            if (strConfigName.ToLower() == "general.webshopurl")
                                            {
                                                if (!(strConfigValue.LastIndexOf("/") + 1 == strConfigValue.Length))
                                                    strConfigValue += "/";
                                            }
                                            paramConfig = new SqlParameter("@CFG_Value", strConfigValue);
                                            withBlock.Parameters.Add(paramConfig);
                                            withBlock.Connection.Open();
                                            withBlock.ExecuteNonQuery();
                                            withBlock.Connection.Close();
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                phdError.Visible = true;
                                litError.Text = System.Web.UI.TemplateControl.GetLocalResourceObject("Step5_Error_UpdatingConfigSettings") + " --- " + ex.Message;
                            }
                        }
                    }
                    else
                        e.Cancel = true;
                    break;
                }

            case 6:
                {
                    break;
                }
        }
    }

    protected void wizInstallation_PreviousButtonClick(object sender, System.Web.UI.WebControls.WizardNavigationEventArgs e)
    {
        if (e.CurrentStepIndex == 6)
            blnConfigDownloadedOnce = false;
    }

    protected void wizInstallation_FinishButtonClick(object sender, System.Web.UI.WebControls.WizardNavigationEventArgs e)
    {
        if ((phdHashSaltKey.Visible | phdConnectionString.Visible) & (!blnConfigUpdatable))
        {
            if (!blnConfigDownloadedOnce)
            {
                litError.Text = System.Web.UI.TemplateControl.GetLocalResourceObject("Step6_Error_MustDownloadWebConfigText");
                btnSaveCopy.Visible = true;
                phdError.Visible = true;
                e.Cancel = true;
            }
            else
            {
                btnSaveCopy.Visible = false;
                phdNote.Visible = false;
            }
        }
        else
            try
            {
                if (ModifiedConfig != null)
                    ModifiedConfig.Save();

                System.Web.UI.Page.Application["DBConnected"] = true;
                litReminder.Visible = false;
            }
            catch (Exception ex)
            {
                if (ModifiedConfig != null)
                    ModifiedConfig = null;
                blnConfigUpdatable = false;
                litError.Text = System.Web.UI.TemplateControl.GetLocalResourceObject("Step6_Error_MustDownloadWebConfigText");
                phdError.Visible = true;
                btnSaveCopy.Visible = true;
                phdNote.Visible = true;
                litReminder.Visible = true;
                e.Cancel = true;
            }
    }


    /// <summary>
    ///     ''' Create database on the specified server
    ///     ''' </summary>
    ///     ''' <param name="strConnString"></param>
    ///     ''' <param name="strDBName"></param>
    ///     ''' <param name="strError"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks>By Mohammad</remarks>
    public bool CreateNewDB(string strConnString, string strDBName, ref string strError)
    {
        if (strCreatedDBName == strDBName)
            return true; // ' skip creation
        strConnString = strConnString.Replace("Initial Catalog=" + strDBName, "Initial Catalog=master");
        SqlClient.SqlConnection conSQL = new SqlClient.SqlConnection(strConnString);
        SqlClient.SqlCommand cmdSQL = new SqlClient.SqlCommand();
        try
        {
            cmdSQL.CommandType = CommandType.Text;
            cmdSQL.Connection = conSQL;
            conSQL.Open();
            cmdSQL.CommandText = "CREATE DATABASE [" + strDBName + "] COLLATE SQL_Latin1_General_CP1_CI_AS";
            cmdSQL.ExecuteNonQuery();
            strCreatedDBName = strDBName;
        }
        catch (Exception ex)
        {
            strError = ex.Message;
            return false;
        }
        finally
        {
            conSQL.Close();
        }
        CreateDBUsers(strConnString, strDBName);
        return true;
    }

    /// <summary>
    ///     ''' Create users with permissions on the specified database
    ///     ''' </summary>
    ///     ''' <param name="strConnString"></param>
    ///     ''' <param name="strDBName"></param>
    ///     ''' <remarks>By Mohammad</remarks>
    public void CreateDBUsers(string strConnString, string strDBName)
    {
        // strConnString = strConnString.Replace("Database=master", "Database=" & strDBName)
        strConnString = strConnString.Replace("Initial Catalog=" + strDBName, "Initial Catalog=master");
        SqlClient.SqlConnection conSQL = new SqlClient.SqlConnection(strConnString);
        SqlClient.SqlCommand cmdSQL = new SqlClient.SqlCommand();

        try
        {
            cmdSQL.CommandType = CommandType.Text;
            cmdSQL.Connection = conSQL;
            conSQL.Open();
            cmdSQL.CommandText = @"CREATE USER [NT_KartrisUser] FOR LOGIN [NT AUTHORITY\NETWORK SERVICE] WITH DEFAULT_SCHEMA=[dbo]";
            cmdSQL.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
        }
        finally
        {
            conSQL.Close();
        }

        try
        {
            string strSvr = txtServerName.Text;
            if (strSvr.Contains(@"\"))
                strSvr = Strings.Split(strSvr, @"\")(0);
            cmdSQL.CommandType = CommandType.Text;
            cmdSQL.Connection = conSQL;
            conSQL.Open();
            cmdSQL.CommandText = "CREATE USER [ASPNET_KartrisUser] FOR LOGIN [" + strSvr + @"\ASPNET] WITH DEFAULT_SCHEMA=[dbo]";
            cmdSQL.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
        }
        finally
        {
            conSQL.Close();
        }
    }

    /// <summary>
    ///     ''' Test a connection string
    ///     ''' </summary>
    ///     ''' <param name="strInputConnectionString"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks>uses the connection string setting in web.config if there's no parameter passed</remarks>
    private bool CheckConnectionDetails(string strInputConnectionString = "")
    {
        bool blnUsedWebConfig = false;
        string strConnection = "";
        if (string.IsNullOrEmpty(strInputConnectionString))
        {
            strConnection = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString;
            blnUsedWebConfig = true;
        }
        else
        {
            strConnectionString = strInputConnectionString;
            strConnection = strInputConnectionString;
            litConnectionString.Text = strInputConnectionString;
        }
        SqlConnection objSQLConnectionKartris = new SqlConnection(strConnection);
        try
        {
            objSQLConnectionKartris.Open();
            phdRetryCheckButton.Visible = false;
            CheckConnectionDetails = true;
            if (blnUsedWebConfig)
                System.Web.UI.Page.Application["DBConnected"] = true;
        }
        catch (Exception ex)
        {
            phdRetryCheckButton.Visible = true;
            CheckConnectionDetails = false;
            phdError.Visible = true;
            litError.Text = System.Web.UI.TemplateControl.GetLocalResourceObject("Error_ConnectionFailedText") + "<code>" + ex.Message + "</code>";
        }
        finally
        {
            if (objSQLConnectionKartris.State == ConnectionState.Open)
                objSQLConnectionKartris.Close();
        }
    }

    /// <summary>
    ///     ''' Execute SQL script - used by ws4_SetUPDatabase step to create the Kartris Database Structure
    ///     ''' </summary>
    ///     ''' <param name="Filename"></param>
    ///     ''' <param name="conn"></param>
    ///     ''' <param name="strError"></param>
    ///     ''' <remarks></remarks>
    private void ExecuteSQLScript(string Filename, SqlClient.SqlConnection conn, ref string strError = null)
    {
        SqlClient.SqlCommand objSQLCommand = new SqlClient.SqlCommand();
        System.IO.StreamReader Reader = new System.IO.StreamReader(Filename);
        char chrDelimiter = Strings.Chr(0);
        int I;
        SqlTransaction objTrans;

        objSQLCommand.CommandType = CommandType.Text;
        objSQLCommand.Connection = objSQLConnection;
        if (objSQLConnection.State == ConnectionState.Closed)
            objSQLConnection.Open();
        objTrans = objSQLConnection.BeginTransaction;
        objSQLCommand.Transaction = objTrans;

        try
        {
            string s = Reader.ReadToEnd();
            s = Strings.Replace(s, Constants.vbCrLf + "GO" + Constants.vbCrLf, chrDelimiter);
            string[] SQL = s.Split(chrDelimiter);

            for (I = 0; I <= Information.UBound(SQL); I++)
            {
                if (Strings.Trim(SQL[I]) != "")
                {
                    objSQLCommand.CommandText = SQL[I];
                    objSQLCommand.ExecuteNonQuery();
                }
            }

            Reader.Close();
            Reader = null;
        }
        catch (Exception ex)
        {
            objTrans.Rollback();
            Reader.Close();
            Reader = null;
            strError = ex.Message;
        }
        finally
        {
            if (objTrans.Connection != null)
                objTrans.Commit();
            objSQLConnection.Close();
        }
        objSQLCommand = null/* TODO Change to default(_) if this is not a reference type */;
    }

    /// <summary>
    ///     ''' GET KARTRIS CONFIG SETTING VALUE
    ///     ''' </summary>
    ///     ''' <param name="ConfigName"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    private string GetConfigValue(string ConfigName)
    {
        string strValue = "";
        if (!string.IsNullOrEmpty(ConfigName))
        {
            string strConnection;
            if (!string.IsNullOrEmpty(strConnectionString))
                strConnection = strConnectionString;
            else
                strConnection = ConfigurationManager.ConnectionStrings("kartrisSQLConnection").ToString;
            SqlConnection objSQLConnection = new SqlConnection(strConnection);
            try
            {
                SqlClient.SqlCommand objSQLCommand = new SqlClient.SqlCommand("SELECT CFG_Value FROM tblKartrisConfig WHERE CFG_Name = '" + System.Web.UI.Page.Server.HtmlEncode(ConfigName) + "'", objSQLConnection);
                objSQLConnection.Open();
                SqlDataReader reader = objSQLCommand.ExecuteReader;
                while (reader.Read())
                {
                    strValue = reader(0).ToString; break;
                }
            }
            catch (Exception ex)
            {
            }

            finally
            {
                if (objSQLConnection.State == ConnectionState.Open)
                    objSQLConnection.Close();
            }
        }
        return strValue;
    }

    /// <summary>
    ///     ''' Code that actually test the folder permissions - used by ws5_FolderPermissions_ON_LOAD and Retry Tests Button_CLICK
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private void TestFoldersPermissions()
    {
        bool blnFoldersAccessible = true;
        StreamWriter sw;
        litUploadsStatus.Text = System.Web.UI.TemplateControl.GetLocalResourceObject("Step3_FailedText");
        litImagesStatus.Text = System.Web.UI.TemplateControl.GetLocalResourceObject("Step3_FailedText");
        litPaymentStatus.Text = System.Web.UI.TemplateControl.GetLocalResourceObject("Step3_FailedText");
        litError.Text = "";
        try
        {
            string strUploadsFolder = GetConfigValue("general.uploadfolder");
            strUploadsPath = System.Web.UI.Page.Server.MapPath(strUploadsFolder);

            if (!string.IsNullOrEmpty(strUploadsPath))
            {
                sw = File.CreateText(strUploadsPath + "KartrisInstallTest.txt");
                sw.WriteLine("Kartris Uploads Folder Permissions Test");
                sw.Flush();
                sw.Close();
                File.Delete(strUploadsPath + "KartrisInstallTest.txt");

                litUploadsStatus.Text = System.Web.UI.TemplateControl.GetLocalResourceObject("Step3_SuccessText");
            }
        }
        catch (Exception ex)
        {
            blnFoldersAccessible = false;
            phdError.Visible = true;
            litError.Text = ex.Message;
        }
        try
        {
            sw = File.CreateText(System.Web.UI.Page.Server.MapPath("~/Images/") + "KartrisInstallTest.txt");
            sw.WriteLine("Kartris Images Folder Permissions Test");
            sw.Flush();
            sw.Close();
            File.Delete(System.Web.UI.Page.Server.MapPath("~/Images/") + "KartrisInstallTest.txt");
            litImagesStatus.Text = System.Web.UI.TemplateControl.GetLocalResourceObject("Step3_SuccessText");
        }
        catch (Exception ex)
        {
            blnFoldersAccessible = false;
            phdError.Visible = true;
            litError.Text += "<br/><br/>" + ex.Message;
        }
        try
        {
            sw = File.CreateText(System.Web.UI.Page.Server.MapPath("~/Plugins/") + "KartrisInstallTest.txt");
            sw.WriteLine("Kartris Payment Folder Permissions Test");
            sw.Flush();
            sw.Close();
            File.Delete(System.Web.UI.Page.Server.MapPath("~/Plugins/") + "KartrisInstallTest.txt");
            litPaymentStatus.Text = System.Web.UI.TemplateControl.GetLocalResourceObject("Step3_SuccessText");
        }
        catch (Exception ex)
        {
            blnFoldersAccessible = false;
            phdError.Visible = true;
            litError.Text += "<br/><br/>" + ex.Message;
        }
        if (blnFoldersAccessible)
            btnRetryTests.Visible = false;
        else
        {
            // Message to say that folder permission aren't properly set yet but setup will still allow user to proceed
            phdError.Visible = true;
            litError.Text += "<br/><br/>" + System.Web.UI.TemplateControl.GetLocalResourceObject("Step6_Error_PermissionsNotSetText");
        }

        blnPermissionsOK = blnFoldersAccessible;
    }

    private Literal GetLiteral(string text)
    {
        Literal rv;
        rv = new Literal();
        rv.Text = text;
        return rv;
    }

    /// <summary>
    ///     ''' (STEP 3 - Connection String) Use Windows Authentication checkbox -> Disable/Enable Username and Password fields
    ///     ''' </summary>
    ///     ''' <param name="sender"></param>
    ///     ''' <param name="e"></param>
    ///     ''' <remarks></remarks>
    protected void chkUseWindowsAuthentication_CheckedChanged(object sender, System.EventArgs e)
    {
        if (chkUseWindowsAuthentication.Checked == true)
        {
            txtUsername.Enabled = false;
            txtPassword.Enabled = false;
            txtUsername.Visible = false;
            txtPassword.Visible = false;
            Step3_Password.Visible = false;
            Step3_Username.Visible = false;
            valUsername.Enabled = false;
            valPassword.Enabled = false;
        }
        else
        {
            Step3_Password.Visible = true;
            Step3_Username.Visible = true;
            txtUsername.Visible = true;
            txtPassword.Visible = true;
            txtUsername.Enabled = true;
            txtPassword.Enabled = true;
            valUsername.Enabled = true;
            valPassword.Enabled = true;
            txtUsername.Focus();
        }
    }
    /// <summary>
    ///     ''' (STEP 3 - Connection String) RETRY CHECK BUTTON Click Event Handler
    ///     ''' </summary>
    ///     ''' <param name="sender"></param>
    ///     ''' <param name="e"></param>
    ///     ''' <remarks></remarks>
    protected void btnRetryCheck_Click(object sender, System.EventArgs e)
    {
        if (string.IsNullOrEmpty(strConnectionString))
            CheckConnectionDetails();
        else
            CheckConnectionDetails(strConnectionString);
    }
    /// <summary>
    ///     ''' (STEP 5 - Folder Permissions) RETRY TESTS BUTTON Click Event Handler
    ///     ''' ''' </summary>
    ///     ''' <param name="sender"></param>
    ///     ''' <param name="e"></param>
    ///     ''' <remarks></remarks>
    protected void btnRetryTests_Click(object sender, System.EventArgs e)
    {
        TestFoldersPermissions();
    }
    /// <summary>
    ///     ''' (STEP 6 - Review Settings) DOWNLOAD MODIFIED WEB.CONFIG BUTTON Click Event Handler 
    ///     ''' </summary>
    ///     ''' <param name="sender"></param>
    ///     ''' <param name="e"></param>
    ///     ''' <remarks></remarks>
    protected void btnSaveCopy_Click(object sender, System.EventArgs e)
    {
        try
        {
            XmlDocument docSave = new XmlDocument();
            docSave.Load(UpdateWebConfig);
            blnConfigDownloadedOnce = true;
            System.Web.UI.Page.Response.Clear();
            System.Web.UI.Page.Response.ContentType = "text/plain";
            System.Web.UI.Page.Response.AppendHeader("Content-Disposition", "attachment; filename=web.config");
            docSave.Save(System.Web.UI.Page.Response.OutputStream);
            System.Web.UI.Page.Response.End();
        }
        catch (Exception eEx)
        {
            litReason.Text = "<p>" + eEx.Message + "</p>";
        }
    }
    private XmlTextReader UpdateWebConfig()
    {
        string webConfigFile = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "web.config");
        XmlDocument docSave = new XmlDocument();
        docSave.Load(webConfigFile);
        if (phdConnectionString.Visible)
        {
            XmlNodeList _xmlConnectionNodeList = docSave.SelectNodes("/configuration/connectionStrings/add");
            if (_xmlConnectionNodeList.Count > 0)
            {
                foreach (XmlNode Node in _xmlConnectionNodeList)
                {
                    foreach (XmlAttribute attrib in Node.Attributes)
                    {
                        if (attrib.Name.ToLower == "name" & attrib.Value.ToLower == "kartrissqlconnection")
                        {
                            Node.Attributes("connectionString").Value = strConnectionString;
                            break; break;
                        }
                    }
                }
            }
        }

        XmlNodeList _xmlNodeList = docSave.SelectNodes("/configuration/appSettings/add");
        if (_xmlNodeList.Count > 0)
        {
            bool blnHashFound = !(!string.IsNullOrEmpty(txtHashKey.Text) & phdHashSaltKey.Visible);
            bool blnTaxRegimeSet = false;
            foreach (XmlNode Node in _xmlNodeList)
            {
                foreach (XmlAttribute attrib in Node.Attributes)
                {
                    if (attrib.Name.ToLower == "key")
                    {
                        if (!string.IsNullOrEmpty(txtHashKey.Text) & phdHashSaltKey.Visible & !blnHashFound)
                        {
                            if (attrib.Value.ToLower == "hashsalt")
                            {
                                Node.Attributes("value").Value = txtHashKey.Text;
                                blnHashFound = true;
                            }
                        }

                        if (attrib.Value.ToLower == "TaxRegime")
                        {
                            string strTaxRegime = ddlTaxRegime.SelectedValue;
                            if (strTaxRegime == "European Union")
                                strTaxRegime = "EU";
                            if (strTaxRegime == "Other")
                                strTaxRegime = "SIMPLE";
                            Node.Attributes("value").Value = strTaxRegime;
                            blnTaxRegimeSet = true;
                        }

                        break;
                    }
                }
                if (blnHashFound & blnTaxRegimeSet)
                    break;
            }
        }

        XmlNode _xmlCustomErrorNode = docSave.SelectSingleNode("/configuration/system.web/customErrors");
        if (_xmlCustomErrorNode != null)
        {
            _xmlCustomErrorNode.Attributes("mode").Value = "RemoteOnly";
            _xmlCustomErrorNode.Attributes("defaultRedirect").Value = "Error.aspx";
        }

        XmlNode xmlSystemWebnode = docSave.SelectSingleNode("/configuration/system.web");

        XmlElement elemWeb = docSave.CreateElement("globalization");

        XmlAttribute Resourceattrib = docSave.CreateAttribute("resourceProviderFactoryType");
        Resourceattrib.Value = "SqlResourceProviderFactory";
        elemWeb.Attributes.Append(Resourceattrib);

        xmlSystemWebnode.AppendChild(elemWeb);

        System.IO.StringReader sr = new System.IO.StringReader(docSave.OuterXml);
        XmlTextReader t = new XmlTextReader(sr);
        return t;
    }

    /// <summary>
    ///     ''' (Setp 3 - continue with exiting connection)
    ///     ''' </summary>
    ///     ''' <param name="sender"></param>
    ///     ''' <param name="e"></param>
    ///     ''' <remarks>by mohammad</remarks>
    protected void btnUseSavedConnection_Click(object sender, System.EventArgs e)
    {
        wizInstallation.ActiveStepIndex += 1;
    }
}
