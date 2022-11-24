using CkartrisImages;
/// <summary>

/// ''' User Control Template for Promotions.

/// ''' </summary>

/// ''' <remarks>By Paul</remarks>
partial class Templates_PromotionTemplate : System.Web.UI.UserControl
{
    private string _PromotionID;
    private string _PromotionName;
    private string _PromotionText;

    public string PromotionID
    {
        get
        {
            return _PromotionID;
        }
        set
        {
            _PromotionID = value;
        }
    }

    public string PromotionName
    {
        get
        {
            return _PromotionName;
        }
        set
        {
            _PromotionName = value;
        }
    }

    public string PromotionText
    {
        get
        {
            return _PromotionText;
        }
        set
        {
            _PromotionText = value;
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        LoadPromotions(PromotionID, PromotionName, PromotionText);
    }

    public void LoadPromotions(string strPromotionID, string strPromotionName, string strPromotionText)
    {
        litPromotionIDHidden.Text = strPromotionID;
        litPromotionName.Text = strPromotionName;
        litPromotionText.Text = strPromotionText;

        if (PromotionID != "")
            UC_ImageView.CreateImageViewer(IMAGE_TYPE.enum_PromotionImage, litPromotionIDHidden.Text, KartSettingsManager.GetKartConfig("frontend.display.images.minithumb.height"), KartSettingsManager.GetKartConfig("frontend.display.images.minithumb.width"), "", "");
    }
}
