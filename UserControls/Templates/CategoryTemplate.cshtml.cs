using Microsoft.VisualBasic;
using CkartrisImages;
/// <summary>

/// ''' User Control Template for the Category View

/// ''' </summary>

/// ''' <remarks>By Mohammad</remarks>
partial class CategoryTemplate : System.Web.UI.UserControl
{
    private int _CategoryID;
    private string _CategoryName;
    private string _CategoryDesc;

    // ' Returns the Current Category ID
    public int CategoryID
    {
        get
        {
            return _CategoryID;
        }
    }

    // ' Returns the Current Category Name
    public string CategoryName
    {
        get
        {
            return _CategoryName;
        }
    }

    // ' Returns the Current Category Description
    public string CategoryDesc
    {
        get
        {
            return _CategoryDesc;
        }
    }


    /// <summary>
    ///     ''' Loads the Category's Info. into the Template's Attributes/Controls
    ///     ''' </summary>
    ///     ''' <param name="pCategoryID"></param>
    ///     ''' <param name="pCategoryName"></param>
    ///     ''' <param name="pCategoryDesc"></param>
    ///     ''' <remarks>By Mohammad</remarks>
    public void LoadCategoryTemplate(int pCategoryID, string pCategoryName, string pCategoryDesc)
    {
        _CategoryID = pCategoryID;
        _CategoryName = pCategoryName;
        _CategoryDesc = pCategoryDesc;

        // ' Load the Name/Description of the Category.
        litCategoryName.Text = _CategoryName;
        litCategoryDesc.Text = ShowLineBreaks(_CategoryDesc);

        // Don't need all that text in viewstate, just bulks up page
        litCategoryName.EnableViewState = false;
        litCategoryDesc.EnableViewState = false;

        if (KartSettingsManager.GetKartConfig("frontend.category.showimage") == "y")
        {
            UC_ImageView.CreateImageViewer(IMAGE_TYPE.enum_CategoryImage, _CategoryID.ToString(), KartSettingsManager.GetKartConfig("frontend.display.images.thumb.height"), KartSettingsManager.GetKartConfig("frontend.display.images.thumb.width"), "", "", null/* Conversion error: Set to default value for this argument */, _CategoryName);
            phdCategoryImage.Visible = true;
        }
        else
            phdCategoryImage.Visible = false;

        phdCategoryDetails.Visible = true;
    }

    public void HideImage()
    {
    }

    public string ShowLineBreaks(string strInput)
    {
        string strOutput = strInput;
        if (Strings.InStr(strInput, "<") > 0 & Strings.InStr(strInput, ">") > 0)
        {
        }
        else
        {
            strOutput = Strings.Replace(strOutput, Constants.vbCrLf, "<br />" + Constants.vbCrLf);
            strOutput = Strings.Replace(strOutput, Constants.vbLf, "<br />" + Constants.vbCrLf);
        }
        return strOutput;
    }
}
