using System;
using System.Data;
using System.Web;
using System.Web.UI.HtmlControls;
using CkartrisDataManipulation;
using CkartrisDisplayFunctions;
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
using CkartrisEnumerations;
using CKartrisSearchManager;
using KartSettingsManager;
using Microsoft.VisualBasic;

namespace EmployeeManagementSystem
{

    partial class Knowledgebase : PageBaseClass
    {

        // We set a value to keep track of any trapped
        // error handled, this way, we can avoid throwing
        // a generic error on top of the handled one.
        private string strErrorThrown = "";

        public Knowledgebase()
        {
            this.Load += Page_Load;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            Page.Title = GetGlobalResourceObject("Knowledgebase", "PageTitle_Knowledgebase") + " | " + Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));

            if (KartSettingsManager.GetKartConfig("frontend.knowledgebase.enabled") == "y")
            {
                pnlSearch.Visible = true;

                if (Request.QueryString("list") == "all")
                {
                    GetKBList();
                }
                else if (Information.IsNumeric(Request.QueryString("kb")))
                {
                    GetKBArticle();
                }
                else if (Request.QueryString("strSearchText") != "")
                {
                    SearchKnowledgebase();
                }
                else
                {
                    // Er, just start page
                }
            }
            else
            {
                // do nothing
            }

        }

        public void GetKBList()
        {
            pnlSearch.Visible = false;
            DataTable tblKB = KBBLL.GetKB(Session("LANG"));
            rptKBList.DataSource = tblKB;
            rptKBList.DataBind();
            mvwKnowledgebase.SetActiveView(viwKBList);
        }

        public void GetKBArticle()
        {
            DataTable tblKB = KBBLL.GetKBByID(Session("LANG"), Request.QueryString("kb"));
            if (tblKB.Rows.Count == 0)
            {
                // An item was called with correctly formatted URL, but
                // the ID doesn't appear to pull out an item, so it's
                // likely the item is no longer available.
                strErrorThrown = "404";
                HttpContext.Current.Server.Transfer("~/404.aspx");
            }
            else
            {
                var drwKB = tblKB.Rows[0];
                if (FixNullFromDB(drwKB["KB_PageTitle"]) is not null)
                    this.Title = FixNullFromDB(drwKB["KB_PageTitle"]);
                litDateUpdated.Text = FormatDate(FixNullFromDB(drwKB["KB_LastUpdated"]), "d", Session("LANG"));
                litKBName.Text = FixNullFromDB(drwKB["KB_Name"]);
                litKBText.Text = FixNullFromDB(drwKB["KB_Text"]);
                litKBID.Text = FixNullFromDB(drwKB["KB_ID"]);

                string strPageMetaDesc = FixNullFromDB(drwKB["KB_MetaDescription"]);
                string strPageMetaKeywords = FixNullFromDB(drwKB["KB_MetaKeywords"]);

                if (!string.IsNullOrEmpty(this.Title))
                {
                    this.Title = CkartrisDisplayFunctions.StripHTML(this.Title) + " | " + Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));
                }

                var metaTag = new HtmlMeta();
                HtmlHead HeadTag = (HtmlHead)Page.Header;

                if (!string.IsNullOrEmpty(strPageMetaDesc))
                {
                    ((PageBaseClass)Page).MetaDescription = strPageMetaDesc;
                }

                if (!string.IsNullOrEmpty(strPageMetaKeywords))
                {
                    ((PageBaseClass)Page).MetaKeywords = strPageMetaKeywords;
                }

                mvwKnowledgebase.SetActiveView(viwKBDetails);
                pnlSearch.Visible = false;
            }
        }

        public string FormatLink(int KB_ID, string KB_Name)
        {
            return "[#" + KB_ID.ToString() + "] " + KB_Name;

        }

        public void SearchKnowledgebase()
        {
            string strSearchText = ValidateSearchKeywords(Request.QueryString("strSearchText"));

            if (!string.IsNullOrEmpty(strSearchText))
            {
                DataTable tblSearchResult = KBBLL.Search(strSearchText, Session("LANG"));

                foreach (DataRow drwResults in tblSearchResult.Rows)
                {
                    drwResults["KB_Name"] = HighLightResultText(FixNullFromDB(drwResults["KB_Name"]), strSearchText);
                    drwResults["KB_Text"] = HighLightResultText(FixNullFromDB(drwResults["KB_Text"]), strSearchText);
                }

                var strSearchSummaryTemplate = GetGlobalResourceObject("Search", "ContentText_SearchSummaryTemplate");

                strSearchSummaryTemplate = Strings.Replace(strSearchSummaryTemplate, "[searchterms]", strSearchText);
                strSearchSummaryTemplate = Strings.Replace(strSearchSummaryTemplate, "[matches]", tblSearchResult.Rows.Count);
                litSearchResult.Text = strSearchSummaryTemplate;

                rptSearchList.DataSource = tblSearchResult;
                rptSearchList.DataBind();

            }
            mvwKnowledgebase.SetActiveView(viwSearchResult);
            updMain.Update();
        }
    }
}