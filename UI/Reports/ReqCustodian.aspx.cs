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
    public partial class ReqCustodian : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_REPORT.REQUISITION_TO_CUSTODIAN))
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
            DDListUtil.LoadDDLFromDB(ddlCurrency, "CurrencyID", "CurrencyCode", "SPMS_Currency", true);

            //exclusive currency list
            int i=ddlCurrency.Items.Count;
            for(int j=0;j<i;j++)
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
            ListItem li = new ListItem();
            li.Value = "99"; li.Text = "All";
            ddlCurrency.Items.Add(li);
            // load Report Type List
            rdlStatus.Items.Add(new ListItem("Requisition on Specific Date", Convert.ToString((int)Constants.ACTIVITY_TYPE.REQUISITION_SPECIFIC_DATE)));
            rdlStatus.Items.Add(new ListItem("Requisition As on Date", Convert.ToString((int)Constants.ACTIVITY_TYPE.REQUISITION_AS_ON_DATE)));
            rdlStatus.Items[0].Selected = true;

            txtDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            Util.RBLChangeSetColor(rdlStatus);
            Util.RBLChangeColor(rdlStatus);
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

            //if (!string.IsNullOrEmpty(ddlCurrency.SelectedValue))
            //{
            //    DDListUtil.LoadCheckBoxListByCurrencyID(chkLSpType, ddlCurrency.SelectedValue);
            //}
            //else
            //{
            //    DDListUtil.LoadCheckBoxListFromDB(chkLSpType, "SPTypeID", "TypeDesc", "SPMS_SPType");
            //}
        }

        protected void btnPrintPreview_Click(object sender, EventArgs e)
        {            
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                ReportDAL rdal = new ReportDAL();
                Result oResult = new Result();

                string sCheckList = Util.GetCheckListIDList(chkLSpType);
                DateTime dtDate = Util.GetDateTimeByString(txtDate.Text);
                string sCurrency = string.Empty;
                //if currency type is not selected All
                if (ddlCurrency.SelectedValue != "99")
                {
                    sCurrency = ddlCurrency.SelectedValue;
                }
                string sRptType = rdlStatus.SelectedValue;

                oResult = rdal.ReqCustodianReport(sCurrency, sCheckList, dtDate, sRptType, oConfig.BankCodeID, oConfig.DivisionID, oConfig.BranchID);
                
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
