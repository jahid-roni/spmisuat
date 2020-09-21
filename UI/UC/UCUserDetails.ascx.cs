using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SBM_BLC1.Configuration;
using SBM_BLC1.Common;
using SBM_BLC1.Entity.Common;

namespace SBM_WebUI.UI.UC
{
    public partial class UCUserDetails : System.Web.UI.UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void Reset()
        {
            txtCheckerComments.Text = string.Empty;
        }

        public void ResetData()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (oConfig != null)
            {
                txtMakerId.Text = oConfig.UserName;
            }
            txtMakeDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtCheckerId.Text = string.Empty;
            txtCheckDate.Text = string.Empty;
            txtCheckerComments.Text = string.Empty;
        }

        public UserDetails UserDetail
        {
            get
            {
                UserDetails oUserDetails = new UserDetails(); 

                oUserDetails.MakerID = txtMakerId.Text;
                oUserDetails.MakeDate = Date.GetDateTimeByString(txtMakeDate.Text.ToString());
                oUserDetails.CheckerID = txtCheckerId.Text;
                oUserDetails.CheckDate = Date.GetDateTimeByString(txtCheckDate.Text.ToString()); 
                oUserDetails.CheckerComment = txtCheckerComments.Text;
                
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                if (oConfig != null)
                {
                    oUserDetails.BankID = oConfig.BankCodeID;
                    oUserDetails.Division = oConfig.DivisionID;
                }

                return oUserDetails;
            }
            set
            {
                UserDetails oUserDetails = (UserDetails)value;

                if (!string.IsNullOrEmpty(oUserDetails.MakerID))
                {
                    txtMakerId.Text = oUserDetails.MakerID;
                    
                    if (oUserDetails.MakeDate != null)
                    {
                        txtMakeDate.Text = oUserDetails.MakeDate.ToString(Constants.DATETIME_FORMAT);
                    }
                }
                else
                {
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

                    if (oConfig != null)
                    {
                        txtMakerId.Text = oConfig.UserName;
                    }

                    txtMakeDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
                }                
               
                txtCheckerId.Text = oUserDetails.CheckerID;

                if (oUserDetails.CheckDate.Year != 1 && oUserDetails.CheckDate.Year != 1900)
                {
                    txtCheckDate.Text = oUserDetails.CheckDate.ToString(Constants.DATETIME_FORMAT);
                }
                else
                {
                    txtCheckDate.Text = "";
                }
                txtCheckerComments.Text = oUserDetails.CheckerComment;
            }
        }
    }
}