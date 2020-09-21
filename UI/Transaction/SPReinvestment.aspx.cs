using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.Transaction;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Configuration;
using SBM_BLC1.Entity.Transaction;
using System.Collections;
using System.Globalization;

namespace SBM_WebUI.mp
{
    public partial class SPReinvestment : System.Web.UI.Page
    {
        #region Local Variable
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_REG_NO = "sRegNo";
        public const string OBJ_PAGE_ID = "sPageID";
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
                    //This is for Page Permission
                    CheckPermission chkPer = new CheckPermission();
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_TRANS.REINVESTMENT))
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
            // Issue set in session 
            if (Session[Constants.SES_REINVESTMENT] == null)
            {
                Reinvestment oReinvestment = new Reinvestment();
                Session.Add(Constants.SES_REINVESTMENT, oReinvestment);
            }
            else
            {
                Reinvestment oReinvestment = new Reinvestment();
                Session[Constants.SES_REINVESTMENT] = oReinvestment;
            }

            gvData.DataSource = null;
            gvData.DataBind();

            

            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            DDListUtil.LoadDDLFromDB(ddlBranch, "BranchID", "BranchName", "SPMS_Branch", true);
          

            //txtNWDAmount.Text = "1";//Default value

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
                   
                    SEARCH_FROM = 3;
                    LoadDataByRegNo(sRegNo, "1"); //query from Tmp
                   
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
                // user Detail
                #region User-Detail.
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                UserDetails oUserDetails = new UserDetails();
                oUserDetails.MakerID = oConfig.UserName;
                oUserDetails.MakeDate = DateTime.Now;
                ucUserDet.UserDetail = oUserDetails;
                #endregion User-Detail.

                fsList.Visible = true;
                LoadPreviousList();
            }

            txtOldRegNo.Focus();
        }
        #endregion InitializeData

        protected void btnReset_Click(object sender, EventArgs e)
        {
            TotalClear();
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

        protected void btnHistoryDetail_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdIssueTransNo.Value))
            {
                ReinvestmentDAL oRDAL = new ReinvestmentDAL();
                Result oResult = null;
                oResult = (Result)oRDAL.GetHistoryData(hdReinvestRefNo.Value);
                if (oResult.Status)
                {
                    VGD.SetData((DataTable)oResult.Return, "History Detail");
                }
            }
            else
            {
                VGD.SetData(null, "History Detail");
            }
        }        
        
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdIssueTransNo.Value))
            {
                Reinvestment oReiv = (Reinvestment)Session[Constants.SES_REINVESTMENT];
                oReiv.ReinvestmentRefNo = hdReinvestRefNo.Value;
                oReiv.OldIssueTransNo = hdIssueTransNo.Value;
                oReiv.MaturityDate = Util.GetDateTimeByString(txtMaturityDate.Text);

                ReinvestmentDAL oReivDAL = new ReinvestmentDAL();
                oReiv.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oReivDAL.Approve(oReiv);
                if (oResult.Status)
                {
                    if (oResult.Message.Equals("E"))
                    {
                        ucMessage.OpenMessage("Denomination of <b>" + (string)oResult.Return + "</b> not available. Cannot be reinvested!", Constants.MSG_TYPE_INFO);
                    }
                    else
                    {
                        ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE + " with Reg. No.: <b>" + (string)oResult.Return + "</b>", Constants.MSG_TYPE_SUCCESS);
                    }
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

        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");

            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.REINVESTMENT).PadLeft(5, '0'), false);
            }
            else
            {
                txtOldRegNo.Focus();
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.REINVESTMENT).PadLeft(5, '0'), false);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdIssueTransNo.Value))
            {
                Reinvestment oReiv = (Reinvestment)Session[Constants.SES_REINVESTMENT];
                oReiv.ReinvestmentRefNo = hdReinvestRefNo.Value;
                oReiv.OldIssueTransNo = hdIssueTransNo.Value;

                ReinvestmentDAL oReivDAL = new ReinvestmentDAL();
                oReiv.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oReivDAL.Reject(oReiv);
                if (oResult.Status)
                {
                    //if (oResult.Message.Equals("E"))
                    //{
                    //    ucMessage.OpenMessage("Required Script is not available now", Constants.MSG_TYPE_ERROR);
                    //}
                    //else
                    //{

                        ucMessage.OpenMessage(Constants.MSG_SUCCESS_REJECT, Constants.MSG_TYPE_SUCCESS);
                    //}
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

        private void TotalClear()
        {
            EnableDisableControl(false);
            // Issue Detail
            txtRefNo.Text = string.Empty;
            txtNewRegNo.Text = string.Empty;
            txtOldRegNo.Text = string.Empty;
            txtIssueDate.Text = string.Empty;
            txtReinvestmentDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtMaturityDate.Text = string.Empty;
            ddlSpType.SelectedIndex = 0;
            ddlBranch.SelectedIndex = 0;
            ddlCustomerType.Items.Clear();

            // tab
            gvCustmerDetail.DataSource = null;
            gvCustmerDetail.DataBind();
            //gvNomDemon.DataSource = null;
            //gvNomDemon.DataBind();
            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();
            txtNDName.Text = string.Empty;
            txtNDRelation.Text = string.Empty;
            txtNDBirthDate.Text = string.Empty;
            txtNDAddress.Text = string.Empty;
            txtNDShare.Text = string.Empty;
            txtNDAmount.Text = string.Empty;

            // Bond Holder Details
            txtBHDHolderName.Text = string.Empty;
            txtIssueName.Text = string.Empty;
            txtBHDAddress.Text = string.Empty;
            txtBHDRelation.Text = string.Empty;
            txtMasterNo.Text = string.Empty;

            // Scrip Detail
            gvCertDetail.DataSource = null;
            gvCertDetail.DataBind();
            txtTotalAmount.Text = string.Empty;
            txtSelectedAmount.Text = string.Empty;
            ddlDenomination.Items.Clear();
            txtQuantity.Text = string.Empty;
            gvDenDetail.DataSource = null;
            gvDenDetail.DataBind();
           
            ucUserDet.ResetData();
            Session[Constants.SES_REINVESTMENT] = null;
            Session["SPPolicy"] = null;
            txtIssueName.Attributes["M"] = null;
        }

        public void ReinvestmentSearchLoadAction(GridViewRow gvRow, string sApprovalType)
        {
            if (gvRow != null)
            {                
                hdDataType.Value = sApprovalType;

                if (sApprovalType.Equals("2"))
                {
                    LoadDataByRegNo(gvRow.Cells[3].Text, sApprovalType);
                    txtNewRegNo.Text = gvRow.Cells[3].Text;
                }
                else
                {
                    LoadDataByRegNo(gvRow.Cells[2].Text, sApprovalType);
                }
            }
        }

        private void EnableDisableControl(bool isApproved)
        {          
            if (isApproved)
            {
                Util.ControlEnabled(txtOldRegNo, false);
                
                //Nominee details
                Util.ControlEnabled(txtNDName, false);
                Util.ControlEnabled(txtNDRelation, false);
                Util.ControlEnabled(txtNDShare, false);
                Util.ControlEnabled(txtNDAddress, false);
                Util.ControlEnabled(txtNDBirthDate, false);
                Util.ControlEnabled(gvNomDetail, false);

                //Nominee Wise Denomination Details
                //Util.ControlEnabled(ddlNWDName, false);
                //Util.ControlEnabled(ddlNWDDenom, false);
                //Util.ControlEnabled(txtNWDQuantity, false);
                //Util.ControlEnabled(gvNomDemon, false);


                //Bond Holders Detais
                Util.ControlEnabled(txtMasterNo, false);
                Util.ControlEnabled(txtBHDHolderName, false);
                Util.ControlEnabled(txtBHDAddress, false);
                Util.ControlEnabled(txtBHDRelation, false);
                
                //Scrip Details
                Util.ControlEnabled(ddlDenomination, false);
                Util.ControlEnabled(txtQuantity, false);
                Util.ControlEnabled(gvDenDetail, false);

                //Buttons
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true);
                Util.ControlEnabled(btnReject, true);

                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnDelete, false);

                btnSearchOldRegNo.Enabled = false;
                btnNewRegNo.Enabled = false;
                btnRefNo.Enabled = false;

                btnNDAdd.Enabled = false;
                btnNDReset.Enabled = false;
                //btnNWDAdd.Enabled = false;
                //btnNWDDelete.Enabled = false;
                btnDenomAdd.Enabled = false;

                fsList.Visible = false;
            }
            else
            {
                Util.ControlEnabled(txtOldRegNo, true);
                
                //Nominee details
                Util.ControlEnabled(txtNDName, true);
                Util.ControlEnabled(txtNDRelation, true);
                Util.ControlEnabled(txtNDShare, true);
                Util.ControlEnabled(txtNDAddress, true);
                Util.ControlEnabled(txtNDBirthDate, true);
                Util.ControlEnabled(gvNomDetail, true);

                //Nominee Wise Denomination Details
                //Util.ControlEnabled(ddlNWDName, true);
                //Util.ControlEnabled(ddlNWDDenom, true);
                //Util.ControlEnabled(txtNWDQuantity, true);
                //Util.ControlEnabled(gvNomDemon, true);


                //Bond Holders Detais
                Util.ControlEnabled(txtMasterNo, true);

                //Scrip Details
                Util.ControlEnabled(ddlDenomination, true);
                Util.ControlEnabled(txtQuantity, true);
                Util.ControlEnabled(gvDenDetail, true);

                //Buttons
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);
                Util.ControlEnabled(btnReject, false);

                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnDelete, true);

                btnSearchOldRegNo.Enabled = true;
                btnNewRegNo.Enabled = true;
                btnRefNo.Enabled = true;

                btnNDAdd.Enabled = true;
                btnNDReset.Enabled = true;
                //btnNWDAdd.Enabled = true;
                //btnNWDDelete.Enabled = true;
                btnDenomAdd.Enabled = true;

                fsList.Visible = true;
            }
        }
    
        private void LoadDataByRegNo(string sRegNo, string sApprovalStatus)
        {
            ReinvestmentDAL oReinvestmentDAL = new ReinvestmentDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = (Result)oReinvestmentDAL.LoadDataByRegistrationNo(sRegNo, sApprovalStatus, oConfig.DivisionID, oConfig.BankCodeID);
            TotalClear();

            if (oResult.Status)
            {
                Reinvestment oReinvestment = (Reinvestment)oResult.Return;
                if (!(oReinvestment.Issue.SPType.SPTypeID.Trim().ToUpper().Contains(Constants.SP_TYPE_WDB)
                || oReinvestment.Issue.SPType.SPTypeID.Trim().ToUpper().Contains(Constants.SP_TYPE_DIB)
                || oReinvestment.Issue.SPType.SPTypeID.Trim().ToUpper().Contains(Constants.SP_TYPE_DPB)))
                {
                    ucMessage.OpenMessage("Only Bonds can be reinvested.!", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    return;
                }
                SetObject(oReinvestment);
                if (hdDataType.Value.Equals("2"))
                {
                    EnableDisableControl(true);
                    Util.ControlEnabled(btnReject, false);
                    Util.ControlEnabled(btnApprove, false);
                    Util.ControlEnabled(btnBack, false);

                    Util.ControlEnabled(btnReset, true);
                    Util.ControlEnabled(btnSave, true);
                    Util.ControlEnabled(btnDelete, true);

                    btnSearchOldRegNo.Enabled = true;
                    btnNewRegNo.Enabled = true;
                    btnRefNo.Enabled = true;

                    fsList.Visible = true;
                }
                else if (SEARCH_FROM.Equals(3))
                {
                    EnableDisableControl(true);
                    Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);
                }
                else if (SEARCH_FROM.Equals(2) || hdDataType.Value.Equals("1"))
                {
                    EnableDisableControl(false);
                }

                if (SEARCH_FROM.Equals(2))
                {
                    StockInfoLoad(ddlSpType.SelectedValue);
                }
                //CalculateEncashment();
            }
            else
            {
                if (oResult.Message.Equals("L"))
                {
                    ucMessage.OpenMessage("Scrips are Liended!!", Constants.MSG_TYPE_INFO);
                }
                else if (oResult.Message.Equals("S"))
                {
                    ucMessage.OpenMessage("Scrips are Stopped!!", Constants.MSG_TYPE_INFO);
                }
                else if (oResult.Message.Equals("R"))
                {
                    ucMessage.OpenMessage("Partial reinvestment not allowed!!", Constants.MSG_TYPE_INFO);
                }
                else if (oResult.Message.Equals("N"))
                {
                    ucMessage.OpenMessage("Registration details not found for reinvestment!!", Constants.MSG_TYPE_INFO);
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_INFO);
                }

                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
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

        protected void gvNomDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvNomDetail, null);
        }

        private void SetObject(Reinvestment oReinvestment)
        {
            if (oReinvestment != null)
            {
                if (oReinvestment.Issue != null)
                {
                    if (oReinvestment.Issue.ScripList.Count <= 0)//((int)Constants.ISSUE_STATUS.ISSUED))
                    {
                        ucMessage.OpenMessage("No Certificates found to reinvest! Please check...", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    }                 
                    else
                    {
                        Session[Constants.SES_REINVESTMENT] = oReinvestment;

                        hdReinvestRefNo.Value = oReinvestment.ReinvestmentRefNo;
                        hdIssueTransNo.Value = oReinvestment.Issue.IssueTransNo;
                        hdRegNo.Value = oReinvestment.Issue.RegNo;

                        txtRefNo.Text = oReinvestment.ReinvestmentRefNo;
                        txtOldRegNo.Text = oReinvestment.Issue.RegNo;
                        txtNewRegNo.Text = oReinvestment.NewRegNo;

                        ddlSpType.Text = oReinvestment.Issue.SPType.SPTypeID.Trim();
                        ddlBranch.Text = oReinvestment.Issue.Branch.BranchID.Trim();

                        txtIssueDate.Text = oReinvestment.Issue.VersionIssueDate.ToString(Constants.DATETIME_FORMAT);
                        txtIssueName.Text = oReinvestment.Issue.IssueName;

                        int iDuration = oReinvestment.Issue.VersionSPPolicy.SPDuration;
                        bool bInMonth = oReinvestment.Issue.VersionSPPolicy.IsSPDurationInMonth;
                        DateTime dtMaturity = DateTime.Now;
                        dtMaturity = oReinvestment.Issue.VersionIssueDate;
                        if (bInMonth)
                        {
                            dtMaturity = dtMaturity.AddMonths(iDuration);
                        }
                        else
                        {
                            dtMaturity = dtMaturity.AddYears(iDuration);
                        }
                        txtMaturityDate.Text = dtMaturity.ToString(Constants.DATETIME_FORMAT);
                        txtTotalAmount.Text = oReinvestment.Issue.ScripList.Sum(s => s.Denomination.DenominationID).ToString();
                        txtSelectedAmount.Text = oReinvestment.ReinvestAmount.ToString();
                        
                        txtReinvestmentDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);                     

                        #region Customer Details
                        DataTable dtCustomerDetails = new DataTable();

                        dtCustomerDetails.Columns.Add(new DataColumn("CustomerID", typeof(string)));
                        dtCustomerDetails.Columns.Add(new DataColumn("Customer Name", typeof(string)));
                        dtCustomerDetails.Columns.Add(new DataColumn("Address", typeof(string)));
                        dtCustomerDetails.Columns.Add(new DataColumn("Phone", typeof(string)));

                        DataRow rowCustomerDetails = null;

                        for (int customerCount = 0; customerCount < oReinvestment.Issue.CustomerDetailsList.Count; customerCount++)
                        {
                            rowCustomerDetails = dtCustomerDetails.NewRow();
                            rowCustomerDetails["CustomerID"] = oReinvestment.Issue.CustomerDetailsList[customerCount].CustomerID;
                            rowCustomerDetails["Customer Name"] = oReinvestment.Issue.CustomerDetailsList[customerCount].CustomerName;                            
                            rowCustomerDetails["Address"] = oReinvestment.Issue.CustomerDetailsList[customerCount].Address;
                            rowCustomerDetails["Phone"] = oReinvestment.Issue.CustomerDetailsList[customerCount].Phone;

                            dtCustomerDetails.Rows.Add(rowCustomerDetails);
                        }

                        gvCustmerDetail.DataSource = dtCustomerDetails;
                        gvCustmerDetail.DataBind();
                        #endregion

                        #region Nominee Detail
                        DataTable dtNomineeDetail = new DataTable();

                        dtNomineeDetail.Columns.Add(new DataColumn("SlNo", typeof(string)));
                        dtNomineeDetail.Columns.Add(new DataColumn("Nominee Name", typeof(string)));
                        dtNomineeDetail.Columns.Add(new DataColumn("Relation", typeof(string)));
                        dtNomineeDetail.Columns.Add(new DataColumn("Address", typeof(string)));
                        dtNomineeDetail.Columns.Add(new DataColumn("Nominee Share", typeof(string)));
                        dtNomineeDetail.Columns.Add(new DataColumn("Amount", typeof(string)));
                        dtNomineeDetail.Columns.Add(new DataColumn("DateOfBirth", typeof(DateTime)));

                        DataRow rowNomineeDetail = null;

                        for (int nomineeCount = 0; nomineeCount < oReinvestment.Issue.NomineeList.Count; nomineeCount++)
                        {
                            rowNomineeDetail = dtNomineeDetail.NewRow();

                            rowNomineeDetail["Slno"] = oReinvestment.Issue.NomineeList[nomineeCount].SlNo;
                            rowNomineeDetail["Nominee Name"] = oReinvestment.Issue.NomineeList[nomineeCount].NomineeName;
                            rowNomineeDetail["Relation"] = oReinvestment.Issue.NomineeList[nomineeCount].Relation;
                            rowNomineeDetail["Address"] = oReinvestment.Issue.NomineeList[nomineeCount].Address;
                            rowNomineeDetail["Nominee Share"] = oReinvestment.Issue.NomineeList[nomineeCount].NomineeShare;
                            if (oReinvestment.Issue.NomineeList[nomineeCount].Amount <= 0)
                            {
                                if (oReinvestment.Issue.NomineeList[nomineeCount].NomineeShare > 0)
                                {
                                    rowNomineeDetail["Amount"] = oReinvestment.Issue.IssueAmount * (oReinvestment.Issue.NomineeList[nomineeCount].NomineeShare / 100);
                                }
                            }
                            else
                            {
                                rowNomineeDetail["Amount"] = oReinvestment.Issue.NomineeList[nomineeCount].Amount;
                            }
                            if (Date.GetDateTimeByString(oReinvestment.Issue.NomineeList[nomineeCount].DateOfBirth.ToString()).Year > 1900)
                            {
                                rowNomineeDetail["DateOfBirth"] = Date.GetDateTimeByString(oReinvestment.Issue.NomineeList[nomineeCount].DateOfBirth.ToString());
                            }
                            dtNomineeDetail.Rows.Add(rowNomineeDetail);
                        }
                        gvNomDetail.DataSource = dtNomineeDetail;
                        gvNomDetail.DataBind();

                        #endregion

                        DataTable dtSripTable = new DataTable();

                        dtSripTable.Columns.Add(new DataColumn("Denomination", typeof(string)));
                        dtSripTable.Columns.Add(new DataColumn("Certificate No", typeof(string)));
                        DataRow rowScrip = null;

                        ArrayList alScrip = new ArrayList();
                        
                        for (int iScripCount = 0; iScripCount < oReinvestment.Issue.ScripList.Count; iScripCount++)
                        {
                            if (!alScrip.Contains(oReinvestment.Issue.ScripList[iScripCount].SPScripID))
                            {
                                alScrip.Add(oReinvestment.Issue.ScripList[iScripCount].SPScripID);

                                rowScrip = dtSripTable.NewRow();

                                rowScrip["Denomination"] = oReinvestment.Issue.ScripList[iScripCount].Denomination.DenominationID;
                                rowScrip["Certificate No"] = oReinvestment.Issue.ScripList[iScripCount].SPSeries + " " + oReinvestment.Issue.ScripList[iScripCount].SlNo;

                                dtSripTable.Rows.Add(rowScrip);
                            }                            
                        }
                        dtSripTable.DefaultView.Sort = "Certificate No ASC"; 

                        gvCertDetail.DataSource = dtSripTable;
                        gvCertDetail.DataBind();

                        gvDenDetail.DataSource = oReinvestment.DtReinvestmentDetail;
                        gvDenDetail.DataBind();
                                                                        
                        LoadBySPType();

                        //#region LoadByPolicy
                        //SPPolicy oSPPolicy = oReinvestment.Issue.VersionSPPolicy;
                        //DDListUtil.Assign(ddlCustomerType, oSPPolicy.DTCustomerTypePolicy, true);
                        //DDListUtil.Assign(ddlCustomerType, oReinvestment.Issue.CustomerType.CustomerTypeID);

                        //DataTable dtDenom = new DataTable();
                        //if (oSPPolicy.SPType.ListOfDenomination.Denomination.Count > 0)
                        //{
                        //    dtDenom.Columns.Add(new DataColumn("Text", typeof(string)));
                        //    dtDenom.Columns.Add(new DataColumn("Value", typeof(string)));

                        //    DataRow rowDenom = null;
                        //    for (int i = 0; i < oSPPolicy.SPType.ListOfDenomination.Denomination.Count; i++)
                        //    {
                        //        rowDenom = dtDenom.NewRow();

                        //        rowDenom["Text"] = oSPPolicy.SPType.ListOfDenomination.Denomination[i].DenominationID.ToString();
                        //        rowDenom["Value"] = oSPPolicy.SPType.ListOfDenomination.Denomination[i].Series.ToString();
                        //        dtDenom.Rows.Add(rowDenom);
                        //    }
                        //}
                        //DDListUtil.Assign(ddlDenomination, dtDenom, true);
                        //#endregion

                       
                    }                    

                    UserDetails ooUserDetails = ucUserDet.UserDetail;
                    if (hdDataType.Value.Equals("2"))
                    {
                        ooUserDetails.MakerID = oReinvestment.UserDetails.MakerID;
                        ooUserDetails.MakeDate = oReinvestment.UserDetails.MakeDate;
                        ooUserDetails.CheckDate = oReinvestment.UserDetails.CheckDate;
                        ooUserDetails.CheckerID = oReinvestment.UserDetails.CheckerID;
                        ooUserDetails.CheckerComment = oReinvestment.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = ooUserDetails;
                        txtReinvestmentDate.Text = oReinvestment.ReinvestDate.ToString(Constants.DATETIME_FORMAT);
                    }
                    else if (SEARCH_FROM.Equals(2) || hdDataType.Value.Equals("1"))
                    {                        
                        ooUserDetails.CheckDate = oReinvestment.UserDetails.CheckDate;
                        ooUserDetails.CheckerID = oReinvestment.UserDetails.CheckerID;
                        ooUserDetails.CheckerComment = oReinvestment.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = ooUserDetails;
                        
                    }
                    else if (SEARCH_FROM.Equals(3))
                    {
                        ooUserDetails.MakerID = oReinvestment.UserDetails.MakerID;
                        ooUserDetails.CheckerComment = oReinvestment.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = ooUserDetails;
                        
                    }
                }
            }
        }

        private void LoadBySPType()
        {
            Reinvestment oReinvestment = (Reinvestment)Session[Constants.SES_REINVESTMENT];
            SPPolicy oSPPolicy = oReinvestment.Issue.VersionSPPolicy;
            SPPolicy oSPPolicyNew = Session["SPPolicy"] as SPPolicy;
            if (oSPPolicyNew == null)
            {
                SPPolicyDAL oSPPolicyDAL = new SPPolicyDAL();
                Result oResult = (Result)oSPPolicyDAL.LatestPolicy(ddlSpType.SelectedValue, Constants.ACTIVITY_TYPE.ISSUE, Util.GetDateTimeByString(txtMaturityDate.Text));

                if (oResult.Status)
                {
                    oSPPolicyNew = (SPPolicy)oResult.Return;
                    //oReinvestment.Issue.VersionSPPolicy = oSPPolicy;
                    Session["SPPolicy"] = oSPPolicyNew;
                }
            }
            //if (oResult.Status)
            {
                ddlCustomerType.Items.Clear();                
                ddlDenomination.Items.Clear();

                //oSPPolicy = (SPPolicy)oResult.Return;
                DDListUtil.Assign(ddlCustomerType, oSPPolicy.DTCustomerTypePolicy, true);
                if (oReinvestment != null)
                {
                    DDListUtil.Assign(ddlCustomerType, oReinvestment.Issue.CustomerType.CustomerTypeID);
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
                DDListUtil.Assign(ddlDenomination, dtDenom, true);
                                
                if (oSPPolicy.IsBondHolderRequired)
                {                    
                    //ddlNWDDenom.Enabled = true;
                    //ddlNWDName.Enabled = true;
                    txtBHDHolderName.Enabled = true;
                    txtBHDAddress.Enabled = true;
                    txtBHDRelation.Enabled = true;

                    txtBHDHolderName.Text = oReinvestment.Issue.BondHolderName;
                    txtBHDAddress.Text = oReinvestment.Issue.BondHolderAddress;
                    txtBHDRelation.Text = oReinvestment.Issue.BondHolderRelation;
                }
                else
                {                    
                    //ddlNWDDenom.Enabled = false;
                    //ddlNWDName.Enabled = false;
                    txtBHDHolderName.Enabled = false;
                    txtBHDAddress.Enabled = false;
                    txtBHDRelation.Enabled = false;
                }                
            }
        }

        protected void btnNDAdd_Click(object sender, EventArgs e)
        {
            Reinvestment oReinvestment = (Reinvestment)Session[Constants.SES_REINVESTMENT];
            decimal dCount = 0;

            if (oReinvestment != null)
            {
                if (oReinvestment.Issue.NomineeList.Count > 0)
                {
                    for (int i = 0; i < oReinvestment.Issue.NomineeList.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(hdNomSlno.Value))
                        {
                            int j = Util.GetIntNumber(hdNomSlno.Value);
                            if (j - 1 != i)
                            {
                                dCount += oReinvestment.Issue.NomineeList[i].NomineeShare;
                            }
                        }
                        else if (oReinvestment.Issue.NomineeList[i].NomineeShare > 0)
                        {
                            dCount += oReinvestment.Issue.NomineeList[i].NomineeShare;
                        }
                    }
                }
                if (dCount + Util.GetDecimalNumber(txtNDShare.Text) <= 100)
                {
                    AddNomineeToSession(oReinvestment);
                    
                    txtNDName.Text = string.Empty;
                    txtNDRelation.Text = string.Empty;
                    txtNDBirthDate.Text = string.Empty;
                    txtNDAddress.Text = string.Empty;
                    txtNDShare.Text = string.Empty;
                    txtNDAmount.Text = string.Empty;
                }
                else
                {
                    ucMessage.OpenMessage("Total amount of share cannot be exceeded 100", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
            else
            {
                AddNomineeToSession(new Reinvestment());

                txtNDName.Text = string.Empty;
                txtNDRelation.Text = string.Empty;
                txtNDBirthDate.Text = string.Empty;
                txtNDAddress.Text = string.Empty;
                txtNDShare.Text = string.Empty;
                txtNDAmount.Text = string.Empty;
            }
            hdNomSlno.Value = "";
        }

        private void AddNomineeToSession(Reinvestment oReinvestment)
        {
            //Nominee Details
            Nominee oNominee = null;
            bool isToEdit = false;
            int editIndex = 0;

            if (!string.IsNullOrEmpty(hdNomSlno.Value))
            {
                oNominee = oReinvestment.Issue.NomineeList.Where(n => n.SlNo.Equals(Convert.ToInt32(hdNomSlno.Value))).SingleOrDefault();
                if (oNominee != null)
                {
                    oNominee.SlNo = Convert.ToInt32(hdNomSlno.Value);
                    editIndex = oReinvestment.Issue.NomineeList.FindIndex(n => n.SlNo.Equals(Convert.ToInt32(hdNomSlno.Value)));
                    isToEdit = true;
                }
                else
                {
                    oNominee = new Nominee();
                    oNominee.SlNo = oReinvestment.Issue.NomineeList.Count + 1;
                }
            }
            else
            {
                oNominee = new Nominee();
                oNominee.SlNo = oReinvestment.Issue.NomineeList.Count + 1;
            }
            oNominee.Address = txtNDAddress.Text;
            oNominee.Amount = Util.GetDecimalNumber(txtNDAmount.Text);
            oNominee.IssueTransNo = Convert.ToString(1);
            oNominee.NomineeName = txtNDName.Text;
            oNominee.NomineeShare = Util.GetDecimalNumber(txtNDShare.Text);
            oNominee.Relation = txtNDRelation.Text;
            oNominee.DateOfBirth = Util.GetDateTimeByString(txtNDBirthDate.Text);
            oNominee.UserDetails = ucUserDet.UserDetail;
            //Add Nominee
            if (!isToEdit)
            {
                oReinvestment.Issue.NomineeList.Add(oNominee);
            }
            else // Edit Nominee
            {
                oReinvestment.Issue.NomineeList[editIndex] = oNominee;
            }

            DataTable dtNominee = new DataTable();

            dtNominee.Columns.Add(new DataColumn("SlNo", typeof(string)));
            dtNominee.Columns.Add(new DataColumn("Nominee Name", typeof(string)));
            dtNominee.Columns.Add(new DataColumn("Relation", typeof(string)));
            dtNominee.Columns.Add(new DataColumn("Address", typeof(string)));
            dtNominee.Columns.Add(new DataColumn("Nominee Share", typeof(string)));
            dtNominee.Columns.Add(new DataColumn("Amount", typeof(string)));
            dtNominee.Columns.Add(new DataColumn("DateOfBirth", typeof(DateTime)));

            DataRow rowNominee = null;
            for (int i = 0; i < oReinvestment.Issue.NomineeList.Count; i++)
            {
                rowNominee = dtNominee.NewRow();

                rowNominee["Slno"] = oReinvestment.Issue.NomineeList[i].SlNo;
                rowNominee["Nominee Name"] = oReinvestment.Issue.NomineeList[i].NomineeName;
                rowNominee["Relation"] = oReinvestment.Issue.NomineeList[i].Relation;
                rowNominee["Address"] = oReinvestment.Issue.NomineeList[i].Address;
                rowNominee["Nominee Share"] = oReinvestment.Issue.NomineeList[i].NomineeShare;
                rowNominee["Amount"] = oReinvestment.Issue.NomineeList[i].Amount;
                if (oReinvestment.Issue.NomineeList[i].DateOfBirth.Year > 1900)
                {
                    rowNominee["DateOfBirth"] = Date.GetDateTimeByString(oReinvestment.Issue.NomineeList[i].DateOfBirth.ToString());
                }
                
                dtNominee.Rows.Add(rowNominee);
            }

            //Reload Grid
            gvNomDetail.DataSource = dtNominee;
            gvNomDetail.DataBind();
            //Update Session
            Session[Constants.SES_REINVESTMENT] = oReinvestment;
        }

        protected void gvCustomerDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            Reinvestment oReinv = Session[Constants.SES_REINVESTMENT] as Reinvestment;

            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;

            if (((Button)e.CommandSource).Text.Equals("Select"))
            {
                CustomerDetails oCustDetl = oReinv.Issue.CustomerDetailsList.Where(cs => cs.CustomerID.Equals(Convert.ToInt32(gvRow.Cells[1].Text))).SingleOrDefault();
                //txtIssueName.Attributes["M"]
                //used for identifying whether any Modification(M)
                // occurs in CustomeDetailPage
                if (oCustDetl != null && (txtIssueName.Attributes["M"] != null && txtIssueName.Attributes["M"].Equals("M")))
                {
                    oCustDetl.isViewOnly = true;
                    if (!txtOldRegNo.Enabled)
                    {
                        oCustDetl.isReinvestmet = false;
                    }
                    else
                    {
                        oCustDetl.isReinvestmet = true;
                    }
                    CustomerDetail.SetCustomerDetails(oCustDetl);
                }
                else
                {
                    CustomerDetailsDAL oCustDetlDAL = new CustomerDetailsDAL();

                    oCustDetl = new CustomerDetails();
                    oCustDetl.CustomerID = Convert.ToInt32(gvRow.Cells[1].Text);
                    oCustDetl.isViewOnly = true;
                    if (!txtOldRegNo.Enabled)
                    {
                        oCustDetl.isReinvestmet = false;
                    }
                    else
                    {
                        oCustDetl.isReinvestmet = true;
                    }
                    Result oResult = oCustDetlDAL.LoadByID(oCustDetl);
                    if (oResult.Status)
                    {
                        CustomerDetail.SetCustomerDetails(oResult.Return as CustomerDetails);
                    }
                }
            }
        }

        public void CustomerDetailAction(CustomerDetails oCustomer)
        {
            Reinvestment oReinv = Session[Constants.SES_REINVESTMENT] as Reinvestment;
            Issue oIssue = oReinv.Issue;
            
            if (oIssue == null)
            {
                oIssue = new Issue();
            }
            
            CustomerDetails oCustDetlExist = oIssue.CustomerDetailsList.Where(c => c.CustomerID.Equals(oCustomer.CustomerID)).SingleOrDefault();
            if (oCustDetlExist != null)
            {
                oIssue.CustomerDetailsList.Remove(oCustDetlExist);
            }
                        
            oIssue.CustomerDetailsList.Add(oCustomer);            

            DataTable dtCustomerDetails = new DataTable();

            dtCustomerDetails.Columns.Add(new DataColumn("CustomerID", typeof(string)));
            dtCustomerDetails.Columns.Add(new DataColumn("Customer Name", typeof(string)));
            dtCustomerDetails.Columns.Add(new DataColumn("Address", typeof(string)));
            dtCustomerDetails.Columns.Add(new DataColumn("Phone", typeof(string)));


            DataRow rowCustomerDetails = null;
            string issueName = string.Empty;

            for (int i = 0; i < oIssue.CustomerDetailsList.Count; i++)
            {
                rowCustomerDetails = dtCustomerDetails.NewRow();

                rowCustomerDetails["CustomerID"] = oIssue.CustomerDetailsList[i].CustomerID == -1 ? "New Customer" : oIssue.CustomerDetailsList[i].CustomerID.ToString();// oIssue.CustomerDetailsList[i].CustomerID;
                rowCustomerDetails["Customer Name"] = oIssue.CustomerDetailsList[i].CustomerName;
                rowCustomerDetails["Address"] = oIssue.CustomerDetailsList[i].Address;
                rowCustomerDetails["Phone"] = oIssue.CustomerDetailsList[i].Phone;

                dtCustomerDetails.Rows.Add(rowCustomerDetails);
            }

            gvCustmerDetail.DataSource = dtCustomerDetails;
            gvCustmerDetail.DataBind();

            oReinv.Issue = oIssue;

            //Save & Load button clicked in CustomerDetail
            txtIssueName.Attributes["M"] = "M";

            if (txtBHDHolderName.Enabled)
            {
                txtBHDHolderName.Text = oIssue.CustomerDetailsList[0].CustomerName;
                txtBHDRelation.Text = "SELF";
                txtBHDAddress.Text = oIssue.CustomerDetailsList[0].Address;
            }

            Session[Constants.SES_REINVESTMENT] = oReinv;
        }

        protected void gvNomDetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //just for signature to Handle IE exception
        }
        protected void gvNomDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = null;

            if (((Button)e.CommandSource).Text.Equals("Select"))
            {
                gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;

                txtNDName.Text = gvRow.Cells[2].Text;
                txtNDRelation.Text = gvRow.Cells[3].Text;
                txtNDAddress.Text = gvRow.Cells[4].Text;
                txtNDShare.Text = gvRow.Cells[5].Text;
                txtNDAmount.Text = gvRow.Cells[6].Text;
                DateTime parsedDate;
                DateTime.TryParseExact(gvRow.Cells[7].Text, Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                if (parsedDate.Year > 1900)
                {
                    txtNDBirthDate.Text = parsedDate.ToString(Constants.DATETIME_FORMAT);
                }
                hdNomSlno.Value = ((HiddenField)gvRow.FindControl("hdNomineeSlno")).Value;
            }
            else if (((Button)e.CommandSource).Text.Equals("Delete"))
            {
                gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;

                string slno = ((HiddenField)gvRow.FindControl("hdNomineeSlno")).Value;

                if (!string.IsNullOrEmpty(slno))
                {
                    Reinvestment oReinvestment = (Reinvestment)Session[Constants.SES_REINVESTMENT];
                    Issue oIssue = oReinvestment.Issue;
                    Nominee oNominee = null;
                    if (oIssue != null)
                    {
                        oNominee = oIssue.NomineeList.Where(n => n.SlNo.Equals(Convert.ToInt32(slno))).SingleOrDefault();
                    }

                    if (oNominee != null)
                    {
                        oIssue.NomineeList.Remove(oNominee);
                    }

                    DataTable dtNominee = new DataTable();

                    dtNominee.Columns.Add(new DataColumn("SlNo", typeof(string)));
                    dtNominee.Columns.Add(new DataColumn("Nominee Name", typeof(string)));
                    dtNominee.Columns.Add(new DataColumn("Relation", typeof(string)));
                    dtNominee.Columns.Add(new DataColumn("Address", typeof(string)));
                    dtNominee.Columns.Add(new DataColumn("Nominee Share", typeof(string)));
                    dtNominee.Columns.Add(new DataColumn("Amount", typeof(string)));
                    dtNominee.Columns.Add(new DataColumn("DateOfBirth", typeof(DateTime)));

                    DataRow rowNominee = null;
                    for (int i = 0; i < oIssue.NomineeList.Count; i++)
                    {
                        rowNominee = dtNominee.NewRow();

                        rowNominee["Slno"] = oIssue.NomineeList[i].SlNo;
                        rowNominee["Nominee Name"] = oIssue.NomineeList[i].NomineeName;
                        rowNominee["Relation"] = oIssue.NomineeList[i].Relation;
                        rowNominee["Address"] = oIssue.NomineeList[i].Address;
                        rowNominee["Nominee Share"] = oIssue.NomineeList[i].NomineeShare;
                        rowNominee["Amount"] = oIssue.NomineeList[i].Amount;
                        if (oIssue.NomineeList[i].DateOfBirth.Year > 1900)
                        {
                            rowNominee["DateOfBirth"] = Date.GetDateTimeByString(oIssue.NomineeList[i].DateOfBirth.ToString());
                        }
                        dtNominee.Rows.Add(rowNominee);
                    }

                    //Reload Grid
                    gvNomDetail.DataSource = dtNominee;
                    gvNomDetail.DataBind();
                }
            }
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
                }
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
                SEARCH_FROM = 2;
                hdDataType.Value = "1";
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                LoadDataByRegNo(gvRow.Cells[2].Text, "1");
            }
        }

        public void PopupIssueSearchLoadAction(string sRegNo, string sName, string sApprovalStaus)
        {            
            SEARCH_FROM = 2;
            hdDataType.Value = "2";
            LoadDataByRegNo(sRegNo, "2");
        }


        public void LoadPreviousList()
        {
            ReinvestmentDAL oRDAL = new ReinvestmentDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oRDAL.LoadUnapprovedList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);
            
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

        protected void btnSearchOldRegNo_Click(object sender, EventArgs e)
        {
            //if (txtOldRegNo.Text.ToUpper().Contains(Constants.SP_TYPE_WDB)
            //    || txtOldRegNo.Text.ToUpper().Contains(Constants.SP_TYPE_DIB)
            //    || txtOldRegNo.Text.ToUpper().Contains(Constants.SP_TYPE_DPB))
            //{
                Reinvestment oReinvestment = new Reinvestment();
                oReinvestment.Issue.RegNo = txtOldRegNo.Text.ToUpper();
                hdDataType.Value = string.Empty;
                SEARCH_FROM = 2;
                LoadDataByRegNo(txtOldRegNo.Text, "");

                //ReinvestmentDAL oReinvDAL = new ReinvestmentDAL();
                //Result oResult = new Result();
                //oResult = oReinvDAL.CheckAlreadyReinvested(oReinvestment);
                //if (oResult.Status)
                //{
                //    string sTmp = txtOldRegNo.Text;
                //    TotalClear();
                //    txtOldRegNo.Text = sTmp;
                //    ucMessage.OpenMessage("Already Reinvested.!", Constants.MSG_TYPE_INFO);
                //    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                //}
                //else
                //{
                //    LoadDataByRegNo(txtOldRegNo.Text, "");
                //}
            //}
            //else
            //{
            //    ucMessage.OpenMessage("Only Bonds can be reinvested.!", Constants.MSG_TYPE_INFO);
            //    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            //}
        }

        protected void btnDenomAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlDenomination.SelectedValue) && Util.GetIntNumber(txtQuantity.Text) > 0 && !string.IsNullOrEmpty(txtTotalAmount.Text))
            {
                int iDenomStatus = CheckDenominationInStock();
                
                if(iDenomStatus.Equals(0))
                {
                    ucMessage.OpenMessage("selected denomination currently out of stock.", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
                else if (iDenomStatus.Equals(-1))
                {
                    ucMessage.OpenMessage("Denomination quantity cannot be higher than current stock quantity!!", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
                else
                {
                    Reinvestment oReinvestment = (Reinvestment)Session[Constants.SES_REINVESTMENT];

                    if (oReinvestment != null)
                    {
                        AddDenominationToSession(oReinvestment);
                    }
                }
            }
        }

        private void AddDenominationToSession(Reinvestment oReinvestment)
        {
            Issue oIssue = oReinvestment.Issue;

            if (oIssue == null)
            {
                oIssue = new Issue();
            }

            
            DataTable dtDenom = new DataTable();

            dtDenom.Columns.Add(new DataColumn("DenominationID", typeof(string)));
            dtDenom.Columns.Add(new DataColumn("Quantity", typeof(string)));
            decimal dTotalAmount = 0;
            DataRow rowDenom = null;
            for (int i = 0; i < oIssue.IssueDenominationList.Count; i++)
            {
                if (oIssue.IssueDenominationList[i].Denomination.DenominationID != Util.GetIntNumber(ddlDenomination.SelectedValue))
                {
                    rowDenom = dtDenom.NewRow();

                    rowDenom["DenominationID"] = oIssue.IssueDenominationList[i].Denomination.DenominationID.ToString();
                    rowDenom["Quantity"] = oIssue.IssueDenominationList[i].Quantity.ToString();
                    dTotalAmount += (Convert.ToDecimal(oIssue.IssueDenominationList[i].Denomination.DenominationID) * Convert.ToDecimal(oIssue.IssueDenominationList[i].Quantity));
                    dtDenom.Rows.Add(rowDenom);
                }
            }
            dTotalAmount += Convert.ToDecimal(ddlDenomination.SelectedValue) * Convert.ToDecimal(txtQuantity.Text);

            if (Util.GetDecimalNumber(txtTotalAmount.Text) >= dTotalAmount)
            {
                IssueDenomination oIssueDenom = oIssue.IssueDenominationList.Where(d => d.Denomination.DenominationID.Equals(Convert.ToInt32(ddlDenomination.SelectedValue))).SingleOrDefault();

                if (oIssueDenom != null)
                {
                    oIssue.IssueDenominationList.Remove(oIssueDenom);
                }
                else
                {
                    oIssueDenom = new IssueDenomination();
                }

                oIssueDenom.Denomination.DenominationID = Util.GetIntNumber(ddlDenomination.SelectedValue);

                oIssueDenom.IssueNo = 1;//To be queried
                oIssueDenom.IssueTransNo = 1;
                oIssueDenom.Quantity = Util.GetIntNumber(txtQuantity.Text);
                if (!string.IsNullOrEmpty(ddlSpType.SelectedValue))
                {
                    oIssueDenom.SPType.SPTypeID = ddlSpType.SelectedValue;
                }
                //Add to List
                oIssueDenom.UserDetail = ucUserDet.UserDetail;
                oIssue.IssueDenominationList.Add(oIssueDenom);

                rowDenom = dtDenom.NewRow();

                rowDenom["DenominationID"] = ddlDenomination.SelectedValue.ToString();
                rowDenom["Quantity"] = txtQuantity.Text.ToString();

                dtDenom.Rows.Add(rowDenom);

                gvDenDetail.DataSource = dtDenom;
                gvDenDetail.DataBind();
                txtSelectedAmount.Text = dTotalAmount.ToString("N2");

                oReinvestment.ReinvestAmount = dTotalAmount;
                oReinvestment.Issue.IssueDenominationList = oIssue.IssueDenominationList;
            }

            else
            {
                ucMessage.OpenMessage("You cannot exceed total amount.", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }// if not exceed total amount
            Session[Constants.SES_REINVESTMENT] = oReinvestment;
        }

        private int CheckDenominationInStock()
        {            
            DataTable dtStockReg = ViewState[_STOCK_INFO] as DataTable;
            int dInStock = 1;

            if (dtStockReg != null && dtStockReg.Rows.Count > 0)
            {
                int iRNumber = 0;
                int iUNumber = 0;
                int iCRNumber = 0;
                for (int i = 0; i < dtStockReg.Rows.Count; i++)
                {
                    if (DB.GetDBValue(dtStockReg.Rows[i]["Denomination"]).Equals(ddlDenomination.SelectedValue))
                    {
                        iRNumber = Util.GetIntNumber(DB.GetDBValue(dtStockReg.Rows[i]["Remaining Demomination"]));
                        break;
                    }
                }
                if (iRNumber.Equals(0))
                {
                    dInStock = 0;
                }
                else
                {

                    for (int i = 0; i < gvDenDetail.Rows.Count; i++)
                    {
                        GridViewRow dr = (GridViewRow)gvDenDetail.Rows[i];
                        if (dr.Cells[1].Text == ddlDenomination.SelectedValue)
                        {
                            iUNumber += Util.GetIntNumber(dr.Cells[2].Text);
                        }
                    }
                    iCRNumber = iRNumber - iUNumber;
                    if (Util.GetIntNumber(txtQuantity.Text) > iCRNumber)
                    {
                        dInStock = -1;                       
                    }
                }                
            }
            else
            {
                dInStock = 0;
            }

            return dInStock;            
        }

        protected void gvDenDetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void gvDenDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            Reinvestment oReinvestment = (Reinvestment)Session[Constants.SES_REINVESTMENT];

            Issue oIssue = oReinvestment.Issue;

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
                gvDenDetail.DataSource = dtDenom;
                gvDenDetail.DataBind();

                txtSelectedAmount.Text = iTotalAmount.ToString("N2");
                // txtAppliedAmount.Text = iTotalAmount.ToString();

                Session[Constants.SES_REINVESTMENT] = oReinvestment;
            }
        }


        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (hdDataType.Value != "2")
            {
                Reinvestment oReinvestment = (Reinvestment)Session[Constants.SES_REINVESTMENT];
                if (oReinvestment != null)
                {
                    ReinvestmentDAL oReinvestmentDAL = new ReinvestmentDAL();
                    Result oResult = (Result)oReinvestmentDAL.DeleteReinvestment(oReinvestment);
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
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_APPROVED_DELETE_DATA, Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                if (hdDataType.Value != "2")
                {
                    Reinvestment oReinvestment = (Reinvestment)Session[Constants.SES_REINVESTMENT];
                    Issue oIssue = null;
                    if (oReinvestment != null)
                    {
                        oIssue = oReinvestment.Issue;

                        if (oIssue == null)
                        {
                            oIssue = new Issue();
                        }
                        if (Util.GetDecimalNumber(txtSelectedAmount.Text) < Util.GetDecimalNumber(txtTotalAmount.Text))
                        {
                            ucMessage.OpenMessage("Partial reinvestment is not allowed.", Constants.MSG_TYPE_INFO);
                            return;
                        }
                    }
                    else
                    {
                        oReinvestment = new Reinvestment();
                    }

                    if (string.IsNullOrEmpty(oReinvestment.ReinvestmentRefNo))
                    {
                        oReinvestment.ReinvestmentRefNo = "-1";
                    }
                    oReinvestment.OldIssueNo = 1;

                    // Issue Details 
                    oIssue.IssueName = txtIssueName.Text.ToUpper();
                    oIssue.VersionIssueDate = Util.GetDateTimeByString(txtMaturityDate.Text);

                    oIssue.MasterNo = txtMasterNo.Text;
                    // Bond Holder Details
                    oIssue.BondHolderAddress = txtBHDAddress.Text.ToUpper();
                    oIssue.BondHolderName = txtBHDHolderName.Text.ToUpper();
                    oIssue.BondHolderRelation = txtBHDRelation.Text.ToUpper();

                    //SP Type Details
                    oIssue.SPType.SPTypeID = ddlSpType.SelectedValue != "" ? ddlSpType.SelectedValue : "";
                    oIssue.Branch.BranchID = ddlBranch.SelectedValue != "" ? ddlBranch.SelectedValue : "";
                    //oIssue.IssueTransNo = hdTransNo.Value == "" ? "-1" : hdTransNo.Value;
                    oIssue.RegNo = txtOldRegNo.Text;
                    oIssue.CustomerType.CustomerTypeID = ddlCustomerType.SelectedValue != "" ? ddlCustomerType.SelectedValue : "";
                    oIssue.IssueAmount = Util.GetDecimalNumber(txtSelectedAmount.Text);

                    //Status Details
                    oIssue.IsApproved = 1;
                    oIssue.IsClaimed = false;
                    oIssue.Status = 1;//only for Temporary purpose


                    //User Details
                    oIssue.UserDetails = ucUserDet.UserDetail;
                    oReinvestment.ReinvestDate = Util.GetDateTimeByString(txtReinvestmentDate.Text);
                    oReinvestment.UserDetails = ucUserDet.UserDetail;

                    if (!string.IsNullOrEmpty(oConfig.UserName))
                    {
                        oIssue.UserDetails.MakerID = oConfig.UserName;
                        oReinvestment.UserDetails.MakerID = oConfig.UserName;
                    }
                    if (!string.IsNullOrEmpty(oConfig.DivisionID))
                    {
                        oIssue.DivisionID = oConfig.DivisionID;
                        oReinvestment.UserDetails.Division = oConfig.DivisionID;
                    }
                    if (!string.IsNullOrEmpty(oConfig.BankCodeID))
                    {
                        oIssue.BankID = oConfig.BankCodeID;
                        oReinvestment.UserDetails.BankID = oConfig.BankCodeID;
                    }
                    oReinvestment.Issue = oIssue;
                    SPPolicy oSPPolicyNew = Session["SPPolicy"] as SPPolicy;
                    if (oSPPolicyNew != null)
                    {
                        oReinvestment.Issue.VersionSPPolicy = oSPPolicyNew;
                    }
                    ReinvestmentDAL oReinvestmentDAL = new ReinvestmentDAL();
                    Result oResult = (Result)oReinvestmentDAL.Save(oReinvestment);

                    if (oResult.Status)
                    {
                        LoadPreviousList();
                        TotalClear();
                        ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
                    }
                    else
                    {
                        ucMessage.OpenMessage(oResult.Message, Constants.MSG_TYPE_ERROR);
                    }
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_APPROVED_SAVE_DATA, Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
        }

        protected void btnNewRegNo_Click(object sender, EventArgs e)
        {
            SEARCH_FROM = 2;
            hdDataType.Value = "2";
            LoadDataByRegNo(txtNewRegNo.Text.Trim(), "2");
        }

        protected void txtOldRegNo_TextChanged(object sender, EventArgs e)
        {
            btnSearchOldRegNo_Click(sender, e);
        }
    }
}
