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
using System.Data;

namespace SBM_WebUI.mp
{
    public partial class SecurityPolicySetup : System.Web.UI.Page
    {
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_SP_ID = "sSPID";
        public const string OBJ_PAGE_ID = "sPageID";

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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_SA.SECURITY_POLICY))
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
            string sSPID = Request.QueryString[OBJ_SP_ID];
            string sPageID = Request.QueryString[OBJ_PAGE_ID];
            if (!string.IsNullOrEmpty(sSPID))
            {
                sSPID = oCrypManager.GetDecryptedString(sSPID, Constants.CRYPT_PASSWORD_STRING);
            }
            if (!string.IsNullOrEmpty(sPageID))
            {
                sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
            }

            if (!string.IsNullOrEmpty(sSPID) && !string.IsNullOrEmpty(sPageID))
            {
                string sOperationType = sPageID.Substring(4, 1);
                if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
                {
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    LoadDataByID(Convert.ToInt32(sSPID));

                    Util.ControlEnabled(ddlRequiredToChangePw, false);
                    Util.ControlEnabled(txtOldPwprohibited, false);
                    Util.ControlEnabled(txtMinimumPwLg, false);
                    Util.ControlEnabled(ddlAlphaNumPw, false);
                    Util.ControlEnabled(txtPwExpDays, false);
                    Util.ControlEnabled(txtDayEarly, false);
                    Util.ControlEnabled(txtWrongLoginTrial, false);                    

                    // button 
                    Util.ControlEnabled(btnReject, true);
                    Util.ControlEnabled(btnApprove, true);
                    Util.ControlEnabled(btnBack, true);

                    Util.ControlEnabled(btnLoad, false);
                    Util.ControlEnabled(btnReset, false);
                    Util.ControlEnabled(btnSave, false);
                    Util.ControlEnabled(btnDelete, false);

                    #region User-Detail.
                    Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);
                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    oUserDetails.CheckerID = oConfig.UserName;
                    oUserDetails.CheckDate = DateTime.Now;
                    ucUserDet.UserDetail = oUserDetails;
                    #endregion User-Detail.

                    fsList.Visible = false;
                }
            }
            else
            {
                Util.ControlEnabled(ddlRequiredToChangePw, true);
                Util.ControlEnabled(txtOldPwprohibited, true);
                Util.ControlEnabled(txtMinimumPwLg, true);
                Util.ControlEnabled(ddlAlphaNumPw, true);
                Util.ControlEnabled(txtPwExpDays, true);
                Util.ControlEnabled(txtDayEarly, true);
                Util.ControlEnabled(txtWrongLoginTrial, true);

                // button 
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);

                Util.ControlEnabled(btnLoad, true);
                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);

                #region User-Detail.
                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), false);
                UserDetails oUserDetails = new UserDetails();
                oUserDetails.MakeDate = DateTime.Now;
                ucUserDet.UserDetail = oUserDetails;
                #endregion User-Detail.

                fsList.Visible = true;
                LoadList();
            }
        }
        #endregion InitializeData


        #region Basic Operational Function from control EVENT


        protected void btnSave_Click(object sender, EventArgs e)
        {
            
            SecurityPolicy oSP = new SecurityPolicy(Util.GetIntNumber(hdSecurityPolicyID.Value));
            SecurityPolicyDAL oSPDAL = new SecurityPolicyDAL();

            oSP.IsChangePassFirst = ddlRequiredToChangePw.SelectedValue == "true" ? true : false;
            oSP.OldPassProhibited = Util.GetIntNumber(txtOldPwprohibited.Text.Trim());
            oSP.MinPassLength = Util.GetIntNumber(txtMinimumPwLg.Text.Trim());
            oSP.IsForceAlphaNumericPass = ddlAlphaNumPw.SelectedValue == "true" ? true : false; 
            oSP.PassExpiresAfterDays = Util.GetIntNumber(txtPwExpDays.Text.Trim());
            oSP.DaysEarlierPass = Util.GetIntNumber(txtDayEarly.Text.Trim());
            oSP.MaxWrongTtrial = Util.GetIntNumber(txtWrongLoginTrial.Text.Trim());


            oSP.UserDetails = ucUserDet.UserDetail;
            Result oResult = (Result)oSPDAL.Save(oSP);

            if (oResult.Status)
            {
                this.LoadList();
                ClearTextValue();
                hdSecurityPolicyID.Value = "";
                ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
            }
        }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearTextValue();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_SA_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_SA.SECURITY_POLICY).PadLeft(5, '0'), false);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdSecurityPolicyID.Value))
            {
                SecurityPolicy oSP = new SecurityPolicy(Util.GetIntNumber(hdSecurityPolicyID.Value));
                SecurityPolicyDAL oSPDAL = new SecurityPolicyDAL();
                oSP.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oSPDAL.Reject(oSP);
                if (oResult.Status)
                {
                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_REJECT, Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_REJECT, Constants.MSG_TYPE_ERROR);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_REJECT, Constants.MSG_TYPE_ERROR);
            }

        }
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdSecurityPolicyID.Value))
            {
                SecurityPolicy oSP = new SecurityPolicy(Util.GetIntNumber(hdSecurityPolicyID.Value));
                SecurityPolicyDAL oSPDAL = new SecurityPolicyDAL();

                oSP.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oSPDAL.Approve(oSP);
                if (oResult.Status)
                {
                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE, Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_APPROVE, Constants.MSG_TYPE_ERROR);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_APPROVE, Constants.MSG_TYPE_ERROR);
            }
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdSecurityPolicyID.Value))
            {
                SecurityPolicyDAL oSpDAL = new SecurityPolicyDAL();
                Result oResult = (Result)oSpDAL.Detete(hdSecurityPolicyID.Value);
                if (oResult.Status)
                {
                    this.LoadList();
                    this.ClearTextValue();

                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_DELETE, Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                    if (oResult.Message.Equals(Constants.TABLE_MAIN))
                    {
                        ucMessage.OpenMessage(Constants.MSG_APPROVED_DELETE_DATA, Constants.MSG_TYPE_ERROR);
                    }
                    else
                    {
                        ucMessage.OpenMessage(Constants.MSG_ERROR_DELETE, Constants.MSG_TYPE_ERROR);
                    }
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_DELETE, Constants.MSG_TYPE_ERROR);
            }
        }


        protected void btnLoad_Click(object sender, EventArgs e)
        {
            //Load Approved Data
            SecurityPolicyDAL oSpDAL = new SecurityPolicyDAL();
            Result oResult = new Result();
            oResult = oSpDAL.LoadApprovedData();
            SecurityPolicy oSp = null;
            if (oResult.Status)
            {
                oSp = (SecurityPolicy)oResult.Return;
                if (oSp != null)
                {
                    DDListUtil.Assign(ddlRequiredToChangePw, oSp.IsChangePassFirst);
                    txtOldPwprohibited.Text = oSp.OldPassProhibited.ToString();
                    txtMinimumPwLg.Text = oSp.MinPassLength.ToString();
                    DDListUtil.Assign(ddlAlphaNumPw, oSp.IsForceAlphaNumericPass);
                    txtPwExpDays.Text = oSp.PassExpiresAfterDays.ToString();
                    txtDayEarly.Text = oSp.DaysEarlierPass.ToString();
                    txtWrongLoginTrial.Text = oSp.MaxWrongTtrial.ToString();

                    ucUserDet.UserDetail = oSp.UserDetails;
                    hdSecurityPolicyID.Value = oSp.SecurityPolicyID.ToString();
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
            }
            
        }

        protected void gvList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            LoadDataByID(Convert.ToInt32(gvRow.Cells[1].Text));
        }
        protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvList, null);
        }

        public void PopErrorMsgAction(string sType)
        {
            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_SA_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_SA.SECURITY_POLICY).PadLeft(5, '0'), false);
            }
            else
            {
                // no action
            }
        }

        #endregion Basic Operational Function from control EVENT

        #region Utility Method
        private void ClearTextValue()
        {
            ddlRequiredToChangePw.SelectedIndex = 0;
            txtOldPwprohibited.Text=string.Empty;
            txtMinimumPwLg.Text=string.Empty;
            ddlAlphaNumPw.SelectedIndex = 0;
            txtPwExpDays.Text=string.Empty;
            txtDayEarly.Text=string.Empty;
            txtWrongLoginTrial.Text=string.Empty;
        }

        // ok
        private void LoadList()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (oConfig != null)
            {

                SecurityPolicy oSP = new SecurityPolicy();
                SecurityPolicyDAL oSPDal = new SecurityPolicyDAL();
                Result oResult = oSPDal.LoadUnapprovedList(oConfig.UserName, false);
                gvList.DataSource = null;
                gvList.DataBind();

                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList.Rows.Count > 0)
                    {
                        dtTmpList.Columns.Remove("MakerID");

                        gvList.DataSource = dtTmpList;
                        gvList.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;
                    }
                    else
                    {
                        ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
                    }
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
            }
        }

        private void LoadDataByID(int iSPID)
        {
            SecurityPolicy oSp = new SecurityPolicy(iSPID);
            SecurityPolicyDAL oSpDAL = new SecurityPolicyDAL();

            Result oResult = new Result();
            oResult = oSpDAL.LoadByID(oSp);
            ClearTextValue();
            if (oResult.Status)
            {
                oSp = (SecurityPolicy)oResult.Return;
                if (oSp != null)
                {
                    DDListUtil.Assign(ddlRequiredToChangePw, oSp.IsChangePassFirst);
                    txtOldPwprohibited.Text = oSp.OldPassProhibited.ToString();
                    txtMinimumPwLg.Text = oSp.MinPassLength.ToString();
                    DDListUtil.Assign(ddlAlphaNumPw, oSp.IsForceAlphaNumericPass);
                    txtPwExpDays.Text = oSp.PassExpiresAfterDays.ToString();
                    txtDayEarly.Text = oSp.DaysEarlierPass.ToString();
                    txtWrongLoginTrial.Text = oSp.MaxWrongTtrial.ToString();

                    ucUserDet.UserDetail = oSp.UserDetails;
                    hdSecurityPolicyID.Value = oSp.SecurityPolicyID.ToString();
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
            }
        }
        #endregion Utility Method

    }
}
