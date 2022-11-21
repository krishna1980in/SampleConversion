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

partial class Admin_LoginsList : _PageBaseClass
{
    private static int intSelectedLoginID;
    private static bool Login_Protected;
    private static int LOGIN_LanguageID;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetLocalResourceObject("PageTitle_Logins") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");
    }

    protected void gvwLogins_RowEditing(object sender, GridViewEditEventArgs e)
    {
        hidLoginID.Value = e.NewEditIndex;
    }

    protected void gvwLogins_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        // hide delete button if login is protected
        if (e.Row.DataItem != null)
        {
            DataRowView LoginRow = (DataRowView)e.Row.DataItem;
            Login_Protected = LoginRow("LOGIN_Protected");
            if (Login_Protected)
                (LinkButton)e.Row.Cells(9).Controls(0).Visible = false;
        }
    }

    private void EditLogin(object src, GridViewCommandEventArgs e)
    {

        // get the row index stored in the CommandArgument property 
        int intRowIndex = Convert.ToInt32(e.CommandArgument);

        // get the GridViewRow where the command is raised 
        GridViewRow rowLogins = (GridView)e.CommandSource.Rows(intRowIndex);

        intSelectedLoginID = System.Convert.ToInt32((HiddenField)rowLogins.Cells(5).Controls(1).Value);
        Login_Protected = System.Convert.ToBoolean((HiddenField)rowLogins.Cells(6).Controls(1).Value);
        string LOGIN_UserName = rowLogins.Cells(0).Text;

        if (e.CommandName == "DeleteLogins")
        {
            if (!Login_Protected)
            {
                string strMessage = Replace(GetGlobalResourceObject("_Kartris", "ContentText_ConfirmDeleteItem"), "[itemname]", LOGIN_UserName);
                _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.Confirmation, strMessage);
            }
        }
        else
        {
            txtPassword.Visible = false;
            valRequiredUserPassword.Enabled = false;
            lblPassword.Visible = true;
            btnChangePassword.Text = GetGlobalResourceObject("_Kartris", "ContentText_ConfigChange2");
            btnChangePassword.Visible = true;

            EnableAll();
            string LOGIN_EmailAddress = rowLogins.Cells(1).Text;
            string LOGIN_Password = rowLogins.Cells(2).Text;
            CheckBox LOGIN_Config = rowLogins.Cells(3).Controls(1);
            CheckBox LOGIN_Products = rowLogins.Cells(3).Controls(3);
            CheckBox LOGIN_Orders = rowLogins.Cells(3).Controls(5);
            CheckBox LOGIN_Tickets = rowLogins.Cells(3).Controls(7);
            CheckBox LOGIN_Live = rowLogins.Cells(4).Controls(1);
            var LOGIN_PushNotifications = (HiddenField)rowLogins.Cells(8).Controls(1).Value;

            LOGIN_LanguageID = System.Convert.ToInt32((HiddenField)rowLogins.Cells(7).Controls(1).Value);


            radNo.Checked = !LOGIN_Live.Checked;
            radYes.Checked = LOGIN_Live.Checked;

            chkLoginEditConfig.Checked = LOGIN_Config.Checked;
            chkLoginEditProducts.Checked = LOGIN_Products.Checked;
            chkLoginEditOrders.Checked = LOGIN_Orders.Checked;
            chkLoginEditTickets.Checked = LOGIN_Tickets.Checked;

            if (Login_Protected)
            {
                chkLoginEditConfig.Enabled = false;
                chkLoginEditOrders.Enabled = false;
                chkLoginEditProducts.Enabled = false;
                chkLoginEditTickets.Enabled = false;
                radYes.Enabled = false;
                radNo.Enabled = false;
            }

            txtPassword.Text = LOGIN_Password;
            txtUsername.Text = LOGIN_UserName;
            txtEmailAddress.Text = LOGIN_EmailAddress;
            hidEditPushNotifications.Value = LOGIN_PushNotifications;


            XmlDocument xmlDoc = new XmlDocument();

            if (!string.IsNullOrEmpty(LOGIN_PushNotifications))
                xmlDoc.LoadXml(LOGIN_PushNotifications);
            BindXMLtoGridView(xmlDoc);

            ddlLanguages.SelectedValue = LOGIN_LanguageID;
            lnkNewLogin.Visible = false;
            // hide the gridview
            gvwLogins.Visible = false;
        }
    }

    protected void _UC_PopupMsg_Confirmed()
    {
        if (intSelectedLoginID == LoginsBLL.Delete(intSelectedLoginID))
        {
            gvwLogins.DataBind();
            updMain.Update();
        }
        else
            _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, GetGlobalResourceObject("_Kartris", "ContentText_Error"));
    }

    protected void btnCancel_Click(object sender, System.EventArgs e)
    {
        pnlEdit.Visible = false;
        lnkNewLogin.Visible = true;
        gvwLogins.Visible = true;
    }

    protected void lnkNewLogin_Click(object sender, System.EventArgs e)
    {
        EnableAll();
        lnkNewLogin.Visible = false;
        intSelectedLoginID = 0;
        Login_Protected = false;
        LOGIN_LanguageID = 1;

        radNo.Checked = true;
        radYes.Checked = false;
        chkLoginEditConfig.Checked = false;
        chkLoginEditProducts.Checked = false;
        chkLoginEditOrders.Checked = false;
        chkLoginEditTickets.Checked = false;
        btnChangePassword.Visible = false;

        txtPassword.Text = "";
        txtEmailAddress.Text = "";
        txtUsername.Text = "";
        ddlLanguages.SelectedValue = LOGIN_LanguageID;

        txtPassword.Visible = true;
        valRequiredUserPassword.Enabled = true;
        lblPassword.Visible = false;

        // hide the gridview
        gvwLogins.Visible = false;
    }

    private void EnableAll()
    {
        pnlEdit.Visible = true;
        chkLoginEditConfig.Enabled = true;
        chkLoginEditOrders.Enabled = true;
        chkLoginEditProducts.Enabled = true;
        chkLoginEditTickets.Enabled = true;
        radYes.Enabled = true;
        radNo.Enabled = true;
    }

    protected void btnUpdate_Click(object sender, System.EventArgs e)
    {
        if (UserNameExist())
            return;
        Page.Validate("UserDetails");
        if (Page.IsValid)
        {
            if (intSelectedLoginID == 0)
                LoginsBLL.Add(txtUsername.Text, txtPassword.Text, radYes.Checked, chkLoginEditOrders.Checked, chkLoginEditProducts.Checked, chkLoginEditConfig.Checked, Login_Protected, ddlLanguages.SelectedValue, txtEmailAddress.Text, chkLoginEditTickets.Checked, hidEditPushNotifications.Value);
            else if (intSelectedLoginID > 0)
            {
                string strPassword;
                if (txtPassword.Visible)
                    strPassword = txtPassword.Text;
                else
                    strPassword = "";
                LoginsBLL.Update(intSelectedLoginID, txtUsername.Text, strPassword, radYes.Checked, chkLoginEditOrders.Checked, chkLoginEditProducts.Checked, chkLoginEditConfig.Checked, Login_Protected, ddlLanguages.SelectedValue, txtEmailAddress.Text, chkLoginEditTickets.Checked, hidEditPushNotifications.Value);
                txtPassword.Visible = false;
                valRequiredUserPassword.Enabled = false;
                lblPassword.Visible = true;
                btnChangePassword.Text = GetGlobalResourceObject("_Kartris", "ContentText_ConfigChange2");
            }
            lnkNewLogin.Visible = true;
            pnlEdit.Visible = false;
            gvwLogins.Visible = true;
            lnkNewLogin.Visible = true;
            gvwLogins.DataBind();
            (Skins_Admin_Template)this.Master.DataUpdated();
        }
    }

    public bool UserNameExist()
    {
        foreach (GridViewRow row in gvwLogins.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                if (intSelectedLoginID != System.Convert.ToInt32((HiddenField)row.Cells(5).Controls(1).Value))
                {
                    if (row.Cells(0).Text.ToLower == txtUsername.Text.ToLower)
                    {
                        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, GetGlobalResourceObject("_Login", "ContentText_UserNameAlreadyExist"));
                        return true;
                    }
                }
            }
        }
        return false;
    }

    protected void btnChangePassword_Click(object sender, System.EventArgs e)
    {
        if (txtPassword.Visible)
        {
            txtPassword.Visible = false;
            valRequiredUserPassword.Enabled = false;
            lblPassword.Visible = true;
            btnChangePassword.Text = GetGlobalResourceObject("_Kartris", "ContentText_ConfigChange2");
        }
        else
        {
            txtPassword.Visible = true;
            valRequiredUserPassword.Enabled = true;
            lblPassword.Visible = false;
            btnChangePassword.Text = GetGlobalResourceObject("_Kartris", "FormButton_Cancel");
        }
    }
    protected void lnkNew_Click(object sender, System.EventArgs e)
    {
        txtDeviceLabel.Text = "";
        txtDeviceURI.Text = "";
        hidOrigDeviceLabel.Value = "";
        chkDeviceLive.Checked = true;
        litContentTextAddEditDevices.Text = GetGlobalResourceObject("_Kartris", "ContentText_AddNew");
        popPushNotification.Show();
    }
    private void EditDevice(object src, GridViewCommandEventArgs e)
    {
        litContentTextAddEditDevices.Text = GetGlobalResourceObject("_Kartris", "FormButton_Edit");

        // get the row index stored in the CommandArgument property 
        int intRowIndex = Convert.ToInt32(e.CommandArgument);

        // get the GridViewRow where the command is raised 
        GridViewRow rowLogins = (GridView)e.CommandSource.Rows(intRowIndex);

        string Device_Name = rowLogins.Cells(0).Text;

        // an existing device - search entry and update XML
        // retrieve push notification XML for this user
        string Login_Notifications = Trim(hidEditPushNotifications.Value);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(Login_Notifications);
        XmlNodeList xmlNodeList = xmlDoc.GetElementsByTagName("Device");
        foreach (XmlNode node in xmlNodeList)
        {
            if (node.ChildNodes(0).InnerText == Device_Name)
            {
                if (e.CommandName == "DeleteDevice")
                {
                    node.ParentNode.RemoveChild(node);
                    // Bind the new XML data to the push notifications gridview
                    DataSet ds = new DataSet();
                    ds.ReadXml(new XmlNodeReader(xmlDoc));
                    hidEditPushNotifications.Value = xmlDoc.OuterXml;

                    BindXMLtoGridView(xmlDoc);
                }
                else
                {
                    hidOrigDeviceLabel.Value = Device_Name;
                    txtDeviceLabel.Text = Device_Name;
                    txtDeviceURI.Text = node.ChildNodes(2).InnerText;
                    chkDeviceLive.Checked = node.ChildNodes(3).InnerText;

                    popPushNotification.Show();
                }
                break;
            }
        }
    }
    protected void btnSubmit_Click(object sender, System.EventArgs e)
    {
        Page.Validate("PushNotifications");
        if (IsValid)
        {
            // retrieve push notification XML for this user
            string Login_Notifications = Trim(hidEditPushNotifications.Value);
            XmlDocument xmlDoc = new XmlDocument();

            if (string.IsNullOrEmpty(Login_Notifications))
            {
                // this is first push notification device entry so still need to build up XML

                // XML declaration
                XmlNode declaration = xmlDoc.CreateNode(XmlNodeType.XmlDeclaration, null/* TODO Change to default(_) if this is not a reference type */, null/* TODO Change to default(_) if this is not a reference type */);
                xmlDoc.AppendChild(declaration);

                // Root element: PushNotifications
                XmlElement root = xmlDoc.CreateElement("PushNotifications");
                xmlDoc.AppendChild(root);

                // Sub-element: Device
                XmlElement Device = xmlDoc.CreateElement("Device");

                root.AppendChild(Device);

                SetAllChild(ref Device, true);
            }
            else
            {
                // xml structure is already present
                xmlDoc.LoadXml(Login_Notifications);

                if (string.IsNullOrEmpty(hidOrigDeviceLabel.Value))
                {
                    // new device so just need to add it to the XML
                    XmlElement root = xmlDoc.DocumentElement;
                    // Sub-element: Device
                    XmlElement Device = xmlDoc.CreateElement("Device");

                    SetAllChild(ref Device, true);

                    root.AppendChild(Device);
                }
                else
                {
                    // an existing device - search entry and update XML
                    XmlNodeList xmlNodeList = xmlDoc.GetElementsByTagName("Device");
                    foreach (XmlNode node in xmlNodeList)
                    {
                        if (node.ChildNodes(0).InnerText == hidOrigDeviceLabel.Value)
                            SetAllChild(ref node, false);
                    }
                }
            }

            hidEditPushNotifications.Value = xmlDoc.OuterXml;

            BindXMLtoGridView(xmlDoc);
        }
        else
            popPushNotification.Show();
    }
    private void BindXMLtoGridView(XmlDocument xmlDoc)
    {
        // Bind the XML data to the push notifications gridview
        DataSet ds = new DataSet();
        ds.ReadXml(new XmlNodeReader(xmlDoc));
        if (ds.Tables.Count > 0)
            gvwPushNoticationsList.DataSource = ds.Tables(0);
        else
            gvwPushNoticationsList.DataSource = null;
        gvwPushNoticationsList.DataBind();
    }
    private void SetAllChild(ref XmlElement DeviceNode, bool blnNewNode)
    {
        {
            var withBlock = DeviceNode;
            if (blnNewNode)
            {
                // Sub-element: Name
                XmlElement DeviceName = withBlock.OwnerDocument.CreateElement("Name");
                DeviceName.InnerText = txtDeviceLabel.Text;
                withBlock.AppendChild(DeviceName);

                // Sub-element: Platform
                XmlElement Platform = withBlock.OwnerDocument.CreateElement("Platform");
                Platform.InnerText = "devid";
                withBlock.AppendChild(Platform);

                // Sub-element: URI (CDATA)
                XmlElement URI = withBlock.OwnerDocument.CreateElement("URI");
                XmlNode cdata = withBlock.OwnerDocument.CreateCDataSection(txtDeviceURI.Text);
                URI.AppendChild(cdata);
                withBlock.AppendChild(URI);

                // Sub-element: Live ?
                XmlElement Live = withBlock.OwnerDocument.CreateElement("Live");
                Live.InnerText = chkDeviceLive.Checked;
                withBlock.AppendChild(Live);
            }
            else
            {
                withBlock.ChildNodes(0).InnerText = txtDeviceLabel.Text;
                withBlock.ChildNodes(1).InnerText = "devid";
                withBlock.ChildNodes(2).InnerText = txtDeviceURI.Text;
                withBlock.ChildNodes(3).InnerText = chkDeviceLive.Checked;
            }
        }
    }
}
