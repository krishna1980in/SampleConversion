using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
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
using CkartrisBLL;
using CkartrisDataManipulation;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

internal partial class Customer : PageBaseClass
{

    protected static bool blnNoBasketItem;
    protected double numCustomerDiscount;
    protected static bool AFF_IsAffiliate;
    protected static double AFF_AffiliateCommission;
    protected DateTime ML_SignupDateTime, ML_ConfirmationDateTime;

    private kartris.Basket objBasket = new kartris.Basket();
    private int numAppMaxOrders, numAppMaxBaskets;
    private string strAppUploadsFolder, strShow;
    private System.Data.DataTable tblOrder, tblSavedBasket, tblWishLists, dtbDownloadableProducts;
    private int numCustomerID;
    private bool ML_SendMail;
    private string ML_Format;
    private string strAction;
    private int SESSION_ID;

    private static int Order_PageSize;
    private static int SavedBasket_PageSize;
    private static int WishList_PageSize;

    public Customer()
    {
        this.Load += Page_Load;
        this.PreRender += Page_PreRender;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        strAction = LCase(Request.QueryString("action"));
        if (strAction != "savebasket" && strAction != "wishlists")
            strAction = "home";

        SESSION_ID = Session("SessionID");

        // --------------------------------------------------
        // Redirect if not logged in
        // --------------------------------------------------
        if (!User.Identity.IsAuthenticated)
        {
            Response.Redirect("~/CustomerAccount.aspx");
        }
        else
        {
            numCustomerID = CurrentLoggedUser.ID;
        }

        // --------------------------------------------------
        // Fill customer details
        // --------------------------------------------------
        var objUsersBLL = new UsersBLL();
        if (!IsPostBack)
        {
            Array arrNameAndVAT = Strings.Split(objUsersBLL.GetNameandEUVAT(CurrentLoggedUser.ID), "|||");
            txtCustomerName.Text = arrNameAndVAT(0);
            txtEUVATNumber.Text = arrNameAndVAT(1);

            // v3.0001 EORI number
            var objObjectConfigBLL = new ObjectConfigBLL();
            txtEORINumber.Text = objObjectConfigBLL.GetValue("K:user.EORI", CurrentLoggedUser.ID);
            litUserEmail.Text = User.Identity.Name;
        }

        // --------------------------------------------------
        // Grab some settings
        // --------------------------------------------------
        try
        {
            numAppMaxOrders = KartSettingsManager.GetKartConfig("frontend.users.myaccount.orderhistory.max");
            numAppMaxBaskets = KartSettingsManager.GetKartConfig("frontend.users.myaccount.savedbaskets.max");
            strAppUploadsFolder = KartSettingsManager.GetKartConfig("general.uploadfolder");
        }
        catch (Exception ex)
        {
        }
        strShow = "";
        // If we're showing a full list, or config settings set to 0, then don't limit (-2)
        if (numAppMaxOrders == 0 | strShow == "orders")
            numAppMaxOrders = -2;
        if (numAppMaxBaskets == 0 | strShow == "baskets")
            numAppMaxBaskets = -2;
        if (numAppMaxBaskets == 0 | strShow == "wishlists")
            numAppMaxBaskets = -2;

        // --------------------------------------------------
        // Customer discount
        // --------------------------------------------------
        numCustomerDiscount = BasketBLL.GetCustomerDiscount(numCustomerID);

        litUserEmail.Text = User.Identity.Name;

        // --------------------------------------------------
        // Hide/show accordion panes
        // --------------------------------------------------
        if (LCase(KartSettingsManager.GetKartConfig("frontend.cataloguemode")) == "y")
        {
            acpDownloadableProducts.Visible = false;
            acpSavedBaskets.Visible = false;
            acpWishLists.Visible = false;
            pnlSaveWishLists.Visible = false;
            pnlSaveBasket.Visible = false;
        }
        else
        {
            // Show/hide order history
            if (Val(KartSettingsManager.GetKartConfig("frontend.users.myaccount.orderhistory.max")) == 0)
            {
                acpOrderHistory.Visible = false;
            }
            else
            {
                acpOrderHistory.Visible = true;
                BuildNavigatePage("order");
            }
            // Show/hide saved baskets
            if (Val(KartSettingsManager.GetKartConfig("frontend.users.myaccount.savedbaskets.max")) == 0)
            {
                acpSavedBaskets.Visible = false;
            }
            else
            {
                acpSavedBaskets.Visible = true;
                BuildNavigatePage("basket");
            }
            // Show/hide wishlists
            if (LCase(KartSettingsManager.GetKartConfig("frontend.users.wishlists.enabled")) == "n" | Val(KartSettingsManager.GetKartConfig("frontend.users.myaccount.wishlists.max")) == 0)
            {
                acpWishLists.Visible = false;
            }
            else
            {
                acpWishLists.Visible = true;
                BuildNavigatePage("wishlist");
            }
            // Show/hide affiliates
            if (LCase(KartSettingsManager.GetKartConfig("frontend.users.myaccount.affiliates.enabled")) == "n")
            {
                acpAffiliates.Visible = false;
            }
            else
            {
                acpAffiliates.Visible = true;
            }
            // Show/hide mailing list
            if (LCase(KartSettingsManager.GetKartConfig("frontend.users.mailinglist.enabled")) == "n")
            {
                acpMailingList.Visible = false;
            }
            else
            {
                acpMailingList.Visible = true;
            }
        }

        // --------------------------------------------------
        // Reset password link
        // --------------------------------------------------
        if (!User.Identity.IsAuthenticated)
        {
            if (!string.IsNullOrEmpty(Request.QueryString("ref")))
            {
                lblCurrentPassword.Text = GetGlobalResourceObject("Kartris", "FormLabel_EmailAddress");
                txtCurrentPassword.TextMode = TextBoxMode.SingleLine;
                string strRef = Request.QueryString("ref");
            }
            else
            {
                Response.Redirect(WebShopURL() + "Customer.aspx?action=password");
            }
        }
        else
        {

            // --------------------------------------------------
            // Setup addresses
            // --------------------------------------------------
            if (!IsPostBack)
            {
                ViewState("lstUsrAddresses") = KartrisClasses.Address.GetAll(CurrentLoggedUser.ID);
            }

            if (ViewState("lstUsrAddresses") is null)
                ViewState("lstUsrAddresses") = KartrisClasses.Address.GetAll(CurrentLoggedUser.ID);
            if (mvwAddresses.ActiveViewIndex == "0")
            {
                if (CurrentLoggedUser.DefaultBillingAddressID > 0 & !(((List<KartrisClasses.Address>)ViewState("lstUsrAddresses")).Find(p => p.ID == CurrentLoggedUser.DefaultBillingAddressID) == null))
                {
                    UC_DefaultBilling.Address = ((List<KartrisClasses.Address>)ViewState("lstUsrAddresses")).Find(p => p.ID == CurrentLoggedUser.DefaultBillingAddressID);
                    litContentTextNoAddress.Visible = false;
                }
                else
                {
                    UC_DefaultBilling.Visible = false;
                    litContentTextNoAddress.Visible = true;
                }
                if (CurrentLoggedUser.DefaultShippingAddressID > 0 & !(((List<KartrisClasses.Address>)ViewState("lstUsrAddresses")).Find(p => p.ID == CurrentLoggedUser.DefaultShippingAddressID) == null))
                {
                    UC_DefaultShipping.Address = ((List<KartrisClasses.Address>)ViewState("lstUsrAddresses")).Find(p => p.ID == CurrentLoggedUser.DefaultShippingAddressID);
                    litContentTextNoAddress2.Visible = false;
                }
                else
                {
                    UC_DefaultShipping.Visible = false;
                    litContentTextNoAddress2.Visible = true;
                }
            }

            else if (mvwAddresses.ActiveViewIndex == "1")
            {
                CreateBillingAddressesDetails();
            }
            else if (mvwAddresses.ActiveViewIndex == "2")
            {
                CreateShippingAddressesDetails();
            }


        }
        if (!Page.IsPostBack)
        {
            // --------------------------------------------------
            // Set active tab
            // --------------------------------------------------
            string strQSAction = Request.QueryString("action");
            switch (strQSAction ?? "")
            {
                case "home":
                    {
                        tabContainerCustomer.ActiveTabIndex = 0;
                        break;
                    }
                case "details":
                    {
                        tabContainerCustomer.ActiveTabIndex = 1;
                        break;
                    }
                case "addresses":
                    {
                        tabContainerCustomer.ActiveTabIndex = 2;
                        break;
                    }
                case "password":
                    {
                        tabContainerCustomer.ActiveTabIndex = 3;
                        break;
                    }
                case "savebasket":
                    {
                        tabContainerCustomer.ActiveTabIndex = 0;
                        phdSaveBasket.Visible = true;
                        phdHome.Visible = false;
                        break;
                    }
                case "wishlists":
                    {
                        tabContainerCustomer.ActiveTabIndex = 0;
                        phdSaveWishLists.Visible = true;
                        phdHome.Visible = false;
                        break;
                    }

                default:
                    {
                        tabContainerCustomer.ActiveTabIndex = 0;
                        break;
                    }
            }
            UC_Updated.reset();
        }

        // acpAffiliates.Visible = False
        // acpDownloadableProducts.Visible = False
        // acpMailingList.Visible = False
        // acpOrderHistory.Visible = False
        // acpSavedBaskets.Visible = False
        // acpWishLists.Visible = False
    }

