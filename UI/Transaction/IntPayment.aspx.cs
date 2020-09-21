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
using SBM_BLC1.DAL.Report;

namespace SBM_WebUI.mp
{
    public partial class IntPayment : System.Web.UI.Page
    {

        #region Local Variable
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_REG_NO = "sRegNo";
        public const string OBJ_PAGE_ID = "sPageID";
        public int SEARCH_FROM = 0;
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_TRANS.INTEREST_PAYMENT))
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
            TotalClear();
            gvData.PageSize = (int)Constants.PAGING_UNAPPROVED;
            SIssue.Type = Convert.ToString((int)Constants.SEARCH_ISSUE.INTEREST_PAYMENT);

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
                    SEARCH_FROM = 3;
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    LoadDataByRegNo("", sRegNo, "1");

                    #region User-Detail.
                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    oUserDetails.MakeDate = DateTime.Now;
                    oUserDetails.CheckerID = oConfig.UserName;
                    oUserDetails.CheckDate = DateTime.Now;
                    ucUserDet.UserDetail = oUserDetails;
                    #endregion User-Detail.

                    fsList.Visible = false;
                }
            }
            else
            {
                EnableDisableControl(false);

                #region User-Detail
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                UserDetails oUserDetails = new UserDetails();
                oUserDetails.MakerID = oConfig.UserName;
                oUserDetails.MakeDate = DateTime.Now;
                ucUserDet.UserDetail = oUserDetails;
                #endregion

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

                string sTxtPaymentDate = txtPaymentDate.Text;
                bool bChkMarkAsPremature = chkMarkAsPremature.Checked;
                bool bChkIndiYesNo = chkIndiYesNo.Checked;
                LoadDataByRegNo("", txtRegistrationNo.Text, "");
                txtPaymentDate.Text = sTxtPaymentDate;
                chkMarkAsPremature.Checked = bChkMarkAsPremature;
                chkIndiYesNo.Checked = bChkIndiYesNo;
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

        protected void ddlPDCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPDAccountNo.Text = string.Empty;
            txtPDAccountName.Text = string.Empty;
            if (string.IsNullOrEmpty(ddlPDCurrency.SelectedValue))
            {
                txtPDConvRate.Text = "0.00";
                txtPDConvRate.Enabled = false;
            }
            else
            {
                InterestPayment oIntPay = Session[Constants.SES_INTEREST_PAYMENT] as InterestPayment;
                if (oIntPay != null)
                {
                    if (oIntPay.Issue.SPType.SPTypeID.Equals("WDB") // for WDB
                        && ddlPDCurrency.SelectedValue.Equals("00")) //for BDT
                    {
                        txtPDConvRate.Enabled = false;
                        txtPDConvRate.Text = "1.00";
                    }
                    else if (ddlPDCurrency.SelectedValue != oIntPay.Issue.SPType.Currency.CurrencyID)
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

            txtPDConvertedAmount.Text = (Util.GetDecimalNumber(txtPDConvRate.Text) * (Util.GetDecimalNumber(txtPDPaymentAmount.Text))).ToString("N2");
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
                SEARCH_FROM = 4;
                hdDataType.Value = "";
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                LoadDataByRegNo("", gvRow.Cells[1].Text, "1");

            }
        }

        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");

            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.INTEREST_PAYMENT).PadLeft(5, '0'), false);
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
            Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.INTEREST_PAYMENT).PadLeft(5, '0'), false);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdIntTransNo.Value))
            {
                InterestPayment oIntPay = new InterestPayment(hdIntTransNo.Value);
                InterestPaymentDAL oIntPayDAL = new InterestPaymentDAL();
                oIntPay.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oIntPayDAL.Reject(oIntPay);
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
            if (!string.IsNullOrEmpty(hdIntTransNo.Value))
            {
                InterestPayment oIntPay = GetObject();//new InterestPayment(hdIntTransNo.Value);
                #region For Account Entries
                InterestPayment oIntPaySess = Session[Constants.SES_INTEREST_PAYMENT] as InterestPayment;
                if (oIntPaySess != null)
                {
                    oIntPay.Issue.RegNo = oIntPaySess.Issue.RegNo;
                    oIntPay.Issue.SPType.SPTypeID = oIntPaySess.Issue.SPType.SPTypeID;
                    oIntPay.Issue.IssueName = oIntPaySess.Issue.IssueName;
                }
                #endregion
                InterestPaymentDAL oIntPayDAL = new InterestPaymentDAL();
                oIntPay.UserDetails = ucUserDet.UserDetail;
                Result oResult = (Result)oIntPayDAL.Approve(oIntPay);
                if (oResult.Status)
                {
                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE, Constants.MSG_TYPE_SUCCESS);
                    //SBM_BLC1.DAL.Common.ReportDAL rdal = new SBM_BLC1.DAL.Common.ReportDAL();
                    //oResult = new Result();
                    //oResult = rdal.InerestAdviceReport(oIntPay.IntPaymentTransNo, true);
                    //if (oResult.Status)
                    //{
                    //    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    //    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                    //}
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
                    if (Util.GetDoubleNumber(txtPDConvertedAmount.Text) <= 0)
                    {
                        ucMessage.OpenMessage("Conversion Rate must be greater than 0.Please correct", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        return;
                    }

                    InterestPayment oInterestPayment = GetObject();
                    oInterestPayment.UserDetails = ucUserDet.UserDetail;
                    oInterestPayment.UserDetails.MakeDate = DateTime.Now;
                    ucUserDet.ResetData();
                    InterestPaymentDAL oInterestPaymentDAL = new InterestPaymentDAL();
                    Result oResult = oInterestPaymentDAL.Save(oInterestPayment);

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

        protected void btnShowDetails_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdIssueTransNo.Value))
            {
                InterestPaymentDAL oIntPayDAL = new InterestPaymentDAL();
                Result oResult = null;
                oResult = (Result)oIntPayDAL.LoadShowDetailTransByRegistrationNo(hdIssueTransNo.Value);
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
            //if (!string.IsNullOrEmpty(hdIssueTransNo.Value) && ddlSpType.SelectedIndex != 0)
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

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            CalculatePayment(false);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!hdDataType.Value.Equals("2"))
            {
                if (!string.IsNullOrEmpty(hdIntTransNo.Value))
                {
                    InterestPaymentDAL oIntPayDAL = new InterestPaymentDAL();
                    Result oResult = (Result)oIntPayDAL.Detete(hdIntTransNo.Value);
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
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }

        public void IPELoadAction(string sTransNo, string sRegNo, string sApprovalStaus)
        {
            if (!string.IsNullOrEmpty(sRegNo))
            {
                SEARCH_FROM = 1;
                hdDataType.Value = sApprovalStaus;
                LoadDataByRegNo(sTransNo, sRegNo, sApprovalStaus);

            }
        }

        public void PopupIssueSearchLoadAction(string sRegNo, string sTransNo, string sApprovalStaus)
        {
            if (!string.IsNullOrEmpty(sRegNo))
            {
                SEARCH_FROM = 2;
                hdDataType.Value = "";
                LoadDataByRegNo("", sRegNo, sApprovalStaus);
            }
        }

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            if (ValidateScripAndCouponRangeSelection())
            {
                SelectDeselectPaymentGrid(null, true, true);
            }
            else
            {
                foreach (GridViewRow gvr in gvPaymentDetails.Rows)
                {
                    if ((gvr.FindControl("chkSelected") as CheckBox).Enabled)
                    {
                        SelectDeselectPaymentGrid(gvr, true, false);
                    }
                }
            }
            CalculatePayment(false);
        }

        protected void btnDeSelect_Click(object sender, EventArgs e)
        {
            if (ValidateScripAndCouponRangeSelection())
            {
                SelectDeselectPaymentGrid(null, false, true);
            }
            else
            {
                foreach (GridViewRow gvr in gvPaymentDetails.Rows)
                {
                    if ((gvr.FindControl("chkSelected") as CheckBox).Checked)
                    {
                        SelectDeselectPaymentGrid(gvr, false, false);
                    }
                }
            }
            CalculatePayment(false);
        }

        #endregion Event Method...


        #region Util Method...

        private void LoadDataByRegNo(string sInterestPayTansNo, string sRegNo, string sApprovalStatus)
        {
            InterestPaymentDAL oIntPayDAL = new InterestPaymentDAL();
            Result oResult = null;
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
            oResult = (Result)oIntPayDAL.LoadDataByRegistrationNo(sInterestPayTansNo, sRegNo, chkMarkAsPremature.Checked, sApprovalStatus, sDivisionID, sBankID);

            TotalClear();

            if (oResult.Status)
            {
                InterestPayment oInterestPayment = (InterestPayment)oResult.Return;
                SetObject(oInterestPayment);
                //CalculatePayment();
                if (hdDataType.Value.Equals("2"))
                {
                    EnableDisableControl(true);
                    Util.ControlEnabled(btnReject, false);
                    Util.ControlEnabled(btnApprove, false);
                    Util.ControlEnabled(btnBack, false);

                    Util.ControlEnabled(btnReset, true);
                    Util.ControlEnabled(btnSave, true);
                    Util.ControlEnabled(btnDelete, true);
                    Util.ControlEnabled(btnIntPayTransSearch, true);
                    Util.ControlEnabled(btnRegSearch, true);

                    fsList.Visible = true;
                }
                else if (SEARCH_FROM.Equals(3))
                {
                    EnableDisableControl(true);
                    Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);
                }
                else
                {
                    EnableDisableControl(false);

                    if (ddlPDPaymentMode.SelectedItem.Value.Equals("1") || ddlPDPaymentMode.SelectedItem.Value.Equals("2"))
                    {
                        txtPDAccountNo.Enabled = false;
                        txtPDDraftNo.Enabled = false;
                    }
                    else if (ddlPDPaymentMode.SelectedItem.Value.Equals("3"))
                    {
                        txtPDAccountNo.Enabled = false;
                        txtPDDraftNo.Enabled = true;
                    }
                    else
                    {
                        txtPDAccountNo.Enabled = true;
                        txtPDDraftNo.Enabled = false;
                    }
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
            }
        }

        private void SelectDeselectPaymentGrid(GridViewRow gvr, bool isSelect, bool isRangeSelection)
        {
            if (isRangeSelection)
            {
                int iFromCoupon = Convert.ToInt32(txtFromCoupon.Text);
                int iToCoupon = Convert.ToInt32(txtToCoupon.Text);
                //Formula of seekig starting postion in the GridRows: (iFromCoupon * ScripsCount) - ScripsCount
                int iFromIndx = (iFromCoupon * (ddlScrips.Items.Count - 1)) - (ddlScrips.Items.Count - 1);
                //Formula of seekig ending postion in the GridRows: iToCoupon * ScripsCount - 1
                int iToIndx = iToCoupon * (ddlScrips.Items.Count - 1) - 1;
                int iFromScrip = Convert.ToInt32(txtFromScrip.Text);
                int iToScrip = Convert.ToInt32(txtToScrip.Text);

                if (iFromScrip > iToScrip)
                {
                    ucMessage.OpenMessage("<b>To Scrip</b> should be greater or equal to <b>From Scrip</b>!", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    return;
                }
                else if (iFromCoupon > iToCoupon)
                {
                    ucMessage.OpenMessage("<b>To Coupon</b> no. should be greater or equal to <b>From Coupon</b> no.!", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    return;
                }
                else if (iFromCoupon == 0 || iToCoupon == 0 || iToCoupon > ddlCouponInstalNo.Items.Count - 1)//deducting the blank(<-All->) item
                {
                    ucMessage.OpenMessage("Invalid coupon no.!", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    return;
                }
                else if ((iToCoupon - iFromCoupon)>600)
                {
                    ucMessage.OpenMessage("<b>Script range </b> should be less then or equal to <b>600</b>.!", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    return;
                }
                else
                {
                    int scipCount = iToScrip - iFromScrip + 1;


                    if (Session[Constants.SES_INTEREST_PAYMENT] != null)
                    {
                        InterestPayment iPayment = ((InterestPayment)Session[Constants.SES_INTEREST_PAYMENT]);
                        DataTable dtTmpList = iPayment.DtPayment;
                        for (int i = 0; i < dtTmpList.Rows.Count; i++)
                        {
                            string[] sCheckScript = Convert.ToString(dtTmpList.Rows[i]["CertificatNo"]).Split(' ');
                            int iCheckScript = Convert.ToInt32(sCheckScript[sCheckScript.Length - 1]);

                            if (iCheckScript >= iFromScrip && iCheckScript <= iToScrip)
                            {
                                dtTmpList.Rows[i]["Selected"] = true;
                            }
                        }
                        DataView dv = new DataView(dtTmpList);
                        if (dv.Count > 0)
                        {
                            dv.RowFilter = "Selected = 'true'";
                            if (dv.Count > 0)
                            {
                                gvPaymentDetails.DataSource = dv;
                                gvPaymentDetails.DataBind();
                            }
                        }
                        else
                        {
                            ucMessage.OpenMessage("Invalid <b> Script range</b>. No record selected. Please Check.", Constants.MSG_TYPE_INFO);
                            ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                            return;
                        }
                        iPayment.DtPayment = dtTmpList;
                        Session[Constants.SES_INTEREST_PAYMENT] = iPayment;
                    }

                    //for (int i = 0; i < gvPaymentDetails.Rows.Count ; i++)
                    //{
                    //    string[] sCheckScrip = gvPaymentDetails.Rows[i].Cells[4].Text.Split(' ');
                    //    int iCheckScrip = Convert.ToInt32(sCheckScrip[sCheckScrip.Length - 1]);
                    //    if (iCheckScrip >= iFromScrip && iCheckScrip <= iToScrip)
                    //    {
                    //        if (!(gvPaymentDetails.Rows[i].FindControl("chkStopPayment") as CheckBox).Checked)
                    //        {
                    //            if ((gvPaymentDetails.Rows[i].FindControl("chkSelected") as CheckBox).Enabled)
                    //            {
                    //                (gvPaymentDetails.Rows[i].FindControl("chkSelected") as CheckBox).Checked = isSelect;
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            else
            {
                if (!(gvr.FindControl("chkStopPayment") as CheckBox).Checked)
                {
                    if (ddlScrips.SelectedValue.Equals(Constants.LABEL_ALL))
                    {
                        if (ddlCouponInstalNo.SelectedValue.Equals(Constants.LABEL_ALL))
                        {
                            (gvr.FindControl("chkSelected") as CheckBox).Checked = isSelect;
                            //(gvr.FindControl("txtInterestAmount") as TextBox).Enabled = isSelect;
                        }
                        else
                        {
                            if (gvr.Cells[6].Text.Equals(ddlCouponInstalNo.SelectedValue))
                            {
                                (gvr.FindControl("chkSelected") as CheckBox).Checked = isSelect;
                                //(gvr.FindControl("txtInterestAmount") as TextBox).Enabled = isSelect;
                            }
                        }

                    }
                    else if (ddlCouponInstalNo.SelectedValue.Equals(Constants.LABEL_ALL))
                    {
                        if (ddlScrips.SelectedValue.Equals(Constants.LABEL_ALL))
                        {
                            (gvr.FindControl("chkSelected") as CheckBox).Checked = isSelect;
                            //(gvr.FindControl("txtInterestAmount") as TextBox).Enabled = isSelect;
                        }
                        else
                        {
                            if (gvr.Cells[4].Text.Equals(ddlScrips.SelectedValue))
                            {
                                (gvr.FindControl("chkSelected") as CheckBox).Checked = isSelect;
                                //(gvr.FindControl("txtInterestAmount") as TextBox).Enabled = isSelect;
                            }
                        }
                    }
                    else
                    {
                        if (gvr.Cells[4].Text.Equals(ddlScrips.SelectedValue) && gvr.Cells[6].Text.Equals(ddlCouponInstalNo.SelectedValue))
                        {
                            (gvr.FindControl("chkSelected") as CheckBox).Checked = isSelect;
                            //(gvr.FindControl("txtInterestAmount") as TextBox).Enabled = isSelect;
                        }
                    }
                }
            }
        }

        private bool ValidateScripAndCouponRangeSelection()
        {
            bool isRangeOk = true;

            if (!string.IsNullOrEmpty(txtFromScrip.Text) && !string.IsNullOrEmpty(txtToScrip.Text)
                && !string.IsNullOrEmpty(txtFromCoupon.Text) && !string.IsNullOrEmpty(txtToCoupon.Text))
            {
                isRangeOk = true;
            }
            else if (string.IsNullOrEmpty(txtFromScrip.Text) && string.IsNullOrEmpty(txtToScrip.Text)
                && string.IsNullOrEmpty(txtFromCoupon.Text) && string.IsNullOrEmpty(txtToCoupon.Text))
            {
                isRangeOk = false;
            }
            else if (string.IsNullOrEmpty(txtFromScrip.Text))
            {
                isRangeOk = false;
                ucMessage.OpenMessage("<b>From Scrip</b> cannot be empty!", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
            else if (string.IsNullOrEmpty(txtToScrip.Text))
            {
                isRangeOk = false;
                ucMessage.OpenMessage("<b>To Scrip</b> cannot be empty!", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
            else if (string.IsNullOrEmpty(txtFromCoupon.Text))
            {
                isRangeOk = false;
                ucMessage.OpenMessage("<b>From Coupon</b> cannot be empty!", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
            else if (string.IsNullOrEmpty(txtToCoupon.Text))
            {
                isRangeOk = false;
                ucMessage.OpenMessage("<b>To Coupon</b> cannot be empty!", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }

            return isRangeOk;
        }
        private void CalculatePayment(bool changeRateType)
        {
            decimal dSocialSecRate = 0;
            int iSPDuration = 0;
            int iMonthDuration = 0;
            InterestPayment oInterestPayment = Session[Constants.SES_INTEREST_PAYMENT] as InterestPayment;
            if (oInterestPayment != null)
            {
                if (chkMarkAsPremature.Checked)
                {

                }

                dSocialSecRate = oInterestPayment.Issue.VersionSPPolicy.SocialSecurityAmount;
                iSPDuration = oInterestPayment.Issue.VersionSPPolicy.SPDuration;
                iMonthDuration = 12 / (oInterestPayment.Issue.VersionSPPolicy.NoOfCoupons / iSPDuration);

                int iCouponCount = 0;
                int iSelectScripts = 0;
                string sLastScripts = "";
                decimal dCalInterest = 0;
                decimal dSSAmount = 0;
                decimal dTolenomination = 0;
                DateTime dtMaturityDateWDB = DateTime.Today.AddYears(-100);
                DateTime dtIssueDateWDB = oInterestPayment.Issue.VersionIssueDate;
                foreach (GridViewRow gvr in gvPaymentDetails.Rows)
                {
                    if ((gvr.FindControl("chkSelected") as CheckBox).Checked)
                    {
                        iCouponCount++;
                        if (!sLastScripts.Contains(Convert.ToString(gvr.Cells[4].Text)))
                        {
                            sLastScripts = sLastScripts + "|"+ Convert.ToString(gvr.Cells[4].Text);
                        }
                        dtMaturityDateWDB = Convert.ToDateTime(gvr.Cells[7].Text);
                        dCalInterest += Util.GetDecimalNumber((gvr.FindControl("txtInterestAmount") as TextBox).Text);
                        dTolenomination += Util.GetDecimalNumber(gvr.Cells[5].Text);//Denomination dataField
                        // **ZQ IT IS WORKING BUT FOR THE TIME IT HAS STOPED FOR ON DEMAND..                     
                        //dIncomeTax += Util.GetDecimalNumber((gvr.FindControl("lblIncomeTax") as Label).Text); //Util.GetDecimalNumber(gvr.Cells[9].Text);                   
                    }
                }
                string[] sLastScriptsList = sLastScripts.Split(Convert.ToChar("|"));
                iSelectScripts = sLastScriptsList.Length-1;

                InterestPaymentDAL oIntPayDAL = new InterestPaymentDAL();
                //Calculated Reduction Interest for Pre Mature
                DataTable dtPrematureReduction = (DataTable)oIntPayDAL.GetEarlyEncashmentCalculatedData(txtRegistrationNo.Text.Trim()).Return;
                if (chkMarkAsPremature.Checked)
                {
                    decimal dReductionInterest;
                    dReductionInterest=0;
                    for (int i = 0; i < dtPrematureReduction.Rows.Count; i++)
                    {
                        dReductionInterest += Convert.ToDecimal(dtPrematureReduction.Rows[i]["ReductionAmount"]);
                    }
                    dCalInterest = dCalInterest - dReductionInterest;
                }
                #region IncomeTax Calcultion
                // **Tmp ZQ
                double dbTaxPayable = 0;
                DateTime dtMaturityDate = DateTime.Today.AddYears(-100);
                if (oInterestPayment.Issue.VersionSPPolicy.IncomeTaxApplyAmount > 0)
                {
                    double dbIntPayable = 0;
                    DateTime dtUptoDate = DateTime.Today;
                    DateTime dtChackDate = DateTime.Today;
                    string stUptoDate = "30-Jun-";

                    foreach (GridViewRow gvr in gvPaymentDetails.Rows)
                    {
                        if ((gvr.FindControl("chkSelected") as CheckBox).Checked)
                        {
                            if (oInterestPayment.Issue.SPType.SPTypeID != "BSP")
                            {
                                if (dtMaturityDate != Convert.ToDateTime(gvr.Cells[7].Text))
                                {
                                    dtMaturityDate = Convert.ToDateTime(gvr.Cells[7].Text);
                                    dtUptoDate = Convert.ToDateTime(stUptoDate + dtMaturityDate.Year.ToString());
                                    if (dtMaturityDate > dtUptoDate)
                                    {
                                        dtUptoDate = dtUptoDate.AddYears(1);
                                    }
                                    if (dtChackDate != dtUptoDate)
                                    {
                                        dtChackDate = dtUptoDate;

                                        if (dbIntPayable > 0)
                                        {
                                            Result oResult = oIntPayDAL.GetCalculatedIncomeTax(oInterestPayment.Issue.IssueTransNo, dbIntPayable, dtUptoDate.AddYears(-1), changeRateType);
                                            if (oResult.Status)
                                            {
                                                dbTaxPayable += Convert.ToDouble(oResult.Return);
                                            }
                                        }
                                        dbIntPayable = 0;
                                    }
                                }
                                dbIntPayable += Convert.ToDouble((gvr.FindControl("txtInterestAmount") as TextBox).Text);
                            }
                            else
                            {
                                dbIntPayable += Convert.ToDouble((gvr.FindControl("txtInterestAmount") as TextBox).Text);
                            }
                        }
                    }

                    if (dbIntPayable > 0)
                    {
                        if (dtUptoDate.AddYears(-1) < dtMaturityDate)
                        {
                            dtUptoDate = dtMaturityDate;
                        }
                        Result oResult = oIntPayDAL.GetCalculatedIncomeTax(oInterestPayment.Issue.IssueTransNo, dbIntPayable, dtUptoDate, changeRateType);
                        if (oResult.Status)
                        {
                            dbTaxPayable += Convert.ToDouble(oResult.Return);
                        }
                    }
                }
                else
                {
                    Result oResult = oIntPayDAL.GetCalculatedIncomeTax(oInterestPayment.Issue.IssueTransNo, Convert.ToDouble(dCalInterest), DateTime.Today, changeRateType);
                    if (oResult.Status)
                    {
                        dbTaxPayable = Convert.ToDouble(oResult.Return);
                    }
                }
                #endregion

                #region Assign Data in calculation field set
                txtPDCouponInsSelected.Text = iCouponCount.ToString();

                decimal cCalInt = 0;
                decimal cSSP = 0;
                decimal cTax = 0;
                bool checkWDBSSP = false;
                if ((dtIssueDateWDB >= Convert.ToDateTime("01-Jul-2010") && dtIssueDateWDB <= Convert.ToDateTime("30-Jun-2011"))
                    && (oInterestPayment.Issue.SPType.SPTypeID.ToString() == "WDB") //&& dSocialSecRate > 0
                    && ((iSelectScripts * oInterestPayment.Issue.ScripList[0].NoOfCoupons) == iCouponCount)) //oInterestPayment.Issue.ScripList.Count * oInterestPayment.Issue.ScripList[0].NoOfCoupons
                {
                    cCalInt = Convert.ToDecimal(66809.60);
                    cSSP = Convert.ToDecimal(0);
                    cTax = Convert.ToDecimal(0);
                    checkWDBSSP = true;
                }
                else if ((dtIssueDateWDB >= Convert.ToDateTime("01-Jul-2011") && dtIssueDateWDB <= Convert.ToDateTime("30-Jun-2012"))
                    && (oInterestPayment.Issue.SPType.SPTypeID.ToString() == "WDB") //&& dSocialSecRate > 0
                    && ((iSelectScripts * oInterestPayment.Issue.ScripList[0].NoOfCoupons) == iCouponCount)) //oInterestPayment.Issue.ScripList.Count * oInterestPayment.Issue.ScripList[0].NoOfCoupons
                {
                    cCalInt = Convert.ToDecimal(72154.82);
                    cSSP = Convert.ToDecimal(5247.62);
                    cTax = Convert.ToDecimal(0);
                    checkWDBSSP = true;
                }
                else if ((dtIssueDateWDB >= Convert.ToDateTime("01-Jul-2012") && dtIssueDateWDB <= Convert.ToDateTime("22-May-2015"))
                                && (oInterestPayment.Issue.SPType.SPTypeID.ToString() == "WDB") //&& dSocialSecRate > 0
                                && ((iSelectScripts * oInterestPayment.Issue.ScripList[0].NoOfCoupons) == iCouponCount)) //oInterestPayment.Issue.ScripList.Count * oInterestPayment.Issue.ScripList[0].NoOfCoupons
                {
                    cCalInt = Convert.ToDecimal(73812.45);
                    cSSP = Convert.ToDecimal(5272.32);
                    cTax = Convert.ToDecimal(0);
                    checkWDBSSP = true;
                }

                else if ((dtIssueDateWDB >= Convert.ToDateTime("23-May-2015"))
                        && (oInterestPayment.Issue.SPType.SPTypeID.ToString() == "WDB") //&& dSocialSecRate > 0 
                        && ((iSelectScripts * oInterestPayment.Issue.ScripList[0].NoOfCoupons) == iCouponCount)) //oInterestPayment.Issue.ScripList.Count * oInterestPayment.Issue.ScripList[0].NoOfCoupons
                {
                    cCalInt = Convert.ToDecimal(79084.77);
                    cSSP = Convert.ToDecimal(0);
                    cTax = Convert.ToDecimal(0);
                    checkWDBSSP = true;
                }
                if (checkWDBSSP)
                {
                    //decimal cCalInt = Convert.ToDecimal(70814.45);
                    //decimal cSSP = Convert.ToDecimal(4072.77);
                    //decimal cTax = Convert.ToDecimal(0);

                    dCalInterest = cCalInt * (dTolenomination / 100000) / 10;
                    dSSAmount = cSSP * (dTolenomination / 100000) / 10;
                    dbTaxPayable = Convert.ToDouble(cTax * (dTolenomination / 100000) / 10);

                    decimal dtmpIInt = 0;
                    dtmpIInt = dCalInterest / iCouponCount;
                    decimal dtmpITax = 0;
                    dtmpITax = Convert.ToDecimal(dbTaxPayable) / iCouponCount;


                    foreach (GridViewRow gvr in gvPaymentDetails.Rows)
                    {
                        if ((gvr.FindControl("chkSelected") as CheckBox).Checked)
                        {

                            (gvr.FindControl("txtInterestAmount") as TextBox).Text = dtmpIInt.ToString("N2");
                            (gvr.FindControl("lblIncomeTax") as Label).Text = dtmpITax.ToString("N2");
                            gvr.Cells[10].Text = (Util.GetDecimalNumber((gvr.FindControl("txtInterestAmount") as TextBox).Text) - dtmpITax).ToString("N3");
                        }
                    }


                    ////dbTaxPayable = Math.Round(dbTaxPayable,2);
                    //dCalInterest = Math.Ceiling(dCalInterest);
                    //dbTaxPayable = Math.Truncate(dbTaxPayable);

                    //dSSAmount = Convert.ToDecimal(Convert.ToDouble(dTolenomination / oInterestPayment.Issue.ScripList[0].NoOfCoupons) * Math.Pow(Convert.ToDouble(1 + (dSocialSecRate / 100) / 2), Convert.ToDouble(5 * 2)));
                    //dSSAmount = Math.Truncate(Math.Round(dSSAmount, 2) * 100) / 100;
                    //dSSAmount = (dSSAmount - (dTolenomination/oInterestPayment.Issue.ScripList[0].NoOfCoupons)); //oInterestPayment.Issue.ScripList[0].NoOfCoupons;
                    //dSSAmount = Math.Truncate(dSSAmount);
                }
                else if (chkMarkAsPremature.Checked == false)
                {
                    dSSAmount = dTolenomination * ((dSocialSecRate / 100) / 12) * iMonthDuration;
                }
                txtPDSocialSecurityAmount.Text = dSSAmount.ToString("N2");
                txtPDCalcInterest.Text = (dCalInterest).ToString("N2");
                txtPDIntPayable.Text = (dCalInterest).ToString("N2");

                txtPDIncomeTax.Text = dbTaxPayable.ToString("N2");
                if ((DateTime.Today >= dtMaturityDateWDB) && (oInterestPayment.Issue.SPType.SPTypeID.ToString() == "WDB") && dSocialSecRate > 0 && ((oInterestPayment.Issue.ScripList.Count * oInterestPayment.Issue.ScripList[0].NoOfCoupons) == iCouponCount))
                { }
                else
                {
                    CalculateIncomTax();
                }

                txtPDPaymentAmount.Text = (dCalInterest - Convert.ToDecimal(dbTaxPayable) + dSSAmount).ToString("N2");
                txtPDConvertedAmount.Text = (Util.GetDecimalNumber(txtPDPaymentAmount.Text) * Util.GetDecimalNumber(txtPDConvRate.Text)).ToString("N2");
                #endregion
            }
        }

        private void SetObject(InterestPayment oInterestPayment)
        {
            if (oInterestPayment != null)
            {
                if (oInterestPayment.Issue != null)
                {
                    hdIssueTransNo.Value = oInterestPayment.Issue.IssueTransNo.ToString();
                    hdRegNo.Value = oInterestPayment.Issue.RegNo.ToString();
                    hdIntTransNo.Value = oInterestPayment.IntPaymentTransNo;
                    txtIntPayTransNo.Text = oInterestPayment.IntPaymentTransNo;
                    txtRegistrationNo.Text = oInterestPayment.Issue.RegNo.ToString();
                    ddlSpType.Text = oInterestPayment.Issue.SPType.SPTypeID.Trim();
                    ddlBranch.Text = oInterestPayment.Issue.Branch.BranchID.Trim();
                    txtTotalAmount.Text = oInterestPayment.Issue.IssueAmount.ToString("N2");
                    txtMasterID.Text = oInterestPayment.Issue.MasterNo;

                    #region Set Scrip DropDown
                    ddlScrips.Items.Clear();

                    for (int scripCount = 0; scripCount < oInterestPayment.Issue.ScripList.Count; scripCount++)
                    {
                        ListItem lItemScrip = new ListItem();

                        if (scripCount == 0)
                        {
                            lItemScrip.Text = "<-" + Constants.LABEL_ALL + "->";
                            lItemScrip.Value = Constants.LABEL_ALL;

                            ddlScrips.Items.Add(lItemScrip);

                            lItemScrip = new ListItem();
                        }

                        lItemScrip.Text = oInterestPayment.Issue.ScripList[scripCount].SPSeries + " " + oInterestPayment.Issue.ScripList[scripCount].SlNo;
                        lItemScrip.Value = oInterestPayment.Issue.ScripList[scripCount].SPSeries + " " + oInterestPayment.Issue.ScripList[scripCount].SlNo;

                        ddlScrips.Items.Add(lItemScrip);
                    }
                    #endregion

                    #region Set Coupon/Installment Drown
                    ddlCouponInstalNo.Items.Clear();

                    for (int couponCount = 0; couponCount < oInterestPayment.Issue.ScripList[0].NoOfCoupons; couponCount++)
                    {
                        ListItem lItemCoupon = new ListItem();

                        if (couponCount == 0)
                        {
                            lItemCoupon.Text = "<-" + Constants.LABEL_ALL + "->";
                            lItemCoupon.Value = Constants.LABEL_ALL;

                            ddlCouponInstalNo.Items.Add(lItemCoupon);

                            lItemCoupon = new ListItem();
                        }

                        lItemCoupon.Text = (couponCount + 1).ToString();
                        lItemCoupon.Value = (couponCount + 1).ToString();

                        ddlCouponInstalNo.Items.Add(lItemCoupon);
                    }
                    #endregion

                    txtIssueDate.Text = oInterestPayment.Issue.VersionIssueDate.ToString(Constants.DATETIME_FORMAT);
                    txtIssueName.Text = oInterestPayment.Issue.IssueName;

                    #region Customer Details
                    DataTable dtCustomerDetails = new DataTable();

                    dtCustomerDetails.Columns.Add(new DataColumn("MasterNo", typeof(string)));
                    dtCustomerDetails.Columns.Add(new DataColumn("Customer Name", typeof(string)));
                    dtCustomerDetails.Columns.Add(new DataColumn("Address", typeof(string)));
                    dtCustomerDetails.Columns.Add(new DataColumn("Phone", typeof(string)));

                    DataRow rowCustomerDetails = null;

                    for (int customerCount = 0; customerCount < oInterestPayment.Issue.CustomerDetailsList.Count; customerCount++)
                    {
                        rowCustomerDetails = dtCustomerDetails.NewRow();

                        rowCustomerDetails["MasterNo"] = oInterestPayment.Issue.CustomerDetailsList[customerCount].MasterNo;
                        rowCustomerDetails["Customer Name"] = oInterestPayment.Issue.CustomerDetailsList[customerCount].CustomerName;
                        rowCustomerDetails["Address"] = oInterestPayment.Issue.CustomerDetailsList[customerCount].Address;
                        rowCustomerDetails["Phone"] = oInterestPayment.Issue.CustomerDetailsList[customerCount].Phone;

                        dtCustomerDetails.Rows.Add(rowCustomerDetails);
                    }

                    gvCustomerDetail.DataSource = dtCustomerDetails;
                    gvCustomerDetail.DataBind();
                    #endregion

                    #region Payment Detail

                    string sRegNo = Request.QueryString[OBJ_REG_NO];
                    if (!string.IsNullOrEmpty(sRegNo))
                    {
                        sRegNo = oCrypManager.GetDecryptedString(sRegNo, Constants.CRYPT_PASSWORD_STRING);
                    }
                    if (!string.IsNullOrEmpty(sRegNo) || SEARCH_FROM.Equals(1))
                    {
                        DataView dv = new DataView(oInterestPayment.DtPayment);
                        if (dv.Count > 0)
                        {
                            if (SEARCH_FROM.Equals(1) && hdDataType.Value.Equals("2"))
                            {
                                dv.RowFilter = "Paid = 'true'";
                            }
                            else
                            {
                                dv.RowFilter = "Selected = 'true'";
                            }
                            if (dv.Count > 0)
                            {
                                gvPaymentDetails.DataSource = dv;
                                gvPaymentDetails.DataBind();
                            }
                        }
                    }
                    else
                    {
                        if (oInterestPayment.DtPayment.Rows.Count > 1000)
                        {
                            ucMessage.OpenMessage(txtRegistrationNo.Text + " has payable scripts of <b>" + oInterestPayment.DtPayment.Rows.Count.ToString() + "</b>. Use Script range selection option to complete this payment.", Constants.MSG_TYPE_INFO);
                            ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        }
                        else
                        {
                            gvPaymentDetails.DataSource = oInterestPayment.DtPayment;
                            gvPaymentDetails.DataBind();
                        }
                    }

                    DDListUtil.Assign(ddlPDPaymentMode, oInterestPayment.Issue.VersionSPPolicy.DTPaymentPolicy, true);
                    DDListUtil.Assign(ddlPDCurrency, oInterestPayment.Issue.VersionSPPolicy.DTCurrencyActivityPolicy, true);

                    if (oInterestPayment.InterestRate != 0)
                    {
                        txtPDInterestRate.Text = oInterestPayment.InterestRate.ToString("N2");
                        if (Util.GetDecimalNumber(txtPDInterestRate.Text) != Util.GetDecimalNumber(oInterestPayment.Issue.VersionSPPolicy.DTGeneralInterestPolicy.Rows[0]["ClaimRate"].ToString()))
                        {
                            chkMarkAsPremature.Checked = true;
                        }
                        else
                        {
                            chkMarkAsPremature.Checked = false;
                        }
                    }
                    else
                    {
                        txtPDInterestRate.Text = oInterestPayment.Issue.VersionSPPolicy.DTGeneralInterestPolicy.Rows[0]["ClaimRate"].ToString();
                    }

                    if (string.IsNullOrEmpty(txtIntPayTransNo.Text))
                    {
                        txtPaymentDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
                        txtPDCouponInsSelected.Text = "0"; Convert.ToString(gvPaymentDetails.Rows.Count);
                        DDListUtil.Assign(ddlPDPaymentMode, "6"); // this is for Customer Account                                                               
                        //Assign default currency
                        if (oInterestPayment.Issue.SPType.SPTypeID.Equals("WDB"))
                        {
                            DDListUtil.Assign(ddlPDCurrency, "00"); //Default currency BDT for WDB
                        }
                        else
                        {
                            DDListUtil.Assign(ddlPDCurrency, oInterestPayment.Issue.SPType.Currency.CurrencyID);
                        }
                        txtPDConvRate.Enabled = false;
                    }
                    else
                    {
                        DDListUtil.Assign(ddlPDPaymentMode, oInterestPayment.Payment.PaymentMode);
                        DDListUtil.Assign(ddlPDCurrency, oInterestPayment.CurrencyID);
                        txtPaymentDate.Text = oInterestPayment.PaymentDate.ToString(Constants.DATETIME_FORMAT);
                        if (SEARCH_FROM.Equals(4)) // if from IntPayment Tmp table
                        {
                            DataView dv = new DataView(oInterestPayment.DtPayment);
                            if (dv.Count > 0)
                            {
                                dv.RowFilter = "Selected = 'true'";
                                txtPDCouponInsSelected.Text = dv.Count.ToString();
                            }
                        }
                        else
                        {
                            txtPDCouponInsSelected.Text = Convert.ToString(gvPaymentDetails.Rows.Count);
                        }

                        if (oInterestPayment.Issue.SPType.SPTypeID.Equals("WDB") && oInterestPayment.CurrencyID.Equals("00"))
                        {
                            txtPDConvRate.Enabled = false;
                        }
                        else if (oInterestPayment.Issue.SPType.Currency.CurrencyID != oInterestPayment.CurrencyID)
                        {
                            txtPDConvRate.Enabled = true;
                        }
                        else
                        {
                            txtPDConvRate.Enabled = false;
                        }

                        txtPDConvRate.Text = Convert.ToString(oInterestPayment.ConvRate);

                        if (oInterestPayment.Issue.SPType.Currency.CurrencyID.Equals(oInterestPayment.CurrencyID))
                        {
                            txtPDConvertedAmount.Text = oInterestPayment.PaymentAmount.ToString("N2");
                        }
                        else
                        {
                            if (oInterestPayment.CurrencyID.Equals("00"))
                            {
                                txtPDConvertedAmount.Text = ((oInterestPayment.PaymentAmount) * (Util.GetDecimalNumber(txtPDConvRate.Text))).ToString("N2");
                            }
                            else
                            {
                                txtPDConvertedAmount.Text = ((oInterestPayment.PaymentAmount) / (Util.GetDecimalNumber(txtPDConvRate.Text))).ToString("N2");
                            }
                        }
                    }

                    txtPDCalcInterest.Text = oInterestPayment.CalculatedInterest.ToString("N2");
                    txtPDSocialSecurityAmount.Text = oInterestPayment.SocialSecurityAmount.ToString("N2");
                    txtPDIncomeTax.Text = oInterestPayment.IncomeTax.ToString("N2");
                    txtPDIntPayable.Text = oInterestPayment.PaidInterest.ToString("N2");
                    txtPDPaymentAmount.Text = oInterestPayment.PaymentAmount.ToString("N2");
                    if (!string.IsNullOrEmpty(oInterestPayment.AccountNo) && oInterestPayment.AccountNo.Length >= 12)
                    {
                        txtPDAccountNo.Text = oInterestPayment.AccountNo.Substring(0, 12);
                        txtPDAccountName.Text = oInterestPayment.Payment.AccountName;
                    }
                    txtPDDraftNo.Text = oInterestPayment.RefNo;

                    #endregion

                    #region Nominee Detail
                    DataTable dtNomineeDetail = new DataTable();

                    dtNomineeDetail.Columns.Add(new DataColumn("Nominee Name", typeof(string)));
                    dtNomineeDetail.Columns.Add(new DataColumn("Relation", typeof(string)));
                    dtNomineeDetail.Columns.Add(new DataColumn("Address", typeof(string)));
                    dtNomineeDetail.Columns.Add(new DataColumn("Nominee Share", typeof(string)));
                    dtNomineeDetail.Columns.Add(new DataColumn("Amount", typeof(string)));

                    DataRow rowNomineeDetail = null;

                    for (int nomineeCount = 0; nomineeCount < oInterestPayment.Issue.NomineeList.Count; nomineeCount++)
                    {
                        rowNomineeDetail = dtNomineeDetail.NewRow();

                        rowNomineeDetail["Nominee Name"] = oInterestPayment.Issue.NomineeList[nomineeCount].NomineeName;
                        rowNomineeDetail["Relation"] = oInterestPayment.Issue.NomineeList[nomineeCount].Relation;
                        rowNomineeDetail["Address"] = oInterestPayment.Issue.NomineeList[nomineeCount].Address;
                        rowNomineeDetail["Nominee Share"] = oInterestPayment.Issue.NomineeList[nomineeCount].NomineeShare;
                        rowNomineeDetail["Amount"] = oInterestPayment.Issue.NomineeList[nomineeCount].Amount;

                        dtNomineeDetail.Rows.Add(rowNomineeDetail);
                    }
                    gvNomDetail.DataSource = dtNomineeDetail;
                    gvNomDetail.DataBind();

                    #endregion

                    Session[Constants.SES_INTEREST_PAYMENT] = oInterestPayment;

                    //Added by Ayesha on 14/06/2012
                    //Modified by Istiak on 14/06/2012


                    //if Loaded from Approval page or if Approved Data Loaded 
                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    if ((SEARCH_FROM.Equals(1) && hdDataType.Value.Equals("2")))
                    {
                        oUserDetails.MakerID = oInterestPayment.UserDetails.MakerID;
                        oUserDetails.MakeDate = oInterestPayment.UserDetails.MakeDate;
                        oUserDetails.CheckDate = oInterestPayment.UserDetails.CheckDate;
                        oUserDetails.CheckerID = oInterestPayment.UserDetails.CheckerID;
                        oUserDetails.CheckerComment = oInterestPayment.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;

                    }
                    if ((hdDataType.Value.Equals("1") && SEARCH_FROM.Equals(1)) || SEARCH_FROM.Equals(4))
                    {
                        oUserDetails.CheckerID = oInterestPayment.UserDetails.CheckerID;
                        oUserDetails.CheckDate = oInterestPayment.UserDetails.CheckDate;
                        oUserDetails.CheckerComment = oInterestPayment.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;
                    }
                    if (SEARCH_FROM.Equals(3) || SEARCH_FROM.Equals(2))
                    {
                        oUserDetails.MakerID = oInterestPayment.UserDetails.MakerID;
                        oUserDetails.CheckerComment = oInterestPayment.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;

                        //PopUp message on Stopped scrips for Checker

                        //DataView dv = new DataView(oInterestPayment.DtPayment);
                        //if (dv.Count > 0)
                        //{
                        //    dv.RowFilter = "StopPayment = 'true'";

                        //    if (dv.Count > 0)
                        //    {
                        //        ucMessage.OpenMessage("Some scrips are stopped!!", Constants.MSG_TYPE_INFO);
                        //        ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        //    }
                        //}
                    }

                    //CalculatePayment();
                }
            }
        }

        private InterestPayment GetObject()
        {
            InterestPayment oInterestPayment = new InterestPayment();

            #region InterestPaymentDetail
            foreach (GridViewRow gvr in gvPaymentDetails.Rows)
            {
                if ((gvr.FindControl("chkSelected") as CheckBox).Checked)
                {
                    InterestPaymentDetail oInterestPaymentDetail = new InterestPaymentDetail();

                    oInterestPaymentDetail.IntPaymentTransNo = hdIntTransNo.Value == "" ? "-1" : hdIntTransNo.Value;//if any Transaction loaded from Temp table
                    oInterestPaymentDetail.CalculatedInterest = Util.GetDecimalNumber((gvr.FindControl("txtInterestAmount") as TextBox).Text);
                    oInterestPaymentDetail.CouponNo = Util.GetIntNumber(gvr.Cells[6].Text);
                    oInterestPaymentDetail.IncomeTax = Util.GetDecimalNumber((gvr.FindControl("lblIncomeTax") as Label).Text);
                    oInterestPaymentDetail.MaturityDate = Util.GetDateTimeByString(gvr.Cells[7].Text.Trim());
                    oInterestPaymentDetail.PaidInterest = Util.GetDecimalNumber(gvr.Cells[10].Text);
                    oInterestPaymentDetail.Scrip.SPScripID = Util.GetIntNumber((gvr.FindControl("hdnSPScripID") as HiddenField).Value);

                    //Add in List
                    oInterestPayment.InterestPaymentDetailList.Add(oInterestPaymentDetail);
                }
            }
            #endregion

            #region InterestPayment
            oInterestPayment.IntPaymentTransNo = hdIntTransNo.Value == "" ? "-1" : hdIntTransNo.Value; //if any Transaction loaded from Temp table
            oInterestPayment.Issue.IssueTransNo = hdIssueTransNo.Value;
            if (!string.IsNullOrEmpty(txtPDAccountNo.Text))
            {
                oInterestPayment.AccountNo = txtPDAccountNo.Text; //+ ddlPDCurrency.SelectedValue;
            }
            oInterestPayment.RefNo = txtPDDraftNo.Text;
            oInterestPayment.CalculatedInterest = Util.GetDecimalNumber(txtPDCalcInterest.Text);
            oInterestPayment.ConvRate = Util.GetDecimalNumber(txtPDConvRate.Text);
            oInterestPayment.CurrencyID = ddlPDCurrency.SelectedValue.Trim();
            oInterestPayment.IncomeTax = Util.GetDecimalNumber(txtPDIncomeTax.Text);
            oInterestPayment.InterestRate = Util.GetDecimalNumber(txtPDInterestRate.Text);
            oInterestPayment.SocialSecurityAmount = Util.GetDecimalNumber(txtPDSocialSecurityAmount.Text);
            oInterestPayment.PaidInterest = Util.GetDecimalNumber(txtPDIntPayable.Text);
            oInterestPayment.Payment.PaymentMode = Convert.ToInt32(ddlPDPaymentMode.SelectedValue);
            oInterestPayment.PaymentAmount = Util.GetDecimalNumber(txtPDPaymentAmount.Text);
            oInterestPayment.PaymentDate = Util.GetDateTimeByString(txtPaymentDate.Text);
            oInterestPayment.UserDetails = ucUserDet.UserDetail;
            #endregion

            return oInterestPayment;
        }

        private void LoadPreviousList()
        {
            InterestPaymentDAL oIntPayDAL = new InterestPaymentDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oIntPayDAL.LoadUnapprovedList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);

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

        private void TotalClear()
        {
            // set null in session                         
            Session[Constants.SES_INTEREST_PAYMENT] = null;

            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            gvPaymentDetails.DataSource = null;
            gvPaymentDetails.DataBind();

            //Interest Payment Details
            txtIntPayTransNo.Text = string.Empty;
            txtPaymentDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            hdIntTransNo.Value = string.Empty;
            hdIssueTransNo.Value = string.Empty;
            //Payment Details
            txtPDCouponInsSelected.Text = "0";
            txtPDCalcInterest.Text = "0.00";
            txtPDIntPayable.Text = "0.00";
            txtPDSocialSecurityAmount.Text = "0.00";
            txtPDIncomeTax.Text = "0.00";
            txtPDPaymentAmount.Text = "0.00";
            txtPDConvRate.Text = "1.00";
            txtPDConvertedAmount.Text = "0.00";
            txtPDInterestRate.Text = "0.00";
            txtPDAccountNo.Text = string.Empty;
            txtPDAccountName.Text = string.Empty;
            txtPDDraftNo.Text = string.Empty;
            // Dropdown load SPType
            ddlSpType.SelectedIndex = 0;
            ddlBranch.SelectedIndex = 0;


            //Issue Details
            txtRegistrationNo.Text = string.Empty;
            txtIssueName.Text = string.Empty;
            txtTotalAmount.Text = string.Empty;
            txtMasterID.Text = string.Empty;
            txtIssueDate.Text = string.Empty;

            //Coupon \ Installments Details
            ddlScrips.Items.Clear();
            ddlCouponInstalNo.Items.Clear();

            //Payment Details
            ddlPDPaymentMode.Items.Clear();
            ddlPDCurrency.Items.Clear();

            chkIndiYesNo.Checked = false;
            chkMarkAsPremature.Checked = false;
            ucUserDet.ResetData();
        }

        private void EnableDisableControl(bool isEnabled)
        {
            if (isEnabled)
            {
                //Payment Details
                Util.ControlEnabled(txtPDCouponInsSelected, false);
                Util.ControlEnabled(txtPDCalcInterest, false);
                Util.ControlEnabled(txtPDIntPayable, false);
                Util.ControlEnabled(txtPDSocialSecurityAmount, false);
                Util.ControlEnabled(txtPDIncomeTax, false);
                Util.ControlEnabled(txtPDPaymentAmount, false);
                Util.ControlEnabled(txtPDConvRate, false);
                Util.ControlEnabled(txtPDAccountNo, false);
                Util.ControlEnabled(txtPDDraftNo, false);
                Util.ControlEnabled(txtPDConvertedAmount, false);
                Util.ControlEnabled(txtPDAccountName, false);
                Util.ControlEnabled(txtPDInterestRate, false);

                //Payment Details
                Util.ControlEnabled(ddlPDPaymentMode, false);
                Util.ControlEnabled(ddlPDCurrency, false);

                //Interest Payment Details                
                Util.ControlEnabled(txtPaymentDate, false);
                Util.ControlEnabled(chkMarkAsPremature, false);
                Util.ControlEnabled(chkIndiYesNo, false);

                //Issue Details
                Util.ControlEnabled(ddlSpType, false);
                Util.ControlEnabled(ddlBranch, false);
                Util.ControlEnabled(txtRegistrationNo, false);

                //Coupon \ Installments Details
                Util.ControlEnabled(ddlScrips, false);
                Util.ControlEnabled(ddlCouponInstalNo, false);
                Util.ControlEnabled(gvPaymentDetails, false);

                // button
                Util.ControlEnabled(btnIntPayTransSearch, false);
                Util.ControlEnabled(btnRegSearch, false);

                Util.ControlEnabled(btnCalculate, false);
                Util.ControlEnabled(btnSelect, false);
                Util.ControlEnabled(btnDeSelect, false);

                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true);

                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);

                fsList.Visible = false;
            }
            else
            {
                //Payment Details
                Util.ControlEnabled(txtPDCouponInsSelected, false);
                Util.ControlEnabled(txtPDCalcInterest, false);
                Util.ControlEnabled(txtPDIntPayable, false);
                Util.ControlEnabled(txtPDSocialSecurityAmount, false);
                Util.ControlEnabled(txtPDIncomeTax, true);
                Util.ControlEnabled(txtPDPaymentAmount, false);
                //Util.ControlEnabled(txtPDConvRate, true);
                Util.ControlEnabled(txtPDAccountNo, false);
                Util.ControlEnabled(txtPDDraftNo, false);
                Util.ControlEnabled(txtPDConvertedAmount, false);
                Util.ControlEnabled(txtPDAccountName, false);
                Util.ControlEnabled(txtPDInterestRate, false);

                // button 
                Util.ControlEnabled(btnIntPayTransSearch, true);
                Util.ControlEnabled(btnRegSearch, true);

                Util.ControlEnabled(btnCalculate, true);
                Util.ControlEnabled(btnSelect, true);
                Util.ControlEnabled(btnDeSelect, true);

                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);

                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);

                //Interest Payment Details                
                Util.ControlEnabled(txtPaymentDate, true);
                Util.ControlEnabled(chkMarkAsPremature, true);
                Util.ControlEnabled(chkIndiYesNo, true);

                //Payment Details
                Util.ControlEnabled(ddlPDPaymentMode, true);
                Util.ControlEnabled(ddlPDCurrency, true);

                //Issue Details
                Util.ControlEnabled(ddlSpType, false);
                Util.ControlEnabled(ddlBranch, false);
                Util.ControlEnabled(txtRegistrationNo, true);
                //Coupon \ Installments Details
                Util.ControlEnabled(ddlScrips, true);
                Util.ControlEnabled(ddlCouponInstalNo, true);
                Util.ControlEnabled(gvPaymentDetails, true);

                fsList.Visible = true;
            }
        }

        #endregion Util Method...

        protected void btnViewJournals_Click(object sender, EventArgs e)
        {
            InterestPayment oInterestPayment = Session[Constants.SES_INTEREST_PAYMENT] as InterestPayment;
            SBM_BLC1.DAL.Common.ReportDAL rdal = new SBM_BLC1.DAL.Common.ReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                if (oInterestPayment.UserDetails.CheckerID.Trim() != "")
                {
                    oResult = rdal.InerestAdviceReport(oInterestPayment.IntPaymentTransNo, true);
                }
                else
                {
                    oResult = rdal.InerestAdviceReport(oInterestPayment.IntPaymentTransNo,false);
                }

                if (oResult.Status)
                {
                    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                }
            }
        }

        protected void txtPDIncomeTax_TextChanged(object sender, EventArgs e)
        {
            CalculateIncomTax();
        }
        private void CalculateIncomTax()
        {
            if (Util.GetDecimalNumber(txtPDCouponInsSelected.Text) > 0)
            {
                decimal dIncomeTax = 0;
                int iCouponCount = 0;

                dIncomeTax = Util.GetDecimalNumber(txtPDIncomeTax.Text);
                iCouponCount = Util.GetIntNumber(txtPDCouponInsSelected.Text);

                decimal dtmpITax = 0;
                foreach (GridViewRow gvr in gvPaymentDetails.Rows)
                {
                    if ((gvr.FindControl("chkSelected") as CheckBox).Checked)
                    {
                        dtmpITax = dIncomeTax / iCouponCount;
                        //dIncomeTax += Util.GetDecimalNumber((gvr.FindControl("lblIncomeTax") as Label).Text); //Util.GetDecimalNumber(gvr.Cells[9].Text);                   
                        (gvr.FindControl("lblIncomeTax") as Label).Text = dtmpITax.ToString("N2");
                        gvr.Cells[10].Text = (Util.GetDecimalNumber((gvr.FindControl("txtInterestAmount") as TextBox).Text) - dtmpITax).ToString("N3");
                    }
                }
                txtPDPaymentAmount.Text = (Util.GetDecimalNumber(txtPDCalcInterest.Text) - Convert.ToDecimal(dIncomeTax) + Util.GetDecimalNumber(txtPDSocialSecurityAmount.Text)).ToString("N2");
                txtPDConvertedAmount.Text = (Util.GetDecimalNumber(txtPDPaymentAmount.Text) * Util.GetDecimalNumber(txtPDConvRate.Text)).ToString("N2");

            }
            else
            {
                txtPDIncomeTax.Text = "0.00";
            }

        }

        protected void txtRegistrationNo_TextChanged(object sender, EventArgs e)
        {
            btnRegSearch_Click(sender, e);
        }

        protected void btnTaxAdjustment_Click(object sender, EventArgs e)
        {
            CalculatePayment(true);
        }



        //protected void gvPaymentDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    gvPaymentDetails.PageIndex = e.NewPageIndex;
        //    if (Session[Constants.SES_INTEREST_PAYMENT]!= null)
        //    {
        //        DataTable dtTmpList = ((InterestPayment)Session[Constants.SES_INTEREST_PAYMENT]).DtPayment ;
        //        gvData.DataSource = dtTmpList;
        //        gvData.DataBind();
        //    }
        //}
    }
}

