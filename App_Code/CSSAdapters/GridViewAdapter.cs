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

using System.Data;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Kartris
{
    public class GridViewAdapter : System.Web.UI.WebControls.Adapters.WebControlAdapter
    {
        protected WebControlAdapterExtender This = null/* TODO Change to default(_) if this is not a reference type */;
        private WebControlAdapterExtender _extender = null/* TODO Change to default(_) if this is not a reference type */;
        private WebControlAdapterExtender Extender
        {
            get
            {
                if (((IsNothing(_extender) && (!Information.IsNothing(System.Web.UI.WebControls.Adapters.WebControlAdapter.Control))) || ((!IsNothing(_extender)) && (!System.Web.UI.WebControls.Adapters.WebControlAdapter.Control.Equals(_extender.AdaptedControl)))))
                    _extender = new WebControlAdapterExtender(System.Web.UI.WebControls.Adapters.WebControlAdapter.Control);

                System.Diagnostics.Debug.Assert(!IsNothing(_extender), "CSS Friendly adapters internal error", "Null extender instance");
                return _extender;
            }
        }

        // / ///////////////////////////////////////////////////////////////////////////////
        // / PROTECTED        

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Extender.AdapterEnabled)
                RegisterScripts();
        }

        protected override void RenderBeginTag(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
                Extender.RenderBeginTag(writer, "Kartris-GridView");
            else
                base.RenderBeginTag(writer);
        }

        protected override void RenderEndTag(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
                Extender.RenderEndTag(writer);
            else
                base.RenderEndTag(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
            {
                GridView gridView = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
                if ((!Information.IsNothing(gridView)))
                {
                    writer.Indent = writer.Indent + 1;
                    WritePagerSection(writer, PagerPosition.Top);

                    writer.WriteLine();
                    writer.WriteBeginTag("table");
                    writer.WriteAttribute("cellpadding", "0");
                    writer.WriteAttribute("cellspacing", "0");
                    writer.WriteAttribute("summary", System.Web.UI.WebControls.Adapters.WebControlAdapter.Control.ToolTip);

                    if (!string.IsNullOrEmpty(gridView.CssClass))
                        writer.WriteAttribute("class", gridView.CssClass);

                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.Indent = writer.Indent + 1;

                    ArrayList rows = new ArrayList();
                    GridViewRowCollection gvrc = null;

                    // /////////////////// HEAD /////////////////////////////

                    rows.Clear();
                    if ((gridView.ShowHeader && (!Information.IsNothing(gridView.HeaderRow))))
                        rows.Add(gridView.HeaderRow);
                    gvrc = new GridViewRowCollection(rows);
                    WriteRows(writer, gridView, gvrc, "thead");

                    // /////////////////// FOOT /////////////////////////////

                    rows.Clear();
                    if ((gridView.ShowFooter && (!Information.IsNothing(gridView.FooterRow))))
                        rows.Add(gridView.FooterRow);
                    gvrc = new GridViewRowCollection(rows);
                    WriteRows(writer, gridView, gvrc, "tfoot");

                    // /////////////////// BODY /////////////////////////////

                    WriteRows(writer, gridView, gridView.Rows, "tbody");

                    // //////////////////////////////////////////////////////

                    writer.Indent = writer.Indent - 1;
                    writer.WriteLine();
                    writer.WriteEndTag("table");

                    WritePagerSection(writer, PagerPosition.Bottom);

                    writer.Indent = writer.Indent - 1;
                    writer.WriteLine();
                }
            }
            else
                base.RenderContents(writer);
        }

        // / ///////////////////////////////////////////////////////////////////////////////
        // / PRIVATE        

        private void RegisterScripts()
        {
        }

        private void WriteRows(HtmlTextWriter writer, GridView gridView, GridViewRowCollection rows, string tableSection)
        {
            if (rows.Count > 0)
            {
                writer.WriteLine();
                writer.WriteBeginTag(tableSection);
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Indent = writer.Indent + 1;

                GridViewRow row;
                foreach (var row in rows)
                {
                    writer.WriteLine();
                    writer.WriteBeginTag("tr");

                    string className = GetRowClass(gridView, row);
                    if (row.CssClass != "")
                        className = row.CssClass;
                    if ((!string.IsNullOrEmpty(className)))
                        writer.WriteAttribute("class", className);

                    // We can uncomment the following block of code if we want to add arbitrary attributes.
                    // 
                    // Dim key As String
                    // For Each key In row.Attributes.Keys
                    // writer.WriteAttribute(key, row.Attributes(key))
                    // Next

                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.Indent = writer.Indent + 1;

                    TableCell cell;
                    foreach (var cell in row.Cells)
                    {
                        DataControlFieldCell fieldCell = cell;
                        if (((!Information.IsNothing(fieldCell)) && (!Information.IsNothing(fieldCell.ContainingField))))
                        {
                            DataControlField field = fieldCell.ContainingField;
                            if ((!field.Visible))
                                cell.Visible = false;

                            if (((!Information.IsNothing(field.ItemStyle)) && (!string.IsNullOrEmpty(field.ItemStyle.CssClass))))
                            {
                                if ((!string.IsNullOrEmpty(cell.CssClass)))
                                    cell.CssClass += " ";
                                cell.CssClass += field.ItemStyle.CssClass;
                            }
                        }

                        writer.WriteLine();
                        cell.RenderControl(writer);
                    }

                    writer.Indent = writer.Indent - 1;
                    writer.WriteLine();
                    writer.WriteEndTag("tr");
                }

                writer.Indent = writer.Indent - 1;
                writer.WriteLine();
                writer.WriteEndTag(tableSection);
            }
        }

        private string GetRowClass(GridView gridView, GridViewRow row)
        {
            string className = "";

            if (((row.RowState & DataControlRowState.Alternate) == DataControlRowState.Alternate))
            {
                className += " Kartris-GridView-Alternate ";
                if ((!Information.IsNothing(gridView.AlternatingRowStyle)))
                    className += gridView.AlternatingRowStyle.CssClass;
            }
            else if ((row.Equals(gridView.HeaderRow) && (!Information.IsNothing(gridView.HeaderStyle)) && (!string.IsNullOrEmpty(gridView.HeaderStyle.CssClass))))
                className += " " + gridView.HeaderStyle.CssClass;
            else if ((row.Equals(gridView.FooterRow) && (!Information.IsNothing(gridView.FooterStyle)) && (!string.IsNullOrEmpty(gridView.FooterStyle.CssClass))))
                className += " " + gridView.FooterStyle.CssClass;
            else if (((!Information.IsNothing(gridView.RowStyle)) && (!string.IsNullOrEmpty(gridView.RowStyle.CssClass))))
                className += " " + gridView.RowStyle.CssClass;

            if (((row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit))
            {
                className += " Kartris-GridView-Edit ";
                if ((!Information.IsNothing(gridView.EditRowStyle)))
                    className += gridView.EditRowStyle.CssClass;
            }

            if (((row.RowState & DataControlRowState.Insert) == DataControlRowState.Insert))
                className += " Kartris-GridView-Insert ";

            if (((row.RowState & DataControlRowState.Selected) == DataControlRowState.Selected))
            {
                className += " Kartris-GridView-Selected ";
                if ((!Information.IsNothing(gridView.SelectedRowStyle)))
                    className += gridView.SelectedRowStyle.CssClass;
            }

            return className.Trim();
        }

        private void WritePagerSection(HtmlTextWriter writer, PagerPosition pos)
        {
            GridView gridView = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;

            if (((!Information.IsNothing(gridView)) && gridView.AllowPaging && ((gridView.PagerSettings.Position == pos) || (gridView.PagerSettings.Position == PagerPosition.TopAndBottom))))
            {
                Table innerTable = null;

                if (((pos == PagerPosition.Top) && (!Information.IsNothing(gridView.TopPagerRow)) && (gridView.TopPagerRow.Cells.Count == 1) && (gridView.TopPagerRow.Cells[0].Controls.Count == 1) && (gridView.TopPagerRow.Cells[0].Controls[0] is Table)))
                    innerTable = (Table)gridView.TopPagerRow.Cells[0].Controls[0];
                else if (((pos == PagerPosition.Bottom) && (!Information.IsNothing(gridView.BottomPagerRow)) && (gridView.BottomPagerRow.Cells.Count == 1) && (gridView.BottomPagerRow.Cells[0].Controls.Count == 1) && (gridView.BottomPagerRow.Cells[0].Controls[0] is Table)))
                    innerTable = (Table)gridView.BottomPagerRow.Cells[0].Controls[0];

                if (((!Information.IsNothing(innerTable)) && (innerTable.Rows.Count == 1)))
                {
                    string className = "Kartris-GridView-Pagination Kartris-GridView-";
                    className += Interaction.IIf(pos == PagerPosition.Top, "Top ", "Bottom ");
                    if ((!Information.IsNothing(gridView.PagerStyle)))
                        className += gridView.PagerStyle.CssClass;
                    className = className.Trim();

                    writer.WriteLine();
                    writer.WriteBeginTag("div");
                    writer.WriteAttribute("class", className);
                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.Indent = writer.Indent + 1;

                    TableRow row = innerTable.Rows[0];
                    TableCell cell;
                    foreach (var cell in row.Cells)
                    {
                        Control ctrl;
                        foreach (var ctrl in cell.Controls)
                        {
                            writer.WriteLine();
                            ctrl.RenderControl(writer);
                        }
                    }

                    writer.Indent = writer.Indent - 1;
                    writer.WriteLine();
                    writer.WriteEndTag("div");
                }
            }
        }
    }
}
