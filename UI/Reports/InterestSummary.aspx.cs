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
    public partial class InterestSummary : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_REPORT.INTEREST_SUMMARY))
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
            
            gvData.DataSource = null;
            gvData.DataBind();
            
            if (oConfig != null)
            {
                DateTime dtFromDate = Util.GetDateTimeByString(txtFromDate.Text);
                DateTime dtToDate = Util.GetDateTimeByString(txtToDate.Text);
                string sSPTypeID = ddlSpType.SelectedValue;

                oResult = rdal.InterestSummaryShowListData(sSPTypeID, dtFromDate, dtToDate, oConfig.BankCodeID, oConfig.DivisionID);
                if (oResult.Status)
                {                    
                    DataTable dtTmp = (DataTable)oResult.Return;
                    if (dtTmp != null && dtTmp.Rows.Count>0)
                    {
                        
                        dtTmp.Columns.Remove("InterestReimburseTransNo");
                        dtTmp.Columns.Remove("ConvRate");
                        dtTmp.Columns.Remove("SPCurrency");
                        dtTmp.Columns.Remove("ClaimCurrency");
                        dtTmp.Columns.Remove("CurrencyID");

                        DataColumn dtColIsSelect = new DataColumn();

                        dtTmp.Columns["EncashmentClaimReferenceNo"].SetOrdinal(0);
                        dtTmp.Columns["SPTypeID"].SetOrdinal(1);
                        dtTmp.Columns["StatementDate"].SetOrdinal(2);
                        dtTmp.Columns["TypeDesc"].SetOrdinal(3);
                        dtTmp.Columns["Pieces"].SetOrdinal(4);
                        dtTmp.Columns["TotalAmountCleared"].SetOrdinal(5);
                        dtTmp.Columns["CurrencyCode"].SetOrdinal(6);

                        gvData.DataSource = dtTmp;
                        gvData.DataBind();

                        this.gvData.HeaderRow.Cells[0].Text = "Select";
                        this.gvData.HeaderRow.Cells[1].Text = "Ref No";
                        this.gvData.HeaderRow.Cells[2].Text = "SP Type";
                        this.gvData.HeaderRow.Cells[3].Text = "Date";
                        this.gvData.HeaderRow.Cells[4].Text = "Description";
                        this.gvData.HeaderRow.Cells[5].Text = "Pieces";
                        this.gvData.HeaderRow.Cells[6].Text = "Amount";
                    }

                    Session[Constants.SES_RPT_SHOW] = dtTmp;
                }
            }
        }

        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, null);
        }


        protected void btnPrintPreview_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                ReportDAL rdal = new ReportDAL();
                Result oResult = new Result();
                DataTable dtReportData = new DataTable();

                DataTable dtData = (DataTable)Session[Constants.SES_RPT_SHOW];
                dtReportData = dtData.Clone();
                foreach (GridViewRow gvr in gvData.Rows)
                {
                    if ((gvr.FindControl("chkData") as CheckBox).Checked)
                    {
                        dtReportData.ImportRow(dtData.Rows[gvr.RowIndex]);
                    }
                }
                dtReportData.AcceptChanges();
                oResult = rdal.InterestSummaryReport(dtReportData, oConfig.BranchID);
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
