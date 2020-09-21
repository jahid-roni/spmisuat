using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SBM_BLC1.Common;
using SBM_BLC1.DAL.Common;
using SBM_BLC1.Entity.Common;
using System.Data;

namespace SBM_WebUI.mp
{
    public partial class DataUploadReport : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_REPORT.DATA_UPLOAD_REPORT))
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
            DDListUtil.LoadDDLFromDB(ddlJournalType, "JournalType", "Description", "SPMS_JournalType", true);            
            ReportDAL rdal = new ReportDAL();
            Result oResult = rdal.LoadAccountingEntryOperator();
            if (oResult.Status)
            {
               DataTable dtAccountUser = (DataTable)oResult.Return;
               DDListUtil.Assign(ddlOperator, dtAccountUser, false);
            }
            txtToDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtFromDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            chkAccEntryOper.Checked = true;
            chkJournalType.Checked = true;
            chkUploadDataRange.Checked = true;

        }
        #endregion InitializeData



        protected void btnPrintPreview_Click(object sender, EventArgs e)
        {
            ReportDAL rdal = new ReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                bool bChkAccEntryOperator = chkAccEntryOper.Checked;
                string sAccountOperator = ddlOperator.SelectedValue;

                bool bChkJournalType = chkJournalType.Checked;
                string sJournalType = ddlJournalType.SelectedValue;

                bool bChkUploadDataRange = chkUploadDataRange.Checked;
                DateTime dtFromDate = Util.GetDateTimeByString(txtFromDate.Text);
                DateTime dtToDate = Util.GetDateTimeByString(txtToDate.Text);

                oResult = rdal.DataUploadReport(bChkAccEntryOperator, sAccountOperator, bChkJournalType, sJournalType, bChkUploadDataRange, dtFromDate, dtToDate, oConfig.BankCodeID, oConfig.DivisionID, oConfig.BranchID);

                if (oResult.Status)
                {
                    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                }
            }
        }
    }
}
