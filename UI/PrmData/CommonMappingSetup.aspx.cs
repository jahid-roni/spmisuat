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

public partial class CommonMappingSetup : System.Web.UI.Page
{
    #region Local Variable

    CryptographyManager oCrypManager = new CryptographyManager();
    public const string OBJ_CURRENCY_ID = "BaseCurrencyID";
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
                if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CONFIG.COMMON_MAPPING))
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
        
        gvList.DataSource=null;
        gvList.DataBind();

        string currencyID = Request.QueryString[OBJ_CURRENCY_ID];
        string sPageID = Request.QueryString[OBJ_PAGE_ID];
        if (!string.IsNullOrEmpty(currencyID))
        {
            currencyID = oCrypManager.GetDecryptedString(currencyID, Constants.CRYPT_PASSWORD_STRING);
        }
        if (!string.IsNullOrEmpty(sPageID))
        {
            sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
        }

        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (!string.IsNullOrEmpty(currencyID) && !string.IsNullOrEmpty(sPageID))
        {
            string sOperationType = sPageID.Substring(4, 1);
            if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
            {                
                LoadDataByID(currencyID);

                // general Control
                Util.ControlEnabled(ddlCurrencyID, false);
                Util.ControlEnabled(txtAccName, false);
                Util.ControlEnabled(txtAccount, false);

                // user Detail
                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);

                // button 
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true); 
                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);

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
            // general Control
            Util.ControlEnabled(ddlCurrencyID, true);
            Util.ControlEnabled(txtAccount, true);

            // button 
            Util.ControlEnabled(btnReject, false);
            Util.ControlEnabled(btnApprove, false);
            Util.ControlEnabled(btnBack, false);
            Util.ControlEnabled(btnReset, true);
            Util.ControlEnabled(btnSave, true);
            Util.ControlEnabled(btnDelete, true);
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
            Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.COMMON_MAPPING).PadLeft(5, '0'), false);    
        }
        else
        {
            // no action
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.COMMON_MAPPING).PadLeft(5, '0'), false);
    } 
    
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdCurrencyID.Value))
        {
            CommonMappingDAL oCMDAL = new CommonMappingDAL();
            Result oResult = (Result)oCMDAL.Detete(hdCurrencyID.Value);
            if (oResult.Status)
            {
                this.LoadList();
                this.ClearTextValue();
                hdCurrencyID.Value = "";
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
            ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
        }
    }

    protected void btnReject_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdCurrencyID.Value))
        {
            CommonMapping oCM = new CommonMapping(hdCurrencyID.Value);
            CommonMappingDAL oCMDAL = new CommonMappingDAL();
            oCM.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oCMDAL.Reject(oCM);
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
            ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
        }
    }

    protected void btnApprove_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdCurrencyID.Value))
        {
            CommonMapping oCM = new CommonMapping(hdCurrencyID.Value);
            CommonMappingDAL oCMDAL = new CommonMappingDAL();
            oCM.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oCMDAL.Approve(oCM);
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
        CommonMapping oCM = new CommonMapping();
        CommonMappingDAL oCMDAL = new CommonMappingDAL();
        if (!string.IsNullOrEmpty(ddlCurrencyID.SelectedItem.Value))
        {
            oCM.Currency.CurrencyID = ddlCurrencyID.SelectedItem.Value;
        }
        oCM.PnLAcc= SBM_BLC1.Common.String.RemoveSeperator(txtAccount.Text);
        oCM.PnLAccName = txtAccName.Text;

        oCM.UserDetails = ucUserDet.UserDetail;
        oCM.UserDetails.MakeDate = DateTime.Now;
        ucUserDet.ResetData();

        Result oResult = (Result)oCMDAL.Save(oCM);

        if (oResult.Status)
        {
            this.LoadList();
            this.ClearTextValue();
            hdCurrencyID.Value = "";

            ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
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
            e.Row.Cells[3].Text = SBM_BLC1.Common.String.AddSeperator(e.Row.Cells[3].Text);
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
    #endregion Basic Operational Function from control EVENT


    #region Supporting or Utility function

    public void ClearTextValue()
    {
        ddlCurrencyID.Enabled = true;
        ddlCurrencyID.SelectedIndex = 0;
        txtAccount.Text = "";
        ucUserDet.ResetData();
        hdDataType.Value = string.Empty;        
    }

    public void LoadList()
    {
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (oConfig != null)
        {
            CommonMapping oCM = new CommonMapping();
            CommonMappingDAL oCMDAL = new CommonMappingDAL();
            Result oResult = oCMDAL.LoadUnapprovedList(oConfig.UserName, false);

            if (oResult.Status)
            {
                DataTable dtTmpCurrencyList = (DataTable)oResult.Return;
                if (dtTmpCurrencyList.Rows.Count > 0)
                {
                    gvList.DataSource = dtTmpCurrencyList;
                    gvList.DataBind();
                    this.gvList.HeaderRow.Cells[1].Text = "Currency ID";

                    Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpCurrencyList;
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
        CommonMapping oCM = new CommonMapping(sCurrencyID);
        CommonMappingDAL oCMDAL = new CommonMappingDAL();
        
        Result oResult = new Result();
        oResult = oCMDAL.LoadByID(oCM);
        if (oResult.Status)
        {
            oCM = (CommonMapping)oResult.Return;
            txtAccount.Text = oCM.PnLAcc.Trim();
            txtAccName.Text = oCM.PnLAccName.Trim();
            DDListUtil.Assign(ddlCurrencyID, oCM.Currency.CurrencyID);
            ddlCurrencyID.Enabled = false;
            if (string.IsNullOrEmpty(hdDataType.Value))
            {
                //When Loading from Approver End
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.MakerID = oCM.UserDetails.MakerID;
                userDetails.MakeDate = oCM.UserDetails.MakeDate;
                ucUserDet.UserDetail = userDetails;
            }
            else if (hdDataType.Value.Equals("T"))
            {
                //When loading from temp table
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.CheckDate = oCM.UserDetails.CheckDate;
                userDetails.CheckerID = oCM.UserDetails.CheckerID;
                userDetails.CheckDate = oCM.UserDetails.CheckDate;
                userDetails.CheckerComment = oCM.UserDetails.CheckerComment;
                ucUserDet.UserDetail = userDetails;
            }            
            
            hdCurrencyID.Value = sCurrencyID;
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
        }
    }

    #endregion Supporting or Utility function

}
