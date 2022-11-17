using System;
using System.Reflection;
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

internal partial class Customer_Invoice : PageBaseClass
{
    public Customer_Invoice()
    {
        this.Load += Page_Load;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        int numOrderID, numCustomerID = default;

        // Bump customer out if not authenticated
        if (!User.Identity.IsAuthenticated)
        {
            Response.Redirect("~/CustomerAccount.aspx");
        }
        else
        {
            numCustomerID = ((KartrisMemberShipUser)Membership.GetUser()).ID;
        }

        // Display invoice
        try
        {
            numOrderID = (int)Request.QueryString("O_ID");
            UC_Invoice.CustomerID = numCustomerID;
            UC_Invoice.OrderID = numOrderID;
            UC_Invoice.FrontOrBack = "back"; // tell user control is on back end
        }
        catch (Exception ex)
        {
            CkartrisFormatErrors.ReportHandledError(ex, MethodBase.GetCurrentMethod());
            HttpContext.Current.Server.Transfer("~/Error.aspx");
        }

    }

}