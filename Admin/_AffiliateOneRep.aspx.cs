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

partial class _AffiliateOneRep : _PageBaseClass
{
    private int numMonth, numYear, numAffiliateID;
    private int intPageSize = 15;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Kartris", "PageTitle_AffiliateStats") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        try
        {
            numAffiliateID = Request.QueryString("CustomerID");
        }
        catch (Exception ex)
        {
            Response.Redirect("_AffiliateReport.aspx");
        }

        try
        {
            numMonth = Request.QueryString("GivenMonth");
            numYear = Request.QueryString("GivenYear");
        }
        catch (Exception ex)
        {
            numMonth = Month(CkartrisDisplayFunctions.NowOffset);
            numYear = Year(CkartrisDisplayFunctions.NowOffset);
        }

        if (!(IsPostBack))
        {
            litHitsReportDate.Text = DateTime.MonthName(numMonth) + " " + numYear;

            DisplaySummaryReport();

            DisplayRawDataHitsReport();

            DisplayRawDataSalesReport();

            lnkAffPayReport.NavigateUrl = "_AffiliatePayRep.aspx?CustomerID=" + numAffiliateID;
        }
    }

    private void DisplaySummaryReport()
    {
        System.Data.DataTable tblSummary = new System.Data.DataTable();
        kartris.Basket objBasket = new kartris.Basket();

        tblSummary = AffiliateBLL._GetAffiliateSummaryReport(numMonth, numYear, numAffiliateID);
        if (tblSummary.Rows.Count > 0)
        {
            litAffiliateName.Text = tblSummary.Rows[0].Item["U_EmailAddress"];
            litTotalOrder.Text = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, tblSummary.Rows[0].Item["OrderTotal"]);
            litCommissionMonth.Text = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, tblSummary.Rows[0].Item["Commission"]);
            litTotalHits.Text = tblSummary.Rows[0].Item["Clicks"];
        }

        tblSummary.Dispose();
        objBasket = null/* TODO Change to default(_) if this is not a reference type */;
    }

    private void DisplayRawDataHitsReport()
    {
        System.Data.DataTable tblSummary = new System.Data.DataTable();
        kartris.Basket objBasket = new kartris.Basket();
        int intTotalRowCount;

        tblSummary = AffiliateBLL._GetAffiliateRawDataHitsReport(numMonth, numYear, numAffiliateID, 1, 1000000);
        intTotalRowCount = tblSummary.Rows.Count;

        phdNoResults.Visible = Interaction.IIf(intTotalRowCount > 0, false, true);

        if (intTotalRowCount <= _UC_RawDataHitsPager.CurrentPage * _UC_RawDataHitsPager.ItemsPerPage)
            _UC_RawDataHitsPager.CurrentPage = IIf(_UC_RawDataHitsPager.CurrentPage - 1 < 0, 0, _UC_RawDataHitsPager.CurrentPage - 1);

        gvwRawDataHits.DataSource = AffiliateBLL._GetAffiliateRawDataHitsReport(numMonth, numYear, numAffiliateID, _UC_RawDataHitsPager.CurrentPage + 1, intPageSize);
        gvwRawDataHits.DataBind();

        _UC_RawDataHitsPager.TotalItems = intTotalRowCount;
        _UC_RawDataHitsPager.ItemsPerPage = intPageSize;
        _UC_RawDataHitsPager.PopulatePagerControl();

        tblSummary.Dispose();
        objBasket = null/* TODO Change to default(_) if this is not a reference type */;
    }

    private void DisplayRawDataSalesReport()
    {
        System.Data.DataTable tblSummary = new System.Data.DataTable();
        kartris.Basket objBasket = new kartris.Basket();
        int intTotalRowCount;

        tblSummary = AffiliateBLL._GetAffiliateRawDataSalesReport(numMonth, numYear, numAffiliateID, 1, 1000000);
        intTotalRowCount = tblSummary.Rows.Count;

        phdNoResults2.Visible = Interaction.IIf(intTotalRowCount > 0, false, true);

        if (intTotalRowCount <= _UC_RawDataSalesPager.CurrentPage * _UC_RawDataSalesPager.ItemsPerPage)
            _UC_RawDataSalesPager.CurrentPage = IIf(_UC_RawDataSalesPager.CurrentPage - 1 < 0, 0, _UC_RawDataSalesPager.CurrentPage - 1);

        gvwRawDataSales.DataSource = AffiliateBLL._GetAffiliateRawDataSalesReport(numMonth, numYear, numAffiliateID, _UC_RawDataSalesPager.CurrentPage + 1, intPageSize);
        gvwRawDataSales.DataBind();

        _UC_RawDataSalesPager.TotalItems = intTotalRowCount;
        _UC_RawDataSalesPager.ItemsPerPage = intPageSize;
        _UC_RawDataSalesPager.PopulatePagerControl();

        tblSummary.Dispose();
        objBasket = null/* TODO Change to default(_) if this is not a reference type */;
    }

    protected void _UC_RawDataHitsPager_PageChanged()
    {
        DisplayRawDataHitsReport();
    }

    protected void _UC_RawDataSalesPager_PageChanged()
    {
        DisplayRawDataSalesReport();
    }

    protected string GetDateTime(DateTime datDate)
    {
        return CkartrisDisplayFunctions.FormatDate(datDate, "t", Session("_LANG"));
    }
}
