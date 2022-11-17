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
using System.Linq;
using CkartrisDataManipulation;
using CkartrisDisplayFunctions;
using CkartrisEnumerations;
using KartSettingsManager;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic

internal partial class Search : PageBaseClass
{

    private const string c_PAGER_QUERY_STRING_KEY = "PPGR";

    public Search()
    {
        this.Load += Page_Load;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        Page.Title = GetGlobalResourceObject("Search", "PageTitle_ProductSearch") + " | " + Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));

        // Set the currency symbols to user's choice
        litPriceSymbol.Text = CurrenciesBLL.CurrencySymbol(Session("CUR_ID"));
        litPriceSymbol2.Text = CurrenciesBLL.CurrencySymbol(Session("CUR_ID"));

        // Collect values posted to page
        // With the search, we do things old style, accepting
        // all parameters as querystrings to the page. This
        // way it is possible to use remote search boxes that
        // submit to the search page, or send someone a link
        // in an email, for example, that runs a search.
        string strSearchText = Strings.Trim(Request.QueryString("strSearchText"));
        string strSearchMethod = Request.QueryString("strSearchMethod");
        string strType = Request.QueryString("strType");
        if (string.IsNullOrEmpty(strSearchMethod))
            strSearchMethod = "any"; // default to 'any' search if no method specified
        float numPriceFrom = 0.0f;
        float numPriceTo = 0.0f;

        // Numbers will cause errors if non-numeric values 
        // added, so we request these in a try/catch just in
        // case. Any errors, FROM price is treated as zero.
        try
        {
            numPriceFrom = Request.QueryString("numPriceFrom");
        }
        catch (Exception ex)
        {
            numPriceFrom = 0f;
        }
        // Any errors, TO price is treated as 10,000,000. Should
        // cover everything except
        try
        {
            numPriceTo = Request.QueryString("numPriceTo");
        }
        catch (Exception ex)
        {
            numPriceTo = 0f;
        }

        // If no searchtext value via querystring, we try to recover from cookie.
        // The cookie lets us store details so we can view products etc. and come
        // back to the search result we were looking at.
        string strSearchCookieName = HttpSecureCookie.GetCookieName("Search");
        if (Strings.Len(strSearchText) < 1 && !string.IsNullOrEmpty(Strings.Trim(Request.QueryString("strResults"))))
        {
            try
            {
                strSearchText = Request.Cookies(strSearchCookieName).Values("exactPhrase");
            }
            catch (Exception ex)
            {
                // if no cookie exists
            }

            // We need to strip the + signs that appear where
            // spaces were in the query due to collecting the
            // values via querystring
            strSearchText = Strings.Replace(strSearchText, "+", " ");

            try
            {
                strSearchMethod = Request.Cookies(strSearchCookieName).Values("searchMethod");
            }
            catch (Exception ex)
            {
                strSearchMethod = "any";
            }
            try
            {
                strType = Request.Cookies(strSearchCookieName).Values("type");
            }
            catch (Exception ex)
            {
                strType = "classic";
            }
            try
            {
                numPriceFrom = Request.Cookies(strSearchCookieName).Values("minPrice");
            }
            catch (Exception ex)
            {
                numPriceFrom = 0f;
            }
            try
            {
                numPriceTo = Request.Cookies(strSearchCookieName).Values("maxPrice");
            }
            catch (Exception ex)
            {
                numPriceTo = 0f;
            }
        }
        else
        {
            // This page got querystrings, so we're going to write these
            // to the search cookie so we have latest search there.
            if (Request.Cookies(strSearchCookieName) is null)
                CKartrisSearchManager.CreateSearchCookie();

            // Save the search values to a cookie
            CKartrisSearchManager.UpdateSearchCookie(StripHTML(strSearchText), SEARCH_TYPE.advanced, strSearchMethod, numPriceFrom, numPriceTo);
        }

        // Log the search term, for back end stats purposes
        if (!string.IsNullOrEmpty(strSearchText))
            StatisticsBLL.ReportSearchStatistics(strSearchText);

        // create comma separated list of keywords based on the strSearchText
        string strKeywords = Strings.Replace(strSearchText, " ", ",");
        while (Strings.InStr(strKeywords, ",,") != 0)
            strKeywords = Strings.Replace(strKeywords, ",,", ",");

        // Ok, let's run the search
        if (!string.IsNullOrEmpty(strSearchText))
        {
            GetSearchResult(strKeywords, strSearchText, strSearchMethod, numPriceFrom, numPriceTo);




            if (strType == "advanced")
            {
                tabSearchContainer.ActiveTab = tabSearch_Advanced;
                pnlAdvanced.Visible = true;
                pnlClassic.Visible = false;
            }
        }

    }

    // Run the search
    public void GetSearchResult(string strKeyWords, string strSearchText, string strSearchMethod, float numPriceFrom, float numPriceTo)



    {

        if (!string.IsNullOrEmpty(strKeyWords))
        {
            int numPageSize = GetKartConfig("frontend.search.pagesize");
            DataTable tblSearchResult;
            var numTotalProducts = default(int);

            // Gets the value of the Paging Key "PPGR" from the URL.
            var numPageIndex = default(short);
            try
            {
                if (Request.QueryString(c_PAGER_QUERY_STRING_KEY) is null)
                {
                    numPageIndex = 0;
                }
                else
                {
                    numPageIndex = Request.QueryString(c_PAGER_QUERY_STRING_KEY);
                }
            }
            catch (Exception ex)
            {
            }

            // We need to find customer group, as some items may not be
            // visible in search if customer is not allowed to view
            // certain products
            short numCGroupID = 0;
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                numCGroupID = ((PageBaseClass)Page).CurrentLoggedUser.CustomerGroupID;
            }

            // We need to convert values entered to default currency
            // if the user has selected another currency for use on
            // the site     
            if (Session("CUR_ID") != CurrenciesBLL.GetDefaultCurrency())
            {
                numPriceFrom = CurrenciesBLL.ConvertCurrency(Session("CUR_ID"), numPriceFrom, CurrenciesBLL.GetDefaultCurrency());
            }

            if (Session("CUR_ID") != CurrenciesBLL.GetDefaultCurrency())
            {
                numPriceTo = CurrenciesBLL.ConvertCurrency(Session("CUR_ID"), numPriceTo, CurrenciesBLL.GetDefaultCurrency());
            }



            tblSearchResult = KartrisDBBLL.GetSearchResult(strSearchText, strKeyWords, Session("LANG"), numPageIndex, numPageSize, numTotalProducts, numPriceFrom, numPriceTo, strSearchMethod, numCGroupID);



            // Write the search summary 'your search for XXX produced Y results'
            var strSearchSummaryTemplate = GetGlobalResourceObject("Search", "ContentText_SearchSummaryTemplate");
            if (!string.IsNullOrEmpty(strSearchText))
            {
                strSearchSummaryTemplate = Strings.Replace(strSearchSummaryTemplate, "[searchterms]", Server.HtmlEncode(strSearchText));
                strSearchSummaryTemplate = Strings.Replace(strSearchSummaryTemplate, "[matches]", numTotalProducts);
                litSearchResult.Text = strSearchSummaryTemplate;
                updSearchResultArea.Visible = true;
            }
            else
            {
                updSearchResultArea.Visible = true;
                litSearchResult.Text = GetGlobalResourceObject("Search", "ContentText_SearchNothing");
            }

            if (tblSearchResult is not null && tblSearchResult.Rows.Count != 0)
            {

                // If the total products couldn't be fitted in 1 Page, Then Initialize the Pager.
                if (numTotalProducts > numPageSize)
                {

                    // Load the Header & Footer Pagers
                    UC_ItemPager_Footer.LoadPager(numTotalProducts, numPageSize, c_PAGER_QUERY_STRING_KEY);
                    UC_ItemPager_Footer.DisableLink(numPageIndex);
                    UC_ItemPager_Footer.Visible = true;
                }

                for (int row = 0, loopTo = tblSearchResult.Rows.Count - 1; row <= loopTo; row++)
                {
                    tblSearchResult.Rows(row)("P_Desc") = CKartrisSearchManager.HighLightResultText(FixNullFromDB(tblSearchResult.Rows(row)("P_Desc")), strKeyWords);
                    tblSearchResult.Rows(row)("P_Name") = CKartrisSearchManager.HighLightResultText(Server.HtmlEncode(FixNullFromDB(tblSearchResult.Rows(row)("P_Name"))), strKeyWords);
                }

                UC_SearchResult.LoadSearchResult(tblSearchResult);
            }

        }
    }
}