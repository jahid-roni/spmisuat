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
using SBM_BLC1.DAL.Report;

namespace SBM_WebUI.mp
{
    public partial class BBDataExportReports : System.Web.UI.Page
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
            //DDListUtil.LoadCheckBoxListFromDB(chkLSpType, "SPTypeID", "TypeDesc", "SPMS_SPType");
            //DDListUtil.LoadDDLFromDB(ddlPayment, "PaymentMode", "Description", "SPMS_PaymentMode", true);
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);

            txtToDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtFromDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            // load Report Type List
            rdlStatus.Items.Add(new ListItem("Sale Data", "I"));
            rdlStatus.Items.Add(new ListItem("Interest", "C"));
            rdlStatus.Items.Add(new ListItem("Encashed", "E"));
            rdlStatus.Items.Add(new ListItem("All Payment", "P"));

            rdlStatus.Items[0].Selected = true;

            //ddlStatus.Items.Add(new ListItem("Created", Convert.ToString(Constants.TRANSACTION_STATUS.CREATED)));
            //ddlStatus.Items.Add(new ListItem("Ready For Approve", Convert.ToString(Constants.TRANSACTION_STATUS.READY_FOR_APPROVE)));
            //ddlStatus.Items.Add(new ListItem("Approved", Convert.ToString(Constants.TRANSACTION_STATUS.APPROVED)));
            //ddlStatus.Items.Add(new ListItem("Rejected", Convert.ToString(Constants.TRANSACTION_STATUS.REJECTED)));
            //ddlStatus.Items.Add(new ListItem("Canceled", Convert.ToString(Constants.TRANSACTION_STATUS.CANCELED)));
            //ddlStatus.Items[2].Selected = true;//By default "Approved" will be set

            Util.RBLChangeSetColor(rdlStatus);
            Util.RBLChangeColor(rdlStatus);

            //Util.ChkChangeSetColor(chkForAllUser);
        }
        #endregion InitializeData


        protected void btnPrintPreview_Click(object sender, EventArgs e)
        {
            BBExportManager rdal = new BBExportManager();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                // Parameter
                //string sCheckList = Util.GetCheckListIDList(chkLSpType);
                DateTime dtFromDate = Util.GetDateTimeByString(txtFromDate.Text);
                DateTime dtToDate = Util.GetDateTimeByString(txtToDate.Text);
                //string sPayMode = ddlPayment.SelectedValue;
                string sRptType = rdlStatus.SelectedValue;
                //string sTranStatus = ddlStatus.SelectedValue;
               // bool bAllUser = chkForAllUser.Checked;

                if (Convert.ToString(ddlSpType.SelectedValue) != "")
                {
                    string filePath = Server.MapPath("~") + "\\bbexcel";
                    if (sRptType == "I")
                    {

                        rdal.Download_SaleData(ddlSpType.SelectedValue.ToString(),oConfig.DivisionID, dtFromDate, dtToDate, filePath);
                    }
                    else if (sRptType == "C")
                    {
                        rdal.Download_PaymentData(sRptType, ddlSpType.SelectedValue.ToString(), oConfig.DivisionID, dtFromDate, dtToDate, filePath);
                    }
                    else if (sRptType == "E")
                    {
                        rdal.Download_PaymentData(sRptType, ddlSpType.SelectedValue.ToString(), oConfig.DivisionID, dtFromDate, dtToDate, filePath);
                    }
                    else if (sRptType == "P")
                    {
                        rdal.Download_PaymentData(sRptType, ddlSpType.SelectedValue.ToString(), oConfig.DivisionID, dtFromDate, dtToDate, filePath);
                    }
                }
            }
        }
    }
}
