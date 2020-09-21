using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.Configuration;
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Entity.Transaction;
using SBM_BLC1.Transaction;
using System.Collections;
using System.Text;
using SBM_BLC1.DAL.Report;
using SBM_BLC1.DAL.Utility;
using SBM_BLC1.Entity;
using SBM_BLC1.Entity.Utility;

namespace SBM_WebUI.mp
{
    public partial class BDDBEntry : System.Web.UI.Page
    {
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_UTILITY.AUTHORIZE_DELETE))
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

        private void InitializeData()
        {                                    
           // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlPDCurrencyCode, "CurrencyID", "CurrencyCode", "SPMS_Currency", true);
        }

                
        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearControlValues();

        }        


        public void PopErrorMsgAction(string sType)
        {
            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.RECEIVE).PadLeft(5, '0'), false);
            }
            else
            {
                // no action
            }
        }


        private void SaveAction()
        {
            BDDB oBDDB = new BDDB();
            oBDDB.MasterID = txtMasterNo.Text;

            oBDDB.AccFullNo = txtAccNo.Text.Trim();
            oBDDB.CurrCode = ddlPDCurrencyCode.SelectedValue.Trim();
            oBDDB.Address1 = txtAddress1.Text.ToUpper();
            oBDDB.Address2 = txtAddress2.Text.ToUpper();
            oBDDB.Address3 = txtAddress3.Text.ToUpper();
            oBDDB.Address4 = txtAddress4.Text.ToUpper();
            oBDDB.BranchCode = txtBranchCode.Text;
            if (!string.IsNullOrEmpty(txtAccHolderName.Text))
            {
                string[] sAccName = txtAccHolderName.Text.Split(' ');
                if (sAccName.Length > 2)
                {
                    oBDDB.Name1 = sAccName[0].ToUpper();
                    oBDDB.Name2 = sAccName[1].ToUpper();
                    oBDDB.Name3 = sAccName[2].ToUpper();
                }
                else if (sAccName.Length == 2)
                {
                    oBDDB.Name1 = sAccName[0].ToUpper();
                    oBDDB.Name2 = sAccName[1].ToUpper();
                }
                else
                {
                    oBDDB.Name1 = sAccName[0].ToUpper();
                }
            }

            BDDBEntryDAL oBDDBEntryDAL = new BDDBEntryDAL();
            Result oResult = oBDDBEntryDAL.Save(oBDDB);
            if (oResult.Status)
            {
                ClearControlValues();
                ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
            }

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveAction();
        }

        private void ClearControlValues()
        {
            txtAccHolderName.Text = string.Empty;
            txtAccNo.Text = string.Empty;
            txtMasterNo.Text = string.Empty;
            txtAddress1.Text = string.Empty;
            txtAddress2.Text = string.Empty;
            txtAddress3.Text = string.Empty;
            txtAddress4.Text = string.Empty;
            txtBranchCode.Text = string.Empty; ;
            ddlPDCurrencyCode.SelectedIndex = 0;
        }

        protected void txtAccNo_TextChanged(object sender, EventArgs e)
        {
            if (txtAccNo.Text.Length >= 12)
            { 
                txtBranchCode.Text=txtAccNo.Text.Substring(1, 2);
                txtMasterNo.Text = txtAccNo.Text.Substring(0, 9);
            }

        }

    }
}

