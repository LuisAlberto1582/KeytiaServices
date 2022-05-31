using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.RepExcedentes
{
    public partial class RepExedentesPuesto : System.Web.UI.Page
    {
        static string esquema = DSODataContext.Schema;
        static string connStr = DSODataContext.ConnectionString;
        static string cencos = "";
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
            Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionExcedentes", "Excedentes('JSONExcedentes');", true);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionHistorico", "Historico('JSONHistorico');", true);
        }
        #region WeMethod Json
        [WebMethod]
        public static string JSONExcedentes(string cencosClave)
        {
            cencos = cencosClave;
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;

                DataTable dt = new DataTable();
                string esquema = DSODataContext.Schema;
                string connStr = DSODataContext.ConnectionString;

                string sp = "exec [ConsumoMovilCencosPuesto] @Schema = '" + esquema + "',  @FechaIniRep = '{0}', @FechaFinRep = '{1}', @ClaveCencos = {2}";
                string query = string.Format(sp, fechaInicio1, fechaFinal1, cencos);

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

                if (dt.Columns.Contains("ClavePuesto"))
                {
                    dt.Columns["ClavePuesto"].ColumnName = "recid";
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
        [WebMethod]
        public static string JSONHistorico(string cencosClave1)
        {
            cencos = cencosClave1;
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;

                DataTable dt = new DataTable();
                string esquema = DSODataContext.Schema;
                string connStr = DSODataContext.ConnectionString;

                dt =  ObtieneHistoricoConsumo(Convert.ToInt32(cencos), fechaInicio1, fechaFinal1);

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

                if (dt.Columns.Contains("Concepto"))
                {
                    dt.Columns["Concepto"].ColumnName = "recid";
                }
                #endregion
                List<string> customers = new List<string>();

                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;
                 StringBuilder columns = new StringBuilder();
                foreach (DataRow dr in dt.Rows)
                {
                  
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        columns.AppendLine("{ field:\""+ col.ColumnName + "\", caption: 'ClavePuesto', size: '1', sortable: true, frozen: true },");
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
                }
                //r = new string[] { "{\"records1\":" + serializer.Serialize(rows) + "}", "{\"columnsName\":" + columns.ToString() + "}" };

                return "{\"records1\":" + serializer.Serialize(rows) + "}";
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        static DataTable ObtieneHistoricoConsumo(int cencos, string fechaInicio1,string fechaFinal1)
        {
            string sp = "EXEC [HistoricoPorDepartamento] @Schema = '" + esquema + "',  @FechaIniRep = '{0}', @FechaFinRep = '{1}', @ClaveCencos = {2}";
            string query = string.Format(sp, fechaInicio1, fechaFinal1, cencos);
            DataTable dtFinal = new DataTable();
            DataTable dt = new DataTable();
            dt = DSODataAccess.Execute(query, connStr);
            if( dt != null && dt.Rows.Count > 0)
            {
                List<HistCencos> lista = new List<HistCencos>();
                foreach (DataRow dr in dt.Rows)
                {
                    HistCencos historico = new HistCencos();
                    DateTime date;
                    date = Convert.ToDateTime(dr["FechaPub"]);
                    string nomMes = ObtieneNomMes(Convert.ToInt32(date.Month));
                    string anio = date.Year.ToString();

                    historico.FechaInt = Convert.ToInt32(dr["FECHAINT"]);
                    historico.Fecha = dr["FechaPub"].ToString();
                    historico.Mes = nomMes + '-'+ anio;
                    historico.Renta = dr["Renta"].ToString();
                    historico.Excedente = dr["Excedente"].ToString();
                    historico.Total = dr["Total"].ToString();
                    lista.Add(historico);
                }

                if(lista.Count > 0)
                {
                    var list = lista.OrderBy(x => x.FechaInt);
                    dtFinal.Columns.Add("CONCEPTO");
                    foreach (var item in list)/*crea las columnas del Datatable*/
                    {                     
                        string nomColum = item.Mes.ToString().ToUpper();
                        dtFinal.Columns.Add(nomColum);                     
                    }
                    dtFinal.Columns.Add("TOTAL");
                    Double rentaTotal = 0;
                    Double excedenteTotal = 0;
                    Double totales = 0;

                    DataRow row = dtFinal.NewRow();
                    row = dtFinal.NewRow();
                    int pas = 0;
                    foreach (var item in list)
                    {
                        if(pas == 0)
                        {
                            row["CONCEPTO"] = "Renta";
                            pas = 1;
                        }
                        string nomColum = item.Mes.ToString().ToUpper();
                        string rentaT = item.Renta;
                        row[nomColum] = rentaT;
                        rentaTotal += Convert.ToDouble(rentaT);
                    }
                    row["TOTAL"] = rentaTotal;
                    dtFinal.Rows.Add(row);

                    int b = 0;
                    row = dtFinal.NewRow();
                    foreach (var item in list)
                    {
                        if(b == 0)
                        {
                            row["CONCEPTO"] = "Excedente";
                            b = 1;
                        }
                        string nomColum = item.Mes.ToString().ToUpper();
                        string excedenteT = item.Excedente;
                        row[nomColum] = excedenteT;
                        excedenteTotal += Convert.ToDouble(excedenteT);
                    }
                    row["TOTAL"] = excedenteTotal;
                    dtFinal.Rows.Add(row);

                    int a = 0;
                    row = dtFinal.NewRow();
                    foreach (var item in list)
                    {
                        string nomColum = item.Mes.ToString().ToUpper();
                        if (a == 0)
                        {
                            row["CONCEPTO"] = "Total";
                            a = 1;
                        }
                        
                        string totalT = item.Total;
                        row[nomColum] = totalT;
                        totales += Convert.ToDouble(totalT);
                    }
                    row["TOTAL"] = totales;
                    dtFinal.Rows.Add(row);             
                }
            }

            return dtFinal;
        }
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            ExportFile excelFile = new ExportFile();
            string sp = "exec [ConsumoMovilCencosPuesto] @Schema = '" + esquema + "',  @FechaIniRep = '{0}', @FechaFinRep = '{1}', @ClaveCencos = {2}";
            string query = string.Format(sp, fechaInicio1, fechaFinal1, cencos);

            DataTable dt = DSODataAccess.Execute(query, connStr);
            DataTable dt1 = ObtieneHistoricoConsumo(Convert.ToInt32(cencos), fechaInicio1, fechaFinal1);
            string file = excelFile.GeneraArchivoExcel("ExcedentePorPuestos_"+ DateTime.Now.ToString("hhmmss"), fechaInicio1, fechaFinal1, dt, dt1, 2);

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
        static string ObtieneNomMes(int mes)
        {
            DateTimeFormatInfo dtInfo = new CultureInfo("es-ES",false).DateTimeFormat;
            return dtInfo.GetMonthName(mes);
        }
    }
    public class HistCencos
    {
        public int FechaInt { get; set; }
        public string Fecha { get; set; }
        public string Mes { get; set; }
        public string Renta { get; set; }
        public string Excedente { get; set; }
        public string Total { get; set; }

    }
}