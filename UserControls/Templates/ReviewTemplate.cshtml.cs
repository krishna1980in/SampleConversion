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
using CkartrisImages;

/// <summary>

/// ''' User Control Template for the Product Review.

/// ''' </summary>

/// ''' <remarks></remarks>
partial class UserControls_Templates_New_ReviewTemplate : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {

        // ' Creating the POSITIVE/NEGATIVE Stars of the current Review:
        // ' 1. Creating new Image Control
        // ' 2. Calling the SetImage Function to set the POSITIVE star Image. (kartris.vb)
        // ' 3. Adding the newly created Image to the PlaceHolder


        short numRating = 5; // Default to highest

        // Try to see if there is a value stored. Occasionally
        // upgrades can lead to null values, we just need
        // to handle those without the page crashing
        try
        {
            numRating = System.Convert.ToInt16(litReviewRatingHidden.Text);
        }
        catch (Exception ex)
        {
        }

        // ' POSITIVE STARS
        for (short i = 1; i <= numRating; i++)
        {
            Image imgReviewYes = new Image();
            SetImage(imgReviewYes, IMAGE_TYPE.enum_OtherImage, "reviewYes");
            if (!File.Exists(System.Web.UI.UserControl.Server.MapPath(imgReviewYes.ImageUrl)))
                imgReviewYes.Visible = false;
            imgReviewYes.AlternateText = "*";
            phdStars.Controls.Add(imgReviewYes);
        }

        // ' NEGATIVE STARS 
        for (short i = numRating + 1; i <= 5; i++)
        {
            Image imgReviewNo = new Image();
            SetImage(imgReviewNo, IMAGE_TYPE.enum_OtherImage, "reviewNo");
            if (!File.Exists(System.Web.UI.UserControl.Server.MapPath(imgReviewNo.ImageUrl)))
                imgReviewNo.Visible = false;
            imgReviewNo.AlternateText = "-";
            phdStars.Controls.Add(imgReviewNo);
        }

        // ' Formats the creation date of the review.
        // ' A CONFIG Setting Key 'frontend.reviews.dateformat' holds the default reviews date's format.
        litReviewDateCreated.Text = Format((DateTime)litReviewDateCreated.Text, KartSettingsManager.GetKartConfig("frontend.reviews.dateformat"));
    }
}
