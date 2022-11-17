using System;
using System.Text;
// ========================================================================
// Copyright 2014 BORNXenon
// incorporated into Kartris with permission

// GNU GENERAL PUBLIC LICENSE v2
// This program is free software distributed under the GPL without any
// warranty.
// www.gnu.org/licenses/gpl-2.0.html
// ========================================================================
using System.Xml;
using CkartrisDisplayFunctions;

internal partial class rss : System.Web.UI.Page
{
    public rss()
    {
        this.Load += Page_Load;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        int intSizeOfFeed = 25;
        string strWebshopURL = CkartrisBLL.WebShopURLhttp;
        string strWebshopName = Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname"));
        int intLanguageID = CkartrisBLL.GetLanguageIDfromSession;
        string strSkinFolder = LanguagesBLL.GetTheme(intLanguageID);
        int intLanguage = Session("LANG");

        // If Session is empty, get default language
        if (intLanguage == 0)
        {
            intLanguage = KartSettingsManager.GetKartConfig("frontend.languages.default");
        }

        // Check for Empty Skin Folder
        if (string.IsNullOrEmpty(strSkinFolder))
        {
            strSkinFolder = "Kartris";
        }

        // Clear any previous output from the buffer
        Response.Clear();
        Response.ContentType = "application/xml";

        // XML Declaration Tag
        using (var xml = new XmlTextWriter(Response.OutputStream, Encoding.UTF8))
        {
            xml.WriteStartDocument();

            // RSS Tag
            xml.WriteStartElement("rss");
            xml.WriteAttributeString("version", "2.0");
            xml.WriteAttributeString("xmlns:atom", "http://www.w3.org/2005/Atom");

            // The Channel Tag - RSS Feed Details
            xml.WriteStartElement("channel");
            xml.WriteElementString("title", strWebshopName + " RSS Feed");
            xml.WriteElementString("link", strWebshopURL);
            xml.WriteElementString("description", "All the latest news, marketing and promotions from " + strWebshopName);
            xml.WriteElementString("copyright", "Copyright " + System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(DateTime.Now) + ". All rights reserved.");

            // Image
            xml.WriteStartElement("image");
            xml.WriteElementString("url", strWebshopURL + "Skins/" + strSkinFolder + "/Images/logo.png");
            xml.WriteElementString("title", strWebshopName + " RSS Feed");
            xml.WriteElementString("link", strWebshopURL);
            xml.WriteElementString("width", "130");
            xml.WriteElementString("height", "90");
            xml.WriteEndElement();

            // Atom Tag
            xml.WriteStartElement("atom:link");
            xml.WriteAttributeString("href", strWebshopURL + "rss.aspx");
            xml.WriteAttributeString("rel", "self");
            xml.WriteAttributeString("type", "application/rss+xml");
            xml.WriteEndElement();

            // Pull news records into a DataTable
            DataTable tblLatestNews = NewsBLL.GetLatestNews(intLanguage, intSizeOfFeed);

            // Loop through DataTable and write news items to xml output
            foreach (DataRow row in tblLatestNews.Rows)
            {
                xml.WriteStartElement("item");
                xml.WriteElementString("title", row("N_Name").ToString());
                xml.WriteElementString("description", CkartrisDisplayFunctions.TruncateDescription(Server.HtmlDecode(row("N_Text").ToString()), KartSettingsManager.GetKartConfig("frontend.news.display.truncatestory")) + " <a href=\"" + strWebshopURL + "News.aspx?NewsID=" + row("N_ID").ToString() + "\">[more]</a>");
                xml.WriteElementString("link", strWebshopURL + "News.aspx?NewsID=" + row("N_ID").ToString());
                xml.WriteElementString("guid", strWebshopURL + "News.aspx?NewsID=" + row("N_ID").ToString());
                xml.WriteElementString("pubDate", dateRFC822(row("N_DateCreated")));
                xml.WriteEndElement();
            }

            xml.WriteEndElement();
            xml.WriteEndElement();
            xml.WriteEndDocument();
            xml.Flush();
            xml.Close();
            Response.End();

        }
    }

    // Function converts date string to RFC822 format
    public string dateRFC822(DateTime date)
    {
        int offset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;
        string timeZone__1 = "+" + offset.ToString().PadLeft(2, '0');
        if (offset < 0)
        {
            int i = offset * -1;
            timeZone__1 = "-" + i.ToString().PadLeft(2, '0');
        }
        return date.ToString("ddd, dd MMM yyy HH:mm:ss " + timeZone__1.PadRight(5, '0'));
    }

}