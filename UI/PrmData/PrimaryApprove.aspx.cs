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
 * Jerin Afsana                 April    02,2012                Business implementation              
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



public partial class PrimaryApprove : System.Web.UI.Page
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
                //if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CONFIG.CONFIG_APPROVE))
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
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        DDListUtil.LoadActiveUser(ddlUserName, "UserName", "UserName", "SA_User", (int)Constants.USER_GROUP.CHECKER, false, oConfig.DivisionID);
        SearchAction();
    }

    protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (!e.CommandName.Equals("Page"))
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            string sPageID = string.Empty;
            string sType = Request.QueryString["pType"];

            if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.BRANCH).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_BRANCH_SETUP + "?BranchID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.BD_BANKADDRESS).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_BB_ADDRESS_SETUP + "?SPTypeID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CURRENCY).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_CURRENCY_SETUP + "?CurrencyID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CURRENCY_WISE_ACCOUNT_MAPPING).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_CURRENCY_WISE_ACC_MAP_SETUP + "?CurrencyID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CUSTOMER_DETAIL).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_CUSTOMER_DETAIL_SETUP + "?CustomerID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CUSTOMER_TYPE).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_CUSTOMER_TYPE_SETUP + "?CustomerTypeID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CUSTOMER_TYPE_WISE_SP_LIMIT).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_CUSTOMER_TYPE_WISE_SP_LIMITSETUP + "?sCustomerID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sSpTypeID=" + oCrypManager.GetEncryptedString(gvRow.Cells[3].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.DIVISION).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_DIVISION_SETUP + "?DivisionID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_CERTIFICATE).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_CERTIFICATE_MAPPING + "?SPTypeID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text.Trim()) + "&sDenomID=" + oCrypManager.GetEncryptedString(gvRow.Cells[2].Text.Trim()) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_POLICY).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_POLICY_SETUP + "?sPolicyID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text.Trim()) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_REPORT).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_REPORT_MAPPING + "?SPTypeID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text.Trim()) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_TYPE).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_SPTYPE_SETUP + "?SPTypeID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_WISEA_CCOUNT).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_SP_WISE_ACC_MAP_SETUP + "?SPTypeID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SYSTEM_CONFIG).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_SYSTEM_CONFIG + "?CustomerDataFile=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.COMMON_MAPPING).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_COMMON_MAPPING + "?BaseCurrencyID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.BANK).PadLeft(5, '0')))
            {
                Response.Redirect(Constants.PAGE_BANK + "?BankID=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text) + "&sPageID=" + oCrypManager.GetEncryptedString(sType), false);
            }
        }
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
    public void SearchAction()
    {
        string sType = Request.QueryString["pType"];
        gvData.DataSource = null;
        gvData.DataBind();

        if (!string.IsNullOrEmpty(sType))
        {
            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = new DataTable();

            if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.BRANCH).PadLeft(5, '0')))
            {
                #region BRANCH
                lgText.InnerHtml = "Branch Approval Queue List";
                BranchDAL oBranchDAL = new BranchDAL();
                TemplateBuilder tbGvData = new TemplateBuilder();
                tbGvData.AppendLiteralString("No 'Branch' record found");
                gvData.EmptyDataTemplate = tbGvData;

                Result oResult = oBranchDAL.LoadUnapprovedList(null, true);
                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {
                            gvData.DataSource = dtTmpList;
                            gvData.DataBind();

                            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;

                            this.gvData.HeaderRow.Cells[1].Text = "Branch ID";
                            this.gvData.HeaderRow.Cells[2].Text = "Branch Name";
                            this.gvData.HeaderRow.Cells[3].Text = "BB Code";
                            this.gvData.HeaderRow.Cells[4].Text = "Address";
                            this.gvData.HeaderRow.Cells[5].Text = "Maker ID";
                            this.gvData.HeaderRow.Cells[6].Text = "Make Date";
                        }
                    }
                }
                #endregion BRANCH
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.BD_BANKADDRESS).PadLeft(5, '0')))
            {
                #region Bangladesh Bank Address
                lgText.InnerHtml = "Bangladesh Bank Address Approval Queue List";

                BBAddressDAL oBBAddressDAL = new BBAddressDAL();
                TemplateBuilder tbGvData = new TemplateBuilder();
                tbGvData.AppendLiteralString("No 'Bangladesh Bank Address' record found");
                gvData.EmptyDataTemplate = tbGvData;

                Result oResult = oBBAddressDAL.LoadUnapprovedList(null, true);
                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {

                            gvData.DataSource = dtTmpList;
                            gvData.DataBind();

                            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;

                            this.gvData.HeaderRow.Cells[1].Text = "SP Type";
                            this.gvData.HeaderRow.Cells[2].Text = "Sales Statemet Address";
                            this.gvData.HeaderRow.Cells[3].Text = "Commission Claim Address";
                            this.gvData.HeaderRow.Cells[4].Text = "Interest Claim Address";
                            this.gvData.HeaderRow.Cells[5].Text = "Encashment Claim Address";
                            this.gvData.HeaderRow.Cells[6].Text = "Reinvestment Address";
                            this.gvData.HeaderRow.Cells[7].Text = "Maker ID";
                            this.gvData.HeaderRow.Cells[8].Text = "Make Date";
                        }
                    }
                }
                #endregion Bangladesh Bank Address
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CURRENCY).PadLeft(5, '0')))
            {
                #region Currency
                lgText.InnerHtml = "Currency Approval Queue List";
                CurrencyDAL oCurrencyDAL = new CurrencyDAL();
                TemplateBuilder tbGvData = new TemplateBuilder();
                tbGvData.AppendLiteralString("No 'Currency' record found");
                gvData.EmptyDataTemplate = tbGvData;

                Result oResult = oCurrencyDAL.LoadUnapprovedList(null, true);
                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {
                            gvData.DataSource = dtTmpList;
                            gvData.DataBind();

                            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;

                            this.gvData.HeaderRow.Cells[1].Text = "Currency ID";
                            this.gvData.HeaderRow.Cells[2].Text = "Currency Code";
                            this.gvData.HeaderRow.Cells[3].Text = "Currency Symbol";
                            this.gvData.HeaderRow.Cells[4].Text = "Currency Description";
                            this.gvData.HeaderRow.Cells[5].Text = "Maker ID";
                            this.gvData.HeaderRow.Cells[6].Text = "Make Date";
                        }
                    }
                }
                #endregion Currency
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CURRENCY_WISE_ACCOUNT_MAPPING).PadLeft(5, '0')))
            {
                #region Currency Wise Account Mapping
                lgText.InnerHtml = "Currency Wise Account Mapping Approval Queue List";
                CurrencyWiseAccountMappingDAL oCurrencyWiseAccountMappingDAL = new CurrencyWiseAccountMappingDAL();
                TemplateBuilder tbGvData = new TemplateBuilder();
                tbGvData.AppendLiteralString("No 'Currency Wise Account Mapping ' record found");
                gvData.EmptyDataTemplate = tbGvData;

                Result oResult = oCurrencyWiseAccountMappingDAL.LoadUnapprovedList(null, true);
                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {
                            // for adding seperator....
                            alAddSeperatorIndex = new ArrayList();
                            alAddSeperatorIndex.Add(3);
                            alAddSeperatorIndex.Add(4);
                            alAddSeperatorIndex.Add(5);
                            alAddSeperatorIndex.Add(6);
                            alAddSeperatorIndex.Add(7);

                            dtTmpList.Columns.Remove("SuspenseAccName");
                            dtTmpList.Columns.Remove("ForeignExchangeAccName");
                            dtTmpList.Columns.Remove("BranchFxAccName");
                            dtTmpList.Columns.Remove("BranchExFxAccName");
                            dtTmpList.Columns.Remove("BangladesgBankAccName");

                            gvData.DataSource = dtTmpList;
                            gvData.DataBind();

                            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;

                            this.gvData.HeaderRow.Cells[1].Text = "Currency ID";
                            this.gvData.HeaderRow.Cells[2].Text = "Currency Code";
                            this.gvData.HeaderRow.Cells[3].Text = "Suspense Acc";
                            this.gvData.HeaderRow.Cells[4].Text = "Foreign Exchange Acc";
                            this.gvData.HeaderRow.Cells[5].Text = "Branch Fx Acc";
                            this.gvData.HeaderRow.Cells[6].Text = "Branch Ex Fx Acc";
                            this.gvData.HeaderRow.Cells[7].Text = "Bangladesg Bank Acc";
                            this.gvData.HeaderRow.Cells[8].Text = "Maker ID";
                            this.gvData.HeaderRow.Cells[9].Text = "Make Date";
                        }
                    }
                }
                #endregion Currency Wise Account Mapping
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CUSTOMER_DETAIL).PadLeft(5, '0')))
            {
                #region Customer Detail
                lgText.InnerHtml = "Customer Detail Approval Queue List";
                CustomerDetailsDAL oCustomerDetailsDAL = new CustomerDetailsDAL();
                TemplateBuilder tbGvData = new TemplateBuilder();
                tbGvData.AppendLiteralString("No 'Customer Detail' record found");
                gvData.EmptyDataTemplate = tbGvData;

                Result oResult = oCustomerDetailsDAL.LoadUnapprovedList(null, true);
                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {
                            gvData.DataSource = dtTmpList;
                            gvData.DataBind();

                            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;

                            this.gvData.HeaderRow.Cells[1].Text = "Customer ID";
                            this.gvData.HeaderRow.Cells[2].Text = "Customer Name";
                            this.gvData.HeaderRow.Cells[3].Text = "Date of Birth";
                            this.gvData.HeaderRow.Cells[4].Text = "Address";
                            this.gvData.HeaderRow.Cells[5].Text = "Phone";
                            this.gvData.HeaderRow.Cells[6].Text = "Email";
                            this.gvData.HeaderRow.Cells[7].Text = "Maker ID";
                            this.gvData.HeaderRow.Cells[8].Text = "Make Date";
                        }
                    }
                }
                #endregion Customer Detail
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CUSTOMER_TYPE).PadLeft(5, '0')))
            {
                #region Customer Type
                lgText.InnerHtml = "Customer Type Approval Queue List";
                CustomerTypeDAL oCustomerTypeDAL = new CustomerTypeDAL();
                TemplateBuilder tbGvData = new TemplateBuilder();
                tbGvData.AppendLiteralString("No 'Customer Type' record found");
                gvData.EmptyDataTemplate = tbGvData;

                Result oResult = oCustomerTypeDAL.LoadUnapprovedList(null, true);
                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {
                            gvData.DataSource = dtTmpList;
                            gvData.DataBind();

                            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;

                            this.gvData.HeaderRow.Cells[1].Text = "Customer Type ID";
                            this.gvData.HeaderRow.Cells[2].Text = "Customer Type Description";
                            this.gvData.HeaderRow.Cells[3].Text = "Max Members";
                            this.gvData.HeaderRow.Cells[4].Text = "Is Organization";
                            this.gvData.HeaderRow.Cells[5].Text = "Maker ID";
                            this.gvData.HeaderRow.Cells[6].Text = "Make Date";
                        }
                    }
                }
                #endregion Customer Type
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CUSTOMER_TYPE_WISE_SP_LIMIT).PadLeft(5, '0')))
            {
                #region Customer Type wise SP Type Maping
                lgText.InnerHtml = "Customer Type wise SP Type Maping Approval Queue List";

                CustomerTypeWiseSPLimitDAL oCustomerTypeWiseSPLimitDAL = new CustomerTypeWiseSPLimitDAL();
                TemplateBuilder tbGvData = new TemplateBuilder();
                tbGvData.AppendLiteralString("No 'Customer Type wise Sp Type Maping' record found");
                gvData.EmptyDataTemplate = tbGvData;

                Result oResult = oCustomerTypeWiseSPLimitDAL.LoadUnapprovedList(null, true);
                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {
                            gvData.DataSource = dtTmpList;
                            gvData.DataBind();

                            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;

                            this.gvData.HeaderRow.Cells[1].Text = "Customer Type ID";
                            this.gvData.HeaderRow.Cells[2].Text = "Customer Type";
                            this.gvData.HeaderRow.Cells[3].Text = "SP Type";
                            this.gvData.HeaderRow.Cells[4].Text = "Minimum Limit";
                            this.gvData.HeaderRow.Cells[5].Text = "Maximum Limit";
                            this.gvData.HeaderRow.Cells[6].Text = "Maker ID";
                            this.gvData.HeaderRow.Cells[7].Text = "Make Date";
                        }
                    }
                }
                #endregion Customer Type wise SP Type Maping
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.DIVISION).PadLeft(5, '0')))
            {
                #region Area Approval
                lgText.InnerHtml = "Area Approval Queue List";
                DivisionDAL oDivisionDAL = new DivisionDAL();
                TemplateBuilder tbGvData = new TemplateBuilder();
                tbGvData.AppendLiteralString("No 'Area' record found");
                gvData.EmptyDataTemplate = tbGvData;

                Result oResult = oDivisionDAL.LoadUnapprovedList(null, true);
                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {
                            dtTmpList.Columns.Remove("BranchID");

                            gvData.DataSource = dtTmpList;
                            gvData.DataBind();

                            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;

                            this.gvData.HeaderRow.Cells[1].Text = "Area ID";
                            this.gvData.HeaderRow.Cells[2].Text = "BB Code";
                            this.gvData.HeaderRow.Cells[3].Text = "Area Name";
                            this.gvData.HeaderRow.Cells[4].Text = "Address";
                            this.gvData.HeaderRow.Cells[5].Text = "Maker ID";
                            this.gvData.HeaderRow.Cells[6].Text = "Make Date";
                        }
                    }
                }
                #endregion Area Approval
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_CERTIFICATE).PadLeft(5, '0')))
            {
                #region Script Mapping Certificate
                lgText.InnerHtml = "Script Mapping Certificate Approval Queue List";
                ScripMappingDAL oScripMappingDAL = new ScripMappingDAL();
                TemplateBuilder tbGvData = new TemplateBuilder();
                tbGvData.AppendLiteralString("No 'Script Mapping Certificate' record found");
                gvData.EmptyDataTemplate = tbGvData;

                Result oResult = oScripMappingDAL.LoadUnapprovedList(null, true);
                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {
                            dtTmpList = DDListUtil.MapTableWithXML(dtTmpList, "ScriptFormatMapping", "ReportType", "SP", 2);

                            gvData.DataSource = dtTmpList;
                            gvData.DataBind();

                            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;

                            this.gvData.HeaderRow.Cells[1].Text = "SP Type";
                            this.gvData.HeaderRow.Cells[2].Text = "Denomination";
                            this.gvData.HeaderRow.Cells[3].Text = "Script Format";
                            this.gvData.HeaderRow.Cells[4].Text = "Maker ID";
                            this.gvData.HeaderRow.Cells[5].Text = "Maker Date";
                        }
                    }
                }
                #endregion Script Mapping Certificate
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_POLICY).PadLeft(5, '0')))
            {
                #region SP Policy
                lgText.InnerHtml = "SP Policy Approval Queue List";
                SPPolicyDAL oSPPolicyDAL = new SPPolicyDAL();
                TemplateBuilder tbGvData = new TemplateBuilder();
                tbGvData.AppendLiteralString("No 'SP Policy' record found");
                gvData.EmptyDataTemplate = tbGvData;

                Result oResult = oSPPolicyDAL.LoadUnapprovedList(null, true);
                if (oResult != null)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {
                            dtTmpList.Columns.Remove("CheckerID");
                            dtTmpList.Columns.Remove("CheckDate");
                            dtTmpList.Columns.Remove("CheckerComment");
                            dtTmpList.Columns.Remove("PolicyEffectDate");
                            dtTmpList.Columns.Remove("IsSPDurationInMonth");
                            dtTmpList.Columns.Remove("SPInterestType");
                            dtTmpList.Columns.Remove("InterestTypeAfterIntPayment");
                            dtTmpList.Columns.Remove("PreMaturityInterestType");
                            dtTmpList.Columns.Remove("PreMatIntTypeAfterIntPayment");
                            dtTmpList.Columns.Remove("IsNomineePerScripRequired");
                            dtTmpList.Columns.Remove("IsBondHolderRequired");
                            dtTmpList.Columns.Remove("IsFoeignAddressRequired");
                            dtTmpList.Columns.Remove("SupportedSex");
                            dtTmpList.Columns.Remove("PartiallyEncashable");
                            dtTmpList.Columns.Remove("ReinvestmentSuported");
                            dtTmpList.Columns.Remove("InterestReinvestable");
                            dtTmpList.Columns.Remove("PartiallyEncashedReinvestable");
                            dtTmpList.Columns.Remove("MaxNoOfReinvestment");
                            dtTmpList.Columns.Remove("NonOrgCommission");
                            dtTmpList.Columns.Remove("NonOrgCommissionType");
                            dtTmpList.Columns.Remove("OrgCommission");
                            dtTmpList.Columns.Remove("OrgCommissionType");
                            dtTmpList.Columns.Remove("Levi");
                            dtTmpList.Columns.Remove("LeviType");
                            dtTmpList.Columns.Remove("IncomeTax");
                            dtTmpList.Columns.Remove("IncomeTaxType");
                            dtTmpList.Columns.Remove("IncomeTaxApplyAmount");
                            dtTmpList.Columns.Remove("IncomeTaxYearlyYN");
                            dtTmpList.Columns.Remove("IsOrganizationLeviTax");
                            dtTmpList.Columns.Remove("InterestRemuneration");
                            dtTmpList.Columns.Remove("InterestRemunerationType");
                            dtTmpList.Columns.Remove("Remuneration");
                            dtTmpList.Columns.Remove("RemunerationType");

                            gvData.DataSource = dtTmpList;
                            gvData.DataBind();

                            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;

                            this.gvData.HeaderRow.Cells[1].Text = "Policy ID";
                            this.gvData.HeaderRow.Cells[2].Text = "SP Type";
                            this.gvData.HeaderRow.Cells[3].Text = "SP Duration";
                            this.gvData.HeaderRow.Cells[4].Text = "No Of Coupons";
                            this.gvData.HeaderRow.Cells[5].Text = "Minimum Age";
                            this.gvData.HeaderRow.Cells[6].Text = "Maximum Age";
                            this.gvData.HeaderRow.Cells[7].Text = "Is Approved";
                            this.gvData.HeaderRow.Cells[8].Text = "Maker ID";
                            this.gvData.HeaderRow.Cells[9].Text = "Make Date";
                        }
                    }
                }
                #endregion SP Policy
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_REPORT).PadLeft(5, '0')))
            {
                #region Report Mapping
                lgText.InnerHtml = "Report Mapping Approval Queue List";
                SPReportMappingDAL oSPReportMappingDAL = new SPReportMappingDAL();
                TemplateBuilder tbGvData = new TemplateBuilder();
                tbGvData.AppendLiteralString("No 'Report Mapping' record found");
                gvData.EmptyDataTemplate = tbGvData;

                Result oResult = oSPReportMappingDAL.LoadUnapprovedList(null, true);
                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {
                            DataTable dtMappedData = null;

                            dtMappedData = DDListUtil.MapTableWithXML(dtTmpList, "ReportFormatMapping", "ReportType", "SS", 1);
                            dtMappedData = DDListUtil.MapTableWithXML(dtMappedData, "ReportFormatMapping", "ReportType", "CC", 2);
                            dtMappedData = DDListUtil.MapTableWithXML(dtMappedData, "ReportFormatMapping", "ReportType", "IC", 3);
                            dtMappedData = DDListUtil.MapTableWithXML(dtMappedData, "ReportFormatMapping", "ReportType", "EC", 4);

                            gvData.DataSource = dtMappedData;
                            gvData.DataBind();

                            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtMappedData;

                            this.gvData.HeaderRow.Cells[1].Text = "SP Type";
                            this.gvData.HeaderRow.Cells[2].Text = "Sales Statemet";
                            this.gvData.HeaderRow.Cells[3].Text = "Commission Claim";
                            this.gvData.HeaderRow.Cells[4].Text = "Interest Claim ";
                            this.gvData.HeaderRow.Cells[5].Text = "Encashment Claim";
                            this.gvData.HeaderRow.Cells[6].Text = "Maker ID";
                            this.gvData.HeaderRow.Cells[7].Text = "Maker Date";
                        }
                    }
                }
                #endregion Report Mapping
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_TYPE).PadLeft(5, '0')))
            {
                #region Sanchaya Patra Type
                lgText.InnerHtml = "Sanchaya Patra Type Approval Queue List";
                SPTypeDAL oSPTypeDAL = new SPTypeDAL();
                TemplateBuilder tbGvData = new TemplateBuilder();
                tbGvData.AppendLiteralString("No 'Sanchaya Patra Type' record found");
                gvData.EmptyDataTemplate = tbGvData;

                Result oResult = oSPTypeDAL.LoadTmpDataTableList();
                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {
                            gvData.DataSource = dtTmpList;
                            gvData.DataBind();

                            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;

                            this.gvData.HeaderRow.Cells[1].Text = "SP Type";
                            this.gvData.HeaderRow.Cells[2].Text = "Type Description";
                            this.gvData.HeaderRow.Cells[3].Text = "Currency Code";
                            this.gvData.HeaderRow.Cells[4].Text = "Maker ID";
                            this.gvData.HeaderRow.Cells[5].Text = "Make Date";
                        }
                    }
                }
                #endregion Sanchaya Patra Type
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_WISEA_CCOUNT).PadLeft(5, '0')))
            {
                #region Sanchaya Patra wise Account Mapping
                lgText.InnerHtml = "Sanchaya Patra wise Account Mapping Approval Queue List";
                SPTypeWiseAccountMappingDAL oSPTypeWiseAccountMappingDAL = new SPTypeWiseAccountMappingDAL();
                TemplateBuilder tbGvData = new TemplateBuilder();
                tbGvData.AppendLiteralString("No 'Sanchaya Patra wise Account Mapping' record found");
                gvData.EmptyDataTemplate = tbGvData;

                Result oResult = oSPTypeWiseAccountMappingDAL.LoadUnapprovedList(null, true);
                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {
                            // for adding seperator....
                            alAddSeperatorIndex = new ArrayList();
                            alAddSeperatorIndex.Add(2);
                            alAddSeperatorIndex.Add(3);
                            alAddSeperatorIndex.Add(4);
                            alAddSeperatorIndex.Add(5);
                            alAddSeperatorIndex.Add(6);
                            alAddSeperatorIndex.Add(7);

                            dtTmpList.Columns.Remove("StockInHandAccName");
                            dtTmpList.Columns.Remove("LiabilityOnStockAccName");
                            dtTmpList.Columns.Remove("HoldingAccName");
                            dtTmpList.Columns.Remove("AccruedInterestAccName");
                            dtTmpList.Columns.Remove("AdvAgainstInterestAccName");
                            dtTmpList.Columns.Remove("AdvAgainstPrincipalAccName");
                            dtTmpList.Columns.Remove("CheckerID");
                            dtTmpList.Columns.Remove("CheckDate");
                            dtTmpList.Columns.Remove("CheckerComment");

                            gvData.DataSource = dtTmpList;
                            gvData.DataBind();

                            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;

                            this.gvData.HeaderRow.Cells[1].Text = "SPType ID";
                            this.gvData.HeaderRow.Cells[2].Text = "Stock In Hand Acc";
                            this.gvData.HeaderRow.Cells[3].Text = "Liability On  Stock Acc";
                            this.gvData.HeaderRow.Cells[4].Text = "Holding Acc";
                            this.gvData.HeaderRow.Cells[5].Text = "Accrued Interest Acc";
                            this.gvData.HeaderRow.Cells[6].Text = "AdvAgainst Interest Acc";
                            this.gvData.HeaderRow.Cells[7].Text = "Adv Against Principal Acc";
                            this.gvData.HeaderRow.Cells[8].Text = "Maker ID";
                            this.gvData.HeaderRow.Cells[9].Text = "Make Date";
                        }
                    }
                }
                #endregion Sanchaya Patra wise Account Mapping
            }
            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SYSTEM_CONFIG).PadLeft(5, '0')))
            {
                #region System Configuration
                lgText.InnerHtml = "System Configuration Approval Queue List";
                SystemConfigurationDAL oSCDAL = new SystemConfigurationDAL();
                TemplateBuilder tbGvData = new TemplateBuilder();
                tbGvData.AppendLiteralString("No 'System Configuration' record found");
                gvData.EmptyDataTemplate = tbGvData;

                Result oResult = oSCDAL.LoadUnapprovedList(null, true);
                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {
                            dtTmpList.Columns.Remove("Dr_Code");
                            dtTmpList.Columns.Remove("Cr_Code");

                            gvData.DataSource = dtTmpList;
                            gvData.DataBind();

                            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;

                            this.gvData.HeaderRow.Cells[1].Text = "Customer Data File";
                            this.gvData.HeaderRow.Cells[2].Text = "Originator ID";
                            this.gvData.HeaderRow.Cells[3].Text = "Row Type Header";
                            this.gvData.HeaderRow.Cells[4].Text = "Row Type Footer";
                            this.gvData.HeaderRow.Cells[5].Text = "Dr Transaction Code";
                            this.gvData.HeaderRow.Cells[6].Text = "Cr Transaction Code";
                            this.gvData.HeaderRow.Cells[7].Text = "Maker ID";
                            this.gvData.HeaderRow.Cells[8].Text = "Make Date";
                        }
                    }
                }
                #endregion System Configuration
            }

            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.COMMON_MAPPING).PadLeft(5, '0')))
            {
                #region Common Mapping
                lgText.InnerHtml = "Common Mapping Approval Queue List";
                CommonMappingDAL oCmAL = new CommonMappingDAL();
                TemplateBuilder tbGvData = new TemplateBuilder();
                tbGvData.AppendLiteralString("No 'Common Mapping' record found");
                gvData.EmptyDataTemplate = tbGvData;

                Result oResult = oCmAL.LoadUnapprovedList(null, true);
                if (oResult.Status)
                {

                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {
                            alAddSeperatorIndex = new ArrayList();
                            alAddSeperatorIndex.Add(3);

                            gvData.DataSource = dtTmpList;
                            gvData.DataBind();

                            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;

                            this.gvData.HeaderRow.Cells[1].Text = "Currency ID";
                        }
                    }
                }
                #endregion Common Mapping
            }

            else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_CONFIG.BANK).PadLeft(5, '0')))
            {
                #region Common Mapping
                lgText.InnerHtml = "Bank Setup Approval Queue List";
                BankDAL oBankDAL = new BankDAL();
                TemplateBuilder tbGvData = new TemplateBuilder();
                tbGvData.AppendLiteralString("No 'Bank Setup' record found");
                gvData.EmptyDataTemplate = tbGvData;

                Result oResult = oBankDAL.LoadUnapprovedList(null, true);
                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList != null)
                    {
                        if (dtTmpList.Rows.Count > 0)
                        {
                            gvData.DataSource = dtTmpList;
                            gvData.DataBind();

                            this.gvData.HeaderRow.Cells[1].Text = "Bank ID";
                            this.gvData.HeaderRow.Cells[2].Text = "BB Code";
                            this.gvData.HeaderRow.Cells[3].Text = "Bank Name";
                            this.gvData.HeaderRow.Cells[4].Text = "Branch ID";
                            this.gvData.HeaderRow.Cells[5].Text = "Address";
                            this.gvData.HeaderRow.Cells[6].Text = "Maker ID";
                            this.gvData.HeaderRow.Cells[7].Text = "Make Date";

                            Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;
                        }
                    }
                }
                #endregion Common Mapping
            }
        }
    }

    protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataTable dt = (DataTable)gvData.DataSource;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dt.Rows[0][i].GetType() == typeof(DateTime))
                {
                    if (!e.Row.Cells[i + 1].Text.Equals("&nbsp;"))
                    {
                        e.Row.Cells[i + 1].Text = Convert.ToDateTime(e.Row.Cells[i + 1].Text).ToString("dd-MMM-yyyy");
                    }
                }
            }
            if (alAddSeperatorIndex != null)
            {
                for (int i = 0; i < alAddSeperatorIndex.Count; i++)
                {
                    e.Row.Cells[System.Convert.ToInt32(alAddSeperatorIndex[i])].Text = SBM_BLC1.Common.String.AddSeperator(e.Row.Cells[System.Convert.ToInt32(alAddSeperatorIndex[i])].Text);
                }
            }
        }
    }

}

