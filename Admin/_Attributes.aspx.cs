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
using CkartrisFormatErrors;

partial class Admin_Attributes : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Product", "FormLabel_TabProductAttributes") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        valSummary.ValidationGroup = LANG_ELEM_TABLE_TYPE.Attributes;
        lnkBtnSave.ValidationGroup = LANG_ELEM_TABLE_TYPE.Attributes;
        ShowAttributeList("");

        if (dtlOptions.Controls.Count > 0)
        {
            // ' ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            // ' Loading the LanguageStrings for the Options' Part
            Control ctlOptFooter = dtlOptions.Controls(dtlOptions.Controls.Count - 1);
            (_LanguageContainer)ctlOptFooter.FindControl("_UC_LangContainer_NewOption").CreateLanguageStrings(LANG_ELEM_TABLE_TYPE.AttributeOption, true, 0);
            if (dtlOptions.SelectedIndex != -1)
            {
                int numOptionID = System.Convert.ToInt32((Literal)dtlOptions.SelectedItem.FindControl("litOptionID").Text);
                (_LanguageContainer)dtlOptions.SelectedItem.FindControl("_UC_LangContainer_SelectedOption").CreateLanguageStrings(LANG_ELEM_TABLE_TYPE.AttributeOption, false, numOptionID);
            }
        }


        if (Page.Form.Enctype != "multipart/form-data")
            // In this form there is a repeater with a file uploader. Becuase of the lifecycle of this page, the file upload control
            // cannot set the form encoding type at the correct time. For this reason we set the encoding type to Multipart at this point.
            // which is the required encoding type for file uploads.
            Page.Form.Enctype = "multipart/form-data";
    }

    protected void lnkBtnNewAttribute_Click(object sender, System.EventArgs e)
    {
        _UC_EditAttribute.EditAttribute(0);
        mvwAttributes.SetActiveView(vwEditAttribute);
        updAttributes.Update();
    }

    protected void gvwAttributes_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvwAttributes.PageIndex = e.NewPageIndex;
    }

    protected void gvwAttributes_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EditAttribute")
        {
            gvwAttributes.SelectedIndex = e.CommandArgument % gvwAttributes.PageSize;
            _UC_EditAttribute.EditAttribute(System.Convert.ToInt32(gvwAttributes.SelectedValue()));
            mvwAttributes.SetActiveView(vwEditAttribute);
            updAttributes.Update();
        }
        else if (e.CommandName == "EditValues")
        {
            mvwAttributes.SetActiveView(vwEditValues);
            litAttribID.Text = e.CommandArgument;
            LoadAttributeOptions(System.Convert.ToInt32(e.CommandArgument));
            updAttributes.Update();
        }
    }

    /// <summary>
    ///     ''' If an attribute option was selected, cancel this.
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private void CancelOptionSelection()
    {
        dtlOptions.SelectedIndex = -1;
    }

    /// <summary>
    ///     ''' Load options for a given attribute
    ///     ''' </summary>
    ///     ''' <param name="AttributeId"></param>
    ///     ''' <remarks>Works for checklist and dropdown option lists</remarks>
    private void LoadAttributeOptions(int AttributeId)
    {
        DataTable dtOptions = AttributesBLL.GetOptionsByAttributeID(AttributeId);

        // Add data that is not alreaddy in the table.
        dtOptions.Columns.Add("ATTRIB_ID", typeof(int));
        dtOptions.Columns.Add("ImageURL", typeof(string));
        foreach (DataRow dr in dtOptions.Rows)
            dr("ATTRIB_ID") = AttributeId;

        dtlOptions.DataSource = dtOptions;
        dtlOptions.DataBind();
    }

    protected void lnkBtnCancel_Click(object sender, System.EventArgs e)
    {
        mvwAttributes.SetActiveView(viwAttributeList);
        updAttributes.Update();
    }

    protected void lnkBtnSave_Click(object sender, System.EventArgs e)
    {
        if (_UC_EditAttribute.SaveChanges())
        {
            ShowAttributeList("");
            mvwAttributes.SetActiveView(viwAttributeList);
            updAttributes.Update();
            (Skins_Admin_Template)this.Master.DataUpdated();
        }
    }

    protected void lnkBtnDelete_Click(object sender, System.EventArgs e)
    {
        _UC_EditAttribute.Delete();
    }

    /// <summary>
    ///     ''' Close any add or edit controls and show the options.
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private void ShowAttributeOptions(int AttributeId)
    {
        Control ctlFooter = dtlOptions.Controls(dtlOptions.Controls.Count - 1);
        (PlaceHolder)ctlFooter.FindControl("phdNewItem").Visible = false;
        LoadAttributeOptions(AttributeId);
        updEditValues.Update();
    }

    /// <summary>
    ///     ''' Prepare the area for editing an existing option
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private void PrepareEditOptionEntry(int OptionIndex = -1)
    {
        if (OptionIndex > -1)
            dtlOptions.SelectedIndex = OptionIndex;
        Control ctlFooter = dtlOptions.Controls(dtlOptions.Controls.Count - 1);
        if (!IsNothing(ctlFooter))
        {
            if ((PlaceHolder)ctlFooter.FindControl("phdNewItem").Visible == true)
                // Hide "new" control is displayed
                (PlaceHolder)ctlFooter.FindControl("phdNewItem").Visible = false;
        }
    }

    /// <summary>
    ///     ''' Prepare the area for adding new attribute options
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private void PrepareNewOptionEntry()
    {
        dtlOptions.SelectedIndex = -1;
        ShowAttributeOptions(litAttribID.Text);

        Control ctlFooter = dtlOptions.Controls(dtlOptions.Controls.Count - 1);
        (PlaceHolder)ctlFooter.FindControl("phdNewItem").Visible = true;
        (_LanguageContainer)ctlFooter.FindControl("_UC_LangContainer_NewOption").CreateLanguageStrings(LANG_ELEM_TABLE_TYPE.AttributeOption, true, 0);
        (CheckBox)ctlFooter.FindControl("chkSelected").Focus();
    }

    /// <summary>
    ///     ''' Record a new Attribute Option to the database
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private void InsertNewOptionEntry()
    {
        Control ctlFooter = dtlOptions.Controls(dtlOptions.Controls.Count - 1);
        int numAttributeID = System.Convert.ToInt32(litAttribID.Text);
        int numOrderByValue = System.Convert.ToInt32((TextBox)ctlFooter.FindControl("txtOrderByValue").Text);
        string strMessage = "";
        try
        {
            DataTable tblContents = (_LanguageContainer)ctlFooter.FindControl("_UC_LangContainer_NewOption").ReadContent();
            if (tblContents.Rows.Count > 0)
            {
                if (!AttributesBLL._AddNewAttributeOption(tblContents, numAttributeID, numOrderByValue, strMessage))
                {
                    // If Not then raise error.
                    _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMessage);
        }
        ShowAttributeOptions(numAttributeID);
    }

    /// <summary>
    ///     ''' Update an option that already exists (Edit)
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private void UpdateOptionEntry()
    {
        string strMessage = "";               // Holds the return error message from called functions / methods
        _LanguageContainer LangContainer;     // Language control that holds the name of the option in all different languages
        DataTable tblContents = null/* TODO Change to default(_) if this is not a reference type */;      // Table of language strings (names of option) extracted from language container control
        TextBox txtSortOrder;                 // text box that holds the sort order
        int numOrderByValue = 0;          // Sort order extracted from control
        Literal litAttributeOptionId;               // Control that holds the attribute Option ID
        int numAttributeOptionId = 0;     // Attribute Option ID extracted from literal control
        DataListItem DLI = null/* TODO Change to default(_) if this is not a reference type */;

        foreach (DataListItem item in dtlOptions.Items)
        {
            if (item.ItemType == ListItemType.SelectedItem)
            {
                DLI = item;
                break;
            }
        }

        // Get the Attribute ID
        litAttributeOptionId = (Literal)DLI.FindControl("litOptionID");
        if (!IsNothing(litAttributeOptionId))
            numAttributeOptionId = litAttributeOptionId.Text;

        // Get sort order
        txtSortOrder = (TextBox)DLI.FindControl("txtOrderByValue");
        if (!IsNothing(txtSortOrder))
        {
            if (IsNumeric(txtSortOrder.Text))
                numOrderByValue = System.Convert.ToInt32(txtSortOrder.Text);
        }

        // Get string (name of option)
        LangContainer = (_LanguageContainer)DLI.FindControl("_UC_LangContainer_SelectedOption");
        if (!IsNothing(LangContainer))
            tblContents = LangContainer.ReadContent;

        try
        {
            if (!IsNothing(tblContents))
            {
                if (tblContents.Rows.Count > 0)
                {
                    if (!AttributesBLL._UpdateAttributeOption(tblContents, numAttributeOptionId, numOrderByValue, strMessage))
                    {
                        // If Not then raise error.
                        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                        return;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMessage);
        }

        dtlOptions.SelectedIndex = -1;
        LoadAttributeOptions(System.Convert.ToInt32(litAttribID.Text));
        updEditValues.Update();
    }

    private void ShowAttributeList(string strKeyword)
    {
        DataTable tblAttributes = new DataTable();
        tblAttributes = AttributesBLL._GetAttributesByLanguage(Session("_LANG"));

        if (tblAttributes.Rows.Count == 0)
        {
            mvwAttributeData.SetActiveView(vwNoItems); return;
        }
        mvwAttributeData.SetActiveView(vwAttributeList);
        tblAttributes.Columns.Add(new DataColumn("ATTRIB_TypeModified", Type.GetType("System.String")));
        tblAttributes.Columns.Add(new DataColumn("ATTRIB_CompareModified", Type.GetType("System.String")));

        foreach (DataRow row in tblAttributes.Rows)
        {
            switch (System.Convert.ToChar(row("ATTRIB_Type")))
            {
                case "t":
                    {
                        row("ATTRIB_TypeModified") = GetGlobalResourceObject("_Attributes", "FormLabel_AttributeTypeText");
                        break;
                    }

                case "d":
                    {
                        row("ATTRIB_TypeModified") = GetGlobalResourceObject("_Attributes", "FormLabel_AttributeTypeDropdown");
                        break;
                    }

                case "c":
                    {
                        row("ATTRIB_TypeModified") = GetGlobalResourceObject("_Attributes", "FormLabel_AttributeTypeCheckbox");
                        break;
                    }
            }
            switch (System.Convert.ToChar(row("ATTRIB_Compare")))
            {
                case "a":
                    {
                        row("ATTRIB_CompareModified") = GetGlobalResourceObject("_Attributes", "FormLabel_CompareAlways");
                        break;
                    }

                case "e":
                    {
                        row("ATTRIB_CompareModified") = GetGlobalResourceObject("_Attributes", "FormLabel_CompareEvery");
                        break;
                    }

                case "o":
                    {
                        row("ATTRIB_CompareModified") = GetGlobalResourceObject("_Attributes", "FormLabel_CompareOne");
                        break;
                    }

                case "n":
                    {
                        row("ATTRIB_CompareModified") = GetGlobalResourceObject("_Attributes", "FormLabel_CompareNever");
                        break;
                    }
            }
        }

        // We need to put the data into a dataview in order to
        // filter by the keywords
        DataView dvwAttributes = new DataView(tblAttributes);

        if (strKeyword != "")
        {
            dvwAttributes.RowFilter = "ATTRIB_Name LIKE'%" + strKeyword + "%'";
            dvwAttributes.Sort = "ATTRIB_Name";
        }

        gvwAttributes.DataSource = dvwAttributes;
        gvwAttributes.DataBind();
    }


    protected void _UC_EditAttribute_AttributeDeleted()
    {
        ShowAttributeList("");
        mvwAttributes.SetActiveView(viwAttributeList);
        updAttributes.Update();
        (Skins_Admin_Template)this.Master.DataUpdated();
    }

    protected void gvwAttributes_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton lnkBtnAttributeValues = (LinkButton)e.Row.FindControl("lnkBtnAttributeValues");
            if (!IsNothing(lnkBtnAttributeValues))
            {
                DataRowView dr = e.Row.DataItem;
                if (dr("ATTRIB_Type") == "c" | dr("ATTRIB_Type") == "d")
                    lnkBtnAttributeValues.Visible = true;
                else
                    lnkBtnAttributeValues.Visible = false;
            }
        }
    }

    protected void dtlOptions_ItemCommand(object source, DataListCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "new":
                {
                    PrepareNewOptionEntry();
                    break;
                }

            case "save":
                {
                    InsertNewOptionEntry();
                    break;
                }

            case "edit":
                {
                    PrepareEditOptionEntry(e.Item.ItemIndex);
                    LoadAttributeOptions(System.Convert.ToInt32(litAttribID.Text));
                    break;
                }

            case "cancel":
                {
                    dtlOptions.SelectedIndex = -1;
                    LoadAttributeOptions(System.Convert.ToInt32(litAttribID.Text));
                    updEditValues.Update();
                    break;
                }

            case "update":
                {
                    UpdateOptionEntry();
                    break;
                }

            case "delete":
                {
                    DeleteOptionEntry();
                    break;
                }

            case "LinkOptionGroupOptions":
                {
                    {
                        var withBlock = _UC_PopupAttributeOption;
                        withBlock.AttributeOptionId = System.Convert.ToInt32(e.CommandArgument);
                        withBlock.ShowAttributeOptions();
                    }

                    break;
                }
        }
    }

    /// <summary>
    ///     ''' User has clicked the delete button.
    ///     ''' </summary>
    ///     ''' <remarks>We need to confirm this selection</remarks>
    private new void DeleteOptionEntry()
    {
        string strOptionName = (Literal)dtlOptions.SelectedItem.FindControl("litOptionName").Text;
        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.Confirmation, Replace(GetGlobalResourceObject("_Kartris", "ContentText_ConfirmDeleteItem"), "[itemname]", strOptionName));
    }

    /// <summary>
    ///     ''' Delete a given attribute option
    ///     ''' </summary>
    ///     ''' <param name="OptionId"></param>
    ///     ''' <remarks></remarks>
    private new void DeleteOptionEntry(int OptionId)
    {
        string strMessage = null;
        if (AttributesBLL._DeleteAttributeOption(OptionId, strMessage))
        {
            CancelOptionSelection();
            ShowAttributeOptions(litAttribID.Text);
        }
        else
            _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
    }


    protected void dtlOptions_ItemDataBound(object sender, DataListItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.SelectedItem)
        {
            int numOptionID = System.Convert.ToInt32((Literal)e.Item.FindControl("litOptionID").Text);
            (_LanguageContainer)e.Item.FindControl("_UC_LangContainer_SelectedOption").CreateLanguageStrings(LANG_ELEM_TABLE_TYPE.AttributeOption, false, numOptionID);

            _FileUploader FileUploader = (_FileUploader)e.Item.FindControl("UC_SwatchFileUploader");
            if (!IsNothing(FileUploader))
                FileUploader.LoadImages();
        }
    }

    /// <summary>
    ///     ''' User wants to confirm the question that was asked in the popup box
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void _UC_PopupMsg_Confirmed()
    {
        if (dtlOptions.SelectedIndex > -1)
        {
            int numOptionID = System.Convert.ToInt32((Literal)dtlOptions.SelectedItem.FindControl("litOptionID").Text);
            DeleteOptionEntry(numOptionID);
        }
    }
}
