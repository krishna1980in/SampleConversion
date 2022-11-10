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
    partial class Customer_Account : PageBaseClass
    {
        public Customer_Account()
        {
            this.Load += Page_Load;
        }

        private void Page_Load(object sender, EventArgs e)
        {

            // Don't show this page if user already logged in, go to the customer home page
            if (!Page.IsPostBack)
            {
                if (LCase(KartSettingsManager.GetKartConfig("frontend.users.access")) == "yes")
                    litMustBeLoggedIn.Visible = true;
                if (User.Identity.IsAuthenticated)
                    Response.Redirect("~/Customer.aspx?action=home");
            }

        }

    }
}