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

partial class Admin_SupportTickets : _PageBaseClass
{
    protected void ShowMasterUpdateMessage()
    {
        (Skins_Admin_Template)this.Master.DataUpdated();
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Tickets", "PageTitle_SupportTickets") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        if (!Page.IsPostBack)
        {

            // Store ID of admin in hidden field, so we don't lose
            // session in postbacks
            hidPresentAdminUserID.Value = Session("_UserID");

            // Set number of records per page
            int intRowsPerPage = 25;
            try
            {
                intRowsPerPage = System.Convert.ToDouble(KartSettingsManager.GetKartConfig("backend.display.pagesize"));
            }
            catch (Exception ex)
            {
            }
            gvwTickets.PageSize = intRowsPerPage;

            if (KartSettingsManager.GetKartConfig("frontend.supporttickets.enabled") != "y")
            {
                litFeatureDisabled.Text = Replace(GetGlobalResourceObject("_Kartris", "ContentText_DisabledInFrontEnd"), "[name]", GetGlobalResourceObject("_Tickets", "PageTitle_SupportTickets"));
                phdFeatureDisabled.Visible = true;
            }
            else
                phdFeatureDisabled.Visible = false;

            if (TicketQS() != 0)
            {
                _UC_EditTicket.GetTicket(TicketQS);
                mvwTickets.SetActiveView(viwEditTicket);
                updMain.Update();
            }
            else
            {
                ddlLanguages.Items.Clear();
                ddlLanguages.DataSource = GetLanguagesFromCache();
                ddlLanguages.DataBind();
                GetTicketTypes();
                GetTicketLogins();

                if (!string.IsNullOrEmpty(Request.QueryString("u")) || !string.IsNullOrEmpty(Request.QueryString("s")) || !string.IsNullOrEmpty(Request.QueryString("c")))
                {
                    if (!string.IsNullOrEmpty(Request.QueryString("u")))
                        ddlAssignedLogin.SelectedValue = Request.QueryString("u");
                    if (!string.IsNullOrEmpty(Request.QueryString("s")))
                        ddlTicketStatus.SelectedValue = Request.QueryString("s");
                    if (!string.IsNullOrEmpty(Request.QueryString("c")))
                        txtUser.Text = Request.QueryString("c");
                    FindTickets();
                }
                else
                    LoadSupportTickets();
            }
        }
    }

    public void LoadSupportTickets()
    {
        litListingType.Text = "NoSearch";
        DataTable tblSupportTickets = TicketsBLL._GetTickets();

        if (tblSupportTickets.Rows.Count == 0)
        {
            gvwTickets.DataSource = null;
            gvwTickets.DataBind();
            gvwTickets.Visible = false;
            pnlNoTickets.Visible = true;
            pnlTicketColors.Visible = false;
            updTickets.Update();
            return;
        }
        else
            pnlNoTickets.Visible = false;

        tblSupportTickets.Columns.Add(new DataColumn("DateOpened", Type.GetType("System.String")));
        tblSupportTickets.Columns.Add(new DataColumn("LastMessage", Type.GetType("System.String")));

        foreach (DataRow drwSupportTicket in tblSupportTickets.Rows)
        {
            drwSupportTicket("DateOpened") = FormatDate(drwSupportTicket("TIC_DateOpened"), "d", Session("_LANG"));
            drwSupportTicket("LastMessage") = FormatDate(drwSupportTicket("LastMessageDate"), "t", Session("_LANG"));
            if (System.Convert.ToString(FixNullFromDB(drwSupportTicket("TIC_Subject"))).Length > 30)
                drwSupportTicket("TIC_Subject") = Left(drwSupportTicket("TIC_Subject"), 27) + "...";
        }

        gvwTickets.Visible = true;
        DataView dvwTickets = tblSupportTickets.DefaultView;
        dvwTickets.Sort = "TIC_DateOpened DESC";
        gvwTickets.DataSource = dvwTickets.Table;
        gvwTickets.DataBind();
    }

