using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KeytiaWeb.UserInterface.DepuracionDetalleCDR.Models;

namespace KeytiaWeb.UserInterface.DepuracionDetalleCDR
{
    public partial class DepuracionDetalleCDR : System.Web.UI.Page
    {
        private string esquema;
        private string iCodUsuario;
        private string connStr;
        private string usuario;
        private string password;
        private string servidor;
        private string bd;
        private string path;
        protected void Page_Load(object sender, EventArgs e)
        {
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            esquema = DSODataContext.Schema;
            iCodUsuario = HttpContext.Current.Session["iCodUsuario"].ToString();
            connStr = DSODataContext.ConnectionString;
            SqlConnectionStringBuilder connString = new SqlConnectionStringBuilder(connStr);
            usuario = connString.UserID;
            password = connString.Password;
            servidor = connString.DataSource;
            bd = connString.InitialCatalog;
            path = ConfigurationManager.AppSettings.Get("RutaRespaldos");
            if (!Page.IsPostBack)
            {
                ObtenerMesesEnDetalle();
            }
        }

        protected void ObtenerMesesEnDetalle()
        {
            DataTable dt = new DataTable();
            StringBuilder consulta = new StringBuilder();

            gvMeses.DataSource = null;
            gvMeses.DataBind();

            consulta.AppendLine("select datepart(yyyy, convert(DateTime, FechaInicio)) as Anio");
            consulta.AppendLine(",datepart(mm, convert(DateTime, FechaInicio)) as Mes");
            consulta.AppendLine(",COUNT(*) as CantidadLlamadas");
            consulta.AppendLine(",sum(convert(float, costo)) as CostoTotal");
            consulta.AppendLine(",sum(duracionMin) as CantidadMinutos");
            consulta.AppendLine("from " + esquema + ".[Visdetallados('Detall','DetalleCDR','Español')]");
            consulta.AppendLine("group by datepart(yyyy, convert(DateTime, FechaInicio)),");
            consulta.AppendLine("datepart(mm, convert(DateTime, FechaInicio))");
            consulta.AppendLine("order by datepart(yyyy, convert(DateTime, FechaInicio)),");
            consulta.AppendLine("datepart(mm, convert(DateTime, FechaInicio))");

            dt = DSODataAccess.Execute(consulta.ToString(), connStr);

            gvMeses.DataSource = dt;
            gvMeses.DataBind();
        }

        protected void btnRespaldo_Click(object sender, EventArgs e)
        {
            string tabla;
            bool sel = RespaldarMesesSeleccionados(out tabla);
            if (!sel)
            {
                lblTituloModalMensaje.Text = "Error";
                lblMensaje.Text = "No se seleccionó ningún mes para respaldar.";
                mpeEtqMensaje.Show();
            }
            else
            {
                lblTituloModalMensaje.Text = "Proceso terminado";
                lblMensaje.Text = "Proceso de respaldo terminado" + tabla;
                mpeEtqMensaje.Show();
            }
        }

        protected bool RespaldarMesesSeleccionados(out string tabla)
        {
            bool sel = false;
            tabla = "";
            foreach (GridViewRow row in gvMeses.Rows)
            {
                CheckBox check = row.FindControl("chkSel") as CheckBox;

                if (check.Checked)
                {
                    sel = true;
                    int anio = Convert.ToInt16(row.Cells[1].Text);
                    int mes = Convert.ToInt16(row.Cells[2].Text);
                    string cantidadRegistros = row.Cells[3].Text;
                    string totalEnMoneda = row.Cells[4].Text.Replace("$", "").Replace(",", "");
                    string cantidadMinutos = row.Cells[5].Text;
                    bool ok = GenerarRespaldo(anio, mes, cantidadRegistros, totalEnMoneda, cantidadMinutos);
                    if (!ok)
                    {
                        tabla = tabla +
                            "<tr>" +
                                "<td>" + anio.ToString() + "</td>" +
                                "<td>" + mes.ToString() + "</td>" +
                            "</tr>";
                    }
                }
            }

            if (!String.IsNullOrEmpty(tabla))
            {
                tabla = @", los siguientes meses no se respaldaron correctamente:<br/><br/> 
                      <table class='tablaMsg'>
                        <tr>
                            <th>Año</th>
                            <th>Mes</th>
                        </tr>"
                    + tabla +
                "</Table>";
            }


            return sel;
        }

