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
using CkartrisEnumerations;
using KartSettingsManager;
using System.Web.HttpContext;

partial class Admin_Suppliers : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = System.Web.HttpContext.GetGlobalResourceObject("_Suppliers", "PageTitle_Suppliers") + " | " + System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");
        if (!Page.IsPostBack)
            GetSuppliersList();
    }

    public void GetSuppliersList()
    {
        mvwSuppliersData.SetActiveView(viwSuppliersList);

        gvwSuppliers.DataSource = null;
        gvwSuppliers.DataBind();

        DataTable tblSuppliersList = new DataTable();
        tblSuppliersList = GetSuppliersFromCache();

        if (tblSuppliersList.Rows.Count == 0)
            mvwSuppliersData.SetActiveView(viwNoItems);

        gvwSuppliers.DataSource = tblSuppliersList;
        gvwSuppliers.DataBind();
    }

    protected void gvwSuppliers_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvwSuppliers.PageIndex = e.NewPageIndex;
        GetSuppliersList();
    }

    private void gvwSuppliers_RowCommand(object src, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "EditSupplier":
                {
                    gvwSuppliers.SelectedIndex = System.Convert.ToInt32(e.CommandArgument) - (gvwSuppliers.PageSize * gvwSuppliers.PageIndex);
                    litSupplierID.Text = gvwSuppliers.SelectedValue;
                    PrepareExistingSupplier();
                    break;
                }

            case "viwLinkedProducts":
                {
                    gvwSuppliers.SelectedIndex = System.Convert.ToInt32(e.CommandArgument) - (gvwSuppliers.PageSize * gvwSuppliers.PageIndex);
                    litSupplierID.Text = gvwSuppliers.SelectedValue;
                    GetLinkedProducts();
                    break;
                }
        }
    }

    private void GetLinkedProducts()
    {
        pnlNoLinkedProducts.Visible = false;
        gvwLinkedProducts.DataSource = null;
        gvwLinkedProducts.DataBind();

        DataTable tblLinkedProducts = new DataTable();
        ProductsBLL objProductsBLL = new ProductsBLL();
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
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitAssignmentStatement(AssignmentStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.AssignmentStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 
        tblLinkedProducts = objProductsBLL._GetProductsBySupplier(Session("_LANG"), GetSupplierID())

 */
        if (tblLinkedProducts.Rows.Count == 0)
        {
            pnlNoLinkedProducts.Visible = true;
            gvwLinkedProducts.Visible = false;
        }
        else
        {
            pnlNoLinkedProducts.Visible = false;
            gvwLinkedProducts.Visible = true;
            gvwLinkedProducts.DataSource = tblLinkedProducts;
            gvwLinkedProducts.DataBind();
        }

        mvwSuppliers.SetActiveView(viwLinkedProducts);
        updSupplierDetails.Update();
    }

    protected void lnkBtnCancel_Click(object sender, System.EventArgs e)
    {
        litSupplierID.Text = "0";
        mvwSuppliers.SetActiveView(viwSuppliersData);
        updSupplierDetails.Update();
    }

    protected void lnkBtnNewSupplier_Click(object sender, System.EventArgs e)
    {
        PrepareNewSupplier();
    }

    protected void lnkBtnSave_Click(object sender, System.EventArgs e)
    {
        // ' calling the save method for (INSERT/UPDATE)
        if (GetSupplierID() == 0)
        {
            if (!SaveSupplier(DML_OPERATION.INSERT))
                return;
        }
        else if (!SaveSupplier(DML_OPERATION.UPDATE))
            return;

        // Show animated 'updated' message


        (Skins_Admin_Template)this.Master.DataUpdated();

        GetSuppliersList();

        litSupplierID.Text = "0";
        mvwSuppliers.SetActiveView(viwSuppliersData);
        updSupplierDetails.Update();
    }

    public bool SaveSupplier(DML_OPERATION enumOperation)
    {
        string strSupplierName = txtSupplierName.Text;
        bool blnLive = chkSupplierLive.Checked;
        UsersBLL objUsersBLL = new UsersBLL();

        string strMessage = "";
        switch (enumOperation)
        {
            case object _ when DML_OPERATION.UPDATE:
                {
                    if (!objUsersBLL._UpdateSuppliers(GetSupplierID(), strSupplierName, blnLive, strMessage))
                    {
                        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                        return false;
                    }

                    break;
                }

            case object _ when DML_OPERATION.INSERT:
                {
                    if (!objUsersBLL._AddSuppliers(strSupplierName, blnLive, strMessage))
                    {
                        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                        return false;
                    }

                    break;
                }
        }

        RefreshSuppliersCache();
        return true;
    }

    private int GetSupplierID()
    {
        if (litSupplierID.Text != "")
            return System.Convert.ToInt32(litSupplierID.Text);
        return 0;
    }

    public void PrepareNewSupplier()
    {
        litSupplierID.Text = "0";
        txtSupplierName.Text = null;
        chkSupplierLive.Checked = false;
        mvwSuppliers.SetActiveView(viwDetails);
        updSupplierDetails.Update();
    }

    public void PrepareExistingSupplier()
    {
        chkSupplierLive.Checked = (CheckBox)gvwSuppliers.SelectedRow.Cells(1).FindControl("chkSup_Live").Checked;
        txtSupplierName.Text = gvwSuppliers.SelectedRow.Cells(0).Text;
        mvwSuppliers.SetActiveView(viwDetails);
        updSupplierDetails.Update();
    }

    protected void lnkBtnHideLinkedProducts_Click(object sender, System.EventArgs e)
    {
        mvwSuppliers.SetActiveView(viwSuppliersData);
    }

    protected void gvwLinkedProducts_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvwLinkedProducts.PageIndex = e.NewPageIndex;
        GetLinkedProducts();
    }
}
