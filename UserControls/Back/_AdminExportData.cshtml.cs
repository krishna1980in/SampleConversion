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

partial class UserControls_Back_AdminExportData : System.Web.UI.UserControl
{
    public event ExportSavedEventHandler ExportSaved;

    public delegate void ExportSavedEventHandler();

    protected void btnExportOrders_Click(object sender, System.EventArgs e)
    {
        bool blnParametersAllOK = false;
        string strStartDate = null;
        string strEndDate = null;
        if (IsDate(txtStartDate.Text))
            strStartDate = MonthName((DateTime)txtStartDate.Text.Month, true) + " " + (DateTime)txtStartDate.Text.Day + " " + (DateTime)txtStartDate.Text.Year;
        else if (IsNumeric(txtStartDate.Text))
            strStartDate = System.Convert.ToString(txtStartDate.Text);
        else
            valCustomStartDate.IsValid = false;
        if (IsDate(txtEndDate.Text))
            strEndDate = MonthName((DateTime)txtEndDate.Text.Month, true) + " " + (DateTime)txtEndDate.Text.Day + " " + (DateTime)txtEndDate.Text.Year;
        else if (IsNumeric(txtEndDate.Text))
            strEndDate = System.Convert.ToString(txtEndDate.Text);
        else
            valCustomEndDate.IsValid = false;
        // check if start and end fields are both date or isnumeric
        if ((IsDate(txtStartDate.Text) & IsDate(txtEndDate.Text)) | (IsNumeric(txtStartDate.Text) & IsNumeric(txtEndDate.Text)))
            blnParametersAllOK = true;

        if (blnParametersAllOK)
        {
            int FieldSeparator = ddlFieldDelimiter.SelectedValue;
            int StringSeparator = ddlStringDelimiter.SelectedValue;
            DataTable tblExportedData = ExportBLL._ExportOrders(strStartDate, strEndDate, chkOrderDetails.Checked, chkIncompleteOrders.Checked);
            CKartrisCSVExporter.WriteToCSV(tblExportedData, txtFileName.Text, FieldSeparator, StringSeparator);
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!System.Web.UI.Control.Page.IsPostBack)
            LoadSavedExports();
    }

    protected void btnCustomExport_Click(object sender, System.EventArgs e)
    {
        CustomExport();
    }

    public void CustomExport()
    {
        // If txtSqlQuery.Text.ToUpper.StartsWith("SELECT") Then
        int FieldSeparator = ddlCustomFieldDelimiter.SelectedValue;
        int StringSeparator = ddlCustomStringDelimiter.SelectedValue;
        DataTable tblExportedData = ExportBLL._CustomExecute(txtSqlQuery.Text);
        CKartrisCSVExporter.WriteToCSV(tblExportedData, txtExportName.Text, FieldSeparator, StringSeparator);
    }

    protected void btnSaveExport_Click(object sender, System.EventArgs e)
    {
        if (_GetExportID() == 0)
            SaveExport(DML_OPERATION.INSERT);
        else
            SaveExport(DML_OPERATION.UPDATE);
    }

    public void SaveExport(DML_OPERATION enumOperation)
    {
        string strMessage = "";

        switch (enumOperation)
        {
            case object _ when DML_OPERATION.UPDATE:
                {
                    if (!ExportBLL._UpdateSavedExport(_GetExportID(), txtExportName.Text, txtSqlQuery.Text, ddlCustomFieldDelimiter.SelectedValue, ddlCustomStringDelimiter.SelectedValue, strMessage))
                    {
                        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                        return;
                    }

                    break;
                }

            case object _ when DML_OPERATION.INSERT:
                {
                    if (!ExportBLL._AddSavedExport(txtExportName.Text, txtSqlQuery.Text, ddlCustomFieldDelimiter.SelectedValue, ddlCustomStringDelimiter.SelectedValue, strMessage))
                    {
                        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                        return;
                    }

                    break;
                }
        }

        LoadSavedExports();
        tabExport.ActiveTab = tabSavedExports;
        ClearFields();
        updMain.Update();
        ExportSaved?.Invoke();
    }

    public void LoadSavedExports()
    {
        DataTable tblSavedExports = ExportBLL._GetSavedExports();
        gvwSavedExports.DataSource = tblSavedExports;
        gvwSavedExports.DataBind();
        updSavedExports.Update();
    }

    protected void btnCancel_Click(object sender, System.EventArgs e)
    {
        ClearFields();
    }

    public void ClearFields()
    {
        txtExportName.Text = null;
        txtSqlQuery.Text = null;
        ddlCustomFieldDelimiter.SelectedIndex = 0;
        ddlCustomStringDelimiter.SelectedIndex = 0;
        litSavedExportID.Text = "0";
        litSavedExportName.Text = "";
        lnkBtnDelete.Visible = false;
        btnCancel.Visible = false;
        updCustomExport.Update();
    }

    public int _GetExportID()
    {
        if (!IsNumeric(litSavedExportID.Text))
            litSavedExportID.Text = "0";
        return System.Convert.ToInt32(litSavedExportID.Text);
    }

    protected void gvwSavedExports_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "cmdExport":
                {
                    ExportByID(e.CommandArgument);
                    break;
                }

            case "cmdEdit":
                {
                    LoadSavedExport(e.CommandArgument);
                    break;
                }
        }
    }

    public void LoadSavedExport(long numExportID)
    {
        DataTable tblSavedExport = ExportBLL._GetSavedExport(numExportID);
        if (tblSavedExport.Rows.Count != 1)
            return;
        DataRow row = tblSavedExport.Rows(0);

        txtExportName.Text = FixNullFromDB(row("Export_Name"));
        txtSqlQuery.Text = FixNullFromDB(row("Export_Details"));
        ddlCustomFieldDelimiter.SelectedValue = FixNullFromDB(row("Export_FieldDelimiter"));
        ddlCustomStringDelimiter.SelectedValue = FixNullFromDB(row("Export_StringDelimiter"));

        lnkBtnDelete.Visible = true;
        btnCancel.Visible = true;
        litSavedExportID.Text = numExportID;
        litSavedExportName.Text = FixNullFromDB(row("Export_Name"));
        tabExport.ActiveTab = tabCustomExport;
        updMain.Update();
    }

    public void ExportByID(long numExportID)
    {
        DataTable tblSavedExport = ExportBLL._GetSavedExport(numExportID);
        if (tblSavedExport.Rows.Count != 1)
            return;

        DataRow row = tblSavedExport.Rows(0);

        DataTable tblExportedData = ExportBLL._CustomExecute(FixNullFromDB(row("Export_Details")));
        CKartrisCSVExporter.WriteToCSV(tblExportedData, FixNullFromDB(row("Export_Name")), FixNullFromDB(row("Export_FieldDelimiter")), FixNullFromDB(row("Export_StringDelimiter")));
    }

    protected void lnkBtnDelete_Click(object sender, System.EventArgs e)
    {
        string strMessage = Replace(System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_ConfirmDeleteItem"), "[itemname]", litSavedExportName.Text);
        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.Confirmation, strMessage);
    }

    protected void _UC_PopupMsg_Confirmed()
    {
        string strMessage = "";
        if (ExportBLL._DeleteSavedExport(_GetExportID(), strMessage))
        {
            LoadSavedExports();
            ClearFields();
        }
        else
            _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
    }
}
