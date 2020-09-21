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
    public partial class TransactionDetailsReports : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_REPORT.TRANSACTION_DETAILS_REPORT))
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
            DDListUtil.LoadDDLFromDB(ddlPayment, "PaymentMode", "Description", "SPMS_PaymentMode", true);

            txtToDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtFromDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            // load Report Type List
            rdlStatus.Items.Add(new ListItem("Issue", Convert.ToString((int)Constants.ACTIVITY_TYPE.ISSUE)));
            rdlStatus.Items.Add(new ListItem("Interest", Convert.ToString((int)Constants.ACTIVITY_TYPE.INTEREST)));
            rdlStatus.Items.Add(new ListItem("Encashed", Convert.ToString((int)Constants.ACTIVITY_TYPE.ENCASHED)));
            rdlStatus.Items[2].Selected = true;

            ddlStatus.Items.Add(new ListItem("Created", Convert.ToString(Constants.TRANSACTION_STATUS.CREATED)));
            ddlStatus.Items.Add(new ListItem("Ready For Approve", Convert.ToString(Constants.TRANSACTION_STATUS.READY_FOR_APPROVE)));
            ddlStatus.Items.Add(new ListItem("Approved", Convert.ToString(Constants.TRANSACTION_STATUS.APPROVED)));
            ddlStatus.Items.Add(new ListItem("Rejected", Convert.ToString(Constants.TRANSACTION_STATUS.REJECTED)));
            ddlStatus.Items.Add(new ListItem("Canceled", Convert.ToString(Constants.TRANSACTION_STATUS.CANCELED)));
            ddlStatus.Items[2].Selected = true;//By default "Approved" will be set

            Util.RBLChangeSetColor(rdlStatus);
            Util.RBLChangeColor(rdlStatus);

            Util.ChkChangeSetColor(chkForAllUser);
        }
        #endregion InitializeData


        protected void btnPrintPreview_Click(object sender, EventArgs e)
        {
            ReportDAL rdal = new ReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                // Parameter
                string sCheckList = Util.GetCheckListIDList(chkLSpType);
                DateTime dtFromDate = Util.GetDateTimeByString(txtFromDate.Text);
                DateTime dtToDate = Util.GetDateTimeByString(txtToDate.Text);
                string sPayMode = ddlPayment.SelectedValue;
                string sRptType = rdlStatus.SelectedValue;
                string sTranStatus = ddlStatus.SelectedValue;
                bool bAllUser = chkForAllUser.Checked;

                oResult = rdal.TransactionDetailsReport(sPayMode, bAllUser, sTranStatus, sRptType, dtFromDate, dtToDate, sCheckList, oConfig.BankCodeID, oConfig.DivisionID, oConfig.UserName, oConfig.BranchID);
                if (oResult.Status)
                {
                    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                }
            }
        }
    }
}
