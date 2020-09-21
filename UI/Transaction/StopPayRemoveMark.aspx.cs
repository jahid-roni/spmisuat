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

    public partial class StopPayRemoveMark : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_TRANS.STOP_PAYMENT_REMOVE_MARK))
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
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
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

                    LoadDataByRegNo("", sRegNo, "1");//Loaded from Temp table

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
                #region User-Detail.
              
                UserDetails oUserDetails = new UserDetails();
                oUserDetails.MakerID = oConfig.UserName;
                oUserDetails.MakeDate = DateTime.Now;
                ucUserDet.UserDetail = oUserDetails;
                #endregion User-Detail.

                fsList.Visible = true;
                LoadPreviousList();
            }
        }
        #endregion InitializeData

        #region Event Method...

        
         #region RegSearchClick not used at the moment
        protected void btnRegSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRegNo.Text))
            {
                SEARCH_FROM = 2;
                hdDataType.Value = "";
                LoadDataByRegNo("", txtRegNo.Text, "");
            }
        } 
        #endregion

        public void PopupStopPaySearchLoadAction(string sTransNo, string sRegNo, string sApprovalStaus)
        {
            if (!string.IsNullOrEmpty(sRegNo))
            {
                string sTmpValue = hdButtonType.Value;

                SEARCH_FROM = 1;
                hdDataType.Value = sApprovalStaus;
                LoadDataByRegNo(sTransNo, sRegNo, sApprovalStaus);     
            }
        }
        
        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, null);
        }

        protected void gvCertInfo_RowDataBound(object sender, GridViewRowEventArgs e)
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
                LoadDataByRegNo("", gvRow.Cells[1].Text, "1");
            }
        }

        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");

            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.STOP_PAYMENT_REMOVE_MARK).PadLeft(5, '0'), false);
            }
            else
            {
                // no action
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.STOP_PAYMENT_REMOVE_MARK).PadLeft(5, '0'), false);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdStopPayRemoveTransNo.Value))
            {
                StopPaymentRemove oStopPayRemove = new StopPaymentRemove(hdStopPayRemoveTransNo.Value);
                StopPaymentDAL oStopPayDAL = new StopPaymentDAL();
                oStopPayRemove.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oStopPayDAL.RejectStopPayRemoveMark(oStopPayRemove);
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
            if (!string.IsNullOrEmpty(hdStopPayRemoveTransNo.Value))
            {
                StopPaymentRemove oStopPayRemove = new StopPaymentRemove(hdStopPayRemoveTransNo.Value);
                oStopPayRemove.StopPayment.StopPaymentTransNo = txtStopTransNo.Text;
                StopPaymentDAL oStopPayDAL = new StopPaymentDAL();
                oStopPayRemove.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oStopPayDAL.ApproveStopPayRemoveMark(oStopPayRemove);
                if (oResult.Status)
                {
                    ReportDAL rdal = new ReportDAL();

                    oResult = rdal.StopPaymentLetter(Constants.LETTER_TYPE_STOP_REMOVE, txtStopPayRemoveTransNo.Text.Trim());
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
            EnableDisableControl(false);
            TotalClear();
        }

        protected void btnMark_Click(object sender, EventArgs e)
        {
            if (ddlCDCertif.SelectedItem != null)
            {
                if (!string.IsNullOrEmpty(ddlCDCertif.SelectedItem.Value))
                {
                    Calculate("MS");
                }
            }
        }
        protected void btnMarkAll_Click(object sender, EventArgs e)
        {
            if (ddlCDDenom.SelectedItem != null)
            {
                if (!string.IsNullOrEmpty(ddlCDDenom.SelectedItem.Value))
                {
                    Calculate("MA");
                }
            }
        }
        protected void btnRemoveAll_Click(object sender, EventArgs e)
        {
            Calculate("RA");
        }
        protected void btnRemoveSearch_Click(object sender, EventArgs e)
        {
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                if (ddlCDDenom.Items.Count > 1)
                {
                    StopPaymentRemove oStopPayRemove = GetObject();
                    StopPaymentDAL oSPDAL = new StopPaymentDAL();
                    oStopPayRemove.UserDetails = ucUserDet.UserDetail;
                    oStopPayRemove.UserDetails.MakeDate = DateTime.Now;
                    ucUserDet.ResetData();
                    Result oResult = oSPDAL.SaveStopPayRemoveMark(oStopPayRemove);
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
                if (!string.IsNullOrEmpty(hdStopPayRemoveTransNo.Value))
                {
                    StopPaymentDAL oStopPayDAL = new StopPaymentDAL();
                    Result oResult = (Result)oStopPayDAL.DeteteStopPayRemoveMark(hdStopPayRemoveTransNo.Value);
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
                StopPaymentRemove oStopPayRemove = (StopPaymentRemove)Session[Constants.SES_STOPPAY_REMOVE_MARK];
                DDListUtil.Add(ddlCDCertif, "", "");
                //Filtered by Denomination
                List<Scrip> filteredScripList = oStopPayRemove.Issue.ScripList.Where(s => s.Denomination.DenominationID.ToString().Equals(ddlCDDenom.SelectedValue)).ToList();

                for (int iScripCount = 0; iScripCount < filteredScripList.Count; iScripCount++)
                {
                    DDListUtil.Add(ddlCDCertif, filteredScripList[iScripCount].SPSeries + " " + filteredScripList[iScripCount].SlNo, filteredScripList[iScripCount].SPScripID.ToString());
                }
            }
        }

        private void Calculate(string sAction)
        {
            StopPaymentRemove oStopPayRemove = (StopPaymentRemove)Session[Constants.SES_STOPPAY_REMOVE_MARK];

            DataTable dt = null;

            if (oStopPayRemove != null)
            {                
                dt = oStopPayRemove.DtStopPayRemoveDetail;

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

                        DataRow[] selectedRow = dt.Select("SPScripID = " + ddlCDCertif.Items[certificateIndex + iSelectedIndex].Value);

                        if (selectedRow.Count() <= 0)
                        {
                            DataRow row = dt.NewRow();
                            row["SPScripID"] = ddlCDCertif.Items[certificateIndex + iSelectedIndex].Value;
                            row["Denomination"] = ddlCDDenom.SelectedItem.Value;
                            string sTmp = ddlCDCertif.Items[certificateIndex + iSelectedIndex].Text;
                            row["SP Series"] = (sTmp.Substring(0, sTmp.LastIndexOf(' '))).Trim();
                            row["Sl No"] = (sTmp.Substring(sTmp.LastIndexOf(' ') + 1, (sTmp.Length - sTmp.LastIndexOf(' ')) - 1)).Trim();
                            row["Status"] = "SP Removed";
                            dt.Rows.Add(row);
                        }
                    }
                }
                #endregion
            }
            else if (sAction.Equals("MA")) //Mark All
            {
                #region Mark All
                if (oStopPayRemove != null)
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

                    for (int scripIndex = 0; scripIndex < oStopPayRemove.Issue.ScripList.Count; scripIndex++)
                    {
                        DataRow row = dt.NewRow();

                        if (ddlCDDenom.SelectedItem.Value.Equals(oStopPayRemove.Issue.ScripList[scripIndex].Denomination.DenominationID.ToString()))
                        {
                            row["SPScripID"] = oStopPayRemove.Issue.ScripList[scripIndex].SPScripID;
                            row["Denomination"] = oStopPayRemove.Issue.ScripList[scripIndex].Denomination.DenominationID;
                            row["SP Series"] = oStopPayRemove.Issue.ScripList[scripIndex].SPSeries;
                            row["Sl No"] = oStopPayRemove.Issue.ScripList[scripIndex].SlNo;
                            row["Status"] = "SP Removed";
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

            oStopPayRemove.DtStopPayRemoveDetail = dt;
            Session[Constants.SES_STOPPAY_REMOVE_MARK] = oStopPayRemove;//update session

            DataTable TmpDt = dt.Copy();
            if (TmpDt.Columns.Contains("SPScripID"))
            {
                TmpDt.Columns.Remove("SPScripID");
            }

            gvCertInfo.DataSource = TmpDt;
            gvCertInfo.DataBind();

            decimal dTotalAmount = 0m;
            foreach (GridViewRow gvr in gvCertInfo.Rows)
            {
                dTotalAmount += Util.GetDecimalNumber(gvr.Cells[1].Text);
            }

            txtCDTotalAmount.Text = dTotalAmount.ToString("N2");
        }

        protected void gvCertInfo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            StopPaymentRemove oStopPayRemove = (StopPaymentRemove)Session[Constants.SES_STOPPAY_REMOVE_MARK];

            if (oStopPayRemove != null && gvRow != null)
            {
                oStopPayRemove.DtStopPayRemoveDetail.Rows.RemoveAt(gvRow.RowIndex);
                DataTable dtTmp = oStopPayRemove.DtStopPayRemoveDetail.Copy();
                if(dtTmp.Columns.Contains("SPScripID"))
                {
                    dtTmp.Columns.Remove("SPScripID");
                }
              
                gvCertInfo.DataSource = dtTmp;
                gvCertInfo.DataBind();

                Session[Constants.SES_STOPPAY_REMOVE_MARK] = oStopPayRemove;
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

        #endregion Event Method...


        #region Util Method...
        private void LoadDataByRegNo(string sTransNo,string sRegNo, string sApprovalStaus)
        {
            StopPaymentDAL oStopPayDAL = new StopPaymentDAL();
            Result oResult = null;
            
            
            if (hdButtonType.Value.Equals("S"))//StopPayment search
            {
                oResult = (Result)oStopPayDAL.LoadStopPayRemoveMarkByRegNo("", sTransNo, sRegNo, "");
            }
            else if (hdButtonType.Value.Equals("D")|| SEARCH_FROM.Equals(3)) //StopPayment Remove search
            {
                oResult = (Result)oStopPayDAL.LoadStopPayRemoveMarkByRegNo(sTransNo, "", sRegNo, sApprovalStaus);
            }
            
            TotalClear();

            if (oResult.Status)
            {
                StopPaymentRemove oStopPayRemove = (StopPaymentRemove)oResult.Return;

                SetObject(oStopPayRemove);

                if (hdDataType.Value.Equals("2") && hdButtonType.Value.Equals("D"))
                {
                    EnableDisableControl(true);
                    Util.ControlEnabled(btnReject, false);
                    Util.ControlEnabled(btnApprove, false);
                    Util.ControlEnabled(btnBack, false);

                    Util.ControlEnabled(btnReset, true);
                    Util.ControlEnabled(btnSave, true);
                    Util.ControlEnabled(btnDelete, true);

                    btnRegSearch.Enabled = true;

                    btnRemoveSearch.Enabled = true;

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
                }
                hdButtonType.Value = "";
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
            }

        }

        private void SetObject(StopPaymentRemove oStopPayRemove)
        {
            if (oStopPayRemove != null)
            {
                if (oStopPayRemove.Issue != null)
                {
                    hdStopPayRemoveTransNo.Value = oStopPayRemove.StopPaymentRemoveTransNo;
                    hdIssueTransNo.Value = oStopPayRemove.Issue.IssueTransNo;
                    hdRegNo.Value = oStopPayRemove.Issue.RegNo;

                    txtRegNo.Text = oStopPayRemove.Issue.RegNo.ToString();
                    ddlSpType.Text = oStopPayRemove.Issue.SPType.SPTypeID.Trim();
                    ddlBranch.Text = oStopPayRemove.Issue.Branch.BranchID.Trim();
                    txtIssueName.Text = oStopPayRemove.Issue.IssueName;

                    //Loading from StopPayment Search
                    if (oStopPayRemove.StopPaymentRemoveDate.Year.Equals(1) || oStopPayRemove.StopPaymentRemoveDate.Year.Equals(1900))
                    {
                        txtRemoveDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
                    }
                    else //Loading from StopPaymentRemove Search
                    {
                        txtRemoveDate.Text = oStopPayRemove.StopPaymentRemoveDate.ToString(Constants.DATETIME_FORMAT);
                    }

                    txtStopPayRemoveTransNo.Text = oStopPayRemove.StopPaymentRemoveTransNo.ToString();

                    //Issue Details
                    DDListUtil.Assign(ddlSpType, oStopPayRemove.Issue.SPType.SPTypeID);
                    DDListUtil.Assign(ddlBranch, oStopPayRemove.Issue.Branch.BranchID);
                    txtRegNo.Text = oStopPayRemove.Issue.RegNo;
                    txtIssueDate.Text = oStopPayRemove.Issue.VersionIssueDate.ToString(Constants.DATETIME_FORMAT);
                    txtStopDate.Text = oStopPayRemove.StopPayment.StopPaymentDate.ToString(Constants.DATETIME_FORMAT);
                    txtStopAmount.Text = oStopPayRemove.StopPayment.StopPaymentAmount.ToString("N2");
                    txtStopTransNo.Text = oStopPayRemove.StopPayment.StopPaymentTransNo;
                    DDListUtil.Assign(ddlCustomerType, oStopPayRemove.Issue.VersionSPPolicy.DTCustomerTypePolicy, true);
                    DDListUtil.Assign(ddlCustomerType, oStopPayRemove.Issue.CustomerType.CustomerTypeID);

                    // Remarks
                    txtStopRemarks.Text = oStopPayRemove.StopPayment.Remarks;
                    txtRemoveRemarks.Text = oStopPayRemove.Remarks;
                    
                    #region Customer Details
                    DataTable dtCustomerDetails = new DataTable();

                    dtCustomerDetails.Columns.Add(new DataColumn("Customer Name", typeof(string)));
                    //dtCustomerDetails.Columns.Add(new DataColumn("Customer ID", typeof(string)));
                    dtCustomerDetails.Columns.Add(new DataColumn("Address", typeof(string)));
                    dtCustomerDetails.Columns.Add(new DataColumn("Phone", typeof(string)));

                    DataRow rowCustomerDetails = null;

                    for (int customerCount = 0; customerCount < oStopPayRemove.Issue.CustomerDetailsList.Count; customerCount++)
                    {
                        rowCustomerDetails = dtCustomerDetails.NewRow();

                        rowCustomerDetails["Customer Name"] = oStopPayRemove.Issue.CustomerDetailsList[customerCount].CustomerName;
                        //rowCustomerDetails["Customer ID"] = oStopPayRemove.Issue.CustomerDetailsList[customerCount].CustomerID;
                        rowCustomerDetails["Address"] = oStopPayRemove.Issue.CustomerDetailsList[customerCount].Address;
                        rowCustomerDetails["Phone"] = oStopPayRemove.Issue.CustomerDetailsList[customerCount].Phone;

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

                    for (int nomineeCount = 0; nomineeCount < oStopPayRemove.Issue.NomineeList.Count; nomineeCount++)
                    {
                        rowNomineeDetail = dtNomineeDetail.NewRow();

                        rowNomineeDetail["Nominee Name"] = oStopPayRemove.Issue.NomineeList[nomineeCount].NomineeName;
                        rowNomineeDetail["Relation"] = oStopPayRemove.Issue.NomineeList[nomineeCount].Relation;
                        rowNomineeDetail["Address"] = oStopPayRemove.Issue.NomineeList[nomineeCount].Address;
                        rowNomineeDetail["Nominee Share"] = oStopPayRemove.Issue.NomineeList[nomineeCount].NomineeShare;
                        rowNomineeDetail["Amount"] = oStopPayRemove.Issue.NomineeList[nomineeCount].Amount;

                        dtNomineeDetail.Rows.Add(rowNomineeDetail);
                    }
                    gvNomDetail.DataSource = dtNomineeDetail;
                    gvNomDetail.DataBind();
                    #endregion

                    #region Certificate Detail
                    ArrayList alScrip = new ArrayList();
                    ddlCDDenom.Items.Clear();
                    DDListUtil.Add(ddlCDDenom, "", "");
                    if (!(hdDataType.Value.Equals("2") && hdButtonType.Value.Equals("D")))
                    {
                        for (int iScripCount = 0; iScripCount < oStopPayRemove.Issue.ScripList.Count; iScripCount++)
                        {
                            if (!alScrip.Contains(oStopPayRemove.Issue.ScripList[iScripCount].Denomination.DenominationID))
                            {
                                DDListUtil.Add(ddlCDDenom, oStopPayRemove.Issue.ScripList[iScripCount].Denomination.DenominationID.ToString(), oStopPayRemove.Issue.ScripList[iScripCount].Denomination.DenominationID.ToString());
                                alScrip.Add(oStopPayRemove.Issue.ScripList[iScripCount].Denomination.DenominationID);
                            }
                        }
                    }
                    
                    #endregion Certificate Detail

                    // Certificate Detail
                    txtCDTotalAmount.Text = oStopPayRemove.StopPaymentRemoveAmount.ToString("N2");
                    DataTable dtTmp = oStopPayRemove.DtStopPayRemoveDetail;
                    if (dtTmp.Columns.Contains("SPScripID"))
                    {
                        dtTmp.Columns.Remove("SPScripID");
                    }
                    gvCertInfo.DataSource = dtTmp;
                    gvCertInfo.DataBind();

                    #region Set User Details
                    Session[Constants.SES_STOPPAY_REMOVE_MARK] = oStopPayRemove;
                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    //Prompt message if already Stop Mark Removed                    
                    if ((SEARCH_FROM.Equals(1)) && hdDataType.Value.Equals("2") && ddlCDDenom.Items.Count <= 1)
                    {
                        ucMessage.OpenMessage("Stop Mark removed!!", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    }
                    else if (hdButtonType.Value.Equals("S")) //Stop Payment Approve search
                    {
                        if (ddlCDDenom.Items.Count <= 1)
                        {
                            ucMessage.OpenMessage("Stop Mark removed!!", Constants.MSG_TYPE_INFO);
                            ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        }
                    }
                    else if (hdDataType.Value.Equals("2") && hdButtonType.Value.Equals("D")) //StopPaymentRemove Approve search
                    {
                        oUserDetails.MakerID = oStopPayRemove.UserDetails.MakerID;
                        oUserDetails.MakeDate = oStopPayRemove.UserDetails.MakeDate;
                        oUserDetails.CheckerID = oStopPayRemove.UserDetails.CheckerID;
                        oUserDetails.CheckDate = oStopPayRemove.UserDetails.CheckDate;
                        oUserDetails.CheckerComment = oStopPayRemove.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;
                        txtRemoveDate.Text = oStopPayRemove.StopPaymentRemoveDate.ToString(Constants.DATETIME_FORMAT);
                    }
                    else if ((hdDataType.Value.Equals("1") && SEARCH_FROM.Equals(1)) || (SEARCH_FROM.Equals(2) && hdDataType.Value.Equals("1")))
                    {
                        oUserDetails.CheckerID = oStopPayRemove.UserDetails.CheckerID;
                        oUserDetails.CheckDate = oStopPayRemove.UserDetails.CheckDate;
                        oUserDetails.CheckerComment = oStopPayRemove.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;
                    }
                    else if (SEARCH_FROM.Equals(3)) // Coming from Approval page
                    {
                        oUserDetails.MakerID = oStopPayRemove.UserDetails.MakerID;
                        oUserDetails.CheckerComment = oStopPayRemove.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;
                    } 
                    #endregion

                    Calculate("");
                }
            }
        }

        private StopPaymentRemove GetObject()
        {
            StopPaymentRemove oStopPayRemove = (StopPaymentRemove)Session[Constants.SES_STOPPAY_REMOVE_MARK];

            if (oStopPayRemove != null)
            {
                if (string.IsNullOrEmpty(oStopPayRemove.StopPaymentRemoveTransNo))
                {
                    oStopPayRemove.StopPaymentRemoveTransNo = "-1";
                }
                oStopPayRemove.Remarks = txtRemoveRemarks.Text;
                oStopPayRemove.StopPaymentRemoveAmount = Util.GetDecimalNumber(txtCDTotalAmount.Text);
                oStopPayRemove.StopPaymentRemoveDate = Util.GetDateTimeByString(txtRemoveDate.Text);

                oStopPayRemove.UserDetails = ucUserDet.UserDetail;
            }

            return oStopPayRemove;
        }

        private void TotalClear()
        {
            EnableDisableControl(false);
            // Stop Payment Remove Mark set in session 
            StopPaymentRemove oStopPayRemoveMark = new StopPaymentRemove();
            if (Session[Constants.SES_STOPPAY_MARK] == null)
            {
                Session.Add(Constants.SES_STOPPAY_MARK, oStopPayRemoveMark);
            }
            else
            {
                Session[Constants.SES_STOPPAY_MARK] = oStopPayRemoveMark;
            }

            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            gvCertInfo.DataSource = null;
            gvCertInfo.DataBind();

            hdStopPayRemoveTransNo.Value = "";
            hdIssueTransNo.Value = "";
            hdRegNo.Value = "";
           // hdButtonType.Value = "";

            //Stop Payment Remove Mark Transaction No
            txtStopPayRemoveTransNo.Text = string.Empty;
            txtRemoveDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            //Issue Details
            Util.ControlEnabled(txtStopTransNo, true);
            if (ddlBranch.Items.Count > 0)
            {
                ddlBranch.SelectedIndex = 0;
            }
            txtRegNo.Text = string.Empty;
            txtIssueDate.Text = string.Empty;
            txtIssueName.Text = string.Empty;
            txtStopAmount.Text = string.Empty;
            ddlCustomerType.Items.Clear();
            if (ddlSpType.Items.Count > 0)
            {
                ddlSpType.SelectedIndex = 0;
            }
            txtStopDate.Text = string.Empty;
            txtStopTransNo.Text = string.Empty;
          
            // Remarks  
            txtRemoveRemarks.Text = string.Empty;
            txtStopRemarks.Text = string.Empty;

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
            // Stop Payment Remove Mark set in session 
            Session[Constants.SES_STOPPAY_MARK] = null;
            
            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            gvCertInfo.DataSource = null;
            gvCertInfo.DataBind();

            hdStopPayRemoveTransNo.Value = "";
            hdIssueTransNo.Value = "";
            hdRegNo.Value = "";
            // hdButtonType.Value = "";

            //Stop Payment Remove Mark Transaction No
            txtStopPayRemoveTransNo.Text = string.Empty;
            txtRemoveDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            //Issue Details
            Util.ControlEnabled(txtStopTransNo, true);
            if (ddlBranch.Items.Count > 0)
            {
                ddlBranch.SelectedIndex = 0;
            }
            txtRegNo.Text = string.Empty;
            txtIssueDate.Text = string.Empty;
            txtIssueName.Text = string.Empty;
            txtStopAmount.Text = string.Empty;
            ddlCustomerType.Items.Clear();
            if (ddlSpType.Items.Count > 0)
            {
                ddlSpType.SelectedIndex = 0;
            }
            txtStopDate.Text = string.Empty;
            txtStopTransNo.Text = string.Empty;
            
            // Remarks  
            txtRemoveRemarks.Text = string.Empty;
            txtStopRemarks.Text = string.Empty;

            //Certificate Detail
            ddlCDDenom.Items.Clear();
            ddlCDCertif.Items.Clear();
            txtCDUpTo.Text = "1";
            txtCDTotalAmount.Text = string.Empty;

            ucUserDet.Reset();
            ucUserDet.ResetData();
        }

        public void LoadPreviousList()
        {
            StopPaymentDAL oStopPayDAL = new StopPaymentDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oStopPayDAL.LoadUnapprovedPaymentRemoveMarkList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);
            
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

            //Stop Transaction Details
            Util.ControlEnabled(txtStopPayRemoveTransNo, false);
            Util.ControlEnabled(txtStopTransNo, false);
            Util.ControlEnabled(txtStopDate, false);

            //Issue Details
            Util.ControlEnabled(ddlBranch, false);
            Util.ControlEnabled(txtIssueDate, false);
            Util.ControlEnabled(txtStopAmount, false);
            Util.ControlEnabled(ddlCustomerType, false);
            Util.ControlEnabled(ddlSpType, false);
            Util.ControlEnabled(txtIssueName, false);

            //Remarks
            Util.ControlEnabled(txtStopRemarks, false);

            //Certificate
            Util.ControlEnabled(txtCDTotalAmount, false);

            if (isApproved)
            {                
                //Issue Details
                //Util.ControlEnabled(txtRegNo, false);

                //Remarks
                Util.ControlEnabled(txtRemoveRemarks, false);

                //gvData.Enabled = false;
                                
                //Customer(s) Details
                gvCustomerDetail.Enabled = false;

                //Nominee(s) Details
                gvNomDetail.Enabled = false;

                //Certificate
                gvCertInfo.Enabled = false;

                //Certificate Detail
                Util.ControlEnabled(ddlCDDenom, false);
                Util.ControlEnabled(ddlCDCertif, false);
                Util.ControlEnabled(txtCDUpTo, false);
                
                btnMark.Enabled = false;
                btnMarkAll.Enabled = false;
                btnRemoveAll.Enabled = false;

                //Util.ControlEnabled(btnMarkAll, false);
                //Util.ControlEnabled(btnRemoveAll, false);
                //Util.ControlEnabled(btnMark, false);

                // user Detail
                //Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);

                // button
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true);

                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);

                btnRegSearch.Enabled = false;
                btnRemoveSearch.Enabled = false;


                fsList.Visible = false;
            }
            else
            {                                                
                //Issue Details
                //Util.ControlEnabled(txtRegNo, true);

                // Remarks  
                Util.ControlEnabled(txtRemoveRemarks, true);

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

                //Util.ControlEnabled(btnMarkAll, true);
                //Util.ControlEnabled(btnRemoveAll, true);
               // Util.ControlEnabled(btnMark, true);
                btnMarkAll.Enabled = true;
                btnMark.Enabled = true;
                btnRemoveAll.Enabled = true;

                // button 
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);

                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);

                btnRegSearch.Enabled = true; 
                btnRemoveSearch.Enabled = true;

                fsList.Visible = true;
            }
        }
        #endregion
    }
}