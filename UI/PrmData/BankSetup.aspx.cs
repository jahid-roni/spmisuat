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

public partial class BankSetup : System.Web.UI.Page
{

    #region Local Variable

    CryptographyManager oCrypManager = new CryptographyManager();
    public const string OBJ_BANK_ID = "BankID";
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
                if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CONFIG.BANK))
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
        // Dropdown load
        string sMonth = DateTime.Today.Month.ToString();
        DDListUtil.Assign(ddlFromMonth, sMonth);
        DDListUtil.Assign(ddlToMonth, sMonth);
        DDListUtil.LoadDDLFromDB(ddlBranchCode, "BranchID", "BBCode", "SPMS_Branch", true);
        DDListUtil.LoadDDLFromXML(ddlCountryName, "Country", "Type", "Country", true);
        DDListUtil.LoadDDLFromXML(ddlFromMonth, "Month", "Type", "Month", true);
        DDListUtil.LoadDDLFromXML(ddlToMonth, "Month", "Type", "Month", true);
        if (ddlCountryName.Items.Count > 0)
        {
            ddlCountryName.SelectedIndex = 1;
        }

        gvBankList.PageSize = (int)Constants.PAGING_UNAPPROVED;

        string sBankID = Request.QueryString[OBJ_BANK_ID];
        string sPageID = Request.QueryString[OBJ_PAGE_ID];
        if (!string.IsNullOrEmpty(sBankID))
        {
            sBankID = oCrypManager.GetDecryptedString(sBankID, Constants.CRYPT_PASSWORD_STRING);
        }
        if (!string.IsNullOrEmpty(sPageID))
        {
            sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
        }
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (!string.IsNullOrEmpty(sBankID) && !string.IsNullOrEmpty(sPageID))
        {
            string sOperationType = sPageID.Substring(4, 1);
            if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
            {                
                LoadDataByID(sBankID);

                // general Control
                Util.ControlEnabled(txtBankID, false);
                Util.ControlEnabled(txtAddress, false);
                Util.ControlEnabled(txtDivisionName, false);
                Util.ControlEnabled(txtBdBankCode, false);
                Util.ControlEnabled(txtZipCode, false);
                Util.ControlEnabled(ddlCountryName, false);
                Util.ControlEnabled(txtPhoneNumber, false);
                Util.ControlEnabled(txtEmailID, false);
                Util.ControlEnabled(txtFaxNumber, false);
                Util.ControlEnabled(txtAddress, false);

                Util.ControlEnabled(txtSWIFTBIC, false);
                Util.ControlEnabled(ddlFromMonth, false);
                Util.ControlEnabled(ddlToMonth, false);
                Util.ControlEnabled(ddlBranchCode, false);


                // user Detail                
                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);

                // button 
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);
                Util.ControlEnabled(btnLoad, false);
                Util.ControlEnabled(btnBack, true);
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
            Util.ControlEnabled(btnReset, true);
            Util.ControlEnabled(btnSave, true);
            Util.ControlEnabled(btnDelete, true);
            Util.ControlEnabled(btnLoad, true);
            Util.ControlEnabled(btnBack, false);
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
    
    public void PopLoadAction(string sID)
    {
        hdDataType.Value = "M";
        LoadDataByID(sID);
    }

    public void PopErrorMsgAction(string sType)
    {
        if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
        {
            Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.BANK).PadLeft(5, '0'), false);
        }
        else
        {
            // no action
        }
    }
    
  
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.BANK).PadLeft(5, '0'), false);
    }

    protected void btnLoad_Click(object sender, EventArgs e)
    {
        string sBankID = Request[txtBankID.UniqueID].Trim().ToUpper();
        ClearTextValue();
        hdDataType.Value = "L";
        LoadDataByID(sBankID);
        txtBankID.Text = sBankID;
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdBankID.Value))
        {
            BankDAL oBankDAL = new BankDAL();
            Result oResult = (Result)oBankDAL.Detete(hdBankID.Value);
            if (oResult.Status)
            {
                LoadList();
                ClearTextValue();
                hdBankID.Value = "";
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
        if (!string.IsNullOrEmpty(hdBankID.Value))
        {
            Bank oBank = new Bank(hdBankID.Value);
            BankDAL oBankDAL = new BankDAL();
            oBank.UserDetails = ucUserDet.UserDetail;
            Result oResult = (Result)oBankDAL.Reject(oBank);

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
        if (!string.IsNullOrEmpty(hdBankID.Value))
        {
            Bank oBank = new Bank(hdBankID.Value);
            BankDAL oBankDAL = new BankDAL();
            oBank.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oBankDAL.Approve(oBank);
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
        Bank oBank = new Bank();
        BankDAL oBankDAL = new BankDAL();
        oBank.BankID = Request[txtBankID.UniqueID].Trim().ToUpper();
        if (!string.IsNullOrEmpty(ddlBranchCode.SelectedItem.Value))
        {
            oBank.Branch.BranchID = ddlBranchCode.SelectedItem.Value;
        }
        oBank.DivisionName = txtDivisionName.Text;
        oBank.BBCode = txtBdBankCode.Text;
        oBank.Address = txtAddress.Text;
        oBank.ZipCode = txtZipCode.Text;
        oBank.Phone = txtPhoneNumber.Text;
        if (!string.IsNullOrEmpty(ddlCountryName.SelectedItem.Value))
        {
            oBank.Country = ddlCountryName.SelectedItem.Value;
        }
        oBank.Email = txtEmailID.Text;
        oBank.Fax = txtFaxNumber.Text;
        oBank.SWIFTBIC = txtSWIFTBIC.Text;
        if (!string.IsNullOrEmpty(ddlFromMonth.SelectedItem.Value))
        {
            oBank.FiscalFormMonth = Convert.ToInt16(ddlFromMonth.SelectedItem.Value);
        }
        if (!string.IsNullOrEmpty(ddlToMonth.SelectedItem.Value))
        {
            oBank.FiscalToMonth = Convert.ToInt16(ddlToMonth.SelectedItem.Value);
        }

        oBank.UserDetails = ucUserDet.UserDetail;
        oBank.UserDetails.MakeDate = DateTime.Now;
        ucUserDet.ResetData();

        Result oResult = (Result)oBankDAL.Save(oBank);

        if (oResult.Status)
        {
            LoadList();
            ClearTextValue();
            hdBankID.Value = "";

            ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
        }
    }

    protected void gvBankList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvBankList.PageIndex = e.NewPageIndex;
        if (Session[Constants.SES_CONFIG_UNAPPROVE_DATA] != null)
        {
            DataTable dtTmpList = (DataTable)Session[Constants.SES_CONFIG_UNAPPROVE_DATA];
            gvBankList.DataSource = dtTmpList;
            gvBankList.DataBind();
        }
    }

    protected void gvBankList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (! e.CommandName.Equals("Page"))
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
        txtBankID.ReadOnly = false;
        txtBankID.Text = "";        
        txtDivisionName.Text = "";
        txtBdBankCode.Text = "";
        txtAddress.Text = "";
        txtZipCode.Text = "";
        txtPhoneNumber.Text = "";
        txtEmailID.Text = "";
        txtFaxNumber.Text = "";
        txtSWIFTBIC.Text = "";
        ddlBranchCode.SelectedIndex = 0;
        ddlCountryName.SelectedIndex = 0;
        ddlFromMonth.SelectedIndex = 0;
        ddlToMonth.SelectedIndex = 0;
        ucUserDet.ResetData();
        hdDataType.Value = string.Empty;
    }

    public void LoadList()
    {
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (oConfig != null)
        {
            Bank oBank = new Bank();
            BankDAL oBankDAL = new BankDAL();
            Result oResult = oBankDAL.LoadUnapprovedList(oConfig.UserName, false);

            if (oResult.Status)
            {
                DataTable dtTmpList = (DataTable)oResult.Return;
                if (dtTmpList.Rows.Count > 0)
                {
                    dtTmpList.Columns.Remove("MakerID");

                    gvBankList.DataSource = dtTmpList;
                    gvBankList.DataBind();

                    Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;
                }
                else
                {
                    gvBankList.DataSource = null;
                    gvBankList.DataBind();
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

    private void LoadDataByID(string sBankID)
    {
        Bank oBank = new Bank(sBankID);
        BankDAL oBankDAL = new BankDAL();
        Result oResult = new Result();
        oResult = oBankDAL.LoadByID(oBank);
        if (oResult.Status)
        {
            oBank = (Bank)oResult.Return;

            txtBankID.Text = oBank.BankID.Trim();
            txtBankID.ReadOnly = true;
            txtDivisionName.Text = oBank.DivisionName;
            txtBdBankCode.Text = oBank.BBCode;
            txtAddress.Text = oBank.Address;
            txtZipCode.Text = oBank.ZipCode;
            txtPhoneNumber.Text = oBank.Phone;
            txtEmailID.Text = oBank.Email;
            txtFaxNumber.Text = oBank.Fax;
            txtSWIFTBIC.Text = oBank.SWIFTBIC;

            if (string.IsNullOrEmpty(hdDataType.Value))
            {
                //When Loading from Approver End
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.MakerID = oBank.UserDetails.MakerID;
                userDetails.MakeDate = oBank.UserDetails.MakeDate;
                ucUserDet.UserDetail = userDetails;
            }
            else if (hdDataType.Value.Equals("T"))
            {
                //When loading from temp table
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.CheckerID = oBank.UserDetails.CheckerID;
                userDetails.CheckDate = oBank.UserDetails.CheckDate;
                userDetails.CheckerComment = oBank.UserDetails.CheckerComment;
                ucUserDet.UserDetail = userDetails;
            }
            
            DDListUtil.Assign(ddlBranchCode, oBank.Branch.BranchID);
            DDListUtil.Assign(ddlToMonth, oBank.FiscalToMonth.ToString());
            DDListUtil.Assign(ddlFromMonth, oBank.FiscalFormMonth.ToString());
            DDListUtil.Assign(ddlCountryName, oBank.Country.ToString());

            hdBankID.Value = sBankID;
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
        }
    }

    #endregion Supporting or Utility function
}
