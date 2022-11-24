// ========================================================================
// Kartris - www.kartris.com
// Copyright 2021 CACTUSOFT

// GNU GENERAL PUBLIC LICENSE v2
// This program is free software distributed under the GPL without any
// warranty.
// www.gnu.org/licenses/gpl-2.0.html

// KARTRIS COMMERCIAL LICENSE
// If a valid license.config issued by Cactusoft is present, the KCL
// overrides the GPL v2.
// www.kartris.com/t-Kartris-Commercial-License.aspx
// ========================================================================
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
using Microsoft.VisualBasic;

partial class UserControls_Skin_CategoryMenuAccordion : System.Web.UI.UserControl
{
    private long _lngCategoryID = 0;
    private string _strParent = "";

    public long RootCategoryID
    {
        get
        {
            return _lngCategoryID;
        }
        set
        {
            _lngCategoryID = value;
        }
    }
    public string CategoryParentString
    {
        get
        {
            return _strParent;
        }
        set
        {
            _strParent = value;
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {

        // We can try to build the accordion and set
        // the right pane open. But on canonical URLs
        // or pages accessed via URLs that don't give
        // hierarchy information, this would error
        // out. So put it in a 'try-catch'.
        try
        {
            SiteMapNode nodeStarting = null/* TODO Change to default(_) if this is not a reference type */;

            // Set starting node of sitemap
            // This allows the possibility of having two
            // or more menus, each starting from a top
            // level category.
            if (_lngCategoryID > 0)
            {
                nodeStarting = SiteMap.Providers("CategorySiteMapProvider").FindSiteMapNodeFromKey("0-" + CkartrisBLL.GetLanguageIDfromSession + "," + Interaction.IIf(_strParent != "", _strParent + ",", "") + _lngCategoryID);
                if (nodeStarting == null)
                    nodeStarting = SiteMap.RootNode;
            }
            else
                nodeStarting = SiteMap.RootNode;

            for (int numCounter = 0; numCounter <= nodeStarting.ChildNodes.Count - 1; numCounter++)
            {
                SiteMapNode nodSiteMap = (SiteMapNode)nodeStarting.ChildNodes(numCounter);
                AjaxControlToolkit.AccordionPane acpCategory = new AjaxControlToolkit.AccordionPane();
                int intCategory_CGID = System.Convert.ToInt32(nodSiteMap("CG_ID")); // Need to hide items user not allowed to see
                int intThisUser = 0;
                try
                {
                    intThisUser = (PageBaseClass)System.Web.UI.Control.Page.CurrentLoggedUser.CustomerGroupID;
                }
                catch (Exception ex)
                {
                }

                if (intThisUser == intCategory_CGID | intCategory_CGID == 0)
                {
                    // Each accordion pane needs a unique ID
                    acpCategory.ID = "Pane" + nodeStarting.ChildNodes(numCounter).Key;

                    HyperLink lnkCatName = new HyperLink();
                    lnkCatName.NavigateUrl = nodeStarting.ChildNodes(numCounter).Url.ToString();
                    lnkCatName.Text = nodeStarting.ChildNodes(numCounter).Title.ToString();

                    // link added to pane
                    acpCategory.HeaderContainer.Controls.Add(lnkCatName);

                    // produce list of children
                    BulletedList objMenu = new BulletedList();
                    objMenu.DisplayMode = BulletedListDisplayMode.HyperLink;

                    // If child nodes, then loop to produce submenu
                    if (nodSiteMap.HasChildNodes)
                    {

                        // since this pane has childnodes, blank the navigateurl to suppress postback when header is clicked
                        lnkCatName.NavigateUrl = "";

                        // items list added to menu
                        for (int numSubCounter = nodSiteMap.ChildNodes.Count - 1; numSubCounter >= 0; numSubCounter += -1)
                        {
                            int intCategory_sub_CGID = System.Convert.ToInt32(nodSiteMap.ChildNodes(numSubCounter)("CG_ID")); // Need to hide items user not allowed to see
                            if (intThisUser == intCategory_sub_CGID | intCategory_sub_CGID == 0)
                                objMenu.Items.Insert(0, (new ListItem(nodSiteMap.ChildNodes(numSubCounter).Title.ToString(), nodSiteMap.ChildNodes(numSubCounter).Url.ToString())));
                        }
                        // adds menu to container pane
                        acpCategory.ContentContainer.Controls.Add(objMenu);

                        // adds pane to accordion
                        accCategories.Panes.Add(acpCategory);
                    }


                    // adds menu to container pane
                    acpCategory.ContentContainer.Controls.Add(objMenu);

                    // adds pane to accordion
                    accCategories.Panes.Add(acpCategory);
                }
            }

            if (SiteMap.CurrentNode == null)
                accCategories.SelectedIndex = -1;
            else
                // get the top level node for the current node 
                if (SiteMap.CurrentNode != null)
            {
                SiteMapNode nodCurrentTopLevel = GetTopLevelNode(SiteMap.CurrentNode);

                // cycle through all accordine panes and open the one that matches the current top level node
                var intCount = 0;
                foreach (AjaxControlToolkit.AccordionPane pane in accCategories.Panes)
                {
                    if ((pane.ID == "Pane" + nodCurrentTopLevel.Key))
                    {
                        accCategories.SelectedIndex = intCount;
                        break;
                    }
                    intCount += 1;
                }
            }
        }
        catch (Exception ex)
        {
        }
    }

    private SiteMapNode GetTopLevelNode(SiteMapNode node)
    {
        SiteMapNode nodeStarting = null/* TODO Change to default(_) if this is not a reference type */;

        if (_lngCategoryID > 0)
        {
            nodeStarting = SiteMap.Providers("CategorySiteMapProvider").FindSiteMapNodeFromKey("0-" + CkartrisBLL.GetLanguageIDfromSession + "," + Interaction.IIf(_strParent != "", _strParent + ",", "") + _lngCategoryID);
            if (nodeStarting == null)
                nodeStarting = SiteMap.RootNode;
        }
        else
            nodeStarting = SiteMap.RootNode;

        if (_lngCategoryID == 0)
        {
            while (!node.ParentNode == nodeStarting)
                node = node.ParentNode;
        }
        else
            return nodeStarting;

        return node;
    }
}
