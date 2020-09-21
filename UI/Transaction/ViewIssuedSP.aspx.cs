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
using SBM_BLC1.Entity.Transaction;
using SBM_BLC1.Transaction;
using SBM_BLC1.Entity.Configuration;
using System.Collections;
using SBM_BLC1.Common;
using SBM_BLC1.DAL.Common;
using System.Drawing;
using System.Text;
//using SBM_BLC1.DAL.Transaction;

namespace SBM_WebUI.mp
{
    public partial class ViewIssuedSP : System.Web.UI.Page
    {
        #region Local Variable
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_REG_NO = "RegNo";
        public const string OBJ_PAGE_ID = "sPageID";
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_TRANS.SP_ISSUE_VIEW))
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
            hdRegNo.Value = ""; 
            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            DDListUtil.LoadDDLFromDB(ddlBranch, "BranchID", "BranchName", "SPMS_Branch", true);
            DDListUtil.LoadDDLFromDB(ddlCollectionBranch, "BranchID", "BranchName", "SPMS_Branch", true);
            DDListUtil.LoadDDLFromDB(ddlPDPaymentMode, "PaymentMode", "Description", "SPMS_PaymentMode", true);
            DDListUtil.LoadDDLFromDB(ddlPDCurrency, "CurrencyID", "CurrencyCode", "SPMS_Currency", true);
            // end of Intial Data

            // Issue Details
            txtIssueDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            Util.ControlEnabled(ddlSpType, false);
            Util.ControlEnabled(ddlBranch, false);
            Util.ControlEnabled(ddlCollectionBranch, false);
            Util.ControlEnabled(txtIssueDate, false);
            
            Util.ControlEnabled(ddlCustomerType, false);
            Util.ControlEnabled(txtAppliedAmount, false);

            // Bond Holder Details
            Util.ControlEnabled(txtBHDHolderName, false);
            //Util.ControlEnabled(txtIssueName, false);
            Util.ControlEnabled(txtBHDAddress, false);
            Util.ControlEnabled(txtBHDRelation, false);

            // payment mode
            Util.ControlEnabled(ddlPDPaymentMode, false);
            Util.ControlEnabled(ddlPDCurrency, false);
            Util.ControlEnabled(txtPDAccDraftNo, false);
            Util.ControlEnabled(txtPDAccName, false);
            Util.ControlEnabled(txtPDConvRate, false);
            Util.ControlEnabled(txtPDPaymentAmount, false);


            Util.ControlEnabled(ucUserDet.FindControl("txtMakerId"), false);
            Util.ControlEnabled(ucUserDet.FindControl("txtMakeDate"), false);
            Util.ControlEnabled(ucUserDet.FindControl("txtCheckerId"), false);
            Util.ControlEnabled(ucUserDet.FindControl("txtCheckDate"), false);
            Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), false);

            gvCustomerDetail.DataSource = null;
            gvCustomerDetail.DataBind();

            gvNomDetail.DataSource = null;
            gvNomDetail.DataBind();

            //gvNomDemon.DataSource = null;
            //gvNomDemon.DataBind();

            gvDenomDetail.DataSource = null;
            gvDenomDetail.DataBind();

            //ddlTaxYear.SelectedValue = DateTime.Today.Year.ToString();

            string sRegNo = "";
            sRegNo=Request.QueryString["qRegNo"];
            if (!string.IsNullOrEmpty(sRegNo))
            {
                sRegNo = oCrypManager.GetDecryptedString(sRegNo, Constants.CRYPT_PASSWORD_STRING);
            }
            if (sRegNo == null)
            {
                sRegNo = "";
            }
            if (sRegNo.Trim() != "")
            {
                LoadDataByID(sRegNo);
            }
            
        }
        #endregion InitializeData


        protected void btnLoad_Click(object sender, EventArgs e)
        {
            if (txtRegistrationNo.Text.Length > 0)
            {
                LoadDataByID(txtRegistrationNo.Text);
            }
        }

        public void PopupIssueSearchLoadAction(string sRegNo,  string sTransNo, string sApprovalStaus)
        {
            if (!string.IsNullOrEmpty(sRegNo))
            {
                LoadDataByID(sRegNo);
            }
        }

        public void PopErrorMsgAction(string sType)
        {
            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
               // Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_WISEA_CCOUNT).PadLeft(5, '0'), false);
            }
            else
            {
                // no action
            }
        }

        protected void btnLeftShift_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdRegNo.Value))
            {
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                Issue oIssue = new Issue();
                oIssue.RegNo = hdRegNo.Value;
                oIssue.DivisionID = oConfig.DivisionID;
                oIssue.BankID = oConfig.BankCodeID;

                IssueDAL oIssueDAL = new IssueDAL();
                Result oResult = oIssueDAL.LoadViewDataID(oIssue, "L");

                if (oResult.Status)
                {
                    oIssue = (Issue)oResult.Return;
                    SetObject(oIssue);
                }
                else
                {
                    hdRegNo.Value = "";
                    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                }
            }
            else
            {
                ucMessage.OpenMessage("You must load Issue first before viewing Issue Detail!", Constants.MSG_TYPE_INFO);
            }
        }

        protected void btnRegSearch_Click(object sender, EventArgs e)
        {            
            if (!string.IsNullOrEmpty(txtRegistrationNo.Text))
            {
                LoadDataByID(txtRegistrationNo.Text);
            }
        }

        protected void btnRightShift_Click(object sender, EventArgs e)
        {            
            if (!string.IsNullOrEmpty(hdRegNo.Value))
            {
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                Issue oIssue = new Issue();
                oIssue.RegNo = hdRegNo.Value;
                oIssue.DivisionID = oConfig.DivisionID;
                oIssue.BankID = oConfig.BankCodeID;
                IssueDAL oIssueDAL = new IssueDAL();
                Result oResult = oIssueDAL.LoadViewDataID(oIssue, "R");

                if (oResult.Status)
                {
                    oIssue = (Issue)oResult.Return;
                    SetObject(oIssue);
                }
                else
                {
                    hdRegNo.Value = "";
                    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                }
            }
            else
            {
                ucMessage.OpenMessage("You must load Issue first before viewing Issue Detail!", Constants.MSG_TYPE_INFO);
            }
        }

        private void SetObject(Issue oIssue)
        {
            if (oIssue != null)
            {
                if (Session[Constants.SES_CURRENT_ISSUE] != null)
                {
                    //Store Issue object to Session
                    Session[Constants.SES_CURRENT_ISSUE] = oIssue;
                }
                else
                {
                    Session.Add(Constants.SES_CURRENT_ISSUE, oIssue);
                }
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                //if (!oConfig.UserRole.Contains("HO"))
                //{
                //    if (!oConfig.RMBranchID.Contains(oIssue.Branch.BranchID.Trim()) && !oConfig.RMBranchID.Contains(oIssue.CollectionBranch.Trim()))
                //    {
                //        ucMessage.OpenMessage("Data you requested not associated with your branch!", Constants.MSG_TYPE_INFO);
                //        return;
                //    }
                //}
                hdRegNo.Value = oIssue.RegNo;
                txtRegistrationNo.Text = oIssue.RegNo;
                txtAppliedAmount.Text = oIssue.IssueAmount.ToString("N2");
                txtIssueName.Text = oIssue.IssueName;
                txtIssueDate.Text = oIssue.VersionIssueDate.ToString(Constants.DATETIME_FORMAT);
                txtBHDAddress.Text = oIssue.BondHolderAddress;
                txtBHDHolderName.Text = oIssue.BondHolderName;
                txtBHDRelation.Text = oIssue.BondHolderRelation;

                ucUserDet.UserDetail = oIssue.UserDetails;

                LoadCustomerTypeBySpType(oIssue.SPType.SPTypeID);
                DDListUtil.Assign(ddlSpType, oIssue.SPType.SPTypeID);
                DDListUtil.Assign(ddlCustomerType, oIssue.CustomerType.CustomerTypeID);
                DDListUtil.Assign(ddlBranch, oIssue.Branch.BranchID);
                DDListUtil.Assign(ddlCollectionBranch, oIssue.CollectionBranch);
                DDListUtil.Assign(ddlPDCurrency, oIssue.Currency.CurrencyID);

                // payment 
                DDListUtil.Assign(ddlPDPaymentMode, oIssue.Payment.PaymentMode);
                txtPDConvRate.Text = oIssue.Payment.ConvRate.ToString();
                txtPDPaymentAmount.Text = oIssue.Payment.PaymentAmount.ToString("N2");
                txtPDAccDraftNo.Text = oIssue.Payment.AccountNo.ToString();
                txtPDAccName.Text = oIssue.AccountName;
                // end of payment 

                // customer Loading 
                gvCustomerDetail.DataSource = null;
                gvCustomerDetail.DataBind();
                if (oIssue.CustomerDetailsList.Count > 0)
                {
                    DataTable dtCustomerDetail = new DataTable("dtCustomerDetail");

                    dtCustomerDetail.Columns.Add(new DataColumn("bfCustomerID", typeof(string)));
                    dtCustomerDetail.Columns.Add(new DataColumn("bfCustomerName", typeof(string)));
                    dtCustomerDetail.Columns.Add(new DataColumn("bfAddress", typeof(string)));                    
                    dtCustomerDetail.Columns.Add(new DataColumn("bfDateOfBirth", typeof(string)));                    
                    dtCustomerDetail.Columns.Add(new DataColumn("bfPhone", typeof(string)));                    
                    dtCustomerDetail.Columns.Add(new DataColumn("bfPassportNo", typeof(string)));
                    dtCustomerDetail.Columns.Add(new DataColumn("bfForeignAddress", typeof(string)));

                    DataRow rowCD = null;
                    for (int i = 0; i < oIssue.CustomerDetailsList.Count; i++)
                    {
                        rowCD = dtCustomerDetail.NewRow();

                        rowCD["bfCustomerID"] = oIssue.CustomerDetailsList[i].CustomerID.ToString();
                        rowCD["bfCustomerName"] = oIssue.CustomerDetailsList[i].CustomerName.ToString();
                        rowCD["bfAddress"] = oIssue.CustomerDetailsList[i].Address.ToString();

                        if (oIssue.CustomerDetailsList[i].DateOfBirth.Year == 1900)
                        {
                            rowCD["bfDateOfBirth"] = "";
                        }
                        else
                        {
                            rowCD["bfDateOfBirth"] = oIssue.CustomerDetailsList[i].DateOfBirth.ToString("dd-MMM-yyyy");
                        }
                                                
                        rowCD["bfPhone"] = oIssue.CustomerDetailsList[i].Phone.ToString();                        
                        rowCD["bfPassportNo"] = oIssue.CustomerDetailsList[i].PassportNo.ToString();
                        rowCD["bfForeignAddress"] = oIssue.CustomerDetailsList[i].ForeignAddress.ToString();

                        dtCustomerDetail.Rows.Add(rowCD);
                    }
                    gvCustomerDetail.DataSource = dtCustomerDetail;
                    gvCustomerDetail.DataBind();
                }
                // end of loading 


                // start to nominee loading..
                gvNomDetail.DataSource = null;
                gvNomDetail.DataBind();
                if (oIssue.NomineeList.Count > 0)
                {
                    DataTable dtNonimeeData = new DataTable("dtNonimeeData");

                    dtNonimeeData.Columns.Add(new DataColumn("bfSlNo", typeof(string)));
                    dtNonimeeData.Columns.Add(new DataColumn("bfNomineeName", typeof(string)));                    
                    dtNonimeeData.Columns.Add(new DataColumn("bfAddress", typeof(string)));
                    dtNonimeeData.Columns.Add(new DataColumn("bfRelation", typeof(string)));
                    dtNonimeeData.Columns.Add(new DataColumn("bfNomineeShare", typeof(string))); //decimal                    
                    dtNonimeeData.Columns.Add(new DataColumn("bfAmount", typeof(decimal)));

                    DataRow row = null;
                    for (int i = 0; i < oIssue.NomineeList.Count; i++)
                    {
                        row = dtNonimeeData.NewRow();

                        row["bfSlNo"] = oIssue.NomineeList[i].SlNo.ToString();
                        row["bfNomineeName"] = oIssue.NomineeList[i].NomineeName.ToString();                        
                        row["bfRelation"] = oIssue.NomineeList[i].Relation.ToString();
                        row["bfNomineeShare"] = oIssue.NomineeList[i].NomineeShare.ToString();                        
                        row["bfAddress"] = oIssue.NomineeList[i].Address.ToString();                        
                        row["bfAmount"] = oIssue.NomineeList[i].Amount.ToString();

                        dtNonimeeData.Rows.Add(row);
                    }
                    gvNomDetail.DataSource = dtNonimeeData;
                    gvNomDetail.DataBind();

                }
                // end of nominee loading..

                //scrip loading
                gvDenomDetail.DataSource = null;
                gvDenomDetail.DataBind();
                if (oIssue.ScripList.Count > 0)
                {
                    DataTable dtScripData = new DataTable("dtScripData");

                    dtScripData.Columns.Add(new DataColumn("bfDenomination", typeof(string)));
                    dtScripData.Columns.Add(new DataColumn("bfSeries", typeof(string)));
                    dtScripData.Columns.Add(new DataColumn("bfSerialNo", typeof(string)));
                    dtScripData.Columns.Add(new DataColumn("bfStatus", typeof(string)));

                    DataRow row = null;
                    for (int i = 0; i < oIssue.ScripList.Count; i++)
                    {
                        row = dtScripData.NewRow();
                        row["bfDenomination"] = oIssue.ScripList[i].Denomination.DenominationID.ToString();
                        row["bfSeries"] = oIssue.ScripList[i].SPSeries.ToString();
                        row["bfSerialNo"] = oIssue.ScripList[i].SlNo.ToString();
                        row["bfStatus"] = ((Constants.SCRIPT_STATUS)oIssue.ScripList[i].Status).ToString();

                        dtScripData.Rows.Add(row);
                    }

                    gvDenomDetail.DataSource = dtScripData;
                    gvDenomDetail.DataBind();
                }
                //issue document loading
                gvIssueDocument.DataSource = null;
                gvIssueDocument.DataBind();
                if (oIssue.IsDocumentUploaded)
                {
                    DataTable dtIssueDocument = new DataTable("IssueDocument");
                    dtIssueDocument.Columns.Add(new DataColumn("bfIssueDocument", typeof(string)));
                    DataRow row = null;
                    row = dtIssueDocument.NewRow();
                    row["bfIssueDocument"] = " Click \"Document Button\" to view \"" + oIssue.RegNo + "\" issue documents";
                    dtIssueDocument.Rows.Add(row);

                    gvIssueDocument.DataSource = dtIssueDocument;
                    gvIssueDocument.DataBind();
                }
                // end of scrip loading 
            }
        }

        private void LoadCustomerTypeBySpType(string sSPType)
        {
            CustomerTypeWiseSPLimitDAL oCustTypeWiseSPLimit = new CustomerTypeWiseSPLimitDAL();
            Result oResult = (Result)oCustTypeWiseSPLimit.GetCustomerTypeBySpType(sSPType);
            if (oResult.Status)
            {
                ddlCustomerType.Items.Clear();
                DataTable dtGetCustomerTypeID = (DataTable)oResult.Return;
                DDListUtil.Assign(ddlCustomerType, dtGetCustomerTypeID);
            }
        }

        public void LoadDataByID(string sRegNo)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Issue oIssue = new Issue();
            oIssue.RegNo = sRegNo.Trim();
            oIssue.DivisionID = oConfig.DivisionID;
            oIssue.BankID = oConfig.BankCodeID;
            IssueDAL oIssueDAL = new IssueDAL();
            Result oResult = oIssueDAL.LoadViewDataID(oIssue, null);
            
            if (oResult.Status)
            {
                oIssue = (Issue)oResult.Return;
                SetObject(oIssue);
            }
            else
            {
                hdRegNo.Value = "";
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
            }
        }

        protected void btnIssueUpdate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdRegNo.Value))
            {
                Response.Redirect(Constants.PAGE_TRAN_ISSUE_UPDATE + "?sIsUpdate=" + oCrypManager.GetEncryptedString("1") + "&sRegID=" + oCrypManager.GetEncryptedString(txtRegistrationNo.Text) + "&sPageID=" + oCrypManager.GetEncryptedString(Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_UPDATE).PadLeft(5, '0')), false);
            }
            else
            {
                ucMessage.OpenMessage("You must load Issue first before updating Issue Detail!", Constants.MSG_TYPE_INFO);
            }
        }

        protected void btnShowPolicy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdRegNo.Value))
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
            }
            else
            {
                ucMessage.OpenMessage("You must load Issue first before viewing policy Detail!", Constants.MSG_TYPE_INFO);
            }
        }

        protected void gvCustomerDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;

            if (((Button)e.CommandSource).Text.Equals("Select"))
            {
                CustomerDetailsDAL oCustDetlDAL = new CustomerDetailsDAL();

                CustomerDetails oCustDetl = new CustomerDetails();
                oCustDetl.CustomerID = Convert.ToInt32(gvRow.Cells[1].Text);
                oCustDetl.isViewOnly = true;

                Result oResult = oCustDetlDAL.LoadOnlyMainByID(oCustDetl);
                if (oResult.Status)
                {
                    CustomerDetail.SetCustomerDetails(oResult.Return as CustomerDetails);
                }
            }                     
        }

        //protected void btnCertificate_Click(object sender, EventArgs e)
        //{
        //    ReportDAL rdal = new ReportDAL();
        //    Result oResult = new Result();
        //    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];


        //    if (!string.IsNullOrEmpty(txtRegistrationNo.Text))
        //    {

        //        if (oConfig != null)
        //        {
        //            oResult = rdal.InvestmentCertificate(txtRegistrationNo.Text,txtIssueName.Text, oConfig.BranchID);

        //            //Page.RegisterStartupScript(Constants.REPORT_WINDOW, "<script> alert('opended')</script>");

        //            if (oResult.Status)
        //            {
        //                Session[Constants.SES_RPT_DATA] = oResult.Return;
        //                Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
        //            }
        //        }
        //    }
        //    else
        //    {
        //        ucMessage.OpenMessage("Enter a valid reg no. Please check!", Constants.MSG_TYPE_INFO);
        //        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);

        //    }
        //}

        protected void gvDenomDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((Convert.ToString(e.Row.Cells[3].Text).ToUpper() == "LIENED") || Convert.ToString(e.Row.Cells[3].Text).ToUpper() == "DAMAGED")
                {
                    e.Row.BackColor = Color.Red;
                }
                else
                {
                    e.Row.BackColor = gvDenomDetail.RowStyle.BackColor;
                }
            }
        }

        //protected void btnEncashCertificate_Click(object sender, EventArgs e)
        //{
        //    ReportDAL rdal = new ReportDAL();
        //    Result oResult = new Result();
        //    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];


        //    if (!string.IsNullOrEmpty(txtRegistrationNo.Text))
        //    {

        //        if (oConfig != null)
        //        {
        //            oResult = rdal.EncashmentCertificate(txtRegistrationNo.Text, txtIssueName.Text, oConfig.BranchID);

        //            //Page.RegisterStartupScript(Constants.REPORT_WINDOW, "<script> alert('opended')</script>");

        //            if (oResult.Status)
        //            {
        //                Session[Constants.SES_RPT_DATA] = oResult.Return;
        //                Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
        //            }
        //        }
        //    }
        //    else
        //    {
        //        ucMessage.OpenMessage("Enter a valid reg no. Please check!", Constants.MSG_TYPE_INFO);
        //        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);

        //    }
        //}
        //protected void btnPaymentCertificate_Click(object sender, EventArgs e)
        //{
        //    ReportDAL rdal = new ReportDAL();
        //    Result oResult = new Result();
        //    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];


        //    if (!string.IsNullOrEmpty(txtRegistrationNo.Text))
        //    {

        //        if (oConfig != null)
        //        {
        //            oResult = rdal.PaymentCertificate(txtRegistrationNo.Text, txtIssueName.Text, oConfig.BranchID,ddlTaxYear.SelectedValue.ToString());

        //            //Page.RegisterStartupScript(Constants.REPORT_WINDOW, "<script> alert('opended')</script>");

        //            if (oResult.Status)
        //            {
        //                Session[Constants.SES_RPT_DATA] = oResult.Return;
        //                Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
        //            }
        //        }
        //    }
        //    else
        //    {
        //        ucMessage.OpenMessage("Enter a valid reg no. Please check!", Constants.MSG_TYPE_INFO);
        //        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);

        //    }
        //}

        

        protected void btnIssueDocument_Click(object sender, EventArgs e)
        {
            IssueDAL oIssueDAL = new IssueDAL();
            Result oResult = oIssueDAL.LoadIssueDocument_PDF(txtRegistrationNo.Text.Trim());
            if (oResult.Status)
            {
                if (((DataTable)oResult.Return).Rows.Count > 0)
                {
                    Session[Constants.SES_PDF_DATA] = (byte[])(((DataTable)oResult.Return).Rows[0]["IssuePDFFile"]);
                    Page.RegisterStartupScript(Constants.PDF_WINDOW, Util.OpenPDFView());
                    //byte[] oRepDoc = null;
                    //oRepDoc = (byte[])(((DataTable)oResult.Return).Rows[0]["IssuePDFFile"]);
                    //Response.Buffer = true;
                    //Response.Charset = "";
                    ////if (Request.QueryString["download"] == "1")
                    ////{
                    //Response.AppendHeader("Content-Disposition", "attachment; filename=IssueDocument_" + DateTime.Today.ToString("yyyyMMdd"));
                    ////}
                    //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    //Response.ContentType = "application/pdf";
                    //Response.BinaryWrite(oRepDoc);
                    //Response.Flush();
                    //Response.End();
                }
            }
        }

        protected void gvNomDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = null;

            if (((Button)e.CommandSource).Text.Equals("View"))
            {
                gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                hdNomSlno.Value = ((HiddenField)gvRow.FindControl("hdNomineeSlno")).Value;
                Issue oIssue = (Issue)Session[Constants.SES_CURRENT_ISSUE];
                Nominee oNominee = oIssue.NomineeList.Where(n => n.SlNo.Equals(Convert.ToInt32(hdNomSlno.Value))).SingleOrDefault();
                //DateTime parsedDate;
                //DateTime.TryParseExact(gvRow.Cells[7].Text, Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                string sNomDetails = "";

                sNomDetails += "<fieldset><legend>Nominee Details</legend>";
                sNomDetails += "<table class=\"tableBody\" border=\"0\">";

                sNomDetails += "<tr><td>Name: </td><td>" + oNominee.NomineeName + "</td><tr>";
                sNomDetails += "<tr><td>Relation: </td><td>" + oNominee.Relation + "</td><tr>";
                sNomDetails += "<tr><td>Present Address: </td><td>" + oNominee.Address + "</td><tr>";
                sNomDetails += "<tr><td>Parmanent Address: </td><td>" + oNominee.ParmanentAddress + "</td><tr>";
                sNomDetails += "<tr><td>Share: </td><td>" + oNominee.NomineeShare.ToString("N2") + "</td><tr>";
                sNomDetails += "<tr><td>Amount: </td><td>" + oNominee.Amount.ToString("N2") + "</td><tr>";

                if (oNominee.DateOfBirth.Year > 1900)
                {
                    sNomDetails += "<tr><td>DOB:</td><td>" + oNominee.DateOfBirth.ToString(Constants.DATETIME_dd_MMM_yyyy) + "</td><tr>";
                }
                else
                {
                    sNomDetails += "<tr><td>DOB:</td><td></td><tr>";
                }

                sNomDetails += "<tr><td>Gender: </td><td>" + oNominee.Sex + "</td><tr>";
                sNomDetails += "<tr><td>Resident: </td><td>" + oNominee.ResidentStatus + "</td><tr>";
                sNomDetails += "<tr><td>Country: </td><td>" + oNominee.Resident_Country + "</td><tr>";
                sNomDetails += "<tr><td>NationalID: </td><td>" + oNominee.NationalID + "</td><tr>";
                sNomDetails += "<tr><td>NationalID Country: </td><td>" + oNominee.NationalID_Country + "</td><tr>";

                sNomDetails += "<tr><td>PassportNo: </td><td>" + oNominee.PassportNo + "</td><tr>";
                sNomDetails += "<tr><td>PassportNo Country: </td><td>" + oNominee.PassportNo_Country + "</td><tr>";
                sNomDetails += "<tr><td>BirthCertificateNo: </td><td>" + oNominee.BirthCertificateNo + "</td><tr>";
                sNomDetails += "<tr><td>BirthCertificateNo Country: </td><td>" + oNominee.BirthCertificateNo_Country + "</td><tr>";

                sNomDetails += "<tr><td>TIN: </td><td>" + oNominee.TIN + "</td><tr>";
                sNomDetails += "<tr><td>Phone: </td><td>" + oNominee.Phone + "</td><tr>";
                sNomDetails += "<tr><td>Email: </td><td>" + oNominee.EmailAddress + "</td><tr>";
                sNomDetails += "</table>";
                sNomDetails += "</fieldset>";
                UCMessageInfo.OpenMessage(sNomDetails, Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel6, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);

            }
        }
    }
}
