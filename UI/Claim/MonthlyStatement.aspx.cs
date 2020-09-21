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
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Configuration;
using SBM_BLC1.Entity.Common;

namespace SBM_WebUI.mp
{
    public partial class MonthlyStatement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Constants.SES_USER_CONFIG] != null)
            {
                if (!Page.IsPostBack)
                {
                    InitializeData();
                    //This is for Page Permission
                    CheckPermission chkPer = new CheckPermission();
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_CLAIM.MONTHLY_STATEMENT))
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
            DDListUtil.LoadDDLFromDB(ddlSpType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            txtDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
        }
        #endregion InitializeData

        protected void ddlSpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlSpType.SelectedValue))
            {
                LoadBySPType();
            }
            else
            {
                ddlCurrency.Items.Clear();
            }
        }

        private void LoadBySPType()
        {
            SPPolicy oSPPolicy = null;
            SPPolicyDAL oSPPolicyDAL = new SPPolicyDAL();
            Result oResult = (Result)oSPPolicyDAL.LatestPolicy(ddlSpType.SelectedValue, Constants.ACTIVITY_TYPE.ISSUE, DateTime.Now);
            if (oResult.Status)
            {
                ddlCurrency.Items.Clear();

                oSPPolicy = (SPPolicy)oResult.Return;
                DDListUtil.Assign(ddlCurrency, oSPPolicy.DTCurrencyActivityPolicy, true);
            }
        }
        

        #region Event
        protected void btnPreview_Click(object sender, EventArgs e)
        {

        }
        protected void btnSaveAndPreview_Click(object sender, EventArgs e)
        {

        }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtConfirmPassword.Text = "";
            ddlSpType.SelectedIndex = 0;
            ddlCurrency.Items.Clear();
        }
        #endregion Event
    }
}
