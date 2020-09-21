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

namespace SBM_WebUI.UI.UC
{
    public partial class UCSearchStopPayment : System.Web.UI.UserControl
    {
        public string Type = "";
        public string Title = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitializeData();
            }
        }

        private void InitializeData()
        {
            lblTitle.Text = Title;
            gvSearchList.PageSize = (int)Constants.PAGING_SEARCH;
            DDListUtil.LoadDDLFromDB(ddlSPType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            Clear();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            IssueDAL oIssueDAL = new IssueDAL();
            gvSearchList.DataSource = null;
            gvSearchList.DataBind();
            Result oResult = new Result();

            if (hdPageType.Value == "1.1")
            {
                if (hdCheckDate.Value.Equals("false") || hdCheckDate.Value.Equals(""))
                {
                    oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(""), null, null, rdolApproval.SelectedItem.Value, Type, null, null, null, oConfig.DivisionID, oConfig.BankCodeID);
                }
                else
                {
                    oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(Request[txtIssueDate.UniqueID]), null, null, rdolApproval.SelectedItem.Value, Type, null, null, null, oConfig.DivisionID, oConfig.BankCodeID);
                }
            }
            else if (hdPageType.Value == "2.1")
            {
                if (hdCheckDate.Value.Equals("false") || hdCheckDate.Value.Equals(""))
                {
                    oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(""), null, null, rdolApproval.SelectedItem.Value, Type, null, null, null, oConfig.DivisionID, oConfig.BankCodeID);
                }
                else
                {
                    oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(Request[txtIssueDate.UniqueID]), null, null, rdolApproval.SelectedItem.Value, Type, null, null, null, oConfig.DivisionID, oConfig.BankCodeID);
                }
            }
            else if (hdPageType.Value == "2.2")
            {
                if (hdCheckDate.Value.Equals("false") || hdCheckDate.Value.Equals(""))
                {
                    oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(""), null, null, rdolApproval.SelectedItem.Value, "14", null, null, null, oConfig.DivisionID, oConfig.BankCodeID);
                }
                else
                {
                    oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(Request[txtIssueDate.UniqueID]), null, null, rdolApproval.SelectedItem.Value, "14", null, null, null, oConfig.DivisionID, oConfig.BankCodeID);          
                }
            }
            else if (hdPageType.Value == "3.1")
            {
                if (hdCheckDate.Value.Equals("false") || hdCheckDate.Value.Equals(""))
                {
                    oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(""), null, null, rdolApproval.SelectedItem.Value, Type, null, null, null, oConfig.DivisionID, oConfig.BankCodeID);
                }
                else
                {
                    oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(Request[txtIssueDate.UniqueID]), null, null, rdolApproval.SelectedItem.Value, Type, null, null, null, oConfig.DivisionID, oConfig.BankCodeID);
                }
            }
            else if (hdPageType.Value == "3.2")
            {
                if (hdCheckDate.Value.Equals("false") || hdCheckDate.Value.Equals(""))
                {
                    oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(""), null, null, rdolApproval.SelectedItem.Value, "14", "D", null, null, oConfig.DivisionID, oConfig.BankCodeID);
                }
                else
                {
                    oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(Request[txtIssueDate.UniqueID]), null, null, rdolApproval.SelectedItem.Value, "14", null, null, null, oConfig.DivisionID, oConfig.BankCodeID);
                }
            }
            else if (hdPageType.Value == "4.2")
            {
                if (hdCheckDate.Value.Equals("false") || hdCheckDate.Value.Equals(""))
                {
                    oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(""), null, null, rdolApproval.SelectedItem.Value, Type, null, null, null, oConfig.DivisionID, oConfig.BankCodeID);
                }
                else
                {
                    oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(Request[txtIssueDate.UniqueID]), null, null, rdolApproval.SelectedItem.Value, Type, null, null, null, oConfig.DivisionID, oConfig.BankCodeID);
                }
            }
            else if (hdPageType.Value == "5.2")
            {
                if (hdCheckDate.Value.Equals("false") || hdCheckDate.Value.Equals(""))
                {
                    oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(""), null, null, rdolApproval.SelectedItem.Value, Type, null, null, null, oConfig.DivisionID, oConfig.BankCodeID);
                }
                else
                {
                    oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(Request[txtIssueDate.UniqueID]), null, null, rdolApproval.SelectedItem.Value, Type, null, null, null, oConfig.DivisionID, oConfig.BankCodeID);
                }
            }

            if (hdCheckDate.Value.Equals("false") || hdCheckDate.Value.Equals(""))
            {
                txtIssueDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            }

            ScriptManager.RegisterStartupScript(this.upSuccess, typeof(string), Constants.POPUP_WINDOW, " HideProgressStatus('ctl00_cphDet_StopPay_lblProgress') ", true);

            if (oResult.Status)
            {
                DataTable dtIssueList = (DataTable)oResult.Return;
                if (dtIssueList.Rows.Count > 0)
                {
                    gvSearchList.DataSource = dtIssueList;
                    gvSearchList.DataBind();
                    Session[Constants.SES_CONFIG_APPROVE_DATA] = dtIssueList;
                }
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            Clear();
        }

        public void Clear()
        {
            txtIssueDate.Text = string.Empty;
            txtRegNo.Text = string.Empty;
            ddlSPType.SelectedIndex = 0;
            gvSearchList.DataSource = null;
            gvSearchList.DataBind();
            txtIssueDate.Text = DateTime.Now.ToString(Constants.DATETIME_FORMAT);
            Session[Constants.SES_CONFIG_APPROVE_DATA] = null;
        }

        protected void gvSearchList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvSearchList, null);
        }

        protected void gvSearchList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSearchList.PageIndex = e.NewPageIndex;
            if (Session[Constants.SES_CONFIG_APPROVE_DATA] != null)
            {
                DataTable dtTmpList = (DataTable)Session[Constants.SES_CONFIG_APPROVE_DATA];
                gvSearchList.DataSource = dtTmpList;
                gvSearchList.DataBind();
            }
        }

        protected void gvSearchList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("Page"))
            {
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                object[] oMethodParameters = new object[3];
                oMethodParameters[0] = gvRow.Cells[1].Text;
                oMethodParameters[1] = gvRow.Cells[2].Text;
                oMethodParameters[2] = rdolApproval.SelectedItem.Value;
                Clear();
                try
                {
                    Page.GetType().InvokeMember("PopupStopPaySearchLoadAction", BindingFlags.InvokeMethod, null, this.Page, oMethodParameters);
                }
                catch (TargetInvocationException TIE)
                {
                    // nothing.. 
                }
            }
        }
    }
}