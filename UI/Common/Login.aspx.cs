using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SBM_BLC1.Common;
using SBM_BLC1.Entity.Common;
using SBM_BLC1.Entity.SecurityAdmin;

using System.Data;
using SBM_BLC1.SecurityAdmin;

namespace SBM_WebUI.UI.Common
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitializeData();
            }
            UserDAL oUserDAL = new UserDAL();
            Result oResult = oUserDAL.GetAllUser();
            if (oResult.Status)
            {
                string sUserList = "";
                DataTable dtUserList = (DataTable)oResult.Return;
                for (int i = 0; i < dtUserList.Rows.Count; i++)
                {
                    if (sUserList.Length > 0)
                    {
                        sUserList += "~";
                    }
                    sUserList += dtUserList.Rows[i]["UserName"];
                }
                hdUserList.Value = sUserList;
            }
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            UserDAL oUserDAL = new UserDAL();
            if (!txtUserName.Text.Contains("Admin"))
            {
                if (!Util.IsAuthenticated(txtUserName.Text.Trim(), txtPassword.Text.Trim(), Properties.Settings.Default.ADName))
                {
                    oUserDAL.SetUser_LoginTry(this.txtUserName.Text, false);
                    lblMessage.Text = "HSBC AD Error. Please check Login ID name and Password ..! ";
                    return;
                }
            }
            Result oResult = oUserDAL.LogInUser(txtUserName.Text.Trim(), txtPassword.Text.Trim(), "DHK");
            if (oResult.Status)
            {
                User oUser = (User)oResult.Return;
                oUserDAL.SetUser_LoginTry(this.txtUserName.Text, true);

                Config oConfig = new Config();
                oConfig.UserID = oUser.UserID.ToString();
                //oConfig.DivisionID = "SYL";//Convert.ToString(ddlDivision.SelectedValue);//oUser.DivisionID;
                oConfig.DivisionID = "DHK";
                //if (oConfig.DivisionID=="DHK")
                //{
                //   oConfig.BranchID = "01";
                //}
                //else if (oConfig.DivisionID=="SYL")
                //{
                //    oConfig.BranchID = "08";
                //}
                //else if (oConfig.DivisionID == "DHK")
                //{
                //    oConfig.BranchID = "08";
                //}
                oConfig.BranchID = oUser.BranchID.Trim();
                oConfig.GroupID = oUser.GroupID.Trim();
                oConfig.UserName = oUser.UserName.ToString();
                oConfig.BankCodeID = Constants.BANK_TYPE_SBM;
                oConfig.LoginUser = oUser;
                Session.Add(Constants.SES_USER_CONFIG, oConfig);
                Response.Redirect(Constants.PAGE_HOME, false);
            }
            else
            {
                oUserDAL.SetUser_LoginTry(this.txtUserName.Text, false);
                lblMessage.Text = "Sorry. please check your user name and password..! ";
            }
        }
        private void InitializeData()
        {
            //DDListUtil.LoadDDLFromDB(ddlDivision, "DivisionID", "DivisionName", "SPMS_Division", true);
            //ddlDivision.SelectedIndex = 1;
            UserDAL usDal = new UserDAL();
            Result oR = usDal.GetOperationType();
            DataTable dtOt = null;
            if (oR.Status)
            {
                dtOt = (DataTable)oR.Return;
                Session[Constants.SES_GROUP_OPERATION_DATA] = dtOt;
            }
        }
    }
}
