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
    public partial class UCSearchLienMark : System.Web.UI.UserControl
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
            DDListUtil.LoadDDLFromDB(ddlSPType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            Clear();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            IssueDAL oIssueDAL = new IssueDAL();
            Result oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(txtIssueDate.Text), null, null, rdolApproval.SelectedItem.Value, Type, txtOurRefNo.Text, txtTheirRefNo.Text, null, oConfig.DivisionID, oConfig.BankCodeID);
            ScriptManager.RegisterStartupScript(this.upSuccess, typeof(string), Constants.POPUP_WINDOW, " HideProgressStatus('ctl00_cphDet_LienMark_lblProgress')  ", true);
            if (oResult.Status)
            {
                DataTable dtIssueList = (DataTable)oResult.Return;
                if (dtIssueList.Rows.Count > 0)
                {
                    gvSearchList.DataSource = dtIssueList;
                    gvSearchList.DataBind();

                    Session[Constants.SES_CONFIG_APPROVE_DATA] = dtIssueList;
                }
                else
                {
                    gvSearchList.DataSource = null;
                    gvSearchList.DataBind();
                }
            }
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

        protected void btnReset_Click(object sender, EventArgs e)
        {
            Clear();
        }

        public void Clear()
        {
            txtIssueDate.Text = string.Empty;
            txtRegNo.Text = string.Empty;
            txtTheirRefNo.Text = string.Empty;
            txtOurRefNo.Text = string.Empty;

            ddlSPType.SelectedIndex = 0;
            gvSearchList.DataSource = null;
            gvSearchList.DataBind();

            Session[Constants.SES_CONFIG_APPROVE_DATA] = null;
            //rdolApproval.Items[0].Selected = true;
            //rdolApproval.Items[1].Selected = false;
        }

        protected void gvSearchList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvSearchList, null);
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
                    Page.GetType().InvokeMember("PopupLienMarkSearchLoadAction", BindingFlags.InvokeMethod, null, this.Page, oMethodParameters);
                }
                catch (TargetInvocationException TIE)
                {
                    // nothing.. 
                }
            }
        }
    }
}