using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Text;
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Configuration;
using System.Data;
using SBM_BLC1.Common;
using System.Collections;

namespace SBM_WebUI.UI.UC
{
    public partial class UCMessageInfo : System.Web.UI.UserControl
    {
        public string PageCaption = string.Empty;
        public string Caption = string.Empty;
        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                lblPageCaption.Text = "View Details";
                //lblMsg.Text = "Progressing..";
            }
        }

        public void OpenMessage(string sMessage, string sType)
        {
            lblMsg.Text = sMessage;
            if (sType.Equals(Constants.MSG_TYPE_ERROR))
            {
                //imgID.ImageUrl = "~/Images/error.jpg";
                lblMsg.CssClass = "lblError";
            }
            else if (sType.Equals(Constants.MSG_TYPE_SUCCESS))
            {
                //imgID.ImageUrl = "~/Images/success.jpg";
                lblMsg.CssClass = "lblSuccess";
            }
            else if (sType.Equals(Constants.MSG_TYPE_INFO))
            {
                //imgID.ImageUrl = "~/Images/Info.jpg";
                lblMsg.CssClass = "lblInfo";
            }
        }

        public void ResetMessage(string sMessage)
        {
            lblMsg.Text = sMessage;
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            object[] oMethodParameters = new object[1];

            oMethodParameters[0] = hdReturnType.Value;
            try
            {
                Page.GetType().InvokeMember("PopErrorMsgAction", BindingFlags.InvokeMethod, null, this.Page, oMethodParameters);
            }
            catch (TargetInvocationException TIE)
            {
                // nothing.. 
            }
        }
    }
}