/*
 * File name            : DesignationSetup
 * Author               : A.K.M. Tanjil Hossain,
 *                      : A.K.M. Zahidul Quaium,
 *                      : Tanvir Alam
 *                      : Jerin Afsana
 * Date                 : Sep 01,2014
 * Version              : 1.0
 *
 * Description          : To manage Designation
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
using SBM_BLC1.Entity.Common;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.SecurityAdmin;

namespace SBM_WebUI.mp
{
    public partial class DesignationSetup : System.Web.UI.Page
    {
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_DESIGNATION_ID = "sDsgID";
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_SA.DESIGNATION))
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

            string dsgID = Request.QueryString[OBJ_DESIGNATION_ID];
            string sPageID = Request.QueryString[OBJ_PAGE_ID];
            txtDesignationID.ReadOnly = true;

            if (!string.IsNullOrEmpty(dsgID))
            {
                dsgID = oCrypManager.GetDecryptedString(dsgID, Constants.CRYPT_PASSWORD_STRING);
            }
            if (!string.IsNullOrEmpty(sPageID))
            {
                sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
            }

            if (!string.IsNullOrEmpty(dsgID) && !string.IsNullOrEmpty(sPageID))
            {
                string sOperationType = sPageID.Substring(4, 1);
                if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
                {
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    LoadDataByID(Util.GetIntNumber(dsgID));

                    // general Control
                    Util.ControlEnabled(txtDesignationID, false);
                    Util.ControlEnabled(txtDesignationName, false);

                    

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
                Util.ControlEnabled(txtDesignationID, true);
                Util.ControlEnabled(txtDesignationName, true);

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
                Response.Redirect(Constants.PAGE_SA_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_SA.DESIGNATION).PadLeft(5, '0'), false);
            }
            else
            {
                // no action
            }
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_SA_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_SA.DESIGNATION).PadLeft(5, '0'), false);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdDsgID.Value))
            {
                Designation oDsg = new Designation(Util.GetIntNumber(hdDsgID.Value));
                DesignationDAL oDsgDAL = new DesignationDAL();
                oDsg.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oDsgDAL.Reject(oDsg);
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

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request[txtDesignationID.UniqueID].Trim()))
            {
                LoadDataByID(Util.GetIntNumber(txtDesignationID.Text));
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdDsgID.Value))
            {
                Designation oDsg = new Designation(Util.GetIntNumber(hdDsgID.Value));
                DesignationDAL oDsgDAL = new DesignationDAL();

                oDsg.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oDsgDAL.Approve(oDsg);
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
            hdDsgID.Value = "";
            ClearTextValue();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Designation oDsg = new Designation();
            DesignationDAL oDsgDAL = new DesignationDAL();

            oDsg.DesignationID = Util.GetIntNumber(txtDesignationID.Text);
            oDsg.Description = txtDesignationName.Text;
            oDsg.UserDetails = ucUserDet.UserDetail; ;

            Result oResult = (Result)oDsgDAL.Save(oDsg);

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

        // ok
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdDsgID.Value))
            {
                DesignationDAL oDsgDAL = new DesignationDAL();
                Result oResult = (Result)oDsgDAL.Detete(hdDsgID.Value);
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
            txtDesignationID.Text = string.Empty;
            txtDesignationName.Text = string.Empty;
        }

        // ok
        private void LoadList()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (oConfig != null)
            {
                Designation oDsg = new Designation();
                DesignationDAL oDsgDAL = new DesignationDAL();
                Result oResult = oDsgDAL.LoadUnapprovedList(oConfig.UserName, false);

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
                }                
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_NOT_FOUND, Constants.MSG_TYPE_ERROR);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
            }
        }

        private void LoadDataByID(int iDsgID)
        {
            Designation oDsg = new Designation(iDsgID);
            DesignationDAL oDsgDAL = new DesignationDAL();

            Result oResult = new Result();
            oResult = oDsgDAL.LoadByID(oDsg);
            ClearTextValue();
            if (oResult.Status)
            {
                oDsg = (Designation)oResult.Return;
                if (oDsg != null)
                {
                    txtDesignationID.Text = oDsg.DesignationID.ToString();
                    txtDesignationName.Text = oDsg.Description.ToString();
                    
                    ucUserDet.UserDetail = oDsg.UserDetails;
                    hdDsgID.Value = iDsgID.ToString();
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
            DesignationDAL oDesgDAL = new DesignationDAL();
            Result oResult = oDesgDAL.LoadMoveData(txtDesignationID.Text, sType);
            if (oResult.Status)
            {
                Designation oDesg = (Designation)oResult.Return;
                if (oDesg != null)
                {
                    ClearTextValue();
                    txtDesignationID.Text = oDesg.DesignationID.ToString();
                    txtDesignationName.Text = oDesg.Description.ToString();
                }
                ucUserDet.UserDetail = oDesg.UserDetails;
                hdDsgID.Value = oDesg.DesignationID.ToString();
            }
        }
    }
}
