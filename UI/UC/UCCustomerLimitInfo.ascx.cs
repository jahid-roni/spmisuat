using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Text;
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Configuration;
using System.Data;
using SBM_BLC1.Common;
using SBM_WebUI.mp;
using SBM_BLC1.Entity.Transaction;
using SBM_BLC1.Transaction;

namespace SBM_WebUI.UI.UC
{
    public partial class UCCustomerLimitInfo : System.Web.UI.UserControl
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitializeData();
            }
        }

        private void InitializeData()
        {
            ClearText();
        }

        private void ClearText()
        {
            gvSearchList.DataSource = null;
            gvSearchList.DataBind();
            txtSPType.Text = string.Empty;
            txtCustomerType.Text = string.Empty;
            txtMaximumLimit.Text = string.Empty;
            txtMinimumLimit.Text = string.Empty;
        }

        public void SetData(string sSpTypeID, string sCustomerType, string sMasterNo)
        {
            CustomerTypeWiseSPLimit oCTWiseSPLimit = new CustomerTypeWiseSPLimit(sCustomerType, sSpTypeID);
            CustomerTypeWiseSPLimitDAL oCTWiseSPLimitDAL = new CustomerTypeWiseSPLimitDAL();
            Result oResult = new Result();
            oResult = oCTWiseSPLimitDAL.LoadByID(oCTWiseSPLimit);
            if (oResult.Status)
            {
                oCTWiseSPLimit = (CustomerTypeWiseSPLimit)oResult.Return;
                txtSPType.Text = sSpTypeID;
                txtCustomerType.Text = sCustomerType;
                txtMaximumLimit.Text = oCTWiseSPLimit.MinimumLimit.ToString();
                txtMinimumLimit.Text = oCTWiseSPLimit.MaximumLimit.ToString();
            }

            // Load Grid Data
            oResult = oCTWiseSPLimitDAL.CheckUserLimit(sSpTypeID, sMasterNo);
            if (oResult.Status)
            { 
                DataTable dtLimit = (DataTable)oResult.Return;
                if (dtLimit != null)
                {
                    if (dtLimit.Rows.Count > 0)
                    {
                        dtLimit.Rows[0]["Allowed Limit"] = (Util.GetDecimalNumber(dtLimit.Rows[0]["IssueAmount"].ToString()) - Util.GetDecimalNumber(txtMaximumLimit.Text)).ToString();
                    }
                }
                gvSearchList.DataSource = dtLimit;
                gvSearchList.DataBind();
                decimal tfsHours = (decimal)dtLimit.Compute("Sum(IssueAmount)", "");
                txtTotalAmount.Text = tfsHours.ToString("N2");
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            ClearText();
        }
   }
}