    // We create two download links on the page (.aspx),
    // one is a linkbutton to trigger local file download,
    // the other is a hyperlink to a fully-qualified URL.
    // We use the code below to hide the one that isn't
    // needed when the item is databound.
    protected void rptDownloadableProducts_ItemDataBound(object sender, Global.System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        string strDownloadType;
        if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
        {
            strDownloadType = e.Item.DataItem("V_DownloadType");
            if (strDownloadType == "l")
            {
                // Full qualified path
                // Hide link button
                ((LinkButton)e.Item.FindControl("lnkDownload")).Visible = false;
            }
            else
            {
                // local file, type "u"
                // Hide hyperlink
                ((HyperLink)e.Item.FindControl("hlnkDownload")).Visible = false;
            }
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {

            phdSaveBasket.Visible = false;
            phdHome.Visible = false;
            phdSaveWishLists.Visible = false;

            dtbDownloadableProducts = BasketBLL.GetDownloadableProducts(numCustomerID);
            if (dtbDownloadableProducts.Rows.Count > 0)
            {
                phdDownloadableProducts.Visible = true;
                rptDownloadableProducts.DataSource = dtbDownloadableProducts;
                rptDownloadableProducts.DataBind();
            }
            else
            {
                phdDownloadableProducts.Visible = false;
                acpDownloadableProducts.Visible = false;
            }
            switch (Strings.LCase(strAction) ?? "")
            {
                case "savebasket":
                    {
                        this.Title = GetGlobalResourceObject("Basket", "PageTitle_SaveRecoverBasketContents") + GetGlobalResourceObject("Kartris", "PageTitle_Separator") + Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));
                        phdSaveBasket.Visible = true;
                        break;
                    }

                case "home":
                    {
                        this.Title = GetGlobalResourceObject("Kartris", "PageTitle_MyAccount") + GetGlobalResourceObject("Kartris", "PageTitle_Separator") + Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));
                        phdHome.Visible = true;

                        System.Data.DataTable tblCustomerData;

                        // '// initialize orders navigation page
                        Order_PageSize = KartSettingsManager.GetKartConfig("frontend.users.myaccount.orderhistory.max");
                        ViewState("Order_PageTotalSize") = BasketBLL.GetCustomerOrdersTotal(numCustomerID);
                        ViewState("Order_PageIndex") = 1;
                        lnkBtnOrderPrev.Enabled = false;
                        lnkBtnOrderNext.Enabled = true;
                        if (Order_PageSize >= ViewState("Order_PageTotalSize"))
                        {
                            lnkBtnOrderPrev.Visible = false;
                            lnkBtnOrderNext.Visible = false;
                        }

                        BuildNavigatePage("order");

                        // '// initialize saved baskets navigation page
                        SavedBasket_PageSize = KartSettingsManager.GetKartConfig("frontend.users.myaccount.savedbaskets.max");
                        ViewState("SavedBasket_PageTotalSize") = BasketBLL.GetSavedBasketTotal(numCustomerID);
                        ViewState("SavedBasket_PageIndex") = 1;
                        lnkBtnBasketPrev.Enabled = false;
                        lnkBtnBasketNext.Enabled = true;
                        if (SavedBasket_PageSize >= ViewState("SavedBasket_PageTotalSize"))
                        {
                            lnkBtnBasketPrev.Visible = false;
                            lnkBtnBasketNext.Visible = false;
                        }

                        BuildNavigatePage("basket");

                        // '// initialize wishlists navigation page
                        WishList_PageSize = KartSettingsManager.GetKartConfig("frontend.users.myaccount.wishlists.max");
                        ViewState("WishList_PageTotalSize") = BasketBLL.GetWishListTotal(numCustomerID);
                        ViewState("WishList_PageIndex") = 1;
                        lnkBtnWishlistPrev.Enabled = false;
                        lnkBtnWishlistNext.Enabled = true;
                        if (WishList_PageSize >= ViewState("WishList_PageTotalSize"))
                        {
                            lnkBtnWishlistPrev.Visible = false;
                            lnkBtnWishlistNext.Visible = false;
                        }

                        BuildNavigatePage("wishlist");

                        var oItems = new List<Kartris.BasketItem>();
                        objBasket.LoadBasketItems();
                        oItems = objBasket.BasketItems;
                        blnNoBasketItem = oItems.Count == 0;


                        tblCustomerData = BasketBLL.GetCustomerData(numCustomerID);
                        if (tblCustomerData.Rows.Count > 0)
                        {
                            // '// affiliate
                            AFF_IsAffiliate = FixNullFromDB(tblCustomerData.Rows[0]["U_IsAffiliate"]);
                            AFF_AffiliateCommission = FixNullFromDB(tblCustomerData.Rows[0]["U_AffiliateCommission"]);

                            // '// mailing list
                            ML_ConfirmationDateTime = FixNullFromDB(tblCustomerData.Rows[0]["U_ML_ConfirmationDateTime"]);
                            ML_SignupDateTime = FixNullFromDB(tblCustomerData.Rows[0]["U_ML_SignupDateTime"]);
                            ML_SendMail = FixNullFromDB(tblCustomerData.Rows[0]["U_ML_SendMail"]);
                            ML_Format = FixNullFromDB(tblCustomerData.Rows[0]["U_ML_Format"]);
                        }

                        ddlMailingList.Items.Clear();
                        ddlMailingList.Items.Add(GetGlobalResourceObject("Kartris", "ContentText_SendMailsOff").ToString);
                        ddlMailingList.Items.Add(GetGlobalResourceObject("Checkout", "ContentText_SendMailsPlain").ToString);
                        ddlMailingList.Items.Add(GetGlobalResourceObject("Checkout", "ContentText_SendMailsHTML").ToString);
                        ddlMailingList.Items(0).Value = "n";
                        ddlMailingList.Items(1).Value = "t";
                        ddlMailingList.Items(2).Value = "h";
                        if (ML_SendMail)
                            ddlMailingList.SelectedIndex = 0;
                        if (ML_SendMail && ML_Format != "h")
                            ddlMailingList.SelectedIndex = 1;
                        if (ML_SendMail && ML_Format == "h")
                            ddlMailingList.SelectedIndex = 2;
                        break;
                    }