        protected bool GenerarRespaldo(int anio, int mes, string cantidadRegistros, string totalEnMoneda, string cantidadMinutos)
        {
            DateTime fechaInicial = new DateTime(anio, mes, 1);
            DateTime fechaFinal = fechaInicial.AddMonths(1).AddDays(-1);
            int diaInicial = Convert.ToInt32(fechaInicial.ToString("dd"));
            int diaFinal = Convert.ToInt32(fechaFinal.ToString("dd"));
            double noReg = Convert.ToDouble(cantidadRegistros);

            for (int i = diaInicial; i <= diaFinal; i++)
            {
                DateTime fecha = new DateTime(anio, mes, i);
                GeneraDetalladosBcp(fecha);
                GeneraDetalleBcp(fecha);
            }
            string archivoDetallados = UneArchivosDetallados(fechaInicial, fechaFinal);
            string archivoDetalle = UneArchivosDetalle(fechaInicial, fechaFinal);
            double noRegDetallados = ContarLineas(archivoDetallados, mes, anio);
            double noRegDetalle = ContarLineas(archivoDetalle, mes, anio);

            if (noReg == noRegDetallados && noReg == noRegDetalle)
            {

                if (!ComprimeArchivo(archivoDetallados))
                {
                    return false;
                }
                if (!ComprimeArchivo(archivoDetalle))
                {
                    return false;
                }
                if (!ActualizaBitacoraDepuracionDetalleCDR(anio, mes, cantidadRegistros, totalEnMoneda, cantidadMinutos))
                {
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }

        }

        protected void GeneraDetalleBcp(DateTime fecha)
        {
            string storedProcedure = "SPDetalladosCamposRespaldo";
            string nombreArchivo = path + "DetalleBcp\\" + fecha.ToString("dd") + ".txt";

            if (!Directory.Exists(path + "DetalleBcp\\"))
            {
                Directory.CreateDirectory(path + "DetalleBcp\\");
            }

            if (File.Exists(nombreArchivo))
            {
                File.Delete(nombreArchivo);
            }

            string query = string.Format("exec {0}..{1} '{2}','{3}','{4}'", bd, storedProcedure, esquema, fecha.ToString("yyyy-MM-dd 00:00:00"), fecha.ToString("yyyy-MM-dd 23:59:59"));

            string bcpCMD = "bcp \"" + query + "\" queryout \"" + nombreArchivo + "\" -w -t\"|\" -r\\n  -U " + usuario + " -P " + password + " -S " + servidor;

            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + bcpCMD);

            procStartInfo.RedirectStandardOutput = false;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = false;
            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            proc.WaitForExit();
            proc.Close();
            proc.Dispose();
        }

        protected void GeneraDetalladosBcp(DateTime fecha)
        {

            string nombreArchivo = path + "DetalladosBcp\\" + fecha.ToString("dd") + ".txt";

            if (!Directory.Exists(path + "DetalladosBcp\\"))
            {
                Directory.CreateDirectory(path + "DetalladosBcp\\");
            }

            if (File.Exists(nombreArchivo))
            {
                File.Delete(nombreArchivo);
            }

            string query = string.Format("select * from {0}.{1}.Detallados where icodmaestro = 89 and Date01 >='{2}' and Date01 <= '{3}'",
                bd, esquema, fecha.ToString("yyyy-MM-dd 00:00:00"), fecha.ToString("yyyy-MM-dd 23:59:59"));

            string bcpCMD = "bcp \"" + query + "\" queryout \"" + nombreArchivo + "\" -c -t\"|\" -r\\n  -U " + usuario + " -P " + password + " -S " + servidor;

            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + bcpCMD);

            procStartInfo.RedirectStandardOutput = false;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = false;
            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            proc.WaitForExit();
            proc.Close();
            proc.Dispose();
        }

