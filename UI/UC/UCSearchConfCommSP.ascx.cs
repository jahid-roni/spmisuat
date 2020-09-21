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
using System.Collections;

namespace SBM_WebUI.UI.UC
{
    public partial class UCSearchConfCommSP : System.Web.UI.UserControl
    {
        public string PageCaption = string.Empty;
        public string Caption = string.Empty;
        public string Type = string.Empty;
        ArrayList alAddSeperatorIndex = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Clear();
                gvSearchList.PageSize = (int)Constants.PAGING_SEARCH;
                lblPageCaption.Text = PageCaption;
                lblCap.Text = Caption;
                // Dropdown load SPType

                if (Type.Equals("ReportMappingSearch") || 
                        Type.Equals("BBAddressSearch") || 
                        Type.Equals("SPWiseAccountMappingSearch") ||
                        Type.Equals("SPCertificateFormatMappingSearch") ) // rest of all
                {
                    // SP type
                    DDListUtil.LoadDDLFromDB(ddlID, "SPTypeID", "TypeDesc", "SPMS_SPType", true);
                }
                else if (Type.Equals("CommonAccountMappingSearch") || Type.Equals("CurrencywiseAccountMappingSearch") ) 
                {
                    // currency 10
                    DDListUtil.LoadDDLFromDB(ddlID, "CurrencyID", "CurrencyCode", "SPMS_Currency", true); 
                }
            } 
        }
        protected void gvSearchList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvSearchList, alAddSeperatorIndex);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            CustomerType oCustomerType = new CustomerType();
            Result oResult = new Result();
            DataTable dtTmpList = null;

            if (Type.Equals("ReportMappingSearch")) // 08
            {
                SPReportMappingDAL oDAL = new SPReportMappingDAL();
                oResult = oDAL.AppSearchList(ddlID.SelectedValue);
                ScriptManager.RegisterStartupScript(this.upSuccess, typeof(string), Constants.POPUP_WINDOW, " HideProgressStatus('ctl00_cphDet_ucSearchConfComm_lblProgress') ", true);
                if (oResult.Status)
                {
                    dtTmpList = (DataTable)oResult.Return;
                    DataTable dtMappedData = null;

                    dtMappedData = DDListUtil.MapTableWithXML(dtTmpList, "ReportFormatMapping", "ReportType", "SS", 1);
                    dtMappedData = DDListUtil.MapTableWithXML(dtMappedData, "ReportFormatMapping", "ReportType", "CC", 2);
                    dtMappedData = DDListUtil.MapTableWithXML(dtMappedData, "ReportFormatMapping", "ReportType", "IC", 3);
                    dtMappedData = DDListUtil.MapTableWithXML(dtMappedData, "ReportFormatMapping", "ReportType", "EC", 4);

                    gvSearchList.DataSource = dtMappedData;
                    gvSearchList.DataBind();

                    Session[Constants.SES_CONFIG_APPROVE_DATA] = dtMappedData;
                }
            }
            else if (Type.Equals("BBAddressSearch")) // 10
            {
                BBAddressDAL oDAL = new BBAddressDAL();
                oResult = oDAL.AppSearchList(ddlID.SelectedValue);
                if (oResult.Status)
                {
                    dtTmpList = (DataTable)oResult.Return;
                    Session[Constants.SES_CONFIG_APPROVE_DATA] = dtTmpList;
                }
            }
            else if (Type.Equals("CommonAccountMappingSearch"))  // 13
            {
                CommonMappingDAL oDAL = new CommonMappingDAL();
                oResult = oDAL.AppSearchList(ddlID.SelectedValue);
                if (oResult.Status)
                {
                    dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList.Rows.Count > 0)
                    {
                        alAddSeperatorIndex = new ArrayList();
                        alAddSeperatorIndex.Add(3);
                    }
                    Session[Constants.SES_CONFIG_APPROVE_DATA] = dtTmpList;
                }
            }
            else if (Type.Equals("SPWiseAccountMappingSearch")) // 14
            {
                SPTypeWiseAccountMappingDAL oDAL = new SPTypeWiseAccountMappingDAL();
                oResult = oDAL.AppSearchList(ddlID.SelectedValue);
                if (oResult.Status)
                {
                    dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList.Rows.Count > 0)
                    {
                        alAddSeperatorIndex = new ArrayList();
                        alAddSeperatorIndex.Add(2);
                        alAddSeperatorIndex.Add(3);
                        alAddSeperatorIndex.Add(4);
                    }
                }
                Session[Constants.SES_CONFIG_APPROVE_DATA] = dtTmpList;
            }
            else if (Type.Equals("CurrencywiseAccountMappingSearch")) // 15
            {
                CurrencyWiseAccountMappingDAL oDAL = new CurrencyWiseAccountMappingDAL();
                oResult = oDAL.AppSearchList(ddlID.SelectedValue);
                if (oResult.Status)
                {
                    dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList.Rows.Count > 0)
                    {
                        alAddSeperatorIndex = new ArrayList();
                        alAddSeperatorIndex.Add(3);
                        alAddSeperatorIndex.Add(4);
                        alAddSeperatorIndex.Add(5);
                    }
                }
                Session[Constants.SES_CONFIG_APPROVE_DATA] = dtTmpList;
            }
            else if (Type.Equals("SPCertificateFormatMappingSearch")) // 9
            {
                ScripMappingDAL oDAL = new ScripMappingDAL();
                oResult = oDAL.AppSearchList(ddlID.SelectedValue);
                if (oResult.Status)
                {
                    dtTmpList = (DataTable)oResult.Return;
                    dtTmpList = DDListUtil.MapTableWithXML(dtTmpList, "ScriptFormatMapping", "ReportType", "SP", 2);
                    gvSearchList.DataSource = dtTmpList;
                    gvSearchList.DataBind();
                }
                Session[Constants.SES_CONFIG_APPROVE_DATA] = dtTmpList;
            }


            if (!Type.Equals("ReportMappingSearch") && !Type.Equals("SPCertificateFormatMappingSearch") )
            {
                gvSearchList.DataSource = dtTmpList;
                gvSearchList.DataBind();

                Session[Constants.SES_CONFIG_APPROVE_DATA] = dtTmpList;
            }

        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        public void Clear()
        {
            ddlID.SelectedIndex = 0;

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
                //oMethodParameters[0] = gvRow.Cells[1].Text;
                oMethodParameters[0] = gvRow;
                try
                {
                    Page.GetType().InvokeMember("PopLoadActionCommSpAndCur", BindingFlags.InvokeMethod, null, this.Page, oMethodParameters);
                }
                catch (TargetInvocationException TIE)
                {
                    // nothing.. 
                }
            }
        }


    }
}