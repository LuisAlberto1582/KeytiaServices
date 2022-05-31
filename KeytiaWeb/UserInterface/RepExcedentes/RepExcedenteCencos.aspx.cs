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

namespace KeytiaWeb.UserInterface.DashboardFC.RepExcedentes
{
    public partial class RepExcedenteCencos : System.Web.UI.Page
    {
        DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        StringBuilder query = new StringBuilder();
        static string iCodUsuario = string.Empty;
        static string iCodPerfil = string.Empty;
        Parametros param;
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        static string fechaInicio1 = "";
        static string fechaFinal1 = "";
        public void Page_PreInit(object o, EventArgs e)
        {
            EnsureChildControls();
            Page.ClientScript.GetPostBackEventReference(this, "");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.Attributes.Add("enctype", "multipart/form-data");

            try
            {
                esquema = DSODataContext.Schema;
                connStr = DSODataContext.ConnectionString;

                #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
                ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
                #endregion
                if (!Page.IsPostBack)
                {
                    #region Inicia los valores default de los controles de fecha y banderas de clientes y perfiles
                    try
                    {
                        if (Session["Language"].ToString() == "Español")
                        {
                            pdtInicio.setRegion("es");
                        }

                        pdtInicio.MaxDateTime = DateTime.Today;
                        pdtInicio.MinDateTime = new DateTime((DateTime.Now.Year - 2), 1, 1);
                        CalculaFechasDeDashboard();

                        iCodUsuario = Session["iCodUsuario"].ToString();
                        iCodPerfil = Session["iCodPerfil"].ToString();
                    }
                    catch (Exception ex)
                    {
                        throw new KeytiaWebException(
                            "Ocurrio un error al darle valores default a los campos de fecha en '" + Request.Path
                            + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                    }
                    #endregion Inicia los valores default de los controles de fecha

                    //Parametros Iniciales
                    param = new Parametros();
                    param.PeriodoInfo = Convert.ToDateTime(Session["FechaInicioEx"]).ToString("yyyy-MM-dd") + " 00:00:00";
                    fechaInicio1 = Session["FechaInicioEx"].ToString();
                    fechaFinal1 = Session["FechaFinEx"].ToString();
                }
                else
                {
                    fechaInicio = Convert.ToDateTime(pdtInicio.Date);
                    Session["FechaInicioEx"] = fechaInicio.ToString("yyyy-MM-dd");
                    Session["FechaFinEx"] = fechaInicio.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                    fechaInicio1 = Session["FechaInicioEx"].ToString();
                    fechaFinal1 = Session["FechaFinEx"].ToString();

                }

                IniciaProceso();              
            }
            catch
            {

            }
        }
        private void CalculaFechasDeDashboard()
        {
            #region Calcula Fechas

            DataTable fechaMaxFact = DSODataAccess.Execute(ConsultaFechaMaximaFactura());
            // Se valida que el datatable contenga resultados.
            if (fechaMaxFact.Rows.Count > 0)
            {
                fechaInicio = Convert.ToDateTime(fechaMaxFact.Rows[0][0]);
            }
            else { fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); }

            pdtInicio.CreateControls();
            pdtInicio.DataValue = (object)fechaInicio;

            Session["FechaInicioEx"] = fechaInicio.ToString("yyyy-MM-dd");
            Session["FechaFinEx"] = fechaInicio.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            #endregion Calcula Fechas
        }
        private string ConsultaFechaMaximaFactura()
        {
            query.Length = 0;
            query.AppendLine("SELECT isnull(MAX(FechaPub),GETDATE())");
            query.AppendLine("FROM " + DSODataContext.Schema + ".ResumenFacturasDeMoviles WITH(NOLOCK)");
            return query.ToString();
        }
        #region Metodos
        private void IniciaProceso()
        {
            ObtieneInfo();
        }
        private void ObtieneInfo()
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionCabeceras", "Cabeceras('JSONCabeceras');", true);
        }
        #endregion Metodos
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

                string sp = "exec [RepEspecialExcedentes] @Schema = '" + esquema + "',  @FechaIniRep = '{0}', @FechaFinRep = '{1}'";
                string query = string.Format(sp, fechaInicio1, fechaFinal1);

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

                if (dt.Columns.Contains("ClaveCencos"))
                {
                    dt.Columns["ClaveCencos"].ColumnName = "recid";
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
            fechaInicio = Convert.ToDateTime(pdtInicio.Date);
            Session["FechaInicioEx"] = fechaInicio.ToString("yyyy-MM-dd");
            Session["FechaFinEx"] = fechaInicio.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            fechaInicio1 = Session["FechaInicioEx"].ToString();
            fechaFinal1 = Session["FechaFinEx"].ToString();
            IniciaProceso();
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            ExportFile excelFile = new ExportFile();
            string sp = "exec [RepEspecialExcedentes] @Schema = '" + esquema + "',  @FechaIniRep = '{0}', @FechaFinRep = '{1}'";
            string query = string.Format(sp, fechaInicio1, fechaFinal1);
            
            DataTable dt = DSODataAccess.Execute(query, connStr);
            
            string file = excelFile.GeneraArchivoExcel("ExcedentePorDepartamentos_"+ DateTime.Now.ToString("hhmmss"), fechaInicio1, fechaFinal1,dt,dt,1);

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