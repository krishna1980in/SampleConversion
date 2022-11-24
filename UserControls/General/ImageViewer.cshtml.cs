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
using System.Web.UI;
using CkartrisEnumerations;
using CkartrisImages;

/// <summary>

/// ''' User Control used to view images.

/// ''' Can generate a single image and the enclosing

/// ''' sized div, or an image gallery effect that

/// ''' includes javascript to dynamically swamp the

/// ''' main image when the small thumbs are clicked.

/// ''' </summary>

/// ''' <remarks>By Paul</remarks>
partial class ImageViewer : System.Web.UI.UserControl
{
    private IMAGE_TYPE _eImageType;
    private string _strFolderPath;
    private string _strImagesDirName;
    private SmallImagesType _eViewType;
    private string _strItemType;
    private bool _blnPlaceHolder = false;
    private bool _blnFoundImage = true;
    public bool _blnLargeViewClickable = false;
    public bool _blnVersionImages = false;

    // We can use this on pages with the image viewer
    // to switch off the large link when no image
    // found (even if placeholder shows)
    public bool FoundImage
    {
        get
        {
            return _blnFoundImage;
        }
    }

    // This determines if the main image is clickable to
    // produce a large view. A product image on the product
    // page will be clickable for large view, but the image
    // on the large view itself won't be. So we can also use
    // this property to determine if the image is a preview
    // one on a product, or a large view.
    public bool LargeViewClickable
    {
        set
        {
            _blnLargeViewClickable = value;
        }
    }

    // Is this set of images being called for versions?
    // If so, we need to hide the main image and use some
    // classes in Foundation to get the desired effect
    public bool VersionImages
    {
        set
        {
            _blnVersionImages = value;
        }
    }

    enum SmallImagesType
    {
        enum_ImageButton = 1
    }

    // Folder path
    public string GetFolderPath
    {
        get
        {
            return _strFolderPath;
        }
    }

    public void ClearImages()
    {
        litGalleryThumbs.Text = "";
        litSingleImage.Text = "";
    }

