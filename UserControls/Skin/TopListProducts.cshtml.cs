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

partial class UserControls_Skin_TopListProducts : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!System.Web.UI.Control.Page.IsPostBack)
            LoadTopListProducts();
    }

    public void LoadTopListProducts()
    {
        DataTable tblTopListProducts = new DataTable();
        tblTopListProducts.Columns.Add(new DataColumn("P_ID", Type.GetType("System.Int32")));
        tblTopListProducts.Columns.Add(new DataColumn("P_Name", Type.GetType("System.String")));

        int intTruncateLength = System.Convert.ToInt32(KartSettingsManager.GetKartConfig("frontend.display.topsellers.truncate"));
        DataRow[] drwTopSeller = KartSettingsManager.GetTopListProductsFromCache.Select("LANG_ID=" + System.Web.UI.UserControl.Session["LANG"]);
        int numItemCount = KartSettingsManager.GetKartConfig("frontend.display.topsellers.quantity");

        if (numItemCount > drwTopSeller.Length)
            numItemCount = drwTopSeller.Length;

        for (int i = 0; i <= numItemCount - 1; i++)
            tblTopListProducts.Rows.Add(drwTopSeller[i]("P_ID"), IIf(intTruncateLength > 0, CkartrisDisplayFunctions.TruncateDescription(drwTopSeller[i]("P_Name"), intTruncateLength)
                    , drwTopSeller[i]("P_Name")));

        rptTopListProducts.DataSource = tblTopListProducts;
        rptTopListProducts.DataBind();

        // Turn off if no products
        if (numItemCount == 0)
            this.Visible = false;
    }
}
