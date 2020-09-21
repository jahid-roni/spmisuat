/*
 * File name            : SystemConfigSetup.cs
 * Author               : A.K.M. Tanjil Hossain,
 *                      : A.K.M. Zahidul Quaium,
 *                      : Tanvir Alam
 *                      : Aysha Afreen
 * Date                 : Sep 01,2014
 * Version              : 1.0
 *
 * Description          : System Config Setup Page
 *
 * Modification history :
 * Name                         Date                            Desc
 * Tanvir Alam                Sep 01,2014                Business implementation 
 * A.K.M. Zahidul Quaium        February 02,2012                Business implementation              
 * Aysha Afreen                 April    02,2012                Business implementation              
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
using SBM_BLC1.Transaction;
using SBM_BLC1.DAL.Claim;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.DAL.Report;
using System.Globalization;

namespace SBM_WebUI.mp
{
    public partial class InterestClaim : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CLAIM.INTEREST))
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

            // year
            for (int i = 1995; i < DateTime.Now.Year +1 +1; i++)
            {
                DDListUtil.Add(ddlYear, i.ToString(), i.ToString());
            }

            ddlYear.Text = DateTime.Now.Year.ToString();
            //Set ToDate
            txtClaimDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtPaymentDateFrom.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtPaymentDateTo.Text = DateTime.Now.ToString("dd/MM/yyyy");
            
            ClaimDAL claimDAL = new ClaimDAL();
            //Store BASE_CURRENCY
            ViewState[_BASE_CUREENCY] = claimDAL.GetBaseCurrencyID();

        }
        #endregion InitializeData

        public void ClaimSearchLoadAction(DataRow dtRow)
        {
            if (dtRow != null)
            {
                string sInterestClaimTransNo = Convert.ToString(dtRow["InterestClaimTransNo"]);
                hdnClaimTransNo.Value = sInterestClaimTransNo;

                txtReferenceNo.Text = Convert.ToString(dtRow["Reference No"]);
                
                DateTime parsedDate;
                DateTime.TryParseExact(Convert.ToString(dtRow["Statement Date"]), Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                txtClaimDate.Text = parsedDate.ToString(Constants.DATETIME_FORMAT);
                DateTime.TryParseExact(Convert.ToString(dtRow["From Date"]), Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                txtPaymentDateFrom.Text = parsedDate.ToString(Constants.DATETIME_FORMAT);
                DateTime.TryParseExact(Convert.ToString(dtRow["To Date"]), Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                txtPaymentDateTo.Text = parsedDate.ToString(Constants.DATETIME_FORMAT);
                ddlYear.Text = Util.GetDateTimeByString(txtClaimDate.Text).Year.ToString();
                ddlSpType.Text = Convert.ToString(dtRow["SP Type"]);
                string sSPCurrency = ViewState[_SPTYPE_CUREENCY] as string;
                //if (string.IsNullOrEmpty(sSPCurrency))
                {
                    ClaimDAL claimDAL = new ClaimDAL();
                    ViewState[_SPTYPE_CUREENCY] = claimDAL.GetSPCurrency(ddlSpType.SelectedValue);
                }
                
                FillIntPaymentDetailGrid(sInterestClaimTransNo);


                //DDListUtil.Assign(ddlConversionCurrency, Convert.ToString(dtRow["Currency ID"]));
                //txtConversionRate.Text = Convert.ToString(dtRow["Conversion Rate"]);

                txtTotalLevi1.Text = Convert.ToString(dtRow["Levi"]);
                txtTotalIncomeTax1.Text = Convert.ToString(dtRow["Income Tax"]);
                txtTotalInterest1.Text = Convert.ToString(dtRow["Interest Amount"]);

                txtRemuneration.Text = "0.00";// Format(vDataRow(cOInterestReimbursementClaim.cOproperties.Remuneration), "0.00")
                //temporary
                txtTotalIncomeTax2.Text = CalculateConvertedAmount(Util.GetDecimalNumber(txtTotalIncomeTax1.Text), Util.GetDecimalNumber(txtConversionRate.Text)).ToString("0.00"); //Mid(Format(Me.CalculateConvertedAmount(Val(Me.txtTotalIncomeTax.Text.Trim), Val(Me.txtConversionRate.Text.Trim)), "0.0000"), 1, Len(Format(Me.CalculateConvertedAmount(Val(Me.txtTotalIncomeTax.Text.Trim), Val(Me.txtConversionRate.Text.Trim)), "0.0000")) - 2)
                //temporary
                txtTotalLevi2.Text = "0.00";//;Mid(Format(Me.CalculateConvertedAmount(Val(Me.txtTotalLevi.Text.Trim), Val(Me.txtConversionRate.Text.Trim)), "0.0000"), 1, Len(Format(Me.CalculateConvertedAmount(Val(Me.txtTotalLevi.Text.Trim), Val(Me.txtConversionRate.Text.Trim)), "0.0000")) - 2)
                txtTotalInterest2.Text = CalculateConvertedAmount(Util.GetDecimalNumber(txtTotalInterest1.Text), Util.GetDecimalNumber(txtConversionRate.Text)).ToString("0.00");//Mid(Format(Me.CalculateConvertedAmount(Val(Me.txtTotalInterest.Text.Trim), Val(Me.txtConversionRate.Text.Trim)), "0.0000"), 1, Len(Format(Me.CalculateConvertedAmount(Val(Me.txtTotalInterest.Text.Trim), Val(Me.txtConversionRate.Text.Trim)), "0.0000")) - 2)
                //'Code Added By Jahid
                //'Me.txtTotalInterest.Text = Format(CDbl(Me.txtTotalInterest.Text), "0.00") ' - CDbl(Me.txtTotalLevi.Text) - CDbl(Me.txtTotalIncomeTax.Text)
                //'Me.txtTotalConvertedInterest.Text = Format(CDbl(Me.txtTotalConvertedInterest.Text), "0.00") ' - CDbl(Me.txtTotalConvertedLevi.Text) - CDbl(Me.txtTotalConvertedIncomeTax.Text)

                //strToDate = Format(CDate(vDataRow(cOInterestReimbursementClaim.cOproperties.ToDate)), modCommon.DateFormat)

                //Me.btnSelectAll.Enabled = False
                //Me.btnDeselectAll.Enabled = False


                EnableDisableControls(false);
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
                ddlConversionCurrency.Enabled = true;
                txtConversionRate.ReadOnly = false;
                txtClaimDate.ReadOnly = false;
                txtPaymentDateFrom.Enabled = true;
                txtPaymentDateTo.Enabled = true;                
                btnCalculate.Enabled = true;
                btnSelectAll.Enabled = true;
                btnDeselectAll.Enabled = true;
                btnShowData.Enabled = true;
                btnSaveAndPreview.Enabled = true;

            }
            else
            {
                ddlSpType.Enabled = false;
                txtReferenceNo.ReadOnly = true;
                ddlYear.Enabled = false;                
                ddlConversionCurrency.Enabled = false;
                txtConversionRate.ReadOnly = true;
                txtClaimDate.ReadOnly = true;
                txtPaymentDateFrom.Enabled = false;
                txtPaymentDateTo.Enabled = false;                
                btnCalculate.Enabled = false;
                btnSelectAll.Enabled = false;
                btnDeselectAll.Enabled = false;
                btnShowData.Enabled = false;
                btnSaveAndPreview.Enabled = false;
            }
        }

        #region Event

        protected void ddlSpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearControlValues();
        }

        protected void btnShowData_Click(object sender, EventArgs e)
        {
            if (ddlSpType.SelectedValue == "3MS" && ddlReportCategory.SelectedValue == "ALL")
            {
                return;
            }
            if (ddlSpType.SelectedValue == "BSP" && ddlReportCategory.SelectedValue == "ALL")
            {
                return;
            }
            LoadSPTypeData();
        }

        private void LoadSPTypeData()
        {
            if (ddlSpType.SelectedIndex <= 0)
            {
                ClearControlValues();
            }
            else
            {
                ClearControlValues();
                FillIntPaymentDetailGrid(string.Empty);
            }
        }

        private void FillIntPaymentDetailGrid(string sInterestClaimTransNo)
        {
            InterestClaimDAL intClaimDAL = new InterestClaimDAL();
            ClaimDAL claimDAL = new ClaimDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            /////Currency Update
            string sCurrecncyCriteria = " INNER JOIN (SELECT     TOP (1) PolicyID, MAX(PolicyEffectDate) AS PolicyEffectDate FROM SPMS_SPPolicy WHERE (SPTypeID = '" + ddlSpType.SelectedValue + "') GROUP BY PolicyID ORDER BY PolicyEffectDate DESC) AS b INNER JOIN SPMS_SPCurrencyPolicy ON b.PolicyID = SPMS_SPCurrencyPolicy.PolicyID ON a.CurrencyID = SPMS_SPCurrencyPolicy.CurrencyID ";
            sCurrecncyCriteria += " WHERE (SPMS_SPCurrencyPolicy.ActivityType = " + (int)Constants.ACTIVITY_TYPE.INTEREST_CLAIM + ")";

            DataTable dtCurrencySource = claimDAL.GetCurrencySource(sCurrecncyCriteria);

            if (dtCurrencySource != null)
            {
                ddlConversionCurrency.DataSource = dtCurrencySource;
                ddlConversionCurrency.DataTextField = "DisplayMember";
                ddlConversionCurrency.DataValueField = "ValueMember";
                ddlConversionCurrency.DataBind();
                ddlConversionCurrency.Enabled = false ;
            }

            string sSPCurrency = ViewState[_SPTYPE_CUREENCY] as string;

            if (string.IsNullOrEmpty(sSPCurrency))
            {
                sSPCurrency = claimDAL.GetSPCurrency(ddlSpType.SelectedValue);
            }

            ViewState[_SPTYPE_CUREENCY] = sSPCurrency;//claimDAL.GetSPCurrency(ddlSpType.SelectedValue);

            LoadCurrencyData();
            ////


            string reportType = "";
            reportType = ddlReportCategory.SelectedValue;
            Result oResult = intClaimDAL.GetInterestClaimData(ddlSpType.SelectedValue, Util.GetDateTimeByString(txtPaymentDateFrom.Text), Util.GetDateTimeByString(txtPaymentDateTo.Text), reportType, sInterestClaimTransNo, oConfig.DivisionID, oConfig.BankCodeID);

            if (oResult.Status)
            {
                DataTable dtIntClaim = oResult.Return as DataTable;

                if (dtIntClaim != null && dtIntClaim.Rows.Count > 0)
                {
                    hdnGridTotal.Value = dtIntClaim.Rows.Count.ToString();
                    
                    dtIntClaim.Columns.Remove("CurrencyID");
                    dtIntClaim.Columns.Remove("InterestRate");
                    dtIntClaim.Columns.Remove("PaidInterestA");                    
                    dtIntClaim.Columns.Remove("IncomeTaxA");
                    dtIntClaim.Columns.Remove("PaymentAmountA");

                    gvData.DataSource = dtIntClaim;
                    gvData.DataBind();

                    DDListUtil.Assign(ddlConversionCurrency, Convert.ToString(dtIntClaim.Rows[0]["PCurrencyID"]));
                    if (ddlConversionCurrency.SelectedIndex.Equals(0))
                    {
                        ucMessage.OpenMessage("Please assign currency in Policy Setup for Interest Claim.", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    }

                    txtConversionRate.Text = Convert.ToString(dtIntClaim.Rows[0]["PConvRate"]);
                    txtConversionRate.Enabled = false;

                    if (!string.IsNullOrEmpty(sInterestClaimTransNo))
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
                        txtSelectCount.Text = "TOTAL SELECTED " + dtIntClaim.Rows.Count + " OF " + dtIntClaim.Rows.Count;
                    }
                    else
                    {
                        txtSelectCount.Text = "TOTAL SELECTED 0 OF " + dtIntClaim.Rows.Count;
                    }
                }
            }
        }

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            CalculateInterestClaim();            
        }

        private void CalculateInterestClaim()
        {
            decimal dCalInterest = 0;
            decimal dIncomeTax = 0;
            int iSelectedCount = 0;
            string sRegNo = "";
            foreach (GridViewRow gvr in gvData.Rows)
            {
                CheckBox chk = (CheckBox)gvr.FindControl("chkSelected");
                if (chk != null)
                {
                    if (chk.Checked)
                    {
                        dCalInterest += Util.GetDecimalNumber(gvr.Cells[7].Text);//Payment Amount to be calculated
                        dIncomeTax += Util.GetDecimalNumber(gvr.Cells[6].Text);
                        sRegNo = Convert.ToString(gvr.Cells[1].Text).Trim();
                        iSelectedCount++;
                    }
                }
            }
            InterestClaimDAL intClaimDAL = new InterestClaimDAL();
            Result cdresult = intClaimDAL.GetConversionDetails(sRegNo);
            string sCCY = "00";
            decimal dRate = 1;
            if (cdresult.Status)
            {
                DataTable dtConversionCCY = (DataTable)cdresult.Return;
                sCCY = Convert.ToString(dtConversionCCY.Rows[0]["CurrencyID"]);
                dRate = Convert.ToDecimal(dtConversionCCY.Rows[0]["ConvRate"]);
            }
            DDListUtil.Assign(ddlConversionCurrency, sCCY);
            ddlConversionCurrency.Enabled = false;
            txtConversionRate.Text = dRate.ToString("N2");
            txtConversionRate.Enabled = false;

            #region Assign Data in calculation field set
            txtSelectCount.Text = "TOTAL SELECTED " + iSelectedCount.ToString() + " OF " + gvData.Rows.Count;
            txtTotalIncomeTax1.Text = dIncomeTax.ToString("N2");
            txtRemuneration.Text = CalculateRemuneration(false).ToString("N2");
            txtTotalIncomeTax2.Text = (CalculateConvertedAmount(dIncomeTax, Util.GetDecimalNumber(txtConversionRate.Text))).ToString("N2");
            txtTotalInterest1.Text = (dCalInterest).ToString("N2");
            txtTotalInterest2.Text = CalculateConvertedAmount(dCalInterest, Util.GetDecimalNumber(txtConversionRate.Text)).ToString("N2");

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

        protected void ddlConversionCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCurrencyData();
        }

        protected void btnSelectAll_Click(object sender, EventArgs e)
        {
            SelectDeselectAllCheck(true);            
            CalculateInterestClaim();
        }

        protected void btnDeselectAll_Click(object sender, EventArgs e)
        {
            SelectDeselectAllCheck(false);            
            CalculateInterestClaim();
        }
        
        protected void btnReset_Click(object sender, EventArgs e)
        {
            EnableDisableControls(true);
            ClearControlValues();

            txtPaymentDateFrom.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtPaymentDateTo.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtClaimDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            ddlSpType.SelectedIndex = 0;
            txtSelectCount.Text = string.Empty;
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            if (IsRowSelected())
            {
                CalculateInterestClaim();
                SBM_BLC1.Entity.Claim.InterestClaim oIClaim = GetObject();
                Preview(oIClaim);
            }
            else
            {
                ucMessage.OpenMessage("Please select data to preview.", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }

        }

        protected void btnSaveAndPreview_Click(object sender, EventArgs e)
        {
            if (SaveAndPreviewAction())
            {
                ClearControlValues();
                LoadSPTypeData();
            }
        }

        public bool SaveAndPreviewAction()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (oConfig != null)
            {
                if (IsValidateData())
                {
                    pnlMsg.Visible = false;

                    DataTable dtInterestClaim = GetDataTable();
                    if (dtInterestClaim.Rows.Count > 0)
                    {
                        CalculateInterestClaim();

                        #region     03.  GetObject    ... Get Value

                        SBM_BLC1.Entity.Claim.InterestClaim oIClaim = GetObject();
                        #endregion  03.  GetObject    ... Get Value

                        #region     04. Call and verify..
                        InterestClaimDAL iClaimDAL = new InterestClaimDAL();
                        Result oResult = iClaimDAL.InsertData(oIClaim, dtInterestClaim);
                        if (!oResult.Status)
                        {
                            ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
                            ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                            return false;
                        }

                        txtReferenceNo.Text = oResult.Return as string;

                        #endregion  04. Call and varify..

                        #region     05. for preview
                        Preview(oIClaim);
                        #endregion  05. for preview
                    }
                    else
                    {
                        ucMessage.OpenMessage("Select at least one registration to save & print Interest claim.", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        return false;
                    }
                }
                else
                {
                    ucMessage.OpenMessage("Please select a single transaction for Interest claim.", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    return false;
                }

                return true;
            }

            return false;
        }

        private void Preview(SBM_BLC1.Entity.Claim.InterestClaim oIClaim)
        {
            if (IsValidateData())
            {
                CalculateInterestClaim();
                pnlMsg.Visible = false;
                //set required data
                oIClaim.InterestClaimReferenceNo = txtReferenceNo.Text;
                if (!string.IsNullOrEmpty(ddlConversionCurrency.SelectedItem.Text))
                {
                    string[] CurrencyDesc = ddlConversionCurrency.SelectedItem.Text.Split(':');
                    oIClaim.Currency.CurrencyDesc = CurrencyDesc[1].Trim();
                }
                if (!string.IsNullOrEmpty(ddlSpType.SelectedItem.Text))
                {
                    string[] TypeDesc = ddlSpType.SelectedItem.Text.Split(':');
                    oIClaim.SPType.TypeDesc = TypeDesc[1].Trim();
                }
                if (chkListForBB.Items[0].Selected || ddlSpType.SelectedItem.Text.Contains("DIB") || ddlSpType.SelectedItem.Text.Contains("DPB"))
                {
                    CreateReportDocument(oIClaim);
                }
                if (!ddlSpType.SelectedItem.Text.Contains("DIB") && !ddlSpType.SelectedItem.Text.Contains("DPB"))
                {
                    CreateReportDetailDocument(oIClaim);
                }
            }
            else
            {
                ucMessage.OpenMessage("Please select a single transaction for Interest claim.", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }

        private DataTable GetDataTable()
        {
            DataTable dtInterestClaim = new DataTable("dtInterestClaim");

            dtInterestClaim.Columns.Add(new DataColumn("IntPaymentTransNo", typeof(string)));
            dtInterestClaim.Columns.Add(new DataColumn("NoOfCoupon", typeof(string)));
            dtInterestClaim.Columns.Add(new DataColumn("PaidInterestA", typeof(string)));
            dtInterestClaim.Columns.Add(new DataColumn("Levi", typeof(string)));
            dtInterestClaim.Columns.Add(new DataColumn("IncomeTax", typeof(string)));            

            DataRow rowIC = null;
            foreach (GridViewRow gvr in gvData.Rows)
            {
                CheckBox chk = (CheckBox)gvr.FindControl("chkSelected");
                if (chk != null)
                {
                    if (chk.Checked)
                    {
                        rowIC = dtInterestClaim.NewRow();

                        rowIC["IntPaymentTransNo"] = (gvr.FindControl("hdnIntPayTranNo") as HiddenField).Value;
                        rowIC["NoOfCoupon"] = (gvr.FindControl("hdnNoOfCoupon") as HiddenField).Value;
                        rowIC["PaidInterestA"] = gvr.Cells[4].Text;
                        rowIC["Levi"] = (gvr.FindControl("hdnLevi") as HiddenField).Value;
                        rowIC["IncomeTax"] = gvr.Cells[6].Text;

                        dtInterestClaim.Rows.Add(rowIC);
                    }
                }
            }

            return dtInterestClaim;
        }

        private SBM_BLC1.Entity.Claim.InterestClaim GetObject()
        {
            SBM_BLC1.Entity.Claim.InterestClaim oIClaim = new SBM_BLC1.Entity.Claim.InterestClaim();
            oIClaim.SPType.SPTypeID = ddlSpType.SelectedValue;
            oIClaim.StatementDate = Util.GetDateTimeByString(txtClaimDate.Text);
            oIClaim.FromDate = Util.GetDateTimeByString(txtPaymentDateFrom.Text);
            oIClaim.ToDate = Util.GetDateTimeByString(txtPaymentDateTo.Text);
            oIClaim.Currency.CurrencyID = ddlConversionCurrency.SelectedValue;
            oIClaim.ConvRate = Util.GetDoubleNumber(txtConversionRate.Text);
            oIClaim.InterestAmount = Util.GetDoubleNumber(txtTotalInterest1.Text);
            oIClaim.IncomeTax = Util.GetDoubleNumber(txtTotalIncomeTax1.Text);
            oIClaim.Levi = Util.GetDoubleNumber(txtTotalLevi1.Text);
            oIClaim.Remuneration = Util.GetDoubleNumber(txtRemuneration.Text);
            oIClaim.CalculatedInterestAmount = oIClaim.InterestAmount + oIClaim.IncomeTax;

            if (ddlReportCategory.SelectedValue == "ALL")
            {
                oIClaim.ReportCategory = 0;
            }
            else if (ddlReportCategory.SelectedValue == "OLD")
            {
                oIClaim.ReportCategory = 1;
            }
            else if (ddlReportCategory.SelectedValue == "NEW")
            {
                oIClaim.ReportCategory = 2;
            }

            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            
            oIClaim.UserDetails.MakerID = oConfig.UserName;
            oIClaim.UserDetails.Division = oConfig.DivisionID;
            oIClaim.UserDetails.BankID = oConfig.BankCodeID;           

            return oIClaim;
        }
        #endregion Event

        #region Utility Methods
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
                        txtTotalIncomeTax2.Text = "0.00";                        
                        txtTotalInterest2.Text = "0.00";                        
                    }
                    else
                    {
                        txtConversionRate.Text = dblTemp.ToString("0.0000");
                        txtConversionRate.Enabled = false;
                        txtTotalIncomeTax2.Text = CalculateConvertedAmount(Util.GetDecimalNumber(txtTotalIncomeTax1.Text), Convert.ToDecimal(dblTemp)).ToString("N2");                        
                        txtTotalInterest2.Text = CalculateConvertedAmount(Util.GetDecimalNumber(txtTotalInterest1.Text), Convert.ToDecimal(dblTemp)).ToString("N2");
                    }
                }
            }
            else
            {
                txtConversionRate.Text = "0.0000";
                txtConversionRate.Enabled = false;
                txtTotalIncomeTax2.Text = "0.00";
                txtTotalInterest2.Text = "0.00";
            }
        }

        private string GetInterestPaymentTransNo(string sTransNo, bool isSingle)
        {
            string sList = string.Empty;
            try
            {
                foreach (GridViewRow gvr in gvData.Rows)
                {
                    CheckBox chk = (CheckBox)gvr.FindControl("chkSelected");
                    if (chk != null)
                    {
                        if (chk.Checked)
                        {
                            HiddenField hdTranNo = (HiddenField)gvr.FindControl("hdnIntPayTranNo");
                            if (hdTranNo != null)
                            {
                                if (sList.Length > 0)
                                {
                                    sList += " , ";
                                }
                                sList += " '" + hdTranNo.Value + "' ";
                            }
                        }
                    }
                }
                if (isSingle)
                {
                    string[] sTmp = sList.Split(',');
                    if (sTmp.Length > 1)
                    {
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return sList;
            
            //        Try
            //        {
            //        Dim drow As DataRow
            //        Dim drows() As DataRow
            //        Dim dcRows() As DataRow
            //        Dim intI As Integer
            //        Dim strTemp As String
            //        strTemp = ""
            //        drows = Me.dgrIntPaymentDetails._Table.Select("IsSelect=True")
            //        If vSingle = True Then
            //            If drows.Length > 1 Then
            //                Throw New Exception("Please select a single transaction for interest claim.")
            //            End If
            //        End If
            //        For Each drow In drows
            //            If (vTransNo = CStr(drow("IntPaymentTransNo")) And vInclude) Or vTransNo <> CStr(drow("IntPaymentTransNo")) Then
            //                If strTemp = "" Then
            //                    strTemp &= "('" & CStr(drow("IntPaymentTransNo")) & "'"
            //                Else
            //                    strTemp &= ",'" & CStr(drow("IntPaymentTransNo")) & "'"
            //                End If
            //            End If
            //        Next
            //        If vInclude Then
            //            If strTemp = "" Then
            //                strTemp &= "('" & vTransNo & "')"
            //            Else
            //                strTemp &= ",'" & vTransNo & "')"
            //            End If
            //        Else
            //            If strTemp = "" Then
            //                strTemp &= "('')"
            //            Else
            //                strTemp &= ")"
            //            End If
            //        End If
            //        Return strTemp
            //        }
            //    Catch ex As Exception
            //        Throw ex
            //    End Try

            //End Function
            
        }

        public double CalculateRemuneration(bool status)
        {
            string sTransNos = GetInterestPaymentTransNo("", status);
            double dRemuneration = 0;

            if (sTransNos.Length > 0)
            {
                InterestClaimDAL icDAL = new InterestClaimDAL();                
                dRemuneration = icDAL.CalculateRemuneration(sTransNos);                
            }

            return dRemuneration;
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
                
                if (status)
                {
                    txtSelectCount.Text = "TOTAL SELECTED " + gvData.Rows.Count + " OF " + gvData.Rows.Count;
                }
                else
                {
                    txtSelectCount.Text = "TOTAL SELECTED 0 OF " + gvData.Rows.Count;
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsSingleSelected(bool bSingle)
        {
            int iCount = 0;
            if (bSingle)
            {
                foreach (GridViewRow gvr in gvData.Rows)
                {
                    CheckBox chk = (CheckBox)gvr.FindControl("chkSelected");
                    if (chk != null)
                    {
                        if (chk.Checked)
                        {
                            iCount++;
                        }
                    }
                    if (iCount > 1)
                    {
                        break;
                    }
                }
            }
            if (iCount > 1 && bSingle)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void CreateReportDocument(SBM_BLC1.Entity.Claim.InterestClaim oIClaim)
        {
            ClaimReportDAL oCRDAL = new ClaimReportDAL();
            Result oResult = new Result();
            string sInterpaymentTransNos = GetInterestPaymentTransNo(txtReferenceNo.Text, false);// IsValidateData());
            //string sSelectList = GetInterestPaymentTransNo("", false);
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (oConfig != null)
            {
                Session["ExportType"] = ddlExportType.SelectedValue;

                oResult = oCRDAL.InterestReportDocument(oIClaim, sInterpaymentTransNos, oConfig.BranchID);
                if (oResult.Status)
                {
                    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport2(1));
                }
                else
                {
                    throw new Exception(oResult.Message);
                }
            }
        }

        public void CreateReportDetailDocument(SBM_BLC1.Entity.Claim.InterestClaim oIClaim)
        {            
            string sInterpaymentTransNos = GetInterestPaymentTransNo(txtReferenceNo.Text, false);

            ClaimReportDAL oCRDAL = new ClaimReportDAL();
            Result oResult = new Result();

            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (oConfig != null)
            {
                Session["ExportType"] = ddlExportType.SelectedValue;
                string sRegNos = GetRegNo();
                oResult = oCRDAL.InterestReportDetailDocument(oIClaim, sInterpaymentTransNos, sRegNos, oConfig.BranchID);
                if (oResult.Status)
                {
                    Session[Constants.SES_RPT_DATA_2] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW_2, Util.OpenReport2(2));
                }
                else
                {
                    throw new Exception(oResult.Message);
                }
            }
        }

        public bool IsValidateData()
        {
            bool bResult = false;
            InterestClaimDAL oIcDAL = new InterestClaimDAL();
            Result oResult = new Result();
            oResult = oIcDAL.GetReportFormat(ddlSpType.SelectedValue);
            int iRepFrmt = -1;
            if (oResult.Status)
            {
                iRepFrmt = (int)oResult.Return;
            }

            if (iRepFrmt == (int)SBM_BLV1.baseCommon.enmInterestClaimReportFormat.WDB || iRepFrmt == (int)SBM_BLV1.baseCommon.enmInterestClaimReportFormat.DollarBond)
            {
                // single Trans ID is needed
                bResult = IsSingleSelected(true);
            }
            else if (iRepFrmt == (int)SBM_BLV1.baseCommon.enmInterestClaimReportFormat.SP || iRepFrmt == (int)SBM_BLV1.baseCommon.enmInterestClaimReportFormat.SP1)
            {
                // no need to single Trans data
                bResult = true;
            }
            return bResult;
        }

        public string GetRegNo()
        {
            string sList = string.Empty;
            
            try
            {
                foreach (GridViewRow gvr in gvData.Rows)
                {
                    CheckBox chk = (CheckBox)gvr.FindControl("chkSelected");
                    if (chk != null)
                    {
                        if (chk.Checked)
                        {                           
                            sList += ",'" + gvr.Cells[1].Text + "' ";
                        }
                    }
                }
                //Discard first occurance of ','
                if (sList.Length > 0)
                {
                    sList = sList.Substring(1);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return sList;
        }

        private bool IsRowSelected()
        {
            bool isRowselected = false;
                        
            try
            {
                if (gvData.Rows.Count > 0)
                {
                    foreach (GridViewRow gvr in gvData.Rows)
                    {
                        CheckBox chk = (CheckBox)gvr.FindControl("chkSelected");
                        if (chk != null)
                        {
                            if (chk.Checked)
                            {
                                isRowselected = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isRowselected;
        }

        private void ClearControlValues()
        {
            txtReferenceNo.Text = string.Empty;
            txtSelectCount.Text = string.Empty;
            txtTotalIncomeTax1.Text = "0.00";
            txtTotalLevi1.Text = "0.00";
            txtTotalInterest1.Text = "0.00";
            txtRemuneration.Text = "0.00";
            txtTotalIncomeTax2.Text = "0.00";
            txtTotalLevi2.Text = "0.00";
            txtTotalInterest2.Text = "0.00";
            txtConversionRate.Text = "0.0000";
            if (ddlConversionCurrency.Items.Count > 0)
            {
                ddlConversionCurrency.SelectedIndex = 0;
            }
            if (chkListForBB.Items.Count > 1)
            {
                chkListForBB.Items[0].Selected = false;
                chkListForBB.Items[1].Selected = false;
            }
            ddlConversionCurrency.Enabled = false;
            ddlYear.Text = DateTime.Now.Year.ToString();
            gvData.DataSource = null;
            gvData.DataBind();
        }
        #endregion        

        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");
        }

        protected void txtConversionRate_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtConversionRate.Text) || Convert.ToDecimal(txtConversionRate.Text) <= 0)
            {
                txtTotalIncomeTax2.Text = "0.00";
                txtTotalLevi2.Text = "0.00";
                txtTotalInterest2.Text = "0.00";                
            }
            else
            {
                txtTotalIncomeTax2.Text = CalculateConvertedAmount(Util.GetDecimalNumber(txtTotalIncomeTax1.Text), Util.GetDecimalNumber(txtConversionRate.Text)).ToString("N2");
                txtTotalLevi2.Text = "0.00";
                txtTotalInterest2.Text = CalculateConvertedAmount(Util.GetDecimalNumber(txtTotalInterest1.Text), Util.GetDecimalNumber(txtConversionRate.Text)).ToString("N2");                
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Result oResult = null;
            try
            {

            if (string.IsNullOrEmpty(hdnClaimTransNo.Value))
            {
                ucMessage.OpenMessage("Please select a valid Interest Claim for delete.", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
            else
            {
                InterestClaimDAL iClaimDAL = new InterestClaimDAL();
                oResult =iClaimDAL.DeleteInterestClaim(hdnClaimTransNo.Value);

                if (oResult.Status)
                {
                    ucMessage.OpenMessage("Interest Claim data deleted successfully.", Constants.MSG_APPROVED_SAVE_DATA);
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
