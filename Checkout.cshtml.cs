using System;
using System.Collections.Generic;
using System.Linq;
using Braintree;
using CkartrisBLL;
using CkartrisDataManipulation;
using KartSettingsManager;
using MailChimp.Net.Models;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic
using PaymentsBLL;

/// <summary>
/// Checkout - this page handles users checking out,
/// the subsequent confirmation page, and then
/// formats the form which is posted to payment
/// gateways to initiate a payment.
/// </summary>
internal partial class _Checkout : PageBaseClass
{
    private string _SelectedPaymentMethod = "";
    private bool _blnAnonymousCheckout = false;

    public _Checkout()
    {
        this.Load += Page_Load;
        this.LoadComplete += Page_LoadComplete;
    }

    /// <summary>
    /// Page Load
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {

            // ---------------------------------------
            // SET PAGE TITLE
            // ---------------------------------------
            Page.Title = GetLocalResourceObject("PageTitle_CheckOut") + " | " + Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));

            // ---------------------------------------
            // GET LIST OF PAYMENT GATEWAYS
            // This comes from a config setting that
            // is populated dynamically from the
            // available payment gateways that are
            // turned on in the 'plugins' folder.
            // ---------------------------------------
            string strPaymentMethods = GetKartConfig("frontend.payment.gatewayslist");

            // ---------------------------------------
            // CHECK IF USER IS LOGGED IN
            // ---------------------------------------
            bool blnAuthorized = false;
            if (CurrentLoggedUser is not null)
            {
                blnAuthorized = CurrentLoggedUser.isAuthorized;
            }

            // ---------------------------------------
            // SHOW CUSTOMER COMMENTS BOX?
            // ---------------------------------------
            if (GetKartConfig("frontend.checkout.comments.enabled") == "n")
            {
                phdCustomerComments.Visible = false;
            }

            // =======================================
            // PAYMENT GATEWAYS
            // Paypal, SagePay, etc.
            // =======================================
            var arrPaymentsMethods = Strings.Split(strPaymentMethods, ",");

            // If the order value inclusive of everything is zero, we don't want
            // to bill the customer. Instead we activate the PO method, even if
            // this user is not authorized to use it. We hide the other payment
            // methods.
            Kartris.Basket objBasket = Session("Basket");
            bool blnOrderIsFree = false; // Disable, suspect this might misfire (objBasket.FinalPriceIncTax = 0)

            // This line below looks a bit more complicated than it should. We have seen
            // some cases where orders slip by without payment, when they should not. It doesn't seem
            // to be possible, but apparently has happened in some cases. The code below is an idea to try
            // to stop this, the assumption that if the finalprice shows as zero because of some glitch,
            // maybe the first item in the basket would have a zero name too. Or that maybe it will trigger
            // an error. Only time will tell. If this causes problems, comment it out and just stop accepting
            // free orders (most sites don't do this, but some use it to give promotions away).
            try
            {
                blnOrderIsFree = objBasket.FinalPriceIncTax == 0 & objBasket.BasketItems.Item(0).Name != "";
            }
            catch (Exception ex)
            {
                // order stays as not free
            }

            if (blnOrderIsFree)
            {
                // Add the PO option with name 'FREE' and hide payment selection
                // The 'False' flag indicates this is not for authorized users
                // only. PO normally is, but here we are using this for all users
                // if total price is zero.
                ddlPaymentGateways.Items.Add(new ListItem("FREE", "po_offlinepayment" + "::False"));
                updPaymentMethods.Visible = false;
                valPaymentGateways.Enabled = false;
            }
            else
            {
                // Order isn't free. Load up the payment gateways.
                try
                {
                    // Add the default 'Please select' line at top of menu
                    ddlPaymentGateways.Items.Add(new ListItem(GetGlobalResourceObject("Kartris", "ContentText_DropdownSelectDefault"), "::False"));

                    // ---------------------------------------
                    // LOOP THROUGH PAYMENT METHODS
                    // ---------------------------------------
                    foreach (string strGatewayEntry in arrPaymentsMethods)
                    {

                        // The config setting stores info for each gateway,
                        // separated by double colons (::)
                        var arrGateway = Strings.Split(strGatewayEntry, "::");

                        // ---------------------------------------
                        // CHECK PAYMENT GATEWAY DATA VALID
                        // We shoud have 5 bits of data (in a zero
                        // based array).
                        // ---------------------------------------
                        if (Information.UBound(arrGateway) == 4)
                        {
                            bool blnOkToAdd = true;

                            // Is this a payment gateway? (value='p')
                            if (arrGateway[4] == "p")
                            {

                                // Is this only available for 'authorized'
                                // customers? For offline (PO) orders in
                                // particular, you probably only want 
                                // trusted customers to be able to use it.
                                // To do this, set the gateway's settings
                                // to 'Authorized Only' and then edit any
                                // customer you want to be able to use this
                                // payment system to 'Authorize' them.
                                if (Strings.LCase(arrGateway[2]) == "true")
                                {
                                    blnOkToAdd = blnAuthorized;
                                }

                                // ---------------------------------------
                                // CHECK STATUS OF GATEWAY
                                // There are four possibilities
                                // On, Off, Test, Fake
                                // The last two are for use when testing
                                // a payment system. 'Test' will pass an
                                // order through using the gateways test
                                // mode, if available. 'Fake' bypasses 
                                // the payment gateway completely, and
                                // just simulates the callback the gateway
                                // should make to your callback page.
                                // For more info, see the PDF User Guide.

                                // Test/Fake are only available if you are
                                // logged in as a back end admin. This
                                // means you can setup and test a new
                                // payment system on a live site without
                                // it being visible to customers.
                                // ---------------------------------------
                                if (Strings.LCase(arrGateway[1]) == "test" | Strings.LCase(arrGateway[1]) == "fake")
                                {
                                    blnOkToAdd = HttpSecureCookie.IsBackendAuthenticated;
                                }

                                // Gateway is turned off, don't add it
                                // to the list.
                                else if (Strings.LCase(arrGateway[1]) == "off")
                                {
                                    blnOkToAdd = false;
                                }
                            }
                            else
                            {

                                // Not a payment system... shipping plugins for 
                                // USPS, UPS, etc. are stored in the same
                                // plugins folder, but we don't want them
                                // available as a choice of payment system!
                                blnOkToAdd = false;
                            }

                            // This is a payment system and is available to 
                            // this customer
                            if (blnOkToAdd)
                            {
                                string strGatewayName = arrGateway[0];

                                // Get the 'friendly' name of the payment system from 
                                // the gateway's config. Note you can have friendly
                                // names for multiple languages in the config file:

                                // <setting name="FriendlyName(en-GB)" serializeAs="String">
                                // <value>Offline payment</value>
                                // </setting>
                                string strFriendlyName = Payment.GetPluginFriendlyName(strGatewayName);

                                // If no friendly name, use the Gateway's default name
                                // (Paypal, SagePay, etc.)
                                // Friendly name is better, because 'SagePay' probably means
                                // less to a customer than 'Pay with Credit Card'
                                if (Interfaces.Utils.TrimWhiteSpace(strFriendlyName) != "")
                                {
                                    ddlPaymentGateways.Items.Add(new ListItem(strFriendlyName, arrGateway[0].ToString() + "::" + arrGateway[3].ToString()));
                                }
                                else
                                {
                                    ddlPaymentGateways.Items.Add(new ListItem(strGatewayName, arrGateway[0].ToString() + "::" + arrGateway[3].ToString()));
                                }

                                if (strGatewayName.ToLower() == "po_offlinepayment")
                                {

                                    // Default name for PO (offline payment)
                                    strGatewayName = GetGlobalResourceObject("Checkout", "ContentText_Po");
                                }

                            }
                        }
                        else
                        {
                            // Didn't have the four values needed for payment
                            // gateway config
                            throw new Exception("Invalid gatewaylist config setting!");
                        }

                    }

                    // ---------------------------------------
                    // SHOW PAYMENT METHODS DROPDOWN
                    // Note that the count of gateways we get
                    // from the dropdown menu, but that has
                    // an extra line 'Please select', so the
                    // count will be 1 higher than the actual
                    // number of gateways. Hence '1' means no
                    // payment systems, '2' means there is one
                    // and so on.
                    // ---------------------------------------

                    // If there are no valid payment systems (Count = 1),
                    // we log an exception.
                    if (ddlPaymentGateways.Items.Count == 1)
                    {
                        throw new Exception("No valid payment gateways");
                    }

                    // If there is one (Count = 2) then we don't need to
                    // show the user a choice, since there is
                    // only one to choose from. So we default to
                    // that and hide the validators and dropdown.
                    else if (ddlPaymentGateways.Items.Count == 2)
                    {
                        var arrSelectedGateway = Strings.Split(ddlPaymentGateways.Items(1).Value, "::");
                        _SelectedPaymentMethod = arrSelectedGateway[0];
                        _blnAnonymousCheckout = Conversions.ToBoolean(arrSelectedGateway[1]);
                        ddlPaymentGateways.SelectedValue = ddlPaymentGateways.Items(1).Value;
                        phdPaymentMethods.Visible = false;
                        valPaymentGateways.Enabled = false; // disable validation just to be sure
                        if (_SelectedPaymentMethod == "PO_OfflinePayment")
                            phdPONumber.Visible = true;
                        else
                            phdPONumber.Visible = false;

                        // Store value in hidden field. We hope this will be more
                        // robust if page times out
                        litPaymentGatewayHidden.Text = _SelectedPaymentMethod;
                    }
                    else
                    {
                        // More than one payment method available,
                        // show dropdown and give user the choice.
                        // Hide the PO number field.
                        phdPaymentMethods.Visible = true;
                        // txtPurchaseOrderNo.Style.Item("display") = "none"
                        // phdPONumber.Style.Item("display") = "none"
                        phdPONumber.Visible = false;
                    }
                }

                // ---------------------------------------
                // ERROR LOADING PAYMENT GATEWAYS LIST
                // ---------------------------------------
                catch (Exception ex)
                {
                    throw new Exception("Error loading payment gateway list");
                }
            }


            // ---------------------------------------
            // CLEAR ADDRESS CONTROLS
            // ---------------------------------------
            UC_BillingAddress.Clear();
            UC_ShippingAddress.Clear();

            // ---------------------------------------
            // CUSTOMER OPTION TO SELECT
            // EMAIL UPDATES OF ORDER STATUS?
            // ---------------------------------------
            if (GetKartConfig("frontend.checkout.ordertracking") != "n" & GetKartConfig("backend.orders.emailupdates") != "n")
            {
                phdOrderEmails.Visible = true;
            }
            else
            {
                phdOrderEmails.Visible = false;
                chkOrderEmails.Checked = false;
            }

            // ---------------------------------------
            // SHOW MAILING LIST OPT-IN BOX?
            // ---------------------------------------
            if (GetKartConfig("frontend.users.mailinglist.enabled") != "n")
            {
                phdMailingList.Visible = true;
                chkMailingList.Checked = false;
            }
            else
            {
                phdMailingList.Visible = false;
                chkMailingList.Checked = false;
            }

            // ---------------------------------------
            // SHOW SAVE-BASKET OPTION?
            // Customers can save the basket if they
            // want to make the same order again in
            // future
            // ---------------------------------------
            if (GetKartConfig("frontend.checkout.savebasket") != "n")
                phdSaveBasket.Visible = true;
            else
                phdSaveBasket.Visible = false;

            // ---------------------------------------
            // SHOW Ts & Cs CHECKBOX CONFIRMATION?
            // ---------------------------------------
            if (GetKartConfig("frontend.checkout.termsandconditions") != "n")
                phdTermsAndConditions.Visible = true;
            else
                phdTermsAndConditions.Visible = false;
            ConfigureAddressFields();
        }
        else
        {



        }

        // =======================================
        // SHOW LOGIN BOX
        // If the user is not logged in, or has
        // not proceeded through first steps of
        // creating new user, then we show the
        // login / new user options.
        // The 'proceed' button is hidden.
        // =======================================
        if (!(UC_KartrisLogin.Cleared | User.Identity.IsAuthenticated))
        {

            // Show login box
            mvwCheckout.ActiveViewIndex = 0;
            btnProceed.Visible = false;
        }
        else
        {

            // Show checkout form if not already set to
            // go to confirmation page
            if (mvwCheckout.ActiveViewIndex != 2)
                mvwCheckout.ActiveViewIndex = 1;
            else
                valSummary.Enabled = false;
            btnProceed.Visible = true;
        }

        // =======================================
        // SETUP CHECKOUT FORM
        // Runs if user is logged in already
        // =======================================
        if (!(CurrentLoggedUser == null))
        {

            // Show checkout form if not already set to
            // go to confirmation page
            if (mvwCheckout.ActiveViewIndex != 2)
                mvwCheckout.ActiveViewIndex = 1;
            else
                valSummary.Enabled = false;

            // Fresh form arrival
            if (!Page.IsPostBack)
            {

                // Set up first user address (billing)
                List<KartrisClasses.Address> lstUsrAddresses = null;

                // ---------------------------------------
                // BILLING ADDRESS
                // ---------------------------------------
                if (UC_BillingAddress.Addresses is null)
                {

                    // Find all addresses in this user's account
                    lstUsrAddresses = KartrisClasses.Address.GetAll(CurrentLoggedUser.ID);

                    // Populate dropdown by filtering billing/universal addresses
                    UC_BillingAddress.Addresses = lstUsrAddresses.FindAll(p => p.Type == "b" | p.Type == "u");
                }

                // ---------------------------------------
                // SHIPPING ADDRESS
                // ---------------------------------------
                if (UC_ShippingAddress.Addresses is null)
                {

                    // Find all addresses in this user's account
                    if (lstUsrAddresses is null)
                        lstUsrAddresses = KartrisClasses.Address.GetAll(CurrentLoggedUser.ID);

                    // Populate dropdown by filtering shipping/universal addresses
                    UC_ShippingAddress.Addresses = lstUsrAddresses.FindAll(ShippingAdd => ShippingAdd.Type == "s" | ShippingAdd.Type == "u");
                }

                // ---------------------------------------
                // SHIPPING/BILLING ADDRESS NOT SAME
                // ---------------------------------------

                if (!(CurrentLoggedUser.DefaultBillingAddressID == CurrentLoggedUser.DefaultShippingAddressID))
                {
                    if (!_blnAnonymousCheckout)
                    {
                        chkSameShippingAsBilling.Checked = false;
                    }
                    else
                    {
                        chkSameShippingAsBilling.Checked = true;
                    }
                }
                else
                {
                    chkSameShippingAsBilling.Checked = true;
                }

                if (UC_BasketSummary.GetBasket.AllDigital)
                {
                    pnlShippingAddress.Visible = false;
                    UC_ShippingAddress.Visible = false;
                }
                else if (!chkSameShippingAsBilling.Checked)
                {
                    // Show shipping address block
                    pnlShippingAddress.Visible = true;
                    UC_ShippingAddress.Visible = true;
                }
                else
                {
                    pnlShippingAddress.Visible = false;
                    UC_ShippingAddress.Visible = false;
                }

                // ---------------------------------------
                // SELECT DEFAULT ADDRESSES
                // ---------------------------------------
                if (UC_BillingAddress.SelectedID == 0)
                {
                    UC_BillingAddress.SelectedID = CurrentLoggedUser.DefaultBillingAddressID;
                }
                if (UC_ShippingAddress.SelectedID == 0)
                {
                    UC_ShippingAddress.SelectedID = CurrentLoggedUser.DefaultShippingAddressID;
                }
            }
        }

    }

    /// <summary>
    /// Page Load Complete
    /// </summary>
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        // Fails for new users
        if (!this.IsPostBack)
        {
            try
            {
                var objUsersBLL = new UsersBLL();
                if (txtEUVAT.Text == "" & phdEUVAT.Visible == true)
                {
                    txtEUVAT.Text = objUsersBLL.GetCustomerEUVATNumber(CurrentLoggedUser.ID);
                }
            }
            catch (Exception ex)
            {
                // probably a new user or don't need vate
            }
        }

        // ---------------------------------------
        // ZERO ITEMS IN BASKET!
        // ---------------------------------------
        if (UC_BasketView.GetBasketItems.Count == 0)
            Response.Redirect("~/Basket.aspx");

        // ---------------------------------------
        // CHECK MINIMUM ORDER VALUE MET
        // ---------------------------------------
        double numMinOrderValue = GetKartConfig("frontend.orders.minordervalue");

        if (numMinOrderValue > 0d)
        {

            if (GetKartConfig("general.tax.pricesinctax") == "y")
            {

                // Prices include tax
                if (UC_BasketView.GetBasket.TotalIncTax < CurrenciesBLL.ConvertCurrency(Session("CUR_ID"), numMinOrderValue))
                {
                    Response.Redirect("~/Basket.aspx?error=minimum");
                }
            }
            else if (UC_BasketView.GetBasket.TotalExTax < CurrenciesBLL.ConvertCurrency(Session("CUR_ID"), numMinOrderValue))
            {
                Response.Redirect("~/Basket.aspx?error=minimum");
            }
        }

        // Just to be sure we get shipping price, have
        // had issues where sometimes a single shipping method
        // doesn't trigger lookup for shipping price
        // UC_BasketView.RefreshShippingMethods()
        // If Not Me.IsPostBack() Then
        // txtEUVAT_AutoPostback()
        // End If

    }
    /// <summary>
