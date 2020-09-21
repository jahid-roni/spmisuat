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
    public partial class ReceiveReg : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_REPORT.RECEIVE_REGISTER))
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

            txtFromDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtToDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtBBDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtOurDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

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
            ReportDAL rdal = new ReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                string sCheckList = Util.GetCheckListIDList(chkLSpType);
                DateTime dtFromDate = Util.GetDateTimeByString(txtFromDate.Text);
                DateTime dtToDate = Util.GetDateTimeByString(txtToDate.Text);
                DateTime dtBBDate = Util.GetDateTimeByString(txtBBDate.Text);
                DateTime dtOurDate = Util.GetDateTimeByString(txtOurDate.Text);
                string sCurrency = ddlCurrency.SelectedValue;
                string sOurReference = txtOurReference.Text;
                string sBBReference = txtBBReference.Text;

                oResult = rdal.RecieveRegReport(sCurrency, sCheckList, dtFromDate, dtToDate, sOurReference, dtOurDate, sBBReference, dtBBDate, chkFBB.Checked, oConfig.BankCodeID, oConfig.DivisionID);
                
                if (oResult.Status)
                {
                    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                }
            }
        }
    }
}
