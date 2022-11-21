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
using CkartrisEnumerations;
using CkartrisDataManipulation;
using MailChimp.Net.Models;
using MailChimp.Net.Core;

partial class Admin_MailChimp : _PageBaseClass
{
    public event ShowMasterUpdateEventHandler ShowMasterUpdate;

    public delegate void ShowMasterUpdateEventHandler();

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = "MailChimp | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        // Fill config setting values in 
        txtCFG_Value1.Text = KartSettingsManager.GetKartConfig("general.mailchimp.apikey");
        txtCFG_Value2.Text = KartSettingsManager.GetKartConfig("general.mailchimp.apiurl");
        txtCFG_Value3.Text = KartSettingsManager.GetKartConfig("general.mailchimp.listid");
        txtCFG_Value4.Text = KartSettingsManager.GetKartConfig("general.mailchimp.storeid");

        txtCFG_Value5.Text = KartSettingsManager.GetKartConfig("general.mailchimp.mailinglistid");

        // Show status
        if (KartSettingsManager.GetKartConfig("general.mailchimp.enabled") == "y")
            litStatus.Text = "enabled";
        else
            litStatus.Text = "not enabled";
        // Only show the code to create a store if all the required
        // config settings are set
        if (txtCFG_Value1.Text == "" | txtCFG_Value2.Text == "" | txtCFG_Value3.Text == "" | txtCFG_Value4.Text == "")
            phdMailChimpAPI.Visible = false;
        else
        {
            phdMailChimpAPI.Visible = true;

            // Fill Up MailChimp Extra Fields
            try
            {
                if (!txtCFG_Value4.Text.Equals(""))
                {
                    MailChimpBLL manager = new MailChimpBLL();
                    Store store = manager.manager.ECommerceStores.GetAsync(txtCFG_Value4.Text).Result;
                    if (store != null)
                    {
                        txtStoreName.Text = store.Name;
                        txtStoreDomain.Text = store.Domain;
                        txtStoreEmail.Text = store.EmailAddress;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }

    protected void ShowMasterUpdateMessage()
    {
        (Skins_Admin_Template)this.Master.DataUpdated();
    }

    protected void lnkCreateStore_Click()
    {
        MailChimpBLL manager = new MailChimpBLL();
        try
        {
            string storeEmail = (txtStoreEmail.Text.Equals("")) ? null : txtStoreEmail.Text;
            string storeDomain = (txtStoreDomain.Text.Equals("")) ? null : txtStoreDomain.Text;
            string storeName = (txtStoreName.Text.Equals("")) ? null : txtStoreName.Text;
            Store store = manager.AddStore(txtCFG_Value4.Text, storeName, storeDomain, storeEmail).Result();
            ShowMasterUpdateMessage();
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                Type exceptionType = ex.InnerException.GetType();
                if (exceptionType.Name.Equals("MailChimpException"))
                {
                    MailChimpException mcException = (MailChimpException)ex.InnerException;
                    _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, mcException.Detail);
                }
                else
                    _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, "An error occured. Please check your MailChimp setup and your config setting values.");
            }
        }
    }
}
