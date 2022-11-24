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

partial class UserControls_Statistics_CategoryStats : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!System.Web.UI.Control.Page.IsPostBack)
        {
            foreach (ListItem itm in ddlDuration.Items)
                itm.Text += " " + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_Months");
            GetCategoryYearSummary();
        }
    }

    protected void gvwYearSummary_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName == "DateSelected")
        {
            gvwYearSummary.SelectedIndex = System.Convert.ToInt32(e.CommandArgument);
            string[] strKey = Split(gvwYearSummary.SelectedValue, "_");
            byte numMonth = System.Convert.ToByte(strKey[0]); // ' To Get the selected month
            short numYear = System.Convert.ToInt16(strKey[1]); // ' To Get the selected year

            ShowCategoryList(numMonth, numYear);
        }
    }

    protected void gvwCategoryList_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvwCategoryList.PageIndex = e.NewPageIndex;
        string[] strKey = Split(gvwYearSummary.SelectedValue, "_");
        byte numMonth = System.Convert.ToByte(strKey[0]); // ' To Get the selected month
        short numYear = System.Convert.ToInt16(strKey[1]); // ' To Get the selected year

        ShowCategoryList(numMonth, numYear);
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

    private void GetCategoryYearSummary()
    {
        DataTable tblCategorySummary = new DataTable();
        tblCategorySummary = StatisticsBLL._GetCategoryYearSummary();

        byte numReturnedMonths = tblCategorySummary.Rows.Count;

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
            DataRow[] rowCategories = tblCategorySummary.Select("MonthYear=" + (numMonth + (numYear * 100)));

            if (rowCategories.Length > 0)
                numHits = rowCategories[0]("NoOfHits");
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
            DataRow[] rowCategories = tblCategorySummary.Select("MonthYear=" + (numMonth + (numYear * 100)));

            if (rowCategories.Length > 0)
                numHits = rowCategories[0]("NoOfHits");
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
                var withBlock = _UC_KartChartCategoryYearSummary;
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

    public void ShowCategoryList(byte numMonth, short numYear)
    {
        int numCounter = 0;

        DataTable tblMonthCategoryList = new DataTable();
        tblMonthCategoryList = StatisticsBLL._GetCategoriesByDate(numMonth, numYear, System.Web.UI.UserControl.Session["_LANG"]);

        tblMonthCategoryList.Columns.Add(new DataColumn("TopHits", Type.GetType("System.Int64")));

        if (tblMonthCategoryList.Rows.Count == 0)
            phdNoResults.Visible = true;
        else
        {
            phdNoResults.Visible = false;

            while (!numCounter == tblMonthCategoryList.Rows.Count)
            {
                try
                {
                    tblMonthCategoryList.Rows.Item(numCounter)("TopHits") = tblMonthCategoryList.Rows.Item(0)("NoOfHits");
                }
                catch (Exception ex)
                {
                }
                numCounter = numCounter + 1;
            }
        }

        litTitle.Text = DateTime.MonthName(numMonth) + ", " + numYear;
        phdOptions.Visible = false;

        DataView dvwCategoryList = tblMonthCategoryList.DefaultView;
        dvwCategoryList.Sort = "NoOfHits DESC";

        if (ddlDisplayType.SelectedValue == "table")
        {
            gvwCategoryList.DataSource = dvwCategoryList;
            gvwCategoryList.DataBind();
            mvwYearSummaryTable.SetActiveView(viwCategoryListTable);
        }
        else
        {
            {
                var withBlock = _UC_KartChartCategoryMonthDetails;
                withBlock.CopyDesign(_UC_KartChartCategoryYearSummary);
                withBlock.YTitle = System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_StoreHits");
                withBlock.XDataField = "CategoryName";
                withBlock.YDataField = "NoOfHits";
                withBlock.ToolTipField = "CategoryName";
                withBlock.ShowOptions = false;
                withBlock.DataSource = dvwCategoryList;
                withBlock.DrawChart();
            }
            mvwYearSummaryChart.SetActiveView(viwCategoryListChart);
        }

        updMain.Update();
    }

    protected void _UC_KartChartCategoryYearSummary_ChartClicked(string value)
    {
        DateTime dat = (DateTime)value;
        ShowCategoryList(dat.Month, dat.Year);
    }

    protected void _UC_KartChartCategoryYearSummary_OptionsChanged()
    {
        GetCategoryYearSummary();
    }

    protected void _UC_KartChartCategoryMonthDetails_OptionsChanged()
    {
        DateTime dat = (DateTime)_UC_KartChartCategoryYearSummary.PostBackValue;
        ShowCategoryList(dat.Month, dat.Year);
    }

    protected void ddlDuration_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        GetCategoryYearSummary();
    }

    protected void ddlDisplayType_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        GetCategoryYearSummary();
    }
}
