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
using CkartrisImages;

internal partial class LargeImage : System.Web.UI.Page
{
    public LargeImage()
    {
        this.Load += Page_Load;
        this.PreInit += Page_PreInit;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        int P_ID = Request.QueryString("P_ID");
        var blnFullSize = Request.QueryString("blnFullSize") == "y";

        if (blnFullSize == true)
        {
            // Set height and width to zero, this will trigger
            // image as full size in imageviewer
            UC_ImageView.CreateImageViewer(IMAGE_TYPE.enum_ProductImage, P_ID, 0, 0, "", "");




        }
        else
        {
            // Set image height and width to defaults
            UC_ImageView.CreateImageViewer(IMAGE_TYPE.enum_ProductImage, P_ID, KartSettingsManager.GetKartConfig("frontend.display.images.large.height"), KartSettingsManager.GetKartConfig("frontend.display.images.large.width"), "", "");




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