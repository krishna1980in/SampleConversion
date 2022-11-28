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
using CkartrisImages;
using KartSettingsManager;

partial class UserControls_Back_CategoryFilters : System.Web.UI.UserControl
{
    public event ShowMasterUpdateEventHandler ShowMasterUpdate;

    public delegate void ShowMasterUpdateEventHandler();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!System.Web.UI.Control.Page.IsPostBack)
        {
            PowerpackBLL objPowerpackBLL = new PowerpackBLL();
            txtXML.Text = objPowerpackBLL._GetFilterXMLByCategory(_GetCategoryID());
            if (txtXML.Text == "Not Enabled")
                this.Visible = false;
        }
    }

    protected void lnkBtnSave_Click(object sender, EventArgs e)
    {
        PowerpackBLL objPowerpackBLL = new PowerpackBLL();
        int intObjectConfigID = objPowerpackBLL._GetFilterObjectID();
        string strMessage = string.Empty;
        ObjectConfigBLL objObjectConfigBLL = new ObjectConfigBLL();
        if (!objObjectConfigBLL._SetConfigValue(intObjectConfigID, _GetCategoryID(), txtXML.Text, strMessage))
            _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
        else
            ShowMasterUpdate?.Invoke();
    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        PowerpackBLL objPowerpackBLL = new PowerpackBLL();
        txtXML.Text = objPowerpackBLL._GenerateFilterXML(_GetCategoryID(), System.Web.UI.UserControl.Session["_LANG"]);
    }
}
