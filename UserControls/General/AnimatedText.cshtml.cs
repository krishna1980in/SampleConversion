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

partial class UserControls_AnimatedText : System.Web.UI.UserControl
{
    private string _MessageText;

    public string MessageText
    {
        get
        {
            return _MessageText;
        }
        set
        {
            _MessageText = value;
        }
    }

    public void ShowAnimatedText()
    {
        pHolderAnimation.Visible = true;
        if (_MessageText != "")
            litMessage.Text = _MessageText;
        MyPanel_Load(this, new EventArgs());
    }

    public void reset()
    {
        pHolderAnimation.Visible = false;
    }

    protected void MyPanel_Load(object sender, System.EventArgs e)
    {
    }
}
