using System;
using System.Linq;
using CkartrisBLL;
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
using CkartrisDataManipulation;
using CkartrisDisplayFunctions;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic

internal partial class CustomerTickets : PageBaseClass
{
    public CustomerTickets()
    {
        this.Load += Page_Load;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (KartSettingsManager.GetKartConfig("frontend.supporttickets.enabled") == "y")
        {
            int TIC_ID = 0;
            if (User.Identity.IsAuthenticated)
            {
                if (!Page.IsPostBack)
                {
                    try
                    {
                        TIC_ID = Request.QueryString("TIC_ID");
                    }
                    catch (Exception ex)
                    {
                        TIC_ID = 0;
                    }
                    if (TIC_ID > 0)
                    {
                        UC_TicketDetails.ViewTicketDetails(TIC_ID);
                        mvwTickets.SetActiveView(viwTicketDetails);
                        updMain.Update();
                    }
                    else
                    {
                        mvwTickets.ActiveViewIndex = 1;
                        LoadUserTickets();
                    }
                    if (((PageBaseClass)Page).CurrentLoggedUser.isSupportValid)
                    {
                        string strMessage = GetGlobalResourceObject("Tickets", "ContentText_SupportExpiresMessage");
                        strMessage = strMessage.Replace("[date]", FormatDate(((PageBaseClass)Page).CurrentLoggedUser.SupportEndDate, "d", Session("Lang")));
                        lblSupportExpirationMessage.Text = strMessage;
                        lblSupportExpirationMessage.Visible = true;
                        lblSupportExpirationMessage.CssClass = "expirywarning";
                    }
                    else if (((PageBaseClass)Page).CurrentLoggedUser.SupportEndDate != default && ((PageBaseClass)Page).CurrentLoggedUser.SupportEndDate != "#12:00:00 AM#")
                    {
                        string strMessage = GetGlobalResourceObject("Tickets", "ContentText_SupportExpiredMessage");
                        strMessage = strMessage.Replace("[date]", FormatDate(((PageBaseClass)Page).CurrentLoggedUser.SupportEndDate, "d", Session("Lang")));
                        lblSupportExpirationMessage.Text = strMessage;
                        lblSupportExpirationMessage.Visible = true;
                        lblSupportExpirationMessage.CssClass = "expiredwarning";
                    }
                    else
                    {
                        lblSupportExpirationMessage.Text = "";
                        lblSupportExpirationMessage.Visible = false;
                    }
                }
            }
            else
            {
                mvwTickets.ActiveViewIndex = 0;
            }
        }
        else
        {
            mvwMain.SetActiveView(viwNotExist);
        }
    }

    protected void UC_WriteTicket_WritingFinished()
    {
        LoadUserTickets();
        mvwTickets.SetActiveView(viwTickets);
        updMain.Update();
    }

    public void LoadUserTickets()
    {
        DataTable tblUserTickets = TicketsBLL.GetSupportTicketsByUser(CurrentLoggedUser.ID);
        tblUserTickets.Columns.Add(new DataColumn("DateOpened", Type.GetType("System.String")));
        tblUserTickets.Columns.Add(new DataColumn("DateClosed", Type.GetType("System.String")));
        foreach (DataRow drwTicket in tblUserTickets.Rows)
        {
            if (!object.ReferenceEquals(drwTicket("TIC_DateOpened"), DBNull.Value))
            {
                drwTicket("DateOpened") = FormatDate(drwTicket("TIC_DateOpened"), "d", Session("LANG"));
            }
            if (!object.ReferenceEquals(drwTicket("TIC_DateClosed"), DBNull.Value))
            {
                drwTicket("DateClosed") = FormatDate(drwTicket("TIC_DateClosed"), "d", Session("LANG"));
            }
            else
            {
                drwTicket("DateClosed") = GetGlobalResourceObject("Tickets", "ContentText_TicketIsOpen");
            }
            if (FixNullFromDB(drwTicket("TIC_Subject")).Length > 38)
                drwTicket("TIC_Subject") = Strings.Left(drwTicket("TIC_Subject"), 35) + "...";
        }
        if (tblUserTickets.Rows.Count > gvwTickets.PageSize)
        {
            gvwTickets.AllowPaging = true;
        }
        else
        {
            gvwTickets.AllowPaging = false;
        }
        DataView dvwTickets = tblUserTickets.DefaultView;
        dvwTickets.Sort = "TIC_DateOpened DESC";
        gvwTickets.DataSource = dvwTickets.Table;
        gvwTickets.DataBind();
    }

    protected void btnOpenTicket_Click(object sender, EventArgs e)
    {
        UC_WriteTicket.OpenNewTicket(CurrentLoggedUser.ID);
        mvwTickets.SetActiveView(viwWriteTicket);
        updMain.Update();
    }
    public string FormatURL(int TIC_ID)
    {
        return "CustomerTickets.aspx?TIC_ID=" + TIC_ID.ToString();
    }

    protected void gvwTickets_RowDataBound(object sender, Global.System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((Literal)e.Row.Cells(4).FindControl("litAwaitingReply")).Text)
            {
                if (e.Row.RowState == DataControlRowState.Alternate)
                {
                    e.Row.CssClass = "sp_highlight_ticket";
                }
                else
                {
                    e.Row.CssClass = "sp_highlight_ticket";
                }
            }
        }
    }
}