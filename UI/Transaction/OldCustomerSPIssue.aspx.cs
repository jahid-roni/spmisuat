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

namespace SBM_WebUI.mp
{
    public partial class OldCustomerSPIssue : System.Web.UI.Page
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
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            DDListUtil.LoadDDLFromDB(ddlBranch, "BranchID", "BranchName", "SPMS_Branch", true);
            ddlBranch.Text = oConfig.BranchID;

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();

            gvScripDetail.DataSource = null;
            gvScripDetail.DataBind();

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            gvData.DataSource = null;
            gvData.DataBind();

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
                    Util.ControlEnabled(txtIssueDate, false);
                    Util.ControlEnabled(ddlCustomerType, false);
                    Util.ControlEnabled(txtAppliedAmount, false);
                    Util.ControlEnabled(txtRegistrationNo, false);

                    //Nominee(s) Details
                    Util.ControlEnabled(txtNDName, false);
                    Util.ControlEnabled(txtNDRelation, false);
                    Util.ControlEnabled(txtNDAddress, false);
                    Util.ControlEnabled(txtNDShare, false);
                    

                    gvNomDetail.Enabled = false;
                    gvCustomerDetail.Enabled = false;

                    // issue name
                    Util.ControlEnabled(txtIssueName, false);
                    Util.ControlEnabled(txtMasterNo, false);

                    // Bond Holder Details
                    Util.ControlEnabled(txtBHDHolderName, false);
                    Util.ControlEnabled(txtBHDAddress, false);
                    Util.ControlEnabled(txtBHDRelation, false);

                    // Certificate(s) Detail
                    Util.ControlEnabled(txtTotalAmount, false);
                    gvScripDetail.Enabled = false;
                    Util.ControlEnabled(ddlCDDenom, false);
                    Util.ControlEnabled(txtCDSeriesFrom, false);
                    Util.ControlEnabled(txtCDSeriesTo, false);
                    Util.ControlEnabled(txtCDQuantity, false);

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
                    Util.ControlEnabled(btnNDReset, false);
                    Util.ControlEnabled(btnAddCertificate, false);

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
                gvScripDetail.Enabled = true;
                Util.ControlEnabled(ddlCDDenom, true);
                Util.ControlEnabled(txtCDSeriesFrom, true);
                Util.ControlEnabled(txtCDSeriesTo, true);
                Util.ControlEnabled(txtCDQuantity, false);               

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


        private void TotalClear()
        {
            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();

            gvScripDetail.DataSource = null;
            gvScripDetail.DataBind();

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
            txtNDName.Text = "";
            txtNDRelation.Text = "";
            txtNDAddress.Text = "";
            txtNDShare.Text = "";
            txtNDAmount.Text = "";

            // issue name
            txtIssueName.Text = "";
            txtMasterNo.Text = "";

            // Bond Holder Details
            txtBHDHolderName.Text = "";
            txtBHDAddress.Text = "";
            txtBHDRelation.Text = "";

            // Certificate(s) Detail
            txtTotalAmount.Text = "";
            ddlCDDenom.Items.Clear();
            txtCDSeries.Text = "";
            txtCDSeriesFrom.Text = "";
            txtCDSeriesTo.Text = "";
            txtCDQuantity.Text = "";
            ucUserDet.ResetData();
            
            hdNomSlno.Value = "";
            Session[Constants.SES_CURRENT_ISSUE] = null;
        }

        private void LoadDataByRegNo(string sRegNo)
        {
            Issue oIssue = new Issue();
            oIssue.RegNo = sRegNo != "" ? sRegNo : txtRegistrationNo.Text.Trim();
            IssueDAL oIssueDAL = new IssueDAL();
            Result oResult = oIssueDAL.LoadOldCustomerDataByRegNo(oIssue);

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

                txtTotalAmount.Text = oIssue.IssueAmount.ToString();
                txtIssueDate.Text = oIssue.VersionIssueDate.ToString(Constants.DATETIME_FORMAT);
                txtRegistrationNo.Text = oIssue.RegNo;

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
                        rowCD["bfDateOfBirth"] = oIssue.CustomerDetailsList[i].DateOfBirth.ToString("dd-MMM-yyyy");
                        rowCD["bfAddress"] = oIssue.CustomerDetailsList[i].Address.ToString();
                        rowCD["bfPhone"] = oIssue.CustomerDetailsList[i].Phone.ToString();
                        //rowCD["bfDateOfBirth2"] = oIssue.CustomerDetailsList[i].DateOfBirth2.ToString("dd-MMM-yyyy");
                        rowCD["bfNationality"] = oIssue.CustomerDetailsList[i].Nationality.ToString();
                        rowCD["bfPassportNo"] = oIssue.CustomerDetailsList[i].PassportNo.ToString();
                        rowCD["bfForeignAddress"] = oIssue.CustomerDetailsList[i].ForeignAddress.ToString();

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
                }
                // end of nominee loading..


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
                gvScripDetail.DataSource = dtDenom;
                gvScripDetail.DataBind();
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
            gvScripDetail.Enabled = bStatus;
            Util.ControlEnabled(ddlCDDenom, bStatus);
            Util.ControlEnabled(txtCDSeriesFrom, bStatus);
            Util.ControlEnabled(txtCDSeriesTo, bStatus);
            Util.ControlEnabled(txtCDQuantity, bStatus);
            chkFiscalYear.Enabled = bStatus;
            chkFiscalYear.Checked = false;

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
                AddNomineeToSession(new Issue());

