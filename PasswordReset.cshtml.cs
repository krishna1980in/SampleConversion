using System;
using System.Linq;
using CkartrisBLL;
// ========================================================================
// Kartris - www.kartris.com
// Copyright 2018 CACTUSOFT

// GNU GENERAL PUBLIC LICENSE v2
// This program is free software distributed under the GPL without any
// warranty.
// www.gnu.org/licenses/gpl-2.0.html

// KARTRIS COMMERCIAL LICENSE
// If a valid license.config issued by Cactusoft is present, the KCL
// overrides the GPL v2.
// www.kartris.com/t-Kartris-Commercial-License.aspx
// ========================================================================
using CkartrisDataManipulation;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

internal partial class PasswordReset : PageBaseClass
{

    private void Page_Load(object sender, EventArgs e)
    {


        if (!string.IsNullOrEmpty(Request.QueryString("ref")))
        {
            lblCurrentPassword.Text = GetGlobalResourceObject("Kartris", "FormLabel_EmailAddress");
            txtCurrentPassword.TextMode = TextBoxMode.SingleLine;
            string strRef = Request.QueryString("ref");
        }

        else
        {
            Response.Redirect(WebShopURL() + "CustomerAccount.aspx");
        }

    }



    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        var objUsersBLL = new UsersBLL();
        if (string.IsNullOrEmpty(Request.QueryString("ref")))
        {
            string strUserPassword = txtCurrentPassword.Text;
            string strNewPassword = txtNewPassword.Text;

            // Only update if validators ok
            if (this.IsValid)
            {
                if (Membership.ValidateUser(CurrentLoggedUser.Email, strUserPassword))
                {
                    if (objUsersBLL.ChangePassword(CurrentLoggedUser.ID, strUserPassword, strNewPassword) > 0)
                        UC_Updated.ShowAnimatedText();
                }
                else
                {
                    string strErrorMessage = Strings.Replace(GetGlobalResourceObject("Kartris", "ContentText_CustomerCodeIncorrect"), "[label]", GetLocalResourceObject("FormLabel_ExistingCustomerCode"));
                    litWrongPassword.Text = "<span class=\"error\">" + strErrorMessage + "</span>";
                }
            }
        }

        else
        {
            string strRef = Request.QueryString("ref");
            string strEmailAddress = txtCurrentPassword.Text;

            DataTable dtbUserDetails = objUsersBLL.GetDetails(strEmailAddress);
            if (dtbUserDetails.Rows.Count > 0)
            {
                int intUserID = dtbUserDetails[0]("U_ID");
                string strTempPassword = FixNullFromDB(dtbUserDetails[0]("U_TempPassword"));
                DateTime datExpiry = Conversions.ToDate(Interaction.IIf(Information.IsDate(FixNullFromDB(dtbUserDetails[0]("U_TempPasswordExpiry"))), dtbUserDetails[0]("U_TempPasswordExpiry"), CkartrisDisplayFunctions.NowOffset.AddMinutes(-1)));
                if (string.IsNullOrEmpty(strTempPassword))
                    datExpiry = CkartrisDisplayFunctions.NowOffset.AddMinutes(-1);

                if (datExpiry > CkartrisDisplayFunctions.NowOffset)
                {
                    if (objUsersBLL.EncryptSHA256Managed(strTempPassword, objUsersBLL.GetSaltByEmail(strEmailAddress)) == strRef)
                    {
                        int intSuccess = objUsersBLL.ChangePasswordViaRecovery(intUserID, txtConfirmPassword.Text);
                        if (intSuccess == intUserID)
                        {
                            UC_Updated.ShowAnimatedText();
                            Response.Redirect(WebShopURL() + "CustomerAccount.aspx?m=u");
                        }
                        else
                        {
                            litWrongPassword.Text = "<span class=\"error\">" + GetGlobalResourceObject("Kartris", "ContentText_ErrorText") + "</span>";
                        }
                    }
                    else
                    {
                        litWrongPassword.Text = "<span class=\"error\">" + GetGlobalResourceObject("Kartris", "ContentText_LinkExpiredOrIncorrect") + "</span>";
                    }
                }

                else
                {
                    litWrongPassword.Text = "<span class=\"error\">" + GetGlobalResourceObject("Kartris", "ContentText_LinkExpiredOrIncorrect") + "</span>";
                }
            }

            else
            {
                litWrongPassword.Text = "<span class=\"error\">" + GetGlobalResourceObject("Kartris", "ContentText_NotFoundInDB") + "</span>";
            }
        }

    }

}