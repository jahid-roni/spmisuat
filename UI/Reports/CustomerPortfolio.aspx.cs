using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.DAL.Claim;
using SBM_BLC1.Entity.Common;
using CrystalDecisions.Enterprise;
using SBM_BLC1.Transaction;
using SBM_BLV1;
using SBM_BLC1.Common;
using SBM_BLC1.DAL.Common;

namespace SBM_WebUI.mp
{
    public partial class CustomerPortfolio : System.Web.UI.Page
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_TRANS.SP_ISSUE_VIEW))
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

        protected void InitializeData()
        {
            
        }
        public void PopErrorMsgAction(string sType)
        {
            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_TRAN_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_TRANS.SP_ISSUE_UPDATE).PadLeft(5, '0'), false);
            }
            else
            {
                // no action
            }
        }

        protected void btnPortfolio_Click(object sender, EventArgs e)
        {
            ReportDAL rdal = new ReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                // Parameter
                string sCheckList = "";
                string sRptType = "CP";


                oResult = rdal.ExportCustomerPortfolio(sRptType, ddlTransType.SelectedValue.ToString(), txtSearchValue.Text, ddlIssueStatus.SelectedValue.ToString());
                if (oResult.Status)
                {
                    this.ExportToCSV((DataTable)oResult.Return, "");
                }
            }
        }

        protected void btnPresentValue_Click(object sender, EventArgs e)
        {
             ReportDAL rdal = new ReportDAL();
            Result oResult = new Result();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

            if (oConfig != null)
            {
                // Parameter
                string sCheckList = "";
                string sRptType = "PV";


                oResult = rdal.ExportCustomerPortfolio(sRptType, ddlTransType.SelectedValue.ToString(), txtSearchValue.Text, ddlIssueStatus.SelectedValue.ToString());
                if (oResult.Status)
                {
                    this.ExportToCSV((DataTable)oResult.Return, "");
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearchValue.Text))
            {
                if (ddlTransType.SelectedIndex > 0)
                {

                    Result oResult = null;
                    IssueDAL ad= new IssueDAL();

                    if (ddlTransType.SelectedValue != "S")
                    {
                        //if (ddlTransType.SelectedValue.ToString().Contains("OI") && txtSearchValue.Text.Length<12)
                        //{
                        //    ucMessage.OpenMessage("Invalid OWS_ISS_REFID, Serach Test is small to execute. Please Check.", Constants.MSG_TYPE_INFO);
                        //    ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        //    return;
                        //}
                        //else if (ddlTransType.SelectedValue.ToString().Contains("OC") && txtSearchValue.Text.Length < 12)
                        //{
                        //    ucMessage.OpenMessage("Invalid OWS_ISS_REFID, Serach Test is small to execute. Please Check.", Constants.MSG_TYPE_INFO);
                        //    ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        //    return;
                        //}

                        //else if (ddlTransType.SelectedValue.ToString().Contains("ON") && txtSearchValue.Text.Length < 12)
                        //{
                        //    ucMessage.OpenMessage("Invalid OWS_ISS_REFID, Serach Test is small to execute. Please Check.", Constants.MSG_TYPE_INFO);
                        //    ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        //    return;
                        //}

                        //else if (ddlTransType.SelectedValue.ToString().Contains("CN") && txtSearchValue.Text.Length < 12)
                        //{
                        //    ucMessage.OpenMessage("Invalid OWS_ISS_REFID, Serach Test is small to execute. Please Check.", Constants.MSG_TYPE_INFO);
                        //    ScriptManager.RegisterStartupScript(this.UpdatePanel8, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                        //    return;
                        //}

                        oResult = ad.Load_CustomerPortfolio_List(ddlTransType.SelectedValue.ToString(), txtSearchValue.Text, ddlIssueStatus.SelectedValue.ToString());
                    }
                    if (oResult.Status)
                    {
                        gvTransactionList.DataSource = oResult.Return;
                        gvTransactionList.DataBind();
                    }
                    else
                    {
                        DataTable dt1 = new DataTable();
                        gvTransactionList.DataSource = dt1;
                        gvTransactionList.DataBind();
                    }
                }
                else
                {
                    DataTable dt2 = new DataTable();
                    gvTransactionList.DataSource = dt2;
                    gvTransactionList.DataBind();

                }
            }
            else
            {
                DataTable dt3 = new DataTable();
                gvTransactionList.DataSource = dt3;
                gvTransactionList.DataBind();
            }
        }

        protected void ddlTransType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //btnDeleteAll.Visible = false;
            //if (ddlTransType.SelectedIndex > 0)
            //{
            //    if (ddlTransType.SelectedValue == "I")
            //    {
            //        btnDeleteAll.Visible = true;
            //    }
            //}
        }


        protected void gvTransactionList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if ((e.Row.RowType == DataControlRowType.Header) || (e.Row.RowType == DataControlRowType.DataRow))
            {
                if ((e.Row.RowType == DataControlRowType.DataRow))
                {
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        e.Row.Cells[i].Wrap = true;
                        e.Row.Cells[i].BackColor = System.Drawing.Color.Silver;
                    }
                }
                e.Row.Cells[0].Width = new Unit("50px");
                e.Row.Cells[1].Width = new Unit("150px");
                e.Row.Cells[2].Width = new Unit("75px");
                e.Row.Cells[3].Width = new Unit("50px");
                e.Row.Cells[4].Width = new Unit("130px");
                e.Row.Cells[5].Width = new Unit("100px");
                e.Row.Cells[6].Width = new Unit("100px");
                e.Row.Cells[7].Width = new Unit("100px");
                e.Row.Cells[8].Width = new Unit("145px");
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

        protected void gvTransactionList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "View")
            {
                CryptographyManager oCrypManager = new CryptographyManager();
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;

                string urlRef = Constants.PAGE_ISS_VIEW + "?qRegNo=" + oCrypManager.GetEncryptedString(gvRow.Cells[1].Text);
                Page.RegisterStartupScript(Constants.PAGE_WINDOW, Util.OpenViewIssue(urlRef));

            }
        }

        protected void ddlIssueStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSearch_Click(sender, e);
        }
    }
    }
