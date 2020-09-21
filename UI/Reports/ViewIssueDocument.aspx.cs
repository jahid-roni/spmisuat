using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using System.Data;
using System.IO;
using CrystalDecisions.Shared;
using System.Xml;
using System.Text;
using SBM_BLC1.Common;


namespace LAMS.Web.UI
{
    public partial class ViewIssueDocument : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                byte[] oRepDoc = null;

                    if (Session[Constants.SES_PDF_DATA] != null)
                    {
                        oRepDoc = (byte[])Session[Constants.SES_PDF_DATA];
                        Response.Buffer = true;
                        Response.Charset = "";
                        //if (Request.QueryString["download"] == "1")
                        //{
                        //    Response.AppendHeader("Content-Disposition", "attachment; filename=IssueDocument_"+DateTime.Today.ToString("yyyyMMdd"));
                        //}
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.ContentType = "application/pdf";
                        Response.BinaryWrite(oRepDoc);
                        Response.Flush();
                        Response.End();
                    }
                }

                
            catch (Exception Exp)
            {

            }

        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                //CrystalReportViewer1.Dispose();
                //CrystalReportViewer1 = null;
                //oRepDoc.Close();
                //oRepDoc.Dispose();
                GC.Collect();
            }
            catch (Exception)
            {

            }


        }

        


    }
}
