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

partial class UserControls_Back_AdminFTS : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!System.Web.UI.Control.Page.IsPostBack)
        {
            try
            {
                LoadFTSInformation();
            }
            catch (Exception ex)
            {
                mvwFTS.SetActiveView(viwNotSupported);
                updMain.Update();
            }
        }
    }

    public void LoadFTSInformation()
    {
        bool blnKartrisCatalogExist;
        bool blnKartrisFTSEnabled;
        string strKartrisFTSLanguages = "";
        bool blnFTSSupported;
        string strMsg = "";
        KartrisDBBLL._GetFTSInformation(blnKartrisCatalogExist, blnKartrisFTSEnabled, strKartrisFTSLanguages, blnFTSSupported, strMsg);
        if (blnFTSSupported)
        {
            if (blnKartrisCatalogExist && blnKartrisFTSEnabled)
            {
                if (strKartrisFTSLanguages.Contains("Neutral"))
                {
                    strKartrisFTSLanguages = strKartrisFTSLanguages.Replace("Neutral", "<b>Neutral*</b>");
                    litNeutralLanguages.Visible = true;
                }
                if (strKartrisFTSLanguages.EndsWith("##"))
                    strKartrisFTSLanguages = strKartrisFTSLanguages.TrimEnd("##");
                litFTSLanguages.Text = strKartrisFTSLanguages.Replace("##", ", ");
                mvwFTS.SetActiveView(viwEnabled);
            }
            else
                mvwFTS.SetActiveView(viwNotEnabled);
        }
        else
            mvwFTS.SetActiveView(viwNotSupported);
        updMain.Update();
    }

    protected void lnkSetupFTS_Click(object sender, System.EventArgs e)
    {
        KartrisDBBLL.SetupFTS();
        ConfigBLL._UpdateConfigValue("general.fts.enabled", "y");
        LoadFTSInformation();
    }

    protected void lnkStopFTS_Click(object sender, System.EventArgs e)
    {
        KartrisDBBLL.StopFTS();
        ConfigBLL._UpdateConfigValue("general.fts.enabled", "n");
        LoadFTSInformation();
    }
}
