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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

/// <summary>

/// ''' Utility class for creating labels within a PDF document

/// ''' </summary>

/// ''' <remarks></remarks>
public class PdfLabelUtil
{

    /// <summary>
    ///     ''' Generate a list of labels on a PDF page. Use a format sheet to define the layout of each label
    ///     ''' </summary>
    ///     ''' <param name="Addresses">List of strings that represent the postal address</param>
    ///     ''' <param name="lf">Format of label sheet that is to be printed</param>
    ///     ''' <param name="QtyEachLabel">How many of each label that should be printed</param>
    ///     ''' <returns>Memorystream object representing a PDF document.</returns>
    ///     ''' <remarks></remarks>
    public static MemoryStream GeneratePdfLabels(List<string> Addresses, LabelFormat lf, int QtyEachLabel = 1)
    {
        GeneratePdfLabels = new MemoryStream();

        // The label sheet is basically a table and each cell is a single label

        // Format related
        int CellsPerPage = lf.RowCount * lf.ColumnCount;
        int CellsThisPage = 0;
        XRect ContentRectangle;       // A single cell content rectangle. This is the rectangle that can be used for contents and accounts for margins and padding.
        XSize ContentSize;            // Size of content area inside a cell.
        double ContentLeftPos;        // left edge of current content area.
        double ContentTopPos;         // Top edge of current content area

        // Layout related
        XColor StrokeColor = XColors.Black;
        XColor FillColor = XColors.Black;
        XPen Pen = new XPen(StrokeColor, 0.1);
        XBrush Brush = new XSolidBrush(FillColor);
        XGraphics Gfx;
        XGraphicsPath Path;

        int LoopTemp = 0;         // Counts each itteration. Used with QtyEachLabel
        int CurrentColumn = 1;
        int CurrentRow = 1;
        PdfDocument Doc = new PdfDocument();
        PdfPage page = null/* TODO Change to default(_) if this is not a reference type */;
        AddPage(ref Doc, ref page, lf);
        Gfx = XGraphics.FromPdfPage(page);

        // Ensure that at least 1 of each label is printed.
        if (QtyEachLabel < 1)
            QtyEachLabel = 1;

        // Define the content area size
        ContentSize = new XSize(XUnit.FromMillimeter(lf.LabelWidth - lf.LabelPaddingLeft - lf.LabelPaddingRight).Point, XUnit.FromMillimeter(lf.LabelHeight - lf.LabelPaddingTop - lf.LabelPaddingBottom).Point);

        if (!Information.IsNothing(Addresses))
        {
            if (Addresses.Count > 0)
            {
                // We actually have addresses to output.
                foreach (string Address in Addresses)
                {
                    // Once for each address
                    for (LoopTemp = 1; LoopTemp <= QtyEachLabel; LoopTemp++)
                    {
                        // Once for each copy of this address.
                        if (CellsThisPage == CellsPerPage)
                        {
                            // This pages worth of cells are filled up. Create a new page
                            AddPage(ref Doc, ref page, lf);
                            Gfx = XGraphics.FromPdfPage(page);
                            CellsThisPage = 0;
                        }

                        // Calculate which row and column we are working on.
                        CurrentColumn = (CellsThisPage + 1) % lf.ColumnCount;
                        CurrentRow = Fix((CellsThisPage + 1) / (double)lf.ColumnCount);

                        if (CurrentColumn == 0)
                            // This occurs when you are working on the last column of the row. 
                            // This affects the count for column and row
                            CurrentColumn = lf.ColumnCount;
                        else
                            // We are not viewing the last column so this number will be decremented by one.
                            CurrentRow = CurrentRow + 1;

                        // Calculate the left position of the current cell.
                        ContentLeftPos = ((CurrentColumn - 1) * lf.HorizontalPitch) + lf.LeftMargin + lf.LabelPaddingLeft;

                        // Calculate the top position of the current cell.
                        ContentTopPos = ((CurrentRow - 1) * lf.VerticalPitch) + lf.TopMargin + lf.LabelPaddingTop;

                        // Define the content rectangle.
                        ContentRectangle = new XRect(new XPoint(XUnit.FromMillimeter(ContentLeftPos).Point, XUnit.FromMillimeter(ContentTopPos).Point), ContentSize);

                        Path = new XGraphicsPath();

                        // Add the address string to the page.
                        Path.AddString(Address, new XFontFamily("Arial"), XFontStyle.Regular, 11, ContentRectangle, XStringFormats.TopLeft);

                        Gfx.DrawPath(Pen, Brush, Path);

                        // Increment the cell count
                        CellsThisPage = CellsThisPage + 1;
                    }
                }
                // Output the document
                Doc.Save(GeneratePdfLabels, false);
            }
        }
    }

