partial class UserControls_Skin_LoginStatus : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {

        // Hide support tickets if not activated from config
        if (KartSettingsManager.GetKartConfig("frontend.supporttickets.enabled") != "y")
            lnkSupportTickets.Visible = false;

        // Show main login and status links
        if (System.Web.UI.Control.Page.User.Identity.IsAuthenticated)
        {
            phdLoggedIn.Visible = true;
            lnkMyAccount.ToolTip = System.Web.UI.Control.Page.User.Identity.Name;
        }
        else
            phdLoggedOut.Visible = true;
    }
}