        private string UneArchivosDetalle(DateTime fechaInicial, DateTime fechaFinal)
        {

            string rutaArchivos = path + "DetalleBcp\\";
            string archivo = String.Format("{0}.detalle({1}-{2}).bcp", esquema, fechaInicial.ToString("yyyyMMdd"), fechaFinal.ToString("yyyyMMdd"));

            if (File.Exists(path + archivo))
            {
                File.Delete(path + archivo);
            }

            List<string> listaArchivos = new List<string>();
            listaArchivos = Directory.GetFiles(rutaArchivos).ToList();


            using (StreamWriter Output = new StreamWriter(path + archivo, true, Encoding.Unicode))
            {
                foreach (string iFile in listaArchivos)
                {
                    using (StreamReader ReadFile = new StreamReader(iFile, Encoding.Unicode))
                    {

                        while (ReadFile.Peek() != -1)
                        {
                            Output.WriteLine(ReadFile.ReadLine());
                        }
                    }
                }
            }
            Directory.Delete(rutaArchivos, true);
            return archivo;
        }

        private string UneArchivosDetallados(DateTime fechaInicial, DateTime fechaFinal)
        {

            string rutaArchivos = path + "DetalladosBcp\\";
            string archivo = String.Format("{0}.detallados({1}-{2}).bcp", esquema, fechaInicial.ToString("yyyyMMdd"), fechaFinal.ToString("yyyyMMdd"));

            if (File.Exists(path + archivo))
            {
                File.Delete(path + archivo);
            }

            List<string> listaArchivos = new List<string>();
            listaArchivos = Directory.GetFiles(rutaArchivos).ToList();


            using (StreamWriter Output = new StreamWriter(path + archivo, true, Encoding.Default))
            {
                foreach (string iFile in listaArchivos)
                {
                    using (StreamReader ReadFile = new StreamReader(iFile, Encoding.Default))
                    {

                        while (ReadFile.Peek() != -1)
                        {
                            Output.WriteLine(ReadFile.ReadLine());
                        }
                    }
                }
            }
            Directory.Delete(rutaArchivos, true);
            return archivo;
        }

        protected double ContarLineas(string archivo, int mes, int anio)
        {
            string[] lineas = File.ReadAllLines(path + archivo);
            double cantidadLineas = lineas.Length;
            return cantidadLineas;
        }

        protected bool ComprimeArchivo(string nombreArchivoOrigen)
        {
            try
            {
                string RarPath = ConfigurationManager.AppSettings.Get("DirectorioWinRar");
                string rutaYNombreOrigen = path + nombreArchivoOrigen;
                string rutaYNombreDestino = Path.ChangeExtension(rutaYNombreOrigen, ".rar");

                if (File.Exists(rutaYNombreDestino))
                {
                    File.Delete(rutaYNombreDestino);
                }

                //COMPRIME EL ARCHIVO ORIGEN EN LA CARPETA DESTINO
                string argumento = string.Format("A -IBCK -ep {0} {1}", String.Format("\"{0}\"", rutaYNombreDestino), String.Format("\"{0}\"", rutaYNombreOrigen));
                Process process = new Process();
                process.StartInfo.FileName = RarPath;
                process.StartInfo.Arguments = argumento;
                process.Start();
                process.WaitForExit();

                File.Delete(rutaYNombreOrigen);

                return true;
            }
            catch
            {
                Util.LogMessage("DepuracionDetalleCDR: Error comprimiendo respaldo");
                return false;
            }


        }

