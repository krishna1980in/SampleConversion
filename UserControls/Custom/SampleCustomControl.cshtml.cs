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
using KartSettingsManager;

/// <summary>

/// ''' Sample Custom Control - must always inherit KartrisClasses.CustomProductControl

/// ''' Has 2 dropdowns - width and length, price is just calculated by multiplying selected width and length

/// ''' </summary>

/// ''' <remarks></remarks>
public class SampleCustomControl : KartrisClasses.CustomProductControl
{
    private string _ParameterValues = "";
    private string _ItemDescription = "";
    private double _ItemPrice = -1;


    /// <summary>
    ///     ''' Calculates the price that will result from processing the given parameter values
    ///     ''' Primarily used to check if stored price in the db/basket is correct, used before checkout code processes an order
    ///     ''' </summary>
    ///     ''' <param name="ParameterValues">Comma separated list of parameters to be computed</param>
    ///     ''' <returns></returns>
    ///     ''' <remarks>returns -1 if parameters are invalid</remarks>
    public override double CalculatePrice(string ParameterValues)
    {
        double _CalculatedPrice;
        try
        {
            string[] arrParameters = Strings.Split(ParameterValues, ",");
            // Width and Length for this custom control so we're expecting 2 indexes
            _CalculatedPrice = arrParameters[0] * arrParameters[1];
        }
        catch (Exception ex)
        {
            _CalculatedPrice = -1;
        }

        return _CalculatedPrice;
    }

    /// <summary>
    ///     ''' Instructs the user control to compute and populate the properties with the correct values based on the selected options in the user control
    ///     ''' This function must be called before retrieving the 3 properties - ParameterValues, ItemDescription and ItemPrice
    ///     ''' </summary>
    ///     ''' <returns></returns>
    ///     ''' <remarks>should return either "success" or an error message</remarks>
    public override string ComputeFromSelectedOptions()
    {
        try
        {
            // Generate an item description string based on the selected options in the control
            _ItemDescription = ddlWidth.SelectedValue + " x " + ddlLength.SelectedValue + " item";

            // Prepare the comma separated list of parameter values based on the selected options in the control
            _ParameterValues = ddlWidth.SelectedValue + "," + ddlLength.SelectedValue;

            // Formula to compute the item price - this varies from one custom product control to another
            _ItemPrice = ddlLength.SelectedValue * ddlWidth.SelectedValue;

            litCustomPrice.Visible = true;
            lblCustomPrice.Visible = true;
            litCustomPrice.Text = CurrenciesBLL.FormatCurrencyPrice(Session("CUR_ID"), _ItemPrice);

            return "success";
        }
        catch (Exception ex)
        {
            litCustomPrice.Visible = false;
            lblCustomPrice.Visible = false;
            // An error occurred while trying to compute item price, return the error message to the user
            return ex.Message;
        }
    }

    /// <summary>
    ///     ''' Returns the item description based on the selected options in the control
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public override string ItemDescription
    {
        get
        {
            return _ItemDescription;
        }
    }

    /// <summary>
    ///     ''' Returns the computed price from the selected options in the control
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public override double ItemPrice
    {
        get
        {
            return _ItemPrice;
        }
    }

    /// <summary>
    ///     ''' Returns the comma separated list of values from the selected options in the control
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public override string ParameterValues
    {
        get
        {
            return _ParameterValues;
        }
    }


    /// <summary>
    ///     ''' Handle SelectedIndexChanged event for both width and length dropdowns
    ///     ''' </summary>
    ///     ''' <param name="sender"></param>
    ///     ''' <param name="e"></param>
    ///     ''' <remarks></remarks>
    protected void AnyDropdown_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        ComputeFromSelectedOptions();
    }
    /// <summary>
    ///     ''' Compute price based on initial values when control is loaded for the first time
    ///     ''' </summary>
    ///     ''' <param name="sender"></param>
    ///     ''' <param name="e"></param>
    ///     ''' <remarks></remarks>
    protected void Me_Load(object sender, System.EventArgs e)
    {
        if (!IsPostBack)
            ComputeFromSelectedOptions();
    }
}
