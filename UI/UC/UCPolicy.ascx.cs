using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SBM_BLC1.Entity.Configuration;
using System.Data;
using System.Collections;
using SBM_BLC1.DAL.Common;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Common;

namespace SBM_WebUI.UI.UC
{
    public partial class UCPolicy : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void SetPolicyDetails(SPPolicy oSPPolicy)
        {
            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlSPType, "SPTypeID", "TypeDesc", "SPMS_SPType" , true);
            DDListUtil.LoadDDLFromXML(ddlApplicableSex, "ApplicableSex", "Type", "Sex", true);
            
            // gvCustomerType load  6.	Eligibility
            UtilityDAL oUtilityDAL = new UtilityDAL();
            Result oResult = new Result();
            oResult = oUtilityDAL.GetDDLDataList("CustomerTypeID", "TypeDesc", "SPMS_CustomerType", true);
            if (oResult.Status)
            {
                DataTable dtGetAll = (DataTable)oResult.Return;
                gvCustomerType.DataSource = dtGetAll;
                gvCustomerType.DataBind();
                gvCustomerType.Enabled = false;

                GridViewRowCollection growArr = (GridViewRowCollection)gvCustomerType.Rows;
                foreach (GridViewRow row in growArr)
                {
                    row.Cells[1].Visible = false;
                }
            }
            else
            {
                gvCustomerType.DataSource = false;
                gvCustomerType.DataBind();
            }


            // common
            DDListUtil.Assign(ddlSPType, oSPPolicy.SPType.SPTypeID);
            ddlSPType.Enabled = false;
            txtEffectiveDate.Text = oSPPolicy.PolicyEffectDate.ToString(Constants.DATETIME_FORMAT);
            txtEffectiveDate.Enabled = false;


            // 1.0 General 
            Util.SetRadioData(rblIsSPDurationInMonth, oSPPolicy.IsSPDurationInMonth);
            rblIsSPDurationInMonth.Enabled = false;
            Util.RBLChangeSetColor(rblIsSPDurationInMonth);
            rblIsSPDurationInMonth.Enabled = false;
            txtDuration.Text = oSPPolicy.SPDuration.ToString();
            txtDuration.Enabled = false;
            txtNoOfCoupon.Text = oSPPolicy.NoOfCoupons.ToString();
            txtNoOfCoupon.Enabled = false;

            DDListUtil.Assign(ddlIntrType, oSPPolicy.SPInterestType);
            ddlIntrType.Enabled = false;
            Util.SetRadioData(rblInterestTypeAfterIntPayment, oSPPolicy.InterestTypeAfterIntPayment);
            Util.RBLChangeSetColor(rblInterestTypeAfterIntPayment);
            rblInterestTypeAfterIntPayment.Enabled = false;
            DDListUtil.Assign(ddlPreMatIntrType, oSPPolicy.PreMaturityInterestType);
            ddlPreMatIntrType.Enabled = false;
            Util.SetRadioData(rblGSPreMatIntrClaim, oSPPolicy.PreMatIntTypeAfterIntPayment);
            rblGSPreMatIntrClaim.Enabled = false;
            Util.RBLChangeSetColor(rblGSPreMatIntrClaim);

            Util.SetRadioData(rblIsBondHolderRequired, oSPPolicy.IsBondHolderRequired);
            Util.RBLChangeSetColor(rblIsBondHolderRequired);
            rblIsBondHolderRequired.Enabled = false;
            Util.SetRadioData(rblIsNomineePerScripRequired, oSPPolicy.IsNomineePerScripRequired);
            Util.RBLChangeSetColor(rblIsNomineePerScripRequired);
            rblIsNomineePerScripRequired.Enabled=false;

            Util.SetRadioData(rblIsFoeignAddressRequired, oSPPolicy.IsFoeignAddressRequired);
            Util.RBLChangeSetColor(rblIsFoeignAddressRequired);
            rblIsFoeignAddressRequired.Enabled = false;

