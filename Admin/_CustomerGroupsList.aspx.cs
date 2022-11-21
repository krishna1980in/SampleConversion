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
using CkartrisDataManipulation;
using KartSettingsManager;

partial class Admin_CustomerGroupsList : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetLocalResourceObject("PageTitle_CustomerGroups") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        lnkBtnSave.ValidationGroup = LANG_ELEM_TABLE_TYPE.CustomerGroups;
        valSummary.ValidationGroup = LANG_ELEM_TABLE_TYPE.CustomerGroups;

        if (!Page.IsPostBack)
            GetCustomersList();
        if (GetCGID() == 0)
            _UC_LangContainer.CreateLanguageStrings(LANG_ELEM_TABLE_TYPE.CustomerGroups, true);
        else
            _UC_LangContainer.CreateLanguageStrings(LANG_ELEM_TABLE_TYPE.CustomerGroups, false, GetCGID);
    }

    public void GetCustomersList()
    {
        gvwCustomers.DataSource = null;
        gvwCustomers.DataBind();

        mvwCustomerGroupList.SetActiveView(viwCGData);

        using (DataTable tblCGs = new DataTable())
        {
            DataRow[] rowCustomer = GetCustomerGroupsFromCache.Select("LANG_ID=" + Session("_LANG"));

            if (rowCustomer.Length == 0)
            {
                mvwCustomerGroupList.SetActiveView(viwCGNoItems); return;
            }

            tblCGs.Columns.Add(new DataColumn("CG_ID", Type.GetType("System.Int16")));
            tblCGs.Columns.Add(new DataColumn("LANG_ID", Type.GetType("System.Byte")));
            tblCGs.Columns.Add(new DataColumn("CG_Name", Type.GetType("System.String")));
            tblCGs.Columns.Add(new DataColumn("CG_Discount", Type.GetType("System.Single")));
            tblCGs.Columns.Add(new DataColumn("CG_Live", Type.GetType("System.Boolean")));

            for (int i = 0; i <= rowCustomer.Length - 1; i++)
                tblCGs.Rows.Add(System.Convert.ToInt16(rowCustomer[i]("CG_ID")), System.Convert.ToByte(rowCustomer[i]("LANG_ID")), System.Convert.ToString(rowCustomer[i]("CG_Name")), System.Convert.ToSingle(rowCustomer[i]("CG_Discount")), System.Convert.ToBoolean(rowCustomer[i]("CG_Live")));

            gvwCustomers.DataSource = tblCGs;
            gvwCustomers.DataBind();
        }
    }

    private void gvwCustomers_RowCommand(object src, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "EditCustomerGroup":
                {
                    gvwCustomers.SelectedIndex = System.Convert.ToInt32(e.CommandArgument) - (gvwCustomers.PageSize * gvwCustomers.PageIndex);
                    litCustomerGroupID.Text = gvwCustomers.SelectedValue;
                    PrepareExistingCG();
                    break;
                }
        }
    }

    protected void lnkBtnCancel_Click(object sender, System.EventArgs e)
    {
        litCustomerGroupID.Text = "0";
        mvwCustomerGroups.SetActiveView(viwMain);
        updCGDetails.Update();
    }

    protected void lnkBtnNewCG_Click(object sender, System.EventArgs e)
    {
        PrepareNewCG();
    }

    protected void lnkBtnSave_Click(object sender, System.EventArgs e)
    {
        // ' calling the save method for (INSERT/UPDATE)
        if (GetCGID() == 0)
        {
            if (!SaveCG(DML_OPERATION.INSERT))
                return;
        }
        else if (!SaveCG(DML_OPERATION.UPDATE))
            return;

        // Show animated 'updated' message
        (Skins_Admin_Template)this.Master.DataUpdated();

        GetCustomersList();

        litCustomerGroupID.Text = "0";
        mvwCustomerGroups.SetActiveView(viwMain);
        updCGDetails.Update();
    }

    public bool SaveCG(DML_OPERATION enumOperation)
    {
        UsersBLL objUsersBLL = new UsersBLL();
        DataTable tblLanguageElements = new DataTable();
        tblLanguageElements = _UC_LangContainer.ReadContent();

        float snglDiscount = 0;
        bool blnLive = chkCGLive.Checked;

        string strMessage = "";
        switch (enumOperation)
        {
            case object _ when DML_OPERATION.UPDATE:
                {
                    if (!objUsersBLL._UpdateCustomerGroups(tblLanguageElements, GetCGID(), snglDiscount, blnLive, strMessage))
                    {
                        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                        return false;
                    }

                    break;
                }

            case object _ when DML_OPERATION.INSERT:
                {
                    if (!objUsersBLL._AddCustomerGroups(tblLanguageElements, snglDiscount, blnLive, strMessage))
                    {
                        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                        return false;
                    }

                    break;
                }
        }
        RefreshCustomerGroupsCache();
        return true;
    }

    private int GetCGID()
    {
        if (litCustomerGroupID.Text != "")
            return System.Convert.ToInt32(litCustomerGroupID.Text);
        return 0;
    }

    public void PrepareNewCG()
    {
        litCustomerGroupID.Text = "0";
        _UC_LangContainer.CreateLanguageStrings(LANG_ELEM_TABLE_TYPE.CustomerGroups, true);
        chkCGLive.Checked = false;
        mvwCustomerGroups.SetActiveView(viwDetails);
        updCGDetails.Update();
    }

    public void PrepareExistingCG()
    {
        _UC_LangContainer.CreateLanguageStrings(LANG_ELEM_TABLE_TYPE.CustomerGroups, false, System.Convert.ToInt64(litCustomerGroupID.Text));
        chkCGLive.Checked = (CheckBox)gvwCustomers.SelectedRow.Cells(2).FindControl("chkCG_Live").Checked;
        mvwCustomerGroups.SetActiveView(viwDetails);
        updCGDetails.Update();
    }
}
