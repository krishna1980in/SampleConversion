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

using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


namespace Kartris
{
    public class TreeViewAdapter : System.Web.UI.WebControls.Adapters.HierarchicalDataBoundControlAdapter, IPostBackEventHandler, IPostBackDataHandler
    {
        private WebControlAdapterExtender _extender = null/* TODO Change to default(_) if this is not a reference type */;
        private int _checkboxIndex = 1;
        private HiddenField _viewState = null;
        private bool _updateViewState = false;
        private string _newViewState = "";

        public TreeViewAdapter() : base()
        {
            if ((_viewState == null))
                _viewState = new HiddenField();
        }

        protected WebControlAdapterExtender This = null/* TODO Change to default(_) if this is not a reference type */;
        private WebControlAdapterExtender Extender
        {
            get
            {
                if (((IsNothing(_extender) && (!Information.IsNothing(System.Web.UI.WebControls.Adapters.HierarchicalDataBoundControlAdapter.Control))) || ((!IsNothing(_extender)) && (!System.Web.UI.WebControls.Adapters.HierarchicalDataBoundControlAdapter.Control.Equals(_extender.AdaptedControl)))))
                    _extender = new WebControlAdapterExtender(System.Web.UI.WebControls.Adapters.HierarchicalDataBoundControlAdapter.Control);

                System.Diagnostics.Debug.Assert(!IsNothing(_extender), "CSS Friendly adapters internal error", "Null extender instance");
                return _extender;
            }
        }

        // Implementation of IPostBackDataHandler
        public virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            return true;
        }

        public virtual void RaisePostDataChangedEvent()
        {
            TreeView treeView = (TreeView)System.Web.UI.WebControls.Adapters.HierarchicalDataBoundControlAdapter.Control;
            if ((!(treeView) == null))
            {
                TreeNodeCollection items = treeView.Nodes;
                _checkboxIndex = 1;
                UpdateCheckmarks(items);
            }
        }

