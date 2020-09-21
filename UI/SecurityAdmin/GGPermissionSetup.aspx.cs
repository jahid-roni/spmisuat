/*
 * File name            : GGPermissionSetup
 * Author               : A.K.M. Tanjil Hossain,
 *                      : A.K.M. Zahidul Quaium,
 *                      : Tanvir Alam
 *                      : Jerin Afsana
 * Date                 : Sep 01,2014
 * Version              : 1.0
 *
 * Description          : To manager Role Psermission
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
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Entity.SecurityAdmin;
using SBM_BLC1.SecurityAdmin;

namespace SBM_WebUI.mp
{
    public partial class GGPermissionSetup : System.Web.UI.Page
    {

        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_GP_ID = "sGPID";
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_SA.GROUP_PERMISSION))
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
            //gvData.PageSize = (int)Constants.PAGING_UNAPPROVED;

            // Dropdown load SPType
            DDListUtil.LoadDDLFromDB(ddlDepartmentID, "DepartmentID", "Description", "SA_Department", true);

            GroupDAL gpDal = new GroupDAL();
            Result oR = gpDal.GetOperationType();
            DataTable dtOt = null;
            if (oR.Status)
            {
                dtOt = (DataTable)oR.Return;
                Session[Constants.SES_GROUP_OPERATION_DATA] = dtOt;
            }


            string gpID = Request.QueryString[OBJ_GP_ID];
            string sPageID = Request.QueryString[OBJ_PAGE_ID];
            if (!string.IsNullOrEmpty(gpID))
            {
                gpID = oCrypManager.GetDecryptedString(gpID, Constants.CRYPT_PASSWORD_STRING);
            }
            if (!string.IsNullOrEmpty(sPageID))
            {
                sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
            }

            LoadScreenList();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (!string.IsNullOrEmpty(gpID) && !string.IsNullOrEmpty(sPageID))
            {
                string sOperationType = sPageID.Substring(4, 1);
                if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
                {                    
                    LoadDataByID(Util.GetIntNumber(gpID));


                    // control
                    Util.ControlEnabled(txtDescription, false);
                    Util.ControlEnabled(txtGroupName, false);
                    Util.ControlEnabled(ddlDepartmentID, false);
                    gvScreenList.Enabled = false;

                    // button 
                    Util.ControlEnabled(btnReject, true);
                    Util.ControlEnabled(btnApprove, true);
                    Util.ControlEnabled(btnBack, true);
                    Util.ControlEnabled(btnReset, false);
                    Util.ControlEnabled(btnSave, false);
                    Util.ControlEnabled(btnDelete, false);
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

                    //fsList.Visible = false;
                }
            }
            else
            {
                // control
                Util.ControlEnabled(txtDescription, true);
                Util.ControlEnabled(txtGroupName, true);
                Util.ControlEnabled(ddlDepartmentID, true);
                gvScreenList.Enabled = true;



                // button 
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);
                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);
                Util.ControlEnabled(btnFirst, true);
                Util.ControlEnabled(btnPrevious, true);
                Util.ControlEnabled(btnNext, true);
                Util.ControlEnabled(btnLast, true);
                Util.ControlEnabled(ucUserDet.FindControl("txtCheckerComments"), false);

                #region User-Detail.
                UserDetails oUserDetails = new UserDetails();
                oUserDetails.MakerID = oConfig.UserName;
                oUserDetails.MakeDate = DateTime.Now;
                ucUserDet.UserDetail = oUserDetails;
                #endregion User-Detail.

                //fsList.Visible = true;

                //LoadList();
            }


        }
        #endregion InitializeData

        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //gvData.PageIndex = e.NewPageIndex;
            //if (Session[Constants.SES_CONFIG_UNAPPROVE_DATA] != null)
            //{
            //    DataTable dtTmpList = (DataTable)Session[Constants.SES_CONFIG_UNAPPROVE_DATA];
            //    gvData.DataSource = dtTmpList;
            //    gvData.DataBind();
            //}
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
            //Util.GridDateFormat(e, gvData, null);
        }

        public void PopErrorMsgAction(string sType)
        {
            if (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_SA_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_SA.GROUP_PERMISSION).PadLeft(5, '0'), false);
            }
            else
            {
                // no action
            }
        }


        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_SA_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_SA.GROUP_PERMISSION).PadLeft(5, '0'), false);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdGpID.Value))
            {
                Group oGp = new Group(Util.GetIntNumber(hdGpID.Value));
                GroupDAL oGpDAL = new GroupDAL();
                oGp.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oGpDAL.Reject(oGp);
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
            if (!string.IsNullOrEmpty(hdGpID.Value))
            {
                Group oGp = new Group(Util.GetIntNumber(hdGpID.Value));
                GroupDAL oGpDAL = new GroupDAL();
                oGp.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oGpDAL.Approve(oGp);
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
            hdGpID.Value = "";
            ClearTextValue();
        }

        private List<Screen> GetScreenList()
        {
            List<Screen> oScreenList = new List<Screen>();
            Screen oScreen = null; 
            string sOpVal = string.Empty;
            if (Session[Constants.SES_GROUP_OPERATION_DATA] != null)
            {
                DataTable dtOt = (DataTable)Session[Constants.SES_GROUP_OPERATION_DATA];
                if (dtOt.Rows.Count > 0)
                {
                    foreach (GridViewRow row in gvScreenList.Rows)
                    {
                        sOpVal = string.Empty;
                        oScreen = new Screen();
                        oScreen.ScreenID = Util.GetIntNumber(row.Cells[0].Text);
                   
                        oScreen.ScreenName = row.Cells[1].Text;
                        for (int i = 0; i < dtOt.Rows.Count; i++)
                        {
                            CheckBox chk = (CheckBox)row.FindControl(dtOt.Rows[i]["Description"].ToString());
                            if (chk != null)
                            {
                                if (chk.Checked)
                                {
                                    sOpVal += "," + dtOt.Rows[i]["OperationTypeID"].ToString();                                
                                }
                            }
                            if (sOpVal.Length > 0)
                            {
                                oScreen.OperationVal = sOpVal.Substring(1);
                            }                            
                        }
                        if (sOpVal.Length > 0)
                        {
                            oScreenList.Add(oScreen);
                        }
                    }
                }
            }

            return oScreenList;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Group oGp = new Group();
            GroupDAL oGpDAL = new GroupDAL();

            oGp.GroupID = Util.GetIntNumber(Request[txtGroupID.UniqueID].Trim());
            oGp.GroupName = txtGroupName.Text;
            oGp.Description = txtDescription.Text;
            oGp.Department.DepartmentID = Util.GetIntNumber(ddlDepartmentID.SelectedValue);
            oGp.ScreenList = GetScreenList();

            oGp.UserDetails = ucUserDet.UserDetail;
            oGp.UserDetails.CheckDate = DateTime.Now;
            oGp.UserDetails.CheckerID = oGp.UserDetails.MakerID;

            Result oResult = (Result)oGpDAL.Save(oGp);

            if (oResult.Status)
            {
                ClearTextValue();
                //LoadList();
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
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            
            MoveAction("S");
        }

        private void MoveAction(string sType)
        {
            GroupDAL oGpDAL = new GroupDAL();
            Result oResult = oGpDAL.LoadMoveData(txtGroupName.Text, sType);
            ClearTextValue();
            if (oResult.Status)
            {
                Group oGp = (Group)oResult.Return;
                if (oGp != null)
                {                                        
                    SetObject(oGp);                    
                    hdGpID.Value = oGp.GroupID.ToString();
                }
                else
                {
                    ucMessage.OpenMessage("Group Name does not exist", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Info"), true);
                }
               
            }
            else
            {
                ucMessage.OpenMessage("Group Name does not exist", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Info"), true);
            }
            
        }

        public void SetObject(Group oGp)
        {
            txtGroupID.Text = oGp.GroupID.ToString();
            txtGroupName.Text = oGp.GroupName.ToString();
            txtDescription.Text = oGp.Description.ToString();
            DDListUtil.Assign(ddlDepartmentID, oGp.Department.DepartmentID);

            DataTable dtOpr = (DataTable)Session[Constants.SES_GROUP_OPERATION_DATA];
            int iCount = dtOpr.Rows.Count;

            DataView dv = new DataView();
            dv.Table = oGp.DTScreenList;
            
            if (oGp.DTScreenList.Rows.Count > 0)
            {
                foreach (GridViewRow row in gvScreenList.Rows)
                {

                    dv.RowFilter = "ScreenID = '" + row.Cells[0].Text + "'";
                    if (dv.Count > 0)
                    {
                        if (dv[0]["ScreenID"].ToString() == row.Cells[0].Text)
                        {
                            string sOperationCode = dv[0]["OperationCode"].ToString();
                            for (int i = dtOpr.Rows.Count; i > 0; i--)
                            {
                                CheckBox chk = (CheckBox)row.FindControl(dtOpr.Rows[i - 1]["Description"].ToString());
                                string sRowVal = dtOpr.Rows[i - 1]["OperationTypeID"].ToString();
                                if (sOperationCode.Contains(sRowVal))
                                {                                    
                                    if (chk != null)
                                    {
                                        if (sRowVal.Equals("1"))
                                        {
                                            if (!sOperationCode[0].ToString().Equals(sRowVal))
                                            {
                                                continue;
                                            }                                            
                                        }

                                        chk.Checked = true;
                                    }
                                }
                                else
                                {
                                    if (chk != null)
                                    {
                                        chk.Checked = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //UserDetails userDetail = oGp.UserDetails;
            //userDetail.MakeDate = DateTime.Now;
            //ucUserDet.UserDetail = userDetail;
            hdGpID.Value = oGp.GroupID.ToString();
        }

        // ok
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdGpID.Value))
            {
                GroupDAL oGpDAL = new GroupDAL();
                Result oResult = (Result)oGpDAL.Detete(hdGpID.Value);
                if (oResult.Status)
                {
                    //this.LoadList();
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
            txtGroupID.Text = string.Empty;
            txtGroupName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            if (ddlDepartmentID.Items.Count > 0)
            {
                ddlDepartmentID.SelectedIndex = 0;
            }

            DataTable dtOpr = (DataTable)Session[Constants.SES_GROUP_OPERATION_DATA];
            int iCount = dtOpr.Rows.Count;
            if (dtOpr.Rows.Count > 0)
            {
                foreach (GridViewRow row in gvScreenList.Rows)
                {
                    for (int i = 0; i < dtOpr.Rows.Count; i++)
                    {
                        CheckBox chk = (CheckBox)row.FindControl(dtOpr.Rows[i]["Description"].ToString());
                        if (chk != null)
                        {
                            chk.Checked = false;
                        }
                    }
                }
            }
        }

        // ok
        private void LoadList()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (oConfig != null)
            {
                Group oGP = new Group();
                GroupDAL oGpDAL = new GroupDAL();
                Result oResult = oGpDAL.LoadUnapprovedList(oConfig.UserName, false);
                //gvData.DataSource = null;
                //gvData.DataBind();

                if (oResult.Status)
                {
                    DataTable dtTmpList = (DataTable)oResult.Return;
                    if (dtTmpList.Rows.Count > 0)
                    {
                        dtTmpList.Columns.Remove("Maker ID");

                        //gvData.DataSource = dtTmpList;
                        //gvData.DataBind();
                        Session[Constants.SES_CONFIG_UNAPPROVE_DATA] = dtTmpList;
                    }                    
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
            }
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(Request[txtDepartmentID.UniqueID].Trim()))
            //{
            //    LoadDataByID(Util.GetIntNumber(Request[txtDepartmentID.UniqueID].Trim()));
            //}
            //else
            //{
            //    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
            //    ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            //}
        }

        private void LoadScreenList()
        {
            GroupDAL oGpDAL = new GroupDAL();
            Result oResult = new Result();
            oResult = oGpDAL.LoadScreenList();
            
            gvScreenList.DataSource = null;
            gvScreenList.DataBind();

            if (oResult.Status)
            {
                DataTable dtData = (DataTable)oResult.Return;

                gvScreenList.DataSource = dtData;
                gvScreenList.DataBind();
                int iCount = Util.GetIntNumber(oResult.Message);
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }

        private void LoadDataByID(int iGpD)
        {
            Group oGp = new Group(iGpD);
            GroupDAL oGpDAL = new GroupDAL();

            Result oResult = new Result();
            oResult = oGpDAL.LoadByID(oGp);
            ClearTextValue();
            if (oResult.Status)
            {
                oGp = (Group)oResult.Return;
                if (oGp != null)
                {
                    SetObject(oGp);
                }
                else
                {
                    ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
            }
        }

        
    }
}
