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
    public partial class BBDocumentInquiry : System.Web.UI.Page
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

                //#region Issue
                //else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE).PadLeft(5, '0')))
                //{
                //Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                //if (oConfig.GroupID != "1" && oConfig.GroupID != "12")
                //{
                //    if (gvRow.Cells[8].Text.Contains("MANAGER"))
                //    {
                //        ucMessage.OpenMessage("This registration required Manager Approval. Please check.", Constants.MSG_TYPE_ERROR);
                //        ScriptManager.RegisterStartupScript(this.upGv, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                //        return;
                //    }
                //}

                //    Response.Redirect(Constants.PAGE_TRAN_ISSUE + "?sTransID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                //}
                //else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_UPDATE).PadLeft(5, '0')))
                //{
                //    Response.Redirect(Constants.PAGE_TRAN_ISSUE_UPDATE + "?sIsUpdate=" + oCrypManager.GetEncryptedString("0") + "&sRegID=" + oCrypManager.GetEncryptedString(gvRow.Cells[2].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                //}
                //else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_OLD).PadLeft(5, '0')))
                //{
                //    Response.Redirect(Constants.PAGE_TRAN_ISSUE_OLD_CUSTOMER + "?sRegID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
                //}
                //#endregion Issue
                if (gvRow.Cells[1].Text != "")
                {
                    IssueDAL oIssueDAL = new IssueDAL();
                    Result oResult = oIssueDAL.LoadBBDocument_PDF(gvRow.Cells[1].Text);
                    if (oResult.Status)
                    {
                        if (((DataTable)oResult.Return).Rows.Count > 0)
                        {
                            Session[Constants.SES_PDF_DATA] = (byte[])(((DataTable)oResult.Return).Rows[0]["LetterImage"]);
                            Page.RegisterStartupScript(Constants.PDF_WINDOW, Util.OpenPDFView());
                        }
                    }
                }
            }
        }

        protected void InitializeData()
        {
            //gvData.PageSize = (int)Constants.PAGING_UNAPPROVED;

            txtFromDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtToDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            DataTable dtTmpGetData = new DataTable();
            gvData.DataSource = dtTmpGetData;
            gvData.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //if (txtLetterRefNo.Text == "")
            //{
            //    ucMessage.OpenMessage("BB letter ref no cannot be empty. Please check.", Constants.MSG_TYPE_ERROR);
            //    ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            //    return;
            //}
            if (ddlLetterType.SelectedValue== "")
            {
                ucMessage.OpenMessage("BB letter tye cannot be empty. Please check.", Constants.MSG_TYPE_ERROR);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                return;
            }
            SearchAction();
            
        }

        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvData.PageIndex = e.NewPageIndex;
            if (Session[Constants.SES_DOCUMENT_INQUERY_DATA] != null)
            {
                DataTable dtTmpList = (DataTable)Session[Constants.SES_DOCUMENT_INQUERY_DATA];
                gvData.DataSource = dtTmpList;
                gvData.DataBind();
            }
        }

        protected void SearchAction()
        {

            gvData.DataSource = null;
            gvData.DataBind();

                Session[Constants.SES_DOCUMENT_INQUERY_DATA] = new DataTable();
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];


                    lgText.InnerHtml = "SP Issue Tracker Queue List";
                    IssueDAL oIssueDAL = new IssueDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Issue' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    string dtFromDate = Util.GetDateTimeByString(txtFromDate.Text).ToString("dd-MMM-yyyy");
                    string dtToDate = Util.GetDateTimeByString(txtToDate.Text).ToString("dd-MMM-yyyy");

                    Result oResult = oIssueDAL.LoadBBDocumentList(txtLetterRefNo.Text, ddlLetterType.SelectedValue, dtFromDate, dtToDate);
                    DataTable dtTmpGetData= (DataTable)oResult.Return;
                    if (dtTmpGetData != null)
                    {
                        gvData.DataSource = dtTmpGetData;
                        gvData.DataBind();
                        Session[Constants.SES_DOCUMENT_INQUERY_DATA] = dtTmpGetData;
                    }
                    else
                    {
                        dtTmpGetData = new DataTable();
                        gvData.DataSource = dtTmpGetData;
                        gvData.DataBind();
                    }
        }

        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, alAddSeperatorIndex);
        }

        
    }
}

