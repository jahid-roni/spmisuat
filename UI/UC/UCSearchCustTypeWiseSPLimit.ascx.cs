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
    public partial class CustomerTypeWiseSPLimitSetup : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitializeData();
            } 
        }

        private void InitializeData()
        {
            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlSPType, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
            DDListUtil.LoadDDLFromDB(ddlCustType, "CustomerTypeID", "TypeDesc", "SPMS_CustomerType", true);
            gvSearchList.PageSize = (int)Constants.PAGING_SEARCH;

            gvSearchList.DataSource = null;
            gvSearchList.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            CustomerType oCustomerType = new CustomerType();
            CustomerTypeWiseSPLimitDAL oCustTypeWiseSPLmtDAL = new CustomerTypeWiseSPLimitDAL();

            Result oResult = oCustTypeWiseSPLmtDAL.SearchList(ddlCustType.SelectedValue, ddlSPType.SelectedValue);
            DataTable dtTmpList = null;
            ScriptManager.RegisterStartupScript(this.upSuccess, typeof(string), Constants.POPUP_WINDOW, " HideProgressStatus('ctl00_cphDet_ucSearchCustTypeWiseSPLimit_lblProgress') ", true);
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
            ddlCustType.SelectedIndex = 0;
            ddlSPType.SelectedIndex = 0;
            gvSearchList.DataSource = null;
            gvSearchList.DataBind();
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
                object[] oMethodParameters = new object[2];
                oMethodParameters[0] = gvRow.Cells[1].Text;
                oMethodParameters[1] = gvRow.Cells[3].Text;

                try
                {
                    Page.GetType().InvokeMember("CustTypeWiseSPLimitLoadAction", BindingFlags.InvokeMethod, null, this.Page, oMethodParameters);
                }
                catch (TargetInvocationException TIE)
                {
                    // nothing.. 
                }
            }
        }
    }
}