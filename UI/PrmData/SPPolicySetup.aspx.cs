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
using SBM_BLC1.DAL.Common;
using System.Collections;

public partial class SPPolicySetup : System.Web.UI.Page
{
    #region Local Variable

    CryptographyManager oCrypManager = new CryptographyManager();
    public const string OBJ_POLICY_ID = "sPolicyID";
    public const string OBJ_PAGE_ID = "sPageID";
    public const string SES_ELIGI_LIST = "sess_Eligibility";
    public const string SES_ACTI_CURR_LIST = "sess_ActiCurrency";
    public const string SES_EARLY_ENCASH_LIST = "sess_EarlyEncashment";
    public const string SES_GENERAL_INT_LIST = "sess_GeneralInt";

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
                if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CONFIG.SP_POLICY))
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

        Session.Add(SES_ELIGI_LIST, null);
        Session.Add(SES_ACTI_CURR_LIST, null);
        Session.Add(SES_EARLY_ENCASH_LIST, null);
        Session.Add(SES_GENERAL_INT_LIST, null);

        gvActiCurrency.DataSource = null;
        gvActiCurrency.DataBind();

        gvEncashmentIntRate.DataSource = null;
        gvEncashmentIntRate.DataBind();

        gvGeneralInt.DataSource = null;
        gvGeneralInt.DataBind();

        gvEliPaymentPolicy.DataSource = null;
        gvEliPaymentPolicy.DataBind();
        hdDataType.Value = "";

        // Dropdown load SPType
        DDListUtil.LoadDDLFromDB(ddlSPType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
        DDListUtil.LoadDDLFromDB(ddlCurrency, "CurrencyID", "CurrencyCode", "SPMS_Currency", true);
        DDListUtil.LoadDDLFromDB(ddlPaymentMode, "PaymentMode", "Description", "SPMS_PaymentMode", true);
        DDListUtil.LoadDDLFromXML(ddlApplicableSex, "ApplicableSex", "Type", "Sex", true);

        DDListUtil.LoadDDLFromDB(ddlEligibilityCustomerActivityType, "ActivityType", "Description", "SPMS_ActivityType", true);
        if (ddlEligibilityCustomerActivityType.Items.Count > 0)
        {
            ListItem lt = ddlEligibilityCustomerActivityType.Items.FindByText("2 : Issue Payment to BB");
            ddlEligibilityCustomerActivityType.Items.Remove(lt);
            lt = ddlEligibilityCustomerActivityType.Items.FindByText("3 : Commission Claim");
            ddlEligibilityCustomerActivityType.Items.Remove(lt);
            lt = ddlEligibilityCustomerActivityType.Items.FindByText("4 : Commission Receive");
            ddlEligibilityCustomerActivityType.Items.Remove(lt);
            lt = ddlEligibilityCustomerActivityType.Items.FindByText("6 : Interest Claim");
            ddlEligibilityCustomerActivityType.Items.Remove(lt);
            lt = ddlEligibilityCustomerActivityType.Items.FindByText("7 : Interest Reimburse");
            ddlEligibilityCustomerActivityType.Items.Remove(lt);
            lt = ddlEligibilityCustomerActivityType.Items.FindByText("9 : Principal Claim");
            ddlEligibilityCustomerActivityType.Items.Remove(lt);
            lt = ddlEligibilityCustomerActivityType.Items.FindByText("10 : Principal Reimburse");
            ddlEligibilityCustomerActivityType.Items.Remove(lt);
            lt = ddlEligibilityCustomerActivityType.Items.FindByText("11 : Remuneration Claim");
            ddlEligibilityCustomerActivityType.Items.Remove(lt);
            lt = ddlEligibilityCustomerActivityType.Items.FindByText("12 : Remuneration Receive");
            ddlEligibilityCustomerActivityType.Items.Remove(lt);
            lt = ddlEligibilityCustomerActivityType.Items.FindByText("13 : Receive");
            ddlEligibilityCustomerActivityType.Items.Remove(lt);
            lt = ddlEligibilityCustomerActivityType.Items.FindByText("14 : Duplicate Issue");
            ddlEligibilityCustomerActivityType.Items.Remove(lt);
            lt = ddlEligibilityCustomerActivityType.Items.FindByText("15 : Reinvestment");
            ddlEligibilityCustomerActivityType.Items.Remove(lt);
        }
        DDListUtil.LoadDDLFromDB(ddlCurrencyActiveType, "ActivityType", "Description", "SPMS_ActivityType", true);
        if (ddlCurrencyActiveType.Items.Count > 0)
        {
            ListItem lt = ddlCurrencyActiveType.Items.FindByText("2 : Issue Payment to BB");
            ddlCurrencyActiveType.Items.Remove(lt);
            lt = ddlCurrencyActiveType.Items.FindByText("13 : Receive");
            ddlCurrencyActiveType.Items.Remove(lt);
            lt = ddlCurrencyActiveType.Items.FindByText("14 : Duplicate Issue");
            ddlCurrencyActiveType.Items.Remove(lt);
            lt = ddlCurrencyActiveType.Items.FindByText("15 : Reinvestment");
            ddlCurrencyActiveType.Items.Remove(lt);
        }


        // gvCustomerType load  6.	Eligibility
        UtilityDAL oUtilityDAL = new UtilityDAL();
        Result oResult = new Result();
        oResult = oUtilityDAL.GetDDLDataList("CustomerTypeID", "TypeDesc", "SPMS_CustomerType", true);
        if (oResult.Status)
        {
            DataTable dtGetAll = (DataTable)oResult.Return;
            gvCustomerType.DataSource = dtGetAll;
            gvCustomerType.DataBind();

            GridViewRowCollection growArr = (GridViewRowCollection)gvCustomerType.Rows;
            foreach (GridViewRow row in growArr)
            {
                row.Cells[1].Visible = false;
            }
        }

        string sPolicyID = Request.QueryString[OBJ_POLICY_ID];
        string sPageID = Request.QueryString[OBJ_PAGE_ID];
        if (!string.IsNullOrEmpty(sPolicyID))
        {
            sPolicyID = oCrypManager.GetDecryptedString(sPolicyID, Constants.CRYPT_PASSWORD_STRING);
        }
        if (!string.IsNullOrEmpty(sPageID))
        {
            sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
        }

        SetRadioEffect();
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

        if (!string.IsNullOrEmpty(sPolicyID) && !string.IsNullOrEmpty(sPageID))
        {
            string sOperationType = sPageID.Substring(4, 1);
            if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
            {                
                LoadDataByID(Convert.ToInt32(sPolicyID));

                // common
                Util.ControlEnabled(txtEffectiveDate, false);
                Util.ControlEnabled(ddlSPType, false);
                Util.ControlEnabled(btnSearch, false);

                // general Control
                Util.ControlEnabled(rblIsSPDurationInMonth, false);
                Util.ControlEnabled(txtDuration, false);
                Util.ControlEnabled(txtNoOfCoupon, false);
                Util.ControlEnabled(ddlIntrType, false);
                Util.ControlEnabled(rblInterestTypeAfterIntPayment, false);
                Util.ControlEnabled(ddlPreMatIntrType, false);
                Util.ControlEnabled(rblGSPreMatIntrClaim, false);

                Util.ControlEnabled(rblIsBondHolderRequired, false);
                Util.ControlEnabled(rblIsNomineePerScripRequired, false);
                Util.ControlEnabled(rblIsFoeignAddressRequired, false);

                Util.ControlEnabled(rblReinvestmentSuported, false);
                Util.ControlEnabled(rblInterestReinvestable, false);
                Util.ControlEnabled(txtReinNumber, false);
                Util.ControlEnabled(rblPartiallyEncashable, false);
                Util.ControlEnabled(rblPartiallyEncashedReinvestable, false);

                // Currency Setup
                Util.ControlEnabled(ddlCurrencyActiveType, false);
                Util.ControlEnabled(ddlCurrency, false);
                gvActiCurrency.Enabled = false;

                //Early Encashment Setup
                Util.ControlEnabled(txtEarlyEncashCouponNo, false);
                Util.ControlEnabled(txtCommonIntRate, false);
                Util.ControlEnabled(chkMaturedCoupon, false);

                // General Interest Setup
                Util.ControlEnabled(txtGeneralIntCouponNo, false);
                Util.ControlEnabled(txtGIClaimRate, false);
                Util.ControlEnabled(txtNonclaimIntRate, false);

                // Commission Setup
                Util.ControlEnabled(txtComSetNonOrgComm, false);
                Util.ControlEnabled(rblComSetNonOrgChargeOnPer, false);
                
                Util.ControlEnabled(rblComSetNonOrgCalculateInt, false);
                Util.ControlEnabled(txtComSetOrgCommission, false);
                Util.ControlEnabled(rblComSetOrgChargeOnPer, false);
                Util.ControlEnabled(rblComSetOrgCalculateInt, false);
                Util.ControlEnabled(txtComSetIntRemuneration, false);
                Util.ControlEnabled(rblComSetIntRemuChargeOnPer, false);
                Util.ControlEnabled(rblComSetIntRemuCalculateInt, false);
                Util.ControlEnabled(txtComSetRemuneration, false);
                Util.ControlEnabled(rblComSetRemuChargeOnPer, false);
                Util.ControlEnabled(rblComSetRemuCalculateInt, false);
                Util.ControlEnabled(txtComSetLevi, false);
                Util.ControlEnabled(rblComSetLevi, false);
                Util.ControlEnabled(txtComSetIncomeTax, false);
                Util.ControlEnabled(rblComSetIncomeTax, false);
                Util.ControlEnabled(txtSocialSecurityAmount, false);
                Util.ControlEnabled(rblSocialSecurityAmount, false);
                Util.ControlEnabled(txtComSetIncomeTaxAbove, false);
                Util.ControlEnabled(chkYearly, false);


                // Eligibility Setup
                Util.ControlEnabled(ddlEligibilityCustomerActivityType, false);
                Util.ControlEnabled(txtMinimumAge, false);
                Util.ControlEnabled(txtMaximumAge, false);
                Util.ControlEnabled(ddlApplicableSex, false);
                Util.ControlEnabled(ddlPaymentMode, false);
                gvCustomerType.Enabled = false;
                gvEliPaymentPolicy.Enabled = false;

                // user Detail
                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);

                // button 
                Util.ControlEnabled(btnAddNewActiCurrency, false);
                Util.ControlEnabled(btnSaveEarlyEncashment, false);
                Util.ControlEnabled(btnGeneralSaveInterest, false);
                Util.ControlEnabled(btnAddNewEligibility, false);
                Util.ControlEnabled(btnReject, true);
                Util.ControlEnabled(btnApprove, true);
                Util.ControlEnabled(btnReset, false);
                Util.ControlEnabled(btnSave, false);
                Util.ControlEnabled(btnDelete, false);
                Util.ControlEnabled(btnAddNewSPPolicy, false);
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
            Util.ControlEnabled(btnSearch, true);
            Util.ControlEnabled(btnAddNewActiCurrency, true);
            Util.ControlEnabled(btnSaveEarlyEncashment, true);
            Util.ControlEnabled(btnGeneralSaveInterest, true);
            Util.ControlEnabled(btnAddNewEligibility, true);
            Util.ControlEnabled(btnReject, false);
            Util.ControlEnabled(btnApprove, false);
            Util.ControlEnabled(btnReset, true);
            Util.ControlEnabled(btnSave, true);
            Util.ControlEnabled(btnDelete, true);
            Util.ControlEnabled(btnAddNewSPPolicy, true);
            Util.ControlEnabled(btnBack, false);

            // Currency Setup
            Util.ControlEnabled(ddlCurrencyActiveType, true);
            Util.ControlEnabled(ddlCurrency, true);
            gvActiCurrency.Enabled = true;

            //Early Encashment Setup
            Util.ControlEnabled(txtEarlyEncashCouponNo, true);
            Util.ControlEnabled(txtCommonIntRate, true);
            Util.ControlEnabled(chkMaturedCoupon, true);

            // General Interest Setup
            Util.ControlEnabled(txtGeneralIntCouponNo, true);
            Util.ControlEnabled(txtGIClaimRate, true);
            Util.ControlEnabled(txtNonclaimIntRate, true);
           
            if (rblComSetNonOrgChargeOnPer.Items.Count > 0)
            {
                
                rblComSetNonOrgChargeOnPer.Items[0].Selected = true;
            }
            if (rblComSetNonOrgCalculateInt.Items.Count > 0)
            {
                
                rblComSetNonOrgCalculateInt.Items[0].Selected = true;
            }
            if (rblComSetOrgChargeOnPer.Items.Count > 0)
            {
                
                rblComSetOrgChargeOnPer.Items[0].Selected = true;
            }
            if (rblComSetOrgCalculateInt.Items.Count > 0)
            {

                rblComSetOrgCalculateInt.Items[0].Selected = true;
            }
            if (rblComSetIntRemuChargeOnPer.Items.Count > 0)
            {

                rblComSetIntRemuChargeOnPer.Items[0].Selected = true;
            }
            if (rblComSetIntRemuCalculateInt.Items.Count > 0)
            {

                rblComSetIntRemuCalculateInt.Items[0].Selected = true;
            }
            if (rblComSetRemuChargeOnPer.Items.Count > 0)
            {

                rblComSetRemuChargeOnPer.Items[0].Selected = true;
            }
            if (rblComSetRemuCalculateInt.Items.Count > 0)
            {

                rblComSetRemuCalculateInt.Items[0].Selected = true;
            }
            if (rblComSetLevi.Items.Count > 0)
            {

                rblComSetLevi.Items[0].Selected = true;
            }
            if (rblComSetIncomeTax.Items.Count > 0)
            {

                rblComSetIncomeTax.Items[0].Selected = true;
            }
            if (rblSocialSecurityAmount.Items.Count > 0)
            {

                rblSocialSecurityAmount.Items[0].Selected = true;
            }
            


            // Eligibility Setup
            Util.ControlEnabled(ddlEligibilityCustomerActivityType, true);
            Util.ControlEnabled(txtMinimumAge, true);
            Util.ControlEnabled(txtMaximumAge, true);
            Util.ControlEnabled(ddlApplicableSex, true);
            Util.ControlEnabled(ddlPaymentMode, true);
            gvCustomerType.Enabled = true;
            gvEliPaymentPolicy.Enabled = true;

            
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
            Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_POLICY).PadLeft(5, '0'), false);
        }
        else
        {
            // no action
        }
    }


    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PAGE_PRIMARY_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_CONFIG.SP_POLICY).PadLeft(5, '0'), false);
    } 

    protected void btnLoad_Click(object sender, EventArgs e)
    {
        //LoadDataByID(Convert.ToInt32(txtBranchID.Text));
    }

    public void LoadEarlyEncashmentAndGeneralInterest()
    {
        if (!string.IsNullOrEmpty(ddlSPType.SelectedValue))
        {
            SetRadioEffect();

            if (rblIsSPDurationInMonth.SelectedItem != null && txtDuration.Text != "" && txtNoOfCoupon.Text != "")
            {
                int iTotalMonth = TotalMonth();
                if (iTotalMonth > 0)
                {
                    int iMonthDiff = iTotalMonth / Util.GetIntNumber(txtNoOfCoupon.Text);
                    // Encashment Interest Rate
                    txtEarlyEncashCouponNo.Text = txtNoOfCoupon.Text;
                    DataTable oDtEE = new DataTable("dtDataEE");
                    oDtEE.Columns.Add(new DataColumn("bfCouponInstallmentNo", typeof(string)));
                    oDtEE.Columns.Add(new DataColumn("bfMonthFrom", typeof(string)));
                    oDtEE.Columns.Add(new DataColumn("bfMonthTo", typeof(string)));
                    oDtEE.Columns.Add(new DataColumn("bfInterestRate", typeof(string)));
                    oDtEE.Columns.Add(new DataColumn("bfNoOfSlabsIntPayable", typeof(string)));
                    oDtEE.Columns.Add(new DataColumn("hdPolicyID", typeof(string)));
                    oDtEE.Columns.Add(new DataColumn("hdSlabNoID", typeof(string)));
                    DataRow rowEE = null;


                    // 4.0 General Interest Setup
                    txtGeneralIntCouponNo.Text = txtNoOfCoupon.Text;
                    DataTable oDtGI = new DataTable("dtDataG");
                    oDtGI.Columns.Add(new DataColumn("bfCouponInstallmentNo", typeof(string)));
                    oDtGI.Columns.Add(new DataColumn("bfMonthFrom", typeof(string)));
                    oDtGI.Columns.Add(new DataColumn("bfMonthTo", typeof(string)));
                    oDtGI.Columns.Add(new DataColumn("bfClaimRate", typeof(string)));
                    oDtGI.Columns.Add(new DataColumn("bfNonclaimIntRate", typeof(string)));
                    oDtGI.Columns.Add(new DataColumn("hdPolicyID", typeof(string)));
                    oDtGI.Columns.Add(new DataColumn("hdSlabNoID", typeof(string)));
                    DataRow rowGI = null;

                    for (int i = 0; i < Util.GetIntNumber(txtNoOfCoupon.Text); i++)
                    {
                        // 
                        rowEE = oDtEE.NewRow();
                        rowEE["bfCouponInstallmentNo"] = i + 1;
                        rowEE["bfMonthFrom"] = 1 + (i * iMonthDiff);
                        rowEE["bfMonthTo"] = (i + 1) * iMonthDiff;
                        rowEE["bfInterestRate"] = "0.00";
                        rowEE["bfNoOfSlabsIntPayable"] = "0";
                        rowEE["hdPolicyID"] = "1";
                        rowEE["hdSlabNoID"] = i + 1;
                        oDtEE.Rows.Add(rowEE);


                        rowGI = oDtGI.NewRow();
                        rowGI["bfCouponInstallmentNo"] = i + 1;
                        rowGI["bfMonthFrom"] = 1 + (i * iMonthDiff); 
                        rowGI["bfMonthTo"] = (i + 1) * iMonthDiff;
                        rowGI["bfClaimRate"] = "0.00";
                        rowGI["bfNonclaimIntRate"] = "0.00";
                        rowGI["hdPolicyID"] = "1";
                        rowGI["hdSlabNoID"] = i + 1;
                        oDtGI.Rows.Add(rowGI);
                    }
                    gvEncashmentIntRate.DataSource = oDtEE;
                    gvEncashmentIntRate.DataBind();
                    Session[SES_EARLY_ENCASH_LIST] = oDtEE;

                    gvGeneralInt.DataSource = oDtGI;
                    gvGeneralInt.DataBind();
                    Session[SES_GENERAL_INT_LIST] = oDtGI;
                }
            }
        }
        else
        { 
            ucMessage.OpenMessage("Please select SP Type", Constants.MSG_TYPE_ERROR);
        }
    }
    protected void txtDuration_TextChanged(object sender, EventArgs e)
    {
        LoadEarlyEncashmentAndGeneralInterest(); 
    }
    protected void txtNoOfCoupon_TextChanged(object sender, EventArgs e)
    {
        LoadEarlyEncashmentAndGeneralInterest(); 
    }

    protected void rblIsSPDurationInMonth_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadEarlyEncashmentAndGeneralInterest(); 
    }


    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (hdDataType.Value != "M")
        {
            if (!string.IsNullOrEmpty(hdPolicyID.Value))
            {
                SPPolicyDAL oSPPolicyDAL = new SPPolicyDAL();
                Result oResult = (Result)oSPPolicyDAL.Detete(hdPolicyID.Value);
                if (oResult.Status)
                {
                    this.LoadList();
                    ClearTextValue();
                    hdPolicyID.Value = "";

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
        if (!string.IsNullOrEmpty(hdPolicyID.Value))
        {
            SPPolicy oSPPolicy = new SPPolicy(Convert.ToInt32(hdPolicyID.Value));
            SPPolicyDAL oSPPolicyDAL = new SPPolicyDAL();
            oSPPolicy.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oSPPolicyDAL.Reject(oSPPolicy);
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
        if (!string.IsNullOrEmpty(hdPolicyID.Value))
        {
            SPPolicy oSPPolicy = new SPPolicy(Convert.ToInt32(hdPolicyID.Value));
            SPPolicyDAL oSPPolicyDAL = new SPPolicyDAL();
            oSPPolicy.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oSPPolicyDAL.Approve(oSPPolicy);
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
        SPPolicyDAL oSPPolicyDAL = new SPPolicyDAL();
        
        //if it's a fresh entry or Policy Effective date has been modified after Load
        if (hdDataType.Value.Equals("") || (hdPolEffDate.Value != txtEffectiveDate.Text))
        {
            Result oResultP = oSPPolicyDAL.IsPolicyExist(ddlSPType.SelectedItem.Value, SBM_BLC1.Common.Date.GetDateTimeByString(txtEffectiveDate.Text));

            if (oResultP.Status)
            {
                if (Convert.ToInt32(oResultP.Return) > 0)
                {
                    ucMessage.OpenMessage("Already a Policy found on this date.", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                    return;
                }
            }
        }

        SPPolicy oSPPolicy = new SPPolicy();        

        // tmp...
        if (Util.CheckNumber(hdPolicyID.Value))
        {
            oSPPolicy.PolicyID = Util.GetIntNumber(hdPolicyID.Value);
        }
        else
        {
            oSPPolicy.PolicyID = -1;
            hdPolicyID.Value = "-1";
        }

        // common
        if (!string.IsNullOrEmpty(ddlSPType.SelectedItem.Value))
        {
            oSPPolicy.SPType.SPTypeID = ddlSPType.SelectedItem.Value;
        }
        oSPPolicy.PolicyEffectDate = SBM_BLC1.Common.Date.GetDateTimeByString(txtEffectiveDate.Text);


        // 1.0 General 
        oSPPolicy.IsSPDurationInMonth = Util.GetRadioData(rblIsSPDurationInMonth.SelectedItem.Value);
        oSPPolicy.SPDuration = Util.GetIntNumber(txtDuration.Text == "" ? "0" : txtDuration.Text);
        oSPPolicy.NoOfCoupons = Util.GetIntNumber(txtNoOfCoupon.Text == "" ? "0" : txtNoOfCoupon.Text);
        oSPPolicy.SPInterestType = Util.GetIntNumber(ddlIntrType.SelectedItem.Value == "" ? "0" : ddlIntrType.SelectedItem.Value);
        oSPPolicy.InterestTypeAfterIntPayment = Util.GetIntNumber(Util.GetRadioStringData(rblInterestTypeAfterIntPayment));
        if (ddlPreMatIntrType.SelectedItem != null)
        {
            oSPPolicy.PreMaturityInterestType = Util.GetIntNumber(ddlPreMatIntrType.SelectedItem.Value == "" ? "0" : ddlPreMatIntrType.SelectedItem.Value);
        }
        else
        {
            oSPPolicy.PreMaturityInterestType = 0;
        }
        oSPPolicy.PreMatIntTypeAfterIntPayment = Util.GetIntNumber(Util.GetRadioStringData(rblGSPreMatIntrClaim));
        oSPPolicy.IsBondHolderRequired = Util.GetRadioData(rblIsBondHolderRequired);
        oSPPolicy.IsNomineePerScripRequired = Util.GetRadioData(rblIsNomineePerScripRequired);
        oSPPolicy.IsFoeignAddressRequired = Util.GetRadioData(rblIsFoeignAddressRequired);

        oSPPolicy.ReinvestmentSuported = Util.GetRadioData(rblReinvestmentSuported);
        oSPPolicy.InterestReinvestable = Util.GetRadioData(rblInterestReinvestable);
        oSPPolicy.MaxNoOfReinvestment = Util.GetIntNumber(txtReinNumber.Text == "" ? "0" : txtReinNumber.Text);
        oSPPolicy.PartiallyEncashable = Util.GetRadioData(rblPartiallyEncashable);
        oSPPolicy.PartiallyEncashedReinvestable = Util.GetRadioData(rblPartiallyEncashedReinvestable);

        // 2.Activity Currency
        foreach (GridViewRow gvr in gvActiCurrency.Rows)
        {
            CurrencyActivityPolicy oCAPolicy = new CurrencyActivityPolicy();
            oCAPolicy.SPPolicy.PolicyID = Util.GetIntNumber(hdPolicyID.Value);

            oCAPolicy.ActivityTypeID = (gvr.FindControl("hdCustActiID") as HiddenField).Value;
            oCAPolicy.Currency.CurrencyID = (gvr.FindControl("hdCurrencyID") as HiddenField).Value;

            oSPPolicy.CurrencyActivityPolicy.Add(oCAPolicy);
        }

        //3. Encashment Interest Rate
        foreach (GridViewRow gvr in gvEncashmentIntRate.Rows)
        {
            EarlyEncashmentPolicy oEEPolicy = new EarlyEncashmentPolicy();
            oEEPolicy.SPPolicy.PolicyID = Util.GetIntNumber(hdPolicyID.Value);
            oEEPolicy.SlabNo = Util.GetIntNumber(gvr.Cells[0].Text);
            oEEPolicy.MonthFrom = Util.GetIntNumber(gvr.Cells[1].Text);
            oEEPolicy.MonthTo = Util.GetIntNumber(gvr.Cells[2].Text);
            oEEPolicy.InterestRate = Util.GetDecimalNumber(((TextBox)gvr.FindControl("bfInterestRate")).Text);
            oEEPolicy.NoOfSlabsIntPayable = Util.GetIntNumber(((TextBox)gvr.FindControl("bfNoOfSlabsIntPayable")).Text);

            oSPPolicy.EarlyEncashmentPolicy.Add(oEEPolicy);
        }

        // 4. General Interest Setup
        foreach (GridViewRow gvr in gvGeneralInt.Rows)
        {
            GeneralInterestPolicy oGIPolicy = new GeneralInterestPolicy();
            oGIPolicy.SPPolicy.PolicyID = Util.GetIntNumber(hdPolicyID.Value);
            oGIPolicy.SlabNo = Util.GetIntNumber(gvr.Cells[0].Text);
            oGIPolicy.MonthFrom = Util.GetIntNumber(gvr.Cells[1].Text);
            oGIPolicy.MonthTo = Util.GetIntNumber(gvr.Cells[2].Text);
            oGIPolicy.NonClaimRate = Util.GetDecimalNumber(((TextBox)gvr.FindControl("bfNonclaimIntRate")).Text);
            oGIPolicy.ClaimRate = Util.GetDecimalNumber(((TextBox)gvr.FindControl("bfClaimRate")).Text);

            oSPPolicy.GeneralInterestPolicy.Add(oGIPolicy);
        }

        // 5.0  Commission Setup
        oSPPolicy.NonOrgCommission = Util.GetDecimalNumber(txtComSetNonOrgComm.Text);
        oSPPolicy.NonOrgCommissionType = Util.GetRadioStringData(rblComSetNonOrgChargeOnPer) + Util.GetRadioStringData(rblComSetNonOrgCalculateInt);

        oSPPolicy.OrgCommission = Util.GetDecimalNumber(txtComSetOrgCommission.Text);
        oSPPolicy.OrgCommissionType = Util.GetRadioStringData(rblComSetOrgChargeOnPer) + Util.GetRadioStringData(rblComSetOrgCalculateInt);

        oSPPolicy.InterestRemuneration = Util.GetDecimalNumber(txtComSetIntRemuneration.Text);
        oSPPolicy.InterestRemunerationType = Util.GetRadioStringData(rblComSetIntRemuChargeOnPer) + Util.GetRadioStringData(rblComSetIntRemuCalculateInt);

        oSPPolicy.Remuneration = Util.GetDecimalNumber(txtComSetRemuneration.Text);
        oSPPolicy.RemunerationType = Util.GetRadioStringData(rblComSetRemuChargeOnPer) + Util.GetRadioStringData(rblComSetRemuCalculateInt);

        oSPPolicy.Levi = Util.GetDecimalNumber(txtComSetLevi.Text);
        oSPPolicy.LeviType = Util.GetRadioStringData(rblComSetLevi);

        oSPPolicy.IncomeTax = Util.GetDecimalNumber(txtComSetIncomeTax.Text);
        oSPPolicy.IncomeTaxType = Util.GetRadioStringData(rblComSetIncomeTax);
        oSPPolicy.IncomeTaxApplyAmount = Util.GetDecimalNumber(txtComSetIncomeTaxAbove.Text);
        oSPPolicy.IncomeTaxYearlyYN = chkYearly.Checked;

        oSPPolicy.SocialSecurityAmount = Util.GetDecimalNumber(txtSocialSecurityAmount.Text);
        oSPPolicy.SocialSecurityAmountType = Util.GetRadioStringData(rblSocialSecurityAmount);

        //6 start to set data Eligibility Setup
        //6.1 Payment Policy
        DataTable oDtEli = Session[SES_ELIGI_LIST] as DataTable;
        for (int i = 0; i < oDtEli.Rows.Count; i++)
        {
            PaymentPolicy oPP = new PaymentPolicy();
            oPP.SPPolicy.PolicyID = Util.GetIntNumber(hdPolicyID.Value);
            oPP.ActivityTypeID = Convert.ToString(oDtEli.Rows[i]["hdEliCustActiID"]); 
            oPP.PaymentModeID = Convert.ToString(oDtEli.Rows[i]["hdEliPayModeID"]); 
            oSPPolicy.PaymentPolicy.Add(oPP);
        }
        
        oSPPolicy.SupportedSex = Convert.ToInt32(ddlApplicableSex.SelectedValue);
        oSPPolicy.MinimumAge = Convert.ToInt32(txtMinimumAge.Text);
        oSPPolicy.MaximumAge = Convert.ToInt32(txtMaximumAge.Text);


        //6.2 Application Customer Type 
        foreach (GridViewRow gvr in gvCustomerType.Rows)
        {
            CustomerTypePolicy CTPolicy = null;
            if ((gvr.FindControl("chkCusomerType") as CheckBox).Checked)
            {
                CTPolicy = new CustomerTypePolicy();
                CTPolicy.SPPolicy.PolicyID = Util.GetIntNumber(hdPolicyID.Value);

                CTPolicy.CustomerType.CustomerTypeID = (gvr.FindControl("hdCustomerTypeID") as HiddenField).Value;
                oSPPolicy.CustomerTypePolicy.Add(CTPolicy);
            }
        }
        // end of Eligibility Setup



        oSPPolicy.UserDetails = ucUserDet.UserDetail;
        oSPPolicy.UserDetails.MakeDate = DateTime.Now;
        oSPPolicy.UserDetails.CheckerID = string.Empty;
        //oSPPolicy.UserDetails.CheckDate = null;
        oSPPolicy.UserDetails.CheckerComment = string.Empty;
        ucUserDet.ResetData();

        Result oResult = (Result)oSPPolicyDAL.Save(oSPPolicy);

        if (oResult.Status)
        {
            this.LoadList();
            ClearTextValue();
            hdPolicyID.Value = "";

            ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
        }
    }

    protected void btnAddNewSPPolicy_Click(object sender, EventArgs e)
    {        
        SPPolicyDAL oSPPolicyDAL = new SPPolicyDAL();
                
        Result oResultP = oSPPolicyDAL.IsPolicyExist(ddlSPType.SelectedItem.Value, SBM_BLC1.Common.Date.GetDateTimeByString(txtEffectiveDate.Text));

        if (oResultP.Status)
        {
            if (Convert.ToInt32(oResultP.Return) > 0)
            {
                ucMessage.OpenMessage("Already a Policy found on this date.", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                return;
            }
        }

        SPPolicy oSPPolicy = new SPPolicy();
        
        // common
        if (!string.IsNullOrEmpty(ddlSPType.SelectedItem.Value))
        {
            oSPPolicy.SPType.SPTypeID = ddlSPType.SelectedItem.Value;
        }
        oSPPolicy.PolicyEffectDate = SBM_BLC1.Common.Date.GetDateTimeByString(txtEffectiveDate.Text);
        oSPPolicy.PolicyID = -1;
        hdPolicyID.Value = "-1";

        // 1.0 General 
        oSPPolicy.IsSPDurationInMonth = Util.GetRadioData(rblIsSPDurationInMonth.SelectedItem.Value);
        oSPPolicy.SPDuration = Util.GetIntNumber(txtDuration.Text == "" ? "0" : txtDuration.Text);
        oSPPolicy.NoOfCoupons = Util.GetIntNumber(txtNoOfCoupon.Text == "" ? "0" : txtNoOfCoupon.Text);
        oSPPolicy.SPInterestType = Util.GetIntNumber(ddlIntrType.SelectedItem.Value == "" ? "0" : ddlIntrType.SelectedItem.Value);
        oSPPolicy.InterestTypeAfterIntPayment = Util.GetIntNumber(Util.GetRadioStringData(rblInterestTypeAfterIntPayment));
        if (ddlPreMatIntrType.SelectedItem != null)
        {
            oSPPolicy.PreMaturityInterestType = Util.GetIntNumber(ddlPreMatIntrType.SelectedItem.Value == "" ? "0" : ddlPreMatIntrType.SelectedItem.Value);
        }
        else
        {
            oSPPolicy.PreMaturityInterestType = 0;
        }
        oSPPolicy.PreMatIntTypeAfterIntPayment = Util.GetIntNumber(Util.GetRadioStringData(rblGSPreMatIntrClaim));
        oSPPolicy.IsBondHolderRequired = Util.GetRadioData(rblIsBondHolderRequired);
        oSPPolicy.IsNomineePerScripRequired = Util.GetRadioData(rblIsNomineePerScripRequired);
        oSPPolicy.IsFoeignAddressRequired = Util.GetRadioData(rblIsFoeignAddressRequired);

        oSPPolicy.ReinvestmentSuported = Util.GetRadioData(rblReinvestmentSuported);
        oSPPolicy.InterestReinvestable = Util.GetRadioData(rblInterestReinvestable);
        oSPPolicy.MaxNoOfReinvestment = Util.GetIntNumber(txtReinNumber.Text == "" ? "0" : txtReinNumber.Text);
        oSPPolicy.PartiallyEncashable = Util.GetRadioData(rblPartiallyEncashable);
        oSPPolicy.PartiallyEncashedReinvestable = Util.GetRadioData(rblPartiallyEncashedReinvestable);

        // 2.Activity Currency
        foreach (GridViewRow gvr in gvActiCurrency.Rows)
        {
            CurrencyActivityPolicy oCAPolicy = new CurrencyActivityPolicy();
            oCAPolicy.SPPolicy.PolicyID = Util.GetIntNumber(hdPolicyID.Value);

            oCAPolicy.ActivityTypeID = (gvr.FindControl("hdCustActiID") as HiddenField).Value;
            oCAPolicy.Currency.CurrencyID = (gvr.FindControl("hdCurrencyID") as HiddenField).Value;

            oSPPolicy.CurrencyActivityPolicy.Add(oCAPolicy);
        }

        //3. Encashment Interest Rate
        foreach (GridViewRow gvr in gvEncashmentIntRate.Rows)
        {
            EarlyEncashmentPolicy oEEPolicy = new EarlyEncashmentPolicy();
            oEEPolicy.SPPolicy.PolicyID = Util.GetIntNumber(hdPolicyID.Value);
            oEEPolicy.SlabNo = Util.GetIntNumber(gvr.Cells[0].Text);
            oEEPolicy.MonthFrom = Util.GetIntNumber(gvr.Cells[1].Text);
            oEEPolicy.MonthTo = Util.GetIntNumber(gvr.Cells[2].Text);
            oEEPolicy.InterestRate = Util.GetDecimalNumber(((TextBox)gvr.FindControl("bfInterestRate")).Text);
            oEEPolicy.NoOfSlabsIntPayable = Util.GetIntNumber(((TextBox)gvr.FindControl("bfNoOfSlabsIntPayable")).Text);

            oSPPolicy.EarlyEncashmentPolicy.Add(oEEPolicy);
        }

        // 4. General Interest Setup
        foreach (GridViewRow gvr in gvGeneralInt.Rows)
        {
            GeneralInterestPolicy oGIPolicy = new GeneralInterestPolicy();
            oGIPolicy.SPPolicy.PolicyID = Util.GetIntNumber(hdPolicyID.Value);
            oGIPolicy.SlabNo = Util.GetIntNumber(gvr.Cells[0].Text);
            oGIPolicy.MonthFrom = Util.GetIntNumber(gvr.Cells[1].Text);
            oGIPolicy.MonthTo = Util.GetIntNumber(gvr.Cells[2].Text);
            oGIPolicy.NonClaimRate = Util.GetDecimalNumber(((TextBox)gvr.FindControl("bfNonclaimIntRate")).Text);
            oGIPolicy.ClaimRate = Util.GetDecimalNumber(((TextBox)gvr.FindControl("bfClaimRate")).Text);

            oSPPolicy.GeneralInterestPolicy.Add(oGIPolicy);
        }

        // 5.0  Commission Setup
        oSPPolicy.NonOrgCommission = Util.GetDecimalNumber(txtComSetNonOrgComm.Text);
        oSPPolicy.NonOrgCommissionType = Util.GetRadioStringData(rblComSetNonOrgChargeOnPer) + Util.GetRadioStringData(rblComSetNonOrgCalculateInt);

        oSPPolicy.OrgCommission = Util.GetDecimalNumber(txtComSetOrgCommission.Text);
        oSPPolicy.OrgCommissionType = Util.GetRadioStringData(rblComSetOrgChargeOnPer) + Util.GetRadioStringData(rblComSetOrgCalculateInt);

        oSPPolicy.InterestRemuneration = Util.GetDecimalNumber(txtComSetIntRemuneration.Text);
        oSPPolicy.InterestRemunerationType = Util.GetRadioStringData(rblComSetIntRemuChargeOnPer) + Util.GetRadioStringData(rblComSetIntRemuCalculateInt);

        oSPPolicy.Remuneration = Util.GetDecimalNumber(txtComSetRemuneration.Text);
        oSPPolicy.RemunerationType = Util.GetRadioStringData(rblComSetRemuChargeOnPer) + Util.GetRadioStringData(rblComSetRemuCalculateInt);

        oSPPolicy.Levi = Util.GetDecimalNumber(txtComSetLevi.Text);
        oSPPolicy.LeviType = Util.GetRadioStringData(rblComSetLevi);

        oSPPolicy.IncomeTax = Util.GetDecimalNumber(txtComSetIncomeTax.Text);
        oSPPolicy.IncomeTaxType = Util.GetRadioStringData(rblComSetIncomeTax);
        oSPPolicy.IncomeTaxApplyAmount = Util.GetDecimalNumber(txtComSetIncomeTaxAbove.Text);
        oSPPolicy.IncomeTaxYearlyYN = chkYearly.Checked;

        oSPPolicy.SocialSecurityAmount = Util.GetDecimalNumber(txtSocialSecurityAmount.Text);
        oSPPolicy.SocialSecurityAmountType = Util.GetRadioStringData(rblSocialSecurityAmount);

        //6 start to set data Eligibility Setup
        //6.1 Payment Policy
        foreach (GridViewRow gvr in gvEliPaymentPolicy.Rows)
        {
            PaymentPolicy oPP = new PaymentPolicy();
            oPP.SPPolicy.PolicyID = Util.GetIntNumber(hdPolicyID.Value);
            oPP.ActivityTypeID = (gvr.FindControl("hdEliCustActiID") as HiddenField).Value;
            oPP.PaymentModeID = (gvr.FindControl("hdEliPayModeID") as HiddenField).Value;
            oSPPolicy.PaymentPolicy.Add(oPP);
        }
        oSPPolicy.SupportedSex = Convert.ToInt32(ddlApplicableSex.SelectedValue);
        oSPPolicy.MinimumAge = Convert.ToInt32(txtMinimumAge.Text);
        oSPPolicy.MaximumAge = Convert.ToInt32(txtMaximumAge.Text);


        //6.2 Application Customer Type 
        foreach (GridViewRow gvr in gvCustomerType.Rows)
        {
            CustomerTypePolicy CTPolicy = null;
            if ((gvr.FindControl("chkCusomerType") as CheckBox).Checked)
            {
                CTPolicy = new CustomerTypePolicy();
                CTPolicy.SPPolicy.PolicyID = Util.GetIntNumber(hdPolicyID.Value);

                CTPolicy.CustomerType.CustomerTypeID = (gvr.FindControl("hdCustomerTypeID") as HiddenField).Value;
                oSPPolicy.CustomerTypePolicy.Add(CTPolicy);
            }
        }
        // end of Eligibility Setup



        oSPPolicy.UserDetails = ucUserDet.UserDetail;
        oSPPolicy.UserDetails.MakeDate = DateTime.Now;
        oSPPolicy.UserDetails.CheckerID = "";
        oSPPolicy.UserDetails.CheckDate = DateTime.Now;
        oSPPolicy.UserDetails.CheckerComment = "";
        ucUserDet.ResetData();

        Result oResult = (Result)oSPPolicyDAL.Save(oSPPolicy);

        if (oResult.Status)
        {
            this.LoadList();
            ClearTextValue();
            hdPolicyID.Value = "";

            ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
        }
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {        
        ClearTextValue();        
    }

    protected void gvList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (!e.CommandName.Equals("Page"))
        {
            ClearTextValue();
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            hdDataType.Value = "T";
            LoadDataByID(Convert.ToInt32(gvRow.Cells[1].Text));                        
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

            if (gvList.Rows.Count > 0)
            {
                this.gvList.HeaderRow.Cells[1].Text = "Policy ID";
                this.gvList.HeaderRow.Cells[2].Text = "SP Type";
                this.gvList.HeaderRow.Cells[3].Text = "SP Duration";
                this.gvList.HeaderRow.Cells[4].Text = "No Of Coupons";
                this.gvList.HeaderRow.Cells[5].Text = "Minimum Age";
                this.gvList.HeaderRow.Cells[6].Text = "Maximum Age";
                this.gvList.HeaderRow.Cells[7].Text = "Is Approved";
                this.gvList.HeaderRow.Cells[8].Text = "Maker ID";
                this.gvList.HeaderRow.Cells[9].Text = "Maker Date";
            }
        }
    }

    protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Util.GridDateFormat(e, gvList, null);
    }
    #endregion Basic Operational Function from control EVENT

    protected void btnGeneralSaveInterest_Click(object sender, EventArgs e)
    {
        if (Session[SES_GENERAL_INT_LIST] != null)
        {
            DataTable oDtGeneralInt = (DataTable)Session[SES_GENERAL_INT_LIST];
            for (int i = 0; i < oDtGeneralInt.Rows.Count; i++)
            {
                oDtGeneralInt.Rows[i]["bfClaimRate"] = Util.GetDecimalNumber(txtGIClaimRate.Text);
                oDtGeneralInt.Rows[i]["bfNonclaimIntRate"] = Util.GetDecimalNumber(txtNonclaimIntRate.Text);
            }
            gvGeneralInt.DataSource = oDtGeneralInt;
            gvGeneralInt.DataBind();

            Session[SES_GENERAL_INT_LIST] = oDtGeneralInt;
        }
    }

    protected void btnSaveEarlyEncashment_Click(object sender, EventArgs e)
    {
        if (Session[SES_EARLY_ENCASH_LIST] != null)
        {
            DataTable oDtEarlyEncas = (DataTable)Session[SES_EARLY_ENCASH_LIST];
            for (int i = 0; i < oDtEarlyEncas.Rows.Count; i++)
            {
                oDtEarlyEncas.Rows[i]["bfInterestRate"] = txtCommonIntRate.Text;
            }
            gvEncashmentIntRate.DataSource = oDtEarlyEncas;
            gvEncashmentIntRate.DataBind();

            Session[SES_EARLY_ENCASH_LIST] = oDtEarlyEncas;
        }
    }

    protected void btnAddNewEligibility_Click(object sender, EventArgs e)
    {
        string sCustActi = ddlEligibilityCustomerActivityType.SelectedItem.Value;
        string sPayMode = ddlPaymentMode.SelectedItem.Value;
        DataTable oDtEli = null;
        if (Session[SES_ELIGI_LIST] != null)
        {
            oDtEli = (DataTable)Session[SES_ELIGI_LIST];
            for (int i = 0; i < oDtEli.Rows.Count; i++)
            {
                if (oDtEli.Rows[i]["hdEliPayModeID"].ToString() == sPayMode && oDtEli.Rows[i]["hdEliCustActiID"].ToString() == sCustActi)
                {
                    oDtEli.Rows[i].Delete();
                }
            }
        }
        else
        {
            // Eligibility grid..
            oDtEli = new DataTable("dtDataEli");
            oDtEli.Columns.Add(new DataColumn("bfCustActiType", typeof(string)));
            oDtEli.Columns.Add(new DataColumn("bfPayModeType", typeof(string)));
            oDtEli.Columns.Add(new DataColumn("hdEliPayModeID", typeof(string)));
            oDtEli.Columns.Add(new DataColumn("hdEliCustActiID", typeof(string)));
        }

        // adding new rows.. 
        DataRow rowEli = oDtEli.NewRow();
        rowEli["bfCustActiType"] = ddlEligibilityCustomerActivityType.SelectedItem.Text;
        rowEli["bfPayModeType"] = ddlPaymentMode.SelectedItem.Text;
        rowEli["hdEliPayModeID"] = sPayMode;
        rowEli["hdEliCustActiID"] = sCustActi;

        oDtEli.Rows.Add(rowEli);

        gvEliPaymentPolicy.DataSource = oDtEli;
        gvEliPaymentPolicy.DataBind();

        Session[SES_ELIGI_LIST] = oDtEli;

    }

    protected void gvEliPaymentPolicy_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
        string sPayModeID = ((HiddenField)(gvRow.FindControl("hdEliPayModeID"))).Value;
        string sCustID = ((HiddenField)(gvRow.FindControl("hdEliCustActiID"))).Value;

        if (Session[SES_ELIGI_LIST] != null)
        {
            DataTable oDtEli = (DataTable)Session[SES_ELIGI_LIST];
            for (int i = 0; i < oDtEli.Rows.Count; i++)
            {
                if (oDtEli.Rows[i]["hdEliPayModeID"].ToString() == sPayModeID && oDtEli.Rows[i]["hdEliCustActiID"].ToString() == sCustID)
                {
                    oDtEli.Rows[ i ].Delete();
                }
            }
            gvEliPaymentPolicy.DataSource = oDtEli;
            gvEliPaymentPolicy.DataBind();

            Session[SES_ELIGI_LIST] = oDtEli;
        }
    }


    protected void btnAddNewActiCurrency_Click(object sender, EventArgs e)
    {
        string sActiType = ddlCurrencyActiveType.SelectedItem.Value;
        string sCurrency = ddlCurrency.SelectedItem.Value;
        DataTable oDtCurrency = null;
        if (Session[SES_ACTI_CURR_LIST] != null)
        {
            oDtCurrency = (DataTable)Session[SES_ACTI_CURR_LIST];
            for (int i = 0; i < oDtCurrency.Rows.Count; i++)
            {
                if (oDtCurrency.Rows[i]["hdCurrencyID"].ToString() == sCurrency && oDtCurrency.Rows[i]["hdCustActiID"].ToString() == sActiType)
                {
                    oDtCurrency.Rows[i].Delete();
                }
            }
        }
        else
        {
            // 2.0 Currency Setup
            oDtCurrency = new DataTable("dtDataCurrency");
            oDtCurrency.Columns.Add(new DataColumn("bfActivityType", typeof(string)));
            oDtCurrency.Columns.Add(new DataColumn("bfCurrency", typeof(string)));
            oDtCurrency.Columns.Add(new DataColumn("hdCurrencyID", typeof(string)));
            oDtCurrency.Columns.Add(new DataColumn("hdCustActiID", typeof(string)));
        }
            // adding new rows.. 
            DataRow rowCurr = oDtCurrency.NewRow();

            rowCurr["bfActivityType"] = ddlCurrencyActiveType.SelectedItem.Text;
            rowCurr["bfCurrency"] = ddlCurrency.SelectedItem.Text;
            rowCurr["hdCurrencyID"] = sCurrency;
            rowCurr["hdCustActiID"] = sActiType;

            oDtCurrency.Rows.Add(rowCurr);

            gvActiCurrency.DataSource = oDtCurrency;
            gvActiCurrency.DataBind();

            Session[SES_ACTI_CURR_LIST] = oDtCurrency;
        
    }
    protected void gvActiCurrency_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
        string sCurrencyID = ((HiddenField)(gvRow.FindControl("hdCurrencyID"))).Value;
        string sCustActiID = ((HiddenField)(gvRow.FindControl("hdCustActiID"))).Value;

        if (Session[SES_ACTI_CURR_LIST] != null)
        {
            DataTable oDtCurr = (DataTable)Session[SES_ACTI_CURR_LIST];
            for (int i = 0; i < oDtCurr.Rows.Count; i++)
            {
                if (oDtCurr.Rows[i]["hdCustActiID"].ToString() == sCustActiID && oDtCurr.Rows[i]["hdCurrencyID"].ToString() == sCurrencyID)
                {
                    oDtCurr.Rows[i].Delete();
                }
            }
            gvActiCurrency.DataSource = oDtCurr;
            gvActiCurrency.DataBind();

            Session[SES_ACTI_CURR_LIST] = oDtCurr;
        }
    }

    

    #region Supporting or Utility function

    public void ClearTextValue()
    {
       
        //Clear session
        Session[SES_ELIGI_LIST] = null;
        Session[SES_ACTI_CURR_LIST] = null;
        Session[SES_EARLY_ENCASH_LIST] = null;
        Session[SES_GENERAL_INT_LIST] = null;
        // common
        txtEffectiveDate.Text=string.Empty;
        ddlSPType.SelectedIndex = 0;
        ddlSPType.Enabled = true;
        // general Control
        Util.ClearData(rblIsSPDurationInMonth);
        txtDuration.Text = string.Empty;
        txtNoOfCoupon.Text = string.Empty;
        ddlIntrType.SelectedIndex = 0;
        Util.ClearData(rblInterestTypeAfterIntPayment);
        ddlPreMatIntrType.SelectedIndex = 0;
        Util.ClearData(rblGSPreMatIntrClaim);

        Util.ClearData(rblIsBondHolderRequired);
        Util.ClearData(rblIsNomineePerScripRequired);
        Util.ClearData(rblIsFoeignAddressRequired);

        Util.ClearData(rblReinvestmentSuported);
        Util.ClearData(rblInterestReinvestable);
        txtReinNumber.Text = string.Empty;
        Util.ClearData(rblPartiallyEncashable);
        Util.ClearData(rblPartiallyEncashedReinvestable);

        // Currency Setup
        ddlCurrencyActiveType.SelectedIndex=0;
        ddlCurrency.SelectedIndex = 0;
        gvActiCurrency.DataSource = null;
        gvActiCurrency.DataBind();


        //Early Encashment Setup
        txtEarlyEncashCouponNo.Text=string.Empty;
        txtCommonIntRate.Text = string.Empty;
        chkMaturedCoupon.Checked = false;
        gvEncashmentIntRate.DataSource = null;
        gvEncashmentIntRate.DataBind();

        // General Interest Setup
        txtGeneralIntCouponNo.Text = string.Empty;
        txtGIClaimRate.Text = string.Empty;
        txtNonclaimIntRate.Text = string.Empty;
        gvGeneralInt.DataSource = null;
        gvGeneralInt.DataBind();


        // Commission Setup
        txtComSetNonOrgComm.Text=string.Empty;
        Util.ClearData(rblComSetNonOrgChargeOnPer);
        Util.ClearData(rblComSetNonOrgCalculateInt);
        txtComSetOrgCommission.Text = string.Empty;
        Util.ClearData(rblComSetOrgChargeOnPer);
        Util.ClearData(rblComSetOrgCalculateInt);
        txtComSetIntRemuneration.Text = string.Empty;
        Util.ClearData(rblComSetIntRemuChargeOnPer);
        Util.ClearData(rblComSetIntRemuCalculateInt);
        txtComSetRemuneration.Text = string.Empty;
        Util.ClearData(rblComSetRemuChargeOnPer);
        Util.ClearData(rblComSetRemuCalculateInt);
        txtComSetLevi.Text = string.Empty;
        Util.ClearData(rblComSetLevi);
        txtComSetIncomeTax.Text =string.Empty;
        Util.ClearData(rblComSetIncomeTax);
        txtSocialSecurityAmount.Text = string.Empty;
        Util.ClearData(rblSocialSecurityAmount);
        txtComSetIncomeTaxAbove.Text = string.Empty;
        chkYearly.Checked = false;
        Util.ChkChangeSetColor(chkYearly);
        // Eligibility Setup
        ddlEligibilityCustomerActivityType.SelectedIndex = 0;
        txtMinimumAge.Text = string.Empty;
        txtMaximumAge.Text = string.Empty;
        ddlApplicableSex.SelectedIndex = 0;
        ddlPaymentMode.SelectedIndex = 0;

        gvEliPaymentPolicy.DataSource = null;
        gvEliPaymentPolicy.DataBind();

        GridViewRowCollection gvRows = (GridViewRowCollection)gvCustomerType.Rows;
        foreach (GridViewRow row in gvRows)
        {
            CheckBox oCheckBox = (CheckBox)row.FindControl("chkCusomerType");
            oCheckBox.Checked = false;
        }
        hdPolicyID.Value = string.Empty;
        hdDataType.Value = string.Empty;
        hdPolEffDate.Value = string.Empty;
       
        ucUserDet.ResetData();
    }

    public void LoadList()
    {
        Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
        if (oConfig != null)
        {
            SPPolicy oSPPolicy = new SPPolicy();
            SPPolicyDAL SPPolicyDAL = new SPPolicyDAL();
            Result oResult = SPPolicyDAL.LoadUnapprovedList(oConfig.UserName, false);

            if (oResult.Status)
            {
                DataTable dtTmpList = (DataTable)oResult.Return;
                if (dtTmpList.Rows.Count > 0)
                {
                    dtTmpList.Columns.Remove("CheckerID");
                    dtTmpList.Columns.Remove("CheckDate");
                    dtTmpList.Columns.Remove("CheckerComment");
                    dtTmpList.Columns.Remove("PolicyEffectDate");
                    dtTmpList.Columns.Remove("IsSPDurationInMonth");
                    dtTmpList.Columns.Remove("SPInterestType");
                    dtTmpList.Columns.Remove("InterestTypeAfterIntPayment");
                    dtTmpList.Columns.Remove("PreMaturityInterestType");
                    dtTmpList.Columns.Remove("PreMatIntTypeAfterIntPayment");
                    dtTmpList.Columns.Remove("IsNomineePerScripRequired");
                    dtTmpList.Columns.Remove("IsBondHolderRequired");
                    dtTmpList.Columns.Remove("IsFoeignAddressRequired");
                    dtTmpList.Columns.Remove("SupportedSex");
                    dtTmpList.Columns.Remove("PartiallyEncashable");
                    dtTmpList.Columns.Remove("ReinvestmentSuported");
                    dtTmpList.Columns.Remove("InterestReinvestable");
                    dtTmpList.Columns.Remove("PartiallyEncashedReinvestable");
                    dtTmpList.Columns.Remove("MaxNoOfReinvestment");
                    dtTmpList.Columns.Remove("NonOrgCommission");
                    dtTmpList.Columns.Remove("NonOrgCommissionType");
                    dtTmpList.Columns.Remove("OrgCommission");
                    dtTmpList.Columns.Remove("OrgCommissionType");
                    dtTmpList.Columns.Remove("Levi");
                    dtTmpList.Columns.Remove("LeviType");
                    dtTmpList.Columns.Remove("IncomeTax");
                    dtTmpList.Columns.Remove("IncomeTaxType");
                    dtTmpList.Columns.Remove("SocialSecurityAmount");
                    dtTmpList.Columns.Remove("SocialSecurityAmountType");
                    dtTmpList.Columns.Remove("IncomeTaxApplyAmount");
                    dtTmpList.Columns.Remove("IncomeTaxYearlyYN");
                    dtTmpList.Columns.Remove("IsOrganizationLeviTax");
                    dtTmpList.Columns.Remove("InterestRemuneration");
                    dtTmpList.Columns.Remove("InterestRemunerationType");
                    dtTmpList.Columns.Remove("Remuneration");
                    dtTmpList.Columns.Remove("RemunerationType");

                    gvList.DataSource = dtTmpList;
                    gvList.DataBind();
                    
                    Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;

                    if (gvList.Rows.Count > 0)
                    {
                        this.gvList.HeaderRow.Cells[1].Text = "Policy ID";
                        this.gvList.HeaderRow.Cells[2].Text = "SP Type";
                        this.gvList.HeaderRow.Cells[3].Text = "SP Duration";
                        this.gvList.HeaderRow.Cells[4].Text = "No Of Coupons";
                        this.gvList.HeaderRow.Cells[5].Text = "Minimum Age";
                        this.gvList.HeaderRow.Cells[6].Text = "Maximum Age";
                        this.gvList.HeaderRow.Cells[7].Text = "Is Approved";
                        this.gvList.HeaderRow.Cells[8].Text = "Maker ID";
                        this.gvList.HeaderRow.Cells[9].Text = "Make Date";
                    }
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

    private void SetPolicy(SPPolicy oSPPolicy)
    {
        if (oSPPolicy != null)
        {
            #region common
            DDListUtil.Assign(ddlSPType, oSPPolicy.SPType.SPTypeID);
            ddlSPType.Enabled = false;
            txtEffectiveDate.Text = oSPPolicy.PolicyEffectDate.ToString(Constants.DATETIME_FORMAT);
            #endregion common

            #region 1.0 General
            Util.SetRadioData(rblIsSPDurationInMonth, oSPPolicy.IsSPDurationInMonth);
            Util.RBLChangeSetColor(rblIsSPDurationInMonth);
            txtDuration.Text = oSPPolicy.SPDuration.ToString();
            txtNoOfCoupon.Text = oSPPolicy.NoOfCoupons.ToString();

            DDListUtil.Assign(ddlIntrType, oSPPolicy.SPInterestType);
            Util.SetRadioData(rblInterestTypeAfterIntPayment, oSPPolicy.InterestTypeAfterIntPayment);
            Util.RBLChangeSetColor(rblInterestTypeAfterIntPayment);
            DDListUtil.Assign(ddlPreMatIntrType, oSPPolicy.PreMaturityInterestType);
            Util.SetRadioData(rblGSPreMatIntrClaim, oSPPolicy.PreMatIntTypeAfterIntPayment);
            Util.RBLChangeSetColor(rblGSPreMatIntrClaim);

            Util.SetRadioData(rblIsBondHolderRequired, oSPPolicy.IsBondHolderRequired);
            Util.RBLChangeSetColor(rblIsBondHolderRequired);
            Util.SetRadioData(rblIsNomineePerScripRequired, oSPPolicy.IsNomineePerScripRequired);
            Util.RBLChangeSetColor(rblIsNomineePerScripRequired);
            Util.SetRadioData(rblIsFoeignAddressRequired, oSPPolicy.IsFoeignAddressRequired);
            Util.RBLChangeSetColor(rblIsFoeignAddressRequired);

            Util.SetRadioData(rblReinvestmentSuported, oSPPolicy.ReinvestmentSuported);
            Util.RBLChangeSetColor(rblReinvestmentSuported);
            Util.SetRadioData(rblInterestReinvestable, oSPPolicy.InterestReinvestable);
            Util.RBLChangeSetColor(rblInterestReinvestable);
            txtReinNumber.Text = oSPPolicy.MaxNoOfReinvestment.ToString();
            Util.SetRadioData(rblPartiallyEncashable, oSPPolicy.PartiallyEncashable);
            Util.RBLChangeSetColor(rblPartiallyEncashable);
            Util.SetRadioData(rblPartiallyEncashedReinvestable, oSPPolicy.PartiallyEncashedReinvestable);
            Util.RBLChangeSetColor(rblPartiallyEncashedReinvestable);
            #endregion 1.0 General

            #region 2.0 Currency Setup
            DataTable oDtCurr = new DataTable("dtDataCurrency");
            oDtCurr.Columns.Add(new DataColumn("bfActivityType", typeof(string)));
            oDtCurr.Columns.Add(new DataColumn("bfCurrency", typeof(string)));
            oDtCurr.Columns.Add(new DataColumn("hdCurrencyID", typeof(string)));
            oDtCurr.Columns.Add(new DataColumn("hdCustActiID", typeof(string)));
            DataRow rowCurr = null;

            for (int i = 0; i < oSPPolicy.CurrencyActivityPolicy.Count; i++)
            {
                rowCurr = oDtCurr.NewRow();
                rowCurr["bfActivityType"] = oSPPolicy.CurrencyActivityPolicy[i].ActivityTypeID + " : " + oSPPolicy.CurrencyActivityPolicy[i].ActivityTypeValue;
                rowCurr["bfCurrency"] = oSPPolicy.CurrencyActivityPolicy[i].Currency.CurrencyID + " : " + oSPPolicy.CurrencyActivityPolicy[i].Currency.CurrencyCode;
                rowCurr["hdCurrencyID"] = oSPPolicy.CurrencyActivityPolicy[i].Currency.CurrencyID;
                rowCurr["hdCustActiID"] = oSPPolicy.CurrencyActivityPolicy[i].ActivityTypeID;

                oDtCurr.Rows.Add(rowCurr);
            }
            gvActiCurrency.DataSource = oDtCurr;
            gvActiCurrency.DataBind();
            Session[SES_ACTI_CURR_LIST] = oDtCurr;
            #endregion 2.0 Currency Setup

            #region 3.0 Early Encashment Setup
            txtEarlyEncashCouponNo.Text = oSPPolicy.NoOfCoupons.ToString();
            DataTable oDtEE = new DataTable("dtDataEE");

            oDtEE.Columns.Add(new DataColumn("bfCouponInstallmentNo", typeof(string)));
            oDtEE.Columns.Add(new DataColumn("bfMonthFrom", typeof(string)));
            oDtEE.Columns.Add(new DataColumn("bfMonthTo", typeof(string)));
            oDtEE.Columns.Add(new DataColumn("bfInterestRate", typeof(string)));
            oDtEE.Columns.Add(new DataColumn("bfNoOfSlabsIntPayable", typeof(string)));

            oDtEE.Columns.Add(new DataColumn("hdPolicyID", typeof(string)));
            oDtEE.Columns.Add(new DataColumn("hdSlabNoID", typeof(string)));
            DataRow rowEE = null;

            for (int i = 0; i < oSPPolicy.EarlyEncashmentPolicy.Count; i++)
            {
                rowEE = oDtEE.NewRow();
                rowEE["bfCouponInstallmentNo"] = oSPPolicy.EarlyEncashmentPolicy[i].SlabNo.ToString();
                rowEE["bfMonthFrom"] = oSPPolicy.EarlyEncashmentPolicy[i].MonthFrom.ToString();
                rowEE["bfMonthTo"] = oSPPolicy.EarlyEncashmentPolicy[i].MonthTo.ToString();
                rowEE["bfInterestRate"] = oSPPolicy.EarlyEncashmentPolicy[i].InterestRate.ToString();
                rowEE["bfNoOfSlabsIntPayable"] = oSPPolicy.EarlyEncashmentPolicy[i].NoOfSlabsIntPayable.ToString();

                rowEE["hdPolicyID"] = oSPPolicy.PolicyID.ToString().Trim();
                rowEE["hdSlabNoID"] = oSPPolicy.EarlyEncashmentPolicy[i].SlabNo.ToString();

                oDtEE.Rows.Add(rowEE);
            }
            gvEncashmentIntRate.DataSource = oDtEE;
            gvEncashmentIntRate.DataBind();
            Session[SES_EARLY_ENCASH_LIST] = oDtEE;
            #endregion 3.0 Early Encashment Setup

            #region 4.0 General Interest Setup
            txtGeneralIntCouponNo.Text = oSPPolicy.NoOfCoupons.ToString();

            DataTable oDtGI = new DataTable("dtDataG");
            oDtGI.Columns.Add(new DataColumn("bfCouponInstallmentNo", typeof(string)));
            oDtGI.Columns.Add(new DataColumn("bfMonthFrom", typeof(string)));
            oDtGI.Columns.Add(new DataColumn("bfMonthTo", typeof(string)));
            oDtGI.Columns.Add(new DataColumn("bfClaimRate", typeof(string)));
            oDtGI.Columns.Add(new DataColumn("bfNonclaimIntRate", typeof(string)));

            oDtGI.Columns.Add(new DataColumn("hdPolicyID", typeof(string)));
            oDtGI.Columns.Add(new DataColumn("hdSlabNoID", typeof(string)));
            DataRow rowGI = null;

            for (int i = 0; i < oSPPolicy.GeneralInterestPolicy.Count; i++)
            {
                rowGI = oDtGI.NewRow();
                rowGI["bfCouponInstallmentNo"] = oSPPolicy.GeneralInterestPolicy[i].SlabNo.ToString();
                rowGI["bfMonthFrom"] = oSPPolicy.GeneralInterestPolicy[i].MonthFrom.ToString();
                rowGI["bfMonthTo"] = oSPPolicy.GeneralInterestPolicy[i].MonthTo.ToString();
                rowGI["bfClaimRate"] = oSPPolicy.GeneralInterestPolicy[i].ClaimRate.ToString();
                rowGI["bfNonclaimIntRate"] = oSPPolicy.GeneralInterestPolicy[i].NonClaimRate.ToString();

                rowGI["hdPolicyID"] = oSPPolicy.PolicyID.ToString().Trim();
                rowGI["hdSlabNoID"] = oSPPolicy.GeneralInterestPolicy[i].SlabNo.ToString();

                oDtGI.Rows.Add(rowGI);
            }
            gvGeneralInt.DataSource = oDtGI;
            gvGeneralInt.DataBind();
            Session[SES_GENERAL_INT_LIST] = oDtGI;
            #endregion 4.0 General Interest Setup

            #region 5.0  Commission Setup
            txtComSetNonOrgComm.Text = oSPPolicy.NonOrgCommission.ToString();
            Util.SetRadioData(rblComSetNonOrgChargeOnPer, oSPPolicy.NonOrgCommissionType.Substring(0, 1));
            Util.RBLChangeSetColor(rblComSetNonOrgChargeOnPer);
            Util.SetRadioData(rblComSetNonOrgCalculateInt, oSPPolicy.NonOrgCommissionType.Substring(1, 1));
            Util.RBLChangeSetColor(rblComSetNonOrgCalculateInt);
            txtComSetOrgCommission.Text = oSPPolicy.OrgCommission.ToString();
            Util.SetRadioData(rblComSetOrgChargeOnPer, oSPPolicy.OrgCommissionType.Substring(0, 1));
            Util.RBLChangeSetColor(rblComSetOrgChargeOnPer);
            Util.SetRadioData(rblComSetOrgCalculateInt, oSPPolicy.OrgCommissionType.Substring(1, 1));
            Util.RBLChangeSetColor(rblComSetOrgCalculateInt);
            txtComSetIntRemuneration.Text = oSPPolicy.InterestRemuneration.ToString();
            Util.SetRadioData(rblComSetIntRemuChargeOnPer, oSPPolicy.InterestRemunerationType.Substring(0, 1));
            Util.RBLChangeSetColor(rblComSetIntRemuChargeOnPer);
            Util.SetRadioData(rblComSetIntRemuCalculateInt, oSPPolicy.InterestRemunerationType.Substring(1, 1));
            Util.RBLChangeSetColor(rblComSetIntRemuCalculateInt);
            txtComSetRemuneration.Text = oSPPolicy.Remuneration.ToString();
            Util.SetRadioData(rblComSetRemuChargeOnPer, oSPPolicy.RemunerationType.Substring(0, 1));
            Util.RBLChangeSetColor(rblComSetRemuChargeOnPer);
            Util.SetRadioData(rblComSetRemuCalculateInt, oSPPolicy.RemunerationType.Substring(1, 1));
            Util.RBLChangeSetColor(rblComSetRemuCalculateInt);
            txtComSetLevi.Text = oSPPolicy.Levi.ToString();
            Util.SetRadioData(rblComSetLevi, oSPPolicy.LeviType);
            Util.RBLChangeSetColor(rblComSetLevi);
            txtComSetIncomeTax.Text = oSPPolicy.IncomeTax.ToString();
            Util.SetRadioData(rblComSetIncomeTax, oSPPolicy.IncomeTaxType.ToString());
            Util.RBLChangeSetColor(rblComSetIncomeTax);
            txtSocialSecurityAmount.Text = oSPPolicy.SocialSecurityAmount.ToString();
            Util.SetRadioData(rblSocialSecurityAmount, oSPPolicy.SocialSecurityAmountType.ToString());
            Util.RBLChangeSetColor(rblSocialSecurityAmount);
            txtComSetIncomeTaxAbove.Text = oSPPolicy.IncomeTaxApplyAmount.ToString();
            Util.SetCheckData(chkYearly, oSPPolicy.IncomeTaxYearlyYN);
            Util.ChkChangeSetColor(chkYearly);
            #endregion 5.0  Commission Setup

            #region 6.0  Eligibility
            ArrayList alCTPID = new ArrayList();
            if (oSPPolicy.CustomerTypePolicy.Count > 0)
            {
                int i = 0;
                foreach (CustomerTypePolicy oCTP in oSPPolicy.CustomerTypePolicy)
                {
                    alCTPID.Insert(i, oCTP.CustomerType.CustomerTypeID);
                    i++;
                }
            }

            GridViewRowCollection gvRows = (GridViewRowCollection)gvCustomerType.Rows;
            foreach (GridViewRow row in gvRows)
            {
                CheckBox oCheckBox = (CheckBox)row.FindControl("chkCusomerType");

                if (alCTPID.Contains(row.Cells[1].Text))
                {
                    oCheckBox.Checked = true;
                }
                else
                {
                    oCheckBox.Checked = false;
                }
            }
            // Eligibility grid..
            DataTable oDtEli = new DataTable("dtDataEli");
            oDtEli.Columns.Add(new DataColumn("bfCustActiType", typeof(string)));
            oDtEli.Columns.Add(new DataColumn("bfPayModeType", typeof(string)));
            oDtEli.Columns.Add(new DataColumn("hdEliPayModeID", typeof(string)));
            oDtEli.Columns.Add(new DataColumn("hdEliCustActiID", typeof(string)));
            DataRow rowEli = null;

            for (int i = 0; i < oSPPolicy.PaymentPolicy.Count; i++)
            {
                rowEli = oDtEli.NewRow();
                rowEli["bfCustActiType"] = oSPPolicy.PaymentPolicy[i].ActivityTypeID + " : " + oSPPolicy.PaymentPolicy[i].ActivityTypeValue;
                rowEli["bfPayModeType"] = oSPPolicy.PaymentPolicy[i].PaymentModeID + " : " + oSPPolicy.PaymentPolicy[i].PaymentModeValue;
                rowEli["hdEliPayModeID"] = oSPPolicy.PaymentPolicy[i].PaymentModeID;
                rowEli["hdEliCustActiID"] = oSPPolicy.PaymentPolicy[i].ActivityTypeID;

                oDtEli.Rows.Add(rowEli);
            }
            gvEliPaymentPolicy.DataSource = oDtEli;
            gvEliPaymentPolicy.DataBind();
            Session.Add(SES_ELIGI_LIST, oDtEli);

            // end of Eligibility grid..
            DDListUtil.Assign(ddlApplicableSex, oSPPolicy.SupportedSex);
            txtMinimumAge.Text = oSPPolicy.MinimumAge.ToString();
            txtMaximumAge.Text = oSPPolicy.MaximumAge.ToString();
            #endregion 6.0  Eligibility
            
            if (string.IsNullOrEmpty(hdDataType.Value))
            {                
                //When Loading from Approver End
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.MakerID = oSPPolicy.UserDetails.MakerID;
                userDetails.MakeDate = oSPPolicy.UserDetails.MakeDate;
                ucUserDet.UserDetail = userDetails;
            }
            else if (hdDataType.Value.Equals("T"))
            {
                //When loading from temp table
                UserDetails userDetails = ucUserDet.UserDetail;
                userDetails.CheckerID = oSPPolicy.UserDetails.CheckerID;
                userDetails.CheckDate = oSPPolicy.UserDetails.CheckDate;
                userDetails.CheckerComment = oSPPolicy.UserDetails.CheckerComment;
                ucUserDet.UserDetail = userDetails;
            }

            hdPolicyID.Value = oSPPolicy.PolicyID.ToString();
            hdPolEffDate.Value = oSPPolicy.PolicyEffectDate.ToString(Constants.DATETIME_FORMAT);
            SetRadioEffect();
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
        }
    }

    private void LoadDataByID(int sPolicyID)
    {
        if (sPolicyID != 0)
        {
            SPPolicy oSPPolicy = new SPPolicy(sPolicyID);
            SPPolicyDAL oSPPolicyDAL = new SPPolicyDAL();
            Result oResult = new Result();

            oResult = oSPPolicyDAL.LoadByID(oSPPolicy);
            
            if (oResult.Status)
            {
                oSPPolicy = (SPPolicy)oResult.Return;
                SetPolicy(oSPPolicy);
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

    public void SearchPolicyLoadAction( string sSPType , string sDate)
    {
        ClearTextValue();

        string[] spTmp = sSPType.Split(':');
        string spTmpID = spTmp[0];
        
        hdDataType.Value = "M";

        SPPolicyDAL spDal = new SPPolicyDAL();
        Result oResult = spDal.GetLatestPolicyDetail(spTmpID.Trim(), Constants.ACTIVITY_TYPE.ISSUE, Util.GetDateTimeByString(sDate));
        if (oResult.Status)
        {
            SPPolicy oSPPolicy = new SPPolicy();
            oSPPolicy = (SPPolicy)oResult.Return;
            SetPolicy(oSPPolicy);            
        }
        else
        {
            ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
        }
    }

    public int TotalMonth()
    {
        int iTotalMonth = 0;
        if (rblIsSPDurationInMonth.SelectedItem != null)
        {
            if (rblIsSPDurationInMonth.SelectedItem.Value == "1")
            {
                iTotalMonth = Util.GetIntNumber(txtDuration.Text);
            }
            else if (rblIsSPDurationInMonth.SelectedItem.Value == "0")
            {
                iTotalMonth = 12 * Util.GetIntNumber(txtDuration.Text);
            }
        }
        return iTotalMonth;
    }

    public void SetRadioEffect()
    {
        Util.RBLChangeColor(rblIsSPDurationInMonth);
        Util.RBLChangeColor(rblInterestTypeAfterIntPayment);
        Util.RBLChangeColor(rblGSPreMatIntrClaim);
        Util.RBLChangeColor(rblIsBondHolderRequired);
        Util.RBLChangeColor(rblIsNomineePerScripRequired);
        Util.RBLChangeColor(rblIsFoeignAddressRequired);
        Util.RBLChangeColor(rblReinvestmentSuported);
        Util.RBLChangeColor(rblInterestReinvestable);
        Util.RBLChangeColor(rblPartiallyEncashable);
        Util.RBLChangeColor(rblPartiallyEncashedReinvestable);
        Util.RBLChangeColor(rblComSetNonOrgChargeOnPer);
        Util.RBLChangeColor(rblComSetNonOrgCalculateInt);
        Util.RBLChangeColor(rblComSetOrgChargeOnPer);
        Util.RBLChangeColor(rblComSetOrgCalculateInt);
        Util.RBLChangeColor(rblComSetIntRemuChargeOnPer);
        Util.RBLChangeColor(rblComSetIntRemuCalculateInt);
        Util.RBLChangeColor(rblComSetRemuChargeOnPer);
        Util.RBLChangeColor(rblComSetRemuCalculateInt);
        Util.RBLChangeColor(rblComSetLevi);
        Util.RBLChangeColor(rblComSetIncomeTax);
        Util.RBLChangeColor(rblSocialSecurityAmount);


        Util.RBLChangeSetColor(rblComSetNonOrgChargeOnPer);
        Util.RBLChangeSetColor(rblComSetNonOrgCalculateInt);
        Util.RBLChangeSetColor(rblComSetOrgChargeOnPer);
        Util.RBLChangeSetColor(rblComSetOrgCalculateInt);
        Util.RBLChangeSetColor(rblComSetIntRemuCalculateInt);
        Util.RBLChangeSetColor(rblComSetIntRemuChargeOnPer);
        Util.RBLChangeSetColor(rblComSetRemuChargeOnPer);
        Util.RBLChangeSetColor(rblComSetRemuCalculateInt);
        Util.RBLChangeSetColor(rblComSetLevi);
        Util.RBLChangeSetColor(rblComSetIncomeTax);
        Util.ChkChangeSetColor(chkYearly);
        Util.ChkChangeSetColor(chkMaturedCoupon);
        Util.RBLChangeSetColor(rblPartiallyEncashedReinvestable);
        Util.RBLChangeSetColor(rblPartiallyEncashable);
        Util.RBLChangeSetColor(rblInterestReinvestable);
        Util.RBLChangeSetColor(rblReinvestmentSuported);
        Util.RBLChangeSetColor(rblIsFoeignAddressRequired);
        Util.RBLChangeSetColor(rblIsNomineePerScripRequired);
        Util.RBLChangeSetColor(rblIsBondHolderRequired);
        Util.RBLChangeSetColor(rblGSPreMatIntrClaim);
        Util.RBLChangeSetColor(rblInterestTypeAfterIntPayment);
        Util.RBLChangeSetColor(rblIsSPDurationInMonth);
        Util.RBLChangeSetColor(rblSocialSecurityAmount);
    }

    #endregion Supporting or Utility function    
}