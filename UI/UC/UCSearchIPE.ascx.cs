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
using SBM_WebUI.mp;
using SBM_BLC1.Entity.Transaction;
using SBM_BLC1.Transaction;

namespace SBM_WebUI.UI.UC
{
    public partial class UCSearchIPE : System.Web.UI.UserControl
    {
        public string Label1 = string.Empty;
        public string PageTitle = string.Empty;
        public string Type = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitializeData();
            }
        }

        private void InitializeData()
        {
            lblTitle.Text = PageTitle;
            lblTransType.Text = Label1;

            gvSearchList.PageSize = (int)Constants.PAGING_SEARCH;

            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlSPType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);

            Util.RBLChangeSetColor(rblStatus);
            Util.RBLChangeColor(rblStatus);

            Clear();
        }

        protected void gvSearchList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvSearchList, null);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Issue oIssue = new Issue();
            IssueDAL oIssueDAL = new IssueDAL();
            
            gvSearchList.DataSource = null;
            gvSearchList.DataBind();

            Result oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(txtFromDate.Text), txtTransNo.Text, null, rblStatus.SelectedItem.Value, Type, Util.GetDateTimeByString(txtToDate.Text).ToString(), null, null, oConfig.DivisionID, oConfig.BankCodeID);
            ScriptManager.RegisterStartupScript(this.upSuccess, typeof(string), Constants.POPUP_WINDOW, " HideProgressStatus('ctl00_cphDet_SearchIPE_lblProgress') ", true);
            if (oResult.Status)
            {
                DataTable dtSPDataList = (DataTable)oResult.Return;
                if (dtSPDataList.Rows.Count > 0)
                {
                    gvSearchList.DataSource = dtSPDataList;
                    gvSearchList.DataBind();

                    Session[Constants.SES_CONFIG_APPROVE_DATA] = dtSPDataList;
                }
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            Clear();
        }

        public void Clear()
        {
            //DropDown
            ddlSPType.SelectedIndex = 0;
            //TextBox
            txtFromDate.Text = string.Empty;
            txtToDate.Text = string.Empty;
            txtRegNo.Text = string.Empty;
            txtTransNo.Text = string.Empty;
            //Grid
            gvSearchList.DataSource = null;
            gvSearchList.DataBind();
            Session[Constants.SES_CONFIG_APPROVE_DATA] = null;
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
                oMethodParameters[2] = rblStatus.SelectedItem.Value; 

                try
                {
                    Clear();
                    Page.GetType().InvokeMember("IPELoadAction", BindingFlags.InvokeMethod, null, this.Page, oMethodParameters);
                }
                catch (TargetInvocationException TIE)
                {
                    // nothing.. 
                }
            }            
        }
    }
}