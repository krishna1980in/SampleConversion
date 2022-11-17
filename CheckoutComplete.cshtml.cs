using System;
using Microsoft.VisualBasic; 
// Install-Package Microsoft.VisualBasic
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

internal partial class CheckoutComplete : PageBaseClass
{
    public CheckoutComplete()
    {
        this.Load += Page_Load;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session("OrderDetails") is not null & Session("OrderID") is not null)
        {
            string strOrderDetails = Session("OrderDetails");
            if (KartSettingsManager.GetKartConfig("general.email.enableHTML") == "y")
            {
                litOrderDetails.Text = CkartrisBLL.ExtractHTMLBodyContents(strOrderDetails);
            }
            else
            {
                litOrderDetails.Text = Strings.Replace(strOrderDetails, Constants.vbCrLf, "<br/>");
            }

            Session("OrderID") = null;
            Session("OrderDetails") = null;
        }
    }

}