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
using System.Web;
using System.Web.UI;

/// <summary>

/// ''' This Class "OptionsContainer.ascx" introduces the User Control that is 

/// '''     responsible to view the Options for a Product.

/// ''' IMPORTANT: You Need to call the "InitializeOption" Method BEFORE using

/// '''     this class(before running the page that holds the "OptionsContainer.ascx" User Control)

/// ''' </summary>

/// ''' <remarks></remarks>
partial class OptionsContainer : System.Web.UI.UserControl
{

    /// <summary>
    ///     ''' The Attributes of the "OptionsContainer" Class
    ///     '''    _ProductID  -> Holds the ProductID of the Options being shown in the Container.
    ///     '''    _LanguageID -> Holds the LanguageID of the Options being shown in the Container.
    ///     '''    _CurrencyID -> Holds the CurrencyID of the Options' Prices being shown in the Container.
    ///     ''' </summary>
    ///     ''' <remarks>By Mohammad</remarks>
    private int _ProductID = -1;
    private short _LanguageID = -1;
    private int _NoOptionsRows = -1;

    // Sometimes we see errors in the page_load, it seems that _ProductID = -1 for some reason
    // If we raise an event, we can try to reset the options in the ProductVersions control,
    // if the event is triggered
    public event SomethingWentWrongEventHandler SomethingWentWrong;

    public delegate void SomethingWentWrongEventHandler();

    // ' If this is true, will use the entered price for each option combination instead of adding the "selected option" price separately
    private bool _UseCombinationPrices = false;
    public bool IsUsingCombinationPrices
    {
        get
        {
            return _UseCombinationPrices;
        }
    }

    /// <summary>
    ///     ''' Loads the "OptionsContainer.ascx" and checks for validity.
    ///     ''' </summary>
    ///     ''' <param name="sender"></param>
    ///     ''' <param name="e"></param>
    ///     ''' <remarks></remarks>
    protected void Page_Load(object sender, System.EventArgs e)
    {
        // ' The next -IF Statement- checks if the Product, Language, and the Currency all are have value,
        // '      otherwise it shows an error message specified in -lblErrorDefaults-
        // If Page.IsPostBack Then
        if (!(_ProductID == -1))
        {
        }
        else
            // lblErrorDefaults.Visible = True
            if (this.Visible)
            SomethingWentWrong?.Invoke();
    }

    /// <summary>
    ///     ''' Returns the ProductID of the OptionContainer
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks>By Mohammad</remarks>
    public int ProductID
    {
        get
        {
            return _ProductID;
        }
    }

    /// <summary>
    ///     ''' Returns the LanguageID of the OptionContainer
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks>By Mohammad</remarks>
    public short LanguageID
    {
        get
        {
            return _LanguageID;
        }
    }

    /// <summary>
    ///     ''' Returns the Number of Rows in the OptionGroup Table
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public int GetNoOfRows
    {
        get
        {
            return _NoOptionsRows;
        }
    }

    /// <summary>
    ///     ''' Creates a new instance of Options with specific Product, Language, and Currency.
    ///     ''' </summary>
    ///     ''' <param name="pProductID">The Options' ProductID to be viewed.</param>
    ///     ''' <param name="pLanguageID">The Options' LanguageID to be viewed.</param>
    ///     ''' <param name="pUseCombinationPrices">If this is true, will use the entered price for each option combination instead of adding the "selected option" price separately.</param>
    ///     ''' <remarks>By Mohammad</remarks>
    ///     ''' 'Public Sub InitializeOption(ByVal productID As Integer, ByVal languageID As Short, ByVal currencyID As Short)
    public void InitializeOption(int pProductID, short pLanguageID, bool pUseCombinationPrices)
    {
        _ProductID = pProductID;
        _LanguageID = pLanguageID;

        _NoOptionsRows = -1;

        _UseCombinationPrices = pUseCombinationPrices;

        CreateProductOptions();
    }

