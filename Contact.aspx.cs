using CkartrisBLL;
using CkartrisDataManipulation; // ' Clear the form for new reviews.
                                // ' Activates the Result View.
using CkartrisDisplayFunctions;
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
using KartSettingsManager;

namespace EmployeeManagementSystem
{
    class _failedMemberConversionMarker1
    {
    }
#error Cannot convert ClassBlockSyntax - see comment for details
    /* Cannot convert ClassBlockSyntax, CONVERSION ERROR: Object reference not set to an instance of an object. in 'Partial Class contact\r\n I...' at character 691
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

    Partial Class contact
        Inherits PageBaseClass

        Shared Basket As New kartris.Basket
        Shared BasketItems As Global.System.Collections.Generic.List(Of Kartris.BasketItem)

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.Load
            Global.EmployeeManagementSystem.Page.Title = Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "PageTitle_ContactUs") & " | " & MyBase.Server.HtmlEncode(Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "Config_Webshopname"))
            If Not Global.EmployeeManagementSystem.Page.IsPostBack Then
                If GetKartConfig("frontend.cataloguemode") = "y" Then
                    chkIncludeItems.Checked = False
                    chkIncludeItems.Visible = False
                End If
            End If
        End Sub

        Protected Sub btnSendMessage_Click(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles btnSendMessage.Click
            Global.EmployeeManagementSystem.Page.Validate()
            If Global.EmployeeManagementSystem.Page.IsValid AndAlso ajaxNoBotContact.IsValid Then

                Dim strTo As String = LanguagesBLL.GetEmailToContact(GetLanguageIDfromSession)
                Dim strBody As String = Me.CreateMessageBody()
                Dim strFrom As String = ""
                If GetKartConfig("general.email.spoofcontactmail") = "y" Then strFrom = txtEmail.Text Else strFrom = LanguagesBLL.GetEmailFrom(GetLanguageIDfromSession)
                If SendEmail(strFrom, strTo, Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "PageTitle_ContactUs") & " - " & txtName.Text, strBody, txtEmail.Text, txtName.Text) Then
                    litResult.Text = Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "ContentText_MailWasSent")
                    Me.ClearForm()
                Else
                    litResult.Text = Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "ContentText_Error")
                End If

                Me.ClearForm() '' Clear the form for new reviews.
                mvwWriting.SetActiveView(viwWritingResult)   '' Activates the Result View.

            End If
            updMain.Update()
        End Sub

        Function CreateMessageBody() As String
            Dim strBldr As New Global.System.Text.StringBuilder("")
            strBldr.Append(Global.System.Web.HttpContext.GetGlobalResourceObject("Email", "EmailText_ContactStart") & " " & Global.System.Web.HttpContext.Current.Request.Url.ToString & Global.Microsoft.VisualBasic.Constants.vbCrLf)
            strBldr.Append(Global.System.Web.HttpContext.GetGlobalResourceObject("Email", "EmailText_OrderEmailBreaker"))
            strBldr.Append(Global.System.Web.HttpContext.GetGlobalResourceObject("Email", "EmailText_ContactName") & txtName.Text & Global.Microsoft.VisualBasic.Constants.vbCrLf)
            strBldr.Append(Global.System.Web.HttpContext.GetGlobalResourceObject("Email", "EmailText_ContactEmail") & txtEmail.Text & Global.Microsoft.VisualBasic.Constants.vbCrLf)
            strBldr.Append(Global.System.Web.HttpContext.GetGlobalResourceObject("Email", "EmailText_ContactIP") & CkartrisEnvironment.GetClientIPAddress() & Global.Microsoft.VisualBasic.Constants.vbCrLf)
            strBldr.Append(Global.System.Web.HttpContext.GetGlobalResourceObject("Email", "EmailText_ContactDateStamp") & Global.Microsoft.VisualBasic.DateAndTime.Now.ToString & Global.Microsoft.VisualBasic.Constants.vbCrLf)
            strBldr.Append(Global.System.Web.HttpContext.GetGlobalResourceObject("Email", "EmailText_OrderEmailBreaker"))
            strBldr.Append(txtMessage.Text)
            If chkIncludeItems.Checked Then strBldr.Append(Me.GetBasket())
            Return strBldr.ToString
        End Function

        Function GetBasket() As String
            Dim _Item As BasketItem
            Global.EmployeeManagementSystem.contact.Basket.LoadBasketItems()
            Global.EmployeeManagementSystem.contact.BasketItems = Global.EmployeeManagementSystem.contact.Basket.BasketItems
            If Global.EmployeeManagementSystem.contact.BasketItems.Count = 0 Then Return Global.Microsoft.VisualBasic.Constants.vbCrLf
            Dim strBldrItems As New Global.System.Text.StringBuilder("")
            strBldrItems.Append(Global.Microsoft.VisualBasic.Constants.vbCrLf & Global.System.Web.HttpContext.GetGlobalResourceObject("Email", "EmailText_OrderEmailBreaker"))
            strBldrItems.Append(Global.System.Web.HttpContext.GetGlobalResourceObject("Email", "EmailText_ContactBasketContents") & Global.Microsoft.VisualBasic.Constants.vbCrLf)
            For i As Integer = 0 To Global.EmployeeManagementSystem.contact.BasketItems.Count - 1
                _Item = Global.EmployeeManagementSystem.contact.BasketItems(i)
                strBldrItems.Append(_Item.Quantity & " X " & _Item.ProductName & " - " & _Item.VersionName & " (" & _Item.VersionCode & ")" & Global.Microsoft.VisualBasic.Constants.vbCrLf)
            Next
            strBldrItems.Append(Global.System.Web.HttpContext.GetGlobalResourceObject("Email", "EmailText_OrderEmailBreaker"))
            Return strBldrItems.ToString
        End Function

        Sub ClearForm()
            txtName.Text = String.Empty
            txtEmail.Text = String.Empty
            txtMessage.Text = String.Empty
        End Sub

        Protected Sub btnBack_Click(ByVal sender As Object, ByVal e As Global.System.EventArgs)
            mvwWriting.SetActiveView(viwWritingForm)
        End Sub

    End Class

     */
}