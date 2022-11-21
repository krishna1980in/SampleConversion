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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

partial class Admin_PaymentsList : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Kartris", "ContentText_Payments") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        if (!Page.IsPostBack)
            _UC_PaymentsList.RefreshPaymentsList();
        txtSearch.Focus();
    }


    protected void btnSearch_Click(object sender, System.EventArgs e)
    {

        // Handle search for payment number
        // Don't check we find one, quicker
        // this way.
        if (IsNumeric(txtSearch.Text))
        {
            // Search by Payment ID
            int intPaymentID = System.Convert.ToInt32(txtSearch.Text);
            Response.Redirect("_ModifyPayment.aspx?PaymentID=" + intPaymentID);
        }

        // Reset the order list's pager control
        _UC_PaymentsList.ResetCurrentPage();

        if (IsDate(txtSearch.Text))
        {
            // Date search
            _UC_PaymentsList.datValue1 = txtSearch.Text;
            _UC_PaymentsList.datGateway = null;
            _UC_PaymentsList.CallMode = "d";
        }
        else
        {
            _UC_PaymentsList.CallMode = "g";
            _UC_PaymentsList.datValue1 = null;
            _UC_PaymentsList.datGateway = txtSearch.Text;
        }
        _UC_PaymentsList.RefreshPaymentsList();
    }

    protected void ShowMasterUpdateMessage()
    {
        (Skins_Admin_Template)this.Master.DataUpdated();
        (Skins_Admin_Template)this.Master.LoadTaskList();
    }
}
