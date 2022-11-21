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

partial class Admin_FeaturedProducts : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "PageTitle_FeaturedProducts") + " | " + System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        if (!Page.IsPostBack)
        {
            LoadFeaturedProducts();
            short numMax = System.Convert.ToInt16(GetKartConfig("frontend.featuredproducts.display.max"));
            switch (numMax)
            {
                case 0:
                    {
                        litMaxNumberToDisplay.Text = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_None");
                        break;
                    }

                case -1:
                    {
                        litMaxNumberToDisplay.Text = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_Unlimited");
                        break;
                    }

                default:
                    {
                        litMaxNumberToDisplay.Text = numMax;
                        break;
                    }
            }
        }
    }

    private void LoadFeaturedProducts()
    {
        mvwFeaturedProducts.SetActiveView(viwProductList);
        lbxFeaturedProducts.Items.Clear();
        try
        {
            ProductsBLL objProductsBLL = new ProductsBLL();
            ;/* Cannot convert LocalDeclarationStatementSyntax, System.ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
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
   at ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertInitializer(VariableDeclaratorSyntax declarator)
   at ICSharpCode.CodeConverter.CSharp.CommonConversions.SplitVariableDeclarations(VariableDeclaratorSyntax declarator)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 
            Dim tblFeaturedProducts As DataTable = objProductsBLL._GetFeaturedProducts(Session("_LANG"))

 */
            if (tblFeaturedProducts.Rows.Count == 0)
                mvwFeaturedProducts.SetActiveView(viwNoItems);
            if (tblFeaturedProducts.Rows.Count > 0)
                tblFeaturedProducts.DefaultView.Sort = "ProductPriority DESC";
            foreach (DataRow rowProduct in tblFeaturedProducts.Rows)
                AddProductToList(rowProduct("ProductID"), rowProduct("ProductName"), rowProduct("ProductPriority"));
        }
        catch (Exception ex)
        {
        }
    }

    private void AddProductToList(int intProductID, string strProductName, byte intPriority)
    {

        // Add item if ID is not already there
        if (lbxFeaturedProducts.Items.FindByValue(System.Convert.ToString(intProductID)) == null)
        {
            int intIndex = 0;
            bool blnAdded = false;
            foreach (ListItem itmProduct in lbxFeaturedPriorities.Items)
            {
                if (itmProduct.Value <= intPriority)
                {
                    lbxFeaturedProducts.Items.Insert(intIndex, new ListItem(strProductName, intProductID));
                    lbxFeaturedPriorities.Items.Insert(intIndex, new ListItem(intProductID, intPriority));
                    blnAdded = true;
                    break;
                }
                intIndex += 1;
            }
            if (blnAdded == false)
            {
                lbxFeaturedProducts.Items.Add(new ListItem(strProductName, intProductID));
                lbxFeaturedPriorities.Items.Add(new ListItem(intProductID, intPriority));
            }
            ReLoadFeaturedProducts();
        }
    }

    private void RemoveProductFromList(int intProductID)
    {
        if (!lbxFeaturedProducts.Items.FindByValue(System.Convert.ToString(intProductID)) == null)
        {
            lbxFeaturedProducts.Items.Remove(lbxFeaturedProducts.Items.FindByValue(System.Convert.ToString(intProductID)));
            lbxFeaturedPriorities.Items.Remove(lbxFeaturedPriorities.Items.FindByText(System.Convert.ToString(intProductID)));
        }
        ReLoadFeaturedProducts();
    }

    private void ReLoadFeaturedProducts()
    {
        DataTable tblFeaturedProducts = new DataTable();
        tblFeaturedProducts.Columns.Add(new DataColumn("ProductID", Type.GetType("System.Int32")));
        tblFeaturedProducts.Columns.Add(new DataColumn("ProductName", Type.GetType("System.String")));
        tblFeaturedProducts.Columns.Add(new DataColumn("ProductPriority", Type.GetType("System.Byte")));

        foreach (ListItem itmProduct in lbxFeaturedProducts.Items)
            tblFeaturedProducts.Rows.Add(System.Convert.ToInt32(itmProduct.Value), itmProduct.Text, lbxFeaturedPriorities.Items.FindByText(itmProduct.Value).Value);
        if (tblFeaturedProducts.Rows.Count == 0)
            mvwFeaturedProducts.SetActiveView(viwNoItems);
        else
        {
            mvwFeaturedProducts.SetActiveView(viwProductList);
            // phdProductsList.Visible = False

            gvwFeaturedProducts.DataSource = tblFeaturedProducts;
            gvwFeaturedProducts.DataBind();
        }

        updFeaturedProducts.Update();
    }

    protected void btnAdd_Click(object sender, System.EventArgs e)
    {
        if (txtProduct.Text != "")
        {
            CheckAutoCompleteData();
            txtProduct.Text = "";
            txtFeaturedLevel.Text = "";
        }
    }

    public void CheckAutoCompleteData()
    {
        string strAutoCompleteText = "";
        int numItemID = 0;
        string strItemName = "";
        strAutoCompleteText = txtProduct.Text;
        var numIndex = strAutoCompleteText.LastIndexOf("(");

        if (Len(txtFeaturedLevel.Text) > 0)
        {
            if (strAutoCompleteText != "" && strAutoCompleteText.Contains("(") && strAutoCompleteText.Contains(")"))
            {
                try
                {
                    numItemID = System.Convert.ToInt32(Strings.Replace(Strings.Mid(strAutoCompleteText, numIndex + 2), ")", ""));
                    ProductsBLL objProductsBLL = new ProductsBLL();
                    if (objProductsBLL._GetCustomerGroup(numItemID) != 0)
                    {
                        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ProductLimitedToCustomerGroupAsFeatured"));
                        return;
                    }
                    AddProductToList(numItemID, Strings.Left(strAutoCompleteText, numIndex - 1), txtFeaturedLevel.Text);
                }
                catch (Exception ex)
                {
                    _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_InvalidValue"));
                }
            }
            else
                _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_InvalidValue"));
        }
    }

    protected void gvwFeaturedProducts_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "EditFeaturedProducts":
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
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitAssignmentStatement(AssignmentStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.AssignmentStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 
                Session("tab") = "Featured_products"

 */
                    System.Web.HttpContext.Response.Redirect("~/Admin/_ModifyProduct.aspx?ProductID=" + e.CommandArgument);
                    break;
                }

            case "ChangePriority":
                {
                    string strNewPriority = (TextBox)gvwFeaturedProducts.Rows(e.CommandArgument).Cells(1).FindControl("txtPriority").Text;
                    if (Information.IsNumeric(strNewPriority))
                        ChangePriority(System.Convert.ToInt16(e.CommandArgument), System.Convert.ToInt32(strNewPriority));
                    else
                    {
                        (TextBox)gvwFeaturedProducts.Rows(e.CommandArgument).Cells(1).FindControl("txtPriority").Text = "";
                        return;
                    }

                    break;
                }

            case "RemoveProduct":
                {
                    RemoveProductFromList(e.CommandArgument);
                    break;
                }
        }
    }

    public void ChangePriority(short numIndx, int numNewPriority)
    {
        int numPID = lbxFeaturedProducts.Items(numIndx).Value;
        string strPName = lbxFeaturedProducts.Items(numIndx).Text;
        RemoveProductFromList(numPID);
        AddProductToList(numPID, strPName, numNewPriority);
    }

    protected void btnSaveChanges_Click(object sender, System.EventArgs e)
    {
        SaveChanges();
    }

    public void SaveChanges()
    {
        DataTable tblFeaturedProducts = new DataTable();
        tblFeaturedProducts.Columns.Add(new DataColumn("ProductID", Type.GetType("System.Int32")));
        tblFeaturedProducts.Columns.Add(new DataColumn("Priority", Type.GetType("System.Byte")));

        foreach (ListItem itm in lbxFeaturedPriorities.Items)
            tblFeaturedProducts.Rows.Add(System.Convert.ToInt32(itm.Text), System.Convert.ToByte(itm.Value));

        string strMessage = "";
        if (!ProductsBLL._UpdateFeaturedProducts(tblFeaturedProducts, strMessage))
        {
            _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
            return;
        }

        RefreshFeaturedProductsCache();
        (Skins_Admin_Template)this.Master.DataUpdated();
    }
}
