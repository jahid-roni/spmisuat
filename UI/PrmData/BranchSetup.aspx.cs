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
using SBM_BLC1.Configuration;
using SBM_BLC1.Common;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Entity.SecurityAdmin;
using System.Collections;



public partial class BranchSetup : System.Web.UI.Page
{
    
    #region Local Variable

    CryptographyManager oCrypManager = new CryptographyManager();
    public const string OBJ_BRANCH_ID = "BranchID"; 
    public const string OBJ_PAGE_ID = "sPageID";
    public Hashtable htblControlsList = new Hashtable();  

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
                if (! chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CONFIG.BRANCH))
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
        DDListUtil.LoadDDLFromXML(ddlCountryName, "Country", "Type", "Country", true);
        gvBranchList.PageSize = (int)Constants.PAGING_UNAPPROVED;

        if (ddlCountryName.Items.Count > 0)
        {
            ddlCountryName.SelectedIndex = 1;
        }

        string branchID = Request.QueryString[OBJ_BRANCH_ID];
        string sPageID = Request.QueryString[OBJ_PAGE_ID];
        if (!string.IsNullOrEmpty(branchID))
        {
            branchID = oCrypManager.GetDecryptedString(branchID, Constants.CRYPT_PASSWORD_STRING);
        }
        if (!string.IsNullOrEmpty(sPageID))
        {
            sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
        }

        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if ( !string.IsNullOrEmpty(branchID) &&  !string.IsNullOrEmpty(sPageID))
        {
            string sOperationType = sPageID.Substring(4, 1);
            if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
            {                
                LoadDataByID(branchID);

                // general Control
                Util.ControlEnabled(txtBranchID, false);
                Util.ControlEnabled(txtAddress, false);
                Util.ControlEnabled(txtBranchName, false);
                Util.ControlEnabled(txtBdBankCode, false);
                Util.ControlEnabled(txtZipCode, false);
                Util.ControlEnabled(ddlCountryName, false);
                Util.ControlEnabled(txtPhoneNumber, false);
                Util.ControlEnabled(txtEmailID, false);
                Util.ControlEnabled(txtFaxNumber, false);
                Util.ControlEnabled(txtAddress, false);

                // user Detail
                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);

                // button 
                Util.ControlEnabled(btnLoad, false);
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);
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
            Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.BRANCH).PadLeft(5, '0'), false);
        }
        else
        {
            // no action
        }
    }
    
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.BRANCH).PadLeft(5, '0'), false);
    }

    protected void btnLoad_Click(object sender, EventArgs e)
    {
        string sBranchID = Request[this.txtBranchID.UniqueID].Trim().ToUpper();
        ClearTextValue();
        hdDataType.Value = "L";
        LoadDataByID(sBranchID);
        txtBranchID.Text = sBranchID;
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdBranchID.Value))
        {
            BranchDAL oBranchDAL = new BranchDAL();
            Result oResult = (Result)oBranchDAL.Detete(hdBranchID.Value);
            if (oResult.Status)
            {
                this.LoadList();
                ClearTextValue();
                hdBranchID.Value = "";
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

    protected void btnReset_Click(object sender, EventArgs e)
    {
        ClearTextValue();
    }

    protected void btnReject_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdBranchID.Value))
        {
            Branch oBranch = new Branch(hdBranchID.Value);
            BranchDAL oBranchDAL = new BranchDAL();
            oBranch.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oBranchDAL.Reject(oBranch);
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
        if (!string.IsNullOrEmpty(hdBranchID.Value))
        {
            Branch oBranch = new Branch(hdBranchID.Value);
            BranchDAL oBranchDAL = new BranchDAL();
            oBranch.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oBranchDAL.Approve(oBranch);
            if (oResult.Status)
            {
                ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE, Constants.MSG_TYPE_SUCCESS);
                ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE + " with<BR> <b>Branch Code:&nbsp;" + txtBranchID.Text + "&nbsp;:&nbsp;" + txtBranchName.Text.ToUpper() + "</B>", Constants.MSG_TYPE_SUCCESS);
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
        Branch oBranch = new Branch();
        BranchDAL oBranchDAL = new BranchDAL();
        oBranch.BranchID = Request[txtBranchID.UniqueID].Trim().ToUpper();
        txtBranchID.Text = Request[txtBranchID.UniqueID].Trim().ToUpper();
        oBranch.BranchName = txtBranchName.Text.ToUpper();
        oBranch.BbCode = txtBdBankCode.Text;
        oBranch.Address = txtAddress.Text;
        oBranch.ZipCode = txtZipCode.Text;
        oBranch.Phone = txtPhoneNumber.Text;
        if (!string.IsNullOrEmpty(ddlCountryName.SelectedItem.Value))
        {
            oBranch.Country = ddlCountryName.SelectedItem.Value;
        }
        oBranch.Email = txtEmailID.Text;
        oBranch.Fax = txtFaxNumber.Text;

        oBranch.UserDetails = ucUserDet.UserDetail;
        oBranch.UserDetails.MakeDate = DateTime.Now;
        ucUserDet.ResetData();
        Result oResult = (Result)oBranchDAL.Save(oBranch);

        if (oResult.Status)
        {
            this.LoadList();
            ClearTextValue();
            hdBranchID.Value = "";

            ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
           // ScriptManager.RegisterStartupScript(this.upData, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Save"), true);
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
           // ScriptManager.RegisterStartupScript(this.upData, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Save"), true);
        }
    }

    protected void gvBranchList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvBranchList.PageIndex = e.NewPageIndex;
        if (Session[Constants.SES_CONFIG_UNAPPROVE_DATA] != null)
        {
            DataTable dtTmpList = (DataTable)Session[Constants.SES_CONFIG_UNAPPROVE_DATA];
            gvBranchList.DataSource = dtTmpList;
            gvBranchList.DataBind();
        }
    }

    protected void gvBranchList_RowCommand(object sender, GridViewCommandEventArgs e)
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
        txtBranchID.Text = "";
        txtBranchID.ReadOnly = false;
        txtBranchName.Text = "";
        txtBdBankCode.Text = "";
        txtAddress.Text = "";
        txtZipCode.Text = "";
        txtPhoneNumber.Text = "";
        if(ddlCountryName.Items.Count>0){
            ddlCountryName.SelectedIndex=1;
        }
        txtEmailID.Text = "";
        txtFaxNumber.Text = "";
        ucUserDet.ResetData();
        hdDataType.Value = string.Empty;
    }

    public void LoadList()
    {
        Branch oBranch = new Branch();
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (oConfig != null)
        {
            BranchDAL oBranchDAL = new BranchDAL();
            Result oResult = oBranchDAL.LoadUnapprovedList(oConfig.UserName, false);

            if (oResult.Status)
            {
                DataTable dtTmpBranchList = (DataTable)oResult.Return;
                if (dtTmpBranchList.Rows.Count > 0)
                {
                    dtTmpBranchList.Columns.Remove("MakerID");

                    gvBranchList.DataSource = dtTmpBranchList;
                    gvBranchList.DataBind();

                    Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpBranchList;
                }
                else
                {
                    gvBranchList.DataSource = null;
                    gvBranchList.DataBind();
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
            }
        }
    }

    private void LoadDataByID(string sBranchID)
    {
        Branch oBranch = new Branch(sBranchID);
        BranchDAL oBranchDAL = new BranchDAL();
        Result oResult = new Result();
        oResult = oBranchDAL.LoadByID(oBranch);
        if (oResult.Status)
        {
            oBranch = (Branch)oResult.Return;

            txtBranchID.Text = oBranch.BranchID.Trim();
            txtBranchID.ReadOnly = true;
            txtBranchName.Text = oBranch.BranchName;
            txtBdBankCode.Text = oBranch.BbCode;
            txtAddress.Text = oBranch.Address;
            txtZipCode.Text = oBranch.ZipCode;
            txtPhoneNumber.Text = oBranch.Phone;
            txtEmailID.Text = oBranch.Email;
            txtFaxNumber.Text = oBranch.Fax;
            if (string.IsNullOrEmpty(hdDataType.Value))
            {
                //When Loading from Approver End
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.MakerID = oBranch.UserDetails.MakerID;
                userDetails.MakeDate = oBranch.UserDetails.MakeDate;
                ucUserDet.UserDetail = userDetails;
            }
            else if (hdDataType.Value.Equals("T"))
            {
                //When loading from temp table
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.CheckDate = oBranch.UserDetails.CheckDate;
                userDetails.CheckerID = oBranch.UserDetails.CheckerID;
                userDetails.CheckDate = oBranch.UserDetails.CheckDate;
                userDetails.CheckerComment = oBranch.UserDetails.CheckerComment;
                ucUserDet.UserDetail = userDetails;
            }            

            DDListUtil.Assign(ddlCountryName, oBranch.Country.ToString());
            hdBranchID.Value = sBranchID;
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
        }
    }

    #endregion Supporting or Utility function
    
}
