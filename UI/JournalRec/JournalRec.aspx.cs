using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.DAL.JournalRecon;
using SBM_BLC1.Entity.Common;

namespace SBM_WebUI.mp
{
    public partial class JournalRec : System.Web.UI.Page
    {
        #region InitializeData

        


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Constants.SES_USER_CONFIG] != null)
            {
                if (!Page.IsPostBack)
                {
                    Util.InvalidateSession();
                    Initialization();
                    //This is for Page Permission
                    CheckPermission chkPer = new CheckPermission();
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_JOURNAL_RECON.JOURNAL_RECON))
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

        private void Initialization()
        {
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            txtDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            DDListUtil.LoadDDLFromXML(ddlAccNumbe, "JournalAccount", "Type", "JournalAccount", false);
            DDListUtil.LoadDDLFromXML(ddlJournalStatus, "JournalStatus", "Type", "JournalStatus", false);
            lblAccName.Text = "Softcell Solution Ltd ";


            foreach (ListItem lstItem in rdlType.Items)
            {
                if (lstItem.Selected == true)
                {
                    lstItem.Attributes["style"] = "color:#EE8927; forecolor:black; font-weight:bold";
                    lstItem.Attributes["onclick"] = string.Format("RbChangeColor( this ), rdlInitSetUp(this) ");
                }
                else
                {
                    lstItem.Attributes["style"] = "color:#000000; forecolor:black; font-weight:normal ";
                    lstItem.Attributes["onclick"] = string.Format("RbChangeColor( this ), rdlInitSetUp(this) ");
                }
            }

        }

        #endregion InitializeData

        protected void btnReconcile_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)Session[Constants.SES_JOURNAL_RECON];
            J.SetData(dt);
        }
        protected void btnAutoRecon_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)Session[Constants.SES_JOURNAL_RECON];
        }
        protected void btnMoveMain_Click(object sender, EventArgs e)
        {
            JournalDAL oJournalDAL = new JournalDAL();
            DataTable dt = (DataTable)Session[Constants.SES_JOURNAL_RECON];
            if (dt.Rows.Count>0)
            {
                Result oResult = (Result)oJournalDAL.MoveToMain(dt);
                if (oResult.Status)
                {
                    txtTotalDebitAmount.Text = "";
                    txtTotalCreditAmount.Text = "";
                    txtTotalNoofDebitTransaction.Text = "";
                    txtTotalNoofCreditTransaction.Text = "";
                    txtTotalBalance.Text = "";
                    gvAccJournals.DataSource = null;
                    gvAccJournals.DataBind();

                    ucMessage.OpenMessage("Save done , Istiak message need to move into constain page.. ", Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                    ucMessage.OpenMessage("Error.., Istiak message need to move", Constants.MSG_TYPE_ERROR);
                }
            }
            else
            {
                ucMessage.OpenMessage("No data is exist to move, Istiak message need to move into constain page.. ", Constants.MSG_TYPE_ERROR);
            }
        }
        protected void btnMoveArchive_Click(object sender, EventArgs e)
        {
            JournalDAL oJournalDAL = new JournalDAL();
            DataTable dt = (DataTable)Session[Constants.SES_JOURNAL_RECON];
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (dt.Rows.Count > 0)
            {
                Result oResult = (Result)oJournalDAL.MoveToArchive(dt, oConfig.UserName);
                if (oResult.Status)
                {
                    txtTotalDebitAmount.Text = "";
                    txtTotalCreditAmount.Text = "";
                    txtTotalNoofDebitTransaction.Text = "";
                    txtTotalNoofCreditTransaction.Text = "";
                    txtTotalBalance.Text = "";
                    gvAccJournals.DataSource = null;
                    gvAccJournals.DataBind();

                    ucMessage.OpenMessage("Save done , Istiak message need to move into constain page.. ", Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                    ucMessage.OpenMessage("Error.., Istiak message need to move", Constants.MSG_TYPE_ERROR);
                }
            }
            else
            {
                ucMessage.OpenMessage("No data is exist to move, Istiak message need to move into constain page.. ", Constants.MSG_TYPE_ERROR);
            }

        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            JournalDAL oJournalDAL = new JournalDAL();

            string _anexCriteria = string.Empty;
            if (!string.IsNullOrEmpty(ddlSpType.SelectedItem.Value))
            {
                _anexCriteria += " AND SPTypeID = '" + ddlSpType.SelectedItem.Value + "'";
            }
            if (!string.IsNullOrEmpty(txtDate.Text.Trim()))
            {
                _anexCriteria += " AND ValueDate >= '" + Util.GetDateTimeByString(txtDate.Text).ToString("yyyy-MM-dd") + "' ";
            }
            if (!string.IsNullOrEmpty(ddlAccNumbe.SelectedItem.Value))
            {
                _anexCriteria += " AND AccountNo = '" + ddlAccNumbe.SelectedItem.Value + "'";
            }
            if (!string.IsNullOrEmpty(ddlAmountCondition.SelectedItem.Value) && !string.IsNullOrEmpty(txtAmountCondition.Text.Trim()))
            {
                _anexCriteria += " AND Amount " + ddlAmountCondition.SelectedItem.Value + " " + txtAmountCondition.Text.Trim() + "";
            }
            if (!string.IsNullOrEmpty(txtPaymentFromDate.Text.Trim()) && !string.IsNullOrEmpty(txtPaymentToDate.Text.Trim()))
            {
                //no expression
            }
            if (!string.IsNullOrEmpty(txtReconFromDate.Text.Trim()))
            {
                _anexCriteria += " AND replace(convert(varchar, ReceiveDate, 106), ' ', '-') BETWEEN '" + txtReconFromDate.Text.Trim() + "' AND '" + (string.IsNullOrEmpty(txtReconToDate.Text.Trim()) ? txtReconFromDate.Text.Trim() : txtReconToDate.Text.Trim()) + "'";
            }
            if (!string.IsNullOrEmpty(ddlJournalStatus.SelectedItem.Value))
            {
                _anexCriteria += " AND IsReconciled = " + (ddlJournalStatus.SelectedItem.Value == "Reconciled" ? 1 : 0) + "";
            }
            if (!string.IsNullOrEmpty(ddlDrCr.SelectedItem.Value))
            {
                _anexCriteria += " AND DrCr = '" + (ddlDrCr.SelectedItem.Value == "Dr" ? 'D' : 'C') + "'";
            }
            if (chkAscending.Checked)
            {
                _anexCriteria += " ORDER BY SeqNo ASC";
            }
            else
            {
                _anexCriteria += " ORDER BY SeqNo DESC";
            }



            string sTableName = "";
            if (rdlType.SelectedValue.Equals("Download_Journal"))
            {
                sTableName = "";
                btnReconcile.Enabled = false;
                btnMoveArchive.Enabled = false;
                btnMoveMain.Enabled = false;
                btnBalance.Enabled = false;
                btnAutoRecon.Enabled = true;
            }
            else if (rdlType.SelectedValue.Equals("Current_Journal"))
            {
                sTableName = "SPMS_AccountReconciliation";
                btnReconcile.Enabled = true;
                btnMoveArchive.Enabled = true;
                btnMoveMain.Enabled = false;
                btnBalance.Enabled = true;
                btnAutoRecon.Enabled = true;
            }
            else if (rdlType.SelectedValue.Equals("Archives"))
            {
                sTableName = "SPMS_AccountReconciliation_Archive";
                btnReconcile.Enabled = false;
                btnMoveArchive.Enabled = false;
                btnMoveMain.Enabled = true;
                btnBalance.Enabled = true;
                btnAutoRecon.Enabled = true;
            }

            DataTable dtTmp = (DataTable)(oJournalDAL.LoadAccountingJournalList(_anexCriteria, sTableName).Return);
            SetData(dtTmp);
            Session[Constants.SES_JOURNAL_RECON] = dtTmp;


        }
        public void SetData(DataTable dt)
        {
            txtTotalDebitAmount.Text = "";
            txtTotalCreditAmount.Text = "";
            txtTotalNoofDebitTransaction.Text = "";
            txtTotalNoofCreditTransaction.Text = "";
            txtTotalBalance.Text = "";
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            gvAccJournals.DataSource = null;
            gvAccJournals.DataBind();

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    gvAccJournals.DataSource = dt;
                    gvAccJournals.DataBind();

                    Session[Constants.SES_ACC_RECON] = dt;

                    // detail section
                    object sumDrCountObj;
                    sumDrCountObj = dt.Compute("Count(Amount)", "DrCr = 'D'");

                    object sumCrCountObj;
                    sumCrCountObj = dt.Compute("Count(Amount)", "DrCr = 'C'");
                    
                    object sumDrObj;
                    sumDrObj = dt.Compute("Sum(Amount)", "DrCr = 'D'");
                    sumDrObj = sumDrObj.ToString() == "" ? 0 : sumDrObj;
                    
                    object sumCrObj;
                    sumCrObj = dt.Compute("Sum(Amount)", "DrCr = 'C'");
                    sumCrObj = sumCrObj.ToString() == "" ? 0 : sumCrObj;

                    txtTotalDebitAmount.Text = sumDrObj.ToString();
                    txtTotalCreditAmount.Text = sumCrObj.ToString();

                    txtTotalNoofDebitTransaction.Text = sumDrCountObj.ToString();
                    txtTotalNoofCreditTransaction.Text = sumCrCountObj.ToString();

                    txtTotalBalance.Text = Convert.ToString(Convert.ToDecimal(sumCrObj.ToString()) + Convert.ToDecimal(sumCrObj.ToString()));

                }
            }
        }

    }
}
