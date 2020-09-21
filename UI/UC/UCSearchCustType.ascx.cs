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

namespace SBM_WebUI.UI.UC
{
    public partial class UCSearchCustType : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                gvSearchList.PageSize = (int)Constants.PAGING_SEARCH;
                Clear();
            } 
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            CustomerType oCustomerType = new CustomerType();
            CustomerTypeDAL oCustomerTypeDAL = new CustomerTypeDAL();

            Result oResult = oCustomerTypeDAL.SearchList(txtCustomerType.Text , txtDescription.Text,txtNumberOfMaximumMember.Text);
            DataTable dtTmpList = null;
            ScriptManager.RegisterStartupScript(this.upSuccess, typeof(string), Constants.POPUP_WINDOW, " HideProgressStatus('ctl00_cphDet_ucSearchCustType_lblProgress') ", true);
            if (oResult.Status)
            {
                dtTmpList = (DataTable)oResult.Return;
                if (dtTmpList.Rows.Count > 0)
                {
                    gvSearchList.DataSource = dtTmpList;
                    gvSearchList.DataBind();
                }
                else
                {
                    gvSearchList.DataSource = null;
                    gvSearchList.DataBind();
                }
            }
            Session[Constants.SES_CONFIG_APPROVE_DATA] = dtTmpList;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        public void Clear()
        {
            txtCustomerType.Text = "";
            txtDescription.Text = "";
            txtNumberOfMaximumMember.Text = "";

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
                object[] oMethodParameters = new object[1];
                oMethodParameters[0] = gvRow.Cells[1].Text;

                try
                {
                    Page.GetType().InvokeMember("CustTypeLoadAction", BindingFlags.InvokeMethod, null, this.Page, oMethodParameters);
                }
                catch (TargetInvocationException TIE)
                {
                    // nothing.. 
                }
            }
        }
    }
}