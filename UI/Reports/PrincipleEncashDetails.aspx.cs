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
    public partial class PrincipleEncashDetails : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_REPORT.PRINCIPLE_ENCASHMENT_DETAILS))
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
            DDListUtil.LoadDDLFromDB(ddlCurrency, "CurrencyID", "CurrencyCode", "SPMS_Currency", true); 
            DDListUtil.LoadCheckBoxListFromDB(chkLSpType, "SPTypeID", "TypeDesc", "SPMS_SPType");
            txtFromDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtToDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            // load Report Type List
            rdlReportType.Items.Add(new ListItem("Encashed", Convert.ToString((int)Constants.ACTIVITY_TYPE.ENCASHED)));
            rdlReportType.Items.Add(new ListItem("Claimed", Convert.ToString((int)Constants.ACTIVITY_TYPE.CLAIMED)));
            rdlReportType.Items.Add(new ListItem("Cleared", Convert.ToString((int)Constants.ACTIVITY_TYPE.CLEARED)));
            rdlReportType.Items[0].Selected = true;

            Util.RBLChangeSetColor(rdlReportType);
            Util.RBLChangeColor(rdlReportType);
        }
        #endregion InitializeData

        protected void ddlCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlCurrency.SelectedValue))
            {
                DDListUtil.LoadCheckBoxListByCurrencyID(chkLSpType, ddlCurrency.SelectedValue);
            }
            else
            {
                DDListUtil.LoadCheckBoxListFromDB(chkLSpType, "SPTypeID", "TypeDesc", "SPMS_SPType");
            }
        }

        protected void btnPrintPreview_Click(object sender, EventArgs e)
        {            
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            
            if (oConfig != null)
            {
                ReportDAL rdal = new ReportDAL();
                Result oResult = new Result();
                // Parameter
                string sCheckList = Util.GetCheckListIDList(chkLSpType);
                DateTime dtFromDate = Util.GetDateTimeByString(txtFromDate.Text);
                DateTime dtToDate = Util.GetDateTimeByString(txtToDate.Text);
                string sCurrency = ddlCurrency.SelectedValue;

                string sRptType = rdlReportType.SelectedValue;

                oResult = rdal.PrincipleEncashDetailsReport(sCurrency, sCheckList, dtFromDate, dtToDate, sRptType, oConfig.BankCodeID, oConfig.DivisionID, oConfig.BranchID);
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
