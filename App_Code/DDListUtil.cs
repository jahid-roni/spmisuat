using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SBM_BLC1.Entity.Configuration;
using SBM_BLC1.Common;
using System.IO;
using SBM_BLC1.DAL.Common;
using SBM_BLC1.Entity.Common;

public static class DDListUtil
{
    /// This Method is used for assign the value by both Display And Value Member
    public static void Add(DropDownList DropDownList, string sDisplayMember, string sValueMember)
    {
        DropDownList.Items.Add(new ListItem(sDisplayMember, sValueMember));
    }

    /// This Method is used for assign the value by both Display And Value Member
    public static void Add(CheckBoxList chkBoxList, string sDisplayMember, string sValueMember)
    {
        chkBoxList.Items.Add(new ListItem(sDisplayMember, sValueMember));
    }

    /// This method is used for deleting all the item from particular DropDownList
    public static void RemoveAll(DropDownList DropDownList)
    {
        for (int i = 0; i < DropDownList.Items.Count; i++)
        {
            DropDownList.Items.RemoveAt(i);
        }
    }

    /// This Method is used for assign the value by both Display And Value Member
    public static void Assign(DropDownList DropDownList, string sDisplayMember, string sValueMember)
    {
        for (int i = 0; i < DropDownList.Items.Count; i++)
        {
            if (DropDownList.Items[i].Text == sDisplayMember && DropDownList.Items[i].Value == sValueMember)
            {
                DropDownList.SelectedIndex = i;
                return;
            }
        }
    }

    /// This Method is used for assign the value by Value Member
    public static void Assign(DropDownList DropDownList, string sValueMember)
    {
        if (DropDownList != null && sValueMember!=null)
        {
            if (DropDownList.Items.Count > 0)
            {
                DropDownList.SelectedIndex = 0;
                for (int i = 0; i < DropDownList.Items.Count; i++)
                {
                    if (DropDownList.Items[i].Value.Trim() == Convert.ToString(sValueMember).Trim())
                    {
                        DropDownList.Items[i].Selected = true;
                        DropDownList.SelectedIndex = i;
                        return;
                    }
                }
            }
        }
    }

    public static void Assign(DropDownList ddl, int iValue)
    {
        for (int i = 0; i < ddl.Items.Count; i++)
        {
            if (ddl.Items[i].Value.Trim() == iValue.ToString())
            {
                ddl.Items[i].Selected = true;
                ddl.SelectedIndex = i;
                return;
            }
        }
    }

    public static void Assign(DropDownList ddl, bool bValue)
    {
        for (int i = 0; i < ddl.Items.Count; i++)
        {
            if (Convert.ToBoolean(ddl.Items[i].Value.Trim()) == bValue)
            {
                ddl.Items[i].Selected = true;
                ddl.SelectedIndex = i;
                return;
            }
        }
    }

    /// This Method is used for assign text based on display text
    public static void Assign(DropDownList DropDownList, string sText, bool bIsDisplay)
    {
        for (int i = 0; i < DropDownList.Items.Count; i++)
        {
            if (DropDownList.Items[i].Text == sText)
            {
                DropDownList.Items[i].Selected = true;
                DropDownList.SelectedIndex = i;
                return;
            }
        }
    }


    public static void Assign(DropDownList oDDList, object objList, SBM_BLC1.Common.Constants.DDL_TYPE dDL_TYPE)
    {
        oDDList.Items.Clear();
        if (Constants.DDL_TYPE.BRANCHLIST.Equals(dDL_TYPE))
        {
            BranchList oBranchList = (BranchList)objList;
            if (oBranchList.ListOfBranch.Count > 0)
            {
                for (int i = 0; i < oBranchList.ListOfBranch.Count; i++)
                {
                    Add(oDDList, oBranchList.ListOfBranch[i].BbCode.Trim(), oBranchList.ListOfBranch[i].BranchID.Trim());
                }
            }
        }
    }


