using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace SBM_WebUI.mp
{
    public partial class CustomerTrackNoReg : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PageLoadGrid();
            ChkChangeColor(chkIncludeFiscalYear);
            
        }
        private void ChkChangeColor(CheckBox cblBox)
        {
            cblBox.Attributes["onclick"] = string.Format("ChkChangeColor( this ) ");
        }

        private void PageLoadGrid()
        {
            DataTable oDataTable = new DataTable("dtData");

            oDataTable.Columns.Add(new DataColumn("Val1", typeof(string)));



            DataRow row = oDataTable.NewRow();
            row["Val1"] = "Value 1";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "Value 2";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "Value 3";
            oDataTable.Rows.Add(row);

            row = oDataTable.NewRow();
            row["Val1"] = "Value 4";
            oDataTable.Rows.Add(row);

            gvRegDetails.DataSource = oDataTable;
            gvRegDetails.DataBind();

            DataTable dt = new DataTable("Init");
            dt.Columns.Add(new DataColumn("Val1", typeof(string)));

            dt.Rows.Add(DBNull.Value);
            gvRegSearchDetails.DataSource = dt;
            gvRegSearchDetails.DataBind();
            gvRegSearchDetails.Rows[0].Cells[0].Enabled = false;


             gvTrackSearch.DataSource = dt;
            gvTrackSearch.DataBind();
            gvTrackSearch.Rows[0].Cells[0].Enabled = false;
        }
    }
}
