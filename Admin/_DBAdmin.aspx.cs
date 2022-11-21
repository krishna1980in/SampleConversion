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

partial class Admin_DBAdmin : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Kartris", "BackMenu_DBAdmin") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        if (!this.IsPostBack)
        {
            // Hide/disable some tabs if not in 'expertmode'
            if (KartSettingsManager.GetKartConfig("backend.expertmode") != "y")
            {
                tabMain.Visible = false;
                tabMain.HeaderText = "";

                tabDBRemoval.Visible = false;
                tabDBRemoval.HeaderText = "";

                tabExecuteQuery.Visible = false;
                tabExecuteQuery.HeaderText = "";

                tabDBTools.Visible = false;
                tabDBTools.HeaderText = "";

                tabFTS.Visible = false;
                tabFTS.HeaderText = "";
            }

            // ' Deleted Items
            CheckDeletedItems();
        }
    }

    public void CheckDeletedItems()
    {
        DataTable tblDeletedItems = KartrisDBBLL._GetDeletedItems();
        if (tblDeletedItems.Rows.Count > 0)
            phdDeletedItems.Visible = true;
        else
            phdDeletedItems.Visible = false;
        updDeletedItems.Update();
    }

    protected void _UC_AdminDataRemoval_BackUpData()
    {
        tabDBAdmin.ActiveTab = tabDBTools;
        updMain.Update();
    }

    public bool SetExpertSetting()
    {
        if (KartSettingsManager.GetKartConfig("backend.expertmode") != "y")
            return false;
        else
            return true;
    }

    protected void _UC_AdminExportData_ExportSaved()
    {
        (Skins_Admin_Template)this.Master.DataUpdated();
    }

    protected void lnkBtnDeleteFiles_Click(object sender, System.EventArgs e)
    {
        KartrisDBBLL.DeleteNotNeededFiles();
        CheckDeletedItems();
    }
}
