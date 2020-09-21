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
using SBM_BLC1.Common;
using SBM_BLC1.Entity.Reconciliation;
using SBM_BLC1.DAL.Reconciliation;
using SBM_BLC1.Entity.Common;
using System.Globalization;
using SBM_BLC1.Entity.Reimbursement;
using SBM_BLC1.Configuration;
using SBM_BLC1.Entity.Configuration;

namespace SBM_WebUI.mp
{
    public partial class ComClaimRec : System.Web.UI.Page
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
            gvSales.PageSize = (int)Constants.PAGING_UNAPPROVED;

            gvClaim.DataSource = null;
            gvClaim.DataBind();

            gvSales.DataSource = null;
            gvSales.DataBind();

            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            // year
            for (int i = 1995; i < DateTime.Today.Year + 1; i++)
            {
                DDListUtil.Add(ddlYear, i.ToString(), i.ToString());
            }
            DDListUtil.Assign(ddlYear, DateTime.Now.Year);
            txtReconDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtRecConvRate.Text = "0.00";

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
                ddlSpType.Focus();
            }
        }
        #endregion InitializeData

        private void LoadPreviousList()
        {
            CommClaimReconciliationDAL oComClaimRecDAL = new CommClaimReconciliationDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oComClaimRecDAL.LoadUnapprovedList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);

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
            CommClaimReconciliationDAL oComClaimRecDAL = new CommClaimReconciliationDAL();
            Result oResult = oComClaimRecDAL.GetCommClaimDataByBBRefNo(sBBRefNo, sApprovalStatus);
            if (oResult.Status)
            {
                SetObject(oResult.Return as CommissionReimbursement);
            }
        }

        private void SetObject(CommissionReimbursement oComReim)
        {
            Session[Constants.SES_COMM_CLAIM_RECON] = oComReim;

            if (oComReim != null)
            {
                ddlSpType.Text = oComReim.SPType.SPTypeID;
                ddlYear.Text = oComReim.ReimburseDate.Year.ToString();
                txtReconDate.Text = oComReim.ReimburseDate.ToString(Constants.DATETIME_FORMAT);
                txtBBReferenceNo.Text = oComReim.CommissionReimburseReferenceNo;
                LoadCurrencyBySPType(oComReim.SPType.SPTypeID);
                ddlReconCurrency.Text = oComReim.Currency.CurrencyID;
                txtRecConvRate.Text = Convert.ToString(oComReim.ConvRate);
                hdnReconComClaimTransNo.Value = oComReim.CommissionReimburseTransNo;

                //Set user details
                if (SEARCH_FROM.Equals(1))//if viewed from Temp By Maker
                {
                    ucUserDet.UserDetail = oComReim.UserDetails;
                }
                else if (SEARCH_FROM.Equals(2))//if viewed from Temp By Checker
                {
                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    oUserDetails.MakeDate = oComReim.UserDetails.MakeDate;
                    oUserDetails.MakerID = oComReim.UserDetails.MakerID;
                    ucUserDet.UserDetail = oUserDetails;
                }

                gvClaim.DataSource = oComReim.DtCommClaimReimbursement;
                gvClaim.DataBind();
                //Set a background color as Selected
                //gvClaim.Rows[0].BackColor = Color.Blue;
                SetClaimSatementDetailSectionValue(oComReim.DtCommClaimReimbursement);

                if (gvClaim.Rows.Count > 0)
                {
                    PopulateSalesStatementDetailsGrid(gvClaim.Rows[0]);
                }
            }
        }

        private void PopulateSalesStatementDetailsGrid(GridViewRow gvRow)
        {
            CommClaimReconciliationDAL oComRecDAL = new CommClaimReconciliationDAL();
            Result oResult = oComRecDAL.GetSalesDetailByRefNo(gvRow.Cells[2].Text);
            if (oResult.Status)
            {
                DataTable dtSalesDetail = oResult.Return as DataTable;
                
                //txtNoOfReg.Text = dtSalesDetail.Rows.Count.ToString();
                object obj = dtSalesDetail.Compute("SUM(TotalOrgCommission)", "");
                txtSalOrgComTtl.Text = Convert.ToDecimal(obj).ToString("N2");
                obj = dtSalesDetail.Compute("SUM(TotalNonOrgCommission)", "");
                txtSalNonOrgComTtl.Text = Convert.ToDecimal(obj).ToString("N2");

                gvSales.DataSource = dtSalesDetail;
                gvSales.DataBind();
            }
        }

        private void EnableDisableControl(bool isApproved)
        {
            // general Control
            if (isApproved)
            {
                //BB Details
                Util.ControlEnabled(ddlSpType, false);
                Util.ControlEnabled(ddlYear, false);
                Util.ControlEnabled(txtReconDate, false);
                Util.ControlEnabled(txtBBReferenceNo, false);
                Util.ControlEnabled(ddlReconCurrency, false);
                Util.ControlEnabled(txtRecConvRate, false);

                // Sales Claim Detail 
                Util.ControlEnabled(txtReferenceNo, false);
                Util.ControlEnabled(txtClaimBBDate, false);
                Util.ControlEnabled(txtClaimFromDate, false);
                Util.ControlEnabled(txtClaimToDate, false);
                Util.ControlEnabled(txtOrgCommssn, false);
                Util.ControlEnabled(txtNonOrgCommssn, false);
                Util.ControlEnabled(txtClaimComment, false);
                Util.ControlEnabled(txtRecOrgCommssn, false);
                Util.ControlEnabled(txtRecNonOrgCommssn, false);

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
                Util.ControlEnabled(txtReconDate, true);
                Util.ControlEnabled(txtBBReferenceNo, true);
                Util.ControlEnabled(ddlReconCurrency, true);
                Util.ControlEnabled(txtRecConvRate, true);

                // Sales Claim Detail 
                Util.ControlEnabled(txtReferenceNo, true);
                Util.ControlEnabled(txtClaimBBDate, true);
                Util.ControlEnabled(txtClaimFromDate, true);
                Util.ControlEnabled(txtClaimToDate, true);
                Util.ControlEnabled(txtOrgCommssn, true);
                Util.ControlEnabled(txtNonOrgCommssn, true);
                Util.ControlEnabled(txtClaimComment, true);
                Util.ControlEnabled(txtRecOrgCommssn, true);
                Util.ControlEnabled(txtRecNonOrgCommssn, true);

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
                Response.Redirect(Constants.PAGE_REC_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_RECON.COMMISSION).PadLeft(5, '0'), false);
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
            CommissionReimbursement oCommReim = (CommissionReimbursement)Session[Constants.SES_COMM_CLAIM_RECON];
            if (oCommReim == null)
            {
                oCommReim = new CommissionReimbursement();
            }

            // SaleStatement List
            DataTable dtClaimDetails = oCommReim.DtCommClaimReimbursement;
            if (dtClaimDetails.Columns.Count <= 0)
            {
                dtClaimDetails.Columns.Add(new DataColumn("RefNo", typeof(string)));
                dtClaimDetails.Columns.Add(new DataColumn("ClaimDateBB", typeof(string)));
                dtClaimDetails.Columns.Add(new DataColumn("ClaimDateFrom", typeof(string)));
                dtClaimDetails.Columns.Add(new DataColumn("ClaimDateTo", typeof(string)));
                dtClaimDetails.Columns.Add(new DataColumn("TotlOrgComm", typeof(decimal)));
                dtClaimDetails.Columns.Add(new DataColumn("TotlNonOrgComm", typeof(decimal)));
                dtClaimDetails.Columns.Add(new DataColumn("RecOrgComm", typeof(decimal)));
                dtClaimDetails.Columns.Add(new DataColumn("RecNonOrgComm", typeof(decimal)));
                dtClaimDetails.Columns.Add(new DataColumn("Comment", typeof(string)));
                //saving in DB purpose. This is hidden is design
                dtClaimDetails.Columns.Add(new DataColumn("CommClaimTransNo", typeof(string)));
            }
            DataRow[] rows = dtClaimDetails.Select("RefNo='" + txtReferenceNo.Text + "'");
            
            if (rows.Length <= 0)
            {
                DataRow rowClaimDetails = dtClaimDetails.NewRow();
                rowClaimDetails["RefNo"] = txtReferenceNo.Text.ToUpper();
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
                rowClaimDetails["TotlOrgComm"] = txtOrgCommssn.Text;
                rowClaimDetails["TotlNonOrgComm"] = txtNonOrgCommssn.Text;
                rowClaimDetails["RecOrgComm"] = txtRecOrgCommssn.Text;
                rowClaimDetails["RecNonOrgComm"] = txtRecNonOrgCommssn.Text;                
                rowClaimDetails["Comment"] = txtClaimComment.Text.ToUpper();
                rowClaimDetails["CommClaimTransNo"] = hdnCommClaimTrnsNo.Value;

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
                    dtClaimDetails.Rows[iRowIndex]["RecOrgComm"] = txtRecOrgCommssn.Text;
                    dtClaimDetails.Rows[iRowIndex]["RecNonOrgComm"] = txtRecNonOrgCommssn.Text;
                    dtClaimDetails.Rows[iRowIndex]["Comment"] = txtClaimComment.Text.ToUpper();
                }             
            }

            //Store in Session
            oCommReim.DtCommClaimReimbursement = dtClaimDetails;
            Session[Constants.SES_COMM_CLAIM_RECON] = oCommReim;

            gvClaim.DataSource = dtClaimDetails;
            gvClaim.DataBind();

            //Set Values                
            SetClaimSatementDetailSectionValue(dtClaimDetails);
            ClearCommClaimSection();
            txtReferenceNo.Focus();
        }

        protected void ClearCommClaimSection()
        {
            txtReferenceNo.Text = string.Empty;
            txtClaimBBDate.Text = string.Empty;
            txtClaimFromDate.Text = string.Empty;
            txtClaimToDate.Text = string.Empty;
            txtClaimConvRate.Text = string.Empty;
            txtClaimCurrncy.Text = string.Empty;
            txtOrgCommssn.Text = string.Empty;
            txtNonOrgCommssn.Text = string.Empty;
            txtRecOrgCommssn.Text = string.Empty;
            txtRecNonOrgCommssn.Text = string.Empty;
            txtClaimComment.Text = string.Empty;         
        }

        protected void SetClaimSatementDetailSectionValue(DataTable dtClaimDetails)
        {
            object obj = dtClaimDetails.Compute("SUM(RecOrgComm)", "");
            txtOrgComSubTotl.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString("N2") : "0.00";
            obj = dtClaimDetails.Compute("SUM(RecNonOrgComm)", "");
            txtNonOrgComSubTotl.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString("N2") : "0.00";
            txtVariation.Text = (Util.GetDecimalNumber(txtOrgComSubTotl.Text) + Util.GetDecimalNumber(txtNonOrgComSubTotl.Text)).ToString("N2");
        }

        protected void gvClaim_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //get the row
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            //gvClaim.RowStyle.CssClass = "odd";
            //gvRow.BackColor = Color.Blue;

            if (((Button)e.CommandSource).Text.Equals("Select"))
            {
                PopulateSalesStatementDetailsGrid(gvRow);
                SetCommReconDetails(gvRow);
                btnAdd.Focus();
            }
            else if (((Button)e.CommandSource).Text.Equals("Remove"))
            {
                CommissionReimbursement oComReim = (CommissionReimbursement)Session[Constants.SES_COMM_CLAIM_RECON];

                if (oComReim != null)
                {
                    oComReim.DtCommClaimReimbursement.Rows.RemoveAt(gvRow.RowIndex);
                                        
                    gvClaim.DataSource = oComReim.DtCommClaimReimbursement;
                    gvClaim.DataBind();

                    //Store in Session                    
                    Session[Constants.SES_COMM_CLAIM_RECON] = oComReim;
                    
                    if (gvRow.Cells[2].Text.Equals(txtReferenceNo.Text.Trim()))
                    {
                        ClearCommClaimSection();
                    }
                    //Set Values
                    SetClaimSatementDetailSectionValue(oComReim.DtCommClaimReimbursement);                    
                }

                txtReferenceNo.Focus();
            }
        }

        protected void gvClaim_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        private void SetCommReconDetails(GridViewRow gvRow)
        {
            txtReferenceNo.Text = gvRow.Cells[2].Text;
            txtClaimBBDate.Text = Convert.ToDateTime(gvRow.Cells[3].Text).ToString(Constants.DATETIME_FORMAT);
            txtClaimFromDate.Text = Convert.ToDateTime(gvRow.Cells[4].Text).ToString(Constants.DATETIME_FORMAT);
            txtClaimToDate.Text = Convert.ToDateTime(gvRow.Cells[5].Text).ToString(Constants.DATETIME_FORMAT);
            txtClaimCurrncy.Text = "BDT";
            txtClaimConvRate.Text = "1.0000";
            txtOrgCommssn.Text = gvRow.Cells[6].Text;
            txtNonOrgCommssn.Text = gvRow.Cells[7].Text;
            txtRecOrgCommssn.Text = gvRow.Cells[8].Text;
            txtRecNonOrgCommssn.Text = gvRow.Cells[9].Text;
            //To avoid assigning HTML value
            //as it assigns '&nbsp;' while reading 'empty' from cell
            txtClaimComment.Text = System.Web.HttpUtility.HtmlDecode(gvRow.Cells[10].Text);
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
                sRefNo = ddlSpType.SelectedValue + "/ABC/" + sRefNo + "/" + ddlYear.SelectedValue;
                txtReferenceNo.Text = sRefNo;
                isRefNoFrmtOk = true;
            }
            if (isRefNoFrmtOk)
            {
                int iIndx = -1;
                CommissionReimbursement oCommReim = (CommissionReimbursement)Session[Constants.SES_COMM_CLAIM_RECON];
                if (oCommReim != null)
                {
                    DataTable dtClaimDetails = oCommReim.DtCommClaimReimbursement;
                    //For Each dr As DataRow In dt.Select("query")
                    DataRow[] rows = dtClaimDetails.Select("RefNo='" + txtReferenceNo.Text + "'");                    
                    foreach (var vClaimDtl in rows)
                    {
                        iIndx = dtClaimDetails.Rows.IndexOf(vClaimDtl);
                        if((gvClaim.Rows.Count > iIndx))
                        {
                            GridViewRow gvRow = gvClaim.Rows[iIndx];
                            SetCommReconDetails(gvRow);
                        }
                        break;
                    }
                }
                if (iIndx.Equals(-1))
                {
                    CommClaimReconciliationDAL oComRecDAL = new CommClaimReconciliationDAL();
                    Result oResult = oComRecDAL.GetCommClaimDataByRefNo(txtReferenceNo.Text, ucUserDet.UserDetail.Division);

                    if (oResult.Status)
                    {
                        DataTable dtCommClaim = oResult.Return as DataTable;
                        if (dtCommClaim.Columns.Count.Equals(1))
                        {
                            ucMessage.OpenMessage("This reference number already reconciled !!", Constants.MSG_TYPE_INFO);
                            ScriptManager.RegisterStartupScript(this.UpdatePanel5, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                            ClearCommClaimSection();
                        }
                        else if (dtCommClaim.Rows.Count > 0)
                        {
                            //DDListUtil.Assign(ddlSpType, DB.GetDBValue(dtCommClaim.Rows[0]["SPTypeID"]));
                            txtClaimBBDate.Text = (Date.GetDateTimeByString(dtCommClaim.Rows[0]["StatementDate"].ToString())).ToString(Constants.DATETIME_FORMAT);
                            txtClaimFromDate.Text = (Date.GetDateTimeByString(dtCommClaim.Rows[0]["FromDate"].ToString())).ToString(Constants.DATETIME_FORMAT);
                            txtClaimToDate.Text = (Date.GetDateTimeByString(dtCommClaim.Rows[0]["ToDate"].ToString())).ToString(Constants.DATETIME_FORMAT);
                            txtClaimCurrncy.Text = DB.GetDBValue(dtCommClaim.Rows[0]["CurrencyCode"]);
                            txtClaimConvRate.Text = DB.GetDBDecimalValue(dtCommClaim.Rows[0]["ConvRate"]).ToString();
                            txtOrgCommssn.Text = DB.GetDBDecimalValue(dtCommClaim.Rows[0]["TotalOrgCommission"]).ToString();
                            txtNonOrgCommssn.Text = DB.GetDBDecimalValue(dtCommClaim.Rows[0]["TotalNonOrgCommission"]).ToString();
                            txtRecOrgCommssn.Text = DB.GetDBDecimalValue(dtCommClaim.Rows[0]["TotalOrgCommission"]).ToString();
                            txtRecNonOrgCommssn.Text = DB.GetDBDecimalValue(dtCommClaim.Rows[0]["TotalNonOrgCommission"]).ToString();
                            txtClaimComment.Text = string.Empty;
                            hdnCommClaimTrnsNo.Value = DB.GetDBValue(dtCommClaim.Rows[0]["CommissionClaimTransNo"]);
                        }
                        else
                        {
                            ucMessage.OpenMessage("Invalid reference number!!", Constants.MSG_TYPE_INFO);
                            ScriptManager.RegisterStartupScript(this.UpdatePanel5, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                            ClearCommClaimSection();
                        }
                    }
                }

                btnAdd.Focus();
            }
            else
            {
                ucMessage.OpenMessage("Please select SP Type and Year.", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                ClearCommClaimSection();
            }
        }

        private void TotalClear()
        {
            Session[Constants.SES_COMM_CLAIM_RECON] = null;
            //GridView controlls
            gvClaim.DataSource = null;
            gvClaim.DataBind();

            gvSales.DataSource = null;
            gvSales.DataBind();

            ddlSpType.SelectedIndex = 0;
            ddlYear.Text = DateTime.Now.Year.ToString();
            ddlReconCurrency.Items.Clear();
            //TextBox controls            
            txtReconDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtBBReferenceNo.Text = string.Empty;
            txtClaimBBDate.Text = string.Empty;
            txtOrgComSubTotl.Text = string.Empty;
            txtNonOrgComSubTotl.Text = string.Empty;
            txtVariation.Text = string.Empty;
            txtSalNonOrgComTtl.Text = string.Empty;
            txtSalOrgComTtl.Text = string.Empty;

            ClearCommClaimSection();
            ucUserDet.ResetData();
            hdnReconComClaimTransNo.Value = string.Empty;
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBBReferenceNo.Text))
            {
                CommissionReimbursement oCommReim = new CommissionReimbursement();
                oCommReim.CommissionReimburseReferenceNo = txtBBReferenceNo.Text;
                CommClaimReconciliationDAL oCommClaimDAL = new CommClaimReconciliationDAL();
                oCommReim.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oCommClaimDAL.Reject(oCommReim);
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
                CommissionReimbursement oComReim = Session[Constants.SES_COMM_CLAIM_RECON] as CommissionReimbursement;                
                CommClaimReconciliationDAL oCommClaimRecDAL = new CommClaimReconciliationDAL();                                
                //get User Details
                oComReim.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oCommClaimRecDAL.Approve(oComReim);
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
            if (!string.IsNullOrEmpty(hdnReconComClaimTransNo.Value))
            {
                CommClaimReconciliationDAL oCommClaimReconDAL = new CommClaimReconciliationDAL();
                Result oResult = (Result)oCommClaimReconDAL.Detete(hdnReconComClaimTransNo.Value);

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
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            CommissionReimbursement oCommReim = (CommissionReimbursement)Session[Constants.SES_COMM_CLAIM_RECON];
            if (oCommReim != null)
            {
                oCommReim.CommissionReimburseTransNo = hdnReconComClaimTransNo.Value.Equals("") ? "-1" : hdnReconComClaimTransNo.Value;
                oCommReim.CommissionReimburseReferenceNo = txtBBReferenceNo.Text.Trim().ToUpper();
                oCommReim.SPType.SPTypeID = ddlSpType.SelectedValue;
                oCommReim.ReimburseDate = Util.GetDateTimeByString(txtReconDate.Text);
                oCommReim.Currency.CurrencyID = ddlReconCurrency.SelectedValue;                
                oCommReim.ConvRate = Util.GetDecimalNumber(txtRecConvRate.Text);
                oCommReim.OrgCommissionAmount = Util.GetDecimalNumber(txtOrgComSubTotl.Text);
                oCommReim.NonOrgCommissionAmount = Util.GetDecimalNumber(txtNonOrgComSubTotl.Text);                

                oCommReim.UserDetails = ucUserDet.UserDetail;

                CommClaimReconciliationDAL oCommClaimRecDAL = new CommClaimReconciliationDAL();
                Result oResult = oCommClaimRecDAL.Save(oCommReim);
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
                    txtRecConvRate.Enabled = true;
                    txtRecConvRate.Text = string.Empty;
                }
                else
                {
                    txtRecConvRate.Text = "1.0000";
                    txtRecConvRate.Enabled = false;
                }
            }

            txtRecConvRate.Text = string.Empty;
            txtRecConvRate.Enabled = true;
            txtBBReferenceNo.Focus();
        }

        private void LoadCurrencyBySPType(string sSPTypeID)
        {
            ddlReconCurrency.Items.Clear();
            SPPolicyDAL oSPPolicyDAL = new SPPolicyDAL();
            Result oResult = oSPPolicyDAL.GetCurrencyList(sSPTypeID, Constants.ACTIVITY_TYPE.COMMISSION_RECEIVE, DateTime.Now);
            if (oResult.Status)
            {
                DDListUtil.Assign(ddlReconCurrency, (DataTable)oResult.Return, true);
            }
        }

        protected void ddlReconCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sBaseCurrency = ViewState[_BASE_CURRENCY] as string;
            if (!string.IsNullOrEmpty(sBaseCurrency))
            {
                if (sBaseCurrency.Equals(ddlReconCurrency.SelectedValue))
                {
                    txtRecConvRate.Enabled = false;
                    txtRecConvRate.Text = "1.0000";
                }
                else
                {
                    txtRecConvRate.Enabled = true;
                    txtRecConvRate.Text = string.Empty;
                }
            }

            txtReferenceNo.Focus();
        }
    }
}
