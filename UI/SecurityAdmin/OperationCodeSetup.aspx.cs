using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SBM_BLC1.Entity.SecurityAdmin;
using SBM_BLC1.SecurityAdmin;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Common;
using System.Data;

namespace SBM_WebUI.mp
{
    public partial class OperationCodeSetup : System.Web.UI.Page
    {
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_OPERATION_ID = "sOpID";
        public const string OBJ_PAGE_ID = "sPageID";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Constants.SES_USER_CONFIG] != null)
            {
                if (!Page.IsPostBack)
                {
                    InitializeData();
                    //This is for Page Permission
                    CheckPermission chkPer = new CheckPermission();
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_SA.OPERATION_CODE))
                    {
                        Response.Redirect(Constants.PAGE_ERROR, false);
                    }
                    //End Of Page Permission
                }
            }
            else
            {
                Response.Redirect(Constants.PAGE_LOGIN, false);
            }
        }
        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvData.PageIndex = e.NewPageIndex;
            if (Session[Constants.SES_OP_CODE] != null)
            {
                DataTable dtTmpList = (DataTable)Session[Constants.SES_OP_CODE];
                gvData.DataSource = dtTmpList;
                gvData.DataBind();
            }
           
        }

        protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("Page"))
            {
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                LoadDataByID(Util.GetIntNumber(gvRow.Cells[1].Text));
            }
        }
        

        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            
        }
        public void PopErrorMsgAction(string sType)
        {

        }
        protected void btnLoad_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request[txtOperationID.UniqueID].Trim()))
            {
                LoadDataByID(Util.GetIntNumber(Request[txtOperationID.UniqueID].Trim()));
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdOpID.Value))
            {
                OperationCodeDAL oOpcDAL = new OperationCodeDAL();
                Result oResult = (Result)oOpcDAL.Detete(hdOpID.Value);
                if (oResult.Status)
                {
                    this.LoadList();
                    this.ClearTextValue();

                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_DELETE, Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                   
                        ucMessage.OpenMessage(Constants.MSG_ERROR_DELETE, Constants.MSG_TYPE_ERROR);
                    
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_DELETE, Constants.MSG_TYPE_ERROR);
            }
        }
        
        protected void btnSave_Click(object sender, EventArgs e)
        {
            OperationCode oOpc = new OperationCode();
            OperationCodeDAL oOpcDAL = new OperationCodeDAL();

            oOpc.OperationID = Util.GetIntNumber(Request[txtOperationID.UniqueID].Trim());
            oOpc.Description = txtDescription.Text;


            Result oResult = (Result)oOpcDAL.Save(oOpc);

            if (oResult.Status)
            {
                this.LoadList();

                ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
            }
        }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            hdOpID.Value = "";
            ClearTextValue();
        }
        protected void btnFirst_Click(object sender, EventArgs e)
        {
            MoveAction("F");
        }
        protected void btnLast_Click(object sender, EventArgs e)
        {
            MoveAction("L");
        }
        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            MoveAction("P");
        }
        protected void btnNext_Click(object sender, EventArgs e)
        {
            MoveAction("N");
        }
        private void MoveAction(string sType)
        {
            OperationCodeDAL oOpcDAL = new OperationCodeDAL();
            Result oResult = oOpcDAL.LoadMoveData(txtOperationID.Text, sType);
            if (oResult.Status)
            {
                OperationCode oOpc = (OperationCode)oResult.Return;
                if (oOpc != null)
                {
                    ClearTextValue();
                    txtOperationID.Text = oOpc.OperationID.ToString();
                    txtDescription.Text = oOpc.Description.ToString();
                }
                else
                {
                    ucMessage.OpenMessage("Operation Code does not exist", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel7, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Info"), true);
                }

                hdOpID.Value = oOpc.OperationID.ToString();
            }
            else
            {
                ucMessage.OpenMessage("Operation Code does not exist", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel7, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Info"), true);
            }
            
        }
        private void InitializeData()
        {
            gvData.PageSize = (int)Constants.PAGING_UNAPPROVED;

            
            LoadList();
        }
        private void LoadList()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (oConfig != null)
            {

                OperationCode oOpc = new OperationCode();
                OperationCodeDAL oOpcDAL = new OperationCodeDAL();
                Result oResult = oOpcDAL.LoadOperationCodeList();
                gvData.DataSource = null;
                gvData.DataBind();

                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList.Rows.Count > 0)
                    {
                       

                        gvData.DataSource = dtTmpList;
                        gvData.DataBind();
                        Session[Constants.SES_OP_CODE] = dtTmpList;
                    }
                    else
                    {
                        ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
                    }
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
            }
        }
        private void LoadDataByID(int iOpcID)
        {
            OperationCode oOpc = new OperationCode(iOpcID);
            OperationCodeDAL oOpcDAL = new OperationCodeDAL();

            Result oResult = new Result();
            oResult = oOpcDAL.LoadByID(oOpc);
            ClearTextValue();
            if (oResult.Status)
            {
                oOpc = (OperationCode)oResult.Return;
                if (oOpc != null)
                {
                    txtOperationID.Text = oOpc.OperationID.ToString();
                    txtDescription.Text = oOpc.Description.ToString();


                    hdOpID.Value = iOpcID.ToString();
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
            }
        }
        private void ClearTextValue()
        {
            txtOperationID.Text = string.Empty;
            txtDescription.Text = string.Empty;
        }
    }
}
