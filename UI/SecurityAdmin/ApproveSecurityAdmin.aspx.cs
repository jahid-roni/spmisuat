/*
 * File name            : ApproveSecurityAdmin
 * Author               : A.K.M. Tanjil Hossain,
 *                      : A.K.M. Zahidul Quaium,
 *                      : Tanvir Alam
 *                      : Jerin Afsana
 * Date                 : Sep 01,2014
 * Version              : 1.0
 *
 * Description          : To approve all unapprove data.
 *
 * Modification history :
 * Name                         Date                            Desc
 * Tanvir Alam                Sep 01,2014                Business implementation 
 * A.K.M. Zahidul Quaium        February 02,2012                Business implementation              
 * Jerin Afsana                 April    02,2012                Business implementation              
 * Copyright (c) 2012: Softcell Solution Ltd
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SBM_BLC1.Entity.SecurityAdmin;
using SBM_BLC1.SecurityAdmin;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Common;
using System.Data;
using System.Collections;

namespace SBM_WebUI.mp
{
    public partial class ApproveSecurityAdmin : System.Web.UI.Page
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
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            string sPageID = string.Empty;
            string sType = Request.QueryString["pType"];

            if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_SA.USER).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_SA_USER + "?sUserID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_SA.DEPARTMENT).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_SA_DEPARTMENT + "?sDptID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_SA.DESIGNATION).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_SA_DESIGNATION + "?sDsgID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_SA.SECURITY_POLICY).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_SA_SECURITY_POLICY + "?sSPID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_SA.GROUP_PERMISSION).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_SA_GROUP_PERMISSION + "?sGPID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_SA.SCREEN_SETUP).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_SA_SCREEN_SETUP + "?sSCrID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
        }

        protected void InitializeData()
        {
            if (Session[Constants.SES_CONFIG_UNAPPROVE_DATA] == null)
            {
                Session.Add(Constants.SES_CONFIG_UNAPPROVE_DATA, new DataTable());
            }
            else
            {
                Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = new DataTable();
            }

            DDListUtil.LoadDDLFromDB(ddlUserName, "UserName", "UserName", "SA_User", false);
            SearchAction();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (ddlUserName.SelectedItem.Value != "")
            {
                if (Session[Constants.SES_SA_UNAPPROVE_DATA] != null)
                {
                    DataTable dtTmpList = (DataTable)Session[Constants.SES_SA_UNAPPROVE_DATA];
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

        protected void SearchAction()
        {
            string sType = Request.QueryString["pType"];
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            gvData.DataSource = null;
            gvData.DataBind();
            
            if (!string.IsNullOrEmpty(sType))
            {
                Session[Constants.SES_SA_UNAPPROVE_DATA] = new DataTable();

                if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_SA.USER).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "User Approval Queue List";
                    UserDAL oUserDAL = new UserDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'User' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oUserDAL.LoadUnapprovedList(null, true, oConfig.DivisionID, oConfig.BankCodeID);
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {
                            gvData.DataSource = dtTmpList;
                            gvData.DataBind();
                            Session[Constants.SES_SA_UNAPPROVE_DATA] = dtTmpList;
                        }
                    }
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_SA.DEPARTMENT).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Department Approval Queue List";
                    DepartmentDAL oDptDAL = new DepartmentDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Department' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oDptDAL.LoadUnapprovedList(null, true);
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        gvData.DataSource = dtTmpList;
                        gvData.DataBind();
                        Session[Constants.SES_SA_UNAPPROVE_DATA] = dtTmpList;
                    }
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_SA.DESIGNATION).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Designation Approval Queue List";
                    DesignationDAL oDsgDAL = new DesignationDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Designation' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oDsgDAL.LoadUnapprovedList(null, true);
                    DataTable dtTmpDsgList = (DataTable)oResult.Return;
                    if (dtTmpDsgList != null)
                    {
                        gvData.DataSource = dtTmpDsgList;
                        gvData.DataBind();
                        Session[Constants.SES_SA_UNAPPROVE_DATA] = dtTmpDsgList;
                    }
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_SA.SECURITY_POLICY).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Security policy Approval Queue List";
                    SecurityPolicyDAL oSPDAL = new SecurityPolicyDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Security policy' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oSPDAL.LoadUnapprovedList(null, true);
                    DataTable dtTmpSPList = (DataTable)oResult.Return;
                    if (dtTmpSPList != null)
                    {
                        gvData.DataSource = dtTmpSPList;
                        gvData.DataBind();
                        Session[Constants.SES_SA_UNAPPROVE_DATA] = dtTmpSPList;
                    }
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_SA.GROUP_PERMISSION).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Group Permission Approval Queue List";
                    GroupDAL oGPDAL = new GroupDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Group Permission' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = oGPDAL.LoadUnapprovedList(null, true);
                    DataTable dtTmpSPList = (DataTable)oResult.Return;
                    if (dtTmpSPList != null)
                    {
                        gvData.DataSource = dtTmpSPList;
                        gvData.DataBind();
                        Session[Constants.SES_SA_UNAPPROVE_DATA] = dtTmpSPList;
                    }
                }


                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_SA.SCREEN_SETUP).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Screen Setup Approval Queue List";
                    ScreenDAL sCRDAL = new ScreenDAL();
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Screen Setup' record found");
                    gvData.EmptyDataTemplate = tbGvData;

                    Result oResult = sCRDAL.LoadUnapprovedList(null, true);
                    DataTable dtTmpSPList = (DataTable)oResult.Return;
                    if (dtTmpSPList != null)
                    {
                        gvData.DataSource = dtTmpSPList;
                        gvData.DataBind();
                        Session[Constants.SES_SA_UNAPPROVE_DATA] = dtTmpSPList;
                    }
                }
            }
        }

        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, alAddSeperatorIndex);
        }
    }
}
