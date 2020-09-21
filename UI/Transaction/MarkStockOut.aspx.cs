using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace SBM_WebUI.mp
{
    public partial class MarkStockOut : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            PageLoadApplicableCustomerType();

        }

        private void PageLoadApplicableCustomerType()
        {
            DataTable oDataTable = new DataTable("dtData");

            oDataTable.Columns.Add(new DataColumn("Val1", typeof(string)));
            oDataTable.Columns.Add(new DataColumn("Val2", typeof(string)));
            oDataTable.Columns.Add(new DataColumn("Val3", typeof(string)));
            oDataTable.Columns.Add(new DataColumn("Val4", typeof(string)));
            oDataTable.Columns.Add(new DataColumn("Val5", typeof(string)));
            oDataTable.Columns.Add(new DataColumn("Val6", typeof(string)));
            oDataTable.Columns.Add(new DataColumn("Val7", typeof(string)));

            DataRow row = oDataTable.NewRow();
            row["Val1"] = "3MS ";
            row["Val2"] = "Months Inter..";
            row["Val3"] = "Inter..";
            row["Val4"] = "3Months ..";
            row["Val5"] = "3MS : 3MInter..";
            row["Val6"] = "onths Inter..";
            row["Val7"] = "onths Inter..";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "3MS ";
            row["Val2"] = "Months Inter..";
            row["Val3"] = "Inter..";
            row["Val4"] = "3Months ..";
            row["Val5"] = "3MS : 3MInter..";
            row["Val6"] = "onths Inter..";
            row["Val7"] = "onths Inter..";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "3MS ";
            row["Val2"] = "Months Inter..";
            row["Val3"] = "Inter..";
            row["Val4"] = "3Months ..";
            row["Val5"] = "3MS : 3MInter..";
            row["Val6"] = "onths Inter..";
            row["Val7"] = "onths Inter..";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "3MS ";
            row["Val2"] = "Months Inter..";
            row["Val3"] = "Inter..";
            row["Val4"] = "3Months ..";
            row["Val5"] = "3MS : 3MInter..";
            row["Val6"] = "onths Inter..";
            row["Val7"] = "onths Inter..";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "3MS ";
            row["Val2"] = "Months Inter..";
            row["Val3"] = "Inter..";
            row["Val4"] = "3Months ..";
            row["Val5"] = "3MS : 3MInter..";
            row["Val6"] = "onths Inter..";
            row["Val7"] = "onths Inter..";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "3MS ";
            row["Val2"] = "Months Inter..";
            row["Val3"] = "Inter..";
            row["Val4"] = "3Months ..";
            row["Val5"] = "3MS : 3MInter..";
            row["Val6"] = "onths Inter..";
            row["Val7"] = "onths Inter..";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "3MS ";
            row["Val2"] = "Months Inter..";
            row["Val3"] = "Inter..";
            row["Val4"] = "3Months ..";
            row["Val5"] = "3MS : 3MInter..";
            row["Val6"] = "onths Inter..";
            row["Val7"] = "onths Inter..";
            oDataTable.Rows.Add(row);

            gvSPType.DataSource = oDataTable;
            gvSPType.DataBind();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }
    }
}

