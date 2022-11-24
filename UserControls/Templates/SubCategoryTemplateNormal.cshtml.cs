using CkartrisImages;
using CkartrisDisplayFunctions;
using KartSettingsManager;

/// <summary>

/// ''' User Control Template for the Normal View of the Child Categories (SubCategories)

/// ''' </summary>

/// ''' <remarks>By Mohammad</remarks>
partial class SubCategoryTemplateNormal : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        lnkCategoryName.NavigateUrl = SiteMapHelper.CreateURL(SiteMapHelper.Page.Category, litCategoryID.Text, System.Web.UI.UserControl.Request.QueryString["strParent"] + "," + System.Web.UI.UserControl.Request.QueryString["CategoryID"]);

        lnkMore.NavigateUrl = lnkCategoryName.NavigateUrl;

        UC_ImageView.CreateImageViewer(IMAGE_TYPE.enum_CategoryImage, litCategoryID.Text, KartSettingsManager.GetKartConfig("frontend.display.images.thumb.height"), KartSettingsManager.GetKartConfig("frontend.display.images.thumb.width"), lnkCategoryName.NavigateUrl, null/* Conversion error: Set to default value for this argument */, null/* Conversion error: Set to default value for this argument */, lnkCategoryName.Text);

        // ' Truncating the Description Text, depending on the related key in CONFIG Setting
        // ' The Full Description Text is Held by a Hidden Literal Control.
        int intMaxChar;
        intMaxChar = GetKartConfig("frontend.categories.display.normal.truncatedescription");
        litCategoryDesc.Text = TruncateDescription(litCategoryDescHidden.Text, intMaxChar);
    }
}
