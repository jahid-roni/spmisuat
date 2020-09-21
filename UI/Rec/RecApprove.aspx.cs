using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SBM_BLC1.Common;
using System.Data;
using SBM_BLC1.DAL.Reconciliation;
using SBM_BLC1.Entity.Common;
using System.Collections;
using SBM_BLC1.DAL.Reimbursement;

namespace SBM_WebUI.mp
{
    public partial class RecApprove : System.Web.UI.Page
    {
        #region Local Variable
        CryptographyManager oCrypManager = new CryptographyManager();
        ArrayList alAddSeperatorIndex = null;
        #endregion Local Variable


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Constants.SES_USER_CONFIG] != null)
            {
                if (!Page.IsPostBack)
                {
                    Util.InvalidateSession();
                    InitializeData();
                }
            }
            else
            {
                Response.Redirect(Constants.PAGE_LOGIN, false);
            }
        }

        protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("Page"))
            {
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                string sPageID = string.Empty;
                string sType = Request.QueryString["pType"];

                #region Reconciliation
                if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_RECON.SALES_STATEMENT).PadLeft(5, '0')))
                {
                    Response.Redirect(Constants.PAGE_REC_SALE_STATEMENT + "?sRefNo=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_RECON.COMMISSION).PadLeft(5, '0')))
                {
                    Response.Redirect(Constants.PAGE_REC_COMMISSION_CLAIM + "?sRefNo=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_RECON.INTEREST).PadLeft(5, '0')))
                {
                    Response.Redirect(Constants.PAGE_REC_INTEREST_CLAIM + "?sRefNo=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_RECON.ENCASHMENT).PadLeft(5, '0')))
                {
                    Response.Redirect(Constants.PAGE_REC_ENCASHMENT_CLAIM + "?sRefNo=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                #endregion Issue                
            }
        }

        protected void InitializeData()
        {
            gvData.PageSize = (int)Constants.PAGING_UNAPPROVED;

            if (Session[Constants.SES_CONFIG_UNAPPROVE_DATA] == null)
            {
                Session.Add(Constants.SES_CONFIG_UNAPPROVE_DATA, new DataTable());
            }
            else
            {
                Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = new DataTable();
            }
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            DDListUtil.LoadActiveUser(ddlUserName, "UserName", "UserName", "SA_User", (int)Constants.USER_GROUP.RECONCILIATION_MAKER, false, oConfig.DivisionID);  
            SearchAction();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (ddlUserName.SelectedItem.Value != "")
            {
                if (Session[Constants.SES_CONFIG_UNAPPROVE_DATA] != null)
                {
                    DataTable dtTmpList = (DataTable)Session[Constants.SES_CONFIG_UNAPPROVE_DATA];
                    gvData.DataSource = null;
                    if (dtTmpList.Rows.Count > 0)
                    {
                        DataView dv = new DataView();
                        DataTable dt = dtTmpList.Copy();

                        if (dt.Columns["Maker ID"] != null)
                        {
                            dt.Columns["Maker ID"].ColumnName = "MakerID";
                        }
                        dv.Table = dt.Copy();
                        dv.RowFilter = "MakerID ='" + ddlUserName.SelectedItem.Value + "'";
                        if (dv.Count > 0)
                        {
                            gvData.DataSource = dv.Table;
                        }
                    }
                    gvData.DataBind();
                }
            }
            else
            {
                SearchAction();
            }
        }

        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvData.PageIndex = e.NewPageIndex;
            if (Session[Constants.SES_CONFIG_UNAPPROVE_DATA] != null)
            {
                DataTable dtTmpList = (DataTable)Session[Constants.SES_CONFIG_UNAPPROVE_DATA];
                gvData.DataSource = dtTmpList;
                gvData.DataBind();
            }
        }

        protected void SearchAction()
        {
            string sType = Request.QueryString["pType"];
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            gvData.DataSource = null;
            gvData.DataBind();

            if (!string.IsNullOrEmpty(sType))
            {
                Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = new DataTable();

                #region Recon Sales Statement
                if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_RECON.SALES_STATEMENT).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Reconciliantion Sales Statement Approval Queue List";
                    SaleStatementReconDAL oSaleStatmentReconDAL = new SaleStatementReconDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Reconciliantion Sale Statement' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oSaleStatmentReconDAL.LoadUnapprovedList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtTmpCurrencyList = (DataTable)oResult.Return;
                    if (dtTmpCurrencyList != null)
                    {
                        gvData.DataSource = dtTmpCurrencyList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpCurrencyList;
                    }
                }
                #endregion Recon Sales Statement

                #region Recon Commission Claim
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_RECON.COMMISSION).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Reconciliantion Commission Claim Approval Queue List";
                    CommClaimReconciliationDAL oCommClaimRecDAL = new CommClaimReconciliationDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Reconciliantion Commission Claim' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oCommClaimRecDAL.LoadUnapprovedList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtTmpCurrencyList = (DataTable)oResult.Return;
                    if (dtTmpCurrencyList != null)
                    {
                        gvData.DataSource = dtTmpCurrencyList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpCurrencyList;
                    }
                } 
                #endregion

                #region Recon Interest Claim
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_RECON.INTEREST).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Reconciliantion Interest Claim Approval Queue List";
                    InterestReimbursementDAL oIntReimDAL = new InterestReimbursementDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Reconciliantion Interest Claim' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oIntReimDAL.LoadUnapprovedList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtTmpIssueUpdateList = (DataTable)oResult.Return;
                    if (dtTmpIssueUpdateList != null)
                    {
                        gvData.DataSource = dtTmpIssueUpdateList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpIssueUpdateList;
                    }
                } 
                #endregion

                #region Recon Encashment Claim
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_RECON.ENCASHMENT).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Reconciliantion Encashment Claim Approval Queue List";
                    EncashmentReimbursementDAL oEncashReimDAL = new EncashmentReimbursementDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Reconciliantion Encashment Claim' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oEncashReimDAL.LoadUnapprovedList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtTmpIssueUpdateList = (DataTable)oResult.Return;
                    if (dtTmpIssueUpdateList != null)
                    {
                        gvData.DataSource = dtTmpIssueUpdateList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpIssueUpdateList;
                    }
                } 
                #endregion
                

                //Make Filter By User
                if (!string.IsNullOrEmpty(oConfig.FilterMakerID))
                {
                    DDListUtil.Assign(ddlUserName, oConfig.FilterMakerID.Trim());
                    FilterData(oConfig.FilterMakerID.Trim());
                }
            }
        }

        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, alAddSeperatorIndex);
        }

        protected void ddlUserName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlUserName.SelectedItem.Value != "")
            {
                FilterData(ddlUserName.SelectedItem.Value);
            }
            else
            {
                //Show
                gvData.PageIndex = 1;
                DataTable dtTmpList = (DataTable)Session[Constants.SES_CONFIG_UNAPPROVE_DATA];
                gvData.DataSource = dtTmpList;
                gvData.DataBind();
            }
        }

        private void FilterData(string sMakerID)
        {

            if (sMakerID != "")
            {
                if (Session[Constants.SES_CONFIG_UNAPPROVE_DATA] != null)
                {
                    DataTable dtTmpList = (DataTable)Session[Constants.SES_CONFIG_UNAPPROVE_DATA];
                    gvData.DataSource = null;
                    if (dtTmpList.Rows.Count > 0)
                    {
                        DataView dv = new DataView();
                        DataTable dt = dtTmpList.Copy();

                        if (dt.Columns["Maker ID"] != null)
                        {
                            dt.Columns["Maker ID"].ColumnName = "MakerID";
                        }
                        dv = dt.DefaultView;
                        dv.RowFilter = "MakerID ='" + ddlUserName.SelectedItem.Value + "'";
                        if (dv.Count > 0)
                        {
                            gvData.DataSource = dv.ToTable();
                        }
                    }
                    gvData.DataBind();

                    Config oConfig = Session[Constants.SES_USER_CONFIG] as Config;
                    oConfig.FilterMakerID = sMakerID;
                }
            }
        }
    }
}
