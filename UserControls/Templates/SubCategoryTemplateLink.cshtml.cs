partial class SubCategoryTemplateLink : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        lnkCategoryName.NavigateUrl = SiteMapHelper.CreateURL(SiteMapHelper.Page.Category, litCategoryIDHidden.Text, System.Web.UI.UserControl.Request.QueryString["strParent"] + "," + System.Web.UI.UserControl.Request.QueryString["CategoryID"]);
    }
}
