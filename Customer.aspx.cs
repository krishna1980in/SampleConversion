using CkartrisBLL;
using CkartrisDataManipulation;

namespace EmployeeManagementSystem
{
    class _failedMemberConversionMarker1
    {
    }
#error Cannot convert ClassBlockSyntax - see comment for details
    /* Cannot convert ClassBlockSyntax, CONVERSION ERROR: Object reference not set to an instance of an object. in 'Partial Class Customer\r\n ...' at character 628
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

    Partial Class Customer
        Inherits PageBaseClass

        Protected Shared blnNoBasketItem As Boolean
        Protected numCustomerDiscount As Double
        Protected Shared AFF_IsAffiliate As Boolean
        Protected Shared AFF_AffiliateCommission As Double
        Protected ML_SignupDateTime, ML_ConfirmationDateTime As Global.System.DateTime

        Private objBasket As New kartris.Basket
        Private numAppMaxOrders, numAppMaxBaskets As Integer
        Private strAppUploadsFolder, strShow As String
        Private tblOrder, tblSavedBasket, tblWishLists, dtbDownloadableProducts As Global.System.Data.DataTable
        Private numCustomerID As Integer
        Private ML_SendMail As Boolean
        Private ML_Format As String
        Private strAction As String
        Private SESSION_ID As Integer

        Private Shared Order_PageSize As Integer
        Private Shared SavedBasket_PageSize As Integer
        Private Shared WishList_PageSize As Integer

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.Load

            Me.strAction = LCase(MyBase.Request.QueryString("action"))
            If Me.strAction <> "savebasket" AndAlso Me.strAction <> "wishlists" Then Me.strAction = "home"

            Me.SESSION_ID = Session("SessionID")

            '--------------------------------------------------
            'Redirect if not logged in
            '--------------------------------------------------
            If Not (MyBase.User.Identity.IsAuthenticated) Then
                MyBase.Response.Redirect("~/CustomerAccount.aspx")
            Else
                Me.numCustomerID = CurrentLoggedUser.ID
            End If

            '--------------------------------------------------
            'Fill customer details
            '--------------------------------------------------
            Dim objUsersBLL As New UsersBLL
            If Not IsPostBack Then
                Dim arrNameAndVAT As Global.System.Array = Split(objUsersBLL.GetNameandEUVAT(CurrentLoggedUser.ID), "|||")
                txtCustomerName.Text = arrNameAndVAT(0)
                txtEUVATNumber.Text = arrNameAndVAT(1)

                'v3.0001 EORI number
                Dim objObjectConfigBLL As New ObjectConfigBLL
                txtEORINumber.Text = objObjectConfigBLL.GetValue("K:user.EORI", CurrentLoggedUser.ID)
                litUserEmail.Text = MyBase.User.Identity.Name
            End If

            '--------------------------------------------------
            'Grab some settings
            '--------------------------------------------------
            Try
                Me.numAppMaxOrders = CInt(KartSettingsManager.GetKartConfig("frontend.users.myaccount.orderhistory.max"))
                Me.numAppMaxBaskets = CInt(KartSettingsManager.GetKartConfig("frontend.users.myaccount.savedbaskets.max"))
                Me.strAppUploadsFolder = KartSettingsManager.GetKartConfig("general.uploadfolder")
            Catch ex As Global.System.Exception
            End Try
            Me.strShow = ""
            'If we're showing a full list, or config settings set to 0, then don't limit (-2)
            If Me.numAppMaxOrders = 0 Or Me.strShow = "orders" Then Me.numAppMaxOrders = -2
            If Me.numAppMaxBaskets = 0 Or Me.strShow = "baskets" Then Me.numAppMaxBaskets = -2
            If Me.numAppMaxBaskets = 0 Or Me.strShow = "wishlists" Then Me.numAppMaxBaskets = -2

            '--------------------------------------------------
            'Customer discount
            '--------------------------------------------------
            Me.numCustomerDiscount = BasketBLL.GetCustomerDiscount(Me.numCustomerID)

            litUserEmail.Text = MyBase.User.Identity.Name

            '--------------------------------------------------
            'Hide/show accordion panes
            '--------------------------------------------------
            If LCase(KartSettingsManager.GetKartConfig("frontend.cataloguemode")) = "y" Then
                acpDownloadableProducts.Visible = False
                acpSavedBaskets.Visible = False
                acpWishLists.Visible = False
                pnlSaveWishLists.Visible = False
                pnlSaveBasket.Visible = False
            Else
                'Show/hide order history
                If Val(KartSettingsManager.GetKartConfig("frontend.users.myaccount.orderhistory.max")) = 0 Then
                    acpOrderHistory.Visible = False
                Else
                    acpOrderHistory.Visible = True
                    Call Me.BuildNavigatePage("order")
                End If
                'Show/hide saved baskets
                If Val(KartSettingsManager.GetKartConfig("frontend.users.myaccount.savedbaskets.max")) = 0 Then
                    acpSavedBaskets.Visible = False
                Else
                    acpSavedBaskets.Visible = True
                    Call Me.BuildNavigatePage("basket")
                End If
                'Show/hide wishlists
                If LCase(KartSettingsManager.GetKartConfig("frontend.users.wishlists.enabled")) = "n" Or Val(KartSettingsManager.GetKartConfig("frontend.users.myaccount.wishlists.max")) = 0 Then
                    acpWishLists.Visible = False
                Else
                    acpWishLists.Visible = True
                    Call Me.BuildNavigatePage("wishlist")
                End If
                'Show/hide affiliates
                If LCase(KartSettingsManager.GetKartConfig("frontend.users.myaccount.affiliates.enabled")) = "n" Then
                    acpAffiliates.Visible = False
                Else
                    acpAffiliates.Visible = True
                End If
                'Show/hide mailing list
                If LCase(KartSettingsManager.GetKartConfig("frontend.users.mailinglist.enabled")) = "n" Then
                    acpMailingList.Visible = False
                Else
                    acpMailingList.Visible = True
                End If
            End If

            '--------------------------------------------------
            'Reset password link
            '--------------------------------------------------
            If Not MyBase.User.Identity.IsAuthenticated Then
                If Not String.IsNullOrEmpty(MyBase.Request.QueryString("ref")) Then
                    lblCurrentPassword.Text = Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "FormLabel_EmailAddress")
                    txtCurrentPassword.TextMode = Global.System.Web.UI.WebControls.TextBoxMode.SingleLine
                    Dim strRef As String = MyBase.Request.QueryString("ref")
                Else
                    MyBase.Response.Redirect(WebShopURL() & "Customer.aspx?action=password")
                End If
            Else

                '--------------------------------------------------
                'Setup addresses
                '--------------------------------------------------
                If Not IsPostBack Then
                    ViewState("lstUsrAddresses") = KartrisClasses.Address.GetAll(CurrentLoggedUser.ID)
                End If

                If ViewState("lstUsrAddresses") Is Nothing Then ViewState("lstUsrAddresses") = KartrisClasses.Address.GetAll(CurrentLoggedUser.ID)
                If mvwAddresses.ActiveViewIndex = "0" Then
                    If CurrentLoggedUser.DefaultBillingAddressID > 0 And Not IsNothing(CType(ViewState("lstUsrAddresses"), Global.System.Collections.Generic.List(Of KartrisClasses.Address)).Find(Function(p) p.ID = CurrentLoggedUser.DefaultBillingAddressID)) Then
                        UC_DefaultBilling.Address = CType(ViewState("lstUsrAddresses"), Global.System.Collections.Generic.List(Of KartrisClasses.Address)).Find(Function(p) p.ID = CurrentLoggedUser.DefaultBillingAddressID)
                        litContentTextNoAddress.Visible = False
                    Else
                        UC_DefaultBilling.Visible = False
                        litContentTextNoAddress.Visible = True
                    End If
                    If CurrentLoggedUser.DefaultShippingAddressID > 0 And Not IsNothing(CType(ViewState("lstUsrAddresses"), Global.System.Collections.Generic.List(Of KartrisClasses.Address)).Find(Function(p) p.ID = CurrentLoggedUser.DefaultShippingAddressID)) Then
                        UC_DefaultShipping.Address = CType(ViewState("lstUsrAddresses"), Global.System.Collections.Generic.List(Of KartrisClasses.Address)).Find(Function(p) p.ID = CurrentLoggedUser.DefaultShippingAddressID)
                        litContentTextNoAddress2.Visible = False
                    Else
                        UC_DefaultShipping.Visible = False
                        litContentTextNoAddress2.Visible = True
                    End If

                ElseIf mvwAddresses.ActiveViewIndex = "1" Then
                    Me.CreateBillingAddressesDetails()
                ElseIf mvwAddresses.ActiveViewIndex = "2" Then
                    Me.CreateShippingAddressesDetails()
                End If


            End If
            If Not Global.EmployeeManagementSystem.Page.IsPostBack Then
                '--------------------------------------------------
                'Set active tab
                '--------------------------------------------------
                Dim strQSAction As String = MyBase.Request.QueryString("action")
                Select Case strQSAction
                    Case "home"
                        tabContainerCustomer.ActiveTabIndex = 0
                    Case "details"
                        tabContainerCustomer.ActiveTabIndex = 1
                    Case "addresses"
                        tabContainerCustomer.ActiveTabIndex = 2
                    Case "password"
                        tabContainerCustomer.ActiveTabIndex = 3
                    Case "savebasket"
                        tabContainerCustomer.ActiveTabIndex = 0
                        phdSaveBasket.Visible = True
                        phdHome.Visible = False
                    Case "wishlists"
                        tabContainerCustomer.ActiveTabIndex = 0
                        phdSaveWishLists.Visible = True
                        phdHome.Visible = False
                    Case Else
                        tabContainerCustomer.ActiveTabIndex = 0
                End Select
                UC_Updated.reset()
            End If

            'acpAffiliates.Visible = False
            'acpDownloadableProducts.Visible = False
            'acpMailingList.Visible = False
            'acpOrderHistory.Visible = False
            'acpSavedBaskets.Visible = False
            'acpWishLists.Visible = False
        End Sub

        'We create two download links on the page (.aspx),
        'one is a linkbutton to trigger local file download,
        'the other is a hyperlink to a fully-qualified URL.
        'We use the code below to hide the one that isn't
        'needed when the item is databound.
        Protected Sub rptDownloadableProducts_ItemDataBound(ByVal sender As Object, ByVal e As Global.System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptDownloadableProducts.ItemDataBound
            Dim strDownloadType As String
            If e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.Item Or e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.AlternatingItem Then
                strDownloadType = e.Item.DataItem("V_DownloadType")
                If strDownloadType = "l" Then
                    'Full qualified path
                    'Hide link button
                    CType(e.Item.FindControl("lnkDownload"), Global.System.Web.UI.WebControls.LinkButton).Visible = False
                Else
                    'local file, type "u"
                    'Hide hyperlink
                    CType(e.Item.FindControl("hlnkDownload"), Global.System.Web.UI.WebControls.HyperLink).Visible = False
                End If
            End If
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.PreRender

            If Not IsPostBack Then

                phdSaveBasket.Visible = False
                phdHome.Visible = False
                phdSaveWishLists.Visible = False

                Me.dtbDownloadableProducts = BasketBLL.GetDownloadableProducts(Me.numCustomerID)
                If Me.dtbDownloadableProducts.Rows.Count > 0 Then
                    phdDownloadableProducts.Visible = True
                    rptDownloadableProducts.DataSource = Me.dtbDownloadableProducts
                    rptDownloadableProducts.DataBind()
                Else
                    phdDownloadableProducts.Visible = False
                    acpDownloadableProducts.Visible = False
                End If
                Select Case Global.Microsoft.VisualBasic.Strings.LCase(Me.strAction)
                    Case "savebasket"
                        Me.Title = Global.System.Web.HttpContext.GetGlobalResourceObject("Basket", "PageTitle_SaveRecoverBasketContents") & Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "PageTitle_Separator") & MyBase.Server.HtmlEncode(Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "Config_Webshopname"))
                        phdSaveBasket.Visible = True

                    Case "home"
                        Me.Title = Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "PageTitle_MyAccount") & Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "PageTitle_Separator") & MyBase.Server.HtmlEncode(Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "Config_Webshopname"))
                        phdHome.Visible = True

                        Dim tblCustomerData As Global.System.Data.DataTable

                        ''// initialize orders navigation page
                        Global.EmployeeManagementSystem.Customer.Order_PageSize = CInt(KartSettingsManager.GetKartConfig("frontend.users.myaccount.orderhistory.max"))
                        ViewState("Order_PageTotalSize") = BasketBLL.GetCustomerOrdersTotal(Me.numCustomerID)
                        ViewState("Order_PageIndex") = 1
                        lnkBtnOrderPrev.Enabled = False : lnkBtnOrderNext.Enabled = True
                        If Global.EmployeeManagementSystem.Customer.Order_PageSize >= ViewState("Order_PageTotalSize") Then
                            lnkBtnOrderPrev.Visible = False : lnkBtnOrderNext.Visible = False
                        End If

                        Call Me.BuildNavigatePage("order")

                        ''// initialize saved baskets navigation page
                        Global.EmployeeManagementSystem.Customer.SavedBasket_PageSize = CInt(KartSettingsManager.GetKartConfig("frontend.users.myaccount.savedbaskets.max"))
                        ViewState("SavedBasket_PageTotalSize") = BasketBLL.GetSavedBasketTotal(Me.numCustomerID)
                        ViewState("SavedBasket_PageIndex") = 1
                        lnkBtnBasketPrev.Enabled = False : lnkBtnBasketNext.Enabled = True
                        If Global.EmployeeManagementSystem.Customer.SavedBasket_PageSize >= ViewState("SavedBasket_PageTotalSize") Then
                            lnkBtnBasketPrev.Visible = False : lnkBtnBasketNext.Visible = False
                        End If

                        Call Me.BuildNavigatePage("basket")

                        ''// initialize wishlists navigation page
                        Global.EmployeeManagementSystem.Customer.WishList_PageSize = CInt(KartSettingsManager.GetKartConfig("frontend.users.myaccount.wishlists.max"))
                        ViewState("WishList_PageTotalSize") = BasketBLL.GetWishListTotal(Me.numCustomerID)
                        ViewState("WishList_PageIndex") = 1
                        lnkBtnWishlistPrev.Enabled = False : lnkBtnWishlistNext.Enabled = True
                        If Global.EmployeeManagementSystem.Customer.WishList_PageSize >= ViewState("WishList_PageTotalSize") Then
                            lnkBtnWishlistPrev.Visible = False : lnkBtnWishlistNext.Visible = False
                        End If

                        Call Me.BuildNavigatePage("wishlist")

                        Dim oItems As New Global.System.Collections.Generic.List(Of Kartris.BasketItem)
                        Me.objBasket.LoadBasketItems()
                        oItems = Me.objBasket.BasketItems
                        Global.EmployeeManagementSystem.Customer.blnNoBasketItem = (oItems.Count = 0)


                        tblCustomerData = BasketBLL.GetCustomerData(Me.numCustomerID)
                        If tblCustomerData.Rows.Count > 0 Then
                            ''// affiliate
                            Global.EmployeeManagementSystem.Customer.AFF_IsAffiliate = FixNullFromDB(tblCustomerData.Rows(0).Item("U_IsAffiliate"))
                            Global.EmployeeManagementSystem.Customer.AFF_AffiliateCommission = FixNullFromDB(tblCustomerData.Rows(0).Item("U_AffiliateCommission"))

                            ''// mailing list
                            Me.ML_ConfirmationDateTime = FixNullFromDB(tblCustomerData.Rows(0).Item("U_ML_ConfirmationDateTime"))
                            Me.ML_SignupDateTime = FixNullFromDB(tblCustomerData.Rows(0).Item("U_ML_SignupDateTime"))
                            Me.ML_SendMail = FixNullFromDB(tblCustomerData.Rows(0).Item("U_ML_SendMail"))
                            Me.ML_Format = FixNullFromDB(tblCustomerData.Rows(0).Item("U_ML_Format"))
                        End If

                        ddlMailingList.Items.Clear()
                        ddlMailingList.Items.Add(Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "ContentText_SendMailsOff").ToString)
                        ddlMailingList.Items.Add(Global.System.Web.HttpContext.GetGlobalResourceObject("Checkout", "ContentText_SendMailsPlain").ToString)
                        ddlMailingList.Items.Add(Global.System.Web.HttpContext.GetGlobalResourceObject("Checkout", "ContentText_SendMailsHTML").ToString)
                        ddlMailingList.Items(0).Value = "n"
                        ddlMailingList.Items(1).Value = "t"
                        ddlMailingList.Items(2).Value = "h"
                        If Me.ML_SendMail Then ddlMailingList.SelectedIndex = 0
                        If Me.ML_SendMail AndAlso Me.ML_Format <> "h" Then ddlMailingList.SelectedIndex = 1
                        If Me.ML_SendMail AndAlso Me.ML_Format = "h" Then ddlMailingList.SelectedIndex = 2


                    Case "wishlists"
                        Me.Title = Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "PageTitle_WishListLogin") & Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "PageTitle_Separator") & MyBase.Server.HtmlEncode(Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "Config_Webshopname"))
                        phdSaveWishLists.Visible = True

                        Dim numWishlistsID As Long

                        Try
                            numWishlistsID = IIf(MyBase.Request.QueryString("WL_ID") = "", 0, CInt(MyBase.Request.QueryString("WL_ID")))
                        Catch ex As Global.System.Exception
                            Global.System.Web.HttpContext.Current.Response.Redirect("~/Customer.aspx")
                        End Try

                        If numWishlistsID <> 0 Then
                            Me.tblWishLists = BasketBLL.GetCustomerWishList(Me.numCustomerID, numWishlistsID)

                            If Me.tblWishLists.Rows.Count > 0 Then
                                txtWL_Name.Text = FixNullFromDB(Me.tblWishLists.Rows(0).Item("WL_Name")) & ""
                                txtWL_PublicPassword.Text = FixNullFromDB(Me.tblWishLists.Rows(0).Item("WL_PublicPassword")) & ""
                                txtWL_Message.Text = FixNullFromDB(Me.tblWishLists.Rows(0).Item("WL_Message")) & ""
                            Else
                                Global.System.Web.HttpContext.Current.Response.Redirect("~/Customer.aspx")
                            End If

                            Me.tblWishLists = Nothing

                        Else
                            Dim tblCustomerData As New Global.System.Data.DataTable
                            tblCustomerData = BasketBLL.GetCustomerData(Me.numCustomerID)
                        End If
                    Case Else

                End Select


            Else ''postback

                Select Case Global.Microsoft.VisualBasic.Strings.LCase(Me.strAction)
                    Case "savebasket"
                        Me.Title = Global.System.Web.HttpContext.GetGlobalResourceObject("Basket", "PageTitle_SaveRecoverBasketContents") & Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "PageTitle_Separator") & MyBase.Server.HtmlEncode(Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "Config_Webshopname"))

                    Case "home"
                        Me.Title = Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "PageTitle_MyAccount") & Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "PageTitle_Separator") & MyBase.Server.HtmlEncode(Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "Config_Webshopname"))

                    Case "wishlists"
                        Me.Title = Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "PageTitle_WishListLogin") & Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "PageTitle_Separator") & MyBase.Server.HtmlEncode(Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "Config_Webshopname"))

                    Case Else

                End Select


            End If

        End Sub

        Sub RemoveSavedBasket_Click(ByVal Sender As Object, ByVal E As Global.System.Web.UI.WebControls.CommandEventArgs)
            Dim numBasketID As Long
            numBasketID = E.CommandArgument
            BasketBLL.DeleteSavedBasket(numBasketID)
            Call Me.BuildNavigatePage("basket")
            UC_Updated.ShowAnimatedText()
        End Sub

        Sub LoadSavedBasket_Click(ByVal Sender As Object, ByVal E As Global.System.Web.UI.WebControls.CommandEventArgs)
            Dim numSavedBasketID, numBasketID As Long
            numSavedBasketID = E.CommandArgument
            numBasketID = Me.SESSION_ID
            BasketBLL.LoadSavedBasket(numSavedBasketID, numBasketID)
            Global.EmployeeManagementSystem.Customer.blnNoBasketItem = Me.objBasket.BasketItems.Count > 0
            Call Me.RefreshMiniBasket()
        End Sub

        Sub SaveBasket_Click(ByVal Sender As Object, ByVal E As Global.System.Web.UI.WebControls.CommandEventArgs)
            If Me.IsValid Then
                Call BasketBLL.SaveBasket(Me.numCustomerID, Trim(txtBasketName.Text), Me.SESSION_ID)
                UC_Updated.ShowAnimatedText()
                accMyAccount.SelectedIndex = 2
                phdSaveBasket.Visible = False
                phdHome.Visible = True
                Call Me.BuildNavigatePage("basket")
                updMain.Update()
            End If
        End Sub

        Sub SaveWishLists_Click(ByVal Sender As Object, ByVal E As Global.System.Web.UI.WebControls.CommandEventArgs)
            Dim strName, strPublicPassword, strMessage As String
            Dim strErrorMsg As New Global.System.Text.StringBuilder("")

            strName = txtWL_Name.Text
            strPublicPassword = txtWL_PublicPassword.Text
            strMessage = txtWL_Message.Text

            If Me.IsValid Then
                Dim tblWishLists As New Global.System.Data.DataTable

                Dim numWishlistsID As Long = IIf(MyBase.Request.QueryString("WL_ID") = "", 0, CInt(MyBase.Request.QueryString("WL_ID")))

                If numWishlistsID = 0 Then

                    Dim strEmail As String = ""
                    strEmail = CurrentLoggedUser.Email

                    tblWishLists = BasketBLL.GetWishListLogin(strEmail, strPublicPassword)
                    If tblWishLists.Rows.Count > 0 Then ''// password already exist for this owner
                        With UC_PopUpInfo
                            Dim strError As String = Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "ContentText_WishListPublicPasswordExists")
                            strError = Global.Microsoft.VisualBasic.Strings.Replace(strError, "<label>", Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "ContentText_WishListPublicPassword"))
                            .SetTitle = Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "PageTitle_WishListLogin")
                            .SetTextMessage = strError
                            .ShowPopup()
                        End With
                    Else ''// new wishlist (create it)
                        Call BasketBLL.SaveWishLists(numWishlistsID, Me.SESSION_ID, Me.numCustomerID, strName, strPublicPassword, strMessage)
                        UC_Updated.ShowAnimatedText()
                        accMyAccount.SelectedIndex = 3
                        phdSaveWishLists.Visible = False
                        phdHome.Visible = True
                        Call Me.BuildNavigatePage("wishlist")
                        updMain.Update()
                    End If

                Else ''// existing wishlist (update it)
                    Call BasketBLL.SaveWishLists(numWishlistsID, Me.SESSION_ID, Me.numCustomerID, strName, strPublicPassword, strMessage)
                    UC_Updated.ShowAnimatedText()
                    accMyAccount.SelectedIndex = 3
                    phdSaveWishLists.Visible = False
                    phdHome.Visible = True
                    Call Me.BuildNavigatePage("wishlist")
                    updMain.Update()
                End If

            End If

        End Sub

        Protected Sub lnkDownload_Click(ByVal Sender As Object, ByVal E As Global.System.Web.UI.WebControls.CommandEventArgs)
            DownloadFile(E.CommandArgument)
        End Sub

        Sub RemoveWishLists_Click(ByVal Sender As Object, ByVal E As Global.System.Web.UI.WebControls.CommandEventArgs)
            Dim numWishListsID As Long

            numWishListsID = E.CommandArgument
            BasketBLL.DeleteWishLists(numWishListsID)
            Call Me.BuildNavigatePage("wishlist")
            UC_Updated.ShowAnimatedText()
        End Sub

        Sub EditWishLists_Click(ByVal Sender As Object, ByVal E As Global.System.Web.UI.WebControls.CommandEventArgs)
            Dim numWishListsID As Long
            numWishListsID = E.CommandArgument
            MyBase.Response.Redirect("~/Customer.aspx?action=wishlists&WL_ID=" & numWishListsID)
        End Sub

        Sub LoadWishLists_Click(ByVal Sender As Object, ByVal E As Global.System.Web.UI.WebControls.CommandEventArgs)
            Dim numWishListsID, numBasketID As Long
            numWishListsID = E.CommandArgument
            numBasketID = Me.SESSION_ID
            BasketBLL.LoadWishlists(numWishListsID, numBasketID)
            Global.EmployeeManagementSystem.Customer.blnNoBasketItem = Me.objBasket.BasketItems.Count > 0
            Call Me.RefreshMiniBasket()
            UC_Updated.ShowAnimatedText()
        End Sub

        Sub RefreshMiniBasket()
            Dim objMaster As Global.System.Web.UI.MasterPage
            Dim objBasket As Object
            objMaster = Global.EmployeeManagementSystem.Page.Master
            objBasket = objMaster.FindControl("UC_MiniBasket")
            objBasket.LoadMiniBasket()
        End Sub

        Function GetCustomerDiscount() As Double
            Dim numCustomerID As Integer
            numCustomerID = Global.Microsoft.VisualBasic.Conversion.Val(Me.SESSION_ID)
            Return BasketBLL.GetCustomerDiscount(numCustomerID)
        End Function

        Protected Sub PageNavigate_Click(ByVal sender As Object, ByVal E As Global.System.Web.UI.WebControls.CommandEventArgs)

            Select Case Global.Microsoft.VisualBasic.Strings.LCase(E.CommandName)
                Case "order"
                    If LCase(sender.id) = "lnkbtnorderprev" Then
                        ViewState("Order_PageIndex") = ViewState("Order_PageIndex") - 1
                    ElseIf LCase(sender.id) = "lnkbtnordernext" Then
                        ViewState("Order_PageIndex") = ViewState("Order_PageIndex") + 1
                    End If
                    Call Me.BuildNavigatePage("order")
                    lnkBtnOrderPrev.Enabled = IIf(ViewState("Order_PageIndex") <= 1, False, True)
                    lnkBtnOrderNext.Enabled = IIf(ViewState("Order_PageIndex") >= ViewState("Order_PageTotalSize") / Global.EmployeeManagementSystem.Customer.Order_PageSize, False, True)

                Case "basket"
                    If LCase(sender.id) = "lnkbtnbasketprev" Then
                        ViewState("SavedBasket_PageIndex") = ViewState("SavedBasket_PageIndex") - 1
                    ElseIf LCase(sender.id) = "lnkbtnbasketnext" Then
                        ViewState("SavedBasket_PageIndex") = ViewState("SavedBasket_PageIndex") + 1
                    End If
                    Call Me.BuildNavigatePage("basket")
                    lnkBtnBasketPrev.Enabled = IIf(ViewState("SavedBasket_PageIndex") <= 1, False, True)
                    lnkBtnBasketNext.Enabled = IIf(ViewState("SavedBasket_PageIndex") >= ViewState("SavedBasket_PageTotalSize") / Global.EmployeeManagementSystem.Customer.SavedBasket_PageSize, False, True)

                Case "wishlist"
                    If LCase(sender.id) = "lnkbtnwishlistprev" Then
                        ViewState("WishList_PageIndex") = ViewState("WishList_PageIndex") - 1
                    ElseIf LCase(sender.id) = "lnkbtnwishlistnext" Then
                        ViewState("WishList_PageIndex") = ViewState("WishList_PageIndex") + 1
                    End If
                    Call Me.BuildNavigatePage("wishlist")
                    lnkBtnWishlistPrev.Enabled = IIf(ViewState("WishList_PageIndex") <= 1, False, True)
                    lnkBtnWishlistNext.Enabled = IIf(ViewState("WishList_PageIndex") >= ViewState("WishList_PageTotalSize") / Global.EmployeeManagementSystem.Customer.WishList_PageSize, False, True)

            End Select

        End Sub

        Private Sub BuildNavigatePage(ByVal strPage As String)

            Select Case Global.Microsoft.VisualBasic.Strings.LCase(strPage)
                Case "order"
                    Me.tblOrder = BasketBLL.GetCustomerOrders(Me.numCustomerID, (ViewState("Order_PageIndex") - 1) * Global.EmployeeManagementSystem.Customer.Order_PageSize, Global.EmployeeManagementSystem.Customer.Order_PageSize)
                    rptOrder.DataSource = Me.tblOrder
                    rptOrder.DataBind()
                    updMain.Update()

                Case "basket"
                    Me.tblSavedBasket = BasketBLL.GetSavedBasket(Me.numCustomerID, (ViewState("SavedBasket_PageIndex") - 1) * Global.EmployeeManagementSystem.Customer.SavedBasket_PageSize, Global.EmployeeManagementSystem.Customer.SavedBasket_PageSize)
                    rptSavedBasket.DataSource = Me.tblSavedBasket
                    rptSavedBasket.DataBind()
                    updMain.Update()

                Case "wishlist"
                    Me.tblWishLists = BasketBLL.GetWishLists(Me.numCustomerID, (ViewState("WishList_PageIndex") - 1) * Global.EmployeeManagementSystem.Customer.WishList_PageSize, Global.EmployeeManagementSystem.Customer.WishList_PageSize)
                    rptWishLists.DataSource = Me.tblWishLists
                    rptWishLists.DataBind()
                    updMain.Update()

            End Select

        End Sub

        Sub Affiliate_Click(ByVal Sender As Object, ByVal E As Global.System.Web.UI.WebControls.CommandEventArgs)
            Dim tblAffiliates As Global.System.Data.DataTable

            With UC_PopUpInfo
                .SetTitle = Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "PageTitle_Affiliates")
                .SetTextMessage = Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "ContentText_AffiliateApplicationDetail")
                .ShowPopup()
            End With

            AffiliateBLL.UpdateCustomerAffiliateStatus(Me.numCustomerID)

            tblAffiliates = BasketBLL.GetCustomerData(Me.numCustomerID)
            If tblAffiliates.Rows.Count > 0 Then
                Global.EmployeeManagementSystem.Customer.AFF_IsAffiliate = tblAffiliates.Rows(0).Item("U_IsAffiliate")
                Global.EmployeeManagementSystem.Customer.AFF_AffiliateCommission = tblAffiliates.Rows(0).Item("U_AffiliateCommission")
            End If

            updMain.Update()

        End Sub

        Sub MailingList_Click(ByVal Sender As Object, ByVal E As Global.System.Web.UI.WebControls.CommandEventArgs)
            Dim strCommand As String
            Dim tblCustomerData As New Global.System.Data.DataTable

            strCommand = E.CommandName

            If Global.Microsoft.VisualBasic.Strings.LCase(strCommand) = Global.Microsoft.VisualBasic.Strings.LCase("MailVerified") Then

                BasketBLL.UpdateCustomerMailFormat(Me.numCustomerID, LCase(ddlMailingList.SelectedValue))

                With UC_PopUpMessage
                    .SetTitle = Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "PageTitle_MailingList")
                    .SetTextMessage = Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "ContentText_PreferencesChanged")
                    .ShowPopup()
                End With

            Else ''// mail not verified or mail not signed up

                tblCustomerData = BasketBLL.GetCustomerData(Me.numCustomerID)
                Dim strEmail As String = "", strPassword As String = ""

                If tblCustomerData.Rows.Count > 0 Then
                    strEmail = FixNullFromDB(tblCustomerData.Rows(0).Item("U_EmailAddress")) & ""
                End If

                'Update user in our db
                BasketBLL.UpdateCustomerMailingList(strEmail, strPassword)

                'If mailchimp is active, we want to add the user to the mailing list
                If KartSettingsManager.GetKartConfig("general.mailchimp.enabled") = "y" Then
                    'Add user direct to MailChimp
                    BasketBLL.AddListSubscriber(strEmail)
                Else
                    'Use the built in mailing list
                    Dim sbdBodyText As Global.System.Text.StringBuilder = New Global.System.Text.StringBuilder()
                    Dim strBodyText As String
                    Dim strMailingListSignUpLink As String = WebShopURL() & "Default.aspx?id=" & Me.numCustomerID & "&r=" & strPassword

                    sbdBodyText.Append(Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "EmailText_NewsletterSignup") & Global.Microsoft.VisualBasic.Constants.vbCrLf & Global.Microsoft.VisualBasic.Constants.vbCrLf)
                    sbdBodyText.Append(strMailingListSignUpLink & Global.Microsoft.VisualBasic.Constants.vbCrLf & Global.Microsoft.VisualBasic.Constants.vbCrLf)
                    sbdBodyText.Append(Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "EmailText_NewsletterAuthorizeFooter"))

                    strBodyText = sbdBodyText.ToString
                    strBodyText = Replace(strBodyText, "[IPADDRESS]", MyBase.Request.UserHostAddress())
                    strBodyText = Global.Microsoft.VisualBasic.Strings.Replace(strBodyText, "[WEBSHOPNAME]", Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "Config_Webshopname"))
                    strBodyText = Replace(strBodyText, "[WEBSHOPURL]", WebShopURL)
                    strBodyText = strBodyText & Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "ContentText_NewsletterSignup")

                    Dim strFrom As String = LanguagesBLL.GetEmailFrom(GetLanguageIDfromSession)

                    Dim blnHTMLEmail As Boolean = KartSettingsManager.GetKartConfig("general.email.enableHTML") = "y"
                    If blnHTMLEmail Then
                        Dim strHTMLEmailText As String = RetrieveHTMLEmailTemplate("MailingListSignUp")
                        'build up the HTML email if template is found
                        If Not String.IsNullOrEmpty(strHTMLEmailText) Then
                            strHTMLEmailText = strHTMLEmailText.Replace("[mailinglistconfirmationlink]", strMailingListSignUpLink)
                            strHTMLEmailText = strHTMLEmailText.Replace("[websitename]", Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "Config_Webshopname"))
                            strHTMLEmailText = strHTMLEmailText.Replace("[customerip]", MyBase.Request.UserHostAddress())
                            strBodyText = strHTMLEmailText
                        Else
                            blnHTMLEmail = False
                        End If
                    End If

                    'GDPR Mod - v2.9013 
                    'We want to be able to have a log of all opt-in
                    'requests sent, so we can prove the user was sent
                    'the GDPR notice, and also prove what text they
                    'received. We do this by BCCing the confirmation
                    'opt-in mail to an email address. A free account
                    'like gmail would be good for this.
                    Dim objBCCsCollection As New Global.System.Net.Mail.MailAddressCollection
                    Dim strGDPROptinArchiveEmail As String = KartSettingsManager.GetKartConfig("general.gdpr.mailinglistbcc")
                    If strGDPROptinArchiveEmail.Length > 0 Then
                        objBCCsCollection.Add(strGDPROptinArchiveEmail)
                    End If
                    SendEmail(strFrom, strEmail, Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "PageTitle_MailingList"), strBodyText, , , , , blnHTMLEmail,, objBCCsCollection)
                End If

            End If

            tblCustomerData = BasketBLL.GetCustomerData(Me.numCustomerID)
            If tblCustomerData.Rows.Count Then
                Me.ML_ConfirmationDateTime = FixNullFromDB(tblCustomerData.Rows(0).Item("U_ML_ConfirmationDateTime"))
                Me.ML_SignupDateTime = FixNullFromDB(tblCustomerData.Rows(0).Item("U_ML_SignupDateTime"))
                Me.ML_SendMail = FixNullFromDB(tblCustomerData.Rows(0).Item("U_ML_SendMail"))
                Me.ML_Format = FixNullFromDB(tblCustomerData.Rows(0).Item("U_ML_Format"))
            End If

            updMain.Update()
        End Sub

        Protected Sub rptOrder_ItemDataBound(ByVal sender As Object, ByVal e As Global.System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptOrder.ItemDataBound
            Dim numTotal As Double
            If e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.Item Or e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.AlternatingItem Then
                CType(e.Item.FindControl("lnkBtnOrderView"), Global.System.Web.UI.WebControls.LinkButton).PostBackUrl = "~/CustomerViewOrder.aspx?O_ID=" & e.Item.DataItem("O_ID")
                CType(e.Item.FindControl("lnkBtnOrderInvoice"), Global.System.Web.UI.WebControls.LinkButton).PostBackUrl = "~/CustomerInvoice.aspx?O_ID=" & e.Item.DataItem("O_ID")
                numTotal = e.Item.DataItem("O_TotalPrice")
                CType(e.Item.FindControl("litOrdersTotal"), Global.System.Web.UI.WebControls.Literal).[Text] = e.Item.DataItem("CUR_Symbol") & Global.Microsoft.VisualBasic.Strings.FormatNumber(Global.System.Math.Round(numTotal, e.Item.DataItem("CUR_RoundNumbers")), 2)
            End If

        End Sub

        Protected Sub rptSavedBasket_ItemDataBound(ByVal sender As Object, ByVal e As Global.System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptSavedBasket.ItemDataBound

            If e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.Item Or e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.AlternatingItem Then
                CType(e.Item.FindControl("divBasketName"), Global.System.Web.UI.HtmlControls.HtmlGenericControl).InnerText = e.Item.DataItem("SBSKT_Name")
            End If

        End Sub

        Protected Sub rptWishLists_ItemDataBound(ByVal sender As Object, ByVal e As Global.System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptWishLists.ItemDataBound

            If e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.Item Or e.Item.ItemType = Global.System.Web.UI.WebControls.ListItemType.AlternatingItem Then
                CType(e.Item.FindControl("spnWishListName"), Global.System.Web.UI.HtmlControls.HtmlGenericControl).InnerText = e.Item.DataItem("WL_Name")
            End If

        End Sub

        ''' <summary>
        ''' Submit new password
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub btnPasswordSubmit_Click(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles btnPasswordSubmit.Click
            Dim objUsersBLL As New UsersBLL
            If String.IsNullOrEmpty(MyBase.Request.QueryString("ref")) Then
                Dim strUserPassword As String = txtCurrentPassword.Text
                Dim strNewPassword As String = txtNewPassword.Text

                'Only update if validators ok
                If Me.IsValid Then
                    If Global.System.Web.Security.Membership.ValidateUser(CurrentLoggedUser.Email, strUserPassword) Then
                        If objUsersBLL.ChangePassword(CurrentLoggedUser.ID, strUserPassword, strNewPassword) > 0 Then UC_Updated.ShowAnimatedText()
                    Else
                        Dim strErrorMessage As String = Global.Microsoft.VisualBasic.Strings.Replace(Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "ContentText_CustomerCodeIncorrect"), "[label]", Global.System.Web.HttpContext.GetGlobalResourceObject("Login", "FormLabel_ExistingCustomerCode"))
                        litWrongPassword.Text = "<span class=""error"">" & strErrorMessage & "</span>"
                    End If
                End If

            Else
                Dim strRef As String = MyBase.Request.QueryString("ref")
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
                                MyBase.Response.Redirect(WebShopURL() & "CustomerAccount.aspx?m=u")
                            Else
                                litWrongPassword.Text = "<span class=""error"">" & Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "ContentText_ErrorText") & "</span>"
                            End If
                        Else
                            litWrongPassword.Text = "<span class=""error"">" & Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "ContentText_LinkExpiredOrIncorrect") & "</span>"
                        End If

                    Else
                        litWrongPassword.Text = "<span class=""error"">" & Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "ContentText_LinkExpiredOrIncorrect") & "</span>"
                    End If

                Else
                    litWrongPassword.Text = "<span class=""error"">" & Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "ContentText_NotFoundInDB") & "</span>"
                End If
            End If

        End Sub

        ''' <summary>
        ''' Submit new details (general customer details tab)
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub btnDetailsSubmit_Click(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles btnDetailsSubmit.Click
            Global.EmployeeManagementSystem.Page.Validate("NameAndVat")
            If Global.EmployeeManagementSystem.Page.IsValid Then
                Dim objUsersBLL As New UsersBLL
                If objUsersBLL.UpdateNameandEUVAT(CurrentLoggedUser.ID, txtCustomerName.Text, txtEUVATNumber.Text) = CurrentLoggedUser.ID Then UC_Updated.ShowAnimatedText()

                'EORI number
                Dim objObjectConfigBLL As New ObjectConfigBLL
                Dim blnAddedEORI As Boolean = objObjectConfigBLL._SetConfigValue(11, CurrentLoggedUser.ID, txtEORINumber.Text, "")
            End If
        End Sub

        ''' <summary>
        ''' Addresses - load complete
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub Page_LoadComplete(ByVal sender As Global.System.[Object], ByVal e As Global.System.EventArgs) Handles Me.LoadComplete
            If mvwAddresses.ActiveViewIndex > 0 Then btnBack.Visible = True Else btnBack.Visible = False
        End Sub

        ''' <summary>
        ''' Addresses - edit billing click
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub lnkEditBilling_Click(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles lnkEditBilling.Click

            mvwAddresses.ActiveViewIndex = "1"
            Me.CreateBillingAddressesDetails()
            'UC_NewEditAddress.DisplayType = "Billing"
            UC_NewEditAddress.ValidationGroup = "Billing"
            btnSaveNewAddress.OnClientClick = "Page_ClientValidate('Billing');"
            UC_Updated.reset()
        End Sub


        ''' <summary>
        ''' Addresses - edit shipping click
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub lnkEditShipping_Click(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles lnkEditShipping.Click

            mvwAddresses.ActiveViewIndex = "2"
            Me.CreateShippingAddressesDetails()
            'UC_NewEditAddress.DisplayType = "Shipping"
            UC_NewEditAddress.ValidationGroup = "Shipping"
            btnSaveNewAddress.OnClientClick = "Page_ClientValidate('Shipping');"
            UC_Updated.reset()
        End Sub

        Protected Sub btnBack_Click(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles btnBack.Click
            mvwAddresses.ActiveViewIndex = "0"
            If CurrentLoggedUser.DefaultBillingAddressID > 0 And Not IsNothing(CType(ViewState("lstUsrAddresses"), Global.System.Collections.Generic.List(Of KartrisClasses.Address)).Find(Function(p) p.ID = CurrentLoggedUser.DefaultBillingAddressID)) Then
                UC_DefaultBilling.Address = CType(ViewState("lstUsrAddresses"), Global.System.Collections.Generic.List(Of KartrisClasses.Address)).Find(Function(p) p.ID = CurrentLoggedUser.DefaultBillingAddressID)
                litContentTextNoAddress.Visible = False
            Else
                UC_DefaultBilling.Visible = False
                litContentTextNoAddress.Visible = True
            End If
            If CurrentLoggedUser.DefaultShippingAddressID > 0 And Not IsNothing(CType(ViewState("lstUsrAddresses"), Global.System.Collections.Generic.List(Of KartrisClasses.Address)).Find(Function(p) p.ID = CurrentLoggedUser.DefaultShippingAddressID)) Then
                UC_DefaultShipping.Address = CType(ViewState("lstUsrAddresses"), Global.System.Collections.Generic.List(Of KartrisClasses.Address)).Find(Function(p) p.ID = CurrentLoggedUser.DefaultShippingAddressID)
                litContentTextNoAddress2.Visible = False
            Else
                UC_DefaultShipping.Visible = False
                litContentTextNoAddress2.Visible = True
            End If
            UC_Updated.reset()
        End Sub

        ''' <summary>
        ''' Addresses - save new address
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub btnSaveNewAddress_Click(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles btnSaveNewAddress.Click
            If UC_NewEditAddress.AddressType = "s" Then
                Global.EmployeeManagementSystem.Page.Validate("Shipping")
            End If
            If UC_NewEditAddress.AddressType = "b" Then
                Global.EmployeeManagementSystem.Page.Validate("Billing")
            End If
            If UC_NewEditAddress.AddressType = "u" Then
                Global.EmployeeManagementSystem.Page.Validate("Shipping")
            End If

            If Global.EmployeeManagementSystem.Page.IsValid Then

                If chkAlso.Checked = False Then
                    If UC_NewEditAddress.DisplayType = "Billing" Then UC_NewEditAddress.AddressType = "b" Else UC_NewEditAddress.AddressType = "s"
                Else
                    UC_NewEditAddress.AddressType = "u"
                End If

                Dim intGeneratedAddressID As Integer = KartrisClasses.Address.AddUpdate(UC_NewEditAddress.EnteredAddress, CurrentLoggedUser.ID, chkMakeDefault.Checked, UC_NewEditAddress.EnteredAddress.ID)
                If intGeneratedAddressID > 0 Then

                    Dim objAddress As KartrisClasses.Address = UC_NewEditAddress.EnteredAddress
                    objAddress.ID = intGeneratedAddressID

                    If UC_NewEditAddress.EnteredAddress.ID > 0 Then
                        Dim AddresstobeDeleted As KartrisClasses.Address = CType(ViewState("lstUsrAddresses"), Global.System.Collections.Generic.List(Of KartrisClasses.Address)).Find(Function(p) p.ID = intGeneratedAddressID)
                        CType(ViewState("lstUsrAddresses"), Global.System.Collections.Generic.List(Of KartrisClasses.Address)).Remove(AddresstobeDeleted)
                    End If

                    CType(ViewState("lstUsrAddresses"), Global.System.Collections.Generic.List(Of KartrisClasses.Address)).Add(objAddress)
                    If UC_NewEditAddress.DisplayType = "Billing" Then
                        pnlBilling.Controls.Clear()
                        Me.CreateBillingAddressesDetails()
                    Else
                        pnlShipping.Controls.Clear()
                        Me.CreateShippingAddressesDetails()
                    End If

                    If chkMakeDefault.Checked Then
                        If UC_NewEditAddress.DisplayType = "Billing" Then CurrentLoggedUser.DefaultBillingAddressID = intGeneratedAddressID Else CurrentLoggedUser.DefaultShippingAddressID = intGeneratedAddressID
                        If chkAlso.Checked Then If UC_NewEditAddress.DisplayType = "Billing" Then CurrentLoggedUser.DefaultShippingAddressID = intGeneratedAddressID
                    End If
                    Me.ResetAddressInput()
                    UC_Updated.ShowAnimatedText()
                End If
            Else
                popExtender.Show()
            End If
        End Sub

        ''' <summary>
        ''' Addresses - click "add address"
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub btnAddAddress_Click(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles lnkAddBilling.Click, lnkAddShipping.Click
            UC_Updated.reset()
            Me.ResetAddressInput()
            If UC_NewEditAddress.DisplayType = "Billing" Then
                UC_NewEditAddress.ValidationGroup = "Billing"
                chkAlso.Text = Global.System.Web.HttpContext.GetGlobalResourceObject("Address", "ContentText_CanAlsoBeUsedAsShippingAddress")
            Else
                UC_NewEditAddress.ValidationGroup = "Shipping"
                chkAlso.Text = Global.System.Web.HttpContext.GetGlobalResourceObject("Address", "ContentText_CanAlsoBeUsedAsBillingAddress")
            End If

            litAddressTitle.Text = Global.System.Web.HttpContext.GetGlobalResourceObject("Address", "ContentText_AddEditAddress")
            popExtender.Show()
        End Sub

        ''' <summary>
        ''' Addresses - click "edit address"
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub btnEditAddress_Click(ByVal sender As Object, ByVal e As Global.System.EventArgs)

            UC_Updated.reset()
            litAddressTitle.Text = Global.System.Web.HttpContext.GetGlobalResourceObject("Kartris", "ContentText_Edit")
            Dim lnkEdit As Global.System.Web.UI.WebControls.LinkButton = CType(sender, Global.System.Web.UI.WebControls.LinkButton)

            UC_NewEditAddress.InitialAddressToDisplay = CType(lnkEdit.Parent.Parent, UserControls_General_AddressDetails).Address

            If UC_NewEditAddress.DisplayType = "Billing" Then
                chkAlso.Text = Global.System.Web.HttpContext.GetGlobalResourceObject("Address", "ContentText_CanAlsoBeUsedAsShippingAddress")
                If CurrentLoggedUser.DefaultBillingAddressID = CType(lnkEdit.Parent.Parent, UserControls_General_AddressDetails).Address.ID Then chkMakeDefault.Checked = True
                If CurrentLoggedUser.DefaultBillingAddressID = UC_NewEditAddress.EnteredAddress.ID Then chkMakeDefault.Checked = True Else chkMakeDefault.Checked = False
                UC_NewEditAddress.ValidationGroup = "Billing"
            Else
                chkAlso.Text = Global.System.Web.HttpContext.GetGlobalResourceObject("Address", "ContentText_CanAlsoBeUsedAsBillingAddress")
                If CurrentLoggedUser.DefaultShippingAddressID = CType(lnkEdit.Parent.Parent, UserControls_General_AddressDetails).Address.ID Then chkMakeDefault.Checked = True
                If CurrentLoggedUser.DefaultShippingAddressID = UC_NewEditAddress.EnteredAddress.ID Then chkMakeDefault.Checked = True Else chkMakeDefault.Checked = False
                UC_NewEditAddress.ValidationGroup = "Shipping"
            End If

            If UC_NewEditAddress.EnteredAddress.Type = "u" Then chkAlso.Checked = True Else chkAlso.Checked = False
            UC_Updated.reset()
            popExtender.Show()
        End Sub

        ''' <summary>
        ''' Addresses - delete address click
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub btnDeleteAddress_Click(ByVal sender As Object, ByVal e As Global.System.EventArgs)
            Dim UC_Delete As UserControls_General_AddressDetails = CType(CType(sender, Global.System.Web.UI.WebControls.LinkButton).Parent.Parent, UserControls_General_AddressDetails)
            Dim intAddressID As Integer = UC_Delete.Address.ID
            Dim objAddress As KartrisClasses.Address = UC_Delete.Address

            If KartrisClasses.Address.Delete(intAddressID, CurrentLoggedUser.ID) > 0 Then
                CType(ViewState("lstUsrAddresses"), Global.System.Collections.Generic.List(Of KartrisClasses.Address)).Remove(objAddress)
                If UC_NewEditAddress.DisplayType = "Shipping" Then pnlShipping.Controls.Remove(UC_Delete) Else pnlBilling.Controls.Remove(UC_Delete)
            End If
        End Sub

        ''' <summary>
        ''' Addresses - refresh
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ResetAddressInput()
            UC_NewEditAddress.Clear()
            chkMakeDefault.Checked = False
            chkAlso.Checked = False
        End Sub

        ''' <summary>
        ''' Addresses - create shipping address
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CreateShippingAddressesDetails()
            Dim lstShippingAddresses As Global.System.Collections.Generic.List(Of KartrisClasses.Address) = CType(ViewState("lstUsrAddresses"), Global.System.Collections.Generic.List(Of KartrisClasses.Address)).FindAll(Function(ShippingAdd) ShippingAdd.Type = "s" Or ShippingAdd.Type = "u")
            For Each objAddress In lstShippingAddresses
                Dim UC_Shipping As UserControls_General_AddressDetails = DirectCast(LoadControl("~/UserControls/General/AddressDetails.ascx"), UserControls_General_AddressDetails)
                UC_Shipping.ID = "dtlShipping-" & objAddress.ID
                pnlShipping.Controls.Add(UC_Shipping)

                Dim UC_ShippingInstance As UserControls_General_AddressDetails = DirectCast(mvwAddresses.Views(2).FindControl("dtlShipping-" & objAddress.ID), UserControls_General_AddressDetails)
                UC_ShippingInstance.Address = objAddress
                UC_ShippingInstance.ShowButtons = True
                AddHandler UC_ShippingInstance.btnEditClicked, AddressOf Me.btnEditAddress_Click
                AddHandler UC_ShippingInstance.btnDeleteClicked, AddressOf Me.btnDeleteAddress_Click
            Next
        End Sub

        ''' <summary>
        ''' Addresses - create billing address
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CreateBillingAddressesDetails()
            Dim lstBillingAddresses As Global.System.Collections.Generic.List(Of KartrisClasses.Address) = CType(ViewState("lstUsrAddresses"), Global.System.Collections.Generic.List(Of KartrisClasses.Address)).FindAll(Function(BillingAdd) BillingAdd.Type = "b" Or BillingAdd.Type = "u")
            For Each objAddress In lstBillingAddresses
                Dim UC_Billing As UserControls_General_AddressDetails = DirectCast(LoadControl("~/UserControls/General/AddressDetails.ascx"), UserControls_General_AddressDetails)
                UC_Billing.ID = "UC_Billing-" & objAddress.ID
                pnlBilling.Controls.Add(UC_Billing)
                Dim UC_BillingInstance As UserControls_General_AddressDetails = CType(mvwAddresses.Views(1).FindControl("UC_Billing-" & objAddress.ID), UserControls_General_AddressDetails)
                UC_BillingInstance.Address = objAddress
                UC_BillingInstance.ShowButtons = True
                AddHandler UC_BillingInstance.btnEditClicked, AddressOf Me.btnEditAddress_Click
                AddHandler UC_BillingInstance.btnDeleteClicked, AddressOf Me.btnDeleteAddress_Click
            Next
        End Sub
    End Class

     */
}