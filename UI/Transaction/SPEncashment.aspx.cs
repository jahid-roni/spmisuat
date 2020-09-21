using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Transaction;
using SBM_BLC1.Entity.Transaction;
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Configuration;

namespace SBM_WebUI.mp
{
    public partial class SPEncashment : System.Web.UI.Page
    {

        #region Local Variable
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_REG_NO = "sRegNo";
        public const string OBJ_PAGE_ID = "sPageID";
        public int SEARCH_FROM = -1;
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_TRANS.ENCASHED))
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

            TotalClear();

            gvData.DataSource = null;
            gvData.DataBind();



            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            DDListUtil.LoadDDLFromDB(ddlBranch, "BranchID", "BranchName", "SPMS_Branch", true);

            string sRegNo = Request.QueryString[OBJ_REG_NO];
            string sPageID = Request.QueryString[OBJ_PAGE_ID];

            if (!string.IsNullOrEmpty(sRegNo))
            {
                sRegNo = oCrypManager.GetDecryptedString(sRegNo, Constants.CRYPT_PASSWORD_STRING);
            }
            if (!string.IsNullOrEmpty(sPageID))
            {
                sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
            }


            if (!string.IsNullOrEmpty(sRegNo) && !string.IsNullOrEmpty(sPageID))
            {
                string sOperationType = sPageID.Substring(4, 1);
                if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
                {
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    //ishtiak
                    SEARCH_FROM = 0;//For Checker
                    LoadDataByRegNo("", sRegNo, "1");//Data From Temp



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


                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                UserDetails oUserDetails = new UserDetails();
                oUserDetails.MakerID = oConfig.UserName;
                oUserDetails.MakeDate = DateTime.Now;
                ucUserDet.UserDetail = oUserDetails;

                fsList.Visible = true;
                LoadPreviousList();
            }
        }
        #endregion InitializeData

        #region Event Method...

        protected void btnRegSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRegistrationNo.Text))
            {
                SEARCH_FROM = 2;
                hdDataType.Value = "";
                LoadDataByRegNo("", txtRegistrationNo.Text, "");//Data From Main
            }
        }
        protected void txtPDAccountNo_TextChanged(object sender, EventArgs e)
        {
            txtPDAccountName.Text = "";
            txtPDAccountNo.Enabled = true;
            txtPDAccountNo.CssClass = "textInput";


            if (!string.IsNullOrEmpty(txtPDAccountNo.Text))
            {
                if (txtPDAccountNo.Text.Length < 11)
                {
                    ucMessage.OpenMessage("Account no must be 11 digit (Only Account No.)", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    txtPDAccountName.Text = "";
                    return;
                }
                string sCurrencyCode = string.Empty;
                if (!string.IsNullOrEmpty(ddlPDCurrency.SelectedValue))
                {
                    sCurrencyCode = ddlPDCurrency.SelectedValue;
                }
                CustomerDetailsDAL oCDDal = new CustomerDetailsDAL();
                Result oResult = oCDDal.LoadDataFromBDDB2ByAccountNo(txtPDAccountNo.Text + sCurrencyCode);
                if (oResult.Status)
                {
                    DataTable dt = (DataTable)oResult.Return;
                    if (dt.Rows.Count != 0)
                    {
                        txtPDAccountName.Text = Convert.ToString(dt.Rows[0]["AciAccName"]);
                    }
                    else
                    {
                        txtPDAccountName.Text = string.Empty;
                    }
                }
                else
                {
                    txtPDAccountName.Text = "";
                }
            }
            else
            {
                txtPDAccountName.Text = "";
            }
        }

        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, null);
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
                //ishtiak(SEARCH_FROM=3)
                SEARCH_FROM = 2;
                hdDataType.Value = "1";
                LoadDataByRegNo("", gvRow.Cells[1].Text, "1");
            }
        }

        protected void btnShowDetails_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdIssueTransNo.Value))
            {
                InterestPaymentDAL oIntPayDAL = new InterestPaymentDAL();
                Result oResult = (Result)oIntPayDAL.LoadShowDetailTransByRegistrationNo(hdIssueTransNo.Value);
                if (oResult.Status)
                {
                    VGD.SetData((DataTable)oResult.Return, "Transaction Detail");
                }
            }
            else
            {
                VGD.SetData(null, "Transaction Detail");
            }
        }

        protected void btnShowPolicy_Click(object sender, EventArgs e)
        {
            // if (!string.IsNullOrEmpty(hdIssueTransNo.Value) && ddlSpType.SelectedIndex != 0)
            if (ddlSpType.SelectedIndex != 0)
            {
                SPPolicy oPolicy = null;
                SPPolicyDAL oSPPolicyDAL = new SPPolicyDAL();
                Result oResult = null;
                if (string.IsNullOrEmpty(txtIssueDate.Text))
                {
                    oResult = (Result)oSPPolicyDAL.GetLatestPolicyDetail(ddlSpType.SelectedValue, Constants.ACTIVITY_TYPE.ISSUE, DateTime.Now);
                }
                else
                {
                    oResult = (Result)oSPPolicyDAL.GetLatestPolicyDetail(ddlSpType.SelectedValue, Constants.ACTIVITY_TYPE.ISSUE, Date.GetDateTimeByString(txtIssueDate.Text.ToString()));
                }

                if (oResult.Status)
                {
                    oPolicy = (SPPolicy)oResult.Return;
                    PD.SetPolicyDetails(oPolicy);
                }
            }
            else
            {
                ucMessage.OpenMessage("You must select SP Type first before viewing policy Detail!", Constants.MSG_TYPE_INFO);
            }
        }

        protected void btnStopPmt_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdIssueTransNo.Value))
            {
                StopPaymentDAL oSPDAL = new StopPaymentDAL();
                Result oResult = null;
                oResult = (Result)oSPDAL.LoadStopPayHistory(hdIssueTransNo.Value);
                if (oResult.Status)
                {
                    VGD.SetData((DataTable)oResult.Return, "Stop Payment Detail");
                }
            }
            else
            {
                VGD.SetData(null, "Stop Payment Detail");
            }
        }

        protected void btnLienStatus_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdIssueTransNo.Value))
            {
                LienDAL oLienDAL = new LienDAL();
                Result oResult = null;
                oResult = (Result)oLienDAL.LoadLienHistory(hdIssueTransNo.Value);
                if (oResult.Status)
                {
                    VGD.SetData((DataTable)oResult.Return, "Lien Status Detail");
                }
            }
            else
            {
                VGD.SetData(null, "Lien Status Detail");
            }
        }

        public void PopupIssueSearchLoadAction(string sRegNo, string sName, string sApprovalStaus)
        {
            if (!string.IsNullOrEmpty(sRegNo))
            {//ishtiak
                SEARCH_FROM = 2;
                hdDataType.Value = "";
                LoadDataByRegNo("", sRegNo, sApprovalStaus);
            }
        }

        public void IPELoadAction(string sEncshTransNo, string sRegNo, string sApprovalStaus)
        {
            if (!string.IsNullOrEmpty(sRegNo))
            {
                //ishtiak
                SEARCH_FROM = 1;
                hdDataType.Value = sApprovalStaus;
                LoadDataByRegNo(sEncshTransNo, sRegNo, sApprovalStaus);
            }
        }


        protected void btnSelect_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow gvr in gvEncashmentDetails.Rows)
            {
                if (!(gvr.FindControl("chkEncashed") as CheckBox).Checked
                    && !(gvr.FindControl("chkStopPayment") as CheckBox).Checked
                    && !(gvr.FindControl("chkLien") as CheckBox).Checked)
                {
                    (gvr.FindControl("chkSelected") as CheckBox).Checked = true;
                }
                else
                {
                    (gvr.FindControl("chkSelected") as CheckBox).Checked = false;
                }
            }

            CalculateEncashment(false);
        }

        protected void btnDeSelect_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow gvr in gvEncashmentDetails.Rows)
            {
                (gvr.FindControl("chkSelected") as CheckBox).Checked = false;
            }
            CalculateEncashment(false);
        }

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            CalculateEncashment(false);
        }

        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");

            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.ENCASHED).PadLeft(5, '0'), false);
            }
            else if (sType.Equals(Constants.BTN_SAVE))
            {
                txtRegistrationNo.Focus();
            }
            else
            {
                // no action
            }            
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.ENCASHED).PadLeft(5, '0'), false);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdEncashTransNo.Value))
            {
                Encashment oEncash = new Encashment(hdEncashTransNo.Value);
                EncashmentDAL oEncashDAL = new EncashmentDAL();
                oEncash.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oEncashDAL.Reject(oEncash);
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
            if (!string.IsNullOrEmpty(hdEncashTransNo.Value))
            {
                Encashment oEncash = Session[Constants.SES_ENCASHMENT] as Encashment;//new Encashment(hdEncashTransNo.Value);
                if (oEncash != null)
                {
                    oEncash.EncashmentTransNo = hdEncashTransNo.Value;
                    #region For Account Entries
                    oEncash.PrincipalAmount = Util.GetDecimalNumber(txtPDPrincipleAmount.Text);
                    oEncash.InterestToBePaid = Util.GetDecimalNumber(txtPDInterestAmount.Text);
                    oEncash.LeviToBePaid = Util.GetDecimalNumber(txtPDLevi.Text);
                    oEncash.IncomeTaxToBePaid = Util.GetDecimalNumber(txtPDIncomeTax.Text);
                    oEncash.PrincipalConvRate = Util.GetDecimalNumber(txtPDConvRate.Text);
                    #endregion
                }
                EncashmentDAL oEncashDAL = new EncashmentDAL();
                oEncash.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oEncashDAL.Approve(oEncash);
                if (oResult.Status)
                {
                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE, Constants.MSG_TYPE_SUCCESS);
                    SBM_BLC1.DAL.Common.ReportDAL rdal = new SBM_BLC1.DAL.Common.ReportDAL();
                    oResult = rdal.PrincipleAdviceReport(oEncash.EncashmentTransNo, true);
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
            EnableDisableControl(false);
            TotalClear();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                if (!hdDataType.Value.Equals("2"))
                {                    
                    Encashment oEnch = GetObject();
                    EncashmentDAL oEnchDAL = new EncashmentDAL();
                    Result oResult = oEnchDAL.Save(oEnch);
                    if (oResult.Status)
                    {
                        TotalClear();
                        LoadPreviousList();
                        ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
                    }
                    else
                    {
                        if (oResult.Message.Contains("UNIQUE KEY"))
                        {
                            string sMessage = "Registration No (" + txtRegistrationNo.Text + ") has already been processed by another user. Please check.";
                            ucMessage.OpenMessage(sMessage, Constants.MSG_TYPE_ERROR);
                        }
                        else
                        {
                            ucMessage.OpenMessage(oResult.Message, Constants.MSG_TYPE_ERROR);
                        }
                    }
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_APPROVED_SAVE_DATA, Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!hdDataType.Value.Equals("2"))
            {
                if (!string.IsNullOrEmpty(hdEncashTransNo.Value))
                {
                    EncashmentDAL oEncashDAL = new EncashmentDAL();
                    Result oResult = (Result)oEncashDAL.Detete(hdEncashTransNo.Value);
                    if (oResult.Status)
                    {
                        TotalClear();
                        LoadPreviousList();
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
            else
            {
                ucMessage.OpenMessage(Constants.MSG_APPROVED_DELETE_DATA, Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
            }
        }

        #endregion Event Method...

        #region Util Method...

        public void LoadPreviousList()
        {
            EncashmentDAL oEncashDAL = new EncashmentDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oEncashDAL.LoadUnapprovedList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);

            DataTable dtTmpList = null;

            gvData.DataSource = null;
            gvData.DataBind();

            if (oResult.Status)
            {
                dtTmpList = (DataTable)oResult.Return;
                if (dtTmpList != null)
                {
                    if (dtTmpList.Rows.Count > 0)
                    {
                        gvData.DataSource = dtTmpList;
                        gvData.DataBind();
                    }
                }
            }

            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;
        }

        private Encashment GetObject()
        {
            Encashment oEnch = new Encashment();

            #region Encashment Detail
            foreach (GridViewRow gvr in gvEncashmentDetails.Rows)
            {
                if ((gvr.FindControl("chkSelected") as CheckBox).Checked)
                {
                    EncashmentDetails oEnchDetail = new EncashmentDetails();

                    oEnchDetail.EncashmentTransNo = hdEncashTransNo.Value == "" ? "-1" : hdEncashTransNo.Value;//if any Transaction loaded from Temp table                    
                    oEnchDetail.Scrip.SPScripID = Util.GetIntNumber((gvr.FindControl("hdnSPScripID") as HiddenField).Value);
                    oEnchDetail.MaturityDate = Util.GetDateTimeByString(txtPDMaturityDate.Text);
                    oEnchDetail.AlreadyEncashedCoupons = Util.GetIntNumber((gvr.FindControl("hdnAlreadyEnchsdCoupons") as HiddenField).Value);
                    oEnchDetail.CouponsToBeEncashed = Util.GetIntNumber((gvr.FindControl("hdnCouponsToBeEnchsd") as HiddenField).Value);
                    oEnchDetail.CalculatedInterest = Util.GetDecimalNumber((gvr.FindControl("txtTotalInterest") as TextBox).Text);
                    oEnchDetail.AlreadyPaidInterest = Util.GetDecimalNumber((gvr.FindControl("lblPaidInterest") as Label).Text);
                    oEnchDetail.InterestToBePaid = Util.GetDecimalNumber(gvr.Cells[8].Text);
                    oEnchDetail.CalculatedLevi = 0;
                    oEnchDetail.AlreadyPaidLevi = 0;
                    oEnchDetail.LeviToBePaid = 0;
                    oEnchDetail.CalculatedIncomeTax = Util.GetDecimalNumber((gvr.FindControl("lblIncomeTax") as Label).Text);
                    oEnchDetail.AlreadyPaidIncomeTax = Util.GetDecimalNumber((gvr.FindControl("hdnPaidIncomeTax") as HiddenField).Value);
                    oEnchDetail.IncomeTaxToBePaid = Util.GetDecimalNumber((gvr.FindControl("lblIncomeTax") as Label).Text);
                    oEnchDetail.InterestAmount = Util.GetDecimalNumber(gvr.Cells[8].Text) - Util.GetDecimalNumber((gvr.FindControl("lblIncomeTax") as Label).Text);
                    oEnchDetail.PrincipalAmount = Util.GetDecimalNumber((gvr.FindControl("lblDenomination") as Label).Text);

                    //Add in List
                    oEnch.EncashmentDetailList.Add(oEnchDetail);
                }
            }
            #endregion Encashment Detail


            #region Encashment Parent table..

            oEnch.EncashmentTransNo = hdEncashTransNo.Value == "" ? "-1" : hdEncashTransNo.Value; //if any Transaction loaded from Temp table
            oEnch.Issue.IssueTransNo = hdIssueTransNo.Value;
            oEnch.EncashDate = Date.GetDateTimeByString(txtPaymentDate.Text);
            oEnch.PaidDate = Date.GetDateTimeByString(txtPaymentDate.Text);
            oEnch.InterestRate = Util.GetDecimalNumber(txtPDInterestRate.Text);
            oEnch.CalculatedInterest = Util.GetDecimalNumber(txtPDCalcInterest.Text);
            oEnch.AlreadyPaidInterest = Util.GetDecimalNumber(txtPDPaidInterest.Text);
            oEnch.InterestToBePaid = Util.GetDecimalNumber(txtPDCalcInterest.Text) - Util.GetDecimalNumber(txtPDPaidInterest.Text);
            oEnch.CalculatedLevi = Util.GetDecimalNumber(txtPDLevi.Text);
            oEnch.AlreadyPaidLevi = 0;//??
            oEnch.LeviToBePaid = Util.GetDecimalNumber(txtPDLevi.Text) - oEnch.AlreadyPaidLevi;//????
            oEnch.CalculatedIncomeTax = Util.GetDecimalNumber(txtPDIncomeTax.Text);
            oEnch.AlreadyPaidIncomeTax = oEnch.EncashmentDetailList.Count > 0 ? oEnch.EncashmentDetailList.Sum(ed => ed.AlreadyPaidIncomeTax) : 0;
            oEnch.IncomeTaxToBePaid = Util.GetDecimalNumber(txtPDIncomeTax.Text);
            oEnch.InterestPaymentMode.PaymentMode = Util.GetIntNumber(ddlPDPaymentMode.SelectedItem.Value);
            oEnch.InterestCurrency.CurrencyID = ddlPDCurrency.SelectedItem.Value.Trim();
            oEnch.InterestConvRate = Util.GetDecimalNumber(txtPDConvRate.Text);
            oEnch.InterestAmount = Util.GetDecimalNumber(txtPDInterestPayable.Text);
            if (!string.IsNullOrEmpty(txtPDAccountNo.Text))
            {
                oEnch.InterestAccountNo = txtPDAccountNo.Text; //+ ddlPDCurrency.SelectedValue;
            }
            oEnch.InterestRefNo = txtPDDraftNo.Text;
            oEnch.PrincipalPaymentMode.PaymentMode = Util.GetIntNumber(ddlPDPaymentMode.SelectedItem.Value);
            oEnch.PrincipalCurrency.CurrencyID = ddlPDCurrency.SelectedItem.Value.Trim();
            oEnch.PrincipalConvRate = Util.GetDecimalNumber(txtPDConvRate.Text);
            oEnch.PrincipalAmount = Util.GetDecimalNumber(txtPDPrincipleAmount.Text);
            if (!string.IsNullOrEmpty(txtPDAccountNo.Text))
            {
                oEnch.PrincipalAccountNo = txtPDAccountNo.Text; //+ ddlPDCurrency.SelectedValue;
            }
            oEnch.PrincipalRefNo = txtPDDraftNo.Text;
            oEnch.IsReinvested = false;
            oEnch.ReinvestmentAmount = 0;
            oEnch.IsPaid = false;
            oEnch.IsClaimed = false;

            oEnch.UserDetails = ucUserDet.UserDetail;
            #endregion Encashment Parent table..

            return oEnch;
        }

        private void SetObject(Encashment oEnch)
        {
            if (oEnch != null)
            {
                if (oEnch.Issue != null)
                {
                    Session[Constants.SES_ENCASHMENT] = oEnch;

                    hdEncashTransNo.Value = oEnch.EncashmentTransNo;
                    hdIssueTransNo.Value = oEnch.Issue.IssueTransNo;
                    hdRegNo.Value = oEnch.Issue.RegNo;
                    txtEncashTransNo.Text = oEnch.EncashmentTransNo;
                    txtRegistrationNo.Text = oEnch.Issue.RegNo.ToString();
                    ddlSpType.Text = oEnch.Issue.SPType.SPTypeID.Trim();
                    ddlBranch.Text = oEnch.Issue.Branch.BranchID.Trim();

                    txtIssueDate.Text = oEnch.Issue.VersionIssueDate.ToString(Constants.DATETIME_FORMAT);
                    txtIssueName.Text = oEnch.Issue.IssueName;
                    txtTotalAmount.Text = oEnch.Issue.IssueAmount.ToString("N2");
                    txtMasterID.Text = oEnch.Issue.MasterNo;

                    
                    if (oEnch.Issue.VersionSPPolicy.DTGeneralInterestPolicy.Rows.Count > 0)
                    {
                        txtPDInterestRate.Text = Convert.ToString(oEnch.Issue.VersionSPPolicy.DTGeneralInterestPolicy.Rows[0]["ClaimRate"]);
                    }
                    ddlPDPaymentMode.Items.Clear();
                    DDListUtil.Assign(ddlPDPaymentMode, oEnch.Issue.VersionSPPolicy.DTPaymentPolicy, true);

                    ddlPDCurrency.Items.Clear();
                    DDListUtil.Assign(ddlPDCurrency, oEnch.Issue.VersionSPPolicy.DTCurrencyActivityPolicy, true);


                    if (string.IsNullOrEmpty(txtEncashTransNo.Text))
                    {
                        txtPaymentDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
                        DDListUtil.Assign(ddlPDPaymentMode, "6"); // this is for Customer Account                  
                        if (oEnch.Issue.SPType.SPTypeID.Equals("WDB"))
                        {
                            DDListUtil.Assign(ddlPDCurrency, "00"); //BDT: Default currency BDT for WDB
                        }
                        else
                        {
                            DDListUtil.Assign(ddlPDCurrency, oEnch.Issue.SPType.Currency.CurrencyID);
                        }
                        txtPDConvRate.Enabled = false;
                    }
                    else
                    {
                        txtPaymentDate.Text = oEnch.PaidDate.ToString(Constants.DATETIME_FORMAT);
                        DDListUtil.Assign(ddlPDPaymentMode, oEnch.PrincipalPaymentMode.PaymentMode);
                        DDListUtil.Assign(ddlPDCurrency, oEnch.PrincipalCurrency.CurrencyID);
                        if (oEnch.Issue.SPType.SPTypeID.Equals("WDB") && oEnch.PrincipalCurrency.CurrencyID.Equals("00"))
                        {
                            txtPDConvRate.Enabled = false;
                        }
                        else if (oEnch.Issue.SPType.Currency.CurrencyID != oEnch.PrincipalCurrency.CurrencyID)
                        {
                            txtPDConvRate.Enabled = true;
                        }
                        else
                        {
                            txtPDConvRate.Enabled = false;
                        }
                        txtPDConvRate.Text = Convert.ToString(oEnch.InterestConvRate);
                    }

                    int iDuration = oEnch.Issue.VersionSPPolicy.SPDuration;
                    bool bInMonth = oEnch.Issue.VersionSPPolicy.IsSPDurationInMonth;
                    DateTime dtMaturity = DateTime.Now;
                    dtMaturity = oEnch.Issue.VersionIssueDate;
                    if (bInMonth)
                    {
                        dtMaturity = dtMaturity.AddMonths(iDuration);
                    }
                    else
                    {
                        dtMaturity = dtMaturity.AddYears(iDuration);
                    }

                    if (DateTime.Now.Date < dtMaturity.Date)
                    {
                        chkMarkAsPremature.Checked = true;
                    }
                    else
                    {
                        chkMarkAsPremature.Checked = false;
                    }

                    #region Payment Details
                    txtPDMaturityDate.Text = dtMaturity.ToString(Constants.DATETIME_FORMAT);
                    if (!string.IsNullOrEmpty(oEnch.PrincipalAccountNo) && oEnch.PrincipalAccountNo.Length >= 12)
                    {
                        txtPDAccountNo.Text = oEnch.PrincipalAccountNo.Substring(0, 12);
                        txtPDAccountName.Text = oEnch.AccName;
                    }
                    txtPDPrincipleAmount.Text = oEnch.PrincipalAmount.ToString("N2");
                    txtPDCalcInterest.Text = oEnch.CalculatedInterest.ToString("N2");
                    txtPDPaidInterest.Text = oEnch.AlreadyPaidInterest.ToString("N2");
                    txtPDLevi.Text = "0.00";
                    txtPDIncomeTax.Text = oEnch.CalculatedIncomeTax.ToString("N2");
                    txtPDInterestAmount.Text = oEnch.InterestToBePaid.ToString("N2");
                    txtPDInterestPayable.Text = oEnch.InterestAmount.ToString("N2");
                    if (oEnch.Issue.SPType.Currency.CurrencyID.Equals(oEnch.PrincipalCurrency.CurrencyID.Trim()))
                    {
                        txtPDConvertedAmount.Text = (oEnch.PrincipalAmount + Util.GetDecimalNumber(txtPDInterestPayable.Text)).ToString("N2");
                    }
                    else
                    {
                        if (oEnch.PrincipalCurrency.CurrencyID.Trim().Equals("00"))
                        {
                            txtPDConvertedAmount.Text = ((oEnch.PrincipalAmount + Util.GetDecimalNumber(txtPDInterestPayable.Text)) * (Util.GetDecimalNumber(txtPDConvRate.Text))).ToString("N2");
                        }
                        else
                        {
                            txtPDConvertedAmount.Text = ((oEnch.PrincipalAmount + Util.GetDecimalNumber(txtPDInterestPayable.Text)) / (Util.GetDecimalNumber(txtPDConvRate.Text))).ToString("N2");
                        }
                    }
                    #endregion

                    #region Customer Details
                    DataTable dtCustomerDetails = new DataTable();

                    dtCustomerDetails.Columns.Add(new DataColumn("MasterNo", typeof(string)));
                    dtCustomerDetails.Columns.Add(new DataColumn("Customer Name", typeof(string)));
                    dtCustomerDetails.Columns.Add(new DataColumn("Address", typeof(string)));
                    dtCustomerDetails.Columns.Add(new DataColumn("Phone", typeof(string)));

                    DataRow rowCustomerDetails = null;

                    for (int customerCount = 0; customerCount < oEnch.Issue.CustomerDetailsList.Count; customerCount++)
                    {
                        rowCustomerDetails = dtCustomerDetails.NewRow();

                        rowCustomerDetails["MasterNo"] = oEnch.Issue.CustomerDetailsList[customerCount].MasterNo;
                        rowCustomerDetails["Customer Name"] = oEnch.Issue.CustomerDetailsList[customerCount].CustomerName;
                        rowCustomerDetails["Address"] = oEnch.Issue.CustomerDetailsList[customerCount].Address;
                        rowCustomerDetails["Phone"] = oEnch.Issue.CustomerDetailsList[customerCount].Phone;

                        dtCustomerDetails.Rows.Add(rowCustomerDetails);
                    }

                    gvCustomerDetail.DataSource = dtCustomerDetails;
                    gvCustomerDetail.DataBind();
                    #endregion

                    #region Nominee Detail
                    DataTable dtNomineeDetail = new DataTable();

                    dtNomineeDetail.Columns.Add(new DataColumn("Nominee Name", typeof(string)));
                    dtNomineeDetail.Columns.Add(new DataColumn("Relation", typeof(string)));
                    dtNomineeDetail.Columns.Add(new DataColumn("Address", typeof(string)));
                    dtNomineeDetail.Columns.Add(new DataColumn("Nominee Share", typeof(string)));
                    dtNomineeDetail.Columns.Add(new DataColumn("Amount", typeof(string)));

                    DataRow rowNomineeDetail = null;

                    for (int nomineeCount = 0; nomineeCount < oEnch.Issue.NomineeList.Count; nomineeCount++)
                    {
                        rowNomineeDetail = dtNomineeDetail.NewRow();

                        rowNomineeDetail["Nominee Name"] = oEnch.Issue.NomineeList[nomineeCount].NomineeName;
                        rowNomineeDetail["Relation"] = oEnch.Issue.NomineeList[nomineeCount].Relation;
                        rowNomineeDetail["Address"] = oEnch.Issue.NomineeList[nomineeCount].Address;
                        rowNomineeDetail["Nominee Share"] = oEnch.Issue.NomineeList[nomineeCount].NomineeShare;
                        rowNomineeDetail["Amount"] = oEnch.Issue.NomineeList[nomineeCount].Amount;

                        dtNomineeDetail.Rows.Add(rowNomineeDetail);
                    }
                    gvNomDetail.DataSource = dtNomineeDetail;
                    gvNomDetail.DataBind();

                    #endregion

                    string sRegNo = Request.QueryString[OBJ_REG_NO];
                    if (!string.IsNullOrEmpty(sRegNo))
                    {
                        sRegNo = oCrypManager.GetDecryptedString(sRegNo, Constants.CRYPT_PASSWORD_STRING);
                    }

                    gvEncashmentDetails.DataSource = null;
                    gvEncashmentDetails.DataBind();
                    //Change Grid Column's caption based on SPType
                    if (oEnch.Issue.SPType.SPTypeID.Equals("BSP") || oEnch.Issue.SPType.SPTypeID.Equals("PSP"))
                    {
                        thInstallNo.InnerHtml = "Script No.";
                    }
                    else
                    {
                        thInstallNo.InnerHtml = "Last <br>Instal. No.</br>";
                    }

                    DataView dv = new DataView(oEnch.DtEncashment);

                    if (!string.IsNullOrEmpty(sRegNo) || SEARCH_FROM.Equals(1) || (SEARCH_FROM.Equals(2) && !string.IsNullOrEmpty(hdDataType.Value)))
                    {
                        if (dv.Count > 0)
                        {
                            if (SEARCH_FROM.Equals(1) && hdDataType.Value.Equals("2"))
                            {
                                dv.RowFilter = "Encashed = 'true'";
                            }
                            else
                            {
                                dv.RowFilter = "Selected = 'true'";
                            }
                            if (dv.Count > 0)
                            {
                                gvEncashmentDetails.DataSource = dv;
                                gvEncashmentDetails.DataBind();
                            }
                        }
                    }
                    else
                    {
                        gvEncashmentDetails.DataSource = oEnch.DtEncashment;
                        gvEncashmentDetails.DataBind();
                    }
                    ////Check whether it's Liened
                    //if (SEARCH_FROM.Equals(-1) || (SEARCH_FROM.Equals(2) && string.IsNullOrEmpty(hdDataType.Value)))
                    //{
                    //    if (dv.Count > 0)
                    //    {
                    //        dv.RowFilter = "Lien='true'";
                    //        if (dv.Count > 0)
                    //        {
                    //            ucMessage.OpenMessage("Scrips Liened!!", Constants.MSG_TYPE_INFO);
                    //            ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    //        }
                    //    }
                    //}
                    //PopUp message on Early Encashment for Checker
                    
                    if ((ddlSpType.SelectedValue.Contains("WDB")
                        || ddlSpType.SelectedValue.Contains("DIB")
                        || ddlSpType.SelectedValue.Contains("DPB"))
                        && (DateTime.Now.Date - dtMaturity.Date).Days/365>=2)
                    {
                        ucMessage.OpenMessage("Registration found for reinvestment. Please check!", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    }

                    if (SEARCH_FROM.Equals(0))
                    {
                        if (oEnch.PaidDate.Date < dtMaturity.Date)
                        {
                            ucMessage.OpenMessage("Early Encashment.", Constants.MSG_TYPE_INFO);
                            ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        }
                    }
                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    if (hdDataType.Value.Equals("2") && SEARCH_FROM.Equals(1))
                    {
                        oUserDetails.MakerID = oEnch.UserDetails.MakerID;
                        oUserDetails.MakeDate = oEnch.UserDetails.MakeDate;
                        oUserDetails.CheckerID = oEnch.UserDetails.CheckerID;
                        oUserDetails.CheckDate = oEnch.UserDetails.CheckDate;
                        oUserDetails.CheckerComment = oEnch.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;
                        txtPaymentDate.Text = oEnch.PaidDate.ToString(Constants.DATETIME_FORMAT);
                    }
                    if ((hdDataType.Value.Equals("1") && SEARCH_FROM.Equals(1)) || SEARCH_FROM.Equals(2))
                    {
                        oUserDetails.CheckerID = oEnch.UserDetails.CheckerID;
                        oUserDetails.CheckDate = oEnch.UserDetails.CheckDate;
                        oUserDetails.CheckerComment = oEnch.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;
                    }
                    if (SEARCH_FROM.Equals(0))
                    {
                        oUserDetails.MakerID = oEnch.UserDetails.MakerID;
                        oUserDetails.CheckerComment = oEnch.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;
                    }
                }
            }
        }

        private void LoadDataByRegNo(string sEncshTransNo, string sRegNo, string sApprovalStaus)
        {
            Result oResult = null;
            EncashmentDAL oEnchDAL = new EncashmentDAL();
            string sDivisionID = string.Empty;
            string sBankID = string.Empty;
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (oConfig != null)
            {
                sDivisionID = oConfig.DivisionID;
                sBankID = oConfig.BankCodeID;
            }
            //>>Lien and Stop Payment Checking Start
            LienDAL ld = new LienDAL();
            oResult = ld.CheckScripStatus(sRegNo);
            if (!string.IsNullOrEmpty(oResult.Message))
            {
                if (oResult.Message.Equals("L"))
                {
                    ucMessage.OpenMessage("Some Scrips are Liended!!", Constants.MSG_TYPE_INFO);
                }
                else if (oResult.Message.Equals("S"))
                {
                    ucMessage.OpenMessage("Some Scrips are Stopped!!", Constants.MSG_TYPE_INFO);
                }
                else if (oResult.Message.Equals("E"))
                {
                    ucMessage.OpenMessage("Some Scrips are Encashed!!", Constants.MSG_TYPE_INFO);
                }
                else
                {
                    ucMessage.OpenMessage(oResult.Message, Constants.MSG_TYPE_INFO);
                }

                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
            //<<Lien and Stop Payment Checking End
            oResult = null;
            oResult = (Result)oEnchDAL.LoadDataByRegistrationNo(sEncshTransNo, sRegNo, chkMarkAsPremature.Checked, sApprovalStaus, sDivisionID, sBankID);
            TotalClear();
            if (oResult.Status)
            {
                Encashment oEnch = (Encashment)oResult.Return;
                SetObject(oEnch);
                if (sApprovalStaus.Equals("2") && SEARCH_FROM.Equals(1))
                {
                    EnableDisableControl(true);
                    Util.ControlEnabled(btnReject, false);
                    Util.ControlEnabled(btnApprove, false);
                    Util.ControlEnabled(btnBack, false);

                    Util.ControlEnabled(btnSelect, false);
                    Util.ControlEnabled(btnDeSelect, false);
                    Util.ControlEnabled(btnCalculate, false);

                    Util.ControlEnabled(btnReset, true);
                    Util.ControlEnabled(btnSave, true);
                    Util.ControlEnabled(btnDelete, true);

                    btnRegSearch.Enabled = true;
                    btnEncashTransSearch.Enabled = true;

                    fsList.Visible = true;
                }
                else if (SEARCH_FROM.Equals(0))
                {
                    EnableDisableControl(true);
                    Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);
                }
                else if (sApprovalStaus == "1" || SEARCH_FROM.Equals(2))
                {
                    EnableDisableControl(false);
                }
                //CalculateEncashment();               
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
            }
        }

        private void TotalClear()
        {
            EnableDisableControl(false);
            // IntPayment set in session 
            Encashment oEncashment = new Encashment();
            if (Session[Constants.SES_ENCASHMENT] == null)
            {
                Session.Add(Constants.SES_ENCASHMENT, oEncashment);
            }
            else
            {
                Session[Constants.SES_ENCASHMENT] = oEncashment;
            }

            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            gvEncashmentDetails.DataSource = null;
            gvEncashmentDetails.DataBind();

            txtIssueDate.Text = string.Empty;
            txtPaymentDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            hdEncashTransNo.Value = "";
            hdIssueTransNo.Value = "";
            hdRegNo.Value = "";

            chkIndiYesNo.Checked = false;
            chkMarkAsPremature.Checked = false;

            //Payment Details
            txtPDMaturityDate.Text = string.Empty;
            txtPDPrincipleAmount.Text = "0.00";
            txtPDCalcInterest.Text = "0.00";
            txtPDPaidInterest.Text = "0.00";
            txtPDInterestAmount.Text = "0.00";
            txtPDLevi.Text = "0.00";
            txtPDIncomeTax.Text = "0.00";
            txtPDInterestPayable.Text = "0.00";
            txtPDConvRate.Text = "1.00";
            txtPDAccountNo.Text = "";
            txtPDDraftNo.Text = "";
            txtPDConvertedAmount.Text = "0.00";
            txtPDAccountName.Text = "";
            txtPDInterestRate.Text = "0.00";
            ddlPDCurrency.Items.Clear();
            ddlPDPaymentMode.Items.Clear();

            // Dropdown load SPType
            ddlSpType.SelectedIndex = 0;
            ddlBranch.SelectedIndex = 0;

            //Interest Payment Details
            txtEncashTransNo.Text = string.Empty;

            //Issue Details
            txtRegistrationNo.Text = string.Empty;
            txtIssueName.Text = string.Empty;
            txtTotalAmount.Text = string.Empty;
            txtMasterID.Text = string.Empty;

            ucUserDet.ResetData();
        }


        private void CalculateEncashment(bool changeRateType)
        {
            decimal dCalInt = 0;
            decimal dPaidInt = 0;
            decimal dSSP = 0;
            decimal dPrincAmnt = 0;
            decimal dLevi = 0;
            decimal dIncomeTax = 0;
            int iCount = 0;
            Encashment oEnch = Session[Constants.SES_ENCASHMENT] as Encashment;

            foreach (GridViewRow gvr in gvEncashmentDetails.Rows)
            {
                if ((gvr.FindControl("chkSelected") as CheckBox).Checked)
                {
                    dCalInt += Util.GetDecimalNumber((gvr.FindControl("txtTotalInterest") as TextBox).Text);
                    dPaidInt += Util.GetDecimalNumber((gvr.FindControl("lblPaidInterest") as Label).Text);
                    dPrincAmnt += Util.GetDecimalNumber((gvr.FindControl("lblDenomination") as Label).Text);

                    // ZQ IT IS WORKING BUT FOR THE TIME IT HAS STOPED FOR ON DEMAND..  
                    dIncomeTax += Util.GetDecimalNumber((gvr.FindControl("lblIncomeTax") as Label).Text);
                    iCount += 1;
                }
            }

            if (dCalInt - dPaidInt > 0) // && oEnch.Issue.VersionSPPolicy.IncomeTaxApplyAmount > 0)
            {
                dIncomeTax = 0;
                InterestPaymentDAL oIntPayDAL = new InterestPaymentDAL();
                Result oResult = oIntPayDAL.GetCalculatedIncomeTax(oEnch.Issue.IssueTransNo, Convert.ToDouble(dCalInt - dPaidInt), DateTime.Today.Date, changeRateType);
                if (oResult.Status)
                {
                    dIncomeTax = Convert.ToDecimal(oResult.Return);
                }
            }
            if (changeRateType)
            {
                if (Convert.ToDecimal(oEnch.Issue.VersionSPPolicy.IncomeTax) == 10)
                {
                    oEnch.Issue.VersionSPPolicy.IncomeTax = 5;
                }
                else
                {
                    oEnch.Issue.VersionSPPolicy.IncomeTax = 10;
                }
            }
            if (oEnch.Issue.SPType.SPTypeID == "BSP" &&  Util.GetDateTimeByString(txtPaymentDate.Text) >= Util.GetDateTimeByString(txtPDMaturityDate.Text))
            {
                if (oEnch.Issue.VersionSPPolicy.SocialSecurityAmount > 0)
                {
                    decimal dRIntRate = Convert.ToDecimal(txtPDInterestRate.Text) - Convert.ToDecimal(oEnch.Issue.VersionSPPolicy.SocialSecurityAmount);
                    decimal dRSSPRate = Convert.ToDecimal(oEnch.Issue.VersionSPPolicy.SocialSecurityAmount);
                    decimal dRTaxRate = Convert.ToDecimal(oEnch.Issue.VersionSPPolicy.IncomeTax);
                    decimal dIncomeTaxApplyAmount = Convert.ToDecimal(oEnch.Issue.VersionSPPolicy.IncomeTaxApplyAmount);

                    //


                    decimal dRCalcInt = dPrincAmnt * (dRIntRate / 100) * 5;
                    decimal dRIncomeTax = dRCalcInt * (dRTaxRate / 100);
                    decimal dRSSPAmount = dPrincAmnt * (dRSSPRate / 100) * 5;
                    dIncomeTax = dRIncomeTax;
                    dCalInt = dRCalcInt + dRSSPAmount;
                }
                //else if (oEnch.Issue.CustomerType.CustomerTypeID == "03")
                //{
                //    if (Util.GetDateTimeByString(txtPDMaturityDate.Text) >= Util.GetDateTimeByString("01/07/2016"))
                //    {
                //        dIncomeTax = (dCalInt * 5) / 100;
                //    }
                //    else
                //    {
                //        dIncomeTax = 0;
                //    }
                //}
            }
            else if (dCalInt - dPaidInt > 0)
            {
                decimal dRTaxRate = Convert.ToDecimal(oEnch.Issue.VersionSPPolicy.IncomeTax);
                dIncomeTax = (dCalInt - dPaidInt) * (dRTaxRate / 100);
            }
            else 
            {
                dIncomeTax = 0;
            }
            if (oEnch.Issue.SPType.SPTypeID == "BSP"
                && Util.GetDateTimeByString(txtPaymentDate.Text) >= Util.GetDateTimeByString(txtPDMaturityDate.Text)
                && Util.GetDateTimeByString(txtIssueDate.Text) > Util.GetDateTimeByString("09/08/2000")
                && !(Util.GetDateTimeByString(txtIssueDate.Text) >= Util.GetDateTimeByString("01/07/2005") && Util.GetDateTimeByString(txtIssueDate.Text) <= Util.GetDateTimeByString("30/06/2006")))
                //&& oEnch.Issue.CustomerType.CustomerTypeID != "03")
            {

                TimeSpan ts;
                ts = Util.GetDateTimeByString(txtPaymentDate.Text) - Util.GetDateTimeByString(txtPDMaturityDate.Text);

                int iDuration = Convert.ToInt32(Math.Floor(ts.TotalDays / 365));
                if (iDuration > 0)
                {

                    EncashmentDAL oEncashmentDAL = new EncashmentDAL();
                    Result oResultR = oEncashmentDAL.LoadReinvestmentPolicy(oEnch.Issue.SPType.SPTypeID, Util.GetDateTimeByString(txtPDMaturityDate.Text), iDuration);

                    DataTable dataRe = oResultR.Return as DataTable;


                    if (iDuration > 4)
                    {
                        iDuration = 5;
                    }

                    decimal dRIntRate = Convert.ToDecimal(dataRe.Rows[0]["InterestRate"]) - Convert.ToDecimal(dataRe.Rows[0]["SocialSecurityAmount"]);
                    decimal dRSSPRate = Convert.ToDecimal(dataRe.Rows[0]["SocialSecurityAmount"]);
                    decimal dRTaxRate = Convert.ToDecimal(oEnch.Issue.VersionSPPolicy.IncomeTax);
                    decimal dIncomeTaxApplyAmount = Convert.ToDecimal(dataRe.Rows[0]["IncomeTaxApplyAmount"]);

                    //


                    decimal dRCalcInt = (dPrincAmnt + (dCalInt - dIncomeTax)) * (dRIntRate / 100) * iDuration;
                    decimal dRIncomeTax = dRCalcInt * (dRTaxRate / 100);
                    decimal dRSSPAmount = (dPrincAmnt + (dCalInt - dIncomeTax)) * (dRSSPRate / 100) * 5;

                    dCalInt += dRCalcInt + dRSSPAmount;

                    if (dCalInt - dPaidInt > 0 && dIncomeTaxApplyAmount > 0)
                    {
                        if (oEnch.Issue.CustomerType.CustomerTypeID == "01")
                        {
                            if ((dRCalcInt - dIncomeTaxApplyAmount) > 0)
                            {
                                dRIncomeTax = (dRCalcInt - dIncomeTaxApplyAmount) * (dRTaxRate / 100);
                            }
                            else
                            {
                                dRIncomeTax = 0;
                            }
                        }
                        else
                        {
                            if ((dRCalcInt - dIncomeTaxApplyAmount) > 0)
                            {
                                dRIncomeTax = (dRCalcInt - dIncomeTaxApplyAmount * 2) * (dRTaxRate / 100);
                            }
                            else
                            {
                                dRIncomeTax = 0;
                            }
                        }
                    }

                    dIncomeTax += dRIncomeTax;

                    decimal dtmpIInt = 0;
                    dtmpIInt = dCalInt / iCount;


                    foreach (GridViewRow gvr in gvEncashmentDetails.Rows)
                    {
                        if ((gvr.FindControl("chkSelected") as CheckBox).Checked)
                        {
                            (gvr.FindControl("txtTotalInterest") as TextBox).Text = dtmpIInt.ToString("N2");
                            gvr.Cells[8].Text = (dtmpIInt - Util.GetDecimalNumber((gvr.FindControl("lblPaidInterest") as Label).Text)).ToString("N2");
                            gvr.Cells[10].Text = (Util.GetDecimalNumber((gvr.FindControl("lblDenomination") as Label).Text) + Util.GetDecimalNumber(gvr.Cells[8].Text) - Util.GetDecimalNumber((gvr.FindControl("lblIncomeTax") as Label).Text)).ToString("N2");
                        }
                    }
                }
            }
            //txtPDIncomeTax.Text = dIncomeTax.ToString("N2");


            #region Assign Data in calculation field set
            txtPDCalcInterest.Text = dCalInt.ToString("N2");
            txtPDPaidInterest.Text = dPaidInt.ToString("N2");
            txtPDInterestAmount.Text = (dCalInt - dPaidInt).ToString("N2");
            txtPDPrincipleAmount.Text = dPrincAmnt.ToString("N2");
            txtPDLevi.Text = dLevi.ToString("N2");
            txtPDIncomeTax.Text = dIncomeTax.ToString("N2");
            txtPDInterestPayable.Text = (dCalInt - dPaidInt - dLevi - dIncomeTax).ToString("N2");
            if (oEnch.Issue.SPType.Currency.CurrencyID.Equals(oEnch.PrincipalCurrency.CurrencyID.Trim()))
            {
                txtPDConvertedAmount.Text = (dPrincAmnt + Util.GetDecimalNumber(txtPDInterestPayable.Text)).ToString("N2");
            }
            else
            {
                if (oEnch.PrincipalCurrency.CurrencyID.Trim().Equals(Constants.CURRENCY_CODE_BDT))
                {
                    txtPDConvertedAmount.Text = (dPrincAmnt + Util.GetDecimalNumber(txtPDInterestPayable.Text) * (Util.GetDecimalNumber(txtPDConvRate.Text))).ToString("N2");
                }
                else
                {
                    if (Util.GetDecimalNumber(txtPDConvRate.Text) > 0)
                    {
                        txtPDConvertedAmount.Text = ((dPrincAmnt + Util.GetDecimalNumber(txtPDInterestPayable.Text)) / Util.GetDecimalNumber(txtPDConvRate.Text)).ToString("N2");
                    }
                }
            }
            CalculateIncomTax();


            #endregion
        }

        private void EnableDisableControl(bool isApproved)
        {
            if (isApproved)
            {
                //Interest Payment Details
                Util.ControlEnabled(txtEncashTransNo, false);
                Util.ControlEnabled(txtPaymentDate, false);
                Util.ControlEnabled(chkMarkAsPremature, false);
                Util.ControlEnabled(chkIndiYesNo, false);


                //Issue Details
                Util.ControlEnabled(txtRegistrationNo, false);

                //Customer(s) Details
                Util.ControlEnabled(gvCustomerDetail, false);

                //Nominee(s) Details
                Util.ControlEnabled(gvNomDetail, false);

                //Coupon \ Installments Details

                gvEncashmentDetails.Enabled = false;
                //Payment Details
                Util.ControlEnabled(ddlPDPaymentMode, false);
                Util.ControlEnabled(ddlPDCurrency, false);
                Util.ControlEnabled(txtPDIncomeTax, false);
                Util.ControlEnabled(txtPDConvRate, false);
                Util.ControlEnabled(txtPDAccountNo, false);

                // button
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true);

                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);
                Util.ControlEnabled(btnSelect, false);
                Util.ControlEnabled(btnDeSelect, false);
                Util.ControlEnabled(btnCalculate, false);
                btnEncashTransSearch.Enabled = false;
                btnRegSearch.Enabled = false;
                fsList.Visible = false;
            }
            else
            {
                // button 
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);

                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);

                btnEncashTransSearch.Enabled = true;
                btnRegSearch.Enabled = true;

                //Interest Payment Details
                Util.ControlEnabled(txtEncashTransNo, true);
                Util.ControlEnabled(txtPaymentDate, true);
                Util.ControlEnabled(chkMarkAsPremature, true);
                Util.ControlEnabled(chkIndiYesNo, true);

                //Issue Details
                Util.ControlEnabled(txtRegistrationNo, true);

                //Customer(s) Details
                Util.ControlEnabled(gvCustomerDetail, true);

                //Nominee(s) Details
                Util.ControlEnabled(gvNomDetail, true);

                //Coupon \ Installments Details
                Util.ControlEnabled(gvEncashmentDetails, true);

                //Payment Details
                Util.ControlEnabled(ddlPDPaymentMode, true);
                Util.ControlEnabled(ddlPDCurrency, true);
                Util.ControlEnabled(txtPDIncomeTax, true);
                //Util.ControlEnabled(txtPDConvRate, true);
                Util.ControlEnabled(txtPDAccountNo, true);
                //buttons
                Util.ControlEnabled(btnSelect, true);
                Util.ControlEnabled(btnDeSelect, true);
                Util.ControlEnabled(btnCalculate, true);

                fsList.Visible = true;
            }
        }
        #endregion Util Method...

        protected void ddlPDCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlPDCurrency.SelectedValue))
            {
                txtPDConvRate.Text = "0.00";
                txtPDConvRate.Enabled = false;
            }
            else
            {
                Encashment oEnch = Session[Constants.SES_ENCASHMENT] as Encashment;
                if (oEnch != null)
                {
                    if (oEnch.Issue.SPType.SPTypeID.Equals("WDB") // for WDB
                        && ddlPDCurrency.SelectedValue.Equals("00")) //for BDT
                    {
                        txtPDConvRate.Enabled = false;
                        txtPDConvRate.Text = "1.00";
                    }
                    else if (ddlPDCurrency.SelectedValue != oEnch.Issue.SPType.Currency.CurrencyID)
                    {
                        txtPDConvRate.Enabled = true;
                        txtPDConvRate.Text = "0.00";
                    }
                    else
                    {
                        txtPDConvRate.Text = "1.00";
                        txtPDConvRate.Enabled = false;
                    }
                }
            }

            txtPDConvertedAmount.Text = (Util.GetDecimalNumber(txtPDConvRate.Text) * (Util.GetDecimalNumber(txtPDPrincipleAmount.Text) + Util.GetDecimalNumber(txtPDInterestPayable.Text))).ToString("N2");
        }

        protected void txtPDIncomeTax_TextChanged(object sender, EventArgs e)
        {
            CalculateIncomTax();
        }
        private void CalculateIncomTax()
        {
            Encashment oEnch = Session[Constants.SES_ENCASHMENT] as Encashment;
            int iCount = 0;
            foreach (GridViewRow gvr in gvEncashmentDetails.Rows)
            {
                if ((gvr.FindControl("chkSelected") as CheckBox).Checked)
                {
                    iCount += 1;
                }
            }
            decimal dIncomeTax = 0;
            if (iCount > 0)
            {
                dIncomeTax = Util.GetDecimalNumber(txtPDIncomeTax.Text);

                decimal dtmpITax = 0;
                dtmpITax = dIncomeTax / iCount;


                foreach (GridViewRow gvr in gvEncashmentDetails.Rows)
                {
                    if ((gvr.FindControl("chkSelected") as CheckBox).Checked)
                    {
                        (gvr.FindControl("lblIncomeTax") as Label).Text = dtmpITax.ToString("N2");
                        gvr.Cells[10].Text = (Util.GetDecimalNumber((gvr.FindControl("lblDenomination") as Label).Text) + Util.GetDecimalNumber(gvr.Cells[8].Text) - dtmpITax).ToString("N2");
                    }
                }
            }
            txtPDInterestPayable.Text = (Util.GetDecimalNumber(txtPDCalcInterest.Text) - Util.GetDecimalNumber(txtPDPaidInterest.Text) - Convert.ToDecimal(dIncomeTax)).ToString("N2");
            if (oEnch.Issue.SPType.Currency.CurrencyID.Equals(oEnch.PrincipalCurrency.CurrencyID.Trim()))
            {
                txtPDConvertedAmount.Text = (Util.GetDecimalNumber(txtPDPrincipleAmount.Text) + Util.GetDecimalNumber(txtPDInterestPayable.Text)).ToString("N2");
            }
            else
            {
                if (oEnch.PrincipalCurrency.CurrencyID.Trim().Equals(Constants.CURRENCY_CODE_BDT))
                {
                    txtPDConvertedAmount.Text = (Util.GetDecimalNumber(txtPDPrincipleAmount.Text) + Util.GetDecimalNumber(txtPDInterestPayable.Text) * (Util.GetDecimalNumber(txtPDConvRate.Text))).ToString("N2");
                }
                else
                {
                    if (Util.GetDecimalNumber(txtPDConvRate.Text) > 0)
                    {
                        txtPDConvertedAmount.Text = ((Util.GetDecimalNumber(txtPDPrincipleAmount.Text) + Util.GetDecimalNumber(txtPDInterestPayable.Text)) / Util.GetDecimalNumber(txtPDConvRate.Text)).ToString("N2");
                    }
                }
            }
        }

        protected void txtRegistrationNo_TextChanged(object sender, EventArgs e)
        {
            btnRegSearch_Click(sender, e);
        }

        protected void btnViewJournals_Click(object sender, EventArgs e)
        {
            Encashment oEnch = Session[Constants.SES_ENCASHMENT] as Encashment;
            SBM_BLC1.DAL.Common.ReportDAL rdal = new SBM_BLC1.DAL.Common.ReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                if (oEnch.UserDetails.CheckerID.Trim() != "")
                {
                    oResult = rdal.PrincipleAdviceReport(oEnch.EncashmentTransNo, true);
                }
                else
                {
                    oResult = rdal.PrincipleAdviceReport(oEnch.EncashmentTransNo, false);
                }
                if (oResult.Status)
                {
                    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                }
            }
        }

        protected void btnTaxAdjustment_Click(object sender, EventArgs e)
        {
            CalculateEncashment(true);
        }
    }
}
