//Jakir 20121213 SECL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
//add
using SPMS_Business.Common;
using SPMS_Business.Entity.Common;
using SPMS_Business.Configuration;
using SPMS_Business.Entity.Transaction;
using SPMS_Business.Transaction;
using SPMS_Business.Entity.Configuration;
using SPMS_Business.Entity.JournalRecon;
using SPMS_Business.DAL.JournalRecon;
using System.Collections;

namespace SPMS_Web.mp
{
    public partial class JournalEntryApprove : System.Web.UI.Page
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
                    //if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_JOURNAL_RECON.JOURNAL_REC_APPROVE))
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
        protected void InitializeTmpTable()
        {
            AccountReconciliation oAccountReconciliation = new AccountReconciliation();
            DataTable dtAccountReconciliation = new DataTable();

            dtAccountReconciliation.Columns.Add(new DataColumn("ValueDate", typeof(string)));//[ValueDate] [datetime] NOT NULL,
            dtAccountReconciliation.Columns.Add(new DataColumn("SeqNo", typeof(int)));//[SeqNo] [int] NOT NULL,
            //2
            dtAccountReconciliation.Columns.Add(new DataColumn("AccountNo", typeof(string)));//[AccountNo] [varchar](13) NOT NULL,
            dtAccountReconciliation.Columns.Add(new DataColumn("ReferenceNo", typeof(string)));//[ReferenceNo] [varchar](20) NULL,
            dtAccountReconciliation.Columns.Add(new DataColumn("SPTypeID", typeof(string)));//[SPTypeID] [char](3) NULL,
            dtAccountReconciliation.Columns.Add(new DataColumn("Narration", typeof(string)));//[Narration] [varchar](100) NOT NULL,
            dtAccountReconciliation.Columns.Add(new DataColumn("CurrencyID", typeof(string)));//[CurrencyID] [char](2) NOT NULL,
            dtAccountReconciliation.Columns.Add(new DataColumn("Amount", typeof(decimal)));//[Amount] [decimal](24, 2) NOT NULL,
            dtAccountReconciliation.Columns.Add(new DataColumn("JournalType", typeof(int)));//[JournalType] [smallint] NULL,
            dtAccountReconciliation.Columns.Add(new DataColumn("DrCr", typeof(string)));//[DrCr] [char](1) NOT NULL,

            //common for bunch of record
            dtAccountReconciliation.Columns.Add(new DataColumn("IsReconciled", typeof(int)));//[IsReconciled] [bit] NOT NULL,
            dtAccountReconciliation.Columns.Add(new DataColumn("ReceiveDate", typeof(string)));//[ReceiveDate] [datetime] NULL,
            dtAccountReconciliation.Columns.Add(new DataColumn("IsManual", typeof(int)));//[IsManual] [bit] NOT NULL,
            dtAccountReconciliation.Columns.Add(new DataColumn("ManualChecker", typeof(string)));//[ManualChecker] [varchar](20) NULL,
            dtAccountReconciliation.Columns.Add(new DataColumn("DownLoadBy", typeof(string)));//[DownLoadBy] [varchar](20) NOT NULL,
            dtAccountReconciliation.Columns.Add(new DataColumn("ReconBy", typeof(string)));//[ReconBy] [varchar](20) NULL,


            dtAccountReconciliation.Columns.Add(new DataColumn("DivisionID", typeof(string)));//[DivisionID] [char](3) NOT NULL,
            dtAccountReconciliation.Columns.Add(new DataColumn("BankID", typeof(string)));//[BankID] [char](3) NULL,
            dtAccountReconciliation.Columns.Add(new DataColumn("MakerID", typeof(string)));//[MakerID] [varchar](20) NULL,
            dtAccountReconciliation.Columns.Add(new DataColumn("MakeDate", typeof(string)));//[MakeDate] [datetime] NULL,
            //[CheckerID] [varchar](20) NULL,
            //[CheckDate] [datetime] NULL,
            //[CheckerComment] [varchar](max) NULL,
            dtAccountReconciliation.Columns.Add(new DataColumn("ActionFlag", typeof(string)));

            oAccountReconciliation.DtAccountRec = dtAccountReconciliation.Clone();
            oAccountReconciliation.DtDelAccountRec = dtAccountReconciliation.Clone();
            Session[Constants.SES_ACC_RECON] = oAccountReconciliation;
        }
        #region InitializeData
        private void InitializeData()
        {

            string sRegNo = Request.QueryString[OBJ_REG_NO];
            string sPageID = Request.QueryString[OBJ_PAGE_ID];

            //Temporary table creation
            this.InitializeTmpTable();

            //Controls initialize
            DDListUtil.LoadDDLFromDB(ddlSPType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            DDListUtil.LoadDDLFromDB(ddlMaker, "UserName", "UserName", "SA_User", false);
            DDListUtil.LoadDDLFromXML(ddlAccountNo, "JournalAccount", "Type", "JournalAccount", false);

            txtTransDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            if (true)
            {
                //sRegNo = oCrypManager.GetDecryptedString(sRegNo, Constants.CRYPT_PASSWORD_STRING);
                //sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                LoadUnapprovedAccRecon();

                #region User-Detail.
                UserDetails oUserDetails = ucUserDet.UserDetail;
                oUserDetails.CheckerID = oConfig.UserName;
                oUserDetails.CheckDate = DateTime.Now;

                ucUserDet.UserDetail = oUserDetails;
                #endregion User-Detail.

                #region Enable-disable controls
                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);
                #endregion Enable-disable controls
            }
            else
            {

                //Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                //UserDetails oUserDetails = new UserDetails();
                //oUserDetails.MakerID = oConfig.UserName;
                //oUserDetails.MakeDate = DateTime.Now;

                //ucUserDet.UserDetail = oUserDetails;
                //EnableDisableControl(true);
            }
        }
        protected void LoadUnapprovedAccRecon()
        {
            AccountReconciliation oAccountReconciliation = new AccountReconciliation();
            JournalDAL oJournalDAL = new JournalDAL();
            DataTable dtTmp = ((AccountReconciliation)Session[Constants.SES_ACC_RECON]).DtAccountRec.Clone();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            oAccountReconciliation = (AccountReconciliation)oJournalDAL.GetAllUnapprovedAccJournal(ddlSPType.SelectedItem.Value.Trim(),ddlAccountNo.SelectedItem.Value.Trim(),ddlMaker.SelectedItem.Value.Trim(), oConfig.DivisionID, oConfig.BankCodeID).Return;


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
                rowAccRecon["MakeDate"] = (dr["MakeDate"].ToString() == string.Empty || dr["MakeDate"] == null ? string.Empty : DateTime.Parse(dr["MakeDate"].ToString().Trim()).ToString("dd-MM-yyyy"));
                rowAccRecon["ActionFlag"] = "NIL";

                dtTmp.Rows.Add(rowAccRecon);
            }

