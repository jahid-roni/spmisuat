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
using System.Collections.Generic;
using SBM_BLC1.Entity.SecurityAdmin;


public class CheckPermission
{
    public CheckPermission()
    {
        //
        // TODO: Add constructor logic here
        //
    }


    public static void FindAllButton(Control ctrl, Hashtable htblControlsList)
    {
        if (ctrl != null)
        {
            foreach (Control c in ctrl.Controls)
            {
                if (c is Button)
                {
                    Button btn = ((Button)c);
                    if (!htblControlsList.Contains(btn.ID.ToUpper()))
                    {
                        htblControlsList.Add(btn.ID.ToUpper(), btn.UniqueID);
                    }
                }
                FindAllButton(c, htblControlsList);
            }
        }
    }

    public bool CheckPagePermission(Control oPage, Config oConfig, int iPageID)
    {
        bool isView = false;
        Hashtable htblControlsList = new Hashtable();
        Control ctrl = oPage;
        FindAllButton(ctrl, htblControlsList);
        if (oConfig != null)
        {
            List<Screen> ScreenObjectList = oConfig.LoginUser.Group.ScreenList;
            Screen oScreen = ScreenObjectList.Where(d => d.ScreenID == iPageID).SingleOrDefault();
            if (oScreen != null)
            {
                string scr = "";
                for (int i = 0; i < oScreen.ScreenObjectList.Count; i++)
                {
                    scr = oScreen.ScreenObjectList[i].ObjectName.ToString();
                    string sControlID = (string)htblControlsList[scr.ToUpper()];
                    if (sControlID != null)
                    {
                        Button oBtn = (Button)oPage.FindControl(sControlID);
                        if (oBtn != null)
                        {
                            if (oBtn.Visible == true)
                            {
                                oBtn.Visible = oScreen.ScreenObjectList[i].IsActive;
                            }
                        }
                    }
                }
                isView = oScreen.IsView;
            }
            else
            {
                //Response.Redirect(Constants.PAGE_LOGIN, false);
                isView = false;
            }
        }
        else
        {
            //Response.Redirect(Constants.PAGE_LOGIN, false);
            isView = false;
        }
        return isView;
    }

}

