
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

// '// payments made

// '// unpaid sales
// '// activity hits
// Keep values in default currency, since that is what commission will be paid in
// Converting to another currency will mislead people
// Keep values in default currency, since that is what commission will be paid in
// Converting to another currency will mislead people
using CkartrisBLL; // 'activity

namespace EmployeeManagementSystem
{
    class _failedMemberConversionMarker1
    {
    }
#error Cannot convert ClassBlockSyntax - see comment for details
    /* Cannot convert ClassBlockSyntax, CONVERSION ERROR: Object reference not set to an instance of an object. in 'Partial Class CustomerAffil...' at character 555
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

    Partial Class CustomerAffiliates
        Inherits PageBaseClass

        Protected AFF_SalesTotal As Double
        Protected AFF_HitsTotal As Double
        Private UnpaidCount, PaymentCount, SalesLinkCount As Integer
        Private UnpaidTotal, PaymentTotal As Double
        Private objCurrency As New CurrenciesBLL

        Private Class AffiliateActivity

            Private _Share As Double
            Private _ActivityMonth As Integer
            Private _ActivityYear As Integer
            Private _GraphValue As Integer

            Public Property Share() As Double
                Get
                    Return Me._Share
                End Get
                Set(ByVal value As Double)
                    Me._Share = value
                End Set
            End Property

            Public Property ActivityMonth() As Integer
                Get
                    Return Me._ActivityMonth
                End Get
                Set(ByVal value As Integer)
                    Me._ActivityMonth = value
                End Set
            End Property

            Public Property ActivityYear() As Integer
                Get
                    Return Me._ActivityYear
                End Get
                Set(ByVal value As Integer)
                    Me._ActivityYear = value
                End Set
            End Property

            Public Property GraphValue() As Integer
                Get
                    Return Me._GraphValue
                End Get
                Set(ByVal value As Integer)
                    Me._GraphValue = value
                End Set
            End Property

        End Class

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.Load
            Dim strCallMode As String
            Dim numCustomerID As Integer

            If Not (User.Identity.IsAuthenticated) Then
                Response.Redirect("~/CustomerAccount.aspx")
            Else
                numCustomerID = CurrentLoggedUser.ID
            End If

            strCallMode = Request.QueryString("activity") & ""

            Select Case Global.Microsoft.VisualBasic.Strings.LCase(strCallMode)
                Case "monthly"
                    phdMonthly.Visible = True
                    phdApply.Visible = False
                    phdBalance.Visible = False
                    phdActivity.Visible = False

                    Dim numMonth, numYear As Integer
                    numMonth = Val(Request.QueryString("Month"))
                    numYear = Val(Request.QueryString("Year"))

                    If numMonth < 1 AndAlso numMonth > 12 Then numMonth = 0

                    If numMonth = 0 Or numYear = 0 Then
                        numMonth = Month(CkartrisDisplayFunctions.NowOffset)
                        numYear = Year(CkartrisDisplayFunctions.NowOffset)
                    ElseIf DateDiff("m", numYear & "/" & numMonth & "/1", Year(CkartrisDisplayFunctions.NowOffset) & "/" & Month(CkartrisDisplayFunctions.NowOffset) & "/1") < 0 Or DateDiff("m", numYear & "/" & numMonth & "/1", DateAdd("m", -11, Year(CkartrisDisplayFunctions.NowOffset) & "/" & Month(CkartrisDisplayFunctions.NowOffset) & "/1")) > 0 Then
                        numMonth = Month(CkartrisDisplayFunctions.NowOffset)
                        numYear = Year(CkartrisDisplayFunctions.NowOffset)
                    End If

                    litAffMonthly_Date.Text = Global.Microsoft.VisualBasic.DateAndTime.MonthName(numMonth) & " " & numYear

                    Dim objCurrency As New CurrenciesBLL
                    Dim numTotalPrice, numTotalCommission, numTotalHits As Double
                    Dim dtbCommission As Global.System.Data.DataTable
                    Dim Basket As New kartris.Basket

                    dtbCommission = AffiliateBLL.GetCustomerAffiliateCommission(numCustomerID, numMonth, numYear)
                    If dtbCommission.Rows.Count > 0 Then
                        numTotalPrice = dtbCommission.Rows(0).Item("OrderTotal")
                        numTotalCommission = dtbCommission.Rows(0).Item("Commission")
                        numTotalHits = dtbCommission.Rows(0).Item("Hits")
                    End If
                    litAffMonthly_TotalPrice.Text = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency(), numTotalPrice)
                    litAffMonthly_TotalCommission.Text = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency(), numTotalCommission)
                    litAffMonthly_TotalHits.Text = numTotalHits

                    dtbCommission = AffiliateBLL.GetCustomerAffiliateSalesLink(numCustomerID, numMonth, numYear)

                    Me.SalesLinkCount = 0
                    rptAffiliateSalesLink.DataSource = dtbCommission
                    rptAffiliateSalesLink.DataBind()


                Case "apply"
                    phdMonthly.Visible = False
                    phdApply.Visible = True
                    phdBalance.Visible = False
                    phdActivity.Visible = False
                    Dim OldBasketBLL As New kartris.Basket
                    Call AffiliateBLL.UpdateCustomerAffiliateStatus(numCustomerID)

                Case "balance"
                    phdMonthly.Visible = False
                    phdApply.Visible = False
                    phdBalance.Visible = True
                    phdActivity.Visible = False

                    Dim dtbPayments As New Global.System.Data.DataTable
                    Dim Basket As New kartris.Basket

                    ''// payments made
                    dtbPayments = AffiliateBLL.GetCustomerAffiliatePayments(numCustomerID)

                    Me.PaymentCount = 0 : Me.PaymentTotal = 0
                    rptAffPayments.DataSource = dtbPayments
                    rptAffPayments.DataBind()

                    ''// unpaid sales
                    dtbPayments = AffiliateBLL.GetCustomerAffiliateUnpaidSales(numCustomerID)

                    Me.UnpaidCount = 0 : Me.UnpaidTotal = 0
                    rptAffiliateUnpaid.DataSource = dtbPayments
                    rptAffiliateUnpaid.DataBind()

                Case Else ''activity
                    phdMonthly.Visible = False
                    phdApply.Visible = False
                    phdBalance.Visible = False
                    phdActivity.Visible = True

                    Dim aryAffiliateSales, aryAffiliateHits As New Global.System.Collections.ArrayList
                    Dim dtbActivity As New Global.System.Data.DataTable
                    Dim Basket As New kartris.Basket
                    Dim ActivityDate As Date = CkartrisDisplayFunctions.NowOffset
                    Dim numMaxValue As Integer

                    aryAffiliateSales.Clear()
                    For i As Integer = 1 To 12
                        Dim objAff_Activity As New Global.EmployeeManagementSystem.CustomerAffiliates.AffiliateActivity
                        objAff_Activity.ActivityMonth = Global.Microsoft.VisualBasic.DateAndTime.Month(ActivityDate)
                        objAff_Activity.ActivityYear = Global.Microsoft.VisualBasic.DateAndTime.Year(ActivityDate)
                        objAff_Activity.Share = 0
                        objAff_Activity.GraphValue = 1
                        aryAffiliateSales.Add(objAff_Activity)
                        ActivityDate = Global.Microsoft.VisualBasic.DateAndTime.DateAdd(Global.Microsoft.VisualBasic.DateInterval.Month, -1, ActivityDate)
                    Next

                    numMaxValue = 0
                    dtbActivity = AffiliateBLL.GetCustomerAffiliateActivitySales(numCustomerID)
                    For i As Integer = 1 To dtbActivity.Rows.Count
                        For Each objItem As Global.EmployeeManagementSystem.CustomerAffiliates.AffiliateActivity In aryAffiliateSales
                            If dtbActivity.Rows(i - 1).Item("TheMonth") = objItem.ActivityMonth AndAlso dtbActivity.Rows(i - 1).Item("TheYear") = objItem.ActivityYear Then
                                objItem.Share = dtbActivity.Rows(i - 1).Item("OrderAmount")
                                If objItem.Share > numMaxValue Then numMaxValue = objItem.Share
                            End If
                        Next
                    Next

                    Me.AFF_SalesTotal = 0
                    For Each objItem As Global.EmployeeManagementSystem.CustomerAffiliates.AffiliateActivity In aryAffiliateSales
                        If numMaxValue > 0 Then
                            objItem.GraphValue = (objItem.Share / numMaxValue) * 100
                        End If
                        Me.AFF_SalesTotal = Me.AFF_SalesTotal + objItem.Share
                    Next

                    rptAffiliateActivitySales.DataSource = aryAffiliateSales
                    rptAffiliateActivitySales.DataBind()

                    ActivityDate = CkartrisDisplayFunctions.NowOffset
                    ''// activity hits
                    aryAffiliateHits.Clear()
                    For i As Integer = 1 To 12
                        Dim objAff_Activity As New Global.EmployeeManagementSystem.CustomerAffiliates.AffiliateActivity
                        objAff_Activity.ActivityMonth = Global.Microsoft.VisualBasic.DateAndTime.Month(ActivityDate)
                        objAff_Activity.ActivityYear = Global.Microsoft.VisualBasic.DateAndTime.Year(ActivityDate)
                        objAff_Activity.Share = 0
                        objAff_Activity.GraphValue = 1
                        aryAffiliateHits.Add(objAff_Activity)
                        ActivityDate = Global.Microsoft.VisualBasic.DateAndTime.DateAdd(Global.Microsoft.VisualBasic.DateInterval.Month, -1, ActivityDate)
                    Next

                    numMaxValue = 0
                    dtbActivity = AffiliateBLL.GetCustomerAffiliateActivityHits(numCustomerID)
                    For i As Integer = 1 To dtbActivity.Rows.Count
                        For Each objItem As Global.EmployeeManagementSystem.CustomerAffiliates.AffiliateActivity In aryAffiliateHits
                            If dtbActivity.Rows(i - 1).Item("TheMonth") = objItem.ActivityMonth AndAlso dtbActivity.Rows(i - 1).Item("TheYear") = objItem.ActivityYear Then
                                objItem.Share = Global.System.Math.Round(dtbActivity.Rows(i - 1).Item("HitCount"))
                                If objItem.Share > numMaxValue Then numMaxValue = objItem.Share
                            End If
                        Next
                    Next

                    Me.AFF_HitsTotal = 0
                    For Each objItem As Global.EmployeeManagementSystem.CustomerAffiliates.AffiliateActivity In aryAffiliateHits
                        If numMaxValue > 0 Then
                            objItem.GraphValue = (objItem.Share / numMaxValue) * 100
                        End If
                        Me.AFF_HitsTotal = Me.AFF_HitsTotal + objItem.Share
                    Next

                    rptAffiliateActivityHits.DataSource = aryAffiliateHits
                    rptAffiliateActivityHits.DataBind()

            End Select

        End Sub

        Protected Sub rptAffiliateActivitySales_ItemDataBound(ByVal sender As Object, ByVal e As Global.System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptAffiliateActivitySales.ItemDataBound
            Dim objCurrency As New CurrenciesBLL
            Dim numShare As Double
            Dim numMonth, numYear As Integer
            Dim numDefaultCurrencyID, numCurrencyID As Integer
            Dim objItem As Global.EmployeeManagementSystem.CustomerAffiliates.AffiliateActivity

            numCurrencyID = Session("CUR_ID")
            numDefaultCurrencyID = CurrenciesBLL.GetDefaultCurrency()

            If (e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.Item) Or (e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.AlternatingItem) Then
                objItem = CType(e.Item.DataItem, Global.EmployeeManagementSystem.CustomerAffiliates.AffiliateActivity)
                numShare = objItem.Share
                numMonth = objItem.ActivityMonth
                numYear = objItem.ActivityYear
                'Keep values in default currency, since that is what commission will be paid in
                'Converting to another currency will mislead people
                CType(e.Item.FindControl("litSalesShare"), Global.System.Web.UI.WebControls.Literal).[Text] = CurrenciesBLL.FormatCurrencyPrice(numDefaultCurrencyID, numShare)
                CType(e.Item.FindControl("hypLnkMonth"), Global.System.Web.UI.WebControls.HyperLink).[Text] = Global.Microsoft.VisualBasic.DateAndTime.MonthName(numMonth) & " " & numYear
                CType(e.Item.FindControl("hypLnkMonth"), Global.System.Web.UI.WebControls.HyperLink).NavigateUrl = "CustomerAffiliates.aspx?activity=monthly&month=" & numMonth & "&year=" & numYear
            End If

            If e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.Footer Then
                'Keep values in default currency, since that is what commission will be paid in
                'Converting to another currency will mislead people
                CType(e.Item.FindControl("litSalesTotal"), Global.System.Web.UI.WebControls.Literal).[Text] = CurrenciesBLL.FormatCurrencyPrice(numDefaultCurrencyID, Me.AFF_SalesTotal)
            End If

        End Sub

        Protected Sub rptAffiliateActivityHits_ItemDataBound(ByVal sender As Object, ByVal e As Global.System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptAffiliateActivityHits.ItemDataBound
            Dim numShare As Double
            Dim numMonth, numYear As Integer
            Dim objItem As Global.EmployeeManagementSystem.CustomerAffiliates.AffiliateActivity

            If (e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.Item) Or (e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.AlternatingItem) Then
                objItem = CType(e.Item.DataItem, Global.EmployeeManagementSystem.CustomerAffiliates.AffiliateActivity)
                numShare = objItem.Share
                numMonth = objItem.ActivityMonth
                numYear = objItem.ActivityYear
                CType(e.Item.FindControl("litHitsShare"), Global.System.Web.UI.WebControls.Literal).[Text] = numShare
                CType(e.Item.FindControl("hypLnkMonth"), Global.System.Web.UI.WebControls.HyperLink).[Text] = Global.Microsoft.VisualBasic.DateAndTime.MonthName(numMonth) & " " & numYear
            End If

            If e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.Footer Then
                CType(e.Item.FindControl("litHitsTotal"), Global.System.Web.UI.WebControls.Literal).[Text] = Me.AFF_HitsTotal
            End If

        End Sub

        Protected Sub rptAffiliateSalesLink_ItemDataBound(ByVal sender As Object, ByVal e As Global.System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptAffiliateSalesLink.ItemDataBound
            Dim datValue As Date

            If (e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.Item) Or (e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.AlternatingItem) Then
                Me.SalesLinkCount = Me.SalesLinkCount + 1
                CType(e.Item.FindControl("litSalesLinkCnt"), Global.System.Web.UI.WebControls.Literal).[Text] = Me.SalesLinkCount
                datValue = e.Item.DataItem("O_Date")
                CType(e.Item.FindControl("litSalesLinkDate"), Global.System.Web.UI.WebControls.Literal).[Text] = CkartrisDisplayFunctions.FormatDate(datValue, "t", Session("LANG"))

                CType(e.Item.FindControl("litSalesLinkValue"), Global.System.Web.UI.WebControls.Literal).[Text] = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency(), e.Item.DataItem("OrderTotal"))
                CType(e.Item.FindControl("litSalesLinkCommission"), Global.System.Web.UI.WebControls.Literal).[Text] = e.Item.DataItem("O_AffiliatePercentage") & "%"
                CType(e.Item.FindControl("litSalesLinkTotal"), Global.System.Web.UI.WebControls.Literal).[Text] = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency(), e.Item.DataItem("Commission"))
                If e.Item.DataItem("O_AffiliatePaymentID") > 0 Then
                    CType(e.Item.FindControl("imgSalesLinkPaid"), Global.System.Web.UI.WebControls.Image).ImageUrl = WebShopURL() & "images/tick.gif"
                Else
                    CType(e.Item.FindControl("imgSalesLinkPaid"), Global.System.Web.UI.WebControls.Image).ImageUrl = WebShopURL() & "images/tick_empty.gif"
                End If
            End If

        End Sub

        Protected Sub rptAffPayments_ItemDataBound(ByVal sender As Object, ByVal e As Global.System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptAffPayments.ItemDataBound
            Dim datValue As Date

            If (e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.Item) Or (e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.AlternatingItem) Then
                Me.PaymentCount = Me.PaymentCount + 1
                datValue = e.Item.DataItem("AFP_DateTime")
                CType(e.Item.FindControl("litPaymentDate"), Global.System.Web.UI.WebControls.Literal).[Text] = CkartrisDisplayFunctions.FormatDate(datValue, "t", Session("LANG"))
                CType(e.Item.FindControl("litPaymentPayment"), Global.System.Web.UI.WebControls.Literal).[Text] = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency(), e.Item.DataItem("TotalPayment"))
                Me.PaymentTotal = Me.PaymentTotal + e.Item.DataItem("TotalPayment")
            End If

            If e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.Footer Then
                CType(e.Item.FindControl("litPaymentTotal"), Global.System.Web.UI.WebControls.Literal).[Text] = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency(), Me.PaymentTotal)
            End If

        End Sub

        Protected Sub rptAffiliateUnpaid_ItemDataBound(ByVal sender As Object, ByVal e As Global.System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptAffiliateUnpaid.ItemDataBound
            Dim datValue As Date

            If (e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.Item) Or (e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.AlternatingItem) Then
                Me.UnpaidCount = Me.UnpaidCount + 1
                datValue = e.Item.DataItem("O_Date")
                CType(e.Item.FindControl("litUnpaidDate"), Global.System.Web.UI.WebControls.Literal).[Text] = CkartrisDisplayFunctions.FormatDate(datValue, "t", Session("LANG"))

                CType(e.Item.FindControl("litUnpaidPayment"), Global.System.Web.UI.WebControls.Literal).[Text] = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency(), e.Item.DataItem("Commission"))

                CType(e.Item.FindControl("litUnpaidPaymentPercent"), Global.System.Web.UI.WebControls.Literal).[Text] = e.Item.DataItem("O_AffiliatePercentage") & "%"

                Me.UnpaidTotal = Me.UnpaidTotal + e.Item.DataItem("Commission")
            End If

            If e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.Footer Then
                CType(e.Item.FindControl("litUnpaidTotal"), Global.System.Web.UI.WebControls.Literal).[Text] = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency(), Me.UnpaidTotal)
            End If

        End Sub

    End Class

     */
}