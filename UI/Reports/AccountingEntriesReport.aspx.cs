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
using SBM.DM;

namespace SBM_WebUI.mp
{
    public partial class AccountingEntriesReport : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_REPORT.ACCOUNTING_ENTRIES_REPORT))
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
            DDListUtil.LoadCheckBoxListFromDB(chkLJournalType, "JournalType", "Description", "SPMS_JournalType");
            if (ddlUploadStatus.Items.Count > 0)
            {
                ddlUploadStatus.Text = "1";
            }
            txtToDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtFromDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
        }
        #endregion InitializeData

        protected void btnPrintPreview_Click(object sender, EventArgs e)
        {
            ReportDAL rdal = new ReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                int iUploadStatus = System.Convert.ToInt32(ddlUploadStatus.SelectedValue);
                DateTime dtFromDate = Util.GetDateTimeByString(txtFromDate.Text);
                DateTime dtToDate = Util.GetDateTimeByString(txtToDate.Text);
                string sJournalTypeList = Util.GetCheckListIDList(chkLJournalType);
                bool bchkDataRange = chkDataRange.Checked;

                oResult = rdal.AccountEntriesData(iUploadStatus, sJournalTypeList.Substring(1, sJournalTypeList.Length - 2), bchkDataRange, dtFromDate, dtToDate, oConfig.BankCodeID, oConfig.DivisionID, oConfig.UserName, oConfig.BranchID,chkAllUser.Checked);

                if (oResult.Status)
                {
                    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                }
            }
        }
    }
}
