/*Jakir20121226*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//add
using System.Collections;
using SBM_BLC1.Configuration;
using SBM_BLC1.Common;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Entity.Transaction;
using SBM_BLC1.Entity.JournalRecon;
using SBM_BLC1.DAL.Common;
using SBM_BLC1.DAL.JournalRecon;
using SBM_BLC1.Transaction;


namespace SBM_WebUI.mp
{
    public partial class OutstandingReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Constants.SES_USER_CONFIG] != null)
            {
                if (!Page.IsPostBack)
                {
                    Util.InvalidateSession();
                    Initialization();
                    //This is for Page Permission
                    CheckPermission chkPer = new CheckPermission();
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_JOURNAL_RECON.JOURNAL_RECON))
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
        private void Initialization()
        {
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            //txtDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            DDListUtil.LoadDDLFromXML(ddlAccountNo, "JournalAccount", "Type", "JournalAccount", false);
            //DDListUtil.LoadDDLFromXML(ddlJournalStatus, "JournalStatus", "Type", "JournalStatus", false);
            lblAccName.Text = "Softcell Solution Ltd ";
            this.RBLChangeColor(rdlType);
            this.ChkChangeColor(chkAscending);
        }

        public void RBLChangeColor(RadioButtonList rdoBox)
        {
            for (int i = 0; i <= rdoBox.Items.Count - 1; i++)
            {
                rdoBox.Items[i].Attributes["onclick"] = string.Format("RbChangeColor( this ) ");
            }
        }
        private void ChkChangeColor(CheckBox cblBox)
        {
            cblBox.Attributes["onclick"] = string.Format("ChkChangeColor( this ) ");
        }

        protected void btnOutStanding_Click(object sender, EventArgs e)
        {
            ReportDAL rdal = new ReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                //string sCheckList = Util.GetCheckListIDList(chkLSpType);
                //string sRptType = rdlStatus.SelectedValue;
                DateTime dtFromPayment = Util.GetDateTimeByString(txtPaymentFromDate.Text);
                DateTime dtToPayment = Util.GetDateTimeByString(txtPaymentToDate.Text);

                DateTime dtFromRecon = Util.GetDateTimeByString(txtReconFromDate.Text);
                DateTime dtToRecon = Util.GetDateTimeByString(txtReconToDate.Text);

                oResult = rdal.OutstandingReport(dtFromRecon, dtToRecon,oConfig.BankCodeID, oConfig.DivisionID, oConfig.BranchID);

                if (oResult.Status)
                {
                    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                }
            }
        }

    }
}