                case "wishlists":
                    {
                        this.Title = GetGlobalResourceObject("Kartris", "PageTitle_WishListLogin") + GetGlobalResourceObject("Kartris", "PageTitle_Separator") + Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));
                        phdSaveWishLists.Visible = true;

                        var numWishlistsID = default(long);

                        try
                        {
                            numWishlistsID = Conversions.ToLong(Interaction.IIf(Request.QueryString("WL_ID") == "", 0, Request.QueryString("WL_ID")));
                        }
                        catch (Exception ex)
                        {
                            Current.Response.Redirect("~/Customer.aspx");
                        }

                        if (numWishlistsID != 0L)
                        {
                            tblWishLists = BasketBLL.GetCustomerWishList(numCustomerID, numWishlistsID);

                            if (tblWishLists.Rows.Count > 0)
                            {
                                txtWL_Name.Text = FixNullFromDB(tblWishLists.Rows[0]["WL_Name"]) + "";
                                txtWL_PublicPassword.Text = FixNullFromDB(tblWishLists.Rows[0]["WL_PublicPassword"]) + "";
                                txtWL_Message.Text = FixNullFromDB(tblWishLists.Rows[0]["WL_Message"]) + "";
                            }
                            else
                            {
                                Current.Response.Redirect("~/Customer.aspx");
                            }

                            tblWishLists = null;
                        }

                        else
                        {
                            var tblCustomerData = new System.Data.DataTable();
                            tblCustomerData = BasketBLL.GetCustomerData(numCustomerID);
                        }

                        break;
                    }

                default:
                    {
                        break;
                    }

            }
        }


        else // 'postback
        {

            switch (Strings.LCase(strAction) ?? "")
            {
                case "savebasket":
                    {
                        this.Title = GetGlobalResourceObject("Basket", "PageTitle_SaveRecoverBasketContents") + GetGlobalResourceObject("Kartris", "PageTitle_Separator") + Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));
                        break;
                    }

                case "home":
                    {
                        this.Title = GetGlobalResourceObject("Kartris", "PageTitle_MyAccount") + GetGlobalResourceObject("Kartris", "PageTitle_Separator") + Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));
                        break;
                    }

                case "wishlists":
                    {
                        this.Title = GetGlobalResourceObject("Kartris", "PageTitle_WishListLogin") + GetGlobalResourceObject("Kartris", "PageTitle_Separator") + Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));
                        break;
                    }

                default:
                    {
                        break;
                    }

            }


        }

    }

    public void RemoveSavedBasket_Click(object Sender, CommandEventArgs E)
    {
        long numBasketID;
        numBasketID = E.CommandArgument;
        BasketBLL.DeleteSavedBasket(numBasketID);
        BuildNavigatePage("basket");
        UC_Updated.ShowAnimatedText();
    }

    public void LoadSavedBasket_Click(object Sender, CommandEventArgs E)
    {
        long numSavedBasketID, numBasketID;
        numSavedBasketID = E.CommandArgument;
        numBasketID = SESSION_ID;
        BasketBLL.LoadSavedBasket(numSavedBasketID, numBasketID);
        blnNoBasketItem = objBasket.BasketItems.Count > 0;
        RefreshMiniBasket();
    }

    public void SaveBasket_Click(object Sender, CommandEventArgs E)
    {
        if (this.IsValid)
        {
            BasketBLL.SaveBasket(numCustomerID, Strings.Trim(txtBasketName.Text), SESSION_ID);
            UC_Updated.ShowAnimatedText();
            accMyAccount.SelectedIndex = 2;
            phdSaveBasket.Visible = false;
            phdHome.Visible = true;
            BuildNavigatePage("basket");
            updMain.Update();
        }
    }

    public void SaveWishLists_Click(object Sender, CommandEventArgs E)
    {
        string strName, strPublicPassword, strMessage;
        var strErrorMsg = new StringBuilder("");

        strName = txtWL_Name.Text;
        strPublicPassword = txtWL_PublicPassword.Text;
        strMessage = txtWL_Message.Text;

        if (this.IsValid)
        {
            var tblWishLists = new DataTable();

            long numWishlistsID = Conversions.ToLong(Interaction.IIf(Request.QueryString("WL_ID") == "", 0, Request.QueryString("WL_ID")));

            if (numWishlistsID == 0L)
            {

                string strEmail = "";
                strEmail = CurrentLoggedUser.Email;

                tblWishLists = BasketBLL.GetWishListLogin(strEmail, strPublicPassword);
                if (tblWishLists.Rows.Count > 0) // '// password already exist for this owner
                {
                    {
                        var withBlock = UC_PopUpInfo;
                        string strError = GetGlobalResourceObject("Kartris", "ContentText_WishListPublicPasswordExists");
                        strError = Strings.Replace(strError, "<label>", GetGlobalResourceObject("Kartris", "ContentText_WishListPublicPassword"));
                        withBlock.SetTitle = GetGlobalResourceObject("Kartris", "PageTitle_WishListLogin");
                        withBlock.SetTextMessage = strError;
                        withBlock.ShowPopup();
                    }
                }
                else // '// new wishlist (create it)
                {
                    BasketBLL.SaveWishLists(numWishlistsID, SESSION_ID, numCustomerID, strName, strPublicPassword, strMessage);
                    UC_Updated.ShowAnimatedText();
                    accMyAccount.SelectedIndex = 3;
                    phdSaveWishLists.Visible = false;
                    phdHome.Visible = true;
                    BuildNavigatePage("wishlist");
                    updMain.Update();
                }
            }

            else // '// existing wishlist (update it)
            {
                BasketBLL.SaveWishLists(numWishlistsID, SESSION_ID, numCustomerID, strName, strPublicPassword, strMessage);
                UC_Updated.ShowAnimatedText();
                accMyAccount.SelectedIndex = 3;
                phdSaveWishLists.Visible = false;
                phdHome.Visible = true;
                BuildNavigatePage("wishlist");
                updMain.Update();
            }

        }

    }

    protected void lnkDownload_Click(object Sender, CommandEventArgs E)
    {
        DownloadFile(E.CommandArgument);
    }

    public void RemoveWishLists_Click(object Sender, CommandEventArgs E)
    {
        long numWishListsID;

        numWishListsID = E.CommandArgument;
        BasketBLL.DeleteWishLists(numWishListsID);
        BuildNavigatePage("wishlist");
        UC_Updated.ShowAnimatedText();
    }

    public void EditWishLists_Click(object Sender, CommandEventArgs E)
    {
        long numWishListsID;
        numWishListsID = E.CommandArgument;
        Response.Redirect("~/Customer.aspx?action=wishlists&WL_ID=" + numWishListsID);
    }

    public void LoadWishLists_Click(object Sender, CommandEventArgs E)
    {
        long numWishListsID, numBasketID;
        numWishListsID = E.CommandArgument;
        numBasketID = SESSION_ID;
        BasketBLL.LoadWishlists(numWishListsID, numBasketID);
        blnNoBasketItem = objBasket.BasketItems.Count > 0;
        RefreshMiniBasket();
        UC_Updated.ShowAnimatedText();
    }

    public void RefreshMiniBasket()
    {
        MasterPage objMaster;
        object objBasket;
        objMaster = Page.Master;
        objBasket = objMaster.FindControl("UC_MiniBasket");
        objBasket.LoadMiniBasket();
    }

    public double GetCustomerDiscount()
    {
        int numCustomerID;
        numCustomerID = (int)Math.Round(Conversion.Val(SESSION_ID));
        return BasketBLL.GetCustomerDiscount(numCustomerID);
    }

    protected void PageNavigate_Click(object sender, CommandEventArgs E)
    {

        switch (LCase(E.CommandName))
        {
            case "order":
                {
                    if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(LCase(sender.id), "lnkbtnorderprev", false)))
                    {
                        ViewState("Order_PageIndex") = ViewState("Order_PageIndex") - 1;
                    }
                    else if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(LCase(sender.id), "lnkbtnordernext", false)))
                    {
                        ViewState("Order_PageIndex") = ViewState("Order_PageIndex") + 1;
                    }
                    BuildNavigatePage("order");
                    lnkBtnOrderPrev.Enabled = Interaction.IIf(ViewState("Order_PageIndex") <= 1, false, true);
                    lnkBtnOrderNext.Enabled = Interaction.IIf(ViewState("Order_PageIndex") >= ViewState("Order_PageTotalSize") / Order_PageSize, false, true);
                    break;
                }

            case "basket":
                {
                    if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(LCase(sender.id), "lnkbtnbasketprev", false)))
                    {
                        ViewState("SavedBasket_PageIndex") = ViewState("SavedBasket_PageIndex") - 1;
                    }
                    else if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(LCase(sender.id), "lnkbtnbasketnext", false)))
                    {
                        ViewState("SavedBasket_PageIndex") = ViewState("SavedBasket_PageIndex") + 1;
                    }
                    BuildNavigatePage("basket");
                    lnkBtnBasketPrev.Enabled = Interaction.IIf(ViewState("SavedBasket_PageIndex") <= 1, false, true);
                    lnkBtnBasketNext.Enabled = Interaction.IIf(ViewState("SavedBasket_PageIndex") >= ViewState("SavedBasket_PageTotalSize") / SavedBasket_PageSize, false, true);
                    break;
                }

            case "wishlist":
                {
                    if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(LCase(sender.id), "lnkbtnwishlistprev", false)))
                    {
                        ViewState("WishList_PageIndex") = ViewState("WishList_PageIndex") - 1;
                    }
                    else if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(LCase(sender.id), "lnkbtnwishlistnext", false)))
                    {
                        ViewState("WishList_PageIndex") = ViewState("WishList_PageIndex") + 1;
                    }
                    BuildNavigatePage("wishlist");
                    lnkBtnWishlistPrev.Enabled = Interaction.IIf(ViewState("WishList_PageIndex") <= 1, false, true);
                    lnkBtnWishlistNext.Enabled = Interaction.IIf(ViewState("WishList_PageIndex") >= ViewState("WishList_PageTotalSize") / WishList_PageSize, false, true);
                    break;
                }

        }

    }

    private void BuildNavigatePage(string strPage)
    {

        switch (Strings.LCase(strPage) ?? "")
        {
            case "order":
                {
                    tblOrder = BasketBLL.GetCustomerOrders(numCustomerID, (ViewState("Order_PageIndex") - 1) * Order_PageSize, Order_PageSize);
                    rptOrder.DataSource = tblOrder;
                    rptOrder.DataBind();
                    updMain.Update();
                    break;
                }

            case "basket":
                {
                    tblSavedBasket = BasketBLL.GetSavedBasket(numCustomerID, (ViewState("SavedBasket_PageIndex") - 1) * SavedBasket_PageSize, SavedBasket_PageSize);
                    rptSavedBasket.DataSource = tblSavedBasket;
                    rptSavedBasket.DataBind();
                    updMain.Update();
                    break;
                }

            case "wishlist":
                {
                    tblWishLists = BasketBLL.GetWishLists(numCustomerID, (ViewState("WishList_PageIndex") - 1) * WishList_PageSize, WishList_PageSize);
                    rptWishLists.DataSource = tblWishLists;
                    rptWishLists.DataBind();
                    updMain.Update();
                    break;
                }

        }

    }

    public void Affiliate_Click(object Sender, CommandEventArgs E)
    {
        System.Data.DataTable tblAffiliates;

        {
            var withBlock = UC_PopUpInfo;
            withBlock.SetTitle = GetGlobalResourceObject("Kartris", "PageTitle_Affiliates");
            withBlock.SetTextMessage = GetGlobalResourceObject("Kartris", "ContentText_AffiliateApplicationDetail");
            withBlock.ShowPopup();
        }

        AffiliateBLL.UpdateCustomerAffiliateStatus(numCustomerID);

        tblAffiliates = BasketBLL.GetCustomerData(numCustomerID);
        if (tblAffiliates.Rows.Count > 0)
        {
            AFF_IsAffiliate = Conversions.ToBoolean(tblAffiliates.Rows[0]["U_IsAffiliate"]);
            AFF_AffiliateCommission = Conversions.ToDouble(tblAffiliates.Rows[0]["U_AffiliateCommission"]);
        }

        updMain.Update();

    }

    public void MailingList_Click(object Sender, CommandEventArgs E)
    {
        string strCommand;
        var tblCustomerData = new System.Data.DataTable();

        strCommand = E.CommandName;

        if ((Strings.LCase(strCommand) ?? "") == (Strings.LCase("MailVerified") ?? ""))
        {

            BasketBLL.UpdateCustomerMailFormat(numCustomerID, LCase(ddlMailingList.SelectedValue));

            {
                var withBlock = UC_PopUpMessage;
                withBlock.SetTitle = GetGlobalResourceObject("Kartris", "PageTitle_MailingList");
                withBlock.SetTextMessage = GetGlobalResourceObject("Kartris", "ContentText_PreferencesChanged");
                withBlock.ShowPopup();
            }
        }

        else // '// mail not verified or mail not signed up
        {

            tblCustomerData = BasketBLL.GetCustomerData(numCustomerID);
            string strEmail = "";
            string strPassword = "";

            if (tblCustomerData.Rows.Count > 0)
            {
                strEmail = FixNullFromDB(tblCustomerData.Rows[0]["U_EmailAddress"]) + "";
            }

            // Update user in our db
            BasketBLL.UpdateCustomerMailingList(strEmail, strPassword);

            // If mailchimp is active, we want to add the user to the mailing list
            if (KartSettingsManager.GetKartConfig("general.mailchimp.enabled") == "y")
            {
                // Add user direct to MailChimp
                BasketBLL.AddListSubscriber(strEmail);
            }
            else
            {
                // Use the built in mailing list
                var sbdBodyText = new StringBuilder();
                string strBodyText;
                string strMailingListSignUpLink = WebShopURL() + "Default.aspx?id=" + numCustomerID + "&r=" + strPassword;

                sbdBodyText.Append(GetGlobalResourceObject("Kartris", "EmailText_NewsletterSignup") + Constants.vbCrLf + Constants.vbCrLf);
                sbdBodyText.Append(strMailingListSignUpLink + Constants.vbCrLf + Constants.vbCrLf);
                sbdBodyText.Append(GetGlobalResourceObject("Kartris", "EmailText_NewsletterAuthorizeFooter"));

                strBodyText = sbdBodyText.ToString();
                strBodyText = Strings.Replace(strBodyText, "[IPADDRESS]", Request.UserHostAddress());
                strBodyText = Strings.Replace(strBodyText, "[WEBSHOPNAME]", GetGlobalResourceObject("Kartris", "Config_Webshopname"));
                strBodyText = Strings.Replace(strBodyText, "[WEBSHOPURL]", WebShopURL);
                strBodyText = strBodyText + GetGlobalResourceObject("Kartris", "ContentText_NewsletterSignup");

                string strFrom = LanguagesBLL.GetEmailFrom(GetLanguageIDfromSession);

                bool blnHTMLEmail = KartSettingsManager.GetKartConfig("general.email.enableHTML") == "y";
                if (blnHTMLEmail)
                {
                    string strHTMLEmailText = RetrieveHTMLEmailTemplate("MailingListSignUp");
                    // build up the HTML email if template is found
                    if (!string.IsNullOrEmpty(strHTMLEmailText))
                    {
                        strHTMLEmailText = strHTMLEmailText.Replace("[mailinglistconfirmationlink]", strMailingListSignUpLink);
                        strHTMLEmailText = strHTMLEmailText.Replace("[websitename]", GetGlobalResourceObject("Kartris", "Config_Webshopname"));
                        strHTMLEmailText = strHTMLEmailText.Replace("[customerip]", Request.UserHostAddress());
                        strBodyText = strHTMLEmailText;
                    }
                    else
                    {
                        blnHTMLEmail = false;
                    }
                }

                // GDPR Mod - v2.9013 
                // We want to be able to have a log of all opt-in
                // requests sent, so we can prove the user was sent
                // the GDPR notice, and also prove what text they
                // received. We do this by BCCing the confirmation
                // opt-in mail to an email address. A free account
                // like gmail would be good for this.
                var objBCCsCollection = new System.Net.Mail.MailAddressCollection();
                string strGDPROptinArchiveEmail = KartSettingsManager.GetKartConfig("general.gdpr.mailinglistbcc");
                if (strGDPROptinArchiveEmail.Length > 0)
                {
                    objBCCsCollection.Add(strGDPROptinArchiveEmail);
                }
                SendEmail(strFrom, strEmail, GetGlobalResourceObject("Kartris", "PageTitle_MailingList"), strBodyText, default, default, default, default, blnHTMLEmail, default, objBCCsCollection);
            }

        }

        tblCustomerData = BasketBLL.GetCustomerData(numCustomerID);
        if (Conversions.ToBoolean(tblCustomerData.Rows.Count))
        {
            ML_ConfirmationDateTime = FixNullFromDB(tblCustomerData.Rows[0]["U_ML_ConfirmationDateTime"]);
            ML_SignupDateTime = FixNullFromDB(tblCustomerData.Rows[0]["U_ML_SignupDateTime"]);
            ML_SendMail = FixNullFromDB(tblCustomerData.Rows[0]["U_ML_SendMail"]);
            ML_Format = FixNullFromDB(tblCustomerData.Rows[0]["U_ML_Format"]);
        }

        updMain.Update();
    }

    protected void rptOrder_ItemDataBound(object sender, Global.System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        double numTotal;
        if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
        {
            ((LinkButton)e.Item.FindControl("lnkBtnOrderView")).PostBackUrl = "~/CustomerViewOrder.aspx?O_ID=" + e.Item.DataItem("O_ID");
            ((LinkButton)e.Item.FindControl("lnkBtnOrderInvoice")).PostBackUrl = "~/CustomerInvoice.aspx?O_ID=" + e.Item.DataItem("O_ID");
            numTotal = e.Item.DataItem("O_TotalPrice");
            ((Literal)e.Item.FindControl("litOrdersTotal")).Text = e.Item.DataItem("CUR_Symbol") + Strings.FormatNumber(Math.Round(numTotal, e.Item.DataItem("CUR_RoundNumbers")), 2);
        }

    }

    protected void rptSavedBasket_ItemDataBound(object sender, Global.System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {

        if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
        {
            ((HtmlGenericControl)e.Item.FindControl("divBasketName")).InnerText = e.Item.DataItem("SBSKT_Name");
        }

    }

    protected void rptWishLists_ItemDataBound(object sender, Global.System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {

        if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
        {
            ((HtmlGenericControl)e.Item.FindControl("spnWishListName")).InnerText = e.Item.DataItem("WL_Name");
        }

    }

    /// <summary>
    /// Submit new password
    /// </summary>
    /// <remarks></remarks>
    protected void btnPasswordSubmit_Click(object sender, EventArgs e)
    {
        var objUsersBLL = new UsersBLL();
        if (string.IsNullOrEmpty(Request.QueryString("ref")))
        {
            string strUserPassword = txtCurrentPassword.Text;
            string strNewPassword = txtNewPassword.Text;

            // Only update if validators ok
            if (this.IsValid)
            {
                if (Membership.ValidateUser(CurrentLoggedUser.Email, strUserPassword))
                {
                    if (objUsersBLL.ChangePassword(CurrentLoggedUser.ID, strUserPassword, strNewPassword) > 0)
                        UC_Updated.ShowAnimatedText();
                }
                else
                {
                    string strErrorMessage = Strings.Replace(GetGlobalResourceObject("Kartris", "ContentText_CustomerCodeIncorrect"), "[label]", GetGlobalResourceObject("Login", "FormLabel_ExistingCustomerCode"));
                    litWrongPassword.Text = "<span class=\"error\">" + strErrorMessage + "</span>";
                }
            }
        }

        else
        {
            string strRef = Request.QueryString("ref");
            string strEmailAddress = txtCurrentPassword.Text;

            DataTable dtbUserDetails = objUsersBLL.GetDetails(strEmailAddress);
            if (dtbUserDetails.Rows.Count > 0)
            {
                int intUserID = dtbUserDetails[0]("U_ID");
                string strTempPassword = FixNullFromDB(dtbUserDetails[0]("U_TempPassword"));
                DateTime datExpiry = Conversions.ToDate(Interaction.IIf(Information.IsDate(FixNullFromDB(dtbUserDetails[0]("U_TempPasswordExpiry"))), dtbUserDetails[0]("U_TempPasswordExpiry"), CkartrisDisplayFunctions.NowOffset.AddMinutes(-1)));
                if (string.IsNullOrEmpty(strTempPassword))
                    datExpiry = CkartrisDisplayFunctions.NowOffset.AddMinutes(-1);

                if (datExpiry > CkartrisDisplayFunctions.NowOffset)
                {
                    if (objUsersBLL.EncryptSHA256Managed(strTempPassword, objUsersBLL.GetSaltByEmail(strEmailAddress)) == strRef)
                    {
                        int intSuccess = objUsersBLL.ChangePasswordViaRecovery(intUserID, txtConfirmPassword.Text);
                        if (intSuccess == intUserID)
                        {
                            UC_Updated.ShowAnimatedText();
                            Response.Redirect(WebShopURL() + "CustomerAccount.aspx?m=u");
                        }
                        else
                        {
                            litWrongPassword.Text = "<span class=\"error\">" + GetGlobalResourceObject("Kartris", "ContentText_ErrorText") + "</span>";
                        }
                    }
                    else
                    {
                        litWrongPassword.Text = "<span class=\"error\">" + GetGlobalResourceObject("Kartris", "ContentText_LinkExpiredOrIncorrect") + "</span>";
                    }
                }

                else
                {
                    litWrongPassword.Text = "<span class=\"error\">" + GetGlobalResourceObject("Kartris", "ContentText_LinkExpiredOrIncorrect") + "</span>";
                }
            }

            else
            {
                litWrongPassword.Text = "<span class=\"error\">" + GetGlobalResourceObject("Kartris", "ContentText_NotFoundInDB") + "</span>";
            }
        }

    }

    /// <summary>
    /// Submit new details (general customer details tab)
    /// </summary>
    /// <remarks></remarks>
    protected void btnDetailsSubmit_Click(object sender, EventArgs e)
    {
        Page.Validate("NameAndVat");
        if (Page.IsValid)
        {
            var objUsersBLL = new UsersBLL();
            if (objUsersBLL.UpdateNameandEUVAT(CurrentLoggedUser.ID, txtCustomerName.Text, txtEUVATNumber.Text) == CurrentLoggedUser.ID)
                UC_Updated.ShowAnimatedText();

            // EORI number
            var objObjectConfigBLL = new ObjectConfigBLL();
            bool blnAddedEORI = objObjectConfigBLL._SetConfigValue(11, CurrentLoggedUser.ID, txtEORINumber.Text, "");
        }
    }

    /// <summary>
    /// Addresses - load complete
    /// </summary>
    /// <remarks></remarks>
    private void Page_LoadComplete(object sender, EventArgs e)
    {
        if (mvwAddresses.ActiveViewIndex > 0)
            btnBack.Visible = true;
        else
            btnBack.Visible = false;
    }

    /// <summary>
    /// Addresses - edit billing click
    /// </summary>
    /// <remarks></remarks>
    protected void lnkEditBilling_Click(object sender, EventArgs e)
    {

        mvwAddresses.ActiveViewIndex = "1";
        CreateBillingAddressesDetails();
        // UC_NewEditAddress.DisplayType = "Billing"
        UC_NewEditAddress.ValidationGroup = "Billing";
        btnSaveNewAddress.OnClientClick = "Page_ClientValidate('Billing');";
        UC_Updated.reset();
    }


    /// <summary>
    /// Addresses - edit shipping click
    /// </summary>
    /// <remarks></remarks>
    protected void lnkEditShipping_Click(object sender, EventArgs e)
    {

        mvwAddresses.ActiveViewIndex = "2";
        CreateShippingAddressesDetails();
        // UC_NewEditAddress.DisplayType = "Shipping"
        UC_NewEditAddress.ValidationGroup = "Shipping";
        btnSaveNewAddress.OnClientClick = "Page_ClientValidate('Shipping');";
        UC_Updated.reset();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        mvwAddresses.ActiveViewIndex = "0";
        if (CurrentLoggedUser.DefaultBillingAddressID > 0 & !(((List<KartrisClasses.Address>)ViewState("lstUsrAddresses")).Find(p => p.ID == CurrentLoggedUser.DefaultBillingAddressID) == null))
        {
            UC_DefaultBilling.Address = ((List<KartrisClasses.Address>)ViewState("lstUsrAddresses")).Find(p => p.ID == CurrentLoggedUser.DefaultBillingAddressID);
            litContentTextNoAddress.Visible = false;
        }
        else
        {
            UC_DefaultBilling.Visible = false;
            litContentTextNoAddress.Visible = true;
        }
        if (CurrentLoggedUser.DefaultShippingAddressID > 0 & !(((List<KartrisClasses.Address>)ViewState("lstUsrAddresses")).Find(p => p.ID == CurrentLoggedUser.DefaultShippingAddressID) == null))
        {
            UC_DefaultShipping.Address = ((List<KartrisClasses.Address>)ViewState("lstUsrAddresses")).Find(p => p.ID == CurrentLoggedUser.DefaultShippingAddressID);
            litContentTextNoAddress2.Visible = false;
        }
        else
        {
            UC_DefaultShipping.Visible = false;
            litContentTextNoAddress2.Visible = true;
        }
        UC_Updated.reset();
    }

    /// <summary>
    /// Addresses - save new address
    /// </summary>
    /// <remarks></remarks>
    protected void btnSaveNewAddress_Click(object sender, EventArgs e)
    {
        if (UC_NewEditAddress.AddressType == "s")
        {
            Page.Validate("Shipping");
        }
        if (UC_NewEditAddress.AddressType == "b")
        {
            Page.Validate("Billing");
        }
        if (UC_NewEditAddress.AddressType == "u")
        {
            Page.Validate("Shipping");
        }

        if (Page.IsValid)
        {

            if (chkAlso.Checked == false)
            {
                if (UC_NewEditAddress.DisplayType == "Billing")
                    UC_NewEditAddress.AddressType = "b";
                else
                    UC_NewEditAddress.AddressType = "s";
            }
            else
            {
                UC_NewEditAddress.AddressType = "u";
            }

            int intGeneratedAddressID = KartrisClasses.Address.AddUpdate(UC_NewEditAddress.EnteredAddress, CurrentLoggedUser.ID, chkMakeDefault.Checked, UC_NewEditAddress.EnteredAddress.ID);
            if (intGeneratedAddressID > 0)
            {

                KartrisClasses.Address objAddress = UC_NewEditAddress.EnteredAddress;
                objAddress.ID = intGeneratedAddressID;

                if (UC_NewEditAddress.EnteredAddress.ID > 0)
                {
                    var AddresstobeDeleted = ((List<KartrisClasses.Address>)ViewState("lstUsrAddresses")).Find(p => p.ID == intGeneratedAddressID);
                    ((List<KartrisClasses.Address>)ViewState("lstUsrAddresses")).Remove(AddresstobeDeleted);

                } ((List<KartrisClasses.Address>)ViewState("lstUsrAddresses")).Add(objAddress);
                if (UC_NewEditAddress.DisplayType == "Billing")
                {
                    pnlBilling.Controls.Clear();
                    CreateBillingAddressesDetails();
                }
                else
                {
                    pnlShipping.Controls.Clear();
                    CreateShippingAddressesDetails();
                }

                if (chkMakeDefault.Checked)
                {
                    if (UC_NewEditAddress.DisplayType == "Billing")
                        CurrentLoggedUser.DefaultBillingAddressID = intGeneratedAddressID;
                    else
                        CurrentLoggedUser.DefaultShippingAddressID = intGeneratedAddressID;
                    if (chkAlso.Checked)
                    {
                        if (UC_NewEditAddress.DisplayType == "Billing")
                            CurrentLoggedUser.DefaultShippingAddressID = intGeneratedAddressID;
                    }
                }
                ResetAddressInput();
                UC_Updated.ShowAnimatedText();
            }
        }
        else
        {
            popExtender.Show();
        }
    }

    /// <summary>
    /// Addresses - click "add address"
    /// </summary>
    /// <remarks></remarks>
    protected void btnAddAddress_Click(object sender, EventArgs e)
    {
        UC_Updated.reset();
        ResetAddressInput();
        if (UC_NewEditAddress.DisplayType == "Billing")
        {
            UC_NewEditAddress.ValidationGroup = "Billing";
            chkAlso.Text = GetGlobalResourceObject("Address", "ContentText_CanAlsoBeUsedAsShippingAddress");
        }
        else
        {
            UC_NewEditAddress.ValidationGroup = "Shipping";
            chkAlso.Text = GetGlobalResourceObject("Address", "ContentText_CanAlsoBeUsedAsBillingAddress");
        }

        litAddressTitle.Text = GetGlobalResourceObject("Address", "ContentText_AddEditAddress");
        popExtender.Show();
    }

    /// <summary>
    /// Addresses - click "edit address"
    /// </summary>
    /// <remarks></remarks>
    protected void btnEditAddress_Click(object sender, EventArgs e)
    {

        UC_Updated.reset();
        litAddressTitle.Text = GetGlobalResourceObject("Kartris", "ContentText_Edit");
        LinkButton lnkEdit = (LinkButton)sender;

        UC_NewEditAddress.InitialAddressToDisplay = ((UserControls_General_AddressDetails)lnkEdit.Parent.Parent).Address;

        if (UC_NewEditAddress.DisplayType == "Billing")
        {
            chkAlso.Text = GetGlobalResourceObject("Address", "ContentText_CanAlsoBeUsedAsShippingAddress");
            if (CurrentLoggedUser.DefaultBillingAddressID == ((UserControls_General_AddressDetails)lnkEdit.Parent.Parent).Address.ID)
                chkMakeDefault.Checked = true;
            if (CurrentLoggedUser.DefaultBillingAddressID == UC_NewEditAddress.EnteredAddress.ID)
                chkMakeDefault.Checked = true;
            else
                chkMakeDefault.Checked = false;
            UC_NewEditAddress.ValidationGroup = "Billing";
        }
        else
        {
            chkAlso.Text = GetGlobalResourceObject("Address", "ContentText_CanAlsoBeUsedAsBillingAddress");
            if (CurrentLoggedUser.DefaultShippingAddressID == ((UserControls_General_AddressDetails)lnkEdit.Parent.Parent).Address.ID)
                chkMakeDefault.Checked = true;
            if (CurrentLoggedUser.DefaultShippingAddressID == UC_NewEditAddress.EnteredAddress.ID)
                chkMakeDefault.Checked = true;
            else
                chkMakeDefault.Checked = false;
            UC_NewEditAddress.ValidationGroup = "Shipping";
        }

        if (UC_NewEditAddress.EnteredAddress.Type == "u")
            chkAlso.Checked = true;
        else
            chkAlso.Checked = false;
        UC_Updated.reset();
        popExtender.Show();
    }

    /// <summary>
    /// Addresses - delete address click
    /// </summary>
    /// <remarks></remarks>
    protected void btnDeleteAddress_Click(object sender, EventArgs e)
    {
        UserControls_General_AddressDetails UC_Delete = (UserControls_General_AddressDetails)((LinkButton)sender).Parent.Parent;
        int intAddressID = UC_Delete.Address.ID;
        KartrisClasses.Address objAddress = UC_Delete.Address;

        if (KartrisClasses.Address.Delete(intAddressID, CurrentLoggedUser.ID) > 0)
        {
            ((List<KartrisClasses.Address>)ViewState("lstUsrAddresses")).Remove(objAddress);
            if (UC_NewEditAddress.DisplayType == "Shipping")
                pnlShipping.Controls.Remove(UC_Delete);
            else
                pnlBilling.Controls.Remove(UC_Delete);
        }
    }

    /// <summary>
    /// Addresses - refresh
    /// </summary>
    /// <remarks></remarks>
    private void ResetAddressInput()
    {
        UC_NewEditAddress.Clear();
        chkMakeDefault.Checked = false;
        chkAlso.Checked = false;
    }

    /// <summary>
    /// Addresses - create shipping address
    /// </summary>
    /// <remarks></remarks>
    public void CreateShippingAddressesDetails()
    {
        var lstShippingAddresses = ((List<KartrisClasses.Address>)ViewState("lstUsrAddresses")).FindAll(ShippingAdd => ShippingAdd.Type == "s" | ShippingAdd.Type == "u");
        foreach (var objAddress in lstShippingAddresses)
        {
            UserControls_General_AddressDetails UC_Shipping = (UserControls_General_AddressDetails)LoadControl("~/UserControls/General/AddressDetails.ascx");
            UC_Shipping.ID = "dtlShipping-" + objAddress.ID;
            pnlShipping.Controls.Add(UC_Shipping);

            UserControls_General_AddressDetails UC_ShippingInstance = (UserControls_General_AddressDetails)mvwAddresses.Views(2).FindControl("dtlShipping-" + objAddress.ID);
            UC_ShippingInstance.Address = objAddress;
            UC_ShippingInstance.ShowButtons = true;
            UC_ShippingInstance.btnEditClicked += this.btnEditAddress_Click;
            UC_ShippingInstance.btnDeleteClicked += this.btnDeleteAddress_Click;
        }
    }

    /// <summary>
    /// Addresses - create billing address
    /// </summary>
    /// <remarks></remarks>
    public void CreateBillingAddressesDetails()
    {
        var lstBillingAddresses = ((List<KartrisClasses.Address>)ViewState("lstUsrAddresses")).FindAll(BillingAdd => BillingAdd.Type == "b" | BillingAdd.Type == "u");
        foreach (var objAddress in lstBillingAddresses)
        {
            UserControls_General_AddressDetails UC_Billing = (UserControls_General_AddressDetails)LoadControl("~/UserControls/General/AddressDetails.ascx");
            UC_Billing.ID = "UC_Billing-" + objAddress.ID;
            pnlBilling.Controls.Add(UC_Billing);
            UserControls_General_AddressDetails UC_BillingInstance = (UserControls_General_AddressDetails)mvwAddresses.Views(1).FindControl("UC_Billing-" + objAddress.ID);
            UC_BillingInstance.Address = objAddress;
            UC_BillingInstance.ShowButtons = true;
            UC_BillingInstance.btnEditClicked += this.btnEditAddress_Click;
            UC_BillingInstance.btnDeleteClicked += this.btnDeleteAddress_Click;
        }
    }
}