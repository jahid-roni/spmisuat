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

public partial class CustomerTypeWiseSPLimitSetup : System.Web.UI.Page
{
    #region Local Variable

    CryptographyManager oCrypManager = new CryptographyManager();
    public const string OBJ_CUSTOMER_GROPU_ID = "sCustomerID";
    public const string OBJ_SPTYPE_GROPU_ID = "sSpTypeID";
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
                if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CONFIG.CUSTOMER_TYPE_WISE_SP_LIMIT))
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
        DDListUtil.LoadDDLFromDB(ddlCustType, "CustomerTypeID", "TypeDesc", "SPMS_CustomerType", true);
        gvList.PageSize = (int)Constants.PAGING_SEARCH;

        string sPTypeID = Request.QueryString[OBJ_SPTYPE_GROPU_ID];
        string sCustomerID = Request.QueryString[OBJ_CUSTOMER_GROPU_ID];
        string sPageID = Request.QueryString[OBJ_PAGE_ID];
        hdCustomerID.Value = string.Empty;

        if (!string.IsNullOrEmpty(sPTypeID))
        {
            sPTypeID = oCrypManager.GetDecryptedString(sPTypeID, Constants.CRYPT_PASSWORD_STRING);
        }
        if (!string.IsNullOrEmpty(sCustomerID))
        {
            sCustomerID = oCrypManager.GetDecryptedString(sCustomerID, Constants.CRYPT_PASSWORD_STRING);
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
                LoadDataByID(sCustomerID, sPTypeID);

                // general Control
                Util.ControlEnabled(ddlSPType, false);
                Util.ControlEnabled(ddlCustType, false);
                Util.ControlEnabled(txtMaxLim, false);
                Util.ControlEnabled(txtMinLim, false);

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

    public void PopErrorMsgAction(string sType)
    {
        if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
        {
            Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CUSTOMER_TYPE_WISE_SP_LIMIT).PadLeft(5, '0'), false);
        }
        else
        {
            // no action
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CUSTOMER_TYPE_WISE_SP_LIMIT).PadLeft(5, '0'), false);
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdCustomerID.Value))
        {
            CustomerTypeWiseSPLimitDAL oCTWiseSPLimitDAL = new CustomerTypeWiseSPLimitDAL();
            Result oResult = (Result)oCTWiseSPLimitDAL.Detete(hdCustomerID.Value == "" ? "0" : hdCustomerID.Value, hdSPTypeID.Value == "" ? "0" : hdSPTypeID.Value);
            if (oResult.Status)
            {
                ClearTextValue();
                this.LoadList();
                hdCustomerID.Value = "";
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
        ddlSPType.Enabled = true;
        ddlCustType.Enabled = true;
        if (ddlCustType.Items.Count > 0)
        {
            ddlCustType.SelectedIndex = 0;
        }
        if (ddlSPType.Items.Count > 0)
        {
            ddlSPType.SelectedIndex = 0;
        }
        txtMaxLim.Text = string.Empty;
        txtMinLim.Text = string.Empty;
        ucUserDet.ResetData();
        hdDataType.Value = string.Empty;
    }

    protected void btnReject_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdCustomerID.Value))
        {
            CustomerTypeWiseSPLimit oCTWiseSPLimit = new CustomerTypeWiseSPLimit(hdCustomerID.Value == "" ? "0" : hdCustomerID.Value, hdSPTypeID.Value == "" ? "0" : hdSPTypeID.Value);
            CustomerTypeWiseSPLimitDAL oCTWiseSPLimitDAL = new CustomerTypeWiseSPLimitDAL();
            oCTWiseSPLimit.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oCTWiseSPLimitDAL.Reject(oCTWiseSPLimit);
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
        if (!string.IsNullOrEmpty(hdCustomerID.Value))
        {
            CustomerTypeWiseSPLimit oCTWiseSPLimit = new CustomerTypeWiseSPLimit(hdCustomerID.Value == "" ? "0" : hdCustomerID.Value, hdSPTypeID.Value == "" ? "0" : hdSPTypeID.Value);
            CustomerTypeWiseSPLimitDAL oCTWiseSPLimitDAL = new CustomerTypeWiseSPLimitDAL();
            oCTWiseSPLimit.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oCTWiseSPLimitDAL.Approve(oCTWiseSPLimit);
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
            ucMessage.OpenMessage(Constants.MSG_ERROR_REJECT, Constants.MSG_TYPE_ERROR);
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        CustomerTypeWiseSPLimit oCTWiseSPLimit = new CustomerTypeWiseSPLimit();
        CustomerTypeWiseSPLimitDAL oCTWiseSPLimitDAL = new CustomerTypeWiseSPLimitDAL();

        oCTWiseSPLimit.SPType.SPTypeID = ddlSPType.SelectedItem.Value.Trim();
        oCTWiseSPLimit.CustomerType.CustomerTypeID = ddlCustType.SelectedItem.Value.Trim();

        oCTWiseSPLimit.MinimumLimit = Convert.ToInt32(txtMinLim.Text.Trim());
        oCTWiseSPLimit.MaximumLimit = Convert.ToInt32(txtMaxLim.Text.Trim());

        oCTWiseSPLimit.UserDetails = ucUserDet.UserDetail;
        oCTWiseSPLimit.UserDetails.MakeDate = DateTime.Now;
        ucUserDet.ResetData();
        Result oResult = (Result)oCTWiseSPLimitDAL.Save(oCTWiseSPLimit);

        if (oResult.Status)
        {
            ClearTextValue();
            this.LoadList();
            hdCustomerID.Value = "";

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
            HiddenField hdCustId = ((HiddenField)gvRow.FindControl("hdCustTypeID"));
            if (hdCustId != null)
            {                
                hdDataType.Value = "T";
                LoadDataByID(hdCustId.Value, gvRow.Cells[2].Text.Trim());
            }
        }
    }

    protected void gvList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvList.PageIndex = e.NewPageIndex;
        if (Session[Constants.SES_CONFIG_UNAPPROVE_DATA] != null)
        {
            DataTable dtTmpList = (DataTable)Session[Constants.SES_CONFIG_UNAPPROVE_DATA];

            DataTable dtTmp = new DataTable();
            dtTmp = dtTmpList.Copy();

            dtTmp.Columns.Remove("CustomerTypeID");
            dtTmp.Columns.Remove("MakerID");

            gvList.DataSource = dtTmp;
            gvList.DataBind();

            this.gvList.HeaderRow.Cells[1].Text = "Customer Type";
            this.gvList.HeaderRow.Cells[2].Text = "SP Type";
            this.gvList.HeaderRow.Cells[3].Text = "Minimum Limit";
            this.gvList.HeaderRow.Cells[4].Text = "Maximum Limit";
            this.gvList.HeaderRow.Cells[5].Text = "Make Date";


            int iStart = gvList.PageIndex * gvList.PageSize;
            //int iEnd = iStart + gvList.PageSize;
            //if (iEnd >= gvList.Rows.Count)
            //{
              //  iEnd = gvList.Rows.Count;
            //}
            //for (int i = iStart; i < iEnd; i++)
            for (int i = 0; i < gvList.Rows.Count; i++)
            {
                HiddenField oHdCust = ((HiddenField)gvList.Rows[i].FindControl("hdCustTypeID"));
                if (oHdCust != null)
                {
                    oHdCust.Value = dtTmpList.Rows[i+iStart]["CustomerTypeID"].ToString();
                }
            }
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
            CustomerTypeWiseSPLimit oCTWiseSPLimit = new CustomerTypeWiseSPLimit();
            CustomerTypeWiseSPLimitDAL oCTWiseSPLimitDAL = new CustomerTypeWiseSPLimitDAL();

            Result oResult = oCTWiseSPLimitDAL.LoadUnapprovedList(oConfig.UserName, false);

            if (oResult.Status)
            {
                DataTable dtTmpDataList = (DataTable)oResult.Return;
                DataTable dtTmp = new DataTable();
                dtTmp = dtTmpDataList.Copy();
                if (dtTmpDataList.Rows.Count > 0)
                {
                    dtTmpDataList.Columns.Remove("CustomerTypeID");
                    dtTmpDataList.Columns.Remove("MakerID");

                    gvList.DataSource = dtTmpDataList;
                    gvList.DataBind();

                    this.gvList.HeaderRow.Cells[1].Text = "Customer Type";
                    this.gvList.HeaderRow.Cells[2].Text = "SP Type";
                    this.gvList.HeaderRow.Cells[3].Text = "Minimum Limit";
                    this.gvList.HeaderRow.Cells[4].Text = "Maximum Limit";
                    this.gvList.HeaderRow.Cells[5].Text = "Make Date";


                    int iStart = gvList.PageIndex * gvList.PageSize;
                    int iEnd = iStart + gvList.PageSize;
                    if (iEnd >= gvList.Rows.Count)
                    {
                        iEnd = gvList.Rows.Count;
                    }
                    for (int i = iStart ; i < iEnd; i++)
                    {
                        HiddenField oHdCust =  ((HiddenField)gvList.Rows[i].FindControl("hdCustTypeID"));
                        if (oHdCust != null)
                        {
                            oHdCust.Value = dtTmp.Rows[i]["CustomerTypeID"].ToString();
                        }
                    }

                    Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmp;
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

    public void CustTypeWiseSPLimitLoadAction(string sCustomerID, string sSPTypeID)
    {
        hdDataType.Value = "M";
        LoadDataByID(sCustomerID, sSPTypeID);
    }

    private void LoadDataByID(string sCustomerID, string sSPTypeID)
    {
        CustomerTypeWiseSPLimit oCTWiseSPLimit = new CustomerTypeWiseSPLimit(sCustomerID, sSPTypeID);
        CustomerTypeWiseSPLimitDAL oCTWiseSPLimitDAL = new CustomerTypeWiseSPLimitDAL();

        Result oResult = new Result();
        oResult = oCTWiseSPLimitDAL.LoadByID(oCTWiseSPLimit);
        if (oResult.Status)
        {
            oCTWiseSPLimit = (CustomerTypeWiseSPLimit)oResult.Return;
            DDListUtil.Assign(ddlSPType, oCTWiseSPLimit.SPType.SPTypeID.Trim());
            DDListUtil.Assign(ddlCustType, oCTWiseSPLimit.CustomerType.CustomerTypeID.Trim());
            ddlCustType.Enabled = false;
            ddlSPType.Enabled = false;
            txtMinLim.Text = oCTWiseSPLimit.MinimumLimit.ToString();
            txtMaxLim.Text = oCTWiseSPLimit.MaximumLimit.ToString();
            if (string.IsNullOrEmpty(hdDataType.Value))
            {
                //When Loading from Approver End
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.MakerID = oCTWiseSPLimit.UserDetails.MakerID;
                userDetails.MakeDate = oCTWiseSPLimit.UserDetails.MakeDate;
                ucUserDet.UserDetail = userDetails;
            }
            else if (hdDataType.Value.Equals("T"))
            {
                //When loading from temp table
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.CheckDate = oCTWiseSPLimit.UserDetails.CheckDate;
                userDetails.CheckerID = oCTWiseSPLimit.UserDetails.CheckerID;
                userDetails.CheckDate = oCTWiseSPLimit.UserDetails.CheckDate;
                userDetails.CheckerComment = oCTWiseSPLimit.UserDetails.CheckerComment;
                ucUserDet.UserDetail = userDetails;
            } 
            
            hdCustomerID.Value = oCTWiseSPLimit.CustomerType.CustomerTypeID;
            hdSPTypeID.Value = oCTWiseSPLimit.SPType.SPTypeID;
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
        }
    }
    #endregion Supporting or Utility function

}