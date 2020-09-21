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
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Configuration;
using System.Data;

public partial class BDBankAddressSetup : System.Web.UI.Page
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
                if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CONFIG.BD_BANKADDRESS))
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
        DDListUtil.LoadDDLFromDB(ddlSPType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
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
                Util.ControlEnabled(ddlSPType, false);
                Util.ControlEnabled(txtSaleStatementAddress, false);
                Util.ControlEnabled(txtCommClaimAddress, false);
                Util.ControlEnabled(txtInrestClaimAddress, false);
                Util.ControlEnabled(txtEncashClaimAddress, false);
                Util.ControlEnabled(txtReinAddress, false);

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
                Util.ControlEnabled(btnSearch, false);

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
            Util.ControlEnabled(btnSearch, true);
            Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), false);
            #region User-Detail
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
            Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.BD_BANKADDRESS).PadLeft(5, '0'), false);
        }
        else
        {
            // no action
        }
    }

    protected void btnLoad_Click(object sender, EventArgs e)
    {
        string sValue = ddlSPType.SelectedItem.Value;
        ClearTextValue();
        hdDataType.Value = "L";
        LoadDataByID(sValue);
        ddlSPType.SelectedValue = sValue;
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdSPTypeID.Value))
        {
            BBAddressDAL oBBAddressDAL = new BBAddressDAL();
            Result oResult = (Result)oBBAddressDAL.Detete(hdSPTypeID.Value);
            if (oResult.Status)
            {
                LoadList();
                ClearTextValue();                
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
    
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.BD_BANKADDRESS).PadLeft(5, '0'), false);
    }   
 
    private void ClearTextValue()
    {
        ddlSPType.Enabled = true;
        if (ddlSPType.Items.Count > 0)
        {
            ddlSPType.SelectedIndex = 0;
        }
        txtCommClaimAddress.Text = string.Empty;
        txtEncashClaimAddress.Text = string.Empty;
        txtInrestClaimAddress.Text = string.Empty;
        txtReinAddress.Text = string.Empty;
        txtSaleStatementAddress.Text = string.Empty;
        ucUserDet.ResetData();
        hdDataType.Value = string.Empty;
    }

    protected void btnReject_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdSPTypeID.Value))
        {
            BBAddress oBBAddress = new BBAddress(hdSPTypeID.Value);
            BBAddressDAL oBBAddressDAL = new BBAddressDAL();
            oBBAddress.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oBBAddressDAL.Reject(oBBAddress);
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
            BBAddress oBBAddress = new BBAddress(hdSPTypeID.Value);
            BBAddressDAL oBBAddressDAL = new BBAddressDAL();
            oBBAddress.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oBBAddressDAL.Approve(oBBAddress);
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
        BBAddress oBBAddress = new BBAddress();
        BBAddressDAL oBBAddressDAL = new BBAddressDAL();
        if (!string.IsNullOrEmpty(ddlSPType.SelectedItem.Value))
        {
            oBBAddress.SPType.SPTypeID = ddlSPType.SelectedItem.Value;
        }
        oBBAddress.SalesStatemetAddress = txtSaleStatementAddress.Text;
        oBBAddress.CommissionClaimAddress = txtCommClaimAddress.Text;
        oBBAddress.InterestClaimAddress = txtInrestClaimAddress.Text;
        oBBAddress.EncashmentClaimAddress = txtEncashClaimAddress.Text;
        oBBAddress.ReinvestmentAddress = txtReinAddress.Text;

        oBBAddress.UserDetails = ucUserDet.UserDetail;
        oBBAddress.UserDetails.MakeDate = DateTime.Now;
        ucUserDet.ResetData();

        Result oResult = (Result)oBBAddressDAL.Save(oBBAddress);

        if (oResult.Status)
        {
            LoadList();
            ClearTextValue();
            hdSPTypeID.Value = "";

            ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
        }
    }

    protected void gvList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (!e.CommandName.Equals("Page"))
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            hdDataType.Value = "T";
            LoadDataByID(gvRow.Cells[1].Text.Trim());
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
        }
    }

    protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Util.GridDateFormat(e, gvList, null);
    }

    #endregion Basic Operational Function from control EVENT


    #region Supporting or Utility function

    public void LoadList()
    {
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (oConfig != null)
        {
            BBAddress oBBAddress = new BBAddress();
            BBAddressDAL oBBAddressDAL = new BBAddressDAL();

            Result oResult = oBBAddressDAL.LoadUnapprovedList(oConfig.UserName, false);

            if (oResult.Status)
            {
                DataTable dtTmpDataList = (DataTable)oResult.Return;
                if (dtTmpDataList.Rows.Count > 0)
                {
                    dtTmpDataList.Columns.Remove("MakerID");

                    gvList.DataSource = dtTmpDataList;
                    gvList.DataBind();

                    Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpDataList;

                    this.gvList.HeaderRow.Cells[1].Text = "SP Type";
                    this.gvList.HeaderRow.Cells[2].Text = "Sales Statemet Address";
                    this.gvList.HeaderRow.Cells[3].Text = "Commission Claim Address";
                    this.gvList.HeaderRow.Cells[4].Text = "Interest Claim Address";
                    this.gvList.HeaderRow.Cells[5].Text = "Encashment Claim Address";
                    this.gvList.HeaderRow.Cells[6].Text = "Reinvestment Address";
                    this.gvList.HeaderRow.Cells[7].Text = "Make Date";
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
        BBAddress oBBAddress = new BBAddress(sSPTypeID);
        BBAddressDAL oBBAddressDAL = new BBAddressDAL();
        Result oResult = new Result();
        oResult = oBBAddressDAL.LoadByID(oBBAddress);
        if (oResult.Status)
        {
            oBBAddress = (BBAddress)oResult.Return;

            DDListUtil.Assign(ddlSPType,oBBAddress.SPType.SPTypeID);
            ddlSPType.Enabled = false;
            txtSaleStatementAddress.Text=  oBBAddress.SalesStatemetAddress ;
            txtCommClaimAddress.Text = oBBAddress.CommissionClaimAddress;
            txtInrestClaimAddress.Text = oBBAddress.InterestClaimAddress;
            txtEncashClaimAddress.Text = oBBAddress.EncashmentClaimAddress;
            txtReinAddress.Text = oBBAddress.ReinvestmentAddress;

            if (string.IsNullOrEmpty(hdDataType.Value))
            {
                //When Loading from Approver End
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.MakerID = oBBAddress.UserDetails.MakerID;
                userDetails.MakeDate = oBBAddress.UserDetails.MakeDate;
                ucUserDet.UserDetail = userDetails;
            }
            else if (hdDataType.Value.Equals("T"))
            {
                //When loading from temp table
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.CheckerID = oBBAddress.UserDetails.CheckerID;
                userDetails.CheckDate = oBBAddress.UserDetails.CheckDate;
                userDetails.CheckerComment = oBBAddress.UserDetails.CheckerComment;
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
