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
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;

public enum kartChrtType
{
    Area = 13,
    Bar = 7 // Default
,
    Column = 10,
    Line = 3,
    Pie = 17,
    Doughnut = 18,
    Pyramid = 34,
    StepLine = 5,
    Spline = 4
}

public enum kartChrtStyle
{
    Cube = 0 // Default
,
    Cylinder = 1,
    Emboss = 2,
    LightToDark = 3,
    Wedge = 4
}

partial class UserControls_Back_KartrisChart : System.Web.UI.UserControl
{
    private int numMinHeight = 400;  // ' Min allowed height (used for the dynamic size)
    private int numMinWidth = 500;   // ' Min allowed width (used for the dynamic size)
    private int numMaxHeight = 500;   // ' Max allowed height (used for the dynamic size)
    private int numMaxWidth = 800;   // ' Max allowed width (used for the dynamic size)

    private int numHeight = 30; // ' 30px for each vertical point (used for the dynamic size)

    public int SetHeight
    {
        set
        {
            numHeight = value;
        }
    }

    private int numWidth = 25;  // ' 25px for each horizantal point (used for the dynamic size)
    public int SetWidth
    {
        set
        {
            numWidth = value;
        }
    }

    private bool blnDynamicSize = true;
    public bool DynamicSize
    {
        set
        {
            blnDynamicSize = value;
        }
    }

    private string strTitle = string.Empty;
    public string Title
    {
        set
        {
            strTitle = value;
        }
    }

    private kartChrtType valChartType = kartChrtType.Column;
    public kartChrtType ChartType
    {
        get
        {
            return ddlChartType.SelectedValue;
        }
        set
        {
            valChartType = value;
        }
    }

    private kartChrtStyle valChartStyle = kartChrtStyle.LightToDark;
    public kartChrtStyle ChartStyle
    {
        get
        {
            return ddlChartStyle.SelectedValue;
        }
        set
        {
            valChartStyle = value;
        }
    }

    private string strTooltipField = null;
    public string ToolTipField
    {
        set
        {
            strTooltipField = value;
        }
    }

    private string strPostBackField = null;
    public string PostBackField
    {
        set
        {
            strPostBackField = value;
        }
    }

    private bool blnValueAsLabel = true;
    public bool ValueAsLabel
    {
        get
        {
            return chkShowLabel.Checked;
        }
        set
        {
            blnValueAsLabel = value;
        }
    }

    private bool blnEnable3D = false;
    public bool Enable3DView
    {
        get
        {
            return chkEnable3D.Checked;
        }
        set
        {
            blnEnable3D = value;
        }
    }

    private string strXTitle = string.Empty;
    public string XTitle
    {
        set
        {
            strXTitle = value;
        }
    }

    private string strYTitle = string.Empty;
    public string YTitle
    {
        set
        {
            strYTitle = value;
        }
    }

    private DataView dvwDataSource = null/* TODO Change to default(_) if this is not a reference type */;
    public DataView DataSource
    {
        set
        {
            dvwDataSource = value;
        }
    }

    private string strXDataField = string.Empty;
    public string XDataField
    {
        set
        {
            strXDataField = value;
        }
    }

    private string strYDataField = string.Empty;
    public string YDataField
    {
        set
        {
            strYDataField = value;
        }
    }

    private string strYDataFormat = string.Empty;
    public string YDataFormat
    {
        set
        {
            strYDataFormat = value;
        }
    }

    private string strSelectedPostBackValue = null;
    public string PostBackValue
    {
        get
        {
            return strSelectedPostBackValue;
        }
    }

    private string strLable = string.Empty;
    public string ChartLable
    {
        set
        {
            strLable = value;
        }
    }

    // ' Show/Hide Options' link
    public bool ShowOptions
    {
        set
        {
            btnOptions.Visible = value;
            updOptions.Update();
        }
    }

    public event ChartClickedEventHandler ChartClicked;

    public delegate void ChartClickedEventHandler(string value);

    public event OptionsChangedEventHandler OptionsChanged;

    public delegate void OptionsChangedEventHandler();

