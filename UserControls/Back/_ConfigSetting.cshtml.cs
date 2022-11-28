// ========================================================================
// Kartris - www.kartris.com
// Copyright 2016 CACTUSOFT

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
using CkartrisDataManipulation;

partial class _ConfigSetting : System.Web.UI.UserControl
{
    public event ShowMasterUpdateEventHandler ShowMasterUpdate;

    public delegate void ShowMasterUpdateEventHandler();

    protected void Page_Load(object sender, System.EventArgs e)
    {

        // Reset messages to blank
        phdMessageError.Visible = false;
        lstRadioButtons.Visible = false;

        if (!System.Web.UI.Control.Page.IsPostBack)
        {

            // Set the text of the 'details' link
            lnkDetails.Text = "[+] <span class=\"bold\">" + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_Details") + "</span>";

            CreateMenu();

            // Set number of records per page
            int intRowsPerPage = 25;
            try
            {
                intRowsPerPage = System.Convert.ToInt32(KartSettingsManager.GetKartConfig("backend.display.pagesize"));
            }
            catch (Exception ex)
            {
            }
            gvwConfig.PageSize = intRowsPerPage;

            // Not in expert mode
            // Disable 'new' config setting link
            if (KartSettingsManager.GetKartConfig("backend.expertmode") != "y")
                btnNew.Visible = false;
            if (System.Web.UI.UserControl.Request.QueryString["name"] != null)
            {
                txtSearchStarting.Text = System.Web.UI.UserControl.Request.QueryString["name"];
                SearchConfig(txtSearchStarting.Text);
            }
        }
    }

    public void CreateMenu()
    {
        if (HttpRuntime.Cache("KartSettingsCache") != null)
        {
            menConfig.Items.Clear();
            DataTable tblWebSettings = (DataTable)HttpRuntime.Cache("KartSettingsCache");
            foreach (DataRow drwWebSetting in tblWebSettings.Rows)
            {
                Array arrConfig = Split(drwWebSetting(0).ToString, ".");

                for (int numCounter = 0; numCounter <= Information.UBound(arrConfig); numCounter++)
                {
                    if (LCase(arrConfig(0)) == "hidden")
                        break;
                    string strConfigTrail = "";
                    string strConfigName = arrConfig(numCounter);
                    string strConfigValue = drwWebSetting(1).ToString;
                    MenuItem itmParent = null/* TODO Change to default(_) if this is not a reference type */;

                    for (int i = 0; i <= numCounter; i++)
                    {
                        if (i > 0)
                            strConfigTrail += ".";
                        strConfigTrail += arrConfig(i);
                    }
                    string strParentTrail = "";
                    if (numCounter == 0)
                        strParentTrail = strConfigName;
                    else
                        strParentTrail = Strings.Left(strConfigTrail, Strings.InStrRev(strConfigTrail, ".") - 1);


                    MenuItem itmConfigMenu = new MenuItem();
                    itmConfigMenu.Text = strConfigName;
                    itmConfigMenu.Value = strConfigTrail;
                    if (Strings.LCase(strConfigValue) == "y")
                        strConfigValue = "yes";
                    if (Strings.LCase(strConfigValue) == "n")
                        strConfigValue = "no";
                    itmConfigMenu.ToolTip = strConfigTrail + " : " + strConfigValue;
                    itmConfigMenu.NavigateUrl = CkartrisBLL.WebShopURL + "Admin/_Config.aspx?name=" + strConfigTrail;
                    itmConfigMenu.Selectable = (numCounter == Information.UBound(arrConfig) | numCounter == Information.UBound(arrConfig) - 1);


                    foreach (MenuItem itmConfig in menConfig.Items)
                    {
                        if (itmConfig.Value == strParentTrail)
                        {
                            itmParent = itmConfig;
                            break;
                        }
                        else
                        {
                            MenuItem itmFind = FindSubMenuItem(itmConfig, strParentTrail);
                            if (itmFind != null)
                            {
                                itmParent = itmFind;
                                break;
                            }
                        }
                    }
                    if (itmParent == null)
                        menConfig.Items.Add(itmConfigMenu);
                    else if (FindSubMenuItem(itmParent, strConfigTrail) == null & numCounter > 0)
                        itmParent.ChildItems.Add(itmConfigMenu);
                }
            }
        }
    }