    public static void Assign(DropDownList oDDList, DataTable dtGetAll)
    {
        for (int i = 0; i < dtGetAll.Rows.Count; i++)
        {
            Add(oDDList, dtGetAll.Rows[i]["Text"].ToString().Trim(), dtGetAll.Rows[i]["Value"].ToString().Trim());
        }
    }

    public static void Assign(DropDownList oDDList, DataTable dtGetAll , bool isEmptyItem)
    {
        if (isEmptyItem)
        {
            Add(oDDList, "", "");
        }
        for (int i = 0; i < dtGetAll.Rows.Count; i++)
        {
            Add(oDDList, dtGetAll.Rows[i]["Text"].ToString().Trim(), dtGetAll.Rows[i]["Value"].ToString().Trim());
        }
    }

    public static void LoadDDLFromDB(DropDownList oDDList, string sValue, string sText, string sTable, bool isFormat)
    {
        UtilityDAL oUtilityDAL = new UtilityDAL();
        Result oResult = new Result();
        oResult = oUtilityDAL.GetDDLDataList(sValue, sText, sTable, isFormat);
        if (oResult.Status)
        {
            DataTable dtGetAll = (DataTable)oResult.Return;
            oDDList.Items.Clear();
            Add(oDDList, "", "");
            for (int i = 0; i < dtGetAll.Rows.Count; i++)
            {
                Add(oDDList, dtGetAll.Rows[i]["Text"].ToString().Trim(), dtGetAll.Rows[i]["Value"].ToString().Trim());
            } 
        }
    }

    public static void LoadCheckBoxListFromDB(CheckBoxList oCheckBoxList, string sValue, string sText, string sTable)
    {
        UtilityDAL oUtilityDAL = new UtilityDAL();
        Result oResult = new Result();
        oResult = oUtilityDAL.GetDDLDataList(sValue, sText, sTable , false);
        if (oResult.Status)
        {
            DataTable dtGetAll = (DataTable)oResult.Return;
            oCheckBoxList.Items.Clear();
            for (int i = 0; i < dtGetAll.Rows.Count; i++)
            {
                Add(oCheckBoxList, dtGetAll.Rows[i]["Text"].ToString().Trim(), dtGetAll.Rows[i]["Value"].ToString().Trim());
            }
        }
    }

    public static void LoadCheckBoxListByCurrencyID(CheckBoxList oCheckBoxList, string sCurrencyID)
    {
        UtilityDAL oUtilityDAL = new UtilityDAL();
        Result oResult = new Result();
        oResult = oUtilityDAL.GetSPTypeListByCurrecyID(sCurrencyID);
        if (oResult.Status)
        {
            DataTable dtGetAll = (DataTable)oResult.Return;
            oCheckBoxList.Items.Clear();
            for (int i = 0; i < dtGetAll.Rows.Count; i++)
            {
                Add(oCheckBoxList, dtGetAll.Rows[i]["Text"].ToString().Trim(), dtGetAll.Rows[i]["Value"].ToString().Trim());
            }
        }
    }