        // Implementation of IPostBackEventHandler
        public void RaisePostBackEvent(string eventArgument)
        {
            TreeView treeView = (TreeView)System.Web.UI.WebControls.Adapters.HierarchicalDataBoundControlAdapter.Control;
            if ((!(treeView) == null))
            {
                TreeNodeCollection items = treeView.Nodes;
                if ((!(eventArgument) == null))
                {
                    if ((eventArgument.StartsWith("s") || eventArgument.StartsWith("e")))
                    {
                        string selectedNodeValuePath = eventArgument.Substring(1).Replace(@"\", "/");
                        TreeNode selectedNode = treeView.FindNode(selectedNodeValuePath);
                        if ((!(selectedNode) == null))
                        {
                            bool bSelectedNodeChanged = !selectedNode.Equals(treeView.SelectedNode);
                            ClearSelectedNode(items);
                            selectedNode.Selected = true;
                            // does not raise the SelectedNodeChanged event so we have to do it manually (below).
                            if (eventArgument.StartsWith("e"))
                                selectedNode.Expand();
                            if (bSelectedNodeChanged)
                                Extender.RaiseAdaptedEvent("SelectedNodeChanged", new EventArgs());
                        }
                    }
                    else if (eventArgument.StartsWith("p"))
                    {
                        string parentNodeValuePath = eventArgument.Substring(1).Replace(@"\", "/");
                        TreeNode parentNode = treeView.FindNode(parentNodeValuePath);
                        if (((!parentNode == null) && ((parentNode.ChildNodes == null) || (parentNode.ChildNodes.Count == 0))))
                            parentNode.Expand();
                    }
                }
            }
        }

        protected override object SaveAdapterViewState()
        {
            string retStr = "";
            TreeView treeView = (TreeView)System.Web.UI.WebControls.Adapters.HierarchicalDataBoundControlAdapter.Control;
            if (((!(treeView) == null) && (!(_viewState) == null)))
            {
                if (((!(_viewState) == null) && (!(System.Web.UI.Adapters.ControlAdapter.Page) == null) && (!(System.Web.UI.Adapters.ControlAdapter.Page.Form) == null) && (!System.Web.UI.Adapters.ControlAdapter.Page.Form.Controls.Contains(_viewState))))
                {
                    Panel panel = new Panel();
                    panel.Controls.Add(_viewState);
                    System.Web.UI.Adapters.ControlAdapter.Page.Form.Controls.Add(panel);
                    string script = ("document.getElementById('"
                                + (_viewState.ClientID + ("').value = GetViewState__KartrisTreeView('"
                                + (Extender.MakeChildId("UL") + "');"))));
                    System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterOnSubmitStatement(this.GetType(), _viewState.ClientID, script);
                }
                retStr = (_viewState.UniqueID + ("|" + ComposeViewState(treeView.Nodes, "")));
            }
            return retStr;
        }

        protected override void LoadAdapterViewState(object state)
        {
            TreeView treeView = (TreeView)System.Web.UI.WebControls.Adapters.HierarchicalDataBoundControlAdapter.Control;
            string oldViewState = System.Convert.ToString(state);
            if (((!(treeView) == null) && (!(oldViewState) == null) && (oldViewState.Split(Microsoft.VisualBasic.ChrW(124)).Length == 2)))
            {
                string hiddenInputName = oldViewState.Split(Microsoft.VisualBasic.ChrW(124))(0);
                string oldExpansionState = oldViewState.Split(Microsoft.VisualBasic.ChrW(124))(1);
                if ((!treeView.ShowExpandCollapse))
                {
                    _newViewState = oldExpansionState;
                    _updateViewState = true;
                }
                else if ((!(System.Web.UI.Adapters.ControlAdapter.Page.Request.Form[hiddenInputName]) == null))
                {
                    _newViewState = System.Web.UI.Adapters.ControlAdapter.Page.Request.Form[hiddenInputName];
                    _updateViewState = true;
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            if (Extender.AdapterEnabled)
            {
                _updateViewState = false;
                _newViewState = "";
                TreeView treeView = (TreeView)System.Web.UI.WebControls.Adapters.HierarchicalDataBoundControlAdapter.Control;
                if ((!(treeView) == null))
                    treeView.EnableClientScript = false;
            }
            base.OnInit(e);
            if (Extender.AdapterEnabled)
                RegisterScripts();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            TreeView treeView = (TreeView)System.Web.UI.WebControls.Adapters.HierarchicalDataBoundControlAdapter.Control;
            if ((Extender.AdapterEnabled && _updateViewState && (!(treeView) == null)))
            {
                treeView.CollapseAll();
                ExpandToState(treeView.Nodes, _newViewState);
                _updateViewState = false;
            }
        }

        private void RegisterScripts()
        {
            Extender.RegisterScripts();

            string folderPath = WebConfigurationManager.AppSettings.Get("Kartris-JavaScript-Path");
            if ((string.IsNullOrEmpty(folderPath)))
                folderPath = "~/JavaScript";

            string filePath = Interaction.IIf(folderPath.EndsWith("/"), folderPath + "TreeViewAdapter.js", folderPath + "/TreeViewAdapter.js");
            System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterClientScriptInclude(this.GetType(), this.GetType().ToString(), System.Web.UI.Adapters.ControlAdapter.Page.ResolveUrl(filePath));
        }

        protected override void RenderBeginTag(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
                Extender.RenderBeginTag(writer, "Kartris-TreeView");
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
                TreeView treeView = (TreeView)System.Web.UI.WebControls.Adapters.HierarchicalDataBoundControlAdapter.Control;
                if ((!(treeView) == null))
                {
                    writer.Indent = (writer.Indent + 1);
                    _checkboxIndex = 1;
                    BuildItems(treeView.Nodes, true, true, writer);
                    writer.Indent = (writer.Indent - 1);
                    writer.WriteLine();
                }
            }
            else
                base.RenderContents(writer);
        }

        private void BuildItems(TreeNodeCollection items, bool isRoot, bool isExpanded, HtmlTextWriter writer)
        {
            if ((items.Count > 0))
            {
                writer.WriteLine();
                writer.WriteBeginTag("ul");
                if (isRoot)
                    writer.WriteAttribute("id", Extender.MakeChildId("UL"));
                if (!isExpanded)
                    writer.WriteAttribute("class", "Kartris-TreeView-Hide");
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Indent = (writer.Indent + 1);
                foreach (TreeNode item in items)
                    BuildItem(item, writer);
                writer.Indent = (writer.Indent - 1);
                writer.WriteLine();
                writer.WriteEndTag("ul");
            }
        }

        private void BuildItem(TreeNode item, HtmlTextWriter writer)
        {
            TreeView treeView = (TreeView)System.Web.UI.WebControls.Adapters.HierarchicalDataBoundControlAdapter.Control;
            if (((!(treeView) == null) && (!(item) == null) && (!(writer) == null)))
            {
                writer.WriteLine();
                writer.WriteBeginTag("li");
                writer.WriteAttribute("class", GetNodeClass(item));
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Indent = (writer.Indent + 1);
                writer.WriteLine();
                if ((IsExpandable(item) && treeView.ShowExpandCollapse))
                    WriteNodeExpander(treeView, item, writer);
                if (IsCheckbox(treeView, item))
                    WriteNodeCheckbox(treeView, item, writer);
                else if (IsLink(item))
                    WriteNodeLink(treeView, item, writer);
                else
                    WriteNodePlain(treeView, item, writer);
                if (HasChildren(item))
                    BuildItems(item.ChildNodes, false, item.Expanded.Equals(true), writer);
                writer.Indent = (writer.Indent - 1);
                writer.WriteLine();
                writer.WriteEndTag("li");
            }
        }

        private void WriteNodeExpander(TreeView treeView, TreeNode item, HtmlTextWriter writer)
        {
            writer.WriteBeginTag("span");
            writer.WriteAttribute("class", Interaction.IIf(item.Expanded.Equals(true), "Kartris-TreeView-Collapse", "Kartris-TreeView-Expand"));
            if (HasChildren(item))
                writer.WriteAttribute("onclick", "ExpandCollapse__KartrisTreeView(this)");
            else
                writer.WriteAttribute("onclick", System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.GetPostBackEventReference(treeView, ("p" + System.Web.UI.Adapters.ControlAdapter.Page.Server.HtmlEncode(item.ValuePath).Replace("/", @"\")), true));
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write("&nbsp;");
            writer.WriteEndTag("span");
            writer.WriteLine();
        }

        private void WriteNodeImage(TreeView treeView, TreeNode item, HtmlTextWriter writer)
        {
            string imgSrc = GetImageSrc(treeView, item);
            if ((!string.IsNullOrEmpty(imgSrc)))
            {
                writer.WriteBeginTag("img");
                writer.WriteAttribute("src", treeView.ResolveClientUrl(imgSrc));
                writer.WriteAttribute("alt", Interaction.IIf(!string.IsNullOrEmpty(item.ToolTip), item.ToolTip, Interaction.IIf(!string.IsNullOrEmpty(treeView.ToolTip), treeView.ToolTip, item.Text)));
                writer.Write(HtmlTextWriter.SelfClosingTagEnd);
            }
        }

        private void WriteNodeCheckbox(TreeView treeView, TreeNode item, HtmlTextWriter writer)
        {
            writer.WriteBeginTag("input");
            writer.WriteAttribute("type", "checkbox");
            writer.WriteAttribute("id", (treeView.ClientID + ("n"
                            + (_checkboxIndex.ToString() + "CheckBox"))));
            writer.WriteAttribute("name", (treeView.UniqueID + ("n"
                            + (_checkboxIndex.ToString() + "CheckBox"))));
            if ((!string.IsNullOrEmpty(treeView.Attributes["OnClientClickedCheckbox"])))
                writer.WriteAttribute("onclick", treeView.Attributes["OnClientClickedCheckbox"]);
            if (item.Checked)
                writer.WriteAttribute("checked", "checked");
            writer.Write(HtmlTextWriter.SelfClosingTagEnd);
            if ((!string.IsNullOrEmpty(item.Text)))
            {
                writer.WriteLine();
                writer.WriteBeginTag("label");
                writer.WriteAttribute("for", (treeView.ClientID + ("n"
                                + (_checkboxIndex.ToString() + "CheckBox"))));
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Write(item.Text);
                writer.WriteEndTag("label");
            }
            _checkboxIndex = (_checkboxIndex + 1);
        }

        private void WriteNodeLink(TreeView treeView, TreeNode item, HtmlTextWriter writer)
        {
            writer.WriteBeginTag("a");
            if ((!string.IsNullOrEmpty(item.NavigateUrl)))
                writer.WriteAttribute("href", Extender.ResolveUrl(item.NavigateUrl));
            else
            {
                string codePrefix = "";
                if ((item.SelectAction == TreeNodeSelectAction.Select))
                    codePrefix = "s";
                else if ((item.SelectAction == TreeNodeSelectAction.SelectExpand))
                    codePrefix = "e";
                else if (item.PopulateOnDemand)
                    codePrefix = "p";
                writer.WriteAttribute("href", System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.GetPostBackClientHyperlink(treeView, (codePrefix + System.Web.UI.Adapters.ControlAdapter.Page.Server.HtmlEncode(item.ValuePath).Replace("/", @"\")), true));
            }
            WebControlAdapterExtender.WriteTargetAttribute(writer, item.Target);
            if ((!string.IsNullOrEmpty(item.ToolTip)))
                writer.WriteAttribute("title", item.ToolTip);
            else if ((!string.IsNullOrEmpty(treeView.ToolTip)))
                writer.WriteAttribute("title", treeView.ToolTip);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Indent = (writer.Indent + 1);
            writer.WriteLine();
            WriteNodeImage(treeView, item, writer);
            writer.Write(item.Text);
            writer.Indent = (writer.Indent - 1);
            writer.WriteEndTag("a");
        }

        private void WriteNodePlain(TreeView treeView, TreeNode item, HtmlTextWriter writer)
        {
            writer.WriteBeginTag("span");
            if (IsExpandable(item))
            {
                writer.WriteAttribute("class", "Kartris-TreeView-ClickableNonLink");
                if ((treeView.ShowExpandCollapse))
                    writer.WriteAttribute("onclick", "ExpandCollapse__KartrisTreeView(this.parentNode.getElementsByTagName('span')[0])");
            }
            else
                writer.WriteAttribute("class", "Kartris-TreeView-NonLink");
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Indent = (writer.Indent + 1);
            writer.WriteLine();
            WriteNodeImage(treeView, item, writer);
            writer.Write(item.Text);
            writer.Indent = (writer.Indent - 1);
            writer.WriteEndTag("span");
        }

        private void UpdateCheckmarks(TreeNodeCollection items)
        {
            TreeView treeView = (TreeView)System.Web.UI.WebControls.Adapters.HierarchicalDataBoundControlAdapter.Control;
            if (((!(treeView) == null) && (!(items) == null)))
            {
                foreach (TreeNode item in items)
                {
                    if (IsCheckbox(treeView, item))
                    {
                        string name = (treeView.UniqueID + ("n"
+ (_checkboxIndex.ToString() + "CheckBox")));
                        bool bIsNowChecked = (!(System.Web.UI.Adapters.ControlAdapter.Page.Request.Form[name]) == null);
                        if ((item.Checked != bIsNowChecked))
                        {
                            item.Checked = bIsNowChecked;
                            Extender.RaiseAdaptedEvent("TreeNodeCheckChanged", new TreeNodeEventArgs(item));
                        }
                        _checkboxIndex = (_checkboxIndex + 1);
                    }
                    if (HasChildren(item))
                        UpdateCheckmarks(item.ChildNodes);
                }
            }
        }

        private bool IsLink(TreeNode item)
        {
            return ((!(item) == null) && ((!string.IsNullOrEmpty(item.NavigateUrl)) || (item.PopulateOnDemand) || (item.SelectAction == TreeNodeSelectAction.Select) || (item.SelectAction == TreeNodeSelectAction.SelectExpand)));
        }

        private bool IsCheckbox(TreeView treeView, TreeNode item)
        {
            bool bItemCheckBoxDisallowed = ((!Information.IsNothing(item.ShowCheckBox)) && (item.ShowCheckBox.Value == false));
            bool bItemCheckBoxWanted = ((!Information.IsNothing(item.ShowCheckBox)) && (item.ShowCheckBox.Value == true));
            bool bTreeCheckBoxWanted = (treeView.ShowCheckBoxes == TreeNodeTypes.All) || (((treeView.ShowCheckBoxes == TreeNodeTypes.Leaf) && !IsExpandable(item)) || ((treeView.ShowCheckBoxes == TreeNodeTypes.Parent) && IsExpandable(item)) || ((treeView.ShowCheckBoxes == TreeNodeTypes.Root) && (item.Depth == 0)));
            return (!bItemCheckBoxDisallowed && (bItemCheckBoxWanted || bTreeCheckBoxWanted));
        }

        private string GetNodeClass(TreeNode item)
        {
            string value = "Kartris-TreeView-Leaf";
            if ((!(item) == null))
            {
                if ((item.Depth == 0))
                {
                    if (IsExpandable(item))
                        value = "Kartris-TreeView-Root";
                    else
                        value = "Kartris-TreeView-Root Kartris-TreeView-Leaf";
                }
                else if (IsExpandable(item))
                    value = "Kartris-TreeView-Parent";
                if (item.Selected)
                    value = (value + " Kartris-TreeView-Selected");
                else if (IsChildNodeSelected(item))
                    value = (value + " Kartris-TreeView-ChildSelected");
                else if (IsParentNodeSelected(item))
                    value = (value + " Kartris-TreeView-ParentSelected");
            }
            return value;
        }

        private string GetImageSrc(TreeView treeView, TreeNode item)
        {
            string imgSrc = "";
            if (((!(treeView) == null) && (!(item) == null)))
            {
                imgSrc = item.ImageUrl;
                if ((string.IsNullOrEmpty(imgSrc)))
                {
                    if ((item.Depth == 0))
                    {
                        if (((!(treeView.RootNodeStyle) == null) && (!string.IsNullOrEmpty(treeView.RootNodeStyle.ImageUrl))))
                            imgSrc = treeView.RootNodeStyle.ImageUrl;
                    }
                    else if (!IsExpandable(item))
                    {
                        if (((!(treeView.LeafNodeStyle) == null) && (!string.IsNullOrEmpty(treeView.LeafNodeStyle.ImageUrl))))
                            imgSrc = treeView.LeafNodeStyle.ImageUrl;
                    }
                    else if (((!(treeView.ParentNodeStyle) == null) && (!string.IsNullOrEmpty(treeView.ParentNodeStyle.ImageUrl))))
                        imgSrc = treeView.ParentNodeStyle.ImageUrl;
                }
                if (((string.IsNullOrEmpty(imgSrc)) && (!(treeView.LevelStyles) == null) && (treeView.LevelStyles.Count > item.Depth)))
                {
                    if ((!string.IsNullOrEmpty(treeView.LevelStyles[item.Depth].ImageUrl)))
                        imgSrc = treeView.LevelStyles[item.Depth].ImageUrl;
                }
            }
            return imgSrc;
        }

        private bool HasChildren(TreeNode item)
        {
            return ((!(item) == null) && (!(item.ChildNodes) == null) && (item.ChildNodes.Count > 0));
        }

        private bool IsExpandable(TreeNode item)
        {
            return (HasChildren(item) || ((!(item) == null) && item.PopulateOnDemand));
        }

        private void ClearSelectedNode(TreeNodeCollection nodes)
        {
            if ((!(nodes) == null))
            {
                foreach (TreeNode node in nodes)
                {
                    if (node.Selected)
                        node.Selected = false;
                    if ((!(node.ChildNodes) == null))
                        ClearSelectedNode(node.ChildNodes);
                }
            }
        }

        private new bool IsChildNodeSelected(TreeNode item)
        {
            bool bRet = false;
            if (((!(item) == null) && (!(item.ChildNodes) == null)))
                bRet = IsChildNodeSelected(item.ChildNodes);
            return bRet;
        }

        private new bool IsChildNodeSelected(TreeNodeCollection nodes)
        {
            bool bRet = false;
            if ((!(nodes) == null))
            {
                foreach (TreeNode node in nodes)
                {
                    if ((node.Selected || IsChildNodeSelected(node.ChildNodes)))
                    {
                        bRet = true;
                        break;
                    }
                }
            }
            return bRet;
        }

        private bool IsParentNodeSelected(TreeNode item)
        {
            bool bRet = false;
            if (((!(item) == null) && (!(item.Parent) == null)))
            {
                if (item.Parent.Selected)
                    bRet = true;
                else
                    bRet = IsParentNodeSelected(item.Parent);
            }
            return bRet;
        }

        private string ComposeViewState(TreeNodeCollection nodes, string state)
        {
            if ((!(nodes) == null))
            {
                foreach (TreeNode node in nodes)
                {
                    if ((IsExpandable(node)))
                    {
                        if (node.Expanded.Equals(true))
                        {
                            state += "e";
                            state = ComposeViewState(node.ChildNodes, state);
                        }
                        else
                            state += "n";
                    }
                }
            }
            return state;
        }

        private string ExpandToState(TreeNodeCollection nodes, string state)
        {
            if (((!(nodes) == null) && (!string.IsNullOrEmpty(state))))
            {
                foreach (TreeNode node in nodes)
                {
                    if (IsExpandable(node))
                    {
                        bool bExpand = (state[0] == Microsoft.VisualBasic.ChrW(101));
                        state = state.Substring(1);
                        if (bExpand)
                        {
                            node.Expand();
                            state = ExpandToState(node.ChildNodes, state);
                        }
                    }
                }
            }
            return state;
        }

        public static void ExpandToDepth(TreeNodeCollection nodes, int expandDepth)
        {
            if ((!(nodes) == null))
            {
                foreach (TreeNode node in nodes)
                {
                    if ((node.Depth < expandDepth))
                    {
                        node.Expand();
                        ExpandToDepth(node.ChildNodes, expandDepth);
                    }
                }
            }
        }
    }
}