            gvAccRecon.DataSource = dtTmp;
            gvAccRecon.DataBind();

            //Store in session
            ((AccountReconciliation)Session[Constants.SES_ACC_RECON]).DtAccountRec = dtTmp.Copy();
            //Set Values
            object obj = new object();
            if (dtTmp.Select("DrCr='D'").Length > 0)
            {
                txtNoOfDrTrans.Text = dtTmp.Select("DrCr='D'").Length.ToString();
                obj = dtTmp.Compute("SUM(Amount)", "DrCr='D'");
                txtTotDrAmt.Text = !DBNull.Value.Equals(obj) ? Convert.ToDecimal(obj).ToString() : "0";
            }
            else
            {
                txtNoOfDrTrans.Text = "0";
                txtTotDrAmt.Text = "0";

            }
            if (dtTmp.Select("DrCr='C'").Length > 0)
            {
                txtNoOfCrTrans.Text = dtTmp.Select("DrCr='C'").Length.ToString();
                obj = dtTmp.Compute("SUM(Amount)", "DrCr='C'");
                txtTotCrAmt.Text = Convert.ToDecimal(obj).ToString();
            }
            else
            {
                txtNoOfCrTrans.Text = "0";
                txtTotCrAmt.Text = "0";
            }

        }
        #endregion InitializeData

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            this.doAction("APPROVE");
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            this.doAction("REJECT");
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            AccountReconciliation oAccountReconciliation = new AccountReconciliation();
            JournalDAL oJournalDAL = new JournalDAL();
            this.LoadUnapprovedAccRecon();
        }
        protected void btnSelectAll_Click(object sender, EventArgs e)
        {
            this.SelectDeselectAllCheck(true);
        }
        protected void btnDeselectAll_Click(object sender, EventArgs e)
        {
            this.SelectDeselectAllCheck(false);
        }
        private void SelectDeselectAllCheck(bool status)
        {
            try
            {
                foreach (GridViewRow gvr in gvAccRecon.Rows)
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
        private void doAction(string ActionType)
        {
            AccountReconciliation oAccountReconciliation = new AccountReconciliation();
            DataTable dtTmp = ((AccountReconciliation)Session[Constants.SES_ACC_RECON]).DtAccountRec.Clone();
            dtTmp = ((AccountReconciliation)Session[Constants.SES_ACC_RECON]).DtAccountRec.Copy();

            foreach (GridViewRow gvr in gvAccRecon.Rows)
            {
                CheckBox chk = (CheckBox)gvr.FindControl("chkSelected");
                if (chk != null)
                {
                    if (chk.Checked)
                    {
                        dtTmp.Rows[gvr.RowIndex]["ActionFlag"] = ActionType;
                    }
                 }
            }
            //do DB transaction
            oAccountReconciliation.DtAccountRec = dtTmp.Copy();
            oAccountReconciliation.UserDetails = ucUserDet.UserDetail;
            JournalDAL oJournalDAL = new JournalDAL();
            if (ActionType == "APPROVE")
            {
                if (oJournalDAL.ApproveAccJournal(oAccountReconciliation).Status)
                {
                    ucMessage.OpenMessage("Successfully Approved.", Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                    ucMessage.OpenMessage("Approved failure!!", Constants.MSG_TYPE_ERROR);
                }
            }
            else if (ActionType == "REJECT")
            {
                if (oJournalDAL.RejectAccJournal(oAccountReconciliation).Status)
                {
                    ucMessage.OpenMessage("Successfully Rejected.", Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                    ucMessage.OpenMessage("Reject failure!!", Constants.MSG_TYPE_ERROR);
                }
            }
            ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            //refresh grid view
            this.LoadUnapprovedAccRecon();

            #region Assign Data in calculation field set

            //txtTotalFaceValue.Text = dTolFaceValue.ToString("N2");

            #endregion
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
