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
using SBM_BLC1.Entity.Transaction;

namespace SBM_WebUI.mp
{
    public partial class DuplicateIssueScript : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_TRANS.DUPLICATE_ISSUE))
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

            // user Detail
            Util.ControlEnabled(ucUserDet.FindControl("txtMakerId"), false);
            Util.ControlEnabled(ucUserDet.FindControl("txtCheckerId"), false);

            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            DDListUtil.LoadDDLFromDB(ddlBranch, "BranchID", "BranchName", "SPMS_Branch", true);

            string sRegNo = Request.QueryString[OBJ_REG_NO];
            string sPageID = Request.QueryString[OBJ_PAGE_ID];
            
            //Issue Details
            Util.ControlEnabled(txtStopTransNo, false);
            Util.ControlEnabled(txtStopDate, false);
            Util.ControlEnabled(ddlBranch, false);
            Util.ControlEnabled(txtIssueDate, false);
            Util.ControlEnabled(txtStopAmount, false);
            Util.ControlEnabled(ddlCustomerType, false);
            Util.ControlEnabled(ddlSpType, false);
            Util.ControlEnabled(txtIssueName, false);
            Util.ControlEnabled(txtMasterID, false);

            //Stop Payment Remove Mark Transaction No
            Util.ControlEnabled(txtRemoveTransNo, false);
            
            // Remarks
            Util.ControlEnabled(txtStopRemarks, false);

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
                    gvData.Enabled = false;
                    hdDataType.Value = "";
                    hdButtonType.Value = "D";
                    SEARCH_FROM = 1;
                    LoadDataByRegNo("", sRegNo, "1"); // comes from Tmp table. 

                    gvData.Enabled = false;

                    //Stop Payment Remove Mark Transaction No
                    Util.ControlEnabled(btnRemoveSearch, false);
                    Util.ControlEnabled(btnRegSearch, false);
                    Util.ControlEnabled(txtDuplicateIssueDate, false);

                    gvDenomDetail.Enabled = false;
                    btnAddDenomination.Enabled = false;

                    // Remarks  
                    Util.ControlEnabled(txtIssueRemarks, false);
                    Util.ControlEnabled(txtDuplicateMark,false);
                    Util.ControlEnabled(ddlDDDenom, false);
                    Util.ControlEnabled(txtDDQuantity, false);

                    // user Detail
                    Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);

                    // button
                    Util.ControlEnabled(btnReject, true);
                    Util.ControlEnabled(btnApprove, true);
                    Util.ControlEnabled(btnBack, true);

                    Util.ControlEnabled(btnReset, false);
                    Util.ControlEnabled(btnSave, false);
                    Util.ControlEnabled(btnDelete, false);

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
                gvData.Enabled = true;

                //Stop Payment Remove Mark Transaction No
                Util.ControlEnabled(btnRemoveSearch, true);
                Util.ControlEnabled(txtDuplicateIssueDate, false);

                //Issue Details
                //Util.ControlEnabled(txtRegNo, true);
                Util.ControlEnabled(btnRegSearch, true);

                gvDenomDetail.Enabled = true;
                btnAddDenomination.Enabled = true;

                // Remarks  
                Util.ControlEnabled(txtIssueRemarks, true);
                Util.ControlEnabled(txtDuplicateMark,true);

                // button 
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);

                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);

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
        protected void btnRemoveSearch_Click(object sender, EventArgs e)
        {

        }      

        private void EnableDisableControl(bool isEnabled)
        {
            gvData.Enabled = isEnabled;            
            gvDenomDetail.Enabled = isEnabled;
            txtIssueRemarks.Enabled = isEnabled;
            txtDuplicateMark.Enabled = isEnabled;
            ddlDDDenom.Enabled = isEnabled;
            txtDDQuantity.Enabled = isEnabled;
            btnAddDenomination.Enabled = isEnabled;
        }

        protected void btnDeleteAllDenomination_Click(object sender, EventArgs e)
        {
            DuplicateIssue oDI = (DuplicateIssue)Session[Constants.SES_DUPLICATE_ISSUE];
            if (oDI != null)
            {
                // Clear fields.. 
                ddlDDDenom.SelectedIndex = 0;
                //txtDuplicateMark.Text = string.Empty;
                txtDDQuantity.Text = "1";
                DataTable tmpDt = (DataTable)oDI.DtDuplicateScripsList;
                if (tmpDt.Rows.Count > 0)
                {
                    for (int i = 0; i < tmpDt.Rows.Count; i++)
                    {
                        tmpDt.Rows.RemoveAt(i);
                        i = -1;
                    }
                }
                oDI.DtDuplicateScripsList = tmpDt;
                gvDenomDetail.DataSource = oDI.DtDuplicateScripsList;
                gvDenomDetail.DataBind();
                Session[Constants.SES_DUPLICATE_ISSUE] = oDI;
                Calculation();
            }

        }
        protected void btnAddDenomination_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlDDDenom.SelectedItem.Value) && !string.IsNullOrEmpty(txtDDQuantity.Text))
            {
                ReceiveDAL rd = new ReceiveDAL();

                Result oResult= rd.CheckScripAvailabilty(ddlSpType.SelectedItem.Value, Convert.ToInt32(ddlDDDenom.SelectedItem.Value), Convert.ToInt32(txtDDQuantity.Text));
                if (oResult.Status)
                {
                    DuplicateIssue oDI = (DuplicateIssue)Session[Constants.SES_DUPLICATE_ISSUE];

                    DataTable dt = (DataTable)oDI.DtDuplicateScripsList;
                    DataRow row = null;

                    decimal dMax = Util.GetDecimalNumber(txtTotalStopAmount.Text);
                    decimal dOld = Util.GetDecimalNumber(txtTotalAmount.Text);
                    decimal dNext = dOld + Util.GetDecimalNumber(ddlDDDenom.SelectedItem.Value) * Util.GetIntNumber(txtDDQuantity.Text);

                    if (dNext <= dMax)
                    {
                        for (int i = 0; i < Util.GetIntNumber(txtDDQuantity.Text); i++)
                        {
                            row = dt.NewRow();
                            row["Denomination"] = ddlDDDenom.SelectedItem.Value;
                            row["DuplicateMark"] = txtDuplicateMark.Text;
                            dt.Rows.Add(row);
                        }
                        // Clear fields.. 
                        ddlDDDenom.SelectedIndex = 0;
                        // txtDuplicateMark.Text = string.Empty;
                        txtDDQuantity.Text = "1";

                        oDI.DtDuplicateScripsList = dt;
                        gvDenomDetail.DataSource = oDI.DtDuplicateScripsList;
                        gvDenomDetail.DataBind();
                        Session[Constants.SES_DUPLICATE_ISSUE] = oDI;
                        Calculation();
                    }
                    else
                    {
                        ucMessage.OpenMessage("Duplicate amount and stop certificate amount must be same", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    }
                }
                else
                {
                    ucMessage.OpenMessage("Scrips are not available for denomination (" + ddlDDDenom.SelectedValue + "). Please check.", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
        }

        public void PopupIssueSearchLoadAction(string sTransNo, string sRegNo, string sApprovalStaus)
        {
            if (!string.IsNullOrEmpty(sRegNo))
            {                
                LoadDataByRegNo(sTransNo, sRegNo, sApprovalStaus);
            }
        }

        public void PopupStopPaySearchLoadAction(string sDuplicateIssueTransNo, string sRegNo, string sApprovalStaus)
        {
            string sTmpValue = hdButtonType.Value;
            if (!string.IsNullOrEmpty(sRegNo))
            {
               // SEARCH_FROM = 1;
                hdDataType.Value = string.Empty;
                LoadDataByRegNo(sDuplicateIssueTransNo, sRegNo, sApprovalStaus);
            }
            hdButtonType.Value = sTmpValue;

            if (sApprovalStaus == "1")
            {
                hdDataType.Value = "";
                if (hdButtonType.Value.Equals("D"))
                {
                    EnableDisableControl(true);
                }
                else
                {
                    EnableDisableControl(false);
                }
            }
            else
            {
                hdDataType.Value = string.Empty;
                if (hdButtonType.Value.Equals("D"))
                {
                    EnableDisableControl(false);
                    gvData.Enabled = true;
                }
                else
                {
                    EnableDisableControl(true);
                }
            }
            //hdButtonType.Value = "";
        }

        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");

            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.DUPLICATE_ISSUE).PadLeft(5, '0'), false);
            }
            else
            {
                txtRegNo.Focus();
            }
        }
        
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.DUPLICATE_ISSUE).PadLeft(5, '0'), false);
        }
        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdDuplicateIssueTransNo.Value))
            {
                DuplicateIssue oDI = new DuplicateIssue(hdDuplicateIssueTransNo.Value);
                DuplicateIssueDAL oDIDAL = new DuplicateIssueDAL();
                oDI.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oDIDAL.Reject(oDI);
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
            if (!string.IsNullOrEmpty(hdDuplicateIssueTransNo.Value))
            {
                DuplicateIssue oDI = (DuplicateIssue)Session[Constants.SES_DUPLICATE_ISSUE];                
                DuplicateIssueDAL oDIDAL = new DuplicateIssueDAL();
                oDI.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oDIDAL.Approve(oDI);
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
            TotalClear();
            hdDataType.Value = "";
            EnableDisableControl(true);
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {            
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                if (btnAddDenomination.Enabled)
                {
                    DuplicateIssue oDI = GetObject();

                    if (oDI.DuplicateIssueAmount != oDI.StopPayment.StopPaymentAmount)
                    {
                        ucMessage.OpenMessage("Cannot be saved!! Duplicate Denomination Amount and Stop Certificate Amount must be same.", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    }
                    else
                    {
                        oDI.UserDetails = ucUserDet.UserDetail;
                        oDI.UserDetails.MakeDate = DateTime.Now;
                        ucUserDet.ResetData();
                        DuplicateIssueDAL oDIDAL = new DuplicateIssueDAL();

                        Result oResult = oDIDAL.Save(oDI);
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
            if (hdDataType.Value != "2")
            {
                if (!string.IsNullOrEmpty(hdDuplicateIssueTransNo.Value))
                {
                    DuplicateIssueDAL oDIDAL = new DuplicateIssueDAL();
                    Result oResult = (Result)oDIDAL.Detete(hdDuplicateIssueTransNo.Value);
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
                hdDataType.Value = "1";
                hdButtonType.Value = "D";
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                LoadDataByRegNo(gvRow.Cells[1].Text, gvRow.Cells[2].Text, "1");
                EnableDisableControl(true);
                
            }
        }

        protected void gvDenomDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            DuplicateIssue oDI = (DuplicateIssue)Session[Constants.SES_DUPLICATE_ISSUE];

            if (oDI != null && gvRow != null)
            {
                decimal dTotalAmount = Util.GetDecimalNumber(txtTotalAmount.Text) - Util.GetDecimalNumber(gvRow.Cells[1].Text);

                oDI.DtDuplicateScripsList.Rows.RemoveAt(gvRow.RowIndex);
                gvDenomDetail.DataSource = oDI.DtDuplicateScripsList;
                gvDenomDetail.DataBind();

                Session[Constants.SES_DUPLICATE_ISSUE] = oDI;

                // calculate txtTotalAmount
                txtTotalAmount.Text = dTotalAmount.ToString("N2");
            }
        }

        protected void gvStopPayment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, null);
        }
        
        #endregion Event Method...


        #region Util Method 

        private void Calculation()
        {
            DuplicateIssue oDI = (DuplicateIssue)Session[Constants.SES_DUPLICATE_ISSUE];
            DataTable dt = oDI.DtDuplicateScripsList;
            decimal dAmount = 0m;
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                { 
                    for(int i=0; i<dt.Rows.Count; i++)
                        dAmount += Util.GetDecimalNumber(dt.Rows[i]["Denomination"].ToString());
                }
            }
            txtTotalAmount.Text = dAmount.ToString("N2");
        }

        private void SetObject(DuplicateIssue oDI)
        {            
            if (oDI != null)
            {
                if (oDI.Issue != null)
                {
                    hdDuplicateIssueTransNo.Value = oDI.DuplicateIssueTransNo;
                    hdIssueTransNo.Value = oDI.Issue.IssueTransNo;
                    hdRegNo.Value = oDI.Issue.RegNo;

                    // general Search
                    txtRemoveTransNo.Text = oDI.DuplicateIssueTransNo;
                    txtDuplicateIssueDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
                    
                    // Issue Detail
                    DDListUtil.Assign(ddlSpType, oDI.Issue.SPType.SPTypeID);
                    DDListUtil.Assign(ddlBranch, oDI.Issue.Branch.BranchID);
                    txtRegNo.Text = oDI.Issue.RegNo;
                    txtIssueDate.Text = oDI.Issue.VersionIssueDate.ToString(Constants.DATETIME_FORMAT);
                    txtStopDate.Text = oDI.StopPayment.StopPaymentDate.ToString(Constants.DATETIME_FORMAT);
                    txtStopAmount.Text = oDI.StopPayment.StopPaymentAmount.ToString("N2");
                    txtStopTransNo.Text = oDI.StopPayment.StopPaymentTransNo;
                    txtIssueName.Text = oDI.Issue.IssueName;
                    txtMasterID.Text = oDI.Issue.MasterNo;
                    DDListUtil.Assign(ddlCustomerType, oDI.Issue.VersionSPPolicy.DTCustomerTypePolicy, true);
                    DDListUtil.Assign(ddlCustomerType, oDI.Issue.CustomerType.CustomerTypeID);
                    
                    //Customer(s) Details
                    #region Customer Details
                    DataTable dtCustomerDetails = new DataTable();

                    dtCustomerDetails.Columns.Add(new DataColumn("Customer Name", typeof(string)));
                    //dtCustomerDetails.Columns.Add(new DataColumn("Customer ID", typeof(string)));
                    dtCustomerDetails.Columns.Add(new DataColumn("Address", typeof(string)));
                    dtCustomerDetails.Columns.Add(new DataColumn("Phone", typeof(string)));

                    DataRow rowCustomerDetails = null;

                    for (int customerCount = 0; customerCount < oDI.Issue.CustomerDetailsList.Count; customerCount++)
                    {
                        rowCustomerDetails = dtCustomerDetails.NewRow();

                        rowCustomerDetails["Customer Name"] = oDI.Issue.CustomerDetailsList[customerCount].CustomerName;
                        //rowCustomerDetails["Customer ID"] = oDI.Issue.CustomerDetailsList[customerCount].CustomerID;
                        rowCustomerDetails["Address"] = oDI.Issue.CustomerDetailsList[customerCount].Address;
                        rowCustomerDetails["Phone"] = oDI.Issue.CustomerDetailsList[customerCount].Phone;

                        dtCustomerDetails.Rows.Add(rowCustomerDetails);
                    }

                    gvCustomerDetail.DataSource = dtCustomerDetails;
                    gvCustomerDetail.DataBind();
                    #endregion
                    
                    //Nominee(s) Details
                    #region Nominee Detail
                    DataTable dtNomineeDetail = new DataTable();

                    dtNomineeDetail.Columns.Add(new DataColumn("Nominee Name", typeof(string)));
                    dtNomineeDetail.Columns.Add(new DataColumn("Relation", typeof(string)));
                    dtNomineeDetail.Columns.Add(new DataColumn("Address", typeof(string)));
                    dtNomineeDetail.Columns.Add(new DataColumn("Nominee Share", typeof(string)));
                    dtNomineeDetail.Columns.Add(new DataColumn("Amount", typeof(string)));

                    DataRow rowNomineeDetail = null;

                    for (int nomineeCount = 0; nomineeCount < oDI.Issue.NomineeList.Count; nomineeCount++)
                    {
                        rowNomineeDetail = dtNomineeDetail.NewRow();

                        rowNomineeDetail["Nominee Name"] = oDI.Issue.NomineeList[nomineeCount].NomineeName;
                        rowNomineeDetail["Relation"] = oDI.Issue.NomineeList[nomineeCount].Relation;
                        rowNomineeDetail["Address"] = oDI.Issue.NomineeList[nomineeCount].Address;
                        rowNomineeDetail["Nominee Share"] = oDI.Issue.NomineeList[nomineeCount].NomineeShare;
                        rowNomineeDetail["Amount"] = oDI.Issue.NomineeList[nomineeCount].Amount;

                        dtNomineeDetail.Rows.Add(rowNomineeDetail);
                    }
                    gvNomDetail.DataSource = dtNomineeDetail;
                    gvNomDetail.DataBind();
                    #endregion

                    // Remarks
                    txtStopRemarks.Text = oDI.StopPayment.Remarks;
                    txtIssueRemarks.Text = "";
                    
                    //Stop Certificate(s) Details
                    DataTable tmpDt = oDI.DtStopPaymentList.Copy();
                    if (tmpDt.Columns.Contains("SPScripID"))
                    {
                        tmpDt.Columns.Remove("SPScripID");
                    }
                    gvStopPayment.DataSource = tmpDt;
                    //gvStopPayment.DataSource = oDI.DtStopPaymentList;
                    gvStopPayment.DataBind();
                    txtTotalStopAmount.Text = oDI.StopPayment.StopPaymentAmount.ToString("N2");

                    //Replace Denomination(s) details
                    gvDenomDetail.DataSource = oDI.DtDuplicateScripsList;
                    gvDenomDetail.DataBind();

                    //Set Duplicate Remarks
                    if (oDI.DtDuplicateScripsList.Rows.Count > 0)
                    {
                        txtDuplicateMark.Text = Convert.ToString(oDI.DtDuplicateScripsList.Rows[0]["DuplicateMark"]);
                    }
                    else
                    {
                        txtDuplicateMark.Text = "Duplicate Issued On " + DateTime.Now.ToString(Constants.DATETIME_FORMAT);
                    }

                    // User Info
                   // ucUserDet.UserDetail = oDI.UserDetails;

                    // Denomination
                    DataTable dtDenom = new DataTable();
                    
                    if (oDI.Issue.VersionSPPolicy.SPType.ListOfDenomination.Denomination.Count > 0)
                    {
                        dtDenom.Columns.Add(new DataColumn("Text", typeof(string)));
                        dtDenom.Columns.Add(new DataColumn("Value", typeof(string)));

                        DataRow rowDenom = null;
                        for (int i = 0; i < oDI.Issue.VersionSPPolicy.SPType.ListOfDenomination.Denomination.Count; i++)
                        {
                            rowDenom = dtDenom.NewRow();

                            rowDenom["Text"] = oDI.Issue.VersionSPPolicy.SPType.ListOfDenomination.Denomination[i].DenominationID.ToString();
                            rowDenom["Value"] = oDI.Issue.VersionSPPolicy.SPType.ListOfDenomination.Denomination[i].Series.ToString();
                            dtDenom.Rows.Add(rowDenom);
                        }
                    }
                    ddlDDDenom.Items.Clear();
                    DDListUtil.Assign(ddlDDDenom, dtDenom, true);
                }
            }
            Session[Constants.SES_DUPLICATE_ISSUE] = oDI;

            //User Deatils
            UserDetails oUserDetails = ucUserDet.UserDetail;
            if (hdButtonType.Value.Equals("D") && hdDataType.Value.Equals("2"))
            {
                oUserDetails.MakerID = oDI.UserDetails.MakerID;
                oUserDetails.MakeDate = oDI.UserDetails.MakeDate;
                oUserDetails.CheckerID = oDI.UserDetails.CheckerID;
                oUserDetails.CheckDate = oDI.UserDetails.CheckDate;
                oUserDetails.CheckerComment = oDI.UserDetails.CheckerComment;
                ucUserDet.UserDetail = oUserDetails;
                txtDuplicateIssueDate.Text = oDI.DuplicateIssueDate.ToString(Constants.DATETIME_FORMAT);
            }
            else if (SEARCH_FROM.Equals(1))
            {
                oUserDetails.MakerID = oDI.UserDetails.MakerID;
                oUserDetails.CheckerComment = oDI.UserDetails.CheckerComment;
                ucUserDet.UserDetail = oUserDetails;
            }
            else
            {
                oUserDetails.CheckerID = oDI.UserDetails.CheckerID;
                oUserDetails.CheckDate = oDI.UserDetails.CheckDate;
                oUserDetails.CheckerComment = oDI.UserDetails.CheckerComment;
                ucUserDet.UserDetail = oUserDetails;
            }

            
            Calculation();
        }
        
        private DuplicateIssue GetObject()
        {
            DuplicateIssue oDI = (DuplicateIssue)Session[Constants.SES_DUPLICATE_ISSUE];

            if (oDI != null)
            {
                if (string.IsNullOrEmpty(oDI.DuplicateIssueTransNo))
                {
                    oDI.DuplicateIssueTransNo = "-1";
                }
                oDI.DuplicateIssueAmount = Util.GetDecimalNumber(txtTotalAmount.Text);
                oDI.DuplicateIssueDate = Util.GetDateTimeByString(txtDuplicateIssueDate.Text);
                
                
                oDI.UserDetails = ucUserDet.UserDetail;
            }

            return oDI;
        }

        private void LoadDataByRegNo(string sTransactionNo, string sRegNo, string sApprovalStaus)
        {
            DuplicateIssueDAL oDuplicateIssueDAL = new DuplicateIssueDAL();
            Result oResult = null;

            if (hdButtonType.Value.Equals("S"))
            {
                //Search from Stop Payment       
                oResult = (Result)oDuplicateIssueDAL.LoadDuplicateIssueByStopPayTransNo(sTransactionNo, sApprovalStaus);
            }
            else if (hdButtonType.Value.Equals("D"))
            {
                //Search from Duplicate Issue
                oResult = (Result)oDuplicateIssueDAL.LoadDuplicateIssueByTransactionNo(sTransactionNo, sRegNo, sApprovalStaus);
            }

            TotalClear();

            if (oResult.Status)
            {
                DuplicateIssue oDuplicateIssue = (DuplicateIssue)oResult.Return;
                SetObject(oDuplicateIssue);
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
            }
            hdButtonType.Value = "";
        }

        private void TotalClear()
        {
            // Duplicate issue set in session 
            DuplicateIssue oDI = new DuplicateIssue();
            if (Session[Constants.SES_DUPLICATE_ISSUE] == null)
            {
                Session.Add(Constants.SES_DUPLICATE_ISSUE, oDI);
            }
            else
            {
                Session[Constants.SES_DUPLICATE_ISSUE] = oDI;
            }

            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            gvStopPayment.DataSource = null;
            gvStopPayment.DataBind();

            gvDenomDetail.DataSource = null;
            gvDenomDetail.DataBind();

            gvStopPayment.DataSource = null;
            gvStopPayment.DataBind();

            hdDuplicateIssueTransNo.Value = "";
            hdIssueTransNo.Value = "";
            hdRegNo.Value = "";
           // hdButtonType.Value = "";

            //Stop Payment Remove Mark Transaction No
            txtRemoveTransNo.Text = string.Empty;
            txtDuplicateIssueDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            //Issue Details
            //Util.ControlEnabled(txtStopTransNo, true);
            if (ddlBranch.Items.Count > 0)
            {
                ddlBranch.SelectedIndex = 0;
            }
            ddlDDDenom.Items.Clear();
            txtDDQuantity.Text = "1";
            txtRegNo.Text = string.Empty;
            txtIssueDate.Text = string.Empty;
            txtIssueName.Text = string.Empty;
            txtStopAmount.Text = string.Empty;
            txtTotalStopAmount.Text = string.Empty;
            txtTotalAmount.Text = string.Empty;

            ddlCustomerType.Items.Clear();
            if (ddlSpType.Items.Count > 0)
            {
                ddlSpType.SelectedIndex = 0;
            }
            txtStopDate.Text = string.Empty;
            txtStopTransNo.Text = string.Empty;

            //Customer(s) Details
            gvCustomerDetail.Enabled = true;

            //Nominee(s) Details
            gvNomDetail.Enabled = true;

            // Remarks  
            txtIssueRemarks.Text = string.Empty;
            txtStopRemarks.Text = string.Empty;

            txtDuplicateMark.Text = string.Empty;
            ucUserDet.ResetData();            
        }

        private void LoadPreviousList()
        {
            DuplicateIssueDAL oDIDAL = new DuplicateIssueDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oDIDAL.LoadUnapprovedList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);

            DataTable dtTmpList = null;

            if (oResult.Status)
            {
                dtTmpList = (DataTable)oResult.Return;

                gvData.DataSource = dtTmpList;
                gvData.DataBind();
            }

            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;
        }
        protected void btnRegSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRegNo.Text))
            {
                LoadDataByRegNo("", txtRegNo.Text, "");
            }
        }

        #endregion Util Method
    }
}