using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using SBM_BLC1.Common;

namespace SBM_WebUI.mp
{
    public partial class DownloadEBBSCustomer : System.Web.UI.Page
    {
        private int iProcessed, iImported, iUncompleted;
        private static DataTable dtFileType;
        private static DataTable _dtError;
        protected void Page_PreInit(object sender, EventArgs e)
        {
            //this.PageAuthen(false, Convert.ToInt32(ScreenID.ImportData));
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ScriptManager sm;
                sm = (ScriptManager)this.Master.FindControl("ToolkitScriptManager1");
                sm.AsyncPostBackTimeout = 96000;

                if (!IsPostBack)
                {
                    Util.InvalidateSession();

                    if (Session["ConStr"] == null)
                        Session["ConStr"] = ConfigurationManager.ConnectionStrings["SPMSConnectionString"].ConnectionString;
                    //This is for Page Permission
                    CheckPermission chkPer = new CheckPermission();
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    if (!chkPer.CheckPagePermission((Control)this.Page, oConfig, (int)Constants.PAGEINDEX_UTILITY.DOWNLOAD_EBBS_CUSTOMER))
                    {
                        Response.Redirect(Constants.PAGE_ERROR, false);
                    }
                    //End Of Page Permission


                    //this.LoadCombo();
                    this.SetDate();
                    this.ResetStatus();
                }
                this.PrepareImportHistory();
            }
            catch (Exception ex)
            {

                lblMessage.Text = ex.Message;
            }
        }
        private void PrepareImportHistory()
        {
            try
            {
                BLImport im = new BLImport(Convert.ToString(Session["ConStr"]));
                grdFileProcessHistory.DataSource = im.GetFileProcessingHistory();
                grdFileProcessHistory.DataBind();
            }
            catch (Exception ex)
            {

                lblMessage.Text = ex.Message;
            }
        }
        private void SetDate()
        {
            try
            {

                this.txtImportDate.Text = DateTime.Today.ToString("dd-MMM-yyyy");
            }

            catch (Exception ex)
            {

                throw ex;
            }
        }
        protected void ddlFileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txtFilePath.Text = Convert.ToString(ddlFileType.SelectedValue);
                BLImport im = new BLImport(Convert.ToString(Session["ConStr"]));
                string sFilePath="";
                string sFullPath="";

