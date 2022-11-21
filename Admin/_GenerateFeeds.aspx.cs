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
using Microsoft.VisualBasic;
using CkartrisDataManipulation;
using CkartrisDisplayFunctions;
using KartSettingsManager;
using FeedsBLL;

partial class Admin_GenerateFeeds : _PageBaseClass
{
    private static string strAppUploadsFolder = KartSettingsManager.GetKartConfig("general.uploadfolder");

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Feeds", "PageTitle_GenerateFeeds") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");
    }

    /// <summary>
    ///     ''' Create full friendly URL
    ///     ''' </summary>
    public string CreateFeedURL(string strCulture, string strTitle, string strType, string strItemID)
    {
        string strURL = "";

        // Formats the text/name part of the URL, replaces spaces with a 
        // dash and then strips or replaces disallowed URL chars
        strTitle = CleanURL(Strings.Replace(strTitle, " ", "-"));

        switch (strType)
        {
            case "p":
                {
                    // Product
                    strURL = CkartrisBLL.WebShopURL + strCulture + strTitle + "__p-" + strItemID + ".aspx";
                    break;
                }

            case "c":
                {
                    // Category
                    strURL = CkartrisBLL.WebShopURL + strCulture + strTitle + "__c-p-0-0-" + strItemID + ".aspx";
                    break;
                }

            case "t":
                {
                    // Custom page text
                    strURL = CkartrisBLL.WebShopURL + "t-" + strTitle + ".aspx";
                    break;
                }

            case "n":
                {
                    // News
                    strURL = CkartrisBLL.WebShopURL + strCulture + strTitle + "__n-" + strItemID + ".aspx";
                    break;
                }
        }

        return strURL;
    }

    /// <summary>
    ///     ''' Generate the Sitemap file
    ///     ''' </summary>
    protected void btnGenerate_Click(object sender, System.EventArgs e)
    {
        XmlTextWriter CurrentXmlSiteMap = CreateXMLSiteMap("sitemap.xml");

        List<string> lstAdded = new List<string>();
        List<string> lstAddedProducts = new List<string>();
        int intTotalURLs = 0;
        int intCurrentURLCounter = 0;
        string strLink = "";


        // Add nodes from the web.sitemap file
        foreach (SiteMapNode node in SiteMap.Providers("MenuSiteMap").RootNode.GetAllNodes)
        {
            if (!string.IsNullOrEmpty(node.Url))
            {
                string strURL = node.Url;
                if (Strings.Right(strURL, 1) == "?")
                    strURL = Strings.Left(strURL, strURL.Length - 1);
                strURL = FixURL(strURL);
                if (!lstAdded.Contains(strURL))
                {
                    AddURLElement(ref CurrentXmlSiteMap, strURL);
                    lstAdded.Add(strURL);
                    intTotalURLs += 1;
                }
            }
        }

        lstAdded.Clear();
        int intSiteMapCounter = 0;
        DataTable tblProducts = null/* TODO Change to default(_) if this is not a reference type */;

        DataTable dtbFeedData = _GetFeedData();

        foreach (var drwFeedData in dtbFeedData.Rows)
        {
            string strCulture = drwFeedData("LANG_Culture").ToString() + "/";
            if (strCulture == "/")
                strCulture = "";

            // Try/catch so one bad URL won't crash the whole thing
            try
            {
                AddURLElement(ref CurrentXmlSiteMap, CreateFeedURL(strCulture, drwFeedData("PAGE_Name").ToString(), drwFeedData("RecordType").ToString(), drwFeedData("ItemID").ToString()));

                if (intCurrentURLCounter == 49990)
                {
                    intCurrentURLCounter = 0;
                    intSiteMapCounter += 1;
                    CloseXMLSitemap(ref CurrentXmlSiteMap);
                    CurrentXmlSiteMap = CreateXMLSiteMap("sitemap" + intSiteMapCounter + ".xml");
                }
                intCurrentURLCounter += 1;
            }
            catch (Exception ex)
            {
            }
        }

        CloseXMLSitemap(ref CurrentXmlSiteMap);

        // create a sitemap index if multiple files were generated
        if (intSiteMapCounter > 0)
        {
            XmlTextWriter xmlSiteMap = new XmlTextWriter(Path.Combine(Request.PhysicalApplicationPath, "xmlsitemapindex.xml"), System.Text.Encoding.UTF8);
            {
                var withBlock = xmlSiteMap;
                withBlock.WriteStartDocument();
                withBlock.WriteWhitespace(Constants.vbCrLf);
                withBlock.WriteStartElement("sitemapindex");
                withBlock.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

                withBlock.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                withBlock.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/siteindex.xsd");


                withBlock.WriteWhitespace(Constants.vbCrLf);
                withBlock.WriteStartElement("sitemap");
                withBlock.WriteWhitespace(Constants.vbCrLf);
                withBlock.WriteElementString("loc", CkartrisBLL.WebShopURLhttp + "sitemap.xml");
                withBlock.WriteWhitespace(Constants.vbCrLf);
                withBlock.WriteEndElement();

                for (int i = 1; i <= intSiteMapCounter; i++)
                {
                    withBlock.WriteWhitespace(Constants.vbCrLf);
                    withBlock.WriteStartElement("sitemap");
                    withBlock.WriteWhitespace(Constants.vbCrLf);
                    withBlock.WriteElementString("loc", CkartrisBLL.WebShopURLhttp + "sitemap" + i + ".xml");
                    withBlock.WriteWhitespace(Constants.vbCrLf);
                    withBlock.WriteEndElement();
                }
                withBlock.Flush();
                withBlock.Close();
            }

            // We have a sitemap index file, so link to that
            lnkGenerated.NavigateUrl = CkartrisBLL.WebShopURLhttp + "xmlsitemapindex.xml";
            litFilePath.Text = CkartrisBLL.WebShopURLhttp + "xmlsitemapindex.xml";
        }
        else
        {
            // Just one sitemap, link to that
            lnkGenerated.NavigateUrl = CkartrisBLL.WebShopURLhttp + "sitemap.xml";
            litFilePath.Text = CkartrisBLL.WebShopURLhttp + "sitemap.xml";
        }

        // Show link to file
        lnkGenerated.Visible = true;
        litFilePath.Visible = true;

        // Show update animation
        ShowMasterUpdateMessage();
    }

    /// <summary>
    ///     ''' Generate start lines of sitemap.xml file
    ///     ''' </summary>
    private XmlTextWriter CreateXMLSiteMap(string strFileName)
    {
        XmlTextWriter xmlSiteMap = new XmlTextWriter(Server.MapPath("~/") + strFileName, System.Text.Encoding.UTF8);
        {
            var withBlock = xmlSiteMap;
            withBlock.WriteStartDocument();
            withBlock.WriteWhitespace(Constants.vbCrLf);
            withBlock.WriteStartElement("urlset");
            withBlock.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

            withBlock.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            withBlock.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/siteindex.xsd");
        }

        return xmlSiteMap;
    }

    /// <summary>
    ///     ''' Generate closing lines of sitemap.xml file
    ///     ''' </summary>
    private void CloseXMLSitemap(ref XmlTextWriter xmlSiteMap)
    {
        {
            var withBlock = xmlSiteMap;
            withBlock.WriteWhitespace(Constants.vbCrLf);
            withBlock.WriteEndElement();
            withBlock.WriteWhitespace(Constants.vbCrLf);
            withBlock.WriteEndDocument();
            withBlock.Flush();
            withBlock.Close();
        }
    }

    /// <summary>
    ///     ''' Add a URL line to the sitemap.xml
    ///     ''' </summary>
    private void AddURLElement(ref XmlTextWriter xmlSiteMap, string strURL, string strPriority = "0.5")
    {
        {
            var withBlock = xmlSiteMap;
            withBlock.WriteWhitespace(Constants.vbCrLf);
            withBlock.WriteStartElement("url");
            withBlock.WriteWhitespace(Constants.vbCrLf);
            withBlock.WriteWhitespace("   ");
            withBlock.WriteElementString("loc", strURL);
            withBlock.WriteWhitespace(Constants.vbCrLf);
            withBlock.WriteWhitespace("   ");
            // Set default page to higher priority
            if (Strings.Right(strURL, 13).ToLower() == "/default.aspx")
                withBlock.WriteElementString("priority", "1.0");
            else if (Strings.Right(strURL, 10).ToLower() == "/news.aspx")
                withBlock.WriteElementString("priority", "1.0");
            else
                withBlock.WriteElementString("priority", strPriority);
            withBlock.WriteWhitespace(Constants.vbCrLf);
            withBlock.WriteWhitespace("   ");
            withBlock.WriteElementString("changefreq", ddlChangeFrequency.SelectedValue);
            withBlock.WriteWhitespace(Constants.vbCrLf);
            withBlock.WriteEndElement();
        }
    }

    /// <summary>
    ///     ''' Generate the Froogle / GoogleBase / Google Merchant Feed
    ///     ''' (at some point, Google will settle on a name and keep it!)
    ///     ''' </summary>
    protected void btnFroogle_Click(object sender, System.EventArgs e)
    {
        DataTable tblProducts = null/* TODO Change to default(_) if this is not a reference type */;
        DataTable tblVersions = null/* TODO Change to default(_) if this is not a reference type */;
        List<string> lstAddedProducts = new List<string>();
        List<string> lstAdded = new List<string>();
        string strDesc;
        string strProductName;
        string strVersionName;
        string strPrice;
        string strLink;
        string strImageLink;
        string strAvailability = "";

        FileStream objFileStream = null;
        StreamWriter objStreamWriter = null;
        XmlTextWriter xmlGoogleBase = null/* TODO Change to default(_) if this is not a reference type */;

        // We can generate two formats; txt and xml
        // The XML one is preferred but we've found Google
        // to be a bit more picky and unhelpful with error
        // messages. So .txt format gives a good fallback.
        // The .txt feed is typically smaller too.
        if (ddlXMLorTXT.SelectedValue == "txt")
        {
            objFileStream = new FileStream(Server.MapPath(strAppUploadsFolder) + @"\temp\GoogleBase.txt", FileMode.Create, FileAccess.Write);
            objStreamWriter = new StreamWriter(objFileStream);
            // the seek method is used to move the cursor to next position to avoid text to be overwritten
            // s.BaseStream.Seek(0, SeekOrigin.End)
            // write out the headers
            objStreamWriter.WriteLine("id" + Constants.vbTab + "title" + Constants.vbTab + "description" + Constants.vbTab + "price" + Constants.vbTab + "link" + Constants.vbTab + "image_link" + Constants.vbTab + "condition");
        }
        else if (ddlXMLorTXT.SelectedValue == "csv")
        {
            objFileStream = new FileStream(Server.MapPath(strAppUploadsFolder) + @"\temp\GoogleBase.csv", FileMode.Create, FileAccess.Write);
            objStreamWriter = new StreamWriter(objFileStream);
            // the seek method is used to move the cursor to next position to avoid text to be overwritten
            // s.BaseStream.Seek(0, SeekOrigin.End)
            // write out the headers
            objStreamWriter.WriteLine("'id','title','description','price','link','image_link','condition'");
        }
        else
        {
            // XML feed format
            xmlGoogleBase = new XmlTextWriter(Server.MapPath(strAppUploadsFolder) + @"\temp\GoogleBase.xml", System.Text.Encoding.UTF8);
            // Add the header parts
            {
                var withBlock = xmlGoogleBase;
                withBlock.WriteStartDocument();
                withBlock.WriteWhitespace(Constants.vbCrLf);
                withBlock.WriteStartElement("rss");
                withBlock.WriteAttributeString("version", "2.0");
                withBlock.WriteAttributeString("xmlns:g", "http://base.google.com/ns/1.0");
                withBlock.WriteWhitespace(Constants.vbCrLf);
                withBlock.WriteStartElement("channel");
                withBlock.WriteWhitespace(Constants.vbCrLf);
                withBlock.WriteElementString("title", Server.HtmlEncode(GetGlobalResourceObject("Kartris", "Config_Webshopname")));
                withBlock.WriteWhitespace(Constants.vbCrLf);
                withBlock.WriteElementString("link", CkartrisBLL.WebShopURLhttp);
                withBlock.WriteWhitespace(Constants.vbCrLf);
                withBlock.WriteElementString("description", GetGlobalResourceObject("Kartris", "ContentText_DefaultMetaDescription"));
            }
        }

        // Add lines for each item
        ProductsBLL objProductsBLL = new ProductsBLL();
        VersionsBLL objVersionsBLL = new VersionsBLL();
        foreach (SiteMapNode node in SiteMap.Providers("CategorySiteMapProvider").RootNode.GetAllNodes)
        {
            int intCategoryID = System.Convert.ToInt32(Mid(node.Key, InStrRev(node.Key, ",") + 1));
            if (!lstAdded.Contains(intCategoryID))
            {
                lstAdded.Add(intCategoryID);

                // Fill table with products for each category
                tblProducts = objProductsBLL.GetProductsPageByCategory(intCategoryID, 1, 0, short.MaxValue, 0, short.MaxValue);

                // Loop through each product
                foreach (DataRow drwProduct in tblProducts.Rows)
                {
                    if (!lstAddedProducts.Contains(drwProduct("P_ID")))
                    {
                        lstAddedProducts.Add(drwProduct("P_ID"));
                        strProductName = CkartrisDisplayFunctions.StripHTML(FixNullFromDB(drwProduct("P_Name")));

                        DataTable dtGoogleAttributes = AttributesBLL.GetSpecialAttributesByProductID(drwProduct("P_ID"), 1);

                        try
                        {
                            // Loop through each version
                            foreach (DataRow drwVersion in objVersionsBLL.GetByProduct(drwProduct("P_ID"), 1, 0).Rows)
                            {
                                strVersionName = FixNullFromDB(drwVersion("V_Name"));
                                strDesc = Replace(Replace(FixNullFromDB(drwVersion("V_Desc")), Constants.vbTab, ""), Constants.vbCrLf, "");
                                strPrice = CurrenciesBLL.FormatCurrencyPrice(1, System.Convert.ToDouble(FixNullFromDB((drwVersion("V_Price")))), false);
                                strLink = SiteMapHelper.CreateURL(SiteMapHelper.Page.CanonicalProduct, drwProduct("P_ID"), null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, drwProduct("P_Name"));

                                // Format image link
                                // Ideally we look to see if there is a version image
                                // If not, we use a product image
                                // If nothing found, we leave blank
                                // Note that Froogle/Google will not accept URLs
                                // that go to scripts (such as Image.ashx that provides
                                // thumbnails). Instead, we have to give them the full
                                // image link.
                                strImageLink = ImageLink(FixNullFromDB(drwVersion("V_ID")), FixNullFromDB(drwProduct("P_ID")));

                                strLink = FixURL(strLink);

                                if (strProductName != strVersionName & string.IsNullOrEmpty(strVersionName))
                                    strProductName += " - " + strVersionName;
                                if (string.IsNullOrEmpty(strDesc))
                                    strDesc = FixNullFromDB(drwProduct("P_Desc"));

                                if (ddlXMLorTXT.SelectedValue == "txt")
                                    AddFroogleTextLine(ref objStreamWriter, FixNullFromDB(drwVersion("V_CodeNumber")), strProductName, strDesc, strPrice, strLink, strImageLink);
                                else if (ddlXMLorTXT.SelectedValue == "csv")
                                    AddFroogleCSVLine(ref objStreamWriter, FixNullFromDB(drwVersion("V_CodeNumber")), strProductName, strDesc, strPrice, strLink, strImageLink);
                                else
                                {
                                    var withBlock = xmlGoogleBase;
                                    // ====================================
                                    // BASIC DETAILS
                                    // ====================================
                                    withBlock.WriteWhitespace(Constants.vbCrLf);
                                    withBlock.WriteStartElement("item");
                                    withBlock.WriteWhitespace(Constants.vbCrLf);
                                    withBlock.WriteWhitespace("   ");
                                    withBlock.WriteElementString("g:id", FixNullFromDB(drwVersion("V_CodeNumber")));
                                    withBlock.WriteWhitespace(Constants.vbCrLf);
                                    withBlock.WriteWhitespace("   ");
                                    withBlock.WriteElementString("title", CkartrisDisplayFunctions.StripHTML(strProductName));
                                    withBlock.WriteWhitespace(Constants.vbCrLf);
                                    withBlock.WriteWhitespace("   ");
                                    withBlock.WriteElementString("description", CkartrisDisplayFunctions.StripHTML(strDesc));
                                    withBlock.WriteWhitespace(Constants.vbCrLf);
                                    withBlock.WriteWhitespace("   ");
                                    withBlock.WriteElementString("g:price", strPrice + " " + CurrenciesBLL.CurrencyCode(CurrenciesBLL.GetDefaultCurrency)); // v3, now format currency with ISO code after it
                                    withBlock.WriteWhitespace(Constants.vbCrLf);
                                    withBlock.WriteWhitespace("   ");
                                    withBlock.WriteElementString("link", strLink);
                                    withBlock.WriteWhitespace(Constants.vbCrLf);
                                    withBlock.WriteWhitespace("   ");

                                    // ====================================
                                    // GTIN, MPN or Brand
                                    // ====================================
                                    // Google wants one of MPN, GTIN or Brand
                                    // Many stores use MPN as their SKU/VersionCode
                                    // If so, uncommented section below to take care of this

                                    // .WriteElementString("g:mpn", FixNullFromDB(drwVersion("V_CodeNumber")))
                                    // .WriteWhitespace(vbCrLf)
                                    // .WriteWhitespace("   ")

                                    // ====================================
                                    // STOCK AVAILABILITY
                                    // ====================================
                                    // Need to check if out of stock
                                    if (drwVersion("V_Quantity") < 1.0F & drwVersion("V_QuantityWarnLevel") > 0.0F)
                                        // out of stock
                                        strAvailability = "out of stock";
                                    else
                                        // in stock
                                        strAvailability = "in stock";
                                    withBlock.WriteElementString("g:availability", strAvailability); // v3, in or out of stock
                                    withBlock.WriteWhitespace(Constants.vbCrLf);
                                    withBlock.WriteWhitespace("   ");

                                    // ====================================
                                    // IMAGE LINK
                                    // ====================================
                                    if (strImageLink != "")
                                    {
                                        withBlock.WriteElementString("g:image_link", strImageLink);
                                        withBlock.WriteWhitespace(Constants.vbCrLf);
                                        withBlock.WriteWhitespace("   ");
                                    }
                                    withBlock.WriteElementString("g:condition", UCase(ddlCondition.SelectedValue));
                                    withBlock.WriteWhitespace(Constants.vbCrLf);

                                    // ====================================
                                    // OTHER GOOGLE ATTRIBUTES
                                    // ====================================
                                    // Next, we loop through all the special Google attributes that have
                                    // been setup 
                                    foreach (DataRow drwGoogle in dtGoogleAttributes.Rows)
                                    {
                                        if (FixNullFromDB(drwGoogle("ATTRIBV_Value")) != null)
                                        {
                                            withBlock.WriteWhitespace("   ");
                                            withBlock.WriteElementString(FixNullFromDB(drwGoogle("ATTRIB_Name")), FixNullFromDB(drwGoogle("ATTRIBV_Value")));
                                            withBlock.WriteWhitespace(Constants.vbCrLf);
                                        }
                                    }
                                    withBlock.WriteEndElement();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
        }

        if (ddlXMLorTXT.SelectedValue == "txt")
        {
            objStreamWriter.Close();
            lnkGenerated.NavigateUrl = strAppUploadsFolder + "temp/GoogleBase.txt";

            // Show full URL that needs to be given to Google
            litFilePath.Text = Replace(strAppUploadsFolder, "~/", cKartrisBLL.WebShopURLhttp) + "temp/GoogleBase.txt";
            litFilePath.Visible = true;
        }
        else if (ddlXMLorTXT.SelectedValue == "csv")
        {
            objStreamWriter.Close();
            lnkGenerated.NavigateUrl = strAppUploadsFolder + "temp/GoogleBase.csv";

            // Show full URL that needs to be given to Google
            litFilePath.Text = Replace(strAppUploadsFolder, "~/", CkartrisBLL.WebShopURLhttp) + "temp/GoogleBase.csv";
            litFilePath.Visible = true;
        }
        else
        {
            CloseXMLSitemap(ref xmlGoogleBase);
            lnkGenerated.NavigateUrl = strAppUploadsFolder + "temp/GoogleBase.xml";

            // Show full URL that needs to be given to Google
            litFilePath.Text = Replace(strAppUploadsFolder, "~/", CkartrisBLL.WebShopURLhttp) + "temp/GoogleBase.xml";
            litFilePath.Visible = true;
        }
        lnkGenerated.Visible = true;

        ShowMasterUpdateMessage();
    }

    /// <summary>
    ///     ''' Add a text line to Froogle/Google feed
    ///     ''' </summary>
    private void AddFroogleTextLine(ref StreamWriter objStreamWriter, string V_Codenumber, string V_Name, string V_Desc, double V_Price, string strProductLink, string strImageLink)
    {
        objStreamWriter.WriteLine(V_Codenumber + Constants.vbTab + V_Name + Constants.vbTab + V_Desc + Constants.vbTab + V_Price + Constants.vbTab + strProductLink + Constants.vbTab + strImageLink + Constants.vbTab + UCase(ddlCondition.SelectedValue));
    }

    /// <summary>
    ///     ''' Add a text line to Froogle/Google feed
    ///     ''' </summary>
    private void AddFroogleCSVLine(ref StreamWriter objStreamWriter, string V_Codenumber, string V_Name, string V_Desc, double V_Price, string strProductLink, string strImageLink)
    {
        objStreamWriter.WriteLine("\"" + V_Codenumber + "\",\"" + Strings.Replace(V_Name, "\"", "\"\"") + "\",\"" + Strings.Replace(V_Desc, "\"", "\"\"") + "\",\"" + V_Price + "\",\"" + strProductLink + "\",\"" + strImageLink + "\",\"" + UCase(ddlCondition.SelectedValue) + "\"");
    }

    /// <summary>
    ///     ''' Fix URL - ensure is fully qualified absolute URL
    ///     ''' </summary>
    private string FixURL(string strLink)
    {
        // Dim numPortNumber As Integer = Context.Request.ServerVariables("SERVER_PORT")
        // Dim strServerName As String = Context.Request.ServerVariables("SERVER_NAME")

        // Dim strNewWebShopURL As String = strServerName

        // Add port number to end, if not default one
        // If numPortNumber <> 80 Then
        // strNewWebShopURL &= ":" & numPortNumber.ToString
        // End If

        if (Strings.InStr(strLink, "~/") > 0)
            strLink = Replace(strLink, "~/", cKartrisBLL.WebShopURLhttp);
        else if (!InStr(strLink, cKartrisBLL.WebShopURLhttp) > 0)
            // Link begins with just /
            strLink = Left(CkartrisBLL.WebShopURLhttp, Len(CkartrisBLL.WebShopURLhttp) - 1) + strLink;

        if (InStr(strLink, CkartrisBLL.WebShopURLhttp + CkartrisBLL.WebShopFolder))
            strLink = Replace(strLink, CkartrisBLL.WebShopURLhttp + CkartrisBLL.WebShopFolder, CkartrisBLL.WebShopURLhttp);
        return strLink;
    }

    /// <summary>
    ///     ''' Find an image, create a fully qualified link for it
    ///     ''' We need to look first for version image, then if
    ///     ''' none exists, a product one.
    ///     ''' Last straw is to return a blank.
    ///     ''' </summary>
    private string ImageLink(int V_ID, int P_ID)
    {
        Array arrImageTypes = Split(KartSettingsManager.GetKartConfig("backend.imagetypes"), ",");

        string strImageLink = "";

        strImageLink = "";

        // This is folder where product images would be, if there
        // are any
        DirectoryInfo dirFolderProducts = new DirectoryInfo(Server.MapPath("~/Images/Products/" + P_ID));
        FileInfo objFile = null;

        // Does product folder exist? If yes, continue, otherwise
        // we can stop - no images.
        if (dirFolderProducts.Exists)
        {

            // Folder where versions would be
            DirectoryInfo dirFolderVersions = new DirectoryInfo(Server.MapPath("~/Images/Products/" + P_ID + "/" + V_ID));
            if (dirFolderVersions.Exists)
            {
                // Try to find a version image
                foreach (var objFile in dirFolderVersions.GetFiles())
                {
                    strImageLink = cKartrisBLL.WebShopURLhttp + "Images/Products/" + P_ID + "/" + V_ID + "/" + objFile.Name;
                    break;
                }
            }
            else
                // No versions folder, let's pull product image instead
                foreach (var objFile in dirFolderProducts.GetFiles())
                {
                    strImageLink = cKartrisBLL.WebShopURLhttp + "Images/Products/" + P_ID + "/" + objFile.Name;
                    break;
                }
        }
        else
            // No product image folder = no product or version images
            strImageLink = "";

        return strImageLink;
    }

    // Just show the 'updated' message
    protected void ShowMasterUpdateMessage()
    {
        (Skins_Admin_Template)this.Master.DataUpdated();
    }
}
