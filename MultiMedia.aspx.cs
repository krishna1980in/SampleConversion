

// These parameters are added to the URL of the 
// iframe src (set in PopupMessage.ascx.vb) by the
// ShowMediaPopup function on ProductView.aspx. In this
// way, the URL of the iframe can be different for each
// link, with the parameters being changed dynamically
// by javascript just before the popup is made visible.

// retrieve all the links for this specific record - called by LoadProduct() in ProductView.ascx.vb & ????? in ProductVersions.ascx.vb


// add the results items into the cloned DataTable

// bind the results to a GridView

// get this item's media type details

// get the default height and width from the media type if the item doesn't have its height and width set

// check if the media link item already contains the embed code
// embed - just write it directly to the page
// template is needed so load up the appropriate control

// try to get if media link has its own set of parameters

// and get the default paramters from the media type if none

// parse the parameters
// format: parameter1=value1;parameter2=value2;parameter3=value3

// finally add the template to the placeholder

// and close the inline div

// We used a skin specified in the language record, if none
// then use the default 'Kartris' one.
using CkartrisDataManipulation;

namespace EmployeeManagementSystem
{
    class _failedMemberConversionMarker1
    {
    }
#error Cannot convert ClassBlockSyntax - see comment for details
    /* Cannot convert ClassBlockSyntax, CONVERSION ERROR: Object reference not set to an instance of an object. in 'Partial Class MultiMedia\r\...' at character 627
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

    Partial Class MultiMedia
        Inherits Global.System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.Load

            'These parameters are added to the URL of the 
            'iframe src (set in PopupMessage.ascx.vb) by the
            'ShowMediaPopup function on ProductView.aspx. In this
            'way, the URL of the iframe can be different for each
            'link, with the parameters being changed dynamically
            'by javascript just before the popup is made visible.
            Dim intML_ID As Integer = Me.Request.QueryString("ML_ID")
            Dim strMT_Extension As String = Me.Request.QueryString("MT_Extension")
            Dim intParentID As Integer = Me.Request.QueryString("intParentID")
            Dim strParentType As String = Me.Request.QueryString("strParentType")
            Dim intWidth As Integer = Me.Request.QueryString("intWidth")
            Dim intHeight As Integer = Me.Request.QueryString("intHeight")

            Dim tblMediaLinks As Global.System.Data.DataTable
            If Not Me.IsPostBack Then
                If intML_ID > 0 Then

                    'retrieve all the links for this specific record - called by LoadProduct() in ProductView.ascx.vb & ????? in ProductVersions.ascx.vb

                    tblMediaLinks = MediaBLL.GetMediaLinksByParent(intParentID, strParentType)

                    Dim objResults = From rowLink In tblMediaLinks.AsEnumerable() Where rowLink.Field(Of Integer)("ML_ID") = intML_ID
                    Dim drwResult As Global.System.Data.DataRow = Nothing
                    Dim tblFilteredLinks As Global.System.Data.DataTable = tblMediaLinks.Clone

                    ' add the results items into the cloned DataTable
                    For Each objValue In objResults
                        drwResult = tblFilteredLinks.NewRow()
                        drwResult.ItemArray = objValue.ItemArray
                        tblFilteredLinks.Rows.Add(drwResult)
                    Next

                    ' bind the results to a GridView
                    rptMediaLinks.DataSource = tblFilteredLinks
                    rptMediaLinks.DataBind()

                End If
            End If

        End Sub

        Protected Sub rptMediaLinks_ItemDataBound(ByVal sender As Object, ByVal e As Global.System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptMediaLinks.ItemDataBound
            If e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.AlternatingItem OrElse _
                e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.Item Then

                Dim strWebShopURL As String = CkartrisBLL.WebShopURL

                Dim drvMediaLink As Global.System.Data.DataRowView = CType(e.Item.DataItem, Global.System.Data.DataRowView)

                'get this item's media type details
                Dim strMT_Extension As String = FixNullFromDB(drvMediaLink.Item("MT_Extension"))
                Dim blnisEmbedded As Boolean = CType(FixNullFromDB(drvMediaLink.Item("MT_Embed")), Boolean)


                Dim intHeight As Integer = FixNullFromDB(drvMediaLink.Item("ML_Height"))
                Dim intWidth As Integer = FixNullFromDB(drvMediaLink.Item("ML_Width"))

                'get the default height and width from the media type if the item doesn't have its height and width set
                If intHeight = 0 Then
                    intHeight = FixNullFromDB(drvMediaLink.Item("MT_DefaultHeight"))
                End If
                If intWidth = 0 Then
                    intWidth = FixNullFromDB(drvMediaLink.Item("MT_DefaultWidth"))
                End If

                Dim strMediaLinksFolder As String = KartSettingsManager.GetKartConfig("general.uploadfolder") & "Media/"

                Dim intML_ID As Integer = drvMediaLink.Item("ML_ID")
                Dim strML_EmbedSource As String = FixNullFromDB(drvMediaLink.Item("ML_EmbedSource"))
                Dim strML_Type As String = FixNullFromDB(drvMediaLink.Item("ML_ParentType"))

                Dim strML_EmbedSource1 As String = ""

                If String.IsNullOrEmpty(strML_EmbedSource) Then
                    If strMT_Extension <> "html5video" Then
                        strML_EmbedSource = strWebShopURL & Global.Microsoft.VisualBasic.Strings.Replace(strMediaLinksFolder, "~/", "") & intML_ID & "." & strMT_Extension
                    Else
                        strML_EmbedSource = strWebShopURL & Global.Microsoft.VisualBasic.Strings.Replace(strMediaLinksFolder, "~/", "") & intML_ID & ".mp4"
                        strML_EmbedSource1 = strWebShopURL & Global.Microsoft.VisualBasic.Strings.Replace(strMediaLinksFolder, "~/", "") & intML_ID & ".webm"
                    End If
                End If

                Dim litInline As New Global.System.Web.UI.WebControls.Literal

                'check if the media link item already contains the embed code
                If blnisEmbedded Then
                    'embed - just write it directly to the page
                    litInline.[Text] += strML_EmbedSource
                    phdInline.Controls.Add(litInline)
                Else
                    litInline.[Text] = "<div id='inline_" & intML_ID & "'>"
                    phdInline.Controls.Add(litInline)
                    'template is needed so load up the appropriate control
                    Dim UC_Media As Global.System.Web.UI.UserControl = Me.LoadControl("~/UserControls/MediaTemplates/" & strMT_Extension & ".ascx")
                    If UC_Media IsNot Nothing Then
                        UC_Media.Attributes.Add("Source", strML_EmbedSource)
                        UC_Media.Attributes.Add("Height", intHeight)
                        UC_Media.Attributes.Add("Width", intWidth)
                        If strMT_Extension = "html5video" Then UC_Media.Attributes.Add("WebM", strML_EmbedSource1)

                        'try to get if media link has its own set of parameters
                        Dim strParameters As String = FixNullFromDB(drvMediaLink.Item("ML_Parameters"))

                        'and get the default paramters from the media type if none
                        If String.IsNullOrEmpty(strParameters) Then
                            strParameters = FixNullFromDB(drvMediaLink.Item("MT_DefaultParameters"))
                        End If

                        'parse the parameters
                        'format: parameter1=value1;parameter2=value2;parameter3=value3
                        If String.IsNullOrEmpty(strParameters) Then
                            Dim arrParameters As String() = Global.Microsoft.VisualBasic.Strings.Split(strParameters, ";")

                            For x = 0 To Global.Microsoft.VisualBasic.Information.UBound(arrParameters)
                                Dim arrParameter As String() = Global.Microsoft.VisualBasic.Strings.Split(arrParameters(x), "=")
                                If Global.Microsoft.VisualBasic.Information.UBound(arrParameter) = 1 Then
                                    UC_Media.Attributes.Add(arrParameter(0), arrParameter(1))
                                End If
                            Next
                        End If
                    End If

                    'finally add the template to the placeholder
                    phdInline.Controls.Add(UC_Media)

                    'and close the inline div
                    Dim litInlineEnd As New Global.System.Web.UI.WebControls.Literal
                    litInlineEnd.[Text] = "</div>"
                    phdInline.Controls.Add(litInlineEnd)

                End If
            End If
        End Sub

        Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.PreInit

            Dim strSkin As String = CkartrisBLL.Skin(Me.Session("LANG"))

            'We used a skin specified in the language record, if none
            'then use the default 'Kartris' one.
            If Not String.IsNullOrEmpty(strSkin) Then
                strSkin = "Kartris"
            End If

        End Sub
    End Class

     */
}