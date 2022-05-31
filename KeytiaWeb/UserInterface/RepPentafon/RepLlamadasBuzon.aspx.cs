using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.RepPentafon
{
    public partial class RepLlamadasBuzon : System.Web.UI.Page
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
                fechaInicio = "";
                fechaFinal = "";
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionLlamBuzon", "LlamBuzon('JSONLlamBuzon');", true);
        }

        private static DataTable ObtieneSitios(string esquema,string connStr)
        {
            StringBuilder query = new StringBuilder();
            //query.AppendLine(" SELECT iCodCatalogo, vchDescripcion FROM " + esquema + ".HistSitio WITH(NOLOCK)");
            //query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia>= GETDATE()");
            query.AppendLine(" SELECT Sitio AS iCodCatalogo,SitioDesc AS vchDescripcion  FROM " + esquema + ".[VisAcumulados('AcumDia','ResumenCDR','Español')]WITH(NOLOCK)");
            query.AppendLine(" WHERE fechaInicio  >= '"+ fechaInicio + "' AND fechaInicio <= '"+ fechaFinal + "'");
            query.AppendLine(" GROUP BY Sitio, SitioDesc");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            return dt;
        }
        #region WeMethod Json
        [WebMethod]
        public static string JSONLlamBuzon()
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;

                DataTable dt = new DataTable();
                string esquema = DSODataContext.Schema;
                string connStr = DSODataContext.ConnectionString;

                dt = ObtieneInfoSitios(esquema, connStr);

                //dt = DSODataAccess.Execute(query, connStr);

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
        private static DataTable ObtieneInfoSitios(string esquema,string connStr)
        {
            string sp = "EXEC [ObtieneLlamadasBuzonSitios] @Schema = '" + esquema + "',  @Fechainicio = '{0}', @Fechafin = '{1}',@Sitio = {2}";
            DataTable dt = new DataTable();
            dt.Columns.Add("iCodCatalogo");
            dt.Columns.Add("Descripcion");
            dt.Columns.Add("TotalGral");
            dt.Columns.Add("TotSmart");
            dt.Columns.Add("TotLlamSmart");
            dt.Columns.Add("TotalLlamGral");
            dt.Columns.Add("TotMinGrla");

            int contador = 1;
            if(fechaInicio != "" && fechaFinal != "")
            {
                DataTable dts = ObtieneSitios(esquema, connStr);
                if(dts != null && dts.Rows.Count > 0)
                {
                    foreach (DataRow item in dts.Rows)
                    {
                        int IcodSitio = Convert.ToInt32(item["iCodCatalogo"]);
                        string nomSitio = item["vchDescripcion"].ToString();

                        string query = string.Format(sp, fechaInicio, fechaFinal, IcodSitio);
                        DataTable dt1 = DSODataAccess.Execute(query, connStr);

                        if(dt1 != null && dt1.Rows.Count > 0)
                        {
                            DataRow dr1 = dt1.Rows[0];
                            DataRow row = dt.NewRow();
                            row["iCodCatalogo"] = contador;
                            row["Descripcion"] = nomSitio;
                            row["TotalGral"] = Convert.ToDecimal(dr1["TotalGral"]);
                            row["TotSmart"] = Convert.ToDecimal(dr1["TotSmart"]);
                            row["TotLlamSmart"] = Convert.ToInt32(dr1["TotLlamSmart"]);
                            row["TotalLlamGral"] = Convert.ToInt32(dr1["TotalLlamGral"]);
                            row["TotMinGrla"] = Convert.ToInt32(dr1["TotMinGrla"]);
                            dt.Rows.Add(row);

                            contador++;

                        }
                    }
                }

            }
            
            return dt;

        }
        protected void btnAplicarFecha_Click(object sender, EventArgs e)
        {
            fechaInicio = inicioFecha.Value;
            fechaFinal = finalFecha.Value;
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            if (fechaInicio != "" && fechaFinal != "")
            {
                DataTable dt = ObtieneInfoSitios(esquema, connStr);
                ExportExcel excelFile = new ExportExcel();
                string pathFile = excelFile.GeneraArchivoExcel("LlamadasBuzon_" + DateTime.Now.ToString("hhmmss"), fechaInicio, fechaFinal, dt, dt, 1, 1);
                ExportFiles(pathFile);
            }

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