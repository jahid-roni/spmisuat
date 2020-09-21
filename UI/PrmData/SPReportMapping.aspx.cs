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
public partial class SPReportMapping : System.Web.UI.Page
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
                if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CONFIG.SP_REPORT))
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
        SPTypeDAL oSPTypeDAL = new SPTypeDAL();
        gvList.PageSize = (int)Constants.PAGING_UNAPPROVED;

        // Dropdown load SPType
        DDListUtil.LoadDDLFromDB(ddlSPType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
        DDListUtil.LoadDDLFromXML(ddlSaleStatement, "ReportFormatMapping", "ReportType", "SS", true);
        DDListUtil.LoadDDLFromXML(ddlCommissionClaim, "ReportFormatMapping", "ReportType", "CC", true);
        DDListUtil.LoadDDLFromXML(ddlEncashmentClaim, "ReportFormatMapping", "ReportType", "EC", true);
        DDListUtil.LoadDDLFromXML(ddlInterestClaim, "ReportFormatMapping", "ReportType", "IC", true);

        string sSPTypeID = Request.QueryString[OBJ_SPTYPE_ID];
        string sPageID = Request.QueryString[OBJ_PAGE_ID];
        if (!string.IsNullOrEmpty(sSPTypeID))
        {
            sSPTypeID = oCrypManager.GetDecryptedString(sSPTypeID, Constants.CRYPT_PASSWORD_STRING);
        }
        if (!string.IsNullOrEmpty(sPageID))
        {
            sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
        }
        
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (!string.IsNullOrEmpty(sSPTypeID) && !string.IsNullOrEmpty(sPageID))
        {
            string sOperationType = sPageID.Substring(4, 1);
            if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
            {                
                LoadDataByID(sSPTypeID);

                // general Control
                Util.ControlEnabled(ddlCommissionClaim, false);
                Util.ControlEnabled(ddlEncashmentClaim, false);
                Util.ControlEnabled(ddlInterestClaim, false);
                Util.ControlEnabled(ddlSaleStatement, false);
                Util.ControlEnabled(ddlSPType, false);

                // user Detail
                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);

                // button 
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true);
                Util.ControlEnabled(btnSearch, false);
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
            Util.ControlEnabled(btnSearch, true);
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
            Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_REPORT).PadLeft(5, '0'), false);
        }
        else
        {
            // no action
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_REPORT).PadLeft(5, '0'), false);
    }

    protected void btnLoad_Click(object sender, EventArgs e)
    {
        string sTmp = ddlSPType.SelectedValue;
        ClearTextValue();
        hdDataType.Value = "L";
        LoadDataByID(sTmp);
        ddlSPType.SelectedValue = sTmp;
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdSPTypeID.Value))
        {
            SPReportMappingDAL oSPReportMappingDAL = new SPReportMappingDAL();
            Result oResult = (Result)oSPReportMappingDAL.Detete(hdSPTypeID.Value);
            if (oResult.Status)
            {
                this.LoadList();
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

    protected void btnReject_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdSPTypeID.Value))
        {
            SPReportMap oSPReportMap = new SPReportMap(hdSPTypeID.Value);
            SPReportMappingDAL oSPReportMappingDAL = new SPReportMappingDAL();
            oSPReportMap.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oSPReportMappingDAL.Reject(oSPReportMap);
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
            SPReportMap oSPReportMap = new SPReportMap(hdSPTypeID.Value);
            SPReportMappingDAL oSPReportMappingDAL = new SPReportMappingDAL();
            oSPReportMap.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oSPReportMappingDAL.Approve(oSPReportMap);
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

    protected void btnEncashmentPrv_Click(object sender, EventArgs e)
    {
        PreviewAction(ddlEncashmentClaim.SelectedValue, "EC");
    }
    protected void btnCommissionPrv_Click(object sender, EventArgs e)
    {
        PreviewAction(ddlCommissionClaim.SelectedValue , "CC");
    }
    
    protected void btnInterestPrv_Click(object sender, EventArgs e)
    {
        PreviewAction(ddlInterestClaim.SelectedValue, "IC");
    }
    
    protected void btnSalesPrv_Click(object sender, EventArgs e)
    {
        PreviewAction(ddlSaleStatement.SelectedValue, "SS");
    }
    

    private void PreviewAction(string sRptType , string sDDLType)
    {
        if (! string.IsNullOrEmpty(ddlSPType.SelectedValue))
        {
            if (sDDLType.Equals("EC"))
            {
                if (sRptType.Equals(""))
                { }
                else if (sRptType.Equals(""))
                { }
                else if (sRptType.Equals(""))
                { }
            }
            else if (sDDLType.Equals("CC"))
            {
                if (sRptType.Equals(""))
                { }
                else if (sRptType.Equals(""))
                { }
                else if (sRptType.Equals(""))
                { }
            }
            else if (sDDLType.Equals("IC"))
            {
                if (sRptType.Equals(""))
                { }
                else if (sRptType.Equals(""))
                { }
                else if (sRptType.Equals(""))
                { }
            }
            else if (sDDLType.Equals("SS"))
            {
                if (sRptType.Equals(""))
                { }
                else if (sRptType.Equals(""))
                { }
                else if (sRptType.Equals(""))
                { }
            }
        }
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        SPReportMap oSPReportMap = new SPReportMap();
        SPReportMappingDAL oSPReportMappingDAL = new SPReportMappingDAL();
        if (!string.IsNullOrEmpty(ddlSPType.SelectedValue))
        {
            oSPReportMap.SPTypeID = ddlSPType.SelectedValue;
        }
        if (!string.IsNullOrEmpty(ddlCommissionClaim.SelectedValue))
        {
            oSPReportMap.CommissionClaimFormat = Convert.ToInt16(ddlCommissionClaim.SelectedValue);
        }
        if (!string.IsNullOrEmpty(ddlEncashmentClaim.SelectedValue))
        {
            oSPReportMap.EncashmentClaimFormat = Convert.ToInt16(ddlEncashmentClaim.SelectedValue);
        }
        if (!string.IsNullOrEmpty(ddlInterestClaim.SelectedValue))
        {
            oSPReportMap.InterestClaimFormat = Convert.ToInt16(ddlInterestClaim.SelectedValue);
        }
        if (!string.IsNullOrEmpty(ddlSaleStatement.SelectedValue))
        {
            oSPReportMap.SalesStatemetFormat = Convert.ToInt16(ddlSaleStatement.SelectedValue);
        }

        oSPReportMap.UserDetails = ucUserDet.UserDetail;
        oSPReportMap.UserDetails.MakeDate = DateTime.Now;
        ucUserDet.ResetData();

        Result oResult = (Result)oSPReportMappingDAL.Save(oSPReportMap);

        if (oResult.Status)
        {
            this.LoadList();
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
            LoadDataByID(gvRow.Cells[1].Text);
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

    public void ClearTextValue()
    {
        ddlCommissionClaim.SelectedIndex = 0;
        ddlEncashmentClaim.SelectedIndex = 0;
        ddlInterestClaim.SelectedIndex = 0;
        ddlSaleStatement.SelectedIndex = 0;
        if (ddlSPType.Items.Count > 0)
        {
            ddlSPType.SelectedIndex = 0;
            ddlSPType.Enabled = true;
        }
        hdSPTypeID.Value = string.Empty;

        ucUserDet.ResetData();
        hdDataType.Value = string.Empty;
    }

    public void LoadList()
    {
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (oConfig != null)
        {
            SPReportMapping oSPReportMapping = new SPReportMapping();

            SPReportMappingDAL oSPReportMappingDAL = new SPReportMappingDAL();
            Result oResult = oSPReportMappingDAL.LoadUnapprovedList(oConfig.UserName, false);

            if (oResult.Status)
            {
                DataTable dtTmpReportMappingList = (DataTable)oResult.Return;
                if (dtTmpReportMappingList.Rows.Count > 0)
                {
                    dtTmpReportMappingList.Columns.Remove("MakerID");

                    DataTable dtMappedData = null;

                    dtMappedData = DDListUtil.MapTableWithXML(dtTmpReportMappingList, "ReportFormatMapping", "ReportType", "SS", 1);
                    dtMappedData = DDListUtil.MapTableWithXML(dtMappedData, "ReportFormatMapping", "ReportType", "CC", 2);
                    dtMappedData = DDListUtil.MapTableWithXML(dtMappedData, "ReportFormatMapping", "ReportType", "IC", 3);
                    dtMappedData = DDListUtil.MapTableWithXML(dtMappedData, "ReportFormatMapping", "ReportType", "EC", 4);

                    gvList.DataSource = dtMappedData;
                    gvList.DataBind();

                    Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtMappedData;

                    this.gvList.HeaderRow.Cells[1].Text = "SP Type";
                    this.gvList.HeaderRow.Cells[2].Text = "Sales Statemet";
                    this.gvList.HeaderRow.Cells[3].Text = "Commission Claim";
                    this.gvList.HeaderRow.Cells[4].Text = "Interest Claim";
                    this.gvList.HeaderRow.Cells[5].Text = "Encashment Claim";
                    this.gvList.HeaderRow.Cells[6].Text = "Make Date";
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
        SPReportMap oSPReportMap = new SPReportMap(sSPTypeID);
        SPReportMappingDAL oSPReportMappingDAL = new SPReportMappingDAL();
        Result oResult = new Result();
        oResult = oSPReportMappingDAL.LoadByID(oSPReportMap);
        if (oResult.Status)
        {
            oSPReportMap = (SPReportMap)oResult.Return;

            DDListUtil.Assign(ddlCommissionClaim, oSPReportMap.CommissionClaimFormat.ToString());
            DDListUtil.Assign(ddlEncashmentClaim, oSPReportMap.EncashmentClaimFormat.ToString());
            DDListUtil.Assign(ddlInterestClaim, oSPReportMap.InterestClaimFormat.ToString());
            DDListUtil.Assign(ddlSaleStatement, oSPReportMap.SalesStatemetFormat.ToString());
            DDListUtil.Assign(ddlSPType, oSPReportMap.SPTypeID.Trim());
            ddlSPType.Enabled = false;
            if (string.IsNullOrEmpty(hdDataType.Value))
            {
                //When Loading from Approver End
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.MakerID = oSPReportMap.UserDetails.MakerID;
                userDetails.MakeDate = oSPReportMap.UserDetails.MakeDate;
                ucUserDet.UserDetail = userDetails;
            }
            else if (hdDataType.Value.Equals("T"))
            {
                //When loading from temp table
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.CheckDate = oSPReportMap.UserDetails.CheckDate;
                userDetails.CheckerID = oSPReportMap.UserDetails.CheckerID;
                userDetails.CheckDate = oSPReportMap.UserDetails.CheckDate;
                userDetails.CheckerComment = oSPReportMap.UserDetails.CheckerComment;
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
