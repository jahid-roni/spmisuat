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
    public partial class ReinvestmentStatement : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_REPORT.REINVESTMENT_STATEMENT))
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
            gvData.DataSource = null;
            gvData.DataBind();

            DateTime dtFromDate = Util.GetDateTimeByString(txtFromDate.Text);
            DateTime dtToDate = Util.GetDateTimeByString(txtToDate.Text);
            string sSPTypeID = ddlSpType.SelectedValue;

            ReportDAL rdal = new ReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            oResult = rdal.ReinvestmentStatementShowListData(sSPTypeID, dtFromDate, dtToDate, oConfig.BankCodeID, oConfig.DivisionID);
            if (oResult.Status)
            {
                DataTable dtTmp = (DataTable)oResult.Return;

                if (dtTmp != null)
                {
                    dtTmp.Columns.Remove("MakeDate");

                    dtTmp.Columns["ReinvestmentRefNo"].SetOrdinal(0);
                    dtTmp.Columns["OldRegNo"].SetOrdinal(1);
                    dtTmp.Columns["NewRegNo"].SetOrdinal(2);
                    dtTmp.Columns["IssueDate"].SetOrdinal(3);
                    dtTmp.Columns["IssueAmount"].SetOrdinal(4);

                    gvData.DataSource = dtTmp;
                    gvData.DataBind();

                    this.gvData.HeaderRow.Cells[1].Text = "Reinvestment Ref No.";
                    this.gvData.HeaderRow.Cells[2].Text = "Old Reg No.";
                    this.gvData.HeaderRow.Cells[3].Text = "New Reg No.";
                    this.gvData.HeaderRow.Cells[4].Text = "Issue Date";
                    this.gvData.HeaderRow.Cells[5].Text = "Amount";
                }
                Session[Constants.SES_RPT_SHOW] = dtTmp;
            }
        }

        protected void btnPrintPreview_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                ReportDAL rdal = new ReportDAL();
                Result oResult = new Result();

                DataTable dtData = (DataTable)Session[Constants.SES_RPT_SHOW];

                if (dtData != null && dtData.Rows.Count > 0)
                {
                    DataTable dtReportData = dtData.Clone();

                    foreach (GridViewRow gvr in gvData.Rows)
                    {
                        if ((gvr.FindControl("chkData") as CheckBox).Checked)
                        {
                            dtReportData.ImportRow(dtData.Rows[gvr.RowIndex]);
                        }
                    }

                    dtReportData.AcceptChanges();

                    oResult = rdal.ReinvestmentCoverLetter(dtReportData, ddlSpType.SelectedValue, oConfig.BranchID);

                    if (oResult.Status)
                    {
                        Session["ExportType"] = ddlExportType.SelectedValue;
                        if (ddlSpType.SelectedValue.ToString() != "WDB")
                        {
                            Session[Constants.SES_RPT_DATA] = oResult.Return;
                            Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());

                            oResult = rdal.ReinvestmentReportDIB(dtReportData, ddlSpType.SelectedValue, oConfig.BranchID);
                            Session[Constants.SES_RPT_DATA_2] = oResult.Return;
                            Page.RegisterStartupScript(Constants.REPORT_WINDOW_2, Util.OpenReport2(2));
                        }
                        else
                        {
                            oResult = rdal.ReinvestmentReportWDB(dtReportData, ddlSpType.SelectedValue, oConfig.BranchID);
                            Session[Constants.SES_RPT_DATA_3] = oResult.Return;
                            Page.RegisterStartupScript(Constants.REPORT_WINDOW_3, Util.OpenReport2(3));
                        }

                    }

                }
            }
        }

        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, null);
        }
    }
}
