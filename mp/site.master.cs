using SBM_BLC1.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SBM_WebUI.Scripts
{
    public partial class site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Constants.SES_USER_CONFIG] != null)
            {
                if (!Page.IsPostBack)
                {
                    Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                    //if (oConfig.BankCodeID == Constants.BANK_TYPE_AMEX)
                    //{
                    //    lblBankType.Text = "HSBX, " + oConfig.DivisionID;
                    //    //lblMex.Visible = false;
                    //}
                    //else if (oConfig.BankCodeID == Constants.BANK_TYPE_SBM)
                    //{
                    //    lblBankType.Text = "HSBC, " + oConfig.DivisionID;
                    //    //lblSBM.Visible = false;
                    //}
                    //else if (oConfig.BankCodeID == Constants.BANK_TYPE_SCG)
                    //{
                    //    lblBankType.Text = "HSBY, " + oConfig.DivisionID;
                    //    //lblScg.Visible = false;
                    //}

                    // This block is used for setting Login INFO
                    //Label label = (Label)Master.FindControl("lblUserName");
                    string sUserName = "";
                    if (oConfig.LoginUser.FirstName.Length > 0)
                    {
                        sUserName = oConfig.LoginUser.FirstName.Trim();
                    }
                    if (oConfig.LoginUser.LastName.Length > 0)
                    {
                        if (sUserName.Length > 0)
                        {
                            sUserName += " ";
                        }
                        sUserName += oConfig.LoginUser.LastName.Trim();
                    }
                    lblUserName.Text = sUserName;
                    lblDivision.Text = oConfig.DivisionID;
                    lblBankType.Text = "HSBC, " + oConfig.DivisionID;
                    // This block is used for setting Login INFO
                                        
                    DivisionButton_SetColor(oConfig.DivisionID);

                    Session[Constants.SES_USER_CONFIG] = oConfig;
                    if (oConfig.DivisionID == "DHK")
                    {
                        lblDivision.Text = "Dhaka";
                    }
                    else if (oConfig.DivisionID == "CTG")
                    {
                        lblDivision.Text = "Chittagong";
                    }
                    else if (oConfig.DivisionID == "SYL")
                    {
                        lblDivision.Text = "Sylhet";
                    }
                }
            }
            else
            {
                Response.Redirect(Constants.PAGE_LOGIN, false);
            }
        }

        public string UserName
        {
            get { return lblUserName.Text; }
            set { lblUserName.Text = value; }
        }

        //protected void ASPxMenu1_ItemDataBound(object source, MenuItemEventArgs e)
        //{
        //    SiteMapNode node = e.Item.DataItem as SiteMapNode;
        //    if (node != null)
        //        e.Item.Text = "<b>" + node["result"] + "</b> " + e.Item.Text;
        //}

        protected void lblMex_Click(object sender, EventArgs e)
        {
            //lblMex.Visible = false;
            //lblScg.Visible = true;
            //lblSBM.Visible = true;

            if (Session[Constants.SES_USER_CONFIG] != null)
            {
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                oConfig.BankCodeID = Constants.BANK_TYPE_AMEX;
                lblBankType.Text = "AMEX";
                Session[Constants.SES_USER_CONFIG] = oConfig;
                Response.Redirect(Constants.PAGE_HOME, false);
            }
        }
        protected void lblScg_Click(object sender, EventArgs e)
        {
            //lblMex.Visible = true;
            //lblScg.Visible = false;
            //lblSBM.Visible = true;

            if (Session[Constants.SES_USER_CONFIG] != null)
            {
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                oConfig.BankCodeID = Constants.BANK_TYPE_SCG;
                lblBankType.Text = "SCG";
                Session[Constants.SES_USER_CONFIG] = oConfig;
                Response.Redirect(Constants.PAGE_HOME, false);
            }
        }
        protected void lblSBM_Click(object sender, EventArgs e)
        {
            //lblMex.Visible = true;
            //lblScg.Visible = true;
            //lblSBM.Visible = false;

            if (Session[Constants.SES_USER_CONFIG] != null)
            {
                Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
                oConfig.BankCodeID = Constants.BANK_TYPE_SBM;
                lblBankType.Text = "HSB";
                Session[Constants.SES_USER_CONFIG] = oConfig;
                Response.Redirect(Constants.PAGE_HOME, false);
            }
        }

        protected void lblLogout_Click(object sender, EventArgs e)
        {
            try
            {
                Session.Clear();
                Session.Abandon();
                Response.Redirect(Constants.PAGE_LOGIN, false);
            }
            catch (Exception exp)
            {

            }
        }

        protected void btnDHK_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            oConfig.DivisionID="DHK";
            oConfig.BranchID = "01";
           Session[Constants.SES_USER_CONFIG] = oConfig;

           Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void btnCTG_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            oConfig.DivisionID = "CTG";
            oConfig.BranchID = "04";

            Session[Constants.SES_USER_CONFIG] = oConfig;

            Response.Redirect(Request.Url.AbsoluteUri);

        }

        protected void btnSYL_Click(object sender, EventArgs e)
        {
            Config oConfig = (Config)Session[Constants.SES_USER_CONFIG];
            oConfig.DivisionID = "SYL";
            oConfig.BranchID = "06";
            Session[Constants.SES_USER_CONFIG] = oConfig;

            Response.Redirect(Request.Url.AbsoluteUri);
        }
        private void DivisionButton_SetColor(string vDiv)
        {
            Color c = Color.Transparent;
            if (btnDHK.BackColor != Color.Transparent)
            {
                c = btnDHK.BackColor;
            }
            else
            {
                c = btnSYL.BackColor;
            }

            if (vDiv == "DHK")
            {
                btnDHK.BackColor = Color.Transparent;
                btnCTG.BackColor = c;
                btnSYL.BackColor = c;
            }
            else if (vDiv == "CTG")
            {
                btnDHK.BackColor = c;
                btnCTG.BackColor = Color.Transparent;
                btnSYL.BackColor = c;

            }
            else
            {
                btnDHK.BackColor = c;
                btnCTG.BackColor = c;
                btnSYL.BackColor = Color.Transparent;
            }
            
        }
    }
}