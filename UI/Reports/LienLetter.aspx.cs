using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.Transaction;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Entity.Transaction;
using System.Collections;
using SBM_BLC1.DAL.Common;

namespace SBM_WebUI.mp
{
    public partial class LienLetter : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_REPORT.LIEN_LETTER))
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
            gvData.DataSource = null;
            gvData.DataBind();
            // Dropdown load SPType
            txtLienDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            Util.RBLChangeSetColor(rdoLetterType);
            Util.RBLChangeColor(rdoLetterType);
        }
        #endregion InitializeData

        // Line Mark Remove call back function
        public void PopupStopPaySearchLoadAction(string sLienRemoveTransNo, string sRegNo, string sApprovalStaus)
        {
            if (!string.IsNullOrEmpty(sRegNo))
            {
                LoadDataByRegNo(sLienRemoveTransNo, sRegNo, sApprovalStaus, "LMR");
            }
        }

        // Line call back function
        public void PopupLienMarkSearchLoadAction(string sLienMarkTransNo, string sRegNo, string sApprovalStaus)
        {
            if (!string.IsNullOrEmpty(sRegNo))
            {
                LoadDataByRegNo(sLienMarkTransNo, sRegNo, sApprovalStaus, "LM");
            }
        }

        private void LoadDataByRegNo(string sTransNo, string sRegNo, string sApprovalStaus, string sType)
        {
            LienDAL oLienDAL = new LienDAL();
            Result oResult = null;

            TotalClear();

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Reg No", typeof(string)));
            dt.Columns.Add(new DataColumn("Issue Date", typeof(string)));
            dt.Columns.Add(new DataColumn("Bond Holder", typeof(string)));
            dt.Columns.Add(new DataColumn("Certificate No", typeof(string)));
            dt.Columns.Add(new DataColumn("Denomination", typeof(string)));
            dt.Columns.Add(new DataColumn("Amount", typeof(string)));
            
            DataRow row = null;
            ArrayList al = new ArrayList();
            
            if (sType.Equals("LM")) //LienMark search
            {
                oResult = (Result)oLienDAL.LoadLienMarkByTransactionNo(sTransNo, sApprovalStaus);
                if (oResult.Status)
                {
                    Lien oLien = (Lien)oResult.Return;
                    if (oLien != null)
                    {

                        txtLienBank.Text = oLien.LienBank + (oLien.LienBankAddress == "" ? "" : "," + oLien.LienBankAddress);
                        txtOurRefNo.Text = oLien.OurRef;
                        txtThierRefNo.Text = oLien.TheirRef;
                        txtLienTransNo.Text = oLien.LienTransNo;
                        txtLienDate.Text = oLien.LienDate.ToString(Constants.DATETIME_FORMAT);

                        
                        if (oLien.DtLienDetails.Rows.Count > 0)
                        {
                            for (int i = 0; i < oLien.DtLienDetails.Rows.Count; i++)
                            {
                                if (!al.Contains(oLien.DtLienDetails.Rows[i]["SP Series"]))
                                {
                                    row = dt.NewRow();
                                    row["Reg No"] = oLien.Issue.RegNo;
                                    row["Issue Date"] = oLien.Issue.VersionIssueDate.ToString(Constants.DATETIME_dd_MMM_yyyy);
                                    row["Bond Holder"] = oLien.Issue.BondHolderName;
                                    row["Certificate No"] = oLien.DtLienDetails.Rows[i]["SP Series"] + "-" + oLien.DtLienDetails.Rows[i]["SPScripID"];
                                    row["Denomination"] = oLien.DtLienDetails.Rows[i]["Denomination"];
                                    row["Amount"] = oLien.DtLienDetails.Rows[i]["Denomination"];

                                    dt.Rows.Add(row);
                                    al.Add(oLien.DtLienDetails.Rows[i]["SP Series"]);
                                }
                                else
                                {
                                    dt.Rows[al.IndexOf(oLien.DtLienDetails.Rows[i]["SP Series"])]["Amount"] = Convert.ToDouble(dt.Rows[al.IndexOf(oLien.DtLienDetails.Rows[i]["SP Series"])]["Amount"]) + Convert.ToDouble(oLien.DtLienDetails.Rows[i]["Denomination"]);
                                    dt.Rows[al.IndexOf(oLien.DtLienDetails.Rows[i]["SP Series"])]["Certificate No"] = dt.Rows[al.IndexOf(oLien.DtLienDetails.Rows[i]["SP Series"])]["Certificate No"] + ", " + oLien.DtLienDetails.Rows[i]["SP Series"] + "-" + oLien.DtLienDetails.Rows[i]["SPScripID"];
                                }
                            }
                        }

                        if (dt.Rows.Count > 0)
                        {
                            gvData.DataSource = dt;
                        }
                        else
                        {
                            gvData.DataSource = null;
                        }
                        gvData.DataBind();
                    }
                }
            }
            else if (sType.Equals("LMR")) //LienMark remove search
            {
                oResult = (Result)oLienDAL.LoadLienRemoveMarkByRegNo(sTransNo, "", sRegNo, sApprovalStaus);
                if (oResult.Status)
                {
                    LienRemove oLienRemove = (LienRemove)oResult.Return;
                    if (oLienRemove != null)
                    {
                        txtLienBank.Text = oLienRemove.LienBank + (oLienRemove.LienBankAddress == "" ? "" : "," + oLienRemove.LienBankAddress);
                        txtOurRefNo.Text = oLienRemove.OurRef;
                        txtThierRefNo.Text = oLienRemove.TheirRef;
                        //txtLienTransNo.Text = oLienRemove.Lien.LienTransNo;
                        txtLienTransNo.Text = sTransNo;
                        txtLienDate.Text = oLienRemove.Lien.LienDate.ToString(Constants.DATETIME_FORMAT);

                        if (oLienRemove.DtLienRemoveDetail.Rows.Count > 0)
                        {
                            for (int i = 0; i < oLienRemove.DtLienRemoveDetail.Rows.Count; i++)
                            {
                                if (!al.Contains(oLienRemove.DtLienRemoveDetail.Rows[i]["SP Series"]))
                                {
                                    row = dt.NewRow();
                                    row["Reg No"] = oLienRemove.Issue.RegNo;
                                    row["Issue Date"] = oLienRemove.Issue.VersionIssueDate.ToString(Constants.DATETIME_dd_MMM_yyyy);
                                    row["Bond Holder"] = oLienRemove.Issue.BondHolderName;
                                    row["Certificate No"] = oLienRemove.DtLienRemoveDetail.Rows[i]["SP Series"] + "-" + oLienRemove.DtLienRemoveDetail.Rows[i]["SPScripID"];
                                    row["Denomination"] = oLienRemove.DtLienRemoveDetail.Rows[i]["Denomination"];
                                    row["Amount"] = oLienRemove.DtLienRemoveDetail.Rows[i]["Denomination"];

                                    dt.Rows.Add(row);
                                    al.Add(oLienRemove.DtLienRemoveDetail.Rows[i]["SP Series"]);
                                }
                                else
                                {
                                    dt.Rows[al.IndexOf(oLienRemove.DtLienRemoveDetail.Rows[i]["SP Series"])]["Amount"] = Convert.ToDouble(dt.Rows[al.IndexOf(oLienRemove.DtLienRemoveDetail.Rows[i]["SP Series"])]["Amount"]) + Convert.ToDouble(oLienRemove.DtLienRemoveDetail.Rows[i]["Denomination"]);
                                    dt.Rows[al.IndexOf(oLienRemove.DtLienRemoveDetail.Rows[i]["SP Series"])]["Certificate No"] = dt.Rows[al.IndexOf(oLienRemove.DtLienRemoveDetail.Rows[i]["SP Series"])]["Certificate No"] + ", " + oLienRemove.DtLienRemoveDetail.Rows[i]["SP Series"] + "-" + oLienRemove.DtLienRemoveDetail.Rows[i]["SPScripID"];
                                }
                            }
                        }

                        if (dt.Rows.Count > 0)
                        {
                            gvData.DataSource = dt;
                        }
                        else
                        {
                            gvData.DataSource = null;
                        }
                        gvData.DataBind();
                    }
                }
            }
        }

        private void TotalClear()
        {
            txtLienBank.Text= "";
            txtLienTransNo.Text= "";
            txtOurRefNo.Text= "";
            txtThierRefNo.Text= "";
            txtLienDate.Text= "";

            gvData.DataSource = null;
            gvData.DataBind();
        
        }


        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvData, null);
        }


        protected void rdoLetterType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            TotalClear();
        }

        protected void btnPrintPreview_Click(object sender, EventArgs e)
        {
            ReportDAL rdal = new ReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null & gvData.Rows.Count > 0)
            {                
                string sLetterType = rdoLetterType.Items[0].Selected == true ? "Lien" : "Unlien";
                
                oResult = rdal.LienLetter(sLetterType, txtLienTransNo.Text);

                if (oResult.Status)
                {
                    Session["ExportType"] = ddlExportType.SelectedValue;
                    Session[Constants.SES_RPT_DATA] = oResult.Return;
                    Page.RegisterStartupScript(Constants.REPORT_WINDOW, Util.OpenReport());
                }
            }
        }
    }
}
