using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.Entity.Transaction;
using SBM_BLC1.Transaction;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Configuration;


namespace SBM_WebUI.mp
{
    public partial class IssueUpdate : System.Web.UI.Page
    {
        #region Local Variable
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_REG_NO = "sRegID";
        public const string OBJ_PAGE_ID = "sPageID";
        public const string OBJ_IS_UPDATE = "sIsUpdate";
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_TRANS.SP_ISSUE_UPDATE))
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
            string sRegID = Request.QueryString[OBJ_REG_NO];
            string sPageID = Request.QueryString[OBJ_PAGE_ID];
            string sIsUpdate = Request.QueryString[OBJ_IS_UPDATE];

            if (!string.IsNullOrEmpty(sRegID))
            {
                sRegID = oCrypManager.GetDecryptedString(sRegID, Constants.CRYPT_PASSWORD_STRING);
            }
            if (!string.IsNullOrEmpty(sPageID))
            {
                sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
            }
            if (!string.IsNullOrEmpty(sIsUpdate))
            {
                sIsUpdate = oCrypManager.GetDecryptedString(sIsUpdate, Constants.CRYPT_PASSWORD_STRING);
            }

            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            // common portion to Enabled or not
            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            DDListUtil.LoadDDLFromDB(ddlBranch, "BranchID", "BranchName", "SPMS_Branch", true);

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();
            // year
            for (int i = 1990; i < 2020; i++)
            {
                DDListUtil.Add(ddlYear, i.ToString(), i.ToString());
            }
            
            if (!string.IsNullOrEmpty(sRegID) && !string.IsNullOrEmpty(sPageID) && sIsUpdate.Equals("0"))
            {
                string sOperationType = sPageID.Substring(4, 1);
                if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
                {
                    // come from approve page.. 
                    LoadDataByRegNo(string.Empty, sRegID, "1");
                    EnableDisableControl(true);
                    divOldNominee.Visible = true;
                    // user detail
                    Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);                    

                    #region User-Detail.
                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    oUserDetails.CheckerID = oConfig.UserName;
                    oUserDetails.CheckDate = DateTime.Now;
                    ucUserDet.UserDetail = oUserDetails;
                    #endregion User-Detail.
                }
            }
            else
            {
                fsList.Visible = true;
                LoadUnapprovedList();
                // come from View page.. 
                if (!string.IsNullOrEmpty(sIsUpdate) && sIsUpdate.Equals("1"))
                {
                    LoadDataByRegNo(string.Empty, sRegID, "1");
                    divOldNominee.Visible = false;
                }
                
                if (oConfig != null)
                {
                    UserDetails oUserDetails = new UserDetails();
                    oUserDetails.MakerID = oConfig.UserName;
                    oUserDetails.MakeDate = DateTime.Now;
                    ucUserDet.UserDetail = oUserDetails;
                }

                EnableDisableControl(false);           
            }
        }
        #endregion InitializeData

        

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_UPDATE).PadLeft(5, '0'), false);
        }

        protected void btnReset_OnClick(object sender, EventArgs e)
        {
            TotalClear();
        }

        private void LoadBySPType(Issue oIssue)
        {
            SPPolicy oSPPolicy = null;            
            SPPolicyDAL oSPPolicyDAL = new SPPolicyDAL();
            Result oResult = (Result)oSPPolicyDAL.LatestPolicy(oIssue.SPType.SPTypeID, Constants.ACTIVITY_TYPE.ISSUE, oIssue.VersionIssueDate);
            if (oResult.Status)
            {
                ddlCustomerType.Items.Clear();
                oSPPolicy = (SPPolicy)oResult.Return;
                DDListUtil.Assign(ddlCustomerType, oSPPolicy.DTCustomerTypePolicy, true);                
            }
        }

        protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("Page"))
            {
                SEARCH_FROM = 2;
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                LoadDataByRegNo("", gvRow.Cells[2].Text, "1");//query from Temp Table                
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

        private void LoadUnapprovedList()
        {
            IssueDAL oIssueDAL = new IssueDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oIssueDAL.LoadTmpIssueUpdateDataTableList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);
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

                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;
                    }
                }
            }
        }

        public void PopupStopPaySearchLoadAction(string sIssueUpdateTransNo, string sRegNo, string sApprovalStaus)
        {
            if (!string.IsNullOrEmpty(sRegNo))
            {
                SEARCH_FROM = 2;
                LoadDataByRegNo(sIssueUpdateTransNo, sRegNo, sApprovalStaus);
                hdDataType.Value = sApprovalStaus;
                divOldNominee.Visible = false;
                //if searched from Approved
                if (sApprovalStaus.Equals("2"))
                {
                    EnableDisableControl(true);                    
                }
                else
                {
                    EnableDisableControl(false);                    
                }
            }
        }

        public void PopupIssueSearchLoadAction(string sRegNo, string sTra, string sApprovalStaus)
        {
            SEARCH_FROM = 1;
            LoadDataByRegNo(string.Empty, sRegNo, sApprovalStaus);            
            EnableDisableControl(false);
            divOldNominee.Visible = false;
        }

        public void LoadDataByRegNo(string sIssueUpdateTrnsNo, string sRegNo, string sApprovalStaus)
        {
            TotalClear();
            Issue oIssue = new Issue();
            oIssue.RegNo = sRegNo != "" ? sRegNo : txtRegistrationNo.Text.Trim();
            oIssue.UpdateIssueTransNo = sIssueUpdateTrnsNo;
            if (sApprovalStaus.Equals("2"))
            {
                oIssue.IsApproved = 2;
            }
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (oConfig != null)
            {
                oIssue.BankID = oConfig.BankCodeID;
                oIssue.DivisionID = oConfig.DivisionID;
            }

            IssueDAL oIssueDAL = new IssueDAL();
            Result oResult = oIssueDAL.LoadDataByRegNo(oIssue);

            oIssue = (Issue)oResult.Return;
            //Store Issue object to Session
            Session[Constants.SES_CURRENT_ISSUE] = oIssue;

            if (oResult.Status)
            {
                #region Issue Detail
                LoadBySPType(oIssue);
                hdUpdateIssueTransNo.Value = oIssue.UpdateIssueTransNo;
                hdIssueTransNo.Value = oIssue.IssueTransNo;
                //Issue Details
                DDListUtil.Assign(ddlSpType, oIssue.SPType.SPTypeID);
                DDListUtil.Assign(ddlCustomerType, oIssue.CustomerType.CustomerTypeID);
                DDListUtil.Assign(ddlBranch, oIssue.Branch.BranchID);
                DDListUtil.Assign(ddlYear, oIssue.UserDetails.CheckDate.Year);

                txtTransNo.Text = oIssue.UpdateIssueTransNo;
                if (oIssue.ChangeDate.Year <= 1900)
                {
                    txtChangeDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
                }
                else
                {
                    txtChangeDate.Text = oIssue.ChangeDate.ToString(Constants.DATETIME_FORMAT);
                }
                txtTotalAmount.Text = oIssue.IssueAmount.ToString();
                txtIssueDate.Text = oIssue.OldVersionIssueDate.ToString(Constants.DATETIME_FORMAT);
                txtRegistrationNo.Text = oIssue.RegNo;
                #endregion

                #region Change Details
                txtRemarks.Text = oIssue.IssueUpdateRemarks;

                if (oIssue.UpdateNewIssueDate.Year <= 1900)
                {
                    txtNewIssueDate.Text = string.Empty;
                }
                else
                {
                    txtNewIssueDate.Text = oIssue.UpdateNewIssueDate.ToString(Constants.DATETIME_FORMAT);
                }
                txtNewIssueName.Text = oIssue.IssueName;
                txtOldIssueName.Text = oIssue.OldIssueName;
                txtOldIssueDate.Text = oIssue.OldVersionIssueDate.ToString(Constants.DATETIME_FORMAT);

                #endregion

                #region Customer Details
                gvCustomerDetail.DataSource = null;
                gvCustomerDetail.DataBind();
                if (oIssue.CustomerDetailsList.Count > 0)
                {
                    DataTable dtCustomerDetail = new DataTable("dtCustomerDetail");

                    dtCustomerDetail.Columns.Add(new DataColumn("bfCustomerID", typeof(string)));
                    dtCustomerDetail.Columns.Add(new DataColumn("bfCustomerName", typeof(string)));
                    dtCustomerDetail.Columns.Add(new DataColumn("bfDateOfBirth", typeof(DateTime)));
                    dtCustomerDetail.Columns.Add(new DataColumn("bfAddress", typeof(string)));
                    dtCustomerDetail.Columns.Add(new DataColumn("bfPhone", typeof(string)));
                    dtCustomerDetail.Columns.Add(new DataColumn("bfDateOfBirth2", typeof(DateTime)));
                    dtCustomerDetail.Columns.Add(new DataColumn("bfNationality", typeof(string)));
                    dtCustomerDetail.Columns.Add(new DataColumn("bfPassportNo", typeof(string)));
                    dtCustomerDetail.Columns.Add(new DataColumn("bfForeignAddress", typeof(string)));

                    DataRow rowCD = null;
                    for (int i = 0; i < oIssue.CustomerDetailsList.Count; i++)
                    {
                        rowCD = dtCustomerDetail.NewRow();

                        rowCD["bfCustomerID"] = oIssue.CustomerDetailsList[i].CustomerID.ToString();
                        rowCD["bfCustomerName"] = oIssue.CustomerDetailsList[i].CustomerName.ToString();
                        if (oIssue.CustomerDetailsList[i].DateOfBirth.Year > 1900)
                        {
                            rowCD["bfDateOfBirth"] = oIssue.CustomerDetailsList[i].DateOfBirth;
                        }
                        rowCD["bfAddress"] = oIssue.CustomerDetailsList[i].Address.ToString();
                        rowCD["bfPhone"] = oIssue.CustomerDetailsList[i].Phone.ToString();
                        //if (oIssue.CustomerDetailsList[i].DateOfBirth2.Year > 1900)
                        //{
                        //    rowCD["bfDateOfBirth2"] = oIssue.CustomerDetailsList[i].DateOfBirth2;
                        //}
                        rowCD["bfNationality"] = oIssue.CustomerDetailsList[i].Nationality.ToString();
                        rowCD["bfPassportNo"] = oIssue.CustomerDetailsList[i].PassportNo.ToString();
                        rowCD["bfForeignAddress"] = oIssue.CustomerDetailsList[i].ForeignAddress.ToString();

                        dtCustomerDetail.Rows.Add(rowCD);
                    }
                    gvCustomerDetail.DataSource = dtCustomerDetail;
                    gvCustomerDetail.DataBind();
                }
                #endregion

                #region Nominee Details
                gvNomDetail.DataSource = null;
                gvNomDetail.DataBind();

                if (oIssue.NomineeList.Count > 0 || oIssue.OldNomineeList.Count > 0)
                {
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

                    gvNomDetail.DataSource = dtNominee;
                    gvNomDetail.DataBind();

                    dtNominee.Rows.Clear();

                    for (int iNomIndx = 0; iNomIndx < oIssue.OldNomineeList.Count; iNomIndx++)
                    {
                        rowNominee = dtNominee.NewRow();

                        rowNominee["Slno"] = oIssue.OldNomineeList[iNomIndx].SlNo;
                        rowNominee["NomineeName"] = oIssue.OldNomineeList[iNomIndx].NomineeName;
                        rowNominee["Relation"] = oIssue.OldNomineeList[iNomIndx].Relation;
                        rowNominee["Address"] = oIssue.OldNomineeList[iNomIndx].Address;
                        rowNominee["NomineeShare"] = oIssue.OldNomineeList[iNomIndx].NomineeShare;
                        rowNominee["Amount"] = oIssue.OldNomineeList[iNomIndx].Amount;

                        dtNominee.Rows.Add(rowNominee);
                    }

                    gvOldNomineeList.DataSource = dtNominee;
                    gvOldNomineeList.DataBind();
                }
                #endregion

                #region User Details
                UserDetails oUserDetails = ucUserDet.UserDetail;
                if (!SEARCH_FROM.Equals(1))
                {
                    if (SEARCH_FROM.Equals(2) && sApprovalStaus.Equals("1"))
                    {
                        //Lodaing from StopSearch form
                        //with unapproved data
                        oUserDetails.MakerID = oIssue.UserDetails.MakerID;
                        oUserDetails.MakeDate = DateTime.Now;
                        oUserDetails.CheckerID = oIssue.UserDetails.CheckerID;
                        oUserDetails.CheckDate = oIssue.UserDetails.CheckDate;
                        oUserDetails.CheckerComment = oIssue.UserDetails.CheckerComment;
                        ucUserDet.UserDetail = oUserDetails;
                    }
                    else if (SEARCH_FROM.Equals(0))
                    {
                        //Lodaing for Approve 
                        //viewed by Checker
                        oUserDetails.MakerID = oIssue.UserDetails.MakerID;
                        oUserDetails.MakeDate = DateTime.Now;
                        ucUserDet.UserDetail = oUserDetails;
                    }
                }
                #endregion
            }
            else
            {
                ucMessage.OpenMessage("Invalid Registration no. Please check..", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }

        protected void btnNDAdd_Click(object sender, EventArgs e)
        {
            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];

            if (oIssue != null)
            {
                AddNomineeToSession(oIssue);
            }
            else
            {
                AddNomineeToSession(new Issue());
            }

            ResetNomineeControls();
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
                if (oNominee == null)
                {
                    oNominee = new Nominee();
                    oNominee.SlNo = oIssue.NomineeList.Count + 1;
                }
                else
                {
                    oNominee.SlNo = Convert.ToInt32(hdNomSlno.Value);
                    editIndex = oIssue.NomineeList.FindIndex(n => n.SlNo.Equals(Convert.ToInt32(hdNomSlno.Value)));
                    isToEdit = true;
                }
            }
            else
            {
                oNominee = new Nominee();
                oNominee.SlNo = oIssue.NomineeList.Count + 1;
            }
            if (!isToEdit) //in case of New ADD
            {
                if ((Convert.ToDecimal(oIssue.NomineeList.Sum(n => n.Amount)) + Util.GetDecimalNumber(txtNDAmount.Text)) > Util.GetDecimalNumber(txtTotalAmount.Text))
                {
                    ucMessage.OpenMessage("Total amount cannot be exceeded Nominee amount.", Constants.MSG_TYPE_ERROR);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    return;
                }
            }

            oNominee.Address = txtNDAddress.Text.ToUpper();
            oNominee.Amount = Util.GetDecimalNumber(txtNDAmount.Text);
            oNominee.IssueTransNo = Convert.ToString(1);
            oNominee.NomineeName = txtNDName.Text.ToUpper();
            oNominee.NomineeShare = Util.GetDecimalNumber(txtNDShare.Text);
            oNominee.Relation = txtNDRelation.Text.ToUpper();
            oNominee.Status = 1;
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

        private void ResetNomineeControls()
        {
            txtNDName.Text = string.Empty;
            txtNDRelation.Text = string.Empty;
            txtNDAddress.Text = string.Empty;
            txtNDShare.Text = string.Empty;
            txtNDAmount.Text = string.Empty;
            hdNomSlno.Value = string.Empty;
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
                hdNomSlno.Value = ((HiddenField)gvRow.FindControl("hdNomineeSlno")).Value;
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

        protected void gvCustomerDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
           // Util.GridDateFormat(e, gvCustomerDetail, null);
        }
        protected void gvNomDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Util.GridDateFormat(e, gvNomDetail, null);
        }


        protected void gvNomDetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //just for signature to Handle IE exception
        }
        
        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");

            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_UPDATE).PadLeft(5, '0'), false);
            }
            else
            {
                txtRegistrationNo.Focus();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];

            if (oIssue != null)
            {
                if (!string.IsNullOrEmpty(txtNewIssueName.Text))
                {
                    oIssue.IssueName = txtNewIssueName.Text.ToUpper();
                }
                else
                {
                    oIssue.IssueName = txtOldIssueName.Text.ToUpper();
                }
                if (!string.IsNullOrEmpty(txtNewIssueDate.Text))
                {
                    oIssue.VersionIssueDate = Util.GetDateTimeByString(txtNewIssueDate.Text);
                }
                oIssue.IssueUpdateRemarks = txtRemarks.Text.ToUpper();
                string sDtForPolicy = txtNewIssueDate.Text;
                if(string.IsNullOrEmpty(sDtForPolicy))
                {
                    sDtForPolicy = txtOldIssueDate.Text;
                }
                SPPolicyDAL oSPPolicyDAL = new SPPolicyDAL();
                Result oResult1 = (Result)oSPPolicyDAL.LatestPolicy(oIssue.SPType.SPTypeID, Constants.ACTIVITY_TYPE.ISSUE, Util.GetDateTimeByString(sDtForPolicy));
                if (oResult1.Status)
                {
                    oIssue.VersionSPPolicy = oResult1.Return as SPPolicy;
                }
                oIssue.UserDetails = ucUserDet.UserDetail;


                IssueDAL oIssueDAL = new IssueDAL();
                Result oResult = (Result)oIssueDAL.SaveIssueUpdate(oIssue);
                if (oResult.Status)
                {
                    TotalClear();
                    LoadUnapprovedList();
                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
                }
            }
            else
            {
                ucMessage.OpenMessage("Problem in Data. Please check", Constants.MSG_TYPE_ERROR);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtNewIssueDate.Enabled)
            {
                if (!string.IsNullOrEmpty(hdUpdateIssueTransNo.Value))
                {
                    IssueDAL oIssueDAL = new IssueDAL();
                    Result oResult = (Result)oIssueDAL.DeteteIssueUpdate(hdIssueTransNo.Value, hdUpdateIssueTransNo.Value);
                    if (oResult.Status)
                    {
                        TotalClear();
                        LoadUnapprovedList();
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
                    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_APPROVED_DELETE_DATA, Constants.MSG_TYPE_INFO);               
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
            IssueDAL oIssueDAL = new IssueDAL();
            oIssue.UserDetails = ucUserDet.UserDetail;
            Result oResult = (Result)oIssueDAL.ApproveIssueUpdate(oIssue);
            if (oResult.Status)
            {
                ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE, Constants.MSG_TYPE_SUCCESS);
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_APPROVE, Constants.MSG_TYPE_ERROR);
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdUpdateIssueTransNo.Value))
            {
                Issue oIssue = new Issue(hdIssueTransNo.Value);
                IssueDAL oIssueDAL = new IssueDAL();
                oIssue.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oIssueDAL.RejectIssueUpdate(oIssue);
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

        private void EnableDisableControl(bool isApproved)
        {            
            if (isApproved)
            {
                // search detail.. 
                Util.ControlEnabled(txtChangeDate, false);
                Util.ControlEnabled(txtRegistrationNo, false);
                Util.ControlEnabled(ddlYear, false);
                // Remarks
                Util.ControlEnabled(txtRemarks, false);

                // Change Details                
                Util.ControlEnabled(txtNewIssueDate, false);                
                Util.ControlEnabled(txtNewIssueName, false);

                //Gridview control
                Util.ControlEnabled(gvNomDetail, false);
                
                // user detail
                //Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);

                // button 
                Util.ControlEnabled(btnNDAdd, false);
                Util.ControlEnabled(btnNDReset, false);

                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true);

                Util.ControlEnabled(btnTransSearch, false);
                Util.ControlEnabled(btnRegSearch, false);
                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);
            }
            else
            {
                Util.ControlEnabled(txtChangeDate, true);
                Util.ControlEnabled(txtRegistrationNo, true);
                Util.ControlEnabled(ddlYear, true);
                // Remarks
                Util.ControlEnabled(txtRemarks, true);

                // Change Details                
                Util.ControlEnabled(txtNewIssueDate, true);
                Util.ControlEnabled(txtNewIssueName, true);

                //Gridview control
                Util.ControlEnabled(gvNomDetail, true);

                // user detail
                //Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), false);

                // button 
                Util.ControlEnabled(btnNDAdd, true);
                Util.ControlEnabled(btnNDReset, true);

                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);

                Util.ControlEnabled(btnTransSearch, true);
                Util.ControlEnabled(btnRegSearch, true);
                Util.ControlEnabled(btnReset, true);                
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);                                
            }
        }

        private void TotalClear()
        {
            // search detail.. 
            txtTransNo.Text = string.Empty;
            txtChangeDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            // Issue Details
            ddlSpType.SelectedIndex = 0;
            ddlBranch.SelectedIndex = 0;
            ddlYear.SelectedIndex = 0;
            txtIssueDate.Text = string.Empty;
            if (ddlCustomerType.Items.Count > 0)
            {
                ddlCustomerType.SelectedIndex = 0;
            }
            txtTotalAmount.Text = string.Empty;

            //Nominee Details
            txtNDAddress.Text = string.Empty;
            txtNDAmount.Text = string.Empty;
            txtNDName.Text = string.Empty;
            txtNDRelation.Text = string.Empty;
            txtNDShare.Text = string.Empty;

            // Change Details
            txtChangeDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtOldIssueDate.Text = string.Empty;
            txtOldIssueName.Text = string.Empty;
            txtNewIssueDate.Text = string.Empty;
            txtNewIssueName.Text = string.Empty;

            // Issue Details
            txtRegistrationNo.Text = string.Empty;

            // Remarks
            txtRemarks.Text = string.Empty;
            //gridView
            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();
            gvOldNomineeList.DataSource = null;
            gvOldNomineeList.DataBind();

            ucUserDet.ResetData();

            //Hidden fields            
            hdDataType.Value = string.Empty;
            hdIssueTransNo.Value = string.Empty;
            hdIssueTransNo.Value = string.Empty;
            hdNomSlno.Value = string.Empty;
            hdUpdateIssueTransNo.Value = string.Empty;

            Session[Constants.SES_CURRENT_ISSUE] = null;
        }

        protected void btnRegSearch_Click(object sender, EventArgs e)
        {
            SEARCH_FROM = 1;
            LoadDataByRegNo(string.Empty, txtRegistrationNo.Text.Trim(), "2");
            EnableDisableControl(false);
            divOldNominee.Visible = false;
        }       
    }
}
