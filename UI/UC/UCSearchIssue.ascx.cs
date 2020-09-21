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
    public partial class UCSearchIssue : System.Web.UI.UserControl
    {
        public string Type = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitializeData();

                for (int i = 0; i <= rdolApproval.Items.Count - 1; i++)
                {
                    rdolApproval.Items[i].Attributes["onClick"] = string.Format("CheckIssueStatus( this ); ");
                }
            } 
        }

        private void InitializeData()
        {
            DDListUtil.LoadDDLFromDB(ddlSPType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            gvSearchList.PageSize = (int)Constants.PAGING_SEARCH;
            
            rdolApproval.Items[0].Enabled = true;
            rdolApproval.Items[1].Enabled = true;

            if (this.Type.Equals("2") || this.Type.Equals("4") || this.Type.Equals("5") || this.Type.Equals("9") || this.Type.Equals("8") || this.Type.Equals("11") || this.Type.Equals("12"))
            //Issue Update(4) + stop payment(5) + LM(9) + "Interest Payment(8) + Encashment (11) "
            {
                rdolApproval.Items[0].Enabled = false;
                rdolApproval.Items[0].Selected = false;
                rdolApproval.Items[1].Enabled = true;
                rdolApproval.Items[1].Selected = true;
            }
            Clear();            
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            IssueDAL oIssueDAL = new IssueDAL();
            Result oResult = null;
            if (rdolApproval.Items[0].Selected)
            {
                oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(txtIssueDate.Text), txtScripNo.Text, "", rdolApproval.SelectedItem.Value, Type, null, null, null, oConfig.DivisionID, oConfig.BankCodeID);
            }
            else
            {
                oResult = oIssueDAL.SearchList(txtRegNo.Text, ddlSPType.SelectedValue, Util.GetDateTimeByString(txtIssueDate.Text), txtScripNo.Text, Util.GetCheckListIDList(chklIssueStatus), rdolApproval.SelectedItem.Value, Type, null, null, null, oConfig.DivisionID, oConfig.BankCodeID);
            }            
            
            ScriptManager.RegisterStartupScript(this.upSuccess, typeof(string), Constants.POPUP_WINDOW, " HideProgressStatus('ctl00_cphDet_SIssue_lblProgress') ", true);
            if (oResult.Status)
            {
                DataTable dtTmpList = (DataTable)oResult.Return;
                if (dtTmpList.Rows.Count > 0)
                {
                    gvSearchList.DataSource = dtTmpList;
                    gvSearchList.DataBind();

                    Session[Constants.SES_CONFIG_APPROVE_DATA] = dtTmpList;
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
            txtScripNo.Text = string.Empty;
            ddlSPType.SelectedIndex = 0;
            gvSearchList.DataSource = null;
            gvSearchList.DataBind();
            foreach (ListItem boxItem in chklIssueStatus.Items)
            {
                boxItem.Selected = true;
            }
            Session[Constants.SES_CONFIG_APPROVE_DATA] = null;
            txtRegNo.Focus();
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
                if (this.Type.Equals("5")) // STOP PAYMENT
                {
                    oMethodParameters[0] = gvRow.Cells[1].Text;
                    oMethodParameters[1] = string.Empty;//gvRow.Cells[6].Text;
                    oMethodParameters[2] = rdolApproval.SelectedItem.Value;
                }
                else
                {
                    oMethodParameters[0] = gvRow.Cells[1].Text;
                    oMethodParameters[1] = gvRow.Cells[2].Text;
                    oMethodParameters[2] = rdolApproval.SelectedItem.Value;
                }
                try
                {
                    Page.GetType().InvokeMember("PopupIssueSearchLoadAction", BindingFlags.InvokeMethod, null, this.Page, oMethodParameters);
                }
                catch (TargetInvocationException TIE)
                {
                    // nothing.. 
                }
            }
        }
    }
}