    protected void gvwTickets_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvwTickets.PageIndex = e.NewPageIndex;
        if (litListingType.Text == "Search")
            FindTickets();
        else
            LoadSupportTickets();
    }

    protected void gvwTickets_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName == "edit_ticket")
        {
            _UC_EditTicket.GetTicket(e.CommandArgument);
            mvwTickets.SetActiveView(viwEditTicket);
            updMain.Update();
        }
    }

    protected void gvwTickets_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int numLoginID = 0;
            if (!string.IsNullOrEmpty((Literal)e.Row.Cells[7].FindControl("litLoginID").Text))
                numLoginID = System.Convert.ToInt32((Literal)e.Row.Cells[7].FindControl("litLoginID").Text);
            switch ((Literal)e.Row.Cells[7].FindControl("litTicketStatus").Text)
            {
                case "o":
                    {
                        if (numLoginID == 0 || numLoginID == hidPresentAdminUserID.Value)
                        {
                            // Check TIC_AwaitingResponse
                            if ((Literal)e.Row.Cells[7].FindControl("litAwaitingResponse").Text == true)
                            {
                                if (e.Row.RowState == DataControlRowState.Alternate)
                                    e.Row.CssClass = "Kartris-GridView-Alternate-Yellow";
                                else
                                    e.Row.CssClass = "Kartris-GridView-Yellow";
                            }
                        }
                        else if (e.Row.RowState == DataControlRowState.Alternate)
                            e.Row.CssClass = "Kartris-GridView-Alternate-Green";
                        else
                            e.Row.CssClass = "Kartris-GridView-Green";
                        break;
                    }

                case "u":
                case "n":
                    {
                        if (e.Row.RowState == DataControlRowState.Alternate)
                            e.Row.CssClass = "Kartris-GridView-Alternate-Red";
                        else
                            e.Row.CssClass = "Kartris-GridView-Red";
                        break;
                    }

                case "c":
                    {
                        if (e.Row.RowState == DataControlRowState.Alternate)
                            e.Row.CssClass = "Kartris-GridView-Alternate-Done";
                        else
                            e.Row.CssClass = "Kartris-GridView-Done";
                        break;
                    }
            }
        }
    }

    protected void lnkBtnShowTicketsList_Click(object sender, System.EventArgs e)
    {
        if (TicketQS() != 0)
            Response.Redirect("_SupportTickets.aspx");
        mvwTickets.SetActiveView(viwTickets);
        updMain.Update();
    }

    protected void _UC_EditTicket_TicketChanged()
    {
        LoadSupportTickets();
        updTickets.Update();
        (Skins_Admin_Template)this.Master.LoadTaskList();
    }

    protected void _UC_EditTicket_TicketDeleted()
    {
        if (TicketQS() != 0)
            Response.Redirect("_SupportTickets.aspx");
        LoadSupportTickets();
        mvwTickets.SetActiveView(viwTickets);
        updMain.Update();
        (Skins_Admin_Template)this.Master.LoadTaskList();
    }

    public void GetTicketTypes()
    {
        if (ddlTicketType.Items.Count == 0)
        {
            ddlTicketType.Items.Clear();
            ddlTicketType.Items.Add(new ListItem(GetGlobalResourceObject("_Kartris", "BackMenu_SearchAll"), "-1"));
            ddlTicketType.AppendDataBoundItems = true;
            ddlTicketType.DataTextField = "STT_Name";
            ddlTicketType.DataValueField = "STT_ID";
            DataTable tblTicketTypes = TicketsBLL._GetTicketTypes();
            ddlTicketType.DataSource = tblTicketTypes;
            ddlTicketType.DataBind();
        }
    }

    public void GetTicketLogins()
    {
        if (ddlAssignedLogin.Items.Count == 0)
        {
            ddlAssignedLogin.Items.Clear();
            ddlAssignedLogin.Items.Add(new ListItem(GetGlobalResourceObject("_Kartris", "BackMenu_SearchAll"), "-1"));
            ddlAssignedLogin.Items.Add(new ListItem(GetGlobalResourceObject("_Kartris", "ContentText_Unassigned"), "0"));
            ddlAssignedLogin.AppendDataBoundItems = true;
            ddlAssignedLogin.DataTextField = "LOGIN_Username";
            ddlAssignedLogin.DataValueField = "LOGIN_ID";
            DataTable tblTicketLogins = LoginsBLL._GetSupportUsers();
            ddlAssignedLogin.DataSource = tblTicketLogins;
            ddlAssignedLogin.DataBind();
        }
    }

    public long TicketQS()
    {
        if (!string.IsNullOrEmpty(Request.QueryString("TIC_ID")) && Request.QueryString("TIC_ID") != 0 && IsNumeric(Request.QueryString("TIC_ID")))
            return System.Convert.ToInt64(Request.QueryString("TIC_ID"));
        return 0;
    }
    protected void ddlAssignedLogin_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        FindTickets();
    }

    protected void ddlLanguages_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        FindTickets();
    }

    protected void ddlTicketStatus_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        FindTickets();
    }

    protected void ddlTicketType_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        FindTickets();
    }

    protected void txtUser_TextChanged(object sender, System.EventArgs e)
    {
        FindTickets();
    }

    protected void btnFind_Click(object sender, System.EventArgs e)
    {
        FindTickets();
    }

    protected void btnClear_Click(object sender, System.EventArgs e)
    {
        txtSearchStarting.Text = "";
        txtUser.Text = "";
        ddlLanguages.SelectedValue = LanguagesBLL.GetDefaultLanguageID();
        ddlAssignedLogin.SelectedIndex = 0;
        ddlTicketType.SelectedIndex = 0;
        ddlTicketStatus.SelectedIndex = 0;
        LoadSupportTickets();
    }

    public void FindTickets()
    {
        string strKeyword = txtSearchStarting.Text;
        short numLanguageID = ddlLanguages.SelectedValue();
        short numAssignedID = ddlAssignedLogin.SelectedValue();
        short numTypeID = ddlTicketType.SelectedValue();
        char chrStatus = ddlTicketStatus.SelectedValue();

        int numUserID = -1;
        string strUserEmail = "";

        if (!string.IsNullOrEmpty(txtUser.Text))
        {
            if (IsNumeric(txtUser.Text))
                numUserID = System.Convert.ToInt32(txtUser.Text);
            else
                strUserEmail = txtUser.Text;
        }
        litListingType.Text = "Search";

        DataTable tblResult = TicketsBLL._SearchTickets(strKeyword, numLanguageID, numAssignedID, numTypeID, numUserID, strUserEmail, chrStatus);

        if (tblResult.Rows.Count == 0)
        {
            gvwTickets.DataSource = null;
            gvwTickets.DataBind();
            gvwTickets.Visible = false;
            pnlNoTickets.Visible = true;
            pnlTicketColors.Visible = false;
            updTickets.Update();
            return;
        }
        else
            pnlNoTickets.Visible = false;


        tblResult.Columns.Add(new DataColumn("DateOpened", Type.GetType("System.String")));
        tblResult.Columns.Add(new DataColumn("LastMessage", Type.GetType("System.String")));

        foreach (DataRow drwResult in tblResult.Rows)
        {
            drwResult("DateOpened") = FormatDate(drwResult("TIC_DateOpened"), "d", Session("_LANG"));
            drwResult("LastMessage") = FormatDate(drwResult("LastMessageDate"), "t", Session("_LANG"));
            if (System.Convert.ToString(FixNullFromDB(drwResult("TIC_Subject"))).Length > 30)
                drwResult("TIC_Subject") = Left(drwResult("TIC_Subject"), 27) + "...";
        }
        gvwTickets.Visible = true;
        DataView dvwResults = tblResult.DefaultView;
        dvwResults.Sort = "TIC_DateOpened DESC";
        gvwTickets.DataSource = dvwResults.Table;
        gvwTickets.DataBind();
    }
}
