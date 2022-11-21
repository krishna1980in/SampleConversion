// ========================================================================
// Kartris - www.kartris.com
// Copyright 2020 CACTUSOFT

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
using CkartrisDataManipulation;

partial class Admin_MarkupPrices : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Page.Title = GetGlobalResourceObject("_MarkupPrices", "PageTitle_MarkupPrices") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");
            InitializeContents();
        }
        if (ddlSupplier.Items.Count <= 1)
        {
            DataRow[] drwSuppliers = KartSettingsManager.GetSuppliersFromCache.Select("SUP_Live = 1");
            ddlSupplier.DataTextField = "SUP_Name";
            ddlSupplier.DataValueField = "SUP_ID";
            ddlSupplier.DataSource = drwSuppliers;
            ddlSupplier.DataBind();
        }
    }

    public void InitializeContents()
    {
        litCurrencySymbol1.Text = CurrenciesBLL.CurrencySymbol(CurrenciesBLL.GetDefaultCurrency());
        litCurrencySymbol2.Text = litCurrencySymbol1.Text;
        CategoriesBLL objCategoriesBLL = new CategoriesBLL();
        DataTable dtbCategories = objCategoriesBLL._GetWithProducts(Session("_LANG"));
        DataView dvwCategories = dtbCategories.DefaultView;
        dvwCategories.Sort = "CAT_Name";
        chklistCategories.Items.Clear();
        chklistCategories.DataTextField = "CAT_Name";
        chklistCategories.DataValueField = "CAT_ID";
        chklistCategories.DataSource = dvwCategories;
        chklistCategories.DataBind();

        ddlMarkType.Items.Clear();
        ddlMarkType.Items.Add(new ListItem("%", "p"));
        ddlMarkType.Items.Add(new ListItem(CurrenciesBLL.CurrencyCode(CurrenciesBLL.GetDefaultCurrency), "v"));
    }

    protected void rdo_CheckedChanged(object sender, System.EventArgs e)
    {
        phdCategories.Visible = !rdoAllCategories.Checked;
        updCategories.Update();
    }

    protected void btnSubmitStep1_Click(object sender, System.EventArgs e)
    {
        float numFromPrice = -1; // ' default min price
        float numToPrice = 99999999999; // ' default max price
        string strCategories = null;
        string strCategoryIDs = "0";
        int numSupplierID;

        // Collect value of supplier dropdown
        numSupplierID = ddlSupplier.SelectedValue;

        if (!string.IsNullOrEmpty(txtFromPrice.Text) && IsNumeric(txtFromPrice.Text))
            numFromPrice = System.Convert.ToSingle(txtFromPrice.Text);
        if (!string.IsNullOrEmpty(txtToPrice.Text) && IsNumeric(txtToPrice.Text))
            numToPrice = System.Convert.ToSingle(txtToPrice.Text);
        if (rdoSelectedCategories.Checked)
        {
            strCategoryIDs = "";
            foreach (ListItem itm in chklistCategories.Items)
            {
                // ' comma separeted category info (IDs and Names)
                if (itm.Selected)
                {
                    strCategories += itm.Text + " ,"; strCategoryIDs += itm.Value + ",";
                }
            }
            if (!string.IsNullOrEmpty(strCategories) && strCategories.EndsWith(","))
            {
                strCategories = strCategories.TrimEnd(","); strCategories = strCategories.Trim();
            }
            if (!string.IsNullOrEmpty(strCategoryIDs) && strCategoryIDs.EndsWith(","))
                strCategoryIDs = strCategoryIDs.TrimEnd(",");
        }

        if (string.IsNullOrEmpty(strCategoryIDs))
        {
            _UC_PopupMsg.ShowConfirmation(CkartrisEnumerations.MESSAGE_TYPE.ErrorMessage, GetGlobalResourceObject("_MarkupPrices", "ContentText_MarkupCategoriesNotSelected"));
            return;
        }

        VersionsBLL objVersionsBLL = new VersionsBLL();
        DataTable dtbVersionsRaw = objVersionsBLL._GetVersionsByCategoryList(Session("_LANG"), numFromPrice, numToPrice, strCategoryIDs);

        // Here we filter the raw versions by supplier ID if the supplier ID is not zero
        if (numSupplierID > 0)
            dtbVersionsRaw.DefaultView.RowFilter = ("P_SupplierID=" + numSupplierID);
        else
        {
        }

        DataTable dtbVersions = dtbVersionsRaw.DefaultView.ToTable;

        // Build up list of version IDs in order to pull
        // out qty discounts
        string strVersionIDs = "";
        foreach (DataRow drwVersion in dtbVersions.Rows)
        {
            if (strVersionIDs != "")
                strVersionIDs += ",";
            strVersionIDs += drwVersion("V_ID");
        }

        if (ddlTargetField.SelectedValue == "qd")
            dtbVersions = objVersionsBLL._GetQuantityDiscountsByVersionIDList(strVersionIDs, Session("_LANG"));
        else
            dtbVersions.Columns.Add(new DataColumn("QD_Quantity", Type.GetType("System.String")));

        dtbVersions.Columns.Add(new DataColumn("V_OldPrice", Type.GetType("System.String"))); // ' Original price with symbol
        dtbVersions.Columns.Add(new DataColumn("V_NewPrice", Type.GetType("System.String"))); // ' New price with symbol
        dtbVersions.Columns.Add(new DataColumn("V_OldRRP", Type.GetType("System.String"))); // ' Original rrp with symbol
        dtbVersions.Columns.Add(new DataColumn("V_NewRRP", Type.GetType("System.String"))); // ' New rrp with symbol

        float numValue = 0.0F;
        numValue = System.Convert.ToSingle(txtMarkValue.Text);
        if (ddlMarkType.SelectedValue == "v")
        {
            if (ddlMarkUpDown.SelectedValue == "down")
                numValue = 0 - numValue; // ' price will be marked down
            foreach (DataRow drwVersion in dtbVersions.Rows)
            {
                if (ddlTargetField.SelectedValue == "price")
                {
                    drwVersion("V_OldPrice") = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, drwVersion("V_Price"));
                    drwVersion("V_NewPrice") = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, drwVersion("V_Price") + numValue, false);
                }
                else if (ddlTargetField.SelectedValue == "rrp")
                {
                    drwVersion("V_OldRRP") = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, FixNullFromDB(drwVersion("V_RRP")));
                    drwVersion("V_NewRRP") = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, FixNullFromDB(drwVersion("V_RRP")) + numValue, false);
                }
                else if (ddlTargetField.SelectedValue == "qd")
                {
                    drwVersion("V_OldPrice") = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, FixNullFromDB(drwVersion("QD_Price")));
                    drwVersion("V_NewPrice") = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, FixNullFromDB(drwVersion("QD_Price")) + numValue, false);
                }
            }
        }
        else
            foreach (DataRow drwVersion in dtbVersions.Rows)
            {
                if (ddlTargetField.SelectedValue == "price")
                {
                    drwVersion("V_OldPrice") = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, drwVersion("V_Price"));
                    // ' price will be marked up
                    if (ddlMarkUpDown.SelectedValue == "up")
                        drwVersion("V_NewPrice") = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, drwVersion("V_Price") + ((numValue * drwVersion("V_Price")) / (double)100), false);
                    // ' price will be marked down
                    if (ddlMarkUpDown.SelectedValue == "down")
                        drwVersion("V_NewPrice") = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, drwVersion("V_Price") - ((numValue * drwVersion("V_Price")) / (double)100), false);
                }
                else if (ddlTargetField.SelectedValue == "rrp")
                {
                    float sngRRP = FixNullFromDB(drwVersion("V_RRP"));
                    drwVersion("V_OldRRP") = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, sngRRP);
                    // ' price will be marked up
                    if (ddlMarkUpDown.SelectedValue == "up")
                        drwVersion("V_NewRRP") = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, sngRRP + ((numValue * sngRRP) / (double)100), false);
                    // ' price will be marked down
                    if (ddlMarkUpDown.SelectedValue == "down")
                        drwVersion("V_NewRRP") = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, sngRRP - ((numValue * sngRRP) / (double)100), false);
                }
                else if (ddlTargetField.SelectedValue == "qd")
                {
                    drwVersion("V_OldPrice") = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, drwVersion("QD_Price"));
                    // ' price will be marked up
                    if (ddlMarkUpDown.SelectedValue == "up")
                        drwVersion("V_NewPrice") = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, drwVersion("QD_Price") + ((numValue * drwVersion("QD_Price")) / (double)100), false);
                    // ' price will be marked down
                    if (ddlMarkUpDown.SelectedValue == "down")
                        drwVersion("V_NewPrice") = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, drwVersion("QD_Price") - ((numValue * drwVersion("QD_Price")) / (double)100), false);
                }
            }

        if (ddlTargetField.SelectedValue == "price")
        {
            gvwVersions.Columns(1).Visible = false;
            gvwVersions.Columns(2).Visible = true;
            gvwVersions.Columns(3).Visible = true;
            gvwVersions.Columns(4).Visible = false;
            gvwVersions.Columns(5).Visible = false;
        }
        else if (ddlTargetField.SelectedValue == "rrp")
        {
            gvwVersions.Columns(1).Visible = false;
            gvwVersions.Columns(2).Visible = false;
            gvwVersions.Columns(3).Visible = false;
            gvwVersions.Columns(4).Visible = true;
            gvwVersions.Columns(5).Visible = true;
        }
        else if (ddlTargetField.SelectedValue == "qd")
        {
            gvwVersions.Columns(1).Visible = true;
            gvwVersions.Columns(2).Visible = true;
            gvwVersions.Columns(3).Visible = true;
            gvwVersions.Columns(4).Visible = false;
            gvwVersions.Columns(5).Visible = false;
        }

        gvwVersions.DataSource = dtbVersions;
        gvwVersions.DataBind();


        mvwMain.SetActiveView(viwStep2);
        updMain.Update();
    }

    protected void lnkBtnBack_Click(object sender, System.EventArgs e)
    {
        gvwVersions.DataSource = null;
        gvwVersions.DataBind();
        mvwMain.SetActiveView(viwStep1);
        updMain.Update();
    }

    protected void btnSave_Click(object sender, System.EventArgs e)
    {
        SaveChanges();
    }

    /// <summary>
    ///     ''' Save new prices
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void SaveChanges()
    {
        string strIDsPrices = null;
        string strSymbol = CurrenciesBLL.CurrencySymbol(CurrenciesBLL.GetDefaultCurrency);
        int numCounter = 0;

        // ' loop through all gridview rows and get info. to update prices for checked versions
        foreach (GridViewRow gvr in gvwVersions.Rows)
        {
            if (gvr.RowType == DataControlRowType.DataRow)
            {
                if ((CheckBox)gvr.Cells(3).FindControl("chkSave").Checked)
                {
                    numCounter += 1;
                    // ' Appened to list of IDs & Prices in the form of (ID1#Price1;ID2#Price2; ...) 
                    // '    during this will currency symbol will be ignored from the price
                    if (ddlTargetField.SelectedValue == "price")
                        strIDsPrices += (Literal)gvr.Cells(0).FindControl("litID").Text + "#" + Replace(HttpContext.Current.Server.HtmlDecode(gvr.Cells(3).Text), strSymbol, "") + ";";
                    else if (ddlTargetField.SelectedValue == "rrp")
                        strIDsPrices += (Literal)gvr.Cells(0).FindControl("litID").Text + "#" + Replace(HttpContext.Current.Server.HtmlDecode(gvr.Cells(5).Text), strSymbol, "") + ";";
                    else if (ddlTargetField.SelectedValue == "qd")
                        strIDsPrices += (Literal)gvr.Cells(0).FindControl("litID").Text + "," + HttpContext.Current.Server.HtmlDecode(gvr.Cells(1).Text) + "#" + Replace(HttpContext.Current.Server.HtmlDecode(gvr.Cells(3).Text), strSymbol, "") + ";";
                }
            }
        }

        if (numCounter == 0)
        {
            _UC_PopupMsg.ShowConfirmation(CkartrisEnumerations.MESSAGE_TYPE.ErrorMessage, GetGlobalResourceObject("_MarkupPrices", "ContentText_MarkupNoneSelected"));
            return;
        }
        else
        {
            if (strIDsPrices.EndsWith(";"))
                strIDsPrices = strIDsPrices.TrimEnd(";");
            string strMessage = null;
            if (!VersionsBLL._MarkupPrices(strIDsPrices, ddlTargetField.SelectedValue, strMessage))
            {
                // ' error occurred while updating the db
                _UC_PopupMsg.ShowConfirmation(CkartrisEnumerations.MESSAGE_TYPE.ErrorMessage, strMessage);
                return;
            }
            ClearForm(); // ' clear the form for new entry
            (Skins_Admin_Template)this.Master.DataUpdated();
        }
    }

    public void ClearForm()
    {
        txtFromPrice.Text = null;
        txtToPrice.Text = null;
        txtMarkValue.Text = null;
        ddlMarkType.SelectedIndex = 0;
        ddlMarkUpDown.SelectedIndex = 0;
        // ' clear selection from separate categories
        foreach (ListItem itm in chklistCategories.Items)
            itm.Selected = false;
        rdoAllCategories.Checked = true;
        rdoSelectedCategories.Checked = false;
        rdo_CheckedChanged(this, new EventArgs());
        mvwMain.SetActiveView(viwStep1);
        updMain.Update();
    }

    protected void SelectAllChanged(object sender, System.EventArgs e)
    {
        foreach (GridViewRow gvrVersion in gvwVersions.Rows)
        {
            if (gvrVersion.RowType == DataControlRowType.DataRow)
                (CheckBox)gvrVersion.Cells(3).FindControl("chkSave").Checked = (CheckBox)sender.Checked;
        }
    }

    protected void btnUploadPriceList_Click(object sender, System.EventArgs e)
    {
        if (filUploader.HasFile)
        {
            string strFileExt = Path.GetExtension(filUploader.PostedFile.FileName);
            string[] arrExcludedFileTypes = ConfigurationManager.AppSettings("ExcludedUploadFiles").ToString().Split(",");
            for (int i = 0; i <= arrExcludedFileTypes.GetUpperBound(0); i++)
            {
                if (Strings.Replace(strFileExt.ToLower(), ".", "") == arrExcludedFileTypes[i].ToLower())
                {
                    // Banned file type, don't upload
                    // Log error so attempts can be seen in logs
                    CkartrisFormatErrors.LogError("Attempt to upload a file of type: " + arrExcludedFileTypes[i].ToLower());
                    _UC_PopupMsg.ShowConfirmation(CkartrisEnumerations.MESSAGE_TYPE.ErrorMessage, "It is not permitted to upload files of this type. Change 'ExcludedUploadFiles' in the web.config if you need to upload this file.");
                    return;
                }
            }
            if (!Directory.Exists(Server.MapPath(KartSettingsManager.GetKartConfig("general.uploadfolder") + "temp/")))
                Directory.CreateDirectory(Server.MapPath(KartSettingsManager.GetKartConfig("general.uploadfolder") + "temp/"));
            string strFileName = KartSettingsManager.GetKartConfig("general.uploadfolder") + "temp/" + Guid.NewGuid().ToString() + strFileExt;
            filUploader.SaveAs(Server.MapPath(strFileName));
            StreamReader f = new StreamReader(Server.MapPath(strFileName));
            txtPriceList.Text = f.ReadToEnd();
            f.Dispose();
            try
            {
                System.IO.File.Delete(Server.MapPath(strFileName));
            }
            catch (Exception ex)
            {
            }
        }
        else
            _UC_PopupMsg.ShowConfirmation(CkartrisEnumerations.MESSAGE_TYPE.ErrorMessage, GetGlobalResourceObject("_Kartris", "ContentText_NoFile"));
    }

    protected void btnSubmitPriceList_Click(object sender, System.EventArgs e)
    {
        int numLines = 0;
        string strMessage = string.Empty;
        VersionsBLL objVersionsBLL = new VersionsBLL();
        if (objVersionsBLL._UpdatePriceList(txtPriceList.Text.Replace(Strings.Chr(10), "#"), numLines, strMessage))
        {
            txtPriceList.Text = null; updPriceList.Update(); // ' clear the list for new entry
            (Skins_Admin_Template)this.Master.DataUpdated();
        }
        else
            _UC_PopupMsg.ShowConfirmation(CkartrisEnumerations.MESSAGE_TYPE.ErrorMessage, strMessage + "<br/> Line " + numLines);
    }

    protected void btnUploadCustomerGroupPriceList_Click(object sender, System.EventArgs e)
    {
        if (filUploader2.HasFile)
        {
            string strFileExt = Path.GetExtension(filUploader2.PostedFile.FileName);
            string[] arrExcludedFileTypes = ConfigurationManager.AppSettings("ExcludedUploadFiles").ToString().Split(",");
            for (int i = 0; i <= arrExcludedFileTypes.GetUpperBound(0); i++)
            {
                if (Strings.Replace(strFileExt.ToLower(), ".", "") == arrExcludedFileTypes[i].ToLower())
                {
                    // Banned file type, don't upload
                    // Log error so attempts can be seen in logs
                    CkartrisFormatErrors.LogError("Attempt to upload a file of type: " + arrExcludedFileTypes[i].ToLower());
                    _UC_PopupMsg.ShowConfirmation(CkartrisEnumerations.MESSAGE_TYPE.ErrorMessage, "It is not permitted to upload files of this type. Change 'ExcludedUploadFiles' in the web.config if you need to upload this file.");
                    return;
                }
            }
            if (!Directory.Exists(Server.MapPath(KartSettingsManager.GetKartConfig("general.uploadfolder") + "temp/")))
                Directory.CreateDirectory(Server.MapPath(KartSettingsManager.GetKartConfig("general.uploadfolder") + "temp/"));
            string strFileName = KartSettingsManager.GetKartConfig("general.uploadfolder") + "temp/" + Guid.NewGuid().ToString() + strFileExt;
            filUploader2.SaveAs(Server.MapPath(strFileName));
            StreamReader f = new StreamReader(Server.MapPath(strFileName));
            txtCustomerGroupPriceList.Text = f.ReadToEnd();
            f.Dispose();
            try
            {
                System.IO.File.Delete(Server.MapPath(strFileName));
            }
            catch (Exception ex)
            {
            }
        }
        else
            _UC_PopupMsg.ShowConfirmation(CkartrisEnumerations.MESSAGE_TYPE.ErrorMessage, GetGlobalResourceObject("_Kartris", "ContentText_NoFile"));
    }

    protected void btnSubmitCustomerGroupPriceList_Click(object sender, System.EventArgs e)
    {
        int numLines = 0;
        string strMessage = string.Empty;

        // Paul mod 2021/01/13
        bool blnUseVersionCode = false;
        if (ddlVersionIdentifier.SelectedValue == "V_CodeNumber")
            blnUseVersionCode = true;
        VersionsBLL objVersionsBLL = new VersionsBLL();

        if (objVersionsBLL._UpdateCustomerGroupPriceList(txtCustomerGroupPriceList.Text.Replace(Strings.Chr(10), "#"), numLines, strMessage, blnUseVersionCode))
        {
            txtCustomerGroupPriceList.Text = null; updCustomerGroupPriceList.Update(); // ' clear the list for new entry
            (Skins_Admin_Template)this.Master.DataUpdated();
        }
        else
            _UC_PopupMsg.ShowConfirmation(CkartrisEnumerations.MESSAGE_TYPE.ErrorMessage, strMessage + "<br/> Line " + numLines);
    }

    protected void btnUploadQuantityDiscounts_Click(object sender, System.EventArgs e)
    {
        if (filUploader3.HasFile)
        {
            string strFileExt = Path.GetExtension(filUploader3.PostedFile.FileName);
            string[] arrExcludedFileTypes = ConfigurationManager.AppSettings("ExcludedUploadFiles").ToString().Split(",");
            for (int i = 0; i <= arrExcludedFileTypes.GetUpperBound(0); i++)
            {
                if (Strings.Replace(strFileExt.ToLower(), ".", "") == arrExcludedFileTypes[i].ToLower())
                {
                    // Banned file type, don't upload
                    // Log error so attempts can be seen in logs
                    CkartrisFormatErrors.LogError("Attempt to upload a file of type: " + arrExcludedFileTypes[i].ToLower());
                    _UC_PopupMsg.ShowConfirmation(CkartrisEnumerations.MESSAGE_TYPE.ErrorMessage, "It is not permitted to upload files of this type. Change 'ExcludedUploadFiles' in the web.config if you need to upload this file.");
                    return;
                }
            }
            if (!Directory.Exists(Server.MapPath(KartSettingsManager.GetKartConfig("general.uploadfolder") + "temp/")))
                Directory.CreateDirectory(Server.MapPath(KartSettingsManager.GetKartConfig("general.uploadfolder") + "temp/"));
            string strFileName = KartSettingsManager.GetKartConfig("general.uploadfolder") + "temp/" + Guid.NewGuid().ToString() + strFileExt;
            filUploader3.SaveAs(Server.MapPath(strFileName));
            StreamReader f = new StreamReader(Server.MapPath(strFileName));
            txtQuantityDiscounts.Text = f.ReadToEnd();
            f.Dispose();
            try
            {
                System.IO.File.Delete(Server.MapPath(strFileName));
            }
            catch (Exception ex)
            {
            }
        }
        else
            _UC_PopupMsg.ShowConfirmation(CkartrisEnumerations.MESSAGE_TYPE.ErrorMessage, GetGlobalResourceObject("_Kartris", "ContentText_NoFile"));
    }

    protected void btnSubmitQuantityDiscounts_Click(object sender, System.EventArgs e)
    {

        // We want to parse the price list row here first, identify the parameters and 
        // then send these to sproc, rather than try to do the parsing there which seems
        // to be far more complicated.
        // 'QD_VersionID','QD_Quantity','QD_Price'
        int numQD_VersionID = 0;
        Int64 numQD_Quantity = 0;
        double numQD_Price = 0;
        Int64 numCounter = 0;

        string strMessage = string.Empty;

        string[] aryQuantityDiscountLines = txtQuantityDiscounts.Text.Replace(Strings.Chr(10), "#").Split(new char[] { '#' });

        DataTable tblQtyDiscount = new DataTable();
        tblQtyDiscount.Columns.Add(new DataColumn("QD_VersionID", Type.GetType("System.Int64")));
        tblQtyDiscount.Columns.Add(new DataColumn("QD_Quantity", Type.GetType("System.Single")));
        tblQtyDiscount.Columns.Add(new DataColumn("QD_Price", Type.GetType("System.Single")));
        VersionsBLL objVersionsBLL = new VersionsBLL();
        for (numCounter = 0; numCounter <= aryQuantityDiscountLines.Length - 1; numCounter++)
        {
            try
            {
                string[] aryThisLine = aryQuantityDiscountLines[numCounter].Split(new char[] { ',' });

                numQD_VersionID = System.Convert.ToInt32(aryThisLine[0]);
                numQD_Quantity = System.Convert.ToInt64(aryThisLine[1]);
                numQD_Price = Convert.ToDouble(aryThisLine[2]);

                tblQtyDiscount.Rows.Add(numQD_VersionID, numQD_Quantity, HandleDecimalValues(numQD_Price));

                objVersionsBLL._UpdateQuantityDiscount(tblQtyDiscount, numQD_VersionID, strMessage);
            }
            catch (Exception ex)
            {
            }
        }

        (Skins_Admin_Template)this.Master.DataUpdated();
    }
}
