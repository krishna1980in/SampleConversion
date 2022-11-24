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

partial class Templates_WriteTicketTemplate : System.Web.UI.UserControl
{
    public event WritingFinishedEventHandler WritingFinished;

    public delegate void WritingFinishedEventHandler();

    private int GetUserID()
    {
        if (!string.IsNullOrEmpty(litUserID.Text) && IsNumeric(litUserID.Text))
            return System.Convert.ToInt32(litUserID.Text);
        litUserID.Text = "0";
        return 0;
    }

    public void OpenNewTicket(int numUserID)
    {
        litUserID.Text = numUserID;
        txtSubject.Text = null;
        txtTicketMessage.Text = null;
        mvwWriting.SetActiveView(viwWritingForm);
        updMain.Update();
    }

    protected void btnAddTicket_Click(object sender, System.EventArgs e)
    {
        AddNewTicket();
    }

    public void AddNewTicket()
    {
        if (GetUserID() != 0)
        {
            string strSubject = txtSubject.Text;
            string strTicketMessage = txtTicketMessage.Text;
            int numTicketType = ddlTicketType.SelectedValue;
            string strMsg = null;
            int intNewTicketID = TicketsBLL.AddSupportTicket(GetUserID, numTicketType, strSubject, strTicketMessage, strMsg);
            if (intNewTicketID > 0)
            {
                DataTable tblTicketLogins = LoginsBLL.GetSupportUsers();
                string strAdminEmail = "";
                System.Net.Mail.MailAddressCollection BccAddresses = null;

                // ticket is unassigned - send to all support admins
                foreach (DataRow drwLogins in tblTicketLogins.Rows)
                {
                    string strTempEmail = FixNullFromDB(drwLogins("LOGIN_EmailAddress"));
                    if (!string.IsNullOrEmpty(strTempEmail))
                    {
                        if (string.IsNullOrEmpty(strAdminEmail))
                            strAdminEmail = strTempEmail;
                        else
                        {
                            if (BccAddresses == null)
                                BccAddresses = new System.Net.Mail.MailAddressCollection();
                            BccAddresses.Add(new System.Net.Mail.MailAddress(strTempEmail));
                        }
                    }
                }

                // insert ticket # to the email subject
                string strEmailSubject = Strings.Replace(System.Web.UI.TemplateControl.GetGlobalResourceObject("Tickets", "Config_Subjectline6"), "[ticketno]", intNewTicketID);

                // append the backend ticket link to the ticket email text
                strTicketMessage = strSubject + Constants.vbCrLf + Constants.vbCrLf + strTicketMessage + Constants.vbCrLf + Constants.vbCrLf + CkartrisBLL.WebShopURL + "Admin/_SupportTickets.aspx?tic_id=" + intNewTicketID;

                SendEmail(LanguagesBLL.GetEmailFrom(CkartrisBLL.GetLanguageIDfromSession), strAdminEmail, strEmailSubject, strTicketMessage, null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, BccAddresses);

                // Send a support notification request to Windows Store App if enabled         
                CkartrisBLL.PushKartrisNotification("s");

                litResult.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("Tickets", "ContentText_TicketSuccessfullySubmited");
            }
            else
                litResult.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_Error");
            mvwWriting.SetActiveView(viwWritingResult);
            updMain.Update();
        }
    }

    protected void btnCancel_Click(object sender, System.EventArgs e)
    {
        WritingFinished?.Invoke();
    }

    protected void btnBack_Click(object sender, System.EventArgs e)
    {
        WritingFinished?.Invoke();
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!System.Web.UI.Control.Page.IsPostBack)
        {
            if (System.Web.UI.Control.Page.User.Identity.IsAuthenticated)
                FillLevels();
        }
    }

    public void FillLevels()
    {
        DataTable tblTicketTypes = TicketsBLL._GetTicketTypes();
        DataView dvwTicketTypes = tblTicketTypes.DefaultView;
        if (!(PageBaseClass)System.Web.UI.Control.Page.CurrentLoggedUser.isSupportValid)
            dvwTicketTypes.RowFilter = "STT_Level = 's'";
        ddlTicketType.DataTextField = "STT_Name";
        ddlTicketType.DataValueField = "STT_ID";
        ddlTicketType.DataSource = dvwTicketTypes;
        ddlTicketType.DataBind();
    }
}
