partial class UserControls_Back_AdminSearch : System.Web.UI.UserControl
{
    protected void btnSearch_Click(object sender, System.EventArgs e)
    {
        System.Web.UI.UserControl.Response.Redirect("~/Admin/_Search.aspx?key=" + System.Web.UI.UserControl.Server.UrlEncode(CkartrisDisplayFunctions.RemoveXSS(txtSearch.Text)) + "&location=" + ddlFilter.SelectedValue);
    }
}
