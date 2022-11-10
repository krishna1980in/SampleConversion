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
    partial class Promotions : PageBaseClass
    {
        public Promotions()
        {
            this.Load += Page_Load;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (KartSettingsManager.GetKartConfig("frontend.promotions.enabled") == "y")
            {
                Page.Title = GetGlobalResourceObject("Kartris", "SubHeading_Promotions") + " | " + Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));
                UC_Promotions.LoadAllPromotions(Session("LANG"));
            }
            else
            {
                mvwMain.SetActiveView(viwNotExist);
            }
        }
    }
}