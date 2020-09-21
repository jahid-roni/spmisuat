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

public partial class CustomerTypeSetup : System.Web.UI.Page
{
    #region Local Variable

    CryptographyManager oCrypManager = new CryptographyManager();
    public const string OBJ_CUSTOMER_TYPE_ID = "CustomerTypeID";
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
                if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CONFIG.CUSTOMER_TYPE))
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
        string customerTypeID = Request.QueryString[OBJ_CUSTOMER_TYPE_ID];
        string sPageID = Request.QueryString[OBJ_PAGE_ID];
        gvList.PageSize = (int)Constants.PAGING_UNAPPROVED;

        if (!string.IsNullOrEmpty(customerTypeID))
        {
            customerTypeID = oCrypManager.GetDecryptedString(customerTypeID, Constants.CRYPT_PASSWORD_STRING);
        }
        if (!string.IsNullOrEmpty(sPageID))
        {
            sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
        }

        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (!string.IsNullOrEmpty(customerTypeID) && !string.IsNullOrEmpty(sPageID))
        {
            string sOperationType = sPageID.Substring(4, 1);
            if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
            {                
                LoadDataByID(customerTypeID);

                // general Control
                Util.ControlEnabled(txtCustomerTypeID, false);
                Util.ControlEnabled(txtDescription, false);
                Util.ControlEnabled(txtNoOfMaxMembers, false);
                Util.ControlEnabled(chkIsOrganization, false);

                // user Detail
                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);

                // button 
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);
                Util.ControlEnabled(btnLoad, false);
                Util.ControlEnabled(btnSearch, false);
                Util.ControlEnabled(btnBack, true);

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
            Util.ControlEnabled(btnReset, true);
            Util.ControlEnabled(btnSave, true);
            Util.ControlEnabled(btnDelete, true);
            Util.ControlEnabled(btnLoad, true);
            Util.ControlEnabled(btnSearch, true);
            Util.ControlEnabled(btnBack, false);
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

    public void PopErrorMsgAction(string sType)
    {
        if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
        {
            Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CUSTOMER_TYPE).PadLeft(5, '0'), false);
        }
        else
        {
            // no action
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CUSTOMER_TYPE).PadLeft(5, '0'), false);
    }

    protected void btnLoad_Click(object sender, EventArgs e)
    {
        string sCustTypeId = Request[this.txtCustomerTypeID.UniqueID].Trim().Trim().ToUpper(); 
        ClearTextValue();
        hdDataType.Value = "L";
        LoadDataByID(sCustTypeId);
        txtCustomerTypeID.Text = sCustTypeId;
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdCustomerTypeID.Value))
        {
            CustomerTypeDAL oCustomerTypeDAL = new CustomerTypeDAL();
            Result oResult = (Result)oCustomerTypeDAL.Detete(hdCustomerTypeID.Value);
            if (oResult.Status)
            {
                this.LoadList();
                ClearTextValue();
                hdCustomerTypeID.Value = string.Empty;
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
        if (!string.IsNullOrEmpty(hdCustomerTypeID.Value))
        {
            CustomerType oCustomerType = new CustomerType(hdCustomerTypeID.Value);
            CustomerTypeDAL oCustomerTypeDAL = new CustomerTypeDAL();
            oCustomerType.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oCustomerTypeDAL.Reject(oCustomerType);
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
        if (!string.IsNullOrEmpty(hdCustomerTypeID.Value))
        {
            CustomerType oCustomerType = new CustomerType(hdCustomerTypeID.Value);
            CustomerTypeDAL oCustomerTypeDAL = new CustomerTypeDAL();
            oCustomerType.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oCustomerTypeDAL.Approve(oCustomerType);
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
        CustomerType oCustomerType = new CustomerType();
        CustomerTypeDAL oCustomerTypeDAL = new CustomerTypeDAL();

        oCustomerType.CustomerTypeID = Request[txtCustomerTypeID.UniqueID].Trim().ToUpper();
        txtCustomerTypeID.Text = Request[txtCustomerTypeID.UniqueID].Trim().ToUpper(); 
        oCustomerType.CustomerTypeDesc = txtDescription.Text;
        oCustomerType.IsOrganization = Convert.ToBoolean(chkIsOrganization.Checked);
        oCustomerType.MaxMembers = Convert.ToInt32(txtNoOfMaxMembers.Text);

        oCustomerType.UserDetails = ucUserDet.UserDetail;
        oCustomerType.UserDetails.MakeDate = DateTime.Now;
        ucUserDet.ResetData();
        Result oResult = (Result)oCustomerTypeDAL.Save(oCustomerType);

        if (oResult.Status)
        {
            this.LoadList();
            ClearTextValue();
            hdCustomerTypeID.Value = string.Empty;

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

            this.gvList.HeaderRow.Cells[1].Text = "Customer Type ID";
            this.gvList.HeaderRow.Cells[2].Text = "Customer Type Description";
            this.gvList.HeaderRow.Cells[3].Text = "Max Members";
            this.gvList.HeaderRow.Cells[4].Text = "Is Organization";
            this.gvList.HeaderRow.Cells[5].Text = "Make Date";
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

    public void CustTypeLoadAction(string sID)
    {
        if (sID.Length > 0)
        {
            hdDataType.Value = "M";
            LoadDataByID(sID);
        }
    }

    public void ClearTextValue()
    {
        txtCustomerTypeID.Text="";
        txtCustomerTypeID.ReadOnly = false;
        txtDescription.Text="";
        chkIsOrganization.Checked = false;
        txtNoOfMaxMembers.Text = "";
        ucUserDet.ResetData();
        hdDataType.Value = string.Empty;
    }

    public void LoadList()
    {
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (oConfig != null)
        {

            CustomerType oCustomerType = new CustomerType();
            CustomerTypeDAL oCustomerTypeDAL = new CustomerTypeDAL();


            Result oResult = oCustomerTypeDAL.LoadUnapprovedList(oConfig.UserName, false);

            if (oResult.Status)
            {
                DataTable dtTmpCustomerTypeList = (DataTable)oResult.Return;
                if (dtTmpCustomerTypeList.Rows.Count > 0)
                {
                    dtTmpCustomerTypeList.Columns.Remove("MakerID");

                    gvList.DataSource = dtTmpCustomerTypeList;
                    gvList.DataBind();

                    Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpCustomerTypeList;

                    this.gvList.HeaderRow.Cells[1].Text = "Customer Type ID";
                    this.gvList.HeaderRow.Cells[2].Text = "Customer Type Description";
                    this.gvList.HeaderRow.Cells[3].Text = "Max Members";
                    this.gvList.HeaderRow.Cells[4].Text = "Is Organization";
                    this.gvList.HeaderRow.Cells[5].Text = "Make Date";
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


    private void LoadDataByID(string sCustomerTypeID)
    {
        CustomerType oCustomerType = new CustomerType(sCustomerTypeID);
        CustomerTypeDAL oCustomerTypeDAL = new CustomerTypeDAL();
        Result oResult = new Result();
        oResult = oCustomerTypeDAL.LoadByID(oCustomerType);
        if (oResult.Status)
        {
            oCustomerType = (CustomerType)oResult.Return;

            txtCustomerTypeID.Text = oCustomerType.CustomerTypeID.Trim();
            txtCustomerTypeID.ReadOnly = true;
            txtDescription.Text = oCustomerType.CustomerTypeDesc;
            txtNoOfMaxMembers.Text = oCustomerType.MaxMembers.ToString();
            chkIsOrganization.Checked = oCustomerType.IsOrganization;
            Util.ChkChangeSetColor(chkIsOrganization);
            if (string.IsNullOrEmpty(hdDataType.Value))
            {
                //When Loading from Approver End
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.MakerID = oCustomerType.UserDetails.MakerID;
                userDetails.MakeDate = oCustomerType.UserDetails.MakeDate;
                ucUserDet.UserDetail = userDetails;
            }
            else if (hdDataType.Value.Equals("T"))
            {
                //When loading from temp table
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.CheckDate = oCustomerType.UserDetails.CheckDate;
                userDetails.CheckerID = oCustomerType.UserDetails.CheckerID;
                userDetails.CheckDate = oCustomerType.UserDetails.CheckDate;
                userDetails.CheckerComment = oCustomerType.UserDetails.CheckerComment;
                ucUserDet.UserDetail = userDetails;
            } 
            
            hdCustomerTypeID.Value = sCustomerTypeID;
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
