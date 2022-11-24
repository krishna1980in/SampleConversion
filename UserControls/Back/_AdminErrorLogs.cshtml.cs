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
using KartSettingsManager;

partial class UserControls_Back_AdminErrorLogs : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!System.Web.UI.Control.Page.IsPostBack)
        {
            GetErrorLogs();
            litErrorLogPath.Text = ConfigurationManager.AppSettings("errorlogpath");
            litErrorLogStatus.Text = IIf(GetKartConfig("general.errorlogs.enabled") == "y", System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_Enabled"), System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_Disabled"));

            btnSubmit_Click(sender, e); // this submits most recent month to preload
            try
            {
                // Try to preset to today's file, most people will want to
                // view this so handy to default it
                // lbxFiles.SelectedValue = "2014.08/2014.08.30.config"
                lbxFiles.SelectedValue = Strings.Format(DateTime.Now, "yyyy.MM") + "/" + System.Convert.ToString(DateTime.Now.Year) + "." + Strings.Format(DateTime.Now, "MM") + "." + Strings.Format(DateTime.Now, "dd") + ".config";
                lbxFiles_SelectedIndexChanged(sender, e);
            }
            catch (Exception ex)
            {
            }
        }
    }

    public void GetErrorLogs()
    {
        ClearErrorText();
        ddlMonthYear.Items.Clear();
        ddlMonthYear.Items.Add(new ListItem(System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_DropDownSelect"), "0"));
        DirectoryInfo dirErrors = new DirectoryInfo(System.Web.UI.UserControl.Server.MapPath("~/" + ConfigurationManager.AppSettings("errorlogpath") + "/Errors"));
        if (dirErrors.Exists)
        {
            foreach (DirectoryInfo d in dirErrors.GetDirectories())
                ddlMonthYear.Items.Add(new ListItem(d.Name, d.Name));
        }

        if (ddlMonthYear.Items.Count > 0)
            ddlMonthYear.SelectedValue = ddlMonthYear.Items(ddlMonthYear.Items.Count - 1).Value;
    }

    protected void btnSubmit_Click(object sender, System.EventArgs e)
    {
        ClearErrorText();
        if (ddlMonthYear.SelectedValue != "0")
        {
            DirectoryInfo dirErrors = new DirectoryInfo(System.Web.UI.UserControl.Server.MapPath("~/" + ConfigurationManager.AppSettings("errorlogpath") + "/Errors/" + ddlMonthYear.SelectedValue));
            if (dirErrors.Exists)
            {
                foreach (FileInfo f in dirErrors.GetFiles())
                {
                    string strName;
                    strName = Microsoft.VisualBasic.Left(f.Name, f.Name.IndexOf(".config"));
                    strName = Interaction.IIf(strName.StartsWith("_"), strName.TrimStart("_"), strName);
                    lbxFiles.Items.Add(new ListItem(strName, ddlMonthYear.SelectedValue + "/" + f.Name));
                }
            }
        }
        btnRefresh.Visible = false;
    }

    protected void lbxFiles_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        string strFilePath = System.Web.UI.UserControl.Server.MapPath("~/" + ConfigurationManager.AppSettings("errorlogpath") + "/Errors/" + lbxFiles.SelectedValue);
        StreamReader filReader = new StreamReader(strFilePath);
        try
        {
            txtFileText.Text = filReader.ReadToEnd();
            filReader.Close();
        }
        catch (Exception ex)
        {
            ClearErrorText();
        }
        try
        {
            filReader.Close();
        }
        catch (Exception ex)
        {
        }
        btnRefresh.Visible = true;
    }

    private void ClearErrorText()
    {
        lbxFiles.Items.Clear();
        txtFileText.Text = string.Empty;
    }


    protected void lnkBtnDeleteAllErrorLogs_Click(object sender, System.EventArgs e)
    {
        string strMessage = Strings.Replace(System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_ConfirmDeleteItem"), "[itemname]", System.Web.UI.TemplateControl.GetGlobalResourceObject("_Admin", "ContentText_AllErrorLogs"));
        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.Confirmation, strMessage);
    }

    protected void _UC_PopupMsg_Confirmed()
    {
        string strResult = DeleteErrorLogs();
        if (!strResult == null)
        {
            if (!string.IsNullOrEmpty(strResult))
            {
                litError.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_NotDeletedFolders") + "<br/>" + strResult;
                phdError.Visible = true;
            }
        }
        else
        {
            litError.Text = "";
            phdError.Visible = false;
        }
        GetErrorLogs();
    }

    public string DeleteErrorLogs()
    {
        StringBuilder strBldrNotDeletedFolders = new StringBuilder("");
        StringBuilder strBldrNotDeletedFiles = new StringBuilder("");
        string strLogPath = ConfigurationManager.AppSettings("errorlogpath"); // GetKartConfig("general.errorlogs.path")
        DirectoryInfo dirErrors = new DirectoryInfo(System.Web.UI.UserControl.Server.MapPath("~/" + strLogPath));
        StringBuilder strBldr = new StringBuilder("");
        FileInfo[] fleLog;
        if (dirErrors.Exists)
        {
            foreach (DirectoryInfo d in dirErrors.GetDirectories())
            {
                if (!System.Web.UI.UserControl.Application["isMediumTrust"])
                {
                    try
                    {
                        d.Delete(true);
                    }
                    catch (Exception ex)
                    {
                        strBldrNotDeletedFolders.AppendLine(d.Name);
                    }
                }
                else
                    foreach (DirectoryInfo innerDir in d.GetDirectories())
                    {
                        fleLog = innerDir.GetFiles();
                        for (int i = 0; i <= fleLog.Length - 1; i++)
                        {
                            try
                            {
                                fleLog[i].Delete();
                            }
                            catch (Exception ex)
                            {
                                strBldrNotDeletedFiles.AppendLine(d.Name).AppendLine(fleLog[i].Name);
                            }
                        }
                    }
            }
        }
        if (!System.Web.UI.UserControl.Application["isMediumTrust"])
            return strBldrNotDeletedFolders.ToString();
        return strBldrNotDeletedFiles.ToString();

        btnRefresh.Visible = false;
    }

    protected void btnRefresh_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        string strFilePath = System.Web.UI.UserControl.Server.MapPath("~/" + ConfigurationManager.AppSettings("errorlogpath") + "/Errors/" + lbxFiles.SelectedValue);
        StreamReader filReader = new StreamReader(strFilePath);
        try
        {
            txtFileText.Text = filReader.ReadToEnd();
            filReader.Close();
        }
        catch (Exception ex)
        {
            ClearErrorText();
        }
        try
        {
            filReader.Close();
        }
        catch (Exception ex)
        {
        }
    }
}
