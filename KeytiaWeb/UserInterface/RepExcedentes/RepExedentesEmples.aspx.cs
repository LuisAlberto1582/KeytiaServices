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

namespace KeytiaWeb.UserInterface.RepExcedentes
{
    public partial class RepExedentesEmples : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        static string cencos = "";
        static string puesto = "";
        static string fechaInicio1 = "";
        static string fechaFinal1 = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            fechaInicio1 = Session["FechaInicioEx"].ToString();
            fechaFinal1 = Session["FechaFinEx"].ToString();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionExcedentesEmples", "ExcedentesEmples('JSONExcedentesEmples');", true);

        }
        #region WeMethod Json
        [WebMethod]
        public static string JSONExcedentesEmples(string cencosClave,string clavePuesto)
        {
            cencos = cencosClave;
            puesto = clavePuesto;
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;

                DataTable dt = new DataTable();
                string esquema = DSODataContext.Schema;
                string connStr = DSODataContext.ConnectionString;

                string sp = "exec [ConsumoMovilAgrupPuesto] @Schema = '" + esquema + "',  @FechaIniRep = '{0}', @FechaFinRep = '{1}', @ClaveCencos = {2},@ClavePuesto = {3}";
                string query = string.Format(sp, fechaInicio1, fechaFinal1, cencos, puesto);

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

                if (dt.Columns.Contains("CenCos"))
                {
                    dt.Columns["CenCos"].ColumnName = "recid";
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
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            ExportFile excelFile = new ExportFile();
            string sp = "exec [ConsumoMovilAgrupPuesto] @Schema = '" + esquema + "',  @FechaIniRep = '{0}', @FechaFinRep = '{1}', @ClaveCencos = {2},@ClavePuesto = {3}";
            string query = string.Format(sp, fechaInicio1, fechaFinal1, cencos, puesto);

            DataTable dt = DSODataAccess.Execute(query, connStr);

            string file = excelFile.GeneraArchivoExcel("ExcedenteporEmpleado_"+DateTime.Now.ToString("hhmmss"), fechaInicio1, fechaFinal1, dt, dt, 3);

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