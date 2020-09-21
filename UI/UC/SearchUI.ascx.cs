using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;


namespace SBM_WebUI.UI.UC
{
    public partial class SearchUI : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnAction_OnClick(object sender, EventArgs e)
        {
            object[] oMethodParameters = new object[1];
            oMethodParameters[0] = txtName.Text;
            Page.GetType().InvokeMember("SearchUIAction", BindingFlags.InvokeMethod, null, this.Page, oMethodParameters);
        }
    }
}