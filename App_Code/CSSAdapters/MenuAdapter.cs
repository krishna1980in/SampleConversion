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

using System.Collections;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Kartris
{
    public class MenuAdapter : System.Web.UI.WebControls.Adapters.MenuAdapter
    {
        private WebControlAdapterExtender _extender = null/* TODO Change to default(_) if this is not a reference type */;
        private WebControlAdapterExtender Extender
        {
            get
            {
                if (((_extender == null) && (System.Web.UI.WebControls.Adapters.MenuAdapter.Control != null)) || ((_extender != null) && (System.Web.UI.WebControls.Adapters.MenuAdapter.Control != _extender.AdaptedControl)))
                    _extender = new WebControlAdapterExtender(System.Web.UI.WebControls.Adapters.MenuAdapter.Control);

                System.Diagnostics.Debug.Assert(_extender != null, "CSS Friendly adapters internal error", "Null extender instance");
                return _extender;
            }
        }

        protected new override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Extender.AdapterEnabled)
                RegisterScripts();
        }

        private void RegisterScripts()
        {
            Extender.RegisterScripts();

            string folderPath = WebConfigurationManager.AppSettings.Get("CSSFriendly-JavaScript-Path");
            if ((string.IsNullOrEmpty(folderPath)))
                folderPath = "~/JavaScript";

            string filePath = Interaction.IIf(folderPath.EndsWith("/"), folderPath + "MenuAdapter.js", folderPath + "/MenuAdapter.js");
            System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterClientScriptInclude(this.GetType(), this.GetType().ToString(), System.Web.UI.Adapters.ControlAdapter.Page.ResolveUrl(filePath));
        }

        protected new override void RenderBeginTag(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
                Extender.RenderBeginTag(writer, "KartrisMenu-" + System.Web.UI.WebControls.Adapters.MenuAdapter.Control.Orientation.ToString());
            else
                base.RenderBeginTag(writer);
        }

        protected new override void RenderEndTag(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
                Extender.RenderEndTag(writer);
            else
                base.RenderEndTag(writer);
        }

        protected new override void RenderContents(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
            {
                writer.Indent += 1;
                BuildItems(System.Web.UI.WebControls.Adapters.MenuAdapter.Control.Items, true, writer);
                writer.Indent -= 1;
                writer.WriteLine();
            }
            else
                base.RenderContents(writer);
        }

        private void BuildItems(MenuItemCollection items, bool isRoot, HtmlTextWriter writer)
        {
            if (items.Count > 0)
            {
                writer.WriteLine();

                writer.WriteBeginTag("ul");
                if (isRoot)
                    writer.WriteAttribute("class", "KartrisMenu");
                else
                    writer.WriteAttribute("class", "KartrisSubMenu dropdown");
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Indent += 1;

                foreach (MenuItem item in items)
                    BuildItem(item, writer);

                writer.Indent -= 1;
                writer.WriteLine();
                writer.WriteEndTag("ul");
            }
        }

        private void BuildItem(MenuItem item, HtmlTextWriter writer)
        {
            Menu menu = System.Web.UI.WebControls.Adapters.MenuAdapter.Control as Menu;
            if ((menu != null) && (item != null) && (writer != null))
            {
                writer.WriteLine();
                writer.WriteBeginTag("li");

                string theClass = Interaction.IIf((item.ChildItems.Count > 0), "KartrisMenu-WithChildren has-dropdown", "KartrisMenu-Leaf");
                string selectedStatusClass = GetSelectStatusClass(item);
                if (!String.IsNullOrEmpty(selectedStatusClass))
                    theClass += " " + selectedStatusClass;
                writer.WriteAttribute("class", theClass);

                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Indent += 1;
                writer.WriteLine();

                if (((item.Depth < menu.StaticDisplayLevels) && (menu.StaticItemTemplate != null)) || ((item.Depth >= menu.StaticDisplayLevels) && (menu.DynamicItemTemplate != null)))
                {
                    writer.WriteBeginTag("div");
                    writer.WriteAttribute("class", GetItemClass(menu, item));
                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.Indent += 1;
                    writer.WriteLine();

                    MenuItemTemplateContainer container = new MenuItemTemplateContainer(menu.Items.IndexOf(item), item);
                    // added to solve the <a href='<%# Eval("Text")%>'> binding problem
                    // http://forums.asp.net/t/1069719.aspx
                    // http://msdn2.microsoft.com/en-us/library/system.web.ui.control.bindingcontainer.aspx
                    // The BindingContainer property is the same as the NamingContainer property, 
                    // except when the control is part of a template. In that case, the BindingContainer 
                    // property is set to the Control that defines the template.
                    menu.Controls.Add(container);

                    if ((item.Depth < menu.StaticDisplayLevels) && (menu.StaticItemTemplate != null))
                        menu.StaticItemTemplate.InstantiateIn(container);
                    else
                        menu.DynamicItemTemplate.InstantiateIn(container);
                    container.DataBind();
                    // Databinding must occurs before rendering
                    container.RenderControl(writer);
                    writer.Indent -= 1;
                    writer.WriteLine();
                    writer.WriteEndTag("div");
                }
                else
                {
                    if (IsLink(item))
                    {
                        writer.WriteBeginTag("a");
                        if (!String.IsNullOrEmpty(item.NavigateUrl))
                            writer.WriteAttribute("href", System.Web.UI.Adapters.ControlAdapter.Page.Server.HtmlEncode(menu.ResolveClientUrl(item.NavigateUrl)));
                        else
                            writer.WriteAttribute("href", System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.GetPostBackClientHyperlink(menu, "b" + item.ValuePath.Replace(menu.PathSeparator.ToString(), @"\"), true));

                        writer.WriteAttribute("class", GetItemClass(menu, item));
                        WebControlAdapterExtender.WriteTargetAttribute(writer, item.Target);

                        if (!String.IsNullOrEmpty(item.ToolTip))
                            writer.WriteAttribute("title", item.ToolTip);
                        else if (!String.IsNullOrEmpty(menu.ToolTip))
                            writer.WriteAttribute("title", menu.ToolTip);
                        // Optionally, depending on the config settings, we
                        // can write a unique ID to each menu link. This makes
                        // it possible to write special CSS styles for any
                        // particular link in the menu. Keep turned off if not
                        // using this feature to keep page size down.
                        if (KartSettingsManager.GetKartConfig("frontend.navigationmenu.cssids") == "y")
                            writer.WriteAttribute("id", (((((item.Text).Replace(" ", "")).Replace("&", "")).Replace(",", "")).Replace("�", "o")).Replace("�", "O"));
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.Indent += 1;
                        writer.WriteLine();
                    }
                    else
                    {
                        writer.WriteBeginTag("span");
                        writer.WriteAttribute("class", GetItemClass(menu, item));
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.Indent += 1;
                        writer.WriteLine();
                    }

                    if (!String.IsNullOrEmpty(item.ImageUrl))
                    {
                        writer.WriteBeginTag("img");
                        writer.WriteAttribute("src", menu.ResolveClientUrl(item.ImageUrl));
                        writer.WriteAttribute("alt", Interaction.IIf(!String.IsNullOrEmpty(item.ToolTip), item.ToolTip, (Interaction.IIf(!String.IsNullOrEmpty(menu.ToolTip), menu.ToolTip, item.Text))));
                        writer.Write(HtmlTextWriter.SelfClosingTagEnd);
                    }

                    writer.Write(HttpUtility.HtmlEncode(item.Text));

                    if (IsLink(item))
                    {
                        writer.Indent -= 1;
                        writer.WriteEndTag("a");
                    }
                    else
                    {
                        writer.Indent -= 1;
                        writer.WriteEndTag("span");
                    }
                }

                if ((item.ChildItems != null) && (item.ChildItems.Count > 0))
                    BuildItems(item.ChildItems, false, writer);

                writer.Indent -= 1;
                writer.WriteLine();
                writer.WriteEndTag("li");
            }
        }

        protected virtual bool IsLink(MenuItem item)
        {
            return (item != null) && item.Enabled && ((!String.IsNullOrEmpty(item.NavigateUrl)) || item.Selectable);
        }

        private string GetItemClass(Menu menu, MenuItem item)
        {
            string value = "KartrisMenu-NonLink";
            if (item != null)
            {
                if (((item.Depth < menu.StaticDisplayLevels) && (menu.StaticItemTemplate != null)) || ((item.Depth >= menu.StaticDisplayLevels) && (menu.DynamicItemTemplate != null)))
                    value = "KartrisMenu-Template";
                else if (IsLink(item))
                    value = "KartrisMenu-Link";
                string selectedStatusClass = GetSelectStatusClass(item);
                if (!String.IsNullOrEmpty(selectedStatusClass))
                    value += " " + selectedStatusClass;
            }
            return value;
        }

        private string GetSelectStatusClass(MenuItem item)
        {
            string value = "";
            if (item.Selected)
                value += " KartrisMenu-Selected";
            else if (IsChildItemSelected(item))
                value += " KartrisMenu-ChildSelected";
            else if (IsParentItemSelected(item))
                value += " KartrisMenu-ParentSelected";
            return value;
        }

        private bool IsChildItemSelected(MenuItem item)
        {
            bool bRet = false;

            if ((item != null) && (item.ChildItems != null))
                bRet = IsChildItemSelected(item.ChildItems);

            return bRet;
        }

        private bool IsChildItemSelected(MenuItemCollection items)
        {
            bool bRet = false;

            if (items != null)
            {
                foreach (MenuItem item in items)
                {
                    if (item.Selected || IsChildItemSelected(item.ChildItems))
                    {
                        bRet = true;
                        break;
                    }
                }
            }

            return bRet;
        }

        private bool IsParentItemSelected(MenuItem item)
        {
            bool bRet = false;

            if ((item != null) && (item.Parent != null))
            {
                if (item.Parent.Selected)
                    bRet = true;
                else
                    bRet = IsParentItemSelected(item.Parent);
            }

            return bRet;
        }
    }
}
