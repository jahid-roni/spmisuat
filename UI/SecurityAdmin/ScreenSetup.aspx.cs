using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SBM_BLC1.Common;
using SBM_BLC1.Entity.Common;
using System.Data;
using SBM_BLC1.Entity.SecurityAdmin;
using SBM_BLC1.SecurityAdmin;


namespace SBM_WebUI.mp
{
    public partial class ScreenSetup : System.Web.UI.Page
    {
        CryptographyManager oCrypManager = new CryptographyManager();
        public const string OBJ_SCREEN_ID = "sSCrID";
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
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_SA.SCREEN_SETUP))
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

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdScrID.Value))
            {
                ScreenDAL oScrDAL = new ScreenDAL();
                Result oResult = (Result)oScrDAL.Detete(hdScrID.Value);
                if (oResult.Status)
                {
                    this.LoadList();
                    this.ClearTextValue();

                    hdScrID.Value = "";
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
        protected void btnSave_Click(object sender, EventArgs e)
        {
            Screen oScr = (Screen)Session[Constants.SES_CURRENT_SCREEN];
            ScreenDAL oScrDAL = new ScreenDAL();
            oScr.ScreenID = Util.GetIntNumber(Request[txtScreenID.UniqueID].Trim());
            oScr.ScreenName = txtScreenName.Text;
            oScr.Description = txtDescription.Text;
            oScr.UserDetails = ucUserDet.UserDetail;

            Result oResult = (Result)oScrDAL.Save(oScr);

            if (oResult.Status)
            {
                this.LoadList();

                ucMessage.OpenMessage(Constants.MSG_SUCCESS_SAVE, Constants.MSG_TYPE_SUCCESS);
            }
            else
            {
                ucMessage.OpenMessage(Constants.MSG_ERROR_SAVE, Constants.MSG_TYPE_ERROR);
            }
            txtScreenID.Enabled = true;
            txtScreenID.Text = string.Empty;
            txtScreenName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            gvScreen.DataSource = null;
            gvScreen.DataBind();
            Session[Constants.SES_CURRENT_SCREEN] = null;
        }
        protected void btnLoad_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request[txtScreenID.UniqueID].Trim().ToUpper()))
            {
        
            

            int iScrID = Util.GetIntNumber(Request[txtScreenID.UniqueID].Trim());
            
            LoadDataByID(iScrID);
 

           
            
            
            }
        }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            hdScrID.Value = "";
            ClearTextValue();
        }

        protected void btnSaveScreenObject_Click(object sender, EventArgs e)
        {
            Screen oScr = new Screen();
            ScreenDAL oScrDAL = new ScreenDAL();
            Screen oScreen = (Screen)Session[Constants.SES_CURRENT_SCREEN];
            if (oScreen != null)
            {
                AddScreenObject(oScreen);
            }
            else
            {
                AddScreenObject(new Screen());
            }

            // clear Data after completing save
            txtObjectCaption.Text = string.Empty;
            txtObjectName.Text = string.Empty;
            chkIsActive.Checked = false;
            if (ddlOperationTypeId.Items.Count > 0)
            {
                ddlOperationTypeId.SelectedIndex = 0; ;
            }
            hdScrObjID.Value = "";

        }
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdScrID.Value))
            {
                Screen oScr = new Screen(Util.GetIntNumber(hdScrID.Value));
                ScreenDAL oScrDAL = new ScreenDAL();
                oScr.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oScrDAL.Approve(oScr);
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
        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdScrID.Value))
            {
                Screen oScr = new Screen(Util.GetIntNumber(hdScrID.Value));
                ScreenDAL oScrDAL = new ScreenDAL();
                oScr.UserDetails = ucUserDet.UserDetail;

                Result oResult = (Result)oScrDAL.Reject(oScr);
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

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PAGE_SA_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_SA.SCREEN_SETUP).PadLeft(5, '0'), false);
        }
        private void InitializeData()
        {
            gvData.PageSize = (int)Constants.PAGING_UNAPPROVED;
            DDListUtil.LoadDDLFromDB(ddlOperationTypeId, "OperationTypeID", "Description", "SA_OperationType", true);

            string ScrID = Request.QueryString[OBJ_SCREEN_ID];
            string sPageID = Request.QueryString[OBJ_PAGE_ID];
            
            if (!string.IsNullOrEmpty(ScrID))
            {
                ScrID = oCrypManager.GetDecryptedString(ScrID, Constants.CRYPT_PASSWORD_STRING);
            }
            if (!string.IsNullOrEmpty(sPageID))
            {
                sPageID = oCrypManager.GetDecryptedString(sPageID, Constants.CRYPT_PASSWORD_STRING);
            }

            if (!string.IsNullOrEmpty(ScrID) && !string.IsNullOrEmpty(sPageID))
            {
                string sOperationType = sPageID.Substring(4, 1);
                if (Constants.OPERATION_TYPE_APPROVAL.Equals(sOperationType))
                {
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                   LoadDataByID(Util.GetIntNumber(ScrID));

                    // general Control
                    Util.ControlEnabled(txtScreenID, false);
                    Util.ControlEnabled(txtScreenName, false);
                    Util.ControlEnabled(txtDescription, false);
                    Util.ControlEnabled(txtObjectCaption, false);
                    Util.ControlEnabled(txtObjectName, false);
                    Util.ControlEnabled(chkIsActive, false);



                    // button 
                    Util.ControlEnabled(btnReject, true);
                    Util.ControlEnabled(btnApprove, true);
                    Util.ControlEnabled(btnBack, true);
                    Util.ControlEnabled(btnReset, false);
                    Util.ControlEnabled(btnSave, false);
                    Util.ControlEnabled(btnDelete, false);
                    Util.ControlEnabled(btnLoad, false);
                    
                    gvScreen.Enabled = false;
                   


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
                Util.ControlEnabled(txtScreenID, true);
                Util.ControlEnabled(txtScreenName, true);
                Util.ControlEnabled(txtDescription, true);

                // button 
                Util.ControlEnabled(btnReject, false);
                Util.ControlEnabled(btnApprove, false);
                Util.ControlEnabled(btnBack, false);
                Util.ControlEnabled(btnReset, true);
                Util.ControlEnabled(btnSave, true);
                Util.ControlEnabled(btnDelete, true);
                Util.ControlEnabled(btnLoad, true);
               

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



        private void LoadList()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            if (oConfig != null)
            {

                Screen oScr = new Screen();
                ScreenDAL oScrDAL = new ScreenDAL();
                Result oResult = oScrDAL.LoadUnapprovedList(oConfig.UserName, false);
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
        public void PopErrorMsgAction(string sType)
        {
            if  (sType.Equals(Constants.BTN_APPROVE) || sType.Equals(Constants.BTN_REJECT))
            {
                Response.Redirect(Constants.PAGE_SA_APPROVAL + "?pType=" + Convert.ToString((int)Constants.PAGEINDEX_SA.SCREEN_SETUP).PadLeft(5, '0'), false);
            }
            else
            {
                // no action
            }
        }
        private void ClearTextValue()
        {
            txtScreenID.Enabled = true;
            txtScreenID.Text= string.Empty;
            txtScreenName.Text= string.Empty;
            txtDescription.Text = string.Empty;
            gvScreen.DataSource = null;
            gvScreen.DataBind();
            Session[Constants.SES_CURRENT_SCREEN] = null;
            
        }

        private void LoadDataByID(int iScrID)
        {
            Screen oScr = new Screen(iScrID);
            ScreenDAL oScrDAL = new ScreenDAL();

            Result oResult = new Result();
            oResult = oScrDAL.LoadByID(oScr);
            ClearTextValue();
            if (oResult.Status)
            {
                oScr = (Screen)oResult.Return;
                if (oScr != null)
                {
                    txtScreenID.Enabled = false;
                    txtScreenID.Text= oScr.ScreenID.ToString();
                    txtScreenName.Text = oScr.ScreenName.ToString();
                    txtDescription.Text = oScr.Description.ToString();

                    ucUserDet.UserDetail = oScr.UserDetails;
                    hdScrID.Value = iScrID.ToString();
                    // load child data...

                    DataTable oDataTable = new DataTable("dtData");

                    oDataTable.Columns.Add(new DataColumn("bfSLNo", typeof(string)));
                    oDataTable.Columns.Add(new DataColumn("bfObjectName", typeof(string)));
                    oDataTable.Columns.Add(new DataColumn("bfObjectCaption", typeof(string)));
                    oDataTable.Columns.Add(new DataColumn("bfOperationTypeID", typeof(string)));
                    oDataTable.Columns.Add(new DataColumn("bfIsActive", typeof(bool)));
                    ScreenObject oScreenObject = null;
                    DataRow row = null;
                    if (oScr.ScreenObjectList.Count > 0)
                    {
                        for (int i = 0; i < oScr.ScreenObjectList.Count; i++)
                        {
                            oScreenObject = (ScreenObject)oScr.ScreenObjectList[i];
                            
                            row = oDataTable.NewRow();
                            row["bfSLNo"] = oScreenObject.SLNo.ToString().ToUpper();
                            row["bfObjectName"] = oScreenObject.ObjectName.ToString().ToUpper();
                            row["bfObjectCaption"] = oScreenObject.ObjectCaption.ToString().ToUpper();
                           
                            row["bfOperationTypeID"] = oScreenObject.OperationType.ToString().ToUpper();
                         

                            row["bfIsActive"] = oScreenObject.IsActive.ToString().ToUpper();

                            oDataTable.Rows.Add(row);
                        }

                        gvScreen.DataSource = oDataTable;
                        gvScreen.DataBind();
                    }
                    
                    Session[Constants.SES_CURRENT_SCREEN] = oScr;
                    
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
        protected void gvScreen_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.GridDateFormat(e, gvScreen, null);
        }
        protected void gvScreen_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = null;

            if (((Button)e.CommandSource).Text.Equals("Select"))
            {
                gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                // LoadscreenChildDataByID(Util.GetIntNumber(gvRow.Cells[1].Text));
                Screen oScreen = (Screen)Session[Constants.SES_CURRENT_SCREEN];
                if (oScreen != null)
                {
                    //HiddenField hdNomSLNo =  (HiddenField)gvRow.FindControl("bfSLNo");
                    ScreenObject oScreenObj = oScreen.ScreenObjectList[gvRow.RowIndex];//.Where(d => d.SLNo.Equals(Util.GetIntNumber(hdSLNo.Value))).SingleOrDefault();
                    if (oScreenObj != null)
                    {
                        chkIsActive.Checked = oScreenObj.IsActive;
                        txtObjectCaption.Text = oScreenObj.ObjectCaption;
                        txtObjectName.Text = oScreenObj.ObjectName;
                        DDListUtil.Assign(ddlOperationTypeId, oScreenObj.OperationTypeID);
                        hdScrObjID.Value = gvRow.RowIndex.ToString();
                    }
                }
            }
            else if (((Button)e.CommandSource).Text.Equals("Delete"))
            {
                gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;


                Screen oScreen = (Screen)Session[Constants.SES_CURRENT_SCREEN];

                ScreenObject oScreenObj = oScreen.ScreenObjectList[gvRow.RowIndex];
                if (oScreen != null)
                {


                    oScreen.ScreenObjectList.Remove(oScreenObj);
                }

                DataTable oDataTable = new DataTable("dtData");

                oDataTable.Columns.Add(new DataColumn("bfSLNo", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfObjectName", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfObjectCaption", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfOperationTypeID", typeof(string)));
                oDataTable.Columns.Add(new DataColumn("bfIsActive", typeof(bool)));
                //ScreenObject oScreenObject = null;
                DataRow row = null;

                for (int i = 0; i < oScreen.ScreenObjectList.Count; i++)
                {

                    row = oDataTable.NewRow();

                    row["bfSLNo"] = oScreen.ScreenObjectList[i].SLNo.ToString().ToUpper();
                    row["bfObjectName"] = oScreen.ScreenObjectList[i].ObjectName.ToString().ToUpper();
                    row["bfObjectCaption"] = oScreen.ScreenObjectList[i].ObjectCaption.ToString().ToUpper();

                    row["bfOperationTypeID"] = oScreen.ScreenObjectList[i].OperationType.ToString().ToUpper();


                    row["bfIsActive"] = oScreen.ScreenObjectList[i].IsActive.ToString().ToUpper();
                    oDataTable.Rows.Add(row);
                }
                gvScreen.DataSource = oDataTable;
                gvScreen.DataBind();

                Session[Constants.SES_CURRENT_SCREEN] = oScreen;

                ucMessage.OpenMessage(Constants.MSG_SUCCESS_DELETE, Constants.MSG_TYPE_SUCCESS);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("Delete"), true);
            }
        }
            
        


        
        protected void gvScreen_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //just for signature to Handle IE exception 
        }
        
        private void AddScreenObject(Screen oScreen)
        {
            ScreenObject oScreenObject = null;
            bool isToEdit = false;
            int editIndex = 0;
            if (!string.IsNullOrEmpty(hdScrObjID.Value))
            {
               // if (oScreen.ScreenObjectList.Count > 0)
                //{
                    oScreenObject = oScreen.ScreenObjectList[Convert.ToInt32(hdScrObjID.Value)];////.Where(n => n.SLNo.Equals(Convert.ToInt32(hdSLNo.Value))).SingleOrDefault();

                    //oScreenObject.SLNo = Convert.ToInt32(hdScrObjID.Value);
                    editIndex = Convert.ToInt32(hdScrObjID.Value); //oScreen.ScreenObjectList.FindIndex(n => n.SLNo.Equals(Convert.ToInt32(hdSLNo.Value)));
                    isToEdit = true;
                //}
            }
            else
            {
                oScreenObject = new ScreenObject();
                oScreenObject.SLNo = 0; //oScreen.ScreenObjectList.Count + 1;
            }
            oScreenObject.ObjectName = txtObjectName.Text;
            oScreenObject.ObjectCaption = txtObjectCaption.Text;
            
            // get value from DDL
            string sOpValue = ddlOperationTypeId.SelectedItem.Text;
            string[] sOpVal = sOpValue.Split(':');
            if (sOpVal.Length > 0)
            {
                oScreenObject.OperationType = sOpVal[1];
            }
            oScreenObject.OperationTypeID = Convert.ToInt32(ddlOperationTypeId.SelectedItem.Value);
            
            oScreenObject.IsActive = chkIsActive.Checked;
            
            //Add Nominee
            if (!isToEdit)
            {
               // oScreen.ScreenObjectList.Clear();
                
                oScreen.ScreenObjectList.Add(oScreenObject);
            }
            else // Edit Nominee
            {
                oScreen.ScreenObjectList[editIndex] = oScreenObject;
            }

            
            

            DataTable dtScreenObject = new DataTable();
            
           // DataTable dtOperationType = new DataTable();

            dtScreenObject.Columns.Add(new DataColumn("bfObjectName", typeof(string)));
            dtScreenObject.Columns.Add(new DataColumn("bfObjectCaption", typeof(string)));
            dtScreenObject.Columns.Add(new DataColumn("bfIsActive", typeof(bool)));
            dtScreenObject.Columns.Add(new DataColumn("bfOperationTypeID", typeof(string)));
            //dtOperationType.Columns.Add(new DataColumn("Text", typeof(string)));
            //dtOperationType.Columns.Add(new DataColumn("Value", typeof(int)));
            


            DataRow rowScreenObject = null;
            //DataRow rowOperationType = null;
            for (int i = 0; i < oScreen.ScreenObjectList.Count; i++)
            {
                rowScreenObject = dtScreenObject.NewRow();

                rowScreenObject["bfObjectName"] = oScreen.ScreenObjectList[i].ObjectName;
                rowScreenObject["bfObjectCaption"] = oScreen.ScreenObjectList[i].ObjectCaption;
                rowScreenObject["bfIsActive"] = oScreen.ScreenObjectList[i].IsActive;

                rowScreenObject["bfOperationTypeID"] = oScreen.ScreenObjectList[i].OperationType;
               
                dtScreenObject.Rows.Add(rowScreenObject);
                //rowOperationType = dtOperationType.NewRow();
                //rowOperationType["Text"] = oScreen.ScreenObjectList[i].OperationType;
                //rowOperationType["Value"] = oScreen.ScreenObjectList[i].OperationTypeID;

                //dtOperationType.Rows.Add(rowOperationType);
            }

            //Reload Grid
            gvScreen.DataSource = dtScreenObject;
            gvScreen.DataBind();
           
           

           
            //Update Session
           
           Session[Constants.SES_CURRENT_SCREEN] = oScreen;

        }


    }
}