    // this is called from various pages that need to display
    // an image or gallery
    public void CreateImageViewer(IMAGE_TYPE eImageType, string strImagesDirName, int numImageHeight, int numImageWidth, string strHyperlink, string strParentDirName = null, SmallImagesType viewType = SmallImagesType.enum_ImageButton, string strAltText = "")
    {

        // We need to be able to handle showing full size
        // images directly (not via Image.ashx script) if
        // the imageview is called within the new page
        // rather than an AJAX popup. Easiest way is to
        // have zero height and width passed in. Then we
        // set a boolean for convenience to use later.
        bool blnFullSizeImage = false;

        // Set AltText to the property if nothing
        // passed into this optional value
        if ((!string.IsNullOrEmpty(strAltText)) & strAltText != "")
            strAltText = CkartrisDisplayFunctions.RemoveXSS(strAltText);

        if (numImageHeight == 0 & numImageWidth == 0)
        {
            // Set boolean to use later
            blnFullSizeImage = true;

            // Set some default height and width for 'no image'
            numImageHeight = KartSettingsManager.GetKartConfig("frontend.display.images.large.height");
            numImageWidth = KartSettingsManager.GetKartConfig("frontend.display.images.large.width");
        }

        // Declare max height and width to use for surrounding div
        int numImageHeightMax, numImageWidthMax;

        // Set surrounding div to be square
        // same height and width depending on
        // longest dimension of image
        if (numImageHeight > numImageWidth)
        {
            numImageHeightMax = numImageHeight;
            numImageWidthMax = numImageHeight;
        }
        else
        {
            numImageHeightMax = numImageWidth;
            numImageWidthMax = numImageWidth;
        }

        // make sure hyperlink uses webshop URL settings
        if (strHyperlink.Contains("~/"))
            strHyperlink = Replace(strHyperlink, "~/", CkartrisBLL.WebShopURL);

        // product, category or version - set type and path accordingly
        switch (eImageType)
        {
            case object _ when IMAGE_TYPE.enum_ProductImage:
                {
                    _strFolderPath = strProductImagesPath + "/" + strImagesDirName + "/";
                    _strItemType = "p";
                    _blnPlaceHolder = (KartSettingsManager.GetKartConfig("frontend.display.image.products.placeholder") == "y");
                    break;
                }

            case object _ when IMAGE_TYPE.enum_CategoryImage:
                {
                    _strFolderPath = strCategoryImagesPath + "/" + strImagesDirName + "/";
                    _strItemType = "c";
                    _blnPlaceHolder = (KartSettingsManager.GetKartConfig("frontend.display.image.categories.placeholder") == "y");
                    break;
                }

            case object _ when IMAGE_TYPE.enum_VersionImage:
                {
                    _strFolderPath = strProductImagesPath + "/" + strParentDirName + "/" + strImagesDirName + "/";
                    _strItemType = "v";
                    _blnPlaceHolder = (KartSettingsManager.GetKartConfig("frontend.display.image.versions.placeholder") == "y");
                    break;
                }

            case object _ when IMAGE_TYPE.enum_PromotionImage:
                {
                    _strFolderPath = strPromotionImagesPath + "/" + strImagesDirName + "/";
                    _strItemType = "s"; // s for "specials" as p is already taken!
                    _blnPlaceHolder = (KartSettingsManager.GetKartConfig("frontend.display.image.promotions.placeholder") == "y");
                    break;
                }

            default:
                {
                    return;
                }
        }

        _eImageType = eImageType;
        _strImagesDirName = strImagesDirName;
        _eViewType = viewType;

        // find what folder and images we're looking for
        DirectoryInfo dirFolder = new DirectoryInfo(UserControl.Server.MapPath(_strFolderPath));
        FileInfo objFile = null;
        int intIndex = 0;
        string strImageLinkPath = "";
        string strImageFilePath = "";
        string strImageMainView = "";
        string strImageMainViewStart = "";
        string strJSFunctionDifferentiate = "_" + _strItemType + "_" + strImagesDirName; // & "_" & _strDifferentiation
        string strFirstImageName = "";

        // if folder exists
        if (dirFolder.Exists)
        {
            if (dirFolder.GetFiles().Length < 1)
            {
                // =======================================
                // NO IMAGES FOUND
                // But folder found
                // =======================================
                NoImage(numImageHeight, numImageWidth, _blnPlaceHolder, strHyperlink);

                // If we don't want placeholders, then we hide whole 
                // control instead
                if (_blnPlaceHolder == false)
                    this.Visible = false;
            }
            else if ((dirFolder.GetFiles().Length == 1 | strHyperlink != "") & (_blnLargeViewClickable == false & !_blnVersionImages))
            {

                // =======================================
                // SINGLE IMAGE
                // =======================================
                pnlImageViewer.Visible = false;
                pnlSingleImage.Visible = true;

                // loop through all images
                // in folder and form image button links
                foreach (var objFile in dirFolder.GetFiles())
                {
                    intIndex += 1;
                    if (intIndex == 1)
                        strFirstImageName = objFile.Name;
                    switch (_eViewType)
                    {
                        case SmallImagesType.enum_ImageButton:
                            {
                                strImageMainViewStart = "Image.ashx?strFileName=" + objFile.Name + "&amp;strItemType=" + _strItemType + "&amp;numMaxHeight=" + numImageHeight + "&amp;numMaxWidth=" + numImageWidth + "&amp;numItem=" + strImagesDirName + "&amp;strParent=" + strParentDirName;

                                // ---------------------------------------
                                // LARGE VIEW DISPLAY, OR GENERAL IMAGE
                                // DISPLAY WHERE NO POPUP LARGE VIEW IS
                                // REQUIRED
                                // Can be 'New page' or AJAX type
                                // ---------------------------------------
                                if (blnFullSizeImage)
                                {
                                    // ---------------------------------------
                                    // IN 'NEW PAGE' LARGE VIEW MODE
                                    // Direct link to the image itself
                                    // ---------------------------------------
                                    strImageMainView = Strings.Replace(_strFolderPath + objFile.Name, "~/", "");
                                    litSingleImage.Text += "<!-- LARGE VIEW DISPLAY: IN 'NEW PAGE' LARGE VIEW MODE Direct link to the image itself --><img alt=\"" + strAltText + "\" src=\"" + strImageMainView + "\" height=\"" + numImageHeight + "\" width=\"" + numImageWidth + "\" />";
                                }
                                else
                                {
                                    // ---------------------------------------
                                    // IN 'AJAX' LARGE VIEW MODE
                                    // Link to Image.ashx resizer
                                    // ---------------------------------------
                                    litSingleImage.Text += "<!-- IMAGE DISPLAY: Image with no 'large view' click --><div class=\"imageholder singleimage\" style=\"width: " + numImageWidthMax + "px;" + "height: " + numImageHeightMax + "px; max-width: 100%; max-height: 100%;\">";

                                    if (strHyperlink != "")
                                        litSingleImage.Text += "<a href=\"" + strHyperlink + "\">";
                                    litSingleImage.Text += "<img alt=\"" + strAltText + "\" src=\"" + strImageMainViewStart + "\" height=\"" + numImageHeight + "\" width=\"" + numImageWidth + "\" />";
                                    if (strHyperlink != "")
                                        litSingleImage.Text += "</a>";
                                    litSingleImage.Text += "</div>" + Constants.vbCrLf;
                                }

                                break;
                            }
                    }
                    break;
                }
            }
            else
            {

                // =======================================
                // PRODUCT PAGE VIEWER WITH GALLERY
                // =======================================
                pnlImageViewer.Visible = true;
                pnlSingleImage.Visible = false;

                // loop through all images
                // in folder and form image button links
                foreach (var objFile in dirFolder.GetFiles())
                {
                    intIndex += 1;
                    switch (_eViewType)
                    {
                        case SmallImagesType.enum_ImageButton:
                            {

                                // Product main product image
                                // Set link to thumbnail image
                                // This is the same for 'new page' and 'AJAX' settings
                                // because it is always a thumbnail
                                strImageLinkPath = "Image.ashx?strFileName=" + objFile.Name + "&amp;strItemType=" + _strItemType + "&amp;numMaxHeight=" + numImageHeight + "&amp;numMaxWidth=" + numImageWidth + "&amp;numItem=" + strImagesDirName + "&amp;strParent=" + strParentDirName;

                                if (blnFullSizeImage)
                                    // ---------------------------------------
                                    // IN 'NEW PAGE' LARGE VIEW MODE
                                    // Direct link to the image itself
                                    // ---------------------------------------
                                    strImageMainView = Strings.Replace(_strFolderPath + objFile.Name, "~/", "");
                                else
                                    // ---------------------------------------
                                    // IN 'AJAX' LARGE VIEW MODE
                                    // Link to Image.ashx resizer
                                    // ---------------------------------------
                                    strImageMainView = "Image.ashx?strFileName=" + objFile.Name + "&amp;strItemType=" + _strItemType + "&amp;numMaxHeight=" + numImageHeight + "&amp;numMaxWidth=" + numImageWidth + "&amp;numItem=" + strImagesDirName + "&amp;strParent=" + strParentDirName;

                                if (intIndex == 1 & !_blnVersionImages)
                                {
                                    // ---------------------------------------
                                    // SET INITIAL LARGE IMAGE PREVIEW
                                    // Defaults to first image in folder
                                    // ---------------------------------------
                                    strFirstImageName = objFile.Name;
                                    strImageMainViewStart = "Image.ashx?strFileName=" + objFile.Name + "&amp;strItemType=" + _strItemType + "&amp;numMaxHeight=" + numImageHeight + "&amp;numMaxWidth=" + numImageWidth + "&amp;numItem=" + strImagesDirName + "&amp;strParent=" + strParentDirName;
                                }

                                // ---------------------------------------
                                // BUILD UP GALLERY
                                // ---------------------------------------

                                // Foundation Clearing lightbox version
                                strImageFilePath = Strings.Replace(_strFolderPath + objFile.Name, "~/", "");

                                // Only need gallery if more than one image; but
                                // if we don't have this code at all, the main
                                // image doesn't seem to produce a popup. Instead,
                                // we just set it invisible, this seems to hide
                                // it while the popup functionality is retained.
                                if (_blnVersionImages)
                                {
                                    // Version images
                                    if (intIndex == 1)
                                        // First image in version gallery
                                        litGalleryThumbs.Text += "<li class=\"clearing-featured-img\"><a href=\"" + strImageFilePath + "\"><img src=\"" + strImageLinkPath + "\"></a></li>"; // & vbCrLf
                                    else
                                        // Subsequent images in version gallery
                                        litGalleryThumbs.Text += "<li class=\"hide\"><a href=\"" + strImageFilePath + "\"><img src=\"" + strImageLinkPath + "\"></a></li>";// & vbCrLf
                                }
                                else
                                    // product images
                                    if (dirFolder.GetFiles().Length > 1)
                                    // Product image gallery thumbs
                                    litGalleryThumbs.Text += "<li><a href=\"" + strImageFilePath + "\"><img src=\"" + strImageLinkPath + "\"></a></li>"; // & vbCrLf
                                else
                                    // Hide gallery
                                    litGalleryThumbs.Text += "<li style=\"visibility: hidden;\"><a href=\"" + strImageFilePath + "\"><img src=\"" + strImageLinkPath + "\"></a></li>";// & vbCrLf
                                break;
                            }
                    }
                }


                // ---------------------------------------
                // MAIN IMAGE PREVIEW (not large view)
                // ---------------------------------------
                if (KartSettingsManager.GetKartConfig("frontend.display.images.large.linktype") == "n")
                {
                    if (!_blnVersionImages)
                    {
                        // ---------------------------------------
                        // IN 'NEW PAGE' LARGE VIEW MODE
                        // Direct link to the image itself
                        // ---------------------------------------
                        litMainImage.Text += "<!-- MAIN IMAGE PREVIEW: IN 'NEW PAGE' LARGE VIEW MODE Direct link to the image itself --><div class=\"imageholder hand\" >";
                        litMainImage.Text += "<a target=\"_blank\" href=\"" + "LargeImage.aspx?P_ID=" + strImagesDirName + "&blnFullSize=y" + "\">";
                        litMainImage.Text += "<img alt=\"" + strAltText + "\" src=\"" + strImageMainViewStart + "\" height=\"" + numImageHeight + "\" width=\"" + numImageWidth + "\" /></a>";
                        litMainImage.Text += "</div>" + Constants.vbCrLf;
                    }
                    else
                        litMainImage.Text = "";
                }
                else if (!_blnVersionImages)
                {
                    // ---------------------------------------
                    // IN 'AJAX' LARGE VIEW MODE
                    // ---------------------------------------
                    // Fixes old big image staying on top of the new one
                    if (!string.IsNullOrEmpty(litMainImage.Text))
                        litMainImage.Text = "";
                    litMainImage.Text += "<!-- MAIN IMAGE PREVIEW: IN 'AJAX' LARGE VIEW MODE --><div class=\"imageholder hand\" " + "style=\"max-width: 100%; max-height: 100%;\">";
                    litMainImage.Text += "<img alt=\"" + strAltText + "\" src=\"" + strImageMainViewStart + "\" height=\"" + numImageHeight + "\" width=\"" + numImageWidth + "\" />" + Constants.vbCrLf;
                    litMainImage.Text += "</div>";
                }
            }
        }
        else
            // =======================================
            // NO FOLDER FOUND
            // No images, and even no folder for this
            // item
            // =======================================
            NoImage(numImageHeight, numImageWidth, _blnPlaceHolder, strHyperlink);
    }

