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
using SBM_BLC1.DAL.Common;
using System.Drawing;
//using SBM_BLC1.DAL.Transaction;

namespace SBM_WebUI.mp
{
    public partial class SPIssueCertificate : System.Web.UI.Page
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
            hdIssueTransNo.Value = "";
            hdChargeAmount.Value = "";

            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            DDListUtil.LoadDDLFromDB(ddlBranch, "BranchID", "BranchName", "SPMS_Branch", true);
            // end of Intial Data

            // Issue Details
            txtIssueDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            Util.ControlEnabled(ddlSpType, false);
            Util.ControlEnabled(ddlBranch, false);
            Util.ControlEnabled(txtIssueDate, false);
            
            Util.ControlEnabled(ddlCustomerType, false);
            Util.ControlEnabled(txtAppliedAmount, false);

            // Bond Holder Details
            Util.ControlEnabled(txtBHDHolderName, false);
            //Util.ControlEnabled(txtIssueName, false);
            Util.ControlEnabled(txtBHDAddress, false);
            Util.ControlEnabled(txtBHDRelation, false);



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

            ddlCertificateType.SelectedIndex = 0;
            ddlCertCustomerType.SelectedValue = "REGULAR";
            txtLastCertDate.Text = "";
            txtNoofCertPrint.Text = "0";
            txtLastMaker.Text = "";
            txtDuplicate.Text = "";
            ddlWaiveCharges.SelectedIndex = 0;
            txtChargeAmount.Text = "0.00";
            txtVATAmount.Text = "0.00";
            txtTotalAmount.Text = "0.00";


            ddlTaxYear.SelectedValue = DateTime.Today.Year.ToString();
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
                    hdIssueTransNo.Value = "";
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
                    hdIssueTransNo.Value = "";
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
                hdRegNo.Value = oIssue.RegNo;
                hdIssueTransNo.Value = oIssue.IssueTransNo;
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

                txtLastCertDate.Text = "";
                txtNoofCertPrint.Text = "0";
                txtLastMaker.Text = "";
                txtDuplicate.Text = "";
                ddlCertificateType.SelectedValue = "0";
                ddlCertCustomerType.SelectedValue = "REGULAR";
                ddlWaiveCharges.SelectedIndex = 0;
                txtChargeAmount.Text = "0.00";
                hdChargeAmount.Value = txtChargeAmount.Text;
                txtVATAmount.Text = "0.00";
                txtTotalAmount.Text = (Convert.ToDouble(txtChargeAmount.Text) + Convert.ToDouble(txtVATAmount.Text)).ToString("N2");

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
                hdIssueTransNo.Value = "";
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

        protected void btnPrintCertificate_Click(object sender, EventArgs e)
        {
            ReportDAL rdal = new ReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];