        protected bool ActualizaBitacoraDepuracionDetalleCDR(int anio, int mes, string cantidadRegistros, string totalEnMoneda, string cantidadMinutos)
        {
            bool updateOK = true;
            bool insertOK = false;
            DateTime fechaInicial = new DateTime(anio, mes, 1);
            DateTime fechaFinal = fechaInicial.AddMonths(1).AddDays(-1);
            StringBuilder insert = new StringBuilder();
            DataRow dr = null;
            dr = BuscaBitacoraDepuracionDetalleCDR(anio, mes);

            if (dr != null)
            {
                StringBuilder update = new StringBuilder();

                update.AppendLine("UPDATE " + esquema + ".[BitacoraDepuracionDetalleCDR]");
                update.AppendLine(" SET dtFinVigencia = dtIniVigencia");
                update.AppendLine(" WHERE iCodRegistro = " + dr["iCodRegistro"]);

                updateOK = DSODataAccess.ExecuteNonQuery(update.ToString(), connStr);
            }

            if (updateOK)
            {
                insert.AppendLine("INSERT INTO " + esquema + ".[BitacoraDepuracionDetalleCDR] ");
                insert.AppendLine("([FechaInicio], [FechaFin], [FechaMovimiento], [CantidadRegistros], [TotalEnMoneda], ");
                insert.AppendLine("[CantidadMinutos], [RutaArchivo], [dtIniVigencia], [dtFinVigencia], [iCodUsuario], [dtFecUltAct]) VALUES(");
                insert.AppendLine("'" + fechaInicial.ToString("yyyy-MM-dd 00:00:00") + "'");
                insert.AppendLine(" , '" + fechaFinal.ToString("yyyy-MM-dd 23:59:59") + "'");
                insert.AppendLine(" , GetDate()");
                insert.AppendLine(" , " + cantidadRegistros);
                insert.AppendLine(" , " + totalEnMoneda);
                insert.AppendLine(" , " + cantidadMinutos);
                insert.AppendLine(" , ''");
                insert.AppendLine(" , '2011-01-01'");
                insert.AppendLine(" , '2079-01-01'");
                insert.AppendLine(" , " + iCodUsuario);
                insert.AppendLine(" , GetDate())");

                insertOK = DSODataAccess.ExecuteNonQuery(insert.ToString(), connStr);
            }

            return insertOK;
        }

        protected DataRow BuscaBitacoraDepuracionDetalleCDR(int anio, int mes)
        {
            DateTime fechaInicial = new DateTime(anio, mes, 1);
            DateTime fechaFinal = fechaInicial.AddMonths(1).AddDays(-1);
            StringBuilder consulta = new StringBuilder();
            DataRow dr = null;
            consulta.AppendLine("SELECT iCodRegistro");
            consulta.AppendLine(" ,FechaInicio");
            consulta.AppendLine(" ,FechaFin");
            consulta.AppendLine(" ,FechaMovimiento");
            consulta.AppendLine(" ,CantidadRegistros");
            consulta.AppendLine(" ,TotalEnMoneda");
            consulta.AppendLine(" ,CantidadMinutos");
            consulta.AppendLine(" ,RutaArchivo");
            consulta.AppendLine(" ,dtIniVigencia");
            consulta.AppendLine(" ,dtFinVigencia");
            consulta.AppendLine(" ,iCodUsuario");
            consulta.AppendLine(" ,dtFecUltAct");
            consulta.AppendLine(" FROM " + esquema + ".[BitacoraDepuracionDetalleCDR]");
            consulta.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia");
            consulta.AppendLine(" AND FechaInicio = '" + fechaInicial.ToString("yyyy-MM-dd 00:00:00") + "'");
            consulta.AppendLine(" AND FechaFin = '" + fechaFinal.ToString("yyyy-MM-dd 23:59:59") + "'");


            return dr = DSODataAccess.ExecuteDataRow(consulta.ToString(), connStr);
        }
        protected void btnDepuracion_Click(object sender, EventArgs e)
        {
            bool correcto;
            string tblSeleccionados;
            string tblIncorrectos;
            List<Seleccionado> seleccionados;

            bool sel = ValidarRespaldos(out correcto, out tblSeleccionados, out tblIncorrectos, out seleccionados);

            if (sel)
            {
                if (correcto)
                {
                    EjecutarDepuracion(seleccionados);
                }
                else
                {
                    lblTituloModalMensaje.Text = "Error";
                    lblMensaje.Text = "No se pueden ejecutar la depuración, el respaldo de los siguientes meses no existe o no se generó correctamente:<br/><br/>"
                        + tblIncorrectos;
                    mpeEtqMensaje.Show();
                }
            }
            else
            {
                lblTituloModalMensaje.Text = "Error";
                lblMensaje.Text = "No se seleccionó ningún mes para depurar.";
                mpeEtqMensaje.Show();
            }


        }

