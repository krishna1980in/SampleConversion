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

partial class UserControls_Front_MediaGallery : System.Web.UI.UserControl
{
    private string _ParentType;
    private int _ParentID;

    private string _strInlineJS;

    /// <summary>
    ///     ''' Media Links Parent Type
    ///     ''' </summary>
    ///     ''' <value>Can either be 'p' or 'v'</value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public string ParentType
    {
        set
        {
            _ParentType = value;
        }
        get
        {
            return _ParentType;
        }
    }


    /// <summary>
    ///     ''' Media Links Parent ID
    ///     ''' </summary>
    ///     ''' <value>Version or Product ID</value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public long ParentID
    {
        set
        {
            _ParentID = value;
        }
        get
        {
            return _ParentID;
        }
    }


    protected void Page_Load(object sender, System.EventArgs e)
    {
        DataTable tblMediaLinks;
        if (!System.Web.UI.UserControl.IsPostBack)
        {
            if (_ParentID > 0)
            {
                // retrieve all the links for this specific record - called by LoadProduct() in ProductView.ascx.vb & ????? in ProductVersions.ascx.vb
                tblMediaLinks = MediaBLL.GetMediaLinksByParent(_ParentID, _ParentType);

                if (tblMediaLinks.Rows.Count > 0)
                {
                    rptMediaLinks.DataSource = tblMediaLinks;
                    // call databind to trigger rptMediaLinks_ItemDataBound() Sub
                    rptMediaLinks.DataBind();
                }
                else
                    // hide media gallery
                    phdMediaGallery.Visible = false;
            }
        }
    }

