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


namespace Kartris
{
    public class FormRewriterControlAdapter : System.Web.UI.Adapters.ControlAdapter
    {
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            base.Render(new RewriteFormHtmlTextWriter(writer));
        }
    }

    public class RewriteFormHtmlTextWriter : HtmlTextWriter
    {
        public RewriteFormHtmlTextWriter(HtmlTextWriter writer) : base(writer)
        {
            this.InnerWriter = writer.InnerWriter;
        }

        public RewriteFormHtmlTextWriter(System.IO.TextWriter writer) : base(writer)
        {
            base.InnerWriter = writer;
        }

        public override void WriteAttribute(string name, string value, bool fEncode)
        {

            // If the attribute we are writing is the "action" attribute, and we are not on a sub-control, 
            // then replace the value to write with the raw URL of the request - which ensures that we'll
            // preserve the PathInfo value on postback scenarios

            if ((name == "action"))
            {
                HttpContext Context;
                Context = HttpContext.Current;

                if (Context.Items("ActionAlreadyWritten") == null)
                {
                    value = Context.Request.RawUrl;

                    // Indicate that we've already rewritten the <form>'s action attribute to prevent
                    // us from rewriting a sub-control under the <form> control

                    Context.Items("ActionAlreadyWritten") = true;
                }
            }

            base.WriteAttribute(name, value, fEncode);
        }
    }
}
