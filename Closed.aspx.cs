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
    partial class _Closed : System.Web.UI.Page
    {
        public _Closed()
        {
            Load += Page_Load;
        }

        private void Page_Load(object sender, EventArgs e)
        {

            // If this page is called or hit by anyone when store is open, send
            // them to home page.
            if (KartSettingsManager.GetKartConfig("general.storestatus") == "open")
                Response.Redirect("~/");

        }
    }
}