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

partial class Admin_Home : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        string[] arrAuth = HttpSecureCookie.Decrypt(Session("Back_Auth"));

        // Show stats graphs
        phdOrderStats.Visible = (System.Convert.ToBoolean(arrAuth[3]) | System.Convert.ToBoolean(arrAuth[1])) & KartSettingsManager.GetKartConfig("backend.homepage.graphs") != "OFF";
    }

    protected void btnRestart_Click(object sender, System.EventArgs e)
    {
        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.Confirmation, GetGlobalResourceObject("_Kartris", "ContentText_ConfirmRestartKartris"));
    }

    protected void _UC_PopupMsg_Confirmed()
    {
        if (CkartrisBLL.RestartKartrisApplication())
            (Skins_Admin_Template)this.Master.DataUpdated();
        else
            _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, GetGlobalResourceObject("_Kartris", "ContentText_ErrorCantRestartKartris"));
    }
}
