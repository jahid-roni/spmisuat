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
    public partial class IssueReg : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_REPORT.ISSUE_REGISTER))
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
            txtDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            //exclusive currency list
            int i = ddlCurrency.Items.Count;
            for (int j = 0; j < i; j++)
            {
                for (int k = 0; k < i; k++)
                {
                    if (ddlCurrency.Items[k].Value != "" && ddlCurrency.Items[k].Value != "00" && ddlCurrency.Items[k].Value != "01")
                    {
                        ddlCurrency.Items.RemoveAt(k);
                        i--;
                        j--;
                        break;
                    }
                }
            }   
            ListItem li=new ListItem();
            li.Value="99"; li.Text="All";
            ddlCurrency.Items.Add(li);

            // load Report Type List
            rdlStatus.Items.Add(new ListItem("Issue", Convert.ToString((int)Constants.ACTIVITY_TYPE.ISSUE)));
            rdlStatus.Items.Add(new ListItem("Old Issue", Convert.ToString((int)Constants.ACTIVITY_TYPE.ISSUE_OLD)));
            rdlStatus.Items.Add(new ListItem("Reinvest", Convert.ToString((int)Constants.ACTIVITY_TYPE.REINVESTMENT)));
            rdlStatus.Items.Add(new ListItem("Stop Payment", Convert.ToString((int)Constants.ACTIVITY_TYPE.STOP_PAYMENT)));
            rdlStatus.Items.Add(new ListItem("Stop Payment Remove", Convert.ToString((int)Constants.ACTIVITY_TYPE.STOP_PAYMENT_REMOVE)));
            rdlStatus.Items.Add(new ListItem("Duplicate Issue", Convert.ToString((int)Constants.ACTIVITY_TYPE.DUPLICATE_ISSUE)));
            rdlStatus.Items.Add(new ListItem("Lien Mark", Convert.ToString((int)Constants.ACTIVITY_TYPE.LIEN_MARK)));
            rdlStatus.Items.Add(new ListItem("Lien Mark Remove", Convert.ToString((int)Constants.ACTIVITY_TYPE.LIEN_MARK_REMOVE)));
            rdlStatus.Items[0].Selected = true;

            Util.RBLChangeSetColor(rdlStatus);
            Util.RBLChangeColor(rdlStatus);
            Util.ChkChangeSetColor(chkBelow);
        }
        #endregion InitializeData

        protected void ddlCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlCurrency.SelectedValue))
            {
                if (ddlCurrency.SelectedValue == "99")
                {
                    DDListUtil.LoadCheckBoxListFromDB(chkLSpType, "SPTypeID", "TypeDesc", "SPMS_SPType");
                }
                else
                {
                    DDListUtil.LoadCheckBoxListByCurrencyID(chkLSpType, ddlCurrency.SelectedValue);
                }
            }
            else
            {
                DDListUtil.LoadCheckBoxListFromDB(chkLSpType, "SPTypeID", "TypeDesc", "SPMS_SPType");
            }
        }

        protected void btnPrintPreview_Click(object sender, EventArgs e)
        {
            ReportDAL rdal = new ReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                // Parameter
                string sCheckList = Util.GetCheckListIDList(chkLSpType);
                DateTime dtDate = Util.GetDateTimeByString(txtDate.Text);
                string sCurrency = ddlCurrency.SelectedValue;
                string sRptType = rdlStatus.SelectedValue;
                bool bOrder = chkBelow.Checked;

                oResult = rdal.IssueRegReport(sCurrency, sCheckList, dtDate, sRptType, bOrder, oConfig.UserName, oConfig.BankCodeID, oConfig.DivisionID);
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
