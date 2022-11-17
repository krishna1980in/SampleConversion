using System;
using System.Linq;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic
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
internal partial class Wishlist : PageBaseClass
{
    private bool blnPrivate = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        int numCustomerID = 0;
        int numWishlistID = 0;

        blnPrivate = false;
        if (!IsPostBack)
        {

            try
            {
                numWishlistID = Request.QueryString("WishlistID");
            }
            catch (Exception ex)
            {
            }

            if (numWishlistID > 0) // '// query string paramater passed
            {

                if (!User.Identity.IsAuthenticated)
                {
                    numCustomerID = 0;
                }
                else
                {
                    numCustomerID = CurrentLoggedUser.ID;
                }

                var tblWishList = new DataTable();
                var objBasket = new kartris.Basket();

                tblWishList = BasketBLL.GetCustomerWishList(numCustomerID, numWishlistID);

                bool blnAllow = false;
                if (tblWishList.Rows.Count > 0)
                {
                    if (tblWishList.Rows(0).Item("WL_UserID") == numCustomerID)
                        blnAllow = true;
                }

                if (blnAllow) // '// current login is the owner of passed wishlist id 
                {
                    blnPrivate = true;
                    litOwnerName.Text = Server.HtmlEncode(CkartrisDataManipulation.FixNullFromDB(tblWishList.Rows(0).Item("U_AccountHolderName"))) + "";
                    if (litOwnerName.Text == "")
                        litOwnerName.Text = Server.HtmlEncode(tblWishList.Rows(0).Item("U_EmailAddress")) + "";
                    litMessage.Text = tblWishList.Rows(0).Item("WL_Message");
                    rptWishList.DataSource = BasketBLL.GetRequiredWishlist(numCustomerID, numWishlistID, Session("LANG"));
                    rptWishList.DataBind();
                    pnlLogin.Visible = false;
                    pnlWishlist.Visible = true;
                }
                else // '// wishlist id is from the other owner or wishlist id doesn't exist
                {
                    {
                        var withBlock = popMessage;
                        withBlock.SetTitle = GetGlobalResourceObject("Kartris", "PageTitle_WishListLogin");
                        withBlock.SetTextMessage = GetGlobalResourceObject("Kartris", "ContentText_WishListNotFound");
                        withBlock.ShowPopup();
                    }
                    pnlLogin.Visible = true;
                    pnlWishlist.Visible = false;
                }

                tblWishList.Dispose();
                objBasket = default;
            }

            else // '// invalid passed wishlist id or not passed
            {

                blnPrivate = false;

                // '// for kartris v2
                var objSession = new SessionsBLL();
                if (Val(objSession.Value("WL_ID")) > 0) // '// get wishlist from current session
                {

                    var tblWishList = new DataTable();
                    var objBasket = new kartris.Basket();


                    // '// for kartris v2
                    Session("WL_ID") = objSession.Value("WL_ID");
                    litOwnerName.Text = Server.HtmlEncode(objSession.Value("WL_Owner"));
                    litMessage.Text = Server.HtmlEncode(objSession.Value("WL_Message"));
                    rptWishList.DataSource = BasketBLL.GetRequiredWishlist(Val(objSession.Value("WL_UserID")), Val(objSession.Value("WL_ID")), Session("LANG"));
                    rptWishList.DataBind();

                    tblWishList.Dispose();
                    objBasket = default;

                    pnlLogin.Visible = false;
                    pnlWishlist.Visible = true;
                }

                else
                {
                    pnlLogin.Visible = true;
                    pnlWishlist.Visible = false;
                }

            }

        }

    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (blnPrivate)
        {
            phdLogout.Visible = false;
        }
        else
        {
            phdLogout.Visible = true;
        }
    }

    protected void rptWishList_ItemDataBound(object sender, Global.System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {

        if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
        {
            ((HyperLink)e.Item.FindControl("lnkWishListItem")).Text = e.Item.DataItem("VersionName");
            string strURL = SiteMapHelper.CreateURL(SiteMapHelper.Page.CanonicalProduct, e.Item.DataItem("V_ProductID"));
            string strOptionLink = "";

            // Dim objOptionsBasket As New kartris.Basket

            if (!string.IsNullOrEmpty(BasketBLL.GetOptionText(CkartrisBLL.GetLanguageIDfromSession, e.Item.DataItem("BV_ID"), strOptionLink)))
            {
                strOptionLink = "&strOptions=" + strOptionLink;
            }
            else
            {
                strOptionLink = "&strOptions=0";
            }

            if (strURL.Contains("?"))
            {
                strURL = strURL + strOptionLink;
            }
            else
            {
                strURL = strURL + Strings.Replace(strOptionLink, "&", "?");

            } ((HyperLink)e.Item.FindControl("lnkWishListItem")).NavigateUrl = strURL;

            // objOptionsBasket = Nothing

            if (blnPrivate)
            {
                ((Literal)e.Item.FindControl("litRequired")).Text = e.Item.DataItem("WishlistQty") + "/" + e.Item.DataItem("BV_Quantity") + " " + GetGlobalResourceObject("Kartris", "ContentText_StillRequired");
            }
            else
            {
                ((Literal)e.Item.FindControl("litRequired")).Text = e.Item.DataItem("WishlistQty") + " " + GetGlobalResourceObject("Kartris", "ContentText_StillRequired");
            }

            if (e.Item.DataItem("WishlistQty") < 1)
            {
                ((Literal)e.Item.FindControl("litRequired")).Text = GetGlobalResourceObject("Kartris", "ContentText_AllRequiredPurchased");
            }

        }

    }

    public void WishListLogin_Click(object Sender, CommandEventArgs E)
    {
        // Dim objBasket As New kartris.Basket

        string strWishlistEmail = Strings.Trim(txtWishListEmail.Text);
        string strPassword = Strings.Trim(txtPassword.Text);

        DataTable tblWishList = BasketBLL.GetWishListLogin(strWishlistEmail, strPassword);

        if (tblWishList.Rows.Count > 0)
        {

            // '// for kartris v2
            var objSession = new SessionsBLL();
            objSession.Edit("WL_UserID", tblWishList.Rows(0).Item("WL_UserID"));
            objSession.Edit("WL_ID", tblWishList.Rows(0).Item("WL_ID"));
            objSession.Edit("WL_Owner", Interaction.IIf(tblWishList.Rows(0).Item("U_AccountHolderName") + "" == "", strWishlistEmail, tblWishList.Rows(0).Item("U_AccountHolderName")));
            objSession.Edit("WL_Message", tblWishList.Rows(0).Item("WL_Message"));
            litOwnerName.Text = Server.HtmlEncode(Interaction.IIf(tblWishList.Rows(0).Item("U_AccountHolderName") + "" == "", strWishlistEmail, tblWishList.Rows(0).Item("U_AccountHolderName")));
            litMessage.Text = Server.HtmlEncode(tblWishList.Rows(0).Item("WL_Message"));

            rptWishList.DataSource = BasketBLL.GetRequiredWishlist(tblWishList.Rows(0).Item("WL_UserID"), tblWishList.Rows(0).Item("WL_ID"), Session("LANG"));
            rptWishList.DataBind();

            Session("WL_UserID") = tblWishList.Rows(0).Item("WL_UserID");
            Session("WL_ID") = tblWishList.Rows(0).Item("WL_ID");
            Session("WL_Owner") = Interaction.IIf(tblWishList.Rows(0).Item("U_AccountHolderName") + "" == "", strWishlistEmail, tblWishList.Rows(0).Item("U_AccountHolderName"));
            Session("WL_Message") = tblWishList.Rows(0).Item("WL_Message");

            tblWishList.Dispose();
            // objBasket = Nothing

            pnlLogin.Visible = false;
            pnlWishlist.Visible = true;

            updWishlist.Update();
        }
        else
        {
            {
                var withBlock = popMessage;
                withBlock.SetTitle = GetGlobalResourceObject("Kartris", "PageTitle_WishListLogin");
                withBlock.SetTextMessage = GetGlobalResourceObject("Kartris", "ContentText_WishListNotFound");
                withBlock.ShowPopup();
            }
        }

        tblWishList.Dispose();
        // objBasket = Nothing

    }

    protected void Logout_Click()
    {

        // '// for kartris v2
        var objSession = new SessionsBLL();
        objSession.Delete("WL_UserID");
        objSession.Delete("WL_ID");
        objSession.Delete("WL_Owner");
        objSession.Delete("WL_Message");
        // '//

        // '// to be removed
        Session("WL_UserID") = 0;
        Session("WL_ID") = 0;
        Session("WL_Owner") = "";
        Session("WL_Message") = "";

        txtPassword.Text = "";

        pnlLogin.Visible = true;
        pnlWishlist.Visible = false;

        updWishlist.Update();

    }
}