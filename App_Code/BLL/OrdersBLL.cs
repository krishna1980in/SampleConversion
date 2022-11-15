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

using CkartrisDataManipulation;
using CkartrisFormatErrors;
using System.Data;
using System.Data.SqlClient;
using kartrisOrdersData;
using kartrisOrdersDataTableAdapters;
using System.Web.HttpContext;
using CkartrisEnumerations;

public class OrdersBLL
{
    public enum ORDERS_LIST_CALLMODE
    {
        RECENT,
        UNFINISHED,
        INVOICE,
        DISPATCH,
        COMPLETE,
        PAYMENT,
        GATEWAY,
        AFFILIATE,
        BYDATE,
        BYBATCH,
        CUSTOMER,
        SEARCH,
        CANCELLED
    }

    private static OrderstblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;
    private static InvoiceRowsTblAdptr _InvoiceRowsAdptr = null/* TODO Change to default(_) if this is not a reference type */;
    private static PaymentsTblAdptr _PaymentsAdptr = null/* TODO Change to default(_) if this is not a reference type */;

    protected static InvoiceRowsTblAdptr InvoiceRowsAdptr
    {
        get
        {
            _InvoiceRowsAdptr = new InvoiceRowsTblAdptr();
            return _InvoiceRowsAdptr;
        }
    }

    protected static OrderstblAdptr Adptr
    {
        get
        {
            _Adptr = new OrderstblAdptr();
            return _Adptr;
        }
    }

    protected static PaymentsTblAdptr PaymentsAdptr
    {
        get
        {
            _PaymentsAdptr = new PaymentsTblAdptr();
            return _PaymentsAdptr;
        }
    }

