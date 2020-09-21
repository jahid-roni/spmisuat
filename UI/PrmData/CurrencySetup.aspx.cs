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

public partial class CurrencySetup : System.Web.UI.Page
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
                if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CONFIG.CURRENCY))
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
        gvList.PageSize = (int)Constants.PAGING_UNAPPROVED;

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
                Util.ControlEnabled(txtCurrencyID, false);
                Util.ControlEnabled(txtCurrencyCode, false);
                Util.ControlEnabled(txtCurrencySymbol, false);
                Util.ControlEnabled(txtDescription, false);

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

    public void PopLoadAction(string sID)
    {
        hdDataType.Value = "M";
        LoadDataByID(sID);
    }

    public void PopErrorMsgAction(string sType)
    {
        if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
        {
            Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CURRENCY).PadLeft(5, '0'), false);
        }
        else
        {
            // no action
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CURRENCY).PadLeft(5, '0'), false);
    } 

    protected void btnLoad_Click(object sender, EventArgs e)
    {
        string sCur = Request[txtCurrencyID.UniqueID].Trim().ToUpper();
        ClearTextValue();
        hdDataType.Value = "L";
        LoadDataByID(sCur);        
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdCurrencyID.Value))
        {
            CurrencyDAL oCurrencyDAL = new CurrencyDAL();
            Result oResult = (Result)oCurrencyDAL.Detete(hdCurrencyID.Value);
            if (oResult.Status)
            {
                this.LoadList();
                this.ClearTextValue();
                hdCurrencyID.Value = string.Empty;

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
        if (!string.IsNullOrEmpty(hdCurrencyID.Value))
        {
            Currency oCurrency = new Currency(hdCurrencyID.Value);
            CurrencyDAL oCurrencyDAL = new CurrencyDAL();
            oCurrency.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oCurrencyDAL.Reject(oCurrency);
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
        if (!string.IsNullOrEmpty(hdCurrencyID.Value))
        {
            Currency oCurrency = new Currency(hdCurrencyID.Value);
            CurrencyDAL oCurrencyDAL = new CurrencyDAL();
            oCurrency.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oCurrencyDAL.Approve(oCurrency);
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
        Currency oCurrency = new Currency();
        CurrencyDAL oCurrencyDAL = new CurrencyDAL();

        oCurrency.CurrencyID = Request[txtCurrencyID.UniqueID].Trim().ToUpper();
        txtCurrencyID.Text = Request[txtCurrencyID.UniqueID].Trim().ToUpper();
        oCurrency.CurrencyCode = txtCurrencyCode.Text;
        oCurrency.CurrencyDesc = txtDescription.Text;
        oCurrency.CurrencySymbol = txtCurrencySymbol.Text;

        oCurrency.UserDetails = ucUserDet.UserDetail;

        oCurrency.UserDetails.MakeDate = DateTime.Now;
        ucUserDet.ResetData();
        Result oResult = (Result)oCurrencyDAL.Save(oCurrency);

        if (oResult.Status)
        {
            this.LoadList();
            this.ClearTextValue();
            hdCurrencyID.Value = string.Empty;

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

    #endregion Basic Operational Function from control EVENT


    #region Supporting or Utility function

    public void ClearTextValue()
    {        
        txtCurrencyID.ReadOnly = false;
        txtCurrencyID.Text = string.Empty;
        txtCurrencyCode.Text = string.Empty;
        txtDescription.Text = string.Empty;
        txtCurrencySymbol.Text = string.Empty;
        ucUserDet.ResetData();
        hdDataType.Value = string.Empty;
    }

    public void LoadList()
    {
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (oConfig != null)
        {
            Currency oCurrency = new Currency();
            CurrencyDAL oCurrencyDAL = new CurrencyDAL();
            Result oResult = oCurrencyDAL.LoadUnapprovedList(oConfig.UserName, false);

            if (oResult.Status)
            {
                DataTable dtTmpCurrencyList = (DataTable)oResult.Return;
                if (dtTmpCurrencyList.Rows.Count > 0)
                {
                    dtTmpCurrencyList.Columns.Remove("MakerID");

                    Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpCurrencyList;


                    gvList.DataSource = dtTmpCurrencyList;
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
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
        }
    }

    private void LoadDataByID(string sCurrencyID)
    {
        Currency oCurrency = new Currency(sCurrencyID);
        CurrencyDAL oCurrencyDAL = new CurrencyDAL();
        Result oResult = new Result();
        oResult = oCurrencyDAL.LoadByID(oCurrency);
        if (oResult.Status)
        {
            oCurrency = (Currency)oResult.Return;

            txtCurrencyID.Text = oCurrency.CurrencyID.Trim();
            txtCurrencyID.ReadOnly = true;
            txtCurrencyCode.Text = oCurrency.CurrencyCode;
            txtDescription.Text = oCurrency.CurrencyDesc;
            txtCurrencySymbol.Text = oCurrency.CurrencySymbol;
            if (string.IsNullOrEmpty(hdDataType.Value))
            {
                //When Loading from Approver End
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.MakerID = oCurrency.UserDetails.MakerID;
                userDetails.MakeDate = oCurrency.UserDetails.MakeDate;
                ucUserDet.UserDetail = userDetails;
            }
            else if (hdDataType.Value.Equals("T"))
            {
                //When loading from temp table
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.CheckDate = oCurrency.UserDetails.CheckDate;
                userDetails.CheckerID = oCurrency.UserDetails.CheckerID;
                userDetails.CheckDate = oCurrency.UserDetails.CheckDate;
                userDetails.CheckerComment = oCurrency.UserDetails.CheckerComment;
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
