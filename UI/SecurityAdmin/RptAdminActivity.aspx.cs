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
    public partial class RptAdminActivity : System.Web.UI.Page
    {
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_USER_ID = "sUserID"; 
        public const string OBJ_PAGE_ID = "sPageID";


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
            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlUserClass, "GroupID", "GroupName", "SA_Group", true);                        

            string sUserID = Request.QueryString[OBJ_USER_ID];
            string sPageID = Request.QueryString[OBJ_PAGE_ID];
            if (!string.IsNullOrEmpty(sUserID))
            {
                sUserID = oCrypManager.GetDecryptedString(sUserID, Constants.CRYPT_PASSWORD_STRING);
            }
            if (!string.IsNullOrEmpty(sPageID))
            {
                sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
            }

            if (!string.IsNullOrEmpty(sUserID) && !string.IsNullOrEmpty(sPageID))
            {
                string sOperationType = sPageID.Substring(4, 1);
                if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
                {
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    //LoadDataByID(sUserID);

                    

                    //UserDetails oUserDetails = ucUserDet.UserDetail;
                    //oUserDetails.CheckerID = oConfig.UserName;
                    //if (oUserDetails.CheckDate != null)
                    //{
                    //    oUserDetails.CheckDate = DateTime.Now;
                    //}
                    //ucUserDet.UserDetail = oUserDetails;
                    //fsList.Visible = false;
                }
            }
            else
            {
                
                //Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                //UserDetails oUserDetails = new UserDetails();
                //oUserDetails.MakerID = oConfig.UserName;
                //oUserDetails.MakeDate = DateTime.Now;
                //ucUserDet.UserDetail = oUserDetails;

                //fsList.Visible = true;
            }
            
        }

        #endregion InitializeData

        protected void btnPrintPreview_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                AdminReportDAL aRDal = new AdminReportDAL();
                Result oResult = new Result();
                // Parameter                
                DateTime dtDate = Util.GetDateTimeByString(txtReporDate.Text);
                oResult = aRDal.AdminActivityReport(dtDate, ddlUserClass.SelectedValue, txtBankName.Text, txtBranchName.Text, oConfig.BankCodeID, oConfig.DivisionID);
                if (oResult.Status)
                {
                    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    //Response.Redirect(Constants.PAGE_RPT_VIEW);
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                }
            }
        }
    }
}
