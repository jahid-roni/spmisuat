using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace SBM_WebUI.mp
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PageLoadApplicableCustomerType();
        }

        private void PageLoadApplicableCustomerType()
        {
            DataTable oDataTable = new DataTable("dtData");

            oDataTable.Columns.Add(new DataColumn("Val1", typeof(string)));



            DataRow row = oDataTable.NewRow();
            row["Val1"] = "01: Individual";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "02: Joint";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "03: Institute";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "04: Joint";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "05: Institute";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "06: Joint";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "07: Institute";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "04: Joint";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "05: Institute";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "06: Joint";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "07: Institute";
            oDataTable.Rows.Add(row);

            gvCustomerType.DataSource = oDataTable;
            gvCustomerType.DataBind();
        }
    }
}