                sFullPath = txtFilePath.Text + ddlFileType.SelectedItem.Text + ".txt";
                txtFileCreationDate.Text = im.FileCreationTime(sFullPath).ToString("dd-MMM-yyyy h:mm:ss tt");
                try
                {
                    if (DateTime.Parse(txtFileCreationDate.Text) < DateTime.Parse("01-Jan-1900"))
                    {
                        throw new Exception("Invalid file");
                    }
                }
                catch (Exception ex)
                {

                    lblMessage.Text = "File not found";
                    return;
                }
                this.ResetStatus();
                if (im.IsFileExist((enmFileID)ddlFileType.SelectedIndex, Convert.ToDateTime(txtFileCreationDate.Text)))
                {
                    throw new Exception(ddlFileType.SelectedItem.Text + " already imported. Please check. . ");
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
        private void ProcessBDDB1()
        {
            string ConnectionStr="Data Source=BDWPAPSPM01\\SPM_SQL01;Initial Catalog=SPMSDB; Persist Security Info=True;User ID=spms;Password=team@SBM; Connection Timeout=28800";
            //BLImportBDDB1 im = new BLImportBDDB1(Convert.ToString(Session["ConStr"]), this.txtFilePath.Text);
            BLImportBDDB1 im = new BLImportBDDB1(ConnectionStr, this.txtFilePath.Text);

            im.pMsiImpDate = Convert.ToDateTime(this.txtImportDate.Text);

            iProcessed = 0;
            iImported = 0;
            iUncompleted = 0;
            iProcessed = im.GetFileRowCount();

            System.Data.OleDb.OleDbDataReader drd;

            im.Delete();

            drd = im.GetFileDataReader();
            _dtError = im.GetDataTable();
            DataRow drErrorData;
            try
            {
                stBDDB1 dBDDB1 = new stBDDB1();
                while (drd.Read())
                {
                    dBDDB1.MASTER = (drd["MASTER"] == DBNull.Value ? "" : ValidDBString((string)drd["MASTER"]));
                    dBDDB1.BR = (drd["BR"] == DBNull.Value ? "" : (string)drd["BR"]);
                    dBDDB1.NAME1 = (drd["NAME1"] == DBNull.Value ? "" : this.ValidDBString((string)drd["NAME1"]));
                    dBDDB1.NAME2 = (drd["NAME2"] == DBNull.Value ? "" : this.ValidDBString((string)drd["NAME2"]));
                    dBDDB1.NAME3 = (drd["NAME3"] == DBNull.Value ? "" : this.ValidDBString((string)drd["NAME3"]));
                    dBDDB1.ADD1 = (drd["ADD1"] == DBNull.Value ? "" : this.ValidDBString((string)drd["ADD1"]));
                    dBDDB1.ADD2 = (drd["ADD2"] == DBNull.Value ? "" : this.ValidDBString((string)drd["ADD2"]));
                    dBDDB1.ADD3 = (drd["ADD3"] == DBNull.Value ? "" : this.ValidDBString((string)drd["ADD3"]));
                    dBDDB1.ADD4 = (drd["ADD4"] == DBNull.Value ? "" : this.ValidDBString((string)drd["ADD4"]));
                    dBDDB1.TELEPHONE = (drd["TELC"] == DBNull.Value ? "" : (string)drd["TELC"]);
                    dBDDB1.TELEPHONE += (drd["TELR"] == DBNull.Value ? "" : (string)drd["TELR"]);
                    dBDDB1.TELEPHONE = this.ValidDBString(dBDDB1.TELEPHONE);

                    try
                    {
                        im.Save(dBDDB1);
                        iImported += 1;
                    }
                    catch (Exception ex)
                    {
                        drErrorData = _dtError.NewRow();
                        drErrorData["MASTER"] = dBDDB1.MASTER;
                        drErrorData["BR"] = dBDDB1.BR;
                        drErrorData["NAME1"] = dBDDB1.NAME1;
                        drErrorData["NAME2"] = dBDDB1.NAME2;
                        drErrorData["NAME3"] = dBDDB1.NAME3;
                        drErrorData["ERROR"] = ex.Message;
                        _dtError.Rows.Add(drErrorData);
                        iUncompleted += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblMessage.Text = ex.Message;
            }
            finally
            {
                drd.Close();
                im.CloseLocal();
                im = null;
            }

        }
        private void ProcessBDDB2()
        {

            string ConnectionStr = "Data Source=BDWPAPSPM01\\SPM_SQL01;Initial Catalog=SPMSDB; Persist Security Info=True;User ID=spms;Password=team@SBM; Connection Timeout=28800";
            //BLImportBDDB2 im = new BLImportBDDB2(Convert.ToString(Session["ConStr"]), this.txtFilePath.Text);
            BLImportBDDB2 im = new BLImportBDDB2(ConnectionStr, this.txtFilePath.Text);

            im.pMsiImpDate = Convert.ToDateTime(this.txtImportDate.Text);

            iProcessed = 0;
            iImported = 0;
            iUncompleted = 0;
            iProcessed = im.GetFileRowCount();

            System.Data.OleDb.OleDbDataReader drd;

            im.Delete();

            drd = im.GetFileDataReader();


            _dtError = im.GetDataTable();
            DataRow drErrorData;
            try
            {
                stBDDB2 dBDDB2 = new stBDDB2();
                while (drd.Read())
                {
                    if (iImported == 395860)
                    {
                        dBDDB2.MASTER = (drd["MASTER"] == DBNull.Value ? "" : (string)drd["MASTER"]);
                    }
                    dBDDB2.BR = (drd["BR"] == DBNull.Value ? "" : (string)drd["BR"]);
                    dBDDB2.MASTER = (drd["MASTER"] == DBNull.Value ? "" : (string)drd["MASTER"]);
                    dBDDB2.NAME = (drd["NAME"] == DBNull.Value ? "" : this.ValidDBString((string)drd["NAME"]));
                    dBDDB2.FULLNO = (drd["FULLNO"] == DBNull.Value ? "" : (string)drd["FULLNO"]);
                    dBDDB2.CCY = (drd["CCY"] == DBNull.Value ? "" : (string)drd["CCY"]);
                    dBDDB2.LBAL = (drd["LBAL"] == DBNull.Value ? "" : (string)drd["LBAL"]);
                    dBDDB2.LBALFLAG = (drd["LBALFLAG"] == DBNull.Value ? "" : (string)drd["LBALFLAG"]);
                    dBDDB2.OPEN = (drd["OPEN"] == DBNull.Value ? "" : (string)drd["OPEN"]);
                    dBDDB2.ACLAS = (drd["ACLAS"] == DBNull.Value ? "" : (string)drd["ACLAS"]);
                    dBDDB2.CRIND = (drd["CRIND"] == DBNull.Value ? "" : (string)drd["CRIND"]);
                    dBDDB2.CRATE = (drd["CRATE"] == DBNull.Value ? "" : (string)drd["CRATE"]);
                    dBDDB2.DRIND = (drd["DRIND"] == DBNull.Value ? "" : (string)drd["DRIND"]);
                    dBDDB2.DRATE = (drd["DRATE"] == DBNull.Value ? "" : (string)drd["DRATE"]);
                    dBDDB2.ODLIMIT = (drd["ODLIMIT"] == DBNull.Value ? "" : (string)drd["ODLIMIT"]);
                    dBDDB2.LIMFLAG = (drd["LIMFLAG"] == DBNull.Value ? "" : (string)drd["LIMFLAG"]);
                    dBDDB2.EXP = (drd["EXP"] == DBNull.Value ? "" : (string)drd["EXP"]);
                    dBDDB2.FDMAT = (drd["FDMAT"] == DBNull.Value ? "" : (string)drd["FDMAT"]);
                    dBDDB2.STAFF = (drd["STAFF"] == DBNull.Value ? "" : (string)drd["STAFF"]);
                    dBDDB2.SEG = (drd["SEG"] == DBNull.Value ? "" : (string)drd["SEG"]);
                    dBDDB2.PRD = (drd["PRD"] == DBNull.Value ? "" : (string)drd["PRD"]);
                    dBDDB2.BDTEQUIV = (drd["BDTEQUIV"] == DBNull.Value ? "" : (string)drd["BDTEQUIV"]);
                    dBDDB2.BDTFLAG = (drd["BDTFLAG"] == DBNull.Value ? "" : (string)drd["BDTFLAG"]);
                    dBDDB2.TERM = (drd["TERM"] == DBNull.Value ? "" : (string)drd["TERM"]);
                    dBDDB2.CRINTDUE = (drd["CRINTDUE"] == DBNull.Value ? "" : (string)drd["CRINTDUE"]);
                    dBDDB2.CRINTACNO = (drd["CRINTACNO"] == DBNull.Value ? "" : (string)drd["CRINTACNO"]);
                    dBDDB2.DRINTDUE = (drd["DRINTDUE"] == DBNull.Value ? "" : (string)drd["DRINTDUE"]);
                    dBDDB2.DRINTACNO = (drd["DRINTACNO"] == DBNull.Value ? "" : (string)drd["DRINTACNO"]);
                    dBDDB2.RELNO = (drd["RELNO"] == DBNull.Value ? "" : (string)drd["RELNO"]);


                    try
                    {
                        im.Save(dBDDB2);
                        iImported += 1;
                    }
                    catch (Exception ex)
                    {
                        drErrorData = _dtError.NewRow();
                        try
                        {
                            drErrorData["MASTER"] = dBDDB2.MASTER;
                            drErrorData["BR"] = dBDDB2.BR;
                            drErrorData["NAME"] = dBDDB2.NAME;
                            drErrorData["FULLNO"] = dBDDB2.FULLNO;
                            drErrorData["CCY"] = dBDDB2.CCY;
                            drErrorData["ERROR"] = ex.Message;
                            _dtError.Rows.Add(drErrorData);
                            iUncompleted += 1;
                        }
                        catch (Exception)
                        {

                            iUncompleted += 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblMessage.Text = ex.Message;
            }
            finally
            {
                drd.Close();
                im.CloseLocal();
                im = null;
            }

        }
        private void ProcessTXN()
        {

            BLImportTXN im = new BLImportTXN(Convert.ToString(Session["ConStr"]), this.txtFilePath.Text);
            im.pMsiImpDate = Convert.ToDateTime(this.txtImportDate.Text);

            iProcessed = 0;
            iImported = 0;
            iUncompleted = 0;
            iProcessed = im.GetFileRowCount();

            System.Data.OleDb.OleDbDataReader drd;

            im.Delete();

            drd = im.GetFileDataReader();

            _dtError = im.GetDataTable();
            DataRow drErrorData;
            try
            {
                stTXN dBTXN = new stTXN();
                while (drd.Read())
                {

                    dBTXN.TXNBR = (drd["TXNBR"] == DBNull.Value ? "" : (string)drd["TXNBR"]);
                    dBTXN.FULLNO = (drd["FULLNO"] == DBNull.Value ? "" : (string)drd["FULLNO"]);
                    dBTXN.VALUE = (drd["VALUE"] == DBNull.Value ? "" : (string)drd["VALUE"]);
                    dBTXN.TXNDATE = (drd["TXNDATE"] == DBNull.Value ? "" : (string)drd["TXNDATE"]);
                    dBTXN.CCY = (drd["CCY"] == DBNull.Value ? "" : (string)drd["CCY"]);
                    dBTXN.DRAMT = (drd["DRAMT"] == DBNull.Value ? "" : (string)drd["DRAMT"]);
                    dBTXN.CRAMT = (drd["CRAMT"] == DBNull.Value ? "" : (string)drd["CRAMT"]);
                    dBTXN.TRANSCODE = (drd["TRANSCODE"] == DBNull.Value ? "" : (string)drd["TRANSCODE"]);
                    dBTXN.MAKID = (drd["MAKID"] == DBNull.Value ? "" : (string)drd["MAKID"]);


                    try
                    {
                        im.Save(dBTXN);
                        iImported += 1;
                    }
                    catch (Exception ex)
                    {
                        drErrorData = _dtError.NewRow();

                        drErrorData["TXNBR"] = dBTXN.TXNBR;
                        drErrorData["CCY"] = dBTXN.CCY;
                        drErrorData["FULLNO"] = dBTXN.FULLNO;
                        drErrorData["VALUE"] = dBTXN.VALUE;
                        drErrorData["TXNDATE"] = dBTXN.TXNDATE;
                        drErrorData["ERROR"] = ex.Message;
                        _dtError.Rows.Add(drErrorData);
                        iUncompleted += 1;
                    }
                    this.SetUpdateStatus();
                }
            }
            catch (Exception ex)
            {
                this.lblMessage.Text = ex.Message;
            }
            finally
            {
                drd.Close();
                im.CloseLocal();
                im = null;
            }

        }
        private void ProcessSOBB4()
        {

            BLImportSOBB4 im = new BLImportSOBB4(Convert.ToString(Session["ConStr"]), this.txtFilePath.Text);
            im.pMsiImpDate = Convert.ToDateTime(this.txtImportDate.Text);

            iProcessed = 0;
            iImported = 0;
            iUncompleted = 0;
            iProcessed = im.GetFileRowCount();


            System.Data.OleDb.OleDbDataReader drd;

            im.Delete();

            drd = im.GetFileDataReader();


            _dtError = im.GetDataTable();
            DataRow drErrorData;
            try
            {
                stSOBB4 dBSOBB4 = new stSOBB4();
                while (drd.Read())
                {

                    dBSOBB4.BR = (drd["BR"] == DBNull.Value ? "" : (string)drd["BR"]);
                    dBSOBB4.CCY = (drd["CCY"] == DBNull.Value ? "" : (string)drd["CCY"]);
                    dBSOBB4.DR = (drd["DR"] == DBNull.Value ? "" : (string)drd["DR"]);
                    dBSOBB4.DRACC = (drd["DRACC"] == DBNull.Value ? "" : (string)drd["DRACC"]);
                    dBSOBB4.DRACCNO = (drd["DRACCNO"] == DBNull.Value ? "" : (string)drd["DRACCNO"]);
                    dBSOBB4.INSNO = (drd["INSNO"] == DBNull.Value ? "" : (string)drd["INSNO"]);
                    dBSOBB4.INSDT = (drd["INSDT"] == DBNull.Value ? "" : (string)drd["INSDT"]);
                    dBSOBB4.STDT = (drd["STDT"] == DBNull.Value ? "" : (string)drd["STDT"]);
                    dBSOBB4.STAMT = (drd["STAMT"] == DBNull.Value ? "" : (string)drd["STAMT"]);
                    dBSOBB4.ENDDT = (drd["ENDDT"] == DBNull.Value ? "" : (string)drd["ENDDT"]);
                    dBSOBB4.ENDAMT = (drd["ENDAMT"] == DBNull.Value ? "" : (string)drd["ENDAMT"]);
                    dBSOBB4.FREC = (drd["FREC"] == DBNull.Value ? "" : (string)drd["FREC"]);
                    dBSOBB4.NRR = (drd["NRR"] == DBNull.Value ? "" : (string)drd["NRR"]);
                    dBSOBB4.NRR = dBSOBB4.NRR.Replace("'", "''");
                    dBSOBB4.CR = (drd["CR"] == DBNull.Value ? "" : (string)drd["CR"]);
                    dBSOBB4.CRACC = (drd["CRACC"] == DBNull.Value ? "" : (string)drd["CRACC"]);
                    dBSOBB4.CRACCNO = (drd["CRACCNO"] == DBNull.Value ? "" : (string)drd["CRACCNO"]);


                    try
                    {
                        im.Save(dBSOBB4);
                        iImported += 1;
                    }
                    catch (Exception ex)
                    {
                        drErrorData = _dtError.NewRow();

                        drErrorData["BR"] = dBSOBB4.BR;
                        drErrorData["CCY"] = dBSOBB4.CCY;
                        drErrorData["DRACCNO"] = dBSOBB4.DRACCNO;
                        drErrorData["INSNO"] = dBSOBB4.INSNO;
                        drErrorData["CRACCNO"] = dBSOBB4.CRACCNO;
                        drErrorData["ERROR"] = ex.Message;
                        _dtError.Rows.Add(drErrorData);
                        iUncompleted += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblMessage.Text = ex.Message;
            }
            finally
            {
                drd.Close();
                im.CloseLocal();
                im = null;
            }

        }
        private void ProcessInfoCD()
        {
            BLImportInfoCD im = new BLImportInfoCD(Convert.ToString(Session["ConStr"]), this.txtFilePath.Text);
            im.pMsiImpDate = Convert.ToDateTime(this.txtImportDate.Text);

            iProcessed = 0;
            iImported = 0;
            iUncompleted = 0;
            iProcessed = im.GetFileRowCount();

            System.Data.OleDb.OleDbDataReader drd;

            im.Delete();

            drd = im.GetFileDataReader();


            _dtError = im.GetDataTable();
            DataRow drErrorData;

            try
            {
                stInfoCD dBInfoCD = new stInfoCD();
                while (drd.Read())
                {

                    dBInfoCD.MASTER = (drd["MASTER"] == DBNull.Value ? "" : (string)drd["MASTER"]);
                    dBInfoCD.ACCOUNT = (drd["ACCOUNT"] == DBNull.Value ? "" : (string)drd["ACCOUNT"]);
                    dBInfoCD.CCY = (drd["CCY"] == DBNull.Value ? "" : (string)drd["CCY"]);
                    dBInfoCD.SECTOR = (drd["SECTOR"] == DBNull.Value ? "" : (string)drd["SECTOR"]);
                    dBInfoCD.BTYPE = (drd["BTYPE"] == DBNull.Value ? "" : (string)drd["BTYPE"]);
                    dBInfoCD.DTYPE = (drd["DTYPE"] == DBNull.Value ? "" : (string)drd["DTYPE"]);
                    dBInfoCD.EPURPOSE = (drd["EPURPOSE"] == DBNull.Value ? "" : (string)drd["EPURPOSE"]);
                    dBInfoCD.SCODE = (drd["SCODE"] == DBNull.Value ? "" : (string)drd["SCODE"]);

                    try
                    {
                        im.Save(dBInfoCD);
                        iImported += 1;
                    }
                    catch (Exception ex)
                    {
                        drErrorData = _dtError.NewRow();

                        drErrorData["MASTER"] = dBInfoCD.MASTER;
                        drErrorData["ACCOUNT"] = dBInfoCD.ACCOUNT;
                        drErrorData["CCY"] = dBInfoCD.CCY;
                        drErrorData["SECTOR"] = dBInfoCD.SECTOR;
                        drErrorData["ERROR"] = ex.Message;
                        _dtError.Rows.Add(drErrorData);
                        iUncompleted += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblMessage.Text = ex.Message;
            }
            finally
            {
                drd.Close();
                im.CloseLocal();
                im = null;
            }

        }
        private void ProcessXLSheet()
        {
            BLImportXLSheet im = new BLImportXLSheet(Convert.ToString(Session["ConStr"]), this.txtFilePath.Text);
            im.pMsiImpDate = Convert.ToDateTime(this.txtImportDate.Text);

            iProcessed = 0;
            iImported = 0;
            iUncompleted = 0;
            iProcessed = im.GetFileRowCount();

            System.Data.OleDb.OleDbDataReader drd;

            im.Delete();

            drd = im.GetFileDataReader();


            _dtError = im.GetDataTable();
            DataRow drErrorData;

            try
            {
                stXLSheet dBXLSheet = new stXLSheet();
                while (drd.Read())
                {

                    dBXLSheet.BBSMASTER = (drd["BBSMASTER"] == DBNull.Value ? "" : Convert.ToString(drd["BBSMASTER"]));
                    dBXLSheet.CUSTTITLE = (drd["CustomerTitle"] == DBNull.Value ? "" : this.ValidDBString(Convert.ToString(drd["CustomerTitle"])));
                    dBXLSheet.CUSTTITLE = dBXLSheet.CUSTTITLE.Replace("'", "''");

                    dBXLSheet.CUSTNAME = (drd["CustomerName"] == DBNull.Value ? "" : this.ValidDBString(Convert.ToString(drd["CustomerName"])));
                    dBXLSheet.CUSTNAME = dBXLSheet.CUSTNAME.Replace("'", "''");

                    dBXLSheet.CTYPE = (drd["CustomerType"] == DBNull.Value ? "" : Convert.ToString(drd["CustomerType"]));
                    dBXLSheet.OWNSN = (drd["OwnerSN"] == DBNull.Value ? "" : Convert.ToString(drd["OwnerSN"]));
                    dBXLSheet.OCODE = (drd["OwnerCode"] == DBNull.Value ? "" : Convert.ToString(drd["OwnerCode"]));
                    dBXLSheet.BRANCHCODE = (drd["BranchCode"] == DBNull.Value ? "" : Convert.ToString(drd["BranchCode"]));
                    dBXLSheet.NID = (drd["NID"] == DBNull.Value ? "" : Convert.ToString(drd["NID"]));
                    dBXLSheet.BCODE = (drd["BCODE"] == DBNull.Value ? "" : Convert.ToString(drd["BCODE"]));
                    dBXLSheet.SNAME = (drd["SNAME"] == DBNull.Value ? "NA" : this.ValidDBString(Convert.ToString(drd["SNAME"])));
                    dBXLSheet.SNAME = dBXLSheet.SNAME.Replace("'", "''");
                    dBXLSheet.NAME = (drd["NAME"] == DBNull.Value ? "" : this.ValidDBString(Convert.ToString(drd["NAME"])));
                    dBXLSheet.NAME = dBXLSheet.NAME.Replace("'", "''");
                    dBXLSheet.F_TITLE = (drd["F_TITLE"] == DBNull.Value ? "" : this.ValidDBString(Convert.ToString(drd["F_TITLE"])));
                    dBXLSheet.F_TITLE = dBXLSheet.F_TITLE.Replace("'", "''");
                    dBXLSheet.FNAME = (drd["FNAME"] == DBNull.Value ? "" : this.ValidDBString(Convert.ToString(drd["FNAME"])));
                    dBXLSheet.FNAME = dBXLSheet.FNAME.Replace("'", "''");
                    dBXLSheet.MNAME = (drd["MNAME"] == DBNull.Value ? "" : this.ValidDBString(Convert.ToString(drd["MNAME"])));
                    dBXLSheet.MNAME = dBXLSheet.MNAME.Replace("'", "''");
                    dBXLSheet.HNAME = (drd["HNAME"] == DBNull.Value ? "" : this.ValidDBString(Convert.ToString(drd["HNAME"])));
                    dBXLSheet.HNAME = dBXLSheet.HNAME.Replace("'", "''");
                    dBXLSheet.PADR1 = (drd["PADR1"] == DBNull.Value ? "" : this.ValidDBString(Convert.ToString(drd["PADR1"])));
                    dBXLSheet.PADR1 = dBXLSheet.PADR1.Replace("'", "''");
                    dBXLSheet.PADR2 = (drd["PADR2"] == DBNull.Value ? "" : this.ValidDBString(Convert.ToString(drd["PADR2"])));
                    dBXLSheet.PADR2 = dBXLSheet.PADR2.Replace("'", "''");
                    dBXLSheet.BADR1 = (drd["BADR1"] == DBNull.Value ? "" : this.ValidDBString(Convert.ToString(drd["BADR1"])));
                    dBXLSheet.BADR1 = dBXLSheet.BADR1.Replace("'", "''");
                    dBXLSheet.BADR2 = (drd["BADR2"] == DBNull.Value ? "" : this.ValidDBString(Convert.ToString(drd["BADR2"])));
                    dBXLSheet.BADR2 = dBXLSheet.BADR2.Replace("'", "''");
                    dBXLSheet.FADR1 = (drd["FADR1"] == DBNull.Value ? "" : this.ValidDBString(Convert.ToString(drd["FADR1"])));
                    dBXLSheet.FADR1 = dBXLSheet.FADR1.Replace("'", "''");
                    dBXLSheet.FADR2 = (drd["FADR2"] == DBNull.Value ? "" : this.ValidDBString(Convert.ToString(drd["FADR2"])));
                    dBXLSheet.FADR2 = dBXLSheet.FADR2.Replace("'", "''");
                    dBXLSheet.TIN = (drd["TIN"] == DBNull.Value ? "" : Convert.ToString(drd["TIN"]));
                    dBXLSheet.TEL = (drd["TEL"] == DBNull.Value ? "" : Convert.ToString(drd["TEL"]));
                    dBXLSheet.BTYPE = (drd["BTYPE"] == DBNull.Value ? "" : Convert.ToString(drd["BTYPE"]));
                    dBXLSheet.PIN = (drd["PIN"] == DBNull.Value ? "" : Convert.ToString(drd["PIN"]));
                    dBXLSheet.STYPE = (drd["STYPE"] == DBNull.Value ? "" : Convert.ToString(drd["STYPE"]));
                    dBXLSheet.SCODE = (drd["SCODE"] == DBNull.Value ? "" : Convert.ToString(drd["SCODE"]));
                    dBXLSheet.SEGMENT = (drd["SegmentCode"] == DBNull.Value ? "" : Convert.ToString(drd["SegmentCode"]));
                    dBXLSheet.INS_TYPE = (drd["INS_TYPE"] == DBNull.Value ? "" : Convert.ToString(drd["INS_TYPE"]));
                    try
                    {
                        im.Save(dBXLSheet);
                        iImported += 1;
                    }
                    catch (Exception ex)
                    {
                        drErrorData = _dtError.NewRow();

                        drErrorData["BBSMASTER"] = dBXLSheet.BBSMASTER;
                        drErrorData["CUSTOMERNAME"] = dBXLSheet.CUSTNAME;
                        drErrorData["OWNERSN"] = dBXLSheet.OWNSN;
                        drErrorData["OWNERCODE"] = dBXLSheet.OCODE;
                        drErrorData["ERROR"] = ex.Message;
                        _dtError.Rows.Add(drErrorData);
                        iUncompleted += 1;
                    }
                }
                lblMessage.Text = "File imported completed.";
            }
            catch (Exception ex)
            {
                this.lblMessage.Text = ex.Message;
            }
            finally
            {
                drd.Close();
                im.CloseLocal();
                im = null;
            }

        }
        private string ValidDBString(string vValue)
        {
            vValue = vValue.Replace("'", "''");
            return vValue;
        }
        protected void btnHome_Click(object sender, EventArgs e)
        {

            Response.Redirect("~/home.aspx");
        }
        private void PreparedErrorGrid()
        {
            this.grdBDDB1.DataSource = _dtError;
            this.grdBDDB1.DataBind();
        }
        private void ResetStatus()
        {
            lblMessage.Text = "";
            this.txtProcess.Text = "0";
            this.txtImported.Text = "0";
            this.txtUncompleted.Text = "0";
            grdBDDB1.DataSource = null;
            grdBDDB1.DataBind();
        }
        private bool IsValidate(enmFileID vFileID)
        {
            BLImport im = new BLImport(Convert.ToString(Session["ConStr"]));
            //im.CheckConsistency((enmFileID)ddlFileType.SelectedIndex);
            if (im.IsFileExist((enmFileID)ddlFileType.SelectedIndex, Convert.ToDateTime(txtFileCreationDate.Text)))
            {
                throw new Exception(ddlFileType.SelectedItem.Text + " already imported. Please check. . ");
            }
            if (im.IsImportExist(vFileID, Convert.ToDateTime(this.txtImportDate.Text)))
            {
                throw new Exception(ddlFileType.SelectedItem.Text + " already imported. Please check. . ");
            }
            if (vFileID != enmFileID.SOBB04 && vFileID != enmFileID.InfoCD)
            {
                if (im.IsProcessExist(vFileID))
                {
                    throw new Exception(ddlFileType.SelectedItem.Text + " already imported and Not yet processed. Please check. . ");
                }
            }
            //if (!im.DoArchived(vFileID))
            //{
            //    throw new Exception("Imported data still not archived. Please Check.");
            //}
            return true;
        }
        protected void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                this.IsValidate((enmFileID)ddlFileType.SelectedIndex);
                this.ResetStatus();

                BLImport im = new BLImport(Convert.ToString(Session["ConStr"]));

                if (this.ddlFileType.SelectedItem.Text == "BDDB1")
                {
                    //if (im.DoArchived(enmFileID.BDDB1) == false)
                    //{
                    //    throw new Exception("Import data archive operation fail. Please check. . .");
                    //}
                    this.ProcessBDDB1();
                }
                else if (this.ddlFileType.SelectedItem.Text == "BDDB2")
                {
                    //if (im.DoArchived(enmFileID.BDDB2) == false)
                    //{
                    //    throw new Exception("Import data archive operation fail. Please check. . .");
                    //}
                    this.ProcessBDDB2();
                }
                //else if (this.ddlFileType.SelectedItem.Text == "TXN")
                //{
                //    if (im.DoArchived(enmFileID.TXN) == false)
                //    {
                //        throw new Exception("Import data archive operation fail. Please check. . .");
                //    }
                //    this.ProcessTXN();
                //}
                //else if (this.ddlFileType.SelectedItem.Text == "SOBB4")
                //{
                //    if (im.DoArchived(enmFileID.SOBB04) == false)
                //    {
                //        throw new Exception("Import data archive operation fail. Please check. . .");
                //    }
                //    this.ProcessSOBB4();
                //}
                //else if (this.ddlFileType.SelectedItem.Text == "InfoCD")
                //{
                //    if (im.DoArchived(enmFileID.InfoCD) == false)
                //    {
                //        throw new Exception("Import data archive operation fail. Please check. . .");
                //    }
                //    this.ProcessInfoCD();
                //}
                //else if (this.ddlFileType.SelectedItem.Text == "XLSheet")
                //{
                //    if (im.DoArchived(enmFileID.XLSheet) == false)
                //    {
                //        throw new Exception("Import data archive operation fail. Please check. . .");
                //    }
                //    this.ProcessXLSheet();
                //}
                else
                {
                    throw new Exception("Invalid file selection. Please check");
                }
                this.txtProcess.Text = Convert.ToString(iProcessed);
                this.txtImported.Text = Convert.ToString(iImported);
                this.txtUncompleted.Text = Convert.ToString(iUncompleted);
                this.PreparedErrorGrid();
                this.SaveHistory((enmFileID)ddlFileType.SelectedIndex);
                if (_dtError.Rows.Count > 0)
                {
                    string sFileName, sFilePath;
                    sFileName = ddlFileType.SelectedItem.Text + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xml";
                    if (this.txtFilePath.Text.Substring(this.txtFilePath.Text.Length - 4, 4) == ".xls")
                    {
                        sFilePath = this.txtFilePath.Text.Substring(0, this.txtFilePath.Text.LastIndexOf("\\")) + "\\";
                    }
                    else
                    {
                        sFilePath = this.txtFilePath.Text + "\\";
                    }
                    _dtError.TableName = "Error";
                    _dtError.WriteXml(sFilePath + sFileName);
                }
                this.PrepareImportHistory();
            }
            catch (Exception ex)
            {
                this.lblMessage.Text = ex.Message;
            }
        }
        private int SaveHistory(enmFileID vFileID)
        {
            BLImport im = new BLImport(Convert.ToString(Session["ConStr"]));
            return im.SaveHistory(vFileID, Convert.ToDateTime(this.txtImportDate.Text), Convert.ToDateTime(txtFileCreationDate.Text), Convert.ToString(Session["gUserName"]));

        }
        private void SetUpdateStatus()
        {
            this.txtProcess.Text = Convert.ToString(iProcessed);
            this.txtImported.Text = Convert.ToString(iImported);
            this.txtUncompleted.Text = Convert.ToString(iUncompleted);
        }
        protected void grdBDDB1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.PreparedErrorGrid();
            grdBDDB1.PageIndex = e.NewPageIndex;
        }

        protected void grdFileProcessHistory_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int iFileID;
                DateTime dForDate;
                if (grdFileProcessHistory.Rows[Convert.ToInt32(e.CommandArgument)].Cells[7].Text == "&nbsp;")
                {
                    throw new Exception("Invalid Delete Operation. Please Check. . .");
                }
                iFileID = Convert.ToInt32(grdFileProcessHistory.Rows[Convert.ToInt32(e.CommandArgument)].Cells[7].Text);
                dForDate = Convert.ToDateTime(grdFileProcessHistory.Rows[Convert.ToInt32(e.CommandArgument)].Cells[2].Text);

                BLImport im = new BLImport(Convert.ToString(Session["ConStr"]));

                im.DeleteProcessHistory((enmFileID)iFileID, dForDate);

                this.PrepareImportHistory();
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = ex.Message;
            }
        }
    }
}
