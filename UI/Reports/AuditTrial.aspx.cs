using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.DAL.Common;
using SBM_BLC1.Entity.Common;

namespace SBM_WebUI.mp
{
    public partial class AuditTrial : System.Web.UI.Page
    {
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_REPORT.AUDIT_TRAIL))
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
            // Dropdown load SPType
            DDListUtil.LoadCheckBoxListFromDB(chkLSpType, "SPTypeID", "TypeDesc", "SPMS_SPType");
            txtFromDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtToDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            // load Report Type List
            rdlStatus.Items.Add(new ListItem("Audit Trail (SP Received)", Constants.ACTIVITY_TYPE.RECEIVE.ToString()));
            rdlStatus.Items.Add(new ListItem("Audit Trail (Stop Payment Remove)", Constants.ACTIVITY_TYPE.STOP_PAYMENT_REMOVE.ToString()));
            rdlStatus.Items.Add(new ListItem("Audit Trail (Issue)", Constants.ACTIVITY_TYPE.ISSUE.ToString()));
            rdlStatus.Items.Add(new ListItem("Audit Trail (Duplicate Issue)", Constants.ACTIVITY_TYPE.DUPLICATE_ISSUE.ToString()));
            rdlStatus.Items.Add(new ListItem("Audit Trail (Reinvestment)", Constants.ACTIVITY_TYPE.REINVESTMENT.ToString()));
            rdlStatus.Items.Add(new ListItem("Audit Trail (Commission Reimbursement)", Constants.ACTIVITY_TYPE.COMMISSION_REIMBURSE.ToString()));
            rdlStatus.Items.Add(new ListItem("Audit Trail (Interest Payment)", Constants.ACTIVITY_TYPE.INTEREST_PAYMENT.ToString()));
            rdlStatus.Items.Add(new ListItem("Audit Trail (Interest Reimbursement)", Constants.ACTIVITY_TYPE.INTEREST_REIMBURSE.ToString()));
            rdlStatus.Items.Add(new ListItem("Audit Trail (Principle Payment)", Constants.ACTIVITY_TYPE.PRINCIPAL_PAYMENT.ToString()));
            rdlStatus.Items.Add(new ListItem("Audit Trail (Principle Reimbursement)", Constants.ACTIVITY_TYPE.PRINCIPAL_REIMBURSE.ToString()));
            rdlStatus.Items.Add(new ListItem("Audit Trail (Lien)", Constants.ACTIVITY_TYPE.LIEN_MARK.ToString()));
            rdlStatus.Items.Add(new ListItem("Audit Trail (Reconciliation)", Constants.ACTIVITY_TYPE.RECONCILIATION.ToString()));
            rdlStatus.Items.Add(new ListItem("Audit Trail (Lien Withdrawal)", Constants.ACTIVITY_TYPE.LIEN_WITHDRAWAL.ToString()));
            rdlStatus.Items.Add(new ListItem("Audit Trail (Add Journals)", Constants.ACTIVITY_TYPE.ADD_JOURNALS.ToString()));
            rdlStatus.Items.Add(new ListItem("Audit Trail (Stop Payment)", Constants.ACTIVITY_TYPE.STOP_PAYMENT.ToString()));
            rdlStatus.Items.Add(new ListItem("Audit Trail (Issue Update)", Constants.ACTIVITY_TYPE.ISSUE_UPDATE.ToString()));
            rdlStatus.Items.Add(new ListItem("Audit Trail (Issue Update Details)", Constants.ACTIVITY_TYPE.ISSUE_UPDATE_DETAILS.ToString()));
            rdlStatus.Items[0].Selected = true;
            // end of Report Type List

            Util.RBLChangeSetColor(rdlStatus);
            Util.RBLChangeColor(rdlStatus);
        }
        #endregion InitializeData

        protected void btnPrintPreview_Click(object sender, EventArgs e)
        {
            ReportDAL rdal = new ReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                string sCheckList = Util.GetCheckListIDList(chkLSpType);
                string sRptType = rdlStatus.SelectedValue;
                DateTime dtFromDate = Util.GetDateTimeByString(txtFromDate.Text);
                DateTime dtToDate = Util.GetDateTimeByString(txtToDate.Text);
                
                oResult = rdal.AuditTrail(sRptType, sCheckList, dtFromDate, dtToDate, oConfig.BankCodeID, oConfig.DivisionID, oConfig.BranchID);
                
                if (oResult.Status)
                {
                    Session["ExportType"] = ddlExportType.SelectedValue;
                    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                }
            }
        }
    }
}