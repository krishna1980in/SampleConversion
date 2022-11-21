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

partial class Admin_OrdersList : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetLocalResourceObject("PageTitle_Orders") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        OrdersBLL.ORDERS_LIST_CALLMODE CallMode;
        if (!Page.IsPostBack)
        {
            if (Trim(Request.QueryString("CallMode")) != "")
            {
                try
                {
                    // Read the query string and see if it's one of the Order list's Callmode
                    string strCallMode = Request.QueryString("CallMode");

                    CallMode = System.Enum.Parse(typeof(OrdersBLL.ORDERS_LIST_CALLMODE), Strings.UCase(strCallMode));

                    // Set the Page Title and Description text for the Callmode
                    ProcessCallMode(CallMode);
                }
                catch (Exception ex)
                {
                    // Display in recent mode if the Querystring is invalid
                    CallMode = OrdersBLL.ORDERS_LIST_CALLMODE.RECENT;
                }
            }
            else
                CallMode = OrdersBLL.ORDERS_LIST_CALLMODE.RECENT;
        }
        txtSearch.Focus();
    }

    private void ProcessCallMode(OrdersBLL.ORDERS_LIST_CALLMODE CallMode)
    {
        var strCallMode = StrConv(System.Enum.GetName(typeof(OrdersBLL.ORDERS_LIST_CALLMODE), CallMode), VbStrConv.ProperCase);
        if (Strings.LCase(strCallMode) == "bybatch")
        {
            litOrderListMode.Text = GetLocalResourceObject("PageTitle_BatchProcess");
            litModeDetails.Text = GetLocalResourceObject("ContentText_BatchProcessText");
        }
        else if (Strings.LCase(strCallMode) == "customer")
        {
            litOrderListMode.Text = GetLocalResourceObject("PageTitle_OrdersCustomerEmail");
            litModeDetails.Text = GetLocalResourceObject("ContentText_OrdersEmailText");
            _UC_OrdersList.datValue2 = Request.QueryString("CustomerID");
        }
        else
        {
            litOrderListMode.Text = GetLocalResourceObject("PageTitle_Orders" + strCallMode);
            litModeDetails.Text = GetLocalResourceObject("ContentText_Orders" + strCallMode + "Text");
        }

        if (CallMode == OrdersBLL.ORDERS_LIST_CALLMODE.GATEWAY)
        {
            litEnterDate.Text = GetLocalResourceObject("ContentText_SelectGateWay");
            lblFindOrder.Text = GetLocalResourceObject("ContextText_TransID");
            phdGateway.Visible = true;
            btnCalendar.Visible = false;
            calDateSearch.Enabled = false;
            if (ddlGateways.Items.Count > 0)
            {
                if (ddlGateways.Items(0).Value != "All Payment Gateways")
                    ddlGateways.Items.Insert(0, "All Payment Gateways");
            }
            else
                ddlGateways.Items.Insert(0, "All Payment Gateways");
        }
        else if (CallMode == OrdersBLL.ORDERS_LIST_CALLMODE.BYBATCH)
        {
            pnlSearch.Visible = false;
            _UC_OrdersList.BatchProcess = true;
            CallMode = OrdersBLL.ORDERS_LIST_CALLMODE.BYDATE;
        }
        _UC_OrdersList.CallMode = CallMode;
    }

    protected void btnSearch_Click(object sender, System.EventArgs e)
    {

        // Handle search for order number
        // Don't check we find one, quicker
        // this way.
        if (IsNumeric(txtSearch.Text))
        {
            // Search by Order ID
            int intOrderID = System.Convert.ToInt32(txtSearch.Text);
            Response.Redirect("_ModifyOrderStatus.aspx?OrderID=" + intOrderID);
        }

        // Reset the order list's pager control
        _UC_OrdersList.ResetCurrentPage();
        if (Trim(Request.QueryString("CallMode")) == Strings.LCase("gateway"))
        {
            ProcessCallMode(OrdersBLL.ORDERS_LIST_CALLMODE.GATEWAY);
            _UC_OrdersList.datValue1 = ddlGateways.SelectedValue;
            _UC_OrdersList.datValue2 = txtSearch.Text;
            _UC_OrdersList.RefreshOrdersList();
        }
        else
            // Check the search box and decide what type of
            // search this is
            if (Trim(txtSearch.Text) == "")
        {
        }
        else if (IsDate(txtSearch.Text))
        {
            // Date search
            _UC_OrdersList.datValue1 = txtSearch.Text;
            ProcessCallMode(OrdersBLL.ORDERS_LIST_CALLMODE.BYDATE);
            _UC_OrdersList.RefreshOrdersList();
            if (IsDate(_UC_OrdersList.datValue1) & _UC_OrdersList.datValue1 != DateTime.MinValue)
                txtSearch.Text = _UC_OrdersList.datValue1;
        }
        else
        {
            // Text search
            _UC_OrdersList.datValue2 = txtSearch.Text;
            ProcessCallMode(OrdersBLL.ORDERS_LIST_CALLMODE.SEARCH);
            _UC_OrdersList.RefreshOrdersList();
        }
    }

    protected void ShowMasterUpdateMessage()
    {
        (Skins_Admin_Template)this.Master.DataUpdated();
        (Skins_Admin_Template)this.Master.LoadTaskList();
    }
}
