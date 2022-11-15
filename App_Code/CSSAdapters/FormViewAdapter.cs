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
    public class FormViewAdapter : CompositeDataBoundControlAdapter
    {
        protected override string HeaderText
        {
            get
            {
                return ControlAsFormView.HeaderText;
            }
        }
        protected override string FooterText
        {
            get
            {
                return ControlAsFormView.FooterText;
            }
        }
        protected override ITemplate HeaderTemplate
        {
            get
            {
                return ControlAsFormView.HeaderTemplate;
            }
        }
        protected override ITemplate FooterTemplate
        {
            get
            {
                return ControlAsFormView.FooterTemplate;
            }
        }
        protected override TableRow HeaderRow
        {
            get
            {
                return ControlAsFormView.HeaderRow;
            }
        }
        protected override TableRow FooterRow
        {
            get
            {
                return ControlAsFormView.FooterRow;
            }
        }
        protected override bool AllowPaging
        {
            get
            {
                return ControlAsFormView.AllowPaging;
            }
        }
        protected override int DataItemCount
        {
            get
            {
                return ControlAsFormView.DataItemCount;
            }
        }
        protected override int DataItemIndex
        {
            get
            {
                return ControlAsFormView.DataItemIndex;
            }
        }
        protected override PagerSettings PagerSettings
        {
            get
            {
                return ControlAsFormView.PagerSettings;
            }
        }

        public FormViewAdapter()
        {
            _classMain = "Kartris-FormView";
            _classHeader = "Kartris-FormView-Header";
            _classData = "Kartris-FormView-Data";
            _classFooter = "Kartris-FormView-Footer";
            _classPagination = "Kartris-FormView-Pagination";
            _classOtherPage = "Kartris-FormView-OtherPage";
            _classActivePage = "Kartris-FormView-ActivePage";
        }

        protected override void BuildItem(HtmlTextWriter writer)
        {
            if (((!IsNothing(ControlAsFormView.Row)) && (ControlAsFormView.Row.Cells.Count > 0) && (ControlAsFormView.Row.Cells.Item(0).Controls.Count > 0)))
            {
                writer.WriteLine();
                writer.WriteBeginTag("div");
                writer.WriteAttribute("class", _classData);
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Indent = writer.Indent + 1;
                writer.WriteLine();

                Control itemCtrl;
                foreach (var itemCtrl in ControlAsFormView.Row.Cells(0).Controls)
                    itemCtrl.RenderControl(writer);

                writer.Indent = writer.Indent - 1;
                writer.WriteLine();
                writer.WriteEndTag("div");
            }
        }
    }
}