    /// <summary>
    ///     ''' Creates the available options for the Product, and attches them to the Page.
    ///     ''' </summary>
    ///     ''' <remarks>By Mohammad</remarks>
    private void CreateProductOptions()
    {
        phdOptions.Controls.Clear();
        UserControl.Trace.Warn("-------------Container.phdOptions.Controls.Clear()--------------------");

        lblErrorDefaults.Visible = false;

        // ' Creating a table to hold the Options of the specified Product.
        // ' Also calling "VersionsBLL.GetProductOptions" to get the data from database.
        DataTable tblOptions = new DataTable();
        VersionsBLL objVersionsBLL = new VersionsBLL();
        tblOptions = objVersionsBLL.GetProductOptions(_ProductID, _LanguageID);
        _NoOptionsRows = tblOptions.Rows.Count;

        if (_NoOptionsRows == 0)
            return;
        UserControl.Trace.Warn("-------------ProductID is:" + _ProductID + "--------------------");
        Int32 intOptionsGroupID;
        string strOptionsName;
        string strOptionsDesc;
        char chrOptionsType;

        // ' These variables to hold the attributes of the "Options" Class.
        bool blnOptionsMandatory = false;

        // ' The next -For Each- loop is read the available Options for the specified Product.
        DataRow rowOptions;
        foreach (var rowOptions in tblOptions.Rows)
        {

            // ' Reading the data from the -tblOptions- in to some variables.
            intOptionsGroupID = IIf(IsNotNULL(rowOptions("OPTG_ID")), rowOptions("OPTG_ID"), -1);
            strOptionsName = IIf(IsNotNULL(rowOptions("OPTG_Name")), rowOptions("OPTG_Name"), null/* TODO Change to default(_) if this is not a reference type */);
            strOptionsDesc = IIf(IsNotNULL(rowOptions("OPTG_Desc")), rowOptions("OPTG_Desc"), "");
            chrOptionsType = IIf(IsNotNULL(rowOptions("OPTG_OptionDisplayType")), rowOptions("OPTG_OptionDisplayType"), null/* TODO Change to default(_) if this is not a reference type */);
            blnOptionsMandatory = System.Convert.ToBoolean(rowOptions("P_OPTG_MustSelected"));

            // ' If some objects didn't have value then Move to the next Option
            if (intOptionsGroupID == -1 | strOptionsName == null | chrOptionsType == default(Char))
                continue;

            // ' Creating new dataTable to hold the values for specified Option.
            DataTable tblOptionValues = new DataTable();
            tblOptionValues = objVersionsBLL.GetProductOptionValues(_ProductID, _LanguageID, intOptionsGroupID);

            if (chrOptionsType == "l")
            {
                if (tblOptionValues.Rows.Count == 1)
                    chrOptionsType = "c";
                else
                    chrOptionsType = "r";
            }

            // ' Creating new User Control -UC_Options- to hold the values for the specified Option.
            Options UC_Option;

            // ' Loading the User Cotrol "Options.ascx" in the newly created -UC_Options-
            // '      to be shown in the page.
            UC_Option = (Options)TemplateControl.LoadControl("Options.ascx");

            // ' Initializing the -UC_Options-
            UC_Option.CreateOption(intOptionsGroupID, strOptionsName, chrOptionsType, strOptionsDesc, blnOptionsMandatory);
            DataRow rowOptionValues;
            bool blnIsSelectedOption = false;

            // ' The next -For Each- loop is read the values of the specified option
            // '      and to add these values to the User Control -UC_Options-.
            int numSelectedOptions = 0;
            foreach (var rowOptionValues in tblOptionValues.Rows)
            {
                blnIsSelectedOption = System.Convert.ToBoolean(rowOptionValues("P_OPT_Selected"));
                if (blnIsSelectedOption)
                    numSelectedOptions += 1;
                // ' Creating new item(value) for each option.
                {
                    var withBlock = UC_Option;
                    withBlock.NewItem(System.Convert.ToString(rowOptionValues("OPT_Name")), System.Convert.ToString(System.Convert.ToInt32(rowOptionValues("OPT_ID"))), System.Convert.ToDouble(rowOptionValues("P_OPT_PriceChange")), System.Convert.ToBoolean(rowOptionValues("P_OPT_Selected")), !_UseCombinationPrices);
                }
            }
            if (chrOptionsType == "r" && numSelectedOptions == 0)
                UC_Option.SelectFirstOption();

            // ' Adding an Event Handler for the User Control -UC_Options-
            UC_Option.Option_IndexChanged += UCEvent_Options_IndexChaged;

            // ' Adding the newly created User Control -UC_Options- to the -pnlOptions-
            phdOptions.Controls.Add(UC_Option);
            UserControl.Trace.Warn("--------------phdOptions.Controls.Add(UC_Option)------------------------");
        }
    }

