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
    public partial class ReinvestmentSummary : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_REPORT.REINVESTMENT_SUMMARY))
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
            gvData.DataSource = null;
            gvData.DataBind();
            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            txtFromDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtToDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
        }
        #endregion InitializeData


        protected void btnShowData_Click(object sender, EventArgs e)
        {
            ReportDAL rdal = new ReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                DateTime dtFromDate = Util.GetDateTimeByString(txtFromDate.Text);
                DateTime dtToDate = Util.GetDateTimeByString(txtToDate.Text);
                string sSPTypeID = ddlSpType.SelectedValue;

                oResult = rdal.ReinvestmentShowListData(sSPTypeID, dtFromDate, dtToDate, oConfig.BankCodeID, oConfig.DivisionID);
                if (oResult.Status)
                {
                    gvData.DataSource = oResult.Return;
                    gvData.DataBind();
                }
            }
        }
        
        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, null);
            if (gvData.DataSource != null)
            {
                e.Row.Cells[8].Visible = false;
                e.Row.Cells[9].Visible = false;
            }
        }

        protected void btnPrintPreview_Click(object sender, EventArgs e)
        {
            DateTime dtFromDate = Util.GetDateTimeByString(txtFromDate.Text);
            DateTime dtToDate = Util.GetDateTimeByString(txtToDate.Text);
            string sSPTypeID = ddlSpType.SelectedValue;

            DataTable dtData = new DataTable();
            dtData.Columns.Add(new DataColumn("EncashmentClaimReferenceNo", typeof(string)));
            dtData.Columns.Add(new DataColumn("OldRegNo", typeof(string)));
            dtData.Columns.Add(new DataColumn("NewRegNo", typeof(string)));
            dtData.Columns.Add(new DataColumn("StatementDate", typeof(string)));
            dtData.Columns.Add(new DataColumn("TypeDesc", typeof(string)));
            dtData.Columns.Add(new DataColumn("SPTypeID", typeof(string)));
            dtData.Columns.Add(new DataColumn("CurrencyCode", typeof(string)));
            dtData.Columns.Add(new DataColumn("TotalAmountCleared", typeof(decimal)));
            dtData.Columns.Add(new DataColumn("Pieces", typeof(int)));
            
            
            DataRow dr = null;
            foreach (GridViewRow gvr in gvData.Rows)
            {
                if ((gvr.FindControl("chkSelected") as CheckBox).Checked)
                {
                    dr = dtData.NewRow();
                    dr["EncashmentClaimReferenceNo"] = gvr.Cells[1].Text;
                    dr["StatementDate"] = gvr.Cells[2].Text;
                    dr["TypeDesc"] = gvr.Cells[3].Text;
                    dr["SPTypeID"] = gvr.Cells[6].Text;
                    dr["CurrencyCode"] = gvr.Cells[7].Text;
                    dr["TotalAmountCleared"] = Util.GetDecimalNumber( gvr.Cells[5].Text);
                    dr["Pieces"] = Util.GetIntNumber(gvr.Cells[4].Text);
                    dr["OldRegNo"] = gvr.Cells[8].Text;
                    dr["NewRegNo"] = gvr.Cells[9].Text;
                    dtData.Rows.Add(dr);
                }
            }
            ReportDAL rdal = new ReportDAL();
            Result oResult = new Result();

            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            string sDesc = ddlSpType.SelectedItem.Text;
            sDesc = sDesc.Substring(sDesc.IndexOf(":") + 1);
            oResult = rdal.ReinvestmentSummeryReport(dtData, sDesc, oConfig.BranchID);
            if (oResult.Status)
            {
                Session["ExportType"] = ddlExportType.SelectedValue;
                Session[Constants.SES_RPT_DATA] = oResult.Return;
                Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
            }
        }
    }
}
