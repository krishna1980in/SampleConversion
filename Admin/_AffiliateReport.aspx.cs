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

partial class _AffiliateReport : _PageBaseClass
{
    private int numTotalMonthlyHits = 0;
    private double numTotalMonthlySales = 0;
    private double numTotalAnnualSales = 0;
    protected static int Report_Month, Report_Year;

    private class AffiliateReport
    {
        private double _AffValue;
        private int _AffMonth;
        private int _AffYear;
        private int _GraphValue;

        public double AffValue
        {
            get
            {
                return _AffValue;
            }
            set
            {
                _AffValue = value;
            }
        }

        public int AffMonth
        {
            get
            {
                return _AffMonth;
            }
            set
            {
                _AffMonth = value;
            }
        }

        public int AffYear
        {
            get
            {
                return _AffYear;
            }
            set
            {
                _AffYear = value;
            }
        }

        public int GraphValue
        {
            get
            {
                return _GraphValue;
            }
            set
            {
                _GraphValue = value;
            }
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Kartris", "PageTitle_AffiliateStats") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        if (!Page.IsPostBack)
        {
            DisplayHitsReport();
            DisplaySalesReport();
        }
        else
            (Skins_Admin_Template)this.Master.LoadCategoryMenu();
    }

    private void DisplayHitsReport()
    {
        ArrayList aryAffiliateHits = new ArrayList();
        System.Data.DataTable tblAffiliateHits = new System.Data.DataTable();
        kartris.Basket objBasket = new kartris.Basket();
        DateTime datReport = CkartrisDisplayFunctions.NowOffset;
        int numMaxValue, numHitsTotal;
        int numMonth = Month(CkartrisDisplayFunctions.NowOffset);
        int numYear = Year(CkartrisDisplayFunctions.NowOffset);

        if (Request.QueryString("Report_Month") != "")
        {
            try
            {
                numMonth = Request.QueryString("Report_Month");
            }
            catch (Exception ex)
            {
            }
        }
        if (Request.QueryString("Report_Year") != "")
        {
            try
            {
                numYear = Request.QueryString("Report_Year");
            }
            catch (Exception ex)
            {
            }
        }

        litHitsReportDate.Text = DateTime.MonthName(numMonth) + " " + numYear;

        Report_Month = numMonth;
        Report_Year = numYear;

        // '// get monthly hits
        numTotalMonthlyHits = 0;
        tblAffiliateHits = AffiliateBLL._GetAffiliateMonthlyHitsReport(numMonth, numYear);
        rptMonthlyHits.DataSource = tblAffiliateHits;
        rptMonthlyHits.DataBind();
        litTotalMonthlyHits.Text = numTotalMonthlyHits;

        // '// get annual hits
        aryAffiliateHits.Clear();
        for (int i = 1; i <= 12; i++)
        {
            AffiliateReport objAffRep = new AffiliateReport();
            objAffRep.AffMonth = DateTime.Month(datReport);
            objAffRep.AffYear = DateTime.Year(datReport);
            objAffRep.AffValue = 0;
            objAffRep.GraphValue = 1;
            aryAffiliateHits.Add(objAffRep);
            datReport = DateTime.DateAdd(DateInterval.Month, -1, datReport);
        }

        numMaxValue = 0;
        tblAffiliateHits = AffiliateBLL._GetAffiliateAnnualHitsReport;
        for (int i = 1; i <= tblAffiliateHits.Rows.Count; i++)
        {
            foreach (AffiliateReport itmAffiliateMonth in aryAffiliateHits)
            {
                if (tblAffiliateHits.Rows[i - 1].Item["TheMonth"] == itmAffiliateMonth.AffMonth && tblAffiliateHits.Rows[i - 1].Item["TheYear"] == itmAffiliateMonth.AffYear)
                {
                    itmAffiliateMonth.AffValue = tblAffiliateHits.Rows[i - 1].Item["Hits"];
                    if (itmAffiliateMonth.AffValue > numMaxValue)
                        numMaxValue = itmAffiliateMonth.AffValue;
                }
            }
        }

        numHitsTotal = 0;
        foreach (AffiliateReport itmAffiliateMonth in aryAffiliateHits)
        {
            if (numMaxValue > 0)
                itmAffiliateMonth.GraphValue = (itmAffiliateMonth.AffValue / numMaxValue) * 100;
            numHitsTotal = numHitsTotal + itmAffiliateMonth.AffValue;
        }

        rptAnnualHits.DataSource = aryAffiliateHits;
        rptAnnualHits.DataBind();

        tblAffiliateHits.Dispose();
        objBasket = null/* TODO Change to default(_) if this is not a reference type */;
    }

