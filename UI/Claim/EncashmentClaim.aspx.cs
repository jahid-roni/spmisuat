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
using SBM_BLC1.DAL.Claim;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Transaction;
using SBM_BLC1.DAL.Report;

namespace SBM_WebUI.mp
{
    public partial class EncashmentClaim : System.Web.UI.Page
    {

        public string sBaseCurrency = "";
        public string sSPCurrency = "";

        public string _BASE_CUREENCY = "baseCurrency";
        public string _SPTYPE_CUREENCY = "spTypeCurrency";
        public string _ENCASH_CUREENCY = "encashCurrency";

        public decimal dConversionRate = 1.00m;  // comes from XML or other place. 

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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CLAIM.ENCASHMENT))
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
            //gvData.DataSource = null;
            //gvData.DataBind();
            ClearTotal();

            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            txtPaymentDateFrom.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtPaymentDateTo.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtClaimDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            // year
            for (int i = 1995; i < DateTime.Now.Year +1 +1; i++)
            {
                DDListUtil.Add(ddlYear, i.ToString(), i.ToString());
            }
            ddlYear.Text = DateTime.Now.Year.ToString();//set current year
            txtConversionRate.Text = dConversionRate.ToString();
            //Store BASE_CURRENCY
            ClaimDAL claimDAL = new ClaimDAL();
            ViewState[_BASE_CUREENCY] = claimDAL.GetBaseCurrencyID();
        }
        #endregion InitializeData


        #region Event
        public void ClaimSearchLoadAction(DataRow dtRow)
        {
            ClearTotal();
            if (dtRow != null)
            {
                /*
                 Me.nudYear.Value = CType(Format(Me.dtpStatementDate.Value, "yyyy"), Decimal)
                Me.cBlnInvalidRefNo = False
                cboSPType.SelectedValue = CStr(vDataRow(cOEncashReimbursementClaim.cOproperties.SPTypeID))
                Me.cStrSPCurrency = Me.cOclsRptCommissionClaim.GetSPCurrency(Me.cboSPType.SelectedValue)
                strCurrecncyCriteria = " INNER JOIN ([SELECT top 1  PolicyID,max(PolicyEffectDate) from  SPMS_SPPolicy WHERE SPTypeID='" & Me.cboSPType.SelectedValue & "' group by PolicyID order by max(PolicyEffectDate) DESC ]. AS b INNER JOIN SPMS_SPCurrencyPolicy ON b.PolicyID = SPMS_SPCurrencyPolicy.PolicyID) ON a.CurrencyID = SPMS_SPCurrencyPolicy.CurrencyID"
                strCurrecncyCriteria &= " WHERE (((SPMS_SPCurrencyPolicy.ActivityType)=" & modCommon.enmActivityType.Principal_Claim & "))"
                Me.cboCurrencyID.DataSource = Nothing
                Me.cboCurrencyID.DataSource = cOCommon.GetCurrencySource(strCurrecncyCriteria)
                cboCurrencyID.SelectedValue = CStr(vDataRow(cOEncashReimbursementClaim.cOproperties.CurrencyID))
                txtConversionRate.Text = Strings.Format(vDataRow(cOEncashReimbursementClaim.cOproperties.ConvRate), "0.0000")

                txtTotalLevi.Text = Format(vDataRow(cOEncashReimbursementClaim.cOproperties.Levi), "0.00")
                txtTotalIncomeTax.Text = Format(vDataRow(cOEncashReimbursementClaim.cOproperties.IncomeTax), "0.00")
                txtTotalInterest.Text = Format(vDataRow(cOEncashReimbursementClaim.cOproperties.InterestAmount), "0.00")
                txtTotalPrincipal.Text = Format(vDataRow(cOEncashReimbursementClaim.cOproperties.PrincipalAmount), "0.00")
                txtRemuneration.Text = Format(vDataRow(cOEncashReimbursementClaim.cOproperties.Remuneration), "0.00")

                Me.btnSelectAll.Enabled = False
                Me.btnDeselectAll.Enabled = False

                Try
                    Me.DisableButtons(True)
                    If Not modCommon.IsProcessing Then
                        modCommon.StartProcessing(Me, Me.enmFunctionIndex.LoadSearchedData)
                    Else
                        Me.LoadSearchedData()
                    End If
                Catch ex As Exception
                    Throw ex
                Finally
                    Me.DisableButtons(False)
                End Try
                 */

                ddlSpType.Text = Convert.ToString(dtRow["SP Type"]);               
                ClaimDAL claimDAL = new ClaimDAL();
                ViewState[_SPTYPE_CUREENCY] = claimDAL.GetSPCurrency(ddlSpType.SelectedValue);
                ViewState[_ENCASH_CUREENCY] = Convert.ToString(dtRow["CurrencyID"]);
                txtConversionRate.Text = Convert.ToString(dtRow["Conv. Rate"]);
                hdnClaimTransNo.Value = Convert.ToString(dtRow["EncashmentClaimTransNo"]);                
                LoadShowData(Convert.ToString(dtRow["EncashmentClaimTransNo"]));
                txtReferenceNo.Text = Convert.ToString(dtRow["Reference No"]);
                txtClaimDate.Text = Convert.ToDateTime(dtRow["Statement Date"]).ToString(Constants.DATETIME_FORMAT);
                /*
                
                txtPaymentDateFrom.Text = Convert.ToDateTime(dtRow["Reference No"]);
                txtPaymentDateTo.Text = Convert.ToDateTime(dtRow["Reference No"]);
                ddlYear.Text = Util.GetDateTimeByString(txtClaimDate.Text).Year.ToString();
                txtConversionRate.Text = Convert.ToString(dtRow["Reference No"]);
                txtTotalIncomeTax1.Text = Convert.ToString(dtRow["Reference No"]);
                txtTotalIncomeTax2.Text = "";
                txtTotalInterest1.Text = Convert.ToString(dtRow["Reference No"]);
                txtTotalInterest2.Text = "";
                txtTotalPrincipal1.Text = Convert.ToString(dtRow["Reference No"]);
                txtTotalPrincipal2.Text = "";
                txtRemuneration.Text = ""; */               
                EnableDisableControls(false);
            }
        }

        protected void ddlSpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearTotal();
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
            ClearTotal();
            LoadShowData("");
        }

        protected void ddlConversionCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCurrencyData();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            EnableDisableControls(true);
            ClearTotal();

            txtPaymentDateFrom.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtPaymentDateTo.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtClaimDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            ddlSpType.SelectedIndex = 0;
            txtTotalSelectedRow.Text = string.Empty;
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            if (IsRowSelected())
            {
                //CalculateData();
                if (ddlSpType.SelectedValue == "3MS" && ddlReportCategory.SelectedValue == "ALL")
                {
                    return;
                }
                if (ddlSpType.SelectedValue == "BSP" && ddlReportCategory.SelectedValue == "ALL")
                {
                    return;
                }

                PreviewAction();
            }
            else
            {
                ucMessage.OpenMessage("Please select data to preview.", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }
        protected void btnSaveAndPreview_Click(object sender, EventArgs e)
        {
            if (ddlSpType.SelectedValue == "3MS" && ddlReportCategory.SelectedValue == "ALL")
            {
                return;
            }
            if (ddlSpType.SelectedValue == "BSP" && ddlReportCategory.SelectedValue == "ALL")
            {
                return;
            }
            
            if (SaveAndPreviewAction())
            {
                ClearTotal();
                //LoadShowData("");
            }
        }
        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, null);
        }

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            CalculateData();
            CalculateTotalNoOfSelection();
            CalculateRemuneration(true);
        }

        protected void btnSelectAll_Click(object sender, EventArgs e)
        {
            SelectDeselectAllCheck(true);
            CalculateRemuneration(true);
            CalculateData();
        }
        protected void btnDeselectAll_Click(object sender, EventArgs e)
        {
            SelectDeselectAllCheck(false);
            CalculateRemuneration(false);
            CalculateData();
        }

        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");
        }
        #endregion Event


        #region Utility methods

        private bool IsRowSelected()
        {
            bool isRowselected = false;

            try
            {
                if (gvData.Rows.Count > 0)
                {
                    foreach (GridViewRow gvr in gvData.Rows)
                    {
                        CheckBox chk = (CheckBox)gvr.FindControl("IsSelect");
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

        private void PreviewAction()
        {
            //   01. varify..
            if (IsValidateData())
            {
                CalculateData();
                CalculateTotalNoOfSelection();
                CalculateRemuneration(true);

                #region  for printing
                if (chkWithCoverLetter.Checked ||ddlSpType.SelectedItem.Text.Contains("DIB") || ddlSpType.SelectedItem.Text.Contains("DPB"))
                {
                    CreateReportDocument();
                }
                if (!ddlSpType.SelectedItem.Text.Contains("DIB") && !ddlSpType.SelectedItem.Text.Contains("DPB"))
                {
                    CreateReportDetailDocument();
                }
                #endregion for printing
            }
            else
            {
                ucMessage.OpenMessage("Please select a single transaction for " + ddlSpType.SelectedValue + ".", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }

        public bool SaveAndPreviewAction()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                if (IsValidateData())
                {
                    CalculateData();
                    CalculateTotalNoOfSelection();
                    CalculateRemuneration(true);

                    DataTable dtECChild = GetObjectChild();
                    if (dtECChild.Rows.Count > 0)
                    {
                        EncashmentClaimDAL oEcDAL = new EncashmentClaimDAL();
                        Result oResult = new Result();

                        #region     01.  GetObject
                        SBM_BLC1.Entity.Claim.EncashmentClaim oEC = GetObjectMain();

                        #endregion  01.  GetObject

                        #region     02 Call and varify..
                        try
                        {
                            oResult = (Result)oEcDAL.InsertData(oEC, dtECChild);

                            if (!oResult.Status)
                            {
                                ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
                                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                                return false;
                            }

                            txtReferenceNo.Text = oResult.Return as string;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion  02. Call and varify..

                        #region  03. for printing
                        if (chkWithCoverLetter.Checked || ddlSpType.SelectedItem.Text.Contains("DIB") || ddlSpType.SelectedItem.Text.Contains("DPB"))
                        {
                            CreateReportDocument();
                        }
                        if (!ddlSpType.SelectedItem.Text.Contains("DIB") && !ddlSpType.SelectedItem.Text.Contains("DPB"))
                        {
                            CreateReportDetailDocument();
                        }

                        #endregion for printing
                        //if (chkWithCoverLetter.Checked)
                        //{
                        //    CreateReportDocument();
                        //}
                        //CreateReportDetailDocument();
                    }
                    else
                    {
                        ucMessage.OpenMessage("Select at least one registration to save & preview.", Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        return false;
                    }
                }
                else
                {
                    ucMessage.OpenMessage("Please select a single transaction for " + ddlSpType.SelectedValue + ".", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    return false;
                }

                return true;
            }

            return false;
        }

        public bool IsValidateData()
        {
            bool bResult = false;
            EncashmentClaimDAL oEcDAL = new EncashmentClaimDAL();
            Result oResult = new Result();
            oResult = oEcDAL.GetReportFormat(ddlSpType.SelectedValue);
            int iRepFrmt = -1;
            if (oResult.Status)
            {
                iRepFrmt = (int)oResult.Return;
            }

            if (iRepFrmt == ((int)SBM_BLV1.baseCommon.enmEncashmentClaimReportFormat.WDB) || iRepFrmt == ((int)SBM_BLV1.baseCommon.enmEncashmentClaimReportFormat.DollarBond))
            {
                // single Trans ID is needed
                bResult = IsSingle(true);
            }
            else if (iRepFrmt == ((int)SBM_BLV1.baseCommon.enmEncashmentClaimReportFormat.SP))
            {
                // no need to single Trans data
                bResult = true;
            }
            return bResult;
        }

        public void CreateReportDocument()
        {
            ClaimReportDAL oCRDAL = new ClaimReportDAL();
            Result oResult = new Result();

            string sSelectList = GetTransNos(false);
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (oConfig != null)
            {
                string sConvPrinciple = CalculateConvertedAmount(Convert.ToDouble(txtTotalPrincipal1.Text), Convert.ToDouble(txtConversionRate.Text)).ToString("N2");
                string sConvInt = CalculateConvertedAmount(Convert.ToDouble(txtTotalInterest1.Text), Convert.ToDouble(txtConversionRate.Text)).ToString("N2");
                string sConvTax = CalculateConvertedAmount(Convert.ToDouble(txtTotalIncomeTax1.Text), Convert.ToDouble(txtConversionRate.Text)).ToString("N2");

                string sRemunerationReport = Convert.ToString(CalculateConvertedAmount(Util.GetDoubleNumber(txtRemuneration.Text), Util.GetDoubleNumber(txtConversionRate.Text)));

                string sNetPayment = (Util.GetDecimalNumber(sConvPrinciple) + Util.GetDecimalNumber(sConvInt) + Util.GetDecimalNumber(sRemunerationReport) - Util.GetDecimalNumber(sConvTax)).ToString("N2");
                string sAlreadyPaidInt = GetAlReadyPaidInterestSummery();
                string ConversionCurrencyTextData = ddlConversionCurrency.SelectedItem.Text;
                string[] sCCTextData = ConversionCurrencyTextData.Split(':');
                string sCurrencyText = sCCTextData[1].Trim();

                string[] sTypes = ddlSpType.SelectedItem.Text.Split(':');
                string sType = sTypes[1].Trim();
                DateTime claimDate = Util.GetDateTimeByString(txtClaimDate.Text);
                oResult = oCRDAL.EncashmentReportDocument(ddlSpType.SelectedValue, sType, txtReferenceNo.Text, sSelectList, txtConversionRate.Text, ddlConversionCurrency.SelectedValue, sConvPrinciple, sConvTax, txtTotalLevi2.Text, sConvInt, sRemunerationReport, sNetPayment, "", sAlreadyPaidInt, sBaseCurrency, sCurrencyText, oConfig.BankCodeID, oConfig.DivisionID, oConfig.BranchID, claimDate);
                if (oResult.Status)
                {
                    Session["ExportType"] = ddlExportType.SelectedValue;
                    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport2(1));
                }
            }
        }

        public void CreateReportDetailDocument()
        {
            ClaimReportDAL oCRDAL = new ClaimReportDAL();
            Result oResult = new Result();

            string sSelectList = GetTransNos(false);
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (oConfig != null)
            {
                string sRemunerationReport = Convert.ToString(CalculateConvertedAmount(Util.GetDoubleNumber(txtRemuneration.Text), Util.GetDoubleNumber(txtConversionRate.Text)));
                string sNetPayment = Convert.ToString(Util.GetDoubleNumber(txtTotalPrincipal2.Text) + CalculateConvertedAmount(Util.GetDoubleNumber(txtRemuneration.Text), Util.GetDoubleNumber(txtConversionRate.Text)) + Convert.ToDouble(txtTotalInterest2.Text) - (Convert.ToDouble(txtTotalIncomeTax2.Text) + Convert.ToDouble(txtTotalLevi2.Text)));
                string sAmountInWordsReport = ddlConversionCurrency.SelectedValue + " " +
                    SBM_BLC1.Common.String.NumberToWords(Convert.ToInt32(
                        Util.GetDoubleNumber(txtTotalPrincipal2.Text) +
                        CalculateConvertedAmount(Util.GetDoubleNumber(txtRemuneration.Text), Util.GetDoubleNumber(txtConversionRate.Text)) +
                        Util.GetDoubleNumber(txtTotalInterest2.Text) -
                        (
                            Util.GetDoubleNumber(txtTotalIncomeTax2.Text) + Util.GetDoubleNumber(txtTotalLevi2.Text))
                        ));
                string sAlreadyPaidInt = GetAlReadyPaidInterestSummery();
                string ConversionCurrencyTextData = ddlConversionCurrency.SelectedItem.Text;
                string[] sCCTextData = ConversionCurrencyTextData.Split(':');
                string sCurrencyText = sCCTextData[1].Trim();

                string[] sTypes = ddlSpType.SelectedItem.Text.Split(':');
                string sType = sTypes[1].Trim();


                string sRegList = GetRegNo();
                DateTime statDate = Util.GetDateTimeByString(txtClaimDate.Text);
                oResult = oCRDAL.EncashmentReportDetailDocument(ddlSpType.SelectedValue, sType, txtReferenceNo.Text, sSelectList, txtConversionRate.Text, ddlConversionCurrency.SelectedValue, txtTotalPrincipal2.Text, txtTotalIncomeTax2.Text, txtTotalLevi2.Text, txtTotalInterest2.Text, sRemunerationReport, sNetPayment, sAmountInWordsReport + " ONLY ", sAlreadyPaidInt, sBaseCurrency, sCurrencyText, chkWithCoverLetter.Checked, sRegList, oConfig.BankCodeID, oConfig.DivisionID, oConfig.BranchID, statDate, Convert.ToString(ddlReportCategory.SelectedValue).Substring(0, 1));
                if (oResult.Status)
                {
                    Session["ExportType"] = ddlExportType.SelectedValue;
                    Session[Constants.SES_RPT_DATA_2] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW_2, Util.OpenReport2(2));
                }
            }
        }

        public bool IsSingle(bool bSingle)
        {
            int iCount = 0;
            if (bSingle)
            {
                foreach (GridViewRow gvr in gvData.Rows)
                {
                    CheckBox chk = (CheckBox)gvr.FindControl("IsSelect");
                    if (chk != null)
                    {
                        if (chk.Checked)
                        {
                            iCount++;
                        }
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

        public SBM_BLC1.Entity.Claim.EncashmentClaim GetObjectMain()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            SBM_BLC1.Entity.Claim.EncashmentClaim oEC = new SBM_BLC1.Entity.Claim.EncashmentClaim();

            oEC.SPType.SPTypeID = ddlSpType.SelectedValue;
            oEC.StatementDate = Util.GetDateTimeByString(txtClaimDate.Text);
            oEC.FromDate = Util.GetDateTimeByString(txtPaymentDateFrom.Text);
            oEC.ToDate = Util.GetDateTimeByString(txtPaymentDateTo.Text);
            oEC.DurationType = ((int)SBM_BLV1.baseCommon.enmDurationType.Daily).ToString();
            oEC.Currency.CurrencyID = ddlConversionCurrency.SelectedValue;
            oEC.ConvRate = Util.GetDoubleNumber(txtConversionRate.Text);
            oEC.PrincipalAmount = Util.GetDoubleNumber(txtTotalPrincipal1.Text.Trim());
            oEC.InterestAmount = Util.GetDoubleNumber(txtTotalInterest1.Text.Trim());
            oEC.Levi = Util.GetDoubleNumber(txtTotalLevi1.Text.Trim());
            oEC.IncomeTax = Util.GetDoubleNumber(txtTotalIncomeTax1.Text.Trim());
            oEC.Remuneration = Util.GetDoubleNumber(txtRemuneration.Text.Trim());

            if (ddlReportCategory.SelectedValue == "ALL")
            {
                oEC.ReportCategory = 0;
            }
            else if (ddlReportCategory.SelectedValue == "OLD")
            {
                oEC.ReportCategory = 1;
            }
            else if (ddlReportCategory.SelectedValue == "NEW")
            {
                oEC.ReportCategory = 2;
            }
            else if (ddlReportCategory.SelectedValue == "PRE")
            {
                oEC.ReportCategory = 3;
            }
            else if (ddlReportCategory.SelectedValue == "RIV")
            {
                oEC.ReportCategory = 4;
            }

            oEC.UserDetails.MakerID = oConfig.UserName;
            oEC.UserDetails.Division = oConfig.DivisionID;
            oEC.UserDetails.BankID = oConfig.BankCodeID;

            return oEC;
        }

        public DataTable GetObjectChild()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            DataTable dtChild = new DataTable();

            dtChild.Columns.Add(new DataColumn("EncashmentClaimTransNo", typeof(string)));
            dtChild.Columns.Add(new DataColumn("EncashmentTransNo", typeof(string)));
            dtChild.Columns.Add(new DataColumn("NoOfCouponsToBeEncashed", typeof(string)));
            dtChild.Columns.Add(new DataColumn("ActualPrincipalAmount", typeof(string)));
            dtChild.Columns.Add(new DataColumn("InterestToBePaid", typeof(string)));
            dtChild.Columns.Add(new DataColumn("LeviToBePaid", typeof(string)));
            dtChild.Columns.Add(new DataColumn("IncomeTaxToBePaid", typeof(string)));
            dtChild.Columns.Add(new DataColumn("MakerID", typeof(string)));

            DataRow rowChild = null;
            foreach (GridViewRow gvr in gvData.Rows)
            {
                CheckBox chk = (CheckBox)gvr.FindControl("IsSelect");
                HiddenField hdCoupons = (HiddenField)gvr.FindControl("NoOfCouponsToBeEncashed");

                HiddenField hdEncashmentTransNo = (HiddenField)gvr.FindControl("EncashmentTransNo");
                if (chk != null && hdCoupons != null && hdEncashmentTransNo != null)
                {
                    if (chk.Checked)
                    {
                        rowChild = dtChild.NewRow();
                        rowChild["EncashmentClaimTransNo"] = txtReferenceNo.Text;
                        rowChild["EncashmentTransNo"] = hdEncashmentTransNo.Value;

                        rowChild["NoOfCouponsToBeEncashed"] = hdCoupons.Value;
                        rowChild["ActualPrincipalAmount"] = gvr.Cells[4].Text;
                        rowChild["InterestToBePaid"] = gvr.Cells[5].Text;
                        rowChild["LeviToBePaid"] = gvr.Cells[6].Text;
                        rowChild["IncomeTaxToBePaid"] = gvr.Cells[6].Text;
                        rowChild["MakerID"] = oConfig.UserName;

                        dtChild.Rows.Add(rowChild);
                    }
                }
            }

            return dtChild;
        }

        public string GetAlReadyPaidInterestSummery()
        {
            double dInt = 0.0;
            foreach (GridViewRow gvr in gvData.Rows)
            {
                CheckBox chk = (CheckBox)gvr.FindControl("IsSelect");
                if (chk != null)
                {
                    if (chk.Checked)
                    {
                        HiddenField hdTranNo = (HiddenField)gvr.FindControl("AlreadyPaidInterest");
                        if (hdTranNo != null)
                        {
                            dInt += Util.GetDoubleNumber(hdTranNo.Value);
                        }
                    }
                }
            }
            return Convert.ToString(dInt);
        }

        public string GetTransNos(bool bSingle)
        {
            string sList = "";
            try
            {
                foreach (GridViewRow gvr in gvData.Rows)
                {
                    CheckBox chk = (CheckBox)gvr.FindControl("IsSelect");
                    if (chk != null)
                    {
                        if (chk.Checked)
                        {
                            HiddenField hdTranNo = (HiddenField)gvr.FindControl("EncashmentTransNo");
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
                if (bSingle)
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
        }

        public string GetRegNo()
        {
            string sList = "";
            try
            {
                foreach (GridViewRow gvr in gvData.Rows)
                {
                    CheckBox chk = (CheckBox)gvr.FindControl("IsSelect");
                    if (chk != null)
                    {
                        if (chk.Checked)
                        {
                            if (sList.Length > 0)
                            {
                                sList += " , ";
                            }
                            sList += " '" + gvr.Cells[1].Text + "' ";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return sList;
        }

        public void CalculateRemuneration(bool status)
        {
            string sTransNos = GetTransNos(false);
            if (sTransNos.Length > 0)
            {
                EncashmentClaimDAL ecDAL = new EncashmentClaimDAL();
                Result oResult = new Result();
                oResult = (Result)ecDAL.CalculateRemuneration(sTransNos);
                if (oResult.Status)
                {
                    txtRemuneration.Text = Convert.ToString(oResult.Return);
                }
                else
                {
                    txtRemuneration.Text = "0.00";
                }
            }
            else
            {
                txtRemuneration.Text = "0.00";
            }
        }

        public void ClearTotal()
        {
            txtReferenceNo.Text = string.Empty;
            txtTotalPrincipal1.Text = "0.00";
            txtTotalIncomeTax1.Text = "0.00";
            txtRemuneration.Text = "0.00";
            txtTotalIncomeTax2.Text = "0.00";
            txtTotalPrincipal2.Text = "0.00";
            chkWithCoverLetter.Checked = false;
            if (ddlConversionCurrency.Items.Count > 0)
            {
                ddlConversionCurrency.SelectedIndex = 0;
            }
            txtTotalLevi1.Text = "0.00";
            txtTotalLevi2.Text = "0.00";
            txtTotalInterest1.Text = "0.00";
            txtConversionRate.Text = "0.0000";
            txtTotalInterest2.Text = "0.00";

            gvData.DataSource = null;
            gvData.DataBind();
        }

        public void LoadShowData(string strEncashmentClaimTransNo)
        {
            string sEncashCurrncy = string.Empty;
            string sCurrecncyCriteria = string.Empty;
            // this block is used for Reset the all controls..                         

            if (ddlConversionCurrency.DataSource != null)
            {
                ddlConversionCurrency.SelectedIndex = 0;
            }
            ddlConversionCurrency.Enabled = false;

            // this block is used for loading data based on input data.... 
            ClaimDAL claimDAL = new ClaimDAL();
            sCurrecncyCriteria = " INNER JOIN ( ( SELECT top 1  PolicyID,max(PolicyEffectDate) as PolicyEffectDate from  SPMS_SPPolicy WHERE SPTypeID='" + ddlSpType.SelectedValue + "' group by PolicyID order by max(PolicyEffectDate) DESC ) AS b INNER JOIN SPMS_SPCurrencyPolicy ON b.PolicyID = SPMS_SPCurrencyPolicy.PolicyID) ON a.CurrencyID = SPMS_SPCurrencyPolicy.CurrencyID ";
            sCurrecncyCriteria += " WHERE (( SPMS_SPCurrencyPolicy.ActivityType =" + (int)Constants.ACTIVITY_TYPE.PRINCIPAL_CLAIM + "))";
            DataTable dtCurrencySource = claimDAL.GetCurrencySource(sCurrecncyCriteria);
            if (dtCurrencySource != null)
            {
                ddlConversionCurrency.DataSource = dtCurrencySource;
                ddlConversionCurrency.DataTextField = "DisplayMember";
                ddlConversionCurrency.DataValueField = "ValueMember";
                ddlConversionCurrency.DataBind();
                ddlConversionCurrency.Enabled = true;
            }
            sEncashCurrncy = ViewState[_ENCASH_CUREENCY] as string;
            ddlConversionCurrency.Enabled = true;
            if (string.IsNullOrEmpty(sEncashCurrncy))
            {
                sSPCurrency = claimDAL.GetSPCurrency(ddlSpType.SelectedValue);
                DDListUtil.Assign(ddlConversionCurrency, sSPCurrency);
            }
            else
            {
                DDListUtil.Assign(ddlConversionCurrency, sEncashCurrncy);
            }
            if (string.IsNullOrEmpty(strEncashmentClaimTransNo))
            {
                LoadCurrencyData();
            }
            FillEncashmentDetailGrid(strEncashmentClaimTransNo);
        }

        public void FillEncashmentDetailGrid(string strEncashmentClaimTransNo)
        {
            DataTable dtDataTable = null;
            EncashmentClaimDAL oEcDAL = new EncashmentClaimDAL();
            ClaimDAL claimDAL = new ClaimDAL();
            Result oResult = new Result();
            try
            {
                sBaseCurrency = ViewState[_BASE_CUREENCY] as string;
                sSPCurrency = ViewState[_SPTYPE_CUREENCY] as string;
                if (string.IsNullOrEmpty(sSPCurrency))
                {
                    sSPCurrency = claimDAL.GetSPCurrency(ddlSpType.SelectedValue);
                }
                ViewState[_SPTYPE_CUREENCY] = sSPCurrency;

                DataColumn dtColIsSelect = new DataColumn();
                dtColIsSelect.AllowDBNull = false;
                dtColIsSelect.DefaultValue = false;
                dtColIsSelect.ReadOnly = false;
                dtColIsSelect.DataType = System.Type.GetType("System.Boolean");
                dtColIsSelect.Caption = "Select";
                dtColIsSelect.ColumnName = "IsSelect";

                DateTime dtFromDate = Util.GetDateTimeByString(txtPaymentDateFrom.Text);
                DateTime dtToDate = Util.GetDateTimeByString(txtPaymentDateTo.Text);
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                string reportType = "";
                reportType = ddlReportCategory.SelectedValue;
                oResult = oEcDAL.LoadRegistrationDetailsData(ddlSpType.SelectedValue, reportType, strEncashmentClaimTransNo, dtFromDate, dtToDate, Convert.ToInt16(ddlYear.SelectedValue), "", oConfig.DivisionID, oConfig.BankCodeID);
                if (oResult.Status)
                {
                    dtDataTable = (DataTable)oResult.Return;

                    DDListUtil.Assign(ddlReportCategory, Convert.ToString(dtDataTable.Rows[0]["ReportType"]));

                    dtDataTable.Columns.Add(dtColIsSelect);

                    dtDataTable.Columns["IsSelect"].SetOrdinal(0);
                    dtDataTable.Columns["RegNo"].SetOrdinal(1);
                    dtDataTable.Columns["IssueName"].SetOrdinal(2);
                    dtDataTable.Columns["EncashDate"].SetOrdinal(3);
                    dtDataTable.Columns["ActualPrincipalAmount"].SetOrdinal(4);
                    dtDataTable.Columns["InterestToBePaid"].SetOrdinal(5);
                    dtDataTable.Columns["LeviToBePaid"].SetOrdinal(6);
                    dtDataTable.Columns["IncomeTaxToBePaid"].SetOrdinal(7);

                    if (dtDataTable.Rows.Count > 0 && dConversionRate > 0)
                    {
                        DDListUtil.Assign(ddlConversionCurrency, Convert.ToString(dtDataTable.Rows[0]["PCurrencyID"]));
                        if (ddlConversionCurrency.SelectedIndex.Equals(0))
                        {
                            ucMessage.OpenMessage("Please assign currency in Policy Setup for Interest Claim.", Constants.MSG_TYPE_INFO);
                            ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        }

                        txtConversionRate.Text = Convert.ToString(dtDataTable.Rows[0]["PConvRate"]);
                        txtConversionRate.Enabled = false;

                        gvData.DataSource = dtDataTable;
                        gvData.DataBind();

                        if (!string.IsNullOrEmpty(strEncashmentClaimTransNo))
                        {
                            foreach (GridViewRow gvr in gvData.Rows)
                            {
                                CheckBox chk = (CheckBox)gvr.FindControl("IsSelect");
                                if (chk != null)
                                {
                                    chk.Checked = true;
                                    chk.Enabled = false;
                                }
                            }
                            CalculateData();
                        }
                    }
                   
                    CalculateTotalNoOfSelection();
                }               
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        private double CalculateConvertedAmount(double vAmount, double vConvRate)
        {
            try
            {
                string sBaseCurrency = ViewState[_BASE_CUREENCY] as string;
                string sSPCurrency = ViewState[_SPTYPE_CUREENCY] as string;
                if (sSPCurrency == ddlConversionCurrency.SelectedValue)
                {
                    return vAmount;
                }
                else if (ddlConversionCurrency.SelectedValue != this.sBaseCurrency & vConvRate > 0)
                {
                    return vAmount * vConvRate;
                }
                else
                {
                    //return vAmount / vConvRate;
                    return vAmount * vConvRate;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CalculateData()
        {
            decimal dPrincipal = 0;
            decimal dInterest = 0;
            decimal dLevi = 0;
            decimal dIncomeTax = 0;
            string sRegNo = "";
            try
            {
                foreach (GridViewRow gvr in gvData.Rows)
                {
                    CheckBox chk = (CheckBox)gvr.FindControl("IsSelect");
                    if (chk != null)
                    {
                        if (chk.Checked)
                        {
                            dPrincipal += Util.GetDecimalNumber(gvr.Cells[4].Text);
                            dInterest += Util.GetDecimalNumber(gvr.Cells[5].Text);
                            dLevi += Util.GetDecimalNumber(gvr.Cells[6].Text);
                            dIncomeTax += Util.GetDecimalNumber(gvr.Cells[7].Text);
                            sRegNo = Convert.ToString(gvr.Cells[1].Text).Trim();
                        }
                    }
                }

                string sCCY = "00";
                decimal dRate = 1;

                if (ddlSpType.SelectedValue.ToString() == "DIB" ||
                            ddlSpType.SelectedValue.ToString() == "DPB")
                {
                    EncashmentClaimDAL encClaimDAL = new EncashmentClaimDAL();
                    Result cdresult = encClaimDAL.GetConversionDetails(sRegNo);

                    if (cdresult.Status)
                    {
                        DataTable dtConversionCCY = (DataTable)cdresult.Return;
                        sCCY = Convert.ToString(dtConversionCCY.Rows[0]["CurrencyID"]);
                        dRate = Convert.ToDecimal(dtConversionCCY.Rows[0]["ConvRate"]);
                    }
                }

                DDListUtil.Assign(ddlConversionCurrency, sCCY);
                ddlConversionCurrency.Enabled = false;
                txtConversionRate.Text = dRate.ToString("N2");
                txtConversionRate.Enabled = false;

                Double dbConvRate = Convert.ToDouble(txtConversionRate.Text);
                txtTotalPrincipal1.Text = dPrincipal.ToString("N2");
                txtTotalPrincipal2.Text = CalculateConvertedAmount(Convert.ToDouble(dPrincipal), Convert.ToDouble(dbConvRate)).ToString("N2");

                txtTotalIncomeTax1.Text = dIncomeTax.ToString("N2");
                txtTotalIncomeTax2.Text = CalculateConvertedAmount(Convert.ToDouble(dIncomeTax), Convert.ToDouble(dbConvRate)).ToString("N2");

                txtTotalInterest1.Text = dInterest.ToString("N2");
                txtTotalInterest2.Text = CalculateConvertedAmount(Convert.ToDouble(dInterest), Convert.ToDouble(dbConvRate)).ToString("N2");

                txtTotalLevi1.Text = dLevi.ToString("N2");
                txtTotalLevi2.Text = CalculateConvertedAmount(Convert.ToDouble(dLevi), Convert.ToDouble(dbConvRate)).ToString("N2");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SelectDeselectAllCheck(bool status)
        {
            try
            {
                foreach (GridViewRow gvr in gvData.Rows)
                {
                    CheckBox chk = (CheckBox)gvr.FindControl("IsSelect");
                    if (chk != null)
                    {
                        chk.Checked = status;
                    }
                }
                string sText = string.Empty;
                if (status)
                {
                    sText = "Total selected " + gvData.Rows.Count + " of " + gvData.Rows.Count;
                }
                else
                {
                    sText = "Total selected 0 of " + gvData.Rows.Count;
                }
                txtTotalSelectedRow.Text = sText.ToUpper();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CalculateTotalNoOfSelection()
        {
            try
            {
                int iCount = 0;
                foreach (GridViewRow gvr in gvData.Rows)
                {
                    CheckBox chk = (CheckBox)gvr.FindControl("IsSelect");
                    if (chk != null)
                    {
                        if (chk.Checked)
                        {
                            iCount++;
                        }
                    }
                }
                string sText = "Total selected " + iCount + " of " + gvData.Rows.Count;
                txtTotalSelectedRow.Text = sText.ToUpper();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void LoadCurrencyData()
        {
            if (ddlConversionCurrency.SelectedIndex != 0)
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
                        txtTotalLevi2.Text = "0.00";
                        txtTotalInterest2.Text = "0.00";
                        txtTotalPrincipal2.Text = "0.00";
                    }
                    else if (dblTemp == claimDAL.GetConversionRateForSameTypeSP(ddlSpType.SelectedValue))
                    {
                        txtConversionRate.Text = System.String.Format("{0:0.0000}", dblTemp);
                        txtConversionRate.Enabled = false;
                        txtTotalIncomeTax2.Text = System.String.Format("{0:0.00}", CalculateConvertedAmount(Convert.ToDouble(txtTotalIncomeTax1.Text), dblTemp));
                        txtTotalLevi2.Text = System.String.Format("{0:0.00}", CalculateConvertedAmount(Convert.ToDouble(txtTotalLevi1.Text), dblTemp));
                        txtTotalInterest2.Text = System.String.Format("{0:0.00}", CalculateConvertedAmount(Convert.ToDouble(txtTotalInterest1.Text), dblTemp));
                        txtTotalPrincipal2.Text = System.String.Format("{0:0.00}", CalculateConvertedAmount(Convert.ToDouble(txtTotalPrincipal1.Text), dblTemp));
                        dConversionRate = Convert.ToDecimal(dblTemp);
                    }
                    else
                    {
                        txtConversionRate.Text = "0.0000";
                        txtConversionRate.Enabled = false;
                        txtTotalIncomeTax2.Text = "0.00";
                        txtTotalLevi2.Text = "0.00";
                        txtTotalInterest2.Text = "0.00";
                        txtTotalPrincipal2.Text = "0.00";
                        dConversionRate = 0.0m;
                    }
                }
            }
            else
            {
                txtConversionRate.Text = "0.0000";
                txtConversionRate.Enabled = false;
                txtTotalIncomeTax2.Text = "0.00";
                txtTotalLevi2.Text = "0.00";
                txtTotalInterest2.Text = "0.00";
                txtTotalPrincipal2.Text = "0.00";
                dConversionRate = 0.0m;
            }
        }

        private void EnableDisableControls(bool isEnabled)
        {
            if (isEnabled)
            {
                ddlSpType.Enabled = true;
                txtReferenceNo.ReadOnly = false;
                ddlYear.Enabled = true;
                chkWithCoverLetter.Enabled = true;
                ddlConversionCurrency.Enabled = false;
                txtConversionRate.ReadOnly = true;
                txtClaimDate.ReadOnly = false;
                txtPaymentDateFrom.Enabled = true;
                txtPaymentDateTo.Enabled = true;
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
                //chkForBB.Enabled = false;                
                ddlConversionCurrency.Enabled = false;
                txtConversionRate.ReadOnly = true;
                txtClaimDate.ReadOnly = true;
                txtPaymentDateFrom.Enabled = false;
                txtPaymentDateTo.Enabled = false;
                btnCalculate.Enabled = false;
                btnSelectAll.Enabled = false;
                btnDeselectAll.Enabled = false;
                btnSaveAndPreview.Enabled = false;
            }
        }
        #endregion Utility Methods

        protected void txtConversionRate_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtConversionRate.Text) || Convert.ToDecimal(txtConversionRate.Text) <= 0)
            {
                txtTotalIncomeTax2.Text = "0.00";
                txtTotalLevi2.Text = "0.00";
                txtTotalInterest2.Text = "0.00";
                txtTotalPrincipal2.Text = "0.00";
            }
            else
            {
                txtTotalIncomeTax2.Text = CalculateConvertedAmount(Util.GetDoubleNumber(txtTotalIncomeTax1.Text), Util.GetDoubleNumber(txtConversionRate.Text)).ToString("N2");
                txtTotalLevi2.Text = "0.00";
                txtTotalInterest2.Text = CalculateConvertedAmount(Util.GetDoubleNumber(txtTotalInterest1.Text), Util.GetDoubleNumber(txtConversionRate.Text)).ToString("N2");
                txtTotalPrincipal2.Text = CalculateConvertedAmount(Util.GetDoubleNumber(txtTotalPrincipal1.Text), Util.GetDoubleNumber(txtConversionRate.Text)).ToString("N2");
            }
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Result oResult = null;
            try
            {

                if (string.IsNullOrEmpty(hdnClaimTransNo.Value))
                {
                    ucMessage.OpenMessage("Please select a valid Encashment Claim for delete.", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
                else
                {
                    EncashmentClaimDAL iClaimDAL = new EncashmentClaimDAL();
                    oResult = iClaimDAL.DeleteEncashmentClaim (hdnClaimTransNo.Value);

                    if (oResult.Status)
                    {
                        ucMessage.OpenMessage("Encashment Claim data deleted successfully.", Constants.MSG_APPROVED_SAVE_DATA);
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
        
