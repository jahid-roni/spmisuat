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
using SBM_BLC1.Common;
using SBM_BLC1.DAL.Common;

namespace SBM_WebUI.mp
{
    public partial class CustomerDetailsView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnMasterLoad_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMasterNo.Text))
            {
                CustomerDetails oCustomerDetails = new CustomerDetails();
                oCustomerDetails.MasterNo = txtMasterNo.Text;
                CustomerDetailsDAL oCustomerDetailsDAL = new CustomerDetailsDAL();
                Result oResult = new Result();
                ClearData();
                oResult = oCustomerDetailsDAL.LoadByMasterID(oCustomerDetails);
                if (oResult.Status)
                {
                    //txtCustomerID.Text = Convert.ToString(oCustomerDetails.CustomerID);
                    //txtMasterNo.Text = oCustomerDetails.MasterNo;
                    //txtCustomerName.Text = oCustomerDetails.CustomerName;
                    //txtPhone.Text = oCustomerDetails.Phone;
                    //txtAddress.Text = oCustomerDetails.Address;
                    //txtDateofBirth.Text = oCustomerDetails.DateOfBirth.ToString(Constants.DATETIME_FORMAT);
                    //ddlSex.Text = oCustomerDetails.Sex;
                    //ddlResidenceStatus.Text = oCustomerDetails.ResidenceStatus;
                    //txtNationalID.Text = oCustomerDetails.NationalID;
                    //txtEmail.Text = oCustomerDetails.EmailAddress;

                    //hdTmpCustomerID.Value = "";
                    SetCustomerDetails(oCustomerDetails);
                }
            }
            StringBuilder sbUrl = new StringBuilder();
            sbUrl.Append("<script> ");
            sbUrl.Append(" CustomerDetailPopup() ");
            sbUrl.Append("</script>");
            Page.RegisterStartupScript("OpenWindows", sbUrl.ToString());
        }

        private void ClearData()
        {
            txtMasterNo.Text = string.Empty;
            txtCustomerID.Text = string.Empty;
            txtCustomerName.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtForignAddress.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtDateofBirth.Text = string.Empty;
            txtSex.Text = string.Empty;
            txtNationality.Text = "Bangladeshi";
            txtPassportNo.Text = string.Empty;
            txtIssueAt.Text = string.Empty;
            txtNationalID.Text = string.Empty;
            txtBirthCertNo.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtResStatus.Text = string.Empty;

            txtCustomerName2.Text = string.Empty;
            txtAddress2.Text = string.Empty;
            txtForignAddress2.Text = string.Empty;
            txtPhone2.Text = string.Empty;
            txtDateofBirth2.Text = string.Empty;
            txtSex2.Text = string.Empty;
            txtNationality2.Text = "Bangladeshi";
            txtPassportNo2.Text = string.Empty;
            txtIssueAt2.Text = string.Empty;
            txtNationalID2.Text = string.Empty;
            txtBirthCertNo2.Text = string.Empty;
            txtEmail2.Text = string.Empty;
            txtResStatus2.Text = string.Empty;

            hdTmpCustomerID.Value = "";

        }

        public void SetCustomerDetails(CustomerDetails oCustomerDetails)
        {
            ClearData();
            if (oCustomerDetails != null)
            {

                txtCustomerID.Text = oCustomerDetails.CustomerID.ToString();
                txtMasterNo.Text = oCustomerDetails.MasterNo;
                txtCustomerName.Text = oCustomerDetails.CustomerName;
                txtAddress.Text = oCustomerDetails.Address;
                txtForignAddress.Text = oCustomerDetails.ForeignAddress;
                txtPhone.Text = oCustomerDetails.Phone;
                txtDateofBirth.Text = oCustomerDetails.DateOfBirth.ToString(Constants.DATETIME_FORMAT);
                txtSex.Text = oCustomerDetails.Sex.ToString();
                txtNationality.Text = oCustomerDetails.Nationality;
                txtPassportNo.Text = oCustomerDetails.PassportNo;
                //txtIssueAt.Text = oCustomerDetails.PassportNo_IssueAt;
                txtNationalID.Text = oCustomerDetails.NationalID;
                txtBirthCertNo.Text = oCustomerDetails.BirthCertificateNo;
                txtEmail.Text = oCustomerDetails.EmailAddress;
                txtResStatus.Text = oCustomerDetails.ResidenceStatus;

                //txtCustomerName2.Text = oCustomerDetails.CustomerName2;
                //txtAddress2.Text = oCustomerDetails.Address2;
                //txtForignAddress2.Text = oCustomerDetails.ForeignAddress2;
                //txtPhone2.Text = oCustomerDetails.Phone2;
                //txtDateofBirth2.Text = oCustomerDetails.DateOfBirth2.ToString(Constants.DATETIME_FORMAT);
                //txtSex2.Text = oCustomerDetails.Sex2;
                //txtNationality2.Text = oCustomerDetails.Nationality2;
                //txtPassportNo2.Text = oCustomerDetails.PassportNo2;
                //txtIssueAt2.Text = oCustomerDetails.IssuedAt2;
                //txtNationalID2.Text = oCustomerDetails.NationalID2;
                //txtBirthCertNo2.Text = oCustomerDetails.BirthCertificateNo2;
                //txtEmail2.Text = oCustomerDetails.EmailAddress2;
                //txtResStatus2.Text = oCustomerDetails.ResidenceStatus2;

                hdTmpCustomerID.Value = oCustomerDetails.CustomerID.ToString();
            }
        }

        protected void btnPrintLimit_Click(object sender, EventArgs e)
        {
            ReportDAL rdal = new ReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];


            if (!string.IsNullOrEmpty(txtMasterNo.Text))
            {

                if (oConfig != null)
                {
                    string sCustomerID = txtCustomerID.Text;
                    string sMasterNo = txtMasterNo.Text;
                    string sCustomerName = txtCustomerName.Text;

                    oResult = rdal.CustomerLimitReport(sCustomerName, sCustomerID, sMasterNo, oConfig.BankCodeID, oConfig.DivisionID, oConfig.BranchID);

                    if (oResult.Status)
                    {
                        if (oResult.Return != null)
                        {
                            Session[Constants.SES_RPT_DATA] = oResult.Return;
                            Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                        }
                        else
                        {
                            ucMessage.OpenMessage(oResult.Message, Constants.MSG_TYPE_INFO);
                            ScriptManager.RegisterStartupScript(this.upSuccess, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        }
                    }
                }
            }
            else
            {
                ucMessage.OpenMessage("Enter a valid master no. Please check!", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.upSuccess, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);

            }
        }

        public void PopErrorMsgAction(string sType)
        {
            //nothing
        }
    }
}