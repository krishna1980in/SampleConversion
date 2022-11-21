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
using CkartrisDataManipulation;
using CkartrisDisplayFunctions;
using CkartrisEnumerations;
using KartSettingsManager;

partial class Admin_SupportTicketTypes : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Tickets", "PageTitle_SupportTicketTypes") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        if (!Page.IsPostBack)
        {
            // Set number of records per page
            int intRowsPerPage = 25;
            try
            {
                intRowsPerPage = System.Convert.ToDouble(KartSettingsManager.GetKartConfig("backend.display.pagesize"));
            }
            catch (Exception ex)
            {
            }
            gvwTicketTypes.PageSize = intRowsPerPage;

            if (KartSettingsManager.GetKartConfig("frontend.supporttickets.enabled") != "y")
            {
                litFeatureDisabled.Text = Replace(GetGlobalResourceObject("_Kartris", "ContentText_DisabledInFrontEnd"), "[name]", GetGlobalResourceObject("_Tickets", "PageTitle_SupportTickets"));
                phdFeatureDisabled.Visible = true;
            }
            else
                phdFeatureDisabled.Visible = false;

            LoadSupportTicketTypes();
        }
    }

    public void LoadSupportTicketTypes()
    {
        DataTable tblSupportTicketTypes = TicketsBLL._GetTicketTypes();

        if (tblSupportTicketTypes.Rows.Count == 0)
        {
            gvwTicketTypes.DataSource = null;
            gvwTicketTypes.DataBind();
            gvwTicketTypes.Visible = false;
            pnlNoTypes.Visible = true;
            updTypes.Update();
            return;
        }
        else
            pnlNoTypes.Visible = false;

        gvwTicketTypes.Visible = true;
        gvwTicketTypes.DataSource = tblSupportTicketTypes;
        gvwTicketTypes.DataBind();

        mvwTypes.SetActiveView(viwTypes);
        updMain.Update();
    }

    protected void gvwTicketTypes_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvwTicketTypes.PageIndex = e.NewPageIndex;
        LoadSupportTicketTypes();
    }

    protected void gvwTicketTypes_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName == "edit_type")
            PrepareExistingSupportType(e.CommandArgument);
    }

    protected void gvwTicketTypes_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if ((Literal)e.Row.Cells[1].FindControl("litSupportLevel").Text == "p")
            {
                if (e.Row.RowState == DataControlRowState.Alternate)
                    e.Row.CssClass = "Kartris-GridView-Alternate-Red";
                else
                    e.Row.CssClass = "Kartris-GridView-Red";
            }
        }
    }

    protected void lnkBtnShowTypesList_Click(object sender, System.EventArgs e)
    {
        mvwTypes.SetActiveView(viwTypes);
        updMain.Update();
    }

    protected void lnkBtnNewType_Click(object sender, System.EventArgs e)
    {
        PrepareNewSupportType();
    }

    public void PrepareNewSupportType()
    {
        litTypeID.Text = "0";
        txtSupportType.Text = null;
        litSupportType.Text = null;
        ddlSupportLevel.SelectedIndex = 0;
        phdDelete.Visible = false;
        mvwTypes.SetActiveView(viwEditType);
        updMain.Update();
    }

    public void PrepareExistingSupportType(int numTypeID)
    {
        DataTable tblTypes = TicketsBLL._GetTicketTypes();
        DataRow[] drwSupportType = tblTypes.Select("STT_ID=" + numTypeID);
        if (drwSupportType.Length != 1)
            return;
        litTypeID.Text = numTypeID;
        txtSupportType.Text = FixNullFromDB(drwSupportType[0]("STT_Name"));
        litSupportType.Text = FixNullFromDB(drwSupportType[0]("STT_Name"));

        // If tickets from before types have changed, this might fail,
        // so put in a try/catch loop for safety
        try
        {
            ddlSupportLevel.SelectedValue = FixNullFromDB(drwSupportType[0]("STT_Level"));
        }
        catch (Exception ex)
        {
        }

        DataView dvwTypes = tblTypes.DefaultView;
        dvwTypes.RowFilter = "STT_ID <> " + numTypeID;
        ddlTicketType.DataTextField = "STT_Name";
        ddlTicketType.DataValueField = "STT_ID";
        ddlTicketType.DataSource = dvwTypes;
        ddlTicketType.DataBind();
        phdDelete.Visible = true;
        mvwTypes.SetActiveView(viwEditType);
        updMain.Update();
    }

    public int GetTypeID()
    {
        if (litTypeID.Text != "")
            return System.Convert.ToInt32(litTypeID.Text);
        return 0;
    }

    protected void btnSave_Click(object sender, System.EventArgs e)
    {
        if (SaveChanges())
        {
            LoadSupportTicketTypes();
            (Skins_Admin_Template)this.Master.DataUpdated();
        }
    }

    protected void btnCancel_Click(object sender, System.EventArgs e)
    {
        mvwTypes.SetActiveView(viwTypes);
        updMain.Update();
    }

    protected void lnkBtnDelete_Click(object sender, System.EventArgs e)
    {
        string strMessage = Replace(GetGlobalResourceObject("_Kartris", "ContentText_ConfirmDeleteItem"), "[itemname]", litSupportType.Text);
        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.Confirmation, strMessage);
    }

    public bool SaveChanges()
    {
        if ((GetTypeID() == 0))
        {
            if (!SaveType(DML_OPERATION.INSERT))
                return false;
        }
        else if (!SaveType(DML_OPERATION.UPDATE))
            return false;

        return true;
    }

    private bool SaveType(DML_OPERATION enumOperation)
    {
        string strTypeName = txtSupportType.Text;
        char chrTypeLevel = ddlSupportLevel.SelectedValue;
        string strMessage = "";
        long numTypeID = GetTypeID();
        switch (enumOperation)
        {
            case object _ when DML_OPERATION.UPDATE:
                {
                    if (!TicketsBLL._UpdateTicketType(numTypeID, strTypeName, chrTypeLevel, strMessage))
                    {
                        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                        return false;
                    }

                    break;
                }

            case object _ when DML_OPERATION.INSERT:
                {
                    if (!TicketsBLL._AddTicketType(strTypeName, chrTypeLevel, strMessage))
                    {
                        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                        return false;
                    }

                    break;
                }
        }

        return true;
    }

    protected void _UC_PopupMsg_Confirmed()
    {
        string strMessage = "";
        if (TicketsBLL._DeleteTicketType(GetTypeID(), ddlTicketType.SelectedValue, strMessage))
        {
            LoadSupportTicketTypes();
            (Skins_Admin_Template)this.Master.DataUpdated();
        }
        else
            _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
    }
}