        protected bool ValidarCargasInicializadas()
        {
            bool inicializadas = false;
            DataTable dt = new DataTable();
            StringBuilder consulta = new StringBuilder();

            consulta.AppendLine("select iCodCatalogo, EstCargaDesc, registros, regd, regp");
            consulta.AppendLine("from " + esquema + ".[VisHistoricos('Cargas', 'Cargas CDRs', 'Español')] with(NOLOCK)");
            consulta.AppendLine("where EstCargaCod like '%CarInicial%'");
            consulta.AppendLine("and dtinivigencia <> dtfinvigencia  and dtfinvigencia> getdate()");

            dt = DSODataAccess.Execute(consulta.ToString(), connStr);

            int filas = dt.Rows.Count;

            if (filas > 0)
            {
                inicializadas = true;
            }

            return inicializadas;
        }


        protected bool ValidarRespaldos(out bool correcto, out string tblSeleccionados, out string tblIncorrectos, out List<Seleccionado> seleccionados)
        {
            seleccionados = new List<Seleccionado>();
            correcto = true;

            tblIncorrectos = @"<table class='tablaMsg'>
                        <tr>
                            <th>Año</th>
                            <th>Mes</th>
                        </tr>";

            tblSeleccionados = @"<table class='tablaMsg'>
                        <tr>
                            <th>Año</th>
                            <th>Mes</th>
                        </tr>";

            foreach (GridViewRow row in gvMeses.Rows)
            {
                CheckBox check = row.FindControl("chkSel") as CheckBox;

                if (check.Checked)
                {
                    int anio = Convert.ToInt16(row.Cells[1].Text);
                    int mes = Convert.ToInt16(row.Cells[2].Text);
                    int cantidadRegistros = Convert.ToInt32(row.Cells[3].Text);
                    decimal totalEnMoneda = Convert.ToDecimal(row.Cells[4].Text.Replace("$", "").Replace(",", ""));
                    int cantidadMinutos = Convert.ToInt32(row.Cells[5].Text);

                    bool existe = ValidaBitacora(anio, mes, cantidadRegistros, totalEnMoneda, cantidadMinutos);

                    seleccionados.Add(new Seleccionado
                    {
                        anio = anio,
                        mes = mes
                    });

                    tblSeleccionados = tblSeleccionados +
                            "<tr>" +
                                "<td>" + anio.ToString() + "</td>" +
                                "<td>" + mes.ToString() + "</td></tr>";
                    if (!existe)
                    {
                        correcto = false;
                        tblIncorrectos = tblIncorrectos +
                            "<tr>" +
                                "<td>" + anio.ToString() + "</td>" +
                                "<td>" + mes.ToString() + "</td></tr>";
                    }
                }
            }

            tblSeleccionados = tblSeleccionados + "</Table>";

            tblIncorrectos = tblIncorrectos + "</Table>";

            if (seleccionados.Count == 0)
            {
                return false;
            }
            else
            {
                Session["seleccionados"] = seleccionados;
                return true;
            }

        }

        protected bool ValidaBitacora(int anio, int mes, int cantidadRegistros, decimal totalEnMoneda, int cantidadMinutos)
        {
            bool existe = false;
            DataRow dr = null;
            dr = BuscaBitacoraDepuracionDetalleCDR(anio, mes);
            int cantidadRegistrosBD = Convert.ToInt32(dr["CantidadRegistros"].ToString());
            decimal totalEnMonedaBD = Convert.ToDecimal(dr["TotalEnMoneda"].ToString());
            int cantidadMinutosBD = Convert.ToInt32(dr["CantidadMinutos"].ToString());
            if (dr != null)
            {
                if (cantidadRegistrosBD == cantidadRegistros &&
                    totalEnMonedaBD == totalEnMoneda &&
                    cantidadMinutosBD == cantidadMinutos)
                {
                    existe = true;
                }
            }

            return existe;
        }

