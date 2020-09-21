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
using SBM_BLC1.Entity.SecurityAdmin;
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.DAL.Common;

namespace SBM_WebUI.mp
{
    public partial class LM : System.Web.UI.Page
    {

        #region Local Variable
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_REG_NO = "sRegNo";
        public const string OBJ_PAGE_ID = "sPageID";
        public int SEARCH_FROM = 0;
        public Hashtable htblControlsList = new Hashtable();  
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_TRANS.LIEN_MARK))
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
            //SIssue.Type = Convert.ToString((int)Constants.SEARCH_ISSUE.LIEN_MARK);

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
                   
                    LoadDataByRegNo("", sRegNo, "1");//query from Temp

                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    oUserDetails.CheckerID = oConfig.UserName;
                    oUserDetails.CheckDate = DateTime.Now;
                    oUserDetails.MakeDate = DateTime.Now;
                    ucUserDet.UserDetail = oUserDetails;
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

               // fsList.Visible = true;
                LoadPreviousList();
            }
        }
        #endregion InitializeData

        #region Event Method...

        protected void btnRegSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRegNo.Text))
            {
                hdDataType.Value = "";
                LoadDataByRegNo("", txtRegNo.Text, "2");
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
                hdDataType.Value = "";
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
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

        public void PopupLienMarkSearchLoadAction(string sLienMarkTransNo, string sRegNo, string sApprovalStaus)
         {
            if (!string.IsNullOrEmpty(sRegNo))
            {
                SEARCH_FROM = 1;
                hdDataType.Value = sApprovalStaus;
                LoadDataByRegNo(sLienMarkTransNo, sRegNo, sApprovalStaus);
                
            }
        }
        public void PopupIssueSearchLoadAction(string sRegNo, string sLienMarkTransNo, string sApprovalStaus)
        {
            if (!string.IsNullOrEmpty(sRegNo))
            {
                SEARCH_FROM = 2;
                hdDataType.Value = "";
                //LoadDataByRegNo(sLienMarkTransNo, sRegNo, sApprovalStaus);
                LoadDataByRegNo("", sRegNo, sApprovalStaus);
            }
        }


        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");

            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.LIEN_MARK).PadLeft(5, '0'), false);
            }
            else
            {
                txtRegNo.Focus();
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.LIEN_MARK).PadLeft(5, '0'), false);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdLienTransNo.Value))
            {
                Lien oLien = new Lien(hdLienTransNo.Value);
                LienDAL oLienDAL = new LienDAL();
                oLien.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oLienDAL.RejectLienMark(oLien);
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
            if (!string.IsNullOrEmpty(hdLienTransNo.Value))
            {
                Lien oLien = new Lien(hdLienTransNo.Value);
                LienDAL oLienDAL = new LienDAL();
                oLien.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oLienDAL.ApproveLienMark(oLien);
                if (oResult.Status)
                {                    
                    ReportDAL rdal = new ReportDAL();

                    oResult = rdal.LienLetter(Constants.LETTER_TYPE_LIEN, txtLienTransNo.Text);

                    if (oResult.Status)
                    {
                        Session[Constants.SES_RPT_DATA] = oResult.Return;
                        Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                    }
                    ClearAfterApprove();
                    //ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE, Constants.MSG_TYPE_SUCCESS);
                    //Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.LIEN_MARK).PadLeft(5, '0'), false);                    
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                if (!hdDataType.Value.Equals("2"))
                {
                    Lien oLien = GetObject();
                    oLien.UserDetails = ucUserDet.UserDetail;
                    oLien.UserDetails.MakeDate = DateTime.Now;
                    ucUserDet.ResetData();
                    LienDAL oLienDAL = new LienDAL();
                    Result oResult = oLienDAL.SaveLienMark(oLien);

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
                if (!string.IsNullOrEmpty(hdLienTransNo.Value))
                {
                    LienDAL oStopPayDAL = new LienDAL();
                    Result oResult = (Result)oStopPayDAL.DeteteLienMark(hdLienTransNo.Value);

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
                Lien oLien = (Lien)Session[Constants.SES_LIEN_MARK];
                DDListUtil.Add(ddlCDCertif, "", "");
                //Filtered by Denomination
                List<Scrip> filteredScripList = oLien.Issue.ScripList.Where(s => s.Denomination.DenominationID.ToString().Equals(ddlCDDenom.SelectedValue)).ToList();

                for (int iScripCount = 0; iScripCount < filteredScripList.Count; iScripCount++)
                {
                    DDListUtil.Add(ddlCDCertif, filteredScripList[iScripCount].SPSeries + " " + filteredScripList[iScripCount].SlNo, filteredScripList[iScripCount].SPScripID.ToString() + ":" + filteredScripList[iScripCount].OlsStatus.ToString());
                }
            }
        }

        protected void gvCertInfo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            Lien oLien = (Lien)Session[Constants.SES_LIEN_MARK];

            if (oLien != null && gvRow != null)
            {
                oLien.DtLienDetails.Rows.RemoveAt(gvRow.RowIndex);
                DataTable dtTmp = oLien.DtLienDetails.Copy();
                if (dtTmp.Columns.Contains("SPScripID"))
                {
                    dtTmp.Columns.Remove("SPScripID");
                }
                gvCertInfo.DataSource = dtTmp;
                gvCertInfo.DataBind();

                Session[Constants.SES_LIEN_MARK] = oLien;
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

        private void LoadDataByRegNo(string sLienMarkTransNo, string sRegNo, string sApprovalStaus)
        {
            LienDAL oLienDAL = new LienDAL();
            Result oResult = (Result)oLienDAL.LoadLienMarkByRegNo(sLienMarkTransNo, sRegNo, sApprovalStaus);
            TotalClear();
            if (oResult.Status)
            {
                Lien oLien = (Lien)oResult.Return;
                SetObject(oLien);
                
                if (hdDataType.Value.Equals("2"))
                {
                    EnableDisableControl(true);
                    Util.ControlEnabled(btnReset, true);
                    Util.ControlEnabled(btnSave, true);
                    Util.ControlEnabled(btnDelete, true);
                    btnRegSearch.Enabled = true;
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

                if (oLien.Issue.IssueTransNo == "")
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }
        #endregion Event Method...


        private void SetObject(Lien oLien)
        {
            if (oLien != null)
            {
                if (oLien.Issue != null)
                {
                    hdLienTransNo.Value = oLien.LienTransNo;
                    hdIssueTransNo.Value = oLien.Issue.IssueTransNo;
                    hdRegNo.Value = oLien.Issue.RegNo;

                    //Lien Detail
                    txtRegNo.Text = oLien.Issue.RegNo;
                    txtLienTransNo.Text = oLien.LienTransNo;
                    txtIssueDate.Text = oLien.Issue.VersionIssueDate.ToString(Constants.DATETIME_FORMAT);
                    //txtLienDate.Text = oLien.LienDate.ToString(Constants.DATETIME_FORMAT);
                    txtLienDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
                    txtRemarks.Text = oLien.Remarks;
                    txtOurRefNo.Text = oLien.OurRef;
                    txtTheirRefNo.Text = oLien.TheirRef;
                    txtLienBank.Text = oLien.LienBank;
                    txtBankAddress.Text = oLien.LienBankAddress;
                    txtTotalAmount.Text = oLien.Issue.IssueAmount.ToString();
                    txtIssueName.Text = oLien.Issue.IssueName.ToString();

                    ddlSpType.Text = oLien.Issue.SPType.SPTypeID.Trim();
                    ddlBranch.Text = oLien.Issue.Branch.BranchID.Trim();
                    //ddlCustomerType.Text = oLien.Issue.CustomerType.CustomerTypeID.Trim();
                    DDListUtil.Assign(ddlCustomerType, oLien.Issue.VersionSPPolicy.DTCustomerTypePolicy, true);
                    DDListUtil.Assign(ddlCustomerType, oLien.Issue.CustomerType.CustomerTypeID);
                    ddlYear.Text = DateTime.Now.Year.ToString();

                    #region Customer Details
                    DataTable dtCustomerDetails = new DataTable();

                    dtCustomerDetails.Columns.Add(new DataColumn("Customer Name", typeof(string)));
                    //dtCustomerDetails.Columns.Add(new DataColumn("Customer ID", typeof(string)));
                    dtCustomerDetails.Columns.Add(new DataColumn("Address", typeof(string)));
                    dtCustomerDetails.Columns.Add(new DataColumn("Phone", typeof(string)));

                    DataRow rowCustomerDetails = null;

                    for (int customerCount = 0; customerCount < oLien.Issue.CustomerDetailsList.Count; customerCount++)
                    {
                        rowCustomerDetails = dtCustomerDetails.NewRow();

                        rowCustomerDetails["Customer Name"] = oLien.Issue.CustomerDetailsList[customerCount].CustomerName;
                        //rowCustomerDetails["Customer ID"] = oLien.Issue.CustomerDetailsList[customerCount].CustomerID;
                        rowCustomerDetails["Address"] = oLien.Issue.CustomerDetailsList[customerCount].Address;

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

                    for (int nomineeCount = 0; nomineeCount < oLien.Issue.NomineeList.Count; nomineeCount++)
                    {
                        rowNomineeDetail = dtNomineeDetail.NewRow();

                        rowNomineeDetail["Nominee Name"] = oLien.Issue.NomineeList[nomineeCount].NomineeName;
                        rowNomineeDetail["Relation"] = oLien.Issue.NomineeList[nomineeCount].Relation;
                        rowNomineeDetail["Address"] = oLien.Issue.NomineeList[nomineeCount].Address;
                        rowNomineeDetail["Nominee Share"] = oLien.Issue.NomineeList[nomineeCount].NomineeShare;

                        dtNomineeDetail.Rows.Add(rowNomineeDetail);
                    }
                    gvNomDetail.DataSource = dtNomineeDetail;
                    gvNomDetail.DataBind();
                    #endregion

                    #region Certificate Detail
                    // Certificate Detail
                    ArrayList alScrip = new ArrayList();
                    ddlCDDenom.Items.Clear();
                    DDListUtil.Add(ddlCDDenom, "", "");
                    for (int iScripCount = 0; iScripCount < oLien.Issue.ScripList.Count; iScripCount++)
                    {
                        if (!alScrip.Contains(oLien.Issue.ScripList[iScripCount].Denomination.DenominationID))
                        {
                            DDListUtil.Add(ddlCDDenom, oLien.Issue.ScripList[iScripCount].Denomination.DenominationID.ToString(), oLien.Issue.ScripList[iScripCount].Denomination.DenominationID.ToString());
                            alScrip.Add(oLien.Issue.ScripList[iScripCount].Denomination.DenominationID);
                        }
                    }

                    DataTable dtLienDetails = oLien.DtLienDetails.Copy();
                    if (dtLienDetails.Columns.Contains("OldStatus"))
                    {
                        dtLienDetails.Columns.Remove("OldStatus");
                    }
                    if (dtLienDetails.Columns.Contains("SPScripID"))
                    {
                        dtLienDetails.Columns.Remove("SPScripID");
                    }
                    gvCertInfo.DataSource = dtLienDetails;
                    gvCertInfo.DataBind();

                    for (int i = 0; i < gvCertInfo.Rows.Count; i++)
                    {
                        HiddenField hdObj = (HiddenField)gvCertInfo.Rows[i].FindControl("hdOldStatus");
                        if (hdObj != null)
                        {
                            hdObj.Value = oLien.DtLienDetails.Rows[i]["OldStatus"].ToString();
                        }
                    }
                    #endregion
                                        
                    Session[Constants.SES_LIEN_MARK] = oLien;
                    // user detail
                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    if(hdDataType.Value.Equals("2"))
                    {
                        oUserDetails.MakerID = oLien.UserDetails.MakerID;
                        oUserDetails.MakeDate = oLien.UserDetails.MakeDate;
                        oUserDetails.CheckerID = oLien.UserDetails.CheckerID;
                        oUserDetails.CheckDate = oLien.UserDetails.CheckDate;
                        oUserDetails.CheckerComment = oLien.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;
                        txtLienDate.Text = oLien.LienDate.ToString(Constants.DATETIME_FORMAT);

                    }
                    else if (SEARCH_FROM.Equals(3))
                    {
                        oUserDetails.MakerID = oLien.UserDetails.MakerID;
                        oUserDetails.CheckerComment = oLien.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;
                    }
                    else
                    {
                        oUserDetails.CheckerID = oLien.UserDetails.CheckerID;
                        oUserDetails.CheckDate = oLien.UserDetails.CheckDate;
                        oUserDetails.CheckerComment = oLien.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;
                    }
                }
            }
        }

        private Lien GetObject()
        {
            Lien oLien = (Lien)Session[Constants.SES_LIEN_MARK];

            if (oLien != null)
            {
                if (string.IsNullOrEmpty(oLien.LienTransNo))
                {
                    oLien.LienTransNo = "-1";
                }
                oLien.OurRef = txtOurRefNo.Text.ToUpper();
                oLien.TheirRef = txtTheirRefNo.Text.ToUpper();
                oLien.LienBank = txtLienBank.Text.ToUpper();
                oLien.LienBankAddress = txtBankAddress.Text.ToUpper();
                oLien.Remarks = txtRemarks.Text.ToUpper();
                oLien.LienAmount = Util.GetDecimalNumber(txtCDTotalAmount.Text);
                oLien.LienDate = Util.GetDateTimeByString(txtLienDate.Text);

                oLien.UserDetails = ucUserDet.UserDetail;
            }

            return oLien;
        }

        public void LoadPreviousList()
        {
            LienDAL oLienDAL = new LienDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oLienDAL.LoadUnapprovedLienMarkList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);

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

        private void Calculate(string sAction)
        {
            Lien oLien = (Lien)Session[Constants.SES_LIEN_MARK];

            DataTable dt = null;
            //ArrayList alOldStatus = new ArrayList();

            if (oLien != null)
            {
                dt = oLien.DtLienDetails;
                
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
                    string sCertifVal = ddlCDCertif.SelectedItem.Value;
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
                            row["Status"] = "Liened";
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
                if (oLien != null)
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

                    for (int scripCount = 0; scripCount < oLien.Issue.ScripList.Count; scripCount++)
                    {                        
                        DataRow row = dt.NewRow();

                        if (ddlCDDenom.SelectedItem.Value.Equals(oLien.Issue.ScripList[scripCount].Denomination.DenominationID.ToString()))
                        {
                            row["SPScripID"] = oLien.Issue.ScripList[scripCount].SPScripID;
                            row["Denomination"] = oLien.Issue.ScripList[scripCount].Denomination.DenominationID;
                            row["SP Series"] = oLien.Issue.ScripList[scripCount].SPSeries;
                            row["Sl No"] = oLien.Issue.ScripList[scripCount].SlNo;
                            row["Status"] = "Liened";
                            row["OldStatus"] = oLien.Issue.ScripList[scripCount].OlsStatus;
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

            DataTable tmpDt = dt.Copy();

            oLien.DtLienDetails = dt;
            Session[Constants.SES_LIEN_MARK] = oLien;
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
                    hdObj.Value = oLien.DtLienDetails.Rows[i]["OldStatus"].ToString();
                }
            }

            decimal dTotalAmount = 0m;
            foreach (GridViewRow gvr in gvCertInfo.Rows)
            {
                dTotalAmount += Util.GetDecimalNumber(gvr.Cells[1].Text);
            }
            txtCDTotalAmount.Text = dTotalAmount.ToString("N2");
            //object obj = tmpDt.Compute("SUM(Denomination)", "");
            //txtCDTotalAmount.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString("N2") : "0";
        }

        private void TotalClear()
        {
            // Lien Mark set in session 
            EnableDisableControl(false);
            Lien oLien = new Lien();
            if (Session[Constants.SES_LIEN_MARK] == null)
            {
                Session.Add(Constants.SES_LIEN_MARK, oLien);
            }
            else
            {
                Session[Constants.SES_LIEN_MARK] = oLien;
            }

            gvCertInfo.DataSource = null;
            gvCertInfo.DataBind();

            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            hdLienTransNo.Value = "";
            hdIssueTransNo.Value = "";
            hdRegNo.Value = "";

            //Issue Details.. year
            if (ddlYear.Items.Count > 0)
            {
                ddlYear.SelectedIndex = 0;
            }
            else
            {
                for (int i = 1990; i < DateTime.Now.Year +1 +1; i++)
                {
                    DDListUtil.Add(ddlYear, i.ToString(), i.ToString());
                }
                ddlYear.SelectedIndex = 0;
            }
            txtTotalAmount.Text = string.Empty;
            txtIssueName.Text = string.Empty;
            ddlCustomerType.Items.Clear();
            if (ddlSpType.Items.Count > 0)
            {
                ddlSpType.SelectedIndex = 0;
            }
            if (ddlBranch.Items.Count > 0)
            {
                ddlBranch.SelectedIndex = 0;
            }
            txtLienTransNo.Text = string.Empty;
            txtIssueDate.Text = string.Empty;
            txtRegNo.Text = string.Empty;
            txtLienDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            // Lien Detail 
            txtRemarks.Text = string.Empty;
            txtOurRefNo.Text = string.Empty;
            txtTheirRefNo.Text = string.Empty;
            txtTotalAmount.Text = string.Empty;
            txtLienBank.Text = string.Empty;
            txtBankAddress.Text = string.Empty;
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
            // Lien Mark set in session             
                       
            Session[Constants.SES_LIEN_MARK] = null;            

            gvCertInfo.DataSource = null;
            gvCertInfo.DataBind();

            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            hdLienTransNo.Value = "";
            hdIssueTransNo.Value = "";
            hdRegNo.Value = "";

            //Issue Details.. year
            if (ddlYear.Items.Count > 0)
            {
                ddlYear.SelectedIndex = 0;
            }
            else
            {
                for (int i = 1990; i < DateTime.Now.Year +1 +1; i++)
                {
                    DDListUtil.Add(ddlYear, i.ToString(), i.ToString());
                }
                ddlYear.SelectedIndex = 0;
            }
            txtTotalAmount.Text = string.Empty;
            txtIssueName.Text = string.Empty;
            ddlCustomerType.Items.Clear();
            if (ddlSpType.Items.Count > 0)
            {
                ddlSpType.SelectedIndex = 0;
            }
            if (ddlBranch.Items.Count > 0)
            {
                ddlBranch.SelectedIndex = 0;
            }
            txtLienTransNo.Text = string.Empty;
            txtIssueDate.Text = string.Empty;
            txtRegNo.Text = string.Empty;
            txtLienDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            // Lien Detail 
            txtRemarks.Text = string.Empty;
            txtOurRefNo.Text = string.Empty;
            txtTheirRefNo.Text = string.Empty;
            txtTotalAmount.Text = string.Empty;
            txtLienBank.Text = string.Empty;
            txtBankAddress.Text = string.Empty;
            //Certificate Detail
            ddlCDDenom.Items.Clear();
            ddlCDCertif.Items.Clear();
            txtCDUpTo.Text = "1";
            txtCDTotalAmount.Text = string.Empty;

            ucUserDet.Reset();
            ucUserDet.ResetData();
        }

        private void EnableDisableControl(bool isApproved)
        {
            // general Control

            if (isApproved)
            {
                //gvData.Enabled = false;

                //Issue Details
                Util.ControlEnabled(txtRegNo, false);

                //Customer(s) Details
                gvCustomerDetail.Enabled = false;

                //Nominee(s) Details
                gvNomDetail.Enabled = false;

                //Certificate
                gvCertInfo.Enabled = false;

                //Lien Deail                
                Util.ControlEnabled(txtBankAddress, false);
                Util.ControlEnabled(txtLienBank, false);
                Util.ControlEnabled(txtOurRefNo, false);
                Util.ControlEnabled(txtTheirRefNo, false);
                Util.ControlEnabled(txtRemarks, false);

                //Certificate Detail
                Util.ControlEnabled(ddlCDDenom, false);
                Util.ControlEnabled(btnMarkAll, false);
                Util.ControlEnabled(btnRemoveAll, false);
                Util.ControlEnabled(ddlCDCertif, false);
                Util.ControlEnabled(txtCDUpTo, false);
                Util.ControlEnabled(btnMark, false);

                // user Detail
                //Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), false);

                // button
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true);

                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);

                btnRegSearch.Enabled = false;

                fsList.Visible = false;
            }
            else
            {
                //gvData.Enabled = true;
                // button 
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);

                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);

                //Issue Details
                Util.ControlEnabled(txtRegNo, true);

                //Customer(s) Details
                gvCustomerDetail.Enabled = true;

                //Nominee(s) Details
                gvNomDetail.Enabled = true;

                //Certificate
                gvCertInfo.Enabled = true;

                //Lien Deail                
                Util.ControlEnabled(txtBankAddress, true);
                Util.ControlEnabled(txtLienBank, true);
                Util.ControlEnabled(txtOurRefNo, true);
                Util.ControlEnabled(txtTheirRefNo, true);
                Util.ControlEnabled(txtRemarks, true);

                //Certificate Detail
                Util.ControlEnabled(ddlCDDenom, true);
                Util.ControlEnabled(btnMarkAll, true);
                Util.ControlEnabled(btnRemoveAll, true);
                Util.ControlEnabled(ddlCDCertif, true);
                Util.ControlEnabled(txtCDUpTo, true);
                Util.ControlEnabled(btnMark, true);

                btnRegSearch.Enabled = true;

                fsList.Visible = true;
            }
        }

        protected void txtRegNo_TextChanged(object sender, EventArgs e)
        {
            btnRegSearch_Click(sender, e);
        }
        
    }
}
