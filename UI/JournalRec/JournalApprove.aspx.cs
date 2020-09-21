using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SBM_BLC1.Common;
using System.Collections;
using System.Data;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.DAL.JournalRecon;

namespace SBM_WebUI.mp
{
    public partial class JournalApprove : System.Web.UI.Page
    {

        #region InitializeData

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Constants.SES_USER_CONFIG] != null)
            {
                if (!Page.IsPostBack)
                {
                    Util.InvalidateSession();
                    Initialization();
                    //This is for Page Permission
                    CheckPermission chkPer = new CheckPermission();
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    //if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_JOURNAL_REC.JOURNAL))
                    //{
                    //    Response.Redirect(Constants.PAGE_ERROR, false);
                    //}
                    //End Of Page Permission
                }
            }
            else
            {
                Response.Redirect(Constants.PAGE_LOGIN, false);
            }
        }

        private void Initialization()
        {
            gvData.Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            hdLoginUserName.Value = oConfig.UserName;
            string sType = Request.QueryString["pType"];
            if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_JOURNAL_RECON.JOURNAL_MANUAL).PadLeft(5, '0')))
            {
                lblRefNo.Visible = false;
                ddlReferenceNum.Visible = false;
            }
            if (Session[Constants.SES_CONFIG_UNAPPROVE_DATA] == null)
            {
                Session.Add(Constants.SES_CONFIG_UNAPPROVE_DATA, new DataTable());
            }
            else
            {
                Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = new DataTable();
            }
            DDListUtil.LoadDDLFromXML(ddlAccNumbe, "JournalAccount", "Type", "JournalAccount", false);
        }

        #endregion InitializeData



        #region Basic Operational Function from control EVENT

        private void SearchAction()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            string sType = Request.QueryString["pType"];
            gvData.DataSource = null;
            gvData.DataBind();

            txtTotalDebitAmount.Text = "";
            txtTotalCreditAmount.Text = "";
            txtTotalNoofDebitTransaction.Text = "";
            txtTotalNoofCreditTransaction.Text = "";
            txtTotalBalance.Text = "";

            if (!string.IsNullOrEmpty(sType) && oConfig !=null )
            {
                Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = new DataTable();
                if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_JOURNAL_RECON.JOURNAL_RECON).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Accounting Journal Approval Queue List";
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Accounting Journal' record found");
                    gvData.EmptyDataTemplate = tbGvData;
                    JournalDAL oJournalDAL = new JournalDAL();

                    DateTime dtFromDate = Util.GetDateTimeByString(txtRecFromDate.Text);
                    DateTime dtToDate = Util.GetDateTimeByString(txtRecToDate.Text);
                    string sAccountNmbr  = ddlAccNumbe.SelectedValue;
                    string sReferenceNmbr = ddlReferenceNum.SelectedValue;

                    Result oResult = oJournalDAL.LoadUnapprovedAccountingJournalList(sAccountNmbr, sReferenceNmbr, dtFromDate, dtToDate, oConfig.DivisionID, oConfig.BankCodeID);

                    if (oResult.Status)
                    {
                        DataTable dtTmpList = (DataTable)oResult.Return;
                        if (dtTmpList != null)
                        {
                            if (dtTmpList.Rows.Count > 0)
                            {
                                gvData.DataSource = dtTmpList;
                                gvData.DataBind();

                                Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;
                                
                                // detail section
                                object sumDrObj;
                                sumDrObj = dtTmpList.Compute("Sum(Amount)", "DrCr = 'D' and IsReconciled=1 ");
                                sumDrObj = sumDrObj.ToString() == "" ? 0 : sumDrObj;

                                object sumCrObj;
                                sumCrObj = dtTmpList.Compute("Sum(Amount)", "DrCr = 'C' and IsReconciled=1 ");
                                sumCrObj = sumCrObj.ToString() == "" ? 0 : sumCrObj;

                                object sumDrCountObj;
                                sumDrCountObj = dtTmpList.Compute("Count(Amount)", "DrCr = 'D' and IsReconciled=1 ");

                                object sumCrCountObj;
                                sumCrCountObj = dtTmpList.Compute("Count(Amount)", "DrCr = 'C' and IsReconciled=1 ");
                                

                                txtTotalDebitAmount.Text = sumDrObj.ToString();
                                txtTotalCreditAmount.Text = sumCrObj.ToString();

                                txtTotalNoofDebitTransaction.Text = sumDrCountObj.ToString();
                                txtTotalNoofCreditTransaction.Text = sumCrCountObj.ToString();

                                txtTotalBalance.Text = Convert.ToString(Convert.ToDecimal(sumCrObj.ToString()) + Convert.ToDecimal(sumCrObj.ToString()));

                            }
                        }
                    }
                }
                else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_JOURNAL_RECON.JOURNAL_MANUAL).PadLeft(5, '0')))
                {
                    lgText.InnerHtml = "Menual Journal Approval Queue List";
                    TemplateBuilder tbGvData = new TemplateBuilder();
                    tbGvData.AppendLiteralString("No 'Menual Journal' record found");
                    gvData.EmptyDataTemplate = tbGvData;
                    //BranchDAL oBranchDAL = new BranchDAL();
                    JournalDAL oJournalDAL = new JournalDAL();

                    DateTime dtFromDate = Util.GetDateTimeByString(txtRecFromDate.Text);
                    DateTime dtToDate = Util.GetDateTimeByString(txtRecToDate.Text);
                    string sAccountNmbr = ddlAccNumbe.SelectedValue;

                    Result oResult = oJournalDAL.LoadUnapprovedManualAcctJournalList(sAccountNmbr, null, dtFromDate, dtToDate, oConfig.DivisionID, oConfig.BankCodeID);

                    if (oResult.Status)
                    {
                        DataTable dtTmpList = ((SBM_BLC1.Entity.JournalRecon.AccountReconciliation)oResult.Return).DtAccountRec;
                        if (dtTmpList != null)
                        {
                            if (dtTmpList.Rows.Count > 0)
                            {
                                gvData.DataSource = dtTmpList;
                                gvData.DataBind();

                                Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;

                                // detail section
                                object sumDrObj;
                                sumDrObj = dtTmpList.Compute("Sum(Amount)", "DrCr = 'D' and IsReconciled=0 ");
                                sumDrObj = sumDrObj.ToString() == "" ? 0 : sumDrObj;

                                object sumCrObj;
                                sumCrObj = dtTmpList.Compute("Sum(Amount)", "DrCr = 'C' and IsReconciled=0 ");
                                sumCrObj = sumCrObj.ToString() == "" ? 0 : sumCrObj;

                                object sumDrCountObj;
                                sumDrCountObj = dtTmpList.Compute("Count(Amount)", "DrCr = 'D' and IsReconciled=0 ");

                                object sumCrCountObj;
                                sumCrCountObj = dtTmpList.Compute("Count(Amount)", "DrCr = 'C' and IsReconciled=0 ");


                                txtTotalDebitAmount.Text = sumDrObj.ToString();
                                txtTotalCreditAmount.Text = sumCrObj.ToString();

                                txtTotalNoofDebitTransaction.Text = sumDrCountObj.ToString();
                                txtTotalNoofCreditTransaction.Text = sumCrCountObj.ToString();

                                txtTotalBalance.Text = Convert.ToString(Convert.ToDecimal(sumCrObj.ToString()) + Convert.ToDecimal(sumCrObj.ToString()));

                            }
                        }
                    }
                }
            }
        }
        
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SearchAction();
        }
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            JournalDAL oJournalDAL = new JournalDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            string sType = Request.QueryString["pType"];
            if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_JOURNAL_RECON.JOURNAL_RECON).PadLeft(5, '0')))
            {
                DataTable dt = new DataTable();
                DataTable dtTmpList = (DataTable)Session[Constants.SES_CONFIG_UNAPPROVE_DATA];
                int i = 0;
                dt = dtTmpList.Clone();
                foreach (GridViewRow gvr in gvData.Rows)
                {
                    if ((gvr.FindControl("txtRefNo") as TextBox).Text.Trim() != "")
                    {
                        DataRow newRow = dt.NewRow();
                        newRow.ItemArray = dtTmpList.Rows[i].ItemArray;

                        newRow["ReferenceNo"] = (gvr.FindControl("txtRefNo") as TextBox).Text.Trim().ToUpper();
                        newRow["ReceiveDate"] = (gvr.FindControl("txtReceiveDt") as TextBox).Text.Trim();
                        newRow["ReconBy"] = (gvr.FindControl("txtReconBy") as TextBox).Text.Trim().ToUpper();


                        dt.Rows.Add(newRow);
                    }
                    i++;
                }

                Result oResult = (Result)oJournalDAL.ApproveAccountingJournal(dt, oConfig.UserName);
                if (oResult.Status)
                {
                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE, Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_APPROVE, Constants.MSG_TYPE_ERROR);
                }

            }else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_JOURNAL_RECON.JOURNAL_MANUAL).PadLeft(5, '0')))
            {
                DataTable dt = (DataTable)Session[Constants.SES_CONFIG_UNAPPROVE_DATA];
                Result oResult = (Result)oJournalDAL.ApproveManualAcctJournal(dt, oConfig.UserName);
                this.SearchAction();
                if (oResult.Status)
                {
                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE, Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_APPROVE, Constants.MSG_TYPE_ERROR);
                }
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }
        public void PopErrorMsgAction(string sTypeReturn)
        {
            //string sType = Request.QueryString["pType"];
            //if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_JOURNAL_REC.JOURNAL).PadLeft(5, '0')))
            //{
                SearchAction();
            //}
            //else if (sType.Equals(Convert.ToString((int)Constants.PAGEINDEX_JOURNAL_REC.JOURNAL_MANUAL).PadLeft(5, '0')))
            //{

            //}

        }
        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            ArrayList al = new ArrayList();
            // Depends on approval type.. 
            //al.Add(3);
            //Util.GridDateFormat(e, gvData, null, al);
        }

        //protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    gvData.PageIndex = e.NewPageIndex;
        //    if (Session[Constants.SES_CONFIG_UNAPPROVE_DATA] != null)
        //    {
        //        DataTable dtTmpList = (DataTable)Session[Constants.SES_CONFIG_UNAPPROVE_DATA];
        //        gvData.DataSource = dtTmpList;
        //        gvData.DataBind();
        //    }
        //}
        #endregion Basic Operational Function from control EVENT
    }
}
