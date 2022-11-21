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
using System.Web.HttpContext;

partial class _Affiliate_PayRep : _PageBaseClass
{
    private int UserID, intPageSize, numDueDays;
    protected static bool blnCheckHeader;


    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "PageTitle_AffiliateReportFor") + " | " + System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        intPageSize = 10;

        try
        {
            ;/* Cannot convert AssignmentStatementSyntax, System.ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
Parameter name: index
   at System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.<>c__DisplayClass83_0.<ConvertArguments>b__0(ArgumentSyntax a, Int32 i)
   at System.Linq.Enumerable.<SelectIterator>d__5`2.MoveNext()
   at System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
   at Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SeparatedList[TNode](IEnumerable`1 nodes)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitPredefinedCastExpression(PredefinedCastExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.PredefinedCastExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitPredefinedCastExpression(PredefinedCastExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.PredefinedCastExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitAssignmentStatement(AssignmentStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.AssignmentStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 
            UserID = CInt(Request.QueryString("CustomerID"))

 */
        }
        catch (Exception ex)
        {
            System.Web.HttpContext.Current.Response.Redirect("_CustomersList.aspx?mode=af");
        }

        numDueDays = KartSettingsManager.GetKartConfig("frontend.users.affiliates.commissionduedays");

        if (!(IsPostBack))
        {
            RefreshCommissionSummary();
            RefreshUnpaidCommissionList();
            RefreshPaidCommissionList();
        }
    }

    protected void gvwUnpaid_DataBound(object sender, System.EventArgs e)
    {
        try
        {
            sender.HeaderRow.Cells(2).Text = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_Commission");

            (CheckBox)gvwUnpaid.HeaderRow.FindControl("chkHeader").Checked = blnCheckHeader;
        }
        catch (Exception ex)
        {
        }
    }

    protected void gvwUnpaid_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        System.Data.DataRowView drvUnpaid;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            drvUnpaid = e.Row.DataItem;

            if ((DateTime)drvUnpaid.Item["O_Date"] <= DateTime.Today.AddDays(-numDueDays))
                (CheckBox)e.Row.FindControl("chkPaid").Checked = true;
            else
                (CheckBox)e.Row.FindControl("chkPaid").Checked = false;
        }
    }

    protected void gvwPaid_DataBound(object sender, System.EventArgs e)
    {
        try
        {
            sender.HeaderRow.Cells(2).Text = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_Value");
        }
        catch (Exception ex)
        {
        }
    }

    protected string GetDateTime(DateTime datInput)
    {
        ;/* Cannot convert ReturnStatementSyntax, System.ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
Parameter name: index
   at System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.<>c__DisplayClass83_0.<ConvertArguments>b__0(ArgumentSyntax a, Int32 i)
   at System.Linq.Enumerable.<SelectIterator>d__5`2.MoveNext()
   at System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
   at Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SeparatedList[TNode](IEnumerable`1 nodes)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.<>c__DisplayClass83_0.<ConvertArguments>b__0(ArgumentSyntax a, Int32 i)
   at System.Linq.Enumerable.<SelectIterator>d__5`2.MoveNext()
   at System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
   at Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SeparatedList[TNode](IEnumerable`1 nodes)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitReturnStatement(ReturnStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ReturnStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 
        Return CkartrisDisplayFunctions.FormatDate(datInput, "t", Session("_LANG"))

 */
    }

    protected string GetCommission(double numCommission, double numPercentage)
    {
        return CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, numCommission) + " (" + numPercentage + "%)";
    }

    protected string GetTotalPayment(double numPayment)
    {
        return CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, numPayment);
    }

    public void RefreshCommissionSummary()
    {
        kartris.Basket objBasket = new kartris.Basket();
        DataTable tblCommissions = new DataTable();

        tblCommissions = AffiliateBLL._GetCustomerAffiliateCommissionSummary(UserID);
        if (tblCommissions.Rows.Count > 0)
        {
            litEmail.Text = tblCommissions.Rows(0).Item("EmailAddress");
            litPaidCommission.Text = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, tblCommissions.Rows(0).Item("PaidCommission"));
            litUnpaidCommission.Text = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, tblCommissions.Rows(0).Item("UnpaidCommission"));
        }

        if (tblCommissions.Rows(0).Item("EmailAddress") == "")
        {
        }

        tblCommissions.Dispose();
        objBasket = null/* TODO Change to default(_) if this is not a reference type */;
    }

    public void RefreshUnpaidCommissionList()
    {
        kartris.Basket objBasket = new kartris.Basket();
        int intTotalRowCount;
        DataTable tblCommissions = new DataTable();

        tblCommissions = AffiliateBLL._GetCustomerAffiliateUnpaidCommission(UserID, 1, 1000000);
        intTotalRowCount = tblCommissions.Rows.Count;

        if (intTotalRowCount == 0)
        {
            litNoUnpaidRec.Text = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_BackSearchNoResults");
            btnSetAsPaid.Visible = false;
            gvwUnpaid.Visible = false;
            divNoUnpaidRec.Visible = true;
        }
        else
        {
            litNoUnpaidRec.Text = "";
            btnSetAsPaid.Visible = true;
            gvwUnpaid.Visible = true;
            divNoUnpaidRec.Visible = false;
        }

        if (intTotalRowCount <= _UC_UnpaidPager.CurrentPage * _UC_UnpaidPager.ItemsPerPage)
            _UC_UnpaidPager.CurrentPage = IIf(_UC_UnpaidPager.CurrentPage - 1 < 0, 0, _UC_UnpaidPager.CurrentPage - 1);

        gvwUnpaid.DataSource = AffiliateBLL._GetCustomerAffiliateUnpaidCommission(UserID, _UC_UnpaidPager.CurrentPage + 1, intPageSize);
        gvwUnpaid.DataBind();
        _UC_UnpaidPager.TotalItems = intTotalRowCount;
        _UC_UnpaidPager.ItemsPerPage = intPageSize;
        _UC_UnpaidPager.PopulatePagerControl();


        objBasket = null/* TODO Change to default(_) if this is not a reference type */;
        tblCommissions.Dispose();
    }

    public void RefreshPaidCommissionList()
    {
        kartris.Basket objBasket = new kartris.Basket();
        int intTotalRowCount;
        DataTable tblCommissions = new DataTable();

        tblCommissions = AffiliateBLL._GetCustomerAffiliatePaidCommission(UserID, 1, 1000000);
        intTotalRowCount = tblCommissions.Rows.Count;

        if (intTotalRowCount == 0)
        {
            litNoPaidRec.Text = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_BackSearchNoResults");
            gvwPaid.Visible = false;
            divNoPaidRec.Visible = true;
        }
        else
        {
            litNoPaidRec.Text = "";
            gvwPaid.Visible = true;
            divNoPaidRec.Visible = false;
        }

        if (intTotalRowCount <= _UC_PaidPager.CurrentPage * _UC_PaidPager.ItemsPerPage)
            _UC_PaidPager.CurrentPage = IIf(_UC_PaidPager.CurrentPage - 1 < 0, 0, _UC_PaidPager.CurrentPage - 1);

        gvwPaid.DataSource = AffiliateBLL._GetCustomerAffiliatePaidCommission(UserID, _UC_PaidPager.CurrentPage + 1, intPageSize);
        gvwPaid.DataBind();

        _UC_PaidPager.TotalItems = intTotalRowCount;
        _UC_PaidPager.ItemsPerPage = intPageSize;
        _UC_PaidPager.PopulatePagerControl();

        objBasket = null/* TODO Change to default(_) if this is not a reference type */;
        tblCommissions.Dispose();
    }

    protected void _UC_UnpaidPager_PageChanged()
    {
        RefreshUnpaidCommissionList();
    }

    protected void _UC_PaidPager_PageChanged()
    {
        RefreshPaidCommissionList();
    }

    public void CheckAll_Click(object sender, EventArgs e)
    {
        blnCheckHeader = sender.@checked;

        RefreshUnpaidCommissionList();

        foreach (GridViewRow grwCommission in gvwUnpaid.Rows)
            (CheckBox)grwCommission.FindControl("chkPaid").Checked = sender.@checked;
    }

    public void CheckPaid_Click(object sender, EventArgs e)
    {
        if (sender.Checked == false)
            (CheckBox)gvwUnpaid.HeaderRow.FindControl("chkHeader").Checked = false;
    }


    protected void btnSetAsPaid_Click(object sender, System.EventArgs e)
    {
        kartris.Basket objBasket = new kartris.Basket();
        int intAffiliatePaymentID, intOrderID;

        intAffiliatePaymentID = AffiliateBLL._AddAffiliatePayments(UserID);

        foreach (GridViewRow grwCommission in gvwUnpaid.Rows)
        {
            if ((CheckBox)grwCommission.FindControl("chkPaid").Checked)
            {
                intOrderID = System.Convert.ToInt32((HiddenField)grwCommission.FindControl("hidOrderID").Value);
                AffiliateBLL._UpdateAffiliateCommission(intAffiliatePaymentID, intOrderID);
            }
        }

        RefreshCommissionSummary();

        RefreshUnpaidCommissionList();

        RefreshPaidCommissionList();



        objBasket = null/* TODO Change to default(_) if this is not a reference type */;
    }

    protected void MarkUnpaid_Click(object sender, CommandEventArgs E)
    {
        kartris.Basket objBasket = new kartris.Basket();
        int intAffiliatePaymentID;

        intAffiliatePaymentID = E.CommandArgument;

        AffiliateBLL._UpdateAffiliatePayment(intAffiliatePaymentID);

        RefreshCommissionSummary();

        RefreshUnpaidCommissionList();

        RefreshPaidCommissionList();

        (CheckBox)gvwUnpaid.HeaderRow.FindControl("chkHeader").Checked = false;

        objBasket = null/* TODO Change to default(_) if this is not a reference type */;
    }
}
