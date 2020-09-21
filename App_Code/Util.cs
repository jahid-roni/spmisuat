using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections;
using System.Globalization;
using SBM_BLC1.Common;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.DirectoryServices.AccountManagement;

public class Util
{
    public Util()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public static bool IsAuthenticated(string AD_USER_NAME, string AD_PWD, string AD_ADDRESS)
    {
        bool authenticated = false;

        try
        {
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, AD_ADDRESS))
            {
                if (pc == null)
                {
                    authenticated = false;
                }
                else
                {
                    authenticated = pc.ValidateCredentials(AD_USER_NAME, AD_PWD);
                }
            }
        }

        catch (Exception ex)
        {
            throw ex;
        }
        return authenticated;
    }
    public static bool CheckNumber(string sData)
    {
        if (sData.Length == 0)
        {
            return false;
        }
        double Num;
        bool isNum = double.TryParse(sData, out Num);
        return isNum;
    }
    public static decimal GetDecimalNumber(string sData)
    {
        if (sData.Length == 0)
        {
            return Convert.ToDecimal(0);
        }
        decimal Num;
        bool isNum = decimal.TryParse(sData, out Num);
        if (isNum)
        {
            //string s = sData.Replace(",", "");
            return Convert.ToDecimal(Num);
        }
        else
        {
            return Convert.ToDecimal(0);
        }
    }

    public static double GetDoubleNumber(string sData)
    {
        if (sData.Length == 0)
        {
            return Convert.ToDouble(0);
        }
        decimal Num;
        bool isNum = decimal.TryParse(sData, out Num);
        if (isNum)
        {
            //string s = sData.Replace(",", "");
            return Convert.ToDouble(Num);
        }
        else
        {
            return Convert.ToDouble(0);
        }
    }

    public static void InvalidateSession()
    {
        try
        {
            //Get the session key list needs to be excluded from session nullify
            ArrayList oExclusionList = new ArrayList();

            //Exclusion must, so add them in collection by force
            oExclusionList.Add(Constants.SES_USER_CONFIG);
            // IF REQUIRED ..ADD MORE...

            ArrayList oStandingSession = new ArrayList();

            //Get all the live keys and put them in a modifiable collection
            for (int i = 0; i < HttpContext.Current.Session.Keys.Count; i++)
            {
                oStandingSession.Add((System.String)HttpContext.Current.Session.Keys[i]);
            }

            //Remove all the sessions except these are in exclusion key list
            foreach (System.String sKey in oStandingSession)
            {
                if (!oExclusionList.Contains(sKey))
                {
                    HttpContext.Current.Session.Remove(sKey);
                }
            }
        }
        catch (Exception e)
        {

        }
    }

    public static int GetIntNumber(string sData)
    {
        if (sData.Length == 0)
        {
            return Convert.ToInt32(0);
        }
        double Num;
        bool isNum = double.TryParse(sData, out Num);
        if (isNum)
        {
            //string s = sData.Replace(",", "");
            return Convert.ToInt32(Num);
        }
        else
        {
            return Convert.ToInt32(0);
        }
    }


    public static void ClearData(RadioButtonList rblType)
    {
        foreach (ListItem lstItem in rblType.Items)
        {
            lstItem.Selected = false;
        }
    }

    public static void RBLChangeColor(RadioButtonList rdoBox)
    {
        for (int i = 0; i <= rdoBox.Items.Count - 1; i++)
        {
            rdoBox.Items[i].Attributes["onclick"] = string.Format("RbChangeColor( this ) ");
        }
    }

    public static void RBLChangeSetColor(RadioButtonList rdoBox)
    {
        foreach (ListItem lstItem in rdoBox.Items)
        {
            if (lstItem.Selected == true)
            {
                lstItem.Attributes["style"] = "color:#EE8927; forecolor:black; font-weight:bold";
            }
            else
            {
                lstItem.Attributes["style"] = "color:#000000; forecolor:black; font-weight:normal ";
            }
        }
    }

    public static void ChklChangeColor(CheckBoxList chklBox)
    {
        for (int i = 0; i <= chklBox.Items.Count - 1; i++)
        {
            chklBox.Items[i].Attributes["onclick"] = string.Format("ChkChangeColor( this ) ");
        }
    }

    public static void ChklChangeSetColor(CheckBoxList chklBox)
    {
        foreach (ListItem lstItem in chklBox.Items)
        {
            if (lstItem.Selected == true)
            {
                lstItem.Attributes["style"] = "color:#EE8927; forecolor:black; font-weight:bold";
            }
            else
            {
                lstItem.Attributes["style"] = "color:#000000; forecolor:black; font-weight:normal ";
            }
        }
    }


    public static string GetCheckListIDList(CheckBoxList chkLSpType)
    {
        string sCheckList = string.Empty;
        foreach (ListItem boxItem in chkLSpType.Items)
        {
            if (boxItem.Selected == true)
            {
                if (sCheckList.Length > 0)
                {
                    sCheckList += ",";
                }
                sCheckList += "'" + ((string)boxItem.Value) + "'";
            }
        }
        return sCheckList;
    }


    public static void ChkChangeSetColor(CheckBox cblBox)
    {
        cblBox.Attributes["onclick"] = string.Format("ChkChangeColor( this ) ");

        if (cblBox.Checked)
        {
            cblBox.ForeColor = System.Drawing.ColorTranslator.FromHtml("#EE8927");
            cblBox.ControlStyle.Font.Bold = true;
        }
        else
        {
            cblBox.ForeColor = System.Drawing.ColorTranslator.FromHtml("#000000");
            cblBox.ControlStyle.Font.Bold = false;
        }
    }
    public static void GridDateFormat(GridViewRowEventArgs e, GridView gvData, ArrayList alAddSeperatorIndex)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataTable dt = (DataTable)gvData.DataSource;
            if (dt != null)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (dt.Rows[0][i].GetType() == typeof(Decimal))
                    {
                        try
                        {
                            e.Row.Cells[i + 1].Text = Convert.ToDecimal(e.Row.Cells[i + 1].Text).ToString("N2");
                        }
                        catch (Exception Exp)
                        {

                        }
                    }
                    else if (dt.Rows[0][i].GetType() == typeof(DateTime))
                    {
                        try
                        {
                            if (!e.Row.Cells[i].Text.Equals("&nbsp;"))
                            {
                                e.Row.Cells[i].Text = Convert.ToDateTime(e.Row.Cells[i].Text).ToString("dd-MMM-yyyy");
                            }
                        }
                        catch (Exception Exp)
                        {

                        }
                        try
                        {
                            if (!e.Row.Cells[i + 1].Text.Equals("&nbsp;"))
                            {
                                e.Row.Cells[i + 1].Text = Convert.ToDateTime(e.Row.Cells[i + 1].Text).ToString("dd-MMM-yyyy");
                            }
                        }
                        catch (Exception Exp)
                        {

                        }
                    }
                }
            }

            if (alAddSeperatorIndex != null)
            {
                for (int i = 0; i < alAddSeperatorIndex.Count; i++)
                {
                    e.Row.Cells[System.Convert.ToInt32(alAddSeperatorIndex[i])].Text = SBM_BLC1.Common.String.AddSeperator(e.Row.Cells[System.Convert.ToInt32(alAddSeperatorIndex[i])].Text);
                }
            }
        }
    }
    public static void GridDateFormat(GridViewRowEventArgs e, GridView gvData, ArrayList alAddSeperatorIndex, ArrayList alCommaSeperatorIndex)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataTable dt = (DataTable)gvData.DataSource;
            if (dt != null)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (dt.Rows[0][i].GetType() == typeof(Decimal))
                    {
                        try
                        {
                            e.Row.Cells[i + 1].Text = Convert.ToDecimal(e.Row.Cells[i + 1].Text).ToString("N2");
                        }
                        catch (Exception Exp)
                        {

                        }
                    }
                    else if (dt.Rows[0][i].GetType() == typeof(DateTime))
                    {
                        try
                        {
                            if (!e.Row.Cells[i].Text.Equals("&nbsp;"))
                            {
                                e.Row.Cells[i].Text = Convert.ToDateTime(e.Row.Cells[i].Text).ToString("dd-MMM-yyyy");
                            }
                        }
                        catch (Exception Exp)
                        {

                        }
                        try
                        {
                            if (!e.Row.Cells[i + 1].Text.Equals("&nbsp;"))
                            {
                                e.Row.Cells[i + 1].Text = Convert.ToDateTime(e.Row.Cells[i + 1].Text).ToString("dd-MMM-yyyy");
                            }
                        }
                        catch (Exception Exp)
                        {

                        }
                    }
                }
            }

            if (alAddSeperatorIndex != null)
            {
                for (int i = 0; i < alAddSeperatorIndex.Count; i++)
                {
                    e.Row.Cells[System.Convert.ToInt32(alAddSeperatorIndex[i])].Text = SBM_BLC1.Common.String.AddSeperator(e.Row.Cells[System.Convert.ToInt32(alAddSeperatorIndex[i])].Text);
                }
            }
            if (alCommaSeperatorIndex != null)
            {
                for (int i = 0; i < alCommaSeperatorIndex.Count; i++)
                {
                    //e.Row.Cells[System.Convert.ToInt32(alCommaSeperatorIndex[i])].Text = Convert.ToString(e.Row.Cells[System.Convert.ToInt32(alCommaSeperatorIndex[i])].Text.ToString("N2"));
                    e.Row.Cells[System.Convert.ToInt32(alCommaSeperatorIndex[i])].Text = Convert.ToDecimal(e.Row.Cells[System.Convert.ToInt32(alCommaSeperatorIndex[i])].Text).ToString("N2");
                }
            }
        }
    }
    public static void SetCheckData(CheckBox chkYearly, bool bValue)
    {
        chkYearly.Checked = bValue;
    }
    public static bool GetRadioData(string sValue)
    {
        if (sValue == "0")
        {
            return false;
        }
        else if (sValue == "1")
        {
            return true;
        }
        else if (sValue == "TRUE")
        {
            return true;
        }
        else if (sValue == "FALSE")
        {
            return false;
        }
        else
            return false;
    }

    public static bool GetRadioData(RadioButtonList rblButton)
    {
        if (rblButton.SelectedItem != null)
        {
            if (rblButton.SelectedItem.Value == "0")
            {
                return false;
            }
            else if (rblButton.SelectedItem.Value == "1")
            {
                return true;
            }
            else if (rblButton.SelectedItem.Value == "TRUE")
            {
                return true;
            }
            else if (rblButton.SelectedItem.Value == "FALSE")
            {
                return false;
            }
            else
                return false;
        }

        return false;
    }

    public static string GetRadioStringData(RadioButtonList rblComSetIncomeTax)
    {
        if (rblComSetIncomeTax.SelectedItem != null)
        {
            return rblComSetIncomeTax.SelectedItem.Value;

        }
        else
        {
            return "0";
        }
    }
    public static void SetRadioData(RadioButtonList rblType, string sValue)
    {
        if (sValue != null)
        {
            foreach (ListItem lstItem in rblType.Items)
            {
                if (sValue == lstItem.Value)
                {
                    lstItem.Selected = true;
                }
                else
                {
                    lstItem.Selected = false;
                }
            }
        }
    }
    public static void SetRadioData(RadioButtonList rblType, int iValue)
    {
        if (iValue > -1)
            foreach (ListItem lstItem in rblType.Items)
            {
                if (Convert.ToString(iValue).Equals(lstItem.Value))
                {
                    lstItem.Selected = true;
                }
                else
                {
                    lstItem.Selected = false;
                }
            }
    }
    public static void SetRadioData(RadioButtonList rblType, bool bValue)
    {
        string sTmpValue = bValue == false ? "0" : "1";

        foreach (ListItem lstItem in rblType.Items)
        {
            if (sTmpValue.Equals(lstItem.Value))
            {
                lstItem.Selected = true;
                //break;
            }
            else
            {
                lstItem.Selected = false;
            }
        }
    }
    public static void ControlEnabled(object oControl, bool isEnabled)
    {
        if (oControl != null)
        {
            if (oControl.GetType().Name.Equals("TextBox"))
            {
                ((TextBox)oControl).Enabled = isEnabled;
            }
            else if (oControl.GetType().Name.Equals("Button"))
            {
                ((Button)oControl).Visible = isEnabled;
            }
            else if (oControl.GetType().Name.Equals("DropDownList"))
            {
                ((DropDownList)oControl).Enabled = isEnabled;
            }
            else if (oControl.GetType().Name.Equals("CheckBox"))
            {
                ((CheckBox)oControl).Enabled = isEnabled;
            }
            else if (oControl.GetType().Name.Equals("RadioButtonList"))
            {
                ((RadioButtonList)oControl).Enabled = isEnabled;
            }
            else if (oControl.GetType().Name.Equals("GridView"))
            {
                ((GridView)oControl).Enabled = isEnabled;
            }
        }
    }
    public static string formatDateValue(DateTime vDate)
    {
        string res = "";
        if (vDate <= Convert.ToDateTime("01/01/1900"))
        {
            res = "NULL";
        }
        else
        {
            res = "'" + vDate.ToString("dd/MMM/yyyy") + "'";
        }
        return res;
    }

    public static DateTime GetDateTimeByString(string dateTimeString)
    {
        try
        {
            if (!string.IsNullOrEmpty(dateTimeString))
            {
                CultureInfo ci = new CultureInfo("en-GB");
                DateTime dt = Convert.ToDateTime(dateTimeString, ci);

                return new DateTime(dt.Year, dt.Month, dt.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Kind);
            }
        }
        catch (Exception ex)
        {
            return new DateTime(1900, 01, 01);
        }
        return new DateTime(1900, 01, 01);
    }



    public static string LoadDDLFromXML(string tableType, string rowType, string value, string selectedValue)
    {
        DataSet ds = new DataSet();
        string filePath = HttpContext.Current.Request.MapPath("~/ConfigData/ConfigMapping.xml");
        ds.ReadXml(filePath);
        DataTable dt = new DataTable();
        string sResult = string.Empty;
        if (ds.Tables.Count > 0)
        {
            dt = ds.Tables[tableType];
            DataView dv = new DataView();
            dv.Table = dt;
            dv.RowFilter = rowType + "='" + value.Trim() + "'";
            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    if (dv[i]["FormatID"].Equals(selectedValue))
                    {
                        sResult = dv[i]["FormatName"].ToString().Trim();
                    }
                }
            }
        }
        else
        {
            throw new Exception("Invalid confguration in format mapping. Please check");
        }

        return sResult;
    }
    public static string OpenViewIssue(string urlRef)
    {
        StringBuilder sbUrl = new StringBuilder();
        sbUrl.Append("<script>window.open('");
        sbUrl.Append(urlRef);
        sbUrl.Append("');</script>");
        return sbUrl.ToString();
    }
    public static string OpenPDFView()
    {
        string sUrl = Constants.PAGE_PDF_VIEW + "?id=" + Guid.NewGuid().ToString();
        StringBuilder sbUrl = new StringBuilder();
        sbUrl.Append("<script>window.open('");
        sbUrl.Append(sUrl);
        sbUrl.Append("');</script>");
        return sbUrl.ToString();
    }
    public static string OpenReport()
    {
        string sUrl = Constants.PAGE_RPT_VIEW + "?id=" + Guid.NewGuid().ToString();
        StringBuilder sbUrl = new StringBuilder();
        sbUrl.Append("<script>window.open('");
        sbUrl.Append(sUrl);
        sbUrl.Append("');</script>");
        return sbUrl.ToString();
    }
    public static string OpenReport2(int windno)
    {
        string sUrl = Constants.PAGE_RPT_VIEW + "?id=" + Guid.NewGuid().ToString() + "&windno=" + windno;
        StringBuilder sbUrl = new StringBuilder();
        sbUrl.Append("<script>window.open('");
        sbUrl.Append(sUrl);
        sbUrl.Append("');</script>");
        return sbUrl.ToString();
    }
    public static string OpenPopup(string sType)
    {
        StringBuilder sbUrl = new StringBuilder();
        sbUrl.Append(" MsgPopupReturnTrue('" + sType + "'); ");
        return sbUrl.ToString();
    }

    public static string RedirectUrl(string sUrl)
    {
        StringBuilder sbUrl = new StringBuilder();
        sbUrl.Append("<script>window.location.replace('");
        sbUrl.Append(sUrl);
        sbUrl.Append("');</script>");
        return sbUrl.ToString();
    }
}



