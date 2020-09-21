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

using SBM_BLC1.Common;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Configuration;
using SBM_BLC1.Entity.Configuration;


public partial class CurrencyWiseAccMapping : System.Web.UI.Page
{

    #region Local Variable

    CryptographyManager oCrypManager = new CryptographyManager();
    public const string OBJ_CURRENCY_ID = "CurrencyID";
    public const string OBJ_PAGE_ID = "sPageID";

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
                CheckPermission chkPer = new CheckPermission();
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CONFIG.CURRENCY_WISE_ACCOUNT_MAPPING))
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
        // Dropdown load SPType
        DDListUtil.LoadDDLFromDB(ddlCurrencyID, "CurrencyID", "CurrencyCode", "SPMS_Currency", true);
        
        gvList.PageSize = (int)Constants.PAGING_UNAPPROVED;
        string sCurrencyID = Request.QueryString[OBJ_CURRENCY_ID];
        string sPageID = Request.QueryString[OBJ_PAGE_ID];
        if (!string.IsNullOrEmpty(sCurrencyID))
        {
            sCurrencyID = oCrypManager.GetDecryptedString(sCurrencyID, Constants.CRYPT_PASSWORD_STRING);
        }
        if (!string.IsNullOrEmpty(sPageID))
        {
            sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
        }
        
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (!string.IsNullOrEmpty(sCurrencyID) && !string.IsNullOrEmpty(sPageID))
        {
            string sOperationType = sPageID.Substring(4, 1);
            if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
            {                
                LoadDataByID(sCurrencyID);

                // general Control
                Util.ControlEnabled(ddlCurrencyID, false);
                Util.ControlEnabled(txtSusPenAcc, false);
                Util.ControlEnabled(txtSusPenName, false);
                Util.ControlEnabled(txtForExcchAcc, false);
                Util.ControlEnabled(txtForExcchName, false);
                Util.ControlEnabled(txtBraFaxAcc, false);
                Util.ControlEnabled(txtBraFaxName, false);
                Util.ControlEnabled(txtBraExcFaxAcc, false);
                Util.ControlEnabled(txtBraExcFaxName, false);
                Util.ControlEnabled(txtBngBankAcc, false);
                Util.ControlEnabled(txtBngBankName, false);


                // user Detail
                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);

                // button 
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true);

                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);
                Util.ControlEnabled(btnLoad, false);

                #region User-Detail.
                UserDetails oUserDetails = ucUserDet.UserDetail;
                oUserDetails.CheckerID = oConfig.UserName;
                oUserDetails.CheckDate = DateTime.Now;
                ucUserDet.UserDetail = oUserDetails;
                #endregion User-Detail.

                fsList.Visible = false;
            }
        }
        else
        {
            // button 
            Util.ControlEnabled(btnReject, false);
            Util.ControlEnabled(btnApprove, false);
            Util.ControlEnabled(btnBack, false);

            Util.ControlEnabled(btnReset, true);
            Util.ControlEnabled(btnSave, true);
            Util.ControlEnabled(btnDelete, true);
            Util.ControlEnabled(btnLoad, true);
            Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), false);
            #region User-Detail.            
            UserDetails oUserDetails = new UserDetails();
            oUserDetails.MakerID = oConfig.UserName;
            oUserDetails.MakeDate = DateTime.Now;
            ucUserDet.UserDetail = oUserDetails;
            #endregion User-Detail.

            fsList.Visible = true;
            LoadList();
        }
    }

    #endregion InitializeData


    #region Basic Operational Function from control EVENT

    public void PopLoadActionCommSpAndCur(GridViewRow gvRow)
    {
        if (gvRow != null)
        {
            hdDataType.Value = "M";
            LoadDataByID(gvRow.Cells[1].Text);
        }
    }

    public void PopErrorMsgAction(string sType)
    {
        if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
        {
            Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CURRENCY_WISE_ACCOUNT_MAPPING).PadLeft(5, '0'), false);
        }
        else
        {
            // no action
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CURRENCY_WISE_ACCOUNT_MAPPING).PadLeft(5, '0'), false);
    }

    protected void btnLoad_Click(object sender, EventArgs e)
    {
        string sValue = ddlCurrencyID.SelectedItem.Value;
        ClearTextValue();
        hdDataType.Value = "L";
        LoadDataByID(sValue);
        ddlCurrencyID.SelectedValue = sValue;
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdnCurrencyID.Value))
        {
            CurrencyWiseAccountMappingDAL oCurrencyWiseAccountMappingDAL = new CurrencyWiseAccountMappingDAL();
            Result oResult = (Result)oCurrencyWiseAccountMappingDAL.Detete(hdnCurrencyID.Value);
            if (oResult.Status)
            {
                ClearTextValue();
                this.LoadList();
                hdnCurrencyID.Value = "";
                ucMessage.OpenMessage(Constants.MSG_SUCCESS_DELETE, Constants.MSG_TYPE_SUCCESS);
            }
            else
            {
                if (oResult.Message.Equals(Constants.TABLE_MAIN))
                {
                    ucMessage.OpenMessage(Constants.MSG_APPROVED_DELETE_DATA, Constants.MSG_TYPE_ERROR);
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_DELETE, Constants.MSG_TYPE_ERROR);
                }
            }
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_DELETE, Constants.MSG_TYPE_ERROR);
        }
    }

    private void ClearTextValue()
    {
        txtBngBankAcc.Text = string.Empty;
        txtBngBankName.Text = string.Empty;
        txtBraExcFaxAcc.Text = string.Empty;
        txtBraExcFaxName.Text = string.Empty;
        txtBraFaxAcc.Text = string.Empty;
        txtBraFaxName.Text = string.Empty;
        txtForExcchAcc.Text = string.Empty;
        txtForExcchName.Text = string.Empty;
        txtSusPenAcc.Text = string.Empty;
        txtSusPenName.Text = string.Empty;
        if (ddlCurrencyID.Items.Count > 0)
        {
            ddlCurrencyID.SelectedIndex = 0;
            ddlCurrencyID.Enabled = true;
        }
        ucUserDet.ResetData();
        hdDataType.Value = string.Empty;
    }

    protected void btnReject_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdnCurrencyID.Value))
        {
            CurrencyWiseAccountMapping oCurrencyWiseAccountMapping = new CurrencyWiseAccountMapping(hdnCurrencyID.Value);
            CurrencyWiseAccountMappingDAL oCurrencyWiseAccountMappingDAL = new CurrencyWiseAccountMappingDAL();

            oCurrencyWiseAccountMapping.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oCurrencyWiseAccountMappingDAL.Reject(oCurrencyWiseAccountMapping);
            if (oResult.Status)
            {
                ucMessage.OpenMessage(Constants.MSG_SUCCESS_REJECT, Constants.MSG_TYPE_SUCCESS);
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_REJECT, Constants.MSG_TYPE_ERROR);
            }
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_REJECT, Constants.MSG_TYPE_ERROR);
        }
    }

    protected void btnApprove_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdnCurrencyID.Value))
        {
            CurrencyWiseAccountMapping oCurrencyWiseAccountMapping = new CurrencyWiseAccountMapping(hdnCurrencyID.Value);
            CurrencyWiseAccountMappingDAL oCurrencyWiseAccountMappingDAL = new CurrencyWiseAccountMappingDAL();

            oCurrencyWiseAccountMapping.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oCurrencyWiseAccountMappingDAL.Approve(oCurrencyWiseAccountMapping);
            if (oResult.Status)
            {
                ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE, Constants.MSG_TYPE_SUCCESS);
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_APPROVE, Constants.MSG_TYPE_ERROR);
            }
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_APPROVE, Constants.MSG_TYPE_ERROR);
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        CurrencyWiseAccountMapping oCurrencyWiseAccountMapping = new CurrencyWiseAccountMapping();
        CurrencyWiseAccountMappingDAL oCurrencyWiseAccountMappingDAL = new CurrencyWiseAccountMappingDAL();

        oCurrencyWiseAccountMapping.Currency.CurrencyID = ddlCurrencyID.SelectedItem.Value;

        oCurrencyWiseAccountMapping.SuspenseAcc = SBM_BLC1.Common.String.RemoveSeperator(txtSusPenAcc.Text);
        oCurrencyWiseAccountMapping.SuspenseAccName = txtSusPenName.Text;
        oCurrencyWiseAccountMapping.ForeignExchangeAcc = SBM_BLC1.Common.String.RemoveSeperator(txtForExcchAcc.Text);
        oCurrencyWiseAccountMapping.ForeignExchangeAccName = txtForExcchName.Text;
        oCurrencyWiseAccountMapping.BranchFxAcc = SBM_BLC1.Common.String.RemoveSeperator(txtBraFaxAcc.Text);
        oCurrencyWiseAccountMapping.BranchFxAccName = txtBraFaxName.Text;
        oCurrencyWiseAccountMapping.BranchExFxAcc = SBM_BLC1.Common.String.RemoveSeperator(txtBraExcFaxAcc.Text);
        oCurrencyWiseAccountMapping.BranchExFxAccName = txtBraExcFaxName.Text;
        oCurrencyWiseAccountMapping.BangladesgBankAcc = SBM_BLC1.Common.String.RemoveSeperator(txtBngBankAcc.Text);
        oCurrencyWiseAccountMapping.BangladesgBankAccName = txtBngBankName.Text;

        oCurrencyWiseAccountMapping.UserDetails = ucUserDet.UserDetail;
        oCurrencyWiseAccountMapping.UserDetails.MakeDate = DateTime.Now;
        ucUserDet.ResetData();

        Result oResult = (Result)oCurrencyWiseAccountMappingDAL.Save(oCurrencyWiseAccountMapping);

        if (oResult.Status)
        {
            ClearTextValue();
            this.LoadList();
            hdnCurrencyID.Value = "";

            ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
        }
    }

    protected void gvList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvList.PageIndex = e.NewPageIndex;
        if (Session[Constants.SES_CONFIG_UNAPPROVE_DATA] != null)
        {
            DataTable dtTmpList = (DataTable)Session[Constants.SES_CONFIG_UNAPPROVE_DATA];
            gvList.DataSource = dtTmpList;
            gvList.DataBind();

            this.gvList.HeaderRow.Cells[1].Text = "Currency ID";
            this.gvList.HeaderRow.Cells[2].Text = "Currency Code";
            this.gvList.HeaderRow.Cells[3].Text = "Suspense Acc";
            this.gvList.HeaderRow.Cells[4].Text = "Foreign Exchange Acc";
            this.gvList.HeaderRow.Cells[5].Text = "Branch Fx Acc";
            this.gvList.HeaderRow.Cells[6].Text = "Branch ExFx Acc";
            this.gvList.HeaderRow.Cells[7].Text = "Bangladesg Bank Acc";
            this.gvList.HeaderRow.Cells[8].Text = "Make Date";
        }
    }

    protected void gvList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (!e.CommandName.Equals("Page"))
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            hdDataType.Value = "T";
            LoadDataByID(gvRow.Cells[1].Text);
        }
    }

    protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //e.Row.Cells[2].Visible = false; // for hiding Currency ID

        if (e.Row.RowType == DataControlRowType.DataRow)
        {            
            DataTable dt = (DataTable)gvList.DataSource;
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
            e.Row.Cells[3].Text = SBM_BLC1.Common.String.AddSeperator(e.Row.Cells[3].Text);
            e.Row.Cells[4].Text = SBM_BLC1.Common.String.AddSeperator(e.Row.Cells[4].Text);
            e.Row.Cells[5].Text = SBM_BLC1.Common.String.AddSeperator(e.Row.Cells[5].Text);
            e.Row.Cells[6].Text = SBM_BLC1.Common.String.AddSeperator(e.Row.Cells[6].Text);
            e.Row.Cells[7].Text = SBM_BLC1.Common.String.AddSeperator(e.Row.Cells[7].Text);
        }
    }

    #endregion Basic Operational Function from control EVENT


    #region Supporting or Utility function

    public void LoadList()
    {
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (oConfig != null)
        {
            CurrencyWiseAccountMapping oCurrencyWiseAccountMapping = new CurrencyWiseAccountMapping();
            CurrencyWiseAccountMappingDAL oCurrencyWiseAccountMappingDAL = new CurrencyWiseAccountMappingDAL();
            Result oResult = oCurrencyWiseAccountMappingDAL.LoadUnapprovedList(oConfig.UserName, false);

            if (oResult.Status)
            {
                DataTable dtTmpList = (DataTable)oResult.Return;
                if (dtTmpList.Rows.Count > 0)
                {
                    dtTmpList.Columns.Remove("MakerID");
                    dtTmpList.Columns.Remove("SuspenseAccName");
                    dtTmpList.Columns.Remove("ForeignExchangeAccName");
                    dtTmpList.Columns.Remove("BranchFxAccName");
                    dtTmpList.Columns.Remove("BranchExFxAccName");
                    dtTmpList.Columns.Remove("BangladesgBankAccName");

                    gvList.DataSource = dtTmpList;
                    gvList.DataBind();

                    Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;

                    this.gvList.HeaderRow.Cells[1].Text = "Currency ID";
                    this.gvList.HeaderRow.Cells[2].Text = "Currency Code";
                    this.gvList.HeaderRow.Cells[3].Text = "Suspense Acc";
                    this.gvList.HeaderRow.Cells[4].Text = "Foreign Exchange Acc";
                    this.gvList.HeaderRow.Cells[5].Text = "Branch Fx Acc";
                    this.gvList.HeaderRow.Cells[6].Text = "Branch ExFx Acc";
                    this.gvList.HeaderRow.Cells[7].Text = "Bangladesg Bank Acc";
                    this.gvList.HeaderRow.Cells[8].Text = "Make Date";
                }
                else
                {
                    gvList.DataSource = null;
                    gvList.DataBind();
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
            }
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
        }
    }

    private void LoadDataByID(string sCurrencyID)
    {
        CurrencyWiseAccountMapping oCurrencyWiseAccountMapping = new CurrencyWiseAccountMapping(sCurrencyID);
        CurrencyWiseAccountMappingDAL oCurrencyWiseAccountMappingDAL = new CurrencyWiseAccountMappingDAL();
        Result oResult = new Result();
        oResult = oCurrencyWiseAccountMappingDAL.LoadByID(oCurrencyWiseAccountMapping);
        if (oResult.Status)
        {
            oCurrencyWiseAccountMapping = (CurrencyWiseAccountMapping)oResult.Return;

            DDListUtil.Assign(ddlCurrencyID, oCurrencyWiseAccountMapping.Currency.CurrencyID);
            ddlCurrencyID.Enabled = false;
            txtSusPenAcc.Text = SBM_BLC1.Common.String.AddSeperator(oCurrencyWiseAccountMapping.SuspenseAcc);
            txtSusPenName.Text = oCurrencyWiseAccountMapping.SuspenseAccName;
            txtForExcchAcc.Text = SBM_BLC1.Common.String.AddSeperator(oCurrencyWiseAccountMapping.ForeignExchangeAcc);
            txtForExcchName.Text = oCurrencyWiseAccountMapping.ForeignExchangeAccName;
            txtBraFaxAcc.Text = SBM_BLC1.Common.String.AddSeperator(oCurrencyWiseAccountMapping.BranchFxAcc);
            txtBraFaxName.Text = oCurrencyWiseAccountMapping.BranchFxAccName;
            txtBraExcFaxAcc.Text = SBM_BLC1.Common.String.AddSeperator(oCurrencyWiseAccountMapping.BranchExFxAcc);
            txtBraExcFaxName.Text = oCurrencyWiseAccountMapping.BranchExFxAccName;
            txtBngBankAcc.Text = SBM_BLC1.Common.String.AddSeperator(oCurrencyWiseAccountMapping.BangladesgBankAcc);
            txtBngBankName.Text = oCurrencyWiseAccountMapping.BangladesgBankAccName;
            if (string.IsNullOrEmpty(hdDataType.Value))
            {
                //When Loading from Approver End
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.MakerID = oCurrencyWiseAccountMapping.UserDetails.MakerID;
                userDetails.MakeDate = oCurrencyWiseAccountMapping.UserDetails.MakeDate;
                ucUserDet.UserDetail = userDetails;
            }
            else if (hdDataType.Value.Equals("T"))
            {
                //When loading from temp table
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.CheckDate = oCurrencyWiseAccountMapping.UserDetails.CheckDate;
                userDetails.CheckerID = oCurrencyWiseAccountMapping.UserDetails.CheckerID;
                userDetails.CheckDate = oCurrencyWiseAccountMapping.UserDetails.CheckDate;
                userDetails.CheckerComment = oCurrencyWiseAccountMapping.UserDetails.CheckerComment;
                ucUserDet.UserDetail = userDetails;
            }             

            hdnCurrencyID.Value = sCurrencyID;
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
        }
    }

    #endregion Supporting or Utility function

}
