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
using System.Collections;
using SBM_BLC1.DAL.Common;

namespace SBM_WebUI.mp
{
    public partial class LMRemove : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_TRANS.LIEN_MARK_REMOVE))
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

            gvCertInfo.DataSource = null;
            gvCertInfo.DataBind();            

          
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
                    SEARCH_FROM = 4;
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    LoadDataByRegNo("", sRegNo, "1");//Load from Temp table
                    

                    #region User-Detail.
                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    oUserDetails.CheckerID = oConfig.UserName;
                    oUserDetails.CheckDate = DateTime.Now;
                    oUserDetails.MakeDate = DateTime.Now;
                    ucUserDet.UserDetail = oUserDetails;
                    #endregion User-Detail.

                }
            }
            else
            {
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                UserDetails oUserDetails = new UserDetails();
                oUserDetails.MakerID = oConfig.UserName;
                oUserDetails.MakeDate = DateTime.Now;
                ucUserDet.UserDetail = oUserDetails;
                LoadPreviousList();
            }
        }
        #endregion InitializeData


        #region Event Method...

        public void PopupLienMarkSearchLoadAction(string sLienMarkTransNo, string sRegNo, string sApprovalStaus)
        {
            if (!string.IsNullOrEmpty(sRegNo))
            {
                SEARCH_FROM = 2;
                //hdDataType.Value = sApprovalStaus;
                hdDataType.Value = "";
                LoadDataByRegNo(sLienMarkTransNo, sRegNo, sApprovalStaus);

            }
        }

        public void PopupStopPaySearchLoadAction(string sLienRemoveTransNo, string sRegNo, string sApprovalStaus)
        {
            if (!string.IsNullOrEmpty(sRegNo))
            {
                SEARCH_FROM = 1;
                hdDataType.Value = sApprovalStaus;
                LoadDataByRegNo(sLienRemoveTransNo, sRegNo, sApprovalStaus);
            }
        }
        

        //public void PopupIssueSearchLoadAction(string sRegNo, string sTransNo, string sApprovalStaus)
        //{
        //    //if (!string.IsNullOrEmpty(sRegNo))
        //    //{
        //    //    hdDataType.Value = "";
        //    //    SEARCH_FROM = 2;
        //    //    LoadDataByRegNo(sRegNo, sApprovalStaus);
        //    //}
        //}

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
                hdDataType.Value = "";
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                SEARCH_FROM = 3;
                LoadDataByRegNo("", gvRow.Cells[1].Text, "1");
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

        private void Calculate(string sAction)
        {
            LienRemove oLienRemove = (LienRemove)Session[Constants.SES_LIEN_REMOVE_MARK];

            DataTable dt = null;

            if (oLienRemove != null)
            {
                dt = oLienRemove.DtLienRemoveDetail;

                if (dt.Columns.Count <= 0)
                {
                    dt.Columns.Add(new DataColumn("SPScripID", typeof(string)));
                    dt.Columns.Add(new DataColumn("Denomination", typeof(string)));
                    dt.Columns.Add(new DataColumn("SP Series", typeof(string)));
                    dt.Columns.Add(new DataColumn("Sl No", typeof(string)));
                    dt.Columns.Add(new DataColumn("Status", typeof(string)));
                }
            }

            if (sAction.Equals("MS")) //Mark Single
            {
                #region Mark Single
                int iUpTo = Util.GetIntNumber(txtCDUpTo.Text);

                if (iUpTo > 0)
                {
                    int iSelectedIndex = ddlCDCertif.SelectedIndex;

                    for (int certificateIndex = 0; certificateIndex < iUpTo; certificateIndex++)
                    {
                        if (certificateIndex + iSelectedIndex == ddlCDCertif.Items.Count)
                        {
                            break;
                        }
                        
                        bool isExist = false;

                        if (dt.Rows.Count > 0)
                        {
                            DataRow[] selectedRow = dt.Select("SPScripID = " + ddlCDCertif.Items[certificateIndex + iSelectedIndex].Value);
                            if (selectedRow.Count() > 0)
                            {
                                isExist = true;
                            }
                        }

                        if (!isExist)
                        {                            
                            DataRow row = dt.NewRow();

                            row["SPScripID"] = ddlCDCertif.Items[certificateIndex + iSelectedIndex].Value;
                            row["Denomination"] = ddlCDDenom.SelectedItem.Value;
                            string sTmp = ddlCDCertif.Items[certificateIndex + iSelectedIndex].Text;
                            row["SP Series"] = (sTmp.Substring(0, sTmp.LastIndexOf(' '))).Trim();
                            row["Sl No"] = (sTmp.Substring(sTmp.LastIndexOf(' ') + 1, (sTmp.Length - sTmp.LastIndexOf(' ')) - 1)).Trim();
                            row["Status"] = "WAITING FOR APPROVE";
                            dt.Rows.Add(row);
                        }
                    }
                }
                #endregion
            }
            else if (sAction.Equals("MA")) //Mark All
            {
                #region Mark All
                if (oLienRemove != null)
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

                    for (int scripIndex = 0; scripIndex < oLienRemove.Issue.ScripList.Count; scripIndex++)
                    {                                                
                        DataRow row = dt.NewRow();

                        if (ddlCDDenom.SelectedItem.Value.Equals(oLienRemove.Issue.ScripList[scripIndex].Denomination.DenominationID.ToString()))
                        {
                            row["SPScripID"] = oLienRemove.Issue.ScripList[scripIndex].SPScripID;
                            row["Denomination"] = oLienRemove.Issue.ScripList[scripIndex].Denomination.DenominationID;
                            row["SP Series"] = oLienRemove.Issue.ScripList[scripIndex].SPSeries;
                            row["Sl No"] = oLienRemove.Issue.ScripList[scripIndex].SlNo;
                            row["Status"] = "WAITING FOR APPROVE";
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
            if (dt.Rows.Count > 0)
            {
                dt.DefaultView.Sort = "Sl No";    
            }

            oLienRemove.DtLienRemoveDetail = dt;
            DataTable tmpDt = dt.Copy();
            Session[Constants.SES_LIEN_REMOVE_MARK] = oLienRemove;//update session
            if (tmpDt.Columns.Contains("SPScripID"))
            {
                tmpDt.Columns.Remove("SPScripID");
            }
            gvCertInfo.DataSource = tmpDt;
            gvCertInfo.DataBind();

            decimal dTotalAmount = 0m;
            foreach (GridViewRow gvr in gvCertInfo.Rows)
            {
                dTotalAmount += Util.GetDecimalNumber(gvr.Cells[1].Text);
            }

            txtCDRemoveAmount.Text = dTotalAmount.ToString("N2");
        }              

        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");

            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.LIEN_MARK_REMOVE).PadLeft(5, '0'), false);
            }
            else
            {
                // no action
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.LIEN_MARK_REMOVE).PadLeft(5, '0'), false);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdLienRemoveTransNo.Value))
            {
                LienRemove oLienRemove = new LienRemove(hdLienRemoveTransNo.Value);
                LienDAL oLienDAL = new LienDAL();
                oLienRemove.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oLienDAL.RejectLienRemoveMark(oLienRemove);
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
            if (!string.IsNullOrEmpty(hdLienRemoveTransNo.Value))
            {
                LienRemove oLienRemove = new LienRemove(hdLienRemoveTransNo.Value);
                LienDAL oLienDAL = new LienDAL();
                oLienRemove.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oLienDAL.ApproveLienRemoveMark(oLienRemove);
                if (oResult.Status)
                {                    
                    ReportDAL rdal = new ReportDAL();

                    oResult = rdal.LienLetter(Constants.LETTER_TYPE_LIEN_REMOVE, txtUnlienTransNo.Text);

                    if (oResult.Status)
                    {
                        Session[Constants.SES_RPT_DATA] = oResult.Return;
                        Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                    }
                    //ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE, Constants.MSG_TYPE_SUCCESS);
                    ClearAfterApprove();
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND + " to Approve", Constants.MSG_TYPE_INFO);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_APPROVE, Constants.MSG_TYPE_ERROR);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            //EnableDisableControl(false);
            TotalClear();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                if (!hdDataType.Value.Equals("2"))
                {
                    LienRemove oLienRemove = GetObject();

                    LienDAL oLienDAL = new LienDAL();
                    Result oResult = oLienDAL.SaveLienRemoveMark(oLienRemove);
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
                    ucMessage.OpenMessage(Constants.MSG_APPROVED_SAVE_DATA, Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!hdDataType.Value.Equals("2"))
            {
                if (!string.IsNullOrEmpty(hdLienRemoveTransNo.Value))
                {
                    LienDAL oLienDAL = new LienDAL();
                    Result oResult = (Result)oLienDAL.DeteteLienRemoveMark(hdLienRemoveTransNo.Value);
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
                LienRemove oLienRemove = (LienRemove)Session[Constants.SES_LIEN_REMOVE_MARK];
                DDListUtil.Add(ddlCDCertif, "", "");
                if ((!string.IsNullOrEmpty(oLienRemove.LienRemoveTransNo) && oLienRemove.DtLienRemoveDetail.Rows.Count > 0) || string.IsNullOrEmpty(oLienRemove.LienRemoveTransNo))
                {
                    //Filtered by Denomination
                    List<Scrip> filteredScripList = oLienRemove.Issue.ScripList.Where(s => s.Denomination.DenominationID.ToString().Equals(ddlCDDenom.SelectedValue)).ToList();

                    for (int iScripCount = 0; iScripCount < filteredScripList.Count; iScripCount++)
                    {
                        DDListUtil.Add(ddlCDCertif, filteredScripList[iScripCount].SPSeries + " " + filteredScripList[iScripCount].SlNo, filteredScripList[iScripCount].SPScripID.ToString());
                    }
                }
            }
        }

        protected void gvCertInfo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            LienRemove oLienRemove = (LienRemove)Session[Constants.SES_LIEN_REMOVE_MARK];

            if (oLienRemove != null && gvRow != null)
            {
                oLienRemove.DtLienRemoveDetail.Rows.RemoveAt(gvRow.RowIndex);
                DataTable tmpDt = oLienRemove.DtLienRemoveDetail.Copy();
                if (tmpDt.Columns.Contains("SPScripID"))
                {
                    tmpDt.Columns.Remove("SPScripID");
                }
                gvCertInfo.DataSource = tmpDt;
                gvCertInfo.DataBind();

                Session[Constants.SES_STOPPAY_REMOVE_MARK] = oLienRemove;
            }

            decimal dTotalAmount = 0m;
            foreach (GridViewRow gvr in gvCertInfo.Rows)
            {
                dTotalAmount += Util.GetDecimalNumber(gvr.Cells[1].Text);
            }

            txtCDRemoveAmount.Text = dTotalAmount.ToString("N2");
        }


        protected void gvCertInfo_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //
        }
        #endregion Event Method...


        #region Util Method...
        private void LoadDataByRegNo(string sTransNo, string sRegNo, string sApprovalStaus)
        {
            LienDAL oLienDAL = new LienDAL();
            Result oResult = null;
            if (SEARCH_FROM.Equals(2)) //LienMark search
            {
                oResult = (Result)oLienDAL.LoadLienRemoveMarkByRegNo("", sTransNo, sRegNo, "");
            }
            else if (SEARCH_FROM.Equals(1) || SEARCH_FROM.Equals(3) || SEARCH_FROM.Equals(4))
            {
                oResult = (Result)oLienDAL.LoadLienRemoveMarkByRegNo(sTransNo, "", sRegNo, sApprovalStaus);
            }
            
            TotalClear();
            //hdIssueTransNo.Value = sRegNo;
            if (oResult.Status)
            {
                LienRemove oLienRemove = (LienRemove)oResult.Return;
                SetObject(oLienRemove);
       
                if (hdDataType.Value.Equals("2"))
                {
                    EnableDisableControl(true);
                    Util.ControlEnabled(btnReject, false);
                    Util.ControlEnabled(btnApprove, false);
                    Util.ControlEnabled(btnBack, false);

                    Util.ControlEnabled(btnReset, true);
                    Util.ControlEnabled(btnSave, true);
                    Util.ControlEnabled(btnDelete, true);

                    fsList.Visible = true;
                    btnRegSearch.Enabled = true;
                    btnUnLienSearch.Enabled = true;

                }
                else if (SEARCH_FROM.Equals(4))
                {
                    EnableDisableControl(true);
                    Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);
                }
                else
                {
                    EnableDisableControl(false);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
            }
        }

        private void LoadDataByTransactionNo(string sTransactionNo)
        {
            LienDAL oLienDAL = new LienDAL();
            Result oResult = (Result)oLienDAL.LoadLienRemoveMarkByTransactionNo(sTransactionNo);

            TotalClear();

            if (oResult.Status)
            {
                LienRemove oLienRemove = (LienRemove)oResult.Return;
                SetObject(oLienRemove);
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
            }

        }

        private void SetObject(LienRemove oLienRemove)
        {
            if (oLienRemove != null)
            {
                if (oLienRemove.Issue != null)
                {
                    hdLienRemoveTransNo.Value = oLienRemove.LienRemoveTransNo;
                    hdIssueTransNo.Value = oLienRemove.Issue.IssueTransNo;
                    hdRegNo.Value = oLienRemove.Issue.RegNo;

                    //Lien Detail
                    txtLienDate.Text = oLienRemove.Lien.LienDate.ToString(Constants.DATETIME_FORMAT);
                    txtLienTransNo.Text = oLienRemove.Lien.LienTransNo;
                    txtLienAmount.Text = oLienRemove.Lien.LienAmount.ToString();
                                        
                    //Load from LienMark search
                    if (oLienRemove.LienRemoveDate.Year.Equals(1) || oLienRemove.LienRemoveDate.Year.Equals(1900))
                    {
                        txtUnlienDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);    
                    }
                    else //Load from LienMark Remove search
                    {
                        txtUnlienDate.Text = oLienRemove.LienRemoveDate.ToString(Constants.DATETIME_FORMAT);
                    }
                    txtOurRefNo.Text = oLienRemove.OurRef;
                    txtTheirRefNo.Text = oLienRemove.TheirRef;
                    if (!string.IsNullOrEmpty(oLienRemove.LienBank))
                    {
                        txtLienBank.Text = oLienRemove.LienBank;
                    }
                    else
                    {
                        txtLienBank.Text = oLienRemove.Lien.LienBank;
                    }
                    if (!string.IsNullOrEmpty(oLienRemove.LienBankAddress))
                    {
                        txtBankAddress.Text = oLienRemove.LienBankAddress;
                    }
                    else
                    {
                        txtBankAddress.Text = oLienRemove.Lien.LienBankAddress;
                    }
                    // Remarks
                    txtRemarks.Text = oLienRemove.Remarks;

                    //Issue Details
                    txtRegNo.Text = oLienRemove.Issue.RegNo.ToString();
                    txtIssueDate.Text = oLienRemove.Issue.VersionIssueDate.ToString(Constants.DATETIME_FORMAT);
                    txtIssueName.Text = oLienRemove.Issue.IssueName.ToString();

                    ddlSpType.Text = oLienRemove.Issue.SPType.SPTypeID.Trim();
                    ddlBranch.Text = oLienRemove.Issue.Branch.BranchID.Trim();

                    DDListUtil.Assign(ddlCustomerType, oLienRemove.Issue.VersionSPPolicy.DTCustomerTypePolicy, true);
                    DDListUtil.Assign(ddlCustomerType, oLienRemove.Issue.CustomerType.CustomerTypeID);

                    // User Info
                   // ucUserDet.UserDetail = oLienRemove.UserDetails;

                    #region Customer Details
                    DataTable dtCustomerDetails = new DataTable();

                    dtCustomerDetails.Columns.Add(new DataColumn("Customer Name", typeof(string)));
                    //dtCustomerDetails.Columns.Add(new DataColumn("Customer ID", typeof(string)));
                    dtCustomerDetails.Columns.Add(new DataColumn("Address", typeof(string)));

                    DataRow rowCustomerDetails = null;

                    for (int customerCount = 0; customerCount < oLienRemove.Issue.CustomerDetailsList.Count; customerCount++)
                    {
                        rowCustomerDetails = dtCustomerDetails.NewRow();

                        rowCustomerDetails["Customer Name"] = oLienRemove.Issue.CustomerDetailsList[customerCount].CustomerName;
                        //rowCustomerDetails["Customer ID"] = oLienRemove.Issue.CustomerDetailsList[customerCount].CustomerID;
                        rowCustomerDetails["Address"] = oLienRemove.Issue.CustomerDetailsList[customerCount].Address;

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

                    DataRow rowNomineeDetail = null;

                    for (int nomineeCount = 0; nomineeCount < oLienRemove.Issue.NomineeList.Count; nomineeCount++)
                    {
                        rowNomineeDetail = dtNomineeDetail.NewRow();

                        rowNomineeDetail["Nominee Name"] = oLienRemove.Issue.NomineeList[nomineeCount].NomineeName;
                        rowNomineeDetail["Relation"] = oLienRemove.Issue.NomineeList[nomineeCount].Relation;
                        rowNomineeDetail["Address"] = oLienRemove.Issue.NomineeList[nomineeCount].Address;
                        rowNomineeDetail["Nominee Share"] = oLienRemove.Issue.NomineeList[nomineeCount].NomineeShare;

                        dtNomineeDetail.Rows.Add(rowNomineeDetail);
                    }
                    gvNomDetail.DataSource = dtNomineeDetail;
                    gvNomDetail.DataBind();
                    #endregion

                    #region Certificate Detail
                    ArrayList alScrip = new ArrayList();
                    ddlCDDenom.Items.Clear();
                    DDListUtil.Add(ddlCDDenom, "", "");
                    for (int iScripCount = 0; iScripCount < oLienRemove.Issue.ScripList.Count; iScripCount++)
                    {
                        if (!alScrip.Contains(oLienRemove.Issue.ScripList[iScripCount].Denomination.DenominationID))
                        {
                            DDListUtil.Add(ddlCDDenom, oLienRemove.Issue.ScripList[iScripCount].Denomination.DenominationID.ToString(), oLienRemove.Issue.ScripList[iScripCount].Denomination.DenominationID.ToString());
                            alScrip.Add(oLienRemove.Issue.ScripList[iScripCount].Denomination.DenominationID);
                        }
                    }
                    #endregion Certificate Detail

                    // Certificate Detail
                    txtCDRemoveAmount.Text = oLienRemove.LienRemoveAmount.ToString("N2");
                    DataTable tmpDt = oLienRemove.DtLienRemoveDetail.Copy();
                    if (tmpDt.Columns.Contains("SPScripID"))
                    {
                        tmpDt.Columns.Remove("SPScripID");
                    }
                    gvCertInfo.DataSource = tmpDt;
                    gvCertInfo.DataBind();

                    Session[Constants.SES_LIEN_REMOVE_MARK] = oLienRemove;

                    if ((SEARCH_FROM.Equals(2)) && ddlCDDenom.Items.Count <= 1)
                    {
                        ucMessage.OpenMessage("Lien Mark removed!!", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    }
                    else
                    {
                        txtUnlienTransNo.Text = oLienRemove.LienRemoveTransNo;
                    }
                    // user detail

                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    if (hdDataType.Value.Equals("2") )
                    {
                        oUserDetails.MakerID = oLienRemove.UserDetails.MakerID;
                        oUserDetails.MakeDate = oLienRemove.UserDetails.MakeDate;
                        oUserDetails.CheckerID = oLienRemove.UserDetails.CheckerID;
                        oUserDetails.CheckDate = oLienRemove.UserDetails.CheckDate;
                        oUserDetails.CheckerComment = oLienRemove.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;                        
                    }
                    if (SEARCH_FROM.Equals(4))
                    {
                        oUserDetails.MakerID = oLienRemove.UserDetails.MakerID;
                        oUserDetails.CheckerComment = oLienRemove.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;
                    }
                    else
                    {
                        oUserDetails.CheckerID = oLienRemove.UserDetails.CheckerID;
                        oUserDetails.CheckDate = oLienRemove.UserDetails.CheckDate;
                        oUserDetails.CheckerComment = oLienRemove.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;
                    }
                }
            }
        }

        private LienRemove GetObject()
        {
            LienRemove oLienRemove = (LienRemove)Session[Constants.SES_LIEN_REMOVE_MARK];

            if (oLienRemove != null)
            {
                if (string.IsNullOrEmpty(oLienRemove.LienRemoveTransNo))
                {
                    oLienRemove.LienRemoveTransNo = "-1";
                }

                oLienRemove.LienRemoveAmount = Util.GetDecimalNumber(txtCDRemoveAmount.Text);
                oLienRemove.LienRemoveDate = Util.GetDateTimeByString(txtUnlienDate.Text);
                oLienRemove.LienBank = txtLienBank.Text.ToUpper();
                oLienRemove.LienBankAddress = txtBankAddress.Text.ToUpper();
                oLienRemove.OurRef = txtOurRefNo.Text.ToUpper();
                oLienRemove.TheirRef = txtTheirRefNo.Text.ToUpper();
                oLienRemove.Remarks = txtRemarks.Text.ToUpper();

                oLienRemove.UserDetails = ucUserDet.UserDetail;
            }

            return oLienRemove;
        }

        private void TotalClear()
        {
            EnableDisableControl(false);
            // Stop Payment Remove Mark set in session 
            LienRemove oLienRemoveMark = new LienRemove();
            if (Session[Constants.SES_LIEN_REMOVE_MARK] == null)
            {
                Session.Add(Constants.SES_LIEN_REMOVE_MARK, oLienRemoveMark);
            }
            else
            {
                Session[Constants.SES_LIEN_REMOVE_MARK] = oLienRemoveMark;
            }

            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            gvCertInfo.DataSource = null;
            gvCertInfo.DataBind();

            hdLienRemoveTransNo.Value = "";
            hdIssueTransNo.Value = "";
            hdRegNo.Value = "";

            //Lien Remove Mark Transaction No
            txtLienAmount.Text = string.Empty;
            txtLienTransNo.Text = string.Empty;
            txtUnlienTransNo.Text = string.Empty;
            txtUnlienDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtRegNo.Text = string.Empty;
            
            //Issue Details
            txtIssueDate.Text = string.Empty;
            txtIssueName.Text = string.Empty;
            if (ddlBranch.Items.Count > 0)
            {
                ddlBranch.SelectedIndex = 0;
            }

            ddlCustomerType.Items.Clear();
            if (ddlSpType.Items.Count > 0)
            {
                ddlSpType.SelectedIndex = 0;
            }

            //Customer(s) Details
            gvCustomerDetail.Enabled = true;

            //Nominee(s) Details
            gvNomDetail.Enabled = true;

            //Certificate
            gvCertInfo.Enabled = true;

            // Lien Remove Detail  
            txtRemarks.Text = string.Empty;
            txtOurRefNo.Text = string.Empty;
            txtTheirRefNo.Text = string.Empty;
            txtLienBank.Text = string.Empty;
            txtBankAddress.Text = string.Empty;
            txtLienAmount.Text = string.Empty;

            //Certificate Detail
            ddlCDDenom.Items.Clear();
            ddlCDCertif.Items.Clear();
            txtCDUpTo.Text = "1";
            txtCDRemoveAmount.Text = string.Empty;

            ucUserDet.Reset();
            ucUserDet.ResetData();
        }

        private void ClearAfterApprove()
        {            
            // Stop Payment Remove Mark set in session 
            
            Session[Constants.SES_LIEN_REMOVE_MARK] = null;            

            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            gvCertInfo.DataSource = null;
            gvCertInfo.DataBind();

            hdLienRemoveTransNo.Value = "";
            hdIssueTransNo.Value = "";
            hdRegNo.Value = "";

            //Lien Remove Mark Transaction No
            txtLienAmount.Text = string.Empty;
            txtLienTransNo.Text = string.Empty;
            txtUnlienTransNo.Text = string.Empty;
            txtUnlienDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtRegNo.Text = string.Empty;

            //Issue Details
            txtIssueDate.Text = string.Empty;
            txtIssueName.Text = string.Empty;
            if (ddlBranch.Items.Count > 0)
            {
                ddlBranch.SelectedIndex = 0;
            }

            ddlCustomerType.Items.Clear();
            if (ddlSpType.Items.Count > 0)
            {
                ddlSpType.SelectedIndex = 0;
            }

            //Customer(s) Details
            gvCustomerDetail.Enabled = true;

            //Nominee(s) Details
            gvNomDetail.Enabled = true;

            //Certificate
            gvCertInfo.Enabled = true;

            // Lien Remove Detail  
            txtRemarks.Text = string.Empty;
            txtOurRefNo.Text = string.Empty;
            txtTheirRefNo.Text = string.Empty;
            txtLienBank.Text = string.Empty;
            txtBankAddress.Text = string.Empty;
            txtLienAmount.Text = string.Empty;

            //Certificate Detail
            ddlCDDenom.Items.Clear();
            ddlCDCertif.Items.Clear();
            txtCDUpTo.Text = "1";
            txtCDRemoveAmount.Text = string.Empty;

            ucUserDet.Reset();
            ucUserDet.ResetData();
        }

        public void LoadPreviousList()
        {
            LienDAL oLienDAL = new LienDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oLienDAL.LoadUnapprovedLienRemoveMarkList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);
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

        private void EnableDisableControl(bool isApproved)
        {
            // general Control

            if (isApproved)
            {                          
                //Issue Details
                //Util.ControlEnabled(txtRegNo, false);                
                                                
                //Lien Remove Mark Detail
                Util.ControlEnabled(txtOurRefNo, false);
                Util.ControlEnabled(txtTheirRefNo, false);
                Util.ControlEnabled(txtLienBank, false);
                Util.ControlEnabled(txtBankAddress, false);
                Util.ControlEnabled(txtRemarks, false);

                //Customer(s) Details
                gvCustomerDetail.Enabled = false;

                //Nominee(s) Details
                gvNomDetail.Enabled = false;

                //Certificate
                gvCertInfo.Enabled = false;

                //Certificate Detail
                Util.ControlEnabled(ddlCDDenom, false);
                Util.ControlEnabled(btnMarkAll, false);
                Util.ControlEnabled(btnRemoveAll, false);
                Util.ControlEnabled(ddlCDCertif, false);
                Util.ControlEnabled(txtCDUpTo, false);
                Util.ControlEnabled(btnMark, false);

                // user Detail
               // Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), false);

                // button
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true);

                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);

                btnRegSearch.Enabled = false;
                btnUnLienSearch.Enabled = false;

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

                //Issue Details
                //Util.ControlEnabled(txtRegNo, true);                
                
                //Lien Remove Mark Detail
                Util.ControlEnabled(txtOurRefNo, true);
                Util.ControlEnabled(txtTheirRefNo, true);
                Util.ControlEnabled(txtLienBank, true);
                Util.ControlEnabled(txtBankAddress, true);
                Util.ControlEnabled(txtRemarks, true);

                //Customer(s) Details
                gvCustomerDetail.Enabled = true;

                //Nominee(s) Details
                gvNomDetail.Enabled = true;

                //Certificate
                gvCertInfo.Enabled = true;

                //Certificate Detail
                Util.ControlEnabled(ddlCDDenom, true);
                Util.ControlEnabled(btnMarkAll, true);
                Util.ControlEnabled(btnRemoveAll, true);
                Util.ControlEnabled(ddlCDCertif, true);
                Util.ControlEnabled(txtCDUpTo, true);
                Util.ControlEnabled(btnMark, true);
              
                btnRegSearch.Enabled = true;
                btnUnLienSearch.Enabled = true;

                fsList.Visible = true;
            }
        }
        #endregion

        protected void btnRegSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRegNo.Text))
            {
                SEARCH_FROM = 2;
                hdDataType.Value = "";
                LoadDataByRegNo("", txtRegNo.Text, "");
            }
        }
    }
}