    /// <summary>
    ///     ''' recursively find a menuitem
    ///     ''' </summary>
    ///     ''' <param name="itmMenu"></param>
    ///     ''' <param name="strValue"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    private MenuItem FindSubMenuItem(MenuItem itmMenu, string strValue)
    {
        MenuItem itmReturn = null/* TODO Change to default(_) if this is not a reference type */;
        if ((itmMenu.ChildItems.Count != 1 & itmMenu.Parent != null))
            itmMenu.Selectable = true;
        else
            itmMenu.Selectable = false;
        if (itmMenu.ChildItems.Count > 0)
            itmMenu.ToolTip = "";
        foreach (MenuItem itmChild in itmMenu.ChildItems)
        {
            if (itmChild.Value == strValue)
                return itmChild;
            itmReturn = FindSubMenuItem(itmChild, strValue);
        }
        return itmReturn;
    }

    protected void lnkBtnBack_Click()
    {
        ClearTextControls();
        mvwConfig.SetActiveView(viwResult);
    }

    protected void btnFind_Click(object sender, System.EventArgs e)
    {
        gvwConfig.PageIndex = 0;
        SearchConfig(txtSearchStarting.Text);
    }

    protected void btnClear_Click(object sender, System.EventArgs e)
    {
        txtSearchStarting.Text = string.Empty;
        SearchConfig(txtSearchStarting.Text);
    }


    protected void btnNew_Click(object sender, System.EventArgs e)
    {
        PrepareNewEntry();
    }

    private void SearchConfig(string pstrText, int pIndx = 0, int pPageIndx = 0)
    {
        DataTable tblConfig = new DataTable();

        if (ddlConfigFilter.SelectedValue == "i")
            tblConfig = ConfigBLL._SearchConfig(pstrText, true);
        else
            tblConfig = ConfigBLL._SearchConfig(pstrText, false);

        if (tblConfig.Rows.Count == 0)
            // No results
            mvwConfig.SetActiveView(viwNoResult);
        else if (tblConfig.Rows.Count == 1)
        {
            // One result, try to select it
            gvwConfig.DataSource = tblConfig;
            gvwConfig.DataBind();
            gvwConfig.SelectedIndex = pIndx;

            PrepareEditEntry();
            mvwConfig.SetActiveView(viwEdit);
        }
        else
                     // If search blank, show config selection menu instead
                     // of search results
                     if (pstrText == "")
            // Blank search, show menu
            mvwConfig.SetActiveView(viwMenu);
        else
        {
            // Searched for something,
            // show results
            gvwConfig.DataSource = tblConfig;
            gvwConfig.DataBind();
            gvwConfig.SelectedIndex = pIndx;

            mvwConfig.SetActiveView(viwResult);
        }
    }

