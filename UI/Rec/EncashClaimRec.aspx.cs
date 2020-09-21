//Jakir 20120913
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
    public partial class EncashClaimRec : System.Web.UI.Page
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
            txtTotRecRem.Text = "0.00";
            txtBBAmount.Text = "0.00";

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
                EncashmentReimbursement oIntReim = (EncashmentReimbursement)Session[Constants.SES_INTE_RECON];
                if (oIntReim != null)
                {
                    DataTable dtClaimDetails = oIntReim.DtEncsClaimReimbursement;
                    DataRow[] rows = dtClaimDetails.Select("ClaimRefNo='" + txtClaimRefNo.Text + "'");
                    foreach (var vClaimDtl in rows)
                    {
                        //iIndx = dtClaimDetails.Rows.IndexOf(vClaimDtl);
                        //if ((gvClaim.Rows.Count > iIndx))
                        //{
                        //    GridViewRow gvRow = gvClaim.Rows[iIndx];
                        //    SetEncashmentClaimDetails(gvRow);
                        //}
                        break;
                    }
                }
                if (iIndx.Equals(-1))
                {
                    EncashmentReimbursementDAL oEncashmentReimbursementDAL = new EncashmentReimbursementDAL();
                    Result oResult = oEncashmentReimbursementDAL.GetEncsClaimStatementByClaimRefNo(txtClaimRefNo.Text.Trim(), oConfig.DivisionID, oConfig.BankCodeID);
                    if (oResult.Status)
                    {
                        DataTable DtEncsrsetClaim = oResult.Return as DataTable;
                        //if (DtEncsrsetClaim.Columns.Count.Equals(1))
                        //{
                        //    ucMessage.OpenMessage("This reference number already reconciled !!", Constants.MSG_TYPE_INFO);
                        //    ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        //    ClearEncashmentClaimSection();
                        //}
                        //else 
                        if (DtEncsrsetClaim.Rows.Count > 0)
                        {
                            DDListUtil.Assign(ddlSpType, DB.GetDBValue(DtEncsrsetClaim.Rows[0]["SPTypeID"]));
                            txtClaimDate.Text = (Date.GetDateTimeByString(DtEncsrsetClaim.Rows[0]["StatementDate"].ToString())).ToString(Constants.DATETIME_FORMAT);
                            txtClaimDateFrom.Text = (Date.GetDateTimeByString(DtEncsrsetClaim.Rows[0]["FromDate"].ToString())).ToString(Constants.DATETIME_FORMAT);
                            txtClaimDateTo.Text = (Date.GetDateTimeByString(DtEncsrsetClaim.Rows[0]["ToDate"].ToString())).ToString(Constants.DATETIME_FORMAT);
                            txtClaimCurrncy.Text = DB.GetDBValue(DtEncsrsetClaim.Rows[0]["CurrencyCode"]);
                            txtClaimCurrncy.Attributes["CurrencyID"] = DB.GetDBValue(DtEncsrsetClaim.Rows[0]["CurrencyID"]);
                            txtConvRate.Text = DB.GetDBValue(DtEncsrsetClaim.Rows[0]["ConvRate"]);

                            txtReimConvRate.Text = "1.0000";
                            txtCovRateToBC.Text = "1.0000";

                            txtEncashment.Text = DB.GetDBValue(DtEncsrsetClaim.Rows[0]["EncashmentAmount"]);
                            txtRemuneration.Text = DB.GetDBValue(DtEncsrsetClaim.Rows[0]["Remuneration"]);
                            hdnEncashmentClaimTransNo.Value = DB.GetDBValue(DtEncsrsetClaim.Rows[0]["EncashmentClaimTransNo"]);
                        }
                        else
                        {
                            ucMessage.OpenMessage("Invalid reference number !!", Constants.MSG_TYPE_INFO);
                            ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                            ClearEncashmentClaimSection();
                        }
                    }
                    else
                    {
                        ucMessage.OpenMessage(oResult.Message, Constants.MSG_TYPE_INFO);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        ClearEncashmentClaimSection();
                    }
                }

                btnAdd.Focus();
            }
            else
            {
                ucMessage.OpenMessage("Please select SP Type and Year.", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                ClearEncashmentClaimSection();
            }
        }
        #endregion InitializeData

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            EncashmentReimbursement oEncashmentReimbursement = (EncashmentReimbursement)Session[Constants.SES_INTE_RECON];
            if (oEncashmentReimbursement == null)
            {
                oEncashmentReimbursement = new EncashmentReimbursement();
            }
            DataTable dtClaimDetails = oEncashmentReimbursement.DtEncsClaimReimbursement;
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
                dtClaimDetails.Columns.Add(new DataColumn("EncashmentClaimTransNo", typeof(string)));
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
                rowClaimDetails["ClaimAmount"] = txtEncashment.Text;
                rowClaimDetails["ReconAmount"] = txtEncashment.Text;
                rowClaimDetails["Remuneration"] = txtRemuneration.Text;
                rowClaimDetails["EncashmentClaimTransNo"] = hdnEncashmentClaimTransNo.Value;

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
                    //dtClaimDetails.Rows[iRowIndex]["ReconEncashment"] = txtReconEncashment.Text;
                    //dtClaimDetails.Rows[iRowIndex]["ReconRemuneration"] = txtReconRemuneration.Text;
                    //dtClaimDetails.Rows[iRowIndex]["Comment"] = txtComment.Text.ToUpper();
                }
            }

            //Store in Session
            oEncashmentReimbursement.DtEncsClaimReimbursement = dtClaimDetails;
            Session[Constants.SES_INTE_RECON] = oEncashmentReimbursement;

            gvClaim.DataSource = dtClaimDetails;
            gvClaim.DataBind();

            Populate_PaymentDetails(txtClaimRefNo.Text);

            Calculate_ClaimDetails(oEncashmentReimbursement);

            ClearEncashmentClaimSection();
            txtClaimRefNo.Focus();
        }
        private void Calculate_ClaimDetails(EncashmentReimbursement oIR)
        {
            if (oIR == null)
            {
                return;
            }
            //Set Values
            //Total Encashment
            object obj = oIR.DtEncsPaymentDetails.Compute("SUM(ClaimAmount)", "");
            txtTotEncashment.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";
            //Total Recon Encashment
            obj = oIR.DtEncsPaymentDetails.Compute("SUM(ReconAmount)", "");
            txtTotRecInt.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";
            //Total Remunaration
            obj = oIR.DtEncsClaimReimbursement.Compute("SUM(Remuneration)", "");
            txtTotRemuneration.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";

            if (txtTotRecRem.Text.Trim() == "")
            {
                txtTotRecRem.Text = "0.00";
            }

            //Total Recon Remunaration
            //obj = oEncashmentReimbursement.DtEncsClaimReimbursement.Compute("SUM(ReconRemuneration)", "");
            //txtTotRecRem.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";
            //variation
            txtTotVariation.Text = (Util.GetDecimalNumber(txtTotRecInt.Text)-Util.GetDecimalNumber(txtTotEncashment.Text) ).ToString();
            txtTotReconVariation.Text = (Util.GetDecimalNumber(txtTotRecRem.Text) - Util.GetDecimalNumber(txtTotRemuneration.Text)).ToString();
            txtBBVariation.Text = (Util.GetDecimalNumber(txtBBAmount.Text) - (Util.GetDecimalNumber(txtTotRecInt.Text) + Util.GetDecimalNumber(txtTotRecRem.Text))).ToString();
        }
        private void Calculate_PaymentDetails(EncashmentReimbursement oIR, string sClaimRefNo)
        {

            //Set Values
            //NoOfEncashmentPay
            object obj = oIR.DtEncsClaimReimbursement.Compute("COUNT(ClaimRefNo)", "ClaimRefNo='" + sClaimRefNo + "'");
            txtNoOfEncashmentPay.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";
            //Total Encashment
            obj = oIR.DtEncsPaymentDetails.Compute("SUM(ClaimAmount)", "ClaimRefNo='" + sClaimRefNo + "'");
            txtTotalClaimAmount.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";
            //Total Recon Encashment
            obj = oIR.DtEncsPaymentDetails.Compute("SUM(ReconAmount)", "ClaimRefNo='" + sClaimRefNo + "'");
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
                //SetEncashmentClaimDetails(gvRow);
                PopulateEncashmentPaymentDetailsGrid(gvRow);
                //btnAdd.Focus();
            }
            else if (((Button)e.CommandSource).Text.Equals("Remove"))
            {
                EncashmentReimbursement oEncashmentReimbursement = (EncashmentReimbursement)Session[Constants.SES_INTE_RECON];

                if (oEncashmentReimbursement != null)
                {
                    oEncashmentReimbursement.DtEncsClaimReimbursement.Rows.RemoveAt(gvRow.RowIndex);
                    oEncashmentReimbursement.DtEncsClaimReimbursement.AcceptChanges();

                    gvClaim.DataSource = oEncashmentReimbursement.DtEncsClaimReimbursement;
                    gvClaim.DataBind();

                    //Delete Encashment Payment Details
                    Delete_PaymentDetails(gvRow.Cells[2].Text, oEncashmentReimbursement);
                    gvIntClaimDetail.DataSource = null;

                    //Store in Session                    
                    Session[Constants.SES_INTE_RECON] = oEncashmentReimbursement;

                    if (gvRow.Cells[2].Text.Equals(txtClaimRefNo.Text.Trim()))
                    {
                        ClearEncashmentClaimSection();
                    }

                    Calculate_ClaimDetails(oEncashmentReimbursement);

                    ClearEncashmentPaymentDetail();
                }

                txtClaimRefNo.Focus();
            }
        }
        private void Delete_PaymentDetails(string sClaimRefNo, EncashmentReimbursement oIR)
        {
            DataView dv = oIR.DtEncsPaymentDetails.DefaultView;
            dv.RowFilter = "ClaimRefNo='" + sClaimRefNo.Trim() + "'";

            for (int i = 0; i < dv.Count; i++)
            {
                dv.Delete(0);
            }
            oIR.DtEncsPaymentDetails.AcceptChanges();
            dv.RowFilter = null;
        }
        private void Delete_PaymentDetails(string sClaimRefNo, string sRegNo, EncashmentReimbursement oIR)
        {
            DataView dv = oIR.DtEncsPaymentDetails.DefaultView;
            dv.RowFilter = "ClaimRefNo='" + sClaimRefNo.Trim() + "' And RegNo='" + sRegNo.Trim() + "'";

            for (int i = 0; i < dv.Count; i++)
            {
                dv.Delete(0);
            }
            oIR.DtEncsPaymentDetails.AcceptChanges();
            dv.RowFilter = null;
        }
        protected void gvClaim_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //for handling IE exception
        }
        private void PopulateEncashmentPaymentDetailsGrid(GridViewRow gvRow)
        {
            Populate_PaymentDetails(gvRow.Cells[2].Text);

        }
        private void Populate_PaymentDetails(string sClaimRefNo)
        {
            EncashmentReimbursement oEncashmentReimbursement = (EncashmentReimbursement)Session[Constants.SES_INTE_RECON];
            if (oEncashmentReimbursement == null)
            {
                oEncashmentReimbursement = new EncashmentReimbursement();
            }
            DataTable dtPaymentDetails = oEncashmentReimbursement.DtEncsPaymentDetails;
            if (dtPaymentDetails.Columns.Count <= 0)
            {
                dtPaymentDetails.Columns.Add(new DataColumn("ClaimRefNo", typeof(string)));
                dtPaymentDetails.Columns.Add(new DataColumn("RegNo", typeof(string)));
                dtPaymentDetails.Columns.Add(new DataColumn("PaymentDate", typeof(string)));
                dtPaymentDetails.Columns.Add(new DataColumn("ClaimAmount", typeof(decimal)));
                dtPaymentDetails.Columns.Add(new DataColumn("ReconAmount", typeof(decimal)));
                //saving in DB purpose. This is hidden in design
                dtPaymentDetails.Columns.Add(new DataColumn("EncashmentClaimTransNo", typeof(string)));
                dtPaymentDetails.Columns.Add(new DataColumn("EncashmentTransNo", typeof(string)));
                dtPaymentDetails.Columns.Add(new DataColumn("Narration", typeof(string)));
                dtPaymentDetails.Columns.Add(new DataColumn("AccountNo", typeof(string)));
                dtPaymentDetails.TableName = "EncPaymentDetails";
            }
            DataRow[] rows = dtPaymentDetails.Select("ClaimRefNo='" + sClaimRefNo + "'");

            if (rows.Length <= 0)
            {

                EncashmentReimbursementDAL oEncashmentReimbursementDAL = new EncashmentReimbursementDAL();
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                Result oResult = oEncashmentReimbursementDAL.GetEncsPaymentDetailsByClaimTransNo(sClaimRefNo, oConfig.DivisionID, oConfig.BankCodeID);
                DataTable dtTmpPayDetail = null;
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
                    r["EncashmentClaimTransNo"] = dtTmpPayDetail.Rows[i]["EncashmentClaimTransNo"];
                    r["EncashmentTransNo"] = dtTmpPayDetail.Rows[i]["EncashmentTransNo"];
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

            Calculate_PaymentDetails(oEncashmentReimbursement, sClaimRefNo);


        }
        private void SetEncashmentClaimDetails(GridViewRow gvRow)
        {
            txtClaimRefNo.Text = gvRow.Cells[2].Text;
            txtClaimDate.Text = Convert.ToDateTime(gvRow.Cells[3].Text).ToString(Constants.DATETIME_FORMAT);
            txtClaimDateFrom.Text = Convert.ToDateTime(gvRow.Cells[4].Text).ToString(Constants.DATETIME_FORMAT);
            txtClaimDateTo.Text = Convert.ToDateTime(gvRow.Cells[5].Text).ToString(Constants.DATETIME_FORMAT);
            txtClaimCurrncy.Text = "BDT";
            txtConvRate.Text = "1.0000";

            txtReimConvRate.Text = "1.0000";
            txtCovRateToBC.Text = "1.0000";

            txtEncashment.Text = gvRow.Cells[6].Text;
            //txtReconEncashment.Text = gvRow.Cells[7].Text;
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
            EncashmentReimbursementDAL oIntReimburDAL = new EncashmentReimbursementDAL();
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
            EncashmentReimbursementDAL oEncashmentReimbursementDAL = new EncashmentReimbursementDAL();
            Result oResult = oEncashmentReimbursementDAL.GetEncsClaimReimburseStatementByBBRefNo(sBBRefNo, sApprovalStatus, oConfig.DivisionID, oConfig.BankCodeID);
            if (oResult.Status)
            {
                SetObject(oResult.Return as EncashmentReimbursement);
            }
        }

        private void SetObject(EncashmentReimbursement oEncashmentReimbursement)
        {
            Session[Constants.SES_INTE_RECON] = oEncashmentReimbursement;

            if (oEncashmentReimbursement != null)
            {
                ddlSpType.Text = oEncashmentReimbursement.SPType.SPTypeID;
                ddlYear.Text = oEncashmentReimbursement.ReimburseDate.Year.ToString();
                txtBBReferenceNo.Text = oEncashmentReimbursement.EncashmentReimburseReferenceNo;
                LoadCurrencyBySPType(oEncashmentReimbursement.SPType.SPTypeID);
                ddlReconCurrency.Text = oEncashmentReimbursement.Currency.CurrencyID;
                txtReimConvRate.Text = Convert.ToString(oEncashmentReimbursement.ConvRate);
                txtCovRateToBC.Text = Convert.ToString(oEncashmentReimbursement.ConvRateToBC);
                txtBBAmount.Text = Convert.ToString(oEncashmentReimbursement.BBAmount);
                //Set user details
                if (SEARCH_FROM.Equals(1))//if viewed from Temp By Maker
                {
                    ucUserDet.UserDetail = oEncashmentReimbursement.UserDetails;
                }
                else if (SEARCH_FROM.Equals(2))//if viewed from Temp By Checker
                {
                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    oUserDetails.MakeDate = oEncashmentReimbursement.UserDetails.MakeDate;
                    oUserDetails.MakerID = oEncashmentReimbursement.UserDetails.MakerID;
                    ucUserDet.UserDetail = oUserDetails;
                }

                gvClaim.DataSource = oEncashmentReimbursement.DtEncsClaimReimbursement;
                gvClaim.DataBind();

                //txtTotRecInt.Text = Convert.ToString(oEncashmentReimbursement.ClaimAmount);                
                txtTotRecRem.Text = Convert.ToString(oEncashmentReimbursement.Remuneration);

                ////From EncashmentClaim 
                //object obj = oEncashmentReimbursement.DtEncsClaimReimbursement.Compute("SUM(Encashment)", "");
                //txtTotEncashment.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0";
                ////From EncashmentClaim
                //obj = oEncashmentReimbursement.DtEncsClaimReimbursement.Compute("SUM(Remuneration)", "");
                //txtTotRemuneration.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0";

                Calculate_ClaimDetails(oEncashmentReimbursement);

                //txtTotVariation.Text = (Util.GetDecimalNumber(txtTotEncashment.Text) - oEncashmentReimbursement.EncashmentAmount).ToString();
                hdnEncashmentReimburseTransNo.Value = oEncashmentReimbursement.EncashmentReimburseTransNo;
                //Set a background color as Selected
                //gvClaim.Rows[0].BackColor = Color.Blue;
                //SetClaimSatementDetailSectionValue(oSalesStatementReconciled.DtClaimDetails);

                if (gvClaim.Rows.Count > 0)
                {
                    PopulateEncashmentPaymentDetailsGrid(gvClaim.Rows[0]);
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

                // Encashment Claim Detail
                Util.ControlEnabled(txtClaimRefNo, false);
                //Util.ControlEnabled(txtReconEncashment, false);                
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

                // Encashment Claim Detail 
                Util.ControlEnabled(txtClaimRefNo, true);
                //Util.ControlEnabled(txtReconEncashment, true);                
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

        protected void ClearEncashmentClaimSection()
        {
            txtClaimRefNo.Text = string.Empty;
            txtClaimDate.Text = string.Empty;
            txtClaimDateFrom.Text = string.Empty;
            txtClaimDateTo.Text = string.Empty;
            txtClaimCurrncy.Text = string.Empty;
            txtConvRate.Text = string.Empty;

            txtReimConvRate.Text = "1.0000";
            txtCovRateToBC.Text = "1.0000";

            txtEncashment.Text = "0.00";
            //txtReconEncashment.Text = "0.00";
            txtRemuneration.Text = "0.00";
            //txtReconRemuneration.Text = "0.00";
            //txtComment.Text = string.Empty;
        }
        protected void ClearEncashmentPaymentDetailSection()
        {
            txtRegNo.Text = "";
            txtPaymentDate.Text = "";
            txtClaimAmount.Text = "";
            txtReconAmount.Text = "";
            hdnEncashmentTransNo.Value = "";
            hdnClaimRefNo.Value = "";
        }
        protected void ClearEncashmentPaymentDetail()
        {
            gvIntClaimDetail.DataSource = null;
            gvIntClaimDetail.DataBind();
            txtNoOfEncashmentPay.Text = string.Empty;
            txtTotalClaimAmount.Text = string.Empty;
            txtTotalReconAmount.Text = string.Empty;
        }

        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");

            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_REC_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_RECON.ENCASHMENT).PadLeft(5, '0'), false);
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
            txtTotEncashment.Text = string.Empty;
            txtTotRecInt.Text = string.Empty;
            txtTotRemuneration.Text = string.Empty;
            txtTotRecRem.Text = string.Empty;
            txtTotVariation.Text = string.Empty;
            txtTotReconVariation.Text = string.Empty;
            txtBBAmount.Text = "0.00";

            ClearEncashmentClaimSection();
            ClearEncashmentPaymentDetail();
            ucUserDet.ResetData();
            hdnEncashmentReimburseTransNo.Value = string.Empty;
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBBReferenceNo.Text))
            {
                EncashmentReimbursement oIntReim = new EncashmentReimbursement();
                oIntReim.EncashmentReimburseReferenceNo = txtBBReferenceNo.Text;
                EncashmentReimbursementDAL oIntReimDAL = new EncashmentReimbursementDAL();
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
                EncashmentReimbursement oIntReim = Session[Constants.SES_INTE_RECON] as EncashmentReimbursement;
                EncashmentReimbursementDAL oEncashmentReimbursementDAL = new EncashmentReimbursementDAL();

                //get User Details
                oIntReim.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oEncashmentReimbursementDAL.Approve(oIntReim);
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
            if (!string.IsNullOrEmpty(hdnEncashmentReimburseTransNo.Value))
            {
                EncashmentReimbursementDAL oIntReimDAL = new EncashmentReimbursementDAL();
                Result oResult = (Result)oIntReimDAL.Detete(hdnEncashmentReimburseTransNo.Value);
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
            EncashmentReimbursement oEncashmentReimbursement = (EncashmentReimbursement)Session[Constants.SES_INTE_RECON];
            if (oEncashmentReimbursement != null)
            {
                oEncashmentReimbursement.EncashmentReimburseTransNo = hdnEncashmentReimburseTransNo.Value.Equals("") ? "-1" : hdnEncashmentReimburseTransNo.Value;
                oEncashmentReimbursement.EncashmentReimburseReferenceNo = txtBBReferenceNo.Text.Trim().ToUpper();
                oEncashmentReimbursement.SPType.SPTypeID = ddlSpType.SelectedValue;
                oEncashmentReimbursement.ReimburseDate = DateTime.Now;
                oEncashmentReimbursement.Currency.CurrencyID = ddlReconCurrency.SelectedValue;
                oEncashmentReimbursement.ConvRate = Util.GetDecimalNumber(txtReimConvRate.Text);
                oEncashmentReimbursement.ConvRateToBC = Util.GetDecimalNumber(txtCovRateToBC.Text);
                oEncashmentReimbursement.BBAmount  = Util.GetDecimalNumber(txtBBAmount.Text);
                oEncashmentReimbursement.ClaimAmount = Util.GetDecimalNumber(txtTotEncashment.Text);
                oEncashmentReimbursement.ReconAmount = Util.GetDecimalNumber(txtTotRecInt.Text);
                oEncashmentReimbursement.Remuneration = Util.GetDecimalNumber(txtTotRecRem.Text);


                decimal dIR = 0;
                decimal dDR = 0;
                object obj = oEncashmentReimbursement.DtEncsClaimReimbursement.Compute("SUM(ReconAmount)", "");
                dIR = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj) : 0;

                obj = 0;
                obj = oEncashmentReimbursement.DtEncsPaymentDetails.Compute("SUM(ReconAmount)", "");
                dDR = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj) : 0;

                if (dIR != dDR)
                {
                    ucMessage.OpenMessage("Claim recon details and payment recon details mismatched. May have manual journal. Please check..", Constants.MSG_TYPE_ERROR);
                    return;
                }

                oEncashmentReimbursement.UserDetails = ucUserDet.UserDetail;
                EncashmentReimbursementDAL oEncashmentReimbursementDAL = new EncashmentReimbursementDAL();
                Result oResult = oEncashmentReimbursementDAL.Save(oEncashmentReimbursement);
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
            Response.Redirect(Constants.PAGE_REC_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_RECON.ENCASHMENT).PadLeft(5, '0'), false);
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
            Result oResult = oSPPolicyDAL.GetCurrencyList(sSPTypeID, Constants.ACTIVITY_TYPE.PRINCIPAL_REIMBURSE, DateTime.Now);
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
            ClearEncashmentClaimSection();
            this.txtClaimRefNo.Focus();
        }

        protected void gvIntClaimDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //get the row
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            //gvRow.BackColor = Color.Blue;

            if (((Button)e.CommandSource).Text.Equals("Select"))
            {
                //SetEncashmentClaimDetails(gvRow);
                EncashmentReimbursement oEncashmentReimbursement = (EncashmentReimbursement)Session[Constants.SES_INTE_RECON];

                DataView dv = new DataView();
                dv.Table = oEncashmentReimbursement.DtEncsPaymentDetails;
                dv.RowFilter = "ClaimRefNo='" + gvRow.Cells[2].Text.Trim() + "'";

                txtRegNo.Text = dv[gvRow.RowIndex]["RegNo"].ToString();
                txtPaymentDate.Text = dv[gvRow.RowIndex]["PaymentDate"].ToString();
                txtClaimAmount.Text = dv[gvRow.RowIndex]["ClaimAmount"].ToString();
                txtReconAmount.Text = dv[gvRow.RowIndex]["ReconAmount"].ToString();
                txtIntClaimTransNo.Text = dv[gvRow.RowIndex]["EncashmentClaimTransNo"].ToString();
                txtEncTransNo.Text = dv[gvRow.RowIndex]["EncashmentTransNo"].ToString();
                txtCRefNo.Text = dv[gvRow.RowIndex]["ClaimRefNo"].ToString();
                txtReconAmount.Focus();
                dv.RowFilter = null;
                //btnAdd.Focus();

            }
            else if (((Button)e.CommandSource).Text.Equals("Remove"))
            {
                EncashmentReimbursement oEncashmentReimbursement = (EncashmentReimbursement)Session[Constants.SES_INTE_RECON];

                if (oEncashmentReimbursement != null)
                {
                    //Delete Encashment Payment Details
                    Delete_PaymentDetails(gvRow.Cells[2].Text, gvRow.Cells[3].Text, oEncashmentReimbursement);
                    Populate_PaymentDetails(gvRow.Cells[2].Text);

                    DataView dv1 = oEncashmentReimbursement.DtEncsClaimReimbursement.DefaultView;
                    dv1.RowFilter = "ClaimRefNo='" + gvRow.Cells[2].Text + "'";
                    if (dv1.Count > 0)
                    {
                        object obj = oEncashmentReimbursement.DtEncsPaymentDetails.Compute("SUM(ReconAmount)", "ClaimRefNo='" + gvRow.Cells[2].Text + "'");
                        dv1[0]["ReconAmount"] = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";
                        dv1.RowFilter = null;
                    }

                    //Store in Session                    
                    Session[Constants.SES_INTE_RECON] = oEncashmentReimbursement;
                    Calculate_ClaimDetails(oEncashmentReimbursement);

                    ClearEncashmentPaymentDetailSection();
                }

                txtClaimRefNo.Focus();
            }
        }

        protected void btnResetDetails_Click(object sender, EventArgs e)
        {
            ClearEncashmentPaymentDetailSection();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            EncashmentReimbursement oEncashmentReimbursement = (EncashmentReimbursement)Session[Constants.SES_INTE_RECON];
            try
            {
                DataView dv = oEncashmentReimbursement.DtEncsPaymentDetails.DefaultView;
                dv.RowFilter = "EncashmentTransNo='" + txtEncTransNo.Text + "' AND EncashmentClaimTransNo='" + txtIntClaimTransNo.Text + "'";
                if (dv.Count > 0)
                {
                    dv[0]["ReconAmount"] = Convert.ToDecimal(txtReconAmount.Text).ToString("N2");
                    dv.RowFilter = null;
                }
                oEncashmentReimbursement.DtEncsPaymentDetails.AcceptChanges();


                Populate_PaymentDetails(txtCRefNo.Text);
                Calculate_PaymentDetails(oEncashmentReimbursement, txtCRefNo.Text);
                DataView dv1 = oEncashmentReimbursement.DtEncsClaimReimbursement.DefaultView;
                dv1.RowFilter = "ClaimRefNo='" + txtCRefNo.Text + "'";
                if (dv1.Count > 0)
                {
                    object obj = oEncashmentReimbursement.DtEncsPaymentDetails.Compute("SUM(ReconAmount)", "ClaimRefNo='" + txtCRefNo.Text + "'");
                    dv1[0]["ReconAmount"] = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";
                    dv1.RowFilter = null;
                }
                Calculate_ClaimDetails(oEncashmentReimbursement);
                oEncashmentReimbursement.DtEncsPaymentDetails.AcceptChanges();
                ClearEncashmentPaymentDetailSection();
            }
            catch (Exception ex)
            {
                ucMessage.OpenMessage(ex.Message, Constants.MSG_TYPE_ERROR);
            }
        }

        protected void txtTotRecRem_TextChanged(object sender, EventArgs e)
        {
            if (txtTotRecRem.Text == string.Empty)
            {
                txtTotRecRem.Text = txtTotRemuneration.Text;
            }
            EncashmentReimbursement oEncashmentReimbursement = (EncashmentReimbursement)Session[Constants.SES_INTE_RECON];
            oEncashmentReimbursement.Remuneration = Util.GetDecimalNumber(txtTotRecRem.Text);
            Session[Constants.SES_INTE_RECON] = oEncashmentReimbursement;
            //Total Recon Remunaration
            //obj = oEncashmentReimbursement.DtEncsClaimReimbursement.Compute("SUM(ReconRemuneration)", "");
            //txtTotRecRem.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0.00";
            //variation
           // txtTotVariation.Text = (Util.GetDecimalNumber(txtTotEncashment.Text) - Util.GetDecimalNumber(txtTotRecInt.Text)).ToString();
            txtTotReconVariation.Text = (Util.GetDecimalNumber(txtTotRecRem.Text)-Util.GetDecimalNumber(txtTotRemuneration.Text)).ToString();
            txtBBVariation.Text = (Util.GetDecimalNumber(txtBBAmount.Text) - (Util.GetDecimalNumber(txtTotRecInt.Text) + Util.GetDecimalNumber(txtTotRecRem.Text))).ToString();
        }

        protected void txtBBAmount_TextChanged(object sender, EventArgs e)
        {
            
            EncashmentReimbursement oEncashmentReimbursement = (EncashmentReimbursement)Session[Constants.SES_INTE_RECON];
            Calculate_ClaimDetails(oEncashmentReimbursement);
        }

    }
}
