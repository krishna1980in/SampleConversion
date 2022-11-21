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
using CkartrisEnumerations;
using Kartris;
using CkartrisDataManipulation;
using KartSettingsManager;
using CkartrisDisplayFunctions;

partial class Admin_Downloads : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = System.Web.HttpContext.GetGlobalResourceObject("_Versions", "PageTitle_VersionDownloads") + " | " + System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        // ' The following line is important for the confirmation msg box
        // '  => it will allow the event of the server side button to be fired
        Page.ClientScript.GetPostBackEventReference(this, string.Empty);

        if (!Page.IsPostBack)
        {
            LoadDownloads();
            LoadLinks();
            CheckTempFolder();
        }
    }

    protected void gvwLinks_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvwLinks.PageIndex = e.NewPageIndex;
        LoadLinks();
    }

    protected void gvwNonLinkedFiles_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvwNonLinkedFiles.PageIndex = e.NewPageIndex;
        LoadDownloads();
    }

    protected void gvwDownloads_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvwDownloads.PageIndex = e.NewPageIndex;
        LoadDownloads();
    }

    public void LoadDownloads()
    {
        mvwDownloads.SetActiveView(viwDownloadData);
        VersionsBLL objVersionsBLL = new VersionsBLL();
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
        Dim tblDownloads As DataTable = objVersionsBLL._GetDownloadableFiles(Session("_LANG"))

 */
        if (tblDownloads.Rows.Count == 0)
            mvwDownloads.SetActiveView(viwNoDownloads);
        gvwDownloads.DataSource = tblDownloads;
        gvwDownloads.DataBind();
        FindNonRelatedFiles(tblDownloads);
        updMain.Update();
    }

    public void LoadLinks()
    {
        mvwLinks.SetActiveView(viwLinksData);
        VersionsBLL objVersionsBLL = new VersionsBLL();
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
        Dim tblLinks As DataTable = objVersionsBLL._GetDownloadableLinks(Session("_LANG"))

 */
        if (tblLinks.Rows.Count == 0)
            mvwLinks.SetActiveView(viwNoLinks);
        gvwLinks.DataSource = tblLinks;
        gvwLinks.DataBind();
        updMain.Update();
    }

    public void FindNonRelatedFiles(DataTable tblDownloads)
    {
        mvwNonLinked.SetActiveView(viwNonLinkedData);
        DirectoryInfo _dirInfo = new DirectoryInfo(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder")));
        DataTable tblNonRelatedFiles = new DataTable();
        tblNonRelatedFiles.Columns.Add(new DataColumn("FileName", Type.GetType("System.String")));
        tblNonRelatedFiles.Columns.Add(new DataColumn("FileSize", Type.GetType("System.String")));
        lstNonRelatedFiles.Items.Clear();
        foreach (FileInfo _file in _dirInfo.GetFiles())
        {
            DataRow[] drFiles = tblDownloads.Select("V_DownloadInfo='" + _file.Name + "'");
            if (drFiles.Length == 0)
            {
                tblNonRelatedFiles.Rows.Add(_file.Name, GetFileLength(_file.Length));
                lstNonRelatedFiles.Items.Add(_file.Name);
            }
        }
        litNonLinkedFiles.Text = System.Convert.ToString(lstNonRelatedFiles.Items.Count + CheckTempFolder());
        if (tblNonRelatedFiles.Rows.Count == 0)
            mvwNonLinked.SetActiveView(viwNoNonLinked);
        gvwNonLinkedFiles.DataSource = tblNonRelatedFiles;
        gvwNonLinkedFiles.DataBind();
    }

    public int CheckTempFolder()
    {
        DirectoryInfo _dirInfo = new DirectoryInfo(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder") + "temp/"));
        int numNoTempFiles = _dirInfo.GetFiles().Count();
        if (numNoTempFiles > 0)
        {
            litTempFiles.Text = numNoTempFiles;
            phdNoTempFiles.Visible = false;
            phdTempFilesExist.Visible = true;
        }
        return numNoTempFiles;
    }

    protected void gvwDownloads_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "OpenFile":
                {
                    DownloadFile(e.CommandArgument);
                    break;
                }

            case "ChangeFile":
            case "NewFile":
                {
                    gvwDownloads.SelectedIndex = e.CommandArgument % gvwDownloads.PageSize;
                    _UC_UploaderPopup.OpenFileUpload();
                    break;
                }

            case "RenameFile":
                {
                    litType.Text = "u";
                    mvwPopup.SetActiveView(viwPopupDownload);
                    gvwDownloads.SelectedIndex = e.CommandArgument % gvwDownloads.PageSize;
                    litPopupVersionName.Text = (Literal)gvwDownloads.SelectedRow.Cells(1).FindControl("litVersionName").Text;
                    litPopupFileName.Text = (Literal)gvwDownloads.SelectedRow.Cells(2).FindControl("litFile").Text;
                    txtPopupFileName.Text = (Literal)gvwDownloads.SelectedRow.Cells(2).FindControl("litFile").Text;
                    lnkSave.ValidationGroup = "FileRename";
                    popExtender.Show();
                    break;
                }
        }
    }

    protected void gvwDownloads_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string strFileName = (LinkButton)e.Row.Cells[3].FindControl("lnkFile").Text;
                if (!string.IsNullOrEmpty(strFileName))
                {
                    if (File.Exists(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder") + strFileName)))
                    {
                        FileInfo _FileInfo;
                        _FileInfo = new FileInfo(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder") + strFileName));
                        (Literal)e.Row.Cells[3].FindControl("litSize").Text = GetFileLength(_FileInfo.Length);
                    }
                    else
                    {
                        (LinkButton)e.Row.Cells[2].FindControl("lnkFile").Visible = false;
                        (Literal)e.Row.Cells[2].FindControl("litFile").Visible = true;
                        (Literal)e.Row.Cells[3].FindControl("litSize").Text = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_Missing");
                        (LinkButton)e.Row.Cells[4].FindControl("lnkBtnRenameFile").Visible = false;
                        (LinkButton)e.Row.Cells[4].FindControl("lnkBtnChangeDownloadFile").Visible = false;
                        (LinkButton)e.Row.Cells[4].FindControl("lnkBtnUploadFile").Visible = true;
                        e.Row.CssClass = "Kartris-GridView-Green";
                    }
                }
                else
                {
                    (Literal)e.Row.Cells[2].FindControl("litFile").Text = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_Unassigned");
                    (Literal)e.Row.Cells[2].FindControl("litFile").Visible = true;
                    (Literal)e.Row.Cells[3].FindControl("litSize").Text = "-";
                    (Literal)e.Row.Cells[3].FindControl("litSize").Visible = true;
                    (LinkButton)e.Row.Cells[4].FindControl("lnkBtnRenameFile").Visible = false;
                    (LinkButton)e.Row.Cells[4].FindControl("lnkBtnChangeDownloadFile").Visible = false;
                    (LinkButton)e.Row.Cells[4].FindControl("lnkBtnUploadFile").Visible = true;
                    e.Row.CssClass = "Kartris-GridView-Red";
                }
            }
        }
        catch (Exception ex)
        {
        }
    }

    protected void gvwLinks_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "ChangeLink":
                {
                    litType.Text = "l";
                    mvwPopup.SetActiveView(viwPopupLink);
                    gvwLinks.SelectedIndex = e.CommandArgument % gvwLinks.PageSize;
                    litPopupVersionName.Text = (Literal)gvwLinks.SelectedRow.Cells(1).FindControl("litVersionName").Text;
                    litPopupLinkLocation.Text = (HyperLink)gvwLinks.SelectedRow.Cells(2).FindControl("lnkLinkLocation").Text;
                    txtPopupLinkLocation.Text = (HyperLink)gvwLinks.SelectedRow.Cells(2).FindControl("lnkLinkLocation").Text;
                    popExtender.Show();
                    break;
                }
        }
    }

    protected void gvwLinks_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string strLinkLocation = (Literal)e.Row.Cells[2].FindControl("litLinkLocation").Text;
                if (string.IsNullOrEmpty(strLinkLocation))
                {
                    (Literal)e.Row.Cells[2].FindControl("litLinkLocation").Text = "-";
                    (Literal)e.Row.Cells[2].FindControl("litLinkLocation").Visible = true;
                    e.Row.CssClass = "Kartris-GridView-Red";
                }
            }
        }
        catch (Exception ex)
        {
        }
    }

    public string GetFileLength(long numSizeInBytes)
    {
        return System.Convert.ToString(Math.Ceiling(numSizeInBytes / (double)1024.0F)) + " KB";
    }

    protected void _UC_UploaderPopup_NeedCategoryRefresh()
    {
        (Skins_Admin_Template)this.Master.LoadCategoryMenu();
    }

    protected void _UC_UploaderPopupUploadClicked()
    {
        if (_UC_UploaderPopup.HasFile())
        {
            string strCodeNumber = (Literal)gvwDownloads.SelectedRow.Cells(0).FindControl("litCodeNumber").Text;
            long lngVersionNumber = System.Convert.ToInt64(gvwDownloads.SelectedValue);
            string strFileName = strCodeNumber + "_" + _UC_UploaderPopup.GetFileName();
            string strUploadFolder = GetKartConfig("general.uploadfolder");
            string strTempFolder = strUploadFolder + "temp/";
            if (!Directory.Exists(System.Web.HttpContext.Server.MapPath(strTempFolder)))
                Directory.CreateDirectory(System.Web.HttpContext.Server.MapPath(strTempFolder));
            string strSavedPath = strTempFolder + strFileName;
            _UC_UploaderPopup.SaveFile(System.Web.HttpContext.Server.MapPath(strSavedPath));
            string strMessage = string.Empty;
            VersionsBLL objVersionsBLL = new VersionsBLL();
            if (!objVersionsBLL._UpdateVersionDownloadInfo(lngVersionNumber, strFileName, "u", strMessage))
            {
                _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                File.SetAttributes(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder") + "temp/" + strFileName), FileAttributes.Normal);
                File.Delete(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder") + "temp/" + strFileName));
                return;
            }
            (Skins_Admin_Template)this.Master.DataUpdated();
            LoadDownloads();
        }
    }

    protected void lnkSave_Click(object sender, System.EventArgs e)
    {
        lnkSave.ValidationGroup = "";
        VersionsBLL objVersionsBLL = new VersionsBLL();
        if (litType.Text == "u")
        {
            string strUploadFolder = GetKartConfig("general.uploadfolder");
            if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + litPopupFileName.Text)))
            {
                File.Copy(System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + litPopupFileName.Text), System.Web.HttpContext.Current.Server.MapPath(strUploadFolder + "temp/" + txtPopupFileName.Text));
                long lngVersionNumber = System.Convert.ToInt64(gvwDownloads.SelectedValue);
                string strMessage = string.Empty;

                if (!objVersionsBLL._UpdateVersionDownloadInfo(lngVersionNumber, txtPopupFileName.Text, "u", strMessage))
                {
                    _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                    File.SetAttributes(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder") + "temp/" + txtPopupFileName.Text), FileAttributes.Normal);
                    File.Delete(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder") + "temp/" + txtPopupFileName.Text));
                    return;
                }
                File.SetAttributes(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder") + litPopupFileName.Text), FileAttributes.Normal);
                File.Delete(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder") + litPopupFileName.Text));
                (Skins_Admin_Template)this.Master.DataUpdated();
                LoadDownloads();
            }
        }
        else
        {
            long lngVersionNumber = System.Convert.ToInt64(gvwLinks.SelectedValue);
            string strMessage = string.Empty;
            if (!objVersionsBLL._UpdateVersionDownloadInfo(lngVersionNumber, Replace(txtPopupLinkLocation.Text, "http://", ""), "l", strMessage))
            {
                _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                return;
            }
            (Skins_Admin_Template)this.Master.DataUpdated();
            LoadLinks();
        }
    }

    protected void gvwNonLinkedFiles_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "OpenFile":
                {
                    DownloadFile(e.CommandArgument);
                    break;
                }

            case "DeleteFile":
                {
                    try
                    {
                        File.SetAttributes(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder") + e.CommandArgument), FileAttributes.Normal);
                        File.Delete(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder") + e.CommandArgument));
                        (Skins_Admin_Template)this.Master.DataUpdated();
                        LoadDownloads();
                    }
                    catch (Exception ex)
                    {
                        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_FilePermissionsError"));
                    }

                    break;
                }

            case "DeleteAllFiles":
                {
                    DeleteNonRelatedFiles();
                    break;
                }
        }
    }

    protected void lnkBtnDeleteTempFiles_Click(object sender, System.EventArgs e)
    {
        DirectoryInfo _dirInfo = new DirectoryInfo(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder") + "temp/"));
        foreach (FileInfo _file in _dirInfo.GetFiles())
        {
            try
            {
                File.SetAttributes(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder") + "temp/" + _file.Name), FileAttributes.Normal);
                File.Delete(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder") + "temp/" + _file.Name));
            }
            catch (Exception ex)
            {
            }
        }
        (Skins_Admin_Template)this.Master.DataUpdated();
        LoadDownloads();
    }

    public void DeleteNonRelatedFiles()
    {
        VersionsBLL objVersionsBLL = new VersionsBLL();
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
        Dim tblDownloads As DataTable = objVersionsBLL._GetDownloadableFiles(Session("_lang"))

 */
        DirectoryInfo _dirInfo = new DirectoryInfo(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder")));
        foreach (FileInfo _file in _dirInfo.GetFiles())
        {
            DataRow[] drFiles = tblDownloads.Select("V_DownloadInfo='" + _file.Name + "'");
            if (drFiles.Length == 0)
            {
                try
                {
                    File.SetAttributes(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder") + _file.Name), FileAttributes.Normal);
                    File.Delete(System.Web.HttpContext.Server.MapPath(GetKartConfig("general.uploadfolder") + _file.Name));
                }
                catch (Exception ex)
                {
                }
            }
        }
        (Skins_Admin_Template)this.Master.DataUpdated();
        LoadDownloads();
    }

    public string FormatFileURL(string strURL)
    {
        if (Strings.Left(strURL.ToLower(), 4) == "http")
            // absolute link
            return strURL;
        else
            // local url
            return "../" + strURL;
    }
}
