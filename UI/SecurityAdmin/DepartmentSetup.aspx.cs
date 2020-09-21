/*
 * File name            : DepartmentSetup
 * Author               : A.K.M. Tanjil Hossain,
 *                      : A.K.M. Zahidul Quaium,
 *                      : Tanvir Alam
 *                      : Jerin Afsana
 * Date                 : Sep 01,2014
 * Version              : 1.0
 *
 * Description          : To manage Department 
 *
 * Modification history :
 * Name                         Date                            Desc
 * Tanvir Alam                Sep 01,2014                Business implementation 
 * A.K.M. Zahidul Quaium        February 02,2012                Business implementation              
 * Jerin Afsana                 April    02,2012                Business implementation              
 * Copyright (c) 2012: Softcell Solution Ltd
 */

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
    public partial class DepartmentSetup : System.Web.UI.Page
    {
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_DEPARTMENT_ID = "sDptID";
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_SA.DEPARTMENT))
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

        #region InitializeData

        private void InitializeData()
        {
            gvData.PageSize = (int)Constants.PAGING_UNAPPROVED;

            string dptID = Request.QueryString[OBJ_DEPARTMENT_ID];
            string sPageID = Request.QueryString[OBJ_PAGE_ID];
            txtDepartmentID.ReadOnly = true;
            if (!string.IsNullOrEmpty(dptID))
            {
                dptID = oCrypManager.GetDecryptedString(dptID, Constants.CRYPT_PASSWORD_STRING);
            }
            if (!string.IsNullOrEmpty(sPageID))
            {
                sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
            }

            if (!string.IsNullOrEmpty(dptID) && !string.IsNullOrEmpty(sPageID))
            {
                string sOperationType = sPageID.Substring(4, 1);
                if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
                {
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    LoadDataByID(Util.GetIntNumber(dptID));

                    // general Control
                    Util.ControlEnabled(txtDepartmentID, false); 
                    Util.ControlEnabled(txtDepartmentName, false);

                    

                    // button 
                    Util.ControlEnabled(btnReject, true);
                    Util.ControlEnabled(btnApprove, true);
                    Util.ControlEnabled(btnBack, true);
                    Util.ControlEnabled(btnReset, false);
                    Util.ControlEnabled(btnSave, false);
                    Util.ControlEnabled(btnDelete, false);
                    Util.ControlEnabled(btnLoad, false);
                    Util.ControlEnabled(btnFirst, false);
                    Util.ControlEnabled(btnPrevious, false);
                    Util.ControlEnabled(btnNext, false);
                    Util.ControlEnabled(btnLast, false);


                    #region User-Detail.
                    Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), true);
                    UserDetails oUserDetails = ucUserDet.UserDetail;
                    oUserDetails.CheckerID = oConfig.UserName;
                    oUserDetails.CheckDate = DateTime.Now;
                    ucUserDet.UserDetail = oUserDetails;
                    #endregion User-Detail.

                    fsList.Visible = false;
                }
            }
            else
            {
                // general Control
                Util.ControlEnabled(txtDepartmentID, true); 
                Util.ControlEnabled(txtDepartmentName, true);

                // button 
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);
                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);
                Util.ControlEnabled(btnLoad, true);
                Util.ControlEnabled(btnFirst, true);
                Util.ControlEnabled(btnPrevious, true);
                Util.ControlEnabled(btnNext, true);
                Util.ControlEnabled(btnLast, true);

                #region User-Detail.
                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), false);
                UserDetails oUserDetails = new UserDetails();
                oUserDetails.MakeDate = DateTime.Now;
                ucUserDet.UserDetail = oUserDetails;
                #endregion User-Detail.

                fsList.Visible = true;
            }
            LoadList();
        }

        #endregion InitializeData

        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvData.PageIndex = e.NewPageIndex;
            if (Session[Constants.SES_CONFIG_UNAPPROVE_DATA] != null)
            {
                DataTable dtTmpList = (DataTable)Session[Constants.SES_CONFIG_UNAPPROVE_DATA];
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
            Util.GridDateFormat(e, gvData, null);
        }

        public void PopErrorMsgAction(string sType)
        {
            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_SA_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_SA.DEPARTMENT).PadLeft(5, '0'), false);
            }
            else
            {
                // no action
            }
        }


        protected void btnBack_Click(object sender, EventArgs e) 
        {
            Response.Redirect(Constants.PAGE_SA_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_SA.DEPARTMENT).PadLeft(5, '0'), false);
        }

        protected void btnReject_Click(object sender, EventArgs e) 
        {
            if (!string.IsNullOrEmpty(hdDptID.Value))
            {
                Department oDpt = new Department(Util.GetIntNumber(hdDptID.Value));
                DepartmentDAL oDptDAL = new DepartmentDAL();
                oDpt.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oDptDAL.Reject(oDpt);
                if (oResult.Status)
                {
                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_REJECT, Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_REJECT, Constants.MSG_TYPE_ERROR);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_REJECT, Constants.MSG_TYPE_ERROR);
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdDptID.Value))
            {
                Department oDpt = new Department(Util.GetIntNumber(hdDptID.Value));
                DepartmentDAL oDptDAL = new DepartmentDAL();
                oDpt.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oDptDAL.Approve(oDpt);
                if (oResult.Status)
                {
                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_APPROVE, Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_APPROVE, Constants.MSG_TYPE_ERROR);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_APPROVE, Constants.MSG_TYPE_ERROR);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            hdDptID.Value = "";
            ClearTextValue();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Department oDpt = new Department();
            DepartmentDAL oDptDAL = new DepartmentDAL();
            
            oDpt.DepartmentID = Util.GetIntNumber(Request[txtDepartmentID.UniqueID].Trim());
            oDpt.Description = txtDepartmentName.Text;
            oDpt.UserDetails = ucUserDet.UserDetail; 

            Result oResult = (Result)oDptDAL.Save(oDpt);

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

        protected void btnFirst_Click(object sender, EventArgs e)
        {
            MoveAction("F");
        }
        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            MoveAction("P");
        }
        protected void btnNext_Click(object sender, EventArgs e)
        {
            MoveAction("N");
        }
        protected void btnLast_Click(object sender, EventArgs e)
        {
            MoveAction("L");
        }

        private void MoveAction(string sType)
        {
            DepartmentDAL oDptDAL = new DepartmentDAL();
            Result oResult = oDptDAL.LoadMoveData(txtDepartmentID.Text, sType);
            if (oResult.Status)
            {
                Department oDpt = (Department)oResult.Return;
                if (oDpt != null)
                {
                    ClearTextValue();
                    txtDepartmentID.Text = oDpt.DepartmentID.ToString();
                    txtDepartmentName.Text = oDpt.Description.ToString();
                }
                ucUserDet.UserDetail = oDpt.UserDetails;
                hdDptID.Value = oDpt.DepartmentID.ToString();
            }
        }
        
        // ok
        protected void btnDelete_Click(object sender, EventArgs e) 
        {
            if (!string.IsNullOrEmpty(hdDptID.Value))
            {
                DepartmentDAL oDptDAL = new DepartmentDAL();
                Result oResult = (Result)oDptDAL.Detete(hdDptID.Value);
                if (oResult.Status)
                {
                    this.LoadList();
                    this.ClearTextValue();

                    ucMessage.OpenMessage(Constants.MSG_SUCCESS_DELETE, Constants.MSG_TYPE_SUCCESS);
                }
                else
                {
                    if (oResult.Message.Equals(Constants.TABLE_MAIN))
                    {
                        ucMessage.OpenMessage(Constants.MSG_APPROVED_DELETE_DATA, Constants.MSG_TYPE_ERROR);
                    }
                    else
                    {
                        ucMessage.OpenMessage(Constants.MSG_ERROR_DELETE, Constants.MSG_TYPE_ERROR);
                    }
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_DELETE, Constants.MSG_TYPE_ERROR);
            }
        }
        
        // ok
        private void ClearTextValue()
        {
            txtDepartmentID.Text = string.Empty;
            txtDepartmentName.Text = string.Empty;
        }

        // ok
        private void LoadList()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (oConfig != null)
            {  

                Department oDpt = new Department();
                DepartmentDAL oDptDAL = new DepartmentDAL();
                Result oResult = oDptDAL.LoadUnapprovedList(oConfig.UserName, false);
                gvData.DataSource = null;
                gvData.DataBind();

                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList.Rows.Count > 0)
                    {
                        dtTmpList.Columns.Remove("MakerID");

                        gvData.DataSource = dtTmpList;
                        gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;
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

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request[txtDepartmentID.UniqueID].Trim()))
            {
                LoadDataByID(Util.GetIntNumber(Request[txtDepartmentID.UniqueID].Trim()));
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }


        private void LoadDataByID(int iDptID)
        {
            Department oDpt = new Department(iDptID);
            DepartmentDAL oDptDAL = new DepartmentDAL();

            Result oResult = new Result();
            oResult = oDptDAL.LoadByID(oDpt);
            ClearTextValue();
            if (oResult.Status)
            {
                oDpt = (Department)oResult.Return;
                if (oDpt != null)
                {
                    txtDepartmentID.Text = oDpt.DepartmentID.ToString();
                    txtDepartmentName.Text = oDpt.Description.ToString();

                    ucUserDet.UserDetail = oDpt.UserDetails;
                    hdDptID.Value = iDptID.ToString();
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
    }
}