    public static void LoadScriptFormat(DropDownList oDDList, string spType)
    {
        DataSet ds = new DataSet();
        string filePath=HttpContext.Current.Request.MapPath("~/ConfigData/ConfigMapping.xml");

        ds.ReadXml(filePath);

        DataTable dt = new DataTable();

        if (ds.Tables.Count > 0)
        {
            dt = ds.Tables["ScriptFormatMapping"];

            DataView dv = new DataView();
            dv.Table = dt;
            dv.RowFilter = "SPType='" + spType.Trim() + "'";

            Add(oDDList, "", "");

            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    Add(oDDList, dv[i]["FormatID"].ToString().Trim() + " : " + dv[i]["FormatName"].ToString().Trim(), dv[i]["FormatID"].ToString().Trim());
                }
            }
        }
        else
        {
            throw new Exception("Invalid confguration in format mapping. Please check");
        }

    }

    public static void LoadDDLFromXML(DropDownList oDDList, string tableType, string rowType, string value, bool isFormat)
    {
        DataSet ds = new DataSet();
        string filePath = HttpContext.Current.Request.MapPath("~/ConfigData/ConfigMapping.xml");

        ds.ReadXml(filePath);

        DataTable dt = new DataTable();

        if (ds.Tables.Count > 0)
        {
            dt = ds.Tables[tableType];

            DataView dv = new DataView();
            dv.Table = dt;
            
            dv.RowFilter = rowType + "='" + value.Trim() + "'";
            
            Add(oDDList, "", "");

            if (dv.Count > 0)
            {
                if (isFormat)
                {
                    for (int i = 0; i < dv.Count; i++)
                    {
                        Add(oDDList, dv[i]["FormatID"].ToString().Trim().PadLeft(2, '0') + " : " + dv[i]["FormatName"].ToString().Trim(), dv[i]["FormatID"].ToString().Trim());
                    }
                }
                else
                {
                    for (int i = 0; i < dv.Count; i++)
                    {
                        Add(oDDList, dv[i]["FormatName"].ToString().Trim(), dv[i]["FormatID"].ToString().Trim());
                    }
                }
            }
        }
        else
        {
            throw new Exception("Invalid confguration in format mapping. Please check");
        }
    }

    public static DataTable MapTableWithXML(DataTable dtTable, string tableType, string rowTypeName, string rowTypeValue, int colIndex)
    {
        DataSet ds = new DataSet();
        string filePath = HttpContext.Current.Request.MapPath("~/ConfigData/ConfigMapping.xml");

        ds.ReadXml(filePath);
        DataTable dtMappedData = null;
        
        if (ds.Tables.Count > 0)
        {
            if (dtTable != null)
            {
                dtMappedData = dtTable.Clone();


                foreach (DataColumn dc in dtMappedData.Columns)
                {
                    if (dc.DataType == typeof(Int16))
                    {
                        dc.DataType = typeof(string);
                    }
                }

                foreach (DataRow dr in dtTable.Rows)
                {
                    dtMappedData.ImportRow(dr);
                }

                DataTable dt = ds.Tables[tableType];

                DataView dv = new DataView();
                dv.Table = dt;
                dv.RowFilter = rowTypeName + "='" + rowTypeValue + "'";

                for (int rowCount = 0; rowCount < dtMappedData.Rows.Count; rowCount++)
                {
                    if (dv.Count > 0)
                    {
                        for (int filterCount = 0; filterCount < dv.Count; filterCount++)
                        {
                            if (dv[filterCount]["FormatID"].ToString().Equals(dtMappedData.Rows[rowCount][colIndex].ToString()))
                            {
                                dtMappedData.Rows[rowCount][colIndex] = dv[filterCount]["FormatName"].ToString();

                                break;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            throw new Exception("Invalid confguration in format mapping. Please check");
        }

        return dtMappedData;
    }

    public static void LoadActiveUser(DropDownList oDDList, string sValue, string sText, string sTable, int iMakerGroupID, bool isFormat, string sDivisionID)
    {
        UtilityDAL oUtilityDAL = new UtilityDAL();
        Result oResult = new Result();
        oResult = oUtilityDAL.GetActiveUserListByGroupID(sValue, sText, sTable, iMakerGroupID, isFormat, sDivisionID);
        if (oResult.Status)
        {
            DataTable dtGetAll = (DataTable)oResult.Return;
            oDDList.Items.Clear();
            Add(oDDList, "", "");
            for (int i = 0; i < dtGetAll.Rows.Count; i++)
            {
                Add(oDDList, dtGetAll.Rows[i]["Text"].ToString().Trim(), dtGetAll.Rows[i]["Value"].ToString().Trim());
            }
        }
    } 
}
