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
using CkartrisDisplayFunctions;
using CkartrisDataManipulation;

partial class UserControls_Statistics_AverageOrders : System.Web.UI.UserControl
{
    private bool blnMiniDisplay = true;

    public bool IsMiniDisplay
    {
        set
        {
            blnMiniDisplay = value;
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!System.Web.UI.Control.Page.IsPostBack & KartSettingsManager.GetKartConfig("backend.homepage.graphs") != "OFF")
            LoadAverageOrders();
    }

    public void LoadAverageOrders()
    {
        try
        {
            DataTable tblDummy = StatisticsBLL._GetAverageOrders();
            DataRow drwDummy = tblDummy.Rows(0);
            string strTurnover = string.Empty;
            int numOrders = 0;
            DataTable tblOrders = new DataTable();

            if (ddlDisplayType.SelectedValue == "table")
            {
                tblOrders.Columns.Add(new DataColumn("Period", Type.GetType("System.String")));
                tblOrders.Columns.Add(new DataColumn("Orders", Type.GetType("System.Int32")));
                tblOrders.Columns.Add(new DataColumn("Turnover", Type.GetType("System.String")));
                tblOrders.Columns.Add(new DataColumn("TurnoverOrders", Type.GetType("System.String")));
                tblOrders.Columns.Add(new DataColumn("SortValue", Type.GetType("System.Int32")));

                strTurnover = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, FixNullFromDB(drwDummy("Last24HoursTurnover")));
                numOrders = FixNullFromDB(drwDummy("Last24HoursOrders"));
                tblOrders.Rows.Add(System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_Last24Hours"), numOrders, strTurnover, strTurnover + " (" + numOrders + " " + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_Orders") + ")", 1);

                strTurnover = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, FixNullFromDB(drwDummy("LastWeekTurnover")));
                numOrders = FixNullFromDB(drwDummy("LastWeekOrders"));
                tblOrders.Rows.Add(System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_Last7Days"), numOrders, strTurnover, strTurnover + " (" + numOrders + " " + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_Orders") + ")", 2);

                strTurnover = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, FixNullFromDB(drwDummy("LastMonthTurnover")));
                numOrders = FixNullFromDB(drwDummy("LastMonthOrders"));
                tblOrders.Rows.Add(System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_LastMonth"), numOrders, strTurnover, strTurnover + " (" + numOrders + " " + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_Orders") + ")", 3);

                strTurnover = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, FixNullFromDB(drwDummy("LastYearTurnover")));
                numOrders = FixNullFromDB(drwDummy("LastYearOrders"));
                tblOrders.Rows.Add(System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_LastYear"), numOrders, strTurnover, strTurnover + " (" + numOrders + " " + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_Orders") + ")", 4);
            }
            else
            {
                tblOrders.Columns.Add(new DataColumn("Period", Type.GetType("System.String")));
                tblOrders.Columns.Add(new DataColumn("Orders", Type.GetType("System.Int32")));
                tblOrders.Columns.Add(new DataColumn("Turnover", Type.GetType("System.Single")));
                tblOrders.Columns.Add(new DataColumn("TurnoverOrders", Type.GetType("System.String")));
                tblOrders.Columns.Add(new DataColumn("SortValue", Type.GetType("System.Int32")));

                strTurnover = CurrenciesBLL.ConvertCurrency(CurrenciesBLL.GetDefaultCurrency, FixNullFromDB(drwDummy("Last24HoursTurnover")));
                numOrders = FixNullFromDB(drwDummy("Last24HoursOrders"));
                tblOrders.Rows.Add(System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_Last24Hours"), numOrders, strTurnover, strTurnover + " (" + numOrders + " " + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_Orders") + ")", 1);

                strTurnover = CurrenciesBLL.ConvertCurrency(CurrenciesBLL.GetDefaultCurrency, FixNullFromDB(drwDummy("LastWeekTurnover")));
                numOrders = FixNullFromDB(drwDummy("LastWeekOrders"));
                tblOrders.Rows.Add(System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_Last7Days"), numOrders, strTurnover, strTurnover + " (" + numOrders + " " + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_Orders") + ")", 2);

                strTurnover = CurrenciesBLL.ConvertCurrency(CurrenciesBLL.GetDefaultCurrency, FixNullFromDB(drwDummy("LastMonthTurnover")));
                numOrders = FixNullFromDB(drwDummy("LastMonthOrders"));
                tblOrders.Rows.Add(System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_LastMonth"), numOrders, strTurnover, strTurnover + " (" + numOrders + " " + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_Orders") + ")", 3);

                strTurnover = CurrenciesBLL.ConvertCurrency(CurrenciesBLL.GetDefaultCurrency, FixNullFromDB(drwDummy("LastYearTurnover")));
                numOrders = FixNullFromDB(drwDummy("LastYearOrders"));
                tblOrders.Rows.Add(System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_LastYear"), numOrders, strTurnover, strTurnover + " (" + numOrders + " " + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_Orders") + ")", 4);
            }

            if (tblOrders.Rows.Count > 1)
            {
                DataView dvwOrders = tblOrders.DefaultView;
                if (ddlDisplayType.SelectedValue == "table")
                {
                    dvwOrders.Sort = "SortValue";
                    gvwAverageOrders.DataSource = dvwOrders;
                    gvwAverageOrders.DataBind();
                    mvwOrders.SetActiveView(viwAverageOrdersTable);
                }
                else
                {
                    dvwOrders.Sort = "SortValue DESC";
                    DataRow[] drwCurrency = CurrenciesBLL._GetByCurrencyID(CurrenciesBLL.GetDefaultCurrency);
                    int numRounds = FixNullFromDB(drwCurrency[0]("CUR_RoundNumbers"));
                    string strDataFormat = CurrenciesBLL.CurrencySymbol(CurrenciesBLL.GetDefaultCurrency) + "0";
                    if (numRounds > 0)
                    {
                        strDataFormat += ".";
                        for (int i = 0; i <= numRounds - 1; i++)
                            strDataFormat += "0";
                    }
                    {
                        var withBlock = _UC_KartChartAverageOrders;
                        withBlock.YTitle = System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_Turnover") + "(" + CurrenciesBLL.CurrencySymbol(CurrenciesBLL.GetDefaultCurrency) + ")";
                        withBlock.XDataField = "Period";
                        withBlock.YDataField = "Turnover";
                        withBlock.YDataFormat = strDataFormat;
                        withBlock.ToolTipField = "TurnoverOrders";
                        if (blnMiniDisplay)
                        {
                            withBlock.ShowOptions = false;
                            withBlock.DynamicSize = false;
                            withBlock.SetHeight = 140;
                            withBlock.SetWidth = 250;
                        }
                        withBlock.DataSource = dvwOrders;
                        withBlock.DrawChart();
                    }
                    updAverageOrdersChart.Update();
                    mvwOrders.SetActiveView(viwAverageOrdersChart);
                }
            }
            else
                mvwOrders.SetActiveView(viwNoData);
            updAverageOrders.Update();
        }
        catch (Exception ex)
        {
            CkartrisFormatErrors.ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
    }

    protected void ddlDisplayType_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        LoadAverageOrders();
    }
}
