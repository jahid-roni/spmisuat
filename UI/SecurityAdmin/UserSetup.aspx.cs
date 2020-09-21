/*
 * File name            : UserSetup
 * Author               : A.K.M. Tanjil Hossain,
 *                      : A.K.M. Zahidul Quaium,
 *                      : Tanvir Alam
 *                      : Jerin Afsana
 * Date                 : Sep 01,2014
 * Version              : 1.0
 *
 * Description          : To manager User Detail
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
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Entity.SecurityAdmin;

using System.Data;
using SBM_BLC1.SecurityAdmin;

namespace SBM_WebUI.mp
{
    public partial class UserSetup : System.Web.UI.Page
    {
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_USER_ID = "sUserID";
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_SA.USER))
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
            gvData.PageSize = (int)Constants.PAGING_UNAPPROVED;

            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlUserClass, "GroupID", "GroupName", "SA_Group", true);
            DDListUtil.LoadDDLFromDB(ddlDesignation, "DesignationID", "Description", "SA_Designation", true);
            DDListUtil.LoadDDLFromDB(ddlDivision, "DivisionID", "DivisionName", "SPMS_Division", true);            
            Util.ChkChangeSetColor(chkStatus);

            string sUserID = Request.QueryString[OBJ_USER_ID];
            string sPageID = Request.QueryString[OBJ_PAGE_ID];
            if (!string.IsNullOrEmpty(sUserID))
            {
                sUserID = oCrypManager.GetDecryptedString(sUserID, Constants.CRYPT_PASSWORD_STRING);
            }
            if (!string.IsNullOrEmpty(sPageID))
            {
                sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
            }

            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (!string.IsNullOrEmpty(sUserID) && !string.IsNullOrEmpty(sPageID))
            {
                string sOperationType = sPageID.Substring(4, 1);
                if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
                {                    
                    LoadDataByID(sUserID);

                    // general Control
                    Util.ControlEnabled(txtLoginUserID, false);
                    Util.ControlEnabled(ddlUserClass, false);
                    Util.ControlEnabled(ddlDesignation, false);
                    Util.ControlEnabled(ddlDivision, false);
                    Util.ControlEnabled(txtUserFirstName, false);
                    Util.ControlEnabled(txtUserLastName, false);
                    Util.ControlEnabled(txtConfirmPassword, false);
                    Util.ControlEnabled(txtNewPassword, false);
                    Util.ControlEnabled(chkStatus, false);

                    // button 
                    Util.ControlEnabled(btnReject, true);
                    Util.ControlEnabled(btnApprove, true);
                    Util.ControlEnabled(btnBack, true);
                    Util.ControlEnabled(btnReset, false);
                    Util.ControlEnabled(btnSave, false);
                    Util.ControlEnabled(btnDelete, false);
                    Util.ControlEnabled(btnLoad, false);
                    Util.ControlEnabled(btnFirst, false);
                    Util.ControlEnabled(btnPrevious, false);
                    Util.ControlEnabled(btnNext, false);
                    Util.ControlEnabled(btnLast, false);
                    Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);

                    #region User-Detail.
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
                // general Control
                Util.ControlEnabled(txtLoginUserID, true);
                Util.ControlEnabled(ddlUserClass, true);
                Util.ControlEnabled(ddlDesignation, true);
                Util.ControlEnabled(ddlDivision, true);
                Util.ControlEnabled(txtUserFirstName, true);
                Util.ControlEnabled(txtUserLastName, true);
                Util.ControlEnabled(txtConfirmPassword, true);
                Util.ControlEnabled(txtNewPassword, true);
                Util.ControlEnabled(chkStatus, true);

                // button 
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);
                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);
                Util.ControlEnabled(btnLoad, true);
                Util.ControlEnabled(btnFirst, true);
                Util.ControlEnabled(btnPrevious, true);
                Util.ControlEnabled(btnNext, true);
                Util.ControlEnabled(btnLast, true);

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


        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvData.PageIndex = e.NewPageIndex;
            if (Session[Constants.SES_CONFIG_UNAPPROVE_DATA] != null)
            {
                DataTable dtTmpList = (DataTable)Session[Constants.SES_CONFIG_UNAPPROVE_DATA];
                gvData.DataSource = dtTmpList;
                gvData.DataBind();
            }
        }

        protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("Page"))
            {
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                hdDataType.Value = "T";
                LoadDataByID(gvRow.Cells[1].Text);
            }
        }
        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, null);
        }

        public void PopErrorMsgAction(string sType)
        {
            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_SA_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_SA.USER).PadLeft(5, '0'), false);
            }
            else
            {
                txtLoginUserID.Focus();
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_SA_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_SA.USER).PadLeft(5, '0'), false);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdUserID.Value))
            {
                User oUesr = new User(txtLoginUserID.Text);
                UserDAL oUserDAL = new UserDAL();
                oUesr.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oUserDAL.Reject(oUesr);
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
            if (!string.IsNullOrEmpty(hdUserID.Value))
            {
                User oUesr = new User(txtLoginUserID.Text);
                UserDAL oUserDAL = new UserDAL();
                oUesr.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oUserDAL.Approve(oUesr);
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

        protected void btnReset_Click(object sender, EventArgs e)
        {            
            ClearTextValue();            
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsIsDigitValidPassword(string password)
        {
            return password.Any(c => IsDigit(c));
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // need to check first
            //This block is used for checking the Security Policy
            UserDAL oUserDAL = new UserDAL();
            bool status = true;
            string sErrorMessage = string.Empty;
            Result oResult = null;
            if (string.IsNullOrEmpty(hdUserID.Value))
            {
                oResult = oUserDAL.CheckUserIDAvailability(txtLoginUserID.Text.Trim());

                if (oResult.Status)
                {
                    if (Convert.ToInt32((Object)oResult.Return) > 0)
                    {
                        sErrorMessage = "This User Login ID exists. Please try with another one";
                        txtLoginUserID.Text = string.Empty;
                        status = false;
                    }
                }
            }
            else
            {
                SecurityPolicyDAL oSpDAL = new SecurityPolicyDAL();
                Result oPOResult = oSpDAL.LoadApprovedData();
                SecurityPolicy oSp = null;

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
            }
            //This block is used for checking the Security Policy


            if (status)
            {
                User oUser = new User();                

                oUser.UserID = Util.GetIntNumber(hdUserID.Value == "" ? "0" : hdUserID.Value);
                oUser.UserName = Request[txtLoginUserID.UniqueID].Trim();
                oUser.Group.GroupID = Util.GetIntNumber(ddlUserClass.SelectedItem.Value);
                oUser.FirstName = txtUserFirstName.Text;
                oUser.LastName = txtUserLastName.Text;
                oUser.Password = txtNewPassword.Text;
                oUser.Designation.DesignationID = Util.GetIntNumber(ddlDesignation.SelectedItem.Value);
                oUser.Status = chkStatus.Checked;
                oUser.DivisionID = ddlDivision.SelectedValue;
                oUser.UserDetails = ucUserDet.UserDetail; ;

                oResult = (Result)oUserDAL.Save(oUser);

                if (oResult.Status)
                {
                    ClearTextValue();
                    LoadList();
                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
                }
            }
            else
            {
                ucMessage.OpenMessage("<UI>" + sErrorMessage + "</UI>", Constants.MSG_TYPE_ERROR);
            }
        }

        // ok
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdUserID.Value))
            {
                UserDAL oUserDAL = new UserDAL();
                Result oResult = (Result)oUserDAL.Detete(txtLoginUserID.Text);
                if (oResult.Status)
                {
                    ClearTextValue();
                    LoadList();                    
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

        // ok
        private void ClearTextValue()
        {            
            txtLoginUserID.ReadOnly = false;
            txtLoginUserID.Text = string.Empty;
            ddlUserClass.SelectedIndex = 0;
            ddlDesignation.SelectedIndex = 0;
            ddlDivision.SelectedIndex = 0;
            txtUserFirstName.Text = string.Empty;
            txtUserLastName.Text = string.Empty;
            txtNewPassword.Attributes.Add("value", "");
            txtConfirmPassword.Attributes.Add("value", "");
            chkStatus.Checked = false;
            hdUserID.Value = string.Empty;
            hdDataType.Value = string.Empty;
            ucUserDet.ResetData();
        }

        // ok
        private void LoadList()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (oConfig != null)
            {
                gvData.DataSource = null;
                gvData.DataBind();

                User oUser = new User();
                UserDAL oUserDAL = new UserDAL();
                Result oResult = oUserDAL.LoadUnapprovedList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);

                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList.Rows.Count > 0)
                    {
                        dtTmpList.Columns.Remove("Maker ID");

                        gvData.DataSource = dtTmpList;
                        gvData.DataBind();                        
                    }

                    Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;
                }
                //else
                //{
                //    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                //    ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
                //}
            }
            //else
            //{
            //    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
            //    ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
            //}
        }

        protected void btnFirst_Click(object sender, EventArgs e)
        {
            MoveAction("F");
        }
        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            MoveAction("P");
        }
        protected void btnNext_Click(object sender, EventArgs e)
        {
            MoveAction("N");
        }
        protected void btnLast_Click(object sender, EventArgs e)
        {
            MoveAction("L");
        }

        private void MoveAction(string sType)
        {
            txtLoginUserID.ReadOnly = true;

            UserDAL oUserDAL = new UserDAL();
            Result oResult = oUserDAL.LoadMoveData(txtLoginUserID.Text, sType);
            if (oResult.Status) 
            {
                User oUser = (User)oResult.Return;
                if (oUser != null)
                {
                    ClearTextValue();
                    hdDataType.Value = "M";
                    txtLoginUserID.Text = oUser.UserName.ToString();
                    txtUserFirstName.Text = oUser.FirstName.ToString();
                    txtUserLastName.Text = oUser.LastName.ToString();
                    txtNewPassword.Attributes.Add("value", oUser.Password.ToString());
                    txtConfirmPassword.Attributes.Add("value", oUser.Password.ToString());
                    chkStatus.Checked = oUser.Status;
                    Util.ChkChangeSetColor(chkStatus);

                    DDListUtil.Assign(ddlDesignation, oUser.Designation.DesignationID.ToString());
                    DDListUtil.Assign(ddlUserClass, oUser.Group.GroupID.ToString());
                }

                ucUserDet.UserDetail = oUser.UserDetails;
                hdUserID.Value = oUser.UserID.ToString();
            }
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request[txtLoginUserID.UniqueID].Trim()))
            {                
                hdDataType.Value = "L";
                LoadDataByID(Request[txtLoginUserID.UniqueID].Trim());
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
            }
        }


        private void LoadDataByID(string sUserID)
        {
            User oUser = new User(sUserID);
            UserDAL oUserDAL = new UserDAL();

            Result oResult = new Result();
            oResult = oUserDAL.LoadByID(oUser);
            
            if (oResult.Status)
            {
                oUser = (User)oResult.Return;
                if (oUser != null)
                {
                    txtLoginUserID.Text = oUser.UserName.ToString();
                    txtUserFirstName.Text = oUser.FirstName.ToString();
                    txtUserLastName.Text = oUser.LastName.ToString();
                    txtNewPassword.Attributes.Add("value", oUser.Password.ToString());
                    txtConfirmPassword.Attributes.Add("value", oUser.Password.ToString());
                    chkStatus.Checked = oUser.Status;
                    Util.ChkChangeSetColor(chkStatus);

                    DDListUtil.Assign(ddlDesignation, oUser.Designation.DesignationID.ToString());
                    DDListUtil.Assign(ddlUserClass, oUser.Group.GroupID.ToString());
                    DDListUtil.Assign(ddlDivision, oUser.DivisionID.Trim());
                    if (string.IsNullOrEmpty(hdDataType.Value))
                    {
                        //When Loading from Approver End
                        UserDetails userDetails = ucUserDet.UserDetail;
                        userDetails.MakerID = oUser.UserDetails.MakerID;
                        userDetails.MakeDate = oUser.UserDetails.MakeDate;
                        ucUserDet.UserDetail = userDetails;
                    }
                    else if (hdDataType.Value.Equals("T"))
                    {
                        //When loading from temp table
                        UserDetails userDetails = ucUserDet.UserDetail;
                        userDetails.CheckerID = oUser.UserDetails.CheckerID;
                        userDetails.CheckDate = oUser.UserDetails.CheckDate;
                        userDetails.CheckerComment = oUser.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = userDetails;
                    }

                    hdUserID.Value = oUser.UserID.ToString();
                    txtLoginUserID.ReadOnly = false;
                }
                else
                {
                    ClearTextValue();
                    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
            else
            {
                ClearTextValue();
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
            }
        }        
    }
}
