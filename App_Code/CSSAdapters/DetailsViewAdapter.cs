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
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Kartris
{
    public class DetailsViewAdapter : CompositeDataBoundControlAdapter
    {
        protected override string HeaderText
        {
            get
            {
                return ControlAsDetailsView.HeaderText;
            }
        }
        protected override string FooterText
        {
            get
            {
                return ControlAsDetailsView.FooterText;
            }
        }
        protected override ITemplate HeaderTemplate
        {
            get
            {
                return ControlAsDetailsView.HeaderTemplate;
            }
        }
        protected override ITemplate FooterTemplate
        {
            get
            {
                return ControlAsDetailsView.FooterTemplate;
            }
        }
        protected override TableRow HeaderRow
        {
            get
            {
                return ControlAsDetailsView.HeaderRow;
            }
        }
        protected override TableRow FooterRow
        {
            get
            {
                return ControlAsDetailsView.FooterRow;
            }
        }
        protected override bool AllowPaging
        {
            get
            {
                return ControlAsDetailsView.AllowPaging;
            }
        }
        protected override int DataItemCount
        {
            get
            {
                return ControlAsDetailsView.DataItemCount;
            }
        }
        protected override int DataItemIndex
        {
            get
            {
                return ControlAsDetailsView.DataItemIndex;
            }
        }
        protected override PagerSettings PagerSettings
        {
            get
            {
                return ControlAsDetailsView.PagerSettings;
            }
        }

        public DetailsViewAdapter()
        {
            _classMain = "Kartris-DetailsView";
            _classHeader = "Kartris-DetailsView-Header";
            _classData = "Kartris-DetailsView-Data";
            _classFooter = "Kartris-DetailsView-Footer";
            _classPagination = "Kartris-DetailsView-Pagination";
            _classOtherPage = "Kartris-DetailsView-OtherPage";
            _classActivePage = "Kartris-DetailsView-ActivePage";
        }

        protected override void BuildItem(HtmlTextWriter writer)
        {
            if ((IsDetailsView && (ControlAsDetailsView.Rows.Count > 0)))
            {
                writer.WriteLine();
                writer.WriteBeginTag("div");
                writer.WriteAttribute("class", _classData);
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Indent = writer.Indent + 1;

                writer.WriteLine();
                writer.WriteBeginTag("ul");
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Indent = writer.Indent + 1;

                bool useFields = (!ControlAsDetailsView.AutoGenerateRows) && (ControlAsDetailsView.Fields.Count == ControlAsDetailsView.Rows.Count);
                int countRenderedRows = 0;
                int iRow;
                for (iRow = 0; iRow <= ControlAsDetailsView.Rows.Count - 1; iRow++)
                {
                    if (((!useFields) || ControlAsDetailsView.Fields(iRow).Visible))
                    {
                        DetailsViewRow row = ControlAsDetailsView.Rows(iRow);

                        if (((!ControlAsDetailsView.AutoGenerateRows) && ((row.RowState & DataControlRowState.Insert) == DataControlRowState.Insert) && (!ControlAsDetailsView.Fields.Item(row.RowIndex).InsertVisible)))
                            continue;

                        writer.WriteLine();
                        writer.WriteBeginTag("li");

                        string theClass = Interaction.IIf(((countRenderedRows % 2) == 1), "Kartris-DetailsView-Alternate", "");
                        if ((useFields && (!IsNothing(ControlAsDetailsView.Fields(iRow).ItemStyle)) && (!string.IsNullOrEmpty(ControlAsDetailsView.Fields(iRow).ItemStyle.CssClass))))
                        {
                            if ((!string.IsNullOrEmpty(theClass)))
                                theClass += " ";
                            theClass += ControlAsDetailsView.Fields(iRow).ItemStyle.CssClass;
                        }
                        if ((!string.IsNullOrEmpty(theClass)))
                            writer.WriteAttribute("class", theClass);
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.Indent = writer.Indent + 1;
                        writer.WriteLine();

                        int iCell;
                        for (iCell = 0; iCell <= row.Cells.Count - 1; iCell++)
                        {
                            TableCell cell = row.Cells[iCell];
                            writer.WriteBeginTag("span");
                            if (iCell == 0)
                                writer.WriteAttribute("class", "Kartris-DetailsView-Name");
                            else if (iCell == 1)
                                writer.WriteAttribute("class", "Kartris-DetailsView-Value");
                            else
                                writer.WriteAttribute("class", "Kartris-DetailsView-Misc");
                            writer.Write(HtmlTextWriter.TagRightChar);
                            if ((!string.IsNullOrEmpty(cell.Text)))
                                writer.Write(cell.Text);
                            Control cellChildControl;
                            foreach (var cellChildControl in cell.Controls)
                                cellChildControl.RenderControl(writer);
                            writer.WriteEndTag("span");
                        }

                        writer.Indent = writer.Indent - 1;
                        writer.WriteLine();
                        writer.WriteEndTag("li");
                        countRenderedRows += 1;
                    }
                }

                writer.Indent = writer.Indent - 1;
                writer.WriteLine();
                writer.WriteEndTag("ul");

                writer.Indent = writer.Indent - 1;
                writer.WriteLine();
                writer.WriteEndTag("div");
            }
        }
    }
}