            Util.SetRadioData(rblReinvestmentSuported, oSPPolicy.ReinvestmentSuported);
            Util.RBLChangeSetColor(rblReinvestmentSuported);
            rblReinvestmentSuported.Enabled = false;
            Util.SetRadioData(rblInterestReinvestable, oSPPolicy.InterestReinvestable);
            Util.RBLChangeSetColor(rblInterestReinvestable);
            rblInterestReinvestable.Enabled = false;
            txtReinNumber.Text = oSPPolicy.MaxNoOfReinvestment.ToString();
            txtReinNumber.Enabled = false;
            Util.SetRadioData(rblPartiallyEncashable, oSPPolicy.PartiallyEncashable);
            Util.RBLChangeSetColor(rblPartiallyEncashable);
            rblPartiallyEncashable.Enabled = false;
            Util.SetRadioData(rblPartiallyEncashedReinvestable, oSPPolicy.PartiallyEncashedReinvestable);
            Util.RBLChangeSetColor(rblPartiallyEncashedReinvestable);
            rblPartiallyEncashedReinvestable.Enabled = false;

            // 2.0 Currency Setup
            DataTable oDtCurr = new DataTable("dtDataCurrency");
            oDtCurr.Columns.Add(new DataColumn("bfActivityType", typeof(string)));
            oDtCurr.Columns.Add(new DataColumn("bfCurrency", typeof(string)));
            DataRow rowCurr = null;

            for (int i = 0; i < oSPPolicy.CurrencyActivityPolicy.Count; i++)
            {
                rowCurr = oDtCurr.NewRow();
                rowCurr["bfActivityType"] = oSPPolicy.CurrencyActivityPolicy[i].ActivityTypeID + " : " + oSPPolicy.CurrencyActivityPolicy[i].ActivityTypeValue;
                rowCurr["bfCurrency"] = oSPPolicy.CurrencyActivityPolicy[i].Currency.CurrencyID + " : " + oSPPolicy.CurrencyActivityPolicy[i].Currency.CurrencyCode;

                oDtCurr.Rows.Add(rowCurr);
            }
            gvActiCurrency.DataSource = oDtCurr;
            gvActiCurrency.DataBind();
            gvActiCurrency.Enabled = false;

            // 3.0 Early Encashment Setup
            txtEarlyEncashCouponNo.Text = oSPPolicy.NoOfCoupons.ToString();
            txtEarlyEncashCouponNo.Enabled = false;
            txtCommonIntRate.Enabled = false;
            chkMaturedCoupon.Enabled = false;
            DataTable oDtEE = new DataTable("dtDataEE");

            oDtEE.Columns.Add(new DataColumn("bfCouponInstallmentNo", typeof(string)));
            oDtEE.Columns.Add(new DataColumn("bfMonthFrom", typeof(string)));
            oDtEE.Columns.Add(new DataColumn("bfMonthTo", typeof(string)));
            oDtEE.Columns.Add(new DataColumn("bfInterestRate", typeof(string)));
            oDtEE.Columns.Add(new DataColumn("bfNoOfSlabsIntPayable", typeof(string)));

            DataRow rowEE = null;

            for (int i = 0; i < oSPPolicy.EarlyEncashmentPolicy.Count; i++)
            {
                rowEE = oDtEE.NewRow();
                rowEE["bfCouponInstallmentNo"] = oSPPolicy.EarlyEncashmentPolicy[i].SlabNo.ToString();
                rowEE["bfMonthFrom"] = oSPPolicy.EarlyEncashmentPolicy[i].MonthFrom.ToString();
                rowEE["bfMonthTo"] = oSPPolicy.EarlyEncashmentPolicy[i].MonthTo.ToString();
                rowEE["bfInterestRate"] = oSPPolicy.EarlyEncashmentPolicy[i].InterestRate.ToString();
                rowEE["bfNoOfSlabsIntPayable"] = oSPPolicy.EarlyEncashmentPolicy[i].NoOfSlabsIntPayable.ToString();

                oDtEE.Rows.Add(rowEE);
            }
            gvEncashmentIntRate.DataSource = oDtEE;
            gvEncashmentIntRate.DataBind();
            gvEncashmentIntRate.Enabled = false;

