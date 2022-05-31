using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.RepBimbo
{
    public partial class RepBimboMoviles : Page
    {
        protected DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        protected DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
        static Dictionary<string, string> param = new Dictionary<string, string>();
        private void CalculaFechasDeDashboard()
        {
            DateTime ldtFechaInicio = new DateTime();
            string lsfechaInicio = DSODataAccess.ExecuteScalar(ConsultaFechaMaximaDeDetallFactCDR()).ToString();
            if (DateTime.TryParse(lsfechaInicio, out ldtFechaInicio))
            {
                //NZ 20150319 Se establecio que siempre se mostrara el primer dia de cada mes.
                fechaInicio = new DateTime(ldtFechaInicio.Year, ldtFechaInicio.Month, 1);
                fechaFinal = fechaInicio.AddMonths(1).AddDays(-1);
            }

            Session["FechaInicio"] = fechaInicio.ToString("yyyy-MM-dd");
            Session["FechaFin"] = fechaFinal.ToString("yyyy-MM-dd");
        }
        private void LeeQueryString()
        {
            param.Clear();
            param.Add("Nav", string.Empty);
            param.Add("Sitio", string.Empty);
            param.Add("TDest", string.Empty);
            param.Add("Emple", string.Empty);
            param.Add("CenCos", string.Empty);
            param.Add("TipoLlam", string.Empty);
            param.Add("Concepto", string.Empty);
            param.Add("Exten", string.Empty);
            param.Add("Carrier", string.Empty);
            param.Add("NumMarc", string.Empty);
            param.Add("Linea", string.Empty);
            param.Add("NumGpoTronk", string.Empty);
            param.Add("CtaMae", string.Empty);
            param.Add("EqCelular", string.Empty);
            param.Add("Tel", string.Empty);
            param.Add("MesAnio", string.Empty);
            param.Add("MiConsumo", string.Empty);
            param.Add("Level", string.Empty);
            param.Add("TipoConsumo", string.Empty);

            for (int i = 0; i < param.Count; i++)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Request.QueryString[param.Keys.ElementAt(i)]))
                    {
                        param[param.Keys.ElementAt(i)] = Request.QueryString[param.Keys.ElementAt(i)];
                    }
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring " + param.Keys.ElementAt(i) + " en " + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //NZ Este codigo se debe incluir para que el backend se entere de que las fechas del frente cambiaron.
            (Master as KeytiaOH).ExtraerFechasRangeFrontToBack();
            LeeQueryString();
            if (!Page.IsPostBack && (ValidaConsultaFechasBD() || (Session["FechaInicio"].ToString() == "" && Session["FechaFin"].ToString() == "")))
            {
                if (Session["FechaInicio"].ToString() == "" && Session["FechaFin"].ToString() == "")
                {
                    #region Inicia los valores default de los controles de fecha
                    try
                    {
                        CalculaFechasDeDashboard();
                    }
                    catch (Exception ex)
                    {
                        throw new KeytiaWebException(
                            "Ocurrio un error al darle valores default a los campos de fecha en '" + Request.Path
                            + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                    }
                    #endregion Inicia los valores default de los controles de fecha
                }
            }
            else
            {
                //En este caso que es Moviles, aquí se ven meses completos, así que si se selecciona una fecha 
                //diferente el primer mes, se mueven internamente las fechas para que siempre sea mes completo.
                #region Fecha mes completo
                if (Session["FechaInicio"].ToString() != null && Session["FechaInicio"].ToString() != "")
                {
                    DateTime fechaAux = Convert.ToDateTime(Session["FechaInicio"].ToString());
                    DateTime fechaFinalAux = Convert.ToDateTime(Session["FechaFin"].ToString());
                    fechaInicio = new DateTime(fechaAux.Year, fechaAux.Month, 1);
                    fechaFinal = new DateTime(fechaFinalAux.Year, fechaFinalAux.Month, 1);
                    fechaFinal = fechaFinal.AddMonths(1).AddDays(-1);

                    Session["FechaInicio"] = fechaInicio.ToString("yyyy-MM-dd");
                    Session["FechaFin"] = fechaFinal.ToString("yyyy-MM-dd");
                }
                #endregion
            }
        }

        bool ValidaConsultaFechasBD()
        {
            bool consultarBD = false;
            if (string.IsNullOrEmpty(param["Nav"]))
            {
                if (Session["ListaNavegacion"] != null &&
                    (((List<MapNav>)Session["ListaNavegacion"]).LastOrDefault(x => x.URL.ToLower().Contains(Request.Path.ToLower()))) != null)
                {
                    consultarBD = false;
                }
                else { consultarBD = true; }
            }
            else if (param["Nav"] != string.Empty && !string.IsNullOrEmpty(param["Level"]))
            {
                if (Session["ListaNavegacion"] == null)
                {
                    consultarBD = true;
                }
                else if (((List<MapNav>)Session["ListaNavegacion"]).Count == 0)
                {
                    consultarBD = false;
                }
                else if ((((List<MapNav>)Session["ListaNavegacion"]).LastOrDefault(x => x.URL.ToLower().Contains(Request.Path.ToLower()))) != null)
                {
                    consultarBD = false;
                }
                else if ((((List<MapNav>)Session["ListaNavegacion"]).LastOrDefault(x => x.URL.ToLower().Contains(Request.Path.ToLower()))) == null)
                {
                    consultarBD = true;
                }
                else
                {
                    consultarBD = true;
                }
            }
            else
            {
                consultarBD = false;
            }

            return consultarBD;
        }


        public string ConsultaFechaMaximaDeDetallFactCDR()
        {
            StringBuilder lsb = new StringBuilder();
            //RM 20161214 Se modifico la consulta para que regrese una fecha por default, el primero del mes actual en dado caso de no encontrar una 
            lsb.Append("select isNull(Max(FechaInicio),'" + DateTime.Now.ToString("yyyy-MM-01 00:00:00") + "') as FechaInicio \n ");
            lsb.Append("from " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleFacturaCDR','Español')]  \n ");
            lsb.Append("where Carrier = 373 \n ");
            return lsb.ToString();
        }
        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            ExportXLS(".xlsx");
        }


        public void ExportXLS(string tipoExtensionArchivo)
        {
            CrearXLS(tipoExtensionArchivo);
        }

        protected void CrearXLS(string lsExt)
        {
            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                #region Exportar Reportes solo con tabla

                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\InventarioTelecom\ReporteTabla" + lsExt);
                lExcel.Abrir();

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                //Se quita logica de mas, puesto que solo se exportara una tabla. 
                //La plantilla que se usa para este reporte no llevara la sección de fechas no lleva fechas.
                #region Reporte Inventario de Equipo Telecom

                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Inventario de líneas móviles");

                DataTable ldt = QueryConsultaLinea();

                if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                {
                    DataView dvldt = new DataView(ldt);
                    ldt = dvldt.ToTable(false,
                        new string[] {"Extension",
                            "No Nomina",
                            "Nombre Completo",
                            "Codigo Empleado",
                            "Nombre Centro de Costos",
                            "Codigo Centro de Costos",
                            "Nombre Razon Social",
                            "Codigo Razon Social",
                            "Nombre Tipo Plan",
                            "Codigo Tipo Plan",
                            "Nombre Tipo Equipo",
                            "Codigo Tipo Equipo",
                            "Modelo Equipo",
                            "IMEI",
                            "Plan",
                            "Plazo Forzoso",
                            "RID",
                            "TopRID"
                            });

                    ldt.AcceptChanges();
                }

                if (ldt.Rows.Count > 0)
                {
                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                }

                #endregion Reporte Inventario de Equipo Telecom

                #endregion Exportar Reportes solo con tabla

                string psFileKey;
                string psTempPath;

                psFileKey = Guid.NewGuid().ToString();
                psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
                System.IO.Directory.CreateDirectory(psTempPath);

                string lsFileName = System.IO.Path.Combine(psTempPath, "cc." + psFileKey + ".temp" + lsExt);
                Session[psFileKey] = lsFileName;
                lExcel.FilePath = lsFileName;
                lExcel.SalvarComo();
                ExportarArchivo(lsExt, psFileKey, psTempPath, "Reporte" + "_" + "Equipos_Telecom");
            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrExportTo", e, lsExt);
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

        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {
            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        [WebMethod]
        public static object ListaMoviles() => JsonConvert.SerializeObject(ConsultaInventarioLinea());

        public static DataTable QueryConsultaLinea()
        {
            string Moviles = $@"
            declare @Where varchar(max)
            declare @TodosCampos varchar(max)
            set @Where = ''
            set @TodosCampos = 'null'
            if @TodosCampos <> 'null'
            begin
            set @Where = @Where + 'isnull([No Nomina],'''')+isnull([Nombre Completo],'''')+isnull([Nombre Centro de Costos],'''')+isnull([Nombre Razon Social],'''')
            +isnull([Nombre Tipo Plan],'''')+isnull([Nombre Tipo Equipo],'''')+isnull([Modelo Equipo],'''')+isnull([IMEI],'''')+isnull([Plan],'''')
            +isnull(convert(varchar,[Plazo Forzoso],103),'''') like ''%' + @TodosCampos + '%'' '
            end
            exec dbo.LineasTelcelTodosCamposRest @Schema='{DSODataContext.Schema}', @Fields = '[Extension],
            [No Nomina],
            [Nombre Completo],
            [Codigo Empleado],
            [Nombre Centro de Costos],
            [Codigo Centro de Costos],
            [Nombre Razon Social],
            [Codigo Razon Social],
            [Nombre Tipo Plan],
            [Codigo Tipo Plan],
            [Nombre Tipo Equipo],
            [Codigo Tipo Equipo],
            [Modelo Equipo],
            [IMEI],
            [Plan],
            [Plazo Forzoso]',
            @Where =@Where,
            @Group = '',
            @Order = '[Nombre Completo] Asc,[Nombre Centro de Costos] Asc,[No Nomina] Asc,[Extension] Asc,[Nombre Razon Social] Asc,[Nombre Tipo Plan] Asc,[Nombre Tipo Equipo] Asc,[Modelo Equipo] Asc,[IMEI] Asc,[Plan] Asc,[Plazo Forzoso] Asc',
            @OrderInv = '[Nombre Completo] Desc,[Nombre Centro de Costos] Desc,[No Nomina] Desc,[Extension] Desc,[Nombre Razon Social] Desc,[Nombre Tipo Plan] Desc,[Nombre Tipo Equipo] Desc,[Modelo Equipo] Desc,[IMEI] Desc,[Plan] Desc,[Plazo Forzoso] Desc',
            @Start = 0,
            @OrderDir = 'Asc',
            @Moneda = 'MXP',
            @Idioma = 'Español'";
            return DSODataAccess.Execute(Moviles);
        }
        public static List<Movil> ConsultaInventarioLinea()
        {
            DataTable dataTable = QueryConsultaLinea();
            List<Movil> ListMovil = new List<Movil>();
            int i = 1;
            foreach (DataRow item in dataTable.Rows)
            {
                ListMovil.Add(
                    new Movil()
                    {
                        recid = i.ToString(),
                        Extension = item["Extension"].ToString(),
                        NoNomina = item["No Nomina"].ToString(),
                        NombreCompleto = item["Nombre Completo"].ToString(),
                        CodigoEmpleado = item["Codigo Empleado"].ToString(),
                        NombreCentrodeCostos = item["Nombre Centro de Costos"].ToString(),
                        CodigoCentrodeCostos = item["Codigo Centro de Costos"].ToString(),
                        NombreRazonSocial = item["Nombre Razon Social"].ToString(),
                        CodigoRazonSocial = item["Codigo Razon Social"].ToString(),
                        NombreTipoPlan = item["Nombre Tipo Plan"].ToString(),
                        CodigoTipoPlan = item["Codigo Tipo Plan"].ToString(),
                        NombreTipoEquipo = item["Nombre Tipo Equipo"].ToString(),
                        CodigoTipoEquipo = item["Codigo Tipo Equipo"].ToString(),
                        ModeloEquipo = item["Modelo Equipo"].ToString(),
                        IMEI = item["IMEI"].ToString(),
                        Plan = item["Plan"].ToString(),
                        PlazoForzoso = item["Plazo Forzoso"].ToString(),
                        RID = item["RID"].ToString(),
                        TopRID = item["TopRID"].ToString()
                    });
                i++;
            }
            return ListMovil;
        }

    }

    public class Movil
    {
        public string recid { get; set; }
        public string Extension { get; set; }
        public string NoNomina { get; set; }
        public string NombreCompleto { get; set; }
        public string CodigoEmpleado { get; set; }
        public string NombreCentrodeCostos { get; set; }
        public string CodigoCentrodeCostos { get; set; }
        public string NombreRazonSocial { get; set; }
        public string CodigoRazonSocial { get; set; }
        public string NombreTipoPlan { get; set; }
        public string CodigoTipoPlan { get; set; }
        public string NombreTipoEquipo { get; set; }
        public string CodigoTipoEquipo { get; set; }
        public string ModeloEquipo { get; set; }
        public string IMEI { get; set; }
        public string Plan { get; set; }
        public string PlazoForzoso { get; set; }
        public string RID { get; set; }
        public string TopRID { get; set; }

    }
}