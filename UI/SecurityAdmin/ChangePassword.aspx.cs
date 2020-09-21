/*
 * File name            : ChangePassword
 * Author               : A.K.M. Tanjil Hossain,
 *                      : A.K.M. Zahidul Quaium,
 *                      : Tanvir Alam
 *                      : Jerin Afsana
 * Date                 : Sep 01,2014
 * Version              : 1.0
 *
 * Description          : To manage Password
 *
 * Modification history :
 * Name                         Date                            Desc
 * Tanvir Alam                Sep 01,2014                Business implementation 
 * A.K.M. Zahidul Quaium        February 02,2012                Business implementation              
 * Jerin Afsana                 April    02,2012                Business implementation              
 * Copyright (c) 2012: Softcell Solution Ltd
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SBM_BLC1.Common;
using SBM_BLC1.Entity.SecurityAdmin;
using SBM_BLC1.SecurityAdmin;
using SBM_BLC1.Entity.Common;

namespace SBM_WebUI.mp
{
    public partial class ChangePassword : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Constants.SES_USER_CONFIG] != null)
            {
                if (!Page.IsPostBack)
                {
                    InitializeData();
                    //This is for Page Permission
                    CheckPermission chkPer = new CheckPermission();
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_SA.CHANGE_PASSWORD))
                    {
                        Response.Redirect(Constants.PAGE_ERROR, false);
                    }
                    //End Of Page Permission
                }
            }
            else
            {
                Response.Redirect(Constants.PAGE_LOGIN, false);
            }
        }

        #region InitializeData
        private void InitializeData()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            txtLoginUserID.Text = oConfig.UserName.ToString();
        }
        #endregion InitializeData


        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsIsDigitValidPassword(string password)
        {
            return password.Any(c => IsDigit(c));
        }

        public void PopErrorMsgAction(string sType)
        {
            // nothing            
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNewPassword.Text) && !string.IsNullOrEmpty(txtOldPassword.Text) && !string.IsNullOrEmpty(txtConfirmPassword.Text))
            {
                string sErrorMessage = string.Empty;
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                if (oConfig != null)
                {
                    // need to check first
                    //This block is used for checking the Security Policy
                    SecurityPolicyDAL oSpDAL = new SecurityPolicyDAL();
                    Result oPOResult = new Result();
                    oPOResult = oSpDAL.LoadApprovedData();
                    SecurityPolicy oSp = null;
                    bool status = true;

                    if (oPOResult.Status)
                    {
                        oSp = (SecurityPolicy)oPOResult.Return;
                        if (oSp != null)
                        {
                            int iMinLength = oSp.MinPassLength;
                            bool bAlpNu = oSp.IsForceAlphaNumericPass;

                            if (txtNewPassword.Text.Length < iMinLength)
                            {
                                sErrorMessage += "<LI>Password is too short.</LI>";
                                status = false;
                            }
                            if (bAlpNu)
                            {
                                if (IsIsDigitValidPassword(txtNewPassword.Text) == false)
                                {
                                    sErrorMessage += "<LI>Password must be alpha numeric.</LI>";
                                    status = false;
                                }
                            }
                        }
                    }
                    //This block is used for checking the Security Policy

                    if (status == true)
                    {
                        User oUesr = new User(txtLoginUserID.Text);
                        oUesr.Password = txtNewPassword.Text;
                        UserDAL oUserDAL = new UserDAL();
                        Result oResult = (Result)oUserDAL.ChangePassword(oConfig.UserID, oConfig.UserName, txtOldPassword.Text, txtNewPassword.Text);
                        if (oResult.Status)
                        {
                            ucMessage.OpenMessage(oResult.Message, Constants.MSG_TYPE_SUCCESS);
                        }
                        else
                        {
                            ucMessage.OpenMessage(oResult.Message, Constants.MSG_TYPE_ERROR);
                        }
                    }
                    else
                    {
                        ucMessage.OpenMessage("<UI>" + sErrorMessage + "</UI>", Constants.MSG_TYPE_ERROR);
                    }
                }
            }
        }
    }
}
