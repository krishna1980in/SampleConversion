using System;
using System.Linq;
using System.Text;
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
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

internal partial class Compare : PageBaseClass
{

    private DataTable c_tblProductsToCompare = new DataTable();

    public Compare()
    {
        this.Load += Page_Load;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (KartSettingsManager.GetKartConfig("frontend.products.comparison.enabled") == "y")
        {
            Page.Title = GetGlobalResourceObject("Products", "PageTitle_ProductComparision") + " | " + Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));
            DoCompare();
        }
        else
        {
            mvwMain.SetActiveView(viwNotExist);
        }
    }

    public void DoCompare()
    {
        // collect ID of item to process, ensure no non-int values for security
        int intID = 0;
        try
        {
            intID = Request.QueryString("id");
        }
        catch (Exception ex)
        {
            // 
        }

        // ' Reading the QueryString Key "action" to delete/add a product from/to the comparison.
        // ' Session("ProductsToCompare") --> holds the IDs of the products (separated by commas) that need to be appeared in comparison,
        // '   well, the productID must be rounded by "(" & ")" ... 
        switch (Request.QueryString("action"))
        {
            case "clear":
                {
                    // ' To clear all comparison items
                    Session("ProductsToCompare") = "";
                    break;
                }
            case "del":
                {
                    // ' To delete the product from the comparison list, we need to replace "(ProdID)" by ""
                    Session("ProductsToCompare") = Session("ProductsToCompare").Replace("(" + intID.ToString() + ")", "");
                    Session("ProductsToCompare") = Session("ProductsToCompare").Replace(",,", ",");
                    break;
                }
            case "add":
                {
                    // ' To add a product to the comparison list you need to add comma followed by a Parentheses-rounded ProdID
                    Session("ProductsToCompare") += ",(" + intID.ToString() + ")";
                    break;
                }

            default:
                {
                    break;
                }
        }

        // hide clear button if no items to compare
        if (Session("ProductsToCompare") == "")
            btnClearSession.Visible = false;

        // ' If the product list ends with comma, then we need to remove that comma.
        if (Session("ProductsToCompare").EndsWith(","))
        {
            Session("ProductsToCompare") = Session("ProductsToCompare").TrimEnd(',');
        }
        // ' If the product list starts with comma, then we need to remove that comma.
        if (Session("ProductsToCompare").StartsWith(","))
        {
            Session("ProductsToCompare") = Session("ProductsToCompare").Substring(1);
        }


        string strProductsList = Session("ProductsToCompare");

        // ' Getting the product list without parentheses, to send the list to a stored procedure.
        strProductsList = strProductsList.Replace("(", "");
        strProductsList = strProductsList.Replace(")", "");

        // ' If the list is empty, then exit
        if (strProductsList.Length == 0)
            return;

        // ' Eliminates the spacing in the list.
        strProductsList = strProductsList.Replace(" ", "");

        // ' Gets the ProductIDs in an array of strings.
        var arrProducts = Strings.Split(strProductsList, ",");

        short numCGroupID = 0;
        if (HttpContext.Current.User.Identity.IsAuthenticated)
        {
            numCGroupID = ((PageBaseClass)Page).CurrentLoggedUser.CustomerGroupID;
        }

        // ' Gets the attributes(if any) for comparison from the db.
        c_tblProductsToCompare = KartrisDBBLL.GetProductsAttributesToCompare(strProductsList, Session("LANG"), numCGroupID);

        var strBldrNotIncludedAttributeList = new StringBuilder("");

        // ' Getting the attributes that must be shown for if each product has this attribute.
        DataRow[] drEach;
        drEach = c_tblProductsToCompare.Select("ATTRIB_Compare='e'");
        for (int iEach = 0, loopTo = drEach.Length - 1; iEach <= loopTo; iEach++)
        {
            foreach (var row in c_tblProductsToCompare.Rows)
            {
                DataRow[] dr;
                dr = c_tblProductsToCompare.Select(Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("P_ID=", row("P_ID")), "AND ATTRIB_ID=")) + drEach[iEach]("ATTRIB_ID"));
                if (dr.Length == 0)
                {
                    strBldrNotIncludedAttributeList.Append(drEach[iEach]("ATTRIB_ID"));
                    strBldrNotIncludedAttributeList.Append(",");
                    break;
                }
            }
        }

        // ' Getting the attributes that must be shown for if at least one product has this attribute.
        bool blnAttributeExist = false;
        DataRow[] drOnlyOne;
        drOnlyOne = c_tblProductsToCompare.Select("ATTRIB_Compare='o'");
        for (int iOnlyOne = 0, loopTo1 = drOnlyOne.Length - 1; iOnlyOne <= loopTo1; iOnlyOne++)
        {
            if (!object.ReferenceEquals(drOnlyOne[iOnlyOne]("ATTRIBV_Value"), DBNull.Value))
            {
                blnAttributeExist = true;
                break;
            }
        }
        if (blnAttributeExist == false && drOnlyOne.Length > 0)
        {
            strBldrNotIncludedAttributeList.Append(drOnlyOne[0]("ATTRIB_ID"));
            strBldrNotIncludedAttributeList.Append(",");
        }

        // ' Getting the list of products that should be excluded from the comparison,
        // '   to be sent to ProductCompare.ascx
        var arExcludedAttributeList = new string[] { "" };
        if (strBldrNotIncludedAttributeList.ToString().Contains(","))
        {
            strBldrNotIncludedAttributeList.Remove(strBldrNotIncludedAttributeList.ToString().LastIndexOf(","), 1);
        }
        arExcludedAttributeList = strBldrNotIncludedAttributeList.ToString().Split(",");

        // ' Loads the UC ProductCompare.ascx to show the comparison's attributes.
        UC_ProductComparison.LoadProductComparison(c_tblProductsToCompare, arExcludedAttributeList);

    }
    protected void btnClearSession_Click(object sender, EventArgs e)
    {
        Response.Redirect(CkartrisBLL.WebShopURL + "Compare.aspx?action=clear");
    }
}