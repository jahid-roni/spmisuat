/*
 * File name            : SystemConfigSetup.cs
 * Author               : A.K.M. Tanjil Hossain,
 *                      : A.K.M. Zahidul Quaium,
 *                      : Tanvir Alam
 *                      : Jerin Afsana
 * Date                 : Sep 01,2014
 * Version              : 1.0
 *
 * Description          : System Config Setup Page
 *
 * Modification history :
 * Name                         Date                            Desc
 * Tanvir Alam                Sep 01,2014                Business implementation 
 * A.K.M. Zahidul Quaium        February 02,2012                Business implementation              
 * Jerin Afsana                April    02,2012                Business implementation              
 * Copyright (c) 2012: Softcell Solution Ltd
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using SBM_BLC1.Configuration;
using SBM_BLC1.Common;
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Entity.Common;
using System.Collections;
using SBM_BLC1.Transaction;



namespace SBM_WebUI.mp
{
    public partial class Approve : System.Web.UI.Page
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
                    //This is for Page Permission
                    //CheckPermission chkPer = new CheckPermission();
                    //Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    //if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_TRANS.TRANS_APPROVE))
                    //{
                    //    Response.Redirect(Constants.PAGE_ERROR, false);
                    //}
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
                
                #region Receive
                if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.RECEIVE).PadLeft(5, '0')))
                {                  
  
                    Response.Redirect(Constants.PAGE_TRAN_RECEIVE + "?sTranscID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                #endregion

                #region Issue
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE).PadLeft(5, '0')))
                {
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                if (oConfig.GroupID != "1" && oConfig.GroupID != "12")
                {
                    if (gvRow.Cells[8].Text.Contains("MANAGER"))
                    {
                        ucMessage.OpenMessage("This registration required Manager Approval. Please check.", Constants.MSG_TYPE_ERROR);
                        ScriptManager.RegisterStartupScript(this.upGv, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        return;
                    }
                }

                    Response.Redirect(Constants.PAGE_TRAN_ISSUE + "?sTransID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_UPDATE).PadLeft(5, '0')))
                {
                    Response.Redirect(Constants.PAGE_TRAN_ISSUE_UPDATE + "?sIsUpdate=" + oCrypManager.GetEncryptedString("0") + "&sRegID=" + oCrypManager.GetEncryptedString(gvRow.Cells[2].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_OLD).PadLeft(5, '0')))
                {
                    Response.Redirect(Constants.PAGE_TRAN_ISSUE_OLD_CUSTOMER + "?sRegID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_ONL).PadLeft(5, '0')))
                {
                    Response.Redirect(Constants.PAGE_TRAN_ISSUE_ONLINE+ "?sRegID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                #endregion Issue

                #region Interest Payment
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.INTEREST_PAYMENT).PadLeft(5, '0')))
                {
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    if (oConfig.GroupID != "1" && oConfig.GroupID != "12")
                    {
                        if (gvRow.Cells[6].Text.Contains("MANAGER"))
                        {
                            ucMessage.OpenMessage("This registration required Manager Approval. Please check.", Constants.MSG_TYPE_ERROR);
                            ScriptManager.RegisterStartupScript(this.upGv, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                            return;
                        }
                    }                 
                    Response.Redirect(Constants.PAGE_TRAN_INTPAYMENT + "?sRegNo=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                #endregion Interest Payment

                #region Enchashed
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.ENCASHED).PadLeft(5, '0')))
                {
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    if (oConfig.GroupID != "1" && oConfig.GroupID != "12")
                    {
                        if (gvRow.Cells[6].Text.Contains("MANAGER"))
                        {
                            ucMessage.OpenMessage("This registration required Manager Approval. Please check.", Constants.MSG_TYPE_ERROR);
                            ScriptManager.RegisterStartupScript(this.upGv, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                            return;
                        }
                    }  
                    Response.Redirect(Constants.PAGE_TRAN_ENCASHMENT + "?sRegNo=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                #endregion Enchashed

                #region Stop Payment
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.STOP_PAYMENT_MARK).PadLeft(5, '0')))
                {
                    Response.Redirect(Constants.PAGE_TRAN_STOP_PAYMENT_MARK + "?sRegNo=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.STOP_PAYMENT_REMOVE_MARK).PadLeft(5, '0')))
                {
                    Response.Redirect(Constants.PAGE_TRAN_STOP_PAYMENT_REMOVE_MARK + "?sRegNo=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.DUPLICATE_ISSUE).PadLeft(5, '0')))
                {
                    Response.Redirect(Constants.PAGE_TRAN_DUPLICATE_ISSUE + "?sRegNo=" + oCrypManager.GetEncryptedString(gvRow.Cells[2].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                #endregion Stop Payment

                #region Lien
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.LIEN_MARK).PadLeft(5, '0')))
                {
                    Response.Redirect(Constants.PAGE_TRAN_LIEN_MARK + "?sRegNo=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.LIEN_MARK_REMOVE).PadLeft(5, '0')))
                {
                    Response.Redirect(Constants.PAGE_TRAN_LIEN_MARK_REMOVE + "?sRegNo=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                #endregion Lien

                #region Reinvestment
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.REINVESTMENT).PadLeft(5, '0')))
                {
                    Response.Redirect(Constants.PAGE_TRAN_REINVESTMENT + "?sRegNo=" + oCrypManager.GetEncryptedString(gvRow.Cells[2].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                #endregion Reinvestment

                #region ACERegister
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.ACE_MANAGER).PadLeft(5, '0')))
                {
                    Response.Redirect(Constants.PAGE_TRAN_ACE + "?sRegNo=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                }
                #endregion ACERegister
            }
        }

        protected void InitializeData()
        {
            //gvData.PageSize = (int)Constants.PAGING_UNAPPROVED;

            if (Session[Constants.SES_CONFIG_UNAPPROVE_DATA] == null)
            {
                Session.Add(Constants.SES_CONFIG_UNAPPROVE_DATA, new DataTable());
            }
            else
            {
                Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = new DataTable();
            }

            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            string sType = Request.QueryString["pType"];
            
            if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.LIEN_MARK).PadLeft(5, '0'))
                || sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.LIEN_MARK_REMOVE).PadLeft(5, '0')))
            {
                DDListUtil.LoadActiveUser(ddlUserName, "UserName", "UserName", "SA_User", (int)Constants.USER_GROUP.LIEN_MAKER, false, oConfig.DivisionID);
            }
            else
            {
                DDListUtil.LoadActiveUser(ddlUserName, "UserName", "UserName", "SA_User", (int)Constants.USER_GROUP.MAKER, false, oConfig.DivisionID);
            }
            
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
                        dv = dt.DefaultView;
                        dv.RowFilter = "MakerID ='" + ddlUserName.SelectedItem.Value + "'";
                        if (dv.Count > 0)
                        {
                            gvData.DataSource = dv.ToTable();
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
                if (!string.IsNullOrEmpty(ddlUserName.SelectedValue))
                {
                    FilterData(ddlUserName.SelectedValue);
                }
                else
                {
                    DataTable dtTmpList = (DataTable)Session[Constants.SES_CONFIG_UNAPPROVE_DATA];
                    gvData.DataSource = dtTmpList;
                    gvData.DataBind();
                }
            }
        }

        protected void SearchAction()
        {
            string sType = Request.QueryString["pType"];

            gvData.DataSource = null;
            gvData.DataBind();

            if (!string.IsNullOrEmpty(sType))
            {
                Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = new DataTable();
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

                #region Receive
                if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.RECEIVE).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "SP Receive Approval Queue List";
                    ReceiveDAL oReceiveDAL = new ReceiveDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Receive' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oReceiveDAL.LoadUnapprovedList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {
                            gvData.DataSource = dtTmpList;
                            gvData.DataBind();
                            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;
                        }
                    }
                }
                #endregion Receive

                #region Issue
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "SP Issue Approval Queue List";
                    IssueDAL oIssueDAL = new IssueDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Issue' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oIssueDAL.LoadUnapprovedList(null, true, oConfig.DivisionID,  oConfig.BankCodeID);
                    DataTable dtTmpCurrencyList = (DataTable)oResult.Return;
                    if (dtTmpCurrencyList != null)
                    {
                        gvData.DataSource = dtTmpCurrencyList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpCurrencyList;
                    }
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_UPDATE).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "SP Issue Update Approval Queue List";
                    IssueDAL oIssueDAL = new IssueDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Issue Update' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oIssueDAL.LoadTmpIssueUpdateDataTableList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtTmpIssueUpdateList = (DataTable)oResult.Return;
                    if (dtTmpIssueUpdateList != null)
                    {
                        gvData.DataSource = dtTmpIssueUpdateList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpIssueUpdateList;
                    }
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_OLD).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "SP Old Customer Issue Approval Queue List";
                    IssueDAL oIssueDAL = new IssueDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Old Customer Issue' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oIssueDAL.LoadUnapprovedOldCustomerIssueList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtTmpIssueUpdateList = (DataTable)oResult.Return;
                    if (dtTmpIssueUpdateList != null)
                    {
                        gvData.DataSource = dtTmpIssueUpdateList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpIssueUpdateList;
                    }
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_ONL).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "SP Online Issue Approval Queue List";
                    IssueDAL oIssueDAL = new IssueDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'SP Online Issue' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oIssueDAL.LoadUnapprovedOnlineIssueList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtTmpIssueUpdateList = (DataTable)oResult.Return;
                    if (dtTmpIssueUpdateList != null)
                    {
                        gvData.DataSource = dtTmpIssueUpdateList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpIssueUpdateList;
                    }
                }
                #endregion Issue

                #region interst payment
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.INTEREST_PAYMENT).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Interst Payment Approval Queue List";
                    InterestPaymentDAL oIntPayDAL = new InterestPaymentDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Interst Payment' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oIntPayDAL.LoadUnapprovedList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtTmpIntPayList = (DataTable)oResult.Return;
                    if (dtTmpIntPayList != null)
                    {
                        gvData.DataSource = dtTmpIntPayList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpIntPayList;
                    }
                }
                #endregion interst payment

                #region Encashment
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.ENCASHED).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Encashment Approval Queue List";
                    EncashmentDAL oEncashDAL = new EncashmentDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Encashment' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oEncashDAL.LoadUnapprovedList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtTmpIntPayList = (DataTable)oResult.Return;
                    if (dtTmpIntPayList != null)
                    {
                        gvData.DataSource = dtTmpIntPayList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpIntPayList;
                    }
                }
                #endregion Encashment

                #region Stop Payment
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.STOP_PAYMENT_MARK).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Payment Mark Approval Queue List";
                    StopPaymentDAL oStopPayDAL = new StopPaymentDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Payment Mark' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oStopPayDAL.LoadUnapprovedPaymentMarkList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtList = (DataTable)oResult.Return;
                    if (dtList != null)
                    {
                        gvData.DataSource = dtList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtList;
                    }
                }

                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.STOP_PAYMENT_REMOVE_MARK).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Payment Remove Mark Approval Queue List";
                    StopPaymentDAL oStopPayDAL = new StopPaymentDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Payment Remove Marks' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oStopPayDAL.LoadUnapprovedPaymentRemoveMarkList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtList = (DataTable)oResult.Return;
                    if (dtList != null)
                    {
                        gvData.DataSource = dtList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtList;
                    }
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.DUPLICATE_ISSUE).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Duplicate Issues Approval Queue List";
                    DuplicateIssueDAL oDIDAL = new DuplicateIssueDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Duplicate Issues' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oDIDAL.LoadUnapprovedList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtList = (DataTable)oResult.Return;
                    if (dtList != null)
                    {
                        gvData.DataSource = dtList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtList;
                    }
                }
                #endregion Stop Payment

                #region Lien 
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.LIEN_MARK).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Lien Mark Approval Queue List";
                    LienDAL oLienDAL = new LienDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Lien Mark' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oLienDAL.LoadUnapprovedLienMarkList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtList = (DataTable)oResult.Return;
                    if (dtList != null)
                    {
                        gvData.DataSource = dtList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtList;
                    }
                }

                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.LIEN_MARK_REMOVE).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Lien Mark Remove Approval Queue List";
                    LienDAL oLienDAL = new LienDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Lien Mark Remove' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oLienDAL.LoadUnapprovedLienRemoveMarkList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtList = (DataTable)oResult.Return;
                    if (dtList != null)
                    {
                        gvData.DataSource = dtList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtList;
                    }
                }
                #endregion Lien

                #region Reinvestment 
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.REINVESTMENT).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Reinvestment Approval Queue List";
                    ReinvestmentDAL oRiDAL = new ReinvestmentDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Reinvestment' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oRiDAL.LoadUnapprovedList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtList = (DataTable)oResult.Return;
                    if (dtList != null)
                    {
                        gvData.DataSource = dtList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtList;
                    }
                }
                #endregion Reinvestment

                #region ACE Register
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.ACE_MANAGER).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "ACE Registration Queue List";
                    IssueDAL oIssueDAL = new IssueDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oIssueDAL.LoadUnapprovedACERegisterList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtList = (DataTable)oResult.Return;
                    if (dtList != null)
                    {
                        gvData.DataSource = dtList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtList;
                    }
                }
                #endregion ACE Register

                //Make Filter By User
                //Config oConfig = Session[Constants.SES_USER_CONFIG] as Config;
                if (!string.IsNullOrEmpty( oConfig.FilterMakerID))
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

                Config oConfig = Session[Constants.SES_USER_CONFIG] as Config;
                oConfig.FilterMakerID = string.Empty;
            }
        }
    }
}

