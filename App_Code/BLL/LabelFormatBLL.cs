// DispatchLabels.ascx is a custom control that allows users to select
// a label format from a drop down and then send the client to a handler
// that will output a PDF file of dispatch labels in that format.
// Copyright (C) 2014  Craig Moore - Deadline Automation Limited.
// 
// GNU GENERAL PUBLIC LICENSE v2
// This program is free software distributed under the GPL without any
// warranty.
// www.gnu.org/licenses/gpl-2.0.html
// 
// If you make any modifications to this program please clearly state who,
// when and some small details in this header.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using kartrisLabelFormatDataTableAdapters;

public class LabelFormatBLL
{
    private static tblKartrisLabelFormatsTableAdapter _Adptr = null/* TODO Change to default(_) if this is not a reference type */;

    protected static tblKartrisLabelFormatsTableAdapter Adptr
    {
        get
        {
            if (IsNothing(_Adptr))
                _Adptr = new tblKartrisLabelFormatsTableAdapter();
            return _Adptr;
        }
    }

    /// <summary>
    ///     ''' Return all label formats in the database.
    ///     ''' </summary>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static List<LabelFormat> GetLabelFormats()
    {
        GetLabelFormats = null;
        DataTable dt = Adptr.GetData;
        LabelFormat lf;
        if (!IsNothing(dt))
        {
            GetLabelFormats = new List<LabelFormat>();
            foreach (DataRow dr in dt.Rows)
            {
                lf = new LabelFormat();
                {
                    var withBlock = lf;
                    withBlock.Id = dr("LBF_ID");
                    withBlock.Name = dr("LBF_LabelName");
                    withBlock.Description = dr("LBF_LabelDescription");
                    withBlock.PageWidth = System.Convert.ToDouble(dr("LBF_PageWidth"));
                    withBlock.PageHeight = System.Convert.ToDouble(dr("LBF_PageHeight"));
                    withBlock.LabelWidth = System.Convert.ToDouble(dr("LBF_LabelWidth"));
                    withBlock.LabelHeight = System.Convert.ToDouble(dr("LBF_LabelHeight"));
                    withBlock.TopMargin = System.Convert.ToDouble(dr("LBF_TopMargin"));
                    withBlock.LeftMargin = System.Convert.ToDouble(dr("LBF_LeftMargin"));
                    withBlock.LabelPaddingLeft = System.Convert.ToDouble(dr("LBF_LabelPaddingLeft"));
                    withBlock.LabelPaddingRight = System.Convert.ToDouble(dr("LBF_LabelPaddingRight"));
                    withBlock.LabelPaddingTop = System.Convert.ToDouble(dr("LBF_LabelPaddingTop"));
                    withBlock.LabelPaddingBottom = System.Convert.ToDouble(dr("LBF_LabelPaddingBottom"));
                    withBlock.VerticalPitch = System.Convert.ToDouble(dr("LBF_VerticalPitch"));
                    withBlock.HorizontalPitch = System.Convert.ToDouble(dr("LBF_HorizontalPitch"));
                    withBlock.ColumnCount = System.Convert.ToInt32(dr("LBF_LabelColumnCount"));
                    withBlock.RowCount = System.Convert.ToInt32(dr("LBF_LabelRowCount"));
                }
                GetLabelFormats.Add(lf);
            }
        }
    }

    /// <summary>
    ///     ''' Return only one label format from the database.
    ///     ''' </summary>
    ///     ''' <param name="FormatId">The Id that uniquely identifies the label format we wish to retrieve</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static LabelFormat GetLabelFormat(int FormatId)
    {
        DataTable dt = Adptr.GetSingleFormat(FormatId);
        if (!IsNothing(dt))
        {
            GetLabelFormat = new LabelFormat();
            {
                var withBlock = GetLabelFormat;
                withBlock.Id = dt(0)("LBF_ID");
                withBlock.Name = dt(0)("LBF_LabelName");
                withBlock.Description = dt(0)("LBF_LabelDescription");
                withBlock.PageWidth = System.Convert.ToDouble(dt(0)("LBF_PageWidth"));
                withBlock.PageHeight = System.Convert.ToDouble(dt(0)("LBF_PageHeight"));
                withBlock.LabelWidth = System.Convert.ToDouble(dt(0)("LBF_LabelWidth"));
                withBlock.LabelHeight = System.Convert.ToDouble(dt(0)("LBF_LabelHeight"));
                withBlock.TopMargin = System.Convert.ToDouble(dt(0)("LBF_TopMargin"));
                withBlock.LeftMargin = System.Convert.ToDouble(dt(0)("LBF_LeftMargin"));
                withBlock.LabelPaddingLeft = System.Convert.ToDouble(dt(0)("LBF_LabelPaddingLeft"));
                withBlock.LabelPaddingRight = System.Convert.ToDouble(dt(0)("LBF_LabelPaddingRight"));
                withBlock.LabelPaddingTop = System.Convert.ToDouble(dt(0)("LBF_LabelPaddingTop"));
                withBlock.LabelPaddingBottom = System.Convert.ToDouble(dt(0)("LBF_LabelPaddingBottom"));
                withBlock.VerticalPitch = System.Convert.ToDouble(dt(0)("LBF_VerticalPitch"));
                withBlock.HorizontalPitch = System.Convert.ToDouble(dt(0)("LBF_HorizontalPitch"));
                withBlock.ColumnCount = System.Convert.ToInt32(dt(0)("LBF_LabelColumnCount"));
                withBlock.RowCount = System.Convert.ToInt32(dt(0)("LBF_LabelRowCount"));
            }
        }
        else
            return null/* TODO Change to default(_) if this is not a reference type */;
    }
}
