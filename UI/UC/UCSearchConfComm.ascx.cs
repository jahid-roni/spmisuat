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
    public partial class UCSearchConfComm : System.Web.UI.UserControl
    {
        public string PageCaption = string.Empty;
        public string Caption_1 = string.Empty;
        public string Caption_2 = string.Empty;
        public string Type = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Clear();
                gvSearchList.PageSize = (int)Constants.PAGING_SEARCH;
                lblPageCaption.Text = PageCaption;
                lblCap1.Text = Caption_1;
                lblCap2.Text = Caption_2;

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

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            CustomerType oCustomerType = new CustomerType();
            Result oResult = new Result();
            if (Type.Equals("BankSearch"))
            {
                BankDAL oBDAL = new BankDAL();
                oResult = oBDAL.AppSearchList(txtID.Text, txtName.Text);
            }
            else if (Type.Equals("DivisionSearch"))
            {
                DivisionDAL oDivisionDAL = new DivisionDAL();
                oResult = oDivisionDAL.AppSearchList(txtID.Text, txtName.Text);
            }
            else if (Type.Equals("CurrencySearch"))
            {
                CurrencyDAL oCurrencyDAL = new CurrencyDAL();
                oResult = oCurrencyDAL.AppSearchList(txtID.Text, txtName.Text);
            }
            else if (Type.Equals("BranchSearch"))
            {
                BranchDAL oBranchDAL = new BranchDAL();
                oResult = oBranchDAL.AppSearchList(txtID.Text, txtName.Text);
            }

            ScriptManager.RegisterStartupScript(this.upGv, typeof(string), Constants.POPUP_WINDOW, " HideProgressStatus('ctl00_cphDet_ucSearchConfComm_lblProgress') ", true);
            if (oResult.Status)
            {
                DataTable dtTmpCustomerTypeList = (DataTable)oResult.Return;
                if (dtTmpCustomerTypeList.Rows.Count > 0)
                {
                    gvSearchList.DataSource = dtTmpCustomerTypeList;
                    gvSearchList.DataBind();
                }
                else
                {
                    gvSearchList.DataSource = null;
                    gvSearchList.DataBind();
                }
                Session[Constants.SES_CONFIG_APPROVE_DATA] = dtTmpCustomerTypeList;
            }
            
            ScriptManager.RegisterStartupScript(this.upGv, typeof(string), Constants.POPUP_WINDOW, " HideProgressStatus('ctl00_cphDet_ucSearchConfComm_lblProgress') ", true);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        public void Clear()
        {
            txtID.Text = "";
            txtName.Text = "";

            gvSearchList.DataSource = null;
            gvSearchList.DataBind();
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
                    Page.GetType().InvokeMember("PopLoadAction", BindingFlags.InvokeMethod, null, this.Page, oMethodParameters);
                }
                catch (TargetInvocationException TIE)
                {
                    // nothing.. 
                }
            }
        }
    }
}