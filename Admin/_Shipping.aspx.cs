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

partial class Admin_Shipping : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Shipping", "PageTitle_Shipping") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        if (!Page.IsPostBack)
            GetShippingSystemData();
    }

    public void GetShippingSystemData()
    {
        phdCalculation.Visible = false;

        // Set the integrated shipping options stuff
        // to hidden, activate only below if needed
        tabPnlShippingMethods.Visible = false;
        tabPnlShippingZones.Visible = false;

        // Have to do these too, but in ASP.net where
        // header text still shows on tab set to be
        // not visible
        litContentTextShippingMethods.Visible = false;
        litContentTextShippingZones.Visible = false;


        litShippingCalculated.Visible = true;
        litShippingCalculationDescription.Text = ConfigBLL._GetConfigDesc("frontend.checkout.shipping.calcbyweight");
        phdCalculation.Visible = true;
        if (KartSettingsManager.GetKartConfig("frontend.checkout.shipping.calcbyweight") == "y")
        {
            litShippingCalculated.Text = GetGlobalResourceObject("_Shipping", "ContentText_CalculateWeight");
            chkCalcByWeight.Checked = true;
        }
        else
        {
            litShippingCalculated.Text = GetGlobalResourceObject("_Shipping", "ContentText_CalculateOrderValue");
            chkCalcByWeight.Checked = false;
        }
        tabPnlShippingMethods.Visible = true;
        tabPnlShippingZones.Visible = true;

        // Have to do these too, but in ASP.net where
        // header text still shows on tab set to be
        // not visible
        litContentTextShippingMethods.Visible = true;
        litContentTextShippingZones.Visible = true;

        updCollapsiblePanel.Update();
    }

    private void HideShippingCalculationOptions()
    {
        CollapsiblePanelShippingCalculation.Collapsed = true;
        CollapsiblePanelShippingCalculation.ClientState = "true";

        lnkBtnChangeShippingCalculation.Visible = true;
        lnkBtnCancelShippingCalculation.Visible = false;

        updCollapsiblePanel.Update();
    }

    private void ShowShippingCalculationOptions()
    {
        CollapsiblePanelShippingCalculation.Collapsed = false;
        CollapsiblePanelShippingCalculation.ClientState = "false";

        lnkBtnChangeShippingCalculation.Visible = false;
        lnkBtnCancelShippingCalculation.Visible = true;

        updCollapsiblePanel.Update();
    }

    protected void lnkBtnChangeShippingCalculation_Click(object sender, System.EventArgs e)
    {
        ShowShippingCalculationOptions();
    }

    protected void lnkBtnCancelShippingCalculation_Click(object sender, System.EventArgs e)
    {
        HideShippingCalculationOptions();
    }

    protected void lnkBtnSaveShippingCalculation_Click(object sender, System.EventArgs e)
    {
        string strUseWeight = "n";
        if (chkCalcByWeight.Checked)
            strUseWeight = "y";
        if (ConfigBLL._UpdateConfigValue("frontend.checkout.shipping.calcbyweight", strUseWeight))
        {
            GetShippingSystemData();
            HideShippingCalculationOptions();
        }

        ShowMasterUpdateMessage();
    }

    protected void _UC_ShippingMethods_UpdateSaved()
    {
        ShowMasterUpdateMessage();
    }

    protected void _UC_ShippingZones_UpdateSaved()
    {
        ShowMasterUpdateMessage();
    }

    protected void ShowMasterUpdateMessage()
    {
        (Skins_Admin_Template)this.Master.DataUpdated();
    }
}
