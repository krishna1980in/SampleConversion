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
using CkartrisEnumerations;
using KartSettingsManager;
using System.Web.HttpContext;

partial class Admin_StockNotifications : _PageBaseClass
{
    public event ShowMasterUpdateEventHandler ShowMasterUpdate;

    public delegate void ShowMasterUpdateEventHandler();

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = System.Web.HttpContext.GetGlobalResourceObject("_StockNotification", "ContentText_StockNotifications") + " | " + System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        if (!Page.IsPostBack)
        {
            int intRowsPerPage = 25;
            try
            {
                intRowsPerPage = System.Convert.ToDouble(KartSettingsManager.GetKartConfig("backend.display.pagesize"));
            }
            catch (Exception ex)
            {
            }
            gvwDetails.PageSize = intRowsPerPage;

            LoadStockNotifications();

            // Hide the button and gvw if no items awaiting checks
            if (gvwDetails.Rows.Count == 0)
                phdCurrentNotifications.Visible = false;
            else
                phdCurrentNotifications.Visible = true;
        }
    }

    // Run notifications check button pushed
    protected void btnCheckAndSend_Click(object sender, System.EventArgs e)
    {
        StockNotificationsBLL._RunBulkCheck();
        LoadStockNotifications();
        updStockNotifications.Update();
        ShowMasterUpdate?.Invoke();
    }

    // Load up and refresh the gridviews
    private void LoadStockNotifications()
    {
        gvwDetails.DataSource = StockNotificationsBLL._GetStockNotificationsDetails("w");
        gvwDetails.DataBind();
        gvwDetailsClosed.DataSource = StockNotificationsBLL._GetStockNotificationsDetails("s");
        gvwDetailsClosed.DataBind();
    }

    // Page notifications gridview
    protected void gvwDetails_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvwDetails.PageIndex = e.NewPageIndex;
        LoadStockNotifications();
    }

    // Page closed notifications gridview
    protected void gvwDetailsClosed_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvwDetailsClosed.PageIndex = e.NewPageIndex;
        LoadStockNotifications();
    }
}
