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

public partial class DivisionSetup : System.Web.UI.Page
{
    #region Local Variable

    CryptographyManager oCrypManager = new CryptographyManager();
    public const string OBJ_DIVISION_ID = "DivisionID";
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
                if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CONFIG.DIVISION))
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
        DDListUtil.LoadDDLFromDB(ddlBranchCode, "BranchID", "BranchName", "SPMS_Branch", true);
        DDListUtil.LoadDDLFromXML(ddlCountryName, "Country", "Type", "Country", true);
        DDListUtil.LoadDDLFromXML(ddlFromMonth, "Month", "Type", "Month", true);
        DDListUtil.LoadDDLFromXML(ddlToMonth, "Month", "Type", "Month", true);
        gvDivisionList.PageSize = (int)Constants.PAGING_UNAPPROVED;
        
        if (ddlCountryName.Items.Count > 0)
        {
            ddlCountryName.SelectedIndex = 1;
        }

        string divisionID = Request.QueryString[OBJ_DIVISION_ID];
        string sPageID = Request.QueryString[OBJ_PAGE_ID];
        if (!string.IsNullOrEmpty(divisionID))
        {
            divisionID = oCrypManager.GetDecryptedString(divisionID, Constants.CRYPT_PASSWORD_STRING);
        }
        if (!string.IsNullOrEmpty(sPageID))
        {
            sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
        }
        
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (!string.IsNullOrEmpty(divisionID) && !string.IsNullOrEmpty(sPageID))
        {
            string sOperationType = sPageID.Substring(4, 1);
            if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
            {                
                LoadDataByID(divisionID);

                // general Control
                Util.ControlEnabled(txtDivisionID, false);
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
            Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.DIVISION).PadLeft(5, '0'), false);
        }
        else
        {
            // no action
        }
    }


    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.DIVISION).PadLeft(5, '0'), false);
    }
    
    public void PopLoadAction(string sID)
    {
        hdDataType.Value = "M";
        LoadDataByID(sID);
    }

    protected void btnLoad_Click(object sender, EventArgs e)
    {
        string sDivision = Request[txtDivisionID.UniqueID].Trim().ToUpper(); 
        ClearTextValue();
        hdDataType.Value = "L";
        LoadDataByID(sDivision);
        txtDivisionID.Text = sDivision;
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdDivisionID.Value))
        {
            DivisionDAL oDivisionDAL = new DivisionDAL();
            Result oResult = (Result)oDivisionDAL.Detete(hdDivisionID.Value);
            if (oResult.Status)
            {
                this.LoadList();
                ClearTextValue();
                hdDivisionID.Value = "";
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
        if (!string.IsNullOrEmpty(hdDivisionID.Value))
        {
            Division oDivision = new Division(hdDivisionID.Value);
            DivisionDAL oDivisionDAL = new DivisionDAL();
            oDivision.UserDetails = ucUserDet.UserDetail;
            Result oResult = (Result)oDivisionDAL.Reject(oDivision);

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
        if (!string.IsNullOrEmpty(hdDivisionID.Value))
        {
            Division oDivision = new Division(hdDivisionID.Value);
            DivisionDAL oDivisionDAL = new DivisionDAL();
            oDivision.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oDivisionDAL.Approve(oDivision);
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
        Division oDivision = new Division();
        DivisionDAL oDivisionDAL = new DivisionDAL();
        oDivision.DivisionID = Request[txtDivisionID.UniqueID].Trim().ToUpper();
        txtDivisionID.Text = Request[txtDivisionID.UniqueID].Trim().ToUpper(); 
        if (!string.IsNullOrEmpty(ddlBranchCode.SelectedItem.Value))
        {
            oDivision.Branch.BranchID = ddlBranchCode.SelectedItem.Value;
        }
        oDivision.DivisionName = txtDivisionName.Text;
        oDivision.BBCode = txtBdBankCode.Text;
        oDivision.Address = txtAddress.Text;
        oDivision.ZipCode = txtZipCode.Text;
        oDivision.Phone = txtPhoneNumber.Text;
        if (!string.IsNullOrEmpty(ddlCountryName.SelectedItem.Value))
        {
            oDivision.Country = ddlCountryName.SelectedItem.Value;
        }
        oDivision.Email = txtEmailID.Text;
        oDivision.Fax = txtFaxNumber.Text;
        oDivision.SWIFTBIC = txtSWIFTBIC.Text;
        if (!string.IsNullOrEmpty(ddlFromMonth.SelectedItem.Value))
        {
            oDivision.FiscalFormMonth = Convert.ToInt16(ddlFromMonth.SelectedItem.Value);
        }
        if (!string.IsNullOrEmpty(ddlToMonth.SelectedItem.Value))
        {
            oDivision.FiscalToMonth = Convert.ToInt16(ddlToMonth.SelectedItem.Value);
        }

        oDivision.UserDetails = ucUserDet.UserDetail;
        oDivision.UserDetails.MakeDate = DateTime.Now;
        ucUserDet.ResetData();

        Result oResult = (Result)oDivisionDAL.Save(oDivision);

        if (oResult.Status)
        {
            this.LoadList();
            ClearTextValue();
            hdDivisionID.Value = "";

            ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
        }
    }


    protected void gvDivisionList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvDivisionList.PageIndex = e.NewPageIndex;
        if (Session[Constants.SES_CONFIG_UNAPPROVE_DATA] != null)
        {
            DataTable dtTmpList = (DataTable)Session[Constants.SES_CONFIG_UNAPPROVE_DATA];
            gvDivisionList.DataSource = dtTmpList;
            gvDivisionList.DataBind();
        }
    }
    protected void gvDivisionList_RowCommand(object sender, GridViewCommandEventArgs e)
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
        txtDivisionID.Text = "";
        txtDivisionID.ReadOnly = false;
        ddlBranchCode.SelectedItem.Value = "";
        txtDivisionName.Text = "";
        txtBdBankCode.Text = "";
        txtAddress.Text = "";
        txtZipCode.Text = "";
        txtPhoneNumber.Text = "";
        txtEmailID.Text = "";
        txtFaxNumber.Text = "";
        txtSWIFTBIC.Text = "";
        ddlBranchCode.SelectedIndex = 0;
        if (ddlCountryName.Items.Count > 0)
        {
            ddlCountryName.SelectedIndex = 1;
        }
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
            Division oDivision = new Division();
            DivisionDAL oDivisionDAL = new DivisionDAL();
            Result oResult = oDivisionDAL.LoadUnapprovedList(oConfig.UserName, false);

            if (oResult.Status)
            {
                DataTable dtTmpDivisionList = (DataTable)oResult.Return;
                if (dtTmpDivisionList.Rows.Count > 0)
                {
                    dtTmpDivisionList.Columns.Remove("MakerID");

                    gvDivisionList.DataSource = dtTmpDivisionList;
                    gvDivisionList.DataBind();

                    Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpDivisionList;
                }
                else
                {
                    gvDivisionList.DataSource = null;
                    gvDivisionList.DataBind();
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
            }
        }
    }

    private void LoadDataByID(string sDivisionID)
    {
        Division oDivision = new Division(sDivisionID);
        DivisionDAL oDivisionDAL = new DivisionDAL();
        Result oResult = new Result();
        oResult = oDivisionDAL.LoadByID(oDivision);
        if (oResult.Status)
        {
            oDivision = (Division)oResult.Return;

            txtDivisionID.Text = oDivision.DivisionID.Trim();
            txtDivisionID.ReadOnly = true;
            txtDivisionName.Text = oDivision.DivisionName;
            txtBdBankCode.Text = oDivision.BBCode;
            txtAddress.Text = oDivision.Address;
            txtZipCode.Text = oDivision.ZipCode;
            txtPhoneNumber.Text = oDivision.Phone;
            txtEmailID.Text = oDivision.Email;
            txtFaxNumber.Text = oDivision.Fax;
            txtSWIFTBIC.Text = oDivision.SWIFTBIC;
            if (string.IsNullOrEmpty(hdDataType.Value))
            {
                //When Loading from Approver End
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.MakerID = oDivision.UserDetails.MakerID;
                userDetails.MakeDate = oDivision.UserDetails.MakeDate;
                ucUserDet.UserDetail = userDetails;
            }
            else if (hdDataType.Value.Equals("T"))
            {
                //When loading from temp table
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.CheckDate = oDivision.UserDetails.CheckDate;
                userDetails.CheckerID = oDivision.UserDetails.CheckerID;
                userDetails.CheckDate = oDivision.UserDetails.CheckDate;
                userDetails.CheckerComment = oDivision.UserDetails.CheckerComment;
                ucUserDet.UserDetail = userDetails;
            }            

            DDListUtil.Assign(ddlBranchCode, oDivision.Branch.BranchID);
            DDListUtil.Assign(ddlToMonth, oDivision.FiscalToMonth.ToString());
            DDListUtil.Assign(ddlFromMonth, oDivision.FiscalFormMonth.ToString());
            DDListUtil.Assign(ddlCountryName, oDivision.Country.ToString());
                    
            hdDivisionID.Value = sDivisionID;
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
        }
    }

    #endregion Supporting or Utility function
}
