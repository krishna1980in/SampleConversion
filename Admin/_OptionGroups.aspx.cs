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

partial class Admin_OptionGroups : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Options", "PageTitle_OptionGroups") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        if (!Page.IsPostBack)
            _UC_OptionGroup.ShowOptionGroups();
    }

    protected void ShowMasterUpdateMessage()
    {
        (Skins_Admin_Template)this.Master.DataUpdated();
    }
}
