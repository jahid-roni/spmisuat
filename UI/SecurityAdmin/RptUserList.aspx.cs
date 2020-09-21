using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SBM_BLC1.Common;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Entity.SecurityAdmin;

using System.Data;
using SBM_BLC1.SecurityAdmin;
using SBM_BLC1.DAL.Common;

namespace SBM_WebUI.mp
{
    public partial class RptUserList : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Constants.SES_USER_CONFIG] != null)
            {
                if (!Page.IsPostBack)
                {
                    InitializeData();
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
            txtBranchName.Text = string.Empty;
            txtBankName.Text = string.Empty;
        }
        #endregion InitializeData

        protected void btnPrintPreview_Click(object sender, EventArgs e)
        {
            AdminReportDAL rDal = new AdminReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                // Parameter
                string sBankName = txtBankName.Text;
                string sBranchName = txtBranchName.Text;

                oResult = rDal.UserListReport(sBankName, sBranchName, oConfig.BankCodeID, oConfig.DivisionID);

                if (oResult.Status)
                {
                    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                }
            }
        }
       
    }
}