    private void DisplaySalesReport()
    {
        ArrayList aryAffiliateSales = new ArrayList();
        System.Data.DataTable tblAffiliateSales = new System.Data.DataTable();
        kartris.Basket objBasket = new kartris.Basket();
        DateTime datReport = CkartrisDisplayFunctions.NowOffset;
        int numMaxValue, numSalesTotal;
        int numMonth = Month(CkartrisDisplayFunctions.NowOffset);
        int numYear = Year(CkartrisDisplayFunctions.NowOffset);

        if (Request.QueryString("Report_Month") != "")
        {
            try
            {
                numMonth = Request.QueryString("Report_Month");
            }
            catch (Exception ex)
            {
            }
        }
        if (Request.QueryString("Report_Year") != "")
        {
            try
            {
                numYear = Request.QueryString("Report_Year");
            }
            catch (Exception ex)
            {
            }
        }

        litSalesReportDate.Text = MonthName(Month(CkartrisDisplayFunctions.NowOffset)) + " " + Year(CkartrisDisplayFunctions.NowOffset);

        Report_Month = numMonth;
        Report_Year = numYear;

        numTotalAnnualSales = 0;

        // '// get monthly sales
        numTotalMonthlySales = 0;
        tblAffiliateSales = AffiliateBLL._GetAffiliateMonthlySalesReport(numMonth, numYear);
        rptMonthlySales.DataSource = tblAffiliateSales;
        rptMonthlySales.DataBind();
        litTotalMonthlySales.Text = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, numTotalMonthlySales);

        // '// get annual sales
        aryAffiliateSales.Clear();
        for (int i = 1; i <= 12; i++)
        {
            AffiliateReport objAffRep = new AffiliateReport();
            objAffRep.AffMonth = DateTime.Month(datReport);
            objAffRep.AffYear = DateTime.Year(datReport);
            objAffRep.AffValue = 0;
            objAffRep.GraphValue = 1;
            aryAffiliateSales.Add(objAffRep);
            datReport = DateTime.DateAdd(DateInterval.Month, -1, datReport);
        }

        numMaxValue = 0;
        tblAffiliateSales = AffiliateBLL._GetAffiliateAnnualSalesReport;
        for (int i = 1; i <= tblAffiliateSales.Rows.Count; i++)
        {
            foreach (AffiliateReport itmAffiliateMonth in aryAffiliateSales)
            {
                if (tblAffiliateSales.Rows[i - 1].Item["TheMonth"] == itmAffiliateMonth.AffMonth && tblAffiliateSales.Rows[i - 1].Item["TheYear"] == itmAffiliateMonth.AffYear)
                {
                    itmAffiliateMonth.AffValue = tblAffiliateSales.Rows[i - 1].Item["OrderAmount"];
                    if (itmAffiliateMonth.AffValue > numMaxValue)
                        numMaxValue = itmAffiliateMonth.AffValue;
                }
            }
        }

        numSalesTotal = 0;
        foreach (AffiliateReport itmAffiliateMonth in aryAffiliateSales)
        {
            if (numMaxValue > 0)
                itmAffiliateMonth.GraphValue = (itmAffiliateMonth.AffValue / numMaxValue) * 100;
            numSalesTotal = numSalesTotal + itmAffiliateMonth.AffValue;
        }

        rptAnnualSales.DataSource = aryAffiliateSales;
        rptAnnualSales.DataBind();

        tblAffiliateSales.Dispose();
        objBasket = null/* TODO Change to default(_) if this is not a reference type */;
    }

