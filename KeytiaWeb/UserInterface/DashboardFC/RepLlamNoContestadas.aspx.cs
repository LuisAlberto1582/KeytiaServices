using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class RepLlamNoContestadas : System.Web.UI.Page
    {
        static string esquema = DSODataContext.Schema;
        static string connStr = DSODataContext.ConnectionString;
        static string fechaInicio = "";
        static string fechaFinal = "";
        static string iCodUsuario = string.Empty;
        static string iCodPerfil = string.Empty;
        static int tipoLlam;
        static int tipoDetall;
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            iCodUsuario = Session["iCodUsuario"].ToString();
            iCodPerfil = Session["iCodPerfil"].ToString();
            //if (!Page.IsPostBack)
            //{
            //    IniciaProceso();
            //}
            IniciaProceso();
        }
        public void IniciaProceso()
        {
            tipoDetall = Convert.ToInt32(rbtnList.SelectedValue);
            
            if(rbtnEnlace.Checked == true)
            {
                fechaInicio = inicioFecha.Value;
                fechaFinal = finalFecha.Value;
                tipoLlam = 1;
                
            }
            else if(rbtnEntrada.Checked == true)
            {
                fechaInicio = inicioFecha.Value;
                fechaFinal = finalFecha.Value;
                tipoLlam = 0;
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionRepLlamNoContestadas", "RepLlamNoContestadas('JSONRepLlamNoContestadas');", true);
        }
        protected void btnAplicarFecha_Click(object sender, EventArgs e)
        {
            fechaInicio = inicioFecha.Value;
            fechaFinal = finalFecha.Value;
            if(rbtnEnlace.Checked == true)
            {
                tipoLlam = 1;
            }
            else if(rbtnEntrada.Checked == true)
            {
                tipoLlam = 0;
            }
            Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionRepLlamNoContestadas", "RepLlamNoContestadas('JSONRepLlamNoContestadas');", true);
        }
        #region WeMethod Json
        [WebMethod]
        public static string JSONRepLlamNoContestadas()
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;

                DataTable dt = new DataTable();
                string esquema = DSODataContext.Schema;
                string connStr = DSODataContext.ConnectionString;
                if(tipoLlam == 1)
                {
                    string sp = "EXEC [dbo].RepLlamNoContestadasEnlace @Schema = '" + esquema + "', @FechaIni = '{0}',@FechaFin='{1}',@TipoDetall={2}";
                    string query = string.Format(sp, fechaInicio, fechaFinal,tipoDetall);
                    dt = DSODataAccess.Execute(query, connStr);
                }
                else if(tipoLlam == 0)
                {
                    dt = ResFinal();
                }
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

                if (dt.Columns.Contains("Tipodestino"))
                {
                    dt.Columns["Tipodestino"].ColumnName = "recid";
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
        private static DataTable ResFinal()
        {
            DataTable dt1 = new DataTable();
            string sp = "EXEC [dbo].RepLlamNoContestadasEntrada @Schema = '" + esquema + "', @FechaIni = '{0}',@FechaFin='{1}',@TipoDetall={2}";
            string query = string.Format(sp, fechaInicio, fechaFinal, tipoDetall);
            DataTable dt = DSODataAccess.Execute(query, connStr);
            if(dt != null && dt.Rows.Count>0)
            {
                dt1 = FiltraData(dt);
            }
            return dt1;
        }
        private static DataTable FiltraData(DataTable dt)
        {
            DataTable dt1 = new DataTable();
            //dt1.Columns.Add("recid", typeof(int));
            dt1.Columns.Add("Tipodestino", typeof(string));
            dt1.Columns.Add("Fecha", typeof(string));
            dt1.Columns.Add("Hora", typeof(string));
            dt1.Columns.Add("CallerID", typeof(string));
            dt1.Columns.Add("ExtOriginal", typeof(string));
            dt1.Columns.Add("ExtDesvia", typeof(string));
            dt1.Columns.Add("ExtFinal", typeof(string));
            dt1.Columns.Add("Desconexion", typeof(string));
            dt1.Columns.Add("UltimoRedireccion", typeof(string));
            dt1.Columns.Add("Duracion", typeof(int));
            int contador=1;
            foreach (DataRow dr in dt.Rows)
            {
                string origDeviceName = dr["OrigDeviceName"].ToString();
                string destDeviceName = dr["destDeviceName"].ToString();
                string tipoDestino = dr["Tipodestino"].ToString();
                string fecha = dr["Fecha"].ToString();
                string hora = dr["Hora"].ToString();
                string callerId = dr["CallerID"].ToString();
                string extOriginal = dr["ExtOriginal"].ToString();
                string extDesvia = dr["ExtDesvia"].ToString();
                string extFinal = dr["ExtFinal"].ToString();
                string descon = dr["Desconexion"].ToString();
                string redirect = dr["UltimoRedireccion"].ToString();
                int duracion = Convert.ToInt32(dr["Duracion"]);
                int origDevice = ValidaDatos(origDeviceName);
                int destDevice = ValidaDatos(destDeviceName);
                if(origDevice == 1 && destDevice == 0)
                {
                    dt1.Rows.Add(tipoDestino, fecha, hora, callerId, extOriginal, extDesvia, extFinal, descon, redirect, duracion);
                    contador++;
                }
            }

            return dt1;
        }
        public static int ValidaDatos(string dato)
        {
            string v = @"^SIP|^Trunk|^TRUNK|^LBO|^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$";
            Match ve = Regex.Match(dato, v);
            int resultado = 0;
            if (ve.Success)
            {
                resultado = 1;
            }

            return resultado;
        }
      
        #endregion
       
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            if (rbtnEnlace.Checked == true)
            {
                tipoLlam = 1;
            }
            else if (rbtnEntrada.Checked == true)
            {
                tipoLlam = 0;
            }

            tipoDetall = Convert.ToInt32(rbtnList.SelectedValue);

            if (fechaInicio != "" && fechaFinal != "")
            {
                ExportFiles excelFile = new ExportFiles();
                string nomFile = "";
                DataTable dt = new DataTable();
                ExcelAccess lExcel = new ExcelAccess();
                try
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTabla" + ".xlsx");
                    lExcel.Abrir();
                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");
                    nomFile = "ReporteLlamadas_";
                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Tipo Destino");
                    DataTable ldt = new DataTable();
                    if (tipoLlam == 1)
                    {
                        string sp = "EXEC [dbo].RepLlamNoContestadasEnlace @Schema = '" + esquema + "', @FechaIni = '{0}',@FechaFin='{1}',@TipoDetall={2}";
                        string query = string.Format(sp, fechaInicio, fechaFinal, tipoDetall);
                        ldt = DSODataAccess.Execute(query, connStr);
                    }
                    else if(tipoLlam == 0)
                    {
                        ldt = ResFinal();
                    }

                    if (ldt.Rows.Count > 0)
                    {
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                    }
                    string psFileKey;
                    string psTempPath;

                    psFileKey = Guid.NewGuid().ToString();
                    psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
                    System.IO.Directory.CreateDirectory(psTempPath);

                    string lsFileName = System.IO.Path.Combine(psTempPath, "cc." + psFileKey + ".temp" + ".xlsx");
                    Session[psFileKey] = lsFileName;

                    lExcel.FilePath = lsFileName;
                    lExcel.SalvarComo();

                    ExportarArchivo(".xlsx", psFileKey, psTempPath, "Reportes" + "_" + nomFile);
                }
                catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
                catch (Exception ex)
                {
                    throw new KeytiaWebException("ErrExportTo", ex, ".xlsx");
                }
                finally
                {
                    if (lExcel != null)
                    {
                        lExcel.Cerrar(true);
                        lExcel.Dispose();

                    }
                }
            }

        }
        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {
            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        protected void rbtnList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

}