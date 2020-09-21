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
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Common;
using SBM_BLC1.Configuration;
using SBM_BLC1.Entity.Configuration;

public partial class SPCertificateMapping : System.Web.UI.Page
{
    #region Local Variable

    CryptographyManager oCrypManager = new CryptographyManager();
    public const string OBJ_LIMIT_GROPU_ID = "LimitGroupID";
    public const string OBJ_SPTYPE_ID = "SPTypeID";
    public const string OBJ_DENOM_ID = "sDenomID";
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
                if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CONFIG.SP_CERTIFICATE))
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
        DDListUtil.LoadDDLFromXML(ddlScriptFormat, "ScriptFormatMapping", "ReportType", "SP", true);
        gvList.PageSize = (int)Constants.PAGING_UNAPPROVED;
        
        SPTypeDAL oSPTypeDAL = new SPTypeDAL();
        SetDenominationBySPTypeID(oSPTypeDAL);

        string sPTypeID = Request.QueryString[OBJ_SPTYPE_ID];
        string sDenomID = Request.QueryString[OBJ_DENOM_ID]; 
        string sPageID = Request.QueryString[OBJ_PAGE_ID];
        if (!string.IsNullOrEmpty(sPTypeID))
        {
            sPTypeID = oCrypManager.GetDecryptedString(sPTypeID, Constants.CRYPT_PASSWORD_STRING);
        }
        if (!string.IsNullOrEmpty(sDenomID))
        {
            sDenomID = oCrypManager.GetDecryptedString(sDenomID, Constants.CRYPT_PASSWORD_STRING);
        }
        if (!string.IsNullOrEmpty(sPageID))
        {
            sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
        }
        
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (!string.IsNullOrEmpty(sPTypeID) && !string.IsNullOrEmpty(sDenomID) && !string.IsNullOrEmpty(sPageID))
        {
            string sOperationType = sPageID.Substring(4, 1);
            if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
            {                
                LoadDataByID(sPTypeID, sDenomID);

                // general Control
                Util.ControlEnabled(ddlSPType, false);
                Util.ControlEnabled(ddlDenomination, false);
                Util.ControlEnabled(ddlScriptFormat, false);

                // user Detail
                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);

                // button 
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true);

                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);
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
            Util.ControlEnabled(btnSearch, true);
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
            LoadDataByID(gvRow.Cells[1].Text, gvRow.Cells[2].Text);
        }
    }

    public void PopErrorMsgAction(string sType)
    {
        if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
        {
            Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_CERTIFICATE).PadLeft(5, '0'), false);
        }
        else
        {
            // no action
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_CERTIFICATE).PadLeft(5, '0'), false);
    }

    protected void btnLoad_Click(object sender, EventArgs e)
    {
        hdDataType.Value = "L";
        LoadDataByID(ddlSPType.SelectedItem.Value,ddlDenomination.SelectedValue);
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdSPTypeID.Value))
        {
            ScripMappingDAL oScripMappingDAL = new ScripMappingDAL();
            Result oResult = (Result)oScripMappingDAL.Detete(hdSPTypeID.Value, Convert.ToInt32(hdDenomination.Value));
            if (oResult.Status)
            {
                ClearTextValue();
                this.LoadList();
                hdDenomination.Value = string.Empty;
                hdSPTypeID.Value = string.Empty;

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
        if (ddlScriptFormat.Items.Count > 0)
        {
            ddlScriptFormat.SelectedIndex = 0;
        }
        ddlSPType.Enabled = true;
        if (ddlSPType.Items.Count > 0)
        {
            ddlSPType.SelectedIndex = 0;        
        }
        ddlDenomination.Enabled = true;
        if (ddlDenomination.Items.Count > 0)
        {
            ddlDenomination.Items.Clear();
        }
        ucUserDet.ResetData();
        hdDataType.Value = string.Empty;
    }

    protected void btnReject_Click(object sender, EventArgs e)
    {
        ScripMapping oScripMapping = new ScripMapping(hdSPTypeID.Value, Convert.ToInt32(hdDenomination.Value == "" ? "0" : hdDenomination.Value));
        ScripMappingDAL oScripMappingDAL = new ScripMappingDAL();
        oScripMapping.UserDetails = ucUserDet.UserDetail;

        Result oResult = (Result)oScripMappingDAL.Reject(oScripMapping);
        if (oResult.Status)
        {
            ucMessage.OpenMessage(Constants.MSG_SUCCESS_REJECT, Constants.MSG_TYPE_SUCCESS);
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_REJECT, Constants.MSG_TYPE_ERROR);
        }
    }
    protected void btnApprove_Click(object sender, EventArgs e)
    {
        ScripMapping oScripMapping = new ScripMapping(hdSPTypeID.Value, Convert.ToInt32(hdDenomination.Value == "" ? "0" : hdDenomination.Value));
        ScripMappingDAL oScripMappingDAL = new ScripMappingDAL();
        
        oScripMapping.UserDetails = ucUserDet.UserDetail;

        Result oResult = (Result)oScripMappingDAL.Approve(oScripMapping);
        if (oResult.Status)
        {
            ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE, Constants.MSG_TYPE_SUCCESS);
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_APPROVE, Constants.MSG_TYPE_ERROR);
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        ScripMappingDAL oScripMappingDAL = new ScripMappingDAL();
        ScripMapping oScripMapping = new ScripMapping();
        if (!string.IsNullOrEmpty(ddlSPType.SelectedItem.Value))
        {
            oScripMapping.SPTypeID = ddlSPType.SelectedItem.Value;
        }
        if (!string.IsNullOrEmpty(ddlDenomination.SelectedItem.Value))
        {
            oScripMapping.Denomination = Convert.ToInt32(ddlDenomination.SelectedItem.Value);
        }
        if (!string.IsNullOrEmpty(ddlScriptFormat.SelectedValue))
        {
            oScripMapping.ScripFormat = Convert.ToInt16(ddlScriptFormat.SelectedValue);
        }

        oScripMapping.UserDetails = ucUserDet.UserDetail;
        oScripMapping.UserDetails.MakeDate = DateTime.Now;
        ucUserDet.ResetData();
        Result oResult = (Result)oScripMappingDAL.Save(oScripMapping);

        if (oResult.Status)
        {
            ClearTextValue();
            this.LoadList();
            hdDenomination.Value = string.Empty;
            hdSPTypeID.Value = string.Empty;

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
            LoadDataByID(gvRow.Cells[1].Text.Trim(), gvRow.Cells[2].Text);
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
        ScripMappingDAL oScripMappingDAL = new ScripMappingDAL();

        Result oResult = oScripMappingDAL.LoadUnapprovedList(oConfig.UserName, false);

        if (oResult.Status)
        {
            DataTable dtTmpDataList = (DataTable)oResult.Return;
            if (dtTmpDataList.Rows.Count > 0)
            {
                dtTmpDataList.Columns.Remove("MakerID");
                
                dtTmpDataList = DDListUtil.MapTableWithXML(dtTmpDataList, "ScriptFormatMapping", "ReportType", "SP", 2);

                gvList.DataSource = dtTmpDataList;
                gvList.DataBind();

                Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpDataList;

                this.gvList.HeaderRow.Cells[1].Text = "SPType";
                this.gvList.HeaderRow.Cells[2].Text = "Denomination";
                this.gvList.HeaderRow.Cells[3].Text = "Scrip Format";
                this.gvList.HeaderRow.Cells[4].Text = "Make Date";                
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

    private void LoadDataByID(string sSPTypeID,string sDenomination)
    {
        ScripMapping oScripMapping = new ScripMapping(sSPTypeID, Convert.ToInt32(sDenomination == "" ? "0" : sDenomination));
        ScripMappingDAL oScripMappingDAL = new ScripMappingDAL();

        Result oResult = new Result();
        oResult = oScripMappingDAL.LoadByID(oScripMapping);
        if (oResult.Status)
        {
            oScripMapping = (ScripMapping)oResult.Return;
            DDListUtil.Assign(ddlSPType, oScripMapping.SPTypeID.Trim());
            //ddlSPType.Text = oScripMapping.SPTypeID.Trim();
            ddlSPType.Enabled = false;
            SetDenominationBySPTypeID(null);
            ddlDenomination.Enabled = false;
            
            DDListUtil.Assign(ddlDenomination, oScripMapping.Denomination.ToString());
            DDListUtil.Assign(ddlScriptFormat, oScripMapping.ScripFormat.ToString());

            hdSPTypeID.Value = oScripMapping.SPTypeID.Trim();
            hdDenomination.Value = oScripMapping.Denomination.ToString();
            if (string.IsNullOrEmpty(hdDataType.Value))
            {
                //When Loading from Approver End
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.MakerID = oScripMapping.UserDetails.MakerID;
                userDetails.MakeDate = oScripMapping.UserDetails.MakeDate;
                ucUserDet.UserDetail = userDetails;
            }
            else if (hdDataType.Value.Equals("T"))
            {
                //When loading from temp table
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.CheckDate = oScripMapping.UserDetails.CheckDate;
                userDetails.CheckerID = oScripMapping.UserDetails.CheckerID;
                userDetails.CheckDate = oScripMapping.UserDetails.CheckDate;
                userDetails.CheckerComment = oScripMapping.UserDetails.CheckerComment;
                ucUserDet.UserDetail = userDetails;
            }             
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
        }
    }

    #endregion Supporting or Utility function

    protected void ddlSPType_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetDenominationBySPTypeID(null);
    }

    protected void SetDenominationBySPTypeID(SPTypeDAL oSPTypeDAL)
    {
        if (oSPTypeDAL == null)
        {
            oSPTypeDAL = new SPTypeDAL();
        }
        Result oResult = (Result)oSPTypeDAL.GetDDLDenomList(ddlSPType.SelectedValue);
        if (oResult.Status)
        {
            ddlDenomination.Items.Clear();
            DataTable dtGetDenomID = (DataTable)oResult.Return;
            DDListUtil.Assign(ddlDenomination, dtGetDenomID);
        }
    }    
}