    protected void rptMonthlyHits_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        if ((e.Item.ItemType == ListItemType.Item) | (e.Item.ItemType == ListItemType.AlternatingItem))
            numTotalMonthlyHits = numTotalMonthlyHits + System.Convert.ToInt32(e.Item.DataItem("Hits"));
    }

    protected void rptAnnualHits_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        double numHits;
        int numMonth, numYear;
        AffiliateReport objAffiliateReport;

        if ((e.Item.ItemType == ListItemType.Item) | (e.Item.ItemType == ListItemType.AlternatingItem))
        {
            objAffiliateReport = (AffiliateReport)e.Item.DataItem;
            numHits = objAffiliateReport.AffValue;
            numMonth = objAffiliateReport.AffMonth;
            numYear = objAffiliateReport.AffYear;
            (Literal)e.Item.FindControl("litHitsValue").Text = numHits;
            (LinkButton)e.Item.FindControl("lnkHitsDate").Text = DateTime.MonthName(numMonth) + " " + numYear;
            (LinkButton)e.Item.FindControl("lnkHitsDate").CommandArgument = numMonth + "," + numYear;
        }
    }

    protected void rptMonthlySales_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        if ((e.Item.ItemType == ListItemType.Item) | (e.Item.ItemType == ListItemType.AlternatingItem))
            numTotalMonthlySales = numTotalMonthlySales + System.Convert.ToDouble(e.Item.DataItem("OrderAmount"));
    }

    protected void rptAnnualSales_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        double numSales;
        int numMonth, numYear;
        AffiliateReport objAffiliateReport;

        if ((e.Item.ItemType == ListItemType.Item) | (e.Item.ItemType == ListItemType.AlternatingItem))
        {
            objAffiliateReport = (AffiliateReport)e.Item.DataItem;
            numSales = objAffiliateReport.AffValue;
            numMonth = objAffiliateReport.AffMonth;
            numYear = objAffiliateReport.AffYear;
            (Literal)e.Item.FindControl("litSalesValue").Text = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, numSales);
            (LinkButton)e.Item.FindControl("lnkSalesDate").Text = DateTime.MonthName(numMonth) + " " + numYear;
            (LinkButton)e.Item.FindControl("lnkSalesDate").CommandArgument = numMonth + "," + numYear;
            numTotalAnnualSales = numTotalAnnualSales + numSales;
        }
    }

    protected void HitsDate_Click(object sender, CommandEventArgs e)
    {
        string strDate = e.CommandArgument;
        string[] aryDates = strDate.Split(",");
        int numMonth = System.Convert.ToInt32(aryDates[0]);
        int numYear = System.Convert.ToInt32(aryDates[1]);

        litHitsReportDate.Text = DateTime.MonthName(numMonth) + " " + numYear;

        Report_Month = numMonth;
        Report_Year = numYear;

        System.Data.DataTable tblAffiliateHits = new System.Data.DataTable();
        kartris.Basket objBasket = new kartris.Basket();

        tblAffiliateHits = AffiliateBLL._GetAffiliateMonthlyHitsReport(numMonth, numYear);

        numTotalMonthlyHits = 0;
        rptMonthlyHits.DataSource = tblAffiliateHits;
        rptMonthlyHits.DataBind();
        litTotalMonthlyHits.Text = numTotalMonthlyHits;

        tblAffiliateHits.Dispose();
        objBasket = null/* TODO Change to default(_) if this is not a reference type */;
    }

    protected void SalesDate_Click(object sender, CommandEventArgs e)
    {
        int numMonth, numYear;
        string strDate = e.CommandArgument;

        string[] aryDates = strDate.Split(",");
        numMonth = System.Convert.ToInt32(aryDates[0]);
        numYear = System.Convert.ToInt32(aryDates[1]);

        litSalesReportDate.Text = DateTime.MonthName(numMonth) + " " + numYear;

        System.Data.DataTable tblAffiliateSales = new System.Data.DataTable();
        kartris.Basket objBasket = new kartris.Basket();

        tblAffiliateSales = AffiliateBLL._GetAffiliateMonthlySalesReport(numMonth, numYear);

        numTotalMonthlySales = 0;
        rptMonthlySales.DataSource = tblAffiliateSales;
        rptMonthlySales.DataBind();
        litTotalMonthlySales.Text = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, numTotalMonthlySales);

        tblAffiliateSales.Dispose();
        objBasket = null/* TODO Change to default(_) if this is not a reference type */;
    }
}
