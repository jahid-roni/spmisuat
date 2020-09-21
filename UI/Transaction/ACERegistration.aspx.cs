using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//add
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Configuration;
using SBM_BLC1.Entity.Transaction;
using SBM_BLC1.Transaction;
using SBM_BLC1.Entity.Configuration;
using System.Collections;
//using SBM_BLC1.DAL.Transaction;

namespace SBM_WebUI.UI.Transaction
{
    public partial class ACERegistration : System.Web.UI.Page
    {
        #region Local Variable
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_REG_NO = "sRegNo";
        public const string OBJ_PAGE_ID = "sPageID";        
        #endregion Local Variable


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Constants.SES_USER_CONFIG] != null)
            {
                if (!Page.IsPostBack)
                {
                    Util.InvalidateSession();
                    InitializeData();
                    //This is for Page Permission
                    CheckPermission chkPer = new CheckPermission();
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_TRANS.SP_ISSUE))
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
            hdRegNo.Value = "";
            // Dropdown load SPType

            string sRegNo = Request.QueryString[OBJ_REG_NO];
            string sPageID = Request.QueryString[OBJ_PAGE_ID];


            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            DDListUtil.LoadDDLFromDB(ddlBranch, "BranchID", "BranchName", "SPMS_Branch", true);
            // end of Intial Data

            // Issue Details
            txtIssueDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            gvDenomDetail.DataSource = null;
            gvDenomDetail.DataBind();


            if (!string.IsNullOrEmpty(sRegNo) && !string.IsNullOrEmpty(sPageID))
            {
                sRegNo = oCrypManager.GetDecryptedString(sRegNo, Constants.CRYPT_PASSWORD_STRING);
                sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);

                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];                
                LoadDataByID(sRegNo, "Approve");//query from Temp Table

                #region User-Detail.
                UserDetails oUserDetails = ucUserDet.UserDetail;
                oUserDetails.CheckerID = oConfig.UserName;
                oUserDetails.CheckDate = DateTime.Now;

                ucUserDet.UserDetail = oUserDetails;
                #endregion User-Detail.

                fsList.Visible = false;
                #region Enable-disable controls
                EnableDisableControl(false);
                #endregion Enable-disable controls
            }
            else
            {
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                UserDetails oUserDetails = new UserDetails();
                oUserDetails.MakerID = oConfig.UserName;
                oUserDetails.MakeDate = DateTime.Now;

                ucUserDet.UserDetail = oUserDetails;
                EnableDisableControl(true);
                fsList.Visible = true;
                LoadPreviousList();
            }
        }
        #endregion InitializeData

        private void LoadPreviousList()
        {
            IssueDAL oIssueDAL = new IssueDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oIssueDAL.LoadUnapprovedACERegisterList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);
            gvData.DataSource = null;
            gvData.DataBind();
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
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            if (txtRegistrationNo.Text.Length > 0)
            {
                LoadDataByID(txtRegistrationNo.Text, null);
            }
        }

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
                hdDataType.Value = "1";//loading from Temp Table                
                LoadDataByID(gvRow.Cells[1].Text, null);//query from Temp Table
            }
        }
        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, null);
        }
        public void PopupIssueSearchLoadAction(string sRegNo, string sTransNo, string sApprovalStaus)
        {
            if (!string.IsNullOrEmpty(sRegNo))
            {
                LoadDataByID(sRegNo, null);
            }
        }

        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");

            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.ACE_MANAGER).PadLeft(5, '0'), false);
            }
            else
            {
                txtRegistrationNo.Focus();
            }
        }

        protected void btnLeftShift_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdRegNo.Value))
            {
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                Issue oIssue = new Issue();
                oIssue.RegNo = hdRegNo.Value;
                oIssue.DivisionID = oConfig.DivisionID;
                oIssue.BankID = oConfig.BankCodeID;
                IssueDAL oIssueDAL = new IssueDAL();
                Result oResult = oIssueDAL.LoadViewDataID(oIssue, "L");

                if (oResult.Status)
                {
                    oIssue = (Issue)oResult.Return;
                    SetObject(oIssue,null);
                }
                else
                {
                    hdRegNo.Value = "";
                    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                }
            }
            else
            {
                ucMessage.OpenMessage("You must load Issue first before viewing Issue Detail!", Constants.MSG_TYPE_INFO);
            }
        }

        protected void btnRegSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRegistrationNo.Text))
            {
                LoadDataByID(txtRegistrationNo.Text, null);
            }
        }

        protected void btnRightShift_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdRegNo.Value))
            {
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                Issue oIssue = new Issue();
                oIssue.RegNo = hdRegNo.Value;
                oIssue.DivisionID = oConfig.DivisionID;
                oIssue.BankID = oConfig.BankCodeID;
                IssueDAL oIssueDAL = new IssueDAL();
                Result oResult = oIssueDAL.LoadViewDataID(oIssue, "R");

                if (oResult.Status)
                {
                    oIssue = (Issue)oResult.Return;
                    SetObject(oIssue,null);
                }
                else
                {
                    hdRegNo.Value = "";
                    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                }
            }
            else
            {
                ucMessage.OpenMessage("You must load Issue first before viewing Issue Detail!", Constants.MSG_TYPE_INFO);
            }
        }

        private void SetObject(Issue oIssue, string flag)
        {
            if (oIssue != null)
            {
                hdRegNo.Value = oIssue.RegNo;
                txtRegistrationNo.Text = oIssue.RegNo;
                txtAppliedAmount.Text = oIssue.IssueAmount.ToString("N2");

                LoadCustomerTypeBySpType(oIssue.SPType.SPTypeID);
                DDListUtil.Assign(ddlSpType, oIssue.SPType.SPTypeID);
                DDListUtil.Assign(ddlCustomerType, oIssue.CustomerType.CustomerTypeID);
                DDListUtil.Assign(ddlBranch, oIssue.Branch.BranchID);

                txtIssueDate.Text = oIssue.VersionIssueDate.ToString(Constants.DATETIME_FORMAT);
                txtIssueName.Text = oIssue.IssueName;
                txtMasterNo.Text = oIssue.MasterNo;
                try
                {

                    ddlStatus.SelectedValue = oIssue.ACERegister.Status;
                    if (oIssue.ACERegister.StatusDate.Year == 1 || oIssue.ACERegister.StatusDate.Year <= 1900)
                    {
                        txtStatusDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
                    }
                    else
                    {
                        txtStatusDate.Text = oIssue.ACERegister.StatusDate.ToString(Constants.DATETIME_FORMAT);
                    }
                    txtInvLocation.Text = oIssue.ACERegister.InventoryLocation;
                    if (string.IsNullOrEmpty(oIssue.ACERegister.AccountNo))
                    {
                        txtPDAccDraftNo.Text = oIssue.Payment.AccountNo;
                        txtAccountName.Text = oIssue.AccountName;
                    }
                    else
                    {
                        txtPDAccDraftNo.Text = oIssue.ACERegister.AccountNo;
                        txtAccountName.Text = oIssue.ACERegister.Payment.AccountName;
                    }

                    txtRemarks.Text = oIssue.ACERegister.Remarks;
                    // end of ACE Register 

                    #region Scrip Loading
                    gvDenomDetail.DataSource = null;
                    gvDenomDetail.DataBind();
                    if (oIssue.ScripList.Count > 0)
                    {
                        DataTable dtScripData = new DataTable("dtScripData");

                        dtScripData.Columns.Add(new DataColumn("bfDenomination", typeof(string)));
                        dtScripData.Columns.Add(new DataColumn("bfSeries", typeof(string)));
                        dtScripData.Columns.Add(new DataColumn("bfSerialNo", typeof(string)));

                        DataRow row = null;
                        for (int i = 0; i < oIssue.ScripList.Count; i++)
                        {
                            row = dtScripData.NewRow();
                            row["bfDenomination"] = oIssue.ScripList[i].Denomination.DenominationID.ToString();
                            row["bfSeries"] = oIssue.ScripList[i].SPSeries.ToString();
                            row["bfSerialNo"] = oIssue.ScripList[i].SlNo.ToString();

                            dtScripData.Rows.Add(row);
                        }

                        gvDenomDetail.DataSource = dtScripData;
                        gvDenomDetail.DataBind();
                    }
                    #endregion

                    //UserDetails oUserDetails = ucUserDet.UserDetail;
                    if (!string.IsNullOrEmpty(flag))
                    {
                        ucUserDet.UserDetail = oIssue.ACERegister.UserDetails;
                    }
                }
                catch (Exception)
                {

                    //For new issue ACE
                }

            }
        }
        private void LoadCustomerTypeBySpType(string sSPType)
        {
            CustomerTypeWiseSPLimitDAL oCustTypeWiseSPLimit = new CustomerTypeWiseSPLimitDAL();
            Result oResult = (Result)oCustTypeWiseSPLimit.GetCustomerTypeBySpType(sSPType);
            if (oResult.Status)
            {
                ddlCustomerType.Items.Clear();
                DataTable dtGetCustomerTypeID = (DataTable)oResult.Return;
                DDListUtil.Assign(ddlCustomerType, dtGetCustomerTypeID);
            }
        }

        public void LoadDataByID(string sRegNo, string flag)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Issue oIssue = new Issue();
            oIssue.RegNo = sRegNo.Trim();
            oIssue.DivisionID = oConfig.DivisionID;
            oIssue.BankID = oConfig.BankCodeID;
            IssueDAL oIssueDAL = new IssueDAL();
            Result oResult = oIssueDAL.LoadViewDataID(oIssue, null);

            if (oResult.Status)
            {
                oIssue = (Issue)oResult.Return;
                if (string.IsNullOrEmpty(oIssue.IssueTransNo))
                {
                    TotalClear();
                    ucMessage.OpenMessage("Invalid Registration No, No record found. Please Check.", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel3, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
                else
                {
                    SetObject(oIssue, flag);
                }
            }
            else
            {
                hdRegNo.Value = "";
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdRegNo.Value))
            {
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                if (oConfig != null)
                {
                    ACERegister oACERegister = new ACERegister();
                    IssueDAL oIssueDAL = new IssueDAL();

                    oACERegister.RegNo = hdRegNo.Value;
                    oACERegister.AccountNo = txtPDAccDraftNo.Text.Trim();
                    oACERegister.Status = ddlStatus.SelectedValue;
                    oACERegister.StatusDate = DateTime.Today;
                    oACERegister.InventoryLocation = txtInvLocation.Text;
                    oACERegister.Remarks = txtRemarks.Text.ToUpper();
                    oACERegister.BankID = oConfig.BankCodeID;
                    oACERegister.DivisionID = oConfig.DivisionID;
                    //User Details
                    oACERegister.UserDetails = ucUserDet.UserDetail;
                    oACERegister.UserDetails.MakeDate = DateTime.Now;

                    //Response.Redirect(Consants.PAGE_TRAN_ISSUE_UPDATE + "?sIsUpdate=" + oCrypManager.GetEncryptedString("1") + "&sRegID=" + oCrypManager.GetEncryptedString(txtRegistrationNo.Text) + "&sPageID=" + oCrypManager.GetEncryptedString(Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_UPDATE).PadLeft(5, '0')), false);

                    Result oResult = (Result)oIssueDAL.SaveACERegister(oACERegister);

                    if (oResult.Status)
                    {
                        TotalClear();
                        LoadPreviousList();
                        ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
                    }
                    else
                    {
                        ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
                    }
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
                }
            }
            else
            {
                ucMessage.OpenMessage("You must load Issue first before updating Issue Detail!", Constants.MSG_TYPE_INFO);
            }
        }
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            ACERegister oACERegister = new ACERegister();
            oACERegister.RegNo = txtRegistrationNo.Text.Trim();
            IssueDAL oIssueDAL = new IssueDAL();
            oACERegister.UserDetails = ucUserDet.UserDetail;
            Result oResult = (Result)oIssueDAL.ApproveACERegister(oACERegister);
            if (oResult.Status)
            {
                if (oResult.Message.Equals("E"))
                {
                    ucMessage.OpenMessage("Required Script is not available now", Constants.MSG_TYPE_ERROR);
                }
                else
                {
                    //TotalClear();
                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE + " with Reg. No.: <b>" + (string)oResult.Return + "</b>", Constants.MSG_TYPE_SUCCESS);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_APPROVE, Constants.MSG_TYPE_ERROR);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.ACE_MANAGER).PadLeft(5, '0'), false);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdRegNo.Value))
            {
                ACERegister oACERegister = new ACERegister();
                oACERegister.RegNo = hdRegNo.Value;
                oACERegister.UserDetails = ucUserDet.UserDetail;
                
                IssueDAL oIssueDAL = new IssueDAL();

                Result oResult = (Result)oIssueDAL.RejectACERegister(oACERegister);
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
                ucMessage.OpenMessage(Constants.MSG_ERROR_REJECT, Constants.MSG_TYPE_INFO);
            }
        }

        private void TotalClear()
        {
            gvDenomDetail.DataSource = null;
            gvDenomDetail.DataBind();

            //Issue Details
            ddlSpType.SelectedIndex = 0;
            ddlBranch.SelectedIndex = 0;
            txtIssueDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtRegistrationNo.Text = "";
            ddlCustomerType.Items.Clear();
            txtAppliedAmount.Text = "";
            txtIssueName.Text = string.Empty;
            txtMasterNo.Text = string.Empty;

            //ACER Details
            txtStatusDate.Text = string.Empty;
            txtPDAccDraftNo.Text = string.Empty;
            txtAccountName.Text = string.Empty;
            txtInvLocation.Text = string.Empty;
            txtRemarks.Text = string.Empty;

            //Clear Hidden values            
            hdRegNo.Value = string.Empty;
            hdDataType.Value = string.Empty;

            //User Detail
            ucUserDet.ResetData();
        }

        private void EnableDisableControl(bool isEnabled)
        {
            // general Control
            if (isEnabled)
            {                         
                //TextBox controls
                Util.ControlEnabled(txtRegistrationNo, true);
                Util.ControlEnabled(txtStatusDate, true);
                Util.ControlEnabled(txtPDAccDraftNo, true);
                Util.ControlEnabled(txtInvLocation,true);
                Util.ControlEnabled(txtRemarks, true);
                //ddl
                //Util.ControlEnabled(ddlStatus, true);
                // button
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);
                //Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);
                Util.ControlEnabled(btnLeftShift, true);
                Util.ControlEnabled(btnRightShift, true);
                
                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), false);
                                                
                fsList.Visible = true;
            }
            else
            {
                //TextBox controls
                Util.ControlEnabled(txtRegistrationNo, false);
                Util.ControlEnabled(txtStatusDate, false);
                Util.ControlEnabled(txtPDAccDraftNo, false);
                Util.ControlEnabled(txtInvLocation, false);
                Util.ControlEnabled(txtRemarks, false);
                //ddl
                //Util.ControlEnabled(ddlStatus, false);
                // button 
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true);
                //Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);
                Util.ControlEnabled(btnLeftShift, false);
                Util.ControlEnabled(btnRightShift, false);
                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);                
               fsList.Visible = false;
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdRegNo.Value))
            {
                if (hdDataType.Value.Equals("1"))
                {
                    IssueDAL oIssueDAL = new IssueDAL();
                    Result oResult = (Result)oIssueDAL.DeteteACERegister(hdRegNo.Value);
                    if (oResult.Status)
                    {
                        TotalClear();
                        LoadPreviousList();
                        ucMessage.OpenMessage(Constants.MSG_SUCCESS_DELETE, Constants.MSG_TYPE_SUCCESS);
                    }
                    else
                    {
                        ucMessage.OpenMessage(Constants.MSG_ERROR_DELETE, Constants.MSG_TYPE_ERROR);
                    }
                }
                else
                {
                    ucMessage.OpenMessage("No unapproved data found to delete!!", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel3, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
            else
            {
                ucMessage.OpenMessage("Data has not been loaded!!", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel3, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }

        protected void txtPDAccDraftNo_TextChanged(object sender, EventArgs e)
        {            
            if (!string.IsNullOrEmpty(txtPDAccDraftNo.Text))
            {
                if (txtPDAccDraftNo.Text.Length < 13)
                {
                    ucMessage.OpenMessage("Account no. must be 13 digits (with currency code)", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel3, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    txtAccountName.Text = string.Empty;
                    return;
                }
                
                CustomerDetailsDAL oCDDal = new CustomerDetailsDAL();
                Result oResult = oCDDal.LoadDataFromBDDB2ByAccountNo(txtPDAccDraftNo.Text);
                if (oResult.Status)
                {
                    DataTable dt = (DataTable)oResult.Return;
                    if (dt.Rows.Count != 0)
                    {
                        txtAccountName.Text = Convert.ToString(dt.Rows[0]["AciAccName"]);
                    }
                    else
                    {
                        txtAccountName.Text = string.Empty;
                    }
                }
                else
                {
                    txtAccountName.Text = string.Empty;
                }
            }
            else
            {
                txtAccountName.Text = string.Empty;
            }
        }
    }
}