        protected void EjecutarDepuracion(List<Seleccionado> seleccionados)
        {
            bool cargasInicializadas = ValidarCargasInicializadas();
            if (!cargasInicializadas)
            {
                bool vigenciaActualizada = ActualizarVigenciaCargas(0);
                if (vigenciaActualizada)
                {
                    byte regeneraIndices = 0;
                    int cont = 0;
                    string tblIncorrectos = "";
                    foreach (var s in seleccionados)
                    {
                        cont = cont + 1;
                        if (cont == seleccionados.Count)
                        {
                            regeneraIndices = 1;
                        }
                        bool eliminado = EliminarDetalle(s.mes, s.anio, regeneraIndices);
                        if (!eliminado)
                        {
                            tblIncorrectos = tblIncorrectos +
                            "<tr>" +
                                "<td>" + s.anio.ToString() + "</td>" +
                                "<td>" + s.mes.ToString() + "</td></tr>";
                        }
                    }
                    ActualizarVigenciaCargas(1);
                    if (String.IsNullOrEmpty(tblIncorrectos))
                    {
                        lblTituloModalMensaje.Text = "Proceso terminado";
                        lblMensaje.Text = "El proceso de depuración se ejecutó correctamente.";
                        ObtenerMesesEnDetalle();
                        mpeEtqMensaje.Show();
                    }
                    else
                    {
                        tblIncorrectos = @"<table class='tablaMsg'>
                        <tr>
                            <th>Año</th>
                            <th>Mes</th>
                        </tr>" +
                        tblIncorrectos +
                        "</Table>";

                        lblTituloModalMensaje.Text = "Proceso terminado";
                        lblMensaje.Text = "El proceso de depuración terminó con errores, los siguientes meses no se pudieron eliminar:<br/><br/>" + tblIncorrectos;
                        ObtenerMesesEnDetalle();
                        mpeEtqMensaje.Show();
                    }
                }
            }
            else
            {
                lblTituloModalMensaje.Text = "Error";
                lblMensaje.Text = "No se puede iniciar la depuración ya que existen cargas inicializadas para este esquema, intenta de nuevo más tarde.";
                mpeEtqMensaje.Show();
            }
        }

        protected bool ActualizarVigenciaCargas(byte paso)
        {
            StringBuilder consulta = new StringBuilder();
            if (paso == 0)
            {
                consulta.AppendLine("UPDATE " + esquema + ".[vishistoricos('CargasA','Cargas CDRs','Español')]");
                consulta.AppendLine(" set dtFinVigencia = '2011-11-11', dtFecUltAct = GETDATE()");
                consulta.AppendLine(" WHERE dtFinVigencia >= GETDATE()");
            }
            else
            {
                consulta.AppendLine("UPDATE " + esquema + ".[vishistoricos('CargasA','Cargas CDRs','Español')]");
                consulta.AppendLine(" set dtFinVigencia = '2079-01-01', dtFecUltAct = GETDATE()");
                consulta.AppendLine(" WHERE dtFinVigencia = '2011-11-11'");
            }

            return DSODataAccess.ExecuteNonQuery(consulta.ToString(), connStr);
        }

        protected bool EliminarDetalle(int mes, int anio, byte reconstruirIndices)
        {
            DateTime fechaInicial = new DateTime(anio, mes, 1);
            DateTime fechaFinal = fechaInicial.AddMonths(1).AddDays(-1);
            StringBuilder consulta = new StringBuilder();

            consulta.AppendLine("exec [EliminaDetalleXFecha]");
            consulta.AppendLine("@Schema = '" + esquema + "',  ");
            consulta.AppendLine("@FechaIni = '" + fechaInicial.ToString("yyyy-MM-dd 00:00:00") + "', ");
            consulta.AppendLine("@FechaFin = '" + fechaFinal.ToString("yyyy-MM-dd 23:59:59") + "', ");
            consulta.AppendLine("@reconstruirIndices = " + reconstruirIndices.ToString());

            return DSODataAccess.ExecuteNonQuery(consulta.ToString(), connStr);
        }
    }
}