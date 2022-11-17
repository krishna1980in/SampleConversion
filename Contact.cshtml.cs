using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CkartrisBLL;
using CkartrisDataManipulation;
using CkartrisDisplayFunctions;
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
using KartSettingsManager;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic

internal partial class contact : PageBaseClass
{

    private static kartris.Basket Basket = new kartris.Basket();
    private static List<Kartris.BasketItem> BasketItems;

    public contact()
    {
        this.Load += Page_Load;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("Kartris", "PageTitle_ContactUs") + " | " + Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));
        if (!Page.IsPostBack)
        {
            if (GetKartConfig("frontend.cataloguemode") == "y")
            {
                chkIncludeItems.Checked = false;
                chkIncludeItems.Visible = false;
            }
        }
    }

    protected void btnSendMessage_Click(object sender, EventArgs e)
    {
        Page.Validate();
        if (Page.IsValid && ajaxNoBotContact.IsValid)
        {

            string strTo = LanguagesBLL.GetEmailToContact(GetLanguageIDfromSession);
            string strBody = CreateMessageBody();
            string strFrom = "";
            if (GetKartConfig("general.email.spoofcontactmail") == "y")
                strFrom = txtEmail.Text;
            else
                strFrom = LanguagesBLL.GetEmailFrom(GetLanguageIDfromSession);
            if (SendEmail(strFrom, strTo, GetGlobalResourceObject("Kartris", "PageTitle_ContactUs") + " - " + txtName.Text, strBody, txtEmail.Text, txtName.Text))
            {
                litResult.Text = GetGlobalResourceObject("Kartris", "ContentText_MailWasSent");
                ClearForm();
            }
            else
            {
                litResult.Text = GetGlobalResourceObject("Kartris", "ContentText_Error");
            }

            ClearForm(); // ' Clear the form for new reviews.
            mvwWriting.SetActiveView(viwWritingResult);   // ' Activates the Result View.

        }
        updMain.Update();
    }

    public string CreateMessageBody()
    {
        var strBldr = new StringBuilder("");
        strBldr.Append(GetGlobalResourceObject("Email", "EmailText_ContactStart") + " " + Current.Request.Url.ToString + Constants.vbCrLf);
        strBldr.Append(GetGlobalResourceObject("Email", "EmailText_OrderEmailBreaker"));
        strBldr.Append(GetGlobalResourceObject("Email", "EmailText_ContactName") + txtName.Text + Constants.vbCrLf);
        strBldr.Append(GetGlobalResourceObject("Email", "EmailText_ContactEmail") + txtEmail.Text + Constants.vbCrLf);
        strBldr.Append(GetGlobalResourceObject("Email", "EmailText_ContactIP") + CkartrisEnvironment.GetClientIPAddress() + Constants.vbCrLf);
        strBldr.Append(GetGlobalResourceObject("Email", "EmailText_ContactDateStamp") + DateTime.Now.ToString() + Constants.vbCrLf);
        strBldr.Append(GetGlobalResourceObject("Email", "EmailText_OrderEmailBreaker"));
        strBldr.Append(txtMessage.Text);
        if (chkIncludeItems.Checked)
            strBldr.Append(GetBasket());
        return strBldr.ToString();
    }

    public string GetBasket()
    {
        BasketItem _Item;
        Basket.LoadBasketItems();
        BasketItems = Basket.BasketItems;
        if (BasketItems.Count == 0)
            return Constants.vbCrLf;
        var strBldrItems = new StringBuilder("");
        strBldrItems.Append(Constants.vbCrLf + GetGlobalResourceObject("Email", "EmailText_OrderEmailBreaker"));
        strBldrItems.Append(GetGlobalResourceObject("Email", "EmailText_ContactBasketContents") + Constants.vbCrLf);
        for (int i = 0, loopTo = BasketItems.Count - 1; i <= loopTo; i++)
        {
            _Item = BasketItems[i];
            strBldrItems.Append(_Item.Quantity + " X " + _Item.ProductName + " - " + _Item.VersionName + " (" + _Item.VersionCode + ")" + Constants.vbCrLf);
        }
        strBldrItems.Append(GetGlobalResourceObject("Email", "EmailText_OrderEmailBreaker"));
        return strBldrItems.ToString();
    }

    public void ClearForm()
    {
        txtName.Text = string.Empty;
        txtEmail.Text = string.Empty;
        txtMessage.Text = string.Empty;
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        mvwWriting.SetActiveView(viwWritingForm);
    }

}