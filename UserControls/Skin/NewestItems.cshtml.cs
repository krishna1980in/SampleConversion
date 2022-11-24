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

partial class UserControls_Skin_NewestItems : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!System.Web.UI.Control.Page.IsPostBack)
            LoadNewestProducts();
    }

    public void LoadNewestProducts()
    {
        DataTable tblNewestProducts = new DataTable();
        tblNewestProducts.Columns.Add(new DataColumn("P_ID", Type.GetType("System.Int32")));
        tblNewestProducts.Columns.Add(new DataColumn("P_Name", Type.GetType("System.String")));
        tblNewestProducts.Columns.Add(new DataColumn("MinPrice", Type.GetType("System.Decimal")));

        DataRow[] drwNewProducts = ProductsBLL.GetNewestProducts(System.Web.UI.UserControl.Session["LANG"]);

        int numItemCount = KartSettingsManager.GetKartConfig("frontend.display.newestproducts");

        if (numItemCount > drwNewProducts.Length)
            numItemCount = drwNewProducts.Length;
        for (int i = 0; i <= numItemCount - 1; i++)
            tblNewestProducts.Rows.Add(drwNewProducts[i]("P_ID"), drwNewProducts[i]("P_Name"), drwNewProducts[i]("MinPrice"));

        rptNewestItems.DataSource = tblNewestProducts;
        rptNewestItems.DataBind();

        // Hide whole control if no products to show
        if (numItemCount == 0)
            this.Visible = false;
    }
}
