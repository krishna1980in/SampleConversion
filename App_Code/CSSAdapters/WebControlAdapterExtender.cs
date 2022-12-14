using System;
using System.Collections.Generic;
using System.Reflection;

using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Kartris
{
    public class WebControlAdapterExtender
    {
        private WebControl _adaptedControl = null;
        public WebControl AdaptedControl
        {
            get
            {
                System.Diagnostics.Debug.Assert(!Information.IsNothing(_adaptedControl), "CSS Friendly adapters internal error", "No control has been defined for the adapter extender");
                return _adaptedControl;
            }
        }

        public bool AdapterEnabled
        {
            get
            {
                bool bReturn = true;  // normally the adapters are enabled

                // Individual controls can use the expando property called AdapterEnabled
                // as a way to turn the adapters off.
                // <asp:TreeView runat="server" AdapterEnabled="false" />

                if (((!Information.IsNothing(AdaptedControl)) && (!string.IsNullOrEmpty(AdaptedControl.Attributes["AdapterEnabled"])) && (AdaptedControl.Attributes["AdapterEnabled"].IndexOf("false", StringComparison.OrdinalIgnoreCase) == 0)))
                    bReturn = false;

                return bReturn;
            }
        }

        private bool _disableAutoAccessKey = false;  // used when dealing with things like read-only textboxes that should not have access keys
        public bool AutoAccessKey
        {
            get
            {
                // Individual controls can use the expando property called AdapterEnabled
                // as a way to turn on/off the heurisitic for automatically setting the AccessKey
                // attribute in the rendered HTML.  The default is shown below in the initialization
                // of the bReturn variable.
                // <asp:TreeView runat="server" AutoAccessKey="false" />

                bool bReturn = true;  // by default, the adapter will make access keys are available
                if ((_disableAutoAccessKey || ((!Information.IsNothing(AdaptedControl)) && (!string.IsNullOrEmpty(AdaptedControl.Attributes["AutoAccessKey"])) && (AdaptedControl.Attributes["AutoAccessKey"].IndexOf("false", StringComparison.OrdinalIgnoreCase) == 0))))
                    bReturn = false;

                return bReturn;
            }
        }

        public WebControlAdapterExtender(WebControl adaptedControl)
        {
            _adaptedControl = adaptedControl;
        }

        public void RegisterScripts()
        {
            string folderPath = WebConfigurationManager.AppSettings.Get("CSSFriendly-JavaScript-Path");
            if ((string.IsNullOrEmpty(folderPath)))
                folderPath = "~/JavaScript";

            string filePath = Interaction.IIf(folderPath.EndsWith("/"), folderPath + "AdapterUtils.js", folderPath + "/AdapterUtils.js");
            AdaptedControl.Page.ClientScript.RegisterClientScriptInclude(this.GetType(), this.GetType().ToString(), AdaptedControl.Page.ResolveUrl(filePath));
        }

        public string ResolveUrl(string url)
        {
            string urlToResolve = url;
            int nPound = url.LastIndexOf("#");
            int nSlash = url.LastIndexOf("/");
            if (((nPound > -1) && (nSlash > -1) && ((nSlash + 1) == nPound)))
                // We have been given a somewhat strange URL.  It has a foreward slash (/) immediately followed
                // by a pound sign (#) like this xxx/#yyy.  This sort of oddly shaped URL is what you get when
                // you use named anchors instead of pages in the url attribute of a sitemapnode in an ASP.NET
                // sitemap like this:
                // 
                // <siteMapNode url="#Introduction" title="Introduction"  description="Introduction" />
                // 
                // The intend of the sitemap author is clearly to create a link to a named anchor in the page
                // that looks like these:
                // 
                // <a id="Introduction"></a>       (XHTML 1.1 Strict compliant)
                // <a name="Introduction"></a>     (more old fashioned but quite common in many pages)
                // 
                // However, the sitemap interpretter in ASP.NET doesn't understand url values that start with
                // a pound.  It prepends the current site's path in front of it before making it into a link
                // (typically for a TreeView or Menu).  We'll undo that problem, however, by converting this
                // sort of oddly shaped URL back into what was intended: a simple reference to a named anchor
                // that is expected to be within the current page.

                urlToResolve = url.Substring(nPound);
            else
                urlToResolve = AdaptedControl.ResolveClientUrl(urlToResolve);

            // And, just to be safe, we'll make sure there aren't any troublesome characters in whatever URL
            // we have decided to use at this point.
            string NewUrl = AdaptedControl.Page.Server.HtmlEncode(urlToResolve);

            return NewUrl;
        }

        public void RaiseAdaptedEvent(string eventName, EventArgs e)
        {
            string attr = "OnAdapted" + eventName;
            if (((!Information.IsNothing(AdaptedControl)) && (!string.IsNullOrEmpty(AdaptedControl.Attributes[attr]))))
            {
                string delegateName = AdaptedControl.Attributes[attr];
                Control methodOwner = AdaptedControl.Parent;
                MethodInfo method = methodOwner.GetType().GetMethod(delegateName);
                if ((Information.IsNothing(method)))
                {
                    methodOwner = AdaptedControl.Page;
                    method = methodOwner.GetType().GetMethod(delegateName);
                }
                if ((!Information.IsNothing(method)))
                {
                    object[] args = new object[2] { };
                    args[0] = AdaptedControl;
                    args[1] = e;
                    method.Invoke(methodOwner, args);
                }
            }
        }

        public void RenderBeginTag(HtmlTextWriter writer, string cssClass)
        {
            string id = "";
            if ((!Information.IsNothing(AdaptedControl)))
                id = AdaptedControl.ClientID;

            if ((!string.IsNullOrEmpty(AdaptedControl.Attributes["CssSelectorClass"])))
            {
                WriteBeginDiv(writer, AdaptedControl.Attributes["CssSelectorClass"], id);
                id = "";
            }

            WriteBeginDiv(writer, cssClass, id);
        }

        public void RenderEndTag(HtmlTextWriter writer)
        {
            WriteEndDiv(writer);

            if ((!string.IsNullOrEmpty(AdaptedControl.Attributes["CssSelectorClass"])))
                WriteEndDiv(writer);
        }

        public static void RemoveProblemChildren(Control ctrl, List<ControlRestorationInfo> stashedControls)
        {
            RemoveProblemTypes(ctrl.Controls, stashedControls);
        }

        public static void RemoveProblemTypes(ControlCollection coll, List<ControlRestorationInfo> stashedControls)
        {
            Control ctrl;
            foreach (var ctrl in coll)
            {
                if (((ctrl is RequiredFieldValidator) || (ctrl is CompareValidator) || (ctrl is RegularExpressionValidator) || (ctrl is ValidationSummary)))
                {
                    ControlRestorationInfo cri = new ControlRestorationInfo(ctrl, coll);
                    stashedControls.Add(cri);
                    coll.Remove(ctrl);
                    continue;
                }

                if ((ctrl.HasControls()))
                    RemoveProblemTypes(ctrl.Controls, stashedControls);
            }
        }

        public static void RestoreProblemChildren(List<ControlRestorationInfo> stashedControls)
        {
            ControlRestorationInfo cri;
            foreach (var cri in stashedControls)
                cri.Restore();
        }

        public string MakeChildId(string postfix)
        {
            return AdaptedControl.ClientID + "_" + postfix;
        }

        public static string MakeNameFromId(string id)
        {
            string name = "";
            int i;
            for (i = 0; i <= id.Length - 1; i++)
            {
                char thisChar = id[i];
                char prevChar = ' ';
                if (((i - 1) > -1))
                    prevChar = id[i - 1];
                char nextChar = ' ';
                if (((i + 1) < id.Length))
                    nextChar = id[i + 1];
                if (thisChar == '_')
                {
                    if (prevChar == '_')
                        name += "_";
                    else if (nextChar == '_')
                        name += "$_";
                    else
                        name += "$";
                }
                else
                    name += thisChar;
            }
            return name;
        }

        public static string MakeIdWithButtonType(string id, ButtonType type)
        {
            string idWithType = id;
            switch (type)
            {
                case ButtonType.Button:
                    {
                        idWithType += "Button";
                        break;
                        break;
                    }

                case ButtonType.Image:
                    {
                        idWithType += "ImageButton";
                        break;
                        break;
                    }

                case ButtonType.Link:
                    {
                        idWithType += "LinkButton";
                        break;
                        break;
                    }
            }
            return idWithType;
        }

        public string MakeChildName(string postfix)
        {
            return MakeNameFromId(MakeChildId(postfix));
        }

        public static void WriteBeginDiv(HtmlTextWriter writer, string className, string id)
        {
            writer.WriteLine();
            writer.WriteBeginTag("div");
            if ((!string.IsNullOrEmpty(className)))
                writer.WriteAttribute("class", className);
            if ((!string.IsNullOrEmpty(id)))
                writer.WriteAttribute("id", id);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Indent = writer.Indent + 1;
        }

        public static void WriteEndDiv(HtmlTextWriter writer)
        {
            writer.Indent = writer.Indent - 1;
            writer.WriteLine();
            writer.WriteEndTag("div");
        }

        public static void WriteSpan(HtmlTextWriter writer, string className, string content)
        {
            if ((!string.IsNullOrEmpty(content)))
            {
                writer.WriteLine();
                writer.WriteBeginTag("span");
                if ((!string.IsNullOrEmpty(className)))
                    writer.WriteAttribute("class", className);
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Write(content);
                writer.WriteEndTag("span");
            }
        }

        public static void WriteImage(HtmlTextWriter writer, string url, string alt)
        {
            if ((!string.IsNullOrEmpty(url)))
            {
                writer.WriteLine();
                writer.WriteBeginTag("img");
                writer.WriteAttribute("src", url);
                writer.WriteAttribute("alt", alt);
                writer.Write(HtmlTextWriter.SelfClosingTagEnd);
            }
        }

        public static void WriteLink(HtmlTextWriter writer, string className, string url, string title, string content)
        {
            if (((!string.IsNullOrEmpty(url)) && (!string.IsNullOrEmpty(content))))
            {
                writer.WriteLine();
                writer.WriteBeginTag("a");
                if ((!string.IsNullOrEmpty(className)))
                    writer.WriteAttribute("class", className);
                writer.WriteAttribute("href", url);
                writer.WriteAttribute("title", title);
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Write(content);
                writer.WriteEndTag("a");
            }
        }

        // Can't be static because it uses MakeChildId
        public void WriteLabel(HtmlTextWriter writer, string className, string text, string forId)
        {
            if ((!string.IsNullOrEmpty(text)))
            {
                writer.WriteLine();
                writer.WriteBeginTag("label");
                writer.WriteAttribute("for", MakeChildId(forId));
                if ((!string.IsNullOrEmpty(className)))
                    writer.WriteAttribute("class", className);
                writer.Write(HtmlTextWriter.TagRightChar);

                if ((AutoAccessKey))
                {
                    writer.WriteBeginTag("em");
                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.Write(text[0].ToString());
                    writer.WriteEndTag("em");
                    if ((!string.IsNullOrEmpty(text)))
                        writer.Write(text.Substring(1));
                }
                else
                    writer.Write(text);

                writer.WriteEndTag("label");
            }
        }

        // Can't be static because it uses MakeChildId
        public void WriteTextBox(HtmlTextWriter writer, bool isPassword, string labelClassName, string labelText, string inputClassName, string id, string value)
        {
            WriteLabel(writer, labelClassName, labelText, id);

            writer.WriteLine();
            writer.WriteBeginTag("input");
            writer.WriteAttribute("type", Interaction.IIf(isPassword, "password", "text"));
            if ((!string.IsNullOrEmpty(inputClassName)))
                writer.WriteAttribute("class", inputClassName);
            writer.WriteAttribute("id", MakeChildId(id));
            writer.WriteAttribute("name", MakeChildName(id));
            writer.WriteAttribute("value", value);
            if ((AutoAccessKey && (!string.IsNullOrEmpty(labelText))))
                writer.WriteAttribute("accesskey", labelText[0].ToString().ToLower());
            writer.Write(HtmlTextWriter.SelfClosingTagEnd);
        }

        // Can't be static because it uses MakeChildId
        public void WriteReadOnlyTextBox(HtmlTextWriter writer, string labelClassName, string labelText, string inputClassName, string value)
        {
            bool oldDisableAutoAccessKey = _disableAutoAccessKey;
            _disableAutoAccessKey = true;

            WriteLabel(writer, labelClassName, labelText, "");

            writer.WriteLine();
            writer.WriteBeginTag("input");
            writer.WriteAttribute("readonly", "readonly");
            if ((!string.IsNullOrEmpty(inputClassName)))
                writer.WriteAttribute("class", inputClassName);
            writer.WriteAttribute("value", value);
            writer.Write(HtmlTextWriter.SelfClosingTagEnd);

            _disableAutoAccessKey = oldDisableAutoAccessKey;
        }

        // Can't be static because it uses MakeChildId
        public void WriteCheckBox(HtmlTextWriter writer, string labelClassName, string labelText, string inputClassName, string id, bool isChecked)
        {
            writer.WriteLine();
            writer.WriteBeginTag("input");
            writer.WriteAttribute("type", "checkbox");
            if ((!string.IsNullOrEmpty(inputClassName)))
                writer.WriteAttribute("class", inputClassName);
            writer.WriteAttribute("id", MakeChildId(id));
            writer.WriteAttribute("name", MakeChildName(id));
            if ((isChecked))
                writer.WriteAttribute("checked", "checked");
            if ((AutoAccessKey && (!string.IsNullOrEmpty(labelText))))
                writer.WriteAttribute("accesskey", labelText[0].ToString().ToLower());
            writer.Write(HtmlTextWriter.SelfClosingTagEnd);

            WriteLabel(writer, labelClassName, labelText, id);
        }

        // Can't be static because it uses MakeChildId
        public void WriteSubmit(HtmlTextWriter writer, ButtonType buttonType, string className, string id, string imageUrl, string javascript, string text)
        {
            writer.WriteLine();

            string idWithType = id;

            switch (buttonType)
            {
                case buttonType.Button:
                    {
                        writer.WriteBeginTag("input");
                        writer.WriteAttribute("type", "submit");
                        writer.WriteAttribute("value", text);
                        idWithType += "Button";
                        break;
                        break;
                    }

                case buttonType.Image:
                    {
                        writer.WriteBeginTag("input");
                        writer.WriteAttribute("type", "image");
                        writer.WriteAttribute("src", imageUrl);
                        idWithType += "ImageButton";
                        break;
                        break;
                    }

                case buttonType.Link:
                    {
                        writer.WriteBeginTag("a");
                        idWithType += "LinkButton";
                        break;
                        break;
                    }
            }

            if ((!string.IsNullOrEmpty(className)))
                writer.WriteAttribute("class", className);
            writer.WriteAttribute("id", MakeChildId(idWithType));
            writer.WriteAttribute("name", MakeChildName(idWithType));

            if ((!string.IsNullOrEmpty(javascript)))
            {
                string pureJS = javascript;
                if ((pureJS.StartsWith("javascript:")))
                    pureJS = pureJS.Substring("javascript:".Length);
                switch (buttonType)
                {
                    case buttonType.Button:
                        {
                            writer.WriteAttribute("onclick", pureJS);
                            break;
                            break;
                        }

                    case buttonType.Image:
                        {
                            writer.WriteAttribute("onclick", pureJS);
                            break;
                            break;
                        }

                    case buttonType.Link:
                        {
                            writer.WriteAttribute("href", javascript);
                            break;
                            break;
                        }
                }
            }

            if ((buttonType == buttonType.Link))
            {
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Write(text);
                writer.WriteEndTag("a");
            }
            else
                writer.Write(HtmlTextWriter.SelfClosingTagEnd);
        }

        public static void WriteRequiredFieldValidator(HtmlTextWriter writer, RequiredFieldValidator rfv, string className, string controlToValidate, string msg)
        {
            if ((!Information.IsNothing(rfv)))
            {
                rfv.CssClass = className;
                rfv.ControlToValidate = controlToValidate;
                rfv.ErrorMessage = msg;
                rfv.RenderControl(writer);
            }
        }

        public static void WriteRegularExpressionValidator(HtmlTextWriter writer, RegularExpressionValidator rev, string className, string controlToValidate, string msg, string expression)
        {
            if ((!Information.IsNothing(rev)))
            {
                rev.CssClass = className;
                rev.ControlToValidate = controlToValidate;
                rev.ErrorMessage = msg;
                rev.ValidationExpression = expression;
                rev.RenderControl(writer);
            }
        }

        public static void WriteCompareValidator(HtmlTextWriter writer, CompareValidator cv, string className, string controlToValidate, string msg, string controlToCompare)
        {
            if ((!Information.IsNothing(cv)))
            {
                cv.CssClass = className;
                cv.ControlToValidate = controlToValidate;
                cv.ErrorMessage = msg;
                cv.ControlToCompare = controlToCompare;
                cv.RenderControl(writer);
            }
        }

        public static void WriteTargetAttribute(HtmlTextWriter writer, string targetValue)
        {
            if (((!Information.IsNothing(writer)) && (!string.IsNullOrEmpty(targetValue))))
            {
                // If the targetValue is _blank then we have an opportunity to use attributes other than "target"
                // which allows us to be compliant at the XHTML 1.1 Strict level. Specifically, we can use a combination
                // of "onclick" and "onkeypress" to achieve what we want to achieve when we used to render
                // target='blank'.
                // 
                // If the targetValue is other than _blank then we fall back to using the "target" attribute.
                // This is a heuristic that can be refined over time.
                if ((targetValue.Equals("_blank", StringComparison.OrdinalIgnoreCase)))
                {
                    string js = "window.open(this.href, '_blank', ''); return false;";
                    writer.WriteAttribute("onclick", js);
                    writer.WriteAttribute("onkeypress", js);
                }
                else
                    writer.WriteAttribute("target", targetValue);
            }
        }
    }



    public class ControlRestorationInfo
    {
        private Control _ctrl = null;
        public Control Control
        {
            get
            {
                return _ctrl;
            }
        }

        private ControlCollection _coll = null;
        public ControlCollection Collection
        {
            get
            {
                return _coll;
            }
        }

        public bool IsValid
        {
            get
            {
                return (!Information.IsNothing(Control)) && (!Information.IsNothing(Collection));
            }
        }

        public ControlRestorationInfo(Control ctrl, ControlCollection coll)
        {
            _ctrl = ctrl;
            _coll = coll;
        }

        public void Restore()
        {
            if ((IsValid))
                _coll.Add(_ctrl);
        }
    }
}
