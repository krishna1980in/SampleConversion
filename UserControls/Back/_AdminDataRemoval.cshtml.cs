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
using CkartrisImages;
using CkartrisDataManipulation;

partial class UserControls_Back_AdminDataRemoval : System.Web.UI.UserControl
{
    public event BackUpDataEventHandler BackUpData;

    public delegate void BackUpDataEventHandler();

    public void LoadTablesToBeRemoved(char chrDataType)
    {
        DataTable tblTablesToBeRemoved = KartrisDBBLL._GetAdminRelatedTables(chrDataType);

        bulTablesToBeRemoved.DataSource = tblTablesToBeRemoved;
        bulTablesToBeRemoved.DataTextField = "ShortName";
        bulTablesToBeRemoved.DataBind();
    }

    protected void btnSubmit_Click(object sender, System.EventArgs e)
    {
        switch (ddlDataToRemove.SelectedValue)
        {
            case "P":
            case "O":
            case "S":
            case "C":
                {
                    LoadTablesToBeRemoved(System.Convert.ToChar(ddlDataToRemove.SelectedValue));
                    mvwAdminRelatedTables.SetActiveView(viwAdminRelatedTablesWarning);
                    break;
                }

            default:
                {
                    mvwAdminRelatedTables.SetActiveView(viwAdminRelatedTablesEmpty);
                    break;
                }
        }
    }

    protected void lnkBtnOpenLogin_Click(object sender, System.EventArgs e)
    {
        _UC_LoginPopup.ShowLogin();
    }

    public void ClearData(char chrDataType, string strUserName, string strPassword)
    {
        bool blnSucceeded = false;
        string strOutput = "";
        KartrisDBBLL._AdminClearRelatedData(chrDataType, strUserName, strPassword, blnSucceeded, strOutput);
        ddlDataToRemove.SelectedIndex = 0;
        if (!blnSucceeded)
        {
            litError.Text = strOutput;
            mvwAdminRelatedTables.SetActiveView(viwAdminRelatedTablesFailed);
        }
        else
        {
            if (chrDataType == "P")
                RemoveProductsRelatedImages();
            RefreshSiteMap();
            litOutput.Text = strOutput;
            mvwAdminRelatedTables.SetActiveView(viwAdminRelatedTablesSucceeded);
        }
        updAdminDataRemoval.Update();
    }

    protected void _UC_LoginPopup_SubmitClicked(string strUserName, string strPassword)
    {
        switch (ddlDataToRemove.SelectedValue)
        {
            case "P":
            case "O":
            case "S":
            case "C":
                {
                    ClearData(System.Convert.ToChar(ddlDataToRemove.SelectedValue), strUserName, strPassword);
                    break;
                }
        }
    }

    protected void btnCancel_Click(object sender, System.EventArgs e)
    {
        ddlDataToRemove.SelectedIndex = 0;
        mvwAdminRelatedTables.SetActiveView(viwAdminRelatedTablesEmpty);
        updAdminDataRemoval.Update();
    }

    protected void btnBackupNow_Click(object sender, System.EventArgs e)
    {
        BackUpData?.Invoke();
    }
}
