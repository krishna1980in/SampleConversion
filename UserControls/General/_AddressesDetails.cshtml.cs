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
using KartrisClasses;
using CkartrisEnumerations;

partial class UserControls_General_AddressesDetails : System.Web.UI.UserControl
{

    /// <summary>
    ///     ''' Event to indicate data updated, channelled upwards to show the 'updated' animation
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public event _UCEvent_DataUpdatedEventHandler _UCEvent_DataUpdated;

    public delegate void _UCEvent_DataUpdatedEventHandler();

    /// <summary>
    ///     ''' Page load
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (Strings.Trim(System.Web.UI.UserControl.Request.QueryString["CustomerID"]) != "")
        {
            try
            {
                int U_ID = System.Convert.ToInt32(System.Web.UI.UserControl.Request.QueryString["CustomerID"]);
                UsersBLL objUsersBLL = new UsersBLL();
                hidUserID.Value = U_ID;
                // retrieve address list based on the current customer ID and the set adddress type
                DataTable dtUserDetails = objUsersBLL._GetAddressesByUserID(U_ID, hidDisplayAddressType.Value);
                rptrUserAddresses.DataSource = dtUserDetails;
                rptrUserAddresses.DataBind();
            }
            catch (Exception ex)
            {
            }
        }
    }

    /// <summary>
    ///     ''' Determines the address type to display
    ///     ''' </summary>
    ///     ''' <value>value should either be "s" for shipping or "b" for billing</value>
    ///     ''' <remarks></remarks>
    public string AddressType
    {
        set
        {
            hidDisplayAddressType.Value = value;
        }
    }

    /// <summary>
    ///     ''' Binding addresses to repeater
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private void rptrUserAddresses_ItemDataBound(object Sender, RepeaterItemEventArgs e)
    {
        if ((e.Item.ItemType == ListItemType.Item) | (e.Item.ItemType == ListItemType.AlternatingItem))
        {
            Literal litCountry = (Literal)e.Item.FindControl("litCountry");
            if (litCountry != null)
            {
                try
                {
                    // Replace Country ID with proper Country Name
                    litCountry.Text = Country.Get(System.Convert.ToInt32(litCountry.Text)).Name;
                }
                catch (Exception ex)
                {
                }
            }
        }
    }

    /// <summary>
    ///     ''' Handles clicks on Edit or Delete links on addresses
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void LinkButton_Command(object Sender, CommandEventArgs e)
    {
        var strCommandName = e.CommandName;
        int numCommandArgument = e.CommandArgument;
        if (strCommandName.ToLower == "edit")
        {
            ResetAddressInput();
            KartrisClasses.Address objAddress = KartrisClasses.Address.Get(numCommandArgument);
            UC_NewEditAddress.InitialAddressToDisplay = objAddress;
            // pnlNewAddress.Visible = True
            popExtender.Show();
        }
        else if (strCommandName.ToLower == "delete")
        {
            hidAddressToDeleteID.Value = numCommandArgument;
            _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.Confirmation, System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_ConfirmDeleteItemUnspecified"));
        }
        else
        {
        }
    }

    /// <summary>
    ///     ''' Add new address
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void NewAddress()
    {
        ResetAddressInput();
        popExtender.Show();
    }


    /// <summary>
    ///     ''' Submit/save address 
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void btnSaveNewAddress_Click()
    {
        int intGeneratedAddressID = Address.AddUpdate(UC_NewEditAddress.EnteredAddress, hidUserID.Value, null/* Conversion error: Set to default value for this argument */, UC_NewEditAddress.EnteredAddress.ID);
        UsersBLL objUsersBLL = new UsersBLL();
        DataTable dtUserDetails = objUsersBLL._GetAddressesByUserID(hidUserID.Value, hidDisplayAddressType.Value);
        rptrUserAddresses.DataSource = dtUserDetails;
        rptrUserAddresses.DataBind();
        _UCEvent_DataUpdated?.Invoke();
    }

    /// <summary>
    ///     ''' Confirms deleting an address
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    protected void _UC_PopupMsg_Confirmed()
    {
        KartrisClasses.Address.Delete(hidAddressToDeleteID.Value, hidUserID.Value);
        UsersBLL objUsersBLL = new UsersBLL();
        DataTable dtUserDetails = objUsersBLL._GetAddressesByUserID(hidUserID.Value, hidDisplayAddressType.Value);
        rptrUserAddresses.DataSource = dtUserDetails;
        rptrUserAddresses.DataBind();
        _UCEvent_DataUpdated?.Invoke();
    }

    /// <summary>
    ///     ''' Cancel popup
    ///     ''' </summary>
    ///     ''' <remarks>Need this for the delete popup, because the popExtender is linked to the edit address panel cancel</remarks>
    protected void _UC_PopupMsg_Cancelled()
    {
    }

    /// <summary>
    ///     ''' Resets the address form
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private void ResetAddressInput()
    {
        UC_NewEditAddress.Clear();
    }
}
