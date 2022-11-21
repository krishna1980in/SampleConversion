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
using CkartrisEnumerations;
using CkartrisDataManipulation;

partial class Admin_Promotions : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Promotions", "PageTitle_Promotions") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");

        if (!Page.IsPostBack)
        {
            if (KartSettingsManager.GetKartConfig("frontend.promotions.enabled") != "y")
            {
                litFeatureDisabled.Text = Replace(GetGlobalResourceObject("_Kartris", "ContentText_DisabledInFrontEnd"), "[name]", GetGlobalResourceObject("_Promotions", "PageTitle_Promotions"));
                phdFeatureDisabled.Visible = true;
            }
            else
                phdFeatureDisabled.Visible = false;
            ShowPromotionList();
        }

        lnkBtnSave.ValidationGroup = LANG_ELEM_TABLE_TYPE.Promotions;
    }

    private void ShowPromotionList()
    {
        using (DataTable tblPromotions = new DataTable())
        {
            DataTable tblPromotionDetails = new DataTable();
            tblPromotionDetails = PromotionsBLL._GetData();

            if (tblPromotionDetails.Rows.Count == 0)
            {
                mvwPromotions.SetActiveView(viwNoItems); return;
            }
            mvwPromotions.SetActiveView(viwPromotionList);

            tblPromotions.Columns.Add(new DataColumn("PROM_ID", Type.GetType("System.Int32")));
            tblPromotions.Columns.Add(new DataColumn("PROM_Text", Type.GetType("System.String")));
            tblPromotions.Columns.Add(new DataColumn("PROM_Start", Type.GetType("System.DateTime")));
            tblPromotions.Columns.Add(new DataColumn("PROM_End", Type.GetType("System.DateTime")));
            tblPromotions.Columns.Add(new DataColumn("PROM_Live", Type.GetType("System.Boolean")));
            tblPromotions.Columns.Add(new DataColumn("PROM_OrderNo", Type.GetType("System.Int32")));
            tblPromotions.Columns.Add(new DataColumn("PROM_MaxQuantity", Type.GetType("System.Byte")));


            foreach (DataRow row in tblPromotionDetails.Rows)
                tblPromotions.Rows.Add(System.Convert.ToInt32(row("PROM_ID")), GetPromotionText(System.Convert.ToInt32(row("PROM_ID"))), (DateTime)row("PROM_StartDate").ToString(), (DateTime)row("PROM_EndDate").ToString(), System.Convert.ToBoolean(row("PROM_Live")), FixNullFromDB(row("PROM_OrderByValue")), FixNullFromDB(row("PROM_MaxQuantity")));

            gvwPromotions.DataSource = tblPromotions;
            gvwPromotions.DataBind();
        }
    }

    private string GetPromotionText(int intPromotionID)
    {
        DataTable tblPromotionParts = new DataTable();
        tblPromotionParts = PromotionsBLL._GetPartsByPromotion(intPromotionID, Session("_LANG"));

        StringBuilder strBldrPromotionText = new StringBuilder("");
        int intTextCounter = 0;
        CategoriesBLL objCategoriesBLL = new CategoriesBLL();
        ProductsBLL objProductsBLL = new ProductsBLL();
        VersionsBLL objVersionsBLL = new VersionsBLL();

        foreach (DataRow row in tblPromotionParts.Rows)
        {
            string strText = row("PS_Text");
            string strStringID = row("PS_ID");
            string strValue = CkartrisDisplayFunctions.FixDecimal(FixNullFromDB(row("PP_Value")));
            string strItemID = FixNullFromDB(row("PP_ItemID"));
            string strItemName = "";
            string strItemLink = "";

            if (strText.Contains("[X]"))
                strText = strText.Replace("[X]", strValue);

            if (strText.Contains("[C]") && strItemID != "")
            {
                strItemName = objCategoriesBLL._GetNameByCategoryID(System.Convert.ToInt32(strItemID), Session("_LANG"));
                strItemLink = " <b><a href='_ModifyCategory.aspx?CategoryID=" + strItemID + "'>" + strItemName + "</a></b>";
                strText = strText.Replace("[C]", strItemLink);
            }

            if (strText.Contains("[P]") && strItemID != "")
            {
                strItemName = objProductsBLL._GetNameByProductID(System.Convert.ToInt32(strItemID), Session("_LANG"));
                strItemLink = " <b><a href='_ModifyProduct.aspx?ProductID=" + strItemID + "'>" + strItemName + "</a></b>";
                strText = strText.Replace("[P]", strItemLink);
            }

            if (strText.Contains("[V]") && strItemID != "")
            {
                int ProductID = objVersionsBLL.GetProductID_s(System.Convert.ToInt32(strItemID));
                strItemName = objVersionsBLL._GetNameByVersionID(System.Convert.ToInt32(strItemID), Session("_LANG"));
                strItemLink = " <b><a href='_ModifyProduct.aspx?ProductID=" + ProductID + "'>" + objProductsBLL._GetNameByProductID(ProductID, 1) + " (" + strItemName + ")</a></b>";
                strText = strText.Replace("[V]", strItemLink);
            }

            if (strText.Contains("[£]"))
                strText = strText.Replace("[£]", CurrenciesBLL.CurrencySymbol(CurrenciesBLL.GetDefaultCurrency));

            intTextCounter += 1;
            if (intTextCounter > 1)
                strBldrPromotionText.Append(", ");
            strBldrPromotionText.Append(strText);
        }
        return strBldrPromotionText.ToString();
    }

    protected void gvwPromotions_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvwPromotions.PageIndex = e.NewPageIndex;
        ShowPromotionList();
    }

    protected void lnkBtnNewPromotion_Click(object sender, System.EventArgs e)
    {
        litPromotionID.Text = "0";
        _UC_EditPromotion.EditPromotion(GetPromotionID());

        StartUpdate();
    }

    protected void gvwPromotions_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EditPromotion")
        {
            gvwPromotions.SelectedIndex = e.CommandArgument % gvwPromotions.PageSize;
            litPromotionID.Text = gvwPromotions.SelectedValue();
            _UC_EditPromotion.EditPromotion(GetPromotionID());

            StartUpdate();
        }
    }


    /// <summary>
    ///     ''' returns the promotion id (saved in a hidden control)
    ///     ''' </summary>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    private int GetPromotionID()
    {
        if (litPromotionID.Text != "")
            return System.Convert.ToInt32(litPromotionID.Text);
        return 0;
    }

    protected void lnkBtnCancel_Click(object sender, System.EventArgs e)
    {
        ShowPromotionList();
    }

    protected void lnkBtnSave_Click(object sender, System.EventArgs e)
    {
        if (_UC_EditPromotion.SaveChanges())
        {
            ShowPromotionList();
            FinishUpdate();

            (Skins_Admin_Template)this.Master.DataUpdated();
        }
    }

    protected void gvwPromotions_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int strPromotionID = (Literal)e.Row.Cells[1].FindControl("litPromotionID").Text;
            if (Directory.Exists(Server.MapPath("~/Images/Promotions/" + strPromotionID)))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Server.MapPath("~/Images/Promotions/" + strPromotionID));
                FileInfo[] fileImage;
                fileImage = dirInfo.GetFiles();
                if (fileImage.Length > 0)
                    (Image)e.Row.Cells[1].FindControl("imgPromotion").ImageUrl = "~/Images/Promotions/" + strPromotionID + "/" + fileImage[0].Name + "?nocache=" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            }

            string strImagePath = (Image)e.Row.Cells[1].FindControl("imgPromotion").ImageUrl;
            if (strImagePath == null || strImagePath == "")
                (Image)e.Row.Cells[1].FindControl("imgPromotion").ImageUrl = "~/Skins/" + CkartrisBLL.Skin(Session("LANG")) + "/Images/no_image_available.png";
            else
            {
                strImagePath = Strings.Left(strImagePath, strImagePath.IndexOf("?"));
                if (!File.Exists(Server.MapPath(strImagePath)))
                    (Image)e.Row.Cells[1].FindControl("imgPromotion").ImageUrl = "~/Skins/" + CkartrisBLL.Skin(Session("LANG")) + "/Images/no_image_available.png";
            }
        }
    }

    protected void lnkBtnBack_Click(object sender, System.EventArgs e)
    {
        FinishUpdate();
    }

    protected void _UC_EditPromotion_NeedCategoryRefresh()
    {
        (Skins_Admin_Template)this.Master.LoadCategoryMenu();
    }

    protected void ShowMasterUpdateMessage()
    {
        (Skins_Admin_Template)this.Master.DataUpdated();
    }

    public void FinishUpdate()
    {
        ShowPromotionList();
        phdViewPromotions.Visible = true;
        phdEditPromotion.Visible = false;
        updViewPromotion.Update();
        updEditPromotion.Update();
    }
    public void StartUpdate()
    {
        phdViewPromotions.Visible = false;
        phdEditPromotion.Visible = true;
        updViewPromotion.Update();
        updEditPromotion.Update();
    }
}
