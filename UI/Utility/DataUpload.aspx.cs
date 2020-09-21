using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SBM_BLC1.Common;
using SBM_BLC1.DAL.Claim;
using SBM_BLC1.Entity.Common;
using CrystalDecisions.Enterprise;

namespace SBM_WebUI.mp
{
    public partial class DataUpload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Constants.SES_USER_CONFIG] != null)
            {
                if (!Page.IsPostBack)
                {
                    Util.InvalidateSession();
                    InitializeData();
                    //This is for Page Permission
                    CheckPermission chkPer = new CheckPermission();
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_UTILITY.DATA_UPLOAD ))
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

        protected void InitializeData()
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            DDListUtil.LoadActiveUser(ddlUserName, "UserName", "UserName", "SA_User", (int)Constants.USER_GROUP.MAKER, false, oConfig.DivisionID);
            DDListUtil.LoadDDLFromDB(ddlJournalType, "JournalType", "Description", "SPMS_JournalType", false);
            RBLChangeColor(rblCurrencyActivity);            
            lblActivity.Text = rblCurrencyActivity.Items[0].Value;            
        }

        public void RBLChangeColor(RadioButtonList rdoBox)
        {
            for (int i = 0; i <= rdoBox.Items.Count - 1; i++)
            {
                rdoBox.Items[i].Attributes["onclick"] = string.Format("RbChangeColor( this ) ");
            }
        }
        

        protected void ddlJournalType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }
        private bool CheckForUpload()
        {
            Result oResult = null;
            DataUploadDAL oDataUploadDAL = new DataUploadDAL();
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            oResult = oDataUploadDAL.CheckForUpload(ddlUserName.SelectedValue, Convert.ToByte(ddlJournalType.SelectedValue), oConfig.DivisionID);
            if (oResult.Status)
            {
                DataTable dt = oResult.Return as DataTable;
                if (dt.Rows.Count > 0)
                {
                    string sMessage = null;
                    sMessage = "Following transaction details has already been queued for upload. Please check..";
                    //sMessage += "\n\tUpload Transaction No : " + Convert.ToString(dt.Rows[0]["UploadTransactionNo"]);
                    sMessage += "\n\tMaker ID : " + Convert.ToString(dt.Rows[0]["AccEntryOperator"]);
                    sMessage += "\n\tJournal Type : " + Convert.ToString(dt.Rows[0]["Description"]);

                    ucMessage.OpenMessage(sMessage, Constants.MSG_TYPE_ERROR);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);

                    ddlJournalType.SelectedIndex = 0;
                    return false;
                }
            }
            return true;
        }
        protected void LoadData()
        {
            ClearData(false);

            if (!string.IsNullOrEmpty(ddlJournalType.SelectedValue) && !string.IsNullOrEmpty(ddlUserName.SelectedValue))
            {
                Result oResult = null;
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                DataUploadDAL oDataUploadDAL = new DataUploadDAL();
                txtStUpldFilePth.Text = Constants.FILE_UPLOAD_PATH;
                if (!CheckForUpload())
                {
                    return;
                }
                
                oResult = oDataUploadDAL.GetCurrentData(ddlUserName.SelectedValue, Convert.ToByte(ddlJournalType.SelectedValue), oConfig.DivisionID, oConfig.BankCodeID);
                if (oResult.Status)
                {
                    DataTable dtCurrntData = oResult.Return as DataTable;

                    if (dtCurrntData.Rows.Count == 0)
                    {
                        LoadDataToSelect();
                    }
                    else
                    {
                        LoadDataToUpdate(dtCurrntData);
                    }
                }
            }            
        }

        protected void LoadDataToSelect()
        {
            /*
             Me.rbtSelect.Checked = True
                Me.lblActivity.Text = Me.rbtSelect.Text
                Me.btnSelect.Text = "S&elect"
                Me.txtTransactionNo.Text = ""
                Me.txtFileSeqNo.Text = ""
                Me.txtOriginatorID.Text = .OriginatorID
                Me.txtOperator.Text = modCommon.UserName
                Me.txtTransactionDate.Text = ""
                Me.txtTransactionTime.Text = ""
                Me.cDtGridData = .GetEmptyListTable
                Me.dgrList.DataSource = Me.cDtGridData
                Me.dgrList.TableStyles(0).MappingName = Me.cDtGridData.TableName
                Me.cDtDetailsData = .GetEmptyDetailsTable
                Me.dgrDetails.DataSource = Me.cDtDetailsData
                Me.dgrDetails.TableStyles(0).MappingName = Me.cDtDetailsData.TableName

                Me.txtFileName.Text = .FileName
             */
            SetRadioButtonIndx(1);
            lblActivity.Text = rblCurrencyActivity.Items[1].Value;
            btnSelect.Text = "Select";
            txtTransacNo.Text = string.Empty;
            txtStFileSeqNo.Text = string.Empty;
            
            DataUploadDAL oDataUploadDAL = new DataUploadDAL();
            Result oResult = oDataUploadDAL.GetFileUploadData(txtStUpldFilePth.Text);
            if (oResult.Status)
            {
                txtStOriginatorID.Text = oResult.Message;//set OriginatorID through Message
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                if (oConfig.DivisionID == "CTG")
                {
                    txtStOriginatorID.Text = "3607";
                }
                else if (oConfig.DivisionID == "KHU")
                {
                    txtStOriginatorID.Text = "4007";
                }
                else if (oConfig.DivisionID == "BOG")
                {
                    txtStOriginatorID.Text = "3207";
                }
                else if (oConfig.DivisionID == "SYL")
                {
                    txtStOriginatorID.Text = "3207";
                }

                DataTable dtDataUpload = oResult.Return as DataTable;
                if (dtDataUpload.Rows.Count > 0)
                {
                    Session[Constants.SES_DATA_UPLOAD] = dtDataUpload;
                    ScriptManager.RegisterStartupScript(UpdatePanel1, GetType(), "script", "javascript:ConfirmDialog();", true);                    
                }          
            }

            SetGridData(oDataUploadDAL);            
        }

        protected void LoadDataToUpdate(DataTable dtUpdate)
        {
            /*
             Select Case CByte(vUploadInfo.Rows(0)(.TableProperties.UploadStatus))
                    Case modCommon.enmUploadStatus.Selected
                        Me.rbtMake.Checked = True
                        Me.lblActivity.Text = Me.rbtMake.Text
                        Me.btnSelect.Text = "Mak&e"
                        Me.btnCancel.Enabled = True
                        Me.btnCancel.Tag = Me.btnCancel.Enabled
                    Case modCommon.enmUploadStatus.Upload_File_Created
                        Me.rbtUploaded.Checked = True
                        Me.lblActivity.Text = Me.rbtUploaded.Text
                        Me.btnSelect.Text = "Uploade&d"
                    Case Else
                        Throw modCommon.UserDefinedException("Invalid Data Status. Please check...")
                End Select
             */
            //if (Convert.ToByte(dtUpdate.Rows[0]["UploadStatus"]).Equals(SBM_BLV1.baseCommon.enmUploadStatus.Selected))
            if (Convert.ToByte(dtUpdate.Rows[0]["UploadStatus"]).Equals((Byte)SBM_BLV1.baseCommon.enmUploadStatus.Selected))
            {
                SetRadioButtonIndx(2);
                lblActivity.Text = rblCurrencyActivity.Items[2].Value;                
                btnSelect.Text = "Make";
            }
            else if (Convert.ToByte(dtUpdate.Rows[0]["UploadStatus"]).Equals((Byte)SBM_BLV1.baseCommon.enmUploadStatus.Upload_File_Created))
            {
                SetRadioButtonIndx(3);
                lblActivity.Text = rblCurrencyActivity.Items[3].Value;                
                btnSelect.Text = "Uploaded";
            }
            else
            {
                ucMessage.OpenMessage("Invalid Data Status. Please check...", Constants.MSG_TYPE_ERROR);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }

            txtTransacNo.Text = DB.GetDBValue(dtUpdate.Rows[0]["UploadTransactionNo"]);
            txtStFileSeqNo.Text = DB.GetDBValue(dtUpdate.Rows[0]["FileSeqNo"]);
            txtStOriginatorID.Text = DB.GetDBValue(dtUpdate.Rows[0]["OriginatorID"]);
            
            txtStMaker.Text = DB.GetDBValue(dtUpdate.Rows[0]["MakerID"]);
            txtStTransDate.Text = Date.GetDateTimeByString(dtUpdate.Rows[0]["TransactionDate"].ToString()).ToString(Constants.DATETIME_FORMAT);
            txtStTransTime.Text = Date.GetDateTimeByString(dtUpdate.Rows[0]["TransactionTime"].ToString()).ToString(Constants.TIME_FORMAT);//Strings.Format(vUploadInfo.Rows(0)(.TableProperties.TransactionTime), modCommon.TimeFormat)
            txtStUpldFilePth.Text = DB.GetDBValue(dtUpdate.Rows[0]["FileName"]);
            /*
              Me.cDtGridData = .GetListDataToUpdate(Me.txtTransactionNo.Text)
              Me.dgrList.DataSource = Me.cDtGridData
                Me.dgrList.TableStyles(0).MappingName = Me.cDtGridData.TableName
                If Me.cDtGridData.Rows.Count > 0 Then
                    Me.dgrList.CurrentRowIndex = 0
                End If
                If Me.rbtMake.Checked Then
                    Me.SelectAllTrans(True)
                    Me.dgrList.ReadOnly = True
                Else
                    Me.dgrList.ReadOnly = False
                    For i = 0 To Me.cDtGridData.Rows.Count - 1
                        If CByte(Me.cDtGridData.Rows(i)(.TableProperties.UploadStatus)) = modCommon.enmUploadStatus.Uploaded Then
                            Me.cDtGridData.Rows(i)(.TableProperties.IsSelected) = True
                        End If
                    Next
                End If
                Me.cDtDetailsData = .GetDetailsDataToUpdate(Me.txtTransactionNo.Text)

                Me.SetDetailsGridData()
                Me.UpdateSummary()
             */
            DataUploadDAL oDataUploadDAL = new DataUploadDAL();
            Result oResult = oDataUploadDAL.GetListDataToUpdate(txtTransacNo.Text);
            if (oResult.Status)
            {
                DataTable dtListData = oResult.Return as DataTable;
                DataTable dtBindData = dtListData.Copy();
                //Remove Column
                dtBindData.Columns.Remove("UploadStatus");

                gvTransactionList.DataSource = dtBindData;
                gvTransactionList.DataBind();

                if (rblCurrencyActivity.Items[2].Selected || rblCurrencyActivity.Items[3].Selected)
                {
                    SelectAllTrans(true);
                    gvTransactionList.Enabled = false;
                }
                else
                {
                    gvTransactionList.Enabled = true;
                    for (int iRowIndx = 0; iRowIndx < dtListData.Rows.Count; iRowIndx++)
                    {
                        if (Convert.ToByte(dtListData.Rows[iRowIndx]["UploadStatus"]).Equals(SBM_BLV1.baseCommon.enmUploadStatus.Uploaded))
                        {
                            dtListData.Rows[iRowIndx]["IsSelected"] = true;
                        }
                    }
                }

                oResult = oDataUploadDAL.GetDetailsDataToUpdate(txtTransacNo.Text);
                if (oResult.Status)
                {
                    /*
                                 If Me.dgrList.CurrentRowIndex = -1 Then
                            dtTemp = Me.cOBusiness.GetEmptyDetailsTable
                        Else
                            dtTemp = Me.cDtDetailsData.Clone
                            With Me.cOBusiness.TableProperties
                                drRows = Me.cDtDetailsData.Select(.AccTransactionNo & " = " & modCommon.ValidDBValue(Me.dgrList._Rows(Me.dgrList.CurrentRowIndex)(.AccTransactionNo), modCommon.enmDBValueType.Texts))
                            End With
                            For intIndex = 0 To drRows.Length - 1
                                dtTemp.ImportRow(drRows(intIndex))
                            Next
                        End If
                        Me.dgrDetails.DataSource = dtTemp
                        Me.dgrDetails.TableStyles(0).MappingName = dtTemp.TableName
                     */
                    if (gvTransactionList.Rows.Count > 0)
                    {
                        SetDetailsGridData(oResult.Return as DataTable, gvTransactionList.Rows[0]);
                    }

                    UpdateSummary(oResult.Return as DataTable, oDataUploadDAL);
                }
            }
        }

        private void SetGridData(DataUploadDAL oDataUploadDAL)
        {
            if (oDataUploadDAL == null)
            {
                oDataUploadDAL = new DataUploadDAL();
            }
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            Result oResult = oDataUploadDAL.GetListDataToSelect(ddlUserName.SelectedValue, Convert.ToByte(ddlJournalType.SelectedValue), oConfig.DivisionID, oConfig.BankCodeID);

            DataTable dtListData = null;
            if (oResult.Status)
            {
                dtListData = oResult.Return as DataTable;
                DataTable dtBindData = dtListData.Copy();
                //Remove Column
                dtBindData.Columns.Remove("UploadStatus");

                gvTransactionList.DataSource = dtBindData;
                gvTransactionList.DataBind();
            }

            oResult = oDataUploadDAL.GetDetailsDataToSelect(ddlUserName.SelectedValue, Convert.ToByte(ddlJournalType.SelectedValue));

            if (oResult.Status)
            {
                if (gvTransactionList.Rows.Count > 0)
                {
                    SetDetailsGridData(oResult.Return as DataTable, gvTransactionList.Rows[0]);
                }
            }

            UpdateSummary(oResult.Return as DataTable, oDataUploadDAL);
        }

        private void SelectAllTrans(bool isSelect)
        {
            foreach (GridViewRow gvr in gvTransactionList.Rows)
            {
                CheckBox cbSelect = (CheckBox)(gvr.FindControl("chkSelect"));
                cbSelect.Checked = isSelect;
            }

            UpdateSummary(null, null);
        }

        private void UpdateSummary(DataTable dtDetailData, DataUploadDAL oDataUploadDAL)
        {
            string sTransNos = string.Empty;
            int iSelTrans = 0;
            try
            {
                foreach (GridViewRow gvr in gvTransactionList.Rows)
                {
                    CheckBox cbSelect = (CheckBox)(gvr.FindControl("chkSelect"));
                    if (cbSelect.Checked)
                    {
                        if (string.IsNullOrEmpty(sTransNos.Trim()))
                        {
                            sTransNos = SBM_BLV1.baseCommon.ValidDBValue(gvr.Cells[2].Text, SBM_BLV1.baseCommon.enmDBValueType.Texts);
                        }
                        else
                        {
                            sTransNos += "," + SBM_BLV1.baseCommon.ValidDBValue(gvr.Cells[2].Text, SBM_BLV1.baseCommon.enmDBValueType.Texts);
                        }
                        iSelTrans += 1;
                    }
                }

                this.txtStTransSelected.Text = iSelTrans.ToString();
                if (string.IsNullOrEmpty(sTransNos.Trim()))
                {
                    txtStTtlCrdtTrns.Text = "0";
                    txtStTtlCrAmount.Text = "0.00";
                    txtStTtlDbAmnt.Text = "0.00";
                }
                else
                {
                    if (oDataUploadDAL == null)
                    {
                        oDataUploadDAL = new DataUploadDAL();
                    }

                    if (dtDetailData == null)
                    {
                        Result oResult = oDataUploadDAL.GetDetailsDataToSelect(ddlUserName.SelectedValue, Convert.ToByte(ddlJournalType.SelectedValue));
                        if (oResult.Status)
                        {
                            dtDetailData = oResult.Return as DataTable;
                        }
                    }

                    txtStTtlCrdtTrns.Text = SBM_BLV1.baseCommon.NullTerminatedValue(dtDetailData.Compute("COUNT(AccTransactionNo)", "AccTransactionNo IN(" + sTransNos + ") AND DrCr = " + SBM_BLV1.baseCommon.ValidDBValue(oDataUploadDAL.GetCRCode().Return, SBM_BLV1.baseCommon.enmDBValueType.Texts)));
                    txtStTtlCrAmount.Text = Util.GetDecimalNumber(SBM_BLV1.baseCommon.NullTerminatedValue(dtDetailData.Compute("SUM(amount)", "AccTransactionNo IN(" + sTransNos + ") AND DrCr = " + SBM_BLV1.baseCommon.ValidDBValue(oDataUploadDAL.GetCRCode().Return, SBM_BLV1.baseCommon.enmDBValueType.Texts)))).ToString("N2");
                    txtStTtlDbAmnt.Text = Util.GetDecimalNumber(SBM_BLV1.baseCommon.NullTerminatedValue(dtDetailData.Compute("SUM(amount)", "AccTransactionNo IN(" + sTransNos + ") AND DrCr = " + SBM_BLV1.baseCommon.ValidDBValue(oDataUploadDAL.GetDRCode().Return, SBM_BLV1.baseCommon.enmDBValueType.Texts)))).ToString("N2");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetDetailsGridData(DataTable dtDetatilTmp, GridViewRow gvr)
        {
            DataTable dtTmp = dtDetatilTmp.Clone();
            DataRow[] drRows = dtDetatilTmp.Select("AccTransactionNo = " + SBM_BLV1.baseCommon.ValidDBValue(gvr.Cells[2].Text, SBM_BLV1.baseCommon.enmDBValueType.Texts));

            for (int iDrRowIndx = 0; iDrRowIndx < drRows.Count(); iDrRowIndx++)
            {
                dtTmp.ImportRow(drRows[iDrRowIndx]);
            }

            dtTmp.Columns.Remove("AccTransactionNo");
            dtTmp.Columns.Remove("SeqNo");
            dtTmp.Columns.Remove("CurrencyID");
            dtTmp.Columns.Remove("TrCode");

            gvTransactionDetail.DataSource = dtTmp;
            gvTransactionDetail.DataBind();

        }

        protected void btnSelectAll_Click(object sender, EventArgs e)
        {
            SelectAllTrans(true);
        }

        protected void btnDeSelectAll_Click(object sender, EventArgs e)
        {
            SelectAllTrans(false);
        }

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            if (!rblCurrencyActivity.Items[0].Selected)
            {
                /*
                 Try
            Me.cBlnIsInProcess = True
            Me.DisableButtons(True)
            If Not modCommon.IsProcessing Then
                modCommon.StartProcessing(Me, Me.enmFunctionIndex.SaveRecord)
            Else
                Me.SaveRecord()
            End If
        Catch ex As Exception
            If InStr(ex.Message, "File not found.") <= 0 Then
                modCommon.ShowErrorMessage(ex, Me.Text)
            End If
        Finally
            Me.DisableButtons(False)
            Me.cBlnIsInProcess = False
        End Try
                 */
                //SBM_BLV1.baseCommon.S
                SaveRecord();
            }
        }

        protected void hdnLinkButton_Click(object sender, EventArgs e)
        {
            DataTable dtDataUpload = Session[Constants.SES_DATA_UPLOAD] as DataTable;

            if (dtDataUpload != null && dtDataUpload.Rows.Count > 0)
            {
                ddlJournalType.Text = DB.GetDBValue(dtDataUpload.Rows[0]["JournalType"]);
                ddlUserName.Text = DB.GetDBValue(dtDataUpload.Rows[0]["AccEntryOperator"]);
                LoadDataToUpdate(dtDataUpload);
            }

            SetGridData(null);
        }

        private bool SaveRecord()
        {
            string sFailMessage = string.Empty;
            string sSuccessMessage = string.Empty;
            SBM_BLV1.baseCommon.enmUploadStatus oActionTo = new SBM_BLV1.baseCommon.enmUploadStatus();
            /*
             Try
            
            
            If Me.rbtSelect.Checked Then
                strFailMessage = "Error occured while selecting data to make upload file. Please check..."
                strSuccessMessage = "Data are selected to make upload file and updated successfully."
                oActionTo = modCommon.enmUploadStatus.Selected
            ElseIf Me.rbtMake.Checked Then
                strFailMessage = "Error occured while making upload file. Please check..."
                strSuccessMessage = "Upload file made successfully in " & Me.txtFileName.Text & "."
                oActionTo = modCommon.enmUploadStatus.Upload_File_Created
            ElseIf Me.rbtUploaded.Checked Then
                strFailMessage = "Error occured while updating data as uploaded. Please check..."
                strSuccessMessage = "Data are updated successfully as uploaded and the upload file " & Me.txtFileName.Text & " can easily be used for further upload."
                oActionTo = modCommon.enmUploadStatus.Uploaded
            End If
            With Me.cOBusiness
                .JournalType = CByte(Me.cboJournalType._SelectedValue)
                .AccEntryOperator = Me.cboAccEntryOperator._SelectedValue
                .FileName = Me.txtFileName._TextToSave
                .OriginatorID = Me.txtOriginatorID._TextToSave
                .[Operator] = modCommon.UserName
                If Not Me.rbtSelect.Checked Then
                    .UploadTransactionNo = Me.txtTransactionNo._TextToSave
                    .FileSeqNo = CInt(Me.txtFileSeqNo._TextToSave)
                    .TransactionDate = CDate(Me.txtTransactionDate._TextToSave)
                    .TransactionTime = Me.txtTransactionTime._TextToSave
                End If
                If .SaveData(oActionTo, Me.cDtGridData.Select(.TableProperties.IsSelected & " = " & True), Me.cDtDetailsData) Then
                    If .UploadStatus = modCommon.enmUploadStatus.Upload_File_Created And Me.rbtUploaded.Checked Then
                        MessageBox.Show("Data are updated successfully as uploaded", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
                    Else
                        MessageBox.Show(strSuccessMessage, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
                    End If
                    Me.LoadData()
                Else
                    MessageBox.Show(strFailMessage, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                End If
            End With
        Catch ex As Exception
             */
            if (!CheckForUpload())
            {
                return false;
            }

            if (Util.GetIntNumber(txtStTransSelected.Text).Equals(0))
            {
                ucMessage.OpenMessage("Please select at least one transaction.", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
            else
            {
                if (rblCurrencyActivity.Items[1].Selected)
                {
                    sFailMessage = "Error occured while selecting data to make upload file. Please check...";
                    sSuccessMessage = "Data are selected to make upload file and updated successfully.";
                    oActionTo = SBM_BLV1.baseCommon.enmUploadStatus.Selected;
                }
                else if (rblCurrencyActivity.Items[2].Selected)
                {
                    sFailMessage = "Error occured while making upload file. Please check...";
                    sSuccessMessage = "Upload file made successfully in " + txtStUpldFilePth.Text + ".";
                    oActionTo = SBM_BLV1.baseCommon.enmUploadStatus.Upload_File_Created;
                }
                else if (rblCurrencyActivity.Items[3].Selected)
                {
                    sFailMessage = "Error occured while updating data as uploaded. Please check...";
                    sSuccessMessage = "Data are updated successfully as uploaded and the upload file " + txtStUpldFilePth.Text + " can easily be used for further upload.";
                    oActionTo = SBM_BLV1.baseCommon.enmUploadStatus.Uploaded;
                }

                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];

                SaveData oSaveData = new SaveData();
                oSaveData.JournalType = ddlJournalType.SelectedValue;
                oSaveData.AccEntryOperator = ddlUserName.SelectedValue;
                oSaveData.FileName = txtStUpldFilePth.Text;
                oSaveData.OriginatorID = txtStOriginatorID.Text;
                oSaveData.DivisionID = oConfig.DivisionID;
                oSaveData.BankID = oConfig.BankCodeID;
                oSaveData.MakerID = oConfig.UserName;

                if (!rblCurrencyActivity.Items[1].Selected)
                {
                    oSaveData.UploadTransactionNo = txtTransacNo.Text;
                    oSaveData.FileSeqNo = Util.GetIntNumber(txtStFileSeqNo.Text);
                    oSaveData.TransactionDate = Util.GetDateTimeByString(txtStTransDate.Text);
                    oSaveData.TransactionTime = txtStTransTime.Text;
                }

                DataTable dtSelectData = GetSelectedData();
                DataTable dtDetailData = null;

                DataUploadDAL oDataUploadDAL = new DataUploadDAL();
                Result oResult = oDataUploadDAL.GetDetailsDataToUpdate(txtTransacNo.Text);

                if (oResult.Status)
                {
                    dtDetailData = oResult.Return as DataTable;
                }

                oResult = oDataUploadDAL.SaveData(oSaveData, oActionTo, dtSelectData, dtDetailData);
                if (oResult.Status)
                {
                    Byte bytUploadStatus = (Byte)oResult.Return;
                    if (bytUploadStatus.Equals((Byte)SBM_BLV1.baseCommon.enmUploadStatus.Upload_File_Created) && rblCurrencyActivity.Items[3].Selected)
                    {
                        ucMessage.OpenMessage("Data are updated successfully as uploaded", Constants.MSG_TYPE_SUCCESS);                       
                    }
                    else
                    {
                        ucMessage.OpenMessage(sSuccessMessage, Constants.MSG_TYPE_SUCCESS);                        
                    }

                    LoadData();
                }
                else
                {
                    ucMessage.OpenMessage(sFailMessage + "\n Source: "+ oResult.Message , Constants.MSG_TYPE_ERROR);
                    return false;
                }
            }

            return true;
        }

        private DataTable GetSelectedData()
        {
            DataTable dtSelected = new DataTable();
            dtSelected.Columns.Add(new DataColumn("IsSelected", typeof(bool)));
            dtSelected.Columns.Add(new DataColumn("AccTransactionNo", typeof(string)));
            dtSelected.Columns.Add(new DataColumn("TransactionDate", typeof(DateTime)));
            dtSelected.Columns.Add(new DataColumn("Narration", typeof(string)));

            foreach (GridViewRow gvr in gvTransactionList.Rows)
            {
                if ((gvr.FindControl("chkSelect") as CheckBox).Checked)
                {
                    DataRow dr = dtSelected.NewRow();
                    dr["IsSelected"] = true;
                    dr["AccTransactionNo"] = gvr.Cells[2].Text;
                    dr["TransactionDate"] = Util.GetDateTimeByString(gvr.Cells[3].Text);
                    dr["Narration"] = gvr.Cells[4].Text;

                    dtSelected.Rows.Add(dr);
                }
            }

            return dtSelected;
        }

        private void ClearData(bool isRestClicked)
        {
            if (isRestClicked)
            {
                ddlUserName.SelectedIndex = 0;
                ddlJournalType.SelectedIndex = 0;
            }
            SetRadioButtonIndx(0);
            lblActivity.Text = rblCurrencyActivity.Items[0].Value;
            btnSelect.Text = "None";
            txtTransacNo.Text = string.Empty;
            txtStFileSeqNo.Text = string.Empty;
            txtStOriginatorID.Text = string.Empty;
            txtStMaker.Text = string.Empty;
            txtStTransDate.Text = string.Empty;
            txtStTransTime.Text = string.Empty;
            txtStUpldFilePth.Text = string.Empty;

            gvTransactionList.Enabled = true;
            gvTransactionList.DataSource = null;
            gvTransactionList.DataBind();

            gvTransactionDetail.DataSource = null;
            gvTransactionDetail.DataBind();

            txtStTtlCrdtTrns.Text = "0";
            txtStTransSelected.Text = "0";
            txtStTtlCrAmount.Text = "0.00";
            txtStTtlDbAmnt.Text = "0.00";
        }

        private void SetRadioButtonIndx(int iRdbIndx)
        {
            for (int iIndx = 0; iIndx < rblCurrencyActivity.Items.Count; iIndx++)
            {
                if (iIndx.Equals(iRdbIndx))
                {
                    rblCurrencyActivity.Items[iIndx].Selected = true;
                }
                else
                {
                    rblCurrencyActivity.Items[iIndx].Selected = false;
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (!rblCurrencyActivity.Items[2].Selected)
            {
                ucMessage.OpenMessage("Only marked data can be washed out..", Constants.MSG_TYPE_INFO);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
            else
            {
                CancelData();
            }
        }

        private void CancelData()
        {
            string sFailMessage = string.Empty;
            string sSuccessMessage = string.Empty;
            SBM_BLV1.baseCommon.enmUploadStatus oActionTo = SBM_BLV1.baseCommon.enmUploadStatus.Canceled;

            sFailMessage = "Error occured while washed the selection of data for making upload file. Please check...";
            sSuccessMessage = "Selection of data for making upload file is successfully washed out...";

            DataUploadDAL oDataUploadDAL = new DataUploadDAL();
            Result oResult = oDataUploadDAL.CancelData(txtTransacNo.Text, oActionTo);
            if (oResult.Status)
            {
                LoadData();
                ucMessage.OpenMessage(sSuccessMessage, Constants.MSG_TYPE_SUCCESS);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);                
            }
            else
            {
                ucMessage.OpenMessage(sFailMessage, Constants.MSG_TYPE_ERROR);
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
            }
        }

        public void PopErrorMsgAction(string sType)
        {
            ucMessage.ResetMessage("Operation in progress...");
        }

        protected void chkSelect_OnCheckedChanged(object sender, EventArgs e)
        {
            GridViewRow gvr = (GridViewRow)((DataControlFieldCell)((CheckBox)sender).Parent).Parent;
            DataUploadDAL oDataUploadDAL = new DataUploadDAL();
            Result oResult = null;
            if (string.IsNullOrEmpty(txtTransacNo.Text))
            {
                oResult = oDataUploadDAL.GetDetailsDataToSelect(ddlUserName.SelectedValue, Convert.ToByte(ddlJournalType.SelectedValue));
            }
            else
            {
                oResult = oDataUploadDAL.GetDetailsDataToUpdate(txtTransacNo.Text);
            }
            if (oResult != null)
            {
                if (oResult.Status)
                {
                    SetDetailsGridData(oResult.Return as DataTable, gvr);
                    UpdateSummary(oResult.Return as DataTable, oDataUploadDAL);
                }
            }            
        }

        protected void gvTransactionList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string sButtonText = ((Button)e.CommandSource).CommandName;
            if (sButtonText.Equals("Select"))
            {                
                GridViewRow gvRow = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                DataUploadDAL oDataUploadDAL = new DataUploadDAL();
                Result oResult = null;
                if (string.IsNullOrEmpty(txtTransacNo.Text))
                {
                    oResult = oDataUploadDAL.GetDetailsDataToSelect(ddlUserName.SelectedValue, Convert.ToByte(ddlJournalType.SelectedValue));
                }
                else
                {
                    oResult = oDataUploadDAL.GetDetailsDataToUpdate(txtTransacNo.Text);
                }
                if (oResult != null)
                {
                    if (oResult.Status)
                    {
                        SetDetailsGridData(oResult.Return as DataTable, gvRow);                        
                    }
                }   
            }
        }

        protected void btnResetRemain_Click(object sender, EventArgs e)
        {
            /*
             If Not Me.rbtUploaded.Checked Then
                If Me.rbtMake.Checked Then
                    MessageBox.Show("Data for which an Upload file is created can only be reseted.", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
                    Exit Sub
                End If
            End If
             */
            if (!rblCurrencyActivity.Items[3].Selected)
            {
                if (rblCurrencyActivity.Items[2].Selected)
                {
                    ucMessage.OpenMessage("Data for which an Upload file is created can only be reseted.", Constants.MSG_TYPE_INFO);
                    ScriptManager.RegisterStartupScript(this.UpdatePanel2, typeof(string), Constants.POPUP_WINDOW, Util.OpenPopup("info"), true);
                }
            }
            ClearData(true);
        }

        protected void ddlUserName_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

    }
}
