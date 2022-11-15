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
    public abstract class CompositeDataBoundControlAdapter : System.Web.UI.WebControls.Adapters.DataBoundControlAdapter
    {
        protected WebControlAdapterExtender This = null/* TODO Change to default(_) if this is not a reference type */;
        private WebControlAdapterExtender _extender = null/* TODO Change to default(_) if this is not a reference type */;
        private WebControlAdapterExtender Extender
        {
            get
            {
                if (((IsNothing(_extender) && (!Information.IsNothing(System.Web.UI.WebControls.Adapters.DataBoundControlAdapter.Control))) || ((!IsNothing(_extender)) && (!System.Web.UI.WebControls.Adapters.DataBoundControlAdapter.Control.Equals(_extender.AdaptedControl)))))
                    _extender = new WebControlAdapterExtender(System.Web.UI.WebControls.Adapters.DataBoundControlAdapter.Control);

                System.Diagnostics.Debug.Assert(!IsNothing(_extender), "CSS Friendly adapters internal error", "Null extender instance");
                return _extender;
            }
        }

        protected string _classMain = "";
        protected string _classHeader = "";
        protected string _classData = "";
        protected string _classFooter = "";
        protected string _classPagination = "";
        protected string _classOtherPage = "";
        protected string _classActivePage = "";

        protected CompositeDataBoundControl View
        {
            get
            {
                try
                {
                    return (CompositeDataBoundControl)System.Web.UI.WebControls.Adapters.DataBoundControlAdapter.Control;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        protected DetailsView ControlAsDetailsView
        {
            get
            {
                try
                {
                    return (DetailsView)System.Web.UI.WebControls.Adapters.DataBoundControlAdapter.Control;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        protected bool IsDetailsView
        {
            get
            {
                return !Information.IsNothing(ControlAsDetailsView);
            }
        }

        protected FormView ControlAsFormView
        {
            get
            {
                try
                {
                    return (FormView)System.Web.UI.WebControls.Adapters.DataBoundControlAdapter.Control;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        protected bool IsFormView
        {
            get
            {
                return !Information.IsNothing(ControlAsFormView);
            }
        }

        protected abstract string HeaderText { get; }
        protected abstract string FooterText { get; }
        protected abstract ITemplate HeaderTemplate { get; }
        protected abstract ITemplate FooterTemplate { get; }
        protected abstract TableRow HeaderRow { get; }
        protected abstract TableRow FooterRow { get; }
        protected abstract bool AllowPaging { get; }
        protected abstract int DataItemCount { get; }
        protected abstract int DataItemIndex { get; }
        protected abstract PagerSettings PagerSettings { get; }

        // / ///////////////////////////////////////////////////////////////////////////////
        // / METHODS

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if ((Extender.AdapterEnabled))
                RegisterScripts();
        }

        protected override void RenderBeginTag(HtmlTextWriter writer)
        {
            if ((Extender.AdapterEnabled))
            {
                if (((!Information.IsNothing(System.Web.UI.WebControls.Adapters.DataBoundControlAdapter.Control)) && (!string.IsNullOrEmpty(System.Web.UI.WebControls.Adapters.DataBoundControlAdapter.Control.Attributes.Item["CssSelectorClass"]))))
                {
                    writer.WriteLine();
                    writer.WriteBeginTag("div");
                    writer.WriteAttribute("class", System.Web.UI.WebControls.Adapters.DataBoundControlAdapter.Control.Attributes["CssSelectorClass"]);
                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.Indent = writer.Indent + 1;
                }

                writer.WriteLine();
                writer.WriteBeginTag("div");
                writer.WriteAttribute("class", _classMain);
                writer.Write(HtmlTextWriter.TagRightChar);
            }
            else
                base.RenderBeginTag(writer);
        }

        protected override void RenderEndTag(HtmlTextWriter writer)
        {
            if ((Extender.AdapterEnabled))
            {
                writer.WriteEndTag("div");

                if (((!Information.IsNothing(System.Web.UI.WebControls.Adapters.DataBoundControlAdapter.Control)) && (!string.IsNullOrEmpty(System.Web.UI.WebControls.Adapters.DataBoundControlAdapter.Control.Attributes.Item["CssSelectorClass"]))))
                {
                    writer.Indent = writer.Indent - 1;
                    writer.WriteLine();
                    writer.WriteEndTag("div");
                }

                writer.WriteLine();
            }
            else
                base.RenderEndTag(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if ((Extender.AdapterEnabled))
            {
                if (!Information.IsNothing(View))
                {
                    writer.Indent = writer.Indent + 1;

                    BuildRow(HeaderRow, _classHeader, writer);
                    BuildItem(writer);
                    BuildRow(FooterRow, _classFooter, writer);
                    BuildPaging(writer);

                    writer.Indent = writer.Indent - 1;
                    writer.WriteLine();
                }
            }
            else
                base.RenderContents(writer);
        }

        protected virtual void BuildItem(HtmlTextWriter writer)
        {
        }

        protected virtual void BuildRow(TableRow row, string cssClass, HtmlTextWriter writer)
        {
            if (((!Information.IsNothing(row)) && row.Visible))
            {

                // If there isn't any content, don't render anything.

                bool bHasContent = false;
                int iCell;
                TableCell cell;
                for (iCell = 0; iCell <= row.Cells.Count - 1; iCell++)
                {
                    cell = row.Cells[iCell];
                    if (((!string.IsNullOrEmpty(cell.Text)) || (cell.Controls.Count > 0)))
                    {
                        bHasContent = true;
                        break;
                    }
                }

                if ((bHasContent))
                {
                    writer.WriteLine();
                    writer.WriteBeginTag("div");
                    writer.WriteAttribute("class", cssClass);
                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.Indent = writer.Indent + 1;
                    writer.WriteLine();

                    for (iCell = 0; iCell <= row.Cells.Count - 1; iCell++)
                    {
                        cell = row.Cells[iCell];
                        if ((!string.IsNullOrEmpty(cell.Text)))
                            writer.Write(cell.Text);
                        Control cellChildControl;
                        foreach (var cellChildControl in cell.Controls)
                            cellChildControl.RenderControl(writer);
                    }

                    writer.Indent = writer.Indent - 1;
                    writer.WriteLine();
                    writer.WriteEndTag("div");
                }
            }
        }

        protected virtual void BuildPaging(HtmlTextWriter writer)
        {
            if ((AllowPaging && (DataItemCount > 0)))
            {
                writer.WriteLine();
                writer.WriteBeginTag("div");
                writer.WriteAttribute("class", _classPagination);
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Indent = writer.Indent + 1;

                int iStart = 0;
                int iEnd = DataItemCount;
                int nPages = iEnd - iStart + 1;
                bool bExceededPageButtonCount = nPages > PagerSettings.PageButtonCount;

                if ((bExceededPageButtonCount))
                {
                    iStart = (DataItemIndex / PagerSettings.PageButtonCount) * PagerSettings.PageButtonCount;
                    iEnd = Math.Min(iStart + PagerSettings.PageButtonCount, DataItemCount);
                }

                writer.WriteLine();

                if ((bExceededPageButtonCount && (iStart > 0)))
                {
                    writer.WriteBeginTag("a");
                    writer.WriteAttribute("class", _classOtherPage);
                    writer.WriteAttribute("href", System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.GetPostBackClientHyperlink(System.Web.UI.WebControls.Adapters.DataBoundControlAdapter.Control, "Page$" + iStart.ToString(), true));
                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.Write("...");
                    writer.WriteEndTag("a");
                }

                int iDataItem;
                for (iDataItem = iStart; iDataItem <= iEnd - 1; iDataItem++)
                {
                    string strPage = (iDataItem + 1).ToString();
                    if ((DataItemIndex == iDataItem))
                    {
                        writer.WriteBeginTag("span");
                        writer.WriteAttribute("class", _classActivePage);
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.Write(strPage);
                        writer.WriteEndTag("span");
                    }
                    else
                    {
                        writer.WriteBeginTag("a");
                        writer.WriteAttribute("class", _classOtherPage);
                        writer.WriteAttribute("href", System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.GetPostBackClientHyperlink(System.Web.UI.WebControls.Adapters.DataBoundControlAdapter.Control, "Page$" + strPage, true));
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.Write(strPage);
                        writer.WriteEndTag("a");
                    }
                }

                if ((bExceededPageButtonCount && (iEnd < DataItemCount)))
                {
                    writer.WriteBeginTag("a");
                    writer.WriteAttribute("class", _classOtherPage);
                    writer.WriteAttribute("href", System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.GetPostBackClientHyperlink(System.Web.UI.WebControls.Adapters.DataBoundControlAdapter.Control, "Page$" + (iEnd + 1).ToString(), true));
                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.Write("...");
                    writer.WriteEndTag("a");
                }

                writer.Indent = writer.Indent - 1;
                writer.WriteLine();
                writer.WriteEndTag("div");
            }
        }

        protected virtual void RegisterScripts()
        {
        }
    }
}
