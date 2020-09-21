using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
//add
using SBM_BLC1.Common;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Configuration;
using SBM_BLC1.Entity.Transaction;
using SBM_BLC1.Transaction;
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Entity.JournalRecon;
using SBM_BLC1.DAL.JournalRecon;
using System.Collections;

namespace SBM_WebUI.mp
{
    public partial class AddJournalManual : System.Web.UI.Page
    {
        #region Local Variable
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_REG_NO = "sRegNo";
        public const string OBJ_PAGE_ID = "sPageID";
        public static string Action_flag = string.Empty;

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
                    //if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_JOURNAL_REC.))
                    //{
                    //    Response.Redirect(Constants.PAGE_ERROR, false);
                    //}
                    //End Of Page Permission
                }
            }
            else
            {
                Response.Redirect(Constants.PAGE_LOGIN, false);
            }

        }
        protected void InitializeBuffTable()
        {
            AccountReconciliation oAccountReconciliation = new AccountReconciliation();
            DataTable dtAccRecon = new DataTable();

            dtAccRecon.Columns.Add(new DataColumn("ValueDate", typeof(string)));//[ValueDate] [datetime] NOT NULL,
            dtAccRecon.Columns.Add(new DataColumn("SeqNo", typeof(int)));//[SeqNo] [int] NOT NULL,
            //2
            dtAccRecon.Columns.Add(new DataColumn("AccountNo", typeof(string)));//[AccountNo] [varchar](13) NOT NULL,
            dtAccRecon.Columns.Add(new DataColumn("ReferenceNo", typeof(string)));//[ReferenceNo] [varchar](20) NULL,
            dtAccRecon.Columns.Add(new DataColumn("SPTypeID", typeof(string)));//[SPTypeID] [char](3) NULL,
            dtAccRecon.Columns.Add(new DataColumn("Narration", typeof(string)));//[Narration] [varchar](100) NOT NULL,
            dtAccRecon.Columns.Add(new DataColumn("CurrencyID", typeof(string)));//[CurrencyID] [char](2) NOT NULL,
            dtAccRecon.Columns.Add(new DataColumn("Amount", typeof(decimal)));//[Amount] [decimal](24, 2) NOT NULL,
            dtAccRecon.Columns.Add(new DataColumn("JournalType", typeof(int)));//[JournalType] [smallint] NULL,
            dtAccRecon.Columns.Add(new DataColumn("DrCr", typeof(string)));//[DrCr] [char](1) NOT NULL,

            //common for bunch of record
            dtAccRecon.Columns.Add(new DataColumn("IsReconciled", typeof(int)));//[IsReconciled] [bit] NOT NULL,
            dtAccRecon.Columns.Add(new DataColumn("ReceiveDate", typeof(string)));//[ReceiveDate] [datetime] NULL,
            dtAccRecon.Columns.Add(new DataColumn("IsManual", typeof(int)));//[IsManual] [bit] NOT NULL,
            dtAccRecon.Columns.Add(new DataColumn("ManualChecker", typeof(string)));//[ManualChecker] [varchar](20) NULL,
            dtAccRecon.Columns.Add(new DataColumn("DownLoadBy", typeof(string)));//[DownLoadBy] [varchar](20) NOT NULL,
            dtAccRecon.Columns.Add(new DataColumn("ReconBy", typeof(string)));//[ReconBy] [varchar](20) NULL,


            dtAccRecon.Columns.Add(new DataColumn("DivisionID", typeof(string)));//[DivisionID] [char](3) NOT NULL,
            dtAccRecon.Columns.Add(new DataColumn("BankID", typeof(string)));//[BankID] [char](3) NULL,
            dtAccRecon.Columns.Add(new DataColumn("MakerID", typeof(string)));//[MakerID] [varchar](20) NULL,
            dtAccRecon.Columns.Add(new DataColumn("MakeDate", typeof(string)));//[MakeDate] [datetime] NULL,
            //[CheckerID] [varchar](20) NULL,
            //[CheckDate] [datetime] NULL,
            //[CheckerComment] [varchar](max) NULL,
            dtAccRecon.Columns.Add(new DataColumn("ActionFlag", typeof(string)));


            oAccountReconciliation.DtAccountRec = dtAccRecon.Clone();
            Session[Constants.SES_ACC_RECON] = oAccountReconciliation;
           
        }
        #region InitializeData
        private void InitializeData()
        {
            //hdRegNo.Value = "";
            string sRegNo = Request.QueryString[OBJ_REG_NO];
            string sPageID = Request.QueryString[OBJ_PAGE_ID];

            //Temporary table creation
            this.InitializeBuffTable();

            //Controls initialize
            DDListUtil.LoadDDLFromDB(ddlSPType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            DDListUtil.LoadDDLFromDB(ddlMaker, "UserName", "UserName", "SA_User", false);
            DDListUtil.LoadDDLFromXML(ddlAccountNo, "JournalAccount", "Type", "JournalAccount", false);
            DDListUtil.LoadDDLFromDB(ddlCurrency, "CurrencyID", "CurrencyCode", "SPMS_Currency", true);

            txtTransDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            if (true)
            {
                //sRegNo = oCrypManager.GetDecryptedString(sRegNo, Constants.CRYPT_PASSWORD_STRING);
                //sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);

                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

                LoadUnapprovedAccRecon(oConfig.UserName, oConfig.DivisionID, oConfig.BankCodeID);

                #region User-Detail.
                UserDetails oUserDetails = new UserDetails();
                oUserDetails.MakerID = oConfig.UserName;
                oUserDetails.MakeDate = DateTime.Now;
                ucUserDet.UserDetail = oUserDetails;
                #endregion User-Detail.

                #region Enable-disable controls
                this.EnableDisableControls(false);
                #endregion Enable-disable controls
            }
            else
            {
                //Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                //UserDetails oUserDetails = new UserDetails();
                //oUserDetails.MakerID = oConfig.UserName;
                //oUserDetails.MakeDate = DateTime.Now;
                //ucUserDet.UserDetail = oUserDetails;
                //EnableDisableControl
            }
        }
        private void EnableDisableControls(bool isEnabled)
        {
            if (isEnabled)
            {
                txtTotDrAmt.Enabled = true;
                txtTotCrAmt.Enabled = true;
                txtNoOfDrTrans.Enabled = true;
                txtNoOfCrTrans.Enabled = true;
            }
            else
            {
                txtTotDrAmt.Enabled = false;
                txtTotCrAmt.Enabled = false;
                txtNoOfDrTrans.Enabled = false;
                txtNoOfCrTrans.Enabled = false;
            }
        }
        protected void LoadUnapprovedAccRecon(string UserName, string DivisionID, string BankCodeID)
        {
            AccountReconciliation oAccountReconciliation = new AccountReconciliation();
            JournalDAL oJournalDAL = new JournalDAL();
            DataTable dtTmp = ((AccountReconciliation)Session[Constants.SES_ACC_RECON]).DtAccountRec.Clone();
            oAccountReconciliation = (AccountReconciliation)oJournalDAL.LoadUnapprovedManualAcctJournalList(string.Empty,UserName,null,null,DivisionID, BankCodeID).Return;


            foreach (DataRow dr in oAccountReconciliation.DtAccountRec.Rows)
            {
                DataRow rowAccRecon = dtTmp.NewRow();
                rowAccRecon["ValueDate"] = dr["ValueDate"].ToString();
                rowAccRecon["SeqNo"] = int.Parse(dr["SeqNo"].ToString());
                rowAccRecon["AccountNo"] = dr["AccountNo"].ToString();
                rowAccRecon["SPTypeID"] = dr["SPTypeID"].ToString();
                rowAccRecon["Narration"] = dr["Narration"].ToString();
                rowAccRecon["CurrencyID"] = dr["CurrencyID"].ToString();
                rowAccRecon["Amount"] = decimal.Parse(dr["Amount"].ToString());
                rowAccRecon["JournalType"] = 0;
                rowAccRecon["DrCr"] = dr["DrCr"].ToString();
                //[DivisionID] [char](3) NOT NULL,
                //[BankID] [char](3) NULL,
                //[MakerID] [varchar](20) NULL,
                //[MakeDate] [datetime] NULL,
                //[CheckerID] [varchar](20) NULL,
                //[CheckDate] [datetime] NULL,
                //[CheckerComment] [varchar](max) NULL,
                rowAccRecon["DivisionID"] = (dr["DivisionID"].ToString() == string.Empty || dr["DivisionID"] == null ? string.Empty : dr["DivisionID"].ToString().Trim());
                rowAccRecon["BankID"] = (dr["BankID"].ToString() == string.Empty || dr["BankID"] == null ? string.Empty : dr["BankID"].ToString().Trim());
                rowAccRecon["MakerID"] = (dr["MakerID"].ToString() == string.Empty || dr["MakerID"] == null ? string.Empty : dr["MakerID"].ToString().Trim());
                rowAccRecon["MakeDate"] = (dr["MakeDate"].ToString() == string.Empty || dr["MakeDate"] == null ? string.Empty : dr["MakeDate"].ToString().Trim());
                rowAccRecon["ActionFlag"] = "NIL";

                dtTmp.Rows.Add(rowAccRecon);
            }
            //Store template table(null) in session
            ((AccountReconciliation)Session[Constants.SES_ACC_RECON]).DtAccountRec = dtTmp.Copy();
            //Set Views
            this.SetView(dtTmp);

        }
        #endregion InitializeData
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            AccountReconciliation oAccountReconciliation = new AccountReconciliation();

            DataTable dtAccRecon = null;
            DataTable DtGvAccRecon = new DataTable();
            if (Session[Constants.SES_ACC_RECON] != null)
            {
                dtAccRecon = ((AccountReconciliation)Session[Constants.SES_ACC_RECON]).DtAccountRec;
            }

            if (true)
            {
                DateTime parsedDate;
                string[] s = Action_flag.Split(':');
                if (s.Length == 2 && s[0] == "Edit")
                {
                    dtAccRecon.Rows[int.Parse(s[1])]["SPTypeID"] = ddlSPType.SelectedItem.Value;
                    dtAccRecon.Rows[int.Parse(s[1])]["AccountNo"] = ddlAccountNo.SelectedItem.Value.Trim();
                    dtAccRecon.Rows[int.Parse(s[1])]["Narration"] = txtNarration.Text.Trim();
                    dtAccRecon.Rows[int.Parse(s[1])]["DrCr"] = ddlDrCr.SelectedItem.Value;
                    dtAccRecon.Rows[int.Parse(s[1])]["CurrencyID"] = ddlCurrency.SelectedItem.Value;
                    dtAccRecon.Rows[int.Parse(s[1])]["Amount"] = decimal.Parse(txtAmount.Text.Trim());
                    dtAccRecon.Rows[int.Parse(s[1])]["JournalType"] = 0;
                    DateTime.TryParseExact(txtTransDate.Text, Constants.DateTimeFormats, null, System.Globalization.DateTimeStyles.None, out parsedDate);
                    dtAccRecon.Rows[int.Parse(s[1])]["ReceiveDate"] = parsedDate.ToString(Constants.DATETIME_dd_MMM_yyyy);
                    dtAccRecon.Rows[int.Parse(s[1])]["ActionFlag"] = "UPDATE";

                    Action_flag = string.Empty;
                }
                else
                {
                    DataRow rowAccRecon = dtAccRecon.NewRow();
                    DateTime.TryParseExact(txtTransDate.Text, Constants.DateTimeFormats, null, System.Globalization.DateTimeStyles.None, out parsedDate);
                    rowAccRecon["ValueDate"] = parsedDate.ToString(Constants.DATETIME_dd_MMM_yyyy);
                    rowAccRecon["SPTypeID"] = ddlSPType.SelectedItem.Value;
                    rowAccRecon["AccountNo"] = ddlAccountNo.SelectedItem.Value.Trim();
                    rowAccRecon["Narration"] = txtNarration.Text.Trim();
                    rowAccRecon["DrCr"] = ddlDrCr.SelectedItem.Value;
                    rowAccRecon["CurrencyID"] = ddlCurrency.SelectedItem.Value;
                    rowAccRecon["Amount"] = decimal.Parse(txtAmount.Text.Trim());
                    rowAccRecon["JournalType"] = 0;
                    DateTime.TryParseExact(txtTransDate.Text, Constants.DateTimeFormats, null, System.Globalization.DateTimeStyles.None, out parsedDate);
                    rowAccRecon["ReceiveDate"] = parsedDate.ToString(Constants.DATETIME_dd_MMM_yyyy);
                    rowAccRecon["ActionFlag"] = "INSERT";
                    dtAccRecon.Rows.Add(rowAccRecon);
                }
                //Store in Session
                ((AccountReconciliation)Session[Constants.SES_ACC_RECON]).DtAccountRec = dtAccRecon;
                this.SetView(dtAccRecon);
            }
            else
            {

            }
        }
        protected void gvAccRecon_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //get the row
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            //gvRow.BackColor = Color.Blue;

            if (((Button)e.CommandSource).Text.Equals("Edit"))
            {
                Action_flag = "Edit:" + gvRow.RowIndex;
                DateTime parsedDate;
                DataTable dtTmp = ((AccountReconciliation)Session[Constants.SES_ACC_RECON]).DtAccountRec;
                DateTime.TryParseExact(dtTmp.Rows[gvRow.RowIndex]["ValueDate"].ToString(), Constants.DateTimeFormats, null, System.Globalization.DateTimeStyles.None, out parsedDate);
                txtTransDate.Text = parsedDate.ToString("dd-MMM-yyyy");
                ddlSPType.SelectedIndex = ddlSPType.Items.IndexOf(ddlSPType.Items.FindByValue(dtTmp.Rows[gvRow.RowIndex]["SPTypeID"].ToString()));
                ddlAccountNo.SelectedIndex = ddlAccountNo.Items.IndexOf(ddlAccountNo.Items.FindByValue(dtTmp.Rows[gvRow.RowIndex]["AccountNo"].ToString()));
                txtNarration.Text = dtTmp.Rows[gvRow.RowIndex]["Narration"].ToString();
                //ddlMaker.SelectedIndex = ddlMaker.Items.IndexOf(ddlMaker.Items.FindByValue(dtTmp.Rows[gvRow.RowIndex]["ManualChecker"].ToString()));
                ddlDrCr.SelectedIndex = ddlDrCr.Items.IndexOf(ddlDrCr.Items.FindByValue(dtTmp.Rows[gvRow.RowIndex]["DrCr"].ToString()));
                ddlCurrency.SelectedIndex = ddlCurrency.Items.IndexOf(ddlCurrency.Items.FindByValue(dtTmp.Rows[gvRow.RowIndex]["CurrencyID"].ToString()));
                txtAmount.Text = dtTmp.Rows[gvRow.RowIndex]["Amount"].ToString();
            }
            else if (((Button)e.CommandSource).Text.Equals("Remove"))
            {
                DataTable dtMain = ((AccountReconciliation)Session[Constants.SES_ACC_RECON]).DtAccountRec;
                if (dtMain.Rows.Count > 0)
                {
                    dtMain.Rows[gvRow.RowIndex]["ActionFlag"] = "DELETE";
                    //Set Values
                    this.SetView(dtMain);
                }
            }
            else if (((Button)e.CommandSource).Text.Equals("Undo"))
            {
                DataTable dtMain = ((AccountReconciliation)Session[Constants.SES_ACC_RECON]).DtAccountRec;
                if (dtMain.Rows.Count > 0)
                {
                    dtMain.Rows[gvRow.RowIndex]["ActionFlag"] = "NIL";
                    //Set Values
                    this.SetView(dtMain);
                }
            }
        }
        protected void SetView(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                TemplateBuilder tbGvData = new TemplateBuilder();
                tbGvData.AppendLiteralString("No pending record found!");
                gvAccRecon.EmptyDataTemplate = tbGvData;
                gvAccRecon.DataBind();
            }
            else if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    DataView dv = new DataView(dt);
                    //dv.RowFilter = "ActionFlag<>'DELETE'";
                    gvAccRecon.DataSource = dv;
                    gvAccRecon.DataBind();
                    int i = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        string s = dr["ActionFlag"].ToString().Trim();
                        if (s == "DELETE")
                        {
                            ((Button)gvAccRecon.Rows[i].FindControl("btnEdit")).Enabled = false;
                            ((Button)gvAccRecon.Rows[i].FindControl("btnRemove")).Text = "Undo";
                            //((Button)gvAccRecon.Rows[i].FindControl("btnRemove")).Enabled = false;
                        }
                        i++;
                    }
                    //Set Values
                    object obj = new object();
                    if (dt.Select("DrCr='D'").Length > 0)
                    {
                        txtNoOfDrTrans.Text = dt.Select("DrCr='D' AND ActionFlag<>'DELETE'").Length.ToString();
                        obj = dt.Compute("SUM(Amount)", "DrCr='D' AND ActionFlag<>'DELETE'");
                        txtTotDrAmt.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0";
                    }
                    else
                    {
                        txtNoOfDrTrans.Text = "0";
                        txtTotDrAmt.Text = "0";

                    }
                    if (dt.Select("DrCr='C'").Length > 0)
                    {
                        txtNoOfCrTrans.Text = dt.Select("DrCr='C' AND ActionFlag<>'DELETE'").Length.ToString();
                        obj = dt.Compute("SUM(Amount)", "DrCr='C' AND ActionFlag<>'DELETE'");
                        txtTotCrAmt.Text = Convert.ToDecimal(obj != DBNull.Value ? obj : "0").ToString();
                    }
                    else
                    {
                        txtNoOfCrTrans.Text = "0";
                        txtTotCrAmt.Text = "0";
                    }
                }
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            AccountReconciliation oAccountReconciliation = new AccountReconciliation();
            JournalDAL oJournalDAL = new JournalDAL();
            oAccountReconciliation = (AccountReconciliation)Session[Constants.SES_ACC_RECON];
            oAccountReconciliation.UserDetails = ucUserDet.UserDetail;
            if (oJournalDAL.SaveAccJournal(oAccountReconciliation).Status)
            {
                ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);

            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
            }
            //finally do
            ((AccountReconciliation)Session[Constants.SES_ACC_RECON]).DtAccountRec.Clear();
            this.SetView(null);
            //after
            ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);


        }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            Action_flag = string.Empty;
            ddlSPType.SelectedIndex = 0;
            txtTransDate.Text = string.Empty;
            ddlAccountNo.SelectedIndex = 0;
            txtNarration.Text = string.Empty;
            //ddlMaker.SelectedIndex = 0;
            ddlDrCr.SelectedIndex = 0;
            ddlCurrency.SelectedIndex = 0;
            txtAmount.Text = string.Empty;
        }
        public void PopErrorMsgAction(string sType)
        {
            if (sType.Equals(Constants.MSG_TYPE_INFO))
            {
                // no action
            }
            else
            {
                // no action
            }
        }
    }
}
