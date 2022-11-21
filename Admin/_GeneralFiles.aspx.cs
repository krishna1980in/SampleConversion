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
using CkartrisDataManipulation;
using KartSettingsManager;

partial class Admin_GeneralFiles : _PageBaseClass
{
    private bool blnFileSaved = false;

    private string strFilesFolder = KartSettingsManager.GetKartConfig("general.uploadfolder") + "General/";

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (Page.IsPostBack)
            Page.ClientScript.GetPostBackEventReference(this, string.Empty);
        else
        {
            Page.Form.Enctype = "multipart/form-data";
            LoadGeneralFiles();
        }
        Page.Title = GetGlobalResourceObject("_Kartris", "PageTitle_GeneralFiles") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");
    }

    public string FormatFilePath(string strFilename)
    {
        return strFilesFolder + strFilename;
    }

    private void LoadGeneralFiles()
    {
        DataTable dtbFiles = new DataTable();
        dtbFiles.Columns.Add(new DataColumn("F_Name", Type.GetType("System.String")));
        dtbFiles.Columns.Add(new DataColumn("F_Type", Type.GetType("System.String")));
        dtbFiles.Columns.Add(new DataColumn("F_Size", Type.GetType("System.String")));
        dtbFiles.Columns.Add(new DataColumn("F_LastUpdated", Type.GetType("System.String")));

        DirectoryInfo dirGeneralFiles = new DirectoryInfo(Server.MapPath(strFilesFolder));
        FileInfo[] filInfo;
        if (!dirGeneralFiles.Exists())
            return;
        filInfo = dirGeneralFiles.GetFiles();
        string strName = "";
        string strType = "";
        string strSize = "";
        string strLastUpdated = "";
        for (int i = 0; i <= filInfo.GetUpperBound(0); i++)
        {
            strName = filInfo[i].Name;
            strType = filInfo[i].Extension;
            strSize = System.Convert.ToString(filInfo[i].Length() / (double)1000) + " KB";
            strLastUpdated = CkartrisDisplayFunctions.FormatDate(filInfo[i].LastWriteTime, "t", Session("_LANG"));
            if (strName.ToLower() == "web.config")
                continue;
            dtbFiles.Rows.Add(strName, strType, strSize, strLastUpdated);
        }

        if (dtbFiles.Rows.Count > 0)
        {
            gvwFiles.DataSource = dtbFiles;
            gvwFiles.DataBind();
            mvwGeneralFiles.SetActiveView(viwFiles);
        }
        else
            mvwGeneralFiles.SetActiveView(viwNoFiles);
    }

    protected void gvwFiles_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvwFiles.PageIndex = e.NewPageIndex;
        LoadGeneralFiles();
    }

    protected void lnkUpload_Click(object sender, System.EventArgs e) // Handles lnkUpload.Click
    {
        if (filUploader.HasFile)
        {
            if (!IsFileExist(filUploader.FileName))
            {
                // ' To avoid saving the file twice
                if (!blnFileSaved)
                {
                    try
                    {
                        filUploader.SaveAs(Server.MapPath(strFilesFolder + filUploader.FileName));
                        (Skins_Admin_Template)this.Master.DataUpdated();
                        LoadGeneralFiles();
                    }
                    catch (Exception ex)
                    {
                        litStatus2.Text = ex.Message;
                        popExtender2.Show();
                        blnFileSaved = false;
                    }
                }
            }
            else
            {
                litStatus2.Text = GetGlobalResourceObject("_Kartris", "ContentText_AlreadyExists");
                popExtender2.Show();
            }
        }
        else
        {
            litStatus2.Text = GetGlobalResourceObject("_Kartris", "ContentText_NoFile");
            popExtender2.Show();
        }
        (Skins_Admin_Template)this.Master.LoadCategoryMenu();
        updMain.Update();
    }

    public bool IsFileExist(string strFileName)
    {
        DirectoryInfo dirGeneralFiles = new DirectoryInfo(Server.MapPath(strFilesFolder));
        if (!dirGeneralFiles.Exists())
            Directory.CreateDirectory(Server.MapPath(strFilesFolder));
        FileInfo[] filInfo;
        filInfo = dirGeneralFiles.GetFiles(strFileName);
        if (filInfo.Length > 0)
            return true;
        return false;
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        popExtender.Show();
    }

    protected void gvwFiles_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "DeleteFile")
        {
            hidFileNameToDelete.Value = e.CommandArgument;
            string strMessage = Replace(GetGlobalResourceObject("_Kartris", "ContentText_ConfirmDeleteItem"), "[itemname]", e.CommandArgument);
            _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.Confirmation, strMessage);
        }
    }

    protected void _UC_PopupMsg_Confirmed()
    {
        if (!string.IsNullOrEmpty(hidFileNameToDelete.Value))
        {
            try
            {
                File.Delete(Server.MapPath(strFilesFolder + hidFileNameToDelete.Value));
                hidFileNameToDelete.Value = "";
                LoadGeneralFiles();
                (Skins_Admin_Template)this.Master.DataUpdated();
                updMain.Update();
            }
            catch (Exception ex)
            {
                _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, ex.Message);
            }
        }
    }
}
