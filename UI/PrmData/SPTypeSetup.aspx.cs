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

public partial class SPTypeSetup : System.Web.UI.Page
{
    #region Local Variable

    CryptographyManager oCrypManager = new CryptographyManager();
    public const string OBJ_SPTYPE_ID = "SPTypeID";
    public const string OBJ_DENOM_ID = "sDenomID";
    public const string OBJ_PAGE_ID = "sPageID";
    public SPTypeList spTypeList = null;

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
                if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CONFIG.SP_TYPE))
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
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        gvList.PageSize = (int)Constants.PAGING_UNAPPROVED;

        if (Session[Constants.SES_SPTYPELIST] == null)
        {
            Session.Add(Constants.SES_SPTYPELIST, new SPTypeList());
        }
        else
        {
            Session[Constants.SES_SPTYPELIST] = new SPTypeList();
        }
        
        if (Session[Constants.SES_SPTYPE] == null)
        {
            Session.Add(Constants.SES_SPTYPE, new SPType());
        }
        else
        {
            Session[Constants.SES_SPTYPE] = new SPType();
        }
        

        SPTypeDAL spTypeDal = new SPTypeDAL();
        spTypeList = new SPTypeList();
        Result oRslSPTypeList = spTypeDal.LoadList(oConfig.UserName);
        spTypeList = (SPTypeList)oRslSPTypeList.Return;
        Session[Constants.SES_SPTYPELIST] = spTypeList;

        gvDenom.DataSource = null;
        gvDenom.DataBind();
        hdDataType.Value = "";

        // Dropdown load
        DDListUtil.LoadDDLFromDB(ddlCurrency, "CurrencyID", "CurrencyCode", "SPMS_Currency", true); 
        
        string sPTypeID = Request.QueryString[OBJ_SPTYPE_ID];
        string sPageID = Request.QueryString[OBJ_PAGE_ID];
        

        if (!string.IsNullOrEmpty(sPTypeID))
        {
            sPTypeID = oCrypManager.GetDecryptedString(sPTypeID, Constants.CRYPT_PASSWORD_STRING).Trim();
        }
        if (!string.IsNullOrEmpty(sPageID))
        {
            sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING).Trim();
        }

        if (!string.IsNullOrEmpty(sPTypeID) && !string.IsNullOrEmpty(sPageID))
        {
            string sOperationType = sPageID.Substring(4, 1);
            if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
            {
                if (spTypeList.ListOfSPType.Count > 0)
                {
                    LoadDenomDataListBySPTypeID(sPTypeID.Trim(), spTypeList);
                    LoadSPDetailData(sPTypeID.Trim(), spTypeList);
                }

                // general Control
                Util.ControlEnabled(txtSpTypeId, false);
                Util.ControlEnabled(txtDescription, false);
                Util.ControlEnabled(ddlCurrency, false);
                Util.ControlEnabled(txtDenomination, false);
                Util.ControlEnabled(txtReOrderLevel, false);
                Util.ControlEnabled(txtSeries, false);
                Util.ControlEnabled(txtNoOfDigitInSeries, false);

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

                Util.ControlEnabled(btnSaveDenom, false);
                Util.ControlEnabled(btnDenomReset, false);

                #region User-Detail.
                UserDetails oUserDetails = ucUserDet.UserDetail;
                oUserDetails.CheckerID = oConfig.UserName;
                oUserDetails.CheckDate = DateTime.Now;
                ucUserDet.UserDetail = oUserDetails;
                #endregion User-Detail.

                fsList.Visible = false;
                fsDenom.Visible = true;
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

            Util.ControlEnabled(btnSaveDenom, true);
            Util.ControlEnabled(btnDenomReset, true);
            Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), false);
            #region User-Detail.
            UserDetails oUserDetails = new UserDetails();
            oUserDetails.MakerID = oConfig.UserName;
            oUserDetails.MakeDate = DateTime.Now;
            ucUserDet.UserDetail = oUserDetails;
            #endregion User-Detail.

            fsList.Visible = true;
            fsDenom.Visible = true;
            LoadSPDataList(spTypeList);
        }
    }

    #endregion InitializeData


    #region Basic Operational Function from control EVENT

    public void PopErrorMsgAction(string sType)
    {
        if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
        {
            Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_TYPE).PadLeft(5, '0'), false);
        }
        else
        {
            // no action
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_TYPE).PadLeft(5, '0'), false);
    }

    public void SPLoadAction(string sSPTypeID)
    {
        if (!string.IsNullOrEmpty(sSPTypeID))
        {
            SPTypeDAL oSpTypeDal = new SPTypeDAL();
            Result oResult = oSpTypeDal.LoadByID(sSPTypeID);
            if (oResult.Status)
            {
                SPType oSpType = (SPType)oResult.Return;
                if (oSpType != null)
                {
                    hdDataType.Value = oResult.Message;
                    SetSPType(oSpType);
                    Session[Constants.SES_SPTYPE] = oSpType;
                }
            }
        }
    }

    private void SetSPType(SPType oSpType)
    {
        txtSpTypeId.Text = oSpType.SPTypeID.ToString().Trim();
        txtDescription.Text = oSpType.TypeDesc.ToString().Trim();
        ddlCurrency.Text = oSpType.Currency.CurrencyID.Trim();
        hdSPTypeID.Value = oSpType.SPTypeID.ToString().Trim();
        if (string.IsNullOrEmpty(hdDataType.Value))
        {
            //When Loading from Approver End
            UserDetails userDetails = ucUserDet.UserDetail;
            userDetails.MakerID = oSpType.UserDetails.MakerID;
            userDetails.MakeDate = oSpType.UserDetails.MakeDate;
            ucUserDet.UserDetail = userDetails;
        }
        else if (hdDataType.Value.Equals("T"))
        {
            //When loading from temp table
            UserDetails userDetails = ucUserDet.UserDetail;
            userDetails.CheckDate = oSpType.UserDetails.CheckDate;
            userDetails.CheckerID = oSpType.UserDetails.CheckerID;
            userDetails.CheckDate = oSpType.UserDetails.CheckDate;
            userDetails.CheckerComment = oSpType.UserDetails.CheckerComment;
            ucUserDet.UserDetail = userDetails;
        }         

        DataTable oDataTable = new DataTable("dtData");

        oDataTable.Columns.Add(new DataColumn("bfDenomSPType", typeof(string)));
        oDataTable.Columns.Add(new DataColumn("bfDenomination", typeof(string)));
        oDataTable.Columns.Add(new DataColumn("bfSPSeries", typeof(string)));
        oDataTable.Columns.Add(new DataColumn("bfDigitsInSlNo", typeof(string)));
        oDataTable.Columns.Add(new DataColumn("bfReOrderLevel", typeof(string)));

        DataRow row = null;
        gvDenom.DataSource = null;
        gvDenom.DataBind();
        if (oSpType.ListOfDenomination.Denomination.Count > 0)
        {
            for (int j = 0; j < oSpType.ListOfDenomination.Denomination.Count; j++)
            {
                Denomination oDenomination = (Denomination)oSpType.ListOfDenomination.Denomination[j];

                row = oDataTable.NewRow();
                row["bfDenomSPType"] = oSpType.SPTypeID.ToString();
                row["bfDenomination"] = oDenomination.DenominationID.ToString();
                row["bfSPSeries"] = oDenomination.Series.ToString();
                row["bfDigitsInSlNo"] = oDenomination.NoOfDigitsInSeries.ToString();
                row["bfReOrderLevel"] = oDenomination.ReOrderLevel.ToString();
                oDataTable.Rows.Add(row);
            }
            gvDenom.DataSource = oDataTable;
            gvDenom.DataBind();
        }
    }


    protected void btnLoad_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request[txtSpTypeId.UniqueID].Trim().ToUpper()))
        {
            spTypeList = (SPTypeList)Session[Constants.SES_SPTYPELIST];

            string sTmp = Request[txtSpTypeId.UniqueID].Trim().ToUpper();
            ClearTextValue();
            SPType oSpType = spTypeList.ListOfSPType.Where(d => d.SPTypeID.Equals(sTmp.Trim())).SingleOrDefault();
            if (oSpType != null)
            {
                LoadDenomDataListBySPTypeID(sTmp, spTypeList);
                LoadSPDetailData(sTmp, spTypeList);
                txtSpTypeId.Text = sTmp;
                Session[Constants.SES_SPTYPE] = oSpType;
            }
            else
            {
                SPLoadAction(sTmp);
            }
        }
    }

    private void ClearTextValue()
    {
        //Clear Denomination Session
        Session[Constants.SES_SPTYPE] = null;
        //Clear Grid
        gvDenom.DataSource = null;
        gvDenom.DataBind();
        //Clear control fields
        txtSpTypeId.Text = string.Empty;
        txtSpTypeId.ReadOnly = false;
        txtDescription.Text = string.Empty;
        ddlCurrency.SelectedIndex = 0;
        ucUserDet.ResetData();
        hdDataType.Value = string.Empty;
    }
    public void ResetDenom()
    {        
        //Clear control fields
        txtDenomination.Text = "";
        txtReOrderLevel.Text = "";
        txtSeries.Text = "";
        txtNoOfDigitInSeries.Text = "";
    }

    private void LoadSPDetailData(string sSpTypeId, SPTypeList spList)
    {
        if (spList != null)
        {
            if (spList.ListOfSPType.Count > 0)
            {
                SPType oSpType = null;
                for (int i = 0; i < spList.ListOfSPType.Count; i++)
                {
                    oSpType = (SPType)spList.ListOfSPType[i];
                    if (sSpTypeId.Equals(oSpType.SPTypeID.Trim()))
                    {
                        txtSpTypeId.Text = oSpType.SPTypeID.ToString().Trim();
                        txtSpTypeId.ReadOnly = true;
                        txtDescription.Text = oSpType.TypeDesc.ToString().Trim();
                        ddlCurrency.Text = oSpType.Currency.CurrencyID.Trim();
                        hdSPTypeID.Value = oSpType.SPTypeID.ToString().Trim();

                        ucUserDet.UserDetail = oSpType.UserDetails;
                    }
                }
            }
        }
    }

    protected void gvDenom_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        //just for signature to Handle IE exception 
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (hdDataType.Value != "M")
        {
            SPTypeDAL oSPTypeDAL = new SPTypeDAL();
            if (hdSPTypeID.Value != "")
            {
                Result oResult = (Result)oSPTypeDAL.Detete(hdSPTypeID.Value);
                if (oResult.Status)
                {
                    ReloadList(oSPTypeDAL, string.Empty);
                    ClearTextValue();
                    ResetDenom();
                    hdDenomID.Value = string.Empty;
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
        else
        {
            ucMessage.OpenMessage(Constants.MSG_APPROVED_DELETE_DATA, Constants.MSG_TYPE_INFO);
            ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            ClearTextValue();
        }
    }

    protected void btnReject_Click(object sender, EventArgs e)
    {
        SPTypeDAL oSPTypeDAL = new SPTypeDAL();
        SPType oSPType = new SPType(hdSPTypeID.Value);
        oSPType.UserDetails = ucUserDet.UserDetail;

        Result oResult = (Result)oSPTypeDAL.Reject(oSPType);
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
        SPTypeDAL oSPTypeDAL = new SPTypeDAL();
        SPType oSPType = new SPType(hdSPTypeID.Value);
        oSPType.UserDetails = ucUserDet.UserDetail;

        Result oResult = (Result)oSPTypeDAL.Approve(oSPType);
        if (oResult.Status)
        {
            ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE, Constants.MSG_TYPE_SUCCESS);
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_APPROVE, Constants.MSG_TYPE_ERROR);
        }
    }

    protected void btnSaveDenom_Click(object sender, EventArgs e)
    {
        SPType oSPType = (SPType)Session[Constants.SES_SPTYPE];

        Result oResult = new Result();
        bool isExist = false;
        // need to check first for adding can be possible or not...
        SPTypeDAL oSPTypeDAL = new SPTypeDAL();

        if (oSPType.ListOfDenomination.Denomination.Count > 0)
        {
            Denomination oDenomSerRemove = oSPType.ListOfDenomination.Denomination.Where(d => d.Series.Equals(txtSeries.Text)).SingleOrDefault();
            Denomination oDenomRemove = oSPType.ListOfDenomination.Denomination.Where(d => d.DenominationID.Equals(Convert.ToInt32(txtDenomination.Text))).SingleOrDefault();
            if (oDenomSerRemove != null && oDenomRemove==null)
            {
                isExist = true;
            }
        }
        if (!isExist)
        {
            oResult = oSPTypeDAL.IsExistSeriesName(txtSeries.Text);
            if (oResult.Status)
            {
                isExist = (bool)oResult.Return;
            }
        }

        if (!isExist)
        {

            
            if (oSPType != null)
            {
                if (oSPType.ListOfDenomination.Denomination.Count > 0)
                {
                    Denomination oDenomRemove = oSPType.ListOfDenomination.Denomination.Where(d => d.DenominationID.Equals(Convert.ToInt32(txtDenomination.Text))).SingleOrDefault();
                    if (oDenomRemove != null)
                    {
                        oSPType.ListOfDenomination.Denomination.Remove(oDenomRemove);
                    }
                }
            }
            else
            {
                oSPType = new SPType();
            }

            Denomination oDenomination = new Denomination();

            oDenomination.DenominationID = Util.GetIntNumber(txtDenomination.Text);
            oDenomination.NoOfDigitsInSeries = Util.GetIntNumber(txtNoOfDigitInSeries.Text);
            oDenomination.ReOrderLevel = Util.GetIntNumber(txtReOrderLevel.Text);
            oDenomination.Series = txtSeries.Text.Trim().ToUpper();
            oDenomination.SPType.SPTypeID = hdSPTypeID.Value.Trim();

            oDenomination.UserDetail = ucUserDet.UserDetail;

            oSPType.ListOfDenomination.Denomination.Add(oDenomination);
            Session[Constants.SES_SPTYPE] = oSPType;

            ReloadDenomTmpListFromSession();
            ResetDenom();
        }
        else
        {
            ucMessage.OpenMessage("This Series "+ txtSeries.Text+ " all ready exist for other Denotation !", Constants.MSG_TYPE_ERROR);
            ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
        }
    }
    
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdDataType.Value))
        {
            SPType oSPType = (SPType)Session[Constants.SES_SPTYPE];
            if (oSPType == null)
            {
                oSPType = new SPType();
            }
            SPTypeDAL oSPTypeDAL = new SPTypeDAL();

            oSPType.SPTypeID = Request[txtSpTypeId.UniqueID].Trim().ToUpper();
            oSPType.TypeDesc = txtDescription.Text.Trim().ToUpper();

            if (!string.IsNullOrEmpty(ddlCurrency.SelectedValue))
            {
                oSPType.Currency.CurrencyID = ddlCurrency.SelectedValue;
            }

            oSPType.UserDetails = ucUserDet.UserDetail;
            oSPType.UserDetails.MakeDate = DateTime.Now;
            ucUserDet.ResetData();

            Result oResult = (Result)oSPTypeDAL.Save(oSPType);

            if (oResult.Status)
            {
                ReloadList(oSPTypeDAL, oSPType.SPTypeID);
                ClearTextValue();
                ResetDenom();
                hdDenomID.Value = string.Empty;
                hdSPTypeID.Value = string.Empty;

                ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
            }
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_APPROVED_SAVE_DATA, Constants.MSG_TYPE_INFO);
            ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            ClearTextValue();
            ResetDenom();
            hdDenomID.Value = string.Empty;
            hdSPTypeID.Value = string.Empty;
        }
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        txtDenomination.Enabled = true;
        txtSeries.Enabled = true;
        ClearTextValue();
    }

    private void ReloadList(SPTypeDAL oSPTypeDAL,string sSPTpyeID)
    {
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

        spTypeList = new SPTypeList();
        Result oRslSPTypeList = oSPTypeDAL.LoadList(oConfig.UserName);
        spTypeList = (SPTypeList)oRslSPTypeList.Return;
        Session[Constants.SES_SPTYPELIST] = spTypeList;
        LoadSPDataList(spTypeList);
        LoadDenomDataListBySPTypeID(sSPTpyeID, spTypeList);
    }

    private void ReloadDenomTmpListFromSession()
    {
        SPType oSPType = (SPType)Session[Constants.SES_SPTYPE];

        DataTable oDataTable = new DataTable("dtData");

        oDataTable.Columns.Add(new DataColumn("bfDenomSPType", typeof(string)));
        oDataTable.Columns.Add(new DataColumn("bfDenomination", typeof(string)));
        oDataTable.Columns.Add(new DataColumn("bfSPSeries", typeof(string)));
        oDataTable.Columns.Add(new DataColumn("bfDigitsInSlNo", typeof(string)));
        oDataTable.Columns.Add(new DataColumn("bfReOrderLevel", typeof(string)));


        string sSPTypeTmp = oSPType.SPTypeID;
        if (string.IsNullOrEmpty(sSPTypeTmp))
        {
            sSPTypeTmp = txtSpTypeId.Text;
        }
        if (string.IsNullOrEmpty(sSPTypeTmp))
        {
            sSPTypeTmp = "Not Define Yet";
        }

        DataRow row = null;
        if (oSPType.ListOfDenomination.Denomination.Count > 0)
        {
            for (int j = 0; j < oSPType.ListOfDenomination.Denomination.Count; j++)
            {
                Denomination oDenomination = (Denomination)oSPType.ListOfDenomination.Denomination[j];

                row = oDataTable.NewRow();
                //row["bfDenomSPType"] = oSPType.SPTypeID == "" ? "Not Define Yet" : oSPType.SPTypeID.ToString();
                row["bfDenomSPType"] = sSPTypeTmp;
                row["bfDenomination"] = oDenomination.DenominationID.ToString();
                row["bfSPSeries"] = oDenomination.Series.ToString();
                row["bfDigitsInSlNo"] = oDenomination.NoOfDigitsInSeries.ToString();
                row["bfReOrderLevel"] = oDenomination.ReOrderLevel.ToString();
                oDataTable.Rows.Add(row);
            }
        }
        gvDenom.DataSource = oDataTable;
        gvDenom.DataBind();
    }

    private void ReloadListFromSession( string sSPTpyeID)
    {
        SPTypeList spTypeList = (SPTypeList)Session[Constants.SES_SPTYPELIST];
        LoadSPDataList(spTypeList);
        LoadDenomDataListBySPTypeID(sSPTpyeID, spTypeList);
    }
    
    protected void gvList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvList.PageIndex = e.NewPageIndex;
        if (Session[Constants.SES_SPTYPELIST] != null)
        {
            spTypeList = (SPTypeList)Session[Constants.SES_SPTYPELIST];
            LoadSPDataList(spTypeList);
        }
    }
    
    protected void gvList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (!e.CommandName.Equals("Page"))
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            spTypeList = (SPTypeList)Session[Constants.SES_SPTYPELIST];
            SPType oSpType = new SPType();
            if (spTypeList != null)
            {
                oSpType = spTypeList.ListOfSPType.Where(d => d.SPTypeID.Equals(gvRow.Cells[1].Text.Trim())).SingleOrDefault();
                Session[Constants.SES_SPTYPE] = oSpType;

                ReloadDenomTmpListFromSession();
                hdDataType.Value = "T";
                LoadSPDetailData(gvRow.Cells[1].Text.Trim(), spTypeList);
                ResetDenom();
            }
        }
    }

    private void LoadDenomDataListBySPTypeID(string spTYpeID, SPTypeList spList)
    {
        gvDenom.DataSource = null;
        gvDenom.DataBind();

        if ( !string.IsNullOrEmpty(spTYpeID) && spList != null)
        {
            if (spList.ListOfSPType.Count > 0)
            {
                DataTable oDataTable = new DataTable("dtData");

                oDataTable.Columns.Add(new DataColumn("bfDenomSPType", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfDenomination", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfSPSeries", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfDigitsInSlNo", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfReOrderLevel", typeof(string)));

                DataRow row = null;
                SPType oSpType = spTypeList.ListOfSPType.Where(d => d.SPTypeID.Equals(spTYpeID)).SingleOrDefault();
                if (oSpType != null)
                {
                    if (oSpType.ListOfDenomination.Denomination.Count > 0)
                    {
                        for (int j = 0; j < oSpType.ListOfDenomination.Denomination.Count; j++)
                        {
                            Denomination oDenomination = (Denomination)oSpType.ListOfDenomination.Denomination[j];

                            row = oDataTable.NewRow();
                            row["bfDenomSPType"] = oSpType.SPTypeID.ToString();
                            row["bfDenomination"] = oDenomination.DenominationID.ToString();
                            row["bfSPSeries"] = oDenomination.Series.ToString();
                            row["bfDigitsInSlNo"] = oDenomination.NoOfDigitsInSeries.ToString();
                            row["bfReOrderLevel"] = oDenomination.ReOrderLevel.ToString();
                            oDataTable.Rows.Add(row);
                        }
                    }
                }
                // 
                else
                {

                }
                gvDenom.DataSource = oDataTable;
                gvDenom.DataBind();
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

    protected void gvDenom_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
        SPType oSPType = (SPType)Session[Constants.SES_SPTYPE];
        if (((Button)e.CommandSource).Text.Equals("Select"))
        {
            txtDenomination.Enabled = false;
            txtSeries.Enabled = false;
            LoadDenomDetailData(gvRow.Cells[2].Text.Trim());
        }
        else if (((Button)e.CommandSource).Text.Equals("Delete"))
        {
            if (oSPType.ListOfDenomination.Denomination.Count > 0)
            {
                hdSPTypeID.Value = gvRow.Cells[2].Text.Trim();
                hdDenomID.Value = gvRow.Cells[3].Text.Trim();
                Denomination oDenomination = oSPType.ListOfDenomination.Denomination.Where(d => d.DenominationID.Equals(Convert.ToInt32(hdDenomID.Value))).SingleOrDefault();
                if (oDenomination != null)
                {
                    oSPType.ListOfDenomination.Denomination.Remove(oDenomination);
                }
                Session[Constants.SES_SPTYPE] = oSPType;
                ReloadDenomTmpListFromSession();
                ResetDenom();
            }
        }
    }

    private void LoadDenomDetailData(string sDenomID)
    {
        SPType oSPType = (SPType)Session[Constants.SES_SPTYPE];
        if (oSPType != null)
        {
            Denomination oDenomination = oSPType.ListOfDenomination.Denomination.Where(d => d.DenominationID.Equals(Convert.ToInt32(sDenomID))).SingleOrDefault();
            if (oDenomination != null)
            {
                if (oDenomination.DenominationID == Convert.ToInt32(sDenomID))
                {
                    txtDenomination.Text = oDenomination.DenominationID.ToString();
                    txtReOrderLevel.Text = oDenomination.ReOrderLevel.ToString().Trim();
                    txtSeries.Text = oDenomination.Series.ToString().Trim();
                    txtNoOfDigitInSeries.Text = oDenomination.NoOfDigitsInSeries.ToString().Trim();

                    hdDenomID.Value = oDenomination.DenominationID.ToString();
                }
            }
        }
    }

    private void LoadDenomDetailData(string sTypeID, string sDenomID, SPTypeList spList)
    {
        if (spList != null)
        {
            if (spList.ListOfSPType.Count > 0)
            {
                SPType oSpType = null;
                for (int i = 0; i < spList.ListOfSPType.Count; i++)
                {
                    oSpType = (SPType)spList.ListOfSPType[i];
                    if (sTypeID.Equals(oSpType.SPTypeID.Trim()))
                    {
                        for (int j = 0; j < oSpType.ListOfDenomination.Denomination.Count; j++)
                        {
                            Denomination oDenomination = oSpType.ListOfDenomination.Denomination[j];
                            if (oDenomination.DenominationID ==  Convert.ToInt32(sDenomID))
                            {
                                txtDenomination.Text = oDenomination.DenominationID.ToString();
                                txtReOrderLevel.Text = oDenomination.ReOrderLevel.ToString().Trim();
                                txtSeries.Text = oDenomination.Series.ToString().Trim();
                                txtNoOfDigitInSeries.Text = oDenomination.NoOfDigitsInSeries.ToString().Trim();

                                //ucUserDet.UserDetail = oDenomination.UserDetail;

                                hdDenomID.Value = oDenomination.DenominationID.ToString();
                            }
                        }
                    }
                }
            }
        }
    }

    #endregion Basic Operational Function from control EVENT


    #region Supporting or Utility function
    private void LoadSPDataList(SPTypeList spList)
    {
        if (spList != null)
        {
            if (spList.ListOfSPType.Count > 0)
            {
                DataTable oDataTable = new DataTable("dtData");

                oDataTable.Columns.Add(new DataColumn("bfSpType", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfTypeDesc", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfCurrencyCode", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("MakeDate", typeof(DateTime)));

                DataRow row = null;
                SPType oSpType = null;
                for (int i = 0; i < spList.ListOfSPType.Count; i++)
                {
                    oSpType = (SPType)spList.ListOfSPType[i];
                    row = oDataTable.NewRow();
                    row["bfSpType"] = oSpType.SPTypeID;
                    row["bfTypeDesc"] = oSpType.TypeDesc;
                    row["bfCurrencyCode"] = oSpType.Currency.CurrencyCode;
                    row["MakeDate"] = oSpType.UserDetails.MakeDate.ToString("dd-MMM-yyyy");
                    oDataTable.Rows.Add(row);
                }
                gvList.DataSource = oDataTable;
                gvList.DataBind();
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

    protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Util.GridDateFormat(e, gvList, null);
    }

    #endregion Supporting or Utility function   
}