/// Payment Method Changed, refresh
/// </summary>
protected void ddlPaymentGateways_SelectedIndexChanged(object sender, EventArgs e)
{
    var arrSelectedGateway = Strings.Split(ddlPaymentGateways.SelectedItem.Value, "::");
    _SelectedPaymentMethod = arrSelectedGateway[0];
    _blnAnonymousCheckout = Conversions.ToBoolean(arrSelectedGateway[1]);
    ConfigureAddressFields(true);

    // Store value in hidden field. We hope this will be more
    // robust if page times out
    litPaymentGatewayHidden.Text = _SelectedPaymentMethod;

    // Decide whether to show PO field
    if (_SelectedPaymentMethod == "PO_OfflinePayment")
    {
        phdPONumber.Visible = true;
    }
    else
    {
        phdPONumber.Visible = false;
        txtPurchaseOrderNo.Text = "";
        litPONumberText.Text = "";
    }

}
/// <summary>
    /// Show or Hide Address Fields depending on the selected payment gateway and basket contents
    /// </summary>
    /// <remarks></remarks>
    private void ConfigureAddressFields(bool blnUpdateAddressPanel = false)
    {
        if (_blnAnonymousCheckout)
        {
            chkSameShippingAsBilling.Visible = false;
            chkSameShippingAsBilling.Checked = false;
            lblchkSameShipping.Visible = false;
            UC_BillingAddress.Visible = false;
        }
        else
        {
            chkSameShippingAsBilling.Visible = true;
            // chkSameShippingAsBilling.Checked = True
            lblchkSameShipping.Visible = true;
            UC_BillingAddress.Visible = true;
        }

        if (UC_BasketSummary.GetBasket.AllDigital)
        {
            pnlShippingAddress.Visible = false;
            UC_ShippingAddress.Visible = false;
            chkSameShippingAsBilling.Visible = false;
            lblchkSameShipping.Visible = false;
        }
        else if (!_blnAnonymousCheckout)
        {
            chkSameShippingAsBilling.Visible = true;
            lblchkSameShipping.Visible = true;
            if (chkSameShippingAsBilling.Checked)
            {
                pnlShippingAddress.Visible = false;
                UC_ShippingAddress.Visible = false;
            }
            else
            {
                pnlShippingAddress.Visible = true;
                UC_ShippingAddress.Visible = true;
            }
        }
        else
        {
            pnlShippingAddress.Visible = true;
            UC_ShippingAddress.Visible = true;

        }
        if (blnUpdateAddressPanel)
            updAddresses.Update();
    }

    /// <summary>
    /// Billing country changed, refresh
    /// shipping methods
    /// </summary>
    protected void BillingCountryUpdated(object sender, EventArgs e)
    {
        if (chkSameShippingAsBilling.Checked)
        {
            pnlShippingAddress.Visible = false;
            UC_ShippingAddress.Visible = false;
            RefreshShippingMethods("billing");
        }
        else
        {
            pnlShippingAddress.Visible = true;
            UC_ShippingAddress.Visible = true;
            RefreshShippingMethods("shipping");
        }
        updAddresses.Update();
    }

 /// <summary>
  /// Shipping country updated, refresh
  /// shipping methods
  /// </summary>
    protected void ShippingCountryUpdated(object sender, EventArgs e)
    {
        if (chkSameShippingAsBilling.Checked)
        {
            pnlShippingAddress.Visible = false;
            UC_ShippingAddress.Visible = false;
            RefreshShippingMethods("billing");
        }
        else
        {
            pnlShippingAddress.Visible = true;
            UC_ShippingAddress.Visible = true;
            RefreshShippingMethods("shipping");
        }
        updAddresses.Update();
    }

    /// <summary>
    /// The checkbox for 'same shipping
    /// as billing address' has been 
    /// changed (checked/unchecked))
    /// </summary>
    protected void chkSameShippingAsBilling_CheckedChanged(object sender, EventArgs e)
    {
        if (chkSameShippingAsBilling.Checked)
        {
            pnlShippingAddress.Visible = false;
            UC_ShippingAddress.Visible = false;
            RefreshShippingMethods("billing");
        }
        else
        {
            pnlShippingAddress.Visible = true;
            UC_ShippingAddress.Visible = true;
            RefreshShippingMethods("shipping");
        }
        updAddresses.Update();
    }

    /// <summary>
    /// Refresh shipping methods
    /// </summary>
    private void RefreshShippingMethods(string strControl = "shipping")
    {
        int numShippingDestinationID;
        if (strControl == "billing")
        {
            if (UC_BillingAddress.SelectedAddress is not null)
            {
                numShippingDestinationID = UC_BillingAddress.SelectedAddress.CountryID;
            }
            else
            {
                numShippingDestinationID = 0;
            }
        }
        else if (UC_ShippingAddress.SelectedAddress is not null)
        {
            numShippingDestinationID = UC_ShippingAddress.SelectedAddress.CountryID;
        }
        else
        {
            numShippingDestinationID = 0;
        }

        // =======================================
        // EU VAT and EORI fields
        // New brexit-related changes

        // =======================================
        KartrisClasses.Address adrShipping = default;
        if (chkSameShippingAsBilling.Checked)
        {
            // Shipping address same as billing
            // so use billing address (as shipping
            // address)
            if (UC_BillingAddress.SelectedID > 0)
            {
                adrShipping = UC_BillingAddress.Addresses.Find(Add => Operators.ConditionalCompareObjectEqual(Add.ID, UC_BillingAddress.SelectedID, false));
            }
            else if (numShippingDestinationID > 0)
            {
                adrShipping = UC_BillingAddress.SelectedAddress;
            }
        }
        // Must use shipping address
        else if (UC_ShippingAddress.SelectedID > 0)
        {
            adrShipping = UC_ShippingAddress.Addresses.Find(Add => Operators.ConditionalCompareObjectEqual(Add.ID, UC_ShippingAddress.SelectedID, false));
        }
        else if (numShippingDestinationID > 0)
        {
            adrShipping = UC_ShippingAddress.SelectedAddress;
        }

        // Two new functions determine whether to show EU VAT and EORI fields at checkout
        try
        {
            phdEUVAT.Visible = CkartrisRegionalSettings.ShowEUVATField(UCase(GetKartConfig("general.tax.euvatcountry")), adrShipping.Country.IsoCode, adrShipping.Country.D_Tax, adrShipping.Country.TaxExtra, GetKartConfig("general.tax.domesticshowfield") == "y");
        }
        catch (Exception ex)
        {
            phdEUVAT.Visible = false;
        }
        try
        {
            phdEORI.Visible = CkartrisRegionalSettings.ShowEORIField(adrShipping.Country.D_Tax, adrShipping.Country.TaxExtra);
        }
        catch (Exception ex)
        {
            phdEORI.Visible = false;
        }


        if (phdEUVAT.Visible)
        {
            // Show VAT country ISO code in front of field
            litMSCode.Text = adrShipping.Country.IsoCode;

            // EU uses 'EL' for Greece, and not the 
            // ISO code 'GR', so we need to adjust for this
            if (litMSCode.Text == "GR")
                litMSCode.Text = "EL";

            // Try to fill EU number
            try
            {
                var objUsersBLL = new UsersBLL();
                txtEUVAT.Text = objUsersBLL.GetCustomerEUVATNumber(CurrentLoggedUser.ID);
            }
            catch (Exception ex)
            {

            }
        }
        else
        {
            // Country of user is same as store
            // Hide VAT box
            txtEUVAT.Text = "";
            txtEUVAT_AutoPostback();
        }

        if (phdEORI.Visible)
        {
            // Try to fill EU number
            var objObjectConfigBLL = new ObjectConfigBLL();
            try
            {
                txtEORI.Text = objObjectConfigBLL.GetValue("K:user.eori", CurrentLoggedUser.ID);
            }
            catch (Exception ex)
            {

            }
        }

        // =======================================================
        // SET SHIPPING DETAILS FROM ADDRESS CONTROL
        // =======================================================
        var objShippingDetails = new Interfaces.objShippingDetails();
        try
        {
            {
                var withBlock = objShippingDetails.RecipientsAddress;
                if (chkSameShippingAsBilling.Checked)
                {
                    withBlock.Postcode = UC_BillingAddress.SelectedAddress.Postcode;
                    withBlock.CountryID = UC_BillingAddress.SelectedAddress.Country.CountryId;
                    withBlock.CountryIsoCode = UC_BillingAddress.SelectedAddress.Country.IsoCode;
                    withBlock.CountryName = UC_BillingAddress.SelectedAddress.Country.Name;
                }
                else
                {
                    withBlock.Postcode = UC_ShippingAddress.SelectedAddress.Postcode;
                    withBlock.CountryID = UC_ShippingAddress.SelectedAddress.Country.CountryId;
                    withBlock.CountryIsoCode = UC_ShippingAddress.SelectedAddress.Country.IsoCode;
                    withBlock.CountryName = UC_ShippingAddress.SelectedAddress.Country.Name;
                }
            }
        }
        catch (Exception ex)
        {

        }

        // =======================================
        // UPDATE BASKET WITH SHIPPING DETAILS
        // =======================================
        UC_BasketView.ShippingDetails = objShippingDetails;
        UC_BasketSummary.ShippingDetails = objShippingDetails;

        UC_BasketView.ShippingDestinationID = numShippingDestinationID;
        UC_BasketSummary.ShippingDestinationID = numShippingDestinationID;

        UC_BasketView.RefreshShippingMethods();

    }

    /// <summary>
    /// Proceed button clicked, moves us
    /// to next stage
    /// </summary>
    protected void btnProceed_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_SelectedPaymentMethod))
        {
            var arrSelectedGateway = Strings.Split(ddlPaymentGateways.SelectedItem.Value, "::");
            _SelectedPaymentMethod = arrSelectedGateway[0];
            _blnAnonymousCheckout = Conversions.ToBoolean(arrSelectedGateway[1]);
        }

        Kartris.Interfaces.PaymentGateway clsPlugin = default;
        string strGatewayName = _SelectedPaymentMethod;

        if (mvwCheckout.ActiveViewIndex > "0")
        {
            clsPlugin = Payment.PPLoader(strGatewayName);
            if (LCase(clsPlugin.Status) == "test" | LCase(clsPlugin.Status) == "fake")
                litFakeOrTest.Visible = true;
            else
                litFakeOrTest.Visible = false;
        }

        bool blnBasketAllDigital = UC_BasketSummary.GetBasket.AllDigital;

        if (mvwCheckout.ActiveViewIndex == "1")
        {
            // =======================================
            // ADDRESS DETAILS ENTERED,
            // SHIPPING METHODS SELECTED
            // COMMENTS ADDED
            // =======================================
            litOtherErrors.Text = "";

            // Handle EU VAT number validation
            bool blnValidEUVAT;
            if (txtEUVAT.Visible & !string.IsNullOrEmpty(txtEUVAT.Text))
            {
                txtEUVAT_AutoPostback();
                blnValidEUVAT = Session("blnEUVATValidated");
                if (!blnValidEUVAT)
                    litOtherErrors.Text = GetGlobalResourceObject("Kartris", "ContentText_VATNumberInvalid");
            }
            else
            {
                blnValidEUVAT = true;
            }

            // Validation - no shipping selected
            if (!UC_BasketView.GetBasket.AllFreeShipping & UC_BasketView.SelectedShippingID == 0)
            {
                litOtherErrors.Text += GetGlobalResourceObject("Checkout", "ContentText_NoShippingSelected") + "<br />";
            }

            if (!_blnAnonymousCheckout)
            {
                // Validation - no billing address
                if (UC_BillingAddress.SelectedAddress is null)
                {
                    litOtherErrors.Text += GetGlobalResourceObject("Checkout", "ContentText_NoBillingAddress") + "<br />";
                    blnValidEUVAT = false;
                }
            }

            if (!blnBasketAllDigital)
            {
                // Validation - no shipping address
                if (!chkSameShippingAsBilling.Checked)
                {
                    if (UC_ShippingAddress.SelectedAddress is null)
                    {
                        litOtherErrors.Text += GetGlobalResourceObject("Checkout", "ContentText_NoShippingAddress") + "<br />";
                        blnValidEUVAT = false;
                    }
                }
            }

            // Validation - Ts and Cs agreement not checked
            if (GetKartConfig("frontend.checkout.termsandconditions") != "n")
            {
                if (!chkTermsAndConditions.Checked)
                {
                    litOtherErrors.Text += GetGlobalResourceObject("Checkout", "ContentText_ErrorTermsNotChecked") + "<br />";
                    blnValidEUVAT = false;
                }
            }

            // =======================================
            // PAGE IS VALID
            // Set up confirmation page
            // =======================================
            if (Page.IsValid & (UC_BasketView.SelectedShippingID != 0 | UC_BasketView.GetBasket.AllFreeShipping) & blnValidEUVAT)
            {

                bool isOk = true;

                // Set billing address to one selected by user
                // Set shipping address to this, or to selected shipping one, depending on same-shipping checkbox
                if (!blnBasketAllDigital)
                {
                    if (!_blnAnonymousCheckout && UC_BillingAddress.SelectedAddress is not null)
                    {
                        if (chkSameShippingAsBilling.Checked)
                        {
                            UC_Shipping.Address = UC_BillingAddress.SelectedAddress;
                        }
                        else
                        {
                            UC_Shipping.Address = UC_ShippingAddress.SelectedAddress;
                        }
                    }
                    else
                    {
                        UC_Shipping.Address = UC_ShippingAddress.SelectedAddress;
                    }
                }
                else if (!_blnAnonymousCheckout)
                {
                    // For downloable orders, we don't really need shipping, but
                    // payment systems require it. Therefore, we set it to same
                    // as billing and also check the box (which is hidden) to say
                    // we're using same shipping address as for billing
                    UC_Shipping.Address = UC_BillingAddress.SelectedAddress;
                    chkSameShippingAsBilling.Checked = true;
                }

                if (_blnAnonymousCheckout)
                {
                    UC_Billing.Visible = false;
                    litBillingDetails.Visible = false;
                }
                else
                {
                    UC_Billing.Address = UC_BillingAddress.SelectedAddress;
                    UC_Billing.Visible = true;
                    litBillingDetails.Visible = true;
                }

                // Hide shipping address from being visible if all items in order
                // are downloadable
                if (blnBasketAllDigital)
                {
                    UC_Shipping.Visible = false;
                    litShippingDetails.Visible = false;
                }
                else
                {
                    UC_Shipping.Visible = true;
                    litShippingDetails.Visible = true;
                }


                // Show payment method on confirmation page
                if (_SelectedPaymentMethod.ToLower == "po_offlinepayment")
                {
                    // Change 'po_offlinepayment' to language string for this payment type
                    litPaymentMethod.Text = Server.HtmlEncode(GetGlobalResourceObject("Checkout", "ContentText_Po"));
                    litPONumberText.Text = GetGlobalResourceObject("Invoice", "ContentText_PONumber") + ": <strong>" + txtPurchaseOrderNo.Text + "</strong><br/>";
                }
                else
                {
                    // try to get the friendly name - use the payment gateway name if its blank
                    string strFriendlyName = Payment.GetPluginFriendlyName(strGatewayName);
                    if (Interfaces.Utils.TrimWhiteSpace(strFriendlyName) != "")
                    {
                        litPaymentMethod.Text = strFriendlyName;
                    }
                    else
                    {
                        litPaymentMethod.Text = _SelectedPaymentMethod;
                    }
                }

                // Show PO number
                if (txtPurchaseOrderNo.Text == "")
                {
                    litPONumberText.Text = "";
                }

                // Show VAT number
                if (txtEUVAT.Text != "")
                {
                    litVATNumberText.Text = GetGlobalResourceObject("Invoice", "FormLabel_CardholderEUVatNum") + ": <strong>" + txtEUVAT.Text + "</strong><br/>";
                }

                // Show VAT number
                if (txtEORI.Text != "")
                {
                    litEORINumberText.Text = "EORI: <strong>" + txtEORI.Text + "</strong><br/>";
                }

                // Show whether mailing list opted into
                if (!chkMailingList.Checked)
                    litMailingListYes.Visible = false;
                else
                    litMailingListYes.Visible = true;

                // Show whether 'receive order updates' was checked
                if (!chkOrderEmails.Checked)
                    litOrderEmailsYes2.Visible = false;
                else
                    litOrderEmailsYes2.Visible = true;

                // Show whether 'save basket' was checked
                if (!chkSaveBasket.Checked)
                    litSaveBasketYes.Visible = false;
                else
                    litSaveBasketYes.Visible = true;

                // Show comments (HTMLencoded for XSS protection)
                if (!string.IsNullOrEmpty(Strings.Trim(txtComments.Text)))
                {
                    lblComments.Text = Server.HtmlEncode(CkartrisDisplayFunctions.StripHTML(txtComments.Text));
                    pnlComments.Visible = true;
                }
                else
                {
                    pnlComments.Visible = false;
                }

                // Set various variables for use later
                int CUR_ID = Session("CUR_ID");
                Kartris.Basket objBasket = Session("Basket");

                // Some items might be excluded from customer discount
                if (objBasket.HasCustomerDiscountExemption)
                {

                }
                short intGatewayCurrency;

                // Set payment gateway
                if (Interfaces.Utils.TrimWhiteSpace(clsPlugin.Currency) != "")
                {
                    intGatewayCurrency = CurrenciesBLL.CurrencyID(clsPlugin.Currency);
                }
                else
                {
                    intGatewayCurrency = (short)CUR_ID;
                }

                // If payment system can only process a particular currency
                // we show a message that the order was converted from user
                // selected currency to this for processing
                if (intGatewayCurrency != CUR_ID)
                {
                    pnlProcessCurrency.Visible = true;
                    lblProcessCurrency.Text = CurrenciesBLL.FormatCurrencyPrice(intGatewayCurrency, CurrenciesBLL.ConvertCurrency(intGatewayCurrency, objBasket.FinalPriceIncTax, CUR_ID), default, false);
                }
                else
                {
                    pnlProcessCurrency.Visible = false;
                }

                // Back button, in case customers need to change anything
                btnBack.Visible = true;

                // Show Credit Card Input Usercontrol if payment gateway type is local
                if (LCase(clsPlugin.GatewayType) == "local" & !(clsPlugin.GatewayName.ToLower == "po_offlinepayment" | clsPlugin.GatewayName.ToLower == "bitcoin" | clsPlugin.GatewayName.ToLower == "easypaymultibanco" | clsPlugin.GatewayName.ToLower == "braintreepayment"))
                {
                    UC_CreditCardInput.AcceptsPaypal = clsPlugin.AcceptsPaypal;
                    phdCreditCardInput.Visible = true;
                }
                else if (clsPlugin.GatewayName.ToLower == "braintreepayment")  // show BrainTree input form
                {
                    string clientToken = "";
                    try
                    {
                        clientToken = PaymentsBLL.GenerateClientToken();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            clientToken = "";
                            throw new BrainTreeException("Something went wrong, please contact an Administrator.", "");
                        }
                        catch (BrainTreeException bEx)
                        {
                            UC_PopUpErrors.SetTextMessage = bEx.CustomMessage is not null ? bEx.CustomMessage : bEx.Message;
                            UC_PopUpErrors.SetTitle = GetGlobalResourceObject("Kartris", "ContentText_CorrectErrors");
                            UC_PopUpErrors.ShowPopup();
                            mvwCheckout.ActiveViewIndex = 1;
                            isOk = false;
                        }
                    }

                    // Mailchimp library
                    var mailChimpLib = new MailChimpBLL(CurrentLoggedUser, objBasket, CurrenciesBLL.CurrencyCode(Session("CUR_ID")));

                    // Mailchimp
                    bool blnMailChimp = KartSettingsManager.GetKartConfig("general.mailchimp.enabled") == "y";

                    if (!clientToken.Equals(""))
                    {
                        // MAILCHIMP Adding Cart to BrainTree Payments
                        // If the User is Logged
                        if (blnMailChimp)
                        {
                            if (CurrentLoggedUser is not null)
                            {
                                Session("BraintreeCartId") = mailChimpLib.AddCartToCustomerToStore().Result;
                            }
                        }

                        phdBrainTree.Visible = true;
                        phdCreditCardInput.Visible = false;
                        phdPONumber.Visible = false;
                        txtPurchaseOrderNo.Text = "";
                        litPONumberText.Text = "";
                        tbClientToken.Value = clientToken;
                        tbAmount.Value = objBasket.FinalPriceIncTax;
                        ScriptManager.RegisterStartupScript(this, typeof(Page), "BrainTreeSelected", "var clientToken = $(\"input[name*='tbClientToken']\")[0]; $(clientToken).on('input', function () {clientTokenChanged();});$(clientToken).trigger(\"input\");", true);
                    }
                }
                else
                {
                    phdCreditCardInput.Visible = false;
                }

                if (isOk)
                {
                    // Move to next step
                    mvwCheckout.ActiveViewIndex = "2";
                    btnProceed.OnClientClick = "";
                }
            }
            else
            {
                // Show any errors not handled by
                // client side validation
                popExtender.Show();
            }
        }

        else if (mvwCheckout.ActiveViewIndex == "2")
        {

            // =======================================
            // PROCEED CLICKED ON ORDER REVIEW PAGE
            // =======================================
            bool blnValid = false;
            int numSelectedShippingID = 0;
            try
            {
                numSelectedShippingID = UC_BasketView.SelectedShippingID;
            }
            catch (Exception ex)
            {

            }

            // If numSelectedShippingID = 0 Then Response.Redirect("Checkout.aspx?shippingzero")

            // This causes issues with shipping selection, drop it for now
            // Load the basket again to verify contents. Check if quantities are still valid
            // UC_BasketSummary.LoadBasket()
            // Dim objValidateBasket As Kartris.Basket = UC_BasketSummary.GetBasket
            // If objValidateBasket.AdjustedQuantities Then
            // UC_BasketView.LoadBasket()
            // mvwCheckout.ActiveViewIndex = "1"
            // Exit Sub
            // End If
            // objValidateBasket = Nothing

            // For local payment gateway types, credit
            // card details were entered. Validate these.
            if (phdCreditCardInput.Visible)
            {
                // Validate Credit Card Input here
                Page.Validate("CreditCard");
                if (IsValid)
                    blnValid = true;
            }

            // Handle local payment scenarios
            // This could be a local type payment gateway
            // where card data is entered directly into
            // Kartris. Or it could be the PO (purchase
            // order) / offline payment method, where a
            // user can checkout without giving card
            // details and will pay offline.
            if (LCase(clsPlugin.GatewayType) != "local" | blnValid | clsPlugin.GatewayName.ToLower == "po_offlinepayment" | clsPlugin.GatewayName.ToLower == "bitcoin" | clsPlugin.GatewayName.ToLower == "easypaymultibanco" | clsPlugin.GatewayName.ToLower == "braintreepayment")
            {

                // Setup variables to use later
                int C_ID = 0;
                int O_ID;
                int CUR_ID = Session("CUR_ID");

                bool blnUseHTMLOrderEmail = GetKartConfig("general.email.enableHTML") == "y";
                var sbdHTMLOrderEmail = new StringBuilder();
                var sbdHTMLOrderContents = new StringBuilder();
                var sbdHTMLOrderBasket = new StringBuilder();

                // Dim strBillingAddressText As String, strShippingAddressText As String
                string strSubject = "";
                string strTempEmailTextHolder = "";

                var sbdNewCustomerEmailText = new StringBuilder();
                var sbdBodyText = new StringBuilder();
                var sbdBasketItems = new StringBuilder();
                List<Kartris.BasketItem> arrBasketItems;

                Kartris.Basket objBasket = Session("Basket");

                Kartris.Interfaces.objOrder objOrder = default;

                bool blnNewUser = true;
                bool blnAppPricesIncTax;
                bool blnAppShowTaxDisplay;
                bool blnAppUSmultistatetax;
                if (ConfigurationManager.AppSettings("TaxRegime").ToLower == "us" | ConfigurationManager.AppSettings("TaxRegime").ToLower == "simple")
                {
                    blnAppPricesIncTax = false;
                    blnAppShowTaxDisplay = false;
                    blnAppUSmultistatetax = true;
                }
                else
                {
                    blnAppPricesIncTax = GetKartConfig("general.tax.pricesinctax") == "y";
                    blnAppShowTaxDisplay = GetKartConfig("frontend.display.showtax") == "y";
                    blnAppUSmultistatetax = false;
                }

                // Get the order confirmation template if HTML email is enabled
                if (blnUseHTMLOrderEmail)
                {
                    sbdHTMLOrderEmail.Append(RetrieveHTMLEmailTemplate("OrderConfirmation"));
                    // switch back to normal text email if the template can't be retrieved
                    if (sbdHTMLOrderEmail.Length < 1)
                        blnUseHTMLOrderEmail = false;
                }

                // Determine whether order will use the currency
                // specified in the payment gateway settings, or
                // the one the user has chosen on the store.
                short intGatewayCurrency;
                if (Interfaces.Utils.TrimWhiteSpace(clsPlugin.Currency) != "")
                {
                    intGatewayCurrency = CurrenciesBLL.CurrencyID(clsPlugin.Currency);
                }
                else
                {
                    intGatewayCurrency = (short)CUR_ID;
                }

                // Set a boolean value if currency of order
                // set in payment system is different to 
                // that the user was using. This way we know
                // if we need to convert the total to a
                // different currency for payment.
                bool blnDifferentGatewayCurrency = CUR_ID != intGatewayCurrency;

                // Determine if is existing user, if
                // so set Customer ID to the logged in
                // user
                if (User.Identity.IsAuthenticated)
                {
                    C_ID = CurrentLoggedUser.ID;
                    blnNewUser = false;
                }

                // Handle Promotion Coupons
                if (!string.IsNullOrEmpty(objBasket.CouponName) & objBasket.CouponDiscount.IncTax == 0)
                {
                    strTempEmailTextHolder = GetGlobalResourceObject("Email", "EmailText_OrderEmailBreaker") + Constants.vbCrLf + " " + GetGlobalResourceObject("Basket", "ContentText_ApplyCouponCode") + Constants.vbCrLf + " " + objBasket.CouponName + Constants.vbCrLf;
                    sbdBodyText.AppendLine(strTempEmailTextHolder);
                    if (blnUseHTMLOrderEmail)
                    {
                        sbdHTMLOrderContents.Append(GetBasketModifierHTMLEmailText(objBasket.CouponDiscount, GetGlobalResourceObject("Kartris", "ContentText_CouponDiscount"), objBasket.CouponName));
                    }
                }

                // Promotion discount
                string strPromotionDescription = "";
                if (objBasket.PromotionDiscount.IncTax < 0)
                {
                    foreach (PromotionBasketModifier objPromotion in UC_BasketSummary.GetPromotionsDiscount)
                    {
                        sbdBodyText.AppendLine(GetItemEmailText(objPromotion.Quantity + " x " + GetGlobalResourceObject("Kartris", "ContentText_PromotionDiscount"), objPromotion.Name, objPromotion.ExTax, objPromotion.IncTax, objPromotion.TaxAmount, objPromotion.ComputedTaxRate));
                        if (blnUseHTMLOrderEmail)
                        {
                            sbdHTMLOrderContents.Append(GetHTMLEmailRowText(objPromotion.Quantity + " x " + GetGlobalResourceObject("Kartris", "ContentText_PromotionDiscount"), objPromotion.Name, objPromotion.ExTax, objPromotion.IncTax, objPromotion.TaxAmount, objPromotion.ComputedTaxRate));
                        }
                        if (!string.IsNullOrEmpty(strPromotionDescription))
                            strPromotionDescription += Constants.vbCrLf + objPromotion.Name;
                        else
                            strPromotionDescription += objPromotion.Name;
                    }
                }

                // Coupon discount
                if (objBasket.CouponDiscount.IncTax < 0)
                {
                    sbdBodyText.AppendLine(GetBasketModifierEmailText(objBasket.CouponDiscount, GetGlobalResourceObject("Kartris", "ContentText_CouponDiscount"), objBasket.CouponName));
                    if (blnUseHTMLOrderEmail)
                    {
                        sbdHTMLOrderContents.Append(GetBasketModifierHTMLEmailText(objBasket.CouponDiscount, GetGlobalResourceObject("Kartris", "ContentText_CouponDiscount"), objBasket.CouponName));
                    }
                }

                // Customer discount
                // We need to show this line if the discount exists (i.e. not zero) but
                // also if zero but there are items that are exempt from the discount, so
                // it's clear why the discount is zero.
                if (objBasket.CustomerDiscount.IncTax < 0 | objBasket.HasCustomerDiscountExemption)
                {
                    sbdBodyText.AppendLine(GetBasketModifierEmailText(objBasket.CustomerDiscount, GetGlobalResourceObject("Basket", "ContentText_Discount"), "[customerdiscountexempttext]"));
                    if (blnUseHTMLOrderEmail)
                    {
                        sbdHTMLOrderContents.Append(GetBasketModifierHTMLEmailText(objBasket.CustomerDiscount, GetGlobalResourceObject("Basket", "ContentText_Discount"), "[customerdiscountexempttext]"));
                    }
                }

                // Shipping
                sbdBodyText.AppendLine(GetBasketModifierEmailText(objBasket.ShippingPrice, GetGlobalResourceObject("Address", "ContentText_Shipping"), Interaction.IIf(string.IsNullOrEmpty(objBasket.ShippingDescription), objBasket.ShippingName, objBasket.ShippingDescription)));
                if (blnUseHTMLOrderEmail)
                {
                    sbdHTMLOrderContents.Append(GetBasketModifierHTMLEmailText(objBasket.ShippingPrice, GetGlobalResourceObject("Address", "ContentText_Shipping"), Interaction.IIf(string.IsNullOrEmpty(objBasket.ShippingDescription), objBasket.ShippingName, objBasket.ShippingDescription)));
                }

                // Order handling charge
                if (objBasket.OrderHandlingPrice.ExTax > 0)
                {
                    sbdBodyText.AppendLine(GetBasketModifierEmailText(objBasket.OrderHandlingPrice, GetGlobalResourceObject("Kartris", "ContentText_OrderHandlingCharge"), ""));
                    if (blnUseHTMLOrderEmail)
                    {
                        sbdHTMLOrderContents.Append(GetBasketModifierHTMLEmailText(objBasket.OrderHandlingPrice, GetGlobalResourceObject("Kartris", "ContentText_OrderHandlingCharge"), ""));
                    }
                }

                sbdBodyText.AppendLine(GetGlobalResourceObject("Email", "EmailText_OrderEmailBreaker"));

                // Order totals
                if (blnAppPricesIncTax == false | blnAppShowTaxDisplay)
                {
                    sbdBodyText.AppendLine(" " + GetGlobalResourceObject("Checkout", "ContentText_OrderValue") + " = " + CurrenciesBLL.FormatCurrencyPrice(CUR_ID, objBasket.FinalPriceExTax, default, false) + Constants.vbCrLf);
                    sbdBodyText.Append(" " + GetGlobalResourceObject("Kartris", "ContentText_Tax") + " = " + CurrenciesBLL.FormatCurrencyPrice(CUR_ID, objBasket.FinalPriceTaxAmount, default, false) + Interaction.IIf(blnAppUSmultistatetax, " (" + Math.Round(objBasket.D_Tax * 100, 5) + "%)", "") + Constants.vbCrLf);
                }
                var objLanguageElementsBLL = new LanguageElementsBLL();
                sbdBodyText.Append(" " + GetGlobalResourceObject("Basket", "ContentText_TotalInclusive") + " = " + CurrenciesBLL.FormatCurrencyPrice(CUR_ID, objBasket.FinalPriceIncTax, default, false) + " (" + CurrenciesBLL.CurrencyCode(CUR_ID) + " - " + objLanguageElementsBLL.GetElementValue(GetLanguageIDfromSession, CkartrisEnumerations.LANG_ELEM_TABLE_TYPE.Currencies, CkartrisEnumerations.LANG_ELEM_FIELD_NAME.Name, CUR_ID) + ")" + Constants.vbCrLf);
                sbdBodyText.AppendLine(GetGlobalResourceObject("Email", "EmailText_OrderEmailBreaker"));
                if (blnUseHTMLOrderEmail)
                {
                    sbdHTMLOrderContents.Append("<tr class=\"row_totals\"><td colspan=\"2\">");
                    if (blnAppPricesIncTax == false | blnAppShowTaxDisplay)
                    {
                        sbdHTMLOrderContents.AppendLine(" " + GetGlobalResourceObject("Checkout", "ContentText_OrderValue") + " = " + CurrenciesBLL.FormatCurrencyPrice(CUR_ID, objBasket.FinalPriceExTax, default, false) + "<br/>");
                        sbdHTMLOrderContents.Append(" " + GetGlobalResourceObject("Kartris", "ContentText_Tax") + " = " + CurrenciesBLL.FormatCurrencyPrice(CUR_ID, objBasket.FinalPriceTaxAmount, default, false) + Interaction.IIf(blnAppUSmultistatetax, " (" + Math.Round(objBasket.D_Tax * 100, 5) + "%)", "") + "<br/>");
                    }
                    sbdHTMLOrderContents.Append("(" + CurrenciesBLL.CurrencyCode(CUR_ID) + " - " + objLanguageElementsBLL.GetElementValue(GetLanguageIDfromSession, CkartrisEnumerations.LANG_ELEM_TABLE_TYPE.Currencies, CkartrisEnumerations.LANG_ELEM_FIELD_NAME.Name, CUR_ID) + ") <strong>" + GetGlobalResourceObject("Basket", "ContentText_TotalInclusive") + " = " + CurrenciesBLL.FormatCurrencyPrice(CUR_ID, objBasket.FinalPriceIncTax, default, false) + "</strong></td></tr>");
                }

                // Handle order total conversion to different currency.
                // Some payment systems only accept one currency, e.g.
                // USD. In this case, if you have multiple currencies
                // on your site, the amount needs to be converted to
                // this one currency in order to process the payment on
                // the payment gateway.
                double numGatewayTotalPrice;
                if (blnDifferentGatewayCurrency)
                {
                    numGatewayTotalPrice = CurrenciesBLL.FormatCurrencyPrice(intGatewayCurrency, CurrenciesBLL.ConvertCurrency(intGatewayCurrency, objBasket.FinalPriceIncTax, CUR_ID), false, false);
                    if (clsPlugin.GatewayName.ToLower == "bitcoin")
                        numGatewayTotalPrice = Math.Round(numGatewayTotalPrice, 8);

                    sbdBodyText.AppendLine(" " + GetGlobalResourceObject("Email", "EmailText_ProcessCurrencyExp1") + Constants.vbCrLf);
                    sbdBodyText.Append(" " + GetGlobalResourceObject("Email", "ContentText_TotalInclusive") + " = " + CurrenciesBLL.FormatCurrencyPrice(intGatewayCurrency, numGatewayTotalPrice, default, false) + " (" + CurrenciesBLL.CurrencyCode(intGatewayCurrency) + " - " + objLanguageElementsBLL.GetElementValue(GetLanguageIDfromSession, CkartrisEnumerations.LANG_ELEM_TABLE_TYPE.Currencies, CkartrisEnumerations.LANG_ELEM_FIELD_NAME.Name, intGatewayCurrency) + ")" + Constants.vbCrLf);
                    sbdBodyText.Append(GetGlobalResourceObject("Email", "EmailText_OrderEmailBreaker") + Constants.vbCrLf);

                    if (blnUseHTMLOrderEmail)
                    {
                        sbdHTMLOrderContents.Append("<tr class=\"row_processcurrency\"><td colspan=\"2\">");
                        sbdHTMLOrderContents.AppendLine(" " + GetGlobalResourceObject("Email", "EmailText_ProcessCurrencyExp1") + "<br/>");
                        sbdHTMLOrderContents.Append(" " + GetGlobalResourceObject("Email", "ContentText_TotalInclusive") + " = " + CurrenciesBLL.FormatCurrencyPrice(intGatewayCurrency, numGatewayTotalPrice, default, false) + " (" + CurrenciesBLL.CurrencyCode(intGatewayCurrency) + " - " + objLanguageElementsBLL.GetElementValue(GetLanguageIDfromSession, CkartrisEnumerations.LANG_ELEM_TABLE_TYPE.Currencies, CkartrisEnumerations.LANG_ELEM_FIELD_NAME.Name, intGatewayCurrency) + ")" + "<br/>");
                        sbdHTMLOrderContents.Append("</td></tr>");
                    }
                }
                else
                {
                    // User was using same currency as the gateway requires, or
                    // the gateway supports multiple currencies... no conversion
                    // needed.
                    numGatewayTotalPrice = objBasket.FinalPriceIncTax;
                }

                // Total Saved
                if (objBasket.TotalAmountSaved > 0 & KartSettingsManager.GetKartConfig("frontend.checkout.confirmation.showtotalsaved") == "y")
                {
                    strTempEmailTextHolder = " " + GetGlobalResourceObject("Email", "EmailText_TotalSaved1") + CurrenciesBLL.FormatCurrencyPrice(CUR_ID, objBasket.TotalAmountSaved, default, false) + GetGlobalResourceObject("Email", "EmailText_TotalSaved2") + Constants.vbCrLf;
                    sbdBodyText.AppendLine(strTempEmailTextHolder);
                    sbdBodyText.Append(GetGlobalResourceObject("Email", "EmailText_OrderEmailBreaker"));
                    if (blnUseHTMLOrderEmail)
                    {
                        sbdHTMLOrderEmail.Replace("[totalsavedline]", strTempEmailTextHolder.Replace(Constants.vbCrLf, "<br/>"));
                    }
                }
                else
                {
                    sbdHTMLOrderEmail.Replace("[totalsavedline]", "");
                }

                // Customer billing information
                sbdBodyText.Append(Constants.vbCrLf);
                {
                    var withBlock = UC_BillingAddress.SelectedAddress;
                    sbdBodyText.Append(" " + GetGlobalResourceObject("Email", "EmailText_PurchaseContactDetails") + Constants.vbCrLf);

                    if (!_blnAnonymousCheckout)
                    {
                        sbdBodyText.Append(" " + GetGlobalResourceObject("Address", "FormLabel_CardHolderName") + ": " + withBlock.FullName + Constants.vbCrLf + " " + GetGlobalResourceObject("Email", "EmailText_Company") + ": " + withBlock.Company + Constants.vbCrLf + Interaction.IIf(!string.IsNullOrEmpty(txtEUVAT.Text), " " + GetGlobalResourceObject("Invoice", "FormLabel_CardholderEUVatNum") + ": " + txtEUVAT.Text + Constants.vbCrLf, ""));
                    }

                    sbdBodyText.Append(" " + GetGlobalResourceObject("Kartris", "FormLabel_EmailAddress") + ": " + UC_KartrisLogin.UserEmailAddress + Constants.vbCrLf);

                    if (!_blnAnonymousCheckout)
                    {
                        sbdBodyText.Append(" " + GetGlobalResourceObject("Address", "FormLabel_Telephone") + ": " + withBlock.Phone + Constants.vbCrLf + Constants.vbCrLf);
                    }

                    sbdBodyText.Append(" " + GetGlobalResourceObject("Email", "EmailText_Address") + ":" + Constants.vbCrLf);

                    if (!_blnAnonymousCheckout)
                    {
                        sbdBodyText.Append(" " + withBlock.StreetAddress + Constants.vbCrLf + " " + withBlock.TownCity + Constants.vbCrLf + " " + withBlock.County + Constants.vbCrLf + " " + withBlock.Postcode + Constants.vbCrLf + " " + withBlock.Country.Name);
                    }
                    else
                    {
                        sbdBodyText.Append(GetGlobalResourceObject("Kartris", "ContentText_NotApplicable"));
                    }

                    sbdBodyText.Append(Constants.vbCrLf + Constants.vbCrLf + GetGlobalResourceObject("Email", "EmailText_OrderEmailBreaker") + Constants.vbCrLf);

                    if (blnUseHTMLOrderEmail)
                    {
                        if (!_blnAnonymousCheckout)
                        {
                            sbdHTMLOrderEmail.Replace("[billingname]", Server.HtmlEncode(withBlock.FullName));
                            // retrieve the company label text and value if present, otherwise hide both placeholders from the template
                            if (!string.IsNullOrEmpty(withBlock.Company))
                            {
                                sbdHTMLOrderEmail.Replace("[companylabel]", GetGlobalResourceObject("Email", "EmailText_Company") + ": ");
                                sbdHTMLOrderEmail.Replace("[billingcompany]", Server.HtmlEncode(withBlock.Company));
                            }
                            else
                            {
                                sbdHTMLOrderEmail.Replace("[companylabel]", "");
                                sbdHTMLOrderEmail.Replace("[billingcompany]<br />", "");
                                sbdHTMLOrderEmail.Replace("[billingcompany]", "");
                            }
                            // do the same for EUVAT number
                            if (!string.IsNullOrEmpty(txtEUVAT.Text))
                            {
                                sbdHTMLOrderEmail.Replace("[euvatnumberlabel]", GetGlobalResourceObject("Invoice", "FormLabel_CardholderEUVatNum") + ": ");
                                sbdHTMLOrderEmail.Replace("[euvatnumbervalue]", Server.HtmlEncode(txtEUVAT.Text));
                            }
                            else
                            {
                                sbdHTMLOrderEmail.Replace("[euvatnumberlabel]", "");
                                sbdHTMLOrderEmail.Replace("[euvatnumbervalue]<br />", "");
                                sbdHTMLOrderEmail.Replace("[euvatnumbervalue]", "");
                            }
                            // do the same for EORI number
                            if (!string.IsNullOrEmpty(txtEORI.Text))
                            {
                                sbdHTMLOrderEmail.Replace("[eorinumberlabel]", "EORI: ");
                                sbdHTMLOrderEmail.Replace("[eorinumbervalue]", Server.HtmlEncode(txtEORI.Text));
                            }
                            else
                            {
                                sbdHTMLOrderEmail.Replace("[eorinumberlabel]", "");
                                sbdHTMLOrderEmail.Replace("[eorinumbervalue]<br />", "");
                                sbdHTMLOrderEmail.Replace("[eorinumbervalue]", "");
                            }
                            sbdHTMLOrderEmail.Replace("[billingemail]", Server.HtmlEncode(UC_KartrisLogin.UserEmailAddress));
                            sbdHTMLOrderEmail.Replace("[billingphone]", Server.HtmlEncode(withBlock.Phone));
                            sbdHTMLOrderEmail.Replace("[billingstreetaddress]", Server.HtmlEncode(withBlock.StreetAddress));
                            sbdHTMLOrderEmail.Replace("[billingtowncity]", Server.HtmlEncode(withBlock.TownCity));
                            sbdHTMLOrderEmail.Replace("[billingcounty]", Server.HtmlEncode(withBlock.County));
                            sbdHTMLOrderEmail.Replace("[billingpostcode]", Server.HtmlEncode(withBlock.Postcode));
                            sbdHTMLOrderEmail.Replace("[billingcountry]", Server.HtmlEncode(withBlock.Country.Name));
                        }
                        else
                        {
                            sbdHTMLOrderEmail.Replace("[billingname]", GetGlobalResourceObject("Kartris", "ContentText_NotApplicable"));

                            sbdHTMLOrderEmail.Replace("[companylabel]", "");
                            sbdHTMLOrderEmail.Replace("[billingcompany]<br />", "");
                            sbdHTMLOrderEmail.Replace("[billingcompany]", "");

                            sbdHTMLOrderEmail.Replace("[euvatnumberlabel]", "");
                            sbdHTMLOrderEmail.Replace("[euvatnumbervalue]<br />", "");
                            sbdHTMLOrderEmail.Replace("[euvatnumbervalue]", "");

                            sbdHTMLOrderEmail.Replace("[billingemail]", Server.HtmlEncode(UC_KartrisLogin.UserEmailAddress));

                            sbdHTMLOrderEmail.Replace("[billingphone]", GetGlobalResourceObject("Kartris", "ContentText_NotApplicable"));
                            sbdHTMLOrderEmail.Replace("[billingstreetaddress]", "");
                            sbdHTMLOrderEmail.Replace("[billingtowncity]", "");
                            sbdHTMLOrderEmail.Replace("[billingcounty]", "");
                            sbdHTMLOrderEmail.Replace("[billingpostcode]", "");
                            sbdHTMLOrderEmail.Replace("[billingcountry]", "");
                        }

                    }
                }

                // Shipping info
                sbdBodyText.Append(" " + GetGlobalResourceObject("Email", "EmailText_ShippingDetails") + Constants.vbCrLf);
                string strShippingAddressEmailText = "";

                if (!blnBasketAllDigital)
                {
                    if (chkSameShippingAsBilling.Checked)
                    {
                        {
                            var withBlock1 = UC_BillingAddress.SelectedAddress;
                            strShippingAddressEmailText = " " + withBlock1.FullName + Constants.vbCrLf + " " + withBlock1.Company + Constants.vbCrLf + " " + withBlock1.StreetAddress + Constants.vbCrLf + " " + withBlock1.TownCity + Constants.vbCrLf + " " + withBlock1.County + Constants.vbCrLf + " " + withBlock1.Postcode + Constants.vbCrLf + " " + withBlock1.Country.Name + Constants.vbCrLf + Constants.vbCrLf;
                            sbdHTMLOrderEmail.Replace("[shippingname]", Server.HtmlEncode(withBlock1.FullName));
                            sbdHTMLOrderEmail.Replace("[shippingstreetaddress]", Server.HtmlEncode(withBlock1.StreetAddress));
                            sbdHTMLOrderEmail.Replace("[shippingtowncity]", Server.HtmlEncode(withBlock1.TownCity));
                            sbdHTMLOrderEmail.Replace("[shippingcounty]", Server.HtmlEncode(withBlock1.County));
                            sbdHTMLOrderEmail.Replace("[shippingpostcode]", Server.HtmlEncode(withBlock1.Postcode));
                            sbdHTMLOrderEmail.Replace("[shippingcountry]", Server.HtmlEncode(withBlock1.Country.Name));
                            sbdHTMLOrderEmail.Replace("[shippingphone]", Server.HtmlEncode(withBlock1.Phone));
                            if (!string.IsNullOrEmpty(withBlock1.Company))
                            {
                                sbdHTMLOrderEmail.Replace("[shippingcompany]", Server.HtmlEncode(withBlock1.Company));
                            }
                            else
                            {
                                sbdHTMLOrderEmail.Replace("[shippingcompany]<br />", "");
                                sbdHTMLOrderEmail.Replace("[shippingcompany]", "");
                            }
                        }
                    }
                    else
                    {
                        {
                            var withBlock2 = UC_ShippingAddress.SelectedAddress;
                            strShippingAddressEmailText = " " + withBlock2.FullName + Constants.vbCrLf + " " + withBlock2.Company + Constants.vbCrLf + " " + withBlock2.StreetAddress + Constants.vbCrLf + " " + withBlock2.TownCity + Constants.vbCrLf + " " + withBlock2.County + Constants.vbCrLf + " " + withBlock2.Postcode + Constants.vbCrLf + " " + withBlock2.Country.Name + Constants.vbCrLf + Constants.vbCrLf;
                            sbdHTMLOrderEmail.Replace("[shippingname]", Server.HtmlEncode(withBlock2.FullName));
                            sbdHTMLOrderEmail.Replace("[shippingstreetaddress]", Server.HtmlEncode(withBlock2.StreetAddress));
                            sbdHTMLOrderEmail.Replace("[shippingtowncity]", Server.HtmlEncode(withBlock2.TownCity));
                            sbdHTMLOrderEmail.Replace("[shippingcounty]", Server.HtmlEncode(withBlock2.County));
                            sbdHTMLOrderEmail.Replace("[shippingpostcode]", Server.HtmlEncode(withBlock2.Postcode));
                            sbdHTMLOrderEmail.Replace("[shippingcountry]", Server.HtmlEncode(withBlock2.Country.Name));
                            sbdHTMLOrderEmail.Replace("[shippingphone]", Server.HtmlEncode(withBlock2.Phone));
                            if (!string.IsNullOrEmpty(withBlock2.Company))
                            {
                                sbdHTMLOrderEmail.Replace("[shippingcompany]", Server.HtmlEncode(withBlock2.Company));
                            }
                            else
                            {
                                sbdHTMLOrderEmail.Replace("[shippingcompany]", "");
                            }
                        }
                    }
                }
                else
                {
                    // Now we could blank out the shipping address details, not
                    // really relevant for alldownloadable orders, although payment
                    // systems still require them

                    strShippingAddressEmailText = GetGlobalResourceObject("Kartris", "ContentText_NotApplicable") + Constants.vbCrLf + Constants.vbCrLf;
                    sbdHTMLOrderEmail.Replace("[shippingname]", GetGlobalResourceObject("Kartris", "ContentText_NotApplicable"));
                    sbdHTMLOrderEmail.Replace("[shippingstreetaddress]", "");
                    sbdHTMLOrderEmail.Replace("[shippingtowncity]", "");
                    sbdHTMLOrderEmail.Replace("[shippingcounty]", "");
                    sbdHTMLOrderEmail.Replace("[shippingpostcode]", "");
                    sbdHTMLOrderEmail.Replace("[shippingcountry]", "");
                    sbdHTMLOrderEmail.Replace("[shippingphone]", "");

                    sbdHTMLOrderEmail.Replace("[shippingcompany]<br />", "");
                    sbdHTMLOrderEmail.Replace("[shippingcompany]", "");
                }


                sbdBodyText.Append(strShippingAddressEmailText + GetGlobalResourceObject("Email", "EmailText_OrderEmailBreaker") + Constants.vbCrLf);

                // Comments and additional info
                if (!string.IsNullOrEmpty(Strings.Trim(txtComments.Text)))
                {
                    strTempEmailTextHolder = " " + GetGlobalResourceObject("Email", "EmailText_Comments") + ": " + Constants.vbCrLf + Constants.vbCrLf + " " + txtComments.Text + Constants.vbCrLf + Constants.vbCrLf;
                    sbdBodyText.Append(strTempEmailTextHolder);
                    if (blnUseHTMLOrderEmail)
                    {
                        sbdHTMLOrderEmail.Replace("[ordercomments]", Server.HtmlEncode(strTempEmailTextHolder).Replace(Constants.vbCrLf, "<br/>"));
                    }
                }
                else
                {
                    sbdHTMLOrderEmail.Replace("[ordercomments]", "");
                }

                sbdBodyText.Append(" " + GetGlobalResourceObject("Email", "EmailText_OrderTime2") + ": " + CkartrisDisplayFunctions.NowOffset + Constants.vbCrLf);
                sbdBodyText.Append(" " + GetGlobalResourceObject("Email", "EmailText_IPAddress") + ": " + CkartrisEnvironment.GetClientIPAddress() + Constants.vbCrLf);
                sbdBodyText.Append(" " + Request.ServerVariables("HTTP_USER_AGENT") + Constants.vbCrLf);
                if (blnUseHTMLOrderEmail)
                {
                    sbdHTMLOrderEmail.Replace("[nowoffset]", CkartrisDisplayFunctions.NowOffset);
                    sbdHTMLOrderEmail.Replace("[customerip]", CkartrisEnvironment.GetClientIPAddress());
                    sbdHTMLOrderEmail.Replace("[customeruseragent]", Request.ServerVariables("HTTP_USER_AGENT"));
                    sbdHTMLOrderEmail.Replace("[webshopurl]", CkartrisBLL.WebShopURL);
                    sbdHTMLOrderEmail.Replace("[websitename]", Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname")));
                }

                // ======================================
                // SEND NEW ACCOUNT DETAILS
                // This is probably not required and
                // it also sends the user password that
                // they chose too. For this reason, we
                // now turn off at default in the config
                // setting. But it can be turned on if
                // required.
                // ======================================
                if (KartSettingsManager.GetKartConfig("frontend.users.emailnewaccountdetails") == "y" & blnNewUser)
                {
                    // Build up email text
                    strSubject = GetGlobalResourceObject("Email", "Config_Subjectline5");

                    sbdNewCustomerEmailText.Append(GetGlobalResourceObject("Email", "EmailText_CustomerSignedUpHeader") + Constants.vbCrLf + Constants.vbCrLf);
                    sbdNewCustomerEmailText.Append(GetGlobalResourceObject("Email", "EmailText_EmailAddress") + UC_KartrisLogin.UserEmailAddress + Constants.vbCrLf);
                    sbdNewCustomerEmailText.Append(GetGlobalResourceObject("Email", "EmailText_CustomerCode") + UC_KartrisLogin.UserPassword + Constants.vbCrLf + Constants.vbCrLf);
                    sbdNewCustomerEmailText.Append(GetGlobalResourceObject("Email", "EmailText_CustomerSignedUpFooter1") + CkartrisBLL.WebShopURL + "Customer.aspx" + GetGlobalResourceObject("Email", "EmailText_CustomerSignedUpFooter2"));
                    sbdNewCustomerEmailText.Replace("<br>", Constants.vbCrLf).Replace("<br />", Constants.vbCrLf);
                }

                sbdBodyText.Insert(0, sbdBasketItems.ToString());

                arrBasketItems = UC_BasketView.GetBasketItems;

                var objObjectConfigBLL = new ObjectConfigBLL(); // needed in loops below

                if (arrBasketItems is not null)
                {
                    var BasketItem = new BasketItem();
                    // final check if basket items are still there
                    if (arrBasketItems.Count == 0)
                    {
                        CkartrisFormatErrors.LogError(Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("Basket items were lost in the middle of Checkout! Customer redirected to main Basket page." + Constants.vbCrLf + "Details: " + Constants.vbCrLf + "C_ID:", Interaction.IIf(User.Identity.IsAuthenticated, CurrentLoggedUser.ID, " New User")), Constants.vbCrLf), "Payment Gateway: ")) + clsPlugin.GatewayName + Constants.vbCrLf + "Generated Body Text: " + sbdBodyText.ToString());
                        Response.Redirect("~/Basket.aspx");
                    }

                    // Get customer discount, we need this to decide whether to mark items
                    // exempt from it
                    double BSKT_CustomerDiscount = 0d;
                    try
                    {
                        BSKT_CustomerDiscount = BasketBLL.GetCustomerDiscount(CurrentLoggedUser.ID);
                    }
                    catch (Exception ex)
                    {
                        // New user, just defaults to zero as no customer discount in this case
                    }


                    // We need to mark items that are exempt from customer discounts
                    string strMark = "";
                    bool blnHasExemptCustomerDiscountItems = false;

                    // Loop through basket items
                    foreach (Kartris.BasketItem Item in arrBasketItems)
                    {
                        string strCustomControlName = objObjectConfigBLL.GetValue("K:product.customcontrolname", Item.ProductID);
                        string strCustomText = "";

                        var sbdOptionText = new StringBuilder("");
                        if (!string.IsNullOrEmpty(Item.OptionText))
                            sbdOptionText.Append(Constants.vbCrLf + " " + Item.OptionText().Replace("<br>", Constants.vbCrLf + " ").Replace("<br />", Constants.vbCrLf + " "));

                        // Set string to blank or **, to mark items exempt from customer discount
                        if (Item.ExcludeFromCustomerDiscount & BSKT_CustomerDiscount > 0d)
                        {
                            strMark = " **";
                            blnHasExemptCustomerDiscountItems = true;
                        }
                        else
                        {
                            strMark = "";
                        }

                        // Append line for this item
                        sbdBasketItems.AppendLine(GetItemEmailText(Item.Quantity + " x " + Item.ProductName + strMark, Item.VersionName + " (" + Item.CodeNumber + ")" + sbdOptionText.ToString(), Item.ExTax, Item.IncTax, Item.TaxAmount, Item.ComputedTaxRate));

                        if (Item.CustomText != "" && string.IsNullOrEmpty(strCustomControlName))
                        {
                            // Add custom text to mail
                            strCustomText = " [ " + Item.CustomText + " ]" + Constants.vbCrLf;
                            sbdBasketItems.Append(strCustomText);
                        }
                        if (blnUseHTMLOrderEmail)
                        {
                            // this line builds up the individual rows of the order contents table in the HTML email
                            sbdHTMLOrderBasket.AppendLine(GetHTMLEmailRowText(Item.Quantity + " x " + Item.ProductName + strMark, Item.VersionName + " (" + Item.CodeNumber + ") " + sbdOptionText.ToString() + strCustomText, Item.ExTax, Item.IncTax, Item.TaxAmount, Item.ComputedTaxRate, 0, Item.VersionID, Item.ProductID));
                        }
                    }

                    // Now we know if there are customer discount exempt items, can replace
                    // [customerdiscountexempttext] which was inserted with the customer discount
                    // line further above.
                    if (blnHasExemptCustomerDiscountItems)
                    {
                        sbdBodyText.Replace("[customerdiscountexempttext]", GetGlobalResourceObject("Basket", "ContentText_SomeItemsExcludedFromDiscount"));
                        sbdHTMLOrderContents.Replace("[customerdiscountexempttext]", GetGlobalResourceObject("Basket", "ContentText_SomeItemsExcludedFromDiscount"));
                    }
                    else
                    {
                        sbdBodyText.Replace("[customerdiscountexempttext]", "");
                        sbdHTMLOrderContents.Replace("[customerdiscountexempttext]", "");
                    }
                }

                sbdBodyText.Insert(0, sbdBasketItems.ToString());

                if (blnUseHTMLOrderEmail)
                {
                    // build up the table and the header tags, insert basket contents
                    sbdHTMLOrderContents.Insert(0, "<table id=\"orderitems\"><thead><tr>" + Constants.vbCrLf + "<th class=\"col1\">" + GetGlobalResourceObject("Kartris", "ContentText_Item") + "</th>" + Constants.vbCrLf + "<th class=\"col2\">" + GetGlobalResourceObject("Kartris", "ContentText_Price") + "</th></thead><tbody>" + Constants.vbCrLf + sbdHTMLOrderBasket.ToString());
                    // finally close the order contents HTML table
                    sbdHTMLOrderContents.Append("</tbody></table>");
                    // and append the order contents to the main HTML email
                    sbdHTMLOrderEmail.Replace("[ordercontents]", sbdHTMLOrderContents.ToString());
                }

                // check if shippingdestinationid is initialized, if not then reload checkout page
                if (!blnBasketAllDigital)
                {
                    if (UC_BasketSummary.ShippingDestinationID == 0 | UC_BasketView.ShippingDestinationID == 0)
                    {
                        CkartrisFormatErrors.LogError("Basket was reset. Shipping info lost." + Constants.vbCrLf + "BasketView Shipping Destination ID: " + UC_BasketView.ShippingDestinationID + Constants.vbCrLf + "BasketSummary Shipping Destination ID: " + UC_BasketSummary.ShippingDestinationID);
                        Response.Redirect("~/Checkout.aspx");
                    }
                }

                // If this is guest checkout, let's set a boolean value
                // we can pass to the ordersBLL below, this way, the new
                // account created is tagged as guest
                bool blnIsGuest = Session("_IsGuest") == "YES";

                // New instance of orders BLL
                var objOrdersBLL = new OrdersBLL();

                // Create the order record
                O_ID = objOrdersBLL.Add(C_ID, UC_KartrisLogin.UserEmailAddress, UC_KartrisLogin.UserPassword, UC_BillingAddress.SelectedAddress, UC_ShippingAddress.SelectedAddress, chkSameShippingAsBilling.Checked, objBasket, arrBasketItems, Interaction.IIf(blnUseHTMLOrderEmail, sbdHTMLOrderEmail.ToString(), sbdBodyText.ToString()), clsPlugin.GatewayName, Session("LANG"), CUR_ID, intGatewayCurrency, chkOrderEmails.Checked, UC_BasketView.SelectedShippingMethod, numGatewayTotalPrice, Interaction.IIf(string.IsNullOrEmpty(txtEUVAT.Text), "", txtEUVAT.Text), strPromotionDescription, txtPurchaseOrderNo.Text, Strings.Trim(txtComments.Text), blnIsGuest);

                // Store EORI number for client
                try
                {
                    bool blnUpdatedEORI = objObjectConfigBLL.SetConfigValue("K:user.eori", C_ID, txtEORI.Text, "");
                }
                catch (Exception ex)
                {
                    CkartrisFormatErrors.LogError("Error updating K:user.eori");
                }


                Session("_IsGuest") = null;

                // Mailchimp
                bool blnMailChimp = KartSettingsManager.GetKartConfig("general.mailchimp.enabled") == "y";

                if (blnMailChimp)
                {
                    // MAILCHIMP Adding Cart
                    var mailChimpLib = new MailChimpBLL(CurrentLoggedUser, objBasket, CurrenciesBLL.CurrencyCode(Session("CUR_ID")));
                    // If the User is Logged
                    if (CurrentLoggedUser is not null & string.IsNullOrEmpty(Session("BraintreeCartId")))
                    {
                        string addCartResult = mailChimpLib.AddCartToCustomerToStore(O_ID).Result;
                    }
                }

                // Order Creation successful
                if (O_ID > 0)
                {
                    Session("OrderID") = O_ID; // Google analytics needs this later

                    objOrder = new Kartris.Interfaces.objOrder();
                    // Create the Order object and fill in the property values.
                    objOrder.ID = O_ID;
                    objOrder.Description = GetGlobalResourceObject("Kartris", "Config_OrderDescription");
                    objOrder.Amount = numGatewayTotalPrice;
                    objOrder.ShippingPrice = objBasket.ShippingPrice.IncTax;
                    objOrder.OrderHandlingPrice = objBasket.OrderHandlingPrice.IncTax;
                    objOrder.ShippingExTaxPrice = objBasket.ShippingPrice.ExTax;
                    objOrder.Currency = CurrenciesBLL.CurrencyCode(CUR_ID);

                    // Set billing address for order
                    if (!_blnAnonymousCheckout)
                    {
                        {
                            var withBlock3 = UC_BillingAddress.SelectedAddress;
                            objOrder.Billing.Name = withBlock3.FullName;
                            objOrder.Billing.StreetAddress = withBlock3.StreetAddress;
                            objOrder.Billing.TownCity = withBlock3.TownCity;
                            objOrder.Billing.CountyState = withBlock3.County;
                            objOrder.Billing.CountryName = withBlock3.Country.Name;
                            objOrder.Billing.Postcode = withBlock3.Postcode;
                            objOrder.Billing.Phone = withBlock3.Phone;
                            objOrder.Billing.CountryIsoCode = withBlock3.Country.IsoCode;
                            objOrder.Billing.Company = withBlock3.Company;
                        }
                    }

                    // Set shipping address for order
                    if (!blnBasketAllDigital)
                    {
                        if (chkSameShippingAsBilling.Checked)
                        {
                            objOrder.SameShippingAsBilling = true;
                        }
                        else
                        {
                            {
                                var withBlock4 = UC_ShippingAddress.SelectedAddress;
                                objOrder.Shipping.Name = withBlock4.FullName;
                                objOrder.Shipping.StreetAddress = withBlock4.StreetAddress;
                                objOrder.Shipping.TownCity = withBlock4.TownCity;
                                objOrder.Shipping.CountyState = withBlock4.County;
                                objOrder.Shipping.CountryName = withBlock4.Country.Name;
                                objOrder.Shipping.Postcode = withBlock4.Postcode;
                                objOrder.Shipping.Phone = withBlock4.Phone;
                                objOrder.Shipping.CountryIsoCode = withBlock4.Country.IsoCode;
                                objOrder.Shipping.Company = withBlock4.Company;
                            }
                        }
                    }
                    else
                    {
                        // For digital orders, we always set shipping same as 
                        // billing on the order. We generally hide it from view
                        // but most payment systems require shipping address
                        // so this keeps them happy
                        objOrder.SameShippingAsBilling = true;
                    }

                    objOrder.CustomerIPAddress = Request.ServerVariables("REMOTE_HOST");
                    objOrder.CustomerEmail = UC_KartrisLogin.UserEmailAddress;

                    if (!User.Identity.IsAuthenticated)
                    {
                        if (Membership.ValidateUser(UC_KartrisLogin.UserEmailAddress, UC_KartrisLogin.UserPassword))
                        {
                            FormsAuthentication.SetAuthCookie(UC_KartrisLogin.UserEmailAddress, true);
                        }
                    }
                    try
                    {
                        KartrisMemberShipUser KartrisUser = Membership.GetUser(UC_KartrisLogin.UserEmailAddress);
                        if (KartrisUser is not null)
                        {
                            objOrder.CustomerID = KartrisUser.ID;
                        }
                    }
                    catch (Exception ex)
                    {
                        objOrder.CustomerID = 0;
                    }

                    objOrder.CustomerLanguage = LanguagesBLL.GetUICultureByLanguageID_s(Session("LANG"));


                    string strFromEmail = LanguagesBLL.GetEmailFrom(Session("LANG"));

                    // Send new account email to new customer
                    if (KartSettingsManager.GetKartConfig("frontend.users.emailnewaccountdetails") == "y" & blnNewUser & !blnIsGuest)
                    {

                        bool blnHTMLEmail = KartSettingsManager.GetKartConfig("general.email.enableHTML") == "y";
                        if (blnHTMLEmail)
                        {
                            string strHTMLEmailText = RetrieveHTMLEmailTemplate("NewCustomerSignUp");
                            // build up the HTML email if template is found
                            if (!string.IsNullOrEmpty(strHTMLEmailText))
                            {
                                strHTMLEmailText = strHTMLEmailText.Replace("[webshopurl]", WebShopURL);
                                strHTMLEmailText = strHTMLEmailText.Replace("[websitename]", GetGlobalResourceObject("Kartris", "Config_Webshopname"));
                                strHTMLEmailText = strHTMLEmailText.Replace("[customeremail]", UC_KartrisLogin.UserEmailAddress);
                                strHTMLEmailText = strHTMLEmailText.Replace("[customerpassword]", UC_KartrisLogin.UserPassword);
                                sbdNewCustomerEmailText.Clear();
                                sbdNewCustomerEmailText.Append(strHTMLEmailText);
                            }
                            else
                            {
                                blnHTMLEmail = false;
                            }
                        }
                        SendEmail(strFromEmail, UC_KartrisLogin.UserEmailAddress, strSubject, sbdNewCustomerEmailText.ToString(), default, default, default, default, blnHTMLEmail);
                    }

                    // Mailing List
                    if (chkMailingList.Checked)
                    {
                        DateTime ML_SignupDateTime, ML_ConfirmationDateTime;
                        bool blnSignupCustomer = false;
                        if (objOrder.CustomerID > 0)
                        {
                            DataTable tblCustomerData = BasketBLL.GetCustomerData(objOrder.CustomerID);
                            if (tblCustomerData.Rows.Count > 0)
                            {
                                // '// mailing list
                                ML_ConfirmationDateTime = FixNullFromDB(tblCustomerData.Rows(0).Item("U_ML_ConfirmationDateTime"));
                                ML_SignupDateTime = FixNullFromDB(tblCustomerData.Rows(0).Item("U_ML_SignupDateTime"));
                                if (!(ML_SignupDateTime > new DateTime(1900, 1, 1) | ML_ConfirmationDateTime > new DateTime(1900, 1, 1)))
                                    blnSignupCustomer = true;
                            }
                        }
                        else
                        {
                            blnSignupCustomer = false;
                        }


                        if (blnSignupCustomer)
                        {
                            string strRandomString = "";

                            BasketBLL.UpdateCustomerMailingList(UC_KartrisLogin.UserEmailAddress, strRandomString, "h", objOrder.CustomerIPAddress);


                            // If mailchimp is active, we want to add the user to the mailing list
                            if (KartSettingsManager.GetKartConfig("general.mailchimp.enabled") == "y")
                            {
                                // Add user direct to MailChimp
                                BasketBLL.AddListSubscriber(UC_KartrisLogin.UserEmailAddress);
                            }
                            else
                            {
                                // Use the built in mailing list
                                var sbdMLBodyText = new StringBuilder();
                                string strBodyText;
                                string strMailingListSignUpLink = WebShopURL() + "Default.aspx?id=" + objOrder.CustomerID + "&r=" + strRandomString;

                                sbdMLBodyText.Append(GetGlobalResourceObject("Kartris", "EmailText_NewsletterSignup") + Constants.vbCrLf + Constants.vbCrLf);
                                sbdMLBodyText.Append(strMailingListSignUpLink + Constants.vbCrLf + Constants.vbCrLf);
                                sbdMLBodyText.Append(GetGlobalResourceObject("Kartris", "EmailText_NewsletterAuthorizeFooter"));

                                strBodyText = sbdMLBodyText.ToString();
                                strBodyText = Strings.Replace(strBodyText, "[IPADDRESS]", objOrder.CustomerIPAddress);
                                strBodyText = Strings.Replace(strBodyText, "[WEBSHOPNAME]", GetGlobalResourceObject("Kartris", "Config_Webshopname"));
                                strBodyText = Strings.Replace(strBodyText, "[WEBSHOPURL]", WebShopURL);
                                strBodyText = strBodyText + GetGlobalResourceObject("Kartris", "ContentText_NewsletterSignup");

                                bool blnHTMLEmail = KartSettingsManager.GetKartConfig("general.email.enableHTML") == "y";
                                if (blnHTMLEmail)
                                {
                                    string strHTMLEmailText = RetrieveHTMLEmailTemplate("MailingListSignUp");
                                    // build up the HTML email if template is found
                                    if (!string.IsNullOrEmpty(strHTMLEmailText))
                                    {
                                        strHTMLEmailText = strHTMLEmailText.Replace("[mailinglistconfirmationlink]", strMailingListSignUpLink);
                                        strHTMLEmailText = strHTMLEmailText.Replace("[websitename]", GetGlobalResourceObject("Kartris", "Config_Webshopname"));
                                        strHTMLEmailText = strHTMLEmailText.Replace("[customerip]", objOrder.CustomerIPAddress);
                                        strBodyText = strHTMLEmailText;
                                    }
                                    else
                                    {
                                        blnHTMLEmail = false;
                                    }
                                }

                                // GDPR Mod - v2.9013 
                                // We want to be able to have a log of all opt-in
                                // requests sent, so we can prove the user was sent
                                // the GDPR notice, and also prove what text they
                                // received. We do this by BCCing the confirmation
                                // opt-in mail to an email address. A free account
                                // like gmail would be good for this.
                                var objBCCsCollection = new System.Net.Mail.MailAddressCollection();
                                string strGDPROptinArchiveEmail = KartSettingsManager.GetKartConfig("general.gdpr.mailinglistbcc");
                                if (strGDPROptinArchiveEmail.Length > 2)
                                {
                                    objBCCsCollection.Add(strGDPROptinArchiveEmail);
                                }

                                SendEmail(strFromEmail, UC_KartrisLogin.UserEmailAddress, GetGlobalResourceObject("Kartris", "PageTitle_MailingList"), strBodyText, default, default, default, default, blnHTMLEmail, default, objBCCsCollection);
                            }

                        }
                    }

                    // Save Basket
                    if (chkSaveBasket.Checked)
                    {
                        BasketBLL.SaveBasket(objOrder.CustomerID, "Order #" + O_ID + ", " + CkartrisDisplayFunctions.NowOffset, Session("SessionID"));
                    }

                    // v2.9010 Autosave basket
                    // This is in addition to the normal saving process, which lets a 
                    // customer save a named basket
                    try
                    {
                        BasketBLL.AutosaveBasket(objOrder.CustomerID);
                    }
                    catch (Exception ex)
                    {
                        // User not logged in
                    }

                    // objOrder.WebShopURL = Page.Request.Url.ToString.Replace("?new=y", "")
                    objOrder.WebShopURL = WebShopURL() + "Checkout.aspx";

                    // serialize order object and store it as a session value
                    Session("objOrder") = Payment.Serialize(objOrder);

                    // update data field with serialized order and basket objects and selected shipping method id - this allows us to edit this order later if needed
                    objOrdersBLL.DataUpdate(O_ID, Session("objOrder") + "|||" + Payment.Serialize(objBasket) + "|||" + UC_BasketView.SelectedShippingID);

                    string transactionId = "";
                    if (LCase(clsPlugin.GatewayType) == "local")
                    {
                        // ---------------------------------------
                        // LOCAL PROCESS
                        // This includes gateways where card
                        // details are taken through the checkout
                        // page and relayed to the gateway, and
                        // also the 'PO offline' method where
                        // orders are made without payment, to be
                        // paid later offline.
                        // ---------------------------------------
                        bool blnResult;
                        string strBitcoinPaymentAddress = "";

                        string strEasypayPayment = "";
                        string strBraintreePayment = "";

                        if (clsPlugin.GatewayName.ToLower == "po_offlinepayment" || clsPlugin.GatewayName.ToLower == "bitcoin" || clsPlugin.GatewayName.ToLower == "easypaymultibanco" || clsPlugin.GatewayName.ToLower == "braintreepayment")
                        {
                            // PO orders don't need authorizing, they are
                            // effectively successful if placed as payment
                            // will come later
                            if (clsPlugin.GatewayName.ToLower == "bitcoin")
                            {
                                strBitcoinPaymentAddress = clsPlugin.ProcessOrder(Payment.Serialize(objOrder));
                                blnResult = true;
                            }
                            else if (clsPlugin.GatewayName.ToLower == "easypaymultibanco")
                            {
                                strEasypayPayment = clsPlugin.ProcessOrder(Payment.Serialize(objOrder));
                                blnResult = true;
                                BasketBLL.DeleteBasket();
                                Session("Basket") = null;
                            }
                            else if (clsPlugin.GatewayName.ToLower == "braintreepayment")      // if the user selected BrainTree as a paying method, retrieves some data from the form and calls PaymentsBLL to perform the transaction
                            {
                                string paymentMethodNonce, output;
                                decimal amount;
                                short CurrencyID;
                                CurrencyID = Session("CUR_ID");
                                try
                                {
                                    paymentMethodNonce = Request.Form("ctl00$cntMain$tbPaymentMethodNonce");
                                    amount = Request.Form("ctl00$cntMain$tbAmount");
                                    try
                                    {
                                        transactionId = PaymentsBLL.BrainTreePayment(paymentMethodNonce, amount, CurrencyID);
                                    }
                                    catch (BrainTreeException bEx)
                                    {
                                        transactionId = "";
                                        UC_PopUpErrors.SetTextMessage = bEx.CustomMessage is not null ? bEx.CustomMessage : bEx.Message;
                                        UC_PopUpErrors.SetTitle = GetGlobalResourceObject("Kartris", "ContentText_CorrectErrors");
                                        UC_PopUpErrors.ShowPopup();
                                        mvwCheckout.ActiveViewIndex = 1;
                                        blnResult = false;
                                    }


                                    if (!string.IsNullOrEmpty(transactionId))
                                    {
                                        blnResult = true;
                                        try
                                        {
                                            // Mailchimp, try to remove cart
                                            if (blnMailChimp)
                                            {
                                                // Mailchimp library
                                                var mailChimpLib = new MailChimpBLL(CurrentLoggedUser, objBasket, CurrenciesBLL.CurrencyCode(Session("CUR_ID")));

                                                string cartId = Session("BraintreeCartId");
                                                if (cartId is not null)
                                                {
                                                    // Removing Cart and adding Order to successful payment made with Braintree
                                                    MailChimp.Net.Models.Customer mcCustomer = mailChimpLib.GetCustomer(CurrentLoggedUser.ID).Result;
                                                    Order mcOrder = mailChimpLib.AddOrder(mcCustomer, cartId).Result;
                                                    bool mcDeleteCart = mailChimpLib.DeleteCart(cartId).Result;
                                                    Session("BraintreeCartId") = null;

                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.Print(ex.Message);
                                        }
                                        BasketBLL.DeleteBasket();
                                        Session("Basket") = null;

                                    }
                                }
                                catch (Exception ex)
                                {
                                    output = "Error: 81503: Amount is an invalid format.";
                                }
                            }

                            blnResult = true;
                        }
                        else
                        {
                            // ---------------------------------------
                            // COLLECT CARD DETAILS
                            // ---------------------------------------
                            objOrder.CreditCardInfo.CardNumber = UC_CreditCardInput.CardNumber;
                            objOrder.CreditCardInfo.CardType = UC_CreditCardInput.CardType;
                            objOrder.CreditCardInfo.BeginMonth = Strings.Left(UC_CreditCardInput.StartDate, 2);
                            objOrder.CreditCardInfo.BeginYear = Strings.Right(UC_CreditCardInput.StartDate, 2);
                            objOrder.CreditCardInfo.IssueNumber = UC_CreditCardInput.CardIssueNumber;
                            objOrder.CreditCardInfo.CV2 = UC_CreditCardInput.CardSecurityNumber;
                            objOrder.CreditCardInfo.ExpiryMonth = Strings.Left(UC_CreditCardInput.ExpiryDate, 2);
                            objOrder.CreditCardInfo.ExpiryYear = Strings.Right(UC_CreditCardInput.ExpiryDate, 2);

                            // ---------------------------------------
                            // VALIDATE CREDIT CARD DETAILS
                            // Did the gateway return a 'sucess' result on validation?
                            // ---------------------------------------
                            blnResult = clsPlugin.ValidateCardOrder(Payment.Serialize(objOrder), Payment.Serialize(objBasket));
                        }

                        // ---------------------------------------
                        // SUCCESSFULLY PLACED ORDER
                        // ---------------------------------------
                        if (blnResult)
                        {
                            // The transaction was authorized so update the order
                            if (clsPlugin.CallbackSuccessful | clsPlugin.GatewayName.ToLower == "po_offlinepayment" | clsPlugin.GatewayName.ToLower == "easypaymultibanco" | clsPlugin.GatewayName.ToLower == "braintreepayment")
                            {
                                if (clsPlugin.GatewayName.ToLower == "po_offlinepayment" | clsPlugin.GatewayName.ToLower == "easypaymultibanco" | clsPlugin.GatewayName.ToLower == "braintreepayment")
                                {
                                    O_ID = objOrder.ID;
                                }
                                else
                                {
                                    // Get order ID that was passed back with callback
                                    O_ID = clsPlugin.CallbackOrderID;
                                }

                                // ---------------------------------------
                                // CREATE DATATABLE OF ORDER
                                // ---------------------------------------
                                DataTable tblOrder = objOrdersBLL.GetOrderByID(O_ID);

                                string O_CouponCode = "";
                                double O_TotalPriceGateway = 0d;
                                string O_PurchaseOrderNo = "";
                                int O_WishListID = 0;
                                string strCallBodyText = "";
                                string strBasketBLL = "";

                                // ---------------------------------------
                                // DATA EXISTS FOR THIS ORDER ID
                                // ---------------------------------------
                                if (tblOrder.Rows.Count > 0)
                                {
                                    if (tblOrder.Rows(0)("O_Sent") == 0)
                                    {

                                        // Store order details
                                        O_CouponCode = FixNullFromDB(tblOrder.Rows(0)("O_CouponCode"));
                                        O_TotalPriceGateway = tblOrder.Rows(0)("O_TotalPriceGateway");
                                        O_PurchaseOrderNo = tblOrder.Rows(0)("O_PurchaseOrderNo");
                                        O_WishListID = tblOrder.Rows(0)("O_WishListID");
                                        strBasketBLL = tblOrder.Rows(0)("O_Status");

                                        // ---------------------------------------
                                        // FORMAT EMAIL TEXT
                                        // Mark offline orders clearly so they
                                        // are not mistaken for finished orders
                                        // where payment is already received and
                                        // goods need to be dispatched
                                        // ---------------------------------------
                                        if (clsPlugin.GatewayName.ToLower == "po_offlinepayment")
                                        {
                                            string strPOline = "";

                                            strPOline += GetGlobalResourceObject("Invoice", "ContentText_PONumber") + ": " + O_PurchaseOrderNo + Constants.vbCrLf;
                                            strPOline += Constants.vbCrLf;

                                            if (blnUseHTMLOrderEmail)
                                            {
                                                strCallBodyText = tblOrder.Rows(0)("O_Details");
                                                strCallBodyText = strCallBodyText.Replace("[poofflinepaymentdetails]", strPOline.Replace(Constants.vbCrLf, "<br />"));
                                            }
                                            else
                                            {
                                                strCallBodyText = strPOline + tblOrder.Rows(0)("O_Details");
                                            }

                                            try
                                            {
                                                // Mailchimp, try to remove cart
                                                if (blnMailChimp)
                                                {
                                                    // Mailchimp library
                                                    var mailChimpLib = new MailChimpBLL(CurrentLoggedUser, objBasket, CurrenciesBLL.CurrencyCode(Session("CUR_ID")));

                                                    string cartId = "cart_" + O_ID.ToString();
                                                    if (cartId is not null)
                                                    {
                                                        // Removing Cart and adding Order to successful payment made with Braintree
                                                        MailChimp.Net.Models.Customer mcCustomer = mailChimpLib.GetCustomer(CurrentLoggedUser.ID).Result;
                                                        Order mcOrder = mailChimpLib.AddOrder(mcCustomer, cartId).Result;
                                                        bool mcDeleteCart = mailChimpLib.DeleteCart(cartId).Result;

                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Debug.Print(ex.Message);
                                            }
                                        }

                                        else
                                        {
                                            strCallBodyText = tblOrder.Rows(0)("O_Details");
                                            strCallBodyText = strCallBodyText.Replace("[poofflinepaymentdetails]", "");
                                        }

                                        if (clsPlugin.GatewayName.ToLower == "bitcoin")
                                        {
                                            string strBitcoinline = "";

                                            strBitcoinline += GetGlobalResourceObject("Kartris", "ContentText_BitcoinPaymentDetails").ToString.Replace("[bitcoinpaymentaddress]", strBitcoinPaymentAddress);
                                            strBitcoinline += Constants.vbCrLf;

                                            if (blnUseHTMLOrderEmail)
                                            {
                                                strCallBodyText = strCallBodyText.Replace("[bitcoinpaymentdetails]", strBitcoinline);
                                            }
                                            else
                                            {
                                                strCallBodyText += strBitcoinline.Replace("<br/>", Constants.vbCrLf) + strCallBodyText;
                                            }
                                        }

                                        else
                                        {
                                            strCallBodyText = strCallBodyText.Replace("[bitcoinpaymentdetails]", "");
                                        }
                                        // ---------------------------------------
                                        // DETERMINE BEHAVIOUR OF STATUS BOXES
                                        // There are config settings to determine
                                        // whether the 'invoiced' and 'payment
                                        // received' status checkboxes on each
                                        // order are checked when a successful
                                        // payment is received
                                        // ---------------------------------------
                                        bool blnCheckInvoicedOnPayment = KartSettingsManager.GetKartConfig("frontend.orders.checkinvoicedonpayment") == "y";
                                        bool blnCheckReceivedOnPayment = KartSettingsManager.GetKartConfig("frontend.orders.checkreceivedonpayment") == "y";
                                        bool blnCheckSent = true;

                                        // ---------------------------------------
                                        // SET ORDER TIME
                                        // It uses the offset config setting in
                                        // case the server your site runs on is
                                        // not in your time zone
                                        // ---------------------------------------
                                        string strOrderTimeText = GetGlobalResourceObject("Email", "EmailText_OrderTime") + " " + CkartrisDisplayFunctions.NowOffset;
                                        if (clsPlugin.GatewayName.ToLower == "po_offlinepayment" | clsPlugin.GatewayName.ToLower == "bitcoin" | clsPlugin.GatewayName.ToLower == "easypaymultibanco")
                                        {
                                            blnCheckReceivedOnPayment = false;
                                            blnCheckInvoicedOnPayment = false;
                                        }

                                        if (clsPlugin.GatewayName.ToLower == "easypaymultibanco")
                                        {
                                            blnCheckSent = false;
                                        }

                                        // ---------------------------------------
                                        // UPDATE THE ORDER RECORD
                                        // ---------------------------------------
                                        string referenceCode = "";
                                        if (clsPlugin.GatewayName.ToLower == "braintreepayment")
                                        {
                                            referenceCode = transactionId;
                                        }
                                        else
                                        {
                                            referenceCode = clsPlugin.CallbackReferenceCode;
                                        }

                                        int intUpdateResult = objOrdersBLL.CallbackUpdate(O_ID, referenceCode, CkartrisDisplayFunctions.NowOffset, blnCheckSent, blnCheckInvoicedOnPayment, blnCheckReceivedOnPayment, strOrderTimeText, O_CouponCode, O_WishListID, 0, 0, "", 0);
                                        // If intUpdateResult = O_ID Then
                                        string strCustomerEmailText = "";
                                        string strStoreEmailText = "";

                                        strCallBodyText = strCallBodyText.Replace("[orderid]", O_ID.ToString());

                                        // ---------------------------------------
                                        // FORMAT CUSTOMER EMAIL TEXT
                                        // ---------------------------------------
                                        if (KartSettingsManager.GetKartConfig("frontend.checkout.ordertracking") != "n")
                                        {
                                            // Add order tracking information at the top
                                            if (!blnUseHTMLOrderEmail)
                                            {
                                                strCustomerEmailText = GetGlobalResourceObject("Email", "EmailText_OrderLookup") + Constants.vbCrLf + Constants.vbCrLf + WebShopURL() + "Customer.aspx" + Constants.vbCrLf + Constants.vbCrLf;
                                            }
                                        }
                                        // Don't need order tracking info
                                        strCustomerEmailText += strCallBodyText;

                                        // Add in email header above that
                                        if (!blnUseHTMLOrderEmail)
                                        {
                                            strCustomerEmailText = GetGlobalResourceObject("Email", "EmailText_OrderReceived") + Constants.vbCrLf + Constants.vbCrLf + GetGlobalResourceObject("Kartris", "ContentText_OrderNumber") + ": " + O_ID + Constants.vbCrLf + Constants.vbCrLf + strCustomerEmailText;
                                        }
                                        else
                                        {
                                            strCustomerEmailText = strCustomerEmailText.Replace("[storeowneremailheader]", "");
                                        }


                                        // ---------------------------------------
                                        // SEND CUSTOMER EMAIL
                                        // ---------------------------------------
                                        if (KartSettingsManager.GetKartConfig("frontend.orders.emailcustomer") != "n")
                                        {
                                            SendEmail(strFromEmail, UC_KartrisLogin.UserEmailAddress, GetGlobalResourceObject("Email", "Config_Subjectline") + " (#" + O_ID + ")", strCustomerEmailText, default, default, default, default, blnUseHTMLOrderEmail);
                                        }

                                        // ---------------------------------------
                                        // SEND STORE OWNER EMAIL
                                        // ---------------------------------------
                                        if (KartSettingsManager.GetKartConfig("frontend.orders.emailmerchant") != "n")
                                        {
                                            strStoreEmailText = GetGlobalResourceObject("Email", "EmailText_StoreEmailHeader") + Constants.vbCrLf + Constants.vbCrLf;
                                            if (!blnUseHTMLOrderEmail)
                                            {
                                                strStoreEmailText += GetGlobalResourceObject("Kartris", "ContentText_OrderNumber") + ": " + O_ID + Constants.vbCrLf + strCallBodyText;
                                            }
                                            else
                                            {
                                                strStoreEmailText = strCallBodyText.Replace("[storeowneremailheader]", strStoreEmailText);
                                            }
                                            SendEmail(strFromEmail, LanguagesBLL.GetEmailTo(1), GetGlobalResourceObject("Email", "Config_Subjectline2") + " (#" + O_ID + ")", strStoreEmailText, default, default, default, default, blnUseHTMLOrderEmail);
                                        }

                                        // Send an order notification to Windows Store App if enabled
                                        PushKartrisNotification("o");
                                    }
                                }

                                if (clsPlugin.GatewayName.ToLower != "easypaymultibanco")
                                {

                                    // ---------------------------------------

                                    // ORDER UPDATED
                                    // Clear object, transfer to the 
                                    // CheckoutComplete.aspx page
                                    // ---------------------------------------
                                    // Dim BasketObject As Kartris.Basket = New Kartris.Basket


                                    BasketBLL.DeleteBasket();
                                    Session("Basket") = null;
                                    Session("OrderDetails") = strCallBodyText;
                                    Session("OrderID") = O_ID;
                                    Session("_NewPassword") = null;
                                    Session("objOrder") = null;
                                    Server.Transfer("CheckoutComplete.aspx");
                                }
                                else
                                {
                                    Session("GateWayName") = clsPlugin.GatewayName;
                                    Session("_PostBackURL") = "";
                                    Session("EasypayPayment") = strEasypayPayment;
                                    Server.Transfer("VisaDetail.aspx?g=easypay&a=mbrefer");

                                }
                            }
                            else
                            {
                                // ---------------------------------------
                                // REDIRECT TO PAYPAL OR 3D-SECURE
                                // ---------------------------------------
                                string strPostBackURL = clsPlugin.PostbackURL;
                                string strCallbackMessage = clsPlugin.CallbackMessage;
                                clsPlugin = default;
                                Session("_NewPassword") = null;

                                if (string.IsNullOrEmpty(strCallbackMessage))
                                {
                                    Response.Redirect(strPostBackURL);
                                }
                                else
                                {
                                    Session("_CallbackMessage") = strCallbackMessage;
                                    Session("_PostBackURL") = strPostBackURL;
                                    Server.Transfer("CheckoutProcess.aspx", true);
                                }

                            }
                        }

                        else
                        {
                            // ---------------------------------------
                            // ERROR BACK FROM CREDIT CARD GATEWAY
                            // Show error in popup
                            // ---------------------------------------
                            if (clsPlugin.GatewayName.ToLower != "braintreepayment")
                            {
                                UC_PopUpErrors.SetTextMessage = clsPlugin.CallbackMessage;
                            }
                            UC_PopUpErrors.SetTitle = GetGlobalResourceObject("Kartris", "ContentText_CorrectErrors");
                            UC_PopUpErrors.ShowPopup();
                            mvwCheckout.ActiveViewIndex = 1;
                        }

                        clsPlugin = default;
                    }
                    else
                    {
                        // ---------------------------------------
                        // REMOTE PAYMENT PROCESS
                        // ---------------------------------------
                        Session("BasketObject") = objBasket;
                        Session("GatewayName") = strGatewayName;
                        Session("_NewPassword") = null;
                        clsPlugin = default;

                        // Try to delete AUTOSAVE basket
                        var objBasketBLL = new BasketBLL();
                        try
                        {
                            objBasketBLL.DeleteSavedBasketByNameAndUserID(objOrder.CustomerID, "AUTOSAVE");
                        }
                        catch (Exception ex)
                        {

                        }

                        // ---------------------------------------
                        // FORMAT FORM TO POST TO REMOTE GATEWAY
                        // ---------------------------------------
                        Server.Transfer("CheckoutProcess.aspx", true);
                    }
                }
            }


        }

    }

    /// <summary>
    /// Back button click
    /// </summary>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        btnBack.Visible = false;
        mvwCheckout.ActiveViewIndex = 1;
    }


    /// <summary>
    /// Post back automatically and refresh
    /// when EU VAT number is entered
    /// </summary>
    protected void txtEUVAT_AutoPostback()
    {

        // ---------------------------------------
        // VAT NUMBER WAS SUBMITTED
        // ---------------------------------------
        string strEUVatNumber = txtEUVAT.Text;
        var strThisUsersCountryCode = litMSCode.Text;
        if (!string.IsNullOrEmpty(txtEUVAT.Text))
        {

            // Even though we show the country code part outside
            // the text field, some users enter it into the text
            // field too. Rather than cause an error, we want to
            // just check if they do this and then remove it.
            if (Strings.Left(strEUVatNumber, 2).ToUpper() == strThisUsersCountryCode.ToUpper)
            {
                txtEUVAT.Text = Strings.Replace(strEUVatNumber, strThisUsersCountryCode, "");
            }

            // ---------------------------------------
            // We use the official EU web service
            // to validate EU VAT numbers, but can
            // fall back on simpler function if the
            // web service is unreachable or has
            // some other issue
            // ---------------------------------------
            bool blnValid = true;
            DateTime datCurrent;
            string strName = string.Empty;
            string strAddress = string.Empty;
            if (GetKartConfig("general.tax.euvatnumbercheck") == "y")
            {
                try
                {
                    // Try to use web service
                    var svcEUVAT = new eu.europa.ec.checkVatService();
                    datCurrent = svcEUVAT.checkVat(litMSCode.Text, txtEUVAT.Text, blnValid, strName, strAddress);
                    Session("blnEUVATValidated") = blnValid;
                }
                catch (Exception ex)
                {
                    // If web service is unavailable, we fall back
                    // to our CheckVATNumber function, which just
                    // checks the format of the submitted number
                    // against the formats each EU member country
                    // uses for its VAT numbers
                    Session("blnEUVATValidated") = CheckVATNumber(litMSCode.Text, litMSCode.Text + txtEUVAT.Text);
                }
            }
            else
            {
                Session("blnEUVATValidated") = true;
            }
        }
        else
        {

            // No VAT number submitted, so
            // not validated
            // Reset everything
            txtEUVAT.Text = "";
            Session("blnEUVATValidated") = false;
        }

        if (Session("blnEUVATValidated") == true)
        {
            // Show VAT number as valid
            // is blank, hide validation info
            litValidationOfVATNumber.Text = "&#x2705;";
            litValidationOfVATNumber.Visible = true;
        }
        else if (string.IsNullOrEmpty(strEUVatNumber))
        {
            // is blank, hide validation info
            litValidationOfVATNumber.Text = "";
            litValidationOfVATNumber.Visible = false;
        }
        else
        {
            // really is invalid
            litValidationOfVATNumber.Text = "&#x274C;";
            litValidationOfVATNumber.Visible = true;
            // Show VAT number as invalid
        }

        // UC_BasketView.RefreshShippingMethods()

    }

    /// <summary>
    /// This function was updated 21 Oct 2010
    /// by Paul, reflects latest specs obtained
    /// from HMRC web site.
    /// </summary>
    private bool CheckVATNumber(string strISOCode, string strVatNumber)
    {
        strVatNumber = Strings.Replace(Strings.Replace(strVatNumber, " ", ""), "-", "");
        strVatNumber = Strings.Replace(Strings.Replace(strVatNumber, ".", ""), ",", "");
        string strCountryCodeFromVatNumber = Strings.Left(strVatNumber, 2);
        int numVatNumberLength = Strings.Len(strVatNumber) - 2;
        bool blnPassed = false;
        switch (strISOCode ?? "")
        {
            case "AT": // Austria
                {
                    blnPassed = numVatNumberLength == 9 & Strings.UCase(Strings.Mid(strVatNumber, 3, 1)) == "U" & Information.IsNumeric(Strings.Right(strVatNumber, 8)) & (strCountryCodeFromVatNumber ?? "") == (strISOCode ?? "");
                    break;
                }

            case "BE":
            case "BG": // Belgium, Bulgaria
                {
                    blnPassed = (numVatNumberLength == 9 | numVatNumberLength == 10) & Information.IsNumeric(Strings.Right(strVatNumber, 9)) & (strCountryCodeFromVatNumber ?? "") == (strISOCode ?? "");
                    break;
                }

            case "PT":
            case "DE":
            case "EE": // Portugal, Germany, Estonia
                {
                    blnPassed = numVatNumberLength == 9 & Information.IsNumeric(Strings.Right(strVatNumber, 9)) & (strCountryCodeFromVatNumber ?? "") == (strISOCode ?? "");
                    break;
                }

            case "DK":
            case "FI":
            case "LU":
            case "MT":
            case "HU":
            case "SI": // Denmark, Finland, Luxembourg, Malta, Hungary, Slovenia
                {
                    blnPassed = numVatNumberLength == 8 & Information.IsNumeric(Strings.Right(strVatNumber, 8)) & (strCountryCodeFromVatNumber ?? "") == (strISOCode ?? "");
                    break;
                }

            case "FR": // France
                {
                    blnPassed = numVatNumberLength == 11 & Information.IsNumeric(Strings.Right(strVatNumber, 9)) & Strings.InStr(strVatNumber, "O") == 0 & Strings.InStr(strVatNumber, "I") == 0 & (strCountryCodeFromVatNumber ?? "") == (strISOCode ?? "");
                    break;
                }

            case "GR": // Greece
                {
                    blnPassed = numVatNumberLength == 9 & Information.IsNumeric(Strings.Right(strVatNumber, 9)) & strCountryCodeFromVatNumber == "EL";
                    break;
                }

            case "IE": // Ireland
                {
                    blnPassed = numVatNumberLength == 8 & Information.IsNumeric(Strings.Mid(strVatNumber, 5, 4)) & !Information.IsNumeric(Strings.Right(strVatNumber, 1)) & (strCountryCodeFromVatNumber ?? "") == (strISOCode ?? "");
                    break;
                }

            case "IT":
            case "LV":
            case "HR": // Italy, Latvia, Croatia
                {
                    blnPassed = numVatNumberLength == 11 & Information.IsNumeric(Strings.Right(strVatNumber, 11)) & (strCountryCodeFromVatNumber ?? "") == (strISOCode ?? "");
                    break;
                }

            case "NL": // Netherlands
                {
                    blnPassed = numVatNumberLength == 12 & Information.IsNumeric(Strings.Right(strVatNumber, 2)) & Information.IsNumeric(Strings.Mid(strVatNumber, 3, 8)) & Strings.Mid(strVatNumber, 12, 1) == "B" & (strCountryCodeFromVatNumber ?? "") == (strISOCode ?? "");
                    break;
                }

            case "ES": // Spain
                {
                    blnPassed = numVatNumberLength == 9 & Information.IsNumeric(Strings.Mid(strVatNumber, 4, 7)) & (strCountryCodeFromVatNumber ?? "") == (strISOCode ?? "");
                    break;
                }

            case "SE": // Sweden
                {
                    blnPassed = numVatNumberLength == 12 & Information.IsNumeric(Strings.Right(strVatNumber, 12)) & (strCountryCodeFromVatNumber ?? "") == (strISOCode ?? "");
                    break;
                }

            case "GB":
            case "UK":
            case "LT": // United Kingdom, Lithuania
                {
                    blnPassed = (numVatNumberLength == 12 | numVatNumberLength == 9) & Information.IsNumeric(Strings.Right(strVatNumber, 9)) & (strCountryCodeFromVatNumber == "GB" | strCountryCodeFromVatNumber == "LT");
                    break;
                }

            case "CY": // Cyprus
                {
                    blnPassed = numVatNumberLength == 9 & !Information.IsNumeric(Strings.Right(strVatNumber, 1)) & (strCountryCodeFromVatNumber ?? "") == (strISOCode ?? "");
                    break;
                }

            case "CZ": // Czech Republic
                {
                    if (numVatNumberLength >= 11 & numVatNumberLength <= 13)
                    {
                        strVatNumber = Strings.Mid(strVatNumber, 4);
                        numVatNumberLength = Strings.Len(strVatNumber);
                    }
                    blnPassed = numVatNumberLength >= 8 & numVatNumberLength <= 10 & Information.IsNumeric(Strings.Right(strVatNumber, 8)) & (strCountryCodeFromVatNumber == "CS" | strCountryCodeFromVatNumber == "CZ");
                    break;
                }

            case "PL":
            case "SK": // Poland, Slovak Republic
                {
                    blnPassed = numVatNumberLength == 10 & Information.IsNumeric(Strings.Right(strVatNumber, 10)) & (strCountryCodeFromVatNumber ?? "") == (strISOCode ?? "");       // ISO not recognised as in the EU - fail
                    break;
                }

            default:
                {
                    blnPassed = false;
                    break;
                }

        }

        return blnPassed;
    }
}