    // =======================================
    // NO IMAGE FOUND
    // Display placeholder, or hide image
    // =======================================
    public void NoImage(int numImageHeight, int numImageWidth, bool blnPlaceHolder, string strHyperlink)
    {
        _blnFoundImage = false;

        if (strHyperlink != "")
            strHyperlink = "<a href=\"" + strHyperlink + "\">";

        if (blnPlaceHolder)
        {
            // either zero or just one file - no need for gallery
            pnlImageViewer.Visible = false;
            pnlSingleImage.Visible = true;

            // this should for an image placeholder
            string strStartImage = "Image.ashx?strItemType=" + _strItemType + "&amp;numMaxHeight=" + numImageHeight + "&amp;numMaxWidth=" + numImageWidth + "&amp;numItem=0&amp;strParent=0";

            litSingleImage.Text += "<!-- NO IMAGE FOUND --><div class=\"imageviewer\"><div class=\"imageholder singleimage\" style=\"height: " + numImageHeight + "px; max-width: 100%; max-height: 100%;\">";

            // Open hyperlink (if not blank) and show image
            litSingleImage.Text += strHyperlink + "<img alt=\"No image\" src=\"" + strStartImage + "\" />";

            // Close hyperlink if start is not blank
            if (strHyperlink != "")
                litSingleImage.Text += "</a>";

            litSingleImage.Text += "</div></div>" + Constants.vbCrLf;
        }
        else
        {
            pnlImageViewer.Visible = false;
            pnlSingleImage.Visible = false;
        }
    }
}
