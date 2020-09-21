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

namespace SBM_WebUI.UI.UC
{
    public partial class UCCustomerDetail_HSB : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitializeData();            
        }

        private void InitializeData()
        {
            if (!Page.IsPostBack)
            {
                DDListUtil.LoadDDLFromDB(ddlNationalityCountry, "COUNTRY_NAME", "COUNTRY_NAME", "SPMS_CountryList", false);
                DDListUtil.LoadDDLFromDB(ddlResidentCountry, "COUNTRY_NAME", "COUNTRY_NAME", "SPMS_CountryList", false);
                DDListUtil.LoadDDLFromDB(ddlNationalIDCountry, "COUNTRY_NAME", "COUNTRY_NAME", "SPMS_CountryList", false);
                DDListUtil.LoadDDLFromDB(ddlDateofBirthCountry, "COUNTRY_NAME", "COUNTRY_NAME", "SPMS_CountryList", false);
                DDListUtil.LoadDDLFromDB(ddlPassportCountry, "COUNTRY_NAME", "COUNTRY_NAME", "SPMS_CountryList", false);
                DDListUtil.LoadDDLFromDB(ddlBirthCertNoCountry, "COUNTRY_NAME", "COUNTRY_NAME", "SPMS_CountryList", false);

                txtMasterNo.Focus();
            }
            //Enabled only for Joint Customer
            //if (hdnCustomerType.Value.Equals("2"))
            //{
            //    txtCustomerName2.Enabled = true;
            //    txtAddress2.Enabled = true;
            //    txtForignAddress2.Enabled = true;
            //    txtPhone2.Enabled = true;
            //    txtDateofBirth2.Enabled = true;
            //    ddlSex2.Enabled = true;
            //    txtNationality2.Enabled = true;
            //    txtPassportNo2.Enabled = true;
            //    txtIssueAt2.Enabled = true;
            //    txtNationalID2.Enabled = true;
            //    txtBirthCertNo2.Enabled = true;
            //    txtEmail2.Enabled = true;
            //    ddlResidenceStatus2.Enabled = true;
            //}
            //else
            //{
            //    txtCustomerName2.Enabled = false;
            //    txtAddress2.Enabled = false;
            //    txtForignAddress2.Enabled = false;
            //    txtPhone2.Enabled = false;
            //    txtDateofBirth2.Enabled = false;
            //    ddlSex2.Enabled = false;
            //    txtNationality2.Enabled = false;
            //    txtPassportNo2.Enabled = false;
            //    txtIssueAt2.Enabled = false;
            //    txtNationalID2.Enabled = false;
            //    txtBirthCertNo2.Enabled = false;
            //    txtEmail2.Enabled = false;
            //    ddlResidenceStatus2.Enabled = false;
            //}
        }

        protected void btnMasterLoad_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMasterNo.Text))
            {
                CustomerDetails oCustomerDetails = new CustomerDetails();
                oCustomerDetails.MasterNo = txtMasterNo.Text;
                CustomerDetailsDAL oCustomerDetailsDAL = new CustomerDetailsDAL();
                Result oResult = new Result();
                //ClearData();
                oResult = oCustomerDetailsDAL.LoadByMasterID(oCustomerDetails);
                if (oResult.Status)
                {
                    SetCustomerDetails(oCustomerDetails);
                    if (oCustomerDetails.CustomerID > 0)
                    {
                        hdTmpCustomerID.Value = oCustomerDetails.CustomerID.ToString();
                    }
                    else
                    {
                        hdTmpCustomerID.Value = "";
                    }
                }
                else
                {
                    ClearData();
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
            txtPermanentAddress.Text = string.Empty;
            txtForignAddress.Text = string.Empty;
            txtTINNo.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtEmail.Text = string.Empty;
            ddlSex.SelectedIndex = 0;

            txtNationality.Text = "Bangladeshi";
            ddlNationalityCountry.SelectedIndex = 0;
            ddlResidentCountry.SelectedIndex = 0;

            txtDateofBirth.Text = string.Empty;
            ddlDateofBirthCountry.SelectedIndex = 0;
            txtBirthPlace.Text  = string.Empty;

            txtPassportNo.Text = string.Empty;
            ddlPassportCountry.SelectedIndex = 0;
            txtPassportIssueAt.Text = string.Empty;

            txtNationalID.Text = string.Empty;
            ddlNationalIDCountry.SelectedIndex = 0;
            txtNationalIDIssueAt.Text = string.Empty;

            txtBirthCertNo.Text = string.Empty;
            ddlBirthCertNoCountry.SelectedIndex = 0;
            txtBirthCertNoIssueAt.Text = string.Empty;

            ddlResidenceStatus.SelectedIndex = 0;

            //txtCustomerName2.Text = string.Empty;
            //txtAddress2.Text = string.Empty;
            //txtForignAddress2.Text = string.Empty;
            //txtPhone2.Text = string.Empty;
            //txtDateofBirth2.Text = string.Empty;
            //ddlSex2.SelectedIndex = 0;
            //txtNationality2.Text = "Bangladeshi"; 
            //txtPassportNo2.Text = string.Empty;
            //txtIssueAt2.Text = string.Empty;
            //txtNationalID2.Text = string.Empty;
            //txtBirthCertNo2.Text = string.Empty;
            //txtEmail2.Text = string.Empty;
            //ddlResidenceStatus2.SelectedIndex = 0;

            DDListUtil.Assign(ddlDateofBirthCountry, "BANGLADESH");
            DDListUtil.Assign(ddlNationalityCountry, "BANGLADESH");
            DDListUtil.Assign(ddlResidentCountry, "BANGLADESH");
            DDListUtil.Assign(ddlPassportCountry, "BANGLADESH");
            DDListUtil.Assign(ddlNationalIDCountry, "BANGLADESH");
            DDListUtil.Assign(ddlBirthCertNoCountry, "BANGLADESH");

            hdTmpCustomerID.Value = "";
            hdnIsReinvested.Value = "0";

        }
        //private void ClearData2()
        //{
        //    txtCustomerName2.Text = string.Empty;
        //    txtAddress2.Text = string.Empty;
        //    txtForignAddress2.Text = string.Empty;
        //    txtPhone2.Text = string.Empty;
        //    txtDateofBirth2.Text = string.Empty;
        //    ddlSex2.SelectedIndex = 0;
        //    txtNationality2.Text = "Bangladeshi";
        //    txtPassportNo2.Text = string.Empty;
        //    txtIssueAt2.Text = string.Empty;
        //    txtNationalID2.Text = string.Empty;
        //    txtBirthCertNo2.Text = string.Empty;
        //    txtEmail2.Text = string.Empty;
        //    ddlResidenceStatus2.SelectedIndex = 0;
        //    hdnIsReinvested.Value = "0";
        //}

        public void SetCustomerDetails(CustomerDetails oCustomerDetails)
        {
            ClearData();
            if (oCustomerDetails != null)
            {
                if (oCustomerDetails.isReinvestmet)
                {
                    btnSaveAndLoad.Enabled = true;
                    btnMasterLoad.Enabled = false;
                }
                else if (oCustomerDetails.isViewOnly)
                {
                    btnSaveAndLoad.Enabled = false;
                    btnMasterLoad.Enabled = false;
                }
                else
                {
                    btnSaveAndLoad.Enabled = true;
                    btnMasterLoad.Enabled = true;
                }

                txtCustomerID.Text = oCustomerDetails.CustomerID.ToString();
                txtMasterNo.Text = oCustomerDetails.MasterNo;
                txtCustomerName.Text = oCustomerDetails.CustomerName;
                IsHUBCust.Checked = oCustomerDetails.IsHUBCust;
                DDListUtil.Assign(ddlSCCFlags, oCustomerDetails.SCCFlag);

                txtAddress.Text = oCustomerDetails.Address;
                txtPermanentAddress.Text = oCustomerDetails.PermanentAddress;
                txtForignAddress.Text = oCustomerDetails.ForeignAddress;
                txtTINNo.Text = oCustomerDetails.TIN; 

                txtPhone.Text = oCustomerDetails.Phone;

                txtDateofBirth.Text = oCustomerDetails.DateOfBirth.ToString(Constants.DATETIME_FORMAT);
                if (oCustomerDetails.DateOfBirth_Country == "")
                {
                    DDListUtil.Assign(ddlDateofBirthCountry, "BANGLADESH");
                }
                else
                {
                    DDListUtil.Assign(ddlDateofBirthCountry, oCustomerDetails.DateOfBirth_Country);
                }
                

                txtBirthPlace.Text = oCustomerDetails.DateOfBirth_Place;

                DDListUtil.Assign(ddlSex,oCustomerDetails.Sex);

                if (!string.IsNullOrEmpty(oCustomerDetails.Nationality))
                {
                    txtNationality.Text = oCustomerDetails.Nationality;
                }

                if (oCustomerDetails.Nationality_Country == "")
                {
                    DDListUtil.Assign(ddlNationalityCountry, "BANGLADESH");
                }
                else
                {
                    DDListUtil.Assign(ddlNationalityCountry, oCustomerDetails.Nationality_Country);
                }
                if (oCustomerDetails.Resident_Country == "")
                {
                    DDListUtil.Assign(ddlResidentCountry, "BANGLADESH");
                }
                else
                {

                    DDListUtil.Assign(ddlResidentCountry, oCustomerDetails.Resident_Country);
                }
                txtPassportNo.Text = oCustomerDetails.PassportNo;
                if (oCustomerDetails.PassportNo_Country == "")
                {
                    DDListUtil.Assign(ddlPassportCountry, "BANGLADESH");
                }
                else
                {

                    DDListUtil.Assign(ddlPassportCountry, oCustomerDetails.PassportNo_Country);
                }
                txtPassportIssueAt.Text = oCustomerDetails.PassportNo_IssueAt;

                txtNationalID.Text = oCustomerDetails.NationalID;
                if (oCustomerDetails.NationalID_Country == "")
                {
                    DDListUtil.Assign(ddlPassportCountry, "BANGLADESH");
                    DDListUtil.Assign(ddlNationalIDCountry, "BANGLADESH");
                    DDListUtil.Assign(ddlBirthCertNoCountry, "BANGLADESH");
                }
                else
                {

                    DDListUtil.Assign(ddlNationalIDCountry, oCustomerDetails.NationalID_Country);
                }
                txtNationalIDIssueAt.Text = oCustomerDetails.NationalID_IssueAt;

                txtBirthCertNo.Text = oCustomerDetails.BirthCertificateNo;
                if (oCustomerDetails.BirthCertificateNo_Country == "")
                {
                    DDListUtil.Assign(ddlBirthCertNoCountry, "BANGLADESH");
                }
                else
                {
                    DDListUtil.Assign(ddlBirthCertNoCountry, oCustomerDetails.BirthCertificateNo_Country);
                }
                txtBirthCertNoIssueAt.Text = oCustomerDetails.BirthCertificateNo_IssueAt;

                txtEmail.Text = oCustomerDetails.EmailAddress;
                DDListUtil.Assign(ddlResidenceStatus, oCustomerDetails.ResidenceStatus);

                //txtCustomerName2.Text = oCustomerDetails.CustomerName2;
                //txtAddress2.Text = oCustomerDetails.Address2;
                //txtForignAddress2.Text = oCustomerDetails.ForeignAddress2;
                //txtPhone2.Text = oCustomerDetails.Phone2;
                //txtDateofBirth2.Text = oCustomerDetails.DateOfBirth2.ToString(Constants.DATETIME_FORMAT);
                //DDListUtil.Assign(ddlSex2, oCustomerDetails.Sex2);
                //if (!string.IsNullOrEmpty(oCustomerDetails.Nationality2))
                //{
                //    txtNationality2.Text = oCustomerDetails.Nationality2;
                //}
                //txtPassportNo2.Text = oCustomerDetails.PassportNo2;
                //txtIssueAt2.Text = oCustomerDetails.IssuedAt2;
                //txtNationalID2.Text = oCustomerDetails.NationalID2;
                //txtBirthCertNo2.Text = oCustomerDetails.BirthCertificateNo2;
                //txtEmail2.Text = oCustomerDetails.EmailAddress2;
                //DDListUtil.Assign(ddlResidenceStatus2, oCustomerDetails.ResidenceStatus2);

                hdTmpCustomerID.Value = oCustomerDetails.CustomerID.ToString();
                if (oCustomerDetails.isReinvestmet)
                {
                    hdnIsReinvested.Value = "1";
                }
                EnableDisableControls(oCustomerDetails.isViewOnly, oCustomerDetails.isReinvestmet);
            }
        }

        //public void SetCustomerDetails2(CustomerDetails oCustomerDetails)
        //{
        //    ClearData2();
        //    if (oCustomerDetails != null)
        //    {
        //        if (oCustomerDetails.isReinvestmet)
        //        {
        //            btnSaveAndLoad.Enabled = true;
        //            btnMasterLoad.Enabled = false;
        //        }
        //        else if (oCustomerDetails.isViewOnly)
        //        {
        //            btnSaveAndLoad.Enabled = false;
        //            btnMasterLoad.Enabled = false;
        //        }
        //        else
        //        {
        //            btnSaveAndLoad.Enabled = true;
        //            btnMasterLoad.Enabled = true;
        //        }

        //        //txtCustomerID.Text = oCustomerDetails.CustomerID.ToString();
        //        txtMasterNo2.Text = oCustomerDetails.MasterNo2;
        //        txtCustomerName2.Text = oCustomerDetails.CustomerName2;
        //        txtAddress2.Text = oCustomerDetails.Address2;
        //        txtForignAddress2.Text = oCustomerDetails.ForeignAddress2;
        //        txtPhone2.Text = oCustomerDetails.Phone2;
        //        txtDateofBirth2.Text = oCustomerDetails.DateOfBirth2.ToString(Constants.DATETIME_FORMAT);
        //        DDListUtil.Assign(ddlSex2, oCustomerDetails.Sex2);
        //        if (!string.IsNullOrEmpty(oCustomerDetails.Nationality2))
        //        {
        //            txtNationality2.Text = oCustomerDetails.Nationality2;
        //        }
        //        txtPassportNo2.Text = oCustomerDetails.PassportNo2;
        //        txtIssueAt2.Text = oCustomerDetails.IssuedAt2;
        //        txtNationalID2.Text = oCustomerDetails.NationalID2;
        //        txtBirthCertNo2.Text = oCustomerDetails.BirthCertificateNo2;
        //        txtEmail2.Text = oCustomerDetails.EmailAddress2;
        //        DDListUtil.Assign(ddlResidenceStatus2, oCustomerDetails.ResidenceStatus2);

        //        EnableDisableControls(oCustomerDetails.isViewOnly, oCustomerDetails.isReinvestmet);
        //    }
        //}

        public CustomerDetails GetCustomerDetails()
        {
            CustomerDetails oCustomerDetails = new CustomerDetails(Util.GetIntNumber(txtCustomerID.Text));

            oCustomerDetails.CustomerID = Util.GetIntNumber(hdTmpCustomerID.Value == "" ? "-1" : hdTmpCustomerID.Value);
            oCustomerDetails.MasterNo = txtMasterNo.Text;
            oCustomerDetails.CustomerName = txtCustomerName.Text.Replace("'","").ToUpper();

            oCustomerDetails.IsHUBCust=IsHUBCust.Checked;
            oCustomerDetails.SCCFlag = ddlSCCFlags.SelectedItem != null ? ddlSCCFlags.SelectedItem.Value : "0";

            oCustomerDetails.Address = txtAddress.Text.Replace("'", "").ToUpper();
            oCustomerDetails.PermanentAddress = txtPermanentAddress.Text.Replace("'", "").ToUpper();
            oCustomerDetails.ForeignAddress = txtForignAddress.Text.Replace("'", "").ToUpper();
            oCustomerDetails.Phone = txtPhone.Text;

            oCustomerDetails.TIN= txtTINNo.Text;

            oCustomerDetails.DateOfBirth = Util.GetDateTimeByString(txtDateofBirth.Text);
            oCustomerDetails.DateOfBirth_Country = ddlDateofBirthCountry.SelectedItem != null ? ddlDateofBirthCountry.SelectedItem.Value : "Unknown";
            oCustomerDetails.DateOfBirth_Place = txtBirthPlace.Text;

            oCustomerDetails.Sex = ddlSex.SelectedItem != null ? ddlSex.SelectedItem.Value :"U";

            oCustomerDetails.Nationality = txtNationality.Text;
            oCustomerDetails.Nationality_Country = ddlNationalityCountry.SelectedItem != null ? ddlNationalityCountry.SelectedItem.Value : "Unknown";
            oCustomerDetails.Resident_Country = ddlResidentCountry.SelectedItem != null ? ddlResidentCountry.SelectedItem.Value : "Unknown";

            oCustomerDetails.PassportNo = txtPassportNo.Text.ToUpper();
            oCustomerDetails.PassportNo_Country = ddlPassportCountry.SelectedItem != null ? ddlPassportCountry.SelectedItem.Value : "Unknown";
            oCustomerDetails.PassportNo_IssueAt = txtPassportIssueAt.Text;

            oCustomerDetails.NationalID = txtNationalID.Text;
            oCustomerDetails.NationalID_Country = ddlNationalIDCountry.SelectedItem != null ? ddlNationalIDCountry.SelectedItem.Value : "Unknown";
            oCustomerDetails.NationalID_IssueAt = txtNationalIDIssueAt.Text;

            oCustomerDetails.BirthCertificateNo = txtBirthCertNo.Text;
            oCustomerDetails.BirthCertificateNo_Country = ddlBirthCertNoCountry.SelectedItem != null ? ddlBirthCertNoCountry.SelectedItem.Value : "Unknown";
            oCustomerDetails.BirthCertificateNo_IssueAt= txtBirthCertNoIssueAt.Text;

            oCustomerDetails.EmailAddress = txtEmail.Text;
            oCustomerDetails.ResidenceStatus = ddlResidenceStatus.SelectedItem.Value;

            //oCustomerDetails.CustomerName2 = txtCustomerName2.Text.Replace("'", "").ToUpper();
            //oCustomerDetails.Address2 = txtAddress2.Text.Replace("'", "").ToUpper(); ;
            //oCustomerDetails.ForeignAddress2 = txtForignAddress2.Text.Replace("'", "").ToUpper(); ;
            //oCustomerDetails.Phone2 = txtPhone2.Text;
            //oCustomerDetails.DateOfBirth2 = Util.GetDateTimeByString(txtDateofBirth2.Text);
            //oCustomerDetails.Sex2 = ddlSex2.SelectedItem != null ? ddlSex2.SelectedItem.Value : "0";
            //oCustomerDetails.Nationality2= txtNationality2.Text;
            //oCustomerDetails.PassportNo2 = txtPassportNo2.Text.ToUpper();
            //oCustomerDetails.IssuedAt2 = txtIssueAt2.Text;
            //oCustomerDetails.NationalID2 = txtNationalID2.Text;
            //oCustomerDetails.BirthCertificateNo2 = txtBirthCertNo2.Text;
            //oCustomerDetails.EmailAddress2 = txtEmail2.Text;
            //oCustomerDetails.ResidenceStatus2 = ddlResidenceStatus2.SelectedItem.Value;

            //oCustomerDetails.isJointlyOperate = chkOperate.Checked;


            
            return oCustomerDetails; 
        }

        private void EnableDisableControls(bool isViewOnly, bool isReinvesment)
        {
            bool enable=true;

            if (isViewOnly)
            {
                enable = false;
            }
            txtMasterNo.Enabled = enable;
            txtCustomerID.Enabled = enable;
            txtCustomerName.Enabled = enable;
            IsHUBCust.Enabled = enable;
            ddlSCCFlags.Enabled = enable;
            txtAddress.Enabled = enable;
            txtPermanentAddress.Enabled = enable;
            txtForignAddress.Enabled = enable;
            txtPhone.Enabled = enable;
            txtEmail.Enabled = enable;
            ddlSex.Enabled = enable;

            txtNationality.Enabled = enable;
            ddlNationalityCountry.Enabled = enable;
            ddlResidentCountry.Enabled = enable;

            txtDateofBirth.Enabled = enable;
            ddlDateofBirthCountry.Enabled = enable;
            txtBirthPlace.Enabled = enable;

            txtPassportNo.Enabled = enable;
            ddlPassportCountry.Enabled = enable;
            txtPassportIssueAt.Enabled = enable;

            txtNationalID.Enabled = enable;
            ddlNationalIDCountry.Enabled = enable;
            txtNationalIDIssueAt.Enabled = enable;

            txtBirthCertNo.Enabled = enable;
            ddlBirthCertNoCountry.Enabled = enable;
            txtBirthCertNoIssueAt.Enabled = enable;
            ddlResidenceStatus.Enabled = enable;
            if (isReinvesment)
            {
                txtMasterNo.Enabled = false;
                txtCustomerID.Enabled = false;
                txtCustomerName.Enabled = false;
                IsHUBCust.Enabled = false;
                ddlSCCFlags.Enabled = true;
                txtAddress.Enabled = true;
                txtPermanentAddress.Enabled = true;
                txtForignAddress.Enabled = true;
                txtPhone.Enabled = true;
                txtEmail.Enabled = true;
                ddlSex.Enabled = true;

                txtNationality.Enabled = true;
                ddlNationalityCountry.Enabled = true;
                ddlResidentCountry.Enabled = true;

                txtDateofBirth.Enabled = true;
                ddlDateofBirthCountry.Enabled = true;
                txtBirthPlace.Enabled = true;

                txtPassportNo.Enabled = true;
                ddlPassportCountry.Enabled = true;
                txtPassportIssueAt.Enabled = true;

                txtNationalID.Enabled = true;
                ddlNationalIDCountry.Enabled = true;
                txtNationalIDIssueAt.Enabled = true;

                txtBirthCertNo.Enabled = true;
                ddlBirthCertNoCountry.Enabled = true;
                txtBirthCertNoIssueAt.Enabled = true;

                ddlResidenceStatus.Enabled = false;
                btnSaveAndLoad.Enabled = true;
            }
            //txtCustomerID.ReadOnly = isViewOnly;
            //txtMasterNo.ReadOnly = isViewOnly;
            //txtCustomerName.ReadOnly = isViewOnly;
            //if (isReinvesment)
            //{
            //    txtAddress.ReadOnly = false;
            //}
            //else
            //{
            //    txtAddress.ReadOnly = isViewOnly;
            //}
            //txtForignAddress.ReadOnly = isViewOnly;
            //txtPhone.ReadOnly = isViewOnly;
            //txtDateofBirth.ReadOnly = isViewOnly;
            //if (isViewOnly)
            //{
            //    ddlSex.Enabled = false;
            //}
            //else
            //{
            //    ddlSex.Enabled = true;
            //}
            //txtNationality.ReadOnly = isViewOnly;
            //if (isReinvesment)
            //{
            //    txtPassportNo.ReadOnly = false;
            //}
            //else
            //{
            //    txtPassportNo.ReadOnly = isViewOnly;
            //}
            //txtNationalID.ReadOnly = isViewOnly;
            //txtBirthCertNo.ReadOnly = isViewOnly;
            //txtEmail.ReadOnly = isViewOnly;
            //if (isViewOnly)
            //{
            //    ddlResidenceStatus.Enabled = false;
            //}
            //else
            //{
            //    ddlResidenceStatus.Enabled = true;
            //}
        }

        protected void btnSaveAndLoad_Click(object sender, EventArgs e)
        {
            object[] oMethodParameters = new object[1];
            oMethodParameters[0] = GetCustomerDetails();
            try
            {
                ClearData();
                Page.GetType().InvokeMember("CustomerDetailAction", BindingFlags.InvokeMethod, null, this.Page, oMethodParameters);
            }
            catch (TargetInvocationException TIE)
            { 
                // nothing.. 
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            txtCustomerID.Text = string.Empty;
            txtCustomerName.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtPermanentAddress.Text = string.Empty;
            txtForignAddress.Text = string.Empty;
            txtTINNo.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtEmail.Text = string.Empty;
            ddlSex.SelectedIndex = 0;

            txtNationality.Text = "Bangladeshi";
            ddlNationalityCountry.SelectedIndex = 0;
            ddlResidentCountry.SelectedIndex = 0;

            txtDateofBirth.Text = string.Empty;
            ddlDateofBirthCountry.SelectedIndex = 0;
            txtBirthPlace.Text = string.Empty;

            txtPassportNo.Text = string.Empty;
            ddlPassportCountry.SelectedIndex = 0;
            txtPassportIssueAt.Text = string.Empty;

            txtNationalID.Text = string.Empty;
            ddlNationalIDCountry.SelectedIndex = 0;
            txtNationalIDIssueAt.Text = string.Empty;

            txtBirthCertNo.Text = string.Empty;
            ddlBirthCertNoCountry.SelectedIndex = 0;
            txtBirthCertNoIssueAt.Text = string.Empty;

            ddlResidenceStatus.SelectedIndex = 0;

            //txtCustomerName2.Text = string.Empty;
            //txtAddress2.Text = string.Empty;
            //txtForignAddress2.Text = string.Empty;
            //txtPhone2.Text = string.Empty;
            //txtDateofBirth2.Text = string.Empty;
            //ddlSex2.SelectedIndex = 0;
            //txtNationality2.Text = "Bangladeshi"; 
            //txtPassportNo2.Text = string.Empty;
            //txtIssueAt2.Text = string.Empty;
            //txtNationalID2.Text = string.Empty;
            //txtBirthCertNo2.Text = string.Empty;
            //txtEmail2.Text = string.Empty;
            //ddlResidenceStatus2.SelectedIndex = 0;

            hdTmpCustomerID.Value = "";
            hdnIsReinvested.Value = "0";
        }
    }
}