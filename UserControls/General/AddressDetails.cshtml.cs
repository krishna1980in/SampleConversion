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
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Payment;
using KartrisClasses;

partial class UserControls_General_AddressDetails : System.Web.UI.UserControl
{
    private int _AddressID;
    private Address m_address = null/* TODO Change to default(_) if this is not a reference type */;

    public int AddressID
    {
        get
        {
            return _AddressID;
        }
    }

    public bool ShowButtons
    {
        set
        {
            pnlButtons.Visible = value;
        }
    }

    public bool ShowLabel
    {
        set
        {
            litAddressLabel.Visible = value;
        }
    }

    public Address Address
    {
        get
        {
            return m_address;
        }
        set
        {
            _AddressID = value.ID;
            m_address = value;
            try
            {
                litAddressLabel.Text = UserControl.Server.HtmlEncode(value.Label);
                litName.Text = UserControl.Server.HtmlEncode(value.FullName);
                litCompany.Text = UserControl.Server.HtmlEncode(IIf(string.IsNullOrEmpty(value.Company), string.Empty, value.Company));
                litAddress.Text = UserControl.Server.HtmlEncode(value.StreetAddress);
                litTownCity.Text = UserControl.Server.HtmlEncode(value.TownCity);
                litCounty.Text = UserControl.Server.HtmlEncode(value.County);
                litCountry.Text = UserControl.Server.HtmlEncode(Country.Get(value.CountryID).Name);
                litPostcode.Text = UserControl.Server.HtmlEncode(value.Postcode);
                litPhone.Text = UserControl.Server.HtmlEncode(value.Phone);
            }
            catch
            {
            }
        }
    }

    public event btnEditClickedEventHandler btnEditClicked;

    public delegate void btnEditClickedEventHandler(object sender, System.EventArgs e);

    public event btnDeleteClickedEventHandler btnDeleteClicked;

    public delegate void btnDeleteClickedEventHandler(object sender, System.EventArgs e);

    protected void btnEdit_Click(object sender, System.EventArgs e)
    {
        btnEditClicked?.Invoke(sender, e);
    }

    protected void btnDelete_Click(object sender, System.EventArgs e)
    {
        btnDeleteClicked?.Invoke(sender, e);
    }
}