            // 4.0 General Interest Setup
            txtGeneralIntCouponNo.Text = oSPPolicy.NoOfCoupons.ToString();
            txtGeneralIntCouponNo.Enabled = false;
            txtGIClaimRate.Enabled = false;
            txtNonclaimIntRate.Enabled = false;

            DataTable oDtGI = new DataTable("dtDataG");
            oDtGI.Columns.Add(new DataColumn("bfCouponInstallmentNo", typeof(string)));
            oDtGI.Columns.Add(new DataColumn("bfMonthFrom", typeof(string)));
            oDtGI.Columns.Add(new DataColumn("bfMonthTo", typeof(string)));
            oDtGI.Columns.Add(new DataColumn("bfClaimRate", typeof(string)));
            oDtGI.Columns.Add(new DataColumn("bfNonclaimIntRate", typeof(string)));

            DataRow rowGI = null;

            for (int i = 0; i < oSPPolicy.GeneralInterestPolicy.Count; i++)
            {
                rowGI = oDtGI.NewRow();
                rowGI["bfCouponInstallmentNo"] = oSPPolicy.GeneralInterestPolicy[i].SlabNo.ToString();
                rowGI["bfMonthFrom"] = oSPPolicy.GeneralInterestPolicy[i].MonthFrom.ToString();
                rowGI["bfMonthTo"] = oSPPolicy.GeneralInterestPolicy[i].MonthTo.ToString();
                rowGI["bfClaimRate"] = oSPPolicy.GeneralInterestPolicy[i].ClaimRate.ToString();
                rowGI["bfNonclaimIntRate"] = oSPPolicy.GeneralInterestPolicy[i].NonClaimRate.ToString();

                oDtGI.Rows.Add(rowGI);
            }
            gvGeneralInt.DataSource = oDtGI;
            gvGeneralInt.DataBind();
            gvGeneralInt.Enabled = false;

