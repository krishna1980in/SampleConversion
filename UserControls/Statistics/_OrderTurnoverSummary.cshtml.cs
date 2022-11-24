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

partial class UserControls_Back_OrderTurnoverSummary : System.Web.UI.UserControl
{
    private bool blnMiniDisplay;
    public bool IsMiniDisplay
    {
        set
        {
            blnMiniDisplay = value;
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!System.Web.UI.Control.Page.IsPostBack)
        {
            if (blnMiniDisplay)
                phdOptions.Visible = false;
            GetOrdersTurnover();
        }
    }

    public DateTime AdjustTime(DateTime datToAdjust, bool blnStartOfDay)
    {
        DateTime datObject;
        if (blnStartOfDay)
        {
            datObject = new DateTime(datToAdjust.Year, datToAdjust.Month, datToAdjust.Day, 12, 0, 0);
            return datObject;
        }
        datObject = new DateTime(datToAdjust.Year, datToAdjust.Month, datToAdjust.Day, 23, 59, 59);
        return datObject;
    }

    public void GetOrdersTurnover()
    {
        DateTime datFrom = AdjustTime(NowOffset.AddDays(ddlDuration.SelectedValue), true);
        DateTime datTo = AdjustTime(NowOffset, false);

        using (DataTable tblOrderTurnover = StatisticsBLL._GetOrdersTurnover(datFrom, datTo))
        {
            if (ddlDisplayType.SelectedValue == "table")
            {
                tblOrderTurnover.Columns.Add(new DataColumn("Date", Type.GetType("System.String")));
                tblOrderTurnover.Columns.Add(new DataColumn("Turnover", Type.GetType("System.String")));
                tblOrderTurnover.Columns.Add(new DataColumn("TurnoverOrders", Type.GetType("System.String")));
                foreach (DataRow drwOrderTurnover in tblOrderTurnover.Rows)
                {
                    drwOrderTurnover("Date") = FixNullFromDB(drwOrderTurnover("Day")) + ", " + MonthName(FixNullFromDB(drwOrderTurnover("Month")), true); // & "/" & FixNullFromDB(row("Year"))
                    drwOrderTurnover("Date") += " " + FixNullFromDB(drwOrderTurnover("Year"));
                    drwOrderTurnover("Turnover") = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency, FixNullFromDB(drwOrderTurnover("TotalValue")));
                    drwOrderTurnover("TurnoverOrders") = CurrenciesBLL.CurrencySymbol(CurrenciesBLL.GetDefaultCurrency) + drwOrderTurnover("Turnover") + " (" + drwOrderTurnover("Orders") + " " + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_Orders") + ")";
                }
            }
            else
            {
                tblOrderTurnover.Columns.Add(new DataColumn("Date", Type.GetType("System.String")));
                tblOrderTurnover.Columns.Add(new DataColumn("Turnover", Type.GetType("System.Single")));
                tblOrderTurnover.Columns.Add(new DataColumn("TurnoverOrders", Type.GetType("System.String")));
                foreach (DataRow drwOrderTurnover in tblOrderTurnover.Rows)
                {
                    drwOrderTurnover("Date") = FixNullFromDB(drwOrderTurnover("Day")) + ", " + MonthName(FixNullFromDB(drwOrderTurnover("Month")), true); // & "/" & FixNullFromDB(row("Year"))
                    drwOrderTurnover("Date") += " " + FixNullFromDB(drwOrderTurnover("Year"));
                    drwOrderTurnover("Turnover") = CurrenciesBLL.ConvertCurrency(CurrenciesBLL.GetDefaultCurrency, FixNullFromDB(drwOrderTurnover("TotalValue")));
                    drwOrderTurnover("TurnoverOrders") = CurrenciesBLL.CurrencySymbol(CurrenciesBLL.GetDefaultCurrency) + drwOrderTurnover("Turnover") + " (" + drwOrderTurnover("Orders") + " " + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_Orders") + ")";
                }
            }

            if (tblOrderTurnover.Rows.Count > 0)
            {
                DataView dvwOrderTurnover = tblOrderTurnover.DefaultView;
                if (ddlDisplayType.SelectedValue == "table")
                {
                    dvwOrderTurnover.Sort = "Year DESC, Month DESC, Day DESC";
                    gvwOrdersTurnover.DataSource = dvwOrderTurnover;
                    gvwOrdersTurnover.DataBind();
                    mvwOrderStats.SetActiveView(viwTurnoverTable);
                }
                else
                {
                    dvwOrderTurnover.Sort = "Year, Month, Day";
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
                        var withBlock = _UC_KartChartOrderTurnover;
                        withBlock.YTitle = System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_TotalTurnover") + " (" + CurrenciesBLL.CurrencySymbol(CurrenciesBLL.GetDefaultCurrency) + ")";
                        withBlock.XDataField = "Date";
                        withBlock.YDataField = "Turnover";
                        withBlock.YDataFormat = strDataFormat;
                        withBlock.ToolTipField = "TurnoverOrders";
                        withBlock.DataSource = dvwOrderTurnover;
                        if (blnMiniDisplay)
                        {
                            withBlock.ShowOptions = false;
                            withBlock.DynamicSize = false;
                            withBlock.SetHeight = 140;
                            withBlock.SetWidth = 250;
                        }
                        withBlock.DrawChart();
                    }
                    updTurnoverChart.Update();
                    mvwOrderStats.SetActiveView(viwTurnoverChart);
                }
            }
            else
                mvwOrderStats.SetActiveView(viwNoData);
            updTurnover.Update();
        }
    }

    protected void _UC_KartChartOrderTurnover_OptionsChanged()
    {
        GetOrdersTurnover();
    }

    protected void ddlDisplayType_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        GetOrdersTurnover();
    }

    protected void ddlDuration_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        GetOrdersTurnover();
    }
}
