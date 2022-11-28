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
using CkartrisDataManipulation;
using CkartrisDisplayFunctions;
using CkartrisEnumerations;
using KartSettingsManager;
using System.Data;

partial class UserControls_Back_AdminLog : System.Web.UI.UserControl
{
    public event AdminLogsUpdatedEventHandler AdminLogsUpdated;

    public delegate void AdminLogsUpdatedEventHandler();

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!System.Web.UI.Control.Page.IsPostBack)
        {
            txtStartDate.Text = NowOffset.AddDays(-7).Year + "/" + NowOffset.AddDays(-7).Month + "/" + NowOffset.AddDays(-7).Day;
            txtEndDate.Text = NowOffset.Year + "/" + NowOffset.Month + "/" + NowOffset.Day;

            LoadAdminLogRecords();

            litContentTextAdminLogsPurge1.Text = Replace(System.Web.UI.TemplateControl.GetGlobalResourceObject("_DBAdmin", "ContentText_AdminLogsPurge1"), "[date]", "<strong>" + FormatDate(NowOffset.AddDays(1 - System.Convert.ToInt32(GetKartConfig("backend.adminlog.purgedays"))), "d", System.Web.UI.UserControl.Session["_LANG"]) + "</strong>");
        }
    }

    private void LoadAdminLogRecords()
    {
        DataTable tblAdminLog = new DataTable();
        tblAdminLog = KartrisDBBLL._SearchAdminLog(txtAdminLogSearchText.Text, ddlAdminLogSearchBy.SelectedValue, (DateTime)txtStartDate.Text, (DateTime)txtEndDate.Text);

        if (tblAdminLog.Rows.Count == 0)
        {
            mvwAdminLog.SetActiveView(viwAdminLogEmpty); return;
        }

        gvwAdminLog.DataSource = tblAdminLog;
        gvwAdminLog.DataBind();
        mvwAdminLog.SetActiveView(viwAdminLogList);
    }

    protected void btnSearchAdminLog_Click(object sender, System.EventArgs e)
    {
        LoadAdminLogRecords();
        txtAdminLogSearchText.Text = ""; // blank search box
    }

    protected void gvwAdminLog_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvwAdminLog.PageIndex = e.NewPageIndex;
        LoadAdminLogRecords();
    }

    protected void gvwAdminLog_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName == "cmdDetails")
        {
            DataRow drLogDetails = KartrisDBBLL._GetLogByID(System.Convert.ToInt32(e.CommandArgument));
            if (!drLogDetails == null)
                LoadLogDetails(drLogDetails);
        }
    }

    public void LoadLogDetails(DataRow drDetails)
    {
        litLogID.Text = FixNullFromDB(drDetails["AL_ID"]);
        litLogDateTime.Text = FixNullFromDB(drDetails["AL_DateStamp"]);
        litLogUser.Text = FixNullFromDB(drDetails["AL_LoginName"]);
        litLogType.Text = FixNullFromDB(drDetails["AL_Type"]);
        litLogQuery.Text = Replace(FixNullFromDB(drDetails["AL_Query"]), "##", "<br/>&nbsp;&nbsp;&nbsp;&nbsp;");
        litLogDescription.Text = FixNullFromDB(drDetails["AL_Description"]);
        litLogRelatedID.Text = FixNullFromDB(drDetails["AL_RelatedID"]);
        litLogIP.Text = FixNullFromDB(drDetails["AL_IP"]);

        mvwAdminLog.SetActiveView(viwAdminLogDetails);
    }

    protected void lnkBtnBack_Click(object sender, System.EventArgs e)
    {
        mvwAdminLog.SetActiveView(viwAdminLogList);
    }

    protected void btnPurgeOldLogs_Click(object sender, System.EventArgs e)
    {
        string strMessage = "";
        if (!KartrisDBBLL._PurgeOldLogs(strMessage))
        {
            _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
            return;
        }
        LoadAdminLogRecords();
        AdminLogsUpdated?.Invoke();
    }
}
