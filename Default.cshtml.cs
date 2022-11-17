using System;
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
using KartSettingsManager;

internal partial class Main : PageBaseClass
{

    private kartris.Basket objBasket = new kartris.Basket();

    public Main()
    {
        this.Load += Page_Load;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        // Mailing list confirmation
        // Do this here rather than myaccount so
        // it does not rely on user being logged
        // in to account

        if (Request.QueryString("id") != "" & Request.QueryString("r") != "")
        {
            int intResult, UserID;
            string strPassword;

            UserID = Request.QueryString("id");
            strPassword = Request.QueryString("r");
            // strAction = "home"

            intResult = BasketBLL.ConfirmMail(UserID, strPassword);
            if (intResult != 0)
            {
                {
                    var withBlock = UC_PopUpConfirmMail;
                    // .SetWidthHeight(300, 75)
                    withBlock.SetTitle = GetGlobalResourceObject("Kartris", "PageTitle_MailingListProcess");
                    withBlock.SetTextMessage = GetGlobalResourceObject("Kartris", "ContentText_Thankyou");
                    withBlock.ShowPopup();
                }
            }

        }

        if (!Page.IsPostBack)
        {
            // Set canonical tag to webshopURL
            this.CanonicalTag = CkartrisBLL.WebShopURL;
        }

    }

}