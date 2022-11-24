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

partial class UserControls_Statistics_AverageHits : System.Web.UI.UserControl
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
            LoadAverageVisits();
    }

    public void LoadAverageVisits()
    {
        using (DataTable tblDummy = StatisticsBLL._GetAverageVisits())
        {
            using (DataTable tblHits = new DataTable())
            {
                tblHits.Columns.Add(new DataColumn("Period", Type.GetType("System.String")));
                tblHits.Columns.Add(new DataColumn("Hits", Type.GetType("System.Int32")));
                tblHits.Columns.Add(new DataColumn("SortValue", Type.GetType("System.Int32")));

                tblHits.Rows.Add(System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_Last24Hours"), tblDummy.Rows(0)("Last24Hours"), 1);
                tblHits.Rows.Add(System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_Last7Days"), tblDummy.Rows(0)("LastWeek"), 2);
                tblHits.Rows.Add(System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_LastMonth"), tblDummy.Rows(0)("LastMonth"), 3);
                tblHits.Rows.Add(System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_LastYear"), tblDummy.Rows(0)("LastYear"), 4);

                if (tblHits.Rows.Count > 0)
                {
                    DataView dvwHits = tblHits.DefaultView;
                    if (ddlDisplayType.SelectedValue == "table")
                    {
                        dvwHits.Sort = "SortValue";

                        gvwAverageHits.DataSource = dvwHits;
                        gvwAverageHits.DataBind();
                        mvwHits.SetActiveView(viwAverageHitsTable);
                    }
                    else
                    {
                        dvwHits.Sort = "SortValue DESC";
                        {
                            var withBlock = _UC_KartChartAverageHits;
                            withBlock.YTitle = System.Web.UI.TemplateControl.GetGlobalResourceObject("_Statistics", "ContentText_StoreHits");
                            withBlock.XDataField = "Period";
                            withBlock.YDataField = "Hits";
                            withBlock.ToolTipField = "Period";
                            if (blnMiniDisplay)
                            {
                                withBlock.ShowOptions = false;
                                withBlock.DynamicSize = false;
                                withBlock.SetHeight = 140;
                                withBlock.SetWidth = 250;
                            }
                            withBlock.DataSource = dvwHits;
                            withBlock.DrawChart();
                        }
                        updAverageHits.Update();
                        mvwHits.SetActiveView(viwAverageHitsChart);
                    }
                }
                else
                    mvwHits.SetActiveView(viwNoData);
                updAverageVisits.Update();
            }
        }
    }

    protected void ddlDisplayType_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        LoadAverageVisits();
    }
}
