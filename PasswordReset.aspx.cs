

// Only update if validators ok
using CkartrisBLL;
// ========================================================================
// Kartris - www.kartris.com
// Copyright 2018 CACTUSOFT

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

namespace EmployeeManagementSystem
{
    class _failedMemberConversionMarker1
    {
    }

#error Cannot convert ClassBlockSyntax - see comment for details
    /* Cannot convert ClassBlockSyntax, CONVERSION ERROR: Object reference not set to an instance of an object. in 'Partial Class PasswordReset...' at character 572
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

        Partial Class PasswordReset
            Inherits PageBaseClass

            Private Sub Page_Load(ByVal sender As Global.System.[Object], ByVal e As Global.System.EventArgs) Handles Me.Load


                If Not String.IsNullOrEmpty(Request.QueryString("ref")) Then
                    lblCurrentPassword.Text = GetGlobalResourceObject("Kartris", "FormLabel_EmailAddress")
                    txtCurrentPassword.TextMode = Global.System.Web.UI.WebControls.TextBoxMode.SingleLine
                    Dim strRef As String = Request.QueryString("ref")

                Else
                    Response.Redirect(WebShopURL() & "CustomerAccount.aspx")
                End If

            End Sub



            Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles btnSubmit.Click
                Dim objUsersBLL As New UsersBLL
                If String.IsNullOrEmpty(Request.QueryString("ref")) Then
                    Dim strUserPassword As String = txtCurrentPassword.Text
                    Dim strNewPassword As String = txtNewPassword.Text

                    'Only update if validators ok
                    If Me.IsValid Then
                        If Global.System.Web.Security.Membership.ValidateUser(CurrentLoggedUser.Email, strUserPassword) Then
                            If objUsersBLL.ChangePassword(CurrentLoggedUser.ID, strUserPassword, strNewPassword) > 0 Then UC_Updated.ShowAnimatedText()
                        Else
                            Dim strErrorMessage As String = Replace(GetGlobalResourceObject("Kartris", "ContentText_CustomerCodeIncorrect"), "[label]", GetLocalResourceObject("FormLabel_ExistingCustomerCode"))
                            litWrongPassword.Text = "<span class=""error"">" & strErrorMessage & "</span>"
                        End If
                    End If

                Else
                    Dim strRef As String = Request.QueryString("ref")
                    Dim strEmailAddress As String = txtCurrentPassword.Text

                    Dim dtbUserDetails As Global.System.Data.DataTable = objUsersBLL.GetDetails(strEmailAddress)
                    If dtbUserDetails.Rows.Count > 0 Then
                        Dim intUserID As Integer = dtbUserDetails(0)("U_ID")
                        Dim strTempPassword As String = FixNullFromDB(dtbUserDetails(0)("U_TempPassword"))
                        Dim datExpiry As Global.System.DateTime = IIf(IsDate(FixNullFromDB(dtbUserDetails(0)("U_TempPasswordExpiry"))), dtbUserDetails(0)("U_TempPasswordExpiry"),
                                                    CkartrisDisplayFunctions.NowOffset.AddMinutes(-1))
                        If String.IsNullOrEmpty(strTempPassword) Then datExpiry = CkartrisDisplayFunctions.NowOffset.AddMinutes(-1)

                        If datExpiry > CkartrisDisplayFunctions.NowOffset Then
                            If objUsersBLL.EncryptSHA256Managed(strTempPassword, objUsersBLL.GetSaltByEmail(strEmailAddress)) = strRef Then
                                Dim intSuccess As Integer = objUsersBLL.ChangePasswordViaRecovery(intUserID, txtConfirmPassword.Text)
                                If intSuccess = intUserID Then
                                    UC_Updated.ShowAnimatedText()
                                    Response.Redirect(WebShopURL() & "CustomerAccount.aspx?m=u")
                                Else
                                    litWrongPassword.Text = "<span class=""error"">" & GetGlobalResourceObject("Kartris", "ContentText_ErrorText") & "</span>"
                                End If
                            Else
                                litWrongPassword.Text = "<span class=""error"">" & GetGlobalResourceObject("Kartris", "ContentText_LinkExpiredOrIncorrect") & "</span>"
                            End If

                        Else
                            litWrongPassword.Text = "<span class=""error"">" & GetGlobalResourceObject("Kartris", "ContentText_LinkExpiredOrIncorrect") & "</span>"
                        End If

                    Else
                        litWrongPassword.Text = "<span class=""error"">" & GetGlobalResourceObject("Kartris", "ContentText_NotFoundInDB") & "</span>"
                    End If
                End If

            End Sub

        End Class
         */
}