    public void DrawChart()
    {


        // ' Add a Title control to the chart
        kartChart.Titles.Clear();
        if (!string.IsNullOrEmpty(strTitle))
            kartChart.Titles.Add(new Title(strTitle));

        // ' Clear existing legends
        kartChart.Legends.Clear();

        // ' Set the chart type
        kartChart.Series(0).ChartType = valChartType;

        // ' Set the chart style
        // kartChart.Series(0)("DrawingStyle") = IIf(valChartStyle = kartChrtStyle.Cube, "Default", valChartStyle.ToString)

        // ' Show/Hide label on each point
        kartChart.Series(0).IsValueShownAsLabel = blnValueAsLabel;

        // ' Enable/Disable 3D view
        kartChart.ChartAreas(0).Area3DStyle.Enable3D = blnEnable3D;

        // ' Set Axies' Titles
        kartChart.ChartAreas(0).AxisX.Title = strXTitle;
        kartChart.ChartAreas(0).AxisY.Title = strYTitle;

        if (valChartType == kartChrtType.Pie && !string.IsNullOrEmpty(strYDataFormat))
            strLable = "#VALX (#VALY{" + strYDataFormat + "})";
        else if (!string.IsNullOrEmpty(strYDataFormat))
        {
            kartChart.ChartAreas(0).AxisY.LabelStyle.Format = strYDataFormat;
            kartChart.Series(0).LabelFormat = strYDataFormat;
            strLable = "#VALY{" + strYDataFormat + "}";
        }

        if (!string.IsNullOrEmpty(strLable))
            kartChart.Series(0).Label = strLable;

        kartChart.Series(0).SmartLabelStyle.Enabled = true;
        kartChart.Series(0).SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
        kartChart.Series(0).SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.Diamond;
        kartChart.Series(0).SmartLabelStyle.CalloutLineColor = Color.Red;
        kartChart.Series(0).SmartLabelStyle.CalloutLineWidth = 1;
        kartChart.Series(0).SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Box;

        // ' To force the chart to show all the points in the X axis
        kartChart.ChartAreas(0).AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
        // kartChart.ChartAreas(0).AxisY.IntervalOffset = 1
        kartChart.ChartAreas(0).AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
        // kartChart.ChartAreas(0).AxisX.IntervalOffset = 1

        kartChart.Series(0).Points.DataBind(dvwDataSource, strXDataField, strYDataField, GetChartAttributes());


        // ' Show up a legend if the Chart type is pie or doughnut
        if (valChartType == kartChrtType.Pie || valChartType == kartChrtType.Doughnut)
        {
            kartChart.Series(0)("PieDrawingStyle") = "SoftEdge";
            kartChart.ChartAreas(0).Area3DStyle.Enable3D = false;
            kartChart.Legends.Add("KartLegends");
            kartChart.Legends("KartLegends").Alignment = StringAlignment.Center;
            kartChart.Legends("KartLegends").LegendStyle = LegendStyle.Table;
            kartChart.Legends("KartLegends").TableStyle = LegendTableStyle.Wide;
            kartChart.Legends("KartLegends").Docking = Docking.Bottom;
        }

        if (blnDynamicSize)
        {
            // ' Dynamic Size: Will specify the size of the chart depending on no. of points
            kartChart.Height = numHeight * kartChart.Series(0).Points.Count;
            kartChart.Width = numWidth * kartChart.Series(0).Points.Count;
            // ' Check if the dimentions exceed the max.
            AdjustChartDimensions();
        }
        else
        {
            // ' Fixed Size (set by user)
            kartChart.Height = numHeight;
            kartChart.Width = numWidth;
        }

        // ' For small home page summary chart, we want to flip labels on
        // x-axis to be horizontal, so better use of space
        if (numHeight < 201)
        {
            kartChart.ChartAreas(0).AxisX().LabelStyle.Angle = 0;
            kartChart.ChartAreas(0).AxisX().LabelStyle.IsStaggered = true;
        }

        // ' Show up a 'No Data Message', if there is no rendered points
        if (kartChart.Series(0).Points.Count == 0)
            kartChart.Annotations("NoDataAnnotation").Visible = true;
        else
            kartChart.Annotations("NoDataAnnotation").Visible = false;

        // ' Refresh the chart
        // kartChart.ResetAutoValues()
        updKartChart.Update();
    }

    public void CopyDesign(ASP.usercontrols_statistics__kartrischart_ascx chart)
    {
        this.valChartType = chart.ChartType;
        this.valChartStyle = chart.ChartStyle;
        this.Enable3DView = chart.Enable3DView;
        this.ValueAsLabel = chart.ValueAsLabel;
    }

    protected void kartChart_Click(object sender, System.Web.UI.WebControls.ImageMapEventArgs e)
    {
        strSelectedPostBackValue = e.PostBackValue;
        ChartClicked?.Invoke(e.PostBackValue);
    }

    private string GetChartAttributes()
    {
        string strAttributes = "";
        if (!string.IsNullOrEmpty(strTooltipField))
            strAttributes += "Tooltip=" + strTooltipField + ",";
        if (!string.IsNullOrEmpty(strPostBackField))
            strAttributes += "PostBackValue=" + strPostBackField;
        if (strAttributes.EndsWith(","))
            strAttributes = strAttributes.TrimEnd(",");
        return strAttributes;
    }

    private void AdjustChartDimensions()
    {
        if (kartChart.Height.Value < numMinHeight)
            kartChart.Height = numMinHeight;
        if (kartChart.Width.Value < numMinWidth)
            kartChart.Width = numMinWidth;
        if (kartChart.Height.Value > numMaxHeight)
            kartChart.Height = numMaxHeight;
        if (kartChart.Width.Value > numMaxWidth)
            kartChart.Width = numMaxWidth;
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        kartChart.ViewStateContent = SerializationContents.Default;
        System.Web.UI.Control.Page.ClientScript.GetPostBackEventReference(this, string.Empty);
        if (!System.Web.UI.Control.Page.IsPostBack)
        {
            ddlChartType.Items.Clear();
            foreach (var chtType in Enum.GetValues(typeof(kartChrtType)))
                ddlChartType.Items.Add(new ListItem((kartChrtType)chtType.ToString(), (kartChrtType)chtType));
            ddlChartStyle.Items.Clear();
            foreach (var chtStyle in Enum.GetValues(typeof(kartChrtStyle)))
                ddlChartStyle.Items.Add(new ListItem((kartChrtStyle)chtStyle.ToString(), (kartChrtStyle)chtStyle));
            ddlChartType.SelectedValue = valChartType;
            ddlChartStyle.SelectedValue = valChartStyle;
            chkEnable3D.Checked = blnEnable3D;
            chkShowLabel.Checked = blnValueAsLabel;
        }
    }

    protected void btnOptions_Click(object sender, System.EventArgs e)
    {
        popExtender.Show();
    }

    protected void lnkOk_Click(object sender, System.EventArgs e)
    {
        valChartType = ddlChartType.SelectedValue;
        valChartStyle = ddlChartStyle.SelectedValue;
        blnEnable3D = chkEnable3D.Checked;
        blnValueAsLabel = chkShowLabel.Checked;
        OptionsChanged?.Invoke();
        updKartChart.Update();
    }
}
