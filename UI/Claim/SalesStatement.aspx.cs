/*
 * File name            : SystemConfigSetup.cs
 * Author               : A.K.M. Tanjil Hossain,
 *                      : A.K.M. Zahidul Quaium,
 *                      : Tanvir Alam
 *                      : Jerin Afsana
 * Date                 : Sep 01,2014
 * Version              : 1.0
 *
 * Description          : System Config Setup Page
 *
 * Modification history :
 * Name                         Date                            Desc
 * Tanvir Alam                Sep 01,2014                Business implementation 
 * A.K.M. Zahidul Quaium        February 02,2012                Business implementation              
 * Jerin Afsana                 April    02,2012               Business implementation              
 * Copyright (c) 2012: Softcell Solution Ltd
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SBM_BLC1.Common;
using SBM.DM;
using SBM_BLC1.Transaction;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.DAL.Claim;
using SBM_BLC1.DAL.Report;
using System.Globalization;
using CrystalDecisions.CrystalReports.Engine;


namespace SBM_WebUI.mp
{
    public partial class SalesStatement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Constants.SES_USER_CONFIG] != null)
            {
                if (!Page.IsPostBack)
                {
                    InitializeData();
                    //This is for Page Permission
                    CheckPermission chkPer = new CheckPermission();
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CLAIM.SALES_STATEMENT))
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
            gvData.DataSource = null;
            gvData.DataBind();
            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            DDListUtil.LoadDDLFromXML(ddlDuration, "DateDuration", "Type", "Duration", false);
            ddlDuration.Enabled = false;
            txtStatementDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtDateFrom.ReadOnly = true;
            txtTotalFaceValue.Text = "0.00";
            // year
            for (int i = 1995; i < DateTime.Now.Year +1 +1; i++)
            {
                DDListUtil.Add(ddlYear, i.ToString(), i.ToString());
            }
            DDListUtil.Assign(ddlYear, DateTime.Now.Year);
        }

        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");

            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE).PadLeft(5, '0'), false);
            }
            else
            {
                // no action
            }
        }
        #endregion InitializeData        

        #region Event

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            FillRegistrationDetailGrid("");
        }

        protected void ddlSpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSPTypeData();            
        }

        protected void ddlDuration_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDurationData();
        }

        protected void btnSelectAll_Click(object sender, EventArgs e)
        {
            SelectDeselectAllCheck(true);
            CalculateData();
        }

        protected void btnDeselectAll_Click(object sender, EventArgs e)
        {
            SelectDeselectAllCheck(false);
            CalculateData();
        }

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            CalculateData();
        }

        public void ClaimSearchLoadAction(DataRow dtRow)
        {
            if (dtRow != null)
            {                
                string sSalesClaimTransNo = Convert.ToString(dtRow["SaleStatementTransNo"]);
                hdnClaimTransNo.Value = sSalesClaimTransNo;
                txtReferenceNo.Text = Convert.ToString(dtRow["Reference No"]);
                DateTime parsedDate;
                DateTime.TryParseExact(Convert.ToString(dtRow["Statement Date"]), Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                txtStatementDate.Text = parsedDate.ToString(Constants.DATETIME_FORMAT);
                DateTime.TryParseExact(Convert.ToString(dtRow["From Date"]), Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                txtDateFrom.Text = parsedDate.ToString(Constants.DATETIME_FORMAT);
                ddlYear.Text = Util.GetDateTimeByString(txtStatementDate.Text).Year.ToString();
                ddlSpType.Text = Convert.ToString(dtRow["SP Type"]);
                DDListUtil.Assign(ddlDuration,Convert.ToString(dtRow["DurationType"]));

                DateTime.TryParseExact(Convert.ToString(dtRow["To Date"]), Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                string sToDate = parsedDate.ToString(Constants.DATETIME_dd_MMM_yyyy);
                
                DataTable dtTempTodate = new DataTable();                
                dtTempTodate.Columns.Add(new DataColumn("ValueMember", typeof(string)));
                dtTempTodate.Columns.Add(new DataColumn("DisplayMember", typeof(string)));
                                
                DataRow drTempDataRow = dtTempTodate.NewRow();
                drTempDataRow[0] = 0;
                drTempDataRow[1] = "";
                dtTempTodate.Rows.Add(drTempDataRow);

                drTempDataRow = dtTempTodate.NewRow();
                drTempDataRow[0] = sToDate;
                drTempDataRow[1] = sToDate;
                dtTempTodate.Rows.Add(drTempDataRow);

                dtTempTodate.AcceptChanges();
                ddlDateTo.DataSource = dtTempTodate;
                ddlDateTo.DataTextField = "DisplayMember";
                ddlDateTo.DataValueField = "ValueMember";
                ddlDateTo.DataBind();
                ddlDateTo.Text = sToDate;

                FillRegistrationDetailGrid(sSalesClaimTransNo);
                EnableDisableControls(false);                
            }
        }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            EnableDisableControls(true);
            foreach (ListItem item in chkListForBB.Items)
            {
                item.Enabled = true;
                item.Selected = false;
            }
            txtTotalFaceValue.Text = "0.00";
            if (ddlDuration.Items.Count > 0)
            {
                ddlDuration.SelectedIndex = 0;
            } 
            if (ddlSpType.Items.Count > 0)
            {
                ddlSpType.SelectedIndex = 0;
            }
            txtReferenceNo.Text = "";
            DDListUtil.Assign(ddlYear, DateTime.Now.Year);
            txtStatementDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtDateFrom.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            ddlDateTo.Items.Clear();

            gvData.DataSource = null;
            gvData.DataBind();
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {            
            CalculateData();
            SBM_BLC1.Entity.Claim.SalesStatement oSalesStatement = GetObject();
            PreviewAction(oSalesStatement);            
        }
        protected void btnSaveAndPreview_Click(object sender, EventArgs e)
        {                        
            if (SaveAndPreviewAction())
            {
                LoadSPTypeData();
            }
        }
        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, null);
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataTable dt = (DataTable)gvData.DataSource;
                if (dt != null)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (dt.Rows[0][i].GetType() == typeof(Decimal))
                        {
                            try
                            {
                                e.Row.Cells[i + 1].Text = Math.Round(Convert.ToDecimal(e.Row.Cells[i + 1].Text)).ToString();
                            }
                            catch (Exception Exp)
                            {

                            }
                        }
                    }
                }
            }

        }
        #endregion Event

        #region Utility
        private void LoadShowData(string strSaleStatementTransNo)
        {
            // this block is used for Reset the all controls.. 
            gvData.DataSource = null;
            gvData.DataBind();
            ClearTotal();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            DataTable dtDataTable = new DataTable();
            SalesStatementDAL salesDAL = new SalesStatementDAL();
            Result oResult = new Result();
            try
            {
                DataColumn dtColIsSelect = new DataColumn();
                dtColIsSelect.AllowDBNull = false;
                dtColIsSelect.DefaultValue = false;
                dtColIsSelect.ReadOnly = false;
                dtColIsSelect.DataType = System.Type.GetType("System.Boolean");
                dtColIsSelect.Caption = "Select";
                dtColIsSelect.ColumnName = "IsSelect";

                DateTime dtFromDate = Util.GetDateTimeByString(txtDateFrom.Text);
                DateTime dtToDate = Util.GetDateTimeByString(ddlDateTo.SelectedValue);
                oResult = (Result)salesDAL.LoadRegistrationDetailsData(ddlSpType.SelectedValue, dtFromDate, dtToDate, strSaleStatementTransNo, oConfig.DivisionID, oConfig.BankCodeID);
                if (oResult.Status)
                {
                    dtDataTable = (DataTable)oResult.Return;
                    dtDataTable.Columns.Add(dtColIsSelect);

                    if (dtDataTable.Rows.Count > 0)
                    {
                        gvData.DataSource = dtDataTable;
                        gvData.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }

        public void SetToDate(DateTime dtFrom)
        {
            Result oResult = new Result();
            DataTable dtTemp = new DataTable();
            SalesStatementDAL SSDal = new SalesStatementDAL();

            string strWhereCondition = "";
            try
            {
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                strWhereCondition = " WHERE(((SPMS_SalesStatementDetails.IssueTransNo) Is Null) And ((SPMS_Reinvestment.OldIssueTransNo) Is Null) " +
                                    " And ((SPMS_OldCustomerIssue.IssueTransNo) Is Null) And ((SPMS_Issue.SPTypeID) = '" + ddlSpType.SelectedValue + "')" + 
                                    " And SPMS_Issue.IsApproved = " + (int)SBM_BLV1.baseCommon.enmApprovalStatus.Approved + 
                                    " And SPMS_Issue.IsClaimed = 'False' " + 
                                    " And SPMS_Issue.DivisionID = '" + oConfig.DivisionID + "' And SPMS_Issue.BankID = '" + oConfig.BankCodeID + "')";
                oResult = (Result)SSDal.GetSaleStatementToDateSource(strWhereCondition);
                if (oResult.Status)
                {
                    dtTemp = (DataTable)oResult.Return;
                    ddlDateTo.DataSource = dtTemp;
                    ddlDateTo.DataTextField = "DisplayMember";
                    ddlDateTo.DataValueField = "ValueMember";
                    ddlDateTo.DataBind();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void LoadDurationData()
        {
            gvData.DataSource = null;
            gvData.DataBind();
            txtTotalFaceValue.Text = "0.00";

            if (ddlDuration.SelectedIndex > 0)
            {
                CommissionClaimDAL commClaimDAL = new CommissionClaimDAL();
                Result oResult = null;
                if (ddlDuration.SelectedValue.Equals(Convert.ToString((int)SBM_BLV1.baseCommon.enmDurationType.Daily)))
                {
                    //ddlDateTo.Text = Util.GetDateTimeByString(txtDateFrom.Text).ToString(Constants.DATETIME_dd_MMM_yyyy);//for the time being                    
                    oResult = commClaimDAL.GetCommissionClaimToDateSource("", txtDateFrom.Text);
                    if (oResult.Status)
                    {
                        ddlDateTo.DataSource = null;

                        DataTable dtDateTo = (DataTable)oResult.Return;
                        ddlDateTo.DataSource = dtDateTo;
                        ddlDateTo.DataTextField = "DisplayMember";
                        ddlDateTo.DataValueField = "ValueMember";
                        ddlDateTo.DataBind();
                        if (dtDateTo.Rows.Count == 2)
                        {
                            ddlDateTo.Text = dtDateTo.Rows[1]["ValueMember"].ToString();
                        }
                    }
                    ddlDateTo.Enabled = false;
                    FillRegistrationDetailGrid("");
                }
                else if (ddlDuration.SelectedValue.Equals(Convert.ToString((int)SBM_BLV1.baseCommon.enmDurationType.Date_Range)))
                {                                                            
                    SetToDate(Util.GetDateTimeByString(txtDateFrom.Text));
                    ddlDateTo.Enabled = true;
                    if (ddlDateTo.Items.Count > 0)
                    {
                        ddlDateTo.SelectedIndex = 0;
                    }
                }
                else if (ddlDuration.SelectedValue.Equals(Convert.ToString((int)SBM_BLV1.baseCommon.enmDurationType.Half_Yearly)))
                {
                    DateTime dtFromDate = DateTime.Now;
                    DateTime dtToDate = DateTime.Now;

                    if (!string.IsNullOrEmpty(txtDateFrom.Text))
                    {
                        dtFromDate = Util.GetDateTimeByString(txtDateFrom.Text);
                        if (dtFromDate.Month > 1 && dtFromDate.Month <= 6)
                        {
                            dtFromDate = new DateTime(dtFromDate.Year, 1, 1);
                            dtToDate = new DateTime(dtFromDate.Year, 6, 30);
                        }
                        else if (dtFromDate.Month >= 7)
                        {
                            dtFromDate = new DateTime(dtFromDate.Year, 7, 1);
                            dtToDate = new DateTime(dtFromDate.Year, 12, 31);
                        }
                        txtDateFrom.Text = dtFromDate.ToString(Constants.DATETIME_FORMAT);
                        ddlDateTo.DataSource = null;
                        oResult = commClaimDAL.GetCommissionClaimToDateSource("", dtToDate.ToString(Constants.DATETIME_FORMAT));
                        if (oResult.Status)
                        {
                            DataTable dtDateTo = (DataTable)oResult.Return;
                            ddlDateTo.DataSource = dtDateTo;
                            ddlDateTo.DataTextField = "DisplayMember";
                            ddlDateTo.DataValueField = "ValueMember";
                            ddlDateTo.DataBind();
                            if (dtDateTo.Rows.Count == 2)
                            {
                                ddlDateTo.Text = dtDateTo.Rows[1]["ValueMember"].ToString();
                            }
                            ddlDateTo.Enabled = false;
                        }

                        FillRegistrationDetailGrid("");
                    }                    
                }                
                else if (ddlDuration.SelectedValue.Equals(Convert.ToString((int)SBM_BLV1.baseCommon.enmDurationType.Monthly)))
                {
                    DateTime dtFromDate = DateTime.Now;
                    DateTime dtToDate = DateTime.Now;

                    if (!string.IsNullOrEmpty(txtDateFrom.Text))
                    {
                        dtFromDate = Util.GetDateTimeByString(txtDateFrom.Text);
                        dtFromDate = new DateTime(dtFromDate.Year, dtFromDate.Month, 1);
                        int iNoOfDays = System.DateTime.DaysInMonth(dtFromDate.Year, dtFromDate.Month);
                        dtToDate = dtFromDate.AddDays(iNoOfDays - 1);                        

                        txtDateFrom.Text = dtFromDate.ToString(Constants.DATETIME_FORMAT);
                        ddlDateTo.DataSource = null;
                        oResult = commClaimDAL.GetCommissionClaimToDateSource("", dtToDate.ToString(Constants.DATETIME_FORMAT));
                        if (oResult.Status)
                        {
                            DataTable dtDateTo = (DataTable)oResult.Return;
                            ddlDateTo.DataSource = dtDateTo;
                            ddlDateTo.DataTextField = "DisplayMember";
                            ddlDateTo.DataValueField = "ValueMember";
                            ddlDateTo.DataBind();
                            if (dtDateTo.Rows.Count == 2)
                            {
                                ddlDateTo.Text = dtDateTo.Rows[1]["ValueMember"].ToString();
                            }
                            ddlDateTo.Enabled = false;
                        }

                        FillRegistrationDetailGrid("");
                    }                    
                }                               
            }
            else
            {                
                ddlDateTo.SelectedIndex = -1;
                ddlDateTo.Enabled = true;
            }            
        }

        private void FillRegistrationDetailGrid(string sSaleStatementTransNo)
        {
            if (!string.IsNullOrEmpty(ddlSpType.SelectedValue))
            {
                if (ddlSpType.SelectedValue.Equals("3MS") ||
                    ddlSpType.SelectedValue.Equals("BSP") ||
                    ddlSpType.SelectedValue.Equals("DIB") ||
                    ddlSpType.SelectedValue.Equals("DPB") ||
                    ddlSpType.SelectedValue.Equals("FSP") ||
                    ddlSpType.SelectedValue.Equals("PSC") ||
                    ddlSpType.SelectedValue.Equals("WDB")
                  )
                {
                    SalesStatementDAL ssDal = new SalesStatementDAL();
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    Result oResult = ssDal.LoadRegistrationDetailsData(ddlSpType.SelectedValue, Util.GetDateTimeByString(txtDateFrom.Text), Util.GetDateTimeByString(ddlDateTo.SelectedValue), sSaleStatementTransNo, oConfig.DivisionID, oConfig.BankCodeID);                    
                    DataTable dtSaleStatement = oResult.Return as DataTable;

                    if (dtSaleStatement != null && dtSaleStatement.Rows.Count > 0)
                    {                        
                        dtSaleStatement.Columns.Remove("PolicyID");
                        
                        gvData.DataSource = dtSaleStatement;
                        gvData.DataBind();

                        if (!string.IsNullOrEmpty(sSaleStatementTransNo))
                        {
                            foreach (GridViewRow gvr in gvData.Rows)
                            {
                                CheckBox chk = (CheckBox)gvr.FindControl("chkSelected");
                                if (chk != null)
                                {
                                    chk.Checked = true;
                                    chk.Enabled = false;
                                }
                            }

                            CalculateData();
                        }                        
                    }
                
                    ddlDuration.Enabled = true;
                    ddlDateTo.Enabled = true;
                }
                else
                {
                    ddlDuration.Enabled = false;
                    ddlDateTo.Enabled = false;
                }
            }
            else
            {
                ddlDuration.Enabled = false;
                ddlDateTo.Enabled = false;
            }                        
        }

        private void LoadSPTypeData()
        {
            Result oResult = new Result();

            if (!string.IsNullOrEmpty(ddlSpType.SelectedValue))
            {
                try
                {                    
                    ClaimDAL cDAL = new ClaimDAL();
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    oResult = (Result)cDAL.GetSalesStatementFromDate(ddlSpType.SelectedValue, oConfig.DivisionID, oConfig.BankCodeID);
                    if (oResult.Status)
                    {                        
                        txtDateFrom.Text = oResult.Return.ToString();
                        SetToDate(Util.GetDateTimeByString(txtDateFrom.Text));
                        ddlDuration.Enabled = true;
                        ddlDuration.SelectedIndex = 0;                        
                    }
                    else
                    {
                        ddlDuration.SelectedIndex = 0;
                        ddlDateTo.Items.Clear();                        
                    }
                }
                catch (Exception exp)
                {

                }
            }
            else
            {
                txtDateFrom.Text = string.Empty;
                ddlDuration.SelectedIndex = 0;
                ddlDateTo.Items.Clear();
                //Me.ControlOperation(False)
            }

            gvData.DataSource = null;
            gvData.DataBind();
            txtTotalFaceValue.Text = string.Empty;
        }

        private void SelectDeselectAllCheck(bool status)
        {
            try
            {
                foreach (GridViewRow gvr in gvData.Rows)
                {
                    CheckBox chk = (CheckBox)gvr.FindControl("chkSelected");
                    if (chk != null)
                    {
                        chk.Checked = status;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ClearTotal()
        { 

        }
        private bool SaveAndPreviewAction()
        {                                                
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            
            if (oConfig != null)
            {
                if (txtReferenceNo.Text.Equals(string.Empty))
                {
                    DataTable dtRegDetail = GetRegistrationDetail();

                    if (dtRegDetail.Rows.Count > 0)
                    {
                        CalculateData();

                        #region     01.  GetObject    ... Get Value
                        SBM_BLC1.Entity.Claim.SalesStatement oSalesStatement = GetObject();
                        #endregion

                        #region     02. Call and varify..
                        SalesStatementDAL salStatementDAL = new SalesStatementDAL();
                        Result oResult = salStatementDAL.InsertData(oSalesStatement, dtRegDetail);
                        if (!oResult.Status)
                        {
                            ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
                            ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                            return false;
                        }
                        #endregion

                        #region     03. for preview
                        oSalesStatement.SaleStatementReferenceNo = Convert.ToString(oResult.Return);
                        PreviewAction(oSalesStatement);
                        #endregion
                    }
                    else
                    {
                        ucMessage.OpenMessage("Select at least one registration to save & print sales statement.", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        private void PreviewAction(SBM_BLC1.Entity.Claim.SalesStatement oSalesStatement)
        {
            DataTable dtRegistrationDetail = GetRegistrationDetail();
            if (dtRegistrationDetail.Rows.Count > 0)
            {
                string sIssueTransNo = string.Empty;
                for (int rowIndx = 0; rowIndx < dtRegistrationDetail.Rows.Count; rowIndx++)
                {
                    sIssueTransNo += ",'" + Convert.ToString(dtRegistrationDetail.Rows[rowIndx]["IssueTransNo"]) + "'";
                }
                sIssueTransNo = sIssueTransNo.Substring(1);

                if (!string.IsNullOrEmpty(ddlSpType.SelectedItem.Text))
                {
                    string[] TypeDesc = ddlSpType.SelectedItem.Text.Split(':');
                    oSalesStatement.SPType.TypeDesc = TypeDesc[1];
                }
                CreateReportDocument(oSalesStatement);                
                //GetMasterCertificateDetailData(strIssueTransNo, cStrSaleStatementTransNo)
            }
            else
            {
                ucMessage.OpenMessage("Select at least one registration to PREVIEW sales statement.", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }
        public void CreateReportDocument(SBM_BLC1.Entity.Claim.SalesStatement oSalesStatement)
        {
            ClaimReportDAL oCRDAL = new ClaimReportDAL();
            bool isCoverLetter = false;
            foreach (ListItem item in chkListForBB.Items)
            {
                if (item.Selected)
                {
                    if (item.Value.Equals("With Cover Letter"))
                    {
                        isCoverLetter = true;
                    }
                    else
                    {

                    }
                }
            }

            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (oConfig != null)
            {                
                int iReportFormat = 0;
                Result oResult = new Result();
                Result oResult1 = new Result();
                Result oResult2 = new Result();
                SalesStatementDAL sDAL = new SalesStatementDAL();
                Result oResultFormat = sDAL.GetSaleStatementReportFormat(ddlSpType.SelectedValue);
                if (oResultFormat.Status)
                {
                    iReportFormat = (int)oResultFormat.Return;
                }
                if (iReportFormat.Equals((int)SBM_BLV1.baseCommon.enmSalesReportFormat.SP))
                {
                    oResult1 = oCRDAL.SalesStatementReportDocument(oSalesStatement, GetRegistrationDetail(), (int)Constants.ReportType.BSPSalesDetails, oConfig.BranchID);
                    if (isCoverLetter)
                    {
                        oResult = oCRDAL.SalesStatementReportDocument(oSalesStatement, GetRegistrationDetail(), (int)Constants.ReportType.BSPSalesCoverLetter, oConfig.BranchID);
                    }   
                }
                else if (iReportFormat.Equals((int)SBM_BLV1.baseCommon.enmSalesReportFormat.DollarBond))
                {
                    oResult2 = oCRDAL.SalesStatementReportDocument(oSalesStatement, GetRegistrationDetail(), (int)Constants.ReportType.DPBSalesDetails2, oConfig.BranchID);
                    if (isCoverLetter)
                    {
                        oResult = oCRDAL.SalesStatementReportDocument(oSalesStatement, GetRegistrationDetail(), (int)Constants.ReportType.DPBSalesCoverLetter, oConfig.BranchID);
                    }
                }
                else if (iReportFormat.Equals((int)SBM_BLV1.baseCommon.enmSalesReportFormat.WDB))
                {
                    oResult1 = oCRDAL.SalesStatementReportDocument(oSalesStatement, GetRegistrationDetail(), (int)Constants.ReportType.WEDBSalesDetais, oConfig.BranchID);
                    if (isCoverLetter)
                    {
                        oResult = oCRDAL.SalesStatementReportDocument(oSalesStatement, GetRegistrationDetail(), (int)Constants.ReportType.WEDBSalesCoverLetter, oConfig.BranchID);
                    }
                }
                Session["ExportType"] = ddlExportType.SelectedValue;
                if (oResult.Status) //cover letter
                {
                    Session[Constants.SES_RPT_DATA] = oResult.Return as ReportDocument;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport2(1));
                }
                if (oResult1.Status)
                {
                    Session[Constants.SES_RPT_DATA_2] = oResult1.Return as ReportDocument;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW_2, Util.OpenReport2(2));
                    //Session[Constants.SES_RPT_DATA] = oResult1.Return as ReportDocument;
                    //Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                }
                if (oResult2.Status)
                {
                    Session[Constants.SES_RPT_DATA_2] = oResult2.Return as ReportDocument;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW_2, Util.OpenReport2(2));
                }
            }
        }
        private void CalculateData()
        {
            decimal dTolFaceValue = 0;
            
            foreach (GridViewRow gvr in gvData.Rows)
            {
                CheckBox chk = (CheckBox)gvr.FindControl("chkSelected");
                if (chk != null)
                {
                    if (chk.Checked)
                    {
                        dTolFaceValue += Util.GetDecimalNumber(gvr.Cells[5].Text);                        
                    }
                }
            }

            #region Assign Data in calculation field set

            txtTotalFaceValue.Text = dTolFaceValue.ToString("N2");
            
            #endregion
        }

        private SBM_BLC1.Entity.Claim.SalesStatement GetObject()
        {
            SBM_BLC1.Entity.Claim.SalesStatement oSalesStatement = new SBM_BLC1.Entity.Claim.SalesStatement();
            if (!string.IsNullOrEmpty(ddlDuration.SelectedValue))
            {
                oSalesStatement.DurationType = Util.GetIntNumber(ddlDuration.SelectedValue);
            }
            oSalesStatement.FromDate = Util.GetDateTimeByString(txtDateFrom.Text);
            oSalesStatement.SaleStatementReferenceNo = txtReferenceNo.Text;
            oSalesStatement.SPType.SPTypeID = ddlSpType.SelectedValue;
            oSalesStatement.StatementDate = Util.GetDateTimeByString(txtStatementDate.Text);
            oSalesStatement.ToDate = Util.GetDateTimeByString(ddlDateTo.SelectedValue);
            oSalesStatement.TotalFaceValue = Util.GetDoubleNumber(txtTotalFaceValue.Text);

            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            oSalesStatement.UserDetails.MakerID = oConfig.UserName;
            oSalesStatement.UserDetails.Division = oConfig.DivisionID;
            oSalesStatement.UserDetails.BankID = oConfig.BankCodeID;

            return oSalesStatement;
        }

        private DataTable GetRegistrationDetail()
        {
            DataTable dtRegDetails = new DataTable("dtRegDetails");

            dtRegDetails.Columns.Add(new DataColumn("RegNo", typeof(string)));
            dtRegDetails.Columns.Add(new DataColumn("IssueTransNo", typeof(string)));
            dtRegDetails.Columns.Add(new DataColumn("IssueAmount", typeof(double)));
            dtRegDetails.Columns.Add(new DataColumn("OrgAmount", typeof(double)));
            dtRegDetails.Columns.Add(new DataColumn("NonOrgAmount", typeof(double)));

            DataRow rowRegDetail = null;
            foreach (GridViewRow gvr in gvData.Rows)
            {
                CheckBox chk = (CheckBox)gvr.FindControl("chkSelected");
                if (chk != null)
                {
                    if (chk.Checked)
                    {
                        rowRegDetail = dtRegDetails.NewRow();
                        
                        rowRegDetail["RegNo"] = gvr.Cells[1].Text;
                        rowRegDetail["IssueTransNo"] = (gvr.FindControl("hdnIssueTransNo") as HiddenField).Value;
                        rowRegDetail["IssueAmount"] = gvr.Cells[5].Text;
                        rowRegDetail["OrgAmount"] = (gvr.FindControl("hdnOrgAmount") as HiddenField).Value;
                        rowRegDetail["NonOrgAmount"] = (gvr.FindControl("hdnNonOrgAmount") as HiddenField).Value;

                        dtRegDetails.Rows.Add(rowRegDetail);
                    }
                }
            }

            return dtRegDetails;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Result oResult = null;
            try
            {

                if (string.IsNullOrEmpty(hdnClaimTransNo.Value))
                {
                    ucMessage.OpenMessage("Please select a valid Sale Claim for delete.", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
                else
                {
                    SalesStatementDAL iClaimDAL = new SalesStatementDAL();
                    oResult = iClaimDAL.DeleteSaleClaim(hdnClaimTransNo.Value);

                    if (oResult.Status)
                    {
                        ucMessage.OpenMessage("Sale Claim data deleted successfully.", Constants.MSG_APPROVED_SAVE_DATA);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        btnReset_Click(sender, e);
                    }
                    else
                    {
                        ucMessage.OpenMessage("Error: " + oResult.Message, Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    }
                }
            }
            catch (Exception ex)
            {
                ucMessage.OpenMessage("Error: " + ex.Message, Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }
  
        private void EnableDisableControls(bool isEnabled)
        {
            if (isEnabled)
            {
                ddlSpType.Enabled = true;
                txtReferenceNo.ReadOnly = false;
                ddlYear.Enabled = true;
                if (chkListForBB.Items.Count > 1)
                {
                    chkListForBB.Items[0].Enabled = true;
                    chkListForBB.Items[1].Enabled = true;
                }                
                
                ddlDuration.Enabled = true;
                ddlDateTo.Enabled = false;
                btnCalculate.Enabled = true;
                btnSelectAll.Enabled = true;
                btnDeselectAll.Enabled = true;
                btnSaveAndPreview.Enabled = true;
                btnLoad.Visible = true;
            }
            else
            {
                ddlSpType.Enabled = false;
                txtReferenceNo.ReadOnly = true;
                ddlYear.Enabled = false;                
                ddlDuration.Enabled = false;
                ddlDateTo.Enabled = false;
                btnCalculate.Enabled = false;
                btnSelectAll.Enabled = false;
                btnDeselectAll.Enabled = false;
                btnSaveAndPreview.Enabled = false;
                btnLoad.Visible = false;
            }
        }

        #endregion Utility
    }
}
