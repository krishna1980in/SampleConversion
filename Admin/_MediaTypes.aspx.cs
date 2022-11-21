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
using System.Data;
using CkartrisDataManipulation;
using CkartrisEnumerations;
using CkartrisDisplayFunctions;
using CkartrisImages;
using KartSettingsManager;

partial class admin_MediaTypes : _PageBaseClass
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Title = GetGlobalResourceObject("_Kartris", "BackMenu_MediaTypes") + " | " + GetGlobalResourceObject("_Kartris", "ContentText_KartrisName");
        if (!Page.IsPostBack)
            LoadMediaTypes();
    }

    public void LoadMediaTypes()
    {
        DataTable tblMediaTypes = MediaBLL._GetMediaTypes();
        gvwMediaTypes.DataSource = tblMediaTypes;
        gvwMediaTypes.DataBind();
        mvwMedia.SetActiveView(vwTypeList);
        updMediaTypes.Update();
    }

    protected void gvwMediaTypes_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EditMediaType")
            EditMediaType(e.CommandArgument);
    }

    public void EditMediaType(int numMediaTypeID)
    {
        DataRow drType = MediaBLL._GetMediaTypesByID(numMediaTypeID).Rows(0);
        litMediaTypeID.Text = FixNullFromDB(drType["MT_ID"]);
        txtMediaType.Text = FixNullFromDB(drType["MT_Extension"]);
        txtMediaType.ReadOnly = true;
        txtDefaultHeight.Text = FixNullFromDB(drType["MT_DefaultHeight"]);
        txtDefaultWidth.Text = FixNullFromDB(drType["MT_DefaultWidth"]);
        txtDefaultParameters.Text = FixNullFromDB(drType["MT_DefaultParameters"]);
        chkDownloadable.Checked = System.Convert.ToBoolean(FixNullFromDB(drType["MT_DefaultisDownloadable"]));
        chkEmbed.Checked = System.Convert.ToBoolean(FixNullFromDB(drType["MT_Embed"]));
        chkInline.Checked = System.Convert.ToBoolean(FixNullFromDB(drType["MT_Inline"]));
        if (txtDefaultWidth.Text == "999")
        {
            txtDefaultHeight.Text = 999;
            chkFullScreen.Checked = true;
            txtDefaultWidth.Enabled = false;
            txtDefaultHeight.Enabled = false;
        }
        PreviewIcon();
        mvwMedia.SetActiveView(vwEditType);
        updMediaTypes.Update();
    }

    protected void lnkUploadIcon_Click(object sender, System.EventArgs e)
    {
        _UC_IconUploaderPopup.OpenFileUpload();
    }

    public int GetMediaTypeID()
    {
        int numMediaTypeID = 0;
        try
        {
            numMediaTypeID = System.Convert.ToInt32(litMediaTypeID.Text);
        }
        catch (Exception ex)
        {
            numMediaTypeID = 0;
        }
        return numMediaTypeID;
    }

    protected void _UC_IconUploaderPopup_UploadClicked()
    {
        if (_UC_IconUploaderPopup.HasFile)
        {
            string strFileExt = Path.GetExtension(_UC_IconUploaderPopup.GetFileName());
            string strMessage = null;
            if (IsValidFileType(strFileExt, ref strMessage, true))
            {
                phdUploadError.Visible = false; litUploadError.Text = null;
                string strIconsFolder = "~/Images/MediaTypes/";
                string strFileName = _UC_IconUploaderPopup.GetFileName;
                string strExtension = Strings.Mid(strFileName, strFileName.LastIndexOf(".") + 1);
                litOriginalIconName.Text = strFileName;
                if (GetMediaTypeID() == 0)
                {
                    string strTempFolder = strIconsFolder + "temp/";
                    litTempIconName.Text = Guid.NewGuid().ToString() + strExtension;
                    if (!Directory.Exists(Server.MapPath(strTempFolder)))
                        Directory.CreateDirectory(Server.MapPath(strTempFolder));
                    _UC_IconUploaderPopup.SaveFile(Server.MapPath(strTempFolder + litTempIconName.Text));
                    imgMediaIcon.ImageUrl = strTempFolder + litTempIconName.Text + "?nocache=" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                    imgMediaIcon.Visible = true; lnkUploadIcon.Visible = false;
                    lnkRemoveIcon.Visible = true;
                }
                else
                {
                    if (!Directory.Exists(Server.MapPath(strIconsFolder)))
                        Directory.CreateDirectory(Server.MapPath(strIconsFolder));
                    string strIconName = txtMediaType.Text + strExtension;
                    var strExistingIcon = GetIconName(txtMediaType.Text);
                    if (strExistingIcon != null)
                        File.Delete(Server.MapPath(strIconsFolder + strExistingIcon));
                    _UC_IconUploaderPopup.SaveFile(Server.MapPath(strIconsFolder + strIconName));
                    PreviewIcon();
                }
            }
            else
            {
                phdUploadError.Visible = true; litUploadError.Text = strMessage;
            }
        }
    }

    public string GetIconName(string strMediaType)
    {
        string strImageName = strMediaType;
        string strIconsFolder = "~/Images/MediaTypes/";
        DirectoryInfo dirMediaIconImages = new DirectoryInfo(Server.MapPath(strIconsFolder));
        foreach (FileInfo objFile in dirMediaIconImages.GetFiles())
        {
            if (objFile.Name.StartsWith(strImageName + "."))
                return objFile.Name;
        }
        return null;
    }

    public void PreviewIcon()
    {
        var strExistingIcon = GetIconName(txtMediaType.Text);
        if (strExistingIcon != null)
        {
            imgMediaIcon.ImageUrl = "~/Images/MediaTypes/" + strExistingIcon + "?nocache=" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            imgMediaIcon.Visible = true; lnkUploadIcon.Visible = false;
            lnkRemoveIcon.Visible = true;
        }
        else
        {
            imgMediaIcon.ImageUrl = null;
            imgMediaIcon.Visible = false; lnkUploadIcon.Visible = true;
            lnkRemoveIcon.Visible = false;
        }
    }

    protected void lnkBtnCancel_Click(object sender, System.EventArgs e)
    {
        mvwMedia.SetActiveView(vwTypeList);
        updMediaTypes.Update();
    }

    protected void lnkBtnNewType_Click(object sender, System.EventArgs e)
    {
        PrepareNewType();
    }

    public void PrepareNewType()
    {
        litMediaTypeID.Text = "0";
        txtMediaType.Text = null;
        txtMediaType.ReadOnly = false;
        txtDefaultHeight.Text = null;
        txtDefaultWidth.Text = null;
        txtDefaultParameters.Text = null;
        chkDownloadable.Checked = false;
        chkEmbed.Checked = false;
        chkInline.Checked = false;
        imgMediaIcon.ImageUrl = null;
        imgMediaIcon.Visible = false; lnkUploadIcon.Visible = true;
        lnkRemoveIcon.Visible = false;
        mvwMedia.SetActiveView(vwEditType);
        updMediaTypes.Update();
    }

    protected void lnkBtnSave_Click(object sender, System.EventArgs e)
    {
        if (GetMediaTypeID() == 0)
            SaveMediaType(DML_OPERATION.INSERT);
        else
            SaveMediaType(DML_OPERATION.UPDATE);
    }

    private void SaveMediaType(DML_OPERATION enumOperation)
    {
        string strMediaTypeName;
        int numDefaultHeight;
        int numDefaultWidth;
        bool blnDownloadable;
        string strDefaultParameters;
        bool blnEmbed, blnInline;
        string strMessage = "";

        strMediaTypeName = txtMediaType.Text;
        numDefaultHeight = txtDefaultHeight.Text;
        numDefaultWidth = txtDefaultWidth.Text;
        strDefaultParameters = txtDefaultParameters.Text;
        blnDownloadable = chkDownloadable.Checked;
        blnEmbed = chkEmbed.Checked;
        blnInline = chkInline.Checked;

        switch (enumOperation)
        {
            case object _ when DML_OPERATION.UPDATE:
                {
                    if (!MediaBLL._UpdateMediaType(numDefaultHeight, numDefaultWidth, strDefaultParameters, blnDownloadable, blnEmbed, blnInline, GetMediaTypeID, strMessage))
                    {
                        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                        return;
                    }
                    (Skins_Admin_Template)this.Master.DataUpdated();
                    LoadMediaTypes();
                    break;
                }

            case object _ when DML_OPERATION.INSERT:
                {
                    if (!MediaBLL._AddMediaType(strMediaTypeName, numDefaultHeight, numDefaultWidth, strDefaultParameters, blnDownloadable, blnEmbed, blnInline, strMessage))
                    {
                        _UC_PopupMsg.ShowConfirmation(MESSAGE_TYPE.ErrorMessage, strMessage);
                        return;
                    }
                    // ' Icon Saving
                    string strMediaIconsFolder = "~/Images/MediaTypes/";
                    string strTempFolder = strMediaIconsFolder + "temp/";
                    string strIconName = null;
                    if (!string.IsNullOrEmpty(litTempIconName.Text))
                    {
                        try
                        {
                            string strExtension = Mid(litTempIconName.Text, litTempIconName.Text.LastIndexOf(".") + 1);
                            if (File.Exists(Server.MapPath(strMediaIconsFolder + strMediaTypeName + strExtension)))
                                File.Delete(Server.MapPath(strMediaIconsFolder + strMediaTypeName + strExtension));
                            File.Move(Server.MapPath(strTempFolder + litTempIconName.Text), Server.MapPath(strMediaIconsFolder + strMediaTypeName + strExtension));
                            strIconName = strMediaTypeName + strExtension;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    (Skins_Admin_Template)this.Master.DataUpdated();
                    LoadMediaTypes();
                    break;
                }
        }
    }

    protected void lnkRemoveIcon_Click(object sender, System.EventArgs e)
    {
        string strMediaIconsFolder = "~/Images/MediaTypes/";
        string strImageURL = imgMediaIcon.ImageUrl;
        if (strImageURL.Contains("/temp/"))
            strMediaIconsFolder += "temp/";
        string strFileName = Strings.Mid(strImageURL, strImageURL.LastIndexOf("/") + 2);
        strFileName = Strings.Mid(strFileName, 1, strFileName.LastIndexOf("?"));
        if (File.Exists(Server.MapPath(strMediaIconsFolder + strFileName)))
            File.Delete(Server.MapPath(strMediaIconsFolder + strFileName));
        imgMediaIcon.ImageUrl = null;
        imgMediaIcon.Visible = false; lnkUploadIcon.Visible = true;
        lnkRemoveIcon.Visible = false;
    }

    public bool IsValidFileType(string strFileExt, ref string strMessage, bool blnCheckImageTypes)
    {
        string[] arrExcludedFileTypes = ConfigurationManager.AppSettings("ExcludedUploadFiles").ToString().Split(",");
        for (int i = 0; i <= arrExcludedFileTypes.GetUpperBound(0); i++)
        {
            if (Strings.Replace(strFileExt.ToLower(), ".", "") == arrExcludedFileTypes[i].ToLower())
            {
                // Banned file type, don't upload
                // Log error so attempts can be seen in logs
                CkartrisFormatErrors.LogError("Attempt to upload a file of type: " + arrExcludedFileTypes[i].ToLower());
                strMessage = "It is not permitted to upload files of this type. Change 'ExcludedUploadFiles' in the web.config if you need to upload this file.";
                return false;
            }
        }

        if (blnCheckImageTypes)
        {
            // This is a softer check, it checks images are of an acceptable
            // type. The security check on file type above will overrule
            // this 'allow' list here.
            string[] arrAllowedImageTypes = KartSettingsManager.GetKartConfig("backend.imagetypes").Split(",");
            for (int i = 0; i <= arrAllowedImageTypes.GetUpperBound(0); i++)
            {
                if (strFileExt.ToLower() == arrAllowedImageTypes[i].ToLower())
                    return true;
            }

            strMessage = GetGlobalResourceObject("_Kartris", "ContentText_ErrorChkUploadFileType");
            return false;
        }

        return true;
    }

    protected void _UC_IconUploaderPopup_NeedCategoryRefresh()
    {
        (Skins_Admin_Template)this.Master.LoadCategoryMenu();
    }

    protected void chkFullScreen_Checked(object sender, System.EventArgs e)
    {
        if (chkFullScreen.Checked)
        {
            txtDefaultWidth.Text = 999;
            txtDefaultHeight.Text = 999;
            txtDefaultWidth.Enabled = false;
            txtDefaultHeight.Enabled = false;
        }
        else
        {
            txtDefaultWidth.Enabled = true;
            txtDefaultHeight.Enabled = true;
        }
        updMediaTypes.Update();
    }
}
