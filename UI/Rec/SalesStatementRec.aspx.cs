using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.DAL.Reconciliation;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Entity.Reconciliation;
using System.Drawing;
using System.Globalization;

namespace SBM_WebUI.mp
{
    public partial class SalesStatementRec : System.Web.UI.Page
    {
        #region Local Variable
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_REF_NO = "sRefNo";
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
            gvReg.PageSize = (int)Constants.PAGING_UNAPPROVED;
     
            gvClaim.DataSource = null;
            gvClaim.DataBind();

            gvReg.DataSource = null;
            gvReg.DataBind();
            
            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            // year
            for (int i = 1995; i < DateTime.Today.Year + 1; i++)
            {
                DDListUtil.Add(ddlYear, i.ToString(), i.ToString());
            }
            DDListUtil.Assign(ddlYear, DateTime.Now.Year);
            txtDebitDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

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
        #endregion InitializeData

        private void LoadPreviousList()
        {
            SaleStatementReconDAL oSaleStatementReconDAL = new SaleStatementReconDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oSaleStatementReconDAL.LoadUnapprovedList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);

            if (oResult.Status)
            {
                DataTable dtTmpList = (DataTable)oResult.Return;

                if (dtTmpList != null && dtTmpList.Rows.Count > 0)
                {

                    dtTmpList.Columns.Remove("Maker ID");

                }
                
                gvData.DataSource = dtTmpList;
                gvData.DataBind();

                Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;
            }
        }

        private void LoadDataByReferenceNo(string sBBRefNo, string sApprovalStatus)
        {
            SaleStatementReconDAL oSaleStatementReconDAL = new SaleStatementReconDAL();
            Result oResult = oSaleStatementReconDAL.GetSalesStatementDataByBBRefNo(sBBRefNo, sApprovalStatus);
            if (oResult.Status)
            {
                SetObject(oResult.Return as SalesStatementReconciled);                
            }
        }

        private void SetObject(SalesStatementReconciled oSSRecon)
        {
            Session[Constants.SES_SALES_CLAIM_RECON] = oSSRecon;

            if (oSSRecon != null)
            {
                ddlSpType.Text = oSSRecon.SPType.SPTypeID;
                ddlYear.Text = oSSRecon.DebitDate.Year.ToString();
                txtDebitDate.Text = oSSRecon.DebitDate.ToString(Constants.DATETIME_FORMAT);
                txtBBReferenceNo.Text = oSSRecon.ReconSaleStatementReferenceNo;
                hdnReconSaleStatTransNo.Value = oSSRecon.ReconSaleStatementTransNo;

                //Set user details
                if (SEARCH_FROM.Equals(1))//if viewed from Temp By Maker
                {
                    ucUserDet.UserDetail = oSSRecon.UserDetails;
                }
                else if (SEARCH_FROM.Equals(2))//if viewed from Temp By Checker
                {
                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    oUserDetails.MakeDate = oSSRecon.UserDetails.MakeDate;
                    oUserDetails.MakerID = oSSRecon.UserDetails.MakerID;
                    ucUserDet.UserDetail = oUserDetails;                    
                }

                gvClaim.DataSource = oSSRecon.DtClaimDetails;
                gvClaim.DataBind();
                //Set a background color as Selected
                //gvClaim.Rows[0].BackColor = Color.Blue;
                SetClaimSatementDetailSectionValue(oSSRecon.DtClaimDetails);

                if (gvClaim.Rows.Count > 0)
                {
                    PopulateSalesStatementIssueDetailsGrid(gvClaim.Rows[0]);
                }
            }
        }

        private void PopulateSalesStatementIssueDetailsGrid(GridViewRow gvRow)
        {
            SaleStatementReconDAL oSaleStatmentReconDAL = new SaleStatementReconDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oSaleStatmentReconDAL.GetSalesIssueDetailByRefNo(gvRow.Cells[2].Text, oConfig.DivisionID);
            if (oResult.Status)
            {
                DataTable dtSalesIssueDetail = oResult.Return as DataTable;
                //Change Name for computing purpose
                dtSalesIssueDetail.Columns[3].ColumnName = "SalesAmount";

                txtNoOfReg.Text = dtSalesIssueDetail.Rows.Count.ToString();
                object obj = dtSalesIssueDetail.Compute("SUM(SalesAmount)", "");
                txtFaceValue.Text = Convert.ToDecimal(obj).ToString();
                //Change back to previous name
                dtSalesIssueDetail.Columns[3].ColumnName = "Sales Amount";

                gvReg.DataSource = dtSalesIssueDetail;
                gvReg.DataBind();
            }
        }

