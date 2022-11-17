using System;
using System.Collections;
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
using CkartrisBLL;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

internal partial class CustomerAffiliates : PageBaseClass
{

    protected double AFF_SalesTotal;
    protected double AFF_HitsTotal;
    private int UnpaidCount, PaymentCount, SalesLinkCount;
    private double UnpaidTotal, PaymentTotal;
    private CurrenciesBLL objCurrency = new CurrenciesBLL();

    private partial class AffiliateActivity
    {

        private double _Share;
        private int _ActivityMonth;
        private int _ActivityYear;
        private int _GraphValue;

        public double Share
        {
            get
            {
                return _Share;
            }
            set
            {
                _Share = value;
            }
        }

        public int ActivityMonth
        {
            get
            {
                return _ActivityMonth;
            }
            set
            {
                _ActivityMonth = value;
            }
        }

        public int ActivityYear
        {
            get
            {
                return _ActivityYear;
            }
            set
            {
                _ActivityYear = value;
            }
        }

        public int GraphValue
        {
            get
            {
                return _GraphValue;
            }
            set
            {
                _GraphValue = value;
            }
        }

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string strCallMode;
        var numCustomerID = default(int);

        if (!User.Identity.IsAuthenticated)
        {
            Response.Redirect("~/CustomerAccount.aspx");
        }
        else
        {
            numCustomerID = CurrentLoggedUser.ID;
        }

        strCallMode = Request.QueryString("activity") + "";

        switch (Strings.LCase(strCallMode) ?? "")
        {
            case "monthly":
                {
                    phdMonthly.Visible = true;
                    phdApply.Visible = false;
                    phdBalance.Visible = false;
                    phdActivity.Visible = false;

                    int numMonth, numYear;
                    numMonth = Val(Request.QueryString("Month"));
                    numYear = Val(Request.QueryString("Year"));

                    if (numMonth < 1 && numMonth > 12)
                        numMonth = 0;

                    if (numMonth == 0 | numYear == 0)
                    {
                        numMonth = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(CkartrisDisplayFunctions.NowOffset);
                        numYear = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(CkartrisDisplayFunctions.NowOffset);
                    }
                    else if (DateDiff("m", numYear + "/" + numMonth + "/1", System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(CkartrisDisplayFunctions.NowOffset) + "/" + System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(CkartrisDisplayFunctions.NowOffset) + "/1") < 0L | DateDiff("m", numYear + "/" + numMonth + "/1", DateAdd("m", -11, System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(CkartrisDisplayFunctions.NowOffset) + "/" + System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(CkartrisDisplayFunctions.NowOffset) + "/1")) > 0L)
                    {
                        numMonth = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(CkartrisDisplayFunctions.NowOffset);
                        numYear = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(CkartrisDisplayFunctions.NowOffset);
                    }

                    litAffMonthly_Date.Text = DateAndTime.MonthName(numMonth) + " " + numYear;

                    var objCurrency = new CurrenciesBLL();
                    double numTotalPrice = default, numTotalCommission = default, numTotalHits = default;
                    System.Data.DataTable dtbCommission;
                    var Basket = new kartris.Basket();

                    dtbCommission = AffiliateBLL.GetCustomerAffiliateCommission(numCustomerID, numMonth, numYear);
                    if (dtbCommission.Rows.Count > 0)
                    {
                        numTotalPrice = Conversions.ToDouble(dtbCommission.Rows[0]["OrderTotal"]);
                        numTotalCommission = Conversions.ToDouble(dtbCommission.Rows[0]["Commission"]);
                        numTotalHits = Conversions.ToDouble(dtbCommission.Rows[0]["Hits"]);
                    }
                    litAffMonthly_TotalPrice.Text = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency(), numTotalPrice);
                    litAffMonthly_TotalCommission.Text = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency(), numTotalCommission);
                    litAffMonthly_TotalHits.Text = numTotalHits;

