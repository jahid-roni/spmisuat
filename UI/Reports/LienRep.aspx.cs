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
using System.Text;
using LAMS.Web.UI;

namespace SBM_WebUI.mp
{
    public partial class LienRep : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_REPORT.LIEN_REPORT))
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
            DDListUtil.LoadDDLFromDB(ddlUserName, "UserName", "UserName", "SA_User", false);
            txtFromDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtToDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            // load Report Type List
            rdlStatus.Items.Add(new ListItem("Lien Mark", Convert.ToString((int)Constants.ACTIVITY_TYPE.LIEN_MARK)));
            rdlStatus.Items.Add(new ListItem("Lien Mark Remove", Convert.ToString((int)Constants.ACTIVITY_TYPE.LIEN_MARK_REMOVE)));
            rdlStatus.Items.Add(new ListItem("Active Lien", Convert.ToString((int)Constants.ACTIVITY_TYPE.LIEN_ACTIVE)));
            rdlStatus.Items.Add(new ListItem("Lien Status", Convert.ToString((int)Constants.ACTIVITY_TYPE.LIEN_STATUS)));
            rdlStatus.Items[0].Selected = true;
            
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
                // Parameter
                string sCheckList = Util.GetCheckListIDList(chkLSpType);
                string sRptType = rdlStatus.SelectedValue;
                DateTime dtFromDate = Util.GetDateTimeByString(txtFromDate.Text);
                DateTime dtToDate = Util.GetDateTimeByString(txtToDate.Text);
                string sUserID = ddlUserName.SelectedValue;

                oResult = rdal.LienReport(sRptType, sCheckList, dtFromDate, dtToDate, sUserID, oConfig.BankCodeID, oConfig.DivisionID, oConfig.BranchID);

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
