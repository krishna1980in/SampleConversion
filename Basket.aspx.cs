using System;

namespace EmployeeManagementSystem
{
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
    partial class Basket : PageBaseClass
    {
        public Basket()
        {
            this.Load += Page_Load;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = GetGlobalResourceObject("Basket", "PageTitle_ShoppingBasket") + " | " + Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));

            if (!IsPostBack)
            {
                if (Request.QueryString("error") == "minimum")
                {
                    UC_PopUpErrors.SetTextMessage = GetLocalResourceObject("Popup_OrderBelowMin");
                    UC_PopUpErrors.SetTitle = GetGlobalResourceObject("Kartris", "ContentText_CorrectErrors");
                    UC_PopUpErrors.ShowPopup();
                }
            }
        }
    }
}