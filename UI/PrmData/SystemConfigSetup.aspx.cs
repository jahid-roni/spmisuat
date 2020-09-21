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
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO; 


using SBM_BLC1.Common;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Configuration;
using SBM_BLC1.Entity.Configuration;




public partial class SystemConfigSetup : System.Web.UI.Page
{
    #region Local Variable

    CryptographyManager oCrypManager = new CryptographyManager();
    public const string OBJ_CUSTOMER_DATA_FILE_ID = "CustomerDataFile";
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
                if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CONFIG.SYSTEM_CONFIG))
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
        string sCustomerDataFileID = Request.QueryString[OBJ_CUSTOMER_DATA_FILE_ID];
        string sPageID = Request.QueryString[OBJ_PAGE_ID];
        if (!string.IsNullOrEmpty(sCustomerDataFileID))
        {
            sCustomerDataFileID = oCrypManager.GetDecryptedString(sCustomerDataFileID, Constants.CRYPT_PASSWORD_STRING);
        }
        if (!string.IsNullOrEmpty(sPageID))
        {
            sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
        }
        
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (!string.IsNullOrEmpty(sCustomerDataFileID) && !string.IsNullOrEmpty(sPageID))
        {
            string sOperationType = sPageID.Substring(4, 1);
            if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
            {                
                LoadDataByID(sCustomerDataFileID);

                // file path list
                Util.ControlEnabled(txtCustomerDataFile, false);
                Util.ControlEnabled(txtAccountDataFile, false);
                Util.ControlEnabled(txtAllJournal, false);
                Util.ControlEnabled(txtCommissionClaimJournalFile, false);
                Util.ControlEnabled(txtCommissionReimburseJournalFile, false);
                Util.ControlEnabled(txtEncashmentJournal, false);
                Util.ControlEnabled(txtEncashmentReimburseJournal, false);
                Util.ControlEnabled(txtInterestPaymentJournalFile, false);
                Util.ControlEnabled(txtInterestReimburseJournalFile, false);
                Util.ControlEnabled(txtIssueJournalFile, false);
                Util.ControlEnabled(txtReceiveJournalFile, false);
                Util.ControlEnabled(txtReconDataFile, false);
                Util.ControlEnabled(txtSalesStatementJournalFile, false);
                
                // general Control
                Util.ControlEnabled(txtOrigID, false);
                Util.ControlEnabled(txtRowTypeHeader, false);
                Util.ControlEnabled(txtRowTypeFooter, false);
                Util.ControlEnabled(txtDrTranCode, false);
                Util.ControlEnabled(txtDrCode, false);
                Util.ControlEnabled(txtCrCode, false);
                Util.ControlEnabled(txtCrTran, false);

                // user Detail
                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);

                // button 
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnBack, true);

                Util.ControlEnabled(btnLoad, false);
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

            Util.ControlEnabled(btnLoad, true);
            Util.ControlEnabled(btnReset, true);
            Util.ControlEnabled(btnSave, true);
            Util.ControlEnabled(btnDelete, true);
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
            Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SYSTEM_CONFIG).PadLeft(5, '0'), false);
        }
        else
        {
            // no action
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SYSTEM_CONFIG).PadLeft(5, '0'), false);
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdCustomerDataFileID.Value))
        {
            SystemConfigurationDAL oSCDAL = new SystemConfigurationDAL();
            Result oResult = (Result)oSCDAL.Detete(hdCustomerDataFileID.Value);
            if (oResult.Status)
            {
                this.LoadList();
                ClearTextValue();
                hdCustomerDataFileID.Value = "";
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

    protected void btnLoad_Click(object sender, EventArgs e)
    {
        //Load Approved Data
        SystemConfigurationDAL oSCDAL = new SystemConfigurationDAL();
        Result oResult = new Result();
        oResult = oSCDAL.LoadApprovedData();
        if (oResult.Status)
        {
            hdDataType.Value = "M";
            SetData((SystemConfiguration)oResult.Return);
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
        }
    }

    private void SetData(SystemConfiguration oSC)
    {
        // file path data
        txtCustomerDataFile.Text = oSC.CustomerDataFile.Trim();
        txtAccountDataFile.Text = oSC.AccountDataFile.Trim();
        txtAllJournal.Text = oSC.AllJournalFile.Trim();
        txtCommissionClaimJournalFile.Text = oSC.CommissionClaimJournalFile.Trim();
        txtCommissionReimburseJournalFile.Text = oSC.CommissionReimburseJournalFile.Trim();
        txtEncashmentJournal.Text = oSC.EncashmentJournalFile.Trim();
        txtEncashmentReimburseJournal.Text = oSC.EncashmentReimburseJournalFile.Trim();
        txtInterestPaymentJournalFile.Text = oSC.InterestPaymentJournalFile.Trim();
        txtInterestReimburseJournalFile.Text = oSC.InterestReimburseJournalFile.Trim();
        txtIssueJournalFile.Text = oSC.IssueJournalFile.Trim();
        txtReceiveJournalFile.Text = oSC.ReceiveJournalFile.Trim();
        txtReconDataFile.Text = oSC.ReconDataFile.Trim();
        txtSalesStatementJournalFile.Text = oSC.SalesStatementJournalFile.Trim();

        // field data
        txtOrigID.Text = oSC.OriginatorID.Trim();
        txtRowTypeHeader.Text = oSC.RowTypeHeader.Trim();
        txtRowTypeFooter.Text = oSC.RowTypeFooter.Trim();
        txtDrTranCode.Text = oSC.Dr_TransactionCode.Trim();
        txtDrCode.Text = oSC.Dr_Code.Trim();
        txtCrCode.Text = oSC.Cr_Code.Trim();
        txtCrTran.Text = oSC.Cr_TransactionCode.Trim();
        if (string.IsNullOrEmpty(hdDataType.Value))
        {
            //When Loading from Approver End
            UserDetails userDetails = ucUserDet.UserDetail;
            userDetails.MakerID = oSC.UserDetails.MakerID;
            userDetails.MakeDate = oSC.UserDetails.MakeDate;
            ucUserDet.UserDetail = userDetails;
        }
        else if (hdDataType.Value.Equals("T"))
        {
            //When loading from temp table
            UserDetails userDetails = ucUserDet.UserDetail;
            userDetails.CheckDate = oSC.UserDetails.CheckDate;
            userDetails.CheckerID = oSC.UserDetails.CheckerID;
            userDetails.CheckerComment = oSC.UserDetails.CheckerComment;
            userDetails.CheckDate = oSC.UserDetails.CheckDate;
            userDetails.CheckerComment = oSC.UserDetails.CheckerComment;
            ucUserDet.UserDetail = userDetails;
        }         

        hdCustomerDataFileID.Value = oSC.CustomerDataFile.Trim();
    }

    protected void btnReject_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdCustomerDataFileID.Value))
        {
            SystemConfiguration oSC = new SystemConfiguration(hdCustomerDataFileID.Value);
            SystemConfigurationDAL oSCDAL = new SystemConfigurationDAL();
            oSC.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oSCDAL.Reject(oSC);
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
        if (!string.IsNullOrEmpty(hdCustomerDataFileID.Value))
        {

        SystemConfiguration oSC = new SystemConfiguration(hdCustomerDataFileID.Value);
        SystemConfigurationDAL oSCDAL = new SystemConfigurationDAL();
        oSC.UserDetails = ucUserDet.UserDetail;

        Result oResult = (Result)oSCDAL.Approve(oSC);
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
        SystemConfiguration oSC = new SystemConfiguration();
        SystemConfigurationDAL oSCDAL = new SystemConfigurationDAL();

        // file path
        oSC.CustomerDataFile = txtCustomerDataFile.Text.Trim();
        oSC.AccountDataFile = txtAccountDataFile.Text.Trim();
        oSC.AllJournalFile = txtAllJournal.Text.Trim();
        oSC.CommissionClaimJournalFile = txtCommissionClaimJournalFile.Text.Trim();
        oSC.CommissionReimburseJournalFile = txtCommissionReimburseJournalFile.Text.Trim();
        oSC.EncashmentJournalFile = txtEncashmentJournal.Text.Trim();
        oSC.EncashmentReimburseJournalFile = txtEncashmentReimburseJournal.Text.Trim();
        oSC.InterestPaymentJournalFile = txtInterestPaymentJournalFile.Text.Trim();
        oSC.InterestReimburseJournalFile = txtInterestReimburseJournalFile.Text.Trim();
        oSC.IssueJournalFile = txtIssueJournalFile.Text.Trim();
        oSC.ReceiveJournalFile = txtReceiveJournalFile.Text.Trim();
        oSC.ReconDataFile = txtReconDataFile.Text.Trim();
        oSC.SalesStatementJournalFile = txtSalesStatementJournalFile.Text.Trim();        
        
        
        // fields list
        oSC.OriginatorID = txtOrigID.Text.Trim();
        oSC.RowTypeHeader = txtRowTypeHeader.Text.Trim();
        oSC.RowTypeFooter = txtRowTypeFooter.Text.Trim();
        oSC.Dr_TransactionCode = txtDrTranCode.Text.Trim();
        oSC.Dr_Code = txtDrCode.Text.Trim();
        oSC.Cr_Code = txtCrCode.Text.Trim();
        oSC.Cr_TransactionCode = txtCrTran.Text.Trim();


        oSC.UserDetails = ucUserDet.UserDetail;

        oSC.UserDetails.MakeDate = DateTime.Now;
        ucUserDet.ResetData();

        Result oResult = (Result)oSCDAL.Save(oSC);

        if (oResult.Status)
        {
            this.LoadList();
            ClearTextValue();
            hdCustomerDataFileID.Value = "";

            ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
        }
    }

    protected void gvList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
        hdDataType.Value = "T";
        LoadDataByID(gvRow.Cells[1].Text);
    }

    #endregion Basic Operational Function from control EVENT


    #region Supporting or Utility function

    public void ClearTextValue()
    {

        // file path data
        txtCustomerDataFile.Text = string.Empty;
        txtAccountDataFile.Text = string.Empty;
        txtAllJournal.Text = string.Empty;
        txtCommissionClaimJournalFile.Text = string.Empty;
        txtCommissionReimburseJournalFile.Text = string.Empty;
        txtEncashmentJournal.Text = string.Empty;
        txtEncashmentReimburseJournal.Text = string.Empty;
        txtInterestPaymentJournalFile.Text = string.Empty;
        txtInterestReimburseJournalFile.Text = string.Empty;
        txtIssueJournalFile.Text = string.Empty;
        txtReceiveJournalFile.Text = string.Empty;
        txtReconDataFile.Text = string.Empty;
        txtSalesStatementJournalFile.Text = string.Empty;
        // normal data fields
        txtOrigID.Text = string.Empty;
        txtRowTypeHeader.Text = string.Empty;
        txtRowTypeFooter.Text = string.Empty;
        txtDrTranCode.Text = string.Empty;
        txtDrCode.Text = string.Empty;
        txtCrCode.Text = string.Empty;
        txtCrTran.Text = string.Empty;
        ucUserDet.ResetData();
        hdDataType.Value = string.Empty;
    }

    public void LoadList()
    {
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (oConfig != null)
        {

            SystemConfiguration oSC = new SystemConfiguration();
            SystemConfigurationDAL oSCDAL = new SystemConfigurationDAL();
            Result oResult = oSCDAL.LoadUnapprovedList(oConfig.UserName, false);

            if (oResult.Status)
            {
                DataTable dtTmpDataList = (DataTable)oResult.Return;
                if (dtTmpDataList.Rows.Count > 0)
                {
                    dtTmpDataList.Columns.Remove("Dr_Code");
                    dtTmpDataList.Columns.Remove("Cr_Code");
                    dtTmpDataList.Columns.Remove("MakerID");

                    gvList.DataSource = dtTmpDataList;
                    gvList.DataBind();

                    this.gvList.HeaderRow.Cells[1].Text = "Customer Data File";
                    this.gvList.HeaderRow.Cells[2].Text = "Originator ID";
                    this.gvList.HeaderRow.Cells[3].Text = "Row Type Header";
                    this.gvList.HeaderRow.Cells[4].Text = "Row Type Footer";
                    this.gvList.HeaderRow.Cells[5].Text = "Dr Transaction Code";
                    this.gvList.HeaderRow.Cells[6].Text = "Cr Transaction Code";
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

    private void LoadDataByID(string sCustomerDataFileID)
    {
        SystemConfiguration oSC = new SystemConfiguration(sCustomerDataFileID);
        SystemConfigurationDAL oSCDAL = new SystemConfigurationDAL();
        Result oResult = new Result();
        oResult = oSCDAL.LoadByID(oSC);
        if (oResult.Status)
        {
            oSC = (SystemConfiguration)oResult.Return;

            SetData(oSC);
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

