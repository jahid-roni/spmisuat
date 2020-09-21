using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SBM_BLC1.Common;

namespace SBM_WebUI.mp
{
    public partial class ErrorPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnHomePage_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_HOME, false);
        }
    }
}
