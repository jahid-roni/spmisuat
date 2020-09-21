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
using System.Collections;

namespace SBM_WebUI.UI.UC
{
    public partial class UCSearchSPReceive : System.Web.UI.UserControl
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
            gvSearchList.PageSize = (int)Constants.PAGING_SEARCH;

            gvSearchList.DataSource = null;
            gvSearchList.DataBind();

            Util.RBLChangeSetColor(rblStatus);
            Util.RBLChangeColor(rblStatus);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Receive oReceive = new Receive();
            ReceiveDAL oReceiveDAL = new ReceiveDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oReceiveDAL.SearchList(ddlSPType.SelectedValue, txtFromDate.Text, txtToDate.Text, txtFromAmount.Text, txtToAmount.Text, rblStatus.SelectedValue, oConfig.DivisionID, oConfig.BankCodeID);
            DataTable dtSPReceiveList = null;
            ScriptManager.RegisterStartupScript(this.upSuccess, typeof(string), Constants.POPUP_WINDOW, " HideProgressStatus('ctl00_cphDet_ucSearchSPReceive_lblProgress') ", true);
            if (oResult.Status)
            {
                dtSPReceiveList = (DataTable)oResult.Return;
                if (dtSPReceiveList.Rows.Count > 0)
                {
                    gvSearchList.DataSource = dtSPReceiveList;
                    gvSearchList.DataBind();
                }
                else
                {
                    gvSearchList.DataSource = null;
                    gvSearchList.DataBind();
                }
            }
            Session[Constants.SES_CONFIG_APPROVE_DATA] = dtSPReceiveList;
        }

        protected void gvSearchList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            ArrayList al = new ArrayList();
            al.Add(3);
            Util.GridDateFormat(e, gvSearchList, null, al);
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
            //DropDown
            ddlSPType.SelectedIndex = 0;
            //TextBox
            txtFromAmount.Text = string.Empty;
            txtToAmount.Text = string.Empty;
            txtFromDate.Text = string.Empty;
            txtToDate.Text = string.Empty;
            //Grid
            gvSearchList.DataSource = null;
            gvSearchList.DataBind();
            Session[Constants.SES_CONFIG_APPROVE_DATA] = null;
        }

        protected void gvSearchList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("Page"))
            {
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                object[] oMethodParameters = new object[2];
                oMethodParameters[0] = gvRow.Cells[1].Text;
                oMethodParameters[1] = rblStatus.SelectedItem.Value;
                
                Clear();
                try
                {
                    Page.GetType().InvokeMember("SPReceiveLoadAction", BindingFlags.InvokeMethod, null, this.Page, oMethodParameters);
                }
                catch (TargetInvocationException TIE)
                {
                    // nothing.. 
                }
            }
        }

    }
}