    protected void lstRadioButtons_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        litConfigName.Text = lstRadioButtons.SelectedValue;
        txtSearchStarting.Focus();
    }

    protected void gvwConfig_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvwConfig.PageIndex = e.NewPageIndex;
        SearchConfig(litConfigName.Text + txtSearchStarting.Text);
    }

    protected void gvwConfig_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        if (gvwConfig.SelectedIndex == -1)
            return;
        PrepareEditEntry();
        mvwConfig.SetActiveView(viwEdit);
    }

    private void PrepareNewEntry()
    {
        gvwConfig.SelectedIndex = -1;

        ddlCFG_DataType.Items.Clear();
        ddlCFG_DataType.Items.Add(new ListItem("-", "-"));
        ddlCFG_DataType.Items.Add(new ListItem("string", "s"));
        ddlCFG_DataType.Items.Add(new ListItem("numeric", "n"));

        ddlCFG_DisplayType.Items.Clear();
        ddlCFG_DisplayType.Items.Add(new ListItem("-", "-"));
        ddlCFG_DisplayType.Items.Add(new ListItem("string", "s"));
        ddlCFG_DisplayType.Items.Add(new ListItem("numeric", "n"));
        ddlCFG_DisplayType.Items.Add(new ListItem("text", "t"));
        ddlCFG_DisplayType.Items.Add(new ListItem("boolean", "b"));
        ddlCFG_DisplayType.Items.Add(new ListItem("list", "l"));

        ClearTextControls();

        // Prepare value dropdown if required
        var blnShowValuesInDropDownList = BuildDisplayInfoDropDownList(txtCFG_DisplayInfo.Text, txtCFG_Value.Text);

        litPleaseEnterValue.Visible = false;
        phdNameAlreadyExist.Visible = false;
        phdCheckChange.Visible = true;

        txtCFG_Name.Visible = true; valRequiredName.Enabled = true;
        litCFG_Name.Visible = false;

        LockControls();

        mvwConfig.SetActiveView(viwEdit);

        txtCFG_Name.Focus();
    }

    private void PrepareEditEntry()
    {
        ddlCFG_DataType.Items.Clear();
        ddlCFG_DataType.Items.Add(new ListItem("string", "s"));
        ddlCFG_DataType.Items.Add(new ListItem("numeric", "n"));

        ddlCFG_DisplayType.Items.Clear();
        ddlCFG_DisplayType.Items.Add(new ListItem("string", "s"));
        ddlCFG_DisplayType.Items.Add(new ListItem("numeric", "n"));
        ddlCFG_DisplayType.Items.Add(new ListItem("text", "t"));
        ddlCFG_DisplayType.Items.Add(new ListItem("boolean", "b"));
        ddlCFG_DisplayType.Items.Add(new ListItem("list", "l"));

        ClearTextControls();

        DataRow drConfig = ConfigBLL._GetConfigByName(gvwConfig.SelectedRow.Cells(0).Text).Rows(0);
        litCFG_Name.Text = FixNullFromDB(drConfig("CFG_Name"));
        txtCFG_Value.Text = FixNullFromDB(drConfig("CFG_Value"));
        ddlCFG_DataType.SelectedValue = FixNullFromDB(drConfig("CFG_DataType"));
        ddlCFG_DisplayType.SelectedValue = FixNullFromDB(drConfig("CFG_DisplayType"));
        txtCFG_DefaultValue.Text = FixNullFromDB(drConfig("CFG_DefaultValue"));
        txtCFG_Desc.Text = FixNullFromDB(drConfig("CFG_Description"));
        txtCFG_DisplayInfo.Text = FixNullFromDB(drConfig("CFG_DisplayInfo"));
        txtCFG_VersionAdded.Text = FixNullFromDB(drConfig("CFG_VersionAdded"));
        chkCFG_Important.Checked = FixNullFromDB(drConfig("CFG_Important"));

        // Prepare value dropdown if required
        var blnShowValuesInDropDownList = BuildDisplayInfoDropDownList(txtCFG_DisplayInfo.Text, txtCFG_Value.Text);

        phdNameAlreadyExist.Visible = false;
        litPleaseEnterValue.Visible = false;
        phdCheckChange.Visible = false;

        txtCFG_Name.Visible = false; valRequiredName.Enabled = false;
        litCFG_Name.Visible = true;

        ddlCFG_DataType.Enabled = SetExpertSetting();
        ddlCFG_DisplayType.Enabled = SetExpertSetting();
        txtCFG_DisplayInfo.Enabled = SetExpertSetting();
        txtCFG_DefaultValue.Enabled = SetExpertSetting();
        txtCFG_Desc.Enabled = SetExpertSetting();
        txtCFG_VersionAdded.Enabled = SetExpertSetting();
        chkCFG_Important.Enabled = SetExpertSetting();
    }

    public bool BuildDisplayInfoDropDownList(string strDisplayInfo, string strCFG_Value)
    {
        ddlCFG_Value.Items.Clear();
        if (strDisplayInfo.Contains("|"))
        {
            // We have a list of options
            // First step is to build array from the list
            var aryOptions = Strings.Split(strDisplayInfo, "|", -1);
            for (var numCounter = 0; numCounter <= Information.UBound(aryOptions); numCounter++)
                ddlCFG_Value.Items.Add(new ListItem(aryOptions[numCounter], aryOptions[numCounter]));
            // Try to set the dropdown to a value if
            // config value matches one item in the menu
            try
            {
                ddlCFG_Value.SelectedValue = strCFG_Value;
            }
            catch (Exception ex)
            {
            }

            // Hide text field, show dropdown
            txtCFG_Value.Visible = false;
            ddlCFG_Value.Visible = true;

            return true;
        }
        else
        {
            // Hide dropdown, show text field
            txtCFG_Value.Visible = true;
            ddlCFG_Value.Visible = false;
            return false;
        }
    }

    private void ClearTextControls()
    {
        txtCFG_Name.Text = string.Empty;
        txtCFG_Value.Text = string.Empty;
        txtCFG_DefaultValue.Text = string.Empty;
        txtCFG_Desc.Text = string.Empty;
        txtCFG_DisplayInfo.Text = string.Empty;
        txtCFG_VersionAdded.Text = KARTRIS_VERSION;
        chkCFG_Important.Checked = false;
    }

    private void SaveChanges(DML_OPERATION enumOperation)
    {
        char chrDataType = "";
        char chrDisplayType = "";
        string strConfigName = "";
        string strValue = "";
        string strValueDropdown = "";
        string strDesc = "";
        string strDefaultValue = "";
        string strDisplayInfo = "";
        float sngVersionAdded = 0.0F;
        bool blnImportant;
        string strMessage = "";

        strConfigName = txtCFG_Name.Text;

        // We collect values from both the value text field
        // and the dropdown
        strValue = txtCFG_Value.Text;
        strValueDropdown = ddlCFG_Value.Text;

        // If a value is selected from the dropdown, we use
        // this as the value, otherwise we stick to the text
        // field value
        if (strValueDropdown != "")
            strValue = strValueDropdown;

        chrDataType = ddlCFG_DataType.SelectedValue;
        chrDisplayType = ddlCFG_DisplayType.SelectedValue;
        strDefaultValue = txtCFG_DefaultValue.Text;
        strDesc = txtCFG_Desc.Text;
        strDisplayInfo = txtCFG_DisplayInfo.Text;
        sngVersionAdded = System.Convert.ToSingle(txtCFG_VersionAdded.Text);
        blnImportant = chkCFG_Important.Checked;

        switch (enumOperation)
        {
            case object _ when DML_OPERATION.INSERT:
                {
                    if (ConfigBLL._AddConfig(strConfigName, strValue, chrDataType, chrDisplayType, strDisplayInfo, strDesc, strDefaultValue, sngVersionAdded, blnImportant, strMessage))
                    {
                        KartSettingsManager.RefreshCache();

                        if (System.Web.UI.UserControl.Request.QueryString["name"] == null & string.IsNullOrEmpty(txtSearchStarting.Text))
                        {
                            CreateMenu();
                            mvwConfig.SetActiveView(viwMenu);
                        }
                        else
                            mvwConfig.SetActiveView(viwResult);


                        ShowMasterUpdate?.Invoke();
                    }
                    else
                    {
                        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                        phdMessageError.Visible = true;
                    }

                    break;
                }

            case object _ when DML_OPERATION.UPDATE:
                {
                    if (ConfigBLL._UpdateConfig(litCFG_Name.Text, strValue, chrDataType, chrDisplayType, strDisplayInfo, strDesc, strDefaultValue, sngVersionAdded, blnImportant, strMessage))
                    {

                        // ' If isn't a postback, we read the search term from a querystring; this functionality allows us to
                        // '  receive links from the main search, and know what config setting we need to find
                        if (!System.Web.UI.UserControl.Request.QueryString["name"] == null & !this.IsPostBack)
                        {
                            txtSearchStarting.Text = System.Web.UI.UserControl.Request.QueryString["name"];
                            SearchConfig(txtSearchStarting.Text);
                        }
                        else
                            SearchConfig(txtSearchStarting.Text);

                        ShowMasterUpdate?.Invoke();
                        KartSettingsManager.RefreshCache();
                        if (strConfigName.ToLower() == "general.seofriendlyurls.enabled")
                            CkartrisDataManipulation.RefreshSiteMap();
                    }
                    else
                    {
                        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                        phdMessageError.Visible = true;
                    }

                    break;
                }
        }
    }

    public bool SetExpertSetting()
    {
        if (KartSettingsManager.GetKartConfig("backend.expertmode") != "y")
            return false;
        else
            return true;
    }

    // Formats Kartris version number nicely, to 4 decimal places
    public string FormatVersion(float numVersion)
    {
        return numVersion.ToString("N4");
    }
    private bool CheckConfigName()
    {
        string strNewName = txtCFG_Name.Text;
        if (ConfigBLL._GetConfigByName(strNewName).Rows.Count == 1)
            return false;
        return true;
    }
    protected void lnkBtnViewConfigName_Click(object sender, System.EventArgs e)
    {
        SearchConfig(txtCFG_Name.Text);
    }

    protected void btnSave_Click(object sender, System.EventArgs e)
    {
        if (gvwConfig.SelectedIndex == -1)
            SaveChanges(DML_OPERATION.INSERT);
        else
            SaveChanges(DML_OPERATION.UPDATE);
        CreateMenu();
    }

    protected void btnCancel_Click(object sender, System.EventArgs e)
    {
        ClearTextControls();
        mvwConfig.SetActiveView(viwResult);
    }

    protected void lnkBtnCFG_CheckName_Click(object sender, System.EventArgs e)
    {
        if (txtCFG_Name.Text != "")
        {
            if (CheckConfigName())
            {
                UnlockControls();
                lnkBtnCFG_ChangeName.Visible = true;
                lnkBtnCFG_CheckName.Visible = false;
                phdNameAlreadyExist.Visible = false;
                litPleaseEnterValue.Visible = false;
                txtCFG_Value.Focus();
            }
            else
            {
                phdNameAlreadyExist.Visible = true;
                litPleaseEnterValue.Visible = false;
            }
        }
        else
        {
            litPleaseEnterValue.Visible = true;
            phdNameAlreadyExist.Visible = false;
        }
    }

    public void LockControls()
    {
        txtCFG_Name.Enabled = true;
        txtCFG_Value.Enabled = false;
        ddlCFG_DataType.Enabled = false;
        ddlCFG_DisplayType.Enabled = false;
        txtCFG_DisplayInfo.Enabled = false;
        txtCFG_DefaultValue.Enabled = false;
        txtCFG_Desc.Enabled = false;
        txtCFG_VersionAdded.Enabled = false;
        chkCFG_Important.Enabled = false;
    }

    public void UnlockControls()
    {
        txtCFG_Name.Enabled = false;
        txtCFG_Value.Enabled = true;
        ddlCFG_DataType.Enabled = true;
        ddlCFG_DisplayType.Enabled = true;
        txtCFG_DisplayInfo.Enabled = true;
        txtCFG_DefaultValue.Enabled = true;
        txtCFG_Desc.Enabled = true;
        txtCFG_VersionAdded.Enabled = true;
        chkCFG_Important.Enabled = true;
    }

    protected void lnkBtnCFG_ChangeName_Click(object sender, System.EventArgs e)
    {
        LockControls();
        lnkBtnCFG_ChangeName.Visible = false;
        lnkBtnCFG_CheckName.Visible = true;
        txtCFG_Name.Focus();
    }

    protected void lnkDetails_Click(object sender, EventArgs e)
    {
        if (phdDetails.Visible)
        {
            phdDetails.Visible = false;
            // Set the text of the 'details' link
            lnkDetails.Text = "[+] <span class=\"bold\">" + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_Details") + "</span>";
        }
        else
        {
            phdDetails.Visible = true;
            // Set the text of the 'details' link
            lnkDetails.Text = "[-] <span class=\"bold\">" + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_Details") + "</span>";
        }
    }
}
