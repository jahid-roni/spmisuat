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
    public partial class StopPayLetter : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_REPORT.STOP_PAYMENT_REPORT))
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
            rdlStatus.Items.Add(new ListItem("Stop Payment", Convert.ToString((int)Constants.ACTIVITY_TYPE.STOP_PAYMENT)));
            rdlStatus.Items.Add(new ListItem("Duplicate Issue", Convert.ToString((int)Constants.ACTIVITY_TYPE.DUPLICATE_ISSUE)));
            rdlStatus.Items.Add(new ListItem("Active Stop Payment", Convert.ToString((int)Constants.ACTIVITY_TYPE.STOP_PAYMENT_ACTIVE)));
            rdlStatus.Items.Add(new ListItem("Active Duplicate Issue", Convert.ToString((int)Constants.ACTIVITY_TYPE.DUPLICATE_ISSUE_ACTIVE)));
            rdlStatus.Items.Add(new ListItem("Stop Payment Remove", Convert.ToString((int)Constants.ACTIVITY_TYPE.STOP_PAYMENT_REMOVE)));
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

                oResult = rdal.StopPaymentLetter(Constants.LETTER_TYPE_STOP, "201302100006");
                if (oResult.Status)
                {
                    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport2(1));
                }
            }
        }
    }
}