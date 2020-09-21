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
    public partial class ViewReport : System.Web.UI.Page
    {
        ReportDocument oRepDoc = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            bool isException = false;
            int windno = 0;
            try
            {
                windno = int.Parse(Request.QueryString["windno"]);
            }
            catch { }
            try
            {
                if (windno == 1)
                {
                    if (Session[Constants.SES_RPT_DATA] != null)
                    {
                        oRepDoc = (ReportDocument)Session[Constants.SES_RPT_DATA];
                    }
                }
                else if (windno == 2)
                {
                    if (Session[Constants.SES_RPT_DATA_2] != null)
                    {
                        oRepDoc = (ReportDocument)Session[Constants.SES_RPT_DATA_2];
                    }
                }
                else if (windno == 3)
                {
                    if (Session[Constants.SES_RPT_DATA_3] != null)
                    {
                        oRepDoc = (ReportDocument)Session[Constants.SES_RPT_DATA_3];
                    }
                }

                else
                {
                    if (Session[Constants.SES_RPT_DATA] != null)
                    {
                        oRepDoc = (ReportDocument)Session[Constants.SES_RPT_DATA];
                    }
                }
            }
            catch (System.Threading.ThreadAbortException lException)
            {
                // do nothing                
            }
            catch (Exception Exp)
            {
                isException = true;
                ShowMessage();
            }

            #region Load Repoert
            MemoryStream oStream = null;

            if (!isException)
            {
                try
                {

                    if (oRepDoc != null)
                    {

                        CrystalParameterCheck(oRepDoc);
                    }
                    else
                    {

                        oRepDoc = new ReportDocument();
                        oRepDoc.Load(Request.MapPath("Reports/ErroOnReportt.rpt"));

                        // single parameter...
                        ParameterFields parameterFields = oRepDoc.ParameterFields;

                        CrystalParameterPassing(parameterFields, "sMsg", "No data available for preview.");
                    }

                    if (Convert.ToString(Session["ExportType"]) == "WRD")
                    {
                        //oRepDoc.ExportToHttpResponse(ExportFormatType.PortableDocFormat , Response, true, "PDFReport");
                        //Response.End();
                        oStream = (MemoryStream)oRepDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.RichText);
                        Response.Clear();
                        Response.Buffer = true;
                        Response.ContentType = "application/msword";
                        Response.BinaryWrite(oStream.ToArray());
                        Response.End();

                    }
                    else if (Convert.ToString(Session["ExportType"]) == "XLS")
                    {
                        oStream = (MemoryStream)oRepDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);

                        Response.Clear();
                        Response.Buffer = true;
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.BinaryWrite(oStream.ToArray());
                        Response.End();

                    }
                    else if (Convert.ToString(Session["ExportType"]) == "XLR")
                    {
                        oStream = (MemoryStream)oRepDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);

                        Response.Clear();
                        Response.Buffer = true;
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.BinaryWrite(oStream.ToArray());
                        Response.End();
                    }
                    else
                    {
                        oStream = (MemoryStream)oRepDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                        Response.Clear();
                        Response.Buffer = true;
                        Response.ContentType = "application/pdf";
                        Response.BinaryWrite(oStream.ToArray());
                        Response.End();
                    }
                    //oStream = (MemoryStream)oRepDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                    //Response.Clear();
                    //Response.Buffer = true;
                    //Response.ContentType = "application/pdf";
                    //Response.BinaryWrite(oStream.ToArray());
                    //Response.End();

            #endregion End of load report
                }
                catch (System.Threading.ThreadAbortException lException)
                {
                    // do nothing                    
                }
                catch (Exception Exp)
                {
                }
                finally
                {
                    if (oStream != null)
                    {
                        oStream.Flush();
                        oStream.Close();
                        oStream=null;
                        oRepDoc.Close();
                        oRepDoc=null;
                        Page.Dispose();
                    }
                }
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

        protected void CrystalParameterPassing(ParameterFields parameterFields, string strParamFieldName, string strParamValue)
        {
            ParameterValues currentParameterValues = new ParameterValues();
            ParameterDiscreteValue parameterDiscreteValue = new ParameterDiscreteValue();
            parameterDiscreteValue.Value = strParamValue == null ? "" : strParamValue.Trim();
            currentParameterValues.Add(parameterDiscreteValue);

            ParameterField parameterField = parameterFields[strParamFieldName.Trim()];
            parameterField.CurrentValues = currentParameterValues;
        }
        private void CrystalParameterCheck(ReportDocument oRepDoc)
        {
            ParameterFields oParameterFields = oRepDoc.ParameterFields;
            if (oParameterFields.Count > 0)
            {
                for (int i = 0; i < oParameterFields.Count; i++)
                {
                    try
                    {

                        if (oParameterFields[i].CurrentValues.Count == 0)
                        {
                            if (!oParameterFields[i].Name.Contains("-"))
                            {
                                ParameterValues currentParameterValues = new ParameterValues();
                                ParameterDiscreteValue parameterDiscreteValue = new ParameterDiscreteValue();
                                ParameterField parameterField = oParameterFields[oParameterFields[i].Name];

                                if (parameterField.ParameterValueType.ToString().Equals("NumberParameter"))
                                {
                                    parameterDiscreteValue.Value = 0;
                                }
                                else
                                {
                                    parameterDiscreteValue.Value = "";
                                }
                                currentParameterValues.Add(parameterDiscreteValue);
                                parameterField.CurrentValues = currentParameterValues;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }

                }
            }
        }
        private void ShowMessage()
        {
            ReportDocument oRepDoc = new ReportDocument();
            oRepDoc.Load(Request.MapPath("Reports/ErroOnReportt.rpt"));

            // single parameter...
            ParameterFields parameterFields = oRepDoc.ParameterFields;

            CrystalParameterPassing(parameterFields, "sMsg", "");

            CrystalReportViewer1.ReportSource = oRepDoc;

            //MemoryStream oStream = (MemoryStream)oRepDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            //Response.Clear();
            //Response.Buffer = true;
            //Response.ContentType = "application/pdf";
            //Response.BinaryWrite(oStream.ToArray());
            //Response.End();

            //oStream.Flush();
            //oStream.Close();
            //oRepDoc.Dispose();
        }


    }
}
