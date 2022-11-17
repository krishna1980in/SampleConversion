using System;
using System.Linq;
using System.Xml.Linq;
using CkartrisDataManipulation;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic

internal partial class MultiMedia : System.Web.UI.Page
{
    public MultiMedia()
    {
        this.Load += Page_Load;
        this.PreInit += Page_PreInit;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        // These parameters are added to the URL of the 
        // iframe src (set in PopupMessage.ascx.vb) by the
        // ShowMediaPopup function on ProductView.aspx. In this
        // way, the URL of the iframe can be different for each
        // link, with the parameters being changed dynamically
        // by javascript just before the popup is made visible.
        int intML_ID = Request.QueryString("ML_ID");
        string strMT_Extension = Request.QueryString("MT_Extension");
        int intParentID = Request.QueryString("intParentID");
        string strParentType = Request.QueryString("strParentType");
        int intWidth = Request.QueryString("intWidth");
        int intHeight = Request.QueryString("intHeight");

        DataTable tblMediaLinks;
        if (!IsPostBack)
        {
            if (intML_ID > 0)
            {

                // retrieve all the links for this specific record - called by LoadProduct() in ProductView.ascx.vb & ????? in ProductVersions.ascx.vb

                tblMediaLinks = MediaBLL.GetMediaLinksByParent(intParentID, strParentType);

                var objResults = from rowLink in tblMediaLinks.AsEnumerable()
                                 where rowLink.Field<int>("ML_ID") == intML_ID
                                 select rowLink;
                DataRow drwResult = default;
                DataTable tblFilteredLinks = tblMediaLinks.Clone;

                // add the results items into the cloned DataTable
                foreach (var objValue in objResults)
                {
                    drwResult = tblFilteredLinks.NewRow();
                    drwResult.ItemArray = objValue.ItemArray;
                    tblFilteredLinks.Rows.Add(drwResult);
                }

                // bind the results to a GridView
                rptMediaLinks.DataSource = tblFilteredLinks;
                rptMediaLinks.DataBind();

            }
        }

    }

    protected void rptMediaLinks_ItemDataBound(object sender, Global.System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
        {

            string strWebShopURL = CkartrisBLL.WebShopURL;

            DataRowView drvMediaLink = (DataRowView)e.Item.DataItem;

            // get this item's media type details
            string strMT_Extension = FixNullFromDB(drvMediaLink.Item("MT_Extension"));
            bool blnisEmbedded = (bool)FixNullFromDB(drvMediaLink.Item("MT_Embed"));


            int intHeight = FixNullFromDB(drvMediaLink.Item("ML_Height"));
            int intWidth = FixNullFromDB(drvMediaLink.Item("ML_Width"));

            // get the default height and width from the media type if the item doesn't have its height and width set
            if (intHeight == 0)
            {
                intHeight = FixNullFromDB(drvMediaLink.Item("MT_DefaultHeight"));
            }
            if (intWidth == 0)
            {
                intWidth = FixNullFromDB(drvMediaLink.Item("MT_DefaultWidth"));
            }

            string strMediaLinksFolder = KartSettingsManager.GetKartConfig("general.uploadfolder") + "Media/";

            int intML_ID = drvMediaLink.Item("ML_ID");
            string strML_EmbedSource = FixNullFromDB(drvMediaLink.Item("ML_EmbedSource"));
            string strML_Type = FixNullFromDB(drvMediaLink.Item("ML_ParentType"));

            string strML_EmbedSource1 = "";

            if (string.IsNullOrEmpty(strML_EmbedSource))
            {
                if (strMT_Extension != "html5video")
                {
                    strML_EmbedSource = strWebShopURL + Strings.Replace(strMediaLinksFolder, "~/", "") + intML_ID + "." + strMT_Extension;
                }
                else
                {
                    strML_EmbedSource = strWebShopURL + Strings.Replace(strMediaLinksFolder, "~/", "") + intML_ID + ".mp4";
                    strML_EmbedSource1 = strWebShopURL + Strings.Replace(strMediaLinksFolder, "~/", "") + intML_ID + ".webm";
                }
            }

            var litInline = new Literal();

            // check if the media link item already contains the embed code
            if (blnisEmbedded)
            {
                // embed - just write it directly to the page
                litInline.Text += strML_EmbedSource;
                phdInline.Controls.Add(litInline);
            }
            else
            {
                litInline.Text = "<div id='inline_" + intML_ID + "'>";
                phdInline.Controls.Add(litInline);
                // template is needed so load up the appropriate control
                UserControl UC_Media = LoadControl("~/UserControls/MediaTemplates/" + strMT_Extension + ".ascx");
                if (UC_Media is not null)
                {
                    UC_Media.Attributes.Add("Source", strML_EmbedSource);
                    UC_Media.Attributes.Add("Height", intHeight);
                    UC_Media.Attributes.Add("Width", intWidth);
                    if (strMT_Extension == "html5video")
                        UC_Media.Attributes.Add("WebM", strML_EmbedSource1);

                    // try to get if media link has its own set of parameters
                    string strParameters = FixNullFromDB(drvMediaLink.Item("ML_Parameters"));

                    // and get the default paramters from the media type if none
                    if (string.IsNullOrEmpty(strParameters))
                    {
                        strParameters = FixNullFromDB(drvMediaLink.Item("MT_DefaultParameters"));
                    }

                    // parse the parameters
                    // format: parameter1=value1;parameter2=value2;parameter3=value3
                    if (string.IsNullOrEmpty(strParameters))
                    {
                        var arrParameters = Strings.Split(strParameters, ";");

                        for (int x = 0, loopTo = Information.UBound(arrParameters); x <= loopTo; x++)
                        {
                            var arrParameter = Strings.Split(arrParameters[x], "=");
                            if (Information.UBound(arrParameter) == 1)
                            {
                                UC_Media.Attributes.Add(arrParameter[0], arrParameter[1]);
                            }
                        }
                    }
                }

                // finally add the template to the placeholder
                phdInline.Controls.Add(UC_Media);

                // and close the inline div
                var litInlineEnd = new Literal();
                litInlineEnd.Text = "</div>";
                phdInline.Controls.Add(litInlineEnd);

            }
        }
    }

    protected void Page_PreInit(object sender, EventArgs e)
    {

        string strSkin = CkartrisBLL.Skin(Session("LANG"));

        // We used a skin specified in the language record, if none
        // then use the default 'Kartris' one.
        if (!string.IsNullOrEmpty(strSkin))
        {
            strSkin = "Kartris";
        }

    }
}