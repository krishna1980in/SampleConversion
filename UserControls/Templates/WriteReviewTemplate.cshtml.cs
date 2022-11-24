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
using CkartrisDisplayFunctions;

/// <summary>

/// ''' User Control Template for the Write Review Form, used in ProductReview.ascx

/// ''' </summary>

/// ''' <remarks>By Mohammad</remarks>
partial class WriteReviewTemplate : System.Web.UI.UserControl
{
    public event ReviewCreatedEventHandler ReviewCreated;

    public delegate void ReviewCreatedEventHandler();

    private int _ProductID;
    private string _ProductName;

    public string ProductName
    {
        get
        {
            return _ProductName;
        }
    }

    // ' Creates/Loads the important info. about writing reviews.
    public void CreateWriter(int pProductID, string pProductName)
    {
        _ProductID = pProductID;
        _ProductName = pProductName;

        litProductName.Text = pProductName;
    }

    // ' Fills the Rating's DropDownList with the rating values.
    public void FillRatingValues()
    {
        for (short i = 1; i <= System.Convert.ToInt16(KartSettingsManager.GetKartConfig("frontend.reviews.ratings.maxvalue")); i++)
            ddlRating.Items.Add(new ListItem(i, i));
    }

    // ' INSERT TO DB, adding the new review to the Review Table.
    protected void btnAddReview_Click(object sender, System.EventArgs e)
    {
        System.Web.UI.Control.Page.Validate("ReviewForm");
        if (System.Web.UI.Control.Page.IsValid && ajaxNoBotReview.IsValid)
        {
            // ' Calling the INSERT STATEMENT of the Review's Table
            // ' Sets the result of the INSERT, by getting the value from the Cached Resources
            if (ReviewsBLL.AddNewReview(_ProductID, System.Web.UI.UserControl.Session["LANG"], StripHTML(txtTitle.Text), StripHTML(txtReviewText.Text), System.Convert.ToInt16(ddlRating.SelectedValue), StripHTML(txtName.Text), StripHTML(txtEmail.Text), StripHTML(txtLocation.Text), 0, ""))
                litResult.Text = Strings.Replace(System.Web.UI.TemplateControl.GetGlobalResourceObject("Reviews", "ContentText_ReviewAdded3"), "[itemname]", ProductName);
            else
                litResult.Text = System.Web.UI.TemplateControl.GetGlobalResourceObject("Kartris", "ContentText_Error");

            ClearForm(); // ' Clear the form for new reviews.
            mvwWriting.SetActiveView(viwWritingResult);   // ' Activates the Result View.
        }
        updReviewMain.Update();
    }

    // ' Clears the FORM Controls, for new writting.
    public void ClearForm()
    {
        ddlRating.SelectedIndex = 0;
        txtTitle.Text = null;
        txtReviewText.Text = null;
        txtName.Text = null;
        txtLocation.Text = null;
        txtEmail.Text = null;
    }

    // ' Activates the Writting FORM from the result view.
    protected void BtnBack_Click(object sender, System.EventArgs e)
    {
        mvwWriting.SetActiveView(viwWritingForm);
    }

    // ' "Enter your review below for [itemname]"
    protected void Page_Load(object sender, System.EventArgs e)
    {
        litAddRevew.Text = Replace(System.Web.UI.TemplateControl.GetGlobalResourceObject("Reviews", "ContentText_AddReview"), "[itemname]", litProductName.Text);

        if (!System.Web.UI.Control.Page.IsPostBack)
        {
            // Filling the Rating's DropDownList with the allowed values to be used(if the rating is enabled).
            ddlRating.Items.Clear();
            switch (KartSettingsManager.GetKartConfig("frontend.reviews.ratings.enabled"))
            {
                case "y":
                    {
                        ddlRating.Items.Add(new ListItem("-", ""));
                        FillRatingValues();
                        break;
                    }

                case "r":
                    {
                        FillRatingValues();
                        break;
                    }

                case "n":
                    {
                        ddlRating.Enabled = false;
                        break;
                    }
            }
        }
    }
}