    protected void rptMediaLinks_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
        {
            string strWebShopURL = CkartrisBLL.WebShopURL;

            string strIconLink;
            bool blnCustomThumb;

            DataRowView drvMediaLink = (DataRowView)e.Item.DataItem;

            // get this item's media type details
            string strMT_Extension = FixNullFromDB(drvMediaLink.Item("MT_Extension"));
            bool blnisEmbedded = System.Convert.ToBoolean(FixNullFromDB(drvMediaLink.Item("MT_Embed")));
            bool blnisInline = System.Convert.ToBoolean(FixNullFromDB(drvMediaLink.Item("MT_Inline")));
            bool blnisDownloadable;
            try
            {
                blnisDownloadable = System.Convert.ToBoolean(drvMediaLink.Item("ML_isDownloadable"));
            }
            catch (Exception ex)
            {
                blnisDownloadable = System.Convert.ToBoolean(FixNullFromDB(drvMediaLink.Item("MT_DefaultisDownloadable")));
            }

            int intHeight = FixNullFromDB(drvMediaLink.Item("ML_Height"));
            int intWidth = FixNullFromDB(drvMediaLink.Item("ML_Width"));

            // get the default height and width from the media type if the item doesn't have its height and width set
            if (intHeight == 0)
                intHeight = FixNullFromDB(drvMediaLink.Item("MT_DefaultHeight"));
            if (intWidth == 0)
                intWidth = FixNullFromDB(drvMediaLink.Item("MT_DefaultWidth"));

            string strMT_IconFolder = "Images/MediaTypes/";
            string strMediaLinksFolder = KartSettingsManager.GetKartConfig("general.uploadfolder") + "Media/";

            int intML_ID = drvMediaLink.Item("ML_ID");
            string strML_EmbedSource = FixNullFromDB(drvMediaLink.Item("ML_EmbedSource"));
            string strML_Type = FixNullFromDB(drvMediaLink.Item("ML_ParentType"));

            // Get thumbnail or media type icon
            strIconLink = GetThumbPath(intML_ID, strMT_Extension);

            // Need to know if this is a custom thumbnail, can find this
            // from the path. If it is, we will make it a bit bigger and
            // can format a little differently. Custom thumbs will normally
            // be thumbnail images of a video for example, rather than an
            // icon which you'd generally set centrally for a particular file
            // type.
            try
            {
                blnCustomThumb = !strIconLink.Contains("Images/MediaTypes/");
            }
            catch (Exception ex)
            {
                blnCustomThumb = false;
            }

            string strML_EmbedSource1 = "";

            if (string.IsNullOrEmpty(strML_EmbedSource))
            {
                if (strMT_Extension != "html5video")
                    strML_EmbedSource = strWebShopURL + Strings.Replace(strMediaLinksFolder, "~/", "") + intML_ID + "." + strMT_Extension;
                else
                {
                    strML_EmbedSource = strWebShopURL + Strings.Replace(strMediaLinksFolder, "~/", "") + intML_ID + ".mp4";
                    strML_EmbedSource1 = strWebShopURL + Strings.Replace(strMediaLinksFolder, "~/", "") + intML_ID + ".webm";
                }
                if (!File.Exists(System.Web.UI.UserControl.Server.MapPath(strML_EmbedSource.Replace(strWebShopURL, "~/"))))
                {
                    e.Item.Visible = false;
                    CkartrisFormatErrors.LogError("Media File " + strML_EmbedSource + " (product:" + _GetProductID() + ") is not found!");
                    return;
                }
            }

            if (blnisInline)
            {
                Literal litInline = new Literal();
                litInline.Text = "<div id='inline_" + intML_ID + "'>";

                // check if the media link item already contains the embed code
                if (blnisEmbedded)
                {
                    // embed - just write it directly to the page
                    litInline.Text += strML_EmbedSource + "</div>";
                    (PlaceHolder)e.Item.FindControl("phdInline").Controls.Add(litInline);
                }
                else
                {
                    (PlaceHolder)e.Item.FindControl("phdInline").Controls.Add(litInline);
                    // template is needed so load up the appropriate control
                    UserControl UC_Media = System.Web.UI.TemplateControl.LoadControl("~/UserControls/MediaTemplates/" + strMT_Extension + ".ascx");
                    if (UC_Media != null)
                    {
                        UC_Media.Attributes.Add("Source", strML_EmbedSource);
                        UC_Media.Attributes.Add("Height", intHeight);
                        UC_Media.Attributes.Add("Width", intWidth);
                        if (strMT_Extension == "html5video")
                            UC_Media.Attributes.Add("WebM", strML_EmbedSource1);

                        // Previously we got this from db, but now we just preset this, and
                        // use the parameters field for text which is more useful. Nobody
                        // seems to change these anyway.
                        string strParameters = "autostart=0;autoreplay=1;showtime=1;randomplay=1;nopointer=1";

                        // parse the parameters
                        // format: parameter1=value1;parameter2=value2;parameter3=value3
                        if (!string.IsNullOrEmpty(strParameters))
                        {
                            string[] arrParameters = Strings.Split(strParameters, ";");

                            for (var x = 0; x <= Information.UBound(arrParameters); x++)
                            {
                                string[] arrParameter = Strings.Split(arrParameters[x], "=");
                                if (Information.UBound(arrParameter) == 1)
                                    UC_Media.Attributes.Add(arrParameter[0], arrParameter[1]);
                            }
                        }
                    }

                    // finally add the template to the placeholder
                    (PlaceHolder)e.Item.FindControl("phdInline").Controls.Add(UC_Media);

                    // and close the inline div
                    Literal litInlineEnd = new Literal();
                    litInlineEnd.Text = "</div>";
                    (PlaceHolder)e.Item.FindControl("phdInline").Controls.Add(litInlineEnd);
                }
            }
            else
            {

                // Set image
                string strMediaImageLink = "<img alt=\"\" class=\"media_link\" src=\"" + strIconLink + "\" />";


                if (blnisDownloadable | strMT_Extension == "urlnewtab")
                {
                    // Show download link
                    HyperLink lnkDownload = (HyperLink)e.Item.FindControl("lnkDownload");
                    if (lnkDownload != null)
                    {
                        if (blnisDownloadable | strMT_Extension == "urlnewtab")
                        {
                            lnkDownload.Visible = true;
                            lnkDownload.Text = strMediaImageLink + FixNullFromDB(drvMediaLink.Item("ML_Parameters"));
                            lnkDownload.NavigateUrl = strML_EmbedSource;
                            lnkDownload.Target = "_blank";
                            if (blnCustomThumb)
                                lnkDownload.CssClass = lnkDownload.CssClass + " customthumb";
                        }
                        else
                            lnkDownload.Visible = false;
                    }
                }
                else if (strMT_Extension == "urlpopup")
                    // popup with URL in iframe
                    (Literal)e.Item.FindControl("litMediaLink").Text = "<a class=\"" + Interaction.IIf(blnCustomThumb, "customthumb", "") + "\" id=\"poplink_" + intML_ID + "\" href=\"javascript:ShowURLPopup('" + strML_EmbedSource + "','" + intWidth + "','" + intHeight + "')\">" + strMediaImageLink + FixNullFromDB(drvMediaLink.Item("ML_Parameters")) + "</a>";
                else
                    // popup media launcher
                    (Literal)e.Item.FindControl("litMediaLink").Text = "<a class=\"" + Interaction.IIf(blnCustomThumb, "customthumb", "") + "\" id=\"poplink_" + intML_ID + "\" href=\"javascript:ShowMediaPopup('" + intML_ID + "','" + strMT_Extension + "','" + _ParentID + "','" + _ParentType + "','" + intWidth + "','" + intHeight + "')\">" + strMediaImageLink + FixNullFromDB(drvMediaLink.Item("ML_Parameters")) + "</a>";
            }
        }
    }

    // Function to find either default icon, or custom
    // thumbnail (if available)
    public string GetThumbPath(string numMediaID, string strMediaType)
    {
        string strWebShopURL = CkartrisBLL.WebShopURL;
        try
        {
            string strImagesFolder = KartSettingsManager.GetKartConfig("general.uploadfolder") + "Media/";
            DirectoryInfo dirMediaImages = new DirectoryInfo(System.Web.UI.UserControl.Server.MapPath(strImagesFolder));
            try
            {
                foreach (FileInfo objFile in dirMediaImages.GetFiles(numMediaID + "_thumb.*"))
                    return Strings.Replace(strImagesFolder, "~/", strWebShopURL) + objFile.Name;
            }
            catch (Exception ex)
            {
            }

            // Media Link doesn't have thumbnail so get the default media type icon
            strImagesFolder = "~/Images/MediaTypes/";
            DirectoryInfo dirMediaIconImages = new DirectoryInfo(System.Web.UI.UserControl.Server.MapPath(strImagesFolder));
            try
            {
                foreach (FileInfo objFile in dirMediaIconImages.GetFiles(strMediaType + ".*"))
                    return Strings.Replace(strImagesFolder, "~/", strWebShopURL) + objFile.Name;
            }
            catch (Exception ex)
            {
            }
        }
        catch (Exception ex)
        {
        }

        return null;
    }
}