                    dtbCommission = AffiliateBLL.GetCustomerAffiliateSalesLink(numCustomerID, numMonth, numYear);

                    SalesLinkCount = 0;
                    rptAffiliateSalesLink.DataSource = dtbCommission;
                    rptAffiliateSalesLink.DataBind();
                    break;
                }


            case "apply":
                {
                    phdMonthly.Visible = false;
                    phdApply.Visible = true;
                    phdBalance.Visible = false;
                    phdActivity.Visible = false;
                    var OldBasketBLL = new kartris.Basket();
                    AffiliateBLL.UpdateCustomerAffiliateStatus(numCustomerID);
                    break;
                }

            case "balance":
                {
                    phdMonthly.Visible = false;
                    phdApply.Visible = false;
                    phdBalance.Visible = true;
                    phdActivity.Visible = false;

                    var dtbPayments = new System.Data.DataTable();
                    var Basket = new kartris.Basket();

                    // '// payments made
                    dtbPayments = AffiliateBLL.GetCustomerAffiliatePayments(numCustomerID);

                    PaymentCount = 0;
                    PaymentTotal = 0d;
                    rptAffPayments.DataSource = dtbPayments;
                    rptAffPayments.DataBind();

                    // '// unpaid sales
                    dtbPayments = AffiliateBLL.GetCustomerAffiliateUnpaidSales(numCustomerID);

                    UnpaidCount = 0;
                    UnpaidTotal = 0d;
                    rptAffiliateUnpaid.DataSource = dtbPayments;
                    rptAffiliateUnpaid.DataBind(); // 'activity
                    break;
                }

