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
 * Jerin Afsana                 April    02,2012                Business implementation              
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

namespace SBM_WebUI.mp
{
    public partial class CommissionClaim : System.Web.UI.Page
    {
        public string _BASE_CUREENCY = "baseCurrency";
        public string _SPTYPE_CUREENCY = "spTypeCurrency";

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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CLAIM.COMMISSION))
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
            // year
            for (int i = 1995; i < DateTime.Now.Year +1 +1; i++)
            {
                DDListUtil.Add(ddlYear, i.ToString(), i.ToString());
            }
            txtDateFrom.ReadOnly = true;
            DDListUtil.Assign(ddlYear, DateTime.Now.Year);

            ClaimDAL claimDAL = new ClaimDAL();
            //Store BASE_CURRENCY
            ViewState[_BASE_CUREENCY] = claimDAL.GetBaseCurrencyID();
        }
        #endregion InitializeData

        public void ClaimSearchLoadAction(DataRow dtRow)
        {
            if (dtRow != null)
            {               
                string sCommClaimTransNo = Convert.ToString(dtRow["CommissionClaimTransNo"]);
                hdnClaimTransNo.Value = sCommClaimTransNo;
                txtReferenceNo.Text = Convert.ToString(dtRow["Reference No"]);
                DateTime parsedDate;
                DateTime.TryParseExact(Convert.ToString(dtRow["Date"]), Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                txtStatementDate.Text = parsedDate.ToString(Constants.DATETIME_FORMAT);                
                DateTime.TryParseExact(Convert.ToString(dtRow["FromDate"]), Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                txtDateFrom.Text = parsedDate.ToString(Constants.DATETIME_FORMAT);
                ddlYear.Text = Util.GetDateTimeByString(txtStatementDate.Text).Year.ToString();
                ddlSpType.Text = Convert.ToString(dtRow["SP Type"]);
                ddlDuration.Text = Convert.ToString(dtRow["DurationType"]);
                string sSPCurrency = ViewState[_SPTYPE_CUREENCY] as string;
                ClaimDAL claimDAL = null;
                if (string.IsNullOrEmpty(sSPCurrency))
                {
                    claimDAL = new ClaimDAL();
                    ViewState[_SPTYPE_CUREENCY] = claimDAL.GetSPCurrency(ddlSpType.SelectedValue);
                }
                txtConversionRate.Text = Convert.ToString(dtRow["ConvRate"]);
                DateTime.TryParseExact(Convert.ToString(dtRow["ToDate"]), Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                string sToDate = parsedDate.ToString(Constants.DATETIME_dd_MMM_yyyy);
                
                DataTable dtTempTodate = new DataTable();
                dtTempTodate.Columns.Add(new DataColumn("ValueMember", typeof(string)));
                dtTempTodate.Columns.Add(new DataColumn("DisplayMember", typeof(string)));

                DataRow drTempDataRow = dtTempTodate.NewRow();
                drTempDataRow[0] = 0;
                drTempDataRow[1] = string.Empty;
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

                BindCurrency(claimDAL);
                string sCurrencyID = Convert.ToString(dtRow["CurrencyID"]);                
                ddlConversionCurrency.Text = sCurrencyID;

                FillReferenceDetailGrid(sCommClaimTransNo);
                EnableDisableControls(false);
            }
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
       
        #region Event
        protected void ddlSpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearControlValues(false);
            LoadSPTypeData();
        }

        protected void ddlDuration_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDurationData();
        }

        protected void ddlDateTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadToDateData();
        }
        
        protected void btnSelectAll_Click(object sender, EventArgs e)
        {
            LoadCurrencyData();
            SelectDeselectAllCheck(true);
            CalculateCommissionClaim();
        }

        protected void btnDeselectAll_Click(object sender, EventArgs e)
        {
            SelectDeselectAllCheck(false);
            CalculateCommissionClaim();
        }

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            CalculateCommissionClaim();
        }

        protected void ddlConversionCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCurrencyData();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            EnableDisableControls(true);
            ClearControlValues(true);
        }       

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            CalculateCommissionClaim();
            SBM_BLC1.Entity.Claim.CommissionClaim oCommClaim = GetObject();
            Preview(oCommClaim);
        }
        
        protected void btnSaveAndPreview_Click(object sender, EventArgs e)
        {
            if (SaveAndPreviewAction())
            {
                ClearControlValues(false);
                LoadSPTypeData();
            }
        }               
        
        #endregion Event

        #region Utility Methods
        private void LoadSPTypeData()
        {
            //Clear Grid value
            gvData.DataSource = null;
            gvData.DataBind();

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
                    ClaimDAL cDal = new ClaimDAL();
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    Result oResult = (Result)cDal.GetCommissionFromDate(ddlSpType.SelectedValue, oConfig.DivisionID, oConfig.BankCodeID);
                    if (oResult.Status)
                    {
                        string sDate = (string)oResult.Return;
                        if (sDate != null)
                        {
                            txtDateFrom.Text = sDate;
                        }
                    }

                    if (txtDateFrom.Text != string.Empty)
                    {
                        SetToDate();

                        ddlDateTo.Enabled = true;
                        ddlDuration.Enabled = true;

                        ddlDuration.Text = Convert.ToString((int)SBM_BLV1.baseCommon.enmDurationType.Date_Range);
                        
                        BindCurrency(cDal);
                        ddlConversionCurrency.Enabled = true;

                        ViewState[_SPTYPE_CUREENCY] = cDal.GetSPCurrency(ddlSpType.SelectedValue);
                    }
                    /*If Me.txtFromDate.Text.Trim <> "" Then
                    Me.SetToDate()     'Fill ToDate Combo 
                    Me.cboDuration.SelectedValue = CStr(modCommon.enmDurationType.Date_Range)
                    Me.ControlOperation(True)
                    strCurrecncyCriteria = " INNER JOIN ([SELECT top 1  PolicyID,max(PolicyEffectDate) from  SPMS_SPPolicy WHERE SPTypeID='" & Me.cboSPType.SelectedValue & "' group by PolicyID order by max(PolicyEffectDate) DESC ]. AS b INNER JOIN SPMS_SPCurrencyPolicy ON b.PolicyID = SPMS_SPCurrencyPolicy.PolicyID) ON a.CurrencyID = SPMS_SPCurrencyPolicy.CurrencyID"
                    strCurrecncyCriteria &= " WHERE (((SPMS_SPCurrencyPolicy.ActivityType)=" & modCommon.enmActivityType.Commission_Claim & "))"
                    Me.cboCurrencyID.DataSource = cOCommon.GetCurrencySource(strCurrecncyCriteria)
                    */
                    //FillReferenceDetailGrid("");                    

                    //ddlDuration.Enabled = true;

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

        private void LoadDurationData()
        {            
            if (ddlSpType.SelectedIndex > 0)
            {
                //to do...
                string sSPCurrency = ViewState[_SPTYPE_CUREENCY] as string;
                DDListUtil.Assign(ddlConversionCurrency, sSPCurrency);
            }
            SetToDate();

            if (ddlDuration.SelectedIndex > 0)
            {
                CommissionClaimDAL commClaimDAL = new CommissionClaimDAL();
                Result oResult = null;
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                string sCriteria = string.Empty;

                if (ddlDuration.SelectedValue.Equals(Convert.ToString((int)SBM_BLV1.baseCommon.enmDurationType.Daily)))
                {                    
                    oResult = commClaimDAL.GetCommissionClaimToDateSource(sCriteria, txtDateFrom.Text);

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
                        ddlDateTo.Enabled = false;
                    }
                }
                else if (ddlDuration.SelectedValue.Equals(Convert.ToString((int)SBM_BLV1.baseCommon.enmDurationType.Date_Range)))
                {
                    /*                
                    If Me.chkFource.Checked Then
                        cboToDate.DataSource = Nothing
                        cboToDate.DataSource = Me.cOCommon.GetCommissionClaimToDateSource(Nothing, Format(CDate(txtFromDate.Text.Trim), modCommon.DateFormat))
                    End If
                     */
                    gvData.DataSource = null;
                    gvData.DataBind();
                    txtTotalFaceValue.Text = "0.00";
                    txtTotalNonOrgCommission1.Text = "0.00";
                    txtTotalNonOrgCommission2.Text = "0.00";
                    txtTotalOrgCommission1.Text = "0.00";
                    txtTotalOrgCommission2.Text = "0.00";
                    txtTotalCommissionClaim.Text = "0.00";
                    if (ddlConversionCurrency.Items.Count > 0)
                    {
                        ddlConversionCurrency.SelectedIndex = 0;
                    }
                    ddlConversionCurrency.Enabled = false;
                    txtConversionRate.Text = "0.0000";
                    ddlDateTo.SelectedIndex = 0;
                    ddlDateTo.Enabled = true;
                }
                else if (ddlDuration.SelectedValue.Equals(Convert.ToString((int)SBM_BLV1.baseCommon.enmDurationType.Half_Yearly)))
                {
                    DateTime dtFromDate = DateTime.Now;
                    DateTime dtToDate = DateTime.Now;

                    if (!string.IsNullOrEmpty(txtDateFrom.Text))
                    {
                        dtFromDate = Util.GetDateTimeByString(txtDateFrom.Text);
                        if (dtFromDate.Month >= 1 && dtFromDate.Month <= 6)
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
                        oResult = commClaimDAL.GetCommissionClaimToDateSource(sCriteria, dtToDate.ToString(Constants.DATETIME_FORMAT));
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
                    }
                }
                /*
                  ElseIf cboDuration.SelectedValue = CStr(modCommon.enmDurationType.Monthly) Then
                    Dim dtFromDate As Date
                    Dim dtToDate As Date
                    If Me.txtFromDate.Text <> "" Then
                        dtFromDate = CDate(Me.txtFromDate.Text)
                        dtFromDate = New Date(dtFromDate.Year, dtFromDate.Month, 1)
                        dtToDate = DateAdd(DateInterval.Month, 1, dtFromDate)
                        dtToDate = DateAdd(DateInterval.Day, -1, dtToDate)
                    End If
                    txtFromDate.Text = Format(dtFromDate, modCommon.DateFormat)
                    cboToDate.DataSource = Nothing
                    cboToDate.DataSource = Me.cOCommon.GetCommissionClaimToDateSource(Nothing, Format(dtToDate, modCommon.DateFormat))
                End If
                 */

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
                        //dtToDate.Add(dtFromDate.i

                        txtDateFrom.Text = dtFromDate.ToString(Constants.DATETIME_FORMAT);
                        ddlDateTo.DataSource = null;
                        oResult = commClaimDAL.GetCommissionClaimToDateSource(sCriteria, dtToDate.ToString(Constants.DATETIME_FORMAT));
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
                    }
                }
                if (ddlDateTo.Items.Count > 1)
                {
                    ddlDateTo.SelectedIndex = 1;
                }
                FillReferenceDetailGrid("");
                ddlConversionCurrency.Enabled = true;
            }
            else
            {
                gvData.DataSource = null;
                gvData.DataBind();
                txtTotalFaceValue.Text = "0.00";
                txtTotalNonOrgCommission1.Text = "0.00";
                txtTotalNonOrgCommission2.Text = "0.00";
                txtTotalOrgCommission1.Text = "0.00";
                txtTotalOrgCommission2.Text = "0.00";
                txtTotalCommissionClaim.Text = "0.00";
                if (ddlConversionCurrency.Items.Count > 0)
                {
                    ddlConversionCurrency.SelectedIndex = 0;
                }
                ddlConversionCurrency.Enabled = false;
                txtConversionRate.Text = "0.0000";
                ddlDateTo.SelectedIndex = -1;
                ddlDateTo.Enabled = true;
            }
        }

        private void LoadToDateData()
        {
            /*
              If Me.ActiveControl.Name = "cboDuration" Then Exit Function
            If Me.ActiveControl.Name = "btnSearchRefNo" Then Exit Function
            If Me.cboToDate.SelectedIndex > 0 Then
                Me.FillReferenceDetailGrid()
                Me.cboCurrencyID.Enabled = True
                Me.cboCurrencyID.SelectedValue = Me.cStrSPCurrency
                Me.LoadCurrencyData()
            Else
                Me.cDtSalesStatementData = Nothing
                dgrSalesReferenceDetails.DataSource = Nothing
                dgrSalesReferenceDetails.Refresh()
                dgrRegistrationDetails.DataSource = Nothing
                dgrRegistrationDetails.Refresh()
                Me.txtTotalOrgCommission.Text = "0.00"
                Me.txtTotalNonOrgCommission.Text = "0.00"
                Me.txtConvTotalOrgCommission.Text = "0.00"
                Me.txtConvTotalNonOrgCommission.Text = "0.00"
                If Not IsNothing(Me.cboCurrencyID.DataSource) Then
                    Me.cboCurrencyID.SelectedIndex = 0
                End If
                Me.cboCurrencyID.Enabled = False
                Me.txtConversionRate.Text = "0.0000"
            End If
             */
            if (ddlDateTo.SelectedIndex > 0)
            {
                FillReferenceDetailGrid("");
                ddlConversionCurrency.Enabled = true;
                string sSPCurrency = ViewState[_SPTYPE_CUREENCY] as string;
                if (!string.IsNullOrEmpty(sSPCurrency))
                {
                    DDListUtil.Assign(ddlConversionCurrency, sSPCurrency);
                }
                LoadCurrencyData();
            }
            else
            {
                gvData.DataSource = null;
                gvData.DataBind();
                txtTotalFaceValue.Text = "0.00";
                txtTotalNonOrgCommission1.Text = "0.00";
                txtTotalNonOrgCommission2.Text = "0.00";
                txtTotalOrgCommission1.Text = "0.00";
                txtTotalOrgCommission2.Text = "0.00";
                if (ddlConversionCurrency.Items.Count > 0)
                {
                    ddlConversionCurrency.SelectedIndex = 0;
                }
                ddlConversionCurrency.Enabled = false;
                txtConversionRate.Text = "0.0000";
            }
        }

        private void SetToDate()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            string sCondtion = " WHERE (((b.SaleStatementTransNo) Is Null)";
            sCondtion += " AND ((a.SPTypeID) = '" + ddlSpType.SelectedValue + "') AND a.IsClaimed = 'False' )";
            sCondtion += " AND (a.DivisionID = '" + oConfig.DivisionID + "' AND a.BankID = '" + oConfig.BankCodeID + "')";

            CommissionClaimDAL commClaimDAL = new CommissionClaimDAL();
            Result oResult = commClaimDAL.SetToDate(sCondtion);
            ddlDateTo.DataSource = oResult.Return;
            ddlDateTo.DataTextField = "DisplayMember";
            ddlDateTo.DataValueField = "ValueMember";
            ddlDateTo.DataBind();
        }

        private void FillReferenceDetailGrid(string sCommissionClaimTransNo)
        {
            CommissionClaimDAL commClaimDAL = new CommissionClaimDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = commClaimDAL.LoadReferenceDetailsData(ddlSpType.SelectedValue, Util.GetDateTimeByString(txtDateFrom.Text), Util.GetDateTimeByString(ddlDateTo.SelectedValue), sCommissionClaimTransNo, oConfig.DivisionID, oConfig.BankCodeID);
            if (oResult.Status)
            {
                DataTable dtCommClaim = oResult.Return as DataTable;

                if (dtCommClaim != null && dtCommClaim.Rows.Count > 0)
                {                    
                    dtCommClaim.Columns.Remove("OrgCount");

                    gvData.DataSource = dtCommClaim;
                    gvData.DataBind();

                    if (!string.IsNullOrEmpty(sCommissionClaimTransNo))
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
                        CalculateCommissionClaim();
                    }
                    else
                    {
                        LoadCurrencyData();
                    }
                }
            }
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

        private void CalculateCommissionClaim()
        {
            decimal dTolFaceValue = 0;
            decimal dTolOrgComm = 0;
            decimal dNonTolOrgComm = 0;

            LoadCurrencyData();

            foreach (GridViewRow gvr in gvData.Rows)
            {
                CheckBox chk = (CheckBox)gvr.FindControl("chkSelected");
                if (chk != null)
                {
                    if (chk.Checked)
                    {
                        dTolFaceValue += Util.GetDecimalNumber(gvr.Cells[3].Text);
                        dTolOrgComm += Util.GetDecimalNumber(gvr.Cells[7].Text);
                        dNonTolOrgComm += Util.GetDecimalNumber(gvr.Cells[5].Text);                        
                    }
                }
            }

            #region Assign Data in calculation field set 
           
            txtTotalFaceValue.Text = dTolFaceValue.ToString("N2");
            txtTotalNonOrgCommission1.Text = dNonTolOrgComm.ToString("N2");
            txtTotalNonOrgCommission2.Text = (CalculateConvertedAmount(dNonTolOrgComm, Util.GetDecimalNumber(txtConversionRate.Text))).ToString("N2");
            txtTotalOrgCommission1.Text = (dTolOrgComm).ToString("N2");
            txtTotalOrgCommission2.Text = CalculateConvertedAmount(dTolOrgComm, Util.GetDecimalNumber(txtConversionRate.Text)).ToString("N2");

            txtTotalCommissionClaim.Text = (Util.GetDecimalNumber(txtTotalNonOrgCommission2.Text) + Convert.ToDecimal(txtTotalOrgCommission2.Text)).ToString("N2");

            #endregion
        }

        private decimal CalculateConvertedAmount(decimal dAmount, decimal dConvRate)
        {
            decimal dConvAmount = 0;

            try
            {
                string sSPTypeCurrency = ViewState[_SPTYPE_CUREENCY] as string;
                string sBaseCurrency = ViewState[_BASE_CUREENCY] as string;

                if (!string.IsNullOrEmpty(sSPTypeCurrency) && sSPTypeCurrency.Equals(ddlConversionCurrency.SelectedValue))
                {
                    return dAmount;
                }
                else if (!string.IsNullOrEmpty(sBaseCurrency) && !ddlConversionCurrency.SelectedValue.Equals(sBaseCurrency) && dConvRate > 0)
                {
                    dConvAmount = dAmount / dConvRate;
                }
                else
                {
                    dConvAmount = dAmount * dConvRate;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dConvAmount;

        }

        public void LoadCurrencyData()
        {
            if (ddlConversionCurrency.SelectedIndex > 0)
            {
                double dblTemp = -1;
                ClaimDAL claimDAL = new ClaimDAL();
                Result oResultData = claimDAL.GetConversionRate(ddlConversionCurrency.SelectedValue, ddlSpType.SelectedValue);
                if (oResultData.Status)
                {
                    dblTemp = (double)oResultData.Return;

                    if (dblTemp == 0.0)
                    {
                        txtConversionRate.Text = "0.0000";
                        txtConversionRate.Enabled = true;
                        txtConversionRate.Focus();
                        txtTotalNonOrgCommission2.Text = "0.00";
                        txtTotalOrgCommission2.Text = "0.00";
                        txtTotalCommissionClaim.Text = "0.00";
                    }
                    else
                    {
                        txtConversionRate.Text = dblTemp.ToString("0.0000");                        
                        txtTotalNonOrgCommission2.Text = CalculateConvertedAmount(Util.GetDecimalNumber(txtTotalNonOrgCommission1.Text), Convert.ToDecimal(dblTemp)).ToString("N2");
                        txtTotalOrgCommission2.Text = CalculateConvertedAmount(Util.GetDecimalNumber(txtTotalOrgCommission1.Text), Convert.ToDecimal(dblTemp)).ToString("N2");
                        txtConversionRate.Enabled = false;
                        txtTotalCommissionClaim.Text = (Util.GetDecimalNumber(txtTotalNonOrgCommission2.Text) + Convert.ToDecimal(txtTotalOrgCommission2.Text)).ToString("N2");
                    }
                }
            }
            else
            {
                txtConversionRate.Text = "0.0000";
                txtConversionRate.Enabled = false;
                txtTotalNonOrgCommission2.Text = "0.00";
                txtTotalOrgCommission2.Text = "0.00";
                txtTotalCommissionClaim.Text = "0.00";
            }
        }

        private void ClearControlValues(bool isResetClicked)
        {
            hdnClaimTransNo.Value = string.Empty;
            txtReferenceNo.Text = string.Empty;
            txtDateFrom.Text = string.Empty;
            txtStatementDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtTotalFaceValue.Text = "0.00";
            txtTotalNonOrgCommission1.Text = "0.00";
            txtTotalNonOrgCommission2.Text = "0.00";
            txtTotalOrgCommission1.Text = "0.00";
            txtTotalOrgCommission2.Text = "0.00";
            txtTotalCommissionClaim.Text = "0.00";
            txtConversionRate.Text = "0.0000";
            if (isResetClicked)
            {
                ddlSpType.SelectedIndex = 0;
            }
            ddlDuration.SelectedIndex = 0;
            ddlDateTo.Items.Clear();
            if (ddlConversionCurrency.Items.Count > 0)
            {
                ddlConversionCurrency.SelectedIndex = 0;
            }
            ddlConversionCurrency.Enabled = false;
            ddlYear.Text = DateTime.Now.Year.ToString();
            gvData.DataSource = null;
            gvData.DataBind();

            foreach (ListItem item in chkListForBB.Items)
            {
                item.Selected = false;
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
                }
                //chkListForBB.Enabled = true;
                ddlConversionCurrency.Enabled = false;
                txtConversionRate.ReadOnly = true;
                txtStatementDate.ReadOnly = false;
                txtDateFrom.ReadOnly = false;
                ddlDuration.Enabled = true;
                ddlDateTo.Enabled = true;
                btnCalculate.Enabled = true;
                btnSelectAll.Enabled = true;
                btnDeselectAll.Enabled = true;
                btnSaveAndPreview.Enabled = true;
            }
            else
            {
                ddlSpType.Enabled = false;
                txtReferenceNo.ReadOnly = true;
                ddlYear.Enabled = false;
                ddlConversionCurrency.Enabled = false;
                txtConversionRate.ReadOnly = true;
                txtStatementDate.ReadOnly = true;
                txtDateFrom.ReadOnly = true;
                ddlDuration.Enabled = false;
                ddlDateTo.Enabled = false;
                btnCalculate.Enabled = false;
                btnSelectAll.Enabled = false;
                btnDeselectAll.Enabled = false;
                btnSaveAndPreview.Enabled = false;
            }
        }

        private void Preview(SBM_BLC1.Entity.Claim.CommissionClaim oCommClaim)
        {
            if (IsValidateData())
            {
                //set required data
                oCommClaim.CommissionClaimReferenceNo = txtReferenceNo.Text;
                if (!string.IsNullOrEmpty(ddlConversionCurrency.SelectedItem.Text))
                {
                    string[] CurrencyDesc = ddlConversionCurrency.SelectedItem.Text.Split(':');
                    oCommClaim.Currency.CurrencyDesc = CurrencyDesc[1];
                }
                if (!string.IsNullOrEmpty(ddlSpType.SelectedItem.Text))
                {
                    string[] TypeDesc = ddlSpType.SelectedItem.Text.Split(':');
                    oCommClaim.SPType.TypeDesc = TypeDesc[1];
                }
                CreateReportDocument(oCommClaim);
                //LoadDurationData();
            }
        }

        private SBM_BLC1.Entity.Claim.CommissionClaim GetObject()
        {
            SBM_BLC1.Entity.Claim.CommissionClaim oCommClaim = new SBM_BLC1.Entity.Claim.CommissionClaim();
            oCommClaim.SPType.SPTypeID = ddlSpType.SelectedValue;
            oCommClaim.StatementDate = Util.GetDateTimeByString(txtStatementDate.Text);
            oCommClaim.DurationType = Util.GetIntNumber(ddlDuration.SelectedValue);
            oCommClaim.FromDate = Util.GetDateTimeByString(txtDateFrom.Text);
            oCommClaim.ToDate = Util.GetDateTimeByString(ddlDateTo.SelectedValue);
            oCommClaim.Currency.CurrencyID = ddlConversionCurrency.SelectedValue;
            oCommClaim.ConvRate = Util.GetDoubleNumber(txtConversionRate.Text);
            oCommClaim.TotalFaceValue = Util.GetDoubleNumber(txtTotalFaceValue.Text);
            oCommClaim.TotalNonOrgCommission = Util.GetDoubleNumber(txtTotalNonOrgCommission1.Text);
            oCommClaim.TotalOrgCommission = Util.GetDoubleNumber(txtTotalOrgCommission1.Text);

            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            oCommClaim.UserDetails.MakerID = oConfig.UserName;
            oCommClaim.UserDetails.Division = oConfig.DivisionID;
            oCommClaim.UserDetails.BankID = oConfig.BankCodeID;

            return oCommClaim;
        }

        private DataTable GetDataTable()
        {
            CommissionClaimDAL commClaimDAL = new CommissionClaimDAL();
            DataTable dtCommissionClaim = new DataTable("dtCommissionClaim");

            dtCommissionClaim.Columns.Add(new DataColumn("SaleStatementTransNo", typeof(string)));
            dtCommissionClaim.Columns.Add(new DataColumn("TotalFaceValue", typeof(string)));
            dtCommissionClaim.Columns.Add(new DataColumn("TotalOrgCommission", typeof(string)));
            dtCommissionClaim.Columns.Add(new DataColumn("TotalNonOrgCommission", typeof(string)));
            
            string saleTransNos = "";
            foreach (GridViewRow gvr in gvData.Rows)
            {
                CheckBox chk = (CheckBox)gvr.FindControl("chkSelected");
                if (chk != null)
                {
                    if (chk.Checked)
                    {
                        saleTransNos += "," + (gvr.FindControl("hdnSalesStatementTranNo") as HiddenField).Value;                        
                    }
                }
            }
            
            if (!string.IsNullOrEmpty(saleTransNos))
            {
                saleTransNos = saleTransNos.Substring(1);
                Result oResult;
                oResult = commClaimDAL.LoadReferenceDetailsData(saleTransNos);
                if (oResult.Status)
                {
                    dtCommissionClaim = oResult.Return as DataTable;
                }
            }

            return dtCommissionClaim;
        }

        public bool IsValidateData()
        {
            bool bResult = true;
    
            if (gvData.Rows.Count > 0)
            {
                if (GetDataTable().Rows.Count <= 0)
                {
                    bResult = false;
                    ucMessage.OpenMessage("Select at least one Sales Statement.", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);                    
                }

                if (ddlConversionCurrency.SelectedIndex < 1)
                {
                    bResult = false;
                    ucMessage.OpenMessage("Currency Type cannot be empty.", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }

                if (Util.GetDoubleNumber(txtConversionRate.Text) <= 0)
                {
                    bResult = false;
                    ucMessage.OpenMessage("Conversion Rate cannot be ZERO.", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }                
            }

            return bResult;
        }

        public void CreateReportDocument(SBM_BLC1.Entity.Claim.CommissionClaim oCommClaim)
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
                Session["ExportType"] = ddlExportType.SelectedValue;
                Result oResult = oCRDAL.CommissionClaimReportDocument(oCommClaim, GetDataTable(), "", isCoverLetter, oConfig.BranchID);
                if (oResult.Status)
                {
                    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                }
            }
        }

        public bool SaveAndPreviewAction()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            
            if (oConfig != null)
            {
                if (!txtStatementDate.Text.Equals(DateTime.Now.ToString(Constants.DATETIME_FORMAT)))
                {
                    ucMessage.OpenMessage("Statement Date cannot be greater than or less than todays date.", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
                if (txtReferenceNo.Text.Equals(string.Empty))
                {
                    if (!IsValidateData())
                    {
                        return false;
                    }
                }


                if (txtReferenceNo.Text.Equals(string.Empty))
                {
                    CalculateCommissionClaim();

                    #region     01.  GetObject    ... Get Value

                    SBM_BLC1.Entity.Claim.CommissionClaim oCommClaim = GetObject();
                    DataTable dtCommClaim = GetDataTable();

                    #endregion

                    #region     02. Call and varify..
                    CommissionClaimDAL commClaimDAL = new CommissionClaimDAL();
                    Result oResult = commClaimDAL.InsertData(oCommClaim, dtCommClaim);
                    if (oResult.Status)
                    {
                        txtReferenceNo.Text = Convert.ToString(oResult.Return);
                        //ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
                    }
                    else
                    {
                        ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
                        return false;
                    }
                    #endregion

                    #region     03. for preview
                    Preview(oCommClaim);
                    #endregion
                }

                return true;
            }

            return false;
        }

        public void BindCurrency(ClaimDAL cDal)
        {
            string sCurrecncyCriteria = " INNER JOIN (SELECT     TOP (1) PolicyID, MAX(PolicyEffectDate) AS PolicyEffectDate FROM SPMS_SPPolicy WHERE (SPTypeID = '" + ddlSpType.SelectedValue + "') GROUP BY PolicyID ORDER BY PolicyEffectDate DESC) AS b INNER JOIN SPMS_SPCurrencyPolicy ON b.PolicyID = SPMS_SPCurrencyPolicy.PolicyID ON a.CurrencyID = SPMS_SPCurrencyPolicy.CurrencyID ";
            sCurrecncyCriteria += " WHERE (SPMS_SPCurrencyPolicy.ActivityType = " + (int)Constants.ACTIVITY_TYPE.COMMISSION_CLAIM + ")";

            if (cDal == null)
            {
                cDal = new ClaimDAL();
            }
            ddlConversionCurrency.DataSource = cDal.GetCurrencySource(sCurrecncyCriteria) as DataTable;
            ddlConversionCurrency.DataTextField = "DisplayMember";
            ddlConversionCurrency.DataValueField = "ValueMember";
            ddlConversionCurrency.DataBind();
        }
        #endregion        

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Result oResult = null;
            try
            {

                if (string.IsNullOrEmpty(hdnClaimTransNo.Value))
                {
                    ucMessage.OpenMessage("Please select a valid Commission Claim for delete.", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
                else
                {
                    CommissionClaimDAL iClaimDAL = new CommissionClaimDAL();
                    oResult = iClaimDAL.DeleteCommissionClaim(hdnClaimTransNo.Value);

                    if (oResult.Status)
                    {
                        ucMessage.OpenMessage("Commission Claim data deleted successfully.", Constants.MSG_APPROVED_SAVE_DATA);
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
    }
}
