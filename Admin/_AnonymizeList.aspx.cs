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
using KartSettingsManager;

partial class Admin_WholesaleApplicationsList : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = "Anonymization List" + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");
        if (!Page.IsPostBack)
            GetGuestsList();
    }

    protected void btnAnonymize_Click(object sender, System.EventArgs e)
    {
        UsersBLL objUsersBLL = new UsersBLL();
        objUsersBLL._AnonymizeAll();
        GetGuestsList();
    }

    private void GetGuestsList()
    {
        gvwCustomers.DataSource = null;
        gvwCustomers.DataBind();
        // get wholesale - pending customer group list (U_CustomerGroupID = 1)
        UsersBLL objUsersBLL = new UsersBLL();
        gvwCustomers.DataSource = objUsersBLL._GetGuestList();
        gvwCustomers.DataBind();
    }
}
