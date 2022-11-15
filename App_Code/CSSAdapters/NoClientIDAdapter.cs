using System.Web.UI;
using System.Web.UI.Adapters;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace NoId
{
    public class Adapter : ControlAdapter
    {
        protected override void BeginRender(HtmlTextWriter writer)
        {
            // check if the current control doesn't handle postback
            if (ControlAdapter.Control is Table || !(ControlAdapter.Control is IPostBackDataHandler || ControlAdapter.Control is IPostBackEventHandler))
            {
                AttributeCollection attr = null;
                if (ControlAdapter.Control is WebControl)
                    attr = (WebControl)ControlAdapter.Control.Attributes;
                else if (ControlAdapter.Control is HtmlControl)
                    attr = (HtmlControl)ControlAdapter.Control.Attributes;
                if (attr != null)
                {
                    // if noid attribute is set to true, remove ID
                    string noId = attr["noid"];
                    if (noId == "true" || noId == "True")
                        ControlAdapter.Control.ID = null;
                    if (!string.IsNullOrEmpty(noId))
                        attr.Remove("noid");
                }
                // automatically remove ID if control is a label or hiddenfield
                if (ControlAdapter.Control is Label | ControlAdapter.Control is HiddenField)
                    ControlAdapter.Control.ID = null;
            }
            base.BeginRender(writer);
        }
    }
}
