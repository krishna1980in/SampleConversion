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

using kartrisGDPRData;
using kartrisGDPRDataTableAdapters;
using CkartrisDisplayFunctions;
using CkartrisFormatErrors;
using CkartrisDataManipulation;
using System.Web.HttpContext;


// GDPR BLL
// The GDPR (General Data Protection Regulations)
public class GdprBLL
{

    /// <summary>
    ///     ''' User adapter
    ///     ''' </summary>
    private static UserTblAdptr _Adptr_User = null/* TODO Change to default(_) if this is not a reference type */;
    protected static UserTblAdptr Adptr_User
    {
        get
        {
            _Adptr_User = new UserTblAdptr();
            return _Adptr_User;
        }
    }

    /// <summary>
    ///     ''' Addresses adapter
    ///     ''' </summary>
    private static AddressesTblAdptr _Adptr_Addresses = null/* TODO Change to default(_) if this is not a reference type */;
    protected static AddressesTblAdptr Adptr_Addresses
    {
        get
        {
            _Adptr_Addresses = new AddressesTblAdptr();
            return _Adptr_Addresses;
        }
    }

    /// <summary>
    ///     ''' Orders adapter
    ///     ''' </summary>
    private static OrdersTblAdptr _Adptr_Orders = null/* TODO Change to default(_) if this is not a reference type */;
    protected static OrdersTblAdptr Adptr_Orders
    {
        get
        {
            _Adptr_Orders = new OrdersTblAdptr();
            return _Adptr_Orders;
        }
    }

    /// <summary>
    ///     ''' Invoice rows adapter
    ///     ''' </summary>
    private static InvoiceRowsTblAdptr _Adptr_InvoiceRows = null/* TODO Change to default(_) if this is not a reference type */;
    protected static InvoiceRowsTblAdptr Adptr_InvoiceRows
    {
        get
        {
            _Adptr_InvoiceRows = new InvoiceRowsTblAdptr();
            return _Adptr_InvoiceRows;
        }
    }

    /// <summary>
    ///     ''' Reviews adapter
    ///     ''' </summary>
    private static ReviewsTblAdptr _Adptr_Reviews = null/* TODO Change to default(_) if this is not a reference type */;
    protected static ReviewsTblAdptr Adptr_Reviews
    {
        get
        {
            _Adptr_Reviews = new ReviewsTblAdptr();
            return _Adptr_Reviews;
        }
    }

    /// <summary>
    ///     ''' WishLists adapter
    ///     ''' </summary>
    private static WishListsTblAdptr _Adptr_WishLists = null/* TODO Change to default(_) if this is not a reference type */;
    protected static WishListsTblAdptr Adptr_WishLists
    {
        get
        {
            _Adptr_WishLists = new WishListsTblAdptr();
            return _Adptr_WishLists;
        }
    }

    /// <summary>
    ///     ''' Support tickets adapter
    ///     ''' </summary>
    private static SupportTicketsTblAdptr _Adptr_SupportTickets = null/* TODO Change to default(_) if this is not a reference type */;
    protected static SupportTicketsTblAdptr Adptr_SupportTickets
    {
        get
        {
            _Adptr_SupportTickets = new SupportTicketsTblAdptr();
            return _Adptr_SupportTickets;
        }
    }

    /// <summary>
    ///     ''' Support ticket messages adapter
    ///     ''' </summary>
    private static SupportTicketMessagesTblAdptr _Adptr_SupportTicketMessages = null/* TODO Change to default(_) if this is not a reference type */;
    protected static SupportTicketMessagesTblAdptr Adptr_SupportTicketMessages
    {
        get
        {
            _Adptr_SupportTicketMessages = new SupportTicketMessagesTblAdptr();
            return _Adptr_SupportTicketMessages;
        }
    }

    /// <summary>
    ///     ''' Saved baskets adapter
    ///     ''' </summary>
    private static SavedBasketsTblAdptr _Adptr_SavedBaskets = null/* TODO Change to default(_) if this is not a reference type */;
    protected static SavedBasketsTblAdptr Adptr_SavedBaskets
    {
        get
        {
            _Adptr_SavedBaskets = new SavedBasketsTblAdptr();
            return _Adptr_SavedBaskets;
        }
    }

    /// <summary>
    ///     ''' Saved basket values adapter
    ///     ''' </summary>
    private static SavedBasketValuesTblAdptr _Adptr_SavedBasketValues = null/* TODO Change to default(_) if this is not a reference type */;
    protected static SavedBasketValuesTblAdptr Adptr_SavedBasketValues
    {
        get
        {
            _Adptr_SavedBasketValues = new SavedBasketValuesTblAdptr();
            return _Adptr_SavedBasketValues;
        }
    }

