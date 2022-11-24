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

partial class UserControls_Front_NavigationMenu : System.Web.UI.UserControl
{
    protected void menFrontEnd_MenuItemDataBound(object sender, MenuEventArgs e)
    {
        SiteMapNode node = e.Item.DataItem;
        switch (node.Item("Value"))
        {
            case "support":
                {
                    if (KartSettingsManager.GetKartConfig("frontend.knowledgebase.enabled") != "y" && KartSettingsManager.GetKartConfig("frontend.supporttickets.enabled") != "y")
                    {
                        try
                        {
                            menFrontEnd.Items.Remove(e.Item);
                        }
                        catch (Exception ex)
                        {
                        }
                        try
                        {
                            e.Item.Parent.ChildItems.Remove(e.Item);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    break;
                }

            case "news":
                {
                    if (KartSettingsManager.GetKartConfig("frontend.navigationmenu.sitenews") != "y")
                    {
                        try
                        {
                            menFrontEnd.Items.Remove(e.Item);
                        }
                        catch (Exception ex)
                        {
                        }
                        try
                        {
                            e.Item.Parent.ChildItems.Remove(e.Item);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    break;
                }

            case "promotions":
                {
                    if (KartSettingsManager.GetKartConfig("frontend.promotions.enabled") != "y")
                    {
                        try
                        {
                            menFrontEnd.Items.Remove(e.Item);
                        }
                        catch (Exception ex)
                        {
                        }
                        try
                        {
                            e.Item.Parent.ChildItems.Remove(e.Item);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    break;
                }

            case "comparison":
                {
                    if (KartSettingsManager.GetKartConfig("frontend.products.comparison.enabled") != "y")
                    {
                        try
                        {
                            menFrontEnd.Items.Remove(e.Item);
                        }
                        catch (Exception ex)
                        {
                        }
                        try
                        {
                            e.Item.Parent.ChildItems.Remove(e.Item);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    break;
                }

            case "knowledgebase":
                {
                    if (KartSettingsManager.GetKartConfig("frontend.knowledgebase.enabled") != "y")
                    {
                        try
                        {
                            menFrontEnd.Items.Remove(e.Item);
                        }
                        catch (Exception ex)
                        {
                        }
                        try
                        {
                            e.Item.Parent.ChildItems.Remove(e.Item);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    break;
                }

            case "supporttickets":
                {
                    if (KartSettingsManager.GetKartConfig("frontend.supporttickets.enabled") != "y")
                    {
                        try
                        {
                            menFrontEnd.Items.Remove(e.Item);
                        }
                        catch (Exception ex)
                        {
                        }
                        try
                        {
                            e.Item.Parent.ChildItems.Remove(e.Item);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    break;
                }

            case "wishlist":
                {
                    if (KartSettingsManager.GetKartConfig("frontend.users.wishlists.enabled") != "y")
                    {
                        try
                        {
                            menFrontEnd.Items.Remove(e.Item);
                        }
                        catch (Exception ex)
                        {
                        }
                        try
                        {
                            e.Item.Parent.ChildItems.Remove(e.Item);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    break;
                }
        }
    }
}
