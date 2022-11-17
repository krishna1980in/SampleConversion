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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using CkartrisBLL;
using CkartrisDataManipulation;
using KartSettingsManager;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

internal partial class Callback : PageBaseClass
{

    private Kartris.Interfaces.PaymentGateway clsPlugin;

    public Callback()
    {
        this.Load += Page_Load;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string strCallbackError = "";
            string strResult = "";
            string strUpdateResult = "";
            var strMultibancoData = Strings.Split("", " ");
            string strBodyText = "";
            bool blnFullDisplay = true;
            if (Request.QueryString("d") == "off")
                blnFullDisplay = false;
            string strGatewayName = Strings.StrConv(Request.QueryString("g"), Constants.vbProperCase);
            Kartris.Interfaces.PaymentGateway clsPlugin = default;

            // Callback Step 0 - normal callback
            // Callback Step 1 - update order but don't display full HTML if d=off QS is passed, write gateway dll output to screen
            // Callback Step 2 - don't update order, just display result as usual
            // Callback Step 3 - don't update order, write gateway dll output to screen
            int intCallbackStep;
            try
            {
                intCallbackStep = Request.QueryString("step");
            }
            catch (Exception ex)
            {
                intCallbackStep = 0;
            }

            // -----------------------------------------------------
            // CALLBACK
            // We need to know which gateway to process. This can
            // be done in two ways. The Callback.aspx page can be
            // sent a querystring value named 'g', for example:

            // Callback.aspx?g=Paypal

            // (the value of 'g' should match the name of the
            // payment system's plugin folder)

            // Some payment systems won't pass this querystring
            // value and form values at the same time. So instead,
            // the following format can be used:

            // Callback-Paypal.aspx

            // Kartris will recognize this and map it to the
            // querystring version above.
            // -----------------------------------------------------
            if (!string.IsNullOrEmpty(strGatewayName))
            {
                // Let's turn some comment gateway names which
                // might be sent in any case format to the correct
                // mixed case format so they look nice.
                if (Strings.LCase(strGatewayName) == "sagepaydirect")
                    strGatewayName = "SagePayDirect";
                if (Strings.LCase(strGatewayName) == "sagepay")
                    strGatewayName = "SagePay";
                if (Strings.LCase(strGatewayName) == "cp")
                    strGatewayName = "Cactuspay";
                if (Strings.LCase(strGatewayName) == "easypay")
                {
                    strGatewayName = "EasypayCreditCard";
                    CreateQueryStringParams("e", Request.QueryString.Get("?e"));
                    RemoveQueryStringParams("?e");
                    // Request.QueryString = 

                }

                // Loop through incoming fields if form post to this page
                foreach (var fldName in Request.Form)
                {
                    if (!string.IsNullOrEmpty(strResult))
                        strResult += ":-:";
                    strResult += Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("FF_", fldName), ":*:")) + Request.Form(fldName);
                }

