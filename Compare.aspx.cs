
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
// collect ID of item to process, ensure no non-int values for security
// 

// ' Reading the QueryString Key "action" to delete/add a product from/to the comparison.
// ' Session("ProductsToCompare") --> holds the IDs of the products (separated by commas) that need to be appeared in comparison,
// '   well, the productID must be rounded by "(" & ")" ... 
// ' To clear all comparison items
// ' To delete the product from the comparison list, we need to replace "(ProdID)" by ""
// ' To add a product to the comparison list you need to add comma followed by a Parentheses-rounded ProdID

// hide clear button if no items to compare

// ' If the product list ends with comma, then we need to remove that comma.
// ' If the product list starts with comma, then we need to remove that comma.

// ' Getting the product list without parentheses, to send the list to a stored procedure.

// ' If the list is empty, then exit

// ' Eliminates the spacing in the list.

// ' Gets the ProductIDs in an array of strings.

// ' Gets the attributes(if any) for comparison from the db.

// ' Getting the attributes that must be shown for if each product has this attribute.

// ' Getting the attributes that must be shown for if at least one product has this attribute.

// ' Getting the list of products that should be excluded from the comparison,
// '   to be sent to ProductCompare.ascx

// ' Loads the UC ProductCompare.ascx to show the comparison's attributes.
using KartSettingsManager;

