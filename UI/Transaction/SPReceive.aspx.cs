using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.Configuration;
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Entity.Transaction;
using SBM_BLC1.Transaction;
using System.Collections;
using System.Text;
using SBM_BLC1.DAL.Report;

namespace SBM_WebUI.mp
{
    public partial class SPReceive : System.Web.UI.Page
    {

        #region Local Variable
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_TRANSC_ID = "sTranscID";
        public const string OBJ_PAGE_ID = "sPageID";
        public const string SES_ROW_INDEX = "sRowIndex";
        public const string SEC_RE_DATA = "sesReOrderData";
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_TRANS.RECEIVE))
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

        private void InitializeData()
        {
            gvData.PageSize = (int)Constants.PAGING_UNAPPROVED;
            ClearDetailTextValue();
            gvReceiveDetail.DataSource = null;
            gvReceiveDetail.DataBind();
            // Issue set in session 
            if (Session[Constants.SES_RECEIVE] == null)
            {
                Receive oSesReceive = new Receive();
                Session.Add(Constants.SES_RECEIVE, oSesReceive);
            }
            else
            {
                Receive oSesReceive = new Receive();
                Session[Constants.SES_RECEIVE] = oSesReceive;
            }

            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlSPType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);

            string sTranID = Request.QueryString[OBJ_TRANSC_ID];
            string sPageID = Request.QueryString[OBJ_PAGE_ID];

            if (!string.IsNullOrEmpty(sTranID))
            {
                sTranID = oCrypManager.GetDecryptedString(sTranID, Constants.CRYPT_PASSWORD_STRING);
            }
            if (!string.IsNullOrEmpty(sPageID))
            {
                sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
            }

            if (!string.IsNullOrEmpty(sTranID) && !string.IsNullOrEmpty(sPageID))
            {
                string sOperationType = sPageID.Substring(4, 1);
                if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
                {
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    LoadDataByID(sTranID, "1");

                    // user Detail
                    Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);

                    EnableDisableControl(true);

                    #region User-Detail.
                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    oUserDetails.CheckerID = oConfig.UserName;
                    oUserDetails.CheckDate = DateTime.Now;
                    ucUserDet.UserDetail = oUserDetails;
                    #endregion User-Detail.                    
                }
            }
            else
            {
                
                EnableDisableControl(false);

                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                UserDetails oUserDetails = new UserDetails();
                oUserDetails.MakerID = oConfig.UserName;
                oUserDetails.MakeDate = DateTime.Now;
                ucUserDet.UserDetail = oUserDetails;
                
                LoadPreviousList();
            }
        }

        private void LoadPreviousList()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            ReceiveDAL oReceiveDAL = new ReceiveDAL();
            Result oResult = oReceiveDAL.LoadUnapprovedList(oConfig.UserName, false, oConfig.DivisionID, oConfig.BankCodeID);
            DataTable dtTmpList = (DataTable)oResult.Return;
            if (dtTmpList.Rows.Count > 0)
            {
                dtTmpList.Columns.Remove("Maker ID");

                gvData.DataSource = dtTmpList;
                gvData.DataBind();
            }
            else
            {
                gvData.DataSource = null;
                gvData.DataBind();
            }

            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;
        }

        public void SPReceiveLoadAction(string sTransactionNo, string sApprovalStaus)
        {
            this.LoadDataByID(sTransactionNo, sApprovalStaus);
            hdDataType.Value = sApprovalStaus;
        }


        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            ArrayList al = new ArrayList();
            al.Add(3);
            Util.GridDateFormat(e, gvData, null, al);
        }
        protected void gvReceiveDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvReceiveDetail, null, null);
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
                hdDataType.Value = string.Empty;
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                LoadDataByID(gvRow.Cells[1].Text, "1");
            }
        }


        protected void gvReceiveDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            string sButtonText = ((Button)e.CommandSource).CommandName;
            if (sButtonText.Equals("Select"))
            {
                hdDenom.Value = gvRow.Cells[2].Text;

                DDListUtil.Assign(ddlDenomination, gvRow.Cells[2].Text);
                txtSeries.Text = gvRow.Cells[3].Text;
                txtFrom.Text = gvRow.Cells[4].Text;
                txtTo.Text = gvRow.Cells[5].Text;
                txtQuantity.Text = gvRow.Cells[6].Text;
                txtDnmAmount.Text = gvRow.Cells[7].Text;
                txtCabinetNumber.Text = gvRow.Cells[8].Text;
                txtDrawerNumber.Text = gvRow.Cells[9].Text;

                Session[SES_ROW_INDEX] = gvRow.RowIndex;
            }
            else if (sButtonText.Equals("Delete"))
            {
                // need to delete previous data by Denomination
                Receive oReceive = (Receive)Session[Constants.SES_RECEIVE];
                if (oReceive.ReceiveDetailsList.Count > 0)
                {                    
                    if (gvRow.RowIndex != -1)
                    {
                        oReceive.ReceiveDetailsList.RemoveAt(gvRow.RowIndex);                        
                    }
                } 
                
                gvReceiveDetail.DataSource = null;
                gvReceiveDetail.DataBind();

                DataTable oDataTable = new DataTable("dtData");

                oDataTable.Columns.Add(new DataColumn("bfDenom", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfSeries", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfFrom", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfTo", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfQuantity", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfDnmAmount", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfCabinetNumber", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfDrawerNumber", typeof(string)));

                ReceiveDetails oReceiveDetails=null;
                DataRow row = null;
                if (oReceive.ReceiveDetailsList != null)
                {
                    if (oReceive.ReceiveDetailsList.Count > 0)
                    {
                        for (int i = 0; i < oReceive.ReceiveDetailsList.Count; i++)
                        {
                            oReceiveDetails = (ReceiveDetails)oReceive.ReceiveDetailsList[i];

                            row = oDataTable.NewRow();
                            row["bfDenom"] = oReceiveDetails.Denomination.DenominationID.ToString().ToUpper();
                            row["bfSeries"] = oReceiveDetails.SPSeries.ToString().ToUpper();
                            row["bfFrom"] = oReceiveDetails.SeriesFrom.ToString().ToUpper();
                            row["bfTo"] = oReceiveDetails.SeriesTo.ToString().ToUpper();
                            row["bfQuantity"] = oReceiveDetails.ReceiveQuantity.ToString().ToUpper();
                            int iTmp = oReceiveDetails.ReceiveQuantity * oReceiveDetails.Denomination.DenominationID;
                            row["bfDnmAmount"] = iTmp.ToString();
                            row["bfCabinetNumber"] = oReceiveDetails.CabinetNumber.ToString().ToUpper();
                            row["bfDrawerNumber"] = oReceiveDetails.DrawerNumber.ToString().ToUpper();

                            oDataTable.Rows.Add(row);
                        }
                        gvReceiveDetail.DataSource = oDataTable;
                        gvReceiveDetail.DataBind();
                    }
                }
                ddlDenomination.SelectedIndex = 0;
                txtCabinetNumber.Text = string.Empty;
                txtDrawerNumber.Text = string.Empty;
               
                
                Session[Constants.SES_RECEIVE] = oReceive;
            }
        }

        private void ClearDetailTextValue()
        {
            if (ddlDenomination.Items.Count > 0)
            {
                ddlDenomination.SelectedIndex = 0;
            }
            hdDataType.Value = string.Empty;
            txtSeries.Text = "";
            txtFrom.Text = "";
            txtTo.Text = "";
            txtQuantity.Text = "";
            txtDnmAmount.Text = "";
            hdDenom.Value = "";
            txtDnmAmount.Text = "";

            txtTransNo.Text = string.Empty;
            txtReceiveAmount.Text = string.Empty;

            Session[Constants.SES_RECEIVE] = null;
            Session[SES_ROW_INDEX] = null;

            txtReceiveDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
        }

        protected void btnReOrder_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlSPType.SelectedValue))
            {
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                Result oResult = new Result();
                ReceiveDAL oRecDal = new ReceiveDAL();
                oResult = oRecDal.StockDataInfo(ddlSPType.SelectedValue, oConfig.DivisionID, oConfig.BankCodeID);
                if (oResult.Status)
                {
                    VGD.SetData((DataTable)oResult.Return, "Re-Order notification Detail");
                }
            }
            else
            {
                VGD.SetData(null, "Re-Order notification Detail");
            }
        }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            EnableDisableControl(false);
            txtTransNo.Enabled = true;
            txtTransNo.Text = "";
            txtReceiveDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtReceiveAmount.Text = "";
            ddlSPType.SelectedIndex = 0;
            ClearDetailTextValue();

            Session[Constants.SES_RECEIVE] = null;
            Session[SES_ROW_INDEX] = null;

            gvReceiveDetail.DataSource = null;
            gvReceiveDetail.DataBind();

            txtSeries.Text = "";
            txtFrom.Text = "";
            txtTo.Text = "";
            txtQuantity.Text = "";
            txtDnmAmount.Text = "";
            txtCabinetNumber.Text = "";
            txtDrawerNumber.Text = "";
            ucUserDet.ResetData();
        }

        protected void btnResetReceiveDetails_Click(object sender, EventArgs e)
        {
            Session[SES_ROW_INDEX] = null;
            if (ddlDenomination.Items.Count > 0)
            {
                ddlDenomination.SelectedIndex = 0;
            }
            txtSeries.Text = "";
            txtFrom.Text = "";
            txtTo.Text = "";
            txtQuantity.Text = "";
            txtDnmAmount.Text = "";
            txtCabinetNumber.Text = "";
            txtDrawerNumber.Text = "";
        }

        private void LoadDataByID(string sTranID, string sApprovalStaus)
        {
            Receive oReceive = new Receive(sTranID);
            ReceiveDAL oReceiveDAL = new ReceiveDAL();
            Result oResult = new Result();
            oResult = oReceiveDAL.LoadByID(oReceive , sApprovalStaus);
            oReceive = (Receive)oResult.Return;       

            if (oResult.Status)
            {
                Session.Add(Constants.SES_RECEIVE, oReceive);
                
                txtTransNo.Text = oReceive.ReceiveTransNo.Trim();
                if (oReceive.ReceiveDate != null)
                {
                    txtReceiveDate.Text = oReceive.ReceiveDate.ToString(Constants.DATETIME_FORMAT);
                }
                else
                {
                    txtReceiveDate.Text = "";
                }
                txtReceiveAmount.Text = oReceive.ReceiveAmount.ToString("N2").Trim();
                DDListUtil.Assign( ddlSPType,oReceive.SPType.SPTypeID);
                StockInfoLoad(oReceive.SPType.SPTypeID);

                ucUserDet.UserDetail = oReceive.UserDetails;
                // to load Denomination
                if (oReceive.SPType.SPTypeID != null)
                {
                    LoadSPType();
                }

                hdReceiveTransNo.Value = sTranID;
                if (oReceive.ReceiveDetailsList != null)
                {
                    SetSPReceiveDetails(oReceive);
                }
                //if Approved
                if (sApprovalStaus.Equals("2"))
                {
                    EnableDisableControl(true);
                    Util.ControlEnabled(btnReject, false);
                    Util.ControlEnabled(btnApprove, false);
                    Util.ControlEnabled(btnBack, false);

                    Util.ControlEnabled(btnReset, true);
                    Util.ControlEnabled(btnSave, true);
                    Util.ControlEnabled(btnDelete, true);

                    btnSearch.Enabled = true;

                    fsList.Visible = true;
                }
                else
                {
                    EnableDisableControl(false);
                }
            }
            else
            {
                Session.Add(Constants.SES_RECEIVE, new Receive());
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
            }
        }

        private void EnableDisableControl(bool isApproved)
        {
            
            if (isApproved)
            {
                //Receive Details
               
                Util.ControlEnabled(txtReceiveDate, false);
                Util.ControlEnabled(ddlSPType, false);
                Util.ControlEnabled(txtReceiveAmount, false);
                
                //SP Receive Details List
                Util.ControlEnabled(ddlDenomination, false);
                Util.ControlEnabled(txtFrom, false);
                Util.ControlEnabled(txtTo, false);
                Util.ControlEnabled(txtDrawerNumber, false);
                Util.ControlEnabled(txtCabinetNumber, false);

                gvReceiveDetail.Enabled = false;

                // button 
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true);

                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);
               
                btnSearch.Enabled = false;
                btnAddReceiveDetails.Enabled = false;
                btnResetReceiveDetails.Enabled = false;

                fsList.Visible = false;
            }
            else
            {
                //Receive Details
                Util.ControlEnabled(txtReceiveDate, true);
                Util.ControlEnabled(ddlSPType, true);
                Util.ControlEnabled(txtReceiveAmount, true);

                //SP Receive Details List
                Util.ControlEnabled(ddlDenomination, true);
                Util.ControlEnabled(txtFrom, true);
                Util.ControlEnabled(txtTo, true);
                Util.ControlEnabled(txtDrawerNumber, true);
                Util.ControlEnabled(txtCabinetNumber, true);
               
                gvReceiveDetail.Enabled = true;
               
                // button 
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);

                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);

                btnSearch.Enabled = true;
                btnAddReceiveDetails.Enabled = true;
                btnResetReceiveDetails.Enabled = true;

                fsList.Visible = true;

                
            }
        }


        private void SetSPReceiveDetails(Receive oReceive)
        {
            gvReceiveDetail.DataSource = null;
            gvReceiveDetail.DataBind();

            DataTable oDataTable = new DataTable("dtData");

            oDataTable.Columns.Add(new DataColumn("bfDenom", typeof(string)));
            oDataTable.Columns.Add(new DataColumn("bfSeries", typeof(string)));
            oDataTable.Columns.Add(new DataColumn("bfFrom", typeof(string)));
            oDataTable.Columns.Add(new DataColumn("bfTo", typeof(string)));
            oDataTable.Columns.Add(new DataColumn("bfQuantity", typeof(string)));
            oDataTable.Columns.Add(new DataColumn("bfDnmAmount", typeof(string)));
            oDataTable.Columns.Add(new DataColumn("bfCabinetNumber", typeof(string)));
            oDataTable.Columns.Add(new DataColumn("bfDrawerNumber", typeof(string)));

            DataRow row = null;
            ReceiveDetails oReceiveDetails = null;
            if (oReceive.ReceiveDetailsList != null)
            {
                if (oReceive.ReceiveDetailsList.Count > 0)
                {
                    for (int i = 0; i < oReceive.ReceiveDetailsList.Count; i++)
                    {
                        oReceiveDetails = (ReceiveDetails)oReceive.ReceiveDetailsList[i];

                        row = oDataTable.NewRow();
                        row["bfDenom"] = oReceiveDetails.Denomination.DenominationID.ToString();
                        row["bfSeries"] = oReceiveDetails.SPSeries.ToString();
                        row["bfFrom"] = oReceiveDetails.SeriesFrom.ToString();
                        row["bfTo"] = oReceiveDetails.SeriesTo.ToString();
                        row["bfQuantity"] = oReceiveDetails.ReceiveQuantity.ToString();
                        int iTmp = oReceiveDetails.ReceiveQuantity * oReceiveDetails.Denomination.DenominationID;
                        row["bfDnmAmount"] = iTmp.ToString();
                        row["bfCabinetNumber"] = oReceiveDetails.CabinetNumber.ToString().ToUpper();
                        row["bfDrawerNumber"] = oReceiveDetails.DrawerNumber.ToString().ToUpper();

                        oDataTable.Rows.Add(row);
                    }
                    gvReceiveDetail.DataSource = oDataTable;
                    gvReceiveDetail.DataBind();
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
            }
        }

        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");

            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.RECEIVE).PadLeft(5, '0'), false);
            }
            else
            {
                // no action
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.RECEIVE).PadLeft(5, '0'), false);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            Receive oReceive = new Receive(hdReceiveTransNo.Value);
            ReceiveDAL oReceiveDAL = new ReceiveDAL();
            oReceive.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oReceiveDAL.Reject(oReceive);
            if (oResult.Status)
            {
                ucMessage.OpenMessage(Constants.MSG_SUCCESS_REJECT, Constants.MSG_TYPE_SUCCESS);
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_REJECT, Constants.MSG_TYPE_ERROR);
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            Receive oReceive = new Receive(hdReceiveTransNo.Value);
            oReceive.SPType.SPTypeID = ddlSPType.SelectedItem.Value;

            if (gvReceiveDetail.Rows.Count > 0)
            {
                ReceiveDetails oRd = null;
                foreach (GridViewRow row in gvReceiveDetail.Rows)
                {
                    oRd = new ReceiveDetails();

                    oRd.Denomination.DenominationID = Util.GetIntNumber(row.Cells[0].Text);
                    oRd.SPSeries = row.Cells[1].Text;
                    oRd.SeriesFrom = row.Cells[2].Text;
                    oRd.SeriesTo= row.Cells[3].Text;
                    oRd.ReceiveQuantity = Util.GetIntNumber(row.Cells[4].Text);

                    oReceive.ReceiveDetailsList.Add(oRd);
                }
            }
            
            ReceiveDAL oReceiveDAL = new ReceiveDAL();
            oReceive.UserDetails = ucUserDet.UserDetail;
            oReceive.ReceiveAmount = Convert.ToDecimal(txtReceiveAmount.Text);

            Result oResult = (Result)oReceiveDAL.Approve(oReceive);
            if (oResult.Status)
            {
                ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE, Constants.MSG_TYPE_SUCCESS);
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_APPROVE, Constants.MSG_TYPE_ERROR);
            }
        }

        private void SaveAction()
        {           
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                if (hdDataType.Value != "2")
                {
                    Receive oReceive = (Receive)Session[Constants.SES_RECEIVE];
                    //check equality of Amount
                    if (oReceive != null)
                    {
                        int iTotalAmount = oReceive.ReceiveDetailsList.Sum(r => r.Denomination.DenominationID * r.ReceiveQuantity);

                        if (iTotalAmount != Util.GetIntNumber(txtReceiveAmount.Text))
                        {
                            ucMessage.OpenMessage("Denomination amount & Receive Amount must be equal !!", Constants.MSG_TYPE_INFO);
                        }
                        else
                        {
                            oReceive.ReceiveTransNo = hdReceiveTransNo.Value == "" ? "-1" : hdReceiveTransNo.Value;
                            oReceive.SPType.SPTypeID = ddlSPType.SelectedItem.Value;
                            oReceive.ReceiveDate = Util.GetDateTimeByString(txtReceiveDate.Text);
                            oReceive.ReceiveAmount = Util.GetIntNumber(txtReceiveAmount.Text);
                            oReceive.SPType.SPTypeID = ddlSPType.SelectedItem.Value;

                            oReceive.UserDetails = ucUserDet.UserDetail;
                            oReceive.UserDetails.MakerID = oConfig.UserName;
                            oReceive.UserDetails.MakeDate = DateTime.Now;


                            ucUserDet.ResetData();

                            ReceiveDAL oReceiveDAL = new ReceiveDAL();
                            Result oResult = (Result)oReceiveDAL.Save(oReceive);

                            if (oResult.Status)
                            {
                                InitializeData();
                                ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
                                ucUserDet.ResetData();
                            }
                            else
                            {
                                ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
                            }
                        }
                    }
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_APPROVED_SAVE_DATA, Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!hdDataType.Value.Equals("2"))
            {
                ReceiveDAL oReceiveDAL = new ReceiveDAL();
                Result oResult = (Result)oReceiveDAL.Detete(hdReceiveTransNo.Value);
                if (oResult.Status)
                {
                    LoadPreviousList();

                    ClearDetailTextValue();
                    gvReceiveDetail.DataSource = null;
                    gvReceiveDetail.DataBind();

                    txtTransNo.Text = string.Empty;
                    txtReceiveDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
                    ddlSPType.SelectedIndex = 0;
                    ddlDenomination.Items.Clear();
                    txtReceiveAmount.Text = string.Empty;
                    txtDrawerNumber.Text = string.Empty;
                    txtCabinetNumber.Text = string.Empty;

                    hdReceiveTransNo.Value = string.Empty;
                    hdDenom.Value = string.Empty;

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
                ucMessage.OpenMessage(Constants.MSG_APPROVED_DELETE_DATA, Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }


        protected void btnAddReceiveDetails_Click(object sender, EventArgs e)
        {
            Result oResult = new Result();
            ReceiveDAL oRDal = new ReceiveDAL();
            Receive oReceive = (Receive)Session[Constants.SES_RECEIVE];
            string sTrnsNo = string.Empty;
            if (oReceive != null)
            {
                sTrnsNo = oReceive.ReceiveTransNo;
            }

            oResult = oRDal.IsExist(ddlSPType.SelectedValue, ddlDenomination.SelectedValue, txtSeries.Text, txtFrom.Text, txtTo.Text);

            if (oResult.Status)
            {
                int iCount=(int)oResult.Return;
                if (iCount == 0)
                {
                    
                    if (oReceive != null)
                    {
                        AddRDetailToSession(oReceive);
                    }
                    else
                    {
                        AddRDetailToSession(new Receive());
                    }
                }
                else
                {
                    ucMessage.OpenMessage("This series already exists!!", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
        }

        private void AddRDetailToSession(Receive oReceive)
        {
            if (Session[SES_ROW_INDEX] != null)
            {
                oReceive.ReceiveDetailsList.RemoveAt((int)Session[SES_ROW_INDEX]);
                Session[SES_ROW_INDEX] = null;
            }
            // (New Denomination == Old Denomination) && 
            // (Old Series == New Series) && 
            // ( New-From point in || New-To point in || 
            // Old-From point in || Old-To point in )  
            // != Incorrect Format 
            ReceiveDetails oReceiveDetailExist = oReceive.ReceiveDetailsList.Where(c => c.Denomination.DenominationID.Equals(Util.GetIntNumber(ddlDenomination.SelectedItem.Value)) && c.SPSeries.Equals(txtSeries.Text.Trim())
                   && (
                        ( // New-From point in
                            Util.GetIntNumber(txtFrom.Text) >= Util.GetIntNumber(c.SeriesFrom)
                            &&
                            Util.GetIntNumber(txtFrom.Text) <= Util.GetIntNumber(c.SeriesTo)
                        )
                     || ( // New-To point in
                            Util.GetIntNumber(txtTo.Text) >= Util.GetIntNumber(c.SeriesFrom)
                            &&
                            Util.GetIntNumber(txtTo.Text) <= Util.GetIntNumber(c.SeriesTo)
                        )
                     || ( // Old-From point in
                            Util.GetIntNumber(txtFrom.Text) <= Util.GetIntNumber(c.SeriesFrom)
                            &&
                            Util.GetIntNumber(txtTo.Text) >= Util.GetIntNumber(c.SeriesFrom)
                        )
                     || ( // Old-To point in
                            Util.GetIntNumber(txtFrom.Text) <= Util.GetIntNumber(c.SeriesTo)
                            &&
                            Util.GetIntNumber(txtTo.Text) >= Util.GetIntNumber(c.SeriesTo)
                        )
                    )
                    ).SingleOrDefault();


            if (oReceiveDetailExist != null)
            {
                ucMessage.OpenMessage("Inconsistent data !!", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
            else
            {
                // add to list as a new data
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                
                
                int iExistTotalAmount = oReceive.ReceiveDetailsList.Sum(r => r.Denomination.DenominationID * r.ReceiveQuantity);
                if (Util.GetDecimalNumber(iExistTotalAmount.ToString()) + Util.GetDecimalNumber(txtDnmAmount.Text) > Util.GetIntNumber(txtReceiveAmount.Text))
                {
                    ucMessage.OpenMessage("Denomination amount & Receive Amount must be equal !!", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);

                    return;
                }
                ReceiveDetails oReceiveDetails = new ReceiveDetails();
                oReceiveDetails.SPSeries = txtSeries.Text.Trim().ToUpper();
                oReceiveDetails.SeriesFrom = txtFrom.Text.Trim().PadLeft(Convert.ToInt32(hdDigitsInSlNo.Value), '0').ToUpper();
                oReceiveDetails.SeriesTo = txtTo.Text.Trim().PadLeft(Convert.ToInt32(hdDigitsInSlNo.Value), '0').ToUpper();
                oReceiveDetails.CabinetNumber = Util.GetIntNumber(txtCabinetNumber.Text);
                oReceiveDetails.DrawerNumber = Util.GetIntNumber(txtDrawerNumber.Text);

                oReceiveDetails.ReceiveQuantity = Util.GetIntNumber(txtQuantity.Text);
                oReceiveDetails.Denomination.DenominationID = Util.GetIntNumber(ddlDenomination.SelectedItem.Value);
                oReceiveDetails.UserDetails = ucUserDet.UserDetail;
                oReceiveDetails.UserDetails.MakerID = oConfig.UserName;

                oReceive.ReceiveDetailsList.Add(oReceiveDetails);


                gvReceiveDetail.DataSource = null;
                gvReceiveDetail.DataBind();

                DataTable oDataTable = new DataTable("dtData");

                oDataTable.Columns.Add(new DataColumn("bfDenom", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfSeries", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfFrom", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfTo", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfQuantity", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfDnmAmount", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfCabinetNumber", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfDrawerNumber", typeof(string)));

                DataRow row = null;
                if (oReceive.ReceiveDetailsList != null)
                {
                    if (oReceive.ReceiveDetailsList.Count > 0)
                    {
                        for (int i = 0; i < oReceive.ReceiveDetailsList.Count; i++)
                        {
                            oReceiveDetails = (ReceiveDetails)oReceive.ReceiveDetailsList[i];

                            row = oDataTable.NewRow();
                            row["bfDenom"] = oReceiveDetails.Denomination.DenominationID.ToString().ToUpper();
                            row["bfSeries"] = oReceiveDetails.SPSeries.ToString().ToUpper();
                            row["bfFrom"] = oReceiveDetails.SeriesFrom.ToString().ToUpper();
                            row["bfTo"] = oReceiveDetails.SeriesTo.ToString().ToUpper();
                            row["bfQuantity"] = oReceiveDetails.ReceiveQuantity.ToString().ToUpper();
                            int iTmp = oReceiveDetails.ReceiveQuantity * oReceiveDetails.Denomination.DenominationID;
                            row["bfDnmAmount"] = iTmp.ToString();
                            row["bfCabinetNumber"] = oReceiveDetails.CabinetNumber.ToString().ToUpper();
                            row["bfDrawerNumber"] = oReceiveDetails.DrawerNumber.ToString().ToUpper();

                            oDataTable.Rows.Add(row);
                        }
                        gvReceiveDetail.DataSource = oDataTable;
                        gvReceiveDetail.DataBind();
                    }
                }

                Session[Constants.SES_RECEIVE] = oReceive;

                txtSeries.Text = "";
                txtFrom.Text = "";
                txtTo.Text = "";
                txtQuantity.Text = "";
                txtDnmAmount.Text = "";
                txtDrawerNumber .Text = "";
                txtCabinetNumber.Text = "";
                if (ddlDenomination.Items.Count > 0)
                {
                    ddlDenomination.SelectedIndex = 0;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveAction();
        }

        private void LoadSPType()
        {
            SPTypeDAL oSPTypeDAL = new SPTypeDAL();
            Result oResult = (Result)oSPTypeDAL.GetDDLDenomList(ddlSPType.SelectedValue);
            if (oResult.Status)
            {
                ddlDenomination.Items.Clear();
                DataTable dtGetDenomID = (DataTable)oResult.Return;
                if (dtGetDenomID.Rows.Count > 0)
                {
                    DDListUtil.Assign(ddlDenomination, dtGetDenomID, true);

                    txtSeries.Text = string.Empty;
                    txtFrom.Text = string.Empty;
                    txtTo.Text = string.Empty;
                    txtQuantity.Text = string.Empty;                    
                    txtDnmAmount.Text = string.Empty;
                    txtDrawerNumber.Text = string.Empty;
                    txtCabinetNumber.Text = string.Empty;

                    gvReceiveDetail.DataSource = null;
                    gvReceiveDetail.DataBind();
                }
            }
        }

        protected void ddlDenomination_SelectedIndexChanged(object sender, EventArgs e)
        {
            SPTypeDAL oSPTypeDAL = new SPTypeDAL();
            if(!string.IsNullOrEmpty(ddlDenomination.SelectedValue))
            {
                Result oResult = (Result)oSPTypeDAL.GetSeriesName(ddlSPType.SelectedValue, ddlDenomination.SelectedValue);
                if (oResult.Status)
                {
                    DataTable dtSeries = (DataTable)oResult.Return;
                    if (dtSeries.Rows.Count > 0)
                    {
                        txtSeries.Text = Convert.ToString(dtSeries.Rows[0]["SPSeries"]);
                        hdDigitsInSlNo.Value = Convert.ToString(dtSeries.Rows[0]["DigitsInSlNo"]);
                    }

                }
            }
        }

        protected void ddlSPType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session[Constants.SES_RECEIVE] = null;
            LoadSPType();
            txtReceiveAmount.Text = string.Empty;
            StockInfoLoad(ddlSPType.SelectedValue);

            // to show Message based on Re-Order
            StringBuilder sb = new StringBuilder();
            string sHeader = "<table width='100%' cellspacing='0' cellpadding='5' border='1' style='color: Black; background-color: #E9F0D5; border-color: #C1D586; border-width: 1px; border-style: Solid; width: 98%; border-collapse: collapse;'><tr style='color: Black; background-color: #D1D3CF; font-size: 12px; font-weight: normal; white-space: nowrap;'><th style='scope='col'>Denomination</th><th style='scope='col'>Remaining Demomination</th><th style='scope='col'>Re Order Level</th></tr>";

            if (Session[SEC_RE_DATA] != null)
            {
                DataTable dt = (DataTable)Session[SEC_RE_DATA];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    
                    // GridViewRow dr = (GridViewRow)gvStock.Rows[i];
                    if (Util.GetIntNumber(dt.Rows[i]["Remaining Demomination"].ToString()) < Util.GetIntNumber(dt.Rows[i]["Re Order Level"].ToString()))
                    {
                        sb.Append("<tr><td align='left'>").Append(dt.Rows[i]["Denomination"].ToString()).Append("</td><td align='left'>").Append(dt.Rows[i]["Remaining Demomination"].ToString()).Append("</td><td align='left'>").Append(dt.Rows[i]["Re Order Level"].ToString()).Append("</td></tr>");
                     }
                }
            }
            if (sb.Length > 0)
            {
                ucMessage.OpenMessage(sHeader + sb.ToString() + "</table>", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }

        private void StockInfoLoad(string sTypeID)
        {            
            Session[SEC_RE_DATA] = null;
            if (!string.IsNullOrEmpty(sTypeID))
            {
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                Result oResult = new Result();
                ReceiveDAL oRecDal = new ReceiveDAL();
                oResult = oRecDal.StockDataInfo(ddlSPType.SelectedValue, oConfig.DivisionID, oConfig.BankCodeID);
                if (oResult.Status)
                {
                    Session[SEC_RE_DATA] = (DataTable)oResult.Return;                    
                }
            }
        }

        protected void gvReceiveDetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
           //To handle aspx page exception
        }

        protected void btnViewJournals_Click(object sender, EventArgs e)
        {
            
        }
    }
}

