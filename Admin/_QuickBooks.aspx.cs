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

partial class Admin_QuickBooks : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_QuickBooks", "PageTitle_QBIntegration") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        // Reset page elements visibility
        phdMainContent.Visible = true;
        litMessage.Visible = false;

        // Run normal functionality
        if (!IsPostBack)
        {
            txtQBWCPassword.Text = KartSettingsManager.GetKartConfig("general.quickbooks.pass");
            txtQBWCPassword.Attributes("value") = txtQBWCPassword.Text;

            lblFormLabelPassword.Text = "QBWC " + GetGlobalResourceObject("_Kartris", "FormLabel_Password");
        }
    }

    protected void btnUpdate_Click(object sender, System.EventArgs e)
    {
        string strPass = txtQBWCPassword.Text;
        if (!string.IsNullOrEmpty(strPass))
        {
            if (strPass != KartSettingsManager.GetKartConfig("general.quickbooks.pass"))
            {
                txtQBWCPassword.Attributes("value") = strPass;
                UsersBLL objUsersBLL = new UsersBLL();
                KartSettingsManager.SetKartConfig("general.quickbooks.pass", objUsersBLL.EncryptSHA256Managed(txtQBWCPassword.Text, "quickbooks", true));
                (Skins_Admin_Template)this.Master.DataUpdated();
            }
        }
    }

    protected void btnGenerate_Click(object sender, System.EventArgs e)
    {
        if (string.IsNullOrEmpty(txtQBWCPassword.Text))
            txtQBWCPassword.Text = KartSettingsManager.GetKartConfig("general.quickbooks.pass");
        if (string.IsNullOrEmpty(KartSettingsManager.GetKartConfig("general.quickbooks.pass")))
            txtQBWCPassword.Text = "";
        Page.Validate();
        if (IsValid)
        {
            XmlDocument _xmlDoc = new XmlDocument();
            _xmlDoc.Load(Path.Combine(Request.PhysicalApplicationPath, @"Uploads\resources\KartrisQuickBooks.qwc"));

            XmlNode _xmlNode = _xmlDoc.DocumentElement;
            XmlNodeList _xmlNodeList = _xmlNode.SelectNodes("/QBWCXML");

            for (int x = 0; x <= _xmlNodeList.Count - 1; x++)
            {
                foreach (XmlNode Node in _xmlNodeList.Item(x).ChildNodes)
                {
                    // ' check if the current node is the specified parent element
                    if (Node.Name == "AppURL")
                        Node.InnerText = CkartrisBLL.WebShopURL + "KartrisQBService.asmx";
                    else if (Node.Name == "Scheduler")
                    {
                        foreach (XmlNode innerNode in Node.ChildNodes)
                        {
                            // ' check if the current inner node is the element we want
                            if (innerNode.Name == "RunEveryNMinutes")
                                innerNode.InnerText = txtInterval.Text;
                        }
                    }
                    else if (Node.Name == "FileID")
                        Node.InnerText = "{" + System.Guid.NewGuid().ToString() + "}";
                }
            }

            try
            {
                Response.ContentType = "text/plain";
                Response.AppendHeader("Content-Disposition", "attachment; filename=KartrisQuickBooks.qwc");
                _xmlDoc.Save(Response.OutputStream);
                Response.End();
            }
            catch (Exception eEx)
            {
            }
        }
    }
}
