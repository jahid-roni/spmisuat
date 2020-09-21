using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SBM_BLC1.Common;

namespace SBM_WebUI.UI.Common
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Constants.SES_USER_CONFIG] != null)
            {
                if (!Page.IsPostBack)
                {
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    Label label = (Label)Master.FindControl("lblUserName");
                    string sUserName = "";
                    if(oConfig.LoginUser.FirstName.Length>0)
                    {
                        sUserName = oConfig.LoginUser.FirstName.Trim();
                    }
                    if(oConfig.LoginUser.LastName.Length>0)
                    {
                        if (sUserName.Length > 0)
                        {
                            sUserName += " ";
                        }
                        sUserName += oConfig.LoginUser.LastName.Trim();
                    }
                    label.Text = sUserName;
                }
            }
            else
            {
                Response.Redirect(Constants.PAGE_LOGIN, false);
            }
        }
    }
}
