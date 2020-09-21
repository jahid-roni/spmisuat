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
using SBM_BLC1.Common;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Configuration;
using SBM_BLC1.Entity.Configuration;
using System.Data;

public partial class SPWiseAccountMapping : System.Web.UI.Page
{

    #region Local Variable

    CryptographyManager oCrypManager = new CryptographyManager();
    public const string OBJ_SPTYPE_ID = "SPTypeID";
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
                if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CONFIG.SP_WISEA_CCOUNT))
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
        DDListUtil.LoadDDLFromDB(ddlSPTypeID, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
        gvList.PageSize = (int)Constants.PAGING_UNAPPROVED;

        string sPTypeID = Request.QueryString[OBJ_SPTYPE_ID];
        string sPageID = Request.QueryString[OBJ_PAGE_ID];
        if (!string.IsNullOrEmpty(sPTypeID))
        {
            sPTypeID = oCrypManager.GetDecryptedString(sPTypeID, Constants.CRYPT_PASSWORD_STRING);
        }
        if (!string.IsNullOrEmpty(sPageID))
        {
            sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
        }

        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (!string.IsNullOrEmpty(sPTypeID) && !string.IsNullOrEmpty(sPageID))
        {
            string sOperationType = sPageID.Substring(4, 1);
            if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
            {                
                LoadDataByID(sPTypeID);

                // general Control
                Util.ControlEnabled(ddlSPTypeID, false);

                Util.ControlEnabled(txtStoInHandAcc, false);
                Util.ControlEnabled(txtStoInHandName, false);
                Util.ControlEnabled(txtLibOnStoAcc, false);
                Util.ControlEnabled(txtLibOnStoName, false);
                Util.ControlEnabled(txtHolAcc, false);
                Util.ControlEnabled(txtHolName, false);
                Util.ControlEnabled(txtAccComAcc, false);
                Util.ControlEnabled(txtComName, false);
                Util.ControlEnabled(txtAdvAgiIntAcc, false);
                Util.ControlEnabled(txtAdvAgiIntName, false);
                Util.ControlEnabled(txtAdvAgiPriAcc, false);
                Util.ControlEnabled(txtAdvAgiPriName, false);

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
            Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_WISEA_CCOUNT).PadLeft(5, '0'), false);
        }
        else
        {
            // no action
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_WISEA_CCOUNT).PadLeft(5, '0'), false);
    } 

    protected void btnLoad_Click(object sender, EventArgs e)
    {
        string sValue = ddlSPTypeID.SelectedItem.Value;
        ClearTextValue();
        hdDataType.Value = "L";
        LoadDataByID(sValue);
        ddlSPTypeID.SelectedValue = sValue;

    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdSPTypeID.Value))
        {
            SPTypeWiseAccountMappingDAL oSPTypeWiseAccountMappingDAL = new SPTypeWiseAccountMappingDAL();
            Result oResult = (Result)oSPTypeWiseAccountMappingDAL.Detete(hdSPTypeID.Value);
            if (oResult.Status)
            {
                ClearTextValue();
                this.LoadList();
                hdSPTypeID.Value = "";

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
        txtAccComAcc.Text = string.Empty;
        txtComName.Text = string.Empty;
        txtAdvAgiIntAcc.Text = string.Empty;
        txtAdvAgiIntName.Text = string.Empty;
        txtAdvAgiPriAcc.Text = string.Empty;
        txtAdvAgiPriName.Text = string.Empty;
        txtHolAcc.Text = string.Empty;
        txtHolName.Text = string.Empty;
        txtLibOnStoAcc.Text = string.Empty;
        txtLibOnStoName.Text = string.Empty;
        txtStoInHandAcc.Text = string.Empty;
        txtStoInHandName.Text = string.Empty;
        ddlSPTypeID.Enabled = true;
        ddlSPTypeID.SelectedIndex = 0;
        ucUserDet.ResetData();
        hdDataType.Value = string.Empty;
    }

    protected void btnReject_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdSPTypeID.Value))
        {
            SPTypeWiseAccountMapping oSPTypeWiseAccountMapping = new SPTypeWiseAccountMapping(hdSPTypeID.Value);
            SPTypeWiseAccountMappingDAL oSPTypeWiseAccountMappingDAL = new SPTypeWiseAccountMappingDAL();

            oSPTypeWiseAccountMapping.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oSPTypeWiseAccountMappingDAL.Reject(oSPTypeWiseAccountMapping);
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
        if (!string.IsNullOrEmpty(hdSPTypeID.Value))
        {
            SPTypeWiseAccountMapping oSPTypeWiseAccountMapping = new SPTypeWiseAccountMapping(hdSPTypeID.Value);
            SPTypeWiseAccountMappingDAL oSPTypeWiseAccountMappingDAL = new SPTypeWiseAccountMappingDAL();

            oSPTypeWiseAccountMapping.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oSPTypeWiseAccountMappingDAL.Approve(oSPTypeWiseAccountMapping);
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
        SPTypeWiseAccountMapping oSPTypeWiseAccountMapping = new SPTypeWiseAccountMapping();
        SPTypeWiseAccountMappingDAL oSPTypeWiseAccountMappingDAL = new SPTypeWiseAccountMappingDAL();
        if (!string.IsNullOrEmpty(ddlSPTypeID.SelectedItem.Value))
        {
            oSPTypeWiseAccountMapping.SPType.SPTypeID = ddlSPTypeID.SelectedItem.Value;
        }
        oSPTypeWiseAccountMapping.StockInHandAcc = SBM_BLC1.Common.String.RemoveSeperator(txtStoInHandAcc.Text);
        oSPTypeWiseAccountMapping.StockInHandAccName = txtStoInHandName.Text.Trim();
        oSPTypeWiseAccountMapping.LiabilityOnStockAcc = SBM_BLC1.Common.String.RemoveSeperator(txtLibOnStoAcc.Text);
        oSPTypeWiseAccountMapping.LiabilityOnStockAccName = txtLibOnStoName.Text.Trim();
        oSPTypeWiseAccountMapping.HoldingAcc = SBM_BLC1.Common.String.RemoveSeperator(txtHolAcc.Text);
        oSPTypeWiseAccountMapping.HoldingAccName = txtHolName.Text.Trim();
        oSPTypeWiseAccountMapping.AccruedInterestAcc = SBM_BLC1.Common.String.RemoveSeperator(txtAccComAcc.Text);
        oSPTypeWiseAccountMapping.AccruedInterestAccName = txtComName.Text.Trim();
        oSPTypeWiseAccountMapping.AdvAgainstInterestAcc = SBM_BLC1.Common.String.RemoveSeperator(txtAdvAgiIntAcc.Text);
        oSPTypeWiseAccountMapping.AdvAgainstInterestAccName = txtAdvAgiIntName.Text.Trim();
        oSPTypeWiseAccountMapping.AdvAgainstPrincipalAcc = SBM_BLC1.Common.String.RemoveSeperator(txtAdvAgiPriAcc.Text);
        oSPTypeWiseAccountMapping.AdvAgainstPrincipalAccName = txtAdvAgiPriName.Text.Trim();

        oSPTypeWiseAccountMapping.UserDetails = ucUserDet.UserDetail;
        oSPTypeWiseAccountMapping.UserDetails.MakeDate = DateTime.Now;
        ucUserDet.ResetData();

        Result oResult = (Result)oSPTypeWiseAccountMappingDAL.Save(oSPTypeWiseAccountMapping);

        if (oResult.Status)
        {
            ClearTextValue();
            this.LoadList();
            hdSPTypeID.Value = "";

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

            this.gvList.HeaderRow.Cells[1].Text = "SP Type";
            this.gvList.HeaderRow.Cells[2].Text = "Stock In Hand Acc";
            this.gvList.HeaderRow.Cells[3].Text = "Liability On Stock Acc";
            this.gvList.HeaderRow.Cells[4].Text = "Holding Acc";
            this.gvList.HeaderRow.Cells[5].Text = "Accrued Interest Acc";
            this.gvList.HeaderRow.Cells[6].Text = "AdvAgainst Interest Acc";
            this.gvList.HeaderRow.Cells[7].Text = "AdvAgainst Principal Acc";
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
            e.Row.Cells[2].Text = SBM_BLC1.Common.String.AddSeperator(e.Row.Cells[2].Text);
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
            SPTypeWiseAccountMappingDAL oSPTypeWiseAccountMappingDAL = new SPTypeWiseAccountMappingDAL();

            Result oResult = oSPTypeWiseAccountMappingDAL.LoadUnapprovedList(oConfig.UserName, false);

            if (oResult.Status)
            {
                DataTable dtTmpDataList = (DataTable)oResult.Return;
                if (dtTmpDataList.Rows.Count > 0)
                {
                    dtTmpDataList.Columns.Remove("MakerID");
                    dtTmpDataList.Columns.Remove("CheckerID");
                    dtTmpDataList.Columns.Remove("CheckDate");
                    dtTmpDataList.Columns.Remove("CheckerComment");
                    dtTmpDataList.Columns.Remove("AccruedInterestAccName");
                    dtTmpDataList.Columns.Remove("AdvAgainstInterestAccName");
                    dtTmpDataList.Columns.Remove("AdvAgainstPrincipalAccName");
                    dtTmpDataList.Columns.Remove("AdvAgainstPrincipalAcc");
                    dtTmpDataList.Columns.Remove("AccruedInterestAcc");
                    dtTmpDataList.Columns.Remove("LiabilityOnStockAcc");

                    gvList.DataSource = dtTmpDataList;
                    gvList.DataBind();

                    Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpDataList;

                    this.gvList.HeaderRow.Cells[1].Text = "SP Type";
                    this.gvList.HeaderRow.Cells[2].Text = "Stock In Hand Acc";
                    this.gvList.HeaderRow.Cells[3].Text = "Liability On Stock Acc";
                    this.gvList.HeaderRow.Cells[4].Text = "Holding Acc";
                    this.gvList.HeaderRow.Cells[5].Text = "Accrued Interest Acc";
                    this.gvList.HeaderRow.Cells[6].Text = "AdvAgainst Interest Acc";
                    this.gvList.HeaderRow.Cells[7].Text = "AdvAgainst Principal Acc";
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


    private void LoadDataByID(string sSPTypeID)
    {
        SPTypeWiseAccountMapping oSPTypeWiseAccountMapping = new SPTypeWiseAccountMapping(sSPTypeID);
        SPTypeWiseAccountMappingDAL oSPWiseAccountMappingDAL = new SPTypeWiseAccountMappingDAL();
        Result oResult = new Result();
        oResult = oSPWiseAccountMappingDAL.LoadByID(oSPTypeWiseAccountMapping);
        if (oResult.Status)
        {
            oSPTypeWiseAccountMapping = (SPTypeWiseAccountMapping)oResult.Return;

            DDListUtil.Assign(ddlSPTypeID,oSPTypeWiseAccountMapping.SPType.SPTypeID.Trim());
            ddlSPTypeID.Enabled = false;
            txtStoInHandAcc.Text = SBM_BLC1.Common.String.AddSeperator(oSPTypeWiseAccountMapping.StockInHandAcc);
            txtStoInHandName.Text = oSPTypeWiseAccountMapping.StockInHandAccName;
            txtLibOnStoAcc.Text = SBM_BLC1.Common.String.AddSeperator(oSPTypeWiseAccountMapping.LiabilityOnStockAcc);
            txtLibOnStoName.Text = oSPTypeWiseAccountMapping.LiabilityOnStockAccName;
            txtHolAcc.Text = SBM_BLC1.Common.String.AddSeperator(oSPTypeWiseAccountMapping.HoldingAcc);
            txtHolName.Text = oSPTypeWiseAccountMapping.HoldingAccName;
            txtAccComAcc.Text = SBM_BLC1.Common.String.AddSeperator(oSPTypeWiseAccountMapping.AccruedInterestAcc);
            txtComName.Text = oSPTypeWiseAccountMapping.AccruedInterestAccName;
            txtAdvAgiIntAcc.Text = SBM_BLC1.Common.String.AddSeperator(oSPTypeWiseAccountMapping.AdvAgainstInterestAcc);
            txtAdvAgiIntName.Text = oSPTypeWiseAccountMapping.AdvAgainstInterestAccName;
            txtAdvAgiPriAcc.Text = SBM_BLC1.Common.String.AddSeperator(oSPTypeWiseAccountMapping.AdvAgainstPrincipalAcc);
            txtAdvAgiPriName.Text = oSPTypeWiseAccountMapping.AdvAgainstPrincipalAccName;
            if (string.IsNullOrEmpty(hdDataType.Value))
            {
                //When Loading from Approver End
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.MakerID = oSPTypeWiseAccountMapping.UserDetails.MakerID;
                userDetails.MakeDate = oSPTypeWiseAccountMapping.UserDetails.MakeDate;
                ucUserDet.UserDetail = userDetails;
            }
            else if (hdDataType.Value.Equals("T"))
            {
                //When loading from temp table
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.CheckDate = oSPTypeWiseAccountMapping.UserDetails.CheckDate;
                userDetails.CheckerID = oSPTypeWiseAccountMapping.UserDetails.CheckerID;
                userDetails.CheckDate = oSPTypeWiseAccountMapping.UserDetails.CheckDate;
                userDetails.CheckerComment = oSPTypeWiseAccountMapping.UserDetails.CheckerComment;
                ucUserDet.UserDetail = userDetails;
            } 
            
            hdSPTypeID.Value = sSPTypeID;
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
        }
    }   

    #endregion Supporting or Utility function

}
