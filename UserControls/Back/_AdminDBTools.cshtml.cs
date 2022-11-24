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

partial class UserControls_Back_AdminDBTools : System.Web.UI.UserControl
{
    protected void btnCreateBackup_Click(object sender, System.EventArgs e)
    {
        if (!File.Exists(litBackupName.Text))
        {
            CreateBackup(); return;
        }
        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.Confirmation, System.Web.UI.TemplateControl.GetGlobalResourceObject("_DBAdmin", "ContentText_BackupAlreadyExists"));
    }

    protected void Back_Click(object sender, System.EventArgs e)
    {
        mvwBackup.SetActiveView(viwCreateBackup);
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!System.Web.UI.Control.Page.IsPostBack)
        {
            litConfigBackupFolder.Text = GetKartConfig("general.backupfolder");
            GetDBInformation();
            CreateBackupName();
            litRootPath.Text = System.Web.UI.UserControl.Server.MapPath("../");
        }
    }

    private void GetDBInformation()
    {
        string strMessage = string.Empty;
        DataTable tblDBInfo = new DataTable();
        KartrisDBBLL._GetDBInformation(tblDBInfo, strMessage);
        if (tblDBInfo.Rows.Count == 0)
            return;
        foreach (DataRow row in tblDBInfo.Rows)
        {
            litDBName.Text = row("DatabaseName");
            if (System.Convert.ToString(row("FileName")).EndsWith("_log"))
            {
                row("FileType") = "LOG FILE"; continue;
            }
            row("FileType") = "DATA FILE";
        }
        gvwDBInfo.DataSource = tblDBInfo;
        gvwDBInfo.DataBind();
    }

    private void CreateBackupName()
    {
        litBackupName.Text = GetKartConfig("general.backupfolder") + System.Convert.ToString(DateTime.Now.Year) + "." + System.Convert.ToString(DateTime.Now.Month) + "." + System.Convert.ToString(DateTime.Now.Day) + ".bak";
    }

    private void CreateBackup()
    {
        string strMessage = string.Empty;
        if (!KartrisDBBLL._CreateDBBackup(litBackupName.Text, txtBackupDescription.Text, strMessage))
        {
            mvwBackup.SetActiveView(viwBackupFailed);
            litBackupFailed.Text = Replace(System.Web.UI.TemplateControl.GetGlobalResourceObject("_DBAdmin", "ContentText_BackupUnsuccessful"), "[filename]", "<span>" + litBackupName.Text + "</span>");
        }
        else if (File.Exists(litBackupName.Text))
        {
            mvwBackup.SetActiveView(viwBackupSucceeded);
            litBackupSucceeded.Text = Replace(System.Web.UI.TemplateControl.GetGlobalResourceObject("_DBAdmin", "ContentText_BackupSuccessful"), "[filename]", "<span>" + litBackupName.Text + "</span>");
            txtBackupDescription.Text = string.Empty;
        }
        else
        {
            mvwBackup.SetActiveView(viwBackupFailed);
            litBackupFailed.Text = Replace(System.Web.UI.TemplateControl.GetGlobalResourceObject("_DBAdmin", "ContentText_BackupUnsuccessful"), "[filename]", "<span>" + litBackupName.Text + "</span>");
        }
        updAdminTools.Update();
    }

    protected void _UC_PopupMsg_Confirmed()
    {
        CreateBackup();
    }
}
