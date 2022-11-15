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
    public class LoginStatusAdapter : System.Web.UI.WebControls.Adapters.WebControlAdapter
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

            if ((Extender.AdapterEnabled))
                RegisterScripts();
        }

        protected override void RenderBeginTag(HtmlTextWriter writer)
        {
            if ((Extender.AdapterEnabled))
            {
            }
            else
                base.RenderBeginTag(writer);
        }

        protected override void RenderEndTag(HtmlTextWriter writer)
        {
            if ((Extender.AdapterEnabled))
            {
            }
            else
                base.RenderEndTag(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if ((Extender.AdapterEnabled))
            {
                LoginStatus loginStatus = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
                if ((!Information.IsNothing(loginStatus)))
                {
                    string className = Interaction.IIf(!string.IsNullOrEmpty(loginStatus.CssClass), "Kartris-LoginStatus " + loginStatus.CssClass, "Kartris-LoginStatus");
                    if (Membership.GetUser() == null)
                    {
                        if ((!string.IsNullOrEmpty(loginStatus.LoginImageUrl)))
                        {
                            Control ctl = loginStatus.FindControl("ctl03");
                            if ((!Information.IsNothing(ctl)))
                            {
                                writer.WriteBeginTag("input");
                                writer.WriteAttribute("id", loginStatus.ClientID);
                                writer.WriteAttribute("type", "image");
                                writer.WriteAttribute("name", ctl.UniqueID);
                                writer.WriteAttribute("title", loginStatus.ToolTip);
                                writer.WriteAttribute("class", className);
                                writer.WriteAttribute("src", loginStatus.ResolveClientUrl(loginStatus.LoginImageUrl));
                                writer.WriteAttribute("alt", loginStatus.LoginText);
                                writer.Write(HtmlTextWriter.SelfClosingTagEnd);
                                System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(ctl.UniqueID);
                            }
                        }
                        else
                        {
                            Control ctl = loginStatus.FindControl("ctl02");
                            if ((!Information.IsNothing(ctl)))
                            {
                                writer.WriteBeginTag("a");
                                writer.WriteAttribute("id", loginStatus.ClientID);
                                writer.WriteAttribute("title", loginStatus.ToolTip);
                                writer.WriteAttribute("class", className);
                                writer.WriteAttribute("href", System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.GetPostBackClientHyperlink(loginStatus.FindControl("ctl02"), ""));
                                writer.Write(HtmlTextWriter.TagRightChar);
                                writer.Write(loginStatus.LoginText);
                                writer.WriteEndTag("a");
                                System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(ctl.UniqueID);
                            }
                        }
                    }
                    else if ((!string.IsNullOrEmpty(loginStatus.LogoutImageUrl)))
                    {
                        Control ctl = loginStatus.FindControl("ctl01");
                        if ((!Information.IsNothing(ctl)))
                        {
                            writer.WriteBeginTag("input");
                            writer.WriteAttribute("id", loginStatus.ClientID);
                            writer.WriteAttribute("type", "image");
                            writer.WriteAttribute("name", ctl.UniqueID);
                            writer.WriteAttribute("title", loginStatus.ToolTip);
                            writer.WriteAttribute("class", className);
                            writer.WriteAttribute("src", loginStatus.ResolveClientUrl(loginStatus.LogoutImageUrl));
                            writer.WriteAttribute("alt", loginStatus.LogoutText);
                            writer.Write(HtmlTextWriter.SelfClosingTagEnd);
                            System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(ctl.UniqueID);
                        }
                    }
                    else
                    {
                        Control ctl = loginStatus.FindControl("ctl00");
                        if ((!Information.IsNothing(ctl)))
                        {
                            writer.WriteBeginTag("a");
                            writer.WriteAttribute("id", loginStatus.ClientID);
                            writer.WriteAttribute("title", loginStatus.ToolTip);
                            writer.WriteAttribute("class", className);
                            writer.WriteAttribute("href", System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.GetPostBackClientHyperlink(loginStatus.FindControl("ctl00"), ""));
                            writer.Write(HtmlTextWriter.TagRightChar);
                            writer.Write(loginStatus.LogoutText);
                            writer.WriteEndTag("a");
                            System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(ctl.UniqueID);
                        }
                    }
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
    }
}
