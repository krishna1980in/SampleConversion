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

partial class UserControls_Statistics_ProductStats : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!System.Web.UI.Control.Page.IsPostBack & KartSettingsManager.GetKartConfig("backend.homepage.graphs") != "OFF")
        {
            foreach (ListItem itm in ddlDuration.Items)
                itm.Text += " " + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_Months");
            GetProductYearSummary();
        }
    }

    protected void lnkBackTable_Click(object sender, System.EventArgs e)
    {
        mvwYearSummaryTable.SetActiveView(viwYearSummaryListTable);
        phdOptions.Visible = true;
        litTitle.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("_Stats", "ContentText_MonthlyTotals");
        updMain.Update();
    }

    protected void lnkBackChart_Click(object sender, System.EventArgs e)
    {
        mvwYearSummaryChart.SetActiveView(viwYearSummaryListChart);
        phdOptions.Visible = true;
        litTitle.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("_Stats", "ContentText_MonthlyTotals");
        updMain.Update();
    }

    protected void gvwYearSummary_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName == "DateSelected")
        {
            gvwYearSummary.SelectedIndex = System.Convert.ToInt32(e.CommandArgument);
            string[] strKey = Split(gvwYearSummary.SelectedValue, "_");
            byte numMonth = System.Convert.ToByte(strKey[0]); // ' To Get the selected month
            short numYear = System.Convert.ToInt16(strKey[1]); // ' To Get the selected year

            ShowProductList(numMonth, numYear);
        }
    }

    protected void gvwProductList_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvwProductList.PageIndex = e.NewPageIndex;
        string[] strKey = Split(gvwYearSummary.SelectedValue, "_");
        byte numMonth = System.Convert.ToByte(strKey[0]); // ' To Get the selected month
        short numYear = System.Convert.ToInt16(strKey[1]); // ' To Get the selected year

        ShowProductList(numMonth, numYear);
    }

    private void GetProductYearSummary()
    {
        DataTable tblProductSummary = new DataTable();
        tblProductSummary = StatisticsBLL._GetProductYearSummary();

        byte numReturnedMonths = tblProductSummary.Rows.Count;

        DataTable tblYearSummary = new DataTable();
        tblYearSummary.Columns.Add(new DataColumn("MonthYear", Type.GetType("System.String")));
        tblYearSummary.Columns.Add(new DataColumn("NoOfHits", Type.GetType("System.Int64")));
        tblYearSummary.Columns.Add(new DataColumn("TheMonth", Type.GetType("System.Byte")));
        tblYearSummary.Columns.Add(new DataColumn("TheYear", Type.GetType("System.Int16")));
        tblYearSummary.Columns.Add(new DataColumn("TheDate", Type.GetType("System.String")));
        tblYearSummary.Columns.Add(new DataColumn("TopHits", Type.GetType("System.Int64")));

        byte numCurrentMonth = DateTime.Now.Month;
        short numCurrentYear = DateTime.Now.Year;
        bool blnEndOfYear = false;

        int numHits = 0;
        byte numMonth = DateTime.Now.Month;
        int numYear = numCurrentYear;
        int numTopHits = 0;
        int numCounter = 0;

        // Find highest number of hits to use for scale
        while (!numCounter == ddlDuration.SelectedValue)
        {
            if (numMonth == 0)
            {
                numMonth = 12;
                numYear -= 1;
            }

            // Note: we create a unique number for year month, which is the year x 100, plus the month
            // This makes it easier to compare and select using this
            DataRow[] rowProducts = tblProductSummary.Select("MonthYear=" + (numMonth + (numYear * 100)));

            if (rowProducts.Length > 0)
                numHits = rowProducts[0]("NoOfHits");
            if (numHits > numTopHits)
                numTopHits = numHits;
            numHits = 0;
            numMonth -= 1;
            numCounter += 1;
        }

        // reset counters
        numHits = 0;
        numMonth = DateTime.Now.Month;
        numYear = numCurrentYear;
        numCounter = 0;

        // loop to fill table, using scale value obtained above
        while (!numCounter == ddlDuration.SelectedValue)
        {
            if (numMonth == 0)
            {
                numMonth = 12;
                numYear -= 1;
            }

            // Note: we create a unique number for year month, which is the year x 100, plus the month
            // This makes it easier to compare and select using this
            DataRow[] rowProducts = tblProductSummary.Select("MonthYear=" + (numMonth + (numYear * 100)));

            if (rowProducts.Length > 0)
                numHits = rowProducts[0]("NoOfHits");
            // If numTopHits = 0 Then numTopHits = numHits

            tblYearSummary.Rows.Add(numMonth + "_" + numYear, numHits, numMonth, numYear, DateTime.MonthName(numMonth) + ", " + numYear, numTopHits);

            numHits = 0;
            numMonth -= 1;
            numCounter += 1;
        }

        litTitle.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("_Stats", "ContentText_MonthlyTotals");
        phdOptions.Visible = true;

        DataView dvwYearSummary = tblYearSummary.DefaultView;

        if (ddlDisplayType.SelectedValue == "table")
        {
            dvwYearSummary.Sort = "TheYear DESC, TheMonth DESC";
            gvwYearSummary.DataSource = dvwYearSummary;
            gvwYearSummary.DataBind();
            mvwDisplayType.SetActiveView(viwDisplayTable);
        }
        else
        {
            dvwYearSummary.Sort = "TheYear, TheMonth";
            {
                var withBlock = _UC_KartChartProductYearSummary;
                withBlock.YTitle = System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_StoreHits");
                withBlock.XDataField = "TheDate";
                withBlock.YDataField = "NoOfHits";
                withBlock.PostBackField = "TheDate";
                withBlock.ToolTipField = "TheDate";
                withBlock.DataSource = dvwYearSummary;
                withBlock.DrawChart();
            }
            mvwDisplayType.SetActiveView(viwDisplayChart);
        }
        updMain.Update();
    }

    public void ShowProductList(byte numMonth, short numYear)
    {
        int numCounter = 0;

        DataTable tblMonthProductList = new DataTable();
        tblMonthProductList = StatisticsBLL._GetProductsByDate(numMonth, numYear, System.Web.UI.UserControl.Session["_LANG"]);

        tblMonthProductList.Columns.Add(new DataColumn("TopHits", Type.GetType("System.Int64")));

        if (tblMonthProductList.Rows.Count == 0)
            phdNoResults.Visible = true;
        else
        {
            phdNoResults.Visible = false;

            while (!numCounter == tblMonthProductList.Rows.Count)
            {
                try
                {
                    tblMonthProductList.Rows.Item(numCounter)("TopHits") = tblMonthProductList.Rows.Item(0)("NoOfHits");
                }
                catch (Exception ex)
                {
                }
                numCounter = numCounter + 1;
            }
        }

        litTitle.Text = DateTime.MonthName(numMonth) + ", " + numYear;
        phdOptions.Visible = false;

        DataView dvwProductList = tblMonthProductList.DefaultView;
        dvwProductList.Sort = "NoOfHits DESC";

        if (ddlDisplayType.SelectedValue == "table")
        {
            gvwProductList.DataSource = dvwProductList;
            gvwProductList.DataBind();
            mvwYearSummaryTable.SetActiveView(viwProductListTable);
        }
        else
        {
            {
                var withBlock = _UC_KartChartProductMonthDetails;
                withBlock.CopyDesign(_UC_KartChartProductYearSummary);
                withBlock.YTitle = System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_StoreHits");
                withBlock.XDataField = "ProductName";
                withBlock.YDataField = "NoOfHits";
                withBlock.ToolTipField = "ProductName";
                withBlock.ShowOptions = false;
                withBlock.DataSource = dvwProductList;
                withBlock.DrawChart();
            }
            mvwYearSummaryChart.SetActiveView(viwProductListChart);
        }

        updMain.Update();
    }

    protected void gvwProductList_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "DetailsSelected":
                {
                    gvwProductList.SelectedIndex = System.Convert.ToInt32(e.CommandArgument) - (gvwProductList.PageSize * gvwProductList.PageIndex);

                    int numProductID;
                    numProductID = System.Convert.ToInt32((Literal)gvwProductList.SelectedRow.Cells(1).FindControl("litProductID").Text);

                    string[] strKey = Split(gvwYearSummary.SelectedValue, "_");
                    byte numMonth = System.Convert.ToByte(strKey[0]); // ' To Get the selected month
                    short numYear = System.Convert.ToInt16(strKey[1]); // ' To Get the selected year

                    DataTable tblProductStatsDetails = new DataTable();
                    tblProductStatsDetails = StatisticsBLL._GetProductStatsDetailsByDate(numProductID, numMonth, numYear, System.Web.UI.UserControl.Session["_LANG"]);
                    (GridView)gvwProductList.SelectedRow.Cells(1).FindControl("grdStatsDetails").DataSource = tblProductStatsDetails;

                    int numCounter = 0;
                    tblProductStatsDetails.Columns.Add(new DataColumn("TopHits", Type.GetType("System.Int64")));

                    if (tblProductStatsDetails.Rows.Count == 0)
                    {
                    }
                    else
                        while (!numCounter == tblProductStatsDetails.Rows.Count)
                        {
                            try
                            {
                                tblProductStatsDetails.Rows.Item(numCounter)("TopHits") = tblProductStatsDetails.Rows.Item(0)("NoOfHits");
                            }
                            catch (Exception ex)
                            {
                            }
                            numCounter = numCounter + 1;
                        }

                    (GridView)gvwProductList.SelectedRow.Cells(1).FindControl("grdStatsDetails").DataBind();
                    HideDetails();
                    (LinkButton)gvwProductList.SelectedRow.Cells(1).FindControl("lnkDetails").Visible = false;
                    (Panel)gvwProductList.SelectedRow.Cells(2).FindControl("pnlProductDetails").Visible = true;
                    break;
                }

            case "HideDetails":
                {
                    (LinkButton)gvwProductList.SelectedRow.Cells(1).FindControl("lnkDetails").Visible = true;
                    (Panel)gvwProductList.SelectedRow.Cells(2).FindControl("pnlProductDetails").Visible = false;
                    break;
                }
        }
    }

    private void HideDetails()
    {
        foreach (GridViewRow rowProductStats in gvwProductList.Rows)
        {
            if (rowProductStats.RowType == DataControlRowType.DataRow)
            {
                (LinkButton)rowProductStats.FindControl("lnkDetails").Visible = true;
                (Panel)rowProductStats.FindControl("pnlProductDetails").Visible = false;
            }
        }
    }

    protected void _UC_KartChartProductYearSummary_ChartClicked(string value)
    {
        DateTime dat = (DateTime)value;
        ShowProductList(dat.Month, dat.Year);
    }

    protected void _UC_KartChartProductYearSummary_OptionsChanged()
    {
        GetProductYearSummary();
    }

    protected void _UC_KartChartProductMonthDetails_OptionsChanged()
    {
        DateTime dat = (DateTime)_UC_KartChartProductYearSummary.PostBackValue;
        ShowProductList(dat.Month, dat.Year);
    }

    protected void ddlDuration_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        GetProductYearSummary();
    }

    protected void ddlDisplayType_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        GetProductYearSummary();
    }
}
