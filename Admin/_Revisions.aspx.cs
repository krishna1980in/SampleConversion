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
using System.Xml.Linq;
using System.Xml;
using CkartrisEnumerations;
using KartSettingsManager;

partial class Admin_Revisions : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Kartris", "BackMenu_Revisions") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        if (!this.IsPostBack)
        {
            // GetXML(0)
            try
            {
                GetXML(0);
                phdFeed.Visible = true;
                phdFeedNotAccessible.Visible = false;
            }
            catch (Exception ex)
            {
                phdFeed.Visible = false;
                phdFeedNotAccessible.Visible = true;
            }
        }
    }

    protected void GetXML(int numItem)
    {
        string strCacheKey = "RevisionsFeed";
        XDocument xmlDoc = null;

        // We want to pull from cache if possible, it is
        // faster and keeps traffic to kartris.com down
        if (Cache.Get(strCacheKey) == null)
        {

            // Put it in a try, in case a bad result is or some
            // other problem like an error
            try
            {
                // We've reworked this in v2.5004 to prevent timeouts
                // if the kartris.com site where the feed is located
                // is unreachable. We use the httpWebRequest so we can
                // apply a timeout setting of 1 second.
                System.Net.HttpWebRequest reqFeed = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://www.kartris.com/feed/revisions/?url=" + CkartrisBLL.WebShopURL);
                reqFeed.Timeout = 1000; // milliseconds
                System.Net.WebResponse resFeed = reqFeed.GetResponse();
                Stream responseStream = resFeed.GetResponseStream();
                XmlDocument docXML = new XmlDocument();
                docXML.Load(responseStream);
                responseStream.Close();

                // Set XDocument to the XML string we got back from feed
                xmlDoc = XDocument.Parse(docXML.OuterXml);

                // Add feed data to local cache for one hour
                Cache.Insert("RevisionsFeed", XDocument.Parse(xmlDoc.ToString()), null/* TODO Change to default(_) if this is not a reference type */, DateTime.Now.AddMinutes(60), TimeSpan.Zero);
            }
            catch (Exception ex)
            {
            }
        }
        else
            // Pull feed data from cache
            xmlDoc = (XDocument)Cache.Get(strCacheKey);
        ;/* Cannot convert LocalDeclarationStatementSyntax, System.InvalidOperationException: Sequence contains more than one element
   at System.Linq.Enumerable.Single[TSource](IEnumerable`1 source)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.ConvertSelectClauseSyntax(SelectClauseSyntax vbFromClause)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitQueryExpression(QueryExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitQueryExpression(QueryExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertInitializer(VariableDeclaratorSyntax declarator)
   at ICSharpCode.CodeConverter.CSharp.CommonConversions.SplitVariableDeclarations(VariableDeclaratorSyntax declarator)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 

        'Define LINQ query to pull 1 record from XML
        'Note the 'skip' second line from bottom - so we can jump to a record
        'Note the 'take' at end, similar to 'top' in SQL
        Dim xmlChannel = From xmlItem In xmlDoc.Descendants("item") _
        Select intMajorVersion = xmlItem.Element("majorVersion").Value, _
        numMinorVersion = xmlItem.Element("minorVersion").Value, _
        blnIsBeta = xmlItem.Element("beta").Value, _
        datReleaseDate = CDate(xmlItem.Element("releaseDate").Value), _
        strType = xmlItem.Element("type").Value, _
        strBugFixes = xmlItem.Element("bugFixes").Value, _
        strImprovements = xmlItem.Element("improvements").Value _
        Order By datReleaseDate Descending

 */
        // Use the stringbuilder class for performance
        StringBuilder objStringBuilder = new StringBuilder();
        int numCounter = 0;

        foreach (var xmlItem in xmlChannel)
        {

            // If prerelease version, append to top of list
            if (numCounter == 0 & xmlItem.numMinorVersion < KARTRIS_VERSION)
            {
                objStringBuilder.Append("<span class=\"versionlabel\">" + GetGlobalResourceObject("_SoftwareUpdate", "ContentText_Installed") + "*</span>" + Constants.vbCrLf);
                objStringBuilder.Append("<div>" + Constants.vbCrLf);
                objStringBuilder.Append("<div class=\"revisions_Prerelease\"><span class=\"minorversion\">" + KARTRIS_VERSION.ToString("N4") + "</span>" + Constants.vbCrLf);
                objStringBuilder.Append("<span class=\"type\">Pre-release</span></div>" + Constants.vbCrLf);
                objStringBuilder.Append("</div>" + Constants.vbCrLf);
            }
            numCounter = +1;

            if (xmlItem.numMinorVersion == KARTRIS_VERSION)
                objStringBuilder.Append("<span class=\"versionlabel\">" + GetGlobalResourceObject("_SoftwareUpdate", "ContentText_Installed") + "*</span>" + Constants.vbCrLf);

            objStringBuilder.Append("<div>" + Constants.vbCrLf);
            objStringBuilder.Append("<div class=\"revisions_" + xmlItem.strType + "\"><span class=\"minorversion\">" + xmlItem.numMinorVersion + "</span>" + Constants.vbCrLf);
            objStringBuilder.Append("<span class=\"type\">" + xmlItem.strType + "</span>" + Constants.vbCrLf);
            objStringBuilder.Append("<span class=\"date\">(" + CkartrisDisplayFunctions.FormatDate(xmlItem.datReleaseDate, "t", Session("_LANG")) + ")</span></div>" + Constants.vbCrLf);
            if (xmlItem.strBugFixes != "")
                objStringBuilder.Append("<blockquote class=\"bugfixes\">" + Strings.Replace(xmlItem.strBugFixes, "* ", "<br />* ") + "</blockquote>" + Constants.vbCrLf);
            if (xmlItem.strImprovements != "")
                objStringBuilder.Append("<blockquote class=\"improvements\">" + Strings.Replace(xmlItem.strImprovements, "* ", "<br />* ") + "</blockquote>" + Constants.vbCrLf);
            objStringBuilder.Append("</div>" + Constants.vbCrLf);
        }

        // If no data
        if (objStringBuilder.ToString() == "")
            objStringBuilder.Append("<div>No change log info.</div>");

        // Set text to page
        litXMLData.Text = objStringBuilder.ToString();
    }
}