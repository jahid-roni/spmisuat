using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Configuration;
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.DAL.Common;

namespace SBM_WebUI.mp
{
    public partial class LodgmentReq : System.Web.UI.Page
    {
        protected string SESSION_DATA_TABLE = "DataTable";

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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_REPORT.LODGMENT_REQUISITION))
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

        private void InitializeData()
        {
            //Load SPType
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            TotalClear();
        }

        protected void ddlSpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearControlValues();

            if (!string.IsNullOrEmpty(ddlSpType.SelectedValue))
            {                
                LoadBySPType();
            }            
        }

        private void LoadBySPType()
        {
            SPPolicy oSPPolicy = null;
            SPPolicyDAL oSPPolicyDAL = new SPPolicyDAL();
            Result oResult = (Result)oSPPolicyDAL.LatestPolicy(ddlSpType.SelectedValue, Constants.ACTIVITY_TYPE.ISSUE, DateTime.Now);
            if (oResult.Status)
            {                
                ddlDDDenom.Items.Clear();

                oSPPolicy = (SPPolicy)oResult.Return;
               
                DataTable dtDenom = new DataTable();
                if (oSPPolicy.SPType.ListOfDenomination.Denomination.Count > 0)
                {
                    dtDenom.Columns.Add(new DataColumn("Text", typeof(string)));
                    dtDenom.Columns.Add(new DataColumn("Value", typeof(string)));

                    DataRow rowDenom = null;
                    for (int i = 0; i < oSPPolicy.SPType.ListOfDenomination.Denomination.Count; i++)
                    {
                        rowDenom = dtDenom.NewRow();

                        rowDenom["Text"] = oSPPolicy.SPType.ListOfDenomination.Denomination[i].DenominationID.ToString();
                        rowDenom["Value"] = oSPPolicy.SPType.ListOfDenomination.Denomination[i].Series.ToString();
                        dtDenom.Rows.Add(rowDenom);
                    }
                }
                DDListUtil.Assign(ddlDDDenom, dtDenom, true);               
            }
        }

        protected void btnAddDenomination_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlDDDenom.SelectedValue) && !string.IsNullOrEmpty(txtDDQuantity.Text))
            {
                DataTable dtDenom = (DataTable)Session[SESSION_DATA_TABLE];
                bool isNewDenom = true;

                if (dtDenom == null)
                {
                    dtDenom = new DataTable();

                    dtDenom.Columns.Add(new DataColumn("DenominationID", typeof(string)));
                    dtDenom.Columns.Add(new DataColumn("Quantity", typeof(string)));
                    dtDenom.Columns.Add(new DataColumn("Amount", typeof(decimal)));
                }

                for (int denomIndex = 0; denomIndex < dtDenom.Rows.Count; denomIndex++)
                {
                    if (dtDenom.Rows[denomIndex]["DenominationID"].Equals(ddlDDDenom.SelectedValue))
                    {
                        dtDenom.Rows[denomIndex]["DenominationID"] = ddlDDDenom.SelectedValue;
                        dtDenom.Rows[denomIndex]["Quantity"] = txtDDQuantity.Text;
                        dtDenom.Rows[denomIndex]["Amount"] = Convert.ToDecimal(ddlDDDenom.SelectedValue) * Convert.ToDecimal(txtDDQuantity.Text);

                        isNewDenom = false;
                    }
                }

                if (isNewDenom)
                {
                    DataRow rowDenom = null;

                    rowDenom = dtDenom.NewRow();

                    rowDenom["DenominationID"] = ddlDDDenom.SelectedValue;
                    rowDenom["Quantity"] = txtDDQuantity.Text;
                    rowDenom["Amount"] = Convert.ToDecimal(ddlDDDenom.SelectedValue) * Convert.ToDecimal(txtDDQuantity.Text);

                    dtDenom.Rows.Add(rowDenom); 
                }

                gvDenomDetail.DataSource = dtDenom;
                gvDenomDetail.DataBind();

                decimal dTotalAmount = 0;
                foreach (GridViewRow gvr in gvDenomDetail.Rows)
                {
                    dTotalAmount += Convert.ToDecimal(gvr.Cells[3].Text);
                }

                txtTotalAmount.Text = dTotalAmount.ToString("N2");
                
                //Clear Text
                ddlDDDenom.SelectedIndex = 0;
                txtDDQuantity.Text = string.Empty;
                txtAmount.Text = string.Empty;
                //Update Session
                Session[SESSION_DATA_TABLE] = dtDenom;

            }
        }

        protected void gvDenomDetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //only signature
        }

        protected void gvDenomDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            
            DataTable dtDenom = (DataTable)Session[SESSION_DATA_TABLE];
            
            if (dtDenom != null)
            {
                for (int denomIndex = 0; denomIndex < dtDenom.Rows.Count; denomIndex++)
                {
                    if (dtDenom.Rows[denomIndex]["DenominationID"].Equals(gvRow.Cells[1].Text))
                    {
                        dtDenom.Rows.RemoveAt(denomIndex);
                        txtTotalAmount.Text = (Convert.ToDecimal(txtTotalAmount.Text) - Convert.ToDecimal(gvRow.Cells[3].Text)).ToString("N2");                        
                    }
                }
                //Reload Grid
                gvDenomDetail.DataSource = dtDenom;
                gvDenomDetail.DataBind();                                
                //update session
                Session[SESSION_DATA_TABLE] = dtDenom;
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            TotalClear();
        }

        private void TotalClear()
        {
            Session.Add(SESSION_DATA_TABLE, null);

            txtDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtDate.ReadOnly = true;
            //Denomination(s) details
            ddlDDDenom.SelectedIndex = 0;
            txtDDQuantity.Text = "";
            txtTotalAmount.Text = "";

            gvDenomDetail.DataSource = null;
            gvDenomDetail.DataBind();

            ddlSpType.SelectedIndex = 0;
            txtPersonName.Text = "";
        }

        private void ClearControlValues()
        {
            Session[SESSION_DATA_TABLE] = null;

            txtDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);            
            //Denomination(s) details
            ddlDDDenom.Items.Clear();
            txtDDQuantity.Text = "";
            txtTotalAmount.Text = "";

            gvDenomDetail.DataSource = null;
            gvDenomDetail.DataBind();            
            txtPersonName.Text = "";
        }

        protected void btnPrintPreview_Click(object sender, EventArgs e)
        {
            ReportDAL rdal = new ReportDAL();
            Result oResult = new Result(); 
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                string sSPTypeID = ddlSpType.SelectedValue;
                string [] sArrySPTypeDesc = ddlSpType.SelectedItem.Text.Split(':');
                string sSPTypeDesc = string.Empty;
                if (sArrySPTypeDesc.Count() > 1)
                {
                    sSPTypeDesc = sArrySPTypeDesc[1];
                }
                DateTime dtDate = Util.GetDateTimeByString(txtDate.Text);                
                string sPersonName = txtPersonName.Text;

                DataTable dtData = new DataTable();
                dtData.Columns.Add(new DataColumn("Denomination", typeof(string)));
                dtData.Columns.Add(new DataColumn("Qnty", typeof(int)));
                dtData.Columns.Add(new DataColumn("Amount", typeof(decimal)));
                DataRow dr = null;

                foreach (GridViewRow gvr in gvDenomDetail.Rows)
                {
                    dr = dtData.NewRow();

                    dr["Denomination"] = gvr.Cells[1].Text;
                    dr["Qnty"] = gvr.Cells[2].Text;
                    dr["Amount"] = gvr.Cells[3].Text;

                    dtData.Rows.Add(dr);
                }

                oResult = rdal.LodgmentReqReport(sSPTypeID, sSPTypeDesc, dtDate, dtData, sPersonName, oConfig.BankCodeID, oConfig.DivisionID, oConfig.BranchID);
                if (oResult.Status)
                {
                    Session["ExportType"] = ddlExportType.SelectedValue;
                    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                }
                else
                { 
                    
                }
            }
        }
    }
}
