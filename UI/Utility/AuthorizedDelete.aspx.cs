using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.DAL.Claim;
using SBM_BLC1.Entity.Common;
using CrystalDecisions.Enterprise;
using SBM_BLC1.DAL.Utility;
using SBM_BLV1;
using SBM_BLC1.Transaction;

namespace SBM_WebUI.mp
{
    public partial class AuthorizedDelete : System.Web.UI.Page
    {
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_UTILITY.AUTHORIZE_DELETE))
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

        protected void InitializeData()
        {
            btnDeleteAll.Visible = false;
        }
        public void PopErrorMsgAction(string sType)
        {
            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_UPDATE).PadLeft(5, '0'), false);
            }
            else
            {
                // no action
            }
        }
        private string TransactionNo()
        {
            string sTransNo = null;
            foreach (GridViewRow gvr in gvTransactionList.Rows)
            {
                sTransNo=gvr.Cells[2].Text; 
            }
            return sTransNo;
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Result oResult = null;
            string sTransNo = TransactionNo();
            AuthorizedDeleteDAL ad = new AuthorizedDeleteDAL();

            if (ddlTransType.SelectedValue == "I")
            {
                oResult = ad.DeleteTransaction(SBM_BLV1.baseCommon.enmTransType.Issue, sTransNo);
            }
            else if (ddlTransType.SelectedValue == "C")
            {
                oResult = ad.DeleteTransaction(SBM_BLV1.baseCommon.enmTransType.Interest_Payment, sTransNo);
            }
            else if (ddlTransType.SelectedValue == "E")
            {
                oResult = ad.DeleteTransaction(SBM_BLV1.baseCommon.enmTransType.Encashment, sTransNo);
            }
            else
            {
                IssueDAL oIssueDAL = new IssueDAL();
                string sDeleteKey = gvTransactionList.Rows[0].Cells[0].Text;
                oResult = oIssueDAL.DeleteBBDocumentList_AD(sDeleteKey, ddlTransType.Text);
            }
            if (oResult.Status)
            {
                //UpdatePanel3
                btnSearch_Click(sender, e);
                ucMessage.OpenMessage("Authorized Deleted Successfully Executed.", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel3, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
            else
            {
                ucMessage.OpenMessage("Error: " + oResult.Message, Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel3, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }

        protected void btnDeleteAll_Click(object sender, EventArgs e)
        {
            Result oResult = null;
            string sTransNo = TransactionNo();
            AuthorizedDeleteDAL ad = new AuthorizedDeleteDAL();

            if (ddlTransType.SelectedValue == "I")
            {
                oResult = ad.DeleteFullTransaction(SBM_BLV1.baseCommon.enmTransType.Issue, sTransNo);
                if (oResult.Status)
                {
                    //UpdatePanel3
                    ucMessage.OpenMessage("Transaction Deleted Successfully.", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel3, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
                else
                {
                    ucMessage.OpenMessage("Error: " + oResult.Message, Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel3, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }

            }
            else
            {
                ucMessage.OpenMessage("Invalid Transaction Type.", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel3, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);

            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRegNo.Text))
            {
                if (ddlTransType.SelectedIndex > 0)
                {

                    DataTable dt = null;
                    Result oResult = null;
                    AuthorizedDeleteDAL ad = new AuthorizedDeleteDAL();

                    if (ddlTransType.SelectedValue == "I")
                    {
                        oResult = ad.LoadTransactionDetails(SBM_BLV1.baseCommon.enmTransType.Issue, txtRegNo.Text);
                    }
                    else if (ddlTransType.SelectedValue == "C")
                    {
                        oResult = ad.LoadTransactionDetails(SBM_BLV1.baseCommon.enmTransType.Interest_Payment, txtRegNo.Text);
                    }
                    else if (ddlTransType.SelectedValue == "E")
                    {
                        oResult = ad.LoadTransactionDetails(SBM_BLV1.baseCommon.enmTransType.Encashment, txtRegNo.Text);
                    }
                    else
                    {
                        IssueDAL oIssueDAL = new IssueDAL();
                        oResult = oIssueDAL.LoadBBDocumentList_AD(txtRegNo.Text, ddlTransType.Text);
                    }
                    if (oResult.Status)
                    {
                        gvTransactionList.DataSource = oResult.Return;
                        gvTransactionList.DataBind();
                    }
                    else
                    {
                        DataTable dt1 = new DataTable();
                        gvTransactionList.DataSource = dt1;
                        gvTransactionList.DataBind();
                    }
                }
                else
                {
                    DataTable dt2 = new DataTable();
                    gvTransactionList.DataSource = dt2;
                    gvTransactionList.DataBind();

                }
            }
            else
            {
                DataTable dt3 = new DataTable();
                gvTransactionList.DataSource = dt3;
                gvTransactionList.DataBind();
            }
        }

        protected void ddlTransType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDeleteAll.Visible = false;
            if (ddlTransType.SelectedIndex > 0)
            {
                if (ddlTransType.SelectedValue == "I")
                {
                    btnDeleteAll.Visible = true;
                }
            }
        }

    }
}
