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
using SBM_BLC1.Transaction;
using SBM_BLC1.Entity.Transaction;
using SBM_BLC1.DAL.Common;
using SBM_BLC1.DAL.JournalRecon;

namespace SBM_WebUI.UI.UC
{
    public partial class UCReconAuto : System.Web.UI.UserControl
    {
        public string Type = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
            } 
        }


        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)Session[Constants.SES_CONFIG_APPROVE_DATA];
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            JournalDAL oJournalDAL = new JournalDAL();
            Result oResult = null;
            oResult = oJournalDAL.UpdateData(dt, oConfig.DivisionID, oConfig.BankCodeID);
            if (oResult.Status)
            {
                lblProgress.Text = "updated successfully";
            }
            else
            {
                lblProgress.Text = "error";
            }
        }
            

        protected void btnCheck_Click(object sender, EventArgs e)
        {
            //JournalDAL oJournalDAL = new JournalDAL();
            //Result oResult = null;
            //DateTime dtBalanceDate = Util.GetDateTimeByString(txtBalanceDate.Text);
            //Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            //oResult = oJournalDAL.GetJournals(txtRefNo.Text, chkArchive.Checked, dtBalanceDate, oConfig.DivisionID, oConfig.BankCodeID);
            //ScriptManager.RegisterStartupScript(this.upSuccess, typeof(string), Constants.POPUP_WINDOW, " HideProgressStatus('ctl00_cphDet_JBalance_lblProgress') ", true);
            
            //txtTotalDebitAmount.Text = "";
            //txtTotalCreditAmount.Text = "";
            //txtTotalNoofDebitTransaction.Text = "";
            //txtTotalNoofCreditTransaction.Text = "";
            //txtTotalBalance.Text = "";
            
            //if (oResult.Status)
            //{
            //    DataTable dtTmpList = (DataTable)oResult.Return;
            //    if (dtTmpList.Rows.Count > 0)
            //    {
            //        DataTable dtClone = dtTmpList.Clone();
            //        dtClone = dtTmpList.Copy();
            //        Session[Constants.SES_CONFIG_APPROVE_DATA] = dtClone;

            //        dtTmpList.Columns.Remove("CheckerID");

            //        gvSearchList.DataSource = dtTmpList;
            //        gvSearchList.DataBind();


            //        // detail section
            //        object sumDrObj;
            //        sumDrObj = dtTmpList.Compute("Sum(Amount)", "DrCr = 'D' ");
            //        object sumCrObj;
            //        sumCrObj = dtTmpList.Compute("Sum(Amount)", "DrCr = 'C'  ");

            //        object sumDrCountObj;
            //        sumDrCountObj = dtTmpList.Compute("Count(Amount)", "DrCr = 'D' ");

            //        object sumCrCountObj;
            //        sumCrCountObj = dtTmpList.Compute("Count(Amount)", "DrCr = 'C' ");


            //        txtTotalDebitAmount.Text = sumDrObj.ToString();
            //        txtTotalCreditAmount.Text = sumCrObj.ToString();

            //        txtTotalNoofDebitTransaction.Text = sumDrCountObj.ToString();
            //        txtTotalNoofCreditTransaction.Text = sumCrCountObj.ToString();

            //        txtTotalBalance.Text = Convert.ToString(Convert.ToDecimal(sumCrObj.ToString()) + Convert.ToDecimal(sumCrObj.ToString()));
            //    }
            //    else
            //    {
            //        gvSearchList.DataSource = null;
            //        gvSearchList.DataBind();
            //    }
            //}
        }
    }
}