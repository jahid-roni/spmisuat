using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Text;
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Configuration;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.Transaction;
using SBM_BLC1.Entity.Transaction;

namespace SBM_WebUI.UI.UC
{
    public partial class UCSearchClaim : System.Web.UI.UserControl
    {
        public string Type = "";
        public string PageCaption = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitializeData();
            } 
        }

        private void InitializeData()
        {
            DDListUtil.LoadDDLFromDB(ddlSPType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            gvSearchList.PageSize = (int)Constants.PAGE_SIZE_CLAIM;
            Clear();
            lblPageCaption.Text = this.PageCaption;

            chkLastStatement.Attributes.Add("onclick", "CheckLastStatement(this)");
            chkStatemenDt.Attributes.Add("onclick", "StatementDate(this)");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            ClaimDAL oClaimDAL = new ClaimDAL();
            Result oResult = oClaimDAL.SearchList(
                    this.txtRefNo.Text,
                    this.chkLastStatement.Checked,
                    this.ddlSPType.SelectedValue,
                    Util.GetDateTimeByString(Request[txtFromDate.UniqueID].Trim()),
                    Util.GetDateTimeByString(Request[txtToDate.UniqueID].Trim()),
                    this.Type,
                    this.chkStatemenDt.Checked, oConfig.DivisionID, oConfig.BankCodeID);

            ScriptManager.RegisterStartupScript(this.upGv, typeof(string), Constants.POPUP_WINDOW, " HideProgressStatus('ctl00_cphDet_Claim_lblProgress') ", true);

            gvSearchList.DataSource = null;
            gvSearchList.DataBind();

            if (oResult.Status)
            {
                DataTable dtIssueList = (DataTable)oResult.Return;

                DataTable dtSearchListTmp = dtIssueList.Clone();
                dtSearchListTmp = dtIssueList.Copy();

                if (this.Type.Equals(Constants.SEARCH_CLAIM.COMMISSION_CLAIM.ToString()))
                {
                    dtSearchListTmp.Columns.Remove("CommissionClaimTransNo");
                    dtSearchListTmp.Columns.Remove("CurrencyID");
                    dtSearchListTmp.Columns.Remove("ConvRate");
                    dtSearchListTmp.Columns.Remove("DurationType");
                }
                else if (this.Type.Equals(Constants.SEARCH_CLAIM.ENCASHMENT_CLAIM.ToString()))
                {
                    dtSearchListTmp.Columns.Remove("EncashmentClaimTransNo");
                    dtSearchListTmp.Columns.Remove("DurationType");
                    dtSearchListTmp.Columns.Remove("Levi");
                    //dtSearchListTmp.Columns.Remove("CurrencyID");
                    //dtSearchListTmp.Columns.Remove("ConvRate"); 
                }
                else if (this.Type.Equals(Constants.SEARCH_CLAIM.INTEREST_CLAIM.ToString()))
                {
                    dtSearchListTmp.Columns.Remove("InterestClaimTransNo");
                    dtSearchListTmp.Columns.Remove("DurationType");
                    //dtSearchListTmp.Columns.Remove("CurrencyID");
                    dtSearchListTmp.Columns.Remove("Remuneration");
                    dtSearchListTmp.Columns.Remove("Levi");
                }
                else if (this.Type.Equals(Constants.SEARCH_CLAIM.SALESSTATEMENT_CLAIM.ToString()))
                {
                    dtSearchListTmp.Columns.Remove("SaleStatementTransNo");                    
                    dtSearchListTmp.Columns.Remove("DurationType");
                }

                if (dtSearchListTmp != null && dtSearchListTmp.Rows.Count > 0)
                {
                    gvSearchList.DataSource = dtSearchListTmp;
                    gvSearchList.DataBind();
                }
                Session[Constants.SES_CONFIG_APPROVE_DATA] = dtIssueList;
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            Clear();
        }

        public void Clear()
        {
            txtFromDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtToDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtRefNo.Text = string.Empty;
            ddlSPType.SelectedIndex = 0;
            gvSearchList.DataSource = null;
            gvSearchList.DataBind();
            chkLastStatement.Checked = true;
            chkStatemenDt.Checked = false;
            Session[Constants.SES_CONFIG_APPROVE_DATA] = null;
        }

        protected void gvSearchList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvSearchList, null);
        }

        protected void gvSearchList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSearchList.PageIndex = e.NewPageIndex;
            if (Session[Constants.SES_CONFIG_APPROVE_DATA] != null)
            {
                DataTable dtTmpList = (DataTable)Session[Constants.SES_CONFIG_APPROVE_DATA];
                DataTable dtSearchListTmp = dtTmpList.Clone();
                dtSearchListTmp = dtTmpList.Copy();

                if (this.Type.Equals(Constants.SEARCH_CLAIM.COMMISSION_CLAIM.ToString()))
                {
                    dtSearchListTmp.Columns.Remove("CommissionClaimTransNo");
                    dtSearchListTmp.Columns.Remove("CurrencyID");
                    dtSearchListTmp.Columns.Remove("ConvRate");
                    dtSearchListTmp.Columns.Remove("DurationType");
                }
                else if (this.Type.Equals(Constants.SEARCH_CLAIM.ENCASHMENT_CLAIM.ToString()))
                {
                    dtSearchListTmp.Columns.Remove("EncashmentClaimTransNo");
                    dtSearchListTmp.Columns.Remove("DurationType");
                    //dtSearchListTmp.Columns.Remove("CurrencyID");
                    //dtSearchListTmp.Columns.Remove("ConvRate");                    
                }
                else if (this.Type.Equals(Constants.SEARCH_CLAIM.INTEREST_CLAIM.ToString()))
                {
                    dtSearchListTmp.Columns.Remove("InterestClaimTransNo");
                    dtSearchListTmp.Columns.Remove("DurationType");
                    //dtSearchListTmp.Columns.Remove("CurrencyID");
                    dtSearchListTmp.Columns.Remove("Remuneration");
                    dtSearchListTmp.Columns.Remove("Levi");
                }
                else if (this.Type.Equals(Constants.SEARCH_CLAIM.SALESSTATEMENT_CLAIM.ToString()))
                {
                    dtSearchListTmp.Columns.Remove("SaleStatementTransNo");                                        
                    dtSearchListTmp.Columns.Remove("DurationType");
                }

                if (dtSearchListTmp != null && dtSearchListTmp.Rows.Count > 0)
                {
                    gvSearchList.DataSource = dtSearchListTmp;
                    gvSearchList.DataBind();
                }
                Session[Constants.SES_CONFIG_APPROVE_DATA] = dtTmpList;
            }
        }

        protected void gvSearchList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("Page"))
            {
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                object[] oMethodParameters = new object[1];
                DataTable dtData = (DataTable)Session[Constants.SES_CONFIG_APPROVE_DATA];
                if (dtData != null)
                {
                    oMethodParameters[0] = (DataRow)dtData.Rows[gvRow.DataItemIndex];
                }
                try
                {
                    Page.GetType().InvokeMember("ClaimSearchLoadAction", BindingFlags.InvokeMethod, null, this.Page, oMethodParameters);
                }
                catch (TargetInvocationException TIE)
                {
                    // nothing.. 
                }
            }
        }        
    }
}