using CkartrisImages;
using KartSettingsManager;

/// <summary>

/// ''' User Control Template for tiny image thumbs for basket, recent products, etc.

/// ''' Now using GetImageURL function in BasketBLL.vb, so will show version image if available

/// ''' </summary>

/// ''' <remarks>By Paul</remarks>
partial class ProductTemplateImageOnly : System.Web.UI.UserControl
{
    public string CreateImageTag()
    {
        string strImageURL = " ";
        string strImageTag = " ";
        string strItemType = "p";
        bool blnPlaceHolder = (KartSettingsManager.GetKartConfig("frontend.display.image.products.placeholder") == "y");

        // Dim strNavigateURL As String = SiteMapHelper.CreateURL(SiteMapHelper.Page.Product, litProductID.Text, Request.QueryString("strParent"), Request.QueryString("CategoryID"))

        // If recent products, the above can generate a bad path on pages called
        // with unfriend URLs, such as from search results. So let's
        // use canonical URL instead.
        var strNavigateURL = SiteMapHelper.CreateURL(SiteMapHelper.Page.CanonicalProduct, litProductID.Text);

        strImageURL = BasketBLL.GetImageURL(litVersionID.Text, litProductID.Text);

        if (strImageURL != "")
            strImageTag = "<a href=\"" + strNavigateURL + "\"><img alt=\"" + litP_Name.Text + "\" src=\"" + strImageURL + "\" /></a>";
        else if (blnPlaceHolder)
        {
            strImageURL = CkartrisBLL.WebShopURL + "Image.ashx?strItemType=" + strItemType + "&amp;numMaxHeight=" + KartSettingsManager.GetKartConfig("frontend.display.images.minithumb.height") + "&amp;numMaxWidth=" + KartSettingsManager.GetKartConfig("frontend.display.images.minithumb.width") + "&amp;numItem=0&amp;strParent=0";
            strImageTag = "<a href=\"" + strNavigateURL + "\"><img alt=\"No image\" src=\"" + strImageURL + "\" /></a>";
        }
        else
            this.Visible = false;// turn off this whole control

        return strImageTag;
    }
}
