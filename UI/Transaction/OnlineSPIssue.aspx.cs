using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Configuration;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Entity.Transaction;
using System.Collections;
using SBM_BLC1.Transaction;
using System.Globalization;

namespace SBM_WebUI.mp
{
    public partial class OnlineSPIssue : System.Web.UI.Page
    {
        #region Local Variable
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_REG_NO = "sRegID";
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

                    if (txtAccountNo.Text.Length >= 12)
                    {
                        txtAccountNo_TextChanged(sender, e);
                    }
                    CheckPermission chkPer = new CheckPermission();
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_TRANS.SP_ISSUE_OLD))
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
            // Issue set in session 
            if (Session[Constants.SES_CURRENT_ISSUE] == null)
            {
                Issue oSesIssue = new Issue();
                Session.Add(Constants.SES_CURRENT_ISSUE, oSesIssue);
            }
            else
            {
                Issue oSesIssue = new Issue();
                Session[Constants.SES_CURRENT_ISSUE] = oSesIssue;
            }

            //   = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            string sRegID = Request.QueryString[OBJ_REG_NO];
            string sPageID = Request.QueryString[OBJ_PAGE_ID];
            gvData.PageSize = (int)Constants.PAGING_UNAPPROVED;

            if (!string.IsNullOrEmpty(sRegID))
            {
                sRegID = oCrypManager.GetDecryptedString(sRegID, Constants.CRYPT_PASSWORD_STRING);
            }
            if (!string.IsNullOrEmpty(sPageID))
            {
                sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
            }

            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            // common portion to Enabled or not
            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlResidentCountry, "COUNTRY_NAME", "COUNTRY_NAME", "SPMS_CountryList", false);
            DDListUtil.LoadDDLFromDB(ddlNationalIDCountry, "COUNTRY_NAME", "COUNTRY_NAME", "SPMS_CountryList", false);
            DDListUtil.LoadDDLFromDB(ddlPassportNoCountry, "COUNTRY_NAME", "COUNTRY_NAME", "SPMS_CountryList", false);
            DDListUtil.LoadDDLFromDB(ddlBirthCertificateNoCountry, "COUNTRY_NAME", "COUNTRY_NAME", "SPMS_CountryList", false);
            DDListUtil.LoadDDLFromDB(ddlCollectionBranch, "BranchID", "BranchName", "SPMS_Branch", true);

            DDListUtil.Assign(ddlResidentCountry, "BANGLADESH");
            DDListUtil.Assign(ddlPassportNoCountry, "BANGLADESH");
            DDListUtil.Assign(ddlNationalIDCountry, "BANGLADESH");
            DDListUtil.Assign(ddlBirthCertificateNoCountry, "BANGLADESH");


            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            DDListUtil.LoadDDLFromDB(ddlBranch, "BranchID", "BranchName", "SPMS_Branch", true);
            DDListUtil.LoadDDLFromDB(ddlAccountBranch, "BranchID", "BranchName", "SPMS_Branch", true);

            Set_PaymentModeControls();

            ddlBranch.Text = oConfig.BranchID;

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();


            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            gvData.DataSource = null;
            gvData.DataBind();

            txtIssueDate.Text = DateTime.Today.ToString(Constants.DATETIME_FORMAT);
            // user detail
            //Util.ControlEnabled(ucUserDet.FindControl("txtMakerId"), false);
            //Util.ControlEnabled(ucUserDet.FindControl("txtCheckerId"), false);
            if (!string.IsNullOrEmpty(sRegID) && !string.IsNullOrEmpty(sPageID))
            {
                string sOperationType = sPageID.Substring(4, 1);
                if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
                {
                    SEARCH_FROM = 1;
                    // come from approve page.. 
                    LoadDataByRegNo(sRegID);
                    // Issue Details
                    Util.ControlEnabled(ddlSpType, false);
                    Util.ControlEnabled(ddlBranch, false);
                    Util.ControlEnabled(ddlCollectionBranch, false);
                    Util.ControlEnabled(txtIssueDate, false);
                    Util.ControlEnabled(ddlCustomerType, false);
                    Util.ControlEnabled(txtAppliedAmount, false);
                    Util.ControlEnabled(txtRegistrationNo, false);

                    //Nominee(s) Details
                    Util.ControlEnabled(txtNDName, false);
                    Util.ControlEnabled(txtNDRelation, false);
                    Util.ControlEnabled(txtNDAddress, false);
                    Util.ControlEnabled(txtNDShare, false);
                    

                    gvNomDetail.Enabled = true;
                    gvNomDetail.Columns[1].Visible = false;

                    gvCustomerDetail.Enabled = true;

                    // issue name
                    Util.ControlEnabled(txtIssueName, false);
                    Util.ControlEnabled(txtMasterNo, false);

                    // Bond Holder Details
                    Util.ControlEnabled(txtBHDHolderName, false);
                    Util.ControlEnabled(txtBHDAddress, false);
                    Util.ControlEnabled(txtBHDRelation, false);

                    // Certificate(s) Detail
                    Util.ControlEnabled(txtTotalAmount, false);
                    

                    // user detail
                    Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);

                    // button 
                    Util.ControlEnabled(btnReject, true);
                    Util.ControlEnabled(btnApprove, true);
                    Util.ControlEnabled(btnBack, true);

                    Util.ControlEnabled(btnRegSeach, false);
                    Util.ControlEnabled(btnReset, false);
                    Util.ControlEnabled(btnSave, false);
                    Util.ControlEnabled(btnDelete, false);
                    Util.ControlEnabled(btnCDAdd, false);
                    Util.ControlEnabled(btnNDAdd, false);
                    Util.ControlEnabled(btnNDReset, true);

                    txtAccountNo.Enabled = false;
                    txtAccountName.Enabled = false;
                    txtCHQNo.Enabled = false;
                    txtRoutingNo.Enabled = false;
                    ddlPDPaymentMode.Enabled = false;
                    ddlAccountType.Enabled = false;
                    ddlAccountBranch.Enabled = false;

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
                // for first time.

                // Issue Details
                //txtIssueDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
                Util.ControlEnabled(ddlSpType, true);
                Util.ControlEnabled(ddlBranch, true);
                Util.ControlEnabled(ddlCollectionBranch, true);
                Util.ControlEnabled(txtIssueDate, true);
                Util.ControlEnabled(ddlCustomerType, true);
                Util.ControlEnabled(txtAppliedAmount, true);
                Util.ControlEnabled(txtRegistrationNo, true);

                //Nominee(s) Details
                Util.ControlEnabled(txtNDName, true);
                Util.ControlEnabled(txtNDRelation, true);
                Util.ControlEnabled(txtNDAddress, true);
                Util.ControlEnabled(txtNDShare, true);

                gvNomDetail.Enabled = true;
                gvCustomerDetail.Enabled = true;

                // Issue Name
                Util.ControlEnabled(txtIssueName, true);
                Util.ControlEnabled(txtMasterNo, true);

                // Bond Holder Details
                Util.ControlEnabled(txtBHDHolderName, true);
                Util.ControlEnabled(txtBHDAddress, true);
                Util.ControlEnabled(txtBHDRelation, true);

                // Certificate(s) Detail
                Util.ControlEnabled(txtTotalAmount, true);

                // button 
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);

                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);

                UserDetails oUserDetails = new UserDetails();
                oUserDetails.MakerID = oConfig.UserName;
                oUserDetails.MakeDate = DateTime.Now;
                ucUserDet.UserDetail = oUserDetails;

                fsList.Visible = true;
                LoadPreviousList();
            }
        }
        #endregion InitializeData

        private void ControlManager()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (ucUserDet.UserDetail.CheckerID!="")
            {
                SEARCH_FROM = 1;
                // Issue Details
                Util.ControlEnabled(ddlSpType, false);
                Util.ControlEnabled(ddlBranch, false);
                Util.ControlEnabled(ddlCollectionBranch, false);
                Util.ControlEnabled(txtIssueDate, false);
                Util.ControlEnabled(ddlCustomerType, false);
                Util.ControlEnabled(txtAppliedAmount, false);
                Util.ControlEnabled(txtRegistrationNo, false);

                //Nominee(s) Details
                Util.ControlEnabled(txtNDName, false);
                Util.ControlEnabled(txtNDRelation, false);
                Util.ControlEnabled(txtNDAddress, false);
                Util.ControlEnabled(txtNDShare, false);


                gvNomDetail.Enabled = true;
                gvNomDetail.Columns[1].Visible = false;

                gvCustomerDetail.Enabled = true;

                // issue name
                Util.ControlEnabled(txtIssueName, false);
                Util.ControlEnabled(txtMasterNo, false);

                // Bond Holder Details
                Util.ControlEnabled(txtBHDHolderName, false);
                Util.ControlEnabled(txtBHDAddress, false);
                Util.ControlEnabled(txtBHDRelation, false);

                // Certificate(s) Detail
                Util.ControlEnabled(txtTotalAmount, false);


                // user detail
                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);

                // button 
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);

                Util.ControlEnabled(btnRegSeach, false);
                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);
                Util.ControlEnabled(btnCDAdd, false);
                Util.ControlEnabled(btnNDAdd, false);
                Util.ControlEnabled(btnNDReset, true);

                txtAccountNo.Enabled = false;
                txtAccountName.Enabled = false;
                txtCHQNo.Enabled = false;
                txtRoutingNo.Enabled = false;
                ddlPDPaymentMode.Enabled = false;
                ddlAccountType.Enabled = false;
                ddlAccountBranch.Enabled = false;

                gvCustomerDetail.Columns[1].Visible = false;

            }
            else
            {
                // for first time.

                // Issue Details
                //txtIssueDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
                Util.ControlEnabled(ddlSpType, true);
                Util.ControlEnabled(ddlBranch, true);
                Util.ControlEnabled(ddlCollectionBranch, true);
                Util.ControlEnabled(txtIssueDate, true);
                Util.ControlEnabled(ddlCustomerType, true);
                Util.ControlEnabled(txtAppliedAmount, true);
                Util.ControlEnabled(txtRegistrationNo, true);

                //Nominee(s) Details
                Util.ControlEnabled(txtNDName, true);
                Util.ControlEnabled(txtNDRelation, true);
                Util.ControlEnabled(txtNDAddress, true);
                Util.ControlEnabled(txtNDShare, true);

                gvNomDetail.Enabled = true;
                gvCustomerDetail.Enabled = true;

                // Issue Name
                Util.ControlEnabled(txtIssueName, true);
                Util.ControlEnabled(txtMasterNo, true);

                // Bond Holder Details
                Util.ControlEnabled(txtBHDHolderName, true);
                Util.ControlEnabled(txtBHDAddress, true);
                Util.ControlEnabled(txtBHDRelation, true);

                // Certificate(s) Detail
                Util.ControlEnabled(txtTotalAmount, true);

                // button 
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);

                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);

                gvCustomerDetail.Columns[1].Visible = true;

            }
        }
        private void TotalClear()
        {
            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();


            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            // Issue Details
            ddlSpType.SelectedIndex = 0;
            //ddlBranch.SelectedIndex = 0;
            //txtIssueDate.Text = string.Empty;
            ddlCustomerType.Items.Clear();
            txtAppliedAmount.Text = "";
            txtRegistrationNo.Text = "";

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

            // issue name
            txtIssueName.Text = "";
            txtMasterNo.Text = "";

            // Bond Holder Details
            txtBHDHolderName.Text = "";
            txtBHDAddress.Text = "";
            txtBHDRelation.Text = "";

            // Certificate(s) Detail
            txtTotalAmount.Text = "";
            ucUserDet.ResetData();
            
            hdNomSlno.Value = "";
            Session[Constants.SES_CURRENT_ISSUE] = null;

            ddlPDPaymentMode.SelectedValue = "3";
            Set_PaymentModeControls();

            ddlCollectionBranch.SelectedIndex = 0;
        }

        private void LoadDataByRegNo(string sRegNo)
        {
            Issue oIssue = new Issue();
            oIssue.RegNo = sRegNo != "" ? sRegNo : txtRegistrationNo.Text.Trim();
            IssueDAL oIssueDAL = new IssueDAL();
            Result oResult = oIssueDAL.LoadOnlineSaleByRegNo(oIssue);

            oIssue = (Issue)oResult.Return;
            if (Session[Constants.SES_CURRENT_ISSUE] != null)
            {
                //Store Issue object to Session
                Session[Constants.SES_CURRENT_ISSUE] = oIssue;
            }
            else
            {
                Session.Add(Constants.SES_CURRENT_ISSUE, oIssue);
            }

            if (oResult.Status)
            {
                hdTransNo.Value = oIssue.IssueTransNo;
                //Issue Details
                DDListUtil.Assign(ddlSpType, oIssue.SPType.SPTypeID);
                LoadBySPType();
                DDListUtil.Assign(ddlCustomerType, oIssue.CustomerType.CustomerTypeID.Trim());
                DDListUtil.Assign(ddlBranch, oIssue.Branch.BranchID);
                DDListUtil.Assign(ddlCollectionBranch, oIssue.CollectionBranch);            

                //txtTotalAmount.Text = oIssue.IssueAmount.ToString();
                txtIssueDate.Text = oIssue.VersionIssueDate.ToString(Constants.DATETIME_FORMAT);
                txtRegistrationNo.Text = oIssue.RegNo;

                ddlPDPaymentMode.SelectedValue = oIssue.Payment.PaymentMode.ToString();
                ddlAccountType.SelectedValue = oIssue.Payment.AccountType.ToString();
                Set_PaymentModeControls();
                txtAccountNo.Text = oIssue.Payment.AccountNo;
                txtCHQNo.Text = oIssue.Payment.CHQNo;
                txtRoutingNo.Text = oIssue.Payment.RoutingNo;
                DDListUtil.Assign(ddlAccountBranch, oIssue.Payment.AccountBranch.Trim());
                txtTotalAmount.Text = oIssue.Payment.PaymentAmount.ToString("N2");

                // maker Detail
                ucUserDet.UserDetail = oIssue.UserDetails;

                // customer Loading 
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
                    //dtCustomerDetail.Columns.Add(new DataColumn("bfDateOfBirth2", typeof(DateTime)));
                    dtCustomerDetail.Columns.Add(new DataColumn("bfNationality", typeof(string)));
                    dtCustomerDetail.Columns.Add(new DataColumn("bfPassportNo", typeof(string)));
                    //dtCustomerDetail.Columns.Add(new DataColumn("bfForeignAddress", typeof(string)));

                    DataRow rowCD = null;
                    for (int i = 0; i < oIssue.CustomerDetailsList.Count; i++)
                    {
                        rowCD = dtCustomerDetail.NewRow();

                        rowCD["bfCustomerID"] = oIssue.CustomerDetailsList[i].CustomerID.ToString();
                        rowCD["bfCustomerName"] = oIssue.CustomerDetailsList[i].CustomerName.ToString();
                        rowCD["bfDateOfBirth"] = oIssue.CustomerDetailsList[i].DateOfBirth.ToString("dd-MMM-yyyy");
                        rowCD["bfAddress"] = oIssue.CustomerDetailsList[i].Address.ToString();
                        rowCD["bfPhone"] = oIssue.CustomerDetailsList[i].Phone.ToString();
                        //rowCD["bfDateOfBirth2"] = oIssue.CustomerDetailsList[i].DateOfBirth2.ToString("dd-MMM-yyyy");
                        rowCD["bfNationality"] = oIssue.CustomerDetailsList[i].Nationality.ToString();
                        rowCD["bfPassportNo"] = oIssue.CustomerDetailsList[i].PassportNo.ToString();
                        //rowCD["bfForeignAddress"] = oIssue.CustomerDetailsList[i].ForeignAddress.ToString();

                        dtCustomerDetail.Rows.Add(rowCD);
                    }

                    gvCustomerDetail.DataSource = dtCustomerDetail;
                    gvCustomerDetail.DataBind();
                    
                    if (SEARCH_FROM.Equals(1))
                    {
                        gvCustomerDetail.Columns[1].Visible = false;
                    }
                    else
                    {
                        gvCustomerDetail.Columns[1].Visible = true;
                    }
                    //set customer ID as hidden field
                    for (int cusDtlRowIndx = 0; cusDtlRowIndx < gvCustomerDetail.Rows.Count; cusDtlRowIndx++)
                    {
                        HiddenField hdTmpCustomerID = ((HiddenField)gvCustomerDetail.Rows[cusDtlRowIndx].FindControl("hdTmpCustomerID"));
                        if (hdTmpCustomerID != null)
                        {
                            hdTmpCustomerID.Value = cusDtlRowIndx.ToString();
                        }
                    }
                }
                // end of loading 

                //start of Bond info
                txtAppliedAmount.Text = oIssue.IssueAmount.ToString();
                txtIssueName.Text = oIssue.IssueName;
                txtBHDAddress.Text = oIssue.BondHolderAddress;
                txtBHDHolderName.Text = oIssue.BondHolderName;
                txtBHDRelation.Text = oIssue.BondHolderRelation;

                // start to nominee loading..
                gvNomDetail.DataSource = null;
                gvNomDetail.DataBind();
                if (oIssue.NomineeList.Count > 0)
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

                    if (SEARCH_FROM.Equals(1))
                    {
                        gvNomDetail.Columns[1].Visible = false;
                    }
                    else
                    {
                        gvNomDetail.Columns[1].Visible = true;
                    }
                }
                // end of nominee loading..
                if (SEARCH_FROM.Equals(1))
                {
                    btnNDAdd.Visible = false;
                }

                #region Certificate Or Scrip
                DataTable dtDenom = new DataTable();

                dtDenom.Columns.Add(new DataColumn("DenominationID", typeof(string)));
                dtDenom.Columns.Add(new DataColumn("Series", typeof(string)));
                dtDenom.Columns.Add(new DataColumn("SerialNo", typeof(string)));                
                dtDenom.Columns.Add(new DataColumn("chkSelect", typeof(bool)));

                int iTotalAmount = 0;
                DataRow rowDenom = null;
                for (int i = 0; i < oIssue.ScripList.Count; i++)
                {
                    rowDenom = dtDenom.NewRow();

                    rowDenom["DenominationID"] = oIssue.ScripList[i].Denomination.DenominationID.ToString();
                    rowDenom["Series"] = oIssue.ScripList[i].SPSeries.ToString();
                    rowDenom["SerialNo"] = oIssue.ScripList[i].SlNo.ToString();                    
                    rowDenom["chkSelect"] = oIssue.ScripList[i].isEncashed;

                    iTotalAmount = iTotalAmount + oIssue.ScripList[i].Denomination.DenominationID;

                    dtDenom.Rows.Add(rowDenom);
                }
                //Reload Grid
                txtTotalAmount.Text = iTotalAmount.ToString();
                #endregion Certificate Or Scrip

                //  to varify Master Account 
                CustomerDetails oCustomerDetails = new CustomerDetails();
                oCustomerDetails.MasterNo = oIssue.MasterNo;
                txtMasterNo.Text = oIssue.MasterNo;
                CustomerDetailsDAL oCustDAL = new CustomerDetailsDAL();
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


        protected void btnRegSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRegistrationNo.Text))
            {
                LoadDataByRegNo(txtRegistrationNo.Text);
                if (txtAccountNo.Text.Length >= 12)
                {
                    txtAccountNo_TextChanged(sender, e);
                }
                ControlManager();
            }
        }


        public void PopupIssueSearchLoadAction(string sTransNo, string sRegNo, string sApprovalStaus)
        {
            if (!string.IsNullOrEmpty(sRegNo))
            {
                LoadDataByRegNo(sRegNo);
            }
            if (sApprovalStaus.Equals("2"))
            {
                ControlStatus(false);
               
            }
            else if (sApprovalStaus.Equals("1"))
            {
                ControlStatus(true);
            }
        }

        private void ControlStatus(bool bStatus)
        {
            // Issue Details
            Util.ControlEnabled(ddlSpType, bStatus);
            Util.ControlEnabled(ddlBranch, bStatus);
            Util.ControlEnabled(ddlCollectionBranch, bStatus);
            Util.ControlEnabled(txtIssueDate, bStatus);
            Util.ControlEnabled(ddlCustomerType, bStatus);
            Util.ControlEnabled(txtAppliedAmount, bStatus);
            Util.ControlEnabled(txtRegistrationNo, bStatus);

            //Nominee(s) Details
            Util.ControlEnabled(txtNDName, bStatus);
            Util.ControlEnabled(txtNDRelation, bStatus);
            Util.ControlEnabled(txtNDAddress, bStatus);
            Util.ControlEnabled(txtNDShare, bStatus);

            gvNomDetail.Enabled = true;

            // issue name
            Util.ControlEnabled(txtIssueName, bStatus);
            Util.ControlEnabled(txtMasterNo, bStatus);

            // Bond Holder Details
            Util.ControlEnabled(txtBHDHolderName, bStatus);
            Util.ControlEnabled(txtBHDAddress, bStatus);
            Util.ControlEnabled(txtBHDRelation, bStatus);

            // Certificate(s) Detail
            Util.ControlEnabled(txtTotalAmount, bStatus);
            chkFiscalYear.Enabled = bStatus;
            chkFiscalYear.Checked = false;

            ddlPDPaymentMode.Enabled = bStatus;
            txtAccountNo.Enabled = bStatus;
            txtCHQNo.Enabled = bStatus;
            txtRoutingNo.Enabled = bStatus;
            ddlAccountType.Enabled = bStatus;
            ddlAccountBranch.Enabled = bStatus;

            Util.ControlEnabled(btnCDAdd, bStatus);
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



        protected void gvScripDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
            int iTotalAmount = 0;
            DataTable dtDenom = new DataTable();
            DataRow rowDenom = null;

            dtDenom.Columns.Add(new DataColumn("DenominationID", typeof(string)));
            dtDenom.Columns.Add(new DataColumn("Series", typeof(string)));
            dtDenom.Columns.Add(new DataColumn("SerialNo", typeof(string)));
            dtDenom.Columns.Add(new DataColumn("chkSelect", typeof(bool)));


            if (oIssue != null)
            {
                Scrip oScrip = oIssue.ScripList.Where(d => d.Denomination.DenominationID.Equals(Convert.ToInt32(gvRow.Cells[1].Text)) && d.SPSeries.Equals(gvRow.Cells[2].Text) && d.SlNo.Equals(gvRow.Cells[3].Text)).SingleOrDefault();

                // delete button
                if (((Button)e.CommandSource).Text.Equals("Delete"))
                {
                    if (oScrip != null)
                    {
                        oIssue.ScripList.Remove(oScrip);
                    }
                    for (int i = 0; i < oIssue.ScripList.Count; i++)
                    {
                        rowDenom = dtDenom.NewRow();

                        rowDenom["DenominationID"] = oIssue.ScripList[i].Denomination.DenominationID.ToString();
                        rowDenom["Series"] = oIssue.ScripList[i].SPSeries.ToString();
                        rowDenom["SerialNo"] = oIssue.ScripList[i].SlNo.ToString();
                        rowDenom["chkSelect"] = oIssue.ScripList[i].isEncashed;

                        iTotalAmount = iTotalAmount + oIssue.ScripList[i].Denomination.DenominationID;

                        dtDenom.Rows.Add(rowDenom);
                    }
                }


                txtTotalAmount.Text = iTotalAmount.ToString();

                Session[Constants.SES_CURRENT_ISSUE] = oIssue;
            }
        }

        protected void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            GridViewRow row = (GridViewRow)checkbox.NamingContainer;
            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
            if (row != null && oIssue != null)
            {
                oIssue.ScripList[row.DataItemIndex].isEncashed = checkbox.Checked;
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
                ddlPassportNoCountry.SelectedValue= oNominee.PassportNo_Country;
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

        protected void gvScripDetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //just for signature to Handle IE exception
        }
        protected void gvNomDetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //just for signature to Handle IE exception
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

        private void LoadPreviousList()
        {
            IssueDAL oIssueDAL = new IssueDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oIssueDAL.LoadUnapprovedOnlineIssueList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);
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

        public void CustomerDetailAction(CustomerDetails oCustomer)
        {
            //Limit Checkup
            IssueDAL oIssueDAL = new IssueDAL();
            if (ddlSpType.SelectedValue == "FSP")
            {
                if (oCustomer.Sex == "F")
                {
                    //if (oCustomer.DateOfBirth.AddYears(18) < DateTime.Today)
                    //{
                    //    ucMessage.OpenMessage("Customer need to have 18+ age for issuance! Please check..", Constants.MSG_TYPE_SUCCESS);
                    //    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    //    return;
                    //}
                }
                //if (oCustomer.Sex == "M")
                //{
                //    if (oCustomer.DateOfBirth.AddYears(65) < DateTime.Today)
                //    {
                //        ucMessage.OpenMessage("Customer need to have 65+ age for issuance! Please check..", Constants.MSG_TYPE_SUCCESS);
                //        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                //        return;
                //    }
                //}
            }
            if (ddlSpType.SelectedValue == "3MS" || ddlSpType.SelectedValue == "BSP")
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
                        oResult = oIssueDAL.LoadCheckLimit(ddlSpType.SelectedValue, oCustomer.MasterNo, "", oCustomer.NationalID, "", 0);
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
            CustomerDetails oCustMastExist = oIssue.CustomerDetailsList.Where(c => c.CustomerID.Equals(oCustomer.CustomerID)).SingleOrDefault();
            // to check adult user or not..
            if (ddlCustomerType.SelectedItem.Text.Contains("Individual") || ddlCustomerType.SelectedItem.Text.Contains("Joint"))
            {
                DateTime dtCustAdultDate = new DateTime(oCustomer.DateOfBirth.Year + 18, oCustomer.DateOfBirth.Month, oCustomer.DateOfBirth.Day);
                DateTime dtToday = DateTime.Now;

                if (dtToday >= dtCustAdultDate)
                {
                    if (oIssue.CustomerDetailsList.Count > 1)//changed by Istiak
                    {
                        // no same master ID
                        ucMessage.OpenMessage("Cannot added multiple customer of this customer type.", Constants.MSG_TYPE_SUCCESS);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        return;
                    }
                }
            }
            else
            {
               //oCustomer.DateOfBirth = Convert.ToDateTime("1900-01-01");
            }

            if (oIssue == null)
            {
                oIssue = new Issue();
            }
            
            //if (oCustomer.CustomerID != -1) // No delete operation for new Customer information
            //{
            //    CustomerDetails oCustDetlExist = oIssue.CustomerDetailsList.Where(c => c.CustomerID.Equals(oCustomer.CustomerID)).SingleOrDefault();
            //    if (oCustDetlExist != null)
            //    {
            //        oIssue.CustomerDetailsList.Remove(oCustDetlExist);
            //    }
            //}

            if (oCustMastExist != null) // No delete operation for new Customer information
            {
                CustomerDetails oCustDetlExist = oIssue.CustomerDetailsList.Where(c => c.CustomerID.Equals(oCustMastExist.CustomerID)).SingleOrDefault();
                if (oCustDetlExist != null)
                {
                    oIssue.CustomerDetailsList.Remove(oCustDetlExist);
                }
            }

            //oCustomer.UserDetails = ucUserDet.UserDetail;
            oIssue.CustomerDetailsList.Add(oCustomer);
            

            DataTable dtCustomerDetail = new DataTable();

            dtCustomerDetail.Columns.Add(new DataColumn("bfCustomerID", typeof(string)));
            dtCustomerDetail.Columns.Add(new DataColumn("bfCustomerName", typeof(string)));
            dtCustomerDetail.Columns.Add(new DataColumn("bfDateOfBirth", typeof(DateTime)));
            dtCustomerDetail.Columns.Add(new DataColumn("bfAddress", typeof(string)));
            dtCustomerDetail.Columns.Add(new DataColumn("bfPhone", typeof(string)));
            //dtCustomerDetail.Columns.Add(new DataColumn("bfDateOfBirth2", typeof(DateTime)));
            dtCustomerDetail.Columns.Add(new DataColumn("bfNationality", typeof(string)));
            dtCustomerDetail.Columns.Add(new DataColumn("bfPassportNo", typeof(string)));
            //dtCustomerDetail.Columns.Add(new DataColumn("bfForeignAddress", typeof(string)));

            DataRow rowCD = null;
            string issueName = string.Empty;

            for (int i = 0; i < oIssue.CustomerDetailsList.Count; i++)
            {
                rowCD = dtCustomerDetail.NewRow();

                rowCD["bfCustomerID"] = oIssue.CustomerDetailsList[i].CustomerID == -1 ? "New Customer" : oIssue.CustomerDetailsList[i].CustomerID.ToString();// oIssue.CustomerDetailsList[i].CustomerID;
                rowCD["bfCustomerName"] = oIssue.CustomerDetailsList[i].CustomerName.ToString();
                rowCD["bfDateOfBirth"] = oIssue.CustomerDetailsList[i].DateOfBirth.ToString("dd-MMM-yyyy");
                rowCD["bfAddress"] = oIssue.CustomerDetailsList[i].Address.ToString();
                rowCD["bfPhone"] = oIssue.CustomerDetailsList[i].Phone.ToString();
                //rowCD["bfDateOfBirth2"] = oIssue.CustomerDetailsList[i].DateOfBirth2.ToString("dd-MMM-yyyy");
                rowCD["bfNationality"] = oIssue.CustomerDetailsList[i].Nationality.ToString();
                rowCD["bfPassportNo"] = oIssue.CustomerDetailsList[i].PassportNo.ToString();
                //rowCD["bfForeignAddress"] = oIssue.CustomerDetailsList[i].ForeignAddress.ToString();

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

                dtCustomerDetail.Rows.Add(rowCD);
            }
            
            

            gvCustomerDetail.DataSource = dtCustomerDetail;
            gvCustomerDetail.DataBind();

            for (int i = 0; i < gvCustomerDetail.Rows.Count; i++)
            {
                HiddenField hdTmpCustomerID = ((HiddenField)gvCustomerDetail.Rows[i].FindControl("hdTmpCustomerID"));
                if (hdTmpCustomerID != null)
                {
                    hdTmpCustomerID.Value = i.ToString();
                }
            }

            Session[Constants.SES_CURRENT_ISSUE] = oIssue;

            //Set Issue Name
            if (issueName.Length > 100)
            {
                txtIssueName.Text = issueName.Substring(0, 100);
            }
            else
            {
                txtIssueName.Text = issueName;
            }
            if (oCustomer.CustomerName.Length > 50)
            {
                txtBHDHolderName.Text = oCustomer.CustomerName.Substring(0, 50); 
            }
            else
            {
                txtBHDHolderName.Text = oCustomer.CustomerName;
            }

            txtBHDAddress.Text = oCustomer.Address;
            if (!string.IsNullOrEmpty(oCustomer.MasterNo))
            {
                txtMasterNo.Text = oCustomer.MasterNo;
                lblMasterVarified.Text = "Verified!";
            }
            else
            {
                lblMasterVarified.Text = "Not Verified yet!";
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

        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, null);
        }

        protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("Page"))
            {
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                LoadDataByRegNo(gvRow.Cells[1].Text);
                if (txtAccountNo.Text.Length >= 12)
                {
                    txtAccountNo_TextChanged(sender, e);
                }
                ControlStatus(true);
            }
        }

        protected void ddlSpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sSpType = ddlSpType.SelectedValue;            
            TotalClear();
            DDListUtil.Assign(ddlSpType, sSpType);
            txtIssueDate.Text = DateTime.Today.ToString(Constants.DATETIME_FORMAT);
            txtIssueDate_TextChanged(sender, e);
            ddlPDPaymentMode_SelectedIndexChanged(sender, e);
            //txtIssueDate.Text = string.Empty;
            //txtIssueDate.Focus();           
        }

        private void LoadBySPType()
        {
            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
            SPPolicy oSPPolicy = null;
            SPPolicyDAL oSPPolicyDAL = new SPPolicyDAL();
            Result oResult = null;
            if (oIssue == null)
            {
                oResult = (Result)oSPPolicyDAL.LatestPolicy(ddlSpType.SelectedValue, Constants.ACTIVITY_TYPE.ISSUE, Util.GetDateTimeByString(txtIssueDate.Text));
            }
            else
            {
                oResult = (Result)oSPPolicyDAL.LatestPolicy(ddlSpType.SelectedValue, Constants.ACTIVITY_TYPE.ISSUE, oIssue.VersionIssueDate);
            }

            if (oResult != null && oResult.Status)
            {
                ddlCustomerType.Items.Clear();
                
                oSPPolicy = (SPPolicy)oResult.Return;
                //To be used in CustomerDetail
                hdSupportdGndr.Value = Convert.ToString(oSPPolicy.SupportedSex);
                DDListUtil.Assign(ddlCustomerType, oSPPolicy.DTCustomerTypePolicy, true);

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
            }
        }

        protected void txtAppliedAmount_TextChanged(object sender, EventArgs e)
        {
            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
            if (oIssue != null)
            {
                oIssue.NomineeList.Clear();
                oIssue.ScripList.Clear();
                Session[Constants.SES_CURRENT_ISSUE] = oIssue;
            }
            txtTotalAmount.Text = txtAppliedAmount.Text;

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

        protected void gvCustomerDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
            HiddenField ohdTmpCustomerID = (HiddenField)gvRow.Cells[0].FindControl("hdTmpCustomerID");

            if (((Button)e.CommandSource).Text.Equals("Select"))
            {
                if (oIssue != null)
                {
                    if (!string.IsNullOrEmpty(ohdTmpCustomerID.Value))
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
            }
            else if (((Button)e.CommandSource).Text.Equals("Delete"))
            {
                gvCustomerDetail.DataSource = null;
                gvCustomerDetail.DataBind();

                if (oIssue != null)
                {
                    oIssue.CustomerDetailsList.RemoveAt(Convert.ToInt32(ohdTmpCustomerID.Value));
                }

                DataTable dtCustomerDetail = new DataTable();

                dtCustomerDetail.Columns.Add(new DataColumn("bfCustomerID", typeof(string)));
                dtCustomerDetail.Columns.Add(new DataColumn("bfCustomerName", typeof(string)));
                dtCustomerDetail.Columns.Add(new DataColumn("bfDateOfBirth", typeof(DateTime)));
                dtCustomerDetail.Columns.Add(new DataColumn("bfAddress", typeof(string)));
                dtCustomerDetail.Columns.Add(new DataColumn("bfPhone", typeof(string)));
                //dtCustomerDetail.Columns.Add(new DataColumn("bfDateOfBirth2", typeof(DateTime)));
                dtCustomerDetail.Columns.Add(new DataColumn("bfNationality", typeof(string)));
                dtCustomerDetail.Columns.Add(new DataColumn("bfPassportNo", typeof(string)));
                //dtCustomerDetail.Columns.Add(new DataColumn("bfForeignAddress", typeof(string)));

                DataRow rowCD = null;
                for (int i = 0; i < oIssue.CustomerDetailsList.Count; i++)
                {
                    rowCD = dtCustomerDetail.NewRow();

                    rowCD["bfCustomerID"] = oIssue.CustomerDetailsList[i].CustomerID == -1 ? "New Customer" : oIssue.CustomerDetailsList[i].CustomerID.ToString();// oIssue.CustomerDetailsList[i].CustomerID;
                    rowCD["bfCustomerName"] = oIssue.CustomerDetailsList[i].CustomerName.ToString();
                    rowCD["bfDateOfBirth"] = oIssue.CustomerDetailsList[i].DateOfBirth;//.ToString("dd-MMM-yyyy");
                    rowCD["bfAddress"] = oIssue.CustomerDetailsList[i].Address.ToString();
                    rowCD["bfPhone"] = oIssue.CustomerDetailsList[i].Phone.ToString();
                    //rowCD["bfDateOfBirth2"] = oIssue.CustomerDetailsList[i].DateOfBirth2; //.ToString("dd-MMM-yyyy");
                    rowCD["bfNationality"] = oIssue.CustomerDetailsList[i].Nationality.ToString();
                    rowCD["bfPassportNo"] = oIssue.CustomerDetailsList[i].PassportNo.ToString();
                    //rowCD["bfForeignAddress"] = oIssue.CustomerDetailsList[i].ForeignAddress.ToString();

                    dtCustomerDetail.Rows.Add(rowCD);
                }
                gvCustomerDetail.DataSource = dtCustomerDetail;
                gvCustomerDetail.DataBind();
                Session[Constants.SES_CURRENT_ISSUE] = oIssue;

                for (int i = 0; i < gvCustomerDetail.Rows.Count; i++)
                {
                    HiddenField hdTmpCustomerID = ((HiddenField)gvCustomerDetail.Rows[i].FindControl("hdTmpCustomerID"));
                    if (hdTmpCustomerID != null)
                    {
                        hdTmpCustomerID.Value = i.ToString();
                    }
                }
            }
        }

        protected void gvCustomerDetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //just for signature to Handle IE exception 
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

        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");

            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_ONL).PadLeft(5, '0'), false);
            }
            else
            {
                ddlSpType.Focus();
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_ONL).PadLeft(5, '0'), false);
        }

        protected void btnSave_Click(object sender, EventArgs e)
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
            if (oIssue.NomineeList.Count>0 && dCount >= 0 && dCount < 100)
            {
                ucMessage.OpenMessage("Total amount of share must be 100!!", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                return;
            }
            string sSPTypeID = ddlSpType.SelectedValue != "" ? ddlSpType.SelectedValue : "";
            if (sSPTypeID == "3MS" || sSPTypeID == "FSP" || sSPTypeID == "BSP")
            {
                if (Convert.ToInt32(ddlCustomerType.SelectedValue) <= 2)
                {
                    if (oIssue.NomineeList.Count == 0)
                    {
                        ucMessage.OpenMessage("Nominee details must be required for the online sale!!", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        return;
                    }
                    for (int i = 0; i < oIssue.CustomerDetailsList.Count; i++)
                    {
                        if (oIssue.CustomerDetailsList[i].NationalID == "")
                        {
                            ucMessage.OpenMessage("Customer [SLNo: " + (i + 1).ToString() + "] NationalID must be required!!", Constants.MSG_TYPE_INFO);
                            ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                            return;
                        }
                    }
                }
            }
            if (Convert.ToInt32(txtTotalAmount.Text) > 100000)
            {
                for (int i = 0; i < oIssue.CustomerDetailsList.Count; i++)
                {
                    if (oIssue.CustomerDetailsList[i].TIN.Trim() == "")
                    {
                        ucMessage.OpenMessage("Customer [SLNo: " + (i + 1).ToString() + "] TIN No must be required!!", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        return;
                    }
                }
            }
            
            SaveAction();
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

                //// Issue Details 
                oIssue.IssueName = txtIssueName.Text.ToUpper();
                oIssue.IssueTransNo = hdTransNo.Value == "" ? "-1" : hdTransNo.Value;
                oIssue.MasterNo = txtMasterNo.Text;
                oIssue.RegNo = txtRegistrationNo.Text.ToUpper();

                //version detail
                oIssue.VersionIssueDate = Util.GetDateTimeByString(txtIssueDate.Text);
                // Bond Holder Details
                oIssue.BondHolderAddress = txtBHDAddress.Text.ToUpper();
                oIssue.BondHolderName = txtBHDHolderName.Text.ToUpper();
                oIssue.BondHolderRelation = txtBHDRelation.Text.ToUpper();

                ////SP Type Details
                oIssue.SPType.SPTypeID = ddlSpType.SelectedValue != "" ? ddlSpType.SelectedValue : "";
                oIssue.Branch.BranchID = ddlBranch.SelectedValue != "" ? ddlBranch.SelectedValue : "";
                oIssue.CollectionBranch = ddlCollectionBranch.SelectedValue != "" ? ddlCollectionBranch.SelectedValue : "";
                //oIssue.RegNo = txtRegistrationNo.Text;
                oIssue.CustomerType.CustomerTypeID = ddlCustomerType.SelectedValue != "" ? ddlCustomerType.SelectedValue : "";
                oIssue.IssueAmount = Util.GetDecimalNumber(txtTotalAmount.Text);

                //Payment Details
                oIssue.Payment.PaymentMode = Convert.ToInt32(ddlPDPaymentMode.SelectedValue);
                oIssue.Payment.AccountNo = txtAccountNo.Text;
                oIssue.Payment.AccountName = txtAccountName.Text;
                oIssue.Payment.AccountType = ddlAccountType.SelectedValue;
                oIssue.Payment.CHQNo = txtCHQNo.Text;
                oIssue.Payment.RoutingNo = txtRoutingNo.Text;
                oIssue.Payment.AccountBranch = ddlAccountBranch.SelectedValue;
                oIssue.Payment.PaymentAmount = Convert.ToDecimal(txtTotalAmount.Text);
                ////Status Details
                oIssue.IsApproved = 1;
                oIssue.IsClaimed = false;
                oIssue.Status = 1;

                ////User Details
                oIssue.UserDetails = ucUserDet.UserDetail;
                oIssue.UserDetails.MakerID = oConfig.UserName;
                oIssue.UserDetails.Division = oConfig.DivisionID;
                oIssue.UserDetails.BankID = oConfig.BankCodeID;
                oIssue.UserDetails = ucUserDet.UserDetail;
                oIssue.UserDetails.MakeDate = DateTime.Now;
                ucUserDet.ResetData();

                IssueDAL oIssueDAL = new IssueDAL();
                Result oResult = (Result)oIssueDAL.SaveOnlineIssue(oIssue);

                if (oResult.Status)
                {
                    LoadPreviousList();
                    TotalClear();
                    txtIssueDate.Text = DateTime.Today.ToString(Constants.DATETIME_FORMAT);
                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);

                }
                else
                {
                    if (oResult.Message.Equals(Constants.TABLE_MAIN))
                    {
                        ucMessage.OpenMessage(Constants.MSG_APPROVED_SAVE_DATA, Constants.MSG_TYPE_INFO);
                    }
                    else
                    {
                        ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
                    }
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdTransNo.Value))
            {
                Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
                oIssue.IssueTransNo = hdTransNo.Value;
                IssueDAL oIssueDAL = new IssueDAL();
                oIssue.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oIssueDAL.ApproveOnlineIssue(oIssue);
                if (oResult.Status)
                {
                    LoadPreviousList();
                    TotalClear();
                    txtIssueDate.Text = DateTime.Today.ToString(Constants.DATETIME_FORMAT);
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

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdTransNo.Value))
            {
                Issue oIssue = new Issue(hdTransNo.Value);
                IssueDAL oIssueDAL = new IssueDAL();
                oIssue.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oIssueDAL.RejectOldCustomer(oIssue);
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


        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdTransNo.Value))
            {
                IssueDAL oIssueDAL = new IssueDAL();
                Result oResult = (Result)oIssueDAL.DeteteOldCustomer(hdTransNo.Value);
                if (oResult.Status)
                {
                    LoadPreviousList();
                    TotalClear();
                    txtIssueDate.Text = DateTime.Today.ToString(Constants.DATETIME_FORMAT);
                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_DELETE, Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                    if (oResult.Message.Equals(Constants.TABLE_MAIN))
                    {
                        ucMessage.OpenMessage(Constants.MSG_APPROVED_DELETE_DATA, Constants.MSG_TYPE_INFO);
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

        protected void btnReset_Click(object sender, EventArgs e)
        {
            TotalClear();
            txtIssueDate.Text = DateTime.Today.ToString(Constants.DATETIME_FORMAT);
            ControlStatus(true);
            ControlManager();
        }

        protected void txtIssueDate_TextChanged(object sender, EventArgs e)
        {
            string sSpType = ddlSpType.SelectedValue;

            if (!string.IsNullOrEmpty(ddlSpType.SelectedValue))
            {
                TotalClear();

                if (string.IsNullOrEmpty(txtIssueDate.Text))
                {                    
                    ucMessage.OpenMessage("Issue Date cannot be empty!!", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);                    
                }
                else
                {
                    DDListUtil.Assign(ddlSpType, sSpType);

                    txtBHDHolderName.Enabled = false;
                    txtBHDAddress.Enabled = false;
                    txtBHDRelation.Enabled = false;

                    SPPolicyDAL spDal = new SPPolicyDAL();
                    Result oResult = spDal.IsExistPolicy(ddlSpType.SelectedValue);
                    if (oResult.Status)
                    {
                        int i = (int)oResult.Return;
                        if (i > 0)
                        {
                            LoadBySPType();
                            if (ddlSpType.SelectedValue == "WDB")
                            {
                                txtBHDHolderName.Enabled = true;
                                txtBHDAddress.Enabled = true;
                                txtBHDRelation.Enabled = true;
                            }
                        }
                        else
                        {
                            ucMessage.OpenMessage("No policy has been set for " + sSpType + ". Please check.", Constants.MSG_TYPE_INFO);
                            ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        }
                        txtRegistrationNo.Focus();
                    }
                }
            }
            else
            {
                ddlCustomerType.Items.Clear();
            }
        }

        protected void txtAccountNo_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAccountNo.Text))
            {
                if (txtAccountNo.Text.Length < 12)
                {
                    ucMessage.OpenMessage("Account no must be 12 digit (Only Account No.)", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    txtAccountName.Text = "";
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
                            oResult = oIssueDAL.LoadCheckLimit(ddlSpType.SelectedValue, txtAccountNo.Text.Substring(0, 9), "", "", "", Convert.ToDecimal(txtAppliedAmount.Text));
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
                            oResult = oIssueDAL.LoadCheckLimit(ddlSpType.SelectedValue, txtAccountNo.Text.Substring(0, 9), "", "", "", Convert.ToDecimal(txtAppliedAmount.Text) / 2);
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

                string sCurrencyCode = "00";
                Result oResultA = oCDDal.LoadDataFromBDDB2ByAccountNo(txtAccountNo.Text + sCurrencyCode);
                if (oResultA.Status)
                {
                    DataTable dt = (DataTable)oResultA.Return;
                    if (dt.Rows.Count != 0)
                    {
                        txtAccountName.Text = Convert.ToString(dt.Rows[0]["AciAccName"]);
                    }
                    else
                    {
                        txtAccountName.Text = string.Empty;
                    }
                }
                else
                {
                    txtAccountName.Text = "";
                }
            }
            else
            {
                txtAccountName.Text = "";
            }
        }

        protected void ddlPDPaymentMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Set_PaymentModeControls();
        }
        private void Set_PaymentModeControls()
        {
            if (ddlPDPaymentMode.SelectedValue != "1")
            {
                txtAccountNo.Text = "";
                txtAccountNo.Enabled = true;
                txtAccountName.Text = "";
                txtCHQNo.Text = "";
                txtCHQNo.Enabled = true;
                txtRoutingNo.Text = "";
                txtRoutingNo.Enabled = true;
                ddlAccountType.SelectedIndex = 0;
                ddlAccountType.Enabled = true;
                ddlAccountBranch.SelectedIndex = 0;
                ddlAccountBranch.Enabled = true;
            }
            else
            {
                txtAccountNo.Text = "";
                txtAccountNo.Enabled = true;
                txtAccountName.Text = "";
                txtCHQNo.Text = "";
                txtCHQNo.Enabled = false;
                txtRoutingNo.Text = "";
                txtRoutingNo.Enabled = false;
                ddlAccountType.SelectedIndex = 0;
                ddlAccountType.Enabled = false;
                ddlAccountBranch.SelectedIndex = 0;
                ddlAccountBranch.Enabled = false;
            }
        }
        protected void ddlAccountBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlAccountBranch.SelectedIndex > 0)
            {
                IssueDAL oIssueDAL = new IssueDAL();
                Result oResult = new Result();
                oResult = oIssueDAL.GetRoutingNoByBranchID(ddlAccountBranch.SelectedValue);
                txtRoutingNo.Text = oResult.Return.ToString();
            }
        }

    }
}
