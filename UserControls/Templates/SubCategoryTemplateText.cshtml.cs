using CkartrisDisplayFunctions;
using KartSettingsManager;
/// <summary>

/// ''' User Control Template for the Text View of the Child Categories (SubCategories)

/// ''' </summary>

/// ''' <remarks></remarks>
partial class SubCategoryTemplateText : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        lnkCategoryName.NavigateUrl = SiteMapHelper.CreateURL(SiteMapHelper.Page.Category, litCategoryID.Text, System.Web.UI.UserControl.Request.QueryString["strParent"] + "," + System.Web.UI.UserControl.Request.QueryString["CategoryID"]);

        // ' Trancating the Description Text, depending on the related key in CONFIG Setting
        // ' The Full Description Text is Held by a Hidden Literal Control.
        int intMaxChar;
        intMaxChar = GetKartConfig("frontend.categories.display.text.truncatedescription");
        litCategoryDesc.Text = TruncateDescription(litCategoryDescHidden.Text, intMaxChar);
    }
}