            if (!string.IsNullOrEmpty(txtRegistrationNo.Text))
            {
                string sCertType = "";

                if (ddlCertificateType.SelectedIndex < 1)
                {

                        ddlCertificateType.SelectedIndex = 0;
                        ucMessage.OpenMessage("Enter a valid certicate type. Please check!", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        return;
                    
                }

                if (oConfig != null)
                {
                    if (ddlCertificateType.SelectedValue == "1")
                    {
                        sCertType = "Issuance";

                        oResult = rdal.InvestmentCertificate(txtRegistrationNo.Text, txtIssueName.Text, txtDuplicate.Text, oConfig.BranchID);
                        //Page.RegisterStartupScript(Constants.REPORT_WINDOW, "<script> alert('opended')</script>");
                    }
                    else if (ddlCertificateType.SelectedValue == "2")
                    {
                        sCertType = "Encashment";

                        //Encashment
                        oResult = rdal.EncashmentCertificate(txtRegistrationNo.Text, txtIssueName.Text, oConfig.BranchID, txtDuplicate.Text);

                    }
                    else if (ddlCertificateType.SelectedValue == "3")
                    {
                        sCertType = "Interest";

                        //Payment & TAX Certificate
                        oResult = rdal.PaymentCertificate(txtRegistrationNo.Text, txtIssueName.Text, oConfig.BranchID, ddlTaxYear.SelectedValue.ToString(), txtDuplicate.Text);

                    }
                    if (oResult.Status)
                    {
                        IssueDAL oIssueDAL = new IssueDAL();
                        oIssueDAL.Save_CertificatePrintLog(hdIssueTransNo.Value, sCertType,
                                   (Int32.Parse(txtNoofCertPrint.Text) + 1).ToString("N0"),ddlCertCustomerType.SelectedValue.ToString(), txtChargeAmount.Text,
                                            Convert.ToInt32(ddlWaiveCharges.SelectedValue), txtChargeAmount.Text,
                                                            txtVATAmount.Text, oConfig.UserName);
                        
                        ddlCertificateType_SelectedIndexChanged(sender, e);

                        Session[Constants.SES_RPT_DATA] = oResult.Return;
                        Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                    }
                    else
                    {
                        ucMessage.OpenMessage("No " + sCertType + " data found for certificate generation. Please check!", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    }

                }
            }
            else
            {
                ucMessage.OpenMessage("Enter a valid reg no. Please check!", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);

            }
            
        }

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

        protected void ddlCertificateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hdIssueTransNo.Value == "")
            {
                ddlCertificateType.SelectedIndex = 0;
                ucMessage.OpenMessage("Enter a valid reg no. Please check!", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                return;
            }
            string sCertType = "";
            if (ddlCertificateType.SelectedIndex > 0)
            {
                if (ddlCertificateType.SelectedValue == "1")
                {
                    sCertType = "Issuance";
                }
                else if (ddlCertificateType.SelectedValue == "2")
                {
                    sCertType = "Encashment";
                }
                else if (ddlCertificateType.SelectedValue == "3")
                {
                    sCertType = "Interest";
                }
                else
                {
                    ddlCertificateType.SelectedIndex = 0;
                    ucMessage.OpenMessage("Enter a valid certicate type. Please check!", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    return;
                }
            }

            IssueDAL oIssueDAL = new IssueDAL();
            Result oResult = oIssueDAL.Load_CertificatePrintLog(hdIssueTransNo.Value, sCertType,ddlCertCustomerType.SelectedValue.ToString());

            if (oResult.Status)
            {
                DataTable dtCertPrintLog = (DataTable)oResult.Return;
                if (dtCertPrintLog.Rows.Count > 0)
                {
                    if (dtCertPrintLog.Rows[0]["PrintDate"] != DBNull.Value)
                    {
                        txtLastCertDate.Text = Convert.ToDateTime(dtCertPrintLog.Rows[0]["PrintDate"]).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        txtLastCertDate.Text = "";
                    }

                    txtNoofCertPrint.Text = Convert.ToInt32(dtCertPrintLog.Rows[0]["CertIssueNo"]).ToString("N0");

                    if (Convert.ToInt32(dtCertPrintLog.Rows[0]["CertIssueNo"]) > 0)
                    {
                        txtDuplicate.Text = "DUPLICATE";
                    }
                    else 
                    {
                        txtDuplicate.Text = "";
                    }
                    txtLastMaker.Text = Convert.ToString(dtCertPrintLog.Rows[0]["MakerID"]);
                    ddlWaiveCharges.SelectedIndex  = 0;
                    
                    txtChargeAmount.Text = Convert.ToDouble(dtCertPrintLog.Rows[0]["ChargeAmount"]).ToString("N2");
                    hdChargeAmount.Value = txtChargeAmount.Text;
                    txtVATAmount.Text = (Convert.ToDouble(dtCertPrintLog.Rows[0]["ChargeAmount"]) * (Convert.ToDouble(dtCertPrintLog.Rows[0]["VATRate"])/100)).ToString("N2"); ;
                    txtTotalAmount.Text = (Convert.ToDouble(txtChargeAmount.Text) + Convert.ToDouble(txtVATAmount.Text)).ToString("N2");
                }
                else
                {
                    txtLastCertDate.Text = "";
                    txtNoofCertPrint.Text = "0";
                    txtLastMaker.Text = "";
                    txtDuplicate.Text = "";
                    ddlCertificateType.SelectedValue = "0";
                    ddlCertCustomerType.SelectedValue = "REGULAR";
                    ddlWaiveCharges.SelectedIndex = 0;
                    txtChargeAmount.Text = "0.00";
                    hdChargeAmount.Value = txtChargeAmount.Text;
                    txtVATAmount.Text = "0.00";
                    txtTotalAmount.Text = (Convert.ToDouble(txtChargeAmount.Text) + Convert.ToDouble(txtVATAmount.Text)).ToString("N2");
                }
            }
        }

        protected void ddlWaiveCharges_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCertificateType.SelectedIndex < 0)
            {
                ddlCertificateType.Focus();
                ucMessage.OpenMessage("Enter a valid certicate type. Please check!", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                return;
            }
            else
            {
                if (ddlWaiveCharges.SelectedIndex == 1)
                {
                    txtChargeAmount.Text = "0.00";
                    txtTotalAmount.Text = (Convert.ToDouble(txtChargeAmount.Text) + Convert.ToDouble(txtVATAmount.Text)).ToString("N2");
                }
                else if (ddlWaiveCharges.SelectedIndex == 2)
                {
                    txtChargeAmount.Text = "0.00";
                    txtVATAmount.Text = "0.00";
                    txtDuplicate.Text = "";
                    txtTotalAmount.Text = (Convert.ToDouble(txtChargeAmount.Text) + Convert.ToDouble(txtVATAmount.Text)).ToString("N2");
                }
                else
                {
                    ddlCertificateType_SelectedIndexChanged(sender, e);
                }
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtRegistrationNo.Text = "";
            InitializeData();
        }

        protected void ddlCertCustomerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlCertificateType_SelectedIndexChanged(sender, e);
        }
    }
}
