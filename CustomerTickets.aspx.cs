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

namespace EmployeeManagementSystem
{
    class _failedMemberConversionMarker1
    {
    }
#error Cannot convert ClassBlockSyntax - see comment for details
    /* Cannot convert ClassBlockSyntax, CONVERSION ERROR: Object reference not set to an instance of an object. in 'Partial Class CustomerTicke...' at character 623
       at ICSharpCode.CodeConverter.CSharp.HandledEventsAnalyzer.CreateEventContainer(EventContainerSyntax p, SemanticModel semanticModel)
       at ICSharpCode.CodeConverter.CSharp.HandledEventsAnalyzer.<>c__DisplayClass11_0.<HandledEvent>b__2(MethodStatementSyntax _, HandlesClauseItemSyntax e)
       at System.Linq.Enumerable.<SelectManyIterator>d__23`3.MoveNext()
       at System.Linq.Enumerable.<SelectManyIterator>d__17`2.MoveNext()
       at System.Linq.Enumerable.<SelectManyIterator>d__17`2.MoveNext()
       at System.Linq.Lookup`2.Create[TSource](IEnumerable`1 source, Func`2 keySelector, Func`2 elementSelector, IEqualityComparer`1 comparer)
       at ICSharpCode.CodeConverter.CSharp.HandledEventsAnalyzer.<AnalyzeAsync>d__7.MoveNext()
    --- End of stack trace from previous location where exception was thrown ---
       at ICSharpCode.CodeConverter.CSharp.DeclarationNodeVisitor.<GetMethodWithHandlesAsync>d__63.MoveNext()
    --- End of stack trace from previous location where exception was thrown ---
       at ICSharpCode.CodeConverter.CSharp.DeclarationNodeVisitor.<ConvertMembersAsync>d__40.MoveNext()
    --- End of stack trace from previous location where exception was thrown ---
       at ICSharpCode.CodeConverter.CSharp.DeclarationNodeVisitor.<VisitClassBlock>d__46.MoveNext()
    --- End of stack trace from previous location where exception was thrown ---
       at ICSharpCode.CodeConverter.CSharp.CommentConvertingVisitorWrapper.<ConvertHandledAsync>d__8`1.MoveNext()

    Input:

    Partial Class CustomerTickets
        Inherits PageBaseClass

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.Load
            If KartSettingsManager.GetKartConfig("frontend.supporttickets.enabled") = "y" Then
                Dim TIC_ID As Integer = 0
                If User.Identity.IsAuthenticated Then
                    If Not Global.EmployeeManagementSystem.Page.IsPostBack Then
                        Try
                            TIC_ID = Request.QueryString("TIC_ID")
                        Catch ex As Global.System.Exception
                            TIC_ID = 0
                        End Try
                        If TIC_ID > 0 Then
                            UC_TicketDetails.ViewTicketDetails(TIC_ID)
                            mvwTickets.SetActiveView(viwTicketDetails)
                            updMain.Update()
                        Else
                            mvwTickets.ActiveViewIndex = 1
                            Me.LoadUserTickets()
                        End If
                        If DirectCast(Page, PageBaseClass).CurrentLoggedUser.isSupportValid Then
                            Dim strMessage As String = GetGlobalResourceObject("Tickets", "ContentText_SupportExpiresMessage")
                            strMessage = strMessage.Replace("[date]", FormatDate(DirectCast(Page, PageBaseClass).CurrentLoggedUser.SupportEndDate, "d", Session("Lang")))
                            lblSupportExpirationMessage.Text = strMessage : lblSupportExpirationMessage.Visible = True
                            lblSupportExpirationMessage.CssClass = "expirywarning"
                        ElseIf DirectCast(Page, PageBaseClass).CurrentLoggedUser.SupportEndDate <> Nothing AndAlso _
                                DirectCast(Page, PageBaseClass).CurrentLoggedUser.SupportEndDate <> "#12:00:00 AM#" Then
                            Dim strMessage As String = GetGlobalResourceObject("Tickets", "ContentText_SupportExpiredMessage")
                            strMessage = strMessage.Replace("[date]", FormatDate(DirectCast(Page, PageBaseClass).CurrentLoggedUser.SupportEndDate, "d", Session("Lang")))
                            lblSupportExpirationMessage.Text = strMessage : lblSupportExpirationMessage.Visible = True
                            lblSupportExpirationMessage.CssClass = "expiredwarning"
                        Else
                            lblSupportExpirationMessage.Text = "" : lblSupportExpirationMessage.Visible = False
                        End If
                    End If
                Else
                    mvwTickets.ActiveViewIndex = 0
                End If
            Else
                mvwMain.SetActiveView(viwNotExist)
            End If
        End Sub

        Protected Sub UC_WriteTicket_WritingFinished() Handles UC_WriteTicket.WritingFinished
            Me.LoadUserTickets()
            mvwTickets.SetActiveView(viwTickets)
            updMain.Update()
        End Sub

        Sub LoadUserTickets()
            Dim tblUserTickets As Global.System.Data.DataTable = TicketsBLL.GetSupportTicketsByUser(CurrentLoggedUser.ID)
            tblUserTickets.Columns.Add(New Global.System.Data.DataColumn("DateOpened", Global.System.[Type].[GetType]("System.String")))
            tblUserTickets.Columns.Add(New Global.System.Data.DataColumn("DateClosed", Global.System.[Type].[GetType]("System.String")))
            For Each drwTicket As Global.System.Data.DataRow In tblUserTickets.Rows
                If Not drwTicket("TIC_DateOpened") Is Global.System.DBNull.Value Then
                    drwTicket("DateOpened") = FormatDate(drwTicket("TIC_DateOpened"), "d", Session("LANG"))
                End If
                If Not drwTicket("TIC_DateClosed") Is Global.System.DBNull.Value Then
                    drwTicket("DateClosed") = FormatDate(drwTicket("TIC_DateClosed"), "d", Session("LANG"))
                Else
                    drwTicket("DateClosed") = GetGlobalResourceObject("Tickets", "ContentText_TicketIsOpen")
                End If
                If CStr(FixNullFromDB(drwTicket("TIC_Subject"))).Length > 38 Then drwTicket("TIC_Subject") = Global.Microsoft.VisualBasic.Strings.Left(drwTicket("TIC_Subject"), 35) & "..."
            Next
            If tblUserTickets.Rows.Count > gvwTickets.PageSize Then
                gvwTickets.AllowPaging = True
            Else
                gvwTickets.AllowPaging = False
            End If
            Dim dvwTickets As Global.System.Data.DataView = tblUserTickets.DefaultView
            dvwTickets.Sort = "TIC_DateOpened DESC"
            gvwTickets.DataSource = dvwTickets.Table
            gvwTickets.DataBind()
        End Sub

        Protected Sub btnOpenTicket_Click(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles btnOpenTicket.Click
            UC_WriteTicket.OpenNewTicket(CurrentLoggedUser.ID)
            mvwTickets.SetActiveView(viwWriteTicket)
            updMain.Update()
        End Sub
        Function FormatURL(ByVal TIC_ID As Integer) As String
            Return "CustomerTickets.aspx?TIC_ID=" & TIC_ID.ToString
        End Function

        Protected Sub gvwTickets_RowDataBound(ByVal sender As Object, ByVal e As Global.System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvwTickets.RowDataBound
            If e.Row.RowType = Global.System.Web.UI.WebControls.DataControlRowType.DataRow Then
                If CBool(CType(e.Row.Cells(4).FindControl("litAwaitingReply"), Global.System.Web.UI.WebControls.Literal).[Text]) Then
                    If e.Row.RowState = Global.System.Web.UI.WebControls.DataControlRowState.Alternate Then
                        e.Row.CssClass = "sp_highlight_ticket"
                    Else
                        e.Row.CssClass = "sp_highlight_ticket"
                    End If
                End If
            End If
        End Sub
    End Class
     */
}