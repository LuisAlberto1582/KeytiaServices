using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.EtiquetaNums
{
    public partial class AccesEtiquetacion : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        static string fechaInicio = "";
        static string fechaFinal = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            if (!Page.IsPostBack)
            {
                Response.Write("<script type='text/javascript'> sessionStorage.clear();</script>");
                fechaInicio = inicioFecha.Value;
                fechaFinal = finalFecha.Value;
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionCabeceras", "Cabeceras('JSONCabeceras');", true);
        }
        #region WeMethod Json
        [WebMethod]
        public static string JSONCabeceras()
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;

                DataTable dt = new DataTable();
                string esquema = DSODataContext.Schema;
                string connStr = DSODataContext.ConnectionString;

                string sp = "exec [ObtieneAccesoEtiquietacion] @Schema = '" + esquema + "',  @FechaIniRep = '{0}', @FechaFinRep = '{1}'";
                string query = string.Format(sp, fechaInicio, fechaFinal                    );

                dt = DSODataAccess.Execute(query, connStr);

                #region elimina Nulos
                foreach (DataRow dataRow in dt.Rows)
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (dataRow[column] == DBNull.Value)
                        {
                            dataRow[column] = "";
                        }
                    }
                }
                #endregion
                #region Cambia Nombre Columnas

                if (dt.Columns.Contains("iCodCatalogo"))
                {
                    dt.Columns["iCodCatalogo"].ColumnName = "recid";
                }
                #endregion

                List<string> customers = new List<string>();

                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;

                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);

                }

                return "{\"records\":" + serializer.Serialize(rows) + "}";
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        protected void btnAplicarFecha_Click(object sender, EventArgs e)
        {
            fechaInicio = inicioFecha.Value;
            fechaFinal = finalFecha.Value;
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            ExportData excelFile = new ExportData();
            string sp = "exec [ObtieneAccesoEtiquietacion] @Schema = '" + esquema + "',  @FechaIniRep = '{0}', @FechaFinRep = '{1}'";
            string query = string.Format(sp, fechaInicio, fechaFinal);

            DataTable dt = DSODataAccess.Execute(query, connStr);
            string file = excelFile.GeneraArchivoExcel("AccesoEtiquetacion_" + DateTime.Now.ToString("hhmmss"), fechaInicio, fechaFinal, dt, dt, 1);
            ExportFiles(file);
        }
        public void ExportFiles(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var buffer = File.ReadAllBytes(filePath);
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.AddHeader("Content-Type", "application/octet-stream");
                    Response.AddHeader("Content-disposition", "attachment; filename=\"" + Path.GetFileName(filePath) + "\"");
                    Response.BinaryWrite(buffer);
                    Response.ContentType = "application/octet-stream";
                    Response.Flush();
                    buffer = null;

                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                throw new KeytiaWebException("Ocurrio un error en : " + ex);

            }
        }
    }
}