            default:
                {
                    phdMonthly.Visible = false;
                    phdApply.Visible = false;
                    phdBalance.Visible = false;
                    phdActivity.Visible = true;

                    ArrayList aryAffiliateSales = new ArrayList(), aryAffiliateHits = new ArrayList();
                    var dtbActivity = new System.Data.DataTable();
                    var Basket = new kartris.Basket();
                    DateTime ActivityDate = CkartrisDisplayFunctions.NowOffset;
                    int numMaxValue;

                    aryAffiliateSales.Clear();
                    for (int i = 1; i <= 12; i++)
                    {
                        var objAff_Activity = new AffiliateActivity();
                        objAff_Activity.ActivityMonth = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(ActivityDate);
                        objAff_Activity.ActivityYear = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(ActivityDate);
                        objAff_Activity.Share = 0d;
                        objAff_Activity.GraphValue = 1;
                        aryAffiliateSales.Add(objAff_Activity);
                        ActivityDate = DateAndTime.DateAdd(DateInterval.Month, -1, ActivityDate);
                    }

                    numMaxValue = 0;
                    dtbActivity = AffiliateBLL.GetCustomerAffiliateActivitySales(numCustomerID);
                    for (int i = 1, loopTo = dtbActivity.Rows.Count; i <= loopTo; i++)
                    {
                        foreach (AffiliateActivity objItem in aryAffiliateSales)
                        {
                            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(dtbActivity.Rows[i - 1]["TheMonth"], objItem.ActivityMonth, false)) && Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(dtbActivity.Rows[i - 1]["TheYear"], objItem.ActivityYear, false)))
                            {
                                objItem.Share = Conversions.ToDouble(dtbActivity.Rows[i - 1]["OrderAmount"]);
                                if (objItem.Share > numMaxValue)
                                    numMaxValue = (int)Math.Round(objItem.Share);
                            }
                        }
                    }

                    AFF_SalesTotal = 0d;
                    foreach (AffiliateActivity objItem in aryAffiliateSales)
                    {
                        if (numMaxValue > 0)
                        {
                            objItem.GraphValue = (int)Math.Round(objItem.Share / numMaxValue * 100d);
                        }
                        AFF_SalesTotal = AFF_SalesTotal + objItem.Share;
                    }

                    rptAffiliateActivitySales.DataSource = aryAffiliateSales;
                    rptAffiliateActivitySales.DataBind();

                    ActivityDate = CkartrisDisplayFunctions.NowOffset;
                    // '// activity hits
                    aryAffiliateHits.Clear();
                    for (int i = 1; i <= 12; i++)
                    {
                        var objAff_Activity = new AffiliateActivity();
                        objAff_Activity.ActivityMonth = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(ActivityDate);
                        objAff_Activity.ActivityYear = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(ActivityDate);
                        objAff_Activity.Share = 0d;
                        objAff_Activity.GraphValue = 1;
                        aryAffiliateHits.Add(objAff_Activity);
                        ActivityDate = DateAndTime.DateAdd(DateInterval.Month, -1, ActivityDate);
                    }

                    numMaxValue = 0;
                    dtbActivity = AffiliateBLL.GetCustomerAffiliateActivityHits(numCustomerID);
                    for (int i = 1, loopTo1 = dtbActivity.Rows.Count; i <= loopTo1; i++)
                    {
                        foreach (AffiliateActivity objItem in aryAffiliateHits)
                        {
                            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(dtbActivity.Rows[i - 1]["TheMonth"], objItem.ActivityMonth, false)) && Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(dtbActivity.Rows[i - 1]["TheYear"], objItem.ActivityYear, false)))
                            {
                                objItem.Share = Conversions.ToDouble(Math.Round(dtbActivity.Rows[i - 1]["HitCount"]));
                                if (objItem.Share > numMaxValue)
                                    numMaxValue = (int)Math.Round(objItem.Share);
                            }
                        }
                    }

                    AFF_HitsTotal = 0d;
                    foreach (AffiliateActivity objItem in aryAffiliateHits)
                    {
                        if (numMaxValue > 0)
                        {
                            objItem.GraphValue = (int)Math.Round(objItem.Share / numMaxValue * 100d);
                        }
                        AFF_HitsTotal = AFF_HitsTotal + objItem.Share;
                    }

                    rptAffiliateActivityHits.DataSource = aryAffiliateHits;
                    rptAffiliateActivityHits.DataBind();
                    break;
                }

        }

    }

    protected void rptAffiliateActivitySales_ItemDataBound(object sender, Global.System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        var objCurrency = new CurrenciesBLL();
        double numShare;
        int numMonth, numYear;
        int numDefaultCurrencyID, numCurrencyID;
        AffiliateActivity objItem;

        numCurrencyID = Session("CUR_ID");
        numDefaultCurrencyID = CurrenciesBLL.GetDefaultCurrency();

        if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
        {
            objItem = (AffiliateActivity)e.Item.DataItem;
            numShare = objItem.Share;
            numMonth = objItem.ActivityMonth;
            numYear = objItem.ActivityYear;
            // Keep values in default currency, since that is what commission will be paid in
            // Converting to another currency will mislead people
            ((Literal)e.Item.FindControl("litSalesShare")).Text = CurrenciesBLL.FormatCurrencyPrice(numDefaultCurrencyID, numShare);
            ((HyperLink)e.Item.FindControl("hypLnkMonth")).Text = DateAndTime.MonthName(numMonth) + " " + numYear;
            ((HyperLink)e.Item.FindControl("hypLnkMonth")).NavigateUrl = "CustomerAffiliates.aspx?activity=monthly&month=" + numMonth + "&year=" + numYear;
        }

        if (e.Item.ItemType == ListItemType.Footer)
        {
            // Keep values in default currency, since that is what commission will be paid in
            // Converting to another currency will mislead people
            ((Literal)e.Item.FindControl("litSalesTotal")).Text = CurrenciesBLL.FormatCurrencyPrice(numDefaultCurrencyID, AFF_SalesTotal);
        }

    }

    protected void rptAffiliateActivityHits_ItemDataBound(object sender, Global.System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        double numShare;
        int numMonth, numYear;
        AffiliateActivity objItem;

        if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
        {
            objItem = (AffiliateActivity)e.Item.DataItem;
            numShare = objItem.Share;
            numMonth = objItem.ActivityMonth;
            numYear = objItem.ActivityYear;
            ((Literal)e.Item.FindControl("litHitsShare")).Text = numShare;
            ((HyperLink)e.Item.FindControl("hypLnkMonth")).Text = DateAndTime.MonthName(numMonth) + " " + numYear;
        }

        if (e.Item.ItemType == ListItemType.Footer)
        {
            ((Literal)e.Item.FindControl("litHitsTotal")).Text = AFF_HitsTotal;
        }

    }

    protected void rptAffiliateSalesLink_ItemDataBound(object sender, Global.System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        DateTime datValue;

        if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
        {
            SalesLinkCount = SalesLinkCount + 1;
            ((Literal)e.Item.FindControl("litSalesLinkCnt")).Text = SalesLinkCount;
            datValue = e.Item.DataItem("O_Date");
            ((Literal)e.Item.FindControl("litSalesLinkDate")).Text = CkartrisDisplayFunctions.FormatDate(datValue, "t", Session("LANG"));

            ((Literal)e.Item.FindControl("litSalesLinkValue")).Text = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency(), e.Item.DataItem("OrderTotal"));
            ((Literal)e.Item.FindControl("litSalesLinkCommission")).Text = e.Item.DataItem("O_AffiliatePercentage") + "%";
            ((Literal)e.Item.FindControl("litSalesLinkTotal")).Text = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency(), e.Item.DataItem("Commission"));
            if (e.Item.DataItem("O_AffiliatePaymentID") > 0)
            {
                ((Image)e.Item.FindControl("imgSalesLinkPaid")).ImageUrl = WebShopURL() + "images/tick.gif";
            }
            else
            {
                ((Image)e.Item.FindControl("imgSalesLinkPaid")).ImageUrl = WebShopURL() + "images/tick_empty.gif";
            }
        }

    }

    protected void rptAffPayments_ItemDataBound(object sender, Global.System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        DateTime datValue;

        if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
        {
            PaymentCount = PaymentCount + 1;
            datValue = e.Item.DataItem("AFP_DateTime");
            ((Literal)e.Item.FindControl("litPaymentDate")).Text = CkartrisDisplayFunctions.FormatDate(datValue, "t", Session("LANG"));
            ((Literal)e.Item.FindControl("litPaymentPayment")).Text = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency(), e.Item.DataItem("TotalPayment"));
            PaymentTotal = PaymentTotal + e.Item.DataItem("TotalPayment");
        }

        if (e.Item.ItemType == ListItemType.Footer)
        {
            ((Literal)e.Item.FindControl("litPaymentTotal")).Text = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency(), PaymentTotal);
        }

    }

    protected void rptAffiliateUnpaid_ItemDataBound(object sender, Global.System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        DateTime datValue;

        if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
        {
            UnpaidCount = UnpaidCount + 1;
            datValue = e.Item.DataItem("O_Date");
            ((Literal)e.Item.FindControl("litUnpaidDate")).Text = CkartrisDisplayFunctions.FormatDate(datValue, "t", Session("LANG"));

            ((Literal)e.Item.FindControl("litUnpaidPayment")).Text = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency(), e.Item.DataItem("Commission"));

            ((Literal)e.Item.FindControl("litUnpaidPaymentPercent")).Text = e.Item.DataItem("O_AffiliatePercentage") + "%";

            UnpaidTotal = UnpaidTotal + e.Item.DataItem("Commission");
        }

        if (e.Item.ItemType == ListItemType.Footer)
        {
            ((Literal)e.Item.FindControl("litUnpaidTotal")).Text = CurrenciesBLL.FormatCurrencyPrice(CurrenciesBLL.GetDefaultCurrency(), UnpaidTotal);
        }

    }

}