    /// <summary>
    ///     ''' Generate shipping address labels for all orders that are ready for dispatch
    ///     ''' </summary>
    ///     ''' <param name="lf">Format of label sheet that is to be printed</param>
    ///     ''' <param name="QtyEachLabel">How many of each label that should be printed</param>
    ///     ''' <returns>Memorystream object representing a PDF document.</returns>
    ///     ''' <remarks></remarks>
    public static MemoryStream GeneratePdfDispatchLabels(LabelFormat lf, int QtyEachLabel = 0)
    {
        GeneratePdfDispatchLabels = null;
        DateTime FromDateTime = new DateTime(2000, 1, 1);
        DateTime ToDateTime = new DateTime(2049, 1, 1);
        DataTable tblOrdersList = null/* TODO Change to default(_) if this is not a reference type */;                // All dispatch labels
        DataTable tblSingleOrder = null/* TODO Change to default(_) if this is not a reference type */;               // A single dispatch label
        string ShippingAddress = string.Empty;
        List<string> ShippingAddresses = new List<string>();

        // Get the dispatch label information.
        OrdersBLL objOrdersBLL = new OrdersBLL();
        tblOrdersList = objOrdersBLL._GetByStatus(OrdersBLL.ORDERS_LIST_CALLMODE.DISPATCH, 0, 0, FromDateTime, ToDateTime, "", "", 999);

        if (!IsNothing(tblOrdersList))
        {
            if (tblOrdersList.Rows.Count > 0)
            {
                foreach (DataRow OrdersRow in tblOrdersList.Rows)
                {
                    // Itterate each order.
                    tblSingleOrder = objOrdersBLL.GetOrderByID(OrdersRow("O_ID"));
                    if (!IsNothing(tblSingleOrder))
                    {
                        foreach (DataRow DetailRow in tblSingleOrder.Rows)
                        {
                            // Itterate each detail row in the single order (should only be one).
                            ShippingAddress = StripTelephoneNumber(DetailRow("O_ShippingAddress").ToString);
                            ShippingAddresses.Add(ShippingAddress);
                        }
                    }
                }
            }
        }
        if (ShippingAddresses.Count > 0)
            // We have shipping addresses. Generate the PDF from these.
            GeneratePdfDispatchLabels = GeneratePdfLabels(ShippingAddresses, lf, QtyEachLabel);
    }

    private static void AddPage(ref PdfDocument Doc, ref PdfPage Page, LabelFormat lf)
    {
        Page = Doc.AddPage;
        Page.Width = XUnit.FromMillimeter(lf.PageWidth);
        Page.Height = XUnit.FromMillimeter(lf.PageHeight);
    }

    /// <summary>
    ///     ''' Takes a string in that is seperated by newline characters and returns the same text with the telephone number stripped.
    ///     ''' </summary>
    ///     ''' <param name="str">Complete string</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    private static string StripTelephoneNumber(string str)
    {
        StripTelephoneNumber = str;  // Initial Value

        string[] lines = str.Split(Strings.Chr(10));

        if (lines.Count() > 0)
        {
            str = string.Empty;
            foreach (string line in lines)
            {
                if (!IsPhoneNumber(line))
                {
                    line = Strings.Replace(line, Strings.Chr(13), string.Empty);
                    if (Strings.Trim(line).Length > 0)
                        // Line does not contain a telephone number, add it to the string.
                        str = str + line + Constants.vbCrLf;
                }
            }
            return str;
        }
    }


    /// <summary>
    ///     '''  Test to see if a string contains anythign that cannot be used in a phone number. 
    ///     ''' As standard the return is true, but return false if a non-phone number character is found.    
    ///     ''' </summary>
    ///     ''' <param name="input"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    private static bool IsPhoneNumber(string input)
    {
        if (input.Length < 6)
            // Prevents door numbers etc. being detected as a phone number
            return false;
        foreach (char c in input)
        {
            if (!Information.IsNumeric(c))
            {
                if (c != "+" & c != "-" & c != " " & c != "" & Strings.Asc(c) != 10)
                    return false;
            }
        }
        return true;
    }
}