    /// <summary>
    ///     ''' Returns True if the specified dataField has a value.
    ///     ''' </summary>
    ///     ''' <param name="pDataField"> The DataBase Field to check. </param>
    ///     ''' <returns></returns>
    ///     ''' <remarks>By Mohammad</remarks>
    private bool IsNotNULL(object pDataField)
    {
        try
        {
            if (pDataField == DBNull.Value)
                return false;
        }
        catch (Exception ex)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    ///     ''' Used to check the selection validation in the options.
    ///     ''' </summary>
    ///     ''' <returns>List of "option's names" for the non-selected options separated by comma (,). </returns>
    ///     ''' <remarks></remarks>
    public string CheckForValidSelection()
    {
        // ' strNotValidStrings will hold the invalid options (as comma separated names).
        string strNotValidStrings = null;
        UserControl.Trace.Warn("---------------------OptionContainer.CheckForValidSelection -- NO. OF Controls is:" + phdOptions.Controls.Count);
        foreach (Control ctrl in phdOptions.Controls)
        {
            try
            {
                if (ctrl.GetType().ToString().ToLower().Contains("options"))
                {
                    // ' Check for validation
                    if (!(Options)ctrl.IsValidSelection())
                    {
                        // ' Not selected option.
                        strNotValidStrings += (Options)ctrl.GetOptionName() + ",";
                        // ' Highlight the Name of the Option, by sending a parameter as NotValid
                        (Options)ctrl.HighlightValidation(false);
                    }
                    else
                        // ' Valid Selection
                        // ' Rest the color of the Option Name, by sending a parameter as Valid
                        (Options)ctrl.HighlightValidation(true);
                }
            }
            catch (Exception ex)
            {
            }
        }
        return strNotValidStrings;
    }

    /// <summary>
    ///     ''' Use this Event to Write the Code that will be run if the Options' Index has been changed.
    ///     ''' </summary>
    ///     ''' <param name="sender">The name of the Options' control.</param>
    ///     ''' <remarks>By Mohammad</remarks>
    protected void UCEvent_Options_IndexChaged(Options sender)
    {
        // ' Write out the code to be run after the index has been changed.
        // ' <HINT>
        // '  -- If You want to catch an Options' Control in the Page and to call a method 
        // '          for that Option' ControlUse the following Code:
        // '  ------------------------------------------------------------------------------
        // '      Dim obj As Object
        // '      For Each obj In pnlOptions.Controls
        // '          If obj.GetType.ToString() = "ASP.ProductOptions_ascx" Then
        // '             CType(obj, Options).<The MethodName>
        // '          End If
        // '      Next
        // '  ------------------------------------------------------------------------------
        // ' </HINT>
        UserControl.Trace.Warn("UC_OptionsEvent IN Container");
        object objOption;
        float numTotalPrice = 0.0F;
        foreach (var objOption in phdOptions.Controls)
        {
            if (objOption.GetType().ToString() == sender.GetType.ToString())
                numTotalPrice += (Options)objOption.GetPriceChange();
        }
        Event_OptionPriceChanged?.Invoke(numTotalPrice);
    }

    public float GetSelectedPrice()
    {
        Options UC_Option;
        UC_Option = (Options)TemplateControl.LoadControl("Options.ascx");

        float numTotalPrice = 0.0F;
        foreach (var objOption in phdOptions.Controls)
        {
            if (objOption.GetType().ToString() == UC_Option.GetType.ToString())
                numTotalPrice += (Options)objOption.GetPriceChange();
        }
        return numTotalPrice;
    }

    public string GetSelectedOptions()
    {
        string strSelectedOptionValues = null;
        int numCounter = 0;

        foreach (Control ctlOption in phdOptions.Controls)
        {
            try
            {
                if (ctlOption.GetType().ToString().ToLower().Contains("options"))
                {
                    if (String.IsNullOrEmpty((Options)ctlOption.GetSelectedValues()))
                        continue;
                    strSelectedOptionValues += (Options)ctlOption.GetSelectedValues() + ",";
                    numCounter += 1;
                }
            }
            catch (Exception ex)
            {
                // ' Means Non Option Control
                UserControl.Trace.Warn("-------------- EXCEPTION IN OptionsContainer.GetSelectedOptions -------------------");
            }
        }
        if (!String.IsNullOrEmpty(strSelectedOptionValues))
        {
            // purge double commas
            while (strSelectedOptionValues.Contains(",,"))
                strSelectedOptionValues = Strings.Replace(strSelectedOptionValues, ",,", ",");
            // remove the starting and ending commas
            if (strSelectedOptionValues.StartsWith(","))
                strSelectedOptionValues = strSelectedOptionValues.TrimStart(",");
            if (strSelectedOptionValues.EndsWith(","))
                strSelectedOptionValues = strSelectedOptionValues.TrimEnd(",");
        }

        return strSelectedOptionValues;
    }

    public event Event_OptionPriceChangedEventHandler Event_OptionPriceChanged;

    public delegate void Event_OptionPriceChangedEventHandler(float sender);
}