namespace EmployeeManagementSystem
{
    class _failedMemberConversionMarker1
    {
    }
#error Cannot convert ClassBlockSyntax - see comment for details
    /* Cannot convert ClassBlockSyntax, CONVERSION ERROR: Object reference not set to an instance of an object. in 'Partial Class Compare\r\n I...' at character 563
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

    Partial Class Compare
        Inherits PageBaseClass

        Private c_tblProductsToCompare As New Global.System.Data.DataTable

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.Load
            If KartSettingsManager.GetKartConfig("frontend.products.comparison.enabled") = "y" Then
                Global.EmployeeManagementSystem.Page.Title = GetGlobalResourceObject("Products", "PageTitle_ProductComparision") & " | " & Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"))
                Me.DoCompare()
            Else
                mvwMain.SetActiveView(viwNotExist)
            End If
        End Sub

        Sub DoCompare()
            'collect ID of item to process, ensure no non-int values for security
            Dim intID As Integer = 0
            Try
                intID = Request.QueryString("id")
            Catch ex As Global.System.Exception
                '
            End Try

            '' Reading the QueryString Key "action" to delete/add a product from/to the comparison.
            '' Session("ProductsToCompare") --> holds the IDs of the products (separated by commas) that need to be appeared in comparison,
            ''   well, the productID must be rounded by "(" & ")" ... 
            Select Case Request.QueryString("action")
                Case "clear"
                    '' To clear all comparison items
                    Session("ProductsToCompare") = ""
                Case "del"
                    '' To delete the product from the comparison list, we need to replace "(ProdID)" by ""
                    Session("ProductsToCompare") = CStr(Session("ProductsToCompare")).Replace("(" & intID.ToString & ")", "")
                    Session("ProductsToCompare") = CStr(Session("ProductsToCompare")).Replace(",,", ",")
                Case "add"
                    '' To add a product to the comparison list you need to add comma followed by a Parentheses-rounded ProdID
                    Session("ProductsToCompare") += ",(" + intID.ToString + ")"
                Case Else
            End Select

            'hide clear button if no items to compare
            If Session("ProductsToCompare") = "" Then btnClearSession.Visible = False

            '' If the product list ends with comma, then we need to remove that comma.
            If CStr(Session("ProductsToCompare")).EndsWith(",") Then
                Session("ProductsToCompare") = CStr(Session("ProductsToCompare")).TrimEnd(",")
            End If
            '' If the product list starts with comma, then we need to remove that comma.
            If CStr(Session("ProductsToCompare")).StartsWith(",") Then
                Session("ProductsToCompare") = CStr(Session("ProductsToCompare")).Substring(1)
            End If


            Dim strProductsList As String = Session("ProductsToCompare")

            '' Getting the product list without parentheses, to send the list to a stored procedure.
            strProductsList = strProductsList.Replace("(", "")
            strProductsList = strProductsList.Replace(")", "")

            '' If the list is empty, then exit
            If strProductsList.Length = 0 Then Exit Sub

            '' Eliminates the spacing in the list.
            strProductsList = strProductsList.Replace(" ", "")

            '' Gets the ProductIDs in an array of strings.
            Dim arrProducts() As String = Global.Microsoft.VisualBasic.Strings.Split(strProductsList, ",")

            Dim numCGroupID As Short = 0
            If Global.System.Web.HttpContext.Current.User.Identity.IsAuthenticated Then
                numCGroupID = CShort(DirectCast(Page, PageBaseClass).CurrentLoggedUser.CustomerGroupID)
            End If

            '' Gets the attributes(if any) for comparison from the db.
            Me.c_tblProductsToCompare = KartrisDBBLL.GetProductsAttributesToCompare(strProductsList, Session("LANG"), numCGroupID)

            Dim strBldrNotIncludedAttributeList As New Global.System.Text.StringBuilder("")

            '' Getting the attributes that must be shown for if each product has this attribute.
            Dim drEach As Global.System.Data.DataRow()
            drEach = Me.c_tblProductsToCompare.[Select]("ATTRIB_Compare='e'")
            For iEach As Integer = 0 To drEach.Length - 1
                For Each row In Me.c_tblProductsToCompare.Rows
                    Dim dr As Global.System.Data.DataRow()
                    dr = Me.c_tblProductsToCompare.[Select]("P_ID=" & row("P_ID") & "AND ATTRIB_ID=" & drEach(iEach)("ATTRIB_ID"))
                    If dr.Length = 0 Then
                        strBldrNotIncludedAttributeList.Append(CStr(drEach(iEach)("ATTRIB_ID")))
                        strBldrNotIncludedAttributeList.Append(",")
                        Exit For
                    End If
                Next
            Next

            '' Getting the attributes that must be shown for if at least one product has this attribute.
            Dim blnAttributeExist As Boolean = False
            Dim drOnlyOne As Global.System.Data.DataRow()
            drOnlyOne = Me.c_tblProductsToCompare.[Select]("ATTRIB_Compare='o'")
            For iOnlyOne As Integer = 0 To drOnlyOne.Length - 1
                If Not drOnlyOne(iOnlyOne)("ATTRIBV_Value") Is Global.System.DBNull.Value Then
                    blnAttributeExist = True
                    Exit For
                End If
            Next
            If blnAttributeExist = False AndAlso drOnlyOne.Length > 0 Then
                strBldrNotIncludedAttributeList.Append(CStr(drOnlyOne(0)("ATTRIB_ID")))
                strBldrNotIncludedAttributeList.Append(",")
            End If

            '' Getting the list of products that should be excluded from the comparison,
            ''   to be sent to ProductCompare.ascx
            Dim arExcludedAttributeList As String() = New String() {""}
            If strBldrNotIncludedAttributeList.ToString.Contains(",") Then
                strBldrNotIncludedAttributeList.Remove(strBldrNotIncludedAttributeList.ToString.LastIndexOf(","), 1)
            End If
            arExcludedAttributeList = strBldrNotIncludedAttributeList.ToString.Split(",")

            '' Loads the UC ProductCompare.ascx to show the comparison's attributes.
            UC_ProductComparison.LoadProductComparison(Me.c_tblProductsToCompare, arExcludedAttributeList)

        End Sub
        Protected Sub btnClearSession_Click(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles btnClearSession.Click
            Response.Redirect(CkartrisBLL.WebShopURL & "Compare.aspx?action=clear")
        End Sub
    End Class

     */
}