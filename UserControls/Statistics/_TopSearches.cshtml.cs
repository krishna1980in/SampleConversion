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

partial class UserControls_Statistics_TopSearches : System.Web.UI.UserControl
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
        if (!System.Web.UI.Control.Page.IsPostBack & KartSettingsManager.GetKartConfig("backend.homepage.graphs") != "OFF")
        {
            foreach (ListItem itm in ddlDuration.Items)
                itm.Text += " " + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_Days");
            foreach (ListItem itm in ddlTerms.Items)
                itm.Text += " " + System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_Terms");
            if (blnMiniDisplay)
            {
                ddlDisplayType.SelectedValue = "chart";
                ddlDuration.SelectedValue = "-7";
                ddlTerms.SelectedValue = "5";
            }
            if (blnMiniDisplay)
                phdOptions.Visible = false;
            GetSearchStatistics();
        }
    }

    public DateTime AdjustTime(DateTime datToAdjust, bool blnStartOfDay)
    {
        DateTime datObject;
        if (blnStartOfDay)
        {
            datObject = new DateTime(datToAdjust.Year, datToAdjust.Month, datToAdjust.Day, 0, 0, 0);
            return datObject;
        }
        datObject = new DateTime(datToAdjust.Year, datToAdjust.Month, datToAdjust.Day, 23, 59, 59);
        return datObject;
    }

    public void GetSearchStatistics()
    {
        DateTime datFrom = AdjustTime(NowOffset.AddDays(ddlDuration.SelectedValue), true);
        DateTime datTo = AdjustTime(NowOffset, false);

        using (DataTable tblTopSearches = StatisticsBLL._GetTopSearches(datFrom, datTo, ddlTerms.SelectedValue))
        {
            if (tblTopSearches.Rows.Count > 0)
            {
                DataView dvwTopSearches = tblTopSearches.DefaultView;

                if (ddlDisplayType.SelectedValue == "table")
                {
                    dvwTopSearches.Sort = "TotalSearches DESC";
                    gvwSearch.DataSource = dvwTopSearches;
                    gvwSearch.DataBind();
                    mvwSearchStats.SetActiveView(viwSearchTable);
                }
                else
                {
                    dvwTopSearches.Sort = "TotalSearches";
                    {
                        var withBlock = _UC_KartChartSearch;
                        withBlock.YTitle = System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_TotalSearches");
                        withBlock.XDataField = "SS_Keyword";
                        withBlock.YDataField = "TotalSearches";
                        withBlock.ToolTipField = "SS_Keyword";
                        withBlock.DataSource = dvwTopSearches;
                        if (blnMiniDisplay)
                        {
                            withBlock.ShowOptions = false;
                            withBlock.DynamicSize = false;
                            withBlock.SetHeight = 140;
                            withBlock.SetWidth = 250;
                        }
                        withBlock.DrawChart();
                    }
                    updSearchChart.Update();
                    mvwSearchStats.SetActiveView(viwSearchChart);
                }
            }
            else
                mvwSearchStats.SetActiveView(viwNoData);

            updSearch.Update();
        }
    }

    protected void _UC_KartChartSearch_OptionsChanged()
    {
        GetSearchStatistics();
    }

    protected void ddlDisplayType_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        GetSearchStatistics();
    }

    protected void ddlDuration_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        GetSearchStatistics();
    }

    protected void ddlTerms_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        GetSearchStatistics();
    }
}