            // 5.0  Commission Setup
            txtComSetNonOrgComm.Text = oSPPolicy.NonOrgCommission.ToString();
            txtComSetNonOrgComm.Enabled = false;
            Util.SetRadioData(rblComSetNonOrgChargeOnPer, oSPPolicy.NonOrgCommissionType.Substring(0, oSPPolicy.NonOrgCommissionType.Length > 1 ? 1 : 0));
            Util.RBLChangeSetColor(rblComSetNonOrgChargeOnPer);
            rblComSetNonOrgChargeOnPer.Enabled = false;
            Util.SetRadioData(rblComSetNonOrgCalculateInt, oSPPolicy.NonOrgCommissionType.Substring(oSPPolicy.NonOrgCommissionType.Length > 1 ? 1 : 0, oSPPolicy.NonOrgCommissionType.Length > 1 ? 1 : 0));
            Util.RBLChangeSetColor(rblComSetNonOrgCalculateInt);
            rblComSetNonOrgCalculateInt.Enabled = false;
            txtComSetOrgCommission.Text = oSPPolicy.OrgCommission.ToString();
            txtComSetOrgCommission.Enabled = false;
            Util.SetRadioData(rblComSetOrgChargeOnPer, oSPPolicy.OrgCommissionType.Substring(0, oSPPolicy.OrgCommissionType.Length > 1 ? 1 : 0));
            Util.RBLChangeSetColor(rblComSetOrgChargeOnPer);
            rblComSetOrgChargeOnPer.Enabled = false;
            Util.SetRadioData(rblComSetOrgCalculateInt, oSPPolicy.OrgCommissionType.Substring(oSPPolicy.OrgCommissionType.Length > 1 ? 1 : 0, oSPPolicy.OrgCommissionType.Length > 1 ? 1 : 0));
            Util.RBLChangeSetColor(rblComSetOrgCalculateInt);
            rblComSetOrgCalculateInt.Enabled = false;
            txtComSetIntRemuneration.Text = oSPPolicy.InterestRemuneration.ToString();
            txtComSetIntRemuneration.Enabled = false;
            Util.SetRadioData(rblComSetIntRemuChargeOnPer, oSPPolicy.InterestRemunerationType.Substring(0, oSPPolicy.InterestRemunerationType.Length > 1 ? 1 : 0));
            Util.RBLChangeSetColor(rblComSetIntRemuChargeOnPer);
            rblComSetIntRemuChargeOnPer.Enabled = false;
            Util.SetRadioData(rblComSetIntRemuCalculateInt, oSPPolicy.InterestRemunerationType.Substring(oSPPolicy.InterestRemunerationType.Length > 1 ? 1 : 0, oSPPolicy.InterestRemunerationType.Length > 1 ? 1 : 0));
            Util.RBLChangeSetColor(rblComSetIntRemuCalculateInt);
            rblComSetIntRemuCalculateInt.Enabled = false;
            txtComSetRemuneration.Text = oSPPolicy.Remuneration.ToString();
            txtComSetRemuneration.Enabled = false;
            Util.SetRadioData(rblComSetRemuChargeOnPer, oSPPolicy.RemunerationType.Substring(0, oSPPolicy.RemunerationType.Length > 1 ? 1 : 0));
            Util.RBLChangeSetColor(rblComSetRemuChargeOnPer);
            rblComSetRemuChargeOnPer.Enabled = false;
            Util.SetRadioData(rblComSetRemuCalculateInt, oSPPolicy.RemunerationType.Substring(oSPPolicy.RemunerationType.Length > 1 ? 1 : 0, oSPPolicy.RemunerationType.Length > 1 ? 1 : 0));
            Util.RBLChangeSetColor(rblComSetRemuCalculateInt);
            rblComSetRemuCalculateInt.Enabled = false;
            txtComSetLevi.Text = oSPPolicy.Levi.ToString();
            txtComSetLevi.Enabled = false;
            Util.SetRadioData(rblComSetLevi, oSPPolicy.LeviType);
            Util.RBLChangeSetColor(rblComSetLevi);
            rblComSetLevi.Enabled = false;
            txtComSetIncomeTax.Text = oSPPolicy.IncomeTax.ToString();
            txtComSetIncomeTax.Enabled = false;
            Util.SetRadioData(rblComSetIncomeTax, oSPPolicy.IncomeTaxType.ToString());
            Util.RBLChangeSetColor(rblComSetIncomeTax);
            rblComSetIncomeTax.Enabled = false;
            txtComSetIncomeTaxAbove.Text = oSPPolicy.IncomeTaxApplyAmount.ToString();
            txtComSetIncomeTaxAbove.Enabled = false;
            Util.SetCheckData(chkYearly, oSPPolicy.IncomeTaxYearlyYN);
            Util.ChkChangeSetColor(chkYearly);
            chkYearly.Enabled = false;
            txtSocialSecurityAmount.Text = oSPPolicy.SocialSecurityAmount.ToString();
            txtSocialSecurityAmount.Enabled = false;
            Util.SetRadioData(rblSocialSecurityAmount, oSPPolicy.SocialSecurityAmountType.ToString());
            Util.RBLChangeSetColor(rblSocialSecurityAmount);
            rblSocialSecurityAmount.Enabled = false;

            // 6.0  Eligibility 
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
            DataRow rowEli = null;

            for (int i = 0; i < oSPPolicy.PaymentPolicy.Count; i++)
            {
                rowEli = oDtEli.NewRow();
                rowEli["bfCustActiType"] = oSPPolicy.PaymentPolicy[i].ActivityTypeID + " : " + oSPPolicy.PaymentPolicy[i].ActivityTypeValue;
                rowEli["bfPayModeType"] = oSPPolicy.PaymentPolicy[i].PaymentModeID + " : " + oSPPolicy.PaymentPolicy[i].PaymentModeValue;

                oDtEli.Rows.Add(rowEli);
            }
            gvEliPaymentPolicy.DataSource = oDtEli;
            gvEliPaymentPolicy.DataBind();
            gvEliPaymentPolicy.Enabled = false;

            // end of Eligibility grid..
            DDListUtil.Assign(ddlApplicableSex, oSPPolicy.SupportedSex);
            ddlApplicableSex.Enabled = false;
            txtMinimumAge.Text = oSPPolicy.MinimumAge.ToString();
            txtMinimumAge.Enabled = false;
            txtMaximumAge.Text = oSPPolicy.MaximumAge.ToString();
            txtMaximumAge.Enabled = false;
        }
    }
}