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

partial class Admin_CustomersList : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetLocalResourceObject("PageTitle_Customers") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        txtSearch.Focus();
        if (!Page.IsPostBack)
            txtSearch.Attributes.Add("onkeydown", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + btnSearch.ClientID + "').click();return false;}} else {return true}; ");

        string strMode = Request.QueryString("mode");

        if (Strings.Trim(strMode) != "")
        {
            if (strMode == "af")
            {
                _UC_CustomersList.isAffiliates = true;
                if (Trim(Request.QueryString("approve")) == "y")
                    _UC_CustomersList.isApproved = true;
                litCustomersListTitle.Text = GetLocalResourceObject("PageTitle_Customers") + ": <span class=\"h1_light\">" + GetGlobalResourceObject("_Customers", "PageTitle_Affiliates") + "</span>";
            }
            else if (strMode == "ml")
            {
                _UC_CustomersList.isMailingList = true;
                litCustomersListTitle.Text = GetLocalResourceObject("PageTitle_Customers") + ": <span class=\"h1_light\">" + GetGlobalResourceObject("_Customers", "PageTitle_MailingList") + "</span>";
            }
        }
        else if (Request.QueryString("cg") != "")
        {
            if (IsNumeric(Request.QueryString("cg")))
            {
                _UC_CustomersList.CustomerGroupID = System.Convert.ToInt32(Request.QueryString("cg"));
                string strCustomerGroupName = GetGlobalResourceObject("_Customers", "PageTitle_CustomerGroups");
                try
                {
                    DataRow[] drwCustomerGroup = KartSettingsManager.GetCustomerGroupsFromCache.Select("CG_ID=" + _UC_CustomersList.CustomerGroupID + " AND LANG_ID=" + CkartrisBLL.GetLanguageIDfromSession("b"));
                    if (drwCustomerGroup.Length != 0)
                        strCustomerGroupName = drwCustomerGroup[0]("CG_Name").ToString();
                }
                catch (Exception ex)
                {
                }
                litCustomersListTitle.Text = GetLocalResourceObject("PageTitle_Customers") + ": <span class=\"h1_light\">" + strCustomerGroupName + "</span>";
            }
        }

        (Skins_Admin_Template)this.Master.LoadTaskList();

        // Pre-fill search box if we're coming back from
        // a customer and sending back the search term
        if (!Page.IsPostBack)
        {
            txtSearch.Text = Request.QueryString("strSearch");
            RunSearch();
        }
    }

    // Handles search button clicked
    protected void btnSearch_Click(object sender, System.EventArgs e)
    {
        _UC_CustomersList.ResetCurrentPage();
        RunSearch();
    }

    // This actually runs the search; we can use this
    // from the search button, or if the page loads
    // with some values passed in via querystring
    protected void RunSearch()
    {
        UsersBLL objUsersBLL = new UsersBLL();
        if (IsNumeric(txtSearch.Text))
        {
            if ((DataTable)objUsersBLL._GetCustomerDetails(txtSearch.Text).Rows.Count > 0)
                Response.Redirect("_ModifyCustomerStatus.aspx?CustomerID=" + txtSearch.Text);
            else
            {
                _UC_CustomersList.strSearch = txtSearch.Text;
                _UC_CustomersList.RefreshCustomersList();
            }
        }
        else
        {
            _UC_CustomersList.strSearch = txtSearch.Text;
            _UC_CustomersList.RefreshCustomersList();
        }
    }
}
