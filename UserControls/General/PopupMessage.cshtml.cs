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

partial class UserControls_General_PopupMessage : System.Web.UI.UserControl
{
    enum POPUP_TYPE
    {
        TEXT = 1,
        IMAGE = 2 // This is deprecated from v2.9, we use the Foundation Clearing lightbox now
,
        PANEL = 3,
        MEDIA = 4
    }

    private string _Title;
    private string _TextMessage;
    private string _ImagePath;
    private string _MediaPath;
    private bool _HideCancel;
    private POPUP_TYPE _PopupType;

    public string SetTitle
    {
        set
        {
            _Title = value;
        }
    }

    public string SetImagePath
    {
        set
        {
            _PopupType = POPUP_TYPE.IMAGE;
            _ImagePath = value;
        }
    }

    public string SetMediaPath
    {
        set
        {
            _PopupType = POPUP_TYPE.MEDIA;
            _MediaPath = value;
        }
    }

    public string SetTextMessage
    {
        set
        {
            _PopupType = POPUP_TYPE.TEXT;
            _TextMessage = value;
        }
    }

    public Panel SetPanel
    {
        set
        {
            _PopupType = POPUP_TYPE.PANEL;
            value.Visible = true;
            updPopup.ContentTemplateContainer.Controls.Remove(pnlMessage);
            updPopup.ContentTemplateContainer.Controls.Add(value);
            popMessage.PopupControlID = value.ID;
        }
    }

    // We preload the popup so we can trigger it later
    // with javascript
    public void PreLoadPopup(Panel pPnl = null/* TODO Change to default(_) if this is not a reference type */)
    {
        if (_PopupType == POPUP_TYPE.TEXT)
        {
            mvwDisplay.SetActiveView(viwText);
            litMessage.Text = _TextMessage;
        }
        else if (_PopupType == POPUP_TYPE.IMAGE)
        {
        }
        else if (_PopupType == POPUP_TYPE.PANEL)
        {
        }
        else if (_PopupType == POPUP_TYPE.MEDIA)
        {

            // Set the popup message to run a special javascript
            // function to clear up vids when cancelled. If not,
            // the video keeps playing even after the poup is closed
            // so you can hear video soundtrack carries on
            popMessage.OnCancelScript = "document.getElementById('media_iframe').src=''";

            // Create iframe
            litIframeMedia.Text += "<input type=\"hidden\" id=\"media_iframe_base_url\" value=\"" + CkartrisBLL.WebShopURL + "MultiMedia.aspx\" />" + Constants.vbCrLf;
            litIframeMedia.Text += "<div id=\"iframe_container\" style=\"margin-right:-50px;height:100%;\">" + Constants.vbCrLf;
            litIframeMedia.Text += "<iframe id=\"media_iframe\" frameBorder=\"0\"" + Constants.vbCrLf;
            litIframeMedia.Text += " src=\"" + CkartrisBLL.WebShopURL + "MultiMedia.aspx\"" + Constants.vbCrLf;
            litIframeMedia.Text += " width=\"100%\"" + Constants.vbCrLf;
            litIframeMedia.Text += " height=\"100%\"" + Constants.vbCrLf;
            litIframeMedia.Text += " class=\"iframe\"" + Constants.vbCrLf;
            litIframeMedia.Text += "></iframe>" + Constants.vbCrLf;
            litIframeMedia.Text += "</div>" + Constants.vbCrLf;

            mvwDisplay.SetActiveView(viwMedia);
            litMessage.Text = _MediaPath;
        }
        litTitle.Text = _Title;
        updPopup.Update();
    }

    // Can trigger from server side, but best to use
    // the PreLoadPopup
    public void ShowPopup(Panel pPnl = null/* TODO Change to default(_) if this is not a reference type */)
    {
        PreLoadPopup(pPnl);
        popMessage.Show();
    }


    public void SetWidthHeight(int pWidth, int pHeight)
    {
        // If values of 999 used, we assume 100% rather than pixel
        if (pWidth == 999 & pHeight == 999)
        {
            pnlMessage.Width = new Unit(100, UnitType.Percentage);
            pnlMessage.Height = new Unit(100, UnitType.Percentage);
        }
        else
        {
            pnlMessage.Width = new Unit(pWidth, UnitType.Pixel);
            pnlMessage.Height = new Unit(pHeight, UnitType.Pixel);
        }
    }
}
