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
    public partial class UCSearchReinvest : System.Web.UI.UserControl
    {
        public string Type = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitializeData();

                for (int i = 0; i <= rdolApproval.Items.Count - 1; i++)
                {
                    //rdolApproval.Items[i].Attributes["onClick"] = string.Format("CheckIssueStatus( this ) ");
                    rdolApproval.Items[i].Attributes["onClick"] = string.Format("CheckIssueStatus( this ); ");
                 }
            } 
        }

        private void InitializeData()
        {
            DDListUtil.LoadDDLFromDB(ddlSPType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            Clear();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            IssueDAL oIssueDAL = new IssueDAL();
            Result oResult = oIssueDAL.SearchList(null, ddlSPType.SelectedValue, Util.GetDateTimeByString(txtReinvestmentDate.Text), null, Util.GetCheckListIDList(chklIssueStatus), rdolApproval.SelectedItem.Value, Type, txtReferenceNo.Text, txtNewRegistrationNo.Text, txtOldRegistrationNo.Text, oConfig.DivisionID, oConfig.BankCodeID);
            ScriptManager.RegisterStartupScript(this.upSuccess, typeof(string), Constants.POPUP_WINDOW, " HideProgressStatus('ctl00_cphDet_RIS_lblProgress') ", true);
            if (oResult.Status)
            {
                DataTable dtIssueList = (DataTable)oResult.Return;
                if (dtIssueList.Rows.Count > 0)
                {
                    gvSearchList.DataSource = dtIssueList;
                    gvSearchList.DataBind();
                }
                else
                {
                    gvSearchList.DataSource = null;
                    gvSearchList.DataBind();
                }
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            Clear();
        }

        public void Clear()
        {
            txtReferenceNo.Text = string.Empty;
            txtReinvestmentDate.Text = string.Empty;
            txtOldRegistrationNo.Text = string.Empty;
            txtNewRegistrationNo.Text = string.Empty;
            ddlSPType.SelectedIndex = 0;
            gvSearchList.DataSource = null;
            gvSearchList.DataBind();
            foreach (ListItem boxItem in chklIssueStatus.Items)
            {
                boxItem.Selected = true;
            }
        }
        protected void gvSearchList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvSearchList, null);
        }

        protected void gvSearchList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            object[] oMethodParameters = new object[2];
            oMethodParameters[0] = gvRow;// gvRow.Cells[3].Text;
            oMethodParameters[1] = rdolApproval.SelectedItem.Value;
            try
            {
                Page.GetType().InvokeMember("ReinvestmentSearchLoadAction", BindingFlags.InvokeMethod, null, this.Page, oMethodParameters);
            }
            catch (TargetInvocationException TIE)
            {
                // nothing.. 
            }
        }
    }
}