                // Loop through incoming fields if URL with 
                foreach (var fldName in Request.QueryString)
                {
                    if (!string.IsNullOrEmpty(strResult))
                        strResult += ":-:";
                    strResult += Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("QS_", fldName), ":*:")) + Request.QueryString(fldName);
                }

                // Load in the payment gateway in question
                // This is why it is important that the name is
                // passed correctly when setting up the callback.aspx
                clsPlugin = Payment.PPLoader(strGatewayName);

                // According to the dictionary, 'referrer' is the
                // correct spelling and whoever decided on 'referer'
                // is the one who is wrong :)
                string strReferrerURL;
                try
                {
                    strReferrerURL = Request.UrlReferrer.ToString();
                }
                catch (Exception ex)
                {
                    strReferrerURL = Request.ServerVariables("HTTP_REFERER");
                }

                strResult = clsPlugin.ProcessCallback(strResult, strReferrerURL);

                // For Easypay gateway only
                if (strGatewayName.ToLower() == "easypaycreditcard" & Request.QueryString("a") == "notify")
                {
                    blnFullDisplay = false;

                    // Get And parse XML file
                    var XMLreader = new XmlDocument();
                    try
                    {
                        XMLreader.Load(new StringReader(strResult));
                        string strQs = "";
                        RemoveQueryStringParams("a");
                        CreateQueryStringParams("a", "update");
                        CreateQueryStringParams("ep_key", "1078");
                        CreateQueryStringParams("ep_doc", XMLreader.SelectSingleNode("//getautomb_key/ep_doc").InnerText);

                        // Loop through incoming fields if URL with 
                        foreach (var fldName in Request.QueryString)
                        {
                            if (!string.IsNullOrEmpty(strQs))
                                strQs += ":-:";
                            strQs += Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("QS_", fldName), ":*:")) + Request.QueryString(fldName);
                        }

                        strUpdateResult = clsPlugin.ProcessCallback(strQs, strReferrerURL);
                        strMultibancoData = strUpdateResult.Split("&");
                    }
                    catch (Exception ex)
                    {
                        Response.Write("<br>Error: <br>" + ex.Message);

                    }

                }

                // -----------------------------------------------------
                // CALLBACK SUCCESSFUL
                // Lookup order to being processing it
                // -----------------------------------------------------
                if (clsPlugin.CallbackSuccessful)
                {

                    int O_ID = clsPlugin.CallbackOrderID;
                    var objOrdersBLL = new OrdersBLL();
                    var objUsersBLL = new UsersBLL();
                    DataTable tblOrder = objOrdersBLL.GetOrderByID(O_ID);

                    string O_CouponCode = "";
                    double O_TotalPriceGateway = 0d;
                    int O_WLID = 0;
                    int O_CustomerID = 0;
                    int O_LanguageID = 0;
                    int O_CurrencyIDGateway = 0;
                    string strBasketBLL = "";
                    if (tblOrder.Rows.Count > 0)
                    {
                        if (tblOrder.Rows(0)("O_Paid") == 0 || intCallbackStep == 2)
                        {
                            // Store the order details
                            O_CouponCode = FixNullFromDB(tblOrder.Rows(0)("O_CouponCode"));
                            O_TotalPriceGateway = tblOrder.Rows(0)("O_TotalPriceGateway");
                            O_WLID = tblOrder.Rows(0)("O_WishListID");
                            O_CustomerID = tblOrder.Rows(0)("O_CustomerID");
                            O_LanguageID = tblOrder.Rows(0)("O_LanguageID");
                            strBasketBLL = tblOrder.Rows(0)("O_Status");
                            strBodyText = tblOrder.Rows(0)("O_Details");
                            O_CurrencyIDGateway = tblOrder.Rows(0)("O_CurrencyIDGateway");
                        }
                        else
                        {
                            strCallbackError = "Callback Failure: " + Constants.vbCrLf + "Order already submitted (ID: " + O_ID + ", Order sent: 'yes')";
                        }
                    }
                    else
                    {
                        // The order ID we looked up was not
                        // found in our database
                        strCallbackError = "Callback Failure: " + Constants.vbCrLf + "Order id not found (" + O_ID + ")";
                    }

                    // -----------------------------------------------------
                    // NO ERRORS
                    // Proceed to process order and despatch confirmation
                    // -----------------------------------------------------
                    if (string.IsNullOrEmpty(strCallbackError))
                    {


                        if (intCallbackStep != 2 & intCallbackStep != 3)
                        {

                            // If using mailchimp, we need to delete the basket because we don't
                            // want the abandoned cart mail to go out
                            if (KartSettingsManager.GetKartConfig("general.mailchimp.enabled") == "y")
                            {
                                try
                                {
                                    // This should restore the customer's basket
                                    BasketBLL.RecoverAutosaveBasket(O_CustomerID);

                                    // Need to create a temp user for the code below to work
                                    var tempKartrisUser = new KartrisMemberShipUser(O_CustomerID, objUsersBLL.GetEmailByID(O_CustomerID), 0, 0, 0, 0, 1, true);
                                    Kartris.Basket kartrisBasket = Session("Basket");

                                    CkartrisFormatErrors.LogError("Get Session Basket");
                                    CkartrisFormatErrors.LogError("Get Session Basket: " + kartrisBasket.ToString());

                                    var mailChimpLib = new MailChimpBLL(tempKartrisUser, kartrisBasket, CurrenciesBLL.CurrencyCode(Session("CUR_ID")));
                                    CkartrisFormatErrors.LogError("MailChimp callback customer");
                                    // Dim mcCustomer As MailChimp.Net.Models.Customer = mailChimpLib.GetCustomer(tempKartrisUser.ID).Result
                                    // Dim customerJson As String = New JavaScriptSerializer().Serialize(mcCustomer)
                                    // CkartrisFormatErrors.LogError("MailchimpBLL Product before basketItem JSON: " & customerJson)
                                    CkartrisFormatErrors.LogError("MailChimp callback order");
                                    CkartrisFormatErrors.LogError("MailChimp callback O_CustomerID");
                                    // Dim mcOrder As MailChimp.Net.Models.Order = mailChimpLib.AddOrder(mcCustomer, "cart_" & O_ID.ToString()).Result
                                    MailChimp.Net.Models.Order mcOrder = mailChimpLib.AddOrderByCustomerId(tempKartrisUser.ID, "cart_" + O_ID.ToString()).Result;

                                    CkartrisFormatErrors.LogError("MailChimp callback deletecart");
                                    bool mcDeleteCart = mailChimpLib.DeleteCart("cart_" + O_ID).Result;
                                    // Log the success
                                    CkartrisFormatErrors.LogError("Mailchimp basket was deleted successfully");
                                }
                                catch (Exception ex)
                                {
                                    // Log the error
                                    CkartrisFormatErrors.LogError(ex.Message);
                                }
                            }

                            bool blnCheckInvoicedOnPayment = GetKartConfig("frontend.orders.checkinvoicedonpayment") == "y";
                            bool blnCheckReceivedOnPayment = GetKartConfig("frontend.orders.checkreceivedonpayment") == "y";

                            // -----------------------------------------------------
                            // UPDATE ORDER STATUS
                            // Set invoiced and received checkboxes, depending on
                            // config settings
                            // -----------------------------------------------------
                            int intUpdateResult = objOrdersBLL.CallbackUpdate(O_ID, clsPlugin.CallbackReferenceCode, CkartrisDisplayFunctions.NowOffset, true, blnCheckInvoicedOnPayment, blnCheckReceivedOnPayment, GetGlobalResourceObject("Email", "EmailText_OrderTime") + " " + CkartrisDisplayFunctions.NowOffset, O_CouponCode, O_WLID, O_CustomerID, O_CurrencyIDGateway, clsPlugin.GatewayName, O_TotalPriceGateway);
                            if (clsPlugin.GatewayName.ToLower == "easypaycreditcard" & Request.QueryString("a") == "update")
                            {
                                try
                                {
                                    string notes = "Multibanco order with Entity: " + strMultibancoData[2].Split(":")[1] + " and Reference:" + strMultibancoData[3].Split(":")[1];
                                    objOrdersBLL._UpdateStatus(O_ID, true, true, tblOrder.Rows(0)("O_Shipped"), true, tblOrder.Rows(0)("O_Status"), notes, tblOrder.Rows(0)("O_Cancelled"));
                                }
                                catch (Exception ex)
                                {
                                }
                            }


                            // Try to delete AUTOSAVE basket
                            var objBasketBLL = new BasketBLL();
                            try
                            {
                                objBasketBLL.DeleteSavedBasketByNameAndUserID(O_CustomerID, "AUTOSAVE");
                            }
                            catch (Exception ex)
                            {

                            }

                            // -----------------------------------------------------
                            // FORMAT CONFIRMATION EMAIL
                            // -----------------------------------------------------
                            // Set some values for use later
                            bool blnUseHTMLOrderEmail = GetKartConfig("general.email.enableHTML") == "y";
                            string strCustomerEmailText = "";
                            string strStoreEmailText = "";
                            strBodyText = strBodyText.Replace("[orderid]", O_ID.ToString());
                            // we're in the callback page so obviously po_offlinedetails/bitcoin method is not being used
                            strBodyText = strBodyText.Replace("[poofflinepaymentdetails]", "");
                            strBodyText = strBodyText.Replace("[bitcoinpaymentdetails]", "");

                            if (KartSettingsManager.GetKartConfig("frontend.checkout.ordertracking") != "n")
                            {
                                if (!blnUseHTMLOrderEmail)
                                {
                                    // Add order tracking information at the top
                                    strCustomerEmailText = GetGlobalResourceObject("Email", "EmailText_OrderLookup") + Constants.vbCrLf + Constants.vbCrLf + WebShopURL() + "Customer.aspx" + Constants.vbCrLf + Constants.vbCrLf;
                                }
                            }
                            strCustomerEmailText += strBodyText;

                            if (!blnUseHTMLOrderEmail)
                            {
                                // Add in email header above that
                                strCustomerEmailText = GetGlobalResourceObject("Email", "EmailText_OrderReceived") + Constants.vbCrLf + Constants.vbCrLf + GetGlobalResourceObject("Kartris", "ContentText_OrderNumber") + ": " + O_ID + Constants.vbCrLf + Constants.vbCrLf + strCustomerEmailText;
                            }
                            else
                            {
                                strCustomerEmailText = strCustomerEmailText.Replace("[storeowneremailheader]", "");
                            }

                            string strFromEmail = LanguagesBLL.GetEmailFrom(O_LanguageID);

                            // -----------------------------------------------------
                            // SEND CONFIRMATION EMAIL
                            // To customer
                            // -----------------------------------------------------
                            if (KartSettingsManager.GetKartConfig("frontend.orders.emailcustomer") != "n")
                            {
                                SendEmail(strFromEmail, objUsersBLL.GetEmailByID(O_CustomerID), GetGlobalResourceObject("Email", "Config_Subjectline") + " (#" + O_ID + ")", strCustomerEmailText, default, default, default, default, blnUseHTMLOrderEmail);
                            }

                            // -----------------------------------------------------
                            // SEND CONFIRMATION EMAIL
                            // To store owner
                            // -----------------------------------------------------
                            if (KartSettingsManager.GetKartConfig("frontend.orders.emailmerchant") != "n")
                            {
                                if (!blnUseHTMLOrderEmail)
                                {
                                    strStoreEmailText = GetGlobalResourceObject("Email", "EmailText_StoreEmailHeader") + Constants.vbCrLf + Constants.vbCrLf + GetGlobalResourceObject("Kartris", "ContentText_OrderNumber") + ": " + O_ID + Constants.vbCrLf + Constants.vbCrLf + strBodyText;
                                }
                                else
                                {
                                    strStoreEmailText = strBodyText.Replace("[storeowneremailheader]", GetGlobalResourceObject("Email", "EmailText_StoreEmailHeader"));
                                }

                                SendEmail(strFromEmail, LanguagesBLL.GetEmailTo(1), GetGlobalResourceObject("Email", "Config_Subjectline2") + " (#" + O_ID + ")", strStoreEmailText, default, default, default, default, blnUseHTMLOrderEmail);
                            }

                            // Send an order notification to Windows Store App if enabled
                            PushKartrisNotification("o");
                        }
                        else
                        {
                            strBodyText = strBodyText.Replace("[orderid]", O_ID.ToString());
                            // we're in the callback page so obviously po_offline/bitcoin method is not being used
                            strBodyText = strBodyText.Replace("[poofflinepaymentdetails]", "");
                            strBodyText = strBodyText.Replace("[bitcoinpaymentdetails]", "");
                        }

                    }
                }
                else
                {
                    // Record error
                    strCallbackError = "Callback Failure: " + strResult + Constants.vbCrLf + clsPlugin.CallbackMessage + Constants.vbCrLf + "FF: " + Request.Form.ToString + Constants.vbCrLf + "QS: " + Request.QueryString.ToString;
                }
            }
            else
            {
                // No gateway name passed with callback, log error
                strCallbackError = "Callback Failure: Gateway name not specified. " + Constants.vbCrLf + "FF: " + Request.Form.ToString + Constants.vbCrLf + "QS: " + Request.QueryString.ToString;
            }


            // If there was no error then...
            if (string.IsNullOrEmpty(strCallbackError) | strGatewayName.ToLower() == "easypaycreditcard" & Request.QueryString("a") == "notify")
            {
                if (GetKartConfig("frontend.payment.debugmode.enabled") == "y")
                {
                    // -----------------------------------------------------
                    // LOG THE CALLBACK
                    // We log it to the error logs to avoid having to
                    // create a completely separate log system for this.
                    // Logging callbacks is useful for debugging cases
                    // where you think the callback is setup, but orders
                    // don't seem to be processed and moved to the 
                    // "completed" list. The log will show you firstly that
                    // the callback is occurring, and secondly will show
                    // you the values passed through both form fields and
                    // the querystring so you can figure out why these
                    // are not triggering the completion procedure.
                    // -----------------------------------------------------
                    CkartrisFormatErrors.LogError("Successful Callback Log (frontend.payment.debugmode.enabled=y): " + clsPlugin.CallbackMessage + Constants.vbCrLf + "FF: " + Request.Form.ToString + Constants.vbCrLf + "QS: " + Request.QueryString.ToString);
                }

                // Dim BasketObject As kartris.Basket = new OldBasketBLL
                BasketBLL.DeleteBasket();
                Session("Basket") = null;

                // -----------------------------------------------------
                // DISPLAY OUTPUT TO CLIENT
                // Some remote type payment systems allow the callback
                // to return HTML to the payment system's server;
                // we have a setting to return 'full display', which is
                // a fully-formed Kartris page with the order result in
                // it. This works fine if returned directly to a user's
                // browser because the Callback.aspx is called directly
                // in a user's browser (e.g. SagePay VSP Form). But if
                // this full content is returned to a payment system
                // like RBSWorldPay or Realex that will relay it to the
                // client, it will result in a badly formatted page and
                // potentially SSL security warnings. This is because
                // some elements like SCRIPT tags will be stripped for
                // security reasons, local links to CSS files won't
                // find their targets, and images referenced with local
                // or http links will either be missing or trigger
                // browser security warnings.

                // The solution is to use a special HTML template to
                // format the response we send to the payment system.
                // This is optional; if you place the appropriate
                // template in the skin, Kartris should find and use
                // it.

                // The template should be put in a folder within your
                // skin called 'Templates'. It should be named as 
                // follows:

                // Callback(-[GatewayName].html)

                // The Gateway name should match the folder name of the
                // plugin exactly, for example:

                // Callback-Realex.html
                // Callback-RBSWorldPay.html

                // Kartris will find this template, replace the place-
                // holder [orderdetails] with the order details, and
                // serve back the HTML to the client. Pay attention to
                // links to external files, links to not https files
                // and banned tags (such as <script>) that payment gateways
                // might enforce. For more details, see the documentation
                // from the particular gateway to determine what is and is
                // not allowed in the HTML they relay to end users.

                // Tags that will be replaced:
                // [orderdetails] with full order text, same as in email
                // [siterooturl] with the site's root URL. This is used
                // by 2checkout or other gateways that relay the page
                // text on their own server to provide a return link or
                // autodirect.
                // -----------------------------------------------------

                // -----------------------------------------------------
                // CALLBACK TEMPLATE CHECKS
                // -----------------------------------------------------
                // Figure out where to look for this template
                string strPathToCallbackTemplate = "";
                strPathToCallbackTemplate = "~/Skins/" + CkartrisBLL.Skin(Session("LANG")) + "/Templates/Callback_" + strGatewayName + ".html";

                // Create variable to hold the HTML content of the template,
                // if we find one and can read it
                string strCallbackTemplateHTML = "";

                // Check if template exists
                var arrCallbackTemplateHTML = new string[3];
                if (File.Exists(Server.MapPath(strPathToCallbackTemplate)))
                {
                    strCallbackTemplateHTML = File.ReadAllText(Server.MapPath(strPathToCallbackTemplate));
                    if (!strCallbackTemplateHTML.Contains("[orderdetails]"))
                    {
                        // Template does not contain the [orderdetails] tag;
                        // in this case, set the template string to "" so we
                        // can treat as if no template.
                        strCallbackTemplateHTML = "";

                        // Log an error to explain to user the problem if
                        // the required [orderdetails] tag was not found
                        CkartrisFormatErrors.LogError("Callback template " + strPathToCallbackTemplate + " does not contain required [orderdetails] tag.");
                    }
                    else
                    {
                        // replace the [siterooturl] tag if used with site root URL
                        strCallbackTemplateHTML = Strings.Replace(strCallbackTemplateHTML, "[siterooturl]", CkartrisBLL.WebShopURL);

                        // Split the page template around the [orderdetails] tag and store in array
                        arrCallbackTemplateHTML = Strings.Split(strCallbackTemplateHTML, "[orderdetails]", -1);

                    }
                }


                // Show full Kartris page if gateway is set to use
                // full display (normal page output) and there is no
                // template in use for this gateway.
                if (blnFullDisplay & string.IsNullOrEmpty(strCallbackTemplateHTML))
                {
                    lblOrderResult.Text = GetLocalResourceObject("ContentText_TransactionSuccess");
                    if (GetKartConfig("general.email.enableHTML") == "y")
                    {
                        litOrderDetails.Text = ExtractHTMLBodyContents(strBodyText);
                    }
                    else
                    {
                        litOrderDetails.Text = Strings.Replace(strBodyText, Constants.vbCrLf, "<br/>");
                    }
                }
                else
                {
                    Response.Clear();
                    if (intCallbackStep == 1 | intCallbackStep == 3)
                    {
                        Response.Write(strResult);
                    }
                    else
                    {
                        // First part of HTML template
                        try
                        {
                            Response.Write(arrCallbackTemplateHTML[0]);
                        }
                        catch (Exception ex)
                        {
                            // No HTML template or [orderdetails] tag missing
                            // just ignore
                        }
                        Response.Write(GetLocalResourceObject("ContentText_TransactionSuccess"));
                        Response.Write("<br/><br/>");
                        // Response.Write(Replace(strBodyText, vbCrLf, "<br/>"))
                        Response.Write("<p><a href='" + WebShopURL() + "?strWipeBasket=yes'>" + GetGlobalResourceObject("Kartris", "ContentText_ReturnToHomepage") + "</a></p>");

                        // Closing part of HTML template
                        try
                        {
                            Response.Write(arrCallbackTemplateHTML[1]); // Closing part of HTML template
                        }
                        catch (Exception ex)
                        {
                            // No HTML template or [orderdetails] tag missing
                            // just ignore
                        }
                    }
                    Response.End();
                }
            }
            else
            {
                // Log the error
                CkartrisFormatErrors.LogError(strCallbackError);
                if (blnFullDisplay)
                {
                    lblOrderResult.Text = GetLocalResourceObject("ContentText_TransactionFailure");
                    litOrderDetails.Text = strResult;
                }
                else
                {
                    Response.Clear();
                    Response.Write(GetLocalResourceObject("ContentText_TransactionFailure"));
                    Response.Write("<br/><br/>");
                    Response.Write(strResult);
                    Response.Write("<p><a href='" + WebShopURL() + "?strWipeBasket=yes'>" + GetGlobalResourceObject("Kartris", "ContentText_ReturnToHomepage") + "</a></p>");
                    Response.End();
                }

            }

            clsPlugin = default;
        }
    }

    protected void RemoveQueryStringParams(string rname)
    {
        // reflect to readonly property
        var isReadOnly = typeof(System.Collections.Specialized.NameValueCollection).GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
        // make collection editable
        isReadOnly.SetValue(this.Request.QueryString, false, default);
        // remove
        this.Request.QueryString.Remove(rname);
        // make collection readonly again
        isReadOnly.SetValue(this.Request.QueryString, true, default);
    }


    protected void CreateQueryStringParams(string pname, string pvalue)
    {
        // reflect to readonly property
        var isReadOnly = typeof(System.Collections.Specialized.NameValueCollection).GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
        // make collection editable
        isReadOnly.SetValue(this.Request.QueryString, false, default);
        // modify
        this.Request.QueryString.Set(pname, pvalue);
        // make collection readonly again
        isReadOnly.SetValue(this.Request.QueryString, true, default);
    }

    public static void Log(string logMessage, TextWriter w)
    {

        w.Write(Constants.vbCrLf + "Log Entry : ");
        w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
        w.WriteLine("  :");
        w.WriteLine("  :{0}", logMessage);
        w.WriteLine("-------------------------------");
        w.Flush();
    }

}