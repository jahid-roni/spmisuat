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
using System.Collections;
using SBM_BLC1.DAL.Common;
using System.Threading;

namespace SBM_WebUI.mp
{
    public partial class StopPayMark : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_TRANS.STOP_PAYMENT_MARK))
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

            txtCDUpTo.Text = "1";//Default value

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
                   
                    LoadDataByRegNo("", sRegNo, "1");//Loaded from Temp Table

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
            if (!string.IsNullOrEmpty(txtRegNo.Text))
            {
                
                SEARCH_FROM = 2;
                hdDataType.Value = "";
                LoadDataByRegNo("", txtRegNo.Text, "2");//Loaded from Main Table
                //EnableDisableControl(false);
            }
        }

        protected void gvCertInfo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            StopPayment oStopPay = (StopPayment)Session[Constants.SES_STOPPAY_MARK];

            if (oStopPay != null && gvRow != null)
            {
                oStopPay.DtStopPaymentDetail.Rows.RemoveAt(gvRow.RowIndex);

                gvCertInfo.DataSource = oStopPay.DtStopPaymentDetail;
                gvCertInfo.DataBind();

                Session[Constants.SES_STOPPAY_MARK] = oStopPay;
            }

            decimal dTotalAmount = 0m;
            foreach (GridViewRow gvr in gvCertInfo.Rows)
            {
                dTotalAmount += Util.GetDecimalNumber(gvr.Cells[1].Text);
            }

            txtCDTotalAmount.Text = dTotalAmount.ToString("N2");
        }


        protected void gvCertInfo_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //
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
                hdDataType.Value = "";
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                LoadDataByRegNo("", gvRow.Cells[1].Text, "1");
                //EnableDisableControl(false);
            }
        }

        protected void btnMarkAll_OnClick(object sender, EventArgs e)
        {
            if (ddlCDDenom.SelectedItem != null)
            {
                if (!string.IsNullOrEmpty(ddlCDDenom.SelectedItem.Value))
                {
                    Calculate("MA");
                }
            }
        }

        protected void btnMark_OnClick(object sender, EventArgs e)
        {
            if (ddlCDCertif.SelectedItem != null)
            {
                if (!string.IsNullOrEmpty(ddlCDCertif.SelectedItem.Value))
                {
                    Calculate("MS");
                }
            }
        }

        protected void btnRemoveAll_OnClick(object sender, EventArgs e)
        {
            Calculate("RA");
        }


        public void PopupStopPaySearchLoadAction(string sStopPayMarkTrnsactionNo, string sRegNo, string sApprovalStaus)
        {
            if (!string.IsNullOrEmpty(sRegNo))
            {
                SEARCH_FROM = 1;
                hdDataType.Value = sApprovalStaus;
                LoadDataByRegNo(sStopPayMarkTrnsactionNo, sRegNo, sApprovalStaus);
                
            }
        }

        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");

            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.STOP_PAYMENT_MARK).PadLeft(5, '0'), false);
            }
            else
            {
                txtRegNo.Focus();
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.STOP_PAYMENT_MARK).PadLeft(5, '0'), false);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdStopPayTransNo.Value))
            {
                StopPayment oStopPay = new StopPayment(hdStopPayTransNo.Value);
                StopPaymentDAL oStopPayDAL = new StopPaymentDAL();
                oStopPay.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oStopPayDAL.RejectStopPayMark(oStopPay);
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
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND + " to Reject", Constants.MSG_TYPE_INFO);
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdStopPayTransNo.Value))
            {
                StopPayment oStopPay = new StopPayment(hdStopPayTransNo.Value);
                StopPaymentDAL oStopPayDAL = new StopPaymentDAL();
                oStopPay.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oStopPayDAL.ApproveStopPayMark(oStopPay);
                if (oResult.Status)
                {                    
                    ReportDAL rdal = new ReportDAL();

                    oResult = rdal.StopPaymentLetter(Constants.LETTER_TYPE_STOP, txtStopPayTransNo.Text.Trim());
                    if (oResult.Status)
                    {
                        Session[Constants.SES_RPT_DATA] = oResult.Return;
                        Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport2(1));
                    }

                    ClearAfterApprove();

                    //ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE, Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_APPROVE, Constants.MSG_TYPE_ERROR);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND + " to Approve", Constants.MSG_TYPE_INFO);
            }
        }
        
        protected void btnReset_Click(object sender, EventArgs e)
        {
           // EnableDisableControl(false);
            TotalClear();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                if (!(hdDataType.Value.Equals("2")))
                {
                    if (ddlCDDenom.Items.Count > 1)
                    {
                        StopPayment oStopPay = GetObject();
                        oStopPay.UserDetails = ucUserDet.UserDetail;
                        oStopPay.UserDetails.MakeDate = DateTime.Now;
                        ucUserDet.ResetData();
                        StopPaymentDAL oStopPayment = new StopPaymentDAL();
                        Result oResult = oStopPayment.SaveStopPayMark(oStopPay);
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
                if (!string.IsNullOrEmpty(hdStopPayTransNo.Value))
                {
                    StopPaymentDAL oStopPayDAL = new StopPaymentDAL();
                    Result oResult = (Result)oStopPayDAL.DeteteStopPayMark(hdStopPayTransNo.Value);
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

        protected void ddlCDDenom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlCDCertif.Items.Clear();
            if (!string.IsNullOrEmpty(ddlCDDenom.SelectedValue))
            {
                StopPayment oStopPay = (StopPayment)Session[Constants.SES_STOPPAY_MARK];
                DDListUtil.Add(ddlCDCertif, "", "");
                //Filtered by Denomination
                List<Scrip> filteredScripList = oStopPay.Issue.ScripList.Where(s => s.Denomination.DenominationID.ToString().Equals(ddlCDDenom.SelectedValue)).ToList();

                for (int iScripCount = 0; iScripCount < filteredScripList.Count; iScripCount++)
                {
                    DDListUtil.Add(ddlCDCertif, filteredScripList[iScripCount].SPSeries + " " + filteredScripList[iScripCount].SlNo, filteredScripList[iScripCount].SPScripID.ToString() + ":" + filteredScripList[iScripCount].OlsStatus.ToString());
                }
            }
        }

        #endregion Event Method...

        #region Util Method...


        public void PopupIssueSearchLoadAction(string sRegNo, string sStopPaymentTransNo, string sApprovalStaus)
        {
            if (!string.IsNullOrEmpty(sRegNo))
            {
                SEARCH_FROM = 2;
                hdDataType.Value = "";                
                LoadDataByRegNo("", sRegNo, sApprovalStaus);
              
            }
        }

        private void LoadDataByRegNo(string sStopPaymentTransNo, string sRegNo, string sApprovalStaus)
        {
            StopPaymentDAL oStopPayDAL = new StopPaymentDAL();
            Result oResult = (Result)oStopPayDAL.LoadStopPayMarkByRegNo(sStopPaymentTransNo, sRegNo, sApprovalStaus);
            TotalClear();
            if (oResult.Status)
            {
                StopPayment oStopPay = (StopPayment)oResult.Return;
                SetObject(oStopPay);

                if (hdDataType.Value.Equals("2"))
                {
                    EnableDisableControl(true);
                    Util.ControlEnabled(btnReject, false);
                    Util.ControlEnabled(btnApprove, false);
                    Util.ControlEnabled(btnBack, false);

                    Util.ControlEnabled(btnReset, true);
                    Util.ControlEnabled(btnSave, true);
                    Util.ControlEnabled(btnDelete, true);

                    btnRegSearch.Enabled = true;
                    btnStopPaySearch.Enabled = true;
                 
                    
                   

                    fsList.Visible = true;
                }
                else if (SEARCH_FROM.Equals(3))
                {
                    EnableDisableControl(true);
                    Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);
                }
                else if(SEARCH_FROM.Equals(2))
                {
                    EnableDisableControl(false);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
            }
        }


        private void SetObject(StopPayment oStopPay)
        {
            if (oStopPay != null)
            {
                if (oStopPay.Issue != null)
                {
                    hdStopPayTransNo.Value = oStopPay.StopPaymentTransNo;
                    hdIssueTransNo.Value = oStopPay.Issue.IssueTransNo;
                    hdRegNo.Value = oStopPay.Issue.RegNo;

                    txtRegNo.Text = oStopPay.Issue.RegNo.ToString();
                    ddlSpType.Text = oStopPay.Issue.SPType.SPTypeID.Trim();
                    ddlBranch.Text = oStopPay.Issue.Branch.BranchID.Trim();

                    txtStopPaymentDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
                    txtStopPayTransNo.Text = oStopPay.StopPaymentTransNo;
                    txtIssueName.Text = oStopPay.Issue.IssueName;
                    
                    //Issue Details
                    DDListUtil.Assign(ddlSpType, oStopPay.Issue.SPType.SPTypeID);
                    DDListUtil.Assign(ddlYear, oStopPay.Issue.VersionIssueDate.Year);
                    DDListUtil.Assign(ddlBranch,oStopPay.Issue.Branch.BranchID);
                    txtRegNo.Text = oStopPay.Issue.RegNo;
                    txtIssueDate.Text = oStopPay.Issue.VersionIssueDate.ToString(Constants.DATETIME_FORMAT);
                    txtTotalAmount.Text = oStopPay.Issue.IssueAmount.ToString("N2");
                    DDListUtil.Assign(ddlCustomerType, oStopPay.Issue.VersionSPPolicy.DTCustomerTypePolicy, true);
                    DDListUtil.Assign(ddlCustomerType, oStopPay.Issue.CustomerType.CustomerTypeID);

                
                    #region Customer Details
                    DataTable dtCustomerDetails = new DataTable();

                    dtCustomerDetails.Columns.Add(new DataColumn("Customer Name", typeof(string)));
                    //dtCustomerDetails.Columns.Add(new DataColumn("Customer ID", typeof(string)));
                    dtCustomerDetails.Columns.Add(new DataColumn("Address", typeof(string)));
                    dtCustomerDetails.Columns.Add(new DataColumn("Phone", typeof(string)));

                    DataRow rowCustomerDetails = null;

                    for (int customerCount = 0; customerCount < oStopPay.Issue.CustomerDetailsList.Count; customerCount++)
                    {
                        rowCustomerDetails = dtCustomerDetails.NewRow();

                        rowCustomerDetails["Customer Name"] = oStopPay.Issue.CustomerDetailsList[customerCount].CustomerName;
                        //rowCustomerDetails["Customer ID"] = oStopPay.Issue.CustomerDetailsList[customerCount].CustomerID;
                        rowCustomerDetails["Address"] = oStopPay.Issue.CustomerDetailsList[customerCount].Address;
                        rowCustomerDetails["Phone"] = oStopPay.Issue.CustomerDetailsList[customerCount].Phone;

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

                    for (int nomineeCount = 0; nomineeCount < oStopPay.Issue.NomineeList.Count; nomineeCount++)
                    {
                        rowNomineeDetail = dtNomineeDetail.NewRow();

                        rowNomineeDetail["Nominee Name"] = oStopPay.Issue.NomineeList[nomineeCount].NomineeName;
                        rowNomineeDetail["Relation"] = oStopPay.Issue.NomineeList[nomineeCount].Relation;
                        rowNomineeDetail["Address"] = oStopPay.Issue.NomineeList[nomineeCount].Address;
                        rowNomineeDetail["Nominee Share"] = oStopPay.Issue.NomineeList[nomineeCount].NomineeShare;
                        rowNomineeDetail["Amount"] = oStopPay.Issue.NomineeList[nomineeCount].Amount;

                        dtNomineeDetail.Rows.Add(rowNomineeDetail);
                    }
                    gvNomDetail.DataSource = dtNomineeDetail;
                    gvNomDetail.DataBind();

                    #endregion

                    ArrayList alScrip = new ArrayList();
                    ddlCDDenom.Items.Clear();
                    DDListUtil.Add(ddlCDDenom, "", "");
                    for (int iScripCount = 0; iScripCount< oStopPay.Issue.ScripList.Count; iScripCount++)
                    {
                        if (!alScrip.Contains(oStopPay.Issue.ScripList[iScripCount].Denomination.DenominationID))
                        {
                            DDListUtil.Add(ddlCDDenom, oStopPay.Issue.ScripList[iScripCount].Denomination.DenominationID.ToString(), oStopPay.Issue.ScripList[iScripCount].Denomination.DenominationID.ToString());
                            alScrip.Add(oStopPay.Issue.ScripList[iScripCount].Denomination.DenominationID);
                        }
                    }

                    // remarks
                    txtRemarks.Text = oStopPay.Remarks;

                    // Certificate Detail
                    DataTable dtStopPayment = oStopPay.DtStopPaymentDetail.Copy();
                    if (dtStopPayment.Columns.Contains("OldStatus"))
                    {
                        dtStopPayment.Columns.Remove("OldStatus");
                    }
                    if (dtStopPayment.Columns.Contains("SPScripID"))
                    {
                        dtStopPayment.Columns.Remove("SPScripID");
                    }
                    gvCertInfo.DataSource = dtStopPayment;
                    gvCertInfo.DataBind();

                    for (int i = 0; i < gvCertInfo.Rows.Count; i++)
                    {
                        HiddenField hdObj = (HiddenField)gvCertInfo.Rows[i].FindControl("hdOldStatus");
                        if (hdObj != null)
                        {
                            hdObj.Value = oStopPay.DtStopPaymentDetail.Rows[i]["OldStatus"].ToString();
                        }
                    }
                    Session[Constants.SES_STOPPAY_MARK] = oStopPay;

                    // user detail
                   
                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    if (hdDataType.Value.Equals("2") && SEARCH_FROM.Equals(1))
                    {
                        oUserDetails.MakerID = oStopPay.UserDetails.MakerID;
                        oUserDetails.MakeDate = oStopPay.UserDetails.MakeDate;
                        oUserDetails.CheckerID = oStopPay.UserDetails.CheckerID;
                        oUserDetails.CheckDate = oStopPay.UserDetails.CheckDate;
                        oUserDetails.CheckerComment = oStopPay.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;
                        txtStopPaymentDate.Text = oStopPay.StopPaymentDate.ToString(Constants.DATETIME_FORMAT);
                    }
                    if ((hdDataType.Value.Equals("1") && SEARCH_FROM.Equals(1)) || SEARCH_FROM.Equals(2))
                    {
                        oUserDetails.CheckerID = oStopPay.UserDetails.CheckerID;
                        oUserDetails.CheckDate = oStopPay.UserDetails.CheckDate;
                        oUserDetails.CheckerComment = oStopPay.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;
                    }
                    if(SEARCH_FROM.Equals(3))
                    {
                        oUserDetails.MakerID = oStopPay.UserDetails.MakerID;
                        oUserDetails.CheckerComment = oStopPay.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;
                    }

                    if (oStopPay.Issue.Status.Equals((int)Constants.ISSUE_STATUS.FULL_ENCAHSED))
                    {
                        ucMessage.OpenMessage("Already Encashed.", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    }

                    Calculate("");
                }
            }
        }

        private StopPayment GetObject()
        {
            StopPayment oStopPay = (StopPayment)Session[Constants.SES_STOPPAY_MARK];
            
            if (oStopPay != null)
            {
                if (string.IsNullOrEmpty(oStopPay.StopPaymentTransNo))
                {
                    oStopPay.StopPaymentTransNo = "-1";
                }
                oStopPay.Remarks = txtRemarks.Text;
                oStopPay.StopPaymentAmount = Util.GetDecimalNumber(txtCDTotalAmount.Text);
                oStopPay.StopPaymentDate = Util.GetDateTimeByString(txtStopPaymentDate.Text);

                oStopPay.UserDetails = ucUserDet.UserDetail;
            }

            return oStopPay;
        }

        private void TotalClear()
        {
            // Stop Payment Mark set in session 
            EnableDisableControl(false);
            StopPayment oStopPayment = new StopPayment();
            if (Session[Constants.SES_STOPPAY_MARK] == null)
            {
                Session.Add(Constants.SES_STOPPAY_MARK, oStopPayment);
            }
            else
            {
                Session[Constants.SES_STOPPAY_MARK] = oStopPayment;
            }

            gvCertInfo.DataSource = null;
            gvCertInfo.DataBind();

            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            hdStopPayTransNo.Value = "";
            hdIssueTransNo.Value = "";
            hdRegNo.Value = "";

            //Stop Payment Mark Transaction No
            txtStopPaymentDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtStopPayTransNo.Text = string.Empty;
           

            //Issue Details.. year
            if (ddlYear.Items.Count > 0)
            {
                ddlYear.Text = DateTime.Now.Year.ToString();
            }
            else
            {
                for (int i = 1990; i < DateTime.Now.Year +1 +1; i++)
                {
                    DDListUtil.Add(ddlYear, i.ToString(), i.ToString());
                }
                ddlYear.Text=DateTime.Now.Year.ToString();
            }

            txtTotalAmount.Text = string.Empty;
            ddlCustomerType.Items.Clear();
            if (ddlSpType.Items.Count > 0)
            {
                ddlSpType.SelectedIndex = 0;
            }
            if (ddlBranch.Items.Count > 0)
            {
                ddlBranch.SelectedIndex = 0;
            }
            txtIssueDate.Text = string.Empty;
            txtIssueName.Text = string.Empty;
            txtRegNo.Text = string.Empty;

            // Remarks 
            txtRemarks.Text = string.Empty;

            //Certificate Detail
            ddlCDDenom.Items.Clear();
            ddlCDCertif.Items.Clear();
            txtCDUpTo.Text = "1";
            txtCDTotalAmount.Text = string.Empty;

            ucUserDet.Reset();
            ucUserDet.ResetData();
        }

        private void ClearAfterApprove()
        {
            // Stop Payment Mark set in session 

            Session[Constants.SES_STOPPAY_MARK] = null;
            
            gvCertInfo.DataSource = null;
            gvCertInfo.DataBind();

            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            hdStopPayTransNo.Value = "";
            hdIssueTransNo.Value = "";
            hdRegNo.Value = "";

            //Stop Payment Mark Transaction No
            txtStopPaymentDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtStopPayTransNo.Text = string.Empty;


            //Issue Details.. year
            if (ddlYear.Items.Count > 0)
            {
                ddlYear.Text = DateTime.Now.Year.ToString();
            }
            else
            {
                for (int i = 1990; i < DateTime.Now.Year +1 +1; i++)
                {
                    DDListUtil.Add(ddlYear, i.ToString(), i.ToString());
                }
                ddlYear.Text = DateTime.Now.Year.ToString();
            }

            txtTotalAmount.Text = string.Empty;
            ddlCustomerType.Items.Clear();
            if (ddlSpType.Items.Count > 0)
            {
                ddlSpType.SelectedIndex = 0;
            }
            if (ddlBranch.Items.Count > 0)
            {
                ddlBranch.SelectedIndex = 0;
            }
            txtIssueDate.Text = string.Empty;
            txtIssueName.Text = string.Empty;
            txtRegNo.Text = string.Empty;

            // Remarks 
            txtRemarks.Text = string.Empty;

            //Certificate Detail
            ddlCDDenom.Items.Clear();
            ddlCDCertif.Items.Clear();
            txtCDUpTo.Text = "1";
            txtCDTotalAmount.Text = string.Empty;

            ucUserDet.Reset();
            ucUserDet.ResetData();
        }

        private void Calculate(string sAction)
        {
            StopPayment oStopPay = (StopPayment)Session[Constants.SES_STOPPAY_MARK];

            DataTable dt = null;
            //ArrayList alOldStatus = new ArrayList();
            
            if (oStopPay != null)
            {
                dt = (DataTable)oStopPay.DtStopPaymentDetail;

                if (dt.Columns.Count <= 0)
                {
                    dt.Columns.Add(new DataColumn("SPScripID", typeof(string)));
                    dt.Columns.Add(new DataColumn("Denomination", typeof(string)));
                    dt.Columns.Add(new DataColumn("SP Series", typeof(string)));
                    dt.Columns.Add(new DataColumn("Sl No", typeof(string)));
                    dt.Columns.Add(new DataColumn("Status", typeof(string)));
                    dt.Columns.Add(new DataColumn("OldStatus", typeof(string)));
                }
            }
            
            if (sAction.Equals("MS")) //Mark Single
            {
                #region Mark Single
                int iUpTo = Util.GetIntNumber(txtCDUpTo.Text);

                if (iUpTo > 0)
                {
                    string sCertifVal= ddlCDCertif.SelectedItem.Value;
                    string[] sCertifValList = sCertifVal.Split(':');
                    int iSelectedIndex = ddlCDCertif.SelectedIndex;
                    int iOldStatus = Util.GetIntNumber(sCertifValList[1]);


                    for (int certificateIndex = 0; certificateIndex < iUpTo; certificateIndex++)
                    {
                        if (certificateIndex + iSelectedIndex == ddlCDCertif.Items.Count)
                        {
                            break;
                        }

                        string sTmpOldScripStatus = ddlCDCertif.Items[certificateIndex + iSelectedIndex].Value;
                        string[] sTmpList = sTmpOldScripStatus.Split(':');
                        bool isExist = false;
                        
                        if (dt.Rows.Count > 0)
                        {
                            DataRow[] selectedRow = dt.Select("SPScripID = " + sTmpList[0]);
                            if (selectedRow.Count() > 0)
                            {
                                isExist = true;
                            }
                        }
                                               
                        if(!isExist)
                        {                            
                            DataRow row = dt.NewRow();
                            
                            row["SPScripID"] = sTmpList[0];
                            row["Denomination"] = ddlCDDenom.SelectedItem.Value;
                            string sTmp = ddlCDCertif.Items[certificateIndex + iSelectedIndex].Text;
                            row["SP Series"] = (sTmp.Substring(0, sTmp.LastIndexOf(' '))).Trim();
                            row["Sl No"] = (sTmp.Substring(sTmp.LastIndexOf(' ') + 1, (sTmp.Length - sTmp.LastIndexOf(' ')) - 1)).Trim();
                            row["Status"] = "Stopped";
                            row["OldStatus"] = sTmpList[1];
                            dt.Rows.Add(row);
                        }
                    }
                } 
                #endregion
            }
            else if (sAction.Equals("MA")) //Mark All
            {
                #region Mark All
                if (oStopPay != null)
                {
                    int iDnomCount = 0;
                                        
                    if (dt.Rows.Count > 0)
                    {
                        object objCount = dt.Compute("COUNT(Denomination)", "Denomination = " + ddlCDDenom.SelectedItem.Value);
                        if (objCount != null)
                        {
                            iDnomCount = Convert.ToInt32(objCount);
                        }
                        if (iDnomCount > 0)
                        {                         
                            DataRow[] DrDenomCheck = dt.Select("Denomination = " + ddlCDDenom.SelectedItem.Value);
                            foreach (DataRow DrCheck in DrDenomCheck)
                            {
                                dt.Rows.Remove(DrCheck);
                            }                            
                        }
                    }

                    for (int scripCount = 0; scripCount < oStopPay.Issue.ScripList.Count; scripCount++)
                    {
                        DataRow row = dt.NewRow();

                        if (ddlCDDenom.SelectedItem.Value.Equals(oStopPay.Issue.ScripList[scripCount].Denomination.DenominationID.ToString()))
                        {
                            row["SPScripID"] = oStopPay.Issue.ScripList[scripCount].SPScripID;
                            row["Denomination"] = oStopPay.Issue.ScripList[scripCount].Denomination.DenominationID;
                            row["SP Series"] = oStopPay.Issue.ScripList[scripCount].SPSeries;
                            row["Sl No"] = oStopPay.Issue.ScripList[scripCount].SlNo;
                            row["Status"] = "Stopped";
                            row["OldStatus"] = oStopPay.Issue.ScripList[scripCount].OlsStatus;
                            dt.Rows.Add(row);                                                        
                        }
                    }
                } 
                #endregion
            }
            else if (sAction.Equals("RA")) //Remove All
            {
                dt.Rows.Clear();
            }
            
            DataTable tmpDt = dt.Copy();

            oStopPay.DtStopPaymentDetail = dt;
            Session[Constants.SES_STOPPAY_MARK] = oStopPay;
            if (tmpDt.Columns.Contains("OldStatus"))
            {
                tmpDt.Columns.Remove("OldStatus");
            }
            if (tmpDt.Columns.Contains("SPScripID"))
            {
                tmpDt.Columns.Remove("SPScripID");
            }
            gvCertInfo.DataSource = tmpDt;
            gvCertInfo.DataBind();

            for (int i = 0; i < gvCertInfo.Rows.Count; i++)
            {
                HiddenField hdObj = (HiddenField)gvCertInfo.Rows[i].FindControl("hdOldStatus");
                if (hdObj != null)
                {
                    hdObj.Value = oStopPay.DtStopPaymentDetail.Rows[i]["OldStatus"].ToString();
                }
            }

            decimal dTotalAmount = 0m;
            foreach (GridViewRow gvr in gvCertInfo.Rows)
            {
                dTotalAmount += Util.GetDecimalNumber(gvr.Cells[1].Text);
            }
            
            txtCDTotalAmount.Text = dTotalAmount.ToString("N2");            
        }

        public void LoadPreviousList()
        {
            StopPaymentDAL oStopPayDAL = new StopPaymentDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oStopPayDAL.LoadUnapprovedPaymentMarkList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);
            DataTable dtTmpList = null;

            gvData.DataSource = null;
            gvData.DataBind();

            if (oResult.Status)
            {
                dtTmpList = (DataTable)oResult.Return;

                gvData.DataSource = dtTmpList;
                gvData.DataBind();
            }

            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;
        }

        private void EnableDisableControl(bool isApproved)
        {
            // general Control

            if (isApproved)
            {
                //Stop Payment Mark Transaction No                
                Util.ControlEnabled(txtStopPayTransNo, false);
                Util.ControlEnabled(txtRegNo, false);

                //Issue Details
                Util.ControlEnabled(ddlYear, false);
                Util.ControlEnabled(txtTotalAmount, false);
                Util.ControlEnabled(ddlCustomerType, false);
                Util.ControlEnabled(ddlSpType, false);
                Util.ControlEnabled(ddlBranch, false);
                Util.ControlEnabled(txtIssueDate, false);
               
                Util.ControlEnabled(txtIssueName, false);
                                
                //Customer(s) Details
                gvCustomerDetail.Enabled = false;

                //Nominee(s) Details
                gvNomDetail.Enabled = false;

                //Certificate
                gvCertInfo.Enabled = false;

                // Remarks 
                Util.ControlEnabled(txtRemarks, false);

                //Certificate Detail
                Util.ControlEnabled(ddlCDDenom, false);
                Util.ControlEnabled(ddlCDCertif, false);
                Util.ControlEnabled(txtCDUpTo, false);
                Util.ControlEnabled(txtCDTotalAmount, false);
                                
                // button
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true);

                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);

                btnRemoveAll.Enabled = false;
                btnMarkAll.Enabled = false;
                btnMark.Enabled = false;
                btnRegSearch.Enabled = false;
                btnStopPaySearch.Enabled = false;
                
                fsList.Visible = false;
            }
            else
            {
                gvData.Enabled = true;

                Util.ControlEnabled(txtRegNo, true);
                // button 
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);

                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);

                btnRemoveAll.Enabled = true;
                btnMarkAll.Enabled = true;
                btnMark.Enabled = true;
                btnRegSearch.Enabled = true;
                btnStopPaySearch.Enabled = true;

                //Issue Details
                //Util.ControlEnabled(btnRegSearch, true);

                //Stop Payment Mark Transaction No                
                Util.ControlEnabled(txtStopPayTransNo, false);
                //Util.ControlEnabled(btnStopPaySearch, true);
                //Issue Details
                Util.ControlEnabled(ddlYear, false);
                Util.ControlEnabled(txtTotalAmount, false);
                Util.ControlEnabled(ddlCustomerType, false);
                Util.ControlEnabled(ddlSpType, false);
                Util.ControlEnabled(ddlBranch, false);
                Util.ControlEnabled(txtIssueDate, false);

                Util.ControlEnabled(txtIssueName, false);

                // Remarks 
                Util.ControlEnabled(txtRemarks, true);

                //Customer(s) Details
                gvCustomerDetail.Enabled = true;

                //Nominee(s) Details
                gvNomDetail.Enabled = true;

                //Certificate
                gvCertInfo.Enabled = true;

                //Certificate Detail
                Util.ControlEnabled(ddlCDDenom, true);
              
                Util.ControlEnabled(ddlCDCertif, true);
                Util.ControlEnabled(txtCDUpTo, true);
                
                Util.ControlEnabled(txtCDTotalAmount, false);
             
                fsList.Visible = true;
            }
        }
        #endregion
    }
}
