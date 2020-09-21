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
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Configuration;


public partial class CustomerDetailSetup : System.Web.UI.Page
{
    #region Local Variable

    CryptographyManager oCrypManager = new CryptographyManager();
    public const string OBJ_CUSTOMER_ID = "CustomerID";
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
                if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CONFIG.CUSTOMER_DETAIL))
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
        string customerID = Request.QueryString[OBJ_CUSTOMER_ID];
        string sPageID = Request.QueryString[OBJ_PAGE_ID];
        gvList.PageSize = (int)Constants.PAGING_UNAPPROVED;

        if (!string.IsNullOrEmpty(customerID))
        {
            customerID = oCrypManager.GetDecryptedString(customerID, Constants.CRYPT_PASSWORD_STRING);
        }
        if (!string.IsNullOrEmpty(sPageID))
        {
            sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
        }

        if (!string.IsNullOrEmpty(customerID) && !string.IsNullOrEmpty(sPageID))
        {
            string sOperationType = sPageID.Substring(4, 1);
            if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
            {
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                LoadDataByID(Convert.ToInt32(customerID == "" ? "-1" : customerID));

                // general Control
                Util.ControlEnabled(txtCustomerID, false);
                Util.ControlEnabled(txtCustomerName, false);                
                Util.ControlEnabled(txtDateofBirth, false);
                Util.ControlEnabled(txtAddress, false);
                Util.ControlEnabled(txtForignAddress, false);
                Util.ControlEnabled(txtMasterNo, false);
                Util.ControlEnabled(txtNationalID, false);
                Util.ControlEnabled(txtNationality, false);
                Util.ControlEnabled(txtPassportNo, false);
                Util.ControlEnabled(txtIssueAt, false);
                Util.ControlEnabled(txtEmail, false);
                Util.ControlEnabled(txtPhone, false);
                Util.ControlEnabled(ddlSex, false);
                Util.ControlEnabled(txtBirthCertNo, false);

                Util.ControlEnabled(txtCustomerName2, false);
                Util.ControlEnabled(txtDateofBirth2, false);
                Util.ControlEnabled(txtAddress2, false);
                Util.ControlEnabled(txtForignAddress2, false);
                Util.ControlEnabled(txtNationalID2, false);
                Util.ControlEnabled(txtNationality2, false);
                Util.ControlEnabled(txtPassportNo2, false);
                Util.ControlEnabled(txtIssueAt2, false);
                Util.ControlEnabled(txtEmail2, false);
                Util.ControlEnabled(txtPhone2, false);
                Util.ControlEnabled(ddlSex2, false);
                Util.ControlEnabled(txtBirthCertNo2, false);

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
            // button 
            Util.ControlEnabled(btnReject, false);
            Util.ControlEnabled(btnApprove, false);
            Util.ControlEnabled(btnBack, false);

            Util.ControlEnabled(btnReset, true);
            Util.ControlEnabled(btnSave, true);
            Util.ControlEnabled(btnDelete, true);

            #region User-Detail.
            Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), false);
            UserDetails oUserDetails = new UserDetails();
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
            Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CUSTOMER_DETAIL).PadLeft(5, '0'), false);
        }
        else
        {
            // no action
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.CUSTOMER_DETAIL).PadLeft(5, '0'), false);
    } 

    protected void btnMasterLoad_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request[txtMasterNo.UniqueID].Trim().ToUpper()))
        {
            CustomerDetails oCustomerDetails = new CustomerDetails();
            oCustomerDetails.MasterNo = txtMasterNo.Text;
            CustomerDetailsDAL oCustomerDetailsDAL = new CustomerDetailsDAL();
            Result oResult = new Result();
            ClearTextValue();
            oResult = oCustomerDetailsDAL.LoadByID(oCustomerDetails);
            if (oResult.Status)
            {
                txtMasterNo.Text = oCustomerDetails.MasterNo;
                txtCustomerName.Text = oCustomerDetails.CustomerName;
                txtPhone.Text = oCustomerDetails.Phone;
                txtAddress.Text = oCustomerDetails.Address;
                txtNationalID.Text= oCustomerDetails.NationalID;
                txtEmail.Text = oCustomerDetails.EmailAddress;
                
                hdCustomerID.Value = "";
            }
        }
        else
        {            
            ucMessage.OpenMessage("Master ID cannot be null for this types of searching", Constants.MSG_TYPE_ERROR);
        }
    }
    protected void btnCustomerLoad_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request[txtCustomerID.UniqueID].Trim().ToUpper()))
        {
            CustomerDetails oCustomerDetails = new CustomerDetails(Util.GetIntNumber(Request[txtCustomerID.UniqueID].Trim().ToUpper()));
            CustomerDetailsDAL oCustomerDetailsDAL = new CustomerDetailsDAL();
            Result oResult = new Result();

            oResult = oCustomerDetailsDAL.LoadByID(oCustomerDetails);
            if (oResult.Status)
            {
                ClearTextValue();
                oCustomerDetails = (CustomerDetails)oResult.Return;
                txtCustomerID.Text = oCustomerDetails.CustomerID.ToString();
                txtCustomerName.Text = oCustomerDetails.CustomerName;
                txtAddress.Text = oCustomerDetails.Address;
                txtForignAddress.Text = oCustomerDetails.ForeignAddress;
                txtPhone.Text = oCustomerDetails.Phone;
                txtDateofBirth.Text = oCustomerDetails.DateOfBirth.ToString(Constants.DATETIME_FORMAT);
                ddlSex.Text = oCustomerDetails.Sex.ToString();
                txtNationality.Text = oCustomerDetails.Nationality;
                txtPassportNo.Text = oCustomerDetails.PassportNo;
                txtIssueAt.Text = oCustomerDetails.PassportNo_IssuedAt;
                txtNationalID.Text = oCustomerDetails.NationalID;
                txtBirthCertNo.Text = oCustomerDetails.BirthCertificateNo;
                txtEmail.Text = oCustomerDetails.EmailAddress;

                txtCustomerName2.Text = oCustomerDetails.CustomerName2;
                txtAddress2.Text = oCustomerDetails.Address2;
                txtForignAddress2.Text = oCustomerDetails.ForeignAddress2;
                txtPhone2.Text = oCustomerDetails.Phone2;
                txtDateofBirth2.Text = oCustomerDetails.DateOfBirth2.ToString(Constants.DATETIME_FORMAT);
                ddlSex2.Text = oCustomerDetails.Sex2.ToString();
                txtNationality2.Text = oCustomerDetails.Nationality2;
                txtPassportNo2.Text = oCustomerDetails.PassportNo2;
                txtIssueAt2.Text = oCustomerDetails.IssuedAt2;
                txtNationalID2.Text = oCustomerDetails.NationalID2;
                txtBirthCertNo2.Text = oCustomerDetails.BirthCertificateNo2;
                txtEmail2.Text = oCustomerDetails.EmailAddress2;

                hdCustomerID.Value = oCustomerDetails.CustomerID.ToString();
            }
        }
        else
        {            
            ucMessage.OpenMessage("Customer ID cannot be null for this types of searching", Constants.MSG_TYPE_ERROR);
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdCustomerID.Value))
        {
            CustomerDetailsDAL oCustomerDetailsDAL = new CustomerDetailsDAL();
            Result oResult = (Result)oCustomerDetailsDAL.Detete(hdCustomerID.Value);
            if (oResult.Status)
            {
                this.LoadList();
                this.ClearTextValue();
                hdCustomerID.Value = string.Empty;

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
        if (!string.IsNullOrEmpty(hdCustomerID.Value))
        {
            CustomerDetails oCustomerDetails = new CustomerDetails(Convert.ToInt32(hdCustomerID.Value == "" ? "-1" : hdCustomerID.Value));
            CustomerDetailsDAL oCustomerDetailsDAL = new CustomerDetailsDAL();
            oCustomerDetails.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oCustomerDetailsDAL.Reject(oCustomerDetails);
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
            CustomerDetails oCustomerDetails = new CustomerDetails(Convert.ToInt32(hdCustomerID.Value == "" ? "-1" : hdCustomerID.Value));
            CustomerDetailsDAL oCustomerDetailsDAL = new CustomerDetailsDAL();
            oCustomerDetails.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oCustomerDetailsDAL.Approve(oCustomerDetails);
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
        CustomerDetails oCustomerDetails = new CustomerDetails();
        CustomerDetailsDAL oCustomerDetailsDAL = new CustomerDetailsDAL();

        oCustomerDetails.CustomerID = Util.GetIntNumber(Request[hdCustomerID.UniqueID] == "" ? "-1" : Request[hdCustomerID.UniqueID].Trim().ToUpper());
        txtCustomerID.Text= Request[hdCustomerID.UniqueID].Trim().ToUpper();
        oCustomerDetails.CustomerName = txtCustomerName.Text;
        oCustomerDetails.Address = txtAddress.Text;
        oCustomerDetails.ForeignAddress = txtForignAddress.Text;
        oCustomerDetails.Phone = txtPhone.Text;
        oCustomerDetails.DateOfBirth = Util.GetDateTimeByString(txtDateofBirth.Text);
        oCustomerDetails.Sex = ddlSex.SelectedItem.Value;
        oCustomerDetails.Nationality = txtNationality.Text; 
        oCustomerDetails.PassportNo = txtPassportNo.Text; 
        oCustomerDetails.PassportNo_IssuedAt = txtIssueAt.Text; 
        oCustomerDetails.NationalID = txtNationalID.Text;
        oCustomerDetails.BirthCertificateNo = txtBirthCertNo.Text;
        oCustomerDetails.EmailAddress = txtEmail.Text;
        
        oCustomerDetails.CustomerName2 = txtCustomerName2.Text;
        oCustomerDetails.Address2 = txtAddress2.Text;
        oCustomerDetails.ForeignAddress2 = txtForignAddress2.Text;
        oCustomerDetails.Phone2 = txtPhone2.Text;
        oCustomerDetails.DateOfBirth2 = Util.GetDateTimeByString(txtDateofBirth2.Text);
        oCustomerDetails.Sex2 = ddlSex2.SelectedItem.Value;
        oCustomerDetails.Nationality2 = txtNationality2.Text;
        oCustomerDetails.PassportNo2 = txtPassportNo2.Text;
        oCustomerDetails.IssuedAt2 = txtIssueAt2.Text;
        oCustomerDetails.NationalID2 = txtNationalID2.Text;
        oCustomerDetails.BirthCertificateNo2 = txtBirthCertNo2.Text;
        oCustomerDetails.EmailAddress2 = txtEmail2.Text;
        
        oCustomerDetails.UserDetails = ucUserDet.UserDetail;
        oCustomerDetails.UserDetails.MakeDate = DateTime.Now;
        ucUserDet.ResetData();
        Result oResult = (Result)oCustomerDetailsDAL.Save(oCustomerDetails);

        if (oResult.Status)
        {
            this.LoadList();
            this.ClearTextValue();
            hdCustomerID.Value = string.Empty;

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

            this.gvList.HeaderRow.Cells[1].Text = "Customer ID";
            this.gvList.HeaderRow.Cells[2].Text = "Customer Name";
            this.gvList.HeaderRow.Cells[3].Text = "Date of Birth";
            this.gvList.HeaderRow.Cells[4].Text = "Address";
            this.gvList.HeaderRow.Cells[5].Text = "Phone";
            this.gvList.HeaderRow.Cells[6].Text = "Email";
            this.gvList.HeaderRow.Cells[7].Text = "Make Date";
        }
    }

    protected void gvList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (!e.CommandName.Equals("Page"))
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            LoadDataByID(Convert.ToInt32(gvRow.Cells[1].Text == "" ? "-1" : gvRow.Cells[1].Text));
        }
    }

    #endregion Basic Operational Function from control EVENT


    #region Supporting or Utility function

    public void ClearTextValue()
    {
        txtCustomerID.Text = "";
        txtCustomerID.ReadOnly = false;
        txtCustomerName.Text = "";
        txtAddress.Text = "";
        txtForignAddress.Text = "";
        txtPhone.Text = "";
        txtDateofBirth.Text = "";
        ddlSex.SelectedIndex = 0;
        txtNationality.Text = "";
        txtPassportNo.Text = "";
        txtIssueAt.Text = "";
        txtNationalID.Text = "";
        txtBirthCertNo.Text = "";
        txtEmail.Text = "";

        txtCustomerName2.Text = "";
        txtAddress2.Text = "";
        txtForignAddress2.Text = "";
        txtPhone2.Text = "";
        txtDateofBirth2.Text = "";
        ddlSex2.SelectedIndex = 0;
        txtNationality2.Text = "";
        txtPassportNo2.Text = "";
        txtIssueAt2.Text = "";
        txtNationalID2.Text = "";
        txtBirthCertNo2.Text = "";
        txtEmail2.Text = "";        
    }


    public void LoadList()
    {
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (oConfig != null)
        {
            CustomerDetails oCustomerDetails = new CustomerDetails();
            CustomerDetailsDAL oCustomerDetailsDAL = new CustomerDetailsDAL();
            Result oResult = oCustomerDetailsDAL.LoadUnapprovedList(oConfig.UserName, false);

            if (oResult.Status)
            {
                DataTable dtTmpDataList = (DataTable)oResult.Return;
                if (dtTmpDataList.Rows.Count > 0)
                {
                    dtTmpDataList.Columns.Remove("MakerID");

                    gvList.DataSource = dtTmpDataList;
                    gvList.DataBind();

                    Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpDataList;

                    this.gvList.HeaderRow.Cells[1].Text = "Customer ID";
                    this.gvList.HeaderRow.Cells[2].Text = "Customer Name";
                    this.gvList.HeaderRow.Cells[3].Text = "Date of Birth";
                    this.gvList.HeaderRow.Cells[4].Text = "Address";
                    this.gvList.HeaderRow.Cells[5].Text = "Phone";
                    this.gvList.HeaderRow.Cells[6].Text = "Email";
                    this.gvList.HeaderRow.Cells[7].Text = "Make Date";
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

    private void LoadDataByID(int sCustomerID)
    {
        CustomerDetails oCustomerDetails = new CustomerDetails( sCustomerID);
        CustomerDetailsDAL oCustomerDetailsDAL = new CustomerDetailsDAL();
        Result oResult = new Result();

        oResult = oCustomerDetailsDAL.LoadByID(oCustomerDetails);
        if (oResult.Status)
        {
            oCustomerDetails = (CustomerDetails)oResult.Return;

            txtCustomerID.Text = oCustomerDetails.CustomerID.ToString();
            txtCustomerID.ReadOnly = true;
            txtCustomerName.Text = oCustomerDetails.CustomerName;
            txtAddress.Text = oCustomerDetails.Address;            
            txtForignAddress.Text = oCustomerDetails.ForeignAddress;
            txtPhone.Text = oCustomerDetails.Phone;
            txtDateofBirth.Text = oCustomerDetails.DateOfBirth.ToString(Constants.DATETIME_FORMAT);             
            ddlSex.Text = oCustomerDetails.Sex.ToString();
            txtNationality.Text = oCustomerDetails.Nationality;
            txtPassportNo.Text = oCustomerDetails.PassportNo;
            txtIssueAt.Text = oCustomerDetails.PassportNo_IssuedAt;
            txtNationalID.Text = oCustomerDetails.NationalID;
            txtBirthCertNo.Text = oCustomerDetails.BirthCertificateNo;
            txtEmail.Text = oCustomerDetails.EmailAddress;

            txtCustomerName2.Text = oCustomerDetails.CustomerName2;
            txtAddress2.Text = oCustomerDetails.Address2;
            txtForignAddress2.Text = oCustomerDetails.ForeignAddress2;
            txtPhone2.Text = oCustomerDetails.Phone2;
            txtDateofBirth2.Text = oCustomerDetails.DateOfBirth2.ToString(Constants.DATETIME_FORMAT);
            ddlSex2.Text = oCustomerDetails.Sex2.ToString();            
            txtNationality2.Text = oCustomerDetails.Nationality2;
            txtPassportNo2.Text = oCustomerDetails.PassportNo2;
            txtIssueAt2.Text = oCustomerDetails.IssuedAt2;
            txtNationalID2.Text = oCustomerDetails.NationalID2;
            txtBirthCertNo2.Text = oCustomerDetails.BirthCertificateNo2;
            txtEmail2.Text = oCustomerDetails.EmailAddress2;

            ucUserDet.UserDetail = oCustomerDetails.UserDetails;

            hdCustomerID.Value = sCustomerID.ToString();
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