    public static DataTable _GetTileAppData(string OrderSent, string OrderInvoiced, string OrderPaid, string OrderShipped, string OrderCancelled, DateTime DateRangeStart, DateTime DateRangeEnd, int intRangeInMinutes)
    {
        try
        {

            // Perform the update on the DataTable
            return Adptr._GetTileAppData(OrderSent, OrderInvoiced, OrderPaid, OrderShipped, OrderCancelled, DateRangeStart, DateRangeEnd, intRangeInMinutes);
        }
        // If we reach here, no errors, so commit the transaction

        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
            // If we reach here, there was an error, so rollback the transaction
            return null;
        }
    }

    public void _Delete(int O_ID, bool blnReturnStock)
    {
        try
        {

            // Perform the update on the DataTable
            Adptr._Delete(O_ID, blnReturnStock);
        }
        // If we reach here, no errors, so commit the transaction

        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
    }

    public void _PurgeOrders(DateTime O_PurgeDate)
    {
        try
        {

            // Perform the update on the DataTable
            DataTable tblToPurgeOrders = Adptr._ToPurgeOrdersList(O_PurgeDate);
            foreach (DataRow dr in tblToPurgeOrders.Rows)
                Adptr._Delete(dr.Item[0].ToString(), false);
        }
        // If we reach here, no errors, so commit the transaction
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
    }

    /// <summary>
    ///     ''' Clone and cancel an order
    ///     ''' </summary>
    ///     ''' <returns>Returns the newly created order ID</returns>
    public int _CloneAndCancel(int O_ID, string strOrderDetails, KartrisClasses.Address BillingAddress, KartrisClasses.Address ShippingAddress, bool blnSameShippingAsBilling, bool O_Sent, bool O_Invoiced, bool O_Paid, bool O_Shipped, Kartris.Basket BasketObject, List<Kartris.BasketItem> BasketArray, string strShippingMethod, string strNotes, double numGatewayTotalPrice, string O_PurchaseOrderNo, string strPromotionDescription, int intCurrencyID, bool blnOrderEmails)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand();
            cmd.CommandText = "_spKartrisOrders_CloneAndCancel";
            SqlTransaction savePoint = null;
            cmd.CommandType = CommandType.StoredProcedure;
            Log("Before Try");
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;


                int intNewOrderID = 0;
                Kartris.Basket objBasket = BasketObject;
                List<Kartris.BasketItem> arrBasketItems = BasketArray;
                string strBillingAddressText;
                string strShippingAddressText;

                // Build the billing address string to be used in the order record
                {
                    var withBlock = BillingAddress;
                    strBillingAddressText = withBlock.FullName + Constants.vbCrLf + withBlock.Company + Constants.vbCrLf + withBlock.StreetAddress + Constants.vbCrLf + withBlock.TownCity + Constants.vbCrLf + withBlock.County + Constants.vbCrLf + withBlock.Postcode + Constants.vbCrLf + withBlock.Country.Name + Constants.vbCrLf + withBlock.Phone;
                }

                // Build the shipping address string to be used in the order record
                if (blnSameShippingAsBilling)
                    strShippingAddressText = strBillingAddressText;
                else
                {
                    var withBlock = ShippingAddress;
                    strShippingAddressText = withBlock.FullName + Constants.vbCrLf + withBlock.Company + Constants.vbCrLf + withBlock.StreetAddress + Constants.vbCrLf + withBlock.TownCity + Constants.vbCrLf + withBlock.County + Constants.vbCrLf + withBlock.Postcode + Constants.vbCrLf + withBlock.Country.Name + Constants.vbCrLf + withBlock.Phone;
                }

                float numTaxDue;
                if (objBasket.ApplyTax)
                    numTaxDue = 1;
                else
                    numTaxDue = 0;

                double numCurrencyRate = System.Convert.ToDouble(CurrenciesBLL.CurrencyRate(intCurrencyID));

                // Create the actual order record
                {
                    var withBlock = cmd.Parameters;
                    withBlock.AddWithValue("@O_ID", O_ID);
                    withBlock.AddWithValue("@O_Details", strOrderDetails);
                    withBlock.AddWithValue("@O_BillingAddress", strBillingAddressText);
                    withBlock.AddWithValue("@O_ShippingAddress", strShippingAddressText);
                    withBlock.AddWithValue("O_Sent", O_Sent);
                    withBlock.AddWithValue("O_Invoiced", O_Invoiced);
                    withBlock.AddWithValue("O_Paid", O_Paid);
                    withBlock.AddWithValue("O_Shipped", O_Shipped);
                    withBlock.AddWithValue("@O_ShippingPrice", objBasket.ShippingPrice.ExTax);
                    withBlock.AddWithValue("@O_ShippingTax", objBasket.ShippingPrice.TaxAmount);
                    withBlock.AddWithValue("@O_TotalPrice", objBasket.FinalPriceIncTax);
                    withBlock.AddWithValue("@O_LastModified", CkartrisDisplayFunctions.NowOffset);
                    withBlock.AddWithValue("@O_CouponCode", objBasket.CouponCode);
                    withBlock.AddWithValue("@O_CouponDiscountTotal", objBasket.CouponDiscount.IncTax);
                    withBlock.AddWithValue("@O_TaxDue", numTaxDue);
                    withBlock.AddWithValue("@O_TotalPriceGateway", numGatewayTotalPrice);
                    withBlock.AddWithValue("@O_OrderHandlingCharge", objBasket.OrderHandlingPrice.ExTax);
                    withBlock.AddWithValue("@O_OrderHandlingChargeTax", objBasket.OrderHandlingPrice.TaxAmount);
                    withBlock.AddWithValue("@O_ShippingMethod", strShippingMethod);
                    withBlock.AddWithValue("@O_Notes", strNotes);
                    withBlock.AddWithValue("@BackendUserID", HttpContext.Current.Session("_UserID"));
                    withBlock.AddWithValue("@O_PurchaseOrderNo", O_PurchaseOrderNo);
                    withBlock.AddWithValue("@O_PromotionDiscountTotal", objBasket.PromotionDiscount.IncTax);
                    withBlock.AddWithValue("@O_PromotionDescription", strPromotionDescription);
                    withBlock.AddWithValue("@O_AffiliateTotalPrice", objBasket.TotalValueToAffiliate);
                    withBlock.AddWithValue("@O_PricesIncTax", IIf(objBasket.PricesIncTax, 1, 0));
                    withBlock.AddWithValue("@O_SendOrderUpdateEmail", Interaction.IIf(blnOrderEmails, 1, 0));
                    withBlock.AddWithValue("@O_CurrencyRate", numCurrencyRate);
                }


                intNewOrderID = cmd.ExecuteScalar();

                // Cycle through the basket items and add each as order invoice rows
                if (!(arrBasketItems == null))
                {
                    BasketItem BasketItem = new BasketItem();
                    for (int i = 0; i <= arrBasketItems.Count - 1; i++)
                    {
                        BasketItem = arrBasketItems[i];
                        {
                            var withBlock = BasketItem;
                            SqlCommand cmdAddInvoiceRows = new SqlCommand("spKartrisOrders_InvoiceRowsAdd", sqlConn, savePoint);
                            cmdAddInvoiceRows.CommandType = CommandType.StoredProcedure;


                            cmdAddInvoiceRows.Parameters.AddWithValue("@IR_OrderNumberID", intNewOrderID);
                            cmdAddInvoiceRows.Parameters.AddWithValue("@IR_VersionCode", withBlock.VersionCode);
                            cmdAddInvoiceRows.Parameters.AddWithValue("@IR_VersionName", IIf(withBlock.VersionName != withBlock.ProductName | InStr(withBlock.VersionName, withBlock.ProductName) == 0, withBlock.ProductName + " - " + withBlock.VersionName, withBlock.VersionName));
                            cmdAddInvoiceRows.Parameters.AddWithValue("@IR_Quantity", withBlock.Quantity);
                            cmdAddInvoiceRows.Parameters.AddWithValue("@IR_PricePerItem", withBlock.IR_PricePerItem);
                            cmdAddInvoiceRows.Parameters.AddWithValue("@IR_TaxPerItem", withBlock.IR_TaxPerItem);

                            cmdAddInvoiceRows.Parameters.AddWithValue("@IR_ExcludeFromCustomerDiscount", withBlock.ExcludeFromCustomerDiscount);

                            StringBuilder sbdExtraText = new StringBuilder(withBlock.OptionText);
                            if (!string.IsNullOrEmpty(withBlock.CustomText))
                            {
                                if (!string.IsNullOrEmpty(withBlock.OptionText))
                                    sbdExtraText.Append("<br/>");
                                sbdExtraText.Append("[" + withBlock.CustomText + "]");
                            }
                            cmdAddInvoiceRows.Parameters.AddWithValue("@IR_OptionsText", sbdExtraText.ToString());

                            int strOutputValue = cmdAddInvoiceRows.ExecuteScalar();
                            if (strOutputValue == 0)
                                throw new Exception("Failed adding invoice row");
                        }
                    }
                }


                if (objBasket.PromotionDiscount.IncTax < 0)
                {
                    List<Kartris.Promotion> objPromotions = new List<Kartris.Promotion>();
                    ArrayList objPromotionsDiscount = new ArrayList();
                    BasketBLL.CalculatePromotions(objBasket, objPromotions, objPromotionsDiscount, false);
                    foreach (PromotionBasketModifier objPromotion in objPromotionsDiscount)
                    {
                        try
                        {
                            SqlCommand cmdAddPromotionLinks = new SqlCommand("spKartrisOrdersPromotions_Add", sqlConn, savePoint);
                            {
                                var withBlock = cmdAddPromotionLinks;
                                withBlock.CommandType = CommandType.StoredProcedure;
                                withBlock.Parameters.AddWithValue("@OrderID", intNewOrderID);
                                withBlock.Parameters.AddWithValue("@PromotionID", objPromotion.PromotionID);
                                withBlock.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }

                // If we reach here, no errors, so commit the transaction
                savePoint.Commit();
                sqlConn.Close();

                // Try to update customer balance before returning value
                int O_CustomerID = 0;
                UsersBLL objUsersBLL = new UsersBLL();
                OrdersBLL objOrdersBLL = new OrdersBLL();
                try
                {
                    O_CustomerID = objOrdersBLL._GetCustomerIDbyOrderID(O_ID);
                    objUsersBLL.UpdateCustomerBalance(O_CustomerID, System.Convert.ToDecimal(_GetPaymentTotalByCustomerID(O_CustomerID) - _GetOrderTotalByCustomerID(O_CustomerID)));
                }
                catch (Exception ex)
                {
                }

                // Final step
                // We want to update the stock levels
                // First increase stock for first order that was cancelled
                bool blnResult = false;
                blnResult = _AdjustStockLevels(O_ID, "i");

                // Then deplete stock of items in the new order
                blnResult = _AdjustStockLevels(intNewOrderID, "d");

                return intNewOrderID;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
                // Something went wrong, so rollback the order
                if (!savePoint == null)
                    savePoint.Rollback();
                return 0;
                throw;
            }                // Bubble up the exception
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close(); savePoint.Dispose();
                }
            }
        }
        return 0;
    }

    /// <summary>
    ///     ''' Adjust stock levels up or down
    ///     ''' </summary>
    ///     ''' <returns>boolean</returns>
    ///     ''' <remarks>
    ///     ''' Added v3.2002
    ///     ''' This queries by order ID to find invoice rows, and then can increase or 
    ///     ''' decrease the stock levels of the versions in those orders, depending on 
    ///     ''' the strIncreaseDecrease sent in. This is useful to release stock when
    ///     ''' an order is cancelled and replaced, or when creating a new order in the
    ///     ''' back end.
    ///     ''' </remarks>
    public bool _AdjustStockLevels(int O_ID, string strIncreaseDecrease)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand();
            cmd.CommandText = "_spKartrisOrders_AdjustStockLevels";
            SqlTransaction savePoint = null;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;
                // Perform the update on the DataTable
                {
                    var withBlock = cmd.Parameters;
                    withBlock.AddWithValue("@O_ID", O_ID);
                    withBlock.AddWithValue("@strIncreaseDecrease", strIncreaseDecrease);
                }

                cmd.ExecuteNonQuery();

                savePoint.Commit();
                sqlConn.Close();
                return true;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
                // If we reach here, there was an error, so rollback the transaction
                savePoint.Rollback();
                return false;
            }
        }
    }

    /// <summary>
    ///     ''' Update details holding mail text
    ///     ''' </summary>
    public int _DetailsUpdate(long O_ID, string O_Details)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand();
            cmd.CommandText = "_spKartrisOrders_DetailsUpdate";
            SqlTransaction savePoint = null;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;
                // Perform the update on the DataTable
                cmd.Parameters.AddWithValue("@O_ID", O_ID);
                cmd.Parameters.AddWithValue("@O_Details", O_Details);

                int returnValue = cmd.ExecuteScalar();
                if (returnValue != O_ID)
                    throw new Exception("ID is 0? Something's not right");

                // If we reach here, no errors, so commit the transaction
                savePoint.Commit();
                sqlConn.Close();

                // Try to update customer balance before returning value
                int O_CustomerID = 0;
                UsersBLL objUsersBLL = new UsersBLL();
                OrdersBLL objOrdersBLL = new OrdersBLL();
                try
                {
                    O_CustomerID = objOrdersBLL._GetCustomerIDbyOrderID(O_ID);
                    objUsersBLL.UpdateCustomerBalance(O_CustomerID, System.Convert.ToDecimal(_GetPaymentTotalByCustomerID(O_CustomerID) - _GetOrderTotalByCustomerID(O_CustomerID)));
                }
                catch (Exception ex)
                {
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
                // If we reach here, there was an error, so rollback the transaction
                savePoint.Rollback();
                return 0;
            }
        }
    }

    public DataTable GetOrderByID(long O_ID)
    {
        return Adptr.GetData(O_ID);
    }

    public long _GetParentOrderID(long O_ID)
    {
        return Adptr._GetParentOrderID(O_ID);
    }
    public long _GetChildOrderID(long O_ID)
    {
        return Adptr._GetChildOrderID(O_ID);
    }
    public DataTable _GetByStatus(ORDERS_LIST_CALLMODE CallMode, int intPageNo, int AffiliateID = 0, DateTime O_DateRangeStart = default(DateTime), DateTime O_DateRangeEnd = default(DateTime), string strGateway = "", string strGatewayID = "", int Limit = 50)
    {
        // If date range start parameter is empty then use a valid date
        if (O_DateRangeStart == DateTime.MinValue)
            O_DateRangeStart = CkartrisDisplayFunctions.NowOffset.Date;
        else
            // Make sure that the date is passed with time so we're adding 1 second to the range start value
            O_DateRangeStart = O_DateRangeStart.Date.AddMilliseconds(-1);

        // Pass 12:59:59.59 PM of range end value
        if (O_DateRangeEnd == DateTime.MinValue)
            O_DateRangeEnd = O_DateRangeStart.AddDays(1).AddMinutes(-1);
        else
            O_DateRangeEnd = O_DateRangeEnd.Date.AddDays(1).AddMinutes(-1);
        if (CallMode == ORDERS_LIST_CALLMODE.CUSTOMER)
        {
            try
            {
                strGatewayID = System.Convert.ToString(strGatewayID);
            }
            catch (Exception ex)
            {
                strGatewayID = 0;
            }
        }

        return Adptr._GetDataByStatus(System.Enum.GetName(typeof(ORDERS_LIST_CALLMODE), CallMode), AffiliateID, O_DateRangeStart, O_DateRangeEnd, strGateway, strGatewayID, intPageNo, Limit);
    }

    public int _GetByStatusCount(ORDERS_LIST_CALLMODE CallMode, int AffiliateID = 0, DateTime O_DateRangeStart = default(DateTime), DateTime O_DateRangeEnd = default(DateTime), string strGateway = "", string strGatewayID = "")
    {
        // If date range start parameter is empty then use a valid date
        if (O_DateRangeStart == DateTime.MinValue)
            O_DateRangeStart = CkartrisDisplayFunctions.NowOffset.Date;
        else
            // Make sure that the date is passed with time so we're adding 1 second to the range start value
            O_DateRangeStart = O_DateRangeStart.Date.AddMilliseconds(-1);

        // Pass 12:59:59.59 PM of range end value
        if (O_DateRangeEnd == DateTime.MinValue)
            O_DateRangeEnd = O_DateRangeStart.AddDays(1).AddMinutes(-1);
        else
            O_DateRangeEnd = O_DateRangeEnd.Date.AddDays(1).AddMinutes(-1);
        if (CallMode == ORDERS_LIST_CALLMODE.CUSTOMER)
        {
            try
            {
                strGatewayID = System.Convert.ToString(strGatewayID);
            }
            catch (Exception ex)
            {
                strGatewayID = 0;
            }
        }

        return Adptr._GetByStatusCount(System.Enum.GetName(typeof(ORDERS_LIST_CALLMODE), CallMode), AffiliateID, O_DateRangeStart, O_DateRangeEnd, strGateway, strGatewayID);
    }

    public int _UpdateStatus(int O_ID, bool O_Sent, bool O_Paid, bool O_Shipped, bool O_Invoiced, string O_Status, string O_Notes, bool O_Cancelled)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand();
            cmd.CommandText = "_spKartrisOrders_UpdateStatus";
            SqlTransaction savePoint = null;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;
                // Perform the update on the DataTable
                {
                    var withBlock = cmd.Parameters;
                    withBlock.AddWithValue("@O_ID", O_ID);
                    withBlock.AddWithValue("@O_LastModified", CkartrisDisplayFunctions.NowOffset);
                    withBlock.AddWithValue("@O_Sent", Interaction.IIf(O_Sent, 1, 0));
                    withBlock.AddWithValue("@O_Invoiced", Interaction.IIf(O_Invoiced, 1, 0));
                    withBlock.AddWithValue("@O_Shipped", Interaction.IIf(O_Shipped, 1, 0));
                    withBlock.AddWithValue("@O_Paid", Interaction.IIf(O_Paid, 1, 0));
                    withBlock.AddWithValue("@O_Cancelled", Interaction.IIf(O_Cancelled, 1, 0));
                    withBlock.AddWithValue("@O_Status", O_Status);
                    withBlock.AddWithValue("@O_Notes", O_Notes);
                }

                int returnValue = cmd.ExecuteScalar();
                if (returnValue != O_ID)
                    throw new Exception("ID is 0? Something's not right");

                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Orders, System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully"), CreateQuery(cmd), O_ID, sqlConn, savePoint);

                // If we reach here, no errors, so commit the transaction
                savePoint.Commit();
                sqlConn.Close();

                // Try to update customer balance before returning value
                int O_CustomerID = 0;
                UsersBLL objUsersBLL = new UsersBLL();
                OrdersBLL objOrdersBLL = new OrdersBLL();
                try
                {
                    O_CustomerID = objOrdersBLL._GetCustomerIDbyOrderID(O_ID);
                    objUsersBLL.UpdateCustomerBalance(O_CustomerID, System.Convert.ToDecimal(_GetPaymentTotalByCustomerID(O_CustomerID) - _GetOrderTotalByCustomerID(O_CustomerID)));
                }
                catch (Exception ex)
                {
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
                // If we reach here, there was an error, so rollback the transaction
                savePoint.Rollback();
                return 0;
            }
        }
    }

    public static DataTable _GetInvoiceRows(int O_ID)
    {
        return InvoiceRowsAdptr.GetInvoiceRows(O_ID);
    }
    public double _GetOrderTotalByCustomerID(int CustomerID)
    {
        return Adptr._GetCustomerTotal(CustomerID);
    }
    public int _AddNewPayment(DateTime Payment_Date, int Payment_CustomerID, double Payment_Amount, int Payment_CurrencyID, string Payment_Gateway, string Payment_ReferenceCode, double Payment_ExchangeRate, ListItemCollection lcLinkedOrders)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            int intNewPaymentID;
            SqlCommand cmd = sqlConn.CreateCommand();
            cmd.CommandText = "_spKartrisPayments_Add";
            SqlTransaction savePoint = null;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                {
                    var withBlock = cmd;
                    withBlock.Transaction = savePoint;
                    withBlock.Parameters.AddWithValue("@Payment_CustomerID", Payment_CustomerID);
                    withBlock.Parameters.AddWithValue("@Payment_Date", Payment_Date);
                    withBlock.Parameters.AddWithValue("@Payment_Amount", Payment_Amount);
                    withBlock.Parameters.AddWithValue("@Payment_CurrencyID", Payment_CurrencyID);
                    withBlock.Parameters.AddWithValue("@Payment_ReferenceNo", Payment_ReferenceCode);
                    withBlock.Parameters.AddWithValue("@Payment_Gateway", Payment_Gateway);
                    withBlock.Parameters.AddWithValue("@Payment_CurrencyRate", Payment_ExchangeRate);
                    intNewPaymentID = withBlock.ExecuteScalar();
                }


                if (lcLinkedOrders.Count > 0)
                {
                    foreach (ListItem item in lcLinkedOrders)
                    {
                        int intOrderID = System.Convert.ToInt32(item.Value);
                        SqlCommand cmdLinkedOrders = new SqlCommand("_spKartrisPayments_AddLinkedOrder", sqlConn, savePoint);
                        {
                            var withBlock = cmdLinkedOrders;
                            withBlock.CommandType = CommandType.StoredProcedure;
                            withBlock.Transaction = savePoint;
                            withBlock.Parameters.AddWithValue("@OP_PaymentID", intNewPaymentID);
                            withBlock.Parameters.AddWithValue("@OP_OrderID", intOrderID);
                            withBlock.Parameters.AddWithValue("@OP_OrderCanceled", 0);
                            withBlock.ExecuteNonQuery();
                        }
                    }
                }

                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Orders, System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully"), CreateQuery(cmd), intNewPaymentID, sqlConn, savePoint);

                // If we reach here, no errors, so commit the transaction
                savePoint.Commit();
                sqlConn.Close();

                // Try to update customer balance before returning value
                UsersBLL objUsersBLL = new UsersBLL();
                try
                {
                    objUsersBLL.UpdateCustomerBalance(Payment_CustomerID, System.Convert.ToDecimal(_GetPaymentTotalByCustomerID(Payment_CustomerID) - _GetOrderTotalByCustomerID(Payment_CustomerID)));
                }
                catch (Exception ex)
                {
                }

                return intNewPaymentID;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
                // Something went wrong, so rollback the order
                if (!savePoint == null)
                    savePoint.Rollback();
                return 0;
                throw;
            }                // Bubble up the exception
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close(); savePoint.Dispose();
                }
            }
        }
    }
    public int _UpdatePayment(int Payment_ID, DateTime Payment_Date, int Payment_CustomerID, double Payment_Amount, int Payment_CurrencyID, string Payment_Gateway, string Payment_ReferenceCode, double Payment_ExchangeRate, ListItemCollection lcLinkedOrders)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand();
            cmd.CommandText = "_spKartrisPayments_Update";
            SqlTransaction savePoint = null;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                {
                    var withBlock = cmd;
                    withBlock.Transaction = savePoint;
                    withBlock.Parameters.AddWithValue("@Payment_ID", Payment_ID);
                    withBlock.Parameters.AddWithValue("@Payment_CustomerID", Payment_CustomerID);
                    withBlock.Parameters.AddWithValue("@Payment_Date", Payment_Date);
                    withBlock.Parameters.AddWithValue("@Payment_Amount", Payment_Amount);
                    withBlock.Parameters.AddWithValue("@Payment_CurrencyID", Payment_CurrencyID);
                    withBlock.Parameters.AddWithValue("@Payment_ReferenceNo", Payment_ReferenceCode);
                    withBlock.Parameters.AddWithValue("@Payment_Gateway", Payment_Gateway);
                    withBlock.Parameters.AddWithValue("@Payment_CurrencyRate", Payment_ExchangeRate);
                    withBlock.ExecuteNonQuery();
                }

                SqlCommand cmdDeleteExistingLinkedOrders = new SqlCommand("_spKartrisPayments_DeleteLinkedOrders", sqlConn, savePoint);
                {
                    var withBlock = cmdDeleteExistingLinkedOrders;
                    withBlock.CommandType = CommandType.StoredProcedure;
                    withBlock.Transaction = savePoint;
                    withBlock.Parameters.AddWithValue("@OP_PaymentID", Payment_ID);
                    withBlock.ExecuteNonQuery();
                }

                if (lcLinkedOrders.Count > 0)
                {
                    foreach (ListItem item in lcLinkedOrders)
                    {
                        int intOrderID = System.Convert.ToInt32(item.Value);
                        SqlCommand cmdLinkedOrders = new SqlCommand("_spKartrisPayments_AddLinkedOrder", sqlConn, savePoint);
                        {
                            var withBlock = cmdLinkedOrders;
                            withBlock.CommandType = CommandType.StoredProcedure;
                            withBlock.Transaction = savePoint;
                            withBlock.Parameters.AddWithValue("@OP_PaymentID", Payment_ID);
                            withBlock.Parameters.AddWithValue("@OP_OrderID", intOrderID);
                            withBlock.Parameters.AddWithValue("@OP_OrderCanceled", 0);
                            withBlock.ExecuteNonQuery();
                        }
                    }
                }

                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Orders, System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully"), CreateQuery(cmd), Payment_ID, sqlConn, savePoint);

                // If we reach here, no errors, so commit the transaction
                savePoint.Commit();
                sqlConn.Close();
                return Payment_ID;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
                // Something went wrong, so rollback the order
                if (!savePoint == null)
                    savePoint.Rollback();
                return 0;
                throw;
            }                // Bubble up the exception
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close(); savePoint.Dispose();
                }
            }
        }
    }
    public DataTable _GetPaymentByCustomerID(int CustomerID)
    {
        return PaymentsAdptr._GetByCustomerID(CustomerID);
    }
    public DataTable _GetPaymentByID(long PaymentID)
    {
        return PaymentsAdptr._Get(PaymentID);
    }
    public int _DeletePayment(long PaymentID)
    {
        return PaymentsAdptr._Delete(PaymentID);
    }
    public DataTable _GetPaymentLinkedOrders(long PaymentID)
    {
        return PaymentsAdptr._GetLinkedOrders(PaymentID);
    }
    public static DataTable _GetPaymentsFilteredList(string Payment_FilterType, string Payment_Gateway, DateTime Payment_Date, int intPageNo, int Limit)
    {
        if (Payment_Date == DateTime.MinValue)
            Payment_Date = DateTime.Today;
        return PaymentsAdptr._GetFilteredList(Payment_FilterType, Payment_Gateway, Payment_Date, intPageNo, Limit);
    }
    public static int _GetPaymentsFilteredListCount(string Payment_FilterType, string Payment_Gateway, DateTime Payment_Date)
    {
        if (Payment_Date == DateTime.MinValue)
            Payment_Date = DateTime.Today;
        return PaymentsAdptr._GetFilteredListCount(Payment_FilterType, Payment_Gateway, Payment_Date);
    }
    public double _GetPaymentTotalByCustomerID(int CustomerID)
    {
        return PaymentsAdptr._GetCustomerTotal(CustomerID);
    }
    public int _GetCustomerIDbyOrderID(int O_ID)
    {
        return Adptr._GetCustomerIDbyOrderID(O_ID);
    }



    private static void Log(string strText)
    {
    }

    /// <summary>
    ///     ''' ADD ORDER - Used by the checkout page to process an order before redirecting to the payment gateway.
    ///     ''' Steps involve creating a user account if we have a first time customer, adding of new billing and shipping addresses 
    ///     ''' and the creation of the actual order record. All are done in a single transaction so if any of these steps fail, everything 
    ///     ''' can be rolled back.
    ///     ''' </summary>
    ///     ''' <returns>Returns the newly created order ID</returns>
    public int Add(int C_ID, string strUserEmailAddress, string strUserPassword, KartrisClasses.Address BillingAddress, KartrisClasses.Address ShippingAddress, bool blnSameShippingAsBilling, Kartris.Basket BasketObject, List<Kartris.BasketItem> BasketArray, string strOrderDetails, string strGatewayName, int intLanguageID, int intCurrencyID, int intGatewayCurrencyID, bool blnOrderEmails, string strShippingMethod, double numGatewayTotalPrice, string strEUVATNumber, string strPromotionDescription, string strPurchaseOrderNo, string strComments, bool blnIsGuest)
    {

        // Could be possible for shipping method to be blank. Seen it happen.
        // Let's bounce user back to checkout if that happens.
        if (strShippingMethod == "")
            System.Web.HttpContext.Current.Response.Redirect("~/Checkout.aspx");

        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand();
            cmd.CommandText = "spKartrisOrders_Add";
            SqlTransaction savePoint = null;
            cmd.CommandType = CommandType.StoredProcedure;
            Log("Before Try");
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;

                int O_ID = 0;
                bool blnNewUser;
                string strBillingAddressText = "";
                string strShippingAddressText = "";

                Kartris.Basket objBasket = BasketObject;
                UsersBLL objUsersBLL = new UsersBLL();
                List<Kartris.BasketItem> arrBasketItems = BasketArray;

                // if C_ID has a value then we have a legitimate user - proceed with the order transaction
                if (C_ID == 0)
                {
                    // Add a new User record
                    string strRandomSalt = Membership.GeneratePassword(20, 0);
                    SqlCommand cmdAddOrderUser = new SqlCommand("spKartrisUsers_Add", sqlConn, savePoint);
                    cmdAddOrderUser.CommandType = CommandType.StoredProcedure;
                    cmdAddOrderUser.Parameters.AddWithValue("@U_EmailAddress", strUserEmailAddress);
                    cmdAddOrderUser.Parameters.AddWithValue("@U_Password", objUsersBLL.EncryptSHA256Managed(strUserPassword, strRandomSalt));
                    cmdAddOrderUser.Parameters.AddWithValue("@U_SaltValue", strRandomSalt);
                    cmdAddOrderUser.Parameters.AddWithValue("@U_GDPR_SignupIP", CkartrisEnvironment.GetClientIPAddress());
                    cmdAddOrderUser.Parameters.AddWithValue("@U_GDPR_IsGuest", blnIsGuest);
                    C_ID = cmdAddOrderUser.ExecuteScalar();
                    blnNewUser = true;
                }

                if (C_ID > 0)
                {
                    string strFullName = "";
                    if (BillingAddress != null)
                        strFullName = BillingAddress.FullName;
                    if (!(string.IsNullOrEmpty(strEUVATNumber) && string.IsNullOrEmpty(strFullName)))
                    {
                        SqlCommand cmdUpdateUserNameandEUVAT = new SqlCommand("spKartrisUsers_UpdateNameAndEUVAT", sqlConn, savePoint);
                        cmdUpdateUserNameandEUVAT.CommandType = CommandType.StoredProcedure;
                        cmdUpdateUserNameandEUVAT.Parameters.AddWithValue("@U_ID", C_ID);
                        cmdUpdateUserNameandEUVAT.Parameters.AddWithValue("@U_AccountHolderName", strFullName);
                        cmdUpdateUserNameandEUVAT.Parameters.AddWithValue("@U_CardholderEUVATNum", strEUVATNumber);
                        cmdUpdateUserNameandEUVAT.ExecuteNonQuery();
                    }
                }

                // Add a new billing address - if a new one is entered
                if (BillingAddress != null)
                {
                    if (BillingAddress.ID == 0)
                    {
                        {
                            var withBlock = BillingAddress;
                            SqlCommand cmdAddUpdateOrderAddress = new SqlCommand("spKartrisUsers_AddUpdateAddress", sqlConn, savePoint);
                            cmdAddUpdateOrderAddress.CommandType = CommandType.StoredProcedure;
                            cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_UserID", C_ID);
                            cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_Label", withBlock.Label);
                            cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_Name", withBlock.FullName);
                            cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_Company", withBlock.Company);
                            cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_StreetAddress", withBlock.StreetAddress);
                            cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_TownCity", withBlock.TownCity);
                            cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_County", withBlock.County);
                            cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_PostCode", withBlock.Postcode);
                            cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_Country", withBlock.CountryID);
                            cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_Telephone", withBlock.Phone);
                            cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_Type", Interaction.IIf(blnSameShippingAsBilling, "u", "b"));
                            cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_MakeDefault", Interaction.IIf(blnNewUser, 1, 0));
                            cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_ID", 0);

                            cmdAddUpdateOrderAddress.ExecuteNonQuery();
                        }
                    }
                }

                // Add a new shipping address - if a new one is entered
                if (!blnSameShippingAsBilling)
                {
                    if (ShippingAddress != null)
                    {
                        if (ShippingAddress.ID == 0)
                        {
                            {
                                var withBlock = ShippingAddress;
                                SqlCommand cmdAddUpdateOrderAddress = new SqlCommand("spKartrisUsers_AddUpdateAddress", sqlConn, savePoint);
                                cmdAddUpdateOrderAddress.CommandType = CommandType.StoredProcedure;
                                cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_UserID", C_ID);
                                cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_Label", withBlock.Label);
                                cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_Name", withBlock.FullName);
                                cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_Company", withBlock.Company);
                                cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_StreetAddress", withBlock.StreetAddress);
                                cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_TownCity", withBlock.TownCity);
                                cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_County", withBlock.County);
                                cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_PostCode", withBlock.Postcode);
                                cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_Country", withBlock.CountryID);
                                cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_Telephone", withBlock.Phone);
                                cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_Type", "s");
                                cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_MakeDefault", Interaction.IIf(blnNewUser, 1, 0));
                                cmdAddUpdateOrderAddress.Parameters.AddWithValue("@ADR_ID", 0);

                                cmdAddUpdateOrderAddress.ExecuteNonQuery();
                            }
                        }
                    }
                }

                // Build the billing address string to be used in the order record
                if (BillingAddress != null)
                {
                    {
                        var withBlock = BillingAddress;
                        strBillingAddressText = withBlock.FullName + Constants.vbCrLf + withBlock.Company + Constants.vbCrLf + withBlock.StreetAddress + Constants.vbCrLf + withBlock.TownCity + Constants.vbCrLf + withBlock.County + Constants.vbCrLf + withBlock.Postcode + Constants.vbCrLf + withBlock.Country.Name + Constants.vbCrLf + withBlock.Phone;
                    }
                }

                // Build the shipping address string to be used in the order record
                if (blnSameShippingAsBilling)
                    strShippingAddressText = strBillingAddressText;
                else if (ShippingAddress != null)
                {
                    {
                        var withBlock = ShippingAddress;
                        strShippingAddressText = withBlock.FullName + Constants.vbCrLf + withBlock.Company + Constants.vbCrLf + withBlock.StreetAddress + Constants.vbCrLf + withBlock.TownCity + Constants.vbCrLf + withBlock.County + Constants.vbCrLf + withBlock.Postcode + Constants.vbCrLf + withBlock.Country.Name + Constants.vbCrLf + withBlock.Phone;
                    }
                }

                // '// get affiliate id from session variable and compare it to user current affiliate id
                // ' update user affiliate id only if differ
                int intAffiliateID = 0;
                double numAffiliatePercentage = 0;
                if (HttpContext.Current.Session("C_AffiliateID") != null)
                {
                    intAffiliateID = System.Convert.ToInt32(HttpContext.Current.Session("C_AffiliateID"));
                    if (intAffiliateID > 0)
                    {
                        if (AffiliateBLL.IsCustomerAffiliate(intAffiliateID))
                        {
                            SqlCommand cmdUpdateAffiliate = new SqlCommand("spKartrisCustomer_UpdateAffiliate", sqlConn, savePoint);
                            cmdUpdateAffiliate.CommandType = CommandType.StoredProcedure;
                            {
                                var withBlock = cmdUpdateAffiliate.Parameters;
                                withBlock.AddWithValue("@Type", 3);
                                withBlock.AddWithValue("@UserID", C_ID);
                                withBlock.AddWithValue("@AffiliateCommission", 0);
                                withBlock.AddWithValue("@AffiliateID", intAffiliateID);
                            }
                            cmdUpdateAffiliate.ExecuteNonQuery();
                            numAffiliatePercentage = System.Convert.ToDouble(Adptr.GetAffiliateCommission(intAffiliateID));
                        }
                    }
                }

                double numCurrencyRate = System.Convert.ToDouble(CurrenciesBLL.CurrencyRate(intCurrencyID));

                int intWishlistID = 0;
                if (HttpContext.Current.Session("WL_ID") != null)
                    intWishlistID = HttpContext.Current.Session("WL_ID");

                float numTaxDue;
                if (objBasket.ApplyTax)
                    numTaxDue = 1;
                else
                    numTaxDue = 0;

                // Create the actual order record
                {
                    var withBlock = cmd.Parameters;
                    withBlock.AddWithValue("@O_CustomerID", C_ID);
                    withBlock.AddWithValue("@O_Details", strOrderDetails);
                    withBlock.AddWithValue("@O_ShippingPrice", objBasket.ShippingPrice.ExTax);
                    withBlock.AddWithValue("@O_ShippingTax", objBasket.ShippingPrice.TaxAmount);
                    withBlock.AddWithValue("@O_DiscountPercentage", objBasket.CustomerDiscountPercentage);
                    withBlock.AddWithValue("@O_AffiliatePercentage", numAffiliatePercentage);
                    withBlock.AddWithValue("@O_TotalPrice", objBasket.FinalPriceIncTax);
                    withBlock.AddWithValue("@O_Date", CkartrisDisplayFunctions.NowOffset);
                    withBlock.AddWithValue("@O_PurchaseOrderNo", Interaction.IIf(Strings.Trim(strPurchaseOrderNo) != "", strPurchaseOrderNo, ""));
                    withBlock.AddWithValue("@O_SecurityID", 0);
                    withBlock.AddWithValue("@O_Sent", 0);
                    withBlock.AddWithValue("@O_Invoiced", 0);
                    withBlock.AddWithValue("@O_Shipped", 0);
                    withBlock.AddWithValue("@O_Paid", 0);
                    withBlock.AddWithValue("@O_Status", Payment.Serialize(objBasket));
                    withBlock.AddWithValue("@O_LastModified", CkartrisDisplayFunctions.NowOffset);
                    withBlock.AddWithValue("@O_WishListID", intWishlistID);
                    withBlock.AddWithValue("@O_CouponCode", objBasket.CouponCode);
                    withBlock.AddWithValue("@O_CouponDiscountTotal", objBasket.CouponDiscount.IncTax);
                    withBlock.AddWithValue("@O_PricesIncTax", IIf(objBasket.PricesIncTax, 1, 0));
                    withBlock.AddWithValue("@O_TaxDue", numTaxDue);
                    withBlock.AddWithValue("@O_PaymentGateWay", strGatewayName);
                    withBlock.AddWithValue("@O_ReferenceCode", "");
                    withBlock.AddWithValue("@O_LanguageID", intLanguageID);
                    withBlock.AddWithValue("@O_CurrencyID", intCurrencyID);
                    withBlock.AddWithValue("@O_TotalPriceGateway", numGatewayTotalPrice);
                    withBlock.AddWithValue("@O_CurrencyIDGateway", intGatewayCurrencyID);
                    withBlock.AddWithValue("@O_AffiliatePaymentID", 0);
                    withBlock.AddWithValue("@O_AffiliateTotalPrice", objBasket.TotalValueToAffiliate);
                    withBlock.AddWithValue("@O_SendOrderUpdateEmail", Interaction.IIf(blnOrderEmails, 1, 0));
                    withBlock.AddWithValue("@O_OrderHandlingCharge", objBasket.OrderHandlingPrice.ExTax);
                    withBlock.AddWithValue("@O_OrderHandlingChargeTax", objBasket.OrderHandlingPrice.TaxAmount);
                    withBlock.AddWithValue("@O_CurrencyRate", numCurrencyRate);
                    withBlock.AddWithValue("@O_ShippingMethod", strShippingMethod);
                    withBlock.AddWithValue("@O_BillingAddress", strBillingAddressText);
                    withBlock.AddWithValue("@O_ShippingAddress", strShippingAddressText);
                    withBlock.AddWithValue("@O_PromotionDiscountTotal", objBasket.PromotionDiscount.IncTax);
                    withBlock.AddWithValue("@O_PromotionDescription", strPromotionDescription);
                    withBlock.AddWithValue("@O_Comments", Interaction.IIf(Strings.Trim(strComments) != "", strComments, ""));
                }
                O_ID = cmd.ExecuteScalar();

                // Cycle through the basket items and add each as order invoice rows
                if (!(arrBasketItems == null))
                {
                    BasketItem BasketItem = new BasketItem();
                    for (int i = 0; i <= arrBasketItems.Count - 1; i++)
                    {
                        BasketItem = arrBasketItems[i];
                        {
                            var withBlock = BasketItem;
                            SqlCommand cmdAddInvoiceRows = new SqlCommand("spKartrisOrders_InvoiceRowsAdd", sqlConn, savePoint);
                            cmdAddInvoiceRows.CommandType = CommandType.StoredProcedure;


                            cmdAddInvoiceRows.Parameters.AddWithValue("@IR_OrderNumberID", O_ID);
                            cmdAddInvoiceRows.Parameters.AddWithValue("@IR_VersionCode", withBlock.VersionCode);
                            cmdAddInvoiceRows.Parameters.AddWithValue("@IR_VersionName", IIf(withBlock.VersionName != withBlock.ProductName | InStr(withBlock.VersionName, withBlock.ProductName) == 0, withBlock.ProductName + " - " + withBlock.VersionName, withBlock.VersionName));
                            cmdAddInvoiceRows.Parameters.AddWithValue("@IR_Quantity", System.Convert.ToSingle(withBlock.Quantity));
                            cmdAddInvoiceRows.Parameters.AddWithValue("@IR_PricePerItem", withBlock.IR_PricePerItem);
                            cmdAddInvoiceRows.Parameters.AddWithValue("@IR_TaxPerItem", withBlock.IR_TaxPerItem);

                            cmdAddInvoiceRows.Parameters.AddWithValue("@IR_ExcludeFromCustomerDiscount", withBlock.ExcludeFromCustomerDiscount);

                            StringBuilder sbdExtraText = new StringBuilder(withBlock.OptionText);
                            if (!string.IsNullOrEmpty(withBlock.CustomText))
                            {
                                if (!string.IsNullOrEmpty(withBlock.OptionText))
                                    sbdExtraText.Append("<br/>");
                                sbdExtraText.Append("[" + withBlock.CustomText + "]");
                            }
                            cmdAddInvoiceRows.Parameters.AddWithValue("@IR_OptionsText", sbdExtraText.ToString());

                            int strOutputValue = cmdAddInvoiceRows.ExecuteScalar();
                            if (strOutputValue == 0)
                                throw new Exception("Failed adding invoice row");
                        }
                    }
                }


                if (objBasket.PromotionDiscount.IncTax < 0)
                {
                    List<Kartris.Promotion> objPromotions = new List<Kartris.Promotion>();
                    ArrayList objPromotionsDiscount = new ArrayList();
                    BasketBLL.CalculatePromotions(objBasket, objPromotions, objPromotionsDiscount, false);
                    foreach (PromotionBasketModifier objPromotion in objPromotionsDiscount)
                    {
                        try
                        {
                            SqlCommand cmdAddPromotionLinks = new SqlCommand("spKartrisOrdersPromotions_Add", sqlConn, savePoint);
                            {
                                var withBlock = cmdAddPromotionLinks;
                                withBlock.CommandType = CommandType.StoredProcedure;
                                withBlock.Parameters.AddWithValue("@OrderID", O_ID);
                                withBlock.Parameters.AddWithValue("@PromotionID", objPromotion.PromotionID);
                                withBlock.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }

                // If we reach here, no errors, so commit the transaction
                savePoint.Commit();
                sqlConn.Close();
                if (blnNewUser)
                {
                    if (Membership.ValidateUser(strUserEmailAddress, strUserPassword))
                        FormsAuthentication.SetAuthCookie(strUserEmailAddress, true);
                }


                return O_ID;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
                // Something went wrong, so rollback the order
                if (!savePoint == null)
                    savePoint.Rollback();
                return 0;
                throw;
            }                // Bubble up the exception
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close(); savePoint.Dispose();
                }
            }
        }
        return 0;
    }

    /// <summary>
    ///     ''' This is the method used by the callback (callback.aspx) page to update a successful order.
    ///     ''' </summary>
    public int CallbackUpdate(long O_ID, string O_ReferenceCode, DateTime O_LastModified, bool O_Sent, bool O_Invoiced, bool O_Paid, string O_Status, string O_CouponCode, int O_WLID, int O_CustomerID, short GatewayCurrencyID, string O_GatewayName, double O_Amount)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand();
            cmd.CommandText = "spKartrisOrders_CallbackUpdate";
            SqlTransaction savePoint = null;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;
                // Perform the update on the DataTable
                {
                    var withBlock = cmd.Parameters;
                    withBlock.AddWithValue("@O_ID", O_ID);
                    withBlock.AddWithValue("@O_ReferenceCode", O_ReferenceCode);
                    withBlock.AddWithValue("@O_LastModified", O_LastModified);
                    withBlock.AddWithValue("@O_Sent", Interaction.IIf(O_Sent, 1, 0));
                    withBlock.AddWithValue("@O_Invoiced", Interaction.IIf(O_Invoiced, 1, 0));
                    withBlock.AddWithValue("@O_Paid", Interaction.IIf(O_Paid, 1, 0));
                    withBlock.AddWithValue("@O_Status", O_Status);
                    withBlock.AddWithValue("@O_WLID", O_WLID);
                }


                int returnValue = cmd.ExecuteScalar();
                if (returnValue != O_ID)
                    throw new Exception("ID is 0? Something's not right");
                if (!string.IsNullOrEmpty(O_CouponCode))
                {
                    SqlCommand cmdRecordOrderCoupon = new SqlCommand("spKartrisOrders_CouponUsed", sqlConn, savePoint);
                    cmdRecordOrderCoupon.CommandType = CommandType.StoredProcedure;
                    cmdRecordOrderCoupon.Parameters.AddWithValue("@CouponCode", O_CouponCode);
                    cmdRecordOrderCoupon.ExecuteNonQuery();
                }

                // If CustomerID is supplied then that means that the call is coming from the callback page and we need to add a new payment record
                if (O_CustomerID > 0)
                {
                    SqlCommand cmdAddPayment = new SqlCommand("spKartrisPayments_Add", sqlConn, savePoint);
                    cmdAddPayment.CommandType = CommandType.StoredProcedure;
                    {
                        var withBlock = cmdAddPayment.Parameters;
                        withBlock.AddWithValue("@Payment_CustomerID", O_CustomerID);
                        withBlock.AddWithValue("@Payment_CurrencyID", GatewayCurrencyID);
                        withBlock.AddWithValue("@Payment_ReferenceNo", O_ReferenceCode);
                        withBlock.AddWithValue("@Payment_Date", O_LastModified);
                        withBlock.AddWithValue("@Payment_Amount", O_Amount);
                        withBlock.AddWithValue("@Payment_Gateway", O_GatewayName);
                        withBlock.AddWithValue("@Payment_CurrencyRate", CurrenciesBLL.CurrencyRate(GatewayCurrencyID));
                    }
                    returnValue = cmdAddPayment.ExecuteScalar();
                    if (returnValue == 0)
                        throw new Exception("PaymentID is 0? Something's not right");

                    SqlCommand cmdOrderPaymentsLink = new SqlCommand("spKartrisOrderPaymentsLink_Add", sqlConn, savePoint);
                    cmdOrderPaymentsLink.CommandType = CommandType.StoredProcedure;
                    cmdOrderPaymentsLink.Parameters.AddWithValue("@OP_PaymentID", returnValue);
                    cmdOrderPaymentsLink.Parameters.AddWithValue("@OP_OrderID", O_ID);
                    cmdOrderPaymentsLink.ExecuteNonQuery();
                }

                // If we reach here, no errors, so commit the transaction
                savePoint.Commit();
                sqlConn.Close();

                // Try to update customer balance before returning value
                UsersBLL objUsersBLL = new UsersBLL();
                try
                {
                    objUsersBLL.UpdateCustomerBalance(O_CustomerID, System.Convert.ToDecimal(_GetPaymentTotalByCustomerID(O_CustomerID) - _GetOrderTotalByCustomerID(O_CustomerID)));
                }
                catch (Exception ex)
                {
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
                // If we reach here, there was an error, so rollback the transaction
                savePoint.Rollback();
                return 0;
            }
        }
    }
    /// <summary>
    ///     ''' This method is used to update an order's data field before passing the customer to the payment gateway
    ///     ''' </summary>
    public int DataUpdate(long O_ID, string O_Data)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmd = sqlConn.CreateCommand();
            cmd.CommandText = "spKartrisOrders_DataUpdate";
            SqlTransaction savePoint = null;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmd.Transaction = savePoint;
                // Perform the update on the DataTable
                cmd.Parameters.AddWithValue("@O_ID", O_ID);
                cmd.Parameters.AddWithValue("@O_Data", O_Data);

                int returnValue = cmd.ExecuteScalar();
                if (returnValue != O_ID)
                    throw new Exception("ID is 0? Something's not right");

                // If we reach here, no errors, so commit the transaction
                savePoint.Commit();
                sqlConn.Close();

                // Try to update customer balance before returning value
                int O_CustomerID = 0;
                UsersBLL objUsersBLL = new UsersBLL();
                OrdersBLL objOrdersBLL = new OrdersBLL();
                try
                {
                    O_CustomerID = objOrdersBLL._GetCustomerIDbyOrderID(O_ID);
                    objUsersBLL.UpdateCustomerBalance(O_CustomerID, System.Convert.ToDecimal(_GetPaymentTotalByCustomerID(O_CustomerID) - _GetOrderTotalByCustomerID(O_CustomerID)));
                }
                catch (Exception ex)
                {
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
                // If we reach here, there was an error, so rollback the transaction
                savePoint.Rollback();
                return 0;
            }
        }
    }



    /// <summary>
    ///     ''' This is the method used by other payment gateways to update an order through its reference code.
    ///     ''' </summary>
    public static int UpdateByReferenceCode(string O_ReferenceCode, DateTime O_LastModified, bool O_Sent, bool O_Invoiced, bool O_Paid, string O_Status)
    {
        try
        {

            // Perform the update on the DataTable
            int returnValue = Adptr.UpdateByReferenceCode(O_ReferenceCode, O_LastModified, O_Sent, O_Invoiced, O_Paid, O_Status);
            if (returnValue != O_ReferenceCode)
                throw new Exception("The returned reference code doesn't match? Something's not right");
            // If we reach here, no errors, so commit the transaction
            return returnValue;
        }

        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
            // If we reach here, there was an error, so rollback the transaction
            return 0;
        }
    }

    public DataTable GetQBQueue()
    {
        return Adptr.GetQBQueue;
    }

    public int UpdateQBSent(int intOrderID)
    {
        return Adptr.UpdateQBSent(intOrderID);
    }

    public static DataTable GetCardTypeList()
    {
        CardTypesTblAdptr CardTypesAdptr = new CardTypesTblAdptr();
        DataTable dtCardType = CardTypesAdptr.GetCardTypeList;
        CardTypesAdptr = null/* TODO Change to default(_) if this is not a reference type */;
        return dtCardType;
    }
}
