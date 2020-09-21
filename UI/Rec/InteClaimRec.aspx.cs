using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
//add
using SBM_BLC1.Common;
using SBM_BLC1.DAL.Reconciliation;
using SBM_BLC1.DAL.Reimbursement;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Entity.Reconciliation;
using SBM_BLC1.Entity.Reimbursement;
using System.Globalization;
using SBM_BLC1.Configuration;
using SBM_BLC1.Entity.Configuration;

namespace SBM_WebUI.mp
{
    public partial class InteClaimRec : System.Web.UI.Page
    {
        #region Local Variable
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_REF_NO = "sRefNo";
        public const string OBJ_PAGE_ID = "sPageID";
        public const string _BASE_CURRENCY = "sBaseCurrency";
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_TRANS.SP_ISSUE))
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
            gvIntClaimDetail.PageSize = (int)Constants.PAGING_UNAPPROVED;
            gvClaim.DataSource = null;
            gvClaim.DataBind();

            gvIntClaimDetail.DataSource = null;
            gvIntClaimDetail.DataBind();

            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            // year
            for (int i = 1995; i < DateTime.Today.Year + 1; i++)
            {
                DDListUtil.Add(ddlYear, i.ToString(), i.ToString());
            }
            DDListUtil.Assign(ddlYear, DateTime.Now.Year);

            //Initial Value Set
            txtReconciliationDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtReimConvRate.Text = "1.0000";
            txtCovRateToBC.Text = "1.00";

            string sRefNo = Request.QueryString[OBJ_REF_NO];
            string sPageID = Request.QueryString[OBJ_PAGE_ID];

            if (!string.IsNullOrEmpty(sRefNo))
            {
                sRefNo = oCrypManager.GetDecryptedString(sRefNo, Constants.CRYPT_PASSWORD_STRING);
            }
            if (!string.IsNullOrEmpty(sPageID))
            {
                sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
            }

            if (!string.IsNullOrEmpty(sRefNo) && !string.IsNullOrEmpty(sPageID))
            {
                string sOperationType = sPageID.Substring(4, 1);
                if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
                {
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    SEARCH_FROM = 2;
                    LoadDataByReferenceNo(sRefNo, "1");//query from Temp Table
                    #region User-Detail.
                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    oUserDetails.CheckerID = oConfig.UserName;
                    oUserDetails.CheckDate = DateTime.Now;
                    ucUserDet.UserDetail = oUserDetails;
                    #endregion User-Detail.
                    fsList.Visible = false;
                    EnableDisableControl(true);
                }
            }
            else
            {
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                UserDetails oUserDetails = new UserDetails();
                oUserDetails.MakerID = oConfig.UserName;
                oUserDetails.MakeDate = DateTime.Now;

                ucUserDet.UserDetail = oUserDetails;

                fsList.Visible = true;
                LoadPreviousList();
                EnableDisableControl(false);
            }   
        }
        protected void txtClaimRefNo_TextChanged(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            bool isRefNoFrmtOk = false;
            string sRefNo = txtClaimRefNo.Text.Trim();
            //string[] aRefNo = sRefNo.Split('/');
            //if (aRefNo.Length.Equals(4))
            //{
            //    isRefNoFrmtOk = true;
            //}
            if (!string.IsNullOrEmpty(ddlSpType.SelectedValue) && !string.IsNullOrEmpty(ddlYear.SelectedValue))
            {
                //sRefNo = ddlSpType.SelectedValue + "/ABC/" + sRefNo + "/" + ddlYear.SelectedValue;
                txtClaimRefNo.Text = sRefNo;
                isRefNoFrmtOk = true;
            }
            if (isRefNoFrmtOk)
            {
                int iIndx = -1;
                InterestReimbursement oIntReim = (InterestReimbursement)Session[Constants.SES_INTE_RECON];
                if (oIntReim != null)
                {
                    DataTable dtClaimDetails = oIntReim.DtInteClaimReimbursement;                    
                    DataRow[] rows = dtClaimDetails.Select("ClaimRefNo='" + txtClaimRefNo.Text + "'");
                    foreach (var vClaimDtl in rows)
                    {
                        iIndx = dtClaimDetails.Rows.IndexOf(vClaimDtl);
                        if ((gvClaim.Rows.Count > iIndx))
                        {
                            GridViewRow gvRow = gvClaim.Rows[iIndx];
                            SetInterestClaimDetails(gvRow);
                        }
                        break;
                    }
                }
                if (iIndx.Equals(-1))
                {
                    InterestReimbursementDAL oInterestReimbursementDAL = new InterestReimbursementDAL();
                    Result oResult = oInterestReimbursementDAL.GetInteClaimStatementByClaimRefNo(txtClaimRefNo.Text.Trim(), oConfig.DivisionID, oConfig.BankCodeID );
                    if (oResult.Status)
                    {
                        DataTable dtIntersetClaim = oResult.Return as DataTable;
                        if (dtIntersetClaim.Rows.Count > 0)
                        {
                            DDListUtil.Assign(ddlSpType, DB.GetDBValue(dtIntersetClaim.Rows[0]["SPTypeID"]));
                            txtClaimDate.Text = (Date.GetDateTimeByString(dtIntersetClaim.Rows[0]["StatementDate"].ToString())).ToString(Constants.DATETIME_FORMAT);
                            txtClaimDateFrom.Text = (Date.GetDateTimeByString(dtIntersetClaim.Rows[0]["FromDate"].ToString())).ToString(Constants.DATETIME_FORMAT);
                            txtClaimDateTo.Text = (Date.GetDateTimeByString(dtIntersetClaim.Rows[0]["ToDate"].ToString())).ToString(Constants.DATETIME_FORMAT);
                            txtClaimCurrncy.Text = DB.GetDBValue(dtIntersetClaim.Rows[0]["CurrencyCode"]);
                            txtClaimCurrncy.Attributes["CurrencyID"] = DB.GetDBValue(dtIntersetClaim.Rows[0]["CurrencyID"]);
                            txtConvRate.Text = DB.GetDBValue(dtIntersetClaim.Rows[0]["ConvRate"]);

                            txtReimConvRate.Text = "1.0000";
                            txtCovRateToBC.Text = "1.0000";

                            txtInterest.Text = DB.GetDBValue(dtIntersetClaim.Rows[0]["InterestAmount"]);
                            txtRemuneration.Text = DB.GetDBValue(dtIntersetClaim.Rows[0]["Remuneration"]);
                            hdnInterestClaimTransNo.Value = DB.GetDBValue(dtIntersetClaim.Rows[0]["InterestClaimTransNo"]);
                        }
                        else
                        {
                            ucMessage.OpenMessage("Invalid reference number !!", Constants.MSG_TYPE_INFO);
                            ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                            ClearInterestClaimSection();
                        }
                    }
                    else
                    {
                        ucMessage.OpenMessage(oResult.Message, Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        ClearInterestClaimSection();
                    }
                }

                btnAdd.Focus();
            }
            else
            {
                ucMessage.OpenMessage("Please select SP Type and Year.", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                ClearInterestClaimSection();
            }
        }
        #endregion InitializeData
        
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            InterestReimbursement oInterestReimbursement = (InterestReimbursement)Session[Constants.SES_INTE_RECON];
            if (oInterestReimbursement == null)
            {
                oInterestReimbursement = new InterestReimbursement();
            }
            DataTable dtClaimDetails = oInterestReimbursement.DtInteClaimReimbursement;
            if (dtClaimDetails.Columns.Count <= 0)
            {
                dtClaimDetails.Columns.Add(new DataColumn("ClaimRefNo", typeof(string)));
                dtClaimDetails.Columns.Add(new DataColumn("ClaimDate", typeof(string)));
                dtClaimDetails.Columns.Add(new DataColumn("ClaimDateFrom", typeof(string)));
                dtClaimDetails.Columns.Add(new DataColumn("ClaimDateTo", typeof(string)));
                dtClaimDetails.Columns.Add(new DataColumn("ClaimAmount", typeof(decimal)));
                //dtClaimDetails.Columns.Add(new DataColumn("IncomeTax", typeof(decimal)));
                dtClaimDetails.Columns.Add(new DataColumn("ReconAmount", typeof(decimal)));
                dtClaimDetails.Columns.Add(new DataColumn("Remuneration", typeof(decimal)));
                //dtClaimDetails.Columns.Add(new DataColumn("ReconIncomeTax", typeof(decimal)));
                //dtClaimDetails.Columns.Add(new DataColumn("ReconRemuneration", typeof(decimal)));                
                //dtClaimDetails.Columns.Add(new DataColumn("Comment", typeof(string)));
                //saving in DB purpose. This is hidden in design
                dtClaimDetails.Columns.Add(new DataColumn("InterestClaimTransNo", typeof(string)));
            }
            DataRow[] rows = dtClaimDetails.Select("ClaimRefNo='" + txtClaimRefNo.Text + "'");

            if (rows.Length <= 0)
            {
                DataRow rowClaimDetails = dtClaimDetails.NewRow();
                rowClaimDetails["ClaimRefNo"] = txtClaimRefNo.Text;
                DateTime parsedDate;
                DateTime.TryParseExact(txtClaimDate.Text, Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                //parsedDate = Convert.ToDateTime(txtClaimDate.Text);
                rowClaimDetails["ClaimDate"] = parsedDate.ToString(Constants.DATETIME_dd_MMM_yyyy);//txtClaimDate.Text;                
                DateTime.TryParseExact(txtClaimDateFrom.Text, Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                //parsedDate = Convert.ToDateTime(txtClaimDateFrom.Text);
                rowClaimDetails["ClaimDateFrom"] = parsedDate.ToString(Constants.DATETIME_dd_MMM_yyyy);//txtClaimDateFrom.Text;
                DateTime.TryParseExact(txtClaimDateTo.Text, Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                //parsedDate = Convert.ToDateTime(txtClaimDateTo.Text);
                rowClaimDetails["ClaimDateTo"] = parsedDate.ToString(Constants.DATETIME_dd_MMM_yyyy);//txtClaimDateTo.Text;
                rowClaimDetails["ClaimAmount"] = txtInterest.Text;
                rowClaimDetails["ReconAmount"] = txtInterest.Text;
                rowClaimDetails["Remuneration"] = txtRemuneration.Text;
                rowClaimDetails["InterestClaimTransNo"] = hdnInterestClaimTransNo.Value;

                dtClaimDetails.Rows.Add(rowClaimDetails);
            }
            else
            {
                int iRowIndex = -1;
                foreach (var vClaimDtl in rows)
                {
                    iRowIndex = dtClaimDetails.Rows.IndexOf(vClaimDtl);
                    break;
                }

                if (iRowIndex != -1)
                {
                    //dtClaimDetails.Rows[iRowIndex]["ReconInterest"] = txtReconInterest.Text;
                    //dtClaimDetails.Rows[iRowIndex]["ReconRemuneration"] = txtReconRemuneration.Text;
                    //dtClaimDetails.Rows[iRowIndex]["Comment"] = txtComment.Text.ToUpper();
                }                
            }

            //Store in Session
            oInterestReimbursement.DtInteClaimReimbursement = dtClaimDetails;
            Session[Constants.SES_INTE_RECON] = oInterestReimbursement;

            gvClaim.DataSource = dtClaimDetails;
            gvClaim.DataBind();

            Populate_PaymentDetails(txtClaimRefNo.Text);

            Calculate_ClaimDetails(oInterestReimbursement);

            ClearInterestClaimSection();
            txtClaimRefNo.Focus();
        }
        private void Calculate_ClaimDetails(InterestReimbursement oIR)
        {

            if (oIR==null)
            { return; }
            //Set Values
            //Total Interest
            object obj = oIR.DtIntePaymentDetails.Compute("SUM(ClaimAmount)", "");
            txtTotInterest.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";
            //Total Recon Interest
            obj = oIR.DtIntePaymentDetails.Compute("SUM(ReconAmount)", "");
            txtTotRecInt.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";
            //Total Remunaration
            obj = oIR.DtInteClaimReimbursement.Compute("SUM(Remuneration)", "");
            txtTotRemuneration.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";
            //Total Recon Remunaration
            //obj = oInterestReimbursement.DtInteClaimReimbursement.Compute("SUM(ReconRemuneration)", "");
            txtTotRecRem.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";
            //variation
            txtTotVariation.Text = (Util.GetDecimalNumber(txtTotInterest.Text) - Util.GetDecimalNumber(txtTotRecInt.Text)).ToString();
            txtBBVariation.Text = (Util.GetDecimalNumber(txtBBAmount.Text) - Util.GetDecimalNumber(txtTotRecInt.Text)).ToString();

        }
        private void Calculate_PaymentDetails(InterestReimbursement oIR,string sClaimRefNo)
        {

            //Set Values
            //NoOfInterestPay
            object obj = oIR.DtIntePaymentDetails.Compute("COUNT(ClaimRefNo)", "ClaimRefNo='" + sClaimRefNo + "'");
            txtNoOfInterestPay.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";
            //Total Interest
            obj = oIR.DtIntePaymentDetails.Compute("SUM(ClaimAmount)", "ClaimRefNo='" + sClaimRefNo + "'");
            txtTotalClaimAmount.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";
            //Total Recon Interest
            obj = oIR.DtIntePaymentDetails.Compute("SUM(ReconAmount)", "ClaimRefNo='" + sClaimRefNo + "'");
            txtTotalReconAmount.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";
            //variation
            txtTotalVariation.Text = (Util.GetDecimalNumber(txtTotalClaimAmount.Text) - Util.GetDecimalNumber(txtTotalReconAmount.Text)).ToString();

        }
        protected void gvClaim_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //get the row
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;            
            //gvRow.BackColor = Color.Blue;

            if (((Button)e.CommandSource).Text.Equals("Select"))
            {
                //SetInterestClaimDetails(gvRow);
                PopulateInterestPaymentDetailsGrid(gvRow);
                //btnAdd.Focus();
            }
            else if (((Button)e.CommandSource).Text.Equals("Remove"))
            {
                InterestReimbursement oInterestReimbursement = (InterestReimbursement)Session[Constants.SES_INTE_RECON];

                if (oInterestReimbursement != null)
                {
                    oInterestReimbursement.DtInteClaimReimbursement.Rows.RemoveAt(gvRow.RowIndex);
                    oInterestReimbursement.DtInteClaimReimbursement.AcceptChanges();

                    gvClaim.DataSource = oInterestReimbursement.DtInteClaimReimbursement;
                    gvClaim.DataBind();

                    //Delete Interest Payment Details
                    Delete_PaymentDetails(gvRow.Cells[2].Text, oInterestReimbursement);
                    gvIntClaimDetail.DataSource = null;

                    //Store in Session                    
                    Session[Constants.SES_INTE_RECON] = oInterestReimbursement;

                    if (gvRow.Cells[2].Text.Equals(txtClaimRefNo.Text.Trim()))
                    {
                        ClearInterestClaimSection();
                    }

                    Calculate_ClaimDetails(oInterestReimbursement);

                    ClearInterestPaymentDetail();
                }

                txtClaimRefNo.Focus();
            }
        }
        private void Delete_PaymentDetails(string sClaimRefNo, InterestReimbursement oIR)
        {
            DataView dv = oIR.DtIntePaymentDetails.DefaultView;
            dv.RowFilter = "ClaimRefNo='" + sClaimRefNo.Trim() + "'";

            for (int i = 0; i < dv.Count; i++)
            {
                dv.Delete(0);
            }
            oIR.DtIntePaymentDetails.AcceptChanges();
            dv.RowFilter = null;
        }
        private void Delete_PaymentDetails(string sClaimRefNo,string sRegNo, InterestReimbursement oIR)
        {
            DataView dv = oIR.DtIntePaymentDetails.DefaultView;
            dv.RowFilter = "ClaimRefNo='" + sClaimRefNo.Trim() + "' And RegNo='" + sRegNo.Trim() + "'";

            for (int i = 0; i < dv.Count; i++)
            {
                dv.Delete(0);
            }
            oIR.DtIntePaymentDetails.AcceptChanges();
            dv.RowFilter = null;
        }
        protected void gvClaim_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //for handling IE exception
        }
        private void PopulateInterestPaymentDetailsGrid(GridViewRow gvRow)
        {
            Populate_PaymentDetails(gvRow.Cells[2].Text);
            
        }
        private void Populate_PaymentDetails(string sClaimRefNo)
        {
            InterestReimbursement oInterestReimbursement = (InterestReimbursement)Session[Constants.SES_INTE_RECON];
            if (oInterestReimbursement == null)
            {
                oInterestReimbursement = new InterestReimbursement();
            }
            DataTable dtPaymentDetails = oInterestReimbursement.DtIntePaymentDetails;
            if (dtPaymentDetails.Columns.Count <= 0)
            {
                dtPaymentDetails.Columns.Add(new DataColumn("ClaimRefNo", typeof(string)));
                dtPaymentDetails.Columns.Add(new DataColumn("RegNo", typeof(string)));
                dtPaymentDetails.Columns.Add(new DataColumn("PaymentDate", typeof(string)));
                dtPaymentDetails.Columns.Add(new DataColumn("ClaimAmount", typeof(decimal)));
                dtPaymentDetails.Columns.Add(new DataColumn("ReconAmount", typeof(decimal)));
                //saving in DB purpose. This is hidden in design
                dtPaymentDetails.Columns.Add(new DataColumn("InterestClaimTransNo", typeof(string)));
                dtPaymentDetails.Columns.Add(new DataColumn("IntPaymentTransNo", typeof(string)));
                dtPaymentDetails.Columns.Add(new DataColumn("Narration", typeof(string)));
                dtPaymentDetails.Columns.Add(new DataColumn("AccountNo", typeof(string)));
                dtPaymentDetails.TableName = "IntPaymentDetails";
            }
            DataRow[] rows = dtPaymentDetails.Select("ClaimRefNo='" + sClaimRefNo + "'");

            if (rows.Length <= 0)
            {

                InterestReimbursementDAL oInterestReimbursementDAL = new InterestReimbursementDAL();
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                Result oResult = oInterestReimbursementDAL.GetIntePaymentDetailsByClaimTransNo(sClaimRefNo, oConfig.DivisionID, oConfig.BankCodeID);
                DataTable dtTmpPayDetail=null;
                if (oResult.Status)
                {
                    dtTmpPayDetail = oResult.Return as DataTable;
                }
                for (int i = 0; i < dtTmpPayDetail.Rows.Count; i++)
                {
                    DataRow r = dtPaymentDetails.NewRow();
                    r["ClaimRefNo"] = dtTmpPayDetail.Rows[i]["ClaimRefNo"];
                    r["RegNo"] = dtTmpPayDetail.Rows[i]["RegNo"];
                    r["PaymentDate"] = Convert.ToDateTime(dtTmpPayDetail.Rows[i]["PaymentDate"]).ToString("dd-MMM-yyyy");
                    r["ClaimAmount"] = dtTmpPayDetail.Rows[i]["ClaimAmount"];
                    r["ReconAmount"] = dtTmpPayDetail.Rows[i]["ReconAmount"];
                    r["InterestClaimTransNo"] = dtTmpPayDetail.Rows[i]["InterestClaimTransNo"];
                    r["IntPaymentTransNo"] = dtTmpPayDetail.Rows[i]["IntPaymentTransNo"];
                    r["Narration"] = dtTmpPayDetail.Rows[i]["Narration"];
                    r["AccountNo"] = dtTmpPayDetail.Rows[i]["AccountNo"];

                    dtPaymentDetails.Rows.Add(r);
                }
            }
            DataView dv = new DataView();
            dv.Table = dtPaymentDetails;
            dv.RowFilter = "ClaimRefNo='" + sClaimRefNo + "'";

            gvIntClaimDetail.DataSource = dv;
            gvIntClaimDetail.DataBind();

            Calculate_PaymentDetails(oInterestReimbursement, sClaimRefNo);


        }
        private void SetInterestClaimDetails(GridViewRow gvRow)
        {
            txtClaimRefNo.Text = gvRow.Cells[2].Text;
            txtClaimDate.Text = Convert.ToDateTime(gvRow.Cells[3].Text).ToString(Constants.DATETIME_FORMAT);
            txtClaimDateFrom.Text = Convert.ToDateTime(gvRow.Cells[4].Text).ToString(Constants.DATETIME_FORMAT);
            txtClaimDateTo.Text = Convert.ToDateTime(gvRow.Cells[5].Text).ToString(Constants.DATETIME_FORMAT);
            txtClaimCurrncy.Text = "BDT";
            txtConvRate.Text = "1.0000";

            txtReimConvRate.Text = "1.0000";
            txtCovRateToBC.Text = "1.0000";

            txtInterest.Text = gvRow.Cells[6].Text;
            //txtReconInterest.Text = gvRow.Cells[7].Text;
            txtRemuneration.Text = gvRow.Cells[7].Text;
            //txtReconRemuneration.Text = gvRow.Cells[9].Text;
            //To avoid assigning HTML value
            //as it assigns '&nbsp;' while reading 'empty' from cell
            //txtComment.Text = System.Web.HttpUtility.HtmlDecode(gvRow.Cells[10].Text);
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
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                SEARCH_FROM = 1;
                LoadDataByReferenceNo(gvRow.Cells[1].Text, "1");//query from Temp Table
            }
        }

        private void LoadPreviousList()
        {
            InterestReimbursementDAL oIntReimburDAL = new InterestReimbursementDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oIntReimburDAL.LoadUnapprovedList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);
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

        private void LoadDataByReferenceNo(string sBBRefNo, string sApprovalStatus)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            InterestReimbursementDAL oInterestReimbursementDAL = new InterestReimbursementDAL();
            Result oResult = oInterestReimbursementDAL.GetInteClaimReimburseStatementByBBRefNo(sBBRefNo, sApprovalStatus, oConfig.DivisionID, oConfig.BankCodeID);
            if (oResult.Status)
            {
                SetObject(oResult.Return as InterestReimbursement);
            }
        }
        
        private void SetObject(InterestReimbursement oInterestReimbursement)
        {
            Session[Constants.SES_INTE_RECON] = oInterestReimbursement;

            if (oInterestReimbursement != null)
            {
                ddlSpType.Text = oInterestReimbursement.SPType.SPTypeID;
                ddlYear.Text = oInterestReimbursement.ReimburseDate.Year.ToString();
                txtBBReferenceNo.Text = oInterestReimbursement.InterestReimburseReferenceNo;
                LoadCurrencyBySPType(oInterestReimbursement.SPType.SPTypeID);
                ddlReconCurrency.Text = oInterestReimbursement.Currency.CurrencyID;
                txtReimConvRate.Text = Convert.ToString(oInterestReimbursement.ConvRate);
                txtCovRateToBC.Text = Convert.ToString(oInterestReimbursement.ConvRateToBC);
                txtBBAmount.Text = Convert.ToString(oInterestReimbursement.BBAmount);
                //Set user details
                if (SEARCH_FROM.Equals(1))//if viewed from Temp By Maker
                {
                    ucUserDet.UserDetail = oInterestReimbursement.UserDetails;
                }
                else if (SEARCH_FROM.Equals(2))//if viewed from Temp By Checker
                {
                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    oUserDetails.MakeDate = oInterestReimbursement.UserDetails.MakeDate;
                    oUserDetails.MakerID = oInterestReimbursement.UserDetails.MakerID;
                    ucUserDet.UserDetail = oUserDetails;
                }

                gvClaim.DataSource = oInterestReimbursement.DtInteClaimReimbursement;
                gvClaim.DataBind();

                //txtTotRecInt.Text = Convert.ToString(oInterestReimbursement.ClaimAmount);                
                //txtTotRecRem.Text = Convert.ToString(oInterestReimbursement.Remuneration);

                ////From InterestClaim 
                //object obj = oInterestReimbursement.DtInteClaimReimbursement.Compute("SUM(Interest)", "");
                //txtTotInterest.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0";
                ////From InterestClaim
                //obj = oInterestReimbursement.DtInteClaimReimbursement.Compute("SUM(Remuneration)", "");
                //txtTotRemuneration.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0";

                Calculate_ClaimDetails(oInterestReimbursement);

                //txtTotVariation.Text = (Util.GetDecimalNumber(txtTotInterest.Text) - oInterestReimbursement.InterestAmount).ToString();
                hdnInterestReimburseTransNo.Value = oInterestReimbursement.InterestReimburseTransNo;
                //Set a background color as Selected
                //gvClaim.Rows[0].BackColor = Color.Blue;
                //SetClaimSatementDetailSectionValue(oSalesStatementReconciled.DtClaimDetails);

                if (gvClaim.Rows.Count > 0)
                {
                    PopulateInterestPaymentDetailsGrid(gvClaim.Rows[0]);
                }
            }
        }

        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, null);
        }

        private void EnableDisableControl(bool isApproved)
        {
            // general Control
            if (isApproved)
            {
                //BB Details
                Util.ControlEnabled(ddlSpType, false);
                Util.ControlEnabled(ddlYear, false);
                Util.ControlEnabled(ddlReconCurrency, false);
                Util.ControlEnabled(txtBBReferenceNo, false);                
                Util.ControlEnabled(txtCovRateToBC, false);

                // Interest Claim Detail
                Util.ControlEnabled(txtClaimRefNo, false);                
                //Util.ControlEnabled(txtReconInterest, false);                
                //Util.ControlEnabled(txtRemuneration, false);
                //Util.ControlEnabled(txtReconRemuneration, false);
                //Util.ControlEnabled(txtComment, false);

                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);

                // Button
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true);

                Util.ControlEnabled(btnRestClaim, false);
                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);
                Util.ControlEnabled(btnAdd, false);
                
                //Disable Grid's column
                gvClaim.Columns[1].Visible = false;

                Util.ControlEnabled(btnUpdate, false);
                Util.ControlEnabled(btnResetDetails, false);

                //Disable Grid's column
                gvIntClaimDetail.Columns[0].Visible = false;
                gvIntClaimDetail.Columns[1].Visible = false;


                //fsList.Visible = false;
            }
            else
            {
                //BB Details
                Util.ControlEnabled(ddlSpType, true);
                Util.ControlEnabled(ddlYear, true);
                Util.ControlEnabled(ddlReconCurrency, true);
                Util.ControlEnabled(txtBBReferenceNo, true);
                Util.ControlEnabled(txtConvRate, true);
                Util.ControlEnabled(txtCovRateToBC, true);

                // Interest Claim Detail 
                Util.ControlEnabled(txtClaimRefNo, true);                
                //Util.ControlEnabled(txtReconInterest, true);                
                Util.ControlEnabled(txtRemuneration, true);
                //Util.ControlEnabled(txtReconRemuneration, true);
                //Util.ControlEnabled(txtComment, true);

                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), false);
                // Button 
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);

                Util.ControlEnabled(btnRestClaim, true);
                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);

                fsList.Visible = true;
            }
        }

        protected void ClearInterestClaimSection()
        {
            txtClaimRefNo.Text = string.Empty;
            txtClaimDate.Text = string.Empty;
            txtClaimDateFrom.Text = string.Empty;
            txtClaimDateTo.Text = string.Empty;
            txtClaimCurrncy.Text = string.Empty;
            txtConvRate.Text = string.Empty;

            txtReimConvRate.Text = "1.0000";
            txtCovRateToBC.Text = "1.0000";

            txtInterest.Text = "0.00";           
            //txtReconInterest.Text = "0.00";
            txtRemuneration.Text = "0.00";
            //txtReconRemuneration.Text = "0.00";
            //txtComment.Text = string.Empty;
        }
        protected void ClearInterestPaymentDetailSection()
        {
            txtRegNo.Text = "";
            txtPaymentDate.Text = "";
            txtClaimAmount.Text = "";
            txtReconAmount.Text = "";
            hdnIntPayTransNo.Value = "";
            hdnClaimRefNo.Value = "";
        }
        protected void ClearInterestPaymentDetail()
        {
            gvIntClaimDetail.DataSource = null;
            gvIntClaimDetail.DataBind();
            txtNoOfInterestPay.Text = string.Empty;
            txtTotalClaimAmount.Text = string.Empty;
            txtTotalReconAmount.Text = string.Empty;            
        }

        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");

            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_REC_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_RECON.INTEREST).PadLeft(5, '0'), false);
            }
            else if (sType.Equals(Constants.BTN_SAVE))
            {
                ddlSpType.Focus();
            }
            else
            {
                txtClaimRefNo.Focus();
            }
        }

        private void TotalClear()
        {
            Session[Constants.SES_INTE_RECON] = null;
            //GridView controlls 
            gvClaim.DataSource = null;
            gvClaim.DataBind();
            
            ddlSpType.SelectedIndex = 0;
            ddlYear.Text = DateTime.Now.Year.ToString();
            ddlReconCurrency.Items.Clear();

            //TextBox controls of Recon
            txtBBReferenceNo.Text = string.Empty;
            txtReconciliationDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtReimConvRate.Text = "1.0000";
            txtCovRateToBC.Text = "1.0000";
            txtTotInterest.Text = string.Empty;
            txtTotRecInt.Text = string.Empty;
            txtTotRemuneration.Text = string.Empty;
            txtTotRecRem.Text = string.Empty;
            txtTotVariation.Text = string.Empty;

            txtBBAmount.Text = "0.00";

            ClearInterestClaimSection();
            ClearInterestPaymentDetail();
            ucUserDet.ResetData();
            hdnInterestReimburseTransNo.Value = string.Empty;
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBBReferenceNo.Text))
            {
                InterestReimbursement oIntReim = new InterestReimbursement();
                oIntReim.InterestReimburseReferenceNo = txtBBReferenceNo.Text;
                InterestReimbursementDAL oIntReimDAL = new InterestReimbursementDAL();
                oIntReim.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oIntReimDAL.Reject(oIntReim);
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

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBBReferenceNo.Text))
            {
                InterestReimbursement oIntReim = Session[Constants.SES_INTE_RECON] as InterestReimbursement;
                InterestReimbursementDAL oInterestReimbursementDAL = new InterestReimbursementDAL();                
                
                //get User Details
                oIntReim.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oInterestReimbursementDAL.Approve(oIntReim);
                if (oResult.Status)
                {
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

        protected void btnReset_Click(object sender, EventArgs e)
        {
            //EnableDisableControl(false);
            TotalClear();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            //if (!hdDataType.Value.Equals("2"))
            //{
            if (!string.IsNullOrEmpty(hdnInterestReimburseTransNo.Value))
            {
                InterestReimbursementDAL oIntReimDAL = new InterestReimbursementDAL();
                Result oResult = (Result)oIntReimDAL.Detete(hdnInterestReimburseTransNo.Value);
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
            //}
            //else
            //{
            //    ucMessage.OpenMessage(Constants.MSG_ERROR_DELETE, Constants.MSG_TYPE_ERROR);
            //}
            //}
            //else
            //{
            //    ucMessage.OpenMessage(Constants.MSG_APPROVED_DELETE_DATA, Constants.MSG_TYPE_INFO);
            //    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            //}
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            InterestReimbursement oInterestReimbursement = (InterestReimbursement)Session[Constants.SES_INTE_RECON];
            if (oInterestReimbursement != null)
            {
                oInterestReimbursement.InterestReimburseTransNo = hdnInterestReimburseTransNo.Value.Equals("") ? "-1" : hdnInterestReimburseTransNo.Value;
                oInterestReimbursement.InterestReimburseReferenceNo = txtBBReferenceNo.Text.Trim().ToUpper();
                oInterestReimbursement.SPType.SPTypeID = ddlSpType.SelectedValue;
                oInterestReimbursement.ReimburseDate = DateTime.Now;
                oInterestReimbursement.Currency.CurrencyID = ddlReconCurrency.SelectedValue;
                oInterestReimbursement.ConvRate = Util.GetDecimalNumber(txtReimConvRate.Text);
                oInterestReimbursement.ConvRateToBC = Util.GetDecimalNumber(txtCovRateToBC.Text);
                oInterestReimbursement.ClaimAmount = Util.GetDecimalNumber(txtTotInterest.Text);
                oInterestReimbursement.ReconAmount = Util.GetDecimalNumber(txtTotRecInt.Text);                
                oInterestReimbursement.Remuneration = Util.GetDecimalNumber(txtTotRemuneration.Text);
                oInterestReimbursement.BBAmount = Util.GetDecimalNumber(txtBBAmount.Text);

                decimal dIR = 0;
                decimal dDR = 0;
                object obj = oInterestReimbursement.DtInteClaimReimbursement.Compute("SUM(ReconAmount)", "");
                dIR = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj) : 0;

                obj = 0;
                obj = oInterestReimbursement.DtIntePaymentDetails.Compute("SUM(ReconAmount)", "");
                dDR = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj) : 0;

                if (dIR!=dDR)
                {
                    ucMessage.OpenMessage("Claim recon details and payment recon details mismatched. May have manual journal. Please check..", Constants.MSG_TYPE_ERROR);
                    return;
                }

                oInterestReimbursement.UserDetails = ucUserDet.UserDetail;
                InterestReimbursementDAL oInterestReimbursementDAL = new InterestReimbursementDAL();
                Result oResult = oInterestReimbursementDAL.Save(oInterestReimbursement);
                if (oResult.Status)
                {                    
                    LoadPreviousList();
                    TotalClear();
                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                    if (oResult.Message.Equals("A"))
                    {
                        ucMessage.OpenMessage("This BB Reference No. already used. Please check..", Constants.MSG_TYPE_ERROR);
                    }
                    else
                    {
                        ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
                    }
                }
            }
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_REC_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_RECON.INTEREST).PadLeft(5, '0'), false);
        }

        protected void ddlReconCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sBaseCurrency = ViewState[_BASE_CURRENCY] as string;
            if (!string.IsNullOrEmpty(sBaseCurrency))
            {
                if (sBaseCurrency.Equals(ddlReconCurrency.SelectedValue))
                {
                    txtReimConvRate.Enabled = false;
                    txtReimConvRate.Text = "1.0000";
                }
                else
                {
                    txtReimConvRate.Enabled = true;
                    txtReimConvRate.Text = "1.0000";
                }
            }

            txtClaimRefNo.Focus();
        }

        private void LoadCurrencyBySPType(string sSPTypeID)
        {
            ddlReconCurrency.Items.Clear();
            SPPolicyDAL oSPPolicyDAL = new SPPolicyDAL();
            Result oResult = oSPPolicyDAL.GetCurrencyList(sSPTypeID, Constants.ACTIVITY_TYPE.INTEREST_REIMBURSE, DateTime.Now);
            if (oResult.Status)
            {
                DDListUtil.Assign(ddlReconCurrency, (DataTable)oResult.Return, true);
            }
        }

        protected void ddlSpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sSPTypeID = ddlSpType.SelectedValue;
            TotalClear();
            if (!string.IsNullOrEmpty(sSPTypeID))
            {
                ddlSpType.Text = sSPTypeID;
                LoadCurrencyBySPType(ddlSpType.SelectedValue);
                SPTypeDAL oSPTypeDAL = new SPTypeDAL();
                Result oResult = oSPTypeDAL.GetCurrencyBySPTypeID(ddlSpType.SelectedValue);
                if (oResult.Status)
                {
                    SPType oSPType = oResult.Return as SPType;
                    ViewState[_BASE_CURRENCY] = oSPType.Currency.CurrencyID;
                }
                if (ddlSpType.SelectedValue.Equals(Constants.SP_TYPE_DIB)
                   || ddlSpType.SelectedValue.Equals(Constants.SP_TYPE_DPB))
                {
                    txtCovRateToBC.Enabled = true;
                    txtCovRateToBC.Text = "1.0000";
                }
                else
                {
                    txtCovRateToBC.Text = "1.0000";
                    txtCovRateToBC.Enabled = false;
                }
            }

            //txtReimConvRate.Text = string.Empty;
            txtReimConvRate.Enabled = true;
            txtBBReferenceNo.Focus();
        }

        protected void btnRestClaim_Click(object sender, EventArgs e)
        {
            ClearInterestClaimSection();
            this.txtClaimRefNo.Focus();
        }

        protected void gvIntClaimDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //get the row
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            //gvRow.BackColor = Color.Blue;

            if (((Button)e.CommandSource).Text.Equals("Select"))
            {
                //SetInterestClaimDetails(gvRow);
                InterestReimbursement oInterestReimbursement = (InterestReimbursement)Session[Constants.SES_INTE_RECON];

                DataView dv = new DataView();
                dv.Table = oInterestReimbursement.DtIntePaymentDetails;
                dv.RowFilter = "ClaimRefNo='" + gvRow.Cells[2].Text.Trim() + "'";

                txtRegNo.Text = dv[gvRow.RowIndex]["RegNo"].ToString();
                txtPaymentDate.Text = dv[gvRow.RowIndex]["PaymentDate"].ToString();
                txtClaimAmount.Text = dv[gvRow.RowIndex]["ClaimAmount"].ToString();
                txtReconAmount.Text = dv[gvRow.RowIndex]["ReconAmount"].ToString();
                txtIntClaimTransNo.Text = dv[gvRow.RowIndex]["InterestClaimTransNo"].ToString();
                txtIntTransNo.Text = dv[gvRow.RowIndex]["IntPaymentTransNo"].ToString();
                txtCRefNo.Text = dv[gvRow.RowIndex]["ClaimRefNo"].ToString();
                txtReconAmount.Focus();
                dv.RowFilter = null;
                //btnAdd.Focus();
               
            }
            else if (((Button)e.CommandSource).Text.Equals("Remove"))
            {
                InterestReimbursement oInterestReimbursement = (InterestReimbursement)Session[Constants.SES_INTE_RECON];

                if (oInterestReimbursement != null)
                {
                    //Delete Interest Payment Details
                    Delete_PaymentDetails(gvRow.Cells[2].Text, gvRow.Cells[3].Text, oInterestReimbursement);
                    Populate_PaymentDetails(gvRow.Cells[2].Text);

                    DataView dv1 = oInterestReimbursement.DtInteClaimReimbursement.DefaultView;
                    dv1.RowFilter = "ClaimRefNo='" + gvRow.Cells[2].Text + "'";
                    if (dv1.Count > 0)
                    {
                        object obj = oInterestReimbursement.DtIntePaymentDetails.Compute("SUM(ReconAmount)", "ClaimRefNo='" + gvRow.Cells[2].Text + "'");
                        dv1[0]["ReconAmount"] = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";
                        dv1.RowFilter = null;
                    }
                    //Store in Session                    
                    Session[Constants.SES_INTE_RECON] = oInterestReimbursement;
                    Calculate_ClaimDetails(oInterestReimbursement);

                    ClearInterestPaymentDetailSection();
                }

                txtClaimRefNo.Focus();
            }
        }

        protected void btnResetDetails_Click(object sender, EventArgs e)
        {
            ClearInterestPaymentDetailSection();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            InterestReimbursement oInterestReimbursement = (InterestReimbursement)Session[Constants.SES_INTE_RECON];
            try
            {
                DataView dv = oInterestReimbursement.DtIntePaymentDetails.DefaultView;
                dv.RowFilter = "IntPaymentTransNo='" + txtIntTransNo.Text   + "' AND InterestClaimTransNo='" + txtIntClaimTransNo.Text + "'";
                if (dv.Count > 0)
                {
                    dv[0]["ReconAmount"] = Convert.ToDecimal(txtReconAmount.Text).ToString("N2");
                    dv.RowFilter = null;
                }
                oInterestReimbursement.DtIntePaymentDetails.AcceptChanges();


                Populate_PaymentDetails(txtCRefNo.Text);
                Calculate_PaymentDetails(oInterestReimbursement, txtCRefNo.Text  );
                DataView dv1 = oInterestReimbursement.DtInteClaimReimbursement.DefaultView;
                dv1.RowFilter = "ClaimRefNo='" + txtCRefNo.Text + "'";
                if (dv1.Count > 0)
                {
                    object obj = oInterestReimbursement.DtIntePaymentDetails.Compute("SUM(ReconAmount)", "ClaimRefNo='" + txtCRefNo.Text + "'");
                    dv1[0]["ReconAmount"] = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";
                    dv1.RowFilter = null;
                }
                Calculate_ClaimDetails(oInterestReimbursement);
                oInterestReimbursement.DtIntePaymentDetails.AcceptChanges();
                ClearInterestPaymentDetailSection();
            }
            catch (Exception ex)
            {
                ucMessage.OpenMessage(ex.Message, Constants.MSG_TYPE_ERROR);                
            }
        }

        protected void txtBBAmount_TextChanged(object sender, EventArgs e)
        {
            InterestReimbursement oInterestReimbursement = (InterestReimbursement)Session[Constants.SES_INTE_RECON];
            Calculate_ClaimDetails(oInterestReimbursement);
        }

    }
}