        private void SetReconciliationDetails(GridViewRow gvRow)
        {
            txtReferenceNo.Text = gvRow.Cells[2].Text;
            txtClaimBBDate.Text = Convert.ToDateTime(gvRow.Cells[3].Text).ToString(Constants.DATETIME_FORMAT);
            txtClaimFromDate.Text= Convert.ToDateTime(gvRow.Cells[4].Text).ToString(Constants.DATETIME_FORMAT);
            txtClaimToDate.Text= Convert.ToDateTime(gvRow.Cells[5].Text).ToString(Constants.DATETIME_FORMAT);
            txtClaimAmount.Text = gvRow.Cells[6].Text;
            txtDebitAmount.Text = gvRow.Cells[7].Text;
            //To avoid assigning HTML value
            //as it assigns '&nbsp;' while reading 'empty' from cell
            txtClaimComment.Text = System.Web.HttpUtility.HtmlDecode(gvRow.Cells[8].Text);
        }

        private void EnableDisableControl(bool isApproved)
        {
            // general Control
            if (isApproved)
            {
                //BB Details
                Util.ControlEnabled(ddlSpType, false);
                Util.ControlEnabled(ddlYear, false);                
                Util.ControlEnabled(txtBBReferenceNo, false);
                
                // Sales Claim Detail 
                Util.ControlEnabled(txtReferenceNo, false);
                Util.ControlEnabled(txtClaimBBDate, false);
                Util.ControlEnabled(txtClaimFromDate, false);
                Util.ControlEnabled(txtClaimToDate, false);
                Util.ControlEnabled(txtClaimAmount, false);
                Util.ControlEnabled(txtDebitAmount, false);
                Util.ControlEnabled(txtClaimComment, false);

                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);
                                                
                // Button
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true);

                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);
                Util.ControlEnabled(btnAdd, false);
                //Util.ControlEnabled(btnSearch, false);               

                ////Disable Grid's column
                gvClaim.Columns[1].Visible = false;
                
                fsList.Visible = false;
            }
            else
            {
                //BB Details
                Util.ControlEnabled(ddlSpType, true);
                Util.ControlEnabled(ddlYear, true);                
                Util.ControlEnabled(txtBBReferenceNo, true);

                // Sales Claim Detail 
                Util.ControlEnabled(txtReferenceNo, true);
                Util.ControlEnabled(txtClaimBBDate, true);
                Util.ControlEnabled(txtClaimFromDate, true);
                Util.ControlEnabled(txtClaimToDate, true);
                Util.ControlEnabled(txtClaimAmount, true);
                Util.ControlEnabled(txtDebitAmount, true);
                Util.ControlEnabled(txtClaimComment, true);

                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), false);

                // Button 
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);

                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);
                
                fsList.Visible = true;
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

        protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("Page"))
            {
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                SEARCH_FROM = 1;
                LoadDataByReferenceNo(gvRow.Cells[1].Text, "1");//query from Temp Table
            }
        }

        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, null);
        }

        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");

            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_REC_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_RECON.SALES_STATEMENT).PadLeft(5, '0'), false);
            }
            else if (sType.Equals(Constants.BTN_SAVE))
            {
                ddlSpType.Focus();
            }
            else
            {
                txtReferenceNo.Focus();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            SalesStatementReconciled oSalesStatementReconciled = (SalesStatementReconciled)Session[Constants.SES_SALES_CLAIM_RECON];
            if (oSalesStatementReconciled == null)
            {
                oSalesStatementReconciled = new SalesStatementReconciled();
            }
            
            // SaleStatement List
            DataTable dtClaimDetails = oSalesStatementReconciled.DtClaimDetails;
            if (dtClaimDetails.Columns.Count <= 0)
            {
                dtClaimDetails.Columns.Add(new DataColumn("RefNo", typeof(string)));
                dtClaimDetails.Columns.Add(new DataColumn("ClaimDateBB", typeof(string)));
                dtClaimDetails.Columns.Add(new DataColumn("ClaimDateFrom", typeof(string)));
                dtClaimDetails.Columns.Add(new DataColumn("ClaimDateTo", typeof(string)));
                dtClaimDetails.Columns.Add(new DataColumn("ClaimAmount", typeof(decimal)));
                dtClaimDetails.Columns.Add(new DataColumn("DebitAmount", typeof(decimal)));
                dtClaimDetails.Columns.Add(new DataColumn("Comment", typeof(string)));
                //saving in DB purpose. This is hidden is design
                dtClaimDetails.Columns.Add(new DataColumn("SaleStatementTransNo", typeof(string)));
            }
            DataRow[] rows = dtClaimDetails.Select("RefNo='" + txtReferenceNo.Text + "'");

            if (rows.Length <= 0)
            {
                DataRow rowClaimDetails = dtClaimDetails.NewRow();
                rowClaimDetails["RefNo"] = txtReferenceNo.Text;
                DateTime parsedDate;
                DateTime.TryParseExact(txtClaimBBDate.Text, Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                //parsedDate = Convert.ToDateTime(txtClaimBBDate.Text);
                rowClaimDetails["ClaimDateBB"] = parsedDate.ToString(Constants.DATETIME_dd_MMM_yyyy);//txtClaimBBDate.Text;
                DateTime.TryParseExact(txtClaimFromDate.Text, Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                //parsedDate = Convert.ToDateTime(txtClaimFromDate.Text);
                rowClaimDetails["ClaimDateFrom"] = parsedDate.ToString(Constants.DATETIME_dd_MMM_yyyy);//txtClaimFromDate.Text;
                DateTime.TryParseExact(txtClaimToDate.Text, Constants.DateTimeFormats, null, DateTimeStyles.None, out parsedDate);
                //parsedDate = Convert.ToDateTime(txtClaimToDate.Text);
                rowClaimDetails["ClaimDateTo"] = parsedDate.ToString(Constants.DATETIME_dd_MMM_yyyy);//txtClaimToDate.Text;
                rowClaimDetails["ClaimAmount"] = txtClaimAmount.Text;
                rowClaimDetails["DebitAmount"] = txtDebitAmount.Text;
                rowClaimDetails["Comment"] = txtClaimComment.Text;
                rowClaimDetails["SaleStatementTransNo"] = hdnSaleStatTrnsNo.Value;

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
                    dtClaimDetails.Rows[iRowIndex]["DebitAmount"] = txtDebitAmount.Text;
                    dtClaimDetails.Rows[iRowIndex]["Comment"] = txtClaimComment.Text.ToUpper();
                }
            }

            //Store in Session
            oSalesStatementReconciled.DtClaimDetails = dtClaimDetails;
            Session[Constants.SES_SALES_CLAIM_RECON] = oSalesStatementReconciled;

            gvClaim.DataSource = dtClaimDetails;
            gvClaim.DataBind();

            //Set Values
            SetClaimSatementDetailSectionValue(dtClaimDetails);
            ClearSalesClaimSection();
            txtReferenceNo.Focus();
        }
        
        protected void ClearSalesClaimSection()
        {
            txtReferenceNo.Text = string.Empty;
            txtClaimBBDate.Text = string.Empty;
            txtClaimFromDate.Text = string.Empty;
            txtClaimToDate.Text = string.Empty;
            txtClaimAmount.Text = string.Empty;
            txtDebitAmount.Text = "0";
            txtClaimComment.Text = string.Empty;
        }

        protected void SetClaimSatementDetailSectionValue(DataTable dtClaimDetails)
        {
            txtNoOfClaim.Text = dtClaimDetails.Rows.Count.ToString();
            object obj = dtClaimDetails.Compute("SUM(ClaimAmount)", "");
            txtTtlCalimAmnt.Text = Convert.ToDecimal(obj).ToString();
            obj = dtClaimDetails.Compute("SUM(DebitAmount)", "");
            txtTtlDrAmnt.Text = Convert.ToDecimal(obj).ToString();
        }

        protected void gvClaim_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //get the row
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            //gvClaim.RowStyle.CssClass = "odd";
            //gvRow.BackColor = Color.Blue;

            if (((Button)e.CommandSource).Text.Equals("Select"))
            {
                PopulateSalesStatementIssueDetailsGrid(gvRow);
                SetReconciliationDetails(gvRow);
                btnAdd.Focus();
            }
            else if (((Button)e.CommandSource).Text.Equals("Remove"))
            {                
                SalesStatementReconciled oSalesStatementReconciled = (SalesStatementReconciled)Session[Constants.SES_SALES_CLAIM_RECON];
                
                if (oSalesStatementReconciled != null)
                {                    
                    oSalesStatementReconciled.DtClaimDetails.Rows.RemoveAt(gvRow.RowIndex);

                    gvClaim.DataSource = oSalesStatementReconciled.DtClaimDetails;
                    gvClaim.DataBind();

                    //Store in Session                    
                    Session[Constants.SES_SALES_CLAIM_RECON] = oSalesStatementReconciled;

                    if (gvRow.Cells[2].Text.Equals(txtReferenceNo.Text.Trim()))
                    {
                        ClearSalesClaimSection();
                    }

                    //Set Values
                    txtNoOfClaim.Text = oSalesStatementReconciled.DtClaimDetails.Rows.Count.ToString();
                    object obj = oSalesStatementReconciled.DtClaimDetails.Compute("SUM(ClaimAmount)", "");
                    txtTtlCalimAmnt.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0";
                    obj = oSalesStatementReconciled.DtClaimDetails.Compute("SUM(DebitAmount)", "");
                    txtTtlDrAmnt.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0";
                }
            }
        }

        protected void gvClaim_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }
        
        protected void txtReferenceNo_TextChanged(object sender, EventArgs e)
        {
            bool isRefNoFrmtOk = false;
            string sRefNo = txtReferenceNo.Text.Trim();
            //string[] aRefNo = sRefNo.Split('/');
            //if (aRefNo.Length.Equals(4))
            //{
            //    isRefNoFrmtOk = true;
            //}
            if (!string.IsNullOrEmpty(ddlSpType.SelectedValue) && !string.IsNullOrEmpty(ddlYear.SelectedValue))
            {
                //sRefNo = ddlSpType.SelectedValue + "/ABC/" + sRefNo + "/" + ddlYear.SelectedValue;
                txtReferenceNo.Text = sRefNo;
                isRefNoFrmtOk = true;
            }
            if (isRefNoFrmtOk)
            {
                int iIndx = -1;
                SalesStatementReconciled oSSRecon = (SalesStatementReconciled)Session[Constants.SES_SALES_CLAIM_RECON];
                if (oSSRecon != null)
                {
                    DataTable dtClaimDetails = oSSRecon.DtClaimDetails;
                    //For Each dr As DataRow In dt.Select("query")
                    DataRow[] rows = dtClaimDetails.Select("RefNo='" + txtReferenceNo.Text + "'");
                    foreach (var vClaimDtl in rows)
                    {
                        iIndx = dtClaimDetails.Rows.IndexOf(vClaimDtl);
                        if ((gvClaim.Rows.Count > iIndx))
                        {
                            GridViewRow gvRow = gvClaim.Rows[iIndx];
                            SetReconciliationDetails(gvRow);
                        }
                        break;
                    }
                }
                if (iIndx.Equals(-1))
                {
                    SaleStatementReconDAL oSaleStatmentReconDAL = new SaleStatementReconDAL();
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    Result oResult = oSaleStatmentReconDAL.GetSalesStatementDataByRefNo(txtReferenceNo.Text, oConfig.DivisionID);
                    if (oResult.Status)
                    {
                        DataTable dtSaleStatement = oResult.Return as DataTable;
                        if (dtSaleStatement.Columns.Count.Equals(1))
                        {
                            ucMessage.OpenMessage("This reference number already reconciled !!", Constants.MSG_TYPE_INFO);
                            ScriptManager.RegisterStartupScript(this.UpdatePanel5, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                            ClearSalesClaimSection();
                        }
                        else if (dtSaleStatement.Rows.Count > 0)
                        {
                            //DDListUtil.Assign(ddlSpType, DB.GetDBValue(dtSaleStatement.Rows[0]["SPTypeID"]));
                            txtClaimBBDate.Text = (Date.GetDateTimeByString(dtSaleStatement.Rows[0]["StatementDate"].ToString())).ToString(Constants.DATETIME_FORMAT);
                            txtClaimFromDate.Text = (Date.GetDateTimeByString(dtSaleStatement.Rows[0]["FromDate"].ToString())).ToString(Constants.DATETIME_FORMAT);
                            txtClaimToDate.Text = (Date.GetDateTimeByString(dtSaleStatement.Rows[0]["ToDate"].ToString())).ToString(Constants.DATETIME_FORMAT);
                            txtClaimAmount.Text = DB.GetDBValue(dtSaleStatement.Rows[0]["TotalFaceValue"]);
                            txtDebitAmount.Text = DB.GetDBValue(dtSaleStatement.Rows[0]["TotalFaceValue"]);
                            txtClaimComment.Text = string.Empty;
                            hdnSaleStatTrnsNo.Value = DB.GetDBValue(dtSaleStatement.Rows[0]["SaleStatementTransNo"]);
                        }
                        else
                        {
                            ucMessage.OpenMessage("Invalid reference number!!", Constants.MSG_TYPE_INFO);
                            ScriptManager.RegisterStartupScript(this.UpdatePanel5, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                            ClearSalesClaimSection();
                        }
                    }
                }

                btnAdd.Focus();
            }
            else
            {
                ucMessage.OpenMessage("Please select SP Type and Year.", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                ClearSalesClaimSection();
            }
        }

        private void TotalClear()
        {
            Session[Constants.SES_SALES_CLAIM_RECON] = null;
            //GridView controlls
            gvClaim.DataSource = null;
            gvClaim.DataBind();

            gvReg.DataSource = null;
            gvReg.DataBind();
                        
            ddlSpType.SelectedIndex = 0;
            ddlYear.Text = DateTime.Now.Year.ToString();
            //TextBox controls            
            txtDebitDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtBBReferenceNo.Text = string.Empty;
            txtNoOfClaim.Text = string.Empty;
            txtTtlCalimAmnt.Text = string.Empty;
            txtTtlDrAmnt.Text = string.Empty;
            txtNoOfReg.Text = string.Empty;
            txtFaceValue.Text = string.Empty;

            ClearSalesClaimSection();
            ucUserDet.ResetData();
            hdnReconSaleStatTransNo.Value = string.Empty;
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBBReferenceNo.Text))
            {
                SalesStatementReconciled oSalesStatementReconciled = new SalesStatementReconciled();
                oSalesStatementReconciled.ReconSaleStatementReferenceNo = txtBBReferenceNo.Text;
                SaleStatementReconDAL oSaleStatementReconDAL = new SaleStatementReconDAL();
                oSalesStatementReconciled.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oSaleStatementReconDAL.Reject(oSalesStatementReconciled);
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
                //oLien.UserDetails = ucUserDet.UserDetail;
                SaleStatementReconDAL oSaleStatementReconDAL = new SaleStatementReconDAL();
                SalesStatementReconciled oSalesStatementReconciled = new SalesStatementReconciled();
                oSalesStatementReconciled.ReconSaleStatementReferenceNo = txtBBReferenceNo.Text;
                //get User Details
                oSalesStatementReconciled.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oSaleStatementReconDAL.Approve(oSalesStatementReconciled);
                if (oResult.Status)
                {
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

        protected void btnReset_Click(object sender, EventArgs e)
        {
            //EnableDisableControl(false);
            TotalClear();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            //if (!hdDataType.Value.Equals("2"))
            //{
            if (!string.IsNullOrEmpty(hdnReconSaleStatTransNo.Value))
            {
                SaleStatementReconDAL oSaleStatementReconDAL = new SaleStatementReconDAL();
                Result oResult = (Result)oSaleStatementReconDAL.Detete(hdnReconSaleStatTransNo.Value);

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
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_DELETE, Constants.MSG_TYPE_ERROR);
            }
            //}
            //else
            //{
            //    ucMessage.OpenMessage(Constants.MSG_APPROVED_DELETE_DATA, Constants.MSG_TYPE_INFO);
            //    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            //}
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SalesStatementReconciled oSalesStatementReconciled = (SalesStatementReconciled)Session[Constants.SES_SALES_CLAIM_RECON];
            if (oSalesStatementReconciled != null)
            {
                oSalesStatementReconciled.ReconSaleStatementTransNo = hdnReconSaleStatTransNo.Value.Equals("") ? "-1" : hdnReconSaleStatTransNo.Value;
                oSalesStatementReconciled.ReconSaleStatementReferenceNo = txtBBReferenceNo.Text.Trim().ToUpper();
                oSalesStatementReconciled.SPType.SPTypeID = ddlSpType.SelectedValue;
                oSalesStatementReconciled.DebitDate = Util.GetDateTimeByString(txtDebitDate.Text);
                oSalesStatementReconciled.DebitedFaceValue = Util.GetDecimalNumber(txtTtlDrAmnt.Text);
                
                oSalesStatementReconciled.UserDetails = ucUserDet.UserDetail;
                
                SaleStatementReconDAL oSaleStatmentReconDAL = new SaleStatementReconDAL();
                Result oResult = oSaleStatmentReconDAL.Save(oSalesStatementReconciled);
                if (oResult.Status)
                {
                    TotalClear();
                    LoadPreviousList();
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
            Response.Redirect(Constants.PAGE_REC_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_RECON.SALES_STATEMENT).PadLeft(5, '0'), false);
        }
    }
}
