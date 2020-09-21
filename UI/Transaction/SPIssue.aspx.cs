using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Configuration;
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Entity.Transaction;
using SBM_BLC1.Transaction;
using System.Text;
using System.Collections;
using System.Globalization;
using SBM_BLC1.DAL.Report;

namespace SBM_WebUI.mp
{


    public partial class SPIssue : System.Web.UI.Page
    {

        #region Local Variable
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_TRAN_NO = "sTransID";
        public const string OBJ_PAGE_ID = "sPageID";
        public const string TEXT_CHEQUE = "Cheque";
        public const string TEXT_DRAFT = "Draft";
        public int SEARCH_FROM = 0;
        public string _STOCK_INFO = "stockInfo";
        #endregion Local Variable

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Constants.SES_USER_CONFIG] != null)
            {
                if (!Page.IsPostBack)
                {
                    Util.InvalidateSession();
                    InitializeData();
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
            SIssue.Type = Convert.ToString((int)Constants.SEARCH_ISSUE.ISSUE);
            gvData.PageSize = (int)Constants.PAGING_UNAPPROVED;
            TotalClear();
            // Issue set in session 
            if (Session[Constants.SES_CURRENT_ISSUE] == null)
            {
                Issue oSesIssue = new Issue();
                Session.Add(Constants.SES_CURRENT_ISSUE, oSesIssue);
            }
            else
            {
                Issue oSesIssue = new Issue();
                Session[Constants.SES_CURRENT_ISSUE]= oSesIssue;
            }
            gvDenomDetail.DataSource = null;
            gvDenomDetail.DataBind();

            gvCustmerDetail.DataSource = null;
            gvCustmerDetail.DataBind();

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            //gvNomDemon.DataSource = null;
            //gvNomDemon.DataBind();

            gvData.DataSource = null;
            gvData.DataBind();

            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            DDListUtil.LoadDDLFromDB(ddlBranch, "BranchID", "BranchName", "SPMS_Branch", true);
            DDListUtil.LoadDDLFromDB(ddlCollectionBranch, "BranchID", "BranchName", "SPMS_Branch", true);
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            ddlBranch.Text = oConfig.BranchID;
           
            string sTranID = Request.QueryString[OBJ_TRAN_NO];
            string sPageID = Request.QueryString[OBJ_PAGE_ID];

            if (!string.IsNullOrEmpty(sTranID))
            {
                sTranID = oCrypManager.GetDecryptedString(sTranID, Constants.CRYPT_PASSWORD_STRING);
            }
            if (!string.IsNullOrEmpty(sPageID))
            {
                sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
            }
            

            if (!string.IsNullOrEmpty(sTranID) && !string.IsNullOrEmpty(sPageID))
            {
                string sOperationType = sPageID.Substring(4, 1);
                if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
                {
                    SEARCH_FROM = 1;                    
                    LoadDataByTranID(sTranID, "1");//query from Temp Table

                    #region User-Detail.
                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    oUserDetails.CheckerID = oConfig.UserName;
                    oUserDetails.CheckDate = DateTime.Now;
                    oUserDetails.MakeDate = DateTime.Now;
                   
                    ucUserDet.UserDetail = oUserDetails;
                    #endregion User-Detail.

                    fsList.Visible = false;
                }
            }
            else
            {                               
                UserDetails oUserDetails = new UserDetails();
                oUserDetails.MakerID = oConfig.UserName;
                oUserDetails.MakeDate = DateTime.Now;
               
                ucUserDet.UserDetail = oUserDetails;

                fsList.Visible = true;
                LoadPreviousList();
                ddlSpType.Focus();
            }            
        }
        #endregion InitializeData

        private void TotalClear()
        {
            EnableDisableControl(false);
            gvDenomDetail.DataSource = null;
            gvDenomDetail.DataBind();

            gvCustmerDetail.DataSource = null;
            gvCustmerDetail.DataBind();

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            //gvNomDemon.DataSource = null;
            //gvNomDemon.DataBind();

            //gvStock.DataSource = null;
            //gvStock.DataBind();
            ViewState[_STOCK_INFO] = null;

            //Issue Details
            ddlSpType.SelectedIndex = 0;
            txtIssueDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtRegistrationNo.Text = "";
            ddlCustomerType.Items.Clear();
            txtAppliedAmount.Text = "";

            // Issue Name 
            txtIssueName.Text = "";
            txtMasterNo.Text = "";
            lblMasterVarified.Text = "Not verified yet!";
            //chkFiscalYear.Checked = false;

            // Bond Holder Details
            txtBHDHolderName.Text = "";
            txtBHDAddress.Text = "";
            txtBHDRelation.Text = "";

            //Denomination(s) details
            ddlDDDenom.Items.Clear();
            txtDDQuantity.Text = string.Empty;
            txtTotalAmount.Text = string.Empty;

            //Payment Details
            ddlPDPaymentMode.Items.Clear();
            ddlPDCurrencyCode.Items.Clear();
            txtPDAccDraftNo.Text = string.Empty;
            txtPDAccName.Text = string.Empty;
            txtPDConvRate.Text = "1.00";
            txtPDPaymentAmount.Text = string.Empty;

            //Nominee Detail
            //Nominee(s) Details
            txtNDName.Text = string.Empty;
            txtNDRelation.Text = string.Empty;
            txtNDAddress.Text = string.Empty;
            txtNDShare.Text = string.Empty;
            txtNDAmount.Text = string.Empty;
            txtNDateofBirth.Text = string.Empty;

            ddlGender.SelectedValue = "M";
            ddlResidenceStatus.SelectedValue = "R";
            txtNationalID.Text = string.Empty;
            txtNationalID_IssueAt.Text = string.Empty;
            txtPassportNo.Text = string.Empty;
            txtPassportNo_IssueAt.Text = string.Empty;
            txtBirthCertificateNo.Text = string.Empty;
            txtBirthCertificateNo_IssueAt.Text = string.Empty;
            txtTIN.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtEmailAddress.Text = string.Empty;



            DDListUtil.Assign(ddlResidentCountry, "BANGLADESH");
            DDListUtil.Assign(ddlPassportNoCountry, "BANGLADESH");
            DDListUtil.Assign(ddlNationalIDCountry, "BANGLADESH");
            DDListUtil.Assign(ddlBirthCertificateNoCountry, "BANGLADESH");

            //Clear Hidden values
            hdDenom.Value = string.Empty;
            hdNomSlno.Value = string.Empty;
            hdReg.Value = string.Empty;
            hdTransNo.Value = string.Empty;

            //User Detail
            ucUserDet.ResetData();
            Session[Constants.SES_CURRENT_ISSUE] = null;
        }

        protected void btnRegSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRegistrationNo.Text))
            {
                LoadDataByRegNo(txtRegistrationNo.Text);
            }
        }



        protected void btnReset_Click(object sender, EventArgs e)
        {
            EnableDisableControl(false);
            TotalClear();            
        }

        public void PopupIssueSearchLoadAction(string sTransNo, string sRegNo, string sApprovalStaus)
        {
            LoadDataByTranID(sTransNo, sApprovalStaus);
        }

        private void LoadPreviousList()
        {
            IssueDAL oIssueDAL = new IssueDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oIssueDAL.LoadUnapprovedList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);
            gvData.DataSource = null;
            gvData.DataBind();
            if (oResult.Status)
            {
                DataTable dtTmpList = (DataTable)oResult.Return;
                if (dtTmpList != null)
                {
                    if (dtTmpList.Rows.Count > 0)
                    {
                        dtTmpList.Columns.Remove("Maker ID");

                        gvData.DataSource = dtTmpList;
                        gvData.DataBind();                        
                    }
                }

                Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;
            }
        }        

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
            oIssue.IssueTransNo = hdTransNo.Value;
            IssueDAL oIssueDAL = new IssueDAL();
            oIssue.UserDetails = ucUserDet.UserDetail;
            Result oResult = (Result)oIssueDAL.Approve(oIssue);
            if (oResult.Status)
            {
                if (oResult.Message.Equals("E"))
                {
                    ucMessage.OpenMessage("Required Script is not available now", Constants.MSG_TYPE_ERROR);
                }
                else
                {                                        
                    //StockInfoLoad(oIssue.SPType.SPTypeID);             
                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE + " with Reg. No.: <b>" + (string)oResult.Return + "</b>", Constants.MSG_TYPE_SUCCESS);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_APPROVE, Constants.MSG_TYPE_ERROR);
            }
        }
        
        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");
            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE).PadLeft(5, '0'), false);
            }
            else
            {
                ddlSpType.Focus();
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE).PadLeft(5, '0'), false);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (hdReg.Value == "")
            {
                if (!string.IsNullOrEmpty(hdTransNo.Value))
                {
                    IssueDAL oIssueDAL = new IssueDAL();
                    Result oResult = (Result)oIssueDAL.Detete(hdTransNo.Value);
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

        protected void gvDenomDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {            
            Util.GridDateFormat(e, gvDenomDetail, null, null);
        }

        protected void btnLimitStatus_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlCustomerType.SelectedValue) && !string.IsNullOrEmpty(ddlSpType.SelectedValue) && !string.IsNullOrEmpty(txtMasterNo.Text) )
            {
                CustLimitInfo.SetData(ddlSpType.SelectedValue , ddlCustomerType.SelectedValue, txtMasterNo.Text);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, "CustLimitInfoPopupReturnTrue()", true);
            }
            else
            {
                string sMsg = "";
                if (string.IsNullOrEmpty(ddlCustomerType.SelectedValue))
                {
                    sMsg += "SP Type";
                }
                if (string.IsNullOrEmpty(ddlSpType.SelectedValue))
                {
                    if (sMsg.Length > 0)
                    {
                        sMsg += ",";
                    }
                    sMsg += " Customer Type";
                }
                if (string.IsNullOrEmpty(txtMasterNo.Text))
                {
                    if (sMsg.Length > 0)
                    {
                        sMsg += ",";
                    }
                    sMsg += " Master ID";
                }

                sMsg += " is required";
                ucMessage.OpenMessage(sMsg , Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdTransNo.Value))
            {
                Issue oIssue = new Issue(hdTransNo.Value);
                IssueDAL oIssueDAL = new IssueDAL();
                oIssue.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oIssueDAL.Reject(oIssue);
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

        protected void btnShowPolicy_Click(object sender, EventArgs e)
        {
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

        protected void gvCustmerDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {                                                
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
            HiddenField ohdTmpCustomerID = (HiddenField)gvRow.Cells[0].FindControl("hdTmpCustomerID");

            if (((Button)e.CommandSource).Text.Equals("Select"))
            {
                if (oIssue != null)
                {
                    CustomerDetails oCustDetlExist = oIssue.CustomerDetailsList[Convert.ToInt32(ohdTmpCustomerID.Value)];
                    if (oCustDetlExist != null)
                    {
                        if (!txtRegistrationNo.Enabled)
                        {
                            oCustDetlExist.isViewOnly = true;     
                        }
                        CustomerDetail.SetCustomerDetails(oCustDetlExist);
                    }
                }
            }
            else if (((Button)e.CommandSource).Text.Equals("Delete"))
            {
                gvCustmerDetail.DataSource = null;
                gvCustmerDetail.DataBind();

                if (oIssue != null)
                {
                    oIssue.CustomerDetailsList.RemoveAt(Convert.ToInt32(ohdTmpCustomerID.Value));
                }

                DataTable dtCustomerDetails = new DataTable();

                dtCustomerDetails.Columns.Add(new DataColumn("CustomerID", typeof(string)));
                dtCustomerDetails.Columns.Add(new DataColumn("CustomerName", typeof(string)));
                dtCustomerDetails.Columns.Add(new DataColumn("DateOfBirth", typeof(string)));

                string issueName = string.Empty;
                DataRow rowCustomerDetails = null;
                for (int i = 0; i < oIssue.CustomerDetailsList.Count; i++)
                {
                    rowCustomerDetails = dtCustomerDetails.NewRow();

                    rowCustomerDetails["CustomerID"] = oIssue.CustomerDetailsList[i].CustomerID == -1 ? "New Customer" : oIssue.CustomerDetailsList[i].CustomerID.ToString();// oIssue.CustomerDetailsList[i].CustomerID;
                    rowCustomerDetails["CustomerName"] = oIssue.CustomerDetailsList[i].CustomerName;
                    rowCustomerDetails["DateOfBirth"] = oIssue.CustomerDetailsList[i].DateOfBirth.ToString(Constants.DATETIME_dd_MMM_yyyy);

                    //Issue Name is generally the customer name
                    if (i > 0)
                    {
                        issueName += " & ";
                    }

                    issueName += oIssue.CustomerDetailsList[i].CustomerName;

                    //if (!string.IsNullOrEmpty(oIssue.CustomerDetailsList[i].CustomerName2))
                    //{
                    //    issueName += " & " + oIssue.CustomerDetailsList[i].CustomerName2;
                    //}

                    dtCustomerDetails.Rows.Add(rowCustomerDetails);
                }
                gvCustmerDetail.DataSource = dtCustomerDetails;
                gvCustmerDetail.DataBind();

                Session[Constants.SES_CURRENT_ISSUE] = oIssue;

                for (int i = 0; i < gvCustmerDetail.Rows.Count; i++)
                {
                    HiddenField hdTmpCustomerID = ((HiddenField)gvCustmerDetail.Rows[i].FindControl("hdTmpCustomerID"));
                    if (hdTmpCustomerID != null)
                    {
                        hdTmpCustomerID.Value = i.ToString();
                    }
                }

                //Set Issue Name
                txtIssueName.Text = issueName;
            }
        }

        private void LoadDataByRegNo(string sRegNo)
        {
            Issue oIssue = new Issue();
            oIssue.RegNo = sRegNo;
            IssueDAL oIssueDAL = new IssueDAL();
            
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];            
            if (oConfig != null)
            {
                oIssue.BankID = oConfig.BankCodeID;
                oIssue.DivisionID = oConfig.DivisionID;
            }

            Result oResult = oIssueDAL.LoadDataByRegNo(oIssue);
            TotalClear();
            if (oResult.Status)
            {
                oIssue = (Issue)oResult.Return;
                hdTransNo.Value = oIssue.IssueTransNo;

                SetObject(oIssue, "2");               
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }


        private void SetObject(Issue oIssue, string sApprovalStaus)
        {
            #region Issue Details
            hdReg.Value = oIssue.RegNo;

            DDListUtil.Assign(ddlSpType, oIssue.SPType.SPTypeID.Trim());
            LoadBySPType();

            txtRegistrationNo.Text = oIssue.RegNo;
            txtAppliedAmount.Text = oIssue.IssueAmount.ToString("N2");
            txtIssueName.Text = oIssue.IssueName;

            txtBHDAddress.Text = oIssue.BondHolderAddress;
            txtBHDHolderName.Text = oIssue.BondHolderName;
            txtBHDRelation.Text = oIssue.BondHolderRelation;


            DDListUtil.Assign(ddlCustomerType, oIssue.CustomerType.CustomerTypeID);
            DDListUtil.Assign(ddlBranch, oIssue.Branch.BranchID);
            DDListUtil.Assign(ddlCollectionBranch, oIssue.CollectionBranch);            
            #endregion
            
            #region Payment Details
            DDListUtil.Assign(ddlPDPaymentMode, oIssue.Payment.PaymentMode);
            DDListUtil.Assign(ddlPDCurrencyCode, oIssue.Currency.CurrencyID);
            if (oIssue.Currency.CurrencyID.Equals(oIssue.SPType.Currency.CurrencyID))
            {
                txtPDConvRate.Enabled = false;
            }
            else
            {
                txtPDConvRate.Enabled = true;
            }
            txtPDConvRate.Text = oIssue.Payment.ConvRate.ToString();
            txtPDPaymentAmount.Text = oIssue.Payment.PaymentAmount.ToString();
            if (!string.IsNullOrEmpty(oIssue.Payment.AccountNo) && oIssue.Payment.AccountNo.Length>=12)
            {
                txtPDAccDraftNo.Text = oIssue.Payment.AccountNo.Substring(0, 12);
                txtPDAccName.Text = oIssue.AccountName;
            }
            txtPDDraftNo.Text = oIssue.Payment.RefNo;

            int iPayType = oIssue.Payment.PaymentMode;

            if (iPayType == 2)
            {         // Cheque
                lblPDDraftNo.Text = TEXT_CHEQUE;

            }
            else if (iPayType == 3)
            {  // Draft
                // to active draft TextBox
                // to change label
                lblPDDraftNo.Text = TEXT_DRAFT;

            }
            else if (iPayType == 4 || iPayType == 5 || iPayType == 6)
            {  // Account No
                lblPDDraftNo.Text = TEXT_DRAFT;
                txtPDDraftNo.Text = "";
            }
            else
            {
                lblPDDraftNo.Text = TEXT_DRAFT;
                txtPDDraftNo.Text = "";
            } 
            #endregion
            
            #region Customer List
            DataTable dtCustomerDetails = new DataTable();
            dtCustomerDetails.Columns.Add(new DataColumn("CustomerID", typeof(string)));
            dtCustomerDetails.Columns.Add(new DataColumn("CustomerName", typeof(string)));
            dtCustomerDetails.Columns.Add(new DataColumn("DateOfBirth", typeof(string)));

            DataRow rowCustomerDetails = null;
            for (int i = 0; i < oIssue.CustomerDetailsList.Count; i++)
            {
                rowCustomerDetails = dtCustomerDetails.NewRow();
                rowCustomerDetails["CustomerID"] = oIssue.CustomerDetailsList[i].CustomerID;
                rowCustomerDetails["CustomerName"] = oIssue.CustomerDetailsList[i].CustomerName;
                rowCustomerDetails["DateOfBirth"] = oIssue.CustomerDetailsList[i].DateOfBirth.ToString(Constants.DATETIME_dd_MMM_yyyy);
                dtCustomerDetails.Rows.Add(rowCustomerDetails);
            }

            gvCustmerDetail.DataSource = dtCustomerDetails;
            gvCustmerDetail.DataBind();

            if (SEARCH_FROM.Equals(1) || sApprovalStaus.Equals("2"))
            {
                gvCustmerDetail.Columns[1].Visible = false;
            }
            else
            {
                gvCustmerDetail.Columns[1].Visible = true;
            }

            for (int i = 0; i < gvCustmerDetail.Rows.Count; i++)
            {
                HiddenField hdTmpCustomerID = ((HiddenField)gvCustmerDetail.Rows[i].FindControl("hdTmpCustomerID"));
                if (hdTmpCustomerID != null)
                {
                    hdTmpCustomerID.Value = i.ToString();
                }
            } 
            #endregion
           
            #region Nominee List
            DataTable dtNWDenom = new DataTable();
            dtNWDenom.Columns.Add(new DataColumn("Text", typeof(string)));
            dtNWDenom.Columns.Add(new DataColumn("Value", typeof(string)));
            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();



            DataTable dtNominee = new DataTable();
            dtNominee.Columns.Add(new DataColumn("SlNo", typeof(string)));
            dtNominee.Columns.Add(new DataColumn("NomineeName", typeof(string)));
            dtNominee.Columns.Add(new DataColumn("Relation", typeof(string)));
            dtNominee.Columns.Add(new DataColumn("Address", typeof(string)));
            dtNominee.Columns.Add(new DataColumn("NomineeShare", typeof(string)));
            dtNominee.Columns.Add(new DataColumn("Amount", typeof(string)));
            dtNominee.Columns.Add(new DataColumn("DateOfBirth", typeof(DateTime)));

            DataRow rowNominee = null;
            for (int i = 0; i < oIssue.NomineeList.Count; i++)
            {
                rowNominee = dtNominee.NewRow();

                rowNominee["Slno"] = oIssue.NomineeList[i].SlNo;
                rowNominee["NomineeName"] = oIssue.NomineeList[i].NomineeName;
                rowNominee["Relation"] = oIssue.NomineeList[i].Relation;
                rowNominee["Address"] = oIssue.NomineeList[i].Address;
                rowNominee["NomineeShare"] = oIssue.NomineeList[i].NomineeShare;
                rowNominee["Amount"] = oIssue.NomineeList[i].Amount;
                if (oIssue.NomineeList[i].DateOfBirth.Year > 1900)
                {
                    rowNominee["DateOfBirth"] = Date.GetDateTimeByString(oIssue.NomineeList[i].DateOfBirth.ToString());
                }
                dtNominee.Rows.Add(rowNominee);

                // Nomines Name
                rowNominee = dtNWDenom.NewRow();
                rowNominee["Text"] = oIssue.NomineeList[i].NomineeName;
                rowNominee["Value"] = oIssue.NomineeList[i].SlNo;
                dtNWDenom.Rows.Add(rowNominee);
            }

            //Reload Grid
            gvNomDetail.DataSource = dtNominee;
            gvNomDetail.DataBind(); 
            #endregion

            //if (oIssue.SPType.SPTypeID.Trim().Equals("WDB"))
            //{
            //    ddlNWDName.Items.Clear();
            //    DDListUtil.Assign(ddlNWDName, dtNWDenom, true);
            //}
            //else
            //{
            //    ddlNWDName.Items.Clear();
            //    ddlNWDDenom.Items.Clear();
            //}
            
            #region Denomination list
            DataTable dtDenom = new DataTable();
            dtDenom.Columns.Add(new DataColumn("DenominationID", typeof(string)));
            dtDenom.Columns.Add(new DataColumn("Quantity", typeof(string)));
            DataRow rowDenom = null;
            for (int i = 0; i < oIssue.IssueDenominationList.Count; i++)
            {
                rowDenom = dtDenom.NewRow();

                rowDenom["DenominationID"] = oIssue.IssueDenominationList[i].Denomination.DenominationID.ToString();
                rowDenom["Quantity"] = oIssue.IssueDenominationList[i].Quantity.ToString();
                dtDenom.Rows.Add(rowDenom);
            }
            //Reload Grid
            gvDenomDetail.DataSource = dtDenom;
            gvDenomDetail.DataBind();
            txtTotalAmount.Text = oIssue.IssueAmount.ToString("N2"); 
            #endregion

            //  to varify Master Account 
            CustomerDetails oCustomerDetails = new CustomerDetails();
            oCustomerDetails.MasterNo = oIssue.MasterNo;
            txtMasterNo.Text = oIssue.MasterNo;
            if (string.IsNullOrEmpty(oIssue.MasterNo))
            {
                lblMasterVarified.Text = "Not found!";
            }
            else
            {
                lblMasterVarified.Text = "Verified!";
            }

            Session[Constants.SES_CURRENT_ISSUE] = oIssue;
            UserDetails oUserDetails = ucUserDet.UserDetail;
            //if Approved
            #region User Details
            if (sApprovalStaus.Equals("2"))
            {
                oUserDetails.MakerID = oIssue.UserDetails.MakerID;
                oUserDetails.MakeDate = oIssue.UserDetails.MakeDate;
                oUserDetails.CheckerID = oIssue.UserDetails.CheckerID;
                oUserDetails.CheckDate = oIssue.UserDetails.CheckDate;
                oUserDetails.CheckerComment = oIssue.UserDetails.CheckerComment;
                ucUserDet.UserDetail = oUserDetails;
                txtIssueDate.Text = oIssue.VersionIssueDate.ToString(Constants.DATETIME_FORMAT);

                EnableDisableControl(true);
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);

                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);

                btnRegSeach.Enabled = true;

                gvNomDetail.Enabled = true;
                gvNomDetail.Columns[1].Visible = true;


                fsList.Visible = true;
            }
            else if (SEARCH_FROM.Equals(1))
            {

                oUserDetails.MakerID = oIssue.UserDetails.MakerID;
                oUserDetails.CheckerComment = oIssue.UserDetails.CheckerComment;
                ucUserDet.UserDetail = oUserDetails;
                EnableDisableControl(true);
                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);
                txtIssueDate.Text = oIssue.VersionIssueDate.ToString(Constants.DATETIME_FORMAT);

                gvNomDetail.Enabled = true;
                gvNomDetail.Columns[1].Visible = false;
            }
            else
            {
                oUserDetails.CheckerID = oIssue.UserDetails.CheckerID;
                oUserDetails.CheckDate = oIssue.UserDetails.CheckDate;
                oUserDetails.CheckerComment = oIssue.UserDetails.CheckerComment;
                ucUserDet.UserDetail = oUserDetails;
                txtIssueDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

                EnableDisableControl(false);
            }
            if (sApprovalStaus.Equals("1"))
            {
                if (!txtRegistrationNo.Enabled || ddlPDPaymentMode.SelectedItem.Value.Equals("1"))
                {
                    txtPDDraftNo.Enabled = false;
                    txtPDAccDraftNo.Enabled = false;
                }
                else if (ddlPDPaymentMode.SelectedItem.Value.Equals("2") || ddlPDPaymentMode.SelectedItem.Value.Equals("3"))
                {
                    txtPDDraftNo.Enabled = true;
                    txtPDAccDraftNo.Enabled = false;
                }
                else
                {
                    txtPDDraftNo.Enabled = false;
                    txtPDAccDraftNo.Enabled = true;
                }
            } 
            #endregion
        }

        private void LoadDataByTranID(string sTranID, string sApprovalStaus)
        {
            Issue oIssue = new Issue(sTranID);
            IssueDAL oIssueDAL = new IssueDAL();

            Result oResult = oIssueDAL.LoadTmpDataByTransID(oIssue, sApprovalStaus);
            TotalClear();
            if (oResult.Status)
            {
                oIssue = (Issue)oResult.Return;
                hdTransNo.Value = sTranID;

                SetObject(oIssue, sApprovalStaus);

            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }

        private void EnableDisableControl(bool isApproved)
        {
            // general Control
            if (isApproved)
            {
                //Issue Details
                Util.ControlEnabled(ddlSpType, false);
                Util.ControlEnabled(ddlBranch, false);
                Util.ControlEnabled(ddlCollectionBranch, false);
                Util.ControlEnabled(txtIssueDate, false);
                Util.ControlEnabled(txtRegistrationNo, false);
                Util.ControlEnabled(ddlCustomerType, false);
                Util.ControlEnabled(txtAppliedAmount, false);

                // Policy Detail 
                Util.ControlEnabled(txtIssueName, false);
                Util.ControlEnabled(txtMasterNo, false);
                //Util.ControlEnabled(chkFiscalYear, false);

                // Bond Holder Details
                Util.ControlEnabled(txtBHDHolderName, false);
                Util.ControlEnabled(txtBHDAddress, false);
                Util.ControlEnabled(txtBHDRelation, false);

                //Denomination(s) details
                Util.ControlEnabled(ddlDDDenom, false);
                Util.ControlEnabled(txtDDQuantity, false);
                Util.ControlEnabled(txtTotalAmount, false);

                //Payment Details
                Util.ControlEnabled(ddlPDPaymentMode, false);
                Util.ControlEnabled(ddlPDCurrencyCode, false);
                Util.ControlEnabled(txtPDAccDraftNo, false);
                Util.ControlEnabled(txtPDAccName, false);
                Util.ControlEnabled(txtPDDraftNo, false);
                Util.ControlEnabled(txtPDConvRate, false);

                // user Detail
                //Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), false);

                //Nominee Detail
                Util.ControlEnabled(txtNDAddress, false);
                Util.ControlEnabled(txtNDAmount, false);
                Util.ControlEnabled(txtNDName, false);
                Util.ControlEnabled(txtNDRelation, false);
                Util.ControlEnabled(txtNDShare, false);
                Util.ControlEnabled(txtNDateofBirth, false);

                //Nominee Wise Denomination Detail
                //Util.ControlEnabled(ddlNWDName, false);
                //Util.ControlEnabled(ddlNWDDenom, false);
                //Util.ControlEnabled(txtNWDQuantity, false);
                //Util.ControlEnabled(txtNWDAmount, false);


                // button
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true);

                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);
                Util.ControlEnabled(btnCDAdd, false);
                Util.ControlEnabled(btnNDAdd, false);
                Util.ControlEnabled(btnNDReset, false);
                //Util.ControlEnabled(btnNWDAdd, false);
                //Util.ControlEnabled(btnNWDReset, false);
                Util.ControlEnabled(btnAddDenomination, false);
                btnRegSeach.Enabled = false;

                gvCustmerDetail.Enabled = true;                

                
                gvDenomDetail.Enabled = false;
                gvNomDetail.Enabled = false;
                //gvNomDemon.Enabled = false;
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
                Util.ControlEnabled(btnCDAdd, true);
                Util.ControlEnabled(btnNDAdd, true);
                Util.ControlEnabled(btnNDReset, true);
                //Util.ControlEnabled(btnNWDAdd, true);
                //Util.ControlEnabled(btnNWDReset, true);
                Util.ControlEnabled(btnAddDenomination, true);
                btnRegSeach.Enabled = true;


                // Issue Details                
                Util.ControlEnabled(ddlSpType, true);
                Util.ControlEnabled(ddlBranch, true);
                Util.ControlEnabled(ddlCollectionBranch, true);
                Util.ControlEnabled(txtIssueDate, true);
                Util.ControlEnabled(txtRegistrationNo, true);
                Util.ControlEnabled(ddlCustomerType, true);
                Util.ControlEnabled(txtAppliedAmount, true);
                // Util.ControlEnabled(txtNDAmount, true);

                // Bond Holder Details
                //Util.ControlEnabled(txtBHDHolderName, true);
                //Util.ControlEnabled(txtBHDAddress, true);
                //Util.ControlEnabled(txtBHDRelation, true);

                //Denomination(s) details
                Util.ControlEnabled(ddlDDDenom, true);
                Util.ControlEnabled(txtDDQuantity, true);
                Util.ControlEnabled(txtTotalAmount, true);


                // Policy Detail 
                Util.ControlEnabled(txtIssueName, true);
                Util.ControlEnabled(txtMasterNo, true);
                //Util.ControlEnabled(chkFiscalYear, true);

                //Payment Details
                Util.ControlEnabled(ddlPDPaymentMode, true);
                Util.ControlEnabled(ddlPDCurrencyCode, true);
                Util.ControlEnabled(txtPDAccDraftNo, true);
                Util.ControlEnabled(txtPDAccName, true);
                //Util.ControlEnabled(txtPDConvRate, true);
                Util.ControlEnabled(txtPDDraftNo, true);

                gvCustmerDetail.Enabled = true;
                gvDenomDetail.Enabled = true;
                gvNomDetail.Enabled = true;
                //gvNomDemon.Enabled = true;

                //Nominee Detail
                Util.ControlEnabled(txtNDAddress, true);
                Util.ControlEnabled(txtNDAmount, true);
                Util.ControlEnabled(txtNDName, true);
                Util.ControlEnabled(txtNDRelation, true);
                Util.ControlEnabled(txtNDShare, true);
                Util.ControlEnabled(txtNDateofBirth, true);

                //Nominee Wise Denomination Detail
                //Util.ControlEnabled(ddlNWDName, true);
                //Util.ControlEnabled(ddlNWDDenom, true);
                //Util.ControlEnabled(txtNWDQuantity, true);
                //Util.ControlEnabled(txtNWDAmount, true);
                // user Detail
                //Util.ControlEnabled(ucUserDet.FindControl("txtMakerId"), false);
                //Util.ControlEnabled(ucUserDet.FindControl("txtCheckerId"), false);

                fsList.Visible = true;
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
                LoadDataByTranID(gvRow.Cells[1].Text, "1");//query from Temp Table
            }
        }

        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, null);
        }

        protected void gvNomDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvNomDetail, null);
        }


        private void LoadBySPType()
        {
            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
            SPPolicy oSPPolicy = null;
            SPPolicyDAL oSPPolicyDAL = new SPPolicyDAL();
            Result oResult = null;
            if (oIssue == null)
            {
                oResult = (Result)oSPPolicyDAL.LatestPolicy(ddlSpType.SelectedValue, Constants.ACTIVITY_TYPE.ISSUE, DateTime.Now);
            }
            else
            {
                oResult = (Result)oSPPolicyDAL.LatestPolicy(ddlSpType.SelectedValue, Constants.ACTIVITY_TYPE.ISSUE, oIssue.VersionIssueDate);
            }
            if (oResult != null && oResult.Status)
            {
                ddlCustomerType.Items.Clear();
                ddlPDPaymentMode.Items.Clear();
                ddlPDCurrencyCode.Items.Clear();
                ddlDDDenom.Items.Clear();
                //ddlNWDDenom.Items.Clear();

                oSPPolicy = (SPPolicy)oResult.Return;
                //To be used in CustomerDetail
                hdSupportdGndr.Value = Convert.ToString(oSPPolicy.SupportedSex);

                DDListUtil.Assign(ddlCustomerType, oSPPolicy.DTCustomerTypePolicy, true);
                DDListUtil.Assign(ddlPDPaymentMode, oSPPolicy.DTPaymentPolicy, true);
                if (ddlPDPaymentMode.Items.Count > 0)
                {
                    DDListUtil.Assign(ddlPDPaymentMode, Constants.PAYMENT_MODE_CUSTOMER_ACC); // this is for Customer Account
                }
                DDListUtil.Assign(ddlPDCurrencyCode, oSPPolicy.DTCurrencyActivityPolicy, true);
                                
                if (ddlPDCurrencyCode.Items.Count > 0)
                {
                    bool isToBeLoaded = false;
                    if (oIssue != null)
                    {
                        if (oIssue.SPType.SPTypeID != ddlSpType.SelectedValue)
                        {
                            isToBeLoaded = true;
                        }
                    }
                    else
                    {
                        isToBeLoaded = true;
                    }
                    if (isToBeLoaded)
                    {
                        SPTypeDAL spTDAL = new SPTypeDAL();
                        Result oCurResult = spTDAL.GetCurrencyBySPTypeID(ddlSpType.SelectedValue);

                        if (oCurResult.Status)
                        {
                            SPType oSPType = oCurResult.Return as SPType;
                            if (ddlSpType.SelectedValue.Equals(Constants.SP_TYPE_WDB))
                            {
                                //Exceptional for WDB. USD will be set as Default currency 
                                DDListUtil.Assign(ddlPDCurrencyCode, "01");
                                txtPDConvRate.Enabled = true;
                                txtPDConvRate.Text = string.Empty;
                            }
                            else
                            {
                                // Set default currecy tagged with SPType
                                DDListUtil.Assign(ddlPDCurrencyCode, oSPType.Currency.CurrencyID);
                                txtPDConvRate.Enabled = false;
                            }

                            if (oIssue != null)
                            {
                                oIssue.SPType = oSPType;
                            }
                            else
                            {
                                oIssue = new Issue();
                                oIssue.SPType = oSPType;
                            }
                        }
                    }
                }

                DataTable dtDenom = new DataTable();
                if (oSPPolicy.SPType.ListOfDenomination.Denomination.Count > 0)
                {
                    dtDenom.Columns.Add(new DataColumn("Text", typeof(string)));
                    dtDenom.Columns.Add(new DataColumn("Value", typeof(string)));

                    DataRow rowDenom = null;
                    for (int i = 0; i < oSPPolicy.SPType.ListOfDenomination.Denomination.Count; i++)
                    {
                        rowDenom = dtDenom.NewRow();

                        rowDenom["Text"] = oSPPolicy.SPType.ListOfDenomination.Denomination[i].DenominationID.ToString();
                        rowDenom["Value"] = oSPPolicy.SPType.ListOfDenomination.Denomination[i].Series.ToString();
                        dtDenom.Rows.Add(rowDenom);
                    }
                }
                DDListUtil.Assign(ddlDDDenom, dtDenom, true);
                //DDListUtil.Assign(ddlNWDDenom, dtDenom, true);
                
                if (oIssue != null)
                {
                    oIssue.VersionSPPolicy = oSPPolicy;
                }
                else
                {
                    oIssue = new Issue();
                    oIssue.VersionSPPolicy = oSPPolicy;
                }

                Session[Constants.SES_CURRENT_ISSUE] = oIssue;
                
                //gvStock.DataSource = null;
                //gvStock.DataBind();
                StockInfoLoad(oSPPolicy.SPType.SPTypeID);


                txtBHDHolderName.Enabled = false;
                txtBHDAddress.Enabled = false;
                txtBHDRelation.Enabled = false;
                
                if (oSPPolicy.IsBondHolderRequired)
                {
                    hdIsBondHolderRequired.Value = oSPPolicy.IsBondHolderRequired.ToString();
                    //ddlNWDDenom.Enabled = true;
                    //ddlNWDName.Enabled = true;
                    txtBHDHolderName.Enabled = true;
                    txtBHDAddress.Enabled = true;
                    txtBHDRelation.Enabled = true;
                }
                else
                {
                    hdIsBondHolderRequired.Value = oSPPolicy.IsBondHolderRequired.ToString();
                    //ddlNWDDenom.Enabled = false;
                    //ddlNWDName.Enabled = false;
                    txtBHDHolderName.Enabled = false;
                    txtBHDAddress.Enabled = false;
                    txtBHDRelation.Enabled = false;
                }
            }
        }

        protected void ddlSpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sSpType = ddlSpType.SelectedValue;
            if (!string.IsNullOrEmpty(ddlSpType.SelectedValue))
            {                
                TotalClear();                                
                //Session[Constants.SES_CURRENT_ISSUE] = null;
                DDListUtil.Assign(ddlSpType, sSpType);

                //ddlNWDDenom.Items.Clear();
                //ddlNWDName.Items.Clear();

                //ddlNWDDenom.Enabled = false;
                //ddlNWDName.Enabled = false;

                ddlCustomerType.Focus();
                
                SPPolicyDAL spDal = new SPPolicyDAL();
                Result oResult = spDal.IsExistPolicy(ddlSpType.SelectedValue);
                if (oResult.Status)
                {
                    int i = (int)oResult.Return;
                    if (i > 0)
                    {
                        LoadBySPType();
                    }
                    else
                    {
                        ddlCustomerType.Items.Clear();
                        ddlPDPaymentMode.Items.Clear();
                        ddlPDCurrencyCode.Items.Clear();
                        ddlDDDenom.Items.Clear();
                        //ddlNWDDenom.Items.Clear();

                        ucMessage.OpenMessage("No policy has been set for  " + sSpType + ". Please check.", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    }
                }
            }
            else
            {
                ddlSpType.Focus();
                ddlCustomerType.Items.Clear();
                ddlPDPaymentMode.Items.Clear();
                ddlPDCurrencyCode.Items.Clear();
                ddlDDDenom.Items.Clear();
                //ddlNWDDenom.Items.Clear();
            }            
        }

        private void StockInfoLoad(string sTypeID)
        {            
            if (!string.IsNullOrEmpty(sTypeID))
            {
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                Result oResult = new Result();
                ReceiveDAL oRecDal = new ReceiveDAL();
                oResult = oRecDal.StockDataInfo(sTypeID, oConfig.DivisionID, oConfig.BankCodeID);
                if (oResult.Status)
                {
                    ViewState[_STOCK_INFO] = (DataTable)oResult.Return;                    
                }
            }
        }

        public void CustomerDetailAction(CustomerDetails oCustomer)
        {
            //Limit Checkup
            IssueDAL oIssueDAL = new IssueDAL();
            if (ddlSpType.SelectedValue=="3MS" || ddlSpType.SelectedValue=="BSP")
            {
                Result oResult = new Result();
                if (ddlCustomerType.SelectedItem.Text.Contains("Individual"))
                {
                    try
                    {
                        decimal.Parse(txtAppliedAmount.Text);
                        oResult = oIssueDAL.LoadCheckLimit(ddlSpType.SelectedValue, oCustomer.MasterNo, "", oCustomer.NationalID, "", Convert.ToDecimal(txtAppliedAmount.Text));
                    }
                    catch (Exception)
                    {
                        oResult = oIssueDAL.LoadCheckLimit(ddlSpType.SelectedValue, oCustomer.MasterNo, "", oCustomer.NationalID,"", 0);
                    }
                }
                else if (ddlCustomerType.SelectedItem.Text.Contains("Joint"))
                {
                    try
                    {
                        decimal.Parse(txtAppliedAmount.Text);
                        oResult = oIssueDAL.LoadCheckLimit(ddlSpType.SelectedValue, oCustomer.MasterNo, "", oCustomer.NationalID, "", Convert.ToDecimal(txtAppliedAmount.Text) / 2);
                    }
                    catch (Exception)
                    {
                        oResult = oIssueDAL.LoadCheckLimit(ddlSpType.SelectedValue, oCustomer.MasterNo, "", oCustomer.NationalID, "", 0);
                    }
                }
                else
                {
                    oResult.Status = true;
                }
                if (!oResult.Status)
                {
                    ucMessage.OpenMessage(oResult.Message, Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    return;
                }
            }
            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
            // To Check MasterID
            CustomerDetails oCustMastExist = oIssue.CustomerDetailsList.Where(c => c.MasterNo.Equals(oCustomer.MasterNo)).SingleOrDefault();
            DateTime dtCustAdultDate = DateTime.Now;
            try
            {
                // to check adult user or not..
                dtCustAdultDate = new DateTime(oCustomer.DateOfBirth.Year + 18, oCustomer.DateOfBirth.Month, oCustomer.DateOfBirth.Day);
            }
            catch (Exception ex)
            {
                //Throw an Unpre
            }
            
            DateTime dtToday = DateTime.Now;

            if (dtToday >= dtCustAdultDate)
            {
                if (ddlCustomerType.SelectedItem.Text.Contains("Individual"))
                {
                    if (oIssue.CustomerDetailsList.Count > 1)//changed by Istiak
                    {
                        // no same master ID
                        ucMessage.OpenMessage("Cannot create multiple user of this Sanchaya Patra Type", Constants.MSG_TYPE_SUCCESS);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        return;
                    }
                }
            }

            if (oIssue == null)
            {
                oIssue = new Issue();
            }
            if (oCustMastExist != null) // No delete operation for new Customer information
            {
                CustomerDetails oCustDetlExist = oIssue.CustomerDetailsList.Where(c => c.MasterNo.Equals(oCustMastExist.MasterNo)).SingleOrDefault();
                if (oCustDetlExist != null)
                {
                    oIssue.CustomerDetailsList.Remove(oCustDetlExist);
                }
            }
            //oCustomer.UserDetails = ucUserDet.UserDetail;
            oIssue.CustomerDetailsList.Add(oCustomer);


            DataTable dtCustomerDetails = new DataTable();

            dtCustomerDetails.Columns.Add(new DataColumn("CustomerID", typeof(string)));
            dtCustomerDetails.Columns.Add(new DataColumn("CustomerName", typeof(string)));
            dtCustomerDetails.Columns.Add(new DataColumn("DateOfBirth", typeof(string)));

            DataRow rowCustomerDetails = null;
            string issueName = string.Empty;

            for (int i = 0; i < oIssue.CustomerDetailsList.Count; i++)
            {
                rowCustomerDetails = dtCustomerDetails.NewRow();

                rowCustomerDetails["CustomerID"] = oIssue.CustomerDetailsList[i].CustomerID == -1 ? "New Customer" : oIssue.CustomerDetailsList[i].CustomerID.ToString();// oIssue.CustomerDetailsList[i].CustomerID;
                //if (oIssue.CustomerDetailsList[i].CustomerName2.Trim() != "" && oIssue.CustomerDetailsList[i].isJointlyOperate)
                //{
                //    rowCustomerDetails["CustomerName"] = oIssue.CustomerDetailsList[i].CustomerName + " AND " + oIssue.CustomerDetailsList[i].CustomerName2;
                //}
                //else if (oIssue.CustomerDetailsList[i].CustomerName2.Trim() != "" && oIssue.CustomerDetailsList[i].isJointlyOperate == false)
                //{
                //    rowCustomerDetails["CustomerName"] = oIssue.CustomerDetailsList[i].CustomerName + " AND/OR " + oIssue.CustomerDetailsList[i].CustomerName2;
                //}
                //else
                //{
                //    rowCustomerDetails["CustomerName"] = oIssue.CustomerDetailsList[i].CustomerName;
                //}
                rowCustomerDetails["CustomerName"] = oIssue.CustomerDetailsList[i].CustomerName.Trim();
                if (oIssue.CustomerDetailsList[i].DateOfBirth.Year > 1900)
                {
                    rowCustomerDetails["DateOfBirth"] = oIssue.CustomerDetailsList[i].DateOfBirth.ToString(Constants.DATETIME_dd_MMM_yyyy);
                }
                //Issue Name is generally the customer name
             

                if (i > 0)
                {
                    
                    issueName += " AND ";
                }

                issueName += oIssue.CustomerDetailsList[i].CustomerName.Trim();

                //if (!string.IsNullOrEmpty(oIssue.CustomerDetailsList[i].CustomerName2) && oIssue.CustomerDetailsList[i].isJointlyOperate)
                //{
                //    issueName += " AND " + oIssue.CustomerDetailsList[i].CustomerName2;
                //}
                //else if (!string.IsNullOrEmpty(oIssue.CustomerDetailsList[i].CustomerName2) && oIssue.CustomerDetailsList[i].isJointlyOperate == false)
                //{
                //    issueName += " AND/OR " + oIssue.CustomerDetailsList[i].CustomerName2;
                //}

                dtCustomerDetails.Rows.Add(rowCustomerDetails);
            }


            gvCustmerDetail.DataSource = dtCustomerDetails;
            gvCustmerDetail.DataBind();

            for (int i = 0; i < gvCustmerDetail.Rows.Count; i++)
            {
                HiddenField hdTmpCustomerID = ((HiddenField)gvCustmerDetail.Rows[i].FindControl("hdTmpCustomerID"));
                if (hdTmpCustomerID != null)
                {
                    hdTmpCustomerID.Value = i.ToString();
                }
            }
            Session[Constants.SES_CURRENT_ISSUE] = oIssue;

            //Set Issue Name
            txtIssueName.Text = issueName;

            //txtBHDHolderName.Text = oCustomer.CustomerName;
            //txtBHDAddress.Text = oCustomer.Address;
            if (!string.IsNullOrEmpty(oCustomer.MasterNo))
            {
                txtMasterNo.Text = oCustomer.MasterNo;
                lblMasterVarified.Text = "Verified!";
            }
            else
            {
                lblMasterVarified.Text = "Not verified yet!";
            }
            txtIssueName.Focus();
            if (txtBHDHolderName.Enabled)
            {
                txtBHDHolderName.Text = oIssue.CustomerDetailsList[0].CustomerName;
                txtBHDRelation.Text = "SELF";
                txtBHDAddress.Text   = oIssue.CustomerDetailsList[0].Address;
            }
        }


        // need to remove. 
        private void LoadCustomerTypeBySpType()
        {
            CustomerTypeWiseSPLimitDAL oCustTypeWiseSPLimit = new CustomerTypeWiseSPLimitDAL();
            Result oResult = (Result)oCustTypeWiseSPLimit.GetCustomerTypeBySpType(ddlSpType.SelectedValue);
            if (oResult.Status)
            {
                ddlCustomerType.Items.Clear();
                DataTable dtGetCustomerTypeID = (DataTable)oResult.Return;
                DDListUtil.Assign(ddlCustomerType, dtGetCustomerTypeID);
            }

        }
        
        // need to remove. 
        private void LoadDenominationBySPType()
        {
            SPTypeDAL oSPTypeDAL = new SPTypeDAL();
            Result oResult = (Result)oSPTypeDAL.GetDDLDenomList(ddlSpType.SelectedValue);
            if (oResult.Status)
            {
                ddlDDDenom.Items.Clear();
                DataTable dtGetDenomID = (DataTable)oResult.Return;
                DDListUtil.Assign(ddlDDDenom, dtGetDenomID);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (hdReg.Value == "")
            {
                Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];

                decimal dCount = 0;
                if (oIssue != null)
                {
                    if (oIssue.NomineeList.Count > 0)
                    {
                        for (int i = 0; i < oIssue.NomineeList.Count; i++)
                        {
                            if (oIssue.NomineeList[i].NomineeShare > 0)
                            {
                                dCount += oIssue.NomineeList[i].NomineeShare;
                            }
                        }
                    }
                }
                if (dCount == 0 || dCount == 100)
                {
                    try
                    {
                        SaveAction();                    
                    }
                    catch (Exception ex)
                    {
                        ucMessage.OpenMessage(ex.Message, Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    }
                   }
                else
                {
                    ucMessage.OpenMessage("Total amount of share must be 100!!", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_APPROVED_SAVE_DATA, Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }

        private void SaveAction()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            
            if (oConfig != null)
            {
                Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];

                if (oIssue == null)
                {
                    oIssue = new Issue();
                }

                // Issue Details 
                oIssue.IssueName = txtIssueName.Text.ToUpper();
                oIssue.VersionIssueDate = Date.GetDateTimeByString(txtIssueDate.Text.ToString());
                oIssue.AccountName = txtPDAccName.Text;
                oIssue.MasterNo = txtMasterNo.Text;
                
                // Bond Holder Details
                oIssue.BondHolderAddress = txtBHDAddress.Text;
                oIssue.BondHolderName = txtBHDHolderName.Text;
                oIssue.BondHolderRelation = txtBHDRelation.Text;

                //SP Type Details
                oIssue.SPType.SPTypeID = ddlSpType.SelectedValue != "" ? ddlSpType.SelectedValue : "";
                oIssue.Branch.BranchID = ddlBranch.SelectedValue != "" ? ddlBranch.SelectedValue : "";
                oIssue.CollectionBranch = ddlCollectionBranch.SelectedValue != "" ? ddlCollectionBranch.SelectedValue : "";
                oIssue.IssueTransNo = hdTransNo.Value == "" ? "-1" : hdTransNo.Value;
                oIssue.RegNo = hdReg.Value;
                oIssue.CustomerType.CustomerTypeID = ddlCustomerType.SelectedValue != "" ? ddlCustomerType.SelectedValue : "";
                oIssue.IssueAmount = Util.GetDecimalNumber(txtTotalAmount.Text);

                //Status Details
                oIssue.IsApproved = 2; //Changed by Istiak on 13/06/2012
                oIssue.IsClaimed = false;
                oIssue.Status = (int)Constants.ISSUE_STATUS.ISSUED;

                //Payment Details
                oIssue.Currency.CurrencyID = ddlPDCurrencyCode.SelectedValue != "" ? ddlPDCurrencyCode.SelectedValue : "";
                oIssue.Payment.PaymentMode = Util.GetIntNumber(ddlPDPaymentMode.SelectedItem.Value);
                if (!string.IsNullOrEmpty(txtPDAccDraftNo.Text))
                {
                    oIssue.Payment.AccountNo = txtPDAccDraftNo.Text; //+ddlPDCurrencyCode.SelectedValue;
                }                
                oIssue.Payment.PaymentAmount = Util.GetDecimalNumber(txtPDPaymentAmount.Text);
                oIssue.Payment.RefNo = txtPDDraftNo.Text;
                oIssue.Payment.ConvRate = Util.GetDecimalNumber(txtPDConvRate.Text);


                //User Details
                oIssue.UserDetails = ucUserDet.UserDetail;
                oIssue.UserDetails.MakeDate = DateTime.Now;
              
                IssueDAL oIssueDAL = new IssueDAL();
                oIssue.UserDetails = ucUserDet.UserDetail;
                oIssue.UserDetails.MakeDate = DateTime.Now;
                ucUserDet.ResetData();
                Result oResult = (Result)oIssueDAL.Save(oIssue);

                if (oResult.Status)
                {
                    TotalClear();
                    this.LoadPreviousList();
                  
                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);                    
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE + " (" + oResult.Message + ")", Constants.MSG_TYPE_ERROR);
                }
            }else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
            }
        }
    

        protected void btnNDAdd_Click(object sender, EventArgs e)
        {
            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
            decimal dCount = 0;
            if (oIssue != null)
            {
                if (oIssue.NomineeList.Count > 0)
                {
                    for (int i = 0; i < oIssue.NomineeList.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(hdNomSlno.Value))
                        {
                            int j = Util.GetIntNumber(hdNomSlno.Value);
                            if (j - 1 != i)
                            {
                                dCount += oIssue.NomineeList[i].NomineeShare;
                            }
                        }
                        else if (oIssue.NomineeList[i].NomineeShare > 0)
                        {
                            dCount += oIssue.NomineeList[i].NomineeShare;
                        }
                    }
                }
                if (dCount + Util.GetDecimalNumber(txtNDShare.Text) <= 100)
                {
                    AddNomineeToSession(oIssue);

                    txtNDateofBirth.Text = "";


                    txtNDName.Text = string.Empty;
                    txtNDRelation.Text = string.Empty;
                    txtNDAddress.Text = string.Empty;
                    txtNDParmanentAddress.Text = string.Empty;
                    txtNDShare.Text = string.Empty;
                    txtNDAmount.Text = string.Empty;
                    txtNDateofBirth.Text = string.Empty;

                    ddlGender.SelectedValue = "M";
                    ddlResidenceStatus.SelectedValue = "R";
                    txtNationalID.Text = string.Empty;
                    txtNationalID_IssueAt.Text = string.Empty;
                    txtPassportNo.Text = string.Empty;
                    txtPassportNo_IssueAt.Text = string.Empty;
                    txtBirthCertificateNo.Text = string.Empty;
                    txtBirthCertificateNo_IssueAt.Text = string.Empty;
                    txtTIN.Text = string.Empty;
                    txtPhone.Text = string.Empty;
                    txtEmailAddress.Text = string.Empty;

                    DDListUtil.Assign(ddlResidentCountry, "BANGLADESH");
                    DDListUtil.Assign(ddlPassportNoCountry, "BANGLADESH");
                    DDListUtil.Assign(ddlNationalIDCountry, "BANGLADESH");
                    DDListUtil.Assign(ddlBirthCertificateNoCountry, "BANGLADESH");

                }
                else
                {
                    ucMessage.OpenMessage("Total amount of share cannot be exceeded 100", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
            else
            {
                AddNomineeToSession(new Issue());

                txtNDName.Text = string.Empty;
                txtNDRelation.Text = string.Empty;
                txtNDAddress.Text = string.Empty;
                txtNDShare.Text = string.Empty;
                txtNDAmount.Text = string.Empty;
                txtNDateofBirth.Text = string.Empty;

                txtNDAddress.Text = string.Empty;
                txtNDParmanentAddress.Text = string.Empty;
                txtNDShare.Text = string.Empty;
                txtNDAmount.Text = string.Empty;

                ddlGender.SelectedValue = "M";
                ddlResidenceStatus.SelectedValue = "R";
                txtNationalID.Text = string.Empty;
                txtNationalID_IssueAt.Text = string.Empty;
                txtPassportNo.Text = string.Empty;
                txtPassportNo_IssueAt.Text = string.Empty;
                txtBirthCertificateNo.Text = string.Empty;
                txtBirthCertificateNo_IssueAt.Text = string.Empty;
                txtTIN.Text = string.Empty;
                txtPhone.Text = string.Empty;
                txtEmailAddress.Text = string.Empty;


                DDListUtil.Assign(ddlResidentCountry, "BANGLADESH");
                DDListUtil.Assign(ddlPassportNoCountry, "BANGLADESH");
                DDListUtil.Assign(ddlNationalIDCountry, "BANGLADESH");
                DDListUtil.Assign(ddlBirthCertificateNoCountry, "BANGLADESH");
            }
            hdNomSlno.Value = "";
        }

        private void AddNomineeToSession(Issue oIssue)
        {
            //Nominee Details
            Nominee oNominee = null;
            bool isToEdit = false;
            int editIndex = 0;

            if (!string.IsNullOrEmpty(hdNomSlno.Value))
            {
                oNominee = oIssue.NomineeList.Where(n => n.SlNo.Equals(Convert.ToInt32(hdNomSlno.Value))).SingleOrDefault();
                oNominee.SlNo = Convert.ToInt32(hdNomSlno.Value);
                editIndex = oIssue.NomineeList.FindIndex(n => n.SlNo.Equals(Convert.ToInt32(hdNomSlno.Value)));
                isToEdit = true;
            }
            else
            {
                oNominee = new Nominee();
                oNominee.SlNo = oIssue.NomineeList.Count + 1;
            }
            oNominee.Address = txtNDAddress.Text;
            oNominee.ParmanentAddress = txtNDParmanentAddress.Text;
            oNominee.Amount = Util.GetDecimalNumber(txtNDAmount.Text);
            oNominee.IssueTransNo = Convert.ToString(1);
            oNominee.NomineeName = txtNDName.Text;
            oNominee.NomineeShare = Util.GetDecimalNumber(txtNDShare.Text);
            oNominee.Relation = txtNDRelation.Text;
            oNominee.DateOfBirth = Util.GetDateTimeByString(txtNDateofBirth.Text);
            oNominee.Sex = ddlGender.SelectedValue;
            oNominee.ResidentStatus = ddlResidenceStatus.SelectedValue;
            oNominee.Resident_Country = ddlResidentCountry.SelectedValue;
            oNominee.NationalID = txtNationalID.Text;
            oNominee.NationalID_Country = ddlNationalIDCountry.SelectedValue;
            oNominee.NationalID_IssueAt = txtNationalID_IssueAt.Text;
            oNominee.PassportNo = txtPassportNo.Text;
            oNominee.PassportNo_Country = ddlPassportNoCountry.SelectedValue;
            oNominee.PassportNo_IssueAt = txtPassportNo_IssueAt.Text;
            oNominee.BirthCertificateNo = txtBirthCertificateNo.Text;
            oNominee.BirthCertificateNo_Country = ddlBirthCertificateNoCountry.SelectedValue;
            oNominee.BirthCertificateNo_IssueAt = txtBirthCertificateNo_IssueAt.Text;
            oNominee.TIN = txtTIN.Text;
            oNominee.Phone = txtPhone.Text;
            oNominee.EmailAddress = txtEmailAddress.Text;

            oNominee.UserDetails = ucUserDet.UserDetail;
            //Add Nominee
            if (!isToEdit)
            {
                oIssue.NomineeList.Add(oNominee);
            }
            else // Edit Nominee
            {
                oIssue.NomineeList[editIndex] = oNominee;
            }

            DataTable dtNominee = new DataTable();

            dtNominee.Columns.Add(new DataColumn("SlNo", typeof(string)));
            dtNominee.Columns.Add(new DataColumn("NomineeName", typeof(string)));
            dtNominee.Columns.Add(new DataColumn("Relation", typeof(string)));
            dtNominee.Columns.Add(new DataColumn("Address", typeof(string)));
            dtNominee.Columns.Add(new DataColumn("NomineeShare", typeof(string)));
            dtNominee.Columns.Add(new DataColumn("Amount", typeof(string)));

            DataRow rowNominee = null;
            for (int i = 0; i < oIssue.NomineeList.Count; i++)
            {
                rowNominee = dtNominee.NewRow();

                rowNominee["Slno"] = oIssue.NomineeList[i].SlNo;
                rowNominee["NomineeName"] = oIssue.NomineeList[i].NomineeName;
                rowNominee["Relation"] = oIssue.NomineeList[i].Relation;
                rowNominee["Address"] = oIssue.NomineeList[i].Address;
                rowNominee["NomineeShare"] = oIssue.NomineeList[i].NomineeShare;
                rowNominee["Amount"] = oIssue.NomineeList[i].Amount;

                dtNominee.Rows.Add(rowNominee);
            }

            //Reload Grid
            gvNomDetail.DataSource = dtNominee;
            gvNomDetail.DataBind();
            //Update Session
            Session[Constants.SES_CURRENT_ISSUE] = oIssue;
        }

        protected void btnAddDenomination_Click(object sender, EventArgs e)
        {
            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
            DataTable dtStockReg = ViewState[_STOCK_INFO] as DataTable;
            bool isOutOfStock = false;

            if (dtStockReg != null && dtStockReg.Rows.Count > 0)
            {
                int iRNumber = 0;
                int iUNumber = 0;
                int iCRNumber = 0;
                for (int i = 0; i < dtStockReg.Rows.Count; i++)
                {
                    if (DB.GetDBValue(dtStockReg.Rows[i]["Denomination"]).Equals(ddlDDDenom.SelectedValue))
                    {
                        iRNumber = Util.GetIntNumber(DB.GetDBValue(dtStockReg.Rows[i]["Remaining Demomination"]));
                        break;
                    }
                }
                if (iRNumber.Equals(0))
                {
                    isOutOfStock = true;
                }
                else
                {
                    for (int i = 0; i < gvDenomDetail.Rows.Count; i++)
                    {
                        GridViewRow dr = (GridViewRow)gvDenomDetail.Rows[i];
                        if (dr.Cells[1].Text == ddlDDDenom.SelectedValue)
                        {
                            iUNumber += Util.GetIntNumber(dr.Cells[2].Text);
                        }
                    }
                    iCRNumber = iRNumber - iUNumber;
                    if (Util.GetIntNumber(txtDDQuantity.Text) <= iCRNumber)
                    {
                        if (oIssue != null)
                        {
                            AddDenominationToSession(oIssue);
                        }
                        else
                        {
                            AddDenominationToSession(new Issue());
                        }
                    }
                    else
                    {
                        ucMessage.OpenMessage("Denomination quantity cannot be higher than current stock quantity!!", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel9, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    }
                }

                if (ddlDDDenom.Items.Count > 0)
                {
                    ddlDDDenom.SelectedIndex = 0;
                    txtDDQuantity.Text = "";
                }

            }
            else
            {
                isOutOfStock = true;
            }
            if (isOutOfStock)
            {
                ucMessage.OpenMessage("selected denomination currently out of stock.", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel9, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
            if(Util.GetDecimalNumber(txtTotalAmount.Text).Equals(Util.GetDecimalNumber(txtAppliedAmount.Text)))
            {
                txtPDAccDraftNo.Focus();
            }
            else
            {
                ddlDDDenom.Focus();
            }
        }

        private void AddDenominationToSession(Issue oIssue)
        {            
            // need to check that can be possible to add or not
            decimal dOldAmount = 0;
            for (int i = 0; i < oIssue.IssueDenominationList.Count; i++)
            {
                if (oIssue.IssueDenominationList[i].Denomination.DenominationID != Util.GetIntNumber(ddlDDDenom.SelectedValue))
                {
                    dOldAmount = dOldAmount + Convert.ToDecimal(oIssue.IssueDenominationList[i].Denomination.DenominationID) * Convert.ToDecimal(oIssue.IssueDenominationList[i].Quantity);
                } 
            }
            dOldAmount = dOldAmount + Util.GetDecimalNumber(ddlDDDenom.SelectedValue) * Util.GetDecimalNumber(txtDDQuantity.Text);
            if (Convert.ToDecimal(Util.GetDecimalNumber(txtAppliedAmount.Text)) >= dOldAmount)
            {
                // can be possible to add
                if (!string.IsNullOrEmpty(ddlDDDenom.SelectedValue) && !string.IsNullOrEmpty(txtAppliedAmount.Text))
                {
                    IssueDenomination oIssueDenom = oIssue.IssueDenominationList.Where(d => d.Denomination.DenominationID.Equals(Convert.ToInt32(ddlDDDenom.SelectedValue))).SingleOrDefault();

                    if (oIssueDenom != null)
                    {
                        oIssue.IssueDenominationList.Remove(oIssueDenom);
                    }
                    else
                    {
                        oIssueDenom = new IssueDenomination();
                    }

                    oIssueDenom.Denomination.DenominationID = Util.GetIntNumber(ddlDDDenom.SelectedValue);

                    oIssueDenom.IssueNo = 1;//To be queried
                    oIssueDenom.IssueTransNo = 1;
                    oIssueDenom.Quantity = Util.GetIntNumber(txtDDQuantity.Text);
                    if (!string.IsNullOrEmpty(ddlSpType.SelectedValue))
                    {
                        oIssueDenom.SPType.SPTypeID = ddlSpType.SelectedValue;
                    }
                    //Add to List
                    oIssueDenom.UserDetail = ucUserDet.UserDetail;
                    oIssue.IssueDenominationList.Add(oIssueDenom);

                    DataTable dtDenom = new DataTable();

                    dtDenom.Columns.Add(new DataColumn("DenominationID", typeof(string)));
                    dtDenom.Columns.Add(new DataColumn("Quantity", typeof(string)));
                    decimal dTotalAmount = 0;
                    DataRow rowDenom = null;
                    for (int i = 0; i < oIssue.IssueDenominationList.Count; i++)
                    {
                        rowDenom = dtDenom.NewRow();

                        rowDenom["DenominationID"] = oIssue.IssueDenominationList[i].Denomination.DenominationID.ToString();
                        rowDenom["Quantity"] = oIssue.IssueDenominationList[i].Quantity.ToString();
                        dTotalAmount = dTotalAmount + (Convert.ToDecimal(oIssue.IssueDenominationList[i].Denomination.DenominationID) * Convert.ToDecimal(oIssue.IssueDenominationList[i].Quantity));
                        dtDenom.Rows.Add(rowDenom);
                    }
                    // if not exceed total amount
                    if (Util.GetDecimalNumber(txtAppliedAmount.Text) >= dTotalAmount)
                    {
                        //Reload Grid
                        gvDenomDetail.DataSource = dtDenom;
                        gvDenomDetail.DataBind();
                        txtTotalAmount.Text = dTotalAmount.ToString("N2");

                        //Update Session
                        Session[Constants.SES_CURRENT_ISSUE] = oIssue;                        
                    }
                    else
                    {                        
                        ucMessage.OpenMessage("Total amount cannot be exceeded.", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    }
                }                
            }
            else
            {                
                ucMessage.OpenMessage("Total amount cannot be exceeded.", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }
        protected void gvDenomDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
            if (oIssue != null)
            {
                IssueDenomination oIssueDenom = oIssue.IssueDenominationList.Where(d => d.Denomination.DenominationID.Equals(Convert.ToInt32(gvRow.Cells[1].Text))).SingleOrDefault();
                if (oIssueDenom != null)
                {
                    oIssue.IssueDenominationList.Remove(oIssueDenom);
                }

                int iTotalAmount = 0;
                DataTable dtDenom = new DataTable();

                dtDenom.Columns.Add(new DataColumn("DenominationID", typeof(string)));
                dtDenom.Columns.Add(new DataColumn("Quantity", typeof(string)));

                DataRow rowDenom = null;
                for (int i = 0; i < oIssue.IssueDenominationList.Count; i++)
                {
                    rowDenom = dtDenom.NewRow();

                    rowDenom["DenominationID"] = oIssue.IssueDenominationList[i].Denomination.DenominationID.ToString();
                    rowDenom["Quantity"] = oIssue.IssueDenominationList[i].Quantity.ToString();
                    iTotalAmount = iTotalAmount + (oIssue.IssueDenominationList[i].Denomination.DenominationID * oIssue.IssueDenominationList[i].Quantity);
                    dtDenom.Rows.Add(rowDenom);
                }
                //Reload Grid
                gvDenomDetail.DataSource = dtDenom;
                gvDenomDetail.DataBind();
                txtTotalAmount.Text = iTotalAmount.ToString("N2");
               // txtAppliedAmount.Text = iTotalAmount.ToString();

                Session[Constants.SES_CURRENT_ISSUE] = oIssue;
            }
        }

        protected void gvNomDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = null;

            if (((Button)e.CommandSource).Text.Equals("Select"))
            {
                gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                hdNomSlno.Value = ((HiddenField)gvRow.FindControl("hdNomineeSlno")).Value;
                Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
                Nominee oNominee = oIssue.NomineeList.Where(n => n.SlNo.Equals(Convert.ToInt32(hdNomSlno.Value))).SingleOrDefault();
                //DateTime parsedDate;
                //DateTime.TryParseExact(gvRow.Cells[7].Text, Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                if (oNominee.DateOfBirth.Year > 1900)
                {
                    txtNDateofBirth.Text = oNominee.DateOfBirth.ToString(Constants.DATETIME_FORMAT);
                }

                txtNDName.Text = oNominee.NomineeName;
                txtNDRelation.Text = oNominee.Relation;
                txtNDAddress.Text = oNominee.Address;
                txtNDParmanentAddress.Text = oNominee.ParmanentAddress;
                txtNDShare.Text = oNominee.NomineeShare.ToString("N2");
                txtNDAmount.Text = oNominee.Amount.ToString("N2");

                ddlGender.SelectedValue = oNominee.Sex;
                ddlResidenceStatus.SelectedValue = oNominee.ResidentStatus;
                ddlResidentCountry.SelectedValue = oNominee.Resident_Country;
                txtNationalID.Text = oNominee.NationalID;
                ddlNationalIDCountry.SelectedValue = oNominee.NationalID_Country;
                txtNationalID_IssueAt.Text = oNominee.NationalID_IssueAt;
                txtPassportNo.Text = oNominee.PassportNo;
                ddlPassportNoCountry.SelectedValue = oNominee.PassportNo_Country;
                txtPassportNo_IssueAt.Text = oNominee.PassportNo_IssueAt;
                txtBirthCertificateNo.Text = oNominee.BirthCertificateNo;
                ddlBirthCertificateNoCountry.SelectedValue = oNominee.BirthCertificateNo_Country;
                txtBirthCertificateNo_IssueAt.Text = oNominee.BirthCertificateNo_IssueAt;
                txtTIN.Text = oNominee.TIN;
                txtPhone.Text = oNominee.Phone;
                txtEmailAddress.Text = oNominee.EmailAddress;

            }
            else if (((Button)e.CommandSource).Text.Equals("Delete"))
            {
                gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;

                string slno = ((HiddenField)gvRow.FindControl("hdNomineeSlno")).Value;

                if (!string.IsNullOrEmpty(slno))
                {
                    Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];

                    Nominee oNominee = oIssue.NomineeList.Where(n => n.SlNo.Equals(Convert.ToInt32(slno))).SingleOrDefault();

                    if (oNominee != null)
                    {
                        oIssue.NomineeList.Remove(oNominee);
                    }

                    DataTable dtNominee = new DataTable();

                    dtNominee.Columns.Add(new DataColumn("SlNo", typeof(string)));
                    dtNominee.Columns.Add(new DataColumn("NomineeName", typeof(string)));
                    dtNominee.Columns.Add(new DataColumn("Relation", typeof(string)));
                    dtNominee.Columns.Add(new DataColumn("Address", typeof(string)));
                    dtNominee.Columns.Add(new DataColumn("NomineeShare", typeof(string)));
                    dtNominee.Columns.Add(new DataColumn("Amount", typeof(string)));

                    DataRow rowNominee = null;
                    for (int i = 0; i < oIssue.NomineeList.Count; i++)
                    {
                        rowNominee = dtNominee.NewRow();

                        rowNominee["Slno"] = oIssue.NomineeList[i].SlNo;
                        rowNominee["NomineeName"] = oIssue.NomineeList[i].NomineeName;
                        rowNominee["Relation"] = oIssue.NomineeList[i].Relation;
                        rowNominee["Address"] = oIssue.NomineeList[i].Address;
                        rowNominee["NomineeShare"] = oIssue.NomineeList[i].NomineeShare;
                        rowNominee["Amount"] = oIssue.NomineeList[i].Amount;

                        dtNominee.Rows.Add(rowNominee);
                    }

                    //Reload Grid
                    gvNomDetail.DataSource = dtNominee;
                    gvNomDetail.DataBind();
                }
            }
        }

        protected void gvNomDetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //just for signature to Handle IE exception
        }

        protected void gvCustmerDetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //just for signature to Handle IE exception 
        }

        protected void txtPDConvRate_TextChanged(object sender, EventArgs e)
        {
            decimal dConvertedAmount = 0;
            
            try
            {
                if (Util.GetDecimalNumber(txtPDConvRate.Text) != 0)
                {
                    dConvertedAmount = (Util.GetDecimalNumber(txtAppliedAmount.Text) / Util.GetDecimalNumber(txtPDConvRate.Text));
                    txtPDPaymentAmount.Text = dConvertedAmount.ToString("N2");
                }
                else
                {
                    txtPDPaymentAmount.Text = string.Empty;
                }
            }
            catch (Exception)
            {                
                throw;
            }                        
        }

        protected void txtAppliedAmount_TextChanged(object sender, EventArgs e)
        {
            txtPDPaymentAmount.Text = txtAppliedAmount.Text;

            gvDenomDetail.DataSource = null;
            gvDenomDetail.DataBind();
            txtTotalAmount.Text = string.Empty;

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
            if (oIssue != null)
            {
                oIssue.NomineeList.Clear();
                oIssue.IssueDenominationList.Clear();
                Session[Constants.SES_CURRENT_ISSUE] = oIssue;
            }

            txtNDName.Text = string.Empty;
            txtNDRelation.Text = string.Empty;
            txtNDAddress.Text = string.Empty;
            txtNDParmanentAddress.Text = string.Empty;
            txtNDShare.Text = string.Empty;
            txtNDAmount.Text = string.Empty;
            txtNDateofBirth.Text = string.Empty;

            ddlGender.SelectedValue = "M";
            ddlResidenceStatus.SelectedValue = "R";
            txtNationalID.Text = string.Empty;
            txtNationalID_IssueAt.Text = string.Empty;
            txtPassportNo.Text = string.Empty;
            txtPassportNo_IssueAt.Text = string.Empty;
            txtBirthCertificateNo.Text = string.Empty;
            txtBirthCertificateNo_IssueAt.Text = string.Empty;
            txtTIN.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtEmailAddress.Text = string.Empty;

            DDListUtil.Assign(ddlResidentCountry, "BANGLADESH");
            DDListUtil.Assign(ddlPassportNoCountry, "BANGLADESH");
            DDListUtil.Assign(ddlNationalIDCountry, "BANGLADESH");
            DDListUtil.Assign(ddlBirthCertificateNoCountry, "BANGLADESH");

            btnCDAdd.Focus();
        }

        protected void txtMasterNo_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMasterNo.Text))
            {
                CustomerDetails oCustomerDetails = new CustomerDetails();
                oCustomerDetails.MasterNo = txtMasterNo.Text;
                CustomerDetailsDAL oCustDAL = new CustomerDetailsDAL();
                Result oResult = new Result();

                oResult = oCustDAL.VarifiedMasterID(oCustomerDetails);
                if (oResult.Status)
                {
                    lblMasterVarified.Text = oResult.Message;
                }
                else
                {
                    lblMasterVarified.Text = "Not Found!";
                    txtMasterNo.Text = string.Empty;
                }
            }
            else
            {
                lblMasterVarified.Text = "Not verified yet!";
            }
        }

        protected void txtPDAccDraftNo_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPDAccDraftNo.Text))
            {
                if (txtPDAccDraftNo.Text.Length < 12)
                {
                    ucMessage.OpenMessage("Account no must be 12 digit (Only Account No.)", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    txtPDAccName.Text = "";
                    return;
                }

                ///Limit Check
                IssueDAL oIssueDAL = new IssueDAL();
                if (ddlSpType.SelectedValue == "3MS" || ddlSpType.SelectedValue == "BSP")
                {
                    Result oResult = new Result();
                    if (ddlCustomerType.SelectedItem.Text.Contains("Individual"))
                    {
                        try
                        {
                            decimal.Parse(txtAppliedAmount.Text);
                            oResult = oIssueDAL.LoadCheckLimit(ddlSpType.SelectedValue, txtPDAccDraftNo.Text.Substring(0, 9), "", "", "", Convert.ToDecimal(txtAppliedAmount.Text));
                        }
                        catch (Exception)
                        {

                        }
                    }
                    else if (ddlCustomerType.SelectedItem.Text.Contains("Joint"))
                    {
                        try
                        {
                            decimal.Parse(txtAppliedAmount.Text);
                            oResult = oIssueDAL.LoadCheckLimit(ddlSpType.SelectedValue, txtPDAccDraftNo.Text.Substring(0, 9), "", "", "", Convert.ToDecimal(txtAppliedAmount.Text) / 2);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {
                        oResult.Status = true;
                    }
                    if (!oResult.Status)
                    {
                        ucMessage.OpenMessage(oResult.Message, Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        return;
                    }
                }

                CustomerDetailsDAL oCDDal = new CustomerDetailsDAL();

                string sCurrencyCode = string.Empty;
                if (!string.IsNullOrEmpty(ddlPDCurrencyCode.SelectedValue))
                {
                    sCurrencyCode = ddlPDCurrencyCode.SelectedValue;
                }
                Result oResultA = oCDDal.LoadDataFromBDDB2ByAccountNo(txtPDAccDraftNo.Text + sCurrencyCode);
                if (oResultA.Status)
                {
                    DataTable dt = (DataTable)oResultA.Return;
                    if (dt.Rows.Count != 0)
                    {
                        txtPDAccName.Text = Convert.ToString(dt.Rows[0]["AciAccName"]);
                    }
                    else
                    {
                        txtPDAccName.Text = string.Empty;
                    }
                }
                else
                {
                    txtPDAccName.Text = "";
                }
            }
            else
            {
                txtPDAccName.Text = "";
            }

        }
        
        protected void gvDenomDetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //just for signature to Handle IE exception
        }

        protected void btnViewJournals_Click(object sender, EventArgs e)
        {
            ViewJournalDAL vwJournalDal = new ViewJournalDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            Issue oIssue = Session[Constants.SES_CURRENT_ISSUE] as Issue;
            
            if (oConfig != null && oIssue != null)
            {
                SBM_BLV1.baseCommon.enmPaymentMode enmPayMode = (SBM_BLV1.baseCommon.enmPaymentMode)Enum.ToObject(typeof(SBM_BLV1.baseCommon.enmPaymentMode), oIssue.Payment.PaymentMode);
                
                int iIsApproved = 0;

                if (hdReg.Value.Equals(string.Empty))
                {
                    iIsApproved = 1;
                }
                else
                {
                    iIsApproved = 2;
                }

                oResult = vwJournalDal.ViewJournal(SBM_BLV1.baseCommon.enmActivityType.Issue, oIssue.IssueTransNo, ddlSpType.SelectedValue, txtRegistrationNo.Text, txtIssueName.Text, Convert.ToDouble(txtTotalAmount.Text), Convert.ToDouble(txtPDConvRate.Text), ddlPDCurrencyCode.SelectedValue, enmPayMode, iIsApproved, txtPDAccDraftNo.Text, 0, 0, oConfig.UserID, oConfig.BankCodeID, oConfig.DivisionID);

                if (oResult.Status)
                {
                    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                }
            }
        }

        protected void ddlPDCurrencyCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPDAccDraftNo.Text = string.Empty;
            txtPDAccName.Text = string.Empty;

            if (!string.IsNullOrEmpty(ddlPDCurrencyCode.SelectedValue))
            {                
                Issue oIssue = Session[Constants.SES_CURRENT_ISSUE] as Issue;

                if (ddlPDCurrencyCode.SelectedValue != oIssue.SPType.Currency.CurrencyID)
                {
                    //Exceptional for WDB. USD will be set as default currency.
                    if (ddlSpType.SelectedValue.Equals(Constants.SP_TYPE_WDB) && ddlPDCurrencyCode.SelectedValue.Equals("01"))
                    {                        
                        txtPDPaymentAmount.Text = (Util.GetDecimalNumber(txtAppliedAmount.Text) * Util.GetDecimalNumber(txtPDConvRate.Text)).ToString("N2");
                        txtPDConvRate.Enabled = true;
                        txtPDConvRate.Text = string.Empty;
                    }
                    else
                    {
                        txtPDConvRate.Enabled = true;
                        txtPDConvRate.Text = string.Empty;
                        txtPDPaymentAmount.Text = string.Empty;
                    }
                }
                else
                {
                    txtPDConvRate.Text = "1.00";
                    txtPDPaymentAmount.Text = (Util.GetDecimalNumber(txtAppliedAmount.Text) * Util.GetDecimalNumber(txtPDConvRate.Text)).ToString("N2");
                    txtPDConvRate.Enabled = false;
                }

                txtPDAccDraftNo.Focus();
            }
            else
            {                
                txtPDConvRate.Enabled = true;
                txtPDConvRate.Text = string.Empty;
                txtPDPaymentAmount.Text = string.Empty;
                ddlPDCurrencyCode.Focus();
            }            
        }
    }
}