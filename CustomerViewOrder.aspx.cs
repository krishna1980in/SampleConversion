using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
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
using Microsoft.VisualBasic.CompilerServices;

namespace EmployeeManagementSystem
{

    partial class Customer_ViewOrder : PageBaseClass
    {

        protected bool APP_PricesIncTax, APP_ShowTaxDisplay;
        private CurrenciesBLL objCurrency = new CurrenciesBLL();
        private kartris.Basket objBasket = new kartris.Basket();
        private double numTaxDue, numTotalPriceExTax, numTotalPriceIncTax, numCouponDiscount, numCustomerDiscount, numShipping, numOrderHandlingCharge, numTotal;
        private double numDiscountPercentage, numPromotionDiscountTotal, numCouponDiscountTotal, CP_DiscountValue;
        private string strCouponCode, CP_DiscountType, CP_CouponCode;
        private double numShippingPriceIncTax, numShippingPriceExTax, numOrderHandlingPriceIncTax, numOrderHandlingPriceExTax, numFinalTotalPriceInTaxGateway;
        private double numCurrencyIDGateway, numCurrencyID;

        public Customer_ViewOrder()
        {
            this.Load += Page_Load;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int numCustomerID = default, numOrderID;
            DataTable tblOrder;


            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/CustomerAccount.aspx");
            }
            else
            {
                numCustomerID = CurrentLoggedUser.ID;
            }

            if (ConfigurationManager.AppSettings["TaxRegime"].ToLower() == "us" | ConfigurationManager.AppSettings["TaxRegime"].ToLower() == "simple")
            {
                APP_PricesIncTax = false;
                APP_ShowTaxDisplay = false;
            }
            else
            {
                APP_PricesIncTax = GetKartConfig("general.tax.pricesinctax") == "y";
                APP_ShowTaxDisplay = GetKartConfig("frontend.display.showtax") == "y";
            }


            numOrderID = Val(Request.QueryString("O_ID"));

            UC_CustomerOrder.ShowOrderSummary = true;
            UC_CustomerOrder.OrderID = numOrderID;

            var objBasketBLL = new BasketBLL();
            tblOrder = objBasketBLL.GetCustomerOrderDetails(numOrderID);

            if (tblOrder.Rows.Count > 0)
            {
                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectNotEqual(tblOrder.Rows[0]["O_CustomerID"], numCustomerID, false)))
                {
                    phdOrderStatus.Visible = false;
                }
            }
            else
            {
                phdOrderStatus.Visible = false;
            }


        }

        public void Popup_Click(object Sender, CommandEventArgs E)
        {
            Response.Redirect("~/Customer.aspx");
        }


    }
}