    /// <summary>
    ///     ''' Write to text file
    ///     ''' </summary>
    public static void WriteToTextFile(string strFileName, string strContent)
    {
        StringBuilder strData = new StringBuilder("");
        strData.Append(strContent);

        byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(strData.ToString());

        System.Web.HttpContext.Current.Response.Clear();
        System.Web.HttpContext.Current.Response.AddHeader("Content-Type", "application/Word; charset=utf-8"); // this is to force a file download dialog, text/plain seems to show the output in browser
        System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=" + strFileName.Replace(" ", "_").Replace(".", "-") + ".txt");
        System.Web.HttpContext.Current.Response.AddHeader("Content-Length", data.Length.ToString());
        System.Web.HttpContext.Current.Response.ContentEncoding = Encoding.Unicode;
        System.Web.HttpContext.Current.Response.BinaryWrite(data);
        System.Web.HttpContext.Current.Response.End();
        System.Web.HttpContext.Current.Response.Flush();
    }

    /// <summary>
    ///     ''' Format the text for the GDPR file
    ///     ''' </summary>
    public static string FormatGDPRText(int numUserID)
    {
        StringBuilder strData = new StringBuilder("");

        // First, we put a header with time and date and some
        // details of the data within the file

        // ============== GDPR DATA REQUEST ===============
        strData.AppendLine("============== GDPR DATA REQUEST ===============");
        strData.AppendLine("File created: " + CkartrisDisplayFunctions.FormatDate(DateTime.Now(), "t", 1));
        strData.AppendLine("The data contained within this file is a direct export");
        strData.AppendLine("from the site database at " + CkartrisBLL.WebShopURL.ToLower + ".");
        strData.AppendLine("");
        strData.AppendLine("");



        // ============== CUSTOMER DATA ===============
        strData.AppendLine("");
        strData.AppendLine("== CUSTOMER DATA ==");
        strData.AppendLine("");

        string strEmail = "";
        DataTable dtbUser = Adptr_User.GetData(numUserID);
        foreach (DataRow drwUser in dtbUser.Rows) // loop through users, but should only be one!
        {
            for (var i = 0; i <= dtbUser.Columns.Count - 1; i++) // loop through columns
            {
                string strColumnName = dtbUser.Columns(i).ColumnName;
                string strColumnValue = drwUser(i).ToString;

                // Format dates nicely
                if (strColumnName == "U_TempPasswordExpiry" | strColumnName == "U_ML_SignupDateTime" | strColumnName == "U_ML_ConfirmationDateTime")
                {
                    try
                    {
                        strColumnValue = CkartrisDisplayFunctions.FormatDate(strColumnValue, "t", 1);
                        if (strColumnValue.Contains("1/1/1900"))
                            strColumnValue = "-"; // some default dates are stored as 1/1/1900, but really means no date
                    }
                    catch (Exception ex)
                    {
                        strColumnValue = "-";
                    }// some issue with converting date, return a dash
                }

                if (strColumnName == "U_EmailAddress")
                    strEmail = strColumnValue;

                strData.AppendLine(strColumnName + ": [ " + strColumnValue + " ]");
            }
        }

        // ============== ADDRESSES ===============
        strData.AppendLine("");
        strData.AppendLine("== ADDRESSES ==");
        strData.AppendLine("");

        DataTable dtbAddresses = Adptr_Addresses.GetData(numUserID);
        foreach (DataRow drwAddress in dtbAddresses.Rows)
        {
            for (var i = 0; i <= dtbAddresses.Columns.Count - 1; i++)
                strData.AppendLine(dtbAddresses.Columns(i).ColumnName + ": [ " + drwAddress(i).ToString + " ]");
        }

        // ============== ORDERS ===============
        strData.AppendLine("");
        strData.AppendLine("== ORDERS ==");
        strData.AppendLine("");

        int O_ID = 0; // need this to be found in order to get invoice rows later
        DataTable dtbOrders = Adptr_Orders.GetData(numUserID);
        foreach (DataRow drwOrder in dtbOrders.Rows) // loop through orders, could be multiple
        {
            strData.AppendLine("== Start of Order ==");
            for (var i = 0; i <= dtbOrders.Columns.Count - 1; i++) // loop through columns
            {
                string strColumnName = dtbOrders.Columns(i).ColumnName;
                string strColumnValue = drwOrder(i).ToString;


                if (strColumnName != "O_Details" & strColumnName != "O_Data" & strColumnName != "O_Status")
                {

                    // Get the order ID
                    if (strColumnName == "O_ID")
                        O_ID = strColumnValue;

                    // Format street addresses nicely
                    if (strColumnName == "O_BillingAddress" | strColumnName == "O_ShippingAddress")
                    {
                        strColumnValue = Strings.Replace(strColumnValue, Constants.vbCrLf, ", "); // put address on single line, cleaner for data export
                        strColumnValue = Strings.Replace(strColumnValue, ", , ", ", "); // fix any blank values
                        strColumnValue = Strings.Replace(strColumnValue, ", , ", ", "); // fix any blank values
                    }

                    // Format dates nicely
                    if (strColumnName == "O_Date" | strColumnName == "O_LastModified")
                    {
                        try
                        {
                            strColumnValue = CkartrisDisplayFunctions.FormatDate(strColumnValue, "t", 1);
                            if (strColumnValue.Contains("1/1/1900"))
                                strColumnValue = "-"; // some default dates are stored as 1/1/1900, but really means no date
                        }
                        catch (Exception ex)
                        {
                            strColumnValue = "-";
                        }// some issue with converting date, return a dash
                    }
                    strData.AppendLine(strColumnName + ": [ " + strColumnValue + " ]");
                }
            }

            // -------------- INVOICE ROWS --------------
            DataTable dtbInvoiceRows = Adptr_InvoiceRows.GetData(O_ID);
            foreach (DataRow drwInvoiceRow in dtbInvoiceRows.Rows)  // loop through each invoice row in order
            {
                strData.AppendLine("-------------------------------------");
                for (var i = 0; i <= dtbInvoiceRows.Columns.Count - 1; i++)
                    strData.AppendLine(dtbInvoiceRows.Columns(i).ColumnName + ": [ " + drwInvoiceRow(i).ToString + " ]");
            }
            strData.AppendLine("-------------------------------------");
            strData.AppendLine("== End of Order ==");
            strData.AppendLine("");
        }

        // ============== REVIEWS ===============
        strData.AppendLine("");
        strData.AppendLine("== REVIEWS ==");
        strData.AppendLine("");

        DataTable dtbReviews = Adptr_Reviews.GetData(numUserID, strEmail);
        foreach (DataRow drwReview in dtbReviews.Rows)
        {
            for (var i = 0; i <= dtbReviews.Columns.Count - 1; i++)
            {
                string strColumnName = dtbReviews.Columns(i).ColumnName;
                string strColumnValue = drwReview(i).ToString;

                // Format dates nicely
                if (strColumnName == "REV_DateCreated" | strColumnName == "REV_DateLastUpdated")
                {
                    try
                    {
                        strColumnValue = CkartrisDisplayFunctions.FormatDate(strColumnValue, "t", 1);
                        if (strColumnValue.Contains("1/1/1900"))
                            strColumnValue = "-"; // some default dates are stored as 1/1/1900, but really means no date
                    }
                    catch (Exception ex)
                    {
                        strColumnValue = "-";
                    }// some issue with converting date, return a dash
                }

                strData.AppendLine(strColumnName + ": [ " + strColumnValue + " ]");
            }
        }

        // ============== WISHLISTS ===============
        strData.AppendLine("");
        strData.AppendLine("== WISHLISTS ==");
        strData.AppendLine("");

        DataTable dtbWishLists = Adptr_WishLists.GetData(numUserID);
        foreach (DataRow drwWishList in dtbWishLists.Rows)
        {
            strData.AppendLine("== Start of WishList ==");
            for (var i = 0; i <= dtbWishLists.Columns.Count - 1; i++)
            {
                string strColumnName = dtbWishLists.Columns(i).ColumnName;
                string strColumnValue = drwWishList(i).ToString;

                // Format dates nicely
                if (strColumnName == "WL_DateTimeAdded" | strColumnName == "WL_LastUpdated")
                {
                    try
                    {
                        strColumnValue = CkartrisDisplayFunctions.FormatDate(strColumnValue, "t", 1);
                        if (strColumnValue.Contains("1/1/1900"))
                            strColumnValue = "-"; // some default dates are stored as 1/1/1900, but really means no date
                    }
                    catch (Exception ex)
                    {
                        strColumnValue = "-";
                    }// some issue with converting date, return a dash
                }

                strData.AppendLine(strColumnName + ": [ " + strColumnValue + " ]");
            }
            strData.AppendLine("== End of WishList ==");
            strData.AppendLine("");
        }

        // ============== SUPPORT TICKETS ===============
        strData.AppendLine("");
        strData.AppendLine("== SUPPORT TICKETS ==");
        strData.AppendLine("");

        int TIC_ID = 0; // need this to be found in order to get messages later
        DataTable dtbSupportTickets = Adptr_SupportTickets.GetData(numUserID);
        foreach (DataRow drwSupportTicket in dtbSupportTickets.Rows) // loop through orders, could be multiple
        {
            strData.AppendLine("== Start of Support Ticket ==");
            for (var i = 0; i <= dtbSupportTickets.Columns.Count - 1; i++) // loop through columns
            {
                string strColumnName = dtbSupportTickets.Columns(i).ColumnName;
                string strColumnValue = drwSupportTicket(i).ToString;



                // Get the order ID
                if (strColumnName == "TIC_ID")
                    TIC_ID = strColumnValue;

                // Format dates nicely
                if (strColumnName == "TIC_DateOpened" | strColumnName == "TIC_DateClosed")
                {
                    try
                    {
                        strColumnValue = CkartrisDisplayFunctions.FormatDate(strColumnValue, "t", 1);
                        if (strColumnValue.Contains("1/1/1900"))
                            strColumnValue = "-"; // some default dates are stored as 1/1/1900, but really means no date
                    }
                    catch (Exception ex)
                    {
                        strColumnValue = "-";
                    }// some issue with converting date, return a dash
                }
                strData.AppendLine(strColumnName + ": [ " + strColumnValue + " ]");
            }

            // -------------- SUPPORT TICKET MESSAGES --------------
            DataTable dtbSupportTicketMesssages = Adptr_SupportTicketMessages.GetData(TIC_ID);
            foreach (DataRow drwSupportTicketMesssage in dtbSupportTicketMesssages.Rows)  // loop through each message in order
            {
                strData.AppendLine("-------------------------------------");
                for (var i = 0; i <= dtbSupportTicketMesssages.Columns.Count - 1; i++)
                    strData.AppendLine(dtbSupportTicketMesssages.Columns(i).ColumnName + ": [ " + drwSupportTicketMesssage(i).ToString + " ]");
            }
            strData.AppendLine("-------------------------------------");
            strData.AppendLine("== End of Support Ticket ==");
            strData.AppendLine("");
        }

        // ============== SAVED BASKETS ===============
        strData.AppendLine("");
        strData.AppendLine("== SAVED BASKETS ==");
        strData.AppendLine("");

        int SBSKT_ID = 0; // need this to be found in order to get line items later
        DataTable dbtSavedBaskets = Adptr_SavedBaskets.GetData(numUserID);
        foreach (DataRow drwSavedBasket in dbtSavedBaskets.Rows) // loop through orders, could be multiple
        {
            strData.AppendLine("== Start of Saved Basket ==");
            for (var i = 0; i <= dbtSavedBaskets.Columns.Count - 1; i++) // loop through columns
            {
                string strColumnName = dbtSavedBaskets.Columns(i).ColumnName;
                string strColumnValue = drwSavedBasket(i).ToString;

                // Get the saved basket ID
                if (strColumnName == "SBSKT_ID")
                    SBSKT_ID = strColumnValue;

                // Format dates nicely
                if (strColumnName == "SBSKT_DateTimeAdded" | strColumnName == "SBSKT_LastUpdated")
                {
                    try
                    {
                        strColumnValue = CkartrisDisplayFunctions.FormatDate(strColumnValue, "t", 1);
                        if (strColumnValue.Contains("1/1/1900"))
                            strColumnValue = "-"; // some default dates are stored as 1/1/1900, but really means no date
                    }
                    catch (Exception ex)
                    {
                        strColumnValue = "-";
                    }// some issue with converting date, return a dash
                }
                strData.AppendLine(strColumnName + ": [ " + strColumnValue + " ]");
            }

            // -------------- SAVED BASKET VALUES --------------
            DataTable dtbSavedBasketValues = Adptr_SavedBasketValues.GetData(SBSKT_ID);
            foreach (DataRow drwSavedBasketValue in dtbSavedBasketValues.Rows)  // loop through each item in order
            {
                strData.AppendLine("-------------------------------------");
                for (var i = 0; i <= dtbSavedBasketValues.Columns.Count - 1; i++)
                {
                    string strColumnName = dtbSavedBasketValues.Columns(i).ColumnName;
                    string strColumnValue = drwSavedBasketValue(i).ToString;

                    // Format dates nicely
                    if (strColumnName == "BV_DateTimeAdded")
                    {
                        try
                        {
                            strColumnValue = CkartrisDisplayFunctions.FormatDate(strColumnValue, "t", 1);
                            if (strColumnValue.Contains("1/1/1900"))
                                strColumnValue = "-"; // some default dates are stored as 1/1/1900, but really means no date
                        }
                        catch (Exception ex)
                        {
                            strColumnValue = "-";
                        }// some issue with converting date, return a dash
                    }
                    strData.AppendLine(strColumnName + ": [ " + strColumnValue + " ]");
                }
            }
            strData.AppendLine("-------------------------------------");
            strData.AppendLine("== End of Saved Basket ==");
            strData.AppendLine("");
        }

        // ============== END OF FILE ===============
        strData.AppendLine("============== END OF FILE ===============");

        return strData.ToString();
    }
}
