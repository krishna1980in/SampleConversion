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

partial class UserControls_Back_PopupMessage : System.Web.UI.UserControl
{

    /// <summary>
    ///     ''' raised when the user click "yes" to confirm the operation
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public event ConfirmedEventHandler Confirmed;

    public delegate void ConfirmedEventHandler();

    /// <summary>
    ///     ''' raised when the user click "no" to decline the operation
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public event CancelledEventHandler Cancelled;

    public delegate void CancelledEventHandler();

    protected void Page_Load(object sender, System.EventArgs e)
    {
        // ' The following line is important for the confirmation msg box
        // '  => it will allow the event of the server side button to be fired
        System.Web.UI.Control.Page.ClientScript.GetPostBackEventReference(this, string.Empty);
    }

    /// <summary>
    ///     ''' indicates if the confirmation box will have an image or not
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <remarks></remarks>
    public bool ShowImage
    {
        set
        {
            phdImage.Visible = value;
        }
    }

    /// <summary>
    ///     ''' calls the popup control to show in the screen
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public void ShowConfirmation(MESSAGE_TYPE enumType, string strMessage)
    {
        switch (enumType)
        {
            case object _ when MESSAGE_TYPE.Confirmation:
                {
                    litTitle.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_Confirmation");
                    lnkYes.Visible = true;
                    lnkNo.Visible = true;
                    break;
                }

            case object _ when MESSAGE_TYPE.ErrorMessage:
                {
                    litTitle.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_Error");
                    lnkYes.Visible = false;
                    lnkNo.Visible = false;
                    break;
                }

            case object _ when MESSAGE_TYPE.Information:
                {
                    litTitle.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("_Kartris", "ContentText_Information");
                    lnkYes.Visible = false;
                    lnkNo.Visible = false;
                    break;
                }
        }

        litMessage.Text = strMessage;
        popExtender.Show();
    }

    protected void lnkYes_Click(object sender, System.EventArgs e)
    {
        Confirmed?.Invoke();
    }

    protected void lnkNo_Click(object sender, System.EventArgs e)
    {
        Cancelled?.Invoke();
    }
}
