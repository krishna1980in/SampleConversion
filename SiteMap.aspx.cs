using System;
using System.Web.UI;

namespace EmployeeManagementSystem
{

    partial class SiteMap : PageBaseClass
    {
        public SiteMap()
        {
            this.Load += Page_Load;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            // New usercontrol
            Control objCategoryMenuUserControl;
            objCategoryMenuUserControl = LoadControl("~/UserControls/Skin/CategoryMenuAccordion.ascx");

            // Add appropriate menu control
            phdCategoryMenu.Controls.Add(objCategoryMenuUserControl);
        }

    }
}