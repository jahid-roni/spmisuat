using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.DAL.Common;
using SBM_BLC1.Entity.Common;
using System.IO;

namespace SBM_WebUI.mp
{
    public partial class ExportDataDumps : System.Web.UI.Page
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
                    //CheckPermission chkPer = new CheckPermission();
                    //Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    //if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_REPORT.ISSUE_REGISTER))
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

        #region InitializeData
        private void InitializeData()
        {
            // Dropdown load SPType
            DDListUtil.LoadCheckBoxListFromDB(chkLSpType, "SPTypeID", "TypeDesc", "SPMS_SPType");

            for (int i = 0; i < chkLSpType.Items.Count; i++)
            {
                chkLSpType.Items[i].Selected = true;
            }


            txtFromDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            txtToDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            // load Report Type List
            rdlStatus.Items.Add(new ListItem("All Issue Details", "AI"));
            rdlStatus.Items.Add(new ListItem("Live Issue Details", "LI"));
            rdlStatus.Items.Add(new ListItem("Live Customer Details", "LC"));
            rdlStatus.Items.Add(new ListItem("Live Nominee Details", "LN"));
            rdlStatus.Items.Add(new ListItem("Branch wise Commission", "CO"));
            //rdlStatus.Items.Add(new ListItem("Interest Details", "ID"));
            //rdlStatus.Items.Add(new ListItem("Encashment Details", "ED"));
            rdlStatus.Items.Add(new ListItem("Reinvestment Commission", "RC"));
            rdlStatus.Items.Add(new ListItem("Forward Day Interest", "FI"));
            rdlStatus.Items.Add(new ListItem("Forward Day Encashment", "FE"));
            //rdlStatus.Items.Add(new ListItem("Cert Magic Dump", "CM"));
            rdlStatus.Items.Add(new ListItem("Issue Certificate Dump", "ID"));
            rdlStatus.Items.Add(new ListItem("Statement Dump", "SD"));
            //rdlStatus.Items.Add(new ListItem("ACE CCMS Dump", "AC"));
            rdlStatus.Items.Add(new ListItem("Interest Reconciliation", "IR"));
            rdlStatus.Items.Add(new ListItem("Principle Reconciliation", "PR"));
            rdlStatus.Items.Add(new ListItem("Interest Outstanding", "IO"));
            rdlStatus.Items.Add(new ListItem("Principle Outstanding", "PO"));
            rdlStatus.Items.Add(new ListItem("Certificate Print Log", "CP"));
            rdlStatus.Items.Add(new ListItem("User List", "UL"));
            rdlStatus.Items.Add(new ListItem("Security Matrix", "SM"));
            rdlStatus.Items.Add(new ListItem("Sale Document Upload MI", "SU"));
            rdlStatus.Items.Add(new ListItem("Document Upload MI", "DU"));
            rdlStatus.Items.Add(new ListItem("Sales Statement Reference", "SSR"));
            rdlStatus.Items.Add(new ListItem("Interest Claim Reference", "ICR"));
            rdlStatus.Items.Add(new ListItem("Commission Claim Reference", "CCR"));
            rdlStatus.Items.Add(new ListItem("Encashment Claim Reference", "ECR"));
            rdlStatus.Items.Add(new ListItem("Reinvestment Statement Reference", "RSR"));
            //rdlStatus.Items.Add(new ListItem("Sales Statement Reconciliation Advice", "DU"));
            //rdlStatus.Items.Add(new ListItem("Interest Claim Reconciliation Advice", "DU"));
            //rdlStatus.Items.Add(new ListItem("Encashment Claim Reconciliation Advice", "DU"));
            //rdlStatus.Items.Add(new ListItem("Commission Claim Reconciliation Advice", "DU"));
            //rdlStatus.Items.Add(new ListItem("Duplicate Issue Claim Settlement", "DU"));
            //rdlStatus.Items.Add(new ListItem("General Letter", "DU"));
            //rdlStatus.Items.Add(new ListItem("BB Policy Circular", "DU"));
            //rdlStatus.Items.Add(new ListItem("Explanation Letter", "DU"));
            //rdlStatus.Items.Add(new ListItem("Stop Payment Letter", "DU"));
            //rdlStatus.Items.Add(new ListItem("Lien Letter", "DU"));
            //rdlStatus.Items.Add(new ListItem("Other Letter", "DU"));

            rdlStatus.Items[0].Selected = true;

            Util.RBLChangeSetColor(rdlStatus);
            Util.RBLChangeColor(rdlStatus);
        }
        #endregion InitializeData


        protected void btnPrintPreview_Click(object sender, EventArgs e)
        {
            ReportDAL rdal = new ReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                // Parameter
                string sCheckList = "";
                string sRptType = rdlStatus.SelectedValue;
                if (sRptType != "SM" && sRptType != "UL")
                { 
                    sCheckList = Util.GetCheckListIDList(chkLSpType);
                }
                
                
                string dtFromDate = Util.GetDateTimeByString(txtFromDate.Text).ToString("dd-MMM-yyyy");
                string dtToDate = Util.GetDateTimeByString(txtToDate.Text).ToString("dd-MMM-yyyy");


                oResult = rdal.ExportDataDumps(sRptType, sCheckList, dtFromDate, dtToDate,oConfig.DivisionID, oConfig.BankCodeID, oConfig.BranchID);
                if (oResult.Status)
                {
                    this.ExportToCSV((DataTable)oResult.Return, "");
                }
            }
        }
        private void ExportToCSV(DataTable table, string name)
        {
            string sValue = "";
            HttpContext context = HttpContext.Current;
            context.Response.Clear();
            foreach (DataColumn column in table.Columns)
            {
                context.Response.Write(column.ColumnName + ",");
            }
            context.Response.Write(Environment.NewLine);
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (row[i].GetType() == System.Type.GetType("System.DateTime"))
                    {
                        if (row[i] != DBNull.Value)
                        {
                            sValue = ((DateTime)row[i]).ToShortDateString();
                        }
                        else
                        {
                            sValue = "";
                        }
                        sValue = sValue.Replace(",", string.Empty);
                    }
                    else
                    {
                        sValue = row[i].ToString().Replace(",", string.Empty);
                        //sValue = sValue.Replace(";", string.Empty);
                    }

                    sValue = sValue.Replace("\n", string.Empty);
                    sValue = sValue.Replace("\r", string.Empty);
                    sValue = sValue.Replace("\t", string.Empty);
                    sValue = sValue.Replace(",", string.Empty);
                    context.Response.Write(sValue + ",");
                    sValue = "";
                }
                context.Response.Write(Environment.NewLine);
            }
            context.Response.ContentType = "text/csv";
            context.Response.AppendHeader("Content-Disposition", "attachment; filename=" + table.TableName.Trim() + ".csv");
            context.Response.End();
        }
    }
}