                txtNDName.Text = string.Empty;
                txtNDRelation.Text = string.Empty;
                txtNDAddress.Text = string.Empty;
                txtNDShare.Text = string.Empty;
                txtNDAmount.Text = string.Empty;
            }
            hdNomSlno.Value = "";
        }

        protected void btnAddCertificate_Click(object sender, EventArgs e)
        {
            Result oResult = new Result();
            ReceiveDAL oRDal = new ReceiveDAL();
            oResult = oRDal.IsExist(ddlSpType.SelectedValue, ddlCDDenom.SelectedValue, txtCDSeries.Text, txtCDSeriesFrom.Text, txtCDSeriesTo.Text);
            if (oResult.Status)
            {                
                int iCount = (int)oResult.Return;
                if (iCount == 0)
                {
                    Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];

                    if (oIssue != null)
                    {
                        AddScripToSession(oIssue);
                    }
                    else
                    {
                        AddScripToSession(new Issue());
                    }
                    ddlCDDenom.SelectedIndex = 0;
                    txtCDSeries.Text = "";
                    txtCDSeriesFrom.Text = "";
                    txtCDSeriesTo.Text = "";
                    txtCDQuantity.Text = "";
                }
                else
                {
                    ucMessage.OpenMessage("This certificates already exist. Please check..", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
        }

        private void AddScripToSession(Issue oIssue)
        {
            if (!string.IsNullOrEmpty(ddlCDDenom.SelectedValue))
            {
                // need to check that can be possible to add or not
                int iOldAmount = 0;
                for (int i = 0; i < oIssue.ScripList.Count; i++)
                {
                    if (oIssue.ScripList[i].Denomination.DenominationID != Util.GetIntNumber(ddlCDDenom.SelectedValue))
                    {
                        iOldAmount = iOldAmount + oIssue.ScripList[i].Denomination.DenominationID;
                    }
                }

                iOldAmount = iOldAmount + (Util.GetIntNumber(ddlCDDenom.SelectedValue) * Util.GetIntNumber(txtCDQuantity.Text));
                if (Convert.ToInt32(Util.GetDecimalNumber(txtAppliedAmount.Text)) >= iOldAmount)
                {
                    for (int i = Util.GetIntNumber(txtCDSeriesFrom.Text); i <= Util.GetIntNumber(txtCDSeriesTo.Text); i++)
                    {
                        Scrip oScrip = oIssue.ScripList.Where(d => d.Denomination.DenominationID.Equals(Convert.ToInt32(ddlCDDenom.SelectedValue)) && d.SlNo.Equals(i.ToString().PadLeft(7, '0'))).SingleOrDefault();
                        if (oScrip == null)
                        {
                            oScrip = new Scrip();
                            oScrip.Denomination.DenominationID = Util.GetIntNumber(ddlCDDenom.SelectedValue);
                            oScrip.SPSeries = txtCDSeries.Text;
                            oScrip.SlNo = i.ToString().PadLeft(7, '0');
                            oScrip.isEncashed = false;
                            //Add to List
                            oScrip.UserDetails = ucUserDet.UserDetail;
                            oIssue.ScripList.Add(oScrip);
                        }
                    }

                    oIssue.ScripList = oIssue.ScripList.OrderBy(s => s.Denomination.DenominationID).ToList();

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

                    // if not exceed total amount
                    if (Convert.ToInt32(Util.GetDecimalNumber(txtAppliedAmount.Text)) >= iTotalAmount)
                    {
                        //Reload Grid                    
                        gvScripDetail.DataSource = dtDenom;
                        gvScripDetail.DataBind();
                        txtTotalAmount.Text = iTotalAmount.ToString("N2");

                        //Update Session
                        Session[Constants.SES_CURRENT_ISSUE] = oIssue;
                    }
                    else
                    {
                        ucMessage.OpenMessage("Total amount cannot be exceeded.", Constants.MSG_TYPE_INFO);
                    }                    
                }
                else
                {
                    ucMessage.OpenMessage("Total amount cannot be exceeded.", Constants.MSG_TYPE_INFO);
                }
            }
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

                //Reload Grid
                gvScripDetail.DataSource = dtDenom;
                gvScripDetail.DataBind();

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
            oNominee.Amount = Util.GetDecimalNumber(txtNDAmount.Text);
            oNominee.IssueTransNo = Convert.ToString(1);
            oNominee.NomineeName = txtNDName.Text;
            oNominee.NomineeShare = Util.GetDecimalNumber(txtNDShare.Text);
            oNominee.Relation = txtNDRelation.Text;
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
            Result oResult = oIssueDAL.LoadUnapprovedOldCustomerIssueList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);
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
            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
            // To Check MasterID
            CustomerDetails oCustMastExist = oIssue.CustomerDetailsList.Where(c => c.MasterNo.Equals(oCustomer.MasterNo)).SingleOrDefault();
            // to check adult user or not..
            DateTime dtCustAdultDate = new DateTime(oCustomer.DateOfBirth.Year + 18, oCustomer.DateOfBirth.Month, oCustomer.DateOfBirth.Day);
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
                CustomerDetails oCustDetlExist = oIssue.CustomerDetailsList.Where(c => c.MasterNo.Equals(oCustMastExist.MasterNo)).SingleOrDefault();
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
            dtCustomerDetail.Columns.Add(new DataColumn("bfDateOfBirth2", typeof(DateTime)));
            dtCustomerDetail.Columns.Add(new DataColumn("bfNationality", typeof(string)));
            dtCustomerDetail.Columns.Add(new DataColumn("bfPassportNo", typeof(string)));
            dtCustomerDetail.Columns.Add(new DataColumn("bfForeignAddress", typeof(string)));

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
                rowCD["bfForeignAddress"] = oIssue.CustomerDetailsList[i].ForeignAddress.ToString();

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
            txtIssueName.Text = issueName;
            txtBHDHolderName.Text = oCustomer.CustomerName;
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
                ControlStatus(true);
            }
        }

        protected void ddlSpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sSpType = ddlSpType.SelectedValue;            
            TotalClear();
            DDListUtil.Assign(ddlSpType, sSpType);
            txtIssueDate.Text = string.Empty;
            txtIssueDate.Focus();           
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
                ddlCDDenom.Items.Clear();

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
                DDListUtil.Assign(ddlCDDenom, dtDenom, true);
                
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
            gvScripDetail.DataSource = null;
            gvScripDetail.DataBind();

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
            if (oIssue != null)
            {
                oIssue.NomineeList.Clear();
                oIssue.ScripList.Clear();
                Session[Constants.SES_CURRENT_ISSUE] = oIssue;
            }
            txtTotalAmount.Text = string.Empty;
            txtNDName.Text = string.Empty;
            txtNDRelation.Text = string.Empty;
            txtNDAddress.Text = string.Empty;
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
        protected void ddlCDDenom_SelectedIndexChanged(object sender, EventArgs e)
        {
            SPTypeDAL oSPTypeDAL = new SPTypeDAL();
            if (!string.IsNullOrEmpty(ddlCDDenom.SelectedValue))
            {
                Result oResult = (Result)oSPTypeDAL.GetSeriesName(ddlSpType.SelectedValue, ddlCDDenom.SelectedValue);
                if (oResult.Status)
                {
                    DataTable dtSeries = (DataTable)oResult.Return;
                    if (dtSeries.Rows.Count > 0)
                    {
                        txtCDSeries.Text = Convert.ToString(dtSeries.Rows[0]["SPSeries"]);
                        //hdDigitsInSlNo.Value = Convert.ToString(dtSeries.Rows[0]["DigitsInSlNo"]);
                    }
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
                dtCustomerDetail.Columns.Add(new DataColumn("bfDateOfBirth2", typeof(DateTime)));
                dtCustomerDetail.Columns.Add(new DataColumn("bfNationality", typeof(string)));
                dtCustomerDetail.Columns.Add(new DataColumn("bfPassportNo", typeof(string)));
                dtCustomerDetail.Columns.Add(new DataColumn("bfForeignAddress", typeof(string)));

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
                    rowCD["bfForeignAddress"] = oIssue.CustomerDetailsList[i].ForeignAddress.ToString();

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
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_OLD).PadLeft(5, '0'), false);
            }
            else
            {
                ddlSpType.Focus();
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_OLD).PadLeft(5, '0'), false);
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
            if (dCount == 0 || dCount == 100)
            {
                SaveAction();
            }
            else
            {
                ucMessage.OpenMessage("Total amount of share must be 100!!", Constants.MSG_TYPE_INFO);
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
                //oIssue.RegNo = txtRegistrationNo.Text;
                oIssue.CustomerType.CustomerTypeID = ddlCustomerType.SelectedValue != "" ? ddlCustomerType.SelectedValue : "";
                oIssue.IssueAmount = Util.GetDecimalNumber(txtTotalAmount.Text);

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
                Result oResult = (Result)oIssueDAL.SaveOldCustomer(oIssue);

                if (oResult.Status)
                {
                    LoadPreviousList();
                    TotalClear();
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

                Result oResult = (Result)oIssueDAL.ApproveOldCustomer(oIssue);
                if (oResult.Status)
                {
                    LoadPreviousList();
                    TotalClear();
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
            ControlStatus(true);
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
                ddlCDDenom.Items.Clear();
            }
        }
    }
}
